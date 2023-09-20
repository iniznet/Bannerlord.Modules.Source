using System;
using System.Collections.Generic;

namespace psai.net
{
	// Token: 0x02000021 RID: 33
	public class Soundtrack
	{
		// Token: 0x0600022B RID: 555 RVA: 0x0000941E File Offset: 0x0000761E
		public Soundtrack()
		{
			this.m_themes = new Dictionary<int, Theme>();
			this.m_snippets = new Dictionary<int, Segment>();
		}

		// Token: 0x0600022C RID: 556 RVA: 0x0000943C File Offset: 0x0000763C
		public void Clear()
		{
			this.m_themes.Clear();
			this.m_snippets.Clear();
		}

		// Token: 0x0600022D RID: 557 RVA: 0x00009454 File Offset: 0x00007654
		public Theme getThemeById(int id)
		{
			Theme theme;
			this.m_themes.TryGetValue(id, out theme);
			return theme;
		}

		// Token: 0x0600022E RID: 558 RVA: 0x00009474 File Offset: 0x00007674
		public Segment GetSegmentById(int id)
		{
			Segment segment;
			this.m_snippets.TryGetValue(id, out segment);
			return segment;
		}

		// Token: 0x0600022F RID: 559 RVA: 0x00009494 File Offset: 0x00007694
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

		// Token: 0x06000230 RID: 560 RVA: 0x00009524 File Offset: 0x00007724
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

		// Token: 0x06000231 RID: 561 RVA: 0x000095B0 File Offset: 0x000077B0
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

		// Token: 0x06000232 RID: 562 RVA: 0x0000964C File Offset: 0x0000784C
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

		// Token: 0x06000233 RID: 563 RVA: 0x00009714 File Offset: 0x00007914
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

		// Token: 0x04000134 RID: 308
		public Dictionary<int, Theme> m_themes;

		// Token: 0x04000135 RID: 309
		public Dictionary<int, Segment> m_snippets;
	}
}
