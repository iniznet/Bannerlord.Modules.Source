using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI
{
	// Token: 0x0200001C RID: 28
	public class FontFactory
	{
		// Token: 0x170000BB RID: 187
		// (get) Token: 0x06000269 RID: 617 RVA: 0x0000D400 File Offset: 0x0000B600
		// (set) Token: 0x0600026A RID: 618 RVA: 0x0000D408 File Offset: 0x0000B608
		public string DefaultLangageID { get; private set; }

		// Token: 0x170000BC RID: 188
		// (get) Token: 0x0600026B RID: 619 RVA: 0x0000D411 File Offset: 0x0000B611
		// (set) Token: 0x0600026C RID: 620 RVA: 0x0000D419 File Offset: 0x0000B619
		public string CurrentLangageID { get; private set; }

		// Token: 0x170000BD RID: 189
		// (get) Token: 0x0600026D RID: 621 RVA: 0x0000D422 File Offset: 0x0000B622
		public Font DefaultFont
		{
			get
			{
				return this.GetCurrentLanguage().DefaultFont;
			}
		}

		// Token: 0x0600026E RID: 622 RVA: 0x0000D42F File Offset: 0x0000B62F
		public FontFactory(ResourceDepot resourceDepot)
		{
			this._resourceDepot = resourceDepot;
			this._bitmapFonts = new Dictionary<string, Font>();
			this._fontLanguageMap = new Dictionary<string, Language>();
			this._resourceDepot.OnResourceChange += this.OnResourceChange;
		}

		// Token: 0x0600026F RID: 623 RVA: 0x0000D46B File Offset: 0x0000B66B
		private void OnResourceChange()
		{
			this.CheckForUpdates();
		}

		// Token: 0x06000270 RID: 624 RVA: 0x0000D474 File Offset: 0x0000B674
		public void LoadAllFonts(SpriteData spriteData)
		{
			foreach (string text in this._resourceDepot.GetFiles("Fonts", ".fnt", false))
			{
				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(text);
				this.AddFontDefinition(Path.GetDirectoryName(text) + "/", fileNameWithoutExtension, spriteData);
			}
			foreach (string text2 in this._resourceDepot.GetFiles("Fonts", ".xml", false))
			{
				if (Path.GetFileNameWithoutExtension(text2).EndsWith("Languages"))
				{
					this.LoadLocalizationValues(text2);
				}
			}
			if (string.IsNullOrEmpty(this.DefaultLangageID))
			{
				this.DefaultLangageID = "English";
				this.CurrentLangageID = this.DefaultLangageID;
			}
			this._latestSpriteData = spriteData;
		}

		// Token: 0x06000271 RID: 625 RVA: 0x0000D53C File Offset: 0x0000B73C
		public void AddFontDefinition(string fontPath, string fontName, SpriteData spriteData)
		{
			Font font = new Font(fontName, fontPath + fontName + ".fnt", spriteData);
			this._bitmapFonts.Add(fontName, font);
		}

		// Token: 0x06000272 RID: 626 RVA: 0x0000D56C File Offset: 0x0000B76C
		public void LoadLocalizationValues(string sourceXMLPath)
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(sourceXMLPath);
			XmlElement xmlElement = xmlDocument["Languages"];
			XmlAttribute xmlAttribute = xmlElement.Attributes["DefaultLanguage"];
			if (!string.IsNullOrEmpty((xmlAttribute != null) ? xmlAttribute.InnerText : null))
			{
				XmlAttribute xmlAttribute2 = xmlElement.Attributes["DefaultLanguage"];
				this.DefaultLangageID = ((xmlAttribute2 != null) ? xmlAttribute2.InnerText : null) ?? "English";
				this.CurrentLangageID = this.DefaultLangageID;
			}
			foreach (object obj in xmlElement)
			{
				XmlNode xmlNode = (XmlNode)obj;
				if (xmlNode.NodeType == XmlNodeType.Element && xmlNode.Name == "Language")
				{
					Language language = Language.CreateFrom(xmlNode, this);
					Language language2;
					if (this._fontLanguageMap.TryGetValue(language.LanguageID, out language2))
					{
						this._fontLanguageMap[language.LanguageID] = language;
					}
					else
					{
						this._fontLanguageMap.Add(language.LanguageID, language);
					}
				}
			}
		}

		// Token: 0x06000273 RID: 627 RVA: 0x0000D68C File Offset: 0x0000B88C
		public Language GetCurrentLanguage()
		{
			Language language;
			if (this._fontLanguageMap.TryGetValue(this.CurrentLangageID, out language))
			{
				return language;
			}
			Debug.Print("Couldn't find language in language mapping: " + this.CurrentLangageID, 0, Debug.DebugColor.White, 17592186044416UL);
			Debug.FailedAssert("Couldn't find language in language mapping: " + this.CurrentLangageID, "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\GauntletUI\\TaleWorlds.GauntletUI\\FontFactory.cs", "GetCurrentLanguage", 122);
			return this._fontLanguageMap[this.DefaultLangageID];
		}

		// Token: 0x06000274 RID: 628 RVA: 0x0000D703 File Offset: 0x0000B903
		public Font GetFont(string fontName)
		{
			if (this._bitmapFonts.ContainsKey(fontName))
			{
				return this._bitmapFonts[fontName];
			}
			return this.DefaultFont;
		}

		// Token: 0x06000275 RID: 629 RVA: 0x0000D726 File Offset: 0x0000B926
		public IEnumerable<Font> GetFonts()
		{
			return this._bitmapFonts.Values;
		}

		// Token: 0x06000276 RID: 630 RVA: 0x0000D734 File Offset: 0x0000B934
		public string GetFontName(Font font)
		{
			return this._bitmapFonts.FirstOrDefault((KeyValuePair<string, Font> f) => f.Value == font).Key;
		}

		// Token: 0x06000277 RID: 631 RVA: 0x0000D770 File Offset: 0x0000B970
		public Font GetMappedFontForLocalization(string englishFontName)
		{
			if (string.IsNullOrEmpty(englishFontName))
			{
				return this.DefaultFont;
			}
			if (!(this.CurrentLangageID != this.DefaultLangageID))
			{
				return this.GetFont(englishFontName);
			}
			Language currentLanguage = this.GetCurrentLanguage();
			if (currentLanguage.FontMapHasKey(englishFontName))
			{
				return currentLanguage.GetMappedFont(englishFontName);
			}
			return this.DefaultFont;
		}

		// Token: 0x06000278 RID: 632 RVA: 0x0000D7C5 File Offset: 0x0000B9C5
		public void OnLanguageChange(string newLanguageCode)
		{
			this.CurrentLangageID = newLanguageCode;
		}

		// Token: 0x06000279 RID: 633 RVA: 0x0000D7D0 File Offset: 0x0000B9D0
		public Font GetUsableFontForCharacter(int characterCode)
		{
			for (int i = 0; i < this._fontLanguageMap.Values.Count; i++)
			{
				if (this._fontLanguageMap.ElementAt(i).Value.DefaultFont.Characters.ContainsKey(characterCode))
				{
					return this._fontLanguageMap.ElementAt(i).Value.DefaultFont;
				}
			}
			return null;
		}

		// Token: 0x0600027A RID: 634 RVA: 0x0000D83C File Offset: 0x0000BA3C
		public void CheckForUpdates()
		{
			string currentLangageID = this.CurrentLangageID;
			this.CurrentLangageID = null;
			this.DefaultLangageID = null;
			this._bitmapFonts.Clear();
			this._fontLanguageMap.Clear();
			this.LoadAllFonts(this._latestSpriteData);
			this.CurrentLangageID = currentLangageID;
		}

		// Token: 0x04000149 RID: 329
		private readonly Dictionary<string, Font> _bitmapFonts;

		// Token: 0x0400014A RID: 330
		private readonly ResourceDepot _resourceDepot;

		// Token: 0x0400014B RID: 331
		private readonly Dictionary<string, Language> _fontLanguageMap;

		// Token: 0x0400014C RID: 332
		private SpriteData _latestSpriteData;
	}
}
