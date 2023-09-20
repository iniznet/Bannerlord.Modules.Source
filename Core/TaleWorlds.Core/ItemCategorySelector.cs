using System;

namespace TaleWorlds.Core
{
	// Token: 0x0200006D RID: 109
	public abstract class ItemCategorySelector : GameModel
	{
		// Token: 0x06000706 RID: 1798
		public abstract ItemCategory GetItemCategoryForItem(ItemObject itemObject);
	}
}
