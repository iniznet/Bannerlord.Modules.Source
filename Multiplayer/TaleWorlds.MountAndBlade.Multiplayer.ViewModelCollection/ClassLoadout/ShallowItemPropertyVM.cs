using System;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.ClassLoadout
{
	public class ShallowItemPropertyVM : ViewModel
	{
		public ShallowItemPropertyVM(TextObject propertyName, int permille, int value)
		{
			this._propertyName = propertyName;
			this.Permille = permille;
			this.Value = value;
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.NameText = this._propertyName.ToString();
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
		public string NameText
		{
			get
			{
				return this._nameText;
			}
			set
			{
				if (value != this._nameText)
				{
					this._nameText = value;
					base.OnPropertyChangedWithValue<string>(value, "NameText");
				}
			}
		}

		[DataSourceProperty]
		public int Permille
		{
			get
			{
				return this._permille;
			}
			set
			{
				if (value != this._permille)
				{
					this._permille = value;
					base.OnPropertyChangedWithValue(value, "Permille");
				}
			}
		}

		private readonly TextObject _propertyName;

		private string _nameText;

		private int _permille;

		private int _value;
	}
}
