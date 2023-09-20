using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.CustomBattle.CustomBattle;

namespace TaleWorlds.MountAndBlade.CustomBattle
{
	// Token: 0x02000008 RID: 8
	public class ArmyCompositionItemVM : ViewModel
	{
		// Token: 0x06000036 RID: 54 RVA: 0x00005478 File Offset: 0x00003678
		public ArmyCompositionItemVM(ArmyCompositionItemVM.CompositionType type, List<BasicCharacterObject> allCharacterObjects, MBReadOnlyList<SkillObject> allSkills, Action<int, int> onCompositionValueChanged, TroopTypeSelectionPopUpVM troopTypeSelectionPopUp, int[] compositionValues)
		{
			this._allCharacterObjects = allCharacterObjects;
			this._allSkills = allSkills;
			this._onCompositionValueChanged = onCompositionValueChanged;
			this._troopTypeSelectionPopUp = troopTypeSelectionPopUp;
			this._type = type;
			this._compositionValues = compositionValues;
			this._typeIconData = ArmyCompositionItemVM.GetTroopTypeIconData(type, false);
			this.TroopTypes = new MBBindingList<CustomBattleTroopTypeVM>();
			this.InvalidHint = new HintViewModel(new TextObject("{=iSQTtNUD}This faction doesn't have this troop type.", null), null);
			this.AddTroopTypeHint = new HintViewModel(new TextObject("{=eMbuGGus}Select troops to spawn in formation.", null), null);
		}

		// Token: 0x06000037 RID: 55 RVA: 0x000054FE File Offset: 0x000036FE
		public override void RefreshValues()
		{
			base.RefreshValues();
		}

		// Token: 0x06000038 RID: 56 RVA: 0x00005506 File Offset: 0x00003706
		public void SetCurrentSelectedCulture(BasicCultureObject culture)
		{
			this.IsLocked = false;
			this._culture = culture;
			this.PopulateTroopTypes();
		}

		// Token: 0x06000039 RID: 57 RVA: 0x0000551C File Offset: 0x0000371C
		public void ExecuteRandomize(int compositionValue)
		{
			this.IsValid = true;
			this.IsLocked = false;
			this.CompositionValue = compositionValue;
			this.IsValid = this.TroopTypes.Count > 0;
			this.TroopTypes.ApplyActionOnAllItems(delegate(CustomBattleTroopTypeVM x)
			{
				x.ExecuteRandomize();
			});
			if (!this.TroopTypes.Any((CustomBattleTroopTypeVM x) => x.IsSelected) && this.IsValid)
			{
				this.TroopTypes[0].IsSelected = true;
			}
		}

		// Token: 0x0600003A RID: 58 RVA: 0x000055C4 File Offset: 0x000037C4
		public void ExecuteAddTroopTypes()
		{
			string text = GameTexts.FindText("str_custom_battle_choose_troop", this._type.ToString()).ToString();
			TroopTypeSelectionPopUpVM troopTypeSelectionPopUp = this._troopTypeSelectionPopUp;
			if (troopTypeSelectionPopUp == null)
			{
				return;
			}
			troopTypeSelectionPopUp.OpenPopUp(text, this.TroopTypes);
		}

		// Token: 0x0600003B RID: 59 RVA: 0x00005609 File Offset: 0x00003809
		public void RefreshCompositionValue()
		{
			base.OnPropertyChanged("CompositionValue");
		}

		// Token: 0x0600003C RID: 60 RVA: 0x00005616 File Offset: 0x00003816
		private void OnValidityChanged(bool value)
		{
			this.IsLocked = false;
			if (!value)
			{
				this.CompositionValue = 0;
			}
			this.IsLocked = !value;
		}

		// Token: 0x0600003D RID: 61 RVA: 0x00005634 File Offset: 0x00003834
		private void PopulateTroopTypes()
		{
			this.TroopTypes.Clear();
			MBReadOnlyList<BasicCharacterObject> defaultCharacters = this.GetDefaultCharacters();
			foreach (BasicCharacterObject basicCharacterObject in this._allCharacterObjects)
			{
				if (this.IsValidUnitItem(basicCharacterObject))
				{
					this.TroopTypes.Add(new CustomBattleTroopTypeVM(basicCharacterObject, new Action<CustomBattleTroopTypeVM>(this._troopTypeSelectionPopUp.OnItemSelectionToggled), this._typeIconData, this._allSkills, defaultCharacters.Contains(basicCharacterObject)));
				}
			}
			this.IsValid = this.TroopTypes.Count > 0;
			if (this.IsValid)
			{
				if (!this.TroopTypes.Any((CustomBattleTroopTypeVM x) => x.IsDefault))
				{
					this.TroopTypes[0].IsDefault = true;
				}
			}
			this.TroopTypes.ApplyActionOnAllItems(delegate(CustomBattleTroopTypeVM x)
			{
				x.IsSelected = x.IsDefault;
			});
		}

