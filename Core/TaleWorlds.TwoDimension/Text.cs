using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TaleWorlds.TwoDimension.BitmapFont;

namespace TaleWorlds.TwoDimension
{
	public class Text : IText
	{
		public ILanguage CurrentLanguage { get; set; }

		public float ScaleToFitTextInLayout { get; private set; } = 1f;

		public int LineCount { get; private set; }

		public DrawObject2D DrawObject2D
		{
			get
			{
				if (this._meshNeedsUpdate)
				{
					this.RecalculateTextMesh(1f);
					if (this.ScaleToFitTextInLayout != 1f)
					{
						this.RecalculateTextMesh(this.ScaleToFitTextInLayout);
					}
				}
				return this._drawObject2D;
			}
		}

		public Font Font
		{
			get
			{
				return this._font;
			}
			set
			{
				if (this._font != value)
				{
					this._meshNeedsUpdate = true;
					this._preferredSizeNeedsUpdate = true;
					this._font = value;
				}
			}
		}

		private float ExtraPaddingHorizontal
		{
			get
			{
				return 0.5f;
			}
		}

		private float ExtraPaddingVertical
		{
			get
			{
				return 5f;
			}
		}

		public TextHorizontalAlignment HorizontalAlignment
		{
			get
			{
				return this._horizontalAlignment;
			}
			set
			{
				if (this._horizontalAlignment != value)
				{
					this._horizontalAlignment = value;
					this._meshNeedsUpdate = true;
					this._preferredSizeNeedsUpdate = true;
				}
			}
		}

		public TextVerticalAlignment VerticalAlignment
		{
			get
			{
				return this._verticalAlignment;
			}
			set
			{
				if (this._verticalAlignment != value)
				{
					this._verticalAlignment = value;
					this._meshNeedsUpdate = true;
					this._preferredSizeNeedsUpdate = true;
				}
			}
		}

		public float FontSize
		{
			get
			{
				return (float)this._fontSize;
			}
			set
			{
				if (this._fontSize != (int)value)
				{
					this._fontSize = (int)value;
					this._meshNeedsUpdate = true;
					this._preferredSizeNeedsUpdate = true;
				}
			}
		}

		public string Value
		{
			get
			{
				return this._text;
			}
			set
			{
				string text = value;
				if (text == null)
				{
					text = "";
				}
				if (this._text != text)
				{
					this._text = text;
					this._tokens = TextParser.Parse(text, this.CurrentLanguage);
					this._meshNeedsUpdate = true;
					this._preferredSizeNeedsUpdate = true;
				}
			}
		}

		private float EmptyCharacterWidth
		{
			get
			{
				return ((float)this.Font.Characters[32].XAdvance + this.ExtraPaddingHorizontal) * this._scaleValue;
			}
		}

		private float LineHeight
		{
			get
			{
				return ((float)this.Font.Base + this.ExtraPaddingVertical) * this._scaleValue;
			}
		}

		public bool SkipLineOnContainerExceeded
		{
			get
			{
				return this._skipLineOnContainerExceeded;
			}
			set
			{
				if (value != this._skipLineOnContainerExceeded)
				{
					this._skipLineOnContainerExceeded = value;
					this._meshNeedsUpdate = true;
					this._preferredSizeNeedsUpdate = true;
				}
			}
		}

		public Text(int width, int height, Font bitmapFont, Func<int, Font> getUsableFontForCharacter)
		{
			this.Font = bitmapFont;
			this._width = width;
			this._height = height;
			this._getUsableFontForCharacter = getUsableFontForCharacter;
			this._meshNeedsUpdate = true;
			this._preferredSizeNeedsUpdate = true;
			this._text = "";
			this._fontSize = 32;
			this._tokens = null;
			this._textMeshGenerator = new TextMeshGenerator();
		}

