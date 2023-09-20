using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace TaleWorlds.Localization.TextProcessor.LanguageProcessors
{
	// Token: 0x02000038 RID: 56
	public class TurkishTextProcessor : LanguageSpecificTextProcessor
	{
		// Token: 0x17000075 RID: 117
		// (get) Token: 0x06000261 RID: 609 RVA: 0x00018604 File Offset: 0x00016804
		public static List<string> LinkList
		{
			get
			{
				if (TurkishTextProcessor._linkList == null)
				{
					TurkishTextProcessor._linkList = new List<string>();
				}
				return TurkishTextProcessor._linkList;
			}
		}

		// Token: 0x06000262 RID: 610 RVA: 0x0001861C File Offset: 0x0001681C
		private bool IsVowel(char c)
		{
			return TurkishTextProcessor.Vowels.Contains(char.ToLower(c, this.CultureInfoForLanguage));
		}

		// Token: 0x06000263 RID: 611 RVA: 0x00018634 File Offset: 0x00016834
		private char GetNextVowel(StringBuilder stringBuilder)
		{
			string lastWord = this.GetLastWord(stringBuilder);
			char lastVowel;
			if (lastWord != null && TurkishTextProcessor._exceptions.TryGetValue(lastWord.ToLower(this.CultureInfoForLanguage), out lastVowel))
			{
				return lastVowel;
			}
			lastVowel = this.GetLastVowel(stringBuilder);
			if (!TurkishTextProcessor.BackVowels.Contains(char.ToLower(lastVowel, this.CultureInfoForLanguage)))
			{
				return 'e';
			}
			return 'a';
		}

		// Token: 0x06000264 RID: 612 RVA: 0x0001868D File Offset: 0x0001688D
		private bool IsFrontVowel(char c)
		{
			return TurkishTextProcessor.FrontVowels.Contains(char.ToLower(c, this.CultureInfoForLanguage));
		}

		// Token: 0x06000265 RID: 613 RVA: 0x000186A5 File Offset: 0x000168A5
		private bool IsClosedVowel(char c)
		{
			return TurkishTextProcessor.ClosedVowels.Contains(char.ToLower(c, this.CultureInfoForLanguage));
		}

		// Token: 0x06000266 RID: 614 RVA: 0x000186BD File Offset: 0x000168BD
		private bool IsConsonant(char c)
		{
			return TurkishTextProcessor.Consonants.Contains(char.ToLower(c, this.CultureInfoForLanguage));
		}

		// Token: 0x06000267 RID: 615 RVA: 0x000186D5 File Offset: 0x000168D5
		private bool IsUnvoicedConsonant(char c)
		{
			return TurkishTextProcessor.UnvoicedConsonants.Contains(char.ToLower(c, this.CultureInfoForLanguage));
		}

		// Token: 0x06000268 RID: 616 RVA: 0x000186ED File Offset: 0x000168ED
		private bool IsHardUnvoicedConsonant(char c)
		{
			return TurkishTextProcessor.HardUnvoicedConsonants.Contains(char.ToLower(c, this.CultureInfoForLanguage));
		}

		// Token: 0x06000269 RID: 617 RVA: 0x00018705 File Offset: 0x00016905
		private char FrontVowelToBackVowel(char c)
		{
			c = char.ToLower(c, this.CultureInfoForLanguage);
			if (c == 'e')
			{
				return 'a';
			}
			if (c == 'i')
			{
				return 'ı';
			}
			if (c == 'ö')
			{
				return 'o';
			}
			if (c != 'ü')
			{
				return '*';
			}
			return 'u';
		}

		// Token: 0x0600026A RID: 618 RVA: 0x00018740 File Offset: 0x00016940
		private char OpenVowelToClosedVowel(char c)
		{
			c = char.ToLower(c, this.CultureInfoForLanguage);
			if (c == 'a')
			{
				return 'ı';
			}
			if (c == 'e')
			{
				return 'i';
			}
			if (c == 'o')
			{
				return 'u';
			}
			if (c != 'ö')
			{
				return '*';
			}
			return 'ü';
		}

		// Token: 0x0600026B RID: 619 RVA: 0x0001877B File Offset: 0x0001697B
		private char HardConsonantToSoftConsonant(char c)
		{
			c = char.ToLower(c, this.CultureInfoForLanguage);
			if (c == 'p')
			{
				return 'b';
			}
			if (c == 'ç')
			{
				return 'c';
			}
			if (c == 't')
			{
				return 'd';
			}
			if (c != 'k')
			{
				return '*';
			}
			return 'ğ';
		}

		// Token: 0x0600026C RID: 620 RVA: 0x000187B4 File Offset: 0x000169B4
		private char GetLastVowel(StringBuilder outputText)
		{
			for (int i = outputText.Length - 1; i >= 0; i--)
			{
				if (this.IsVowel(outputText[i]))
				{
					return outputText[i];
				}
			}
			return 'i';
		}

		// Token: 0x0600026D RID: 621 RVA: 0x000187F0 File Offset: 0x000169F0
		public override void ProcessToken(string sourceText, ref int cursorPos, string token, StringBuilder outputString)
		{
			bool flag = false;
			if (token == ".link")
			{
				TurkishTextProcessor.LinkList.Add(sourceText.Substring(7));
			}
			else if (sourceText.Contains("<a style=\"Link."))
			{
				if (sourceText[cursorPos - (token.Length + 3)] == '\'')
				{
					flag = this.IsLink(sourceText, token.Length + 2, cursorPos - 1);
				}
				else
				{
					flag = this.IsLink(sourceText, token.Length + 2, cursorPos);
				}
			}
			if (flag)
			{
				if (sourceText[cursorPos - (token.Length + 3)] == '\'')
				{
					cursorPos -= 8;
					outputString.Remove(outputString.Length - 9, 9);
					outputString.Append('\'');
				}
				else
				{
					cursorPos -= 8;
					outputString.Remove(outputString.Length - 8, 8);
				}
			}
			if (token == ".im")
			{
				this.AddSuffix_im(outputString);
			}
			else if (token == ".sin")
			{
				this.AddSuffix_sin(outputString);
			}
			else if (token == ".dir")
			{
				this.AddSuffix_dir(outputString);
			}
			else if (token == ".iz")
			{
				this.AddSuffix_iz(outputString);
			}
			else if (token == ".siniz")
			{
				this.AddSuffix_siniz(outputString);
			}
			else if (token == ".dirler")
			{
				this.AddSuffix_dirler(outputString);
			}
			else if (token == ".i")
			{
				this.AddSuffix_i(outputString);
			}
			else if (token == ".e")
			{
				this.AddSuffix_e(outputString);
			}
			else if (token == ".de")
			{
				this.AddSuffix_de(outputString);
			}
			else if (token == ".den")
			{
				this.AddSuffix_den(outputString);
			}
			else if (token == ".nin")
			{
				this.AddSuffix_nin(outputString);
			}
			else if (token == ".ler")
			{
				this.AddSuffix_ler(outputString);
			}
			else if (token == ".m")
			{
				this.AddSuffix_m(outputString);
			}
			else if (token == ".n")
			{
				this.AddSuffix_n(outputString);
			}
			else if (token == ".in")
			{
				this.AddSuffix_in(outputString);
			}
			else if (token == ".si")
			{
				this.AddSuffix_si(outputString);
			}
			else if (token == ".miz")
			{
				this.AddSuffix_miz(outputString);
			}
			else if (token == ".niz")
			{
				this.AddSuffix_niz(outputString);
			}
			else if (token == ".leri")
			{
				this.AddSuffix_leri(outputString);
			}
			if (flag)
			{
				cursorPos += 8;
				outputString.Append("</b></a>");
			}
		}

		// Token: 0x0600026E RID: 622 RVA: 0x00018AB0 File Offset: 0x00016CB0
		private void AddSuffix_im(StringBuilder outputString)
		{
			char lastVowel = this.GetLastVowel(outputString);
			char c = (this.IsClosedVowel(lastVowel) ? lastVowel : this.OpenVowelToClosedVowel(lastVowel));
			this.SoftenLastCharacter(outputString);
			this.AddYIfNeeded(outputString);
			outputString.Append(c);
			outputString.Append('m');
		}

		// Token: 0x0600026F RID: 623 RVA: 0x00018AF8 File Offset: 0x00016CF8
		private void AddSuffix_sin(StringBuilder outputString)
		{
			char lastVowel = this.GetLastVowel(outputString);
			char c = (this.IsClosedVowel(lastVowel) ? lastVowel : this.OpenVowelToClosedVowel(lastVowel));
			outputString.Append('s');
			outputString.Append(c);
			outputString.Append('n');
		}

		// Token: 0x06000270 RID: 624 RVA: 0x00018B3C File Offset: 0x00016D3C
		private void AddSuffix_dir(StringBuilder outputString)
		{
			char lastVowel = this.GetLastVowel(outputString);
			char c = (this.IsClosedVowel(lastVowel) ? lastVowel : this.OpenVowelToClosedVowel(lastVowel));
			char harmonizedD = this.GetHarmonizedD(outputString);
			outputString.Append(harmonizedD);
			outputString.Append(c);
			outputString.Append('r');
		}

		// Token: 0x06000271 RID: 625 RVA: 0x00018B88 File Offset: 0x00016D88
		private void AddSuffix_iz(StringBuilder outputString)
		{
			char lastVowel = this.GetLastVowel(outputString);
			char c = (this.IsClosedVowel(lastVowel) ? lastVowel : this.OpenVowelToClosedVowel(lastVowel));
			this.SoftenLastCharacter(outputString);
			this.AddYIfNeeded(outputString);
			outputString.Append(c);
			outputString.Append('z');
		}

		// Token: 0x06000272 RID: 626 RVA: 0x00018BD0 File Offset: 0x00016DD0
		private void AddSuffix_siniz(StringBuilder outputString)
		{
			char lastVowel = this.GetLastVowel(outputString);
			char c = (this.IsClosedVowel(lastVowel) ? lastVowel : this.OpenVowelToClosedVowel(lastVowel));
			outputString.Append('s');
			outputString.Append(c);
			outputString.Append('n');
			outputString.Append(c);
			outputString.Append('z');
		}

		// Token: 0x06000273 RID: 627 RVA: 0x00018C24 File Offset: 0x00016E24
		private void AddSuffix_dirler(StringBuilder outputString)
		{
			char lastVowel = this.GetLastVowel(outputString);
			char c = (this.IsClosedVowel(lastVowel) ? lastVowel : this.OpenVowelToClosedVowel(lastVowel));
			char nextVowel = this.GetNextVowel(outputString);
			char harmonizedD = this.GetHarmonizedD(outputString);
			outputString.Append(harmonizedD);
			outputString.Append(c);
			outputString.Append('r');
			outputString.Append('l');
			outputString.Append(nextVowel);
			outputString.Append('r');
		}

		// Token: 0x06000274 RID: 628 RVA: 0x00018C90 File Offset: 0x00016E90
		private void AddSuffix_i(StringBuilder outputString)
		{
			char lastVowel = this.GetLastVowel(outputString);
			char c = (this.IsClosedVowel(lastVowel) ? lastVowel : this.OpenVowelToClosedVowel(lastVowel));
			this.SoftenLastCharacter(outputString);
			if (this.GetLastCharacter(outputString) == '\'' && outputString.Length > 6 && outputString.ToString().EndsWith("Kalesi'", true, TurkishTextProcessor._cultureInfo))
			{
				outputString.Append('n');
			}
			else
			{
				this.AddYIfNeeded(outputString);
			}
			outputString.Append(c);
		}

		// Token: 0x06000275 RID: 629 RVA: 0x00018D08 File Offset: 0x00016F08
		private void AddSuffix_e(StringBuilder outputString)
		{
			char nextVowel = this.GetNextVowel(outputString);
			this.SoftenLastCharacter(outputString);
			this.AddYIfNeeded(outputString);
			outputString.Append(nextVowel);
		}

		// Token: 0x06000276 RID: 630 RVA: 0x00018D34 File Offset: 0x00016F34
		private void AddSuffix_de(StringBuilder outputString)
		{
			char nextVowel = this.GetNextVowel(outputString);
			char harmonizedD = this.GetHarmonizedD(outputString);
			outputString.Append(harmonizedD);
			outputString.Append(nextVowel);
		}

		// Token: 0x06000277 RID: 631 RVA: 0x00018D64 File Offset: 0x00016F64
		private void AddSuffix_den(StringBuilder outputString)
		{
			char nextVowel = this.GetNextVowel(outputString);
			char harmonizedD = this.GetHarmonizedD(outputString);
			outputString.Append(harmonizedD);
			outputString.Append(nextVowel);
			outputString.Append('n');
		}

		// Token: 0x06000278 RID: 632 RVA: 0x00018D9C File Offset: 0x00016F9C
		private void AddSuffix_nin(StringBuilder outputString)
		{
			char lastVowel = this.GetLastVowel(outputString);
			char c = (this.IsClosedVowel(lastVowel) ? lastVowel : this.OpenVowelToClosedVowel(lastVowel));
			char c2 = this.GetLastCharacter(outputString);
			if (c2 == '\'')
			{
				c2 = this.GetSecondLastCharacter(outputString);
			}
			else
			{
				this.SoftenLastCharacter(outputString);
			}
			if (this.IsVowel(c2))
			{
				outputString.Append('n');
			}
			outputString.Append(c);
			outputString.Append('n');
		}

		// Token: 0x06000279 RID: 633 RVA: 0x00018E08 File Offset: 0x00017008
		private void AddSuffix_ler(StringBuilder outputString)
		{
			char nextVowel = this.GetNextVowel(outputString);
			outputString.Append('l');
			outputString.Append(nextVowel);
			outputString.Append('r');
		}

		// Token: 0x0600027A RID: 634 RVA: 0x00018E38 File Offset: 0x00017038
		private void AddSuffix_m(StringBuilder outputString)
		{
			char lastVowel = this.GetLastVowel(outputString);
			char c = (this.IsClosedVowel(lastVowel) ? lastVowel : this.OpenVowelToClosedVowel(lastVowel));
			char lastCharacter = this.GetLastCharacter(outputString);
			this.SoftenLastCharacter(outputString);
			if (this.IsConsonant(lastCharacter))
			{
				outputString.Append(c);
			}
			outputString.Append('m');
		}

		// Token: 0x0600027B RID: 635 RVA: 0x00018E8C File Offset: 0x0001708C
		private void AddSuffix_n(StringBuilder outputString)
		{
			char lastLetter = this.GetLastLetter(outputString);
			char secondLastLetter = this.GetSecondLastLetter(outputString);
			if (this.IsVowel(lastLetter) && !this.IsVowel(secondLastLetter))
			{
				outputString.Append('n');
			}
		}

		// Token: 0x0600027C RID: 636 RVA: 0x00018EC4 File Offset: 0x000170C4
		private void AddSuffix_in(StringBuilder outputString)
		{
			char lastVowel = this.GetLastVowel(outputString);
			char c = (this.IsClosedVowel(lastVowel) ? lastVowel : this.OpenVowelToClosedVowel(lastVowel));
			char lastLetter = this.GetLastLetter(outputString);
			this.SoftenLastCharacter(outputString);
			if (this.IsConsonant(lastLetter))
			{
				outputString.Append(c);
			}
			outputString.Append('n');
		}

		// Token: 0x0600027D RID: 637 RVA: 0x00018F18 File Offset: 0x00017118
		private void AddSuffix_si(StringBuilder outputString)
		{
			char lastVowel = this.GetLastVowel(outputString);
			char c = (this.IsClosedVowel(lastVowel) ? lastVowel : this.OpenVowelToClosedVowel(lastVowel));
			char lastCharacter = this.GetLastCharacter(outputString);
			this.SoftenLastCharacter(outputString);
			if (this.IsVowel(lastCharacter))
			{
				outputString.Append('s');
			}
			outputString.Append(c);
		}

		// Token: 0x0600027E RID: 638 RVA: 0x00018F6C File Offset: 0x0001716C
		private void AddSuffix_miz(StringBuilder outputString)
		{
			char lastVowel = this.GetLastVowel(outputString);
			char c = (this.IsClosedVowel(lastVowel) ? lastVowel : this.OpenVowelToClosedVowel(lastVowel));
			char lastCharacter = this.GetLastCharacter(outputString);
			this.SoftenLastCharacter(outputString);
			if (this.IsConsonant(lastCharacter))
			{
				outputString.Append(c);
			}
			outputString.Append('m');
			outputString.Append(c);
			outputString.Append('z');
		}

		// Token: 0x0600027F RID: 639 RVA: 0x00018FD0 File Offset: 0x000171D0
		private void AddSuffix_niz(StringBuilder outputString)
		{
			char lastVowel = this.GetLastVowel(outputString);
			char c = (this.IsClosedVowel(lastVowel) ? lastVowel : this.OpenVowelToClosedVowel(lastVowel));
			char lastCharacter = this.GetLastCharacter(outputString);
			this.SoftenLastCharacter(outputString);
			if (this.IsConsonant(lastCharacter))
			{
				outputString.Append(c);
			}
			outputString.Append('n');
			outputString.Append(c);
			outputString.Append('z');
		}

		// Token: 0x06000280 RID: 640 RVA: 0x00019034 File Offset: 0x00017234
		private void AddSuffix_leri(StringBuilder outputString)
		{
			this.GetLastVowel(outputString);
			char nextVowel = this.GetNextVowel(outputString);
			char c = ((nextVowel == 'a') ? 'ı' : 'i');
			outputString.Append('l');
			outputString.Append(nextVowel);
			outputString.Append('r');
			outputString.Append(c);
		}

		// Token: 0x06000281 RID: 641 RVA: 0x00019084 File Offset: 0x00017284
		private char GetHarmonizedD(StringBuilder outputString)
		{
			char c = this.GetLastCharacter(outputString);
			if (c == '\'')
			{
				c = this.GetSecondLastCharacter(outputString);
			}
			if (!this.IsUnvoicedConsonant(c))
			{
				return 'd';
			}
			return 't';
		}

		// Token: 0x06000282 RID: 642 RVA: 0x000190B4 File Offset: 0x000172B4
		private void AddYIfNeeded(StringBuilder outputString)
		{
			char lastCharacter = this.GetLastCharacter(outputString);
			if (this.IsVowel(lastCharacter) || (lastCharacter == '\'' && this.IsVowel(this.GetSecondLastCharacter(outputString))))
			{
				outputString.Append('y');
			}
		}

		// Token: 0x06000283 RID: 643 RVA: 0x000190F0 File Offset: 0x000172F0
		private void SoftenLastCharacter(StringBuilder outputString)
		{
			char lastCharacter = this.GetLastCharacter(outputString);
			if (this.IsHardUnvoicedConsonant(lastCharacter) && !this.LastWordNonMutating(outputString))
			{
				outputString[outputString.Length - 1] = this.HardConsonantToSoftConsonant(lastCharacter);
			}
		}

		// Token: 0x06000284 RID: 644 RVA: 0x0001912C File Offset: 0x0001732C
		private string GetLastWord(StringBuilder outputString)
		{
			int num = -1;
			int num2 = outputString.Length - 1;
			while (num2 >= 0 && num < 0)
			{
				if (outputString[num2] == ' ')
				{
					num = num2;
				}
				num2--;
			}
			if (num < outputString.Length - 1)
			{
				return outputString.ToString(num + 1, outputString.Length - num - 1).Trim(new char[] { '\n' });
			}
			return null;
		}

		// Token: 0x06000285 RID: 645 RVA: 0x00019190 File Offset: 0x00017390
		private bool LastWordNonMutating(StringBuilder outputString)
		{
			string lastWord = this.GetLastWord(outputString);
			return lastWord != null && TurkishTextProcessor.NonMutatingWord.Contains(lastWord.ToLower(this.CultureInfoForLanguage));
		}

		// Token: 0x06000286 RID: 646 RVA: 0x000191C0 File Offset: 0x000173C0
		private char GetLastCharacter(StringBuilder outputString)
		{
			if (outputString.Length <= 0)
			{
				return '*';
			}
			return outputString[outputString.Length - 1];
		}

		// Token: 0x06000287 RID: 647 RVA: 0x000191DC File Offset: 0x000173DC
		private char GetLastLetter(StringBuilder outputString)
		{
			for (int i = outputString.Length - 1; i >= 0; i--)
			{
				if (char.IsLetter(outputString[i]))
				{
					return outputString[i];
				}
			}
			return 'x';
		}

		// Token: 0x06000288 RID: 648 RVA: 0x00019214 File Offset: 0x00017414
		private char GetSecondLastLetter(StringBuilder outputString)
		{
			bool flag = false;
			for (int i = outputString.Length - 1; i >= 0; i--)
			{
				if (char.IsLetter(outputString[i]))
				{
					if (flag)
					{
						return outputString[i];
					}
					flag = true;
				}
			}
			return 'x';
		}

		// Token: 0x06000289 RID: 649 RVA: 0x00019253 File Offset: 0x00017453
		private char GetSecondLastCharacter(StringBuilder outputString)
		{
			if (outputString.Length <= 1)
			{
				return '*';
			}
			return outputString[outputString.Length - 2];
		}

		// Token: 0x0600028A RID: 650 RVA: 0x00019270 File Offset: 0x00017470
		private bool IsLink(string sourceText, int tokenLength, int cursorPos)
		{
			string text = sourceText.Remove(cursorPos - tokenLength);
			for (int i = 0; i < TurkishTextProcessor.LinkList.Count; i++)
			{
				if (sourceText.Length >= TurkishTextProcessor.LinkList[i].Length && text.EndsWith(TurkishTextProcessor.LinkList[i]))
				{
					TurkishTextProcessor.LinkList.RemoveAt(i);
					return true;
				}
			}
			return false;
		}

		// Token: 0x17000076 RID: 118
		// (get) Token: 0x0600028B RID: 651 RVA: 0x000192D5 File Offset: 0x000174D5
		public override CultureInfo CultureInfoForLanguage
		{
			get
			{
				return TurkishTextProcessor._cultureInfo;
			}
		}

		// Token: 0x0600028C RID: 652 RVA: 0x000192DC File Offset: 0x000174DC
		public override void ClearTemporaryData()
		{
			TurkishTextProcessor.LinkList.Clear();
		}

		// Token: 0x040000F6 RID: 246
		private static CultureInfo _curCultureInfo = CultureInfo.InvariantCulture;

		// Token: 0x040000F7 RID: 247
		private static char[] Vowels = new char[] { 'a', 'ı', 'o', 'u', 'e', 'i', 'ö', 'ü' };

		// Token: 0x040000F8 RID: 248
		private static char[] BackVowels = new char[] { 'a', 'ı', 'o', 'u' };

		// Token: 0x040000F9 RID: 249
		private static char[] FrontVowels = new char[] { 'e', 'i', 'ö', 'ü' };

		// Token: 0x040000FA RID: 250
		private static char[] OpenVowels = new char[] { 'a', 'e', 'o', 'ö' };

		// Token: 0x040000FB RID: 251
		private static char[] ClosedVowels = new char[] { 'ı', 'i', 'u', 'ü' };

		// Token: 0x040000FC RID: 252
		private static char[] Consonants = new char[]
		{
			'b', 'c', 'ç', 'd', 'f', 'g', 'ğ', 'h', 'j', 'k',
			'l', 'm', 'n', 'p', 'r', 's', 'ş', 't', 'v', 'y',
			'z'
		};

		// Token: 0x040000FD RID: 253
		private static char[] UnvoicedConsonants = new char[] { 'ç', 'f', 'h', 'k', 'p', 's', 'ş', 't' };

		// Token: 0x040000FE RID: 254
		private static char[] HardUnvoicedConsonants = new char[] { 'p', 'ç', 't', 'k' };

		// Token: 0x040000FF RID: 255
		private static string[] NonMutatingWord = new string[]
		{
			"ak", "at", "ek", "et", "göç", "ip", "çöp", "ok", "ot", "saç",
			"sap", "süt", "üç", "suç", "top", "ticaret", "kürk", "dük", "kont", "hizmet"
		};

		// Token: 0x04000100 RID: 256
		private static Dictionary<string, char> _exceptions = new Dictionary<string, char> { { "kontrol", 'e' } };

		// Token: 0x04000101 RID: 257
		[ThreadStatic]
		private static List<string> _linkList = new List<string>();

		// Token: 0x04000102 RID: 258
		private static CultureInfo _cultureInfo = new CultureInfo("tr-TR");
	}
}
