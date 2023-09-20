using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using TaleWorlds.Library;
using TaleWorlds.Library.Http;

namespace TaleWorlds.Diamond.Rest
{
	// Token: 0x0200003F RID: 63
	internal class ClientRestSessionTask
	{
		// Token: 0x1700003F RID: 63
		// (get) Token: 0x06000152 RID: 338 RVA: 0x000045FA File Offset: 0x000027FA
		// (set) Token: 0x06000153 RID: 339 RVA: 0x00004602 File Offset: 0x00002802
		public RestRequestMessage RestRequestMessage { get; private set; }

		// Token: 0x17000040 RID: 64
		// (get) Token: 0x06000154 RID: 340 RVA: 0x0000460B File Offset: 0x0000280B
		// (set) Token: 0x06000155 RID: 341 RVA: 0x00004613 File Offset: 0x00002813
		public bool Finished { get; private set; }

		// Token: 0x17000041 RID: 65
		// (get) Token: 0x06000156 RID: 342 RVA: 0x0000461C File Offset: 0x0000281C
		// (set) Token: 0x06000157 RID: 343 RVA: 0x00004624 File Offset: 0x00002824
		public bool Successful { get; private set; }

		// Token: 0x17000042 RID: 66
		// (get) Token: 0x06000158 RID: 344 RVA: 0x0000462D File Offset: 0x0000282D
		// (set) Token: 0x06000159 RID: 345 RVA: 0x00004635 File Offset: 0x00002835
		public IHttpRequestTask Request { get; private set; }

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x0600015A RID: 346 RVA: 0x0000463E File Offset: 0x0000283E
		// (set) Token: 0x0600015B RID: 347 RVA: 0x00004646 File Offset: 0x00002846
		public RestResponse RestResponse { get; private set; }

		// Token: 0x17000044 RID: 68
		// (get) Token: 0x0600015C RID: 348 RVA: 0x0000464F File Offset: 0x0000284F
		public bool IsCompletelyFinished
		{
			get
			{
				return !this._willTryAgain && this._resultExamined && this.Request.State == HttpRequestTaskState.Finished;
			}
		}

		// Token: 0x0600015D RID: 349 RVA: 0x00004674 File Offset: 0x00002874
		public ClientRestSessionTask(RestRequestMessage restRequestMessage)
		{
			this.RestRequestMessage = restRequestMessage;
			this._taskCompletionSource = new TaskCompletionSource<bool>();
			this._sw = new Stopwatch();
			if (this.RestRequestMessage is RestDataRequestMessage)
			{
				RestDataRequestMessage restDataRequestMessage = (RestDataRequestMessage)this.RestRequestMessage;
				this._messageName = restDataRequestMessage.MessageName;
				return;
			}
			this._messageName = this.RestRequestMessage.TypeName;
		}

		// Token: 0x0600015E RID: 350 RVA: 0x000046E4 File Offset: 0x000028E4
		public void SetRequestData(byte[] userCertificate, string address, ushort port, bool isSecure, IHttpDriver networkClient)
		{
			this.RestRequestMessage.UserCertificate = userCertificate;
			this._requestAddress = address;
			this._requestPort = port;
			this._isSecure = isSecure;
			this._postData = this.RestRequestMessage.SerializeAsJson();
			this._networkClient = networkClient;
			this.CreateAndSetRequest();
		}

		// Token: 0x0600015F RID: 351 RVA: 0x00004734 File Offset: 0x00002934
		private void DetermineNextTry()
		{
			if (this._sw.ElapsedMilliseconds >= (long)ClientRestSessionTask.RequestRetryTimeout)
			{
				this._willTryAgain = false;
				Debug.Print("Retrying http post request, iteration count: " + this._currentIterationCount, 0, Debug.DebugColor.White, 17592186044416UL);
				this.CreateAndSetRequest();
			}
		}

		// Token: 0x06000160 RID: 352 RVA: 0x00004788 File Offset: 0x00002988
		private static string GetCode(WebException webException)
		{
			if (webException.Response != null && webException.Response is HttpWebResponse)
			{
				return ((HttpWebResponse)webException.Response).StatusCode.ToString();
			}
			return "NoCode";
		}

