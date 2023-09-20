using System;
using System.Collections.Generic;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade.View.MissionViews.Singleplayer
{
	public class MissionCustomBattlePreloadView : MissionView
	{
		public override void OnPreMissionTick(float dt)
		{
			if (!this._preloadDone)
			{
				MissionCombatantsLogic missionBehavior = base.Mission.GetMissionBehavior<MissionCombatantsLogic>();
				List<BasicCharacterObject> list = new List<BasicCharacterObject>();
				foreach (IBattleCombatant battleCombatant in missionBehavior.GetAllCombatants())
				{
					list.AddRange(((CustomBattleCombatant)battleCombatant).Characters);
				}
				this._helperInstance.PreloadCharacters(list);
				SiegeDeploymentMissionController missionBehavior2 = Mission.Current.GetMissionBehavior<SiegeDeploymentMissionController>();
				if (missionBehavior2 != null)
				{
					this._helperInstance.PreloadItems(missionBehavior2.GetSiegeMissiles());
				}
				this._preloadDone = true;
			}
		}

		public override void OnSceneRenderingStarted()
		{
			this._helperInstance.WaitForMeshesToBeLoaded();
		}

		public override void OnMissionStateDeactivated()
		{
			base.OnMissionStateDeactivated();
			this._helperInstance.Clear();
		}

		private PreloadHelper _helperInstance = new PreloadHelper();

		private bool _preloadDone;
	}
}
