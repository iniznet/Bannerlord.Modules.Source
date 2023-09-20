using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using psai.Editor;

namespace psai.net
{
	// Token: 0x02000014 RID: 20
	internal class Logik
	{
		// Token: 0x17000051 RID: 81
		// (get) Token: 0x0600013D RID: 317 RVA: 0x00005F73 File Offset: 0x00004173
		// (set) Token: 0x0600013E RID: 318 RVA: 0x00005F7A File Offset: 0x0000417A
		internal static Logik Instance { get; private set; }

		// Token: 0x0600013F RID: 319 RVA: 0x00005F84 File Offset: 0x00004184
		internal void Release()
		{
			for (int i = 0; i < this.m_playbackChannels.Length; i++)
			{
				this.m_playbackChannels[i].Release();
			}
			this.m_platformLayer.Release();
		}

		// Token: 0x06000140 RID: 320 RVA: 0x00005FBC File Offset: 0x000041BC
		internal static int GetRandomInt(int min, int max)
		{
			return Logik.s_random.Next(min, max);
		}

		// Token: 0x06000141 RID: 321 RVA: 0x00005FCA File Offset: 0x000041CA
		internal static float GetRandomFloat()
		{
			return (float)Logik.s_random.NextDouble();
		}

		// Token: 0x06000142 RID: 322 RVA: 0x00005FD7 File Offset: 0x000041D7
		private static void UpdateMaximumLatencyForPlayingBackUnbufferedSounds()
		{
			Logik.s_audioLayerMaximumLatencyForPlayingBackUnbufferedSounds = Logik.s_audioLayerMaximumLatencyForBufferingSounds + Logik.s_audioLayerMaximumLatencyForPlayingbackPrebufferedSounds;
		}

		// Token: 0x06000143 RID: 323 RVA: 0x00005FE9 File Offset: 0x000041E9
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

		// Token: 0x06000144 RID: 324 RVA: 0x00005FFE File Offset: 0x000041FE
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

		// Token: 0x06000145 RID: 325 RVA: 0x00006014 File Offset: 0x00004214
		static Logik()
		{
			Logik.m_stopWatch.Start();
			Logik.UpdateMaximumLatencyForPlayingBackUnbufferedSounds();
			Logik.Instance = new Logik();
		}

		// Token: 0x06000146 RID: 326 RVA: 0x00006068 File Offset: 0x00004268
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

		// Token: 0x06000147 RID: 327 RVA: 0x00006182 File Offset: 0x00004382
		private Logik(string pathToPcbFile)
			: this()
		{
			this.LoadSoundtrack(pathToPcbFile);
		}

		// Token: 0x06000148 RID: 328 RVA: 0x00006194 File Offset: 0x00004394
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

		// Token: 0x06000149 RID: 329 RVA: 0x0000620C File Offset: 0x0000440C
		public PsaiResult LoadSoundtrackByPsaiProject(PsaiProject psaiProject, string fullProjectPath)
		{
			this.m_soundtrack = psaiProject.BuildPsaiDotNetSoundtrackFromProject();
			this.m_psaiCoreSoundtackFilepath = fullProjectPath;
			this.m_psaiCoreSoundtrackDirectoryName = Path.GetDirectoryName(fullProjectPath);
			this.InitMembersAfterSoundtrackHasLoaded();
			return PsaiResult.OK;
		}

		// Token: 0x0600014A RID: 330 RVA: 0x00006234 File Offset: 0x00004434
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

		// Token: 0x0600014B RID: 331 RVA: 0x000062B4 File Offset: 0x000044B4
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

		// Token: 0x0600014C RID: 332 RVA: 0x000063A4 File Offset: 0x000045A4
		internal int GetLastBasicMoodId()
		{
			if (this.m_lastBasicMood != null)
			{
				return this.m_lastBasicMood.id;
			}
			return -1;
		}

		// Token: 0x0600014D RID: 333 RVA: 0x000063BC File Offset: 0x000045BC
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

		// Token: 0x0600014E RID: 334 RVA: 0x000063E8 File Offset: 0x000045E8
		internal static bool CheckIfFileExists(string filepath)
		{
			return File.Exists(filepath);
		}

