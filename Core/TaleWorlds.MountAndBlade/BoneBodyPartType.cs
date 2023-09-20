using System;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020001C3 RID: 451
	public enum BoneBodyPartType : sbyte
	{
		// Token: 0x04000818 RID: 2072
		None = -1,
		// Token: 0x04000819 RID: 2073
		Head,
		// Token: 0x0400081A RID: 2074
		Neck,
		// Token: 0x0400081B RID: 2075
		Chest,
		// Token: 0x0400081C RID: 2076
		Abdomen,
		// Token: 0x0400081D RID: 2077
		ShoulderLeft,
		// Token: 0x0400081E RID: 2078
		ShoulderRight,
		// Token: 0x0400081F RID: 2079
		ArmLeft,
		// Token: 0x04000820 RID: 2080
		ArmRight,
		// Token: 0x04000821 RID: 2081
		Legs,
		// Token: 0x04000822 RID: 2082
		NumOfBodyPartTypes,
		// Token: 0x04000823 RID: 2083
		CriticalBodyPartsBegin = 0,
		// Token: 0x04000824 RID: 2084
		CriticalBodyPartsEnd = 6
	}
}
