using System;
using System.Collections.Generic;
using SandBox.Missions.AgentControllers;
using SandBox.View.Missions.Sound.Components;
using SandBox.View.Missions.Tournaments;
using SandBox.ViewModelCollection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Missions.Handlers;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.View.MissionViews.Order;
using TaleWorlds.MountAndBlade.View.MissionViews.Singleplayer;
using TaleWorlds.MountAndBlade.View.MissionViews.Sound;
using TaleWorlds.MountAndBlade.View.MissionViews.Sound.Components;

namespace SandBox.View.Missions
{
	[ViewCreatorModule]
	public class SandBoxMissionViews
	{
		[ViewMethod("TownCenter")]
		public static MissionView[] OpenTownCenterMission(Mission mission)
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
				ViewCreator.CreateMissionAgentLockVisualizerView(mission),
				SandBoxViewCreator.CreateMissionNameMarkerUIHandler(mission),
				new MissionItemContourControllerView(),
				new MissionAgentContourControllerView(),
				new MusicMissionView(new MusicBaseComponent[]
				{
					new MusicMissionSettlementComponent(),
					new MusicMissionAlleyFightComponent()
				}),
				ViewCreator.CreateMissionBoundaryCrossingView(),
				ViewCreator.CreateMissionLeaveView(),
				ViewCreator.CreatePhotoModeView(),
				SandBoxViewCreator.CreateMissionBarterView(),
				ViewCreator.CreateSingleplayerMissionKillNotificationUIHandler(),
				new MissionBoundaryWallView(),
				new MissionCampaignBattleSpectatorView()
			}.ToArray();
		}

		[ViewMethod("TownAmbush")]
		public static MissionView[] OpenTownAmbushMission(Mission mission)
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
					new MusicMissionBattleComponent()
				}),
				ViewCreator.CreateMissionBoundaryCrossingView(),
				SandBoxViewCreator.CreateMissionBarterView(),
				ViewCreator.CreatePhotoModeView(),
				new MissionBoundaryWallView(),
				new MissionCampaignBattleSpectatorView(),
				ViewCreator.CreateMissionFormationMarkerUIHandler(mission),
				ViewCreator.CreateSingleplayerMissionKillNotificationUIHandler()
			}.ToArray();
		}

		[ViewMethod("FacialAnimationTest")]
		public static MissionView[] OpenFacialAnimationTest(Mission mission)
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
					new MusicMissionSettlementComponent()
				}),
				ViewCreator.CreateMissionBoundaryCrossingView(),
				SandBoxViewCreator.CreateMissionBarterView(),
				new MissionBoundaryWallView(),
				ViewCreator.CreatePhotoModeView()
			}.ToArray();
		}

		[ViewMethod("Indoor")]
		public static MissionView[] OpenTavernMission(Mission mission)
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
				ViewCreator.CreateMissionAgentLockVisualizerView(mission),
				new MusicSilencedMissionView(),
				SandBoxViewCreator.CreateMissionBarterView(),
				ViewCreator.CreateMissionLeaveView(),
				SandBoxViewCreator.CreateBoardGameView(),
				SandBoxViewCreator.CreateMissionNameMarkerUIHandler(mission),
				new MissionItemContourControllerView(),
				new MissionAgentContourControllerView(),
				new MissionCampaignBattleSpectatorView(),
				ViewCreator.CreatePhotoModeView(),
				ViewCreator.CreateSingleplayerMissionKillNotificationUIHandler()
			}.ToArray();
		}

		[ViewMethod("PrisonBreak")]
		public static MissionView[] OpenPrisonBreakMission(Mission mission)
		{
			return new List<MissionView>
			{
				new MissionCampaignView(),
				ViewCreator.CreateMissionSingleplayerEscapeMenu(CampaignOptions.IsIronmanMode),
				ViewCreator.CreateOptionsUIHandler(),
				ViewCreator.CreateMissionMainAgentEquipDropView(mission),
				new MissionSingleplayerViewHandler(),
				ViewCreator.CreateMissionAgentStatusUIHandler(mission),
				ViewCreator.CreateMissionMainAgentEquipmentController(mission),
				new MusicSilencedMissionView(),
				SandBoxViewCreator.CreateMissionBarterView(),
				ViewCreator.CreateMissionLeaveView(),
				SandBoxViewCreator.CreateMissionNameMarkerUIHandler(mission),
				new MissionItemContourControllerView(),
				new MissionAgentContourControllerView(),
				ViewCreator.CreateSingleplayerMissionKillNotificationUIHandler()
			}.ToArray();
		}

		[ViewMethod("Village")]
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
				ViewCreator.CreateMissionAgentLockVisualizerView(mission),
				new MusicMissionView(new MusicBaseComponent[]
				{
					new MusicMissionSettlementComponent(),
					new MusicMissionAlleyFightComponent()
				}),
				ViewCreator.CreateMissionBoundaryCrossingView(),
				SandBoxViewCreator.CreateMissionBarterView(),
				ViewCreator.CreateMissionLeaveView(),
				new MissionBoundaryWallView(),
				SandBoxViewCreator.CreateMissionNameMarkerUIHandler(mission),
				new MissionItemContourControllerView(),
				new MissionAgentContourControllerView(),
				new MissionCampaignBattleSpectatorView(),
				ViewCreator.CreatePhotoModeView(),
				ViewCreator.CreateSingleplayerMissionKillNotificationUIHandler()
			}.ToArray();
		}

		[ViewMethod("Retirement")]
		public static MissionView[] OpenRetirementMission(Mission mission)
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
				ViewCreator.CreateMissionAgentLockVisualizerView(mission),
				ViewCreator.CreateMissionBoundaryCrossingView(),
				SandBoxViewCreator.CreateMissionBarterView(),
				ViewCreator.CreateMissionLeaveView(),
				new MissionBoundaryWallView(),
				SandBoxViewCreator.CreateMissionNameMarkerUIHandler(mission),
				new MissionItemContourControllerView(),
				new MissionAgentContourControllerView(),
				ViewCreator.CreatePhotoModeView()
			}.ToArray();
		}

		[ViewMethod("ArenaPracticeFight")]
		public static MissionView[] OpenArenaStartMission(Mission mission)
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
				new MissionAudienceHandler(0.4f + MBRandom.RandomFloat * 0.3f),
				SandBoxViewCreator.CreateMissionArenaPracticeFightView(),
				ViewCreator.CreateMissionLeaveView(),
				ViewCreator.CreateSingleplayerMissionKillNotificationUIHandler(),
				new MusicArenaPracticeMissionView(),
				SandBoxViewCreator.CreateMissionBarterView(),
				new MissionItemContourControllerView(),
				new MissionAgentContourControllerView(),
				new MissionCampaignBattleSpectatorView(),
				ViewCreator.CreatePhotoModeView(),
				new ArenaPreloadView()
			}.ToArray();
		}

		[ViewMethod("ArenaDuelMission")]
		public static MissionView[] OpenArenaDuelMission(Mission mission)
		{
			return new List<MissionView>
			{
				new MissionCampaignView(),
				ViewCreator.CreateMissionSingleplayerEscapeMenu(CampaignOptions.IsIronmanMode),
				ViewCreator.CreateOptionsUIHandler(),
				ViewCreator.CreateMissionMainAgentEquipDropView(mission),
				ViewCreator.CreateMissionLeaveView(),
				ViewCreator.CreateSingleplayerMissionKillNotificationUIHandler(),
				new MissionSingleplayerViewHandler(),
				new MusicSilencedMissionView(),
				ViewCreator.CreateMissionAgentStatusUIHandler(mission),
				ViewCreator.CreateMissionMainAgentEquipmentController(mission),
				ViewCreator.CreateMissionMainAgentCheerBarkControllerView(mission),
				ViewCreator.CreateMissionAgentLockVisualizerView(mission),
				new MissionAudienceHandler(0.4f + MBRandom.RandomFloat * 0.3f),
				new MissionItemContourControllerView(),
				new MissionAgentContourControllerView(),
				new MissionCampaignBattleSpectatorView(),
				ViewCreator.CreatePhotoModeView()
			}.ToArray();
		}

		[ViewMethod("TownMerchant")]
		public static MissionView[] OpenTownMerchantMission(Mission mission)
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
				new MusicMissionView(new MusicBaseComponent[]
				{
					new MusicMissionSettlementComponent()
				}),
				ViewCreator.CreateMissionAgentLockVisualizerView(mission),
				ViewCreator.CreateMissionLeaveView(),
				SandBoxViewCreator.CreateMissionBarterView(),
				SandBoxViewCreator.CreateMissionNameMarkerUIHandler(mission),
				new MissionItemContourControllerView(),
				new MissionAgentContourControllerView(),
				new MissionCampaignBattleSpectatorView(),
				ViewCreator.CreatePhotoModeView(),
				ViewCreator.CreateSingleplayerMissionKillNotificationUIHandler()
			}.ToArray();
		}

		[ViewMethod("Alley")]
		public static MissionView[] OpenAlleyMission(Mission mission)
		{
			return new List<MissionView>
			{
				new MissionCampaignView(),
				new MissionConversationCameraView(),
				SandBoxViewCreator.CreateMissionConversationView(mission),
				ViewCreator.CreateMissionSingleplayerEscapeMenu(CampaignOptions.IsIronmanMode),
				ViewCreator.CreateMissionLeaveView(),
				ViewCreator.CreateOptionsUIHandler(),
				ViewCreator.CreateMissionMainAgentEquipDropView(mission),
				new MissionSingleplayerViewHandler(),
				ViewCreator.CreateMissionAgentStatusUIHandler(mission),
				ViewCreator.CreateMissionMainAgentEquipmentController(mission),
				ViewCreator.CreateMissionMainAgentCheerBarkControllerView(mission),
				ViewCreator.CreateMissionAgentLockVisualizerView(mission),
				new MusicMissionView(new MusicBaseComponent[]
				{
					new MusicMissionBattleComponent()
				}),
				ViewCreator.CreateMissionBoundaryCrossingView(),
				SandBoxViewCreator.CreateMissionBarterView(),
				new MissionBoundaryWallView(),
				new MissionItemContourControllerView(),
				new MissionAgentContourControllerView(),
				new MissionCampaignBattleSpectatorView(),
				ViewCreator.CreatePhotoModeView(),
				ViewCreator.CreateSingleplayerMissionKillNotificationUIHandler()
			}.ToArray();
		}

		[ViewMethod("SneakTeam3")]
		public static MissionView[] OpenSneakTeam3Mission(Mission mission)
		{
			return new List<MissionView>
			{
				new MissionConversationCameraView(),
				ViewCreator.CreateMissionSingleplayerEscapeMenu(CampaignOptions.IsIronmanMode),
				ViewCreator.CreateOptionsUIHandler(),
				ViewCreator.CreateMissionMainAgentEquipDropView(mission),
				ViewCreator.CreateMissionAgentStatusUIHandler(mission),
				ViewCreator.CreateMissionMainAgentEquipmentController(mission),
				ViewCreator.CreateMissionMainAgentCheerBarkControllerView(mission),
				ViewCreator.CreateMissionAgentLockVisualizerView(mission),
				new MissionSingleplayerViewHandler(),
				ViewCreator.CreateMissionBoundaryCrossingView(),
				new MissionBoundaryWallView(),
				new MissionCampaignBattleSpectatorView(),
				ViewCreator.CreatePhotoModeView(),
				ViewCreator.CreateSingleplayerMissionKillNotificationUIHandler()
			}.ToArray();
		}

		[ViewMethod("SimpleMountedPlayer")]
		public static MissionView[] OpenSimpleMountedPlayerMission(Mission mission)
		{
			return new List<MissionView>().ToArray();
		}

		[ViewMethod("Battle")]
		public static MissionView[] OpenBattleMission(Mission mission)
		{
			List<MissionView> list = new List<MissionView>();
			list.Add(new MissionCampaignView());
			list.Add(ViewCreator.CreateMissionSingleplayerEscapeMenu(CampaignOptions.IsIronmanMode));
			list.Add(ViewCreator.CreateMissionAgentLabelUIHandler(mission));
			list.Add(ViewCreator.CreateMissionBattleScoreUIHandler(mission, new SPScoreboardVM(null)));
			list.Add(ViewCreator.CreateOptionsUIHandler());
			list.Add(ViewCreator.CreateMissionMainAgentEquipDropView(mission));
			MissionView missionView = ViewCreator.CreateMissionOrderUIHandler(null);
			list.Add(missionView);
			list.Add(new OrderTroopPlacer());
			list.Add(new MissionSingleplayerViewHandler());
			list.Add(ViewCreator.CreateMissionAgentStatusUIHandler(mission));
			list.Add(ViewCreator.CreateMissionMainAgentEquipmentController(mission));
			list.Add(ViewCreator.CreateMissionMainAgentCheerBarkControllerView(mission));
			list.Add(ViewCreator.CreateMissionAgentLockVisualizerView(mission));
			list.Add(new MusicBattleMissionView(false));
			list.Add(new DeploymentMissionView());
			list.Add(new MissionDeploymentBoundaryMarker(new BorderFlagEntityFactory("swallowtail_banner"), MissionDeploymentBoundaryMarker.MissionDeploymentBoundaryType.DynamicDeploymentBoundaries, 2f));
			list.Add(ViewCreator.CreateMissionBoundaryCrossingView());
			list.Add(new MissionBoundaryWallView());
			list.Add(ViewCreator.CreateMissionFormationMarkerUIHandler(mission));
			list.Add(ViewCreator.CreateSingleplayerMissionKillNotificationUIHandler());
			list.Add(ViewCreator.CreateMissionSpectatorControlView(mission));
			list.Add(new MissionItemContourControllerView());
			list.Add(new MissionAgentContourControllerView());
			list.Add(new MissionPreloadView());
			list.Add(new MissionCampaignBattleSpectatorView());
			list.Add(ViewCreator.CreatePhotoModeView());
			ISiegeDeploymentView siegeDeploymentView = missionView as ISiegeDeploymentView;
			list.Add(new MissionEntitySelectionUIHandler(new Action<GameEntity>(siegeDeploymentView.OnEntitySelection), new Action<GameEntity>(siegeDeploymentView.OnEntityHover)));
			list.Add(ViewCreator.CreateMissionOrderOfBattleUIHandler(mission, new SPOrderOfBattleVM()));
			return list.ToArray();
		}

		[ViewMethod("AlleyFight")]
		public static MissionView[] OpenAlleyFightMission(Mission mission)
		{
			return new List<MissionView>
			{
				new MissionCampaignView(),
				ViewCreator.CreateMissionSingleplayerEscapeMenu(CampaignOptions.IsIronmanMode),
				ViewCreator.CreateOptionsUIHandler(),
				ViewCreator.CreateMissionMainAgentEquipDropView(mission),
				ViewCreator.CreateMissionBattleScoreUIHandler(mission, new SPScoreboardVM(null)),
				ViewCreator.CreateMissionAgentLabelUIHandler(mission),
				ViewCreator.CreateMissionOrderUIHandler(null),
				new OrderTroopPlacer(),
				new MissionSingleplayerViewHandler(),
				ViewCreator.CreateMissionAgentStatusUIHandler(mission),
				ViewCreator.CreateMissionMainAgentEquipmentController(mission),
				ViewCreator.CreateMissionMainAgentCheerBarkControllerView(mission),
				ViewCreator.CreateMissionAgentLockVisualizerView(mission),
				ViewCreator.CreateMissionBoundaryCrossingView(),
				new MissionBoundaryWallView(),
				ViewCreator.CreateMissionFormationMarkerUIHandler(mission),
				ViewCreator.CreateSingleplayerMissionKillNotificationUIHandler(),
				ViewCreator.CreateMissionSpectatorControlView(mission),
				new MissionItemContourControllerView(),
				new MissionAgentContourControllerView(),
				new MissionCampaignBattleSpectatorView(),
				ViewCreator.CreatePhotoModeView()
			}.ToArray();
		}

		[ViewMethod("HideoutBattle")]
		public static MissionView[] OpenHideoutBattleMission(Mission mission)
		{
			return new List<MissionView>
			{
				new MissionCampaignView(),
				new MissionConversationCameraView(),
				new MissionHideoutCinematicView(),
				SandBoxViewCreator.CreateMissionConversationView(mission),
				ViewCreator.CreateMissionSingleplayerEscapeMenu(CampaignOptions.IsIronmanMode),
				ViewCreator.CreateOptionsUIHandler(),
				ViewCreator.CreateMissionMainAgentEquipDropView(mission),
				ViewCreator.CreateMissionBattleScoreUIHandler(mission, new SPScoreboardVM(null)),
				ViewCreator.CreateMissionAgentLabelUIHandler(mission),
				ViewCreator.CreateMissionOrderUIHandler(null),
				new OrderTroopPlacer(),
				new MissionSingleplayerViewHandler(),
				new MusicSilencedMissionView(),
				ViewCreator.CreateMissionAgentStatusUIHandler(mission),
				ViewCreator.CreateMissionMainAgentEquipmentController(mission),
				ViewCreator.CreateMissionMainAgentCheerBarkControllerView(mission),
				ViewCreator.CreateMissionAgentLockVisualizerView(mission),
				ViewCreator.CreateMissionBoundaryCrossingView(),
				new MissionBoundaryWallView(),
				ViewCreator.CreateMissionFormationMarkerUIHandler(mission),
				ViewCreator.CreateSingleplayerMissionKillNotificationUIHandler(),
				ViewCreator.CreateMissionSpectatorControlView(mission),
				new MissionItemContourControllerView(),
				new MissionAgentContourControllerView(),
				new MissionCampaignBattleSpectatorView(),
				new MissionPreloadView(),
				ViewCreator.CreatePhotoModeView()
			}.ToArray();
		}

		[ViewMethod("EnteringSettlementBattle")]
		public static MissionView[] OpenBattleMissionWhileEnteringSettlement(Mission mission)
		{
			return new List<MissionView>
			{
				new MissionCampaignView(),
				new MissionConversationCameraView(),
				ViewCreator.CreateMissionSingleplayerEscapeMenu(CampaignOptions.IsIronmanMode),
				SandBoxViewCreator.CreateMissionConversationView(mission),
				ViewCreator.CreateOptionsUIHandler(),
				ViewCreator.CreateMissionMainAgentEquipDropView(mission),
				ViewCreator.CreateMissionBattleScoreUIHandler(mission, new SPScoreboardVM(null)),
				ViewCreator.CreateMissionAgentLabelUIHandler(mission),
				ViewCreator.CreateMissionOrderUIHandler(null),
				new OrderTroopPlacer(),
				new MissionSingleplayerViewHandler(),
				ViewCreator.CreateMissionAgentStatusUIHandler(mission),
				ViewCreator.CreateMissionMainAgentEquipmentController(mission),
				ViewCreator.CreateMissionMainAgentCheerBarkControllerView(mission),
				ViewCreator.CreateMissionAgentLockVisualizerView(mission),
				ViewCreator.CreateMissionBoundaryCrossingView(),
				new MissionBoundaryWallView(),
				ViewCreator.CreateMissionFormationMarkerUIHandler(mission),
				ViewCreator.CreateSingleplayerMissionKillNotificationUIHandler(),
				ViewCreator.CreateMissionSpectatorControlView(mission),
				new MissionItemContourControllerView(),
				new MissionAgentContourControllerView(),
				new MissionCampaignBattleSpectatorView(),
				ViewCreator.CreatePhotoModeView()
			}.ToArray();
		}

		[ViewMethod("CombatWithDialogue")]
		public static MissionView[] OpenCombatMissionWithDialogue(Mission mission)
		{
			return new List<MissionView>
			{
				new MissionCampaignView(),
				new MissionConversationCameraView(),
				ViewCreator.CreateMissionSingleplayerEscapeMenu(CampaignOptions.IsIronmanMode),
				SandBoxViewCreator.CreateMissionConversationView(mission),
				ViewCreator.CreateOptionsUIHandler(),
				ViewCreator.CreateMissionMainAgentEquipDropView(mission),
				ViewCreator.CreateMissionBattleScoreUIHandler(mission, new SPScoreboardVM(null)),
				ViewCreator.CreateMissionAgentLabelUIHandler(mission),
				ViewCreator.CreateMissionOrderUIHandler(null),
				new OrderTroopPlacer(),
				new MissionSingleplayerViewHandler(),
				ViewCreator.CreateMissionAgentStatusUIHandler(mission),
				ViewCreator.CreateMissionMainAgentEquipmentController(mission),
				ViewCreator.CreateMissionMainAgentCheerBarkControllerView(mission),
				ViewCreator.CreateMissionAgentLockVisualizerView(mission),
				ViewCreator.CreateMissionBoundaryCrossingView(),
				new MissionBoundaryWallView(),
				ViewCreator.CreateMissionFormationMarkerUIHandler(mission),
				ViewCreator.CreateSingleplayerMissionKillNotificationUIHandler(),
				ViewCreator.CreateMissionSpectatorControlView(mission),
				new MissionItemContourControllerView(),
				new MissionAgentContourControllerView(),
				new MissionCampaignBattleSpectatorView(),
				ViewCreator.CreatePhotoModeView()
			}.ToArray();
		}

		[ViewMethod("SiegeEngine")]
		public static MissionView[] OpenTestSiegeEngineMission(Mission mission)
		{
			return new List<MissionView>
			{
				new MissionConversationCameraView(),
				ViewCreator.CreateMissionOrderUIHandler(null),
				new OrderTroopPlacer()
			}.ToArray();
		}

		[ViewMethod("CustomCameraMission")]
		public static MissionView[] OpenCustomCameraMission(Mission mission)
		{
			return new List<MissionView>
			{
				new MissionConversationCameraView(),
				new MissionCustomCameraView()
			}.ToArray();
		}

		[ViewMethod("AmbushBattle")]
		public static MissionView[] OpenAmbushBattleMission(Mission mission)
		{
			throw new NotImplementedException("Ambush battle is not implemented.");
		}

		[ViewMethod("Ambush")]
		public static MissionView[] OpenAmbushMission(Mission mission)
		{
			return new List<MissionView>
			{
				new MissionCampaignView(),
				new MissionConversationCameraView(),
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
					new MusicMissionBattleComponent()
				}),
				new MissionAmbushView(),
				ViewCreator.CreateMissionBoundaryCrossingView(),
				new MissionBoundaryWallView(),
				ViewCreator.CreateMissionFormationMarkerUIHandler(mission),
				ViewCreator.CreateSingleplayerMissionKillNotificationUIHandler(),
				new MissionCampaignBattleSpectatorView(),
				ViewCreator.CreatePhotoModeView()
			}.ToArray();
		}

		[ViewMethod("Camp")]
		public static MissionView[] OpenCampMission(Mission mission)
		{
			return new List<MissionView>
			{
				new MissionCampaignView(),
				new MissionConversationCameraView(),
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
					new MusicMissionSettlementComponent()
				}),
				ViewCreator.CreateMissionBoundaryCrossingView(),
				new MissionBoundaryWallView(),
				new MissionCampaignBattleSpectatorView(),
				ViewCreator.CreatePhotoModeView(),
				ViewCreator.CreateSingleplayerMissionKillNotificationUIHandler()
			}.ToArray();
		}

		[ViewMethod("SiegeMissionWithDeployment")]
		public static MissionView[] OpenSiegeMissionWithDeployment(Mission mission)
		{
			List<MissionView> list = new List<MissionView>();
			mission.GetMissionBehavior<SiegeDeploymentHandler>();
			list.Add(new MissionCampaignView());
			list.Add(new MissionConversationCameraView());
			list.Add(ViewCreator.CreateMissionSingleplayerEscapeMenu(CampaignOptions.IsIronmanMode));
			list.Add(ViewCreator.CreateOptionsUIHandler());
			list.Add(ViewCreator.CreateMissionMainAgentEquipDropView(mission));
			list.Add(ViewCreator.CreateMissionAgentLabelUIHandler(mission));
			list.Add(ViewCreator.CreateMissionBattleScoreUIHandler(mission, new SPScoreboardVM(null)));
			MissionView missionView = ViewCreator.CreateMissionOrderUIHandler(null);
			list.Add(ViewCreator.CreateMissionAgentStatusUIHandler(mission));
			list.Add(ViewCreator.CreateMissionMainAgentEquipmentController(mission));
			list.Add(ViewCreator.CreateMissionMainAgentCheerBarkControllerView(mission));
			list.Add(ViewCreator.CreateMissionAgentLockVisualizerView(mission));
			list.Add(missionView);
			list.Add(new OrderTroopPlacer());
			list.Add(new MissionSingleplayerViewHandler());
			list.Add(new MusicBattleMissionView(true));
			list.Add(new DeploymentMissionView());
			list.Add(new MissionDeploymentBoundaryMarker(new BorderFlagEntityFactory("swallowtail_banner"), MissionDeploymentBoundaryMarker.MissionDeploymentBoundaryType.StaticSceneBoundaries, 2f));
			list.Add(ViewCreator.CreateMissionBoundaryCrossingView());
			list.Add(ViewCreator.CreateSingleplayerMissionKillNotificationUIHandler());
			list.Add(ViewCreator.CreatePhotoModeView());
			ISiegeDeploymentView siegeDeploymentView = missionView as ISiegeDeploymentView;
			list.Add(new MissionEntitySelectionUIHandler(new Action<GameEntity>(siegeDeploymentView.OnEntitySelection), new Action<GameEntity>(siegeDeploymentView.OnEntityHover)));
			list.Add(ViewCreator.CreateMissionSpectatorControlView(mission));
			list.Add(new MissionItemContourControllerView());
			list.Add(new MissionAgentContourControllerView());
			list.Add(new MissionPreloadView());
			list.Add(new MissionCampaignBattleSpectatorView());
			list.Add(ViewCreator.CreateMissionOrderOfBattleUIHandler(mission, new SPOrderOfBattleVM()));
			list.Add(ViewCreator.CreateMissionSiegeEngineMarkerView(mission));
			return list.ToArray();
		}

		[ViewMethod("SiegeMissionNoDeployment")]
		public static MissionView[] OpenSiegeMissionNoDeployment(Mission mission)
		{
			return new List<MissionView>
			{
				new MissionCampaignView(),
				new MissionConversationCameraView(),
				ViewCreator.CreateMissionSingleplayerEscapeMenu(CampaignOptions.IsIronmanMode),
				ViewCreator.CreateOptionsUIHandler(),
				ViewCreator.CreateMissionMainAgentEquipDropView(mission),
				ViewCreator.CreateMissionAgentLabelUIHandler(mission),
				ViewCreator.CreateMissionBattleScoreUIHandler(mission, new SPScoreboardVM(null)),
				ViewCreator.CreateMissionOrderUIHandler(null),
				new OrderTroopPlacer(),
				new MissionSingleplayerViewHandler(),
				ViewCreator.CreateMissionAgentStatusUIHandler(mission),
				ViewCreator.CreateMissionMainAgentEquipmentController(mission),
				ViewCreator.CreateMissionMainAgentCheerBarkControllerView(mission),
				ViewCreator.CreateMissionAgentLockVisualizerView(mission),
				ViewCreator.CreateSingleplayerMissionKillNotificationUIHandler(),
				ViewCreator.CreatePhotoModeView(),
				new MusicBattleMissionView(true),
				ViewCreator.CreateMissionBoundaryCrossingView(),
				new MissionBoundaryWallView(),
				ViewCreator.CreateMissionSpectatorControlView(mission),
				new MissionItemContourControllerView(),
				new MissionAgentContourControllerView(),
				new MissionPreloadView(),
				new MissionCampaignBattleSpectatorView(),
				ViewCreator.CreateMissionSiegeEngineMarkerView(mission)
			}.ToArray();
		}

		[ViewMethod("SiegeLordsHallFightMission")]
		public static MissionView[] OpenSiegeLordsHallFightMission(Mission mission)
		{
			return new List<MissionView>
			{
				new MissionCampaignView(),
				new MissionConversationCameraView(),
				ViewCreator.CreateMissionSingleplayerEscapeMenu(CampaignOptions.IsIronmanMode),
				ViewCreator.CreateOptionsUIHandler(),
				ViewCreator.CreateMissionMainAgentEquipDropView(mission),
				ViewCreator.CreateMissionBattleScoreUIHandler(mission, new SPScoreboardVM(null)),
				ViewCreator.CreateMissionAgentLabelUIHandler(mission),
				ViewCreator.CreateMissionOrderUIHandler(null),
				new OrderTroopPlacer(),
				new MissionSingleplayerViewHandler(),
				ViewCreator.CreateMissionAgentStatusUIHandler(mission),
				ViewCreator.CreateMissionMainAgentEquipmentController(mission),
				ViewCreator.CreateMissionMainAgentCheerBarkControllerView(mission),
				ViewCreator.CreateMissionBoundaryCrossingView(),
				new MissionBoundaryWallView(),
				ViewCreator.CreateMissionFormationMarkerUIHandler(mission),
				ViewCreator.CreateSingleplayerMissionKillNotificationUIHandler(),
				ViewCreator.CreateMissionSpectatorControlView(mission),
				new MissionItemContourControllerView(),
				new MissionAgentContourControllerView(),
				new MissionPreloadView()
			}.ToArray();
		}

		[ViewMethod("Siege")]
		public static MissionView[] OpenSiegeMission(Mission mission)
		{
			List<MissionView> list = new List<MissionView>();
			mission.GetMissionBehavior<SiegeDeploymentHandler>();
			list.Add(new MissionCampaignView());
			list.Add(new MissionConversationCameraView());
			list.Add(ViewCreator.CreateMissionSingleplayerEscapeMenu(CampaignOptions.IsIronmanMode));
			list.Add(ViewCreator.CreateOptionsUIHandler());
			list.Add(ViewCreator.CreateMissionMainAgentEquipDropView(mission));
			MissionView missionView = ViewCreator.CreateMissionOrderUIHandler(null);
			list.Add(missionView);
			list.Add(new OrderTroopPlacer());
			list.Add(new MissionSingleplayerViewHandler());
			list.Add(new MusicMissionView(new MusicBaseComponent[]
			{
				new MusicMissionBattleComponent()
			}));
			list.Add(new DeploymentMissionView());
			list.Add(new MissionDeploymentBoundaryMarker(new BorderFlagEntityFactory("swallowtail_banner"), MissionDeploymentBoundaryMarker.MissionDeploymentBoundaryType.StaticSceneBoundaries, 2f));
			list.Add(ViewCreator.CreateMissionBoundaryCrossingView());
			list.Add(ViewCreator.CreateSingleplayerMissionKillNotificationUIHandler());
			list.Add(ViewCreator.CreateMissionAgentStatusUIHandler(mission));
			list.Add(ViewCreator.CreateMissionMainAgentEquipmentController(mission));
			list.Add(ViewCreator.CreateMissionMainAgentCheerBarkControllerView(mission));
			list.Add(ViewCreator.CreateMissionAgentLockVisualizerView(mission));
			list.Add(ViewCreator.CreateMissionSpectatorControlView(mission));
			list.Add(ViewCreator.CreatePhotoModeView());
			ISiegeDeploymentView siegeDeploymentView = missionView as ISiegeDeploymentView;
			list.Add(new MissionEntitySelectionUIHandler(new Action<GameEntity>(siegeDeploymentView.OnEntitySelection), new Action<GameEntity>(siegeDeploymentView.OnEntityHover)));
			list.Add(ViewCreator.CreateMissionFormationMarkerUIHandler(mission));
			list.Add(new MissionItemContourControllerView());
			list.Add(new MissionAgentContourControllerView());
			list.Add(new MissionCampaignBattleSpectatorView());
			list.Add(ViewCreator.CreateMissionSiegeEngineMarkerView(mission));
			return list.ToArray();
		}

		[ViewMethod("SiegeMissionForTutorial")]
		public static MissionView[] OpenSiegeMissionForTutorial(Mission mission)
		{
			Debug.FailedAssert("Do not use SiegeForTutorial! Use campaign!", "C:\\Develop\\MB3\\Source\\Bannerlord\\SandBox.View\\Missions\\SandBoxMissionViews.cs", "OpenSiegeMissionForTutorial", 871);
			List<MissionView> list = new List<MissionView>();
			list.Add(new MissionConversationCameraView());
			list.Add(ViewCreator.CreateMissionSingleplayerEscapeMenu(CampaignOptions.IsIronmanMode));
			list.Add(ViewCreator.CreateOptionsUIHandler());
			list.Add(ViewCreator.CreateMissionMainAgentEquipDropView(mission));
			MissionView missionView = ViewCreator.CreateMissionOrderUIHandler(null);
			list.Add(missionView);
			list.Add(new OrderTroopPlacer());
			list.Add(new MissionSingleplayerViewHandler());
			list.Add(new MusicMissionView(new MusicBaseComponent[]
			{
				new MusicMissionBattleComponent()
			}));
			list.Add(ViewCreator.CreateMissionAgentStatusUIHandler(mission));
			list.Add(ViewCreator.CreateMissionMainAgentEquipmentController(mission));
			list.Add(ViewCreator.CreateMissionMainAgentCheerBarkControllerView(mission));
			list.Add(ViewCreator.CreateMissionAgentLockVisualizerView(mission));
			list.Add(ViewCreator.CreateMissionSpectatorControlView(mission));
			list.Add(ViewCreator.CreatePhotoModeView());
			list.Add(ViewCreator.CreateMissionSiegeEngineMarkerView(mission));
			ISiegeDeploymentView siegeDeploymentView = missionView as ISiegeDeploymentView;
			list.Add(new MissionEntitySelectionUIHandler(new Action<GameEntity>(siegeDeploymentView.OnEntitySelection), new Action<GameEntity>(siegeDeploymentView.OnEntityHover)));
			list.Add(new MissionDeploymentBoundaryMarker(new BorderFlagEntityFactory("swallowtail_banner"), MissionDeploymentBoundaryMarker.MissionDeploymentBoundaryType.StaticSceneBoundaries, 2f));
			list.Add(new MissionCampaignBattleSpectatorView());
			return list.ToArray();
		}

		[ViewMethod("AmbushBattleForTutorial")]
		public static MissionView[] OpenAmbushMissionForTutorial(Mission mission)
		{
			List<MissionView> list = new List<MissionView>();
			list.Add(new MissionCampaignView());
			list.Add(new MissionConversationCameraView());
			if (mission.GetMissionBehavior<AmbushMissionController>().IsPlayerAmbusher)
			{
				list.Add(new MissionDeploymentBoundaryMarker(new BorderFlagEntityFactory("swallowtail_banner"), MissionDeploymentBoundaryMarker.MissionDeploymentBoundaryType.StaticSceneBoundaries, 2f));
			}
			list.Add(ViewCreator.CreateMissionSingleplayerEscapeMenu(CampaignOptions.IsIronmanMode));
			list.Add(ViewCreator.CreateOptionsUIHandler());
			list.Add(ViewCreator.CreateMissionMainAgentEquipDropView(mission));
			list.Add(ViewCreator.CreateMissionOrderUIHandler(null));
			list.Add(new OrderTroopPlacer());
			list.Add(new MissionSingleplayerViewHandler());
			list.Add(new MusicMissionView(new MusicBaseComponent[]
			{
				new MusicMissionBattleComponent()
			}));
			list.Add(new MissionAmbushView());
			list.Add(ViewCreator.CreatePhotoModeView());
			list.Add(new MissionAmbushIntroView());
			list.Add(new MissionDeploymentBoundaryMarker(new BorderFlagEntityFactory("swallowtail_banner"), MissionDeploymentBoundaryMarker.MissionDeploymentBoundaryType.StaticSceneBoundaries, 2f));
			list.Add(new MissionCampaignBattleSpectatorView());
			return list.ToArray();
		}

		[ViewMethod("FormationTest")]
		public static MissionView[] OpenFormationTestMission(Mission mission)
		{
			return new List<MissionView>
			{
				new MissionConversationCameraView(),
				ViewCreator.CreateMissionOrderUIHandler(null),
				new OrderTroopPlacer()
			}.ToArray();
		}

		[ViewMethod("VillageBattle")]
		public static MissionView[] OpenVillageBattleMission(Mission mission)
		{
			return new List<MissionView>
			{
				new MissionCampaignView(),
				new MissionConversationCameraView(),
				ViewCreator.CreateMissionSingleplayerEscapeMenu(CampaignOptions.IsIronmanMode),
				ViewCreator.CreateOptionsUIHandler(),
				ViewCreator.CreateMissionMainAgentEquipDropView(mission),
				ViewCreator.CreateMissionBattleScoreUIHandler(mission, new SPScoreboardVM(null)),
				ViewCreator.CreateMissionOrderUIHandler(null),
				new OrderTroopPlacer(),
				ViewCreator.CreateMissionAgentStatusUIHandler(mission),
				ViewCreator.CreateMissionMainAgentEquipmentController(mission),
				ViewCreator.CreateMissionMainAgentCheerBarkControllerView(mission),
				ViewCreator.CreateMissionAgentLockVisualizerView(mission),
				new MissionSingleplayerViewHandler(),
				new MusicMissionView(new MusicBaseComponent[]
				{
					new MusicMissionBattleComponent()
				}),
				ViewCreator.CreateMissionBoundaryCrossingView(),
				new MissionBoundaryWallView(),
				ViewCreator.CreateMissionFormationMarkerUIHandler(mission),
				ViewCreator.CreateMissionSpectatorControlView(mission),
				new MissionItemContourControllerView(),
				new MissionAgentContourControllerView(),
				new MissionCampaignBattleSpectatorView(),
				ViewCreator.CreatePhotoModeView(),
				ViewCreator.CreateSingleplayerMissionKillNotificationUIHandler()
			}.ToArray();
		}

		[ViewMethod("SettlementTest")]
		public static MissionView[] OpenSettlementTestMission(Mission mission)
		{
			return new List<MissionView>
			{
				new MissionConversationCameraView(),
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
					new MusicMissionSettlementComponent()
				}),
				ViewCreator.CreateMissionBoundaryCrossingView(),
				new MissionBoundaryWallView(),
				ViewCreator.CreateMissionSpectatorControlView(mission),
				new MissionCampaignBattleSpectatorView(),
				ViewCreator.CreatePhotoModeView()
			}.ToArray();
		}

		[ViewMethod("EquipmentTest")]
		public static MissionView[] OpenEquipmentTestMission(Mission mission)
		{
			return new List<MissionView>
			{
				new MissionConversationCameraView(),
				ViewCreator.CreateMissionSingleplayerEscapeMenu(CampaignOptions.IsIronmanMode),
				ViewCreator.CreateOptionsUIHandler(),
				ViewCreator.CreateMissionMainAgentEquipDropView(mission),
				new MissionSingleplayerViewHandler(),
				new MusicMissionView(new MusicBaseComponent[]
				{
					new MusicMissionSettlementComponent()
				}),
				ViewCreator.CreateMissionBoundaryCrossingView(),
				new MissionBoundaryWallView(),
				new MissionCampaignBattleSpectatorView(),
				ViewCreator.CreatePhotoModeView()
			}.ToArray();
		}

		[ViewMethod("FacialAnimTest")]
		public static MissionView[] OpenFacialAnimTestMission(Mission mission)
		{
			return new List<MissionView>
			{
				new MissionConversationCameraView(),
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
				SandBoxViewCreator.CreateMissionConversationView(mission),
				SandBoxViewCreator.CreateMissionBarterView(),
				new MissionBoundaryWallView(),
				new MissionCampaignBattleSpectatorView(),
				ViewCreator.CreatePhotoModeView()
			}.ToArray();
		}

		[ViewMethod("EquipItemTool")]
		public static MissionView[] OpenEquipItemToolMission(Mission mission)
		{
			return new List<MissionView>
			{
				new MissionConversationCameraView(),
				ViewCreator.CreateOptionsUIHandler(),
				ViewCreator.CreateMissionMainAgentEquipDropView(mission),
				new MissionEquipItemToolView(),
				ViewCreator.CreateMissionLeaveView()
			}.ToArray();
		}

		[ViewMethod("Conversation")]
		public static MissionView[] OpenConversationMission(Mission mission)
		{
			return new List<MissionView>
			{
				new MissionCampaignView(),
				new MissionConversationCameraView(),
				new MissionSingleplayerViewHandler(),
				new MusicMissionView(new MusicBaseComponent[]
				{
					new MusicMissionSettlementComponent()
				}),
				SandBoxViewCreator.CreateMissionConversationView(mission),
				ViewCreator.CreateMissionSingleplayerEscapeMenu(CampaignOptions.IsIronmanMode),
				ViewCreator.CreateOptionsUIHandler(),
				ViewCreator.CreateMissionMainAgentEquipDropView(mission),
				SandBoxViewCreator.CreateMissionBarterView(),
				ViewCreator.CreateMissionAgentLockVisualizerView(mission),
				new MissionCampaignBattleSpectatorView(),
				ViewCreator.CreatePhotoModeView()
			}.ToArray();
		}
	}
}