		// Token: 0x0600014F RID: 335 RVA: 0x000063F0 File Offset: 0x000045F0
		private int GetRemainingMusicDurationSecondsOfCurrentTheme()
		{
			int num = Logik.GetTimestampMillisElapsedSinceInitialisation() - this.m_timeStampOfLastIntensitySetForCurrentTheme;
			return this.m_lastMusicDuration - num / 1000;
		}

		// Token: 0x06000150 RID: 336 RVA: 0x00006418 File Offset: 0x00004618
		internal static int GetTimestampMillisElapsedSinceInitialisation()
		{
			return (int)Logik.m_stopWatch.ElapsedMilliseconds;
		}

		// Token: 0x06000151 RID: 337 RVA: 0x00006425 File Offset: 0x00004625
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

		// Token: 0x06000152 RID: 338 RVA: 0x00006444 File Offset: 0x00004644
		internal string getVersion()
		{
			return "psai Version .NET 1.7.3";
		}

		// Token: 0x06000153 RID: 339 RVA: 0x0000644B File Offset: 0x0000464B
		internal long GetCurrentSystemTimeMillis()
		{
			return (long)Logik.GetTimestampMillisElapsedSinceInitialisation();
		}

		// Token: 0x06000154 RID: 340 RVA: 0x00006454 File Offset: 0x00004654
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

		// Token: 0x06000155 RID: 341 RVA: 0x000064FC File Offset: 0x000046FC
		private void AddFadeData(int voiceNumber, int fadeoutMillis, float currentVolume, int delayMillis)
		{
			FadeData fadeData = new FadeData();
			fadeData.voiceNumber = voiceNumber;
			fadeData.fadeoutDeltaVolumePerUpdate = currentVolume / ((float)fadeoutMillis / 50f);
			fadeData.currentVolume = currentVolume;
			fadeData.delayMillis = delayMillis;
			this.m_fadeVoices.Add(fadeData);
		}

		// Token: 0x06000156 RID: 342 RVA: 0x00006544 File Offset: 0x00004744
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

		// Token: 0x06000157 RID: 343 RVA: 0x00006582 File Offset: 0x00004782
		private void PsaiErrorCheck(PsaiResult result, string infoAboutLastCall)
		{
		}

		// Token: 0x06000158 RID: 344 RVA: 0x00006584 File Offset: 0x00004784
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

		// Token: 0x06000159 RID: 345 RVA: 0x000065B4 File Offset: 0x000047B4
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

		// Token: 0x0600015A RID: 346 RVA: 0x00006698 File Offset: 0x00004898
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

		// Token: 0x0600015B RID: 347 RVA: 0x00006747 File Offset: 0x00004947
		private void SetThemeAsLastBasicMood(Theme latestBasicMood)
		{
			if (latestBasicMood != null)
			{
				this.m_lastBasicMood = latestBasicMood;
			}
		}

		// Token: 0x0600015C RID: 348 RVA: 0x00006753 File Offset: 0x00004953
		private bool CheckIfAnyThemeIsCurrentlyPlaying()
		{
			return this.m_psaiState == PsaiState.playing && this.m_currentSegmentPlaying != null && this.m_effectiveTheme != null;
		}

		// Token: 0x0600015D RID: 349 RVA: 0x00006774 File Offset: 0x00004974
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

		// Token: 0x0600015E RID: 350 RVA: 0x00006898 File Offset: 0x00004A98
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

		// Token: 0x0600015F RID: 351 RVA: 0x000068DC File Offset: 0x00004ADC
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

		// Token: 0x06000160 RID: 352 RVA: 0x0000691C File Offset: 0x00004B1C
		internal PsaiResult TriggerMusicTheme(int argThemeId, float argIntensity, int argMusicDuration)
		{
			Theme themeById = this.m_soundtrack.getThemeById(argThemeId);
			if (themeById == null)
			{
				return PsaiResult.unknown_theme;
			}
			return this.TriggerMusicTheme(themeById, argIntensity, argMusicDuration);
		}

		// Token: 0x06000161 RID: 353 RVA: 0x00006948 File Offset: 0x00004B48
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

		// Token: 0x06000162 RID: 354 RVA: 0x00006D49 File Offset: 0x00004F49
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

		// Token: 0x06000163 RID: 355 RVA: 0x00006D68 File Offset: 0x00004F68
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

