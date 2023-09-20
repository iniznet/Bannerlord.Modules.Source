using System;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.HostGame.HostGameOptions
{
	public class NumericHostGameOptionDataVM : GenericHostGameOptionDataVM
	{
		public NumericHostGameOptionDataVM(MultiplayerOptions.OptionType optionType, int preferredIndex)
			: base(OptionsVM.OptionsDataType.NumericOption, optionType, preferredIndex)
		{
			this.RefreshData();
		}

		public override void RefreshData()
		{
			MultiplayerOptionsProperty optionProperty = base.OptionType.GetOptionProperty();
			this.Min = optionProperty.BoundsMin;
			this.Max = optionProperty.BoundsMax;
			this.Value = base.OptionType.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
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
					base.OnPropertyChanged("ValueAsString");
					base.OptionType.SetValue(value, MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
				}
			}
		}

		[DataSourceProperty]
		public string ValueAsString
		{
			get
			{
				return this._value.ToString();
			}
		}

		[DataSourceProperty]
		public int Min
		{
			get
			{
				return this._min;
			}
			set
			{
				if (value != this._min)
				{
					this._min = value;
					base.OnPropertyChangedWithValue(value, "Min");
				}
			}
		}

		[DataSourceProperty]
		public int Max
		{
			get
			{
				return this._max;
			}
			set
			{
				if (value != this._max)
				{
					this._max = value;
					base.OnPropertyChangedWithValue(value, "Max");
				}
			}
		}

		private int _value;

		private int _min;

		private int _max;
	}
}
