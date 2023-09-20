using System;
using TaleWorlds.Library;
using TaleWorlds.Localization.Expressions;

namespace TaleWorlds.Localization.TextProcessor
{
	// Token: 0x0200002B RID: 43
	public static class TextGrammarProcessor
	{
		// Token: 0x06000118 RID: 280 RVA: 0x00005E90 File Offset: 0x00004090
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
