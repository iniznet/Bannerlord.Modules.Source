using System;
using TaleWorlds.Localization.TextProcessor;

namespace TaleWorlds.Localization.Expressions
{
	internal class FieldExpression : TextExpression
	{
		public string FieldName
		{
			get
			{
				return base.RawValue;
			}
		}

		public TextExpression InnerExpression
		{
			get
			{
				return this.part2;
			}
		}

		public FieldExpression(TextExpression innerExpression)
		{
			this._innerExpression = innerExpression;
			base.RawValue = innerExpression.RawValue;
		}

		public FieldExpression(TextExpression innerExpression, TextExpression part2)
			: this(innerExpression)
		{
			this.part2 = part2;
		}

		internal override string EvaluateString(TextProcessingContext context, TextObject parent)
		{
			return "";
		}

		internal override TokenType TokenType
		{
			get
			{
				return TokenType.FieldExpression;
			}
		}

		private TextExpression _innerExpression;

		private TextExpression part2;
	}
}
