using System;
using System.Threading.Tasks;
using TaleWorlds.Network;

namespace TaleWorlds.Diamond.Socket
{
	public abstract class ClientSocketSession : ClientsideSession, IClientSession
	{
		protected ClientSocketSession(IClient client, string address, int port)
		{
			this._client = client;
			this._address = address;
			this._port = port;
			base.AddMessageHandler<SocketMessage>(new MessageContractHandlerDelegate<SocketMessage>(this.HandleSocketMessage));
		}

		private void HandleSocketMessage(SocketMessage socketMessage)
		{
			Message message = socketMessage.Message;
			this._client.HandleMessage(message);
		}

		protected override void OnConnected()
		{
			base.OnConnected();
			this._client.OnConnected();
		}

		protected override void OnCantConnect()
		{
			base.OnCantConnect();
			this._client.OnCantConnect();
		}

		protected override void OnDisconnected()
		{
			base.OnDisconnected();
			this._client.OnDisconnected();
		}

		void IClientSession.Connect()
		{
			this.Connect(this._address, this._port, true);
		}

		void IClientSession.Disconnect()
		{
			base.SendDisconnectMessage();
		}

		Task<TReturn> IClientSession.CallFunction<TReturn>(Message message)
		{
			throw new NotImplementedException();
		}

		void IClientSession.SendMessage(Message message)
		{
			base.SendMessage(new SocketMessage(message));
		}

		Task<LoginResult> IClientSession.Login(LoginMessage message)
		{
			throw new NotImplementedException();
		}

		Task<bool> IClientSession.CheckConnection()
		{
			return Task.FromResult<bool>(true);
		}

		private string _address;

		private int _port;

		private IClient _client;
	}
}
