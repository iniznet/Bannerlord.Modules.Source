using System;
using System.Net;
using System.Threading.Tasks;

namespace TaleWorlds.Library.Http
{
	public class DotNetHttpDriver : IHttpDriver
	{
		IHttpRequestTask IHttpDriver.CreateHttpPostRequestTask(string address, string postData, bool withUserToken)
		{
			return new HttpPostRequest(address, postData);
		}

		IHttpRequestTask IHttpDriver.CreateHttpGetRequestTask(string address, bool withUserToken)
		{
			return new HttpGetRequest(address);
		}

		async Task<string> IHttpDriver.HttpGetString(string url, bool withUserToken)
		{
			return await new WebClient().DownloadStringTaskAsync(url);
		}

		async Task<string> IHttpDriver.HttpPostString(string url, string postData, bool withUserToken)
		{
			return await new WebClient().UploadStringTaskAsync(url, postData);
		}

		async Task<byte[]> IHttpDriver.HttpDownloadData(string url)
		{
			return await new WebClient().DownloadDataTaskAsync(url);
		}
	}
}
