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
using TaleWorlds.MountAndBlade.ViewModelCollection.Input;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.HostGame;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.HostGame.HostGameOptions;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.CustomGame
{
	// Token: 0x0200008E RID: 142
	public class MPCustomGameVM : ViewModel
	{
		// Token: 0x1700042A RID: 1066
		// (get) Token: 0x06000D36 RID: 3382 RVA: 0x0002DFE3 File Offset: 0x0002C1E3
		public static bool IsPingInfoAvailable
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06000D37 RID: 3383 RVA: 0x0002DFE8 File Offset: 0x0002C1E8
		public MPCustomGameVM(LobbyState lobbyState, MPCustomGameVM.CustomGameMode customGameMode)
		{
			this._lobbyState = lobbyState;
			this._currentCustomGameList = new List<GameServerEntry>();
			this._lobbyState = lobbyState;
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

		// Token: 0x06000D38 RID: 3384 RVA: 0x0002E0A4 File Offset: 0x0002C2A4
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
			this.HostGame.RefreshValues();
		}

		// Token: 0x06000D39 RID: 3385 RVA: 0x0002E279 File Offset: 0x0002C479
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

		// Token: 0x06000D3A RID: 3386 RVA: 0x0002E2B8 File Offset: 0x0002C4B8
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

		// Token: 0x06000D3B RID: 3387 RVA: 0x0002E31D File Offset: 0x0002C51D
		public void RefreshCustomGameServerList(AvailableCustomGames availableCustomGames)
		{
			this.IsRefreshed = false;
			this._currentCustomGameList = availableCustomGames.CustomGameServerInfos;
			this.RefreshFiltersAndSort();
			this.IsRefreshed = true;
		}

		// Token: 0x06000D3C RID: 3388 RVA: 0x0002E340 File Offset: 0x0002C540
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

		// Token: 0x06000D3D RID: 3389 RVA: 0x0002E404 File Offset: 0x0002C604
		public async void ExecuteRefresh()
		{
			if (this.IsEnabled)
			{
				this.IsRefreshed = false;
				this.GameList.Clear();
				if (this._customGameMode == MPCustomGameVM.CustomGameMode.CustomServer)
				{
					await NetworkMain.GameClient.GetCustomGameServerList();
					MultiplayerOptions.Instance.CurrentOptionsCategory = MultiplayerOptions.OptionsCategory.Default;
				}
				else if (this._customGameMode == MPCustomGameVM.CustomGameMode.PremadeGame)
				{
					await NetworkMain.GameClient.GetPremadeGameList();
					MultiplayerOptions.Instance.CurrentOptionsCategory = MultiplayerOptions.OptionsCategory.PremadeMatch;
				}
				MultiplayerOptions.Instance.OnGameTypeChanged(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions);
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

		// Token: 0x06000D3E RID: 3390 RVA: 0x0002E440 File Offset: 0x0002C640
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

		// Token: 0x06000D3F RID: 3391 RVA: 0x0002E4CC File Offset: 0x0002C6CC
		private bool DoesClientRefuseToJoinCustomServer(GameServerEntry serverEntry)
		{
			string text;
			UniqueSceneId uniqueSceneId;
			return !Utilities.TryGetFullFilePathOfScene(serverEntry.Map, out text) || (serverEntry.UniqueMapId != null && (!Utilities.TryGetUniqueIdentifiersForSceneFile(text, out uniqueSceneId) || uniqueSceneId.Serialize() != serverEntry.UniqueMapId));
		}

		// Token: 0x06000D40 RID: 3392 RVA: 0x0002E514 File Offset: 0x0002C714
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

		// Token: 0x06000D41 RID: 3393 RVA: 0x0002E5D8 File Offset: 0x0002C7D8
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
				Debug.FailedAssert("Fell through game modes, should never happen", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.ViewModelCollection\\Multiplayer\\Lobby\\CustomGame\\MPCustomGameVM.cs", "GetOnTryPasswordForServerAction", 260);
			};
		}

		// Token: 0x06000D42 RID: 3394 RVA: 0x0002E660 File Offset: 0x0002C860
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

		// Token: 0x06000D43 RID: 3395 RVA: 0x0002E6A9 File Offset: 0x0002C8A9
		private void JoinPremadeGame(PremadeGameEntry selectedGame, string passwordInput = "")
		{
			NetworkMain.GameClient.RequestToJoinPremadeGame(selectedGame.Id, passwordInput);
		}

		// Token: 0x06000D44 RID: 3396 RVA: 0x0002E6BC File Offset: 0x0002C8BC
		public void SetRefreshInputKey(HotKey hotKey)
		{
			this.RefreshInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		// Token: 0x1700042B RID: 1067
		// (get) Token: 0x06000D45 RID: 3397 RVA: 0x0002E6CB File Offset: 0x0002C8CB
		// (set) Token: 0x06000D46 RID: 3398 RVA: 0x0002E6D3 File Offset: 0x0002C8D3
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

		// Token: 0x06000D47 RID: 3399 RVA: 0x0002E6F1 File Offset: 0x0002C8F1
		private void ExecuteSelectCustomServerAction(object actionParam)
		{
			(actionParam as CustomServerAction).Execute();
		}

		// Token: 0x1700042C RID: 1068
		// (get) Token: 0x06000D48 RID: 3400 RVA: 0x0002E703 File Offset: 0x0002C903
		// (set) Token: 0x06000D49 RID: 3401 RVA: 0x0002E70B File Offset: 0x0002C90B
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

		// Token: 0x1700042D RID: 1069
		// (get) Token: 0x06000D4A RID: 3402 RVA: 0x0002E737 File Offset: 0x0002C937
		// (set) Token: 0x06000D4B RID: 3403 RVA: 0x0002E73F File Offset: 0x0002C93F
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

		// Token: 0x1700042E RID: 1070
		// (get) Token: 0x06000D4C RID: 3404 RVA: 0x0002E75D File Offset: 0x0002C95D
		// (set) Token: 0x06000D4D RID: 3405 RVA: 0x0002E765 File Offset: 0x0002C965
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

		// Token: 0x1700042F RID: 1071
		// (get) Token: 0x06000D4E RID: 3406 RVA: 0x0002E783 File Offset: 0x0002C983
		// (set) Token: 0x06000D4F RID: 3407 RVA: 0x0002E78B File Offset: 0x0002C98B
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

		// Token: 0x17000430 RID: 1072
		// (get) Token: 0x06000D50 RID: 3408 RVA: 0x0002E7A9 File Offset: 0x0002C9A9
		// (set) Token: 0x06000D51 RID: 3409 RVA: 0x0002E7B1 File Offset: 0x0002C9B1
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

		// Token: 0x17000431 RID: 1073
		// (get) Token: 0x06000D52 RID: 3410 RVA: 0x0002E7CF File Offset: 0x0002C9CF
		// (set) Token: 0x06000D53 RID: 3411 RVA: 0x0002E7D7 File Offset: 0x0002C9D7
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

		// Token: 0x17000432 RID: 1074
		// (get) Token: 0x06000D54 RID: 3412 RVA: 0x0002E7F5 File Offset: 0x0002C9F5
		// (set) Token: 0x06000D55 RID: 3413 RVA: 0x0002E7FD File Offset: 0x0002C9FD
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

		// Token: 0x17000433 RID: 1075
		// (get) Token: 0x06000D56 RID: 3414 RVA: 0x0002E81B File Offset: 0x0002CA1B
		// (set) Token: 0x06000D57 RID: 3415 RVA: 0x0002E823 File Offset: 0x0002CA23
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

		// Token: 0x17000434 RID: 1076
		// (get) Token: 0x06000D58 RID: 3416 RVA: 0x0002E841 File Offset: 0x0002CA41
		// (set) Token: 0x06000D59 RID: 3417 RVA: 0x0002E849 File Offset: 0x0002CA49
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

		// Token: 0x17000435 RID: 1077
		// (get) Token: 0x06000D5A RID: 3418 RVA: 0x0002E867 File Offset: 0x0002CA67
		// (set) Token: 0x06000D5B RID: 3419 RVA: 0x0002E86F File Offset: 0x0002CA6F
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

		// Token: 0x17000436 RID: 1078
		// (get) Token: 0x06000D5C RID: 3420 RVA: 0x0002E88D File Offset: 0x0002CA8D
		// (set) Token: 0x06000D5D RID: 3421 RVA: 0x0002E895 File Offset: 0x0002CA95
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

		// Token: 0x17000437 RID: 1079
		// (get) Token: 0x06000D5E RID: 3422 RVA: 0x0002E8B8 File Offset: 0x0002CAB8
		// (set) Token: 0x06000D5F RID: 3423 RVA: 0x0002E8C0 File Offset: 0x0002CAC0
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

		// Token: 0x17000438 RID: 1080
		// (get) Token: 0x06000D60 RID: 3424 RVA: 0x0002E8DE File Offset: 0x0002CADE
		// (set) Token: 0x06000D61 RID: 3425 RVA: 0x0002E8E6 File Offset: 0x0002CAE6
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

		// Token: 0x17000439 RID: 1081
		// (get) Token: 0x06000D62 RID: 3426 RVA: 0x0002E909 File Offset: 0x0002CB09
		// (set) Token: 0x06000D63 RID: 3427 RVA: 0x0002E911 File Offset: 0x0002CB11
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

		// Token: 0x1700043A RID: 1082
		// (get) Token: 0x06000D64 RID: 3428 RVA: 0x0002E934 File Offset: 0x0002CB34
		// (set) Token: 0x06000D65 RID: 3429 RVA: 0x0002E93C File Offset: 0x0002CB3C
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

		// Token: 0x1700043B RID: 1083
		// (get) Token: 0x06000D66 RID: 3430 RVA: 0x0002E95F File Offset: 0x0002CB5F
		// (set) Token: 0x06000D67 RID: 3431 RVA: 0x0002E967 File Offset: 0x0002CB67
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

		// Token: 0x1700043C RID: 1084
		// (get) Token: 0x06000D68 RID: 3432 RVA: 0x0002E98A File Offset: 0x0002CB8A
		// (set) Token: 0x06000D69 RID: 3433 RVA: 0x0002E992 File Offset: 0x0002CB92
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

		// Token: 0x1700043D RID: 1085
		// (get) Token: 0x06000D6A RID: 3434 RVA: 0x0002E9B5 File Offset: 0x0002CBB5
		// (set) Token: 0x06000D6B RID: 3435 RVA: 0x0002E9BD File Offset: 0x0002CBBD
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

		// Token: 0x1700043E RID: 1086
		// (get) Token: 0x06000D6C RID: 3436 RVA: 0x0002E9E0 File Offset: 0x0002CBE0
		// (set) Token: 0x06000D6D RID: 3437 RVA: 0x0002E9E8 File Offset: 0x0002CBE8
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

		// Token: 0x1700043F RID: 1087
		// (get) Token: 0x06000D6E RID: 3438 RVA: 0x0002EA0B File Offset: 0x0002CC0B
		// (set) Token: 0x06000D6F RID: 3439 RVA: 0x0002EA13 File Offset: 0x0002CC13
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

		// Token: 0x17000440 RID: 1088
		// (get) Token: 0x06000D70 RID: 3440 RVA: 0x0002EA36 File Offset: 0x0002CC36
		// (set) Token: 0x06000D71 RID: 3441 RVA: 0x0002EA3E File Offset: 0x0002CC3E
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

		// Token: 0x17000441 RID: 1089
		// (get) Token: 0x06000D72 RID: 3442 RVA: 0x0002EA61 File Offset: 0x0002CC61
		// (set) Token: 0x06000D73 RID: 3443 RVA: 0x0002EA69 File Offset: 0x0002CC69
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

		// Token: 0x17000442 RID: 1090
		// (get) Token: 0x06000D74 RID: 3444 RVA: 0x0002EA8C File Offset: 0x0002CC8C
		// (set) Token: 0x06000D75 RID: 3445 RVA: 0x0002EA94 File Offset: 0x0002CC94
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

		// Token: 0x17000443 RID: 1091
		// (get) Token: 0x06000D76 RID: 3446 RVA: 0x0002EAB6 File Offset: 0x0002CCB6
		// (set) Token: 0x06000D77 RID: 3447 RVA: 0x0002EABE File Offset: 0x0002CCBE
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

		// Token: 0x17000444 RID: 1092
		// (get) Token: 0x06000D78 RID: 3448 RVA: 0x0002EAE0 File Offset: 0x0002CCE0
		// (set) Token: 0x06000D79 RID: 3449 RVA: 0x0002EAE8 File Offset: 0x0002CCE8
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

		// Token: 0x17000445 RID: 1093
		// (get) Token: 0x06000D7A RID: 3450 RVA: 0x0002EB0A File Offset: 0x0002CD0A
		// (set) Token: 0x06000D7B RID: 3451 RVA: 0x0002EB12 File Offset: 0x0002CD12
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

		// Token: 0x17000446 RID: 1094
		// (get) Token: 0x06000D7C RID: 3452 RVA: 0x0002EB34 File Offset: 0x0002CD34
		// (set) Token: 0x06000D7D RID: 3453 RVA: 0x0002EB3C File Offset: 0x0002CD3C
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

		// Token: 0x17000447 RID: 1095
		// (get) Token: 0x06000D7E RID: 3454 RVA: 0x0002EB5E File Offset: 0x0002CD5E
		// (set) Token: 0x06000D7F RID: 3455 RVA: 0x0002EB68 File Offset: 0x0002CD68
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

		// Token: 0x17000448 RID: 1096
		// (get) Token: 0x06000D80 RID: 3456 RVA: 0x0002EBC4 File Offset: 0x0002CDC4
		// (set) Token: 0x06000D81 RID: 3457 RVA: 0x0002EBCC File Offset: 0x0002CDCC
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

		// Token: 0x17000449 RID: 1097
		// (get) Token: 0x06000D82 RID: 3458 RVA: 0x0002EC29 File Offset: 0x0002CE29
		// (set) Token: 0x06000D83 RID: 3459 RVA: 0x0002EC31 File Offset: 0x0002CE31
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

		// Token: 0x04000659 RID: 1625
		private readonly LobbyState _lobbyState;

		// Token: 0x0400065A RID: 1626
		private List<GameServerEntry> _currentCustomGameList;

		// Token: 0x0400065B RID: 1627
		private MPCustomGameVM.CustomGameMode _customGameMode;

		// Token: 0x0400065C RID: 1628
		private InputKeyItemVM _refreshInputKey;

		// Token: 0x0400065D RID: 1629
		private bool _isEnabled;

		// Token: 0x0400065E RID: 1630
		private int _selectedIndex;

		// Token: 0x0400065F RID: 1631
		private MPCustomGameFiltersVM _filtersData;

		// Token: 0x04000660 RID: 1632
		private MPHostGameVM _hostGame;

		// Token: 0x04000661 RID: 1633
		private MPCustomGameSortControllerVM _sortController;

		// Token: 0x04000662 RID: 1634
		private MBBindingList<MPCustomGameItemVM> _gameList;

		// Token: 0x04000663 RID: 1635
		private bool _isRefreshed;

		// Token: 0x04000664 RID: 1636
		private MBBindingList<StringPairItemWithActionVM> _customServerActionsList;

		// Token: 0x04000665 RID: 1637
		private HintViewModel _isPasswordProtectedHint;

		// Token: 0x04000666 RID: 1638
		private string _createServerText;

		// Token: 0x04000667 RID: 1639
		private string _closeText;

		// Token: 0x04000668 RID: 1640
		private string _serversInfoText;

		// Token: 0x04000669 RID: 1641
		private string _refreshText;

		// Token: 0x0400066A RID: 1642
		private string _joinText;

		// Token: 0x0400066B RID: 1643
		private string _serverNameText;

		// Token: 0x0400066C RID: 1644
		private string _gameTypeText;

		// Token: 0x0400066D RID: 1645
		private string _mapText;

		// Token: 0x0400066E RID: 1646
		private string _playerCountText;

		// Token: 0x0400066F RID: 1647
		private string _pingText;

		// Token: 0x04000670 RID: 1648
		private string _passwordText;

		// Token: 0x04000671 RID: 1649
		private string _firstFactionText;

		// Token: 0x04000672 RID: 1650
		private string _secondFactionText;

		// Token: 0x04000673 RID: 1651
		private string _regionText;

		// Token: 0x04000674 RID: 1652
		private string _premadeMatchTypeText;

		// Token: 0x04000675 RID: 1653
		private string _hostText;

		// Token: 0x04000676 RID: 1654
		private bool _isPlayerBasedCustomBattleEnabled;

		// Token: 0x04000677 RID: 1655
		private bool _isPremadeGameEnabled;

		// Token: 0x04000678 RID: 1656
		private bool _isInParty;

		// Token: 0x04000679 RID: 1657
		private bool _isPartyLeader;

		// Token: 0x0400067A RID: 1658
		private bool _isCustomServerActionsActive;

		// Token: 0x020001E1 RID: 481
		public enum CustomGameMode
		{
			// Token: 0x04000DF7 RID: 3575
			CustomServer,
			// Token: 0x04000DF8 RID: 3576
			PremadeGame
		}
	}
}
