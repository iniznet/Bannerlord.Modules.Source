using System;

namespace psai.net
{
	// Token: 0x02000026 RID: 38
	internal class ThemeQueueEntry : ICloneable
	{
		// Token: 0x0600023F RID: 575 RVA: 0x00009F4C File Offset: 0x0000814C
		internal ThemeQueueEntry()
		{
			this.playmode = PsaiPlayMode.regular;
			this.themeId = -1;
			this.startIntensity = 1f;
			this.restTimeMillis = 0;
			this.holdIntensity = false;
		}

		// Token: 0x06000240 RID: 576 RVA: 0x00009F7B File Offset: 0x0000817B
		public object Clone()
		{
			return (ThemeQueueEntry)base.MemberwiseClone();
		}

		// Token: 0x06000241 RID: 577 RVA: 0x00009F88 File Offset: 0x00008188
		public override string ToString()
		{
			return string.Format("playmode:{0}  themeId:{1}  startIntensity:{2}  restTimeMillis:{3}  holdIntensity:{4}  musicDuration:{5}", new object[] { this.playmode, this.themeId, this.startIntensity, this.restTimeMillis, this.holdIntensity, this.musicDuration });
		}

		// Token: 0x04000152 RID: 338
		internal PsaiPlayMode playmode;

		// Token: 0x04000153 RID: 339
		internal int themeId;

		// Token: 0x04000154 RID: 340
		internal float startIntensity;

		// Token: 0x04000155 RID: 341
		internal int restTimeMillis;

		// Token: 0x04000156 RID: 342
		internal bool holdIntensity;

		// Token: 0x04000157 RID: 343
		internal int musicDuration;
	}
}
