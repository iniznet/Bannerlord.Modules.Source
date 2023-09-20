using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace TaleWorlds.Localization.TextProcessor.LanguageProcessors
{
	// Token: 0x02000037 RID: 55
	public class SpanishTextProcessor : LanguageSpecificTextProcessor
	{
		// Token: 0x06000257 RID: 599 RVA: 0x000182F4 File Offset: 0x000164F4
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

		// Token: 0x06000258 RID: 600 RVA: 0x00018341 File Offset: 0x00016541
		private bool CheckWhiteSpaceAndTextEnd(string sourceText, int cursorPos)
		{
			return cursorPos < sourceText.Length && !char.IsWhiteSpace(sourceText[cursorPos]);
		}

		// Token: 0x06000259 RID: 601 RVA: 0x00018360 File Offset: 0x00016560
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

		// Token: 0x0600025A RID: 602 RVA: 0x000183E8 File Offset: 0x000165E8
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

		// Token: 0x0600025B RID: 603 RVA: 0x000184B0 File Offset: 0x000166B0
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

		// Token: 0x0600025C RID: 604 RVA: 0x000184FC File Offset: 0x000166FC
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

		// Token: 0x17000074 RID: 116
		// (get) Token: 0x0600025D RID: 605 RVA: 0x00018535 File Offset: 0x00016735
		public override CultureInfo CultureInfoForLanguage
		{
			get
			{
				return SpanishTextProcessor.CultureInfo;
			}
		}

		// Token: 0x0600025E RID: 606 RVA: 0x0001853C File Offset: 0x0001673C
		public override void ClearTemporaryData()
		{
			SpanishTextProcessor._curGender = SpanishTextProcessor.WordGenderEnum.NoDeclination;
		}

		// Token: 0x040000F2 RID: 242
		[ThreadStatic]
		private static SpanishTextProcessor.WordGenderEnum _curGender;

		// Token: 0x040000F3 RID: 243
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

		// Token: 0x040000F4 RID: 244
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

		// Token: 0x040000F5 RID: 245
		private static readonly CultureInfo CultureInfo = new CultureInfo("es-es");

		// Token: 0x0200005D RID: 93
		private enum WordGenderEnum
		{
			// Token: 0x0400022D RID: 557
			MasculineSingular,
			// Token: 0x0400022E RID: 558
			MasculinePlural,
			// Token: 0x0400022F RID: 559
			FeminineSingular,
			// Token: 0x04000230 RID: 560
			FemininePlural,
			// Token: 0x04000231 RID: 561
			NeuterSingular,
			// Token: 0x04000232 RID: 562
			NeuterPlural,
			// Token: 0x04000233 RID: 563
			NoDeclination
		}

		// Token: 0x0200005E RID: 94
		private static class GenderTokens
		{
			// Token: 0x04000234 RID: 564
			public const string MasculineSingular = ".MS";

			// Token: 0x04000235 RID: 565
			public const string MasculinePlural = ".MP";

			// Token: 0x04000236 RID: 566
			public const string FeminineSingular = ".FS";

			// Token: 0x04000237 RID: 567
			public const string FemininePlural = ".FP";

			// Token: 0x04000238 RID: 568
			public const string NeuterSingular = ".NS";

			// Token: 0x04000239 RID: 569
			public const string NeuterPlural = ".NP";

			// Token: 0x0400023A RID: 570
			public static readonly List<string> TokenList = new List<string> { ".MS", ".FS", ".NS", ".MP", ".FP", ".NP" };
		}

		// Token: 0x0200005F RID: 95
		private static class FunctionTokens
		{
			// Token: 0x0400023B RID: 571
			public const string DefiniteArticle = ".l";

			// Token: 0x0400023C RID: 572
			public const string DefiniteArticleInUpperCase = ".L";
		}
	}
}
