using System;
using TaleWorlds.Library;
using TaleWorlds.Localization.Expressions;

namespace TaleWorlds.Localization.TextProcessor
{
	// Token: 0x02000027 RID: 39
	public class MBTextModel
	{
		// Token: 0x17000030 RID: 48
		// (get) Token: 0x060000E3 RID: 227 RVA: 0x00004FBA File Offset: 0x000031BA
		internal MBReadOnlyList<TextExpression> RootExpressions
		{
			get
			{
				return this._rootExpressions;
			}
		}

		// Token: 0x060000E5 RID: 229 RVA: 0x00004FD5 File Offset: 0x000031D5
		internal void AddRootExpression(TextExpression newExp)
		{
			this._rootExpressions.Add(newExp);
		}

		// Token: 0x04000055 RID: 85
		internal MBList<TextExpression> _rootExpressions = new MBList<TextExpression>();
	}
}
