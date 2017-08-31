using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Client_GUI
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow
	{
		public static string text = "";
		private static string _ip = "";
		private static int _port;
		private ServerDialogue _sd;
		public MainWindow()
		{
			InitializeComponent();
			var t = new Task(() => Run(logBox));
			t.Start();
		}

		public void Run(TextBox logBox_)
		{
			Console.WriteLine("Thread running!");
			WriteLine("Please Login");

			// We have do do it this way because another thread
			//	owns LogBox and ChatBox components.
			logBox.Dispatcher.Invoke(
				DispatcherPriority.Normal,
				new Action(() => logBox.IsReadOnly = true));
			chatBox.Dispatcher.Invoke(
				DispatcherPriority.Normal,
				new Action(() => chatBox.IsReadOnly = true));
			var ip = WaitForIPAsync();
			var port = _port;
		    TcpClient client;
		    while (true)
		    {
		        try
		        {
                    WriteLine("Connecting...");
                    var t = new TcpClient(ip, port);
		            client = t;
		            break;
		        }
		        // ReSharper disable once RedundantCatchClause
		        catch
		        {
		            WriteLine("Invalid IP or Port. Please try again.");
		            _ip = "";
		            ip = WaitForIPAsync();
		        }
            }
			try
			{
                chatBox.Dispatcher.Invoke(
                    DispatcherPriority.Normal,
                    new Action(() => chatBox.IsReadOnly = false));
                Clear();
                WriteLine("Connected to Server.");
				Stream s = client.GetStream();
				var sr = new StreamReader(s);
				var sw = new StreamWriter(s) {AutoFlush = true};
				var t = new Thread(() => GetInputAsync(sr, logBox));
				t.Start();
				while (true)
				{
					if (text != "")
					{
						if (text == "") { WriteLine("Cannot be blank! "); continue; }
						sw.WriteLine(text);
						text = "";
					}
					Thread.Sleep(500);
				}
			}
			finally
			{
				client.Close();
			}
		}
		public async void GetInputAsync(StreamReader sr_, TextBox logBox_)
		{
			while (true)
			{
				var x = await sr_.ReadLineAsync();
				if (x.StartsWith("/"))
				{
					if (x == "/clr")
					{
						Clear();
						continue;
					}
				}
				WriteLine(x);
				text = "";
			}
			// ReSharper disable once FunctionNeverReturns
		}

		public void WriteLine(string text_)
		{
			logBox.Dispatcher.Invoke(
				DispatcherPriority.Normal,
				new Action(() => logBox.AppendText(text_ + "\n")));
		}
		public void Write(string text_)
		{
			logBox.Dispatcher.Invoke(
				DispatcherPriority.Normal,
				new Action(() => logBox.AppendText(text_)));
		}
		public void Clear(string x_ = null)
		{
			logBox.Dispatcher.Invoke(
				DispatcherPriority.Normal,
				new Action(() => logBox.Clear()));
		}

		public static string WaitForInputAsync()
		{
			while (true)
			{
				if (text != "")
				{
					string x = text;
					text = "";
					return x;
				}
				Thread.Sleep(500);
			}
		}

		public static string WaitForIPAsync()
		{
			while (true)
			{
				if (_ip != "")
				{
					string x = _ip;
					return x;
				}
				Thread.Sleep(500);
			}
		}

		private void Click(object sender_, RoutedEventArgs e_)
		{
			text = chatBox.Text;
			chatBox.Clear();
		}

		private void Login_Click(object sender_, RoutedEventArgs e_)
		{
			var usr = usernameBox.Text;
			var pass = passwordBox.Password;
			_sd = new ServerDialogue(this);
			_sd.Show();
		    if (usr.Contains(":") || pass.Contains(":"))
		    {
		        // Ask for a different Username or Password.
                WriteLine("Username and Password cannot contain ':'! Please change this and try again.");
                return;
		    }
			usernameBox.IsEnabled = false;
			passwordBox.IsEnabled = false;
			Login.IsEnabled = false;
            // Push Username / Password to Server
		    text = usr + ":" + pass;
		}

		public void Connect(string ip_, int port_)
		{
			_ip = ip_;
			_port = port_;
		}

		private void logBox_TextChanged(object sender, TextChangedEventArgs e)
		{

		}
	}
}