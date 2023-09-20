using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using TaleWorlds.Library;
using TaleWorlds.Library.Http;

namespace TaleWorlds.Diamond.Rest
{
	internal class ClientRestSessionTask
	{
		public RestRequestMessage RestRequestMessage { get; private set; }

		public bool Finished { get; private set; }

		public bool Successful { get; private set; }

		public IHttpRequestTask Request { get; private set; }

		public RestResponse RestResponse { get; private set; }

		public bool IsCompletelyFinished
		{
			get
			{
				return !this._willTryAgain && this._resultExamined && this.Request.State == HttpRequestTaskState.Finished;
			}
		}

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

		private void DetermineNextTry()
		{
			if (this._sw.ElapsedMilliseconds >= (long)ClientRestSessionTask.RequestRetryTimeout)
			{
				this._willTryAgain = false;
				Debug.Print("Retrying http post request, iteration count: " + this._currentIterationCount, 0, Debug.DebugColor.White, 17592186044416UL);
				this.CreateAndSetRequest();
			}
		}

		private static string GetCode(WebException webException)
		{
			if (webException.Response != null && webException.Response is HttpWebResponse)
			{
				return ((HttpWebResponse)webException.Response).StatusCode.ToString();
			}
			return "NoCode";
		}

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

		public async Task WaitUntilFinished()
		{
			Debug.Print("ClientRestSessionTask::WaitUntilFinished::" + this._messageName, 0, Debug.DebugColor.White, 17592186044416UL);
			await this._taskCompletionSource.Task;
			Debug.Print("ClientRestSessionTask::WaitUntilFinished::" + this._messageName + " done", 0, Debug.DebugColor.White, 17592186044416UL);
		}

		public void SetFinishedAsSuccessful(RestResponse restResponse)
		{
			Debug.Print("ClientRestSessionTask::SetFinishedAsSuccessful::" + this._messageName, 0, Debug.DebugColor.White, 17592186044416UL);
			this.RestResponse = restResponse;
			this.Successful = true;
			this.Finished = true;
			this._taskCompletionSource.SetResult(true);
			Debug.Print("ClientRestSessionTask::SetFinishedAsSuccessful::" + this._messageName + " done", 0, Debug.DebugColor.White, 17592186044416UL);
		}

		public void SetFinishedAsFailed()
		{
			this.SetFinishedAsFailed(null);
		}

		public void SetFinishedAsFailed(RestResponse restResponse)
		{
			Debug.Print("ClientRestSessionTask::SetFinishedAsFailed::" + this._messageName, 0, Debug.DebugColor.White, 17592186044416UL);
			this.RestResponse = restResponse;
			this.Successful = false;
			this.Finished = true;
			this._taskCompletionSource.SetResult(true);
			Debug.Print("ClientRestSessionTask::SetFinishedAsFailed:: " + this._messageName + " done", 0, Debug.DebugColor.White, 17592186044416UL);
		}

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

		private static readonly int RequestRetryTimeout = 1000;

		public bool _willTryAgain;

		private string _requestAddress;

		private ushort _requestPort;

		private string _postData;

		private bool _isSecure;

		private string _messageName;

		private int _maxIterationCount = 5;

		private int _currentIterationCount;

		private Stopwatch _sw;

		private TaskCompletionSource<bool> _taskCompletionSource;

		private IHttpDriver _networkClient;

		private bool _resultExamined;
	}
}
