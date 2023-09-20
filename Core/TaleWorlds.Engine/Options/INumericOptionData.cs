using System;

namespace TaleWorlds.Engine.Options
{
	// Token: 0x0200009B RID: 155
	public interface INumericOptionData : IOptionData
	{
		// Token: 0x06000BA9 RID: 2985
		float GetMinValue();

		// Token: 0x06000BAA RID: 2986
		float GetMaxValue();

		// Token: 0x06000BAB RID: 2987
		bool GetIsDiscrete();

		// Token: 0x06000BAC RID: 2988
		int GetDiscreteIncrementInterval();

		// Token: 0x06000BAD RID: 2989
		bool GetShouldUpdateContinuously();
	}
}
