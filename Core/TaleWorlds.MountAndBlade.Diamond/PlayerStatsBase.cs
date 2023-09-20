using System;
using Newtonsoft.Json;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.Diamond
{
	[JsonConverter(typeof(PlayerStatsBaseJsonConverter))]
	[Serializable]
	public class PlayerStatsBase
	{
		[JsonProperty]
		public PlayerId PlayerId { get; private set; }

		[JsonProperty]
		public int KillCount { get; set; }

		[JsonProperty]
		public int DeathCount { get; set; }

		[JsonProperty]
		public int AssistCount { get; set; }

		[JsonProperty]
		public int WinCount { get; set; }

		[JsonProperty]
		public int LoseCount { get; set; }

		[JsonProperty]
		public int ForfeitCount { get; set; }

		[JsonIgnore]
		public float AverageKillPerDeath
		{
			get
			{
				return (float)this.KillCount / (float)((this.DeathCount != 0) ? this.DeathCount : 1);
			}
		}

		[JsonProperty]
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
