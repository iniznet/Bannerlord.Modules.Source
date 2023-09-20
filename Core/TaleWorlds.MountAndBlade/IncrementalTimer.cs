using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000239 RID: 569
	public class IncrementalTimer
	{
		// Token: 0x17000626 RID: 1574
		// (get) Token: 0x06001F4F RID: 8015 RVA: 0x0006EECF File Offset: 0x0006D0CF
		// (set) Token: 0x06001F50 RID: 8016 RVA: 0x0006EED7 File Offset: 0x0006D0D7
		public float TimerCounter { get; private set; }

		// Token: 0x06001F51 RID: 8017 RVA: 0x0006EEE0 File Offset: 0x0006D0E0
		public IncrementalTimer(float totalDuration, float tickInterval)
		{
			this._tickInterval = MathF.Max(tickInterval, 0.01f);
			this._totalDuration = MathF.Max(totalDuration, 0.01f);
			this.TimerCounter = 0f;
			this._timer = new Timer(MBCommon.GetTotalMissionTime(), this._tickInterval, true);
		}

		// Token: 0x06001F52 RID: 8018 RVA: 0x0006EF37 File Offset: 0x0006D137
		public bool Check()
		{
			if (this._timer.Check(MBCommon.GetTotalMissionTime()))
			{
				this.TimerCounter += this._tickInterval / this._totalDuration;
				return true;
			}
			return false;
		}

		// Token: 0x06001F53 RID: 8019 RVA: 0x0006EF68 File Offset: 0x0006D168
		public bool HasEnded()
		{
			return this.TimerCounter >= 1f;
		}

		// Token: 0x04000B72 RID: 2930
		private readonly float _totalDuration;

		// Token: 0x04000B73 RID: 2931
		private readonly float _tickInterval;

		// Token: 0x04000B74 RID: 2932
		private readonly Timer _timer;
	}
}
