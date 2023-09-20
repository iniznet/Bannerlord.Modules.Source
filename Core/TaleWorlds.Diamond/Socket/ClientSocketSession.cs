using System;
using System.Threading.Tasks;
using TaleWorlds.Network;

namespace TaleWorlds.Diamond.Socket
{
	// Token: 0x0200002D RID: 45
	public abstract class ClientSocketSession : ClientsideSession, IClientSession
	{
		// Token: 0x060000D9 RID: 217 RVA: 0x000035CD File Offset: 0x000017CD
		protected ClientSocketSession(IClient client, string address, int port)
		{
			this._client = client;
			this._address = address;
			this._port = port;
			base.AddMessageHandler<SocketMessage>(new MessageContractHandlerDelegate<SocketMessage>(this.HandleSocketMessage));
		}

		// Token: 0x060000DA RID: 218 RVA: 0x000035FC File Offset: 0x000017FC
		private void HandleSocketMessage(SocketMessage socketMessage)
		{
			Message message = socketMessage.Message;
			this._client.HandleMessage(message);
		}

		// Token: 0x060000DB RID: 219 RVA: 0x0000361C File Offset: 0x0000181C
		protected override void OnConnected()
		{
			base.OnConnected();
			this._client.OnConnected();
		}

		// Token: 0x060000DC RID: 220 RVA: 0x0000362F File Offset: 0x0000182F
		protected override void OnCantConnect()
		{
			base.OnCantConnect();
			this._client.OnCantConnect();
		}

		// Token: 0x060000DD RID: 221 RVA: 0x00003642 File Offset: 0x00001842
		protected override void OnDisconnected()
		{
			base.OnDisconnected();
			this._client.OnDisconnected();
		}

		// Token: 0x060000DE RID: 222 RVA: 0x00003655 File Offset: 0x00001855
		void IClientSession.Connect()
		{
			this.Connect(this._address, this._port, true);
		}

		// Token: 0x060000DF RID: 223 RVA: 0x0000366A File Offset: 0x0000186A
		void IClientSession.Disconnect()
		{
			base.SendDisconnectMessage();
		}

		// Token: 0x060000E0 RID: 224 RVA: 0x00003672 File Offset: 0x00001872
		Task<TReturn> IClientSession.CallFunction<TReturn>(Message message)
		{
			throw new NotImplementedException();
		}

		// Token: 0x060000E1 RID: 225 RVA: 0x00003679 File Offset: 0x00001879
		void IClientSession.SendMessage(Message message)
		{
			base.SendMessage(new SocketMessage(message));
		}

		// Token: 0x060000E2 RID: 226 RVA: 0x00003687 File Offset: 0x00001887
		Task<LoginResult> IClientSession.Login(LoginMessage message)
		{
			throw new NotImplementedException();
		}

		// Token: 0x060000E3 RID: 227 RVA: 0x0000368E File Offset: 0x0000188E
		Task<bool> IClientSession.CheckConnection()
		{
			return Task.FromResult<bool>(true);
		}

		// Token: 0x0400003D RID: 61
		private string _address;

		// Token: 0x0400003E RID: 62
		private int _port;

		// Token: 0x0400003F RID: 63
		private IClient _client;
	}
}
