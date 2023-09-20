using System;
using System.Collections.Generic;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade.ComponentInterfaces
{
	public abstract class BattleInitializationModel : GameModel
	{
		public abstract List<FormationClass> GetAllAvailableTroopTypes();

		protected abstract bool CanPlayerSideDeployWithOrderOfBattleAux();

		public bool CanPlayerSideDeployWithOrderOfBattle()
		{
			if (!BattleInitializationModel._isCanPlayerSideDeployWithOOBCached)
			{
				BattleInitializationModel._cachedCanPlayerSideDeployWithOOB = this.CanPlayerSideDeployWithOrderOfBattleAux();
				BattleInitializationModel._isCanPlayerSideDeployWithOOBCached = true;
			}
			return BattleInitializationModel._cachedCanPlayerSideDeployWithOOB;
		}

		public void InitializeModel()
		{
			BattleInitializationModel._isCanPlayerSideDeployWithOOBCached = false;
			BattleInitializationModel._isInitialized = true;
		}

		public void FinalizeModel()
		{
			BattleInitializationModel._isInitialized = false;
		}

		public const int MinimumTroopCountForPlayerDeployment = 20;

		private static bool _cachedCanPlayerSideDeployWithOOB;

		private static bool _isCanPlayerSideDeployWithOOBCached;

		private static bool _isInitialized;
	}
}
