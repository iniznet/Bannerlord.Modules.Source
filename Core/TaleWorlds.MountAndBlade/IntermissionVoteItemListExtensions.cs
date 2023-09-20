using System;
using System.Collections.Generic;
using System.Linq;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000303 RID: 771
	public static class IntermissionVoteItemListExtensions
	{
		// Token: 0x0600299D RID: 10653 RVA: 0x000A161C File Offset: 0x0009F81C
		public static bool ContainsItem(this List<IntermissionVoteItem> intermissionVoteItems, string id)
		{
			return intermissionVoteItems != null && intermissionVoteItems.FirstOrDefault((IntermissionVoteItem item) => item.Id == id) != null;
		}

		// Token: 0x0600299E RID: 10654 RVA: 0x000A1650 File Offset: 0x0009F850
		public static IntermissionVoteItem Add(this List<IntermissionVoteItem> intermissionVoteItems, string id)
		{
			IntermissionVoteItem intermissionVoteItem = null;
			if (intermissionVoteItems != null)
			{
				int count = intermissionVoteItems.Count;
				IntermissionVoteItem intermissionVoteItem2 = new IntermissionVoteItem(id, count);
				intermissionVoteItems.Add(intermissionVoteItem2);
				intermissionVoteItem = intermissionVoteItem2;
			}
			return intermissionVoteItem;
		}

		// Token: 0x0600299F RID: 10655 RVA: 0x000A167C File Offset: 0x0009F87C
		public static IntermissionVoteItem GetItem(this List<IntermissionVoteItem> intermissionVoteItems, string id)
		{
			IntermissionVoteItem intermissionVoteItem = null;
			if (intermissionVoteItems != null)
			{
				intermissionVoteItem = intermissionVoteItems.FirstOrDefault((IntermissionVoteItem item) => item.Id == id);
			}
			return intermissionVoteItem;
		}
	}
}
