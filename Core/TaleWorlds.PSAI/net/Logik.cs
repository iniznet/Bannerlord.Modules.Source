using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using psai.Editor;

namespace psai.net
{
	internal class Logik
	{
		internal static Logik Instance { get; private set; }

		internal void Release()
		{
			for (int i = 0; i < this.m_playbackChannels.Length; i++)
			{
				this.m_playbackChannels[i].Release();
			}
			this.m_platformLayer.Release();
		}

		internal static int GetRandomInt(int min, int max)
		{
			return Logik.s_random.Next(min, max);
		}

		internal static float GetRandomFloat()
		{
			return (float)Logik.s_random.NextDouble();
		}

		private static void UpdateMaximumLatencyForPlayingBackUnbufferedSounds()
		{
			Logik.s_audioLayerMaximumLatencyForPlayingBackUnbufferedSounds = Logik.s_audioLayerMaximumLatencyForBufferingSounds + Logik.s_audioLayerMaximumLatencyForPlayingbackPrebufferedSounds;
		}

		internal PsaiResult SetMaximumLatencyNeededByPlatformToBufferSounddata(int latencyInMilliseconds)
		{
			if (latencyInMilliseconds >= 0)
			{
				Logik.s_audioLayerMaximumLatencyForBufferingSounds = latencyInMilliseconds;
				Logik.UpdateMaximumLatencyForPlayingBackUnbufferedSounds();
				return PsaiResult.OK;
			}
			return PsaiResult.invalidParam;
		}

		internal PsaiResult SetMaximumLatencyNeededByPlatformToPlayBackBufferedSounds(int latencyInMilliseconds)
		{
			if (latencyInMilliseconds >= 0)
			{
				Logik.s_audioLayerMaximumLatencyForBufferingSounds = latencyInMilliseconds;
				Logik.UpdateMaximumLatencyForPlayingBackUnbufferedSounds();
				return PsaiResult.OK;
			}
			return PsaiResult.invalidParam;
		}

		static Logik()
		{
			Logik.m_stopWatch.Start();
			Logik.UpdateMaximumLatencyForPlayingBackUnbufferedSounds();
			Logik.Instance = new Logik();
		}

		private Logik()
		{
			this.m_platformLayer = new PlatformLayerStandalone(this);
			this.m_platformLayer.Initialize();
			this.m_soundtrack = new Soundtrack();
			this.m_themeQueue = new List<ThemeQueueEntry>();
			this.m_fadeVoices = new List<FadeData>();
			for (int i = 0; i < 9; i++)
			{
				bool flag = i < 7;
				this.m_playbackChannels[i] = new PlaybackChannel(this, flag);
			}
			this.m_hilightVoiceIndex = -1;
			this.m_lastRegularVoiceNumberReturned = -1;
			this.m_currentVoiceNumber = -1;
			this.m_targetVoice = -1;
			this.m_psaiMasterVolume = 1f;
			this.m_effectiveTheme = null;
			this.m_currentSegmentPlaying = null;
			this.m_currentSnippetTypeRequested = 0;
			this.m_targetSegment = null;
			this.m_targetSegmentSuitabilitiesRequested = 0;
			this.m_psaiState = PsaiState.notready;
			this.m_psaiStateIntended = PsaiState.notready;
			this.m_paused = false;
		}

		private Logik(string pathToPcbFile)
			: this()
		{
			this.LoadSoundtrack(pathToPcbFile);
		}

		internal PsaiResult LoadSoundtrackFromProjectFile(string pathToProjectFile)
		{
			this.m_psaiCoreSoundtackFilepath = pathToProjectFile;
			this.m_psaiCoreSoundtrackDirectoryName = Path.GetDirectoryName(pathToProjectFile);
			this.m_initializationFailure = false;
			PsaiResult psaiResult;
			using (Stream streamOnPsaiSoundtrackFile = this.m_platformLayer.GetStreamOnPsaiSoundtrackFile(this.m_psaiCoreSoundtackFilepath))
			{
				if (streamOnPsaiSoundtrackFile == null)
				{
					psaiResult = PsaiResult.file_notFound;
				}
				else
				{
					PsaiProject psaiProject = PsaiProject.LoadProjectFromStream(streamOnPsaiSoundtrackFile);
					if (psaiProject != null)
					{
						psaiResult = this.LoadSoundtrackByPsaiProject(psaiProject, pathToProjectFile);
					}
					else
					{
						psaiResult = PsaiResult.error_file;
					}
				}
			}
			return psaiResult;
		}

		public PsaiResult LoadSoundtrackByPsaiProject(PsaiProject psaiProject, string fullProjectPath)
		{
			this.m_soundtrack = psaiProject.BuildPsaiDotNetSoundtrackFromProject();
			this.m_psaiCoreSoundtackFilepath = fullProjectPath;
			this.m_psaiCoreSoundtrackDirectoryName = Path.GetDirectoryName(fullProjectPath);
			this.InitMembersAfterSoundtrackHasLoaded();
			return PsaiResult.OK;
		}

		internal PsaiResult LoadSoundtrack(string pathToPcbFile)
		{
			this.m_psaiCoreSoundtackFilepath = pathToPcbFile;
			this.m_psaiCoreSoundtrackDirectoryName = Path.GetDirectoryName(this.m_psaiCoreSoundtackFilepath);
			this.m_initializationFailure = false;
			this.m_soundtrack = new Soundtrack();
			using (Stream streamOnPsaiSoundtrackFile = this.m_platformLayer.GetStreamOnPsaiSoundtrackFile(this.m_psaiCoreSoundtackFilepath))
			{
				PsaiResult psaiResult = this.Readfile_ProtoBuf(streamOnPsaiSoundtrackFile);
				if (psaiResult != PsaiResult.OK)
				{
					return psaiResult;
				}
			}
			this.InitMembersAfterSoundtrackHasLoaded();
			return PsaiResult.OK;
		}

		private void InitMembersAfterSoundtrackHasLoaded()
		{
			this.m_themeQueue.Clear();
			this.m_fadeVoices.Clear();
			foreach (Segment segment in this.m_soundtrack.m_snippets.Values)
			{
				segment.audioData.filePathRelativeToProjectDir = this.m_platformLayer.ConvertFilePathForPlatform(segment.audioData.filePathRelativeToProjectDir);
			}
			this.m_soundtrack.UpdateMaxPreBeatMsOfCompatibleMiddleOrBridgeSnippets();
			this.m_lastBasicMood = this.m_soundtrack.getThemeById(this.GetLastBasicMoodId());
			this.m_psaiState = PsaiState.silence;
			this.m_psaiStateIntended = PsaiState.silence;
			this.m_psaiPlayMode = PsaiPlayMode.regular;
			this.m_psaiPlayModeIntended = PsaiPlayMode.regular;
			this.m_returnToLastBasicMoodFlag = false;
			this.m_holdIntensity = false;
			this.m_latestEndOfSegmentTriggerCall.themeId = -1;
			this.m_soundtrack.BuildAllIndirectionSequences();
		}

		internal int GetLastBasicMoodId()
		{
			if (this.m_lastBasicMood != null)
			{
				return this.m_lastBasicMood.id;
			}
			return -1;
		}

		public void SetLastBasicMood(int themeId)
		{
			Theme themeById = this.m_soundtrack.getThemeById(themeId);
			if (themeById != null)
			{
				this.SetThemeAsLastBasicMood(themeById);
				return;
			}
			this.m_lastBasicMood = null;
		}

		internal static bool CheckIfFileExists(string filepath)
		{
			return File.Exists(filepath);
		}

		private int GetRemainingMusicDurationSecondsOfCurrentTheme()
		{
			int num = Logik.GetTimestampMillisElapsedSinceInitialisation() - this.m_timeStampOfLastIntensitySetForCurrentTheme;
			return this.m_lastMusicDuration - num / 1000;
		}

		internal static int GetTimestampMillisElapsedSinceInitialisation()
		{
			return (int)Logik.m_stopWatch.ElapsedMilliseconds;
		}

		private PsaiResult Readfile_ProtoBuf(Stream stream)
		{
			if (stream == null)
			{
				return PsaiResult.file_notFound;
			}
			this.m_soundtrack.Clear();
			this.m_themeQueue.Clear();
			return PsaiResult.error_file;
		}

		internal string getVersion()
		{
			return "psai Version .NET 1.7.3";
		}

		internal long GetCurrentSystemTimeMillis()
		{
			return (long)Logik.GetTimestampMillisElapsedSinceInitialisation();
		}

		private void startFade(int voiceId, int fadeoutMillis, int timeOffsetMillis)
		{
			if (voiceId > -1)
			{
				float fadeOutVolume = this.m_playbackChannels[voiceId].FadeOutVolume;
				for (int i = 0; i < this.m_fadeVoices.Count; i++)
				{
					FadeData fadeData = this.m_fadeVoices[i];
					if (fadeData.voiceNumber == voiceId)
					{
						fadeData.delayMillis = 0;
						fadeData.fadeoutDeltaVolumePerUpdate = fadeOutVolume / ((float)fadeoutMillis / 50f);
						fadeData.currentVolume = fadeOutVolume;
						return;
					}
				}
				if (fadeOutVolume > 0f)
				{
					this.AddFadeData(voiceId, fadeoutMillis, fadeOutVolume, timeOffsetMillis);
					if (!this.m_timerFades.IsSet())
					{
						this.m_timeStampOfLastFadeUpdate = Logik.GetTimestampMillisElapsedSinceInitialisation();
						this.m_timerFades.SetTimer(50, 0);
					}
				}
			}
		}

		private void AddFadeData(int voiceNumber, int fadeoutMillis, float currentVolume, int delayMillis)
		{
			FadeData fadeData = new FadeData();
			fadeData.voiceNumber = voiceNumber;
			fadeData.fadeoutDeltaVolumePerUpdate = currentVolume / ((float)fadeoutMillis / 50f);
			fadeData.currentVolume = currentVolume;
			fadeData.delayMillis = delayMillis;
			this.m_fadeVoices.Add(fadeData);
		}

