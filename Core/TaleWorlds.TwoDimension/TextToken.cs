using System;
using System.Collections.Generic;

namespace TaleWorlds.TwoDimension
{
	// Token: 0x02000018 RID: 24
	public class TextToken
	{
		// Token: 0x1700004A RID: 74
		// (get) Token: 0x060000D1 RID: 209 RVA: 0x00006226 File Offset: 0x00004426
		// (set) Token: 0x060000D2 RID: 210 RVA: 0x0000622E File Offset: 0x0000442E
		public char Token { get; private set; }

		// Token: 0x1700004B RID: 75
		// (get) Token: 0x060000D3 RID: 211 RVA: 0x00006237 File Offset: 0x00004437
		// (set) Token: 0x060000D4 RID: 212 RVA: 0x0000623F File Offset: 0x0000443F
		public TextToken.TokenType Type { get; private set; }

		// Token: 0x1700004C RID: 76
		// (get) Token: 0x060000D5 RID: 213 RVA: 0x00006248 File Offset: 0x00004448
		// (set) Token: 0x060000D6 RID: 214 RVA: 0x00006250 File Offset: 0x00004450
		public RichTextTag Tag { get; private set; }

		// Token: 0x1700004D RID: 77
		// (get) Token: 0x060000D7 RID: 215 RVA: 0x00006259 File Offset: 0x00004459
		// (set) Token: 0x060000D8 RID: 216 RVA: 0x00006261 File Offset: 0x00004461
		public bool CannotStartLineWithCharacter { get; set; }

		// Token: 0x1700004E RID: 78
		// (get) Token: 0x060000D9 RID: 217 RVA: 0x0000626A File Offset: 0x0000446A
		// (set) Token: 0x060000DA RID: 218 RVA: 0x00006272 File Offset: 0x00004472
		public bool CannotEndLineWithCharacter { get; set; }

		// Token: 0x060000DB RID: 219 RVA: 0x0000627B File Offset: 0x0000447B
		private TextToken(TextToken.TokenType type, char token)
		{
			this.Type = type;
			this.Token = token;
		}

		// Token: 0x060000DC RID: 220 RVA: 0x00006291 File Offset: 0x00004491
		private TextToken(RichTextTag tag)
		{
			this.Type = TextToken.TokenType.Tag;
			this.Tag = tag;
		}

		// Token: 0x060000DD RID: 221 RVA: 0x000062A7 File Offset: 0x000044A7
		public static TextToken CreateEmptyCharacter()
		{
			return new TextToken(TextToken.TokenType.EmptyCharacter, ' ');
		}

		// Token: 0x060000DE RID: 222 RVA: 0x000062B1 File Offset: 0x000044B1
		public static TextToken CreateZeroWidthSpaceCharacter()
		{
			return new TextToken(TextToken.TokenType.ZeroWidthSpace, '\0');
		}

		// Token: 0x060000DF RID: 223 RVA: 0x000062BA File Offset: 0x000044BA
		public static TextToken CreateNonBreakingSpaceCharacter()
		{
			return new TextToken(TextToken.TokenType.NonBreakingSpace, ' ');
		}

		// Token: 0x060000E0 RID: 224 RVA: 0x000062C4 File Offset: 0x000044C4
		public static TextToken CreateWordJoinerCharacter()
		{
			return new TextToken(TextToken.TokenType.WordJoiner, Convert.ToChar(8288));
		}

		// Token: 0x060000E1 RID: 225 RVA: 0x000062D6 File Offset: 0x000044D6
		public static TextToken CreateNewLine()
		{
			return new TextToken(TextToken.TokenType.NewLine, '\n');
		}

		// Token: 0x060000E2 RID: 226 RVA: 0x000062E0 File Offset: 0x000044E0
		public static TextToken CreateTab()
		{
			return new TextToken(TextToken.TokenType.Tab, '\t');
		}

		// Token: 0x060000E3 RID: 227 RVA: 0x000062EA File Offset: 0x000044EA
		public static TextToken CreateCharacter(char character)
		{
			return new TextToken(TextToken.TokenType.Character, character);
		}

		// Token: 0x060000E4 RID: 228 RVA: 0x000062F3 File Offset: 0x000044F3
		public static TextToken CreateTag(RichTextTag tag)
		{
			return new TextToken(tag);
		}

		// Token: 0x060000E5 RID: 229 RVA: 0x000062FB File Offset: 0x000044FB
		public static TextToken CreateCharacterCannotEndLineWith(char character)
		{
			return new TextToken(TextToken.TokenType.Character, character)
			{
				CannotEndLineWithCharacter = true
			};
		}

		// Token: 0x060000E6 RID: 230 RVA: 0x0000630B File Offset: 0x0000450B
		public static TextToken CreateCharacterCannotStartLineWith(char character)
		{
			return new TextToken(TextToken.TokenType.Character, character)
			{
				CannotStartLineWithCharacter = true
			};
		}

		// Token: 0x060000E7 RID: 231 RVA: 0x0000631C File Offset: 0x0000451C
		public static List<TextToken> CreateTokenArrayFromWord(string word)
		{
			List<TextToken> list = new List<TextToken>();
			foreach (char c in word)
			{
				list.Add(TextToken.CreateCharacter(c));
			}
			return list;
		}

		// Token: 0x02000040 RID: 64
		public enum TokenType
		{
			// Token: 0x04000163 RID: 355
			EmptyCharacter,
			// Token: 0x04000164 RID: 356
			ZeroWidthSpace,
			// Token: 0x04000165 RID: 357
			NonBreakingSpace,
			// Token: 0x04000166 RID: 358
			WordJoiner,
			// Token: 0x04000167 RID: 359
			NewLine,
			// Token: 0x04000168 RID: 360
			Tab,
			// Token: 0x04000169 RID: 361
			Character,
			// Token: 0x0400016A RID: 362
			Tag
		}
	}
}
