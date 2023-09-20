using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x0200004D RID: 77
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class KillDeathCountChange : GameNetworkMessage
	{
		// Token: 0x17000081 RID: 129
		// (get) Token: 0x060002AD RID: 685 RVA: 0x0000580D File Offset: 0x00003A0D
		// (set) Token: 0x060002AE RID: 686 RVA: 0x00005815 File Offset: 0x00003A15
		public NetworkCommunicator VictimPeer { get; private set; }

		// Token: 0x17000082 RID: 130
		// (get) Token: 0x060002AF RID: 687 RVA: 0x0000581E File Offset: 0x00003A1E
		// (set) Token: 0x060002B0 RID: 688 RVA: 0x00005826 File Offset: 0x00003A26
		public NetworkCommunicator AttackerPeer { get; private set; }

		// Token: 0x17000083 RID: 131
		// (get) Token: 0x060002B1 RID: 689 RVA: 0x0000582F File Offset: 0x00003A2F
		// (set) Token: 0x060002B2 RID: 690 RVA: 0x00005837 File Offset: 0x00003A37
		public int KillCount { get; private set; }

		// Token: 0x17000084 RID: 132
		// (get) Token: 0x060002B3 RID: 691 RVA: 0x00005840 File Offset: 0x00003A40
		// (set) Token: 0x060002B4 RID: 692 RVA: 0x00005848 File Offset: 0x00003A48
		public int AssistCount { get; private set; }

		// Token: 0x17000085 RID: 133
		// (get) Token: 0x060002B5 RID: 693 RVA: 0x00005851 File Offset: 0x00003A51
		// (set) Token: 0x060002B6 RID: 694 RVA: 0x00005859 File Offset: 0x00003A59
		public int DeathCount { get; private set; }

		// Token: 0x17000086 RID: 134
		// (get) Token: 0x060002B7 RID: 695 RVA: 0x00005862 File Offset: 0x00003A62
		// (set) Token: 0x060002B8 RID: 696 RVA: 0x0000586A File Offset: 0x00003A6A
		public int Score { get; private set; }

		// Token: 0x060002B9 RID: 697 RVA: 0x00005873 File Offset: 0x00003A73
		public KillDeathCountChange(NetworkCommunicator peer, NetworkCommunicator attackerPeer, int killCount, int assistCount, int deathCount, int score)
		{
			this.VictimPeer = peer;
			this.AttackerPeer = attackerPeer;
			this.KillCount = killCount;
			this.AssistCount = assistCount;
			this.DeathCount = deathCount;
			this.Score = score;
		}

		// Token: 0x060002BA RID: 698 RVA: 0x000058A8 File Offset: 0x00003AA8
		public KillDeathCountChange()
		{
		}

		// Token: 0x060002BB RID: 699 RVA: 0x000058B0 File Offset: 0x00003AB0
		protected override bool OnRead()
		{
			bool flag = true;
			this.VictimPeer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref flag, false);
			this.AttackerPeer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref flag, true);
			this.KillCount = GameNetworkMessage.ReadIntFromPacket(CompressionMatchmaker.KillDeathAssistCountCompressionInfo, ref flag);
			this.AssistCount = GameNetworkMessage.ReadIntFromPacket(CompressionMatchmaker.KillDeathAssistCountCompressionInfo, ref flag);
			this.DeathCount = GameNetworkMessage.ReadIntFromPacket(CompressionMatchmaker.KillDeathAssistCountCompressionInfo, ref flag);
			this.Score = GameNetworkMessage.ReadIntFromPacket(CompressionMatchmaker.ScoreCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x060002BC RID: 700 RVA: 0x00005924 File Offset: 0x00003B24
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.VictimPeer);
			GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.AttackerPeer);
			GameNetworkMessage.WriteIntToPacket(this.KillCount, CompressionMatchmaker.KillDeathAssistCountCompressionInfo);
			GameNetworkMessage.WriteIntToPacket(this.AssistCount, CompressionMatchmaker.KillDeathAssistCountCompressionInfo);
			GameNetworkMessage.WriteIntToPacket(this.DeathCount, CompressionMatchmaker.KillDeathAssistCountCompressionInfo);
			GameNetworkMessage.WriteIntToPacket(this.Score, CompressionMatchmaker.ScoreCompressionInfo);
		}

		// Token: 0x060002BD RID: 701 RVA: 0x00005987 File Offset: 0x00003B87
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Administration;
		}

		// Token: 0x060002BE RID: 702 RVA: 0x00005990 File Offset: 0x00003B90
		protected override string OnGetLogFormat()
		{
			object[] array = new object[11];
			array[0] = "Kill-Death Count Changed. Peer: ";
			int num = 1;
			NetworkCommunicator victimPeer = this.VictimPeer;
			array[num] = ((victimPeer != null) ? victimPeer.UserName : null) ?? "NULL";
			array[2] = " killed peer: ";
			int num2 = 3;
			NetworkCommunicator attackerPeer = this.AttackerPeer;
			array[num2] = ((attackerPeer != null) ? attackerPeer.UserName : null) ?? "NULL";
			array[4] = " and now has ";
			array[5] = this.KillCount;
			array[6] = " kills, ";
			array[7] = this.AssistCount;
			array[8] = " assists, and ";
			array[9] = this.DeathCount;
			array[10] = " deaths.";
			return string.Concat(array);
		}
	}
}
