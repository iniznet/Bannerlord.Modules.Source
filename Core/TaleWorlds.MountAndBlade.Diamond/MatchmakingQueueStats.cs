using System;
using System.Collections.Generic;

namespace TaleWorlds.MountAndBlade.Diamond
{
	[Serializable]
	public class MatchmakingQueueStats
	{
		public static MatchmakingQueueStats Empty { get; private set; } = new MatchmakingQueueStats();

		public int TotalCount
		{
			get
			{
				int num = 0;
				foreach (MatchmakingQueueRegionStats matchmakingQueueRegionStats in this._regionStats)
				{
					num += matchmakingQueueRegionStats.TotalCount;
				}
				return num;
			}
		}

		public int AverageWaitTime
		{
			get
			{
				int num = 0;
				int num2 = 0;
				if (this._regionStats.Count > 0)
				{
					foreach (MatchmakingQueueRegionStats matchmakingQueueRegionStats in this._regionStats)
					{
						num2 += matchmakingQueueRegionStats.AverageWaitTime;
					}
					num = num2 / this._regionStats.Count;
				}
				return num;
			}
		}

		public MatchmakingQueueStats()
		{
			this._regionStats = new List<MatchmakingQueueRegionStats>();
		}

		public void AddRegionStats(MatchmakingQueueRegionStats matchmakingQueueRegionStats)
		{
			this._regionStats.Add(matchmakingQueueRegionStats);
		}

		public MatchmakingQueueRegionStats GetRegionStats(string region)
		{
			foreach (MatchmakingQueueRegionStats matchmakingQueueRegionStats in this._regionStats)
			{
				if (matchmakingQueueRegionStats.Region.ToLower() == region.ToLower())
				{
					return matchmakingQueueRegionStats;
				}
			}
			return null;
		}

		public int GetQueueCountOf(string region, string[] gameTypes)
		{
			int num = 0;
			if (!string.IsNullOrEmpty(region) && gameTypes != null)
			{
				MatchmakingQueueRegionStats regionStats = this.GetRegionStats(region);
				if (regionStats != null)
				{
					num = regionStats.GetQueueCountOf(gameTypes);
				}
			}
			return num;
		}

		public string[] GetRegionNames()
		{
			string[] array = new string[this._regionStats.Count];
			for (int i = 0; i < this._regionStats.Count; i++)
			{
				array[i] = this._regionStats[i].Region;
			}
			return array;
		}

		private List<MatchmakingQueueRegionStats> _regionStats;
	}
}
