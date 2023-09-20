using System;
using System.Collections.Generic;

namespace TaleWorlds.Diamond.InnerProcess
{
	public abstract class InnerProcessServer<T> : IInnerProcessServer where T : InnerProcessServerSession, new()
	{
		public InnerProcessManager InnerProcessManager { get; private set; }

		public IEnumerable<T> Sessions
		{
			get
			{
				return this._clientSessions;
			}
		}

		protected InnerProcessServer(InnerProcessManager innerProcessManager)
		{
			this.InnerProcessManager = innerProcessManager;
			this._clientSessions = new List<T>();
		}

		public void Host(int port)
		{
			this.InnerProcessManager.Activate(this, port);
		}

		InnerProcessServerSession IInnerProcessServer.AddNewConnection(IInnerProcessClient client)
		{
			T t = new T();
			this._clientSessions.Add(t);
			return t;
		}

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

		protected abstract void HandleMessage(T serverSession, SessionCredentials sessionCredentials, Message message);

		protected abstract Tuple<HandlerResult, FunctionResult> CallFunction(T serverSession, SessionCredentials sessionCredentials, Message message);

		protected abstract LoginResult Login(T serverSession, LoginMessage message, InnerProcessConnectionInformation connectionInformation);

		protected virtual void OnUpdate()
		{
		}

		private List<T> _clientSessions;
	}
}
