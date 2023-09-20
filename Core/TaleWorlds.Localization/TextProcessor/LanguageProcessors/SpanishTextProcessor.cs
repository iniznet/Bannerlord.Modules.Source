using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace TaleWorlds.Localization.TextProcessor.LanguageProcessors
{
	public class SpanishTextProcessor : LanguageSpecificTextProcessor
	{
		public override void ProcessToken(string sourceText, ref int cursorPos, string token, StringBuilder outputString)
		{
			if (SpanishTextProcessor.GenderTokens.TokenList.Contains(token))
			{
				this.SetGender(token);
			}
			if (token == ".l" || token == ".L")
			{
				this.HandleDefiniteArticles(sourceText, token, cursorPos, outputString);
				SpanishTextProcessor._curGender = SpanishTextProcessor.WordGenderEnum.NoDeclination;
			}
		}

		private bool CheckWhiteSpaceAndTextEnd(string sourceText, int cursorPos)
		{
			return cursorPos < sourceText.Length && !char.IsWhiteSpace(sourceText[cursorPos]);
		}

		private void SetGender(string token)
		{
			if (token == ".MS")
			{
				SpanishTextProcessor._curGender = SpanishTextProcessor.WordGenderEnum.MasculineSingular;
				return;
			}
			if (token == ".MP")
			{
				SpanishTextProcessor._curGender = SpanishTextProcessor.WordGenderEnum.MasculinePlural;
				return;
			}
			if (token == ".FS")
			{
				SpanishTextProcessor._curGender = SpanishTextProcessor.WordGenderEnum.FeminineSingular;
				return;
			}
			if (token == ".FP")
			{
				SpanishTextProcessor._curGender = SpanishTextProcessor.WordGenderEnum.FemininePlural;
				return;
			}
			if (token == ".NS")
			{
				SpanishTextProcessor._curGender = SpanishTextProcessor.WordGenderEnum.NeuterSingular;
				return;
			}
			if (!(token == ".NP"))
			{
				return;
			}
			SpanishTextProcessor._curGender = SpanishTextProcessor.WordGenderEnum.NeuterPlural;
		}

		private void HandleDefiniteArticles(string text, string token, int cursorPos, StringBuilder stringBuilder)
		{
			if (!this.CheckWhiteSpaceAndTextEnd(text, cursorPos))
			{
				return;
			}
			if (SpanishTextProcessor._curGender == SpanishTextProcessor.WordGenderEnum.MasculineSingular || SpanishTextProcessor._curGender == SpanishTextProcessor.WordGenderEnum.MasculinePlural || SpanishTextProcessor._curGender == SpanishTextProcessor.WordGenderEnum.FeminineSingular || SpanishTextProcessor._curGender == SpanishTextProcessor.WordGenderEnum.FemininePlural)
			{
				string text2 = SpanishTextProcessor._genderToDefiniteArticle[SpanishTextProcessor._curGender];
				bool flag = false;
				string text3;
				if (SpanishTextProcessor._curGender == SpanishTextProcessor.WordGenderEnum.MasculineSingular && this.HandleContractions(text, text2, cursorPos, out text3))
				{
					text2 = text3;
					flag = true;
					if (char.IsWhiteSpace(stringBuilder[stringBuilder.Length - 1]))
					{
						stringBuilder.Remove(stringBuilder.Length - 1, 1);
					}
				}
				if (!flag && token == ".L")
				{
					text2 = char.ToUpper(text2[0]).ToString() + text2.Substring(1);
				}
				stringBuilder.Append(text2);
			}
		}

		private bool HandleContractions(string text, string article, int cursorPos, out string newVersion)
		{
			string previousWord = this.GetPreviousWord(text, cursorPos);
			Dictionary<string, string> dictionary;
			if (SpanishTextProcessor.Contractions.TryGetValue(previousWord.ToLower(), out dictionary) && dictionary.TryGetValue(article.TrimEnd(Array.Empty<char>()), out newVersion))
			{
				return true;
			}
			newVersion = string.Empty;
			return false;
		}

		private string GetPreviousWord(string sourceText, int cursorPos)
		{
			string[] array = sourceText.Substring(0, cursorPos).Split(new char[] { ' ' });
			int num = array.Length;
			if (num < 2)
			{
				return "";
			}
			return array[num - 2];
		}

		public override CultureInfo CultureInfoForLanguage
		{
			get
			{
				return SpanishTextProcessor.CultureInfo;
			}
		}

		public override void ClearTemporaryData()
		{
			SpanishTextProcessor._curGender = SpanishTextProcessor.WordGenderEnum.NoDeclination;
		}

		[ThreadStatic]
		private static SpanishTextProcessor.WordGenderEnum _curGender;

		private static readonly Dictionary<string, Dictionary<string, string>> Contractions = new Dictionary<string, Dictionary<string, string>>
		{
			{
				"de",
				new Dictionary<string, string> { { "el", "l " } }
			},
			{
				"a",
				new Dictionary<string, string> { { "el", "l " } }
			}
		};

		private static Dictionary<SpanishTextProcessor.WordGenderEnum, string> _genderToDefiniteArticle = new Dictionary<SpanishTextProcessor.WordGenderEnum, string>
		{
			{
				SpanishTextProcessor.WordGenderEnum.MasculineSingular,
				"el "
			},
			{
				SpanishTextProcessor.WordGenderEnum.MasculinePlural,
				"los "
			},
			{
				SpanishTextProcessor.WordGenderEnum.FeminineSingular,
				"la "
			},
			{
				SpanishTextProcessor.WordGenderEnum.FemininePlural,
				"las "
			},
			{
				SpanishTextProcessor.WordGenderEnum.NeuterSingular,
				""
			},
			{
				SpanishTextProcessor.WordGenderEnum.NeuterPlural,
				""
			}
		};

		private static readonly CultureInfo CultureInfo = new CultureInfo("es-es");

		private enum WordGenderEnum
		{
			MasculineSingular,
			MasculinePlural,
			FeminineSingular,
			FemininePlural,
			NeuterSingular,
			NeuterPlural,
			NoDeclination
		}

		private static class GenderTokens
		{
			public const string MasculineSingular = ".MS";

			public const string MasculinePlural = ".MP";

			public const string FeminineSingular = ".FS";

			public const string FemininePlural = ".FP";

			public const string NeuterSingular = ".NS";

			public const string NeuterPlural = ".NP";

			public static readonly List<string> TokenList = new List<string> { ".MS", ".FS", ".NS", ".MP", ".FP", ".NP" };
		}

		private static class FunctionTokens
		{
			public const string DefiniteArticle = ".l";

			public const string DefiniteArticleInUpperCase = ".L";
		}
	}
}
