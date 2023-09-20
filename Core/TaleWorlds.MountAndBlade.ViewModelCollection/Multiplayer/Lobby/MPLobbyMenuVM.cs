using System;
using System.Threading.Tasks;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.MountAndBlade.ViewModelCollection.Input;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby
{
	public class MPLobbyMenuVM : ViewModel
	{
		public MPLobbyMenuVM(LobbyState lobbyState, Action<bool> setNavigationRestriction, Func<Task> onQuit)
		{
			this._lobbyState = lobbyState;
			this._setNavigationRestriction = setNavigationRestriction;
			this._onQuit = onQuit;
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.HomeText = new TextObject("{=hometab}Home", null).ToString();
			this.MatchmakingText = new TextObject("{=playgame}Play", null).ToString();
			this.ProfileText = new TextObject("{=0647tsif}Profile", null).ToString();
			this.ArmoryText = new TextObject("{=kG0xuyfE}Armory", null).ToString();
			this.PreviousPageInputKey = InputKeyItemVM.CreateFromHotKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("SwitchToPreviousTab"), true);
			this.NextPageInputKey = InputKeyItemVM.CreateFromHotKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("SwitchToNextTab"), true);
		}

		public override void OnFinalize()
		{
			base.OnFinalize();
			this.PreviousPageInputKey.OnFinalize();
			this.NextPageInputKey.OnFinalize();
		}

		public void SetPage(MPLobbyVM.LobbyPage lobbyPage)
		{
			this.PageIndex = (int)lobbyPage;
		}

		private void ExecuteHome()
		{
			this._lobbyState.OnActivateHome();
		}

		private void ExecuteMatchmaking()
		{
			this._lobbyState.OnActivateMatchmaking();
		}

		private void ExecuteCustomServer()
		{
			this._lobbyState.OnActivateCustomServer();
		}

		private void ExecuteArmory()
		{
			this._lobbyState.OnActivateArmory();
		}

		private void ExecuteOptions()
		{
			this._lobbyState.OnActivateOptions();
		}

		private void ExecuteProfile()
		{
			this._lobbyState.OnActivateProfile();
		}

		public async void ExecuteExit()
		{
			Func<Task> onQuit = this._onQuit;
			await ((onQuit != null) ? onQuit() : null);
		}

		public void OnSupportedFeaturesRefreshed(SupportedFeatures supportedFeatures)
		{
			this.IsMatchmakingSupported = supportedFeatures.SupportsFeatures(Features.Matchmaking);
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
		public bool HasProfileNotification
		{
			get
			{
				return this._hasProfileNotification;
			}
			set
			{
				if (value != this._hasProfileNotification)
				{
					this._hasProfileNotification = value;
					base.OnPropertyChangedWithValue(value, "HasProfileNotification");
				}
			}
		}

		[DataSourceProperty]
		public bool IsClanSupported
		{
			get
			{
				return this._isClanSupported;
			}
			set
			{
				if (value != this._isClanSupported)
				{
					this._isClanSupported = value;
					base.OnPropertyChangedWithValue(value, "IsClanSupported");
				}
			}
		}

		[DataSourceProperty]
		public bool IsMatchmakingSupported
		{
			get
			{
				return this._isMatchmakingSupported;
			}
			set
			{
				if (value != this._isMatchmakingSupported)
				{
					this._isMatchmakingSupported = value;
					base.OnPropertyChangedWithValue(value, "IsMatchmakingSupported");
				}
			}
		}

		[DataSourceProperty]
		public int PageIndex
		{
			get
			{
				return this._pageIndex;
			}
			set
			{
				if (value != this._pageIndex)
				{
					this._pageIndex = value;
					base.OnPropertyChangedWithValue(value, "PageIndex");
				}
			}
		}

		[DataSourceProperty]
		public string HomeText
		{
			get
			{
				return this._homeText;
			}
			set
			{
				if (value != this._homeText)
				{
					this._homeText = value;
					base.OnPropertyChangedWithValue<string>(value, "HomeText");
				}
			}
		}

		[DataSourceProperty]
		public string MatchmakingText
		{
			get
			{
				return this._matchmakingText;
			}
			set
			{
				if (value != this._matchmakingText)
				{
					this._matchmakingText = value;
					base.OnPropertyChangedWithValue<string>(value, "MatchmakingText");
				}
			}
		}

		[DataSourceProperty]
		public string ProfileText
		{
			get
			{
				return this._profileText;
			}
			set
			{
				if (value != this._profileText)
				{
					this._profileText = value;
					base.OnPropertyChangedWithValue<string>(value, "ProfileText");
				}
			}
		}

		[DataSourceProperty]
		public string ArmoryText
		{
			get
			{
				return this._armoryText;
			}
			set
			{
				if (value != this._armoryText)
				{
					this._armoryText = value;
					base.OnPropertyChangedWithValue<string>(value, "ArmoryText");
				}
			}
		}

		[DataSourceProperty]
		public InputKeyItemVM PreviousPageInputKey
		{
			get
			{
				return this._previousPageInputKey;
			}
			set
			{
				if (value != this._previousPageInputKey)
				{
					this._previousPageInputKey = value;
					base.OnPropertyChanged("PreviousPageInputKey");
				}
			}
		}

		[DataSourceProperty]
		public InputKeyItemVM NextPageInputKey
		{
			get
			{
				return this._nextPageInputKey;
			}
			set
			{
				if (value != this._nextPageInputKey)
				{
					this._nextPageInputKey = value;
					base.OnPropertyChanged("NextPageInputKey");
				}
			}
		}

		private LobbyState _lobbyState;

		private readonly Action<bool> _setNavigationRestriction;

		private readonly Func<Task> _onQuit;

		private bool _isEnabled;

		private bool _hasProfileNotification;

		private bool _isClanSupported;

		private bool _isMatchmakingSupported;

		private int _pageIndex;

		private string _homeText;

		private string _matchmakingText;

		private string _profileText;

		private string _armoryText;

		private InputKeyItemVM _previousPageInputKey;

		private InputKeyItemVM _nextPageInputKey;
	}
}
