using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Friends
{
	public class MPLobbyPlayerTroopClassVM : ViewModel
	{
		public MPLobbyPlayerTroopClassVM()
		{
			this.Name = "Varangian Guard";
			this.Preview = new ImageIdentifierVM(ImageIdentifierType.Null);
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
		public ImageIdentifierVM Preview
		{
			get
			{
				return this._preview;
			}
			set
			{
				if (value != this._preview)
				{
					this._preview = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "Preview");
				}
			}
		}

		private string _name;

		private ImageIdentifierVM _preview;
	}
}
