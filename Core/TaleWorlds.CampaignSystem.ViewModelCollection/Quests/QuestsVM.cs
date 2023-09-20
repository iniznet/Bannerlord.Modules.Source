using System;
using System.Collections.Generic;
using System.Linq;
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
	public class QuestsVM : ViewModel
	{
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
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.QuestGiverText = GameTexts.FindText("str_quest_given_by", null).ToString();
			this.TimeRemainingLbl = GameTexts.FindText("str_time_remaining", null).ToString();
			this.QuestTitleText = GameTexts.FindText("str_quests", null).ToString();
			this.NoActiveQuestText = GameTexts.FindText("str_no_active_quest", null).ToString();
			this.SortQuestsText = GameTexts.FindText("str_sort_quests", null).ToString();
			this.OldQuestsHint = new HintViewModel(GameTexts.FindText("str_old_quests_explanation", null), null);
			this.DoneLbl = GameTexts.FindText("str_done", null).ToString();
			GameTexts.SetVariable("RANK", GameTexts.FindText("str_active_quests", null));
			GameTexts.SetVariable("NUMBER", this.ActiveQuestsList.Count);
			this.ActiveQuestsText = GameTexts.FindText("str_RANK_with_NUM_between_parenthesis", null).ToString();
			GameTexts.SetVariable("RANK", GameTexts.FindText("str_old_quests", null));
			GameTexts.SetVariable("NUMBER", this.OldQuestsList.Count);
			this.OldQuestsText = GameTexts.FindText("str_RANK_with_NUM_between_parenthesis", null).ToString();
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
				this._viewDataTracker.OnQuestLogExamined(questStageVM2.Log);
				questStageVM2.UpdateIsNew();
				this._selectedQuest.UpdateIsUpdated();
			}
		}

		public void ExecuteOpenQuestGiverEncyclopedia()
		{
			HeroVM currentQuestGiverHero = this.CurrentQuestGiverHero;
			if (currentQuestGiverHero == null)
			{
				return;
			}
			currentQuestGiverHero.ExecuteLink();
		}

		public void ExecuteClose()
		{
			this._closeQuestsScreen();
		}

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

		private void OnSortOptionChanged(SelectorVM<SelectorItemVM> sortSelector)
		{
			this._viewDataTracker.SetQuestSortTypeSelection(sortSelector.SelectedIndex);
			this.ActiveQuestsSortController.SortByOption((QuestItemSortControllerVM.QuestItemSortOption)sortSelector.SelectedIndex);
			this.OldQuestsSortController.SortByOption((QuestItemSortControllerVM.QuestItemSortOption)sortSelector.SelectedIndex);
		}

		public void SetDoneInputKey(HotKey hotKey)
		{
			this.DoneInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
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

		private readonly Action _closeQuestsScreen;

		private readonly IViewDataTracker _viewDataTracker;

		private InputKeyItemVM _doneInputKey;

		private MBBindingList<QuestItemVM> _activeQuestsList;

		private MBBindingList<QuestItemVM> _oldQuestsList;

		private QuestItemVM _selectedQuest;

		private HeroVM _currentQuestGiverHero;

		private string _activeQuestsText;

		private string _oldQuestsText;

		private string _timeRemainingLbl;

		private string _currentQuestTitle;

		private bool _isCurrentQuestGiverHeroHidden;

		private string _questGiverText;

		private string _questTitleText;

		private string _doneLbl;

		private string _noActiveQuestText;

		private string _sortQuestsText;

		private bool _isThereAnyQuest;

		private MBBindingList<QuestStageVM> _currentQuestStages;

		private HintViewModel _timeRemainingHint;

		private HintViewModel _oldQuestsHint;

		private QuestItemSortControllerVM _activeQuestsSortController;

		private QuestItemSortControllerVM _oldQuestsSortController;

		private SelectorVM<SelectorItemVM> _sortSelector;

		public enum QuestCompletionType
		{
			Active,
			Successful,
			UnSuccessful
		}
	}
}
