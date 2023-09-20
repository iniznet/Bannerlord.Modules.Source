using System;
using TaleWorlds.Localization.TextProcessor;

namespace TaleWorlds.Localization.Expressions
{
	// Token: 0x0200001C RID: 28
	internal class FieldExpression : TextExpression
	{
		// Token: 0x17000020 RID: 32
		// (get) Token: 0x060000B8 RID: 184 RVA: 0x0000487B File Offset: 0x00002A7B
		public string FieldName
		{
			get
			{
				return base.RawValue;
			}
		}

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x060000B9 RID: 185 RVA: 0x00004883 File Offset: 0x00002A83
		public TextExpression InnerExpression
		{
			get
			{
				return this.part2;
			}
		}

		// Token: 0x060000BA RID: 186 RVA: 0x0000488B File Offset: 0x00002A8B
		public FieldExpression(TextExpression innerExpression)
		{
			this._innerExpression = innerExpression;
			base.RawValue = innerExpression.RawValue;
		}

		// Token: 0x060000BB RID: 187 RVA: 0x000048A6 File Offset: 0x00002AA6
		public FieldExpression(TextExpression innerExpression, TextExpression part2)
			: this(innerExpression)
		{
			this.part2 = part2;
		}

		// Token: 0x060000BC RID: 188 RVA: 0x000048B6 File Offset: 0x00002AB6
		internal override string EvaluateString(TextProcessingContext context, TextObject parent)
		{
			return "";
		}

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x060000BD RID: 189 RVA: 0x000048BD File Offset: 0x00002ABD
		internal override TokenType TokenType
		{
			get
			{
				return TokenType.FieldExpression;
			}
		}

		// Token: 0x04000043 RID: 67
		private TextExpression _innerExpression;

		// Token: 0x04000044 RID: 68
		private TextExpression part2;
	}
}
