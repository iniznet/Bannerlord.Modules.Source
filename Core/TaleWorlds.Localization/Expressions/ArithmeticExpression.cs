using System;
using TaleWorlds.Localization.TextProcessor;

namespace TaleWorlds.Localization.Expressions
{
	internal class ArithmeticExpression : NumeralExpression
	{
		public ArithmeticExpression(ArithmeticOperation op, TextExpression exp1, TextExpression exp2)
		{
			this._op = op;
			this._exp1 = exp1;
			this._exp2 = exp2;
			base.RawValue = exp1.RawValue + op + exp2.RawValue;
		}

		internal override int EvaluateNumber(TextProcessingContext context, TextObject parent)
		{
			switch (this._op)
			{
			case ArithmeticOperation.Add:
				return base.EvaluateAsNumber(this._exp1, context, parent) + base.EvaluateAsNumber(this._exp2, context, parent);
			case ArithmeticOperation.Subtract:
				return base.EvaluateAsNumber(this._exp1, context, parent) - base.EvaluateAsNumber(this._exp2, context, parent);
			case ArithmeticOperation.Multiply:
				return base.EvaluateAsNumber(this._exp1, context, parent) * base.EvaluateAsNumber(this._exp2, context, parent);
			case ArithmeticOperation.Divide:
				return base.EvaluateAsNumber(this._exp1, context, parent) / base.EvaluateAsNumber(this._exp2, context, parent);
			default:
				return 0;
			}
		}

		internal override string EvaluateString(TextProcessingContext context, TextObject parent)
		{
			return this.EvaluateNumber(context, parent).ToString();
		}

		internal override TokenType TokenType
		{
			get
			{
				if (this._op != ArithmeticOperation.Add && this._op != ArithmeticOperation.Subtract)
				{
					return TokenType.ArithmeticProduct;
				}
				return TokenType.ArithmeticSum;
			}
		}

		private readonly ArithmeticOperation _op;

		private readonly TextExpression _exp1;

		private readonly TextExpression _exp2;
	}
}
