using System;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200038B RID: 907
	public class RandomTimer : Timer
	{
		// Token: 0x060031BE RID: 12734 RVA: 0x000CF03F File Offset: 0x000CD23F
		public RandomTimer(float gameTime, float durationMin, float durationMax)
			: base(gameTime, MBRandom.RandomFloatRanged(durationMin, durationMax), true)
		{
			this.durationMin = durationMin;
			this.durationMax = durationMax;
		}

		// Token: 0x060031BF RID: 12735 RVA: 0x000CF060 File Offset: 0x000CD260
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

		// Token: 0x060031C0 RID: 12736 RVA: 0x000CF086 File Offset: 0x000CD286
		public void ChangeDuration(float min, float max)
		{
			this.durationMin = min;
			this.durationMax = max;
			this.RecomputeDuration();
		}

		// Token: 0x060031C1 RID: 12737 RVA: 0x000CF09C File Offset: 0x000CD29C
		public void RecomputeDuration()
		{
			base.Duration = MBRandom.RandomFloatRanged(this.durationMin, this.durationMax);
		}

		// Token: 0x040014E2 RID: 5346
		private float durationMin;

		// Token: 0x040014E3 RID: 5347
		private float durationMax;
	}
}
