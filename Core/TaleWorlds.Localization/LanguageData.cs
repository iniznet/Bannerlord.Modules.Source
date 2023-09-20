using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using TaleWorlds.Library;

namespace TaleWorlds.Localization
{
	internal class LanguageData
	{
		public static MBReadOnlyList<LanguageData> All
		{
			get
			{
				return LanguageData._all;
			}
		}

		private static MBList<LanguageData> _all { get; } = new MBList<LanguageData>();

		public string Title { get; private set; }

		public string TextProcessor { get; private set; }

		public string[] SupportedIsoCodes { get; private set; }

		public string SubtitleExtension { get; private set; }

		public bool IsUnderDevelopment { get; private set; }

		public MBReadOnlyList<string> XmlPaths
		{
			get
			{
				return this._xmlPaths;
			}
		}

		public IReadOnlyDictionary<string, string> VoiceXmlPathsAndModulePaths
		{
			get
			{
				return this._voiceXmlPathsAndModulePaths;
			}
		}

		public bool IsValid { get; private set; }

		public LanguageData(string stringId)
		{
			this.StringId = stringId;
			this.IsUnderDevelopment = true;
			this.SupportedIsoCodes = new string[0];
			this._xmlPaths = new MBList<string>();
			this._voiceXmlPathsAndModulePaths = new Dictionary<string, string>();
			this.IsValid = false;
		}

		public void InitializeDefault(string title, string[] supportedIsoCodes, string subtitleExtension, string textProcessor, bool isUnderDevelopment)
		{
			this.Title = title;
			this.TextProcessor = textProcessor;
			this.SubtitleExtension = subtitleExtension;
			this.SupportedIsoCodes = supportedIsoCodes;
			this.IsUnderDevelopment = isUnderDevelopment;
			this.IsValid = this.SupportedIsoCodes.Length != 0;
		}

		public static void Clear()
		{
			LanguageData._all.Clear();
		}

		public static LanguageData GetLanguageData(string stringId)
		{
			foreach (LanguageData languageData in LanguageData.All)
			{
				if (languageData.StringId == stringId)
				{
					return languageData;
				}
			}
			return null;
		}

		public static int GetLanguageDataIndex(string stringId)
		{
			for (int i = 0; i < LanguageData._all.Count; i++)
			{
				if (LanguageData._all[i].StringId == stringId)
				{
					return i;
				}
			}
			return -1;
		}

		private void Deserialize(XmlNode node, string modulePath)
		{
			if (node.Attributes == null)
			{
				throw new TWXmlLoadException("LanguageData node does not have any Attributes!");
			}
			XmlAttribute xmlAttribute = node.Attributes["name"];
			string text = ((xmlAttribute != null) ? xmlAttribute.Value : null);
			if (!string.IsNullOrEmpty(text))
			{
				this.Title = text;
			}
			XmlAttribute xmlAttribute2 = node.Attributes["subtitle_extension"];
			string text2 = ((xmlAttribute2 != null) ? xmlAttribute2.Value : null);
			if (!string.IsNullOrEmpty(text2))
			{
				this.SubtitleExtension = text2;
			}
			XmlAttribute xmlAttribute3 = node.Attributes["supported_iso"];
			string text3 = ((xmlAttribute3 != null) ? xmlAttribute3.Value : null);
			if (text3 != null)
			{
				string[] array = text3.Split(new char[] { ',' });
				this.SupportedIsoCodes = new List<string>(this.SupportedIsoCodes).Union(array).ToArray<string>();
			}
			XmlAttribute xmlAttribute4 = node.Attributes["text_processor"];
			string text4 = ((xmlAttribute4 != null) ? xmlAttribute4.Value : null);
			if (!string.IsNullOrEmpty(text))
			{
				this.TextProcessor = text4;
			}
			XmlAttribute xmlAttribute5 = node.Attributes["under_development"];
			if (xmlAttribute5 != null)
			{
				bool flag;
				bool.TryParse(xmlAttribute5.Value, out flag);
				this.IsUnderDevelopment = flag;
			}
			this.IsValid = this.SupportedIsoCodes.Length != 0;
			if (node.HasChildNodes)
			{
				for (node = node.FirstChild; node != null; node = node.NextSibling)
				{
					if (node.Name == "LanguageFile" && node.NodeType != XmlNodeType.Comment && node.Attributes != null)
					{
						XmlAttribute xmlAttribute6 = node.Attributes["xml_path"];
						string text5 = ((xmlAttribute6 != null) ? xmlAttribute6.Value : null);
						if (!string.IsNullOrEmpty(text5) && !this._xmlPaths.Contains(text5))
						{
							this._xmlPaths.Add(modulePath + "/" + text5);
						}
					}
					if (node.Name == "VoiceFile" && node.NodeType != XmlNodeType.Comment && node.Attributes != null)
					{
						XmlAttribute xmlAttribute7 = node.Attributes["xml_path"];
						string text6 = ((xmlAttribute7 != null) ? xmlAttribute7.Value : null);
						string text7 = modulePath + "/" + text6;
						if (!string.IsNullOrEmpty(text6) && !this._voiceXmlPathsAndModulePaths.ContainsKey(text7))
						{
							this._voiceXmlPathsAndModulePaths.Add(text7, modulePath);
						}
					}
				}
			}
		}

		public static void LoadFromXml(XmlDocument doc, string modulePath)
		{
			Debug.Print("Loading localized text xml: " + doc.Name, 0, Debug.DebugColor.White, 17592186044416UL);
			if (doc.HasChildNodes)
			{
				for (XmlNode xmlNode = doc.FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
				{
					if (xmlNode.Name == "LanguageData" && xmlNode.NodeType != XmlNodeType.Comment && xmlNode.Attributes != null)
					{
						XmlAttribute xmlAttribute = xmlNode.Attributes["id"];
						string text = ((xmlAttribute != null) ? xmlAttribute.Value : null);
						if (!string.IsNullOrEmpty(text))
						{
							LanguageData languageData = LanguageData.GetLanguageData(text);
							if (languageData == null)
							{
								languageData = new LanguageData(text);
								LanguageData._all.Add(languageData);
							}
							languageData.Deserialize(xmlNode, modulePath);
						}
					}
				}
			}
		}

		public static void LoadTestData(LanguageData data)
		{
			int languageDataIndex = LanguageData.GetLanguageDataIndex(data.StringId);
			if (languageDataIndex == -1)
			{
				LanguageData._all.Add(data);
				return;
			}
			LanguageData._all[languageDataIndex] = data;
		}

		public readonly string StringId;

		private readonly MBList<string> _xmlPaths;

		private readonly Dictionary<string, string> _voiceXmlPathsAndModulePaths;
	}
}
