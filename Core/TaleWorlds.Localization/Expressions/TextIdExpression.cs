using System;
using TaleWorlds.Localization.TextProcessor;

namespace TaleWorlds.Localization.Expressions
{
	internal class TextIdExpression : TextExpression
	{
		public TextIdExpression(string innerText)
		{
			base.RawValue = innerText;
		}

		internal override string EvaluateString(TextProcessingContext context, TextObject parent)
		{
			return "";
		}

		internal override TokenType TokenType
		{
			get
			{
				return TokenType.textId;
			}
		}
	}
}