		internal int getNextVoiceNumber(bool forHighlight)
		{
			int num;
			if (!forHighlight)
			{
				num = this.m_lastRegularVoiceNumberReturned + 1;
				if (num >= 7)
				{
					num = 0;
				}
				this.m_lastRegularVoiceNumberReturned = num;
			}
			else
			{
				num = this.m_hilightVoiceIndex + 1;
				if (num == 0 || num == 9)
				{
					num = 7;
				}
			}
			return num;
		}

		private void PsaiErrorCheck(PsaiResult result, string infoAboutLastCall)
		{
		}

		private int GetMillisElapsedAfterCurrentSnippetPlaycall()
		{
			if (this.m_currentSegmentPlaying == null)
			{
				return 0;
			}
			if (!this.m_paused)
			{
				return Logik.GetTimestampMillisElapsedSinceInitialisation() - this.m_timeStampCurrentSnippetPlaycall;
			}
			return this.m_timeStampPauseOn - this.m_timeStampCurrentSnippetPlaycall;
		}

		internal PsaiResult setPaused(bool setPaused)
		{
			if ((setPaused && !this.m_paused) || (!setPaused && this.m_paused))
			{
				this.m_paused = setPaused;
				PlaybackChannel[] playbackChannels = this.m_playbackChannels;
				for (int i = 0; i < playbackChannels.Length; i++)
				{
					playbackChannels[i].Paused = setPaused;
				}
				this.m_timerStartSnippetPlayback.SetPaused(setPaused);
				this.m_timerSegmentEndApproaching.SetPaused(setPaused);
				this.m_timerSegmentEndReached.SetPaused(setPaused);
				this.m_timerWakeUpFromRest.SetPaused(setPaused);
				if (setPaused)
				{
					this.m_timeStampPauseOn = Logik.GetTimestampMillisElapsedSinceInitialisation();
					this.m_lastIntensity = this.getCurrentIntensity();
				}
				else
				{
					int num = Logik.GetTimestampMillisElapsedSinceInitialisation() - this.m_timeStampPauseOn;
					int num2 = this.m_timeStampPauseOn - this.m_timeStampCurrentSnippetPlaycall;
					this.m_timeStampCurrentSnippetPlaycall = Logik.GetTimestampMillisElapsedSinceInitialisation() - num2;
					this.m_timeStampOfLastIntensitySetForCurrentTheme += num;
					this.m_estimatedTimestampOfTargetSnippetPlayback += num;
				}
				return PsaiResult.OK;
			}
			return PsaiResult.commandIgnored;
		}

		internal PsaiResult Update()
		{
			if (!this.m_paused)
			{
				if (this.m_timerStartSnippetPlayback.ThresholdHasBeenReached())
				{
					this.m_timerStartSnippetPlayback.Stop();
					this.PlayTargetSegmentImmediately();
				}
				if (this.m_timerSegmentEndApproaching.ThresholdHasBeenReached())
				{
					this.m_timerSegmentEndApproaching.Stop();
					this.SegmentEndApproachingHandler();
				}
				if (this.m_timerSegmentEndReached.ThresholdHasBeenReached())
				{
					this.m_timerSegmentEndReached.Stop();
					this.SegmentEndReachedHandler();
				}
				if (this.m_timerWakeUpFromRest.ThresholdHasBeenReached())
				{
					this.m_timerWakeUpFromRest.Stop();
					this.WakeUpFromRestHandler();
				}
				if (this.m_timerFades.ThresholdHasBeenReached())
				{
					this.m_timerFades.Stop();
					this.updateFades();
				}
			}
			return PsaiResult.OK;
		}

		private void SetThemeAsLastBasicMood(Theme latestBasicMood)
		{
			if (latestBasicMood != null)
			{
				this.m_lastBasicMood = latestBasicMood;
			}
		}

		private bool CheckIfAnyThemeIsCurrentlyPlaying()
		{
			return this.m_psaiState == PsaiState.playing && this.m_currentSegmentPlaying != null && this.m_effectiveTheme != null;
		}

		internal PsaiResult ReturnToLastBasicMood(bool immediately)
		{
			if (this.m_initializationFailure)
			{
				return PsaiResult.initialization_error;
			}
			if (this.m_lastBasicMood == null)
			{
				return PsaiResult.no_basicmood_set;
			}
			if (this.m_paused)
			{
				this.setPaused(false);
			}
			if (this.m_psaiPlayModeIntended == PsaiPlayMode.regular)
			{
				switch (this.m_psaiState)
				{
				case PsaiState.silence:
				case PsaiState.rest:
					this.PlayThemeNowOrAtEndOfCurrentSegmentAndStartEvaluation(this.GetLastBasicMoodId(), this.m_lastBasicMood.intensityAfterRest, this.m_lastBasicMood.musicDurationGeneral, true, false);
					return PsaiResult.OK;
				case PsaiState.playing:
					this.m_themeQueue.Clear();
					this.m_holdIntensity = false;
					this.m_latestEndOfSegmentTriggerCall.themeId = -1;
					if (this.m_currentSegmentPlaying != null && this.m_effectiveTheme.themeType != ThemeType.basicMood)
					{
						bool flag = false;
						if (!immediately)
						{
							flag = this.CheckIfThereIsAPathToEndSegmentForEffectiveSegmentAndLogWarningIfThereIsnt();
						}
						if (immediately || !flag)
						{
							this.PlayThemeNowOrAtEndOfCurrentSegmentAndStartEvaluation(this.GetLastBasicMoodId(), this.m_lastBasicMood.intensityAfterRest, this.m_lastBasicMood.musicDurationGeneral, true, false);
						}
						else
						{
							this.m_psaiStateIntended = PsaiState.playing;
							this.m_returnToLastBasicMoodFlag = true;
						}
						return PsaiResult.OK;
					}
					return PsaiResult.commandIgnored;
				default:
					return PsaiResult.internal_error;
				}
			}
			else
			{
				if (this.m_psaiPlayModeIntended == PsaiPlayMode.menuMode)
				{
					return PsaiResult.commandIgnoredMenuModeActive;
				}
				if (this.m_psaiPlayModeIntended == PsaiPlayMode.cutScene)
				{
					return PsaiResult.commandIgnoredCutsceneActive;
				}
				return PsaiResult.internal_error;
			}
		}

		internal int getUpcomingThemeId()
		{
			PsaiState psaiState = this.m_psaiState;
			if (psaiState == PsaiState.playing)
			{
				if (this.m_latestEndOfSegmentTriggerCall.themeId != -1)
				{
					return this.m_latestEndOfSegmentTriggerCall.themeId;
				}
				ThemeQueueEntry followingThemeQueueEntry = this.getFollowingThemeQueueEntry();
				if (followingThemeQueueEntry != null)
				{
					return followingThemeQueueEntry.themeId;
				}
			}
			return -1;
		}

		internal PsaiResult TriggerMusicTheme(int argThemeId, float argIntensity)
		{
			Theme themeById = this.m_soundtrack.getThemeById(argThemeId);
			if (themeById == null)
			{
				return PsaiResult.unknown_theme;
			}
			if (themeById.m_segments.Count == 0)
			{
				return PsaiResult.essential_segment_missing;
			}
			return this.TriggerMusicTheme(themeById, argIntensity, themeById.musicDurationGeneral);
		}

		internal PsaiResult TriggerMusicTheme(int argThemeId, float argIntensity, int argMusicDuration)
		{
			Theme themeById = this.m_soundtrack.getThemeById(argThemeId);
			if (themeById == null)
			{
				return PsaiResult.unknown_theme;
			}
			return this.TriggerMusicTheme(themeById, argIntensity, argMusicDuration);
		}

