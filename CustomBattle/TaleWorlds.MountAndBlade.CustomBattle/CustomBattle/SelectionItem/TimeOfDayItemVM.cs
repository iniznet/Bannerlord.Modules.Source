using System;
using TaleWorlds.Core.ViewModelCollection.Selector;

namespace TaleWorlds.MountAndBlade.CustomBattle.CustomBattle.SelectionItem
{
	// Token: 0x02000027 RID: 39
	public class TimeOfDayItemVM : SelectorItemVM
	{
		// Token: 0x1700008B RID: 139
		// (get) Token: 0x060001C3 RID: 451 RVA: 0x0000A5A8 File Offset: 0x000087A8
		// (set) Token: 0x060001C4 RID: 452 RVA: 0x0000A5B0 File Offset: 0x000087B0
		public int TimeOfDay { get; private set; }

		// Token: 0x060001C5 RID: 453 RVA: 0x0000A5B9 File Offset: 0x000087B9
		public TimeOfDayItemVM(string timeOfDayName, int timeOfDay)
			: base(timeOfDayName)
		{
			this.TimeOfDay = timeOfDay;
		}
	}
}
