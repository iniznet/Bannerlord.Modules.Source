using System;

namespace psai.net
{
	internal class PsaiTimer
	{
		internal PsaiTimer()
		{
			this.m_isSet = false;
		}

		internal void SetTimer(int delayMillis, int remainingThresholdMilliseconds)
		{
			this.m_estimatedFireTime = Logik.GetTimestampMillisElapsedSinceInitialisation() + delayMillis;
			this.m_estimatedThresholdReachedTime = this.m_estimatedFireTime - remainingThresholdMilliseconds;
			this.m_isSet = true;
		}

		internal bool IsSet()
		{
			return this.m_isSet;
		}

		internal void Stop()
		{
			this.m_isSet = false;
		}

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

		internal int GetRemainingMillisToFireTime()
		{
			if (this.m_isSet && !this.m_isPaused)
			{
				return this.m_estimatedFireTime - Logik.GetTimestampMillisElapsedSinceInitialisation();
			}
			return 999999;
		}

		internal int GetEstimatedFireTime()
		{
			if (this.m_isSet && !this.m_isPaused)
			{
				return this.m_estimatedFireTime;
			}
			return 999999;
		}

		internal bool ThresholdHasBeenReached()
		{
			return this.m_isSet && Logik.GetTimestampMillisElapsedSinceInitialisation() >= this.m_estimatedThresholdReachedTime;
		}

		private bool m_isSet;

		private bool m_isPaused;

		private int m_estimatedThresholdReachedTime;

		private int m_estimatedFireTime;

		private int m_timerPausedTimestamp;
	}
}
