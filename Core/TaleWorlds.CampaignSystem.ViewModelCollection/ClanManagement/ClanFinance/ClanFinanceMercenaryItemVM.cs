using System;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement.ClanFinance
{
	// Token: 0x0200010F RID: 271
	public class ClanFinanceMercenaryItemVM : ClanFinanceIncomeItemBaseVM
	{
		// Token: 0x170008D1 RID: 2257
		// (get) Token: 0x060019BE RID: 6590 RVA: 0x0005D2C6 File Offset: 0x0005B4C6
		// (set) Token: 0x060019BF RID: 6591 RVA: 0x0005D2CE File Offset: 0x0005B4CE
		public Clan Clan { get; private set; }

		// Token: 0x060019C0 RID: 6592 RVA: 0x0005D2D8 File Offset: 0x0005B4D8
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

		// Token: 0x060019C1 RID: 6593 RVA: 0x0005D369 File Offset: 0x0005B569
		protected override void PopulateStatsList()
		{
			base.ItemProperties.Add(new SelectableItemPropertyVM("TEST", "TEST", null));
		}

		// Token: 0x060019C2 RID: 6594 RVA: 0x0005D386 File Offset: 0x0005B586
		protected override void PopulateActionList()
		{
		}
	}
}
