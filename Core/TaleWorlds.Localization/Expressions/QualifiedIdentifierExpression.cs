using System;
using TaleWorlds.Localization.TextProcessor;

namespace TaleWorlds.Localization.Expressions
{
	internal class QualifiedIdentifierExpression : TextExpression
	{
		public string IdentifierName
		{
			get
			{
				return this._identifierName;
			}
		}

		public QualifiedIdentifierExpression(string identifierName)
		{
			this._identifierName = identifierName;
		}

		internal override TokenType TokenType
		{
			get
			{
				return TokenType.QualifiedIdentifier;
			}
		}

		internal override string EvaluateString(TextProcessingContext context, TextObject parent)
		{
			return context.GetQualifiedVariableValue(this._identifierName, parent).Item1.ToStringWithoutClear();
		}

		private readonly string _identifierName;
	}
}
