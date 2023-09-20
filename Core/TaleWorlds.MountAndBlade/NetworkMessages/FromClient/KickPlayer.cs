using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
	// Token: 0x02000011 RID: 17
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
	public sealed class KickPlayer : GameNetworkMessage
	{
		// Token: 0x17000016 RID: 22
		// (get) Token: 0x06000078 RID: 120 RVA: 0x000029A7 File Offset: 0x00000BA7
		// (set) Token: 0x06000079 RID: 121 RVA: 0x000029AF File Offset: 0x00000BAF
		public NetworkCommunicator PlayerPeer { get; private set; }

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x0600007A RID: 122 RVA: 0x000029B8 File Offset: 0x00000BB8
		// (set) Token: 0x0600007B RID: 123 RVA: 0x000029C0 File Offset: 0x00000BC0
		public bool BanPlayer { get; private set; }

		// Token: 0x0600007C RID: 124 RVA: 0x000029C9 File Offset: 0x00000BC9
		public KickPlayer(NetworkCommunicator playerPeer, bool banPlayer)
		{
			this.PlayerPeer = playerPeer;
			this.BanPlayer = banPlayer;
		}

		// Token: 0x0600007D RID: 125 RVA: 0x000029DF File Offset: 0x00000BDF
		public KickPlayer()
		{
		}

		// Token: 0x0600007E RID: 126 RVA: 0x000029E8 File Offset: 0x00000BE8
		protected override bool OnRead()
		{
			bool flag = true;
			this.PlayerPeer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref flag, false);
			this.BanPlayer = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			return flag;
		}

		// Token: 0x0600007F RID: 127 RVA: 0x00002A13 File Offset: 0x00000C13
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.PlayerPeer);
			GameNetworkMessage.WriteBoolToPacket(this.BanPlayer);
		}

		// Token: 0x06000080 RID: 128 RVA: 0x00002A2B File Offset: 0x00000C2B
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Administration;
		}

		// Token: 0x06000081 RID: 129 RVA: 0x00002A33 File Offset: 0x00000C33
		protected override string OnGetLogFormat()
		{
			return "Requested to kick" + (this.BanPlayer ? " and ban" : "") + " player: " + this.PlayerPeer.UserName;
		}
	}
}
