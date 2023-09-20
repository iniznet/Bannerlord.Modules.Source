using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;

namespace SandBox.View.Missions
{
	// Token: 0x0200001B RID: 27
	public class MissionPreloadView : MissionView
	{
		// Token: 0x060000A4 RID: 164 RVA: 0x0000914C File Offset: 0x0000734C
		public override void OnPreMissionTick(float dt)
		{
			if (!this._preloadDone)
			{
				List<BasicCharacterObject> list = new List<BasicCharacterObject>();
				foreach (PartyBase partyBase in MapEvent.PlayerMapEvent.InvolvedParties)
				{
					foreach (TroopRosterElement troopRosterElement in partyBase.MemberRoster.GetTroopRoster())
					{
						for (int i = 0; i < troopRosterElement.Number; i++)
						{
							list.Add(troopRosterElement.Character);
						}
					}
				}
				this._helperInstance.PreloadCharacters(list);
				SiegeDeploymentMissionController missionBehavior = base.Mission.GetMissionBehavior<SiegeDeploymentMissionController>();
				if (missionBehavior != null)
				{
					this._helperInstance.PreloadItems(missionBehavior.GetSiegeMissiles());
				}
				this._preloadDone = true;
			}
		}

		// Token: 0x060000A5 RID: 165 RVA: 0x0000923C File Offset: 0x0000743C
		public override void OnSceneRenderingStarted()
		{
			this._helperInstance.WaitForMeshesToBeLoaded();
		}

		// Token: 0x060000A6 RID: 166 RVA: 0x00009249 File Offset: 0x00007449
		public override void OnMissionStateDeactivated()
		{
			base.OnMissionStateDeactivated();
			this._helperInstance.Clear();
		}

		// Token: 0x04000075 RID: 117
		private readonly PreloadHelper _helperInstance = new PreloadHelper();

		// Token: 0x04000076 RID: 118
		private bool _preloadDone;
	}
}
