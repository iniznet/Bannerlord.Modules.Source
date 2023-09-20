using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TaleWorlds.Diamond
{
	public class ThreadedClientSession : IClientSession
	{
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

		void IClientSession.Connect()
		{
			ThreadedClientSessionConnectTask threadedClientSessionConnectTask = new ThreadedClientSessionConnectTask(this._session);
			Queue<ThreadedClientSessionTask> tasks = this._tasks;
			lock (tasks)
			{
				this._tasks.Enqueue(threadedClientSessionConnectTask);
			}
		}

		void IClientSession.Disconnect()
		{
			ThreadedClientSessionDisconnectTask threadedClientSessionDisconnectTask = new ThreadedClientSessionDisconnectTask(this._session);
			Queue<ThreadedClientSessionTask> tasks = this._tasks;
			lock (tasks)
			{
				this._tasks.Enqueue(threadedClientSessionDisconnectTask);
			}
		}

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

		void IClientSession.SendMessage(Message message)
		{
			ThreadedClientSessionMessageTask threadedClientSessionMessageTask = new ThreadedClientSessionMessageTask(this._session, message);
			Queue<ThreadedClientSessionTask> tasks = this._tasks;
			lock (tasks)
			{
				this._tasks.Enqueue(threadedClientSessionMessageTask);
			}
		}

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

		Task<bool> IClientSession.CheckConnection()
		{
			return this._session.CheckConnection();
		}

		private IClientSession _session;

		private ThreadedClient _threadedClient;

		private Queue<ThreadedClientSessionTask> _tasks;

		private ThreadedClientSessionTask _task;

		private volatile bool _tasBegunJob;

		private readonly int _threadSleepTime;
	}
}
