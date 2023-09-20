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
	// Token: 0x02000108 RID: 264
	public class CustomBattleServer : Client<CustomBattleServer>
	{
		// Token: 0x170001D8 RID: 472
		// (get) Token: 0x060004CB RID: 1227 RVA: 0x000070B2 File Offset: 0x000052B2
		public bool Finished
		{
			get
			{
				return this._state == CustomBattleServer.State.Finished;
			}
		}

		// Token: 0x170001D9 RID: 473
		// (get) Token: 0x060004CC RID: 1228 RVA: 0x000070BD File Offset: 0x000052BD
		public bool IsRegistered
		{
			get
			{
				return this._state == CustomBattleServer.State.RegisteredGame || this._state == CustomBattleServer.State.RegisteredServer;
			}
		}

		// Token: 0x170001DA RID: 474
		// (get) Token: 0x060004CD RID: 1229 RVA: 0x000070D3 File Offset: 0x000052D3
		public bool IsPlaying
		{
			get
			{
				return this._state == CustomBattleServer.State.RegisteredGame;
			}
		}

		// Token: 0x170001DB RID: 475
		// (get) Token: 0x060004CE RID: 1230 RVA: 0x000070DE File Offset: 0x000052DE
		public bool Connected
		{
			get
			{
				return this.CurrentState != CustomBattleServer.State.Working && this.CurrentState > CustomBattleServer.State.Idle;
			}
		}

		// Token: 0x170001DC RID: 476
		// (get) Token: 0x060004CF RID: 1231 RVA: 0x000070F4 File Offset: 0x000052F4
		// (set) Token: 0x060004D0 RID: 1232 RVA: 0x000070FC File Offset: 0x000052FC
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

		// Token: 0x170001DD RID: 477
		// (get) Token: 0x060004D1 RID: 1233 RVA: 0x00007134 File Offset: 0x00005334
		public bool IsIdle
		{
			get
			{
				return this._state == CustomBattleServer.State.RegisteredGame && this._customBattlePlayers.Count == 0 && this._useTimeoutTimer && this._timeoutTimer.ElapsedMilliseconds > (long)this._timeoutDuration;
			}
		}

		// Token: 0x170001DE RID: 478
		// (get) Token: 0x060004D2 RID: 1234 RVA: 0x0000716C File Offset: 0x0000536C
		// (set) Token: 0x060004D3 RID: 1235 RVA: 0x00007174 File Offset: 0x00005374
		public string CustomGameType { get; private set; }

		// Token: 0x170001DF RID: 479
		// (get) Token: 0x060004D4 RID: 1236 RVA: 0x0000717D File Offset: 0x0000537D
		// (set) Token: 0x060004D5 RID: 1237 RVA: 0x00007185 File Offset: 0x00005385
		public string CustomGameScene { get; private set; }

		// Token: 0x170001E0 RID: 480
		// (get) Token: 0x060004D6 RID: 1238 RVA: 0x0000718E File Offset: 0x0000538E
		// (set) Token: 0x060004D7 RID: 1239 RVA: 0x00007196 File Offset: 0x00005396
		public int Port { get; private set; }

		// Token: 0x170001E1 RID: 481
		// (get) Token: 0x060004D8 RID: 1240 RVA: 0x0000719F File Offset: 0x0000539F
		// (set) Token: 0x060004D9 RID: 1241 RVA: 0x000071A7 File Offset: 0x000053A7
		public MultipleBattleResult BattleResult { get; private set; }

		// Token: 0x060004DA RID: 1242 RVA: 0x000071B0 File Offset: 0x000053B0
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

		// Token: 0x060004DB RID: 1243 RVA: 0x000072BC File Offset: 0x000054BC
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

		// Token: 0x060004DC RID: 1244 RVA: 0x00007324 File Offset: 0x00005524
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

		// Token: 0x060004DD RID: 1245 RVA: 0x000073F0 File Offset: 0x000055F0
		public override void OnConnected()
		{
			base.OnConnected();
			this.CurrentState = CustomBattleServer.State.Connected;
			if (this._handler != null)
			{
				this._handler.OnConnected();
			}
		}

		// Token: 0x060004DE RID: 1246 RVA: 0x00007412 File Offset: 0x00005612
		public override void OnCantConnect()
		{
			base.OnCantConnect();
			this.CurrentState = CustomBattleServer.State.Idle;
			if (this._handler != null)
			{
				this._handler.OnCantConnect();
			}
		}

		// Token: 0x060004DF RID: 1247 RVA: 0x00007434 File Offset: 0x00005634
		public override void OnDisconnected()
		{
			base.OnDisconnected();
			this.CurrentState = CustomBattleServer.State.Idle;
			if (this._handler != null)
			{
				this._handler.OnDisconnected();
			}
		}

		// Token: 0x060004E0 RID: 1248 RVA: 0x00007458 File Offset: 0x00005658
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

		// Token: 0x060004E1 RID: 1249 RVA: 0x0000754C File Offset: 0x0000574C
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

		// Token: 0x060004E2 RID: 1250 RVA: 0x00007585 File Offset: 0x00005785
		private void OnClientWantsToConnectCustomGameMessage(ClientWantsToConnectCustomGameMessage message)
		{
			this.HandleOnClientWantsToConnectCustomGameMessage(message);
		}

		// Token: 0x060004E3 RID: 1251 RVA: 0x00007590 File Offset: 0x00005790
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

		// Token: 0x060004E4 RID: 1252 RVA: 0x000075D4 File Offset: 0x000057D4
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

		// Token: 0x060004E5 RID: 1253 RVA: 0x00007628 File Offset: 0x00005828
		private void OnTerminateOperationCustomMessage(TerminateOperationCustomMessage message)
		{
			Random random = new Random();
			this._terminationTime = new DateTime?(DateTime.UtcNow.AddMilliseconds((double)random.Next(3000, 10000)));
		}

		// Token: 0x060004E6 RID: 1254 RVA: 0x00007664 File Offset: 0x00005864
		private void OnSetChatFilterListsMessage(SetChatFilterListsMessage message)
		{
			if (this._handler != null)
			{
				this._handler.OnChatFilterListsReceived(message.ProfanityList, message.AllowList);
			}
		}

		// Token: 0x060004E7 RID: 1255 RVA: 0x00007688 File Offset: 0x00005888
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

		// Token: 0x060004E8 RID: 1256 RVA: 0x000076EC File Offset: 0x000058EC
		public async Task RegisterGame(string gameModule, string gameType, string serverName, int maxPlayerCount, string scene, string uniqueSceneId, int port, string region, string gamePassword, string adminPassword, int permission)
		{
			await this.RegisterGame(0, gameModule, gameType, serverName, maxPlayerCount, scene, uniqueSceneId, port, region, gamePassword, adminPassword, permission, string.Empty);
		}

		// Token: 0x060004E9 RID: 1257 RVA: 0x00007794 File Offset: 0x00005994
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

		// Token: 0x060004EA RID: 1258 RVA: 0x0000784B File Offset: 0x00005A4B
		public void KickPlayer(PlayerId id, bool banPlayer)
		{
			ICustomBattleServerSessionHandler handler = this._handler;
			if (handler == null)
			{
				return;
			}
			handler.OnPlayerKickRequested(id, banPlayer);
		}

		// Token: 0x060004EB RID: 1259 RVA: 0x0000785F File Offset: 0x00005A5F
		public void HandlePlayerDisconnect(PlayerId playerId, DisconnectType disconnectType)
		{
			this._timeoutTimer.Restart();
			this._customBattlePlayers.Remove(playerId);
			base.SendMessage(new PlayerDisconnectedMessage(playerId, disconnectType));
		}

		// Token: 0x060004EC RID: 1260 RVA: 0x00007886 File Offset: 0x00005A86
		public void FinishAsIdle(GameLog[] gameLogs)
		{
			this.FinishGame(gameLogs);
			base.BeginDisconnect();
		}

		// Token: 0x060004ED RID: 1261 RVA: 0x00007895 File Offset: 0x00005A95
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

		// Token: 0x060004EE RID: 1262 RVA: 0x000078D5 File Offset: 0x00005AD5
		public void UpdateGameProperties(string gameType, string scene, string uniqueSceneId)
		{
			this.CustomGameType = gameType;
			this.CustomGameScene = scene;
			base.SendMessage(new UpdateGamePropertiesMessage(gameType, scene, uniqueSceneId));
		}

		// Token: 0x060004EF RID: 1263 RVA: 0x000078F3 File Offset: 0x00005AF3
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

		// Token: 0x060004F0 RID: 1264 RVA: 0x00007919 File Offset: 0x00005B19
		public void BattleStarted(Dictionary<PlayerId, int> playerTeams, string cultureTeam1, string cultureTeam2)
		{
			if (this._shouldReportActivities)
			{
				base.SendMessage(new CustomBattleStartedMessage(this.CustomGameType, playerTeams, new List<string> { cultureTeam2, cultureTeam1 }));
			}
		}

		// Token: 0x060004F1 RID: 1265 RVA: 0x00007948 File Offset: 0x00005B48
		public void BattleFinished(BattleResult battleResult, Dictionary<int, int> teamScores, Dictionary<PlayerId, int> playerScores)
		{
			if (this._shouldReportActivities)
			{
				base.SendMessage(new CustomBattleFinishedMessage(battleResult, teamScores, playerScores));
			}
		}

		// Token: 0x060004F2 RID: 1266 RVA: 0x00007960 File Offset: 0x00005B60
		public void UpdateBattleStats(BattleResult battleResult, Dictionary<int, int> teamScores, Dictionary<PlayerId, int> playerScores)
		{
			if (this._shouldReportActivities)
			{
				this._latestQueuedBattleResult = battleResult;
				this._latestQueuedTeamScores = teamScores;
				this._latestQueuedPlayerScores = playerScores;
			}
		}

		// Token: 0x0400021F RID: 543
		private CustomBattleServer.State _state;

		// Token: 0x04000220 RID: 544
		private string _authToken;

		// Token: 0x04000221 RID: 545
		private List<ModuleInfoModel> _loadedModules;

		// Token: 0x04000222 RID: 546
		private bool _allowsOptionalModules;

		// Token: 0x04000223 RID: 547
		private bool _isSinglePlatformServer;

		// Token: 0x04000224 RID: 548
		private Stopwatch _timer;

		// Token: 0x04000225 RID: 549
		private long _previousTimeInMS;

		// Token: 0x0400022A RID: 554
		private ICustomBattleServerSessionHandler _handler;

		// Token: 0x0400022B RID: 555
		private PeerId _peerId;

		// Token: 0x0400022C RID: 556
		private List<PlayerId> _customBattlePlayers;

		// Token: 0x0400022D RID: 557
		private List<PlayerId> _requestedPlayers;

		// Token: 0x0400022E RID: 558
		private int _defaultServerTimeoutDuration = 600000;

		// Token: 0x0400022F RID: 559
		private int _timeoutDuration;

		// Token: 0x04000230 RID: 560
		private Stopwatch _timeoutTimer;

		// Token: 0x04000231 RID: 561
		private DateTime? _terminationTime;

		// Token: 0x04000232 RID: 562
		private bool _useTimeoutTimer;

		// Token: 0x04000233 RID: 563
		private IBadgeComponent _badgeComponent;

		// Token: 0x04000234 RID: 564
		private readonly List<PlayerData> _badgeComponentPlayers;

		// Token: 0x04000235 RID: 565
		private bool _shouldReportActivities;

		// Token: 0x04000236 RID: 566
		private const float BattleResultUpdatePeriod = 5f;

		// Token: 0x04000237 RID: 567
		private float _battleResultUpdateTimeElapsed;

		// Token: 0x04000238 RID: 568
		private BattleResult _latestQueuedBattleResult;

		// Token: 0x04000239 RID: 569
		private Dictionary<int, int> _latestQueuedTeamScores;

		// Token: 0x0400023A RID: 570
		private Dictionary<PlayerId, int> _latestQueuedPlayerScores;

		// Token: 0x0200017A RID: 378
		public enum State
		{
			// Token: 0x040004FF RID: 1279
			Idle,
			// Token: 0x04000500 RID: 1280
			Working,
			// Token: 0x04000501 RID: 1281
			Connected,
			// Token: 0x04000502 RID: 1282
			SessionRequested,
			// Token: 0x04000503 RID: 1283
			RegisteredServer,
			// Token: 0x04000504 RID: 1284
			RegisteredGame,
			// Token: 0x04000505 RID: 1285
			Finished
		}
	}
}
