using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using TaleWorlds.Library;

namespace TaleWorlds.Localization
{
	// Token: 0x02000002 RID: 2
	internal class LanguageData
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000001 RID: 1 RVA: 0x00002048 File Offset: 0x00000248
		public static MBReadOnlyList<LanguageData> All
		{
			get
			{
				return LanguageData._all;
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000002 RID: 2 RVA: 0x0000204F File Offset: 0x0000024F
		private static MBList<LanguageData> _all { get; } = new MBList<LanguageData>();

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000003 RID: 3 RVA: 0x00002056 File Offset: 0x00000256
		// (set) Token: 0x06000004 RID: 4 RVA: 0x0000205E File Offset: 0x0000025E
		public string Title { get; private set; }

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000005 RID: 5 RVA: 0x00002067 File Offset: 0x00000267
		// (set) Token: 0x06000006 RID: 6 RVA: 0x0000206F File Offset: 0x0000026F
		public string TextProcessor { get; private set; }

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000007 RID: 7 RVA: 0x00002078 File Offset: 0x00000278
		// (set) Token: 0x06000008 RID: 8 RVA: 0x00002080 File Offset: 0x00000280
		public string[] SupportedIsoCodes { get; private set; }

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000009 RID: 9 RVA: 0x00002089 File Offset: 0x00000289
		// (set) Token: 0x0600000A RID: 10 RVA: 0x00002091 File Offset: 0x00000291
		public string SubtitleExtension { get; private set; }

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x0600000B RID: 11 RVA: 0x0000209A File Offset: 0x0000029A
		// (set) Token: 0x0600000C RID: 12 RVA: 0x000020A2 File Offset: 0x000002A2
		public bool IsUnderDevelopment { get; private set; }

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x0600000D RID: 13 RVA: 0x000020AB File Offset: 0x000002AB
		public MBReadOnlyList<string> XmlPaths
		{
			get
			{
				return this._xmlPaths;
			}
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x0600000E RID: 14 RVA: 0x000020B3 File Offset: 0x000002B3
		public IReadOnlyDictionary<string, string> VoiceXmlPathsAndModulePaths
		{
			get
			{
				return this._voiceXmlPathsAndModulePaths;
			}
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x0600000F RID: 15 RVA: 0x000020BB File Offset: 0x000002BB
		// (set) Token: 0x06000010 RID: 16 RVA: 0x000020C3 File Offset: 0x000002C3
		public bool IsValid { get; private set; }

		// Token: 0x06000011 RID: 17 RVA: 0x000020CC File Offset: 0x000002CC
		public LanguageData(string stringId)
		{
			this.StringId = stringId;
			this.IsUnderDevelopment = true;
			this.SupportedIsoCodes = new string[0];
			this._xmlPaths = new MBList<string>();
			this._voiceXmlPathsAndModulePaths = new Dictionary<string, string>();
			this.IsValid = false;
		}

		// Token: 0x06000012 RID: 18 RVA: 0x0000210B File Offset: 0x0000030B
		public void InitializeDefault(string title, string[] supportedIsoCodes, string subtitleExtension, string textProcessor, bool isUnderDevelopment)
		{
			this.Title = title;
			this.TextProcessor = textProcessor;
			this.SubtitleExtension = subtitleExtension;
			this.SupportedIsoCodes = supportedIsoCodes;
			this.IsUnderDevelopment = isUnderDevelopment;
			this.IsValid = this.SupportedIsoCodes.Length != 0;
		}

		// Token: 0x06000013 RID: 19 RVA: 0x00002142 File Offset: 0x00000342
		public static void Clear()
		{
			LanguageData._all.Clear();
		}

		// Token: 0x06000014 RID: 20 RVA: 0x00002150 File Offset: 0x00000350
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

		// Token: 0x06000015 RID: 21 RVA: 0x000021B0 File Offset: 0x000003B0
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

		// Token: 0x06000016 RID: 22 RVA: 0x000021F0 File Offset: 0x000003F0
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

		// Token: 0x06000017 RID: 23 RVA: 0x00002430 File Offset: 0x00000630
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

		// Token: 0x06000018 RID: 24 RVA: 0x000024E8 File Offset: 0x000006E8
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

		// Token: 0x04000002 RID: 2
		public readonly string StringId;

		// Token: 0x04000008 RID: 8
		private readonly MBList<string> _xmlPaths;

		// Token: 0x04000009 RID: 9
		private readonly Dictionary<string, string> _voiceXmlPathsAndModulePaths;
	}
}
