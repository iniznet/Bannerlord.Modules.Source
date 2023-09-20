using System;
using TaleWorlds.Localization.TextProcessor;

namespace TaleWorlds.Localization.Expressions
{
	// Token: 0x0200000D RID: 13
	internal abstract class TextExpression
	{
		// Token: 0x0600008C RID: 140
		internal abstract string EvaluateString(TextProcessingContext context, TextObject parent);

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x0600008D RID: 141
		internal abstract TokenType TokenType { get; }

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x0600008E RID: 142 RVA: 0x0000436C File Offset: 0x0000256C
		// (set) Token: 0x0600008F RID: 143 RVA: 0x00004374 File Offset: 0x00002574
		internal string RawValue { get; set; }

		// Token: 0x06000090 RID: 144 RVA: 0x00004380 File Offset: 0x00002580
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
