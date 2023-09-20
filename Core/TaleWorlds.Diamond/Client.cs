using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using TaleWorlds.Diamond.ClientApplication;
using TaleWorlds.Library;

namespace TaleWorlds.Diamond
{
	public abstract class Client<T> : DiamondClientApplicationObject, IClient where T : Client<T>
	{
		public bool IsInCriticalState { get; set; }

		public virtual long AliveCheckTimeInMiliSeconds
		{
			get
			{
				return 2000L;
			}
		}

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

		protected abstract void OnTick();

		protected void SendMessage(Message message)
		{
			this._clientSession.SendMessage(message);
		}

		public ILoginAccessProvider AccessProvider { get; protected set; }

		protected async Task<LoginResult> Login(LoginMessage message)
		{
			Debug.Print("Logging in", 0, Debug.DebugColor.White, 17592186044416UL);
			return await this._clientSession.Login(message);
		}

		protected async Task<TResult> CallFunction<TResult>(Message message) where TResult : FunctionResult
		{
			return await this._clientSession.CallFunction<TResult>(message);
		}

		protected void AddMessageHandler<TMessage>(ClientMessageHandler<TMessage> messageHandler) where TMessage : Message
		{
			this._messageHandlers.Add(typeof(TMessage), messageHandler);
		}

		public void HandleMessage(Message message)
		{
			this._messageHandlers[message.GetType()].DynamicInvokeWithLog(new object[] { message });
		}

		public virtual void OnConnected()
		{
			this._connectionState = Client<T>.ConnectionState.Connected;
		}

		public virtual void OnCantConnect()
		{
			if (this._autoReconnect)
			{
				this.Reset();
				return;
			}
			this._connectionState = Client<T>.ConnectionState.Idle;
		}

		public virtual void OnDisconnected()
		{
			if (this._autoReconnect)
			{
				this.Reset();
				return;
			}
			this._connectionState = Client<T>.ConnectionState.Idle;
		}

		protected void BeginConnect()
		{
			this._connectionState = Client<T>.ConnectionState.ReadyToConnect;
		}

		protected void BeginDisconnect()
		{
			this._clientSession.Disconnect();
		}

		protected void SetAliveCheckTime(long time)
		{
		}

		private void Reset()
		{
			this._connectionState = Client<T>.ConnectionState.SleepingToConnectAgain;
			this._timer = new Stopwatch();
			this._timer.Start();
			Debug.Print("Waiting " + 5000L + " milliseconds for another connection attempt", 0, Debug.DebugColor.White, 17592186044416UL);
		}

		public Task<bool> CheckConnection()
		{
			return this._clientSession.CheckConnection();
		}

		private IClientSession _clientSession;

		private Dictionary<Type, Delegate> _messageHandlers;

		private Client<T>.ConnectionState _connectionState;

		private Stopwatch _timer;

		private const long ReconnectTime = 5000L;

		private bool _autoReconnect;

		private enum ConnectionState
		{
			Idle,
			ReadyToConnect,
			Connecting,
			Connected,
			SleepingToConnectAgain
		}
	}
}
