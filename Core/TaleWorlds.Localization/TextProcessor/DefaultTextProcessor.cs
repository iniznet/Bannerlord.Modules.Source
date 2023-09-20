using System;
using System.Globalization;
using System.Text;

namespace TaleWorlds.Localization.TextProcessor
{
	public class DefaultTextProcessor : LanguageSpecificTextProcessor
	{
		public override void ProcessToken(string sourceText, ref int cursorPos, string token, StringBuilder outputString)
		{
		}

		public override CultureInfo CultureInfoForLanguage
		{
			get
			{
				return CultureInfo.InvariantCulture;
			}
		}

		public override void ClearTemporaryData()
		{
		}
	}
}
