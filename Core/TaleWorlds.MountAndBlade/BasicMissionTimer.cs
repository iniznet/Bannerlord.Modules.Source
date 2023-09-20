using System;

namespace TaleWorlds.MountAndBlade
{
	public class BasicMissionTimer
	{
		public float ElapsedTime
		{
			get
			{
				return MBCommon.GetTotalMissionTime() - this._startTime;
			}
		}

		public BasicMissionTimer()
		{
			this._startTime = MBCommon.GetTotalMissionTime();
		}

		public void Reset()
		{
			this._startTime = MBCommon.GetTotalMissionTime();
		}

		private float _startTime;
	}
}
