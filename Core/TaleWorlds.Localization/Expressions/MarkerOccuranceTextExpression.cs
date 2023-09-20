using System;
using TaleWorlds.Library;
using TaleWorlds.Localization.TextProcessor;

namespace TaleWorlds.Localization.Expressions
{
	// Token: 0x02000020 RID: 32
	internal class MarkerOccuranceTextExpression : TextExpression
	{
		// Token: 0x17000027 RID: 39
		// (get) Token: 0x060000CA RID: 202 RVA: 0x00004BB0 File Offset: 0x00002DB0
		public string IdentifierName
		{
			get
			{
				return this._identifierName;
			}
		}

		// Token: 0x060000CB RID: 203 RVA: 0x00004BB8 File Offset: 0x00002DB8
		public MarkerOccuranceTextExpression(string identifierName, VariableExpression innerExpression)
		{
			base.RawValue = identifierName;
			this._identifierName = identifierName;
			this._innerVariable = innerExpression;
		}

		// Token: 0x060000CC RID: 204 RVA: 0x00004BD8 File Offset: 0x00002DD8
		private string MarkerOccuranceExpression(string identifierName, string text)
		{
			int i = 0;
			int num = 0;
			int num2 = 0;
			MBStringBuilder mbstringBuilder = default(MBStringBuilder);
			mbstringBuilder.Initialize(16, "MarkerOccuranceExpression");
			while (i < text.Length)
			{
				if (text[i] != '{')
				{
					if (num == 1 && num2 == 0)
					{
						mbstringBuilder.Append(text[i]);
					}
				}
				else
				{
					string text2 = TextProcessingContext.ReadFirstToken(text, ref i);
					if (TextProcessingContext.IsDeclarationFinalizer(text2))
					{
						num--;
						if (num2 > num)
						{
							num2 = num;
						}
					}
					else if (TextProcessingContext.IsDeclaration(text2))
					{
						string text3 = text2.Substring(1);
						bool flag = num2 == num && string.Compare(identifierName, text3, StringComparison.InvariantCultureIgnoreCase) == 0;
						num++;
						if (flag)
						{
							num2 = num;
						}
					}
				}
				i++;
			}
			return mbstringBuilder.ToStringAndRelease();
		}

		// Token: 0x060000CD RID: 205 RVA: 0x00004C88 File Offset: 0x00002E88
		internal override string EvaluateString(TextProcessingContext context, TextObject parent)
		{
			MultiStatement value = this._innerVariable.GetValue(context, parent);
			if (value != null)
			{
				foreach (TextExpression textExpression in value.SubStatements)
				{
					if (textExpression.TokenType == TokenType.LanguageMarker && textExpression.RawValue.Substring(2, textExpression.RawValue.Length - 3) == this.IdentifierName)
					{
						return "1";
					}
				}
			}
			return "0";
		}

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x060000CE RID: 206 RVA: 0x00004D24 File Offset: 0x00002F24
		internal override TokenType TokenType
		{
			get
			{
				return TokenType.MarkerOccuranceExpression;
			}
		}

		// Token: 0x0400004B RID: 75
		private VariableExpression _innerVariable;

		// Token: 0x0400004C RID: 76
		private string _identifierName;
	}
}
