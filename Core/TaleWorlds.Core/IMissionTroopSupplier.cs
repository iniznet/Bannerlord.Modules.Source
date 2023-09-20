using System;
using System.Collections.Generic;

namespace TaleWorlds.Core
{
	// Token: 0x020000B0 RID: 176
	public interface IMissionTroopSupplier
	{
		// Token: 0x0600087A RID: 2170
		IEnumerable<IAgentOriginBase> SupplyTroops(int numberToAllocate);

		// Token: 0x0600087B RID: 2171
		IEnumerable<IAgentOriginBase> GetAllTroops();

		// Token: 0x0600087C RID: 2172
		BasicCharacterObject GetGeneralCharacter();

		// Token: 0x170002BC RID: 700
		// (get) Token: 0x0600087D RID: 2173
		int NumRemovedTroops { get; }

		// Token: 0x170002BD RID: 701
		// (get) Token: 0x0600087E RID: 2174
		int NumTroopsNotSupplied { get; }

		// Token: 0x170002BE RID: 702
		// (get) Token: 0x0600087F RID: 2175
		bool AnyTroopRemainsToBeSupplied { get; }

		// Token: 0x06000880 RID: 2176
		int GetNumberOfPlayerControllableTroops();
	}
}
