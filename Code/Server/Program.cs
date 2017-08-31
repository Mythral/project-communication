#define log

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;


namespace Server
{
	internal class Program
	{
		// static Dictionary<float, string> authTokens;		// Something that we ended up not needing but we may need later on.
		private static IPEndPoint localPort;

		private static TcpListener listener;

		private static Dictionary<int, Thread> threadDb = new Dictionary<int, Thread>();
		//private static List<User> userDB;

		private static Dictionary<string, User> userDb = new Dictionary<string, User>();
		public static Dictionary<string, Chatroom> ChatroomDb = new Dictionary<string, Chatroom>();
		private static Thread userSaver;

		//private static Dictionary<string, int> userConversionDB = new Dictionary<string, int>();
		// Allows a Users ID to be found with a Username

		private const int ClientLimit = 51;
		private static int newThreadId;
		private static string ipAddress = ":5948";
		public static Users Users;
		private static XmlWriter xmlWriter;

		public static string GetLocalIPAddress()
		{
			var host = Dns.GetHostEntry(Dns.GetHostName());
			foreach (var ip in host.AddressList)
			{
				if (ip.AddressFamily == AddressFamily.InterNetwork)
				{
					return ip.ToString();
				}
			}
			throw new Exception("Local IP Address Not Found!");
		}

		public static IPEndPoint CreateIPEndPoint(string endPoint)
		{
			string[] ep = endPoint.Split(':');
			if (ep.Length != 2) throw new FormatException("Invalid endpoint format");
			IPAddress ip;
			if (!IPAddress.TryParse(ep[0], out ip))
			{
				throw new FormatException("Invalid ip-adress");
			}
			int port;
			if (!int.TryParse(ep[1], NumberStyles.None, NumberFormatInfo.CurrentInfo, out port))
			{
				throw new FormatException("Invalid port");
			}
			return new IPEndPoint(ip, port);
		}

		private static void Main()
		{
			Users = new Users();
			Users.User = new List<User>();
			ipAddress = GetLocalIPAddress() + ipAddress;

			localPort = CreateIPEndPoint(ipAddress);
			listener = new TcpListener(localPort);
			listener.Start();
#if log
			Console.WriteLine("Server bound to {0}", localPort);
#endif
			for (var i = 0; i < ClientLimit; i++)
			{
#if log
				Console.WriteLine("Client {0} created!", i);
#endif
				//		 The whole Username system is broken, ID's are more or less pointless, we could just keep them a
				//			a property of User if we end up needing them, but serializing this is ridiculous, keeping
				//			threads based on UserDB is ridiculous, and don't get me started on UserConversionDB, its is
				//			so broken. So lets do something else
				//
				//			We can have a DB of threads that is initialized at startup and a DB of user's that is read
				//			from an XML file. Some thing's will need to be changed in Service() but it shouldn't be too
				//			messy.

				// ReSharper disable once AccessToModifiedClosure
				threadDb.Add(i, new Thread(Service));
				threadDb[i].Start();
				newThreadId = i;
			}

			Console.WriteLine("Initializing XML.");
			if (File.Exists(@"C:\CommUsers.xml"))
			{
				XmlSerializer reader = new XmlSerializer(typeof(Users));
				StreamReader file = new StreamReader(@"C:\CommUsers.xml");
				Users = (Users) reader.Deserialize(file);
				foreach (var user in Users.User)
				{
					userDb.Add(user.Username, user);
				}
				file.Close();
			}
			Console.WriteLine("XML Initialized");
			SaveUsers();
		}

		public static void SaveUsers()
		{
			while (true)
			{
				try
				{
					Thread.Sleep(10000);
					Type[] types = {typeof (User)};
					XmlSerializer writer = new XmlSerializer(typeof (Users), types);
					FileStream file = new FileStream(@"C:\CommUsers.xml", FileMode.Create);
					writer.Serialize(file, Users);
					file.Close();
				}
				catch (Exception)
				{
					Console.WriteLine("Users Saving Failed!");
					throw;
				}
			}
		}

