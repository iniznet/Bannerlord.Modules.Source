using System;
using System.Collections.Generic;

namespace TaleWorlds.TwoDimension
{
	// Token: 0x02000017 RID: 23
	public static class TextParser
	{
		// Token: 0x060000D0 RID: 208 RVA: 0x00006128 File Offset: 0x00004328
		public static List<TextToken> Parse(string text, ILanguage currentLanguage)
		{
			List<TextToken> list = new List<TextToken>(text.Length);
			foreach (char c in text)
			{
				if (c != '\r')
				{
					bool flag = currentLanguage.IsCharacterForbiddenAtEndOfLine(c);
					bool flag2 = currentLanguage.IsCharacterForbiddenAtStartOfLine(c);
					if (c == ' ')
					{
						list.Add(TextToken.CreateEmptyCharacter());
					}
					else if (c == '\t')
					{
						list.Add(TextToken.CreateTab());
					}
					else if (c == '\n')
					{
						list.Add(TextToken.CreateNewLine());
					}
					else if (c == '\u00a0' || c == '\u202f' || c == '\u2007')
					{
						list.Add(TextToken.CreateNonBreakingSpaceCharacter());
					}
					else if (c == '\u200b')
					{
						list.Add(TextToken.CreateZeroWidthSpaceCharacter());
					}
					else if (c == '\u2060')
					{
						list.Add(TextToken.CreateWordJoinerCharacter());
					}
					else
					{
						TextToken textToken = TextToken.CreateCharacter(c);
						textToken.CannotEndLineWithCharacter = flag;
						textToken.CannotStartLineWithCharacter = flag2;
						list.Add(textToken);
					}
				}
			}
			return list;
		}
	}
}
