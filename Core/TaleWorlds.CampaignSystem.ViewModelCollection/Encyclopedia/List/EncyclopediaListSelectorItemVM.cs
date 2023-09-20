using System;
using TaleWorlds.Core.ViewModelCollection.Selector;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.List
{
	// Token: 0x020000C1 RID: 193
	public class EncyclopediaListSelectorItemVM : SelectorItemVM
	{
		// Token: 0x060012DA RID: 4826 RVA: 0x00048CCE File Offset: 0x00046ECE
		public EncyclopediaListSelectorItemVM(EncyclopediaListItemComparer comparer)
			: base(comparer.SortController.Name.ToString())
		{
			this.Comparer = comparer;
		}

		// Token: 0x040008BF RID: 2239
		public EncyclopediaListItemComparer Comparer;
	}
}
