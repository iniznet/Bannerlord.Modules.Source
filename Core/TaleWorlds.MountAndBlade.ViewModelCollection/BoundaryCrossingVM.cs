using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection
{
	public class BoundaryCrossingVM : ViewModel
	{
		public BoundaryCrossingVM(Mission mission, Action<bool> onEscapeMenuToggled)
		{
			this._onEscapeMenuToggled = onEscapeMenuToggled;
			this._mission = mission;
			this._missionBoundaryCrossingHandler = this._mission.GetMissionBehavior<MissionBoundaryCrossingHandler>();
			this._missionBoundaryCrossingHandler.StartTime += this.OnStartTime;
			this._missionBoundaryCrossingHandler.StopTime += this.OnStopTime;
			this._missionBoundaryCrossingHandler.TimeCount += this.OnTimeCount;
			this._show = false;
			this.WarningText = "";
			this.WarningProgress = 0.0;
		}

		[DataSourceProperty]
		public bool Show
		{
			get
			{
				return this._show;
			}
			set
			{
				if (value != this._show)
				{
					this._show = value;
					base.OnPropertyChangedWithValue(value, "Show");
					this._onEscapeMenuToggled(value);
				}
			}
		}

		[DataSourceProperty]
		public string WarningText
		{
			get
			{
				return this._warningText;
			}
			set
			{
				if (value != this._warningText)
				{
					this._warningText = value;
					base.OnPropertyChangedWithValue<string>(value, "WarningText");
				}
			}
		}

		[DataSourceProperty]
		public double WarningProgress
		{
			get
			{
				return this._warningProgress;
			}
			set
			{
				if (value != this._warningProgress)
				{
					this._warningProgress = value;
					base.OnPropertyChangedWithValue(value, "WarningProgress");
					this.WarningIntProgress = (int)(value * 100.0);
				}
			}
		}

		[DataSourceProperty]
		public int WarningIntProgress
		{
			get
			{
				return this._warningIntProgress;
			}
			set
			{
				if (value != this._warningIntProgress)
				{
					this._warningIntProgress = value;
					base.OnPropertyChangedWithValue(value, "WarningIntProgress");
				}
			}
		}

		[DataSourceProperty]
		public int Countdown
		{
			get
			{
				return this._countdown;
			}
			set
			{
				if (value != this._countdown)
				{
					this._countdown = value;
					base.OnPropertyChangedWithValue(value, "Countdown");
				}
			}
		}

		private void OnStartTime(float duration, float progress)
		{
			TextObject textObject = new TextObject("{=eGuQKRhb}You are leaving the area!", null);
			switch (this._mission.Mode)
			{
			case MissionMode.StartUp:
			case MissionMode.Battle:
			case MissionMode.Duel:
			case MissionMode.Stealth:
			case MissionMode.Deployment:
				goto IL_66;
			case MissionMode.Conversation:
				textObject = TextObject.Empty;
				goto IL_66;
			case MissionMode.Barter:
				textObject = TextObject.Empty;
				goto IL_66;
			case MissionMode.CutScene:
				textObject = TextObject.Empty;
				goto IL_66;
			}
			throw new ArgumentOutOfRangeException();
			IL_66:
			MBTextManager.SetTextVariable("MISSION_MODE", textObject, false);
			this.WarningText = GameTexts.FindText("str_out_of_mission_bound", null).ToString();
			this.WarningProgress = 0.0;
			this._duration = duration;
			this.Show = true;
		}

		private void OnStopTime()
		{
			this.Show = false;
		}

		private void OnTimeCount(float progress)
		{
			this.WarningProgress = (double)progress;
			this.Countdown = MathF.Ceiling((1f - progress) * this._duration);
		}

		private readonly Mission _mission;

		private readonly MissionBoundaryCrossingHandler _missionBoundaryCrossingHandler;

		private readonly Action<bool> _onEscapeMenuToggled;

		private float _duration;

		private bool _show = true;

		private string _warningText;

		private double _warningProgress = -1.0;

		private int _warningIntProgress = -1;

		private int _countdown;
	}
}
