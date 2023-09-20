using System;
using System.Threading.Tasks;
using TaleWorlds.Library.Http;

namespace TaleWorlds.Library
{
	// Token: 0x02000033 RID: 51
	public static class HttpHelper
	{
		// Token: 0x06000197 RID: 407 RVA: 0x000062DA File Offset: 0x000044DA
		public static Task<string> DownloadStringTaskAsync(string url)
		{
			return HttpDriverManager.GetDefaultHttpDriver().HttpGetString(url, false);
		}

		// Token: 0x06000198 RID: 408 RVA: 0x000062E8 File Offset: 0x000044E8
		public static Task<byte[]> DownloadDataTaskAsync(string url)
		{
			return HttpDriverManager.GetDefaultHttpDriver().HttpDownloadData(url);
		}
	}
}
