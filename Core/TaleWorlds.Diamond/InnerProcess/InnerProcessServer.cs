using System;
using System.Collections.Generic;

namespace TaleWorlds.Diamond.InnerProcess
{
	// Token: 0x0200004B RID: 75
	public abstract class InnerProcessServer<T> : IInnerProcessServer where T : InnerProcessServerSession, new()
	{
		// Token: 0x17000056 RID: 86
		// (get) Token: 0x060001AE RID: 430 RVA: 0x000053D8 File Offset: 0x000035D8
		// (set) Token: 0x060001AF RID: 431 RVA: 0x000053E0 File Offset: 0x000035E0
		public InnerProcessManager InnerProcessManager { get; private set; }

		// Token: 0x17000057 RID: 87
		// (get) Token: 0x060001B0 RID: 432 RVA: 0x000053E9 File Offset: 0x000035E9
		public IEnumerable<T> Sessions
		{
			get
			{
				return this._clientSessions;
			}
		}

		// Token: 0x060001B1 RID: 433 RVA: 0x000053F1 File Offset: 0x000035F1
		protected InnerProcessServer(InnerProcessManager innerProcessManager)
		{
			this.InnerProcessManager = innerProcessManager;
			this._clientSessions = new List<T>();
		}

		// Token: 0x060001B2 RID: 434 RVA: 0x0000540B File Offset: 0x0000360B
		public void Host(int port)
		{
			this.InnerProcessManager.Activate(this, port);
		}

		// Token: 0x060001B3 RID: 435 RVA: 0x0000541C File Offset: 0x0000361C
		InnerProcessServerSession IInnerProcessServer.AddNewConnection(IInnerProcessClient client)
		{
			T t = new T();
			this._clientSessions.Add(t);
			return t;
		}

		// Token: 0x060001B4 RID: 436 RVA: 0x00005444 File Offset: 0x00003644
		void IInnerProcessServer.Update()
		{
			foreach (T t in this._clientSessions)
			{
				while (t.HasMessage)
				{
					InnerProcessMessageTask innerProcessMessageTask = t.DequeueMessage();
					SessionCredentials sessionCredentials = innerProcessMessageTask.SessionCredentials;
					Message message = innerProcessMessageTask.Message;
					switch (innerProcessMessageTask.Type)
					{
					case InnerProcessMessageTaskType.Login:
					{
						LoginResult loginResult = this.Login(t, (LoginMessage)message, new InnerProcessConnectionInformation());
						if (loginResult.Successful)
						{
							innerProcessMessageTask.SetFinishedAsSuccessful(loginResult);
						}
						break;
					}
					case InnerProcessMessageTaskType.Message:
						this.HandleMessage(t, sessionCredentials, message);
						innerProcessMessageTask.SetFinishedAsSuccessful(null);
						break;
					case InnerProcessMessageTaskType.Function:
					{
						Tuple<HandlerResult, FunctionResult> tuple = this.CallFunction(t, sessionCredentials, message);
						if (tuple.Item1.IsSuccessful)
						{
							innerProcessMessageTask.SetFinishedAsSuccessful(tuple.Item2);
						}
						break;
					}
					}
				}
			}
			this.OnUpdate();
		}

		// Token: 0x060001B5 RID: 437
		protected abstract void HandleMessage(T serverSession, SessionCredentials sessionCredentials, Message message);

		// Token: 0x060001B6 RID: 438
		protected abstract Tuple<HandlerResult, FunctionResult> CallFunction(T serverSession, SessionCredentials sessionCredentials, Message message);

		// Token: 0x060001B7 RID: 439
		protected abstract LoginResult Login(T serverSession, LoginMessage message, InnerProcessConnectionInformation connectionInformation);

		// Token: 0x060001B8 RID: 440 RVA: 0x0000554C File Offset: 0x0000374C
		protected virtual void OnUpdate()
		{
		}

		// Token: 0x0400009D RID: 157
		private List<T> _clientSessions;
	}
}
