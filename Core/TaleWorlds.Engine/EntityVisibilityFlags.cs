using System;

namespace TaleWorlds.Engine
{
	// Token: 0x02000045 RID: 69
	[Flags]
	public enum EntityVisibilityFlags
	{
		// Token: 0x0400006B RID: 107
		None = 0,
		// Token: 0x0400006C RID: 108
		VisibleOnlyWhenEditing = 2,
		// Token: 0x0400006D RID: 109
		NoShadow = 4,
		// Token: 0x0400006E RID: 110
		VisibleOnlyForEnvmap = 8,
		// Token: 0x0400006F RID: 111
		NotVisibleForEnvmap = 16
	}
}
