using System;

namespace psai.net
{
	// Token: 0x0200001D RID: 29
	internal class PsaiTimer
	{
		// Token: 0x06000204 RID: 516 RVA: 0x00008FA4 File Offset: 0x000071A4
		internal PsaiTimer()
		{
			this.m_isSet = false;
		}

		// Token: 0x06000205 RID: 517 RVA: 0x00008FB3 File Offset: 0x000071B3
		internal void SetTimer(int delayMillis, int remainingThresholdMilliseconds)
		{
			this.m_estimatedFireTime = Logik.GetTimestampMillisElapsedSinceInitialisation() + delayMillis;
			this.m_estimatedThresholdReachedTime = this.m_estimatedFireTime - remainingThresholdMilliseconds;
			this.m_isSet = true;
		}

		// Token: 0x06000206 RID: 518 RVA: 0x00008FD7 File Offset: 0x000071D7
		internal bool IsSet()
		{
			return this.m_isSet;
		}

		// Token: 0x06000207 RID: 519 RVA: 0x00008FDF File Offset: 0x000071DF
		internal void Stop()
		{
			this.m_isSet = false;
		}

		// Token: 0x06000208 RID: 520 RVA: 0x00008FE8 File Offset: 0x000071E8
		internal void SetPaused(bool setPaused)
		{
			if (this.m_isSet)
			{
				if (setPaused)
				{
					if (!this.m_isPaused)
					{
						this.m_isPaused = true;
						this.m_timerPausedTimestamp = Logik.GetTimestampMillisElapsedSinceInitialisation();
						return;
					}
				}
				else if (this.m_isPaused)
				{
					this.m_isPaused = false;
					int num = Logik.GetTimestampMillisElapsedSinceInitialisation() - this.m_timerPausedTimestamp;
					this.m_estimatedFireTime += num;
					this.m_estimatedThresholdReachedTime += num;
				}
			}
		}

		// Token: 0x06000209 RID: 521 RVA: 0x00009053 File Offset: 0x00007253
		internal int GetRemainingMillisToFireTime()
		{
			if (this.m_isSet && !this.m_isPaused)
			{
				return this.m_estimatedFireTime - Logik.GetTimestampMillisElapsedSinceInitialisation();
			}
			return 999999;
		}

		// Token: 0x0600020A RID: 522 RVA: 0x00009077 File Offset: 0x00007277
		internal int GetEstimatedFireTime()
		{
			if (this.m_isSet && !this.m_isPaused)
			{
				return this.m_estimatedFireTime;
			}
			return 999999;
		}

		// Token: 0x0600020B RID: 523 RVA: 0x00009095 File Offset: 0x00007295
		internal bool ThresholdHasBeenReached()
		{
			return this.m_isSet && Logik.GetTimestampMillisElapsedSinceInitialisation() >= this.m_estimatedThresholdReachedTime;
		}

		// Token: 0x0400011A RID: 282
		private bool m_isSet;

		// Token: 0x0400011B RID: 283
		private bool m_isPaused;

		// Token: 0x0400011C RID: 284
		private int m_estimatedThresholdReachedTime;

		// Token: 0x0400011D RID: 285
		private int m_estimatedFireTime;

		// Token: 0x0400011E RID: 286
		private int m_timerPausedTimestamp;
	}
}
