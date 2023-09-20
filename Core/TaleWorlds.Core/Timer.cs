using System;

namespace TaleWorlds.Core
{
	// Token: 0x020000C1 RID: 193
	public class Timer
	{
		// Token: 0x17000332 RID: 818
		// (get) Token: 0x06000980 RID: 2432 RVA: 0x0001F920 File Offset: 0x0001DB20
		// (set) Token: 0x06000981 RID: 2433 RVA: 0x0001F928 File Offset: 0x0001DB28
		public float Duration { get; protected set; }

		// Token: 0x06000982 RID: 2434 RVA: 0x0001F931 File Offset: 0x0001DB31
		public Timer(float gameTime, float duration, bool autoReset = true)
		{
			this._startTime = gameTime;
			this._latestGameTime = gameTime;
			this._autoReset = autoReset;
			this.Duration = duration;
		}

		// Token: 0x06000983 RID: 2435 RVA: 0x0001F958 File Offset: 0x0001DB58
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

		// Token: 0x06000984 RID: 2436 RVA: 0x0001F9D8 File Offset: 0x0001DBD8
		public float ElapsedTime()
		{
			return this._latestGameTime - this._startTime;
		}

		// Token: 0x17000333 RID: 819
		// (get) Token: 0x06000985 RID: 2437 RVA: 0x0001F9E7 File Offset: 0x0001DBE7
		// (set) Token: 0x06000986 RID: 2438 RVA: 0x0001F9EF File Offset: 0x0001DBEF
		public float PreviousDeltaTime { get; private set; }

		// Token: 0x06000987 RID: 2439 RVA: 0x0001F9F8 File Offset: 0x0001DBF8
		public void Reset(float gameTime)
		{
			this.Reset(gameTime, this.Duration);
		}

		// Token: 0x06000988 RID: 2440 RVA: 0x0001FA07 File Offset: 0x0001DC07
		public void Reset(float gameTime, float newDuration)
		{
			this._startTime = gameTime;
			this._latestGameTime = gameTime;
			this.Duration = newDuration;
		}

		// Token: 0x06000989 RID: 2441 RVA: 0x0001FA1E File Offset: 0x0001DC1E
		public void AdjustStartTime(float deltaTime)
		{
			this._startTime += deltaTime;
		}

		// Token: 0x0400058D RID: 1421
		private float _startTime;

		// Token: 0x0400058E RID: 1422
		private float _latestGameTime;

		// Token: 0x0400058F RID: 1423
		private bool _autoReset;
	}
}
