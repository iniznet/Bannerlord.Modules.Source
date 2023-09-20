using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TaleWorlds.Library;
using TaleWorlds.Library.Http;
using TaleWorlds.Localization;

namespace TaleWorlds.Diamond.Rest
{
	// Token: 0x0200003E RID: 62
	public class ClientRestSession : IClientSession
	{
		// Token: 0x1700003D RID: 61
		// (get) Token: 0x0600013B RID: 315 RVA: 0x00003FEC File Offset: 0x000021EC
		// (set) Token: 0x0600013C RID: 316 RVA: 0x00003FF4 File Offset: 0x000021F4
		public bool IsConnected { get; private set; }

		// Token: 0x1700003E RID: 62
		// (get) Token: 0x0600013D RID: 317 RVA: 0x00003FFD File Offset: 0x000021FD
		// (set) Token: 0x0600013E RID: 318 RVA: 0x00004005 File Offset: 0x00002205
		public IClient Client { get; private set; }

		// Token: 0x0600013F RID: 319 RVA: 0x00004010 File Offset: 0x00002210
		public ClientRestSession(IClient client, string address, ushort port, bool isSecure, IHttpDriver platformNetworkClient)
		{
			this.Client = client;
			this._sessionInitialized = false;
			this._isSecure = isSecure;
			this._platformNetworkClient = platformNetworkClient;
			this.ResetTimer();
			this._address = address;
			this._port = port;
			this._messageTaskQueue = new Queue<ClientRestSessionTask>();
			this._currentConnectionResultType = ClientRestSession.ConnectionResultType.None;
			this._restDataJsonConverter = new RestDataJsonConverter();
		}

		// Token: 0x06000140 RID: 320 RVA: 0x00004072 File Offset: 0x00002272
		private void ResetTimer()
		{
			this._timer = new Stopwatch();
			this._timer.Start();
		}

		// Token: 0x06000141 RID: 321 RVA: 0x0000408C File Offset: 0x0000228C
		private void AssignRequestJob(ClientRestSessionTask requestMessageTask)
		{
			RestRequestMessage restRequestMessage = requestMessageTask.RestRequestMessage;
			bool flag = false;
			if (restRequestMessage is ConnectMessage)
			{
				if (!this.IsConnected)
				{
					flag = true;
				}
			}
			else if (restRequestMessage is DisconnectMessage)
			{
				if (this.IsConnected)
				{
					flag = true;
				}
			}
			else if (this.IsConnected)
			{
				flag = true;
			}
			if (flag)
			{
				this._currentMessageTask = requestMessageTask;
				this._currentMessageTask.SetRequestData(this._userCertificate, this._address, this._port, this._isSecure, this._platformNetworkClient);
				restRequestMessage.SerializeAsJson();
				this._lastRequestOperationTime = this._timer.ElapsedMilliseconds;
				return;
			}
			Debug.Print("Setting new request message as failed because can't assign it", 0, Debug.DebugColor.White, 17592186044416UL);
			requestMessageTask.SetFinishedAsFailed();
		}

		// Token: 0x06000142 RID: 322 RVA: 0x0000413C File Offset: 0x0000233C
		private void RemoveRequestJob()
		{
			this._currentMessageTask = null;
		}

