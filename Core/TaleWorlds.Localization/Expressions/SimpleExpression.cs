using System;
using TaleWorlds.Localization.TextProcessor;

namespace TaleWorlds.Localization.Expressions
{
	internal class SimpleExpression : TextExpression
	{
		public SimpleExpression(TextExpression innerExpression)
		{
			this._innerExpression = innerExpression;
			base.RawValue = innerExpression.RawValue;
		}

		internal override string EvaluateString(TextProcessingContext context, TextObject parent)
		{
			return this._innerExpression.EvaluateString(context, parent);
		}

		internal override TokenType TokenType
		{
			get
			{
				return TokenType.SimpleExpression;
			}
		}

		private TextExpression _innerExpression;
	}
}