		// Token: 0x06000164 RID: 356 RVA: 0x00006DD4 File Offset: 0x00004FD4
		internal PsaiResult PlaySegmentLayeredAndImmediately(int segmentId)
		{
			Segment segmentById = this.m_soundtrack.GetSegmentById(segmentId);
			if (segmentById != null)
			{
				this.PlaySegmentLayeredAndImmediately(segmentById);
			}
			return PsaiResult.invalidHandle;
		}

		// Token: 0x06000165 RID: 357 RVA: 0x00006DFC File Offset: 0x00004FFC
		internal void PlaySegmentLayeredAndImmediately(Segment segment)
		{
			this.m_hilightVoiceIndex = this.getNextVoiceNumber(true);
			this.m_playbackChannels[this.m_hilightVoiceIndex].StopChannel();
			this.m_playbackChannels[this.m_hilightVoiceIndex].ReleaseSegment();
			this.m_playbackChannels[this.m_hilightVoiceIndex].FadeOutVolume = 1f;
			this.m_playbackChannels[this.m_hilightVoiceIndex].ScheduleSegmentPlayback(segment, Logik.s_audioLayerMaximumLatencyForBufferingSounds + Logik.s_audioLayerMaximumLatencyForPlayingbackPrebufferedSounds);
		}

		// Token: 0x06000166 RID: 358 RVA: 0x00006E70 File Offset: 0x00005070
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

		// Token: 0x06000167 RID: 359 RVA: 0x00006F00 File Offset: 0x00005100
		private void ClearLatestEndOfSegmentTriggerCall()
		{
			this.m_latestEndOfSegmentTriggerCall.themeId = -1;
		}

		// Token: 0x06000168 RID: 360 RVA: 0x00006F0E File Offset: 0x0000510E
		private void ClearQueuedTheme()
		{
			this.m_themeQueue.Clear();
		}

		// Token: 0x06000169 RID: 361 RVA: 0x00006F1C File Offset: 0x0000511C
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

		// Token: 0x0600016A RID: 362 RVA: 0x00006F90 File Offset: 0x00005190
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

		// Token: 0x0600016B RID: 363 RVA: 0x00006FC4 File Offset: 0x000051C4
		internal float getUpcomingIntensity()
		{
			if (this.m_psaiState == PsaiState.playing && this.m_latestEndOfSegmentTriggerCall.themeId != -1)
			{
				return this.m_latestEndOfSegmentTriggerCall.startIntensity;
			}
			return this.getCurrentIntensity();
		}

		// Token: 0x0600016C RID: 364 RVA: 0x00006FF0 File Offset: 0x000051F0
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

		// Token: 0x0600016D RID: 365 RVA: 0x000070C8 File Offset: 0x000052C8
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

		// Token: 0x0600016E RID: 366 RVA: 0x000071EE File Offset: 0x000053EE
		private PsaiResult LoadSegment(Segment snippet, int channelIndex)
		{
			if (snippet == null || channelIndex >= 9)
			{
				return PsaiResult.invalidHandle;
			}
			this.m_playbackChannels[channelIndex].LoadSegment(snippet);
			return PsaiResult.OK;
		}

		// Token: 0x0600016F RID: 367 RVA: 0x0000720C File Offset: 0x0000540C
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

		// Token: 0x06000170 RID: 368 RVA: 0x00007314 File Offset: 0x00005514
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

		// Token: 0x06000171 RID: 369 RVA: 0x00007392 File Offset: 0x00005592
		internal float getVolume()
		{
			return this.m_psaiMasterVolume;
		}

		// Token: 0x06000172 RID: 370 RVA: 0x0000739C File Offset: 0x0000559C
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

		// Token: 0x06000173 RID: 371 RVA: 0x00007403 File Offset: 0x00005603
		private PsaiResult PlayThemeNowOrAtEndOfCurrentSegmentAndStartEvaluation(ThemeQueueEntry tqe, bool immediately)
		{
			return this.PlayThemeNowOrAtEndOfCurrentSegmentAndStartEvaluation(tqe.themeId, tqe.startIntensity, tqe.musicDuration, immediately, tqe.holdIntensity);
		}

		// Token: 0x06000174 RID: 372 RVA: 0x00007424 File Offset: 0x00005624
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