		// Token: 0x06000143 RID: 323 RVA: 0x00004148 File Offset: 0x00002348
		void IClientSession.Tick()
		{
			this.TryAssignJob();
			if (this._currentMessageTask != null)
			{
				this._currentMessageTask.Tick();
				if (this._currentMessageTask.IsCompletelyFinished)
				{
					if (this._currentMessageTask.Request.Successful)
					{
						if (this._currentMessageTask.RestRequestMessage is ConnectMessage)
						{
							this._currentConnectionResultType = ClientRestSession.ConnectionResultType.Connected;
							this._currentMessageTask.SetFinishedAsSuccessful(null);
						}
						else if (this._currentMessageTask.RestRequestMessage is DisconnectMessage)
						{
							this._currentConnectionResultType = ClientRestSession.ConnectionResultType.Disconnected;
							this._currentMessageTask.SetFinishedAsSuccessful(null);
						}
						else
						{
							string responseData = this._currentMessageTask.Request.ResponseData;
							if (!string.IsNullOrEmpty(responseData))
							{
								RestResponse restResponse = JsonConvert.DeserializeObject<RestResponse>(responseData, new JsonConverter[] { this._restDataJsonConverter });
								if (restResponse.Successful)
								{
									this._userCertificate = restResponse.UserCertificate;
									this._currentMessageTask.SetFinishedAsSuccessful(restResponse);
									while (restResponse.RemainingMessageCount > 0)
									{
										RestResponseMessage restResponseMessage = restResponse.TryDequeueMessage();
										this.HandleMessage(restResponseMessage.GetMessage());
									}
								}
								else
								{
									this._currentConnectionResultType = ClientRestSession.ConnectionResultType.Disconnected;
									Debug.Print("Setting current request message as failed because server returned unsuccessful response(" + restResponse.SuccessfulReason + ")", 0, Debug.DebugColor.White, 17592186044416UL);
									this._currentMessageTask.SetFinishedAsFailed(restResponse);
								}
							}
							else
							{
								this._currentConnectionResultType = ClientRestSession.ConnectionResultType.Disconnected;
								Debug.Print("Setting current request message as failed because server returned empty response", 0, Debug.DebugColor.White, 17592186044416UL);
								this._currentMessageTask.SetFinishedAsFailed();
							}
						}
					}
					else
					{
						if (this._currentMessageTask.RestRequestMessage is ConnectMessage)
						{
							this._currentConnectionResultType = ClientRestSession.ConnectionResultType.CantConnect;
						}
						else
						{
							this._currentConnectionResultType = ClientRestSession.ConnectionResultType.Disconnected;
						}
						Debug.Print("Setting current request message as failed because server request is failed", 0, Debug.DebugColor.White, 17592186044416UL);
						this._currentMessageTask.SetFinishedAsFailed();
					}
					this.RemoveRequestJob();
				}
				if (this._currentConnectionResultType != ClientRestSession.ConnectionResultType.None)
				{
					switch (this._currentConnectionResultType)
					{
					case ClientRestSession.ConnectionResultType.Connected:
						this.IsConnected = true;
						this.OnConnected();
						break;
					case ClientRestSession.ConnectionResultType.Disconnected:
						this.IsConnected = false;
						this.ClearMessageTaskQueueDueToDisconnect();
						this._sessionCredentials = null;
						this._sessionInitialized = false;
						this._userCertificate = null;
						this.ResetTimer();
						this.OnDisconnected();
						break;
					case ClientRestSession.ConnectionResultType.CantConnect:
						this._userCertificate = null;
						this.ResetTimer();
						this.OnCantConnect();
						break;
					}
					this._currentConnectionResultType = ClientRestSession.ConnectionResultType.None;
				}
			}
		}

		// Token: 0x06000144 RID: 324 RVA: 0x0000438C File Offset: 0x0000258C
		private void TryAssignJob()
		{
			if (this._currentMessageTask == null)
			{
				if (this._messageTaskQueue.Count > 0)
				{
					ClientRestSessionTask clientRestSessionTask = this._messageTaskQueue.Dequeue();
					this.AssignRequestJob(clientRestSessionTask);
					return;
				}
				if (this.IsConnected && this._sessionInitialized && this._timer.ElapsedMilliseconds - this._lastRequestOperationTime > (this.Client.IsInCriticalState ? ClientRestSession.CriticalStateCheckTime : this.Client.AliveCheckTimeInMiliSeconds) && this._userCertificate != null)
				{
					this.AssignRequestJob(new ClientRestSessionTask(new AliveMessage(this._sessionCredentials)));
				}
			}
		}

		// Token: 0x06000145 RID: 325 RVA: 0x00004428 File Offset: 0x00002628
		private void ClearMessageTaskQueueDueToDisconnect()
		{
			foreach (ClientRestSessionTask clientRestSessionTask in this._messageTaskQueue)
			{
				clientRestSessionTask.SetFinishedAsFailed();
			}
			this._messageTaskQueue.Clear();
		}

		// Token: 0x06000146 RID: 326 RVA: 0x00004484 File Offset: 0x00002684
		public void Connect()
		{
			this.ResetTimer();
			this.SendMessage(new ConnectMessage());
		}

		// Token: 0x06000147 RID: 327 RVA: 0x00004497 File Offset: 0x00002697
		public void Disconnect()
		{
			this.SendMessage(new DisconnectMessage());
			this.ResetTimer();
		}

		// Token: 0x06000148 RID: 328 RVA: 0x000044AA File Offset: 0x000026AA
		private void SendMessage(RestRequestMessage message)
		{
			this._messageTaskQueue.Enqueue(new ClientRestSessionTask(message));
		}

