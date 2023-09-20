using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.CraftingSystem;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting.WeaponDesign.Order
{
	public class OrderDifficultyComparer : IComparer<CraftingOrder>
	{
		public int Compare(CraftingOrder x, CraftingOrder y)
		{
			return (int)(x.OrderDifficulty - y.OrderDifficulty);
		}
	}
}
