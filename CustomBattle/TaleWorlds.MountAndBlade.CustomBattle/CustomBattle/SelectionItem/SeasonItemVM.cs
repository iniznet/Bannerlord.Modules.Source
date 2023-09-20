using System;
using TaleWorlds.Core.ViewModelCollection.Selector;

namespace TaleWorlds.MountAndBlade.CustomBattle.CustomBattle.SelectionItem
{
	// Token: 0x02000026 RID: 38
	public class SeasonItemVM : SelectorItemVM
	{
		// Token: 0x1700008A RID: 138
		// (get) Token: 0x060001C0 RID: 448 RVA: 0x0000A587 File Offset: 0x00008787
		// (set) Token: 0x060001C1 RID: 449 RVA: 0x0000A58F File Offset: 0x0000878F
		public string SeasonId { get; private set; }

		// Token: 0x060001C2 RID: 450 RVA: 0x0000A598 File Offset: 0x00008798
		public SeasonItemVM(string seasonName, string seasonId)
			: base(seasonName)
		{
			this.SeasonId = seasonId;
		}
	}
}
