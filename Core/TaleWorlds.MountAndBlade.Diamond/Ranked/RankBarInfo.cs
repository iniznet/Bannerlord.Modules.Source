using System;

namespace TaleWorlds.MountAndBlade.Diamond.Ranked
{
	[Serializable]
	public class RankBarInfo
	{
		public string RankId { get; private set; }

		public string PreviousRankId { get; private set; }

		public string NextRankId { get; private set; }

		public float ProgressPercentage { get; private set; }

		public int Rating { get; private set; }

		public int RatingToNextRank { get; private set; }

		public bool IsEvaluating { get; private set; }

		public int EvaluationMatchesPlayed { get; private set; }

		public int TotalEvaluationMatchesRequired { get; private set; }

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

		public static RankBarInfo CreateBarInfo(string rankId, string previousRankId, string nextRankId, float progressPercentage, int rating, int ratingToNextRank)
		{
			return new RankBarInfo(rankId, previousRankId, nextRankId, progressPercentage, rating, ratingToNextRank, false, 0, 0);
		}

		public static RankBarInfo CreateUnrankedInfo(int matchesPlayed, int totalMatchesRequired)
		{
			return new RankBarInfo("", "", "", 0f, 0, 0, true, matchesPlayed, totalMatchesRequired);
		}
	}
}
