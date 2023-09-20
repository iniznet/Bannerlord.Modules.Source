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
	// Token: 0x02000019 RID: 25
	public static class ViewCreator
	{
		// Token: 0x060000A6 RID: 166 RVA: 0x00006F53 File Offset: 0x00005153
		public static ScreenBase CreateCreditsScreen()
		{
			return ViewCreatorManager.CreateScreenView<CreditsScreen>();
		}

		// Token: 0x060000A7 RID: 167 RVA: 0x00006F5A File Offset: 0x0000515A
		public static ScreenBase CreateOptionsScreen(bool fromMainMenu)
		{
			return ViewCreatorManager.CreateScreenView<OptionsScreen>(new object[] { fromMainMenu });
		}

		// Token: 0x060000A8 RID: 168 RVA: 0x00006F70 File Offset: 0x00005170
		public static ScreenBase CreateMBFaceGeneratorScreen(BasicCharacterObject character, bool openedFromMultiplayer = false, IFaceGeneratorCustomFilter filter = null)
		{
			return ViewCreatorManager.CreateScreenView<FaceGeneratorScreen>(new object[] { character, openedFromMultiplayer, filter });
		}

		// Token: 0x060000A9 RID: 169 RVA: 0x00006F8E File Offset: 0x0000518E
		public static MissionView CreateMissionAgentStatusUIHandler(Mission mission = null)
		{
			return ViewCreatorManager.CreateMissionView<MissionAgentStatusUIHandler>(mission != null, mission, Array.Empty<object>());
		}

		// Token: 0x060000AA RID: 170 RVA: 0x00006F9F File Offset: 0x0000519F
		public static MissionView CreateMissionMainAgentEquipDropView(Mission mission)
		{
			return ViewCreatorManager.CreateMissionView<MissionMainAgentEquipDropView>(mission != null, mission, Array.Empty<object>());
		}

		// Token: 0x060000AB RID: 171 RVA: 0x00006FB0 File Offset: 0x000051B0
		public static MissionView CreateMissionSiegeEngineMarkerView(Mission mission)
		{
			return ViewCreatorManager.CreateMissionView<MissionSiegeEngineMarkerView>(mission != null, mission, Array.Empty<object>());
		}

		// Token: 0x060000AC RID: 172 RVA: 0x00006FC1 File Offset: 0x000051C1
		public static MissionView CreateMissionMultiplayerPreloadView(Mission mission = null)
		{
			return ViewCreatorManager.CreateMissionView<MissionMultiplayerPreloadView>(mission != null, mission, Array.Empty<object>());
		}

		// Token: 0x060000AD RID: 173 RVA: 0x00006FD2 File Offset: 0x000051D2
		public static MissionView CreateMissionMainAgentEquipmentController(Mission mission = null)
		{
			return ViewCreatorManager.CreateMissionView<MissionMainAgentEquipmentControllerView>(mission != null, mission, Array.Empty<object>());
		}

		// Token: 0x060000AE RID: 174 RVA: 0x00006FE3 File Offset: 0x000051E3
		public static MissionView CreateMissionMainAgentCheerBarkControllerView(Mission mission = null)
		{
			return ViewCreatorManager.CreateMissionView<MissionMainAgentCheerBarkControllerView>(mission != null, mission, Array.Empty<object>());
		}

		// Token: 0x060000AF RID: 175 RVA: 0x00006FF4 File Offset: 0x000051F4
		public static MissionView CreateMissionAgentLockVisualizerView(Mission mission = null)
		{
			return ViewCreatorManager.CreateMissionView<MissionAgentLockVisualizerView>(mission != null, mission, Array.Empty<object>());
		}

		// Token: 0x060000B0 RID: 176 RVA: 0x00007005 File Offset: 0x00005205
		public static MissionView CreateMissionScoreBoardUIHandler(Mission mission, bool isSingleTeam)
		{
			return ViewCreatorManager.CreateMissionView<MissionScoreboardUIHandler>(mission != null, mission, new object[] { isSingleTeam });
		}

		// Token: 0x060000B1 RID: 177 RVA: 0x00007020 File Offset: 0x00005220
		public static MissionView CreateMultiplayerEndOfRoundUIHandler()
		{
			return ViewCreatorManager.CreateMissionView<MultiplayerEndOfRoundUIHandler>(false, null, Array.Empty<object>());
		}

		// Token: 0x060000B2 RID: 178 RVA: 0x0000702E File Offset: 0x0000522E
		public static MissionView CreateMultiplayerTeamSelectUIHandler()
		{
			return ViewCreatorManager.CreateMissionView<MultiplayerTeamSelectUIHandler>(false, null, Array.Empty<object>());
		}

		// Token: 0x060000B3 RID: 179 RVA: 0x0000703C File Offset: 0x0000523C
		public static MissionView CreateMultiplayerCultureSelectUIHandler()
		{
			return ViewCreatorManager.CreateMissionView<MultiplayerCultureSelectUIHandler>(false, null, Array.Empty<object>());
		}

		// Token: 0x060000B4 RID: 180 RVA: 0x0000704A File Offset: 0x0000524A
		public static MissionView CreateLobbyEquipmentUIHandler()
		{
			return ViewCreatorManager.CreateMissionView<MissionLobbyEquipmentUIHandler>(false, null, Array.Empty<object>());
		}

		// Token: 0x060000B5 RID: 181 RVA: 0x00007058 File Offset: 0x00005258
		public static MissionView CreateOptionsUIHandler()
		{
			return ViewCreatorManager.CreateMissionView<MissionOptionsUIHandler>(false, null, Array.Empty<object>());
		}

		// Token: 0x060000B6 RID: 182 RVA: 0x00007066 File Offset: 0x00005266
		public static MissionView CreatePollProgressUIHandler()
		{
			return ViewCreatorManager.CreateMissionView<MultiplayerPollProgressUIHandler>(false, null, Array.Empty<object>());
		}

		// Token: 0x060000B7 RID: 183 RVA: 0x00007074 File Offset: 0x00005274
		public static MissionView CreateMissionMultiplayerEscapeMenu(string gameType)
		{
			return ViewCreatorManager.CreateMissionView<MissionMultiplayerEscapeMenu>(false, null, new object[] { gameType });
		}

		// Token: 0x060000B8 RID: 184 RVA: 0x00007087 File Offset: 0x00005287
		public static MissionView CreateMissionKillNotificationUIHandler()
		{
			return ViewCreatorManager.CreateMissionView<MissionMultiplayerKillNotificationUIHandler>(false, null, Array.Empty<object>());
		}

		// Token: 0x060000B9 RID: 185 RVA: 0x00007095 File Offset: 0x00005295
		public static MissionView CreateMissionServerStatusUIHandler()
		{
			return ViewCreatorManager.CreateMissionView<MissionMultiplayerServerStatusUIHandler>(false, null, Array.Empty<object>());
		}

		// Token: 0x060000BA RID: 186 RVA: 0x000070A3 File Offset: 0x000052A3
		public static MissionView CreateSingleplayerMissionKillNotificationUIHandler()
		{
			return ViewCreatorManager.CreateMissionView<MissionSingleplayerKillNotificationUIHandler>(false, null, Array.Empty<object>());
		}

		// Token: 0x060000BB RID: 187 RVA: 0x000070B1 File Offset: 0x000052B1
		public static MissionView CreateMultiplayerAdminPanelUIHandler()
		{
			return ViewCreatorManager.CreateMissionView<MultiplayerAdminPanelUIHandler>(false, null, Array.Empty<object>());
		}

		// Token: 0x060000BC RID: 188 RVA: 0x000070BF File Offset: 0x000052BF
		public static MissionView CreateMultiplayerFactionBanVoteUIHandler()
		{
			return ViewCreatorManager.CreateMissionView<MultiplayerFactionBanVoteUIHandler>(false, null, Array.Empty<object>());
		}

		// Token: 0x060000BD RID: 189 RVA: 0x000070CD File Offset: 0x000052CD
		public static MissionView CreateMultiplayerMissionHUDExtensionUIHandler()
		{
			return ViewCreatorManager.CreateMissionView<MissionMultiplayerHUDExtensionUIHandler>(false, null, Array.Empty<object>());
		}

		// Token: 0x060000BE RID: 190 RVA: 0x000070DB File Offset: 0x000052DB
		public static MissionView CreateMultiplayerMissionVoiceChatUIHandler()
		{
			return ViewCreatorManager.CreateMissionView<MissionMultiplayerVoiceChatUIHandler>(false, null, Array.Empty<object>());
		}

		// Token: 0x060000BF RID: 191 RVA: 0x000070E9 File Offset: 0x000052E9
		public static MissionView CreateMissionFlagMarkerUIHandler()
		{
			return ViewCreatorManager.CreateMissionView<MissionMultiplayerMarkerUIHandler>(false, null, Array.Empty<object>());
		}

		// Token: 0x060000C0 RID: 192 RVA: 0x000070F7 File Offset: 0x000052F7
		public static MissionView CreateMissionAgentLabelUIHandler(Mission mission)
		{
			return ViewCreatorManager.CreateMissionView<MissionAgentLabelView>(mission != null, mission, Array.Empty<object>());
		}

		// Token: 0x060000C1 RID: 193 RVA: 0x00007108 File Offset: 0x00005308
		public static MissionView CreateMissionOrderUIHandler(Mission mission = null)
		{
			return ViewCreatorManager.CreateMissionView<MissionOrderUIHandler>(mission != null, mission, Array.Empty<object>());
		}

		// Token: 0x060000C2 RID: 194 RVA: 0x00007119 File Offset: 0x00005319
		public static MissionView CreateMissionOrderOfBattleUIHandler(Mission mission, OrderOfBattleVM dataSource)
		{
			return ViewCreatorManager.CreateMissionView<MissionOrderOfBattleUIHandler>(false, mission, new object[] { dataSource });
		}

		// Token: 0x060000C3 RID: 195 RVA: 0x0000712C File Offset: 0x0000532C
		public static MissionView CreateMissionSpectatorControlView(Mission mission = null)
		{
			return ViewCreatorManager.CreateMissionView<MissionSpectatorControlView>(mission != null, mission, Array.Empty<object>());
		}

		// Token: 0x060000C4 RID: 196 RVA: 0x0000713D File Offset: 0x0000533D
		public static MissionView CreateMissionBattleScoreUIHandler(Mission mission, ScoreboardBaseVM dataSource)
		{
			return ViewCreatorManager.CreateMissionView<MissionBattleScoreUIHandler>(false, mission, new object[] { dataSource });
		}

		// Token: 0x060000C5 RID: 197 RVA: 0x00007150 File Offset: 0x00005350
		public static MissionView CreateMissionBoundaryCrossingView()
		{
			return ViewCreatorManager.CreateMissionView<MissionBoundaryCrossingView>(false, null, Array.Empty<object>());
		}

		// Token: 0x060000C6 RID: 198 RVA: 0x0000715E File Offset: 0x0000535E
		public static MissionView CreateMissionLeaveView()
		{
			return ViewCreatorManager.CreateMissionView<MissionLeaveView>(false, null, Array.Empty<object>());
		}

		// Token: 0x060000C7 RID: 199 RVA: 0x0000716C File Offset: 0x0000536C
		public static MissionView CreatePhotoModeView()
		{
			return ViewCreatorManager.CreateMissionView<PhotoModeView>(false, null, Array.Empty<object>());
		}

		// Token: 0x060000C8 RID: 200 RVA: 0x0000717A File Offset: 0x0000537A
		public static MissionView CreateMissionSingleplayerEscapeMenu(bool isIronmanMode)
		{
			return ViewCreatorManager.CreateMissionView<MissionSingleplayerEscapeMenu>(false, null, new object[] { isIronmanMode });
		}

		// Token: 0x060000C9 RID: 201 RVA: 0x00007192 File Offset: 0x00005392
		public static MissionView CreateMultiplayerMissionOrderUIHandler(Mission mission = null)
		{
			return ViewCreatorManager.CreateMissionView<MultiplayerMissionOrderUIHandler>(mission != null, mission, Array.Empty<object>());
		}

		// Token: 0x060000CA RID: 202 RVA: 0x000071A3 File Offset: 0x000053A3
		public static MissionView CreateMultiplayerMissionDeathCardUIHandler(Mission mission = null)
		{
			return ViewCreatorManager.CreateMissionView<MissionMultiplayerDeathCardUIHandler>(mission != null, mission, Array.Empty<object>());
		}

		// Token: 0x060000CB RID: 203 RVA: 0x000071B4 File Offset: 0x000053B4
		public static MissionView CreateOrderTroopPlacerView(Mission mission = null)
		{
			return ViewCreatorManager.CreateMissionView<OrderTroopPlacer>(mission != null, mission, Array.Empty<object>());
		}

		// Token: 0x060000CC RID: 204 RVA: 0x000071C5 File Offset: 0x000053C5
		public static MissionView CreateMissionMultiplayerDuelUI()
		{
			return ViewCreatorManager.CreateMissionView<MissionMultiplayerDuelUI>(false, null, Array.Empty<object>());
		}

		// Token: 0x060000CD RID: 205 RVA: 0x000071D3 File Offset: 0x000053D3
		public static MissionView CreateMissionMultiplayerFFAView()
		{
			return ViewCreatorManager.CreateMissionView<MissionMultiplayerFreeForAllUIHandler>(false, null, Array.Empty<object>());
		}

		// Token: 0x060000CE RID: 206 RVA: 0x000071E1 File Offset: 0x000053E1
		public static MissionView CreateMissionFormationMarkerUIHandler(Mission mission = null)
		{
			return ViewCreatorManager.CreateMissionView<MissionFormationMarkerUIHandler>(mission != null, mission, Array.Empty<object>());
		}

		// Token: 0x060000CF RID: 207 RVA: 0x000071F2 File Offset: 0x000053F2
		public static MissionView CreateMultiplayerEndOfBattleUIHandler()
		{
			return ViewCreatorManager.CreateMissionView<MultiplayerEndOfBattleUIHandler>(false, null, Array.Empty<object>());
		}
	}
}
