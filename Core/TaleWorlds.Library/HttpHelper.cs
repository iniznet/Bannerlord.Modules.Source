using System;
using System.Threading.Tasks;
using TaleWorlds.Library.Http;

namespace TaleWorlds.Library
{
	public static class HttpHelper
	{
		public static Task<string> DownloadStringTaskAsync(string url)
		{
			return HttpDriverManager.GetDefaultHttpDriver().HttpGetString(url, false);
		}

		public static Task<byte[]> DownloadDataTaskAsync(string url)
		{
			return HttpDriverManager.GetDefaultHttpDriver().HttpDownloadData(url);
		}
	}
}
