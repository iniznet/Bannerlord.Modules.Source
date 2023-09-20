using System;
using System.Collections.Generic;

namespace TaleWorlds.Diamond.InnerProcess
{
	public class InnerProcessServerSession
	{
		internal bool HasMessage
		{
			get
			{
				return this._messageTasks.Count > 0;
			}
		}

		public InnerProcessServerSession()
		{
			this._messageTasks = new Queue<InnerProcessMessageTask>();
		}

		public void SendMessage(Message message)
		{
			this._associatedClientSession.EnqueueMessage(message);
		}

		internal void EnqueueMessageTask(InnerProcessMessageTask messageTask)
		{
			this._messageTasks.Enqueue(messageTask);
		}

		internal InnerProcessMessageTask DequeueMessage()
		{
			InnerProcessMessageTask innerProcessMessageTask = null;
			if (this._messageTasks.Count > 0)
			{
				innerProcessMessageTask = this._messageTasks.Dequeue();
			}
			return innerProcessMessageTask;
		}

		internal void HandleConnected(IInnerProcessClient clientSession)
		{
			this._associatedClientSession = clientSession;
		}

		public PeerId PeerId { get; private set; }

		public SessionKey SessionKey { get; private set; }

		public void AssignSession(PeerId peerId, SessionKey sessionKey)
		{
			this.PeerId = peerId;
			this.SessionKey = sessionKey;
		}

		private Queue<InnerProcessMessageTask> _messageTasks;

		private IInnerProcessClient _associatedClientSession;
	}
}
