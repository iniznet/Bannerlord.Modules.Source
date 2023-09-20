using System;
using TaleWorlds.Localization.TextProcessor;

namespace TaleWorlds.Localization.Expressions
{
	// Token: 0x02000010 RID: 16
	internal class LangaugeMarkerExpression : TextExpression
	{
		// Token: 0x06000098 RID: 152 RVA: 0x0000440B File Offset: 0x0000260B
		public LangaugeMarkerExpression(string innerText)
		{
			base.RawValue = innerText;
		}

		// Token: 0x06000099 RID: 153 RVA: 0x0000441A File Offset: 0x0000261A
		internal override string EvaluateString(TextProcessingContext context, TextObject parent)
		{
			return base.RawValue;
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x0600009A RID: 154 RVA: 0x00004422 File Offset: 0x00002622
		internal override TokenType TokenType
		{
			get
			{
				return TokenType.LanguageMarker;
			}
		}
	}
}
