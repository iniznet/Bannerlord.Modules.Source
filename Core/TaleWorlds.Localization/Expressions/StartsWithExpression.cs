using System;
using TaleWorlds.Localization.TextProcessor;

namespace TaleWorlds.Localization.Expressions
{
	internal class StartsWithExpression : TextExpression
	{
		public StartsWithExpression(string identifierName)
		{
			int num = identifierName.IndexOf('(');
			int num2 = identifierName.IndexOf(')');
			this._parameter = identifierName.Remove(num);
			this._functionParams = identifierName.Substring(num + 1, num2 - num - 1).Split(new char[] { ',' });
		}

		internal override TokenType TokenType
		{
			get
			{
				return TokenType.StartsWith;
			}
		}

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

		private readonly string _parameter;

		private readonly string[] _functionParams;
	}
}
