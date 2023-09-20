using System;

namespace TaleWorlds.Engine
{
	// Token: 0x02000071 RID: 113
	[Flags]
	public enum PhysicsMaterialFlags : byte
	{
		// Token: 0x0400014B RID: 331
		None = 0,
		// Token: 0x0400014C RID: 332
		DontStickMissiles = 1,
		// Token: 0x0400014D RID: 333
		Flammable = 2,
		// Token: 0x0400014E RID: 334
		RainSplashesEnabled = 4,
		// Token: 0x0400014F RID: 335
		AttacksCanPassThrough = 8
	}
}
