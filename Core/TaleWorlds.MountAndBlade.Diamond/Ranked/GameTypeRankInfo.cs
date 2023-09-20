using System;

namespace TaleWorlds.MountAndBlade.Diamond.Ranked
{
	// Token: 0x02000153 RID: 339
	[Serializable]
	public class GameTypeRankInfo
	{
		// Token: 0x170002F6 RID: 758
		// (get) Token: 0x06000873 RID: 2163 RVA: 0x0000E456 File Offset: 0x0000C656
		// (set) Token: 0x06000874 RID: 2164 RVA: 0x0000E45E File Offset: 0x0000C65E
		public string GameType { get; private set; }

		// Token: 0x170002F7 RID: 759
		// (get) Token: 0x06000875 RID: 2165 RVA: 0x0000E467 File Offset: 0x0000C667
		// (set) Token: 0x06000876 RID: 2166 RVA: 0x0000E46F File Offset: 0x0000C66F
		public RankBarInfo RankBarInfo { get; private set; }

		// Token: 0x06000877 RID: 2167 RVA: 0x0000E478 File Offset: 0x0000C678
		public GameTypeRankInfo(string gameType, RankBarInfo rankBarInfo)
		{
			this.GameType = gameType;
			this.RankBarInfo = rankBarInfo;
		}
	}
}
