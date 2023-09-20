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
	public class BattleServer : Client<BattleServer>
	{
		public string SceneName { get; private set; }

		public string GameType { get; private set; }

		public string Faction1 { get; private set; }

		public string Faction2 { get; private set; }

		public int MinRequiredPlayerCountToStartBattle { get; private set; }

		public int BattleSize { get; private set; }

		public int RoundThreshold { get; private set; }

		public float MoraleThreshold { get; private set; }

		public Guid BattleId { get; private set; }

		public bool UseAnalytics { get; private set; }

		public bool CaptureMovementData { get; private set; }

		public string AnalyticsServiceAddress { get; private set; }

		public bool IsPremadeGame { get; private set; }

		public PremadeGameType PremadeGameType { get; private set; }

		public PlayerId[] AssignedPlayers { get; private set; }

		public bool IsActive
		{
			get
			{
				return this._state == BattleServer.State.BattleAssigned || this._state == BattleServer.State.Running || this._state == BattleServer.State.WaitingBattle;
			}
		}

		public bool IsFinished
		{
			get
			{
				return this._state == BattleServer.State.Finished;
			}
		}

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

		public void Initialize(IBattleServerSessionHandler handler)
		{
			this._handler = handler;
		}

		public void SetBadgeComponent(IBadgeComponent badgeComponent)
		{
			this._badgeComponent = badgeComponent;
		}

		public void StartServer()
		{
			this._state = BattleServer.State.Connecting;
			base.BeginConnect();
		}

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

		public override void OnConnected()
		{
			base.OnConnected();
			this._state = BattleServer.State.Connected;
			this._handler.OnConnected();
			this.DoLogin();
		}

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

		public void BeginEndMission()
		{
			this._state = BattleServer.State.Finishing;
			base.SendMessage(new BattleEndingMessage());
		}

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

		public void BattleCancelledForPlayerLeaving(PlayerId leaverID)
		{
			base.SendMessage(new BattleCancelledDueToPlayerQuitMessage(leaverID));
		}

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

		public void UpdateBattleStats(BattleResult battleResult, Dictionary<int, int> teamScores)
		{
			if (this._shouldReportActivities)
			{
				this._latestQueuedBattleResult = battleResult;
				this._latestQueuedTeamScores = teamScores;
			}
		}

		private void Shutdown()
		{
			this._state = BattleServer.State.Finished;
			base.BeginDisconnect();
			this._handler.OnDisconnected();
		}

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

		private void OnTerminateOperationMatchmakingMessage(TerminateOperationMatchmakingMessage message)
		{
			Random random = new Random();
			this._terminationTime = new DateTime?(DateTime.UtcNow.AddMilliseconds((double)random.Next(3000, 10000)));
		}

		public void DoNotAcceptNewPlayers()
		{
			base.SendMessage(new StopAcceptingNewPlayersMessage());
		}

		public void OnWarmupEnded()
		{
			this._isWarmupEnded = true;
		}

		public void OnPlayerSpawned(PlayerId playerId)
		{
			int num;
			if (!this._playerSpawnCounts.TryGetValue(playerId, out num))
			{
				num = 0;
			}
			this._playerSpawnCounts[playerId] = num + 1;
		}

		public BattlePeer GetPeer(string name)
		{
			return this._peers.First((BattlePeer peer) => peer.Name == name);
		}

		public BattlePeer GetPeer(PlayerId playerId)
		{
			return this._peers.FirstOrDefault((BattlePeer peer) => peer.PlayerId == playerId);
		}

		public Guid GetPlayerParty(PlayerId playerId)
		{
			Guid empty;
			if (!this._playerPartyMap.TryGetValue(playerId, out empty))
			{
				empty = Guid.Empty;
			}
			return empty;
		}

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

		private async void UpdateMaxAllowedPriority()
		{
			this._passedTimeSinceLastMaxAllowedPriorityRequest = 0f;
			sbyte b = await this.GetMaxAllowedPriority();
			this._maxAllowedPriority = b;
		}

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

		private async Task<sbyte> GetMaxAllowedPriority()
		{
			return (await base.CallFunction<RequestMaxAllowedPriorityResponse>(new RequestMaxAllowedPriorityMessage())).Priority;
		}

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

		private BattleServer.State _state = BattleServer.State.Connecting;

		private IBattleServerSessionHandler _handler;

		private List<BattlePeer> _peers;

		private string _assignedAddress;

		private ushort _assignedPort;

		private string _region;

		private sbyte _priority;

		private sbyte _maxAllowedPriority;

		private byte _numCores;

		private string _password;

		private string _gameMode;

		private PeerId _peerId;

		private float _requestMaxAllowedPriorityIntervalInSeconds = 10f;

		private float _passedTimeSinceLastMaxAllowedPriorityRequest;

		private Stopwatch _timer;

		private long _previousTimeInMS;

		private Queue<NewPlayerMessage> _newPlayerRequests;

		private bool _battleBecomeReady;

		private int _defaultServerTimeoutDuration = 600000;

		private int _timeoutDuration;

		private Stopwatch _timeoutTimer;

		private DateTime? _terminationTime;

		private bool _isWarmupEnded;

		private Dictionary<PlayerId, int> _playerSpawnCounts;

		private IBadgeComponent _badgeComponent;

		private Dictionary<PlayerId, Guid> _playerPartyMap;

		[TupleElementNames(new string[] { "killCount", "damage" })]
		private Dictionary<PlayerId, Dictionary<int, ValueTuple<int, float>>> _playerRoundFriendlyDamageMap;

		private int _maxFriendlyKillCount;

		private float _maxFriendlyDamage;

		private float _maxFriendlyDamagePerSingleRound;

		private float _roundFriendlyDamageLimit;

		private int _maxRoundsOverLimitCount;

		private bool _shouldReportActivities;

		private const float BattleResultUpdatePeriod = 5f;

		private float _battleResultUpdateTimeElapsed;

		private BattleResult _latestQueuedBattleResult;

		private Dictionary<int, int> _latestQueuedTeamScores;

		private enum State
		{
			Idle,
			Connecting,
			Connected,
			LoggingIn,
			WaitingBattle,
			BattleAssigned,
			Running,
			Finishing,
			Finished
		}
	}
}
