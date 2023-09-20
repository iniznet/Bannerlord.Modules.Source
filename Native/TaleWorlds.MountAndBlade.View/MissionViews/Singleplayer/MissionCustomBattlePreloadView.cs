using System;
using System.Collections.Generic;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade.View.MissionViews.Singleplayer
{
	// Token: 0x02000069 RID: 105
	public class MissionCustomBattlePreloadView : MissionView
	{
		// Token: 0x06000449 RID: 1097 RVA: 0x00022088 File Offset: 0x00020288
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

		// Token: 0x0600044A RID: 1098 RVA: 0x0002212C File Offset: 0x0002032C
		public override void OnSceneRenderingStarted()
		{
			this._helperInstance.WaitForMeshesToBeLoaded();
		}

		// Token: 0x0600044B RID: 1099 RVA: 0x00022139 File Offset: 0x00020339
		public override void OnMissionStateDeactivated()
		{
			base.OnMissionStateDeactivated();
			this._helperInstance.Clear();
		}

		// Token: 0x040002B0 RID: 688
		private PreloadHelper _helperInstance = new PreloadHelper();

		// Token: 0x040002B1 RID: 689
		private bool _preloadDone;
	}
}
