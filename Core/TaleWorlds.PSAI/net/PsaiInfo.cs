using System;

namespace psai.net
{
	public struct PsaiInfo
	{
		public PsaiState psaiState { get; private set; }

		public PsaiState upcomingPsaiState { get; private set; }

		public int lastBasicMoodThemeId { get; private set; }

		public int effectiveThemeId { get; private set; }

		public int upcomingThemeId { get; private set; }

		public float currentIntensity { get; private set; }

		public float upcomingIntensity { get; private set; }

		public int themesQueued { get; private set; }

		public int targetSegmentId { get; private set; }

		public bool intensityIsHeld { get; private set; }

		public bool returningToLastBasicMood { get; private set; }

		public int remainingMillisecondsInRestMode { get; private set; }

		public bool paused { get; private set; }

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
