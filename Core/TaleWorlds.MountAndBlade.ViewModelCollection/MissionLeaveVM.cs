using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection
{
	public class MissionLeaveVM : ViewModel
	{
		public MissionLeaveVM(Func<float> getMissionEndTimer, Func<float> getMissionEndTimeInSeconds)
		{
			this._getMissionEndTimer = getMissionEndTimer;
			this._getMissionEndTimeInSeconds = getMissionEndTimeInSeconds;
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.LeaveText = GameTexts.FindText("str_leaving", null).ToString();
		}

		public void Tick(float dt)
		{
			this.CurrentTime = this._getMissionEndTimer();
			this.MaxTime = this._getMissionEndTimeInSeconds();
		}

		[DataSourceProperty]
		public string LeaveText
		{
			get
			{
				return this._leaveText;
			}
			set
			{
				if (value != this._leaveText)
				{
					this._leaveText = value;
					base.OnPropertyChangedWithValue<string>(value, "LeaveText");
				}
			}
		}

		[DataSourceProperty]
		public float MaxTime
		{
			get
			{
				return this._maxTime;
			}
			set
			{
				if (value != this._maxTime)
				{
					this._maxTime = value;
					base.OnPropertyChangedWithValue(value, "MaxTime");
				}
			}
		}

		[DataSourceProperty]
		public float CurrentTime
		{
			get
			{
				return this._currentTime;
			}
			set
			{
				if (value != this._currentTime)
				{
					this._currentTime = value;
					base.OnPropertyChangedWithValue(value, "CurrentTime");
				}
			}
		}

		private Func<float> _getMissionEndTimer;

		private Func<float> _getMissionEndTimeInSeconds;

		private float _maxTime;

		private float _currentTime;

		private string _leaveText;
	}
}
