using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TaleWorlds.Library.Http
{
	public class HttpGetRequest : IHttpRequestTask
	{
		public HttpRequestTaskState State { get; private set; }

		public bool Successful { get; private set; }

		public string ResponseData { get; private set; }

		public HttpStatusCode ResponseStatusCode { get; private set; }

		public Exception Exception { get; private set; }

		static HttpGetRequest()
		{
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
		}

		public HttpGetRequest(string address)
		{
			this._address = address;
			this.State = HttpRequestTaskState.NotStarted;
			this.ResponseData = "";
			this.ResponseStatusCode = HttpStatusCode.OK;
		}

		private void SetFinishedAsSuccessful(string responseData, HttpStatusCode statusCode)
		{
			this.Successful = true;
			this.ResponseData = responseData;
			this.ResponseStatusCode = statusCode;
			if (this._httpWebResponse != null)
			{
				this._httpWebResponse.Close();
			}
			this.State = HttpRequestTaskState.Finished;
		}

		private void SetFinishedAsUnsuccessful(Exception e)
		{
			this.Successful = false;
			this.Exception = e;
			if (this._httpWebResponse != null)
			{
				this._httpWebResponse.Close();
			}
			this.State = HttpRequestTaskState.Finished;
		}

		public void Start()
		{
			this.DoTask();
		}

		public async Task DoTask()
		{
			this.State = HttpRequestTaskState.Working;
			try
			{
				HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(this._address);
				httpWebRequest.Accept = "application/json";
				httpWebRequest.ContentType = "application/json";
				httpWebRequest.Method = "GET";
				httpWebRequest.UserAgent = "WarRide Server";
				WebResponse webResponse = await httpWebRequest.GetResponseAsync();
				this._httpWebResponse = (HttpWebResponse)webResponse;
				Stream responseStream = this._httpWebResponse.GetResponseStream();
				byte[] readBuffer = new byte[1024];
				StringBuilder requestData = new StringBuilder("");
				int num;
				do
				{
					num = await responseStream.ReadAsync(readBuffer, 0, 1024);
					if (num > 0)
					{
						requestData.Append(Encoding.ASCII.GetString(readBuffer, 0, num));
					}
				}
				while (num > 0);
				string text = "";
				if (requestData.Length > 1)
				{
					text = requestData.ToString();
				}
				responseStream.Close();
				this.SetFinishedAsSuccessful(text, this._httpWebResponse.StatusCode);
				responseStream = null;
				readBuffer = null;
				requestData = null;
			}
			catch (Exception ex)
			{
				this.SetFinishedAsUnsuccessful(ex);
			}
		}

		private const int BufferSize = 1024;

		private HttpWebResponse _httpWebResponse;

		private readonly string _address;
	}
}
