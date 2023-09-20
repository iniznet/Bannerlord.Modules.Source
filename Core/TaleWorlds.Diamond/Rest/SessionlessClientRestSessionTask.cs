using System;
using System.Collections.Specialized;
using System.Net;
using TaleWorlds.Library;
using TaleWorlds.Library.Http;

namespace TaleWorlds.Diamond.Rest
{
	internal class SessionlessClientRestSessionTask
	{
		public SessionlessRestRequestMessage RestRequestMessage { get; private set; }

		public bool Finished { get; private set; }

		public bool Successful { get; private set; }

		public IHttpRequestTask Request { get; private set; }

		public SessionlessRestResponse RestResponse { get; private set; }

		public SessionlessClientRestSessionTask(SessionlessRestRequestMessage restRequestMessage)
		{
			this.RestRequestMessage = restRequestMessage;
		}

		public void SetRequestData(string address, ushort port, bool isSecure, IHttpDriver networkClient)
		{
			this._requestAddress = address;
			this._requestPort = port;
			this._isSecure = isSecure;
			this._postData = this.RestRequestMessage.SerializeAsJson();
			this._networkClient = networkClient;
			this.CreateAndSetRequest();
		}

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

		public void SetFinishedAsSuccessful(SessionlessRestResponse restResponse)
		{
			Debug.Print("SessionlessClientRestSessionTask::SetFinishedAsSuccessful", 0, Debug.DebugColor.White, 17592186044416UL);
			this.RestResponse = restResponse;
			this.Successful = true;
			this.Finished = true;
			Debug.Print("SessionlessClientRestSessionTask::SetFinishedAsSuccessful done", 0, Debug.DebugColor.White, 17592186044416UL);
		}

		public void SetFinishedAsFailed()
		{
			this.SetFinishedAsFailed(null);
		}

		public void SetFinishedAsFailed(SessionlessRestResponse restResponse)
		{
			Debug.Print("SessionlessClientRestSessionTask::SetFinishedAsFailed", 0, Debug.DebugColor.White, 17592186044416UL);
			this.RestResponse = restResponse;
			this.Successful = false;
			this.Finished = true;
			Debug.Print("SessionlessClientRestSessionTask::SetFinishedAsFailed done", 0, Debug.DebugColor.White, 17592186044416UL);
		}

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

		private string _requestAddress;

		private ushort _requestPort;

		private string _postData;

		private bool _isSecure;

		private int _maxIterationCount = 5;

		private int _currentIterationCount;

		private IHttpDriver _networkClient;
	}
}
