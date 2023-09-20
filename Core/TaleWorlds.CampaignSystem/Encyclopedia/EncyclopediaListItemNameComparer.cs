using System;

namespace TaleWorlds.CampaignSystem.Encyclopedia
{
	// Token: 0x0200015F RID: 351
	internal class EncyclopediaListItemNameComparer : EncyclopediaListItemComparerBase
	{
		// Token: 0x06001873 RID: 6259 RVA: 0x0007BACD File Offset: 0x00079CCD
		public override int Compare(EncyclopediaListItem x, EncyclopediaListItem y)
		{
			return base.ResolveEquality(x, y);
		}

		// Token: 0x06001874 RID: 6260 RVA: 0x0007BAD7 File Offset: 0x00079CD7
		public override string GetComparedValueText(EncyclopediaListItem item)
		{
			return "";
		}
	}
}
