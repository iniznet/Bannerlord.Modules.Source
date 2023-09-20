using System;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.Extensions
{
	// Token: 0x02000156 RID: 342
	public static class ItemObjectExtensions
	{
		// Token: 0x0600183A RID: 6202 RVA: 0x0007B02B File Offset: 0x0007922B
		public static ItemCategory GetItemCategory(this ItemObject item)
		{
			return item.ItemCategory;
		}
	}
}
