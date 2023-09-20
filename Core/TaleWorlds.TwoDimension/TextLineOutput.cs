using System;
using System.Collections.Generic;

namespace TaleWorlds.TwoDimension
{
	// Token: 0x02000014 RID: 20
	internal class TextLineOutput
	{
		// Token: 0x1700003A RID: 58
		// (get) Token: 0x060000A6 RID: 166 RVA: 0x000058AA File Offset: 0x00003AAA
		// (set) Token: 0x060000A7 RID: 167 RVA: 0x000058B2 File Offset: 0x00003AB2
		public float Width { get; private set; }

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x060000A8 RID: 168 RVA: 0x000058BB File Offset: 0x00003ABB
		// (set) Token: 0x060000A9 RID: 169 RVA: 0x000058C3 File Offset: 0x00003AC3
		public float TextWidth { get; private set; }

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x060000AA RID: 170 RVA: 0x000058CC File Offset: 0x00003ACC
		// (set) Token: 0x060000AB RID: 171 RVA: 0x000058D4 File Offset: 0x00003AD4
		public bool LineEnded { get; internal set; }

		// Token: 0x1700003D RID: 61
		// (get) Token: 0x060000AC RID: 172 RVA: 0x000058DD File Offset: 0x00003ADD
		// (set) Token: 0x060000AD RID: 173 RVA: 0x000058E5 File Offset: 0x00003AE5
		public int EmptyCharacterCount { get; private set; }

		// Token: 0x1700003E RID: 62
		// (get) Token: 0x060000AE RID: 174 RVA: 0x000058EE File Offset: 0x00003AEE
		public int TokenCount
		{
			get
			{
				return this._tokens.Count;
			}
		}

		// Token: 0x1700003F RID: 63
		// (get) Token: 0x060000AF RID: 175 RVA: 0x000058FB File Offset: 0x00003AFB
		// (set) Token: 0x060000B0 RID: 176 RVA: 0x00005903 File Offset: 0x00003B03
		public float Height { get; private set; }

		// Token: 0x17000040 RID: 64
		// (get) Token: 0x060000B1 RID: 177 RVA: 0x0000590C File Offset: 0x00003B0C
		// (set) Token: 0x060000B2 RID: 178 RVA: 0x00005914 File Offset: 0x00003B14
		public float MaxScale { get; private set; }

		// Token: 0x060000B3 RID: 179 RVA: 0x0000591D File Offset: 0x00003B1D
		public TextLineOutput(float lineHeight)
		{
			this._tokens = new List<TextTokenOutput>();
			this.Height = lineHeight;
		}

		// Token: 0x060000B4 RID: 180 RVA: 0x00005938 File Offset: 0x00003B38
		public void AddToken(TextToken textToken, float tokenWidth, float tokenHeight, string style, float scaleValue)
		{
			if (textToken.Type == TextToken.TokenType.EmptyCharacter)
			{
				int emptyCharacterCount = this.EmptyCharacterCount;
				this.EmptyCharacterCount = emptyCharacterCount + 1;
			}
			else
			{
				this.TextWidth += tokenWidth;
			}
			TextTokenOutput textTokenOutput;
			if (tokenHeight > 0f)
			{
				textTokenOutput = new TextTokenOutput(textToken, tokenWidth, tokenHeight, style, scaleValue);
			}
			else
			{
				textTokenOutput = new TextTokenOutput(textToken, tokenWidth, this.Height, style, scaleValue);
			}
			this._tokens.Add(textTokenOutput);
			this.Width += tokenWidth;
			if (tokenHeight > this.Height)
			{
				this.Height = tokenHeight;
			}
			if (scaleValue > this.MaxScale)
			{
				this.MaxScale = scaleValue;
			}
		}

		// Token: 0x060000B5 RID: 181 RVA: 0x000059D2 File Offset: 0x00003BD2
		public TextToken GetToken(int i)
		{
			return this._tokens[i].Token;
		}

		// Token: 0x060000B6 RID: 182 RVA: 0x000059E5 File Offset: 0x00003BE5
		public TextTokenOutput GetTokenOutput(int i)
		{
			return this._tokens[i];
		}

		// Token: 0x060000B7 RID: 183 RVA: 0x000059F4 File Offset: 0x00003BF4
		public TextTokenOutput RemoveTokenFromEnd()
		{
			TextTokenOutput textTokenOutput = this._tokens[this._tokens.Count - 1];
			this._tokens.Remove(textTokenOutput);
			this.Width -= textTokenOutput.Width;
			return textTokenOutput;
		}

		// Token: 0x04000079 RID: 121
		private List<TextTokenOutput> _tokens;
	}
}
