using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace Client
{
	class Program
	{
		public static void Main()
		{
			Console.WriteLine("Enter the servers IP Address: ");
			var ip = Console.ReadLine();
			if (ip == null) return;
			var client = new TcpClient(ip, 5948);
			Console.WriteLine("Enter Mode (1/2): ");
			var readLine = Console.ReadLine();
			if (readLine == null || readLine.ToUpper() != "1") return;
			try
			{
				Console.Clear();
				Stream s = client.GetStream();
				var sr = new StreamReader(s);
				var sw = new StreamWriter(s) {AutoFlush = true};
				var t = new Thread(() => GetInputAsync(sr));
				t.Start();
				while (true)
				{	
					Console.Write("> ");
					var command = Console.ReadLine();
					if (command == "") { Console.WriteLine("Cannot be blank! "); continue; }
					sw.WriteLine(command);
				}
			}
			finally
			{
				client.Close();
			}
		}
		public async static void GetInputAsync(StreamReader sr)
		{
			while (true)
			{
				var x = await sr.ReadLineAsync();
				if (x.StartsWith("/"))
				{
					if (x == "/clr")
					{
						Console.Clear();
						continue;
					}
				}
				Console.WriteLine(x);
				Console.Write("> ");
			}
			// ReSharper disable once FunctionNeverReturns
		}
	}
}