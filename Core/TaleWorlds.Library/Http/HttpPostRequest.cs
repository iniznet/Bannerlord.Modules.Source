using System;
using System.Net.Http;
using System.Text;

namespace TaleWorlds.Library.Http
{
	public class HttpPostRequest : IHttpRequestTask
	{
		public HttpRequestTaskState State { get; private set; }

		public bool Successful { get; private set; }

		public string ResponseData { get; private set; }

		public Exception Exception { get; private set; }

		public HttpPostRequest(HttpClient httpClient, string address, string postData)
			: this(httpClient, address, postData, new Version("1.1"))
		{
		}

		public HttpPostRequest(HttpClient httpClient, string address, string postData, Version version)
		{
			this._httpClient = httpClient;
			this._postData = postData;
			this._address = address;
			this.State = HttpRequestTaskState.NotStarted;
			this.ResponseData = "";
			this._versionToUse = version;
		}

		private void SetFinishedAsSuccessful(string responseData)
		{
			this.Successful = true;
			this.ResponseData = responseData;
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
				Debug.Print("Http Post Request to " + this._address, 0, Debug.DebugColor.White, 17592186044416UL);
				using (HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, this._address))
				{
					requestMessage.Version = this._versionToUse;
					requestMessage.Headers.Add("Accept", "application/json");
					requestMessage.Headers.Add("UserAgent", "TaleWorlds Client");
					requestMessage.Content = new StringContent(this._postData, Encoding.Unicode, "application/json");
					HttpResponseMessage httpResponseMessage = await this._httpClient.SendAsync(requestMessage);
					using (HttpResponseMessage response = httpResponseMessage)
					{
						Debug.Print(string.Concat(new object[] { "Protocol version used for post request to ", this._address, " is: ", response.Version }), 0, Debug.DebugColor.White, 17592186044416UL);
						using (HttpContent content = response.Content)
						{
							this.SetFinishedAsSuccessful(await content.ReadAsStringAsync());
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

		private HttpClient _httpClient;

		private readonly string _address;

		private string _postData;

		private Version _versionToUse;
	}
}
