using System;

namespace TaleWorlds.MountAndBlade.Network.Messages
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class CreatePlayer : GameNetworkMessage
	{
		public int PlayerIndex { get; private set; }

		public string PlayerName { get; private set; }

		public int DisconnectedPeerIndex { get; private set; }

		public bool IsNonExistingDisconnectedPeer { get; private set; }

		public bool IsReceiverPeer { get; private set; }

		public CreatePlayer(int playerIndex, string playerName, int disconnectedPeerIndex, bool isNonExistingDisconnectedPeer = false, bool isReceiverPeer = false)
		{
			this.PlayerIndex = playerIndex;
			this.PlayerName = playerName;
			this.DisconnectedPeerIndex = disconnectedPeerIndex;
			this.IsNonExistingDisconnectedPeer = isNonExistingDisconnectedPeer;
			this.IsReceiverPeer = isReceiverPeer;
		}

		public CreatePlayer()
		{
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteIntToPacket(this.PlayerIndex, CompressionBasic.PlayerCompressionInfo);
			GameNetworkMessage.WriteStringToPacket(this.PlayerName);
			GameNetworkMessage.WriteIntToPacket(this.DisconnectedPeerIndex, CompressionBasic.PlayerCompressionInfo);
			GameNetworkMessage.WriteBoolToPacket(this.IsNonExistingDisconnectedPeer);
			GameNetworkMessage.WriteBoolToPacket(this.IsReceiverPeer);
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.PlayerIndex = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.PlayerCompressionInfo, ref flag);
			this.PlayerName = GameNetworkMessage.ReadStringFromPacket(ref flag);
			this.DisconnectedPeerIndex = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.PlayerCompressionInfo, ref flag);
			this.IsNonExistingDisconnectedPeer = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			this.IsReceiverPeer = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			return flag;
		}

		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Peers;
		}

		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Create a new player with name: ",
				this.PlayerName,
				" and index: ",
				this.PlayerIndex,
				" and dcedIndex: ",
				this.DisconnectedPeerIndex,
				" which is ",
				(!this.IsNonExistingDisconnectedPeer) ? "not" : "",
				" a NonExistingDisconnectedPeer"
			});
		}
	}
}
