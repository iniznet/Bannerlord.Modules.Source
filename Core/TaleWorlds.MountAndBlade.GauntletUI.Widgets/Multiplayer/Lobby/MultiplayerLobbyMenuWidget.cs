using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Lobby
{
	public class MultiplayerLobbyMenuWidget : Widget
	{
		public MultiplayerLobbyMenuWidget(UIContext context)
			: base(context)
		{
		}

		public void LobbyStateChanged(bool isSearchRequested, bool isSearching, bool isMatchmakingEnabled, bool isCustomBattleEnabled, bool isPartyLeader, bool isInParty)
		{
			this.MatchmakingButtonWidget.IsEnabled = isMatchmakingEnabled || isCustomBattleEnabled;
		}

		private void SelectedItemIndexChanged()
		{
			if (this.MenuItemListPanel == null)
			{
				return;
			}
			this.MenuItemListPanel.IntValue = this.SelectedItemIndex - 3;
		}

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

		private int _selectedItemIndex;

		private ListPanel _menuItemListPanel;

		private ButtonWidget _matchmakingButtonWidget;
	}
}
