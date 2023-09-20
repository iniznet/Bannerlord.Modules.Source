using System;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020001D1 RID: 465
	[Flags]
	public enum BlowFlags
	{
		// Token: 0x04000848 RID: 2120
		None = 0,
		// Token: 0x04000849 RID: 2121
		KnockBack = 16,
		// Token: 0x0400084A RID: 2122
		KnockDown = 32,
		// Token: 0x0400084B RID: 2123
		NoSound = 64,
		// Token: 0x0400084C RID: 2124
		CrushThrough = 128,
		// Token: 0x0400084D RID: 2125
		ShrugOff = 256,
		// Token: 0x0400084E RID: 2126
		MakesRear = 512,
		// Token: 0x0400084F RID: 2127
		NonTipThrust = 1024,
		// Token: 0x04000850 RID: 2128
		CanDismount = 2048
	}
}
