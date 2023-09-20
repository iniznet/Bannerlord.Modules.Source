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
	// Token: 0x02000021 RID: 33
	internal class ArenaPreloadView : MissionView
	{
		// Token: 0x060000DF RID: 223 RVA: 0x0000BDC8 File Offset: 0x00009FC8
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

		// Token: 0x060000E0 RID: 224 RVA: 0x0000BEAC File Offset: 0x0000A0AC
		public override void OnSceneRenderingStarted()
		{
			this._helperInstance.WaitForMeshesToBeLoaded();
		}

		// Token: 0x060000E1 RID: 225 RVA: 0x0000BEB9 File Offset: 0x0000A0B9
		public override void OnMissionStateDeactivated()
		{
			base.OnMissionStateDeactivated();
			this._helperInstance.Clear();
		}

		// Token: 0x0400007A RID: 122
		private readonly PreloadHelper _helperInstance = new PreloadHelper();

		// Token: 0x0400007B RID: 123
		private bool _preloadDone;
	}
}
