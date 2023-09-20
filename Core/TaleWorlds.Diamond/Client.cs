using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using TaleWorlds.Diamond.ClientApplication;
using TaleWorlds.Library;

namespace TaleWorlds.Diamond
{
	// Token: 0x02000003 RID: 3
	public abstract class Client<T> : DiamondClientApplicationObject, IClient where T : Client<T>
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000006 RID: 6 RVA: 0x00002338 File Offset: 0x00000538
		// (set) Token: 0x06000007 RID: 7 RVA: 0x00002340 File Offset: 0x00000540
		public bool IsInCriticalState { get; set; }

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000008 RID: 8 RVA: 0x00002349 File Offset: 0x00000549
		public virtual long AliveCheckTimeInMiliSeconds
		{
			get
			{
				return 2000L;
			}
		}

		// Token: 0x06000009 RID: 9 RVA: 0x00002351 File Offset: 0x00000551
		protected Client(DiamondClientApplication diamondClientApplication, IClientSessionProvider<T> sessionProvider, bool autoReconnect)
			: base(diamondClientApplication)
		{
			this._clientSession = sessionProvider.CreateSession((T)((object)this));
			this._messageHandlers = new Dictionary<Type, Delegate>();
			this._autoReconnect = autoReconnect;
			if (autoReconnect)
			{
				this.Reset();
				this._connectionState = Client<T>.ConnectionState.ReadyToConnect;
			}
		}

		// Token: 0x0600000A RID: 10 RVA: 0x00002390 File Offset: 0x00000590
		public void Update()
		{
			this._clientSession.Tick();
			if (this._connectionState == Client<T>.ConnectionState.SleepingToConnectAgain)
			{
				if (this._timer.ElapsedMilliseconds > 5000L)
				{
					this._connectionState = Client<T>.ConnectionState.ReadyToConnect;
					this._timer.Stop();
					this._timer = null;
				}
			}
			else if (this._connectionState == Client<T>.ConnectionState.ReadyToConnect)
			{
				this._connectionState = Client<T>.ConnectionState.Connecting;
				this._clientSession.Connect();
			}
			else
			{
				Client<T>.ConnectionState connectionState = this._connectionState;
			}
			this.OnTick();
		}

		// Token: 0x0600000B RID: 11
		protected abstract void OnTick();

		// Token: 0x0600000C RID: 12 RVA: 0x0000240B File Offset: 0x0000060B
		protected void SendMessage(Message message)
		{
			this._clientSession.SendMessage(message);
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x0600000D RID: 13 RVA: 0x00002419 File Offset: 0x00000619
		// (set) Token: 0x0600000E RID: 14 RVA: 0x00002421 File Offset: 0x00000621
		public ILoginAccessProvider AccessProvider { get; protected set; }

		// Token: 0x0600000F RID: 15 RVA: 0x0000242C File Offset: 0x0000062C
		protected async Task<LoginResult> Login(LoginMessage message)
		{
			Debug.Print("Logging in", 0, Debug.DebugColor.White, 17592186044416UL);
			return await this._clientSession.Login(message);
		}

		// Token: 0x06000010 RID: 16 RVA: 0x0000247C File Offset: 0x0000067C
		protected async Task<TResult> CallFunction<TResult>(Message message) where TResult : FunctionResult
		{
			return await this._clientSession.CallFunction<TResult>(message);
		}

		// Token: 0x06000011 RID: 17 RVA: 0x000024C9 File Offset: 0x000006C9
		protected void AddMessageHandler<TMessage>(ClientMessageHandler<TMessage> messageHandler) where TMessage : Message
		{
			this._messageHandlers.Add(typeof(TMessage), messageHandler);
		}

		// Token: 0x06000012 RID: 18 RVA: 0x000024E1 File Offset: 0x000006E1
		public void HandleMessage(Message message)
		{
			this._messageHandlers[message.GetType()].DynamicInvokeWithLog(new object[] { message });
		}

		// Token: 0x06000013 RID: 19 RVA: 0x00002504 File Offset: 0x00000704
		public virtual void OnConnected()
		{
			this._connectionState = Client<T>.ConnectionState.Connected;
		}

		// Token: 0x06000014 RID: 20 RVA: 0x0000250D File Offset: 0x0000070D
		public virtual void OnCantConnect()
		{
			if (this._autoReconnect)
			{
				this.Reset();
				return;
			}
			this._connectionState = Client<T>.ConnectionState.Idle;
		}

		// Token: 0x06000015 RID: 21 RVA: 0x00002525 File Offset: 0x00000725
		public virtual void OnDisconnected()
		{
			if (this._autoReconnect)
			{
				this.Reset();
				return;
			}
			this._connectionState = Client<T>.ConnectionState.Idle;
		}

		// Token: 0x06000016 RID: 22 RVA: 0x0000253D File Offset: 0x0000073D
		protected void BeginConnect()
		{
			this._connectionState = Client<T>.ConnectionState.ReadyToConnect;
		}

		// Token: 0x06000017 RID: 23 RVA: 0x00002546 File Offset: 0x00000746
		protected void BeginDisconnect()
		{
			this._clientSession.Disconnect();
		}

		// Token: 0x06000018 RID: 24 RVA: 0x00002553 File Offset: 0x00000753
		protected void SetAliveCheckTime(long time)
		{
		}

		// Token: 0x06000019 RID: 25 RVA: 0x00002558 File Offset: 0x00000758
		private void Reset()
		{
			this._connectionState = Client<T>.ConnectionState.SleepingToConnectAgain;
			this._timer = new Stopwatch();
			this._timer.Start();
			Debug.Print("Waiting " + 5000L + " milliseconds for another connection attempt", 0, Debug.DebugColor.White, 17592186044416UL);
		}

		// Token: 0x0600001A RID: 26 RVA: 0x000025AD File Offset: 0x000007AD
		public Task<bool> CheckConnection()
		{
			return this._clientSession.CheckConnection();
		}

		// Token: 0x04000001 RID: 1
		private IClientSession _clientSession;

		// Token: 0x04000002 RID: 2
		private Dictionary<Type, Delegate> _messageHandlers;

		// Token: 0x04000003 RID: 3
		private Client<T>.ConnectionState _connectionState;

		// Token: 0x04000004 RID: 4
		private Stopwatch _timer;

		// Token: 0x04000005 RID: 5
		private const long ReconnectTime = 5000L;

		// Token: 0x04000006 RID: 6
		private bool _autoReconnect;

		// Token: 0x0200005D RID: 93
		private enum ConnectionState
		{
			// Token: 0x040000D5 RID: 213
			Idle,
			// Token: 0x040000D6 RID: 214
			ReadyToConnect,
			// Token: 0x040000D7 RID: 215
			Connecting,
			// Token: 0x040000D8 RID: 216
			Connected,
			// Token: 0x040000D9 RID: 217
			SleepingToConnectAgain
		}
	}
}
