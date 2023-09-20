using System;
using TaleWorlds.CampaignSystem.Settlements.Buildings;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu.TownManagement
{
	public class SettlementDailyProjectVM : SettlementProjectVM
	{
		public SettlementDailyProjectVM(Action<SettlementProjectVM, bool> onSelection, Action<SettlementProjectVM> onSetAsCurrent, Action onResetCurrent, Building building)
			: base(onSelection, onSetAsCurrent, onResetCurrent, building)
		{
			base.IsDaily = true;
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.DefaultText = GameTexts.FindText("str_default", null).ToString();
		}

		public override void RefreshProductionText()
		{
			base.RefreshProductionText();
			base.ProductionText = new TextObject("{=bd7oAQq6}Daily", null).ToString();
		}

		public override void ExecuteAddToQueue()
		{
		}

		public override void ExecuteSetAsActiveDevelopment()
		{
			this._onSelection(this, false);
		}

		public override void ExecuteSetAsCurrent()
		{
			Action<SettlementProjectVM> onSetAsCurrent = this._onSetAsCurrent;
			if (onSetAsCurrent == null)
			{
				return;
			}
			onSetAsCurrent(this);
		}

		public override void ExecuteResetCurrent()
		{
			Action onResetCurrent = this._onResetCurrent;
			if (onResetCurrent == null)
			{
				return;
			}
			onResetCurrent();
		}

		[DataSourceProperty]
		public bool IsDefault
		{
			get
			{
				return this._isDefault;
			}
			set
			{
				if (value != this._isDefault)
				{
					this._isDefault = value;
					base.OnPropertyChangedWithValue(value, "IsDefault");
				}
			}
		}

		[DataSourceProperty]
		public string DefaultText
		{
			get
			{
				return this._defaultText;
			}
			set
			{
				if (value != this._defaultText)
				{
					this._defaultText = value;
					base.OnPropertyChangedWithValue<string>(value, "DefaultText");
				}
			}
		}

		private bool _isDefault;

		private string _defaultText;
	}
}
