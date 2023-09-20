using System;
using System.Collections.Generic;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Lobby
{
	public class MultiplayerLobbyGameTypeCardListPanel : ListPanel
	{
		public MultiplayerLobbyGameTypeCardListPanel(UIContext context)
			: base(context)
		{
			this._cardButtons = new List<MultiplayerLobbyGameTypeCardButtonWidget>();
		}

		private List<MultiplayerLobbyGameTypeCardButtonWidget> _cardButtons;
	}
}
