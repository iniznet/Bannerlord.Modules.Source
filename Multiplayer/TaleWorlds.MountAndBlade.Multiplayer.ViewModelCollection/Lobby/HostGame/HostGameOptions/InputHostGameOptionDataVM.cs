using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Lobby.HostGame.HostGameOptions
{
	public class InputHostGameOptionDataVM : GenericHostGameOptionDataVM
	{
		public InputHostGameOptionDataVM(MultiplayerOptions.OptionType optionType, int preferredIndex)
			: base(4, optionType, preferredIndex)
		{
			this.RefreshData();
		}

		public override void RefreshData()
		{
			string strValue = MultiplayerOptionsExtensions.GetStrValue(base.OptionType, 0);
			this.Text = strValue;
		}

		[DataSourceProperty]
		public string Text
		{
			get
			{
				return this._text;
			}
			set
			{
				if (value != this._text)
				{
					this._text = value;
					base.OnPropertyChangedWithValue<string>(value, "Text");
					MultiplayerOptionsExtensions.SetValue(base.OptionType, value, 0);
				}
			}
		}

		private string _text;
	}
}
