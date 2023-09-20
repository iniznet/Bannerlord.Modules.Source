using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.Extensions
{
	public static class ItemCategories
	{
		public static MBReadOnlyList<ItemCategory> All
		{
			get
			{
				return Campaign.Current.AllItemCategories;
			}
		}
	}
}
