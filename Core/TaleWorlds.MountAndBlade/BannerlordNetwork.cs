using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Diamond;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020002E4 RID: 740
	public static class BannerlordNetwork
	{
		// Token: 0x0600286C RID: 10348 RVA: 0x0009C700 File Offset: 0x0009A900
		private static PlayerConnectionInfo CreateServerPeerConnectionInfo()
		{
			LobbyClient gameClient = NetworkMain.GameClient;
			PlayerConnectionInfo playerConnectionInfo = new PlayerConnectionInfo(gameClient.PlayerID);
			PlayerData playerData = gameClient.PlayerData;
			playerConnectionInfo.AddParameter("PlayerData", playerData);
			playerConnectionInfo.AddParameter("UsedCosmetics", gameClient.UsedCosmetics);
			playerConnectionInfo.Name = gameClient.Name;
			return playerConnectionInfo;
		}

		// Token: 0x0600286D RID: 10349 RVA: 0x0009C74E File Offset: 0x0009A94E
		public static void CreateServerPeer()
		{
			if (MBCommon.CurrentGameType == MBCommon.GameType.MultiClientServer)
			{
				GameNetwork.AddNewPlayerOnServer(BannerlordNetwork.CreateServerPeerConnectionInfo(), true, true);
			}
		}

		// Token: 0x0600286E RID: 10350 RVA: 0x0009C765 File Offset: 0x0009A965
		public static void StartMultiplayerLobbyMission(LobbyMissionType lobbyMissionType)
		{
			BannerlordNetwork.LobbyMissionType = lobbyMissionType;
		}

		// Token: 0x0600286F RID: 10351 RVA: 0x0009C770 File Offset: 0x0009A970
		public static void EndMultiplayerLobbyMission()
		{
			MissionState missionState = Game.Current.GameStateManager.ActiveState as MissionState;
			if (missionState != null && missionState.CurrentMission != null && !missionState.CurrentMission.MissionEnded)
			{
				if (missionState.CurrentMission.CurrentState != Mission.State.Continuing)
				{
					Debug.Print("Remove From Game: Begin delayed disconnect from server.".ToUpper(), 0, Debug.DebugColor.White, 17179869184UL);
					missionState.BeginDelayedDisconnectFromMission();
				}
				else
				{
					Debug.Print("Remove From Game: Begin instant disconnect from server.".ToUpper(), 0, Debug.DebugColor.White, 17179869184UL);
					missionState.CurrentMission.EndMission();
				}
				MBDebug.Print("Starting to clean up the current mission now.", 0, Debug.DebugColor.White, 17179869184UL);
			}
			ChatBox gameHandler = Game.Current.GetGameHandler<ChatBox>();
			if (gameHandler != null)
			{
				gameHandler.ResetMuteList();
			}
		}

		// Token: 0x1700074C RID: 1868
		// (get) Token: 0x06002870 RID: 10352 RVA: 0x0009C82E File Offset: 0x0009AA2E
		// (set) Token: 0x06002871 RID: 10353 RVA: 0x0009C835 File Offset: 0x0009AA35
		public static LobbyMissionType LobbyMissionType { get; private set; }

		// Token: 0x04000ED3 RID: 3795
		public const int DefaultPort = 9999;
	}
}
