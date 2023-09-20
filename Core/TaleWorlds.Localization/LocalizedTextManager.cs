using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;
using TaleWorlds.Library;
using TaleWorlds.Localization.TextProcessor;

namespace TaleWorlds.Localization
{
	// Token: 0x02000007 RID: 7
	public static class LocalizedTextManager
	{
		// Token: 0x06000046 RID: 70 RVA: 0x00002E34 File Offset: 0x00001034
		public static string GetTranslatedText(string languageId, string id)
		{
			LocalizedText localizedText;
			if (LocalizedTextManager._gameTextDictionary.TryGetValue(id, out localizedText))
			{
				return localizedText.GetTranslatedText(languageId);
			}
			return null;
		}

		// Token: 0x06000047 RID: 71 RVA: 0x00002E5C File Offset: 0x0000105C
		public static List<string> GetLanguageIds(bool developmentMode)
		{
			List<string> list = new List<string>();
			foreach (LanguageData languageData in LanguageData.All)
			{
				bool flag = developmentMode || !languageData.IsUnderDevelopment;
				if (languageData != null && languageData.IsValid && flag)
				{
					list.Add(languageData.StringId);
				}
			}
			return list;
		}

		// Token: 0x06000048 RID: 72 RVA: 0x00002EDC File Offset: 0x000010DC
		public static string GetLanguageTitle(string id)
		{
			LanguageData languageData = LanguageData.GetLanguageData(id);
			if (languageData != null)
			{
				return languageData.Title;
			}
			return LanguageData.GetLanguageData("English").Title;
		}

		// Token: 0x06000049 RID: 73 RVA: 0x00002F0C File Offset: 0x0000110C
		public static LanguageSpecificTextProcessor CreateTextProcessorForLanguage(string id)
		{
			LanguageData languageData = LanguageData.GetLanguageData(id);
			if (languageData == null || languageData.TextProcessor == null)
			{
				return new DefaultTextProcessor();
			}
			Type type = Type.GetType(languageData.TextProcessor);
			if (type == null)
			{
				Debug.FailedAssert("Can't find the type: " + languageData.TextProcessor, "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\Base\\TaleWorlds.Localization\\LocalizedTextManager.cs", "CreateTextProcessorForLanguage", 71);
				return new DefaultTextProcessor();
			}
			return (LanguageSpecificTextProcessor)Activator.CreateInstance(type);
		}

		// Token: 0x0600004A RID: 74 RVA: 0x00002F78 File Offset: 0x00001178
		public static void AddLanguageTest(string id, string processor)
		{
			LanguageData languageData = new LanguageData(id);
			languageData.InitializeDefault(id, new string[] { id }, id, processor, false);
			LanguageData.LoadTestData(languageData);
		}

		// Token: 0x0600004B RID: 75 RVA: 0x00002FA8 File Offset: 0x000011A8
		public static int GetLanguageIndex(string id)
		{
			int num = LanguageData.GetLanguageDataIndex(id);
			if (num == -1)
			{
				num = LanguageData.GetLanguageDataIndex("English");
			}
			return num;
		}

		// Token: 0x0600004C RID: 76 RVA: 0x00002FCC File Offset: 0x000011CC
		public static void LoadLocalizationXmls(string[] loadedModules)
		{
			LanguageData.Clear();
			for (int i = 0; i < loadedModules.Length; i++)
			{
				string text = loadedModules[i] + "/ModuleData/Languages";
				if (Directory.Exists(text))
				{
					string[] files = Directory.GetFiles(text, "language_data.xml", SearchOption.AllDirectories);
					for (int j = 0; j < files.Length; j++)
					{
						XmlDocument xmlDocument = LocalizedTextManager.LoadXmlFile(files[j]);
						if (xmlDocument != null)
						{
							LanguageData.LoadFromXml(xmlDocument, text);
						}
					}
				}
			}
		}

		// Token: 0x0600004D RID: 77 RVA: 0x0000303C File Offset: 0x0000123C
		public static string GetDateFormattedByLanguage(string languageCode, DateTime dateTime)
		{
			string shortDatePattern = LocalizedTextManager.GetCultureInfo(languageCode).DateTimeFormat.ShortDatePattern;
			return dateTime.ToString(shortDatePattern);
		}

		// Token: 0x0600004E RID: 78 RVA: 0x00003064 File Offset: 0x00001264
		public static string GetTimeFormattedByLanguage(string languageCode, DateTime dateTime)
		{
			string shortTimePattern = LocalizedTextManager.GetCultureInfo(languageCode).DateTimeFormat.ShortTimePattern;
			return dateTime.ToString(shortTimePattern);
		}

