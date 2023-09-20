using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.ArmyManagement
{
	// Token: 0x02000131 RID: 305
	public class ArmyManagementBoostEventVM : ViewModel
	{
		// Token: 0x17000A0E RID: 2574
		// (get) Token: 0x06001D5C RID: 7516 RVA: 0x00068F8F File Offset: 0x0006718F
		public ArmyManagementBoostEventVM.BoostCurrency CurrencyToPayForCohesion { get; }

		// Token: 0x06001D5D RID: 7517 RVA: 0x00068F97 File Offset: 0x00067197
		public ArmyManagementBoostEventVM(ArmyManagementBoostEventVM.BoostCurrency currencyToPayForCohesion, int amountToPay, int amountOfCohesionToGain, Action<ArmyManagementBoostEventVM> onExecuteEvent)
		{
			this.IsEnabled = true;
			this._onExecuteEvent = onExecuteEvent;
			this.AmountToPay = amountToPay;
			this.AmountOfCohesionToGain = amountOfCohesionToGain;
			this.CurrencyToPayForCohesion = currencyToPayForCohesion;
			this.CurrencyType = (int)currencyToPayForCohesion;
			this.RefreshValues();
		}

		// Token: 0x06001D5E RID: 7518 RVA: 0x00068FD0 File Offset: 0x000671D0
		public override void RefreshValues()
		{
			base.RefreshValues();
			GameTexts.SetVariable("AMOUNT", this.AmountToPay);
			this.SpendText = GameTexts.FindText("str_cohesion_boost_spend", null).ToString();
			GameTexts.SetVariable("GAIN_AMOUNT", this.AmountOfCohesionToGain);
			this.GainText = GameTexts.FindText("str_cohesion_boost_gain", null).ToString();
		}

		// Token: 0x06001D5F RID: 7519 RVA: 0x0006902F File Offset: 0x0006722F
		private void ExecuteEvent()
		{
			this._onExecuteEvent(this);
		}

		// Token: 0x17000A0F RID: 2575
		// (get) Token: 0x06001D60 RID: 7520 RVA: 0x0006903D File Offset: 0x0006723D
		// (set) Token: 0x06001D61 RID: 7521 RVA: 0x00069045 File Offset: 0x00067245
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

		// Token: 0x17000A10 RID: 2576
		// (get) Token: 0x06001D62 RID: 7522 RVA: 0x00069063 File Offset: 0x00067263
		// (set) Token: 0x06001D63 RID: 7523 RVA: 0x0006906B File Offset: 0x0006726B
		[DataSourceProperty]
		public int AmountToPay
		{
			get
			{
				return this._amountToPay;
			}
			set
			{
				if (value != this._amountToPay)
				{
					this._amountToPay = value;
					base.OnPropertyChangedWithValue(value, "AmountToPay");
				}
			}
		}

		// Token: 0x17000A11 RID: 2577
		// (get) Token: 0x06001D64 RID: 7524 RVA: 0x00069089 File Offset: 0x00067289
		// (set) Token: 0x06001D65 RID: 7525 RVA: 0x00069091 File Offset: 0x00067291
		[DataSourceProperty]
		public int CurrencyType
		{
			get
			{
				return this._currencyType;
			}
			set
			{
				if (value != this._currencyType)
				{
					this._currencyType = value;
					base.OnPropertyChangedWithValue(value, "CurrencyType");
				}
			}
		}

		// Token: 0x17000A12 RID: 2578
		// (get) Token: 0x06001D66 RID: 7526 RVA: 0x000690AF File Offset: 0x000672AF
		// (set) Token: 0x06001D67 RID: 7527 RVA: 0x000690B7 File Offset: 0x000672B7
		[DataSourceProperty]
		public int AmountOfCohesionToGain
		{
			get
			{
				return this._amountOfCohesionToGain;
			}
			set
			{
				if (value != this._amountOfCohesionToGain)
				{
					this._amountOfCohesionToGain = value;
					base.OnPropertyChangedWithValue(value, "AmountOfCohesionToGain");
				}
			}
		}

		// Token: 0x17000A13 RID: 2579
		// (get) Token: 0x06001D68 RID: 7528 RVA: 0x000690D5 File Offset: 0x000672D5
		// (set) Token: 0x06001D69 RID: 7529 RVA: 0x000690DD File Offset: 0x000672DD
		[DataSourceProperty]
		public string SpendText
		{
			get
			{
				return this._spendText;
			}
			set
			{
				if (value != this._spendText)
				{
					this._spendText = value;
					base.OnPropertyChangedWithValue<string>(value, "SpendText");
				}
			}
		}

		// Token: 0x17000A14 RID: 2580
		// (get) Token: 0x06001D6A RID: 7530 RVA: 0x00069100 File Offset: 0x00067300
		// (set) Token: 0x06001D6B RID: 7531 RVA: 0x00069108 File Offset: 0x00067308
		[DataSourceProperty]
		public string GainText
		{
			get
			{
				return this._gainText;
			}
			set
			{
				if (value != this._gainText)
				{
					this._gainText = value;
					base.OnPropertyChangedWithValue<string>(value, "GainText");
				}
			}
		}

		// Token: 0x04000DD1 RID: 3537
		private readonly Action<ArmyManagementBoostEventVM> _onExecuteEvent;

		// Token: 0x04000DD2 RID: 3538
		private int _amountToPay;

		// Token: 0x04000DD3 RID: 3539
		private int _amountOfCohesionToGain;

		// Token: 0x04000DD4 RID: 3540
		private int _currencyType;

		// Token: 0x04000DD5 RID: 3541
		private string _spendText;

		// Token: 0x04000DD6 RID: 3542
		private string _gainText;

		// Token: 0x04000DD7 RID: 3543
		private bool _isEnabled;

		// Token: 0x0200027E RID: 638
		public enum BoostCurrency
		{
			// Token: 0x040011BF RID: 4543
			Gold,
			// Token: 0x040011C0 RID: 4544
			Influence
		}
	}
}
