using System;
using System.Collections.Generic;

namespace TaleWorlds.MountAndBlade.View.MissionViews.Multiplayer
{
	[ViewCreatorModule]
	public class MultiplayerMissionViews
	{
		[ViewMethod("MultiplayerFreeForAll")]
		public static MissionView[] OpenFreeForAllMission(Mission mission)
		{
			List<MissionView> list = new List<MissionView>();
			list.Add(ViewCreator.CreateMissionServerStatusUIHandler());
			list.Add(ViewCreator.CreateMissionMultiplayerPreloadView(mission));
			list.Add(ViewCreator.CreateMissionMultiplayerFFAView());
			list.Add(ViewCreator.CreateMissionKillNotificationUIHandler());
			list.Add(ViewCreator.CreateMissionAgentStatusUIHandler(mission));
			list.Add(ViewCreator.CreateMissionMainAgentEquipmentController(mission));
			list.Add(ViewCreator.CreateMissionMainAgentCheerBarkControllerView(mission));
			list.Add(ViewCreator.CreateMissionMultiplayerEscapeMenu("FreeForAll"));
			list.Add(ViewCreator.CreateMultiplayerEndOfBattleUIHandler());
			list.Add(ViewCreator.CreateMissionScoreBoardUIHandler(mission, true));
			list.Add(ViewCreator.CreateLobbyEquipmentUIHandler());
			list.Add(ViewCreator.CreatePollProgressUIHandler());
			list.Add(ViewCreator.CreateMultiplayerMissionHUDExtensionUIHandler());
			list.Add(ViewCreator.CreateMultiplayerMissionDeathCardUIHandler(null));
			list.Add(ViewCreator.CreateOptionsUIHandler());
			list.Add(ViewCreator.CreateMissionMainAgentEquipDropView(mission));
			if (!GameNetwork.IsClient)
			{
				list.Add(ViewCreator.CreateMultiplayerAdminPanelUIHandler());
			}
			list.Add(ViewCreator.CreateMissionBoundaryCrossingView());
			list.Add(new MissionBoundaryWallView());
			return list.ToArray();
		}

		[ViewMethod("MultiplayerTeamDeathmatch")]
		public static MissionView[] OpenTeamDeathmatchMission(Mission mission)
		{
			List<MissionView> list = new List<MissionView>();
			list.Add(ViewCreator.CreateMissionServerStatusUIHandler());
			list.Add(ViewCreator.CreateMissionMultiplayerPreloadView(mission));
			list.Add(ViewCreator.CreateMultiplayerTeamSelectUIHandler());
			list.Add(ViewCreator.CreateMissionKillNotificationUIHandler());
			list.Add(ViewCreator.CreateMissionAgentStatusUIHandler(mission));
			list.Add(ViewCreator.CreateMissionMainAgentEquipmentController(mission));
			list.Add(ViewCreator.CreateMissionMainAgentCheerBarkControllerView(mission));
			list.Add(ViewCreator.CreateMissionMultiplayerEscapeMenu("TeamDeathmatch"));
			list.Add(ViewCreator.CreateMissionScoreBoardUIHandler(mission, false));
			list.Add(ViewCreator.CreateMultiplayerEndOfRoundUIHandler());
			list.Add(ViewCreator.CreateMultiplayerEndOfBattleUIHandler());
			list.Add(ViewCreator.CreateLobbyEquipmentUIHandler());
			list.Add(ViewCreator.CreateMissionAgentLabelUIHandler(mission));
			list.Add(ViewCreator.CreatePollProgressUIHandler());
			list.Add(ViewCreator.CreateMissionFlagMarkerUIHandler());
			list.Add(ViewCreator.CreateMultiplayerMissionHUDExtensionUIHandler());
			list.Add(ViewCreator.CreateMultiplayerMissionDeathCardUIHandler(null));
			list.Add(ViewCreator.CreateOptionsUIHandler());
			list.Add(ViewCreator.CreateMissionMainAgentEquipDropView(mission));
			if (!GameNetwork.IsClient)
			{
				list.Add(ViewCreator.CreateMultiplayerAdminPanelUIHandler());
			}
			list.Add(ViewCreator.CreateMissionBoundaryCrossingView());
			list.Add(new MissionBoundaryWallView());
			list.Add(new MissionItemContourControllerView());
			list.Add(new MissionAgentContourControllerView());
			return list.ToArray();
		}

