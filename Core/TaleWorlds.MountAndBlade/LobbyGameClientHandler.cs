using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Diamond.ChatSystem.Library;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.MountAndBlade.Diamond.Ranked;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200031F RID: 799
	public class LobbyGameClientHandler : ILobbyClientSessionHandler
	{
		// Token: 0x06002B0E RID: 11022 RVA: 0x000A8B03 File Offset: 0x000A6D03
		void ILobbyClientSessionHandler.OnConnected()
		{
		}

		// Token: 0x06002B0F RID: 11023 RVA: 0x000A8B05 File Offset: 0x000A6D05
		void ILobbyClientSessionHandler.OnCantConnect()
		{
		}

		// Token: 0x06002B10 RID: 11024 RVA: 0x000A8B07 File Offset: 0x000A6D07
		void ILobbyClientSessionHandler.OnDisconnected(TextObject feedback)
		{
			if (this.LobbyState != null)
			{
				this.LobbyState.OnDisconnected(feedback);
			}
		}

		// Token: 0x06002B11 RID: 11025 RVA: 0x000A8B1D File Offset: 0x000A6D1D
		void ILobbyClientSessionHandler.OnPlayerDataReceived(PlayerData playerData)
		{
			if (this.LobbyState != null)
			{
				this.LobbyState.OnPlayerDataReceived(playerData);
			}
		}

		// Token: 0x06002B12 RID: 11026 RVA: 0x000A8B33 File Offset: 0x000A6D33
		void ILobbyClientSessionHandler.OnPendingRejoin()
		{
			LobbyState lobbyState = this.LobbyState;
			if (lobbyState == null)
			{
				return;
			}
			lobbyState.OnPendingRejoin();
		}

		// Token: 0x06002B13 RID: 11027 RVA: 0x000A8B45 File Offset: 0x000A6D45
		void ILobbyClientSessionHandler.OnBattleResultReceived()
		{
		}

		// Token: 0x06002B14 RID: 11028 RVA: 0x000A8B47 File Offset: 0x000A6D47
		void ILobbyClientSessionHandler.OnCancelJoiningBattle()
		{
			if (this.LobbyState != null)
			{
				this.LobbyState.OnCancelFindingGame();
			}
		}

		// Token: 0x06002B15 RID: 11029 RVA: 0x000A8B5C File Offset: 0x000A6D5C
		void ILobbyClientSessionHandler.OnRejoinRequestRejected()
		{
		}

		// Token: 0x06002B16 RID: 11030 RVA: 0x000A8B5E File Offset: 0x000A6D5E
		void ILobbyClientSessionHandler.OnFindGameAnswer(bool successful, string[] selectedAndEnabledGameTypes, bool isRejoin)
		{
			if (successful && this.LobbyState != null)
			{
				this.LobbyState.OnUpdateFindingGame(MatchmakingWaitTimeStats.Empty, selectedAndEnabledGameTypes);
			}
		}

		// Token: 0x06002B17 RID: 11031 RVA: 0x000A8B7C File Offset: 0x000A6D7C
		void ILobbyClientSessionHandler.OnEnterBattleWithPartyAnswer(string[] selectedGameTypes)
		{
			if (this.LobbyState != null)
			{
				this.LobbyState.OnUpdateFindingGame(MatchmakingWaitTimeStats.Empty, selectedGameTypes);
			}
		}

		// Token: 0x06002B18 RID: 11032 RVA: 0x000A8B97 File Offset: 0x000A6D97
		void ILobbyClientSessionHandler.OnWhisperMessageReceived(string fromPlayer, string toPlayer, string message)
		{
			if (this.ChatHandler != null)
			{
				this.ChatHandler.ReceiveChatMessage(ChatChannelType.NaN, fromPlayer, message);
			}
			ChatBox.AddWhisperMessage(fromPlayer, message);
		}

		// Token: 0x06002B19 RID: 11033 RVA: 0x000A8BB6 File Offset: 0x000A6DB6
		void ILobbyClientSessionHandler.OnClanMessageReceived(string playerName, string message)
		{
		}

		// Token: 0x06002B1A RID: 11034 RVA: 0x000A8BB8 File Offset: 0x000A6DB8
		void ILobbyClientSessionHandler.OnChannelMessageReceived(ChatChannelType channel, string playerName, string message)
		{
			if (this.ChatHandler != null)
			{
				this.ChatHandler.ReceiveChatMessage(channel, playerName, message);
			}
			ChatBox.AddWhisperMessage(playerName, message);
		}

		// Token: 0x06002B1B RID: 11035 RVA: 0x000A8BD7 File Offset: 0x000A6DD7
		void ILobbyClientSessionHandler.OnPartyMessageReceived(string playerName, string message)
		{
		}

		// Token: 0x06002B1C RID: 11036 RVA: 0x000A8BD9 File Offset: 0x000A6DD9
		void ILobbyClientSessionHandler.OnSystemMessageReceived(string message)
		{
			InformationManager.DisplayMessage(new InformationMessage(message));
		}

		// Token: 0x06002B1D RID: 11037 RVA: 0x000A8BE6 File Offset: 0x000A6DE6
		void ILobbyClientSessionHandler.OnAdminMessageReceived(string message)
		{
			if (this.LobbyState != null)
			{
				this.LobbyState.OnAdminMessageReceived(message);
			}
		}

		// Token: 0x06002B1E RID: 11038 RVA: 0x000A8BFC File Offset: 0x000A6DFC
		void ILobbyClientSessionHandler.OnPartyInvitationReceived(string inviterPlayerName, PlayerId inviterPlayerId)
		{
			if (this.LobbyState != null)
			{
				this.LobbyState.OnPartyInvitationReceived(inviterPlayerName, inviterPlayerId);
			}
		}

		// Token: 0x06002B1F RID: 11039 RVA: 0x000A8C13 File Offset: 0x000A6E13
		void ILobbyClientSessionHandler.OnPartyInvitationInvalidated()
		{
			if (this.LobbyState != null)
			{
				this.LobbyState.OnPartyInvitationInvalidated();
			}
		}

		// Token: 0x06002B20 RID: 11040 RVA: 0x000A8C28 File Offset: 0x000A6E28
		void ILobbyClientSessionHandler.OnPlayerInvitedToParty(PlayerId playerId)
		{
			if (this.LobbyState != null)
			{
				this.LobbyState.OnPlayerInvitedToParty(playerId);
			}
		}

		// Token: 0x06002B21 RID: 11041 RVA: 0x000A8C3E File Offset: 0x000A6E3E
		void ILobbyClientSessionHandler.OnPlayersAddedToParty([TupleElementNames(new string[] { "PlayerId", "PlayerName", "IsPartyLeader" })] List<ValueTuple<PlayerId, string, bool>> addedPlayers, [TupleElementNames(new string[] { "PlayerId", "PlayerName" })] List<ValueTuple<PlayerId, string>> invitedPlayers)
		{
			if (this.LobbyState != null)
			{
				this.LobbyState.OnPlayersAddedToParty(addedPlayers, invitedPlayers);
			}
		}

		// Token: 0x06002B22 RID: 11042 RVA: 0x000A8C55 File Offset: 0x000A6E55
		void ILobbyClientSessionHandler.OnPlayerRemovedFromParty(PlayerId playerId, PartyRemoveReason reason)
		{
			if (this.LobbyState != null)
			{
				this.LobbyState.OnPlayerRemovedFromParty(playerId, reason);
			}
		}

		// Token: 0x06002B23 RID: 11043 RVA: 0x000A8C6C File Offset: 0x000A6E6C
		void ILobbyClientSessionHandler.OnPlayerAssignedPartyLeader(PlayerId partyLeaderId)
		{
			if (this.LobbyState != null)
			{
				this.LobbyState.OnPlayerAssignedPartyLeader(partyLeaderId);
			}
		}

		// Token: 0x06002B24 RID: 11044 RVA: 0x000A8C82 File Offset: 0x000A6E82
		void ILobbyClientSessionHandler.OnPlayerSuggestedToParty(PlayerId playerId, string playerName, PlayerId suggestingPlayerId, string suggestingPlayerName)
		{
			if (this.LobbyState != null)
			{
				this.LobbyState.OnPlayerSuggestedToParty(playerId, playerName, suggestingPlayerId, suggestingPlayerName);
			}
		}

		// Token: 0x06002B25 RID: 11045 RVA: 0x000A8C9C File Offset: 0x000A6E9C
		void ILobbyClientSessionHandler.OnServerStatusReceived(ServerStatus serverStatus)
		{
			if (this.LobbyState != null)
			{
				this.LobbyState.OnServerStatusReceived(serverStatus);
			}
		}

		// Token: 0x06002B26 RID: 11046 RVA: 0x000A8CB2 File Offset: 0x000A6EB2
		void ILobbyClientSessionHandler.OnFriendListReceived(FriendInfo[] friends)
		{
			if (this.LobbyState != null)
			{
				this.LobbyState.OnFriendListReceived(friends);
			}
		}

		// Token: 0x06002B27 RID: 11047 RVA: 0x000A8CC8 File Offset: 0x000A6EC8
		void ILobbyClientSessionHandler.OnRecentPlayerStatusesReceived(FriendInfo[] friends)
		{
			if (this.LobbyState != null)
			{
				this.LobbyState.OnRecentPlayerStatusesReceived(friends);
			}
		}

		// Token: 0x06002B28 RID: 11048 RVA: 0x000A8CDE File Offset: 0x000A6EDE
		void ILobbyClientSessionHandler.OnClanInvitationReceived(string clanName, string clanTag, bool isCreation)
		{
			if (this.LobbyState != null)
			{
				this.LobbyState.OnClanInvitationReceived(clanName, clanTag, isCreation);
			}
		}

		// Token: 0x06002B29 RID: 11049 RVA: 0x000A8CF6 File Offset: 0x000A6EF6
		void ILobbyClientSessionHandler.OnClanInvitationAnswered(PlayerId playerId, ClanCreationAnswer answer)
		{
			if (this.LobbyState != null)
			{
				this.LobbyState.OnClanInvitationAnswered(playerId, answer);
			}
		}

		// Token: 0x06002B2A RID: 11050 RVA: 0x000A8D0D File Offset: 0x000A6F0D
		void ILobbyClientSessionHandler.OnClanCreationSuccessful()
		{
			if (this.LobbyState != null)
			{
				this.LobbyState.OnClanCreationSuccessful();
			}
		}

		// Token: 0x06002B2B RID: 11051 RVA: 0x000A8D22 File Offset: 0x000A6F22
		void ILobbyClientSessionHandler.OnClanCreationFailed()
		{
			if (this.LobbyState != null)
			{
				this.LobbyState.OnClanCreationFailed();
			}
		}

		// Token: 0x06002B2C RID: 11052 RVA: 0x000A8D37 File Offset: 0x000A6F37
		void ILobbyClientSessionHandler.OnClanCreationStarted()
		{
			if (this.LobbyState != null)
			{
				this.LobbyState.OnClanCreationStarted();
			}
		}

		// Token: 0x06002B2D RID: 11053 RVA: 0x000A8D4C File Offset: 0x000A6F4C
		void ILobbyClientSessionHandler.OnClanInfoChanged()
		{
			if (this.LobbyState != null)
			{
				this.LobbyState.OnClanInfoChanged();
			}
		}

		// Token: 0x06002B2E RID: 11054 RVA: 0x000A8D61 File Offset: 0x000A6F61
		void ILobbyClientSessionHandler.OnPremadeGameEligibilityStatusReceived(bool isEligible)
		{
			if (this.LobbyState != null)
			{
				this.LobbyState.OnPremadeGameEligibilityStatusReceived(isEligible);
			}
		}

		// Token: 0x06002B2F RID: 11055 RVA: 0x000A8D77 File Offset: 0x000A6F77
		void ILobbyClientSessionHandler.OnPremadeGameCreated()
		{
			if (this.LobbyState != null)
			{
				this.LobbyState.OnPremadeGameCreated();
			}
		}

		// Token: 0x06002B30 RID: 11056 RVA: 0x000A8D8C File Offset: 0x000A6F8C
		void ILobbyClientSessionHandler.OnPremadeGameListReceived()
		{
			if (this.LobbyState != null)
			{
				this.LobbyState.OnPremadeGameListReceived();
			}
		}

		// Token: 0x06002B31 RID: 11057 RVA: 0x000A8DA1 File Offset: 0x000A6FA1
		void ILobbyClientSessionHandler.OnPremadeGameCreationCancelled()
		{
			if (this.LobbyState != null)
			{
				this.LobbyState.OnPremadeGameCreationCancelled();
			}
		}

		// Token: 0x06002B32 RID: 11058 RVA: 0x000A8DB6 File Offset: 0x000A6FB6
		void ILobbyClientSessionHandler.OnJoinPremadeGameRequested(string clanName, string clanSigilCode, Guid partyId, PlayerId[] challengerPlayerIDs, PlayerId challengerPartyLeaderID, PremadeGameType premadeGameType)
		{
			if (this.LobbyState != null)
			{
				this.LobbyState.OnJoinPremadeGameRequested(clanName, clanSigilCode, partyId, challengerPlayerIDs, challengerPartyLeaderID, premadeGameType);
			}
		}

		// Token: 0x06002B33 RID: 11059 RVA: 0x000A8DD4 File Offset: 0x000A6FD4
		void ILobbyClientSessionHandler.OnJoinPremadeGameRequestSuccessful()
		{
			if (this.LobbyState != null)
			{
				this.LobbyState.OnJoinPremadeGameRequestSuccessful();
			}
		}

		// Token: 0x06002B34 RID: 11060 RVA: 0x000A8DE9 File Offset: 0x000A6FE9
		void ILobbyClientSessionHandler.OnSigilChanged()
		{
			if (this.LobbyState != null)
			{
				this.LobbyState.OnSigilChanged();
			}
		}

		// Token: 0x06002B35 RID: 11061 RVA: 0x000A8DFE File Offset: 0x000A6FFE
		void ILobbyClientSessionHandler.OnNotificationsReceived(LobbyNotification[] notifications)
		{
			if (this.LobbyState != null)
			{
				this.LobbyState.OnNotificationsReceived(notifications);
			}
		}

		// Token: 0x06002B36 RID: 11062 RVA: 0x000A8E14 File Offset: 0x000A7014
		void ILobbyClientSessionHandler.OnGameClientStateChange(LobbyClient.State oldState)
		{
			this.HandleGameClientStateChange(oldState);
		}

		// Token: 0x06002B37 RID: 11063 RVA: 0x000A8E20 File Offset: 0x000A7020
		private async void HandleGameClientStateChange(LobbyClient.State oldState)
		{
			LobbyClient gameClient = NetworkMain.GameClient;
			Debug.Print(string.Concat(new object[] { "[][] New MBGameClient State: ", gameClient.CurrentState, " old state:", oldState }), 0, Debug.DebugColor.White, 17592186044416UL);
			switch (gameClient.CurrentState)
			{
			case LobbyClient.State.Idle:
				if (oldState == LobbyClient.State.AtBattle || oldState == LobbyClient.State.HostingCustomGame || oldState == LobbyClient.State.InCustomGame)
				{
					if (Mission.Current != null && !(Game.Current.GameStateManager.ActiveState is MissionState))
					{
						Game.Current.GameStateManager.PopState(0);
					}
					if (Game.Current.GameStateManager.ActiveState is LobbyGameStateCustomGameClient)
					{
						Game.Current.GameStateManager.PopState(0);
					}
					if (Game.Current.GameStateManager.ActiveState is MissionState)
					{
						MissionState missionSystem = (MissionState)Game.Current.GameStateManager.ActiveState;
						while (missionSystem.CurrentMission.CurrentState == Mission.State.NewlyCreated || missionSystem.CurrentMission.CurrentState == Mission.State.Initializing)
						{
							await Task.Delay(1);
						}
						for (int i = 0; i < 3; i++)
						{
							await Task.Delay(1);
						}
						BannerlordNetwork.EndMultiplayerLobbyMission();
						missionSystem = null;
					}
					while (Mission.Current != null)
					{
						await Task.Delay(1);
					}
					this.LobbyState.SetConnectionState(false);
				}
				else if (oldState == LobbyClient.State.AtLobby || oldState == LobbyClient.State.SearchingBattle)
				{
					this.LobbyState.SetConnectionState(false);
				}
				else if (oldState == LobbyClient.State.WaitingToJoinCustomGame)
				{
					this.LobbyState.SetConnectionState(false);
				}
				else if (oldState == LobbyClient.State.Working)
				{
					this.LobbyState.SetConnectionState(false);
				}
				else if (oldState == LobbyClient.State.SessionRequested)
				{
					this.LobbyState.SetConnectionState(false);
				}
				else if (oldState == LobbyClient.State.Connected)
				{
					this.LobbyState.SetConnectionState(false);
				}
				else
				{
					Debug.FailedAssert("Unexpected old state:" + oldState, "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Network\\LobbyGameClientHandler.cs", "HandleGameClientStateChange", 415);
				}
				break;
			case LobbyClient.State.AtLobby:
				this.LobbyState.SetConnectionState(true);
				break;
			case LobbyClient.State.RequestingToSearchBattle:
				this.LobbyState.OnRequestedToSearchBattle();
				break;
			case LobbyClient.State.RequestingToCancelSearchBattle:
				this.LobbyState.OnRequestedToCancelSearchBattle();
				break;
			}
			this.LobbyState.OnGameClientStateChange(gameClient.CurrentState);
		}

		// Token: 0x06002B38 RID: 11064 RVA: 0x000A8E61 File Offset: 0x000A7061
		void ILobbyClientSessionHandler.OnCustomGameServerListReceived(AvailableCustomGames customGameServerList)
		{
			this.LobbyState.OnCustomGameServerListReceived(customGameServerList);
		}

		// Token: 0x06002B39 RID: 11065 RVA: 0x000A8E70 File Offset: 0x000A7070
		void ILobbyClientSessionHandler.OnMatchmakerGameOver(int oldExperience, int newExperience, List<string> badgesEarned, int lootGained, RankBarInfo oldRankBarInfo, RankBarInfo newRankBarInfo)
		{
			GameStateManager gameStateManager = Game.Current.GameStateManager;
			if (!(gameStateManager.ActiveState is LobbyState))
			{
				if (gameStateManager.ActiveState is MissionState)
				{
					BannerlordNetwork.EndMultiplayerLobbyMission();
				}
				else
				{
					gameStateManager.PopState(0);
				}
			}
			this.LobbyState.OnMatchmakerGameOver(oldExperience, newExperience, badgesEarned, lootGained, oldRankBarInfo, newRankBarInfo);
		}

		// Token: 0x06002B3A RID: 11066 RVA: 0x000A8EC4 File Offset: 0x000A70C4
		void ILobbyClientSessionHandler.OnQuitFromMatchmakerGame()
		{
			GameStateManager gameStateManager = Game.Current.GameStateManager;
			if (!(gameStateManager.ActiveState is LobbyState))
			{
				if (gameStateManager.ActiveState is MissionState)
				{
					BannerlordNetwork.EndMultiplayerLobbyMission();
					return;
				}
				gameStateManager.PopState(0);
			}
		}

		// Token: 0x06002B3B RID: 11067 RVA: 0x000A8F04 File Offset: 0x000A7104
		void ILobbyClientSessionHandler.OnBattleServerInformationReceived(BattleServerInformationForClient battleServerInformation)
		{
			if (this.LobbyState != null)
			{
				this.LobbyState.OnBattleServerInformationReceived(battleServerInformation);
			}
			LobbyGameStateMatchmakerClient lobbyGameStateMatchmakerClient = Game.Current.GameStateManager.CreateState<LobbyGameStateMatchmakerClient>();
			lobbyGameStateMatchmakerClient.SetStartingParameters(this, battleServerInformation.PeerIndex, battleServerInformation.SessionKey, battleServerInformation.ServerAddress, (int)battleServerInformation.ServerPort, battleServerInformation.GameType, battleServerInformation.SceneName);
			Game.Current.GameStateManager.PushState(lobbyGameStateMatchmakerClient, 0);
		}

		// Token: 0x06002B3C RID: 11068 RVA: 0x000A8F78 File Offset: 0x000A7178
		void ILobbyClientSessionHandler.OnBattleServerLost()
		{
			GameStateManager gameStateManager = Game.Current.GameStateManager;
			if (!(gameStateManager.ActiveState is LobbyState))
			{
				if (gameStateManager.ActiveState is MissionState)
				{
					BannerlordNetwork.EndMultiplayerLobbyMission();
				}
				else
				{
					gameStateManager.PopState(0);
				}
			}
			this.LobbyState.OnBattleServerLost();
		}

		// Token: 0x06002B3D RID: 11069 RVA: 0x000A8FC4 File Offset: 0x000A71C4
		void ILobbyClientSessionHandler.OnRemovedFromMatchmakerGame(DisconnectType disconnectType)
		{
			GameStateManager gameStateManager = Game.Current.GameStateManager;
			if (!(gameStateManager.ActiveState is LobbyState))
			{
				if (gameStateManager.ActiveState is MissionState)
				{
					BannerlordNetwork.EndMultiplayerLobbyMission();
				}
				else
				{
					gameStateManager.PopState(0);
				}
			}
			this.LobbyState.OnRemovedFromMatchmakerGame(disconnectType);
		}

		// Token: 0x06002B3E RID: 11070 RVA: 0x000A9010 File Offset: 0x000A7210
		void ILobbyClientSessionHandler.OnRejoinBattleRequestAnswered(bool isSuccessful)
		{
			this.LobbyState.OnRejoinBattleRequestAnswered(isSuccessful);
		}

		// Token: 0x06002B3F RID: 11071 RVA: 0x000A9020 File Offset: 0x000A7220
		void ILobbyClientSessionHandler.OnRegisterCustomGameServerResponse()
		{
			if (!GameNetwork.IsSessionActive)
			{
				LobbyGameStatePlayerBasedCustomServer lobbyGameStatePlayerBasedCustomServer = Game.Current.GameStateManager.CreateState<LobbyGameStatePlayerBasedCustomServer>();
				lobbyGameStatePlayerBasedCustomServer.SetStartingParameters(this);
				Game.Current.GameStateManager.PushState(lobbyGameStatePlayerBasedCustomServer, 0);
			}
		}

		// Token: 0x06002B40 RID: 11072 RVA: 0x000A905C File Offset: 0x000A725C
		void ILobbyClientSessionHandler.OnCustomGameEnd()
		{
			if (Game.Current != null)
			{
				GameStateManager gameStateManager = Game.Current.GameStateManager;
				if (!(gameStateManager.ActiveState is LobbyState))
				{
					if (Game.Current.GameStateManager.ActiveState is MissionState)
					{
						BannerlordNetwork.EndMultiplayerLobbyMission();
						return;
					}
					gameStateManager.PopState(0);
				}
			}
		}

		// Token: 0x06002B41 RID: 11073 RVA: 0x000A90AC File Offset: 0x000A72AC
		PlayerJoinGameResponseDataFromHost[] ILobbyClientSessionHandler.OnClientWantsToConnectCustomGame(PlayerJoinGameData[] playerJoinData, string password)
		{
			Debug.Print("Game join request with party received", 0, Debug.DebugColor.Green, 17592186044416UL);
			CustomGameJoinResponse customGameJoinResponse = CustomGameJoinResponse.UnspecifiedError;
			List<PlayerJoinGameResponseDataFromHost> list = new List<PlayerJoinGameResponseDataFromHost>();
			if (Mission.Current != null && Mission.Current.CurrentState == Mission.State.Continuing)
			{
				for (int i = 0; i < playerJoinData.Length; i++)
				{
					if (BannedPlayerManagerCustomGameClient.IsUserBanned(playerJoinData[i].PlayerId))
					{
						customGameJoinResponse = CustomGameJoinResponse.PlayerBanned;
					}
				}
				if (customGameJoinResponse != CustomGameJoinResponse.PlayerBanned)
				{
					string strValue = MultiplayerOptions.OptionType.AdminPassword.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
					string strValue2 = MultiplayerOptions.OptionType.GamePassword.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
					bool flag = !string.IsNullOrEmpty(strValue) && strValue == password && playerJoinData.Length == 1;
					bool flag2 = flag || string.IsNullOrEmpty(strValue2) || strValue2 == password;
					bool flag3 = MultiplayerOptions.OptionType.MaxNumberOfPlayers.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions) < GameNetwork.NetworkPeerCount + playerJoinData.Length;
					if (flag2 && !flag3)
					{
						List<PlayerConnectionInfo> list2 = new List<PlayerConnectionInfo>();
						foreach (PlayerJoinGameData playerJoinGameData in playerJoinData)
						{
							PlayerConnectionInfo playerConnectionInfo = new PlayerConnectionInfo(playerJoinGameData.PlayerId);
							Dictionary<int, List<int>> usedIndicesFromIds = CosmeticsManagerHelper.GetUsedIndicesFromIds(playerJoinGameData.UsedCosmetics);
							playerConnectionInfo.AddParameter("PlayerData", playerJoinGameData.PlayerData);
							playerConnectionInfo.AddParameter("UsedCosmetics", usedIndicesFromIds);
							playerConnectionInfo.Name = playerJoinGameData.Name;
							list2.Add(playerConnectionInfo);
						}
						GameNetwork.AddPlayersResult addPlayersResult = GameNetwork.HandleNewClientsConnect(list2.ToArray(), flag);
						if (addPlayersResult.Success)
						{
							for (int j = 0; j < playerJoinData.Length; j++)
							{
								PlayerJoinGameData playerJoinGameData2 = playerJoinData[j];
								NetworkCommunicator networkCommunicator = addPlayersResult.NetworkPeers[j];
								PlayerJoinGameResponseDataFromHost playerJoinGameResponseDataFromHost = new PlayerJoinGameResponseDataFromHost
								{
									PlayerId = playerJoinGameData2.PlayerId,
									PeerIndex = networkCommunicator.Index,
									SessionKey = networkCommunicator.SessionKey,
									CustomGameJoinResponse = CustomGameJoinResponse.Success
								};
								list.Add(playerJoinGameResponseDataFromHost);
							}
							customGameJoinResponse = CustomGameJoinResponse.Success;
						}
						else
						{
							customGameJoinResponse = CustomGameJoinResponse.ErrorOnGameServer;
						}
					}
					else if (!flag2)
					{
						customGameJoinResponse = CustomGameJoinResponse.IncorrectPassword;
					}
					else if (flag3)
					{
						customGameJoinResponse = CustomGameJoinResponse.ServerCapacityIsFull;
					}
					else
					{
						customGameJoinResponse = CustomGameJoinResponse.UnspecifiedError;
					}
				}
			}
			else
			{
				customGameJoinResponse = CustomGameJoinResponse.CustomGameServerNotAvailable;
			}
			if (customGameJoinResponse != CustomGameJoinResponse.Success)
			{
				foreach (PlayerJoinGameData playerJoinGameData3 in playerJoinData)
				{
					PlayerJoinGameResponseDataFromHost playerJoinGameResponseDataFromHost2 = new PlayerJoinGameResponseDataFromHost
					{
						PlayerId = playerJoinGameData3.PlayerId,
						PeerIndex = -1,
						SessionKey = -1,
						CustomGameJoinResponse = customGameJoinResponse
					};
					list.Add(playerJoinGameResponseDataFromHost2);
				}
			}
			Debug.Print("Responding game join request with " + customGameJoinResponse, 0, Debug.DebugColor.White, 17592186044416UL);
			return list.ToArray();
		}

		// Token: 0x06002B42 RID: 11074 RVA: 0x000A92FF File Offset: 0x000A74FF
		void ILobbyClientSessionHandler.OnJoinCustomGameResponse(bool success, JoinGameData joinGameData, CustomGameJoinResponse failureReason)
		{
			if (success)
			{
				Module.CurrentModule.GetMultiplayerGameMode(joinGameData.GameServerProperties.GameType).JoinCustomGame(joinGameData);
				Debug.Print("Join game successful", 0, Debug.DebugColor.Green, 17592186044416UL);
			}
		}

		// Token: 0x06002B43 RID: 11075 RVA: 0x000A9334 File Offset: 0x000A7534
		void ILobbyClientSessionHandler.OnJoinCustomGameFailureResponse(CustomGameJoinResponse response)
		{
			this.LobbyState.OnJoinCustomGameFailureResponse(response);
		}

		// Token: 0x06002B44 RID: 11076 RVA: 0x000A9344 File Offset: 0x000A7544
		void ILobbyClientSessionHandler.OnQuitFromCustomGame()
		{
			GameStateManager gameStateManager = Game.Current.GameStateManager;
			if (!(gameStateManager.ActiveState is LobbyState))
			{
				if (gameStateManager.ActiveState is MissionState)
				{
					BannerlordNetwork.EndMultiplayerLobbyMission();
					return;
				}
				gameStateManager.PopState(0);
			}
		}

		// Token: 0x06002B45 RID: 11077 RVA: 0x000A9384 File Offset: 0x000A7584
		void ILobbyClientSessionHandler.OnRemovedFromCustomGame(DisconnectType disconnectType)
		{
			GameStateManager gameStateManager = Game.Current.GameStateManager;
			if (!(gameStateManager.ActiveState is LobbyState))
			{
				if (gameStateManager.ActiveState is MissionState)
				{
					BannerlordNetwork.EndMultiplayerLobbyMission();
				}
				else
				{
					gameStateManager.PopState(0);
				}
			}
			this.LobbyState.OnRemovedFromCustomGame(disconnectType);
			if (this.LobbyState.LobbyClient.IsInParty)
			{
				switch (disconnectType)
				{
				case DisconnectType.QuitFromGame:
				case DisconnectType.TimedOut:
				case DisconnectType.KickedByHost:
				case DisconnectType.KickedByPoll:
				case DisconnectType.BannedByPoll:
				case DisconnectType.Inactivity:
				case DisconnectType.DisconnectedFromLobby:
				case DisconnectType.KickedDueToFriendlyDamage:
				case DisconnectType.PlayStateMismatch:
					this.LobbyState.LobbyClient.KickPlayerFromParty(this.LobbyState.LobbyClient.PlayerID);
					break;
				case DisconnectType.GameEnded:
				case DisconnectType.ServerNotResponding:
					break;
				default:
					return;
				}
			}
		}

		// Token: 0x06002B46 RID: 11078 RVA: 0x000A9435 File Offset: 0x000A7635
		void ILobbyClientSessionHandler.OnEnterCustomBattleWithPartyAnswer()
		{
		}

		// Token: 0x06002B47 RID: 11079 RVA: 0x000A9438 File Offset: 0x000A7638
		void ILobbyClientSessionHandler.OnClientQuitFromCustomGame(PlayerId playerId)
		{
			if (Mission.Current != null && Mission.Current.CurrentState == Mission.State.Continuing)
			{
				NetworkCommunicator networkCommunicator = GameNetwork.NetworkPeers.FirstOrDefault((NetworkCommunicator x) => x.VirtualPlayer.Id == playerId);
				if (networkCommunicator != null && !networkCommunicator.IsServerPeer)
				{
					if (networkCommunicator.GetComponent<MissionPeer>() != null)
					{
						networkCommunicator.QuitFromMission = true;
					}
					GameNetwork.AddNetworkPeerToDisconnectAsServer(networkCommunicator);
					MBDebug.Print("player with id " + playerId + " quit from game", 0, Debug.DebugColor.White, 17592186044416UL);
				}
			}
		}

		// Token: 0x06002B48 RID: 11080 RVA: 0x000A94C8 File Offset: 0x000A76C8
		void ILobbyClientSessionHandler.OnAnnouncementReceived(Announcement announcement)
		{
			if (Mission.Current != null && Mission.Current.CurrentState == Mission.State.Continuing)
			{
				if (announcement.Type == AnnouncementType.Chat)
				{
					InformationManager.DisplayMessage(new InformationMessage(announcement.Text.ToString(), Color.FromUint(4292235858U)));
					return;
				}
				if (announcement.Type == AnnouncementType.Alert)
				{
					InformationManager.AddSystemNotification(announcement.Text.ToString());
				}
			}
		}

		// Token: 0x06002B49 RID: 11081 RVA: 0x000A952C File Offset: 0x000A772C
		void ILobbyClientSessionHandler.OnChatMessageReceived(Guid roomId, string roomName, string playerName, string messageText, string textColor, MessageType type)
		{
			InformationMessage informationMessage;
			if (type != MessageType.System)
			{
				informationMessage = new InformationMessage(string.Concat(new string[] { "[", roomName, "] [", playerName, "]: ", messageText }), Color.ConvertStringToColor(textColor));
			}
			else
			{
				informationMessage = new InformationMessage("[" + roomName + "]: " + messageText, Color.ConvertStringToColor(textColor));
			}
			InformationManager.DisplayMessage(informationMessage);
		}

		// Token: 0x06002B4A RID: 11082 RVA: 0x000A95A4 File Offset: 0x000A77A4
		async Task<bool> ILobbyClientSessionHandler.OnInviteToPlatformSession(PlayerId playerId)
		{
			return await this.LobbyState.OnInviteToPlatformSession(playerId);
		}

		// Token: 0x170007AD RID: 1965
		// (get) Token: 0x06002B4B RID: 11083 RVA: 0x000A95F1 File Offset: 0x000A77F1
		// (set) Token: 0x06002B4C RID: 11084 RVA: 0x000A95F9 File Offset: 0x000A77F9
		public LobbyState LobbyState { get; set; }

		// Token: 0x170007AE RID: 1966
		// (get) Token: 0x06002B4D RID: 11085 RVA: 0x000A9602 File Offset: 0x000A7802
		public LobbyClient GameClient
		{
			get
			{
				return NetworkMain.GameClient;
			}
		}

		// Token: 0x0400105E RID: 4190
		public IChatHandler ChatHandler;
	}
}
