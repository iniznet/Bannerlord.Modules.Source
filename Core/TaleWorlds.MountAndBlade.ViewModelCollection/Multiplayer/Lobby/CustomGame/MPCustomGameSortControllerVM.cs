using System;
using System.Collections.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.CustomGame
{
	// Token: 0x0200008D RID: 141
	public class MPCustomGameSortControllerVM : ViewModel
	{
		// Token: 0x17000413 RID: 1043
		// (get) Token: 0x06000CFA RID: 3322 RVA: 0x0002D55B File Offset: 0x0002B75B
		// (set) Token: 0x06000CFB RID: 3323 RVA: 0x0002D563 File Offset: 0x0002B763
		public MPCustomGameSortControllerVM.CustomServerSortOption? CurrentSortOption { get; private set; }

		// Token: 0x06000CFC RID: 3324 RVA: 0x0002D56C File Offset: 0x0002B76C
		public MPCustomGameSortControllerVM(ref MBBindingList<MPCustomGameItemVM> listToControl, MPCustomGameVM.CustomGameMode customGameMode)
		{
			this._listToControl = listToControl;
			this.IsPremadeMatchesList = customGameMode == MPCustomGameVM.CustomGameMode.PremadeGame;
			this.IsPingInfoAvailable = MPCustomGameVM.IsPingInfoAvailable && !this.IsPremadeMatchesList;
			this._serverNameComparer = new MPCustomGameSortControllerVM.ServerNameComparer();
			this._gameTypeComparer = new MPCustomGameSortControllerVM.GameTypeComparer();
			this._playerCountComparer = new MPCustomGameSortControllerVM.PlayerCountComparer();
			this._passwordComparer = new MPCustomGameSortControllerVM.PasswordComparer();
			this._firstFactionComparer = new MPCustomGameSortControllerVM.FirstFactionComparer();
			this._secondFactionComparer = new MPCustomGameSortControllerVM.SecondFactionComparer();
			this._regionComparer = new MPCustomGameSortControllerVM.RegionComparer();
			this._premadeMatchTypeComparer = new MPCustomGameSortControllerVM.PremadeMatchTypeComparer();
			this._hostComparer = new MPCustomGameSortControllerVM.HostComparer();
			this._pingComparer = new MPCustomGameSortControllerVM.PingComparer();
			this.RefreshValues();
		}

		// Token: 0x06000CFD RID: 3325 RVA: 0x0002D620 File Offset: 0x0002B820
		public void SetSortOption(MPCustomGameSortControllerVM.CustomServerSortOption? sortOption)
		{
			MPCustomGameSortControllerVM.CustomServerSortOption? customServerSortOption = sortOption;
			if (customServerSortOption != null)
			{
				switch (customServerSortOption.GetValueOrDefault())
				{
				case MPCustomGameSortControllerVM.CustomServerSortOption.Name:
					this.ExecuteSortByServerName();
					return;
				case MPCustomGameSortControllerVM.CustomServerSortOption.GameType:
					this.ExecuteSortByGameType();
					return;
				case MPCustomGameSortControllerVM.CustomServerSortOption.PlayerCount:
					this.ExecuteSortByPlayerCount();
					return;
				case MPCustomGameSortControllerVM.CustomServerSortOption.PasswordProtection:
					this.ExecuteSortByPassword();
					return;
				case MPCustomGameSortControllerVM.CustomServerSortOption.FirstFaction:
					this.ExecuteSortByFirstFaction();
					return;
				case MPCustomGameSortControllerVM.CustomServerSortOption.SecondFaction:
					this.ExecuteSortBySecondFaction();
					return;
				case MPCustomGameSortControllerVM.CustomServerSortOption.Region:
					this.ExecuteSortByRegion();
					return;
				case MPCustomGameSortControllerVM.CustomServerSortOption.PremadeMatchType:
					this.ExecuteSortByPremadeMatchType();
					return;
				case MPCustomGameSortControllerVM.CustomServerSortOption.Host:
					this.ExecuteSortByHost();
					return;
				case MPCustomGameSortControllerVM.CustomServerSortOption.Ping:
					this.ExecuteSortByPing();
					break;
				default:
					return;
				}
			}
		}

		// Token: 0x06000CFE RID: 3326 RVA: 0x0002D6B4 File Offset: 0x0002B8B4
		public void SortByCurrentState()
		{
			if (this.IsServerNameSelected)
			{
				this._listToControl.Sort(this._serverNameComparer);
				return;
			}
			if (this.IsGameTypeSelected)
			{
				this._listToControl.Sort(this._gameTypeComparer);
				return;
			}
			if (this.IsPlayerCountSelected)
			{
				this._listToControl.Sort(this._playerCountComparer);
				return;
			}
			if (this.IsPasswordSelected)
			{
				this._listToControl.Sort(this._passwordComparer);
				return;
			}
			if (this.IsFirstFactionSelected)
			{
				this._listToControl.Sort(this._firstFactionComparer);
				return;
			}
			if (this.IsSecondFactionSelected)
			{
				this._listToControl.Sort(this._secondFactionComparer);
				return;
			}
			if (this.IsRegionSelected)
			{
				this._listToControl.Sort(this._regionComparer);
				return;
			}
			if (this.IsPremadeMatchTypeSelected)
			{
				this._listToControl.Sort(this._premadeMatchTypeComparer);
			}
		}

		// Token: 0x06000CFF RID: 3327 RVA: 0x0002D790 File Offset: 0x0002B990
		private void ExecuteSortByServerName()
		{
			int serverNameState = this.ServerNameState;
			this.SetAllStates(MPCustomGameSortControllerVM.SortState.Default);
			this.ServerNameState = (serverNameState + 1) % 3;
			if (this.ServerNameState == 0)
			{
				this.ServerNameState++;
			}
			this._serverNameComparer.SetSortMode(this.ServerNameState == 1);
			this._listToControl.Sort(this._serverNameComparer);
			this.IsServerNameSelected = true;
			this.CurrentSortOption = new MPCustomGameSortControllerVM.CustomServerSortOption?(MPCustomGameSortControllerVM.CustomServerSortOption.Name);
		}

		// Token: 0x06000D00 RID: 3328 RVA: 0x0002D804 File Offset: 0x0002BA04
		private void ExecuteSortByGameType()
		{
			int gameTypeState = this.GameTypeState;
			this.SetAllStates(MPCustomGameSortControllerVM.SortState.Default);
			this.GameTypeState = (gameTypeState + 1) % 3;
			if (this.GameTypeState == 0)
			{
				this.GameTypeState++;
			}
			this._gameTypeComparer.SetSortMode(this.GameTypeState == 1);
			this._listToControl.Sort(this._gameTypeComparer);
			this.IsGameTypeSelected = true;
			this.CurrentSortOption = new MPCustomGameSortControllerVM.CustomServerSortOption?(MPCustomGameSortControllerVM.CustomServerSortOption.GameType);
		}

		// Token: 0x06000D01 RID: 3329 RVA: 0x0002D878 File Offset: 0x0002BA78
		private void ExecuteSortByPlayerCount()
		{
			int playerCountState = this.PlayerCountState;
			this.SetAllStates(MPCustomGameSortControllerVM.SortState.Default);
			this.PlayerCountState = (playerCountState + 1) % 3;
			if (this.PlayerCountState == 0)
			{
				this.PlayerCountState++;
			}
			this._playerCountComparer.SetSortMode(this.PlayerCountState == 1);
			this._listToControl.Sort(this._playerCountComparer);
			this.IsPlayerCountSelected = true;
			this.CurrentSortOption = new MPCustomGameSortControllerVM.CustomServerSortOption?(MPCustomGameSortControllerVM.CustomServerSortOption.PlayerCount);
		}

		// Token: 0x06000D02 RID: 3330 RVA: 0x0002D8EC File Offset: 0x0002BAEC
		private void ExecuteSortByPassword()
		{
			int passwordState = this.PasswordState;
			this.SetAllStates(MPCustomGameSortControllerVM.SortState.Default);
			this.PasswordState = (passwordState + 1) % 3;
			if (this.PasswordState == 0)
			{
				this.PasswordState++;
			}
			this._passwordComparer.SetSortMode(this.PasswordState == 1);
			this._listToControl.Sort(this._passwordComparer);
			this.IsPasswordSelected = true;
			this.CurrentSortOption = new MPCustomGameSortControllerVM.CustomServerSortOption?(MPCustomGameSortControllerVM.CustomServerSortOption.PasswordProtection);
		}

		// Token: 0x06000D03 RID: 3331 RVA: 0x0002D960 File Offset: 0x0002BB60
		private void ExecuteSortByFirstFaction()
		{
			int firstFactionState = this.FirstFactionState;
			this.SetAllStates(MPCustomGameSortControllerVM.SortState.Default);
			this.FirstFactionState = (firstFactionState + 1) % 3;
			if (this.FirstFactionState == 0)
			{
				this.FirstFactionState++;
			}
			this._firstFactionComparer.SetSortMode(this.FirstFactionState == 1);
			this._listToControl.Sort(this._firstFactionComparer);
			this.IsFirstFactionSelected = true;
			this.CurrentSortOption = new MPCustomGameSortControllerVM.CustomServerSortOption?(MPCustomGameSortControllerVM.CustomServerSortOption.FirstFaction);
		}

		// Token: 0x06000D04 RID: 3332 RVA: 0x0002D9D4 File Offset: 0x0002BBD4
		private void ExecuteSortBySecondFaction()
		{
			int secondFactionState = this.SecondFactionState;
			this.SetAllStates(MPCustomGameSortControllerVM.SortState.Default);
			this.SecondFactionState = (secondFactionState + 1) % 3;
			if (this.SecondFactionState == 0)
			{
				this.SecondFactionState++;
			}
			this._secondFactionComparer.SetSortMode(this.SecondFactionState == 1);
			this._listToControl.Sort(this._secondFactionComparer);
			this.IsSecondFactionSelected = true;
			this.CurrentSortOption = new MPCustomGameSortControllerVM.CustomServerSortOption?(MPCustomGameSortControllerVM.CustomServerSortOption.SecondFaction);
		}

		// Token: 0x06000D05 RID: 3333 RVA: 0x0002DA48 File Offset: 0x0002BC48
		private void ExecuteSortByRegion()
		{
			int regionState = this.RegionState;
			this.SetAllStates(MPCustomGameSortControllerVM.SortState.Default);
			this.RegionState = (regionState + 1) % 3;
			if (this.RegionState == 0)
			{
				this.RegionState++;
			}
			this._regionComparer.SetSortMode(this.RegionState == 1);
			this._listToControl.Sort(this._regionComparer);
			this.IsRegionSelected = true;
			this.CurrentSortOption = new MPCustomGameSortControllerVM.CustomServerSortOption?(MPCustomGameSortControllerVM.CustomServerSortOption.Region);
		}

		// Token: 0x06000D06 RID: 3334 RVA: 0x0002DABC File Offset: 0x0002BCBC
		private void ExecuteSortByPremadeMatchType()
		{
			int premadeMatchTypeState = this.PremadeMatchTypeState;
			this.SetAllStates(MPCustomGameSortControllerVM.SortState.Default);
			this.PremadeMatchTypeState = (premadeMatchTypeState + 1) % 3;
			if (this.PremadeMatchTypeState == 0)
			{
				this.PremadeMatchTypeState++;
			}
			this._premadeMatchTypeComparer.SetSortMode(this.PremadeMatchTypeState == 1);
			this._listToControl.Sort(this._premadeMatchTypeComparer);
			this.IsPremadeMatchTypeSelected = true;
			this.CurrentSortOption = new MPCustomGameSortControllerVM.CustomServerSortOption?(MPCustomGameSortControllerVM.CustomServerSortOption.PremadeMatchType);
		}

		// Token: 0x06000D07 RID: 3335 RVA: 0x0002DB30 File Offset: 0x0002BD30
		private void ExecuteSortByHost()
		{
			int hostState = this.HostState;
			this.SetAllStates(MPCustomGameSortControllerVM.SortState.Default);
			this.HostState = (hostState + 1) % 3;
			if (this.HostState == 0)
			{
				this.HostState++;
			}
			this._hostComparer.SetSortMode(this.HostState == 1);
			this._listToControl.Sort(this._hostComparer);
			this.IsHostSelected = true;
			this.CurrentSortOption = new MPCustomGameSortControllerVM.CustomServerSortOption?(MPCustomGameSortControllerVM.CustomServerSortOption.Host);
		}

		// Token: 0x06000D08 RID: 3336 RVA: 0x0002DBA4 File Offset: 0x0002BDA4
		private void ExecuteSortByPing()
		{
			int pingState = this.PingState;
			this.SetAllStates(MPCustomGameSortControllerVM.SortState.Default);
			this.PingState = (pingState + 1) % 3;
			if (this.PingState == 0)
			{
				this.PingState++;
			}
			this._pingComparer.SetSortMode(this.PingState == 1);
			this._listToControl.Sort(this._pingComparer);
			this.IsPingSelected = true;
			this.CurrentSortOption = new MPCustomGameSortControllerVM.CustomServerSortOption?(MPCustomGameSortControllerVM.CustomServerSortOption.Ping);
		}

		// Token: 0x06000D09 RID: 3337 RVA: 0x0002DC1C File Offset: 0x0002BE1C
		private void SetAllStates(MPCustomGameSortControllerVM.SortState state)
		{
			this.ServerNameState = (int)state;
			this.GameTypeState = (int)state;
			this.PlayerCountState = (int)state;
			this.PasswordState = (int)state;
			this.FirstFactionState = (int)state;
			this.SecondFactionState = (int)state;
			this.RegionState = (int)state;
			this.PremadeMatchTypeState = (int)state;
			this.HostState = (int)state;
			this.PingState = (int)state;
			this.IsServerNameSelected = false;
			this.IsGameTypeSelected = false;
			this.IsPlayerCountSelected = false;
			this.IsPasswordSelected = false;
			this.IsFirstFactionSelected = false;
			this.IsSecondFactionSelected = false;
			this.IsRegionSelected = false;
			this.IsPremadeMatchTypeSelected = false;
			this.IsHostSelected = false;
			this.IsPingSelected = false;
		}

		// Token: 0x17000414 RID: 1044
		// (get) Token: 0x06000D0A RID: 3338 RVA: 0x0002DCB5 File Offset: 0x0002BEB5
		// (set) Token: 0x06000D0B RID: 3339 RVA: 0x0002DCBD File Offset: 0x0002BEBD
		[DataSourceProperty]
		public bool IsPremadeMatchesList
		{
			get
			{
				return this._isPremadeMatchesList;
			}
			set
			{
				if (value != this._isPremadeMatchesList)
				{
					this._isPremadeMatchesList = value;
					base.OnPropertyChanged("IsPremadeMatchesList");
				}
			}
		}

		// Token: 0x17000415 RID: 1045
		// (get) Token: 0x06000D0C RID: 3340 RVA: 0x0002DCDA File Offset: 0x0002BEDA
		// (set) Token: 0x06000D0D RID: 3341 RVA: 0x0002DCE2 File Offset: 0x0002BEE2
		[DataSourceProperty]
		public bool IsPingInfoAvailable
		{
			get
			{
				return this._isPingInfoAvailable;
			}
			set
			{
				if (value != this._isPingInfoAvailable)
				{
					this._isPingInfoAvailable = value;
					base.OnPropertyChanged("IsPingInfoAvailable");
				}
			}
		}

		// Token: 0x17000416 RID: 1046
		// (get) Token: 0x06000D0E RID: 3342 RVA: 0x0002DCFF File Offset: 0x0002BEFF
		// (set) Token: 0x06000D0F RID: 3343 RVA: 0x0002DD07 File Offset: 0x0002BF07
		[DataSourceProperty]
		public int ServerNameState
		{
			get
			{
				return this._serverNameState;
			}
			set
			{
				if (value != this._serverNameState)
				{
					this._serverNameState = value;
					base.OnPropertyChanged("ServerNameState");
				}
			}
		}

		// Token: 0x17000417 RID: 1047
		// (get) Token: 0x06000D10 RID: 3344 RVA: 0x0002DD24 File Offset: 0x0002BF24
		// (set) Token: 0x06000D11 RID: 3345 RVA: 0x0002DD2C File Offset: 0x0002BF2C
		[DataSourceProperty]
		public int GameTypeState
		{
			get
			{
				return this._gameTypeState;
			}
			set
			{
				if (value != this._gameTypeState)
				{
					this._gameTypeState = value;
					base.OnPropertyChanged("GameTypeState");
				}
			}
		}

		// Token: 0x17000418 RID: 1048
		// (get) Token: 0x06000D12 RID: 3346 RVA: 0x0002DD49 File Offset: 0x0002BF49
		// (set) Token: 0x06000D13 RID: 3347 RVA: 0x0002DD51 File Offset: 0x0002BF51
		[DataSourceProperty]
		public int PlayerCountState
		{
			get
			{
				return this._playerCountState;
			}
			set
			{
				if (value != this._playerCountState)
				{
					this._playerCountState = value;
					base.OnPropertyChanged("PlayerCountState");
				}
			}
		}

		// Token: 0x17000419 RID: 1049
		// (get) Token: 0x06000D14 RID: 3348 RVA: 0x0002DD6E File Offset: 0x0002BF6E
		// (set) Token: 0x06000D15 RID: 3349 RVA: 0x0002DD76 File Offset: 0x0002BF76
		[DataSourceProperty]
		public int PasswordState
		{
			get
			{
				return this._passwordState;
			}
			set
			{
				if (value != this._passwordState)
				{
					this._passwordState = value;
					base.OnPropertyChanged("PasswordState");
				}
			}
		}

		// Token: 0x1700041A RID: 1050
		// (get) Token: 0x06000D16 RID: 3350 RVA: 0x0002DD93 File Offset: 0x0002BF93
		// (set) Token: 0x06000D17 RID: 3351 RVA: 0x0002DD9B File Offset: 0x0002BF9B
		[DataSourceProperty]
		public int FirstFactionState
		{
			get
			{
				return this._firstFactionState;
			}
			set
			{
				if (value != this._firstFactionState)
				{
					this._firstFactionState = value;
					base.OnPropertyChanged("FirstFactionState");
				}
			}
		}

		// Token: 0x1700041B RID: 1051
		// (get) Token: 0x06000D18 RID: 3352 RVA: 0x0002DDB8 File Offset: 0x0002BFB8
		// (set) Token: 0x06000D19 RID: 3353 RVA: 0x0002DDC0 File Offset: 0x0002BFC0
		[DataSourceProperty]
		public int SecondFactionState
		{
			get
			{
				return this._secondFactionState;
			}
			set
			{
				if (value != this._secondFactionState)
				{
					this._secondFactionState = value;
					base.OnPropertyChanged("SecondFactionState");
				}
			}
		}

		// Token: 0x1700041C RID: 1052
		// (get) Token: 0x06000D1A RID: 3354 RVA: 0x0002DDDD File Offset: 0x0002BFDD
		// (set) Token: 0x06000D1B RID: 3355 RVA: 0x0002DDE5 File Offset: 0x0002BFE5
		[DataSourceProperty]
		public int RegionState
		{
			get
			{
				return this._regionState;
			}
			set
			{
				if (value != this._regionState)
				{
					this._regionState = value;
					base.OnPropertyChanged("RegionState");
				}
			}
		}

		// Token: 0x1700041D RID: 1053
		// (get) Token: 0x06000D1C RID: 3356 RVA: 0x0002DE02 File Offset: 0x0002C002
		// (set) Token: 0x06000D1D RID: 3357 RVA: 0x0002DE0A File Offset: 0x0002C00A
		[DataSourceProperty]
		public int PremadeMatchTypeState
		{
			get
			{
				return this._premadeMatchTypeState;
			}
			set
			{
				if (value != this._premadeMatchTypeState)
				{
					this._premadeMatchTypeState = value;
					base.OnPropertyChanged("PremadeMatchTypeState");
				}
			}
		}

		// Token: 0x1700041E RID: 1054
		// (get) Token: 0x06000D1E RID: 3358 RVA: 0x0002DE27 File Offset: 0x0002C027
		// (set) Token: 0x06000D1F RID: 3359 RVA: 0x0002DE2F File Offset: 0x0002C02F
		[DataSourceProperty]
		public bool IsPlayerCountSelected
		{
			get
			{
				return this._isPlayerCountSelected;
			}
			set
			{
				if (value != this._isPlayerCountSelected)
				{
					this._isPlayerCountSelected = value;
					base.OnPropertyChanged("IsPlayerCountSelected");
				}
			}
		}

		// Token: 0x1700041F RID: 1055
		// (get) Token: 0x06000D20 RID: 3360 RVA: 0x0002DE4C File Offset: 0x0002C04C
		// (set) Token: 0x06000D21 RID: 3361 RVA: 0x0002DE54 File Offset: 0x0002C054
		[DataSourceProperty]
		public int HostState
		{
			get
			{
				return this._hostState;
			}
			set
			{
				if (value != this._hostState)
				{
					this._hostState = value;
					base.OnPropertyChanged("HostState");
				}
			}
		}

		// Token: 0x17000420 RID: 1056
		// (get) Token: 0x06000D22 RID: 3362 RVA: 0x0002DE71 File Offset: 0x0002C071
		// (set) Token: 0x06000D23 RID: 3363 RVA: 0x0002DE79 File Offset: 0x0002C079
		[DataSourceProperty]
		public int PingState
		{
			get
			{
				return this._pingState;
			}
			set
			{
				if (value != this._pingState)
				{
					this._pingState = value;
					base.OnPropertyChanged("PingState");
				}
			}
		}

		// Token: 0x17000421 RID: 1057
		// (get) Token: 0x06000D24 RID: 3364 RVA: 0x0002DE96 File Offset: 0x0002C096
		// (set) Token: 0x06000D25 RID: 3365 RVA: 0x0002DE9E File Offset: 0x0002C09E
		[DataSourceProperty]
		public bool IsServerNameSelected
		{
			get
			{
				return this._isServerNameSelected;
			}
			set
			{
				if (value != this._isServerNameSelected)
				{
					this._isServerNameSelected = value;
					base.OnPropertyChanged("IsServerNameSelected");
				}
			}
		}

		// Token: 0x17000422 RID: 1058
		// (get) Token: 0x06000D26 RID: 3366 RVA: 0x0002DEBB File Offset: 0x0002C0BB
		// (set) Token: 0x06000D27 RID: 3367 RVA: 0x0002DEC3 File Offset: 0x0002C0C3
		[DataSourceProperty]
		public bool IsPasswordSelected
		{
			get
			{
				return this._isPasswordSelected;
			}
			set
			{
				if (value != this._isPasswordSelected)
				{
					this._isPasswordSelected = value;
					base.OnPropertyChanged("IsPasswordSelected");
				}
			}
		}

		// Token: 0x17000423 RID: 1059
		// (get) Token: 0x06000D28 RID: 3368 RVA: 0x0002DEE0 File Offset: 0x0002C0E0
		// (set) Token: 0x06000D29 RID: 3369 RVA: 0x0002DEE8 File Offset: 0x0002C0E8
		[DataSourceProperty]
		public bool IsFirstFactionSelected
		{
			get
			{
				return this._isFirstFactionSelected;
			}
			set
			{
				if (value != this._isFirstFactionSelected)
				{
					this._isFirstFactionSelected = value;
					base.OnPropertyChanged("IsFirstFactionSelected");
				}
			}
		}

		// Token: 0x17000424 RID: 1060
		// (get) Token: 0x06000D2A RID: 3370 RVA: 0x0002DF05 File Offset: 0x0002C105
		// (set) Token: 0x06000D2B RID: 3371 RVA: 0x0002DF0D File Offset: 0x0002C10D
		[DataSourceProperty]
		public bool IsGameTypeSelected
		{
			get
			{
				return this._isGameTypeSelected;
			}
			set
			{
				if (value != this._isGameTypeSelected)
				{
					this._isGameTypeSelected = value;
					base.OnPropertyChanged("IsGameTypeSelected");
				}
			}
		}

		// Token: 0x17000425 RID: 1061
		// (get) Token: 0x06000D2C RID: 3372 RVA: 0x0002DF2A File Offset: 0x0002C12A
		// (set) Token: 0x06000D2D RID: 3373 RVA: 0x0002DF32 File Offset: 0x0002C132
		[DataSourceProperty]
		public bool IsSecondFactionSelected
		{
			get
			{
				return this._isSecondFactionSelected;
			}
			set
			{
				if (value != this._isSecondFactionSelected)
				{
					this._isSecondFactionSelected = value;
					base.OnPropertyChanged("IsSecondFactionSelected");
				}
			}
		}

		// Token: 0x17000426 RID: 1062
		// (get) Token: 0x06000D2E RID: 3374 RVA: 0x0002DF4F File Offset: 0x0002C14F
		// (set) Token: 0x06000D2F RID: 3375 RVA: 0x0002DF57 File Offset: 0x0002C157
		[DataSourceProperty]
		public bool IsRegionSelected
		{
			get
			{
				return this._isRegionSelected;
			}
			set
			{
				if (value != this._isRegionSelected)
				{
					this._isRegionSelected = value;
					base.OnPropertyChanged("IsRegionSelected");
				}
			}
		}

		// Token: 0x17000427 RID: 1063
		// (get) Token: 0x06000D30 RID: 3376 RVA: 0x0002DF74 File Offset: 0x0002C174
		// (set) Token: 0x06000D31 RID: 3377 RVA: 0x0002DF7C File Offset: 0x0002C17C
		[DataSourceProperty]
		public bool IsPremadeMatchTypeSelected
		{
			get
			{
				return this._isPremadeMatchTypeSelected;
			}
			set
			{
				if (value != this._isPremadeMatchTypeSelected)
				{
					this._isPremadeMatchTypeSelected = value;
					base.OnPropertyChanged("IsPremadeMatchTypeSelected");
				}
			}
		}

		// Token: 0x17000428 RID: 1064
		// (get) Token: 0x06000D32 RID: 3378 RVA: 0x0002DF99 File Offset: 0x0002C199
		// (set) Token: 0x06000D33 RID: 3379 RVA: 0x0002DFA1 File Offset: 0x0002C1A1
		[DataSourceProperty]
		public bool IsHostSelected
		{
			get
			{
				return this._isHostSelected;
			}
			set
			{
				if (value != this._isHostSelected)
				{
					this._isHostSelected = value;
					base.OnPropertyChanged("IsHostSelected");
				}
			}
		}

		// Token: 0x17000429 RID: 1065
		// (get) Token: 0x06000D34 RID: 3380 RVA: 0x0002DFBE File Offset: 0x0002C1BE
		// (set) Token: 0x06000D35 RID: 3381 RVA: 0x0002DFC6 File Offset: 0x0002C1C6
		[DataSourceProperty]
		public bool IsPingSelected
		{
			get
			{
				return this._isPingSelected;
			}
			set
			{
				if (value != this._isPingSelected)
				{
					this._isPingSelected = value;
					base.OnPropertyChanged("IsPingSelected");
				}
			}
		}

		// Token: 0x04000637 RID: 1591
		private MBBindingList<MPCustomGameItemVM> _listToControl;

		// Token: 0x04000638 RID: 1592
		private readonly MPCustomGameSortControllerVM.ServerNameComparer _serverNameComparer;

		// Token: 0x04000639 RID: 1593
		private readonly MPCustomGameSortControllerVM.GameTypeComparer _gameTypeComparer;

		// Token: 0x0400063A RID: 1594
		private readonly MPCustomGameSortControllerVM.PlayerCountComparer _playerCountComparer;

		// Token: 0x0400063B RID: 1595
		private readonly MPCustomGameSortControllerVM.PasswordComparer _passwordComparer;

		// Token: 0x0400063C RID: 1596
		private readonly MPCustomGameSortControllerVM.FirstFactionComparer _firstFactionComparer;

		// Token: 0x0400063D RID: 1597
		private readonly MPCustomGameSortControllerVM.SecondFactionComparer _secondFactionComparer;

		// Token: 0x0400063E RID: 1598
		private readonly MPCustomGameSortControllerVM.RegionComparer _regionComparer;

		// Token: 0x0400063F RID: 1599
		private readonly MPCustomGameSortControllerVM.PremadeMatchTypeComparer _premadeMatchTypeComparer;

		// Token: 0x04000640 RID: 1600
		private readonly MPCustomGameSortControllerVM.HostComparer _hostComparer;

		// Token: 0x04000641 RID: 1601
		private readonly MPCustomGameSortControllerVM.PingComparer _pingComparer;

		// Token: 0x04000643 RID: 1603
		private bool _isPremadeMatchesList;

		// Token: 0x04000644 RID: 1604
		private bool _isPingInfoAvailable;

		// Token: 0x04000645 RID: 1605
		private int _serverNameState;

		// Token: 0x04000646 RID: 1606
		private int _gameTypeState;

		// Token: 0x04000647 RID: 1607
		private int _playerCountState;

		// Token: 0x04000648 RID: 1608
		private int _passwordState;

		// Token: 0x04000649 RID: 1609
		private int _firstFactionState;

		// Token: 0x0400064A RID: 1610
		private int _secondFactionState;

		// Token: 0x0400064B RID: 1611
		private int _regionState;

		// Token: 0x0400064C RID: 1612
		private int _premadeMatchTypeState;

		// Token: 0x0400064D RID: 1613
		private int _hostState;

		// Token: 0x0400064E RID: 1614
		private int _pingState;

		// Token: 0x0400064F RID: 1615
		private bool _isServerNameSelected;

		// Token: 0x04000650 RID: 1616
		private bool _isGameTypeSelected;

		// Token: 0x04000651 RID: 1617
		private bool _isPlayerCountSelected;

		// Token: 0x04000652 RID: 1618
		private bool _isPasswordSelected;

		// Token: 0x04000653 RID: 1619
		private bool _isFirstFactionSelected;

		// Token: 0x04000654 RID: 1620
		private bool _isSecondFactionSelected;

		// Token: 0x04000655 RID: 1621
		private bool _isRegionSelected;

		// Token: 0x04000656 RID: 1622
		private bool _isPremadeMatchTypeSelected;

		// Token: 0x04000657 RID: 1623
		private bool _isHostSelected;

		// Token: 0x04000658 RID: 1624
		private bool _isPingSelected;

		// Token: 0x020001D4 RID: 468
		private enum SortState
		{
			// Token: 0x04000DE7 RID: 3559
			Default,
			// Token: 0x04000DE8 RID: 3560
			Ascending,
			// Token: 0x04000DE9 RID: 3561
			Descending
		}

		// Token: 0x020001D5 RID: 469
		public enum CustomServerSortOption
		{
			// Token: 0x04000DEB RID: 3563
			Name,
			// Token: 0x04000DEC RID: 3564
			GameType,
			// Token: 0x04000DED RID: 3565
			PlayerCount,
			// Token: 0x04000DEE RID: 3566
			PasswordProtection,
			// Token: 0x04000DEF RID: 3567
			FirstFaction,
			// Token: 0x04000DF0 RID: 3568
			SecondFaction,
			// Token: 0x04000DF1 RID: 3569
			Region,
			// Token: 0x04000DF2 RID: 3570
			PremadeMatchType,
			// Token: 0x04000DF3 RID: 3571
			Host,
			// Token: 0x04000DF4 RID: 3572
			Ping
		}

		// Token: 0x020001D6 RID: 470
		private abstract class ItemComparer : IComparer<MPCustomGameItemVM>
		{
			// Token: 0x06001A65 RID: 6757 RVA: 0x0005540E File Offset: 0x0005360E
			public void SetSortMode(bool isAscending)
			{
				this._isAscending = isAscending;
			}

			// Token: 0x06001A66 RID: 6758
			public abstract int Compare(MPCustomGameItemVM x, MPCustomGameItemVM y);

			// Token: 0x04000DF5 RID: 3573
			protected bool _isAscending;
		}

		// Token: 0x020001D7 RID: 471
		private class ServerNameComparer : MPCustomGameSortControllerVM.ItemComparer
		{
			// Token: 0x06001A68 RID: 6760 RVA: 0x0005541F File Offset: 0x0005361F
			public override int Compare(MPCustomGameItemVM x, MPCustomGameItemVM y)
			{
				if (this._isAscending)
				{
					return y.NameText.CompareTo(x.NameText) * -1;
				}
				return y.NameText.CompareTo(x.NameText);
			}
		}

		// Token: 0x020001D8 RID: 472
		private class GameTypeComparer : MPCustomGameSortControllerVM.ItemComparer
		{
			// Token: 0x06001A6A RID: 6762 RVA: 0x00055456 File Offset: 0x00053656
			public override int Compare(MPCustomGameItemVM x, MPCustomGameItemVM y)
			{
				if (this._isAscending)
				{
					return y.GameTypeText.CompareTo(x.GameTypeText) * -1;
				}
				return y.GameTypeText.CompareTo(x.GameTypeText);
			}
		}

		// Token: 0x020001D9 RID: 473
		private class PlayerCountComparer : MPCustomGameSortControllerVM.ItemComparer
		{
			// Token: 0x06001A6C RID: 6764 RVA: 0x00055490 File Offset: 0x00053690
			public override int Compare(MPCustomGameItemVM x, MPCustomGameItemVM y)
			{
				if (this._isAscending)
				{
					return y.PlayerCount.CompareTo(x.PlayerCount) * -1;
				}
				return y.PlayerCount.CompareTo(x.PlayerCount);
			}
		}

		// Token: 0x020001DA RID: 474
		private class PasswordComparer : MPCustomGameSortControllerVM.ItemComparer
		{
			// Token: 0x06001A6E RID: 6766 RVA: 0x000554D8 File Offset: 0x000536D8
			public override int Compare(MPCustomGameItemVM x, MPCustomGameItemVM y)
			{
				if (this._isAscending)
				{
					return y.IsPasswordProtected.CompareTo(x.IsPasswordProtected) * -1;
				}
				return y.IsPasswordProtected.CompareTo(x.IsPasswordProtected);
			}
		}

		// Token: 0x020001DB RID: 475
		private class FirstFactionComparer : MPCustomGameSortControllerVM.ItemComparer
		{
			// Token: 0x06001A70 RID: 6768 RVA: 0x00055520 File Offset: 0x00053720
			public override int Compare(MPCustomGameItemVM x, MPCustomGameItemVM y)
			{
				if (this._isAscending)
				{
					return y.FirstFactionName.CompareTo(x.FirstFactionName) * -1;
				}
				return y.FirstFactionName.CompareTo(x.FirstFactionName);
			}
		}

		// Token: 0x020001DC RID: 476
		private class SecondFactionComparer : MPCustomGameSortControllerVM.ItemComparer
		{
			// Token: 0x06001A72 RID: 6770 RVA: 0x00055557 File Offset: 0x00053757
			public override int Compare(MPCustomGameItemVM x, MPCustomGameItemVM y)
			{
				if (this._isAscending)
				{
					return y.SecondFactionName.CompareTo(x.SecondFactionName) * -1;
				}
				return y.SecondFactionName.CompareTo(x.SecondFactionName);
			}
		}

		// Token: 0x020001DD RID: 477
		private class RegionComparer : MPCustomGameSortControllerVM.ItemComparer
		{
			// Token: 0x06001A74 RID: 6772 RVA: 0x0005558E File Offset: 0x0005378E
			public override int Compare(MPCustomGameItemVM x, MPCustomGameItemVM y)
			{
				if (this._isAscending)
				{
					return y.RegionName.CompareTo(x.RegionName) * -1;
				}
				return y.RegionName.CompareTo(x.RegionName);
			}
		}

		// Token: 0x020001DE RID: 478
		private class PremadeMatchTypeComparer : MPCustomGameSortControllerVM.ItemComparer
		{
			// Token: 0x06001A76 RID: 6774 RVA: 0x000555C5 File Offset: 0x000537C5
			public override int Compare(MPCustomGameItemVM x, MPCustomGameItemVM y)
			{
				if (this._isAscending)
				{
					return y.PremadeMatchTypeText.CompareTo(x.PremadeMatchTypeText) * -1;
				}
				return y.PremadeMatchTypeText.CompareTo(x.PremadeMatchTypeText);
			}
		}

		// Token: 0x020001DF RID: 479
		private class HostComparer : MPCustomGameSortControllerVM.ItemComparer
		{
			// Token: 0x06001A78 RID: 6776 RVA: 0x000555FC File Offset: 0x000537FC
			public override int Compare(MPCustomGameItemVM x, MPCustomGameItemVM y)
			{
				if (!(y.HostText == x.HostText))
				{
					string hostText = y.HostText;
					return ((hostText != null) ? hostText.CompareTo(x.HostText) : (-1)) * (this._isAscending ? (-1) : 1);
				}
				return 0;
			}
		}

		// Token: 0x020001E0 RID: 480
		private class PingComparer : MPCustomGameSortControllerVM.ItemComparer
		{
			// Token: 0x06001A7A RID: 6778 RVA: 0x00055640 File Offset: 0x00053840
			public override int Compare(MPCustomGameItemVM x, MPCustomGameItemVM y)
			{
				int num = (this._isAscending ? (-1) : 1);
				if (y.PingText == x.PingText)
				{
					return 0;
				}
				if (y.PingText == "-" || y.PingText == null)
				{
					return num;
				}
				if (x.PingText == "-" || x.PingText == null)
				{
					return num * -1;
				}
				return (int)(long.Parse(y.PingText) - long.Parse(x.PingText)) * num;
			}
		}
	}
}
