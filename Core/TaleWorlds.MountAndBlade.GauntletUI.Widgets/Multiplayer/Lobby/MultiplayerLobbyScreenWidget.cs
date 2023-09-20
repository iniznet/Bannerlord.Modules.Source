using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Lobby.Friend;
using TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Lobby.Matchmaking;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Lobby
{
	public class MultiplayerLobbyScreenWidget : Widget
	{
		public MultiplayerLobbyScreenWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (!this._initialized)
			{
				this.OnLobbyStateChanged();
				this._initialized = true;
			}
		}

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

		private void HomeScreenWidgetPropertyChanged(PropertyOwnerObject owner, string property, bool value)
		{
			this.ToggleFriendListOnTabToggled(property, value);
		}

		private void SocialScreenWidgetPropertyChanged(PropertyOwnerObject owner, string property, bool value)
		{
			this.ToggleFriendListOnTabToggled(property, value);
		}

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

		private bool _initialized;

		private bool _stateChangeLocked;

		private bool _isLoggedIn;

		private bool _isSearchGameRequested;

		private bool _isSearchingGame;

		private bool _isMatchmakingEnabled;

		private bool _isPartyLeader;

		private bool _isInParty;

		private bool _isCustomBattleEnabled;

		private MultiplayerLobbyMenuWidget _menuWidget;

		private MultiplayerLobbyHomeScreenWidget _homeScreenWidget;

		private MultiplayerLobbyMatchmakingScreenWidget _matchmakingScreenWidget;

		private MultiplayerLobbyFriendsPanelWidget _friendsPanelWidget;

		private MultiplayerLobbyProfileScreenWidget _profileScreenWidget;
	}
}
