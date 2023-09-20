using System;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020001CF RID: 463
	public class BasicMissionTimer
	{
		// Token: 0x17000526 RID: 1318
		// (get) Token: 0x06001A35 RID: 6709 RVA: 0x0005C736 File Offset: 0x0005A936
		public float ElapsedTime
		{
			get
			{
				return MBCommon.GetTotalMissionTime() - this._startTime;
			}
		}

		// Token: 0x06001A36 RID: 6710 RVA: 0x0005C744 File Offset: 0x0005A944
		public BasicMissionTimer()
		{
			this._startTime = MBCommon.GetTotalMissionTime();
		}

		// Token: 0x06001A37 RID: 6711 RVA: 0x0005C757 File Offset: 0x0005A957
		public void Reset()
		{
			this._startTime = MBCommon.GetTotalMissionTime();
		}

		// Token: 0x04000830 RID: 2096
		private float _startTime;
	}
}
