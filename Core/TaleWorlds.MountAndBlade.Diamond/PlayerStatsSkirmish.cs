using System;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.Diamond
{
	[Serializable]
	public class PlayerStatsSkirmish : PlayerStatsRanked
	{
		public int MVPs { get; set; }

		public int Score { get; set; }

		public int AverageScore
		{
			get
			{
				return this.Score / ((base.WinCount + base.LoseCount != 0) ? (base.WinCount + base.LoseCount) : 1);
			}
		}

		public PlayerStatsSkirmish()
		{
			base.GameType = "Skirmish";
		}

		public void FillWith(PlayerId playerId, int killCount, int deathCount, int assistCount, int winCount, int loseCount, int forfeitCount, int rating, int ratingDeviation, string rank, bool evaluating, int evaluationMatchesPlayedCount, int mvps, int score)
		{
			base.FillWith(playerId, killCount, deathCount, assistCount, winCount, loseCount, forfeitCount, rating, ratingDeviation, rank, evaluating, evaluationMatchesPlayedCount);
			this.MVPs = mvps;
			this.Score = score;
		}

		public void FillWithNewPlayer(PlayerId playerId, int defaultRating, int defaultRatingDeviation)
		{
			this.FillWith(playerId, 0, 0, 0, 0, 0, 0, defaultRating, defaultRatingDeviation, "", true, 0, 0, 0);
		}

		public void Update(BattlePlayerStatsSkirmish stats, bool won)
		{
			base.Update(stats, won);
			this.MVPs += stats.MVPs;
			this.Score += stats.Score;
		}
	}
}
