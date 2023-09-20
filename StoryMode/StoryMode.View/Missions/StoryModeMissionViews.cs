﻿using System;
using System.Collections.Generic;
using SandBox.View;
using SandBox.View.Missions;
using SandBox.View.Missions.Sound.Components;
using TaleWorlds.CampaignSystem;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.View.MissionViews.Sound;
using TaleWorlds.MountAndBlade.View.MissionViews.Sound.Components;

namespace StoryMode.View.Missions
{
	// Token: 0x02000005 RID: 5
	[ViewCreatorModule]
	public class StoryModeMissionViews
	{
		// Token: 0x06000008 RID: 8 RVA: 0x00002274 File Offset: 0x00000474
		[ViewMethod("TrainingField")]
		public static MissionView[] OpenVillageMission(Mission mission)
		{
			return new List<MissionView>
			{
				new MissionCampaignView(),
				new MissionConversationCameraView(),
				SandBoxViewCreator.CreateMissionConversationView(mission),
				ViewCreator.CreateMissionSingleplayerEscapeMenu(CampaignOptions.IsIronmanMode),
				ViewCreator.CreateOptionsUIHandler(),
				ViewCreator.CreateMissionMainAgentEquipDropView(mission),
				new MissionSingleplayerViewHandler(),
				ViewCreator.CreateMissionAgentStatusUIHandler(mission),
				ViewCreator.CreateMissionMainAgentEquipmentController(mission),
				ViewCreator.CreateMissionMainAgentCheerBarkControllerView(mission),
				ViewCreator.CreateMissionAgentLockVisualizerView(mission),
				new MusicMissionView(new MusicBaseComponent[]
				{
					new MusicMissionSettlementComponent(),
					new MusicMissionAlleyFightComponent()
				}),
				ViewCreator.CreateMissionBoundaryCrossingView(),
				SandBoxViewCreator.CreateMissionBarterView(),
				ViewCreator.CreateMissionLeaveView(),
				ViewCreator.CreatePhotoModeView(),
				new MissionBoundaryWallView(),
				SandBoxViewCreator.CreateMissionNameMarkerUIHandler(mission),
				new MissionItemContourControllerView(),
				new MissionAgentContourControllerView(),
				StoryModeViewCreator.CreateTrainingFieldObjectiveView(mission),
				new MissionCampaignBattleSpectatorView(),
				ViewCreator.CreateSingleplayerMissionKillNotificationUIHandler()
			}.ToArray();
		}
	}
}
