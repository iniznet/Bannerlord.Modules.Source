using System;
using System.Collections.Generic;
using System.Threading;

namespace TaleWorlds.Library
{
	// Token: 0x02000083 RID: 131
	public sealed class SingleThreadedSynchronizationContext : SynchronizationContext
	{
		// Token: 0x06000479 RID: 1145 RVA: 0x0000EA95 File Offset: 0x0000CC95
		public SingleThreadedSynchronizationContext()
		{
			this._worksLock = new object();
			this._futureWorks = new List<SingleThreadedSynchronizationContext.WorkRequest>(100);
			this._currentWorks = new List<SingleThreadedSynchronizationContext.WorkRequest>(100);
			this._mainThreadId = Thread.CurrentThread.ManagedThreadId;
		}

		// Token: 0x0600047A RID: 1146 RVA: 0x0000EAD4 File Offset: 0x0000CCD4
		public override void Send(SendOrPostCallback callback, object state)
		{
			if (this._mainThreadId == Thread.CurrentThread.ManagedThreadId)
			{
				callback.DynamicInvokeWithLog(new object[] { state });
				return;
			}
			using (ManualResetEvent manualResetEvent = new ManualResetEvent(false))
			{
				object worksLock = this._worksLock;
				lock (worksLock)
				{
					this._futureWorks.Add(new SingleThreadedSynchronizationContext.WorkRequest(callback, state, manualResetEvent));
				}
				manualResetEvent.WaitOne();
			}
		}

		// Token: 0x0600047B RID: 1147 RVA: 0x0000EB6C File Offset: 0x0000CD6C
		public override void Post(SendOrPostCallback callback, object state)
		{
			SingleThreadedSynchronizationContext.WorkRequest workRequest = new SingleThreadedSynchronizationContext.WorkRequest(callback, state, null);
			object worksLock = this._worksLock;
			lock (worksLock)
			{
				this._futureWorks.Add(workRequest);
			}
		}

		// Token: 0x0600047C RID: 1148 RVA: 0x0000EBBC File Offset: 0x0000CDBC
		public void Tick()
		{
			object worksLock = this._worksLock;
			lock (worksLock)
			{
				List<SingleThreadedSynchronizationContext.WorkRequest> currentWorks = this._currentWorks;
				this._currentWorks = this._futureWorks;
				this._futureWorks = currentWorks;
			}
			foreach (SingleThreadedSynchronizationContext.WorkRequest workRequest in this._currentWorks)
			{
				workRequest.Invoke();
			}
			this._currentWorks.Clear();
		}

		// Token: 0x0400015B RID: 347
		private List<SingleThreadedSynchronizationContext.WorkRequest> _futureWorks;

		// Token: 0x0400015C RID: 348
		private List<SingleThreadedSynchronizationContext.WorkRequest> _currentWorks;

		// Token: 0x0400015D RID: 349
		private readonly object _worksLock;

		// Token: 0x0400015E RID: 350
		private readonly int _mainThreadId;

		// Token: 0x020000D0 RID: 208
		private struct WorkRequest
		{
			// Token: 0x060006EA RID: 1770 RVA: 0x00015323 File Offset: 0x00013523
			public WorkRequest(SendOrPostCallback callback, object state, ManualResetEvent waitHandle = null)
			{
				this._callback = callback;
				this._state = state;
				this._waitHandle = waitHandle;
			}

			// Token: 0x060006EB RID: 1771 RVA: 0x0001533A File Offset: 0x0001353A
			public void Invoke()
			{
				this._callback.DynamicInvokeWithLog(new object[] { this._state });
				ManualResetEvent waitHandle = this._waitHandle;
				if (waitHandle == null)
				{
					return;
				}
				waitHandle.Set();
			}

			// Token: 0x04000298 RID: 664
			private readonly SendOrPostCallback _callback;

			// Token: 0x04000299 RID: 665
			private readonly object _state;

			// Token: 0x0400029A RID: 666
			private readonly ManualResetEvent _waitHandle;
		}
	}
}
