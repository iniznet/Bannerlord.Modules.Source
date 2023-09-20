using System;
using System.Linq;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x0200012D RID: 301
	[Serializable]
	public class MatchmakingQueueGameTypeStats
	{
		// Token: 0x1700027A RID: 634
		// (get) Token: 0x06000729 RID: 1833 RVA: 0x0000B81B File Offset: 0x00009A1B
		// (set) Token: 0x0600072A RID: 1834 RVA: 0x0000B823 File Offset: 0x00009A23
		public string[] GameTypes { get; private set; }

		// Token: 0x1700027B RID: 635
		// (get) Token: 0x0600072B RID: 1835 RVA: 0x0000B82C File Offset: 0x00009A2C
		// (set) Token: 0x0600072C RID: 1836 RVA: 0x0000B834 File Offset: 0x00009A34
		public int Count { get; set; }

		// Token: 0x1700027C RID: 636
		// (get) Token: 0x0600072D RID: 1837 RVA: 0x0000B83D File Offset: 0x00009A3D
		// (set) Token: 0x0600072E RID: 1838 RVA: 0x0000B845 File Offset: 0x00009A45
		public int TotalWaitTime { get; set; }

		// Token: 0x0600072F RID: 1839 RVA: 0x0000B84E File Offset: 0x00009A4E
		public MatchmakingQueueGameTypeStats(string[] gameTypes)
		{
			this.GameTypes = gameTypes;
		}

		// Token: 0x06000730 RID: 1840 RVA: 0x0000B85D File Offset: 0x00009A5D
		public bool HasGameType(string gameType)
		{
			return this.GameTypes.Contains(gameType);
		}

		// Token: 0x06000731 RID: 1841 RVA: 0x0000B86C File Offset: 0x00009A6C
		public bool EqualWith(string[] gameTypes)
		{
			if (this.GameTypes.Length == gameTypes.Length)
			{
				foreach (string text in gameTypes)
				{
					if (!this.HasGameType(text))
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}

		// Token: 0x06000732 RID: 1842 RVA: 0x0000B8A8 File Offset: 0x00009AA8
		internal bool HasAnyGameType(string[] gameTypes)
		{
			foreach (string text in gameTypes)
			{
				if (this.HasGameType(text))
				{
					return true;
				}
			}
			return false;
		}
	}
}
