using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.Extensions
{
	// Token: 0x02000151 RID: 337
	public static class Items
	{
		// Token: 0x17000663 RID: 1635
		// (get) Token: 0x06001833 RID: 6195 RVA: 0x0007AFD9 File Offset: 0x000791D9
		public static MBReadOnlyList<ItemObject> All
		{
			get
			{
				return Campaign.Current.AllItems;
			}
		}

		// Token: 0x17000664 RID: 1636
		// (get) Token: 0x06001834 RID: 6196 RVA: 0x0007AFE5 File Offset: 0x000791E5
		public static IEnumerable<ItemObject> AllTradeGoods
		{
			get
			{
				MBReadOnlyList<ItemObject> all = Items.All;
				foreach (ItemObject itemObject in all)
				{
					if (itemObject.IsTradeGood)
					{
						yield return itemObject;
					}
				}
				List<ItemObject>.Enumerator enumerator = default(List<ItemObject>.Enumerator);
				yield break;
				yield break;
			}
		}
	}
}
