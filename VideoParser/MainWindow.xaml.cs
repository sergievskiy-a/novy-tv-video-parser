using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VideoParser.BL;

namespace VideoParser
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private WebClient client;

		public MainWindow()
		{
			client = new WebClient();
			InitializeComponent();
		}

		private void getVideoButton_Click(object sender, RoutedEventArgs e)
		{
			string url = pageUrlTextBox.Text;
			try
			{
				var videoUrl = HtmlParser.GetVideoFileUrl(url);
				videoUrlTextBox.Text = videoUrl;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}			
		}

		private void pageUrlTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			if(string.IsNullOrEmpty(pageUrlTextBox.Text))
				getVideoButton.IsEnabled = false;
			else
				getVideoButton.IsEnabled = true;
		}

		private void videoUrlTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			if(!string.IsNullOrWhiteSpace(videoUrlTextBox.Text))
			{
				copyLinkButton.IsEnabled = true;
				openInBrowserButton.IsEnabled = true;
				downloadButton.IsEnabled = true;
			}
		}

		private void copyLinkButton_Click(object sender, RoutedEventArgs e)
		{
			Clipboard.SetText(videoUrlTextBox.Text);
			MessageBox.Show("Текст скопирован:)");
		}

		private void openInBrowserButton_Click(object sender, RoutedEventArgs e)
		{
			Process proc = new Process();
			proc.StartInfo.FileName = videoUrlTextBox.Text;
			proc.StartInfo.UseShellExecute = true;
			proc.Start();
		}

		private void downloadButton_Click(object sender, RoutedEventArgs e)
		{
			startDownload();
			downloadButton.IsEnabled = false;
			downloadGrid.Visibility = Visibility.Visible;
		}

		private void startDownload()
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.Filter = "Video file (*.mp4)|*.mp4";
			saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			if (saveFileDialog.ShowDialog() == true)
			{ 
				var videoFileUrl = videoUrlTextBox.Text;
				client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
				client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);
				client.DownloadFileAsync(new Uri(videoFileUrl), saveFileDialog.FileName);
				downloadCancelbutton.IsEnabled = true;
			}
		}
		void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
		{
			double megabytesIn = double.Parse(e.BytesReceived.ToString()) / 1048576.00;
			double totalmegabytes = double.Parse(e.TotalBytesToReceive.ToString()) / 1048576;
			var percentage = decimal.Parse(e.ProgressPercentage.ToString());
			downloadTotalLabel.Text = $"{megabytesIn.ToString("F1")} of {totalmegabytes.ToString("F0")}Mb";
			downloadProgressBar.Value = int.Parse(Math.Truncate(percentage).ToString());
		}
		void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
		{
			downloadCancelbutton.IsEnabled = false;
			downloadButton.IsEnabled = true;
			downloadButtonTextBlock.IsEnabled = true;
			if (e.Cancelled)
			{
				MessageBox.Show("Загрузка отменена!");
				downloadTotalLabel.Text = "Отменено";
			}
			else
			{
				MessageBox.Show("Загрузка успешно завершена!");
				downloadTotalLabel.Text = "Готово";
			}
		}

		private void downloadCancelbutton_Click(object sender, RoutedEventArgs e)
		{
			client.CancelAsync();
		}
	}
}
