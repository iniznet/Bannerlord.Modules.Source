using System;
using System.Collections.Generic;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x0200012E RID: 302
	[Serializable]
	public class MatchmakingWaitTimeStats
	{
		// Token: 0x1700027D RID: 637
		// (get) Token: 0x06000733 RID: 1843 RVA: 0x0000B8D5 File Offset: 0x00009AD5
		// (set) Token: 0x06000734 RID: 1844 RVA: 0x0000B8DC File Offset: 0x00009ADC
		public static MatchmakingWaitTimeStats Empty { get; private set; } = new MatchmakingWaitTimeStats();

		// Token: 0x06000736 RID: 1846 RVA: 0x0000B8F0 File Offset: 0x00009AF0
		public MatchmakingWaitTimeStats()
		{
			this._regionStats = new List<MatchmakingWaitTimeRegionStats>();
		}

		// Token: 0x06000737 RID: 1847 RVA: 0x0000B903 File Offset: 0x00009B03
		public void AddRegionStats(MatchmakingWaitTimeRegionStats regionStats)
		{
			this._regionStats.Add(regionStats);
		}

		// Token: 0x06000738 RID: 1848 RVA: 0x0000B914 File Offset: 0x00009B14
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

		// Token: 0x06000739 RID: 1849 RVA: 0x0000B980 File Offset: 0x00009B80
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

		// Token: 0x04000340 RID: 832
		private List<MatchmakingWaitTimeRegionStats> _regionStats;
	}
}