		// Token: 0x06000161 RID: 353 RVA: 0x000047D0 File Offset: 0x000029D0
		private void ExamineResult()
		{
			if (!this.Request.Successful)
			{
				bool flag = false;
				if (this.Request.Exception != null && this.Request.Exception is WebException)
				{
					WebException ex = (WebException)this.Request.Exception;
					string code = ClientRestSessionTask.GetCode(ex);
					if (ex.Status == WebExceptionStatus.ConnectionClosed || ex.Status == WebExceptionStatus.ConnectFailure || ex.Status == WebExceptionStatus.KeepAliveFailure || ex.Status == WebExceptionStatus.ReceiveFailure || ex.Status == WebExceptionStatus.RequestCanceled || ex.Status == WebExceptionStatus.Timeout || ex.Status == WebExceptionStatus.ProtocolError || ex.Status == WebExceptionStatus.UnknownError)
					{
						Debug.Print(string.Concat(new object[] { "Http Post Request(code:", code, ") with message(", this.RestRequestMessage, ") failed. Retrying. (", ex.Status, ")" }), 0, Debug.DebugColor.White, 17592186044416UL);
						flag = true;
					}
					else
					{
						Debug.Print(string.Concat(new object[] { "Http Post Request(code:", code, ") with message(", this.RestRequestMessage, ") failed. Exception status: ", ex.Status }), 0, Debug.DebugColor.White, 17592186044416UL);
					}
				}
				else if (this.Request.Exception != null && this.Request.Exception is HttpPostRequestTimeoutException)
				{
					object[] array = new object[6];
					array[0] = "Http Post Request with message(";
					array[1] = this.RestRequestMessage;
					array[2] = ")  failed. Retrying: (";
					int num = 3;
					Exception exception = this.Request.Exception;
					array[num] = ((exception != null) ? exception.GetType() : null);
					array[4] = ") ";
					array[5] = this.Request.Exception;
					Debug.Print(string.Concat(array), 0, Debug.DebugColor.White, 17592186044416UL);
					flag = true;
				}
				else if (this.Request.Exception != null && this.Request.Exception is InvalidOperationException)
				{
					object[] array2 = new object[6];
					array2[0] = "Http Post Request with message(";
					array2[1] = this.RestRequestMessage;
					array2[2] = ")  failed. Retrying: (";
					int num2 = 3;
					Exception exception2 = this.Request.Exception;
					array2[num2] = ((exception2 != null) ? exception2.GetType() : null);
					array2[4] = ") ";
					array2[5] = this.Request.Exception;
					Debug.Print(string.Concat(array2), 0, Debug.DebugColor.White, 17592186044416UL);
					flag = true;
				}
				else
				{
					object[] array3 = new object[6];
					array3[0] = "Http Post Request with message(";
					array3[1] = this.RestRequestMessage;
					array3[2] = ")  failed. Exception: (";
					int num3 = 3;
					Exception exception3 = this.Request.Exception;
					array3[num3] = ((exception3 != null) ? exception3.GetType() : null);
					array3[4] = ") ";
					array3[5] = this.Request.Exception;
					Debug.Print(string.Concat(array3), 0, Debug.DebugColor.White, 17592186044416UL);
				}
				if (this.Request != null && this.Request.Exception != null)
				{
					this.PrintExceptions(this.Request.Exception);
				}
				if (flag)
				{
					if (this._currentIterationCount < this._maxIterationCount)
					{
						this._sw.Restart();
						this._willTryAgain = true;
						this._currentIterationCount++;
						Debug.Print(string.Concat(new object[] { "Http post request(", this.RestRequestMessage, ")  will try again, iteration count: ", this._currentIterationCount }), 0, Debug.DebugColor.White, 17592186044416UL);
					}
					else
					{
						this._willTryAgain = false;
						Debug.Print("Passed max retry count for http post request(" + this.RestRequestMessage + ") ", 0, Debug.DebugColor.White, 17592186044416UL);
					}
				}
				else
				{
					Debug.Print("Http post request(" + this.RestRequestMessage + ")  will not try again due to exception type!", 0, Debug.DebugColor.White, 17592186044416UL);
					this._willTryAgain = false;
				}
			}
			else if (this._currentIterationCount > 0)
			{
				Debug.Print(string.Concat(new object[] { "Http post request(", this.RestRequestMessage, ") is successful with iteration count: ", this._currentIterationCount }), 0, Debug.DebugColor.White, 17592186044416UL);
			}
			this._resultExamined = true;
		}

		// Token: 0x06000162 RID: 354 RVA: 0x00004BF4 File Offset: 0x00002DF4
		public void Tick()
		{
			switch (this.Request.State)
			{
			case HttpRequestTaskState.NotStarted:
				this.Request.Start();
				return;
			case HttpRequestTaskState.Working:
				break;
			case HttpRequestTaskState.Finished:
				if (!this._resultExamined)
				{
					this.ExamineResult();
					return;
				}
				this.DetermineNextTry();
				break;
			default:
				return;
			}
		}