		public Vector2 GetPreferredSize(bool fixedWidth, float widthSize, bool fixedHeight, float heightSize, SpriteData spriteData, float renderScale)
		{
			this._fixedWidth = fixedWidth;
			this._fixedHeight = fixedHeight;
			this._desiredHeight = heightSize;
			this._desiredWidth = widthSize;
			if (this._preferredSizeNeedsUpdate)
			{
				this._preferredSize = new Vector2(0f, 0f);
				if (this._fontSize != 0 && !string.IsNullOrEmpty(this._text))
				{
					this._scaleValue = (float)this._fontSize / (float)this.Font.Size;
					float num = 0f;
					this.LineCount = 1;
					float lineHeight = this.LineHeight;
					float emptyCharacterWidth = this.EmptyCharacterWidth;
					for (int i = 0; i < this._tokens.Count; i++)
					{
						TextToken textToken = this._tokens[i];
						if (textToken.Type == TextToken.TokenType.NewLine)
						{
							int num2 = this.LineCount;
							this.LineCount = num2 + 1;
							if (num > this._preferredSize.X)
							{
								this._preferredSize.X = num;
							}
							num = 0f;
						}
						else if (textToken.Type == TextToken.TokenType.EmptyCharacter || textToken.Type == TextToken.TokenType.NonBreakingSpace)
						{
							num += emptyCharacterWidth;
						}
						else if (textToken.Type == TextToken.TokenType.Character)
						{
							char token = textToken.Token;
							float num3 = this.Font.GetCharacterWidth(token, this.ExtraPaddingHorizontal) * this._scaleValue;
							if (fixedWidth && this._skipLineOnContainerExceeded)
							{
								if (num + num3 > widthSize && num > 0f)
								{
									int indexOfFirstAppropriateCharacterToMoveToNextLineBackwardsFromIndex = TextHelper.GetIndexOfFirstAppropriateCharacterToMoveToNextLineBackwardsFromIndex(this._tokens, i, this.CurrentLanguage);
									if (indexOfFirstAppropriateCharacterToMoveToNextLineBackwardsFromIndex == -1)
									{
										int num4 = Math.Max(0, this._tokens.Count - 2);
										int num5 = Math.Max(0, this._tokens.Count - 1);
										float totalWordWidthBetweenIndices = TextHelper.GetTotalWordWidthBetweenIndices(num4, num5, this._tokens, new Func<TextToken, Font>(this.GetFontForTextToken), this.ExtraPaddingHorizontal, this._scaleValue);
										int num2 = this.LineCount;
										this.LineCount = num2 + 1;
										num = totalWordWidthBetweenIndices + num3;
									}
									else
									{
										float totalWordWidthBetweenIndices2 = TextHelper.GetTotalWordWidthBetweenIndices(indexOfFirstAppropriateCharacterToMoveToNextLineBackwardsFromIndex, i, this._tokens, new Func<TextToken, Font>(this.GetFontForTextToken), this.ExtraPaddingHorizontal, this._scaleValue);
										if (num - totalWordWidthBetweenIndices2 > this._preferredSize.X)
										{
											this._preferredSize.X = num - totalWordWidthBetweenIndices2;
										}
										num = totalWordWidthBetweenIndices2 + num3;
										int num2 = this.LineCount;
										this.LineCount = num2 + 1;
									}
								}
								else
								{
									num += num3;
								}
							}
							else
							{
								num += num3;
							}
						}
					}
					if (num > this._preferredSize.X)
					{
						this._preferredSize.X = num;
					}
					this._preferredSize.Y = (float)this.LineCount * lineHeight;
				}
				this._preferredSize = new Vector2((float)Math.Ceiling((double)this._preferredSize.X) + 1f, (float)Math.Ceiling((double)this._preferredSize.Y) + 1f);
				this._preferredSizeNeedsUpdate = false;
			}
			return this._preferredSize;
		}

		public void UpdateSize(int width, int height)
		{
			if (this._width != width || this._height != height)
			{
				this._width = width;
				this._height = height;
				this._meshNeedsUpdate = true;
				this._preferredSizeNeedsUpdate = true;
				this.ScaleToFitTextInLayout = 1f;
			}
		}

		private Font GetFontForTextToken(TextToken token)
		{
			return this._getUsableFontForCharacter((int)token.Token);
		}