		// Token: 0x06000175 RID: 373 RVA: 0x0000752D File Offset: 0x0000572D
		internal PsaiResult StopMusic(bool immediately)
		{
			return this.StopMusic(immediately, 1000);
		}

		// Token: 0x06000176 RID: 374 RVA: 0x0000753C File Offset: 0x0000573C
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

		// Token: 0x06000177 RID: 375 RVA: 0x00007620 File Offset: 0x00005820
		private void WriteLogWarningIfThereIsNoDirectPathForEffectiveSnippetToEndSnippet()
		{
		}

		// Token: 0x06000178 RID: 376 RVA: 0x00007624 File Offset: 0x00005824
		private bool CheckIfThereIsAPathToEndSegmentForEffectiveSegmentAndLogWarningIfThereIsnt()
		{
			Segment effectiveSegment = this.GetEffectiveSegment();
			return effectiveSegment.IsUsableAs(SegmentSuitability.end) || effectiveSegment.nextSnippetToShortestEndSequence != null;
		}

		// Token: 0x06000179 RID: 377 RVA: 0x0000764C File Offset: 0x0000584C
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

		// Token: 0x0600017A RID: 378 RVA: 0x00007770 File Offset: 0x00005970
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

		// Token: 0x0600017B RID: 379 RVA: 0x000077FC File Offset: 0x000059FC
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

		// Token: 0x0600017C RID: 380 RVA: 0x00007824 File Offset: 0x00005A24
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

		// Token: 0x0600017D RID: 381 RVA: 0x00007A57 File Offset: 0x00005C57
		private PsaiResult PlayThemeAtEndOfCurrentSegment(Theme argTheme, float intensity, int musicDuration)
		{
			this.m_latestEndOfSegmentTriggerCall.themeId = argTheme.id;
			this.m_latestEndOfSegmentTriggerCall.startIntensity = intensity;
			this.m_latestEndOfSegmentTriggerCall.musicDuration = musicDuration;
			this.m_psaiStateIntended = PsaiState.playing;
			return PsaiResult.OK;
		}

		// Token: 0x0600017E RID: 382 RVA: 0x00007A8A File Offset: 0x00005C8A
		private PsaiResult PlayThemeAtEndOfCurrentTheme(Theme argTheme, float argIntensity, int argMusicDuration)
		{
			this.ClearLatestEndOfSegmentTriggerCall();
			this.ClearQueuedTheme();
			this.pushThemeToThemeQueue(argTheme.id, argIntensity, argMusicDuration, false, 1, PsaiPlayMode.regular, false);
			return PsaiResult.OK;
		}

		// Token: 0x0600017F RID: 383 RVA: 0x00007AAC File Offset: 0x00005CAC
		private void SegmentEndReachedHandler()
		{
			if (this.m_targetSegment == null)
			{
				this.m_currentSegmentPlaying = null;
			}
		}

		// Token: 0x06000180 RID: 384 RVA: 0x00007AC0 File Offset: 0x00005CC0
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

		// Token: 0x06000181 RID: 385 RVA: 0x00007B4C File Offset: 0x00005D4C
		private void InitiateTransitionToRestMode()
		{
			this.InitiateTransitionToRestOrSilence(PsaiState.rest);
		}

		// Token: 0x06000182 RID: 386 RVA: 0x00007B58 File Offset: 0x00005D58
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

		// Token: 0x06000183 RID: 387 RVA: 0x00007C42 File Offset: 0x00005E42
		public PsaiResult GoToRest(bool immediately, int fadeOutMilliSeconds)
		{
			return this.GoToRest(immediately, fadeOutMilliSeconds, -1, -1);
		}

		// Token: 0x06000184 RID: 388 RVA: 0x00007C50 File Offset: 0x00005E50
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

		// Token: 0x06000185 RID: 389 RVA: 0x00007CB8 File Offset: 0x00005EB8
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

		// Token: 0x06000186 RID: 390 RVA: 0x00007D9C File Offset: 0x00005F9C
		private void WakeUpFromRestHandler()
		{
			if (this.m_effectiveTheme != null)
			{
				this.PlayThemeNowOrAtEndOfCurrentSegmentAndStartEvaluation(this.m_effectiveTheme.id, this.m_effectiveTheme.intensityAfterRest, this.m_effectiveTheme.musicDurationAfterRest, true, false);
				this.m_psaiState = PsaiState.playing;
				this.m_psaiStateIntended = PsaiState.playing;
			}
		}