		[ViewMethod("MultiplayerDuel")]
		public static MissionView[] OpenDuelMission(Mission mission)
		{
			List<MissionView> list = new List<MissionView>();
			list.Add(ViewCreator.CreateMissionServerStatusUIHandler());
			list.Add(ViewCreator.CreateMissionMultiplayerPreloadView(mission));
			list.Add(ViewCreator.CreateMultiplayerCultureSelectUIHandler());
			list.Add(ViewCreator.CreateMissionKillNotificationUIHandler());
			list.Add(ViewCreator.CreateMissionAgentStatusUIHandler(mission));
			list.Add(ViewCreator.CreateMissionMainAgentEquipmentController(mission));
			list.Add(ViewCreator.CreateMissionMainAgentCheerBarkControllerView(mission));
			list.Add(ViewCreator.CreateMissionMultiplayerEscapeMenu("Duel"));
			list.Add(ViewCreator.CreateMultiplayerEndOfBattleUIHandler());
			list.Add(ViewCreator.CreateMissionScoreBoardUIHandler(mission, true));
			list.Add(ViewCreator.CreateLobbyEquipmentUIHandler());
			list.Add(ViewCreator.CreateMissionMultiplayerDuelUI());
			list.Add(ViewCreator.CreatePollProgressUIHandler());
			list.Add(ViewCreator.CreateOptionsUIHandler());
			list.Add(ViewCreator.CreateMissionMainAgentEquipDropView(mission));
			if (!GameNetwork.IsClient)
			{
				list.Add(ViewCreator.CreateMultiplayerAdminPanelUIHandler());
			}
			list.Add(ViewCreator.CreateMissionBoundaryCrossingView());
			list.Add(new MissionBoundaryWallView());
			list.Add(new MissionItemContourControllerView());
			list.Add(new MissionAgentContourControllerView());
			return list.ToArray();
		}

		[ViewMethod("MultiplayerSiege")]
		public static MissionView[] OpenSiegeMission(Mission mission)
		{
			List<MissionView> list = new List<MissionView>();
			list.Add(ViewCreator.CreateMissionServerStatusUIHandler());
			list.Add(ViewCreator.CreateMissionMultiplayerPreloadView(mission));
			list.Add(ViewCreator.CreateMissionKillNotificationUIHandler());
			list.Add(ViewCreator.CreateMissionAgentStatusUIHandler(mission));
			list.Add(ViewCreator.CreateMissionMainAgentEquipmentController(mission));
			list.Add(ViewCreator.CreateMissionMainAgentCheerBarkControllerView(mission));
			list.Add(ViewCreator.CreateMissionMultiplayerEscapeMenu("Siege"));
			list.Add(ViewCreator.CreateMultiplayerEndOfBattleUIHandler());
			list.Add(ViewCreator.CreateMissionAgentLabelUIHandler(mission));
			list.Add(ViewCreator.CreateMultiplayerTeamSelectUIHandler());
			list.Add(ViewCreator.CreateMissionScoreBoardUIHandler(mission, false));
			list.Add(ViewCreator.CreateMultiplayerEndOfRoundUIHandler());
			list.Add(ViewCreator.CreateLobbyEquipmentUIHandler());
			list.Add(ViewCreator.CreatePollProgressUIHandler());
			list.Add(ViewCreator.CreateMultiplayerMissionHUDExtensionUIHandler());
			list.Add(ViewCreator.CreateMultiplayerMissionDeathCardUIHandler(null));
			list.Add(new MissionItemContourControllerView());
			list.Add(new MissionAgentContourControllerView());
			list.Add(ViewCreator.CreateMissionFlagMarkerUIHandler());
			list.Add(ViewCreator.CreateOptionsUIHandler());
			list.Add(ViewCreator.CreateMissionMainAgentEquipDropView(mission));
			if (!GameNetwork.IsClient)
			{
				list.Add(ViewCreator.CreateMultiplayerAdminPanelUIHandler());
			}
			list.Add(ViewCreator.CreateMissionBoundaryCrossingView());
			list.Add(new MissionBoundaryWallView());
			return list.ToArray();
		}

