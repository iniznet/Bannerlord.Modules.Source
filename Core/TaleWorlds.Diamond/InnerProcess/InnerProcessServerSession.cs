using System;
using System.Collections.Generic;

namespace TaleWorlds.Diamond.InnerProcess
{
	// Token: 0x0200004D RID: 77
	public class InnerProcessServerSession
	{
		// Token: 0x17000058 RID: 88
		// (get) Token: 0x060001BB RID: 443 RVA: 0x0000554E File Offset: 0x0000374E
		internal bool HasMessage
		{
			get
			{
				return this._messageTasks.Count > 0;
			}
		}

		// Token: 0x060001BC RID: 444 RVA: 0x0000555E File Offset: 0x0000375E
		public InnerProcessServerSession()
		{
			this._messageTasks = new Queue<InnerProcessMessageTask>();
		}

		// Token: 0x060001BD RID: 445 RVA: 0x00005571 File Offset: 0x00003771
		public void SendMessage(Message message)
		{
			this._associatedClientSession.EnqueueMessage(message);
		}

		// Token: 0x060001BE RID: 446 RVA: 0x0000557F File Offset: 0x0000377F
		internal void EnqueueMessageTask(InnerProcessMessageTask messageTask)
		{
			this._messageTasks.Enqueue(messageTask);
		}

		// Token: 0x060001BF RID: 447 RVA: 0x00005590 File Offset: 0x00003790
		internal InnerProcessMessageTask DequeueMessage()
		{
			InnerProcessMessageTask innerProcessMessageTask = null;
			if (this._messageTasks.Count > 0)
			{
				innerProcessMessageTask = this._messageTasks.Dequeue();
			}
			return innerProcessMessageTask;
		}

		// Token: 0x060001C0 RID: 448 RVA: 0x000055BA File Offset: 0x000037BA
		internal void HandleConnected(IInnerProcessClient clientSession)
		{
			this._associatedClientSession = clientSession;
		}

		// Token: 0x17000059 RID: 89
		// (get) Token: 0x060001C1 RID: 449 RVA: 0x000055C3 File Offset: 0x000037C3
		// (set) Token: 0x060001C2 RID: 450 RVA: 0x000055CB File Offset: 0x000037CB
		public PeerId PeerId { get; private set; }

		// Token: 0x1700005A RID: 90
		// (get) Token: 0x060001C3 RID: 451 RVA: 0x000055D4 File Offset: 0x000037D4
		// (set) Token: 0x060001C4 RID: 452 RVA: 0x000055DC File Offset: 0x000037DC
		public SessionKey SessionKey { get; private set; }

		// Token: 0x060001C5 RID: 453 RVA: 0x000055E5 File Offset: 0x000037E5
		public void AssignSession(PeerId peerId, SessionKey sessionKey)
		{
			this.PeerId = peerId;
			this.SessionKey = sessionKey;
		}

		// Token: 0x0400009E RID: 158
		private Queue<InnerProcessMessageTask> _messageTasks;

		// Token: 0x0400009F RID: 159
		private IInnerProcessClient _associatedClientSession;
	}
}
