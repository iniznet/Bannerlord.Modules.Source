using System;
using System.Collections.Generic;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x02000130 RID: 304
	[Serializable]
	public class MatchmakingWaitTimeRegionStats
	{
		// Token: 0x1700027E RID: 638
		// (get) Token: 0x0600073A RID: 1850 RVA: 0x0000B9B4 File Offset: 0x00009BB4
		// (set) Token: 0x0600073B RID: 1851 RVA: 0x0000B9BC File Offset: 0x00009BBC
		public string Region { get; private set; }

		// Token: 0x0600073C RID: 1852 RVA: 0x0000B9C5 File Offset: 0x00009BC5
		public MatchmakingWaitTimeRegionStats(string region)
		{
			this.Region = region;
			this._gameTypeAverageWaitTimes = new Dictionary<string, Dictionary<WaitTimeStatType, int>>();
		}

		// Token: 0x0600073D RID: 1853 RVA: 0x0000B9E0 File Offset: 0x00009BE0
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

		// Token: 0x0600073E RID: 1854 RVA: 0x0000BA23 File Offset: 0x00009C23
		public bool HasStatsForGameType(string gameType)
		{
			return gameType != null && this._gameTypeAverageWaitTimes.ContainsKey(gameType);
		}

		// Token: 0x0600073F RID: 1855 RVA: 0x0000BA38 File Offset: 0x00009C38
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

		// Token: 0x04000346 RID: 838
		private Dictionary<string, Dictionary<WaitTimeStatType, int>> _gameTypeAverageWaitTimes;
	}
}
