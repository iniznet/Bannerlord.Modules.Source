using System;
using TaleWorlds.Core.ViewModelCollection.Selector;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement.ClanFinance
{
	public class WorkshopPercentageSelectorItemVM : SelectorItemVM
	{
		public WorkshopPercentageSelectorItemVM(string s, float percentage)
			: base(s)
		{
			this.Percentage = percentage;
		}

		public readonly float Percentage;
	}
}
