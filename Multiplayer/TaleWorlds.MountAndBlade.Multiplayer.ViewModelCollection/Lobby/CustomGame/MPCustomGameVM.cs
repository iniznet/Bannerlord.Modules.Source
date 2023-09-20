using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Lobby.HostGame;
using TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Lobby.HostGame.HostGameOptions;
using TaleWorlds.MountAndBlade.ViewModelCollection.Input;

namespace TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Lobby.CustomGame
{
	public class MPCustomGameVM : ViewModel
	{
		public static bool IsPingInfoAvailable
		{
			get
			{
				return true;
			}
		}

		public MPCustomGameVM(LobbyState lobbyState, MPCustomGameVM.CustomGameMode customGameMode)
		{
			this._lobbyState = lobbyState;
			this._currentCustomGameList = new List<GameServerEntry>();
			this._customGameMode = customGameMode;
			this.HostGame = new MPHostGameVM(this._lobbyState, this._customGameMode);
			this.FiltersData = new MPCustomGameFiltersVM();
			this.GameList = new MBBindingList<MPCustomGameItemVM>();
			this.SortController = new MPCustomGameSortControllerVM(ref this._gameList, this._customGameMode);
			this.CustomServerActionsList = new MBBindingList<StringPairItemWithActionVM>();
			MPCustomGameFiltersVM filtersData = this.FiltersData;
			filtersData.OnFiltersApplied = (Action)Delegate.Combine(filtersData.OnFiltersApplied, new Action(this.RefreshFiltersAndSort));
			this._currentCustomGameList = new List<GameServerEntry>();
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.IsPasswordProtectedHint = new HintViewModel(new TextObject("{=dMdmyb3Y}Password Protected", null), null);
			this.CreateServerText = new TextObject("{=gzdNEM76}Create a Game", null).ToString();
			this.CloseText = new TextObject("{=6MQaCah5}Join a Game", null).ToString();
			this.ServersInfoText = new TextObject("{=WOQZBmMx}Servers", null).ToString();
			this.RefreshText = new TextObject("{=qFPBhVh4}Refresh", null).ToString();
			this.JoinText = new TextObject("{=lWDq0Uss}JOIN", null).ToString();
			this.PasswordText = new TextObject("{=8nJFaJio}Password", null).ToString();
			this.ServerNameText = new TextObject("{=OVcoYxj1}Server Name", null).ToString();
			this.GameTypeText = new TextObject("{=JPimShCw}Game Type", null).ToString();
			this.MapText = new TextObject("{=w9m11T1y}Map", null).ToString();
			this.PlayerCountText = new TextObject("{=RfXJdNye}Players", null).ToString();
			this.PingText = new TextObject("{=7qySRF2T}Ping", null).ToString();
			this.FirstFactionText = new TextObject("{=FhnKJODX}Faction A", null).ToString();
			this.SecondFactionText = new TextObject("{=a9TcHtVw}Faction B", null).ToString();
			this.RegionText = new TextObject("{=uoVKchoC}Region", null).ToString();
			this.PremadeMatchTypeText = new TextObject("{=OzifZbSB}Match Type", null).ToString();
			this.HostText = new TextObject("{=2baWg4Gq}Host", null).ToString();
			this.GameList.ApplyActionOnAllItems(delegate(MPCustomGameItemVM x)
			{
				x.RefreshValues();
			});
			this.SortController.RefreshValues();
			this.FiltersData.RefreshValues();
			MPHostGameVM hostGame = this.HostGame;
			if (hostGame == null)
			{
				return;
			}
			hostGame.RefreshValues();
		}

		public override void OnFinalize()
		{
			base.OnFinalize();
			MPCustomGameFiltersVM filtersData = this.FiltersData;
			filtersData.OnFiltersApplied = (Action)Delegate.Remove(filtersData.OnFiltersApplied, new Action(this.RefreshFiltersAndSort));
			InputKeyItemVM refreshInputKey = this.RefreshInputKey;
			if (refreshInputKey == null)
			{
				return;
			}
			refreshInputKey.OnFinalize();
		}

