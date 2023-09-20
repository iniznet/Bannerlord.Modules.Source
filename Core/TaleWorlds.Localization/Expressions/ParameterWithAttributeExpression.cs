using System;
using TaleWorlds.Localization.TextProcessor;

namespace TaleWorlds.Localization.Expressions
{
	internal class ParameterWithAttributeExpression : TextExpression
	{
		public ParameterWithAttributeExpression(string identifierName)
		{
			this._parameter = identifierName.Remove(identifierName.IndexOf('.'));
			this._attribute = identifierName.Substring(identifierName.IndexOf('.'));
		}

		internal override TokenType TokenType
		{
			get
			{
				return TokenType.ParameterWithAttribute;
			}
		}

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

		private readonly string _parameter;

		private readonly string _attribute;
	}
}
