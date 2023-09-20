using System;
using Helpers;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu.TownManagement
{
	public class TownManagementReserveControlVM : ViewModel
	{
		public TownManagementReserveControlVM(Settlement settlement, Action onReserveUpdated)
		{
			this._settlement = settlement;
			this._onReserveUpdated = onReserveUpdated;
			if (((settlement != null) ? settlement.Town : null) != null)
			{
				this.CurrentReserveAmount = Settlement.CurrentSettlement.Town.BoostBuildingProcess;
				this.CurrentGivenAmount = 0;
				this.MaxReserveAmount = MathF.Min(Hero.MainHero.Gold, 10000);
			}
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			Settlement settlement = this._settlement;
			if (((settlement != null) ? settlement.Town : null) != null)
			{
				this.ReserveText = new TextObject("{=2ckyCKR7}Reserve", null).ToString();
				GameTexts.SetVariable("GOLD_ICON", "{=!}<img src=\"General\\Icons\\Coin@2x\" extend=\"8\">");
				this.UpdateReserveText();
			}
		}

		private void UpdateReserveText()
		{
			TextObject textObject = GameTexts.FindText("str_town_management_reserve_explanation", null);
			textObject.SetTextVariable("BOOST", Campaign.Current.Models.BuildingConstructionModel.GetBoostAmount(this._settlement.Town));
			textObject.SetTextVariable("COST", Campaign.Current.Models.BuildingConstructionModel.GetBoostCost(this._settlement.Town));
			this.ReserveBonusText = textObject.ToString();
		}

		public void ExecuteUpdateReserve()
		{
			this.IsEnabled = false;
			BuildingHelper.BoostBuildingProcessWithGold(this.CurrentReserveAmount + this.CurrentGivenAmount, Settlement.CurrentSettlement.Town);
			this.CurrentGivenAmount = 0;
			GameTexts.SetVariable("GOLD_ICON", "{=!}<img src=\"General\\Icons\\Coin@2x\" extend=\"8\">");
			this.UpdateReserveText();
			this.MaxReserveAmount = MathF.Min(Hero.MainHero.Gold, 10000);
			this.CurrentReserveAmount = Settlement.CurrentSettlement.Town.BoostBuildingProcess;
			Action onReserveUpdated = this._onReserveUpdated;
			if (onReserveUpdated == null)
			{
				return;
			}
			onReserveUpdated();
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
		public int CurrentReserveAmount
		{
			get
			{
				return this._currentReserveAmount;
			}
			set
			{
				if (value != this._currentReserveAmount)
				{
					this._currentReserveAmount = value;
					base.OnPropertyChangedWithValue(value, "CurrentReserveAmount");
					this.CurrentReserveText = (this.CurrentGivenAmount + value).ToString();
				}
			}
		}

		[DataSourceProperty]
		public int CurrentGivenAmount
		{
			get
			{
				return this._currentGivenAmount;
			}
			set
			{
				if (value != this._currentGivenAmount)
				{
					this._currentGivenAmount = value;
					base.OnPropertyChangedWithValue(value, "CurrentGivenAmount");
				}
			}
		}

		[DataSourceProperty]
		public int MaxReserveAmount
		{
			get
			{
				return this._maxReserveAmount;
			}
			set
			{
				if (value != this._maxReserveAmount)
				{
					this._maxReserveAmount = value;
					base.OnPropertyChangedWithValue(value, "MaxReserveAmount");
				}
			}
		}

		[DataSourceProperty]
		public string ReserveBonusText
		{
			get
			{
				return this._reserveBonusText;
			}
			set
			{
				if (value != this._reserveBonusText)
				{
					this._reserveBonusText = value;
					base.OnPropertyChangedWithValue<string>(value, "ReserveBonusText");
				}
			}
		}

		[DataSourceProperty]
		public string ReserveText
		{
			get
			{
				return this._reserveText;
			}
			set
			{
				if (value != this._reserveText)
				{
					this._reserveText = value;
					base.OnPropertyChangedWithValue<string>(value, "ReserveText");
				}
			}
		}

		[DataSourceProperty]
		public string CurrentReserveText
		{
			get
			{
				return this._currentReserveText;
			}
			set
			{
				if (value != this._currentReserveText)
				{
					this._currentReserveText = value;
					base.OnPropertyChangedWithValue<string>(value, "CurrentReserveText");
				}
			}
		}

		private readonly Action _onReserveUpdated;

		private readonly Settlement _settlement;

		private const int MaxOneTimeAmount = 10000;

		private bool _isEnabled;

		private string _reserveText;

		private int _currentReserveAmount;

		private int _currentGivenAmount;

		private int _maxReserveAmount;

		private string _reserveBonusText;

		private string _currentReserveText;
	}
}
