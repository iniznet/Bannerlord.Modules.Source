using System;
using TaleWorlds.Localization.TextProcessor;

namespace TaleWorlds.Localization.Expressions
{
	// Token: 0x02000019 RID: 25
	internal class ComparisonExpression : NumeralExpression
	{
		// Token: 0x060000AC RID: 172 RVA: 0x000045CD File Offset: 0x000027CD
		public ComparisonExpression(ComparisonOperation op, TextExpression exp1, TextExpression exp2)
		{
			this._op = op;
			this._exp1 = exp1;
			this._exp2 = exp2;
			base.RawValue = exp1.RawValue + op + exp2.RawValue;
		}

		// Token: 0x060000AD RID: 173 RVA: 0x00004608 File Offset: 0x00002808
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

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x060000AE RID: 174 RVA: 0x00004703 File Offset: 0x00002903
		internal override TokenType TokenType
		{
			get
			{
				return TokenType.ComparisonExpression;
			}
		}

		// Token: 0x060000AF RID: 175 RVA: 0x00004707 File Offset: 0x00002907
		internal override int EvaluateNumber(TextProcessingContext context, TextObject parent)
		{
			if (!this.EvaluateBoolean(context, parent))
			{
				return 0;
			}
			return 1;
		}

		// Token: 0x060000B0 RID: 176 RVA: 0x00004718 File Offset: 0x00002918
		internal override string EvaluateString(TextProcessingContext context, TextObject parent)
		{
			return this.EvaluateNumber(context, parent).ToString();
		}

		// Token: 0x0400003C RID: 60
		private readonly ComparisonOperation _op;

		// Token: 0x0400003D RID: 61
		private readonly TextExpression _exp1;

		// Token: 0x0400003E RID: 62
		private readonly TextExpression _exp2;
	}
}
