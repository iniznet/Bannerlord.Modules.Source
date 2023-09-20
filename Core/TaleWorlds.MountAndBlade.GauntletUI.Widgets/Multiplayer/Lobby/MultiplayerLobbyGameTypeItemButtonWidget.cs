using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Lobby
{
	public class MultiplayerLobbyGameTypeItemButtonWidget : ButtonWidget
	{
		public MultiplayerLobbyGameTypeItemButtonWidget(UIContext context)
			: base(context)
		{
		}

		private void UpdateSprite()
		{
			base.Brush.DefaultLayer.Sprite = base.Context.SpriteData.GetSprite("MPLobby\\GameTypes\\" + this.GameTypeID);
		}

		[Editor(false)]
		public string GameTypeID
		{
			get
			{
				return this._gameTypeID;
			}
			set
			{
				if (value != this._gameTypeID)
				{
					this._gameTypeID = value;
					base.OnPropertyChanged<string>(value, "GameTypeID");
					this.UpdateSprite();
				}
			}
		}

		private string _gameTypeID;
	}
}
