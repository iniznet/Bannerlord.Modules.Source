using System;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000167 RID: 359
	public interface IQueryData
	{
		// Token: 0x06001251 RID: 4689
		void Expire();

		// Token: 0x06001252 RID: 4690
		void Evaluate(float currentTime);

		// Token: 0x06001253 RID: 4691
		void SetSyncGroup(IQueryData[] syncGroup);
	}
}
