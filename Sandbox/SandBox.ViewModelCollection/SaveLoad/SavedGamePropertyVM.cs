using System;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace SandBox.ViewModelCollection.SaveLoad
{
	public class SavedGamePropertyVM : ViewModel
	{
		public SavedGamePropertyVM(SavedGamePropertyVM.SavedGameProperty type, TextObject value, TextObject hint)
		{
			this.PropertyType = type.ToString();
			this._valueText = value;
			this.Hint = new HintViewModel(hint, null);
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Value = this._valueText.ToString();
		}

		[DataSourceProperty]
		public HintViewModel Hint
		{
			get
			{
				return this._hint;
			}
			set
			{
				if (value != this._hint)
				{
					this._hint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "Hint");
				}
			}
		}

		[DataSourceProperty]
		public string PropertyType
		{
			get
			{
				return this._propertyType;
			}
			set
			{
				if (value != this._propertyType)
				{
					this._propertyType = value;
					base.OnPropertyChangedWithValue<string>(value, "PropertyType");
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

		private TextObject _valueText = TextObject.Empty;

		private HintViewModel _hint;

		private string _propertyType;

		private string _value;

		public enum SavedGameProperty
		{
			None = -1,
			Health,
			Gold,
			Influence,
			PartySize,
			Food,
			Fiefs
		}
	}
}
