using System;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x02000140 RID: 320
	[Serializable]
	public class PlayerStatsCaptain : PlayerStatsRanked
	{
		// Token: 0x170002C3 RID: 707
		// (get) Token: 0x060007E9 RID: 2025 RVA: 0x0000CB7C File Offset: 0x0000AD7C
		// (set) Token: 0x060007EA RID: 2026 RVA: 0x0000CB84 File Offset: 0x0000AD84
		public int CaptainsKilled { get; set; }

		// Token: 0x170002C4 RID: 708
		// (get) Token: 0x060007EB RID: 2027 RVA: 0x0000CB8D File Offset: 0x0000AD8D
		// (set) Token: 0x060007EC RID: 2028 RVA: 0x0000CB95 File Offset: 0x0000AD95
		public int MVPs { get; set; }

		// Token: 0x170002C5 RID: 709
		// (get) Token: 0x060007ED RID: 2029 RVA: 0x0000CB9E File Offset: 0x0000AD9E
		// (set) Token: 0x060007EE RID: 2030 RVA: 0x0000CBA6 File Offset: 0x0000ADA6
		public int Score { get; set; }

		// Token: 0x170002C6 RID: 710
		// (get) Token: 0x060007EF RID: 2031 RVA: 0x0000CBAF File Offset: 0x0000ADAF
		public int AverageScore
		{
			get
			{
				if (this.Score / (base.WinCount + base.LoseCount) == 0)
				{
					return 1;
				}
				return base.WinCount + base.LoseCount;
			}
		}

		// Token: 0x060007F0 RID: 2032 RVA: 0x0000CBD6 File Offset: 0x0000ADD6
		public PlayerStatsCaptain()
		{
			base.GameType = "Captain";
		}

		// Token: 0x060007F1 RID: 2033 RVA: 0x0000CBEC File Offset: 0x0000ADEC
		public void FillWith(PlayerId playerId, int killCount, int deathCount, int assistCount, int winCount, int loseCount, int forfeitCount, int rating, int ratingDeviation, string rank, bool evaluating, int evaluationMatchesPlayedCount, int captainsKilled, int mvps, int score)
		{
			base.FillWith(playerId, killCount, deathCount, assistCount, winCount, loseCount, forfeitCount, rating, ratingDeviation, rank, evaluating, evaluationMatchesPlayedCount);
			this.CaptainsKilled = captainsKilled;
			this.MVPs = mvps;
			this.Score = score;
		}

		// Token: 0x060007F2 RID: 2034 RVA: 0x0000CC2C File Offset: 0x0000AE2C
		public void FillWithNewPlayer(PlayerId playerId, int defaultRating, int defaultRatingDeviation)
		{
			this.FillWith(playerId, 0, 0, 0, 0, 0, 0, defaultRating, defaultRatingDeviation, "", true, 0, 0, 0, 0);
		}

		// Token: 0x060007F3 RID: 2035 RVA: 0x0000CC54 File Offset: 0x0000AE54
		public void Update(BattlePlayerStatsCaptain stats, bool won)
		{
			base.Update(stats, won);
			this.CaptainsKilled += stats.CaptainsKilled;
			this.MVPs += stats.MVPs;
			this.Score += stats.Score;
		}
	}
}
