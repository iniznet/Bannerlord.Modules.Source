using System;
using TaleWorlds.Localization.TextProcessor;

namespace TaleWorlds.Localization.Expressions
{
	internal class ParanthesisExpression : TextExpression
	{
		public ParanthesisExpression(TextExpression innerExpression)
		{
			this._innerExp = innerExpression;
			base.RawValue = "(" + innerExpression.RawValue + ")";
		}

		internal override string EvaluateString(TextProcessingContext context, TextObject parent)
		{
			return this._innerExp.EvaluateString(context, parent);
		}

		internal override TokenType TokenType
		{
			get
			{
				return TokenType.ParenthesisExpression;
			}
		}

		private readonly TextExpression _innerExp;
	}
}
