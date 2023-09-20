using System;
using TaleWorlds.Localization.TextProcessor;

namespace TaleWorlds.Localization.Expressions
{
	// Token: 0x02000025 RID: 37
	internal class StartsWithExpression : TextExpression
	{
		// Token: 0x060000DC RID: 220 RVA: 0x00004EE4 File Offset: 0x000030E4
		public StartsWithExpression(string identifierName)
		{
			int num = identifierName.IndexOf('(');
			int num2 = identifierName.IndexOf(')');
			this._parameter = identifierName.Remove(num);
			this._functionParams = identifierName.Substring(num + 1, num2 - num - 1).Split(new char[] { ',' });
		}

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x060000DD RID: 221 RVA: 0x00004F3A File Offset: 0x0000313A
		internal override TokenType TokenType
		{
			get
			{
				return TokenType.StartsWith;
			}
		}

		// Token: 0x060000DE RID: 222 RVA: 0x00004F40 File Offset: 0x00003140
		internal override string EvaluateString(TextProcessingContext context, TextObject parent)
		{
			TextObject functionParamWithoutEvaluate = context.GetFunctionParamWithoutEvaluate(this._parameter);
			ValueTuple<TextObject, bool> qualifiedVariableValue = context.GetQualifiedVariableValue(functionParamWithoutEvaluate.ToStringWithoutClear(), parent);
			TextObject item = qualifiedVariableValue.Item1;
			if (qualifiedVariableValue.Item2)
			{
				foreach (string text in this._functionParams)
				{
					if (item.ToStringWithoutClear().StartsWith(text, StringComparison.InvariantCultureIgnoreCase))
					{
						return text;
					}
				}
			}
			return "";
		}

		// Token: 0x04000053 RID: 83
		private readonly string _parameter;

		// Token: 0x04000054 RID: 84
		private readonly string[] _functionParams;
	}
}
