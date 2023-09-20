using System;
using TaleWorlds.Library;
using TaleWorlds.Localization.TextProcessor;

namespace TaleWorlds.Localization.Expressions
{
	internal class VariableExpression : TextExpression
	{
		public string IdentifierName
		{
			get
			{
				return this._identifierName;
			}
		}

		public VariableExpression(string identifierName, VariableExpression innerExpression)
		{
			base.RawValue = identifierName;
			this._identifierName = identifierName;
			this._innerVariable = innerExpression;
		}

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

		internal override TokenType TokenType
		{
			get
			{
				return TokenType.Identifier;
			}
		}

		private VariableExpression _innerVariable;

		private string _identifierName;
	}
}
