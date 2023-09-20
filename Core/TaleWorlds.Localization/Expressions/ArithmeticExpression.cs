using System;
using TaleWorlds.Localization.TextProcessor;

namespace TaleWorlds.Localization.Expressions
{
	// Token: 0x0200001A RID: 26
	internal class ArithmeticExpression : NumeralExpression
	{
		// Token: 0x060000B1 RID: 177 RVA: 0x00004735 File Offset: 0x00002935
		public ArithmeticExpression(ArithmeticOperation op, TextExpression exp1, TextExpression exp2)
		{
			this._op = op;
			this._exp1 = exp1;
			this._exp2 = exp2;
			base.RawValue = exp1.RawValue + op + exp2.RawValue;
		}

		// Token: 0x060000B2 RID: 178 RVA: 0x00004770 File Offset: 0x00002970
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

		// Token: 0x060000B3 RID: 179 RVA: 0x00004818 File Offset: 0x00002A18
		internal override string EvaluateString(TextProcessingContext context, TextObject parent)
		{
			return this.EvaluateNumber(context, parent).ToString();
		}

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x060000B4 RID: 180 RVA: 0x00004835 File Offset: 0x00002A35
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

		// Token: 0x0400003F RID: 63
		private readonly ArithmeticOperation _op;

		// Token: 0x04000040 RID: 64
		private readonly TextExpression _exp1;

		// Token: 0x04000041 RID: 65
		private readonly TextExpression _exp2;
	}
}
