using System;
using System.Collections.Generic;

namespace TaleWorlds.MountAndBlade.Diamond
{
	[Serializable]
	public class MatchmakingWaitTimeStats
	{
		public static MatchmakingWaitTimeStats Empty { get; private set; } = new MatchmakingWaitTimeStats();

		public MatchmakingWaitTimeStats()
		{
			this._regionStats = new List<MatchmakingWaitTimeRegionStats>();
		}

		public void AddRegionStats(MatchmakingWaitTimeRegionStats regionStats)
		{
			this._regionStats.Add(regionStats);
		}

		public MatchmakingWaitTimeRegionStats GetRegionStats(string region)
		{
			foreach (MatchmakingWaitTimeRegionStats matchmakingWaitTimeRegionStats in this._regionStats)
			{
				if (matchmakingWaitTimeRegionStats.Region.ToLower() == region.ToLower())
				{
					return matchmakingWaitTimeRegionStats;
				}
			}
			return null;
		}

		public int GetWaitTime(string region, string gameType, WaitTimeStatType statType)
		{
			int num = 0;
			if (!string.IsNullOrEmpty(region) && !string.IsNullOrEmpty(gameType))
			{
				MatchmakingWaitTimeRegionStats regionStats = this.GetRegionStats(region);
				if (regionStats != null)
				{
					num = regionStats.GetWaitTime(gameType, statType);
				}
			}
			return num;
		}

		private List<MatchmakingWaitTimeRegionStats> _regionStats;
	}
}
