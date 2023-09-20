using System;

namespace psai.net
{
	// Token: 0x0200001C RID: 28
	public class PsaiCore
	{
		// Token: 0x17000063 RID: 99
		// (get) Token: 0x060001DB RID: 475 RVA: 0x00008D35 File Offset: 0x00006F35
		// (set) Token: 0x060001DC RID: 476 RVA: 0x00008D4D File Offset: 0x00006F4D
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

		// Token: 0x060001DD RID: 477 RVA: 0x00008D55 File Offset: 0x00006F55
		public static bool IsInstanceInitialized()
		{
			return PsaiCore.s_singleton != null;
		}

		// Token: 0x060001DE RID: 478 RVA: 0x00008D5F File Offset: 0x00006F5F
		public PsaiCore()
		{
			this.m_logik = Logik.Instance;
		}

		// Token: 0x060001DF RID: 479 RVA: 0x00008D72 File Offset: 0x00006F72
		public PsaiResult SetMaximumLatencyNeededByPlatformToBufferSounddata(int latencyInMilliseconds)
		{
			return this.m_logik.SetMaximumLatencyNeededByPlatformToBufferSounddata(latencyInMilliseconds);
		}

		// Token: 0x060001E0 RID: 480 RVA: 0x00008D80 File Offset: 0x00006F80
		public PsaiResult SetMaximumLatencyNeededByPlatformToPlayBackBufferedSounddata(int latencyInMilliseconds)
		{
			return this.m_logik.SetMaximumLatencyNeededByPlatformToPlayBackBufferedSounds(latencyInMilliseconds);
		}

		// Token: 0x060001E1 RID: 481 RVA: 0x00008D8E File Offset: 0x00006F8E
		public PsaiResult LoadSoundtrack(string pathToPcbFile)
		{
			return this.m_logik.LoadSoundtrack(pathToPcbFile);
		}

		// Token: 0x060001E2 RID: 482 RVA: 0x00008D9C File Offset: 0x00006F9C
		public PsaiResult LoadSoundtrackFromProjectFile(string pathToProjectFile)
		{
			return this.m_logik.LoadSoundtrackFromProjectFile(pathToProjectFile);
		}

		// Token: 0x060001E3 RID: 483 RVA: 0x00008DAA File Offset: 0x00006FAA
		public PsaiResult TriggerMusicTheme(int themeId, float intensity)
		{
			return this.m_logik.TriggerMusicTheme(themeId, intensity);
		}

		// Token: 0x060001E4 RID: 484 RVA: 0x00008DB9 File Offset: 0x00006FB9
		public PsaiResult TriggerMusicTheme(int themeId, float intensity, int musicDurationInSeconds)
		{
			return this.m_logik.TriggerMusicTheme(themeId, intensity, musicDurationInSeconds);
		}

		// Token: 0x060001E5 RID: 485 RVA: 0x00008DC9 File Offset: 0x00006FC9
		public PsaiResult AddToCurrentIntensity(float deltaIntensity)
		{
			return this.m_logik.AddToCurrentIntensity(deltaIntensity, false);
		}

		// Token: 0x060001E6 RID: 486 RVA: 0x00008DD8 File Offset: 0x00006FD8
		public PsaiResult StopMusic(bool immediately)
		{
			return this.m_logik.StopMusic(immediately);
		}

		// Token: 0x060001E7 RID: 487 RVA: 0x00008DE6 File Offset: 0x00006FE6
		public PsaiResult StopMusic(bool immediately, float fadeOutSeconds)
		{
			return this.m_logik.StopMusic(immediately, (int)(fadeOutSeconds * 1000f));
		}

		// Token: 0x060001E8 RID: 488 RVA: 0x00008DFC File Offset: 0x00006FFC
		public PsaiResult ReturnToLastBasicMood(bool immediately)
		{
			return this.m_logik.ReturnToLastBasicMood(immediately);
		}

		// Token: 0x060001E9 RID: 489 RVA: 0x00008E0A File Offset: 0x0000700A
		public PsaiResult GoToRest(bool immediately, float fadeOutSeconds)
		{
			return this.m_logik.GoToRest(immediately, (int)(fadeOutSeconds * 1000f));
		}

		// Token: 0x060001EA RID: 490 RVA: 0x00008E20 File Offset: 0x00007020
		public PsaiResult GoToRest(bool immediately, float fadeOutSeconds, int restTimeMin, int restTimeMax)
		{
			return this.m_logik.GoToRest(immediately, (int)(fadeOutSeconds * 1000f), restTimeMin, restTimeMax);
		}

		// Token: 0x060001EB RID: 491 RVA: 0x00008E39 File Offset: 0x00007039
		public PsaiResult HoldCurrentIntensity(bool hold)
		{
			return this.m_logik.HoldCurrentIntensity(hold);
		}

		// Token: 0x060001EC RID: 492 RVA: 0x00008E47 File Offset: 0x00007047
		public string GetVersion()
		{
			return this.m_logik.getVersion();
		}

		// Token: 0x060001ED RID: 493 RVA: 0x00008E54 File Offset: 0x00007054
		public PsaiResult Update()
		{
			return this.m_logik.Update();
		}

		// Token: 0x060001EE RID: 494 RVA: 0x00008E61 File Offset: 0x00007061
		public float GetVolume()
		{
			return this.m_logik.getVolume();
		}

