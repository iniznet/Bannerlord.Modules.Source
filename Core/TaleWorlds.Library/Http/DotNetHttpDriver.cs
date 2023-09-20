using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TaleWorlds.Library.Http
{
	public class DotNetHttpDriver : IHttpDriver
	{
		public DotNetHttpDriver()
		{
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
			this._httpClient = new HttpClient();
		}

		IHttpRequestTask IHttpDriver.CreateHttpPostRequestTask(string address, string postData, bool withUserToken)
		{
			return new HttpPostRequest(this._httpClient, address, postData);
		}

		IHttpRequestTask IHttpDriver.CreateHttpGetRequestTask(string address, bool withUserToken)
		{
			return new HttpGetRequest(this._httpClient, address);
		}

		async Task<string> IHttpDriver.HttpGetString(string url, bool withUserToken)
		{
			return await this._httpClient.GetStringAsync(url);
		}

		async Task<string> IHttpDriver.HttpPostString(string url, string postData, string mediaType, bool withUserToken)
		{
			HttpResponseMessage httpResponseMessage = await this._httpClient.PostAsync(url, new StringContent(postData, Encoding.Unicode, mediaType));
			string text;
			using (HttpResponseMessage response = httpResponseMessage)
			{
				using (HttpContent content = response.Content)
				{
					text = await content.ReadAsStringAsync();
				}
			}
			return text;
		}

		async Task<byte[]> IHttpDriver.HttpDownloadData(string url)
		{
			return await this._httpClient.GetByteArrayAsync(url);
		}

		private HttpClient _httpClient;
	}
}
