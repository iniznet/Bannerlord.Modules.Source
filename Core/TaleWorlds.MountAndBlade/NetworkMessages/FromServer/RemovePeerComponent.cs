using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x020000C7 RID: 199
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class RemovePeerComponent : GameNetworkMessage
	{
		// Token: 0x170001DA RID: 474
		// (get) Token: 0x06000830 RID: 2096 RVA: 0x0000EAAC File Offset: 0x0000CCAC
		// (set) Token: 0x06000831 RID: 2097 RVA: 0x0000EAB4 File Offset: 0x0000CCB4
		public NetworkCommunicator Peer { get; private set; }

		// Token: 0x170001DB RID: 475
		// (get) Token: 0x06000832 RID: 2098 RVA: 0x0000EABD File Offset: 0x0000CCBD
		// (set) Token: 0x06000833 RID: 2099 RVA: 0x0000EAC5 File Offset: 0x0000CCC5
		public uint ComponentId { get; private set; }

		// Token: 0x06000834 RID: 2100 RVA: 0x0000EACE File Offset: 0x0000CCCE
		public RemovePeerComponent(NetworkCommunicator peer, uint componentId)
		{
			this.Peer = peer;
			this.ComponentId = componentId;
		}

		// Token: 0x06000835 RID: 2101 RVA: 0x0000EAE4 File Offset: 0x0000CCE4
		public RemovePeerComponent()
		{
		}

		// Token: 0x06000836 RID: 2102 RVA: 0x0000EAEC File Offset: 0x0000CCEC
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.Peer);
			GameNetworkMessage.WriteUintToPacket(this.ComponentId, CompressionBasic.PeerComponentCompressionInfo);
		}

		// Token: 0x06000837 RID: 2103 RVA: 0x0000EB0C File Offset: 0x0000CD0C
		protected override bool OnRead()
		{
			bool flag = true;
			this.Peer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref flag, false);
			this.ComponentId = GameNetworkMessage.ReadUintFromPacket(CompressionBasic.PeerComponentCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x06000838 RID: 2104 RVA: 0x0000EB3C File Offset: 0x0000CD3C
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Peers;
		}

		// Token: 0x06000839 RID: 2105 RVA: 0x0000EB40 File Offset: 0x0000CD40
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
