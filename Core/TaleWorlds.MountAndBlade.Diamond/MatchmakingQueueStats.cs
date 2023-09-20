using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TaleWorlds.MountAndBlade.Diamond
{
	[Serializable]
	public class MatchmakingQueueStats
	{
		public static MatchmakingQueueStats Empty { get; private set; } = new MatchmakingQueueStats();

		[JsonIgnore]
		public int TotalCount
		{
			get
			{
				int num = 0;
				foreach (MatchmakingQueueRegionStats matchmakingQueueRegionStats in this.RegionStats)
				{
					num += matchmakingQueueRegionStats.TotalCount;
				}
				return num;
			}
		}

		[JsonIgnore]
		public int AverageWaitTime
		{
			get
			{
				int num = 0;
				int num2 = 0;
				if (this.RegionStats.Count > 0)
				{
					foreach (MatchmakingQueueRegionStats matchmakingQueueRegionStats in this.RegionStats)
					{
						num2 += matchmakingQueueRegionStats.AverageWaitTime;
					}
					num = num2 / this.RegionStats.Count;
				}
				return num;
			}
		}

		public MatchmakingQueueStats()
		{
			this.RegionStats = new List<MatchmakingQueueRegionStats>();
		}

		public void AddRegionStats(MatchmakingQueueRegionStats matchmakingQueueRegionStats)
		{
			this.RegionStats.Add(matchmakingQueueRegionStats);
		}

		public MatchmakingQueueRegionStats GetRegionStats(string region)
		{
			foreach (MatchmakingQueueRegionStats matchmakingQueueRegionStats in this.RegionStats)
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
			string[] array = new string[this.RegionStats.Count];
			for (int i = 0; i < this.RegionStats.Count; i++)
			{
				array[i] = this.RegionStats[i].Region;
			}
			return array;
		}

		[JsonProperty]
		public List<MatchmakingQueueRegionStats> RegionStats;
	}
}
