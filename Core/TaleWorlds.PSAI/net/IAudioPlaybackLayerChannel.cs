using System;

namespace psai.net
{
	// Token: 0x0200000E RID: 14
	internal interface IAudioPlaybackLayerChannel
	{
		// Token: 0x06000131 RID: 305
		PsaiResult LoadSegment(Segment segment);

		// Token: 0x06000132 RID: 306
		PsaiResult ReleaseSegment();

		// Token: 0x06000133 RID: 307
		PsaiResult ScheduleSegmentPlayback(Segment segment, int delayMilliseconds);

		// Token: 0x06000134 RID: 308
		PsaiResult StopChannel();

		// Token: 0x06000135 RID: 309
		PsaiResult SetVolume(float volume);

		// Token: 0x06000136 RID: 310
		PsaiResult SetPaused(bool paused);

		// Token: 0x06000137 RID: 311
		void Release();
	}
}
