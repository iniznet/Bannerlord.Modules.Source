using System;
using System.Collections.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Lobby.CustomGame
{
	public class MPCustomGameSortControllerVM : ViewModel
	{
		public MPCustomGameSortControllerVM.CustomServerSortOption? CurrentSortOption { get; private set; }

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

		private MBBindingList<MPCustomGameItemVM> _listToControl;

		private readonly MPCustomGameSortControllerVM.ServerNameComparer _serverNameComparer;

		private readonly MPCustomGameSortControllerVM.GameTypeComparer _gameTypeComparer;

		private readonly MPCustomGameSortControllerVM.PlayerCountComparer _playerCountComparer;

		private readonly MPCustomGameSortControllerVM.PasswordComparer _passwordComparer;

		private readonly MPCustomGameSortControllerVM.FirstFactionComparer _firstFactionComparer;

		private readonly MPCustomGameSortControllerVM.SecondFactionComparer _secondFactionComparer;

		private readonly MPCustomGameSortControllerVM.RegionComparer _regionComparer;

		private readonly MPCustomGameSortControllerVM.PremadeMatchTypeComparer _premadeMatchTypeComparer;

		private readonly MPCustomGameSortControllerVM.HostComparer _hostComparer;

		private readonly MPCustomGameSortControllerVM.PingComparer _pingComparer;

		private bool _isPremadeMatchesList;

		private bool _isPingInfoAvailable;

		private int _serverNameState;

		private int _gameTypeState;

		private int _playerCountState;

		private int _passwordState;

		private int _firstFactionState;

		private int _secondFactionState;

		private int _regionState;

		private int _premadeMatchTypeState;

		private int _hostState;

		private int _pingState;

		private bool _isServerNameSelected;

		private bool _isGameTypeSelected;

		private bool _isPlayerCountSelected;

		private bool _isPasswordSelected;

		private bool _isFirstFactionSelected;

		private bool _isSecondFactionSelected;

		private bool _isRegionSelected;

		private bool _isPremadeMatchTypeSelected;

		private bool _isHostSelected;

		private bool _isPingSelected;

		private enum SortState
		{
			Default,
			Ascending,
			Descending
		}

		public enum CustomServerSortOption
		{
			Name,
			GameType,
			PlayerCount,
			PasswordProtection,
			FirstFaction,
			SecondFaction,
			Region,
			PremadeMatchType,
			Host,
			Ping
		}

		private abstract class ItemComparer : IComparer<MPCustomGameItemVM>
		{
			public void SetSortMode(bool isAscending)
			{
				this._isAscending = isAscending;
			}

			public abstract int Compare(MPCustomGameItemVM x, MPCustomGameItemVM y);

			protected bool _isAscending;
		}

		private class ServerNameComparer : MPCustomGameSortControllerVM.ItemComparer
		{
			public override int Compare(MPCustomGameItemVM x, MPCustomGameItemVM y)
			{
				if (this._isAscending)
				{
					return y.NameText.CompareTo(x.NameText) * -1;
				}
				return y.NameText.CompareTo(x.NameText);
			}
		}

		private class GameTypeComparer : MPCustomGameSortControllerVM.ItemComparer
		{
			public override int Compare(MPCustomGameItemVM x, MPCustomGameItemVM y)
			{
				if (this._isAscending)
				{
					return y.GameTypeText.CompareTo(x.GameTypeText) * -1;
				}
				return y.GameTypeText.CompareTo(x.GameTypeText);
			}
		}

		private class PlayerCountComparer : MPCustomGameSortControllerVM.ItemComparer
		{
			public override int Compare(MPCustomGameItemVM x, MPCustomGameItemVM y)
			{
				if (this._isAscending)
				{
					return y.PlayerCount.CompareTo(x.PlayerCount) * -1;
				}
				return y.PlayerCount.CompareTo(x.PlayerCount);
			}
		}

		private class PasswordComparer : MPCustomGameSortControllerVM.ItemComparer
		{
			public override int Compare(MPCustomGameItemVM x, MPCustomGameItemVM y)
			{
				if (this._isAscending)
				{
					return y.IsPasswordProtected.CompareTo(x.IsPasswordProtected) * -1;
				}
				return y.IsPasswordProtected.CompareTo(x.IsPasswordProtected);
			}
		}

		private class FirstFactionComparer : MPCustomGameSortControllerVM.ItemComparer
		{
			public override int Compare(MPCustomGameItemVM x, MPCustomGameItemVM y)
			{
				if (this._isAscending)
				{
					return y.FirstFactionName.CompareTo(x.FirstFactionName) * -1;
				}
				return y.FirstFactionName.CompareTo(x.FirstFactionName);
			}
		}

		private class SecondFactionComparer : MPCustomGameSortControllerVM.ItemComparer
		{
			public override int Compare(MPCustomGameItemVM x, MPCustomGameItemVM y)
			{
				if (this._isAscending)
				{
					return y.SecondFactionName.CompareTo(x.SecondFactionName) * -1;
				}
				return y.SecondFactionName.CompareTo(x.SecondFactionName);
			}
		}

		private class RegionComparer : MPCustomGameSortControllerVM.ItemComparer
		{
			public override int Compare(MPCustomGameItemVM x, MPCustomGameItemVM y)
			{
				if (this._isAscending)
				{
					return y.RegionName.CompareTo(x.RegionName) * -1;
				}
				return y.RegionName.CompareTo(x.RegionName);
			}
		}

		private class PremadeMatchTypeComparer : MPCustomGameSortControllerVM.ItemComparer
		{
			public override int Compare(MPCustomGameItemVM x, MPCustomGameItemVM y)
			{
				if (this._isAscending)
				{
					return y.PremadeMatchTypeText.CompareTo(x.PremadeMatchTypeText) * -1;
				}
				return y.PremadeMatchTypeText.CompareTo(x.PremadeMatchTypeText);
			}
		}

		private class HostComparer : MPCustomGameSortControllerVM.ItemComparer
		{
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

		private class PingComparer : MPCustomGameSortControllerVM.ItemComparer
		{
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
