using System;
using TaleWorlds.Core.ViewModelCollection.Selector;

namespace TaleWorlds.MountAndBlade.CustomBattle.CustomBattle.SelectionItem
{
	// Token: 0x02000025 RID: 37
	public class SceneLevelItemVM : SelectorItemVM
	{
		// Token: 0x17000089 RID: 137
		// (get) Token: 0x060001BD RID: 445 RVA: 0x0000A560 File Offset: 0x00008760
		// (set) Token: 0x060001BE RID: 446 RVA: 0x0000A568 File Offset: 0x00008768
		public int Level { get; private set; }

		// Token: 0x060001BF RID: 447 RVA: 0x0000A571 File Offset: 0x00008771
		public SceneLevelItemVM(int level)
			: base(level.ToString())
		{
			this.Level = level;
		}
	}
}
