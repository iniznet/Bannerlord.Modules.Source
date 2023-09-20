using System;
using TaleWorlds.Core.ViewModelCollection.Selector;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting
{
	// Token: 0x020000D9 RID: 217
	public class CraftingSecondaryUsageItemVM : SelectorItemVM
	{
		// Token: 0x170006BC RID: 1724
		// (get) Token: 0x0600140E RID: 5134 RVA: 0x0004C22D File Offset: 0x0004A42D
		public int UsageIndex { get; }

		// Token: 0x170006BD RID: 1725
		// (get) Token: 0x0600140F RID: 5135 RVA: 0x0004C235 File Offset: 0x0004A435
		public int SelectorIndex { get; }

		// Token: 0x06001410 RID: 5136 RVA: 0x0004C23D File Offset: 0x0004A43D
		public CraftingSecondaryUsageItemVM(TextObject name, int index, int usageIndex, SelectorVM<CraftingSecondaryUsageItemVM> parentSelector)
			: base(name)
		{
			this._parentSelector = parentSelector;
			this.SelectorIndex = index;
			this.UsageIndex = usageIndex;
		}

		// Token: 0x06001411 RID: 5137 RVA: 0x0004C25C File Offset: 0x0004A45C
		public void ExecuteSelect()
		{
			this._parentSelector.SelectedIndex = this.SelectorIndex;
		}

		// Token: 0x0400095B RID: 2395
		private SelectorVM<CraftingSecondaryUsageItemVM> _parentSelector;
	}
}
