using System;
using System.Collections.Generic;
using TaleWorlds.Library;
using TaleWorlds.Localization.TextProcessor;

namespace TaleWorlds.Localization.Expressions
{
	internal class MultiStatement : TextExpression
	{
		public MultiStatement(IEnumerable<TextExpression> subStatements)
		{
			this._subStatements = subStatements.ToMBList<TextExpression>();
		}

		public MBReadOnlyList<TextExpression> SubStatements
		{
			get
			{
				return this._subStatements;
			}
		}

		internal override TokenType TokenType
		{
			get
			{
				return TokenType.MultiStatement;
			}
		}

		public void AddStatement(TextExpression s2)
		{
			this._subStatements.Add(s2);
		}

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

		private MBList<TextExpression> _subStatements = new MBList<TextExpression>();
	}
}
