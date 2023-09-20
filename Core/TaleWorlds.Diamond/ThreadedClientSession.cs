using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TaleWorlds.Diamond
{
	// Token: 0x02000026 RID: 38
	public class ThreadedClientSession : IClientSession
	{
		// Token: 0x060000AD RID: 173 RVA: 0x00002FEB File Offset: 0x000011EB
		public ThreadedClientSession(ThreadedClient threadedClient, IClientSession session, int threadSleepTime)
		{
			this._session = session;
			this._threadedClient = threadedClient;
			this._tasks = new Queue<ThreadedClientSessionTask>();
			this._task = null;
			this._tasBegunJob = false;
			this._threadSleepTime = threadSleepTime;
			this.RefreshTask(null);
		}

		// Token: 0x060000AE RID: 174 RVA: 0x0000302C File Offset: 0x0000122C
		private void RefreshTask(Task previousTask)
		{
			if (previousTask == null || previousTask.IsCompleted)
			{
				Task.Run(async delegate
				{
					this.ThreadMain();
					await Task.Delay(this._threadSleepTime);
				}).ContinueWith(delegate(Task t)
				{
					this.RefreshTask(t);
				}, TaskContinuationOptions.ExecuteSynchronously);
				return;
			}
			if (previousTask.IsFaulted)
			{
				throw new Exception("ThreadedClientSession.ThreadMain Task is faulted", previousTask.Exception);
			}
			throw new Exception("RefreshTask is called before task is completed");
		}

		// Token: 0x060000AF RID: 175 RVA: 0x00003090 File Offset: 0x00001290
		private void ThreadMain()
		{
			this._session.Tick();
			if (!this._tasBegunJob)
			{
				Queue<ThreadedClientSessionTask> tasks = this._tasks;
				lock (tasks)
				{
					if (this._tasks.Count > 0)
					{
						this._task = this._tasks.Dequeue();
					}
				}
				if (this._task != null)
				{
					this._task.BeginJob();
					this._tasBegunJob = true;
				}
			}
		}

		// Token: 0x060000B0 RID: 176 RVA: 0x0000311C File Offset: 0x0000131C
		void IClientSession.Connect()
		{
			ThreadedClientSessionConnectTask threadedClientSessionConnectTask = new ThreadedClientSessionConnectTask(this._session);
			Queue<ThreadedClientSessionTask> tasks = this._tasks;
			lock (tasks)
			{
				this._tasks.Enqueue(threadedClientSessionConnectTask);
			}
		}

		// Token: 0x060000B1 RID: 177 RVA: 0x00003170 File Offset: 0x00001370
		void IClientSession.Disconnect()
		{
			ThreadedClientSessionDisconnectTask threadedClientSessionDisconnectTask = new ThreadedClientSessionDisconnectTask(this._session);
			Queue<ThreadedClientSessionTask> tasks = this._tasks;
			lock (tasks)
			{
				this._tasks.Enqueue(threadedClientSessionDisconnectTask);
			}
		}

		// Token: 0x060000B2 RID: 178 RVA: 0x000031C4 File Offset: 0x000013C4
		void IClientSession.Tick()
		{
			this._threadedClient.Tick();
			if (this._tasBegunJob)
			{
				this._task.DoMainThreadJob();
				if (this._task.Finished)
				{
					this._task = null;
					this._tasBegunJob = false;
				}
			}
		}

		// Token: 0x060000B3 RID: 179 RVA: 0x00003204 File Offset: 0x00001404
		async Task<LoginResult> IClientSession.Login(LoginMessage message)
		{
			ThreadedClientSessionLoginTask task = new ThreadedClientSessionLoginTask(this._session, message);
			Queue<ThreadedClientSessionTask> tasks = this._tasks;
			lock (tasks)
			{
				this._tasks.Enqueue(task);
			}
			await task.Wait();
			return task.LoginResult;
		}

		// Token: 0x060000B4 RID: 180 RVA: 0x00003254 File Offset: 0x00001454
		void IClientSession.SendMessage(Message message)
		{
			ThreadedClientSessionMessageTask threadedClientSessionMessageTask = new ThreadedClientSessionMessageTask(this._session, message);
			Queue<ThreadedClientSessionTask> tasks = this._tasks;
			lock (tasks)
			{
				this._tasks.Enqueue(threadedClientSessionMessageTask);
			}
		}

		// Token: 0x060000B5 RID: 181 RVA: 0x000032A8 File Offset: 0x000014A8
		async Task<TReturn> IClientSession.CallFunction<TReturn>(Message message)
		{
			ThreadedClientSessionFunctionTask task = new ThreadedClientSessionFunctionTask(this._session, message);
			Queue<ThreadedClientSessionTask> tasks = this._tasks;
			lock (tasks)
			{
				this._tasks.Enqueue(task);
			}
			await task.Wait();
			return (TReturn)((object)task.FunctionResult);
		}

		// Token: 0x060000B6 RID: 182 RVA: 0x000032F5 File Offset: 0x000014F5
		Task<bool> IClientSession.CheckConnection()
		{
			return this._session.CheckConnection();
		}

		// Token: 0x0400002C RID: 44
		private IClientSession _session;

		// Token: 0x0400002D RID: 45
		private ThreadedClient _threadedClient;

		// Token: 0x0400002E RID: 46
		private Queue<ThreadedClientSessionTask> _tasks;

		// Token: 0x0400002F RID: 47
		private ThreadedClientSessionTask _task;

		// Token: 0x04000030 RID: 48
		private volatile bool _tasBegunJob;

		// Token: 0x04000031 RID: 49
		private readonly int _threadSleepTime;
	}
}
