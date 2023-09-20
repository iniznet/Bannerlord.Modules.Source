using System;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu.TownManagement
{
	public class TownManagementDescriptionItemVM : ViewModel
	{
		public TownManagementDescriptionItemVM(TextObject title, int value, int valueChange, TownManagementDescriptionItemVM.DescriptionType type, BasicTooltipViewModel hint = null)
		{
			this._titleObj = title;
			this.Value = value;
			this.ValueChange = valueChange;
			this.Type = (int)type;
			this.Hint = hint ?? new BasicTooltipViewModel();
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Title = this._titleObj.ToString();
			this.RefreshIsWarning();
		}

		private void RefreshIsWarning()
		{
			int type = this.Type;
			if (type == 1)
			{
				this.IsWarning = this.Value < 1;
				return;
			}
			if (type == 5)
			{
				this.IsWarning = this.Value < Campaign.Current.Models.SettlementLoyaltyModel.RebelliousStateStartLoyaltyThreshold;
				return;
			}
			if (type != 7)
			{
				this.IsWarning = false;
				return;
			}
			this.IsWarning = this.Value < 1;
		}

		[DataSourceProperty]
		public int Type
		{
			get
			{
				return this._type;
			}
			set
			{
				if (value != this._type)
				{
					this._type = value;
					base.OnPropertyChangedWithValue(value, "Type");
				}
			}
		}

		[DataSourceProperty]
		public string Title
		{
			get
			{
				return this._title;
			}
			set
			{
				if (value != this._title)
				{
					this._title = value;
					base.OnPropertyChangedWithValue<string>(value, "Title");
				}
			}
		}

		[DataSourceProperty]
		public int Value
		{
			get
			{
				return this._value;
			}
			set
			{
				if (value != this._value)
				{
					this._value = value;
					base.OnPropertyChangedWithValue(value, "Value");
				}
			}
		}

		[DataSourceProperty]
		public int ValueChange
		{
			get
			{
				return this._valueChange;
			}
			set
			{
				if (value != this._valueChange)
				{
					this._valueChange = value;
					base.OnPropertyChangedWithValue(value, "ValueChange");
				}
			}
		}

		[DataSourceProperty]
		public BasicTooltipViewModel Hint
		{
			get
			{
				return this._hint;
			}
			set
			{
				if (value != this._hint && value != null)
				{
					this._hint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "Hint");
				}
			}
		}

		[DataSourceProperty]
		public bool IsWarning
		{
			get
			{
				return this._isWarning;
			}
			set
			{
				if (value != this._isWarning)
				{
					this._isWarning = value;
					base.OnPropertyChangedWithValue(value, "IsWarning");
				}
			}
		}

		private readonly TextObject _titleObj;

		private int _type = -1;

		private string _title;

		private int _value;

		private int _valueChange;

		private BasicTooltipViewModel _hint;

		private bool _isWarning;

		public enum DescriptionType
		{
			Gold,
			Production,
			Militia,
			Prosperity,
			Food,
			Loyalty,
			Security,
			Garrison
		}
	}
}
