using System;
using TaleWorlds.Library;
using TaleWorlds.Localization.TextProcessor;

namespace TaleWorlds.Localization.Expressions
{
	internal class MarkerOccuranceTextExpression : TextExpression
	{
		public string IdentifierName
		{
			get
			{
				return this._identifierName;
			}
		}

		public MarkerOccuranceTextExpression(string identifierName, VariableExpression innerExpression)
		{
			base.RawValue = identifierName;
			this._identifierName = identifierName;
			this._innerVariable = innerExpression;
		}

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

		internal override TokenType TokenType
		{
			get
			{
				return TokenType.MarkerOccuranceExpression;
			}
		}

		private VariableExpression _innerVariable;

		private string _identifierName;
	}
}
