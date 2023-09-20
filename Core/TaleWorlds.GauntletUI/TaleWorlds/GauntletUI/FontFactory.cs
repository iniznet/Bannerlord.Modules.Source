using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI
{
	public class FontFactory
	{
		public string DefaultLangageID { get; private set; }

		public string CurrentLangageID { get; private set; }

		public Font DefaultFont
		{
			get
			{
				return this.GetCurrentLanguage().DefaultFont;
			}
		}

		public FontFactory(ResourceDepot resourceDepot)
		{
			this._resourceDepot = resourceDepot;
			this._bitmapFonts = new Dictionary<string, Font>();
			this._fontLanguageMap = new Dictionary<string, Language>();
			this._resourceDepot.OnResourceChange += this.OnResourceChange;
		}

		private void OnResourceChange()
		{
			this.CheckForUpdates();
		}

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

		public void AddFontDefinition(string fontPath, string fontName, SpriteData spriteData)
		{
			Font font = new Font(fontName, fontPath + fontName + ".fnt", spriteData);
			this._bitmapFonts.Add(fontName, font);
		}

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

		public Font GetFont(string fontName)
		{
			if (this._bitmapFonts.ContainsKey(fontName))
			{
				return this._bitmapFonts[fontName];
			}
			return this.DefaultFont;
		}

		public IEnumerable<Font> GetFonts()
		{
			return this._bitmapFonts.Values;
		}

		public string GetFontName(Font font)
		{
			return this._bitmapFonts.FirstOrDefault((KeyValuePair<string, Font> f) => f.Value == font).Key;
		}

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

		public void OnLanguageChange(string newLanguageCode)
		{
			this.CurrentLangageID = newLanguageCode;
		}

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

		private readonly Dictionary<string, Font> _bitmapFonts;

		private readonly ResourceDepot _resourceDepot;

		private readonly Dictionary<string, Language> _fontLanguageMap;

		private SpriteData _latestSpriteData;
	}
}
