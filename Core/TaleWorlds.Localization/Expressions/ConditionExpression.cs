using System;
using System.Collections.Generic;
using TaleWorlds.Localization.TextProcessor;

namespace TaleWorlds.Localization.Expressions
{
	// Token: 0x0200001D RID: 29
	internal class ConditionExpression : TextExpression
	{
		// Token: 0x060000BE RID: 190 RVA: 0x000048C1 File Offset: 0x00002AC1
		public ConditionExpression(TextExpression condition, TextExpression part1, TextExpression part2)
		{
			this._conditionExpressions = new TextExpression[] { condition };
			this._resultExpressions = new TextExpression[] { part1, part2 };
		}

		// Token: 0x060000BF RID: 191 RVA: 0x000048ED File Offset: 0x00002AED
		public ConditionExpression(List<TextExpression> conditionExpressions, List<TextExpression> resultExpressions2)
		{
			this._conditionExpressions = conditionExpressions.ToArray();
			this._resultExpressions = resultExpressions2.ToArray();
		}

		// Token: 0x060000C0 RID: 192 RVA: 0x00004910 File Offset: 0x00002B10
		internal override string EvaluateString(TextProcessingContext context, TextObject parent)
		{
			bool flag = false;
			int num = 0;
			TextExpression textExpression = null;
			while (!flag && num < this._conditionExpressions.Length)
			{
				TextExpression textExpression2 = this._conditionExpressions[num];
				string text = textExpression2.EvaluateString(context, parent);
				if (text.Length != 0)
				{
					if (textExpression2.TokenType == TokenType.ParameterWithAttribute || textExpression2.TokenType == TokenType.StartsWith)
					{
						flag = !string.IsNullOrEmpty(text);
					}
					else
					{
						flag = base.EvaluateAsNumber(textExpression2, context, parent) != 0;
					}
				}
				if (flag)
				{
					if (num < this._resultExpressions.Length)
					{
						textExpression = this._resultExpressions[num];
					}
				}
				else
				{
					num++;
				}
			}
			if (textExpression == null && num < this._resultExpressions.Length)
			{
				textExpression = this._resultExpressions[num];
			}
			return ((textExpression != null) ? textExpression.EvaluateString(context, parent) : null) ?? "";
		}

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x060000C1 RID: 193 RVA: 0x000049C8 File Offset: 0x00002BC8
		internal override TokenType TokenType
		{
			get
			{
				return TokenType.ConditionalExpression;
			}
		}

		// Token: 0x04000045 RID: 69
		private TextExpression[] _conditionExpressions;

		// Token: 0x04000046 RID: 70
		private TextExpression[] _resultExpressions;
	}
}
