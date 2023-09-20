using System;
using TaleWorlds.Localization.TextProcessor;

namespace TaleWorlds.Localization.Expressions
{
	// Token: 0x02000014 RID: 20
	internal abstract class NumeralExpression : TextExpression
	{
		// Token: 0x060000A7 RID: 167
		internal abstract int EvaluateNumber(TextProcessingContext context, TextObject parent);
	}
}
