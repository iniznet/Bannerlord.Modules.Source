using System;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;

namespace TaleWorlds.MountAndBlade.Multiplayer.View.MissionViews
{
	public static class MultiplayerViewCreator
	{
		public static MissionView CreateMissionMultiplayerPreloadView(Mission mission = null)
		{
			return ViewCreatorManager.CreateMissionView<MissionMultiplayerPreloadView>(mission != null, mission, Array.Empty<object>());
		}

		public static MissionView CreateMissionScoreBoardUIHandler(Mission mission, bool isSingleTeam)
		{
			return ViewCreatorManager.CreateMissionView<MissionScoreboardUIHandler>(mission != null, mission, new object[] { isSingleTeam });
		}

		public static MissionView CreateMultiplayerEndOfRoundUIHandler()
		{
			return ViewCreatorManager.CreateMissionView<MultiplayerEndOfRoundUIHandler>(false, null, Array.Empty<object>());
		}

		public static MissionView CreateMultiplayerTeamSelectUIHandler()
		{
			return ViewCreatorManager.CreateMissionView<MultiplayerTeamSelectUIHandler>(false, null, Array.Empty<object>());
		}

		public static MissionView CreateMultiplayerCultureSelectUIHandler()
		{
			return ViewCreatorManager.CreateMissionView<MultiplayerCultureSelectUIHandler>(false, null, Array.Empty<object>());
		}

		public static MissionView CreateLobbyEquipmentUIHandler()
		{
			return ViewCreatorManager.CreateMissionView<MissionLobbyEquipmentUIHandler>(false, null, Array.Empty<object>());
		}

		public static MissionView CreatePollProgressUIHandler()
		{
			return ViewCreatorManager.CreateMissionView<MultiplayerPollProgressUIHandler>(false, null, Array.Empty<object>());
		}

		public static MissionView CreateMissionMultiplayerEscapeMenu(string gameType)
		{
			return ViewCreatorManager.CreateMissionView<MissionMultiplayerEscapeMenu>(false, null, new object[] { gameType });
		}

		public static MissionView CreateMissionKillNotificationUIHandler()
		{
			return ViewCreatorManager.CreateMissionView<MissionMultiplayerKillNotificationUIHandler>(false, null, Array.Empty<object>());
		}

		public static MissionView CreateMissionServerStatusUIHandler()
		{
			return ViewCreatorManager.CreateMissionView<MissionMultiplayerServerStatusUIHandler>(false, null, Array.Empty<object>());
		}

		public static MissionView CreateMultiplayerAdminPanelUIHandler()
		{
			return ViewCreatorManager.CreateMissionView<MultiplayerAdminPanelUIHandler>(false, null, Array.Empty<object>());
		}

		public static MissionView CreateMultiplayerFactionBanVoteUIHandler()
		{
			return ViewCreatorManager.CreateMissionView<MultiplayerFactionBanVoteUIHandler>(false, null, Array.Empty<object>());
		}

		public static MissionView CreateMultiplayerMissionHUDExtensionUIHandler()
		{
			return ViewCreatorManager.CreateMissionView<MissionMultiplayerHUDExtensionUIHandler>(false, null, Array.Empty<object>());
		}

		public static MissionView CreateMultiplayerMissionVoiceChatUIHandler()
		{
			return ViewCreatorManager.CreateMissionView<MissionMultiplayerVoiceChatUIHandler>(false, null, Array.Empty<object>());
		}

		public static MissionView CreateMultiplayerMissionOrderUIHandler(Mission mission = null)
		{
			return ViewCreatorManager.CreateMissionView<MultiplayerMissionOrderUIHandler>(mission != null, mission, Array.Empty<object>());
		}

		public static MissionView CreateMultiplayerMissionDeathCardUIHandler(Mission mission = null)
		{
			return ViewCreatorManager.CreateMissionView<MissionMultiplayerDeathCardUIHandler>(mission != null, mission, Array.Empty<object>());
		}

		public static MissionView CreateMissionMultiplayerDuelUI()
		{
			return ViewCreatorManager.CreateMissionView<MissionMultiplayerDuelUI>(false, null, Array.Empty<object>());
		}

		public static MissionView CreateMissionMultiplayerFFAView()
		{
			return ViewCreatorManager.CreateMissionView<MissionMultiplayerFreeForAllUIHandler>(false, null, Array.Empty<object>());
		}

		public static MissionView CreateMultiplayerEndOfBattleUIHandler()
		{
			return ViewCreatorManager.CreateMissionView<MultiplayerEndOfBattleUIHandler>(false, null, Array.Empty<object>());
		}

		public static MissionView CreateMissionFlagMarkerUIHandler()
		{
			return ViewCreatorManager.CreateMissionView<MissionMultiplayerMarkerUIHandler>(false, null, Array.Empty<object>());
		}
	}
}
