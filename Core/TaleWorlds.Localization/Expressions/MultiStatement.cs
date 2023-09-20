using System;
using System.Collections.Generic;
using TaleWorlds.Library;
using TaleWorlds.Localization.TextProcessor;

namespace TaleWorlds.Localization.Expressions
{
	// Token: 0x02000013 RID: 19
	internal class MultiStatement : TextExpression
	{
		// Token: 0x060000A2 RID: 162 RVA: 0x000044D2 File Offset: 0x000026D2
		public MultiStatement(IEnumerable<TextExpression> subStatements)
		{
			this._subStatements = subStatements.ToMBList<TextExpression>();
		}

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x060000A3 RID: 163 RVA: 0x000044F1 File Offset: 0x000026F1
		public MBReadOnlyList<TextExpression> SubStatements
		{
			get
			{
				return this._subStatements;
			}
		}

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x060000A4 RID: 164 RVA: 0x000044F9 File Offset: 0x000026F9
		internal override TokenType TokenType
		{
			get
			{
				return TokenType.MultiStatement;
			}
		}

		// Token: 0x060000A5 RID: 165 RVA: 0x000044FD File Offset: 0x000026FD
		public void AddStatement(TextExpression s2)
		{
			this._subStatements.Add(s2);
		}

		// Token: 0x060000A6 RID: 166 RVA: 0x0000450C File Offset: 0x0000270C
		internal override string EvaluateString(TextProcessingContext context, TextObject parent)
		{
			MBStringBuilder mbstringBuilder = default(MBStringBuilder);
			mbstringBuilder.Initialize(16, "EvaluateString");
			foreach (TextExpression textExpression in this._subStatements)
			{
				if (textExpression != null)
				{
					mbstringBuilder.Append<string>(textExpression.EvaluateString(context, parent));
				}
			}
			return mbstringBuilder.ToStringAndRelease();
		}

		// Token: 0x0400002A RID: 42
		private MBList<TextExpression> _subStatements = new MBList<TextExpression>();
	}
}
