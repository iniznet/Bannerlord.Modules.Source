using System;

namespace TaleWorlds.CampaignSystem.Encyclopedia
{
	internal class EncyclopediaListItemNameComparer : EncyclopediaListItemComparerBase
	{
		public override int Compare(EncyclopediaListItem x, EncyclopediaListItem y)
		{
			return base.ResolveEquality(x, y);
		}

		public override string GetComparedValueText(EncyclopediaListItem item)
		{
			return "";
		}
	}
}