		// Token: 0x06000187 RID: 391 RVA: 0x00007DEC File Offset: 0x00005FEC
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

		// Token: 0x06000188 RID: 392 RVA: 0x00007F18 File Offset: 0x00006118
		private Segment GetBestStartSegmentForTheme(int themeId, float intensity)
		{
			Theme themeById = this.m_soundtrack.getThemeById(themeId);
			if (themeById == null)
			{
				return null;
			}
			return this.GetBestStartSegmentForTheme_internal(themeById, intensity);
		}

		// Token: 0x06000189 RID: 393 RVA: 0x00007F40 File Offset: 0x00006140
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

		// Token: 0x0600018A RID: 394 RVA: 0x00008028 File Offset: 0x00006228
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

		// Token: 0x0600018B RID: 395 RVA: 0x0000818C File Offset: 0x0000638C
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

		// Token: 0x0600018C RID: 396 RVA: 0x000081F4 File Offset: 0x000063F4
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

		// Token: 0x0600018D RID: 397 RVA: 0x00008234 File Offset: 0x00006434
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

		// Token: 0x0600018E RID: 398 RVA: 0x00008285 File Offset: 0x00006485
		internal bool menuModeIsActive()
		{
			return this.m_psaiPlayMode == PsaiPlayMode.menuMode;
		}

		// Token: 0x0600018F RID: 399 RVA: 0x00008290 File Offset: 0x00006490
		internal bool cutSceneIsActive()
		{
			return this.m_psaiPlayModeIntended == PsaiPlayMode.cutScene;
		}

		// Token: 0x06000190 RID: 400 RVA: 0x0000829C File Offset: 0x0000649C
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

		// Token: 0x06000191 RID: 401 RVA: 0x0000833C File Offset: 0x0000653C
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

		// Token: 0x06000192 RID: 402 RVA: 0x000083A8 File Offset: 0x000065A8
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

		// Token: 0x06000193 RID: 403 RVA: 0x00008430 File Offset: 0x00006630
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

		// Token: 0x06000194 RID: 404 RVA: 0x000084A1 File Offset: 0x000066A1
		private ThemeQueueEntry getFollowingThemeQueueEntry()
		{
			if (this.m_themeQueue.Count > 0)
			{
				return this.m_themeQueue[0];
			}
			return null;
		}

		// Token: 0x06000195 RID: 405 RVA: 0x000084BF File Offset: 0x000066BF
		private void SetPlayMode(PsaiPlayMode playMode)
		{
			this.m_psaiPlayMode = playMode;
		}

		// Token: 0x06000196 RID: 406 RVA: 0x000084C8 File Offset: 0x000066C8
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

		// Token: 0x06000197 RID: 407 RVA: 0x00008619 File Offset: 0x00006819
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

		// Token: 0x06000198 RID: 408 RVA: 0x0000863A File Offset: 0x0000683A
		internal int getEffectiveThemeId()
		{
			if (this.m_effectiveTheme != null)
			{
				return this.m_effectiveTheme.id;
			}
			return -1;
		}

		// Token: 0x06000199 RID: 409 RVA: 0x00008651 File Offset: 0x00006851
		private int GetEffectiveSegmentSuitabilitiesRequested()
		{
			if (this.m_targetSegment != null)
			{
				return this.m_targetSegmentSuitabilitiesRequested;
			}
			return this.m_currentSnippetTypeRequested;
		}

		// Token: 0x0600019A RID: 410 RVA: 0x00008668 File Offset: 0x00006868
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

		// Token: 0x0600019B RID: 411 RVA: 0x00008701 File Offset: 0x00006901
		private void removeFirstFollowingThemeQueueEntry()
		{
			if (this.m_themeQueue.Count > 0)
			{
				this.m_themeQueue.RemoveAt(0);
			}
		}

		// Token: 0x0600019C RID: 412 RVA: 0x00008720 File Offset: 0x00006920
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

