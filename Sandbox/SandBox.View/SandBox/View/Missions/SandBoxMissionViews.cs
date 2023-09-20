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
	// Token: 0x02000020 RID: 32
	[ViewCreatorModule]
	public class SandBoxMissionViews
	{
		// Token: 0x060000BA RID: 186 RVA: 0x00009CA0 File Offset: 0x00007EA0
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

		// Token: 0x060000BB RID: 187 RVA: 0x00009DC4 File Offset: 0x00007FC4
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

		// Token: 0x060000BC RID: 188 RVA: 0x00009EC8 File Offset: 0x000080C8
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

		// Token: 0x060000BD RID: 189 RVA: 0x00009FAC File Offset: 0x000081AC
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

		// Token: 0x060000BE RID: 190 RVA: 0x0000A0AC File Offset: 0x000082AC
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

		// Token: 0x060000BF RID: 191 RVA: 0x0000A168 File Offset: 0x00008368
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

		// Token: 0x060000C0 RID: 192 RVA: 0x0000A28C File Offset: 0x0000848C
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

		// Token: 0x060000C1 RID: 193 RVA: 0x0000A374 File Offset: 0x00008574
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

		// Token: 0x060000C2 RID: 194 RVA: 0x0000A49C File Offset: 0x0000869C
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

		// Token: 0x060000C3 RID: 195 RVA: 0x0000A58C File Offset: 0x0000878C
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

		// Token: 0x060000C4 RID: 196 RVA: 0x0000A690 File Offset: 0x00008890
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

		// Token: 0x060000C5 RID: 197 RVA: 0x0000A7AC File Offset: 0x000089AC
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

		// Token: 0x060000C6 RID: 198 RVA: 0x0000A867 File Offset: 0x00008A67
		[ViewMethod("SimpleMountedPlayer")]
		public static MissionView[] OpenSimpleMountedPlayerMission(Mission mission)
		{
			return new List<MissionView>().ToArray();
		}

		// Token: 0x060000C7 RID: 199 RVA: 0x0000A874 File Offset: 0x00008A74
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

		// Token: 0x060000C8 RID: 200 RVA: 0x0000AA10 File Offset: 0x00008C10
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

		// Token: 0x060000C9 RID: 201 RVA: 0x0000AB30 File Offset: 0x00008D30
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

		// Token: 0x060000CA RID: 202 RVA: 0x0000AC88 File Offset: 0x00008E88
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

		// Token: 0x060000CB RID: 203 RVA: 0x0000ADC0 File Offset: 0x00008FC0
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

		// Token: 0x060000CC RID: 204 RVA: 0x0000AEF5 File Offset: 0x000090F5
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

		// Token: 0x060000CD RID: 205 RVA: 0x0000AF23 File Offset: 0x00009123
		[ViewMethod("CustomCameraMission")]
		public static MissionView[] OpenCustomCameraMission(Mission mission)
		{
			return new List<MissionView>
			{
				new MissionConversationCameraView(),
				new MissionCustomCameraView()
			}.ToArray();
		}

		// Token: 0x060000CE RID: 206 RVA: 0x0000AF45 File Offset: 0x00009145
		[ViewMethod("AmbushBattle")]
		public static MissionView[] OpenAmbushBattleMission(Mission mission)
		{
			throw new NotImplementedException("Ambush battle is not implemented.");
		}

		// Token: 0x060000CF RID: 207 RVA: 0x0000AF54 File Offset: 0x00009154
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

		// Token: 0x060000D0 RID: 208 RVA: 0x0000B04C File Offset: 0x0000924C
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

		// Token: 0x060000D1 RID: 209 RVA: 0x0000B130 File Offset: 0x00009330
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

		// Token: 0x060000D2 RID: 210 RVA: 0x0000B2D4 File Offset: 0x000094D4
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

		// Token: 0x060000D3 RID: 211 RVA: 0x0000B414 File Offset: 0x00009614
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

		// Token: 0x060000D4 RID: 212 RVA: 0x0000B528 File Offset: 0x00009728
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

		// Token: 0x060000D5 RID: 213 RVA: 0x0000B6AC File Offset: 0x000098AC
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

		// Token: 0x060000D6 RID: 214 RVA: 0x0000B7F4 File Offset: 0x000099F4
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

		// Token: 0x060000D7 RID: 215 RVA: 0x0000B8F4 File Offset: 0x00009AF4
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

		// Token: 0x060000D8 RID: 216 RVA: 0x0000B924 File Offset: 0x00009B24
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

		// Token: 0x060000D9 RID: 217 RVA: 0x0000BA5C File Offset: 0x00009C5C
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

		// Token: 0x060000DA RID: 218 RVA: 0x0000BB34 File Offset: 0x00009D34
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

		// Token: 0x060000DB RID: 219 RVA: 0x0000BBD0 File Offset: 0x00009DD0
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

		// Token: 0x060000DC RID: 220 RVA: 0x0000BCBC File Offset: 0x00009EBC
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

		// Token: 0x060000DD RID: 221 RVA: 0x0000BD0C File Offset: 0x00009F0C
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
