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
	public class SkillVM : ViewModel
	{
		private int _boundAttributeCurrentValue
		{
			get
			{
				return this._developerVM.GetCurrentAttributePoint(this.Skill.CharacterAttribute);
			}
		}

		private int _heroLevel
		{
			get
			{
				return this._developerVM.Hero.CharacterObject.Level;
			}
		}

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

		private bool IsPerkSelected(PerkObject perk)
		{
			return this._developerVM.GetCharacterDeveloper().GetPerkValue(perk) || this._developerVM.PerkSelection.IsPerkSelected(perk);
		}

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

		private bool IsPerkAvailable(PerkObject perk)
		{
			return perk.RequiredSkillValue <= (float)this.Level;
		}

		public void RefreshCanAddFocus()
		{
			bool flag = this._developerVM.UnspentCharacterPoints >= this._developerVM.GetRequiredFocusPointsToAddFocusWithCurrentFocus(this.Skill);
			bool flag2 = this._currentFocusLevel >= Campaign.Current.Models.CharacterDevelopmentModel.MaxFocusPerSkill;
			string addFocusHintString = CampaignUIHelper.GetAddFocusHintString(flag, flag2, this.CurrentFocusLevel, this._boundAttributeCurrentValue, this.Level, this._developerVM.GetCharacterDeveloper(), this.Skill);
			this.AddFocusHint.HintText = (string.IsNullOrEmpty(addFocusHintString) ? TextObject.Empty : new TextObject("{=!}" + addFocusHintString, null));
			this.CanAddFocus = this._developerVM.CanAddFocusToSkillWithFocusAmount(this._currentFocusLevel);
		}

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

		public void ExecuteShowFocusConcept()
		{
			if (this._focusConceptObj != null)
			{
				Campaign.Current.EncyclopediaManager.GoToLink(this._focusConceptObj.EncyclopediaLink);
				return;
			}
			Debug.FailedAssert("Couldn't find Focus encyclopedia page", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem.ViewModelCollection\\CharacterDeveloper\\SkillVM.cs", "ExecuteShowFocusConcept", 266);
		}

		public void ExecuteShowSkillConcept()
		{
			if (this._focusConceptObj != null)
			{
				Campaign.Current.EncyclopediaManager.GoToLink(this._skillConceptObj.EncyclopediaLink);
				return;
			}
			Debug.FailedAssert("Couldn't find Focus encyclopedia page", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem.ViewModelCollection\\CharacterDeveloper\\SkillVM.cs", "ExecuteShowSkillConcept", 278);
		}

		public void ExecuteInspect()
		{
			this._developerVM.SetCurrentSkill(this);
			this.RefreshCanAddFocus();
		}

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

		public bool IsThereAnyChanges()
		{
			return this.CurrentFocusLevel != this._orgFocusAmount;
		}

		public void ApplyChanges()
		{
			for (int i = 0; i < this.CurrentFocusLevel - this._orgFocusAmount; i++)
			{
				this._developerVM.GetCharacterDeveloper().AddFocus(this.Skill, 1, true);
			}
			this._orgFocusAmount = this.CurrentFocusLevel;
		}

		private void OnStartPerkSelection(PerkVM perk)
		{
			this._onStartPerkSelection(perk);
			if (perk.AlternativeType != 0)
			{
				this.Perks.SingleOrDefault((PerkVM p) => p.Perk == perk.Perk.AlternativePerk).IsInSelection = true;
			}
		}

		private void OnPerkSelectionOver(PerkVM perk)
		{
			if (perk.AlternativeType != 0)
			{
				this.Perks.SingleOrDefault((PerkVM p) => p.Perk == perk.Perk.AlternativePerk).IsInSelection = false;
			}
		}

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

		public const int MAX_SKILL_LEVEL = 300;

		public readonly SkillObject Skill;

		private readonly CharacterVM _developerVM;

		private readonly TextObject _boundAttributeName;

		private readonly Concept _focusConceptObj;

		private readonly Concept _skillConceptObj;

		private readonly Action<PerkVM> _onStartPerkSelection;

		private int _orgFocusAmount;

		private MBBindingList<BindingListStringItem> _skillEffects;

		private MBBindingList<PerkVM> _perks;

		private BasicTooltipViewModel _progressHint;

		private HintViewModel _addFocusHint;

		private BasicTooltipViewModel _skillXPHint;

		private BasicTooltipViewModel _learningLimitTooltip;

		private BasicTooltipViewModel _learningRateTooltip;

		private string _nameText;

		private string _skillId;

		private string _addFocusText;

		private string _focusCostText;

		private string _currentLearningRateText;

		private string _nextLevelLearningRateText;

		private string _nextLevelCostText;

		private string _howToLearnText;

		private string _howToLearnTitle;

		private string _type;

		private string _progressText;

		private string _descriptionText;

		private int _level = -1;

		private int _maxLevel;

		private int _currentFocusLevel;

		private int _currentSkillXP;

		private int _xpRequiredForNextLevel;

		private int _nextLevel;

		private int _fullLearningRateLevel;

		private int _numOfUnopenedPerks;

		private bool _isInspected;

		private bool _canAddFocus;

		private bool _canLearnSkill;

		private float _learningRate;

		private double _progressPercentage;

		private enum SkillType
		{
			Default,
			Party,
			Leader
		}
	}
}