		public void RefreshPremadeGameList()
		{
			this.IsRefreshed = false;
			this.GameList.Clear();
			foreach (PremadeGameEntry premadeGameEntry in NetworkMain.GameClient.AvailablePremadeGames.PremadeGameEntries)
			{
				this.GameList.Add(new MPCustomGameItemVM(premadeGameEntry, new Action(this.ExecuteJoin)));
			}
			this.IsRefreshed = true;
		}

		public void RefreshCustomGameServerList(AvailableCustomGames availableCustomGames)
		{
			this.IsRefreshed = false;
			this._currentCustomGameList = availableCustomGames.CustomGameServerInfos;
			this.RefreshFiltersAndSort();
			this.IsRefreshed = true;
		}

		private void RefreshFiltersAndSort()
		{
			this.GameList.Clear();
			List<GameServerEntry> filteredServerList = this.FiltersData.GetFilteredServerList(this._currentCustomGameList);
			bool? hasCrossplayPrivilege = this._lobbyState.HasCrossplayPrivilege;
			bool flag = true;
			GameServerEntry.FilterGameServerEntriesBasedOnCrossplay(ref filteredServerList, (hasCrossplayPrivilege.GetValueOrDefault() == flag) & (hasCrossplayPrivilege != null));
			foreach (GameServerEntry gameServerEntry in filteredServerList)
			{
				this.GameList.Add(new MPCustomGameItemVM(gameServerEntry, new Action(this.ExecuteJoin), new Action<GameServerEntry>(this.ExecuteShowActionsForEntry)));
			}
			this.SortController.SortByCurrentState();
		}

		public async void ExecuteRefresh()
		{
			if (this.IsEnabled)
			{
				this.IsRefreshed = false;
				this.GameList.Clear();
				if (this._customGameMode == MPCustomGameVM.CustomGameMode.CustomServer)
				{
					await NetworkMain.GameClient.GetCustomGameServerList();
					MultiplayerOptions.Instance.CurrentOptionsCategory = 0;
				}
				else if (this._customGameMode == MPCustomGameVM.CustomGameMode.PremadeGame)
				{
					await NetworkMain.GameClient.GetPremadeGameList();
					MultiplayerOptions.Instance.CurrentOptionsCategory = 1;
				}
				MultiplayerOptions.Instance.OnGameTypeChanged(0);
				foreach (GenericHostGameOptionDataVM genericHostGameOptionDataVM in this.HostGame.HostGameOptions.GeneralOptions)
				{
					MultipleSelectionHostGameOptionDataVM multipleSelectionHostGameOptionDataVM = genericHostGameOptionDataVM as MultipleSelectionHostGameOptionDataVM;
					if (multipleSelectionHostGameOptionDataVM != null)
					{
						multipleSelectionHostGameOptionDataVM.RefreshList();
					}
				}
				this.IsRefreshed = true;
			}
		}

		private void ExecuteShowActionsForEntry(GameServerEntry serverEntry)
		{
			this.CustomServerActionsList.Clear();
			List<CustomServerAction> customActionsForServer = this._lobbyState.GetCustomActionsForServer(serverEntry);
			if (customActionsForServer.Count > 0)
			{
				for (int i = 0; i < customActionsForServer.Count; i++)
				{
					CustomServerAction customServerAction = customActionsForServer[i];
					this.CustomServerActionsList.Add(new StringPairItemWithActionVM(new Action<object>(this.ExecuteSelectCustomServerAction), customServerAction.Name, customServerAction.Name, customServerAction));
				}
			}
			if (this.CustomServerActionsList.Count > 0)
			{
				this.IsCustomServerActionsActive = false;
				this.IsCustomServerActionsActive = true;
			}
		}

