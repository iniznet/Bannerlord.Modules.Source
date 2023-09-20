using System;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
	public class RandomTimer : Timer
	{
		public RandomTimer(float gameTime, float durationMin, float durationMax)
			: base(gameTime, MBRandom.RandomFloatRanged(durationMin, durationMax), true)
		{
			this.durationMin = durationMin;
			this.durationMax = durationMax;
		}

		public override bool Check(float gameTime)
		{
			bool flag = false;
			bool flag2;
			do
			{
				flag2 = base.Check(gameTime);
				if (flag2)
				{
					this.RecomputeDuration();
					flag = true;
				}
			}
			while (flag2);
			return flag;
		}

		public void ChangeDuration(float min, float max)
		{
			this.durationMin = min;
			this.durationMax = max;
			this.RecomputeDuration();
		}

		public void RecomputeDuration()
		{
			base.Duration = MBRandom.RandomFloatRanged(this.durationMin, this.durationMax);
		}

		private float durationMin;

		private float durationMax;
	}
}
