using System;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Quests
{
	// Token: 0x02000020 RID: 32
	public class QuestStageTaskVM : ViewModel
	{
		// Token: 0x060001E7 RID: 487 RVA: 0x00010243 File Offset: 0x0000E443
		public QuestStageTaskVM(TextObject taskName, int currentProgress, int targetProgress, LogType type)
		{
			this._taskNameObj = taskName;
			this.CurrentProgress = currentProgress;
			this.TargetProgress = targetProgress;
			base.OnPropertyChanged("NegativeTargetProgress");
			this.ProgressType = (int)type;
			this.RefreshValues();
		}

		// Token: 0x060001E8 RID: 488 RVA: 0x00010279 File Offset: 0x0000E479
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.TaskName = this._taskNameObj.ToString();
		}

		// Token: 0x17000065 RID: 101
		// (get) Token: 0x060001E9 RID: 489 RVA: 0x00010292 File Offset: 0x0000E492
		// (set) Token: 0x060001EA RID: 490 RVA: 0x0001029A File Offset: 0x0000E49A
		[DataSourceProperty]
		public string TaskName
		{
			get
			{
				return this._taskName;
			}
			set
			{
				if (value != this._taskName)
				{
					this._taskName = value;
					base.OnPropertyChangedWithValue<string>(value, "TaskName");
				}
			}
		}

		// Token: 0x17000066 RID: 102
		// (get) Token: 0x060001EB RID: 491 RVA: 0x000102BD File Offset: 0x0000E4BD
		// (set) Token: 0x060001EC RID: 492 RVA: 0x000102C5 File Offset: 0x0000E4C5
		[DataSourceProperty]
		public bool IsValid
		{
			get
			{
				return this._isValid;
			}
			set
			{
				if (value != this._isValid)
				{
					this._isValid = value;
					base.OnPropertyChangedWithValue(value, "IsValid");
				}
			}
		}

		// Token: 0x17000067 RID: 103
		// (get) Token: 0x060001ED RID: 493 RVA: 0x000102E3 File Offset: 0x0000E4E3
		// (set) Token: 0x060001EE RID: 494 RVA: 0x000102EB File Offset: 0x0000E4EB
		[DataSourceProperty]
		public int CurrentProgress
		{
			get
			{
				return this._currentProgress;
			}
			set
			{
				if (value != this._currentProgress)
				{
					this._currentProgress = value;
					base.OnPropertyChangedWithValue(value, "CurrentProgress");
				}
			}
		}

		// Token: 0x17000068 RID: 104
		// (get) Token: 0x060001EF RID: 495 RVA: 0x00010309 File Offset: 0x0000E509
		// (set) Token: 0x060001F0 RID: 496 RVA: 0x00010311 File Offset: 0x0000E511
		[DataSourceProperty]
		public int TargetProgress
		{
			get
			{
				return this._targetProgress;
			}
			set
			{
				if (value != this._targetProgress)
				{
					this._targetProgress = value;
					base.OnPropertyChangedWithValue(value, "TargetProgress");
				}
			}
		}

		// Token: 0x17000069 RID: 105
		// (get) Token: 0x060001F1 RID: 497 RVA: 0x0001032F File Offset: 0x0000E52F
		[DataSourceProperty]
		public int NegativeTargetProgress
		{
			get
			{
				return this._targetProgress * -1;
			}
		}

		// Token: 0x1700006A RID: 106
		// (get) Token: 0x060001F2 RID: 498 RVA: 0x00010339 File Offset: 0x0000E539
		// (set) Token: 0x060001F3 RID: 499 RVA: 0x00010341 File Offset: 0x0000E541
		[DataSourceProperty]
		public int ProgressType
		{
			get
			{
				return this._progressType;
			}
			set
			{
				if (value != this._progressType)
				{
					this._progressType = value;
					base.OnPropertyChangedWithValue(value, "ProgressType");
				}
			}
		}

		// Token: 0x040000DF RID: 223
		private readonly TextObject _taskNameObj;

		// Token: 0x040000E0 RID: 224
		private string _taskName;

		// Token: 0x040000E1 RID: 225
		private int _currentProgress;

		// Token: 0x040000E2 RID: 226
		private int _targetProgress;

		// Token: 0x040000E3 RID: 227
		private int _progressType;

		// Token: 0x040000E4 RID: 228
		private bool _isValid;
	}
}
