using System;
using System.IO;
using TaleWorlds.Engine;

namespace psai.net
{
	// Token: 0x0200000D RID: 13
	public class AudioPlaybackLayerChannelStandalone : IAudioPlaybackLayerChannel
	{
		// Token: 0x06000127 RID: 295 RVA: 0x00005E7F File Offset: 0x0000407F
		public AudioPlaybackLayerChannelStandalone()
		{
			this.index = Music.GetFreeMusicChannelIndex();
		}

		// Token: 0x06000128 RID: 296 RVA: 0x00005E94 File Offset: 0x00004094
		~AudioPlaybackLayerChannelStandalone()
		{
		}

		// Token: 0x06000129 RID: 297 RVA: 0x00005EBC File Offset: 0x000040BC
		public void Release()
		{
		}

		// Token: 0x0600012A RID: 298 RVA: 0x00005EBE File Offset: 0x000040BE
		internal void StopIfPlaying()
		{
			Music.StopMusic(this.index);
		}

		// Token: 0x0600012B RID: 299 RVA: 0x00005ECC File Offset: 0x000040CC
		public PsaiResult LoadSegment(Segment segment)
		{
			this._audioData = segment.audioData;
			string psaiCoreSoundtrackDirectoryName = Logik.Instance.m_psaiCoreSoundtrackDirectoryName;
			string text = Path.Combine(Logik.Instance.m_psaiCoreSoundtrackDirectoryName, this._audioData.filePathRelativeToProjectDir);
			Music.LoadClip(this.index, text);
			return PsaiResult.OK;
		}

		// Token: 0x0600012C RID: 300 RVA: 0x00005F18 File Offset: 0x00004118
		public PsaiResult ReleaseSegment()
		{
			Music.UnloadClip(this.index);
			return PsaiResult.OK;
		}

		// Token: 0x0600012D RID: 301 RVA: 0x00005F26 File Offset: 0x00004126
		public PsaiResult ScheduleSegmentPlayback(Segment snippet, int delayMilliseconds)
		{
			Music.PlayDelayed(this.index, delayMilliseconds);
			return PsaiResult.OK;
		}

		// Token: 0x0600012E RID: 302 RVA: 0x00005F35 File Offset: 0x00004135
		public PsaiResult StopChannel()
		{
			this.StopIfPlaying();
			return PsaiResult.OK;
		}

		// Token: 0x0600012F RID: 303 RVA: 0x00005F3E File Offset: 0x0000413E
		public PsaiResult SetVolume(float volume)
		{
			Music.SetVolume(this.index, volume);
			return PsaiResult.OK;
		}

		// Token: 0x06000130 RID: 304 RVA: 0x00005F4D File Offset: 0x0000414D
		public PsaiResult SetPaused(bool paused)
		{
			if (paused)
			{
				Music.PauseMusic(this.index);
			}
			else
			{
				Music.PlayMusic(this.index);
			}
			return PsaiResult.OK;
		}

		// Token: 0x0400007A RID: 122
		private AudioData _audioData;

		// Token: 0x0400007B RID: 123
		private int index;
	}
}
