using System;
using System.Threading.Tasks;

namespace TaleWorlds.Network
{
	public abstract class MessageServiceConnection
	{
		public MessageServiceConnection()
		{
		}

		public abstract Task SendAsync(string text);

		public abstract void Init(string address, string token);

		public string Address { get; protected set; }

		public event MessageServiceConnection.ClosedDelegate Closed;

		public event MessageServiceConnection.StateChangedDelegate StateChanged;

		public abstract void RegisterProxyClient(string name, IMessageProxyClient playerClient);

		public abstract Task StartAsync();

		public abstract Task StopAsync();

		protected void InvokeClosed()
		{
			MessageServiceConnection.ClosedDelegate closed = this.Closed;
			if (closed == null)
			{
				return;
			}
			closed();
		}

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

		public ConnectionState State;

		public ConnectionState OldState;

		public delegate Task ClosedDelegate();

		public delegate void StateChangedDelegate(ConnectionState oldState, ConnectionState newState);
	}
}
