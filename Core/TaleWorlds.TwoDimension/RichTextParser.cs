using System;
using System.Collections.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.TwoDimension
{
	public class RichTextParser
	{
		public static List<TextToken> Parse(string text)
		{
			int i = 0;
			List<TextToken> list = new List<TextToken>(text.Length);
			while (i < text.Length)
			{
				char c = text[i];
				if (c != '\r')
				{
					if (c == ' ' || c == '\u3000')
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
					else if (c == '<')
					{
						int num = i;
						int num2 = -1;
						while (i < text.Length)
						{
							if (text[i] == '>')
							{
								num2 = i + 1;
								break;
							}
							i++;
						}
						RichTextTag richTextTag = RichTextTagParser.Parse(text, num, num2);
						if (richTextTag.Type == RichTextTagType.TextAfterError)
						{
							if (num2 == -1)
							{
								list.AddRange(TextToken.CreateTokenArrayFromWord(text.Substring(num, text.Length - num)));
							}
							else if (num2 > num)
							{
								list.AddRange(TextToken.CreateTokenArrayFromWord(text.Substring(num, num2 - num)));
							}
							else
							{
								Debug.FailedAssert("This shouldn't happen. Notify Emre", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\GauntletUI\\TaleWorlds.TwoDimension\\BitmapFont\\RichTextParser.cs", "Parse", 74);
							}
						}
						list.Add(TextToken.CreateTag(richTextTag));
					}
					else
					{
						list.Add(TextToken.CreateCharacter(c));
					}
				}
				i++;
			}
			return list;
		}
	}
}
