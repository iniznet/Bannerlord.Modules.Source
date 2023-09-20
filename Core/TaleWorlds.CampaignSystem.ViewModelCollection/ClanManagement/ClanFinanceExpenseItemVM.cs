using System;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement
{
	public class ClanFinanceExpenseItemVM : ViewModel
	{
		public ClanFinanceExpenseItemVM(MobileParty mobileParty)
		{
			this._mobileParty = mobileParty;
			this.CurrentWageTooltip = new BasicTooltipViewModel(() => CampaignUIHelper.GetPartyWageTooltip(mobileParty));
			this.MinWage = 100;
			this.MaxWage = 2000;
			this.CurrentWage = this._mobileParty.TotalWage;
			this.CurrentWageValueText = this.CurrentWage.ToString();
			this.IsUnlimitedWage = !this._mobileParty.HasLimitedWage();
			this.CurrentWageLimit = ((this._mobileParty.PaymentLimit == Campaign.Current.Models.PartyWageModel.MaxWage) ? 2000 : this._mobileParty.PaymentLimit);
			this.IsEnabled = true;
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.CurrentWageText = new TextObject("{=pnFgwLYG}Current Wage", null).ToString();
			this.CurrentWageLimitText = new TextObject("{=sWWxrafa}Current Limit", null).ToString();
			this.TitleText = new TextObject("{=qdoJOH0j}Party Wage", null).ToString();
			this.UnlimitedWageText = new TextObject("{=WySAapWO}Unlimited Wage", null).ToString();
			this.WageLimitHint = new HintViewModel(new TextObject("{=w0slxNAl}If limit is lower than current wage, party will not recruit troops until wage is reduced to the limit. If limit is higher than current wage, party will keep recruiting.", null), null);
			this.UpdateCurrentWageLimitText();
		}

		private void OnCurrentWageLimitUpdated(int newValue)
		{
			if (!this.IsUnlimitedWage)
			{
				this._mobileParty.SetWagePaymentLimit(newValue);
			}
			this.UpdateCurrentWageLimitText();
		}

		private void OnUnlimitedWageToggled(bool newValue)
		{
			this.CurrentWageLimit = 2000;
			if (newValue)
			{
				this._mobileParty.SetWagePaymentLimit(Campaign.Current.Models.PartyWageModel.MaxWage);
			}
			else
			{
				this._mobileParty.SetWagePaymentLimit(2000);
			}
			this.UpdateCurrentWageLimitText();
		}

		private void UpdateCurrentWageLimitText()
		{
			this.CurrentWageLimitValueText = (this.IsUnlimitedWage ? new TextObject("{=lC5xsoSh}Unlimited", null).ToString() : this.CurrentWageLimit.ToString());
		}

		[DataSourceProperty]
		public bool IsEnabled
		{
			get
			{
				return this._isEnabled;
			}
			set
			{
				if (value != this._isEnabled)
				{
					this._isEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsEnabled");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel WageLimitHint
		{
			get
			{
				return this._wageLimitHint;
			}
			set
			{
				if (value != this._wageLimitHint)
				{
					this._wageLimitHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "WageLimitHint");
				}
			}
		}

		[DataSourceProperty]
		public BasicTooltipViewModel CurrentWageTooltip
		{
			get
			{
				return this._currentWageTooltip;
			}
			set
			{
				if (value != this._currentWageTooltip)
				{
					this._currentWageTooltip = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "CurrentWageTooltip");
				}
			}
		}

		[DataSourceProperty]
		public string CurrentWageText
		{
			get
			{
				return this._currentWageText;
			}
			set
			{
				if (value != this._currentWageText)
				{
					this._currentWageText = value;
					base.OnPropertyChangedWithValue<string>(value, "CurrentWageText");
				}
			}
		}

		[DataSourceProperty]
		public string CurrentWageLimitText
		{
			get
			{
				return this._currentWageLimitText;
			}
			set
			{
				if (value != this._currentWageLimitText)
				{
					this._currentWageLimitText = value;
					base.OnPropertyChangedWithValue<string>(value, "CurrentWageLimitText");
				}
			}
		}

		[DataSourceProperty]
		public string CurrentWageValueText
		{
			get
			{
				return this._currentWageValueText;
			}
			set
			{
				if (value != this._currentWageValueText)
				{
					this._currentWageValueText = value;
					base.OnPropertyChangedWithValue<string>(value, "CurrentWageValueText");
				}
			}
		}

		[DataSourceProperty]
		public string CurrentWageLimitValueText
		{
			get
			{
				return this._currentWageLimitValueText;
			}
			set
			{
				if (value != this._currentWageLimitValueText)
				{
					this._currentWageLimitValueText = value;
					base.OnPropertyChangedWithValue<string>(value, "CurrentWageLimitValueText");
				}
			}
		}

		[DataSourceProperty]
		public string UnlimitedWageText
		{
			get
			{
				return this._unlimitedWageText;
			}
			set
			{
				if (value != this._unlimitedWageText)
				{
					this._unlimitedWageText = value;
					base.OnPropertyChangedWithValue<string>(value, "UnlimitedWageText");
				}
			}
		}

		[DataSourceProperty]
		public string TitleText
		{
			get
			{
				return this._titleText;
			}
			set
			{
				if (value != this._titleText)
				{
					this._titleText = value;
					base.OnPropertyChangedWithValue<string>(value, "TitleText");
				}
			}
		}

		[DataSourceProperty]
		public int CurrentWage
		{
			get
			{
				return this._currentWage;
			}
			set
			{
				if (value != this._currentWage)
				{
					this._currentWage = value;
					base.OnPropertyChangedWithValue(value, "CurrentWage");
				}
			}
		}

		[DataSourceProperty]
		public int CurrentWageLimit
		{
			get
			{
				return this._currentWageLimit;
			}
			set
			{
				if (value != this._currentWageLimit)
				{
					this._currentWageLimit = value;
					base.OnPropertyChangedWithValue(value, "CurrentWageLimit");
					this.OnCurrentWageLimitUpdated(value);
				}
			}
		}

		[DataSourceProperty]
		public int MinWage
		{
			get
			{
				return this._minWage;
			}
			set
			{
				if (value != this._minWage)
				{
					this._minWage = value;
					base.OnPropertyChangedWithValue(value, "MinWage");
				}
			}
		}

		[DataSourceProperty]
		public int MaxWage
		{
			get
			{
				return this._maxWage;
			}
			set
			{
				if (value != this._maxWage)
				{
					this._maxWage = value;
					base.OnPropertyChangedWithValue(value, "MaxWage");
				}
			}
		}

		[DataSourceProperty]
		public bool IsUnlimitedWage
		{
			get
			{
				return this._isUnlimitedWage;
			}
			set
			{
				if (value != this._isUnlimitedWage)
				{
					this._isUnlimitedWage = value;
					base.OnPropertyChangedWithValue(value, "IsUnlimitedWage");
					this.OnUnlimitedWageToggled(value);
				}
			}
		}

		private const int UIWageSliderMaxLimit = 2000;

		private const int UIWageSliderMinLimit = 100;

		private readonly MobileParty _mobileParty;

		private bool _isEnabled;

		private int _minWage;

		private int _maxWage;

		private int _currentWage;

		private int _currentWageLimit;

		private string _currentWageText;

		private string _currentWageLimitText;

		private string _currentWageValueText;

		private string _currentWageLimitValueText;

		private string _unlimitedWageText;

		private string _titleText;

		private bool _isUnlimitedWage;

		private HintViewModel _wageLimitHint;

		private BasicTooltipViewModel _currentWageTooltip;
	}
}
