using System;
using System.IO;
using TaleWorlds.Engine;

namespace psai.net
{
	public class AudioPlaybackLayerChannelStandalone : IAudioPlaybackLayerChannel
	{
		public AudioPlaybackLayerChannelStandalone()
		{
			this.index = Music.GetFreeMusicChannelIndex();
		}

		~AudioPlaybackLayerChannelStandalone()
		{
		}

		public void Release()
		{
		}

		internal void StopIfPlaying()
		{
			Music.StopMusic(this.index);
		}

		public PsaiResult LoadSegment(Segment segment)
		{
			this._audioData = segment.audioData;
			string psaiCoreSoundtrackDirectoryName = Logik.Instance.m_psaiCoreSoundtrackDirectoryName;
			string text = Path.Combine(Logik.Instance.m_psaiCoreSoundtrackDirectoryName, this._audioData.filePathRelativeToProjectDir);
			Music.LoadClip(this.index, text);
			return PsaiResult.OK;
		}

		public PsaiResult ReleaseSegment()
		{
			Music.UnloadClip(this.index);
			return PsaiResult.OK;
		}

		public PsaiResult ScheduleSegmentPlayback(Segment snippet, int delayMilliseconds)
		{
			Music.PlayDelayed(this.index, delayMilliseconds);
			return PsaiResult.OK;
		}

		public PsaiResult StopChannel()
		{
			this.StopIfPlaying();
			return PsaiResult.OK;
		}

		public PsaiResult SetVolume(float volume)
		{
			Music.SetVolume(this.index, volume);
			return PsaiResult.OK;
		}

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

		private AudioData _audioData;

		private int index;
	}
}
