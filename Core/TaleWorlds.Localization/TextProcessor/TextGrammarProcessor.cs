using System;
using TaleWorlds.Library;
using TaleWorlds.Localization.Expressions;

namespace TaleWorlds.Localization.TextProcessor
{
	public static class TextGrammarProcessor
	{
		public static string Process(MBTextModel dataRepresentation, TextProcessingContext textContext, TextObject parent = null)
		{
			MBStringBuilder mbstringBuilder = default(MBStringBuilder);
			mbstringBuilder.Initialize(16, "Process");
			foreach (TextExpression textExpression in dataRepresentation.RootExpressions)
			{
				if (textExpression != null)
				{
					string text = textExpression.EvaluateString(textContext, parent).ToString();
					mbstringBuilder.Append<string>(text);
				}
				else
				{
					MBTextManager.ThrowLocalizationError("Exp should not be null!");
				}
			}
			return mbstringBuilder.ToStringAndRelease();
		}
	}
}
