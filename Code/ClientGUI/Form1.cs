using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientGUI
{
	public partial class Client : Form
	{
		public static string text = "";
		public Client()
		{
			InitializeComponent();
			Task t = new Task(() => Run(logBox));
			t.Start();
		}
		private void Send_Click(object sender, EventArgs e)
		{
			text = chatBox.Text;
			chatBox.Clear();
		}

		private void TextBoxKeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				text = chatBox.Text;
				chatBox.Clear();
			}
		}

		public void Run(TextBox logBox)
		{
			Console.WriteLine("Thread running!");
			WriteLine("Enter the servers IP Address: ");
			string ip = WaitForInputAsync();

			TcpClient client = new TcpClient(ip, 5948);
			WriteLine("Enter Mode (1/2): ");
			if (WaitForInputAsync() == "1")
			{
				try
				{
					Clear();
					Stream s = client.GetStream();
					StreamReader sr = new StreamReader(s);
					StreamWriter sw = new StreamWriter(s);
					sw.AutoFlush = true;
					Thread t = new Thread(new ThreadStart(() => GetInputAsync(sr, logBox)));
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
					s.Close();
				}
				finally
				{
					client.Close();
				}
			}
		}
		public async void GetInputAsync(StreamReader sr, TextBox logBox)
		{
			while (true)
			{
				string x = await sr.ReadLineAsync();
				if (x.StartsWith("/"))
				{
					if (x == "/clr")
					{
						Console.Clear();
						continue;
					}
				}
				WriteLine(x);
			}
		}

		public void WriteLine(string text)
		{
			if (this.logBox.InvokeRequired)
			{
				SetTextCallback d = new SetTextCallback(WriteLine);
				this.Invoke(d, new object[] { text });
			}
			else
			{
				this.logBox.AppendText(text + "\n");
			}
		}
		public void Write(string text)
		{
			if (this.logBox.InvokeRequired)
			{
				SetTextCallback d = new SetTextCallback(Write);
				this.Invoke(d, new object[] { text });
			}
			else
			{
				this.logBox.AppendText(text);
			}
		}
		public void Clear(string x = null)
		{
			if (this.logBox.InvokeRequired)
			{
				SetTextCallback d = new SetTextCallback(Clear);
				this.Invoke(d, new object[] { "" });
			}
			else
			{
				this.logBox.Clear();
			}
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

		delegate void SetTextCallback(string text);
	}
}