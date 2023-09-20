using System;
using TaleWorlds.Core.ViewModelCollection.Selector;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting
{
	public class CraftingSecondaryUsageItemVM : SelectorItemVM
	{
		public int UsageIndex { get; }

		public int SelectorIndex { get; }

		public CraftingSecondaryUsageItemVM(TextObject name, int index, int usageIndex, SelectorVM<CraftingSecondaryUsageItemVM> parentSelector)
			: base(name)
		{
			this._parentSelector = parentSelector;
			this.SelectorIndex = index;
			this.UsageIndex = usageIndex;
		}

		public void ExecuteSelect()
		{
			this._parentSelector.SelectedIndex = this.SelectorIndex;
		}

		private SelectorVM<CraftingSecondaryUsageItemVM> _parentSelector;
	}
}
