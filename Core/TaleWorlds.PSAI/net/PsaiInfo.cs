using System;

namespace psai.net
{
	// Token: 0x02000018 RID: 24
	public struct PsaiInfo
	{
		// Token: 0x17000056 RID: 86
		// (get) Token: 0x060001BC RID: 444 RVA: 0x00008B75 File Offset: 0x00006D75
		// (set) Token: 0x060001BD RID: 445 RVA: 0x00008B7D File Offset: 0x00006D7D
		public PsaiState psaiState { get; private set; }

		// Token: 0x17000057 RID: 87
		// (get) Token: 0x060001BE RID: 446 RVA: 0x00008B86 File Offset: 0x00006D86
		// (set) Token: 0x060001BF RID: 447 RVA: 0x00008B8E File Offset: 0x00006D8E
		public PsaiState upcomingPsaiState { get; private set; }

		// Token: 0x17000058 RID: 88
		// (get) Token: 0x060001C0 RID: 448 RVA: 0x00008B97 File Offset: 0x00006D97
		// (set) Token: 0x060001C1 RID: 449 RVA: 0x00008B9F File Offset: 0x00006D9F
		public int lastBasicMoodThemeId { get; private set; }

		// Token: 0x17000059 RID: 89
		// (get) Token: 0x060001C2 RID: 450 RVA: 0x00008BA8 File Offset: 0x00006DA8
		// (set) Token: 0x060001C3 RID: 451 RVA: 0x00008BB0 File Offset: 0x00006DB0
		public int effectiveThemeId { get; private set; }

		// Token: 0x1700005A RID: 90
		// (get) Token: 0x060001C4 RID: 452 RVA: 0x00008BB9 File Offset: 0x00006DB9
		// (set) Token: 0x060001C5 RID: 453 RVA: 0x00008BC1 File Offset: 0x00006DC1
		public int upcomingThemeId { get; private set; }

		// Token: 0x1700005B RID: 91
		// (get) Token: 0x060001C6 RID: 454 RVA: 0x00008BCA File Offset: 0x00006DCA
		// (set) Token: 0x060001C7 RID: 455 RVA: 0x00008BD2 File Offset: 0x00006DD2
		public float currentIntensity { get; private set; }

		// Token: 0x1700005C RID: 92
		// (get) Token: 0x060001C8 RID: 456 RVA: 0x00008BDB File Offset: 0x00006DDB
		// (set) Token: 0x060001C9 RID: 457 RVA: 0x00008BE3 File Offset: 0x00006DE3
		public float upcomingIntensity { get; private set; }

		// Token: 0x1700005D RID: 93
		// (get) Token: 0x060001CA RID: 458 RVA: 0x00008BEC File Offset: 0x00006DEC
		// (set) Token: 0x060001CB RID: 459 RVA: 0x00008BF4 File Offset: 0x00006DF4
		public int themesQueued { get; private set; }

		// Token: 0x1700005E RID: 94
		// (get) Token: 0x060001CC RID: 460 RVA: 0x00008BFD File Offset: 0x00006DFD
		// (set) Token: 0x060001CD RID: 461 RVA: 0x00008C05 File Offset: 0x00006E05
		public int targetSegmentId { get; private set; }

		// Token: 0x1700005F RID: 95
		// (get) Token: 0x060001CE RID: 462 RVA: 0x00008C0E File Offset: 0x00006E0E
		// (set) Token: 0x060001CF RID: 463 RVA: 0x00008C16 File Offset: 0x00006E16
		public bool intensityIsHeld { get; private set; }

		// Token: 0x17000060 RID: 96
		// (get) Token: 0x060001D0 RID: 464 RVA: 0x00008C1F File Offset: 0x00006E1F
		// (set) Token: 0x060001D1 RID: 465 RVA: 0x00008C27 File Offset: 0x00006E27
		public bool returningToLastBasicMood { get; private set; }

		// Token: 0x17000061 RID: 97
		// (get) Token: 0x060001D2 RID: 466 RVA: 0x00008C30 File Offset: 0x00006E30
		// (set) Token: 0x060001D3 RID: 467 RVA: 0x00008C38 File Offset: 0x00006E38
		public int remainingMillisecondsInRestMode { get; private set; }

		// Token: 0x17000062 RID: 98
		// (get) Token: 0x060001D4 RID: 468 RVA: 0x00008C41 File Offset: 0x00006E41
		// (set) Token: 0x060001D5 RID: 469 RVA: 0x00008C49 File Offset: 0x00006E49
		public bool paused { get; private set; }

		// Token: 0x060001D6 RID: 470 RVA: 0x00008C54 File Offset: 0x00006E54
		public PsaiInfo(PsaiState m_psaiState, PsaiState m_upcomingPsaiState, int m_lastBasicMoodThemeId, int m_effectiveThemeId, int m_upcomingThemeId, float m_currentIntensity, float m_upcomingIntensity, int m_themesQueued, int m_targetSegmentId, bool m_intensityIsHeld, bool m_returningToLastBasicMood, int m_remainingMillisecondsInRestMode, bool m_paused)
		{
			this.psaiState = m_psaiState;
			this.upcomingPsaiState = m_upcomingPsaiState;
			this.lastBasicMoodThemeId = m_lastBasicMoodThemeId;
			this.effectiveThemeId = m_effectiveThemeId;
			this.upcomingThemeId = m_upcomingThemeId;
			this.currentIntensity = m_currentIntensity;
			this.upcomingIntensity = m_upcomingIntensity;
			this.themesQueued = m_themesQueued;
			this.targetSegmentId = m_targetSegmentId;
			this.intensityIsHeld = m_intensityIsHeld;
			this.returningToLastBasicMood = m_returningToLastBasicMood;
			this.remainingMillisecondsInRestMode = m_remainingMillisecondsInRestMode;
			this.paused = m_paused;
		}
	}
}
