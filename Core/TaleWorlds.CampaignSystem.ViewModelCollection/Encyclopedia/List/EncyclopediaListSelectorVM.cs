using System;
using TaleWorlds.Core.ViewModelCollection.Selector;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.List
{
	// Token: 0x020000C0 RID: 192
	public class EncyclopediaListSelectorVM : SelectorVM<EncyclopediaListSelectorItemVM>
	{
		// Token: 0x060012D8 RID: 4824 RVA: 0x00048CAB File Offset: 0x00046EAB
		public EncyclopediaListSelectorVM(int selectedIndex, Action<SelectorVM<EncyclopediaListSelectorItemVM>> onChange, Action onActivate)
			: base(selectedIndex, onChange)
		{
			this._onActivate = onActivate;
		}

		// Token: 0x060012D9 RID: 4825 RVA: 0x00048CBC File Offset: 0x00046EBC
		public void ExecuteOnDropdownActivated()
		{
			Action onActivate = this._onActivate;
			if (onActivate == null)
			{
				return;
			}
			onActivate();
		}

		// Token: 0x040008BE RID: 2238
		private Action _onActivate;
	}
}