		internal PsaiResult TriggerMusicTheme(Theme argTheme, float argIntensity, int argMusicDuration)
		{
			if (this.m_initializationFailure)
			{
				return PsaiResult.initialization_error;
			}
			if (this.m_paused)
			{
				this.setPaused(false);
			}
			if (argIntensity > 1f)
			{
				argIntensity = 1f;
			}
			else if (argIntensity < 0f)
			{
				argIntensity = 0f;
			}
			if (this.m_psaiPlayMode == PsaiPlayMode.menuMode)
			{
				return PsaiResult.commandIgnoredMenuModeActive;
			}
			if (this.m_psaiPlayModeIntended == PsaiPlayMode.cutScene)
			{
				return PsaiResult.commandIgnoredCutsceneActive;
			}
			if (this.m_psaiPlayMode == PsaiPlayMode.cutScene && this.m_psaiStateIntended == PsaiState.silence && this.m_currentSegmentPlaying != null)
			{
				this.m_psaiState = PsaiState.playing;
				this.m_psaiStateIntended = PsaiState.playing;
				return this.PlayThemeNowOrAtEndOfCurrentSegmentAndStartEvaluation(argTheme.id, argIntensity, argMusicDuration, true, false);
			}
			Segment effectiveSegment = this.GetEffectiveSegment();
			if (argTheme.themeType == ThemeType.highlightLayer)
			{
				if (effectiveSegment != null && this.m_effectiveTheme != null && effectiveSegment != null && !effectiveSegment.CheckIfAtLeastOneDirectTransitionOrLayeringIsPossible(this.m_soundtrack, argTheme.id))
				{
					return PsaiResult.triggerDenied;
				}
				return this.startHighlight(argTheme);
			}
			else
			{
				if (this.m_returnToLastBasicMoodFlag && argTheme.themeType != ThemeType.basicMood)
				{
					this.m_returnToLastBasicMoodFlag = false;
				}
				if (argTheme.themeType == ThemeType.basicMood)
				{
					this.SetThemeAsLastBasicMood(argTheme);
				}
				if (effectiveSegment == null || this.m_psaiState == PsaiState.silence || this.m_psaiState == PsaiState.rest)
				{
					return this.PlayThemeNowOrAtEndOfCurrentSegmentAndStartEvaluation(argTheme.id, argIntensity, argMusicDuration, true, false);
				}
				Theme themeById = this.m_soundtrack.getThemeById(effectiveSegment.ThemeId);
				if (this.m_psaiStateIntended == PsaiState.silence && effectiveSegment != null)
				{
					ThemeInterruptionBehavior themeInterruptionBehavior = Theme.GetThemeInterruptionBehavior(themeById.themeType, argTheme.themeType);
					if (themeInterruptionBehavior == ThemeInterruptionBehavior.at_end_of_current_snippet || themeInterruptionBehavior == ThemeInterruptionBehavior.never)
					{
						this.m_psaiStateIntended = PsaiState.playing;
						return this.PlayThemeNowOrAtEndOfCurrentSegmentAndStartEvaluation(argTheme.id, argIntensity, argMusicDuration, false, false);
					}
				}
				if (effectiveSegment.ThemeId == argTheme.id)
				{
					this.m_latestEndOfSegmentTriggerCall.themeId = -1;
					this.SetCurrentIntensityAndMusicDuration(argIntensity, argMusicDuration, true);
					this.m_psaiStateIntended = PsaiState.playing;
					return PsaiResult.OK;
				}
				switch (argTheme.themeType)
				{
				case ThemeType.basicMood:
					switch (themeById.themeType)
					{
					case ThemeType.basicMood:
						return this.PlayThemeAtEndOfCurrentSegment(argTheme, argIntensity, argMusicDuration);
					case ThemeType.basicMoodAlt:
						return PsaiResult.OK;
					case ThemeType.action:
						return PsaiResult.OK;
					case ThemeType.shock:
						return PsaiResult.OK;
					case ThemeType.dramaticEvent:
						return PsaiResult.OK;
					}
					break;
				case ThemeType.basicMoodAlt:
					switch (themeById.themeType)
					{
					case ThemeType.basicMood:
						return this.PlayThemeAtEndOfCurrentSegment(argTheme, argIntensity, argMusicDuration);
					case ThemeType.basicMoodAlt:
						return this.PlayThemeAtEndOfCurrentSegment(argTheme, argIntensity, argMusicDuration);
					case ThemeType.action:
						return PsaiResult.triggerIgnoredLowPriority;
					case ThemeType.shock:
						if (this.getThemeTypeOfFirstThemeQueueEntry() == ThemeType.action)
						{
							return PsaiResult.triggerIgnoredLowPriority;
						}
						return this.PlayThemeAtEndOfCurrentSegment(argTheme, argIntensity, argMusicDuration);
					case ThemeType.dramaticEvent:
						return this.PlayThemeAtEndOfCurrentSegment(argTheme, argIntensity, argMusicDuration);
					}
					break;
				case ThemeType.action:
					switch (themeById.themeType)
					{
					case ThemeType.basicMood:
						this.ClearLatestEndOfSegmentTriggerCall();
						return this.PlayThemeNowOrAtEndOfCurrentSegmentAndStartEvaluation(argTheme.id, argIntensity, argMusicDuration, true, false);
					case ThemeType.basicMoodAlt:
						this.ClearLatestEndOfSegmentTriggerCall();
						return this.PlayThemeNowOrAtEndOfCurrentSegmentAndStartEvaluation(argTheme.id, argIntensity, argMusicDuration, true, false);
					case ThemeType.action:
						return this.PlayThemeAtEndOfCurrentSegment(argTheme, argIntensity, argMusicDuration);
					case ThemeType.shock:
						return this.PlayThemeAtEndOfCurrentTheme(argTheme, argIntensity, argMusicDuration);
					case ThemeType.dramaticEvent:
						this.ClearLatestEndOfSegmentTriggerCall();
						return this.PlayThemeNowOrAtEndOfCurrentSegmentAndStartEvaluation(argTheme.id, argIntensity, argMusicDuration, true, false);
					}
					break;
				case ThemeType.shock:
					switch (themeById.themeType)
					{
					case ThemeType.basicMood:
						return this.PlayThemeNowOrAtEndOfCurrentSegmentAndStartEvaluation(argTheme.id, argIntensity, argMusicDuration, true, false);
					case ThemeType.basicMoodAlt:
						return this.PlayThemeNowOrAtEndOfCurrentSegmentAndStartEvaluation(argTheme.id, argIntensity, argMusicDuration, true, false);
					case ThemeType.action:
						this.ClearQueuedTheme();
						this.PushEffectiveThemeToThemeQueue(PsaiPlayMode.regular);
						return this.PlayThemeNowOrAtEndOfCurrentSegmentAndStartEvaluation(argTheme.id, argIntensity, argMusicDuration, true, false);
					case ThemeType.shock:
						return this.PlayThemeAtEndOfCurrentSegment(argTheme, argIntensity, argMusicDuration);
					case ThemeType.dramaticEvent:
						return this.PlayThemeNowOrAtEndOfCurrentSegmentAndStartEvaluation(argTheme.id, argIntensity, argMusicDuration, true, false);
					}
					break;
				case ThemeType.dramaticEvent:
					switch (themeById.themeType)
					{
					case ThemeType.basicMood:
						return this.PlayThemeNowOrAtEndOfCurrentSegmentAndStartEvaluation(argTheme.id, argIntensity, argMusicDuration, true, false);
					case ThemeType.basicMoodAlt:
						return this.PlayThemeNowOrAtEndOfCurrentSegmentAndStartEvaluation(argTheme.id, argIntensity, argMusicDuration, true, false);
					case ThemeType.action:
						return PsaiResult.triggerIgnoredLowPriority;
					case ThemeType.shock:
						if (this.getThemeTypeOfFirstThemeQueueEntry() == ThemeType.action)
						{
							return PsaiResult.triggerIgnoredLowPriority;
						}
						return this.PlayThemeAtEndOfCurrentTheme(argTheme, argIntensity, argMusicDuration);
					case ThemeType.dramaticEvent:
						return this.PlayThemeAtEndOfCurrentSegment(argTheme, argIntensity, argMusicDuration);
					}
					break;
				}
				return PsaiResult.internal_error;
			}
		}

		internal static float ClampPercentValue(float argValue)
		{
			if (argValue > 1f)
			{
				return 1f;
			}
			if (argValue < 0f)
			{
				return 0f;
			}
			return argValue;
		}

		internal PsaiResult AddToCurrentIntensity(float deltaIntensity, bool resetIntensityFalloffToFullMusicDuration)
		{
			if (this.m_psaiState == PsaiState.playing && this.m_psaiPlayMode == PsaiPlayMode.regular)
			{
				if (this.m_latestEndOfSegmentTriggerCall.themeId != -1)
				{
					this.m_latestEndOfSegmentTriggerCall.startIntensity = Logik.ClampPercentValue(this.m_latestEndOfSegmentTriggerCall.startIntensity + deltaIntensity);
				}
				else
				{
					float num = Logik.ClampPercentValue(this.getCurrentIntensity() + deltaIntensity);
					this.SetCurrentIntensityAndMusicDuration(num, this.m_lastMusicDuration, resetIntensityFalloffToFullMusicDuration);
				}
				return PsaiResult.OK;
			}
			return PsaiResult.notReady;
		}

		internal PsaiResult PlaySegmentLayeredAndImmediately(int segmentId)
		{
			Segment segmentById = this.m_soundtrack.GetSegmentById(segmentId);
			if (segmentById != null)
			{
				this.PlaySegmentLayeredAndImmediately(segmentById);
			}
			return PsaiResult.invalidHandle;
		}

		internal void PlaySegmentLayeredAndImmediately(Segment segment)
		{
			this.m_hilightVoiceIndex = this.getNextVoiceNumber(true);
			this.m_playbackChannels[this.m_hilightVoiceIndex].StopChannel();
			this.m_playbackChannels[this.m_hilightVoiceIndex].ReleaseSegment();
			this.m_playbackChannels[this.m_hilightVoiceIndex].FadeOutVolume = 1f;
			this.m_playbackChannels[this.m_hilightVoiceIndex].ScheduleSegmentPlayback(segment, Logik.s_audioLayerMaximumLatencyForBufferingSounds + Logik.s_audioLayerMaximumLatencyForPlayingbackPrebufferedSounds);
		}

		private PsaiResult startHighlight(Theme highlightTheme)
		{
			if (highlightTheme.m_segments.Count <= 0)
			{
				return PsaiResult.essential_segment_missing;
			}
			Segment segment;
			if (this.m_currentSegmentPlaying != null)
			{
				segment = this.GetBestCompatibleSegment(this.m_currentSegmentPlaying, highlightTheme.id, this.getCurrentIntensity(), 15);
			}
			else
			{
				int randomInt = Logik.GetRandomInt(0, highlightTheme.m_segments.Count);
				segment = this.m_soundtrack.GetSegmentById(highlightTheme.m_segments[randomInt].Id);
			}
			if (segment != null)
			{
				this.PlaySegmentLayeredAndImmediately(segment);
				Segment segment2 = segment;
				int playcount = segment2.Playcount;
				segment2.Playcount = playcount + 1;
				return PsaiResult.OK;
			}
			return PsaiResult.essential_segment_missing;
		}

		private void ClearLatestEndOfSegmentTriggerCall()
		{
			this.m_latestEndOfSegmentTriggerCall.themeId = -1;
		}

		private void ClearQueuedTheme()
		{
			this.m_themeQueue.Clear();
		}

		private bool pushThemeToThemeQueue(int themeId, float intensity, int musicDuration, bool clearThemeQueue, int restTimeMillis, PsaiPlayMode playMode, bool holdIntensity)
		{
			if (clearThemeQueue)
			{
				this.m_themeQueue.Clear();
			}
			if (this.m_soundtrack.getThemeById(themeId) != null)
			{
				ThemeQueueEntry themeQueueEntry = new ThemeQueueEntry();
				themeQueueEntry.themeId = themeId;
				themeQueueEntry.startIntensity = intensity;
				themeQueueEntry.musicDuration = musicDuration;
				themeQueueEntry.restTimeMillis = restTimeMillis;
				themeQueueEntry.playmode = playMode;
				themeQueueEntry.holdIntensity = holdIntensity;
				this.m_themeQueue.Insert(0, themeQueueEntry);
				this.m_psaiStateIntended = PsaiState.playing;
				return true;
			}
			return false;
		}