		private void RecalculateTextMesh(float customScaleToFitText = 1f)
		{
			if (this._fontSize == 0 || string.IsNullOrEmpty(this._text))
			{
				this._drawObject2D = null;
				return;
			}
			int num = this._text.Length;
			this._scaleValue = (float)this._fontSize / (float)this.Font.Size * customScaleToFitText;
			float num2 = 0f;
			float num3 = 0f;
			ref BitmapFontCharacter ptr = this.Font.Characters[32];
			float num4 = ((float)this.Font.Base + this.ExtraPaddingVertical) * this._scaleValue;
			float num5 = ((float)ptr.XAdvance + this.ExtraPaddingHorizontal) * this._scaleValue;
			TextOutput textOutput = new TextOutput(num4);
			for (int i = 0; i < this._tokens.Count; i++)
			{
				TextToken textToken = this._tokens[i];
				if (textToken.Type == TextToken.TokenType.NewLine)
				{
					textOutput.AddNewLine(true, 0f);
					num2 = 0f;
					num3 += num4;
				}
				else if (textToken.Type == TextToken.TokenType.EmptyCharacter || textToken.Type == TextToken.TokenType.NonBreakingSpace)
				{
					textOutput.AddToken(textToken, num5, this._scaleValue, "Default", -1f);
					num2 += num5;
				}
				else if (textToken.Type != TextToken.TokenType.ZeroWidthSpace)
				{
					if (textToken.Type == TextToken.TokenType.WordJoiner)
					{
						textOutput.AddToken(textToken, 0f, this._scaleValue, "Default", -1f);
					}
					else if (textToken.Type == TextToken.TokenType.Character)
					{
						char token = textToken.Token;
						float num6 = this.Font.GetCharacterWidth(token, this.ExtraPaddingHorizontal) * this._scaleValue;
						if (num6 == 0f)
						{
							Font font = this._getUsableFontForCharacter((int)token);
							num6 = (((font != null) ? new float?(font.GetCharacterWidth(token, this.ExtraPaddingHorizontal)) : null) * this._scaleValue) ?? 0f;
						}
						if (num2 + num6 > (float)this._width && num2 > 0f && this._skipLineOnContainerExceeded)
						{
							float num7 = num3 + num4;
							int height = this._height;
							int indexOfFirstAppropriateCharacterToMoveToNextLineBackwardsFromIndex = TextHelper.GetIndexOfFirstAppropriateCharacterToMoveToNextLineBackwardsFromIndex(this._tokens, i, this.CurrentLanguage);
							int num8 = TextHelper.GetIndexOfFirstAppropriateCharacterToMoveToNextLineForwardsFromIndex(this._tokens, i, this.CurrentLanguage);
							float num9 = 0f;
							if (indexOfFirstAppropriateCharacterToMoveToNextLineBackwardsFromIndex != -1)
							{
								if (num8 == -1)
								{
									num8 = this._tokens.Count;
								}
								num9 = TextHelper.GetTotalWordWidthBetweenIndices(indexOfFirstAppropriateCharacterToMoveToNextLineBackwardsFromIndex, num8, this._tokens, new Func<TextToken, Font>(this.GetFontForTextToken), this.ExtraPaddingHorizontal, this._scaleValue);
							}
							if (((num9 != 0f && (indexOfFirstAppropriateCharacterToMoveToNextLineBackwardsFromIndex == -1 || num9 > (float)this._width)) || indexOfFirstAppropriateCharacterToMoveToNextLineBackwardsFromIndex == -1) && textOutput.Tokens.Any<TextTokenOutput>())
							{
								List<TextTokenOutput> list = textOutput.RemoveTokensFromEnd(1);
								float num10 = this.Font.GetCharacterWidth(this.CurrentLanguage.GetLineSeperatorChar(), this.ExtraPaddingHorizontal) * this._scaleValue;
								textOutput.AddToken(TextToken.CreateCharacter(this.CurrentLanguage.GetLineSeperatorChar()), num10, this._scaleValue, "Default", -1f);
								textOutput.AddNewLine(false, 0f);
								num3 += num4;
								textOutput.AddToken(list[0].Token, list[0].Width, this._scaleValue, "Default", -1f);
								textOutput.AddToken(textToken, num6, this._scaleValue, "Default", -1f);
								num++;
								num2 = num6 + list[0].Width;
							}
							else
							{
								num2 = TextHelper.GetTotalWordWidthBetweenIndices(indexOfFirstAppropriateCharacterToMoveToNextLineBackwardsFromIndex, i, this._tokens, new Func<TextToken, Font>(this.GetFontForTextToken), this.ExtraPaddingHorizontal, this._scaleValue);
								List<TextTokenOutput> list2 = textOutput.RemoveTokensFromEnd(i - indexOfFirstAppropriateCharacterToMoveToNextLineBackwardsFromIndex);
								textOutput.AddNewLine(false, 0f);
								num3 += num4;
								for (int j = list2.Count - 1; j >= 0; j--)
								{
									TextTokenOutput textTokenOutput = list2[j];
									if (textTokenOutput.Token.Type != TextToken.TokenType.EmptyCharacter && textTokenOutput.Token.Type != TextToken.TokenType.ZeroWidthSpace)
									{
										textOutput.AddToken(textTokenOutput.Token, textTokenOutput.Width, this._scaleValue, "Default", -1f);
									}
								}
								textOutput.AddToken(textToken, num6, this._scaleValue, "Default", -1f);
								num2 += num6;
							}
						}
						else
						{
							textOutput.AddToken(textToken, num6, this._scaleValue, "Default", -1f);
							num2 += num6;
						}
					}
				}
			}
			this._textMeshGenerator.Refresh(this.Font, num, this._scaleValue);
			num2 = 0f;
			num3 = 0f;
			for (int k = 0; k < textOutput.LineCount; k++)
			{
				TextLineOutput line = textOutput.GetLine(k);
				float num11 = num5;
				switch (this._horizontalAlignment)
				{
				case TextHorizontalAlignment.Right:
					num2 = (float)this._width - line.Width;
					break;
				case TextHorizontalAlignment.Center:
				{
					float num12 = 0f;
					if (!line.LineEnded)
					{
						int num13 = 1;
						while (num13 < line.TokenCount && line.GetToken(line.TokenCount - num13).Type == TextToken.TokenType.EmptyCharacter)
						{
							num12 += num5;
							num13++;
						}
						num13 = 0;
						while (num13 < line.TokenCount && line.GetToken(num13).Type == TextToken.TokenType.EmptyCharacter)
						{
							num12 += num5;
							num13++;
						}
					}
					num2 = ((float)this._width - (line.Width - num12)) * 0.5f;
					break;
				}
				case TextHorizontalAlignment.Justify:
				{
					float num14 = (float)this._width - line.TextWidth;
					if (!line.LineEnded)
					{
						int num15 = line.EmptyCharacterCount;
						int num16 = 1;
						while (line.GetToken(line.TokenCount - num16).Type == TextToken.TokenType.EmptyCharacter)
						{
							num15--;
							num16++;
						}
						num16 = 0;
						while (line.GetToken(num16).Type == TextToken.TokenType.EmptyCharacter)
						{
							num15--;
							num16++;
						}
						num11 = num14 / (float)num15;
					}
					break;
				}
				}
				for (int l = 0; l < line.TokenCount; l++)
				{
					Font font2 = this.Font;
					TextToken token2 = line.GetToken(l);
					TextToken.TokenType type = token2.Type;
					if (type != TextToken.TokenType.EmptyCharacter && type != TextToken.TokenType.NonBreakingSpace)
					{
						if (type == TextToken.TokenType.Character)
						{
							int num17 = (int)token2.Token;
							if (!this.Font.Characters.ContainsKey(num17))
							{
								num17 = 0;
							}
							BitmapFontCharacter bitmapFontCharacter = font2.Characters[num17];
							float num18 = num2 + (float)bitmapFontCharacter.XOffset * this._scaleValue;
							float num19 = num3 + (float)bitmapFontCharacter.YOffset * this._scaleValue;
							this._textMeshGenerator.AddCharacterToMesh(num18, num19, bitmapFontCharacter);
							num2 += ((float)bitmapFontCharacter.XAdvance + this.ExtraPaddingHorizontal) * this._scaleValue;
						}
					}
					else
					{
						num2 += num11;
					}
				}
				num2 = 0f;
				num3 += num4;
			}
			if (this._verticalAlignment == TextVerticalAlignment.Center || this._verticalAlignment == TextVerticalAlignment.Bottom)
			{
				float num20;
				if (this._verticalAlignment == TextVerticalAlignment.Center)
				{
					num20 = (float)this._height - num3;
					num20 *= 0.5f;
				}
				else
				{
					num20 = (float)this._height - num3;
				}
				this._textMeshGenerator.AddValueToY(num20);
			}
			this._drawObject2D = this._textMeshGenerator.GenerateMesh();
			this._meshNeedsUpdate = false;
			if (this._fixedHeight && num3 > this._desiredHeight && this._desiredHeight > 1f)
			{
				this.ScaleToFitTextInLayout = this._desiredHeight / num3;
			}
			if (this._fixedWidth && num2 > this._desiredWidth && this._desiredWidth > 1f)
			{
				this.ScaleToFitTextInLayout = Math.Min(this.ScaleToFitTextInLayout, this._desiredWidth / num2);
			}
		}

		private TextHorizontalAlignment _horizontalAlignment;

		private TextVerticalAlignment _verticalAlignment;

		private DrawObject2D _drawObject2D;

		private bool _meshNeedsUpdate;

		private bool _preferredSizeNeedsUpdate;

		private bool _fixedHeight;

		private bool _fixedWidth;

		private float _desiredHeight;

		private float _desiredWidth;

		private Vector2 _preferredSize;

		private string _text;

		private List<TextToken> _tokens;

		private int _fontSize;

		private int _width;

		private int _height;

		private Font _font;

		private float _scaleValue;

		private readonly TextMeshGenerator _textMeshGenerator;

		private readonly Func<int, Font> _getUsableFontForCharacter;

		private bool _skipLineOnContainerExceeded = true;
	}
}