		private static async void Service()
		{
			while (true)
			{
				var soc = listener.AcceptSocket();
#if log
				Console.WriteLine("Connected: {0}", soc.RemoteEndPoint);
#endif
				try
				{
					Stream s = new NetworkStream(soc);
					var sr = new StreamReader(s);
					var sw = new StreamWriter(s) { AutoFlush = true };
					string response;
					string username;
				    string[] xy;
					while (true)
					{
						response = sr.ReadLine();
                        xy = response?.Split(Convert.ToChar(":"));
						username = xy[0];
						if (string.IsNullOrEmpty(xy[0])) continue;
						string passwordHash = GenerateSaltedHash(xy[1], xy[0]); ;
                        if (userDb.ContainsKey(xy[0]))
						{
							// Attempt Login
							if (userDb[username].HashedPassword != passwordHash)
							{
								sw.WriteLine("Invalid Password! Please Restart your program");
								continue;
							}
						}
                        else
                        {
							AddUserToDatabase(username);
							Console.WriteLine("Username: {0}", username);
						}
						userDb[username].Sw = sw;
						userDb[username].HashedPassword = passwordHash;

						break;
					}
					while (true)
					{
						sw.WriteLine("Hello {0}. Enter the name Chatroom you would like to join: ", username);

						response = sr.ReadLine();
						string chatroomName = response;
						if (string.IsNullOrEmpty(response)) continue;
						if (response.Length >= 10)
						{
							sw.WriteLine("Chatroom name has to be less than 10 characters.");
							continue;
						}
						if (!ChatroomDb.ContainsKey(response))
						{
							sw.WriteLine("{0} does not exist would you like to create this Chatroom? (Y/N).", 
								response);
							response = sr.ReadLine();
							if (response?.ToUpper() == "Y")
							{
								ChatroomDb.Add(chatroomName, new Chatroom());
							}
							else
							{
								continue;
							}
						}
						sw.WriteLine("/clr");
						sw.WriteLine("Joining {0}. Enter a message to send to the Chatroom: ", chatroomName);
						var usr = userDb[username];
						ChatroomDb[chatroomName].Users.Add(usr);
						usr.Chatroom = ChatroomDb[chatroomName];
						ChatroomDb[chatroomName].MessageUsers(username + " joined the Chatroom!");
						break;
					}
					var u = userDb[username];

					// We need to implement other threads to constantly check the Users Stream to recieve messages.
					while (true)
					{
						try
						{
							string x = await sr.ReadLineAsync();
							sr.DiscardBufferedData();
							u.Chatroom.MessageUsers(username + ": " + x);
						}
						catch (Exception)
						{
							u.Chatroom.MessageUsers(username + " left the Chatroom!");
						}
					}
				}
				catch (Exception e)
				{
#if log
					Console.WriteLine(e.Message);
#endif
				}
#if log
				Console.WriteLine("Disconnected: " + soc.RemoteEndPoint);
#endif
				newThreadId++;
				threadDb.Add(newThreadId, new Thread(Service));
				threadDb[newThreadId].Start();
			}
			// ReSharper disable once FunctionNeverReturns
		}

		private static string GenerateSaltedHash(string clearText, string salt)
		{
			HashAlgorithm algorithm = new SHA256Managed();

			byte[] plainText = Encoding.UTF8.GetBytes(clearText);
			byte[] saltText = Encoding.UTF8.GetBytes(salt);

			byte[] plainTextWithSaltBytes =
			  new byte[plainText.Length + saltText.Length];

			for (int i = 0; i < plainText.Length; i++)
			{
				plainTextWithSaltBytes[i] = plainText[i];
			}
			for (int i = 0; i < salt.Length; i++)
			{
				plainTextWithSaltBytes[plainText.Length + i] = saltText[i];
			}

			return Convert.ToBase64String(algorithm.ComputeHash(plainTextWithSaltBytes));
		}

		private static void AddUserToDatabase(string u)
		{
			User x = new User {Username = u};
			userDb.Add(u, x);
			Users.User.Add(x);
		}
	}

	[XmlRoot("UserList")]
	[XmlInclude(typeof(User))]
	public class Users
	{
		[XmlArray("UserArray")]
		[XmlArrayItem("UserItem")]
		public List<User> User;
	}


	[XmlType("User")]
	public class User
	{
		[XmlAttribute("Username", DataType = "string")]
		public string Username;
		[XmlAttribute("HashedPassword")]
		public string HashedPassword;
		[XmlIgnore]
		public List<string> Stream = new List<string>();
		[XmlIgnore]
		public StreamWriter Sw;
		[XmlIgnore]
		public Chatroom Chatroom;

		public void WriteToStream(string text)
		{
			Sw.WriteLine(text);
		}
	}

	public class Chatroom
	{
		public List<User> Users = new List<User>();

		public void MessageUsers(string message)
		{
			foreach (var u in Users)
			{
				u.WriteToStream(message);
			}
		}
	}
}