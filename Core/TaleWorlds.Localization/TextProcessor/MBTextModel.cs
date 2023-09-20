using System;
using TaleWorlds.Library;
using TaleWorlds.Localization.Expressions;

namespace TaleWorlds.Localization.TextProcessor
{
	public class MBTextModel
	{
		internal MBReadOnlyList<TextExpression> RootExpressions
		{
			get
			{
				return this._rootExpressions;
			}
		}

		internal void AddRootExpression(TextExpression newExp)
		{
			this._rootExpressions.Add(newExp);
		}

		internal MBList<TextExpression> _rootExpressions = new MBList<TextExpression>();
	}
}
