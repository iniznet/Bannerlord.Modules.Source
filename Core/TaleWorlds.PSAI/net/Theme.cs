using System;
using System.Collections.Generic;
using System.Text;

namespace psai.net
{
	public class Theme
	{
		public static bool ThemeInterruptionBehaviorRequiresEvaluationOfSegmentCompatibilities(ThemeInterruptionBehavior interruptionBehavior)
		{
			return interruptionBehavior == ThemeInterruptionBehavior.immediately || interruptionBehavior == ThemeInterruptionBehavior.at_end_of_current_snippet || interruptionBehavior == ThemeInterruptionBehavior.layer || interruptionBehavior == ThemeInterruptionBehavior.never;
		}

		public static string ThemeTypeToString(ThemeType themeType)
		{
			switch (themeType)
			{
			case ThemeType.basicMood:
				return "Basic Mood";
			case ThemeType.basicMoodAlt:
				return "Mood Alteration";
			case ThemeType.action:
				return "Action";
			case ThemeType.shock:
				return "Shock";
			case ThemeType.highlightLayer:
				return "Highlight Layer";
			case ThemeType.dramaticEvent:
				return "Dramatic Event";
			}
			return "";
		}

		public static ThemeInterruptionBehavior GetThemeInterruptionBehavior(ThemeType sourceThemeType, ThemeType targetThemeType)
		{
			switch (sourceThemeType)
			{
			case ThemeType.basicMood:
				switch (targetThemeType)
				{
				case ThemeType.basicMood:
					return ThemeInterruptionBehavior.at_end_of_current_snippet;
				case ThemeType.basicMoodAlt:
					return ThemeInterruptionBehavior.at_end_of_current_snippet;
				case ThemeType.action:
					return ThemeInterruptionBehavior.immediately;
				case ThemeType.shock:
					return ThemeInterruptionBehavior.immediately;
				case ThemeType.highlightLayer:
					return ThemeInterruptionBehavior.layer;
				case ThemeType.dramaticEvent:
					return ThemeInterruptionBehavior.immediately;
				}
				break;
			case ThemeType.basicMoodAlt:
				switch (targetThemeType)
				{
				case ThemeType.basicMood:
					return ThemeInterruptionBehavior.never;
				case ThemeType.basicMoodAlt:
					return ThemeInterruptionBehavior.at_end_of_current_snippet;
				case ThemeType.action:
					return ThemeInterruptionBehavior.immediately;
				case ThemeType.shock:
					return ThemeInterruptionBehavior.immediately;
				case ThemeType.highlightLayer:
					return ThemeInterruptionBehavior.layer;
				case ThemeType.dramaticEvent:
					return ThemeInterruptionBehavior.immediately;
				}
				break;
			case ThemeType.action:
				switch (targetThemeType)
				{
				case ThemeType.basicMood:
					return ThemeInterruptionBehavior.never;
				case ThemeType.basicMoodAlt:
					return ThemeInterruptionBehavior.never;
				case ThemeType.action:
					return ThemeInterruptionBehavior.at_end_of_current_snippet;
				case ThemeType.shock:
					return ThemeInterruptionBehavior.immediately;
				case ThemeType.highlightLayer:
					return ThemeInterruptionBehavior.layer;
				case ThemeType.dramaticEvent:
					return ThemeInterruptionBehavior.never;
				}
				break;
			case ThemeType.shock:
				switch (targetThemeType)
				{
				case ThemeType.basicMood:
					return ThemeInterruptionBehavior.never;
				case ThemeType.basicMoodAlt:
					return ThemeInterruptionBehavior.never;
				case ThemeType.action:
					return ThemeInterruptionBehavior.never;
				case ThemeType.shock:
					return ThemeInterruptionBehavior.immediately;
				case ThemeType.highlightLayer:
					return ThemeInterruptionBehavior.layer;
				case ThemeType.dramaticEvent:
					return ThemeInterruptionBehavior.never;
				}
				break;
			case ThemeType.highlightLayer:
				return ThemeInterruptionBehavior.never;
			case ThemeType.dramaticEvent:
				switch (targetThemeType)
				{
				case ThemeType.basicMood:
					return ThemeInterruptionBehavior.never;
				case ThemeType.basicMoodAlt:
					return ThemeInterruptionBehavior.never;
				case ThemeType.action:
					return ThemeInterruptionBehavior.immediately;
				case ThemeType.shock:
					return ThemeInterruptionBehavior.immediately;
				case ThemeType.highlightLayer:
					return ThemeInterruptionBehavior.layer;
				case ThemeType.dramaticEvent:
					return ThemeInterruptionBehavior.at_end_of_current_snippet;
				}
				break;
			}
			return ThemeInterruptionBehavior.undefined;
		}

		public Theme()
		{
			this.m_segments = new List<Segment>();
			this.weightings = new Weighting();
			this.id = -1;
			this.restSecondsMax = 0;
			this.restSecondsMin = 0;
			this.priority = 0;
			this.themeType = ThemeType.none;
			this.Name = "";
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(this.Name);
			stringBuilder.Append(" (");
			stringBuilder.Append(this.id);
			stringBuilder.Append(")");
			stringBuilder.Append(" [");
			stringBuilder.Append(this.themeType);
			stringBuilder.Append("]");
			return stringBuilder.ToString();
		}

