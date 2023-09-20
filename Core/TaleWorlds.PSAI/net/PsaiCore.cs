using System;

namespace psai.net
{
	public class PsaiCore
	{
		public static PsaiCore Instance
		{
			get
			{
				if (PsaiCore.s_singleton == null)
				{
					PsaiCore.s_singleton = new PsaiCore();
				}
				return PsaiCore.s_singleton;
			}
			set
			{
				PsaiCore.s_singleton = null;
			}
		}

		public static bool IsInstanceInitialized()
		{
			return PsaiCore.s_singleton != null;
		}

		public PsaiCore()
		{
			this.m_logik = Logik.Instance;
		}

		public PsaiResult SetMaximumLatencyNeededByPlatformToBufferSounddata(int latencyInMilliseconds)
		{
			return this.m_logik.SetMaximumLatencyNeededByPlatformToBufferSounddata(latencyInMilliseconds);
		}

		public PsaiResult SetMaximumLatencyNeededByPlatformToPlayBackBufferedSounddata(int latencyInMilliseconds)
		{
			return this.m_logik.SetMaximumLatencyNeededByPlatformToPlayBackBufferedSounds(latencyInMilliseconds);
		}

		public PsaiResult LoadSoundtrack(string pathToPcbFile)
		{
			return this.m_logik.LoadSoundtrack(pathToPcbFile);
		}

		public PsaiResult LoadSoundtrackFromProjectFile(string pathToProjectFile)
		{
			return this.m_logik.LoadSoundtrackFromProjectFile(pathToProjectFile);
		}

		public PsaiResult TriggerMusicTheme(int themeId, float intensity)
		{
			return this.m_logik.TriggerMusicTheme(themeId, intensity);
		}

		public PsaiResult TriggerMusicTheme(int themeId, float intensity, int musicDurationInSeconds)
		{
			return this.m_logik.TriggerMusicTheme(themeId, intensity, musicDurationInSeconds);
		}

		public PsaiResult AddToCurrentIntensity(float deltaIntensity)
		{
			return this.m_logik.AddToCurrentIntensity(deltaIntensity, false);
		}

		public PsaiResult StopMusic(bool immediately)
		{
			return this.m_logik.StopMusic(immediately);
		}

		public PsaiResult StopMusic(bool immediately, float fadeOutSeconds)
		{
			return this.m_logik.StopMusic(immediately, (int)(fadeOutSeconds * 1000f));
		}

		public PsaiResult ReturnToLastBasicMood(bool immediately)
		{
			return this.m_logik.ReturnToLastBasicMood(immediately);
		}

		public PsaiResult GoToRest(bool immediately, float fadeOutSeconds)
		{
			return this.m_logik.GoToRest(immediately, (int)(fadeOutSeconds * 1000f));
		}

		public PsaiResult GoToRest(bool immediately, float fadeOutSeconds, int restTimeMin, int restTimeMax)
		{
			return this.m_logik.GoToRest(immediately, (int)(fadeOutSeconds * 1000f), restTimeMin, restTimeMax);
		}

		public PsaiResult HoldCurrentIntensity(bool hold)
		{
			return this.m_logik.HoldCurrentIntensity(hold);
		}

		public string GetVersion()
		{
			return this.m_logik.getVersion();
		}

		public PsaiResult Update()
		{
			return this.m_logik.Update();
		}

		public float GetVolume()
		{
			return this.m_logik.getVolume();
		}

		public void SetVolume(float volume)
		{
			this.m_logik.setVolume(volume);
		}

		public void SetPaused(bool setPaused)
		{
			this.m_logik.setPaused(setPaused);
		}

		public float GetCurrentIntensity()
		{
			return this.m_logik.getCurrentIntensity();
		}

		public PsaiInfo GetPsaiInfo()
		{
			return this.m_logik.getPsaiInfo();
		}

		public SoundtrackInfo GetSoundtrackInfo()
		{
			return this.m_logik.m_soundtrack.getSoundtrackInfo();
		}

		public ThemeInfo GetThemeInfo(int themeId)
		{
			return this.m_logik.m_soundtrack.getThemeInfo(themeId);
		}

		public SegmentInfo GetSegmentInfo(int segmentId)
		{
			return this.m_logik.m_soundtrack.getSegmentInfo(segmentId);
		}

		public int GetCurrentSegmentId()
		{
			return this.m_logik.getCurrentSnippetId();
		}

		public int GetCurrentThemeId()
		{
			return this.m_logik.getEffectiveThemeId();
		}

		public int GetRemainingMillisecondsOfCurrentSegmentPlayback()
		{
			return this.m_logik.GetRemainingMillisecondsOfCurrentSegmentPlayback();
		}

		public int GetRemainingMillisecondsUntilNextSegmentStart()
		{
			return this.m_logik.GetRemainingMillisecondsUntilNextSegmentStart();
		}

		public PsaiResult MenuModeEnter(int menuThemeId, float menuThemeIntensity)
		{
			return this.m_logik.MenuModeEnter(menuThemeId, menuThemeIntensity);
		}

		public PsaiResult MenuModeLeave()
		{
			return this.m_logik.MenuModeLeave();
		}

		public bool MenuModeIsActive()
		{
			return this.m_logik.menuModeIsActive();
		}

		public bool CutSceneIsActive()
		{
			return this.m_logik.cutSceneIsActive();
		}

		public PsaiResult CutSceneEnter(int themeId, float intensity)
		{
			return this.m_logik.CutSceneEnter(themeId, intensity);
		}

		public PsaiResult CutSceneLeave(bool immediately, bool reset)
		{
			return this.m_logik.CutSceneLeave(immediately, reset);
		}

		public PsaiResult PlaySegment(int segmentId)
		{
			return this.m_logik.PlaySegmentLayeredAndImmediately(segmentId);
		}

		public bool CheckIfAtLeastOneDirectTransitionOrLayeringIsPossible(int sourceSegmentId, int targetThemeId)
		{
			return this.m_logik.CheckIfAtLeastOneDirectTransitionOrLayeringIsPossible(sourceSegmentId, targetThemeId);
		}

		public void SetLastBasicMood(int themeId)
		{
			this.m_logik.SetLastBasicMood(themeId);
		}

		public void Release()
		{
			this.m_logik.Release();
			this.m_logik = null;
		}

		private Logik m_logik;

		private static PsaiCore s_singleton;
	}
}
