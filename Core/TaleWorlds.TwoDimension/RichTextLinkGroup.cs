using System;
using System.Collections.Generic;

namespace TaleWorlds.TwoDimension
{
	public class RichTextLinkGroup
	{
		public string Href { get; private set; }

		internal int StartIndex { get; private set; }

		internal int EndIndex
		{
			get
			{
				return this.StartIndex + this._tokens.Count;
			}
		}

		internal RichTextLinkGroup(int startIndex, string href)
		{
			this.Href = href;
			this.StartIndex = startIndex;
			this._tokens = new List<TextToken>();
		}

		internal void AddToken(TextToken textToken)
		{
			this._tokens.Add(textToken);
		}

		internal bool Contains(TextToken textToken)
		{
			return this._tokens.Contains(textToken);
		}

		private List<TextToken> _tokens;
	}
}
