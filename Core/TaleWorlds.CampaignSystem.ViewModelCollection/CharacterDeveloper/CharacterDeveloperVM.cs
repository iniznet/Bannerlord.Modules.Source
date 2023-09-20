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
	// Token: 0x02000118 RID: 280
	public class CharacterDeveloperVM : ViewModel
	{
		// Token: 0x06001ACA RID: 6858 RVA: 0x00061240 File Offset: 0x0005F440
		public CharacterDeveloperVM(Action closeCharacterDeveloper)
		{
			this._closeCharacterDeveloper = closeCharacterDeveloper;
			this.TutorialNotification = new ElementNotificationVM();
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
			Campaign.Current.PlayerUpdateTracker.ClearCharacterNotification();
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

		// Token: 0x06001ACB RID: 6859 RVA: 0x000613B8 File Offset: 0x0005F5B8
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

		// Token: 0x06001ACC RID: 6860 RVA: 0x0006154B File Offset: 0x0005F74B
		private void SetPreviousCharacterHint()
		{
			this.PreviousCharacterHint = new BasicTooltipViewModel(delegate
			{
				GameTexts.SetVariable("HOTKEY", this.GetPreviousCharacterKeyText());
				GameTexts.SetVariable("TEXT", GameTexts.FindText("str_inventory_prev_char", null));
				return GameTexts.FindText("str_hotkey_with_hint", null).ToString();
			});
		}

		// Token: 0x06001ACD RID: 6861 RVA: 0x00061564 File Offset: 0x0005F764
		private void SetNextCharacterHint()
		{
			this.NextCharacterHint = new BasicTooltipViewModel(delegate
			{
				GameTexts.SetVariable("HOTKEY", this.GetNextCharacterKeyText());
				GameTexts.SetVariable("TEXT", GameTexts.FindText("str_inventory_next_char", null));
				return GameTexts.FindText("str_hotkey_with_hint", null).ToString();
			});
		}

		// Token: 0x06001ACE RID: 6862 RVA: 0x00061580 File Offset: 0x0005F780
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

		// Token: 0x06001ACF RID: 6863 RVA: 0x000615C8 File Offset: 0x0005F7C8
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

		// Token: 0x06001AD0 RID: 6864 RVA: 0x00061650 File Offset: 0x0005F850
		private void OnPerkSelection()
		{
			this.RefreshCharacterSelector();
		}

		// Token: 0x06001AD1 RID: 6865 RVA: 0x00061658 File Offset: 0x0005F858
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

		// Token: 0x06001AD2 RID: 6866 RVA: 0x000616F8 File Offset: 0x0005F8F8
		public void ExecuteReset()
		{
			foreach (CharacterVM characterVM in this._heroList)
			{
				characterVM.ResetChanges(false);
			}
			this.RefreshCharacterSelector();
		}

		// Token: 0x06001AD3 RID: 6867 RVA: 0x00061750 File Offset: 0x0005F950
		public void ExecuteDone()
		{
			this.ApplyAllChanges();
			this._closeCharacterDeveloper();
		}

		// Token: 0x06001AD4 RID: 6868 RVA: 0x00061764 File Offset: 0x0005F964
		public void ExecuteCancel()
		{
			foreach (CharacterVM characterVM in this._heroList)
			{
				characterVM.ResetChanges(true);
			}
			this._closeCharacterDeveloper();
		}

		// Token: 0x06001AD5 RID: 6869 RVA: 0x000617C0 File Offset: 0x0005F9C0
		private void SetCurrentHero(CharacterVM currentHero)
		{
			CharacterDeveloperVM.<>c__DisplayClass17_0 CS$<>8__locals1 = new CharacterDeveloperVM.<>c__DisplayClass17_0();
			CharacterDeveloperVM.<>c__DisplayClass17_0 CS$<>8__locals2 = CS$<>8__locals1;
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

		// Token: 0x06001AD6 RID: 6870 RVA: 0x00061858 File Offset: 0x0005FA58
		public void ApplyAllChanges()
		{
			foreach (CharacterVM characterVM in this._heroList)
			{
				characterVM.ApplyChanges();
			}
		}

		// Token: 0x06001AD7 RID: 6871 RVA: 0x000618A8 File Offset: 0x0005FAA8
		public bool IsThereAnyChanges()
		{
			return this._heroList.Any((CharacterVM c) => c.IsThereAnyChanges());
		}

		// Token: 0x06001AD8 RID: 6872 RVA: 0x000618D4 File Offset: 0x0005FAD4
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

		// Token: 0x06001AD9 RID: 6873 RVA: 0x000619D4 File Offset: 0x0005FBD4
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

		// Token: 0x06001ADA RID: 6874 RVA: 0x00061AAC File Offset: 0x0005FCAC
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

		// Token: 0x06001ADB RID: 6875 RVA: 0x00061B4C File Offset: 0x0005FD4C
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

		// Token: 0x1700092C RID: 2348
		// (get) Token: 0x06001ADC RID: 6876 RVA: 0x00061BD0 File Offset: 0x0005FDD0
		// (set) Token: 0x06001ADD RID: 6877 RVA: 0x00061BD8 File Offset: 0x0005FDD8
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

		// Token: 0x1700092D RID: 2349
		// (get) Token: 0x06001ADE RID: 6878 RVA: 0x00061BFB File Offset: 0x0005FDFB
		// (set) Token: 0x06001ADF RID: 6879 RVA: 0x00061C03 File Offset: 0x0005FE03
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

		// Token: 0x1700092E RID: 2350
		// (get) Token: 0x06001AE0 RID: 6880 RVA: 0x00061C32 File Offset: 0x0005FE32
		// (set) Token: 0x06001AE1 RID: 6881 RVA: 0x00061C3A File Offset: 0x0005FE3A
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

		// Token: 0x1700092F RID: 2351
		// (get) Token: 0x06001AE2 RID: 6882 RVA: 0x00061C58 File Offset: 0x0005FE58
		// (set) Token: 0x06001AE3 RID: 6883 RVA: 0x00061C60 File Offset: 0x0005FE60
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

		// Token: 0x17000930 RID: 2352
		// (get) Token: 0x06001AE4 RID: 6884 RVA: 0x00061C7E File Offset: 0x0005FE7E
		// (set) Token: 0x06001AE5 RID: 6885 RVA: 0x00061C86 File Offset: 0x0005FE86
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

		// Token: 0x17000931 RID: 2353
		// (get) Token: 0x06001AE6 RID: 6886 RVA: 0x00061CA4 File Offset: 0x0005FEA4
		// (set) Token: 0x06001AE7 RID: 6887 RVA: 0x00061CAC File Offset: 0x0005FEAC
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

		// Token: 0x17000932 RID: 2354
		// (get) Token: 0x06001AE8 RID: 6888 RVA: 0x00061CCA File Offset: 0x0005FECA
		// (set) Token: 0x06001AE9 RID: 6889 RVA: 0x00061CD2 File Offset: 0x0005FED2
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

		// Token: 0x17000933 RID: 2355
		// (get) Token: 0x06001AEA RID: 6890 RVA: 0x00061CF0 File Offset: 0x0005FEF0
		// (set) Token: 0x06001AEB RID: 6891 RVA: 0x00061CF8 File Offset: 0x0005FEF8
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

		// Token: 0x17000934 RID: 2356
		// (get) Token: 0x06001AEC RID: 6892 RVA: 0x00061D1B File Offset: 0x0005FF1B
		// (set) Token: 0x06001AED RID: 6893 RVA: 0x00061D23 File Offset: 0x0005FF23
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

		// Token: 0x17000935 RID: 2357
		// (get) Token: 0x06001AEE RID: 6894 RVA: 0x00061D46 File Offset: 0x0005FF46
		// (set) Token: 0x06001AEF RID: 6895 RVA: 0x00061D4E File Offset: 0x0005FF4E
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

		// Token: 0x17000936 RID: 2358
		// (get) Token: 0x06001AF0 RID: 6896 RVA: 0x00061D71 File Offset: 0x0005FF71
		// (set) Token: 0x06001AF1 RID: 6897 RVA: 0x00061D79 File Offset: 0x0005FF79
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

		// Token: 0x17000937 RID: 2359
		// (get) Token: 0x06001AF2 RID: 6898 RVA: 0x00061D97 File Offset: 0x0005FF97
		// (set) Token: 0x06001AF3 RID: 6899 RVA: 0x00061D9F File Offset: 0x0005FF9F
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

		// Token: 0x17000938 RID: 2360
		// (get) Token: 0x06001AF4 RID: 6900 RVA: 0x00061DBD File Offset: 0x0005FFBD
		// (set) Token: 0x06001AF5 RID: 6901 RVA: 0x00061DC5 File Offset: 0x0005FFC5
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

		// Token: 0x17000939 RID: 2361
		// (get) Token: 0x06001AF6 RID: 6902 RVA: 0x00061DE3 File Offset: 0x0005FFE3
		// (set) Token: 0x06001AF7 RID: 6903 RVA: 0x00061DEB File Offset: 0x0005FFEB
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

		// Token: 0x1700093A RID: 2362
		// (get) Token: 0x06001AF8 RID: 6904 RVA: 0x00061E09 File Offset: 0x00060009
		// (set) Token: 0x06001AF9 RID: 6905 RVA: 0x00061E11 File Offset: 0x00060011
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

		// Token: 0x1700093B RID: 2363
		// (get) Token: 0x06001AFA RID: 6906 RVA: 0x00061E34 File Offset: 0x00060034
		// (set) Token: 0x06001AFB RID: 6907 RVA: 0x00061E3C File Offset: 0x0006003C
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

		// Token: 0x1700093C RID: 2364
		// (get) Token: 0x06001AFC RID: 6908 RVA: 0x00061E5F File Offset: 0x0006005F
		// (set) Token: 0x06001AFD RID: 6909 RVA: 0x00061E67 File Offset: 0x00060067
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

		// Token: 0x1700093D RID: 2365
		// (get) Token: 0x06001AFE RID: 6910 RVA: 0x00061E8A File Offset: 0x0006008A
		// (set) Token: 0x06001AFF RID: 6911 RVA: 0x00061E92 File Offset: 0x00060092
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

		// Token: 0x1700093E RID: 2366
		// (get) Token: 0x06001B00 RID: 6912 RVA: 0x00061EB5 File Offset: 0x000600B5
		// (set) Token: 0x06001B01 RID: 6913 RVA: 0x00061EBD File Offset: 0x000600BD
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

		// Token: 0x1700093F RID: 2367
		// (get) Token: 0x06001B02 RID: 6914 RVA: 0x00061EE0 File Offset: 0x000600E0
		// (set) Token: 0x06001B03 RID: 6915 RVA: 0x00061EE8 File Offset: 0x000600E8
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

		// Token: 0x17000940 RID: 2368
		// (get) Token: 0x06001B04 RID: 6916 RVA: 0x00061F0B File Offset: 0x0006010B
		// (set) Token: 0x06001B05 RID: 6917 RVA: 0x00061F13 File Offset: 0x00060113
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

		// Token: 0x17000941 RID: 2369
		// (get) Token: 0x06001B06 RID: 6918 RVA: 0x00061F31 File Offset: 0x00060131
		// (set) Token: 0x06001B07 RID: 6919 RVA: 0x00061F39 File Offset: 0x00060139
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

		// Token: 0x06001B08 RID: 6920 RVA: 0x00061F57 File Offset: 0x00060157
		private TextObject GetPreviousCharacterKeyText()
		{
			if (this.PreviousCharacterInputKey == null || this._getKeyTextFromKeyId == null)
			{
				return TextObject.Empty;
			}
			return this._getKeyTextFromKeyId(this.PreviousCharacterInputKey.KeyID);
		}

		// Token: 0x06001B09 RID: 6921 RVA: 0x00061F85 File Offset: 0x00060185
		private TextObject GetNextCharacterKeyText()
		{
			if (this.NextCharacterInputKey == null || this._getKeyTextFromKeyId == null)
			{
				return TextObject.Empty;
			}
			return this._getKeyTextFromKeyId(this.NextCharacterInputKey.KeyID);
		}

		// Token: 0x06001B0A RID: 6922 RVA: 0x00061FB3 File Offset: 0x000601B3
		public void SetCancelInputKey(HotKey gameKey)
		{
			this.CancelInputKey = InputKeyItemVM.CreateFromHotKey(gameKey, true);
		}

		// Token: 0x06001B0B RID: 6923 RVA: 0x00061FC2 File Offset: 0x000601C2
		public void SetDoneInputKey(HotKey hotKey)
		{
			this.DoneInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		// Token: 0x06001B0C RID: 6924 RVA: 0x00061FD1 File Offset: 0x000601D1
		public void SetResetInputKey(HotKey hotKey)
		{
			this.ResetInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		// Token: 0x06001B0D RID: 6925 RVA: 0x00061FE0 File Offset: 0x000601E0
		public void SetPreviousCharacterInputKey(HotKey hotKey)
		{
			this.PreviousCharacterInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
			this.SetPreviousCharacterHint();
		}

		// Token: 0x06001B0E RID: 6926 RVA: 0x00061FF5 File Offset: 0x000601F5
		public void SetNextCharacterInputKey(HotKey hotKey)
		{
			this.NextCharacterInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
			this.SetNextCharacterHint();
		}

		// Token: 0x06001B0F RID: 6927 RVA: 0x0006200A File Offset: 0x0006020A
		public void SetGetKeyTextFromKeyIDFunc(Func<string, TextObject> getKeyTextFromKeyId)
		{
			this._getKeyTextFromKeyId = getKeyTextFromKeyId;
		}

		// Token: 0x17000942 RID: 2370
		// (get) Token: 0x06001B10 RID: 6928 RVA: 0x00062013 File Offset: 0x00060213
		// (set) Token: 0x06001B11 RID: 6929 RVA: 0x0006201B File Offset: 0x0006021B
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

		// Token: 0x17000943 RID: 2371
		// (get) Token: 0x06001B12 RID: 6930 RVA: 0x00062039 File Offset: 0x00060239
		// (set) Token: 0x06001B13 RID: 6931 RVA: 0x00062041 File Offset: 0x00060241
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

		// Token: 0x17000944 RID: 2372
		// (get) Token: 0x06001B14 RID: 6932 RVA: 0x0006205F File Offset: 0x0006025F
		// (set) Token: 0x06001B15 RID: 6933 RVA: 0x00062067 File Offset: 0x00060267
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

		// Token: 0x17000945 RID: 2373
		// (get) Token: 0x06001B16 RID: 6934 RVA: 0x00062085 File Offset: 0x00060285
		// (set) Token: 0x06001B17 RID: 6935 RVA: 0x0006208D File Offset: 0x0006028D
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

		// Token: 0x17000946 RID: 2374
		// (get) Token: 0x06001B18 RID: 6936 RVA: 0x000620AB File Offset: 0x000602AB
		// (set) Token: 0x06001B19 RID: 6937 RVA: 0x000620B3 File Offset: 0x000602B3
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

		// Token: 0x04000CB1 RID: 3249
		private readonly Action _closeCharacterDeveloper;

		// Token: 0x04000CB2 RID: 3250
		private readonly List<CharacterVM> _heroList;

		// Token: 0x04000CB3 RID: 3251
		public readonly ReadOnlyCollection<CharacterVM> HeroList;

		// Token: 0x04000CB4 RID: 3252
		private int _heroIndex;

		// Token: 0x04000CB5 RID: 3253
		private string _latestTutorialElementID;

		// Token: 0x04000CB6 RID: 3254
		private Func<string, TextObject> _getKeyTextFromKeyId;

		// Token: 0x04000CB7 RID: 3255
		private bool _isActivePerkHighlightsApplied;

		// Token: 0x04000CB8 RID: 3256
		private readonly string _availablePerksHighlighId = "AvailablePerks";

		// Token: 0x04000CB9 RID: 3257
		private string _skillsText;

		// Token: 0x04000CBA RID: 3258
		private string _doneLbl;

		// Token: 0x04000CBB RID: 3259
		private string _resetLbl;

		// Token: 0x04000CBC RID: 3260
		private string _cancelLbl;

		// Token: 0x04000CBD RID: 3261
		private string _unspentFocusPointsText;

		// Token: 0x04000CBE RID: 3262
		private string _traitsText;

		// Token: 0x04000CBF RID: 3263
		private string _partyRoleText;

		// Token: 0x04000CC0 RID: 3264
		private HintViewModel _unspentCharacterPointsHint;

		// Token: 0x04000CC1 RID: 3265
		private HintViewModel _unspentAttributePointsHint;

		// Token: 0x04000CC2 RID: 3266
		private BasicTooltipViewModel _previousCharacterHint;

		// Token: 0x04000CC3 RID: 3267
		private BasicTooltipViewModel _nextCharacterHint;

		// Token: 0x04000CC4 RID: 3268
		private string _addFocusText;

		// Token: 0x04000CC5 RID: 3269
		private bool _isPlayerAccompanied;

		// Token: 0x04000CC6 RID: 3270
		private string _skillFocusText;

		// Token: 0x04000CC7 RID: 3271
		private ElementNotificationVM _tutorialNotification;

		// Token: 0x04000CC8 RID: 3272
		private HintViewModel _resetHint;

		// Token: 0x04000CC9 RID: 3273
		private HintViewModel _focusVisualHint;

		// Token: 0x04000CCA RID: 3274
		private CharacterVM _currentCharacter;

		// Token: 0x04000CCB RID: 3275
		private string _currentCharacterNameText;

		// Token: 0x04000CCC RID: 3276
		private SelectorVM<SelectorItemVM> _characterList;

		// Token: 0x04000CCD RID: 3277
		private int _unopenedPerksNumForOtherChars;

		// Token: 0x04000CCE RID: 3278
		private bool _hasUnopenedPerksForCurrentCharacter;

		// Token: 0x04000CCF RID: 3279
		private InputKeyItemVM _cancelInputKey;

		// Token: 0x04000CD0 RID: 3280
		private InputKeyItemVM _doneInputKey;

		// Token: 0x04000CD1 RID: 3281
		private InputKeyItemVM _resetInputKey;

		// Token: 0x04000CD2 RID: 3282
		private InputKeyItemVM _previousCharacterInputKey;

		// Token: 0x04000CD3 RID: 3283
		private InputKeyItemVM _nextCharacterInputKey;
	}
}
