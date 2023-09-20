using System;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.Map
{
	// Token: 0x020000CC RID: 204
	internal interface ILocatable<T>
	{
		// Token: 0x1700054E RID: 1358
		// (get) Token: 0x0600129A RID: 4762
		// (set) Token: 0x0600129B RID: 4763
		[CachedData]
		int LocatorNodeIndex { get; set; }

		// Token: 0x1700054F RID: 1359
		// (get) Token: 0x0600129C RID: 4764
		// (set) Token: 0x0600129D RID: 4765
		[CachedData]
		T NextLocatable { get; set; }

		// Token: 0x17000550 RID: 1360
		// (get) Token: 0x0600129E RID: 4766
		[CachedData]
		Vec2 GetPosition2D { get; }
	}
}
