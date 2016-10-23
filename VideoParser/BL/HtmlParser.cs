using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace VideoParser.BL
{
	public static class HtmlParser
	{
		public static string GetHtml(string url)
		{
			if (string.IsNullOrWhiteSpace(url))
				throw new Exception("Пустая ссылка");

			if (!url.Contains("http://"))
				url = "http://" + url;

			string htmlCode = string.Empty;
			try {
				HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
				using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
				{
					if (response.StatusCode == HttpStatusCode.OK)
					{
						Stream receiveStream = response.GetResponseStream();
						StreamReader readStream = null;

						if (response.CharacterSet == null)
						{
							readStream = new StreamReader(receiveStream);
						}
						else
						{
							readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
						}

						htmlCode = readStream.ReadToEnd();
						readStream.Close();
					}
					else
						throw new Exception("Наверное, неправильная ссылка");
				}
			}
			catch
			{
				throw new Exception("Наверное, неправильная ссылка");
			}
			
			return htmlCode;
		}

		public static string GetFrameUrl(string htmlCode)
		{
			int index = htmlCode.IndexOf("http://player.novy.tv/embed");
			string tmp = htmlCode.Remove(0, index);
			string result = tmp.Remove(tmp.IndexOf("\""));
			return result;
		}

		public static string GetVideoFileUrl(string url)
		{
			var html = GetHtml(url);
			var frameUrl = GetFrameUrl(html);
			var videoHtml = GetHtml(frameUrl);

			string fileName = "mq.mp4";
			int index = videoHtml.IndexOf(fileName);
			string tmp = videoHtml.Remove(index + fileName.Length);
			string result = tmp.Remove(0, tmp.LastIndexOf("\"") + 1);
			return result;
		}
	}
}
