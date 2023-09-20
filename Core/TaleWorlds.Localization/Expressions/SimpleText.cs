using System;
using TaleWorlds.Localization.TextProcessor;

namespace TaleWorlds.Localization.Expressions
{
	internal class SimpleText : TextExpression
	{
		public SimpleText(string value)
		{
			base.RawValue = value;
		}

		internal override TokenType TokenType
		{
			get
			{
				return TokenType.Text;
			}
		}

		internal override string EvaluateString(TextProcessingContext context, TextObject parent)
		{
			return base.RawValue;
		}
	}
}
