using System;
using System.Collections.Generic;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.Diamond
{
	public interface IBadgeComponent
	{
		Dictionary<ValueTuple<PlayerId, string, string>, int> DataDictionary { get; }

		void OnPlayerJoin(PlayerData playerData);

		void OnStartingNextBattle();
	}
}
