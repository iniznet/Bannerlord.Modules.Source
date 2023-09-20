using System;
using TaleWorlds.Localization.TextProcessor;

namespace TaleWorlds.Localization.Expressions
{
	// Token: 0x02000012 RID: 18
	internal class SimpleToken : TextExpression
	{
		// Token: 0x0600009E RID: 158 RVA: 0x00004440 File Offset: 0x00002640
		public SimpleToken(TokenType tokenType, string value)
		{
			base.RawValue = value;
			this._tokenType = tokenType;
		}

		// Token: 0x0600009F RID: 159 RVA: 0x00004458 File Offset: 0x00002658
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

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x060000A0 RID: 160 RVA: 0x000044B7 File Offset: 0x000026B7
		internal override TokenType TokenType
		{
			get
			{
				return this._tokenType;
			}
		}

		// Token: 0x04000028 RID: 40
		public static readonly SimpleToken SequenceTerminator = new SimpleToken(TokenType.SequenceTerminator, ".");

		// Token: 0x04000029 RID: 41
		private readonly TokenType _tokenType;
	}
}
