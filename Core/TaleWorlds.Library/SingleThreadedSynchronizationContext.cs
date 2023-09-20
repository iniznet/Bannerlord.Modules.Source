using System;
using System.Collections.Generic;
using System.Threading;

namespace TaleWorlds.Library
{
	public sealed class SingleThreadedSynchronizationContext : SynchronizationContext
	{
		public SingleThreadedSynchronizationContext()
		{
			this._worksLock = new object();
			this._futureWorks = new List<SingleThreadedSynchronizationContext.WorkRequest>(100);
			this._currentWorks = new List<SingleThreadedSynchronizationContext.WorkRequest>(100);
			this._mainThreadId = Thread.CurrentThread.ManagedThreadId;
		}

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

		public override void Post(SendOrPostCallback callback, object state)
		{
			SingleThreadedSynchronizationContext.WorkRequest workRequest = new SingleThreadedSynchronizationContext.WorkRequest(callback, state, null);
			object worksLock = this._worksLock;
			lock (worksLock)
			{
				this._futureWorks.Add(workRequest);
			}
		}

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

		private List<SingleThreadedSynchronizationContext.WorkRequest> _futureWorks;

		private List<SingleThreadedSynchronizationContext.WorkRequest> _currentWorks;

		private readonly object _worksLock;

		private readonly int _mainThreadId;

		private struct WorkRequest
		{
			public WorkRequest(SendOrPostCallback callback, object state, ManualResetEvent waitHandle = null)
			{
				this._callback = callback;
				this._state = state;
				this._waitHandle = waitHandle;
			}

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

			private readonly SendOrPostCallback _callback;

			private readonly object _state;

			private readonly ManualResetEvent _waitHandle;
		}
	}
}