		private ThemeType getThemeTypeOfFirstThemeQueueEntry()
		{
			ThemeQueueEntry followingThemeQueueEntry = this.getFollowingThemeQueueEntry();
			if (followingThemeQueueEntry != null)
			{
				Theme themeById = this.m_soundtrack.getThemeById(followingThemeQueueEntry.themeId);
				if (themeById != null)
				{
					return themeById.themeType;
				}
			}
			return ThemeType.none;
		}

		internal float getUpcomingIntensity()
		{
			if (this.m_psaiState == PsaiState.playing && this.m_latestEndOfSegmentTriggerCall.themeId != -1)
			{
				return this.m_latestEndOfSegmentTriggerCall.startIntensity;
			}
			return this.getCurrentIntensity();
		}

		internal float getCurrentIntensity()
		{
			if (this.m_paused)
			{
				return this.m_lastIntensity;
			}
			if (this.m_psaiState != PsaiState.playing || this.m_psaiStateIntended != PsaiState.playing || this.m_returnToLastBasicMoodFlag)
			{
				return 0f;
			}
			if (this.m_holdIntensity)
			{
				return this.m_heldIntensity;
			}
			float num;
			if (this.m_effectiveTheme == null)
			{
				num = 0f;
			}
			else
			{
				if (this.m_targetSegment != null && (this.m_currentSegmentPlaying == null || this.m_currentSegmentPlaying.ThemeId != this.m_targetSegment.ThemeId))
				{
					return this.m_targetSegment.Intensity;
				}
				int num2 = Logik.GetTimestampMillisElapsedSinceInitialisation() - this.m_timeStampOfLastIntensitySetForCurrentTheme;
				num = this.m_startOrRetriggerIntensityOfCurrentTheme - (float)num2 * this.m_currentIntensitySlope / 1000f;
				if (num < 0f)
				{
					num = 0f;
				}
			}
			this.m_lastIntensity = num;
			return num;
		}

		private PsaiResult PlaySegment(Segment targetSnippet, bool immediately)
		{
			if (this.m_initializationFailure)
			{
				return PsaiResult.initialization_error;
			}
			this.m_timerSegmentEndApproaching.Stop();
			this.m_timerStartSnippetPlayback.Stop();
			this.m_targetVoice = this.getNextVoiceNumber(false);
			PsaiResult psaiResult = this.LoadSegment(targetSnippet, this.m_targetVoice);
			this.PsaiErrorCheck(psaiResult, "LoadSegment()");
			if (psaiResult != PsaiResult.OK)
			{
				return psaiResult;
			}
			this.m_targetSegment = targetSnippet;
			if (immediately || this.m_currentSegmentPlaying == null)
			{
				if (this.m_playbackChannels[this.m_targetVoice].CheckIfSegmentHadEnoughTimeToLoad())
				{
					this.m_estimatedTimestampOfTargetSnippetPlayback = Logik.GetTimestampMillisElapsedSinceInitialisation() + Logik.s_audioLayerMaximumLatencyForPlayingbackPrebufferedSounds;
				}
				else
				{
					this.m_estimatedTimestampOfTargetSnippetPlayback = Logik.GetTimestampMillisElapsedSinceInitialisation() + Logik.s_audioLayerMaximumLatencyForPlayingBackUnbufferedSounds;
				}
				this.PlayTargetSegmentImmediately();
			}
			else
			{
				int millisElapsedAfterCurrentSnippetPlaycall = this.GetMillisElapsedAfterCurrentSnippetPlaycall();
				int num = this.m_currentSegmentPlaying.audioData.GetFullLengthInMilliseconds() - this.m_currentSegmentPlaying.audioData.GetPostBeatZoneInMilliseconds() - targetSnippet.audioData.GetPreBeatZoneInMilliseconds() - millisElapsedAfterCurrentSnippetPlaycall;
				if (num > Logik.s_audioLayerMaximumLatencyForPlayingBackUnbufferedSounds)
				{
					this.m_estimatedTimestampOfTargetSnippetPlayback = Logik.GetTimestampMillisElapsedSinceInitialisation() + num;
					this.m_timerStartSnippetPlayback.SetTimer(num, Logik.s_audioLayerMaximumLatencyForPlayingbackPrebufferedSounds);
				}
				else
				{
					this.m_estimatedTimestampOfTargetSnippetPlayback = Logik.GetTimestampMillisElapsedSinceInitialisation() + Logik.s_audioLayerMaximumLatencyForPlayingbackPrebufferedSounds;
					this.PlayTargetSegmentImmediately();
				}
			}
			return PsaiResult.OK;
		}

		private PsaiResult LoadSegment(Segment snippet, int channelIndex)
		{
			if (snippet == null || channelIndex >= 9)
			{
				return PsaiResult.invalidHandle;
			}
			this.m_playbackChannels[channelIndex].LoadSegment(snippet);
			return PsaiResult.OK;
		}

		private void PlayTargetSegmentImmediately()
		{
			int num;
			if (this.m_playbackChannels[this.m_targetVoice].CheckIfSegmentHadEnoughTimeToLoad())
			{
				num = this.m_estimatedTimestampOfTargetSnippetPlayback - Logik.GetTimestampMillisElapsedSinceInitialisation();
			}
			else
			{
				num = this.m_playbackChannels[this.m_targetVoice].GetMillisecondsUntilLoadingWillHaveFinished() + Logik.s_audioLayerMaximumLatencyForPlayingbackPrebufferedSounds;
			}
			this.m_playbackChannels[this.m_targetVoice].FadeOutVolume = 1f;
			this.m_playbackChannels[this.m_targetVoice].ScheduleSegmentPlayback(this.m_targetSegment, num);
			if (this.m_scheduleFadeoutUponSnippetPlayback)
			{
				this.startFade(this.m_currentVoiceNumber, 500, this.m_targetSegment.audioData.GetPreBeatZoneInMilliseconds() + num);
				this.m_scheduleFadeoutUponSnippetPlayback = false;
			}
			this.m_psaiPlayMode = this.m_psaiPlayModeIntended;
			this.m_currentVoiceNumber = this.m_targetVoice;
			this.m_currentSegmentPlaying = this.m_targetSegment;
			this.m_currentSnippetTypeRequested = this.m_targetSegmentSuitabilitiesRequested;
			Segment currentSegmentPlaying = this.m_currentSegmentPlaying;
			int playcount = currentSegmentPlaying.Playcount;
			currentSegmentPlaying.Playcount = playcount + 1;
			this.SetSegmentEndApproachingAndReachedTimersAfterPlaybackHasStarted(num);
			this.m_targetSegment = null;
			this.m_psaiState = PsaiState.playing;
		}

		internal void SetSegmentEndApproachingAndReachedTimersAfterPlaybackHasStarted(int snippetPlaybackDelayMillis)
		{
			this.m_timeStampCurrentSnippetPlaycall = Logik.GetTimestampMillisElapsedSinceInitialisation() + snippetPlaybackDelayMillis;
			int num = this.m_currentSegmentPlaying.audioData.GetFullLengthInMilliseconds() + snippetPlaybackDelayMillis;
			int num2 = num - this.m_currentSegmentPlaying.audioData.GetPostBeatZoneInMilliseconds() - this.m_currentSegmentPlaying.MaxPreBeatMsOfCompatibleSnippetsWithinSameTheme - Logik.s_audioLayerMaximumLatencyForPlayingBackUnbufferedSounds - 2 * Logik.s_updateIntervalMillis;
			if (num2 < 0)
			{
				num2 = 0;
			}
			this.m_timerSegmentEndApproaching.SetTimer(num2, Logik.s_updateIntervalMillis);
			this.m_timerSegmentEndReached.SetTimer(num, 0);
		}

		internal float getVolume()
		{
			return this.m_psaiMasterVolume;
		}

		internal void setVolume(float vol)
		{
			this.m_psaiMasterVolume = vol;
			if ((double)vol > 1.0)
			{
				this.m_psaiMasterVolume = 1f;
			}
			else if (vol < 0f)
			{
				this.m_psaiMasterVolume = 0f;
			}
			for (int i = 0; i < this.m_playbackChannels.Length; i++)
			{
				this.m_playbackChannels[i].MasterVolume = this.m_psaiMasterVolume;
			}
		}

		private PsaiResult PlayThemeNowOrAtEndOfCurrentSegmentAndStartEvaluation(ThemeQueueEntry tqe, bool immediately)
		{
			return this.PlayThemeNowOrAtEndOfCurrentSegmentAndStartEvaluation(tqe.themeId, tqe.startIntensity, tqe.musicDuration, immediately, tqe.holdIntensity);
		}

		private PsaiResult PlayThemeNowOrAtEndOfCurrentSegmentAndStartEvaluation(int themeId, float intensity, int musicDuration, bool immediately, bool holdIntensity)
		{
			this.SetCurrentIntensityAndMusicDuration(intensity, musicDuration, true);
			this.m_psaiStateIntended = PsaiState.playing;
			this.m_heldIntensity = intensity;
			if (this.m_psaiState == PsaiState.rest)
			{
				this.m_timerWakeUpFromRest.Stop();
			}
			this.m_targetSegmentSuitabilitiesRequested = 1;
			if (this.m_psaiState == PsaiState.playing && this.m_currentSegmentPlaying != null)
			{
				if (this.m_currentSegmentPlaying.IsUsableOnlyAs(SegmentSuitability.end))
				{
					this.m_targetSegmentSuitabilitiesRequested = 1;
				}
				else if (this.getEffectiveThemeId() == themeId)
				{
					this.m_targetSegmentSuitabilitiesRequested = 2;
				}
				else
				{
					this.m_targetSegmentSuitabilitiesRequested = 10;
				}
			}
			this.m_effectiveTheme = this.m_soundtrack.getThemeById(themeId);
			Segment segment;
			if ((this.m_targetSegmentSuitabilitiesRequested & 1) > 0 || this.GetEffectiveSegment() == null)
			{
				segment = this.GetBestStartSegmentForTheme(themeId, intensity);
			}
			else
			{
				segment = this.GetBestCompatibleSegment(this.GetEffectiveSegment(), themeId, intensity, this.m_targetSegmentSuitabilitiesRequested);
			}
			if (segment == null)
			{
				segment = this.substituteSegment(themeId);
				if (segment == null)
				{
					this.StopMusic(true);
					return PsaiResult.essential_segment_missing;
				}
			}
			this.m_holdIntensity = holdIntensity;
			if (immediately && this.GetEffectiveSegment() != null)
			{
				this.m_scheduleFadeoutUponSnippetPlayback = true;
			}
			if (segment != null)
			{
				return this.PlaySegment(segment, immediately);
			}
			return PsaiResult.internal_error;
		}