		// Token: 0x0600004F RID: 79 RVA: 0x0000308A File Offset: 0x0000128A
		public static string GetSubtitleExtensionOfLanguage(string languageId)
		{
			return LocalizedTextManager.GetLanguageData(languageId).SubtitleExtension;
		}

		// Token: 0x06000050 RID: 80 RVA: 0x00003098 File Offset: 0x00001298
		public static string GetLocalizationCodeOfISOLanguageCode(string isoLanguageCode)
		{
			foreach (LanguageData languageData in LanguageData.All)
			{
				string[] supportedIsoCodes = languageData.SupportedIsoCodes;
				for (int i = 0; i < supportedIsoCodes.Length; i++)
				{
					if (string.Equals(supportedIsoCodes[i], isoLanguageCode, StringComparison.InvariantCultureIgnoreCase))
					{
						return languageData.StringId;
					}
				}
			}
			Debug.FailedAssert("Undefined language code " + isoLanguageCode, "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\Base\\TaleWorlds.Localization\\LocalizedTextManager.cs", "GetLocalizationCodeOfISOLanguageCode", 166);
			return "English";
		}

		// Token: 0x06000051 RID: 81 RVA: 0x00003138 File Offset: 0x00001338
		private static CultureInfo GetCultureInfo(string languageId)
		{
			LanguageData languageData = LocalizedTextManager.GetLanguageData(languageId);
			CultureInfo cultureInfo = CultureInfo.InvariantCulture;
			if (languageData.SupportedIsoCodes != null && languageData.SupportedIsoCodes.Length != 0)
			{
				cultureInfo = new CultureInfo(languageData.SupportedIsoCodes[0]);
			}
			return cultureInfo;
		}

		// Token: 0x06000052 RID: 82 RVA: 0x00003174 File Offset: 0x00001374
		private static LanguageData GetLanguageData(string languageId)
		{
			LanguageData languageData = LanguageData.GetLanguageData(languageId);
			if (languageData == null || !languageData.IsValid)
			{
				languageData = LanguageData.GetLanguageData("English");
				Debug.FailedAssert("Undefined language code: " + languageId, "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\Base\\TaleWorlds.Localization\\LocalizedTextManager.cs", "GetLanguageData", 189);
			}
			return languageData;
		}

		// Token: 0x06000053 RID: 83 RVA: 0x000031C0 File Offset: 0x000013C0
		private static XmlDocument LoadXmlFile(string path)
		{
			try
			{
				Debug.Print("opening " + path, 0, Debug.DebugColor.White, 17592186044416UL);
				XmlDocument xmlDocument = new XmlDocument();
				StreamReader streamReader = new StreamReader(path);
				string text = streamReader.ReadToEnd();
				xmlDocument.LoadXml(text);
				streamReader.Close();
				return xmlDocument;
			}
			catch
			{
				Debug.FailedAssert("Could not parse: " + path, "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\Base\\TaleWorlds.Localization\\LocalizedTextManager.cs", "LoadXmlFile", 209);
			}
			return null;
		}

		// Token: 0x06000054 RID: 84 RVA: 0x00003244 File Offset: 0x00001444
		internal static void LoadLanguage(string languageId)
		{
			LocalizedTextManager._gameTextDictionary.Clear();
			LanguageData languageData = LanguageData.GetLanguageData(languageId);
			if (languageData != null)
			{
				LocalizedTextManager.LoadLanguage(languageData);
			}
		}

