using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TaleWorlds.CampaignSystem.ViewModelCollection.Input;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Core.ViewModelCollection.Selector;
using TaleWorlds.Core.ViewModelCollection.Tutorial;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.CharacterDeveloper
{
	public class CharacterDeveloperVM : ViewModel
	{
		public CharacterDeveloperVM(Action closeCharacterDeveloper)
		{
			this._closeCharacterDeveloper = closeCharacterDeveloper;
			this.TutorialNotification = new ElementNotificationVM();
			this._viewDataTracker = Campaign.Current.GetCampaignBehavior<IViewDataTracker>();
			this._heroList = new List<CharacterVM>();
			this.HeroList = new ReadOnlyCollection<CharacterVM>(this._heroList);
			foreach (Hero hero in this.GetApplicableHeroes())
			{
				if (hero == Hero.MainHero)
				{
					this._heroList.Insert(0, new CharacterVM(hero, new Action(this.OnPerkSelection)));
				}
				else
				{
					this._heroList.Add(new CharacterVM(hero, new Action(this.OnPerkSelection)));
				}
			}
			this._heroIndex = 0;
			this.CharacterList = new SelectorVM<SelectorItemVM>(new List<string>(), this._heroIndex, new Action<SelectorVM<SelectorItemVM>>(this.OnCharacterSelection));
			this.RefreshCharacterSelector();
			this.IsPlayerAccompanied = this._heroList.Count > 1;
			this.SetCurrentHero(this._heroList[this._heroIndex]);
			this._viewDataTracker.ClearCharacterNotification();
			this.UnopenedPerksNumForOtherChars = this._heroList.Sum(delegate(CharacterVM h)
			{
				if (h != this.CurrentCharacter)
				{
					return h.GetNumberOfUnselectedPerks();
				}
				return 0;
			});
			Game.Current.EventManager.RegisterEvent<TutorialNotificationElementChangeEvent>(new Action<TutorialNotificationElementChangeEvent>(this.OnTutorialNotificationElementIDChange));
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.DoneLbl = GameTexts.FindText("str_done", null).ToString();
			this.ResetLbl = GameTexts.FindText("str_reset", null).ToString();
			this.CancelLbl = GameTexts.FindText("str_cancel", null).ToString();
			this.SkillsText = GameTexts.FindText("str_skills", null).ToString();
			this.AddFocusText = GameTexts.FindText("str_add_focus", null).ToString();
			this.UnspentCharacterPointsText = GameTexts.FindText("str_character_unspent_character_points", null).ToString();
			this.TraitsText = new TextObject("{=FYJC7cDD}Trait(s)", null).ToString();
			this.PartyRoleText = new TextObject("{=9FJi2SaE}Party Role", null).ToString();
			this.ResetHint = new HintViewModel(GameTexts.FindText("str_reset", null), null);
			this.SkillFocusText = GameTexts.FindText("str_character_skill_focus", null).ToString();
			this.FocusVisualHint = new HintViewModel(new TextObject("{=GwA9oUBC}Your skill focus determines the rate your skill increases with practice", null), null);
			GameTexts.SetVariable("FOCUS_PER_LEVEL", Campaign.Current.Models.CharacterDevelopmentModel.FocusPointsPerLevel);
			GameTexts.SetVariable("ATTRIBUTE_EVERY_LEVEL", Campaign.Current.Models.CharacterDevelopmentModel.LevelsPerAttributePoint);
			this.UnspentCharacterPointsHint = new HintViewModel(GameTexts.FindText("str_character_points_how_to_get", null), null);
			this.UnspentAttributePointsHint = new HintViewModel(GameTexts.FindText("str_attribute_points_how_to_get", null), null);
			this.SetPreviousCharacterHint();
			this.SetNextCharacterHint();
			this.CharacterList.RefreshValues();
			this.CurrentCharacter.RefreshValues();
		}

		private void SetPreviousCharacterHint()
		{
			this.PreviousCharacterHint = new BasicTooltipViewModel(delegate
			{
				GameTexts.SetVariable("HOTKEY", this.GetPreviousCharacterKeyText());
				GameTexts.SetVariable("TEXT", GameTexts.FindText("str_inventory_prev_char", null));
				return GameTexts.FindText("str_hotkey_with_hint", null).ToString();
			});
		}

		private void SetNextCharacterHint()
		{
			this.NextCharacterHint = new BasicTooltipViewModel(delegate
			{
				GameTexts.SetVariable("HOTKEY", this.GetNextCharacterKeyText());
				GameTexts.SetVariable("TEXT", GameTexts.FindText("str_inventory_next_char", null));
				return GameTexts.FindText("str_hotkey_with_hint", null).ToString();
			});
		}

		public void SelectHero(Hero hero)
		{
			for (int i = 0; i < this._heroList.Count; i++)
			{
				if (this._heroList[i].Hero == hero)
				{
					this._heroIndex = i;
					this.RefreshCharacterSelector();
					return;
				}
			}
		}

		private void OnCharacterSelection(SelectorVM<SelectorItemVM> newIndex)
		{
			if (newIndex.SelectedIndex >= 0 && newIndex.SelectedIndex < this._heroList.Count)
			{
				this._heroIndex = newIndex.SelectedIndex;
				this.SetCurrentHero(this._heroList[this._heroIndex]);
				this.UnopenedPerksNumForOtherChars = this._heroList.Sum(delegate(CharacterVM h)
				{
					if (h != this.CurrentCharacter)
					{
						return h.GetNumberOfUnselectedPerks();
					}
					return 0;
				});
				this.HasUnopenedPerksForOtherCharacters = this._heroList[this._heroIndex].GetNumberOfUnselectedPerks() > 0;
			}
		}

		private void OnPerkSelection()
		{
			this.RefreshCharacterSelector();
		}

		private void RefreshCharacterSelector()
		{
			List<string> list = new List<string>();
			for (int i = 0; i < this._heroList.Count; i++)
			{
				string text = this._heroList[i].HeroNameText;
				if (this._heroList[i].GetNumberOfUnselectedPerks() > 0)
				{
					text = GameTexts.FindText("str_STR1_space_STR2", null).SetTextVariable("STR1", text).SetTextVariable("STR2", "{=!}<img src=\"CharacterDeveloper\\UnselectedPerksIcon\" extend=\"2\">")
						.ToString();
				}
				list.Add(text);
			}
			this.CharacterList.Refresh(list, this._heroIndex, new Action<SelectorVM<SelectorItemVM>>(this.OnCharacterSelection));
		}

		public void ExecuteReset()
		{
			foreach (CharacterVM characterVM in this._heroList)
			{
				characterVM.ResetChanges(false);
			}
			this.RefreshCharacterSelector();
		}

		public void ExecuteDone()
		{
			this.ApplyAllChanges();
			this._closeCharacterDeveloper();
		}

		public void ExecuteCancel()
		{
			foreach (CharacterVM characterVM in this._heroList)
			{
				characterVM.ResetChanges(true);
			}
			this._closeCharacterDeveloper();
		}

		private void SetCurrentHero(CharacterVM currentHero)
		{
			CharacterDeveloperVM.<>c__DisplayClass18_0 CS$<>8__locals1 = new CharacterDeveloperVM.<>c__DisplayClass18_0();
			CharacterDeveloperVM.<>c__DisplayClass18_0 CS$<>8__locals2 = CS$<>8__locals1;
			CharacterVM currentCharacter = this.CurrentCharacter;
			SkillObject skillObject;
			if (currentCharacter == null)
			{
				skillObject = null;
			}
			else
			{
				SkillVM skillVM = currentCharacter.Skills.FirstOrDefault((SkillVM s) => s.IsInspected);
				skillObject = ((skillVM != null) ? skillVM.Skill : null);
			}
			CS$<>8__locals2.prevSkill = skillObject;
			this.CurrentCharacter = currentHero;
			if (CS$<>8__locals1.prevSkill != null)
			{
				CharacterVM currentCharacter2 = this.CurrentCharacter;
				if (currentCharacter2 == null)
				{
					return;
				}
				currentCharacter2.SetCurrentSkill(this.CurrentCharacter.Skills.FirstOrDefault((SkillVM s) => s.Skill == CS$<>8__locals1.prevSkill));
			}
		}

		public void ApplyAllChanges()
		{
			foreach (CharacterVM characterVM in this._heroList)
			{
				characterVM.ApplyChanges();
			}
		}

		public bool IsThereAnyChanges()
		{
			return this._heroList.Any((CharacterVM c) => c.IsThereAnyChanges());
		}

		private List<Hero> GetApplicableHeroes()
		{
			List<Hero> list = new List<Hero>();
			Func<Hero, bool> func = (Hero x) => x != null && x.HeroState != Hero.CharacterStates.Disabled && x.IsAlive && !x.IsChild;
			Clan playerClan = Clan.PlayerClan;
			IEnumerable<Hero> enumerable = ((playerClan != null) ? playerClan.Heroes : null);
			foreach (Hero hero in (enumerable ?? Enumerable.Empty<Hero>()))
			{
				if (func(hero))
				{
					list.Add(hero);
				}
			}
			Clan playerClan2 = Clan.PlayerClan;
			enumerable = ((playerClan2 != null) ? playerClan2.Companions : null);
			foreach (Hero hero2 in (enumerable ?? Enumerable.Empty<Hero>()))
			{
				if (func(hero2) && !list.Contains(hero2))
				{
					list.Add(hero2);
				}
			}
			return list;
		}

		private void OnTutorialNotificationElementIDChange(TutorialNotificationElementChangeEvent obj)
		{
			if (obj.NewNotificationElementID != this._latestTutorialElementID)
			{
				if (this._latestTutorialElementID != null)
				{
					this.TutorialNotification.ElementID = string.Empty;
					if (this._isActivePerkHighlightsApplied)
					{
						this.SetAvailablePerksHighlightState(false);
						this._isActivePerkHighlightsApplied = false;
					}
				}
				this._latestTutorialElementID = obj.NewNotificationElementID;
				if (this._latestTutorialElementID != null)
				{
					this.TutorialNotification.ElementID = this._latestTutorialElementID;
					if (!this._isActivePerkHighlightsApplied && this._latestTutorialElementID == this._availablePerksHighlighId)
					{
						this.SetAvailablePerksHighlightState(true);
						this._isActivePerkHighlightsApplied = true;
						SkillVM skillVM = this.CurrentCharacter.Skills.FirstOrDefault((SkillVM s) => s.NumOfUnopenedPerks > 0);
						if (skillVM == null)
						{
							return;
						}
						skillVM.ExecuteInspect();
					}
				}
			}
		}

		private void SetAvailablePerksHighlightState(bool state)
		{
			foreach (SkillVM skillVM in this.CurrentCharacter.Skills)
			{
				foreach (PerkVM perkVM in skillVM.Perks)
				{
					if (state && perkVM.CurrentState == PerkVM.PerkStates.EarnedButNotSelected)
					{
						perkVM.IsTutorialHighlightEnabled = true;
					}
					else if (!state)
					{
						perkVM.IsTutorialHighlightEnabled = false;
					}
				}
			}
		}

		public override void OnFinalize()
		{
			base.OnFinalize();
			Game.Current.EventManager.UnregisterEvent<TutorialNotificationElementChangeEvent>(new Action<TutorialNotificationElementChangeEvent>(this.OnTutorialNotificationElementIDChange));
			this.CancelInputKey.OnFinalize();
			this.DoneInputKey.OnFinalize();
			this.PreviousCharacterInputKey.OnFinalize();
			this.NextCharacterInputKey.OnFinalize();
			this._heroList.ForEach(delegate(CharacterVM h)
			{
				h.OnFinalize();
			});
		}

		[DataSourceProperty]
		public string CurrentCharacterNameText
		{
			get
			{
				return this._currentCharacterNameText;
			}
			set
			{
				if (value != this._currentCharacterNameText)
				{
					this._currentCharacterNameText = value;
					base.OnPropertyChangedWithValue<string>(value, "CurrentCharacterNameText");
				}
			}
		}

		[DataSourceProperty]
		public CharacterVM CurrentCharacter
		{
			get
			{
				return this._currentCharacter;
			}
			set
			{
				if (value != this._currentCharacter)
				{
					this._currentCharacter = value;
					this.CurrentCharacterNameText = this._currentCharacter.HeroNameText;
					base.OnPropertyChangedWithValue<CharacterVM>(value, "CurrentCharacter");
				}
			}
		}

		[DataSourceProperty]
		public SelectorVM<SelectorItemVM> CharacterList
		{
			get
			{
				return this._characterList;
			}
			set
			{
				if (value != this._characterList)
				{
					this._characterList = value;
					base.OnPropertyChangedWithValue<SelectorVM<SelectorItemVM>>(value, "CharacterList");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel FocusVisualHint
		{
			get
			{
				return this._focusVisualHint;
			}
			set
			{
				if (value != this._focusVisualHint)
				{
					this._focusVisualHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "FocusVisualHint");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel ResetHint
		{
			get
			{
				return this._resetHint;
			}
			set
			{
				if (value != this._resetHint)
				{
					this._resetHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "ResetHint");
				}
			}
		}

		[DataSourceProperty]
		public ElementNotificationVM TutorialNotification
		{
			get
			{
				return this._tutorialNotification;
			}
			set
			{
				if (value != this._tutorialNotification)
				{
					this._tutorialNotification = value;
					base.OnPropertyChangedWithValue<ElementNotificationVM>(value, "TutorialNotification");
				}
			}
		}

		[DataSourceProperty]
		public bool IsPlayerAccompanied
		{
			get
			{
				return this._isPlayerAccompanied;
			}
			set
			{
				if (value != this._isPlayerAccompanied)
				{
					this._isPlayerAccompanied = value;
					base.OnPropertyChangedWithValue(value, "IsPlayerAccompanied");
				}
			}
		}

		[DataSourceProperty]
		public string UnspentCharacterPointsText
		{
			get
			{
				return this._unspentFocusPointsText;
			}
			set
			{
				if (value != this._unspentFocusPointsText)
				{
					this._unspentFocusPointsText = value;
					base.OnPropertyChangedWithValue<string>(value, "UnspentCharacterPointsText");
				}
			}
		}

		[DataSourceProperty]
		public string TraitsText
		{
			get
			{
				return this._traitsText;
			}
			set
			{
				if (value != this._traitsText)
				{
					this._traitsText = value;
					base.OnPropertyChangedWithValue<string>(value, "TraitsText");
				}
			}
		}

		[DataSourceProperty]
		public string PartyRoleText
		{
			get
			{
				return this._partyRoleText;
			}
			set
			{
				if (value != this._partyRoleText)
				{
					this._partyRoleText = value;
					base.OnPropertyChangedWithValue<string>(value, "PartyRoleText");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel UnspentCharacterPointsHint
		{
			get
			{
				return this._unspentCharacterPointsHint;
			}
			set
			{
				if (value != this._unspentCharacterPointsHint)
				{
					this._unspentCharacterPointsHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "UnspentCharacterPointsHint");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel UnspentAttributePointsHint
		{
			get
			{
				return this._unspentAttributePointsHint;
			}
			set
			{
				if (value != this._unspentAttributePointsHint)
				{
					this._unspentAttributePointsHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "UnspentAttributePointsHint");
				}
			}
		}

		[DataSourceProperty]
		public BasicTooltipViewModel PreviousCharacterHint
		{
			get
			{
				return this._previousCharacterHint;
			}
			set
			{
				if (value != this._previousCharacterHint)
				{
					this._previousCharacterHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "PreviousCharacterHint");
				}
			}
		}

		[DataSourceProperty]
		public BasicTooltipViewModel NextCharacterHint
		{
			get
			{
				return this._nextCharacterHint;
			}
			set
			{
				if (value != this._nextCharacterHint)
				{
					this._nextCharacterHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "NextCharacterHint");
				}
			}
		}

		[DataSourceProperty]
		public string DoneLbl
		{
			get
			{
				return this._doneLbl;
			}
			set
			{
				if (value != this._doneLbl)
				{
					this._doneLbl = value;
					base.OnPropertyChangedWithValue<string>(value, "DoneLbl");
				}
			}
		}

		[DataSourceProperty]
		public string ResetLbl
		{
			get
			{
				return this._resetLbl;
			}
			set
			{
				if (value != this._resetLbl)
				{
					this._resetLbl = value;
					base.OnPropertyChangedWithValue<string>(value, "ResetLbl");
				}
			}
		}

		[DataSourceProperty]
		public string CancelLbl
		{
			get
			{
				return this._cancelLbl;
			}
			set
			{
				if (value != this._cancelLbl)
				{
					this._cancelLbl = value;
					base.OnPropertyChangedWithValue<string>(value, "CancelLbl");
				}
			}
		}

		[DataSourceProperty]
		public string SkillFocusText
		{
			get
			{
				return this._skillFocusText;
			}
			set
			{
				if (value != this._skillFocusText)
				{
					this._skillFocusText = value;
					base.OnPropertyChangedWithValue<string>(value, "SkillFocusText");
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
		public string SkillsText
		{
			get
			{
				return this._skillsText;
			}
			set
			{
				if (value != this._skillsText)
				{
					this._skillsText = value;
					base.OnPropertyChangedWithValue<string>(value, "SkillsText");
				}
			}
		}

		[DataSourceProperty]
		public int UnopenedPerksNumForOtherChars
		{
			get
			{
				return this._unopenedPerksNumForOtherChars;
			}
			set
			{
				if (value != this._unopenedPerksNumForOtherChars)
				{
					this._unopenedPerksNumForOtherChars = value;
					base.OnPropertyChangedWithValue(value, "UnopenedPerksNumForOtherChars");
				}
			}
		}

		[DataSourceProperty]
		public bool HasUnopenedPerksForOtherCharacters
		{
			get
			{
				return this._hasUnopenedPerksForCurrentCharacter;
			}
			set
			{
				if (value != this._hasUnopenedPerksForCurrentCharacter)
				{
					this._hasUnopenedPerksForCurrentCharacter = value;
					base.OnPropertyChangedWithValue(value, "HasUnopenedPerksForOtherCharacters");
				}
			}
		}

		private TextObject GetPreviousCharacterKeyText()
		{
			if (this.PreviousCharacterInputKey == null || this._getKeyTextFromKeyId == null)
			{
				return TextObject.Empty;
			}
			return this._getKeyTextFromKeyId(this.PreviousCharacterInputKey.KeyID);
		}

		private TextObject GetNextCharacterKeyText()
		{
			if (this.NextCharacterInputKey == null || this._getKeyTextFromKeyId == null)
			{
				return TextObject.Empty;
			}
			return this._getKeyTextFromKeyId(this.NextCharacterInputKey.KeyID);
		}

		public void SetCancelInputKey(HotKey gameKey)
		{
			this.CancelInputKey = InputKeyItemVM.CreateFromHotKey(gameKey, true);
		}

		public void SetDoneInputKey(HotKey hotKey)
		{
			this.DoneInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		public void SetResetInputKey(HotKey hotKey)
		{
			this.ResetInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		public void SetPreviousCharacterInputKey(HotKey hotKey)
		{
			this.PreviousCharacterInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
			this.SetPreviousCharacterHint();
		}

		public void SetNextCharacterInputKey(HotKey hotKey)
		{
			this.NextCharacterInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
			this.SetNextCharacterHint();
		}

		public void SetGetKeyTextFromKeyIDFunc(Func<string, TextObject> getKeyTextFromKeyId)
		{
			this._getKeyTextFromKeyId = getKeyTextFromKeyId;
		}

		[DataSourceProperty]
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

		[DataSourceProperty]
		public InputKeyItemVM DoneInputKey
		{
			get
			{
				return this._doneInputKey;
			}
			set
			{
				if (value != this._doneInputKey)
				{
					this._doneInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "DoneInputKey");
				}
			}
		}

		[DataSourceProperty]
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

		[DataSourceProperty]
		public InputKeyItemVM PreviousCharacterInputKey
		{
			get
			{
				return this._previousCharacterInputKey;
			}
			set
			{
				if (value != this._previousCharacterInputKey)
				{
					this._previousCharacterInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "PreviousCharacterInputKey");
				}
			}
		}

		[DataSourceProperty]
		public InputKeyItemVM NextCharacterInputKey
		{
			get
			{
				return this._nextCharacterInputKey;
			}
			set
			{
				if (value != this._nextCharacterInputKey)
				{
					this._nextCharacterInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "NextCharacterInputKey");
				}
			}
		}

		private readonly Action _closeCharacterDeveloper;

		private readonly List<CharacterVM> _heroList;

		private readonly IViewDataTracker _viewDataTracker;

		public readonly ReadOnlyCollection<CharacterVM> HeroList;

		private int _heroIndex;

		private string _latestTutorialElementID;

		private Func<string, TextObject> _getKeyTextFromKeyId;

		private bool _isActivePerkHighlightsApplied;

		private readonly string _availablePerksHighlighId = "AvailablePerks";

		private string _skillsText;

		private string _doneLbl;

		private string _resetLbl;

		private string _cancelLbl;

		private string _unspentFocusPointsText;

		private string _traitsText;

		private string _partyRoleText;

		private HintViewModel _unspentCharacterPointsHint;

		private HintViewModel _unspentAttributePointsHint;

		private BasicTooltipViewModel _previousCharacterHint;

		private BasicTooltipViewModel _nextCharacterHint;

		private string _addFocusText;

		private bool _isPlayerAccompanied;

		private string _skillFocusText;

		private ElementNotificationVM _tutorialNotification;

		private HintViewModel _resetHint;

		private HintViewModel _focusVisualHint;

		private CharacterVM _currentCharacter;

		private string _currentCharacterNameText;

		private SelectorVM<SelectorItemVM> _characterList;

		private int _unopenedPerksNumForOtherChars;

		private bool _hasUnopenedPerksForCurrentCharacter;

		private InputKeyItemVM _cancelInputKey;

		private InputKeyItemVM _doneInputKey;

		private InputKeyItemVM _resetInputKey;

		private InputKeyItemVM _previousCharacterInputKey;

		private InputKeyItemVM _nextCharacterInputKey;
	}
}
