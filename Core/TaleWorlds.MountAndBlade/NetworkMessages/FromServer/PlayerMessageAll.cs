using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class PlayerMessageAll : GameNetworkMessage
	{
		public NetworkCommunicator Player { get; private set; }

		public string Message { get; private set; }

		public PlayerMessageAll(NetworkCommunicator player, string message)
		{
			this.Player = player;
			this.Message = message;
		}

		public PlayerMessageAll()
		{
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.Player);
			GameNetworkMessage.WriteStringToPacket(this.Message);
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.Player = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref flag, false);
			this.Message = GameNetworkMessage.ReadStringFromPacket(ref flag);
			return flag;
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Messaging;
		}

		protected override string OnGetLogFormat()
		{
			return "Receiving Player message to all: " + this.Message;
		}
	}
}
