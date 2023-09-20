using System;
using System.Collections.Generic;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x02000114 RID: 276
	public interface IBadgeComponent
	{
		// Token: 0x17000203 RID: 515
		// (get) Token: 0x06000541 RID: 1345
		Dictionary<ValueTuple<PlayerId, string, string>, int> DataDictionary { get; }

		// Token: 0x06000542 RID: 1346
		void OnPlayerJoin(PlayerData playerData);

		// Token: 0x06000543 RID: 1347
		void OnStartingNextBattle();
	}
}
