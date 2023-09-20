using System;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting.WeaponDesign
{
	public class WeaponDesignResultPropertyItemVM : ViewModel
	{
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

		private readonly TextObject _description;

		private bool _isExceedingBeneficial;

		private bool _showTooltip;

		private string _propertyLbl;

		private float _propertyValue;

		private float _requiredValue;

		private string _requiredValueText;

		private float _changeAmount;

		private bool _showFloatingPoint;

		private bool _isOrderResult;

		private bool _hasBenefit;

		private HintViewModel _orderRequirementTooltip;

		private HintViewModel _craftedValueTooltip;

		private HintViewModel _bonusPenaltyTooltip;
	}
}
