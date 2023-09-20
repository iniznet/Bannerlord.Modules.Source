using System;
using TaleWorlds.Core.ViewModelCollection.Selector;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.List
{
	public class EncyclopediaListSelectorVM : SelectorVM<EncyclopediaListSelectorItemVM>
	{
		public EncyclopediaListSelectorVM(int selectedIndex, Action<SelectorVM<EncyclopediaListSelectorItemVM>> onChange, Action onActivate)
			: base(selectedIndex, onChange)
		{
			this._onActivate = onActivate;
		}

		public void ExecuteOnDropdownActivated()
		{
			Action onActivate = this._onActivate;
			if (onActivate == null)
			{
				return;
			}
			onActivate();
		}

		private Action _onActivate;
	}
}
