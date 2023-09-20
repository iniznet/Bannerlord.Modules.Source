using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Lobby
{
	// Token: 0x0200009C RID: 156
	public class MultiplayerLobbyMenuWidget : Widget
	{
		// Token: 0x06000836 RID: 2102 RVA: 0x00018022 File Offset: 0x00016222
		public MultiplayerLobbyMenuWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000837 RID: 2103 RVA: 0x0001802B File Offset: 0x0001622B
		public void LobbyStateChanged(bool isSearchRequested, bool isSearching, bool isMatchmakingEnabled, bool isCustomBattleEnabled, bool isPartyLeader, bool isInParty)
		{
			this.MatchmakingButtonWidget.IsEnabled = isMatchmakingEnabled || isCustomBattleEnabled;
		}

		// Token: 0x06000838 RID: 2104 RVA: 0x0001803C File Offset: 0x0001623C
		private void SelectedItemIndexChanged()
		{
			if (this.MenuItemListPanel == null)
			{
				return;
			}
			this.MenuItemListPanel.IntValue = this.SelectedItemIndex - 3;
		}

		// Token: 0x170002E1 RID: 737
		// (get) Token: 0x06000839 RID: 2105 RVA: 0x0001805A File Offset: 0x0001625A
		// (set) Token: 0x0600083A RID: 2106 RVA: 0x00018062 File Offset: 0x00016262
		[Editor(false)]
		public int SelectedItemIndex
		{
			get
			{
				return this._selectedItemIndex;
			}
			set
			{
				if (this._selectedItemIndex != value)
				{
					this._selectedItemIndex = value;
					base.OnPropertyChanged(value, "SelectedItemIndex");
					this.SelectedItemIndexChanged();
				}
			}
		}

		// Token: 0x170002E2 RID: 738
		// (get) Token: 0x0600083B RID: 2107 RVA: 0x00018086 File Offset: 0x00016286
		// (set) Token: 0x0600083C RID: 2108 RVA: 0x0001808E File Offset: 0x0001628E
		[Editor(false)]
		public ListPanel MenuItemListPanel
		{
			get
			{
				return this._menuItemListPanel;
			}
			set
			{
				if (this._menuItemListPanel != value)
				{
					this._menuItemListPanel = value;
					base.OnPropertyChanged<ListPanel>(value, "MenuItemListPanel");
					this.SelectedItemIndexChanged();
				}
			}
		}

		// Token: 0x170002E3 RID: 739
		// (get) Token: 0x0600083D RID: 2109 RVA: 0x000180B2 File Offset: 0x000162B2
		// (set) Token: 0x0600083E RID: 2110 RVA: 0x000180BA File Offset: 0x000162BA
		[Editor(false)]
		public ButtonWidget MatchmakingButtonWidget
		{
			get
			{
				return this._matchmakingButtonWidget;
			}
			set
			{
				if (this._matchmakingButtonWidget != value)
				{
					this._matchmakingButtonWidget = value;
					base.OnPropertyChanged<ButtonWidget>(value, "MatchmakingButtonWidget");
				}
			}
		}

		// Token: 0x040003C3 RID: 963
		private int _selectedItemIndex;

		// Token: 0x040003C4 RID: 964
		private ListPanel _menuItemListPanel;

		// Token: 0x040003C5 RID: 965
		private ButtonWidget _matchmakingButtonWidget;
	}
}