		// Token: 0x06000149 RID: 329 RVA: 0x000044C0 File Offset: 0x000026C0
		async Task<LoginResult> IClientSession.Login(LoginMessage message)
		{
			ClientRestSessionTask clientRestSessionTask = new ClientRestSessionTask(new RestDataRequestMessage(null, message, MessageType.Login));
			this._messageTaskQueue.Enqueue(clientRestSessionTask);
			await clientRestSessionTask.WaitUntilFinished();
			LoginResult loginResult;
			if (!clientRestSessionTask.Successful && !clientRestSessionTask.Request.Successful)
			{
				loginResult = new LoginResult(new TextObject("{=ahobSLlo}Login request failed", null));
			}
			else
			{
				RestFunctionResult functionResult = clientRestSessionTask.RestResponse.FunctionResult;
				LoginResult loginResult2 = null;
				if (functionResult != null)
				{
					loginResult2 = (LoginResult)functionResult.GetFunctionResult();
					if (clientRestSessionTask.Successful)
					{
						this._sessionCredentials = new SessionCredentials(loginResult2.PeerId, loginResult2.SessionKey);
						this._sessionInitialized = true;
					}
				}
				loginResult = loginResult2;
			}
			return loginResult;
		}

		// Token: 0x0600014A RID: 330 RVA: 0x0000450D File Offset: 0x0000270D
		void IClientSession.SendMessage(Message message)
		{
			this.SendMessage(new RestDataRequestMessage(this._sessionCredentials, message, MessageType.Message));
		}

		// Token: 0x0600014B RID: 331 RVA: 0x00004524 File Offset: 0x00002724
		async Task<TResult> IClientSession.CallFunction<TResult>(Message message)
		{
			ClientRestSessionTask clientRestSessionTask = new ClientRestSessionTask(new RestDataRequestMessage(this._sessionCredentials, message, MessageType.Function));
			this._messageTaskQueue.Enqueue(clientRestSessionTask);
			await clientRestSessionTask.WaitUntilFinished();
			if (clientRestSessionTask.Successful)
			{
				return (TResult)((object)clientRestSessionTask.RestResponse.FunctionResult.GetFunctionResult());
			}
			throw new Exception("Could not call function with " + message.GetType().Name);
		}

		// Token: 0x0600014C RID: 332 RVA: 0x00004571 File Offset: 0x00002771
		private void HandleMessage(Message message)
		{
			this.Client.HandleMessage(message);
		}

		// Token: 0x0600014D RID: 333 RVA: 0x0000457F File Offset: 0x0000277F
		private void OnConnected()
		{
			this.Client.OnConnected();
		}

		// Token: 0x0600014E RID: 334 RVA: 0x0000458C File Offset: 0x0000278C
		private void OnDisconnected()
		{
			this.Client.OnDisconnected();
		}

		// Token: 0x0600014F RID: 335 RVA: 0x00004599 File Offset: 0x00002799
		private void OnCantConnect()
		{
			this.Client.OnCantConnect();
		}

		// Token: 0x06000150 RID: 336 RVA: 0x000045A8 File Offset: 0x000027A8
		async Task<bool> IClientSession.CheckConnection()
		{
			bool flag;
			try
			{
				string text = "http://";
				if (this._isSecure)
				{
					text = "https://";
				}
				string text2 = string.Concat(new object[] { text, this._address, ":", this._port, "/Data/Ping" });
				await this._platformNetworkClient.HttpGetString(text2, false);
				flag = true;
			}
			catch
			{
				flag = false;
			}
			return flag;
		}

		// Token: 0x0400005F RID: 95
		private static readonly long CriticalStateCheckTime = 1000L;

		// Token: 0x04000060 RID: 96
		private readonly Queue<ClientRestSessionTask> _messageTaskQueue;

		// Token: 0x04000061 RID: 97
		private readonly string _address;

		// Token: 0x04000062 RID: 98
		private readonly ushort _port;

		// Token: 0x04000063 RID: 99
		private byte[] _userCertificate;

		// Token: 0x04000064 RID: 100
		private ClientRestSessionTask _currentMessageTask;

		// Token: 0x04000066 RID: 102
		private ClientRestSession.ConnectionResultType _currentConnectionResultType;

		// Token: 0x04000067 RID: 103
		private Stopwatch _timer;

		// Token: 0x04000068 RID: 104
		private long _lastRequestOperationTime;

		// Token: 0x04000069 RID: 105
		private bool _sessionInitialized;

		// Token: 0x0400006A RID: 106
		private SessionCredentials _sessionCredentials;

		// Token: 0x0400006C RID: 108
		private RestDataJsonConverter _restDataJsonConverter;

		// Token: 0x0400006D RID: 109
		private bool _isSecure;

		// Token: 0x0400006E RID: 110
		private IHttpDriver _platformNetworkClient;

		// Token: 0x0200006C RID: 108
		private enum ConnectionResultType
		{
			// Token: 0x04000118 RID: 280
			None,
			// Token: 0x04000119 RID: 281
			Connected,
			// Token: 0x0400011A RID: 282
			Disconnected,
			// Token: 0x0400011B RID: 283
			CantConnect
		}
	}
}
