using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.Extensions
{
	public static class Items
	{
		public static MBReadOnlyList<ItemObject> All
		{
			get
			{
				return Campaign.Current.AllItems;
			}
		}

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
