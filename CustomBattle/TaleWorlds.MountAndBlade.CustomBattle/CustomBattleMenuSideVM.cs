using System;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Core.ViewModelCollection.Selector;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.CustomBattle.CustomBattle;
using TaleWorlds.MountAndBlade.CustomBattle.CustomBattle.SelectionItem;

namespace TaleWorlds.MountAndBlade.CustomBattle
{
	public class CustomBattleMenuSideVM : ViewModel
	{
		public BasicCharacterObject SelectedCharacter { get; private set; }

		public CustomBattleMenuSideVM(TextObject sideName, bool isPlayerSide, TroopTypeSelectionPopUpVM troopTypeSelectionPopUp)
		{
			this._sideName = sideName;
			this._isPlayerSide = isPlayerSide;
			this.CompositionGroup = new ArmyCompositionGroupVM(this._isPlayerSide, troopTypeSelectionPopUp);
			this.FactionSelectionGroup = new CustomBattleFactionSelectionVM(new Action<BasicCultureObject>(this.OnCultureSelection));
			this.CharacterSelectionGroup = new SelectorVM<CharacterItemVM>(0, new Action<SelectorVM<CharacterItemVM>>(this.OnCharacterSelection));
			this.ArmorsList = new MBBindingList<CharacterEquipmentItemVM>();
			this.WeaponsList = new MBBindingList<CharacterEquipmentItemVM>();
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Name = this._sideName.ToString();
			this.FactionText = GameTexts.FindText("str_faction", null).ToString();
			if (this._isPlayerSide)
			{
				this.TitleText = new TextObject("{=bLXleed8}Player Character", null).ToString();
			}
			else
			{
				this.TitleText = new TextObject("{=QAYngoNQ}Enemy Character", null).ToString();
			}
			this.CharacterSelectionGroup.ItemList.Clear();
			foreach (BasicCharacterObject basicCharacterObject in CustomBattleData.Characters)
			{
				this.CharacterSelectionGroup.AddItem(new CharacterItemVM(basicCharacterObject));
			}
			this.CharacterSelectionGroup.SelectedIndex = (this._isPlayerSide ? 0 : 1);
			this.UpdateCharacterVisual();
			this.CompositionGroup.RefreshValues();
			this.CharacterSelectionGroup.RefreshValues();
			this.FactionSelectionGroup.RefreshValues();
		}

		public void OnPlayerTypeChange(CustomBattlePlayerType playerType)
		{
			this.CompositionGroup.OnPlayerTypeChange(playerType);
		}

		private void OnCultureSelection(BasicCultureObject selectedCulture)
		{
			this.CompositionGroup.SetCurrentSelectedCulture(selectedCulture);
			if (this.CurrentSelectedCharacter != null)
			{
				this.CurrentSelectedCharacter.ArmorColor1 = selectedCulture.Color;
				this.CurrentSelectedCharacter.ArmorColor2 = selectedCulture.Color2;
				this.CurrentSelectedCharacter.BannerCodeText = selectedCulture.BannerKey;
			}
		}

		private void OnCharacterSelection(SelectorVM<CharacterItemVM> selector)
		{
			BasicCharacterObject character = selector.SelectedItem.Character;
			this.SelectedCharacter = character;
			this.UpdateCharacterVisual();
			if (this.OppositeSide != null)
			{
				int num = 0;
				foreach (CharacterItemVM characterItemVM in selector.ItemList)
				{
					CharacterItemVM characterItemVM2 = this.OppositeSide.CharacterSelectionGroup.ItemList[num];
					if (num == selector.SelectedIndex)
					{
						characterItemVM2.CanBeSelected = false;
					}
					else
					{
						characterItemVM2.CanBeSelected = true;
					}
					num++;
				}
			}
		}

