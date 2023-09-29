using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TaleWorlds.Library;
using TaleWorlds.Localization.TextProcessor;
using TaleWorlds.Localization.TextProcessor.LanguageProcessors;

namespace TaleWorlds.Localization
{
	public static class MBTextManager
	{
		public static string ActiveTextLanguage
		{
			get
			{
				return MBTextManager._activeTextLanguageId;
			}
		}

		public static bool LocalizationDebugMode { get; set; }

		public static bool LanguageExistsInCurrentConfiguration(string language, bool developmentMode)
		{
			return LocalizedTextManager.GetLanguageIds(developmentMode).Any((string l) => l == language);
		}

		public static bool ChangeLanguage(string language)
		{
			if (LocalizedTextManager.GetLanguageIds(true).Any((string l) => l == language))
			{
				MBTextManager._languageProcessor = LocalizedTextManager.CreateTextProcessorForLanguage(language);
				MBTextManager._activeTextLanguageId = language;
				MBTextManager._activeTextLanguageIndex = LocalizedTextManager.GetLanguageIndex(MBTextManager._activeTextLanguageId);
				LocalizedTextManager.LoadLanguage(MBTextManager._activeTextLanguageId);
				return true;
			}
			Debug.FailedAssert("Invalid language", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\Base\\TaleWorlds.Localization\\MBTextManager.cs", "ChangeLanguage", 141);
			return false;
		}

		public static int GetActiveTextLanguageIndex()
		{
			return MBTextManager._activeTextLanguageIndex;
		}

		public static bool TryChangeVoiceLanguage(string language)
		{
			if (LocalizedVoiceManager.GetVoiceLanguageIds().Any((string l) => l == language))
			{
				MBTextManager._activeVoiceLanguageId = language;
				LocalizedVoiceManager.LoadLanguage(MBTextManager._activeVoiceLanguageId);
				return true;
			}
			return false;
		}

		private static TextObject ProcessNumber(object integer)
		{
			return new TextObject(integer.ToString(), null);
		}

		internal static string ProcessTextToString(TextObject to, bool shouldClear)
		{
			if (to == null)
			{
				return null;
			}
			if (TextObject.IsNullOrEmpty(to))
			{
				return "";
			}
			string localizedText = MBTextManager.GetLocalizedText(to.Value);
			string text;
			if (!string.IsNullOrEmpty(to.Value))
			{
				text = MBTextManager.Process(localizedText, to);
				text = MBTextManager._languageProcessor.Process(text);
				if (shouldClear)
				{
					MBTextManager._languageProcessor.ClearTemporaryData();
				}
			}
			else
			{
				text = "";
			}
			if (MBTextManager.LocalizationDebugMode)
			{
				string text2 = to.GetID();
				if (string.IsNullOrEmpty(text2))
				{
					text2 = "!";
				}
				return "(" + text2 + ") " + text;
			}
			return text;
		}

		internal static string ProcessWithoutLanguageProcessor(TextObject to)
		{
			if (to == null)
			{
				return null;
			}
			if (TextObject.IsNullOrEmpty(to))
			{
				return "";
			}
			string localizedText = MBTextManager.GetLocalizedText(to.Value);
			string text;
			if (!string.IsNullOrEmpty(to.Value))
			{
				text = MBTextManager.Process(localizedText, to);
			}
			else
			{
				text = "";
			}
			return text;
		}

		private static string Process(string query, TextObject parent = null)
		{
			List<MBTextToken> list = null;
			if (parent != null)
			{
				list = parent.GetCachedTokens();
			}
			if (list == null)
			{
				list = MBTextManager.Tokenizer.Tokenize(query);
			}
			return TextGrammarProcessor.Process(MBTextParser.Parse(list), MBTextManager.TextContext, parent);
		}

		public static void ClearAll()
		{
			MBTextManager.TextContext.ClearAll();
		}

		public static void SetTextVariable(string variableName, string text, bool sendClients = false)
		{
			if (text == null)
			{
				return;
			}
			MBTextManager.TextContext.SetTextVariable(variableName, new TextObject(text, null));
		}

		public static void SetTextVariable(string variableName, TextObject text, bool sendClients = false)
		{
			if (text == null)
			{
				return;
			}
			MBTextManager.TextContext.SetTextVariable(variableName, text);
		}

		public static void SetTextVariable(string variableName, int content)
		{
			TextObject textObject = MBTextManager.ProcessNumber(content);
			MBTextManager.SetTextVariable(variableName, textObject, false);
		}

		public static void SetTextVariable(string variableName, float content)
		{
			TextObject textObject = MBTextManager.ProcessNumber(content);
			MBTextManager.SetTextVariable(variableName, textObject, false);
		}

		public static void SetTextVariable(string variableName, object content)
		{
			if (content == null)
			{
				return;
			}
			TextObject textObject = new TextObject(content.ToString(), null);
			MBTextManager.SetTextVariable(variableName, textObject, false);
		}

		public static void SetTextVariable(string variableName, int arrayIndex, object content)
		{
			if (content == null)
			{
				return;
			}
			string text = content.ToString();
			MBTextManager.SetTextVariable(variableName + ":" + arrayIndex, text, false);
		}

		public static void SetFunction(string funcName, string functionBody)
		{
			MBTextModel mbtextModel = MBTextParser.Parse(MBTextManager.Tokenizer.Tokenize(functionBody));
			MBTextManager.TextContext.SetFunction(funcName, mbtextModel);
		}

		public static void ResetFunctions()
		{
			MBTextManager.TextContext.ResetFunctions();
		}

		public static void ThrowLocalizationError(string message)
		{
			Debug.FailedAssert(message, "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\Base\\TaleWorlds.Localization\\MBTextManager.cs", "ThrowLocalizationError", 342);
		}

		internal static string GetLocalizedText(string text)
		{
			if (text != null && text.Length > 2 && text[0] == '{' && text[1] == '=')
			{
				if (MBTextManager._idStringBuilder == null)
				{
					MBTextManager._idStringBuilder = new StringBuilder(8);
				}
				else
				{
					MBTextManager._idStringBuilder.Clear();
				}
				if (MBTextManager._targetStringBuilder == null)
				{
					MBTextManager._targetStringBuilder = new StringBuilder(100);
				}
				else
				{
					MBTextManager._targetStringBuilder.Clear();
				}
				int i = 2;
				while (i < text.Length)
				{
					if (text[i] != '}')
					{
						MBTextManager._idStringBuilder.Append(text[i]);
						i++;
					}
					else
					{
						for (i++; i < text.Length; i++)
						{
							MBTextManager._targetStringBuilder.Append(text[i]);
						}
						string text2 = "";
						if (MBTextManager._activeTextLanguageId == "English")
						{
							text2 = MBTextManager._targetStringBuilder.ToString();
							return MBTextManager.RemoveComments(text2);
						}
						if ((MBTextManager._idStringBuilder.Length != 1 || MBTextManager._idStringBuilder[0] != '*') && (MBTextManager._idStringBuilder.Length != 1 || MBTextManager._idStringBuilder[0] != '!'))
						{
							if (MBTextManager._activeTextLanguageId != "English")
							{
								text2 = LocalizedTextManager.GetTranslatedText(MBTextManager._activeTextLanguageId, MBTextManager._idStringBuilder.ToString());
							}
							if (text2 != null)
							{
								return MBTextManager.RemoveComments(text2);
							}
						}
						IL_164:
						return MBTextManager._targetStringBuilder.ToString();
					}
				}
				goto IL_164;
			}
			return text;
		}

		private static string RemoveComments(string localizedText)
		{
			foreach (object obj in MBTextManager.CommentRemoverRegex.Matches(localizedText))
			{
				Match match = (Match)obj;
				localizedText = localizedText.Replace(match.Value, "");
			}
			return localizedText;
		}

		public static string DiscardAnimationTagsAndCheckAnimationTagPositions(string text)
		{
			return MBTextManager.DiscardAnimationTags(text);
		}

		public static string DiscardAnimationTags(string text)
		{
			string text2 = "";
			bool flag = false;
			for (int i = 0; i < text.Length; i++)
			{
				if (text[i] == '[')
				{
					flag = true;
				}
				if (!flag)
				{
					text2 += text[i].ToString();
				}
				if (text[i] == ']')
				{
					flag = false;
				}
			}
			return text2;
		}

		private static bool CheckAnimationTagPositions(string text)
		{
			string text2 = "";
			Match match = MBTextManager.AnimationTagRemoverRegex.Match(text);
			if (match.Success)
			{
				text2 = MBTextManager.DiscardAnimationTags(match.Value);
			}
			return string.IsNullOrEmpty(text2.Replace(" ", ""));
		}

		public static string[] GetConversationAnimations(TextObject to)
		{
			string text = to.CopyTextObject().ToString();
			StringBuilder stringBuilder = new StringBuilder();
			string[] array = new string[4];
			bool flag = false;
			int num = 0;
			if (!string.IsNullOrEmpty(text))
			{
				for (int i = 0; i < text.Length; i++)
				{
					if (text[i] == '[')
					{
						flag = true;
					}
					else if (flag)
					{
						if (text[i] == ',' || text[i] == ']')
						{
							array[num] = stringBuilder.ToString();
							stringBuilder.Clear();
							if (text[i] == ']')
							{
								flag = false;
							}
						}
						else if (text[i] == ':')
						{
							string text2 = stringBuilder.ToString();
							stringBuilder.Clear();
							if (text2 == "ib")
							{
								num = 0;
							}
							else if (text2 == "if")
							{
								num = 1;
							}
							else if (text2 == "rb")
							{
								num = 2;
							}
							else if (text2 == "rf")
							{
								num = 3;
							}
						}
						else if (text[i] != ' ')
						{
							stringBuilder.Append(text[i]);
						}
					}
				}
			}
			return array;
		}

		public static bool TryGetVoiceObject(TextObject to, out VoiceObject vo)
		{
			if (!TextObject.IsNullOrEmpty(to))
			{
				vo = MBTextManager.ProcessTextForVocalization(to);
				return true;
			}
			vo = null;
			return false;
		}

		private static VoiceObject ProcessTextForVocalization(TextObject to)
		{
			if (TextObject.IsNullOrEmpty(to))
			{
				return null;
			}
			string localizationId = MBTextManager.GetLocalizationId(to);
			if (localizationId != "!")
			{
				return LocalizedVoiceManager.GetLocalizedVoice(localizationId);
			}
			Debug.Print("[VOICEOVER]VoiceObject search for: " + localizationId, 0, Debug.DebugColor.White, 17592186044416UL);
			List<MBTextToken> list = to.GetCachedTokens();
			if (list == null)
			{
				list = MBTextManager.Tokenizer.Tokenize(to.Value);
			}
			foreach (MBTextToken mbtextToken in list)
			{
				if (mbtextToken.TokenType == TokenType.Identifier)
				{
					VoiceObject voiceObject = MBTextManager.ProcessTextForVocalization(MBTextManager.TextContext.GetRawTextVariable(mbtextToken.Value, to));
					if (voiceObject != null)
					{
						return voiceObject;
					}
				}
			}
			return null;
		}

		private static string GetLocalizationId(TextObject to)
		{
			string value = to.Value;
			if (value != null && value.Length > 2 && value[0] == '{' && value[1] == '=')
			{
				int num = 2;
				for (int i = num; i < value.Length; i++)
				{
					if (value[i] == '}')
					{
						IL_4F:
						return value.Substring(num, i - num);
					}
				}
				goto IL_4F;
			}
			return string.Empty;
		}

		public const string LinkAttribute = "LINK";

		internal const string LinkTag = ".link";

		internal const int LinkTagLength = 7;

		internal const string LinkEnding = "</b></a>";

		internal const int LinkEndingLength = 8;

		internal const string LinkStarter = "<a style=\"Link.";

		private const string CommentRegexPattern = "{%.+?}";

		private const string AnimationTagsRegexPattern = "\\[.+\\]";

		private static readonly TextProcessingContext TextContext = new TextProcessingContext();

		private static LanguageSpecificTextProcessor _languageProcessor = new EnglishTextProcessor();

		private static string _activeVoiceLanguageId = "English";

		private static string _activeTextLanguageId = "English";

		private static int _activeTextLanguageIndex = 0;

		[ThreadStatic]
		private static StringBuilder _idStringBuilder;

		[ThreadStatic]
		private static StringBuilder _targetStringBuilder;

		private static readonly Regex CommentRemoverRegex = new Regex("{%.+?}");

		private static readonly Regex AnimationTagRemoverRegex = new Regex("\\[.+\\]");

		internal static readonly Tokenizer Tokenizer = new Tokenizer();
	}
}