		internal PsaiResult StopMusic(bool immediately)
		{
			return this.StopMusic(immediately, 1000);
		}

		internal PsaiResult StopMusic(bool immediately, int fadeOutMilliSeconds)
		{
			if (immediately && fadeOutMilliSeconds <= 0)
			{
				fadeOutMilliSeconds = 1000;
			}
			if (this.m_paused)
			{
				this.setPaused(false);
			}
			this.ClearLatestEndOfSegmentTriggerCall();
			this.ClearQueuedTheme();
			if (this.m_psaiPlayMode == PsaiPlayMode.menuMode)
			{
				return PsaiResult.commandIgnoredMenuModeActive;
			}
			if (this.m_psaiPlayModeIntended == PsaiPlayMode.cutScene)
			{
				return PsaiResult.commandIgnoredCutsceneActive;
			}
			if (this.m_initializationFailure)
			{
				return PsaiResult.initialization_error;
			}
			if (this.m_psaiStateIntended == PsaiState.silence && !immediately)
			{
				return PsaiResult.commandIgnored;
			}
			this.m_returnToLastBasicMoodFlag = false;
			this.m_holdIntensity = false;
			PsaiState psaiState = this.m_psaiState;
			if (psaiState - PsaiState.silence > 1)
			{
				if (psaiState == PsaiState.rest)
				{
					this.EnterSilenceMode();
				}
			}
			else if (this.GetEffectiveSegment() != null)
			{
				bool flag = false;
				if (!immediately)
				{
					flag = this.CheckIfThereIsAPathToEndSegmentForEffectiveSegmentAndLogWarningIfThereIsnt();
				}
				if (immediately || !flag)
				{
					this.startFade(this.m_currentVoiceNumber, fadeOutMilliSeconds, 0);
					this.EnterSilenceMode();
				}
				else
				{
					if (this.m_latestEndOfSegmentTriggerCall.themeId != -1)
					{
						this.m_latestEndOfSegmentTriggerCall.themeId = -1;
					}
					this.m_psaiStateIntended = PsaiState.silence;
				}
			}
			return PsaiResult.OK;
		}

		private void WriteLogWarningIfThereIsNoDirectPathForEffectiveSnippetToEndSnippet()
		{
		}

		private bool CheckIfThereIsAPathToEndSegmentForEffectiveSegmentAndLogWarningIfThereIsnt()
		{
			Segment effectiveSegment = this.GetEffectiveSegment();
			return effectiveSegment.IsUsableAs(SegmentSuitability.end) || effectiveSegment.nextSnippetToShortestEndSequence != null;
		}

		private void updateFades()
		{
			bool flag = false;
			int timestampMillisElapsedSinceInitialisation = Logik.GetTimestampMillisElapsedSinceInitialisation();
			int num = timestampMillisElapsedSinceInitialisation - this.m_timeStampOfLastFadeUpdate;
			this.m_timeStampOfLastFadeUpdate = timestampMillisElapsedSinceInitialisation;
			int i = 0;
			while (i < this.m_fadeVoices.Count)
			{
				FadeData fadeData = this.m_fadeVoices[i];
				if (fadeData.delayMillis > 0)
				{
					fadeData.delayMillis -= num;
					if (fadeData.delayMillis <= 0)
					{
						fadeData.delayMillis = 0;
					}
					flag = true;
					i++;
				}
				else
				{
					float num2 = fadeData.currentVolume - fadeData.fadeoutDeltaVolumePerUpdate;
					if (num2 > 0f)
					{
						flag = true;
						fadeData.currentVolume = num2;
						this.m_playbackChannels[fadeData.voiceNumber].FadeOutVolume = num2;
						i++;
					}
					else
					{
						int voiceNumber = fadeData.voiceNumber;
						if (this.m_playbackChannels[voiceNumber].IsPlaying())
						{
							this.m_playbackChannels[voiceNumber].StopChannel();
							this.m_playbackChannels[voiceNumber].ReleaseSegment();
							this.m_fadeVoices.RemoveAt(i);
						}
						else
						{
							this.m_fadeVoices.RemoveAt(i);
						}
					}
				}
			}
			if (flag)
			{
				this.m_timerFades.SetTimer(50, 0);
			}
		}

		internal PsaiResult HoldCurrentIntensity(bool hold)
		{
			switch (this.m_psaiPlayModeIntended)
			{
			case PsaiPlayMode.regular:
				if (hold && this.m_holdIntensity)
				{
					return PsaiResult.commandIgnored;
				}
				if (!hold && !this.m_holdIntensity)
				{
					return PsaiResult.commandIgnored;
				}
				if (hold)
				{
					this.m_remainingMusicDurationAtTimeOfHoldIntensity = this.GetRemainingMusicDurationSecondsOfCurrentTheme();
					this.m_heldIntensity = this.getCurrentIntensity();
					this.m_holdIntensity = true;
				}
				else
				{
					this.SetCurrentIntensityAndMusicDuration(this.m_heldIntensity, this.m_remainingMusicDurationAtTimeOfHoldIntensity, false);
					this.m_holdIntensity = false;
				}
				return PsaiResult.OK;
			case PsaiPlayMode.menuMode:
				return PsaiResult.commandIgnoredMenuModeActive;
			case PsaiPlayMode.cutScene:
				return PsaiResult.commandIgnoredCutsceneActive;
			default:
				return PsaiResult.internal_error;
			}
		}

		private void SetCurrentIntensityAndMusicDuration(float intensity, int musicDuration, bool recalculateIntensitySlope)
		{
			this.m_timeStampOfLastIntensitySetForCurrentTheme = Logik.GetTimestampMillisElapsedSinceInitialisation();
			this.m_lastMusicDuration = musicDuration;
			if (recalculateIntensitySlope)
			{
				this.m_currentIntensitySlope = intensity / (float)musicDuration;
			}
			this.m_startOrRetriggerIntensityOfCurrentTheme = intensity;
		}

		private void SegmentEndApproachingHandler()
		{
			if (this.m_latestEndOfSegmentTriggerCall.themeId != -1)
			{
				this.m_psaiState = PsaiState.playing;
				this.m_psaiStateIntended = PsaiState.playing;
			}
			PsaiState psaiStateIntended = this.m_psaiStateIntended;
			if (psaiStateIntended != PsaiState.silence)
			{
				if (psaiStateIntended != PsaiState.rest)
				{
					if (this.m_returnToLastBasicMoodFlag)
					{
						if ((this.m_currentSegmentPlaying.SnippetTypeBitfield & 4) == 0 && this.CheckIfThereIsAPathToEndSegmentForEffectiveSegmentAndLogWarningIfThereIsnt())
						{
							this.WriteLogWarningIfThereIsNoDirectPathForEffectiveSnippetToEndSnippet();
							this.PlaySegment(this.m_currentSegmentPlaying.nextSnippetToShortestEndSequence, false);
							return;
						}
						this.PlayThemeNowOrAtEndOfCurrentSegmentAndStartEvaluation(this.GetLastBasicMoodId(), this.m_lastBasicMood.intensityAfterRest, this.m_lastBasicMood.musicDurationGeneral, false, false);
						this.m_returnToLastBasicMoodFlag = false;
						return;
					}
					else if (this.m_psaiPlayMode == PsaiPlayMode.regular && this.m_latestEndOfSegmentTriggerCall.themeId != -1)
					{
						Theme themeById = this.m_soundtrack.getThemeById(this.m_latestEndOfSegmentTriggerCall.themeId);
						if (this.m_currentSegmentPlaying.CheckIfAtLeastOneDirectTransitionOrLayeringIsPossible(this.m_soundtrack, themeById.id))
						{
							this.PlayThemeNowOrAtEndOfCurrentSegmentAndStartEvaluation(this.m_latestEndOfSegmentTriggerCall.themeId, this.m_latestEndOfSegmentTriggerCall.startIntensity, this.m_latestEndOfSegmentTriggerCall.musicDuration, false, this.m_latestEndOfSegmentTriggerCall.holdIntensity);
							this.m_latestEndOfSegmentTriggerCall.themeId = -1;
							return;
						}
						Segment segment;
						if (this.m_currentSegmentPlaying.MapOfNextTransitionSegmentToTheme.TryGetValue(themeById.id, out segment))
						{
							this.PlaySegment(segment, false);
							return;
						}
						this.PlayThemeNowOrAtEndOfCurrentSegmentAndStartEvaluation(this.m_latestEndOfSegmentTriggerCall.themeId, this.m_latestEndOfSegmentTriggerCall.startIntensity, this.m_latestEndOfSegmentTriggerCall.musicDuration, true, this.m_latestEndOfSegmentTriggerCall.holdIntensity);
						this.m_latestEndOfSegmentTriggerCall.themeId = -1;
						return;
					}
					else
					{
						if (this.getCurrentIntensity() > 0f)
						{
							this.m_latestEndOfSegmentTriggerCall.themeId = -1;
							this.PlaySegmentOfCurrentTheme(SegmentSuitability.middle);
							return;
						}
						this.IntensityZeroHandler();
					}
				}
				else
				{
					if (this.m_currentSegmentPlaying != null && !this.m_currentSegmentPlaying.IsUsableAs(SegmentSuitability.end))
					{
						this.PlaySegment(this.m_currentSegmentPlaying.nextSnippetToShortestEndSequence, false);
						return;
					}
					if (this.m_psaiState != PsaiState.rest)
					{
						this.EnterRestMode(this.GetLastBasicMoodId(), this.getEffectiveThemeId());
						return;
					}
				}
				return;
			}
			if (this.m_currentSegmentPlaying == null || this.m_currentSegmentPlaying.IsUsableAs(SegmentSuitability.end))
			{
				this.EnterSilenceMode();
				return;
			}
			this.PlaySegment(this.m_currentSegmentPlaying.nextSnippetToShortestEndSequence, false);
		}

