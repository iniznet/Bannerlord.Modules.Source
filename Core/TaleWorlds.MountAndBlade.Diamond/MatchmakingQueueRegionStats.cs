using System;
using System.Collections.Generic;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x0200012C RID: 300
	[Serializable]
	public class MatchmakingQueueRegionStats
	{
		// Token: 0x17000274 RID: 628
		// (get) Token: 0x06000719 RID: 1817 RVA: 0x0000B65D File Offset: 0x0000985D
		// (set) Token: 0x0600071A RID: 1818 RVA: 0x0000B665 File Offset: 0x00009865
		public string Region { get; private set; }

		// Token: 0x17000275 RID: 629
		// (get) Token: 0x0600071B RID: 1819 RVA: 0x0000B670 File Offset: 0x00009870
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

		// Token: 0x17000276 RID: 630
		// (get) Token: 0x0600071C RID: 1820 RVA: 0x0000B6C8 File Offset: 0x000098C8
		// (set) Token: 0x0600071D RID: 1821 RVA: 0x0000B6D0 File Offset: 0x000098D0
		public int MaxWaitTime { get; private set; }

		// Token: 0x17000277 RID: 631
		// (get) Token: 0x0600071E RID: 1822 RVA: 0x0000B6D9 File Offset: 0x000098D9
		// (set) Token: 0x0600071F RID: 1823 RVA: 0x0000B6E1 File Offset: 0x000098E1
		public int MinWaitTime { get; private set; }

		// Token: 0x17000278 RID: 632
		// (get) Token: 0x06000720 RID: 1824 RVA: 0x0000B6EA File Offset: 0x000098EA
		// (set) Token: 0x06000721 RID: 1825 RVA: 0x0000B6F2 File Offset: 0x000098F2
		public int MedianWaitTime { get; private set; }

		// Token: 0x17000279 RID: 633
		// (get) Token: 0x06000722 RID: 1826 RVA: 0x0000B6FB File Offset: 0x000098FB
		// (set) Token: 0x06000723 RID: 1827 RVA: 0x0000B703 File Offset: 0x00009903
		public int AverageWaitTime { get; private set; }

		// Token: 0x06000724 RID: 1828 RVA: 0x0000B70C File Offset: 0x0000990C
		public MatchmakingQueueRegionStats(string region)
		{
			this.Region = region;
			this._gameTypeStats = new List<MatchmakingQueueGameTypeStats>();
		}

		// Token: 0x06000725 RID: 1829 RVA: 0x0000B728 File Offset: 0x00009928
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

		// Token: 0x06000726 RID: 1830 RVA: 0x0000B788 File Offset: 0x00009988
		public void AddStats(MatchmakingQueueGameTypeStats matchmakingQueueGameTypeStats)
		{
			this._gameTypeStats.Add(matchmakingQueueGameTypeStats);
		}

		// Token: 0x06000727 RID: 1831 RVA: 0x0000B798 File Offset: 0x00009998
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

		// Token: 0x06000728 RID: 1832 RVA: 0x0000B7FC File Offset: 0x000099FC
		public void SetWaitTimeStats(int averageWaitTime, int maxWaitTime, int minWaitTime, int medianWaitTime)
		{
			this.AverageWaitTime = averageWaitTime;
			this.MaxWaitTime = maxWaitTime;
			this.MinWaitTime = minWaitTime;
			this.MedianWaitTime = medianWaitTime;
		}

		// Token: 0x04000337 RID: 823
		private List<MatchmakingQueueGameTypeStats> _gameTypeStats;
	}
}
