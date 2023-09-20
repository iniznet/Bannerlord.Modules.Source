using System;
using TaleWorlds.Localization.TextProcessor;

namespace TaleWorlds.Localization.Expressions
{
	// Token: 0x02000018 RID: 24
	internal class ParanthesisExpression : TextExpression
	{
		// Token: 0x060000A9 RID: 169 RVA: 0x00004590 File Offset: 0x00002790
		public ParanthesisExpression(TextExpression innerExpression)
		{
			this._innerExp = innerExpression;
			base.RawValue = "(" + innerExpression.RawValue + ")";
		}

		// Token: 0x060000AA RID: 170 RVA: 0x000045BA File Offset: 0x000027BA
		internal override string EvaluateString(TextProcessingContext context, TextObject parent)
		{
			return this._innerExp.EvaluateString(context, parent);
		}

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x060000AB RID: 171 RVA: 0x000045C9 File Offset: 0x000027C9
		internal override TokenType TokenType
		{
			get
			{
				return TokenType.ParenthesisExpression;
			}
		}

		// Token: 0x0400003B RID: 59
		private readonly TextExpression _innerExp;
	}
}
