using System;

namespace psai.net
{
	internal class ThemeQueueEntry : ICloneable
	{
		internal ThemeQueueEntry()
		{
			this.playmode = PsaiPlayMode.regular;
			this.themeId = -1;
			this.startIntensity = 1f;
			this.restTimeMillis = 0;
			this.holdIntensity = false;
		}

		public object Clone()
		{
			return (ThemeQueueEntry)base.MemberwiseClone();
		}

		public override string ToString()
		{
			return string.Format("playmode:{0}  themeId:{1}  startIntensity:{2}  restTimeMillis:{3}  holdIntensity:{4}  musicDuration:{5}", new object[] { this.playmode, this.themeId, this.startIntensity, this.restTimeMillis, this.holdIntensity, this.musicDuration });
		}

		internal PsaiPlayMode playmode;

		internal int themeId;

		internal float startIntensity;

		internal int restTimeMillis;

		internal bool holdIntensity;

		internal int musicDuration;
	}
}
