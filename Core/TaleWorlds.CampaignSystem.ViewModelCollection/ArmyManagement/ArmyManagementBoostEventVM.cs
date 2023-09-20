using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.ArmyManagement
{
	public class ArmyManagementBoostEventVM : ViewModel
	{
		public ArmyManagementBoostEventVM.BoostCurrency CurrencyToPayForCohesion { get; }

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

		public override void RefreshValues()
		{
			base.RefreshValues();
			GameTexts.SetVariable("AMOUNT", this.AmountToPay);
			this.SpendText = GameTexts.FindText("str_cohesion_boost_spend", null).ToString();
			GameTexts.SetVariable("GAIN_AMOUNT", this.AmountOfCohesionToGain);
			this.GainText = GameTexts.FindText("str_cohesion_boost_gain", null).ToString();
		}

		private void ExecuteEvent()
		{
			this._onExecuteEvent(this);
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

		private readonly Action<ArmyManagementBoostEventVM> _onExecuteEvent;

		private int _amountToPay;

		private int _amountOfCohesionToGain;

		private int _currencyType;

		private string _spendText;

		private string _gainText;

		private bool _isEnabled;

		public enum BoostCurrency
		{
			Gold,
			Influence
		}
	}
}
