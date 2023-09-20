using System;
using System.Collections.Generic;

namespace TaleWorlds.TwoDimension
{
	// Token: 0x0200000C RID: 12
	public class RichTextLinkGroup
	{
		// Token: 0x17000028 RID: 40
		// (get) Token: 0x06000072 RID: 114 RVA: 0x00004526 File Offset: 0x00002726
		// (set) Token: 0x06000073 RID: 115 RVA: 0x0000452E File Offset: 0x0000272E
		public string Href { get; private set; }

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x06000074 RID: 116 RVA: 0x00004537 File Offset: 0x00002737
		// (set) Token: 0x06000075 RID: 117 RVA: 0x0000453F File Offset: 0x0000273F
		internal int StartIndex { get; private set; }

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x06000076 RID: 118 RVA: 0x00004548 File Offset: 0x00002748
		internal int EndIndex
		{
			get
			{
				return this.StartIndex + this._tokens.Count;
			}
		}

		// Token: 0x06000077 RID: 119 RVA: 0x0000455C File Offset: 0x0000275C
		internal RichTextLinkGroup(int startIndex, string href)
		{
			this.Href = href;
			this.StartIndex = startIndex;
			this._tokens = new List<TextToken>();
		}

		// Token: 0x06000078 RID: 120 RVA: 0x0000457D File Offset: 0x0000277D
		internal void AddToken(TextToken textToken)
		{
			this._tokens.Add(textToken);
		}

		// Token: 0x06000079 RID: 121 RVA: 0x0000458B File Offset: 0x0000278B
		internal bool Contains(TextToken textToken)
		{
			return this._tokens.Contains(textToken);
		}

		// Token: 0x04000050 RID: 80
		private List<TextToken> _tokens;
	}
}
