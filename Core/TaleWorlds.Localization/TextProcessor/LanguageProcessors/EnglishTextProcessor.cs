using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace TaleWorlds.Localization.TextProcessor.LanguageProcessors
{
	// Token: 0x02000031 RID: 49
	public class EnglishTextProcessor : LanguageSpecificTextProcessor
	{
		// Token: 0x06000143 RID: 323 RVA: 0x000071F0 File Offset: 0x000053F0
		public override void ProcessToken(string sourceText, ref int cursorPos, string token, StringBuilder outputString)
		{
			char c = token[1];
			if (c == 'a')
			{
				if (this.CheckNextCharIsVowel(sourceText, cursorPos))
				{
					outputString.Append("an");
					return;
				}
				outputString.Append("a");
				return;
			}
			else
			{
				if (c != 'A')
				{
					if (c == 's')
					{
						string text = "";
						int num = 0;
						for (int i = outputString.Length - 1; i >= 0; i--)
						{
							if (outputString[i] == ' ')
							{
								num = i + 1;
								break;
							}
							text += outputString[i].ToString();
						}
						text = new string(text.Reverse<char>().ToArray<char>());
						int length = text.Length;
						if (text.Length > 1)
						{
							string text2;
							if (this.HandleIrregularNouns(text, out text2))
							{
								outputString.Replace(text, text2, num, length);
								return;
							}
							if (this.Handle_ves_Suffix(text, out text2))
							{
								outputString.Replace(text, text2, num, length);
								return;
							}
							if (this.Handle_ies_Suffix(text, out text2))
							{
								outputString.Replace(text, text2, num, length);
								return;
							}
							if (this.Handle_es_Suffix(text, out text2))
							{
								outputString.Replace(text, text2, num, length);
								return;
							}
							if (this.Handle_s_Suffix(text, out text2))
							{
								outputString.Replace(text, text2, num, length);
								return;
							}
							outputString.Append(c);
							return;
						}
					}
					else if (c == 'o')
					{
						this.HandleApostrophe(outputString, cursorPos);
					}
					return;
				}
				if (this.CheckNextCharIsVowel(sourceText, cursorPos))
				{
					outputString.Append("An");
					return;
				}
				outputString.Append("A");
				return;
			}
		}

		// Token: 0x06000144 RID: 324 RVA: 0x0000736C File Offset: 0x0000556C
		private char GetLastCharacter(StringBuilder outputText, int cursorPos)
		{
			for (int i = cursorPos - 1; i >= 0; i--)
			{
				if (char.IsLetter(outputText[i]))
				{
					return outputText[i];
				}
			}
			return 'x';
		}

		// Token: 0x06000145 RID: 325 RVA: 0x000073A0 File Offset: 0x000055A0
		private void HandleApostrophe(StringBuilder outputString, int cursorPos)
		{
			string text = outputString.ToString();
			bool flag = false;
			if (text.Length < cursorPos)
			{
				cursorPos = text.Length;
			}
			if (text.EndsWith("</b></a>"))
			{
				cursorPos -= 8;
				outputString.Remove(outputString.Length - 8, 8);
				flag = true;
			}
			int lastCharacter = (int)this.GetLastCharacter(outputString, cursorPos);
			outputString.Append('\'');
			if (lastCharacter != 115)
			{
				outputString.Append('s');
			}
			if (flag)
			{
				outputString.Append("</b></a>");
			}
		}

		// Token: 0x06000146 RID: 326 RVA: 0x00007418 File Offset: 0x00005618
		private bool CheckNextCharIsVowel(string sourceText, int cursorPos)
		{
			while (cursorPos < sourceText.Length)
			{
				char c = sourceText[cursorPos];
				if ("aeiouAEIOU".Contains(c))
				{
					return true;
				}
				if ("bcdfghjklmnpqrstvwxyzBCDFGHJKLMNPQRSTVWXYZ".Contains(c))
				{
					return false;
				}
				cursorPos++;
			}
			return false;
		}

		// Token: 0x06000147 RID: 327 RVA: 0x0000745C File Offset: 0x0000565C
		private bool HandleIrregularNouns(string text, out string resultPlural)
		{
			resultPlural = null;
			char.IsLower(text[text.Length - 1]);
			string text2 = text.ToLower();
			string text3;
			if (this.IrregularNouns.TryGetValue(text2, out text3))
			{
				if (text.All((char c) => char.IsUpper(c)))
				{
					resultPlural = text3.ToUpper();
				}
				else if (char.IsUpper(text[0]))
				{
					char[] array = text3.ToCharArray();
					array[0] = char.ToUpper(array[0]);
					resultPlural = new string(array);
				}
				else
				{
					resultPlural = text3.ToLower();
				}
				return true;
			}
			return false;
		}

		// Token: 0x06000148 RID: 328 RVA: 0x00007500 File Offset: 0x00005700
		private bool Handle_ves_Suffix(string text, out string resultPlural)
		{
			resultPlural = null;
			bool flag = char.IsLower(text[text.Length - 1]);
			char c = char.ToLower(text[text.Length - 1]);
			char c2 = char.ToLower(text[text.Length - 2]);
			if (c2 != 'o' && "aeiouAEIOU".Contains(c2) && c == 'f')
			{
				resultPlural = text.Remove(text.Length - 1);
				resultPlural += (flag ? "ves" : "VES");
				return true;
			}
			if (c2 == 'f' && "aeiouAEIOU".Contains(c))
			{
				resultPlural = text.Remove(text.Length - 2, 2);
				resultPlural += (flag ? "v" : "V");
				resultPlural += (flag ? c : char.ToUpper(c)).ToString();
				resultPlural += (flag ? "s" : "S");
				return true;
			}
			if (c2 == 'l' && c == 'f')
			{
				resultPlural = text.Remove(text.Length - 1);
				resultPlural += (flag ? "ves" : "VES");
				return true;
			}
			return false;
		}

		// Token: 0x06000149 RID: 329 RVA: 0x00007634 File Offset: 0x00005834
		private bool Handle_ies_Suffix(string text, out string resultPlural)
		{
			resultPlural = null;
			bool flag = char.IsLower(text[text.Length - 1]);
			char c = char.ToLower(text[text.Length - 1]);
			char c2 = char.ToLower(text[text.Length - 2]);
			if ("bcdfghjklmnpqrstvwxyzBCDFGHJKLMNPQRSTVWXYZ".Contains(c2) && c == 'y')
			{
				resultPlural = text.Remove(text.Length - 1);
				resultPlural += (flag ? "ies" : "IES");
				return true;
			}
			return false;
		}

		// Token: 0x0600014A RID: 330 RVA: 0x000076C0 File Offset: 0x000058C0
		private bool Handle_es_Suffix(string text, out string resultPlural)
		{
			resultPlural = null;
			bool flag = char.IsLower(text[text.Length - 1]);
			string text2 = text[text.Length - 1].ToString();
			string text3 = text[text.Length - 2].ToString();
			if (text2 == "z")
			{
				resultPlural = text;
				resultPlural += (flag ? "zes" : "ZES");
				return true;
			}
			if (this.Sibilants.Contains(text2))
			{
				resultPlural = text;
				resultPlural += (flag ? "es" : "ES");
				return true;
			}
			if (this.Sibilants.Contains(text3 + text2))
			{
				resultPlural = text;
				resultPlural += (flag ? "es" : "ES");
				return true;
			}
			if ("bcdfghjklmnpqrstvwxyzBCDFGHJKLMNPQRSTVWXYZ".Contains(text3) && text2 == "o")
			{
				resultPlural = text;
				resultPlural += (flag ? "es" : "ES");
				return true;
			}
			if (text3 == "o" && text2 == "e")
			{
				resultPlural = text;
				resultPlural = resultPlural.Remove(resultPlural.Length - 1);
				resultPlural += (flag ? "es" : "ES");
				return true;
			}
			if (text3 == "i" && text2 == "s")
			{
				resultPlural = text;
				resultPlural = resultPlural.Remove(resultPlural.Length - 2);
				resultPlural += (flag ? "es" : "ES");
				return true;
			}
			return false;
		}

		// Token: 0x0600014B RID: 331 RVA: 0x00007864 File Offset: 0x00005A64
		private bool Handle_s_Suffix(string text, out string resultPlural)
		{
			resultPlural = null;
			bool flag = char.IsLower(text[text.Length - 1]);
			char c = char.ToLower(text[text.Length - 1]);
			char c2 = char.ToLower(text[text.Length - 2]);
			if ("bcdfghjklmnpqrstvwxyzBCDFGHJKLMNPQRSTVWXYZ".Contains(c))
			{
				resultPlural = text;
				resultPlural += (flag ? "s" : "S");
				return true;
			}
			if (c == 'e')
			{
				resultPlural = text;
				resultPlural += (flag ? "s" : "S");
				return true;
			}
			if ("aeiouAEIOU".Contains(c2) && c == 'y')
			{
				resultPlural = text;
				resultPlural += (flag ? "s" : "S");
				return true;
			}
			if (c2 == 'f' && c == 'f')
			{
				resultPlural = text;
				resultPlural += (flag ? "s" : "S");
				return true;
			}
			if (c2 == 'o' && c == 'f')
			{
				resultPlural = text;
				resultPlural += (flag ? "s" : "S");
				return true;
			}
			if ("aeiouAEIOU".Contains(c2) && c == 'o')
			{
				resultPlural = text;
				resultPlural += (flag ? "s" : "S");
				return true;
			}
			return false;
		}

		// Token: 0x17000039 RID: 57
		// (get) Token: 0x0600014C RID: 332 RVA: 0x000079A9 File Offset: 0x00005BA9
		public override CultureInfo CultureInfoForLanguage
		{
			get
			{
				return CultureInfo.InvariantCulture;
			}
		}

		// Token: 0x0600014D RID: 333 RVA: 0x000079B0 File Offset: 0x00005BB0
		public override void ClearTemporaryData()
		{
		}

		// Token: 0x040000A4 RID: 164
		private Dictionary<string, string> IrregularNouns = new Dictionary<string, string>
		{
			{ "man", "men" },
			{ "footman", "footmen" },
			{ "crossbowman", "crossbowmen" },
			{ "pikeman", "pikemen" },
			{ "shieldman", "shieldmen" },
			{ "shieldsman", "shieldsmen" },
			{ "woman", "women" },
			{ "child", "children" },
			{ "mouse", "mice" },
			{ "louse", "lice" },
			{ "tooth", "teeth" },
			{ "goose", "geese" },
			{ "foot", "feet" },
			{ "ox", "oxen" },
			{ "sheep", "sheep" },
			{ "fish", "fish" },
			{ "species", "species" },
			{ "aircraft", "aircraft" },
			{ "news", "news" },
			{ "advice", "advice" },
			{ "information", "information" },
			{ "luggage", "luggage" },
			{ "athletics", "athletics" },
			{ "linguistics", "linguistics" },
			{ "curriculum", "curricula" },
			{ "analysis", "analyses" },
			{ "ellipsis", "ellipses" },
			{ "bison", "bison" },
			{ "corpus", "corpora" },
			{ "crisis", "crises" },
			{ "criterion", "criteria" },
			{ "die", "dice" },
			{ "graffito", "graffiti" },
			{ "cactus", "cacti" },
			{ "focus", "foci" },
			{ "fungus", "fungi" },
			{ "headquarters", "headquarters" },
			{ "trousers", "trousers" },
			{ "cattle", "cattle" },
			{ "scissors", "scissors" },
			{ "index", "indices" },
			{ "vertex", "vertices" },
			{ "matrix", "matrices" },
			{ "radius", "radii" },
			{ "photo", "photos" },
			{ "piano", "pianos" },
			{ "dwarf", "dwarves" },
			{ "wharf", "wharves" },
			{ "formula", "formulae" },
			{ "moose", "moose" },
			{ "phenomenon", "phenomena" }
		};

		// Token: 0x040000A5 RID: 165
		private string[] Sibilants = new string[] { "s", "x", "ch", "sh", "es", "ss" };

		// Token: 0x040000A6 RID: 166
		private const string Vowels = "aeiouAEIOU";

		// Token: 0x040000A7 RID: 167
		private const string Consonants = "bcdfghjklmnpqrstvwxyzBCDFGHJKLMNPQRSTVWXYZ";
	}
}
