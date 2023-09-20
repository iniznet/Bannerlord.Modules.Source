using System;
using System.Linq;
using Newtonsoft.Json;

namespace TaleWorlds.MountAndBlade.Diamond
{
	[Serializable]
	public class MatchmakingQueueGameTypeStats
	{
		[JsonProperty]
		public string[] GameTypes { get; set; }

		[JsonProperty]
		public int Count { get; set; }

		[JsonProperty]
		public int TotalWaitTime { get; set; }

		public MatchmakingQueueGameTypeStats()
		{
		}

		public MatchmakingQueueGameTypeStats(string[] gameTypes)
		{
			this.GameTypes = gameTypes;
		}

		public bool HasGameType(string gameType)
		{
			return this.GameTypes.Contains(gameType);
		}

		public bool EqualWith(string[] gameTypes)
		{
			if (this.GameTypes.Length == gameTypes.Length)
			{
				foreach (string text in gameTypes)
				{
					if (!this.HasGameType(text))
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}

		internal bool HasAnyGameType(string[] gameTypes)
		{
			foreach (string text in gameTypes)
			{
				if (this.HasGameType(text))
				{
					return true;
				}
			}
			return false;
		}
	}
}