		[ViewMethod("MultiplayerBattle")]
		public static MissionView[] OpenBattle(Mission mission)
		{
			List<MissionView> list = new List<MissionView>();
			list.Add(ViewCreator.CreateLobbyEquipmentUIHandler());
			list.Add(ViewCreator.CreateMultiplayerFactionBanVoteUIHandler());
			list.Add(ViewCreator.CreateMissionAgentStatusUIHandler(mission));
			list.Add(ViewCreator.CreateMissionMultiplayerPreloadView(mission));
			list.Add(ViewCreator.CreateMissionMainAgentEquipmentController(mission));
			list.Add(ViewCreator.CreateMissionMainAgentCheerBarkControllerView(mission));
			list.Add(ViewCreator.CreateMissionMultiplayerEscapeMenu("Battle"));
			list.Add(ViewCreator.CreateMultiplayerMissionOrderUIHandler(mission));
			list.Add(ViewCreator.CreateMissionAgentLabelUIHandler(mission));
			list.Add(ViewCreator.CreateOrderTroopPlacerView(mission));
			list.Add(ViewCreator.CreateMultiplayerTeamSelectUIHandler());
			list.Add(ViewCreator.CreateMissionScoreBoardUIHandler(mission, false));
			list.Add(ViewCreator.CreateMultiplayerEndOfRoundUIHandler());
			list.Add(ViewCreator.CreateMultiplayerEndOfBattleUIHandler());
			list.Add(ViewCreator.CreatePollProgressUIHandler());
			list.Add(new MissionItemContourControllerView());
			list.Add(new MissionAgentContourControllerView());
			list.Add(ViewCreator.CreateMissionKillNotificationUIHandler());
			list.Add(ViewCreator.CreateMultiplayerMissionHUDExtensionUIHandler());
			list.Add(ViewCreator.CreateMultiplayerMissionDeathCardUIHandler(null));
			list.Add(ViewCreator.CreateMissionFlagMarkerUIHandler());
			list.Add(ViewCreator.CreateOptionsUIHandler());
			list.Add(ViewCreator.CreateMissionMainAgentEquipDropView(mission));
			if (!GameNetwork.IsClient)
			{
				list.Add(ViewCreator.CreateMultiplayerAdminPanelUIHandler());
			}
			list.Add(ViewCreator.CreateMissionBoundaryCrossingView());
			list.Add(new MissionBoundaryWallView());
			list.Add(new SpectatorCameraView());
			return list.ToArray();
		}

