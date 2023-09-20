using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace TaleWorlds.Localization.TextProcessor.LanguageProcessors
{
	public class FrenchTextProcessor : LanguageSpecificTextProcessor
	{
		public static Dictionary<string, ValueTuple<string, int, bool>> WordGroups
		{
			get
			{
				if (FrenchTextProcessor._wordGroups == null)
				{
					FrenchTextProcessor._wordGroups = new Dictionary<string, ValueTuple<string, int, bool>>();
				}
				return FrenchTextProcessor._wordGroups;
			}
		}

		private string LinkTag
		{
			get
			{
				return ".link";
			}
		}

		private int LinkTagLength
		{
			get
			{
				return 7;
			}
		}

		private string LinkStarter
		{
			get
			{
				return "<a style=\"Link.";
			}
		}

		private string LinkEnding
		{
			get
			{
				return "</b></a>";
			}
		}

		public override void ProcessToken(string sourceText, ref int cursorPos, string token, StringBuilder outputString)
		{
			if (sourceText.Length > this.LinkStarter.Length + cursorPos)
			{
				string text = sourceText.Substring(cursorPos, this.LinkStarter.Length);
				if (token == this.LinkTag || text.Equals(this.LinkStarter))
				{
					cursorPos = this.ProcessLink(sourceText, cursorPos, token, outputString);
				}
			}
			string text2 = token.ToLower();
			if (FrenchTextProcessor.GenderTokens.TokenList.IndexOf(token) >= 0)
			{
				this.SetGenderInfo(token);
				this.ProcessWordGroup(sourceText, token, cursorPos);
				this.ResetGender();
				return;
			}
			if (FrenchTextProcessor.FunctionTokens.TokenList.IndexOf(text2) >= 0 && this.CheckWhiteSpaceAndTextEnd(sourceText, cursorPos))
			{
				ValueTuple<string, bool> valueTuple;
				if (this.IsWordGroup(sourceText, token, cursorPos, out valueTuple))
				{
					this.SetGenderInfo(valueTuple.Item1);
					if (valueTuple.Item2)
					{
						this.SetPlural();
					}
					else
					{
						this.SetSingular();
					}
				}
				if (!(text2 == ".cl"))
				{
					if (!(text2 == ".dl"))
					{
						if (!(text2 == ".l"))
						{
							if (!(text2 == ".a"))
							{
								if (!(text2 == ".d"))
								{
									if (text2 == ".c")
									{
										this.HandleAPreposition(sourceText, token, ref cursorPos, outputString);
									}
								}
								else
								{
									this.HandleDePreposition(sourceText, token, ref cursorPos, outputString);
								}
							}
							else
							{
								this.HandleIndefiniteArticles(sourceText, token, cursorPos, outputString);
							}
						}
						else
						{
							this.HandleDefiniteArticles(sourceText, token, cursorPos, outputString);
						}
					}
					else
					{
						this.HandleDePrepositionFollowedByArticle(sourceText, token, ref cursorPos, outputString);
					}
				}
				else
				{
					this.HandleAPrepositionFollowedByDefiniteArticle(sourceText, token, ref cursorPos, outputString);
				}
				FrenchTextProcessor._isPlural = false;
				FrenchTextProcessor._curGender = FrenchTextProcessor.WordGenderEnum.NoDeclination;
			}
		}

		private bool IsWordGroup(string sourceText, string token, int cursorPos, out ValueTuple<string, bool> tags)
		{
			int num = 0;
			string text = string.Empty;
			tags = new ValueTuple<string, bool>(string.Empty, false);
			foreach (KeyValuePair<string, ValueTuple<string, int, bool>> keyValuePair in FrenchTextProcessor.WordGroups)
			{
				if (keyValuePair.Key.Length > 0 && sourceText.Length >= cursorPos + keyValuePair.Key.Length && keyValuePair.Key.Length > num && keyValuePair.Key.Equals(sourceText.Substring(cursorPos, keyValuePair.Key.Length)))
				{
					text = keyValuePair.Key;
					num = keyValuePair.Key.Length;
				}
			}
			if (!string.IsNullOrEmpty(text))
			{
				tags = new ValueTuple<string, bool>(FrenchTextProcessor.WordGroups[text].Item1, FrenchTextProcessor.WordGroups[text].Item3);
				return true;
			}
			return false;
		}

		private bool CheckNextCharIsVowel(string sourceText, int cursorPos)
		{
			return cursorPos < sourceText.Length && FrenchTextProcessor.Vowels.Contains(char.ToLower(sourceText[cursorPos]));
		}

		private bool CheckWhiteSpaceAndTextEnd(string sourceText, int cursorPos)
		{
			return cursorPos < sourceText.Length && !char.IsWhiteSpace(sourceText[cursorPos]);
		}

		private void SetFeminine()
		{
			FrenchTextProcessor._curGender = FrenchTextProcessor.WordGenderEnum.Feminine;
		}

		private void SetNeuter()
		{
			FrenchTextProcessor._curGender = FrenchTextProcessor.WordGenderEnum.Neuter;
		}

		private void SetMasculine()
		{
			FrenchTextProcessor._curGender = FrenchTextProcessor.WordGenderEnum.Masculine;
		}

		private void SetPlural()
		{
			FrenchTextProcessor._isPlural = true;
		}

		private void SetSingular()
		{
			FrenchTextProcessor._isPlural = false;
		}

		private void HandleDefiniteArticles(string text, string token, int cursorPos, StringBuilder outputString)
		{
			string definiteArticle = this.GetDefiniteArticle(text, token, cursorPos);
			if (!string.IsNullOrEmpty(definiteArticle))
			{
				outputString.Append(definiteArticle);
			}
		}

		private string GetDefiniteArticle(string text, string token, int cursorPos)
		{
			string text2 = null;
			if (FrenchTextProcessor._curGender != FrenchTextProcessor.WordGenderEnum.NoDeclination)
			{
				if (FrenchTextProcessor._isPlural)
				{
					text2 = FrenchTextProcessor._genderToDefiniteArticle[FrenchTextProcessor.WordType.Plural];
				}
				else if (this.CheckNextCharIsVowel(text, cursorPos) && !this.CheckIfNextWordShouldBeConsideredAConsonant(text, token, cursorPos))
				{
					text2 = FrenchTextProcessor._genderToDefiniteArticle[FrenchTextProcessor.WordType.StartingWithaVowel];
				}
				else
				{
					text2 = FrenchTextProcessor._genderToDefiniteArticle[this.GetWordTypeFromGender(FrenchTextProcessor._curGender)];
				}
				string text3 = token.ToLowerInvariant();
				if (text3 == ".cl" || text3 == ".dl")
				{
					if (char.IsUpper(token[2]))
					{
						text2 = char.ToUpper(text2[0]).ToString() + text2.Substring(1);
					}
				}
				else if (text2 != null)
				{
					if (token.All((char x) => !char.IsLetter(x) || char.IsUpper(x)))
					{
						text2 = char.ToUpper(text2[0]).ToString() + text2.Substring(1);
					}
				}
			}
			return text2;
		}

		private void HandleIndefiniteArticles(string text, string token, int cursorPos, StringBuilder stringBuilder)
		{
			string text2 = null;
			if (FrenchTextProcessor._isPlural)
			{
				text2 = FrenchTextProcessor._genderToIndefiniteArticle[FrenchTextProcessor.WordType.Plural];
			}
			else if (FrenchTextProcessor._curGender != FrenchTextProcessor.WordGenderEnum.NoDeclination)
			{
				text2 = FrenchTextProcessor._genderToIndefiniteArticle[this.GetWordTypeFromGender(FrenchTextProcessor._curGender)];
			}
			if (!string.IsNullOrEmpty(text2))
			{
				if (token.All((char x) => !char.IsLetter(x) || char.IsUpper(x)))
				{
					text2 = char.ToUpper(text2[0]).ToString() + text2.Substring(1);
				}
				stringBuilder.Append(text2);
			}
		}

		private void HandleAPreposition(string text, string token, ref int cursorPos, StringBuilder outputString)
		{
			string apreposition = this.GetAPreposition(text, token, cursorPos);
			string nextWord = this.GetNextWord(text, token, cursorPos);
			string text2;
			if (this.CheckIfWordsHaveContraction(apreposition, nextWord.Trim(), out text2))
			{
				outputString.Append(text2);
				cursorPos += nextWord.Length;
				return;
			}
			outputString.Append(apreposition + " ");
		}

		private string GetAPreposition(string text, string token, int cursorPos)
		{
			string text2 = FrenchTextProcessor._aPreposition;
			if (char.IsUpper(token[1]))
			{
				text2 = char.ToUpper(text2[0]).ToString() + text2.Substring(1);
			}
			return text2;
		}

		private void HandleAPrepositionFollowedByDefiniteArticle(string text, string token, ref int cursorPos, StringBuilder outputString)
		{
			string apreposition = this.GetAPreposition(text, token, cursorPos);
			string definiteArticle = this.GetDefiniteArticle(text, token, cursorPos);
			string text2 = string.Empty;
			if (definiteArticle != null && this.CheckIfWordsHaveContraction(apreposition, definiteArticle.Trim(), out text2))
			{
				if (char.IsUpper(token[1]))
				{
					text2 = char.ToUpper(text2[0]).ToString() + text2.Substring(1);
				}
				outputString.Append(text2 + " ");
				return;
			}
			outputString.Append(apreposition + " " + definiteArticle);
		}

		private void HandleDePrepositionFollowedByArticle(string text, string token, ref int cursorPos, StringBuilder outputString)
		{
			string dePreposition = this.GetDePreposition(text, token, cursorPos);
			string definiteArticle = this.GetDefiniteArticle(text, token, cursorPos);
			string text2 = string.Empty;
			if (definiteArticle != null && this.CheckIfWordsHaveContraction(dePreposition, definiteArticle.Trim(), out text2))
			{
				if (char.IsUpper(token[1]))
				{
					text2 = char.ToUpper(text2[0]).ToString() + text2.Substring(1);
				}
				outputString.Append(text2 + " ");
				return;
			}
			outputString.Append(dePreposition + " " + definiteArticle);
		}

		private string GetDePreposition(string text, string token, int cursorPos)
		{
			string text2 = FrenchTextProcessor._dePreposition;
			if (char.IsUpper(token[1]))
			{
				text2 = char.ToUpper(text2[0]).ToString() + text2.Substring(1);
			}
			return text2;
		}

		private void HandleDePreposition(string text, string token, ref int cursorPos, StringBuilder outputString)
		{
			string dePreposition = this.GetDePreposition(text, token, cursorPos);
			string nextWord = this.GetNextWord(text, token, cursorPos);
			bool flag = this.CheckNextCharIsVowel(text, cursorPos) && !this.CheckIfNextWordShouldBeConsideredAConsonant(text, token, cursorPos);
			if (!this.CheckIfWordIsAnArticle(nextWord) && flag)
			{
				outputString.Append(FrenchTextProcessor._dePrepositionWithVowel);
				return;
			}
			string text2;
			if (this.CheckIfWordsHaveContraction(dePreposition, nextWord.Trim(), out text2))
			{
				outputString.Append(text2);
				cursorPos += nextWord.Length;
				return;
			}
			outputString.Append(dePreposition + " ");
		}

		private bool CheckIfNextWordShouldBeConsideredAConsonant(string text, string token, int cursorPos)
		{
			string nextWord = this.GetNextWord(text, token, cursorPos);
			return !string.IsNullOrEmpty(nextWord) && FrenchTextProcessor._shouldBeConsideredConsonants.Contains(nextWord.ToLowerInvariant());
		}

		private bool CheckIfWordsHaveContraction(string t1, string t2, out string result)
		{
			result = string.Empty;
			Dictionary<string, string> dictionary;
			string text;
			if (FrenchTextProcessor.Contractions.TryGetValue(t1.ToLowerInvariant(), out dictionary) && dictionary.TryGetValue(t2.ToLowerInvariant(), out text))
			{
				result = text;
				return true;
			}
			return false;
		}

		private bool CheckIfWordIsAnArticle(string text)
		{
			if (!string.IsNullOrEmpty(text))
			{
				text = text.ToLowerInvariant();
				return FrenchTextProcessor._articles.Contains(text) || text.StartsWith(FrenchTextProcessor._articleVowelStart);
			}
			return false;
		}

		private string GetNextWord(string text, string token, int cursorPos)
		{
			if (cursorPos - token.Length - 2 < text.IndexOf('}'))
			{
				return text.Remove(0, text.IndexOf('}') + 1).Split(new char[] { ' ' })[0];
			}
			return "";
		}

		private FrenchTextProcessor.WordType GetWordTypeFromGender(FrenchTextProcessor.WordGenderEnum gender)
		{
			switch (gender)
			{
			case FrenchTextProcessor.WordGenderEnum.Masculine:
				return FrenchTextProcessor.WordType.Masculine;
			case FrenchTextProcessor.WordGenderEnum.Feminine:
				return FrenchTextProcessor.WordType.Feminine;
			}
			return FrenchTextProcessor.WordType.None;
		}

		private void SetGenderInfo(string token)
		{
			if (token == ".M")
			{
				this.SetMasculine();
				return;
			}
			if (token == ".F")
			{
				this.SetFeminine();
				return;
			}
			if (token == ".N")
			{
				this.SetNeuter();
				return;
			}
			if (token == ".P")
			{
				this.SetPlural();
				return;
			}
			if (token == ".S")
			{
				this.SetSingular();
			}
		}

		private void ProcessWordGroup(string text, string token, int cursorPos)
		{
			string text2 = text.Substring(text.LastIndexOf('}') + 1);
			ValueTuple<string, int, bool> valueTuple;
			if (FrenchTextProcessor.WordGroups.TryGetValue(text2, out valueTuple))
			{
				FrenchTextProcessor.WordGroups[text2] = new ValueTuple<string, int, bool>(valueTuple.Item1, valueTuple.Item2, FrenchTextProcessor._isPlural);
				return;
			}
			FrenchTextProcessor.WordGroups.Add(text2, new ValueTuple<string, int, bool>(token, cursorPos, FrenchTextProcessor._isPlural));
		}

		private void ResetGender()
		{
			FrenchTextProcessor._curGender = FrenchTextProcessor.WordGenderEnum.NoDeclination;
			FrenchTextProcessor._isPlural = false;
		}

		private int ProcessLink(string text, int cursorPos, string token, StringBuilder outputString)
		{
			int num = text.IndexOf(this.LinkEnding, cursorPos);
			if (num > cursorPos)
			{
				string text2 = text.Substring(cursorPos, num - cursorPos);
				string text3 = text2.Substring(0, text2.LastIndexOf('>') + 1);
				string text4 = text2.Substring(text3.Length);
				this.ResetGender();
				ValueTuple<string, int, bool> valueTuple;
				if (token != this.LinkTag && FrenchTextProcessor.WordGroups.TryGetValue(text4, out valueTuple))
				{
					this.SetGenderInfo(valueTuple.Item1);
					if (valueTuple.Item3)
					{
						this.SetPlural();
					}
					else
					{
						this.SetSingular();
					}
				}
				outputString.Append(text3);
				return cursorPos + text3.Length;
			}
			return cursorPos;
		}

		public override CultureInfo CultureInfoForLanguage
		{
			get
			{
				return FrenchTextProcessor.CultureInfo;
			}
		}

		public override void ClearTemporaryData()
		{
			FrenchTextProcessor.WordGroups.Clear();
			FrenchTextProcessor._isPlural = false;
			FrenchTextProcessor._curGender = FrenchTextProcessor.WordGenderEnum.NoDeclination;
		}

		private static char[] Vowels = new char[] { 'a', 'e', 'i', 'o', 'u', 'h' };

		[ThreadStatic]
		private static Dictionary<string, ValueTuple<string, int, bool>> _wordGroups;

		[ThreadStatic]
		private static FrenchTextProcessor.WordGenderEnum _curGender;

		[ThreadStatic]
		private static bool _isPlural = false;

		private static List<string> _articles = new List<string> { "le", "la", "les" };

		private static string _articleVowelStart = "l'";

		private static string _dePreposition = "de";

		private static string _dePrepositionWithVowel = "d'";

		private static string _aPreposition = "à";

		private static readonly Dictionary<string, Dictionary<string, string>> Contractions = new Dictionary<string, Dictionary<string, string>>
		{
			{
				"de",
				new Dictionary<string, string>
				{
					{ "les", "des" },
					{ "le", "du" }
				}
			},
			{
				"à",
				new Dictionary<string, string>
				{
					{ "les", "aux" },
					{ "le", "au" }
				}
			}
		};

		private static Dictionary<FrenchTextProcessor.WordType, string> _genderToDefiniteArticle = new Dictionary<FrenchTextProcessor.WordType, string>
		{
			{
				FrenchTextProcessor.WordType.Masculine,
				"le "
			},
			{
				FrenchTextProcessor.WordType.Feminine,
				"la "
			},
			{
				FrenchTextProcessor.WordType.Plural,
				"les "
			},
			{
				FrenchTextProcessor.WordType.StartingWithaVowel,
				"l'"
			},
			{
				FrenchTextProcessor.WordType.None,
				""
			}
		};

		private static Dictionary<FrenchTextProcessor.WordType, string> _genderToIndefiniteArticle = new Dictionary<FrenchTextProcessor.WordType, string>
		{
			{
				FrenchTextProcessor.WordType.Masculine,
				"un "
			},
			{
				FrenchTextProcessor.WordType.Feminine,
				"une "
			},
			{
				FrenchTextProcessor.WordType.Plural,
				"des "
			},
			{
				FrenchTextProcessor.WordType.StartingWithaVowel,
				""
			},
			{
				FrenchTextProcessor.WordType.None,
				""
			}
		};

		private static List<string> _shouldBeConsideredConsonants = new List<string>
		{
			"hache", "hachette", "héros", "houe", "haute", "hardes", "hachoir", "harnais", "harpon", "haubert",
			"haut", "horde"
		};

		private static readonly CultureInfo CultureInfo = new CultureInfo("fr-FR");

		private enum WordGenderEnum
		{
			Masculine,
			Feminine,
			Neuter,
			NoDeclination
		}

		private enum WordType
		{
			StartingWithaVowel,
			Masculine,
			Feminine,
			Plural,
			None
		}

		private static class GenderTokens
		{
			public const string Masculine = ".M";

			public const string Feminine = ".F";

			public const string Neuter = ".N";

			public const string Plural = ".P";

			public const string Singular = ".S";

			public static readonly List<string> TokenList = new List<string> { ".M", ".F", ".N", ".P", ".S" };
		}

		private static class FunctionTokens
		{
			public const string DefiniteArticle = ".l";

			public const string DefiniteArticleWithBrackets = "{.l}";

			public const string IndefiniteArticle = ".a";

			public const string APreposition = ".c";

			public const string APrepositionFollowedByDefiniteArticle = ".cl";

			public const string DePreposition = ".d";

			public const string DePrepositionFollowedByDefiniteArticle = ".dl";

			public static readonly List<string> TokenList = new List<string> { ".l", ".a", ".d", ".c", ".cl", ".dl" };
		}
	}
}
