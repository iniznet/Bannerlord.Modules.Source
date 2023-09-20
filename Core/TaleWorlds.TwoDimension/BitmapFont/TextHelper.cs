using System;
using System.Collections.Generic;

namespace TaleWorlds.TwoDimension.BitmapFont
{
	// Token: 0x0200003B RID: 59
	internal static class TextHelper
	{
		// Token: 0x0600028F RID: 655 RVA: 0x00009F68 File Offset: 0x00008168
		internal static int GetIndexOfFirstAppropriateCharacterToMoveToNextLineBackwardsFromIndex(List<TextToken> tokens, int startIndex, ILanguage currentLanguage)
		{
			if (!currentLanguage.DoesLanguageRequireSpaceForNewline())
			{
				for (int i = startIndex; i >= 1; i--)
				{
					if (!currentLanguage.IsCharacterForbiddenAtEndOfLine(tokens[i - 1].Token) && !currentLanguage.IsCharacterForbiddenAtStartOfLine(tokens[i].Token))
					{
						return i;
					}
				}
			}
			else
			{
				for (int j = startIndex; j >= 0; j--)
				{
					if (tokens[j].Type == TextToken.TokenType.EmptyCharacter || tokens[j].Type == TextToken.TokenType.ZeroWidthSpace)
					{
						return j + 1;
					}
				}
			}
			return -1;
		}

		// Token: 0x06000290 RID: 656 RVA: 0x00009FE8 File Offset: 0x000081E8
		internal static int GetIndexOfFirstAppropriateCharacterToMoveToNextLineForwardsFromIndex(List<TextToken> tokens, int startIndex, ILanguage currentLanguage)
		{
			if (!currentLanguage.DoesLanguageRequireSpaceForNewline())
			{
				for (int i = startIndex; i < tokens.Count; i++)
				{
					if (i > 0 && !currentLanguage.IsCharacterForbiddenAtEndOfLine(tokens[i - 1].Token) && !currentLanguage.IsCharacterForbiddenAtStartOfLine(tokens[i].Token))
					{
						return i;
					}
				}
			}
			else
			{
				for (int j = startIndex; j < tokens.Count; j++)
				{
					if (tokens[j].Type == TextToken.TokenType.EmptyCharacter || tokens[j].Type == TextToken.TokenType.ZeroWidthSpace)
					{
						return j + 1;
					}
				}
			}
			return -1;
		}

		// Token: 0x06000291 RID: 657 RVA: 0x0000A074 File Offset: 0x00008274
		internal static float GetTotalWordWidthBetweenIndices(int startIndex, int endIndex, List<TextToken> tokens, Func<TextToken, Font> getFontForToken, float extraPadding, float scale)
		{
			float num = 0f;
			for (int i = startIndex; i < endIndex; i++)
			{
				float num2 = num;
				Font font = getFontForToken(tokens[i]);
				num = num2 + ((font != null) ? (font.GetCharacterWidth(tokens[i].Token, extraPadding) * scale) : 0f);
			}
			return num;
		}
	}
}