		private PsaiResult PlayThemeAtEndOfCurrentSegment(Theme argTheme, float intensity, int musicDuration)
		{
			this.m_latestEndOfSegmentTriggerCall.themeId = argTheme.id;
			this.m_latestEndOfSegmentTriggerCall.startIntensity = intensity;
			this.m_latestEndOfSegmentTriggerCall.musicDuration = musicDuration;
			this.m_psaiStateIntended = PsaiState.playing;
			return PsaiResult.OK;
		}

		private PsaiResult PlayThemeAtEndOfCurrentTheme(Theme argTheme, float argIntensity, int argMusicDuration)
		{
			this.ClearLatestEndOfSegmentTriggerCall();
			this.ClearQueuedTheme();
			this.pushThemeToThemeQueue(argTheme.id, argIntensity, argMusicDuration, false, 1, PsaiPlayMode.regular, false);
			return PsaiResult.OK;
		}

		private void SegmentEndReachedHandler()
		{
			if (this.m_targetSegment == null)
			{
				this.m_currentSegmentPlaying = null;
			}
		}

		private void InitiateTransitionToRestOrSilence(PsaiState psaiStateIntended)
		{
			if ((psaiStateIntended == PsaiState.rest || psaiStateIntended == PsaiState.silence) && this.m_currentSegmentPlaying != null)
			{
				if (this.m_currentSegmentPlaying.IsUsableAs(SegmentSuitability.end))
				{
					this.EnterRestMode(this.GetLastBasicMoodId(), this.getEffectiveThemeId());
					return;
				}
				if (!this.CheckIfThereIsAPathToEndSegmentForEffectiveSegmentAndLogWarningIfThereIsnt())
				{
					this.startFade(this.m_currentVoiceNumber, this.GetRemainingMillisecondsOfCurrentSegmentPlayback(), 0);
					this.EnterRestMode(this.GetLastBasicMoodId(), this.getEffectiveThemeId());
					return;
				}
				this.WriteLogWarningIfThereIsNoDirectPathForEffectiveSnippetToEndSnippet();
				this.PlaySegment(this.m_currentSegmentPlaying.nextSnippetToShortestEndSequence, false);
				this.m_psaiStateIntended = psaiStateIntended;
			}
		}

		private void InitiateTransitionToRestMode()
		{
			this.InitiateTransitionToRestOrSilence(PsaiState.rest);
		}

		private void IntensityZeroHandler()
		{
			if (this.m_currentSegmentPlaying != null)
			{
				switch (this.m_soundtrack.getThemeById(this.m_currentSegmentPlaying.ThemeId).themeType)
				{
				case ThemeType.basicMood:
					this.InitiateTransitionToRestMode();
					return;
				case ThemeType.basicMoodAlt:
				case ThemeType.dramaticEvent:
					if (this.m_lastBasicMood != null)
					{
						this.PlayThemeNowOrAtEndOfCurrentSegmentAndStartEvaluation(this.m_lastBasicMood.id, this.m_lastBasicMood.intensityAfterRest, this.m_lastBasicMood.musicDurationAfterRest, false, false);
						return;
					}
					this.InitiateTransitionToRestOrSilence(PsaiState.silence);
					return;
				case ThemeType.action:
					if (this.m_lastBasicMood == null)
					{
						this.InitiateTransitionToRestOrSilence(PsaiState.silence);
						return;
					}
					this.InitiateTransitionToRestMode();
					return;
				case (ThemeType)4:
				case ThemeType.highlightLayer:
					break;
				case ThemeType.shock:
				{
					ThemeQueueEntry followingThemeQueueEntry = this.getFollowingThemeQueueEntry();
					if (followingThemeQueueEntry != null)
					{
						if (this.m_currentSegmentPlaying.CheckIfAnyDirectOrIndirectTransitionIsPossible(this.m_soundtrack, followingThemeQueueEntry.themeId))
						{
							this.PopAndPlayNextFollowingTheme(false);
							return;
						}
						this.InitiateTransitionToRestMode();
						return;
					}
					else
					{
						this.InitiateTransitionToRestMode();
					}
					break;
				}
				default:
					return;
				}
			}
		}

		public PsaiResult GoToRest(bool immediately, int fadeOutMilliSeconds)
		{
			return this.GoToRest(immediately, fadeOutMilliSeconds, -1, -1);
		}

		public PsaiResult GoToRest(bool immediately, int fadeOutMilliSeconds, int restSecondsOverrideMin, int restSecondsOverrideMax)
		{
			if (restSecondsOverrideMin > restSecondsOverrideMax)
			{
				return PsaiResult.invalidParam;
			}
			if (fadeOutMilliSeconds < 0)
			{
				return PsaiResult.invalidParam;
			}
			if (restSecondsOverrideMin >= 0 && restSecondsOverrideMax >= 0)
			{
				this.m_restModeSecondsOverride = Logik.GetRandomInt(restSecondsOverrideMin, restSecondsOverrideMax);
			}
			else
			{
				this.m_restModeSecondsOverride = -1;
			}
			if (!immediately)
			{
				this.InitiateTransitionToRestMode();
			}
			else
			{
				this.startFade(this.m_currentVoiceNumber, fadeOutMilliSeconds, 0);
				this.EnterRestMode(this.GetLastBasicMoodId(), this.getEffectiveThemeId());
			}
			return PsaiResult.OK;
		}

		private void EnterRestMode(int themeIdToWakeUpWith, int themeIdToUseForRestingTimeCalculation)
		{
			this.m_psaiState = PsaiState.rest;
			this.m_holdIntensity = false;
			this.m_timerStartSnippetPlayback.Stop();
			this.m_timerSegmentEndApproaching.Stop();
			this.m_timerWakeUpFromRest.Stop();
			this.m_effectiveTheme = this.m_soundtrack.getThemeById(themeIdToWakeUpWith);
			if (this.m_effectiveTheme != null)
			{
				int num;
				if (this.m_restModeSecondsOverride > 0)
				{
					num = this.m_restModeSecondsOverride;
					this.m_restModeSecondsOverride = -1;
				}
				else
				{
					Theme themeById = this.m_soundtrack.getThemeById(themeIdToUseForRestingTimeCalculation);
					if (themeById != null)
					{
						num = Logik.GetRandomInt(themeById.restSecondsMin, themeById.restSecondsMax) * 1000;
					}
					else
					{
						num = Logik.GetRandomInt(this.m_effectiveTheme.restSecondsMin, this.m_effectiveTheme.restSecondsMax) * 1000;
					}
				}
				if (num > 0)
				{
					this.m_timeStampRestStart = Logik.GetTimestampMillisElapsedSinceInitialisation();
					this.m_timerWakeUpFromRest.SetTimer(num, 0);
					return;
				}
				this.WakeUpFromRestHandler();
			}
		}

		private void WakeUpFromRestHandler()
		{
			if (this.m_effectiveTheme != null)
			{
				this.PlayThemeNowOrAtEndOfCurrentSegmentAndStartEvaluation(this.m_effectiveTheme.id, this.m_effectiveTheme.intensityAfterRest, this.m_effectiveTheme.musicDurationAfterRest, true, false);
				this.m_psaiState = PsaiState.playing;
				this.m_psaiStateIntended = PsaiState.playing;
			}
		}

		private Segment GetBestCompatibleSegment(Segment sourceSegment, int targetThemeId, float intensity, int allowedSegmentSuitabilities)
		{
			float num = 0f;
			int num2 = 0;
			int num3 = 0;
			if (sourceSegment == null)
			{
				return null;
			}
			List<Follower> list = new List<Follower>();
			int count = sourceSegment.Followers.Count;
			for (int i = 0; i < count; i++)
			{
				int snippetId = sourceSegment.Followers[i].snippetId;
				Segment segmentById = this.m_soundtrack.GetSegmentById(snippetId);
				if (segmentById != null && (allowedSegmentSuitabilities & segmentById.SnippetTypeBitfield) > 0 && segmentById.ThemeId == targetThemeId)
				{
					if (i == 0)
					{
						num2 = segmentById.Playcount;
					}
					else if (segmentById.Playcount < num2)
					{
						num2 = segmentById.Playcount;
					}
					if (segmentById.Playcount > num3)
					{
						num3 = segmentById.Playcount;
					}
					float num4 = intensity - segmentById.Intensity;
					if (num4 < 0f)
					{
						num4 *= -1f;
					}
					if (num4 > num)
					{
						num = num4;
					}
					list.Add(sourceSegment.Followers[i]);
				}
			}
			if (list.Count == 0)
			{
				return null;
			}
			Weighting weighting = null;
			Theme themeById = this.m_soundtrack.getThemeById(targetThemeId);
			if (themeById != null)
			{
				weighting = themeById.weightings;
			}
			return this.ChooseBestSegmentFromList(list, weighting, intensity, num3, num2, num);
		}

		private Segment GetBestStartSegmentForTheme(int themeId, float intensity)
		{
			Theme themeById = this.m_soundtrack.getThemeById(themeId);
			if (themeById == null)
			{
				return null;
			}
			return this.GetBestStartSegmentForTheme_internal(themeById, intensity);
		}

