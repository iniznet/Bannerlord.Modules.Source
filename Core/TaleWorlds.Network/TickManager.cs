using System;
using System.Diagnostics;
using System.Threading;

namespace TaleWorlds.Network
{
	public class TickManager
	{
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

		private Stopwatch _stopwatch;

		private int _tickRate = 5000;

		private TickManager.TickDelegate _tickMethod;

		private double _residualWaitTime;

		private double _numberOfTicksPerMilisecond;

		private double _inverseNumberOfTicksPerMilisecond;

		private double _maxTickMilisecond;

		public delegate void TickDelegate();
	}
}
