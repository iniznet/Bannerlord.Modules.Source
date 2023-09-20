using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
	// Token: 0x020000BB RID: 187
	[DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
	public sealed class TeamSetIsEnemyOf : GameNetworkMessage
	{
		// Token: 0x170001C2 RID: 450
		// (get) Token: 0x060007C0 RID: 1984 RVA: 0x0000E0C4 File Offset: 0x0000C2C4
		// (set) Token: 0x060007C1 RID: 1985 RVA: 0x0000E0CC File Offset: 0x0000C2CC
		public MBTeam Team1 { get; private set; }

		// Token: 0x170001C3 RID: 451
		// (get) Token: 0x060007C2 RID: 1986 RVA: 0x0000E0D5 File Offset: 0x0000C2D5
		// (set) Token: 0x060007C3 RID: 1987 RVA: 0x0000E0DD File Offset: 0x0000C2DD
		public MBTeam Team2 { get; private set; }

		// Token: 0x170001C4 RID: 452
		// (get) Token: 0x060007C4 RID: 1988 RVA: 0x0000E0E6 File Offset: 0x0000C2E6
		// (set) Token: 0x060007C5 RID: 1989 RVA: 0x0000E0EE File Offset: 0x0000C2EE
		public bool IsEnemyOf { get; private set; }

		// Token: 0x060007C6 RID: 1990 RVA: 0x0000E0F7 File Offset: 0x0000C2F7
		public TeamSetIsEnemyOf(Team team1, Team team2, bool isEnemyOf)
		{
			this.Team1 = team1.MBTeam;
			this.Team2 = team2.MBTeam;
			this.IsEnemyOf = isEnemyOf;
		}

		// Token: 0x060007C7 RID: 1991 RVA: 0x0000E11E File Offset: 0x0000C31E
		public TeamSetIsEnemyOf()
		{
		}

		// Token: 0x060007C8 RID: 1992 RVA: 0x0000E126 File Offset: 0x0000C326
		protected override void OnWrite()
		{
			GameNetworkMessage.WriteMBTeamReferenceToPacket(this.Team1, CompressionMission.TeamCompressionInfo);
			GameNetworkMessage.WriteMBTeamReferenceToPacket(this.Team2, CompressionMission.TeamCompressionInfo);
			GameNetworkMessage.WriteBoolToPacket(this.IsEnemyOf);
		}

		// Token: 0x060007C9 RID: 1993 RVA: 0x0000E154 File Offset: 0x0000C354
		protected override bool OnRead()
		{
			bool flag = true;
			this.Team1 = GameNetworkMessage.ReadMBTeamReferenceFromPacket(CompressionMission.TeamCompressionInfo, ref flag);
			this.Team2 = GameNetworkMessage.ReadMBTeamReferenceFromPacket(CompressionMission.TeamCompressionInfo, ref flag);
			this.IsEnemyOf = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			return flag;
		}

		// Token: 0x060007CA RID: 1994 RVA: 0x0000E195 File Offset: 0x0000C395
		protected override MultiplayerMessageFilter OnGetLogFilter()
		{
			return MultiplayerMessageFilter.Mission;
		}

		// Token: 0x060007CB RID: 1995 RVA: 0x0000E1A0 File Offset: 0x0000C3A0
		protected override string OnGetLogFormat()
		{
			return string.Concat(new object[]
			{
				this.Team1,
				" is now ",
				this.IsEnemyOf ? "" : "not an ",
				"enemy of ",
				this.Team2
			});
		}
	}
}
