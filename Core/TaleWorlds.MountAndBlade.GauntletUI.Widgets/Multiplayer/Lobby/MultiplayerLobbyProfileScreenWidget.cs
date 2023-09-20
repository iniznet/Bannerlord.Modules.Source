using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Lobby
{
	public class MultiplayerLobbyProfileScreenWidget : Widget
	{
		public MultiplayerLobbyProfileScreenWidget(UIContext context)
			: base(context)
		{
		}

		public void LobbyStateChanged(bool isSearchRequested, bool isSearching, bool isMatchmakingEnabled, bool isCustomBattleEnabled, bool isPartyLeader, bool isInParty)
		{
			this.FindGameButton.IsEnabled = !this.HasUnofficialModulesLoaded && isMatchmakingEnabled && !isSearchRequested && (isPartyLeader || !isInParty);
			this.FindGameButton.IsVisible = !this.HasUnofficialModulesLoaded && !isSearching;
			this.SelectionInfo.IsEnabled = !this.HasUnofficialModulesLoaded && isMatchmakingEnabled;
			this.SelectionInfo.IsVisible = !this.HasUnofficialModulesLoaded && !isSearching;
		}

		private void OnSubpageIndexChange()
		{
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

		private ButtonWidget _findGameButton;

		private Widget _selectionInfo;

		private int _selectedModeIndex;

		private bool _hasUnofficialModulesLoaded;
	}
}