		private Segment GetBestStartSegmentForTheme_internal(Theme theme, float intensity)
		{
			float num = 0f;
			int num2 = 0;
			int num3 = 0;
			List<Follower> list = new List<Follower>();
			int count = theme.m_segments.Count;
			for (int i = 0; i < count; i++)
			{
				Segment segment = theme.m_segments[i];
				if (segment != null && (1 & segment.SnippetTypeBitfield) > 0)
				{
					if (i == 0)
					{
						num2 = segment.Playcount;
					}
					else if (segment.Playcount < num2)
					{
						num2 = segment.Playcount;
					}
					if (segment.Playcount > num3)
					{
						num3 = segment.Playcount;
					}
					float num4 = intensity - segment.Intensity;
					if (num4 < 0f)
					{
						num4 *= -1f;
					}
					if (num4 > num)
					{
						num = num4;
					}
					Follower follower = new Follower(segment.Id, 1f);
					list.Add(follower);
				}
			}
			Weighting weightings = theme.weightings;
			return this.ChooseBestSegmentFromList(list, weightings, intensity, num3, num2, num);
		}

		private Segment ChooseBestSegmentFromList(List<Follower> segmentList, Weighting weighting, float intensity, int maxPlaycount, int minPlaycount, float maxDeltaIntensity)
		{
			Segment segment = null;
			if (segmentList.Count == 0)
			{
				return null;
			}
			int num = maxPlaycount - minPlaycount;
			float num2;
			if (num > 0)
			{
				num2 = 1f / (float)num;
			}
			else
			{
				num2 = 0f;
			}
			float num3;
			if (maxDeltaIntensity > 0f)
			{
				num3 = 1f / maxDeltaIntensity;
			}
			else
			{
				num3 = 0f;
			}
			float num4 = 0f;
			int count = segmentList.Count;
			for (int i = 0; i < count; i++)
			{
				Segment segmentById = this.m_soundtrack.GetSegmentById(segmentList[i].snippetId);
				float num5 = 1f - weighting.switchGroups;
				float num6 = 1f - weighting.intensityVsVariety;
				float intensityVsVariety = weighting.intensityVsVariety;
				float num7 = 1f - weighting.lowPlaycountVsRandom;
				float lowPlaycountVsRandom = weighting.lowPlaycountVsRandom;
				float num8 = segmentList[i].compatibility * num5;
				float num9 = intensity - segmentById.Intensity;
				if (num9 < 0f)
				{
					num9 *= -1f;
				}
				float num10 = (1f - num9 * num3) * num6;
				float num11 = (float)(segmentById.Playcount - minPlaycount) * num2;
				float num12 = (1f - num11) * num7;
				float num13 = Logik.GetRandomFloat() * lowPlaycountVsRandom;
				float num14 = (num12 + num13) * intensityVsVariety * 0.5f;
				float num15 = num8 + num10 + num14;
				if (segment == null || num15 > num4)
				{
					segment = segmentById;
					num4 = num15;
				}
			}
			return segment;
		}

		private void PlaySegmentOfCurrentTheme(SegmentSuitability snippetType)
		{
			if (this.m_effectiveTheme != null)
			{
				float currentIntensity = this.getCurrentIntensity();
				if (this.m_currentSegmentPlaying != null)
				{
					Segment segment = this.GetBestCompatibleSegment(this.m_currentSegmentPlaying, this.m_effectiveTheme.id, currentIntensity, (int)snippetType);
					this.m_targetSegmentSuitabilitiesRequested = (int)snippetType;
					if (segment == null)
					{
						segment = this.substituteSegment(this.m_effectiveTheme.id);
					}
					if (segment != null)
					{
						this.PlaySegment(segment, false);
					}
				}
			}
		}

		private Segment substituteSegment(int themeId)
		{
			Segment segment = null;
			Theme themeById = this.m_soundtrack.getThemeById(themeId);
			if (themeById != null && themeById.m_segments.Count > 0)
			{
				segment = themeById.m_segments[0];
			}
			return segment;
		}

		private void EnterSilenceMode()
		{
			this.m_timerStartSnippetPlayback.Stop();
			this.m_timerSegmentEndApproaching.Stop();
			this.m_timerWakeUpFromRest.Stop();
			this.m_targetSegment = null;
			this.m_effectiveTheme = null;
			this.m_scheduleFadeoutUponSnippetPlayback = false;
			this.m_psaiStateIntended = PsaiState.silence;
			this.m_psaiState = PsaiState.silence;
		}

		internal bool menuModeIsActive()
		{
			return this.m_psaiPlayMode == PsaiPlayMode.menuMode;
		}

		internal bool cutSceneIsActive()
		{
			return this.m_psaiPlayModeIntended == PsaiPlayMode.cutScene;
		}

		internal PsaiResult MenuModeEnter(int menuThemeId, float menuIntensity)
		{
			if (this.m_initializationFailure)
			{
				return PsaiResult.initialization_error;
			}
			if (this.m_paused)
			{
				this.setPaused(false);
			}
			if (this.m_psaiPlayMode != PsaiPlayMode.menuMode)
			{
				if (this.m_psaiPlayMode == PsaiPlayMode.cutScene && this.m_psaiPlayModeIntended == PsaiPlayMode.regular)
				{
					this.PushEffectiveThemeToThemeQueue(this.m_psaiPlayModeIntended);
				}
				else if (this.m_psaiState == PsaiState.playing)
				{
					this.PushEffectiveThemeToThemeQueue(this.m_psaiPlayMode);
				}
				if (this.m_soundtrack.getThemeById(menuThemeId) != null)
				{
					this.PlayThemeNowOrAtEndOfCurrentSegmentAndStartEvaluation(menuThemeId, menuIntensity, 666, true, true);
				}
				else
				{
					this.setPaused(false);
				}
				this.SetPlayMode(PsaiPlayMode.menuMode);
				this.m_psaiPlayModeIntended = PsaiPlayMode.menuMode;
				return PsaiResult.OK;
			}
			return PsaiResult.commandIgnoredMenuModeActive;
		}

		internal PsaiResult MenuModeLeave()
		{
			if (this.m_initializationFailure)
			{
				return PsaiResult.initialization_error;
			}
			if (this.m_paused)
			{
				this.setPaused(false);
			}
			if (this.m_psaiPlayMode != PsaiPlayMode.menuMode)
			{
				return PsaiResult.commandIgnored;
			}
			if (this.getFollowingThemeQueueEntry() != null)
			{
				this.PopAndPlayNextFollowingTheme(true);
				return PsaiResult.OK;
			}
			this.m_psaiStateIntended = PsaiState.silence;
			this.m_psaiState = PsaiState.silence;
			this.SetPlayMode(PsaiPlayMode.regular);
			this.m_psaiPlayModeIntended = PsaiPlayMode.regular;
			this.StopMusic(true);
			return PsaiResult.OK;
		}

		internal PsaiResult CutSceneEnter(int themeId, float intensity)
		{
			if (this.m_initializationFailure)
			{
				return PsaiResult.initialization_error;
			}
			PsaiPlayMode psaiPlayModeIntended = this.m_psaiPlayModeIntended;
			if (psaiPlayModeIntended == PsaiPlayMode.menuMode)
			{
				return PsaiResult.commandIgnoredMenuModeActive;
			}
			if (psaiPlayModeIntended == PsaiPlayMode.cutScene)
			{
				return PsaiResult.commandIgnoredCutsceneActive;
			}
			this.PushEffectiveThemeToThemeQueue(PsaiPlayMode.regular);
			Theme themeById = this.m_soundtrack.getThemeById(themeId);
			this.SetPlayMode(PsaiPlayMode.cutScene);
			this.m_psaiPlayModeIntended = PsaiPlayMode.cutScene;
			if (themeById != null)
			{
				this.PlayThemeNowOrAtEndOfCurrentSegmentAndStartEvaluation(themeId, intensity, themeById.musicDurationGeneral, true, true);
				return PsaiResult.OK;
			}
			this.PlayThemeNowOrAtEndOfCurrentSegmentAndStartEvaluation(this.m_lastBasicMood.id, intensity, this.m_lastBasicMood.musicDurationGeneral, true, true);
			return PsaiResult.unknown_theme;
		}

		internal PsaiResult CutSceneLeave(bool immediately, bool reset)
		{
			if (this.m_initializationFailure)
			{
				return PsaiResult.initialization_error;
			}
			if (this.m_psaiPlayMode != PsaiPlayMode.cutScene || this.m_psaiPlayModeIntended != PsaiPlayMode.cutScene)
			{
				return PsaiResult.commandIgnored;
			}
			if (reset)
			{
				this.m_themeQueue.Clear();
			}
			if (this.getFollowingThemeQueueEntry() != null)
			{
				this.m_psaiPlayModeIntended = PsaiPlayMode.regular;
				this.PopAndPlayNextFollowingTheme(immediately);
				return PsaiResult.OK;
			}
			this.m_psaiStateIntended = PsaiState.silence;
			this.m_psaiState = PsaiState.silence;
			this.m_psaiPlayModeIntended = PsaiPlayMode.regular;
			this.StopMusic(immediately);
			return PsaiResult.OK;
		}

		private ThemeQueueEntry getFollowingThemeQueueEntry()
		{
			if (this.m_themeQueue.Count > 0)
			{
				return this.m_themeQueue[0];
			}
			return null;
		}

		private void SetPlayMode(PsaiPlayMode playMode)
		{
			this.m_psaiPlayMode = playMode;
		}

