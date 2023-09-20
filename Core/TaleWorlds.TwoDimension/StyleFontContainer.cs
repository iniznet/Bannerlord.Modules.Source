using System;
using System.Collections.Generic;

namespace TaleWorlds.TwoDimension
{
	public class StyleFontContainer
	{
		public StyleFontContainer()
		{
			this._styleFonts = new Dictionary<string, StyleFontContainer.FontData>();
		}

		public void Add(string style, Font font, float fontSize)
		{
			StyleFontContainer.FontData fontData = new StyleFontContainer.FontData(font, fontSize);
			this._styleFonts.Add(style, fontData);
		}

		public StyleFontContainer.FontData GetFontData(string style)
		{
			StyleFontContainer.FontData fontData;
			if (this._styleFonts.TryGetValue(style, out fontData))
			{
				return fontData;
			}
			StyleFontContainer.FontData fontData2;
			this._styleFonts.TryGetValue("Default", out fontData2);
			return fontData2;
		}

		public void ClearFonts()
		{
			this._styleFonts.Clear();
		}

		private readonly Dictionary<string, StyleFontContainer.FontData> _styleFonts;

		public struct FontData
		{
			public FontData(Font font, float fontSize)
			{
				this.Font = font;
				this.FontSize = fontSize;
			}

			public Font Font;

			public float FontSize;
		}
	}
}
