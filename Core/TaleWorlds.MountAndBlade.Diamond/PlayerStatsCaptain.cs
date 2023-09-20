using System;
using Newtonsoft.Json;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.Diamond
{
	[Serializable]
	public class PlayerStatsCaptain : PlayerStatsRanked
	{
		public int CaptainsKilled { get; set; }

		public int MVPs { get; set; }

		public int Score { get; set; }

		[JsonIgnore]
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

		public PlayerStatsCaptain()
		{
			base.GameType = "Captain";
		}

		public void FillWith(PlayerId playerId, int killCount, int deathCount, int assistCount, int winCount, int loseCount, int forfeitCount, int rating, int ratingDeviation, string rank, bool evaluating, int evaluationMatchesPlayedCount, int captainsKilled, int mvps, int score)
		{
			base.FillWith(playerId, killCount, deathCount, assistCount, winCount, loseCount, forfeitCount, rating, ratingDeviation, rank, evaluating, evaluationMatchesPlayedCount);
			this.CaptainsKilled = captainsKilled;
			this.MVPs = mvps;
			this.Score = score;
		}

		public void FillWithNewPlayer(PlayerId playerId, int defaultRating, int defaultRatingDeviation)
		{
			this.FillWith(playerId, 0, 0, 0, 0, 0, 0, defaultRating, defaultRatingDeviation, "", true, 0, 0, 0, 0);
		}

		public void Update(BattlePlayerStatsCaptain stats, bool won)
		{
			base.Update(stats, won);
			this.CaptainsKilled += stats.CaptainsKilled;
			this.MVPs += stats.MVPs;
			this.Score += stats.Score;
		}
	}
}