		private bool DoesClientRefuseToJoinCustomServer(GameServerEntry serverEntry)
		{
			string text;
			UniqueSceneId uniqueSceneId;
			return !Utilities.TryGetFullFilePathOfScene(serverEntry.Map, ref text) || (serverEntry.UniqueMapId != null && (!Utilities.TryGetUniqueIdentifiersForSceneFile(text, ref uniqueSceneId) || uniqueSceneId.Serialize() != serverEntry.UniqueMapId));
		}

		public void ExecuteJoin()
		{
			MPCustomGameItemVM mpcustomGameItemVM = this.GameList[this.SelectedIndex];
			if (mpcustomGameItemVM.IsPasswordProtected)
			{
				string text = GameTexts.FindText("str_password_required", null).ToString();
				string text2 = GameTexts.FindText("str_enter_password", null).ToString();
				string text3 = GameTexts.FindText("str_ok", null).ToString();
				string text4 = GameTexts.FindText("str_cancel", null).ToString();
				InformationManager.ShowTextInquiry(new TextInquiryData(text, text2, true, true, text3, text4, this.GetOnTryPasswordForServerAction(mpcustomGameItemVM), null, true, null, "", ""), false, false);
				return;
			}
			if (this._customGameMode == MPCustomGameVM.CustomGameMode.CustomServer)
			{
				this.JoinCustomGame(mpcustomGameItemVM.GameServerInfo, "");
				return;
			}
			if (this._customGameMode == MPCustomGameVM.CustomGameMode.PremadeGame)
			{
				this.JoinPremadeGame(mpcustomGameItemVM.PremadeGameInfo, "");
			}
		}

		private Action<string> GetOnTryPasswordForServerAction(MPCustomGameItemVM serverItem)
		{
			if (this._customGameMode == MPCustomGameVM.CustomGameMode.CustomServer)
			{
				GameServerEntry serverInfo2 = serverItem.GameServerInfo;
				return delegate(string passwordInput)
				{
					this.JoinCustomGame(serverInfo2, passwordInput);
				};
			}
			if (this._customGameMode == MPCustomGameVM.CustomGameMode.PremadeGame)
			{
				PremadeGameEntry serverInfo = serverItem.PremadeGameInfo;
				return delegate(string passwordInput)
				{
					this.JoinPremadeGame(serverInfo, passwordInput);
				};
			}
			return delegate(string _)
			{
				Debug.FailedAssert("Fell through game modes, should never happen", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection\\Lobby\\CustomGame\\MPCustomGameVM.cs", "GetOnTryPasswordForServerAction", 258);
			};
		}

		private async void JoinCustomGame(GameServerEntry selectedServer, string passwordInput = "")
		{
			if (this.DoesClientRefuseToJoinCustomServer(selectedServer))
			{
				this._lobbyState.OnClientRefusedToJoinCustomServer(selectedServer);
				string text = new TextObject("{=Gcs49qcC}The client doesn't have the map ({MAP_NAME}) being played on the server loaded, or the local map is not identical.", null).SetTextVariable("MAP_NAME", selectedServer.Map).ToString();
				string text2 = new TextObject("{=SHiA1TRX}Enabling the DedicatedCustomServerHelper module allows downloading maps from supported servers!", null).ToString();
				string text3 = (Utilities.GetModulesNames().Contains("DedicatedCustomServerHelper") ? text : (text + " " + text2));
				InformationManager.ShowInquiry(new InquiryData(GameTexts.FindText("str_couldnt_join_server", null).ToString(), text3, false, true, "", GameTexts.FindText("str_dismiss", null).ToString(), null, null, "", 0f, null, null, null), false, false);
			}
			else
			{
				TaskAwaiter<bool> taskAwaiter = NetworkMain.GameClient.RequestJoinCustomGame(selectedServer.Id, passwordInput).GetAwaiter();
				if (!taskAwaiter.IsCompleted)
				{
					await taskAwaiter;
					TaskAwaiter<bool> taskAwaiter2;
					taskAwaiter = taskAwaiter2;
					taskAwaiter2 = default(TaskAwaiter<bool>);
				}
				if (!taskAwaiter.GetResult())
				{
					InformationManager.ShowInquiry(new InquiryData("", GameTexts.FindText("str_couldnt_join_server", null).ToString(), true, false, GameTexts.FindText("str_ok", null).ToString(), "", null, null, "", 0f, null, null, null), false, false);
				}
			}
		}

		private void JoinPremadeGame(PremadeGameEntry selectedGame, string passwordInput = "")
		{
			NetworkMain.GameClient.RequestToJoinPremadeGame(selectedGame.Id, passwordInput);
		}

		public void SetRefreshInputKey(HotKey hotKey)
		{
			this.RefreshInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		public InputKeyItemVM RefreshInputKey
		{
			get
			{
				return this._refreshInputKey;
			}
			set
			{
				if (value != this._refreshInputKey)
				{
					this._refreshInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "RefreshInputKey");
				}
			}
		}

		private void ExecuteSelectCustomServerAction(object actionParam)
		{
			(actionParam as CustomServerAction).Execute();
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
					if (this.IsEnabled)
					{
						this.ExecuteRefresh();
					}
				}
			}
		}

