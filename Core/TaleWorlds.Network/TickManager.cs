using System;
using System.Diagnostics;
using System.Threading;

namespace TaleWorlds.Network
{
	// Token: 0x02000029 RID: 41
	public class TickManager
	{
		// Token: 0x06000136 RID: 310 RVA: 0x00005054 File Offset: 0x00003254
		public TickManager(int tickRate, TickManager.TickDelegate tickMethod)
		{
			this._tickRate = tickRate;
			this._tickMethod = tickMethod;
			this._numberOfTicksPerMilisecond = (double)Stopwatch.Frequency / 1000.0;
			this._inverseNumberOfTicksPerMilisecond = 1000.0 / (double)Stopwatch.Frequency;
			this._maxTickMilisecond = 1000.0 / (double)this._tickRate;
			this._stopwatch = new Stopwatch();
			this._stopwatch.Start();
		}

		// Token: 0x06000137 RID: 311 RVA: 0x000050DC File Offset: 0x000032DC
		public void Tick()
		{
			long elapsedTicks = this._stopwatch.ElapsedTicks;
			this._tickMethod();
			double num = (double)(this._stopwatch.ElapsedTicks - elapsedTicks);
			double num2 = this._inverseNumberOfTicksPerMilisecond * num;
			if (num2 < this._maxTickMilisecond)
			{
				double num3 = this._maxTickMilisecond - num2;
				num3 += this._residualWaitTime;
				int num4 = (int)num3;
				this._residualWaitTime = num3 - (double)num4;
				if (num4 > 0)
				{
					Thread.Sleep(num4);
				}
			}
		}

		// Token: 0x04000073 RID: 115
		private Stopwatch _stopwatch;

		// Token: 0x04000074 RID: 116
		private int _tickRate = 5000;

		// Token: 0x04000075 RID: 117
		private TickManager.TickDelegate _tickMethod;

		// Token: 0x04000076 RID: 118
		private double _residualWaitTime;

		// Token: 0x04000077 RID: 119
		private double _numberOfTicksPerMilisecond;

		// Token: 0x04000078 RID: 120
		private double _inverseNumberOfTicksPerMilisecond;

		// Token: 0x04000079 RID: 121
		private double _maxTickMilisecond;

		// Token: 0x02000042 RID: 66
		// (Invoke) Token: 0x06000194 RID: 404
		public delegate void TickDelegate();
	}
}
