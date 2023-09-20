using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace TaleWorlds.Localization.TextProcessor.LanguageProcessors
{
	public class TurkishTextProcessor : LanguageSpecificTextProcessor
	{
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

		private bool IsVowel(char c)
		{
			return TurkishTextProcessor.Vowels.Contains(char.ToLower(c, this.CultureInfoForLanguage));
		}

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

		private bool IsFrontVowel(char c)
		{
			return TurkishTextProcessor.FrontVowels.Contains(char.ToLower(c, this.CultureInfoForLanguage));
		}

		private bool IsClosedVowel(char c)
		{
			return TurkishTextProcessor.ClosedVowels.Contains(char.ToLower(c, this.CultureInfoForLanguage));
		}

		private bool IsConsonant(char c)
		{
			return TurkishTextProcessor.Consonants.Contains(char.ToLower(c, this.CultureInfoForLanguage));
		}

		private bool IsUnvoicedConsonant(char c)
		{
			return TurkishTextProcessor.UnvoicedConsonants.Contains(char.ToLower(c, this.CultureInfoForLanguage));
		}

		private bool IsHardUnvoicedConsonant(char c)
		{
			return TurkishTextProcessor.HardUnvoicedConsonants.Contains(char.ToLower(c, this.CultureInfoForLanguage));
		}

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

		private void AddSuffix_im(StringBuilder outputString)
		{
			char lastVowel = this.GetLastVowel(outputString);
			char c = (this.IsClosedVowel(lastVowel) ? lastVowel : this.OpenVowelToClosedVowel(lastVowel));
			this.SoftenLastCharacter(outputString);
			this.AddYIfNeeded(outputString);
			outputString.Append(c);
			outputString.Append('m');
		}

		private void AddSuffix_sin(StringBuilder outputString)
		{
			char lastVowel = this.GetLastVowel(outputString);
			char c = (this.IsClosedVowel(lastVowel) ? lastVowel : this.OpenVowelToClosedVowel(lastVowel));
			outputString.Append('s');
			outputString.Append(c);
			outputString.Append('n');
		}

		private void AddSuffix_dir(StringBuilder outputString)
		{
			char lastVowel = this.GetLastVowel(outputString);
			char c = (this.IsClosedVowel(lastVowel) ? lastVowel : this.OpenVowelToClosedVowel(lastVowel));
			char harmonizedD = this.GetHarmonizedD(outputString);
			outputString.Append(harmonizedD);
			outputString.Append(c);
			outputString.Append('r');
		}

		private void AddSuffix_iz(StringBuilder outputString)
		{
			char lastVowel = this.GetLastVowel(outputString);
			char c = (this.IsClosedVowel(lastVowel) ? lastVowel : this.OpenVowelToClosedVowel(lastVowel));
			this.SoftenLastCharacter(outputString);
			this.AddYIfNeeded(outputString);
			outputString.Append(c);
			outputString.Append('z');
		}

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

		private void AddSuffix_e(StringBuilder outputString)
		{
			char nextVowel = this.GetNextVowel(outputString);
			this.SoftenLastCharacter(outputString);
			this.AddYIfNeeded(outputString);
			outputString.Append(nextVowel);
		}

		private void AddSuffix_de(StringBuilder outputString)
		{
			char nextVowel = this.GetNextVowel(outputString);
			char harmonizedD = this.GetHarmonizedD(outputString);
			outputString.Append(harmonizedD);
			outputString.Append(nextVowel);
		}

		private void AddSuffix_den(StringBuilder outputString)
		{
			char nextVowel = this.GetNextVowel(outputString);
			char harmonizedD = this.GetHarmonizedD(outputString);
			outputString.Append(harmonizedD);
			outputString.Append(nextVowel);
			outputString.Append('n');
		}

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

		private void AddSuffix_ler(StringBuilder outputString)
		{
			char nextVowel = this.GetNextVowel(outputString);
			outputString.Append('l');
			outputString.Append(nextVowel);
			outputString.Append('r');
		}

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

		private void AddSuffix_n(StringBuilder outputString)
		{
			char lastLetter = this.GetLastLetter(outputString);
			char secondLastLetter = this.GetSecondLastLetter(outputString);
			if (this.IsVowel(lastLetter) && !this.IsVowel(secondLastLetter))
			{
				outputString.Append('n');
			}
		}

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

		private void AddYIfNeeded(StringBuilder outputString)
		{
			char lastCharacter = this.GetLastCharacter(outputString);
			if (this.IsVowel(lastCharacter) || (lastCharacter == '\'' && this.IsVowel(this.GetSecondLastCharacter(outputString))))
			{
				outputString.Append('y');
			}
		}

		private void SoftenLastCharacter(StringBuilder outputString)
		{
			char lastCharacter = this.GetLastCharacter(outputString);
			if (this.IsHardUnvoicedConsonant(lastCharacter) && !this.LastWordNonMutating(outputString))
			{
				outputString[outputString.Length - 1] = this.HardConsonantToSoftConsonant(lastCharacter);
			}
		}

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

		private bool LastWordNonMutating(StringBuilder outputString)
		{
			string lastWord = this.GetLastWord(outputString);
			return lastWord != null && TurkishTextProcessor.NonMutatingWord.Contains(lastWord.ToLower(this.CultureInfoForLanguage));
		}

		private char GetLastCharacter(StringBuilder outputString)
		{
			if (outputString.Length <= 0)
			{
				return '*';
			}
			return outputString[outputString.Length - 1];
		}

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

		private char GetSecondLastCharacter(StringBuilder outputString)
		{
			if (outputString.Length <= 1)
			{
				return '*';
			}
			return outputString[outputString.Length - 2];
		}

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

		public override CultureInfo CultureInfoForLanguage
		{
			get
			{
				return TurkishTextProcessor._cultureInfo;
			}
		}

		public override void ClearTemporaryData()
		{
			TurkishTextProcessor.LinkList.Clear();
		}

		private static CultureInfo _curCultureInfo = CultureInfo.InvariantCulture;

		private static char[] Vowels = new char[] { 'a', 'ı', 'o', 'u', 'e', 'i', 'ö', 'ü' };

		private static char[] BackVowels = new char[] { 'a', 'ı', 'o', 'u' };

		private static char[] FrontVowels = new char[] { 'e', 'i', 'ö', 'ü' };

		private static char[] OpenVowels = new char[] { 'a', 'e', 'o', 'ö' };

		private static char[] ClosedVowels = new char[] { 'ı', 'i', 'u', 'ü' };

		private static char[] Consonants = new char[]
		{
			'b', 'c', 'ç', 'd', 'f', 'g', 'ğ', 'h', 'j', 'k',
			'l', 'm', 'n', 'p', 'r', 's', 'ş', 't', 'v', 'y',
			'z'
		};

		private static char[] UnvoicedConsonants = new char[] { 'ç', 'f', 'h', 'k', 'p', 's', 'ş', 't' };

		private static char[] HardUnvoicedConsonants = new char[] { 'p', 'ç', 't', 'k' };

		private static string[] NonMutatingWord = new string[]
		{
			"ak", "at", "ek", "et", "göç", "ip", "çöp", "ok", "ot", "saç",
			"sap", "süt", "üç", "suç", "top", "ticaret", "kürk", "dük", "kont", "hizmet"
		};

		private static Dictionary<string, char> _exceptions = new Dictionary<string, char> { { "kontrol", 'e' } };

		[ThreadStatic]
		private static List<string> _linkList = new List<string>();

		private static CultureInfo _cultureInfo = new CultureInfo("tr-TR");
	}
}
