using System;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting.WeaponDesign
{
	// Token: 0x020000E7 RID: 231
	public class WeaponDesignResultPropertyItemVM : ViewModel
	{
		// Token: 0x06001526 RID: 5414 RVA: 0x0004EF84 File Offset: 0x0004D184
		public WeaponDesignResultPropertyItemVM(TextObject description, float value, float changeAmount, bool showFloatingPoint)
		{
			this._description = description;
			this.InitialValue = value;
			this.ChangeAmount = changeAmount;
			this.ShowFloatingPoint = showFloatingPoint;
			this.IsOrderResult = false;
			this.OrderRequirementTooltip = new HintViewModel();
			this.CraftedValueTooltip = new HintViewModel();
			this.BonusPenaltyTooltip = new HintViewModel();
			this.RefreshValues();
		}

		// Token: 0x06001527 RID: 5415 RVA: 0x0004EFE4 File Offset: 0x0004D1E4
		public WeaponDesignResultPropertyItemVM(TextObject description, float craftedValue, float requiredValue, float changeAmount, bool showFloatingPoint, bool isExceedingBeneficial, bool showTooltip = true)
		{
			this._showTooltip = showTooltip;
			this._description = description;
			this.TargetValue = requiredValue;
			this.InitialValue = craftedValue;
			this.ChangeAmount = changeAmount;
			this._isExceedingBeneficial = isExceedingBeneficial;
			this.IsOrderResult = true;
			this.ShowFloatingPoint = showFloatingPoint;
			this.OrderRequirementTooltip = new HintViewModel();
			this.CraftedValueTooltip = new HintViewModel();
			this.BonusPenaltyTooltip = new HintViewModel();
			this.RefreshValues();
		}

		// Token: 0x06001528 RID: 5416 RVA: 0x0004F05C File Offset: 0x0004D25C
		public override void RefreshValues()
		{
			base.RefreshValues();
			TextObject description = this._description;
			this.PropertyLbl = ((description != null) ? description.ToString() : null);
			this.RequiredValueText = ((this.TargetValue == 0f) ? string.Empty : ("(" + (this.ShowFloatingPoint ? this.TargetValue.ToString("F1") : this.TargetValue.ToString("F0")) + ")"));
			this.HasBenefit = (this._isExceedingBeneficial ? (this.InitialValue + this.ChangeAmount >= this.TargetValue) : (this.InitialValue + this.ChangeAmount <= this.TargetValue));
			this.OrderRequirementTooltip.HintText = (this._showTooltip ? GameTexts.FindText("str_crafting_order_requirement_tooltip", null) : TextObject.Empty);
			this.CraftedValueTooltip.HintText = (this._showTooltip ? GameTexts.FindText("str_crafting_crafted_value_tooltip", null) : TextObject.Empty);
			this.BonusPenaltyTooltip.HintText = (this._showTooltip ? GameTexts.FindText("str_crafting_bonus_penalty_tooltip", null) : TextObject.Empty);
		}

		// Token: 0x17000723 RID: 1827
		// (get) Token: 0x06001529 RID: 5417 RVA: 0x0004F18F File Offset: 0x0004D38F
		// (set) Token: 0x0600152A RID: 5418 RVA: 0x0004F197 File Offset: 0x0004D397
		[DataSourceProperty]
		public string PropertyLbl
		{
			get
			{
				return this._propertyLbl;
			}
			set
			{
				if (value != this._propertyLbl)
				{
					this._propertyLbl = value;
					base.OnPropertyChangedWithValue<string>(value, "PropertyLbl");
				}
			}
		}

		// Token: 0x17000724 RID: 1828
		// (get) Token: 0x0600152B RID: 5419 RVA: 0x0004F1BA File Offset: 0x0004D3BA
		// (set) Token: 0x0600152C RID: 5420 RVA: 0x0004F1C2 File Offset: 0x0004D3C2
		[DataSourceProperty]
		public float InitialValue
		{
			get
			{
				return this._propertyValue;
			}
			set
			{
				if (value == 0f || value != this._propertyValue)
				{
					this._propertyValue = value;
					base.OnPropertyChangedWithValue(value, "InitialValue");
				}
			}
		}

		// Token: 0x17000725 RID: 1829
		// (get) Token: 0x0600152D RID: 5421 RVA: 0x0004F1E8 File Offset: 0x0004D3E8
		// (set) Token: 0x0600152E RID: 5422 RVA: 0x0004F1F0 File Offset: 0x0004D3F0
		[DataSourceProperty]
		public float TargetValue
		{
			get
			{
				return this._requiredValue;
			}
			set
			{
				if (value != this._requiredValue)
				{
					this._requiredValue = value;
					base.OnPropertyChangedWithValue(value, "TargetValue");
				}
			}
		}

		// Token: 0x17000726 RID: 1830
		// (get) Token: 0x0600152F RID: 5423 RVA: 0x0004F20E File Offset: 0x0004D40E
		// (set) Token: 0x06001530 RID: 5424 RVA: 0x0004F216 File Offset: 0x0004D416
		[DataSourceProperty]
		public string RequiredValueText
		{
			get
			{
				return this._requiredValueText;
			}
			set
			{
				if (value != this._requiredValueText)
				{
					this._requiredValueText = value;
					base.OnPropertyChangedWithValue<string>(value, "RequiredValueText");
				}
			}
		}

		// Token: 0x17000727 RID: 1831
		// (get) Token: 0x06001531 RID: 5425 RVA: 0x0004F239 File Offset: 0x0004D439
		// (set) Token: 0x06001532 RID: 5426 RVA: 0x0004F241 File Offset: 0x0004D441
		[DataSourceProperty]
		public float ChangeAmount
		{
			get
			{
				return this._changeAmount;
			}
			set
			{
				if (this._changeAmount != value)
				{
					this._changeAmount = value;
					base.OnPropertyChangedWithValue(value, "ChangeAmount");
				}
			}
		}

		// Token: 0x17000728 RID: 1832
		// (get) Token: 0x06001533 RID: 5427 RVA: 0x0004F25F File Offset: 0x0004D45F
		// (set) Token: 0x06001534 RID: 5428 RVA: 0x0004F267 File Offset: 0x0004D467
		[DataSourceProperty]
		public bool ShowFloatingPoint
		{
			get
			{
				return this._showFloatingPoint;
			}
			set
			{
				if (this._showFloatingPoint != value)
				{
					this._showFloatingPoint = value;
					base.OnPropertyChangedWithValue(value, "ShowFloatingPoint");
				}
			}
		}

		// Token: 0x17000729 RID: 1833
		// (get) Token: 0x06001535 RID: 5429 RVA: 0x0004F285 File Offset: 0x0004D485
		// (set) Token: 0x06001536 RID: 5430 RVA: 0x0004F28D File Offset: 0x0004D48D
		[DataSourceProperty]
		public bool IsOrderResult
		{
			get
			{
				return this._isOrderResult;
			}
			set
			{
				if (value != this._isOrderResult)
				{
					this._isOrderResult = value;
					base.OnPropertyChangedWithValue(value, "IsOrderResult");
				}
			}
		}

		// Token: 0x1700072A RID: 1834
		// (get) Token: 0x06001537 RID: 5431 RVA: 0x0004F2AB File Offset: 0x0004D4AB
		// (set) Token: 0x06001538 RID: 5432 RVA: 0x0004F2B3 File Offset: 0x0004D4B3
		[DataSourceProperty]
		public bool HasBenefit
		{
			get
			{
				return this._hasBenefit;
			}
			set
			{
				if (value != this._hasBenefit)
				{
					this._hasBenefit = value;
					base.OnPropertyChangedWithValue(value, "HasBenefit");
				}
			}
		}

		// Token: 0x1700072B RID: 1835
		// (get) Token: 0x06001539 RID: 5433 RVA: 0x0004F2D1 File Offset: 0x0004D4D1
		// (set) Token: 0x0600153A RID: 5434 RVA: 0x0004F2D9 File Offset: 0x0004D4D9
		[DataSourceProperty]
		public HintViewModel OrderRequirementTooltip
		{
			get
			{
				return this._orderRequirementTooltip;
			}
			set
			{
				if (value != this._orderRequirementTooltip)
				{
					this._orderRequirementTooltip = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "OrderRequirementTooltip");
				}
			}
		}

		// Token: 0x1700072C RID: 1836
		// (get) Token: 0x0600153B RID: 5435 RVA: 0x0004F2F7 File Offset: 0x0004D4F7
		// (set) Token: 0x0600153C RID: 5436 RVA: 0x0004F2FF File Offset: 0x0004D4FF
		[DataSourceProperty]
		public HintViewModel CraftedValueTooltip
		{
			get
			{
				return this._craftedValueTooltip;
			}
			set
			{
				if (value != this._craftedValueTooltip)
				{
					this._craftedValueTooltip = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "CraftedValueTooltip");
				}
			}
		}

		// Token: 0x1700072D RID: 1837
		// (get) Token: 0x0600153D RID: 5437 RVA: 0x0004F31D File Offset: 0x0004D51D
		// (set) Token: 0x0600153E RID: 5438 RVA: 0x0004F325 File Offset: 0x0004D525
		[DataSourceProperty]
		public HintViewModel BonusPenaltyTooltip
		{
			get
			{
				return this._bonusPenaltyTooltip;
			}
			set
			{
				if (value != this._bonusPenaltyTooltip)
				{
					this._bonusPenaltyTooltip = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "BonusPenaltyTooltip");
				}
			}
		}

		// Token: 0x040009E1 RID: 2529
		private readonly TextObject _description;

		// Token: 0x040009E2 RID: 2530
		private bool _isExceedingBeneficial;

		// Token: 0x040009E3 RID: 2531
		private bool _showTooltip;

		// Token: 0x040009E4 RID: 2532
		private string _propertyLbl;

		// Token: 0x040009E5 RID: 2533
		private float _propertyValue;

		// Token: 0x040009E6 RID: 2534
		private float _requiredValue;

		// Token: 0x040009E7 RID: 2535
		private string _requiredValueText;

		// Token: 0x040009E8 RID: 2536
		private float _changeAmount;

		// Token: 0x040009E9 RID: 2537
		private bool _showFloatingPoint;

		// Token: 0x040009EA RID: 2538
		private bool _isOrderResult;

		// Token: 0x040009EB RID: 2539
		private bool _hasBenefit;

		// Token: 0x040009EC RID: 2540
		private HintViewModel _orderRequirementTooltip;

		// Token: 0x040009ED RID: 2541
		private HintViewModel _craftedValueTooltip;

		// Token: 0x040009EE RID: 2542
		private HintViewModel _bonusPenaltyTooltip;
	}
}
