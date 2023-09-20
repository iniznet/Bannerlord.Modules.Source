using System;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x02000142 RID: 322
	[Serializable]
	public class PlayerStatsRanked : PlayerStatsBase
	{
		// Token: 0x170002CB RID: 715
		// (get) Token: 0x06000800 RID: 2048 RVA: 0x0000CDAD File Offset: 0x0000AFAD
		// (set) Token: 0x06000801 RID: 2049 RVA: 0x0000CDB5 File Offset: 0x0000AFB5
		public int Rating { get; set; }

		// Token: 0x170002CC RID: 716
		// (get) Token: 0x06000802 RID: 2050 RVA: 0x0000CDBE File Offset: 0x0000AFBE
		// (set) Token: 0x06000803 RID: 2051 RVA: 0x0000CDC6 File Offset: 0x0000AFC6
		public string Rank { get; set; }

		// Token: 0x170002CD RID: 717
		// (get) Token: 0x06000804 RID: 2052 RVA: 0x0000CDCF File Offset: 0x0000AFCF
		// (set) Token: 0x06000805 RID: 2053 RVA: 0x0000CDD7 File Offset: 0x0000AFD7
		public bool Evaluating { get; set; }

		// Token: 0x170002CE RID: 718
		// (get) Token: 0x06000806 RID: 2054 RVA: 0x0000CDE0 File Offset: 0x0000AFE0
		// (set) Token: 0x06000807 RID: 2055 RVA: 0x0000CDE8 File Offset: 0x0000AFE8
		public int EvaluationMatchesPlayedCount { get; set; }

		// Token: 0x06000808 RID: 2056 RVA: 0x0000CDF1 File Offset: 0x0000AFF1
		public void FillWith(PlayerId playerId, int killCount, int deathCount, int assistCount, int winCount, int loseCount, int forfeitCount, int rating, int ratingDeviation, string rank, bool evaluating, int evaluationMatchesPlayedCount)
		{
			base.FillWith(playerId, killCount, deathCount, assistCount, winCount, loseCount, forfeitCount);
			this.Rating = rating;
			this.Rank = rank;
			this.Evaluating = evaluating;
			this.EvaluationMatchesPlayedCount = evaluationMatchesPlayedCount;
		}

		// Token: 0x06000809 RID: 2057 RVA: 0x0000CE24 File Offset: 0x0000B024
		public virtual void FillWithNewPlayer(PlayerId playerId, string gameType, int defaultRating, int defaultRatingDeviation)
		{
			this.FillWith(playerId, 0, 0, 0, 0, 0, 0, defaultRating, defaultRatingDeviation, "", true, 0);
		}
	}
}
