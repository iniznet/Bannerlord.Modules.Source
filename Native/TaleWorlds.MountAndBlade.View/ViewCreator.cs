using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.View.MissionViews.Multiplayer;
using TaleWorlds.MountAndBlade.View.MissionViews.Order;
using TaleWorlds.MountAndBlade.View.MissionViews.Singleplayer;
using TaleWorlds.MountAndBlade.View.Screens;
using TaleWorlds.MountAndBlade.ViewModelCollection.OrderOfBattle;
using TaleWorlds.MountAndBlade.ViewModelCollection.Scoreboard;
using TaleWorlds.ScreenSystem;

namespace TaleWorlds.MountAndBlade.View
{
	public static class ViewCreator
	{
		public static ScreenBase CreateCreditsScreen()
		{
			return ViewCreatorManager.CreateScreenView<CreditsScreen>();
		}

		public static ScreenBase CreateOptionsScreen(bool fromMainMenu)
		{
			return ViewCreatorManager.CreateScreenView<OptionsScreen>(new object[] { fromMainMenu });
		}

		public static ScreenBase CreateMBFaceGeneratorScreen(BasicCharacterObject character, bool openedFromMultiplayer = false, IFaceGeneratorCustomFilter filter = null)
		{
			return ViewCreatorManager.CreateScreenView<FaceGeneratorScreen>(new object[] { character, openedFromMultiplayer, filter });
		}

		public static MissionView CreateMissionAgentStatusUIHandler(Mission mission = null)
		{
			return ViewCreatorManager.CreateMissionView<MissionAgentStatusUIHandler>(mission != null, mission, Array.Empty<object>());
		}

		public static MissionView CreateMissionMainAgentEquipDropView(Mission mission)
		{
			return ViewCreatorManager.CreateMissionView<MissionMainAgentEquipDropView>(mission != null, mission, Array.Empty<object>());
		}

		public static MissionView CreateMissionSiegeEngineMarkerView(Mission mission)
		{
			return ViewCreatorManager.CreateMissionView<MissionSiegeEngineMarkerView>(mission != null, mission, Array.Empty<object>());
		}

		public static MissionView CreateMissionMultiplayerPreloadView(Mission mission = null)
		{
			return ViewCreatorManager.CreateMissionView<MissionMultiplayerPreloadView>(mission != null, mission, Array.Empty<object>());
		}

		public static MissionView CreateMissionMainAgentEquipmentController(Mission mission = null)
		{
			return ViewCreatorManager.CreateMissionView<MissionMainAgentEquipmentControllerView>(mission != null, mission, Array.Empty<object>());
		}

		public static MissionView CreateMissionMainAgentCheerBarkControllerView(Mission mission = null)
		{
			return ViewCreatorManager.CreateMissionView<MissionMainAgentCheerBarkControllerView>(mission != null, mission, Array.Empty<object>());
		}

		public static MissionView CreateMissionAgentLockVisualizerView(Mission mission = null)
		{
			return ViewCreatorManager.CreateMissionView<MissionAgentLockVisualizerView>(mission != null, mission, Array.Empty<object>());
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

		public static MissionView CreateOptionsUIHandler()
		{
			return ViewCreatorManager.CreateMissionView<MissionOptionsUIHandler>(false, null, Array.Empty<object>());
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

		public static MissionView CreateSingleplayerMissionKillNotificationUIHandler()
		{
			return ViewCreatorManager.CreateMissionView<MissionSingleplayerKillNotificationUIHandler>(false, null, Array.Empty<object>());
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

		public static MissionView CreateMissionFlagMarkerUIHandler()
		{
			return ViewCreatorManager.CreateMissionView<MissionMultiplayerMarkerUIHandler>(false, null, Array.Empty<object>());
		}

		public static MissionView CreateMissionAgentLabelUIHandler(Mission mission)
		{
			return ViewCreatorManager.CreateMissionView<MissionAgentLabelView>(mission != null, mission, Array.Empty<object>());
		}

		public static MissionView CreateMissionOrderUIHandler(Mission mission = null)
		{
			return ViewCreatorManager.CreateMissionView<MissionOrderUIHandler>(mission != null, mission, Array.Empty<object>());
		}

		public static MissionView CreateMissionOrderOfBattleUIHandler(Mission mission, OrderOfBattleVM dataSource)
		{
			return ViewCreatorManager.CreateMissionView<MissionOrderOfBattleUIHandler>(false, mission, new object[] { dataSource });
		}

		public static MissionView CreateMissionSpectatorControlView(Mission mission = null)
		{
			return ViewCreatorManager.CreateMissionView<MissionSpectatorControlView>(mission != null, mission, Array.Empty<object>());
		}

		public static MissionView CreateMissionBattleScoreUIHandler(Mission mission, ScoreboardBaseVM dataSource)
		{
			return ViewCreatorManager.CreateMissionView<MissionBattleScoreUIHandler>(false, mission, new object[] { dataSource });
		}

		public static MissionView CreateMissionBoundaryCrossingView()
		{
			return ViewCreatorManager.CreateMissionView<MissionBoundaryCrossingView>(false, null, Array.Empty<object>());
		}

		public static MissionView CreateMissionLeaveView()
		{
			return ViewCreatorManager.CreateMissionView<MissionLeaveView>(false, null, Array.Empty<object>());
		}

		public static MissionView CreatePhotoModeView()
		{
			return ViewCreatorManager.CreateMissionView<PhotoModeView>(false, null, Array.Empty<object>());
		}

		public static MissionView CreateMissionSingleplayerEscapeMenu(bool isIronmanMode)
		{
			return ViewCreatorManager.CreateMissionView<MissionSingleplayerEscapeMenu>(false, null, new object[] { isIronmanMode });
		}

		public static MissionView CreateMultiplayerMissionOrderUIHandler(Mission mission = null)
		{
			return ViewCreatorManager.CreateMissionView<MultiplayerMissionOrderUIHandler>(mission != null, mission, Array.Empty<object>());
		}

		public static MissionView CreateMultiplayerMissionDeathCardUIHandler(Mission mission = null)
		{
			return ViewCreatorManager.CreateMissionView<MissionMultiplayerDeathCardUIHandler>(mission != null, mission, Array.Empty<object>());
		}

		public static MissionView CreateOrderTroopPlacerView(Mission mission = null)
		{
			return ViewCreatorManager.CreateMissionView<OrderTroopPlacer>(mission != null, mission, Array.Empty<object>());
		}

		public static MissionView CreateMissionMultiplayerDuelUI()
		{
			return ViewCreatorManager.CreateMissionView<MissionMultiplayerDuelUI>(false, null, Array.Empty<object>());
		}

		public static MissionView CreateMissionMultiplayerFFAView()
		{
			return ViewCreatorManager.CreateMissionView<MissionMultiplayerFreeForAllUIHandler>(false, null, Array.Empty<object>());
		}

		public static MissionView CreateMissionFormationMarkerUIHandler(Mission mission = null)
		{
			return ViewCreatorManager.CreateMissionView<MissionFormationMarkerUIHandler>(mission != null, mission, Array.Empty<object>());
		}

		public static MissionView CreateMultiplayerEndOfBattleUIHandler()
		{
			return ViewCreatorManager.CreateMissionView<MultiplayerEndOfBattleUIHandler>(false, null, Array.Empty<object>());
		}
	}
}
