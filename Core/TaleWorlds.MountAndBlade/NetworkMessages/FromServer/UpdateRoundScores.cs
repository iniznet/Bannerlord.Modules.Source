using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x020000BD RID: 189
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class UpdateRoundScores : GameNetworkMessage
	{
		// Token: 0x170001C6 RID: 454
		// (get) Token: 0x060007D4 RID: 2004 RVA: 0x0000E25D File Offset: 0x0000C45D
		// (set) Token: 0x060007D5 RID: 2005 RVA: 0x0000E265 File Offset: 0x0000C465
		public int AttackerTeamScore { get; private set; }

		// Token: 0x170001C7 RID: 455
		// (get) Token: 0x060007D6 RID: 2006 RVA: 0x0000E26E File Offset: 0x0000C46E
		// (set) Token: 0x060007D7 RID: 2007 RVA: 0x0000E276 File Offset: 0x0000C476
		public int DefenderTeamScore { get; private set; }

		// Token: 0x060007D8 RID: 2008 RVA: 0x0000E27F File Offset: 0x0000C47F
		public UpdateRoundScores(int attackerTeamScore, int defenderTeamScore)
		{
			this.AttackerTeamScore = attackerTeamScore;
			this.DefenderTeamScore = defenderTeamScore;
		}

		// Token: 0x060007D9 RID: 2009 RVA: 0x0000E295 File Offset: 0x0000C495
		public UpdateRoundScores()
		{
		}

		// Token: 0x060007DA RID: 2010 RVA: 0x0000E2A0 File Offset: 0x0000C4A0
		protected override bool OnRead()
		{
			bool flag = true;
			this.AttackerTeamScore = GameNetworkMessage.ReadIntFromPacket(CompressionMission.TeamScoreCompressionInfo, ref flag);
			this.DefenderTeamScore = GameNetworkMessage.ReadIntFromPacket(CompressionMission.TeamScoreCompressionInfo, ref flag);
			return flag;
		}

		// Token: 0x060007DB RID: 2011 RVA: 0x0000E2D4 File Offset: 0x0000C4D4
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteIntToPacket(this.AttackerTeamScore, CompressionMission.TeamScoreCompressionInfo);
			GameNetworkMessage.WriteIntToPacket(this.DefenderTeamScore, CompressionMission.TeamScoreCompressionInfo);
		}

		// Token: 0x060007DC RID: 2012 RVA: 0x0000E2F6 File Offset: 0x0000C4F6
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Mission | MultiplayerMessageFilter.GameMode;
		}

		// Token: 0x060007DD RID: 2013 RVA: 0x0000E2FE File Offset: 0x0000C4FE
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[] { "Update round score. Attackers: ", this.AttackerTeamScore, ", defenders: ", this.DefenderTeamScore });
		}
	}
}
