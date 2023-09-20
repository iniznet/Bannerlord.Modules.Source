using System;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200023A RID: 570
	public static class ItemCollectionElementMissionExtensions
	{
		// Token: 0x06001F54 RID: 8020 RVA: 0x0006EF7C File Offset: 0x0006D17C
		public static StackArray.StackArray4Int GetItemHolsterIndices(this ItemObject item)
		{
			StackArray.StackArray4Int stackArray4Int = default(StackArray.StackArray4Int);
			for (int i = 0; i < item.ItemHolsters.Length; i++)
			{
				stackArray4Int[i] = ((item.ItemHolsters[i].Length > 0) ? MBItem.GetItemHolsterIndex(item.ItemHolsters[i]) : (-1));
			}
			for (int j = item.ItemHolsters.Length; j < 4; j++)
			{
				stackArray4Int[j] = -1;
			}
			return stackArray4Int;
		}
	}
}
