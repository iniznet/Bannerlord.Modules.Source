﻿using System;
using System.Collections.Generic;
using TaleWorlds.Localization.TextProcessor;

namespace TaleWorlds.Localization.Expressions
{
	internal class ConditionExpression : TextExpression
	{
		public ConditionExpression(TextExpression condition, TextExpression part1, TextExpression part2)
		{
			this._conditionExpressions = new TextExpression[] { condition };
			this._resultExpressions = new TextExpression[] { part1, part2 };
		}

		public ConditionExpression(List<TextExpression> conditionExpressions, List<TextExpression> resultExpressions2)
		{
			this._conditionExpressions = conditionExpressions.ToArray();
			this._resultExpressions = resultExpressions2.ToArray();
		}

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

		internal override TokenType TokenType
		{
			get
			{
				return TokenType.ConditionalExpression;
			}
		}

		private TextExpression[] _conditionExpressions;

		private TextExpression[] _resultExpressions;
	}
}
