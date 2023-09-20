using System;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement.ClanFinance
{
	public class ClanFinanceMercenaryItemVM : ClanFinanceIncomeItemBaseVM
	{
		public Clan Clan { get; private set; }

		public ClanFinanceMercenaryItemVM(Action<ClanFinanceIncomeItemBaseVM> onSelection, Action onRefresh)
			: base(onSelection, onRefresh)
		{
			base.IncomeTypeAsEnum = IncomeTypes.MercenaryService;
			this.Clan = Clan.PlayerClan;
			if (this.Clan.IsUnderMercenaryService)
			{
				base.Name = GameTexts.FindText("str_mercenary_service", null).ToString();
				base.Income = (int)(this.Clan.Influence * (float)this.Clan.MercenaryAwardMultiplier);
				base.Visual = new ImageIdentifierVM(this.Clan.Banner);
				base.IncomeValueText = base.DetermineIncomeText(base.Income);
			}
		}

		protected override void PopulateStatsList()
		{
			base.ItemProperties.Add(new SelectableItemPropertyVM("TEST", "TEST", false, null));
		}

		protected override void PopulateActionList()
		{
		}
	}
}
