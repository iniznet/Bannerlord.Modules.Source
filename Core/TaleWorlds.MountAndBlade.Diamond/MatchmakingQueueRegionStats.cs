using System;
using System.Collections.Generic;

namespace TaleWorlds.MountAndBlade.Diamond
{
	[Serializable]
	public class MatchmakingQueueRegionStats
	{
		public string Region { get; private set; }

		public int TotalCount
		{
			get
			{
				int num = 0;
				foreach (MatchmakingQueueGameTypeStats matchmakingQueueGameTypeStats in this._gameTypeStats)
				{
					num += matchmakingQueueGameTypeStats.Count;
				}
				return num;
			}
		}

		public int MaxWaitTime { get; private set; }

		public int MinWaitTime { get; private set; }

		public int MedianWaitTime { get; private set; }

		public int AverageWaitTime { get; private set; }

		public MatchmakingQueueRegionStats(string region)
		{
			this.Region = region;
			this._gameTypeStats = new List<MatchmakingQueueGameTypeStats>();
		}

		public MatchmakingQueueGameTypeStats GetQueueCountObjectOf(string[] gameTypes)
		{
			if (gameTypes != null)
			{
				foreach (MatchmakingQueueGameTypeStats matchmakingQueueGameTypeStats in this._gameTypeStats)
				{
					if (matchmakingQueueGameTypeStats.EqualWith(gameTypes))
					{
						return matchmakingQueueGameTypeStats;
					}
				}
			}
			return null;
		}

		public void AddStats(MatchmakingQueueGameTypeStats matchmakingQueueGameTypeStats)
		{
			this._gameTypeStats.Add(matchmakingQueueGameTypeStats);
		}

		public int GetQueueCountOf(string[] gameTypes)
		{
			int num = 0;
			if (gameTypes != null)
			{
				foreach (MatchmakingQueueGameTypeStats matchmakingQueueGameTypeStats in this._gameTypeStats)
				{
					if (matchmakingQueueGameTypeStats.HasAnyGameType(gameTypes))
					{
						num += matchmakingQueueGameTypeStats.Count;
					}
				}
			}
			return num;
		}

		public void SetWaitTimeStats(int averageWaitTime, int maxWaitTime, int minWaitTime, int medianWaitTime)
		{
			this.AverageWaitTime = averageWaitTime;
			this.MaxWaitTime = maxWaitTime;
			this.MinWaitTime = minWaitTime;
			this.MedianWaitTime = medianWaitTime;
		}

		private List<MatchmakingQueueGameTypeStats> _gameTypeStats;
	}
}
