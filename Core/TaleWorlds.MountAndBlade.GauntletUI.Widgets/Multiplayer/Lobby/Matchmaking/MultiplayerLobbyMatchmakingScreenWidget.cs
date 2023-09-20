using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Lobby.Matchmaking
{
	public class MultiplayerLobbyMatchmakingScreenWidget : Widget
	{
		public MultiplayerLobbyCustomServerScreenWidget CustomServerParentWidget { get; set; }

		public MultiplayerLobbyCustomServerScreenWidget PremadeMatchesParentWidget { get; set; }

		private MultiplayerLobbyMatchmakingScreenWidget.MatchmakingSubPages _selectedMode
		{
			get
			{
				return (MultiplayerLobbyMatchmakingScreenWidget.MatchmakingSubPages)this.SelectedModeIndex;
			}
		}

		public MultiplayerLobbyMatchmakingScreenWidget(UIContext context)
			: base(context)
		{
		}

		public void LobbyStateChanged(bool isSearchRequested, bool isSearching, bool isMatchmakingEnabled, bool isCustomBattleEnabled, bool isPartyLeader, bool isInParty)
		{
			this._latestIsSearchRequested = isSearchRequested;
			this._latestIsSearching = isSearching;
			this._latestIsMatchmakingEnabled = isMatchmakingEnabled;
			this._latestIsCustomBattleEnabled = isCustomBattleEnabled;
			this._latestIsPartyLeader = isPartyLeader;
			this._latestIsInParty = isInParty;
			if (this.CustomServerParentWidget != null)
			{
				this.CustomServerParentWidget.IsInParty = isInParty;
				this.CustomServerParentWidget.IsPartyLeader = isPartyLeader;
			}
			if (this.PremadeMatchesParentWidget != null)
			{
				this.PremadeMatchesParentWidget.IsInParty = isInParty;
				this.PremadeMatchesParentWidget.IsPartyLeader = isPartyLeader;
			}
			this.UpdateStates();
		}

		private void UpdateStates()
		{
			this.FindGameButton.IsEnabled = ((this._selectedMode != MultiplayerLobbyMatchmakingScreenWidget.MatchmakingSubPages.CustomGame && this._latestIsMatchmakingEnabled && this.IsMatchFindPossible) || (this._selectedMode == MultiplayerLobbyMatchmakingScreenWidget.MatchmakingSubPages.CustomGame && this.IsCustomGameFindEnabled)) && (this._latestIsPartyLeader || !this._latestIsInParty) && !this._latestIsSearchRequested;
			this.FindGameButton.IsVisible = !this._latestIsSearching && ((this._latestIsCustomBattleEnabled && this._selectedMode == MultiplayerLobbyMatchmakingScreenWidget.MatchmakingSubPages.CustomGame) || (this._latestIsMatchmakingEnabled && this._selectedMode == MultiplayerLobbyMatchmakingScreenWidget.MatchmakingSubPages.QuickPlay));
			this.SelectionInfo.IsEnabled = this._latestIsMatchmakingEnabled;
			this.SelectionInfo.IsVisible = !this._latestIsSearching && !this._latestIsCustomBattleEnabled;
		}

		private void OnSubpageIndexChange()
		{
			this.FindGameButton.IsVisible = !this._latestIsSearching && ((this._latestIsCustomBattleEnabled && this._selectedMode == MultiplayerLobbyMatchmakingScreenWidget.MatchmakingSubPages.CustomGame) || (this._latestIsMatchmakingEnabled && this._selectedMode == MultiplayerLobbyMatchmakingScreenWidget.MatchmakingSubPages.QuickPlay));
		}

		[Editor(false)]
		public bool IsMatchFindPossible
		{
			get
			{
				return this._isMatchFindPossible;
			}
			set
			{
				if (this._isMatchFindPossible != value)
				{
					this._isMatchFindPossible = value;
					base.OnPropertyChanged(value, "IsMatchFindPossible");
					this.UpdateStates();
				}
			}
		}

		[Editor(false)]
		public bool IsCustomGameFindEnabled
		{
			get
			{
				return this._isCustomGameFindEnabled;
			}
			set
			{
				if (this._isCustomGameFindEnabled != value)
				{
					this._isCustomGameFindEnabled = value;
					base.OnPropertyChanged(value, "IsCustomGameFindEnabled");
					this.UpdateStates();
				}
			}
		}

		[Editor(false)]
		public int SelectedModeIndex
		{
			get
			{
				return this._selectedModeIndex;
			}
			set
			{
				if (this._selectedModeIndex != value)
				{
					this._selectedModeIndex = value;
					base.OnPropertyChanged(value, "SelectedModeIndex");
					this.OnSubpageIndexChange();
				}
			}
		}

		[Editor(false)]
		public ButtonWidget FindGameButton
		{
			get
			{
				return this._findGameButton;
			}
			set
			{
				if (this._findGameButton != value)
				{
					this._findGameButton = value;
					base.OnPropertyChanged<ButtonWidget>(value, "FindGameButton");
				}
			}
		}

		[Editor(false)]
		public Widget SelectionInfo
		{
			get
			{
				return this._selectionInfo;
			}
			set
			{
				if (this._selectionInfo != value)
				{
					this._selectionInfo = value;
					base.OnPropertyChanged<Widget>(value, "SelectionInfo");
				}
			}
		}

		private bool _latestIsSearchRequested;

		private bool _latestIsSearching;

		private bool _latestIsMatchmakingEnabled;

		private bool _latestIsCustomBattleEnabled;

		private bool _latestIsPartyLeader;

		private bool _latestIsInParty;

		private ButtonWidget _findGameButton;

		private Widget _selectionInfo;

		private int _selectedModeIndex;

		private bool _isMatchFindPossible;

		private bool _isCustomGameFindEnabled;

		private enum MatchmakingSubPages
		{
			QuickPlay,
			CustomGame,
			CustomGameList,
			PremadeMatchList,
			Default
		}
	}
}
