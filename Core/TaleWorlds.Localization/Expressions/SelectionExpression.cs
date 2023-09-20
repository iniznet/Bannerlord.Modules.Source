using System;
using System.Collections.Generic;
using TaleWorlds.Localization.TextProcessor;

namespace TaleWorlds.Localization.Expressions
{
	internal class SelectionExpression : TextExpression
	{
		public SelectionExpression(TextExpression selection, List<TextExpression> selectionExpressions)
		{
			this._selection = selection;
			this._selectionExpressions = selectionExpressions;
		}

		internal override string EvaluateString(TextProcessingContext context, TextObject parent)
		{
			int num = base.EvaluateAsNumber(this._selection, context, parent);
			if (num >= 0 && num < this._selectionExpressions.Count)
			{
				return this._selectionExpressions[num].EvaluateString(context, parent);
			}
			return "";
		}

		internal override TokenType TokenType
		{
			get
			{
				return TokenType.SelectionExpression;
			}
		}

		private TextExpression _selection;

		private List<TextExpression> _selectionExpressions;
	}
}
