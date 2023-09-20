using System;
using System.Linq;
using TaleWorlds.CampaignSystem.Issues;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Quests
{
	public class QuestItemVM : ViewModel
	{
		public QuestBase Quest { get; }

		public IssueBase Issue { get; }

		public JournalLogEntry QuestLogEntry { get; }

		public QuestItemVM(JournalLogEntry questLogEntry, Action<QuestItemVM> onSelection, QuestsVM.QuestCompletionType completion)
		{
			this._onSelection = onSelection;
			this.QuestLogEntry = questLogEntry;
			this.Stages = new MBBindingList<QuestStageVM>();
			this._completionType = completion;
			this.IsCompleted = this._completionType > QuestsVM.QuestCompletionType.Active;
			this.IsCompletedSuccessfully = this._completionType == QuestsVM.QuestCompletionType.Successful;
			this.CompletionTypeAsInt = (int)this._completionType;
			bool flag;
			if (!this.IsCompleted)
			{
				QuestBase quest = this.Quest;
				CampaignTime? campaignTime = ((quest != null) ? new CampaignTime?(quest.QuestDueTime) : null);
				CampaignTime never = CampaignTime.Never;
				flag = campaignTime != null && (campaignTime == null || campaignTime.GetValueOrDefault() == never);
			}
			else
			{
				flag = true;
			}
			this.IsRemainingDaysHidden = flag;
			this.IsQuestGiverHeroHidden = false;
			this.IsMainQuest = questLogEntry.IsSpecial;
			foreach (JournalLog journalLog in questLogEntry.GetEntries())
			{
				this.PopulateQuestLog(journalLog, false);
			}
			this.Name = questLogEntry.Title.ToString();
			this.QuestGiverHero = new HeroVM(questLogEntry.RelatedHero, false);
			this.IsTracked = false;
			this.IsTrackable = false;
			this.RefreshValues();
		}

		public QuestItemVM(QuestBase quest, Action<QuestItemVM> onSelection)
		{
			this.Quest = quest;
			this._onSelection = onSelection;
			this.Stages = new MBBindingList<QuestStageVM>();
			this.CompletionTypeAsInt = 0;
			this.IsRemainingDaysHidden = !this.Quest.IsOngoing || this.Quest.IsRemainingTimeHidden;
			this.IsQuestGiverHeroHidden = this.Quest.QuestGiver == null;
			MBReadOnlyList<JournalLog> journalEntries = this.Quest.JournalEntries;
			for (int i = 0; i < journalEntries.Count; i++)
			{
				bool flag = i == journalEntries.Count - 1;
				JournalLog journalLog = journalEntries[i];
				this.PopulateQuestLog(journalLog, flag);
			}
			this.IsMainQuest = quest.IsSpecialQuest;
			if (!this.IsQuestGiverHeroHidden)
			{
				this.QuestGiverHero = new HeroVM(this.Quest.QuestGiver, false);
			}
			this.UpdateIsUpdated();
			this.IsTrackable = !this.Quest.IsFinalized;
			this.IsTracked = this.Quest.IsTrackEnabled;
			this.RefreshValues();
		}

		public QuestItemVM(IssueBase issue, Action<QuestItemVM> onSelection)
		{
			this.Issue = issue;
			this._onSelection = onSelection;
			this.Stages = new MBBindingList<QuestStageVM>();
			this.IsCompleted = false;
			this.CompletionTypeAsInt = 0;
			this.IsRemainingDaysHidden = this.Issue.IsOngoingWithoutQuest;
			this.IsQuestGiverHeroHidden = false;
			this.UpdateRemainingTime(this.Issue.IssueDueTime);
			foreach (JournalLog journalLog in issue.JournalEntries)
			{
				this.PopulateQuestLog(journalLog, false);
			}
			this.Name = issue.Title.ToString();
			this.QuestGiverHero = new HeroVM(issue.IssueOwner, false);
			this.UpdateIsUpdated();
			this.IsTrackable = false;
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			if (this.Quest != null)
			{
				this.Name = this.Quest.Title.ToString();
				this.UpdateRemainingTime(this.Quest.QuestDueTime);
			}
			else if (this.Issue != null)
			{
				this.Name = this.Issue.Title.ToString();
				this.UpdateRemainingTime(this.Issue.IssueDueTime);
			}
			else if (this.QuestLogEntry != null)
			{
				this.Name = this.QuestLogEntry.Title.ToString();
			}
			HeroVM questGiverHero = this.QuestGiverHero;
			if (questGiverHero != null)
			{
				questGiverHero.RefreshValues();
			}
			this.Stages.ApplyActionOnAllItems(delegate(QuestStageVM x)
			{
				x.RefreshValues();
			});
		}

		private void UpdateRemainingTime(CampaignTime dueTime)
		{
			if (this.IsRemainingDaysHidden)
			{
				this.RemainingDays = 0;
			}
			else
			{
				this.RemainingDays = (int)(dueTime - CampaignTime.Now).ToDays;
			}
			GameTexts.SetVariable("DAY_IS_PLURAL", (this.RemainingDays > 1) ? 1 : 0);
			GameTexts.SetVariable("DAY", this.RemainingDays);
			if (dueTime.ToHours - CampaignTime.Now.ToHours < 24.0)
			{
				this.RemainingDaysText = GameTexts.FindText("str_less_than_a_day", null).ToString();
				this.RemainingDaysTextCombined = GameTexts.FindText("str_less_than_a_day", null).ToString();
				return;
			}
			this.RemainingDaysText = GameTexts.FindText("str_DAY_days_capital", null).ToString();
			this.RemainingDaysTextCombined = GameTexts.FindText("str_DAY_days", null).ToString();
		}

		private void PopulateQuestLog(JournalLog log, bool isLastStage)
		{
			string text = log.GetTimeText().ToString();
			if (log.Type != LogType.Text && log.Type != LogType.None)
			{
				int num = MathF.Max(log.Range, 0);
				int num2 = ((log.Type == LogType.TwoWayContinuous) ? log.CurrentProgress : MathF.Max(log.CurrentProgress, 0));
				TextObject textObject = new TextObject("{=Pdo7PpS3}{TASK_NAME} {CURRENT_PROGRESS}/{TARGET_PROGRESS}", null);
				textObject.SetTextVariable("TASK_NAME", log.TaskName);
				textObject.SetTextVariable("CURRENT_PROGRESS", num2);
				textObject.SetTextVariable("TARGET_PROGRESS", num);
				QuestStageTaskVM questStageTaskVM = new QuestStageTaskVM(textObject, num2, num, log.Type);
				this.Stages.Add(new QuestStageVM(log, text, isLastStage, new Action(this.UpdateIsUpdated), questStageTaskVM));
				return;
			}
			this.Stages.Add(new QuestStageVM(log, log.LogText.ToString(), text, isLastStage, new Action(this.UpdateIsUpdated)));
		}

		public void UpdateIsUpdated()
		{
			this.IsUpdated = this.Stages.Any((QuestStageVM s) => s.IsNew);
		}

		public void ExecuteSelection()
		{
			this._onSelection(this);
		}

		public void ExecuteToggleQuestTrack()
		{
			if (this.Quest != null)
			{
				this.Quest.ToggleTrackedObjects();
				this.IsTracked = this.Quest.IsTrackEnabled;
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
		public int CompletionTypeAsInt
		{
			get
			{
				return this._completionTypeAsInt;
			}
			set
			{
				if (value != this._completionTypeAsInt)
				{
					this._completionTypeAsInt = value;
					base.OnPropertyChangedWithValue(value, "CompletionTypeAsInt");
				}
			}
		}

		[DataSourceProperty]
		public bool IsMainQuest
		{
			get
			{
				return this._isMainQuest;
			}
			set
			{
				if (value != this._isMainQuest)
				{
					this._isMainQuest = value;
					base.OnPropertyChangedWithValue(value, "IsMainQuest");
				}
			}
		}

		[DataSourceProperty]
		public bool IsCompletedSuccessfully
		{
			get
			{
				return this._isCompletedSuccessfully;
			}
			set
			{
				if (value != this._isCompletedSuccessfully)
				{
					this._isCompletedSuccessfully = value;
					base.OnPropertyChangedWithValue(value, "IsCompletedSuccessfully");
				}
			}
		}

		[DataSourceProperty]
		public bool IsCompleted
		{
			get
			{
				return this._isCompleted;
			}
			set
			{
				if (value != this._isCompleted)
				{
					this._isCompleted = value;
					base.OnPropertyChangedWithValue(value, "IsCompleted");
				}
			}
		}

		[DataSourceProperty]
		public bool IsUpdated
		{
			get
			{
				return this._isUpdated;
			}
			set
			{
				if (value != this._isUpdated)
				{
					this._isUpdated = value;
					base.OnPropertyChangedWithValue(value, "IsUpdated");
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

		[DataSourceProperty]
		public bool IsRemainingDaysHidden
		{
			get
			{
				return this._isRemainingDaysHidden;
			}
			set
			{
				if (value != this._isRemainingDaysHidden)
				{
					this._isRemainingDaysHidden = value;
					base.OnPropertyChangedWithValue(value, "IsRemainingDaysHidden");
				}
			}
		}

		[DataSourceProperty]
		public bool IsTracked
		{
			get
			{
				return this._isTracked;
			}
			set
			{
				if (value != this._isTracked)
				{
					this._isTracked = value;
					base.OnPropertyChangedWithValue(value, "IsTracked");
				}
			}
		}

		[DataSourceProperty]
		public bool IsTrackable
		{
			get
			{
				return this._isTrackable;
			}
			set
			{
				if (value != this._isTrackable)
				{
					this._isTrackable = value;
					base.OnPropertyChangedWithValue(value, "IsTrackable");
				}
			}
		}

		[DataSourceProperty]
		public string RemainingDaysText
		{
			get
			{
				return this._remainingDaysText;
			}
			set
			{
				if (value != this._remainingDaysText)
				{
					this._remainingDaysText = value;
					base.OnPropertyChangedWithValue<string>(value, "RemainingDaysText");
				}
			}
		}

		[DataSourceProperty]
		public string RemainingDaysTextCombined
		{
			get
			{
				return this._remainingDaysTextCombined;
			}
			set
			{
				if (value != this._remainingDaysTextCombined)
				{
					this._remainingDaysTextCombined = value;
					base.OnPropertyChangedWithValue<string>(value, "RemainingDaysTextCombined");
				}
			}
		}

		[DataSourceProperty]
		public int RemainingDays
		{
			get
			{
				return this._remainingDays;
			}
			set
			{
				if (value != this._remainingDays)
				{
					this._remainingDays = value;
					base.OnPropertyChangedWithValue(value, "RemainingDays");
				}
			}
		}

		[DataSourceProperty]
		public HeroVM QuestGiverHero
		{
			get
			{
				return this._questGiverHero;
			}
			set
			{
				if (value != this._questGiverHero)
				{
					this._questGiverHero = value;
					base.OnPropertyChangedWithValue<HeroVM>(value, "QuestGiverHero");
				}
			}
		}

		[DataSourceProperty]
		public bool IsQuestGiverHeroHidden
		{
			get
			{
				return this._isQuestGiverHeroHidden;
			}
			set
			{
				if (value != this._isQuestGiverHeroHidden)
				{
					this._isQuestGiverHeroHidden = value;
					base.OnPropertyChangedWithValue(value, "IsQuestGiverHeroHidden");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<QuestStageVM> Stages
		{
			get
			{
				return this._stages;
			}
			set
			{
				if (value != this._stages)
				{
					this._stages = value;
					base.OnPropertyChangedWithValue<MBBindingList<QuestStageVM>>(value, "Stages");
				}
			}
		}

		private readonly Action<QuestItemVM> _onSelection;

		private QuestsVM.QuestCompletionType _completionType;

		private string _name;

		private string _remainingDaysText;

		private string _remainingDaysTextCombined;

		private int _remainingDays;

		private int _completionTypeAsInt;

		private bool _isRemainingDaysHidden;

		private bool _isUpdated;

		private bool _isSelected;

		private bool _isCompleted;

		private bool _isCompletedSuccessfully;

		private bool _isTracked;

		private bool _isTrackable;

		private bool _isMainQuest;

		private HeroVM _questGiverHero;

		private bool _isQuestGiverHeroHidden;

		private MBBindingList<QuestStageVM> _stages;
	}
}
