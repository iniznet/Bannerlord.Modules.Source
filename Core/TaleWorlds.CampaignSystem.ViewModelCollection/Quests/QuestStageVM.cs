using System;
using System.Linq;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Quests
{
	// Token: 0x02000021 RID: 33
	public class QuestStageVM : ViewModel
	{
		// Token: 0x060001F4 RID: 500 RVA: 0x00010360 File Offset: 0x0000E560
		public QuestStageVM(JournalLog log, string dateString, bool isLastStage, Action onLogNotified, QuestStageTaskVM stageTask = null)
		{
			this.StageTask = new QuestStageTaskVM(TextObject.Empty, 0, 0, LogType.None);
			this._onLogNotified = onLogNotified;
			string text = log.LogText.ToString();
			GameTexts.SetVariable("ENTRY", text);
			this.DateText = dateString;
			this.DescriptionText = log.LogText.ToString();
			this.IsLastStage = isLastStage;
			this.Log = log;
			this.UpdateIsNew();
			if (stageTask != null)
			{
				this.StageTask = stageTask;
				this.StageTask.IsValid = true;
				this.HasATask = true;
				this.IsTaskCompleted = this.StageTask.CurrentProgress == this.StageTask.TargetProgress;
			}
		}

		// Token: 0x060001F5 RID: 501 RVA: 0x00010410 File Offset: 0x0000E610
		public QuestStageVM(JournalLog log, string description, string dateString, bool isLastStage, Action onLogNotified)
		{
			this.Log = log;
			this.StageTask = new QuestStageTaskVM(TextObject.Empty, 0, 0, LogType.None);
			this._onLogNotified = onLogNotified;
			this.DateText = dateString;
			this.DescriptionText = description;
			this.IsLastStage = isLastStage;
			this.UpdateIsNew();
		}

		// Token: 0x060001F6 RID: 502 RVA: 0x00010461 File Offset: 0x0000E661
		public void ExecuteResetUpdated()
		{
		}

		// Token: 0x060001F7 RID: 503 RVA: 0x00010463 File Offset: 0x0000E663
		public void ExecuteLink(string link)
		{
			Campaign.Current.EncyclopediaManager.GoToLink(link);
		}

		// Token: 0x060001F8 RID: 504 RVA: 0x00010475 File Offset: 0x0000E675
		public void UpdateIsNew()
		{
			if (this.Log != null)
			{
				this.IsNew = PlayerUpdateTracker.Current.UnExaminedQuestLogs.Any((JournalLog l) => l == this.Log);
			}
		}

		// Token: 0x1700006B RID: 107
		// (get) Token: 0x060001F9 RID: 505 RVA: 0x000104A0 File Offset: 0x0000E6A0
		// (set) Token: 0x060001FA RID: 506 RVA: 0x000104A8 File Offset: 0x0000E6A8
		[DataSourceProperty]
		public string DateText
		{
			get
			{
				return this._dateText;
			}
			set
			{
				if (value != this._dateText)
				{
					this._dateText = value;
					base.OnPropertyChangedWithValue<string>(value, "DateText");
				}
			}
		}

		// Token: 0x1700006C RID: 108
		// (get) Token: 0x060001FB RID: 507 RVA: 0x000104CB File Offset: 0x0000E6CB
		// (set) Token: 0x060001FC RID: 508 RVA: 0x000104D3 File Offset: 0x0000E6D3
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

		// Token: 0x1700006D RID: 109
		// (get) Token: 0x060001FD RID: 509 RVA: 0x000104F6 File Offset: 0x0000E6F6
		// (set) Token: 0x060001FE RID: 510 RVA: 0x000104FE File Offset: 0x0000E6FE
		[DataSourceProperty]
		public bool HasATask
		{
			get
			{
				return this._hasATask;
			}
			set
			{
				if (value != this._hasATask)
				{
					this._hasATask = value;
					base.OnPropertyChangedWithValue(value, "HasATask");
				}
			}
		}

		// Token: 0x1700006E RID: 110
		// (get) Token: 0x060001FF RID: 511 RVA: 0x0001051C File Offset: 0x0000E71C
		// (set) Token: 0x06000200 RID: 512 RVA: 0x00010524 File Offset: 0x0000E724
		[DataSourceProperty]
		public bool IsNew
		{
			get
			{
				return this._isNew;
			}
			set
			{
				if (value != this._isNew)
				{
					this._isNew = value;
					base.OnPropertyChangedWithValue(value, "IsNew");
				}
			}
		}

		// Token: 0x1700006F RID: 111
		// (get) Token: 0x06000201 RID: 513 RVA: 0x00010542 File Offset: 0x0000E742
		// (set) Token: 0x06000202 RID: 514 RVA: 0x0001054A File Offset: 0x0000E74A
		[DataSourceProperty]
		public bool IsLastStage
		{
			get
			{
				return this._isLastStage;
			}
			set
			{
				if (value != this._isLastStage)
				{
					this._isLastStage = value;
					base.OnPropertyChangedWithValue(value, "IsLastStage");
				}
			}
		}

		// Token: 0x17000070 RID: 112
		// (get) Token: 0x06000203 RID: 515 RVA: 0x00010568 File Offset: 0x0000E768
		// (set) Token: 0x06000204 RID: 516 RVA: 0x00010570 File Offset: 0x0000E770
		[DataSourceProperty]
		public bool IsTaskCompleted
		{
			get
			{
				return this._isTaskCompleted;
			}
			set
			{
				if (value != this._isTaskCompleted)
				{
					this._isTaskCompleted = value;
					base.OnPropertyChangedWithValue(value, "IsTaskCompleted");
				}
			}
		}

		// Token: 0x17000071 RID: 113
		// (get) Token: 0x06000205 RID: 517 RVA: 0x0001058E File Offset: 0x0000E78E
		// (set) Token: 0x06000206 RID: 518 RVA: 0x00010596 File Offset: 0x0000E796
		[DataSourceProperty]
		public QuestStageTaskVM StageTask
		{
			get
			{
				return this._stageTask;
			}
			set
			{
				if (value != this._stageTask)
				{
					this._stageTask = value;
					base.OnPropertyChangedWithValue<QuestStageTaskVM>(value, "StageTask");
				}
			}
		}

		// Token: 0x040000E5 RID: 229
		public readonly JournalLog Log;

		// Token: 0x040000E6 RID: 230
		private readonly Action _onLogNotified;

		// Token: 0x040000E7 RID: 231
		private string _descriptionText;

		// Token: 0x040000E8 RID: 232
		private string _dateText;

		// Token: 0x040000E9 RID: 233
		private bool _hasATask;

		// Token: 0x040000EA RID: 234
		private bool _isNew;

		// Token: 0x040000EB RID: 235
		private bool _isTaskCompleted;

		// Token: 0x040000EC RID: 236
		private bool _isLastStage;

		// Token: 0x040000ED RID: 237
		private QuestStageTaskVM _stageTask;
	}
}
