using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.ClassLoadout
{
	public class MultiplayerClassLoadoutVM : ViewModel
	{
		private MissionRepresentativeBase missionRep
		{
			get
			{
				NetworkCommunicator myPeer = GameNetwork.MyPeer;
				if (myPeer == null)
				{
					return null;
				}
				VirtualPlayer virtualPlayer = myPeer.VirtualPlayer;
				if (virtualPlayer == null)
				{
					return null;
				}
				return virtualPlayer.GetComponent<MissionRepresentativeBase>();
			}
		}

		private Team _playerTeam
		{
			get
			{
				if (!GameNetwork.IsMyPeerReady)
				{
					return null;
				}
				MissionPeer component = GameNetwork.MyPeer.GetComponent<MissionPeer>();
				if (component.Team == null || component.Team.Side == BattleSideEnum.None)
				{
					return null;
				}
				return component.Team;
			}
		}

		public MultiplayerClassLoadoutVM(MissionMultiplayerGameModeBaseClient gameMode, Action<MultiplayerClassDivisions.MPHeroClass> onRefreshSelection, MultiplayerClassDivisions.MPHeroClass initialHeroSelection)
		{
			MBTextManager.SetTextVariable("newline", "\n", false);
			this._isInitializing = true;
			this._onRefreshSelection = onRefreshSelection;
			this._missionMultiplayerGameMode = gameMode;
			this._mission = gameMode.Mission;
			Team team = GameNetwork.MyPeer.GetComponent<MissionPeer>().Team;
			this.Classes = new MBBindingList<HeroClassGroupVM>();
			this.HeroInformation = new HeroInformationVM();
			this._enemyDictionary = new Dictionary<MissionPeer, MPPlayerVM>();
			this._missionLobbyEquipmentNetworkComponent = Mission.Current.GetMissionBehavior<MissionLobbyEquipmentNetworkComponent>();
			this.IsGoldEnabled = this._missionMultiplayerGameMode.IsGameModeUsingGold;
			if (this.IsGoldEnabled)
			{
				this.Gold = this._missionMultiplayerGameMode.GetGoldAmount();
			}
			HeroClassVM heroClassVM = null;
			this.UseSecondary = team.Side == BattleSideEnum.Defender;
			foreach (MultiplayerClassDivisions.MPHeroClassGroup mpheroClassGroup in MultiplayerClassDivisions.MultiplayerHeroClassGroups)
			{
				HeroClassGroupVM heroClassGroupVM = new HeroClassGroupVM(new Action<HeroClassVM>(this.RefreshCharacter), new Action<HeroPerkVM, MPPerkVM>(this.OnSelectPerk), mpheroClassGroup, this.UseSecondary);
				if (heroClassGroupVM.IsValid)
				{
					this.Classes.Add(heroClassGroupVM);
				}
			}
			int num = ((initialHeroSelection != null) ? ((!gameMode.IsGameModeUsingCasualGold) ? ((gameMode.GameType == MissionLobbyComponent.MultiplayerGameType.Battle) ? initialHeroSelection.TroopBattleCost : initialHeroSelection.TroopCost) : initialHeroSelection.TroopCasualCost) : 0);
			if (initialHeroSelection == null || (this.IsGoldEnabled && num > this.Gold))
			{
				HeroClassGroupVM heroClassGroupVM2 = this.Classes.FirstOrDefault<HeroClassGroupVM>();
				heroClassVM = ((heroClassGroupVM2 != null) ? heroClassGroupVM2.SubClasses.FirstOrDefault<HeroClassVM>() : null);
			}
			else
			{
				foreach (HeroClassGroupVM heroClassGroupVM3 in this.Classes)
				{
					foreach (HeroClassVM heroClassVM2 in heroClassGroupVM3.SubClasses)
					{
						if (heroClassVM2.HeroClass == initialHeroSelection)
						{
							heroClassVM = heroClassVM2;
							break;
						}
					}
					if (heroClassVM != null)
					{
						break;
					}
				}
				if (heroClassVM == null)
				{
					HeroClassGroupVM heroClassGroupVM4 = this.Classes.FirstOrDefault<HeroClassGroupVM>();
					heroClassVM = ((heroClassGroupVM4 != null) ? heroClassGroupVM4.SubClasses.FirstOrDefault<HeroClassVM>() : null);
				}
			}
			this._isInitializing = false;
			this.RefreshCharacter(heroClassVM);
			this._teammateDictionary = new Dictionary<MissionPeer, MPPlayerVM>();
			this.Teammates = new MBBindingList<MPPlayerVM>();
			this.Enemies = new MBBindingList<MPPlayerVM>();
			MissionPeer.OnEquipmentIndexRefreshed += this.RefreshPeerDivision;
			MissionPeer.OnPerkSelectionUpdated += this.RefreshPeerPerkSelection;
			NetworkCommunicator.OnPeerComponentAdded += this.OnPeerComponentAdded;
			this.CultureId = GameNetwork.MyPeer.GetComponent<MissionPeer>().Culture.StringId;
			if (Mission.Current.HasMissionBehavior<MissionMultiplayerSiegeClient>())
			{
				this.ShowAttackerOrDefenderIcons = true;
				this.IsAttacker = team.Side == BattleSideEnum.Attacker;
			}
			this.RefreshValues();
			this._isTeammateAndEnemiesRelevant = Mission.Current.GetMissionBehavior<MissionMultiplayerGameModeBaseClient>().IsGameModeTactical && !Mission.Current.HasMissionBehavior<MissionMultiplayerSiegeClient>() && Mission.Current.GetMissionBehavior<MissionMultiplayerGameModeBaseClient>().GameType != MissionLobbyComponent.MultiplayerGameType.Battle;
			if (this._isTeammateAndEnemiesRelevant)
			{
				this.OnRefreshTeamMembers();
				this.OnRefreshEnemyMembers();
			}
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.UpdateSpawnAndTimerLabels();
			string strValue = MultiplayerOptions.OptionType.GameType.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
			TextObject textObject = new TextObject("{=XJTX8w8M}Warmup Phase - {GAME_MODE}\nWaiting for players to join", null);
			textObject.SetTextVariable("GAME_MODE", GameTexts.FindText("str_multiplayer_official_game_type_name", strValue));
			this.WarmupInfoText = textObject.ToString();
			BasicCultureObject culture = GameNetwork.MyPeer.GetComponent<MissionPeer>().Culture;
			this.Culture = culture.Name.ToString();
			this.Classes.ApplyActionOnAllItems(delegate(HeroClassGroupVM x)
			{
				x.RefreshValues();
			});
			this.CurrentSelectedClass.RefreshValues();
			this.HeroInformation.RefreshValues();
		}

		private void UpdateSpawnAndTimerLabels()
		{
			string keyHyperlinkText = HyperlinkTexts.GetKeyHyperlinkText(HotKeyManager.GetHotKeyId("CombatHotKeyCategory", 13));
			GameTexts.SetVariable("USE_KEY", keyHyperlinkText);
			this.SpawnLabelText = GameTexts.FindText("str_skirmish_battle_press_action_to_spawn", null).ToString();
			if (this._missionMultiplayerGameMode.RoundComponent != null)
			{
				if (!this._missionMultiplayerGameMode.IsInWarmup && !this._missionMultiplayerGameMode.IsRoundInProgress)
				{
					this.IsSpawnTimerVisible = true;
					return;
				}
				this.IsSpawnTimerVisible = false;
				this.IsSpawnLabelVisible = true;
				if (this._missionMultiplayerGameMode.IsRoundInProgress && (this._missionMultiplayerGameMode is MissionMultiplayerGameModeFlagDominationClient && this._missionMultiplayerGameMode.GameType == MissionLobbyComponent.MultiplayerGameType.Skirmish) && GameNetwork.MyPeer.GetComponent<MissionPeer>() != null)
				{
					this.IsSpawnForfeitLabelVisible = true;
					string keyHyperlinkText2 = HyperlinkTexts.GetKeyHyperlinkText(HotKeyManager.GetHotKeyId("CombatHotKeyCategory", "ForfeitSpawn"));
					GameTexts.SetVariable("ALT_WEAP_KEY", keyHyperlinkText2);
					this.SpawnForfeitLabelText = GameTexts.FindText("str_skirmish_battle_press_alternative_to_forfeit_spawning", null).ToString();
					return;
				}
			}
			else
			{
				this.IsSpawnTimerVisible = false;
				this.IsSpawnLabelVisible = true;
			}
		}

		public override void OnFinalize()
		{
			base.OnFinalize();
			MissionPeer.OnEquipmentIndexRefreshed -= this.RefreshPeerDivision;
			MissionPeer.OnPerkSelectionUpdated -= this.RefreshPeerPerkSelection;
			NetworkCommunicator.OnPeerComponentAdded -= this.OnPeerComponentAdded;
		}

		private void RefreshCharacter(HeroClassVM heroClass)
		{
			if (this._isInitializing)
			{
				return;
			}
			foreach (HeroClassGroupVM heroClassGroupVM in this.Classes)
			{
				foreach (HeroClassVM heroClassVM in heroClassGroupVM.SubClasses)
				{
					heroClassVM.IsSelected = false;
				}
			}
			heroClass.IsSelected = true;
			this.CurrentSelectedClass = heroClass;
			if (GameNetwork.IsMyPeerReady)
			{
				MissionPeer component = GameNetwork.MyPeer.GetComponent<MissionPeer>();
				int num = MultiplayerClassDivisions.GetMPHeroClasses(heroClass.HeroClass.Culture).ToList<MultiplayerClassDivisions.MPHeroClass>().IndexOf(heroClass.HeroClass);
				component.NextSelectedTroopIndex = num;
			}
			this.HeroInformation.RefreshWith(heroClass.HeroClass, heroClass.SelectedPerks);
			this._missionLobbyEquipmentNetworkComponent.EquipmentUpdated();
			if (this._missionMultiplayerGameMode.IsGameModeUsingGold)
			{
				this.Gold = this._missionMultiplayerGameMode.GetGoldAmount();
			}
			List<IReadOnlyPerkObject> list = heroClass.Perks.Select((HeroPerkVM x) => x.SelectedPerk).ToList<IReadOnlyPerkObject>();
			this.HeroInformation.RefreshWith(this.HeroInformation.HeroClass, list);
			List<Tuple<HeroPerkVM, MPPerkVM>> list2 = new List<Tuple<HeroPerkVM, MPPerkVM>>();
			foreach (HeroPerkVM heroPerkVM in heroClass.Perks)
			{
				list2.Add(new Tuple<HeroPerkVM, MPPerkVM>(heroPerkVM, heroPerkVM.SelectedPerkItem));
			}
			list2.ForEach(delegate(Tuple<HeroPerkVM, MPPerkVM> p)
			{
				this.OnSelectPerk(p.Item1, p.Item2);
			});
			Action<MultiplayerClassDivisions.MPHeroClass> onRefreshSelection = this._onRefreshSelection;
			if (onRefreshSelection == null)
			{
				return;
			}
			onRefreshSelection(heroClass.HeroClass);
		}

		private void OnSelectPerk(HeroPerkVM heroPerk, MPPerkVM candidate)
		{
			if (GameNetwork.IsMyPeerReady && this.HeroInformation.HeroClass != null && this.CurrentSelectedClass != null)
			{
				MissionPeer component = GameNetwork.MyPeer.GetComponent<MissionPeer>();
				if (!GameNetwork.IsServer || component.SelectPerk(heroPerk.PerkIndex, candidate.PerkIndex, -1))
				{
					this._missionLobbyEquipmentNetworkComponent.PerkUpdated(heroPerk.PerkIndex, candidate.PerkIndex);
				}
				List<IReadOnlyPerkObject> list = this.CurrentSelectedClass.Perks.Select((HeroPerkVM x) => x.SelectedPerk).ToList<IReadOnlyPerkObject>();
				if (list.Count > 0)
				{
					this.HeroInformation.RefreshWith(this.HeroInformation.HeroClass, list);
				}
			}
		}

		public void RefreshPeerDivision(MissionPeer peer, int divisionType)
		{
			MPPlayerVM mpplayerVM = this.Teammates.FirstOrDefault((MPPlayerVM t) => t.Peer == peer);
			if (mpplayerVM != null)
			{
				mpplayerVM.RefreshDivision(false);
			}
		}

		private void RefreshPeerPerkSelection(MissionPeer peer)
		{
			MPPlayerVM mpplayerVM = this.Teammates.FirstOrDefault((MPPlayerVM t) => t.Peer == peer);
			if (mpplayerVM != null)
			{
				mpplayerVM.RefreshActivePerks();
			}
		}

		public void Tick(float dt)
		{
			if (this._missionMultiplayerGameMode != null)
			{
				this.IsInWarmup = this._missionMultiplayerGameMode.IsInWarmup;
				this.IsGoldEnabled = !this.IsInWarmup && this._missionMultiplayerGameMode.IsGameModeUsingGold;
				if (this.IsGoldEnabled)
				{
					this.Gold = this._missionMultiplayerGameMode.GetGoldAmount();
				}
				foreach (HeroClassGroupVM heroClassGroupVM in this.Classes)
				{
					foreach (HeroClassVM heroClassVM in heroClassGroupVM.SubClasses)
					{
						heroClassVM.IsGoldEnabled = this.IsGoldEnabled;
					}
				}
			}
			this.RefreshRemainingTime();
			this._updateTimeElapsed += dt;
			if (this._updateTimeElapsed < 1f)
			{
				return;
			}
			this._updateTimeElapsed = 0f;
			if (this._isTeammateAndEnemiesRelevant)
			{
				this.OnRefreshTeamMembers();
				this.OnRefreshEnemyMembers();
			}
		}

		private void OnPeerComponentAdded(PeerComponent component)
		{
			if (component.IsMine && component is MissionRepresentativeBase)
			{
				this._isTeammateAndEnemiesRelevant = Mission.Current.GetMissionBehavior<MissionMultiplayerGameModeBaseClient>().IsGameModeTactical && !Mission.Current.HasMissionBehavior<MissionMultiplayerSiegeClient>();
				if (this._isTeammateAndEnemiesRelevant)
				{
					this.OnRefreshTeamMembers();
					this.OnRefreshEnemyMembers();
				}
			}
		}

		private void OnRefreshTeamMembers()
		{
			List<MPPlayerVM> list = this.Teammates.ToList<MPPlayerVM>();
			foreach (MissionPeer missionPeer in VirtualPlayer.Peers<MissionPeer>())
			{
				if (missionPeer.GetNetworkPeer().GetComponent<MissionPeer>() != null && this._playerTeam != null && missionPeer.Team == this._playerTeam)
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
				if (mpplayerVM3.CompassElement == null)
				{
					mpplayerVM3.RefreshDivision(false);
				}
			}
		}

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

		public void OnPeerEquipmentRefreshed(MissionPeer peer)
		{
			if (this._teammateDictionary.ContainsKey(peer))
			{
				this._teammateDictionary[peer].RefreshActivePerks();
				return;
			}
			if (this._enemyDictionary.ContainsKey(peer))
			{
				this._enemyDictionary[peer].RefreshActivePerks();
			}
		}

		public void OnGoldUpdated()
		{
			foreach (HeroClassGroupVM heroClassGroupVM in this.Classes)
			{
				heroClassGroupVM.SubClasses.ApplyActionOnAllItems(delegate(HeroClassVM sc)
				{
					sc.UpdateEnabled();
				});
			}
		}

		public void RefreshRemainingTime()
		{
			int num = MathF.Ceiling(this._missionMultiplayerGameMode.RemainingTime);
			this.RemainingTimeText = TimeSpan.FromSeconds((double)num).ToString("mm':'ss");
			this.WarnRemainingTime = (float)num < 5f;
		}

		[DataSourceProperty]
		public string Culture
		{
			get
			{
				return this._culture;
			}
			set
			{
				if (value != this._culture)
				{
					this._culture = value;
					base.OnPropertyChangedWithValue<string>(value, "Culture");
				}
			}
		}

		[DataSourceProperty]
		public string CultureId
		{
			get
			{
				return this._cultureId;
			}
			set
			{
				if (value != this._cultureId)
				{
					this._cultureId = value;
					base.OnPropertyChangedWithValue<string>(value, "CultureId");
				}
			}
		}

		[DataSourceProperty]
		public bool IsSpawnTimerVisible
		{
			get
			{
				return this._isSpawnTimerVisible;
			}
			set
			{
				if (value != this._isSpawnTimerVisible)
				{
					this._isSpawnTimerVisible = value;
					base.OnPropertyChangedWithValue(value, "IsSpawnTimerVisible");
				}
			}
		}

		[DataSourceProperty]
		public string SpawnLabelText
		{
			get
			{
				return this._spawnLabelText;
			}
			set
			{
				if (value != this._spawnLabelText)
				{
					this._spawnLabelText = value;
					base.OnPropertyChangedWithValue<string>(value, "SpawnLabelText");
				}
			}
		}

		[DataSourceProperty]
		public bool IsSpawnLabelVisible
		{
			get
			{
				return this._isSpawnLabelVisible;
			}
			set
			{
				if (value != this._isSpawnLabelVisible)
				{
					this._isSpawnLabelVisible = value;
					base.OnPropertyChangedWithValue(value, "IsSpawnLabelVisible");
				}
			}
		}

		[DataSourceProperty]
		public bool UseSecondary
		{
			get
			{
				return this._useSecondary;
			}
			set
			{
				if (value != this._useSecondary)
				{
					this._useSecondary = value;
					base.OnPropertyChangedWithValue(value, "UseSecondary");
				}
			}
		}

		[DataSourceProperty]
		public bool ShowAttackerOrDefenderIcons
		{
			get
			{
				return this._showAttackerOrDefenderIcons;
			}
			set
			{
				if (value != this._showAttackerOrDefenderIcons)
				{
					this._showAttackerOrDefenderIcons = value;
					base.OnPropertyChangedWithValue(value, "ShowAttackerOrDefenderIcons");
				}
			}
		}

		[DataSourceProperty]
		public bool IsAttacker
		{
			get
			{
				return this._isAttacker;
			}
			set
			{
				if (value != this._isAttacker)
				{
					this._isAttacker = value;
					base.OnPropertyChangedWithValue(value, "IsAttacker");
				}
			}
		}

		[DataSourceProperty]
		public string SpawnForfeitLabelText
		{
			get
			{
				return this._spawnForfeitLabelText;
			}
			set
			{
				if (value != this._spawnForfeitLabelText)
				{
					this._spawnForfeitLabelText = value;
					base.OnPropertyChangedWithValue<string>(value, "SpawnForfeitLabelText");
				}
			}
		}

		[DataSourceProperty]
		public bool IsSpawnForfeitLabelVisible
		{
			get
			{
				return this._isSpawnForfeitLabelVisible;
			}
			set
			{
				if (value != this._isSpawnForfeitLabelVisible)
				{
					this._isSpawnForfeitLabelVisible = value;
					base.OnPropertyChangedWithValue(value, "IsSpawnForfeitLabelVisible");
				}
			}
		}

		[DataSourceProperty]
		public int Gold
		{
			get
			{
				return this._gold;
			}
			set
			{
				if (value != this._gold)
				{
					this._gold = value;
					base.OnPropertyChangedWithValue(value, "Gold");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<MPPlayerVM> Teammates
		{
			get
			{
				return this._teammates;
			}
			set
			{
				if (value != this._teammates)
				{
					this._teammates = value;
					base.OnPropertyChangedWithValue<MBBindingList<MPPlayerVM>>(value, "Teammates");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<MPPlayerVM> Enemies
		{
			get
			{
				return this._enemies;
			}
			set
			{
				if (value != this._enemies)
				{
					this._enemies = value;
					base.OnPropertyChangedWithValue<MBBindingList<MPPlayerVM>>(value, "Enemies");
				}
			}
		}

		[DataSourceProperty]
		public HeroInformationVM HeroInformation
		{
			get
			{
				return this._heroInformation;
			}
			set
			{
				if (value != this._heroInformation)
				{
					this._heroInformation = value;
					base.OnPropertyChangedWithValue<HeroInformationVM>(value, "HeroInformation");
				}
			}
		}

		[DataSourceProperty]
		public HeroClassVM CurrentSelectedClass
		{
			get
			{
				return this._currentSelectedClass;
			}
			set
			{
				if (value != this._currentSelectedClass)
				{
					this._currentSelectedClass = value;
					base.OnPropertyChangedWithValue<HeroClassVM>(value, "CurrentSelectedClass");
				}
			}
		}

		[DataSourceProperty]
		public string RemainingTimeText
		{
			get
			{
				return this._remainingTimeText;
			}
			set
			{
				if (value != this._remainingTimeText)
				{
					this._remainingTimeText = value;
					base.OnPropertyChangedWithValue<string>(value, "RemainingTimeText");
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
		public MBBindingList<HeroClassGroupVM> Classes
		{
			get
			{
				return this._classes;
			}
			set
			{
				if (value != this._classes)
				{
					this._classes = value;
					base.OnPropertyChangedWithValue<MBBindingList<HeroClassGroupVM>>(value, "Classes");
				}
			}
		}

		[DataSourceProperty]
		public bool IsGoldEnabled
		{
			get
			{
				return this._isGoldEnabled;
			}
			set
			{
				if (value != this._isGoldEnabled)
				{
					this._isGoldEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsGoldEnabled");
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

		public const float UPDATE_INTERVAL = 1f;

		private float _updateTimeElapsed;

		private readonly Action<MultiplayerClassDivisions.MPHeroClass> _onRefreshSelection;

		private readonly MissionMultiplayerGameModeBaseClient _missionMultiplayerGameMode;

		private Dictionary<MissionPeer, MPPlayerVM> _enemyDictionary;

		private readonly Mission _mission;

		private bool _isTeammateAndEnemiesRelevant;

		private const float REMAINING_TIME_WARNING_THRESHOLD = 5f;

		private MissionLobbyEquipmentNetworkComponent _missionLobbyEquipmentNetworkComponent;

		private bool _isInitializing;

		private Dictionary<MissionPeer, MPPlayerVM> _teammateDictionary;

		private int _gold;

		private string _culture;

		private string _cultureId;

		private string _spawnLabelText;

		private string _spawnForfeitLabelText;

		private string _remainingTimeText;

		private bool _warnRemainingTime;

		private bool _isSpawnTimerVisible;

		private bool _isSpawnLabelVisible;

		private bool _isSpawnForfeitLabelVisible;

		private bool _isGoldEnabled;

		private bool _isInWarmup;

		private bool _useSecondary;

		private bool _showAttackerOrDefenderIcons;

		private bool _isAttacker;

		private string _warmupInfoText;

		private MBBindingList<HeroClassGroupVM> _classes;

		private HeroInformationVM _heroInformation;

		private HeroClassVM _currentSelectedClass;

		private MBBindingList<MPPlayerVM> _teammates;

		private MBBindingList<MPPlayerVM> _enemies;
	}
}
