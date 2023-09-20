using System;
using System.Collections.Generic;
using System.Text;

namespace psai.net
{
	// Token: 0x02000020 RID: 32
	public class Segment
	{
		// Token: 0x17000066 RID: 102
		// (get) Token: 0x06000211 RID: 529 RVA: 0x000090E3 File Offset: 0x000072E3
		// (set) Token: 0x06000212 RID: 530 RVA: 0x000090EB File Offset: 0x000072EB
		public int Id { get; set; }

		// Token: 0x17000067 RID: 103
		// (get) Token: 0x06000213 RID: 531 RVA: 0x000090F4 File Offset: 0x000072F4
		// (set) Token: 0x06000214 RID: 532 RVA: 0x000090FC File Offset: 0x000072FC
		public float Intensity { get; set; }

		// Token: 0x17000068 RID: 104
		// (get) Token: 0x06000215 RID: 533 RVA: 0x00009105 File Offset: 0x00007305
		// (set) Token: 0x06000216 RID: 534 RVA: 0x0000910D File Offset: 0x0000730D
		public int ThemeId { get; set; }

		// Token: 0x17000069 RID: 105
		// (get) Token: 0x06000217 RID: 535 RVA: 0x00009116 File Offset: 0x00007316
		// (set) Token: 0x06000218 RID: 536 RVA: 0x0000911E File Offset: 0x0000731E
		public string Name { get; set; }

		// Token: 0x1700006A RID: 106
		// (get) Token: 0x06000219 RID: 537 RVA: 0x00009127 File Offset: 0x00007327
		// (set) Token: 0x0600021A RID: 538 RVA: 0x0000912F File Offset: 0x0000732F
		public int Playcount { get; set; }

		// Token: 0x1700006B RID: 107
		// (get) Token: 0x0600021B RID: 539 RVA: 0x00009138 File Offset: 0x00007338
		// (set) Token: 0x0600021C RID: 540 RVA: 0x00009140 File Offset: 0x00007340
		public int MaxPreBeatMsOfCompatibleSnippetsWithinSameTheme { get; set; }

		// Token: 0x1700006C RID: 108
		// (get) Token: 0x0600021D RID: 541 RVA: 0x00009149 File Offset: 0x00007349
		// (set) Token: 0x0600021E RID: 542 RVA: 0x00009151 File Offset: 0x00007351
		public List<Follower> Followers { get; private set; }

		// Token: 0x1700006D RID: 109
		// (get) Token: 0x0600021F RID: 543 RVA: 0x0000915A File Offset: 0x0000735A
		// (set) Token: 0x06000220 RID: 544 RVA: 0x00009162 File Offset: 0x00007362
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

		// Token: 0x06000221 RID: 545 RVA: 0x0000916B File Offset: 0x0000736B
		public Segment()
		{
			this.Followers = new List<Follower>();
			this._mapDirectTransitionToThemeIsPossible = new Dictionary<int, bool>();
			this.MapOfNextTransitionSegmentToTheme = new Dictionary<int, Segment>();
		}

		// Token: 0x06000222 RID: 546 RVA: 0x00009194 File Offset: 0x00007394
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

		// Token: 0x06000223 RID: 547 RVA: 0x0000922D File Offset: 0x0000742D
		public bool IsUsableAs(SegmentSuitability snippetType)
		{
			return (this.SnippetTypeBitfield & (int)snippetType) > 0;
		}

		// Token: 0x06000224 RID: 548 RVA: 0x0000923A File Offset: 0x0000743A
		public bool IsUsableOnlyAs(SegmentSuitability snippetType)
		{
			return (this.SnippetTypeBitfield & (int)snippetType) == (int)snippetType;
		}

		// Token: 0x06000225 RID: 549 RVA: 0x00009248 File Offset: 0x00007448
		private void SetSnippetTypeFlag(SegmentSuitability snippetType)
		{
			this.SnippetTypeBitfield |= (int)snippetType;
		}

		// Token: 0x06000226 RID: 550 RVA: 0x00009268 File Offset: 0x00007468
		private void ClearSnippetTypeFlag(SegmentSuitability snippetType)
		{
			this.SnippetTypeBitfield &= (int)(~(int)snippetType);
		}

		// Token: 0x06000227 RID: 551 RVA: 0x00009288 File Offset: 0x00007488
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

		// Token: 0x06000228 RID: 552 RVA: 0x000092E4 File Offset: 0x000074E4
		internal bool CheckIfAnyDirectOrIndirectTransitionIsPossible(Soundtrack soundtrack, int targetThemeId)
		{
			return this.CheckIfAtLeastOneDirectTransitionOrLayeringIsPossible(soundtrack, targetThemeId) || this.MapOfNextTransitionSegmentToTheme.ContainsKey(targetThemeId);
		}

		// Token: 0x06000229 RID: 553 RVA: 0x00009300 File Offset: 0x00007500
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

		// Token: 0x0600022A RID: 554 RVA: 0x00009394 File Offset: 0x00007594
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

		// Token: 0x04000128 RID: 296
		public AudioData audioData;

		// Token: 0x04000130 RID: 304
		public Dictionary<int, Segment> MapOfNextTransitionSegmentToTheme;

		// Token: 0x04000131 RID: 305
		private Dictionary<int, bool> _mapDirectTransitionToThemeIsPossible;

		// Token: 0x04000132 RID: 306
		private int _snippetTypeBitfield;

		// Token: 0x04000133 RID: 307
		public Segment nextSnippetToShortestEndSequence;
	}
}
