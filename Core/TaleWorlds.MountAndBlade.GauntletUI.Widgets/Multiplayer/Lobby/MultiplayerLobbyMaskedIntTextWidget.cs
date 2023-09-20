using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Lobby
{
	public class MultiplayerLobbyMaskedIntTextWidget : TextWidget
	{
		public MultiplayerLobbyMaskedIntTextWidget(UIContext context)
			: base(context)
		{
		}

		private void IntValueUpdated()
		{
			if (this.IntValue == this.MaskedIntValue)
			{
				base.Text = this.MaskText;
				return;
			}
			base.IntText = this.IntValue;
		}

		[Editor(false)]
		public int IntValue
		{
			get
			{
				return this._intValue;
			}
			set
			{
				if (this._intValue != value)
				{
					this._intValue = value;
					base.OnPropertyChanged(value, "IntValue");
					this.IntValueUpdated();
				}
			}
		}

		[Editor(false)]
		public int MaskedIntValue
		{
			get
			{
				return this._maskedIntValue;
			}
			set
			{
				if (this._maskedIntValue != value)
				{
					this._maskedIntValue = value;
					base.OnPropertyChanged(value, "MaskedIntValue");
				}
			}
		}

		[Editor(false)]
		public string MaskText
		{
			get
			{
				return this._maskText;
			}
			set
			{
				if (this._maskText != value)
				{
					this._maskText = value;
					base.OnPropertyChanged<string>(value, "MaskText");
				}
			}
		}

		private int _intValue;

		private int _maskedIntValue;

		private string _maskText;
	}
}
