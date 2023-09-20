using System;
using System.Threading.Tasks;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.MountAndBlade.ViewModelCollection.Input;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby
{
	// Token: 0x0200005D RID: 93
	public class MPLobbyMenuVM : ViewModel
	{
		// Token: 0x060007D5 RID: 2005 RVA: 0x0001E694 File Offset: 0x0001C894
		public MPLobbyMenuVM(LobbyState lobbyState, Action<bool> setNavigationRestriction, Func<Task> onQuit)
		{
			this._lobbyState = lobbyState;
			this._setNavigationRestriction = setNavigationRestriction;
			this._onQuit = onQuit;
			this.RefreshValues();
		}

		// Token: 0x060007D6 RID: 2006 RVA: 0x0001E6B8 File Offset: 0x0001C8B8
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

		// Token: 0x060007D7 RID: 2007 RVA: 0x0001E763 File Offset: 0x0001C963
		public override void OnFinalize()
		{
			base.OnFinalize();
			this.PreviousPageInputKey.OnFinalize();
			this.NextPageInputKey.OnFinalize();
		}

		// Token: 0x060007D8 RID: 2008 RVA: 0x0001E781 File Offset: 0x0001C981
		public void SetPage(MPLobbyVM.LobbyPage lobbyPage)
		{
			this.PageIndex = (int)lobbyPage;
		}

		// Token: 0x060007D9 RID: 2009 RVA: 0x0001E78A File Offset: 0x0001C98A
		private void ExecuteHome()
		{
			this._lobbyState.OnActivateHome();
		}

		// Token: 0x060007DA RID: 2010 RVA: 0x0001E797 File Offset: 0x0001C997
		private void ExecuteMatchmaking()
		{
			this._lobbyState.OnActivateMatchmaking();
		}

		// Token: 0x060007DB RID: 2011 RVA: 0x0001E7A4 File Offset: 0x0001C9A4
		private void ExecuteCustomServer()
		{
			this._lobbyState.OnActivateCustomServer();
		}

		// Token: 0x060007DC RID: 2012 RVA: 0x0001E7B1 File Offset: 0x0001C9B1
		private void ExecuteArmory()
		{
			this._lobbyState.OnActivateArmory();
		}

		// Token: 0x060007DD RID: 2013 RVA: 0x0001E7BE File Offset: 0x0001C9BE
		private void ExecuteOptions()
		{
			this._lobbyState.OnActivateOptions();
		}

		// Token: 0x060007DE RID: 2014 RVA: 0x0001E7CB File Offset: 0x0001C9CB
		private void ExecuteProfile()
		{
			this._lobbyState.OnActivateProfile();
		}

		// Token: 0x060007DF RID: 2015 RVA: 0x0001E7D8 File Offset: 0x0001C9D8
		public async void ExecuteExit()
		{
			Func<Task> onQuit = this._onQuit;
			await ((onQuit != null) ? onQuit() : null);
		}

		// Token: 0x060007E0 RID: 2016 RVA: 0x0001E811 File Offset: 0x0001CA11
		public void OnSupportedFeaturesRefreshed(SupportedFeatures supportedFeatures)
		{
			this.IsMatchmakingSupported = supportedFeatures.SupportsFeatures(Features.Matchmaking);
		}

		// Token: 0x1700026B RID: 619
		// (get) Token: 0x060007E1 RID: 2017 RVA: 0x0001E820 File Offset: 0x0001CA20
		// (set) Token: 0x060007E2 RID: 2018 RVA: 0x0001E828 File Offset: 0x0001CA28
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

		// Token: 0x1700026C RID: 620
		// (get) Token: 0x060007E3 RID: 2019 RVA: 0x0001E846 File Offset: 0x0001CA46
		// (set) Token: 0x060007E4 RID: 2020 RVA: 0x0001E84E File Offset: 0x0001CA4E
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

		// Token: 0x1700026D RID: 621
		// (get) Token: 0x060007E5 RID: 2021 RVA: 0x0001E86C File Offset: 0x0001CA6C
		// (set) Token: 0x060007E6 RID: 2022 RVA: 0x0001E874 File Offset: 0x0001CA74
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

		// Token: 0x1700026E RID: 622
		// (get) Token: 0x060007E7 RID: 2023 RVA: 0x0001E892 File Offset: 0x0001CA92
		// (set) Token: 0x060007E8 RID: 2024 RVA: 0x0001E89A File Offset: 0x0001CA9A
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

		// Token: 0x1700026F RID: 623
		// (get) Token: 0x060007E9 RID: 2025 RVA: 0x0001E8B8 File Offset: 0x0001CAB8
		// (set) Token: 0x060007EA RID: 2026 RVA: 0x0001E8C0 File Offset: 0x0001CAC0
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

		// Token: 0x17000270 RID: 624
		// (get) Token: 0x060007EB RID: 2027 RVA: 0x0001E8DE File Offset: 0x0001CADE
		// (set) Token: 0x060007EC RID: 2028 RVA: 0x0001E8E6 File Offset: 0x0001CAE6
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

		// Token: 0x17000271 RID: 625
		// (get) Token: 0x060007ED RID: 2029 RVA: 0x0001E909 File Offset: 0x0001CB09
		// (set) Token: 0x060007EE RID: 2030 RVA: 0x0001E911 File Offset: 0x0001CB11
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

		// Token: 0x17000272 RID: 626
		// (get) Token: 0x060007EF RID: 2031 RVA: 0x0001E934 File Offset: 0x0001CB34
		// (set) Token: 0x060007F0 RID: 2032 RVA: 0x0001E93C File Offset: 0x0001CB3C
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

		// Token: 0x17000273 RID: 627
		// (get) Token: 0x060007F1 RID: 2033 RVA: 0x0001E95F File Offset: 0x0001CB5F
		// (set) Token: 0x060007F2 RID: 2034 RVA: 0x0001E967 File Offset: 0x0001CB67
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

		// Token: 0x17000274 RID: 628
		// (get) Token: 0x060007F3 RID: 2035 RVA: 0x0001E98A File Offset: 0x0001CB8A
		// (set) Token: 0x060007F4 RID: 2036 RVA: 0x0001E992 File Offset: 0x0001CB92
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

		// Token: 0x17000275 RID: 629
		// (get) Token: 0x060007F5 RID: 2037 RVA: 0x0001E9AF File Offset: 0x0001CBAF
		// (set) Token: 0x060007F6 RID: 2038 RVA: 0x0001E9B7 File Offset: 0x0001CBB7
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

		// Token: 0x040003FB RID: 1019
		private LobbyState _lobbyState;

		// Token: 0x040003FC RID: 1020
		private readonly Action<bool> _setNavigationRestriction;

		// Token: 0x040003FD RID: 1021
		private readonly Func<Task> _onQuit;

		// Token: 0x040003FE RID: 1022
		private bool _isEnabled;

		// Token: 0x040003FF RID: 1023
		private bool _hasProfileNotification;

		// Token: 0x04000400 RID: 1024
		private bool _isClanSupported;

		// Token: 0x04000401 RID: 1025
		private bool _isMatchmakingSupported;

		// Token: 0x04000402 RID: 1026
		private int _pageIndex;

		// Token: 0x04000403 RID: 1027
		private string _homeText;

		// Token: 0x04000404 RID: 1028
		private string _matchmakingText;

		// Token: 0x04000405 RID: 1029
		private string _profileText;

		// Token: 0x04000406 RID: 1030
		private string _armoryText;

		// Token: 0x04000407 RID: 1031
		private InputKeyItemVM _previousPageInputKey;

		// Token: 0x04000408 RID: 1032
		private InputKeyItemVM _nextPageInputKey;
	}
}
