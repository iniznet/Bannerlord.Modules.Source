using System;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000288 RID: 648
	public class MissionTimer
	{
		// Token: 0x0600226E RID: 8814 RVA: 0x0007D8DD File Offset: 0x0007BADD
		private MissionTimer()
		{
		}

		// Token: 0x0600226F RID: 8815 RVA: 0x0007D8E5 File Offset: 0x0007BAE5
		public MissionTimer(float duration)
		{
			this._startTime = MissionTime.Now;
			this._duration = duration;
		}

		// Token: 0x06002270 RID: 8816 RVA: 0x0007D8FF File Offset: 0x0007BAFF
		public MissionTime GetStartTime()
		{
			return this._startTime;
		}

		// Token: 0x06002271 RID: 8817 RVA: 0x0007D907 File Offset: 0x0007BB07
		public float GetTimerDuration()
		{
			return this._duration;
		}

		// Token: 0x06002272 RID: 8818 RVA: 0x0007D910 File Offset: 0x0007BB10
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

		// Token: 0x06002273 RID: 8819 RVA: 0x0007D96E File Offset: 0x0007BB6E
		public bool Check(bool reset = false)
		{
			bool flag = this.GetRemainingTimeInSeconds(false) <= 0f;
			if (flag && reset)
			{
				this._startTime = MissionTime.Now;
			}
			return flag;
		}

		// Token: 0x06002274 RID: 8820 RVA: 0x0007D991 File Offset: 0x0007BB91
		public static MissionTimer CreateSynchedTimerClient(float startTimeInSeconds, float duration)
		{
			return new MissionTimer
			{
				_startTime = new MissionTime((long)(startTimeInSeconds * 10000000f)),
				_duration = duration
			};
		}

		// Token: 0x04000CDA RID: 3290
		private MissionTime _startTime;

		// Token: 0x04000CDB RID: 3291
		private float _duration;
	}
}
