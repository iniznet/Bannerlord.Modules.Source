using System;
using System.Collections.Generic;

namespace TaleWorlds.TwoDimension
{
	internal class TextLineOutput
	{
		public float Width { get; private set; }

		public float TextWidth { get; private set; }

		public bool LineEnded { get; internal set; }

		public int EmptyCharacterCount { get; private set; }

		public int TokenCount
		{
			get
			{
				return this._tokens.Count;
			}
		}

		public float Height { get; private set; }

		public float MaxScale { get; private set; }

		public TextLineOutput(float lineHeight)
		{
			this._tokens = new List<TextTokenOutput>();
			this.Height = lineHeight;
		}

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

		public TextToken GetToken(int i)
		{
			return this._tokens[i].Token;
		}

		public TextTokenOutput GetTokenOutput(int i)
		{
			return this._tokens[i];
		}

		public TextTokenOutput RemoveTokenFromEnd()
		{
			TextTokenOutput textTokenOutput = this._tokens[this._tokens.Count - 1];
			this._tokens.Remove(textTokenOutput);
			this.Width -= textTokenOutput.Width;
			return textTokenOutput;
		}

		private List<TextTokenOutput> _tokens;
	}
}
