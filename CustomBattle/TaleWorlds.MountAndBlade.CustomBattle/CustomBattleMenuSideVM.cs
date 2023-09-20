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
	// Token: 0x02000009 RID: 9
	public class CustomBattleMenuSideVM : ViewModel
	{
		// Token: 0x17000010 RID: 16
		// (get) Token: 0x0600004D RID: 77 RVA: 0x00005A1C File Offset: 0x00003C1C
		// (set) Token: 0x0600004E RID: 78 RVA: 0x00005A24 File Offset: 0x00003C24
		public BasicCharacterObject SelectedCharacter { get; private set; }

		// Token: 0x0600004F RID: 79 RVA: 0x00005A30 File Offset: 0x00003C30
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

		// Token: 0x06000050 RID: 80 RVA: 0x00005AB0 File Offset: 0x00003CB0
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

		// Token: 0x06000051 RID: 81 RVA: 0x00005BB8 File Offset: 0x00003DB8
		public void OnPlayerTypeChange(CustomBattlePlayerType playerType)
		{
			this.CompositionGroup.OnPlayerTypeChange(playerType);
		}

		// Token: 0x06000052 RID: 82 RVA: 0x00005BC8 File Offset: 0x00003DC8
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

		// Token: 0x06000053 RID: 83 RVA: 0x00005C1C File Offset: 0x00003E1C
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

		// Token: 0x06000054 RID: 84 RVA: 0x00005CBC File Offset: 0x00003EBC
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

		// Token: 0x06000055 RID: 85 RVA: 0x00005F0C File Offset: 0x0000410C
		public void Randomize()
		{
			this.CharacterSelectionGroup.ExecuteRandomize();
			this.FactionSelectionGroup.ExecuteRandomize();
			this.CompositionGroup.ExecuteRandomize();
		}

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000056 RID: 86 RVA: 0x00005F2F File Offset: 0x0000412F
		// (set) Token: 0x06000057 RID: 87 RVA: 0x00005F37 File Offset: 0x00004137
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

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000058 RID: 88 RVA: 0x00005F55 File Offset: 0x00004155
		// (set) Token: 0x06000059 RID: 89 RVA: 0x00005F5D File Offset: 0x0000415D
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

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x0600005A RID: 90 RVA: 0x00005F7B File Offset: 0x0000417B
		// (set) Token: 0x0600005B RID: 91 RVA: 0x00005F83 File Offset: 0x00004183
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

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x0600005C RID: 92 RVA: 0x00005FA1 File Offset: 0x000041A1
		// (set) Token: 0x0600005D RID: 93 RVA: 0x00005FA9 File Offset: 0x000041A9
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

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x0600005E RID: 94 RVA: 0x00005FCC File Offset: 0x000041CC
		// (set) Token: 0x0600005F RID: 95 RVA: 0x00005FD4 File Offset: 0x000041D4
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

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x06000060 RID: 96 RVA: 0x00005FF7 File Offset: 0x000041F7
		// (set) Token: 0x06000061 RID: 97 RVA: 0x00005FFF File Offset: 0x000041FF
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

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x06000062 RID: 98 RVA: 0x00006022 File Offset: 0x00004222
		// (set) Token: 0x06000063 RID: 99 RVA: 0x0000602A File Offset: 0x0000422A
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

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x06000064 RID: 100 RVA: 0x00006048 File Offset: 0x00004248
		// (set) Token: 0x06000065 RID: 101 RVA: 0x00006050 File Offset: 0x00004250
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

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x06000066 RID: 102 RVA: 0x0000606E File Offset: 0x0000426E
		// (set) Token: 0x06000067 RID: 103 RVA: 0x00006076 File Offset: 0x00004276
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

		// Token: 0x04000048 RID: 72
		private readonly TextObject _sideName;

		// Token: 0x04000049 RID: 73
		private readonly bool _isPlayerSide;

		// Token: 0x0400004A RID: 74
		public CustomBattleMenuSideVM OppositeSide;

		// Token: 0x0400004B RID: 75
		private ArmyCompositionGroupVM _compositionGroup;

		// Token: 0x0400004C RID: 76
		private CustomBattleFactionSelectionVM _factionSelectionGroup;

		// Token: 0x0400004D RID: 77
		private SelectorVM<CharacterItemVM> _characterSelectionGroup;

		// Token: 0x0400004E RID: 78
		private CharacterViewModel _currentSelectedCharacter;

		// Token: 0x0400004F RID: 79
		private MBBindingList<CharacterEquipmentItemVM> _armorsList;

		// Token: 0x04000050 RID: 80
		private MBBindingList<CharacterEquipmentItemVM> _weaponsList;

		// Token: 0x04000051 RID: 81
		private string _name;

		// Token: 0x04000052 RID: 82
		private string _factionText;

		// Token: 0x04000053 RID: 83
		private string _titleText;
	}
}
