using System;
using System.Threading.Tasks;

namespace TaleWorlds.Network
{
	// Token: 0x0200000F RID: 15
	public abstract class MessageServiceConnection
	{
		// Token: 0x06000047 RID: 71 RVA: 0x000028C6 File Offset: 0x00000AC6
		public MessageServiceConnection()
		{
		}

		// Token: 0x06000048 RID: 72
		public abstract Task SendAsync(string text);

		// Token: 0x06000049 RID: 73
		public abstract void Init(string address, string token);

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x0600004A RID: 74 RVA: 0x000028CE File Offset: 0x00000ACE
		// (set) Token: 0x0600004B RID: 75 RVA: 0x000028D6 File Offset: 0x00000AD6
		public string Address { get; protected set; }

		// Token: 0x14000005 RID: 5
		// (add) Token: 0x0600004C RID: 76 RVA: 0x000028E0 File Offset: 0x00000AE0
		// (remove) Token: 0x0600004D RID: 77 RVA: 0x00002918 File Offset: 0x00000B18
		public event MessageServiceConnection.ClosedDelegate Closed;

		// Token: 0x14000006 RID: 6
		// (add) Token: 0x0600004E RID: 78 RVA: 0x00002950 File Offset: 0x00000B50
		// (remove) Token: 0x0600004F RID: 79 RVA: 0x00002988 File Offset: 0x00000B88
		public event MessageServiceConnection.StateChangedDelegate StateChanged;

		// Token: 0x06000050 RID: 80
		public abstract void RegisterProxyClient(string name, IMessageProxyClient playerClient);

		// Token: 0x06000051 RID: 81
		public abstract Task StartAsync();

		// Token: 0x06000052 RID: 82
		public abstract Task StopAsync();

		// Token: 0x06000053 RID: 83 RVA: 0x000029BD File Offset: 0x00000BBD
		protected void InvokeClosed()
		{
			MessageServiceConnection.ClosedDelegate closed = this.Closed;
			if (closed == null)
			{
				return;
			}
			closed();
		}

		// Token: 0x06000054 RID: 84 RVA: 0x000029D0 File Offset: 0x00000BD0
		protected void InvokeStateChanged(ConnectionState oldState, ConnectionState newState)
		{
			this.State = newState;
			this.OldState = oldState;
			MessageServiceConnection.StateChangedDelegate stateChanged = this.StateChanged;
			if (stateChanged == null)
			{
				return;
			}
			stateChanged(oldState, newState);
		}

		// Token: 0x04000022 RID: 34
		public ConnectionState State;

		// Token: 0x04000023 RID: 35
		public ConnectionState OldState;

		// Token: 0x02000037 RID: 55
		// (Invoke) Token: 0x0600017A RID: 378
		public delegate Task ClosedDelegate();

		// Token: 0x02000038 RID: 56
		// (Invoke) Token: 0x0600017E RID: 382
		public delegate void StateChangedDelegate(ConnectionState oldState, ConnectionState newState);
	}
}
