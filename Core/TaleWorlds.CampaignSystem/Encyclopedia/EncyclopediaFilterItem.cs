using System;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.Encyclopedia
{
	public class EncyclopediaFilterItem
	{
		public EncyclopediaFilterItem(TextObject name, Predicate<object> predicate)
		{
			this.Name = name;
			this.Predicate = predicate;
			this.IsActive = false;
		}

		public readonly TextObject Name;

		public readonly Predicate<object> Predicate;

		public bool IsActive;
	}
}
