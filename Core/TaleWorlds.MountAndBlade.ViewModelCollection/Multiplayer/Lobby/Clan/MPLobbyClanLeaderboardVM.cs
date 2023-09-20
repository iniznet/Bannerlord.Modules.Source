using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Clan
{
	// Token: 0x0200009B RID: 155
	public class MPLobbyClanLeaderboardVM : ViewModel
	{
		// Token: 0x06000E9D RID: 3741 RVA: 0x0003182B File Offset: 0x0002FA2B
		public MPLobbyClanLeaderboardVM()
		{
			this.ClanItems = new MBBindingList<MPLobbyClanItemVM>();
			this.RefreshValues();
		}

		// Token: 0x06000E9E RID: 3742 RVA: 0x00031844 File Offset: 0x0002FA44
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.CloseText = GameTexts.FindText("str_close", null).ToString();
			this.LeaderboardText = new TextObject("{=vGF5S2hE}Leaderboard", null).ToString();
			this.ClansText = new TextObject("{=bfQLwMUp}Clans", null).ToString();
			this.NameText = new TextObject("{=PDdh1sBj}Name", null).ToString();
			this.GamesWonText = new TextObject("{=dxlkHhw5}Games Won", null).ToString();
			this.GamesLostText = new TextObject("{=BrjpmaJH}Games Lost", null).ToString();
			this.NextText = new TextObject("{=Rvr1bcu8}Next", null).ToString();
			this.PreviousText = new TextObject("{=WXAaWZVf}Previous", null).ToString();
		}

		// Token: 0x06000E9F RID: 3743 RVA: 0x00031908 File Offset: 0x0002FB08
		private async void LoadClanLeaderboard()
		{
			this.IsDataLoading = true;
			ClanLeaderboardInfo clanLeaderboardInfo = await NetworkMain.GameClient.GetClanLeaderboardInfo();
			if (clanLeaderboardInfo != null)
			{
				this._clans = clanLeaderboardInfo.ClanEntries;
			}
			else
			{
				this._clans = new ClanLeaderboardEntry[0];
			}
			this.SortController = new MPLobbyClanLeaderboardSortControllerVM(ref this._clans, new Action(this.OnClansSorted));
			this.GoToPage(0);
			this.IsDataLoading = false;
		}

		// Token: 0x06000EA0 RID: 3744 RVA: 0x00031941 File Offset: 0x0002FB41
		private void OnClansSorted()
		{
			this.GoToPage(0);
		}

		// Token: 0x06000EA1 RID: 3745 RVA: 0x0003194C File Offset: 0x0002FB4C
		private void GoToPage(int pageNumber)
		{
			int num = pageNumber * 30;
			if (num > this._clans.Length - 1)
			{
				return;
			}
			this.ClanItems.Clear();
			int num2 = num;
			while (num2 < num + 30 && num2 != this._clans.Length)
			{
				ClanLeaderboardEntry clanLeaderboardEntry = this._clans[num2];
				this.ClanItems.Add(new MPLobbyClanItemVM(clanLeaderboardEntry.Name, clanLeaderboardEntry.Tag, clanLeaderboardEntry.Sigil, clanLeaderboardEntry.WinCount, clanLeaderboardEntry.LossCount, num2 + 1, clanLeaderboardEntry.ClanId.Equals(NetworkMain.GameClient.ClanID)));
				num2++;
			}
			this._currentPageNumber = pageNumber;
		}

		// Token: 0x06000EA2 RID: 3746 RVA: 0x000319EA File Offset: 0x0002FBEA
		private void ExecuteGoToNextPage()
		{
			if (this._currentPageNumber + 1 <= this._clans.Length / 30)
			{
				this.GoToPage(this._currentPageNumber + 1);
				return;
			}
			this.GoToPage(0);
		}

		// Token: 0x06000EA3 RID: 3747 RVA: 0x00031A17 File Offset: 0x0002FC17
		private void ExecuteGoToPreviousPage()
		{
			if (this._currentPageNumber > 0)
			{
				this.GoToPage(this._currentPageNumber - 1);
				return;
			}
			this.GoToPage(this._clans.Length / 30);
		}

		// Token: 0x06000EA4 RID: 3748 RVA: 0x00031A42 File Offset: 0x0002FC42
		public void ExecuteOpenPopup()
		{
			this.IsEnabled = true;
			this.LoadClanLeaderboard();
		}

		// Token: 0x06000EA5 RID: 3749 RVA: 0x00031A51 File Offset: 0x0002FC51
		public void ExecuteClosePopup()
		{
			this.IsEnabled = false;
		}

		// Token: 0x170004AA RID: 1194
		// (get) Token: 0x06000EA6 RID: 3750 RVA: 0x00031A5A File Offset: 0x0002FC5A
		// (set) Token: 0x06000EA7 RID: 3751 RVA: 0x00031A62 File Offset: 0x0002FC62
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

		// Token: 0x170004AB RID: 1195
		// (get) Token: 0x06000EA8 RID: 3752 RVA: 0x00031A80 File Offset: 0x0002FC80
		// (set) Token: 0x06000EA9 RID: 3753 RVA: 0x00031A88 File Offset: 0x0002FC88
		[DataSourceProperty]
		public bool IsDataLoading
		{
			get
			{
				return this._isDataLoading;
			}
			set
			{
				if (value != this._isDataLoading)
				{
					this._isDataLoading = value;
					base.OnPropertyChangedWithValue(value, "IsDataLoading");
				}
			}
		}

		// Token: 0x170004AC RID: 1196
		// (get) Token: 0x06000EAA RID: 3754 RVA: 0x00031AA6 File Offset: 0x0002FCA6
		// (set) Token: 0x06000EAB RID: 3755 RVA: 0x00031AAE File Offset: 0x0002FCAE
		[DataSourceProperty]
		public string LeaderboardText
		{
			get
			{
				return this._leaderboardText;
			}
			set
			{
				if (value != this._leaderboardText)
				{
					this._leaderboardText = value;
					base.OnPropertyChangedWithValue<string>(value, "LeaderboardText");
				}
			}
		}

		// Token: 0x170004AD RID: 1197
		// (get) Token: 0x06000EAC RID: 3756 RVA: 0x00031AD1 File Offset: 0x0002FCD1
		// (set) Token: 0x06000EAD RID: 3757 RVA: 0x00031AD9 File Offset: 0x0002FCD9
		[DataSourceProperty]
		public string ClansText
		{
			get
			{
				return this._clansText;
			}
			set
			{
				if (value != this._clansText)
				{
					this._clansText = value;
					base.OnPropertyChangedWithValue<string>(value, "ClansText");
				}
			}
		}

		// Token: 0x170004AE RID: 1198
		// (get) Token: 0x06000EAE RID: 3758 RVA: 0x00031AFC File Offset: 0x0002FCFC
		// (set) Token: 0x06000EAF RID: 3759 RVA: 0x00031B04 File Offset: 0x0002FD04
		[DataSourceProperty]
		public string NameText
		{
			get
			{
				return this._nameText;
			}
			set
			{
				if (value != this._nameText)
				{
					this._nameText = value;
					base.OnPropertyChangedWithValue<string>(value, "NameText");
				}
			}
		}

		// Token: 0x170004AF RID: 1199
		// (get) Token: 0x06000EB0 RID: 3760 RVA: 0x00031B27 File Offset: 0x0002FD27
		// (set) Token: 0x06000EB1 RID: 3761 RVA: 0x00031B2F File Offset: 0x0002FD2F
		[DataSourceProperty]
		public string GamesWonText
		{
			get
			{
				return this._gamesWonText;
			}
			set
			{
				if (value != this._gamesWonText)
				{
					this._gamesWonText = value;
					base.OnPropertyChangedWithValue<string>(value, "GamesWonText");
				}
			}
		}

		// Token: 0x170004B0 RID: 1200
		// (get) Token: 0x06000EB2 RID: 3762 RVA: 0x00031B52 File Offset: 0x0002FD52
		// (set) Token: 0x06000EB3 RID: 3763 RVA: 0x00031B5A File Offset: 0x0002FD5A
		[DataSourceProperty]
		public string GamesLostText
		{
			get
			{
				return this._gamesLostText;
			}
			set
			{
				if (value != this._gamesLostText)
				{
					this._gamesLostText = value;
					base.OnPropertyChangedWithValue<string>(value, "GamesLostText");
				}
			}
		}

		// Token: 0x170004B1 RID: 1201
		// (get) Token: 0x06000EB4 RID: 3764 RVA: 0x00031B7D File Offset: 0x0002FD7D
		// (set) Token: 0x06000EB5 RID: 3765 RVA: 0x00031B85 File Offset: 0x0002FD85
		[DataSourceProperty]
		public string NextText
		{
			get
			{
				return this._nextText;
			}
			set
			{
				if (value != this._nextText)
				{
					this._nextText = value;
					base.OnPropertyChangedWithValue<string>(value, "NextText");
				}
			}
		}

		// Token: 0x170004B2 RID: 1202
		// (get) Token: 0x06000EB6 RID: 3766 RVA: 0x00031BA8 File Offset: 0x0002FDA8
		// (set) Token: 0x06000EB7 RID: 3767 RVA: 0x00031BB0 File Offset: 0x0002FDB0
		[DataSourceProperty]
		public string PreviousText
		{
			get
			{
				return this._previousText;
			}
			set
			{
				if (value != this._previousText)
				{
					this._previousText = value;
					base.OnPropertyChangedWithValue<string>(value, "PreviousText");
				}
			}
		}

		// Token: 0x170004B3 RID: 1203
		// (get) Token: 0x06000EB8 RID: 3768 RVA: 0x00031BD3 File Offset: 0x0002FDD3
		// (set) Token: 0x06000EB9 RID: 3769 RVA: 0x00031BDB File Offset: 0x0002FDDB
		[DataSourceProperty]
		public string CloseText
		{
			get
			{
				return this._closeText;
			}
			set
			{
				if (value != this._closeText)
				{
					this._closeText = value;
					base.OnPropertyChangedWithValue<string>(value, "CloseText");
				}
			}
		}

		// Token: 0x170004B4 RID: 1204
		// (get) Token: 0x06000EBA RID: 3770 RVA: 0x00031BFE File Offset: 0x0002FDFE
		// (set) Token: 0x06000EBB RID: 3771 RVA: 0x00031C06 File Offset: 0x0002FE06
		[DataSourceProperty]
		public MBBindingList<MPLobbyClanItemVM> ClanItems
		{
			get
			{
				return this._clanItems;
			}
			set
			{
				if (value != this._clanItems)
				{
					this._clanItems = value;
					base.OnPropertyChangedWithValue<MBBindingList<MPLobbyClanItemVM>>(value, "ClanItems");
				}
			}
		}

		// Token: 0x170004B5 RID: 1205
		// (get) Token: 0x06000EBC RID: 3772 RVA: 0x00031C24 File Offset: 0x0002FE24
		// (set) Token: 0x06000EBD RID: 3773 RVA: 0x00031C2C File Offset: 0x0002FE2C
		[DataSourceProperty]
		public MPLobbyClanLeaderboardSortControllerVM SortController
		{
			get
			{
				return this._sortController;
			}
			set
			{
				if (value != this._sortController)
				{
					this._sortController = value;
					base.OnPropertyChangedWithValue<MPLobbyClanLeaderboardSortControllerVM>(value, "SortController");
				}
			}
		}

		// Token: 0x040006F1 RID: 1777
		private ClanLeaderboardEntry[] _clans;

		// Token: 0x040006F2 RID: 1778
		private const int _clansPerPage = 30;

		// Token: 0x040006F3 RID: 1779
		private int _currentPageNumber;

		// Token: 0x040006F4 RID: 1780
		private bool _isEnabled;

		// Token: 0x040006F5 RID: 1781
		private bool _isDataLoading;

		// Token: 0x040006F6 RID: 1782
		private string _leaderboardText;

		// Token: 0x040006F7 RID: 1783
		private string _clansText;

		// Token: 0x040006F8 RID: 1784
		private string _nameText;

		// Token: 0x040006F9 RID: 1785
		private string _gamesWonText;

		// Token: 0x040006FA RID: 1786
		private string _gamesLostText;

		// Token: 0x040006FB RID: 1787
		private string _nextText;

		// Token: 0x040006FC RID: 1788
		private string _previousText;

		// Token: 0x040006FD RID: 1789
		private string _closeText;

		// Token: 0x040006FE RID: 1790
		private MBBindingList<MPLobbyClanItemVM> _clanItems;

		// Token: 0x040006FF RID: 1791
		private MPLobbyClanLeaderboardSortControllerVM _sortController;
	}
}
