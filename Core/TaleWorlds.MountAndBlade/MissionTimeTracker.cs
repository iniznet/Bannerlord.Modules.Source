using System;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000289 RID: 649
	public class MissionTimeTracker
	{
		// Token: 0x1700068B RID: 1675
		// (get) Token: 0x06002275 RID: 8821 RVA: 0x0007D9B2 File Offset: 0x0007BBB2
		// (set) Token: 0x06002276 RID: 8822 RVA: 0x0007D9BA File Offset: 0x0007BBBA
		public long NumberOfTicks { get; private set; }

		// Token: 0x1700068C RID: 1676
		// (get) Token: 0x06002277 RID: 8823 RVA: 0x0007D9C3 File Offset: 0x0007BBC3
		// (set) Token: 0x06002278 RID: 8824 RVA: 0x0007D9CB File Offset: 0x0007BBCB
		public long DeltaTimeInTicks { get; private set; }

		// Token: 0x06002279 RID: 8825 RVA: 0x0007D9D4 File Offset: 0x0007BBD4
		public MissionTimeTracker(MissionTime initialMapTime)
		{
			this.NumberOfTicks = initialMapTime.NumberOfTicks;
		}

		// Token: 0x0600227A RID: 8826 RVA: 0x0007D9E9 File Offset: 0x0007BBE9
		public MissionTimeTracker()
		{
			this.NumberOfTicks = 0L;
		}

		// Token: 0x0600227B RID: 8827 RVA: 0x0007D9F9 File Offset: 0x0007BBF9
		public void Tick(float seconds)
		{
			this.DeltaTimeInTicks = (long)(seconds * 10000000f);
			this.NumberOfTicks += this.DeltaTimeInTicks;
		}

		// Token: 0x0600227C RID: 8828 RVA: 0x0007DA1C File Offset: 0x0007BC1C
		public void UpdateSync(float newValue)
		{
			long num = (long)(newValue * 10000000f);
			this._lastSyncDifference = num - this.NumberOfTicks;
		}

		// Token: 0x0600227D RID: 8829 RVA: 0x0007DA40 File Offset: 0x0007BC40
		public float GetLastSyncDifference()
		{
			return (float)this._lastSyncDifference / 10000000f;
		}

		// Token: 0x04000CDE RID: 3294
		private long _lastSyncDifference;
	}
}
