using System;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x02000141 RID: 321
	[Serializable]
	public class PlayerStatsDuel : PlayerStatsBase
	{
		// Token: 0x170002C7 RID: 711
		// (get) Token: 0x060007F4 RID: 2036 RVA: 0x0000CCA2 File Offset: 0x0000AEA2
		// (set) Token: 0x060007F5 RID: 2037 RVA: 0x0000CCAA File Offset: 0x0000AEAA
		public int DuelsWon { get; set; }

		// Token: 0x170002C8 RID: 712
		// (get) Token: 0x060007F6 RID: 2038 RVA: 0x0000CCB3 File Offset: 0x0000AEB3
		// (set) Token: 0x060007F7 RID: 2039 RVA: 0x0000CCBB File Offset: 0x0000AEBB
		public int InfantryWins { get; set; }

		// Token: 0x170002C9 RID: 713
		// (get) Token: 0x060007F8 RID: 2040 RVA: 0x0000CCC4 File Offset: 0x0000AEC4
		// (set) Token: 0x060007F9 RID: 2041 RVA: 0x0000CCCC File Offset: 0x0000AECC
		public int ArcherWins { get; set; }

		// Token: 0x170002CA RID: 714
		// (get) Token: 0x060007FA RID: 2042 RVA: 0x0000CCD5 File Offset: 0x0000AED5
		// (set) Token: 0x060007FB RID: 2043 RVA: 0x0000CCDD File Offset: 0x0000AEDD
		public int CavalryWins { get; set; }

		// Token: 0x060007FC RID: 2044 RVA: 0x0000CCE6 File Offset: 0x0000AEE6
		public PlayerStatsDuel()
		{
			base.GameType = "Duel";
		}

		// Token: 0x060007FD RID: 2045 RVA: 0x0000CCF9 File Offset: 0x0000AEF9
		public void FillWith(PlayerId playerId, int killCount, int deathCount, int assistCount, int winCount, int loseCount, int forfeitCount, int duelsWon, int infantryWins, int archerWins, int cavalryWins)
		{
			base.FillWith(playerId, killCount, deathCount, assistCount, winCount, loseCount, forfeitCount);
			this.DuelsWon = duelsWon;
			this.InfantryWins = infantryWins;
			this.ArcherWins = archerWins;
			this.CavalryWins = cavalryWins;
		}

		// Token: 0x060007FE RID: 2046 RVA: 0x0000CD2C File Offset: 0x0000AF2C
		public void FillWithNewPlayer(PlayerId playerId)
		{
			this.FillWith(playerId, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
		}

		// Token: 0x060007FF RID: 2047 RVA: 0x0000CD4C File Offset: 0x0000AF4C
		public void Update(BattlePlayerStatsDuel stats, bool won)
		{
			base.Update(stats, won);
			this.DuelsWon += stats.DuelsWon;
			this.InfantryWins += stats.InfantryWins;
			this.ArcherWins += stats.ArcherWins;
			this.CavalryWins += stats.CavalryWins;
		}
	}
}
