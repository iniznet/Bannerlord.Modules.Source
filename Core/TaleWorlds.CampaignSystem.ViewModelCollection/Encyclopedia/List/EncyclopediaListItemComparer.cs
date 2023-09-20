using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.Encyclopedia;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.List
{
	// Token: 0x020000BF RID: 191
	public class EncyclopediaListItemComparer : IComparer<EncyclopediaListItemVM>
	{
		// Token: 0x1700064C RID: 1612
		// (get) Token: 0x060012D4 RID: 4820 RVA: 0x00048C37 File Offset: 0x00046E37
		public EncyclopediaSortController SortController { get; }

		// Token: 0x060012D5 RID: 4821 RVA: 0x00048C3F File Offset: 0x00046E3F
		public EncyclopediaListItemComparer(EncyclopediaSortController sortController)
		{
			this.SortController = sortController;
		}

		// Token: 0x060012D6 RID: 4822 RVA: 0x00048C50 File Offset: 0x00046E50
		private int GetBookmarkComparison(EncyclopediaListItemVM x, EncyclopediaListItemVM y)
		{
			return -x.IsBookmarked.CompareTo(y.IsBookmarked);
		}

		// Token: 0x060012D7 RID: 4823 RVA: 0x00048C74 File Offset: 0x00046E74
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
