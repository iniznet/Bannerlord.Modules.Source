using System;

namespace TaleWorlds.MountAndBlade.Diamond.Ranked
{
	// Token: 0x02000154 RID: 340
	[Serializable]
	public class RankBarInfo
	{
		// Token: 0x170002F8 RID: 760
		// (get) Token: 0x06000878 RID: 2168 RVA: 0x0000E48E File Offset: 0x0000C68E
		// (set) Token: 0x06000879 RID: 2169 RVA: 0x0000E496 File Offset: 0x0000C696
		public string RankId { get; private set; }

		// Token: 0x170002F9 RID: 761
		// (get) Token: 0x0600087A RID: 2170 RVA: 0x0000E49F File Offset: 0x0000C69F
		// (set) Token: 0x0600087B RID: 2171 RVA: 0x0000E4A7 File Offset: 0x0000C6A7
		public string PreviousRankId { get; private set; }

		// Token: 0x170002FA RID: 762
		// (get) Token: 0x0600087C RID: 2172 RVA: 0x0000E4B0 File Offset: 0x0000C6B0
		// (set) Token: 0x0600087D RID: 2173 RVA: 0x0000E4B8 File Offset: 0x0000C6B8
		public string NextRankId { get; private set; }

		// Token: 0x170002FB RID: 763
		// (get) Token: 0x0600087E RID: 2174 RVA: 0x0000E4C1 File Offset: 0x0000C6C1
		// (set) Token: 0x0600087F RID: 2175 RVA: 0x0000E4C9 File Offset: 0x0000C6C9
		public float ProgressPercentage { get; private set; }

		// Token: 0x170002FC RID: 764
		// (get) Token: 0x06000880 RID: 2176 RVA: 0x0000E4D2 File Offset: 0x0000C6D2
		// (set) Token: 0x06000881 RID: 2177 RVA: 0x0000E4DA File Offset: 0x0000C6DA
		public int Rating { get; private set; }

		// Token: 0x170002FD RID: 765
		// (get) Token: 0x06000882 RID: 2178 RVA: 0x0000E4E3 File Offset: 0x0000C6E3
		// (set) Token: 0x06000883 RID: 2179 RVA: 0x0000E4EB File Offset: 0x0000C6EB
		public int RatingToNextRank { get; private set; }

		// Token: 0x170002FE RID: 766
		// (get) Token: 0x06000884 RID: 2180 RVA: 0x0000E4F4 File Offset: 0x0000C6F4
		// (set) Token: 0x06000885 RID: 2181 RVA: 0x0000E4FC File Offset: 0x0000C6FC
		public bool IsEvaluating { get; private set; }

		// Token: 0x170002FF RID: 767
		// (get) Token: 0x06000886 RID: 2182 RVA: 0x0000E505 File Offset: 0x0000C705
		// (set) Token: 0x06000887 RID: 2183 RVA: 0x0000E50D File Offset: 0x0000C70D
		public int EvaluationMatchesPlayed { get; private set; }

		// Token: 0x17000300 RID: 768
		// (get) Token: 0x06000888 RID: 2184 RVA: 0x0000E516 File Offset: 0x0000C716
		// (set) Token: 0x06000889 RID: 2185 RVA: 0x0000E51E File Offset: 0x0000C71E
		public int TotalEvaluationMatchesRequired { get; private set; }

		// Token: 0x0600088A RID: 2186 RVA: 0x0000E528 File Offset: 0x0000C728
		public RankBarInfo(string rankId, string previousRankId, string nextRankId, float progressPercentage, int rating, int ratingToNextRank, bool isEvaluating, int evaluationMatchesPlayed, int totalEvaluationMatchesRequired)
		{
			this.RankId = rankId;
			this.PreviousRankId = previousRankId;
			this.NextRankId = nextRankId;
			this.ProgressPercentage = progressPercentage;
			this.Rating = rating;
			this.RatingToNextRank = ratingToNextRank;
			this.IsEvaluating = isEvaluating;
			this.EvaluationMatchesPlayed = evaluationMatchesPlayed;
			this.TotalEvaluationMatchesRequired = totalEvaluationMatchesRequired;
		}

		// Token: 0x0600088B RID: 2187 RVA: 0x0000E580 File Offset: 0x0000C780
		public static RankBarInfo CreateBarInfo(string rankId, string previousRankId, string nextRankId, float progressPercentage, int rating, int ratingToNextRank)
		{
			return new RankBarInfo(rankId, previousRankId, nextRankId, progressPercentage, rating, ratingToNextRank, false, 0, 0);
		}

		// Token: 0x0600088C RID: 2188 RVA: 0x0000E5A0 File Offset: 0x0000C7A0
		public static RankBarInfo CreateUnrankedInfo(int matchesPlayed, int totalMatchesRequired)
		{
			return new RankBarInfo("", "", "", 0f, 0, 0, true, matchesPlayed, totalMatchesRequired);
		}
	}
}
