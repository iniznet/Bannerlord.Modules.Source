using System;
using TaleWorlds.Library;
using TaleWorlds.Localization.TextProcessor;

namespace TaleWorlds.Localization.Expressions
{
	// Token: 0x02000021 RID: 33
	internal class ArrayReference : TextExpression
	{
		// Token: 0x060000CF RID: 207 RVA: 0x00004D28 File Offset: 0x00002F28
		public ArrayReference(string rawValue, TextExpression indexExp)
		{
			base.RawValue = rawValue;
			this._indexExp = indexExp;
		}

		// Token: 0x060000D0 RID: 208 RVA: 0x00004D40 File Offset: 0x00002F40
		internal override string EvaluateString(TextProcessingContext context, TextObject parent)
		{
			int num = base.EvaluateAsNumber(this._indexExp, context, parent);
			MultiStatement arrayAccess = context.GetArrayAccess(base.RawValue, num);
			if (arrayAccess != null)
			{
				MBStringBuilder mbstringBuilder = default(MBStringBuilder);
				mbstringBuilder.Initialize(16, "EvaluateString");
				foreach (TextExpression textExpression in arrayAccess.SubStatements)
				{
					mbstringBuilder.Append<string>(textExpression.EvaluateString(context, parent));
				}
				return mbstringBuilder.ToStringAndRelease();
			}
			return "";
		}

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x060000D1 RID: 209 RVA: 0x00004DE4 File Offset: 0x00002FE4
		internal override TokenType TokenType
		{
			get
			{
				return TokenType.ArrayAccess;
			}
		}

		// Token: 0x0400004D RID: 77
		private TextExpression _indexExp;
	}
}
