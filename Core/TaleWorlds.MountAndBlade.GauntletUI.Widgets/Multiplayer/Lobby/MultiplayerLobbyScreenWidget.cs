using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Lobby.Friend;
using TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Lobby.Matchmaking;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Lobby
{
	// Token: 0x0200009F RID: 159
	public class MultiplayerLobbyScreenWidget : Widget
	{
		// Token: 0x0600084E RID: 2126 RVA: 0x00018298 File Offset: 0x00016498
		public MultiplayerLobbyScreenWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x0600084F RID: 2127 RVA: 0x000182A1 File Offset: 0x000164A1
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (!this._initialized)
			{
				this.OnLobbyStateChanged();
				this._initialized = true;
			}
		}

		// Token: 0x06000850 RID: 2128 RVA: 0x000182C0 File Offset: 0x000164C0
		private void OnLoggedInChanged()
		{
			if (!this._isLoggedIn)
			{
				this._stateChangeLocked = true;
				this.IsSearchGameRequested = false;
				this.IsSearchingGame = false;
				this.IsCustomBattleEnabled = false;
				this.IsMatchmakingEnabled = false;
				this.IsPartyLeader = false;
				this.IsInParty = false;
				this._stateChangeLocked = false;
				this.OnLobbyStateChanged();
			}
		}

		// Token: 0x06000851 RID: 2129 RVA: 0x00018314 File Offset: 0x00016514
		private void OnLobbyStateChanged()
		{
			if (!this._stateChangeLocked)
			{
				this.MenuWidget.LobbyStateChanged(this.IsSearchGameRequested, this.IsSearchingGame, this.IsMatchmakingEnabled, this.IsCustomBattleEnabled, this.IsPartyLeader, this.IsInParty);
				this.HomeScreenWidget.LobbyStateChanged(this.IsSearchGameRequested, this.IsSearchingGame, this.IsMatchmakingEnabled, this.IsCustomBattleEnabled, this.IsPartyLeader, this.IsInParty);
				this.MatchmakingScreenWidget.LobbyStateChanged(this.IsSearchGameRequested, this.IsSearchingGame, this.IsMatchmakingEnabled, this.IsCustomBattleEnabled, this.IsPartyLeader, this.IsInParty);
				this.ProfileScreenWidget.LobbyStateChanged(this.IsSearchGameRequested, this.IsSearchingGame, this.IsMatchmakingEnabled, this.IsCustomBattleEnabled, this.IsPartyLeader, this.IsInParty);
			}
		}

		// Token: 0x06000852 RID: 2130 RVA: 0x000183E8 File Offset: 0x000165E8
		private void HomeScreenWidgetPropertyChanged(PropertyOwnerObject owner, string property, bool value)
		{
			this.ToggleFriendListOnTabToggled(property, value);
		}

		// Token: 0x06000853 RID: 2131 RVA: 0x000183F2 File Offset: 0x000165F2
		private void SocialScreenWidgetPropertyChanged(PropertyOwnerObject owner, string property, bool value)
		{
			this.ToggleFriendListOnTabToggled(property, value);
		}

		// Token: 0x06000854 RID: 2132 RVA: 0x000183FC File Offset: 0x000165FC
		private void ToggleFriendListOnTabToggled(string property, bool value)
		{
			if (this.FriendsPanelWidget != null && property == "IsVisible")
			{
				bool flag = value;
				if (!flag)
				{
					MultiplayerLobbyHomeScreenWidget homeScreenWidget = this.HomeScreenWidget;
					if (homeScreenWidget == null || !homeScreenWidget.IsVisible)
					{
						MultiplayerLobbyProfileScreenWidget profileScreenWidget = this.ProfileScreenWidget;
						if (profileScreenWidget == null || !profileScreenWidget.IsVisible)
						{
							goto IL_44;
						}
					}
					flag = true;
				}
				IL_44:
				this.FriendsPanelWidget.IsForcedOpen = flag;
			}
		}

		// Token: 0x170002E9 RID: 745
		// (get) Token: 0x06000855 RID: 2133 RVA: 0x00018459 File Offset: 0x00016659
		// (set) Token: 0x06000856 RID: 2134 RVA: 0x00018461 File Offset: 0x00016661
		[Editor(false)]
		public bool IsLoggedIn
		{
			get
			{
				return this._isLoggedIn;
			}
			set
			{
				if (value != this._isLoggedIn)
				{
					this._isLoggedIn = value;
					base.OnPropertyChanged(value, "IsLoggedIn");
					this.OnLoggedInChanged();
				}
			}
		}

		// Token: 0x170002EA RID: 746
		// (get) Token: 0x06000857 RID: 2135 RVA: 0x00018485 File Offset: 0x00016685
		// (set) Token: 0x06000858 RID: 2136 RVA: 0x0001848D File Offset: 0x0001668D
		[Editor(false)]
		public bool IsSearchGameRequested
		{
			get
			{
				return this._isSearchGameRequested;
			}
			set
			{
				if (this._isSearchGameRequested != value)
				{
					this._isSearchGameRequested = value;
					base.OnPropertyChanged(value, "IsSearchGameRequested");
					this.OnLobbyStateChanged();
				}
			}
		}

		// Token: 0x170002EB RID: 747
		// (get) Token: 0x06000859 RID: 2137 RVA: 0x000184B1 File Offset: 0x000166B1
		// (set) Token: 0x0600085A RID: 2138 RVA: 0x000184B9 File Offset: 0x000166B9
		[Editor(false)]
		public bool IsSearchingGame
		{
			get
			{
				return this._isSearchingGame;
			}
			set
			{
				if (this._isSearchingGame != value)
				{
					this._isSearchingGame = value;
					base.OnPropertyChanged(value, "IsSearchingGame");
					this.OnLobbyStateChanged();
				}
			}
		}

		// Token: 0x170002EC RID: 748
		// (get) Token: 0x0600085B RID: 2139 RVA: 0x000184DD File Offset: 0x000166DD
		// (set) Token: 0x0600085C RID: 2140 RVA: 0x000184E5 File Offset: 0x000166E5
		[Editor(false)]
		public bool IsCustomBattleEnabled
		{
			get
			{
				return this._isCustomBattleEnabled;
			}
			set
			{
				if (this._isCustomBattleEnabled != value)
				{
					this._isCustomBattleEnabled = value;
					base.OnPropertyChanged(value, "IsCustomBattleEnabled");
					this.OnLobbyStateChanged();
				}
			}
		}

		// Token: 0x170002ED RID: 749
		// (get) Token: 0x0600085D RID: 2141 RVA: 0x00018509 File Offset: 0x00016709
		// (set) Token: 0x0600085E RID: 2142 RVA: 0x00018511 File Offset: 0x00016711
		[Editor(false)]
		public bool IsMatchmakingEnabled
		{
			get
			{
				return this._isMatchmakingEnabled;
			}
			set
			{
				if (this._isMatchmakingEnabled != value)
				{
					this._isMatchmakingEnabled = value;
					base.OnPropertyChanged(value, "IsMatchmakingEnabled");
					this.OnLobbyStateChanged();
				}
			}
		}

		// Token: 0x170002EE RID: 750
		// (get) Token: 0x0600085F RID: 2143 RVA: 0x00018535 File Offset: 0x00016735
		// (set) Token: 0x06000860 RID: 2144 RVA: 0x0001853D File Offset: 0x0001673D
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
					this.OnLobbyStateChanged();
				}
			}
		}

		// Token: 0x170002EF RID: 751
		// (get) Token: 0x06000861 RID: 2145 RVA: 0x00018561 File Offset: 0x00016761
		// (set) Token: 0x06000862 RID: 2146 RVA: 0x00018569 File Offset: 0x00016769
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
					this.OnLobbyStateChanged();
				}
			}
		}

		// Token: 0x170002F0 RID: 752
		// (get) Token: 0x06000863 RID: 2147 RVA: 0x0001858D File Offset: 0x0001678D
		// (set) Token: 0x06000864 RID: 2148 RVA: 0x00018595 File Offset: 0x00016795
		[Editor(false)]
		public MultiplayerLobbyMenuWidget MenuWidget
		{
			get
			{
				return this._menuWidget;
			}
			set
			{
				if (this._menuWidget != value)
				{
					this._menuWidget = value;
					base.OnPropertyChanged<MultiplayerLobbyMenuWidget>(value, "MenuWidget");
				}
			}
		}

		// Token: 0x170002F1 RID: 753
		// (get) Token: 0x06000865 RID: 2149 RVA: 0x000185B3 File Offset: 0x000167B3
		// (set) Token: 0x06000866 RID: 2150 RVA: 0x000185BC File Offset: 0x000167BC
		[Editor(false)]
		public MultiplayerLobbyHomeScreenWidget HomeScreenWidget
		{
			get
			{
				return this._homeScreenWidget;
			}
			set
			{
				if (this._homeScreenWidget != value)
				{
					if (this._homeScreenWidget != null)
					{
						this._homeScreenWidget.boolPropertyChanged -= this.HomeScreenWidgetPropertyChanged;
					}
					this._homeScreenWidget = value;
					if (this._homeScreenWidget != null)
					{
						this._homeScreenWidget.boolPropertyChanged += this.HomeScreenWidgetPropertyChanged;
					}
					base.OnPropertyChanged<MultiplayerLobbyHomeScreenWidget>(value, "HomeScreenWidget");
				}
			}
		}

		// Token: 0x170002F2 RID: 754
		// (get) Token: 0x06000867 RID: 2151 RVA: 0x00018623 File Offset: 0x00016823
		// (set) Token: 0x06000868 RID: 2152 RVA: 0x0001862B File Offset: 0x0001682B
		[Editor(false)]
		public MultiplayerLobbyMatchmakingScreenWidget MatchmakingScreenWidget
		{
			get
			{
				return this._matchmakingScreenWidget;
			}
			set
			{
				if (this._matchmakingScreenWidget != value)
				{
					this._matchmakingScreenWidget = value;
					base.OnPropertyChanged<MultiplayerLobbyMatchmakingScreenWidget>(value, "MatchmakingScreenWidget");
				}
			}
		}

		// Token: 0x170002F3 RID: 755
		// (get) Token: 0x06000869 RID: 2153 RVA: 0x00018649 File Offset: 0x00016849
		// (set) Token: 0x0600086A RID: 2154 RVA: 0x00018654 File Offset: 0x00016854
		[Editor(false)]
		public MultiplayerLobbyProfileScreenWidget ProfileScreenWidget
		{
			get
			{
				return this._profileScreenWidget;
			}
			set
			{
				if (this._profileScreenWidget != value)
				{
					if (this._profileScreenWidget != null)
					{
						this._profileScreenWidget.boolPropertyChanged -= this.SocialScreenWidgetPropertyChanged;
					}
					this._profileScreenWidget = value;
					if (this._profileScreenWidget != null)
					{
						this._profileScreenWidget.boolPropertyChanged += this.SocialScreenWidgetPropertyChanged;
					}
					base.OnPropertyChanged<MultiplayerLobbyProfileScreenWidget>(value, "ProfileScreenWidget");
				}
			}
		}

		// Token: 0x170002F4 RID: 756
		// (get) Token: 0x0600086B RID: 2155 RVA: 0x000186BB File Offset: 0x000168BB
		// (set) Token: 0x0600086C RID: 2156 RVA: 0x000186C3 File Offset: 0x000168C3
		[Editor(false)]
		public MultiplayerLobbyFriendsPanelWidget FriendsPanelWidget
		{
			get
			{
				return this._friendsPanelWidget;
			}
			set
			{
				if (this._friendsPanelWidget != value)
				{
					this._friendsPanelWidget = value;
					base.OnPropertyChanged<MultiplayerLobbyFriendsPanelWidget>(value, "FriendsPanelWidget");
				}
			}
		}

		// Token: 0x040003CC RID: 972
		private bool _initialized;

		// Token: 0x040003CD RID: 973
		private bool _stateChangeLocked;

		// Token: 0x040003CE RID: 974
		private bool _isLoggedIn;

		// Token: 0x040003CF RID: 975
		private bool _isSearchGameRequested;

		// Token: 0x040003D0 RID: 976
		private bool _isSearchingGame;

		// Token: 0x040003D1 RID: 977
		private bool _isMatchmakingEnabled;

		// Token: 0x040003D2 RID: 978
		private bool _isPartyLeader;

		// Token: 0x040003D3 RID: 979
		private bool _isInParty;

		// Token: 0x040003D4 RID: 980
		private bool _isCustomBattleEnabled;

		// Token: 0x040003D5 RID: 981
		private MultiplayerLobbyMenuWidget _menuWidget;

		// Token: 0x040003D6 RID: 982
		private MultiplayerLobbyHomeScreenWidget _homeScreenWidget;

		// Token: 0x040003D7 RID: 983
		private MultiplayerLobbyMatchmakingScreenWidget _matchmakingScreenWidget;

		// Token: 0x040003D8 RID: 984
		private MultiplayerLobbyFriendsPanelWidget _friendsPanelWidget;

		// Token: 0x040003D9 RID: 985
		private MultiplayerLobbyProfileScreenWidget _profileScreenWidget;
	}
}
