using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x02000035 RID: 53
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class AddPeerComponent : GameNetworkMessage
	{
		// Token: 0x17000047 RID: 71
		// (get) Token: 0x060001AA RID: 426 RVA: 0x00003FAE File Offset: 0x000021AE
		// (set) Token: 0x060001AB RID: 427 RVA: 0x00003FB6 File Offset: 0x000021B6
		public NetworkCommunicator Peer { get; private set; }

		// Token: 0x17000048 RID: 72
		// (get) Token: 0x060001AC RID: 428 RVA: 0x00003FBF File Offset: 0x000021BF
		// (set) Token: 0x060001AD RID: 429 RVA: 0x00003FC7 File Offset: 0x000021C7
		public uint ComponentId { get; private set; }

		// Token: 0x060001AE RID: 430 RVA: 0x00003FD0 File Offset: 0x000021D0
		public AddPeerComponent(NetworkCommunicator peer, uint componentId)
		{
			this.Peer = peer;
			this.ComponentId = componentId;
		}

		// Token: 0x060001AF RID: 431 RVA: 0x00003FE6 File Offset: 0x000021E6
		public AddPeerComponent()
		{
		}

		// Token: 0x060001B0 RID: 432 RVA: 0x00003FEE File Offset: 0x000021EE
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.Peer);
			GameNetworkMessage.WriteUintToPacket(this.ComponentId, CompressionBasic.PeerComponentCompressionInfo);
		}

		// Token: 0x060001B1 RID: 433 RVA: 0x0000400C File Offset: 0x0000220C
		protected override bool OnRead()
		{
			bool flag = true;
			this.Peer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref flag, false);
			this.ComponentId = GameNetworkMessage.ReadUintFromPacket(CompressionBasic.PeerComponentCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x060001B2 RID: 434 RVA: 0x0000403C File Offset: 0x0000223C
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Peers;
		}

		// Token: 0x060001B3 RID: 435 RVA: 0x00004040 File Offset: 0x00002240
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Add component with ID: ",
				this.ComponentId,
				" to peer:",
				this.Peer.UserName,
				" with peer-index:",
				this.Peer.Index
			});
		}
	}
}
