using System;
using System.Collections.Generic;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.OrderOfBattle
{
	// Token: 0x0200002D RID: 45
	public class OrderOfBattleFormationFilterSelectorItemComparer : IComparer<OrderOfBattleFormationFilterSelectorItemVM>
	{
		// Token: 0x06000329 RID: 809 RVA: 0x0000E1F9 File Offset: 0x0000C3F9
		public int Compare(OrderOfBattleFormationFilterSelectorItemVM x, OrderOfBattleFormationFilterSelectorItemVM y)
		{
			return x.FilterType.CompareTo(y.FilterType);
		}
	}
}
