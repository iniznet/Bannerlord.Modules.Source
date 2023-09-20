using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TaleWorlds.Diamond
{
	public class ThreadedClient : IClient
	{
		public ILoginAccessProvider AccessProvider
		{
			get
			{
				return this._client.AccessProvider;
			}
		}

		public bool IsInCriticalState
		{
			get
			{
				return this._client.IsInCriticalState;
			}
		}

		public long AliveCheckTimeInMiliSeconds
		{
			get
			{
				return this._client.AliveCheckTimeInMiliSeconds;
			}
		}

		public ThreadedClient(IClient client)
		{
			this._client = client;
			this._tasks = new Queue<ThreadedClientTask>();
		}

		public void Tick()
		{
			ThreadedClientTask threadedClientTask = null;
			Queue<ThreadedClientTask> tasks = this._tasks;
			lock (tasks)
			{
				if (this._tasks.Count > 0)
				{
					threadedClientTask = this._tasks.Dequeue();
				}
			}
			if (threadedClientTask != null)
			{
				threadedClientTask.DoJob();
			}
		}

		void IClient.HandleMessage(Message message)
		{
			ThreadedClientHandleMessageTask threadedClientHandleMessageTask = new ThreadedClientHandleMessageTask(this._client, message);
			Queue<ThreadedClientTask> tasks = this._tasks;
			lock (tasks)
			{
				this._tasks.Enqueue(threadedClientHandleMessageTask);
			}
		}

		void IClient.OnConnected()
		{
			ThreadedClientConnectedTask threadedClientConnectedTask = new ThreadedClientConnectedTask(this._client);
			Queue<ThreadedClientTask> tasks = this._tasks;
			lock (tasks)
			{
				this._tasks.Enqueue(threadedClientConnectedTask);
			}
		}

		void IClient.OnDisconnected()
		{
			ThreadedClientDisconnectedTask threadedClientDisconnectedTask = new ThreadedClientDisconnectedTask(this._client);
			Queue<ThreadedClientTask> tasks = this._tasks;
			lock (tasks)
			{
				this._tasks.Enqueue(threadedClientDisconnectedTask);
			}
		}

		void IClient.OnCantConnect()
		{
			ThreadedClientCantConnectTask threadedClientCantConnectTask = new ThreadedClientCantConnectTask(this._client);
			Queue<ThreadedClientTask> tasks = this._tasks;
			lock (tasks)
			{
				this._tasks.Enqueue(threadedClientCantConnectTask);
			}
		}

		public Task<bool> CheckConnection()
		{
			return this._client.CheckConnection();
		}

		private IClient _client;

		private Queue<ThreadedClientTask> _tasks;
	}
}
