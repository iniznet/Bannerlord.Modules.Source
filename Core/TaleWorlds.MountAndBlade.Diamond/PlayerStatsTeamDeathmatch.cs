using System;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.Diamond
{
	[Serializable]
	public class PlayerStatsTeamDeathmatch : PlayerStatsBase
	{
		public int Score { get; set; }

		public float AverageScore
		{
			get
			{
				return (float)this.Score / (float)((base.WinCount + base.LoseCount != 0) ? (base.WinCount + base.LoseCount) : 1);
			}
		}

		public PlayerStatsTeamDeathmatch()
		{
			base.GameType = "TeamDeathmatch";
		}

		public void FillWith(PlayerId playerId, int killCount, int deathCount, int assistCount, int winCount, int loseCount, int forfeitCount, int score)
		{
			base.FillWith(playerId, killCount, deathCount, assistCount, winCount, loseCount, forfeitCount);
			this.Score = score;
		}

		public void FillWithNewPlayer(PlayerId playerId)
		{
			this.FillWith(playerId, 0, 0, 0, 0, 0, 0, 0);
		}

		public void Update(BattlePlayerStatsTeamDeathmatch stats, bool won)
		{
			base.Update(stats, won);
			this.Score += stats.Score;
		}
	}
}
