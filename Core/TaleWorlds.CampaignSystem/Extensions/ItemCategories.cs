using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.Extensions
{
	// Token: 0x02000154 RID: 340
	public static class ItemCategories
	{
		// Token: 0x17000667 RID: 1639
		// (get) Token: 0x06001838 RID: 6200 RVA: 0x0007B013 File Offset: 0x00079213
		public static MBReadOnlyList<ItemCategory> All
		{
			get
			{
				return Campaign.Current.AllItemCategories;
			}
		}
	}
}
