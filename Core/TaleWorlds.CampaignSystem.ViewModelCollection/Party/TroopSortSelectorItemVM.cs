using System;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core.ViewModelCollection.Selector;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Party
{
	// Token: 0x02000026 RID: 38
	public class TroopSortSelectorItemVM : SelectorItemVM
	{
		// Token: 0x170000CB RID: 203
		// (get) Token: 0x060002F0 RID: 752 RVA: 0x000131FC File Offset: 0x000113FC
		// (set) Token: 0x060002F1 RID: 753 RVA: 0x00013204 File Offset: 0x00011404
		public PartyScreenLogic.TroopSortType SortType { get; private set; }

		// Token: 0x060002F2 RID: 754 RVA: 0x0001320D File Offset: 0x0001140D
		public TroopSortSelectorItemVM(TextObject s, PartyScreenLogic.TroopSortType sortType)
			: base(s)
		{
			this.SortType = sortType;
		}
	}
}
