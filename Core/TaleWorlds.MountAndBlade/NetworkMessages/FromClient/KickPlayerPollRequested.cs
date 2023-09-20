using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	// Token: 0x02000012 RID: 18
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class KickPlayerPollRequested : GameNetworkMessage
	{
		// Token: 0x17000018 RID: 24
		// (get) Token: 0x06000082 RID: 130 RVA: 0x00002A63 File Offset: 0x00000C63
		// (set) Token: 0x06000083 RID: 131 RVA: 0x00002A6B File Offset: 0x00000C6B
		public NetworkCommunicator PlayerPeer { get; private set; }

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x06000084 RID: 132 RVA: 0x00002A74 File Offset: 0x00000C74
		// (set) Token: 0x06000085 RID: 133 RVA: 0x00002A7C File Offset: 0x00000C7C
		public bool BanPlayer { get; private set; }

		// Token: 0x06000086 RID: 134 RVA: 0x00002A85 File Offset: 0x00000C85
		public KickPlayerPollRequested(NetworkCommunicator playerPeer, bool banPlayer)
		{
			this.PlayerPeer = playerPeer;
			this.BanPlayer = banPlayer;
		}

		// Token: 0x06000087 RID: 135 RVA: 0x00002A9B File Offset: 0x00000C9B
		public KickPlayerPollRequested()
		{
		}

		// Token: 0x06000088 RID: 136 RVA: 0x00002AA4 File Offset: 0x00000CA4
		protected override bool OnRead()
		{
			bool flag = true;
			this.PlayerPeer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref flag, true);
			this.BanPlayer = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			return flag;
		}

		// Token: 0x06000089 RID: 137 RVA: 0x00002ACF File Offset: 0x00000CCF
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.PlayerPeer);
			GameNetworkMessage.WriteBoolToPacket(this.BanPlayer);
		}

		// Token: 0x0600008A RID: 138 RVA: 0x00002AE7 File Offset: 0x00000CE7
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Administration;
		}

		// Token: 0x0600008B RID: 139 RVA: 0x00002AEF File Offset: 0x00000CEF
		protected override string OnGetLogFormat()
		{
			string text = "Requested to start poll to kick";
			string text2 = (this.BanPlayer ? " and ban" : "");
			string text3 = " player: ";
			NetworkCommunicator playerPeer = this.PlayerPeer;
			return text + text2 + text3 + ((playerPeer != null) ? playerPeer.UserName : null);
		}
	}
}
