using System;
using System.Collections.Generic;
using SandBox.View.Missions.Sound.Components;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.View.MissionViews.Singleplayer;
using TaleWorlds.MountAndBlade.View.MissionViews.Sound;
using TaleWorlds.MountAndBlade.View.MissionViews.Sound.Components;

namespace SandBox.View.Missions.Tournaments
{
	// Token: 0x02000024 RID: 36
	[ViewCreatorModule]
	public class TournamentMissionViews
	{
		// Token: 0x060000F2 RID: 242 RVA: 0x0000C36C File Offset: 0x0000A56C
		[ViewMethod("TournamentArchery")]
		public static MissionView[] OpenTournamentArcheryMission(Mission mission)
		{
			return new List<MissionView>
			{
				new MissionCampaignView(),
				new MissionConversationCameraView(),
				ViewCreator.CreateMissionSingleplayerEscapeMenu(CampaignOptions.IsIronmanMode),
				ViewCreator.CreateOptionsUIHandler(),
				ViewCreator.CreateMissionMainAgentEquipDropView(mission),
				ViewCreator.CreateMissionAgentStatusUIHandler(mission),
				ViewCreator.CreateMissionMainAgentEquipmentController(mission),
				ViewCreator.CreateMissionMainAgentCheerBarkControllerView(mission),
				ViewCreator.CreateMissionAgentLockVisualizerView(mission),
				ViewCreator.CreateMissionSpectatorControlView(mission),
				SandBoxViewCreator.CreateMissionTournamentView(),
				new MissionAudienceHandler(0.4f + MBRandom.RandomFloat * 0.6f),
				new MissionSingleplayerViewHandler(),
				new MissionMessageUIHandler(),
				ViewCreator.CreateSingleplayerMissionKillNotificationUIHandler(),
				new MusicMissionView(new MusicBaseComponent[]
				{
					new MusicMissionTournamentComponent()
				}),
				ViewCreator.CreateMissionAgentLabelUIHandler(mission),
				new MissionItemContourControllerView(),
				ViewCreator.CreatePhotoModeView(),
				new ArenaPreloadView()
			}.ToArray();
		}

		// Token: 0x060000F3 RID: 243 RVA: 0x0000C48C File Offset: 0x0000A68C
		[ViewMethod("TournamentFight")]
		public static MissionView[] OpenTournamentFightMission(Mission mission)
		{
			return new List<MissionView>
			{
				new MissionCampaignView(),
				new MissionConversationCameraView(),
				ViewCreator.CreateMissionSingleplayerEscapeMenu(CampaignOptions.IsIronmanMode),
				ViewCreator.CreateOptionsUIHandler(),
				ViewCreator.CreateMissionMainAgentEquipDropView(mission),
				SandBoxViewCreator.CreateMissionTournamentView(),
				new MissionAudienceHandler(0.4f + MBRandom.RandomFloat * 0.6f),
				ViewCreator.CreateMissionAgentStatusUIHandler(mission),
				ViewCreator.CreateMissionMainAgentEquipmentController(mission),
				ViewCreator.CreateMissionMainAgentCheerBarkControllerView(mission),
				ViewCreator.CreateMissionAgentLockVisualizerView(mission),
				ViewCreator.CreateMissionSpectatorControlView(mission),
				new MusicTournamentMissionView(),
				new MissionSingleplayerViewHandler(),
				ViewCreator.CreateSingleplayerMissionKillNotificationUIHandler(),
				new MusicMissionView(new MusicBaseComponent[]
				{
					new MusicMissionTournamentComponent()
				}),
				ViewCreator.CreateMissionAgentLabelUIHandler(mission),
				new MissionItemContourControllerView(),
				new MissionCampaignBattleSpectatorView(),
				ViewCreator.CreatePhotoModeView(),
				new ArenaPreloadView()
			}.ToArray();
		}

		// Token: 0x060000F4 RID: 244 RVA: 0x0000C5B8 File Offset: 0x0000A7B8
		[ViewMethod("TournamentHorseRace")]
		public static MissionView[] OpenTournamentHorseRaceMission(Mission mission)
		{
			return new List<MissionView>
			{
				new MissionCampaignView(),
				new MissionConversationCameraView(),
				ViewCreator.CreateMissionSingleplayerEscapeMenu(CampaignOptions.IsIronmanMode),
				ViewCreator.CreateOptionsUIHandler(),
				ViewCreator.CreateMissionMainAgentEquipDropView(mission),
				ViewCreator.CreateMissionAgentStatusUIHandler(mission),
				ViewCreator.CreateMissionMainAgentEquipmentController(mission),
				ViewCreator.CreateMissionMainAgentCheerBarkControllerView(mission),
				ViewCreator.CreateMissionAgentLockVisualizerView(mission),
				ViewCreator.CreateMissionSpectatorControlView(mission),
				new MissionTournamentView(),
				new MissionSingleplayerViewHandler(),
				new MissionAudienceHandler(0.4f + MBRandom.RandomFloat * 0.6f),
				ViewCreator.CreateSingleplayerMissionKillNotificationUIHandler(),
				new MusicMissionView(new MusicBaseComponent[]
				{
					new MusicMissionTournamentComponent()
				}),
				ViewCreator.CreateMissionAgentLabelUIHandler(mission),
				new MissionItemContourControllerView(),
				new MissionCampaignBattleSpectatorView(),
				ViewCreator.CreatePhotoModeView(),
				new ArenaPreloadView()
			}.ToArray();
		}

		// Token: 0x060000F5 RID: 245 RVA: 0x0000C6D8 File Offset: 0x0000A8D8
		[ViewMethod("TournamentJousting")]
		public static MissionView[] OpenTournamentJoustingMission(Mission mission)
		{
			return new List<MissionView>
			{
				new MissionCampaignView(),
				new MissionConversationCameraView(),
				ViewCreator.CreateMissionSingleplayerEscapeMenu(CampaignOptions.IsIronmanMode),
				ViewCreator.CreateOptionsUIHandler(),
				ViewCreator.CreateMissionMainAgentEquipDropView(mission),
				ViewCreator.CreateMissionAgentStatusUIHandler(mission),
				ViewCreator.CreateMissionMainAgentEquipmentController(mission),
				ViewCreator.CreateMissionMainAgentCheerBarkControllerView(mission),
				ViewCreator.CreateMissionAgentLockVisualizerView(mission),
				ViewCreator.CreateMissionSpectatorControlView(mission),
				SandBoxViewCreator.CreateMissionTournamentView(),
				new MissionAudienceHandler(0.4f + MBRandom.RandomFloat * 0.6f),
				new MissionSingleplayerViewHandler(),
				new MissionMessageUIHandler(),
				new MissionScoreUIHandler(),
				new MusicMissionView(new MusicBaseComponent[]
				{
					new MusicMissionTournamentComponent()
				}),
				ViewCreator.CreateSingleplayerMissionKillNotificationUIHandler(),
				new MissionTournamentJoustingView(),
				ViewCreator.CreateMissionAgentLabelUIHandler(mission),
				new MissionItemContourControllerView(),
				new MissionCampaignBattleSpectatorView(),
				ViewCreator.CreatePhotoModeView(),
				new ArenaPreloadView()
			}.ToArray();
		}
	}
}
