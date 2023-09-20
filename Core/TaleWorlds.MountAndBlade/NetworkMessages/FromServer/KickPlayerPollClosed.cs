using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x0200004B RID: 75
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class KickPlayerPollClosed : GameNetworkMessage
	{
		// Token: 0x1700007C RID: 124
		// (get) Token: 0x06000297 RID: 663 RVA: 0x000055FE File Offset: 0x000037FE
		// (set) Token: 0x06000298 RID: 664 RVA: 0x00005606 File Offset: 0x00003806
		public NetworkCommunicator PlayerPeer { get; private set; }

		// Token: 0x1700007D RID: 125
		// (get) Token: 0x06000299 RID: 665 RVA: 0x0000560F File Offset: 0x0000380F
		// (set) Token: 0x0600029A RID: 666 RVA: 0x00005617 File Offset: 0x00003817
		public bool Accepted { get; private set; }

		// Token: 0x0600029B RID: 667 RVA: 0x00005620 File Offset: 0x00003820
		public KickPlayerPollClosed(NetworkCommunicator playerPeer, bool accepted)
		{
			this.PlayerPeer = playerPeer;
			this.Accepted = accepted;
		}

		// Token: 0x0600029C RID: 668 RVA: 0x00005636 File Offset: 0x00003836
		public KickPlayerPollClosed()
		{
		}

		// Token: 0x0600029D RID: 669 RVA: 0x00005640 File Offset: 0x00003840
		protected override bool OnRead()
		{
			bool flag = true;
			this.PlayerPeer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref flag, false);
			this.Accepted = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			return flag;
		}

		// Token: 0x0600029E RID: 670 RVA: 0x0000566B File Offset: 0x0000386B
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.PlayerPeer);
			GameNetworkMessage.WriteBoolToPacket(this.Accepted);
		}

		// Token: 0x0600029F RID: 671 RVA: 0x00005683 File Offset: 0x00003883
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Administration;
		}

		// Token: 0x060002A0 RID: 672 RVA: 0x0000568C File Offset: 0x0000388C
		protected override string OnGetLogFormat()
		{
			string[] array = new string[5];
			array[0] = "Poll is closed. ";
			int num = 1;
			NetworkCommunicator playerPeer = this.PlayerPeer;
			array[num] = ((playerPeer != null) ? playerPeer.UserName : null);
			array[2] = " is ";
			array[3] = (this.Accepted ? "" : "not ");
			array[4] = "kicked.";
			return string.Concat(array);
		}
	}
}
