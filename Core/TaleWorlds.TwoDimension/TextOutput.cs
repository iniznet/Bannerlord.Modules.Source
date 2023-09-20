using System;
using System.Collections.Generic;

namespace TaleWorlds.TwoDimension
{
	// Token: 0x02000016 RID: 22
	internal class TextOutput
	{
		// Token: 0x17000041 RID: 65
		// (get) Token: 0x060000BE RID: 190 RVA: 0x00005DEC File Offset: 0x00003FEC
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

		// Token: 0x17000042 RID: 66
		// (get) Token: 0x060000BF RID: 191 RVA: 0x00005E24 File Offset: 0x00004024
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

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x060000C0 RID: 192 RVA: 0x00005E5A File Offset: 0x0000405A
		public float LastLineWidth
		{
			get
			{
				return this._tokensWithLines[this._tokensWithLines.Count - 1].Width;
			}
		}

		// Token: 0x17000044 RID: 68
		// (get) Token: 0x060000C1 RID: 193 RVA: 0x00005E79 File Offset: 0x00004079
		// (set) Token: 0x060000C2 RID: 194 RVA: 0x00005E81 File Offset: 0x00004081
		public float MaxLineHeight { get; private set; }

		// Token: 0x17000045 RID: 69
		// (get) Token: 0x060000C3 RID: 195 RVA: 0x00005E8A File Offset: 0x0000408A
		// (set) Token: 0x060000C4 RID: 196 RVA: 0x00005E92 File Offset: 0x00004092
		public float MaxLineWidth { get; private set; }

		// Token: 0x17000046 RID: 70
		// (get) Token: 0x060000C5 RID: 197 RVA: 0x00005E9B File Offset: 0x0000409B
		// (set) Token: 0x060000C6 RID: 198 RVA: 0x00005EA3 File Offset: 0x000040A3
		public float MaxLineScale { get; private set; }

		// Token: 0x17000047 RID: 71
		// (get) Token: 0x060000C7 RID: 199 RVA: 0x00005EAC File Offset: 0x000040AC
		public int LineCount
		{
			get
			{
				return this._tokensWithLines.Count;
			}
		}

		// Token: 0x060000C8 RID: 200 RVA: 0x00005EBC File Offset: 0x000040BC
		public TextOutput(float lineHeight)
		{
			this._tokensWithLines = new List<TextLineOutput>();
			this._lineHeight = lineHeight;
			TextLineOutput textLineOutput = new TextLineOutput(this._lineHeight);
			this._tokensWithLines.Add(textLineOutput);
			textLineOutput.LineEnded = true;
		}

		// Token: 0x060000C9 RID: 201 RVA: 0x00005F00 File Offset: 0x00004100
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

		// Token: 0x060000CA RID: 202 RVA: 0x00005F7C File Offset: 0x0000417C
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

		// Token: 0x060000CB RID: 203 RVA: 0x00005FF8 File Offset: 0x000041F8
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

		// Token: 0x060000CC RID: 204 RVA: 0x000060A2 File Offset: 0x000042A2
		public TextLineOutput GetLine(int i)
		{
			return this._tokensWithLines[i];
		}

		// Token: 0x17000048 RID: 72
		// (get) Token: 0x060000CD RID: 205 RVA: 0x000060B0 File Offset: 0x000042B0
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

		// Token: 0x17000049 RID: 73
		// (get) Token: 0x060000CE RID: 206 RVA: 0x000060C0 File Offset: 0x000042C0
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

		// Token: 0x060000CF RID: 207 RVA: 0x000060D0 File Offset: 0x000042D0
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

		// Token: 0x04000085 RID: 133
		private List<TextLineOutput> _tokensWithLines;

		// Token: 0x04000086 RID: 134
		private readonly float _lineHeight;
	}
}
