using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x0200004C RID: 76
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class KickPlayerPollOpened : GameNetworkMessage
	{
		// Token: 0x1700007E RID: 126
		// (get) Token: 0x060002A1 RID: 673 RVA: 0x000056E8 File Offset: 0x000038E8
		// (set) Token: 0x060002A2 RID: 674 RVA: 0x000056F0 File Offset: 0x000038F0
		public NetworkCommunicator InitiatorPeer { get; private set; }

		// Token: 0x1700007F RID: 127
		// (get) Token: 0x060002A3 RID: 675 RVA: 0x000056F9 File Offset: 0x000038F9
		// (set) Token: 0x060002A4 RID: 676 RVA: 0x00005701 File Offset: 0x00003901
		public NetworkCommunicator PlayerPeer { get; private set; }

		// Token: 0x17000080 RID: 128
		// (get) Token: 0x060002A5 RID: 677 RVA: 0x0000570A File Offset: 0x0000390A
		// (set) Token: 0x060002A6 RID: 678 RVA: 0x00005712 File Offset: 0x00003912
		public bool BanPlayer { get; private set; }

		// Token: 0x060002A7 RID: 679 RVA: 0x0000571B File Offset: 0x0000391B
		public KickPlayerPollOpened(NetworkCommunicator initiatorPeer, NetworkCommunicator playerPeer, bool banPlayer)
		{
			this.InitiatorPeer = initiatorPeer;
			this.PlayerPeer = playerPeer;
			this.BanPlayer = banPlayer;
		}

		// Token: 0x060002A8 RID: 680 RVA: 0x00005738 File Offset: 0x00003938
		public KickPlayerPollOpened()
		{
		}

		// Token: 0x060002A9 RID: 681 RVA: 0x00005740 File Offset: 0x00003940
		protected override bool OnRead()
		{
			bool flag = true;
			this.InitiatorPeer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref flag, false);
			this.PlayerPeer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref flag, false);
			this.BanPlayer = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			return flag;
		}

		// Token: 0x060002AA RID: 682 RVA: 0x00005779 File Offset: 0x00003979
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.InitiatorPeer);
			GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.PlayerPeer);
			GameNetworkMessage.WriteBoolToPacket(this.BanPlayer);
		}

		// Token: 0x060002AB RID: 683 RVA: 0x0000579C File Offset: 0x0000399C
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Administration;
		}

		// Token: 0x060002AC RID: 684 RVA: 0x000057A4 File Offset: 0x000039A4
		protected override string OnGetLogFormat()
		{
			string[] array = new string[5];
			int num = 0;
			NetworkCommunicator initiatorPeer = this.InitiatorPeer;
			array[num] = ((initiatorPeer != null) ? initiatorPeer.UserName : null);
			array[1] = " wants to start poll to kick";
			array[2] = (this.BanPlayer ? " and ban" : "");
			array[3] = " player: ";
			int num2 = 4;
			NetworkCommunicator playerPeer = this.PlayerPeer;
			array[num2] = ((playerPeer != null) ? playerPeer.UserName : null);
			return string.Concat(array);
		}
	}
}
