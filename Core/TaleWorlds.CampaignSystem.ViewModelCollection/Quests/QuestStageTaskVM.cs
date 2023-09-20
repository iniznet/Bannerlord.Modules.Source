using System;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Quests
{
	public class QuestStageTaskVM : ViewModel
	{
		public QuestStageTaskVM(TextObject taskName, int currentProgress, int targetProgress, LogType type)
		{
			this._taskNameObj = taskName;
			this.CurrentProgress = currentProgress;
			this.TargetProgress = targetProgress;
			base.OnPropertyChanged("NegativeTargetProgress");
			this.ProgressType = (int)type;
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.TaskName = this._taskNameObj.ToString();
		}

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

		[DataSourceProperty]
		public int NegativeTargetProgress
		{
			get
			{
				return this._targetProgress * -1;
			}
		}

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

		private readonly TextObject _taskNameObj;

		private string _taskName;

		private int _currentProgress;

		private int _targetProgress;

		private int _progressType;

		private bool _isValid;
	}
}
