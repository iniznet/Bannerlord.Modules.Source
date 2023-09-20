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
	public class MissionPreloadView : MissionView
	{
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

		public override void OnSceneRenderingStarted()
		{
			this._helperInstance.WaitForMeshesToBeLoaded();
		}

		public override void OnMissionStateDeactivated()
		{
			base.OnMissionStateDeactivated();
			this._helperInstance.Clear();
		}

		private readonly PreloadHelper _helperInstance = new PreloadHelper();

		private bool _preloadDone;
	}
}
