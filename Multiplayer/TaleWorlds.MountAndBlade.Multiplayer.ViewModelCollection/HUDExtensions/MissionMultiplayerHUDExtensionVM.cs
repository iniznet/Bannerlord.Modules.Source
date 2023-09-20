using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.HUDExtensions
{
	public class MissionMultiplayerHUDExtensionVM : ViewModel
	{
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
			this._missionScoreboardComponent.OnRoundPropertiesChanged += this.SetTeamScoresDirty;
			MissionPeer.OnTeamChanged += new MissionPeer.OnTeamChangedDelegate(this.OnTeamChanged);
			NetworkCommunicator.OnPeerComponentAdded += this.OnPeerComponentAdded;
			Mission.Current.OnMissionReset += this.OnMissionReset;
			MissionLobbyComponent missionBehavior = mission.GetMissionBehavior<MissionLobbyComponent>();
			this._isTeamsEnabled = missionBehavior.MissionType != null && missionBehavior.MissionType != 2;
			this._missionLobbyEquipmentNetworkComponent = mission.GetMissionBehavior<MissionLobbyEquipmentNetworkComponent>();
			this.IsRoundCountdownAvailable = this._gameMode.IsGameModeUsingRoundCountdown;
			this.IsRoundCountdownSuspended = false;
			this._isTeamScoresEnabled = this._isTeamsEnabled;
			this.UpdateShowTeamScores();
			this.Teammates = new MBBindingList<MPPlayerVM>();
			this.Enemies = new MBBindingList<MPPlayerVM>();
			this._teammateDictionary = new Dictionary<MissionPeer, MPPlayerVM>();
			this._enemyDictionary = new Dictionary<MissionPeer, MPPlayerVM>();
			this.ShowHud = true;
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			string strValue = MultiplayerOptionsExtensions.GetStrValue(11, 0);
			TextObject textObject = new TextObject("{=XJTX8w8M}Warmup Phase - {GAME_MODE}\nWaiting for players to join", null);
			textObject.SetTextVariable("GAME_MODE", GameTexts.FindText("str_multiplayer_official_game_type_name", strValue));
			this.WarmupInfoText = textObject.ToString();
			this.SpectatorControls.RefreshValues();
		}

		private void OnMissionReset(object sender, PropertyChangedEventArgs e)
		{
			this.IsGeneralWarningCountdownActive = false;
		}

		private void OnPeerComponentAdded(PeerComponent component)
		{
			if (component.IsMine && component is MissionRepresentativeBase)
			{
				NetworkCommunicator myPeer = GameNetwork.MyPeer;
				MissionRepresentativeBase missionRepresentativeBase = ((myPeer != null) ? myPeer.VirtualPlayer.GetComponent<MissionRepresentativeBase>() : null);
				this.AllyTeamScore = this._missionScoreboardComponent.GetRoundScore(1);
				this.EnemyTeamScore = this._missionScoreboardComponent.GetRoundScore(0);
				this._isTeammateAndEnemiesRelevant = Mission.Current.GetMissionBehavior<MissionMultiplayerGameModeBaseClient>().IsGameModeTactical && !Mission.Current.HasMissionBehavior<MissionMultiplayerSiegeClient>() && this._gameMode.GameType != 4;
				this.CommanderInfo = new CommanderInfoVM(missionRepresentativeBase);
				this.ShowCommanderInfo = true;
				if (this._isTeammateAndEnemiesRelevant)
				{
					this.OnRefreshTeamMembers();
					this.OnRefreshEnemyMembers();
				}
				this.ShowPowerLevels = this._gameMode.GameType == 4;
			}
		}

		public override void OnFinalize()
		{
			MissionPeer.OnTeamChanged -= new MissionPeer.OnTeamChangedDelegate(this.OnTeamChanged);
			if (this._gameMode.RoundComponent != null)
			{
				this._gameMode.RoundComponent.OnCurrentRoundStateChanged -= this.OnCurrentGameModeStateChanged;
			}
			if (this._gameMode.WarmupComponent != null)
			{
				this._gameMode.WarmupComponent.OnWarmupEnded -= this.OnCurrentGameModeStateChanged;
			}
			this._missionScoreboardComponent.OnRoundPropertiesChanged -= this.SetTeamScoresDirty;
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

		public void Tick(float dt)
		{
			this.IsInWarmup = this._gameMode.IsInWarmup;
			this.CheckTimers(false);
			if (this._isTeammateAndEnemiesRelevant)
			{
				this.OnRefreshTeamMembers();
				this.OnRefreshEnemyMembers();
			}
			if (this._isTeamScoresDirty)
			{
				this.UpdateTeamScores();
				this._isTeamScoresDirty = false;
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

		private void CheckTimers(bool forceUpdate = false)
		{
			int num;
			int num2;
			if (this._gameMode.CheckTimer(ref num, ref num2, forceUpdate))
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

		private void OnToggleLoadout(bool isActive)
		{
			this.ShowHud = !isActive;
		}

		public void OnSpectatedAgentFocusIn(Agent followedAgent)
		{
			MissionMultiplayerSpectatorHUDVM spectatorControls = this._spectatorControls;
			if (spectatorControls == null)
			{
				return;
			}
			spectatorControls.OnSpectatedAgentFocusIn(followedAgent);
		}

		public void OnSpectatedAgentFocusOut(Agent followedPeer)
		{
			MissionMultiplayerSpectatorHUDVM spectatorControls = this._spectatorControls;
			if (spectatorControls == null)
			{
				return;
			}
			spectatorControls.OnSpectatedAgentFocusOut(followedPeer);
		}

		private void OnCurrentGameModeStateChanged()
		{
			this.CheckTimers(true);
		}

		private void SetTeamScoresDirty()
		{
			this._isTeamScoresDirty = true;
		}

		private void UpdateTeamScores()
		{
			if (this._isTeamScoresEnabled)
			{
				int roundScore = this._missionScoreboardComponent.GetRoundScore(1);
				int roundScore2 = this._missionScoreboardComponent.GetRoundScore(0);
				this.AllyTeamScore = (this._isAttackerTeamAlly ? roundScore : roundScore2);
				this.EnemyTeamScore = (this._isAttackerTeamAlly ? roundScore2 : roundScore);
			}
		}

		private void UpdateTeamBanners()
		{
			Team attackerTeam = Mission.Current.AttackerTeam;
			ImageIdentifierVM imageIdentifierVM = new ImageIdentifierVM(BannerCode.CreateFrom((attackerTeam != null) ? attackerTeam.Banner : null), true);
			Team defenderTeam = Mission.Current.DefenderTeam;
			ImageIdentifierVM imageIdentifierVM2 = new ImageIdentifierVM(BannerCode.CreateFrom((defenderTeam != null) ? defenderTeam.Banner : null), true);
			this.AllyBanner = (this._isAttackerTeamAlly ? imageIdentifierVM : imageIdentifierVM2);
			this.EnemyBanner = (this._isAttackerTeamAlly ? imageIdentifierVM2 : imageIdentifierVM);
		}

		private void OnTeamChanged(NetworkCommunicator peer, Team previousTeam, Team newTeam)
		{
			if (peer.IsMine)
			{
				if (this._isTeamScoresEnabled || this._gameMode.GameType == 4)
				{
					this._isAttackerTeamAlly = newTeam.Side == 1;
					this.SetTeamScoresDirty();
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
			MPPlayerVM mpplayerVM = this.Teammates.SingleOrDefault((MPPlayerVM x) => PeerExtensions.GetNetworkPeer(x.Peer) == peer);
			if (mpplayerVM != null)
			{
				mpplayerVM.RefreshTeam();
			}
			string text;
			string text2;
			this.GetTeamColors(Mission.Current.AttackerTeam, out text, out text2);
			if (this._isTeamScoresEnabled || this._gameMode.GameType == 4)
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

		private void GetTeamColors(Team team, out string color, out string color2)
		{
			color = team.Color.ToString("X");
			color = color.Remove(0, 2);
			color = "#" + color + "FF";
			color2 = team.Color2.ToString("X");
			color2 = color2.Remove(0, 2);
			color2 = "#" + color2 + "FF";
		}

		private void OnRefreshTeamMembers()
		{
			List<MPPlayerVM> list = this.Teammates.ToList<MPPlayerVM>();
			foreach (MissionPeer missionPeer in VirtualPlayer.Peers<MissionPeer>())
			{
				if (PeerExtensions.GetComponent<MissionPeer>(PeerExtensions.GetNetworkPeer(missionPeer)) != null && this._playerTeam != null && missionPeer.Team != null && missionPeer.Team == this._playerTeam)
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

		private void OnRefreshEnemyMembers()
		{
			List<MPPlayerVM> list = this.Enemies.ToList<MPPlayerVM>();
			foreach (MissionPeer missionPeer in VirtualPlayer.Peers<MissionPeer>())
			{
				if (PeerExtensions.GetComponent<MissionPeer>(PeerExtensions.GetNetworkPeer(missionPeer)) != null && this._playerTeam != null && missionPeer.Team != null && missionPeer.Team != this._playerTeam && missionPeer.Team != Mission.Current.SpectatorTeam)
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

		private void UpdateShowTeamScores()
		{
			this.ShowTeamScores = !this._gameMode.IsInWarmup && this.ShowCommanderInfo && this._gameMode.GameType != 3;
		}

		private Team _playerTeam
		{
			get
			{
				if (!GameNetwork.IsMyPeerReady)
				{
					return null;
				}
				MissionPeer component = PeerExtensions.GetComponent<MissionPeer>(GameNetwork.MyPeer);
				if (component == null)
				{
					return null;
				}
				if (component == null)
				{
					return null;
				}
				if (component.Team == null || component.Team.Side == -1)
				{
					return null;
				}
				return component.Team;
			}
		}

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

		private const float RemainingTimeWarningThreshold = 5f;

		private readonly Mission _mission;

		private readonly Dictionary<MissionPeer, MPPlayerVM> _teammateDictionary;

		private readonly Dictionary<MissionPeer, MPPlayerVM> _enemyDictionary;

		private readonly MissionScoreboardComponent _missionScoreboardComponent;

		private readonly MissionLobbyEquipmentNetworkComponent _missionLobbyEquipmentNetworkComponent;

		private readonly MissionMultiplayerGameModeBaseClient _gameMode;

		private readonly bool _isTeamsEnabled;

		private bool _isAttackerTeamAlly;

		private bool _isTeammateAndEnemiesRelevant;

		private bool _isTeamScoresEnabled;

		private bool _isTeamScoresDirty;

		private bool _isOrderActive;

		private CommanderInfoVM _commanderInfo;

		private MissionMultiplayerSpectatorHUDVM _spectatorControls;

		private bool _warnRemainingTime;

		private bool _isRoundCountdownAvailable;

		private bool _isRoundCountdownSuspended;

		private bool _showTeamScores;

		private string _remainingRoundTime;

		private string _allyTeamColor;

		private string _allyTeamColor2;

		private string _enemyTeamColor;

		private string _enemyTeamColor2;

		private string _warmupInfoText;

		private int _allyTeamScore = -1;

		private int _enemyTeamScore = -1;

		private MBBindingList<MPPlayerVM> _teammatesList;

		private MBBindingList<MPPlayerVM> _enemiesList;

		private bool _showHUD;

		private bool _showCommanderInfo;

		private bool _showPowerLevels;

		private bool _isInWarmup;

		private int _generalWarningCountdown;

		private bool _isGeneralWarningCountdownActive;

		private ImageIdentifierVM _defenderBanner;

		private ImageIdentifierVM _attackerBanner;
	}
}
