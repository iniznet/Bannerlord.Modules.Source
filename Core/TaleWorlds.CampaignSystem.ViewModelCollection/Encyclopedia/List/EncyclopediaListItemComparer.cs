using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.Encyclopedia;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.List
{
	public class EncyclopediaListItemComparer : IComparer<EncyclopediaListItemVM>
	{
		public EncyclopediaSortController SortController { get; }

		public EncyclopediaListItemComparer(EncyclopediaSortController sortController)
		{
			this.SortController = sortController;
		}

		private int GetBookmarkComparison(EncyclopediaListItemVM x, EncyclopediaListItemVM y)
		{
			return -x.IsBookmarked.CompareTo(y.IsBookmarked);
		}

		public int Compare(EncyclopediaListItemVM x, EncyclopediaListItemVM y)
		{
			int bookmarkComparison = this.GetBookmarkComparison(x, y);
			if (bookmarkComparison != 0)
			{
				return bookmarkComparison;
			}
			return this.SortController.Comparer.Compare(x.ListItem, y.ListItem);
		}
	}
}
