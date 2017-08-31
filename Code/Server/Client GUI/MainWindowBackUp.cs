//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Data;
//using System.Windows.Documents;
//using System.Windows.Input;
//using System.Windows.Media;
//using System.Windows.Media.Imaging;
//using System.Windows.Navigation;
//using System.Windows.Shapes;

//namespace Client_GUI
//{
//	/// <summary>
//	/// Interaction logic for MainWindow.xaml
//	/// </summary>
//	public partial class MainWindow : Window
//	{
//		public bool logTime = false;
//		ServerDialogue sd;

//		public MainWindow()
//		{
//			InitializeComponent();
//		}

//		private void Login_Click(object sender, RoutedEventArgs e)
//		{
//			string usr = usernameBox.Text;
//			string pass = passwordBox.Password;
//			sd = new ServerDialogue();
//			sd.Show();
//			LogLine("User " + usr + " tried to login with password " + pass);
//		}

//		private void LogLine(string _text)
//		{
//			if (logTime)
//			{
//				logBox.Text += "\n[" + DateTime.Now + "] " + _text;
//				return;
//			}
//			logBox.Text += "\n " + _text;
//		}

//	}
//}
