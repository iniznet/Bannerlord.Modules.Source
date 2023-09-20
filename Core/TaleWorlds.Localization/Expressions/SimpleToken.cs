using System;
using TaleWorlds.Localization.TextProcessor;

namespace TaleWorlds.Localization.Expressions
{
	internal class SimpleToken : TextExpression
	{
		public SimpleToken(TokenType tokenType, string value)
		{
			base.RawValue = value;
			this._tokenType = tokenType;
		}

		internal override string EvaluateString(TextProcessingContext context, TextObject parent)
		{
			switch (this.TokenType)
			{
			case TokenType.FunctionParam:
				return context.GetFunctionParam(base.RawValue).ToStringWithoutClear();
			case TokenType.ParameterWithMarkerOccurance:
				return context.GetParameterWithMarkerOccurance(base.RawValue, parent);
			case TokenType.ParameterWithMultipleMarkerOccurances:
				return context.GetParameterWithMarkerOccurances(base.RawValue, parent);
			default:
				return base.RawValue;
			}
		}

		internal override TokenType TokenType
		{
			get
			{
				return this._tokenType;
			}
		}

		public static readonly SimpleToken SequenceTerminator = new SimpleToken(TokenType.SequenceTerminator, ".");

		private readonly TokenType _tokenType;
	}
}
