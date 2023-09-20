using System;

namespace TaleWorlds.GauntletUI.GamepadNavigation
{
	// Token: 0x02000045 RID: 69
	[Flags]
	public enum GamepadNavigationTypes
	{
		// Token: 0x04000236 RID: 566
		None = 0,
		// Token: 0x04000237 RID: 567
		Up = 1,
		// Token: 0x04000238 RID: 568
		Down = 2,
		// Token: 0x04000239 RID: 569
		Vertical = 3,
		// Token: 0x0400023A RID: 570
		Left = 4,
		// Token: 0x0400023B RID: 571
		Right = 8,
		// Token: 0x0400023C RID: 572
		Horizontal = 12
	}
}
