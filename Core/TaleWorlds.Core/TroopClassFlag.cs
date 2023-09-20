using System;

namespace TaleWorlds.Core
{
	// Token: 0x0200005D RID: 93
	[Flags]
	public enum TroopClassFlag : uint
	{
		// Token: 0x04000359 RID: 857
		None = 0U,
		// Token: 0x0400035A RID: 858
		OneHandedUser = 1U,
		// Token: 0x0400035B RID: 859
		ShieldUser = 2U,
		// Token: 0x0400035C RID: 860
		TwoHandedUser = 4U,
		// Token: 0x0400035D RID: 861
		PoleArmUser = 8U,
		// Token: 0x0400035E RID: 862
		BowUser = 16U,
		// Token: 0x0400035F RID: 863
		ThrownUser = 32U,
		// Token: 0x04000360 RID: 864
		CrossbowUser = 64U,
		// Token: 0x04000361 RID: 865
		Infantry = 256U,
		// Token: 0x04000362 RID: 866
		Cavalry = 512U,
		// Token: 0x04000363 RID: 867
		Ranged = 4096U,
		// Token: 0x04000364 RID: 868
		All = 4294967295U
	}
}
