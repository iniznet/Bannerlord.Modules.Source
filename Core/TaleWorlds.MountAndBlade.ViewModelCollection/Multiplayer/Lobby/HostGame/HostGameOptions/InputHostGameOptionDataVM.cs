using System;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.HostGame.HostGameOptions
{
	public class InputHostGameOptionDataVM : GenericHostGameOptionDataVM
	{
		public InputHostGameOptionDataVM(MultiplayerOptions.OptionType optionType, int preferredIndex)
			: base(OptionsVM.OptionsDataType.InputOption, optionType, preferredIndex)
		{
			this.RefreshData();
		}

		public override void RefreshData()
		{
			string strValue = base.OptionType.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
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
					base.OptionType.SetValue(value, MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
				}
			}
		}

		private string _text;
	}
}
