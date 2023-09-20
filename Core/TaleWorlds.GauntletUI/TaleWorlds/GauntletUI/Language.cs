using System;
using System.Collections.Generic;
using System.Xml;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI
{
	public class Language : ILanguage
	{
		public char[] ForbiddenStartOfLineCharacters { get; private set; }

		public char[] ForbiddenEndOfLineCharacters { get; private set; }

		public string LanguageID { get; private set; }

		public string DefaultFontName { get; private set; }

		public bool DoesFontRequireSpaceForNewline { get; private set; } = true;

		public Font DefaultFont { get; private set; }

		public char LineSeperatorChar { get; private set; }

		public bool FontMapHasKey(string keyFontName)
		{
			return this._fontMap.ContainsKey(keyFontName);
		}

		public Font GetMappedFont(string keyFontName)
		{
			return this._fontMap[keyFontName];
		}

		private Language()
		{
		}

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

		IEnumerable<char> ILanguage.GetForbiddenStartOfLineCharacters()
		{
			return this.ForbiddenStartOfLineCharacters;
		}

		IEnumerable<char> ILanguage.GetForbiddenEndOfLineCharacters()
		{
			return this.ForbiddenEndOfLineCharacters;
		}

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

		string ILanguage.GetLanguageID()
		{
			return this.LanguageID;
		}

		string ILanguage.GetDefaultFontName()
		{
			return this.DefaultFontName;
		}

		Font ILanguage.GetDefaultFont()
		{
			return this.DefaultFont;
		}

		char ILanguage.GetLineSeperatorChar()
		{
			return this.LineSeperatorChar;
		}

		bool ILanguage.DoesLanguageRequireSpaceForNewline()
		{
			return this.DoesFontRequireSpaceForNewline;
		}

		bool ILanguage.FontMapHasKey(string keyFontName)
		{
			return this._fontMap.ContainsKey(keyFontName);
		}

		Font ILanguage.GetMappedFont(string keyFontName)
		{
			return this._fontMap[keyFontName];
		}

		private readonly Dictionary<string, Font> _fontMap = new Dictionary<string, Font>();
	}
}
