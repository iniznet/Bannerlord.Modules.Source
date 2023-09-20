using System;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement
{
	public class ClanCardSelectionPopupItemPropertyVM : ViewModel
	{
		public ClanCardSelectionPopupItemPropertyVM(in ClanCardSelectionItemPropertyInfo info)
		{
			this._titleText = info.Title;
			this._valueText = info.Value;
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			TextObject titleText = this._titleText;
			this.Title = ((titleText != null) ? titleText.ToString() : null) ?? string.Empty;
			TextObject valueText = this._valueText;
			this.Value = ((valueText != null) ? valueText.ToString() : null) ?? string.Empty;
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
		public string Value
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
					base.OnPropertyChangedWithValue<string>(value, "Value");
				}
			}
		}

		private readonly TextObject _titleText;

		private readonly TextObject _valueText;

		private string _title;

		private string _value;
	}
}
