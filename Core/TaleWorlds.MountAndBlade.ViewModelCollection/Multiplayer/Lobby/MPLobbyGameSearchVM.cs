using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.CustomGame;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby
{
	// Token: 0x0200005B RID: 91
	public class MPLobbyGameSearchVM : ViewModel
	{
		// Token: 0x1700025D RID: 605
		// (get) Token: 0x060007AB RID: 1963 RVA: 0x0001E0EB File Offset: 0x0001C2EB
		// (set) Token: 0x060007AC RID: 1964 RVA: 0x0001E0F3 File Offset: 0x0001C2F3
		public MPCustomGameVM.CustomGameMode CustomGameMode { get; private set; }

		// Token: 0x060007AD RID: 1965 RVA: 0x0001E0FC File Offset: 0x0001C2FC
		public MPLobbyGameSearchVM()
		{
			this.GameTypesText = new TextObject("{=cK5DE88I}N/A", null).ToString();
			this.RefreshValues();
		}

		// Token: 0x060007AE RID: 1966 RVA: 0x0001E138 File Offset: 0x0001C338
		public override void RefreshValues()
		{
			base.RefreshValues();
			if (this.CustomGameMode == MPCustomGameVM.CustomGameMode.PremadeGame)
			{
				this.TitleText = new TextObject("{=dkPL25g9}Waiting for an opponent team", null).ToString();
				this.GameTypesText = "";
				this.ShowStats = false;
			}
			else
			{
				this.TitleText = new TextObject("{=FD7EQDmW}Looking for game", null).ToString();
				this.ShowStats = true;
			}
			GameTexts.SetVariable("STR1", "");
			GameTexts.SetVariable("STR2", new TextObject("{=mFMPj9zg}Searching for matches", null));
			this.CurrentWaitingTimeDescription = GameTexts.FindText("str_STR1_space_STR2", null).ToString();
			GameTexts.SetVariable("STR2", new TextObject("{=18yFEEIL}Estimated wait time", null));
			this.AverageWaitingTimeDescription = GameTexts.FindText("str_STR1_space_STR2", null).ToString();
			this.CancelText = new TextObject("{=3CpNUnVl}Cancel", null).ToString();
		}

		// Token: 0x060007AF RID: 1967 RVA: 0x0001E216 File Offset: 0x0001C416
		public void OnTick(float dt)
		{
			if (this.IsEnabled)
			{
				this._waitingTimeElapsed += dt;
				this.CurrentWaitingTime = this.SecondsToString(this._waitingTimeElapsed);
			}
		}

		// Token: 0x060007B0 RID: 1968 RVA: 0x0001E240 File Offset: 0x0001C440
		public void SetEnabled(bool enabled)
		{
			this.IsEnabled = enabled;
			if (enabled)
			{
				this.CanCancelSearch = true;
				this._waitingTimeElapsed = 0f;
			}
			this.RefreshValues();
			if (this.CustomGameMode != MPCustomGameVM.CustomGameMode.PremadeGame)
			{
				this.UpdateCanCancel();
			}
		}

		// Token: 0x060007B1 RID: 1969 RVA: 0x0001E274 File Offset: 0x0001C474
		public void UpdateData(MatchmakingWaitTimeStats matchmakingWaitTimeStats, string[] gameTypeInfo)
		{
			this.ShowStats = true;
			this.CustomGameMode = MPCustomGameVM.CustomGameMode.CustomServer;
			this.TitleText = new TextObject("{=FD7EQDmW}Looking for game", null).ToString();
			int num = 0;
			string[] array = this.GameTypesText.Replace(" ", "").Split(new char[] { ',' });
			foreach (string text in array)
			{
				WaitTimeStatType waitTimeStatType = WaitTimeStatType.SoloDuo;
				if (NetworkMain.GameClient.PlayersInParty.Count >= 3 && NetworkMain.GameClient.PlayersInParty.Count <= 5)
				{
					waitTimeStatType = WaitTimeStatType.Party;
				}
				else if (NetworkMain.GameClient.IsPartyFull)
				{
					waitTimeStatType = WaitTimeStatType.Premade;
				}
				num += matchmakingWaitTimeStats.GetWaitTime(NetworkMain.GetUserCurrentRegion(), text, waitTimeStatType);
			}
			this.AverageWaitingTime = this.SecondsToString((float)(num / array.Length));
			if (gameTypeInfo != null)
			{
				this.GameTypesText = MPLobbyVM.GetLocalizedGameTypesString(gameTypeInfo);
			}
		}

		// Token: 0x060007B2 RID: 1970 RVA: 0x0001E350 File Offset: 0x0001C550
		public void UpdatePremadeGameData()
		{
			this.ShowStats = false;
			this.CustomGameMode = MPCustomGameVM.CustomGameMode.PremadeGame;
			this.TitleText = new TextObject("{=dkPL25g9}Waiting for an opponent team", null).ToString();
			this.GameTypesText = "";
		}

		// Token: 0x060007B3 RID: 1971 RVA: 0x0001E381 File Offset: 0x0001C581
		public void OnJoinPremadeGameRequestSuccessful()
		{
			this.TitleText = new TextObject("{=5coyTZOI}Game is starting!", null).ToString();
		}

		// Token: 0x060007B4 RID: 1972 RVA: 0x0001E399 File Offset: 0x0001C599
		public void OnRequestedToCancelSearchBattle()
		{
			this.CanCancelSearch = false;
		}

		// Token: 0x060007B5 RID: 1973 RVA: 0x0001E3A2 File Offset: 0x0001C5A2
		public void UpdateCanCancel()
		{
			this.CanCancelSearch = !NetworkMain.GameClient.IsInParty || NetworkMain.GameClient.IsPartyLeader;
		}

		// Token: 0x060007B6 RID: 1974 RVA: 0x0001E3C3 File Offset: 0x0001C5C3
		private void ExecuteCancel()
		{
			if (this.CustomGameMode != MPCustomGameVM.CustomGameMode.PremadeGame)
			{
				NetworkMain.GameClient.CancelFindGame();
				return;
			}
			NetworkMain.GameClient.CancelCreatingPremadeGame();
		}

		// Token: 0x060007B7 RID: 1975 RVA: 0x0001E3E4 File Offset: 0x0001C5E4
		private string SecondsToString(float seconds)
		{
			return TimeSpan.FromSeconds((double)seconds).ToString((seconds >= 3600f) ? this._longTimeTextFormat : this._shortTimeTextFormat);
		}

		// Token: 0x1700025E RID: 606
		// (get) Token: 0x060007B8 RID: 1976 RVA: 0x0001E416 File Offset: 0x0001C616
		// (set) Token: 0x060007B9 RID: 1977 RVA: 0x0001E41E File Offset: 0x0001C61E
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

		// Token: 0x1700025F RID: 607
		// (get) Token: 0x060007BA RID: 1978 RVA: 0x0001E43C File Offset: 0x0001C63C
		// (set) Token: 0x060007BB RID: 1979 RVA: 0x0001E444 File Offset: 0x0001C644
		[DataSourceProperty]
		public bool CanCancelSearch
		{
			get
			{
				return this._canCancelSearch;
			}
			set
			{
				if (value != this._canCancelSearch)
				{
					this._canCancelSearch = value;
					base.OnPropertyChangedWithValue(value, "CanCancelSearch");
				}
			}
		}

		// Token: 0x17000260 RID: 608
		// (get) Token: 0x060007BC RID: 1980 RVA: 0x0001E462 File Offset: 0x0001C662
		// (set) Token: 0x060007BD RID: 1981 RVA: 0x0001E46A File Offset: 0x0001C66A
		[DataSourceProperty]
		public bool ShowStats
		{
			get
			{
				return this._showStats;
			}
			set
			{
				if (value != this._showStats)
				{
					this._showStats = value;
					base.OnPropertyChangedWithValue(value, "ShowStats");
				}
			}
		}

		// Token: 0x17000261 RID: 609
		// (get) Token: 0x060007BE RID: 1982 RVA: 0x0001E488 File Offset: 0x0001C688
		// (set) Token: 0x060007BF RID: 1983 RVA: 0x0001E490 File Offset: 0x0001C690
		[DataSourceProperty]
		public string TitleText
		{
			get
			{
				return this._titleText;
			}
			set
			{
				if (value != this._titleText)
				{
					this._titleText = value;
					base.OnPropertyChangedWithValue<string>(value, "TitleText");
				}
			}
		}

		// Token: 0x17000262 RID: 610
		// (get) Token: 0x060007C0 RID: 1984 RVA: 0x0001E4B3 File Offset: 0x0001C6B3
		// (set) Token: 0x060007C1 RID: 1985 RVA: 0x0001E4BB File Offset: 0x0001C6BB
		[DataSourceProperty]
		public string GameTypesText
		{
			get
			{
				return this._gameTypesText;
			}
			set
			{
				if (value != this._gameTypesText)
				{
					this._gameTypesText = value;
					base.OnPropertyChangedWithValue<string>(value, "GameTypesText");
				}
			}
		}

		// Token: 0x17000263 RID: 611
		// (get) Token: 0x060007C2 RID: 1986 RVA: 0x0001E4DE File Offset: 0x0001C6DE
		// (set) Token: 0x060007C3 RID: 1987 RVA: 0x0001E4E6 File Offset: 0x0001C6E6
		[DataSourceProperty]
		public string CancelText
		{
			get
			{
				return this._cancelText;
			}
			set
			{
				if (value != this._cancelText)
				{
					this._cancelText = value;
					base.OnPropertyChangedWithValue<string>(value, "CancelText");
				}
			}
		}

		// Token: 0x17000264 RID: 612
		// (get) Token: 0x060007C4 RID: 1988 RVA: 0x0001E509 File Offset: 0x0001C709
		// (set) Token: 0x060007C5 RID: 1989 RVA: 0x0001E511 File Offset: 0x0001C711
		[DataSourceProperty]
		public string AverageWaitingTime
		{
			get
			{
				return this._averageWaitingTime;
			}
			set
			{
				if (value != this._averageWaitingTime)
				{
					this._averageWaitingTime = value;
					base.OnPropertyChangedWithValue<string>(value, "AverageWaitingTime");
				}
			}
		}

		// Token: 0x17000265 RID: 613
		// (get) Token: 0x060007C6 RID: 1990 RVA: 0x0001E534 File Offset: 0x0001C734
		// (set) Token: 0x060007C7 RID: 1991 RVA: 0x0001E53C File Offset: 0x0001C73C
		[DataSourceProperty]
		public string AverageWaitingTimeDescription
		{
			get
			{
				return this._averageWaitingTimeDescription;
			}
			set
			{
				if (value != this._averageWaitingTimeDescription)
				{
					this._averageWaitingTimeDescription = value;
					base.OnPropertyChangedWithValue<string>(value, "AverageWaitingTimeDescription");
				}
			}
		}

		// Token: 0x17000266 RID: 614
		// (get) Token: 0x060007C8 RID: 1992 RVA: 0x0001E55F File Offset: 0x0001C75F
		// (set) Token: 0x060007C9 RID: 1993 RVA: 0x0001E567 File Offset: 0x0001C767
		[DataSourceProperty]
		public string CurrentWaitingTime
		{
			get
			{
				return this._currentWaitingTime;
			}
			set
			{
				if (value != this._currentWaitingTime)
				{
					this._currentWaitingTime = value;
					base.OnPropertyChangedWithValue<string>(value, "CurrentWaitingTime");
				}
			}
		}

		// Token: 0x17000267 RID: 615
		// (get) Token: 0x060007CA RID: 1994 RVA: 0x0001E58A File Offset: 0x0001C78A
		// (set) Token: 0x060007CB RID: 1995 RVA: 0x0001E592 File Offset: 0x0001C792
		[DataSourceProperty]
		public string CurrentWaitingTimeDescription
		{
			get
			{
				return this._currentWaitingTimeDescription;
			}
			set
			{
				if (value != this._currentWaitingTimeDescription)
				{
					this._currentWaitingTimeDescription = value;
					base.OnPropertyChangedWithValue<string>(value, "CurrentWaitingTimeDescription");
				}
			}
		}

		// Token: 0x040003E9 RID: 1001
		private float _waitingTimeElapsed;

		// Token: 0x040003EA RID: 1002
		private string _shortTimeTextFormat = "mm\\:ss";

		// Token: 0x040003EB RID: 1003
		private string _longTimeTextFormat = "hh\\:mm\\:ss";

		// Token: 0x040003EC RID: 1004
		private bool _isEnabled;

		// Token: 0x040003ED RID: 1005
		private bool _canCancelSearch;

		// Token: 0x040003EE RID: 1006
		private bool _showStats;

		// Token: 0x040003EF RID: 1007
		private string _titleText;

		// Token: 0x040003F0 RID: 1008
		private string _gameTypesText;

		// Token: 0x040003F1 RID: 1009
		private string _cancelText;

		// Token: 0x040003F2 RID: 1010
		private string _averageWaitingTime;

		// Token: 0x040003F3 RID: 1011
		private string _averageWaitingTimeDescription;

		// Token: 0x040003F4 RID: 1012
		private string _currentWaitingTime;

		// Token: 0x040003F5 RID: 1013
		private string _currentWaitingTimeDescription;
	}
}
