using System;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000237 RID: 567
	[Flags]
	public enum GoldGainFlags : ushort
	{
		// Token: 0x04000B65 RID: 2917
		FirstRangedKill = 1,
		// Token: 0x04000B66 RID: 2918
		FirstMeleeKill = 2,
		// Token: 0x04000B67 RID: 2919
		FirstAssist = 4,
		// Token: 0x04000B68 RID: 2920
		SecondAssist = 8,
		// Token: 0x04000B69 RID: 2921
		ThirdAssist = 16,
		// Token: 0x04000B6A RID: 2922
		FifthKill = 32,
		// Token: 0x04000B6B RID: 2923
		TenthKill = 64,
		// Token: 0x04000B6C RID: 2924
		DefaultKill = 128,
		// Token: 0x04000B6D RID: 2925
		DefaultAssist = 256,
		// Token: 0x04000B6E RID: 2926
		ObjectiveCompleted = 512,
		// Token: 0x04000B6F RID: 2927
		ObjectiveDestroyed = 1024,
		// Token: 0x04000B70 RID: 2928
		PerkBonus = 2048
	}
}
