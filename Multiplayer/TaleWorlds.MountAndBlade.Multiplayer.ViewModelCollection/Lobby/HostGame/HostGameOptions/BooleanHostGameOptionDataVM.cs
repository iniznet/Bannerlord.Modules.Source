using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Lobby.HostGame.HostGameOptions
{
	public class BooleanHostGameOptionDataVM : GenericHostGameOptionDataVM
	{
		public BooleanHostGameOptionDataVM(MultiplayerOptions.OptionType optionType, int preferredIndex)
			: base(0, optionType, preferredIndex)
		{
			this.RefreshData();
		}

		public override void RefreshData()
		{
			this.IsSelected = MultiplayerOptionsExtensions.GetBoolValue(base.OptionType, 0);
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
					MultiplayerOptionsExtensions.SetValue(base.OptionType, value, 0);
				}
			}
		}

		private bool _isSelected;
	}
}
