using System;
using TaleWorlds.Localization.TextProcessor;

namespace TaleWorlds.Localization.Expressions
{
	// Token: 0x0200000E RID: 14
	internal class SimpleText : TextExpression
	{
		// Token: 0x06000092 RID: 146 RVA: 0x000043D5 File Offset: 0x000025D5
		public SimpleText(string value)
		{
			base.RawValue = value;
		}

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x06000093 RID: 147 RVA: 0x000043E4 File Offset: 0x000025E4
		internal override TokenType TokenType
		{
			get
			{
				return TokenType.Text;
			}
		}

		// Token: 0x06000094 RID: 148 RVA: 0x000043E8 File Offset: 0x000025E8
		internal override string EvaluateString(TextProcessingContext context, TextObject parent)
		{
			return base.RawValue;
		}
	}
}
