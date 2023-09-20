using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.ClassLoadout
{
	// Token: 0x020000CE RID: 206
	public class MultiplayerClassLoadoutVM : ViewModel
	{
		// Token: 0x17000651 RID: 1617
		// (get) Token: 0x0600132C RID: 4908 RVA: 0x0003EBE8 File Offset: 0x0003CDE8
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

		// Token: 0x17000652 RID: 1618
		// (get) Token: 0x0600132D RID: 4909 RVA: 0x0003EC08 File Offset: 0x0003CE08
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

		// Token: 0x0600132E RID: 4910 RVA: 0x0003EC48 File Offset: 0x0003CE48
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

		// Token: 0x0600132F RID: 4911 RVA: 0x0003EF84 File Offset: 0x0003D184
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

		// Token: 0x06001330 RID: 4912 RVA: 0x0003F038 File Offset: 0x0003D238
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

		// Token: 0x06001331 RID: 4913 RVA: 0x0003F143 File Offset: 0x0003D343
		public override void OnFinalize()
		{
			base.OnFinalize();
			MissionPeer.OnEquipmentIndexRefreshed -= this.RefreshPeerDivision;
			MissionPeer.OnPerkSelectionUpdated -= this.RefreshPeerPerkSelection;
			NetworkCommunicator.OnPeerComponentAdded -= this.OnPeerComponentAdded;
		}

		// Token: 0x06001332 RID: 4914 RVA: 0x0003F180 File Offset: 0x0003D380
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

		// Token: 0x06001333 RID: 4915 RVA: 0x0003F354 File Offset: 0x0003D554
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

		// Token: 0x06001334 RID: 4916 RVA: 0x0003F41C File Offset: 0x0003D61C
		public void RefreshPeerDivision(MissionPeer peer, int divisionType)
		{
			MPPlayerVM mpplayerVM = this.Teammates.FirstOrDefault((MPPlayerVM t) => t.Peer == peer);
			if (mpplayerVM != null)
			{
				mpplayerVM.RefreshDivision(false);
			}
		}

		// Token: 0x06001335 RID: 4917 RVA: 0x0003F458 File Offset: 0x0003D658
		private void RefreshPeerPerkSelection(MissionPeer peer)
		{
			MPPlayerVM mpplayerVM = this.Teammates.FirstOrDefault((MPPlayerVM t) => t.Peer == peer);
			if (mpplayerVM != null)
			{
				mpplayerVM.RefreshActivePerks();
			}
		}

		// Token: 0x06001336 RID: 4918 RVA: 0x0003F494 File Offset: 0x0003D694
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

		// Token: 0x06001337 RID: 4919 RVA: 0x0003F5AC File Offset: 0x0003D7AC
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

		// Token: 0x06001338 RID: 4920 RVA: 0x0003F604 File Offset: 0x0003D804
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

		// Token: 0x06001339 RID: 4921 RVA: 0x0003F768 File Offset: 0x0003D968
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

		// Token: 0x0600133A RID: 4922 RVA: 0x0003F8E4 File Offset: 0x0003DAE4
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

		// Token: 0x0600133B RID: 4923 RVA: 0x0003F930 File Offset: 0x0003DB30
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

		// Token: 0x0600133C RID: 4924 RVA: 0x0003F9A0 File Offset: 0x0003DBA0
		public void RefreshRemainingTime()
		{
			int num = MathF.Ceiling(this._missionMultiplayerGameMode.RemainingTime);
			this.RemainingTimeText = TimeSpan.FromSeconds((double)num).ToString("mm':'ss");
			this.WarnRemainingTime = (float)num < 5f;
		}

		// Token: 0x17000653 RID: 1619
		// (get) Token: 0x0600133D RID: 4925 RVA: 0x0003F9E7 File Offset: 0x0003DBE7
		// (set) Token: 0x0600133E RID: 4926 RVA: 0x0003F9EF File Offset: 0x0003DBEF
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

		// Token: 0x17000654 RID: 1620
		// (get) Token: 0x0600133F RID: 4927 RVA: 0x0003FA12 File Offset: 0x0003DC12
		// (set) Token: 0x06001340 RID: 4928 RVA: 0x0003FA1A File Offset: 0x0003DC1A
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

		// Token: 0x17000655 RID: 1621
		// (get) Token: 0x06001341 RID: 4929 RVA: 0x0003FA3D File Offset: 0x0003DC3D
		// (set) Token: 0x06001342 RID: 4930 RVA: 0x0003FA45 File Offset: 0x0003DC45
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

		// Token: 0x17000656 RID: 1622
		// (get) Token: 0x06001343 RID: 4931 RVA: 0x0003FA63 File Offset: 0x0003DC63
		// (set) Token: 0x06001344 RID: 4932 RVA: 0x0003FA6B File Offset: 0x0003DC6B
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

		// Token: 0x17000657 RID: 1623
		// (get) Token: 0x06001345 RID: 4933 RVA: 0x0003FA8E File Offset: 0x0003DC8E
		// (set) Token: 0x06001346 RID: 4934 RVA: 0x0003FA96 File Offset: 0x0003DC96
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

		// Token: 0x17000658 RID: 1624
		// (get) Token: 0x06001347 RID: 4935 RVA: 0x0003FAB4 File Offset: 0x0003DCB4
		// (set) Token: 0x06001348 RID: 4936 RVA: 0x0003FABC File Offset: 0x0003DCBC
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

		// Token: 0x17000659 RID: 1625
		// (get) Token: 0x06001349 RID: 4937 RVA: 0x0003FADA File Offset: 0x0003DCDA
		// (set) Token: 0x0600134A RID: 4938 RVA: 0x0003FAE2 File Offset: 0x0003DCE2
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

		// Token: 0x1700065A RID: 1626
		// (get) Token: 0x0600134B RID: 4939 RVA: 0x0003FB00 File Offset: 0x0003DD00
		// (set) Token: 0x0600134C RID: 4940 RVA: 0x0003FB08 File Offset: 0x0003DD08
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

		// Token: 0x1700065B RID: 1627
		// (get) Token: 0x0600134D RID: 4941 RVA: 0x0003FB26 File Offset: 0x0003DD26
		// (set) Token: 0x0600134E RID: 4942 RVA: 0x0003FB2E File Offset: 0x0003DD2E
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

		// Token: 0x1700065C RID: 1628
		// (get) Token: 0x0600134F RID: 4943 RVA: 0x0003FB51 File Offset: 0x0003DD51
		// (set) Token: 0x06001350 RID: 4944 RVA: 0x0003FB59 File Offset: 0x0003DD59
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

		// Token: 0x1700065D RID: 1629
		// (get) Token: 0x06001351 RID: 4945 RVA: 0x0003FB77 File Offset: 0x0003DD77
		// (set) Token: 0x06001352 RID: 4946 RVA: 0x0003FB7F File Offset: 0x0003DD7F
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

		// Token: 0x1700065E RID: 1630
		// (get) Token: 0x06001353 RID: 4947 RVA: 0x0003FB9D File Offset: 0x0003DD9D
		// (set) Token: 0x06001354 RID: 4948 RVA: 0x0003FBA5 File Offset: 0x0003DDA5
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

		// Token: 0x1700065F RID: 1631
		// (get) Token: 0x06001355 RID: 4949 RVA: 0x0003FBC3 File Offset: 0x0003DDC3
		// (set) Token: 0x06001356 RID: 4950 RVA: 0x0003FBCB File Offset: 0x0003DDCB
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

		// Token: 0x17000660 RID: 1632
		// (get) Token: 0x06001357 RID: 4951 RVA: 0x0003FBE9 File Offset: 0x0003DDE9
		// (set) Token: 0x06001358 RID: 4952 RVA: 0x0003FBF1 File Offset: 0x0003DDF1
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

		// Token: 0x17000661 RID: 1633
		// (get) Token: 0x06001359 RID: 4953 RVA: 0x0003FC0F File Offset: 0x0003DE0F
		// (set) Token: 0x0600135A RID: 4954 RVA: 0x0003FC17 File Offset: 0x0003DE17
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

		// Token: 0x17000662 RID: 1634
		// (get) Token: 0x0600135B RID: 4955 RVA: 0x0003FC35 File Offset: 0x0003DE35
		// (set) Token: 0x0600135C RID: 4956 RVA: 0x0003FC3D File Offset: 0x0003DE3D
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

		// Token: 0x17000663 RID: 1635
		// (get) Token: 0x0600135D RID: 4957 RVA: 0x0003FC60 File Offset: 0x0003DE60
		// (set) Token: 0x0600135E RID: 4958 RVA: 0x0003FC68 File Offset: 0x0003DE68
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

		// Token: 0x17000664 RID: 1636
		// (get) Token: 0x0600135F RID: 4959 RVA: 0x0003FC86 File Offset: 0x0003DE86
		// (set) Token: 0x06001360 RID: 4960 RVA: 0x0003FC8E File Offset: 0x0003DE8E
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

		// Token: 0x17000665 RID: 1637
		// (get) Token: 0x06001361 RID: 4961 RVA: 0x0003FCAC File Offset: 0x0003DEAC
		// (set) Token: 0x06001362 RID: 4962 RVA: 0x0003FCB4 File Offset: 0x0003DEB4
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

		// Token: 0x17000666 RID: 1638
		// (get) Token: 0x06001363 RID: 4963 RVA: 0x0003FCD2 File Offset: 0x0003DED2
		// (set) Token: 0x06001364 RID: 4964 RVA: 0x0003FCDA File Offset: 0x0003DEDA
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

		// Token: 0x17000667 RID: 1639
		// (get) Token: 0x06001365 RID: 4965 RVA: 0x0003FCF8 File Offset: 0x0003DEF8
		// (set) Token: 0x06001366 RID: 4966 RVA: 0x0003FD00 File Offset: 0x0003DF00
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

		// Token: 0x04000934 RID: 2356
		public const float UPDATE_INTERVAL = 1f;

		// Token: 0x04000935 RID: 2357
		private float _updateTimeElapsed;

		// Token: 0x04000936 RID: 2358
		private readonly Action<MultiplayerClassDivisions.MPHeroClass> _onRefreshSelection;

		// Token: 0x04000937 RID: 2359
		private readonly MissionMultiplayerGameModeBaseClient _missionMultiplayerGameMode;

		// Token: 0x04000938 RID: 2360
		private Dictionary<MissionPeer, MPPlayerVM> _enemyDictionary;

		// Token: 0x04000939 RID: 2361
		private readonly Mission _mission;

		// Token: 0x0400093A RID: 2362
		private bool _isTeammateAndEnemiesRelevant;

		// Token: 0x0400093B RID: 2363
		private const float REMAINING_TIME_WARNING_THRESHOLD = 5f;

		// Token: 0x0400093C RID: 2364
		private MissionLobbyEquipmentNetworkComponent _missionLobbyEquipmentNetworkComponent;

		// Token: 0x0400093D RID: 2365
		private bool _isInitializing;

		// Token: 0x0400093E RID: 2366
		private Dictionary<MissionPeer, MPPlayerVM> _teammateDictionary;

		// Token: 0x0400093F RID: 2367
		private int _gold;

		// Token: 0x04000940 RID: 2368
		private string _culture;

		// Token: 0x04000941 RID: 2369
		private string _cultureId;

		// Token: 0x04000942 RID: 2370
		private string _spawnLabelText;

		// Token: 0x04000943 RID: 2371
		private string _spawnForfeitLabelText;

		// Token: 0x04000944 RID: 2372
		private string _remainingTimeText;

		// Token: 0x04000945 RID: 2373
		private bool _warnRemainingTime;

		// Token: 0x04000946 RID: 2374
		private bool _isSpawnTimerVisible;

		// Token: 0x04000947 RID: 2375
		private bool _isSpawnLabelVisible;

		// Token: 0x04000948 RID: 2376
		private bool _isSpawnForfeitLabelVisible;

		// Token: 0x04000949 RID: 2377
		private bool _isGoldEnabled;

		// Token: 0x0400094A RID: 2378
		private bool _isInWarmup;

		// Token: 0x0400094B RID: 2379
		private bool _useSecondary;

		// Token: 0x0400094C RID: 2380
		private bool _showAttackerOrDefenderIcons;

		// Token: 0x0400094D RID: 2381
		private bool _isAttacker;

		// Token: 0x0400094E RID: 2382
		private string _warmupInfoText;

		// Token: 0x0400094F RID: 2383
		private MBBindingList<HeroClassGroupVM> _classes;

		// Token: 0x04000950 RID: 2384
		private HeroInformationVM _heroInformation;

		// Token: 0x04000951 RID: 2385
		private HeroClassVM _currentSelectedClass;

		// Token: 0x04000952 RID: 2386
		private MBBindingList<MPPlayerVM> _teammates;

		// Token: 0x04000953 RID: 2387
		private MBBindingList<MPPlayerVM> _enemies;
	}
}
