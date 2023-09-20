using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.CustomBattle.CustomBattle;
using TaleWorlds.MountAndBlade.ViewModelCollection.Input;

namespace TaleWorlds.MountAndBlade.CustomBattle
{
	public class CustomBattleMenuVM : ViewModel
	{
		private static CustomBattleCompositionData GetBattleCompositionDataFromCompositionGroup(ArmyCompositionGroupVM compositionGroup)
		{
			return new CustomBattleCompositionData((float)compositionGroup.RangedInfantryComposition.CompositionValue / 100f, (float)compositionGroup.MeleeCavalryComposition.CompositionValue / 100f, (float)compositionGroup.RangedCavalryComposition.CompositionValue / 100f);
		}

		private static List<BasicCharacterObject>[] GetTroopSelections(ArmyCompositionGroupVM armyComposition)
		{
			List<BasicCharacterObject>[] array = new List<BasicCharacterObject>[4];
			array[0] = (from x in armyComposition.MeleeInfantryComposition.TroopTypes
				where x.IsSelected
				select x.Character).ToList<BasicCharacterObject>();
			array[1] = (from x in armyComposition.RangedInfantryComposition.TroopTypes
				where x.IsSelected
				select x.Character).ToList<BasicCharacterObject>();
			array[2] = (from x in armyComposition.MeleeCavalryComposition.TroopTypes
				where x.IsSelected
				select x.Character).ToList<BasicCharacterObject>();
			array[3] = (from x in armyComposition.RangedCavalryComposition.TroopTypes
				where x.IsSelected
				select x.Character).ToList<BasicCharacterObject>();
			return array;
		}

		private static void FillSiegeMachines(List<MissionSiegeWeapon> machines, MBBindingList<CustomBattleSiegeMachineVM> vmMachines)
		{
			foreach (CustomBattleSiegeMachineVM customBattleSiegeMachineVM in vmMachines)
			{
				if (customBattleSiegeMachineVM.SiegeEngineType != null)
				{
					machines.Add(MissionSiegeWeapon.CreateDefaultWeapon(customBattleSiegeMachineVM.SiegeEngineType));
				}
			}
		}

		public CustomBattleMenuVM(CustomBattleState battleState)
		{
			this._customBattleState = battleState;
			this.IsAttackerCustomMachineSelectionEnabled = false;
			this.TroopTypeSelectionPopUp = new TroopTypeSelectionPopUpVM();
			this.PlayerSide = new CustomBattleMenuSideVM(new TextObject("{=BC7n6qxk}PLAYER", null), true, this.TroopTypeSelectionPopUp);
			this.EnemySide = new CustomBattleMenuSideVM(new TextObject("{=35IHscBa}ENEMY", null), false, this.TroopTypeSelectionPopUp);
			this.PlayerSide.OppositeSide = this.EnemySide;
			this.EnemySide.OppositeSide = this.PlayerSide;
			this.MapSelectionGroup = new MapSelectionGroupVM();
			this.GameTypeSelectionGroup = new GameTypeSelectionGroupVM(new Action<CustomBattlePlayerType>(this.OnPlayerTypeChange), new Action<CustomBattleGameType>(this.OnGameTypeChange));
			this.AttackerMeleeMachines = new MBBindingList<CustomBattleSiegeMachineVM>();
			for (int i = 0; i < 3; i++)
			{
				this.AttackerMeleeMachines.Add(new CustomBattleSiegeMachineVM(null, new Action<CustomBattleSiegeMachineVM>(this.OnMeleeMachineSelection), new Action<CustomBattleSiegeMachineVM>(this.OnResetMachineSelection)));
			}
			this.AttackerRangedMachines = new MBBindingList<CustomBattleSiegeMachineVM>();
			for (int j = 0; j < 4; j++)
			{
				this.AttackerRangedMachines.Add(new CustomBattleSiegeMachineVM(null, new Action<CustomBattleSiegeMachineVM>(this.OnAttackerRangedMachineSelection), new Action<CustomBattleSiegeMachineVM>(this.OnResetMachineSelection)));
			}
			this.DefenderMachines = new MBBindingList<CustomBattleSiegeMachineVM>();
			for (int k = 0; k < 4; k++)
			{
				this.DefenderMachines.Add(new CustomBattleSiegeMachineVM(null, new Action<CustomBattleSiegeMachineVM>(this.OnDefenderRangedMachineSelection), new Action<CustomBattleSiegeMachineVM>(this.OnResetMachineSelection)));
			}
			this.RefreshValues();
			this.SetDefaultSiegeMachines();
		}

