using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Lobby
{
	// Token: 0x0200009D RID: 157
	public class MultiplayerLobbyProfileScreenWidget : Widget
	{
		// Token: 0x0600083F RID: 2111 RVA: 0x000180D8 File Offset: 0x000162D8
		public MultiplayerLobbyProfileScreenWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000840 RID: 2112 RVA: 0x000180E4 File Offset: 0x000162E4
		public void LobbyStateChanged(bool isSearchRequested, bool isSearching, bool isMatchmakingEnabled, bool isCustomBattleEnabled, bool isPartyLeader, bool isInParty)
		{
			this.FindGameButton.IsEnabled = !this.HasUnofficialModulesLoaded && isMatchmakingEnabled && !isSearchRequested && (isPartyLeader || !isInParty);
			this.FindGameButton.IsVisible = !this.HasUnofficialModulesLoaded && !isSearching;
			this.SelectionInfo.IsEnabled = !this.HasUnofficialModulesLoaded && isMatchmakingEnabled;
			this.SelectionInfo.IsVisible = !this.HasUnofficialModulesLoaded && !isSearching;
		}

		// Token: 0x06000841 RID: 2113 RVA: 0x00018165 File Offset: 0x00016365
		private void OnSubpageIndexChange()
		{
		}

		// Token: 0x170002E4 RID: 740
		// (get) Token: 0x06000842 RID: 2114 RVA: 0x00018167 File Offset: 0x00016367
		// (set) Token: 0x06000843 RID: 2115 RVA: 0x0001816F File Offset: 0x0001636F
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

		// Token: 0x170002E5 RID: 741
		// (get) Token: 0x06000844 RID: 2116 RVA: 0x00018193 File Offset: 0x00016393
		// (set) Token: 0x06000845 RID: 2117 RVA: 0x0001819B File Offset: 0x0001639B
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

		// Token: 0x170002E6 RID: 742
		// (get) Token: 0x06000846 RID: 2118 RVA: 0x000181B9 File Offset: 0x000163B9
		// (set) Token: 0x06000847 RID: 2119 RVA: 0x000181C1 File Offset: 0x000163C1
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

		// Token: 0x170002E7 RID: 743
		// (get) Token: 0x06000848 RID: 2120 RVA: 0x000181DF File Offset: 0x000163DF
		// (set) Token: 0x06000849 RID: 2121 RVA: 0x000181E7 File Offset: 0x000163E7
		[Editor(false)]
		public bool HasUnofficialModulesLoaded
		{
			get
			{
				return this._hasUnofficialModulesLoaded;
			}
			set
			{
				if (value != this._hasUnofficialModulesLoaded)
				{
					this._hasUnofficialModulesLoaded = value;
					base.OnPropertyChanged(value, "HasUnofficialModulesLoaded");
				}
			}
		}

		// Token: 0x040003C6 RID: 966
		private ButtonWidget _findGameButton;

		// Token: 0x040003C7 RID: 967
		private Widget _selectionInfo;

		// Token: 0x040003C8 RID: 968
		private int _selectedModeIndex;

		// Token: 0x040003C9 RID: 969
		private bool _hasUnofficialModulesLoaded;
	}
}
