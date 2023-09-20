﻿using System;

namespace TaleWorlds.MountAndBlade.Diamond.Ranked
{
	[Serializable]
	public class RankBarInfo
	{
		public string RankId { get; set; }

		public string PreviousRankId { get; set; }

		public string NextRankId { get; set; }

		public float ProgressPercentage { get; set; }

		public int Rating { get; set; }

		public int RatingToNextRank { get; set; }

		public bool IsEvaluating { get; set; }

		public int EvaluationMatchesPlayed { get; set; }

		public int TotalEvaluationMatchesRequired { get; set; }

		public RankBarInfo()
		{
		}

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
