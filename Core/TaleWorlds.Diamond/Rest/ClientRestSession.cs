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
	public class ClientRestSession : IClientSession
	{
		public bool IsConnected { get; private set; }

		public IClient Client { get; private set; }

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

		private void ResetTimer()
		{
			this._timer = new Stopwatch();
			this._timer.Start();
		}

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

		private void RemoveRequestJob()
		{
			this._currentMessageTask = null;
		}

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

		private void ClearMessageTaskQueueDueToDisconnect()
		{
			foreach (ClientRestSessionTask clientRestSessionTask in this._messageTaskQueue)
			{
				clientRestSessionTask.SetFinishedAsFailed();
			}
			this._messageTaskQueue.Clear();
		}

		public void Connect()
		{
			this.ResetTimer();
			this.SendMessage(new ConnectMessage());
		}

		public void Disconnect()
		{
			this.SendMessage(new DisconnectMessage());
			this.ResetTimer();
		}

		private void SendMessage(RestRequestMessage message)
		{
			this._messageTaskQueue.Enqueue(new ClientRestSessionTask(message));
		}

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

		void IClientSession.SendMessage(Message message)
		{
			this.SendMessage(new RestDataRequestMessage(this._sessionCredentials, message, MessageType.Message));
		}

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

		private void HandleMessage(Message message)
		{
			this.Client.HandleMessage(message);
		}

		private void OnConnected()
		{
			this.Client.OnConnected();
		}

		private void OnDisconnected()
		{
			this.Client.OnDisconnected();
		}

		private void OnCantConnect()
		{
			this.Client.OnCantConnect();
		}

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

		private static readonly long CriticalStateCheckTime = 1000L;

		private readonly Queue<ClientRestSessionTask> _messageTaskQueue;

		private readonly string _address;

		private readonly ushort _port;

		private byte[] _userCertificate;

		private ClientRestSessionTask _currentMessageTask;

		private ClientRestSession.ConnectionResultType _currentConnectionResultType;

		private Stopwatch _timer;

		private long _lastRequestOperationTime;

		private bool _sessionInitialized;

		private SessionCredentials _sessionCredentials;

		private RestDataJsonConverter _restDataJsonConverter;

		private bool _isSecure;

		private IHttpDriver _platformNetworkClient;

		private enum ConnectionResultType
		{
			None,
			Connected,
			Disconnected,
			CantConnect
		}
	}
}
