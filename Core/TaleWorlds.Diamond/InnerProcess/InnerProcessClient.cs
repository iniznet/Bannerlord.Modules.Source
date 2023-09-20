using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TaleWorlds.Diamond.InnerProcess
{
	public class InnerProcessClient : IClientSession, IInnerProcessClient
	{
		public InnerProcessManager InnerProcessManager { get; private set; }

		internal bool HasMessage
		{
			get
			{
				return this._receivedMessages.Count > 0;
			}
		}

		public InnerProcessClient(InnerProcessManager innerProcessManager, IClient client, int port)
		{
			this.InnerProcessManager = innerProcessManager;
			this._receivedMessages = new Queue<Message>();
			this._associatedServerSession = null;
			this._client = client;
			this._port = port;
		}

		void IClientSession.Connect()
		{
			this.InnerProcessManager.RequestConnection(this, this._port);
		}

		void IClientSession.Disconnect()
		{
		}

		void IClientSession.Tick()
		{
			while (this.HasMessage)
			{
				Message message = this.DequeueMessage();
				this.HandleMessage(message);
			}
		}

		void IInnerProcessClient.EnqueueMessage(Message message)
		{
			this._receivedMessages.Enqueue(message);
		}

		internal Message DequeueMessage()
		{
			Message message = null;
			if (this._receivedMessages.Count > 0)
			{
				message = this._receivedMessages.Dequeue();
			}
			return message;
		}

		async Task<LoginResult> IClientSession.Login(LoginMessage message)
		{
			InnerProcessMessageTask innerProcessMessageTask = new InnerProcessMessageTask(this._sessionCredentials, message, InnerProcessMessageTaskType.Login);
			this._associatedServerSession.EnqueueMessageTask(innerProcessMessageTask);
			await innerProcessMessageTask.WaitUntilFinished();
			LoginResult loginResult = (LoginResult)innerProcessMessageTask.FunctionResult;
			this._sessionCredentials = new SessionCredentials(loginResult.PeerId, loginResult.SessionKey);
			return loginResult;
		}

		void IClientSession.SendMessage(Message message)
		{
			InnerProcessMessageTask innerProcessMessageTask = new InnerProcessMessageTask(this._sessionCredentials, message, InnerProcessMessageTaskType.Message);
			this._associatedServerSession.EnqueueMessageTask(innerProcessMessageTask);
		}

		async Task<TResult> IClientSession.CallFunction<TResult>(Message message)
		{
			InnerProcessMessageTask innerProcessMessageTask = new InnerProcessMessageTask(this._sessionCredentials, message, InnerProcessMessageTaskType.Function);
			this._associatedServerSession.EnqueueMessageTask(innerProcessMessageTask);
			await innerProcessMessageTask.WaitUntilFinished();
			return (TResult)((object)innerProcessMessageTask.FunctionResult);
		}

		void IInnerProcessClient.HandleConnected(InnerProcessServerSession serverSession)
		{
			this._associatedServerSession = serverSession;
			this.OnConnected();
		}

		private void HandleMessage(Message message)
		{
			this._client.HandleMessage(message);
		}

		private void OnConnected()
		{
			this._client.OnConnected();
		}

		private void OnCantConnect()
		{
			this._client.OnCantConnect();
		}

		private void OnDisconnected()
		{
			this._client.OnDisconnected();
		}

		public Task<bool> CheckConnection()
		{
			return this._client.CheckConnection();
		}

		private InnerProcessServerSession _associatedServerSession;

		private Queue<Message> _receivedMessages;

		private SessionCredentials _sessionCredentials;

		private IClient _client;

		private int _port;
	}
}
