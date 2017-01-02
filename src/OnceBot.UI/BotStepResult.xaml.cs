using Bib3;
using Bib3.Geometrik;
using Microsoft.Win32;
using System;
using System.Linq;
using System.Windows.Controls;

namespace OnceBot.UI
{
	/// <summary>
	/// Interaction logic for BotStepResult.xaml
	/// </summary>
	public partial class BotStepResult : UserControl
	{
		public BotStepResult()
		{
			InitializeComponent();
		}

		OnceBot.BotStepResult presented;

		public void Present(OnceBot.BotStepResult presented)
		{
			this.presented = presented;

			GeneralMessageTextBox.Text = presented?.UIMessageText;

			StartTimeCalView.Text = presented?.StartTime.ToLongTimeString();
			DurationView.Text = ((int?)(presented?.EndTime - presented?.StartTime)?.TotalMilliseconds).ToString();

			WindowRasterSetMatchReport.Text =
				string.Join(Environment.NewLine,
				presented?.RasterSetPatternMatch
				?.GroupBy(match => match.SourcePatternId)
				?.Select(group => group.Key + " : " + group.Count() + " x : " + string.Join("; ", group.Select(match => match.Area.Center().RenderForUI()))) ?? new string[0]);
		}

		static string FileName(OnceBot.BotStepResult step) =>
			step?.StartTime.SictwaiseKalenderString(".", 3);

		string WindowClientRasterFileName =>
			FileName(presented) + ".screenshot.png";

		private void WindowClientRasterWriteToFileButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			var dlg = new SaveFileDialog
			{
				FileName = WindowClientRasterFileName,
			};

			if (dlg.ShowDialog() ?? false)
				WindowClientRasterWriteToFile(dlg.FileName);
		}

		private void WindowClientRasterWriteToFileButton_Drop(object sender, System.Windows.DragEventArgs e)
		{
			WindowClientRasterWriteToFile(Bib3.FCL.Glob.DataiPfaadAlsKombinatioonAusSctandardPfaadUndFileDrop(WindowClientRasterFileName, e));
		}

		void WindowClientRasterWriteToFile(string filePath)
		{
			byte[] png = null;

			var windowClientRaster = presented?.WindowClientRaster;

			if (windowClientRaster.HasValue)
				png = Bib3.FCL.Extension.AsListOctetWindowMediaImagingPng(
				Bib3.FCL.Extension.BitmapSourceB8G8R8A8FromRasterO8R8G8B8HeightOut(windowClientRaster.Value.Key, windowClientRaster.Value.Value, 4, 4));

			Bib3.Glob.ScraibeInhaltNaacDataiPfaad(filePath, png ?? new byte[0]);
		}
	}
}