		// Token: 0x0600019D RID: 413 RVA: 0x000087B2 File Offset: 0x000069B2
		internal int getCurrentSnippetId()
		{
			if (this.m_currentSegmentPlaying != null)
			{
				return this.m_currentSegmentPlaying.Id;
			}
			return -1;
		}

		// Token: 0x0600019E RID: 414 RVA: 0x000087C9 File Offset: 0x000069C9
		internal int GetRemainingMillisecondsOfCurrentSegmentPlayback()
		{
			if (this.m_currentSegmentPlaying != null)
			{
				return this.m_currentSegmentPlaying.audioData.GetFullLengthInMilliseconds() - this.GetMillisElapsedAfterCurrentSnippetPlaycall();
			}
			return -1;
		}

		// Token: 0x0600019F RID: 415 RVA: 0x000087EC File Offset: 0x000069EC
		internal int GetRemainingMillisecondsUntilNextSegmentStart()
		{
			if (this.m_timerStartSnippetPlayback.IsSet() && this.m_timerStartSnippetPlayback.IsSet())
			{
				return this.m_timerStartSnippetPlayback.GetRemainingMillisToFireTime();
			}
			return -1;
		}

		// Token: 0x060001A0 RID: 416 RVA: 0x00008818 File Offset: 0x00006A18
		internal bool CheckIfAtLeastOneDirectTransitionOrLayeringIsPossible(int sourceSegmentId, int targetThemeId)
		{
			Segment segmentById = this.m_soundtrack.GetSegmentById(sourceSegmentId);
			return segmentById != null && segmentById.CheckIfAtLeastOneDirectTransitionOrLayeringIsPossible(this.m_soundtrack, targetThemeId);
		}

		// Token: 0x040000AB RID: 171
		private const string PSAI_VERSION = ".NET 1.7.3";

		// Token: 0x040000AC RID: 172
		private const string m_fullVersionString = "psai Version .NET 1.7.3";

		// Token: 0x040000AD RID: 173
		internal const float COMPATIBILITY_PERCENTAGE_SAME_GROUP = 1f;

		// Token: 0x040000AE RID: 174
		internal const float COMPATIBILITY_PERCENTAGE_OTHER_GROUP = 0.5f;

		// Token: 0x040000AF RID: 175
		internal const int PSAI_CHANNEL_COUNT = 9;

		// Token: 0x040000B0 RID: 176
		internal const int PSAI_CHANNEL_COUNT_HIGHLIGHTS = 2;

		// Token: 0x040000B1 RID: 177
		internal const int PSAI_FADING_UPDATE_INVERVAL_MILLIS = 50;

		// Token: 0x040000B2 RID: 178
		internal const int PSAI_FADEOUTMILLIS_PLAYIMMEDIATELY = 500;

		// Token: 0x040000B3 RID: 179
		internal const int PSAI_FADEOUTMILLIS_STOPMUSIC = 1000;

		// Token: 0x040000B4 RID: 180
		internal const int PSAI_FADEOUTMILLIS_HIGHLIGHT_INTERRUPTED = 2000;

		// Token: 0x040000B5 RID: 181
		internal const int SNIPPET_TYPE_MIDDLE_OR_BRIDGE = 10;

		// Token: 0x040000B7 RID: 183
		private static Random s_random = new Random();

		// Token: 0x040000B8 RID: 184
		internal Soundtrack m_soundtrack;

		// Token: 0x040000B9 RID: 185
		private List<FadeData> m_fadeVoices;

		// Token: 0x040000BA RID: 186
		private int m_currentVoiceNumber;

		// Token: 0x040000BB RID: 187
		private int m_targetVoice;

		// Token: 0x040000BC RID: 188
		private IPlatformLayer m_platformLayer;

		// Token: 0x040000BD RID: 189
		private bool m_initializationFailure;

		// Token: 0x040000BE RID: 190
		internal string m_psaiCoreSoundtackFilepath;

		// Token: 0x040000BF RID: 191
		internal string m_psaiCoreSoundtrackDirectoryName;

		// Token: 0x040000C0 RID: 192
		private static Stopwatch m_stopWatch = new Stopwatch();

		// Token: 0x040000C1 RID: 193
		private Theme m_lastBasicMood;

		// Token: 0x040000C2 RID: 194
		private int m_hilightVoiceIndex;

