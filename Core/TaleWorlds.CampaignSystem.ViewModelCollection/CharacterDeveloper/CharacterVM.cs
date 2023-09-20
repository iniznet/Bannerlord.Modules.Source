using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.ViewModelCollection.CharacterDeveloper.PerkSelection;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.Items;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.CharacterDeveloper
{
	public class CharacterVM : ViewModel
	{
		public IHeroDeveloper GetCharacterDeveloper()
		{
			Hero hero = this.Hero;
			if (hero == null)
			{
				return null;
			}
			return hero.HeroDeveloper;
		}

		public Hero Hero { get; private set; }

		public int OrgUnspentFocusPoints { get; private set; }

		public int OrgUnspentAttributePoints { get; private set; }

		public CharacterVM(Hero hero, Action onPerkSelection)
		{
			this.LevelHint = new HintViewModel();
			this.Hero = hero;
			this.OrgUnspentFocusPoints = this.GetCharacterDeveloper().UnspentFocusPoints;
			this.UnspentCharacterPoints = this.OrgUnspentFocusPoints;
			this.OrgUnspentAttributePoints = this.GetCharacterDeveloper().UnspentAttributePoints;
			this.UnspentAttributePoints = this.OrgUnspentAttributePoints;
			this._developer = this.GetCharacterDeveloper();
			this.Attributes = new MBBindingList<CharacterAttributeItemVM>();
			this.PerkSelection = new PerkSelectionVM(this._developer, new Action<SkillObject>(this.RefreshPerksOfSkill), onPerkSelection);
			this.InitializeCharacter();
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			StringHelpers.SetCharacterProperties("HERO", this.Hero.CharacterObject, null, false);
			this.HeroNameText = this.Hero.CharacterObject.Name.ToString();
			MBTextManager.SetTextVariable("LEVEL", this.Hero.CharacterObject.Level);
			this.HeroLevelText = GameTexts.FindText("str_level_with_value", null).ToString();
			this.HeroInfoText = GameTexts.FindText("str_hero_name_level", null).ToString();
			this.FocusPointsText = GameTexts.FindText("str_focus_points", null).ToString();
			this.InitializeCharacter();
			this.Skills.ApplyActionOnAllItems(delegate(SkillVM x)
			{
				x.RefreshValues();
			});
			this.CurrentSkill.RefreshValues();
		}

		private void InitializeCharacter()
		{
			this.HeroCharacter = new HeroViewModel(CharacterViewModel.StanceTypes.None);
			this.Skills = new MBBindingList<SkillVM>();
			this.Traits = new MBBindingList<EncyclopediaTraitItemVM>();
			this.Attributes.Clear();
			this.HeroCharacter.FillFrom(this.Hero, -1, false, false);
			this.HeroCharacter.SetEquipment(EquipmentIndex.ArmorItemEndSlot, default(EquipmentElement));
			this.HeroCharacter.SetEquipment(EquipmentIndex.HorseHarness, default(EquipmentElement));
			this.HeroCharacter.SetEquipment(EquipmentIndex.NumAllWeaponSlots, default(EquipmentElement));
			foreach (CharacterAttribute characterAttribute in TaleWorlds.CampaignSystem.Extensions.Attributes.All)
			{
				CharacterAttributeItemVM characterAttributeItemVM = new CharacterAttributeItemVM(this.Hero, characterAttribute, this, new Action<CharacterAttributeItemVM>(this.OnInspectAttribute), new Action<CharacterAttributeItemVM>(this.OnAddAttributePoint));
				this.Attributes.Add(characterAttributeItemVM);
				foreach (SkillObject skillObject in characterAttribute.Skills)
				{
					this.Skills.Add(new SkillVM(skillObject, this, new Action<PerkVM>(this.OnStartPerkSelection)));
				}
			}
			using (List<SkillObject>.Enumerator enumerator2 = TaleWorlds.CampaignSystem.Extensions.Skills.All.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					SkillObject skill = enumerator2.Current;
					if (this.Skills.All((SkillVM s) => s.Skill != skill))
					{
						this.Skills.Add(new SkillVM(skill, this, new Action<PerkVM>(this.OnStartPerkSelection)));
					}
				}
			}
			foreach (SkillVM skillVM in this.Skills)
			{
				skillVM.RefreshWithCurrentValues();
			}
			foreach (CharacterAttributeItemVM characterAttributeItemVM2 in this.Attributes)
			{
				characterAttributeItemVM2.RefreshWithCurrentValues();
			}
			this.SetCurrentSkill(this.Skills[0]);
			this.RefreshCharacterValues();
			this.CharacterStats = new MBBindingList<StringPairItemVM>();
			if (this.Hero.GovernorOf != null)
			{
				GameTexts.SetVariable("SETTLEMENT_NAME", this.Hero.GovernorOf.Name.ToString());
				this.CharacterStats.Add(new StringPairItemVM(GameTexts.FindText("str_governor_of_label", null).ToString(), "", null));
			}
			if (MobileParty.MainParty.GetHeroPerkRole(this.Hero) != SkillEffect.PerkRole.None)
			{
				this.CharacterStats.Add(new StringPairItemVM(CampaignUIHelper.GetHeroClanRoleText(this.Hero, Clan.PlayerClan), "", null));
			}
			foreach (TraitObject traitObject in CampaignUIHelper.GetHeroTraits())
			{
				if (this.Hero.GetTraitLevel(traitObject) != 0)
				{
					this.Traits.Add(new EncyclopediaTraitItemVM(traitObject, this.Hero));
				}
			}
		}

		private void OnInspectAttribute(CharacterAttributeItemVM att)
		{
			this.CurrentInspectedAttribute = att;
			this.IsInspectingAnAttribute = true;
		}

		private void OnAddAttributePoint(CharacterAttributeItemVM att)
		{
			int unspentAttributePoints = this.UnspentAttributePoints;
			this.UnspentAttributePoints = unspentAttributePoints - 1;
			this.RefreshCharacterValues();
		}

		public void ExecuteStopInspectingCurrentAttribute()
		{
			this.IsInspectingAnAttribute = false;
			this.CurrentInspectedAttribute = null;
		}

		public void RefreshCharacterValues()
		{
			this.CurrentTotalSkill = this.GetCharacterDeveloper().TotalXp - this.GetCharacterDeveloper().GetXpRequiredForLevel(this.Hero.CharacterObject.Level);
			this.SkillPointsRequiredForNextLevel = this.GetCharacterDeveloper().GetXpRequiredForLevel(this.Hero.CharacterObject.Level + 1) - this.GetCharacterDeveloper().GetXpRequiredForLevel(this.Hero.CharacterObject.Level);
			GameTexts.SetVariable("CURRENTAMOUNT", this.CurrentTotalSkill);
			GameTexts.SetVariable("TARGETAMOUNT", this.SkillPointsRequiredForNextLevel);
			this.LevelProgressText = GameTexts.FindText("str_character_skillpoint_progress", null).ToString();
			GameTexts.SetVariable("newline", "\n");
			GameTexts.SetVariable("CURRENT_SKILL_POINTS", this.CurrentTotalSkill);
			GameTexts.SetVariable("STR1", GameTexts.FindText("str_total_skill_points", null));
			GameTexts.SetVariable("NEXT_SKILL_POINTS", this.SkillPointsRequiredForNextLevel);
			GameTexts.SetVariable("STR2", GameTexts.FindText("str_next_level_at", null));
			string text = GameTexts.FindText("str_string_newline_string", null).ToString();
			GameTexts.SetVariable("SKILL_LEVEL_FOR_LEVEL_UP", this.SkillPointsRequiredForNextLevel - this.CurrentTotalSkill);
			GameTexts.SetVariable("STR1", text);
			GameTexts.SetVariable("STR2", GameTexts.FindText("str_how_to_level_up_character", null));
			string text2 = GameTexts.FindText("str_string_newline_string", null).ToString();
			this.LevelHint.HintText = new TextObject("{=!}" + text2, null);
			foreach (SkillVM skillVM in this.Skills)
			{
				skillVM.RefreshWithCurrentValues();
			}
			foreach (CharacterAttributeItemVM characterAttributeItemVM in this.Attributes)
			{
				characterAttributeItemVM.RefreshWithCurrentValues();
			}
		}

		public void RefreshPerksOfSkill(SkillObject skill)
		{
			SkillVM skillVM = this.Skills.SingleOrDefault((SkillVM s) => s.Skill == skill);
			if (skillVM == null)
			{
				return;
			}
			skillVM.RefreshLists(null);
		}

		public void ResetChanges(bool isCancel)
		{
			this.PerkSelection.ResetSelectedPerks();
			if (!isCancel)
			{
				this.UnspentCharacterPoints = this.OrgUnspentFocusPoints;
				this.UnspentAttributePoints = this.OrgUnspentAttributePoints;
			}
			foreach (CharacterAttributeItemVM characterAttributeItemVM in this.Attributes)
			{
				characterAttributeItemVM.Reset();
			}
			if (!isCancel)
			{
				foreach (CharacterAttributeItemVM characterAttributeItemVM2 in this.Attributes)
				{
					characterAttributeItemVM2.RefreshWithCurrentValues();
				}
			}
			foreach (SkillVM skillVM in this.Skills)
			{
				skillVM.ResetChanges();
			}
			if (!isCancel)
			{
				foreach (SkillVM skillVM2 in this.Skills)
				{
					skillVM2.RefreshWithCurrentValues();
				}
			}
		}

		public void ApplyChanges()
		{
			this.PerkSelection.ApplySelectedPerks();
			foreach (CharacterAttributeItemVM characterAttributeItemVM in this.Attributes)
			{
				characterAttributeItemVM.Commit();
			}
			foreach (SkillVM skillVM in this.Skills)
			{
				skillVM.ApplyChanges();
			}
		}

		public void SetCurrentSkill(SkillVM skill)
		{
			if (this.CurrentSkill != null)
			{
				this.CurrentSkill.IsInspected = false;
			}
			this.CurrentSkill = skill;
			this.CurrentSkill.IsInspected = true;
		}

		public bool IsThereAnyChanges()
		{
			bool flag = this.Skills.Any((SkillVM s) => s.IsThereAnyChanges());
			return this.UnspentCharacterPoints != this.OrgUnspentFocusPoints || this.UnspentAttributePoints != this.OrgUnspentAttributePoints || this.PerkSelection.IsAnyPerkSelected() || flag;
		}

		public int GetRequiredFocusPointsToAddFocusWithCurrentFocus(SkillObject skill)
		{
			return this.Hero.HeroDeveloper.GetRequiredFocusPointsToAddFocus(skill);
		}

		public bool CanAddFocusToSkillWithFocusAmount(int currentFocusAmount)
		{
			return currentFocusAmount < Campaign.Current.Models.CharacterDevelopmentModel.MaxFocusPerSkill && this.UnspentCharacterPoints > 0;
		}

		public bool IsSkillMaxAmongOtherSkills(SkillVM skill)
		{
			if (this.Skills.Count > 0)
			{
				int currentFocusLevel = skill.CurrentFocusLevel;
				return this.Skills.Max((SkillVM s) => s.CurrentFocusLevel) <= currentFocusLevel;
			}
			return false;
		}

		public string GetNameWithNumOfUnopenedPerks()
		{
			if (this.Skills.Sum((SkillVM s) => s.NumOfUnopenedPerks) == 0)
			{
				return this.HeroNameText;
			}
			GameTexts.SetVariable("STR1", this.HeroNameText);
			GameTexts.SetVariable("STR2", "{=!}<img src=\"CharacterDeveloper\\UnselectedPerksIcon\" extend=\"2\">");
			return GameTexts.FindText("str_STR1_space_STR2", null).ToString();
		}

		public int GetCurrentAttributePoint(CharacterAttribute attribute)
		{
			if (this.Attributes.Count > 0)
			{
				return this.Attributes.Single((CharacterAttributeItemVM a) => a.AttributeType == attribute).AttributeValue;
			}
			return 0;
		}

		private void OnStartPerkSelection(PerkVM perk)
		{
			this.PerkSelection.SetCurrentSelectionPerk(perk);
		}

		public int GetNumberOfUnselectedPerks()
		{
			return this.Skills.Sum((SkillVM s) => s.NumOfUnopenedPerks);
		}

		public override void OnFinalize()
		{
			base.OnFinalize();
			this.HeroCharacter.OnFinalize();
		}

		[DataSourceProperty]
		public MBBindingList<SkillVM> Skills
		{
			get
			{
				return this._skills;
			}
			set
			{
				if (value != this._skills)
				{
					this._skills = value;
					base.OnPropertyChangedWithValue<MBBindingList<SkillVM>>(value, "Skills");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<StringPairItemVM> CharacterStats
		{
			get
			{
				return this._characterStats;
			}
			set
			{
				if (value != this._characterStats)
				{
					this._characterStats = value;
					base.OnPropertyChangedWithValue<MBBindingList<StringPairItemVM>>(value, "CharacterStats");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<CharacterAttributeItemVM> Attributes
		{
			get
			{
				return this._attributes;
			}
			set
			{
				if (value != this._attributes)
				{
					this._attributes = value;
					base.OnPropertyChangedWithValue<MBBindingList<CharacterAttributeItemVM>>(value, "Attributes");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<EncyclopediaTraitItemVM> Traits
		{
			get
			{
				return this._traits;
			}
			set
			{
				if (value != this._traits)
				{
					this._traits = value;
					base.OnPropertyChangedWithValue<MBBindingList<EncyclopediaTraitItemVM>>(value, "Traits");
				}
			}
		}

		[DataSourceProperty]
		public PerkSelectionVM PerkSelection
		{
			get
			{
				return this._perkSelection;
			}
			set
			{
				if (value != this._perkSelection)
				{
					this._perkSelection = value;
					base.OnPropertyChangedWithValue<PerkSelectionVM>(value, "PerkSelection");
				}
			}
		}

		[DataSourceProperty]
		public SkillVM CurrentSkill
		{
			get
			{
				return this._currentSkill;
			}
			set
			{
				if (value != this._currentSkill)
				{
					this._currentSkill = value;
					base.OnPropertyChangedWithValue<SkillVM>(value, "CurrentSkill");
				}
			}
		}

		[DataSourceProperty]
		public CharacterAttributeItemVM CurrentInspectedAttribute
		{
			get
			{
				return this._currentInspectedAttribute;
			}
			set
			{
				if (value != this._currentInspectedAttribute)
				{
					this._currentInspectedAttribute = value;
					base.OnPropertyChangedWithValue<CharacterAttributeItemVM>(value, "CurrentInspectedAttribute");
				}
			}
		}

		[DataSourceProperty]
		public string FocusPointsText
		{
			get
			{
				return this._focusPointsText;
			}
			set
			{
				if (value != this._focusPointsText)
				{
					this._focusPointsText = value;
					base.OnPropertyChangedWithValue<string>(value, "FocusPointsText");
				}
			}
		}

		[DataSourceProperty]
		public string LevelProgressText
		{
			get
			{
				return this._levelProgressText;
			}
			set
			{
				if (value != this._levelProgressText)
				{
					this._levelProgressText = value;
					base.OnPropertyChangedWithValue<string>(value, "LevelProgressText");
				}
			}
		}

		[DataSourceProperty]
		public HeroViewModel HeroCharacter
		{
			get
			{
				return this._heroCharacter;
			}
			set
			{
				if (value != this._heroCharacter)
				{
					this._heroCharacter = value;
					base.OnPropertyChangedWithValue<HeroViewModel>(value, "HeroCharacter");
				}
			}
		}

		[DataSourceProperty]
		public bool IsInspectingAnAttribute
		{
			get
			{
				return this._isInspectingAnAttribute;
			}
			set
			{
				if (value != this._isInspectingAnAttribute)
				{
					this._isInspectingAnAttribute = value;
					base.OnPropertyChangedWithValue(value, "IsInspectingAnAttribute");
				}
			}
		}

		[DataSourceProperty]
		public int LevelProgressPercentage
		{
			get
			{
				return this._levelProgressPercentage;
			}
			set
			{
				if (value != this._levelProgressPercentage)
				{
					this._levelProgressPercentage = value;
					base.OnPropertyChangedWithValue(value, "LevelProgressPercentage");
				}
			}
		}

		[DataSourceProperty]
		public int CurrentTotalSkill
		{
			get
			{
				return this._currentTotalSkill;
			}
			set
			{
				if (value != this._currentTotalSkill)
				{
					this._currentTotalSkill = value;
					base.OnPropertyChangedWithValue(value, "CurrentTotalSkill");
				}
			}
		}

		[DataSourceProperty]
		public int SkillPointsRequiredForCurrentLevel
		{
			get
			{
				return this._skillPointsRequiredForCurrentLevel;
			}
			set
			{
				if (value != this._skillPointsRequiredForCurrentLevel)
				{
					this._skillPointsRequiredForCurrentLevel = value;
					base.OnPropertyChangedWithValue(value, "SkillPointsRequiredForCurrentLevel");
				}
			}
		}

		[DataSourceProperty]
		public int SkillPointsRequiredForNextLevel
		{
			get
			{
				return this._skillPointsRequiredForNextLevel;
			}
			set
			{
				if (value != this._skillPointsRequiredForNextLevel)
				{
					this._skillPointsRequiredForNextLevel = value;
					base.OnPropertyChangedWithValue(value, "SkillPointsRequiredForNextLevel");
				}
			}
		}

		[DataSourceProperty]
		public int UnspentCharacterPoints
		{
			get
			{
				return this._unspentCharacterPoints;
			}
			set
			{
				if (value != this._unspentCharacterPoints)
				{
					this._unspentCharacterPoints = value;
					base.OnPropertyChangedWithValue(value, "UnspentCharacterPoints");
				}
			}
		}

		[DataSourceProperty]
		public int UnspentAttributePoints
		{
			get
			{
				return this._unspentAttributePoints;
			}
			set
			{
				if (value != this._unspentAttributePoints)
				{
					this._unspentAttributePoints = value;
					base.OnPropertyChangedWithValue(value, "UnspentAttributePoints");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel LevelHint
		{
			get
			{
				return this._levelHint;
			}
			set
			{
				if (value != this._levelHint)
				{
					this._levelHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "LevelHint");
				}
			}
		}

		[DataSourceProperty]
		public string HeroNameText
		{
			get
			{
				return this._heroNameText;
			}
			set
			{
				if (value != this._heroNameText)
				{
					this._heroNameText = value;
					base.OnPropertyChangedWithValue<string>(value, "HeroNameText");
				}
			}
		}

		[DataSourceProperty]
		public string HeroInfoText
		{
			get
			{
				return this._heroInfoText;
			}
			set
			{
				if (value != this._heroInfoText)
				{
					this._heroInfoText = value;
					base.OnPropertyChangedWithValue<string>(value, "HeroInfoText");
				}
			}
		}

		[DataSourceProperty]
		public string HeroLevelText
		{
			get
			{
				return this._heroLevelText;
			}
			set
			{
				if (value != this._heroLevelText)
				{
					this._heroLevelText = value;
					base.OnPropertyChangedWithValue<string>(value, "HeroLevelText");
				}
			}
		}

		private readonly IHeroDeveloper _developer;

		private MBBindingList<SkillVM> _skills;

		private PerkSelectionVM _perkSelection;

		private HeroViewModel _heroCharacter;

		private int _skillPointsRequiredForCurrentLevel;

		private int _skillPointsRequiredForNextLevel;

		private int _currentTotalSkill;

		private int _levelProgressPercentage;

		private int _unspentCharacterPoints;

		private int _unspentAttributePoints;

		private string _levelProgressText;

		private string _heroNameText;

		private string _heroInfoText;

		private bool _isInspectingAnAttribute;

		private HintViewModel _levelHint;

		private SkillVM _currentSkill;

		private CharacterAttributeItemVM _currentInspectedAttribute;

		private string _heroLevelText;

		private string _focusPointsText;

		private MBBindingList<StringPairItemVM> _characterStats;

		private MBBindingList<CharacterAttributeItemVM> _attributes;

		private MBBindingList<EncyclopediaTraitItemVM> _traits;
	}
}
