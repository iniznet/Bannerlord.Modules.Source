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
	// Token: 0x02000119 RID: 281
	public class CharacterVM : ViewModel
	{
		// Token: 0x06001B1E RID: 6942 RVA: 0x00062165 File Offset: 0x00060365
		public IHeroDeveloper GetCharacterDeveloper()
		{
			Hero hero = this.Hero;
			if (hero == null)
			{
				return null;
			}
			return hero.HeroDeveloper;
		}

		// Token: 0x17000947 RID: 2375
		// (get) Token: 0x06001B1F RID: 6943 RVA: 0x00062178 File Offset: 0x00060378
		// (set) Token: 0x06001B20 RID: 6944 RVA: 0x00062180 File Offset: 0x00060380
		public Hero Hero { get; private set; }

		// Token: 0x17000948 RID: 2376
		// (get) Token: 0x06001B21 RID: 6945 RVA: 0x00062189 File Offset: 0x00060389
		// (set) Token: 0x06001B22 RID: 6946 RVA: 0x00062191 File Offset: 0x00060391
		public int OrgUnspentFocusPoints { get; private set; }

		// Token: 0x17000949 RID: 2377
		// (get) Token: 0x06001B23 RID: 6947 RVA: 0x0006219A File Offset: 0x0006039A
		// (set) Token: 0x06001B24 RID: 6948 RVA: 0x000621A2 File Offset: 0x000603A2
		public int OrgUnspentAttributePoints { get; private set; }

		// Token: 0x06001B25 RID: 6949 RVA: 0x000621AC File Offset: 0x000603AC
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

		// Token: 0x06001B26 RID: 6950 RVA: 0x0006224C File Offset: 0x0006044C
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

		// Token: 0x06001B27 RID: 6951 RVA: 0x0006232C File Offset: 0x0006052C
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

		// Token: 0x06001B28 RID: 6952 RVA: 0x000626A0 File Offset: 0x000608A0
		private void OnInspectAttribute(CharacterAttributeItemVM att)
		{
			this.CurrentInspectedAttribute = att;
			this.IsInspectingAnAttribute = true;
		}

		// Token: 0x06001B29 RID: 6953 RVA: 0x000626B0 File Offset: 0x000608B0
		private void OnAddAttributePoint(CharacterAttributeItemVM att)
		{
			int unspentAttributePoints = this.UnspentAttributePoints;
			this.UnspentAttributePoints = unspentAttributePoints - 1;
			this.RefreshCharacterValues();
		}

		// Token: 0x06001B2A RID: 6954 RVA: 0x000626D3 File Offset: 0x000608D3
		public void ExecuteStopInspectingCurrentAttribute()
		{
			this.IsInspectingAnAttribute = false;
			this.CurrentInspectedAttribute = null;
		}

		// Token: 0x06001B2B RID: 6955 RVA: 0x000626E4 File Offset: 0x000608E4
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

		// Token: 0x06001B2C RID: 6956 RVA: 0x000628D8 File Offset: 0x00060AD8
		public void RefreshPerksOfSkill(SkillObject skill)
		{
			SkillVM skillVM = this.Skills.SingleOrDefault((SkillVM s) => s.Skill == skill);
			if (skillVM == null)
			{
				return;
			}
			skillVM.RefreshLists(null);
		}

		// Token: 0x06001B2D RID: 6957 RVA: 0x00062914 File Offset: 0x00060B14
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

		// Token: 0x06001B2E RID: 6958 RVA: 0x00062A38 File Offset: 0x00060C38
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

		// Token: 0x06001B2F RID: 6959 RVA: 0x00062AC8 File Offset: 0x00060CC8
		public void SetCurrentSkill(SkillVM skill)
		{
			if (this.CurrentSkill != null)
			{
				this.CurrentSkill.IsInspected = false;
			}
			this.CurrentSkill = skill;
			this.CurrentSkill.IsInspected = true;
		}

		// Token: 0x06001B30 RID: 6960 RVA: 0x00062AF4 File Offset: 0x00060CF4
		public bool IsThereAnyChanges()
		{
			bool flag = this.Skills.Any((SkillVM s) => s.IsThereAnyChanges());
			return this.UnspentCharacterPoints != this.OrgUnspentFocusPoints || this.UnspentAttributePoints != this.OrgUnspentAttributePoints || this.PerkSelection.IsAnyPerkSelected() || flag;
		}

		// Token: 0x06001B31 RID: 6961 RVA: 0x00062B58 File Offset: 0x00060D58
		public int GetRequiredFocusPointsToAddFocusWithCurrentFocus(SkillObject skill)
		{
			return this.Hero.HeroDeveloper.GetRequiredFocusPointsToAddFocus(skill);
		}

		// Token: 0x06001B32 RID: 6962 RVA: 0x00062B6B File Offset: 0x00060D6B
		public bool CanAddFocusToSkillWithFocusAmount(int currentFocusAmount)
		{
			return currentFocusAmount < Campaign.Current.Models.CharacterDevelopmentModel.MaxFocusPerSkill && this.UnspentCharacterPoints > 0;
		}

		// Token: 0x06001B33 RID: 6963 RVA: 0x00062B90 File Offset: 0x00060D90
		public bool IsSkillMaxAmongOtherSkills(SkillVM skill)
		{
			if (this.Skills.Count > 0)
			{
				int currentFocusLevel = skill.CurrentFocusLevel;
				return this.Skills.Max((SkillVM s) => s.CurrentFocusLevel) <= currentFocusLevel;
			}
			return false;
		}

		// Token: 0x06001B34 RID: 6964 RVA: 0x00062BE4 File Offset: 0x00060DE4
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

		// Token: 0x06001B35 RID: 6965 RVA: 0x00062C54 File Offset: 0x00060E54
		public int GetCurrentAttributePoint(CharacterAttribute attribute)
		{
			if (this.Attributes.Count > 0)
			{
				return this.Attributes.Single((CharacterAttributeItemVM a) => a.AttributeType == attribute).AttributeValue;
			}
			return 0;
		}

		// Token: 0x06001B36 RID: 6966 RVA: 0x00062C9A File Offset: 0x00060E9A
		private void OnStartPerkSelection(PerkVM perk)
		{
			this.PerkSelection.SetCurrentSelectionPerk(perk);
		}

		// Token: 0x06001B37 RID: 6967 RVA: 0x00062CA8 File Offset: 0x00060EA8
		public int GetNumberOfUnselectedPerks()
		{
			return this.Skills.Sum((SkillVM s) => s.NumOfUnopenedPerks);
		}

		// Token: 0x06001B38 RID: 6968 RVA: 0x00062CD4 File Offset: 0x00060ED4
		public override void OnFinalize()
		{
			base.OnFinalize();
			this.HeroCharacter.OnFinalize();
		}

		// Token: 0x1700094A RID: 2378
		// (get) Token: 0x06001B39 RID: 6969 RVA: 0x00062CE7 File Offset: 0x00060EE7
		// (set) Token: 0x06001B3A RID: 6970 RVA: 0x00062CEF File Offset: 0x00060EEF
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

		// Token: 0x1700094B RID: 2379
		// (get) Token: 0x06001B3B RID: 6971 RVA: 0x00062D0D File Offset: 0x00060F0D
		// (set) Token: 0x06001B3C RID: 6972 RVA: 0x00062D15 File Offset: 0x00060F15
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

		// Token: 0x1700094C RID: 2380
		// (get) Token: 0x06001B3D RID: 6973 RVA: 0x00062D33 File Offset: 0x00060F33
		// (set) Token: 0x06001B3E RID: 6974 RVA: 0x00062D3B File Offset: 0x00060F3B
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

		// Token: 0x1700094D RID: 2381
		// (get) Token: 0x06001B3F RID: 6975 RVA: 0x00062D59 File Offset: 0x00060F59
		// (set) Token: 0x06001B40 RID: 6976 RVA: 0x00062D61 File Offset: 0x00060F61
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

		// Token: 0x1700094E RID: 2382
		// (get) Token: 0x06001B41 RID: 6977 RVA: 0x00062D7F File Offset: 0x00060F7F
		// (set) Token: 0x06001B42 RID: 6978 RVA: 0x00062D87 File Offset: 0x00060F87
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

		// Token: 0x1700094F RID: 2383
		// (get) Token: 0x06001B43 RID: 6979 RVA: 0x00062DA5 File Offset: 0x00060FA5
		// (set) Token: 0x06001B44 RID: 6980 RVA: 0x00062DAD File Offset: 0x00060FAD
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

		// Token: 0x17000950 RID: 2384
		// (get) Token: 0x06001B45 RID: 6981 RVA: 0x00062DCB File Offset: 0x00060FCB
		// (set) Token: 0x06001B46 RID: 6982 RVA: 0x00062DD3 File Offset: 0x00060FD3
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

		// Token: 0x17000951 RID: 2385
		// (get) Token: 0x06001B47 RID: 6983 RVA: 0x00062DF1 File Offset: 0x00060FF1
		// (set) Token: 0x06001B48 RID: 6984 RVA: 0x00062DF9 File Offset: 0x00060FF9
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

		// Token: 0x17000952 RID: 2386
		// (get) Token: 0x06001B49 RID: 6985 RVA: 0x00062E1C File Offset: 0x0006101C
		// (set) Token: 0x06001B4A RID: 6986 RVA: 0x00062E24 File Offset: 0x00061024
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

		// Token: 0x17000953 RID: 2387
		// (get) Token: 0x06001B4B RID: 6987 RVA: 0x00062E47 File Offset: 0x00061047
		// (set) Token: 0x06001B4C RID: 6988 RVA: 0x00062E4F File Offset: 0x0006104F
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

		// Token: 0x17000954 RID: 2388
		// (get) Token: 0x06001B4D RID: 6989 RVA: 0x00062E6D File Offset: 0x0006106D
		// (set) Token: 0x06001B4E RID: 6990 RVA: 0x00062E75 File Offset: 0x00061075
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

		// Token: 0x17000955 RID: 2389
		// (get) Token: 0x06001B4F RID: 6991 RVA: 0x00062E93 File Offset: 0x00061093
		// (set) Token: 0x06001B50 RID: 6992 RVA: 0x00062E9B File Offset: 0x0006109B
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

		// Token: 0x17000956 RID: 2390
		// (get) Token: 0x06001B51 RID: 6993 RVA: 0x00062EB9 File Offset: 0x000610B9
		// (set) Token: 0x06001B52 RID: 6994 RVA: 0x00062EC1 File Offset: 0x000610C1
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

		// Token: 0x17000957 RID: 2391
		// (get) Token: 0x06001B53 RID: 6995 RVA: 0x00062EDF File Offset: 0x000610DF
		// (set) Token: 0x06001B54 RID: 6996 RVA: 0x00062EE7 File Offset: 0x000610E7
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

		// Token: 0x17000958 RID: 2392
		// (get) Token: 0x06001B55 RID: 6997 RVA: 0x00062F05 File Offset: 0x00061105
		// (set) Token: 0x06001B56 RID: 6998 RVA: 0x00062F0D File Offset: 0x0006110D
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

		// Token: 0x17000959 RID: 2393
		// (get) Token: 0x06001B57 RID: 6999 RVA: 0x00062F2B File Offset: 0x0006112B
		// (set) Token: 0x06001B58 RID: 7000 RVA: 0x00062F33 File Offset: 0x00061133
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

		// Token: 0x1700095A RID: 2394
		// (get) Token: 0x06001B59 RID: 7001 RVA: 0x00062F51 File Offset: 0x00061151
		// (set) Token: 0x06001B5A RID: 7002 RVA: 0x00062F59 File Offset: 0x00061159
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

		// Token: 0x1700095B RID: 2395
		// (get) Token: 0x06001B5B RID: 7003 RVA: 0x00062F77 File Offset: 0x00061177
		// (set) Token: 0x06001B5C RID: 7004 RVA: 0x00062F7F File Offset: 0x0006117F
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

		// Token: 0x1700095C RID: 2396
		// (get) Token: 0x06001B5D RID: 7005 RVA: 0x00062F9D File Offset: 0x0006119D
		// (set) Token: 0x06001B5E RID: 7006 RVA: 0x00062FA5 File Offset: 0x000611A5
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

		// Token: 0x1700095D RID: 2397
		// (get) Token: 0x06001B5F RID: 7007 RVA: 0x00062FC8 File Offset: 0x000611C8
		// (set) Token: 0x06001B60 RID: 7008 RVA: 0x00062FD0 File Offset: 0x000611D0
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

		// Token: 0x1700095E RID: 2398
		// (get) Token: 0x06001B61 RID: 7009 RVA: 0x00062FF3 File Offset: 0x000611F3
		// (set) Token: 0x06001B62 RID: 7010 RVA: 0x00062FFB File Offset: 0x000611FB
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

		// Token: 0x04000CD7 RID: 3287
		private readonly IHeroDeveloper _developer;

		// Token: 0x04000CD8 RID: 3288
		private MBBindingList<SkillVM> _skills;

		// Token: 0x04000CD9 RID: 3289
		private PerkSelectionVM _perkSelection;

		// Token: 0x04000CDA RID: 3290
		private HeroViewModel _heroCharacter;

		// Token: 0x04000CDB RID: 3291
		private int _skillPointsRequiredForCurrentLevel;

		// Token: 0x04000CDC RID: 3292
		private int _skillPointsRequiredForNextLevel;

		// Token: 0x04000CDD RID: 3293
		private int _currentTotalSkill;

		// Token: 0x04000CDE RID: 3294
		private int _levelProgressPercentage;

		// Token: 0x04000CDF RID: 3295
		private int _unspentCharacterPoints;

		// Token: 0x04000CE0 RID: 3296
		private int _unspentAttributePoints;

		// Token: 0x04000CE1 RID: 3297
		private string _levelProgressText;

		// Token: 0x04000CE2 RID: 3298
		private string _heroNameText;

		// Token: 0x04000CE3 RID: 3299
		private string _heroInfoText;

		// Token: 0x04000CE4 RID: 3300
		private bool _isInspectingAnAttribute;

		// Token: 0x04000CE5 RID: 3301
		private HintViewModel _levelHint;

		// Token: 0x04000CE6 RID: 3302
		private SkillVM _currentSkill;

		// Token: 0x04000CE7 RID: 3303
		private CharacterAttributeItemVM _currentInspectedAttribute;

		// Token: 0x04000CE8 RID: 3304
		private string _heroLevelText;

		// Token: 0x04000CE9 RID: 3305
		private string _focusPointsText;

		// Token: 0x04000CEA RID: 3306
		private MBBindingList<StringPairItemVM> _characterStats;

		// Token: 0x04000CEB RID: 3307
		private MBBindingList<CharacterAttributeItemVM> _attributes;

		// Token: 0x04000CEC RID: 3308
		private MBBindingList<EncyclopediaTraitItemVM> _traits;
	}
}
