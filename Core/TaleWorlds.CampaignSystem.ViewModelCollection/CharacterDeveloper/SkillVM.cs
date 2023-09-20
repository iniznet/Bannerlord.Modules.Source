using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.CharacterDeveloper
{
	// Token: 0x0200011B RID: 283
	public class SkillVM : ViewModel
	{
		// Token: 0x1700096A RID: 2410
		// (get) Token: 0x06001B7B RID: 7035 RVA: 0x000633FA File Offset: 0x000615FA
		private int _boundAttributeCurrentValue
		{
			get
			{
				return this._developerVM.GetCurrentAttributePoint(this.Skill.CharacterAttribute);
			}
		}

		// Token: 0x1700096B RID: 2411
		// (get) Token: 0x06001B7C RID: 7036 RVA: 0x00063412 File Offset: 0x00061612
		private int _heroLevel
		{
			get
			{
				return this._developerVM.Hero.CharacterObject.Level;
			}
		}

		// Token: 0x06001B7D RID: 7037 RVA: 0x0006342C File Offset: 0x0006162C
		public SkillVM(SkillObject skill, CharacterVM developerVM, Action<PerkVM> onStartPerkSelection)
		{
			this._developerVM = developerVM;
			this.Skill = skill;
			this.MaxLevel = 300;
			this.SkillId = skill.StringId;
			this._onStartPerkSelection = onStartPerkSelection;
			this.IsInspected = false;
			this.Type = (skill.IsPartySkill ? SkillVM.SkillType.Party : (skill.IsLeaderSkill ? SkillVM.SkillType.Leader : SkillVM.SkillType.Default)).ToString();
			this.SkillEffects = new MBBindingList<BindingListStringItem>();
			this.Perks = new MBBindingList<PerkVM>();
			this.AddFocusHint = new HintViewModel();
			this._boundAttributeName = this.Skill.CharacterAttribute.Name;
			this.LearningRateTooltip = new BasicTooltipViewModel(() => CampaignUIHelper.GetLearningRateTooltip(this._boundAttributeCurrentValue, this.CurrentFocusLevel, this.Level, this._heroLevel, this._boundAttributeName));
			this.LearningLimitTooltip = new BasicTooltipViewModel(() => CampaignUIHelper.GetLearningLimitTooltip(this._boundAttributeCurrentValue, this.CurrentFocusLevel, this._boundAttributeName));
			this.InitializeValues();
			this._focusConceptObj = Concept.All.SingleOrDefault((Concept c) => c.StringId == "str_game_objects_skill_focus");
			this._skillConceptObj = Concept.All.SingleOrDefault((Concept c) => c.StringId == "str_game_objects_skills");
			this.RefreshValues();
		}

		// Token: 0x06001B7E RID: 7038 RVA: 0x00063574 File Offset: 0x00061774
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.AddFocusText = GameTexts.FindText("str_add_focus", null).ToString();
			this.HowToLearnText = this.Skill.HowToLearnSkillText.ToString();
			this.HowToLearnTitle = GameTexts.FindText("str_how_to_learn", null).ToString();
			this.DescriptionText = this.Skill.Description.ToString();
			this.NameText = this.Skill.Name.ToString();
			this.InitializeValues();
			this.RefreshWithCurrentValues();
			this.SkillEffects.ApplyActionOnAllItems(delegate(BindingListStringItem x)
			{
				x.RefreshValues();
			});
			this.Perks.ApplyActionOnAllItems(delegate(PerkVM x)
			{
				x.RefreshValues();
			});
		}

		// Token: 0x06001B7F RID: 7039 RVA: 0x00063658 File Offset: 0x00061858
		public void InitializeValues()
		{
			if (this._developerVM.GetCharacterDeveloper() == null)
			{
				this.Level = 0;
			}
			else
			{
				this.Level = this._developerVM.GetCharacterDeveloper().Hero.GetSkillValue(this.Skill);
				this.NextLevel = this.Level + 1;
				this.CurrentSkillXP = this._developerVM.GetCharacterDeveloper().GetSkillXpProgress(this.Skill);
				this.XpRequiredForNextLevel = Campaign.Current.Models.CharacterDevelopmentModel.GetXpRequiredForSkillLevel(this.Level + 1) - Campaign.Current.Models.CharacterDevelopmentModel.GetXpRequiredForSkillLevel(this.Level);
				this.ProgressPercentage = 100.0 * (double)this._currentSkillXP / (double)this.XpRequiredForNextLevel;
				this.ProgressHint = new BasicTooltipViewModel(delegate
				{
					GameTexts.SetVariable("CURRENT_XP", this.CurrentSkillXP.ToString());
					GameTexts.SetVariable("LEVEL_MAX_XP", this.XpRequiredForNextLevel.ToString());
					return GameTexts.FindText("str_current_xp_over_max", null).ToString();
				});
				GameTexts.SetVariable("CURRENT_XP", this.CurrentSkillXP.ToString());
				GameTexts.SetVariable("LEVEL_MAX_XP", this.XpRequiredForNextLevel.ToString());
				this.ProgressText = GameTexts.FindText("str_current_xp_over_max", null).ToString();
				this.SkillXPHint = new BasicTooltipViewModel(delegate
				{
					GameTexts.SetVariable("REQUIRED_XP_FOR_NEXT_LEVEL", this.XpRequiredForNextLevel - this.CurrentSkillXP);
					return GameTexts.FindText("str_skill_xp_hint", null).ToString();
				});
			}
			this._orgFocusAmount = this._developerVM.GetCharacterDeveloper().GetFocus(this.Skill);
			this.CurrentFocusLevel = this._orgFocusAmount;
			this.CreateLists();
		}

		// Token: 0x06001B80 RID: 7040 RVA: 0x000637C8 File Offset: 0x000619C8
		public void RefreshWithCurrentValues()
		{
			float resultNumber = Campaign.Current.Models.CharacterDevelopmentModel.CalculateLearningRate(this._boundAttributeCurrentValue, this.CurrentFocusLevel, this.Level, this._heroLevel, this._boundAttributeName, false).ResultNumber;
			GameTexts.SetVariable("COUNT", resultNumber.ToString("0.00"));
			this.CurrentLearningRateText = GameTexts.FindText("str_learning_rate_COUNT", null).ToString();
			this.CanLearnSkill = resultNumber > 0f;
			this.LearningRate = resultNumber;
			this.FullLearningRateLevel = MathF.Round(Campaign.Current.Models.CharacterDevelopmentModel.CalculateLearningLimit(this._boundAttributeCurrentValue, this.CurrentFocusLevel, this._boundAttributeName, false).ResultNumber);
			int requiredFocusPointsToAddFocusWithCurrentFocus = this._developerVM.GetRequiredFocusPointsToAddFocusWithCurrentFocus(this.Skill);
			GameTexts.SetVariable("COSTAMOUNT", requiredFocusPointsToAddFocusWithCurrentFocus);
			this.FocusCostText = requiredFocusPointsToAddFocusWithCurrentFocus.ToString();
			GameTexts.SetVariable("COUNT", requiredFocusPointsToAddFocusWithCurrentFocus);
			GameTexts.SetVariable("RIGHT", "");
			GameTexts.SetVariable("LEFT", GameTexts.FindText("str_cost_COUNT", null));
			MBTextManager.SetTextVariable("FOCUS_ICON", GameTexts.FindText("str_html_focus_icon", null), false);
			this.NextLevelCostText = GameTexts.FindText("str_sf_text_with_focus_icon", null).ToString();
			this.RefreshCanAddFocus();
		}

		// Token: 0x06001B81 RID: 7041 RVA: 0x00063918 File Offset: 0x00061B18
		public void CreateLists()
		{
			this.SkillEffects.Clear();
			this.Perks.Clear();
			int skillValue = this._developerVM.GetCharacterDeveloper().Hero.GetSkillValue(this.Skill);
			foreach (SkillEffect skillEffect in SkillEffect.All.Where((SkillEffect x) => x.EffectedSkills.Contains(this.Skill)))
			{
				this.SkillEffects.Add(new BindingListStringItem(CampaignUIHelper.GetSkillEffectText(skillEffect, skillValue)));
			}
			foreach (PerkObject perkObject in from p in PerkObject.All
				where p.Skill == this.Skill
				orderby p.RequiredSkillValue
				select p)
			{
				PerkVM.PerkAlternativeType perkAlternativeType = ((perkObject.AlternativePerk == null) ? PerkVM.PerkAlternativeType.NoAlternative : ((perkObject.StringId.CompareTo(perkObject.AlternativePerk.StringId) < 0) ? PerkVM.PerkAlternativeType.FirstAlternative : PerkVM.PerkAlternativeType.SecondAlternative));
				PerkVM perkVM = new PerkVM(perkObject, this.IsPerkAvailable(perkObject), perkAlternativeType, new Action<PerkVM>(this.OnStartPerkSelection), new Action<PerkVM>(this.OnPerkSelectionOver), new Func<PerkObject, bool>(this.IsPerkSelected), new Func<PerkObject, bool>(this.IsPreviousPerkSelected));
				this.Perks.Add(perkVM);
			}
			this.RefreshNumOfUnopenedPerks();
		}

		// Token: 0x06001B82 RID: 7042 RVA: 0x00063AAC File Offset: 0x00061CAC
		public void RefreshLists(SkillObject skill = null)
		{
			if (skill != null && skill != this.Skill)
			{
				return;
			}
			foreach (PerkVM perkVM in this.Perks)
			{
				perkVM.RefreshState();
			}
			this.RefreshNumOfUnopenedPerks();
		}

		// Token: 0x06001B83 RID: 7043 RVA: 0x00063B0C File Offset: 0x00061D0C
		private void RefreshNumOfUnopenedPerks()
		{
			int num = 0;
			foreach (PerkVM perkVM in this.Perks)
			{
				if ((perkVM.CurrentState == PerkVM.PerkStates.EarnedButNotSelected || perkVM.CurrentState == PerkVM.PerkStates.EarnedPreviousPerkNotSelected) && (perkVM.AlternativeType == 1 || perkVM.AlternativeType == 0))
				{
					num++;
				}
			}
			this.NumOfUnopenedPerks = num;
		}

		// Token: 0x06001B84 RID: 7044 RVA: 0x00063B84 File Offset: 0x00061D84
		private bool IsPerkSelected(PerkObject perk)
		{
			return this._developerVM.GetCharacterDeveloper().GetPerkValue(perk) || this._developerVM.PerkSelection.IsPerkSelected(perk);
		}

		// Token: 0x06001B85 RID: 7045 RVA: 0x00063BAC File Offset: 0x00061DAC
		private bool IsPreviousPerkSelected(PerkObject perk)
		{
			IEnumerable<PerkObject> enumerable = PerkObject.All.Where((PerkObject p) => p.Skill == perk.Skill && p.RequiredSkillValue < perk.RequiredSkillValue);
			if (!enumerable.Any<PerkObject>())
			{
				return true;
			}
			PerkObject perkObject = enumerable.MaxBy((PerkObject p) => p.RequiredSkillValue - perk.RequiredSkillValue);
			return this.IsPerkSelected(perkObject) || (perkObject.AlternativePerk != null && this.IsPerkSelected(perkObject.AlternativePerk));
		}

		// Token: 0x06001B86 RID: 7046 RVA: 0x00063C1B File Offset: 0x00061E1B
		private bool IsPerkAvailable(PerkObject perk)
		{
			return perk.RequiredSkillValue <= (float)this.Level;
		}

		// Token: 0x06001B87 RID: 7047 RVA: 0x00063C30 File Offset: 0x00061E30
		public void RefreshCanAddFocus()
		{
			bool flag = this._developerVM.UnspentCharacterPoints >= this._developerVM.GetRequiredFocusPointsToAddFocusWithCurrentFocus(this.Skill);
			bool flag2 = this._currentFocusLevel >= Campaign.Current.Models.CharacterDevelopmentModel.MaxFocusPerSkill;
			string addFocusHintString = CampaignUIHelper.GetAddFocusHintString(flag, flag2, this.CurrentFocusLevel, this._boundAttributeCurrentValue, this.Level, this._developerVM.GetCharacterDeveloper(), this.Skill);
			this.AddFocusHint.HintText = (string.IsNullOrEmpty(addFocusHintString) ? TextObject.Empty : new TextObject("{=!}" + addFocusHintString, null));
			this.CanAddFocus = this._developerVM.CanAddFocusToSkillWithFocusAmount(this._currentFocusLevel);
		}

		// Token: 0x06001B88 RID: 7048 RVA: 0x00063CEC File Offset: 0x00061EEC
		public void ExecuteAddFocus()
		{
			if (this.CanAddFocus)
			{
				this._developerVM.UnspentCharacterPoints -= this._developerVM.GetRequiredFocusPointsToAddFocusWithCurrentFocus(this.Skill);
				int num = this.CurrentFocusLevel;
				this.CurrentFocusLevel = num + 1;
				this._developerVM.RefreshCharacterValues();
				this.RefreshWithCurrentValues();
				MBInformationManager.HideInformations();
				if (this.Level == 0)
				{
					num = this.Level;
					this.Level = num + 1;
					num = this.NextLevel;
					this.NextLevel = num + 1;
				}
				Game.Current.EventManager.TriggerEvent<FocusAddedByPlayerEvent>(new FocusAddedByPlayerEvent(this._developerVM.Hero, this.Skill));
			}
		}

		// Token: 0x06001B89 RID: 7049 RVA: 0x00063D9A File Offset: 0x00061F9A
		public void ExecuteShowFocusConcept()
		{
			if (this._focusConceptObj != null)
			{
				Campaign.Current.EncyclopediaManager.GoToLink(this._focusConceptObj.EncyclopediaLink);
				return;
			}
			Debug.FailedAssert("Couldn't find Focus encyclopedia page", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem.ViewModelCollection\\CharacterDeveloper\\SkillVM.cs", "ExecuteShowFocusConcept", 266);
		}

		// Token: 0x06001B8A RID: 7050 RVA: 0x00063DD8 File Offset: 0x00061FD8
		public void ExecuteShowSkillConcept()
		{
			if (this._focusConceptObj != null)
			{
				Campaign.Current.EncyclopediaManager.GoToLink(this._skillConceptObj.EncyclopediaLink);
				return;
			}
			Debug.FailedAssert("Couldn't find Focus encyclopedia page", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem.ViewModelCollection\\CharacterDeveloper\\SkillVM.cs", "ExecuteShowSkillConcept", 278);
		}

		// Token: 0x06001B8B RID: 7051 RVA: 0x00063E16 File Offset: 0x00062016
		public void ExecuteInspect()
		{
			this._developerVM.SetCurrentSkill(this);
			this.RefreshCanAddFocus();
		}

		// Token: 0x06001B8C RID: 7052 RVA: 0x00063E2C File Offset: 0x0006202C
		public void ResetChanges()
		{
			this.CurrentFocusLevel = this._orgFocusAmount;
			if (this.CurrentFocusLevel == 0 && this.Level == 1)
			{
				this.Level = 0;
				this.NextLevel = this.Level + 1;
			}
			this.Perks.ApplyActionOnAllItems(delegate(PerkVM p)
			{
				p.RefreshState();
			});
			this.RefreshNumOfUnopenedPerks();
		}

		// Token: 0x06001B8D RID: 7053 RVA: 0x00063E9B File Offset: 0x0006209B
		public bool IsThereAnyChanges()
		{
			return this.CurrentFocusLevel != this._orgFocusAmount;
		}

		// Token: 0x06001B8E RID: 7054 RVA: 0x00063EB0 File Offset: 0x000620B0
		public void ApplyChanges()
		{
			for (int i = 0; i < this.CurrentFocusLevel - this._orgFocusAmount; i++)
			{
				this._developerVM.GetCharacterDeveloper().AddFocus(this.Skill, 1, true);
			}
			this._orgFocusAmount = this.CurrentFocusLevel;
		}

		// Token: 0x06001B8F RID: 7055 RVA: 0x00063EFC File Offset: 0x000620FC
		private void OnStartPerkSelection(PerkVM perk)
		{
			this._onStartPerkSelection(perk);
			if (perk.AlternativeType != 0)
			{
				this.Perks.SingleOrDefault((PerkVM p) => p.Perk == perk.Perk.AlternativePerk).IsInSelection = true;
			}
		}

		// Token: 0x06001B90 RID: 7056 RVA: 0x00063F54 File Offset: 0x00062154
		private void OnPerkSelectionOver(PerkVM perk)
		{
			if (perk.AlternativeType != 0)
			{
				this.Perks.SingleOrDefault((PerkVM p) => p.Perk == perk.Perk.AlternativePerk).IsInSelection = false;
			}
		}

		// Token: 0x1700096C RID: 2412
		// (get) Token: 0x06001B91 RID: 7057 RVA: 0x00063F98 File Offset: 0x00062198
		// (set) Token: 0x06001B92 RID: 7058 RVA: 0x00063FA0 File Offset: 0x000621A0
		[DataSourceProperty]
		public string Type
		{
			get
			{
				return this._type;
			}
			set
			{
				if (value != this._type)
				{
					this._type = value;
					base.OnPropertyChangedWithValue<string>(value, "Type");
				}
			}
		}

		// Token: 0x1700096D RID: 2413
		// (get) Token: 0x06001B93 RID: 7059 RVA: 0x00063FC3 File Offset: 0x000621C3
		// (set) Token: 0x06001B94 RID: 7060 RVA: 0x00063FCB File Offset: 0x000621CB
		[DataSourceProperty]
		public string DescriptionText
		{
			get
			{
				return this._descriptionText;
			}
			set
			{
				if (value != this._descriptionText)
				{
					this._descriptionText = value;
					base.OnPropertyChangedWithValue<string>(value, "DescriptionText");
				}
			}
		}

		// Token: 0x1700096E RID: 2414
		// (get) Token: 0x06001B95 RID: 7061 RVA: 0x00063FEE File Offset: 0x000621EE
		// (set) Token: 0x06001B96 RID: 7062 RVA: 0x00063FF6 File Offset: 0x000621F6
		[DataSourceProperty]
		public string HowToLearnText
		{
			get
			{
				return this._howToLearnText;
			}
			set
			{
				if (value != this._howToLearnText)
				{
					this._howToLearnText = value;
					base.OnPropertyChangedWithValue<string>(value, "HowToLearnText");
				}
			}
		}

		// Token: 0x1700096F RID: 2415
		// (get) Token: 0x06001B97 RID: 7063 RVA: 0x00064019 File Offset: 0x00062219
		// (set) Token: 0x06001B98 RID: 7064 RVA: 0x00064021 File Offset: 0x00062221
		[DataSourceProperty]
		public string HowToLearnTitle
		{
			get
			{
				return this._howToLearnTitle;
			}
			set
			{
				if (value != this._howToLearnTitle)
				{
					this._howToLearnTitle = value;
					base.OnPropertyChangedWithValue<string>(value, "HowToLearnTitle");
				}
			}
		}

		// Token: 0x17000970 RID: 2416
		// (get) Token: 0x06001B99 RID: 7065 RVA: 0x00064044 File Offset: 0x00062244
		// (set) Token: 0x06001B9A RID: 7066 RVA: 0x0006404C File Offset: 0x0006224C
		[DataSourceProperty]
		public bool CanAddFocus
		{
			get
			{
				return this._canAddFocus;
			}
			set
			{
				if (value != this._canAddFocus)
				{
					this._canAddFocus = value;
					base.OnPropertyChangedWithValue(value, "CanAddFocus");
				}
			}
		}

		// Token: 0x17000971 RID: 2417
		// (get) Token: 0x06001B9B RID: 7067 RVA: 0x0006406A File Offset: 0x0006226A
		// (set) Token: 0x06001B9C RID: 7068 RVA: 0x00064072 File Offset: 0x00062272
		[DataSourceProperty]
		public bool CanLearnSkill
		{
			get
			{
				return this._canLearnSkill;
			}
			set
			{
				if (value != this._canLearnSkill)
				{
					this._canLearnSkill = value;
					base.OnPropertyChangedWithValue(value, "CanLearnSkill");
				}
			}
		}

		// Token: 0x17000972 RID: 2418
		// (get) Token: 0x06001B9D RID: 7069 RVA: 0x00064090 File Offset: 0x00062290
		// (set) Token: 0x06001B9E RID: 7070 RVA: 0x00064098 File Offset: 0x00062298
		[DataSourceProperty]
		public string NextLevelLearningRateText
		{
			get
			{
				return this._nextLevelLearningRateText;
			}
			set
			{
				if (value != this._nextLevelLearningRateText)
				{
					this._nextLevelLearningRateText = value;
					base.OnPropertyChangedWithValue<string>(value, "NextLevelLearningRateText");
				}
			}
		}

		// Token: 0x17000973 RID: 2419
		// (get) Token: 0x06001B9F RID: 7071 RVA: 0x000640BB File Offset: 0x000622BB
		// (set) Token: 0x06001BA0 RID: 7072 RVA: 0x000640C3 File Offset: 0x000622C3
		[DataSourceProperty]
		public string NextLevelCostText
		{
			get
			{
				return this._nextLevelCostText;
			}
			set
			{
				if (value != this._nextLevelCostText)
				{
					this._nextLevelCostText = value;
					base.OnPropertyChangedWithValue<string>(value, "NextLevelCostText");
				}
			}
		}

		// Token: 0x17000974 RID: 2420
		// (get) Token: 0x06001BA1 RID: 7073 RVA: 0x000640E6 File Offset: 0x000622E6
		// (set) Token: 0x06001BA2 RID: 7074 RVA: 0x000640EE File Offset: 0x000622EE
		[DataSourceProperty]
		public BasicTooltipViewModel ProgressHint
		{
			get
			{
				return this._progressHint;
			}
			set
			{
				if (value != this._progressHint)
				{
					this._progressHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "ProgressHint");
				}
			}
		}

		// Token: 0x17000975 RID: 2421
		// (get) Token: 0x06001BA3 RID: 7075 RVA: 0x0006410C File Offset: 0x0006230C
		// (set) Token: 0x06001BA4 RID: 7076 RVA: 0x00064114 File Offset: 0x00062314
		[DataSourceProperty]
		public BasicTooltipViewModel SkillXPHint
		{
			get
			{
				return this._skillXPHint;
			}
			set
			{
				if (value != this._skillXPHint)
				{
					this._skillXPHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "SkillXPHint");
				}
			}
		}

		// Token: 0x17000976 RID: 2422
		// (get) Token: 0x06001BA5 RID: 7077 RVA: 0x00064132 File Offset: 0x00062332
		// (set) Token: 0x06001BA6 RID: 7078 RVA: 0x0006413A File Offset: 0x0006233A
		[DataSourceProperty]
		public HintViewModel AddFocusHint
		{
			get
			{
				return this._addFocusHint;
			}
			set
			{
				if (value != this._addFocusHint)
				{
					this._addFocusHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "AddFocusHint");
				}
			}
		}

		// Token: 0x17000977 RID: 2423
		// (get) Token: 0x06001BA7 RID: 7079 RVA: 0x00064158 File Offset: 0x00062358
		// (set) Token: 0x06001BA8 RID: 7080 RVA: 0x00064160 File Offset: 0x00062360
		[DataSourceProperty]
		public BasicTooltipViewModel LearningLimitTooltip
		{
			get
			{
				return this._learningLimitTooltip;
			}
			set
			{
				if (value != this._learningLimitTooltip)
				{
					this._learningLimitTooltip = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "LearningLimitTooltip");
				}
			}
		}

		// Token: 0x17000978 RID: 2424
		// (get) Token: 0x06001BA9 RID: 7081 RVA: 0x0006417E File Offset: 0x0006237E
		// (set) Token: 0x06001BAA RID: 7082 RVA: 0x00064186 File Offset: 0x00062386
		[DataSourceProperty]
		public BasicTooltipViewModel LearningRateTooltip
		{
			get
			{
				return this._learningRateTooltip;
			}
			set
			{
				if (value != this._learningRateTooltip)
				{
					this._learningRateTooltip = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "LearningRateTooltip");
				}
			}
		}

		// Token: 0x17000979 RID: 2425
		// (get) Token: 0x06001BAB RID: 7083 RVA: 0x000641A4 File Offset: 0x000623A4
		// (set) Token: 0x06001BAC RID: 7084 RVA: 0x000641AC File Offset: 0x000623AC
		[DataSourceProperty]
		public double ProgressPercentage
		{
			get
			{
				return this._progressPercentage;
			}
			set
			{
				if (value != this._progressPercentage)
				{
					this._progressPercentage = value;
					base.OnPropertyChangedWithValue(value, "ProgressPercentage");
				}
			}
		}

		// Token: 0x1700097A RID: 2426
		// (get) Token: 0x06001BAD RID: 7085 RVA: 0x000641CA File Offset: 0x000623CA
		// (set) Token: 0x06001BAE RID: 7086 RVA: 0x000641D2 File Offset: 0x000623D2
		[DataSourceProperty]
		public float LearningRate
		{
			get
			{
				return this._learningRate;
			}
			set
			{
				if (value != this._learningRate)
				{
					this._learningRate = value;
					base.OnPropertyChangedWithValue(value, "LearningRate");
				}
			}
		}

		// Token: 0x1700097B RID: 2427
		// (get) Token: 0x06001BAF RID: 7087 RVA: 0x000641F0 File Offset: 0x000623F0
		// (set) Token: 0x06001BB0 RID: 7088 RVA: 0x000641F8 File Offset: 0x000623F8
		[DataSourceProperty]
		public int CurrentSkillXP
		{
			get
			{
				return this._currentSkillXP;
			}
			set
			{
				if (value != this._currentSkillXP)
				{
					this._currentSkillXP = value;
					base.OnPropertyChangedWithValue(value, "CurrentSkillXP");
				}
			}
		}

		// Token: 0x1700097C RID: 2428
		// (get) Token: 0x06001BB1 RID: 7089 RVA: 0x00064216 File Offset: 0x00062416
		// (set) Token: 0x06001BB2 RID: 7090 RVA: 0x0006421E File Offset: 0x0006241E
		[DataSourceProperty]
		public int NextLevel
		{
			get
			{
				return this._nextLevel;
			}
			set
			{
				if (value != this._nextLevel)
				{
					this._nextLevel = value;
					base.OnPropertyChangedWithValue(value, "NextLevel");
				}
			}
		}

		// Token: 0x1700097D RID: 2429
		// (get) Token: 0x06001BB3 RID: 7091 RVA: 0x0006423C File Offset: 0x0006243C
		// (set) Token: 0x06001BB4 RID: 7092 RVA: 0x00064244 File Offset: 0x00062444
		[DataSourceProperty]
		public int FullLearningRateLevel
		{
			get
			{
				return this._fullLearningRateLevel;
			}
			set
			{
				if (value != this._fullLearningRateLevel)
				{
					this._fullLearningRateLevel = value;
					base.OnPropertyChangedWithValue(value, "FullLearningRateLevel");
				}
			}
		}

		// Token: 0x1700097E RID: 2430
		// (get) Token: 0x06001BB5 RID: 7093 RVA: 0x00064262 File Offset: 0x00062462
		// (set) Token: 0x06001BB6 RID: 7094 RVA: 0x0006426A File Offset: 0x0006246A
		[DataSourceProperty]
		public int XpRequiredForNextLevel
		{
			get
			{
				return this._xpRequiredForNextLevel;
			}
			set
			{
				if (value != this._xpRequiredForNextLevel)
				{
					this._xpRequiredForNextLevel = value;
					base.OnPropertyChangedWithValue(value, "XpRequiredForNextLevel");
				}
			}
		}

		// Token: 0x1700097F RID: 2431
		// (get) Token: 0x06001BB7 RID: 7095 RVA: 0x00064288 File Offset: 0x00062488
		// (set) Token: 0x06001BB8 RID: 7096 RVA: 0x00064290 File Offset: 0x00062490
		[DataSourceProperty]
		public int NumOfUnopenedPerks
		{
			get
			{
				return this._numOfUnopenedPerks;
			}
			set
			{
				if (value != this._numOfUnopenedPerks)
				{
					this._numOfUnopenedPerks = value;
					base.OnPropertyChangedWithValue(value, "NumOfUnopenedPerks");
				}
			}
		}

		// Token: 0x17000980 RID: 2432
		// (get) Token: 0x06001BB9 RID: 7097 RVA: 0x000642AE File Offset: 0x000624AE
		// (set) Token: 0x06001BBA RID: 7098 RVA: 0x000642B6 File Offset: 0x000624B6
		[DataSourceProperty]
		public string ProgressText
		{
			get
			{
				return this._progressText;
			}
			set
			{
				if (value != this._progressText)
				{
					this._progressText = value;
					base.OnPropertyChangedWithValue<string>(value, "ProgressText");
				}
			}
		}

		// Token: 0x17000981 RID: 2433
		// (get) Token: 0x06001BBB RID: 7099 RVA: 0x000642D9 File Offset: 0x000624D9
		// (set) Token: 0x06001BBC RID: 7100 RVA: 0x000642E1 File Offset: 0x000624E1
		[DataSourceProperty]
		public string FocusCostText
		{
			get
			{
				return this._focusCostText;
			}
			set
			{
				if (value != this._focusCostText)
				{
					this._focusCostText = value;
					base.OnPropertyChangedWithValue<string>(value, "FocusCostText");
				}
			}
		}

		// Token: 0x17000982 RID: 2434
		// (get) Token: 0x06001BBD RID: 7101 RVA: 0x00064304 File Offset: 0x00062504
		// (set) Token: 0x06001BBE RID: 7102 RVA: 0x0006430C File Offset: 0x0006250C
		[DataSourceProperty]
		public MBBindingList<PerkVM> Perks
		{
			get
			{
				return this._perks;
			}
			set
			{
				if (value != this._perks)
				{
					this._perks = value;
					base.OnPropertyChangedWithValue<MBBindingList<PerkVM>>(value, "Perks");
				}
			}
		}

		// Token: 0x17000983 RID: 2435
		// (get) Token: 0x06001BBF RID: 7103 RVA: 0x0006432A File Offset: 0x0006252A
		// (set) Token: 0x06001BC0 RID: 7104 RVA: 0x00064332 File Offset: 0x00062532
		[DataSourceProperty]
		public MBBindingList<BindingListStringItem> SkillEffects
		{
			get
			{
				return this._skillEffects;
			}
			set
			{
				if (value != this._skillEffects)
				{
					this._skillEffects = value;
					base.OnPropertyChangedWithValue<MBBindingList<BindingListStringItem>>(value, "SkillEffects");
				}
			}
		}

		// Token: 0x17000984 RID: 2436
		// (get) Token: 0x06001BC1 RID: 7105 RVA: 0x00064350 File Offset: 0x00062550
		// (set) Token: 0x06001BC2 RID: 7106 RVA: 0x00064358 File Offset: 0x00062558
		[DataSourceProperty]
		public int MaxLevel
		{
			get
			{
				return this._maxLevel;
			}
			set
			{
				if (value != this._maxLevel)
				{
					this._maxLevel = value;
					base.OnPropertyChangedWithValue(value, "MaxLevel");
				}
			}
		}

		// Token: 0x17000985 RID: 2437
		// (get) Token: 0x06001BC3 RID: 7107 RVA: 0x00064376 File Offset: 0x00062576
		// (set) Token: 0x06001BC4 RID: 7108 RVA: 0x0006437E File Offset: 0x0006257E
		[DataSourceProperty]
		public string CurrentLearningRateText
		{
			get
			{
				return this._currentLearningRateText;
			}
			set
			{
				if (value != this._currentLearningRateText)
				{
					this._currentLearningRateText = value;
					base.OnPropertyChangedWithValue<string>(value, "CurrentLearningRateText");
				}
			}
		}

		// Token: 0x17000986 RID: 2438
		// (get) Token: 0x06001BC5 RID: 7109 RVA: 0x000643A1 File Offset: 0x000625A1
		// (set) Token: 0x06001BC6 RID: 7110 RVA: 0x000643A9 File Offset: 0x000625A9
		[DataSourceProperty]
		public int CurrentFocusLevel
		{
			get
			{
				return this._currentFocusLevel;
			}
			set
			{
				if (value != this._currentFocusLevel)
				{
					this._currentFocusLevel = value;
					base.OnPropertyChangedWithValue(value, "CurrentFocusLevel");
				}
			}
		}

		// Token: 0x17000987 RID: 2439
		// (get) Token: 0x06001BC7 RID: 7111 RVA: 0x000643C7 File Offset: 0x000625C7
		// (set) Token: 0x06001BC8 RID: 7112 RVA: 0x000643CF File Offset: 0x000625CF
		[DataSourceProperty]
		public string AddFocusText
		{
			get
			{
				return this._addFocusText;
			}
			set
			{
				if (value != this._addFocusText)
				{
					this._addFocusText = value;
					base.OnPropertyChangedWithValue<string>(value, "AddFocusText");
				}
			}
		}

		// Token: 0x17000988 RID: 2440
		// (get) Token: 0x06001BC9 RID: 7113 RVA: 0x000643F2 File Offset: 0x000625F2
		// (set) Token: 0x06001BCA RID: 7114 RVA: 0x000643FA File Offset: 0x000625FA
		[DataSourceProperty]
		public string SkillId
		{
			get
			{
				return this._skillId;
			}
			set
			{
				if (value != this._skillId)
				{
					this._skillId = value;
					base.OnPropertyChangedWithValue<string>(value, "SkillId");
				}
			}
		}

		// Token: 0x17000989 RID: 2441
		// (get) Token: 0x06001BCB RID: 7115 RVA: 0x0006441D File Offset: 0x0006261D
		// (set) Token: 0x06001BCC RID: 7116 RVA: 0x00064425 File Offset: 0x00062625
		[DataSourceProperty]
		public bool IsInspected
		{
			get
			{
				return this._isInspected;
			}
			set
			{
				if (value != this._isInspected)
				{
					this._isInspected = value;
					base.OnPropertyChangedWithValue(value, "IsInspected");
				}
			}
		}

		// Token: 0x1700098A RID: 2442
		// (get) Token: 0x06001BCD RID: 7117 RVA: 0x00064443 File Offset: 0x00062643
		// (set) Token: 0x06001BCE RID: 7118 RVA: 0x0006444B File Offset: 0x0006264B
		[DataSourceProperty]
		public string NameText
		{
			get
			{
				return this._nameText;
			}
			set
			{
				if (value != this._nameText)
				{
					this._nameText = value;
					base.OnPropertyChangedWithValue<string>(value, "NameText");
				}
			}
		}

		// Token: 0x1700098B RID: 2443
		// (get) Token: 0x06001BCF RID: 7119 RVA: 0x0006446E File Offset: 0x0006266E
		// (set) Token: 0x06001BD0 RID: 7120 RVA: 0x00064476 File Offset: 0x00062676
		[DataSourceProperty]
		public int Level
		{
			get
			{
				return this._level;
			}
			set
			{
				if (value != this._level)
				{
					this._level = value;
					base.OnPropertyChangedWithValue(value, "Level");
				}
			}
		}

		// Token: 0x04000CFE RID: 3326
		public const int MAX_SKILL_LEVEL = 300;

		// Token: 0x04000CFF RID: 3327
		public readonly SkillObject Skill;

		// Token: 0x04000D00 RID: 3328
		private readonly CharacterVM _developerVM;

		// Token: 0x04000D01 RID: 3329
		private readonly TextObject _boundAttributeName;

		// Token: 0x04000D02 RID: 3330
		private readonly Concept _focusConceptObj;

		// Token: 0x04000D03 RID: 3331
		private readonly Concept _skillConceptObj;

		// Token: 0x04000D04 RID: 3332
		private readonly Action<PerkVM> _onStartPerkSelection;

		// Token: 0x04000D05 RID: 3333
		private int _orgFocusAmount;

		// Token: 0x04000D06 RID: 3334
		private MBBindingList<BindingListStringItem> _skillEffects;

		// Token: 0x04000D07 RID: 3335
		private MBBindingList<PerkVM> _perks;

		// Token: 0x04000D08 RID: 3336
		private BasicTooltipViewModel _progressHint;

		// Token: 0x04000D09 RID: 3337
		private HintViewModel _addFocusHint;

		// Token: 0x04000D0A RID: 3338
		private BasicTooltipViewModel _skillXPHint;

		// Token: 0x04000D0B RID: 3339
		private BasicTooltipViewModel _learningLimitTooltip;

		// Token: 0x04000D0C RID: 3340
		private BasicTooltipViewModel _learningRateTooltip;

		// Token: 0x04000D0D RID: 3341
		private string _nameText;

		// Token: 0x04000D0E RID: 3342
		private string _skillId;

		// Token: 0x04000D0F RID: 3343
		private string _addFocusText;

		// Token: 0x04000D10 RID: 3344
		private string _focusCostText;

		// Token: 0x04000D11 RID: 3345
		private string _currentLearningRateText;

		// Token: 0x04000D12 RID: 3346
		private string _nextLevelLearningRateText;

		// Token: 0x04000D13 RID: 3347
		private string _nextLevelCostText;

		// Token: 0x04000D14 RID: 3348
		private string _howToLearnText;

		// Token: 0x04000D15 RID: 3349
		private string _howToLearnTitle;

		// Token: 0x04000D16 RID: 3350
		private string _type;

		// Token: 0x04000D17 RID: 3351
		private string _progressText;

		// Token: 0x04000D18 RID: 3352
		private string _descriptionText;

		// Token: 0x04000D19 RID: 3353
		private int _level = -1;

		// Token: 0x04000D1A RID: 3354
		private int _maxLevel;

		// Token: 0x04000D1B RID: 3355
		private int _currentFocusLevel;

		// Token: 0x04000D1C RID: 3356
		private int _currentSkillXP;

		// Token: 0x04000D1D RID: 3357
		private int _xpRequiredForNextLevel;

		// Token: 0x04000D1E RID: 3358
		private int _nextLevel;

		// Token: 0x04000D1F RID: 3359
		private int _fullLearningRateLevel;

		// Token: 0x04000D20 RID: 3360
		private int _numOfUnopenedPerks;

		// Token: 0x04000D21 RID: 3361
		private bool _isInspected;

		// Token: 0x04000D22 RID: 3362
		private bool _canAddFocus;

		// Token: 0x04000D23 RID: 3363
		private bool _canLearnSkill;

		// Token: 0x04000D24 RID: 3364
		private float _learningRate;

		// Token: 0x04000D25 RID: 3365
		private double _progressPercentage;

		// Token: 0x02000267 RID: 615
		private enum SkillType
		{
			// Token: 0x0400117E RID: 4478
			Default,
			// Token: 0x0400117F RID: 4479
			Party,
			// Token: 0x04001180 RID: 4480
			Leader
		}
	}
}