		// Token: 0x0600003E RID: 62 RVA: 0x00005758 File Offset: 0x00003958
		private bool IsValidUnitItem(BasicCharacterObject o)
		{
			if (o == null || this._culture != o.Culture)
			{
				return false;
			}
			switch (this._type)
			{
			case ArmyCompositionItemVM.CompositionType.MeleeInfantry:
				return o.DefaultFormationClass == null || o.DefaultFormationClass == 5;
			case ArmyCompositionItemVM.CompositionType.RangedInfantry:
				return o.DefaultFormationClass == 1;
			case ArmyCompositionItemVM.CompositionType.MeleeCavalry:
				return o.DefaultFormationClass == 2 || o.DefaultFormationClass == 7 || o.DefaultFormationClass == 6;
			case ArmyCompositionItemVM.CompositionType.RangedCavalry:
				return o.DefaultFormationClass == 3;
			default:
				return false;
			}
		}

		// Token: 0x0600003F RID: 63 RVA: 0x000057E0 File Offset: 0x000039E0
		private MBReadOnlyList<BasicCharacterObject> GetDefaultCharacters()
		{
			MBList<BasicCharacterObject> mblist = new MBList<BasicCharacterObject>();
			FormationClass formationClass = 10;
			switch (this._type)
			{
			case ArmyCompositionItemVM.CompositionType.MeleeInfantry:
				formationClass = 0;
				break;
			case ArmyCompositionItemVM.CompositionType.RangedInfantry:
				formationClass = 1;
				break;
			case ArmyCompositionItemVM.CompositionType.MeleeCavalry:
				formationClass = 2;
				break;
			case ArmyCompositionItemVM.CompositionType.RangedCavalry:
				formationClass = 3;
				break;
			}
			mblist.Add(CustomBattleHelper.GetDefaultTroopOfFormationForFaction(this._culture, formationClass));
			return mblist;
		}

