using System;

namespace TaleWorlds.PlatformService.GOG
{
	// Token: 0x02000009 RID: 9
	public class GOGAchievement
	{
		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000062 RID: 98 RVA: 0x00003002 File Offset: 0x00001202
		// (set) Token: 0x06000063 RID: 99 RVA: 0x0000300A File Offset: 0x0000120A
		public string AchievementName { get; set; }

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000064 RID: 100 RVA: 0x00003013 File Offset: 0x00001213
		// (set) Token: 0x06000065 RID: 101 RVA: 0x0000301B File Offset: 0x0000121B
		public string Name { get; set; }

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000066 RID: 102 RVA: 0x00003024 File Offset: 0x00001224
		// (set) Token: 0x06000067 RID: 103 RVA: 0x0000302C File Offset: 0x0000122C
		public string Description { get; set; }

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000068 RID: 104 RVA: 0x00003035 File Offset: 0x00001235
		// (set) Token: 0x06000069 RID: 105 RVA: 0x0000303D File Offset: 0x0000123D
		public bool Achieved { get; set; }

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x0600006A RID: 106 RVA: 0x00003046 File Offset: 0x00001246
		// (set) Token: 0x0600006B RID: 107 RVA: 0x0000304E File Offset: 0x0000124E
		public int Progress { get; set; }

		// Token: 0x04000018 RID: 24
		public int AchievementID;
	}
}
