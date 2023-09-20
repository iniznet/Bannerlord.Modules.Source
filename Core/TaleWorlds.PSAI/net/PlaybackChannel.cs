using System;

namespace psai.net
{
	public class PlaybackChannel
	{
		internal Segment Segment { get; private set; }

		internal PlaybackChannel(Logik logik, bool isMainChannel)
		{
			this.m_audioPlaybackLayerChannel = new AudioPlaybackLayerChannelStandalone();
			this.m_logik = logik;
			this.m_isMainChannel = isMainChannel;
		}

		internal void Release()
		{
			this.m_audioPlaybackLayerChannel.Release();
		}

		internal bool Paused
		{
			get
			{
				return this.m_paused;
			}
			set
			{
				this.m_paused = value;
				this.m_audioPlaybackLayerChannel.SetPaused(value);
			}
		}

		internal ChannelState GetCurrentChannelState()
		{
			if (this.Segment == null || this.m_stoppedExplicitly)
			{
				return ChannelState.stopped;
			}
			float num = (float)this.GetCountdownToPlaybackInMilliseconds();
			if (this.m_playbackIsScheduled && num > 0f)
			{
				return ChannelState.load;
			}
			if (num * -1f > (float)this.Segment.audioData.GetFullLengthInMilliseconds())
			{
				return ChannelState.stopped;
			}
			return ChannelState.playing;
		}

		internal bool IsPlaying()
		{
			return this.GetCurrentChannelState() == ChannelState.playing;
		}

		internal void LoadSegment(Segment snippet)
		{
			this.Segment = snippet;
			this.m_timeStampOfSnippetLoad = Logik.GetTimestampMillisElapsedSinceInitialisation();
			this.m_playbackIsScheduled = false;
			this.m_stoppedExplicitly = false;
			if (this.m_audioPlaybackLayerChannel != null)
			{
				this.m_audioPlaybackLayerChannel.LoadSegment(snippet);
			}
		}

		internal bool CheckIfSegmentHadEnoughTimeToLoad()
		{
			return this.GetMillisecondsSinceSegmentLoad() >= Logik.s_audioLayerMaximumLatencyForBufferingSounds;
		}

		internal int GetMillisecondsSinceSegmentLoad()
		{
			return Logik.GetTimestampMillisElapsedSinceInitialisation() - this.m_timeStampOfSnippetLoad;
		}

		internal int GetMillisecondsUntilLoadingWillHaveFinished()
		{
			return Math.Max(0, Logik.s_audioLayerMaximumLatencyForBufferingSounds - this.GetMillisecondsSinceSegmentLoad());
		}

		internal void StopChannel()
		{
			this.m_stoppedExplicitly = true;
			if (this.m_audioPlaybackLayerChannel != null)
			{
				this.m_audioPlaybackLayerChannel.StopChannel();
			}
		}

		internal void ReleaseSegment()
		{
			this.Segment = null;
			if (this.m_audioPlaybackLayerChannel != null)
			{
				this.m_audioPlaybackLayerChannel.ReleaseSegment();
			}
		}

		internal int GetCountdownToPlaybackInMilliseconds()
		{
			return this.m_timeStampOfPlaybackStart - Logik.GetTimestampMillisElapsedSinceInitialisation();
		}

		internal void ScheduleSegmentPlayback(Segment snippet, int delayInMilliseconds)
		{
			if (delayInMilliseconds < 0)
			{
				delayInMilliseconds = 0;
			}
			if (snippet != this.Segment)
			{
				this.LoadSegment(snippet);
			}
			this.m_stoppedExplicitly = false;
			this.m_playbackIsScheduled = true;
			this.m_timeStampOfPlaybackStart = Logik.GetTimestampMillisElapsedSinceInitialisation() + delayInMilliseconds;
			if (this.m_audioPlaybackLayerChannel != null)
			{
				this.m_audioPlaybackLayerChannel.ScheduleSegmentPlayback(snippet, delayInMilliseconds);
			}
		}

		internal float MasterVolume
		{
			get
			{
				return this.m_masterVolume;
			}
			set
			{
				if (value >= 0f && value <= 1f)
				{
					this.m_masterVolume = value;
					this.UpdateVolume();
				}
			}
		}

		internal float FadeOutVolume
		{
			get
			{
				return this.m_fadeOutVolume;
			}
			set
			{
				if (value >= 0f && value <= 1f)
				{
					this.m_fadeOutVolume = value;
					this.UpdateVolume();
				}
			}
		}

		private void UpdateVolume()
		{
			if (this.m_audioPlaybackLayerChannel != null)
			{
				float num = this.MasterVolume * this.FadeOutVolume;
				this.m_audioPlaybackLayerChannel.SetVolume(num);
			}
		}

		public void OnPlaybackHasStarted()
		{
			this.m_timeStampOfPlaybackStart = Logik.GetTimestampMillisElapsedSinceInitialisation();
			if (this.m_isMainChannel)
			{
				this.m_logik.SetSegmentEndApproachingAndReachedTimersAfterPlaybackHasStarted(0);
			}
		}

		private Logik m_logik;

		private int m_timeStampOfPlaybackStart;

		private int m_timeStampOfSnippetLoad;

		private bool m_playbackIsScheduled;

		private bool m_stoppedExplicitly;

		private IAudioPlaybackLayerChannel m_audioPlaybackLayerChannel;

		private float m_masterVolume = 1f;

		private float m_fadeOutVolume = 1f;

		private bool m_paused;

		private bool m_isMainChannel;
	}
}
