using System;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000250 RID: 592
	public interface IFlagRemoved : IMissionBehavior
	{
		// Token: 0x06001FF8 RID: 8184
		void OnFlagsRemoved(int remainingFlagIndex);
	}
}
