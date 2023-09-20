using System;

namespace TaleWorlds.Core
{
	// Token: 0x02000081 RID: 129
	public interface IFaceGeneratorCustomFilter
	{
		// Token: 0x060007A5 RID: 1957
		int[] GetHaircutIndices(BasicCharacterObject character);

		// Token: 0x060007A6 RID: 1958
		int[] GetFacialHairIndices(BasicCharacterObject character);

		// Token: 0x060007A7 RID: 1959
		FaceGeneratorStage[] GetAvailableStages();
	}
}
