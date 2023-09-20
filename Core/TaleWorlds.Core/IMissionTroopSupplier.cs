using System;
using System.Collections.Generic;

namespace TaleWorlds.Core
{
	public interface IMissionTroopSupplier
	{
		IEnumerable<IAgentOriginBase> SupplyTroops(int numberToAllocate);

		IEnumerable<IAgentOriginBase> GetAllTroops();

		BasicCharacterObject GetGeneralCharacter();

		int NumRemovedTroops { get; }

		int NumTroopsNotSupplied { get; }

		bool AnyTroopRemainsToBeSupplied { get; }

		int GetNumberOfPlayerControllableTroops();
	}
}
