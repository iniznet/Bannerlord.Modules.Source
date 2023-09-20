using System;
using System.Collections.Generic;

namespace TaleWorlds.Engine.Options
{
	// Token: 0x0200009D RID: 157
	public interface ISelectionOptionData : IOptionData
	{
		// Token: 0x06000BB6 RID: 2998
		int GetSelectableOptionsLimit();

		// Token: 0x06000BB7 RID: 2999
		IEnumerable<SelectionData> GetSelectableOptionNames();
	}
}
