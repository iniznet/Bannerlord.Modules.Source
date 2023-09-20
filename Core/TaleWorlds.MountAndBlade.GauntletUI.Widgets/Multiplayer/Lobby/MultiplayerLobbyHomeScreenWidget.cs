using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Lobby
{
	// Token: 0x0200009A RID: 154
	public class MultiplayerLobbyHomeScreenWidget : Widget
	{
		// Token: 0x06000825 RID: 2085 RVA: 0x00017E4B File Offset: 0x0001604B
		public MultiplayerLobbyHomeScreenWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000826 RID: 2086 RVA: 0x00017E54 File Offset: 0x00016054
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (!this._initialized)
			{
				if (base.IsVisible)
				{
					base.OnPropertyChanged(true, "IsVisible");
				}
				this._initialized = true;
			}
		}

		// Token: 0x06000827 RID: 2087 RVA: 0x00017E80 File Offset: 0x00016080
		public void LobbyStateChanged(bool isSearchRequested, bool isSearching, bool isMatchmakingEnabled, bool isCustomBattleEnabled, bool isPartyLeader, bool isInParty)
		{
			this.FindGameButton.IsEnabled = !this.HasUnofficialModulesLoaded && isMatchmakingEnabled && !isSearchRequested && (isPartyLeader || !isInParty);
			this.FindGameButton.IsVisible = !this.HasUnofficialModulesLoaded && !isSearching;
			this.SelectionInfo.IsEnabled = !this.HasUnofficialModulesLoaded && isMatchmakingEnabled;
			this.SelectionInfo.IsVisible = !this.HasUnofficialModulesLoaded && !isSearching;
		}

		// Token: 0x170002DB RID: 731
		// (get) Token: 0x06000828 RID: 2088 RVA: 0x00017F01 File Offset: 0x00016101
		// (set) Token: 0x06000829 RID: 2089 RVA: 0x00017F09 File Offset: 0x00016109
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

		// Token: 0x170002DC RID: 732
		// (get) Token: 0x0600082A RID: 2090 RVA: 0x00017F27 File Offset: 0x00016127
		// (set) Token: 0x0600082B RID: 2091 RVA: 0x00017F2F File Offset: 0x0001612F
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

		// Token: 0x170002DD RID: 733
		// (get) Token: 0x0600082C RID: 2092 RVA: 0x00017F4D File Offset: 0x0001614D
		// (set) Token: 0x0600082D RID: 2093 RVA: 0x00017F55 File Offset: 0x00016155
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

		// Token: 0x040003BC RID: 956
		private bool _initialized;

		// Token: 0x040003BD RID: 957
		private ButtonWidget _findGameButton;

		// Token: 0x040003BE RID: 958
		private Widget _selectionInfo;

		// Token: 0x040003BF RID: 959
		private bool _hasUnofficialModulesLoaded;
	}
}
