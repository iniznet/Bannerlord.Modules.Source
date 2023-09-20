using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Localization.TextProcessor;

namespace TaleWorlds.Localization.Expressions
{
	internal class FunctionCall : TextExpression
	{
		public FunctionCall(string functionName, IEnumerable<TextExpression> functionParams)
		{
			this._functionName = functionName;
			this._functionParams = functionParams.ToList<TextExpression>();
			base.RawValue = this._functionName;
		}

		internal override string EvaluateString(TextProcessingContext context, TextObject parent)
		{
			return context.CallFunction(this._functionName, this._functionParams, parent).ToStringWithoutClear();
		}

		internal override TokenType TokenType
		{
			get
			{
				return TokenType.FunctionCall;
			}
		}

		private string _functionName;

		private List<TextExpression> _functionParams;
	}
}
