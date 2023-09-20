using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.HUDExtensions
{
	// Token: 0x020000BA RID: 186
	public class MissionMultiplayerHUDExtensionVM : ViewModel
	{
		// Token: 0x060011A5 RID: 4517 RVA: 0x0003A03C File Offset: 0x0003823C
		public MissionMultiplayerHUDExtensionVM(Mission mission)
		{
			this._mission = mission;
			this._missionScoreboardComponent = mission.GetMissionBehavior<MissionScoreboardComponent>();
			this._gameMode = this._mission.GetMissionBehavior<MissionMultiplayerGameModeBaseClient>();
			this.SpectatorControls = new MissionMultiplayerSpectatorHUDVM(this._mission);
			if (this._gameMode.RoundComponent != null)
			{
				this._gameMode.RoundComponent.OnCurrentRoundStateChanged += this.OnCurrentGameModeStateChanged;
			}
			if (this._gameMode.WarmupComponent != null)
			{
				this._gameMode.WarmupComponent.OnWarmupEnded += this.OnCurrentGameModeStateChanged;
			}
			this._missionScoreboardComponent.OnRoundPropertiesChanged += this.UpdateTeamScores;
			MissionPeer.OnTeamChanged += this.OnTeamChanged;
			NetworkCommunicator.OnPeerComponentAdded += this.OnPeerComponentAdded;
			Mission.Current.OnMissionReset += this.OnMissionReset;
			MissionLobbyComponent missionBehavior = mission.GetMissionBehavior<MissionLobbyComponent>();
			this._isTeamsEnabled = missionBehavior.MissionType != MissionLobbyComponent.MultiplayerGameType.FreeForAll && missionBehavior.MissionType != MissionLobbyComponent.MultiplayerGameType.Duel;
			this._missionLobbyEquipmentNetworkComponent = mission.GetMissionBehavior<MissionLobbyEquipmentNetworkComponent>();
			this.IsRoundCountdownAvailable = this._gameMode.IsGameModeUsingRoundCountdown;
			this.IsRoundCountdownSuspended = false;
			this._isTeamScoresEnabled = this._isTeamsEnabled;
			this._isTeamMemberCountsEnabled = missionBehavior.MissionType == MissionLobbyComponent.MultiplayerGameType.Battle;
			this.UpdateShowTeamScores();
			this.Teammates = new MBBindingList<MPPlayerVM>();
			this.Enemies = new MBBindingList<MPPlayerVM>();
			this._teammateDictionary = new Dictionary<MissionPeer, MPPlayerVM>();
			this._enemyDictionary = new Dictionary<MissionPeer, MPPlayerVM>();
			this.ShowHud = true;
			this.RefreshValues();
		}

		// Token: 0x060011A6 RID: 4518 RVA: 0x0003A1D8 File Offset: 0x000383D8
		public override void RefreshValues()
		{
			base.RefreshValues();
			string strValue = MultiplayerOptions.OptionType.GameType.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
			TextObject textObject = new TextObject("{=XJTX8w8M}Warmup Phase - {GAME_MODE}\nWaiting for players to join", null);
			textObject.SetTextVariable("GAME_MODE", GameTexts.FindText("str_multiplayer_official_game_type_name", strValue));
			this.WarmupInfoText = textObject.ToString();
			this.SpectatorControls.RefreshValues();
		}

		// Token: 0x060011A7 RID: 4519 RVA: 0x0003A22E File Offset: 0x0003842E
		private void OnMissionReset(object sender, PropertyChangedEventArgs e)
		{
			this.IsGeneralWarningCountdownActive = false;
		}

		// Token: 0x060011A8 RID: 4520 RVA: 0x0003A238 File Offset: 0x00038438
		private void OnPeerComponentAdded(PeerComponent component)
		{
			if (component.IsMine && component is MissionRepresentativeBase)
			{
				NetworkCommunicator myPeer = GameNetwork.MyPeer;
				MissionRepresentativeBase missionRepresentativeBase = ((myPeer != null) ? myPeer.VirtualPlayer.GetComponent<MissionRepresentativeBase>() : null);
				this.AllyTeamScore = this._missionScoreboardComponent.GetRoundScore(BattleSideEnum.Attacker);
				this.EnemyTeamScore = this._missionScoreboardComponent.GetRoundScore(BattleSideEnum.Defender);
				this._isTeammateAndEnemiesRelevant = Mission.Current.GetMissionBehavior<MissionMultiplayerGameModeBaseClient>().IsGameModeTactical && !Mission.Current.HasMissionBehavior<MissionMultiplayerSiegeClient>() && this._gameMode.GameType != MissionLobbyComponent.MultiplayerGameType.Battle;
				this.CommanderInfo = new CommanderInfoVM(missionRepresentativeBase);
				this.ShowCommanderInfo = true;
				if (this._isTeammateAndEnemiesRelevant)
				{
					this.OnRefreshTeamMembers();
					this.OnRefreshEnemyMembers();
				}
				this.ShowPowerLevels = this._gameMode.GameType == MissionLobbyComponent.MultiplayerGameType.Battle;
			}
		}

		// Token: 0x060011A9 RID: 4521 RVA: 0x0003A308 File Offset: 0x00038508
		public override void OnFinalize()
		{
			MissionPeer.OnTeamChanged -= this.OnTeamChanged;
			if (this._gameMode.RoundComponent != null)
			{
				this._gameMode.RoundComponent.OnCurrentRoundStateChanged -= this.OnCurrentGameModeStateChanged;
			}
			if (this._gameMode.WarmupComponent != null)
			{
				this._gameMode.WarmupComponent.OnWarmupEnded -= this.OnCurrentGameModeStateChanged;
			}
			this._missionScoreboardComponent.OnRoundPropertiesChanged -= this.UpdateTeamScores;
			NetworkCommunicator.OnPeerComponentAdded -= this.OnPeerComponentAdded;
			CommanderInfoVM commanderInfo = this.CommanderInfo;
			if (commanderInfo != null)
			{
				commanderInfo.OnFinalize();
			}
			this.CommanderInfo = null;
			MissionMultiplayerSpectatorHUDVM spectatorControls = this.SpectatorControls;
			if (spectatorControls != null)
			{
				spectatorControls.OnFinalize();
			}
			this.SpectatorControls = null;
			base.OnFinalize();
		}

		// Token: 0x060011AA RID: 4522 RVA: 0x0003A3D8 File Offset: 0x000385D8
		public void Tick(float dt)
		{
			this.IsInWarmup = this._gameMode.IsInWarmup;
			this.CheckTimers(false);
			if (this._isTeammateAndEnemiesRelevant)
			{
				this.OnRefreshTeamMembers();
				this.OnRefreshEnemyMembers();
			}
			CommanderInfoVM commanderInfo = this._commanderInfo;
			if (commanderInfo != null)
			{
				commanderInfo.Tick(dt);
			}
			MissionMultiplayerSpectatorHUDVM spectatorControls = this._spectatorControls;
			if (spectatorControls == null)
			{
				return;
			}
			spectatorControls.Tick(dt);
		}

		// Token: 0x060011AB RID: 4523 RVA: 0x0003A434 File Offset: 0x00038634
		private void CheckTimers(bool forceUpdate = false)
		{
			int num;
			int num2;
			if (this._gameMode.CheckTimer(out num, out num2, forceUpdate))
			{
				this.RemainingRoundTime = TimeSpan.FromSeconds((double)num).ToString("mm':'ss");
				this.WarnRemainingTime = (float)num <= 5f;
				if (this.GeneralWarningCountdown != num2)
				{
					this.IsGeneralWarningCountdownActive = num2 > 0;
					this.GeneralWarningCountdown = num2;
				}
			}
		}

		// Token: 0x060011AC RID: 4524 RVA: 0x0003A499 File Offset: 0x00038699
		private void OnToggleLoadout(bool isActive)
		{
			this.ShowHud = !isActive;
		}

		// Token: 0x060011AD RID: 4525 RVA: 0x0003A4A5 File Offset: 0x000386A5
		public void OnSpectatedAgentFocusIn(Agent followedAgent)
		{
			MissionMultiplayerSpectatorHUDVM spectatorControls = this._spectatorControls;
			if (spectatorControls == null)
			{
				return;
			}
			spectatorControls.OnSpectatedAgentFocusIn(followedAgent);
		}

		// Token: 0x060011AE RID: 4526 RVA: 0x0003A4B8 File Offset: 0x000386B8
		public void OnSpectatedAgentFocusOut(Agent followedPeer)
		{
			MissionMultiplayerSpectatorHUDVM spectatorControls = this._spectatorControls;
			if (spectatorControls == null)
			{
				return;
			}
			spectatorControls.OnSpectatedAgentFocusOut(followedPeer);
		}

		// Token: 0x060011AF RID: 4527 RVA: 0x0003A4CB File Offset: 0x000386CB
		private void OnCurrentGameModeStateChanged()
		{
			this.CheckTimers(true);
		}

		// Token: 0x060011B0 RID: 4528 RVA: 0x0003A4D4 File Offset: 0x000386D4
		private void UpdateTeamScores()
		{
			if (this._isTeamScoresEnabled)
			{
				int roundScore = this._missionScoreboardComponent.GetRoundScore(BattleSideEnum.Attacker);
				int roundScore2 = this._missionScoreboardComponent.GetRoundScore(BattleSideEnum.Defender);
				this.AllyTeamScore = (this._isAttackerTeamAlly ? roundScore : roundScore2);
				this.EnemyTeamScore = (this._isAttackerTeamAlly ? roundScore2 : roundScore);
			}
		}

		// Token: 0x060011B1 RID: 4529 RVA: 0x0003A528 File Offset: 0x00038728
		private void UpdateTeamBanners()
		{
			Team attackerTeam = Mission.Current.AttackerTeam;
			ImageIdentifierVM imageIdentifierVM = new ImageIdentifierVM(BannerCode.CreateFrom((attackerTeam != null) ? attackerTeam.Banner : null), true);
			Team defenderTeam = Mission.Current.DefenderTeam;
			ImageIdentifierVM imageIdentifierVM2 = new ImageIdentifierVM(BannerCode.CreateFrom((defenderTeam != null) ? defenderTeam.Banner : null), true);
			this.AllyBanner = (this._isAttackerTeamAlly ? imageIdentifierVM : imageIdentifierVM2);
			this.EnemyBanner = (this._isAttackerTeamAlly ? imageIdentifierVM2 : imageIdentifierVM);
		}

		// Token: 0x060011B2 RID: 4530 RVA: 0x0003A5A0 File Offset: 0x000387A0
		private void OnTeamChanged(NetworkCommunicator peer, Team previousTeam, Team newTeam)
		{
			if (peer.IsMine)
			{
				if (this._isTeamScoresEnabled || this._gameMode.GameType == MissionLobbyComponent.MultiplayerGameType.Battle)
				{
					this._isAttackerTeamAlly = newTeam.Side == BattleSideEnum.Attacker;
					this.UpdateTeamScores();
				}
				CommanderInfoVM commanderInfo = this.CommanderInfo;
				if (commanderInfo != null)
				{
					commanderInfo.OnTeamChanged();
				}
			}
			if (this.CommanderInfo == null)
			{
				return;
			}
			MPPlayerVM mpplayerVM = this.Teammates.SingleOrDefault((MPPlayerVM x) => x.Peer.GetNetworkPeer() == peer);
			if (mpplayerVM != null)
			{
				mpplayerVM.RefreshTeam();
			}
			string text;
			string text2;
			this.GetTeamColors(Mission.Current.AttackerTeam, out text, out text2);
			if (this._isTeamScoresEnabled || this._gameMode.GameType == MissionLobbyComponent.MultiplayerGameType.Battle)
			{
				string text3;
				string text4;
				this.GetTeamColors(Mission.Current.DefenderTeam, out text3, out text4);
				if (this._isAttackerTeamAlly)
				{
					this.AllyTeamColor = text;
					this.AllyTeamColor2 = text2;
					this.EnemyTeamColor = text3;
					this.EnemyTeamColor2 = text4;
				}
				else
				{
					this.AllyTeamColor = text3;
					this.AllyTeamColor2 = text4;
					this.EnemyTeamColor = text;
					this.EnemyTeamColor2 = text2;
				}
				this.CommanderInfo.RefreshColors(this.AllyTeamColor, this.AllyTeamColor2, this.EnemyTeamColor, this.EnemyTeamColor2);
			}
			else
			{
				this.AllyTeamColor = text;
				this.AllyTeamColor2 = text2;
				this.CommanderInfo.RefreshColors(this.AllyTeamColor, this.AllyTeamColor2, this.EnemyTeamColor, this.EnemyTeamColor2);
			}
			this.UpdateTeamBanners();
		}

		// Token: 0x060011B3 RID: 4531 RVA: 0x0003A70C File Offset: 0x0003890C
		private void GetTeamColors(Team team, out string color, out string color2)
		{
			color = team.Color.ToString("X");
			color = color.Remove(0, 2);
			color = "#" + color + "FF";
			color2 = team.Color2.ToString("X");
			color2 = color2.Remove(0, 2);
			color2 = "#" + color2 + "FF";
		}

		// Token: 0x060011B4 RID: 4532 RVA: 0x0003A780 File Offset: 0x00038980
		private void OnRefreshTeamMembers()
		{
			List<MPPlayerVM> list = this.Teammates.ToList<MPPlayerVM>();
			foreach (MissionPeer missionPeer in VirtualPlayer.Peers<MissionPeer>())
			{
				if (missionPeer.GetNetworkPeer().GetComponent<MissionPeer>() != null && this._playerTeam != null && missionPeer.Team != null && missionPeer.Team == this._playerTeam)
				{
					if (!this._teammateDictionary.ContainsKey(missionPeer))
					{
						MPPlayerVM mpplayerVM = new MPPlayerVM(missionPeer);
						this.Teammates.Add(mpplayerVM);
						this._teammateDictionary.Add(missionPeer, mpplayerVM);
					}
					else
					{
						list.Remove(this._teammateDictionary[missionPeer]);
					}
				}
			}
			foreach (MPPlayerVM mpplayerVM2 in list)
			{
				this.Teammates.Remove(mpplayerVM2);
				this._teammateDictionary.Remove(mpplayerVM2.Peer);
			}
			foreach (MPPlayerVM mpplayerVM3 in this.Teammates)
			{
				mpplayerVM3.RefreshDivision(false);
				mpplayerVM3.RefreshGold();
				mpplayerVM3.RefreshProperties();
				mpplayerVM3.UpdateDisabled();
			}
		}

		// Token: 0x060011B5 RID: 4533 RVA: 0x0003A8F0 File Offset: 0x00038AF0
		private void OnRefreshEnemyMembers()
		{
			List<MPPlayerVM> list = this.Enemies.ToList<MPPlayerVM>();
			foreach (MissionPeer missionPeer in VirtualPlayer.Peers<MissionPeer>())
			{
				if (missionPeer.GetNetworkPeer().GetComponent<MissionPeer>() != null && this._playerTeam != null && missionPeer.Team != null && missionPeer.Team != this._playerTeam && missionPeer.Team != Mission.Current.SpectatorTeam)
				{
					if (!this._enemyDictionary.ContainsKey(missionPeer))
					{
						MPPlayerVM mpplayerVM = new MPPlayerVM(missionPeer);
						this.Enemies.Add(mpplayerVM);
						this._enemyDictionary.Add(missionPeer, mpplayerVM);
					}
					else
					{
						list.Remove(this._enemyDictionary[missionPeer]);
					}
				}
			}
			foreach (MPPlayerVM mpplayerVM2 in list)
			{
				this.Enemies.Remove(mpplayerVM2);
				this._enemyDictionary.Remove(mpplayerVM2.Peer);
			}
			foreach (MPPlayerVM mpplayerVM3 in this.Enemies)
			{
				mpplayerVM3.RefreshDivision(false);
				mpplayerVM3.UpdateDisabled();
			}
		}

		// Token: 0x060011B6 RID: 4534 RVA: 0x0003AA6C File Offset: 0x00038C6C
		private void UpdateShowTeamScores()
		{
			this.ShowTeamScores = !this._gameMode.IsInWarmup && this.ShowCommanderInfo && this._gameMode.GameType != MissionLobbyComponent.MultiplayerGameType.Siege;
		}

		// Token: 0x170005B7 RID: 1463
		// (get) Token: 0x060011B7 RID: 4535 RVA: 0x0003AAA0 File Offset: 0x00038CA0
		private Team _playerTeam
		{
			get
			{
				if (!GameNetwork.IsMyPeerReady)
				{
					return null;
				}
				MissionPeer component = GameNetwork.MyPeer.GetComponent<MissionPeer>();
				if (component == null)
				{
					return null;
				}
				if (component == null)
				{
					return null;
				}
				if (component.Team == null || component.Team.Side == BattleSideEnum.None)
				{
					return null;
				}
				return component.Team;
			}
		}

		// Token: 0x170005B8 RID: 1464
		// (get) Token: 0x060011B8 RID: 4536 RVA: 0x0003AAE9 File Offset: 0x00038CE9
		// (set) Token: 0x060011B9 RID: 4537 RVA: 0x0003AAF1 File Offset: 0x00038CF1
		[DataSourceProperty]
		public bool IsOrderActive
		{
			get
			{
				return this._isOrderActive;
			}
			set
			{
				if (value != this._isOrderActive)
				{
					this._isOrderActive = value;
					base.OnPropertyChangedWithValue(value, "IsOrderActive");
				}
			}
		}

		// Token: 0x170005B9 RID: 1465
		// (get) Token: 0x060011BA RID: 4538 RVA: 0x0003AB0F File Offset: 0x00038D0F
		// (set) Token: 0x060011BB RID: 4539 RVA: 0x0003AB17 File Offset: 0x00038D17
		[DataSourceProperty]
		public CommanderInfoVM CommanderInfo
		{
			get
			{
				return this._commanderInfo;
			}
			set
			{
				if (value != this._commanderInfo)
				{
					this._commanderInfo = value;
					base.OnPropertyChangedWithValue<CommanderInfoVM>(value, "CommanderInfo");
				}
			}
		}

		// Token: 0x170005BA RID: 1466
		// (get) Token: 0x060011BC RID: 4540 RVA: 0x0003AB35 File Offset: 0x00038D35
		// (set) Token: 0x060011BD RID: 4541 RVA: 0x0003AB3D File Offset: 0x00038D3D
		[DataSourceProperty]
		public MissionMultiplayerSpectatorHUDVM SpectatorControls
		{
			get
			{
				return this._spectatorControls;
			}
			set
			{
				if (value != this._spectatorControls)
				{
					this._spectatorControls = value;
					base.OnPropertyChangedWithValue<MissionMultiplayerSpectatorHUDVM>(value, "SpectatorControls");
				}
			}
		}

		// Token: 0x170005BB RID: 1467
		// (get) Token: 0x060011BE RID: 4542 RVA: 0x0003AB5B File Offset: 0x00038D5B
		// (set) Token: 0x060011BF RID: 4543 RVA: 0x0003AB63 File Offset: 0x00038D63
		[DataSourceProperty]
		public MBBindingList<MPPlayerVM> Teammates
		{
			get
			{
				return this._teammatesList;
			}
			set
			{
				if (value != this._teammatesList)
				{
					this._teammatesList = value;
					base.OnPropertyChangedWithValue<MBBindingList<MPPlayerVM>>(value, "Teammates");
				}
			}
		}

		// Token: 0x170005BC RID: 1468
		// (get) Token: 0x060011C0 RID: 4544 RVA: 0x0003AB81 File Offset: 0x00038D81
		// (set) Token: 0x060011C1 RID: 4545 RVA: 0x0003AB89 File Offset: 0x00038D89
		[DataSourceProperty]
		public MBBindingList<MPPlayerVM> Enemies
		{
			get
			{
				return this._enemiesList;
			}
			set
			{
				if (value != this._enemiesList)
				{
					this._enemiesList = value;
					base.OnPropertyChangedWithValue<MBBindingList<MPPlayerVM>>(value, "Enemies");
				}
			}
		}

		// Token: 0x170005BD RID: 1469
		// (get) Token: 0x060011C2 RID: 4546 RVA: 0x0003ABA7 File Offset: 0x00038DA7
		// (set) Token: 0x060011C3 RID: 4547 RVA: 0x0003ABAF File Offset: 0x00038DAF
		[DataSourceProperty]
		public ImageIdentifierVM AllyBanner
		{
			get
			{
				return this._defenderBanner;
			}
			set
			{
				if (value != this._defenderBanner)
				{
					this._defenderBanner = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "AllyBanner");
				}
			}
		}

		// Token: 0x170005BE RID: 1470
		// (get) Token: 0x060011C4 RID: 4548 RVA: 0x0003ABCD File Offset: 0x00038DCD
		// (set) Token: 0x060011C5 RID: 4549 RVA: 0x0003ABD5 File Offset: 0x00038DD5
		[DataSourceProperty]
		public ImageIdentifierVM EnemyBanner
		{
			get
			{
				return this._attackerBanner;
			}
			set
			{
				if (value != this._attackerBanner)
				{
					this._attackerBanner = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "EnemyBanner");
				}
			}
		}

		// Token: 0x170005BF RID: 1471
		// (get) Token: 0x060011C6 RID: 4550 RVA: 0x0003ABF3 File Offset: 0x00038DF3
		// (set) Token: 0x060011C7 RID: 4551 RVA: 0x0003ABFB File Offset: 0x00038DFB
		[DataSourceProperty]
		public bool IsRoundCountdownAvailable
		{
			get
			{
				return this._isRoundCountdownAvailable;
			}
			set
			{
				if (value != this._isRoundCountdownAvailable)
				{
					this._isRoundCountdownAvailable = value;
					base.OnPropertyChangedWithValue(value, "IsRoundCountdownAvailable");
				}
			}
		}

		// Token: 0x170005C0 RID: 1472
		// (get) Token: 0x060011C8 RID: 4552 RVA: 0x0003AC19 File Offset: 0x00038E19
		// (set) Token: 0x060011C9 RID: 4553 RVA: 0x0003AC21 File Offset: 0x00038E21
		[DataSourceProperty]
		public bool IsRoundCountdownSuspended
		{
			get
			{
				return this._isRoundCountdownSuspended;
			}
			set
			{
				if (value != this._isRoundCountdownSuspended)
				{
					this._isRoundCountdownSuspended = value;
					base.OnPropertyChangedWithValue(value, "IsRoundCountdownSuspended");
				}
			}
		}

		// Token: 0x170005C1 RID: 1473
		// (get) Token: 0x060011CA RID: 4554 RVA: 0x0003AC3F File Offset: 0x00038E3F
		// (set) Token: 0x060011CB RID: 4555 RVA: 0x0003AC47 File Offset: 0x00038E47
		[DataSourceProperty]
		public bool ShowTeamScores
		{
			get
			{
				return this._showTeamScores;
			}
			set
			{
				if (value != this._showTeamScores)
				{
					this._showTeamScores = value;
					base.OnPropertyChangedWithValue(value, "ShowTeamScores");
				}
			}
		}

		// Token: 0x170005C2 RID: 1474
		// (get) Token: 0x060011CC RID: 4556 RVA: 0x0003AC65 File Offset: 0x00038E65
		// (set) Token: 0x060011CD RID: 4557 RVA: 0x0003AC6D File Offset: 0x00038E6D
		[DataSourceProperty]
		public string RemainingRoundTime
		{
			get
			{
				return this._remainingRoundTime;
			}
			set
			{
				if (value != this._remainingRoundTime)
				{
					this._remainingRoundTime = value;
					base.OnPropertyChangedWithValue<string>(value, "RemainingRoundTime");
				}
			}
		}

		// Token: 0x170005C3 RID: 1475
		// (get) Token: 0x060011CE RID: 4558 RVA: 0x0003AC90 File Offset: 0x00038E90
		// (set) Token: 0x060011CF RID: 4559 RVA: 0x0003AC98 File Offset: 0x00038E98
		[DataSourceProperty]
		public bool WarnRemainingTime
		{
			get
			{
				return this._warnRemainingTime;
			}
			set
			{
				if (value != this._warnRemainingTime)
				{
					this._warnRemainingTime = value;
					base.OnPropertyChangedWithValue(value, "WarnRemainingTime");
				}
			}
		}

		// Token: 0x170005C4 RID: 1476
		// (get) Token: 0x060011D0 RID: 4560 RVA: 0x0003ACB6 File Offset: 0x00038EB6
		// (set) Token: 0x060011D1 RID: 4561 RVA: 0x0003ACBE File Offset: 0x00038EBE
		[DataSourceProperty]
		public int AllyTeamScore
		{
			get
			{
				return this._allyTeamScore;
			}
			set
			{
				if (value != this._allyTeamScore)
				{
					this._allyTeamScore = value;
					base.OnPropertyChangedWithValue(value, "AllyTeamScore");
				}
			}
		}

		// Token: 0x170005C5 RID: 1477
		// (get) Token: 0x060011D2 RID: 4562 RVA: 0x0003ACDC File Offset: 0x00038EDC
		// (set) Token: 0x060011D3 RID: 4563 RVA: 0x0003ACE4 File Offset: 0x00038EE4
		[DataSourceProperty]
		public int EnemyTeamScore
		{
			get
			{
				return this._enemyTeamScore;
			}
			set
			{
				if (value != this._enemyTeamScore)
				{
					this._enemyTeamScore = value;
					base.OnPropertyChangedWithValue(value, "EnemyTeamScore");
				}
			}
		}

		// Token: 0x170005C6 RID: 1478
		// (get) Token: 0x060011D4 RID: 4564 RVA: 0x0003AD02 File Offset: 0x00038F02
		// (set) Token: 0x060011D5 RID: 4565 RVA: 0x0003AD0A File Offset: 0x00038F0A
		[DataSourceProperty]
		public string AllyTeamColor
		{
			get
			{
				return this._allyTeamColor;
			}
			set
			{
				if (value != this._allyTeamColor)
				{
					this._allyTeamColor = value;
					base.OnPropertyChangedWithValue<string>(value, "AllyTeamColor");
				}
			}
		}

		// Token: 0x170005C7 RID: 1479
		// (get) Token: 0x060011D6 RID: 4566 RVA: 0x0003AD2D File Offset: 0x00038F2D
		// (set) Token: 0x060011D7 RID: 4567 RVA: 0x0003AD35 File Offset: 0x00038F35
		[DataSourceProperty]
		public string AllyTeamColor2
		{
			get
			{
				return this._allyTeamColor2;
			}
			set
			{
				if (value != this._allyTeamColor2)
				{
					this._allyTeamColor2 = value;
					base.OnPropertyChangedWithValue<string>(value, "AllyTeamColor2");
				}
			}
		}

		// Token: 0x170005C8 RID: 1480
		// (get) Token: 0x060011D8 RID: 4568 RVA: 0x0003AD58 File Offset: 0x00038F58
		// (set) Token: 0x060011D9 RID: 4569 RVA: 0x0003AD60 File Offset: 0x00038F60
		[DataSourceProperty]
		public string EnemyTeamColor
		{
			get
			{
				return this._enemyTeamColor;
			}
			set
			{
				if (value != this._enemyTeamColor)
				{
					this._enemyTeamColor = value;
					base.OnPropertyChangedWithValue<string>(value, "EnemyTeamColor");
				}
			}
		}

		// Token: 0x170005C9 RID: 1481
		// (get) Token: 0x060011DA RID: 4570 RVA: 0x0003AD83 File Offset: 0x00038F83
		// (set) Token: 0x060011DB RID: 4571 RVA: 0x0003AD8B File Offset: 0x00038F8B
		[DataSourceProperty]
		public string EnemyTeamColor2
		{
			get
			{
				return this._enemyTeamColor2;
			}
			set
			{
				if (value != this._enemyTeamColor2)
				{
					this._enemyTeamColor2 = value;
					base.OnPropertyChangedWithValue<string>(value, "EnemyTeamColor2");
				}
			}
		}

		// Token: 0x170005CA RID: 1482
		// (get) Token: 0x060011DC RID: 4572 RVA: 0x0003ADAE File Offset: 0x00038FAE
		// (set) Token: 0x060011DD RID: 4573 RVA: 0x0003ADB6 File Offset: 0x00038FB6
		[DataSourceProperty]
		public bool ShowHud
		{
			get
			{
				return this._showHUD;
			}
			set
			{
				if (value != this._showHUD)
				{
					this._showHUD = value;
					base.OnPropertyChangedWithValue(value, "ShowHud");
				}
			}
		}

		// Token: 0x170005CB RID: 1483
		// (get) Token: 0x060011DE RID: 4574 RVA: 0x0003ADD4 File Offset: 0x00038FD4
		// (set) Token: 0x060011DF RID: 4575 RVA: 0x0003ADDC File Offset: 0x00038FDC
		[DataSourceProperty]
		public bool ShowCommanderInfo
		{
			get
			{
				return this._showCommanderInfo;
			}
			set
			{
				if (value != this._showCommanderInfo)
				{
					this._showCommanderInfo = value;
					base.OnPropertyChangedWithValue(value, "ShowCommanderInfo");
					this.UpdateShowTeamScores();
				}
			}
		}

		// Token: 0x170005CC RID: 1484
		// (get) Token: 0x060011E0 RID: 4576 RVA: 0x0003AE00 File Offset: 0x00039000
		// (set) Token: 0x060011E1 RID: 4577 RVA: 0x0003AE08 File Offset: 0x00039008
		[DataSourceProperty]
		public bool ShowPowerLevels
		{
			get
			{
				return this._showPowerLevels;
			}
			set
			{
				if (value != this._showPowerLevels)
				{
					this._showPowerLevels = value;
					base.OnPropertyChangedWithValue(value, "ShowPowerLevels");
				}
			}
		}

		// Token: 0x170005CD RID: 1485
		// (get) Token: 0x060011E2 RID: 4578 RVA: 0x0003AE26 File Offset: 0x00039026
		// (set) Token: 0x060011E3 RID: 4579 RVA: 0x0003AE2E File Offset: 0x0003902E
		[DataSourceProperty]
		public bool IsInWarmup
		{
			get
			{
				return this._isInWarmup;
			}
			set
			{
				if (value != this._isInWarmup)
				{
					this._isInWarmup = value;
					base.OnPropertyChangedWithValue(value, "IsInWarmup");
					this.UpdateShowTeamScores();
					CommanderInfoVM commanderInfo = this.CommanderInfo;
					if (commanderInfo == null)
					{
						return;
					}
					commanderInfo.UpdateWarmupDependentFlags(this._isInWarmup);
				}
			}
		}

		// Token: 0x170005CE RID: 1486
		// (get) Token: 0x060011E4 RID: 4580 RVA: 0x0003AE68 File Offset: 0x00039068
		// (set) Token: 0x060011E5 RID: 4581 RVA: 0x0003AE70 File Offset: 0x00039070
		[DataSourceProperty]
		public string WarmupInfoText
		{
			get
			{
				return this._warmupInfoText;
			}
			set
			{
				if (value != this._warmupInfoText)
				{
					this._warmupInfoText = value;
					base.OnPropertyChangedWithValue<string>(value, "WarmupInfoText");
				}
			}
		}

		// Token: 0x170005CF RID: 1487
		// (get) Token: 0x060011E6 RID: 4582 RVA: 0x0003AE93 File Offset: 0x00039093
		// (set) Token: 0x060011E7 RID: 4583 RVA: 0x0003AE9B File Offset: 0x0003909B
		[DataSourceProperty]
		public int GeneralWarningCountdown
		{
			get
			{
				return this._generalWarningCountdown;
			}
			set
			{
				if (value != this._generalWarningCountdown)
				{
					this._generalWarningCountdown = value;
					base.OnPropertyChangedWithValue(value, "GeneralWarningCountdown");
				}
			}
		}

		// Token: 0x170005D0 RID: 1488
		// (get) Token: 0x060011E8 RID: 4584 RVA: 0x0003AEB9 File Offset: 0x000390B9
		// (set) Token: 0x060011E9 RID: 4585 RVA: 0x0003AEC1 File Offset: 0x000390C1
		[DataSourceProperty]
		public bool IsGeneralWarningCountdownActive
		{
			get
			{
				return this._isGeneralWarningCountdownActive;
			}
			set
			{
				if (value != this._isGeneralWarningCountdownActive)
				{
					this._isGeneralWarningCountdownActive = value;
					base.OnPropertyChangedWithValue(value, "IsGeneralWarningCountdownActive");
				}
			}
		}

		// Token: 0x04000866 RID: 2150
		private const float RemainingTimeWarningThreshold = 5f;

		// Token: 0x04000867 RID: 2151
		private readonly Mission _mission;

		// Token: 0x04000868 RID: 2152
		private readonly Dictionary<MissionPeer, MPPlayerVM> _teammateDictionary;

		// Token: 0x04000869 RID: 2153
		private readonly Dictionary<MissionPeer, MPPlayerVM> _enemyDictionary;

		// Token: 0x0400086A RID: 2154
		private readonly MissionScoreboardComponent _missionScoreboardComponent;

		// Token: 0x0400086B RID: 2155
		private readonly MissionLobbyEquipmentNetworkComponent _missionLobbyEquipmentNetworkComponent;

		// Token: 0x0400086C RID: 2156
		private readonly MissionMultiplayerGameModeBaseClient _gameMode;

		// Token: 0x0400086D RID: 2157
		private readonly bool _isTeamsEnabled;

		// Token: 0x0400086E RID: 2158
		private bool _isAttackerTeamAlly;

		// Token: 0x0400086F RID: 2159
		private bool _isTeammateAndEnemiesRelevant;

		// Token: 0x04000870 RID: 2160
		private bool _isTeamScoresEnabled;

		// Token: 0x04000871 RID: 2161
		private bool _isTeamMemberCountsEnabled;

		// Token: 0x04000872 RID: 2162
		private bool _isOrderActive;

		// Token: 0x04000873 RID: 2163
		private CommanderInfoVM _commanderInfo;

		// Token: 0x04000874 RID: 2164
		private MissionMultiplayerSpectatorHUDVM _spectatorControls;

		// Token: 0x04000875 RID: 2165
		private bool _warnRemainingTime;

		// Token: 0x04000876 RID: 2166
		private bool _isRoundCountdownAvailable;

		// Token: 0x04000877 RID: 2167
		private bool _isRoundCountdownSuspended;

		// Token: 0x04000878 RID: 2168
		private bool _showTeamScores;

		// Token: 0x04000879 RID: 2169
		private string _remainingRoundTime;

		// Token: 0x0400087A RID: 2170
		private string _allyTeamColor;

		// Token: 0x0400087B RID: 2171
		private string _allyTeamColor2;

		// Token: 0x0400087C RID: 2172
		private string _enemyTeamColor;

		// Token: 0x0400087D RID: 2173
		private string _enemyTeamColor2;

		// Token: 0x0400087E RID: 2174
		private string _warmupInfoText;

		// Token: 0x0400087F RID: 2175
		private int _allyTeamScore = -1;

		// Token: 0x04000880 RID: 2176
		private int _enemyTeamScore = -1;

		// Token: 0x04000881 RID: 2177
		private MBBindingList<MPPlayerVM> _teammatesList;

		// Token: 0x04000882 RID: 2178
		private MBBindingList<MPPlayerVM> _enemiesList;

		// Token: 0x04000883 RID: 2179
		private bool _showHUD;

		// Token: 0x04000884 RID: 2180
		private bool _showCommanderInfo;

		// Token: 0x04000885 RID: 2181
		private bool _showPowerLevels;

		// Token: 0x04000886 RID: 2182
		private bool _isInWarmup;

		// Token: 0x04000887 RID: 2183
		private int _generalWarningCountdown;

		// Token: 0x04000888 RID: 2184
		private bool _isGeneralWarningCountdownActive;

		// Token: 0x04000889 RID: 2185
		private ImageIdentifierVM _defenderBanner;

		// Token: 0x0400088A RID: 2186
		private ImageIdentifierVM _attackerBanner;
	}
}
