using System;
using TaleWorlds.Localization.TextProcessor;

namespace TaleWorlds.Localization.Expressions
{
	// Token: 0x0200000F RID: 15
	internal class SimpleNumberExpression : TextExpression
	{
		// Token: 0x06000095 RID: 149 RVA: 0x000043F0 File Offset: 0x000025F0
		public SimpleNumberExpression(string value)
		{
			base.RawValue = value;
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x06000096 RID: 150 RVA: 0x000043FF File Offset: 0x000025FF
		internal override TokenType TokenType
		{
			get
			{
				return TokenType.Number;
			}
		}

		// Token: 0x06000097 RID: 151 RVA: 0x00004403 File Offset: 0x00002603
		internal override string EvaluateString(TextProcessingContext context, TextObject parent)
		{
			return base.RawValue;
		}
	}
}
