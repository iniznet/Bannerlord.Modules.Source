using System;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement.ClanFinance
{
	// Token: 0x0200010E RID: 270
	public class ClanFinanceCommonAreaItemVM : ClanFinanceIncomeItemBaseVM
	{
		// Token: 0x060019BB RID: 6587 RVA: 0x0005D1E0 File Offset: 0x0005B3E0
		public ClanFinanceCommonAreaItemVM(Alley alley, Action<ClanFinanceIncomeItemBaseVM> onSelection, Action onRefresh)
			: base(onSelection, onRefresh)
		{
			base.IncomeTypeAsEnum = IncomeTypes.CommonArea;
			this._alley = alley;
			GameTexts.SetVariable("SETTLEMENT_NAME", alley.Settlement.Name);
			GameTexts.SetVariable("COMMON_AREA_NAME", alley.Name);
			base.Name = GameTexts.FindText("str_clan_finance_common_area", null).ToString();
			base.Income = Campaign.Current.Models.AlleyModel.GetDailyIncomeOfAlley(alley);
			base.Visual = ((alley.Owner.CharacterObject != null) ? new ImageIdentifierVM(CharacterCode.CreateFrom(alley.Owner.CharacterObject)) : new ImageIdentifierVM(ImageIdentifierType.Null));
			base.IncomeValueText = base.DetermineIncomeText(base.Income);
			this.PopulateActionList();
			this.PopulateStatsList();
		}

		// Token: 0x060019BC RID: 6588 RVA: 0x0005D2A7 File Offset: 0x0005B4A7
		protected override void PopulateActionList()
		{
		}

		// Token: 0x060019BD RID: 6589 RVA: 0x0005D2A9 File Offset: 0x0005B4A9
		protected override void PopulateStatsList()
		{
			base.ItemProperties.Add(new SelectableItemPropertyVM("TEST", "TEST", null));
		}

		// Token: 0x04000C38 RID: 3128
		private Alley _alley;
	}
}
