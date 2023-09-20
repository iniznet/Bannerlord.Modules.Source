using System;
using TaleWorlds.Localization.TextProcessor;

namespace TaleWorlds.Localization.Expressions
{
	// Token: 0x02000024 RID: 36
	internal class ParameterWithAttributeExpression : TextExpression
	{
		// Token: 0x060000D9 RID: 217 RVA: 0x00004E61 File Offset: 0x00003061
		public ParameterWithAttributeExpression(string identifierName)
		{
			this._parameter = identifierName.Remove(identifierName.IndexOf('.'));
			this._attribute = identifierName.Substring(identifierName.IndexOf('.'));
		}

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x060000DA RID: 218 RVA: 0x00004E91 File Offset: 0x00003091
		internal override TokenType TokenType
		{
			get
			{
				return TokenType.ParameterWithAttribute;
			}
		}

		// Token: 0x060000DB RID: 219 RVA: 0x00004E98 File Offset: 0x00003098
		internal override string EvaluateString(TextProcessingContext context, TextObject parent)
		{
			TextObject functionParamWithoutEvaluate = context.GetFunctionParamWithoutEvaluate(this._parameter);
			ValueTuple<TextObject, bool> qualifiedVariableValue = context.GetQualifiedVariableValue(functionParamWithoutEvaluate.ToStringWithoutClear() + this._attribute, parent);
			TextObject item = qualifiedVariableValue.Item1;
			if (qualifiedVariableValue.Item2)
			{
				return item.ToStringWithoutClear();
			}
			return "";
		}

		// Token: 0x04000051 RID: 81
		private readonly string _parameter;

		// Token: 0x04000052 RID: 82
		private readonly string _attribute;
	}
}
