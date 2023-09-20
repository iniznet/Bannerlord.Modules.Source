using System;
using System.Collections.Generic;

namespace psai.net
{
	public class Soundtrack
	{
		public Soundtrack()
		{
			this.m_themes = new Dictionary<int, Theme>();
			this.m_snippets = new Dictionary<int, Segment>();
		}

		public void Clear()
		{
			this.m_themes.Clear();
			this.m_snippets.Clear();
		}

		public Theme getThemeById(int id)
		{
			Theme theme;
			this.m_themes.TryGetValue(id, out theme);
			return theme;
		}

		public Segment GetSegmentById(int id)
		{
			Segment segment;
			this.m_snippets.TryGetValue(id, out segment);
			return segment;
		}

		public SoundtrackInfo getSoundtrackInfo()
		{
			SoundtrackInfo soundtrackInfo = new SoundtrackInfo();
			soundtrackInfo.themeCount = this.m_themes.Count;
			soundtrackInfo.themeIds = new int[this.m_themes.Count];
			int num = 0;
			foreach (int num2 in this.m_themes.Keys)
			{
				soundtrackInfo.themeIds[num] = num2;
				num++;
			}
			return soundtrackInfo;
		}

		public ThemeInfo getThemeInfo(int themeId)
		{
			Theme themeById = this.getThemeById(themeId);
			if (themeById != null)
			{
				ThemeInfo themeInfo = new ThemeInfo();
				themeInfo.id = themeById.id;
				themeInfo.type = themeById.themeType;
				themeInfo.name = themeById.Name;
				themeInfo.segmentIds = new int[themeById.m_segments.Count];
				for (int i = 0; i < themeById.m_segments.Count; i++)
				{
					themeInfo.segmentIds[i] = themeById.m_segments[i].Id;
				}
				return themeInfo;
			}
			return null;
		}

		public SegmentInfo getSegmentInfo(int snippetId)
		{
			SegmentInfo segmentInfo = new SegmentInfo();
			Segment segmentById = this.GetSegmentById(snippetId);
			if (segmentById != null)
			{
				segmentInfo.id = segmentById.Id;
				segmentInfo.intensity = segmentById.Intensity;
				segmentInfo.segmentSuitabilitiesBitfield = segmentById.SnippetTypeBitfield;
				segmentInfo.themeId = segmentById.ThemeId;
				segmentInfo.playcount = segmentById.Playcount;
				segmentInfo.name = segmentById.Name;
				segmentInfo.fullLengthInMilliseconds = segmentById.audioData.GetFullLengthInMilliseconds();
				segmentInfo.preBeatLengthInMilliseconds = segmentById.audioData.GetPreBeatZoneInMilliseconds();
				segmentInfo.postBeatLengthInMilliseconds = segmentById.audioData.GetPostBeatZoneInMilliseconds();
			}
			return segmentInfo;
		}

		public void UpdateMaxPreBeatMsOfCompatibleMiddleOrBridgeSnippets()
		{
			foreach (Segment segment in this.m_snippets.Values)
			{
				segment.MaxPreBeatMsOfCompatibleSnippetsWithinSameTheme = 0;
				int count = segment.Followers.Count;
				for (int i = 0; i < count; i++)
				{
					int snippetId = segment.Followers[i].snippetId;
					Segment segmentById = this.GetSegmentById(snippetId);
					if (segmentById != null && (segmentById.SnippetTypeBitfield & 10) > 0)
					{
						int preBeatZoneInMilliseconds = segmentById.audioData.GetPreBeatZoneInMilliseconds();
						if (segment.MaxPreBeatMsOfCompatibleSnippetsWithinSameTheme < preBeatZoneInMilliseconds)
						{
							segment.MaxPreBeatMsOfCompatibleSnippetsWithinSameTheme = preBeatZoneInMilliseconds;
						}
					}
				}
			}
		}

		public void BuildAllIndirectionSequences()
		{
			foreach (Theme theme in this.m_themes.Values)
			{
				theme.BuildSequencesToEndSegmentForAllSnippets();
				foreach (Theme theme2 in this.m_themes.Values)
				{
					if (theme != theme2 && theme2.themeType != ThemeType.highlightLayer && Theme.ThemeInterruptionBehaviorRequiresEvaluationOfSegmentCompatibilities(Theme.GetThemeInterruptionBehavior(theme.themeType, theme2.themeType)))
					{
						theme.BuildSequencesToTargetThemeForAllSegments(this, theme2);
					}
				}
			}
		}

		public Dictionary<int, Theme> m_themes;

		public Dictionary<int, Segment> m_snippets;
	}
}
