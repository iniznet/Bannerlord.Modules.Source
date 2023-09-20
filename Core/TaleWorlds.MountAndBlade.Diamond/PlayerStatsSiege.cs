using System;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.Diamond
{
	[Serializable]
	public class PlayerStatsSiege : PlayerStatsBase
	{
		public int WallsBreached { get; set; }

		public int SiegeEngineKills { get; set; }

		public int SiegeEnginesDestroyed { get; set; }

		public int ObjectiveGoldGained { get; set; }

		public int Score { get; set; }

		public int AverageScore
		{
			get
			{
				return this.Score / ((base.WinCount + base.LoseCount != 0) ? (base.WinCount + base.LoseCount) : 1);
			}
		}

		public int AverageKillCount
		{
			get
			{
				return base.KillCount / ((base.WinCount + base.LoseCount != 0) ? (base.WinCount + base.LoseCount) : 1);
			}
		}

		public PlayerStatsSiege()
		{
			base.GameType = "Siege";
		}

		public void FillWith(PlayerId playerId, int killCount, int deathCount, int assistCount, int winCount, int loseCount, int forfeitCount, int wallsBreached, int siegeEngineKills, int siegeEnginesDestroyed, int objectiveGoldGained, int score)
		{
			base.FillWith(playerId, killCount, deathCount, assistCount, winCount, loseCount, forfeitCount);
			this.WallsBreached = wallsBreached;
			this.SiegeEngineKills = siegeEngineKills;
			this.SiegeEnginesDestroyed = siegeEnginesDestroyed;
			this.ObjectiveGoldGained = objectiveGoldGained;
			this.Score = score;
		}

		public void FillWithNewPlayer(PlayerId playerId)
		{
			this.FillWith(playerId, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
		}

		public void Update(BattlePlayerStatsSiege stats, bool won)
		{
			base.Update(stats, won);
			this.WallsBreached += stats.WallsBreached;
			this.SiegeEngineKills += stats.SiegeEngineKills;
			this.SiegeEnginesDestroyed += stats.SiegeEnginesDestroyed;
			this.ObjectiveGoldGained += stats.ObjectiveGoldGained;
			this.Score += stats.Score;
		}
	}
}
