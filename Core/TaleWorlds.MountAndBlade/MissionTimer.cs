using System;

namespace TaleWorlds.MountAndBlade
{
	public class MissionTimer
	{
		private MissionTimer()
		{
		}

		public MissionTimer(float duration)
		{
			this._startTime = MissionTime.Now;
			this._duration = duration;
		}

		public MissionTime GetStartTime()
		{
			return this._startTime;
		}

		public float GetTimerDuration()
		{
			return this._duration;
		}

		public float GetRemainingTimeInSeconds(bool synched = false)
		{
			if (this._duration < 0f)
			{
				return 0f;
			}
			float num = this._duration - this._startTime.ElapsedSeconds;
			if (synched && GameNetwork.IsClientOrReplay)
			{
				num -= Mission.Current.MissionTimeTracker.GetLastSyncDifference();
			}
			if (num <= 0f)
			{
				return 0f;
			}
			return num;
		}

		public bool Check(bool reset = false)
		{
			bool flag = this.GetRemainingTimeInSeconds(false) <= 0f;
			if (flag && reset)
			{
				this._startTime = MissionTime.Now;
			}
			return flag;
		}

		public static MissionTimer CreateSynchedTimerClient(float startTimeInSeconds, float duration)
		{
			return new MissionTimer
			{
				_startTime = new MissionTime((long)(startTimeInSeconds * 10000000f)),
				_duration = duration
			};
		}

		private MissionTime _startTime;

		private float _duration;
	}
}
