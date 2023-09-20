using System;
using System.Collections.Generic;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.Encyclopedia
{
	// Token: 0x02000160 RID: 352
	public abstract class EncyclopediaListItemComparerBase : IComparer<EncyclopediaListItem>
	{
		// Token: 0x1700066E RID: 1646
		// (get) Token: 0x06001876 RID: 6262 RVA: 0x0007BAE6 File Offset: 0x00079CE6
		// (set) Token: 0x06001877 RID: 6263 RVA: 0x0007BAEE File Offset: 0x00079CEE
		public bool IsAscending { get; private set; }

		// Token: 0x06001878 RID: 6264 RVA: 0x0007BAF7 File Offset: 0x00079CF7
		public void SetSortOrder(bool isAscending)
		{
			this.IsAscending = isAscending;
		}

		// Token: 0x06001879 RID: 6265 RVA: 0x0007BB00 File Offset: 0x00079D00
		public void SwitchSortOrder()
		{
			this.IsAscending = !this.IsAscending;
		}

		// Token: 0x0600187A RID: 6266 RVA: 0x0007BB11 File Offset: 0x00079D11
		public void SetDefaultSortOrder()
		{
			this.IsAscending = false;
		}

		// Token: 0x0600187B RID: 6267
		public abstract int Compare(EncyclopediaListItem x, EncyclopediaListItem y);

		// Token: 0x0600187C RID: 6268
		public abstract string GetComparedValueText(EncyclopediaListItem item);

		// Token: 0x0600187D RID: 6269 RVA: 0x0007BB1A File Offset: 0x00079D1A
		protected int ResolveEquality(EncyclopediaListItem x, EncyclopediaListItem y)
		{
			return x.Name.CompareTo(y.Name);
		}

		// Token: 0x040008AB RID: 2219
		protected readonly TextObject _emptyValue = new TextObject("{=4NaOKslb}-", null);

		// Token: 0x040008AC RID: 2220
		protected readonly TextObject _missingValue = new TextObject("{=keqS2dGa}???", null);
	}
}
