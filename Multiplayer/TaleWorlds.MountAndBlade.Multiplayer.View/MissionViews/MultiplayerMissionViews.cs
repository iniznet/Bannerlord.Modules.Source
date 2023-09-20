using System;
using System.Collections.Generic;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;

namespace TaleWorlds.MountAndBlade.Multiplayer.View.MissionViews
{
	[ViewCreatorModule]
	public class MultiplayerMissionViews
	{
		[ViewMethod("MultiplayerFreeForAll")]
		public static MissionView[] OpenFreeForAllMission(Mission mission)
		{
			List<MissionView> list = new List<MissionView>();
			list.Add(MultiplayerViewCreator.CreateMissionServerStatusUIHandler());
			list.Add(MultiplayerViewCreator.CreateMissionMultiplayerPreloadView(mission));
			list.Add(MultiplayerViewCreator.CreateMissionMultiplayerFFAView());
			list.Add(MultiplayerViewCreator.CreateMissionKillNotificationUIHandler());
			list.Add(ViewCreator.CreateMissionAgentStatusUIHandler(mission));
			list.Add(ViewCreator.CreateMissionMainAgentEquipmentController(mission));
			list.Add(ViewCreator.CreateMissionMainAgentCheerBarkControllerView(mission));
			list.Add(MultiplayerViewCreator.CreateMissionMultiplayerEscapeMenu("FreeForAll"));
			list.Add(MultiplayerViewCreator.CreateMultiplayerEndOfBattleUIHandler());
			list.Add(MultiplayerViewCreator.CreateMissionScoreBoardUIHandler(mission, true));
			list.Add(MultiplayerViewCreator.CreateLobbyEquipmentUIHandler());
			list.Add(MultiplayerViewCreator.CreatePollProgressUIHandler());
			list.Add(MultiplayerViewCreator.CreateMultiplayerMissionHUDExtensionUIHandler());
			list.Add(MultiplayerViewCreator.CreateMultiplayerMissionDeathCardUIHandler(null));
			list.Add(ViewCreator.CreateOptionsUIHandler());
			list.Add(ViewCreator.CreateMissionMainAgentEquipDropView(mission));
			if (!GameNetwork.IsClient)
			{
				list.Add(MultiplayerViewCreator.CreateMultiplayerAdminPanelUIHandler());
			}
			list.Add(ViewCreator.CreateMissionBoundaryCrossingView());
			list.Add(new MissionBoundaryWallView());
			return list.ToArray();
		}

