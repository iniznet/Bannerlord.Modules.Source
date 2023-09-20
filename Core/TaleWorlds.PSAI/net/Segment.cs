using System;
using System.Collections.Generic;
using System.Text;

namespace psai.net
{
	public class Segment
	{
		public int Id { get; set; }

		public float Intensity { get; set; }

		public int ThemeId { get; set; }

		public string Name { get; set; }

		public int Playcount { get; set; }

		public int MaxPreBeatMsOfCompatibleSnippetsWithinSameTheme { get; set; }

		public List<Follower> Followers { get; private set; }

		public int SnippetTypeBitfield
		{
			get
			{
				return this._snippetTypeBitfield;
			}
			set
			{
				this._snippetTypeBitfield = value;
			}
		}

		public Segment()
		{
			this.Followers = new List<Follower>();
			this._mapDirectTransitionToThemeIsPossible = new Dictionary<int, bool>();
			this.MapOfNextTransitionSegmentToTheme = new Dictionary<int, Segment>();
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(this.Name);
			stringBuilder.Append(" (");
			stringBuilder.Append(this.Id);
			stringBuilder.Append(")");
			stringBuilder.Append(" ");
			stringBuilder.Append(Segment.GetStringFromSegmentSuitabilities(this.SnippetTypeBitfield));
			stringBuilder.Append(" [");
			stringBuilder.Append(this.Intensity.ToString("F2"));
			stringBuilder.Append("]");
			return stringBuilder.ToString();
		}

		public bool IsUsableAs(SegmentSuitability snippetType)
		{
			return (this.SnippetTypeBitfield & (int)snippetType) > 0;
		}

		public bool IsUsableOnlyAs(SegmentSuitability snippetType)
		{
			return (this.SnippetTypeBitfield & (int)snippetType) == (int)snippetType;
		}

		private void SetSnippetTypeFlag(SegmentSuitability snippetType)
		{
			this.SnippetTypeBitfield |= (int)snippetType;
		}

		private void ClearSnippetTypeFlag(SegmentSuitability snippetType)
		{
			this.SnippetTypeBitfield &= (int)(~(int)snippetType);
		}

		public Segment ReturnSegmentWithLowestIntensityDifference(List<Segment> argSnippets)
		{
			float num = 1f;
			Segment segment = null;
			for (int i = 0; i < argSnippets.Count; i++)
			{
				Segment segment2 = argSnippets[i];
				if (segment2 != this)
				{
					float num2 = Math.Abs(segment2.Intensity - this.Intensity);
					if (num2 == 0f)
					{
						return segment2;
					}
					if (num2 < num)
					{
						num = num2;
						segment = segment2;
					}
				}
			}
			return segment;
		}

		internal bool CheckIfAnyDirectOrIndirectTransitionIsPossible(Soundtrack soundtrack, int targetThemeId)
		{
			return this.CheckIfAtLeastOneDirectTransitionOrLayeringIsPossible(soundtrack, targetThemeId) || this.MapOfNextTransitionSegmentToTheme.ContainsKey(targetThemeId);
		}

		public bool CheckIfAtLeastOneDirectTransitionOrLayeringIsPossible(Soundtrack soundtrack, int targetThemeId)
		{
			bool flag;
			if (this._mapDirectTransitionToThemeIsPossible.TryGetValue(targetThemeId, out flag))
			{
				return flag;
			}
			foreach (Follower follower in this.Followers)
			{
				if (soundtrack.GetSegmentById(follower.snippetId).ThemeId == targetThemeId)
				{
					this._mapDirectTransitionToThemeIsPossible[targetThemeId] = true;
					return true;
				}
			}
			this._mapDirectTransitionToThemeIsPossible[targetThemeId] = false;
			return false;
		}

		public static string GetStringFromSegmentSuitabilities(int snippetTypeBitfield)
		{
			StringBuilder stringBuilder = new StringBuilder(20);
			stringBuilder.Append("[ ");
			if (snippetTypeBitfield == 0)
			{
				stringBuilder.Append("NULL ");
			}
			if ((snippetTypeBitfield & 1) > 0)
			{
				stringBuilder.Append("START ");
			}
			if ((snippetTypeBitfield & 2) > 0)
			{
				stringBuilder.Append("MID ");
			}
			if ((snippetTypeBitfield & 8) > 0)
			{
				stringBuilder.Append("BRIDGE ");
			}
			if ((snippetTypeBitfield & 4) > 0)
			{
				stringBuilder.Append("END ");
			}
			stringBuilder.Append("]");
			return stringBuilder.ToString();
		}

		public AudioData audioData;

		public Dictionary<int, Segment> MapOfNextTransitionSegmentToTheme;

		private Dictionary<int, bool> _mapDirectTransitionToThemeIsPossible;

		private int _snippetTypeBitfield;

		public Segment nextSnippetToShortestEndSequence;
	}
}
