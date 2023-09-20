using System;
using System.Linq;

namespace TaleWorlds.MountAndBlade.Diamond
{
	[Serializable]
	public class MatchmakingQueueGameTypeStats
	{
		public string[] GameTypes { get; private set; }

		public int Count { get; set; }

		public int TotalWaitTime { get; set; }

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
