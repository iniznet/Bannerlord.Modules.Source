using System;

namespace psai.net
{
	// Token: 0x0200000C RID: 12
	public class AudioData
	{
		// Token: 0x06000123 RID: 291 RVA: 0x00005E20 File Offset: 0x00004020
		public int GetFullLengthInMilliseconds()
		{
			return (int)((long)this.sampleCountTotal * 1000L / (long)this.sampleRateHz);
		}

		// Token: 0x06000124 RID: 292 RVA: 0x00005E39 File Offset: 0x00004039
		public int GetPreBeatZoneInMilliseconds()
		{
			return (int)((long)this.sampleCountPreBeat * 1000L / (long)this.sampleRateHz);
		}

		// Token: 0x06000125 RID: 293 RVA: 0x00005E52 File Offset: 0x00004052
		public int GetPostBeatZoneInMilliseconds()
		{
			return (int)((long)this.sampleCountPostBeat * 1000L / (long)this.sampleRateHz);
		}

		// Token: 0x06000126 RID: 294 RVA: 0x00005E6B File Offset: 0x0000406B
		public int GetSampleCountByMilliseconds(int milliSeconds)
		{
			return (int)((long)this.sampleRateHz * (long)milliSeconds / 1000L);
		}

		// Token: 0x04000074 RID: 116
		public string filePathRelativeToProjectDir;

		// Token: 0x04000075 RID: 117
		public int sampleCountTotal;

		// Token: 0x04000076 RID: 118
		public int sampleCountPreBeat;

		// Token: 0x04000077 RID: 119
		public int sampleCountPostBeat;

		// Token: 0x04000078 RID: 120
		public int sampleRateHz;

		// Token: 0x04000079 RID: 121
		public float bpm;
	}
}
