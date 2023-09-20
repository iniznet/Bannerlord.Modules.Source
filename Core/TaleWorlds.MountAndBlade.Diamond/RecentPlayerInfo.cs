using System;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x0200014A RID: 330
	public class RecentPlayerInfo
	{
		// Token: 0x170002E8 RID: 744
		// (get) Token: 0x06000852 RID: 2130 RVA: 0x0000D78C File Offset: 0x0000B98C
		// (set) Token: 0x06000853 RID: 2131 RVA: 0x0000D794 File Offset: 0x0000B994
		public string PlayerId { get; set; }

		// Token: 0x170002E9 RID: 745
		// (get) Token: 0x06000854 RID: 2132 RVA: 0x0000D79D File Offset: 0x0000B99D
		// (set) Token: 0x06000855 RID: 2133 RVA: 0x0000D7A5 File Offset: 0x0000B9A5
		public string PlayerName { get; set; }

		// Token: 0x170002EA RID: 746
		// (get) Token: 0x06000856 RID: 2134 RVA: 0x0000D7AE File Offset: 0x0000B9AE
		// (set) Token: 0x06000857 RID: 2135 RVA: 0x0000D7B6 File Offset: 0x0000B9B6
		public int ImportanceScore { get; set; }

		// Token: 0x170002EB RID: 747
		// (get) Token: 0x06000858 RID: 2136 RVA: 0x0000D7BF File Offset: 0x0000B9BF
		// (set) Token: 0x06000859 RID: 2137 RVA: 0x0000D7C7 File Offset: 0x0000B9C7
		public DateTime InteractionTime { get; set; }
	}
}