		// Token: 0x06000040 RID: 64 RVA: 0x00005838 File Offset: 0x00003A38
		public static StringItemWithHintVM GetTroopTypeIconData(ArmyCompositionItemVM.CompositionType type, bool isBig = false)
		{
			TextObject textObject = TextObject.Empty;
			string text;
			switch (type)
			{
			case ArmyCompositionItemVM.CompositionType.MeleeInfantry:
				text = (isBig ? "infantry_big" : "infantry");
				textObject = GameTexts.FindText("str_troop_type_name", "Infantry");
				break;
			case ArmyCompositionItemVM.CompositionType.RangedInfantry:
				text = (isBig ? "bow_big" : "bow");
				textObject = GameTexts.FindText("str_troop_type_name", "Ranged");
				break;
			case ArmyCompositionItemVM.CompositionType.MeleeCavalry:
				text = (isBig ? "cavalry_big" : "cavalry");
				textObject = GameTexts.FindText("str_troop_type_name", "Cavalry");
				break;
			case ArmyCompositionItemVM.CompositionType.RangedCavalry:
				text = (isBig ? "horse_archer_big" : "horse_archer");
				textObject = GameTexts.FindText("str_troop_type_name", "HorseArcher");
				break;
			default:
				return new StringItemWithHintVM("", TextObject.Empty);
			}
			return new StringItemWithHintVM("General\\TroopTypeIcons\\icon_troop_type_" + text, new TextObject("{=!}" + textObject.ToString(), null));
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000041 RID: 65 RVA: 0x00005924 File Offset: 0x00003B24
		// (set) Token: 0x06000042 RID: 66 RVA: 0x0000592C File Offset: 0x00003B2C
		[DataSourceProperty]
		public MBBindingList<CustomBattleTroopTypeVM> TroopTypes
		{
			get
			{
				return this._troopTypes;
			}
			set
			{
				if (value != this._troopTypes)
				{
					this._troopTypes = value;
					base.OnPropertyChangedWithValue<MBBindingList<CustomBattleTroopTypeVM>>(value, "TroopTypes");
				}
			}
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000043 RID: 67 RVA: 0x0000594A File Offset: 0x00003B4A
		// (set) Token: 0x06000044 RID: 68 RVA: 0x00005952 File Offset: 0x00003B52
		[DataSourceProperty]
		public HintViewModel InvalidHint
		{
			get
			{
				return this._invalidHint;
			}
			set
			{
				if (value != this._invalidHint)
				{
					this._invalidHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "InvalidHint");
				}
			}
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000045 RID: 69 RVA: 0x00005970 File Offset: 0x00003B70
		// (set) Token: 0x06000046 RID: 70 RVA: 0x00005978 File Offset: 0x00003B78
		[DataSourceProperty]
		public HintViewModel AddTroopTypeHint
		{
			get
			{
				return this._addTroopTypeHint;
			}
			set
			{
				if (value != this._addTroopTypeHint)
				{
					this._addTroopTypeHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "AddTroopTypeHint");
				}
			}
		}

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000047 RID: 71 RVA: 0x00005996 File Offset: 0x00003B96
		// (set) Token: 0x06000048 RID: 72 RVA: 0x0000599E File Offset: 0x00003B9E
		[DataSourceProperty]
		public bool IsLocked
		{
			get
			{
				return this._isLocked;
			}
			set
			{
				if (value != this._isLocked)
				{
					this._isLocked = value;
					base.OnPropertyChangedWithValue(value, "IsLocked");
				}
			}
		}

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000049 RID: 73 RVA: 0x000059BC File Offset: 0x00003BBC
		// (set) Token: 0x0600004A RID: 74 RVA: 0x000059C4 File Offset: 0x00003BC4
		[DataSourceProperty]
		public bool IsValid
		{
			get
			{
				return this._isValid;
			}
			set
			{
				if (value != this._isValid)
				{
					this._isValid = value;
					base.OnPropertyChangedWithValue(value, "IsValid");
				}
				this.OnValidityChanged(value);
			}
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x0600004B RID: 75 RVA: 0x000059E9 File Offset: 0x00003BE9
		// (set) Token: 0x0600004C RID: 76 RVA: 0x000059F8 File Offset: 0x00003BF8
		[DataSourceProperty]
		public int CompositionValue
		{
			get
			{
				return this._compositionValues[(int)this._type];
			}
			set
			{
				if (value != this._compositionValues[(int)this._type])
				{
					this._onCompositionValueChanged(value, (int)this._type);
				}
			}
		}

		// Token: 0x0400003A RID: 58
		private readonly MBReadOnlyList<SkillObject> _allSkills;

		// Token: 0x0400003B RID: 59
		private readonly List<BasicCharacterObject> _allCharacterObjects;

		// Token: 0x0400003C RID: 60
		private readonly Action<int, int> _onCompositionValueChanged;

		// Token: 0x0400003D RID: 61
		private readonly TroopTypeSelectionPopUpVM _troopTypeSelectionPopUp;

		// Token: 0x0400003E RID: 62
		private BasicCultureObject _culture;

		// Token: 0x0400003F RID: 63
		private readonly StringItemWithHintVM _typeIconData;

		// Token: 0x04000040 RID: 64
		private readonly ArmyCompositionItemVM.CompositionType _type;

		// Token: 0x04000041 RID: 65
		private readonly int[] _compositionValues;

		// Token: 0x04000042 RID: 66
		private MBBindingList<CustomBattleTroopTypeVM> _troopTypes;

		// Token: 0x04000043 RID: 67
		private HintViewModel _invalidHint;

		// Token: 0x04000044 RID: 68
		private HintViewModel _addTroopTypeHint;

		// Token: 0x04000045 RID: 69
		private bool _isLocked;

		// Token: 0x04000046 RID: 70
		private bool _isValid;

		// Token: 0x02000031 RID: 49
		public enum CompositionType
		{
			// Token: 0x0400014F RID: 335
			MeleeInfantry,
			// Token: 0x04000150 RID: 336
			RangedInfantry,
			// Token: 0x04000151 RID: 337
			MeleeCavalry,
			// Token: 0x04000152 RID: 338
			RangedCavalry
		}
	}
}
