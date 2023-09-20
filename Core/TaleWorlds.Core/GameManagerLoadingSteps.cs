using System;

namespace TaleWorlds.Core
{
	// Token: 0x02000065 RID: 101
	public enum GameManagerLoadingSteps
	{
		// Token: 0x04000395 RID: 917
		None = -1,
		// Token: 0x04000396 RID: 918
		PreInitializeZerothStep,
		// Token: 0x04000397 RID: 919
		FirstInitializeFirstStep,
		// Token: 0x04000398 RID: 920
		WaitSecondStep,
		// Token: 0x04000399 RID: 921
		SecondInitializeThirdState,
		// Token: 0x0400039A RID: 922
		PostInitializeFourthState,
		// Token: 0x0400039B RID: 923
		FinishLoadingFifthStep,
		// Token: 0x0400039C RID: 924
		LoadingIsOver
	}
}
