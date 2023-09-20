using System;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement.ClanFinance
{
	public class ClanFinanceCommonAreaItemVM : ClanFinanceIncomeItemBaseVM
	{
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

		protected override void PopulateActionList()
		{
		}

		protected override void PopulateStatsList()
		{
			base.ItemProperties.Add(new SelectableItemPropertyVM("TEST", "TEST", false, null));
		}

		private Alley _alley;
	}
}
