using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TaleWorlds.Diamond.InnerProcess
{
	// Token: 0x02000047 RID: 71
	public class InnerProcessClient : IClientSession, IInnerProcessClient
	{
		// Token: 0x17000052 RID: 82
		// (get) Token: 0x06000191 RID: 401 RVA: 0x000050C0 File Offset: 0x000032C0
		// (set) Token: 0x06000192 RID: 402 RVA: 0x000050C8 File Offset: 0x000032C8
		public InnerProcessManager InnerProcessManager { get; private set; }

		// Token: 0x17000053 RID: 83
		// (get) Token: 0x06000193 RID: 403 RVA: 0x000050D1 File Offset: 0x000032D1
		internal bool HasMessage
		{
			get
			{
				return this._receivedMessages.Count > 0;
			}
		}

		// Token: 0x06000194 RID: 404 RVA: 0x000050E1 File Offset: 0x000032E1
		public InnerProcessClient(InnerProcessManager innerProcessManager, IClient client, int port)
		{
			this.InnerProcessManager = innerProcessManager;
			this._receivedMessages = new Queue<Message>();
			this._associatedServerSession = null;
			this._client = client;
			this._port = port;
		}

		// Token: 0x06000195 RID: 405 RVA: 0x00005110 File Offset: 0x00003310
		void IClientSession.Connect()
		{
			this.InnerProcessManager.RequestConnection(this, this._port);
		}

		// Token: 0x06000196 RID: 406 RVA: 0x00005124 File Offset: 0x00003324
		void IClientSession.Disconnect()
		{
		}

		// Token: 0x06000197 RID: 407 RVA: 0x00005128 File Offset: 0x00003328
		void IClientSession.Tick()
		{
			while (this.HasMessage)
			{
				Message message = this.DequeueMessage();
				this.HandleMessage(message);
			}
		}

		// Token: 0x06000198 RID: 408 RVA: 0x0000514D File Offset: 0x0000334D
		void IInnerProcessClient.EnqueueMessage(Message message)
		{
			this._receivedMessages.Enqueue(message);
		}

		// Token: 0x06000199 RID: 409 RVA: 0x0000515C File Offset: 0x0000335C
		internal Message DequeueMessage()
		{
			Message message = null;
			if (this._receivedMessages.Count > 0)
			{
				message = this._receivedMessages.Dequeue();
			}
			return message;
		}

		// Token: 0x0600019A RID: 410 RVA: 0x00005188 File Offset: 0x00003388
		async Task<LoginResult> IClientSession.Login(LoginMessage message)
		{
			InnerProcessMessageTask innerProcessMessageTask = new InnerProcessMessageTask(this._sessionCredentials, message, InnerProcessMessageTaskType.Login);
			this._associatedServerSession.EnqueueMessageTask(innerProcessMessageTask);
			await innerProcessMessageTask.WaitUntilFinished();
			LoginResult loginResult = (LoginResult)innerProcessMessageTask.FunctionResult;
			this._sessionCredentials = new SessionCredentials(loginResult.PeerId, loginResult.SessionKey);
			return loginResult;
		}

		// Token: 0x0600019B RID: 411 RVA: 0x000051D8 File Offset: 0x000033D8
		void IClientSession.SendMessage(Message message)
		{
			InnerProcessMessageTask innerProcessMessageTask = new InnerProcessMessageTask(this._sessionCredentials, message, InnerProcessMessageTaskType.Message);
			this._associatedServerSession.EnqueueMessageTask(innerProcessMessageTask);
		}

		// Token: 0x0600019C RID: 412 RVA: 0x00005200 File Offset: 0x00003400
		async Task<TResult> IClientSession.CallFunction<TResult>(Message message)
		{
			InnerProcessMessageTask innerProcessMessageTask = new InnerProcessMessageTask(this._sessionCredentials, message, InnerProcessMessageTaskType.Function);
			this._associatedServerSession.EnqueueMessageTask(innerProcessMessageTask);
			await innerProcessMessageTask.WaitUntilFinished();
			return (TResult)((object)innerProcessMessageTask.FunctionResult);
		}

		// Token: 0x0600019D RID: 413 RVA: 0x0000524D File Offset: 0x0000344D
		void IInnerProcessClient.HandleConnected(InnerProcessServerSession serverSession)
		{
			this._associatedServerSession = serverSession;
			this.OnConnected();
		}

		// Token: 0x0600019E RID: 414 RVA: 0x0000525C File Offset: 0x0000345C
		private void HandleMessage(Message message)
		{
			this._client.HandleMessage(message);
		}

		// Token: 0x0600019F RID: 415 RVA: 0x0000526A File Offset: 0x0000346A
		private void OnConnected()
		{
			this._client.OnConnected();
		}

		// Token: 0x060001A0 RID: 416 RVA: 0x00005277 File Offset: 0x00003477
		private void OnCantConnect()
		{
			this._client.OnCantConnect();
		}

		// Token: 0x060001A1 RID: 417 RVA: 0x00005284 File Offset: 0x00003484
		private void OnDisconnected()
		{
			this._client.OnDisconnected();
		}

		// Token: 0x060001A2 RID: 418 RVA: 0x00005291 File Offset: 0x00003491
		public Task<bool> CheckConnection()
		{
			return this._client.CheckConnection();
		}

		// Token: 0x04000093 RID: 147
		private InnerProcessServerSession _associatedServerSession;

		// Token: 0x04000094 RID: 148
		private Queue<Message> _receivedMessages;

		// Token: 0x04000095 RID: 149
		private SessionCredentials _sessionCredentials;

		// Token: 0x04000096 RID: 150
		private IClient _client;

		// Token: 0x04000097 RID: 151
		private int _port;
	}
}
