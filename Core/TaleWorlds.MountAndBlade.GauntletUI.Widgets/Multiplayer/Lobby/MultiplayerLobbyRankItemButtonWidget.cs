using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Lobby
{
	public class MultiplayerLobbyRankItemButtonWidget : ButtonWidget
	{
		public MultiplayerLobbyRankItemButtonWidget(UIContext context)
			: base(context)
		{
		}

		private void UpdateSprite()
		{
			string text = "unranked";
			if (this.RankID != string.Empty)
			{
				text = this.RankID;
			}
			base.Brush.DefaultLayer.Sprite = base.Context.SpriteData.GetSprite("MPGeneral\\MPRanks\\" + text);
		}

		[Editor(false)]
		public string RankID
		{
			get
			{
				return this._rankID;
			}
			set
			{
				if (value != this._rankID)
				{
					this._rankID = value;
					base.OnPropertyChanged<string>(value, "RankID");
					this.UpdateSprite();
				}
			}
		}

		private const string _defaultRankID = "unranked";

		private string _rankID;
	}
}
