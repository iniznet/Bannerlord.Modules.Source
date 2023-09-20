using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Messages.FromCustomBattleServer.ToCustomBattleServerManager;
using Messages.FromCustomBattleServerManager.ToCustomBattleServer;
using TaleWorlds.Diamond;
using TaleWorlds.Diamond.ClientApplication;
using TaleWorlds.Library;
using TaleWorlds.ModuleManager;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.Diamond
{
	public class CustomBattleServer : Client<CustomBattleServer>
	{
		public bool Finished
		{
			get
			{
				return this._state == CustomBattleServer.State.Finished;
			}
		}

		public bool IsRegistered
		{
			get
			{
				return this._state == CustomBattleServer.State.RegisteredGame || this._state == CustomBattleServer.State.RegisteredServer;
			}
		}

		public bool IsPlaying
		{
			get
			{
				return this._state == CustomBattleServer.State.RegisteredGame;
			}
		}

		public bool Connected
		{
			get
			{
				return this.CurrentState != CustomBattleServer.State.Working && this.CurrentState > CustomBattleServer.State.Idle;
			}
		}

		public CustomBattleServer.State CurrentState
		{
			get
			{
				return this._state;
			}
			private set
			{
				if (this._state != value)
				{
					CustomBattleServer.State state = this._state;
					this._state = value;
					if (this._handler != null)
					{
						this._handler.OnStateChanged(state);
					}
				}
			}
		}

		public bool IsIdle
		{
			get
			{
				return this._state == CustomBattleServer.State.RegisteredGame && this._customBattlePlayers.Count == 0 && this._useTimeoutTimer && this._timeoutTimer.ElapsedMilliseconds > (long)this._timeoutDuration;
			}
		}

		public string CustomGameType { get; private set; }

		public string CustomGameScene { get; private set; }

		public int Port { get; private set; }

		public MultipleBattleResult BattleResult { get; private set; }

		public CustomBattleServer(DiamondClientApplication diamondClientApplication, IClientSessionProvider<CustomBattleServer> provider)
			: base(diamondClientApplication, provider, false)
		{
			this._peerId = new PeerId(Guid.NewGuid());
			this._customBattlePlayers = new List<PlayerId>();
			this._requestedPlayers = new List<PlayerId>();
			this._timeoutTimer = new Stopwatch();
			this._terminationTime = null;
			this._state = CustomBattleServer.State.Idle;
			this._timer = new Stopwatch();
			this._timer.Start();
			if (!base.Application.Parameters.TryGetParameterAsInt("CustomBattleServer.TimeoutDuration", out this._timeoutDuration))
			{
				this._timeoutDuration = this._defaultServerTimeoutDuration;
			}
			this._badgeComponent = null;
			this._badgeComponentPlayers = new List<PlayerData>();
			this.BattleResult = new MultipleBattleResult();
			base.AddMessageHandler<ClientWantsToConnectCustomGameMessage>(new ClientMessageHandler<ClientWantsToConnectCustomGameMessage>(this.OnClientWantsToConnectCustomGameMessage));
			base.AddMessageHandler<ClientQuitFromCustomGameMessage>(new ClientMessageHandler<ClientQuitFromCustomGameMessage>(this.OnClientQuitFromCustomGameMessage));
			base.AddMessageHandler<TerminateOperationCustomMessage>(new ClientMessageHandler<TerminateOperationCustomMessage>(this.OnTerminateOperationCustomMessage));
			base.AddMessageHandler<SetChatFilterListsMessage>(new ClientMessageHandler<SetChatFilterListsMessage>(this.OnSetChatFilterListsMessage));
		}

		public void SetBadgeComponent(IBadgeComponent badgeComponent)
		{
			this._badgeComponent = badgeComponent;
			if (this._badgeComponent != null)
			{
				foreach (PlayerData playerData in this._badgeComponentPlayers)
				{
					this._badgeComponent.OnPlayerJoin(playerData);
				}
			}
		}

		public void Connect(ICustomBattleServerSessionHandler handler, string authToken, bool isSinglePlatformServer, string[] loadedModuleIDs, bool allowsOptionalModules, bool isPlayerHosted)
		{
			this._handler = handler;
			this._authToken = authToken;
			this._allowsOptionalModules = allowsOptionalModules;
			this._useTimeoutTimer = !isPlayerHosted;
			this._isSinglePlatformServer = isSinglePlatformServer;
			this._loadedModules = new List<ModuleInfoModel>();
			foreach (ModuleInfo moduleInfo in ModuleHelper.GetSortedModules(loadedModuleIDs))
			{
				if (!allowsOptionalModules && moduleInfo.Category == ModuleCategory.MultiplayerOptional)
				{
					throw new InvalidOperationException("Optional modules are explicitly disallowed, yet an optional module (" + moduleInfo.Id + ") was loaded! You must use category 'Server' instead of 'MultiplayerOptional'.");
				}
				ModuleInfoModel moduleInfoModel;
				if (ModuleInfoModel.TryCreateForSession(moduleInfo, out moduleInfoModel))
				{
					this._loadedModules.Add(moduleInfoModel);
				}
			}
			this.CurrentState = CustomBattleServer.State.Working;
			base.BeginConnect();
		}

		public override void OnConnected()
		{
			base.OnConnected();
			this.CurrentState = CustomBattleServer.State.Connected;
			if (this._handler != null)
			{
				this._handler.OnConnected();
			}
		}

		public override void OnCantConnect()
		{
			base.OnCantConnect();
			this.CurrentState = CustomBattleServer.State.Idle;
			if (this._handler != null)
			{
				this._handler.OnCantConnect();
			}
		}

		public override void OnDisconnected()
		{
			base.OnDisconnected();
			this.CurrentState = CustomBattleServer.State.Idle;
			if (this._handler != null)
			{
				this._handler.OnDisconnected();
			}
		}

		protected override void OnTick()
		{
			if (this._terminationTime != null && this._terminationTime < DateTime.UtcNow)
			{
				throw new Exception("Now I am become Death, the destroyer of worlds");
			}
			long elapsedMilliseconds = this._timer.ElapsedMilliseconds;
			float num = (float)(elapsedMilliseconds - this._previousTimeInMS);
			this._previousTimeInMS = elapsedMilliseconds;
			float num2 = num / 1000f;
			this._battleResultUpdateTimeElapsed += num2;
			if (this._battleResultUpdateTimeElapsed >= 5f)
			{
				if (this._latestQueuedBattleResult != null && this._latestQueuedTeamScores != null && this._latestQueuedPlayerScores != null)
				{
					base.SendMessage(new CustomBattleServerStatsUpdateMessage(this._latestQueuedBattleResult, this._latestQueuedTeamScores, this._latestQueuedPlayerScores));
					this._latestQueuedBattleResult = null;
					this._latestQueuedTeamScores = null;
					this._latestQueuedPlayerScores = null;
				}
				this._battleResultUpdateTimeElapsed = 0f;
			}
			CustomBattleServer.State state = this._state;
			if (state == CustomBattleServer.State.Connected)
			{
				this.DoLogin();
			}
		}

		private async void DoLogin()
		{
			this._state = CustomBattleServer.State.SessionRequested;
			LoginResult loginResult = await base.Login(new CustomBattleServerReadyMessage(this._peerId, base.ApplicationVersion, this._authToken, this._loadedModules.ToArray(), this._allowsOptionalModules));
			if (loginResult != null && loginResult.Successful)
			{
				this._state = CustomBattleServer.State.RegisteredServer;
			}
			else
			{
				Console.WriteLine("Login Failed! Server is shutting down.");
			}
		}

		private void OnClientWantsToConnectCustomGameMessage(ClientWantsToConnectCustomGameMessage message)
		{
			this.HandleOnClientWantsToConnectCustomGameMessage(message);
		}

		private async void HandleOnClientWantsToConnectCustomGameMessage(ClientWantsToConnectCustomGameMessage message)
		{
			List<PlayerJoinGameResponseDataFromHost> responses = new List<PlayerJoinGameResponseDataFromHost>();
			if (this.CurrentState == CustomBattleServer.State.Finished)
			{
				foreach (PlayerJoinGameData playerJoinGameData2 in message.PlayerJoinGameData)
				{
					responses.Add(new PlayerJoinGameResponseDataFromHost
					{
						PlayerId = playerJoinGameData2.PlayerId,
						PeerIndex = -1,
						SessionKey = -1,
						CustomGameJoinResponse = CustomGameJoinResponse.CustomGameServerFinishing
					});
				}
			}
			else
			{
				PlayerJoinGameData[] requestedPlayers = message.PlayerJoinGameData;
				for (int k = 0; k < requestedPlayers.Length; k++)
				{
					if (requestedPlayers[k] != null)
					{
						PlayerJoinGameData playerJoinGameData3 = requestedPlayers[k];
						Debug.Print(string.Concat(new object[] { "Player ", playerJoinGameData3.Name, " - ", playerJoinGameData3.PlayerId, " with IP address ", playerJoinGameData3.IpAddress, " wants to join the game" }), 0, Debug.DebugColor.White, 17592186044416UL);
					}
				}
				int j;
				for (int i = 0; i < requestedPlayers.Length; i = j + 1)
				{
					if (requestedPlayers[i] != null)
					{
						List<PlayerJoinGameData> requestedGroup = new List<PlayerJoinGameData>();
						PlayerJoinGameData playerJoinGameData4 = requestedPlayers[i];
						Guid? guid = playerJoinGameData4.PartyId;
						if (guid == null)
						{
							requestedGroup.Add(playerJoinGameData4);
						}
						else
						{
							for (int l = i; l < requestedPlayers.Length; l++)
							{
								PlayerJoinGameData playerJoinGameData5 = requestedPlayers[l];
								guid = playerJoinGameData4.PartyId;
								if (guid.Equals((playerJoinGameData5 != null) ? playerJoinGameData5.PartyId : null))
								{
									requestedGroup.Add(playerJoinGameData5);
									requestedPlayers[l] = null;
								}
							}
						}
						bool flag = true;
						foreach (PlayerJoinGameData playerJoinGameData6 in requestedGroup)
						{
							if (this._requestedPlayers.Contains(playerJoinGameData6.PlayerId) || this._customBattlePlayers.Contains(playerJoinGameData6.PlayerId))
							{
								flag = false;
								break;
							}
						}
						if (flag)
						{
							this._timeoutTimer.Restart();
							foreach (PlayerJoinGameData playerJoinGameData7 in requestedGroup)
							{
								this._requestedPlayers.Add(playerJoinGameData7.PlayerId);
							}
							if (this._handler != null)
							{
								PlayerJoinGameResponseDataFromHost[] array = await this._handler.OnClientWantsToConnectCustomGame(requestedGroup.ToArray(), message.Password);
								if (this._badgeComponent != null)
								{
									foreach (PlayerJoinGameResponseDataFromHost playerJoinGameResponseDataFromHost in array)
									{
										if (playerJoinGameResponseDataFromHost.CustomGameJoinResponse == CustomGameJoinResponse.Success)
										{
											foreach (PlayerJoinGameData playerJoinGameData8 in requestedGroup)
											{
												if (playerJoinGameData8.PlayerId.Equals(playerJoinGameResponseDataFromHost.PlayerId))
												{
													this._badgeComponent.OnPlayerJoin(playerJoinGameData8.PlayerData);
													this._badgeComponentPlayers.Add(playerJoinGameData8.PlayerData);
												}
											}
										}
									}
								}
								responses.AddRange(array);
							}
						}
						else
						{
							foreach (PlayerJoinGameData playerJoinGameData9 in requestedGroup)
							{
								responses.Add(new PlayerJoinGameResponseDataFromHost
								{
									PlayerId = playerJoinGameData9.PlayerId,
									PeerIndex = -1,
									SessionKey = -1,
									CustomGameJoinResponse = CustomGameJoinResponse.NotAllPlayersReady
								});
							}
						}
						requestedGroup = null;
					}
					j = i;
				}
				requestedPlayers = null;
			}
			this.ResponseCustomGameClientConnection(responses.ToArray());
		}

		private void OnClientQuitFromCustomGameMessage(ClientQuitFromCustomGameMessage message)
		{
			if (this.CurrentState == CustomBattleServer.State.RegisteredGame && this._customBattlePlayers.Contains(message.PlayerId))
			{
				if (this._handler != null)
				{
					this._handler.OnClientQuitFromCustomGame(message.PlayerId);
				}
				this._customBattlePlayers.Remove(message.PlayerId);
			}
		}

		private void OnTerminateOperationCustomMessage(TerminateOperationCustomMessage message)
		{
			Random random = new Random();
			this._terminationTime = new DateTime?(DateTime.UtcNow.AddMilliseconds((double)random.Next(3000, 10000)));
		}

		private void OnSetChatFilterListsMessage(SetChatFilterListsMessage message)
		{
			if (this._handler != null)
			{
				this._handler.OnChatFilterListsReceived(message.ProfanityList, message.AllowList);
			}
		}

		public void ResponseCustomGameClientConnection(PlayerJoinGameResponseDataFromHost[] playerJoinData)
		{
			if (this.CurrentState == CustomBattleServer.State.RegisteredGame)
			{
				foreach (PlayerJoinGameResponseDataFromHost playerJoinGameResponseDataFromHost in playerJoinData)
				{
					this._requestedPlayers.Remove(playerJoinGameResponseDataFromHost.PlayerId);
					if (playerJoinGameResponseDataFromHost.CustomGameJoinResponse == CustomGameJoinResponse.Success)
					{
						this._customBattlePlayers.Add(playerJoinGameResponseDataFromHost.PlayerId);
					}
				}
				base.SendMessage(new ResponseCustomGameClientConnectionMessage(playerJoinData));
			}
		}

		public async Task RegisterGame(string gameModule, string gameType, string serverName, int maxPlayerCount, string scene, string uniqueSceneId, int port, string region, string gamePassword, string adminPassword, int permission)
		{
			await this.RegisterGame(0, gameModule, gameType, serverName, maxPlayerCount, scene, uniqueSceneId, port, region, gamePassword, adminPassword, permission, string.Empty);
		}

		public async Task RegisterGame(int gameDefinitionId, string gameModule, string gameType, string serverName, int maxPlayerCount, string scene, string uniqueSceneId, int port, string region, string gamePassword, string adminPassword, int permission, string overriddenIP)
		{
			this.Port = port;
			this.CustomGameType = gameType;
			this.CustomGameScene = scene;
			string text;
			if (!base.Application.Parameters.TryGetParameter("CustomBattleServer.Host.Address", out text))
			{
				text = null;
			}
			bool flag = false;
			if (overriddenIP != string.Empty)
			{
				flag = true;
				text = overriddenIP;
			}
			RegisterCustomGameMessageResponseMessage registerCustomGameMessageResponseMessage = await base.CallFunction<RegisterCustomGameMessageResponseMessage>(new RegisterCustomGameMessage(gameDefinitionId, gameModule, gameType, serverName, text, maxPlayerCount, scene, uniqueSceneId, gamePassword, adminPassword, port, region, permission, !this._isSinglePlatformServer, flag));
			this._shouldReportActivities = registerCustomGameMessageResponseMessage.ShouldReportActivities;
			this.CurrentState = CustomBattleServer.State.RegisteredGame;
			this._timeoutTimer.Start();
			if (this._handler != null)
			{
				this._handler.OnSuccessfulGameRegister();
			}
		}

		public void KickPlayer(PlayerId id, bool banPlayer)
		{
			ICustomBattleServerSessionHandler handler = this._handler;
			if (handler == null)
			{
				return;
			}
			handler.OnPlayerKickRequested(id, banPlayer);
		}

		public void HandlePlayerDisconnect(PlayerId playerId, DisconnectType disconnectType)
		{
			this._timeoutTimer.Restart();
			this._customBattlePlayers.Remove(playerId);
			base.SendMessage(new PlayerDisconnectedMessage(playerId, disconnectType));
		}

		public void FinishAsIdle(GameLog[] gameLogs)
		{
			this.FinishGame(gameLogs);
			base.BeginDisconnect();
		}

		public void FinishGame(GameLog[] gameLogs)
		{
			this.CurrentState = CustomBattleServer.State.Finished;
			if (this._handler != null)
			{
				this._handler.OnGameFinished();
			}
			IBadgeComponent badgeComponent = this._badgeComponent;
			base.SendMessage(new CustomBattleServerFinishingMessage(gameLogs, (badgeComponent != null) ? badgeComponent.DataDictionary : null, this.BattleResult));
		}

		public void UpdateGameProperties(string gameType, string scene, string uniqueSceneId)
		{
			this.CustomGameType = gameType;
			this.CustomGameScene = scene;
			base.SendMessage(new UpdateGamePropertiesMessage(gameType, scene, uniqueSceneId));
		}

		public void BeforeStartingNextBattle(GameLog[] gameLogs)
		{
			IBadgeComponent badgeComponent = this._badgeComponent;
			if (badgeComponent != null)
			{
				badgeComponent.OnStartingNextBattle();
			}
			if (gameLogs != null && gameLogs.Length != 0)
			{
				base.SendMessage(new AddGameLogsMessage(gameLogs));
			}
		}

		public void BattleStarted(Dictionary<PlayerId, int> playerTeams, string cultureTeam1, string cultureTeam2)
		{
			if (this._shouldReportActivities)
			{
				base.SendMessage(new CustomBattleStartedMessage(this.CustomGameType, playerTeams, new List<string> { cultureTeam2, cultureTeam1 }));
			}
		}

		public void BattleFinished(BattleResult battleResult, Dictionary<int, int> teamScores, Dictionary<PlayerId, int> playerScores)
		{
			if (this._shouldReportActivities)
			{
				base.SendMessage(new CustomBattleFinishedMessage(battleResult, teamScores, playerScores));
			}
		}

		public void UpdateBattleStats(BattleResult battleResult, Dictionary<int, int> teamScores, Dictionary<PlayerId, int> playerScores)
		{
			if (this._shouldReportActivities)
			{
				this._latestQueuedBattleResult = battleResult;
				this._latestQueuedTeamScores = teamScores;
				this._latestQueuedPlayerScores = playerScores;
			}
		}

		private CustomBattleServer.State _state;

		private string _authToken;

		private List<ModuleInfoModel> _loadedModules;

		private bool _allowsOptionalModules;

		private bool _isSinglePlatformServer;

		private Stopwatch _timer;

		private long _previousTimeInMS;

		private ICustomBattleServerSessionHandler _handler;

		private PeerId _peerId;

		private List<PlayerId> _customBattlePlayers;

		private List<PlayerId> _requestedPlayers;

		private int _defaultServerTimeoutDuration = 600000;

		private int _timeoutDuration;

		private Stopwatch _timeoutTimer;

		private DateTime? _terminationTime;

		private bool _useTimeoutTimer;

		private IBadgeComponent _badgeComponent;

		private readonly List<PlayerData> _badgeComponentPlayers;

		private bool _shouldReportActivities;

		private const float BattleResultUpdatePeriod = 5f;

		private float _battleResultUpdateTimeElapsed;

		private BattleResult _latestQueuedBattleResult;

		private Dictionary<int, int> _latestQueuedTeamScores;

		private Dictionary<PlayerId, int> _latestQueuedPlayerScores;

		public enum State
		{
			Idle,
			Working,
			Connected,
			SessionRequested,
			RegisteredServer,
			RegisteredGame,
			Finished
		}
	}
}
