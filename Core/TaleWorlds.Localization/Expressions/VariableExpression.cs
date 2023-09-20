using System;
using TaleWorlds.Library;
using TaleWorlds.Localization.TextProcessor;

namespace TaleWorlds.Localization.Expressions
{
	// Token: 0x0200001F RID: 31
	internal class VariableExpression : TextExpression
	{
		// Token: 0x17000025 RID: 37
		// (get) Token: 0x060000C5 RID: 197 RVA: 0x00004A2F File Offset: 0x00002C2F
		public string IdentifierName
		{
			get
			{
				return this._identifierName;
			}
		}

		// Token: 0x060000C6 RID: 198 RVA: 0x00004A37 File Offset: 0x00002C37
		public VariableExpression(string identifierName, VariableExpression innerExpression)
		{
			base.RawValue = identifierName;
			this._identifierName = identifierName;
			this._innerVariable = innerExpression;
		}

		// Token: 0x060000C7 RID: 199 RVA: 0x00004A54 File Offset: 0x00002C54
		internal MultiStatement GetValue(TextProcessingContext context, TextObject parent)
		{
			if (this._innerVariable == null)
			{
				return context.GetVariableValue(this._identifierName, parent);
			}
			MultiStatement value = this._innerVariable.GetValue(context, parent);
			if (value != null && value != null)
			{
				foreach (TextExpression textExpression in value.SubStatements)
				{
					FieldExpression fieldExpression = textExpression as FieldExpression;
					if (fieldExpression != null && fieldExpression.FieldName == this._identifierName)
					{
						if (fieldExpression.InnerExpression is MultiStatement)
						{
							return fieldExpression.InnerExpression as MultiStatement;
						}
						return new MultiStatement(new TextExpression[] { fieldExpression.InnerExpression });
					}
				}
			}
			return null;
		}

		// Token: 0x060000C8 RID: 200 RVA: 0x00004B1C File Offset: 0x00002D1C
		internal override string EvaluateString(TextProcessingContext context, TextObject parent)
		{
			MultiStatement value = this.GetValue(context, parent);
			if (value != null)
			{
				MBStringBuilder mbstringBuilder = default(MBStringBuilder);
				mbstringBuilder.Initialize(16, "EvaluateString");
				foreach (TextExpression textExpression in value.SubStatements)
				{
					if (textExpression != null)
					{
						mbstringBuilder.Append<string>(textExpression.EvaluateString(context, parent));
					}
				}
				return mbstringBuilder.ToStringAndRelease();
			}
			return "";
		}

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x060000C9 RID: 201 RVA: 0x00004BAC File Offset: 0x00002DAC
		internal override TokenType TokenType
		{
			get
			{
				return TokenType.Identifier;
			}
		}

		// Token: 0x04000049 RID: 73
		private VariableExpression _innerVariable;

		// Token: 0x0400004A RID: 74
		private string _identifierName;
	}
}
