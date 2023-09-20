using System;

namespace TaleWorlds.Core
{
	// Token: 0x020000BE RID: 190
	[Flags]
	public enum SkinMask
	{
		// Token: 0x04000576 RID: 1398
		NoneVisible = 0,
		// Token: 0x04000577 RID: 1399
		HeadVisible = 1,
		// Token: 0x04000578 RID: 1400
		BodyVisible = 32,
		// Token: 0x04000579 RID: 1401
		UnderwearVisible = 64,
		// Token: 0x0400057A RID: 1402
		HandsVisible = 128,
		// Token: 0x0400057B RID: 1403
		LegsVisible = 256,
		// Token: 0x0400057C RID: 1404
		AllVisible = 481
	}
}
