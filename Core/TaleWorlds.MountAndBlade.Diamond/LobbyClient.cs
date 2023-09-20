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
	// Token: 0x0200011E RID: 286
	public class LobbyClient : Client<LobbyClient>, IChatClientHandler
	{
		// Token: 0x1700020F RID: 527
		// (get) Token: 0x0600057A RID: 1402 RVA: 0x00008438 File Offset: 0x00006638
		// (set) Token: 0x0600057B RID: 1403 RVA: 0x0000843F File Offset: 0x0000663F
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

		// Token: 0x17000210 RID: 528
		// (get) Token: 0x0600057C RID: 1404 RVA: 0x0000844F File Offset: 0x0000664F
		// (set) Token: 0x0600057D RID: 1405 RVA: 0x00008457 File Offset: 0x00006657
		public PlayerData PlayerData { get; private set; }

		// Token: 0x17000211 RID: 529
		// (get) Token: 0x0600057E RID: 1406 RVA: 0x00008460 File Offset: 0x00006660
		// (set) Token: 0x0600057F RID: 1407 RVA: 0x00008468 File Offset: 0x00006668
		public SupportedFeatures SupportedFeatures { get; private set; }

		// Token: 0x17000212 RID: 530
		// (get) Token: 0x06000580 RID: 1408 RVA: 0x00008471 File Offset: 0x00006671
		// (set) Token: 0x06000581 RID: 1409 RVA: 0x00008479 File Offset: 0x00006679
		public ClanInfo ClanInfo { get; private set; }

		// Token: 0x17000213 RID: 531
		// (get) Token: 0x06000582 RID: 1410 RVA: 0x00008482 File Offset: 0x00006682
		// (set) Token: 0x06000583 RID: 1411 RVA: 0x0000848A File Offset: 0x0000668A
		public ClanHomeInfo ClanHomeInfo { get; private set; }

		// Token: 0x17000214 RID: 532
		// (get) Token: 0x06000584 RID: 1412 RVA: 0x00008493 File Offset: 0x00006693
		public IReadOnlyList<string> OwnedCosmetics
		{
			get
			{
				return this._ownedCosmetics;
			}
		}

		// Token: 0x17000215 RID: 533
		// (get) Token: 0x06000585 RID: 1413 RVA: 0x0000849B File Offset: 0x0000669B
		public IReadOnlyDictionary<string, List<string>> UsedCosmetics
		{
			get
			{
				return this._usedCosmetics;
			}
		}

		// Token: 0x17000216 RID: 534
		// (get) Token: 0x06000586 RID: 1414 RVA: 0x000084A3 File Offset: 0x000066A3
		// (set) Token: 0x06000587 RID: 1415 RVA: 0x000084AB File Offset: 0x000066AB
		public AvailableScenes AvailableScenes { get; private set; }

		// Token: 0x17000217 RID: 535
		// (get) Token: 0x06000588 RID: 1416 RVA: 0x000084B4 File Offset: 0x000066B4
		public PlayerId PlayerID
		{
			get
			{
				return this._playerId;
			}
		}

		// Token: 0x17000218 RID: 536
		// (get) Token: 0x06000589 RID: 1417 RVA: 0x000084BC File Offset: 0x000066BC
		// (set) Token: 0x0600058A RID: 1418 RVA: 0x000084C4 File Offset: 0x000066C4
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

		// Token: 0x17000219 RID: 537
		// (get) Token: 0x0600058B RID: 1419 RVA: 0x000084FC File Offset: 0x000066FC
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

		// Token: 0x1700021A RID: 538
		// (get) Token: 0x0600058C RID: 1420 RVA: 0x00008577 File Offset: 0x00006777
		public bool AtLobby
		{
			get
			{
				return this.CurrentState == LobbyClient.State.AtLobby;
			}
		}

		// Token: 0x1700021B RID: 539
		// (get) Token: 0x0600058D RID: 1421 RVA: 0x00008582 File Offset: 0x00006782
		public bool CanPerformLobbyActions
		{
			get
			{
				return this.CurrentState == LobbyClient.State.AtLobby || this.CurrentState == LobbyClient.State.RequestingToSearchBattle || this.CurrentState == LobbyClient.State.SearchingBattle || this.CurrentState == LobbyClient.State.WaitingToJoinCustomGame;
			}
		}

		// Token: 0x1700021C RID: 540
		// (get) Token: 0x0600058E RID: 1422 RVA: 0x000085AB File Offset: 0x000067AB
		public string Name
		{
			get
			{
				return this._userName;
			}
		}

		// Token: 0x1700021D RID: 541
		// (get) Token: 0x0600058F RID: 1423 RVA: 0x000085B3 File Offset: 0x000067B3
		// (set) Token: 0x06000590 RID: 1424 RVA: 0x000085BB File Offset: 0x000067BB
		public string LastBattleServerAddressForClient { get; private set; }

		// Token: 0x1700021E RID: 542
		// (get) Token: 0x06000591 RID: 1425 RVA: 0x000085C4 File Offset: 0x000067C4
		// (set) Token: 0x06000592 RID: 1426 RVA: 0x000085CC File Offset: 0x000067CC
		public ushort LastBattleServerPortForClient { get; private set; }

		// Token: 0x1700021F RID: 543
		// (get) Token: 0x06000593 RID: 1427 RVA: 0x000085D5 File Offset: 0x000067D5
		public bool Connected
		{
			get
			{
				return this.CurrentState != LobbyClient.State.Working && this.CurrentState > LobbyClient.State.Idle;
			}
		}

		// Token: 0x17000220 RID: 544
		// (get) Token: 0x06000594 RID: 1428 RVA: 0x000085EB File Offset: 0x000067EB
		public bool IsIdle
		{
			get
			{
				return this.CurrentState == LobbyClient.State.Idle;
			}
		}

		// Token: 0x06000595 RID: 1429 RVA: 0x000085F6 File Offset: 0x000067F6
		public void Logout(TextObject logOutReason)
		{
			base.BeginDisconnect();
			this.ChatManager.Cleanup();
			this._logOutReason = logOutReason;
		}

		// Token: 0x17000221 RID: 545
		// (get) Token: 0x06000596 RID: 1430 RVA: 0x00008610 File Offset: 0x00006810
		public bool LoggedIn
		{
			get
			{
				return this.CurrentState != LobbyClient.State.Idle && this.CurrentState != LobbyClient.State.Working && this.CurrentState != LobbyClient.State.Connected && this.CurrentState != LobbyClient.State.SessionRequested;
			}
		}

		// Token: 0x17000222 RID: 546
		// (get) Token: 0x06000597 RID: 1431 RVA: 0x0000863A File Offset: 0x0000683A
		public bool IsInGame
		{
			get
			{
				return this.CurrentState == LobbyClient.State.AtBattle || this.CurrentState == LobbyClient.State.HostingCustomGame || this.CurrentState == LobbyClient.State.InCustomGame;
			}
		}

		// Token: 0x17000223 RID: 547
		// (get) Token: 0x06000598 RID: 1432 RVA: 0x0000865C File Offset: 0x0000685C
		public bool IsHostingCustomGame
		{
			get
			{
				return this._state == LobbyClient.State.HostingCustomGame;
			}
		}

		// Token: 0x17000224 RID: 548
		// (get) Token: 0x06000599 RID: 1433 RVA: 0x00008668 File Offset: 0x00006868
		public bool IsMatchmakingAvailable
		{
			get
			{
				ServerStatus serverStatus = this._serverStatus;
				return serverStatus != null && serverStatus.IsMatchmakingEnabled;
			}
		}

		// Token: 0x17000225 RID: 549
		// (get) Token: 0x0600059A RID: 1434 RVA: 0x0000867B File Offset: 0x0000687B
		public bool IsAbleToSearchForGame
		{
			get
			{
				return this.IsMatchmakingAvailable && this._matchmakerBlockedTime <= DateTime.Now;
			}
		}

		// Token: 0x17000226 RID: 550
		// (get) Token: 0x0600059B RID: 1435 RVA: 0x00008697 File Offset: 0x00006897
		public bool PartySystemAvailable
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000227 RID: 551
		// (get) Token: 0x0600059C RID: 1436 RVA: 0x0000869A File Offset: 0x0000689A
		public bool IsCustomBattleAvailable
		{
			get
			{
				ServerStatus serverStatus = this._serverStatus;
				return serverStatus != null && serverStatus.IsCustomBattleEnabled;
			}
		}

		// Token: 0x17000228 RID: 552
		// (get) Token: 0x0600059D RID: 1437 RVA: 0x000086AD File Offset: 0x000068AD
		public IReadOnlyList<ModuleInfoModel> LoadedUnofficialModules
		{
			get
			{
				return this._loadedUnofficialModules;
			}
		}

		// Token: 0x17000229 RID: 553
		// (get) Token: 0x0600059E RID: 1438 RVA: 0x000086B5 File Offset: 0x000068B5
		public bool HasUnofficialModulesLoaded
		{
			get
			{
				return this.LoadedUnofficialModules.Count > 0;
			}
		}

		// Token: 0x1700022A RID: 554
		// (get) Token: 0x0600059F RID: 1439 RVA: 0x000086C5 File Offset: 0x000068C5
		// (set) Token: 0x060005A0 RID: 1440 RVA: 0x000086CD File Offset: 0x000068CD
		public bool HasUserGeneratedContentPrivilege { get; private set; }

		// Token: 0x1700022B RID: 555
		// (get) Token: 0x060005A1 RID: 1441 RVA: 0x000086D8 File Offset: 0x000068D8
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

		// Token: 0x1700022C RID: 556
		// (get) Token: 0x060005A2 RID: 1442 RVA: 0x0000872F File Offset: 0x0000692F
		public bool IsClanLeader
		{
			get
			{
				ClanPlayer clanPlayer = this.PlayersInClan.Find((ClanPlayer p) => p.PlayerId == this._playerId);
				return clanPlayer != null && clanPlayer.Role == ClanPlayerRole.Leader;
			}
		}

		// Token: 0x1700022D RID: 557
		// (get) Token: 0x060005A3 RID: 1443 RVA: 0x00008756 File Offset: 0x00006956
		public bool IsClanOfficer
		{
			get
			{
				ClanPlayer clanPlayer = this.PlayersInClan.Find((ClanPlayer p) => p.PlayerId == this._playerId);
				return clanPlayer != null && clanPlayer.Role == ClanPlayerRole.Officer;
			}
		}

		// Token: 0x1700022E RID: 558
		// (get) Token: 0x060005A4 RID: 1444 RVA: 0x0000877D File Offset: 0x0000697D
		// (set) Token: 0x060005A5 RID: 1445 RVA: 0x00008785 File Offset: 0x00006985
		public bool IsEligibleToCreatePremadeGame { get; private set; }

		// Token: 0x1700022F RID: 559
		// (get) Token: 0x060005A6 RID: 1446 RVA: 0x0000878E File Offset: 0x0000698E
		// (set) Token: 0x060005A7 RID: 1447 RVA: 0x00008796 File Offset: 0x00006996
		public CustomBattleId CustomBattleId { get; private set; }

		// Token: 0x17000230 RID: 560
		// (get) Token: 0x060005A8 RID: 1448 RVA: 0x0000879F File Offset: 0x0000699F
		// (set) Token: 0x060005A9 RID: 1449 RVA: 0x000087A7 File Offset: 0x000069A7
		public string CustomGameType { get; private set; }

		// Token: 0x17000231 RID: 561
		// (get) Token: 0x060005AA RID: 1450 RVA: 0x000087B0 File Offset: 0x000069B0
		// (set) Token: 0x060005AB RID: 1451 RVA: 0x000087B8 File Offset: 0x000069B8
		public string CustomGameScene { get; private set; }

		// Token: 0x17000232 RID: 562
		// (get) Token: 0x060005AC RID: 1452 RVA: 0x000087C1 File Offset: 0x000069C1
		// (set) Token: 0x060005AD RID: 1453 RVA: 0x000087C9 File Offset: 0x000069C9
		public AvailableCustomGames AvailableCustomGames { get; private set; }

		// Token: 0x17000233 RID: 563
		// (get) Token: 0x060005AE RID: 1454 RVA: 0x000087D2 File Offset: 0x000069D2
		// (set) Token: 0x060005AF RID: 1455 RVA: 0x000087DA File Offset: 0x000069DA
		public PremadeGameList AvailablePremadeGames { get; private set; }

		// Token: 0x17000234 RID: 564
		// (get) Token: 0x060005B0 RID: 1456 RVA: 0x000087E3 File Offset: 0x000069E3
		// (set) Token: 0x060005B1 RID: 1457 RVA: 0x000087EB File Offset: 0x000069EB
		public List<PartyPlayerInLobbyClient> PlayersInParty { get; private set; }

		// Token: 0x17000235 RID: 565
		// (get) Token: 0x060005B2 RID: 1458 RVA: 0x000087F4 File Offset: 0x000069F4
		// (set) Token: 0x060005B3 RID: 1459 RVA: 0x000087FC File Offset: 0x000069FC
		public List<ClanPlayer> PlayersInClan { get; private set; }

		// Token: 0x17000236 RID: 566
		// (get) Token: 0x060005B4 RID: 1460 RVA: 0x00008805 File Offset: 0x00006A05
		// (set) Token: 0x060005B5 RID: 1461 RVA: 0x0000880D File Offset: 0x00006A0D
		public List<ClanPlayerInfo> PlayerInfosInClan { get; private set; }

		// Token: 0x17000237 RID: 567
		// (get) Token: 0x060005B6 RID: 1462 RVA: 0x00008816 File Offset: 0x00006A16
		// (set) Token: 0x060005B7 RID: 1463 RVA: 0x0000881E File Offset: 0x00006A1E
		public FriendInfo[] FriendInfos { get; private set; }

		// Token: 0x17000238 RID: 568
		// (get) Token: 0x060005B8 RID: 1464 RVA: 0x00008827 File Offset: 0x00006A27
		public bool IsInParty
		{
			get
			{
				return this.Connected && this.PlayersInParty.Count > 0;
			}
		}

		// Token: 0x17000239 RID: 569
		// (get) Token: 0x060005B9 RID: 1465 RVA: 0x00008841 File Offset: 0x00006A41
		public bool IsPartyFull
		{
			get
			{
				return this.PlayersInParty.Count == Parameters.MaxPlayerCountInParty;
			}
		}

		// Token: 0x1700023A RID: 570
		// (get) Token: 0x060005BA RID: 1466 RVA: 0x00008855 File Offset: 0x00006A55
		// (set) Token: 0x060005BB RID: 1467 RVA: 0x0000885D File Offset: 0x00006A5D
		public string CurrentMatchId { get; private set; }

		// Token: 0x1700023B RID: 571
		// (get) Token: 0x060005BC RID: 1468 RVA: 0x00008866 File Offset: 0x00006A66
		public bool IsInClan
		{
			get
			{
				return this.PlayersInClan.Count > 0;
			}
		}

		// Token: 0x1700023C RID: 572
		// (get) Token: 0x060005BD RID: 1469 RVA: 0x00008876 File Offset: 0x00006A76
		// (set) Token: 0x060005BE RID: 1470 RVA: 0x0000887E File Offset: 0x00006A7E
		public bool IsPartyInvitationPopupActive { get; private set; }

		// Token: 0x1700023D RID: 573
		// (get) Token: 0x060005BF RID: 1471 RVA: 0x00008887 File Offset: 0x00006A87
		public bool CanInvitePlayers
		{
			get
			{
				SupportedFeatures supportedFeatures = this.SupportedFeatures;
				return supportedFeatures != null && supportedFeatures.SupportsFeatures(Features.Party) && (!this.IsInParty || this.IsPartyLeader);
			}
		}

		// Token: 0x1700023E RID: 574
		// (get) Token: 0x060005C0 RID: 1472 RVA: 0x000088B0 File Offset: 0x00006AB0
		public bool CanSuggestPlayers
		{
			get
			{
				SupportedFeatures supportedFeatures = this.SupportedFeatures;
				return supportedFeatures != null && supportedFeatures.SupportsFeatures(Features.Party) && this.IsInParty && !this.IsPartyLeader;
			}
		}

		// Token: 0x1700023F RID: 575
		// (get) Token: 0x060005C1 RID: 1473 RVA: 0x000088DA File Offset: 0x00006ADA
		// (set) Token: 0x060005C2 RID: 1474 RVA: 0x000088E2 File Offset: 0x00006AE2
		public bool IsRefreshingPlayerData { get; set; }

		// Token: 0x17000240 RID: 576
		// (get) Token: 0x060005C3 RID: 1475 RVA: 0x000088EB File Offset: 0x00006AEB
		// (set) Token: 0x060005C4 RID: 1476 RVA: 0x000088F3 File Offset: 0x00006AF3
		public Guid ClanID { get; private set; }

		// Token: 0x060005C5 RID: 1477 RVA: 0x000088FC File Offset: 0x00006AFC
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

		// Token: 0x17000241 RID: 577
		// (get) Token: 0x060005C6 RID: 1478 RVA: 0x00008E2A File Offset: 0x0000702A
		// (set) Token: 0x060005C7 RID: 1479 RVA: 0x00008E32 File Offset: 0x00007032
		public ChatManager ChatManager { get; private set; }

		// Token: 0x060005C8 RID: 1480 RVA: 0x00008E3C File Offset: 0x0000703C
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

		// Token: 0x060005C9 RID: 1481 RVA: 0x00008EB0 File Offset: 0x000070B0
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

		// Token: 0x060005CA RID: 1482 RVA: 0x00008EF5 File Offset: 0x000070F5
		public void QuitFromCustomGame()
		{
			base.SendMessage(new QuitFromCustomGameMessage());
			this.CurrentState = LobbyClient.State.AtLobby;
			if (this._handler != null)
			{
				this._handler.OnQuitFromCustomGame();
			}
		}

		// Token: 0x060005CB RID: 1483 RVA: 0x00008F1C File Offset: 0x0000711C
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

		// Token: 0x060005CC RID: 1484 RVA: 0x00008F50 File Offset: 0x00007150
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

		// Token: 0x060005CD RID: 1485 RVA: 0x00008FA5 File Offset: 0x000071A5
		public void CancelFindGame()
		{
			this.CurrentState = LobbyClient.State.RequestingToCancelSearchBattle;
			this.CheckAndSendMessage(new CancelBattleRequestMessage());
		}

		// Token: 0x060005CE RID: 1486 RVA: 0x00008FB9 File Offset: 0x000071B9
		public void FindGame()
		{
			this.CurrentState = LobbyClient.State.RequestingToSearchBattle;
			this.CheckAndSendMessage(new FindGameMessage());
		}

		// Token: 0x060005CF RID: 1487 RVA: 0x00008FD0 File Offset: 0x000071D0
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

		// Token: 0x060005D0 RID: 1488 RVA: 0x00009030 File Offset: 0x00007230
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

		// Token: 0x060005D1 RID: 1489 RVA: 0x0000909F File Offset: 0x0000729F
		public void KickPlayer(PlayerId id, bool banPlayer)
		{
			throw new NotImplementedException();
		}

		// Token: 0x060005D2 RID: 1490 RVA: 0x000090A6 File Offset: 0x000072A6
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

		// Token: 0x060005D3 RID: 1491 RVA: 0x000090E4 File Offset: 0x000072E4
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

		// Token: 0x060005D4 RID: 1492 RVA: 0x00009164 File Offset: 0x00007364
		private void CheckAndSendMessage(Message message)
		{
			base.SendMessage(message);
		}

		// Token: 0x060005D5 RID: 1493 RVA: 0x0000916D File Offset: 0x0000736D
		public override void OnConnected()
		{
			base.OnConnected();
			this.CurrentState = LobbyClient.State.Connected;
			if (this._handler != null)
			{
				this._handler.OnConnected();
			}
		}

		// Token: 0x060005D6 RID: 1494 RVA: 0x0000918F File Offset: 0x0000738F
		public override void OnCantConnect()
		{
			base.OnCantConnect();
			this.CurrentState = LobbyClient.State.Idle;
			if (this._handler != null)
			{
				this._handler.OnCantConnect();
			}
		}

		// Token: 0x060005D7 RID: 1495 RVA: 0x000091B4 File Offset: 0x000073B4
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

		// Token: 0x060005D8 RID: 1496 RVA: 0x0000924A File Offset: 0x0000744A
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

		// Token: 0x060005D9 RID: 1497 RVA: 0x0000927C File Offset: 0x0000747C
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

		// Token: 0x060005DA RID: 1498 RVA: 0x000092F0 File Offset: 0x000074F0
		private void OnBattleOverMessage(BattleOverMessage message)
		{
			if (this.CurrentState == LobbyClient.State.AtBattle)
			{
				this.CurrentState = LobbyClient.State.AtLobby;
				this._handler.OnMatchmakerGameOver(message.OldExperience, message.NewExperience, message.EarnedBadges, message.GoldGained, message.OldInfo, message.NewInfo);
			}
		}

		// Token: 0x060005DB RID: 1499 RVA: 0x0000933D File Offset: 0x0000753D
		private void OnJoinChatRoomMessage(JoinChatRoomMessage message)
		{
			this.ChatManager.OnJoinChatRoom(message.ChatRoomInformaton, this.PlayerData.PlayerId, this.PlayerData.LastPlayerName);
		}

		// Token: 0x060005DC RID: 1500 RVA: 0x00009366 File Offset: 0x00007566
		private void OnChatRoomClosedMessage(ChatRoomClosedMessage message)
		{
			this.ChatManager.OnChatRoomClosed(message.ChatRoomId);
		}

		// Token: 0x060005DD RID: 1501 RVA: 0x00009379 File Offset: 0x00007579
		private void OnBattleResultMessage(BattleResultMessage message)
		{
			this._handler.OnBattleResultReceived();
		}

		// Token: 0x060005DE RID: 1502 RVA: 0x00009386 File Offset: 0x00007586
		private void OnBattleServerLostMessage(BattleServerLostMessage message)
		{
			if (this.CurrentState == LobbyClient.State.AtBattle || this.CurrentState == LobbyClient.State.SearchingToRejoinBattle)
			{
				this.CurrentState = LobbyClient.State.AtLobby;
			}
			this._handler.OnBattleServerLost();
		}

		// Token: 0x060005DF RID: 1503 RVA: 0x000093AD File Offset: 0x000075AD
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

		// Token: 0x060005E0 RID: 1504 RVA: 0x000093DA File Offset: 0x000075DA
		private void OnRejoinRequestRejectedMessage(RejoinRequestRejectedMessage message)
		{
			this.CurrentState = LobbyClient.State.AtLobby;
			this._handler.OnRejoinRequestRejected();
		}

		// Token: 0x060005E1 RID: 1505 RVA: 0x000093EE File Offset: 0x000075EE
		private void OnCancelFindGameMessage(CancelFindGameMessage message)
		{
			if (this.CurrentState == LobbyClient.State.SearchingBattle)
			{
				this.CancelFindGame();
			}
		}

		// Token: 0x060005E2 RID: 1506 RVA: 0x000093FF File Offset: 0x000075FF
		private void OnWhisperMessageReceivedMessage(WhisperReceivedMessage message)
		{
			this._handler.OnWhisperMessageReceived(message.FromPlayer, message.ToPlayer, message.Message);
		}

		// Token: 0x060005E3 RID: 1507 RVA: 0x0000941E File Offset: 0x0000761E
		private void OnClanMessageReceivedMessage(ClanMessageReceivedMessage message)
		{
			this._handler.OnClanMessageReceived(message.PlayerName, message.Message);
		}

		// Token: 0x060005E4 RID: 1508 RVA: 0x00009437 File Offset: 0x00007637
		private void OnChannelMessageReceivedMessage(ChannelMessageReceivedMessage message)
		{
			this._handler.OnChannelMessageReceived(message.Channel, message.PlayerName, message.Message);
		}

		// Token: 0x060005E5 RID: 1509 RVA: 0x00009456 File Offset: 0x00007656
		private void OnPartyMessageReceivedMessage(PartyMessageReceivedMessage message)
		{
			this._handler.OnPartyMessageReceived(message.PlayerName, message.Message);
		}

		// Token: 0x060005E6 RID: 1510 RVA: 0x0000946F File Offset: 0x0000766F
		private void OnPlayerQuitFromMatchmakerGameResult(PlayerQuitFromMatchmakerGameResult message)
		{
			if (this.CurrentState == LobbyClient.State.QuittingFromBattle)
			{
				this.CurrentState = LobbyClient.State.AtLobby;
			}
		}

		// Token: 0x060005E7 RID: 1511 RVA: 0x00009484 File Offset: 0x00007684
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

		// Token: 0x060005E8 RID: 1512 RVA: 0x000094E0 File Offset: 0x000076E0
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

		// Token: 0x060005E9 RID: 1513 RVA: 0x000095D4 File Offset: 0x000077D4
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

		// Token: 0x060005EA RID: 1514 RVA: 0x000096C5 File Offset: 0x000078C5
		private void OnClientQuitFromCustomGameMessage(ClientQuitFromCustomGameMessage message)
		{
			if (this._handler != null)
			{
				this._handler.OnClientQuitFromCustomGame(message.PlayerId);
			}
		}

		// Token: 0x060005EB RID: 1515 RVA: 0x000096E0 File Offset: 0x000078E0
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

		// Token: 0x060005EC RID: 1516 RVA: 0x0000970E File Offset: 0x0000790E
		private void OnPlayerRemovedFromMatchmakerGameMessage(PlayerRemovedFromMatchmakerGame message)
		{
			this.CurrentState = LobbyClient.State.AtLobby;
			if (this._handler != null)
			{
				this._handler.OnRemovedFromMatchmakerGame(message.DisconnectType);
			}
		}

		// Token: 0x060005ED RID: 1517 RVA: 0x00009730 File Offset: 0x00007930
		private void OnPlayerRemovedFromCustomGame(PlayerRemovedFromCustomGame message)
		{
			this.CurrentState = LobbyClient.State.AtLobby;
			if (this._handler != null)
			{
				this._handler.OnRemovedFromCustomGame(message.DisconnectType);
			}
		}

		// Token: 0x060005EE RID: 1518 RVA: 0x00009752 File Offset: 0x00007952
		private void OnSystemMessage(SystemMessage message)
		{
			this._handler.OnSystemMessageReceived(message.GetDescription().ToString());
		}

		// Token: 0x060005EF RID: 1519 RVA: 0x0000976A File Offset: 0x0000796A
		private void OnAdminMessage(AdminMessage message)
		{
			this._handler.OnAdminMessageReceived(message.Message);
		}

		// Token: 0x060005F0 RID: 1520 RVA: 0x0000977D File Offset: 0x0000797D
		private void OnInvitationToPartyMessage(InvitationToPartyMessage message)
		{
			this.IsPartyInvitationPopupActive = true;
			this._handler.OnPartyInvitationReceived(message.InviterPlayerName, message.InviterPlayerId);
		}

		// Token: 0x060005F1 RID: 1521 RVA: 0x0000979D File Offset: 0x0000799D
		private void OnPartyInvitationInvalidMessage(PartyInvitationInvalidMessage message)
		{
			this.IsPartyInvitationPopupActive = false;
			this._handler.OnPartyInvitationInvalidated();
		}

		// Token: 0x060005F2 RID: 1522 RVA: 0x000097B1 File Offset: 0x000079B1
		private void OnPlayerInvitedToPartyMessage(PlayerInvitedToPartyMessage message)
		{
			this.PlayersInParty.Add(new PartyPlayerInLobbyClient(message.PlayerId, message.PlayerName, false));
			this._handler.OnPlayerInvitedToParty(message.PlayerId);
		}

		// Token: 0x060005F3 RID: 1523 RVA: 0x000097E4 File Offset: 0x000079E4
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

		// Token: 0x060005F4 RID: 1524 RVA: 0x0000993C File Offset: 0x00007B3C
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

		// Token: 0x060005F5 RID: 1525 RVA: 0x000099B4 File Offset: 0x00007BB4
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

		// Token: 0x060005F6 RID: 1526 RVA: 0x00009A48 File Offset: 0x00007C48
		private void OnPlayerSuggestedToPartyMessage(PlayerSuggestedToPartyMessage message)
		{
			ILobbyClientSessionHandler handler = this._handler;
			if (handler == null)
			{
				return;
			}
			handler.OnPlayerSuggestedToParty(message.PlayerId, message.PlayerName, message.SuggestingPlayerId, message.SuggestingPlayerName);
		}

		// Token: 0x060005F7 RID: 1527 RVA: 0x00009A72 File Offset: 0x00007C72
		private void OnUpdatePlayerDataMessage(UpdatePlayerDataMessage updatePlayerDataMessage)
		{
			this.PlayerData = updatePlayerDataMessage.PlayerData;
			if (this._handler != null)
			{
				this._handler.OnPlayerDataReceived(this.PlayerData);
			}
		}

		// Token: 0x060005F8 RID: 1528 RVA: 0x00009A9C File Offset: 0x00007C9C
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

		// Token: 0x060005F9 RID: 1529 RVA: 0x00009B06 File Offset: 0x00007D06
		private void OnFriendListMessage(FriendListMessage friendListMessage)
		{
			this._friendListTimer.Restart();
			this.FriendInfos = friendListMessage.Friends;
			if (this._handler != null)
			{
				this._handler.OnFriendListReceived(friendListMessage.Friends);
			}
		}

		// Token: 0x060005FA RID: 1530 RVA: 0x00009B38 File Offset: 0x00007D38
		private void OnMatchmakerDisabledMessage(MatchmakerDisabledMessage matchmakerDisabledMessage)
		{
			if (matchmakerDisabledMessage.RemainingTime > 0)
			{
				this._matchmakerBlockedTime = DateTime.Now.AddSeconds((double)matchmakerDisabledMessage.RemainingTime);
				return;
			}
			this._matchmakerBlockedTime = DateTime.MinValue;
		}

		// Token: 0x060005FB RID: 1531 RVA: 0x00009B74 File Offset: 0x00007D74
		private void OnClanCreationRequestMessage(ClanCreationRequestMessage clanCreationRequestMessage)
		{
			if (this._handler != null)
			{
				this._handler.OnClanInvitationReceived(clanCreationRequestMessage.ClanName, clanCreationRequestMessage.ClanTag, true);
			}
		}

		// Token: 0x060005FC RID: 1532 RVA: 0x00009B96 File Offset: 0x00007D96
		private void OnClanCreationRequestAnsweredMessage(ClanCreationRequestAnsweredMessage clanCreationRequestAnsweredMessage)
		{
			if (this._handler != null)
			{
				this._handler.OnClanInvitationAnswered(clanCreationRequestAnsweredMessage.PlayerId, clanCreationRequestAnsweredMessage.ClanCreationAnswer);
			}
		}

		// Token: 0x060005FD RID: 1533 RVA: 0x00009BB7 File Offset: 0x00007DB7
		private void OnClanCreationSuccessfulMessage(ClanCreationSuccessfulMessage clanCreationSuccessfulMessage)
		{
			if (this._handler != null)
			{
				this._handler.OnClanCreationSuccessful();
			}
		}

		// Token: 0x060005FE RID: 1534 RVA: 0x00009BCC File Offset: 0x00007DCC
		private void OnClanCreationFailedMessage(ClanCreationFailedMessage clanCreationFailedMessage)
		{
			if (this._handler != null)
			{
				this._handler.OnClanCreationFailed();
			}
		}

		// Token: 0x060005FF RID: 1535 RVA: 0x00009BE1 File Offset: 0x00007DE1
		private void OnCreateClanAnswerMessage(CreateClanAnswerMessage createClanAnswerMessage)
		{
			if (createClanAnswerMessage.Successful)
			{
				this._handler.OnClanCreationStarted();
			}
		}

		// Token: 0x06000600 RID: 1536 RVA: 0x00009BF6 File Offset: 0x00007DF6
		public void SendWhisper(string playerName, string message)
		{
		}

		// Token: 0x06000601 RID: 1537 RVA: 0x00009BF8 File Offset: 0x00007DF8
		private void OnRecentPlayerStatusesMessage(RecentPlayerStatusesMessage message)
		{
			if (this._handler != null)
			{
				this._handler.OnRecentPlayerStatusesReceived(message.Friends);
			}
		}

		// Token: 0x06000602 RID: 1538 RVA: 0x00009C13 File Offset: 0x00007E13
		public void FleeBattle()
		{
			this.CheckAndSendMessage(new RejoinBattleRequestMessage(false));
		}

		// Token: 0x06000603 RID: 1539 RVA: 0x00009C21 File Offset: 0x00007E21
		public void SendPartyMessage(string message)
		{
		}

		// Token: 0x06000604 RID: 1540 RVA: 0x00009C23 File Offset: 0x00007E23
		public void SendChannelMessage(Guid roomId, string message)
		{
			this.ChatManager.SendMessage(roomId, message);
		}

		// Token: 0x06000605 RID: 1541 RVA: 0x00009C32 File Offset: 0x00007E32
		private void OnClanInfoChangedMessage(ClanInfoChangedMessage clanInfoChangedMessage)
		{
			this.UpdateClanInfo(clanInfoChangedMessage.ClanHomeInfo);
		}

		// Token: 0x06000606 RID: 1542 RVA: 0x00009C40 File Offset: 0x00007E40
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

		// Token: 0x06000607 RID: 1543 RVA: 0x00009CE3 File Offset: 0x00007EE3
		private void OnInvitationToClanMessage(InvitationToClanMessage invitationToClanMessage)
		{
			if (this._handler != null)
			{
				this._handler.OnClanInvitationReceived(invitationToClanMessage.ClanName, invitationToClanMessage.ClanTag, false);
			}
		}

		// Token: 0x06000608 RID: 1544 RVA: 0x00009D05 File Offset: 0x00007F05
		public void RejoinBattle()
		{
			this.CheckAndSendMessage(new RejoinBattleRequestMessage(true));
		}

		// Token: 0x06000609 RID: 1545 RVA: 0x00009D13 File Offset: 0x00007F13
		private void OnJoinPremadeGameAnswerMessage(JoinPremadeGameAnswerMessage joinPremadeGameAnswerMessage)
		{
		}

		// Token: 0x0600060A RID: 1546 RVA: 0x00009D15 File Offset: 0x00007F15
		public void OnBattleResultsSeen()
		{
			this.AssertCanPerformLobbyActions();
			this.CheckAndSendMessage(new BattleResultSeenMessage());
		}

		// Token: 0x0600060B RID: 1547 RVA: 0x00009D28 File Offset: 0x00007F28
		private void OnCreatePremadeGameAnswerMessage(CreatePremadeGameAnswerMessage createPremadeGameAnswerMessage)
		{
			if (createPremadeGameAnswerMessage.Successful)
			{
				this._handler.OnPremadeGameCreated();
			}
		}

		// Token: 0x0600060C RID: 1548 RVA: 0x00009D3D File Offset: 0x00007F3D
		private void OnJoinPremadeGameRequestMessage(JoinPremadeGameRequestMessage joinPremadeGameRequestMessage)
		{
			this._handler.OnJoinPremadeGameRequested(joinPremadeGameRequestMessage.ClanName, joinPremadeGameRequestMessage.Sigil, joinPremadeGameRequestMessage.ChallengerPartyId, joinPremadeGameRequestMessage.ChallengerPlayers, joinPremadeGameRequestMessage.ChallengerPartyLeaderId, joinPremadeGameRequestMessage.PremadeGameType);
		}

		// Token: 0x0600060D RID: 1549 RVA: 0x00009D6E File Offset: 0x00007F6E
		private void OnJoinPremadeGameRequestResultMessage(JoinPremadeGameRequestResultMessage joinPremadeGameRequestResultMessage)
		{
			if (joinPremadeGameRequestResultMessage.Successful)
			{
				this._handler.OnJoinPremadeGameRequestSuccessful();
				this.CurrentState = LobbyClient.State.WaitingToJoinPremadeGame;
			}
		}

		// Token: 0x0600060E RID: 1550 RVA: 0x00009D8C File Offset: 0x00007F8C
		private async void OnClanDisbandedMessage(ClanDisbandedMessage clanDisbandedMessage)
		{
			ClanHomeInfo clanHomeInfo = await this.GetClanHomeInfo();
			this.UpdateClanInfo(clanHomeInfo);
		}

		// Token: 0x0600060F RID: 1551 RVA: 0x00009DC5 File Offset: 0x00007FC5
		private void OnClanGameCreationCancelledMessage(ClanGameCreationCancelledMessage clanGameCreationCancelledMessage)
		{
			this.CurrentState = LobbyClient.State.AtLobby;
			this._handler.OnPremadeGameCreationCancelled();
		}

		// Token: 0x06000610 RID: 1552 RVA: 0x00009DD9 File Offset: 0x00007FD9
		private void OnPremadeGameEligibilityStatusMessage(PremadeGameEligibilityStatusMessage premadeGameEligibilityStatusMessage)
		{
			this._handler.OnPremadeGameEligibilityStatusReceived(premadeGameEligibilityStatusMessage.EligibleGameTypes.Length != 0);
			this.IsEligibleToCreatePremadeGame = premadeGameEligibilityStatusMessage.EligibleGameTypes.Length != 0;
		}

		// Token: 0x06000611 RID: 1553 RVA: 0x00009E00 File Offset: 0x00008000
		private async void OnKickedFromClan(KickedFromClanMessage kickedFromClanMessage)
		{
			ClanHomeInfo clanHomeInfo = await this.GetClanHomeInfo();
			this.UpdateClanInfo(clanHomeInfo);
		}

		// Token: 0x06000612 RID: 1554 RVA: 0x00009E39 File Offset: 0x00008039
		private void OnCustomBattleOverMessage(CustomBattleOverMessage message)
		{
			this.CurrentState = LobbyClient.State.AtLobby;
			this._handler.OnMatchmakerGameOver(message.OldExperience, message.NewExperience, new List<string>(), message.GoldGain, null, null);
		}

		// Token: 0x06000613 RID: 1555 RVA: 0x00009E66 File Offset: 0x00008066
		public void AcceptClanInvitation()
		{
			this.CheckAndSendMessage(new AcceptClanInvitationMessage());
		}

		// Token: 0x06000614 RID: 1556 RVA: 0x00009E73 File Offset: 0x00008073
		public void DeclineClanInvitation()
		{
			this.CheckAndSendMessage(new DeclineClanInvitationMessage());
		}

		// Token: 0x06000615 RID: 1557 RVA: 0x00009E80 File Offset: 0x00008080
		private void OnShowAnnouncementMessage(ShowAnnouncementMessage message)
		{
			ILobbyClientSessionHandler handler = this._handler;
			if (handler == null)
			{
				return;
			}
			handler.OnAnnouncementReceived(message.Announcement);
		}

		// Token: 0x06000616 RID: 1558 RVA: 0x00009E98 File Offset: 0x00008098
		public void MarkNotificationAsRead(int notificationID)
		{
			UpdateNotificationsMessage updateNotificationsMessage = new UpdateNotificationsMessage(new int[] { notificationID });
			this.CheckAndSendMessage(updateNotificationsMessage);
		}

		// Token: 0x06000617 RID: 1559 RVA: 0x00009EBC File Offset: 0x000080BC
		private void OnRejoinBattleRequestAnswerMessage(RejoinBattleRequestAnswerMessage rejoinBattleRequestAnswerMessage)
		{
			this._handler.OnRejoinBattleRequestAnswered(rejoinBattleRequestAnswerMessage.IsSuccessful);
			if (rejoinBattleRequestAnswerMessage.IsSuccessful && rejoinBattleRequestAnswerMessage.IsRejoinAccepted)
			{
				this.CurrentState = LobbyClient.State.SearchingBattle;
			}
		}

		// Token: 0x06000618 RID: 1560 RVA: 0x00009EE6 File Offset: 0x000080E6
		public void AcceptClanCreationRequest()
		{
			this.CheckAndSendMessage(new AcceptClanCreationRequestMessage());
		}

		// Token: 0x06000619 RID: 1561 RVA: 0x00009EF3 File Offset: 0x000080F3
		private void OnPendingBattleRejoinMessage(PendingBattleRejoinMessage pendingBattleRejoinMessage)
		{
			this._handler.OnPendingRejoin();
		}

		// Token: 0x0600061A RID: 1562 RVA: 0x00009F00 File Offset: 0x00008100
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

		// Token: 0x0600061B RID: 1563 RVA: 0x00009F1A File Offset: 0x0000811A
		public void DeclineClanCreationRequest()
		{
			this.CheckAndSendMessage(new DeclineClanCreationRequestMessage());
		}

		// Token: 0x0600061C RID: 1564 RVA: 0x00009F27 File Offset: 0x00008127
		public void PromoteToClanLeader(PlayerId playerId, bool dontUseNameForUnknownPlayer)
		{
			this.CheckAndSendMessage(new PromoteToClanLeaderMessage(playerId, dontUseNameForUnknownPlayer));
		}

		// Token: 0x0600061D RID: 1565 RVA: 0x00009F36 File Offset: 0x00008136
		private void OnLobbyNotificationsMessage(LobbyNotificationsMessage message)
		{
			this._handler.OnNotificationsReceived(message.Notifications);
		}

		// Token: 0x0600061E RID: 1566 RVA: 0x00009F49 File Offset: 0x00008149
		public void KickFromClan(PlayerId playerId)
		{
			this.CheckAndSendMessage(new KickFromClanMessage(playerId));
		}

		// Token: 0x0600061F RID: 1567 RVA: 0x00009F58 File Offset: 0x00008158
		public async Task<CheckClanParameterValidResult> ClanNameExists(string clanName)
		{
			return await base.CallFunction<CheckClanParameterValidResult>(new CheckClanNameValidMessage(clanName));
		}

		// Token: 0x06000620 RID: 1568 RVA: 0x00009FA8 File Offset: 0x000081A8
		public async Task<CheckClanParameterValidResult> ClanTagExists(string clanTag)
		{
			return await base.CallFunction<CheckClanParameterValidResult>(new CheckClanTagValidMessage(clanTag));
		}

		// Token: 0x06000621 RID: 1569 RVA: 0x00009FF8 File Offset: 0x000081F8
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

		// Token: 0x06000622 RID: 1570 RVA: 0x0000A03D File Offset: 0x0000823D
		public void SendChatMessage(Guid roomId, string message)
		{
			this.ChatManager.SendMessage(roomId, message);
		}

		// Token: 0x06000623 RID: 1571 RVA: 0x0000A04C File Offset: 0x0000824C
		public void JoinChannel(ChatChannelType channel)
		{
		}

		// Token: 0x06000624 RID: 1572 RVA: 0x0000A04E File Offset: 0x0000824E
		public void AssignAsClanOfficer(PlayerId playerId, bool dontUseNameForUnknownPlayer)
		{
			this.CheckAndSendMessage(new AssignAsClanOfficerMessage(playerId, dontUseNameForUnknownPlayer));
		}

		// Token: 0x06000625 RID: 1573 RVA: 0x0000A05D File Offset: 0x0000825D
		public void RemoveClanOfficerRoleForPlayer(PlayerId playerId)
		{
			this.CheckAndSendMessage(new RemoveClanOfficerRoleForPlayerMessage(playerId));
		}

		// Token: 0x06000626 RID: 1574 RVA: 0x0000A06B File Offset: 0x0000826B
		public void LeaveChannel(ChatChannelType channel)
		{
		}

		// Token: 0x06000627 RID: 1575 RVA: 0x0000A070 File Offset: 0x00008270
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

		// Token: 0x06000628 RID: 1576 RVA: 0x0000A138 File Offset: 0x00008338
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

		// Token: 0x06000629 RID: 1577 RVA: 0x0000A180 File Offset: 0x00008380
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

		// Token: 0x0600062A RID: 1578 RVA: 0x0000A1CD File Offset: 0x000083CD
		public void SendClanMessage(string message)
		{
		}

		// Token: 0x0600062B RID: 1579 RVA: 0x0000A1D0 File Offset: 0x000083D0
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

		// Token: 0x0600062C RID: 1580 RVA: 0x0000A218 File Offset: 0x00008418
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

		// Token: 0x0600062D RID: 1581 RVA: 0x0000A25D File Offset: 0x0000845D
		public void SetClanInformationText(string informationText)
		{
			this.CheckAndSendMessage(new SetClanInformationMessage(informationText));
		}

		// Token: 0x0600062E RID: 1582 RVA: 0x0000A26B File Offset: 0x0000846B
		public void AddClanAnnouncement(string announcement)
		{
			this.CheckAndSendMessage(new AddClanAnnouncementMessage(announcement));
		}

		// Token: 0x0600062F RID: 1583 RVA: 0x0000A279 File Offset: 0x00008479
		public void EditClanAnnouncement(int announcementId, string text)
		{
			this.CheckAndSendMessage(new EditClanAnnouncementMessage(announcementId, text));
		}

		// Token: 0x06000630 RID: 1584 RVA: 0x0000A288 File Offset: 0x00008488
		public void RemoveClanAnnouncement(int announcementId)
		{
			this.CheckAndSendMessage(new RemoveClanAnnouncementMessage(announcementId));
		}

		// Token: 0x06000631 RID: 1585 RVA: 0x0000A296 File Offset: 0x00008496
		public void ChangeClanFaction(string faction)
		{
			this.CheckAndSendMessage(new ChangeClanFactionMessage(faction));
		}

		// Token: 0x06000632 RID: 1586 RVA: 0x0000A2A4 File Offset: 0x000084A4
		public void ChangeClanSigil(string sigil)
		{
			this.CheckAndSendMessage(new ChangeClanSigilMessage(sigil));
		}

		// Token: 0x06000633 RID: 1587 RVA: 0x0000A2B2 File Offset: 0x000084B2
		public void DestroyClan()
		{
			this.CheckAndSendMessage(new DestroyClanMessage());
		}

		// Token: 0x06000634 RID: 1588 RVA: 0x0000A2BF File Offset: 0x000084BF
		public void InviteToClan(PlayerId invitedPlayerId, bool dontUseNameForUnknownPlayer)
		{
			this.CheckAndSendMessage(new InviteToClanMessage(invitedPlayerId, dontUseNameForUnknownPlayer));
		}

		// Token: 0x06000635 RID: 1589 RVA: 0x0000A2D0 File Offset: 0x000084D0
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

		// Token: 0x06000636 RID: 1590 RVA: 0x0000A345 File Offset: 0x00008545
		public void CancelCreatingPremadeGame()
		{
			this.CheckAndSendMessage(new CancelCreatingPremadeGameMessage());
		}

		// Token: 0x06000637 RID: 1591 RVA: 0x0000A354 File Offset: 0x00008554
		public void RequestToJoinPremadeGame(Guid gameId, string password)
		{
			string text = Common.CalculateMD5Hash(password);
			this.CheckAndSendMessage(new RequestToJoinPremadeGameMessage(gameId, text));
		}

		// Token: 0x06000638 RID: 1592 RVA: 0x0000A375 File Offset: 0x00008575
		public void AcceptJoinPremadeGameRequest(Guid partyId)
		{
			this.CheckAndSendMessage(new AcceptJoinPremadeGameRequestMessage(partyId));
		}

		// Token: 0x06000639 RID: 1593 RVA: 0x0000A383 File Offset: 0x00008583
		public void DeclineJoinPremadeGameRequest(Guid partyId)
		{
			this.CheckAndSendMessage(new DeclineJoinPremadeGameRequestMessage(partyId));
		}

		// Token: 0x0600063A RID: 1594 RVA: 0x0000A391 File Offset: 0x00008591
		public void InviteToParty(PlayerId playerId, bool dontUseNameForUnknownPlayer)
		{
			this.CheckAndSendMessage(new InviteToPartyMessage(playerId, dontUseNameForUnknownPlayer));
		}

		// Token: 0x0600063B RID: 1595 RVA: 0x0000A3A0 File Offset: 0x000085A0
		public void DisbandParty()
		{
			this.CheckAndSendMessage(new DisbandPartyMessage());
		}

		// Token: 0x0600063C RID: 1596 RVA: 0x0000A3AD File Offset: 0x000085AD
		public void Test_CreateChatRoom(string name)
		{
			this.CheckAndSendMessage(new Test_CreateChatRoomMessage(name));
		}

		// Token: 0x0600063D RID: 1597 RVA: 0x0000A3BB File Offset: 0x000085BB
		public void Test_AddChatRoomUser(string name)
		{
			this.CheckAndSendMessage(new Test_AddChatRoomUser(name));
		}

		// Token: 0x0600063E RID: 1598 RVA: 0x0000A3C9 File Offset: 0x000085C9
		public void Test_RemoveChatRoomUser(string name)
		{
			this.CheckAndSendMessage(new Test_RemoveChatRoomUser(name));
		}

		// Token: 0x0600063F RID: 1599 RVA: 0x0000A3D7 File Offset: 0x000085D7
		public void Test_DeleteChatRoom(Guid id)
		{
			this.CheckAndSendMessage(new Test_DeleteChatRoomMessage(id));
		}

		// Token: 0x06000640 RID: 1600 RVA: 0x0000A3E5 File Offset: 0x000085E5
		public IEnumerable<string> Test_ListChatRoomIds()
		{
			return this.ChatManager.Rooms.Select((ChatRoomInformationForClient room) => room.RoomId.ToString());
		}

		// Token: 0x06000641 RID: 1601 RVA: 0x0000A416 File Offset: 0x00008616
		public void KickPlayerFromParty(PlayerId playerId)
		{
			this.CheckAndSendMessage(new KickPlayerFromPartyMessage(playerId));
		}

		// Token: 0x06000642 RID: 1602 RVA: 0x0000A424 File Offset: 0x00008624
		public void OnPlayerNameUpdated(string name)
		{
			this._userName = name;
		}

		// Token: 0x06000643 RID: 1603 RVA: 0x0000A42D File Offset: 0x0000862D
		public void ToggleUseClanSigil(bool isUsed)
		{
			this.CheckAndSendMessage(new UpdateUsingClanSigil(isUsed));
		}

		// Token: 0x06000644 RID: 1604 RVA: 0x0000A43B File Offset: 0x0000863B
		public void PromotePlayerToPartyLeader(PlayerId playerId)
		{
			this.CheckAndSendMessage(new PromotePlayerToPartyLeaderMessage(playerId));
		}

		// Token: 0x06000645 RID: 1605 RVA: 0x0000A449 File Offset: 0x00008649
		public void ChangeSigil(string sigilId)
		{
			this.CheckAndSendMessage(new ChangePlayerSigilMessage(sigilId));
		}

		// Token: 0x06000646 RID: 1606 RVA: 0x0000A458 File Offset: 0x00008658
		public async Task<bool> InviteToPlatformSession(PlayerId playerId)
		{
			bool flag = false;
			if (this._handler != null)
			{
				flag = await this._handler.OnInviteToPlatformSession(playerId);
			}
			return flag;
		}

		// Token: 0x06000647 RID: 1607 RVA: 0x0000A4A8 File Offset: 0x000086A8
		public async void EndCustomGame()
		{
			await base.CallFunction<EndHostingCustomGameResult>(new EndHostingCustomGameMessage());
			if (this._handler != null)
			{
				this._handler.OnCustomGameEnd();
			}
			this.CurrentState = LobbyClient.State.AtLobby;
		}

		// Token: 0x06000648 RID: 1608 RVA: 0x0000A4E4 File Offset: 0x000086E4
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

		// Token: 0x06000649 RID: 1609 RVA: 0x0000A56B File Offset: 0x0000876B
		public void ResponseCustomGameClientConnection(PlayerJoinGameResponseDataFromHost[] playerJoinData)
		{
			base.SendMessage(new ResponseCustomGameClientConnectionMessage(playerJoinData));
		}

		// Token: 0x0600064A RID: 1610 RVA: 0x0000A579 File Offset: 0x00008779
		public void AcceptPartyInvitation()
		{
			this.IsPartyInvitationPopupActive = false;
			this.CheckAndSendMessage(new AcceptPartyInvitationMessage());
		}

		// Token: 0x0600064B RID: 1611 RVA: 0x0000A58D File Offset: 0x0000878D
		public void DeclinePartyInvitation()
		{
			this.IsPartyInvitationPopupActive = false;
			this.CheckAndSendMessage(new DeclinePartyInvitationMessage());
		}

		// Token: 0x0600064C RID: 1612 RVA: 0x0000A5A1 File Offset: 0x000087A1
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

		// Token: 0x0600064D RID: 1613 RVA: 0x0000A5D8 File Offset: 0x000087D8
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

		// Token: 0x0600064E RID: 1614 RVA: 0x0000A628 File Offset: 0x00008828
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

		// Token: 0x0600064F RID: 1615 RVA: 0x0000A678 File Offset: 0x00008878
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

		// Token: 0x06000650 RID: 1616 RVA: 0x0000A6C8 File Offset: 0x000088C8
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

		// Token: 0x06000651 RID: 1617 RVA: 0x0000A710 File Offset: 0x00008910
		public async Task<List<ValueTuple<PlayerId, AnotherPlayerData>>> GetOtherPlayersState(List<PlayerId> players)
		{
			this.AssertCanPerformLobbyActions();
			GetOtherPlayersStateMessageResult getOtherPlayersStateMessageResult = await base.CallFunction<GetOtherPlayersStateMessageResult>(new GetOtherPlayersStateMessage(players));
			return (getOtherPlayersStateMessageResult != null) ? getOtherPlayersStateMessageResult.States : null;
		}

		// Token: 0x06000652 RID: 1618 RVA: 0x0000A760 File Offset: 0x00008960
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

		// Token: 0x06000653 RID: 1619 RVA: 0x0000A7A8 File Offset: 0x000089A8
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

		// Token: 0x06000654 RID: 1620 RVA: 0x0000A7F0 File Offset: 0x000089F0
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

		// Token: 0x06000655 RID: 1621 RVA: 0x0000A840 File Offset: 0x00008A40
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

		// Token: 0x06000656 RID: 1622 RVA: 0x0000A890 File Offset: 0x00008A90
		public async Task<PlayerLeaderboardData[]> GetRankedLeaderboard(string gameType)
		{
			GetRankedLeaderboardMessageResult getRankedLeaderboardMessageResult = await base.CallFunction<GetRankedLeaderboardMessageResult>(new GetRankedLeaderboardMessage(gameType));
			return (getRankedLeaderboardMessageResult != null) ? getRankedLeaderboardMessageResult.LeaderboardPlayers : null;
		}

		// Token: 0x06000657 RID: 1623 RVA: 0x0000A8DD File Offset: 0x00008ADD
		public void SendCreateClanMessage(string clanName, string clanTag, string clanFaction, string clanSigil)
		{
			this.AssertCanPerformLobbyActions();
			base.SendMessage(new CreateClanMessage(clanName, clanTag, clanFaction, clanSigil));
		}

		// Token: 0x06000658 RID: 1624 RVA: 0x0000A8F8 File Offset: 0x00008AF8
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

		// Token: 0x06000659 RID: 1625 RVA: 0x0000A93D File Offset: 0x00008B3D
		public void GetFriendList()
		{
			this.CheckAndSendMessage(new GetFriendListMessage());
		}

		// Token: 0x0600065A RID: 1626 RVA: 0x0000A94A File Offset: 0x00008B4A
		public void AddFriend(PlayerId friendId, bool dontUseNameForUnknownPlayer)
		{
			this.CheckAndSendMessage(new AddFriendMessage(friendId, dontUseNameForUnknownPlayer));
		}

		// Token: 0x0600065B RID: 1627 RVA: 0x0000A959 File Offset: 0x00008B59
		public void RemoveFriend(PlayerId friendId)
		{
			this.CheckAndSendMessage(new RemoveFriendMessage(friendId));
		}

		// Token: 0x0600065C RID: 1628 RVA: 0x0000A967 File Offset: 0x00008B67
		public void RespondToFriendRequest(PlayerId playerId, bool dontUseNameForUnknownPlayer, bool isAccepted, bool isBlocked = false)
		{
			this.CheckAndSendMessage(new FriendRequestResponseMessage(playerId, dontUseNameForUnknownPlayer, isAccepted, isBlocked));
		}

		// Token: 0x0600065D RID: 1629 RVA: 0x0000A97C File Offset: 0x00008B7C
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

		// Token: 0x0600065E RID: 1630 RVA: 0x0000A9C8 File Offset: 0x00008BC8
		public void ChangeUsername(string username)
		{
			if ((this.PlayerData == null || this.PlayerData.Username != username) && username != null && username.Length >= Parameters.UsernameMinLength && username.Length <= Parameters.UsernameMaxLength && Common.IsAllLetters(username))
			{
				this.CheckAndSendMessage(new ChangeUsernameMessage(username));
			}
		}

		// Token: 0x0600065F RID: 1631 RVA: 0x0000AA21 File Offset: 0x00008C21
		void IChatClientHandler.OnChatMessageReceived(Guid roomId, string roomName, string playerName, string textMessage, string textColor, MessageType type)
		{
			if (this._handler != null)
			{
				this._handler.OnChatMessageReceived(roomId, roomName, playerName, textMessage, textColor, type);
			}
		}

		// Token: 0x06000660 RID: 1632 RVA: 0x0000AA40 File Offset: 0x00008C40
		public void AddFriendByUsernameAndId(string username, int userId, bool dontUseNameForUnknownPlayer)
		{
			if (username != null && username.Length >= Parameters.UsernameMinLength && username.Length <= Parameters.UsernameMaxLength && Common.IsAllLetters(username) && userId >= 0 && userId <= Parameters.UserIdMax)
			{
				this.CheckAndSendMessage(new AddFriendByUsernameAndIdMessage(username, userId, dontUseNameForUnknownPlayer));
			}
		}

		// Token: 0x06000661 RID: 1633 RVA: 0x0000AA8C File Offset: 0x00008C8C
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

		// Token: 0x06000662 RID: 1634 RVA: 0x0000AAE4 File Offset: 0x00008CE4
		public bool IsPlayerClanLeader(PlayerId playerID)
		{
			ClanPlayer clanPlayer = this.PlayersInClan.Find((ClanPlayer p) => p.PlayerId == playerID);
			return clanPlayer != null && clanPlayer.Role == ClanPlayerRole.Leader;
		}

		// Token: 0x06000663 RID: 1635 RVA: 0x0000AB24 File Offset: 0x00008D24
		public bool IsPlayerClanOfficer(PlayerId playerID)
		{
			ClanPlayer clanPlayer = this.PlayersInClan.Find((ClanPlayer p) => p.PlayerId == playerID);
			return clanPlayer != null && clanPlayer.Role == ClanPlayerRole.Officer;
		}

		// Token: 0x06000664 RID: 1636 RVA: 0x0000AB64 File Offset: 0x00008D64
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

		// Token: 0x06000665 RID: 1637 RVA: 0x0000ABB4 File Offset: 0x00008DB4
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

		// Token: 0x06000666 RID: 1638 RVA: 0x0000AC04 File Offset: 0x00008E04
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

		// Token: 0x06000667 RID: 1639 RVA: 0x0000AC4C File Offset: 0x00008E4C
		public async Task<string> GetDedicatedCustomServerAuthToken()
		{
			GetDedicatedCustomServerAuthTokenMessageResult getDedicatedCustomServerAuthTokenMessageResult = await base.CallFunction<GetDedicatedCustomServerAuthTokenMessageResult>(new GetDedicatedCustomServerAuthTokenMessage());
			return (getDedicatedCustomServerAuthTokenMessageResult != null) ? getDedicatedCustomServerAuthTokenMessageResult.AuthToken : null;
		}

		// Token: 0x06000668 RID: 1640 RVA: 0x0000AC94 File Offset: 0x00008E94
		public async Task<string> GetOfficialServerProviderName()
		{
			GetOfficialServerProviderNameResult getOfficialServerProviderNameResult = await base.CallFunction<GetOfficialServerProviderNameResult>(new GetOfficialServerProviderNameMessage());
			return ((getOfficialServerProviderNameResult != null) ? getOfficialServerProviderNameResult.Name : null) ?? string.Empty;
		}

		// Token: 0x06000669 RID: 1641 RVA: 0x0000ACDC File Offset: 0x00008EDC
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

		// Token: 0x0600066A RID: 1642 RVA: 0x0000AD2C File Offset: 0x00008F2C
		public bool IsKnownPlayer(PlayerId playerID)
		{
			bool flag = playerID == this._playerId;
			bool flag2 = this.FriendIDs.Contains(playerID);
			bool flag3 = this.IsInParty && this.PlayersInParty.Any((PartyPlayerInLobbyClient p) => p.PlayerId.Equals(playerID));
			bool flag4 = this.IsInClan && this.PlayersInClan.Any((ClanPlayer p) => p.PlayerId.Equals(playerID));
			return flag || flag2 || flag3 || flag4;
		}

		// Token: 0x0600066B RID: 1643 RVA: 0x0000ADB8 File Offset: 0x00008FB8
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

		// Token: 0x0600066C RID: 1644 RVA: 0x0000ADFD File Offset: 0x00008FFD
		private void AssertCanPerformLobbyActions()
		{
		}

		// Token: 0x0600066D RID: 1645 RVA: 0x0000AE00 File Offset: 0x00009000
		public async Task<bool> SendPSPlayerJoinedToPlayerSessionMessage(ulong inviterAccountId)
		{
			PSPlayerJoinedToPlayerSessionMessage psplayerJoinedToPlayerSessionMessage = new PSPlayerJoinedToPlayerSessionMessage(inviterAccountId);
			return (await base.CallFunction<PSPlayerJoinedToPlayerSessionMessageResult>(psplayerJoinedToPlayerSessionMessage)).Successful;
		}

		// Token: 0x0600066E RID: 1646 RVA: 0x0000AE50 File Offset: 0x00009050
		private Task WaitForPendingRequestCompletion(LobbyClient.PendingRequest requestType, PlayerId playerId)
		{
			Task task;
			if (this._pendingPlayerRequests.TryGetValue(new ValueTuple<LobbyClient.PendingRequest, PlayerId>(requestType, playerId), out task))
			{
				return task;
			}
			return Task.CompletedTask;
		}

		// Token: 0x0600066F RID: 1647 RVA: 0x0000AE7C File Offset: 0x0000907C
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

		// Token: 0x040002BA RID: 698
		public const string TestRegionCode = "Test";

		// Token: 0x040002BB RID: 699
		private static readonly int ServerStatusCheckDelay = 30000;

		// Token: 0x040002BC RID: 700
		private static int _friendListCheckDelay;

		// Token: 0x040002BD RID: 701
		private static readonly int CheckForCustomGamesCount = 5;

		// Token: 0x040002BE RID: 702
		private static readonly int CheckForCustomGamesDelay = 5000;

		// Token: 0x040002BF RID: 703
		private ILobbyClientSessionHandler _handler;

		// Token: 0x040002C0 RID: 704
		private readonly Stopwatch _serverStatusTimer;

		// Token: 0x040002C1 RID: 705
		private readonly Stopwatch _friendListTimer;

		// Token: 0x040002C6 RID: 710
		private List<string> _ownedCosmetics;

		// Token: 0x040002C7 RID: 711
		private Dictionary<string, List<string>> _usedCosmetics;

		// Token: 0x040002C9 RID: 713
		private ServerStatus _serverStatus;

		// Token: 0x040002CA RID: 714
		private DateTime _matchmakerBlockedTime;

		// Token: 0x040002CB RID: 715
		private TextObject _logOutReason;

		// Token: 0x040002CC RID: 716
		private LobbyClient.State _state;

		// Token: 0x040002CD RID: 717
		private string _userName;

		// Token: 0x040002CE RID: 718
		private PlayerId _playerId;

		// Token: 0x040002D1 RID: 721
		private List<ModuleInfoModel> _loadedUnofficialModules;

		// Token: 0x040002E1 RID: 737
		public List<PlayerId> FriendIDs;

		// Token: 0x040002E2 RID: 738
		private TimedDictionaryCache<PlayerId, GameTypeRankInfo[]> _cachedRankInfos;

		// Token: 0x040002E3 RID: 739
		private TimedDictionaryCache<PlayerId, PlayerStatsBase[]> _cachedPlayerStats;

		// Token: 0x040002E4 RID: 740
		private TimedDictionaryCache<PlayerId, PlayerData> _cachedPlayerDatas;

		// Token: 0x040002E5 RID: 741
		private TimedDictionaryCache<PlayerId, string> _cachedPlayerBannerlordIDs;

		// Token: 0x040002E6 RID: 742
		private Dictionary<ValueTuple<LobbyClient.PendingRequest, PlayerId>, Task> _pendingPlayerRequests;

		// Token: 0x02000183 RID: 387
		public enum State
		{
			// Token: 0x0400053F RID: 1343
			Idle,
			// Token: 0x04000540 RID: 1344
			Working,
			// Token: 0x04000541 RID: 1345
			Connected,
			// Token: 0x04000542 RID: 1346
			SessionRequested,
			// Token: 0x04000543 RID: 1347
			AtLobby,
			// Token: 0x04000544 RID: 1348
			SearchingToRejoinBattle,
			// Token: 0x04000545 RID: 1349
			RequestingToSearchBattle,
			// Token: 0x04000546 RID: 1350
			RequestingToCancelSearchBattle,
			// Token: 0x04000547 RID: 1351
			SearchingBattle,
			// Token: 0x04000548 RID: 1352
			AtBattle,
			// Token: 0x04000549 RID: 1353
			QuittingFromBattle,
			// Token: 0x0400054A RID: 1354
			WaitingToCreatePremadeGame,
			// Token: 0x0400054B RID: 1355
			WaitingToJoinPremadeGame,
			// Token: 0x0400054C RID: 1356
			WaitingToRegisterCustomGame,
			// Token: 0x0400054D RID: 1357
			HostingCustomGame,
			// Token: 0x0400054E RID: 1358
			WaitingToJoinCustomGame,
			// Token: 0x0400054F RID: 1359
			InCustomGame
		}

		// Token: 0x02000184 RID: 388
		private enum PendingRequest
		{
			// Token: 0x04000551 RID: 1361
			RankInfo,
			// Token: 0x04000552 RID: 1362
			PlayerData,
			// Token: 0x04000553 RID: 1363
			BannerlordID
		}
	}
}
