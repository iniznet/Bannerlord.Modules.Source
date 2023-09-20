using System;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement
{
	// Token: 0x020000FE RID: 254
	public class ClanFinanceExpenseItemVM : ViewModel
	{
		// Token: 0x06001793 RID: 6035 RVA: 0x00056EA4 File Offset: 0x000550A4
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

		// Token: 0x06001794 RID: 6036 RVA: 0x00056F7C File Offset: 0x0005517C
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

		// Token: 0x06001795 RID: 6037 RVA: 0x00057004 File Offset: 0x00055204
		private void OnCurrentWageLimitUpdated(int newValue)
		{
			if (!this.IsUnlimitedWage)
			{
				this._mobileParty.SetWagePaymentLimit(newValue);
			}
			this.UpdateCurrentWageLimitText();
		}

		// Token: 0x06001796 RID: 6038 RVA: 0x00057020 File Offset: 0x00055220
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

		// Token: 0x06001797 RID: 6039 RVA: 0x00057074 File Offset: 0x00055274
		private void UpdateCurrentWageLimitText()
		{
			this.CurrentWageLimitValueText = (this.IsUnlimitedWage ? new TextObject("{=lC5xsoSh}Unlimited", null).ToString() : this.CurrentWageLimit.ToString());
		}

		// Token: 0x170007FE RID: 2046
		// (get) Token: 0x06001798 RID: 6040 RVA: 0x000570AF File Offset: 0x000552AF
		// (set) Token: 0x06001799 RID: 6041 RVA: 0x000570B7 File Offset: 0x000552B7
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

		// Token: 0x170007FF RID: 2047
		// (get) Token: 0x0600179A RID: 6042 RVA: 0x000570D5 File Offset: 0x000552D5
		// (set) Token: 0x0600179B RID: 6043 RVA: 0x000570DD File Offset: 0x000552DD
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

		// Token: 0x17000800 RID: 2048
		// (get) Token: 0x0600179C RID: 6044 RVA: 0x000570FB File Offset: 0x000552FB
		// (set) Token: 0x0600179D RID: 6045 RVA: 0x00057103 File Offset: 0x00055303
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

		// Token: 0x17000801 RID: 2049
		// (get) Token: 0x0600179E RID: 6046 RVA: 0x00057121 File Offset: 0x00055321
		// (set) Token: 0x0600179F RID: 6047 RVA: 0x00057129 File Offset: 0x00055329
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

		// Token: 0x17000802 RID: 2050
		// (get) Token: 0x060017A0 RID: 6048 RVA: 0x0005714C File Offset: 0x0005534C
		// (set) Token: 0x060017A1 RID: 6049 RVA: 0x00057154 File Offset: 0x00055354
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

		// Token: 0x17000803 RID: 2051
		// (get) Token: 0x060017A2 RID: 6050 RVA: 0x00057177 File Offset: 0x00055377
		// (set) Token: 0x060017A3 RID: 6051 RVA: 0x0005717F File Offset: 0x0005537F
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

		// Token: 0x17000804 RID: 2052
		// (get) Token: 0x060017A4 RID: 6052 RVA: 0x000571A2 File Offset: 0x000553A2
		// (set) Token: 0x060017A5 RID: 6053 RVA: 0x000571AA File Offset: 0x000553AA
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

		// Token: 0x17000805 RID: 2053
		// (get) Token: 0x060017A6 RID: 6054 RVA: 0x000571CD File Offset: 0x000553CD
		// (set) Token: 0x060017A7 RID: 6055 RVA: 0x000571D5 File Offset: 0x000553D5
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

		// Token: 0x17000806 RID: 2054
		// (get) Token: 0x060017A8 RID: 6056 RVA: 0x000571F8 File Offset: 0x000553F8
		// (set) Token: 0x060017A9 RID: 6057 RVA: 0x00057200 File Offset: 0x00055400
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

		// Token: 0x17000807 RID: 2055
		// (get) Token: 0x060017AA RID: 6058 RVA: 0x00057223 File Offset: 0x00055423
		// (set) Token: 0x060017AB RID: 6059 RVA: 0x0005722B File Offset: 0x0005542B
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

		// Token: 0x17000808 RID: 2056
		// (get) Token: 0x060017AC RID: 6060 RVA: 0x00057249 File Offset: 0x00055449
		// (set) Token: 0x060017AD RID: 6061 RVA: 0x00057251 File Offset: 0x00055451
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

		// Token: 0x17000809 RID: 2057
		// (get) Token: 0x060017AE RID: 6062 RVA: 0x00057276 File Offset: 0x00055476
		// (set) Token: 0x060017AF RID: 6063 RVA: 0x0005727E File Offset: 0x0005547E
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

		// Token: 0x1700080A RID: 2058
		// (get) Token: 0x060017B0 RID: 6064 RVA: 0x0005729C File Offset: 0x0005549C
		// (set) Token: 0x060017B1 RID: 6065 RVA: 0x000572A4 File Offset: 0x000554A4
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

		// Token: 0x1700080B RID: 2059
		// (get) Token: 0x060017B2 RID: 6066 RVA: 0x000572C2 File Offset: 0x000554C2
		// (set) Token: 0x060017B3 RID: 6067 RVA: 0x000572CA File Offset: 0x000554CA
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

		// Token: 0x04000B26 RID: 2854
		private const int UIWageSliderMaxLimit = 2000;

		// Token: 0x04000B27 RID: 2855
		private const int UIWageSliderMinLimit = 100;

		// Token: 0x04000B28 RID: 2856
		private readonly MobileParty _mobileParty;

		// Token: 0x04000B29 RID: 2857
		private bool _isEnabled;

		// Token: 0x04000B2A RID: 2858
		private int _minWage;

		// Token: 0x04000B2B RID: 2859
		private int _maxWage;

		// Token: 0x04000B2C RID: 2860
		private int _currentWage;

		// Token: 0x04000B2D RID: 2861
		private int _currentWageLimit;

		// Token: 0x04000B2E RID: 2862
		private string _currentWageText;

		// Token: 0x04000B2F RID: 2863
		private string _currentWageLimitText;

		// Token: 0x04000B30 RID: 2864
		private string _currentWageValueText;

		// Token: 0x04000B31 RID: 2865
		private string _currentWageLimitValueText;

		// Token: 0x04000B32 RID: 2866
		private string _unlimitedWageText;

		// Token: 0x04000B33 RID: 2867
		private string _titleText;

		// Token: 0x04000B34 RID: 2868
		private bool _isUnlimitedWage;

		// Token: 0x04000B35 RID: 2869
		private HintViewModel _wageLimitHint;

		// Token: 0x04000B36 RID: 2870
		private BasicTooltipViewModel _currentWageTooltip;
	}
}