		[ViewMethod("MultiplayerTeamDeathmatch")]
		public static MissionView[] OpenTeamDeathmatchMission(Mission mission)
		{
			List<MissionView> list = new List<MissionView>();
			list.Add(MultiplayerViewCreator.CreateMissionServerStatusUIHandler());
			list.Add(MultiplayerViewCreator.CreateMissionMultiplayerPreloadView(mission));
			list.Add(MultiplayerViewCreator.CreateMultiplayerTeamSelectUIHandler());
			list.Add(MultiplayerViewCreator.CreateMissionKillNotificationUIHandler());
			list.Add(ViewCreator.CreateMissionAgentStatusUIHandler(mission));
			list.Add(ViewCreator.CreateMissionMainAgentEquipmentController(mission));
			list.Add(ViewCreator.CreateMissionMainAgentCheerBarkControllerView(mission));
			list.Add(MultiplayerViewCreator.CreateMissionMultiplayerEscapeMenu("TeamDeathmatch"));
			list.Add(MultiplayerViewCreator.CreateMissionScoreBoardUIHandler(mission, false));
			list.Add(MultiplayerViewCreator.CreateMultiplayerEndOfRoundUIHandler());
			list.Add(MultiplayerViewCreator.CreateMultiplayerEndOfBattleUIHandler());
			list.Add(MultiplayerViewCreator.CreateLobbyEquipmentUIHandler());
			list.Add(ViewCreator.CreateMissionAgentLabelUIHandler(mission));
			list.Add(MultiplayerViewCreator.CreatePollProgressUIHandler());
			list.Add(MultiplayerViewCreator.CreateMissionFlagMarkerUIHandler());
			list.Add(MultiplayerViewCreator.CreateMultiplayerMissionHUDExtensionUIHandler());
			list.Add(MultiplayerViewCreator.CreateMultiplayerMissionDeathCardUIHandler(null));
			list.Add(ViewCreator.CreateOptionsUIHandler());
			list.Add(ViewCreator.CreateMissionMainAgentEquipDropView(mission));
			if (!GameNetwork.IsClient)
			{
				list.Add(MultiplayerViewCreator.CreateMultiplayerAdminPanelUIHandler());
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
			list.Add(MultiplayerViewCreator.CreateMissionServerStatusUIHandler());
			list.Add(MultiplayerViewCreator.CreateMissionMultiplayerPreloadView(mission));
			list.Add(MultiplayerViewCreator.CreateMultiplayerCultureSelectUIHandler());
			list.Add(MultiplayerViewCreator.CreateMissionKillNotificationUIHandler());
			list.Add(ViewCreator.CreateMissionAgentStatusUIHandler(mission));
			list.Add(ViewCreator.CreateMissionMainAgentEquipmentController(mission));
			list.Add(ViewCreator.CreateMissionMainAgentCheerBarkControllerView(mission));
			list.Add(MultiplayerViewCreator.CreateMissionMultiplayerEscapeMenu("Duel"));
			list.Add(MultiplayerViewCreator.CreateMultiplayerEndOfBattleUIHandler());
			list.Add(MultiplayerViewCreator.CreateMissionScoreBoardUIHandler(mission, true));
			list.Add(MultiplayerViewCreator.CreateLobbyEquipmentUIHandler());
			list.Add(MultiplayerViewCreator.CreateMissionMultiplayerDuelUI());
			list.Add(MultiplayerViewCreator.CreatePollProgressUIHandler());
			list.Add(ViewCreator.CreateOptionsUIHandler());
			list.Add(ViewCreator.CreateMissionMainAgentEquipDropView(mission));
			if (!GameNetwork.IsClient)
			{
				list.Add(MultiplayerViewCreator.CreateMultiplayerAdminPanelUIHandler());
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
			list.Add(MultiplayerViewCreator.CreateMissionServerStatusUIHandler());
			list.Add(MultiplayerViewCreator.CreateMissionMultiplayerPreloadView(mission));
			list.Add(MultiplayerViewCreator.CreateMissionKillNotificationUIHandler());
			list.Add(ViewCreator.CreateMissionAgentStatusUIHandler(mission));
			list.Add(ViewCreator.CreateMissionMainAgentEquipmentController(mission));
			list.Add(ViewCreator.CreateMissionMainAgentCheerBarkControllerView(mission));
			list.Add(MultiplayerViewCreator.CreateMissionMultiplayerEscapeMenu("Siege"));
			list.Add(MultiplayerViewCreator.CreateMultiplayerEndOfBattleUIHandler());
			list.Add(ViewCreator.CreateMissionAgentLabelUIHandler(mission));
			list.Add(MultiplayerViewCreator.CreateMultiplayerTeamSelectUIHandler());
			list.Add(MultiplayerViewCreator.CreateMissionScoreBoardUIHandler(mission, false));
			list.Add(MultiplayerViewCreator.CreateMultiplayerEndOfRoundUIHandler());
			list.Add(MultiplayerViewCreator.CreateLobbyEquipmentUIHandler());
			list.Add(MultiplayerViewCreator.CreatePollProgressUIHandler());
			list.Add(MultiplayerViewCreator.CreateMultiplayerMissionHUDExtensionUIHandler());
			list.Add(MultiplayerViewCreator.CreateMultiplayerMissionDeathCardUIHandler(null));
			list.Add(new MissionItemContourControllerView());
			list.Add(new MissionAgentContourControllerView());
			list.Add(MultiplayerViewCreator.CreateMissionFlagMarkerUIHandler());
			list.Add(ViewCreator.CreateOptionsUIHandler());
			list.Add(ViewCreator.CreateMissionMainAgentEquipDropView(mission));
			if (!GameNetwork.IsClient)
			{
				list.Add(MultiplayerViewCreator.CreateMultiplayerAdminPanelUIHandler());
			}
			list.Add(ViewCreator.CreateMissionBoundaryCrossingView());
			list.Add(new MissionBoundaryWallView());
			return list.ToArray();
		}

		[ViewMethod("MultiplayerBattle")]
		public static MissionView[] OpenBattle(Mission mission)
		{
			List<MissionView> list = new List<MissionView>();
			list.Add(MultiplayerViewCreator.CreateLobbyEquipmentUIHandler());
			list.Add(MultiplayerViewCreator.CreateMultiplayerFactionBanVoteUIHandler());
			list.Add(ViewCreator.CreateMissionAgentStatusUIHandler(mission));
			list.Add(MultiplayerViewCreator.CreateMissionMultiplayerPreloadView(mission));
			list.Add(ViewCreator.CreateMissionMainAgentEquipmentController(mission));
			list.Add(ViewCreator.CreateMissionMainAgentCheerBarkControllerView(mission));
			list.Add(MultiplayerViewCreator.CreateMissionMultiplayerEscapeMenu("Battle"));
			list.Add(MultiplayerViewCreator.CreateMultiplayerMissionOrderUIHandler(mission));
			list.Add(ViewCreator.CreateMissionAgentLabelUIHandler(mission));
			list.Add(ViewCreator.CreateOrderTroopPlacerView(mission));
			list.Add(MultiplayerViewCreator.CreateMultiplayerTeamSelectUIHandler());
			list.Add(MultiplayerViewCreator.CreateMissionScoreBoardUIHandler(mission, false));
			list.Add(MultiplayerViewCreator.CreateMultiplayerEndOfRoundUIHandler());
			list.Add(MultiplayerViewCreator.CreateMultiplayerEndOfBattleUIHandler());
			list.Add(MultiplayerViewCreator.CreatePollProgressUIHandler());
			list.Add(new MissionItemContourControllerView());
			list.Add(new MissionAgentContourControllerView());
			list.Add(MultiplayerViewCreator.CreateMissionKillNotificationUIHandler());
			list.Add(MultiplayerViewCreator.CreateMultiplayerMissionHUDExtensionUIHandler());
			list.Add(MultiplayerViewCreator.CreateMultiplayerMissionDeathCardUIHandler(null));
			list.Add(MultiplayerViewCreator.CreateMissionFlagMarkerUIHandler());
			list.Add(ViewCreator.CreateOptionsUIHandler());
			list.Add(ViewCreator.CreateMissionMainAgentEquipDropView(mission));
			if (!GameNetwork.IsClient)
			{
				list.Add(MultiplayerViewCreator.CreateMultiplayerAdminPanelUIHandler());
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
			list.Add(MultiplayerViewCreator.CreateLobbyEquipmentUIHandler());
			list.Add(MultiplayerViewCreator.CreateMissionServerStatusUIHandler());
			list.Add(MultiplayerViewCreator.CreateMultiplayerFactionBanVoteUIHandler());
			list.Add(MultiplayerViewCreator.CreateMissionMultiplayerPreloadView(mission));
			list.Add(MultiplayerViewCreator.CreateMissionKillNotificationUIHandler());
			list.Add(ViewCreator.CreateMissionAgentStatusUIHandler(mission));
			list.Add(ViewCreator.CreateMissionMainAgentEquipmentController(mission));
			list.Add(ViewCreator.CreateMissionMainAgentCheerBarkControllerView(mission));
			list.Add(MultiplayerViewCreator.CreateMissionMultiplayerEscapeMenu("Captain"));
			list.Add(MultiplayerViewCreator.CreateMultiplayerMissionOrderUIHandler(mission));
			list.Add(ViewCreator.CreateMissionAgentLabelUIHandler(mission));
			list.Add(ViewCreator.CreateOrderTroopPlacerView(mission));
			list.Add(MultiplayerViewCreator.CreateMultiplayerTeamSelectUIHandler());
			list.Add(MultiplayerViewCreator.CreateMissionScoreBoardUIHandler(mission, false));
			list.Add(MultiplayerViewCreator.CreateMultiplayerEndOfRoundUIHandler());
			list.Add(MultiplayerViewCreator.CreateMultiplayerEndOfBattleUIHandler());
			list.Add(MultiplayerViewCreator.CreatePollProgressUIHandler());
			list.Add(new MissionItemContourControllerView());
			list.Add(new MissionAgentContourControllerView());
			list.Add(MultiplayerViewCreator.CreateMultiplayerMissionHUDExtensionUIHandler());
			list.Add(MultiplayerViewCreator.CreateMultiplayerMissionDeathCardUIHandler(null));
			list.Add(MultiplayerViewCreator.CreateMissionFlagMarkerUIHandler());
			list.Add(ViewCreator.CreateOptionsUIHandler());
			list.Add(ViewCreator.CreateMissionMainAgentEquipDropView(mission));
			if (!GameNetwork.IsClient)
			{
				list.Add(MultiplayerViewCreator.CreateMultiplayerAdminPanelUIHandler());
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
			list.Add(MultiplayerViewCreator.CreateLobbyEquipmentUIHandler());
			list.Add(MultiplayerViewCreator.CreateMissionServerStatusUIHandler());
			list.Add(MultiplayerViewCreator.CreateMultiplayerFactionBanVoteUIHandler());
			list.Add(MultiplayerViewCreator.CreateMissionKillNotificationUIHandler());
			list.Add(ViewCreator.CreateMissionAgentStatusUIHandler(mission));
			list.Add(MultiplayerViewCreator.CreateMissionMultiplayerPreloadView(mission));
			list.Add(ViewCreator.CreateMissionMainAgentEquipmentController(mission));
			list.Add(ViewCreator.CreateMissionMainAgentCheerBarkControllerView(mission));
			list.Add(MultiplayerViewCreator.CreateMissionMultiplayerEscapeMenu("Skirmish"));
			list.Add(MultiplayerViewCreator.CreateMultiplayerMissionOrderUIHandler(mission));
			list.Add(ViewCreator.CreateMissionAgentLabelUIHandler(mission));
			list.Add(ViewCreator.CreateOrderTroopPlacerView(mission));
			list.Add(MultiplayerViewCreator.CreateMultiplayerTeamSelectUIHandler());
			list.Add(MultiplayerViewCreator.CreateMissionScoreBoardUIHandler(mission, false));
			list.Add(MultiplayerViewCreator.CreateMultiplayerEndOfRoundUIHandler());
			list.Add(MultiplayerViewCreator.CreateMultiplayerEndOfBattleUIHandler());
			list.Add(MultiplayerViewCreator.CreatePollProgressUIHandler());
			list.Add(new MissionItemContourControllerView());
			list.Add(new MissionAgentContourControllerView());
			list.Add(MultiplayerViewCreator.CreateMultiplayerMissionHUDExtensionUIHandler());
			list.Add(MultiplayerViewCreator.CreateMultiplayerMissionDeathCardUIHandler(null));
			list.Add(MultiplayerViewCreator.CreateMultiplayerMissionVoiceChatUIHandler());
			list.Add(MultiplayerViewCreator.CreateMissionFlagMarkerUIHandler());
			list.Add(ViewCreator.CreateOptionsUIHandler());
			list.Add(ViewCreator.CreateMissionMainAgentEquipDropView(mission));
			if (!GameNetwork.IsClient)
			{
				list.Add(MultiplayerViewCreator.CreateMultiplayerAdminPanelUIHandler());
			}
			list.Add(ViewCreator.CreateMissionBoundaryCrossingView());
			list.Add(new MissionBoundaryWallView());
			list.Add(new SpectatorCameraView());
			return list.ToArray();
		}
	}
}
