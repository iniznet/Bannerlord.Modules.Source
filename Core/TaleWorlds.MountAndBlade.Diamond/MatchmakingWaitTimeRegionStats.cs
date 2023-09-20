using System;
using System.Collections.Generic;

namespace TaleWorlds.MountAndBlade.Diamond
{
	[Serializable]
	public class MatchmakingWaitTimeRegionStats
	{
		public string Region { get; private set; }

		public MatchmakingWaitTimeRegionStats(string region)
		{
			this.Region = region;
			this._gameTypeAverageWaitTimes = new Dictionary<string, Dictionary<WaitTimeStatType, int>>();
		}

		public void SetGameTypeAverage(string gameType, WaitTimeStatType statType, int average)
		{
			Dictionary<WaitTimeStatType, int> dictionary;
			if (!this._gameTypeAverageWaitTimes.TryGetValue(gameType, out dictionary))
			{
				dictionary = new Dictionary<WaitTimeStatType, int>();
				this._gameTypeAverageWaitTimes.Add(gameType, dictionary);
			}
			this._gameTypeAverageWaitTimes[gameType][statType] = average;
		}

		public bool HasStatsForGameType(string gameType)
		{
			return gameType != null && this._gameTypeAverageWaitTimes.ContainsKey(gameType);
		}

		public int GetWaitTime(string gameType, WaitTimeStatType statType)
		{
			Dictionary<WaitTimeStatType, int> dictionary;
			int num;
			if (this._gameTypeAverageWaitTimes.TryGetValue(gameType, out dictionary) && dictionary.TryGetValue(statType, out num))
			{
				return num;
			}
			return int.MaxValue;
		}

		private Dictionary<string, Dictionary<WaitTimeStatType, int>> _gameTypeAverageWaitTimes;
	}
}
