using System;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x02000144 RID: 324
	[Serializable]
	public class PlayerStatsSkirmish : PlayerStatsRanked
	{
		// Token: 0x170002D6 RID: 726
		// (get) Token: 0x0600081B RID: 2075 RVA: 0x0000CFD8 File Offset: 0x0000B1D8
		// (set) Token: 0x0600081C RID: 2076 RVA: 0x0000CFE0 File Offset: 0x0000B1E0
		public int MVPs { get; set; }

		// Token: 0x170002D7 RID: 727
		// (get) Token: 0x0600081D RID: 2077 RVA: 0x0000CFE9 File Offset: 0x0000B1E9
		// (set) Token: 0x0600081E RID: 2078 RVA: 0x0000CFF1 File Offset: 0x0000B1F1
		public int Score { get; set; }

		// Token: 0x170002D8 RID: 728
		// (get) Token: 0x0600081F RID: 2079 RVA: 0x0000CFFA File Offset: 0x0000B1FA
		public int AverageScore
		{
			get
			{
				return this.Score / ((base.WinCount + base.LoseCount != 0) ? (base.WinCount + base.LoseCount) : 1);
			}
		}

		// Token: 0x06000820 RID: 2080 RVA: 0x0000D022 File Offset: 0x0000B222
		public PlayerStatsSkirmish()
		{
			base.GameType = "Skirmish";
		}

		// Token: 0x06000821 RID: 2081 RVA: 0x0000D038 File Offset: 0x0000B238
		public void FillWith(PlayerId playerId, int killCount, int deathCount, int assistCount, int winCount, int loseCount, int forfeitCount, int rating, int ratingDeviation, string rank, bool evaluating, int evaluationMatchesPlayedCount, int mvps, int score)
		{
			base.FillWith(playerId, killCount, deathCount, assistCount, winCount, loseCount, forfeitCount, rating, ratingDeviation, rank, evaluating, evaluationMatchesPlayedCount);
			this.MVPs = mvps;
			this.Score = score;
		}

		// Token: 0x06000822 RID: 2082 RVA: 0x0000D070 File Offset: 0x0000B270
		public void FillWithNewPlayer(PlayerId playerId, int defaultRating, int defaultRatingDeviation)
		{
			this.FillWith(playerId, 0, 0, 0, 0, 0, 0, defaultRating, defaultRatingDeviation, "", true, 0, 0, 0);
		}

		// Token: 0x06000823 RID: 2083 RVA: 0x0000D095 File Offset: 0x0000B295
		public void Update(BattlePlayerStatsSkirmish stats, bool won)
		{
			base.Update(stats, won);
			this.MVPs += stats.MVPs;
			this.Score += stats.Score;
		}
	}
}
