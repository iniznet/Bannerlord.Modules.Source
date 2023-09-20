using System;
using System.Collections.Generic;

namespace TaleWorlds.TwoDimension
{
	internal class TextOutput
	{
		public float TextHeight
		{
			get
			{
				float num = 0f;
				for (int i = 0; i < this.LineCount; i++)
				{
					TextLineOutput line = this.GetLine(i);
					num += line.Height;
				}
				return num;
			}
		}

		public float TotalLineScale
		{
			get
			{
				float num = 0f;
				for (int i = 0; i < this.LineCount; i++)
				{
					TextLineOutput line = this.GetLine(i);
					num += line.MaxScale;
				}
				return num;
			}
		}

		public float LastLineWidth
		{
			get
			{
				return this._tokensWithLines[this._tokensWithLines.Count - 1].Width;
			}
		}

		public float MaxLineHeight { get; private set; }

		public float MaxLineWidth { get; private set; }

		public float MaxLineScale { get; private set; }

		public int LineCount
		{
			get
			{
				return this._tokensWithLines.Count;
			}
		}

		public TextOutput(float lineHeight)
		{
			this._tokensWithLines = new List<TextLineOutput>();
			this._lineHeight = lineHeight;
			TextLineOutput textLineOutput = new TextLineOutput(this._lineHeight);
			this._tokensWithLines.Add(textLineOutput);
			textLineOutput.LineEnded = true;
		}

		public TextLineOutput AddNewLine(bool currentLineEnded, float newLineBaseHeight = 0f)
		{
			TextLineOutput textLineOutput = this._tokensWithLines[this._tokensWithLines.Count - 1];
			textLineOutput.LineEnded = currentLineEnded;
			TextLineOutput textLineOutput2 = new TextLineOutput(newLineBaseHeight);
			this._tokensWithLines.Add(textLineOutput2);
			textLineOutput2.LineEnded = true;
			if (textLineOutput.Width > this.MaxLineWidth)
			{
				this.MaxLineWidth = textLineOutput.Width;
			}
			if (textLineOutput.MaxScale > this.MaxLineScale)
			{
				this.MaxLineScale = textLineOutput.MaxScale;
			}
			return textLineOutput2;
		}

		public void AddToken(TextToken textToken, float tokenWidth, float scaleValue, string style = "Default", float tokenHeight = -1f)
		{
			TextLineOutput textLineOutput = this._tokensWithLines[this._tokensWithLines.Count - 1];
			textLineOutput.AddToken(textToken, tokenWidth, tokenHeight, style, scaleValue);
			if (tokenHeight > this.MaxLineHeight)
			{
				this.MaxLineHeight = tokenHeight;
			}
			if (textLineOutput.Width > this.MaxLineWidth)
			{
				this.MaxLineWidth = textLineOutput.Width;
			}
			if (textLineOutput.MaxScale > this.MaxLineScale)
			{
				this.MaxLineScale = textLineOutput.MaxScale;
			}
		}

		public List<TextTokenOutput> RemoveTokensFromEnd(int numberOfTokensToRemove)
		{
			List<TextTokenOutput> list = new List<TextTokenOutput>();
			for (int i = 0; i < numberOfTokensToRemove; i++)
			{
				if (this._tokensWithLines[this._tokensWithLines.Count - 1].TokenCount > 0)
				{
					TextLineOutput textLineOutput = this._tokensWithLines[this._tokensWithLines.Count - 1];
					list.Add(textLineOutput.RemoveTokenFromEnd());
				}
				else
				{
					this._tokensWithLines.RemoveAt(this._tokensWithLines.Count - 1);
					TextLineOutput textLineOutput2 = this._tokensWithLines[this._tokensWithLines.Count - 1];
					list.Add(textLineOutput2.RemoveTokenFromEnd());
				}
			}
			return list;
		}

		public TextLineOutput GetLine(int i)
		{
			return this._tokensWithLines[i];
		}

		public IEnumerable<TextTokenOutput> Tokens
		{
			get
			{
				int num;
				for (int i = 0; i < this._tokensWithLines.Count; i = num + 1)
				{
					TextLineOutput tokensWithLine = this._tokensWithLines[i];
					for (int j = 0; j < tokensWithLine.TokenCount; j = num + 1)
					{
						yield return tokensWithLine.GetTokenOutput(j);
						num = j;
					}
					tokensWithLine = null;
					num = i;
				}
				yield break;
			}
		}

		public IEnumerable<TextTokenOutput> TokensWithNewLines
		{
			get
			{
				int num;
				for (int i = 0; i < this._tokensWithLines.Count; i = num + 1)
				{
					TextLineOutput tokensWithLine = this._tokensWithLines[i];
					for (int j = 0; j < tokensWithLine.TokenCount; j = num + 1)
					{
						yield return tokensWithLine.GetTokenOutput(j);
						num = j;
					}
					if (i < this._tokensWithLines.Count - 1)
					{
						yield return new TextTokenOutput(TextToken.CreateNewLine(), 0f, 0f, string.Empty, 0f);
					}
					tokensWithLine = null;
					num = i;
				}
				yield break;
			}
		}

		public void Clear()
		{
			this.MaxLineHeight = 0f;
			this.MaxLineWidth = 0f;
			this.MaxLineScale = 0f;
			this._tokensWithLines.Clear();
			TextLineOutput textLineOutput = new TextLineOutput(this._lineHeight);
			this._tokensWithLines.Add(textLineOutput);
			textLineOutput.LineEnded = true;
		}

		private List<TextLineOutput> _tokensWithLines;

		private readonly float _lineHeight;
	}
}
