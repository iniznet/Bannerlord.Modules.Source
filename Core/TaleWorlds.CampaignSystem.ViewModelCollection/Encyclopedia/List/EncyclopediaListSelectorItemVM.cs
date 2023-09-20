using System;
using TaleWorlds.Core.ViewModelCollection.Selector;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.List
{
	public class EncyclopediaListSelectorItemVM : SelectorItemVM
	{
		public EncyclopediaListSelectorItemVM(EncyclopediaListItemComparer comparer)
			: base(comparer.SortController.Name.ToString())
		{
			this.Comparer = comparer;
		}

		public EncyclopediaListItemComparer Comparer;
	}
}