		internal void BuildSequencesToEndSegmentForAllSnippets()
		{
			foreach (Segment segment in this.m_segments)
			{
				segment.nextSnippetToShortestEndSequence = null;
			}
			List<Segment> list = new List<Segment>();
			foreach (Segment segment2 in this.m_segments)
			{
				if ((segment2.SnippetTypeBitfield & 4) > 0)
				{
					list.Add(segment2);
				}
			}
			this.SetTheNextSnippetToShortestEndSequenceForAllSourceSnippetsOfTheSnippetsInThisList(list.ToArray());
		}

		private void SetTheNextSnippetToShortestEndSequenceForAllSourceSnippetsOfTheSnippetsInThisList(Segment[] listOfSnippetsWithValidEndSequences)
		{
			Dictionary<Segment, List<Segment>> dictionary = new Dictionary<Segment, List<Segment>>();
			foreach (Segment segment in listOfSnippetsWithValidEndSequences)
			{
				foreach (Segment segment2 in this.GetSetOfAllSourceSegmentsCompatibleToSegment(segment, 1f, SegmentSuitability.end))
				{
					if (segment2.nextSnippetToShortestEndSequence == null && segment2.ThemeId == segment.ThemeId)
					{
						List<Segment> list;
						if (dictionary.TryGetValue(segment2, out list))
						{
							list.Add(segment);
						}
						else
						{
							dictionary[segment2] = new List<Segment> { segment };
						}
					}
				}
			}
			foreach (Segment segment3 in dictionary.Keys)
			{
				segment3.nextSnippetToShortestEndSequence = segment3.ReturnSegmentWithLowestIntensityDifference(dictionary[segment3]);
			}
			Segment[] array = new Segment[dictionary.Count];
			dictionary.Keys.CopyTo(array, 0);
			if (array.Length != 0)
			{
				this.SetTheNextSnippetToShortestEndSequenceForAllSourceSnippetsOfTheSnippetsInThisList(array);
			}
		}

		internal void BuildSequencesToTargetThemeForAllSegments(Soundtrack soundtrack, Theme targetTheme)
		{
			foreach (Segment segment in this.m_segments)
			{
				segment.MapOfNextTransitionSegmentToTheme.Remove(targetTheme.id);
			}
			List<Segment> list = new List<Segment>();
			foreach (Segment segment2 in this.m_segments)
			{
				if (segment2.CheckIfAtLeastOneDirectTransitionOrLayeringIsPossible(soundtrack, targetTheme.id))
				{
					list.Add(segment2);
				}
			}
			this.SetTheNextSegmentToShortestTransitionSequenceToTargetThemeForAllSourceSegmentsOfTheSegmentsInThisList(list.ToArray(), soundtrack, targetTheme);
		}

		private List<Segment> GetSetOfAllSourceSegmentsCompatibleToSegment(Segment targetSnippet, float minCompatibilityThreshold, SegmentSuitability doNotIncludeSegmentsWithThisSuitability)
		{
			List<Segment> list = new List<Segment>();
			foreach (Segment segment in this.m_segments)
			{
				if (!segment.IsUsableAs(doNotIncludeSegmentsWithThisSuitability))
				{
					foreach (Follower follower in segment.Followers)
					{
						if (follower.snippetId == targetSnippet.Id && follower.compatibility >= minCompatibilityThreshold)
						{
							list.Add(segment);
						}
					}
				}
			}
			return list;
		}

		private void SetTheNextSegmentToShortestTransitionSequenceToTargetThemeForAllSourceSegmentsOfTheSegmentsInThisList(Segment[] listOfSnippetsWithValidTransitionSequencesToTargetTheme, Soundtrack soundtrack, Theme targetTheme)
		{
			Dictionary<Segment, List<Segment>> dictionary = new Dictionary<Segment, List<Segment>>();
			foreach (Segment segment in listOfSnippetsWithValidTransitionSequencesToTargetTheme)
			{
				List<Segment> setOfAllSourceSegmentsCompatibleToSegment = this.GetSetOfAllSourceSegmentsCompatibleToSegment(segment, 1f, SegmentSuitability.none);
				setOfAllSourceSegmentsCompatibleToSegment.Remove(segment);
				foreach (Segment segment2 in setOfAllSourceSegmentsCompatibleToSegment)
				{
					if (segment2.ThemeId == segment.ThemeId && !segment2.MapOfNextTransitionSegmentToTheme.ContainsKey(targetTheme.id) && !segment2.CheckIfAtLeastOneDirectTransitionOrLayeringIsPossible(soundtrack, targetTheme.id))
					{
						List<Segment> list;
						if (dictionary.TryGetValue(segment2, out list))
						{
							list.Add(segment);
						}
						else
						{
							dictionary[segment2] = new List<Segment> { segment };
						}
					}
				}
			}
			foreach (Segment segment3 in dictionary.Keys)
			{
				segment3.MapOfNextTransitionSegmentToTheme[targetTheme.id] = segment3.ReturnSegmentWithLowestIntensityDifference(dictionary[segment3]);
			}
			Segment[] array = new Segment[dictionary.Count];
			dictionary.Keys.CopyTo(array, 0);
			if (array.Length != 0)
			{
				this.SetTheNextSegmentToShortestTransitionSequenceToTargetThemeForAllSourceSegmentsOfTheSegmentsInThisList(array, soundtrack, targetTheme);
			}
		}

		public int id;

		public string Name;

		public ThemeType themeType;

		public int priority;

		public int restSecondsMax;

		public int restSecondsMin;

		public List<Segment> m_segments;

		public float intensityAfterRest;

		public int musicDurationGeneral;

		public int musicDurationAfterRest;

		public Weighting weightings;
	}
}
