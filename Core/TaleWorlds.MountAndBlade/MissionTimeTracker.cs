using System;

namespace TaleWorlds.MountAndBlade
{
	public class MissionTimeTracker
	{
		public long NumberOfTicks { get; private set; }

		public long DeltaTimeInTicks { get; private set; }

		public MissionTimeTracker(MissionTime initialMapTime)
		{
			this.NumberOfTicks = initialMapTime.NumberOfTicks;
		}

		public MissionTimeTracker()
		{
			this.NumberOfTicks = 0L;
		}

		public void Tick(float seconds)
		{
			this.DeltaTimeInTicks = (long)(seconds * 10000000f);
			this.NumberOfTicks += this.DeltaTimeInTicks;
		}

		public void UpdateSync(float newValue)
		{
			long num = (long)(newValue * 10000000f);
			this._lastSyncDifference = num - this.NumberOfTicks;
		}

		public float GetLastSyncDifference()
		{
			return (float)this._lastSyncDifference / 10000000f;
		}

		private long _lastSyncDifference;
	}
}
