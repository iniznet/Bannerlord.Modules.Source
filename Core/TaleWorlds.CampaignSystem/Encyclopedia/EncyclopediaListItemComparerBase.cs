using System;
using System.Collections.Generic;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.Encyclopedia
{
	public abstract class EncyclopediaListItemComparerBase : IComparer<EncyclopediaListItem>
	{
		public bool IsAscending { get; private set; }

		public void SetSortOrder(bool isAscending)
		{
			this.IsAscending = isAscending;
		}

		public void SwitchSortOrder()
		{
			this.IsAscending = !this.IsAscending;
		}

		public void SetDefaultSortOrder()
		{
			this.IsAscending = false;
		}

		public abstract int Compare(EncyclopediaListItem x, EncyclopediaListItem y);

		public abstract string GetComparedValueText(EncyclopediaListItem item);

		protected int ResolveEquality(EncyclopediaListItem x, EncyclopediaListItem y)
		{
			return x.Name.CompareTo(y.Name);
		}

		protected readonly TextObject _emptyValue = new TextObject("{=4NaOKslb}-", null);

		protected readonly TextObject _missingValue = new TextObject("{=keqS2dGa}???", null);
	}
}
