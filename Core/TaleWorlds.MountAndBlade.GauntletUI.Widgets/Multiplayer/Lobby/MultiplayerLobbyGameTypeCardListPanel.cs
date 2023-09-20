using System;
using System.Collections.Generic;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Lobby
{
	// Token: 0x02000098 RID: 152
	public class MultiplayerLobbyGameTypeCardListPanel : ListPanel
	{
		// Token: 0x06000820 RID: 2080 RVA: 0x00017DCB File Offset: 0x00015FCB
		public MultiplayerLobbyGameTypeCardListPanel(UIContext context)
			: base(context)
		{
			this._cardButtons = new List<MultiplayerLobbyGameTypeCardButtonWidget>();
		}

		// Token: 0x040003BA RID: 954
		private List<MultiplayerLobbyGameTypeCardButtonWidget> _cardButtons;
	}
}
