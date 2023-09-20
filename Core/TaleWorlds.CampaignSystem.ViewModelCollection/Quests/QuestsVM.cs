using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Issues;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.CampaignSystem.ViewModelCollection.Input;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Core.ViewModelCollection.Selector;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Quests
{
	// Token: 0x02000022 RID: 34
	public class QuestsVM : ViewModel
	{
		// Token: 0x06000208 RID: 520 RVA: 0x000105C0 File Offset: 0x0000E7C0
		public QuestsVM(Action closeQuestsScreen)
		{
			this._closeQuestsScreen = closeQuestsScreen;
			this.ActiveQuestsList = new MBBindingList<QuestItemVM>();
			this.OldQuestsList = new MBBindingList<QuestItemVM>();
			this.CurrentQuestStages = new MBBindingList<QuestStageVM>();
			this._viewDataTracker = Campaign.Current.GetCampaignBehavior<IViewDataTracker>();
			QuestBase questSelection = this._viewDataTracker.GetQuestSelection();
			foreach (QuestBase questBase in Campaign.Current.QuestManager.Quests.Where((QuestBase Q) => Q.IsOngoing))
			{
				QuestItemVM questItemVM = new QuestItemVM(questBase, new Action<QuestItemVM>(this.SetSelectedItem));
				if (questSelection != null && questBase == questSelection)
				{
					this.SetSelectedItem(questItemVM);
				}
				this.ActiveQuestsList.Add(questItemVM);
			}
			foreach (KeyValuePair<Hero, IssueBase> keyValuePair in Campaign.Current.IssueManager.Issues.Where((KeyValuePair<Hero, IssueBase> i) => i.Value.IsSolvingWithAlternative))
			{
				QuestItemVM questItemVM2 = new QuestItemVM(keyValuePair.Value, new Action<QuestItemVM>(this.SetSelectedItem));
				this.ActiveQuestsList.Add(questItemVM2);
			}
			foreach (JournalLogEntry journalLogEntry in Campaign.Current.LogEntryHistory.GetGameActionLogs<JournalLogEntry>((JournalLogEntry JournalLogEntry) => true))
			{
				if (journalLogEntry.IsEnded())
				{
					QuestItemVM questItemVM3 = new QuestItemVM(journalLogEntry, new Action<QuestItemVM>(this.SetSelectedItem), journalLogEntry.IsEndedUnsuccessfully() ? QuestsVM.QuestCompletionType.UnSuccessful : QuestsVM.QuestCompletionType.Successful);
					this.OldQuestsList.Add(questItemVM3);
				}
			}
			Comparer<QuestItemVM> comparer = Comparer<QuestItemVM>.Create((QuestItemVM q1, QuestItemVM q2) => q1.IsMainQuest.CompareTo(q2.IsMainQuest));
			this.ActiveQuestsList.Sort(comparer);
			if (!this.OldQuestsList.Any((QuestItemVM q) => q.IsSelected))
			{
				if (!this.ActiveQuestsList.Any((QuestItemVM q) => q.IsSelected))
				{
					if (this.ActiveQuestsList.Count > 0)
					{
						this.SetSelectedItem(this.ActiveQuestsList.FirstOrDefault<QuestItemVM>());
					}
					else if (this.OldQuestsList.Count > 0)
					{
						this.SetSelectedItem(this.OldQuestsList.FirstOrDefault<QuestItemVM>());
					}
				}
			}
			this.IsThereAnyQuest = MathF.Max(this.ActiveQuestsList.Count, this.OldQuestsList.Count) > 0;
			List<TextObject> list = new List<TextObject>
			{
				new TextObject("{=7l0LGKRk}Date Started", null),
				new TextObject("{=Y8EcVL1c}Last Updated", null),
				new TextObject("{=BEXTcJaS}Time Due", null)
			};
			this.ActiveQuestsSortController = new QuestItemSortControllerVM(ref this._activeQuestsList);
			this.OldQuestsSortController = new QuestItemSortControllerVM(ref this._oldQuestsList);
			this.SortSelector = new SelectorVM<SelectorItemVM>(list, this._viewDataTracker.GetQuestSortTypeSelection(), new Action<SelectorVM<SelectorItemVM>>(this.OnSortOptionChanged));
			this.RefreshValues();
			Campaign campaign = Campaign.Current;
			if (campaign == null)
			{
				return;
			}
			PlayerUpdateTracker playerUpdateTracker = campaign.PlayerUpdateTracker;
			if (playerUpdateTracker == null)
			{
				return;
			}
			playerUpdateTracker.ClearQuestNotification();
		}

		// Token: 0x06000209 RID: 521 RVA: 0x00010964 File Offset: 0x0000EB64
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.QuestGiverText = GameTexts.FindText("str_quest_given_by", null).ToString();
			this.TimeRemainingLbl = GameTexts.FindText("str_time_remaining", null).ToString();
			this.QuestTitleText = GameTexts.FindText("str_quests", null).ToString();
			this.OldQuestsText = GameTexts.FindText("str_old_quests", null).ToString();
			this.ActiveQuestsText = GameTexts.FindText("str_active_quests", null).ToString();
			this.NoActiveQuestText = GameTexts.FindText("str_no_active_quest", null).ToString();
			this.SortQuestsText = GameTexts.FindText("str_sort_quests", null).ToString();
			this.OldQuestsHint = new HintViewModel(GameTexts.FindText("str_old_quests_explanation", null), null);
			this.DoneLbl = GameTexts.FindText("str_done", null).ToString();
			this.CurrentQuestStages.ApplyActionOnAllItems(delegate(QuestStageVM x)
			{
				x.RefreshValues();
			});
			this.ActiveQuestsList.ApplyActionOnAllItems(delegate(QuestItemVM x)
			{
				x.RefreshValues();
			});
			this.OldQuestsList.ApplyActionOnAllItems(delegate(QuestItemVM x)
			{
				x.RefreshValues();
			});
			QuestItemVM selectedQuest = this.SelectedQuest;
			if (selectedQuest != null)
			{
				selectedQuest.RefreshValues();
			}
			HeroVM currentQuestGiverHero = this.CurrentQuestGiverHero;
			if (currentQuestGiverHero == null)
			{
				return;
			}
			currentQuestGiverHero.RefreshValues();
		}

		// Token: 0x0600020A RID: 522 RVA: 0x00010AE0 File Offset: 0x0000ECE0
		private void SetSelectedItem(QuestItemVM quest)
		{
			if (this._selectedQuest != quest)
			{
				this.CurrentQuestStages.Clear();
				if (this._selectedQuest != null)
				{
					this._selectedQuest.IsSelected = false;
				}
				if (quest != null)
				{
					quest.IsSelected = true;
				}
				this.SelectedQuest = quest;
				if (this._selectedQuest != null)
				{
					this.CurrentQuestGiverHero = this._selectedQuest.QuestGiverHero;
					this.CurrentQuestTitle = this._selectedQuest.Name;
					this.IsCurrentQuestGiverHeroHidden = this._selectedQuest.IsQuestGiverHeroHidden;
					using (IEnumerator<QuestStageVM> enumerator = this._selectedQuest.Stages.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							QuestStageVM questStageVM = enumerator.Current;
							this.CurrentQuestStages.Add(questStageVM);
						}
						goto IL_D0;
					}
				}
				this.CurrentQuestGiverHero = new HeroVM(null, false);
				this.CurrentQuestTitle = "";
				this.IsCurrentQuestGiverHeroHidden = true;
			}
			IL_D0:
			this._viewDataTracker.SetQuestSelection(quest.Quest);
			this.TimeRemainingHint = new HintViewModel(new TextObject("{=2nN1QuxZ}This quest will be failed unless completed in this time.", null), null);
			foreach (QuestStageVM questStageVM2 in this._selectedQuest.Stages)
			{
				PlayerUpdateTracker.Current.OnQuestlogExamined(questStageVM2.Log);
				questStageVM2.UpdateIsNew();
				this._selectedQuest.UpdateIsUpdated();
			}
		}

		// Token: 0x0600020B RID: 523 RVA: 0x00010C50 File Offset: 0x0000EE50
		public void ExecuteOpenQuestGiverEncyclopedia()
		{
			HeroVM currentQuestGiverHero = this.CurrentQuestGiverHero;
			if (currentQuestGiverHero == null)
			{
				return;
			}
			currentQuestGiverHero.ExecuteLink();
		}

		// Token: 0x0600020C RID: 524 RVA: 0x00010C62 File Offset: 0x0000EE62
		public void ExecuteClose()
		{
			this._closeQuestsScreen();
		}

		// Token: 0x0600020D RID: 525 RVA: 0x00010C70 File Offset: 0x0000EE70
		public void SetSelectedIssue(IssueBase issue)
		{
			foreach (QuestItemVM questItemVM in this.ActiveQuestsList)
			{
				if (questItemVM.Issue == issue)
				{
					this.SetSelectedItem(questItemVM);
				}
			}
		}

		// Token: 0x0600020E RID: 526 RVA: 0x00010CC8 File Offset: 0x0000EEC8
		public void SetSelectedQuest(QuestBase quest)
		{
			foreach (QuestItemVM questItemVM in this.ActiveQuestsList)
			{
				if (questItemVM.Quest == quest)
				{
					this.SetSelectedItem(questItemVM);
				}
			}
		}

		// Token: 0x0600020F RID: 527 RVA: 0x00010D20 File Offset: 0x0000EF20
		public void SetSelectedLog(JournalLogEntry log)
		{
			foreach (QuestItemVM questItemVM in this.OldQuestsList)
			{
				if (questItemVM.QuestLogEntry == log)
				{
					this.SetSelectedItem(questItemVM);
				}
			}
		}

		// Token: 0x06000210 RID: 528 RVA: 0x00010D78 File Offset: 0x0000EF78
		public override void OnFinalize()
		{
			base.OnFinalize();
			InputKeyItemVM doneInputKey = this.DoneInputKey;
			if (doneInputKey == null)
			{
				return;
			}
			doneInputKey.OnFinalize();
		}

		// Token: 0x06000211 RID: 529 RVA: 0x00010D90 File Offset: 0x0000EF90
		private void OnSortOptionChanged(SelectorVM<SelectorItemVM> sortSelector)
		{
			this._viewDataTracker.SetQuestSortTypeSelection(sortSelector.SelectedIndex);
			this.ActiveQuestsSortController.SortByOption((QuestItemSortControllerVM.QuestItemSortOption)sortSelector.SelectedIndex);
			this.OldQuestsSortController.SortByOption((QuestItemSortControllerVM.QuestItemSortOption)sortSelector.SelectedIndex);
		}

		// Token: 0x06000212 RID: 530 RVA: 0x00010DC5 File Offset: 0x0000EFC5
		public void SetDoneInputKey(HotKey hotKey)
		{
			this.DoneInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		// Token: 0x17000072 RID: 114
		// (get) Token: 0x06000213 RID: 531 RVA: 0x00010DD4 File Offset: 0x0000EFD4
		// (set) Token: 0x06000214 RID: 532 RVA: 0x00010DDC File Offset: 0x0000EFDC
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

		// Token: 0x17000073 RID: 115
		// (get) Token: 0x06000215 RID: 533 RVA: 0x00010DFA File Offset: 0x0000EFFA
		// (set) Token: 0x06000216 RID: 534 RVA: 0x00010E02 File Offset: 0x0000F002
		[DataSourceProperty]
		public QuestItemVM SelectedQuest
		{
			get
			{
				return this._selectedQuest;
			}
			set
			{
				if (value != this._selectedQuest)
				{
					this._selectedQuest = value;
					base.OnPropertyChangedWithValue<QuestItemVM>(value, "SelectedQuest");
				}
			}
		}

		// Token: 0x17000074 RID: 116
		// (get) Token: 0x06000217 RID: 535 RVA: 0x00010E20 File Offset: 0x0000F020
		// (set) Token: 0x06000218 RID: 536 RVA: 0x00010E28 File Offset: 0x0000F028
		[DataSourceProperty]
		public MBBindingList<QuestItemVM> ActiveQuestsList
		{
			get
			{
				return this._activeQuestsList;
			}
			set
			{
				if (value != this._activeQuestsList)
				{
					this._activeQuestsList = value;
					base.OnPropertyChangedWithValue<MBBindingList<QuestItemVM>>(value, "ActiveQuestsList");
				}
			}
		}

		// Token: 0x17000075 RID: 117
		// (get) Token: 0x06000219 RID: 537 RVA: 0x00010E46 File Offset: 0x0000F046
		// (set) Token: 0x0600021A RID: 538 RVA: 0x00010E4E File Offset: 0x0000F04E
		[DataSourceProperty]
		public MBBindingList<QuestItemVM> OldQuestsList
		{
			get
			{
				return this._oldQuestsList;
			}
			set
			{
				if (value != this._oldQuestsList)
				{
					this._oldQuestsList = value;
					base.OnPropertyChangedWithValue<MBBindingList<QuestItemVM>>(value, "OldQuestsList");
				}
			}
		}

		// Token: 0x17000076 RID: 118
		// (get) Token: 0x0600021B RID: 539 RVA: 0x00010E6C File Offset: 0x0000F06C
		// (set) Token: 0x0600021C RID: 540 RVA: 0x00010E74 File Offset: 0x0000F074
		[DataSourceProperty]
		public HeroVM CurrentQuestGiverHero
		{
			get
			{
				return this._currentQuestGiverHero;
			}
			set
			{
				if (value != this._currentQuestGiverHero)
				{
					this._currentQuestGiverHero = value;
					base.OnPropertyChangedWithValue<HeroVM>(value, "CurrentQuestGiverHero");
				}
			}
		}

		// Token: 0x17000077 RID: 119
		// (get) Token: 0x0600021D RID: 541 RVA: 0x00010E92 File Offset: 0x0000F092
		// (set) Token: 0x0600021E RID: 542 RVA: 0x00010E9A File Offset: 0x0000F09A
		[DataSourceProperty]
		public string TimeRemainingLbl
		{
			get
			{
				return this._timeRemainingLbl;
			}
			set
			{
				if (value != this._timeRemainingLbl)
				{
					this._timeRemainingLbl = value;
					base.OnPropertyChangedWithValue<string>(value, "TimeRemainingLbl");
				}
			}
		}

		// Token: 0x17000078 RID: 120
		// (get) Token: 0x0600021F RID: 543 RVA: 0x00010EBD File Offset: 0x0000F0BD
		// (set) Token: 0x06000220 RID: 544 RVA: 0x00010EC5 File Offset: 0x0000F0C5
		[DataSourceProperty]
		public bool IsThereAnyQuest
		{
			get
			{
				return this._isThereAnyQuest;
			}
			set
			{
				if (value != this._isThereAnyQuest)
				{
					this._isThereAnyQuest = value;
					base.OnPropertyChangedWithValue(value, "IsThereAnyQuest");
				}
			}
		}

		// Token: 0x17000079 RID: 121
		// (get) Token: 0x06000221 RID: 545 RVA: 0x00010EE3 File Offset: 0x0000F0E3
		// (set) Token: 0x06000222 RID: 546 RVA: 0x00010EEB File Offset: 0x0000F0EB
		[DataSourceProperty]
		public string NoActiveQuestText
		{
			get
			{
				return this._noActiveQuestText;
			}
			set
			{
				if (value != this._noActiveQuestText)
				{
					this._noActiveQuestText = value;
					base.OnPropertyChangedWithValue<string>(value, "NoActiveQuestText");
				}
			}
		}

		// Token: 0x1700007A RID: 122
		// (get) Token: 0x06000223 RID: 547 RVA: 0x00010F0E File Offset: 0x0000F10E
		// (set) Token: 0x06000224 RID: 548 RVA: 0x00010F16 File Offset: 0x0000F116
		[DataSourceProperty]
		public string SortQuestsText
		{
			get
			{
				return this._sortQuestsText;
			}
			set
			{
				if (value != this._sortQuestsText)
				{
					this._sortQuestsText = value;
					base.OnPropertyChangedWithValue<string>(value, "SortQuestsText");
				}
			}
		}

		// Token: 0x1700007B RID: 123
		// (get) Token: 0x06000225 RID: 549 RVA: 0x00010F39 File Offset: 0x0000F139
		// (set) Token: 0x06000226 RID: 550 RVA: 0x00010F41 File Offset: 0x0000F141
		[DataSourceProperty]
		public string QuestGiverText
		{
			get
			{
				return this._questGiverText;
			}
			set
			{
				if (value != this._questGiverText)
				{
					this._questGiverText = value;
					base.OnPropertyChangedWithValue<string>(value, "QuestGiverText");
				}
			}
		}

		// Token: 0x1700007C RID: 124
		// (get) Token: 0x06000227 RID: 551 RVA: 0x00010F64 File Offset: 0x0000F164
		// (set) Token: 0x06000228 RID: 552 RVA: 0x00010F6C File Offset: 0x0000F16C
		[DataSourceProperty]
		public string QuestTitleText
		{
			get
			{
				return this._questTitleText;
			}
			set
			{
				if (value != this._questTitleText)
				{
					this._questTitleText = value;
					base.OnPropertyChangedWithValue<string>(value, "QuestTitleText");
				}
			}
		}

		// Token: 0x1700007D RID: 125
		// (get) Token: 0x06000229 RID: 553 RVA: 0x00010F8F File Offset: 0x0000F18F
		// (set) Token: 0x0600022A RID: 554 RVA: 0x00010F97 File Offset: 0x0000F197
		[DataSourceProperty]
		public string OldQuestsText
		{
			get
			{
				return this._oldQuestsText;
			}
			set
			{
				if (value != this._oldQuestsText)
				{
					this._oldQuestsText = value;
					base.OnPropertyChangedWithValue<string>(value, "OldQuestsText");
				}
			}
		}

		// Token: 0x1700007E RID: 126
		// (get) Token: 0x0600022B RID: 555 RVA: 0x00010FBA File Offset: 0x0000F1BA
		// (set) Token: 0x0600022C RID: 556 RVA: 0x00010FC2 File Offset: 0x0000F1C2
		[DataSourceProperty]
		public string ActiveQuestsText
		{
			get
			{
				return this._activeQuestsText;
			}
			set
			{
				if (value != this._activeQuestsText)
				{
					this._activeQuestsText = value;
					base.OnPropertyChangedWithValue<string>(value, "ActiveQuestsText");
				}
			}
		}

		// Token: 0x1700007F RID: 127
		// (get) Token: 0x0600022D RID: 557 RVA: 0x00010FE5 File Offset: 0x0000F1E5
		// (set) Token: 0x0600022E RID: 558 RVA: 0x00010FED File Offset: 0x0000F1ED
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

		// Token: 0x17000080 RID: 128
		// (get) Token: 0x0600022F RID: 559 RVA: 0x00011010 File Offset: 0x0000F210
		// (set) Token: 0x06000230 RID: 560 RVA: 0x00011018 File Offset: 0x0000F218
		[DataSourceProperty]
		public string CurrentQuestTitle
		{
			get
			{
				return this._currentQuestTitle;
			}
			set
			{
				if (value != this._currentQuestTitle)
				{
					this._currentQuestTitle = value;
					base.OnPropertyChangedWithValue<string>(value, "CurrentQuestTitle");
				}
			}
		}

		// Token: 0x17000081 RID: 129
		// (get) Token: 0x06000231 RID: 561 RVA: 0x0001103B File Offset: 0x0000F23B
		// (set) Token: 0x06000232 RID: 562 RVA: 0x00011043 File Offset: 0x0000F243
		[DataSourceProperty]
		public bool IsCurrentQuestGiverHeroHidden
		{
			get
			{
				return this._isCurrentQuestGiverHeroHidden;
			}
			set
			{
				if (value != this._isCurrentQuestGiverHeroHidden)
				{
					this._isCurrentQuestGiverHeroHidden = value;
					base.OnPropertyChangedWithValue(value, "IsCurrentQuestGiverHeroHidden");
				}
			}
		}

		// Token: 0x17000082 RID: 130
		// (get) Token: 0x06000233 RID: 563 RVA: 0x00011061 File Offset: 0x0000F261
		// (set) Token: 0x06000234 RID: 564 RVA: 0x00011069 File Offset: 0x0000F269
		[DataSourceProperty]
		public MBBindingList<QuestStageVM> CurrentQuestStages
		{
			get
			{
				return this._currentQuestStages;
			}
			set
			{
				if (value != this._currentQuestStages)
				{
					this._currentQuestStages = value;
					base.OnPropertyChangedWithValue<MBBindingList<QuestStageVM>>(value, "CurrentQuestStages");
				}
			}
		}

		// Token: 0x17000083 RID: 131
		// (get) Token: 0x06000235 RID: 565 RVA: 0x00011087 File Offset: 0x0000F287
		// (set) Token: 0x06000236 RID: 566 RVA: 0x0001108F File Offset: 0x0000F28F
		[DataSourceProperty]
		public HintViewModel TimeRemainingHint
		{
			get
			{
				return this._timeRemainingHint;
			}
			set
			{
				if (value != this._timeRemainingHint)
				{
					this._timeRemainingHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "TimeRemainingHint");
				}
			}
		}

		// Token: 0x17000084 RID: 132
		// (get) Token: 0x06000237 RID: 567 RVA: 0x000110AD File Offset: 0x0000F2AD
		// (set) Token: 0x06000238 RID: 568 RVA: 0x000110B5 File Offset: 0x0000F2B5
		[DataSourceProperty]
		public HintViewModel OldQuestsHint
		{
			get
			{
				return this._oldQuestsHint;
			}
			set
			{
				if (value != this._oldQuestsHint)
				{
					this._oldQuestsHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "OldQuestsHint");
				}
			}
		}

		// Token: 0x17000085 RID: 133
		// (get) Token: 0x06000239 RID: 569 RVA: 0x000110D3 File Offset: 0x0000F2D3
		// (set) Token: 0x0600023A RID: 570 RVA: 0x000110DB File Offset: 0x0000F2DB
		[DataSourceProperty]
		public QuestItemSortControllerVM ActiveQuestsSortController
		{
			get
			{
				return this._activeQuestsSortController;
			}
			set
			{
				if (value != this._activeQuestsSortController)
				{
					this._activeQuestsSortController = value;
					base.OnPropertyChangedWithValue<QuestItemSortControllerVM>(value, "ActiveQuestsSortController");
				}
			}
		}

		// Token: 0x17000086 RID: 134
		// (get) Token: 0x0600023B RID: 571 RVA: 0x000110F9 File Offset: 0x0000F2F9
		// (set) Token: 0x0600023C RID: 572 RVA: 0x00011101 File Offset: 0x0000F301
		[DataSourceProperty]
		public QuestItemSortControllerVM OldQuestsSortController
		{
			get
			{
				return this._oldQuestsSortController;
			}
			set
			{
				if (value != this._oldQuestsSortController)
				{
					this._oldQuestsSortController = value;
					base.OnPropertyChangedWithValue<QuestItemSortControllerVM>(value, "OldQuestsSortController");
				}
			}
		}

		// Token: 0x17000087 RID: 135
		// (get) Token: 0x0600023D RID: 573 RVA: 0x0001111F File Offset: 0x0000F31F
		// (set) Token: 0x0600023E RID: 574 RVA: 0x00011127 File Offset: 0x0000F327
		[DataSourceProperty]
		public SelectorVM<SelectorItemVM> SortSelector
		{
			get
			{
				return this._sortSelector;
			}
			set
			{
				if (value != this._sortSelector)
				{
					this._sortSelector = value;
					base.OnPropertyChangedWithValue<SelectorVM<SelectorItemVM>>(value, "SortSelector");
				}
			}
		}

		// Token: 0x040000EE RID: 238
		private readonly Action _closeQuestsScreen;

		// Token: 0x040000EF RID: 239
		private readonly IViewDataTracker _viewDataTracker;

		// Token: 0x040000F0 RID: 240
		private InputKeyItemVM _doneInputKey;

		// Token: 0x040000F1 RID: 241
		private MBBindingList<QuestItemVM> _activeQuestsList;

		// Token: 0x040000F2 RID: 242
		private MBBindingList<QuestItemVM> _oldQuestsList;

		// Token: 0x040000F3 RID: 243
		private QuestItemVM _selectedQuest;

		// Token: 0x040000F4 RID: 244
		private HeroVM _currentQuestGiverHero;

		// Token: 0x040000F5 RID: 245
		private string _activeQuestsText;

		// Token: 0x040000F6 RID: 246
		private string _oldQuestsText;

		// Token: 0x040000F7 RID: 247
		private string _timeRemainingLbl;

		// Token: 0x040000F8 RID: 248
		private string _currentQuestTitle;

		// Token: 0x040000F9 RID: 249
		private bool _isCurrentQuestGiverHeroHidden;

		// Token: 0x040000FA RID: 250
		private string _questGiverText;

		// Token: 0x040000FB RID: 251
		private string _questTitleText;

		// Token: 0x040000FC RID: 252
		private string _doneLbl;

		// Token: 0x040000FD RID: 253
		private string _noActiveQuestText;

		// Token: 0x040000FE RID: 254
		private string _sortQuestsText;

		// Token: 0x040000FF RID: 255
		private bool _isThereAnyQuest;

		// Token: 0x04000100 RID: 256
		private MBBindingList<QuestStageVM> _currentQuestStages;

		// Token: 0x04000101 RID: 257
		private HintViewModel _timeRemainingHint;

		// Token: 0x04000102 RID: 258
		private HintViewModel _oldQuestsHint;

		// Token: 0x04000103 RID: 259
		private QuestItemSortControllerVM _activeQuestsSortController;

		// Token: 0x04000104 RID: 260
		private QuestItemSortControllerVM _oldQuestsSortController;

		// Token: 0x04000105 RID: 261
		private SelectorVM<SelectorItemVM> _sortSelector;

		// Token: 0x0200015D RID: 349
		public enum QuestCompletionType
		{
			// Token: 0x04000EB6 RID: 3766
			Active,
			// Token: 0x04000EB7 RID: 3767
			Successful,
			// Token: 0x04000EB8 RID: 3768
			UnSuccessful
		}
	}
}
