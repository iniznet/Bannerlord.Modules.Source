using System;
using System.Linq;
using TaleWorlds.CampaignSystem.Issues;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Quests
{
	// Token: 0x0200001E RID: 30
	public class QuestItemVM : ViewModel
	{
		// Token: 0x1700004C RID: 76
		// (get) Token: 0x060001B0 RID: 432 RVA: 0x0000F878 File Offset: 0x0000DA78
		public QuestBase Quest { get; }

		// Token: 0x1700004D RID: 77
		// (get) Token: 0x060001B1 RID: 433 RVA: 0x0000F880 File Offset: 0x0000DA80
		public IssueBase Issue { get; }

		// Token: 0x1700004E RID: 78
		// (get) Token: 0x060001B2 RID: 434 RVA: 0x0000F888 File Offset: 0x0000DA88
		public JournalLogEntry QuestLogEntry { get; }

		// Token: 0x060001B3 RID: 435 RVA: 0x0000F890 File Offset: 0x0000DA90
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

		// Token: 0x060001B4 RID: 436 RVA: 0x0000F9D8 File Offset: 0x0000DBD8
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

		// Token: 0x060001B5 RID: 437 RVA: 0x0000FAD8 File Offset: 0x0000DCD8
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

		// Token: 0x060001B6 RID: 438 RVA: 0x0000FBB4 File Offset: 0x0000DDB4
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

		// Token: 0x060001B7 RID: 439 RVA: 0x0000FC84 File Offset: 0x0000DE84
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

		// Token: 0x060001B8 RID: 440 RVA: 0x0000FD5C File Offset: 0x0000DF5C
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

		// Token: 0x060001B9 RID: 441 RVA: 0x0000FE47 File Offset: 0x0000E047
		public void UpdateIsUpdated()
		{
			this.IsUpdated = this.Stages.Any((QuestStageVM s) => s.IsNew);
		}

		// Token: 0x060001BA RID: 442 RVA: 0x0000FE79 File Offset: 0x0000E079
		public void ExecuteSelection()
		{
			this._onSelection(this);
		}

		// Token: 0x060001BB RID: 443 RVA: 0x0000FE87 File Offset: 0x0000E087
		public void ExecuteToggleQuestTrack()
		{
			if (this.Quest != null)
			{
				this.Quest.ToggleTrackedObjects();
				this.IsTracked = this.Quest.IsTrackEnabled;
			}
		}

		// Token: 0x1700004F RID: 79
		// (get) Token: 0x060001BC RID: 444 RVA: 0x0000FEAD File Offset: 0x0000E0AD
		// (set) Token: 0x060001BD RID: 445 RVA: 0x0000FEB5 File Offset: 0x0000E0B5
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

		// Token: 0x17000050 RID: 80
		// (get) Token: 0x060001BE RID: 446 RVA: 0x0000FED8 File Offset: 0x0000E0D8
		// (set) Token: 0x060001BF RID: 447 RVA: 0x0000FEE0 File Offset: 0x0000E0E0
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

		// Token: 0x17000051 RID: 81
		// (get) Token: 0x060001C0 RID: 448 RVA: 0x0000FEFE File Offset: 0x0000E0FE
		// (set) Token: 0x060001C1 RID: 449 RVA: 0x0000FF06 File Offset: 0x0000E106
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

		// Token: 0x17000052 RID: 82
		// (get) Token: 0x060001C2 RID: 450 RVA: 0x0000FF24 File Offset: 0x0000E124
		// (set) Token: 0x060001C3 RID: 451 RVA: 0x0000FF2C File Offset: 0x0000E12C
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

		// Token: 0x17000053 RID: 83
		// (get) Token: 0x060001C4 RID: 452 RVA: 0x0000FF4A File Offset: 0x0000E14A
		// (set) Token: 0x060001C5 RID: 453 RVA: 0x0000FF52 File Offset: 0x0000E152
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

		// Token: 0x17000054 RID: 84
		// (get) Token: 0x060001C6 RID: 454 RVA: 0x0000FF70 File Offset: 0x0000E170
		// (set) Token: 0x060001C7 RID: 455 RVA: 0x0000FF78 File Offset: 0x0000E178
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

		// Token: 0x17000055 RID: 85
		// (get) Token: 0x060001C8 RID: 456 RVA: 0x0000FF96 File Offset: 0x0000E196
		// (set) Token: 0x060001C9 RID: 457 RVA: 0x0000FF9E File Offset: 0x0000E19E
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

		// Token: 0x17000056 RID: 86
		// (get) Token: 0x060001CA RID: 458 RVA: 0x0000FFBC File Offset: 0x0000E1BC
		// (set) Token: 0x060001CB RID: 459 RVA: 0x0000FFC4 File Offset: 0x0000E1C4
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

		// Token: 0x17000057 RID: 87
		// (get) Token: 0x060001CC RID: 460 RVA: 0x0000FFE2 File Offset: 0x0000E1E2
		// (set) Token: 0x060001CD RID: 461 RVA: 0x0000FFEA File Offset: 0x0000E1EA
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

		// Token: 0x17000058 RID: 88
		// (get) Token: 0x060001CE RID: 462 RVA: 0x00010008 File Offset: 0x0000E208
		// (set) Token: 0x060001CF RID: 463 RVA: 0x00010010 File Offset: 0x0000E210
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

		// Token: 0x17000059 RID: 89
		// (get) Token: 0x060001D0 RID: 464 RVA: 0x0001002E File Offset: 0x0000E22E
		// (set) Token: 0x060001D1 RID: 465 RVA: 0x00010036 File Offset: 0x0000E236
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

		// Token: 0x1700005A RID: 90
		// (get) Token: 0x060001D2 RID: 466 RVA: 0x00010059 File Offset: 0x0000E259
		// (set) Token: 0x060001D3 RID: 467 RVA: 0x00010061 File Offset: 0x0000E261
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

		// Token: 0x1700005B RID: 91
		// (get) Token: 0x060001D4 RID: 468 RVA: 0x00010084 File Offset: 0x0000E284
		// (set) Token: 0x060001D5 RID: 469 RVA: 0x0001008C File Offset: 0x0000E28C
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

		// Token: 0x1700005C RID: 92
		// (get) Token: 0x060001D6 RID: 470 RVA: 0x000100AA File Offset: 0x0000E2AA
		// (set) Token: 0x060001D7 RID: 471 RVA: 0x000100B2 File Offset: 0x0000E2B2
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

		// Token: 0x1700005D RID: 93
		// (get) Token: 0x060001D8 RID: 472 RVA: 0x000100D0 File Offset: 0x0000E2D0
		// (set) Token: 0x060001D9 RID: 473 RVA: 0x000100D8 File Offset: 0x0000E2D8
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

		// Token: 0x1700005E RID: 94
		// (get) Token: 0x060001DA RID: 474 RVA: 0x000100F6 File Offset: 0x0000E2F6
		// (set) Token: 0x060001DB RID: 475 RVA: 0x000100FE File Offset: 0x0000E2FE
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

		// Token: 0x040000C7 RID: 199
		private readonly Action<QuestItemVM> _onSelection;

		// Token: 0x040000C8 RID: 200
		private QuestsVM.QuestCompletionType _completionType;

		// Token: 0x040000C9 RID: 201
		private string _name;

		// Token: 0x040000CA RID: 202
		private string _remainingDaysText;

		// Token: 0x040000CB RID: 203
		private string _remainingDaysTextCombined;

		// Token: 0x040000CC RID: 204
		private int _remainingDays;

		// Token: 0x040000CD RID: 205
		private int _completionTypeAsInt;

		// Token: 0x040000CE RID: 206
		private bool _isRemainingDaysHidden;

		// Token: 0x040000CF RID: 207
		private bool _isUpdated;

		// Token: 0x040000D0 RID: 208
		private bool _isSelected;

		// Token: 0x040000D1 RID: 209
		private bool _isCompleted;

		// Token: 0x040000D2 RID: 210
		private bool _isCompletedSuccessfully;

		// Token: 0x040000D3 RID: 211
		private bool _isTracked;

		// Token: 0x040000D4 RID: 212
		private bool _isTrackable;

		// Token: 0x040000D5 RID: 213
		private bool _isMainQuest;

		// Token: 0x040000D6 RID: 214
		private HeroVM _questGiverHero;

		// Token: 0x040000D7 RID: 215
		private bool _isQuestGiverHeroHidden;

		// Token: 0x040000D8 RID: 216
		private MBBindingList<QuestStageVM> _stages;
	}
}
