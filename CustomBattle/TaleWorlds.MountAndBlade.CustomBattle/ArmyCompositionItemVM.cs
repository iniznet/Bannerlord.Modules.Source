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
	public class ArmyCompositionItemVM : ViewModel
	{
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

		public override void RefreshValues()
		{
			base.RefreshValues();
		}

		public void SetCurrentSelectedCulture(BasicCultureObject culture)
		{
			this.IsLocked = false;
			this._culture = culture;
			this.PopulateTroopTypes();
		}

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

		public void RefreshCompositionValue()
		{
			base.OnPropertyChanged("CompositionValue");
		}

		private void OnValidityChanged(bool value)
		{
			this.IsLocked = false;
			if (!value)
			{
				this.CompositionValue = 0;
			}
			this.IsLocked = !value;
		}

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

		private readonly MBReadOnlyList<SkillObject> _allSkills;

		private readonly List<BasicCharacterObject> _allCharacterObjects;

		private readonly Action<int, int> _onCompositionValueChanged;

		private readonly TroopTypeSelectionPopUpVM _troopTypeSelectionPopUp;

		private BasicCultureObject _culture;

		private readonly StringItemWithHintVM _typeIconData;

		private readonly ArmyCompositionItemVM.CompositionType _type;

		private readonly int[] _compositionValues;

		private MBBindingList<CustomBattleTroopTypeVM> _troopTypes;

		private HintViewModel _invalidHint;

		private HintViewModel _addTroopTypeHint;

		private bool _isLocked;

		private bool _isValid;

		public enum CompositionType
		{
			MeleeInfantry,
			RangedInfantry,
			MeleeCavalry,
			RangedCavalry
		}
	}
}
