using System;
using System.Collections.Generic;
using System.Linq;

namespace TaleWorlds.MountAndBlade
{
	public static class IntermissionVoteItemListExtensions
	{
		public static bool ContainsItem(this List<IntermissionVoteItem> intermissionVoteItems, string id)
		{
			return intermissionVoteItems != null && intermissionVoteItems.FirstOrDefault((IntermissionVoteItem item) => item.Id == id) != null;
		}

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
