using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions;

namespace TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Lobby.HostGame.HostGameOptions
{
	public abstract class GenericHostGameOptionDataVM : ViewModel
	{
		public MultiplayerOptions.OptionType OptionType { get; }

		public int PreferredIndex { get; }

		internal GenericHostGameOptionDataVM(OptionsVM.OptionsDataType type, MultiplayerOptions.OptionType optionType, int preferredIndex)
		{
			this.Category = type;
			this.OptionType = optionType;
			this.PreferredIndex = preferredIndex;
			this.Index = preferredIndex;
			this.IsEnabled = true;
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Name = GameTexts.FindText("str_multiplayer_option", this.OptionType.ToString()).ToString();
		}

		public abstract void RefreshData();

		[DataSourceProperty]
		public int Index
		{
			get
			{
				return this._index;
			}
			set
			{
				if (value != this._index)
				{
					this._index = value;
					base.OnPropertyChangedWithValue(value, "Index");
				}
			}
		}

		[DataSourceProperty]
		public int Category
		{
			get
			{
				return this._category;
			}
			set
			{
				if (value != this._category)
				{
					this._category = value;
					base.OnPropertyChangedWithValue(value, "Category");
				}
			}
		}

		[DataSourceProperty]
		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				if (value != this._name)
				{
					this._name = value;
					base.OnPropertyChangedWithValue<string>(value, "Name");
				}
			}
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

		private int _index;

		private int _category;

		private string _name;

		private bool _isEnabled;
	}
}
