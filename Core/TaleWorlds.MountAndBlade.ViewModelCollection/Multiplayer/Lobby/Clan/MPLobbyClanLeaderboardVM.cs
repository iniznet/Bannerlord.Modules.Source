using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Clan
{
	public class MPLobbyClanLeaderboardVM : ViewModel
	{
		public MPLobbyClanLeaderboardVM()
		{
			this.ClanItems = new MBBindingList<MPLobbyClanItemVM>();
			this.RefreshValues();
		}

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

		private void OnClansSorted()
		{
			this.GoToPage(0);
		}

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

		private void ExecuteGoToNextPage()
		{
			if (this._currentPageNumber + 1 <= this._clans.Length / 30)
			{
				this.GoToPage(this._currentPageNumber + 1);
				return;
			}
			this.GoToPage(0);
		}

		private void ExecuteGoToPreviousPage()
		{
			if (this._currentPageNumber > 0)
			{
				this.GoToPage(this._currentPageNumber - 1);
				return;
			}
			this.GoToPage(this._clans.Length / 30);
		}

		public void ExecuteOpenPopup()
		{
			this.IsEnabled = true;
			this.LoadClanLeaderboard();
		}

		public void ExecuteClosePopup()
		{
			this.IsEnabled = false;
		}

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

		private ClanLeaderboardEntry[] _clans;

		private const int _clansPerPage = 30;

		private int _currentPageNumber;

		private bool _isEnabled;

		private bool _isDataLoading;

		private string _leaderboardText;

		private string _clansText;

		private string _nameText;

		private string _gamesWonText;

		private string _gamesLostText;

		private string _nextText;

		private string _previousText;

		private string _closeText;

		private MBBindingList<MPLobbyClanItemVM> _clanItems;

		private MPLobbyClanLeaderboardSortControllerVM _sortController;
	}
}
