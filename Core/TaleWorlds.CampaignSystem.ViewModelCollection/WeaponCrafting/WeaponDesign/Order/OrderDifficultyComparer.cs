using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.CraftingSystem;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting.WeaponDesign.Order
{
	// Token: 0x020000EC RID: 236
	public class OrderDifficultyComparer : IComparer<CraftingOrder>
	{
		// Token: 0x0600162F RID: 5679 RVA: 0x00052EDA File Offset: 0x000510DA
		public int Compare(CraftingOrder x, CraftingOrder y)
		{
			return (int)(x.OrderDifficulty - y.OrderDifficulty);
		}
	}
}