		// Token: 0x06000163 RID: 355 RVA: 0x00004C44 File Offset: 0x00002E44
		public async Task WaitUntilFinished()
		{
			Debug.Print("ClientRestSessionTask::WaitUntilFinished::" + this._messageName, 0, Debug.DebugColor.White, 17592186044416UL);
			await this._taskCompletionSource.Task;
			Debug.Print("ClientRestSessionTask::WaitUntilFinished::" + this._messageName + " done", 0, Debug.DebugColor.White, 17592186044416UL);
		}

		// Token: 0x06000164 RID: 356 RVA: 0x00004C8C File Offset: 0x00002E8C
		public void SetFinishedAsSuccessful(RestResponse restResponse)
		{
			Debug.Print("ClientRestSessionTask::SetFinishedAsSuccessful::" + this._messageName, 0, Debug.DebugColor.White, 17592186044416UL);
			this.RestResponse = restResponse;
			this.Successful = true;
			this.Finished = true;
			this._taskCompletionSource.SetResult(true);
			Debug.Print("ClientRestSessionTask::SetFinishedAsSuccessful::" + this._messageName + " done", 0, Debug.DebugColor.White, 17592186044416UL);
		}

		// Token: 0x06000165 RID: 357 RVA: 0x00004D01 File Offset: 0x00002F01
		public void SetFinishedAsFailed()
		{
			this.SetFinishedAsFailed(null);
		}

		// Token: 0x06000166 RID: 358 RVA: 0x00004D0C File Offset: 0x00002F0C
		public void SetFinishedAsFailed(RestResponse restResponse)
		{
			Debug.Print("ClientRestSessionTask::SetFinishedAsFailed::" + this._messageName, 0, Debug.DebugColor.White, 17592186044416UL);
			this.RestResponse = restResponse;
			this.Successful = false;
			this.Finished = true;
			this._taskCompletionSource.SetResult(true);
			Debug.Print("ClientRestSessionTask::SetFinishedAsFailed:: " + this._messageName + " done", 0, Debug.DebugColor.White, 17592186044416UL);
		}

		// Token: 0x06000167 RID: 359 RVA: 0x00004D84 File Offset: 0x00002F84
		private void CreateAndSetRequest()
		{
			string text = "http://";
			if (this._isSecure)
			{
				text = "https://";
			}
			string text2 = string.Concat(new object[] { text, this._requestAddress, ":", this._requestPort, "/Data/ProcessMessage" });
			NameValueCollection nameValueCollection = new NameValueCollection();
			nameValueCollection.Add("url", text2);
			nameValueCollection.Add("body", this._postData);
			nameValueCollection.Add("verb", "POST");
			this.Request = this._networkClient.CreateHttpPostRequestTask(text2, this._postData, true);
			this._resultExamined = false;
		}

		// Token: 0x06000168 RID: 360 RVA: 0x00004E30 File Offset: 0x00003030
		private void PrintExceptions(Exception e)
		{
			if (e != null)
			{
				Exception ex = e;
				int num = 0;
				while (ex != null)
				{
					Debug.Print(string.Concat(new object[] { "Exception #", num, ": ", ex.Message, " ||| StackTrace: ", ex.InnerException }), 0, Debug.DebugColor.White, 17592186044416UL);
					ex = ex.InnerException;
					num++;
				}
			}
		}

		// Token: 0x0400006F RID: 111
		private static readonly int RequestRetryTimeout = 1000;

		// Token: 0x04000074 RID: 116
		public bool _willTryAgain;

		// Token: 0x04000076 RID: 118
		private string _requestAddress;

		// Token: 0x04000077 RID: 119
		private ushort _requestPort;

		// Token: 0x04000078 RID: 120
		private string _postData;

		// Token: 0x04000079 RID: 121
		private bool _isSecure;

		// Token: 0x0400007A RID: 122
		private string _messageName;

		// Token: 0x0400007B RID: 123
		private int _maxIterationCount = 5;

		// Token: 0x0400007C RID: 124
		private int _currentIterationCount;

		// Token: 0x0400007D RID: 125
		private Stopwatch _sw;

		// Token: 0x0400007E RID: 126
		private TaskCompletionSource<bool> _taskCompletionSource;

		// Token: 0x0400007F RID: 127
		private IHttpDriver _networkClient;

		// Token: 0x04000080 RID: 128
		private bool _resultExamined;
	}
}
