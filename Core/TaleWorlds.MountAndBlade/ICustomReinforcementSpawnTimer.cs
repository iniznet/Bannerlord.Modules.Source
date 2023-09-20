using System;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200026D RID: 621
	public interface ICustomReinforcementSpawnTimer
	{
		// Token: 0x06002127 RID: 8487
		bool Check(BattleSideEnum side);

		// Token: 0x06002128 RID: 8488
		void ResetTimer(BattleSideEnum side);
	}
}