		// Token: 0x060001EF RID: 495 RVA: 0x00008E6E File Offset: 0x0000706E
		public void SetVolume(float volume)
		{
			this.m_logik.setVolume(volume);
		}

		// Token: 0x060001F0 RID: 496 RVA: 0x00008E7C File Offset: 0x0000707C
		public void SetPaused(bool setPaused)
		{
			this.m_logik.setPaused(setPaused);
		}

		// Token: 0x060001F1 RID: 497 RVA: 0x00008E8B File Offset: 0x0000708B
		public float GetCurrentIntensity()
		{
			return this.m_logik.getCurrentIntensity();
		}

		// Token: 0x060001F2 RID: 498 RVA: 0x00008E98 File Offset: 0x00007098
		public PsaiInfo GetPsaiInfo()
		{
			return this.m_logik.getPsaiInfo();
		}

		// Token: 0x060001F3 RID: 499 RVA: 0x00008EA5 File Offset: 0x000070A5
		public SoundtrackInfo GetSoundtrackInfo()
		{
			return this.m_logik.m_soundtrack.getSoundtrackInfo();
		}

		// Token: 0x060001F4 RID: 500 RVA: 0x00008EB7 File Offset: 0x000070B7
		public ThemeInfo GetThemeInfo(int themeId)
		{
			return this.m_logik.m_soundtrack.getThemeInfo(themeId);
		}

		// Token: 0x060001F5 RID: 501 RVA: 0x00008ECA File Offset: 0x000070CA
		public SegmentInfo GetSegmentInfo(int segmentId)
		{
			return this.m_logik.m_soundtrack.getSegmentInfo(segmentId);
		}

		// Token: 0x060001F6 RID: 502 RVA: 0x00008EDD File Offset: 0x000070DD
		public int GetCurrentSegmentId()
		{
			return this.m_logik.getCurrentSnippetId();
		}

		// Token: 0x060001F7 RID: 503 RVA: 0x00008EEA File Offset: 0x000070EA
		public int GetCurrentThemeId()
		{
			return this.m_logik.getEffectiveThemeId();
		}

		// Token: 0x060001F8 RID: 504 RVA: 0x00008EF7 File Offset: 0x000070F7
		public int GetRemainingMillisecondsOfCurrentSegmentPlayback()
		{
			return this.m_logik.GetRemainingMillisecondsOfCurrentSegmentPlayback();
		}

		// Token: 0x060001F9 RID: 505 RVA: 0x00008F04 File Offset: 0x00007104
		public int GetRemainingMillisecondsUntilNextSegmentStart()
		{
			return this.m_logik.GetRemainingMillisecondsUntilNextSegmentStart();
		}

		// Token: 0x060001FA RID: 506 RVA: 0x00008F11 File Offset: 0x00007111
		public PsaiResult MenuModeEnter(int menuThemeId, float menuThemeIntensity)
		{
			return this.m_logik.MenuModeEnter(menuThemeId, menuThemeIntensity);
		}

		// Token: 0x060001FB RID: 507 RVA: 0x00008F20 File Offset: 0x00007120
		public PsaiResult MenuModeLeave()
		{
			return this.m_logik.MenuModeLeave();
		}

		// Token: 0x060001FC RID: 508 RVA: 0x00008F2D File Offset: 0x0000712D
		public bool MenuModeIsActive()
		{
			return this.m_logik.menuModeIsActive();
		}

		// Token: 0x060001FD RID: 509 RVA: 0x00008F3A File Offset: 0x0000713A
		public bool CutSceneIsActive()
		{
			return this.m_logik.cutSceneIsActive();
		}

		// Token: 0x060001FE RID: 510 RVA: 0x00008F47 File Offset: 0x00007147
		public PsaiResult CutSceneEnter(int themeId, float intensity)
		{
			return this.m_logik.CutSceneEnter(themeId, intensity);
		}

		// Token: 0x060001FF RID: 511 RVA: 0x00008F56 File Offset: 0x00007156
		public PsaiResult CutSceneLeave(bool immediately, bool reset)
		{
			return this.m_logik.CutSceneLeave(immediately, reset);
		}

		// Token: 0x06000200 RID: 512 RVA: 0x00008F65 File Offset: 0x00007165
		public PsaiResult PlaySegment(int segmentId)
		{
			return this.m_logik.PlaySegmentLayeredAndImmediately(segmentId);
		}

		// Token: 0x06000201 RID: 513 RVA: 0x00008F73 File Offset: 0x00007173
		public bool CheckIfAtLeastOneDirectTransitionOrLayeringIsPossible(int sourceSegmentId, int targetThemeId)
		{
			return this.m_logik.CheckIfAtLeastOneDirectTransitionOrLayeringIsPossible(sourceSegmentId, targetThemeId);
		}

		// Token: 0x06000202 RID: 514 RVA: 0x00008F82 File Offset: 0x00007182
		public void SetLastBasicMood(int themeId)
		{
			this.m_logik.SetLastBasicMood(themeId);
		}

		// Token: 0x06000203 RID: 515 RVA: 0x00008F90 File Offset: 0x00007190
		public void Release()
		{
			this.m_logik.Release();
			this.m_logik = null;
		}

		// Token: 0x04000118 RID: 280
		private Logik m_logik;

		// Token: 0x04000119 RID: 281
		private static PsaiCore s_singleton;
	}
}
