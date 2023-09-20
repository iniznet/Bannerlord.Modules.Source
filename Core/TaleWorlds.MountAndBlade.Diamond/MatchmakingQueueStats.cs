using System;
using System.Collections.Generic;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x0200012B RID: 299
	[Serializable]
	public class MatchmakingQueueStats
	{
		// Token: 0x17000271 RID: 625
		// (get) Token: 0x0600070F RID: 1807 RVA: 0x0000B469 File Offset: 0x00009669
		// (set) Token: 0x06000710 RID: 1808 RVA: 0x0000B470 File Offset: 0x00009670
		public static MatchmakingQueueStats Empty { get; private set; } = new MatchmakingQueueStats();

		// Token: 0x17000272 RID: 626
		// (get) Token: 0x06000711 RID: 1809 RVA: 0x0000B478 File Offset: 0x00009678
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

		// Token: 0x17000273 RID: 627
		// (get) Token: 0x06000712 RID: 1810 RVA: 0x0000B4D0 File Offset: 0x000096D0
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

		// Token: 0x06000714 RID: 1812 RVA: 0x0000B554 File Offset: 0x00009754
		public MatchmakingQueueStats()
		{
			this._regionStats = new List<MatchmakingQueueRegionStats>();
		}

		// Token: 0x06000715 RID: 1813 RVA: 0x0000B567 File Offset: 0x00009767
		public void AddRegionStats(MatchmakingQueueRegionStats matchmakingQueueRegionStats)
		{
			this._regionStats.Add(matchmakingQueueRegionStats);
		}

		// Token: 0x06000716 RID: 1814 RVA: 0x0000B578 File Offset: 0x00009778
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

		// Token: 0x06000717 RID: 1815 RVA: 0x0000B5E4 File Offset: 0x000097E4
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

		// Token: 0x06000718 RID: 1816 RVA: 0x0000B614 File Offset: 0x00009814
		public string[] GetRegionNames()
		{
			string[] array = new string[this._regionStats.Count];
			for (int i = 0; i < this._regionStats.Count; i++)
			{
				array[i] = this._regionStats[i].Region;
			}
			return array;
		}

		// Token: 0x04000335 RID: 821
		private List<MatchmakingQueueRegionStats> _regionStats;
	}
}
