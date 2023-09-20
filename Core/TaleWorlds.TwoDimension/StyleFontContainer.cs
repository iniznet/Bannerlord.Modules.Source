using System;
using System.Collections.Generic;

namespace TaleWorlds.TwoDimension
{
	// Token: 0x02000011 RID: 17
	public class StyleFontContainer
	{
		// Token: 0x06000088 RID: 136 RVA: 0x00004AD1 File Offset: 0x00002CD1
		public StyleFontContainer()
		{
			this._styleFonts = new Dictionary<string, StyleFontContainer.FontData>();
		}

		// Token: 0x06000089 RID: 137 RVA: 0x00004AE4 File Offset: 0x00002CE4
		public void Add(string style, Font font, float fontSize)
		{
			StyleFontContainer.FontData fontData = new StyleFontContainer.FontData(font, fontSize);
			this._styleFonts.Add(style, fontData);
		}

		// Token: 0x0600008A RID: 138 RVA: 0x00004B08 File Offset: 0x00002D08
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

		// Token: 0x0600008B RID: 139 RVA: 0x00004B3B File Offset: 0x00002D3B
		public void ClearFonts()
		{
			this._styleFonts.Clear();
		}

		// Token: 0x04000059 RID: 89
		private readonly Dictionary<string, StyleFontContainer.FontData> _styleFonts;

		// Token: 0x0200003D RID: 61
		public struct FontData
		{
			// Token: 0x06000295 RID: 661 RVA: 0x0000A0E1 File Offset: 0x000082E1
			public FontData(Font font, float fontSize)
			{
				this.Font = font;
				this.FontSize = fontSize;
			}

			// Token: 0x04000152 RID: 338
			public Font Font;

			// Token: 0x04000153 RID: 339
			public float FontSize;
		}
	}
}
