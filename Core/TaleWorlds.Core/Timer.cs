using System;

namespace TaleWorlds.Core
{
	public class Timer
	{
		public float Duration { get; protected set; }

		public Timer(float gameTime, float duration, bool autoReset = true)
		{
			this._startTime = gameTime;
			this._latestGameTime = gameTime;
			this._autoReset = autoReset;
			this.Duration = duration;
		}

		public virtual bool Check(float gameTime)
		{
			this._latestGameTime = gameTime;
			if (this.Duration <= 0f)
			{
				this.PreviousDeltaTime = this.ElapsedTime();
				this._startTime = gameTime;
				return true;
			}
			bool flag = false;
			if (this.ElapsedTime() >= this.Duration)
			{
				this.PreviousDeltaTime = this.ElapsedTime();
				if (this._autoReset)
				{
					while (this.ElapsedTime() >= this.Duration)
					{
						this._startTime += this.Duration;
					}
				}
				flag = true;
			}
			return flag;
		}

		public float ElapsedTime()
		{
			return this._latestGameTime - this._startTime;
		}

		public float PreviousDeltaTime { get; private set; }

		public void Reset(float gameTime)
		{
			this.Reset(gameTime, this.Duration);
		}

		public void Reset(float gameTime, float newDuration)
		{
			this._startTime = gameTime;
			this._latestGameTime = gameTime;
			this.Duration = newDuration;
		}

		public void AdjustStartTime(float deltaTime)
		{
			this._startTime += deltaTime;
		}

		private float _startTime;

		private float _latestGameTime;

		private bool _autoReset;
	}
}
