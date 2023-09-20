using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Lobby
{
	// Token: 0x02000096 RID: 150
	public class MultiplayerLobbyCustomServerScreenWidget : Widget
	{
		// Token: 0x170002C5 RID: 709
		// (get) Token: 0x060007E8 RID: 2024 RVA: 0x000174A4 File Offset: 0x000156A4
		// (set) Token: 0x060007E9 RID: 2025 RVA: 0x000174AC File Offset: 0x000156AC
		public NavigationScopeTargeter FilterSearchBarScope { get; set; }

		// Token: 0x170002C6 RID: 710
		// (get) Token: 0x060007EA RID: 2026 RVA: 0x000174B5 File Offset: 0x000156B5
		// (set) Token: 0x060007EB RID: 2027 RVA: 0x000174BD File Offset: 0x000156BD
		public NavigationScopeTargeter FilterButtonsScope { get; set; }

		// Token: 0x060007EC RID: 2028 RVA: 0x000174C6 File Offset: 0x000156C6
		public MultiplayerLobbyCustomServerScreenWidget(UIContext context)
			: base(context)
		{
			this._createGameClickHandler = new Action<Widget>(this.OnCreateGameClick);
			this._closeCreatePanelClickHandler = new Action<Widget>(this.OnCloseCreatePanelClick);
			this._isCreateGamePanelActive = false;
		}

		// Token: 0x060007ED RID: 2029 RVA: 0x000174FA File Offset: 0x000156FA
		private void OnCreateGameClick(Widget widget)
		{
			this._isCreateGamePanelActive = true;
			this.UpdatePanels();
		}

		// Token: 0x060007EE RID: 2030 RVA: 0x00017509 File Offset: 0x00015709
		private void OnCloseCreatePanelClick(Widget widget)
		{
			this._isCreateGamePanelActive = false;
			this.UpdatePanels();
		}

		// Token: 0x060007EF RID: 2031 RVA: 0x00017518 File Offset: 0x00015718
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

		// Token: 0x060007F0 RID: 2032 RVA: 0x00017621 File Offset: 0x00015821
		private void FiltersPanelUpdated()
		{
			this.FiltersPanel.AddState("Disabled");
		}

		// Token: 0x060007F1 RID: 2033 RVA: 0x00017633 File Offset: 0x00015833
		private void ServerListItemsChanged(Widget widget)
		{
			this.ServerCountText.IntText = this.ServerListPanel.ChildCount;
		}

		// Token: 0x060007F2 RID: 2034 RVA: 0x0001764B File Offset: 0x0001584B
		private void ServerListItemsChanged(Widget parentWidget, Widget addedWidget)
		{
			this.ServerCountText.IntText = this.ServerListPanel.ChildCount;
		}

		// Token: 0x060007F3 RID: 2035 RVA: 0x00017663 File Offset: 0x00015863
		private void ServerSelectionChanged(Widget child)
		{
			this.OnUpdateJoinServerEnabled();
			this.SelectedIndex = this.ServerListPanel.IntValue;
		}

		// Token: 0x060007F4 RID: 2036 RVA: 0x0001767C File Offset: 0x0001587C
		private void OnUpdateJoinServerEnabled()
		{
			if (this.JoinServerButton != null && this.ServerListPanel != null)
			{
				this.JoinServerButton.IsEnabled = this.ServerListPanel.IntValue >= 0 && (this.IsPartyLeader || !this.IsInParty);
			}
		}

		// Token: 0x060007F5 RID: 2037 RVA: 0x000176CC File Offset: 0x000158CC
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

		// Token: 0x060007F6 RID: 2038 RVA: 0x0001773A File Offset: 0x0001593A
		private void RefreshClicked(Widget widget)
		{
			this.JoinServerButton.IsEnabled = false;
		}

		// Token: 0x170002C7 RID: 711
		// (get) Token: 0x060007F7 RID: 2039 RVA: 0x00017748 File Offset: 0x00015948
		// (set) Token: 0x060007F8 RID: 2040 RVA: 0x00017750 File Offset: 0x00015950
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

		// Token: 0x170002C8 RID: 712
		// (get) Token: 0x060007F9 RID: 2041 RVA: 0x0001776E File Offset: 0x0001596E
		// (set) Token: 0x060007FA RID: 2042 RVA: 0x00017778 File Offset: 0x00015978
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

		// Token: 0x170002C9 RID: 713
		// (get) Token: 0x060007FB RID: 2043 RVA: 0x00017873 File Offset: 0x00015A73
		// (set) Token: 0x060007FC RID: 2044 RVA: 0x0001787B File Offset: 0x00015A7B
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

		// Token: 0x170002CA RID: 714
		// (get) Token: 0x060007FD RID: 2045 RVA: 0x00017899 File Offset: 0x00015A99
		// (set) Token: 0x060007FE RID: 2046 RVA: 0x000178A1 File Offset: 0x00015AA1
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

		// Token: 0x170002CB RID: 715
		// (get) Token: 0x060007FF RID: 2047 RVA: 0x000178BF File Offset: 0x00015ABF
		// (set) Token: 0x06000800 RID: 2048 RVA: 0x000178C8 File Offset: 0x00015AC8
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

		// Token: 0x170002CC RID: 716
		// (get) Token: 0x06000801 RID: 2049 RVA: 0x0001792A File Offset: 0x00015B2A
		// (set) Token: 0x06000802 RID: 2050 RVA: 0x00017934 File Offset: 0x00015B34
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

		// Token: 0x170002CD RID: 717
		// (get) Token: 0x06000803 RID: 2051 RVA: 0x00017996 File Offset: 0x00015B96
		// (set) Token: 0x06000804 RID: 2052 RVA: 0x0001799E File Offset: 0x00015B9E
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

		// Token: 0x170002CE RID: 718
		// (get) Token: 0x06000805 RID: 2053 RVA: 0x000179BC File Offset: 0x00015BBC
		// (set) Token: 0x06000806 RID: 2054 RVA: 0x000179C4 File Offset: 0x00015BC4
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

		// Token: 0x170002CF RID: 719
		// (get) Token: 0x06000807 RID: 2055 RVA: 0x000179E2 File Offset: 0x00015BE2
		// (set) Token: 0x06000808 RID: 2056 RVA: 0x000179EC File Offset: 0x00015BEC
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

		// Token: 0x170002D0 RID: 720
		// (get) Token: 0x06000809 RID: 2057 RVA: 0x00017A5A File Offset: 0x00015C5A
		// (set) Token: 0x0600080A RID: 2058 RVA: 0x00017A62 File Offset: 0x00015C62
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

		// Token: 0x170002D1 RID: 721
		// (get) Token: 0x0600080B RID: 2059 RVA: 0x00017A86 File Offset: 0x00015C86
		// (set) Token: 0x0600080C RID: 2060 RVA: 0x00017A8E File Offset: 0x00015C8E
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

		// Token: 0x170002D2 RID: 722
		// (get) Token: 0x0600080D RID: 2061 RVA: 0x00017AAC File Offset: 0x00015CAC
		// (set) Token: 0x0600080E RID: 2062 RVA: 0x00017AB4 File Offset: 0x00015CB4
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

		// Token: 0x170002D3 RID: 723
		// (get) Token: 0x0600080F RID: 2063 RVA: 0x00017AD2 File Offset: 0x00015CD2
		// (set) Token: 0x06000810 RID: 2064 RVA: 0x00017ADA File Offset: 0x00015CDA
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

		// Token: 0x170002D4 RID: 724
		// (get) Token: 0x06000811 RID: 2065 RVA: 0x00017B04 File Offset: 0x00015D04
		// (set) Token: 0x06000812 RID: 2066 RVA: 0x00017B0C File Offset: 0x00015D0C
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

		// Token: 0x170002D5 RID: 725
		// (get) Token: 0x06000813 RID: 2067 RVA: 0x00017B36 File Offset: 0x00015D36
		// (set) Token: 0x06000814 RID: 2068 RVA: 0x00017B3E File Offset: 0x00015D3E
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

		// Token: 0x170002D6 RID: 726
		// (get) Token: 0x06000815 RID: 2069 RVA: 0x00017B7A File Offset: 0x00015D7A
		// (set) Token: 0x06000816 RID: 2070 RVA: 0x00017B82 File Offset: 0x00015D82
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

		// Token: 0x040003A2 RID: 930
		private readonly Action<Widget> _createGameClickHandler;

		// Token: 0x040003A3 RID: 931
		private readonly Action<Widget> _closeCreatePanelClickHandler;

		// Token: 0x040003A4 RID: 932
		private bool _isCreateGamePanelActive;

		// Token: 0x040003A7 RID: 935
		private int _selectedIndex;

		// Token: 0x040003A8 RID: 936
		private ListPanel _serverListPanel;

		// Token: 0x040003A9 RID: 937
		private ButtonWidget _joinServerButton;

		// Token: 0x040003AA RID: 938
		private ButtonWidget _hostServerButton;

		// Token: 0x040003AB RID: 939
		private ButtonWidget _createGameButton;

		// Token: 0x040003AC RID: 940
		private ButtonWidget _closeCreatePanelButton;

		// Token: 0x040003AD RID: 941
		private Widget _joinGamePanel;

		// Token: 0x040003AE RID: 942
		private Widget _createGamePanel;

		// Token: 0x040003AF RID: 943
		private ButtonWidget _refreshButton;

		// Token: 0x040003B0 RID: 944
		private Widget _filtersPanel;

		// Token: 0x040003B1 RID: 945
		private TextWidget _serverCountText;

		// Token: 0x040003B2 RID: 946
		private TextWidget _infoText;

		// Token: 0x040003B3 RID: 947
		private bool _isPlayerBasedCustomBattleEnabled;

		// Token: 0x040003B4 RID: 948
		private bool _isPremadeGameEnabled;

		// Token: 0x040003B5 RID: 949
		private bool _isPartyLeader;

		// Token: 0x040003B6 RID: 950
		private bool _isInParty;
	}
}
