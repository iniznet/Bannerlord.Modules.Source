using System;
using TaleWorlds.Localization.TextProcessor;

namespace TaleWorlds.Localization.Expressions
{
	// Token: 0x02000011 RID: 17
	internal class TextIdExpression : TextExpression
	{
		// Token: 0x0600009B RID: 155 RVA: 0x00004426 File Offset: 0x00002626
		public TextIdExpression(string innerText)
		{
			base.RawValue = innerText;
		}

		// Token: 0x0600009C RID: 156 RVA: 0x00004435 File Offset: 0x00002635
		internal override string EvaluateString(TextProcessingContext context, TextObject parent)
		{
			return "";
		}

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x0600009D RID: 157 RVA: 0x0000443C File Offset: 0x0000263C
		internal override TokenType TokenType
		{
			get
			{
				return TokenType.textId;
			}
		}
	}
}
