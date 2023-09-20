using System;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement.ClanFinance
{
	// Token: 0x02000110 RID: 272
	public class ClanFinanceTownItemVM : ClanFinanceIncomeItemBaseVM
	{
		// Token: 0x170008D2 RID: 2258
		// (get) Token: 0x060019C3 RID: 6595 RVA: 0x0005D388 File Offset: 0x0005B588
		// (set) Token: 0x060019C4 RID: 6596 RVA: 0x0005D390 File Offset: 0x0005B590
		public Settlement Settlement { get; private set; }

		// Token: 0x060019C5 RID: 6597 RVA: 0x0005D39C File Offset: 0x0005B59C
		public ClanFinanceTownItemVM(Settlement settlement, TaxType taxType, Action<ClanFinanceIncomeItemBaseVM> onSelection, Action onRefresh)
			: base(onSelection, onRefresh)
		{
			base.IncomeTypeAsEnum = IncomeTypes.Settlement;
			this.Settlement = settlement;
			MBTextManager.SetTextVariable("SETTLEMENT_NAME", settlement.Name.ToString(), false);
			base.Name = ((taxType == TaxType.ProsperityTax) ? GameTexts.FindText("str_prosperity_tax", null).ToString() : GameTexts.FindText("str_trade_tax", null).ToString());
			this.IsUnderSiege = settlement.IsUnderSiege;
			this.IsUnderSiegeHint = new HintViewModel(new TextObject("{=!}PLACEHOLDER | THIS SETTLEMENT IS UNDER SIEGE", null), null);
			this.IsUnderRebellion = settlement.IsUnderRebellionAttack();
			this.IsUnderRebellionHint = new HintViewModel(new TextObject("{=!}PLACEHOLDER | THIS SETTLEMENT IS UNDER REBELLION", null), null);
			if (taxType == TaxType.ProsperityTax && settlement.Town != null)
			{
				float resultNumber = Campaign.Current.Models.SettlementTaxModel.CalculateTownTax(settlement.Town, false).ResultNumber;
				base.Income = (this.IsUnderRebellion ? 0 : ((int)resultNumber));
			}
			else if (taxType == TaxType.TradeTax)
			{
				if (settlement.Town != null)
				{
					base.Income = (int)((float)settlement.Town.TradeTaxAccumulated / Campaign.Current.Models.ClanFinanceModel.RevenueSmoothenFraction());
				}
				else if (settlement.Village != null)
				{
					base.Income = ((settlement.Village.VillageState == Village.VillageStates.Looted || settlement.Village.VillageState == Village.VillageStates.BeingRaided) ? 0 : ((int)((float)settlement.Village.TradeTaxAccumulated / Campaign.Current.Models.ClanFinanceModel.RevenueSmoothenFraction())));
				}
			}
			base.IncomeValueText = base.DetermineIncomeText(base.Income);
			this.HasGovernor = settlement.IsTown && settlement.Town.Governor != null;
		}

		// Token: 0x060019C6 RID: 6598 RVA: 0x0005D547 File Offset: 0x0005B747
		protected override void PopulateActionList()
		{
		}

		// Token: 0x060019C7 RID: 6599 RVA: 0x0005D549 File Offset: 0x0005B749
		protected override void PopulateStatsList()
		{
		}

		// Token: 0x170008D3 RID: 2259
		// (get) Token: 0x060019C8 RID: 6600 RVA: 0x0005D54B File Offset: 0x0005B74B
		// (set) Token: 0x060019C9 RID: 6601 RVA: 0x0005D553 File Offset: 0x0005B753
		[DataSourceProperty]
		public bool IsUnderSiege
		{
			get
			{
				return this._isUnderSiege;
			}
			set
			{
				if (value != this._isUnderSiege)
				{
					this._isUnderSiege = value;
					base.OnPropertyChangedWithValue(value, "IsUnderSiege");
				}
			}
		}

		// Token: 0x170008D4 RID: 2260
		// (get) Token: 0x060019CA RID: 6602 RVA: 0x0005D571 File Offset: 0x0005B771
		// (set) Token: 0x060019CB RID: 6603 RVA: 0x0005D579 File Offset: 0x0005B779
		[DataSourceProperty]
		public bool IsUnderRebellion
		{
			get
			{
				return this._isUnderRebellion;
			}
			set
			{
				if (value != this._isUnderRebellion)
				{
					this._isUnderRebellion = value;
					base.OnPropertyChangedWithValue(value, "IsUnderRebellion");
				}
			}
		}

		// Token: 0x170008D5 RID: 2261
		// (get) Token: 0x060019CC RID: 6604 RVA: 0x0005D597 File Offset: 0x0005B797
		// (set) Token: 0x060019CD RID: 6605 RVA: 0x0005D59F File Offset: 0x0005B79F
		[DataSourceProperty]
		public HintViewModel IsUnderSiegeHint
		{
			get
			{
				return this._isUnderSiegeHint;
			}
			set
			{
				if (value != this._isUnderSiegeHint)
				{
					this._isUnderSiegeHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "IsUnderSiegeHint");
				}
			}
		}

		// Token: 0x170008D6 RID: 2262
		// (get) Token: 0x060019CE RID: 6606 RVA: 0x0005D5BD File Offset: 0x0005B7BD
		// (set) Token: 0x060019CF RID: 6607 RVA: 0x0005D5C5 File Offset: 0x0005B7C5
		[DataSourceProperty]
		public HintViewModel IsUnderRebellionHint
		{
			get
			{
				return this._isUnderRebellionHint;
			}
			set
			{
				if (value != this._isUnderRebellionHint)
				{
					this._isUnderRebellionHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "IsUnderRebellionHint");
				}
			}
		}

		// Token: 0x170008D7 RID: 2263
		// (get) Token: 0x060019D0 RID: 6608 RVA: 0x0005D5E3 File Offset: 0x0005B7E3
		// (set) Token: 0x060019D1 RID: 6609 RVA: 0x0005D5EB File Offset: 0x0005B7EB
		[DataSourceProperty]
		public bool HasGovernor
		{
			get
			{
				return this._hasGovernor;
			}
			set
			{
				if (value != this._hasGovernor)
				{
					this._hasGovernor = value;
					base.OnPropertyChangedWithValue(value, "HasGovernor");
				}
			}
		}

		// Token: 0x170008D8 RID: 2264
		// (get) Token: 0x060019D2 RID: 6610 RVA: 0x0005D609 File Offset: 0x0005B809
		// (set) Token: 0x060019D3 RID: 6611 RVA: 0x0005D611 File Offset: 0x0005B811
		[DataSourceProperty]
		public HintViewModel GovernorHint
		{
			get
			{
				return this._governorHint;
			}
			set
			{
				if (value != this._governorHint)
				{
					this._governorHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "GovernorHint");
				}
			}
		}

		// Token: 0x04000C3B RID: 3131
		private bool _isUnderSiege;

		// Token: 0x04000C3C RID: 3132
		private bool _isUnderRebellion;

		// Token: 0x04000C3D RID: 3133
		private HintViewModel _isUnderSiegeHint;

		// Token: 0x04000C3E RID: 3134
		private HintViewModel _isUnderRebellionHint;

		// Token: 0x04000C3F RID: 3135
		private HintViewModel _governorHint;

		// Token: 0x04000C40 RID: 3136
		private bool _hasGovernor;
	}
}
