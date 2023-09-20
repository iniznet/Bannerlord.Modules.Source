using System;
using TaleWorlds.Localization.TextProcessor;

namespace TaleWorlds.Localization.Expressions
{
	internal class LangaugeMarkerExpression : TextExpression
	{
		public LangaugeMarkerExpression(string innerText)
		{
			base.RawValue = innerText;
		}

		internal override string EvaluateString(TextProcessingContext context, TextObject parent)
		{
			return base.RawValue;
		}

		internal override TokenType TokenType
		{
			get
			{
				return TokenType.LanguageMarker;
			}
		}
	}
}
