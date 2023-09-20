using System;
using TaleWorlds.Localization.TextProcessor;

namespace TaleWorlds.Localization.Expressions
{
	// Token: 0x0200001B RID: 27
	internal class SimpleExpression : TextExpression
	{
		// Token: 0x060000B5 RID: 181 RVA: 0x0000484D File Offset: 0x00002A4D
		public SimpleExpression(TextExpression innerExpression)
		{
			this._innerExpression = innerExpression;
			base.RawValue = innerExpression.RawValue;
		}

		// Token: 0x060000B6 RID: 182 RVA: 0x00004868 File Offset: 0x00002A68
		internal override string EvaluateString(TextProcessingContext context, TextObject parent)
		{
			return this._innerExpression.EvaluateString(context, parent);
		}

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x060000B7 RID: 183 RVA: 0x00004877 File Offset: 0x00002A77
		internal override TokenType TokenType
		{
			get
			{
				return TokenType.SimpleExpression;
			}
		}

		// Token: 0x04000042 RID: 66
		private TextExpression _innerExpression;
	}
}
