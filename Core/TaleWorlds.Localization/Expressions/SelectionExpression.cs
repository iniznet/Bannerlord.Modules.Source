using System;
using System.Collections.Generic;
using TaleWorlds.Localization.TextProcessor;

namespace TaleWorlds.Localization.Expressions
{
	// Token: 0x0200001E RID: 30
	internal class SelectionExpression : TextExpression
	{
		// Token: 0x060000C2 RID: 194 RVA: 0x000049CC File Offset: 0x00002BCC
		public SelectionExpression(TextExpression selection, List<TextExpression> selectionExpressions)
		{
			this._selection = selection;
			this._selectionExpressions = selectionExpressions;
		}

		// Token: 0x060000C3 RID: 195 RVA: 0x000049E4 File Offset: 0x00002BE4
		internal override string EvaluateString(TextProcessingContext context, TextObject parent)
		{
			int num = base.EvaluateAsNumber(this._selection, context, parent);
			if (num >= 0 && num < this._selectionExpressions.Count)
			{
				return this._selectionExpressions[num].EvaluateString(context, parent);
			}
			return "";
		}

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x060000C4 RID: 196 RVA: 0x00004A2B File Offset: 0x00002C2B
		internal override TokenType TokenType
		{
			get
			{
				return TokenType.SelectionExpression;
			}
		}

		// Token: 0x04000047 RID: 71
		private TextExpression _selection;

		// Token: 0x04000048 RID: 72
		private List<TextExpression> _selectionExpressions;
	}
}
