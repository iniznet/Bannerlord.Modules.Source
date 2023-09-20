using System;
using System.Collections.Generic;

namespace TaleWorlds.TwoDimension
{
	// Token: 0x02000020 RID: 32
	public interface ILanguage
	{
		// Token: 0x06000127 RID: 295
		IEnumerable<char> GetForbiddenStartOfLineCharacters();

		// Token: 0x06000128 RID: 296
		bool IsCharacterForbiddenAtStartOfLine(char character);

		// Token: 0x06000129 RID: 297
		IEnumerable<char> GetForbiddenEndOfLineCharacters();

		// Token: 0x0600012A RID: 298
		bool IsCharacterForbiddenAtEndOfLine(char character);

		// Token: 0x0600012B RID: 299
		string GetLanguageID();

		// Token: 0x0600012C RID: 300
		string GetDefaultFontName();

		// Token: 0x0600012D RID: 301
		Font GetDefaultFont();

		// Token: 0x0600012E RID: 302
		char GetLineSeperatorChar();

		// Token: 0x0600012F RID: 303
		bool DoesLanguageRequireSpaceForNewline();

		// Token: 0x06000130 RID: 304
		bool FontMapHasKey(string keyFontName);

		// Token: 0x06000131 RID: 305
		Font GetMappedFont(string keyFontName);
	}
}