		public void UpdateCharacterVisual()
		{
			this.CurrentSelectedCharacter = new CharacterViewModel(1);
			this.CurrentSelectedCharacter.FillFrom(this.SelectedCharacter, -1);
			CustomBattleFactionSelectionVM factionSelectionGroup = this.FactionSelectionGroup;
			if (((factionSelectionGroup != null) ? factionSelectionGroup.SelectedItem : null) != null)
			{
				this.CurrentSelectedCharacter.ArmorColor1 = this.FactionSelectionGroup.SelectedItem.Faction.Color;
				this.CurrentSelectedCharacter.ArmorColor2 = this.FactionSelectionGroup.SelectedItem.Faction.Color2;
				this.CurrentSelectedCharacter.BannerCodeText = this.FactionSelectionGroup.SelectedItem.Faction.BannerKey;
			}
			this.ArmorsList.Clear();
			this.ArmorsList.Add(new CharacterEquipmentItemVM(this.SelectedCharacter.Equipment[5].Item));
			this.ArmorsList.Add(new CharacterEquipmentItemVM(this.SelectedCharacter.Equipment[9].Item));
			this.ArmorsList.Add(new CharacterEquipmentItemVM(this.SelectedCharacter.Equipment[6].Item));
			this.ArmorsList.Add(new CharacterEquipmentItemVM(this.SelectedCharacter.Equipment[8].Item));
			this.ArmorsList.Add(new CharacterEquipmentItemVM(this.SelectedCharacter.Equipment[7].Item));
			this.WeaponsList.Clear();
			this.WeaponsList.Add(new CharacterEquipmentItemVM(this.SelectedCharacter.Equipment[0].Item));
			this.WeaponsList.Add(new CharacterEquipmentItemVM(this.SelectedCharacter.Equipment[1].Item));
			this.WeaponsList.Add(new CharacterEquipmentItemVM(this.SelectedCharacter.Equipment[2].Item));
			this.WeaponsList.Add(new CharacterEquipmentItemVM(this.SelectedCharacter.Equipment[3].Item));
			this.WeaponsList.Add(new CharacterEquipmentItemVM(this.SelectedCharacter.Equipment[4].Item));
		}

		public void Randomize()
		{
			this.CharacterSelectionGroup.ExecuteRandomize();
			this.FactionSelectionGroup.ExecuteRandomize();
			this.CompositionGroup.ExecuteRandomize();
		}

		[DataSourceProperty]
		public CharacterViewModel CurrentSelectedCharacter
		{
			get
			{
				return this._currentSelectedCharacter;
			}
			set
			{
				if (value != this._currentSelectedCharacter)
				{
					this._currentSelectedCharacter = value;
					base.OnPropertyChangedWithValue<CharacterViewModel>(value, "CurrentSelectedCharacter");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<CharacterEquipmentItemVM> ArmorsList
		{
			get
			{
				return this._armorsList;
			}
			set
			{
				if (value != this._armorsList)
				{
					this._armorsList = value;
					base.OnPropertyChangedWithValue<MBBindingList<CharacterEquipmentItemVM>>(value, "ArmorsList");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<CharacterEquipmentItemVM> WeaponsList
		{
			get
			{
				return this._weaponsList;
			}
			set
			{
				if (value != this._weaponsList)
				{
					this._weaponsList = value;
					base.OnPropertyChangedWithValue<MBBindingList<CharacterEquipmentItemVM>>(value, "WeaponsList");
				}
			}
		}

		[DataSourceProperty]
		public string FactionText
		{
			get
			{
				return this._factionText;
			}
			set
			{
				if (value != this._factionText)
				{
					this._factionText = value;
					base.OnPropertyChangedWithValue<string>(value, "FactionText");
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
		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				if (value != this._name)
				{
					this._name = value;
					base.OnPropertyChangedWithValue<string>(value, "Name");
				}
			}
		}

		[DataSourceProperty]
		public SelectorVM<CharacterItemVM> CharacterSelectionGroup
		{
			get
			{
				return this._characterSelectionGroup;
			}
			set
			{
				if (value != this._characterSelectionGroup)
				{
					this._characterSelectionGroup = value;
					base.OnPropertyChangedWithValue<SelectorVM<CharacterItemVM>>(value, "CharacterSelectionGroup");
				}
			}
		}

		[DataSourceProperty]
		public ArmyCompositionGroupVM CompositionGroup
		{
			get
			{
				return this._compositionGroup;
			}
			set
			{
				if (value != this._compositionGroup)
				{
					this._compositionGroup = value;
					base.OnPropertyChangedWithValue<ArmyCompositionGroupVM>(value, "CompositionGroup");
				}
			}
		}

		[DataSourceProperty]
		public CustomBattleFactionSelectionVM FactionSelectionGroup
		{
			get
			{
				return this._factionSelectionGroup;
			}
			set
			{
				if (value != this._factionSelectionGroup)
				{
					this._factionSelectionGroup = value;
					base.OnPropertyChangedWithValue<CustomBattleFactionSelectionVM>(value, "FactionSelectionGroup");
				}
			}
		}

		private readonly TextObject _sideName;

		private readonly bool _isPlayerSide;

		public CustomBattleMenuSideVM OppositeSide;

		private ArmyCompositionGroupVM _compositionGroup;

		private CustomBattleFactionSelectionVM _factionSelectionGroup;

		private SelectorVM<CharacterItemVM> _characterSelectionGroup;

		private CharacterViewModel _currentSelectedCharacter;

		private MBBindingList<CharacterEquipmentItemVM> _armorsList;

		private MBBindingList<CharacterEquipmentItemVM> _weaponsList;

		private string _name;

		private string _factionText;

		private string _titleText;
	}
}
