using System;
using TaleWorlds.Localization.TextProcessor;

namespace TaleWorlds.Localization.Expressions
{
	internal abstract class TextExpression
	{
		internal abstract string EvaluateString(TextProcessingContext context, TextObject parent);

		internal abstract TokenType TokenType { get; }

		internal string RawValue { get; set; }

		internal int EvaluateAsNumber(TextExpression exp, TextProcessingContext context, TextObject parent)
		{
			NumeralExpression numeralExpression = exp as NumeralExpression;
			if (numeralExpression != null)
			{
				return numeralExpression.EvaluateNumber(context, parent);
			}
			int num;
			if (int.TryParse(exp.EvaluateString(context, parent), out num))
			{
				return num;
			}
			if (exp.RawValue == null)
			{
				return 0;
			}
			if (exp.RawValue.Length != 0)
			{
				return 1;
			}
			return 0;
		}
	}
}