		private void SetDefaultSiegeMachines()
		{
			this.AttackerMeleeMachines[0].SetMachineType(DefaultSiegeEngineTypes.SiegeTower);
			this.AttackerMeleeMachines[1].SetMachineType(DefaultSiegeEngineTypes.Ram);
			this.AttackerMeleeMachines[2].SetMachineType(DefaultSiegeEngineTypes.SiegeTower);
			this.AttackerRangedMachines[0].SetMachineType(DefaultSiegeEngineTypes.Trebuchet);
			this.AttackerRangedMachines[1].SetMachineType(DefaultSiegeEngineTypes.Onager);
			this.AttackerRangedMachines[2].SetMachineType(DefaultSiegeEngineTypes.Onager);
			this.AttackerRangedMachines[3].SetMachineType(DefaultSiegeEngineTypes.FireBallista);
			this.DefenderMachines[0].SetMachineType(DefaultSiegeEngineTypes.FireCatapult);
			this.DefenderMachines[1].SetMachineType(DefaultSiegeEngineTypes.FireCatapult);
			this.DefenderMachines[2].SetMachineType(DefaultSiegeEngineTypes.Catapult);
			this.DefenderMachines[3].SetMachineType(DefaultSiegeEngineTypes.FireBallista);
		}

		public void SetActiveState(bool isActive)
		{
			if (isActive)
			{
				this.EnemySide.UpdateCharacterVisual();
				this.PlayerSide.UpdateCharacterVisual();
				return;
			}
			this.EnemySide.CurrentSelectedCharacter = null;
			this.PlayerSide.CurrentSelectedCharacter = null;
		}

		private void OnPlayerTypeChange(CustomBattlePlayerType playerType)
		{
			this.PlayerSide.OnPlayerTypeChange(playerType);
		}

		private void OnGameTypeChange(CustomBattleGameType gameType)
		{
			this.MapSelectionGroup.OnGameTypeChange(gameType);
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.RandomizeButtonText = GameTexts.FindText("str_randomize", null).ToString();
			this.StartButtonText = GameTexts.FindText("str_start", null).ToString();
			this.BackButtonText = GameTexts.FindText("str_back", null).ToString();
			this.TitleText = GameTexts.FindText("str_custom_battle", null).ToString();
			this.EnemySide.RefreshValues();
			this.PlayerSide.RefreshValues();
			this.AttackerMeleeMachines.ApplyActionOnAllItems(delegate(CustomBattleSiegeMachineVM x)
			{
				x.RefreshValues();
			});
			this.AttackerRangedMachines.ApplyActionOnAllItems(delegate(CustomBattleSiegeMachineVM x)
			{
				x.RefreshValues();
			});
			this.DefenderMachines.ApplyActionOnAllItems(delegate(CustomBattleSiegeMachineVM x)
			{
				x.RefreshValues();
			});
			this.MapSelectionGroup.RefreshValues();
			TroopTypeSelectionPopUpVM troopTypeSelectionPopUp = this.TroopTypeSelectionPopUp;
			if (troopTypeSelectionPopUp == null)
			{
				return;
			}
			troopTypeSelectionPopUp.RefreshValues();
		}

		private void OnResetMachineSelection(CustomBattleSiegeMachineVM selectedSlot)
		{
			selectedSlot.SetMachineType(null);
		}

		private void OnMeleeMachineSelection(CustomBattleSiegeMachineVM selectedSlot)
		{
			List<InquiryElement> list = new List<InquiryElement>();
			list.Add(new InquiryElement(null, GameTexts.FindText("str_empty", null).ToString(), null));
			foreach (SiegeEngineType siegeEngineType in CustomBattleData.GetAllAttackerMeleeMachines())
			{
				list.Add(new InquiryElement(siegeEngineType, siegeEngineType.Name.ToString(), null));
			}
			MBInformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData(new TextObject("{=MVOWsP48}Select a Melee Machine", null).ToString(), string.Empty, list, false, 1, 1, GameTexts.FindText("str_done", null).ToString(), "", delegate(List<InquiryElement> selectedElements)
			{
				CustomBattleSiegeMachineVM selectedSlot2 = selectedSlot;
				InquiryElement inquiryElement = selectedElements.FirstOrDefault<InquiryElement>();
				selectedSlot2.SetMachineType(((inquiryElement != null) ? inquiryElement.Identifier : null) as SiegeEngineType);
			}, null, ""), false, false);
		}

