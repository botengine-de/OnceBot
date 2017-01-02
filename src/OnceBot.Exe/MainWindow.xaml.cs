using System.Windows;
using System.Windows.Input;

namespace OnceBot.Exe
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public string TitleComputed =>
			"OnceBot v" + (TryFindResource("AppVersionId") ?? "");

		public MainWindow()
		{
			InitializeComponent();
		}

		private void Window_KeyDown(object sender, KeyEventArgs e)
		{
			MainControl?.ProcessInput();
		}

	}
}
