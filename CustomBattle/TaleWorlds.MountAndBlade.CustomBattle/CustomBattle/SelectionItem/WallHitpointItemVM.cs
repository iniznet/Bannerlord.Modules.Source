using System;
using TaleWorlds.Core.ViewModelCollection.Selector;

namespace TaleWorlds.MountAndBlade.CustomBattle.CustomBattle.SelectionItem
{
	// Token: 0x02000028 RID: 40
	public class WallHitpointItemVM : SelectorItemVM
	{
		// Token: 0x1700008C RID: 140
		// (get) Token: 0x060001C6 RID: 454 RVA: 0x0000A5C9 File Offset: 0x000087C9
		// (set) Token: 0x060001C7 RID: 455 RVA: 0x0000A5D1 File Offset: 0x000087D1
		public string WallState { get; private set; }

		// Token: 0x1700008D RID: 141
		// (get) Token: 0x060001C8 RID: 456 RVA: 0x0000A5DA File Offset: 0x000087DA
		// (set) Token: 0x060001C9 RID: 457 RVA: 0x0000A5E2 File Offset: 0x000087E2
		public int BreachedWallCount { get; private set; }

		// Token: 0x060001CA RID: 458 RVA: 0x0000A5EB File Offset: 0x000087EB
		public WallHitpointItemVM(string wallStateName, int breachedWallCount)
			: base(wallStateName)
		{
			this.WallState = wallStateName;
			this.BreachedWallCount = breachedWallCount;
		}
	}
}
