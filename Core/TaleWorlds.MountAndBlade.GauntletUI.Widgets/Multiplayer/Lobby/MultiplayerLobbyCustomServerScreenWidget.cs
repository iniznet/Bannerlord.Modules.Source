using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Lobby
{
	public class MultiplayerLobbyCustomServerScreenWidget : Widget
	{
		public NavigationScopeTargeter FilterSearchBarScope { get; set; }

		public NavigationScopeTargeter FilterButtonsScope { get; set; }

		public MultiplayerLobbyCustomServerScreenWidget(UIContext context)
			: base(context)
		{
			this._createGameClickHandler = new Action<Widget>(this.OnCreateGameClick);
			this._closeCreatePanelClickHandler = new Action<Widget>(this.OnCloseCreatePanelClick);
			this._isCreateGamePanelActive = false;
		}

		private void OnCreateGameClick(Widget widget)
		{
			this._isCreateGamePanelActive = true;
			this.UpdatePanels();
		}

		private void OnCloseCreatePanelClick(Widget widget)
		{
			this._isCreateGamePanelActive = false;
			this.UpdatePanels();
		}

		private void UpdatePanels()
		{
			this.JoinGamePanel.IsVisible = !this._isCreateGamePanelActive;
			this.CreateGameButton.IsVisible = !this._isCreateGamePanelActive;
			this.CreateGamePanel.IsVisible = this._isCreateGamePanelActive;
			this.CloseCreatePanelButton.IsVisible = this._isCreateGamePanelActive;
			this.RefreshButton.IsVisible = !this._isCreateGamePanelActive;
			this.ServerCountText.IsVisible = !this._isCreateGamePanelActive;
			this.InfoText.IsVisible = !this._isCreateGamePanelActive;
			this.JoinServerButton.IsVisible = !this._isCreateGamePanelActive;
			this.HostServerButton.IsVisible = this._isCreateGamePanelActive;
			this.FiltersPanel.SetState(this._isCreateGamePanelActive ? "Disabled" : "Default");
			if (this.FilterSearchBarScope != null)
			{
				this.FilterSearchBarScope.IsScopeDisabled = this._isCreateGamePanelActive;
			}
			if (this.FilterButtonsScope != null)
			{
				this.FilterButtonsScope.IsScopeDisabled = this._isCreateGamePanelActive;
			}
		}

		private void FiltersPanelUpdated()
		{
			this.FiltersPanel.AddState("Disabled");
		}

		private void ServerListItemsChanged(Widget widget)
		{
			this.ServerCountText.IntText = this.ServerListPanel.ChildCount;
		}

		private void ServerListItemsChanged(Widget parentWidget, Widget addedWidget)
		{
			this.ServerCountText.IntText = this.ServerListPanel.ChildCount;
		}

		private void ServerSelectionChanged(Widget child)
		{
			this.OnUpdateJoinServerEnabled();
			this.SelectedIndex = this.ServerListPanel.IntValue;
		}

		private void OnUpdateJoinServerEnabled()
		{
			if (this.JoinServerButton != null && this.ServerListPanel != null)
			{
				this.JoinServerButton.IsEnabled = this.ServerListPanel.IntValue >= 0 && (this.IsPartyLeader || !this.IsInParty);
			}
		}

		private void OnUpdateCreateServerEnabled()
		{
			bool flag = (this.IsPlayerBasedCustomBattleEnabled || this.IsPremadeGameEnabled) && (this.IsPartyLeader || !this.IsInParty);
			if (this.CreateGameButton != null && this.ServerListPanel != null)
			{
				this.CreateGameButton.IsVisible = flag;
			}
			if (this.CreateGamePanel.IsVisible && !flag)
			{
				this._isCreateGamePanelActive = false;
				this.UpdatePanels();
			}
		}

		private void RefreshClicked(Widget widget)
		{
			this.JoinServerButton.IsEnabled = false;
		}

		[Editor(false)]
		public int SelectedIndex
		{
			get
			{
				return this._selectedIndex;
			}
			set
			{
				if (this._selectedIndex != value)
				{
					this._selectedIndex = value;
					base.OnPropertyChanged(value, "SelectedIndex");
				}
			}
		}

		[Editor(false)]
		public ListPanel ServerListPanel
		{
			get
			{
				return this._serverListPanel;
			}
			set
			{
				if (this._serverListPanel != value)
				{
					ListPanel serverListPanel = this._serverListPanel;
					if (serverListPanel != null)
					{
						serverListPanel.ItemAddEventHandlers.Remove(new Action<Widget, Widget>(this.ServerListItemsChanged));
					}
					ListPanel serverListPanel2 = this._serverListPanel;
					if (serverListPanel2 != null)
					{
						serverListPanel2.ItemAfterRemoveEventHandlers.Remove(new Action<Widget>(this.ServerListItemsChanged));
					}
					ListPanel serverListPanel3 = this._serverListPanel;
					if (serverListPanel3 != null)
					{
						serverListPanel3.SelectEventHandlers.Remove(new Action<Widget>(this.ServerSelectionChanged));
					}
					this._serverListPanel = value;
					ListPanel serverListPanel4 = this._serverListPanel;
					if (serverListPanel4 != null)
					{
						serverListPanel4.ItemAddEventHandlers.Add(new Action<Widget, Widget>(this.ServerListItemsChanged));
					}
					ListPanel serverListPanel5 = this._serverListPanel;
					if (serverListPanel5 != null)
					{
						serverListPanel5.ItemAfterRemoveEventHandlers.Add(new Action<Widget>(this.ServerListItemsChanged));
					}
					ListPanel serverListPanel6 = this._serverListPanel;
					if (serverListPanel6 != null)
					{
						serverListPanel6.SelectEventHandlers.Add(new Action<Widget>(this.ServerSelectionChanged));
					}
					base.OnPropertyChanged<ListPanel>(value, "ServerListPanel");
				}
			}
		}

		[Editor(false)]
		public ButtonWidget JoinServerButton
		{
			get
			{
				return this._joinServerButton;
			}
			set
			{
				if (this._joinServerButton != value)
				{
					this._joinServerButton = value;
					base.OnPropertyChanged<ButtonWidget>(value, "JoinServerButton");
				}
			}
		}

		[Editor(false)]
		public ButtonWidget HostServerButton
		{
			get
			{
				return this._hostServerButton;
			}
			set
			{
				if (this._hostServerButton != value)
				{
					this._hostServerButton = value;
					base.OnPropertyChanged<ButtonWidget>(value, "HostServerButton");
				}
			}
		}

		[Editor(false)]
		public ButtonWidget CreateGameButton
		{
			get
			{
				return this._createGameButton;
			}
			set
			{
				if (this._createGameButton != value)
				{
					ButtonWidget createGameButton = this._createGameButton;
					if (createGameButton != null)
					{
						createGameButton.ClickEventHandlers.Remove(this._createGameClickHandler);
					}
					this._createGameButton = value;
					ButtonWidget createGameButton2 = this._createGameButton;
					if (createGameButton2 != null)
					{
						createGameButton2.ClickEventHandlers.Add(this._createGameClickHandler);
					}
					base.OnPropertyChanged<ButtonWidget>(value, "CreateGameButton");
				}
			}
		}

		[Editor(false)]
		public ButtonWidget CloseCreatePanelButton
		{
			get
			{
				return this._closeCreatePanelButton;
			}
			set
			{
				if (this._closeCreatePanelButton != value)
				{
					ButtonWidget closeCreatePanelButton = this._closeCreatePanelButton;
					if (closeCreatePanelButton != null)
					{
						closeCreatePanelButton.ClickEventHandlers.Remove(this._closeCreatePanelClickHandler);
					}
					this._closeCreatePanelButton = value;
					ButtonWidget closeCreatePanelButton2 = this._closeCreatePanelButton;
					if (closeCreatePanelButton2 != null)
					{
						closeCreatePanelButton2.ClickEventHandlers.Add(this._closeCreatePanelClickHandler);
					}
					base.OnPropertyChanged<ButtonWidget>(value, "CloseCreatePanelButton");
				}
			}
		}

		[Editor(false)]
		public Widget JoinGamePanel
		{
			get
			{
				return this._joinGamePanel;
			}
			set
			{
				if (this._joinGamePanel != value)
				{
					this._joinGamePanel = value;
					base.OnPropertyChanged<Widget>(value, "JoinGamePanel");
				}
			}
		}

		[Editor(false)]
		public Widget CreateGamePanel
		{
			get
			{
				return this._createGamePanel;
			}
			set
			{
				if (this._createGamePanel != value)
				{
					this._createGamePanel = value;
					base.OnPropertyChanged<Widget>(value, "CreateGamePanel");
				}
			}
		}

		[Editor(false)]
		public ButtonWidget RefreshButton
		{
			get
			{
				return this._refreshButton;
			}
			set
			{
				if (this._refreshButton != value)
				{
					ButtonWidget refreshButton = this._refreshButton;
					if (refreshButton != null)
					{
						refreshButton.ClickEventHandlers.Remove(new Action<Widget>(this.RefreshClicked));
					}
					this._refreshButton = value;
					ButtonWidget refreshButton2 = this._refreshButton;
					if (refreshButton2 != null)
					{
						refreshButton2.ClickEventHandlers.Add(new Action<Widget>(this.RefreshClicked));
					}
					base.OnPropertyChanged<ButtonWidget>(value, "RefreshButton");
				}
			}
		}

		[Editor(false)]
		public Widget FiltersPanel
		{
			get
			{
				return this._filtersPanel;
			}
			set
			{
				if (this._filtersPanel != value)
				{
					this._filtersPanel = value;
					base.OnPropertyChanged<Widget>(value, "FiltersPanel");
					this.FiltersPanelUpdated();
				}
			}
		}

		[Editor(false)]
		public TextWidget ServerCountText
		{
			get
			{
				return this._serverCountText;
			}
			set
			{
				if (this._serverCountText != value)
				{
					this._serverCountText = value;
					base.OnPropertyChanged<TextWidget>(value, "ServerCountText");
				}
			}
		}

		[Editor(false)]
		public TextWidget InfoText
		{
			get
			{
				return this._infoText;
			}
			set
			{
				if (this._infoText != value)
				{
					this._infoText = value;
					base.OnPropertyChanged<TextWidget>(value, "InfoText");
				}
			}
		}

		[Editor(false)]
		public bool IsPartyLeader
		{
			get
			{
				return this._isPartyLeader;
			}
			set
			{
				if (this._isPartyLeader != value)
				{
					this._isPartyLeader = value;
					base.OnPropertyChanged(value, "IsPartyLeader");
					this.OnUpdateJoinServerEnabled();
					this.OnUpdateCreateServerEnabled();
				}
			}
		}

		[Editor(false)]
		public bool IsInParty
		{
			get
			{
				return this._isInParty;
			}
			set
			{
				if (this._isInParty != value)
				{
					this._isInParty = value;
					base.OnPropertyChanged(value, "IsInParty");
					this.OnUpdateJoinServerEnabled();
					this.OnUpdateCreateServerEnabled();
				}
			}
		}

		[Editor(false)]
		public bool IsPlayerBasedCustomBattleEnabled
		{
			get
			{
				return this._isPlayerBasedCustomBattleEnabled;
			}
			set
			{
				if (!value && this._isCreateGamePanelActive)
				{
					this._isCreateGamePanelActive = false;
					this.UpdatePanels();
				}
				if (this._isPlayerBasedCustomBattleEnabled != value)
				{
					this._isPlayerBasedCustomBattleEnabled = value;
					base.OnPropertyChanged(value, "IsPlayerBasedCustomBattleEnabled");
				}
				this.OnUpdateCreateServerEnabled();
			}
		}

		[Editor(false)]
		public bool IsPremadeGameEnabled
		{
			get
			{
				return this._isPremadeGameEnabled;
			}
			set
			{
				if (!value && this._isCreateGamePanelActive)
				{
					this._isCreateGamePanelActive = false;
					this.UpdatePanels();
				}
				if (this._isPremadeGameEnabled != value)
				{
					this._isPremadeGameEnabled = value;
					base.OnPropertyChanged(value, "IsPremadeGameEnabled");
				}
				this.OnUpdateCreateServerEnabled();
			}
		}

		private readonly Action<Widget> _createGameClickHandler;

		private readonly Action<Widget> _closeCreatePanelClickHandler;

		private bool _isCreateGamePanelActive;

		private int _selectedIndex;

		private ListPanel _serverListPanel;

		private ButtonWidget _joinServerButton;

		private ButtonWidget _hostServerButton;

		private ButtonWidget _createGameButton;

		private ButtonWidget _closeCreatePanelButton;

		private Widget _joinGamePanel;

		private Widget _createGamePanel;

		private ButtonWidget _refreshButton;

		private Widget _filtersPanel;

		private TextWidget _serverCountText;

		private TextWidget _infoText;

		private bool _isPlayerBasedCustomBattleEnabled;

		private bool _isPremadeGameEnabled;

		private bool _isPartyLeader;

		private bool _isInParty;
	}
}