		[DataSourceProperty]
		public int SelectedIndex
		{
			get
			{
				return this._selectedIndex;
			}
			set
			{
				if (value != this._selectedIndex)
				{
					this._selectedIndex = value;
					base.OnPropertyChangedWithValue(value, "SelectedIndex");
				}
			}
		}

		[DataSourceProperty]
		public MPCustomGameFiltersVM FiltersData
		{
			get
			{
				return this._filtersData;
			}
			set
			{
				if (value != this._filtersData)
				{
					this._filtersData = value;
					base.OnPropertyChangedWithValue<MPCustomGameFiltersVM>(value, "FiltersData");
				}
			}
		}

		[DataSourceProperty]
		public MPHostGameVM HostGame
		{
			get
			{
				return this._hostGame;
			}
			set
			{
				if (value != this._hostGame)
				{
					this._hostGame = value;
					base.OnPropertyChangedWithValue<MPHostGameVM>(value, "HostGame");
				}
			}
		}

		[DataSourceProperty]
		public MPCustomGameSortControllerVM SortController
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
					base.OnPropertyChangedWithValue<MPCustomGameSortControllerVM>(value, "SortController");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<MPCustomGameItemVM> GameList
		{
			get
			{
				return this._gameList;
			}
			set
			{
				if (value != this._gameList)
				{
					this._gameList = value;
					base.OnPropertyChangedWithValue<MBBindingList<MPCustomGameItemVM>>(value, "GameList");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel IsPasswordProtectedHint
		{
			get
			{
				return this._isPasswordProtectedHint;
			}
			set
			{
				if (value != this._isPasswordProtectedHint)
				{
					this._isPasswordProtectedHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "IsPasswordProtectedHint");
				}
			}
		}

		[DataSourceProperty]
		public bool IsRefreshed
		{
			get
			{
				return this._isRefreshed;
			}
			set
			{
				if (value != this._isRefreshed)
				{
					this._isRefreshed = value;
					base.OnPropertyChangedWithValue(value, "IsRefreshed");
				}
			}
		}

		[DataSourceProperty]
		public bool IsPartyLeader
		{
			get
			{
				return this._isPartyLeader;
			}
			set
			{
				if (value != this._isPartyLeader)
				{
					this._isPartyLeader = value;
					base.OnPropertyChangedWithValue(value, "IsPartyLeader");
				}
			}
		}

		[DataSourceProperty]
		public bool IsInParty
		{
			get
			{
				return this._isInParty;
			}
			set
			{
				if (value != this._isInParty)
				{
					this._isInParty = value;
					base.OnPropertyChangedWithValue(value, "IsInParty");
				}
			}
		}

		[DataSourceProperty]
		public string CreateServerText
		{
			get
			{
				return this._createServerText;
			}
			set
			{
				if (value != this._createServerText)
				{
					this._createServerText = value;
					base.OnPropertyChangedWithValue<string>(value, "CreateServerText");
				}
			}
		}

		[DataSourceProperty]
		public bool IsCustomServerActionsActive
		{
			get
			{
				return this._isCustomServerActionsActive;
			}
			set
			{
				if (value != this._isCustomServerActionsActive)
				{
					this._isCustomServerActionsActive = value;
					base.OnPropertyChangedWithValue(value, "IsCustomServerActionsActive");
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
		public string ServersInfoText
		{
			get
			{
				return this._serversInfoText;
			}
			set
			{
				if (value != this._serversInfoText)
				{
					this._serversInfoText = value;
					base.OnPropertyChangedWithValue<string>(value, "ServersInfoText");
				}
			}
		}

		[DataSourceProperty]
		public string RefreshText
		{
			get
			{
				return this._refreshText;
			}
			set
			{
				if (value != this._refreshText)
				{
					this._refreshText = value;
					base.OnPropertyChangedWithValue<string>(value, "RefreshText");
				}
			}
		}

		[DataSourceProperty]
		public string JoinText
		{
			get
			{
				return this._joinText;
			}
			set
			{
				if (value != this._joinText)
				{
					this._joinText = value;
					base.OnPropertyChangedWithValue<string>(value, "JoinText");
				}
			}
		}

		[DataSourceProperty]
		public string ServerNameText
		{
			get
			{
				return this._serverNameText;
			}
			set
			{
				if (value != this._serverNameText)
				{
					this._serverNameText = value;
					base.OnPropertyChangedWithValue<string>(value, "ServerNameText");
				}
			}
		}

		[DataSourceProperty]
		public string GameTypeText
		{
			get
			{
				return this._gameTypeText;
			}
			set
			{
				if (value != this._gameTypeText)
				{
					this._gameTypeText = value;
					base.OnPropertyChangedWithValue<string>(value, "GameTypeText");
				}
			}
		}

		[DataSourceProperty]
		public string MapText
		{
			get
			{
				return this._mapText;
			}
			set
			{
				if (value != this._mapText)
				{
					this._mapText = value;
					base.OnPropertyChangedWithValue<string>(value, "MapText");
				}
			}
		}

		[DataSourceProperty]
		public string PlayerCountText
		{
			get
			{
				return this._playerCountText;
			}
			set
			{
				if (value != this._playerCountText)
				{
					this._playerCountText = value;
					base.OnPropertyChangedWithValue<string>(value, "PlayerCountText");
				}
			}
		}

		[DataSourceProperty]
		public string PingText
		{
			get
			{
				return this._pingText;
			}
			set
			{
				if (value != this._pingText)
				{
					this._pingText = value;
					base.OnPropertyChangedWithValue<string>(value, "PingText");
				}
			}
		}

		[DataSourceProperty]
		public string PasswordText
		{
			get
			{
				return this._passwordText;
			}
			set
			{
				if (value != this._passwordText)
				{
					this._passwordText = value;
					base.OnPropertyChangedWithValue<string>(value, "PasswordText");
				}
			}
		}

		[DataSourceProperty]
		public string FirstFactionText
		{
			get
			{
				return this._firstFactionText;
			}
			set
			{
				if (value != this._firstFactionText)
				{
					this._firstFactionText = value;
					base.OnPropertyChanged("FirstFactionText");
				}
			}
		}

		[DataSourceProperty]
		public string SecondFactionText
		{
			get
			{
				return this._secondFactionText;
			}
			set
			{
				if (value != this._secondFactionText)
				{
					this._secondFactionText = value;
					base.OnPropertyChanged("SecondFactionText");
				}
			}
		}

		[DataSourceProperty]
		public string RegionText
		{
			get
			{
				return this._regionText;
			}
			set
			{
				if (value != this._regionText)
				{
					this._regionText = value;
					base.OnPropertyChanged("RegionText");
				}
			}
		}

		[DataSourceProperty]
		public string PremadeMatchTypeText
		{
			get
			{
				return this._premadeMatchTypeText;
			}
			set
			{
				if (value != this._premadeMatchTypeText)
				{
					this._premadeMatchTypeText = value;
					base.OnPropertyChanged("PremadeMatchTypeText");
				}
			}
		}

		[DataSourceProperty]
		public string HostText
		{
			get
			{
				return this._hostText;
			}
			set
			{
				if (value != this._hostText)
				{
					this._hostText = value;
					base.OnPropertyChanged("HostText");
				}
			}
		}

		[DataSourceProperty]
		public bool IsPlayerBasedCustomBattleEnabled
		{
			get
			{
				return this._isPlayerBasedCustomBattleEnabled;
			}
			set
			{
				if (this._customGameMode == MPCustomGameVM.CustomGameMode.CustomServer)
				{
					this.CreateServerText = (value ? new TextObject("{=gzdNEM76}Create a Game", null).ToString() : new TextObject("{=LrE2cUnG}Currently Disabled", null).ToString());
					if (value != this._isPlayerBasedCustomBattleEnabled)
					{
						this._isPlayerBasedCustomBattleEnabled = value;
						base.OnPropertyChangedWithValue(value, "IsPlayerBasedCustomBattleEnabled");
					}
				}
			}
		}

		public bool IsPremadeGameEnabled
		{
			get
			{
				return this._isPremadeGameEnabled;
			}
			set
			{
				if (this._customGameMode == MPCustomGameVM.CustomGameMode.PremadeGame)
				{
					this.CreateServerText = (value ? new TextObject("{=gzdNEM76}Create a Game", null).ToString() : new TextObject("{=LrE2cUnG}Currently Disabled", null).ToString());
					if (value != this._isPremadeGameEnabled)
					{
						this._isPremadeGameEnabled = value;
						base.OnPropertyChangedWithValue(value, "IsPremadeGameEnabled");
					}
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<StringPairItemWithActionVM> CustomServerActionsList
		{
			get
			{
				return this._customServerActionsList;
			}
			set
			{
				if (value != this._customServerActionsList)
				{
					this._customServerActionsList = value;
					base.OnPropertyChangedWithValue<MBBindingList<StringPairItemWithActionVM>>(value, "CustomServerActionsList");
				}
			}
		}

		private readonly LobbyState _lobbyState;

		private List<GameServerEntry> _currentCustomGameList;

		private MPCustomGameVM.CustomGameMode _customGameMode;

		private InputKeyItemVM _refreshInputKey;

		private bool _isEnabled;

		private int _selectedIndex;

		private MPCustomGameFiltersVM _filtersData;

		private MPHostGameVM _hostGame;

		private MPCustomGameSortControllerVM _sortController;

		private MBBindingList<MPCustomGameItemVM> _gameList;

		private bool _isRefreshed;

		private MBBindingList<StringPairItemWithActionVM> _customServerActionsList;

		private HintViewModel _isPasswordProtectedHint;

		private string _createServerText;

		private string _closeText;

		private string _serversInfoText;

		private string _refreshText;

		private string _joinText;

		private string _serverNameText;

		private string _gameTypeText;

		private string _mapText;

		private string _playerCountText;

		private string _pingText;

		private string _passwordText;

		private string _firstFactionText;

		private string _secondFactionText;

		private string _regionText;

		private string _premadeMatchTypeText;

		private string _hostText;

		private bool _isPlayerBasedCustomBattleEnabled;

		private bool _isPremadeGameEnabled;

		private bool _isInParty;

		private bool _isPartyLeader;

		private bool _isCustomServerActionsActive;

		public enum CustomGameMode
		{
			CustomServer,
			PremadeGame
		}
	}
}
