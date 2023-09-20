using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TaleWorlds.MountAndBlade.Diamond
{
	[Serializable]
	public class MatchmakingQueueRegionStats
	{
		[JsonProperty]
		public string Region { get; set; }

		[JsonIgnore]
		public int TotalCount
		{
			get
			{
				int num = 0;
				foreach (MatchmakingQueueGameTypeStats matchmakingQueueGameTypeStats in this.GameTypeStats)
				{
					num += matchmakingQueueGameTypeStats.Count;
				}
				return num;
			}
		}

		[JsonProperty]
		public int MaxWaitTime { get; set; }

		[JsonProperty]
		public int MinWaitTime { get; set; }

		[JsonProperty]
		public int MedianWaitTime { get; set; }

		[JsonProperty]
		public int AverageWaitTime { get; set; }

		public MatchmakingQueueRegionStats(string region)
		{
			this.Region = region;
			this.GameTypeStats = new List<MatchmakingQueueGameTypeStats>();
		}

		public MatchmakingQueueGameTypeStats GetQueueCountObjectOf(string[] gameTypes)
		{
			if (gameTypes != null)
			{
				foreach (MatchmakingQueueGameTypeStats matchmakingQueueGameTypeStats in this.GameTypeStats)
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
			this.GameTypeStats.Add(matchmakingQueueGameTypeStats);
		}

		public int GetQueueCountOf(string[] gameTypes)
		{
			int num = 0;
			if (gameTypes != null)
			{
				foreach (MatchmakingQueueGameTypeStats matchmakingQueueGameTypeStats in this.GameTypeStats)
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

		[JsonProperty]
		public List<MatchmakingQueueGameTypeStats> GameTypeStats;
	}
}
