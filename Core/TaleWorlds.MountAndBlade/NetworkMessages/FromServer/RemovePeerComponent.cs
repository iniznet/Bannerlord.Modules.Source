using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class RemovePeerComponent : GameNetworkMessage
	{
		public NetworkCommunicator Peer { get; private set; }

		public uint ComponentId { get; private set; }

		public RemovePeerComponent(NetworkCommunicator peer, uint componentId)
		{
			this.Peer = peer;
			this.ComponentId = componentId;
		}

		public RemovePeerComponent()
		{
		}

		protected override void OnWrite()
		{
			GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.Peer);
			GameNetworkMessage.WriteUintToPacket(this.ComponentId, CompressionBasic.PeerComponentCompressionInfo);
		}

		protected override bool OnRead()
		{
			bool flag = true;
			this.Peer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref flag, false);
			this.ComponentId = GameNetworkMessage.ReadUintFromPacket(CompressionBasic.PeerComponentCompressionInfo, ref flag);
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
				"Remove component with ID: ",
				this.ComponentId,
				" from peer: ",
				this.Peer.UserName,
				" with peer-index: ",
				this.Peer.Index
			});
		}
	}
}