		[ViewMethod("MultiplayerCaptain")]
		public static MissionView[] OpenCaptain(Mission mission)
		{
			List<MissionView> list = new List<MissionView>();
			list.Add(ViewCreator.CreateLobbyEquipmentUIHandler());
			list.Add(ViewCreator.CreateMissionServerStatusUIHandler());
			list.Add(ViewCreator.CreateMultiplayerFactionBanVoteUIHandler());
			list.Add(ViewCreator.CreateMissionMultiplayerPreloadView(mission));
			list.Add(ViewCreator.CreateMissionKillNotificationUIHandler());
			list.Add(ViewCreator.CreateMissionAgentStatusUIHandler(mission));
			list.Add(ViewCreator.CreateMissionMainAgentEquipmentController(mission));
			list.Add(ViewCreator.CreateMissionMainAgentCheerBarkControllerView(mission));
			list.Add(ViewCreator.CreateMissionMultiplayerEscapeMenu("Captain"));
			list.Add(ViewCreator.CreateMultiplayerMissionOrderUIHandler(mission));
			list.Add(ViewCreator.CreateMissionAgentLabelUIHandler(mission));
			list.Add(ViewCreator.CreateOrderTroopPlacerView(mission));
			list.Add(ViewCreator.CreateMultiplayerTeamSelectUIHandler());
			list.Add(ViewCreator.CreateMissionScoreBoardUIHandler(mission, false));
			list.Add(ViewCreator.CreateMultiplayerEndOfRoundUIHandler());
			list.Add(ViewCreator.CreateMultiplayerEndOfBattleUIHandler());
			list.Add(ViewCreator.CreatePollProgressUIHandler());
			list.Add(new MissionItemContourControllerView());
			list.Add(new MissionAgentContourControllerView());
			list.Add(ViewCreator.CreateMultiplayerMissionHUDExtensionUIHandler());
			list.Add(ViewCreator.CreateMultiplayerMissionDeathCardUIHandler(null));
			list.Add(ViewCreator.CreateMissionFlagMarkerUIHandler());
			list.Add(ViewCreator.CreateOptionsUIHandler());
			list.Add(ViewCreator.CreateMissionMainAgentEquipDropView(mission));
			if (!GameNetwork.IsClient)
			{
				list.Add(ViewCreator.CreateMultiplayerAdminPanelUIHandler());
			}
			list.Add(ViewCreator.CreateMissionBoundaryCrossingView());
			list.Add(new MissionBoundaryWallView());
			list.Add(new SpectatorCameraView());
			return list.ToArray();
		}

		[ViewMethod("MultiplayerSkirmish")]
		public static MissionView[] OpenSkirmish(Mission mission)
		{
			List<MissionView> list = new List<MissionView>();
			list.Add(ViewCreator.CreateLobbyEquipmentUIHandler());
			list.Add(ViewCreator.CreateMissionServerStatusUIHandler());
			list.Add(ViewCreator.CreateMultiplayerFactionBanVoteUIHandler());
			list.Add(ViewCreator.CreateMissionKillNotificationUIHandler());
			list.Add(ViewCreator.CreateMissionAgentStatusUIHandler(mission));
			list.Add(ViewCreator.CreateMissionMultiplayerPreloadView(mission));
			list.Add(ViewCreator.CreateMissionMainAgentEquipmentController(mission));
			list.Add(ViewCreator.CreateMissionMainAgentCheerBarkControllerView(mission));
			list.Add(ViewCreator.CreateMissionMultiplayerEscapeMenu("Skirmish"));
			list.Add(ViewCreator.CreateMultiplayerMissionOrderUIHandler(mission));
			list.Add(ViewCreator.CreateMissionAgentLabelUIHandler(mission));
			list.Add(ViewCreator.CreateOrderTroopPlacerView(mission));
			list.Add(ViewCreator.CreateMultiplayerTeamSelectUIHandler());
			list.Add(ViewCreator.CreateMissionScoreBoardUIHandler(mission, false));
			list.Add(ViewCreator.CreateMultiplayerEndOfRoundUIHandler());
			list.Add(ViewCreator.CreateMultiplayerEndOfBattleUIHandler());
			list.Add(ViewCreator.CreatePollProgressUIHandler());
			list.Add(new MissionItemContourControllerView());
			list.Add(new MissionAgentContourControllerView());
			list.Add(ViewCreator.CreateMultiplayerMissionHUDExtensionUIHandler());
			list.Add(ViewCreator.CreateMultiplayerMissionDeathCardUIHandler(null));
			list.Add(ViewCreator.CreateMultiplayerMissionVoiceChatUIHandler());
			list.Add(ViewCreator.CreateMissionFlagMarkerUIHandler());
			list.Add(ViewCreator.CreateOptionsUIHandler());
			list.Add(ViewCreator.CreateMissionMainAgentEquipDropView(mission));
			if (!GameNetwork.IsClient)
			{
				list.Add(ViewCreator.CreateMultiplayerAdminPanelUIHandler());
			}
			list.Add(ViewCreator.CreateMissionBoundaryCrossingView());
			list.Add(new MissionBoundaryWallView());
			list.Add(new SpectatorCameraView());
			return list.ToArray();
		}
	}
}
