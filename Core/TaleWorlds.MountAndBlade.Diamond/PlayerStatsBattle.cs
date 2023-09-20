using System;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x0200013F RID: 319
	[Serializable]
	public class PlayerStatsBattle : PlayerStatsBase
	{
		// Token: 0x170002C1 RID: 705
		// (get) Token: 0x060007E1 RID: 2017 RVA: 0x0000CAD6 File Offset: 0x0000ACD6
		// (set) Token: 0x060007E2 RID: 2018 RVA: 0x0000CADE File Offset: 0x0000ACDE
		public int RoundsWon { get; private set; }

		// Token: 0x170002C2 RID: 706
		// (get) Token: 0x060007E3 RID: 2019 RVA: 0x0000CAE7 File Offset: 0x0000ACE7
		// (set) Token: 0x060007E4 RID: 2020 RVA: 0x0000CAEF File Offset: 0x0000ACEF
		public int RoundsLost { get; private set; }

		// Token: 0x060007E5 RID: 2021 RVA: 0x0000CAF8 File Offset: 0x0000ACF8
		public PlayerStatsBattle()
		{
			base.GameType = "Battle";
		}

		// Token: 0x060007E6 RID: 2022 RVA: 0x0000CB0B File Offset: 0x0000AD0B
		public void FillWith(PlayerId playerId, int killCount, int deathCount, int assistCount, int winCount, int loseCount, int forfeitCount, int roundsWon, int roundsLost)
		{
			base.FillWith(playerId, killCount, deathCount, assistCount, winCount, loseCount, forfeitCount);
			this.RoundsWon = roundsWon;
			this.RoundsLost = roundsLost;
		}

		// Token: 0x060007E7 RID: 2023 RVA: 0x0000CB30 File Offset: 0x0000AD30
		public void FillWithNewPlayer(PlayerId playerId)
		{
			this.FillWith(playerId, 0, 0, 0, 0, 0, 0, 0, 0);
		}

		// Token: 0x060007E8 RID: 2024 RVA: 0x0000CB4C File Offset: 0x0000AD4C
		public void Update(BattlePlayerStatsBattle stats, bool won)
		{
			base.Update(stats, won);
			this.RoundsWon += stats.RoundsWon;
			this.RoundsLost += stats.RoundsLost;
		}
	}
}
