using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Messages.FromBattleServer.ToBattleServerManager;
using Messages.FromBattleServerManager.ToBattleServer;
using TaleWorlds.Diamond;
using TaleWorlds.Diamond.ClientApplication;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x020000F0 RID: 240
	public class BattleServer : Client<BattleServer>
	{
		// Token: 0x17000189 RID: 393
		// (get) Token: 0x060003E5 RID: 997 RVA: 0x00004FBB File Offset: 0x000031BB
		// (set) Token: 0x060003E6 RID: 998 RVA: 0x00004FC3 File Offset: 0x000031C3
		public string SceneName { get; private set; }

		// Token: 0x1700018A RID: 394
		// (get) Token: 0x060003E7 RID: 999 RVA: 0x00004FCC File Offset: 0x000031CC
		// (set) Token: 0x060003E8 RID: 1000 RVA: 0x00004FD4 File Offset: 0x000031D4
		public string GameType { get; private set; }

		// Token: 0x1700018B RID: 395
		// (get) Token: 0x060003E9 RID: 1001 RVA: 0x00004FDD File Offset: 0x000031DD
		// (set) Token: 0x060003EA RID: 1002 RVA: 0x00004FE5 File Offset: 0x000031E5
		public string Faction1 { get; private set; }

		// Token: 0x1700018C RID: 396
		// (get) Token: 0x060003EB RID: 1003 RVA: 0x00004FEE File Offset: 0x000031EE
		// (set) Token: 0x060003EC RID: 1004 RVA: 0x00004FF6 File Offset: 0x000031F6
		public string Faction2 { get; private set; }

		// Token: 0x1700018D RID: 397
		// (get) Token: 0x060003ED RID: 1005 RVA: 0x00004FFF File Offset: 0x000031FF
		// (set) Token: 0x060003EE RID: 1006 RVA: 0x00005007 File Offset: 0x00003207
		public int MinRequiredPlayerCountToStartBattle { get; private set; }

		// Token: 0x1700018E RID: 398
		// (get) Token: 0x060003EF RID: 1007 RVA: 0x00005010 File Offset: 0x00003210
		// (set) Token: 0x060003F0 RID: 1008 RVA: 0x00005018 File Offset: 0x00003218
		public int BattleSize { get; private set; }

		// Token: 0x1700018F RID: 399
		// (get) Token: 0x060003F1 RID: 1009 RVA: 0x00005021 File Offset: 0x00003221
		// (set) Token: 0x060003F2 RID: 1010 RVA: 0x00005029 File Offset: 0x00003229
		public int RoundThreshold { get; private set; }

		// Token: 0x17000190 RID: 400
		// (get) Token: 0x060003F3 RID: 1011 RVA: 0x00005032 File Offset: 0x00003232
		// (set) Token: 0x060003F4 RID: 1012 RVA: 0x0000503A File Offset: 0x0000323A
		public float MoraleThreshold { get; private set; }

		// Token: 0x17000191 RID: 401
		// (get) Token: 0x060003F5 RID: 1013 RVA: 0x00005043 File Offset: 0x00003243
		// (set) Token: 0x060003F6 RID: 1014 RVA: 0x0000504B File Offset: 0x0000324B
		public Guid BattleId { get; private set; }

		// Token: 0x17000192 RID: 402
		// (get) Token: 0x060003F7 RID: 1015 RVA: 0x00005054 File Offset: 0x00003254
		// (set) Token: 0x060003F8 RID: 1016 RVA: 0x0000505C File Offset: 0x0000325C
		public bool UseAnalytics { get; private set; }

		// Token: 0x17000193 RID: 403
		// (get) Token: 0x060003F9 RID: 1017 RVA: 0x00005065 File Offset: 0x00003265
		// (set) Token: 0x060003FA RID: 1018 RVA: 0x0000506D File Offset: 0x0000326D
		public bool CaptureMovementData { get; private set; }

		// Token: 0x17000194 RID: 404
		// (get) Token: 0x060003FB RID: 1019 RVA: 0x00005076 File Offset: 0x00003276
		// (set) Token: 0x060003FC RID: 1020 RVA: 0x0000507E File Offset: 0x0000327E
		public string AnalyticsServiceAddress { get; private set; }

		// Token: 0x17000195 RID: 405
		// (get) Token: 0x060003FD RID: 1021 RVA: 0x00005087 File Offset: 0x00003287
		// (set) Token: 0x060003FE RID: 1022 RVA: 0x0000508F File Offset: 0x0000328F
		public bool IsPremadeGame { get; private set; }

		// Token: 0x17000196 RID: 406
		// (get) Token: 0x060003FF RID: 1023 RVA: 0x00005098 File Offset: 0x00003298
		// (set) Token: 0x06000400 RID: 1024 RVA: 0x000050A0 File Offset: 0x000032A0
		public PremadeGameType PremadeGameType { get; private set; }

		// Token: 0x17000197 RID: 407
		// (get) Token: 0x06000401 RID: 1025 RVA: 0x000050A9 File Offset: 0x000032A9
		// (set) Token: 0x06000402 RID: 1026 RVA: 0x000050B1 File Offset: 0x000032B1
		public PlayerId[] AssignedPlayers { get; private set; }

		// Token: 0x17000198 RID: 408
		// (get) Token: 0x06000403 RID: 1027 RVA: 0x000050BA File Offset: 0x000032BA
		public bool IsActive
		{
			get
			{
				return this._state == BattleServer.State.BattleAssigned || this._state == BattleServer.State.Running || this._state == BattleServer.State.WaitingBattle;
			}
		}

		// Token: 0x17000199 RID: 409
		// (get) Token: 0x06000404 RID: 1028 RVA: 0x000050D9 File Offset: 0x000032D9
		public bool IsFinished
		{
			get
			{
				return this._state == BattleServer.State.Finished;
			}
		}

		// Token: 0x06000405 RID: 1029 RVA: 0x000050E4 File Offset: 0x000032E4
		public BattleServer(DiamondClientApplication diamondClientApplication, IClientSessionProvider<BattleServer> provider)
			: base(diamondClientApplication, provider, false)
		{
			this._state = BattleServer.State.Idle;
			this._peerId = new PeerId(Guid.NewGuid());
			base.Application.Parameters.TryGetParameter("BattleServer.Host.Address", out this._assignedAddress);
			base.Application.Parameters.TryGetParameterAsUInt16("BattleServer.Host.Port", out this._assignedPort);
			base.Application.Parameters.TryGetParameter("BattleServer.Host.Region", out this._region);
			base.Application.Parameters.TryGetParameterAsSByte("BattleServer.Host.Priority", out this._priority);
			base.Application.Parameters.TryGetParameterAsByte("BattleServer.Host.NumCores", out this._numCores);
			base.Application.Parameters.TryGetParameter("BattleServer.Password", out this._password);
			base.Application.Parameters.TryGetParameter("BattleServer.Host.GameMode", out this._gameMode);
			if (!base.Application.Parameters.TryGetParameterAsInt("BattleServer.TimeoutDuration", out this._timeoutDuration))
			{
				this._timeoutDuration = this._defaultServerTimeoutDuration;
			}
			this._passedTimeSinceLastMaxAllowedPriorityRequest = this._requestMaxAllowedPriorityIntervalInSeconds * 2f;
			this._peers = new List<BattlePeer>();
			this._timer = new Stopwatch();
			this._timer.Start();
			this._timeoutTimer = new Stopwatch();
			this._terminationTime = null;
			this._maxAllowedPriority = sbyte.MaxValue;
			this._newPlayerRequests = new Queue<NewPlayerMessage>();
			this._isWarmupEnded = false;
			this._playerSpawnCounts = new Dictionary<PlayerId, int>();
			this._badgeComponent = null;
			this._playerPartyMap = new Dictionary<PlayerId, Guid>();
			this._playerRoundFriendlyDamageMap = new Dictionary<PlayerId, Dictionary<int, ValueTuple<int, float>>>();
			this._maxFriendlyKillCount = int.MaxValue;
			this._maxFriendlyDamage = float.MaxValue;
			this._maxFriendlyDamagePerSingleRound = float.MaxValue;
			this._roundFriendlyDamageLimit = float.MaxValue;
			this._maxRoundsOverLimitCount = int.MaxValue;
			base.AddMessageHandler<NewPlayerMessage>(new ClientMessageHandler<NewPlayerMessage>(this.OnNewPlayerMessage));
			base.AddMessageHandler<StartBattleMessage>(new ClientMessageHandler<StartBattleMessage>(this.OnStartBattleMessage));
			base.AddMessageHandler<PlayerFledBattleMessage>(new ClientMessageHandler<PlayerFledBattleMessage>(this.OnPlayerFledBattleMessage));
			base.AddMessageHandler<PlayerDisconnectedFromLobbyMessage>(new ClientMessageHandler<PlayerDisconnectedFromLobbyMessage>(this.OnPlayerDisconnectedFromLobbyMessage));
			base.AddMessageHandler<TerminateOperationMatchmakingMessage>(new ClientMessageHandler<TerminateOperationMatchmakingMessage>(this.OnTerminateOperationMatchmakingMessage));
			base.AddMessageHandler<FriendlyDamageKickPlayerResponseMessage>(new ClientMessageHandler<FriendlyDamageKickPlayerResponseMessage>(this.OnFriendlyDamageKickPlayerResponseMessage));
		}

		// Token: 0x06000406 RID: 1030 RVA: 0x0000534A File Offset: 0x0000354A
		public void Initialize(IBattleServerSessionHandler handler)
		{
			this._handler = handler;
		}

		// Token: 0x06000407 RID: 1031 RVA: 0x00005353 File Offset: 0x00003553
		public void SetBadgeComponent(IBadgeComponent badgeComponent)
		{
			this._badgeComponent = badgeComponent;
		}

		// Token: 0x06000408 RID: 1032 RVA: 0x0000535C File Offset: 0x0000355C
		public void StartServer()
		{
			this._state = BattleServer.State.Connecting;
			base.BeginConnect();
		}

		// Token: 0x06000409 RID: 1033 RVA: 0x0000536C File Offset: 0x0000356C
		protected override void OnTick()
		{
			if (this._terminationTime != null && this._terminationTime < DateTime.UtcNow)
			{
				throw new Exception("I am sorry Dave, I am afraid I can't do that");
			}
			long elapsedMilliseconds = this._timer.ElapsedMilliseconds;
			float num = (float)(elapsedMilliseconds - this._previousTimeInMS);
			this._previousTimeInMS = elapsedMilliseconds;
			float num2 = num / 1000f;
			this._passedTimeSinceLastMaxAllowedPriorityRequest += num2;
			this._battleResultUpdateTimeElapsed += num2;
			if (this._battleResultUpdateTimeElapsed >= 5f)
			{
				if (this._latestQueuedBattleResult != null && this._latestQueuedTeamScores != null)
				{
					base.SendMessage(new BattleServerStatsUpdateMessage(this._latestQueuedBattleResult, this._latestQueuedTeamScores));
					this._latestQueuedBattleResult = null;
					this._latestQueuedTeamScores = null;
				}
				this._battleResultUpdateTimeElapsed = 0f;
			}
			switch (this._state)
			{
			case BattleServer.State.Idle:
			case BattleServer.State.Connecting:
			case BattleServer.State.Connected:
			case BattleServer.State.LoggingIn:
			case BattleServer.State.BattleAssigned:
			case BattleServer.State.Running:
			case BattleServer.State.Finishing:
			case BattleServer.State.Finished:
				break;
			case BattleServer.State.WaitingBattle:
				if (this._passedTimeSinceLastMaxAllowedPriorityRequest > this._requestMaxAllowedPriorityIntervalInSeconds)
				{
					this.UpdateMaxAllowedPriority();
				}
				if (this._priority > this._maxAllowedPriority || this._timeoutTimer.ElapsedMilliseconds > (long)this._timeoutDuration)
				{
					this.Shutdown();
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x0600040A RID: 1034 RVA: 0x000054B8 File Offset: 0x000036B8
		private async void DoLogin()
		{
			this._state = BattleServer.State.LoggingIn;
			LoginResult loginResult = await base.Login(new BattleServerReadyMessage(this._peerId, base.ApplicationVersion, this._assignedAddress, this._assignedPort, this._region, this._priority, this._password, this._gameMode));
			if (loginResult != null && loginResult.Successful)
			{
				this._state = BattleServer.State.WaitingBattle;
				this._timeoutTimer.Reset();
				this._timeoutTimer.Start();
			}
			else
			{
				this._state = BattleServer.State.Finished;
			}
		}

		// Token: 0x0600040B RID: 1035 RVA: 0x000054F1 File Offset: 0x000036F1
		public override void OnConnected()
		{
			base.OnConnected();
			this._state = BattleServer.State.Connected;
			this._handler.OnConnected();
			this.DoLogin();
		}

		// Token: 0x0600040C RID: 1036 RVA: 0x00005511 File Offset: 0x00003711
		public override void OnCantConnect()
		{
			base.OnCantConnect();
			this._handler.OnCantConnect();
			this._state = BattleServer.State.Finished;
			if (this._handler != null)
			{
				this._handler.OnStopServer();
			}
		}

		// Token: 0x0600040D RID: 1037 RVA: 0x0000553E File Offset: 0x0000373E
		public override void OnDisconnected()
		{
			base.OnDisconnected();
			this._handler.OnDisconnected();
			this._state = BattleServer.State.Finished;
			if (this._handler != null)
			{
				this._handler.OnStopServer();
			}
		}

		// Token: 0x0600040E RID: 1038 RVA: 0x0000556C File Offset: 0x0000376C
		private void OnNewPlayerMessage(NewPlayerMessage message)
		{
			if (this._battleBecomeReady)
			{
				PlayerBattleInfo playerBattleInfo = message.PlayerBattleInfo;
				PlayerData playerData = message.PlayerData;
				this.ProcessNewPlayer(playerBattleInfo, playerData, message.PlayerParty, message.UsedCosmetics);
				return;
			}
			this._newPlayerRequests.Enqueue(message);
		}

		// Token: 0x0600040F RID: 1039 RVA: 0x000055B0 File Offset: 0x000037B0
		private void ProcessNewPlayer(PlayerBattleInfo playerBattleInfo, PlayerData playerData, Guid playerParty, Dictionary<string, List<string>> usedCosmetics)
		{
			string name = playerBattleInfo.Name;
			PlayerId playerId = playerBattleInfo.PlayerId;
			int teamNo = playerBattleInfo.TeamNo;
			this._playerPartyMap[playerData.PlayerId] = playerParty;
			BattlePeer battlePeer = this.GetPeer(playerId);
			if (battlePeer == null)
			{
				battlePeer = new BattlePeer(name, playerData, usedCosmetics, teamNo, playerBattleInfo.JoinType);
				this._peers.Add(battlePeer);
			}
			else
			{
				battlePeer.Rejoin(teamNo);
			}
			if (!this._playerSpawnCounts.ContainsKey(playerId))
			{
				this._playerSpawnCounts.Add(playerId, 0);
			}
			this._handler.OnNewPlayer(battlePeer);
			IBadgeComponent badgeComponent = this._badgeComponent;
			if (badgeComponent != null)
			{
				badgeComponent.OnPlayerJoin(playerData);
			}
			PlayerBattleServerInformation playerBattleServerInformation = new PlayerBattleServerInformation(battlePeer.Index, battlePeer.SessionKey);
			base.SendMessage(new NewPlayerResponseMessage(playerId, playerBattleServerInformation));
		}

		// Token: 0x06000410 RID: 1040 RVA: 0x0000566F File Offset: 0x0000386F
		public void BeginEndMission()
		{
			this._state = BattleServer.State.Finishing;
			base.SendMessage(new BattleEndingMessage());
		}

		// Token: 0x06000411 RID: 1041 RVA: 0x00005684 File Offset: 0x00003884
		public void EndMission(BattleResult battleResult, GameLog[] gameLogs, int gameTime, Dictionary<int, int> teamScores, Dictionary<PlayerId, int> playerScores)
		{
			this._state = BattleServer.State.Finished;
			this.SetBattleJoinTypes(battleResult);
			IBadgeComponent badgeComponent = this._badgeComponent;
			base.SendMessage(new BattleEndedMessage(battleResult, gameLogs, (badgeComponent != null) ? badgeComponent.DataDictionary : null, gameTime, teamScores, playerScores));
			if (this._handler != null)
			{
				this._handler.OnEndMission();
			}
		}

		// Token: 0x06000412 RID: 1042 RVA: 0x000056D6 File Offset: 0x000038D6
		public void BattleCancelledForPlayerLeaving(PlayerId leaverID)
		{
			base.SendMessage(new BattleCancelledDueToPlayerQuitMessage(leaverID));
		}

		// Token: 0x06000413 RID: 1043 RVA: 0x000056E4 File Offset: 0x000038E4
		public void BattleStarted(BattleResult battleResult)
		{
			if (this._shouldReportActivities)
			{
				Dictionary<PlayerId, int> dictionary = new Dictionary<PlayerId, int>();
				foreach (KeyValuePair<PlayerId, BattlePlayerEntry> keyValuePair in battleResult.PlayerEntries)
				{
					dictionary.Add(keyValuePair.Key, keyValuePair.Value.TeamNo);
				}
				base.SendMessage(new BattleStartedMessage(dictionary));
			}
		}

		// Token: 0x06000414 RID: 1044 RVA: 0x00005764 File Offset: 0x00003964
		public void UpdateBattleStats(BattleResult battleResult, Dictionary<int, int> teamScores)
		{
			if (this._shouldReportActivities)
			{
				this._latestQueuedBattleResult = battleResult;
				this._latestQueuedTeamScores = teamScores;
			}
		}

		// Token: 0x06000415 RID: 1045 RVA: 0x0000577C File Offset: 0x0000397C
		private void Shutdown()
		{
			this._state = BattleServer.State.Finished;
			base.BeginDisconnect();
			this._handler.OnDisconnected();
		}

		// Token: 0x06000416 RID: 1046 RVA: 0x00005798 File Offset: 0x00003998
		private void OnStartBattleMessage(StartBattleMessage message)
		{
			this.BattleId = message.BattleId;
			this.SceneName = message.SceneName;
			this.Faction1 = message.Faction1;
			this.Faction2 = message.Faction2;
			this.GameType = message.GameType;
			this.MinRequiredPlayerCountToStartBattle = message.MinRequiredPlayerCountToStartBattle;
			this.BattleSize = message.BattleSize;
			this.RoundThreshold = message.RoundThreshold;
			this.MoraleThreshold = message.MoraleThreshold;
			this.UseAnalytics = message.UseAnalytics;
			this.CaptureMovementData = message.CaptureMovementData;
			this.AnalyticsServiceAddress = message.AnalyticsServiceAddress;
			this.IsPremadeGame = message.IsPremadeGame;
			this.PremadeGameType = message.PremadeGameType;
			this.AssignedPlayers = message.AssignedPlayers;
			this._maxFriendlyKillCount = message.MaxFriendlyKillCount;
			this._maxFriendlyDamage = message.MaxFriendlyDamage;
			this._maxFriendlyDamagePerSingleRound = message.MaxFriendlyDamagePerSingleRound;
			this._roundFriendlyDamageLimit = message.RoundFriendlyDamageLimit;
			this._maxRoundsOverLimitCount = message.MaxRoundsOverLimitCount;
			this._handler.OnStartGame(this.SceneName, this.GameType, this.Faction1, this.Faction2, this.MinRequiredPlayerCountToStartBattle, this.BattleSize, message.ProfanityList, message.AllowList);
			this._state = BattleServer.State.BattleAssigned;
			base.SendMessage(new BattleInitializedMessage(this.GameType, this.AssignedPlayers.ToList<PlayerId>(), this.Faction2, this.Faction1));
		}

		// Token: 0x06000417 RID: 1047 RVA: 0x00005900 File Offset: 0x00003B00
		private void OnPlayerFledBattleMessage(PlayerFledBattleMessage message)
		{
			if (this._state == BattleServer.State.Finished)
			{
				return;
			}
			PlayerId playerId = message.PlayerId;
			BattlePeer battlePeer = this._peers.First((BattlePeer peer) => peer.PlayerId == playerId);
			if (!battlePeer.Quit)
			{
				BattleResult battleResult;
				this._handler.OnPlayerFledBattle(battlePeer, out battleResult);
				battlePeer.Flee();
				int num;
				bool flag = !this._isWarmupEnded || this._state == BattleServer.State.Finishing || !this._playerSpawnCounts.TryGetValue(playerId, out num) || num <= 0;
				base.SendMessage(new PlayerFledBattleAnswerMessage(playerId, battleResult, flag));
			}
		}

		// Token: 0x06000418 RID: 1048 RVA: 0x000059A4 File Offset: 0x00003BA4
		private void OnPlayerDisconnectedFromLobbyMessage(PlayerDisconnectedFromLobbyMessage message)
		{
			PlayerId playerId = message.PlayerId;
			BattlePeer battlePeer = this._peers.First((BattlePeer peer) => peer.PlayerId == playerId);
			if (!battlePeer.Quit)
			{
				BattleResult battleResult;
				this._handler.OnPlayerFledBattle(battlePeer, out battleResult);
				battlePeer.SetPlayerDisconnectdFromLobby();
			}
		}

		// Token: 0x06000419 RID: 1049 RVA: 0x000059F8 File Offset: 0x00003BF8
		private void OnFriendlyDamageKickPlayerResponseMessage(FriendlyDamageKickPlayerResponseMessage message)
		{
			PlayerId playerId = message.PlayerId;
			BattlePeer battlePeer = this._peers.First((BattlePeer peer) => peer.PlayerId == playerId);
			if (!battlePeer.Quit)
			{
				BattleResult battleResult;
				this._handler.OnPlayerFledBattle(battlePeer, out battleResult);
				battlePeer.SetPlayerKickedDueToFriendlyDamage();
			}
		}

		// Token: 0x0600041A RID: 1050 RVA: 0x00005A4C File Offset: 0x00003C4C
		private void OnTerminateOperationMatchmakingMessage(TerminateOperationMatchmakingMessage message)
		{
			Random random = new Random();
			this._terminationTime = new DateTime?(DateTime.UtcNow.AddMilliseconds((double)random.Next(3000, 10000)));
		}

		// Token: 0x0600041B RID: 1051 RVA: 0x00005A88 File Offset: 0x00003C88
		public void DoNotAcceptNewPlayers()
		{
			base.SendMessage(new StopAcceptingNewPlayersMessage());
		}

		// Token: 0x0600041C RID: 1052 RVA: 0x00005A95 File Offset: 0x00003C95
		public void OnWarmupEnded()
		{
			this._isWarmupEnded = true;
		}

		// Token: 0x0600041D RID: 1053 RVA: 0x00005AA0 File Offset: 0x00003CA0
		public void OnPlayerSpawned(PlayerId playerId)
		{
			int num;
			if (!this._playerSpawnCounts.TryGetValue(playerId, out num))
			{
				num = 0;
			}
			this._playerSpawnCounts[playerId] = num + 1;
		}

		// Token: 0x0600041E RID: 1054 RVA: 0x00005AD0 File Offset: 0x00003CD0
		public BattlePeer GetPeer(string name)
		{
			return this._peers.First((BattlePeer peer) => peer.Name == name);
		}

		// Token: 0x0600041F RID: 1055 RVA: 0x00005B04 File Offset: 0x00003D04
		public BattlePeer GetPeer(PlayerId playerId)
		{
			return this._peers.FirstOrDefault((BattlePeer peer) => peer.PlayerId == playerId);
		}

		// Token: 0x06000420 RID: 1056 RVA: 0x00005B38 File Offset: 0x00003D38
		public Guid GetPlayerParty(PlayerId playerId)
		{
			Guid empty;
			if (!this._playerPartyMap.TryGetValue(playerId, out empty))
			{
				empty = Guid.Empty;
			}
			return empty;
		}

		// Token: 0x06000421 RID: 1057 RVA: 0x00005B5C File Offset: 0x00003D5C
		public void HandlePlayerDisconnect(PlayerId playerId, DisconnectType disconnectType, BattleResult battleResult)
		{
			BattlePeer battlePeer = this._peers.First((BattlePeer peer) => peer.PlayerId == playerId);
			if (!battlePeer.Quit)
			{
				battlePeer.SetPlayerDisconnectdFromGameSession();
				int num;
				bool flag = !this._isWarmupEnded || this._state == BattleServer.State.Finishing || !this._playerSpawnCounts.TryGetValue(playerId, out num) || num <= 0;
				base.SendMessage(new PlayerDisconnectedMessage(playerId, disconnectType, flag, battleResult));
			}
		}

		// Token: 0x06000422 RID: 1058 RVA: 0x00005BE4 File Offset: 0x00003DE4
		public async void InformGameServerReady()
		{
			BattleReadyResponseMessage battleReadyResponseMessage = await base.CallFunction<BattleReadyResponseMessage>(new BattleReadyMessage());
			this._shouldReportActivities = battleReadyResponseMessage.ShouldReportActivities;
			this._state = BattleServer.State.Running;
			this._battleBecomeReady = true;
			while (this._newPlayerRequests.Count > 0)
			{
				NewPlayerMessage newPlayerMessage = this._newPlayerRequests.Dequeue();
				this.ProcessNewPlayer(newPlayerMessage.PlayerBattleInfo, newPlayerMessage.PlayerData, newPlayerMessage.PlayerParty, newPlayerMessage.UsedCosmetics);
			}
		}

		// Token: 0x06000423 RID: 1059 RVA: 0x00005C20 File Offset: 0x00003E20
		private async void UpdateMaxAllowedPriority()
		{
			this._passedTimeSinceLastMaxAllowedPriorityRequest = 0f;
			sbyte b = await this.GetMaxAllowedPriority();
			this._maxAllowedPriority = b;
		}

		// Token: 0x06000424 RID: 1060 RVA: 0x00005C5C File Offset: 0x00003E5C
		public void OnFriendlyHit(int round, PlayerId hitter, PlayerId victim, float damage)
		{
			if (!this._isWarmupEnded || damage <= 0f || round < 0)
			{
				return;
			}
			Dictionary<int, ValueTuple<int, float>> dictionary;
			if (!this._playerRoundFriendlyDamageMap.TryGetValue(hitter, out dictionary))
			{
				dictionary = new Dictionary<int, ValueTuple<int, float>>();
				this._playerRoundFriendlyDamageMap.Add(hitter, dictionary);
			}
			ValueTuple<int, float> valueTuple;
			if (dictionary.TryGetValue(round, out valueTuple))
			{
				dictionary[round] = new ValueTuple<int, float>(valueTuple.Item1, valueTuple.Item2 + damage);
			}
			else
			{
				dictionary.Add(round, new ValueTuple<int, float>(0, damage));
			}
			float num = 0f;
			int num2 = 0;
			bool flag = false;
			foreach (KeyValuePair<int, ValueTuple<int, float>> keyValuePair in dictionary)
			{
				num += keyValuePair.Value.Item2;
				if (num > this._maxFriendlyDamage || keyValuePair.Value.Item2 > this._maxFriendlyDamagePerSingleRound)
				{
					flag = true;
					break;
				}
				if (keyValuePair.Value.Item2 > this._roundFriendlyDamageLimit)
				{
					num2++;
					if (num2 > this._maxRoundsOverLimitCount)
					{
						flag = true;
						break;
					}
				}
			}
			if (flag)
			{
				base.SendMessage(new FriendlyDamageKickPlayerMessage(hitter, dictionary));
			}
		}

		// Token: 0x06000425 RID: 1061 RVA: 0x00005D88 File Offset: 0x00003F88
		public void OnFriendlyKill(int round, PlayerId killer, PlayerId victim)
		{
			if (!this._isWarmupEnded || round < 0)
			{
				return;
			}
			Dictionary<int, ValueTuple<int, float>> dictionary;
			if (!this._playerRoundFriendlyDamageMap.TryGetValue(killer, out dictionary))
			{
				dictionary = new Dictionary<int, ValueTuple<int, float>>();
				this._playerRoundFriendlyDamageMap.Add(killer, dictionary);
			}
			ValueTuple<int, float> valueTuple;
			if (dictionary.TryGetValue(round, out valueTuple))
			{
				dictionary[round] = new ValueTuple<int, float>(valueTuple.Item1 + 1, valueTuple.Item2);
			}
			else
			{
				dictionary.Add(round, new ValueTuple<int, float>(1, 0f));
			}
			int num = 0;
			foreach (KeyValuePair<int, ValueTuple<int, float>> keyValuePair in dictionary)
			{
				num += keyValuePair.Value.Item1;
				if (num > this._maxFriendlyKillCount)
				{
					base.SendMessage(new FriendlyDamageKickPlayerMessage(killer, dictionary));
					break;
				}
			}
		}

		// Token: 0x06000426 RID: 1062 RVA: 0x00005E64 File Offset: 0x00004064
		private async Task<sbyte> GetMaxAllowedPriority()
		{
			return (await base.CallFunction<RequestMaxAllowedPriorityResponse>(new RequestMaxAllowedPriorityMessage())).Priority;
		}

		// Token: 0x06000427 RID: 1063 RVA: 0x00005EAC File Offset: 0x000040AC
		private void SetBattleJoinTypes(BattleResult battleResult)
		{
			foreach (BattlePlayerEntry battlePlayerEntry in battleResult.PlayerEntries.Values)
			{
				foreach (BattlePeer battlePeer in this._peers)
				{
					if (battlePeer.PlayerId == battlePlayerEntry.PlayerId)
					{
						battlePlayerEntry.BattleJoinType = battlePeer.BattleJoinType;
						break;
					}
				}
			}
		}

		// Token: 0x06000428 RID: 1064 RVA: 0x00005F5C File Offset: 0x0000415C
		public bool AllPlayersConnected()
		{
			PlayerId[] assignedPlayers = this.AssignedPlayers;
			for (int i = 0; i < assignedPlayers.Length; i++)
			{
				PlayerId playerId = assignedPlayers[i];
				if (this._peers.FirstOrDefault((BattlePeer p) => p.PlayerId == playerId) == null)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x04000192 RID: 402
		private BattleServer.State _state = BattleServer.State.Connecting;

		// Token: 0x040001A2 RID: 418
		private IBattleServerSessionHandler _handler;

		// Token: 0x040001A3 RID: 419
		private List<BattlePeer> _peers;

		// Token: 0x040001A4 RID: 420
		private string _assignedAddress;

		// Token: 0x040001A5 RID: 421
		private ushort _assignedPort;

		// Token: 0x040001A6 RID: 422
		private string _region;

		// Token: 0x040001A7 RID: 423
		private sbyte _priority;

		// Token: 0x040001A8 RID: 424
		private sbyte _maxAllowedPriority;

		// Token: 0x040001A9 RID: 425
		private byte _numCores;

		// Token: 0x040001AA RID: 426
		private string _password;

		// Token: 0x040001AB RID: 427
		private string _gameMode;

		// Token: 0x040001AC RID: 428
		private PeerId _peerId;

		// Token: 0x040001AD RID: 429
		private float _requestMaxAllowedPriorityIntervalInSeconds = 10f;

		// Token: 0x040001AE RID: 430
		private float _passedTimeSinceLastMaxAllowedPriorityRequest;

		// Token: 0x040001AF RID: 431
		private Stopwatch _timer;

		// Token: 0x040001B0 RID: 432
		private long _previousTimeInMS;

		// Token: 0x040001B1 RID: 433
		private Queue<NewPlayerMessage> _newPlayerRequests;

		// Token: 0x040001B2 RID: 434
		private bool _battleBecomeReady;

		// Token: 0x040001B3 RID: 435
		private int _defaultServerTimeoutDuration = 600000;

		// Token: 0x040001B4 RID: 436
		private int _timeoutDuration;

		// Token: 0x040001B5 RID: 437
		private Stopwatch _timeoutTimer;

		// Token: 0x040001B6 RID: 438
		private DateTime? _terminationTime;

		// Token: 0x040001B7 RID: 439
		private bool _isWarmupEnded;

		// Token: 0x040001B8 RID: 440
		private Dictionary<PlayerId, int> _playerSpawnCounts;

		// Token: 0x040001B9 RID: 441
		private IBadgeComponent _badgeComponent;

		// Token: 0x040001BA RID: 442
		private Dictionary<PlayerId, Guid> _playerPartyMap;

		// Token: 0x040001BB RID: 443
		[TupleElementNames(new string[] { "killCount", "damage" })]
		private Dictionary<PlayerId, Dictionary<int, ValueTuple<int, float>>> _playerRoundFriendlyDamageMap;

		// Token: 0x040001BC RID: 444
		private int _maxFriendlyKillCount;

		// Token: 0x040001BD RID: 445
		private float _maxFriendlyDamage;

		// Token: 0x040001BE RID: 446
		private float _maxFriendlyDamagePerSingleRound;

		// Token: 0x040001BF RID: 447
		private float _roundFriendlyDamageLimit;

		// Token: 0x040001C0 RID: 448
		private int _maxRoundsOverLimitCount;

		// Token: 0x040001C1 RID: 449
		private bool _shouldReportActivities;

		// Token: 0x040001C2 RID: 450
		private const float BattleResultUpdatePeriod = 5f;

		// Token: 0x040001C3 RID: 451
		private float _battleResultUpdateTimeElapsed;

		// Token: 0x040001C4 RID: 452
		private BattleResult _latestQueuedBattleResult;

		// Token: 0x040001C5 RID: 453
		private Dictionary<int, int> _latestQueuedTeamScores;

		// Token: 0x02000165 RID: 357
		private enum State
		{
			// Token: 0x040004BF RID: 1215
			Idle,
			// Token: 0x040004C0 RID: 1216
			Connecting,
			// Token: 0x040004C1 RID: 1217
			Connected,
			// Token: 0x040004C2 RID: 1218
			LoggingIn,
			// Token: 0x040004C3 RID: 1219
			WaitingBattle,
			// Token: 0x040004C4 RID: 1220
			BattleAssigned,
			// Token: 0x040004C5 RID: 1221
			Running,
			// Token: 0x040004C6 RID: 1222
			Finishing,
			// Token: 0x040004C7 RID: 1223
			Finished
		}
	}
}
