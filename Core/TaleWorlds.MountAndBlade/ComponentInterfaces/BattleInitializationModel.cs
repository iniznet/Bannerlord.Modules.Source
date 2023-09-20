using System;
using System.Collections.Generic;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade.ComponentInterfaces
{
	// Token: 0x02000409 RID: 1033
	public abstract class BattleInitializationModel : GameModel
	{
		// Token: 0x0600356A RID: 13674
		public abstract List<FormationClass> GetAllAvailableTroopTypes();

		// Token: 0x0600356B RID: 13675
		protected abstract bool CanPlayerSideDeployWithOrderOfBattleAux();

		// Token: 0x0600356C RID: 13676 RVA: 0x000DE3BF File Offset: 0x000DC5BF
		public bool CanPlayerSideDeployWithOrderOfBattle()
		{
			if (!BattleInitializationModel._isCanPlayerSideDeployWithOOBCached)
			{
				BattleInitializationModel._cachedCanPlayerSideDeployWithOOB = this.CanPlayerSideDeployWithOrderOfBattleAux();
				BattleInitializationModel._isCanPlayerSideDeployWithOOBCached = true;
			}
			return BattleInitializationModel._cachedCanPlayerSideDeployWithOOB;
		}

		// Token: 0x0600356D RID: 13677 RVA: 0x000DE3DE File Offset: 0x000DC5DE
		public void InitializeModel()
		{
			BattleInitializationModel._isCanPlayerSideDeployWithOOBCached = false;
			BattleInitializationModel._isInitialized = true;
		}

		// Token: 0x0600356E RID: 13678 RVA: 0x000DE3EC File Offset: 0x000DC5EC
		public void FinalizeModel()
		{
			BattleInitializationModel._isInitialized = false;
		}

		// Token: 0x040016E6 RID: 5862
		public const int MinimumTroopCountForPlayerDeployment = 20;

		// Token: 0x040016E7 RID: 5863
		private static bool _cachedCanPlayerSideDeployWithOOB;

		// Token: 0x040016E8 RID: 5864
		private static bool _isCanPlayerSideDeployWithOOBCached;

		// Token: 0x040016E9 RID: 5865
		private static bool _isInitialized;
	}
}
