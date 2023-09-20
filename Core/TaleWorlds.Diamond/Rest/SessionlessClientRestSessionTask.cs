using System;
using System.Collections.Specialized;
using System.Net;
using TaleWorlds.Library;
using TaleWorlds.Library.Http;

namespace TaleWorlds.Diamond.Rest
{
	// Token: 0x02000038 RID: 56
	internal class SessionlessClientRestSessionTask
	{
		// Token: 0x17000030 RID: 48
		// (get) Token: 0x06000110 RID: 272 RVA: 0x00003AD5 File Offset: 0x00001CD5
		// (set) Token: 0x06000111 RID: 273 RVA: 0x00003ADD File Offset: 0x00001CDD
		public SessionlessRestRequestMessage RestRequestMessage { get; private set; }

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x06000112 RID: 274 RVA: 0x00003AE6 File Offset: 0x00001CE6
		// (set) Token: 0x06000113 RID: 275 RVA: 0x00003AEE File Offset: 0x00001CEE
		public bool Finished { get; private set; }

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x06000114 RID: 276 RVA: 0x00003AF7 File Offset: 0x00001CF7
		// (set) Token: 0x06000115 RID: 277 RVA: 0x00003AFF File Offset: 0x00001CFF
		public bool Successful { get; private set; }

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x06000116 RID: 278 RVA: 0x00003B08 File Offset: 0x00001D08
		// (set) Token: 0x06000117 RID: 279 RVA: 0x00003B10 File Offset: 0x00001D10
		public IHttpRequestTask Request { get; private set; }

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x06000118 RID: 280 RVA: 0x00003B19 File Offset: 0x00001D19
		// (set) Token: 0x06000119 RID: 281 RVA: 0x00003B21 File Offset: 0x00001D21
		public SessionlessRestResponse RestResponse { get; private set; }

		// Token: 0x0600011A RID: 282 RVA: 0x00003B2A File Offset: 0x00001D2A
		public SessionlessClientRestSessionTask(SessionlessRestRequestMessage restRequestMessage)
		{
			this.RestRequestMessage = restRequestMessage;
		}

		// Token: 0x0600011B RID: 283 RVA: 0x00003B40 File Offset: 0x00001D40
		public void SetRequestData(string address, ushort port, bool isSecure, IHttpDriver networkClient)
		{
			this._requestAddress = address;
			this._requestPort = port;
			this._isSecure = isSecure;
			this._postData = this.RestRequestMessage.SerializeAsJson();
			this._networkClient = networkClient;
			this.CreateAndSetRequest();
		}

		// Token: 0x0600011C RID: 284 RVA: 0x00003B78 File Offset: 0x00001D78
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
				if (!this.Request.Successful && this._currentIterationCount < this._maxIterationCount)
				{
					if (this.Request.Exception != null && this.Request.Exception is WebException)
					{
						WebException ex = (WebException)this.Request.Exception;
						if (ex.Status == WebExceptionStatus.ConnectionClosed || ex.Status == WebExceptionStatus.ConnectFailure || ex.Status == WebExceptionStatus.KeepAliveFailure || ex.Status == WebExceptionStatus.ReceiveFailure || ex.Status == WebExceptionStatus.RequestCanceled || ex.Status == WebExceptionStatus.Timeout || ex.Status == WebExceptionStatus.ProtocolError || ex.Status == WebExceptionStatus.UnknownError)
						{
							Debug.Print("Http Post Request with message failed. Retrying. (" + ex.Status + ")", 0, Debug.DebugColor.White, 17592186044416UL);
							this.CreateAndSetRequest();
						}
						else
						{
							Debug.Print("Http Post Request with message failed. Exception status: " + ex.Status, 0, Debug.DebugColor.White, 17592186044416UL);
						}
					}
					else if (this.Request.Exception != null && this.Request.Exception is InvalidOperationException)
					{
						object[] array = new object[4];
						array[0] = "Http Post Request with message failed. Retrying: (";
						int num = 1;
						Exception exception = this.Request.Exception;
						array[num] = ((exception != null) ? exception.GetType() : null);
						array[2] = ") ";
						array[3] = this.Request.Exception;
						Debug.Print(string.Concat(array), 0, Debug.DebugColor.White, 17592186044416UL);
						this.CreateAndSetRequest();
					}
					else
					{
						object[] array2 = new object[4];
						array2[0] = "Http Post Request with message failed. Exception: (";
						int num2 = 1;
						Exception exception2 = this.Request.Exception;
						array2[num2] = ((exception2 != null) ? exception2.GetType() : null);
						array2[2] = ") ";
						array2[3] = this.Request.Exception;
						Debug.Print(string.Concat(array2), 0, Debug.DebugColor.White, 17592186044416UL);
					}
					this._currentIterationCount++;
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x0600011D RID: 285 RVA: 0x00003D90 File Offset: 0x00001F90
		public void SetFinishedAsSuccessful(SessionlessRestResponse restResponse)
		{
			Debug.Print("SessionlessClientRestSessionTask::SetFinishedAsSuccessful", 0, Debug.DebugColor.White, 17592186044416UL);
			this.RestResponse = restResponse;
			this.Successful = true;
			this.Finished = true;
			Debug.Print("SessionlessClientRestSessionTask::SetFinishedAsSuccessful done", 0, Debug.DebugColor.White, 17592186044416UL);
		}

		// Token: 0x0600011E RID: 286 RVA: 0x00003DDE File Offset: 0x00001FDE
		public void SetFinishedAsFailed()
		{
			this.SetFinishedAsFailed(null);
		}

		// Token: 0x0600011F RID: 287 RVA: 0x00003DE8 File Offset: 0x00001FE8
		public void SetFinishedAsFailed(SessionlessRestResponse restResponse)
		{
			Debug.Print("SessionlessClientRestSessionTask::SetFinishedAsFailed", 0, Debug.DebugColor.White, 17592186044416UL);
			this.RestResponse = restResponse;
			this.Successful = false;
			this.Finished = true;
			Debug.Print("SessionlessClientRestSessionTask::SetFinishedAsFailed done", 0, Debug.DebugColor.White, 17592186044416UL);
		}

		// Token: 0x06000120 RID: 288 RVA: 0x00003E38 File Offset: 0x00002038
		private void CreateAndSetRequest()
		{
			string text = "http://";
			if (this._isSecure)
			{
				text = "https://";
			}
			string text2 = string.Concat(new object[] { text, this._requestAddress, ":", this._requestPort, "/SessionlessData/ProcessMessage" });
			NameValueCollection nameValueCollection = new NameValueCollection();
			nameValueCollection.Add("url", text2);
			nameValueCollection.Add("body", this._postData);
			nameValueCollection.Add("verb", "POST");
			this.Request = this._networkClient.CreateHttpPostRequestTask(text2, this._postData, true);
		}

		// Token: 0x04000050 RID: 80
		private string _requestAddress;

		// Token: 0x04000051 RID: 81
		private ushort _requestPort;

		// Token: 0x04000052 RID: 82
		private string _postData;

		// Token: 0x04000053 RID: 83
		private bool _isSecure;

		// Token: 0x04000054 RID: 84
		private int _maxIterationCount = 5;

		// Token: 0x04000055 RID: 85
		private int _currentIterationCount;

		// Token: 0x04000056 RID: 86
		private IHttpDriver _networkClient;
	}
}
