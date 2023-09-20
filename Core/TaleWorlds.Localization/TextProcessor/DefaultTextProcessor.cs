using System;
using System.Globalization;
using System.Text;

namespace TaleWorlds.Localization.TextProcessor
{
	// Token: 0x02000026 RID: 38
	public class DefaultTextProcessor : LanguageSpecificTextProcessor
	{
		// Token: 0x060000DF RID: 223 RVA: 0x00004FA7 File Offset: 0x000031A7
		public override void ProcessToken(string sourceText, ref int cursorPos, string token, StringBuilder outputString)
		{
		}

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x060000E0 RID: 224 RVA: 0x00004FA9 File Offset: 0x000031A9
		public override CultureInfo CultureInfoForLanguage
		{
			get
			{
				return CultureInfo.InvariantCulture;
			}
		}

		// Token: 0x060000E1 RID: 225 RVA: 0x00004FB0 File Offset: 0x000031B0
		public override void ClearTemporaryData()
		{
		}
	}
}
