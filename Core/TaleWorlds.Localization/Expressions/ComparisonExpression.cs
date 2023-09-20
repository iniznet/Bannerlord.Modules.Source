using System;
using TaleWorlds.Localization.TextProcessor;

namespace TaleWorlds.Localization.Expressions
{
	internal class ComparisonExpression : NumeralExpression
	{
		public ComparisonExpression(ComparisonOperation op, TextExpression exp1, TextExpression exp2)
		{
			this._op = op;
			this._exp1 = exp1;
			this._exp2 = exp2;
			base.RawValue = exp1.RawValue + op + exp2.RawValue;
		}

		internal bool EvaluateBoolean(TextProcessingContext context, TextObject parent)
		{
			switch (this._op)
			{
			case ComparisonOperation.Equals:
				return base.EvaluateAsNumber(this._exp1, context, parent) == base.EvaluateAsNumber(this._exp2, context, parent);
			case ComparisonOperation.NotEquals:
				return base.EvaluateAsNumber(this._exp1, context, parent) != base.EvaluateAsNumber(this._exp2, context, parent);
			case ComparisonOperation.GreaterThan:
				return base.EvaluateAsNumber(this._exp1, context, parent) > base.EvaluateAsNumber(this._exp2, context, parent);
			case ComparisonOperation.GreaterOrEqual:
				return base.EvaluateAsNumber(this._exp1, context, parent) >= base.EvaluateAsNumber(this._exp2, context, parent);
			case ComparisonOperation.LessThan:
				return base.EvaluateAsNumber(this._exp1, context, parent) < base.EvaluateAsNumber(this._exp2, context, parent);
			case ComparisonOperation.LessOrEqual:
				return base.EvaluateAsNumber(this._exp1, context, parent) <= base.EvaluateAsNumber(this._exp2, context, parent);
			default:
				return false;
			}
		}

		internal override TokenType TokenType
		{
			get
			{
				return TokenType.ComparisonExpression;
			}
		}

		internal override int EvaluateNumber(TextProcessingContext context, TextObject parent)
		{
			if (!this.EvaluateBoolean(context, parent))
			{
				return 0;
			}
			return 1;
		}

		internal override string EvaluateString(TextProcessingContext context, TextObject parent)
		{
			return this.EvaluateNumber(context, parent).ToString();
		}

		private readonly ComparisonOperation _op;

		private readonly TextExpression _exp1;

		private readonly TextExpression _exp2;
	}
}
