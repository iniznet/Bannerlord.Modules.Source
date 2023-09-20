using System;
using TaleWorlds.Localization.TextProcessor;

namespace TaleWorlds.Localization.Expressions
{
	// Token: 0x02000023 RID: 35
	internal class QualifiedIdentifierExpression : TextExpression
	{
		// Token: 0x1700002B RID: 43
		// (get) Token: 0x060000D5 RID: 213 RVA: 0x00004E2D File Offset: 0x0000302D
		public string IdentifierName
		{
			get
			{
				return this._identifierName;
			}
		}

		// Token: 0x060000D6 RID: 214 RVA: 0x00004E35 File Offset: 0x00003035
		public QualifiedIdentifierExpression(string identifierName)
		{
			this._identifierName = identifierName;
		}

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x060000D7 RID: 215 RVA: 0x00004E44 File Offset: 0x00003044
		internal override TokenType TokenType
		{
			get
			{
				return TokenType.QualifiedIdentifier;
			}
		}

		// Token: 0x060000D8 RID: 216 RVA: 0x00004E48 File Offset: 0x00003048
		internal override string EvaluateString(TextProcessingContext context, TextObject parent)
		{
			return context.GetQualifiedVariableValue(this._identifierName, parent).Item1.ToStringWithoutClear();
		}

		// Token: 0x04000050 RID: 80
		private readonly string _identifierName;
	}
}
