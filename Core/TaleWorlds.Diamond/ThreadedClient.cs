using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TaleWorlds.Diamond
{
	// Token: 0x02000020 RID: 32
	public class ThreadedClient : IClient
	{
		// Token: 0x17000020 RID: 32
		// (get) Token: 0x06000095 RID: 149 RVA: 0x00002D56 File Offset: 0x00000F56
		public ILoginAccessProvider AccessProvider
		{
			get
			{
				return this._client.AccessProvider;
			}
		}

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x06000096 RID: 150 RVA: 0x00002D63 File Offset: 0x00000F63
		public bool IsInCriticalState
		{
			get
			{
				return this._client.IsInCriticalState;
			}
		}

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x06000097 RID: 151 RVA: 0x00002D70 File Offset: 0x00000F70
		public long AliveCheckTimeInMiliSeconds
		{
			get
			{
				return this._client.AliveCheckTimeInMiliSeconds;
			}
		}

		// Token: 0x06000098 RID: 152 RVA: 0x00002D7D File Offset: 0x00000F7D
		public ThreadedClient(IClient client)
		{
			this._client = client;
			this._tasks = new Queue<ThreadedClientTask>();
		}

		// Token: 0x06000099 RID: 153 RVA: 0x00002D98 File Offset: 0x00000F98
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

		// Token: 0x0600009A RID: 154 RVA: 0x00002DF8 File Offset: 0x00000FF8
		void IClient.HandleMessage(Message message)
		{
			ThreadedClientHandleMessageTask threadedClientHandleMessageTask = new ThreadedClientHandleMessageTask(this._client, message);
			Queue<ThreadedClientTask> tasks = this._tasks;
			lock (tasks)
			{
				this._tasks.Enqueue(threadedClientHandleMessageTask);
			}
		}

		// Token: 0x0600009B RID: 155 RVA: 0x00002E4C File Offset: 0x0000104C
		void IClient.OnConnected()
		{
			ThreadedClientConnectedTask threadedClientConnectedTask = new ThreadedClientConnectedTask(this._client);
			Queue<ThreadedClientTask> tasks = this._tasks;
			lock (tasks)
			{
				this._tasks.Enqueue(threadedClientConnectedTask);
			}
		}

		// Token: 0x0600009C RID: 156 RVA: 0x00002EA0 File Offset: 0x000010A0
		void IClient.OnDisconnected()
		{
			ThreadedClientDisconnectedTask threadedClientDisconnectedTask = new ThreadedClientDisconnectedTask(this._client);
			Queue<ThreadedClientTask> tasks = this._tasks;
			lock (tasks)
			{
				this._tasks.Enqueue(threadedClientDisconnectedTask);
			}
		}

		// Token: 0x0600009D RID: 157 RVA: 0x00002EF4 File Offset: 0x000010F4
		void IClient.OnCantConnect()
		{
			ThreadedClientCantConnectTask threadedClientCantConnectTask = new ThreadedClientCantConnectTask(this._client);
			Queue<ThreadedClientTask> tasks = this._tasks;
			lock (tasks)
			{
				this._tasks.Enqueue(threadedClientCantConnectTask);
			}
		}

		// Token: 0x0600009E RID: 158 RVA: 0x00002F48 File Offset: 0x00001148
		public Task<bool> CheckConnection()
		{
			return this._client.CheckConnection();
		}

		// Token: 0x04000028 RID: 40
		private IClient _client;

		// Token: 0x04000029 RID: 41
		private Queue<ThreadedClientTask> _tasks;
	}
}
