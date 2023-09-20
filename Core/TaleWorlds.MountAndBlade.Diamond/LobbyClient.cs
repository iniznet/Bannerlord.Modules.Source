using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Messages.FromClient.ToLobbyServer;
using Messages.FromLobbyServer.ToClient;
using TaleWorlds.Core;
using TaleWorlds.Diamond;
using TaleWorlds.Diamond.ChatSystem.Library;
using TaleWorlds.Diamond.ClientApplication;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ModuleManager;
using TaleWorlds.MountAndBlade.Diamond.MultiplayerBadges;
using TaleWorlds.MountAndBlade.Diamond.Ranked;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.Diamond
{
	public class LobbyClient : Client<LobbyClient>, IChatClientHandler
	{
		private static int FriendListCheckDelay
		{
			get
			{
				return LobbyClient._friendListCheckDelay;
			}
			set
			{
				if (value != LobbyClient._friendListCheckDelay)
				{
					LobbyClient._friendListCheckDelay = value;
				}
			}
		}

		public PlayerData PlayerData { get; private set; }

		public SupportedFeatures SupportedFeatures { get; private set; }

		public ClanInfo ClanInfo { get; private set; }

		public ClanHomeInfo ClanHomeInfo { get; private set; }

		public IReadOnlyList<string> OwnedCosmetics
		{
			get
			{
				return this._ownedCosmetics;
			}
		}

		public IReadOnlyDictionary<string, List<string>> UsedCosmetics
		{
			get
			{
				return this._usedCosmetics;
			}
		}

		public AvailableScenes AvailableScenes { get; private set; }

		public PlayerId PlayerID
		{
			get
			{
				return this._playerId;
			}
		}

		public LobbyClient.State CurrentState
		{
			get
			{
				return this._state;
			}
			private set
			{
				if (this._state != value)
				{
					LobbyClient.State state = this._state;
					this._state = value;
					if (this._handler != null)
					{
						this._handler.OnGameClientStateChange(state);
					}
				}
			}
		}

		public override long AliveCheckTimeInMiliSeconds
		{
			get
			{
				switch (this.CurrentState)
				{
				case LobbyClient.State.Idle:
				case LobbyClient.State.Working:
				case LobbyClient.State.Connected:
				case LobbyClient.State.SessionRequested:
				case LobbyClient.State.AtLobby:
					return 6000L;
				case LobbyClient.State.SearchingToRejoinBattle:
				case LobbyClient.State.RequestingToSearchBattle:
				case LobbyClient.State.RequestingToCancelSearchBattle:
				case LobbyClient.State.SearchingBattle:
				case LobbyClient.State.QuittingFromBattle:
				case LobbyClient.State.WaitingToRegisterCustomGame:
				case LobbyClient.State.HostingCustomGame:
				case LobbyClient.State.WaitingToJoinCustomGame:
					return 2000L;
				case LobbyClient.State.AtBattle:
				case LobbyClient.State.InCustomGame:
					return 60000L;
				}
				return 1000L;
			}
		}

		public bool AtLobby
		{
			get
			{
				return this.CurrentState == LobbyClient.State.AtLobby;
			}
		}

		public bool CanPerformLobbyActions
		{
			get
			{
				return this.CurrentState == LobbyClient.State.AtLobby || this.CurrentState == LobbyClient.State.RequestingToSearchBattle || this.CurrentState == LobbyClient.State.SearchingBattle || this.CurrentState == LobbyClient.State.WaitingToJoinCustomGame;
			}
		}

		public string Name
		{
			get
			{
				return this._userName;
			}
		}

		public string LastBattleServerAddressForClient { get; private set; }

		public ushort LastBattleServerPortForClient { get; private set; }

		public bool Connected
		{
			get
			{
				return this.CurrentState != LobbyClient.State.Working && this.CurrentState > LobbyClient.State.Idle;
			}
		}

		public bool IsIdle
		{
			get
			{
				return this.CurrentState == LobbyClient.State.Idle;
			}
		}

		public void Logout(TextObject logOutReason)
		{
			base.BeginDisconnect();
			this.ChatManager.Cleanup();
			this._logOutReason = logOutReason;
		}

		public bool LoggedIn
		{
			get
			{
				return this.CurrentState != LobbyClient.State.Idle && this.CurrentState != LobbyClient.State.Working && this.CurrentState != LobbyClient.State.Connected && this.CurrentState != LobbyClient.State.SessionRequested;
			}
		}

		public bool IsInGame
		{
			get
			{
				return this.CurrentState == LobbyClient.State.AtBattle || this.CurrentState == LobbyClient.State.HostingCustomGame || this.CurrentState == LobbyClient.State.InCustomGame;
			}
		}

		public bool IsHostingCustomGame
		{
			get
			{
				return this._state == LobbyClient.State.HostingCustomGame;
			}
		}

		public bool IsMatchmakingAvailable
		{
			get
			{
				ServerStatus serverStatus = this._serverStatus;
				return serverStatus != null && serverStatus.IsMatchmakingEnabled;
			}
		}

		public bool IsAbleToSearchForGame
		{
			get
			{
				return this.IsMatchmakingAvailable && this._matchmakerBlockedTime <= DateTime.Now;
			}
		}

		public bool PartySystemAvailable
		{
			get
			{
				return true;
			}
		}

		public bool IsCustomBattleAvailable
		{
			get
			{
				ServerStatus serverStatus = this._serverStatus;
				return serverStatus != null && serverStatus.IsCustomBattleEnabled;
			}
		}

		public IReadOnlyList<ModuleInfoModel> LoadedUnofficialModules
		{
			get
			{
				return this._loadedUnofficialModules;
			}
		}

		public bool HasUnofficialModulesLoaded
		{
			get
			{
				return this.LoadedUnofficialModules.Count > 0;
			}
		}

		public bool HasUserGeneratedContentPrivilege { get; private set; }

		public bool IsPartyLeader
		{
			get
			{
				if (this.Connected)
				{
					object obj = true;
					PartyPlayerInLobbyClient partyPlayerInLobbyClient = this.PlayersInParty.Find((PartyPlayerInLobbyClient p) => p.PlayerId == this._playerId);
					return object.Equals(obj, (partyPlayerInLobbyClient != null) ? new bool?(partyPlayerInLobbyClient.IsPartyLeader) : null);
				}
				return false;
			}
		}

		public bool IsClanLeader
		{
			get
			{
				ClanPlayer clanPlayer = this.PlayersInClan.Find((ClanPlayer p) => p.PlayerId == this._playerId);
				return clanPlayer != null && clanPlayer.Role == ClanPlayerRole.Leader;
			}
		}

		public bool IsClanOfficer
		{
			get
			{
				ClanPlayer clanPlayer = this.PlayersInClan.Find((ClanPlayer p) => p.PlayerId == this._playerId);
				return clanPlayer != null && clanPlayer.Role == ClanPlayerRole.Officer;
			}
		}

		public bool IsEligibleToCreatePremadeGame { get; private set; }

		public CustomBattleId CustomBattleId { get; private set; }

		public string CustomGameType { get; private set; }

		public string CustomGameScene { get; private set; }

		public AvailableCustomGames AvailableCustomGames { get; private set; }

		public PremadeGameList AvailablePremadeGames { get; private set; }

		public List<PartyPlayerInLobbyClient> PlayersInParty { get; private set; }

		public List<ClanPlayer> PlayersInClan { get; private set; }

		public List<ClanPlayerInfo> PlayerInfosInClan { get; private set; }

		public FriendInfo[] FriendInfos { get; private set; }

		public bool IsInParty
		{
			get
			{
				return this.Connected && this.PlayersInParty.Count > 0;
			}
		}

		public bool IsPartyFull
		{
			get
			{
				return this.PlayersInParty.Count == Parameters.MaxPlayerCountInParty;
			}
		}

		public string CurrentMatchId { get; private set; }

		public bool IsInClan
		{
			get
			{
				return this.PlayersInClan.Count > 0;
			}
		}

		public bool IsPartyInvitationPopupActive { get; private set; }

		public bool CanInvitePlayers
		{
			get
			{
				SupportedFeatures supportedFeatures = this.SupportedFeatures;
				return supportedFeatures != null && supportedFeatures.SupportsFeatures(Features.Party) && (!this.IsInParty || this.IsPartyLeader);
			}
		}

		public bool CanSuggestPlayers
		{
			get
			{
				SupportedFeatures supportedFeatures = this.SupportedFeatures;
				return supportedFeatures != null && supportedFeatures.SupportsFeatures(Features.Party) && this.IsInParty && !this.IsPartyLeader;
			}
		}

		public bool IsRefreshingPlayerData { get; set; }

		public Guid ClanID { get; private set; }

		public LobbyClient(DiamondClientApplication diamondClientApplication, IClientSessionProvider<LobbyClient> sessionProvider)
			: base(diamondClientApplication, sessionProvider, false)
		{
			this._serverStatusTimer = new Stopwatch();
			this._serverStatusTimer.Start();
			this._matchmakerBlockedTime = DateTime.MinValue;
			this._friendListTimer = new Stopwatch();
			this._friendListTimer.Start();
			this.PlayersInParty = new List<PartyPlayerInLobbyClient>();
			this.PlayersInClan = new List<ClanPlayer>();
			this.PlayerInfosInClan = new List<ClanPlayerInfo>();
			this.FriendInfos = new FriendInfo[0];
			this.ChatManager = new ChatManager(this);
			this.ClanID = Guid.Empty;
			this.FriendIDs = new List<PlayerId>();
			this.SupportedFeatures = new SupportedFeatures();
			this._ownedCosmetics = new List<string>();
			this._usedCosmetics = new Dictionary<string, List<string>>();
			this._cachedRankInfos = new TimedDictionaryCache<PlayerId, GameTypeRankInfo[]>(TimeSpan.FromSeconds(10.0));
			this._cachedPlayerStats = new TimedDictionaryCache<PlayerId, PlayerStatsBase[]>(TimeSpan.FromSeconds(10.0));
			this._cachedPlayerDatas = new TimedDictionaryCache<PlayerId, PlayerData>(TimeSpan.FromSeconds(10.0));
			this._cachedPlayerBannerlordIDs = new TimedDictionaryCache<PlayerId, string>(TimeSpan.FromSeconds(30.0));
			this._pendingPlayerRequests = new Dictionary<ValueTuple<LobbyClient.PendingRequest, PlayerId>, Task>();
			base.AddMessageHandler<FindGameAnswerMessage>(new ClientMessageHandler<FindGameAnswerMessage>(this.OnFindGameAnswerMessage));
			base.AddMessageHandler<JoinBattleMessage>(new ClientMessageHandler<JoinBattleMessage>(this.OnJoinBattleMessage));
			base.AddMessageHandler<BattleResultMessage>(new ClientMessageHandler<BattleResultMessage>(this.OnBattleResultMessage));
			base.AddMessageHandler<BattleServerLostMessage>(new ClientMessageHandler<BattleServerLostMessage>(this.OnBattleServerLostMessage));
			base.AddMessageHandler<BattleOverMessage>(new ClientMessageHandler<BattleOverMessage>(this.OnBattleOverMessage));
			base.AddMessageHandler<CancelBattleResponseMessage>(new ClientMessageHandler<CancelBattleResponseMessage>(this.OnCancelBattleResponseMessage));
			base.AddMessageHandler<RejoinRequestRejectedMessage>(new ClientMessageHandler<RejoinRequestRejectedMessage>(this.OnRejoinRequestRejectedMessage));
			base.AddMessageHandler<CancelFindGameMessage>(new ClientMessageHandler<CancelFindGameMessage>(this.OnCancelFindGameMessage));
			base.AddMessageHandler<WhisperReceivedMessage>(new ClientMessageHandler<WhisperReceivedMessage>(this.OnWhisperMessageReceivedMessage));
			base.AddMessageHandler<ClanMessageReceivedMessage>(new ClientMessageHandler<ClanMessageReceivedMessage>(this.OnClanMessageReceivedMessage));
			base.AddMessageHandler<ChannelMessageReceivedMessage>(new ClientMessageHandler<ChannelMessageReceivedMessage>(this.OnChannelMessageReceivedMessage));
			base.AddMessageHandler<PartyMessageReceivedMessage>(new ClientMessageHandler<PartyMessageReceivedMessage>(this.OnPartyMessageReceivedMessage));
			base.AddMessageHandler<SystemMessage>(new ClientMessageHandler<SystemMessage>(this.OnSystemMessage));
			base.AddMessageHandler<InvitationToPartyMessage>(new ClientMessageHandler<InvitationToPartyMessage>(this.OnInvitationToPartyMessage));
			base.AddMessageHandler<PartyInvitationInvalidMessage>(new ClientMessageHandler<PartyInvitationInvalidMessage>(this.OnPartyInvitationInvalidMessage));
			base.AddMessageHandler<UpdatePlayerDataMessage>(new ClientMessageHandler<UpdatePlayerDataMessage>(this.OnUpdatePlayerDataMessage));
			base.AddMessageHandler<RecentPlayerStatusesMessage>(new ClientMessageHandler<RecentPlayerStatusesMessage>(this.OnRecentPlayerStatusesMessage));
			base.AddMessageHandler<PlayerQuitFromMatchmakerGameResult>(new ClientMessageHandler<PlayerQuitFromMatchmakerGameResult>(this.OnPlayerQuitFromMatchmakerGameResult));
			base.AddMessageHandler<PlayerRemovedFromMatchmakerGame>(new ClientMessageHandler<PlayerRemovedFromMatchmakerGame>(this.OnPlayerRemovedFromMatchmakerGameMessage));
			base.AddMessageHandler<EnterBattleWithPartyAnswer>(new ClientMessageHandler<EnterBattleWithPartyAnswer>(this.OnEnterBattleWithPartyAnswerMessage));
			base.AddMessageHandler<JoinCustomGameResultMessage>(new ClientMessageHandler<JoinCustomGameResultMessage>(this.OnJoinCustomGameResultMessage));
			base.AddMessageHandler<ClientWantsToConnectCustomGameMessage>(new ClientMessageHandler<ClientWantsToConnectCustomGameMessage>(this.OnClientWantsToConnectCustomGameMessage));
			base.AddMessageHandler<ClientQuitFromCustomGameMessage>(new ClientMessageHandler<ClientQuitFromCustomGameMessage>(this.OnClientQuitFromCustomGameMessage));
			base.AddMessageHandler<PlayerRemovedFromCustomGame>(new ClientMessageHandler<PlayerRemovedFromCustomGame>(this.OnPlayerRemovedFromCustomGame));
			base.AddMessageHandler<EnterCustomBattleWithPartyAnswer>(new ClientMessageHandler<EnterCustomBattleWithPartyAnswer>(this.OnEnterCustomBattleWithPartyAnswerMessage));
			base.AddMessageHandler<PlayerInvitedToPartyMessage>(new ClientMessageHandler<PlayerInvitedToPartyMessage>(this.OnPlayerInvitedToPartyMessage));
			base.AddMessageHandler<PlayersAddedToPartyMessage>(new ClientMessageHandler<PlayersAddedToPartyMessage>(this.OnPlayerAddedToPartyMessage));
			base.AddMessageHandler<PlayerRemovedFromPartyMessage>(new ClientMessageHandler<PlayerRemovedFromPartyMessage>(this.OnPlayerRemovedFromPartyMessage));
			base.AddMessageHandler<PlayerAssignedPartyLeaderMessage>(new ClientMessageHandler<PlayerAssignedPartyLeaderMessage>(this.OnPlayerAssignedPartyLeaderMessage));
			base.AddMessageHandler<PlayerSuggestedToPartyMessage>(new ClientMessageHandler<PlayerSuggestedToPartyMessage>(this.OnPlayerSuggestedToPartyMessage));
			base.AddMessageHandler<ServerStatusMessage>(new ClientMessageHandler<ServerStatusMessage>(this.OnServerStatusMessage));
			base.AddMessageHandler<MatchmakerDisabledMessage>(new ClientMessageHandler<MatchmakerDisabledMessage>(this.OnMatchmakerDisabledMessage));
			base.AddMessageHandler<FriendListMessage>(new ClientMessageHandler<FriendListMessage>(this.OnFriendListMessage));
			base.AddMessageHandler<AdminMessage>(new ClientMessageHandler<AdminMessage>(this.OnAdminMessage));
			base.AddMessageHandler<CreateClanAnswerMessage>(new ClientMessageHandler<CreateClanAnswerMessage>(this.OnCreateClanAnswerMessage));
			base.AddMessageHandler<ClanCreationRequestMessage>(new ClientMessageHandler<ClanCreationRequestMessage>(this.OnClanCreationRequestMessage));
			base.AddMessageHandler<ClanCreationRequestAnsweredMessage>(new ClientMessageHandler<ClanCreationRequestAnsweredMessage>(this.OnClanCreationRequestAnsweredMessage));
			base.AddMessageHandler<ClanCreationFailedMessage>(new ClientMessageHandler<ClanCreationFailedMessage>(this.OnClanCreationFailedMessage));
			base.AddMessageHandler<ClanCreationSuccessfulMessage>(new ClientMessageHandler<ClanCreationSuccessfulMessage>(this.OnClanCreationSuccessfulMessage));
			base.AddMessageHandler<ClanInfoChangedMessage>(new ClientMessageHandler<ClanInfoChangedMessage>(this.OnClanInfoChangedMessage));
			base.AddMessageHandler<InvitationToClanMessage>(new ClientMessageHandler<InvitationToClanMessage>(this.OnInvitationToClanMessage));
			base.AddMessageHandler<ClanDisbandedMessage>(new ClientMessageHandler<ClanDisbandedMessage>(this.OnClanDisbandedMessage));
			base.AddMessageHandler<KickedFromClanMessage>(new ClientMessageHandler<KickedFromClanMessage>(this.OnKickedFromClan));
			base.AddMessageHandler<JoinPremadeGameAnswerMessage>(new ClientMessageHandler<JoinPremadeGameAnswerMessage>(this.OnJoinPremadeGameAnswerMessage));
			base.AddMessageHandler<PremadeGameEligibilityStatusMessage>(new ClientMessageHandler<PremadeGameEligibilityStatusMessage>(this.OnPremadeGameEligibilityStatusMessage));
			base.AddMessageHandler<CreatePremadeGameAnswerMessage>(new ClientMessageHandler<CreatePremadeGameAnswerMessage>(this.OnCreatePremadeGameAnswerMessage));
			base.AddMessageHandler<JoinPremadeGameRequestMessage>(new ClientMessageHandler<JoinPremadeGameRequestMessage>(this.OnJoinPremadeGameRequestMessage));
			base.AddMessageHandler<JoinPremadeGameRequestResultMessage>(new ClientMessageHandler<JoinPremadeGameRequestResultMessage>(this.OnJoinPremadeGameRequestResultMessage));
			base.AddMessageHandler<ClanGameCreationCancelledMessage>(new ClientMessageHandler<ClanGameCreationCancelledMessage>(this.OnClanGameCreationCancelledMessage));
			base.AddMessageHandler<JoinChatRoomMessage>(new ClientMessageHandler<JoinChatRoomMessage>(this.OnJoinChatRoomMessage));
			base.AddMessageHandler<ChatRoomClosedMessage>(new ClientMessageHandler<ChatRoomClosedMessage>(this.OnChatRoomClosedMessage));
			base.AddMessageHandler<SigilChangeAnswerMessage>(new ClientMessageHandler<SigilChangeAnswerMessage>(this.OnSigilChangeAnswerMessage));
			base.AddMessageHandler<LobbyNotificationsMessage>(new ClientMessageHandler<LobbyNotificationsMessage>(this.OnLobbyNotificationsMessage));
			base.AddMessageHandler<CustomBattleOverMessage>(new ClientMessageHandler<CustomBattleOverMessage>(this.OnCustomBattleOverMessage));
			base.AddMessageHandler<RejoinBattleRequestAnswerMessage>(new ClientMessageHandler<RejoinBattleRequestAnswerMessage>(this.OnRejoinBattleRequestAnswerMessage));
			base.AddMessageHandler<PendingBattleRejoinMessage>(new ClientMessageHandler<PendingBattleRejoinMessage>(this.OnPendingBattleRejoinMessage));
			base.AddMessageHandler<ShowAnnouncementMessage>(new ClientMessageHandler<ShowAnnouncementMessage>(this.OnShowAnnouncementMessage));
		}

		public ChatManager ChatManager { get; private set; }

		public void SetLoadedModules(string[] moduleIDs)
		{
			if (this._loadedUnofficialModules == null)
			{
				this._loadedUnofficialModules = new List<ModuleInfoModel>();
				using (List<ModuleInfo>.Enumerator enumerator = ModuleHelper.GetSortedModules(moduleIDs).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						ModuleInfoModel moduleInfoModel;
						if (ModuleInfoModel.TryCreateForSession(enumerator.Current, out moduleInfoModel))
						{
							this._loadedUnofficialModules.Add(moduleInfoModel);
						}
					}
				}
			}
		}

		public async Task<AvailableCustomGames> GetCustomGameServerList()
		{
			this.AssertCanPerformLobbyActions();
			CustomGameServerListResponse customGameServerListResponse = await base.CallFunction<CustomGameServerListResponse>(new RequestCustomGameServerListMessage());
			Debug.Print("Custom game server list received", 0, Debug.DebugColor.White, 17592186044416UL);
			AvailableCustomGames availableCustomGames;
			if (customGameServerListResponse != null)
			{
				this.AvailableCustomGames = customGameServerListResponse.AvailableCustomGames;
				if (this._handler != null)
				{
					this._handler.OnCustomGameServerListReceived(this.AvailableCustomGames);
				}
				availableCustomGames = this.AvailableCustomGames;
			}
			else
			{
				availableCustomGames = null;
			}
			return availableCustomGames;
		}

		public void QuitFromCustomGame()
		{
			base.SendMessage(new QuitFromCustomGameMessage());
			this.CurrentState = LobbyClient.State.AtLobby;
			if (this._handler != null)
			{
				this._handler.OnQuitFromCustomGame();
			}
		}

		public void QuitFromMatchmakerGame()
		{
			if (this.CurrentState == LobbyClient.State.AtBattle)
			{
				this.CheckAndSendMessage(new QuitFromMatchmakerGameMessage());
				this.CurrentState = LobbyClient.State.QuittingFromBattle;
				if (this._handler != null)
				{
					this._handler.OnQuitFromMatchmakerGame();
				}
			}
		}

		public async Task<bool> RequestJoinCustomGame(CustomBattleId serverId, string password)
		{
			this.CurrentState = LobbyClient.State.WaitingToJoinCustomGame;
			this.CustomBattleId = serverId;
			string text = ((!string.IsNullOrEmpty(password)) ? Common.CalculateMD5Hash(password) : null);
			base.SendMessage(new RequestJoinCustomGameMessage(serverId, text));
			while (this.CurrentState == LobbyClient.State.WaitingToJoinCustomGame)
			{
				await Task.Yield();
			}
			bool flag;
			if (this.CurrentState == LobbyClient.State.InCustomGame)
			{
				flag = true;
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		public void CancelFindGame()
		{
			this.CurrentState = LobbyClient.State.RequestingToCancelSearchBattle;
			this.CheckAndSendMessage(new CancelBattleRequestMessage());
		}

		public void FindGame()
		{
			this.CurrentState = LobbyClient.State.RequestingToSearchBattle;
			this.CheckAndSendMessage(new FindGameMessage());
		}

		public async Task<bool> FindCustomGame(string[] selectedCustomGameTypes, bool? hasCrossplayPrivilege, string region)
		{
			this.CurrentState = LobbyClient.State.WaitingToJoinCustomGame;
			int i = 0;
			while (i < LobbyClient.CheckForCustomGamesCount)
			{
				CustomGameServerListResponse customGameServerListResponse = await base.CallFunction<CustomGameServerListResponse>(new RequestCustomGameServerListMessage());
				if (customGameServerListResponse != null && customGameServerListResponse.AvailableCustomGames.CustomGameServerInfos.Count > 0)
				{
					List<GameServerEntry> list = customGameServerListResponse.AvailableCustomGames.CustomGameServerInfos.OrderByDescending((GameServerEntry c) => c.PlayerCount).ToList<GameServerEntry>();
					bool? flag = hasCrossplayPrivilege;
					bool flag2 = true;
					GameServerEntry.FilterGameServerEntriesBasedOnCrossplay(ref list, (flag.GetValueOrDefault() == flag2) & (flag != null));
					foreach (string text in selectedCustomGameTypes)
					{
						foreach (GameServerEntry gameServerEntry in list)
						{
							if (gameServerEntry.IsOfficial && gameServerEntry.GameType == text && gameServerEntry.Region == region && !gameServerEntry.PasswordProtected && gameServerEntry.MaxPlayerCount >= gameServerEntry.PlayerCount + this.PlayersInParty.Count)
							{
								base.SendMessage(new RequestJoinCustomGameMessage(gameServerEntry.Id, ""));
								while (this.CurrentState == LobbyClient.State.WaitingToJoinCustomGame)
								{
									await Task.Yield();
								}
								if (this.CurrentState == LobbyClient.State.InCustomGame)
								{
									return true;
								}
								return false;
							}
						}
						List<GameServerEntry>.Enumerator enumerator = default(List<GameServerEntry>.Enumerator);
					}
					await Task.Delay(LobbyClient.CheckForCustomGamesDelay);
				}
				int j = i++;
			}
			this.CurrentState = LobbyClient.State.AtLobby;
			return false;
		}

		public async Task<LobbyClientConnectResult> Connect(ILobbyClientSessionHandler lobbyClientSessionHandler, ILoginAccessProvider lobbyClientLoginAccessProvider, string overridenUserName, bool hasUserGeneratedContentPrivilege, PlatformInitParams initParams)
		{
			base.AccessProvider = lobbyClientLoginAccessProvider;
			base.AccessProvider.Initialize(overridenUserName, initParams);
			this._handler = lobbyClientSessionHandler;
			this.CurrentState = LobbyClient.State.Working;
			this.HasUserGeneratedContentPrivilege = hasUserGeneratedContentPrivilege;
			base.BeginConnect();
			while (this.CurrentState == LobbyClient.State.Working)
			{
				await Task.Yield();
			}
			LobbyClientConnectResult lobbyClientConnectResult;
			if (this.CurrentState == LobbyClient.State.Connected)
			{
				AccessObjectResult accessObjectResult = AccessObjectResult.CreateFailed(new TextObject("{=gAeQdLU5}Failed to acquire access data from platform", null));
				Task getAccessObjectTask = Task.Run(delegate
				{
					accessObjectResult = this.AccessProvider.CreateAccessObject();
				});
				while (!getAccessObjectTask.IsCompleted)
				{
					await Task.Yield();
				}
				if (getAccessObjectTask.IsFaulted)
				{
					throw getAccessObjectTask.Exception ?? new Exception("Get access object task faulted without exception");
				}
				if (getAccessObjectTask.IsCanceled)
				{
					throw new Exception("Get access object task canceled");
				}
				if (accessObjectResult.Success)
				{
					this._userName = base.AccessProvider.GetUserName();
					this._playerId = base.AccessProvider.GetPlayerId();
					this.CurrentState = LobbyClient.State.SessionRequested;
					string environmentVariable = Environment.GetEnvironmentVariable("Bannerlord.ConnectionPassword");
					LoginResult loginResult = await base.Login(new InitializeSession(this._playerId, this._userName, accessObjectResult.AccessObject, base.Application.ApplicationVersion, environmentVariable, this._loadedUnofficialModules.ToArray()));
					if (loginResult != null)
					{
						if (loginResult.Successful)
						{
							InitializeSessionResponse initializeSessionResponse = (InitializeSessionResponse)loginResult.LoginResultObject;
							this.PlayerData = initializeSessionResponse.PlayerData;
							this._serverStatus = initializeSessionResponse.ServerStatus;
							this.SupportedFeatures = initializeSessionResponse.SupportedFeatures;
							this.AvailableScenes = initializeSessionResponse.AvailableScenes;
							this._logOutReason = new TextObject("{=i4MNr0bo}Disconnected from the Lobby.", null);
							await PermaMuteList.LoadMutedPlayers(this.PlayerData.PlayerId);
							this._ownedCosmetics.Clear();
							this._usedCosmetics.Clear();
							this._handler.OnPlayerDataReceived(this.PlayerData);
							this._handler.OnServerStatusReceived(initializeSessionResponse.ServerStatus);
							LobbyClient.FriendListCheckDelay = this._serverStatus.FriendListUpdatePeriod * 1000;
							if (initializeSessionResponse.HasPendingRejoin)
							{
								this._handler.OnPendingRejoin();
							}
							this.CurrentState = LobbyClient.State.AtLobby;
							lobbyClientConnectResult = new LobbyClientConnectResult(true, null);
						}
						else
						{
							base.BeginDisconnect();
							lobbyClientConnectResult = new LobbyClientConnectResult(false, loginResult.ErrorCode);
						}
					}
					else
					{
						base.BeginDisconnect();
						lobbyClientConnectResult = new LobbyClientConnectResult(false, new TextObject("{=63X8LERm}Couldn't receive login result from server.", null));
					}
				}
				else
				{
					base.BeginDisconnect();
					lobbyClientConnectResult = new LobbyClientConnectResult(false, accessObjectResult.FailReason ?? new TextObject("{=JO37PkfW}Your platform service is not initialized.", null));
				}
			}
			else
			{
				lobbyClientConnectResult = new LobbyClientConnectResult(false, new TextObject("{=3cWg0cWt}Could not connect to server.", null));
			}
			return lobbyClientConnectResult;
		}

		public void KickPlayer(PlayerId id, bool banPlayer)
		{
			throw new NotImplementedException();
		}

		public void ChangeRegion(string region)
		{
			if (this.PlayerData == null || this.PlayerData.LastRegion != region)
			{
				this.CheckAndSendMessage(new ChangeRegionMessage(region));
			}
			if (this.CurrentState == LobbyClient.State.AtLobby)
			{
				this.PlayerData.LastRegion = region;
			}
		}

		public void ChangeGameTypes(string[] gameTypes)
		{
			bool flag = this.PlayerData == null || this.PlayerData.LastGameTypes.Length != gameTypes.Length;
			if (!flag)
			{
				foreach (string text in gameTypes)
				{
					if (!this.PlayerData.LastGameTypes.Contains(text))
					{
						flag = true;
						break;
					}
				}
			}
			if (flag)
			{
				this.CheckAndSendMessage(new ChangeGameTypesMessage(gameTypes));
			}
			if (this.CurrentState == LobbyClient.State.AtLobby)
			{
				this.PlayerData.LastGameTypes = gameTypes;
			}
		}

		private void CheckAndSendMessage(Message message)
		{
			base.SendMessage(message);
		}

		public override void OnConnected()
		{
			base.OnConnected();
			this.CurrentState = LobbyClient.State.Connected;
			if (this._handler != null)
			{
				this._handler.OnConnected();
			}
		}

		public override void OnCantConnect()
		{
			base.OnCantConnect();
			this.CurrentState = LobbyClient.State.Idle;
			if (this._handler != null)
			{
				this._handler.OnCantConnect();
			}
		}

		public override void OnDisconnected()
		{
			base.OnDisconnected();
			bool loggedIn = this.LoggedIn;
			this.CurrentState = LobbyClient.State.Idle;
			this.PlayerData = null;
			this.PlayersInParty.Clear();
			this.PlayersInClan.Clear();
			this._matchmakerBlockedTime = DateTime.MinValue;
			this.FriendInfos = new FriendInfo[0];
			PermaMuteList.SaveMutedPlayers();
			this._ownedCosmetics.Clear();
			this._usedCosmetics.Clear();
			if (this._handler != null)
			{
				this._handler.OnDisconnected(loggedIn ? this._logOutReason : null);
			}
			this._handler = null;
		}

		private void OnFindGameAnswerMessage(FindGameAnswerMessage message)
		{
			if (!message.Successful)
			{
				this.CurrentState = LobbyClient.State.AtLobby;
			}
			else
			{
				this.CurrentState = LobbyClient.State.SearchingBattle;
			}
			this._handler.OnFindGameAnswer(message.Successful, message.SelectedAndEnabledGameTypes, false);
		}

		private void OnJoinBattleMessage(JoinBattleMessage message)
		{
			BattleServerInformationForClient battleServerInformation = message.BattleServerInformation;
			string text;
			if (base.Application.ProxyAddressMap.TryGetValue(battleServerInformation.ServerAddress, out text))
			{
				battleServerInformation.ServerAddress = text;
			}
			this.LastBattleServerAddressForClient = battleServerInformation.ServerAddress;
			this.LastBattleServerPortForClient = battleServerInformation.ServerPort;
			this.CurrentMatchId = battleServerInformation.MatchId;
			this._handler.OnBattleServerInformationReceived(battleServerInformation);
			this.CurrentState = LobbyClient.State.AtBattle;
		}

		private void OnBattleOverMessage(BattleOverMessage message)
		{
			if (this.CurrentState == LobbyClient.State.AtBattle)
			{
				this.CurrentState = LobbyClient.State.AtLobby;
				this._handler.OnMatchmakerGameOver(message.OldExperience, message.NewExperience, message.EarnedBadges, message.GoldGained, message.OldInfo, message.NewInfo);
			}
		}

		private void OnJoinChatRoomMessage(JoinChatRoomMessage message)
		{
			this.ChatManager.OnJoinChatRoom(message.ChatRoomInformaton, this.PlayerData.PlayerId, this.PlayerData.LastPlayerName);
		}

		private void OnChatRoomClosedMessage(ChatRoomClosedMessage message)
		{
			this.ChatManager.OnChatRoomClosed(message.ChatRoomId);
		}

		private void OnBattleResultMessage(BattleResultMessage message)
		{
			this._handler.OnBattleResultReceived();
		}

		private void OnBattleServerLostMessage(BattleServerLostMessage message)
		{
			if (this.CurrentState == LobbyClient.State.AtBattle || this.CurrentState == LobbyClient.State.SearchingToRejoinBattle)
			{
				this.CurrentState = LobbyClient.State.AtLobby;
			}
			this._handler.OnBattleServerLost();
		}

		private void OnCancelBattleResponseMessage(CancelBattleResponseMessage message)
		{
			if (message.Successful)
			{
				this._handler.OnCancelJoiningBattle();
				this.CurrentState = LobbyClient.State.AtLobby;
				return;
			}
			if (this.CurrentState == LobbyClient.State.RequestingToCancelSearchBattle)
			{
				this.CurrentState = LobbyClient.State.SearchingBattle;
			}
		}

		private void OnRejoinRequestRejectedMessage(RejoinRequestRejectedMessage message)
		{
			this.CurrentState = LobbyClient.State.AtLobby;
			this._handler.OnRejoinRequestRejected();
		}

		private void OnCancelFindGameMessage(CancelFindGameMessage message)
		{
			if (this.CurrentState == LobbyClient.State.SearchingBattle)
			{
				this.CancelFindGame();
			}
		}

		private void OnWhisperMessageReceivedMessage(WhisperReceivedMessage message)
		{
			this._handler.OnWhisperMessageReceived(message.FromPlayer, message.ToPlayer, message.Message);
		}

		private void OnClanMessageReceivedMessage(ClanMessageReceivedMessage message)
		{
			this._handler.OnClanMessageReceived(message.PlayerName, message.Message);
		}

		private void OnChannelMessageReceivedMessage(ChannelMessageReceivedMessage message)
		{
			this._handler.OnChannelMessageReceived(message.Channel, message.PlayerName, message.Message);
		}

		private void OnPartyMessageReceivedMessage(PartyMessageReceivedMessage message)
		{
			this._handler.OnPartyMessageReceived(message.PlayerName, message.Message);
		}

		private void OnPlayerQuitFromMatchmakerGameResult(PlayerQuitFromMatchmakerGameResult message)
		{
			if (this.CurrentState == LobbyClient.State.QuittingFromBattle)
			{
				this.CurrentState = LobbyClient.State.AtLobby;
			}
		}

		private void OnEnterBattleWithPartyAnswerMessage(EnterBattleWithPartyAnswer message)
		{
			if (message.Successful)
			{
				if (this.CurrentState == LobbyClient.State.AtLobby || this.CurrentState == LobbyClient.State.RequestingToSearchBattle)
				{
					this.CurrentState = LobbyClient.State.SearchingBattle;
				}
				else if (this.CurrentState != LobbyClient.State.SearchingBattle)
				{
					LobbyClient.State currentState = this.CurrentState;
				}
				this._handler.OnEnterBattleWithPartyAnswer(message.SelectedAndEnabledGameTypes);
				return;
			}
			this.CurrentState = LobbyClient.State.AtLobby;
		}

		private void OnJoinCustomGameResultMessage(JoinCustomGameResultMessage message)
		{
			if (!message.Success && message.Response == CustomGameJoinResponse.AlreadyRequestedWaitingForServerResponse)
			{
				if (this._handler != null)
				{
					this._handler.OnSystemMessageReceived(new TextObject("{=ivKntfNA}Already requested to join, waiting for server response", null).ToString());
					return;
				}
			}
			else
			{
				if (message.Success)
				{
					message.JoinGameData.GameServerProperties.CheckAndReplaceProxyAddress(base.Application.ProxyAddressMap);
					this.CurrentState = LobbyClient.State.InCustomGame;
					this.LastBattleServerAddressForClient = message.JoinGameData.GameServerProperties.Address;
					this.LastBattleServerPortForClient = (ushort)message.JoinGameData.GameServerProperties.Port;
					this.CurrentMatchId = message.MatchId;
				}
				else
				{
					this.CurrentState = LobbyClient.State.AtLobby;
					if (this._handler != null)
					{
						this._handler.OnJoinCustomGameFailureResponse(message.Response);
					}
				}
				if (this._handler != null)
				{
					this._handler.OnJoinCustomGameResponse(message.Success, message.JoinGameData, message.Response);
				}
			}
		}

		private void OnClientWantsToConnectCustomGameMessage(ClientWantsToConnectCustomGameMessage message)
		{
			this.AssertCanPerformLobbyActions();
			List<PlayerJoinGameResponseDataFromHost> list = new List<PlayerJoinGameResponseDataFromHost>();
			PlayerJoinGameData[] playerJoinGameData = message.PlayerJoinGameData;
			for (int i = 0; i < playerJoinGameData.Length; i++)
			{
				if (playerJoinGameData[i] != null)
				{
					List<PlayerJoinGameData> list2 = new List<PlayerJoinGameData>();
					PlayerJoinGameData playerJoinGameData2 = playerJoinGameData[i];
					Guid? guid = playerJoinGameData2.PartyId;
					if (guid == null)
					{
						list2.Add(playerJoinGameData2);
					}
					else
					{
						for (int j = i; j < playerJoinGameData.Length; j++)
						{
							PlayerJoinGameData playerJoinGameData3 = playerJoinGameData[j];
							guid = playerJoinGameData2.PartyId;
							if (guid.Equals((playerJoinGameData3 != null) ? playerJoinGameData3.PartyId : null))
							{
								list2.Add(playerJoinGameData3);
								playerJoinGameData[j] = null;
							}
						}
					}
					if (this._handler != null)
					{
						PlayerJoinGameResponseDataFromHost[] array = this._handler.OnClientWantsToConnectCustomGame(list2.ToArray(), message.Password);
						list.AddRange(array);
					}
				}
			}
			this.ResponseCustomGameClientConnection(list.ToArray());
		}

		private void OnClientQuitFromCustomGameMessage(ClientQuitFromCustomGameMessage message)
		{
			if (this._handler != null)
			{
				this._handler.OnClientQuitFromCustomGame(message.PlayerId);
			}
		}

		private void OnEnterCustomBattleWithPartyAnswerMessage(EnterCustomBattleWithPartyAnswer message)
		{
			if (message.Successful)
			{
				if (this.CurrentState == LobbyClient.State.AtLobby)
				{
					this.CurrentState = LobbyClient.State.WaitingToJoinCustomGame;
				}
				this._handler.OnEnterCustomBattleWithPartyAnswer();
				return;
			}
			this.CurrentState = LobbyClient.State.AtLobby;
		}

		private void OnPlayerRemovedFromMatchmakerGameMessage(PlayerRemovedFromMatchmakerGame message)
		{
			this.CurrentState = LobbyClient.State.AtLobby;
			if (this._handler != null)
			{
				this._handler.OnRemovedFromMatchmakerGame(message.DisconnectType);
			}
		}

		private void OnPlayerRemovedFromCustomGame(PlayerRemovedFromCustomGame message)
		{
			this.CurrentState = LobbyClient.State.AtLobby;
			if (this._handler != null)
			{
				this._handler.OnRemovedFromCustomGame(message.DisconnectType);
			}
		}

		private void OnSystemMessage(SystemMessage message)
		{
			this._handler.OnSystemMessageReceived(message.GetDescription().ToString());
		}

		private void OnAdminMessage(AdminMessage message)
		{
			this._handler.OnAdminMessageReceived(message.Message);
		}

		private void OnInvitationToPartyMessage(InvitationToPartyMessage message)
		{
			this.IsPartyInvitationPopupActive = true;
			this._handler.OnPartyInvitationReceived(message.InviterPlayerName, message.InviterPlayerId);
		}

		private void OnPartyInvitationInvalidMessage(PartyInvitationInvalidMessage message)
		{
			this.IsPartyInvitationPopupActive = false;
			this._handler.OnPartyInvitationInvalidated();
		}

		private void OnPlayerInvitedToPartyMessage(PlayerInvitedToPartyMessage message)
		{
			this.PlayersInParty.Add(new PartyPlayerInLobbyClient(message.PlayerId, message.PlayerName, false));
			this._handler.OnPlayerInvitedToParty(message.PlayerId);
		}

		private void OnPlayerAddedToPartyMessage(PlayersAddedToPartyMessage message)
		{
			foreach (ValueTuple<PlayerId, string, bool> valueTuple in message.Players)
			{
				PlayerId playerId = valueTuple.Item1;
				string item = valueTuple.Item2;
				bool item2 = valueTuple.Item3;
				PartyPlayerInLobbyClient partyPlayerInLobbyClient = this.PlayersInParty.Find((PartyPlayerInLobbyClient p) => p.PlayerId == playerId);
				if (partyPlayerInLobbyClient != null)
				{
					partyPlayerInLobbyClient.SetAtParty();
				}
				else
				{
					partyPlayerInLobbyClient = new PartyPlayerInLobbyClient(playerId, item, item2);
					this.PlayersInParty.Add(partyPlayerInLobbyClient);
					partyPlayerInLobbyClient.SetAtParty();
				}
				if (playerId != this.PlayerID)
				{
					RecentPlayersManager.AddOrUpdatePlayerEntry(playerId, item, InteractionType.InPartyTogether, -1);
				}
			}
			foreach (ValueTuple<PlayerId, string> valueTuple2 in message.InvitedPlayers)
			{
				PlayerId item3 = valueTuple2.Item1;
				string item4 = valueTuple2.Item2;
				this.PlayersInParty.Add(new PartyPlayerInLobbyClient(item3, item4, false));
			}
			this._handler.OnPlayersAddedToParty(message.Players, message.InvitedPlayers);
		}

		private void OnPlayerRemovedFromPartyMessage(PlayerRemovedFromPartyMessage message)
		{
			if (message.PlayerId == this._playerId)
			{
				this.PlayersInParty.Clear();
			}
			else
			{
				this.PlayersInParty.RemoveAll((PartyPlayerInLobbyClient partyPlayer) => partyPlayer.PlayerId == message.PlayerId);
			}
			this._handler.OnPlayerRemovedFromParty(message.PlayerId, message.Reason);
		}

		private void OnPlayerAssignedPartyLeaderMessage(PlayerAssignedPartyLeaderMessage message)
		{
			PartyPlayerInLobbyClient partyPlayerInLobbyClient = this.PlayersInParty.FirstOrDefault((PartyPlayerInLobbyClient p) => p.IsPartyLeader);
			if (partyPlayerInLobbyClient != null)
			{
				partyPlayerInLobbyClient.SetMember();
			}
			PartyPlayerInLobbyClient partyPlayerInLobbyClient2 = this.PlayersInParty.FirstOrDefault((PartyPlayerInLobbyClient partyPlayer) => partyPlayer.PlayerId == message.PartyLeaderId);
			if (partyPlayerInLobbyClient2 != null)
			{
				partyPlayerInLobbyClient2.SetLeader();
			}
			else
			{
				this.KickPlayerFromParty(this.PlayerID);
			}
			this._handler.OnPlayerAssignedPartyLeader(message.PartyLeaderId);
		}

		private void OnPlayerSuggestedToPartyMessage(PlayerSuggestedToPartyMessage message)
		{
			ILobbyClientSessionHandler handler = this._handler;
			if (handler == null)
			{
				return;
			}
			handler.OnPlayerSuggestedToParty(message.PlayerId, message.PlayerName, message.SuggestingPlayerId, message.SuggestingPlayerName);
		}

		private void OnUpdatePlayerDataMessage(UpdatePlayerDataMessage updatePlayerDataMessage)
		{
			this.PlayerData = updatePlayerDataMessage.PlayerData;
			if (this._handler != null)
			{
				this._handler.OnPlayerDataReceived(this.PlayerData);
			}
		}

		private void OnServerStatusMessage(ServerStatusMessage serverStatusMessage)
		{
			this._serverStatusTimer.Restart();
			this._serverStatus = serverStatusMessage.ServerStatus;
			if (!this.IsAbleToSearchForGame && this.CurrentState == LobbyClient.State.SearchingBattle)
			{
				this.CancelFindGame();
			}
			if (this._handler != null)
			{
				this._handler.OnServerStatusReceived(this._serverStatus);
				LobbyClient.FriendListCheckDelay = this._serverStatus.FriendListUpdatePeriod * 1000;
			}
		}

		private void OnFriendListMessage(FriendListMessage friendListMessage)
		{
			this._friendListTimer.Restart();
			this.FriendInfos = friendListMessage.Friends;
			if (this._handler != null)
			{
				this._handler.OnFriendListReceived(friendListMessage.Friends);
			}
		}

		private void OnMatchmakerDisabledMessage(MatchmakerDisabledMessage matchmakerDisabledMessage)
		{
			if (matchmakerDisabledMessage.RemainingTime > 0)
			{
				this._matchmakerBlockedTime = DateTime.Now.AddSeconds((double)matchmakerDisabledMessage.RemainingTime);
				return;
			}
			this._matchmakerBlockedTime = DateTime.MinValue;
		}

		private void OnClanCreationRequestMessage(ClanCreationRequestMessage clanCreationRequestMessage)
		{
			if (this._handler != null)
			{
				this._handler.OnClanInvitationReceived(clanCreationRequestMessage.ClanName, clanCreationRequestMessage.ClanTag, true);
			}
		}

		private void OnClanCreationRequestAnsweredMessage(ClanCreationRequestAnsweredMessage clanCreationRequestAnsweredMessage)
		{
			if (this._handler != null)
			{
				this._handler.OnClanInvitationAnswered(clanCreationRequestAnsweredMessage.PlayerId, clanCreationRequestAnsweredMessage.ClanCreationAnswer);
			}
		}

		private void OnClanCreationSuccessfulMessage(ClanCreationSuccessfulMessage clanCreationSuccessfulMessage)
		{
			if (this._handler != null)
			{
				this._handler.OnClanCreationSuccessful();
			}
		}

		private void OnClanCreationFailedMessage(ClanCreationFailedMessage clanCreationFailedMessage)
		{
			if (this._handler != null)
			{
				this._handler.OnClanCreationFailed();
			}
		}

		private void OnCreateClanAnswerMessage(CreateClanAnswerMessage createClanAnswerMessage)
		{
			if (createClanAnswerMessage.Successful)
			{
				this._handler.OnClanCreationStarted();
			}
		}

		public void SendWhisper(string playerName, string message)
		{
		}

		private void OnRecentPlayerStatusesMessage(RecentPlayerStatusesMessage message)
		{
			if (this._handler != null)
			{
				this._handler.OnRecentPlayerStatusesReceived(message.Friends);
			}
		}

		public void FleeBattle()
		{
			this.CheckAndSendMessage(new RejoinBattleRequestMessage(false));
		}

		public void SendPartyMessage(string message)
		{
		}

		public void SendChannelMessage(Guid roomId, string message)
		{
			this.ChatManager.SendMessage(roomId, message);
		}

		private void OnClanInfoChangedMessage(ClanInfoChangedMessage clanInfoChangedMessage)
		{
			this.UpdateClanInfo(clanInfoChangedMessage.ClanHomeInfo);
		}

		protected override void OnTick()
		{
			if (this.LoggedIn && !this.IsInGame)
			{
				if (this._serverStatusTimer != null && this._serverStatusTimer.ElapsedMilliseconds > (long)LobbyClient.ServerStatusCheckDelay)
				{
					this._serverStatusTimer.Restart();
					this.CheckAndSendMessage(new GetServerStatusMessage());
				}
				if (this._friendListTimer != null && this._friendListTimer.ElapsedMilliseconds > (long)LobbyClient.FriendListCheckDelay)
				{
					this._friendListTimer.Restart();
					this.CheckAndSendMessage(new GetFriendListMessage());
					PlayerId[] recentPlayerIds = RecentPlayersManager.GetRecentPlayerIds();
					if (recentPlayerIds.Length != 0)
					{
						this.CheckAndSendMessage(new GetRecentPlayersStatusMessage(recentPlayerIds));
					}
				}
			}
			this.ChatManager.OnTick();
		}

		private void OnInvitationToClanMessage(InvitationToClanMessage invitationToClanMessage)
		{
			if (this._handler != null)
			{
				this._handler.OnClanInvitationReceived(invitationToClanMessage.ClanName, invitationToClanMessage.ClanTag, false);
			}
		}

		public void RejoinBattle()
		{
			this.CheckAndSendMessage(new RejoinBattleRequestMessage(true));
		}

		private void OnJoinPremadeGameAnswerMessage(JoinPremadeGameAnswerMessage joinPremadeGameAnswerMessage)
		{
		}

		public void OnBattleResultsSeen()
		{
			this.AssertCanPerformLobbyActions();
			this.CheckAndSendMessage(new BattleResultSeenMessage());
		}

		private void OnCreatePremadeGameAnswerMessage(CreatePremadeGameAnswerMessage createPremadeGameAnswerMessage)
		{
			if (createPremadeGameAnswerMessage.Successful)
			{
				this._handler.OnPremadeGameCreated();
			}
		}

		private void OnJoinPremadeGameRequestMessage(JoinPremadeGameRequestMessage joinPremadeGameRequestMessage)
		{
			this._handler.OnJoinPremadeGameRequested(joinPremadeGameRequestMessage.ClanName, joinPremadeGameRequestMessage.Sigil, joinPremadeGameRequestMessage.ChallengerPartyId, joinPremadeGameRequestMessage.ChallengerPlayers, joinPremadeGameRequestMessage.ChallengerPartyLeaderId, joinPremadeGameRequestMessage.PremadeGameType);
		}

		private void OnJoinPremadeGameRequestResultMessage(JoinPremadeGameRequestResultMessage joinPremadeGameRequestResultMessage)
		{
			if (joinPremadeGameRequestResultMessage.Successful)
			{
				this._handler.OnJoinPremadeGameRequestSuccessful();
				this.CurrentState = LobbyClient.State.WaitingToJoinPremadeGame;
			}
		}

		private async void OnClanDisbandedMessage(ClanDisbandedMessage clanDisbandedMessage)
		{
			ClanHomeInfo clanHomeInfo = await this.GetClanHomeInfo();
			this.UpdateClanInfo(clanHomeInfo);
		}

		private void OnClanGameCreationCancelledMessage(ClanGameCreationCancelledMessage clanGameCreationCancelledMessage)
		{
			this.CurrentState = LobbyClient.State.AtLobby;
			this._handler.OnPremadeGameCreationCancelled();
		}

		private void OnPremadeGameEligibilityStatusMessage(PremadeGameEligibilityStatusMessage premadeGameEligibilityStatusMessage)
		{
			this._handler.OnPremadeGameEligibilityStatusReceived(premadeGameEligibilityStatusMessage.EligibleGameTypes.Length != 0);
			this.IsEligibleToCreatePremadeGame = premadeGameEligibilityStatusMessage.EligibleGameTypes.Length != 0;
		}

		private async void OnKickedFromClan(KickedFromClanMessage kickedFromClanMessage)
		{
			ClanHomeInfo clanHomeInfo = await this.GetClanHomeInfo();
			this.UpdateClanInfo(clanHomeInfo);
		}

		private void OnCustomBattleOverMessage(CustomBattleOverMessage message)
		{
			this.CurrentState = LobbyClient.State.AtLobby;
			this._handler.OnMatchmakerGameOver(message.OldExperience, message.NewExperience, new List<string>(), message.GoldGain, null, null);
		}

		public void AcceptClanInvitation()
		{
			this.CheckAndSendMessage(new AcceptClanInvitationMessage());
		}

		public void DeclineClanInvitation()
		{
			this.CheckAndSendMessage(new DeclineClanInvitationMessage());
		}

		private void OnShowAnnouncementMessage(ShowAnnouncementMessage message)
		{
			ILobbyClientSessionHandler handler = this._handler;
			if (handler == null)
			{
				return;
			}
			handler.OnAnnouncementReceived(message.Announcement);
		}

		public void MarkNotificationAsRead(int notificationID)
		{
			UpdateNotificationsMessage updateNotificationsMessage = new UpdateNotificationsMessage(new int[] { notificationID });
			this.CheckAndSendMessage(updateNotificationsMessage);
		}

		private void OnRejoinBattleRequestAnswerMessage(RejoinBattleRequestAnswerMessage rejoinBattleRequestAnswerMessage)
		{
			this._handler.OnRejoinBattleRequestAnswered(rejoinBattleRequestAnswerMessage.IsSuccessful);
			if (rejoinBattleRequestAnswerMessage.IsSuccessful && rejoinBattleRequestAnswerMessage.IsRejoinAccepted)
			{
				this.CurrentState = LobbyClient.State.SearchingBattle;
			}
		}

		public void AcceptClanCreationRequest()
		{
			this.CheckAndSendMessage(new AcceptClanCreationRequestMessage());
		}

		private void OnPendingBattleRejoinMessage(PendingBattleRejoinMessage pendingBattleRejoinMessage)
		{
			this._handler.OnPendingRejoin();
		}

		private void OnSigilChangeAnswerMessage(SigilChangeAnswerMessage message)
		{
			if (message.Successful)
			{
				ILobbyClientSessionHandler handler = this._handler;
				if (handler == null)
				{
					return;
				}
				handler.OnSigilChanged();
			}
		}

		public void DeclineClanCreationRequest()
		{
			this.CheckAndSendMessage(new DeclineClanCreationRequestMessage());
		}

		public void PromoteToClanLeader(PlayerId playerId, bool dontUseNameForUnknownPlayer)
		{
			this.CheckAndSendMessage(new PromoteToClanLeaderMessage(playerId, dontUseNameForUnknownPlayer));
		}

		private void OnLobbyNotificationsMessage(LobbyNotificationsMessage message)
		{
			this._handler.OnNotificationsReceived(message.Notifications);
		}

		public void KickFromClan(PlayerId playerId)
		{
			this.CheckAndSendMessage(new KickFromClanMessage(playerId));
		}

		public async Task<CheckClanParameterValidResult> ClanNameExists(string clanName)
		{
			return await base.CallFunction<CheckClanParameterValidResult>(new CheckClanNameValidMessage(clanName));
		}

		public async Task<CheckClanParameterValidResult> ClanTagExists(string clanTag)
		{
			return await base.CallFunction<CheckClanParameterValidResult>(new CheckClanTagValidMessage(clanTag));
		}

		public async Task<ClanHomeInfo> GetClanHomeInfo()
		{
			GetClanHomeInfoResult getClanHomeInfoResult = await base.CallFunction<GetClanHomeInfoResult>(new GetClanHomeInfoMessage());
			ClanHomeInfo clanHomeInfo;
			if (getClanHomeInfoResult != null)
			{
				this.UpdateClanInfo(getClanHomeInfoResult.ClanHomeInfo);
				clanHomeInfo = getClanHomeInfoResult.ClanHomeInfo;
			}
			else
			{
				this.UpdateClanInfo(null);
				clanHomeInfo = null;
			}
			return clanHomeInfo;
		}

		public void SendChatMessage(Guid roomId, string message)
		{
			this.ChatManager.SendMessage(roomId, message);
		}

		public void JoinChannel(ChatChannelType channel)
		{
		}

		public void AssignAsClanOfficer(PlayerId playerId, bool dontUseNameForUnknownPlayer)
		{
			this.CheckAndSendMessage(new AssignAsClanOfficerMessage(playerId, dontUseNameForUnknownPlayer));
		}

		public void RemoveClanOfficerRoleForPlayer(PlayerId playerId)
		{
			this.CheckAndSendMessage(new RemoveClanOfficerRoleForPlayerMessage(playerId));
		}

		public void LeaveChannel(ChatChannelType channel)
		{
		}

		private void UpdateClanInfo(ClanHomeInfo clanHomeInfo)
		{
			this.PlayersInClan.Clear();
			this.PlayerInfosInClan.Clear();
			this.ClanID = Guid.Empty;
			this.ClanInfo = null;
			this.ClanHomeInfo = clanHomeInfo;
			if (clanHomeInfo != null)
			{
				if (clanHomeInfo.IsInClan)
				{
					foreach (ClanPlayer clanPlayer in clanHomeInfo.ClanInfo.Players)
					{
						this.PlayersInClan.Add(clanPlayer);
					}
					foreach (ClanPlayerInfo clanPlayerInfo in clanHomeInfo.ClanPlayerInfos)
					{
						this.PlayerInfosInClan.Add(clanPlayerInfo);
					}
					this.ClanID = clanHomeInfo.ClanInfo.ClanId;
				}
				this.ClanInfo = clanHomeInfo.ClanInfo;
			}
			if (this._handler != null)
			{
				this._handler.OnClanInfoChanged();
			}
		}

		public async Task<ClanLeaderboardInfo> GetClanLeaderboardInfo()
		{
			GetClanLeaderboardResult getClanLeaderboardResult = await base.CallFunction<GetClanLeaderboardResult>(new GetClanLeaderboardMessage());
			ClanLeaderboardInfo clanLeaderboardInfo;
			if (getClanLeaderboardResult != null)
			{
				clanLeaderboardInfo = getClanLeaderboardResult.ClanLeaderboardInfo;
			}
			else
			{
				clanLeaderboardInfo = null;
			}
			return clanLeaderboardInfo;
		}

		public async Task<ClanInfo> GetPlayerClanInfo(PlayerId playerId)
		{
			GetPlayerClanInfoResult getPlayerClanInfoResult = await base.CallFunction<GetPlayerClanInfoResult>(new GetPlayerClanInfo(playerId));
			ClanInfo clanInfo;
			if (((getPlayerClanInfoResult != null) ? getPlayerClanInfoResult.ClanInfo : null) != null)
			{
				clanInfo = getPlayerClanInfoResult.ClanInfo;
			}
			else
			{
				clanInfo = null;
			}
			return clanInfo;
		}

		public void SendClanMessage(string message)
		{
		}

		public async Task<PremadeGameList> GetPremadeGameList()
		{
			GetPremadeGameListResult getPremadeGameListResult = await base.CallFunction<GetPremadeGameListResult>(new GetPremadeGameListMessage());
			PremadeGameList premadeGameList;
			if (getPremadeGameListResult != null)
			{
				this.AvailablePremadeGames = getPremadeGameListResult.GameList;
				if (this._handler != null)
				{
					this._handler.OnPremadeGameListReceived();
				}
				premadeGameList = getPremadeGameListResult.GameList;
			}
			else
			{
				premadeGameList = null;
			}
			return premadeGameList;
		}

		public async Task<AvailableScenes> GetAvailableScenes()
		{
			GetAvailableScenesResult getAvailableScenesResult = await base.CallFunction<GetAvailableScenesResult>(new GetAvailableScenesMessage());
			AvailableScenes availableScenes;
			if (getAvailableScenesResult != null)
			{
				availableScenes = getAvailableScenesResult.AvailableScenes;
			}
			else
			{
				availableScenes = null;
			}
			return availableScenes;
		}

		public void SetClanInformationText(string informationText)
		{
			this.CheckAndSendMessage(new SetClanInformationMessage(informationText));
		}

		public void AddClanAnnouncement(string announcement)
		{
			this.CheckAndSendMessage(new AddClanAnnouncementMessage(announcement));
		}

		public void EditClanAnnouncement(int announcementId, string text)
		{
			this.CheckAndSendMessage(new EditClanAnnouncementMessage(announcementId, text));
		}

		public void RemoveClanAnnouncement(int announcementId)
		{
			this.CheckAndSendMessage(new RemoveClanAnnouncementMessage(announcementId));
		}

		public void ChangeClanFaction(string faction)
		{
			this.CheckAndSendMessage(new ChangeClanFactionMessage(faction));
		}

		public void ChangeClanSigil(string sigil)
		{
			this.CheckAndSendMessage(new ChangeClanSigilMessage(sigil));
		}

		public void DestroyClan()
		{
			this.CheckAndSendMessage(new DestroyClanMessage());
		}

		public void InviteToClan(PlayerId invitedPlayerId, bool dontUseNameForUnknownPlayer)
		{
			this.CheckAndSendMessage(new InviteToClanMessage(invitedPlayerId, dontUseNameForUnknownPlayer));
		}

		public async void CreatePremadeGame(string name, string gameType, string mapName, string factionA, string factionB, string password, PremadeGameType premadeGameType)
		{
			this.CurrentState = LobbyClient.State.WaitingToCreatePremadeGame;
			string text = ((!string.IsNullOrEmpty(password)) ? Common.CalculateMD5Hash(password) : null);
			CreatePremadeGameMessageResult createPremadeGameMessageResult = await base.CallFunction<CreatePremadeGameMessageResult>(new CreatePremadeGameMessage(name, gameType, mapName, factionA, factionB, text, premadeGameType));
			if (createPremadeGameMessageResult == null || !createPremadeGameMessageResult.Successful)
			{
				this.CurrentState = LobbyClient.State.AtLobby;
			}
		}

		public void CancelCreatingPremadeGame()
		{
			this.CheckAndSendMessage(new CancelCreatingPremadeGameMessage());
		}

		public void RequestToJoinPremadeGame(Guid gameId, string password)
		{
			string text = Common.CalculateMD5Hash(password);
			this.CheckAndSendMessage(new RequestToJoinPremadeGameMessage(gameId, text));
		}

		public void AcceptJoinPremadeGameRequest(Guid partyId)
		{
			this.CheckAndSendMessage(new AcceptJoinPremadeGameRequestMessage(partyId));
		}

		public void DeclineJoinPremadeGameRequest(Guid partyId)
		{
			this.CheckAndSendMessage(new DeclineJoinPremadeGameRequestMessage(partyId));
		}

		public void InviteToParty(PlayerId playerId, bool dontUseNameForUnknownPlayer)
		{
			this.CheckAndSendMessage(new InviteToPartyMessage(playerId, dontUseNameForUnknownPlayer));
		}

		public void DisbandParty()
		{
			this.CheckAndSendMessage(new DisbandPartyMessage());
		}

		public void Test_CreateChatRoom(string name)
		{
			this.CheckAndSendMessage(new Test_CreateChatRoomMessage(name));
		}

		public void Test_AddChatRoomUser(string name)
		{
			this.CheckAndSendMessage(new Test_AddChatRoomUser(name));
		}

		public void Test_RemoveChatRoomUser(string name)
		{
			this.CheckAndSendMessage(new Test_RemoveChatRoomUser(name));
		}

		public void Test_DeleteChatRoom(Guid id)
		{
			this.CheckAndSendMessage(new Test_DeleteChatRoomMessage(id));
		}

		public IEnumerable<string> Test_ListChatRoomIds()
		{
			return this.ChatManager.Rooms.Select((ChatRoomInformationForClient room) => room.RoomId.ToString());
		}

		public void KickPlayerFromParty(PlayerId playerId)
		{
			this.CheckAndSendMessage(new KickPlayerFromPartyMessage(playerId));
		}

		public void OnPlayerNameUpdated(string name)
		{
			this._userName = name;
		}

		public void ToggleUseClanSigil(bool isUsed)
		{
			this.CheckAndSendMessage(new UpdateUsingClanSigil(isUsed));
		}

		public void PromotePlayerToPartyLeader(PlayerId playerId)
		{
			this.CheckAndSendMessage(new PromotePlayerToPartyLeaderMessage(playerId));
		}

		public void ChangeSigil(string sigilId)
		{
			this.CheckAndSendMessage(new ChangePlayerSigilMessage(sigilId));
		}

		public async Task<bool> InviteToPlatformSession(PlayerId playerId)
		{
			bool flag = false;
			if (this._handler != null)
			{
				flag = await this._handler.OnInviteToPlatformSession(playerId);
			}
			return flag;
		}

		public async void EndCustomGame()
		{
			await base.CallFunction<EndHostingCustomGameResult>(new EndHostingCustomGameMessage());
			if (this._handler != null)
			{
				this._handler.OnCustomGameEnd();
			}
			this.CurrentState = LobbyClient.State.AtLobby;
		}

		public async void RegisterCustomGame(string gameModule, string gameType, string serverName, int maxPlayerCount, string map, string uniqueMapId, string gamePassword, string adminPassword, int port)
		{
			this.CustomGameType = gameType;
			this.CustomGameScene = map;
			this.CurrentState = LobbyClient.State.WaitingToRegisterCustomGame;
			RegisterCustomGameResult registerCustomGameResult = await base.CallFunction<RegisterCustomGameResult>(new RegisterCustomGameMessage(gameModule, gameType, serverName, maxPlayerCount, map, uniqueMapId, gamePassword, adminPassword, port));
			Debug.Print("Register custom game server response received", 0, Debug.DebugColor.White, 17592186044416UL);
			if (registerCustomGameResult.Success)
			{
				this.CurrentState = LobbyClient.State.HostingCustomGame;
				if (this._handler != null)
				{
					this._handler.OnRegisterCustomGameServerResponse();
				}
			}
			else
			{
				this.CurrentState = LobbyClient.State.AtLobby;
			}
		}

		public void ResponseCustomGameClientConnection(PlayerJoinGameResponseDataFromHost[] playerJoinData)
		{
			base.SendMessage(new ResponseCustomGameClientConnectionMessage(playerJoinData));
		}

		public void AcceptPartyInvitation()
		{
			this.IsPartyInvitationPopupActive = false;
			this.CheckAndSendMessage(new AcceptPartyInvitationMessage());
		}

		public void DeclinePartyInvitation()
		{
			this.IsPartyInvitationPopupActive = false;
			this.CheckAndSendMessage(new DeclinePartyInvitationMessage());
		}

		public void UpdateCharacter(BodyProperties bodyProperties, bool isFemale)
		{
			this.AssertCanPerformLobbyActions();
			base.SendMessage(new UpdateCharacterMessage(bodyProperties, isFemale));
			if (this.CanPerformLobbyActions)
			{
				this.PlayerData.BodyProperties = bodyProperties;
				this.PlayerData.IsFemale = isFemale;
			}
		}

		public async Task<bool> UpdateShownBadgeId(string shownBadgeId)
		{
			this.AssertCanPerformLobbyActions();
			UpdateShownBadgeIdMessageResult updateShownBadgeIdMessageResult = await base.CallFunction<UpdateShownBadgeIdMessageResult>(new UpdateShownBadgeIdMessage(shownBadgeId));
			if (updateShownBadgeIdMessageResult != null && updateShownBadgeIdMessageResult.Successful)
			{
				this.PlayerData.ShownBadgeId = shownBadgeId;
			}
			return updateShownBadgeIdMessageResult != null && updateShownBadgeIdMessageResult.Successful;
		}

		public async Task<AnotherPlayerData> GetAnotherPlayerState(PlayerId playerId)
		{
			this.AssertCanPerformLobbyActions();
			GetAnotherPlayerStateMessageResult getAnotherPlayerStateMessageResult = await base.CallFunction<GetAnotherPlayerStateMessageResult>(new GetAnotherPlayerStateMessage(playerId));
			AnotherPlayerData anotherPlayerData;
			if (getAnotherPlayerStateMessageResult != null)
			{
				anotherPlayerData = getAnotherPlayerStateMessageResult.AnotherPlayerData;
			}
			else
			{
				anotherPlayerData = new AnotherPlayerData(AnotherPlayerState.NoAnswer, 0);
			}
			return anotherPlayerData;
		}

		public async Task<PlayerData> GetAnotherPlayerData(PlayerId playerID)
		{
			this.AssertCanPerformLobbyActions();
			await this.WaitForPendingRequestCompletion(LobbyClient.PendingRequest.PlayerData, playerID);
			PlayerData playerData;
			PlayerData playerData2;
			if (this._cachedPlayerDatas.TryGetValue(playerID, out playerData))
			{
				playerData2 = playerData;
			}
			else
			{
				GetAnotherPlayerDataMessageResult getAnotherPlayerDataMessageResult = await this.CreatePendingRequest<GetAnotherPlayerDataMessageResult>(LobbyClient.PendingRequest.PlayerData, playerID, base.CallFunction<GetAnotherPlayerDataMessageResult>(new GetAnotherPlayerDataMessage(playerID)));
				if (((getAnotherPlayerDataMessageResult != null) ? getAnotherPlayerDataMessageResult.AnotherPlayerData : null) != null)
				{
					this._cachedPlayerDatas[playerID] = getAnotherPlayerDataMessageResult.AnotherPlayerData;
				}
				playerData2 = ((getAnotherPlayerDataMessageResult != null) ? getAnotherPlayerDataMessageResult.AnotherPlayerData : null);
			}
			return playerData2;
		}

		public async Task<MatchmakingQueueStats> GetPlayerCountInQueue()
		{
			GetPlayerCountInQueueResult getPlayerCountInQueueResult = await base.CallFunction<GetPlayerCountInQueueResult>(new GetPlayerCountInQueue());
			MatchmakingQueueStats matchmakingQueueStats;
			if (getPlayerCountInQueueResult != null)
			{
				matchmakingQueueStats = getPlayerCountInQueueResult.MatchmakingQueueStats;
			}
			else
			{
				matchmakingQueueStats = MatchmakingQueueStats.Empty;
			}
			return matchmakingQueueStats;
		}

		public async Task<List<ValueTuple<PlayerId, AnotherPlayerData>>> GetOtherPlayersState(List<PlayerId> players)
		{
			this.AssertCanPerformLobbyActions();
			GetOtherPlayersStateMessageResult getOtherPlayersStateMessageResult = await base.CallFunction<GetOtherPlayersStateMessageResult>(new GetOtherPlayersStateMessage(players));
			return (getOtherPlayersStateMessageResult != null) ? getOtherPlayersStateMessageResult.States : null;
		}

		public async Task<MatchmakingWaitTimeStats> GetMatchmakingWaitTimes()
		{
			GetAverageMatchmakingWaitTimesResult getAverageMatchmakingWaitTimesResult = await base.CallFunction<GetAverageMatchmakingWaitTimesResult>(new GetAverageMatchmakingWaitTimesMessage());
			MatchmakingWaitTimeStats matchmakingWaitTimeStats;
			if (getAverageMatchmakingWaitTimesResult != null)
			{
				matchmakingWaitTimeStats = getAverageMatchmakingWaitTimesResult.MatchmakingWaitTimeStats;
			}
			else
			{
				matchmakingWaitTimeStats = MatchmakingWaitTimeStats.Empty;
			}
			return matchmakingWaitTimeStats;
		}

		public async Task<Badge[]> GetPlayerBadges()
		{
			GetPlayerBadgesMessageResult getPlayerBadgesMessageResult = await base.CallFunction<GetPlayerBadgesMessageResult>(new GetPlayerBadgesMessage());
			List<Badge> list = new List<Badge>();
			if (getPlayerBadgesMessageResult != null)
			{
				string[] badges = getPlayerBadgesMessageResult.Badges;
				for (int i = 0; i < badges.Length; i++)
				{
					Badge byId = BadgeManager.GetById(badges[i]);
					if (byId != null)
					{
						list.Add(byId);
					}
				}
			}
			return list.ToArray();
		}

		public async Task<PlayerStatsBase[]> GetPlayerStats(PlayerId playerID)
		{
			PlayerStatsBase[] array;
			PlayerStatsBase[] array2;
			if (this._cachedPlayerStats.TryGetValue(playerID, out array))
			{
				array2 = array;
			}
			else
			{
				GetPlayerStatsMessageResult getPlayerStatsMessageResult = await base.CallFunction<GetPlayerStatsMessageResult>(new GetPlayerStatsMessage(playerID));
				if (((getPlayerStatsMessageResult != null) ? getPlayerStatsMessageResult.PlayerStats : null) != null)
				{
					this._cachedPlayerStats[playerID] = getPlayerStatsMessageResult.PlayerStats;
				}
				array2 = ((getPlayerStatsMessageResult != null) ? getPlayerStatsMessageResult.PlayerStats : null);
			}
			return array2;
		}

		public async Task<GameTypeRankInfo[]> GetGameTypeRankInfo(PlayerId playerID)
		{
			await this.WaitForPendingRequestCompletion(LobbyClient.PendingRequest.RankInfo, playerID);
			GameTypeRankInfo[] array;
			GameTypeRankInfo[] array2;
			if (this._cachedRankInfos.TryGetValue(playerID, out array))
			{
				array2 = array;
			}
			else
			{
				GetPlayerGameTypeRankInfoMessageResult getPlayerGameTypeRankInfoMessageResult = await this.CreatePendingRequest<GetPlayerGameTypeRankInfoMessageResult>(LobbyClient.PendingRequest.RankInfo, playerID, base.CallFunction<GetPlayerGameTypeRankInfoMessageResult>(new GetPlayerGameTypeRankInfoMessage(playerID)));
				if (((getPlayerGameTypeRankInfoMessageResult != null) ? getPlayerGameTypeRankInfoMessageResult.GameTypeRankInfo : null) != null)
				{
					this._cachedRankInfos[playerID] = getPlayerGameTypeRankInfoMessageResult.GameTypeRankInfo;
				}
				array2 = ((getPlayerGameTypeRankInfoMessageResult != null) ? getPlayerGameTypeRankInfoMessageResult.GameTypeRankInfo : null);
			}
			return array2;
		}

		public async Task<PlayerLeaderboardData[]> GetRankedLeaderboard(string gameType)
		{
			GetRankedLeaderboardMessageResult getRankedLeaderboardMessageResult = await base.CallFunction<GetRankedLeaderboardMessageResult>(new GetRankedLeaderboardMessage(gameType));
			return (getRankedLeaderboardMessageResult != null) ? getRankedLeaderboardMessageResult.LeaderboardPlayers : null;
		}

		public void SendCreateClanMessage(string clanName, string clanTag, string clanFaction, string clanSigil)
		{
			this.AssertCanPerformLobbyActions();
			base.SendMessage(new CreateClanMessage(clanName, clanTag, clanFaction, clanSigil));
		}

		public async Task<bool> CanLogin()
		{
			this.CurrentState = LobbyClient.State.Working;
			TaskAwaiter<bool> taskAwaiter = Gatekeeper.IsGenerous().GetAwaiter();
			if (!taskAwaiter.IsCompleted)
			{
				await taskAwaiter;
				TaskAwaiter<bool> taskAwaiter2;
				taskAwaiter = taskAwaiter2;
				taskAwaiter2 = default(TaskAwaiter<bool>);
			}
			bool flag;
			if (taskAwaiter.GetResult())
			{
				this.CurrentState = LobbyClient.State.Idle;
				flag = true;
			}
			else
			{
				await Task.Delay(new Random().Next() % 3000 + 1000);
				this.CurrentState = LobbyClient.State.Idle;
				flag = false;
			}
			return flag;
		}

		public void GetFriendList()
		{
			this.CheckAndSendMessage(new GetFriendListMessage());
		}

		public void AddFriend(PlayerId friendId, bool dontUseNameForUnknownPlayer)
		{
			this.CheckAndSendMessage(new AddFriendMessage(friendId, dontUseNameForUnknownPlayer));
		}

		public void RemoveFriend(PlayerId friendId)
		{
			this.CheckAndSendMessage(new RemoveFriendMessage(friendId));
		}

		public void RespondToFriendRequest(PlayerId playerId, bool dontUseNameForUnknownPlayer, bool isAccepted, bool isBlocked = false)
		{
			this.CheckAndSendMessage(new FriendRequestResponseMessage(playerId, dontUseNameForUnknownPlayer, isAccepted, isBlocked));
		}

		public void ReportPlayer(string gameId, PlayerId player, string playerName, PlayerReportType type, string message)
		{
			Guid guid;
			if (Guid.TryParse(gameId, out guid))
			{
				this.CheckAndSendMessage(new ReportPlayerMessage(guid, player, playerName, type, message));
				return;
			}
			ILobbyClientSessionHandler handler = this._handler;
			if (handler == null)
			{
				return;
			}
			handler.OnSystemMessageReceived(new TextObject("{=dnKQbXIZ}Could not report player: Game does not exist.", null).ToString());
		}

		public void ChangeUsername(string username)
		{
			if ((this.PlayerData == null || this.PlayerData.Username != username) && username != null && username.Length >= Parameters.UsernameMinLength && username.Length <= Parameters.UsernameMaxLength && Common.IsAllLetters(username))
			{
				this.CheckAndSendMessage(new ChangeUsernameMessage(username));
			}
		}

		void IChatClientHandler.OnChatMessageReceived(Guid roomId, string roomName, string playerName, string textMessage, string textColor, MessageType type)
		{
			if (this._handler != null)
			{
				this._handler.OnChatMessageReceived(roomId, roomName, playerName, textMessage, textColor, type);
			}
		}

		public void AddFriendByUsernameAndId(string username, int userId, bool dontUseNameForUnknownPlayer)
		{
			if (username != null && username.Length >= Parameters.UsernameMinLength && username.Length <= Parameters.UsernameMaxLength && Common.IsAllLetters(username) && userId >= 0 && userId <= Parameters.UserIdMax)
			{
				this.CheckAndSendMessage(new AddFriendByUsernameAndIdMessage(username, userId, dontUseNameForUnknownPlayer));
			}
		}

		public async Task<bool> DoesPlayerWithUsernameAndIdExist(string username, int userId)
		{
			bool flag;
			if (username != null && username.Length >= Parameters.UsernameMinLength && username.Length <= Parameters.UsernameMaxLength && Common.IsAllLetters(username) && userId >= 0 && userId <= Parameters.UserIdMax)
			{
				GetPlayerByUsernameAndIdMessageResult getPlayerByUsernameAndIdMessageResult = await base.CallFunction<GetPlayerByUsernameAndIdMessageResult>(new GetPlayerByUsernameAndIdMessage(username, userId));
				flag = getPlayerByUsernameAndIdMessageResult != null && getPlayerByUsernameAndIdMessageResult.PlayerId.IsValid;
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		public bool IsPlayerClanLeader(PlayerId playerID)
		{
			ClanPlayer clanPlayer = this.PlayersInClan.Find((ClanPlayer p) => p.PlayerId == playerID);
			return clanPlayer != null && clanPlayer.Role == ClanPlayerRole.Leader;
		}

		public bool IsPlayerClanOfficer(PlayerId playerID)
		{
			ClanPlayer clanPlayer = this.PlayersInClan.Find((ClanPlayer p) => p.PlayerId == playerID);
			return clanPlayer != null && clanPlayer.Role == ClanPlayerRole.Officer;
		}

		public async Task<bool> UpdateUsedCosmeticItems([TupleElementNames(new string[] { "cosmeticIndex", "isEquipped" })] Dictionary<string, List<ValueTuple<string, bool>>> usedCosmetics)
		{
			List<CosmeticItemInfo> list = new List<CosmeticItemInfo>();
			foreach (string text in usedCosmetics.Keys)
			{
				foreach (ValueTuple<string, bool> valueTuple in usedCosmetics[text])
				{
					CosmeticItemInfo cosmeticItemInfo = new CosmeticItemInfo(text, valueTuple.Item1, valueTuple.Item2);
					list.Add(cosmeticItemInfo);
				}
			}
			UpdateUsedCosmeticItemsMessageResult updateUsedCosmeticItemsMessageResult = await base.CallFunction<UpdateUsedCosmeticItemsMessageResult>(new UpdateUsedCosmeticItemsMessage(list));
			if (updateUsedCosmeticItemsMessageResult != null && updateUsedCosmeticItemsMessageResult.Successful)
			{
				foreach (KeyValuePair<string, List<ValueTuple<string, bool>>> keyValuePair in usedCosmetics)
				{
					if (!string.IsNullOrWhiteSpace(keyValuePair.Key))
					{
						List<string> list2;
						if (!this.UsedCosmetics.TryGetValue(keyValuePair.Key, out list2))
						{
							list2 = new List<string>();
							this._usedCosmetics.Add(keyValuePair.Key, list2);
						}
						foreach (ValueTuple<string, bool> valueTuple2 in keyValuePair.Value)
						{
							string item = valueTuple2.Item1;
							if (valueTuple2.Item2)
							{
								list2.Add(item);
							}
							else
							{
								list2.Remove(item);
							}
						}
					}
				}
			}
			return updateUsedCosmeticItemsMessageResult != null && updateUsedCosmeticItemsMessageResult.Successful;
		}

		[return: TupleElementNames(new string[] { "isSuccessful", "finalGold" })]
		public async Task<ValueTuple<bool, int>> BuyCosmetic(string cosmeticId)
		{
			BuyCosmeticMessageResult buyCosmeticMessageResult = await base.CallFunction<BuyCosmeticMessageResult>(new BuyCosmeticMessage(cosmeticId));
			if (buyCosmeticMessageResult != null && buyCosmeticMessageResult.Successful)
			{
				this._ownedCosmetics.Add(cosmeticId);
			}
			return new ValueTuple<bool, int>(buyCosmeticMessageResult != null && buyCosmeticMessageResult.Successful, (buyCosmeticMessageResult != null) ? buyCosmeticMessageResult.Gold : 0);
		}

		[return: TupleElementNames(new string[] { "isSuccessful", "ownedCosmetics", "usedCosmetics" })]
		public async Task<ValueTuple<bool, List<string>, Dictionary<string, List<string>>>> GetCosmeticsInfo()
		{
			GetUserCosmeticsInfoMessageResult getUserCosmeticsInfoMessageResult = await base.CallFunction<GetUserCosmeticsInfoMessageResult>(new GetUserCosmeticsInfoMessage());
			if (getUserCosmeticsInfoMessageResult != null)
			{
				this._usedCosmetics = getUserCosmeticsInfoMessageResult.UsedCosmetics ?? new Dictionary<string, List<string>>();
				this._ownedCosmetics = getUserCosmeticsInfoMessageResult.OwnedCosmetics ?? new List<string>();
			}
			return new ValueTuple<bool, List<string>, Dictionary<string, List<string>>>(getUserCosmeticsInfoMessageResult != null && getUserCosmeticsInfoMessageResult.Successful, (getUserCosmeticsInfoMessageResult != null) ? getUserCosmeticsInfoMessageResult.OwnedCosmetics : null, (getUserCosmeticsInfoMessageResult != null) ? getUserCosmeticsInfoMessageResult.UsedCosmetics : null);
		}

		public async Task<string> GetDedicatedCustomServerAuthToken()
		{
			GetDedicatedCustomServerAuthTokenMessageResult getDedicatedCustomServerAuthTokenMessageResult = await base.CallFunction<GetDedicatedCustomServerAuthTokenMessageResult>(new GetDedicatedCustomServerAuthTokenMessage());
			return (getDedicatedCustomServerAuthTokenMessageResult != null) ? getDedicatedCustomServerAuthTokenMessageResult.AuthToken : null;
		}

		public async Task<string> GetOfficialServerProviderName()
		{
			GetOfficialServerProviderNameResult getOfficialServerProviderNameResult = await base.CallFunction<GetOfficialServerProviderNameResult>(new GetOfficialServerProviderNameMessage());
			return ((getOfficialServerProviderNameResult != null) ? getOfficialServerProviderNameResult.Name : null) ?? string.Empty;
		}

		public async Task<string> GetPlayerBannerlordID(PlayerId playerId)
		{
			await this.WaitForPendingRequestCompletion(LobbyClient.PendingRequest.BannerlordID, playerId);
			string text;
			string text2;
			if (this._cachedPlayerBannerlordIDs.TryGetValue(playerId, out text))
			{
				text2 = text;
			}
			else
			{
				GetBannerlordIDMessageResult getBannerlordIDMessageResult = await this.CreatePendingRequest<GetBannerlordIDMessageResult>(LobbyClient.PendingRequest.BannerlordID, playerId, base.CallFunction<GetBannerlordIDMessageResult>(new GetBannerlordIDMessage(playerId)));
				if (getBannerlordIDMessageResult != null && getBannerlordIDMessageResult.BannerlordID != null)
				{
					this._cachedPlayerBannerlordIDs[playerId] = getBannerlordIDMessageResult.BannerlordID;
				}
				text2 = ((getBannerlordIDMessageResult != null) ? getBannerlordIDMessageResult.BannerlordID : null) ?? string.Empty;
			}
			return text2;
		}

		public bool IsKnownPlayer(PlayerId playerID)
		{
			bool flag = playerID == this._playerId;
			bool flag2 = this.FriendIDs.Contains(playerID);
			bool flag3 = this.IsInParty && this.PlayersInParty.Any((PartyPlayerInLobbyClient p) => p.PlayerId.Equals(playerID));
			bool flag4 = this.IsInClan && this.PlayersInClan.Any((ClanPlayer p) => p.PlayerId.Equals(playerID));
			return flag || flag2 || flag3 || flag4;
		}

		public async Task<long> GetPingToServer(string IpAddress)
		{
			long num;
			try
			{
				using (Ping ping = new Ping())
				{
					PingReply pingReply = await ping.SendPingAsync(IpAddress, (int)TimeSpan.FromSeconds(15.0).TotalMilliseconds);
					num = ((pingReply.Status != IPStatus.Success) ? (-1L) : pingReply.RoundtripTime);
				}
			}
			catch (Exception)
			{
				num = -1L;
			}
			return num;
		}

		private void AssertCanPerformLobbyActions()
		{
		}

		public async Task<bool> SendPSPlayerJoinedToPlayerSessionMessage(ulong inviterAccountId)
		{
			PSPlayerJoinedToPlayerSessionMessage psplayerJoinedToPlayerSessionMessage = new PSPlayerJoinedToPlayerSessionMessage(inviterAccountId);
			return (await base.CallFunction<PSPlayerJoinedToPlayerSessionMessageResult>(psplayerJoinedToPlayerSessionMessage)).Successful;
		}

		private Task WaitForPendingRequestCompletion(LobbyClient.PendingRequest requestType, PlayerId playerId)
		{
			Task task;
			if (this._pendingPlayerRequests.TryGetValue(new ValueTuple<LobbyClient.PendingRequest, PlayerId>(requestType, playerId), out task))
			{
				return task;
			}
			return Task.CompletedTask;
		}

		private async Task<T> CreatePendingRequest<T>(LobbyClient.PendingRequest requestType, PlayerId playerId, Task<T> requestTask)
		{
			ValueTuple<LobbyClient.PendingRequest, PlayerId> key = new ValueTuple<LobbyClient.PendingRequest, PlayerId>(requestType, playerId);
			T t;
			try
			{
				this._pendingPlayerRequests[key] = requestTask;
				t = await requestTask;
			}
			finally
			{
				this._pendingPlayerRequests.Remove(key);
			}
			return t;
		}

		public const string TestRegionCode = "Test";

		private static readonly int ServerStatusCheckDelay = 30000;

		private static int _friendListCheckDelay;

		private static readonly int CheckForCustomGamesCount = 5;

		private static readonly int CheckForCustomGamesDelay = 5000;

		private ILobbyClientSessionHandler _handler;

		private readonly Stopwatch _serverStatusTimer;

		private readonly Stopwatch _friendListTimer;

		private List<string> _ownedCosmetics;

		private Dictionary<string, List<string>> _usedCosmetics;

		private ServerStatus _serverStatus;

		private DateTime _matchmakerBlockedTime;

		private TextObject _logOutReason;

		private LobbyClient.State _state;

		private string _userName;

		private PlayerId _playerId;

		private List<ModuleInfoModel> _loadedUnofficialModules;

		public List<PlayerId> FriendIDs;

		private TimedDictionaryCache<PlayerId, GameTypeRankInfo[]> _cachedRankInfos;

		private TimedDictionaryCache<PlayerId, PlayerStatsBase[]> _cachedPlayerStats;

		private TimedDictionaryCache<PlayerId, PlayerData> _cachedPlayerDatas;

		private TimedDictionaryCache<PlayerId, string> _cachedPlayerBannerlordIDs;

		private Dictionary<ValueTuple<LobbyClient.PendingRequest, PlayerId>, Task> _pendingPlayerRequests;

		public enum State
		{
			Idle,
			Working,
			Connected,
			SessionRequested,
			AtLobby,
			SearchingToRejoinBattle,
			RequestingToSearchBattle,
			RequestingToCancelSearchBattle,
			SearchingBattle,
			AtBattle,
			QuittingFromBattle,
			WaitingToCreatePremadeGame,
			WaitingToJoinPremadeGame,
			WaitingToRegisterCustomGame,
			HostingCustomGame,
			WaitingToJoinCustomGame,
			InCustomGame
		}

		private enum PendingRequest
		{
			RankInfo,
			PlayerData,
			BannerlordID
		}
	}
}
