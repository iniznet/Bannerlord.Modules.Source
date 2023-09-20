using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Lobby.Matchmaking
{
	// Token: 0x020000A1 RID: 161
	public class MultiplayerLobbyMatchmakingScreenWidget : Widget
	{
		// Token: 0x170002F6 RID: 758
		// (get) Token: 0x06000871 RID: 2161 RVA: 0x00018797 File Offset: 0x00016997
		// (set) Token: 0x06000872 RID: 2162 RVA: 0x0001879F File Offset: 0x0001699F
		public MultiplayerLobbyCustomServerScreenWidget CustomServerParentWidget { get; set; }

		// Token: 0x170002F7 RID: 759
		// (get) Token: 0x06000873 RID: 2163 RVA: 0x000187A8 File Offset: 0x000169A8
		// (set) Token: 0x06000874 RID: 2164 RVA: 0x000187B0 File Offset: 0x000169B0
		public MultiplayerLobbyCustomServerScreenWidget PremadeMatchesParentWidget { get; set; }

		// Token: 0x170002F8 RID: 760
		// (get) Token: 0x06000875 RID: 2165 RVA: 0x000187B9 File Offset: 0x000169B9
		private MultiplayerLobbyMatchmakingScreenWidget.MatchmakingSubPages _selectedMode
		{
			get
			{
				return (MultiplayerLobbyMatchmakingScreenWidget.MatchmakingSubPages)this.SelectedModeIndex;
			}
		}

		// Token: 0x06000876 RID: 2166 RVA: 0x000187C1 File Offset: 0x000169C1
		public MultiplayerLobbyMatchmakingScreenWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000877 RID: 2167 RVA: 0x000187CC File Offset: 0x000169CC
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

		// Token: 0x06000878 RID: 2168 RVA: 0x00018850 File Offset: 0x00016A50
		private void UpdateStates()
		{
			this.FindGameButton.IsEnabled = ((this._selectedMode != MultiplayerLobbyMatchmakingScreenWidget.MatchmakingSubPages.CustomGame && this._latestIsMatchmakingEnabled && this.IsMatchFindPossible) || (this._selectedMode == MultiplayerLobbyMatchmakingScreenWidget.MatchmakingSubPages.CustomGame && this.IsCustomGameFindEnabled)) && (this._latestIsPartyLeader || !this._latestIsInParty) && !this._latestIsSearchRequested;
			this.FindGameButton.IsVisible = !this._latestIsSearching && ((this._latestIsCustomBattleEnabled && this._selectedMode == MultiplayerLobbyMatchmakingScreenWidget.MatchmakingSubPages.CustomGame) || (this._latestIsMatchmakingEnabled && this._selectedMode == MultiplayerLobbyMatchmakingScreenWidget.MatchmakingSubPages.QuickPlay));
			this.SelectionInfo.IsEnabled = this._latestIsMatchmakingEnabled;
			this.SelectionInfo.IsVisible = !this._latestIsSearching && !this._latestIsCustomBattleEnabled;
		}

		// Token: 0x06000879 RID: 2169 RVA: 0x0001891C File Offset: 0x00016B1C
		private void OnSubpageIndexChange()
		{
			this.FindGameButton.IsVisible = !this._latestIsSearching && ((this._latestIsCustomBattleEnabled && this._selectedMode == MultiplayerLobbyMatchmakingScreenWidget.MatchmakingSubPages.CustomGame) || (this._latestIsMatchmakingEnabled && this._selectedMode == MultiplayerLobbyMatchmakingScreenWidget.MatchmakingSubPages.QuickPlay));
		}

		// Token: 0x170002F9 RID: 761
		// (get) Token: 0x0600087A RID: 2170 RVA: 0x0001895C File Offset: 0x00016B5C
		// (set) Token: 0x0600087B RID: 2171 RVA: 0x00018964 File Offset: 0x00016B64
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

		// Token: 0x170002FA RID: 762
		// (get) Token: 0x0600087C RID: 2172 RVA: 0x00018988 File Offset: 0x00016B88
		// (set) Token: 0x0600087D RID: 2173 RVA: 0x00018990 File Offset: 0x00016B90
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

		// Token: 0x170002FB RID: 763
		// (get) Token: 0x0600087E RID: 2174 RVA: 0x000189B4 File Offset: 0x00016BB4
		// (set) Token: 0x0600087F RID: 2175 RVA: 0x000189BC File Offset: 0x00016BBC
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

		// Token: 0x170002FC RID: 764
		// (get) Token: 0x06000880 RID: 2176 RVA: 0x000189E0 File Offset: 0x00016BE0
		// (set) Token: 0x06000881 RID: 2177 RVA: 0x000189E8 File Offset: 0x00016BE8
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

		// Token: 0x170002FD RID: 765
		// (get) Token: 0x06000882 RID: 2178 RVA: 0x00018A06 File Offset: 0x00016C06
		// (set) Token: 0x06000883 RID: 2179 RVA: 0x00018A0E File Offset: 0x00016C0E
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

		// Token: 0x040003DD RID: 989
		private bool _latestIsSearchRequested;

		// Token: 0x040003DE RID: 990
		private bool _latestIsSearching;

		// Token: 0x040003DF RID: 991
		private bool _latestIsMatchmakingEnabled;

		// Token: 0x040003E0 RID: 992
		private bool _latestIsCustomBattleEnabled;

		// Token: 0x040003E1 RID: 993
		private bool _latestIsPartyLeader;

		// Token: 0x040003E2 RID: 994
		private bool _latestIsInParty;

		// Token: 0x040003E3 RID: 995
		private ButtonWidget _findGameButton;

		// Token: 0x040003E4 RID: 996
		private Widget _selectionInfo;

		// Token: 0x040003E5 RID: 997
		private int _selectedModeIndex;

		// Token: 0x040003E6 RID: 998
		private bool _isMatchFindPossible;

		// Token: 0x040003E7 RID: 999
		private bool _isCustomGameFindEnabled;

		// Token: 0x0200018D RID: 397
		private enum MatchmakingSubPages
		{
			// Token: 0x040008EA RID: 2282
			QuickPlay,
			// Token: 0x040008EB RID: 2283
			CustomGame,
			// Token: 0x040008EC RID: 2284
			CustomGameList,
			// Token: 0x040008ED RID: 2285
			PremadeMatchList,
			// Token: 0x040008EE RID: 2286
			Default
		}
	}
}
