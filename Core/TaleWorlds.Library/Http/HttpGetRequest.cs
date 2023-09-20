using System;
using System.Net;
using System.Net.Http;

namespace TaleWorlds.Library.Http
{
	public class HttpGetRequest : IHttpRequestTask
	{
		public HttpRequestTaskState State { get; private set; }

		public bool Successful { get; private set; }

		public string ResponseData { get; private set; }

		public HttpStatusCode ResponseStatusCode { get; private set; }

		public Exception Exception { get; private set; }

		public HttpGetRequest(HttpClient httpClient, string address)
			: this(httpClient, address, new Version("1.1"))
		{
		}

		public HttpGetRequest(HttpClient httpClient, string address, Version version)
		{
			this._versionToUse = version;
			this._address = address;
			this._httpClient = httpClient;
			this.State = HttpRequestTaskState.NotStarted;
			this.ResponseData = "";
			this.ResponseStatusCode = HttpStatusCode.OK;
		}

		private void SetFinishedAsSuccessful(string responseData, HttpStatusCode statusCode)
		{
			this.Successful = true;
			this.ResponseData = responseData;
			this.ResponseStatusCode = statusCode;
			this.State = HttpRequestTaskState.Finished;
		}

		private void SetFinishedAsUnsuccessful(Exception e)
		{
			this.Successful = false;
			this.Exception = e;
			this.State = HttpRequestTaskState.Finished;
		}

		public void Start()
		{
			this.DoTask();
		}

		private async void DoTask()
		{
			this.State = HttpRequestTaskState.Working;
			try
			{
				using (HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, this._address))
				{
					requestMessage.Version = this._versionToUse;
					requestMessage.Headers.Add("Accept", "application/json");
					requestMessage.Headers.Add("UserAgent", "TaleWorlds Client");
					HttpResponseMessage httpResponseMessage = await this._httpClient.SendAsync(requestMessage);
					using (HttpResponseMessage response = httpResponseMessage)
					{
						Console.WriteLine(string.Concat(new object[] { "Protocol version used for get request to ", this._address, " is: ", response.Version }));
						using (HttpContent content = response.Content)
						{
							this.SetFinishedAsSuccessful(await content.ReadAsStringAsync(), response.StatusCode);
						}
						HttpContent content = null;
					}
					HttpResponseMessage response = null;
				}
				HttpRequestMessage requestMessage = null;
			}
			catch (Exception ex)
			{
				this.SetFinishedAsUnsuccessful(ex);
			}
		}

		private const int BufferSize = 1024;

		private HttpClient _httpClient;

		private readonly string _address;

		private Version _versionToUse;
	}
}
