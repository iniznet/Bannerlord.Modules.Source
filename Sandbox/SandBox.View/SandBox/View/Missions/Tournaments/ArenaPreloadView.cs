using System;
using System.Collections.Generic;
using SandBox.Missions.MissionLogics.Arena;
using SandBox.Tournaments.MissionLogics;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;

namespace SandBox.View.Missions.Tournaments
{
	internal class ArenaPreloadView : MissionView
	{
		public override void OnPreMissionTick(float dt)
		{
			if (!this._preloadDone)
			{
				List<BasicCharacterObject> list = new List<BasicCharacterObject>();
				if (Mission.Current.GetMissionBehavior<ArenaPracticeFightMissionController>() != null)
				{
					foreach (CharacterObject characterObject in ArenaPracticeFightMissionController.GetParticipantCharacters(Settlement.CurrentSettlement))
					{
						list.Add(characterObject);
					}
					list.Add(CharacterObject.PlayerCharacter);
				}
				TournamentBehavior missionBehavior = Mission.Current.GetMissionBehavior<TournamentBehavior>();
				if (missionBehavior != null)
				{
					foreach (CharacterObject characterObject2 in missionBehavior.GetAllPossibleParticipants())
					{
						list.Add(characterObject2);
					}
				}
				this._helperInstance.PreloadCharacters(list);
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
