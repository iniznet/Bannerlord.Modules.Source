using System;
using TaleWorlds.Library;
using TaleWorlds.Localization.TextProcessor;

namespace TaleWorlds.Localization.Expressions
{
	internal class ArrayReference : TextExpression
	{
		public ArrayReference(string rawValue, TextExpression indexExp)
		{
			base.RawValue = rawValue;
			this._indexExp = indexExp;
		}

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

		internal override TokenType TokenType
		{
			get
			{
				return TokenType.ArrayAccess;
			}
		}

		private TextExpression _indexExp;
	}
}
