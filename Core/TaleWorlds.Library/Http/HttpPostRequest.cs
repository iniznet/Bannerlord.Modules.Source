using System;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TaleWorlds.Library.Http
{
	// Token: 0x020000A6 RID: 166
	public class HttpPostRequest : IHttpRequestTask
	{
		// Token: 0x170000B8 RID: 184
		// (get) Token: 0x0600060F RID: 1551 RVA: 0x000131CD File Offset: 0x000113CD
		// (set) Token: 0x06000610 RID: 1552 RVA: 0x000131D5 File Offset: 0x000113D5
		public HttpRequestTaskState State { get; private set; }

		// Token: 0x170000B9 RID: 185
		// (get) Token: 0x06000611 RID: 1553 RVA: 0x000131DE File Offset: 0x000113DE
		// (set) Token: 0x06000612 RID: 1554 RVA: 0x000131E6 File Offset: 0x000113E6
		public bool Successful { get; private set; }

		// Token: 0x170000BA RID: 186
		// (get) Token: 0x06000613 RID: 1555 RVA: 0x000131EF File Offset: 0x000113EF
		// (set) Token: 0x06000614 RID: 1556 RVA: 0x000131F7 File Offset: 0x000113F7
		public string ResponseData { get; private set; }

		// Token: 0x170000BB RID: 187
		// (get) Token: 0x06000615 RID: 1557 RVA: 0x00013200 File Offset: 0x00011400
		// (set) Token: 0x06000616 RID: 1558 RVA: 0x00013208 File Offset: 0x00011408
		public Exception Exception { get; private set; }

		// Token: 0x06000617 RID: 1559 RVA: 0x00013211 File Offset: 0x00011411
		static HttpPostRequest()
		{
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
		}

		// Token: 0x06000618 RID: 1560 RVA: 0x00013220 File Offset: 0x00011420
		public HttpPostRequest(string address, string postData)
		{
			this._postData = postData;
			this._address = address;
			this.State = HttpRequestTaskState.NotStarted;
			this._readBuffer = new byte[1024];
			this._requestData = new StringBuilder("");
			this.ResponseData = "";
		}

		// Token: 0x06000619 RID: 1561 RVA: 0x00013273 File Offset: 0x00011473
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

		// Token: 0x0600061A RID: 1562 RVA: 0x0001329D File Offset: 0x0001149D
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

		// Token: 0x0600061B RID: 1563 RVA: 0x000132C7 File Offset: 0x000114C7
		public void Start()
		{
			this.DoTask();
		}

		// Token: 0x0600061C RID: 1564 RVA: 0x000132D0 File Offset: 0x000114D0
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

		// Token: 0x0600061D RID: 1565 RVA: 0x000132F8 File Offset: 0x000114F8
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

		// Token: 0x040001C9 RID: 457
		private const int BufferSize = 1024;

		// Token: 0x040001CA RID: 458
		private HttpWebRequest _httpWebRequest;

		// Token: 0x040001CB RID: 459
		private HttpWebResponse _httpWebResponse;

		// Token: 0x040001CC RID: 460
		private Stream _responseStream;

		// Token: 0x040001CD RID: 461
		private readonly byte[] _readBuffer;

		// Token: 0x040001CE RID: 462
		private readonly StringBuilder _requestData;

		// Token: 0x040001CF RID: 463
		private readonly string _address;

		// Token: 0x040001D0 RID: 464
		private string _postData;
	}
}
