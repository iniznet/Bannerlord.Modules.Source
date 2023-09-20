using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public class IncrementalTimer
	{
		public float TimerCounter { get; private set; }

		public IncrementalTimer(float totalDuration, float tickInterval)
		{
			this._tickInterval = MathF.Max(tickInterval, 0.01f);
			this._totalDuration = MathF.Max(totalDuration, 0.01f);
			this.TimerCounter = 0f;
			this._timer = new Timer(MBCommon.GetTotalMissionTime(), this._tickInterval, true);
		}

		public bool Check()
		{
			if (this._timer.Check(MBCommon.GetTotalMissionTime()))
			{
				this.TimerCounter += this._tickInterval / this._totalDuration;
				return true;
			}
			return false;
		}

		public bool HasEnded()
		{
			return this.TimerCounter >= 1f;
		}

		private readonly float _totalDuration;

		private readonly float _tickInterval;

		private readonly Timer _timer;
	}
}
