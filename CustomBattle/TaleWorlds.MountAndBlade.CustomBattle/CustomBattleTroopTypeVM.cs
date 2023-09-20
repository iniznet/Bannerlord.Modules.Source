using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.CustomBattle
{
	public class CustomBattleTroopTypeVM : ViewModel
	{
		public BasicCharacterObject Character { get; private set; }

		public CustomBattleTroopTypeVM(BasicCharacterObject character, Action<CustomBattleTroopTypeVM> onSelectionToggled, StringItemWithHintVM typeIconData, MBReadOnlyList<SkillObject> allSkills, bool isDefault)
		{
			this.Character = character;
			this.IsDefault = isDefault;
			this._onSelectionToggled = onSelectionToggled;
			this._allSkills = allSkills;
			if (character != null)
			{
				this.Visual = new ImageIdentifierVM(CharacterCode.CreateFrom(character));
				this.NameHint = new HintViewModel(character.Name, null);
				this.TroopSkillsHint = new BasicTooltipViewModel(() => this.GetTroopSkillsTooltip(this.Character));
				this.TierIconData = CustomBattleTroopTypeVM.GetCharacterTierData(this.Character, false);
				this.TypeIconData = typeIconData;
			}
			else
			{
				Debug.FailedAssert("Character shouldn't be null", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.CustomBattle\\CustomBattle\\CustomBattleTroopTypeVM.cs", ".ctor", 39);
			}
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			BasicCharacterObject character = this.Character;
			this.Name = ((character != null) ? character.Name.ToString() : null);
		}

		public void ExecuteToggleSelection()
		{
			Action<CustomBattleTroopTypeVM> onSelectionToggled = this._onSelectionToggled;
			if (onSelectionToggled == null)
			{
				return;
			}
			onSelectionToggled(this);
		}

		public void ExecuteRandomize()
		{
			this.IsSelected = MBRandom.RandomInt(2) == 1;
		}

		private List<TooltipProperty> GetTroopSkillsTooltip(BasicCharacterObject character)
		{
			List<TooltipProperty> list = new List<TooltipProperty>();
			list.Add(new TooltipProperty("", character.Name.ToString(), 1, false, 4096));
			list.Add(new TooltipProperty(GameTexts.FindText("str_skills", null).ToString(), " ", 0, false, 0));
			list.Add(new TooltipProperty("", "", 0, false, 512));
			foreach (SkillObject skillObject in this._allSkills)
			{
				int skillValue = character.GetSkillValue(skillObject);
				if (skillValue > 0)
				{
					list.Add(new TooltipProperty(skillObject.Name.ToString(), skillValue.ToString(), 0, false, 0));
				}
			}
			return list;
		}

		public static StringItemWithHintVM GetCharacterTierData(BasicCharacterObject character, bool isBig = false)
		{
			int characterTier = CustomBattleTroopTypeVM.GetCharacterTier(character);
			if (characterTier <= 0 || characterTier > 7)
			{
				return new StringItemWithHintVM("", TextObject.Empty);
			}
			string text = (isBig ? (characterTier.ToString() + "_big") : characterTier.ToString());
			string text2 = "General\\TroopTierIcons\\icon_tier_" + text;
			GameTexts.SetVariable("TIER_LEVEL", characterTier);
			TextObject textObject = new TextObject("{=!}" + GameTexts.FindText("str_party_troop_tier", null).ToString(), null);
			return new StringItemWithHintVM(text2, textObject);
		}

		public static int GetCharacterTier(BasicCharacterObject character)
		{
			if (character.IsHero)
			{
				return 0;
			}
			return MathF.Min(MathF.Max(MathF.Ceiling(((float)character.Level - 5f) / 5f), 0), 7);
		}

		[DataSourceProperty]
		public ImageIdentifierVM Visual
		{
			get
			{
				return this._visual;
			}
			set
			{
				if (value != this._visual)
				{
					this._visual = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "Visual");
				}
			}
		}

		[DataSourceProperty]
		public BasicTooltipViewModel TroopSkillsHint
		{
			get
			{
				return this._troopSkillsHint;
			}
			set
			{
				if (value != this._troopSkillsHint)
				{
					this._troopSkillsHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "TroopSkillsHint");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel NameHint
		{
			get
			{
				return this._nameHint;
			}
			set
			{
				if (value != this._nameHint)
				{
					this._nameHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "NameHint");
				}
			}
		}

		[DataSourceProperty]
		public StringItemWithHintVM TierIconData
		{
			get
			{
				return this._tierIconData;
			}
			set
			{
				if (value != this._tierIconData)
				{
					this._tierIconData = value;
					base.OnPropertyChangedWithValue<StringItemWithHintVM>(value, "TierIconData");
				}
			}
		}

		[DataSourceProperty]
		public StringItemWithHintVM TypeIconData
		{
			get
			{
				return this._typeIconData;
			}
			set
			{
				if (value != this._typeIconData)
				{
					this._typeIconData = value;
					base.OnPropertyChangedWithValue<StringItemWithHintVM>(value, "TypeIconData");
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
		public bool IsSelected
		{
			get
			{
				return this._isSelected;
			}
			set
			{
				if (value != this._isSelected)
				{
					this._isSelected = value;
					base.OnPropertyChangedWithValue(value, "IsSelected");
				}
			}
		}

		public bool IsDefault;

		private readonly Action<CustomBattleTroopTypeVM> _onSelectionToggled;

		private readonly MBReadOnlyList<SkillObject> _allSkills;

		private ImageIdentifierVM _visual;

		private BasicTooltipViewModel _troopSkillsHint;

		private HintViewModel _nameHint;

		private StringItemWithHintVM _tierIconData;

		private StringItemWithHintVM _typeIconData;

		private string _name;

		private bool _isSelected;
	}
}
