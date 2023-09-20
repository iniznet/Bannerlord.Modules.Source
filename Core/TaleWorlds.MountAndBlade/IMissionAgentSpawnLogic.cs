using System;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200024C RID: 588
	public interface IMissionAgentSpawnLogic : IMissionBehavior
	{
		// Token: 0x06001FEB RID: 8171
		void StartSpawner(BattleSideEnum side);

		// Token: 0x06001FEC RID: 8172
		void StopSpawner(BattleSideEnum side);

		// Token: 0x06001FED RID: 8173
		bool IsSideSpawnEnabled(BattleSideEnum side);

		// Token: 0x06001FEE RID: 8174
		bool IsSideDepleted(BattleSideEnum side);

		// Token: 0x06001FEF RID: 8175
		float GetReinforcementInterval();
	}
}
