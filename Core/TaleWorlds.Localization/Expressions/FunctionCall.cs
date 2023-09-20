using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Localization.TextProcessor;

namespace TaleWorlds.Localization.Expressions
{
	// Token: 0x02000022 RID: 34
	internal class FunctionCall : TextExpression
	{
		// Token: 0x060000D2 RID: 210 RVA: 0x00004DE8 File Offset: 0x00002FE8
		public FunctionCall(string functionName, IEnumerable<TextExpression> functionParams)
		{
			this._functionName = functionName;
			this._functionParams = functionParams.ToList<TextExpression>();
			base.RawValue = this._functionName;
		}

		// Token: 0x060000D3 RID: 211 RVA: 0x00004E0F File Offset: 0x0000300F
		internal override string EvaluateString(TextProcessingContext context, TextObject parent)
		{
			return context.CallFunction(this._functionName, this._functionParams, parent).ToStringWithoutClear();
		}

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x060000D4 RID: 212 RVA: 0x00004E29 File Offset: 0x00003029
		internal override TokenType TokenType
		{
			get
			{
				return TokenType.FunctionCall;
			}
		}

		// Token: 0x0400004E RID: 78
		private string _functionName;

		// Token: 0x0400004F RID: 79
		private List<TextExpression> _functionParams;
	}
}
