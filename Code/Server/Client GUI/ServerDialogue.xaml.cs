using System.Windows;

namespace Client_GUI
{
	// ReSharper disable once RedundantExtendsListEntry
	public partial class ServerDialogue : Window
	{
		public MainWindow callBack;
		public ServerDialogue(MainWindow callBack_)
		{
			InitializeComponent();
			callBack = callBack_;
		}

		private void Connect(object sender_, RoutedEventArgs e_)
		{
			int x;
			int.TryParse(portBox.Text, out x);
			if (x == 0) return;
			callBack.Connect(ipBox.Text, x);
			Close();
		}
	}
}