		// Token: 0x06000055 RID: 85 RVA: 0x0000326C File Offset: 0x0000146C
		private static void LoadLanguage(LanguageData language)
		{
			MBTextManager.ResetFunctions();
			string stringId = language.StringId;
			bool flag = stringId != "English";
			foreach (string text in language.XmlPaths)
			{
				XmlDocument xmlDocument = LocalizedTextManager.LoadXmlFile(text);
				if (xmlDocument != null)
				{
					for (XmlNode xmlNode = xmlDocument.ChildNodes[1].FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
					{
						if (xmlNode.Name == "strings" && xmlNode.HasChildNodes)
						{
							if (flag)
							{
								for (XmlNode xmlNode2 = xmlNode.FirstChild; xmlNode2 != null; xmlNode2 = xmlNode2.NextSibling)
								{
									if (xmlNode2.Name == "string" && xmlNode2.NodeType != XmlNodeType.Comment)
									{
										LocalizedTextManager.DeserializeStrings(xmlNode2, stringId);
									}
								}
							}
						}
						else if (xmlNode.Name == "functions" && xmlNode.HasChildNodes)
						{
							for (XmlNode xmlNode3 = xmlNode.FirstChild; xmlNode3 != null; xmlNode3 = xmlNode3.NextSibling)
							{
								if (xmlNode3.Name == "function" && xmlNode3.NodeType != XmlNodeType.Comment)
								{
									string value = xmlNode3.Attributes["functionName"].Value;
									string value2 = xmlNode3.Attributes["functionBody"].Value;
									MBTextManager.SetFunction(value, value2);
								}
							}
						}
					}
				}
			}
			Debug.Print("Loading localized text xml.", 0, Debug.DebugColor.White, 17592186044416UL);
		}

		// Token: 0x06000056 RID: 86 RVA: 0x00003418 File Offset: 0x00001618
		private static void DeserializeStrings(XmlNode node, string languageId)
		{
			if (node.Attributes == null)
			{
				throw new TWXmlLoadException("Node attributes are null!");
			}
			string value = node.Attributes["id"].Value;
			string value2 = node.Attributes["text"].Value;
			if (!LocalizedTextManager._gameTextDictionary.ContainsKey(value))
			{
				LocalizedText localizedText = new LocalizedText();
				LocalizedTextManager._gameTextDictionary.Add(value, localizedText);
			}
			LocalizedTextManager._gameTextDictionary[value].AddTranslation(languageId, value2);
		}

		// Token: 0x06000057 RID: 87 RVA: 0x00003498 File Offset: 0x00001698
		[CommandLineFunctionality.CommandLineArgumentFunction("change_language", "localization")]
		public static string ChangeLanguage(List<string> strings)
		{
			if (strings.Count != 1)
			{
				return "Format is \"localization.change_language [LanguageCode/LanguageName/ISOCode]\".";
			}
			string text = strings[0];
			int activeTextLanguageIndex = MBTextManager.GetActiveTextLanguageIndex();
			string text2 = null;
			foreach (string text3 in LocalizedTextManager.GetLanguageIds(true))
			{
				if (LocalizedTextManager.GetLanguageTitle(text3).Equals(text, StringComparison.OrdinalIgnoreCase) || LocalizedTextManager.GetSubtitleExtensionOfLanguage(text3).Contains(text))
				{
					text2 = text3;
					break;
				}
			}
			if (string.IsNullOrEmpty(text2))
			{
				return "cant find the language in current configuration.";
			}
			if (LocalizedTextManager.GetLanguageIndex(text2) == activeTextLanguageIndex)
			{
				return "Same language";
			}
			MBTextManager.ChangeLanguage(text2);
			return "New language is " + text2;
		}

		// Token: 0x06000058 RID: 88 RVA: 0x00003558 File Offset: 0x00001758
		[CommandLineFunctionality.CommandLineArgumentFunction("reload_texts", "localization")]
		public static string ReloadTexts(List<string> strings)
		{
			LocalizedTextManager.LoadLanguage(MBTextManager.ActiveTextLanguage);
			return "OK";
		}

		// Token: 0x06000059 RID: 89 RVA: 0x0000356C File Offset: 0x0000176C
		[CommandLineFunctionality.CommandLineArgumentFunction("check_for_errors", "localization")]
		public static string CheckValidity(List<string> strings)
		{
			if (File.Exists("faulty_translation_lines.txt"))
			{
				File.Delete("faulty_translation_lines.txt");
			}
			bool flag = false;
			foreach (string text in LocalizedTextManager.GetLanguageIds(false))
			{
				MBTextManager.ChangeLanguage(text);
				LocalizedTextManager.<CheckValidity>g__Write|22_0("Testing Language: " + MBTextManager.ActiveTextLanguage + "\n\n");
				foreach (KeyValuePair<string, LocalizedText> keyValuePair in LocalizedTextManager._gameTextDictionary)
				{
					string key = keyValuePair.Key;
					string text2;
					bool flag2 = keyValuePair.Value.CheckValidity(key, out text2);
					if (flag2)
					{
						LocalizedTextManager.<CheckValidity>g__Write|22_0(text2);
					}
					flag = flag2 || flag;
				}
				LocalizedTextManager.<CheckValidity>g__Write|22_0("\nTesting Language: " + MBTextManager.ActiveTextLanguage + "\n\n");
			}
			if (!flag)
			{
				return "No errors are found.";
			}
			return "Errors are written into 'faulty_translation_lines.txt' file in the binary folder.";
		}

		// Token: 0x0600005B RID: 91 RVA: 0x00003688 File Offset: 0x00001888
		[CompilerGenerated]
		internal static void <CheckValidity>g__Write|22_0(string s)
		{
			File.AppendAllText("faulty_translation_lines.txt", s, Encoding.Unicode);
		}

		// Token: 0x04000011 RID: 17
		public const string LanguageDataFileName = "language_data";

		// Token: 0x04000012 RID: 18
		public const string DefaultEnglishLanguageId = "English";

		// Token: 0x04000013 RID: 19
		private static readonly Dictionary<string, LocalizedText> _gameTextDictionary = new Dictionary<string, LocalizedText>();
	}
}