		private void OnAttackerRangedMachineSelection(CustomBattleSiegeMachineVM selectedSlot)
		{
			List<InquiryElement> list = new List<InquiryElement>();
			list.Add(new InquiryElement(null, GameTexts.FindText("str_empty", null).ToString(), null));
			foreach (SiegeEngineType siegeEngineType in CustomBattleData.GetAllAttackerRangedMachines())
			{
				list.Add(new InquiryElement(siegeEngineType, siegeEngineType.Name.ToString(), null));
			}
			MBInformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData(new TextObject("{=SLZzfNPr}Select a Ranged Machine", null).ToString(), string.Empty, list, false, 1, 1, GameTexts.FindText("str_done", null).ToString(), "", delegate(List<InquiryElement> selectedElements)
			{
				CustomBattleSiegeMachineVM selectedSlot2 = selectedSlot;
				InquiryElement inquiryElement = selectedElements.FirstOrDefault<InquiryElement>();
				selectedSlot2.SetMachineType(((inquiryElement != null) ? inquiryElement.Identifier : null) as SiegeEngineType);
			}, null, ""), false, false);
		}

		private void OnDefenderRangedMachineSelection(CustomBattleSiegeMachineVM selectedSlot)
		{
			List<InquiryElement> list = new List<InquiryElement>();
			list.Add(new InquiryElement(null, GameTexts.FindText("str_empty", null).ToString(), null));
			foreach (SiegeEngineType siegeEngineType in CustomBattleData.GetAllDefenderRangedMachines())
			{
				list.Add(new InquiryElement(siegeEngineType, siegeEngineType.Name.ToString(), null));
			}
			MBInformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData(new TextObject("{=SLZzfNPr}Select a Ranged Machine", null).ToString(), string.Empty, list, false, 1, 1, GameTexts.FindText("str_done", null).ToString(), "", delegate(List<InquiryElement> selectedElements)
			{
				CustomBattleSiegeMachineVM selectedSlot2 = selectedSlot;
				InquiryElement inquiryElement = selectedElements.FirstOrDefault<InquiryElement>();
				selectedSlot2.SetMachineType(((inquiryElement != null) ? inquiryElement.Identifier : null) as SiegeEngineType);
			}, null, ""), false, false);
		}

		private void ExecuteRandomizeAttackerSiegeEngines()
		{
			MBList<SiegeEngineType> mblist = new MBList<SiegeEngineType>();
			mblist.AddRange(CustomBattleData.GetAllAttackerMeleeMachines());
			mblist.Add(null);
			foreach (CustomBattleSiegeMachineVM customBattleSiegeMachineVM in this._attackerMeleeMachines)
			{
				customBattleSiegeMachineVM.SetMachineType(Extensions.GetRandomElement<SiegeEngineType>(mblist));
			}
			mblist.Clear();
			mblist.AddRange(CustomBattleData.GetAllAttackerRangedMachines());
			mblist.Add(null);
			foreach (CustomBattleSiegeMachineVM customBattleSiegeMachineVM2 in this._attackerRangedMachines)
			{
				customBattleSiegeMachineVM2.SetMachineType(Extensions.GetRandomElement<SiegeEngineType>(mblist));
			}
		}

		private void ExecuteRandomizeDefenderSiegeEngines()
		{
			MBList<SiegeEngineType> mblist = new MBList<SiegeEngineType>();
			mblist.AddRange(CustomBattleData.GetAllDefenderRangedMachines());
			mblist.Add(null);
			foreach (CustomBattleSiegeMachineVM customBattleSiegeMachineVM in this._defenderMachines)
			{
				customBattleSiegeMachineVM.SetMachineType(Extensions.GetRandomElement<SiegeEngineType>(mblist));
			}
		}

		public void ExecuteBack()
		{
			Debug.Print("EXECUTE BACK - PRESSED", 0, 4, 17592186044416UL);
			Game.Current.GameStateManager.PopState(0);
		}

		private CustomBattleData PrepareBattleData()
		{
			BasicCharacterObject selectedCharacter = this.PlayerSide.SelectedCharacter;
			BasicCharacterObject selectedCharacter2 = this.EnemySide.SelectedCharacter;
			int num = this.PlayerSide.CompositionGroup.ArmySize;
			int armySize = this.EnemySide.CompositionGroup.ArmySize;
			bool flag = this.GameTypeSelectionGroup.SelectedPlayerSide == CustomBattlePlayerSide.Attacker;
			bool flag2 = this.GameTypeSelectionGroup.SelectedPlayerType == CustomBattlePlayerType.Commander;
			BasicCharacterObject basicCharacterObject = null;
			if (!flag2)
			{
				MBList<BasicCharacterObject> mblist = Extensions.ToMBList<BasicCharacterObject>(CustomBattleData.Characters);
				mblist.Remove(selectedCharacter);
				mblist.Remove(selectedCharacter2);
				basicCharacterObject = Extensions.GetRandomElement<BasicCharacterObject>(mblist);
				num--;
			}
			int[] troopCounts = CustomBattleHelper.GetTroopCounts(num, CustomBattleMenuVM.GetBattleCompositionDataFromCompositionGroup(this.PlayerSide.CompositionGroup));
			int[] troopCounts2 = CustomBattleHelper.GetTroopCounts(armySize, CustomBattleMenuVM.GetBattleCompositionDataFromCompositionGroup(this.EnemySide.CompositionGroup));
			List<BasicCharacterObject>[] troopSelections = CustomBattleMenuVM.GetTroopSelections(this.PlayerSide.CompositionGroup);
			List<BasicCharacterObject>[] troopSelections2 = CustomBattleMenuVM.GetTroopSelections(this.EnemySide.CompositionGroup);
			BasicCultureObject faction = this.PlayerSide.FactionSelectionGroup.SelectedItem.Faction;
			BasicCultureObject faction2 = this.EnemySide.FactionSelectionGroup.SelectedItem.Faction;
			CustomBattleCombatant[] customBattleParties = CustomBattleHelper.GetCustomBattleParties(selectedCharacter, basicCharacterObject, selectedCharacter2, faction, troopCounts, troopSelections, faction2, troopCounts2, troopSelections2, flag);
			List<MissionSiegeWeapon> list = null;
			List<MissionSiegeWeapon> list2 = null;
			float[] array = null;
			if (this.GameTypeSelectionGroup.SelectedGameType == CustomBattleGameType.Siege)
			{
				list = new List<MissionSiegeWeapon>();
				list2 = new List<MissionSiegeWeapon>();
				CustomBattleMenuVM.FillSiegeMachines(list, this._attackerMeleeMachines);
				CustomBattleMenuVM.FillSiegeMachines(list, this._attackerRangedMachines);
				CustomBattleMenuVM.FillSiegeMachines(list2, this._defenderMachines);
				array = CustomBattleHelper.GetWallHitpointPercentages(this.MapSelectionGroup.SelectedWallBreachedCount);
			}
			return CustomBattleHelper.PrepareBattleData(selectedCharacter, basicCharacterObject, customBattleParties[0], customBattleParties[1], this.GameTypeSelectionGroup.SelectedPlayerSide, this.GameTypeSelectionGroup.SelectedPlayerType, this.GameTypeSelectionGroup.SelectedGameType, this.MapSelectionGroup.SelectedMap.MapId, this.MapSelectionGroup.SelectedSeasonId, (float)this.MapSelectionGroup.SelectedTimeOfDay, list, list2, array, this.MapSelectionGroup.SelectedSceneLevel, this.MapSelectionGroup.IsSallyOutSelected);
		}

		public void ExecuteStart()
		{
			CustomBattleHelper.StartGame(this.PrepareBattleData());
			Debug.Print("EXECUTE START - PRESSED", 0, 4, 17592186044416UL);
		}

		public void ExecuteRandomize()
		{
			this.GameTypeSelectionGroup.RandomizeAll();
			this.MapSelectionGroup.RandomizeAll();
			this.PlayerSide.Randomize();
			this.EnemySide.Randomize();
			this.ExecuteRandomizeAttackerSiegeEngines();
			this.ExecuteRandomizeDefenderSiegeEngines();
			Debug.Print("EXECUTE RANDOMIZE - PRESSED", 0, 4, 17592186044416UL);
		}

		private void ExecuteDoneDefenderCustomMachineSelection()
		{
			this.IsDefenderCustomMachineSelectionEnabled = false;
		}

		private void ExecuteDoneAttackerCustomMachineSelection()
		{
			this.IsAttackerCustomMachineSelectionEnabled = false;
		}

		public override void OnFinalize()
		{
			base.OnFinalize();
			this.StartInputKey.OnFinalize();
			this.CancelInputKey.OnFinalize();
			this.ResetInputKey.OnFinalize();
			this.RandomizeInputKey.OnFinalize();
			TroopTypeSelectionPopUpVM troopTypeSelectionPopUp = this.TroopTypeSelectionPopUp;
			if (troopTypeSelectionPopUp == null)
			{
				return;
			}
			troopTypeSelectionPopUp.OnFinalize();
		}

		[DataSourceProperty]
		public TroopTypeSelectionPopUpVM TroopTypeSelectionPopUp
		{
			get
			{
				return this._troopTypeSelectionPopUp;
			}
			set
			{
				if (value != this._troopTypeSelectionPopUp)
				{
					this._troopTypeSelectionPopUp = value;
					base.OnPropertyChangedWithValue<TroopTypeSelectionPopUpVM>(value, "TroopTypeSelectionPopUp");
				}
			}
		}

		[DataSourceProperty]
		public bool IsAttackerCustomMachineSelectionEnabled
		{
			get
			{
				return this._isAttackerCustomMachineSelectionEnabled;
			}
			set
			{
				if (value != this._isAttackerCustomMachineSelectionEnabled)
				{
					this._isAttackerCustomMachineSelectionEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsAttackerCustomMachineSelectionEnabled");
				}
			}
		}

		[DataSourceProperty]
		public bool IsDefenderCustomMachineSelectionEnabled
		{
			get
			{
				return this._isDefenderCustomMachineSelectionEnabled;
			}
			set
			{
				if (value != this._isDefenderCustomMachineSelectionEnabled)
				{
					this._isDefenderCustomMachineSelectionEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsDefenderCustomMachineSelectionEnabled");
				}
			}
		}

		[DataSourceProperty]
		public string RandomizeButtonText
		{
			get
			{
				return this._randomizeButtonText;
			}
			set
			{
				if (value != this._randomizeButtonText)
				{
					this._randomizeButtonText = value;
					base.OnPropertyChangedWithValue<string>(value, "RandomizeButtonText");
				}
			}
		}

		[DataSourceProperty]
		public string TitleText
		{
			get
			{
				return this._titleText;
			}
			set
			{
				if (value != this._titleText)
				{
					this._titleText = value;
					base.OnPropertyChangedWithValue<string>(value, "TitleText");
				}
			}
		}

		[DataSourceProperty]
		public string BackButtonText
		{
			get
			{
				return this._backButtonText;
			}
			set
			{
				if (value != this._backButtonText)
				{
					this._backButtonText = value;
					base.OnPropertyChangedWithValue<string>(value, "BackButtonText");
				}
			}
		}

		[DataSourceProperty]
		public string StartButtonText
		{
			get
			{
				return this._startButtonText;
			}
			set
			{
				if (value != this._startButtonText)
				{
					this._startButtonText = value;
					base.OnPropertyChangedWithValue<string>(value, "StartButtonText");
				}
			}
		}

		[DataSourceProperty]
		public CustomBattleMenuSideVM EnemySide
		{
			get
			{
				return this._enemySide;
			}
			set
			{
				if (value != this._enemySide)
				{
					this._enemySide = value;
					base.OnPropertyChangedWithValue<CustomBattleMenuSideVM>(value, "EnemySide");
				}
			}
		}

		[DataSourceProperty]
		public CustomBattleMenuSideVM PlayerSide
		{
			get
			{
				return this._playerSide;
			}
			set
			{
				if (value != this._playerSide)
				{
					this._playerSide = value;
					base.OnPropertyChangedWithValue<CustomBattleMenuSideVM>(value, "PlayerSide");
				}
			}
		}

		[DataSourceProperty]
		public GameTypeSelectionGroupVM GameTypeSelectionGroup
		{
			get
			{
				return this._gameTypeSelectionGroup;
			}
			set
			{
				if (value != this._gameTypeSelectionGroup)
				{
					this._gameTypeSelectionGroup = value;
					base.OnPropertyChangedWithValue<GameTypeSelectionGroupVM>(value, "GameTypeSelectionGroup");
				}
			}
		}

		[DataSourceProperty]
		public MapSelectionGroupVM MapSelectionGroup
		{
			get
			{
				return this._mapSelectionGroup;
			}
			set
			{
				if (value != this._mapSelectionGroup)
				{
					this._mapSelectionGroup = value;
					base.OnPropertyChangedWithValue<MapSelectionGroupVM>(value, "MapSelectionGroup");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<CustomBattleSiegeMachineVM> AttackerMeleeMachines
		{
			get
			{
				return this._attackerMeleeMachines;
			}
			set
			{
				if (value != this._attackerMeleeMachines)
				{
					this._attackerMeleeMachines = value;
					base.OnPropertyChangedWithValue<MBBindingList<CustomBattleSiegeMachineVM>>(value, "AttackerMeleeMachines");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<CustomBattleSiegeMachineVM> AttackerRangedMachines
		{
			get
			{
				return this._attackerRangedMachines;
			}
			set
			{
				if (value != this._attackerRangedMachines)
				{
					this._attackerRangedMachines = value;
					base.OnPropertyChangedWithValue<MBBindingList<CustomBattleSiegeMachineVM>>(value, "AttackerRangedMachines");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<CustomBattleSiegeMachineVM> DefenderMachines
		{
			get
			{
				return this._defenderMachines;
			}
			set
			{
				if (value != this._defenderMachines)
				{
					this._defenderMachines = value;
					base.OnPropertyChangedWithValue<MBBindingList<CustomBattleSiegeMachineVM>>(value, "DefenderMachines");
				}
			}
		}

		public void SetStartInputKey(HotKey hotkey)
		{
			this.StartInputKey = InputKeyItemVM.CreateFromHotKey(hotkey, true);
		}

		public void SetCancelInputKey(HotKey hotkey)
		{
			this.CancelInputKey = InputKeyItemVM.CreateFromHotKey(hotkey, true);
			TroopTypeSelectionPopUpVM troopTypeSelectionPopUp = this.TroopTypeSelectionPopUp;
			if (troopTypeSelectionPopUp == null)
			{
				return;
			}
			troopTypeSelectionPopUp.SetCancelInputKey(hotkey);
		}

		public void SetResetInputKey(HotKey hotkey)
		{
			this.ResetInputKey = InputKeyItemVM.CreateFromHotKey(hotkey, true);
			TroopTypeSelectionPopUpVM troopTypeSelectionPopUp = this.TroopTypeSelectionPopUp;
			if (troopTypeSelectionPopUp == null)
			{
				return;
			}
			troopTypeSelectionPopUp.SetResetInputKey(hotkey);
		}

		public void SetRandomizeInputKey(HotKey hotkey)
		{
			this.RandomizeInputKey = InputKeyItemVM.CreateFromHotKey(hotkey, true);
		}

		public InputKeyItemVM StartInputKey
		{
			get
			{
				return this._startInputKey;
			}
			set
			{
				if (value != this._startInputKey)
				{
					this._startInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "StartInputKey");
				}
			}
		}

		public InputKeyItemVM CancelInputKey
		{
			get
			{
				return this._cancelInputKey;
			}
			set
			{
				if (value != this._cancelInputKey)
				{
					this._cancelInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "CancelInputKey");
				}
			}
		}

		public InputKeyItemVM ResetInputKey
		{
			get
			{
				return this._resetInputKey;
			}
			set
			{
				if (value != this._resetInputKey)
				{
					this._resetInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "ResetInputKey");
				}
			}
		}

		public InputKeyItemVM RandomizeInputKey
		{
			get
			{
				return this._randomizeInputKey;
			}
			set
			{
				if (value != this._randomizeInputKey)
				{
					this._randomizeInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "RandomizeInputKey");
				}
			}
		}

		private CustomBattleState _customBattleState;

		private TroopTypeSelectionPopUpVM _troopTypeSelectionPopUp;

		private CustomBattleMenuSideVM _enemySide;

		private CustomBattleMenuSideVM _playerSide;

		private bool _isAttackerCustomMachineSelectionEnabled;

		private bool _isDefenderCustomMachineSelectionEnabled;

		private GameTypeSelectionGroupVM _gameTypeSelectionGroup;

		private MapSelectionGroupVM _mapSelectionGroup;

		private string _randomizeButtonText;

		private string _backButtonText;

		private string _startButtonText;

		private string _titleText;

		private MBBindingList<CustomBattleSiegeMachineVM> _attackerMeleeMachines;

		private MBBindingList<CustomBattleSiegeMachineVM> _attackerRangedMachines;

		private MBBindingList<CustomBattleSiegeMachineVM> _defenderMachines;

		private InputKeyItemVM _startInputKey;

		private InputKeyItemVM _cancelInputKey;

		private InputKeyItemVM _resetInputKey;

		private InputKeyItemVM _randomizeInputKey;
	}
}
