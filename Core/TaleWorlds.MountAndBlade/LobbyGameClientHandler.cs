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
	public class LobbyGameClientHandler : ILobbyClientSessionHandler
	{
		void ILobbyClientSessionHandler.OnConnected()
		{
		}

		void ILobbyClientSessionHandler.OnCantConnect()
		{
		}

		void ILobbyClientSessionHandler.OnDisconnected(TextObject feedback)
		{
			if (this.LobbyState != null)
			{
				this.LobbyState.OnDisconnected(feedback);
			}
		}

		void ILobbyClientSessionHandler.OnPlayerDataReceived(PlayerData playerData)
		{
			if (this.LobbyState != null)
			{
				this.LobbyState.OnPlayerDataReceived(playerData);
			}
		}

		void ILobbyClientSessionHandler.OnPendingRejoin()
		{
			LobbyState lobbyState = this.LobbyState;
			if (lobbyState == null)
			{
				return;
			}
			lobbyState.OnPendingRejoin();
		}

		void ILobbyClientSessionHandler.OnBattleResultReceived()
		{
		}

		void ILobbyClientSessionHandler.OnCancelJoiningBattle()
		{
			if (this.LobbyState != null)
			{
				this.LobbyState.OnCancelFindingGame();
			}
		}

		void ILobbyClientSessionHandler.OnRejoinRequestRejected()
		{
		}

		void ILobbyClientSessionHandler.OnFindGameAnswer(bool successful, string[] selectedAndEnabledGameTypes, bool isRejoin)
		{
			if (successful && this.LobbyState != null)
			{
				this.LobbyState.OnUpdateFindingGame(MatchmakingWaitTimeStats.Empty, selectedAndEnabledGameTypes);
			}
		}

		void ILobbyClientSessionHandler.OnEnterBattleWithPartyAnswer(string[] selectedGameTypes)
		{
			if (this.LobbyState != null)
			{
				this.LobbyState.OnUpdateFindingGame(MatchmakingWaitTimeStats.Empty, selectedGameTypes);
			}
		}

		void ILobbyClientSessionHandler.OnWhisperMessageReceived(string fromPlayer, string toPlayer, string message)
		{
			if (this.ChatHandler != null)
			{
				this.ChatHandler.ReceiveChatMessage(ChatChannelType.NaN, fromPlayer, message);
			}
			ChatBox.AddWhisperMessage(fromPlayer, message);
		}

		void ILobbyClientSessionHandler.OnClanMessageReceived(string playerName, string message)
		{
		}

		void ILobbyClientSessionHandler.OnChannelMessageReceived(ChatChannelType channel, string playerName, string message)
		{
			if (this.ChatHandler != null)
			{
				this.ChatHandler.ReceiveChatMessage(channel, playerName, message);
			}
			ChatBox.AddWhisperMessage(playerName, message);
		}

		void ILobbyClientSessionHandler.OnPartyMessageReceived(string playerName, string message)
		{
		}

		void ILobbyClientSessionHandler.OnSystemMessageReceived(string message)
		{
			InformationManager.DisplayMessage(new InformationMessage(message));
		}

		void ILobbyClientSessionHandler.OnAdminMessageReceived(string message)
		{
			if (this.LobbyState != null)
			{
				this.LobbyState.OnAdminMessageReceived(message);
			}
		}

		void ILobbyClientSessionHandler.OnPartyInvitationReceived(string inviterPlayerName, PlayerId inviterPlayerId)
		{
			if (this.LobbyState != null)
			{
				this.LobbyState.OnPartyInvitationReceived(inviterPlayerName, inviterPlayerId);
			}
		}

		void ILobbyClientSessionHandler.OnPartyInvitationInvalidated()
		{
			if (this.LobbyState != null)
			{
				this.LobbyState.OnPartyInvitationInvalidated();
			}
		}

		void ILobbyClientSessionHandler.OnPlayerInvitedToParty(PlayerId playerId)
		{
			if (this.LobbyState != null)
			{
				this.LobbyState.OnPlayerInvitedToParty(playerId);
			}
		}

		void ILobbyClientSessionHandler.OnPlayersAddedToParty([TupleElementNames(new string[] { "PlayerId", "PlayerName", "IsPartyLeader" })] List<ValueTuple<PlayerId, string, bool>> addedPlayers, [TupleElementNames(new string[] { "PlayerId", "PlayerName" })] List<ValueTuple<PlayerId, string>> invitedPlayers)
		{
			if (this.LobbyState != null)
			{
				this.LobbyState.OnPlayersAddedToParty(addedPlayers, invitedPlayers);
			}
		}

		void ILobbyClientSessionHandler.OnPlayerRemovedFromParty(PlayerId playerId, PartyRemoveReason reason)
		{
			if (this.LobbyState != null)
			{
				this.LobbyState.OnPlayerRemovedFromParty(playerId, reason);
			}
		}

		void ILobbyClientSessionHandler.OnPlayerAssignedPartyLeader(PlayerId partyLeaderId)
		{
			if (this.LobbyState != null)
			{
				this.LobbyState.OnPlayerAssignedPartyLeader(partyLeaderId);
			}
		}

		void ILobbyClientSessionHandler.OnPlayerSuggestedToParty(PlayerId playerId, string playerName, PlayerId suggestingPlayerId, string suggestingPlayerName)
		{
			if (this.LobbyState != null)
			{
				this.LobbyState.OnPlayerSuggestedToParty(playerId, playerName, suggestingPlayerId, suggestingPlayerName);
			}
		}

		void ILobbyClientSessionHandler.OnServerStatusReceived(ServerStatus serverStatus)
		{
			if (this.LobbyState != null)
			{
				this.LobbyState.OnServerStatusReceived(serverStatus);
			}
		}

		void ILobbyClientSessionHandler.OnFriendListReceived(FriendInfo[] friends)
		{
			if (this.LobbyState != null)
			{
				this.LobbyState.OnFriendListReceived(friends);
			}
		}

		void ILobbyClientSessionHandler.OnRecentPlayerStatusesReceived(FriendInfo[] friends)
		{
			if (this.LobbyState != null)
			{
				this.LobbyState.OnRecentPlayerStatusesReceived(friends);
			}
		}

		void ILobbyClientSessionHandler.OnClanInvitationReceived(string clanName, string clanTag, bool isCreation)
		{
			if (this.LobbyState != null)
			{
				this.LobbyState.OnClanInvitationReceived(clanName, clanTag, isCreation);
			}
		}

		void ILobbyClientSessionHandler.OnClanInvitationAnswered(PlayerId playerId, ClanCreationAnswer answer)
		{
			if (this.LobbyState != null)
			{
				this.LobbyState.OnClanInvitationAnswered(playerId, answer);
			}
		}

		void ILobbyClientSessionHandler.OnClanCreationSuccessful()
		{
			if (this.LobbyState != null)
			{
				this.LobbyState.OnClanCreationSuccessful();
			}
		}

		void ILobbyClientSessionHandler.OnClanCreationFailed()
		{
			if (this.LobbyState != null)
			{
				this.LobbyState.OnClanCreationFailed();
			}
		}

		void ILobbyClientSessionHandler.OnClanCreationStarted()
		{
			if (this.LobbyState != null)
			{
				this.LobbyState.OnClanCreationStarted();
			}
		}

		void ILobbyClientSessionHandler.OnClanInfoChanged()
		{
			if (this.LobbyState != null)
			{
				this.LobbyState.OnClanInfoChanged();
			}
		}

		void ILobbyClientSessionHandler.OnPremadeGameEligibilityStatusReceived(bool isEligible)
		{
			if (this.LobbyState != null)
			{
				this.LobbyState.OnPremadeGameEligibilityStatusReceived(isEligible);
			}
		}

		void ILobbyClientSessionHandler.OnPremadeGameCreated()
		{
			if (this.LobbyState != null)
			{
				this.LobbyState.OnPremadeGameCreated();
			}
		}

		void ILobbyClientSessionHandler.OnPremadeGameListReceived()
		{
			if (this.LobbyState != null)
			{
				this.LobbyState.OnPremadeGameListReceived();
			}
		}

		void ILobbyClientSessionHandler.OnPremadeGameCreationCancelled()
		{
			if (this.LobbyState != null)
			{
				this.LobbyState.OnPremadeGameCreationCancelled();
			}
		}

		void ILobbyClientSessionHandler.OnJoinPremadeGameRequested(string clanName, string clanSigilCode, Guid partyId, PlayerId[] challengerPlayerIDs, PlayerId challengerPartyLeaderID, PremadeGameType premadeGameType)
		{
			if (this.LobbyState != null)
			{
				this.LobbyState.OnJoinPremadeGameRequested(clanName, clanSigilCode, partyId, challengerPlayerIDs, challengerPartyLeaderID, premadeGameType);
			}
		}

		void ILobbyClientSessionHandler.OnJoinPremadeGameRequestSuccessful()
		{
			if (this.LobbyState != null)
			{
				this.LobbyState.OnJoinPremadeGameRequestSuccessful();
			}
		}

		void ILobbyClientSessionHandler.OnSigilChanged()
		{
			if (this.LobbyState != null)
			{
				this.LobbyState.OnSigilChanged();
			}
		}

		void ILobbyClientSessionHandler.OnNotificationsReceived(LobbyNotification[] notifications)
		{
			if (this.LobbyState != null)
			{
				this.LobbyState.OnNotificationsReceived(notifications);
			}
		}

		void ILobbyClientSessionHandler.OnGameClientStateChange(LobbyClient.State oldState)
		{
			this.HandleGameClientStateChange(oldState);
		}

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

		void ILobbyClientSessionHandler.OnCustomGameServerListReceived(AvailableCustomGames customGameServerList)
		{
			this.LobbyState.OnCustomGameServerListReceived(customGameServerList);
		}

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

		void ILobbyClientSessionHandler.OnRejoinBattleRequestAnswered(bool isSuccessful)
		{
			this.LobbyState.OnRejoinBattleRequestAnswered(isSuccessful);
		}

		void ILobbyClientSessionHandler.OnRegisterCustomGameServerResponse()
		{
			if (!GameNetwork.IsSessionActive)
			{
				LobbyGameStatePlayerBasedCustomServer lobbyGameStatePlayerBasedCustomServer = Game.Current.GameStateManager.CreateState<LobbyGameStatePlayerBasedCustomServer>();
				lobbyGameStatePlayerBasedCustomServer.SetStartingParameters(this);
				Game.Current.GameStateManager.PushState(lobbyGameStatePlayerBasedCustomServer, 0);
			}
		}

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

		void ILobbyClientSessionHandler.OnJoinCustomGameResponse(bool success, JoinGameData joinGameData, CustomGameJoinResponse failureReason)
		{
			if (success)
			{
				Module.CurrentModule.GetMultiplayerGameMode(joinGameData.GameServerProperties.GameType).JoinCustomGame(joinGameData);
				Debug.Print("Join game successful", 0, Debug.DebugColor.Green, 17592186044416UL);
			}
		}

		void ILobbyClientSessionHandler.OnJoinCustomGameFailureResponse(CustomGameJoinResponse response)
		{
			this.LobbyState.OnJoinCustomGameFailureResponse(response);
		}

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

		void ILobbyClientSessionHandler.OnEnterCustomBattleWithPartyAnswer()
		{
		}

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

		async Task<bool> ILobbyClientSessionHandler.OnInviteToPlatformSession(PlayerId playerId)
		{
			return await this.LobbyState.OnInviteToPlatformSession(playerId);
		}

		public LobbyState LobbyState { get; set; }

		public LobbyClient GameClient
		{
			get
			{
				return NetworkMain.GameClient;
			}
		}

		public IChatHandler ChatHandler;
	}
}
