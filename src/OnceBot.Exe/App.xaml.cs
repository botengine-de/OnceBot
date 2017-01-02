using Bib3;
using OnceBot.UI;
using System;
using System.Text;
using System.Windows;

namespace OnceBot.Exe
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		public App()
		{
			ExeMain.ServiceDelegate = new Service.Service().ClientRequest;
		}

		private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
		{
			try
			{
				var FilePath = ExeMain.AssemblyDirectoryPath.PathToFilesysChild(DateTime.Now.SictwaiseKalenderString(".", 0) + " Exception");

				FilePath.WriteToFileAndCreateDirectoryIfNotExisting(Encoding.UTF8.GetBytes(e.Exception.SictString()));

				var Message = "exception written to file: " + FilePath;

				MessageBox.Show(Message, Message, MessageBoxButton.OK, MessageBoxImage.Exclamation);
			}
			catch (Exception PersistException)
			{
				Bib3.FCL.GBS.Extension.MessageBoxException(PersistException);
			}

			Bib3.FCL.GBS.Extension.MessageBoxException(e.Exception);
		}
	}
}
