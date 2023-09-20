using System;
using Helpers;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu.TownManagement
{
	// Token: 0x02000093 RID: 147
	public class TownManagementReserveControlVM : ViewModel
	{
		// Token: 0x06000E3B RID: 3643 RVA: 0x00038C58 File Offset: 0x00036E58
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

		// Token: 0x06000E3C RID: 3644 RVA: 0x00038CC4 File Offset: 0x00036EC4
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

		// Token: 0x06000E3D RID: 3645 RVA: 0x00038D18 File Offset: 0x00036F18
		private void UpdateReserveText()
		{
			TextObject textObject = GameTexts.FindText("str_town_management_reserve_explanation", null);
			textObject.SetTextVariable("BOOST", Campaign.Current.Models.BuildingConstructionModel.GetBoostAmount(this._settlement.Town));
			textObject.SetTextVariable("COST", Campaign.Current.Models.BuildingConstructionModel.GetBoostCost(this._settlement.Town));
			this.ReserveBonusText = textObject.ToString();
		}

		// Token: 0x06000E3E RID: 3646 RVA: 0x00038D94 File Offset: 0x00036F94
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

		// Token: 0x170004A0 RID: 1184
		// (get) Token: 0x06000E3F RID: 3647 RVA: 0x00038E1F File Offset: 0x0003701F
		// (set) Token: 0x06000E40 RID: 3648 RVA: 0x00038E27 File Offset: 0x00037027
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

		// Token: 0x170004A1 RID: 1185
		// (get) Token: 0x06000E41 RID: 3649 RVA: 0x00038E45 File Offset: 0x00037045
		// (set) Token: 0x06000E42 RID: 3650 RVA: 0x00038E50 File Offset: 0x00037050
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

		// Token: 0x170004A2 RID: 1186
		// (get) Token: 0x06000E43 RID: 3651 RVA: 0x00038E8F File Offset: 0x0003708F
		// (set) Token: 0x06000E44 RID: 3652 RVA: 0x00038E97 File Offset: 0x00037097
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

		// Token: 0x170004A3 RID: 1187
		// (get) Token: 0x06000E45 RID: 3653 RVA: 0x00038EB5 File Offset: 0x000370B5
		// (set) Token: 0x06000E46 RID: 3654 RVA: 0x00038EBD File Offset: 0x000370BD
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

		// Token: 0x170004A4 RID: 1188
		// (get) Token: 0x06000E47 RID: 3655 RVA: 0x00038EDB File Offset: 0x000370DB
		// (set) Token: 0x06000E48 RID: 3656 RVA: 0x00038EE3 File Offset: 0x000370E3
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

		// Token: 0x170004A5 RID: 1189
		// (get) Token: 0x06000E49 RID: 3657 RVA: 0x00038F06 File Offset: 0x00037106
		// (set) Token: 0x06000E4A RID: 3658 RVA: 0x00038F0E File Offset: 0x0003710E
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

		// Token: 0x170004A6 RID: 1190
		// (get) Token: 0x06000E4B RID: 3659 RVA: 0x00038F31 File Offset: 0x00037131
		// (set) Token: 0x06000E4C RID: 3660 RVA: 0x00038F39 File Offset: 0x00037139
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

		// Token: 0x0400069C RID: 1692
		private readonly Action _onReserveUpdated;

		// Token: 0x0400069D RID: 1693
		private readonly Settlement _settlement;

		// Token: 0x0400069E RID: 1694
		private const int MaxOneTimeAmount = 10000;

		// Token: 0x0400069F RID: 1695
		private bool _isEnabled;

		// Token: 0x040006A0 RID: 1696
		private string _reserveText;

		// Token: 0x040006A1 RID: 1697
		private int _currentReserveAmount;

		// Token: 0x040006A2 RID: 1698
		private int _currentGivenAmount;

		// Token: 0x040006A3 RID: 1699
		private int _maxReserveAmount;

		// Token: 0x040006A4 RID: 1700
		private string _reserveBonusText;

		// Token: 0x040006A5 RID: 1701
		private string _currentReserveText;
	}
}
