using System;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.Diamond
{
	[Serializable]
	public class PlayerStatsBase
	{
		public PlayerId PlayerId { get; private set; }

		public int KillCount { get; set; }

		public int DeathCount { get; set; }

		public int AssistCount { get; set; }

		public int WinCount { get; set; }

		public int LoseCount { get; set; }

		public int ForfeitCount { get; set; }

		public float AverageKillPerDeath
		{
			get
			{
				return (float)this.KillCount / (float)((this.DeathCount != 0) ? this.DeathCount : 1);
			}
		}

		public string GameType { get; set; }

		public void FillWith(PlayerId playerId, int killCount, int deathCount, int assistCount, int winCount, int loseCount, int forfeitCount)
		{
			this.PlayerId = playerId;
			this.KillCount = killCount;
			this.DeathCount = deathCount;
			this.AssistCount = assistCount;
			this.WinCount = winCount;
			this.LoseCount = loseCount;
			this.ForfeitCount = forfeitCount;
		}

		public virtual void Update(BattlePlayerStatsBase battleStats, bool won)
		{
			this.KillCount += battleStats.Kills;
			this.DeathCount += battleStats.Deaths;
			this.AssistCount += battleStats.Assists;
			int num;
			if (won)
			{
				num = this.WinCount;
				this.WinCount = num + 1;
				return;
			}
			num = this.LoseCount;
			this.LoseCount = num + 1;
		}
	}
}
