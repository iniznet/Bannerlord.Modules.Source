using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class PlayerMessageTeam : GameNetworkMessage
	{
		public string Message { get; private set; }

		public NetworkCommunicator Player { get; private set; }

		public PlayerMessageTeam(NetworkCommunicator player, string message)
		{
			this.Player = player;
			this.Message = message;
		}

		public PlayerMessageTeam()
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
			return string.Concat(new object[]
			{
				"Receiving team message: ",
				this.Message,
				" from peer: ",
				this.Player.UserName,
				" index: ",
				this.Player.Index
			});
		}
	}
}
