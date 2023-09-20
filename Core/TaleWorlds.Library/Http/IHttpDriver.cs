using System;
using System.Threading.Tasks;

namespace TaleWorlds.Library.Http
{
	// Token: 0x020000A9 RID: 169
	public interface IHttpDriver
	{
		// Token: 0x0600061F RID: 1567
		Task<string> HttpGetString(string url, bool withUserToken);

		// Token: 0x06000620 RID: 1568
		Task<string> HttpPostString(string url, string postData, bool withUserToken);

		// Token: 0x06000621 RID: 1569
		Task<byte[]> HttpDownloadData(string url);

		// Token: 0x06000622 RID: 1570
		IHttpRequestTask CreateHttpPostRequestTask(string address, string postData, bool withUserToken);

		// Token: 0x06000623 RID: 1571
		IHttpRequestTask CreateHttpGetRequestTask(string address, bool withUserToken);
	}
}
