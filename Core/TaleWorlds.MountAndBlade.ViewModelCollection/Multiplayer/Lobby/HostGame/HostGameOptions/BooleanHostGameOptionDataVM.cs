using System;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.HostGame.HostGameOptions
{
	public class BooleanHostGameOptionDataVM : GenericHostGameOptionDataVM
	{
		public BooleanHostGameOptionDataVM(MultiplayerOptions.OptionType optionType, int preferredIndex)
			: base(OptionsVM.OptionsDataType.BooleanOption, optionType, preferredIndex)
		{
			this.RefreshData();
		}

		public override void RefreshData()
		{
			this.IsSelected = base.OptionType.GetBoolValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
		}

		[DataSourceProperty]
		public bool IsSelected
		{
			get
			{
				return this._isSelected;
			}
			set
			{
				if (value != this._isSelected)
				{
					this._isSelected = value;
					base.OnPropertyChangedWithValue(value, "IsSelected");
					base.OptionType.SetValue(value, MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
				}
			}
		}

		private bool _isSelected;
	}
}
