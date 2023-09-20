using System;
using System.Collections.Generic;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade.ComponentInterfaces
{
	public abstract class BattleInitializationModel : GameModel
	{
		public bool BypassPlayerDeployment { get; private set; }

		public abstract List<FormationClass> GetAllAvailableTroopTypes();

		protected abstract bool CanPlayerSideDeployWithOrderOfBattleAux();

		public bool CanPlayerSideDeployWithOrderOfBattle()
		{
			if (!this._isCanPlayerSideDeployWithOOBCached)
			{
				this._cachedCanPlayerSideDeployWithOOB = !this.BypassPlayerDeployment && this.CanPlayerSideDeployWithOrderOfBattleAux();
				this._isCanPlayerSideDeployWithOOBCached = true;
			}
			return this._cachedCanPlayerSideDeployWithOOB;
		}

		public void InitializeModel()
		{
			this._isCanPlayerSideDeployWithOOBCached = false;
			this._isInitialized = true;
		}

		public void FinalizeModel()
		{
			this._isInitialized = false;
		}

		public void SetBypassPlayerDeployment(bool value)
		{
			this.BypassPlayerDeployment = value;
		}

		public const int MinimumTroopCountForPlayerDeployment = 20;

		private bool _cachedCanPlayerSideDeployWithOOB;

		private bool _isCanPlayerSideDeployWithOOBCached;

		private bool _isInitialized;
	}
}
