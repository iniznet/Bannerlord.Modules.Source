using System;
using System.Collections.Generic;
using System.Xml;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI
{
	// Token: 0x0200001D RID: 29
	public class Language : ILanguage
	{
		// Token: 0x170000BE RID: 190
		// (get) Token: 0x0600027B RID: 635 RVA: 0x0000D887 File Offset: 0x0000BA87
		// (set) Token: 0x0600027C RID: 636 RVA: 0x0000D88F File Offset: 0x0000BA8F
		public char[] ForbiddenStartOfLineCharacters { get; private set; }

		// Token: 0x170000BF RID: 191
		// (get) Token: 0x0600027D RID: 637 RVA: 0x0000D898 File Offset: 0x0000BA98
		// (set) Token: 0x0600027E RID: 638 RVA: 0x0000D8A0 File Offset: 0x0000BAA0
		public char[] ForbiddenEndOfLineCharacters { get; private set; }

		// Token: 0x170000C0 RID: 192
		// (get) Token: 0x0600027F RID: 639 RVA: 0x0000D8A9 File Offset: 0x0000BAA9
		// (set) Token: 0x06000280 RID: 640 RVA: 0x0000D8B1 File Offset: 0x0000BAB1
		public string LanguageID { get; private set; }

		// Token: 0x170000C1 RID: 193
		// (get) Token: 0x06000281 RID: 641 RVA: 0x0000D8BA File Offset: 0x0000BABA
		// (set) Token: 0x06000282 RID: 642 RVA: 0x0000D8C2 File Offset: 0x0000BAC2
		public string DefaultFontName { get; private set; }

		// Token: 0x170000C2 RID: 194
		// (get) Token: 0x06000283 RID: 643 RVA: 0x0000D8CB File Offset: 0x0000BACB
		// (set) Token: 0x06000284 RID: 644 RVA: 0x0000D8D3 File Offset: 0x0000BAD3
		public bool DoesFontRequireSpaceForNewline { get; private set; } = true;

		// Token: 0x170000C3 RID: 195
		// (get) Token: 0x06000285 RID: 645 RVA: 0x0000D8DC File Offset: 0x0000BADC
		// (set) Token: 0x06000286 RID: 646 RVA: 0x0000D8E4 File Offset: 0x0000BAE4
		public Font DefaultFont { get; private set; }

		// Token: 0x170000C4 RID: 196
		// (get) Token: 0x06000287 RID: 647 RVA: 0x0000D8ED File Offset: 0x0000BAED
		// (set) Token: 0x06000288 RID: 648 RVA: 0x0000D8F5 File Offset: 0x0000BAF5
		public char LineSeperatorChar { get; private set; }

		// Token: 0x06000289 RID: 649 RVA: 0x0000D8FE File Offset: 0x0000BAFE
		public bool FontMapHasKey(string keyFontName)
		{
			return this._fontMap.ContainsKey(keyFontName);
		}

		// Token: 0x0600028A RID: 650 RVA: 0x0000D90C File Offset: 0x0000BB0C
		public Font GetMappedFont(string keyFontName)
		{
			return this._fontMap[keyFontName];
		}

		// Token: 0x0600028B RID: 651 RVA: 0x0000D91A File Offset: 0x0000BB1A
		private Language()
		{
		}

		// Token: 0x0600028C RID: 652 RVA: 0x0000D934 File Offset: 0x0000BB34
		public static Language CreateFrom(XmlNode languageNode, FontFactory fontFactory)
		{
			Language language = new Language
			{
				LanguageID = languageNode.Attributes["id"].InnerText
			};
			Language language2 = language;
			XmlAttribute xmlAttribute = languageNode.Attributes["DefaultFont"];
			language2.DefaultFontName = ((xmlAttribute != null) ? xmlAttribute.InnerText : null) ?? "Galahad";
			Language language3 = language;
			XmlAttribute xmlAttribute2 = languageNode.Attributes["LineSeperatorChar"];
			language3.LineSeperatorChar = ((xmlAttribute2 != null) ? xmlAttribute2.InnerText[0] : '-');
			language.DefaultFont = fontFactory.GetFont(language.DefaultFontName);
			foreach (object obj in languageNode.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				if (xmlNode.NodeType == XmlNodeType.Element)
				{
					if (xmlNode.Name == "Map")
					{
						string innerText = xmlNode.Attributes["From"].InnerText;
						string innerText2 = xmlNode.Attributes["To"].InnerText;
						language._fontMap.Add(innerText, fontFactory.GetFont(innerText2));
					}
					else if (xmlNode.Name == "NewlineDoesntRequireSpace")
					{
						language.DoesFontRequireSpaceForNewline = false;
					}
					else if (xmlNode.Name == "ForbiddenStartOfLineCharacters")
					{
						Language language4 = language;
						XmlAttribute xmlAttribute3 = xmlNode.Attributes["Characters"];
						language4.ForbiddenStartOfLineCharacters = ((xmlAttribute3 != null) ? xmlAttribute3.InnerText.ToCharArray() : null);
					}
					else if (xmlNode.Name == "ForbiddenEndOfLineCharacters")
					{
						Language language5 = language;
						XmlAttribute xmlAttribute4 = xmlNode.Attributes["Characters"];
						language5.ForbiddenEndOfLineCharacters = ((xmlAttribute4 != null) ? xmlAttribute4.InnerText.ToCharArray() : null);
					}
				}
			}
			return language;
		}

		// Token: 0x0600028D RID: 653 RVA: 0x0000DB18 File Offset: 0x0000BD18
		IEnumerable<char> ILanguage.GetForbiddenStartOfLineCharacters()
		{
			return this.ForbiddenStartOfLineCharacters;
		}

		// Token: 0x0600028E RID: 654 RVA: 0x0000DB20 File Offset: 0x0000BD20
		IEnumerable<char> ILanguage.GetForbiddenEndOfLineCharacters()
		{
			return this.ForbiddenEndOfLineCharacters;
		}

		// Token: 0x0600028F RID: 655 RVA: 0x0000DB28 File Offset: 0x0000BD28
		bool ILanguage.IsCharacterForbiddenAtStartOfLine(char character)
		{
			if (this.ForbiddenStartOfLineCharacters == null || this.ForbiddenStartOfLineCharacters.Length == 0)
			{
				return false;
			}
			for (int i = 0; i < this.ForbiddenStartOfLineCharacters.Length; i++)
			{
				if (this.ForbiddenStartOfLineCharacters[i] == character)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000290 RID: 656 RVA: 0x0000DB6C File Offset: 0x0000BD6C
		bool ILanguage.IsCharacterForbiddenAtEndOfLine(char character)
		{
			if (this.ForbiddenEndOfLineCharacters == null || this.ForbiddenEndOfLineCharacters.Length == 0)
			{
				return false;
			}
			for (int i = 0; i < this.ForbiddenEndOfLineCharacters.Length; i++)
			{
				if (this.ForbiddenEndOfLineCharacters[i] == character)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000291 RID: 657 RVA: 0x0000DBAD File Offset: 0x0000BDAD
		string ILanguage.GetLanguageID()
		{
			return this.LanguageID;
		}

		// Token: 0x06000292 RID: 658 RVA: 0x0000DBB5 File Offset: 0x0000BDB5
		string ILanguage.GetDefaultFontName()
		{
			return this.DefaultFontName;
		}

		// Token: 0x06000293 RID: 659 RVA: 0x0000DBBD File Offset: 0x0000BDBD
		Font ILanguage.GetDefaultFont()
		{
			return this.DefaultFont;
		}

		// Token: 0x06000294 RID: 660 RVA: 0x0000DBC5 File Offset: 0x0000BDC5
		char ILanguage.GetLineSeperatorChar()
		{
			return this.LineSeperatorChar;
		}

		// Token: 0x06000295 RID: 661 RVA: 0x0000DBCD File Offset: 0x0000BDCD
		bool ILanguage.DoesLanguageRequireSpaceForNewline()
		{
			return this.DoesFontRequireSpaceForNewline;
		}

		// Token: 0x06000296 RID: 662 RVA: 0x0000DBD5 File Offset: 0x0000BDD5
		bool ILanguage.FontMapHasKey(string keyFontName)
		{
			return this._fontMap.ContainsKey(keyFontName);
		}

		// Token: 0x06000297 RID: 663 RVA: 0x0000DBE3 File Offset: 0x0000BDE3
		Font ILanguage.GetMappedFont(string keyFontName)
		{
			return this._fontMap[keyFontName];
		}

		// Token: 0x04000154 RID: 340
		private readonly Dictionary<string, Font> _fontMap = new Dictionary<string, Font>();
	}
}