		// Token: 0x040000C3 RID: 195
		private int m_lastRegularVoiceNumberReturned;

		// Token: 0x040000C4 RID: 196
		private float m_psaiMasterVolume;

		// Token: 0x040000C5 RID: 197
		private Segment m_currentSegmentPlaying;

		// Token: 0x040000C6 RID: 198
		private int m_currentSnippetTypeRequested;

		// Token: 0x040000C7 RID: 199
		private Theme m_effectiveTheme;

		// Token: 0x040000C8 RID: 200
		private int m_timeStampCurrentSnippetPlaycall;

		// Token: 0x040000C9 RID: 201
		private int m_estimatedTimestampOfTargetSnippetPlayback;

		// Token: 0x040000CA RID: 202
		private int m_timeStampOfLastIntensitySetForCurrentTheme;

		// Token: 0x040000CB RID: 203
		private int m_timeStampRestStart;

		// Token: 0x040000CC RID: 204
		private Segment m_targetSegment;

		// Token: 0x040000CD RID: 205
		private int m_targetSegmentSuitabilitiesRequested;

		// Token: 0x040000CE RID: 206
		private float m_currentIntensitySlope;

		// Token: 0x040000CF RID: 207
		private float m_lastIntensity;

		// Token: 0x040000D0 RID: 208
		private bool m_holdIntensity;

		// Token: 0x040000D1 RID: 209
		private float m_heldIntensity;

		// Token: 0x040000D2 RID: 210
		private bool m_scheduleFadeoutUponSnippetPlayback;

		// Token: 0x040000D3 RID: 211
		private float m_startOrRetriggerIntensityOfCurrentTheme;

		// Token: 0x040000D4 RID: 212
		private int m_lastMusicDuration;

		// Token: 0x040000D5 RID: 213
		private int m_remainingMusicDurationAtTimeOfHoldIntensity;

		// Token: 0x040000D6 RID: 214
		private PsaiState m_psaiState;

		// Token: 0x040000D7 RID: 215
		private PsaiState m_psaiStateIntended;

		// Token: 0x040000D8 RID: 216
		private List<ThemeQueueEntry> m_themeQueue;

		// Token: 0x040000D9 RID: 217
		private PsaiPlayMode m_psaiPlayMode;

		// Token: 0x040000DA RID: 218
		private PsaiPlayMode m_psaiPlayModeIntended;

		// Token: 0x040000DB RID: 219
		private bool m_returnToLastBasicMoodFlag;

		// Token: 0x040000DC RID: 220
		private PlaybackChannel[] m_playbackChannels = new PlaybackChannel[9];

		// Token: 0x040000DD RID: 221
		internal static int s_audioLayerMaximumLatencyForPlayingbackPrebufferedSounds = 50;

		// Token: 0x040000DE RID: 222
		internal static int s_audioLayerMaximumLatencyForBufferingSounds = 200;

		// Token: 0x040000DF RID: 223
		internal static int s_audioLayerMaximumLatencyForPlayingBackUnbufferedSounds;

		// Token: 0x040000E0 RID: 224
		internal static int s_updateIntervalMillis = 100;

		// Token: 0x040000E1 RID: 225
		private PsaiTimer m_timerStartSnippetPlayback = new PsaiTimer();

		// Token: 0x040000E2 RID: 226
		private PsaiTimer m_timerSegmentEndApproaching = new PsaiTimer();

		// Token: 0x040000E3 RID: 227
		private PsaiTimer m_timerSegmentEndReached = new PsaiTimer();

		// Token: 0x040000E4 RID: 228
		private PsaiTimer m_timerFades = new PsaiTimer();

		// Token: 0x040000E5 RID: 229
		private PsaiTimer m_timerWakeUpFromRest = new PsaiTimer();

		// Token: 0x040000E6 RID: 230
		private int m_timeStampOfLastFadeUpdate;

		// Token: 0x040000E7 RID: 231
		private ThemeQueueEntry m_latestEndOfSegmentTriggerCall = new ThemeQueueEntry();

		// Token: 0x040000E8 RID: 232
		private bool m_paused;

		// Token: 0x040000E9 RID: 233
		private int m_timeStampPauseOn;

		// Token: 0x040000EA RID: 234
		private int m_restModeSecondsOverride;
	}
}
