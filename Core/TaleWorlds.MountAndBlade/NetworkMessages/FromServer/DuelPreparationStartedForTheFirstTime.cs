using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x0200003F RID: 63
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class DuelPreparationStartedForTheFirstTime : GameNetworkMessage
	{
		// Token: 0x1700005B RID: 91
		// (get) Token: 0x0600020D RID: 525 RVA: 0x000047D3 File Offset: 0x000029D3
		// (set) Token: 0x0600020E RID: 526 RVA: 0x000047DB File Offset: 0x000029DB
		public NetworkCommunicator RequesterPeer { get; private set; }

		// Token: 0x1700005C RID: 92
		// (get) Token: 0x0600020F RID: 527 RVA: 0x000047E4 File Offset: 0x000029E4
		// (set) Token: 0x06000210 RID: 528 RVA: 0x000047EC File Offset: 0x000029EC
		public NetworkCommunicator RequesteePeer { get; private set; }

		// Token: 0x1700005D RID: 93
		// (get) Token: 0x06000211 RID: 529 RVA: 0x000047F5 File Offset: 0x000029F5
		// (set) Token: 0x06000212 RID: 530 RVA: 0x000047FD File Offset: 0x000029FD
		public int AreaIndex { get; private set; }

		// Token: 0x06000213 RID: 531 RVA: 0x00004806 File Offset: 0x00002A06
		public DuelPreparationStartedForTheFirstTime(NetworkCommunicator requesterPeer, NetworkCommunicator requesteePeer, int areaIndex)
		{
			this.RequesterPeer = requesterPeer;
			this.RequesteePeer = requesteePeer;
			this.AreaIndex = areaIndex;
		}

		// Token: 0x06000214 RID: 532 RVA: 0x00004823 File Offset: 0x00002A23
		public DuelPreparationStartedForTheFirstTime()
		{
		}

		// Token: 0x06000215 RID: 533 RVA: 0x0000482C File Offset: 0x00002A2C
		protected override bool OnRead()
		{
			bool flag = true;
			this.RequesterPeer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref flag, false);
			this.RequesteePeer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref flag, false);
			this.AreaIndex = GameNetworkMessage.ReadIntFromPacket(CompressionMission.DuelAreaIndexCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x06000216 RID: 534 RVA: 0x0000486A File Offset: 0x00002A6A
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.RequesterPeer);
			GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.RequesteePeer);
			GameNetworkMessage.WriteIntToPacket(this.AreaIndex, CompressionMission.DuelAreaIndexCompressionInfo);
		}

		// Token: 0x06000217 RID: 535 RVA: 0x00004892 File Offset: 0x00002A92
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.GameMode;
		}

		// Token: 0x06000218 RID: 536 RVA: 0x0000489C File Offset: 0x00002A9C
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Duel started between agent with name: ",
				this.RequesteePeer.UserName,
				" and index: ",
				this.RequesteePeer.Index,
				" and agent with name: ",
				this.RequesterPeer.UserName,
				" and index: ",
				this.RequesterPeer.Index
			});
		}
	}
}
