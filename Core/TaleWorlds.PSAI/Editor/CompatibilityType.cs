using System;

namespace psai.Editor
{
	// Token: 0x02000005 RID: 5
	[Serializable]
	public enum CompatibilityType
	{
		// Token: 0x0400001D RID: 29
		undefined,
		// Token: 0x0400001E RID: 30
		allowed_implicitly,
		// Token: 0x0400001F RID: 31
		allowed_manually,
		// Token: 0x04000020 RID: 32
		blocked_implicitly,
		// Token: 0x04000021 RID: 33
		blocked_manually,
		// Token: 0x04000022 RID: 34
		logically_impossible
	}
}
