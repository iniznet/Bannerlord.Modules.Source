using System;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TaleWorlds.Library.Http
{
	public class HttpPostRequest : IHttpRequestTask
	{
		public HttpRequestTaskState State { get; private set; }

		public bool Successful { get; private set; }

		public string ResponseData { get; private set; }

		public Exception Exception { get; private set; }

		static HttpPostRequest()
		{
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
		}

		public HttpPostRequest(string address, string postData)
		{
			this._postData = postData;
			this._address = address;
			this.State = HttpRequestTaskState.NotStarted;
			this._readBuffer = new byte[1024];
			this._requestData = new StringBuilder("");
			this.ResponseData = "";
		}

		private void SetFinishedAsSuccessful(string responseData)
		{
			this.Successful = true;
			this.ResponseData = responseData;
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

		private void ClearRequestStream(Task<Stream> streamTask)
		{
			HttpPostRequest.<>c__DisplayClass29_0 CS$<>8__locals1 = new HttpPostRequest.<>c__DisplayClass29_0();
			CS$<>8__locals1.streamTask = streamTask;
			CS$<>8__locals1.<>4__this = this;
			Task.Run(delegate
			{
				HttpPostRequest.<>c__DisplayClass29_0.<<ClearRequestStream>b__0>d <<ClearRequestStream>b__0>d;
				<<ClearRequestStream>b__0>d.<>4__this = CS$<>8__locals1;
				<<ClearRequestStream>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
				<<ClearRequestStream>b__0>d.<>1__state = -1;
				AsyncTaskMethodBuilder <>t__builder = <<ClearRequestStream>b__0>d.<>t__builder;
				<>t__builder.Start<HttpPostRequest.<>c__DisplayClass29_0.<<ClearRequestStream>b__0>d>(ref <<ClearRequestStream>b__0>d);
				return <<ClearRequestStream>b__0>d.<>t__builder.Task;
			});
		}

		private async Task DoTask()
		{
			this.State = HttpRequestTaskState.Working;
			try
			{
				Debug.Print("Http Post Request to " + this._address, 0, Debug.DebugColor.White, 17592186044416UL);
				byte[] postData = Encoding.Unicode.GetBytes(this._postData);
				this._httpWebRequest = (HttpWebRequest)WebRequest.Create(this._address);
				this._httpWebRequest.Accept = "application/json";
				this._httpWebRequest.ContentType = "application/json; charset=utf-16";
				this._httpWebRequest.Method = "POST";
				this._httpWebRequest.ContentLength = (long)postData.Length;
				this._httpWebRequest.UserAgent = "TaleWorlds Client";
				this._httpWebRequest.TransferEncoding = "";
				Stream stream = await this._httpWebRequest.GetRequestStreamAsync();
				Stream requestStream = stream;
				await requestStream.WriteAsync(postData, 0, postData.Length);
				requestStream.Close();
				this._httpWebResponse = (HttpWebResponse)(await this._httpWebRequest.GetResponseAsync());
				this._responseStream = this._httpWebResponse.GetResponseStream();
				int num;
				do
				{
					num = await this._responseStream.ReadAsync(this._readBuffer, 0, 1024);
					if (num > 0)
					{
						string @string = Encoding.ASCII.GetString(this._readBuffer, 0, num);
						this._requestData.Append(@string);
					}
				}
				while (num > 0);
				string text = "";
				if (this._requestData.Length > 1)
				{
					text = this._requestData.ToString();
				}
				this._responseStream.Close();
				this.SetFinishedAsSuccessful(text);
				postData = null;
				requestStream = null;
			}
			catch (Exception ex)
			{
				this.SetFinishedAsUnsuccessful(ex);
			}
		}

		private const int BufferSize = 1024;

		private HttpWebRequest _httpWebRequest;

		private HttpWebResponse _httpWebResponse;

		private Stream _responseStream;

		private readonly byte[] _readBuffer;

		private readonly StringBuilder _requestData;

		private readonly string _address;

		private string _postData;
	}
}
