using System;

namespace psai.net
{
	public class AudioData
	{
		public int GetFullLengthInMilliseconds()
		{
			return (int)((long)this.sampleCountTotal * 1000L / (long)this.sampleRateHz);
		}

		public int GetPreBeatZoneInMilliseconds()
		{
			return (int)((long)this.sampleCountPreBeat * 1000L / (long)this.sampleRateHz);
		}

		public int GetPostBeatZoneInMilliseconds()
		{
			return (int)((long)this.sampleCountPostBeat * 1000L / (long)this.sampleRateHz);
		}

		public int GetSampleCountByMilliseconds(int milliSeconds)
		{
			return (int)((long)this.sampleRateHz * (long)milliSeconds / 1000L);
		}

		public string filePathRelativeToProjectDir;

		public int sampleCountTotal;

		public int sampleCountPreBeat;

		public int sampleCountPostBeat;

		public int sampleRateHz;

		public float bpm;
	}
}
