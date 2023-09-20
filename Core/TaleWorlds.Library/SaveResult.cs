using System;

namespace TaleWorlds.Library
{
	// Token: 0x02000080 RID: 128
	public enum SaveResult
	{
		// Token: 0x04000150 RID: 336
		Success,
		// Token: 0x04000151 RID: 337
		NoSpace,
		// Token: 0x04000152 RID: 338
		Corrupted,
		// Token: 0x04000153 RID: 339
		GeneralFailure,
		// Token: 0x04000154 RID: 340
		FileDriverFailure,
		// Token: 0x04000155 RID: 341
		PlatformFileHelperFailure,
		// Token: 0x04000156 RID: 342
		ConfigFileFailure,
		// Token: 0x04000157 RID: 343
		SaveLimitReached
	}
}
