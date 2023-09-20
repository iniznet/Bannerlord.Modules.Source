using System;
using TaleWorlds.Library;
using TaleWorlds.Library.NewsManager;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Friends;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Home
{
	// Token: 0x0200007D RID: 125
	public class MPLobbyHomeVM : ViewModel
	{
		// Token: 0x14000005 RID: 5
		// (add) Token: 0x06000B12 RID: 2834 RVA: 0x000273C0 File Offset: 0x000255C0
		// (remove) Token: 0x06000B13 RID: 2835 RVA: 0x000273F8 File Offset: 0x000255F8
		public event Action OnFindGameRequested;

		// Token: 0x06000B14 RID: 2836 RVA: 0x00027430 File Offset: 0x00025630
		public MPLobbyHomeVM(NewsManager newsManager, Action<MPLobbyVM.LobbyPage> onChangePageRequest)
		{
			this._onChangePageRequest = onChangePageRequest;
			this.HasUnofficialModulesLoaded = NetworkMain.GameClient.HasUnofficialModulesLoaded;
			this.Player = new MPLobbyPlayerBaseVM(NetworkMain.GameClient.PlayerID, "", null, null);
			this.News = new MPNewsVM(newsManager);
			this.RefreshValues();
		}

		// Token: 0x06000B15 RID: 2837 RVA: 0x00027488 File Offset: 0x00025688
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.FindGameText = new TextObject("{=yA45PqFc}FIND GAME", null).ToString();
			this.MatchFindNotPossibleText = new TextObject("{=BrYUHFsg}CHOOSE GAME", null).ToString();
			this.OpenProfileText = new TextObject("{=aBCi76ig}Show More", null).ToString();
			this.Player.RefreshValues();
			this.News.RefreshValues();
		}

		// Token: 0x06000B16 RID: 2838 RVA: 0x000274F3 File Offset: 0x000256F3
		public void RefreshPlayerData(PlayerData playerData, bool updateRating = true)
		{
			this.Player.UpdateWith(playerData);
			if (updateRating)
			{
				this.Player.UpdateRating(new Action(this.OnRatingReceived));
			}
		}

		// Token: 0x06000B17 RID: 2839 RVA: 0x0002751B File Offset: 0x0002571B
		private void OnRatingReceived()
		{
			this.Player.RefreshSelectableGameTypes(true, new Action<string>(this.Player.UpdateDisplayedRankInfo), "");
		}

		// Token: 0x06000B18 RID: 2840 RVA: 0x0002753F File Offset: 0x0002573F
		public void OnMatchSelectionChanged(string selectionInfo, bool isMatchFindPossible)
		{
			this.SelectionInfoText = selectionInfo;
			this.IsMatchFindPossible = isMatchFindPossible;
		}

		// Token: 0x06000B19 RID: 2841 RVA: 0x0002754F File Offset: 0x0002574F
		private void ExecuteFindGame()
		{
			if (this.IsMatchFindPossible)
			{
				Action onFindGameRequested = this.OnFindGameRequested;
				if (onFindGameRequested == null)
				{
					return;
				}
				onFindGameRequested();
				return;
			}
			else
			{
				Action<MPLobbyVM.LobbyPage> onChangePageRequest = this._onChangePageRequest;
				if (onChangePageRequest == null)
				{
					return;
				}
				onChangePageRequest(MPLobbyVM.LobbyPage.Matchmaking);
				return;
			}
		}

		// Token: 0x06000B1A RID: 2842 RVA: 0x0002757B File Offset: 0x0002577B
		private void ExecuteOpenMatchmaking()
		{
			Action<MPLobbyVM.LobbyPage> onChangePageRequest = this._onChangePageRequest;
			if (onChangePageRequest == null)
			{
				return;
			}
			onChangePageRequest(MPLobbyVM.LobbyPage.Matchmaking);
		}

		// Token: 0x06000B1B RID: 2843 RVA: 0x0002758E File Offset: 0x0002578E
		private void ExecuteOpenProfile()
		{
			Action<MPLobbyVM.LobbyPage> onChangePageRequest = this._onChangePageRequest;
			if (onChangePageRequest == null)
			{
				return;
			}
			onChangePageRequest(MPLobbyVM.LobbyPage.Profile);
		}

		// Token: 0x06000B1C RID: 2844 RVA: 0x000275A1 File Offset: 0x000257A1
		public void OnClanInfoChanged()
		{
			this.Player.UpdateClanInfo();
		}

		// Token: 0x06000B1D RID: 2845 RVA: 0x000275AE File Offset: 0x000257AE
		public void OnPlayerNameUpdated(string playerName)
		{
			this.Player.UpdateNameAndAvatar(true);
		}

		// Token: 0x06000B1E RID: 2846 RVA: 0x000275BC File Offset: 0x000257BC
		public override void OnFinalize()
		{
			base.OnFinalize();
			this.News.OnFinalize();
			this.News = null;
		}

		// Token: 0x17000372 RID: 882
		// (get) Token: 0x06000B1F RID: 2847 RVA: 0x000275D6 File Offset: 0x000257D6
		// (set) Token: 0x06000B20 RID: 2848 RVA: 0x000275DE File Offset: 0x000257DE
		[DataSourceProperty]
		public bool IsEnabled
		{
			get
			{
				return this._isEnabled;
			}
			set
			{
				if (value != this._isEnabled)
				{
					this._isEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsEnabled");
				}
			}
		}

		// Token: 0x17000373 RID: 883
		// (get) Token: 0x06000B21 RID: 2849 RVA: 0x000275FC File Offset: 0x000257FC
		// (set) Token: 0x06000B22 RID: 2850 RVA: 0x00027604 File Offset: 0x00025804
		[DataSourceProperty]
		public bool IsMatchFindPossible
		{
			get
			{
				return this._isMatchFindPossible;
			}
			set
			{
				if (value != this._isMatchFindPossible)
				{
					this._isMatchFindPossible = value;
					base.OnPropertyChangedWithValue(value, "IsMatchFindPossible");
				}
			}
		}

		// Token: 0x17000374 RID: 884
		// (get) Token: 0x06000B23 RID: 2851 RVA: 0x00027622 File Offset: 0x00025822
		// (set) Token: 0x06000B24 RID: 2852 RVA: 0x0002762A File Offset: 0x0002582A
		[DataSourceProperty]
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
					base.OnPropertyChangedWithValue(value, "HasUnofficialModulesLoaded");
				}
			}
		}

		// Token: 0x17000375 RID: 885
		// (get) Token: 0x06000B25 RID: 2853 RVA: 0x00027648 File Offset: 0x00025848
		// (set) Token: 0x06000B26 RID: 2854 RVA: 0x00027650 File Offset: 0x00025850
		[DataSourceProperty]
		public string FindGameText
		{
			get
			{
				return this._findGameText;
			}
			set
			{
				if (value != this._findGameText)
				{
					this._findGameText = value;
					base.OnPropertyChangedWithValue<string>(value, "FindGameText");
				}
			}
		}

		// Token: 0x17000376 RID: 886
		// (get) Token: 0x06000B27 RID: 2855 RVA: 0x00027673 File Offset: 0x00025873
		// (set) Token: 0x06000B28 RID: 2856 RVA: 0x0002767B File Offset: 0x0002587B
		[DataSourceProperty]
		public string MatchFindNotPossibleText
		{
			get
			{
				return this._matchFindNotPossibleText;
			}
			set
			{
				if (value != this._matchFindNotPossibleText)
				{
					this._matchFindNotPossibleText = value;
					base.OnPropertyChangedWithValue<string>(value, "MatchFindNotPossibleText");
				}
			}
		}

		// Token: 0x17000377 RID: 887
		// (get) Token: 0x06000B29 RID: 2857 RVA: 0x0002769E File Offset: 0x0002589E
		// (set) Token: 0x06000B2A RID: 2858 RVA: 0x000276A6 File Offset: 0x000258A6
		[DataSourceProperty]
		public string SelectionInfoText
		{
			get
			{
				return this._selectionInfoText;
			}
			set
			{
				if (value != this._selectionInfoText)
				{
					this._selectionInfoText = value;
					base.OnPropertyChangedWithValue<string>(value, "SelectionInfoText");
				}
			}
		}

		// Token: 0x17000378 RID: 888
		// (get) Token: 0x06000B2B RID: 2859 RVA: 0x000276C9 File Offset: 0x000258C9
		// (set) Token: 0x06000B2C RID: 2860 RVA: 0x000276D1 File Offset: 0x000258D1
		[DataSourceProperty]
		public string OpenProfileText
		{
			get
			{
				return this._openProfileText;
			}
			set
			{
				if (value != this._openProfileText)
				{
					this._openProfileText = value;
					base.OnPropertyChangedWithValue<string>(value, "OpenProfileText");
				}
			}
		}

		// Token: 0x17000379 RID: 889
		// (get) Token: 0x06000B2D RID: 2861 RVA: 0x000276F4 File Offset: 0x000258F4
		// (set) Token: 0x06000B2E RID: 2862 RVA: 0x000276FC File Offset: 0x000258FC
		[DataSourceProperty]
		public MPLobbyPlayerBaseVM Player
		{
			get
			{
				return this._player;
			}
			set
			{
				if (value != this._player)
				{
					this._player = value;
					base.OnPropertyChangedWithValue<MPLobbyPlayerBaseVM>(value, "Player");
				}
			}
		}

		// Token: 0x1700037A RID: 890
		// (get) Token: 0x06000B2F RID: 2863 RVA: 0x0002771A File Offset: 0x0002591A
		// (set) Token: 0x06000B30 RID: 2864 RVA: 0x00027722 File Offset: 0x00025922
		[DataSourceProperty]
		public MPNewsVM News
		{
			get
			{
				return this._news;
			}
			set
			{
				if (value != this._news)
				{
					this._news = value;
					base.OnPropertyChangedWithValue<MPNewsVM>(value, "News");
				}
			}
		}

		// Token: 0x04000555 RID: 1365
		private readonly Action<MPLobbyVM.LobbyPage> _onChangePageRequest;

		// Token: 0x04000557 RID: 1367
		private bool _isEnabled;

		// Token: 0x04000558 RID: 1368
		private bool _isMatchFindPossible;

		// Token: 0x04000559 RID: 1369
		private bool _hasUnofficialModulesLoaded;

		// Token: 0x0400055A RID: 1370
		private string _findGameText;

		// Token: 0x0400055B RID: 1371
		private string _matchFindNotPossibleText;

		// Token: 0x0400055C RID: 1372
		private string _selectionInfoText;

		// Token: 0x0400055D RID: 1373
		private string _openProfileText;

		// Token: 0x0400055E RID: 1374
		private MPLobbyPlayerBaseVM _player;

		// Token: 0x0400055F RID: 1375
		private MPNewsVM _news;
	}
}
