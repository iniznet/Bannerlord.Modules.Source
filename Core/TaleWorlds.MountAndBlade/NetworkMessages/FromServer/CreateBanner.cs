using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x02000074 RID: 116
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class CreateBanner : GameNetworkMessage
	{
		// Token: 0x170000F3 RID: 243
		// (get) Token: 0x0600047A RID: 1146 RVA: 0x00008B04 File Offset: 0x00006D04
		// (set) Token: 0x0600047B RID: 1147 RVA: 0x00008B0C File Offset: 0x00006D0C
		public NetworkCommunicator Peer { get; private set; }

		// Token: 0x170000F4 RID: 244
		// (get) Token: 0x0600047C RID: 1148 RVA: 0x00008B15 File Offset: 0x00006D15
		// (set) Token: 0x0600047D RID: 1149 RVA: 0x00008B1D File Offset: 0x00006D1D
		public string BannerCode { get; private set; }

		// Token: 0x0600047E RID: 1150 RVA: 0x00008B26 File Offset: 0x00006D26
		public CreateBanner(NetworkCommunicator peer, string bannerCode)
		{
			this.Peer = peer;
			this.BannerCode = bannerCode;
		}

		// Token: 0x0600047F RID: 1151 RVA: 0x00008B3C File Offset: 0x00006D3C
		public CreateBanner()
		{
		}

		// Token: 0x06000480 RID: 1152 RVA: 0x00008B44 File Offset: 0x00006D44
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.Peer);
			GameNetworkMessage.WriteStringToPacket(this.BannerCode);
		}

		// Token: 0x06000481 RID: 1153 RVA: 0x00008B5C File Offset: 0x00006D5C
		protected override bool OnRead()
		{
			bool flag = true;
			this.Peer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref flag, false);
			this.BannerCode = GameNetworkMessage.ReadStringFromPacket(ref flag);
			return flag;
		}

		// Token: 0x06000482 RID: 1154 RVA: 0x00008B87 File Offset: 0x00006D87
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Peers | MultiplayerMessageFilter.AgentsDetailed;
		}

		// Token: 0x06000483 RID: 1155 RVA: 0x00008B8F File Offset: 0x00006D8F
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				"Create banner for peer: ",
				this.Peer.UserName,
				", with index: ",
				this.Peer.Index
			});
		}
	}
}
