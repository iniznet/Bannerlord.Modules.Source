using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.MissionRepresentatives;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x0200003E RID: 62
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class DuelPointsUpdateMessage : GameNetworkMessage
	{
		// Token: 0x17000057 RID: 87
		// (get) Token: 0x060001FF RID: 511 RVA: 0x000046AE File Offset: 0x000028AE
		// (set) Token: 0x06000200 RID: 512 RVA: 0x000046B6 File Offset: 0x000028B6
		public int Bounty { get; private set; }

		// Token: 0x17000058 RID: 88
		// (get) Token: 0x06000201 RID: 513 RVA: 0x000046BF File Offset: 0x000028BF
		// (set) Token: 0x06000202 RID: 514 RVA: 0x000046C7 File Offset: 0x000028C7
		public int Score { get; private set; }

		// Token: 0x17000059 RID: 89
		// (get) Token: 0x06000203 RID: 515 RVA: 0x000046D0 File Offset: 0x000028D0
		// (set) Token: 0x06000204 RID: 516 RVA: 0x000046D8 File Offset: 0x000028D8
		public int NumberOfWins { get; private set; }

		// Token: 0x1700005A RID: 90
		// (get) Token: 0x06000205 RID: 517 RVA: 0x000046E1 File Offset: 0x000028E1
		// (set) Token: 0x06000206 RID: 518 RVA: 0x000046E9 File Offset: 0x000028E9
		public NetworkCommunicator NetworkCommunicator { get; private set; }

		// Token: 0x06000207 RID: 519 RVA: 0x000046F2 File Offset: 0x000028F2
		public DuelPointsUpdateMessage()
		{
		}

		// Token: 0x06000208 RID: 520 RVA: 0x000046FA File Offset: 0x000028FA
		public DuelPointsUpdateMessage(DuelMissionRepresentative representative)
		{
			this.Bounty = representative.Bounty;
			this.Score = representative.Score;
			this.NumberOfWins = representative.NumberOfWins;
			this.NetworkCommunicator = representative.GetNetworkPeer();
		}

		// Token: 0x06000209 RID: 521 RVA: 0x00004732 File Offset: 0x00002932
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteIntToPacket(this.Bounty, CompressionMatchmaker.ScoreCompressionInfo);
			GameNetworkMessage.WriteIntToPacket(this.Score, CompressionMatchmaker.ScoreCompressionInfo);
			GameNetworkMessage.WriteIntToPacket(this.NumberOfWins, CompressionMatchmaker.KillDeathAssistCountCompressionInfo);
			GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.NetworkCommunicator);
		}

		// Token: 0x0600020A RID: 522 RVA: 0x00004770 File Offset: 0x00002970
		protected override bool OnRead()
		{
			bool flag = true;
			this.Bounty = GameNetworkMessage.ReadIntFromPacket(CompressionMatchmaker.ScoreCompressionInfo, ref flag);
			this.Score = GameNetworkMessage.ReadIntFromPacket(CompressionMatchmaker.ScoreCompressionInfo, ref flag);
			this.NumberOfWins = GameNetworkMessage.ReadIntFromPacket(CompressionMatchmaker.KillDeathAssistCountCompressionInfo, ref flag);
			this.NetworkCommunicator = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref flag, false);
			return flag;
		}

		// Token: 0x0600020B RID: 523 RVA: 0x000047C4 File Offset: 0x000029C4
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.GameMode;
		}

		// Token: 0x0600020C RID: 524 RVA: 0x000047CC File Offset: 0x000029CC
		protected override string OnGetLogFormat()
		{
			return "PointUpdateMessage";
		}
	}
}
