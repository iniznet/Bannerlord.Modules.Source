using System;
using System.Net;
using System.Threading.Tasks;

namespace TaleWorlds.Library.Http
{
	// Token: 0x020000A3 RID: 163
	public class DotNetHttpDriver : IHttpDriver
	{
		// Token: 0x060005F5 RID: 1525 RVA: 0x00012F30 File Offset: 0x00011130
		IHttpRequestTask IHttpDriver.CreateHttpPostRequestTask(string address, string postData, bool withUserToken)
		{
			return new HttpPostRequest(address, postData);
		}

		// Token: 0x060005F6 RID: 1526 RVA: 0x00012F39 File Offset: 0x00011139
		IHttpRequestTask IHttpDriver.CreateHttpGetRequestTask(string address, bool withUserToken)
		{
			return new HttpGetRequest(address);
		}

		// Token: 0x060005F7 RID: 1527 RVA: 0x00012F44 File Offset: 0x00011144
		async Task<string> IHttpDriver.HttpGetString(string url, bool withUserToken)
		{
			return await new WebClient().DownloadStringTaskAsync(url);
		}

		// Token: 0x060005F8 RID: 1528 RVA: 0x00012F8C File Offset: 0x0001118C
		async Task<string> IHttpDriver.HttpPostString(string url, string postData, bool withUserToken)
		{
			return await new WebClient().UploadStringTaskAsync(url, postData);
		}

		// Token: 0x060005F9 RID: 1529 RVA: 0x00012FDC File Offset: 0x000111DC
		async Task<byte[]> IHttpDriver.HttpDownloadData(string url)
		{
			return await new WebClient().DownloadDataTaskAsync(url);
		}
	}
}
