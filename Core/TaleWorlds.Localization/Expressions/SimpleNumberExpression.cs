using System;
using TaleWorlds.Localization.TextProcessor;

namespace TaleWorlds.Localization.Expressions
{
	internal class SimpleNumberExpression : TextExpression
	{
		public SimpleNumberExpression(string value)
		{
			base.RawValue = value;
		}

		internal override TokenType TokenType
		{
			get
			{
				return TokenType.Number;
			}
		}

		internal override string EvaluateString(TextProcessingContext context, TextObject parent)
		{
			return base.RawValue;
		}
	}
}