		private void PushEffectiveThemeToThemeQueue(PsaiPlayMode playModeToReturnTo)
		{
			if (this.m_psaiState == PsaiState.rest)
			{
				int num = Logik.GetTimestampMillisElapsedSinceInitialisation() - this.m_timeStampRestStart;
				this.m_timerWakeUpFromRest.Stop();
				this.pushThemeToThemeQueue(this.m_lastBasicMood.id, this.m_lastBasicMood.intensityAfterRest, 0, true, num, PsaiPlayMode.regular, false);
				return;
			}
			if (this.m_latestEndOfSegmentTriggerCall.themeId != -1)
			{
				Theme themeById = this.m_soundtrack.getThemeById(this.m_latestEndOfSegmentTriggerCall.themeId);
				this.pushThemeToThemeQueue(themeById.id, this.m_latestEndOfSegmentTriggerCall.startIntensity, this.m_latestEndOfSegmentTriggerCall.musicDuration, false, 0, PsaiPlayMode.regular, false);
				this.m_latestEndOfSegmentTriggerCall.themeId = -1;
				return;
			}
			Segment effectiveSegment = this.GetEffectiveSegment();
			if (effectiveSegment != null)
			{
				if ((effectiveSegment == this.m_targetSegment && this.m_currentSegmentPlaying == null) || (this.m_targetSegment != null && this.m_currentSegmentPlaying != null && this.m_targetSegment.ThemeId != this.m_currentSegmentPlaying.ThemeId))
				{
					Theme themeById2 = this.m_soundtrack.getThemeById(this.m_targetSegment.ThemeId);
					this.pushThemeToThemeQueue(this.m_targetSegment.ThemeId, this.getUpcomingIntensity(), themeById2.musicDurationGeneral, false, 0, playModeToReturnTo, this.m_holdIntensity);
					return;
				}
				this.pushThemeToThemeQueue(effectiveSegment.ThemeId, this.getCurrentIntensity(), this.GetRemainingMusicDurationSecondsOfCurrentTheme(), false, 0, playModeToReturnTo, this.m_holdIntensity);
			}
		}

		private Segment GetEffectiveSegment()
		{
			if (this.m_targetSegment != null)
			{
				return this.m_targetSegment;
			}
			if (this.m_currentSegmentPlaying != null)
			{
				return this.m_currentSegmentPlaying;
			}
			return null;
		}

		internal int getEffectiveThemeId()
		{
			if (this.m_effectiveTheme != null)
			{
				return this.m_effectiveTheme.id;
			}
			return -1;
		}

		private int GetEffectiveSegmentSuitabilitiesRequested()
		{
			if (this.m_targetSegment != null)
			{
				return this.m_targetSegmentSuitabilitiesRequested;
			}
			return this.m_currentSnippetTypeRequested;
		}

		private void PopAndPlayNextFollowingTheme(bool immediately)
		{
			if (this.getFollowingThemeQueueEntry() != null)
			{
				ThemeQueueEntry followingThemeQueueEntry = this.getFollowingThemeQueueEntry();
				this.m_psaiPlayModeIntended = followingThemeQueueEntry.playmode;
				switch (this.m_psaiPlayModeIntended)
				{
				case PsaiPlayMode.regular:
					this.PlayThemeNowOrAtEndOfCurrentSegmentAndStartEvaluation(followingThemeQueueEntry, immediately);
					break;
				case PsaiPlayMode.menuMode:
					this.PlayThemeNowOrAtEndOfCurrentSegmentAndStartEvaluation(followingThemeQueueEntry.themeId, followingThemeQueueEntry.startIntensity, followingThemeQueueEntry.musicDuration, immediately, followingThemeQueueEntry.holdIntensity);
					break;
				case PsaiPlayMode.cutScene:
					this.PlayThemeNowOrAtEndOfCurrentSegmentAndStartEvaluation(followingThemeQueueEntry.themeId, followingThemeQueueEntry.startIntensity, followingThemeQueueEntry.musicDuration, immediately, followingThemeQueueEntry.holdIntensity);
					break;
				}
				this.removeFirstFollowingThemeQueueEntry();
			}
		}

		private void removeFirstFollowingThemeQueueEntry()
		{
			if (this.m_themeQueue.Count > 0)
			{
				this.m_themeQueue.RemoveAt(0);
			}
		}

		internal PsaiInfo getPsaiInfo()
		{
			PsaiState psaiState = this.m_psaiState;
			PsaiState psaiStateIntended = this.m_psaiStateIntended;
			Theme lastBasicMood = this.m_lastBasicMood;
			int num = ((lastBasicMood != null) ? lastBasicMood.id : (-1));
			int effectiveThemeId = this.getEffectiveThemeId();
			int upcomingThemeId = this.getUpcomingThemeId();
			float currentIntensity = this.getCurrentIntensity();
			float upcomingIntensity = this.getUpcomingIntensity();
			int count = this.m_themeQueue.Count;
			Segment targetSegment = this.m_targetSegment;
			return new PsaiInfo(psaiState, psaiStateIntended, num, effectiveThemeId, upcomingThemeId, currentIntensity, upcomingIntensity, count, (targetSegment != null) ? targetSegment.Id : (-1), this.m_holdIntensity, this.m_returnToLastBasicMoodFlag, this.m_timerWakeUpFromRest.IsSet() ? this.m_timerWakeUpFromRest.GetRemainingMillisToFireTime() : 0, this.m_paused);
		}

		internal int getCurrentSnippetId()
		{
			if (this.m_currentSegmentPlaying != null)
			{
				return this.m_currentSegmentPlaying.Id;
			}
			return -1;
		}

		internal int GetRemainingMillisecondsOfCurrentSegmentPlayback()
		{
			if (this.m_currentSegmentPlaying != null)
			{
				return this.m_currentSegmentPlaying.audioData.GetFullLengthInMilliseconds() - this.GetMillisElapsedAfterCurrentSnippetPlaycall();
			}
			return -1;
		}

		internal int GetRemainingMillisecondsUntilNextSegmentStart()
		{
			if (this.m_timerStartSnippetPlayback.IsSet() && this.m_timerStartSnippetPlayback.IsSet())
			{
				return this.m_timerStartSnippetPlayback.GetRemainingMillisToFireTime();
			}
			return -1;
		}

		internal bool CheckIfAtLeastOneDirectTransitionOrLayeringIsPossible(int sourceSegmentId, int targetThemeId)
		{
			Segment segmentById = this.m_soundtrack.GetSegmentById(sourceSegmentId);
			return segmentById != null && segmentById.CheckIfAtLeastOneDirectTransitionOrLayeringIsPossible(this.m_soundtrack, targetThemeId);
		}

		private const string PSAI_VERSION = ".NET 1.7.3";

		private const string m_fullVersionString = "psai Version .NET 1.7.3";

		internal const float COMPATIBILITY_PERCENTAGE_SAME_GROUP = 1f;

		internal const float COMPATIBILITY_PERCENTAGE_OTHER_GROUP = 0.5f;

		internal const int PSAI_CHANNEL_COUNT = 9;

		internal const int PSAI_CHANNEL_COUNT_HIGHLIGHTS = 2;

		internal const int PSAI_FADING_UPDATE_INVERVAL_MILLIS = 50;

		internal const int PSAI_FADEOUTMILLIS_PLAYIMMEDIATELY = 500;

		internal const int PSAI_FADEOUTMILLIS_STOPMUSIC = 1000;

		internal const int PSAI_FADEOUTMILLIS_HIGHLIGHT_INTERRUPTED = 2000;

		internal const int SNIPPET_TYPE_MIDDLE_OR_BRIDGE = 10;

		private static Random s_random = new Random();

		internal Soundtrack m_soundtrack;

		private List<FadeData> m_fadeVoices;

		private int m_currentVoiceNumber;

		private int m_targetVoice;

		private IPlatformLayer m_platformLayer;

		private bool m_initializationFailure;

		internal string m_psaiCoreSoundtackFilepath;

		internal string m_psaiCoreSoundtrackDirectoryName;

		private static Stopwatch m_stopWatch = new Stopwatch();

		private Theme m_lastBasicMood;

		private int m_hilightVoiceIndex;

		private int m_lastRegularVoiceNumberReturned;

		private float m_psaiMasterVolume;

		private Segment m_currentSegmentPlaying;

		private int m_currentSnippetTypeRequested;

		private Theme m_effectiveTheme;

		private int m_timeStampCurrentSnippetPlaycall;

		private int m_estimatedTimestampOfTargetSnippetPlayback;

		private int m_timeStampOfLastIntensitySetForCurrentTheme;

		private int m_timeStampRestStart;

		private Segment m_targetSegment;

		private int m_targetSegmentSuitabilitiesRequested;

		private float m_currentIntensitySlope;

		private float m_lastIntensity;

		private bool m_holdIntensity;

		private float m_heldIntensity;

		private bool m_scheduleFadeoutUponSnippetPlayback;

		private float m_startOrRetriggerIntensityOfCurrentTheme;

		private int m_lastMusicDuration;

		private int m_remainingMusicDurationAtTimeOfHoldIntensity;

		private PsaiState m_psaiState;

		private PsaiState m_psaiStateIntended;

		private List<ThemeQueueEntry> m_themeQueue;

		private PsaiPlayMode m_psaiPlayMode;

		private PsaiPlayMode m_psaiPlayModeIntended;

		private bool m_returnToLastBasicMoodFlag;

		private PlaybackChannel[] m_playbackChannels = new PlaybackChannel[9];

		internal static int s_audioLayerMaximumLatencyForPlayingbackPrebufferedSounds = 50;

		internal static int s_audioLayerMaximumLatencyForBufferingSounds = 200;

		internal static int s_audioLayerMaximumLatencyForPlayingBackUnbufferedSounds;

		internal static int s_updateIntervalMillis = 100;

		private PsaiTimer m_timerStartSnippetPlayback = new PsaiTimer();

		private PsaiTimer m_timerSegmentEndApproaching = new PsaiTimer();

		private PsaiTimer m_timerSegmentEndReached = new PsaiTimer();

		private PsaiTimer m_timerFades = new PsaiTimer();

		private PsaiTimer m_timerWakeUpFromRest = new PsaiTimer();

		private int m_timeStampOfLastFadeUpdate;

		private ThemeQueueEntry m_latestEndOfSegmentTriggerCall = new ThemeQueueEntry();

		private bool m_paused;

		private int m_timeStampPauseOn;

		private int m_restModeSecondsOverride;
	}
}
