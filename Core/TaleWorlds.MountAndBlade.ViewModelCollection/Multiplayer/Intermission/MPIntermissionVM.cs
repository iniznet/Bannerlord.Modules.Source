using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Intermission
{
	// Token: 0x020000B6 RID: 182
	public class MPIntermissionVM : ViewModel
	{
		// Token: 0x06001106 RID: 4358 RVA: 0x000380A4 File Offset: 0x000362A4
		public MPIntermissionVM()
		{
			this.AvailableMaps = new MBBindingList<MPIntermissionMapItemVM>();
			this.AvailableCultures = new MBBindingList<MPIntermissionCultureItemVM>();
			this.RefreshValues();
		}

		// Token: 0x06001107 RID: 4359 RVA: 0x00038128 File Offset: 0x00036328
		public override void RefreshValues()
		{
			this.QuitText = new TextObject("{=3sRdGQou}Leave", null).ToString();
			this.PlayersLabel = new TextObject("{=RfXJdNye}Players", null).ToString();
			this.MapVoteText = new TextObject("{=DraJ6bxq}Vote for the Next Map", null).ToString();
			this.CultureVoteText = new TextObject("{=oF27vprQ}Vote for the Next Culture", null).ToString();
		}

		// Token: 0x06001108 RID: 4360 RVA: 0x00038190 File Offset: 0x00036390
		public void Tick()
		{
			if (!this._hasBaseNetworkComponentSet)
			{
				this._baseNetworkComponent = GameNetwork.GetNetworkComponent<BaseNetworkComponent>();
				if (this._baseNetworkComponent != null)
				{
					this._hasBaseNetworkComponentSet = true;
					BaseNetworkComponent baseNetworkComponent = this._baseNetworkComponent;
					baseNetworkComponent.OnIntermissionStateUpdated = (Action)Delegate.Combine(baseNetworkComponent.OnIntermissionStateUpdated, new Action(this.OnIntermissionStateUpdated));
					return;
				}
			}
			else if (this._baseNetworkComponent.ClientIntermissionState == MultiplayerIntermissionState.Idle)
			{
				this.NextGameStateTimerLabel = this._serverIdleLabelText.ToString();
				this.NextGameStateTimerValue = string.Empty;
				this.IsMissionTimerEnabled = false;
				this.IsEndGameTimerEnabled = false;
				this.IsNextMapInfoEnabled = false;
				this.IsMapVoteEnabled = false;
				this.IsCultureVoteEnabled = false;
				this.IsPlayerCountEnabled = false;
			}
		}

		// Token: 0x06001109 RID: 4361 RVA: 0x0003823D File Offset: 0x0003643D
		public override void OnFinalize()
		{
			if (this._baseNetworkComponent != null)
			{
				BaseNetworkComponent baseNetworkComponent = this._baseNetworkComponent;
				baseNetworkComponent.OnIntermissionStateUpdated = (Action)Delegate.Remove(baseNetworkComponent.OnIntermissionStateUpdated, new Action(this.OnIntermissionStateUpdated));
			}
			MultiplayerIntermissionVotingManager.Instance.ClearItems();
		}

		// Token: 0x0600110A RID: 4362 RVA: 0x00038278 File Offset: 0x00036478
		private void OnIntermissionStateUpdated()
		{
			this._currentIntermissionState = this._baseNetworkComponent.ClientIntermissionState;
			bool flag = true;
			if (this._currentIntermissionState == MultiplayerIntermissionState.CountingForMapVote)
			{
				int num = (int)this._baseNetworkComponent.CurrentIntermissionTimer;
				this.NextGameStateTimerLabel = this._voteLabelText.ToString();
				this.NextGameStateTimerValue = num.ToString();
				this.IsMissionTimerEnabled = true;
				this.IsEndGameTimerEnabled = false;
				this.IsNextMapInfoEnabled = false;
				this.IsCultureVoteEnabled = false;
				this.IsPlayerCountEnabled = true;
				flag = false;
				List<IntermissionVoteItem> mapVoteItems = MultiplayerIntermissionVotingManager.Instance.MapVoteItems;
				if (mapVoteItems.Count > 0)
				{
					this.IsMapVoteEnabled = true;
					using (List<IntermissionVoteItem>.Enumerator enumerator = mapVoteItems.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							IntermissionVoteItem mapItem = enumerator.Current;
							if (this.AvailableMaps.FirstOrDefault((MPIntermissionMapItemVM m) => m.MapID == mapItem.Id) == null)
							{
								this.AvailableMaps.Add(new MPIntermissionMapItemVM(mapItem.Id, new Action<MPIntermissionMapItemVM>(this.OnPlayerVotedForMap)));
							}
							int voteCount = mapItem.VoteCount;
							this.AvailableMaps.First((MPIntermissionMapItemVM m) => m.MapID == mapItem.Id).Votes = voteCount;
						}
					}
				}
			}
			if (this._baseNetworkComponent.ClientIntermissionState == MultiplayerIntermissionState.CountingForCultureVote)
			{
				int num2 = (int)this._baseNetworkComponent.CurrentIntermissionTimer;
				this.NextGameStateTimerLabel = this._voteLabelText.ToString();
				this.NextGameStateTimerValue = num2.ToString();
				this.IsMissionTimerEnabled = true;
				this.IsEndGameTimerEnabled = false;
				this.IsNextMapInfoEnabled = false;
				this.IsMapVoteEnabled = false;
				this.IsPlayerCountEnabled = true;
				flag = false;
				List<IntermissionVoteItem> cultureVoteItems = MultiplayerIntermissionVotingManager.Instance.CultureVoteItems;
				if (cultureVoteItems.Count > 0)
				{
					this.IsCultureVoteEnabled = true;
					using (List<IntermissionVoteItem>.Enumerator enumerator = cultureVoteItems.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							IntermissionVoteItem cultureItem = enumerator.Current;
							if (this.AvailableCultures.FirstOrDefault((MPIntermissionCultureItemVM c) => c.CultureCode == cultureItem.Id) == null)
							{
								this.AvailableCultures.Add(new MPIntermissionCultureItemVM(cultureItem.Id, new Action<MPIntermissionCultureItemVM>(this.OnPlayerVotedForCulture)));
							}
							int voteCount2 = cultureItem.VoteCount;
							this.AvailableCultures.FirstOrDefault((MPIntermissionCultureItemVM c) => c.CultureCode == cultureItem.Id).Votes = voteCount2;
						}
					}
				}
				string text;
				MultiplayerOptions.Instance.GetOptionFromOptionType(MultiplayerOptions.OptionType.CultureTeam1, MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions).GetValue(out text);
				string text2;
				MultiplayerOptions.Instance.GetOptionFromOptionType(MultiplayerOptions.OptionType.CultureTeam2, MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions).GetValue(out text2);
				this.NextFactionACultureID = text;
				this.NextFactionBCultureID = text2;
			}
			if (this._currentIntermissionState == MultiplayerIntermissionState.CountingForMission)
			{
				int num3 = (int)this._baseNetworkComponent.CurrentIntermissionTimer;
				this.NextGameStateTimerLabel = this._nextGameLabelText.ToString();
				this.NextGameStateTimerValue = num3.ToString();
				this.IsMissionTimerEnabled = true;
				this.IsEndGameTimerEnabled = false;
				this.IsNextMapInfoEnabled = true;
				this.IsMapVoteEnabled = false;
				this.IsCultureVoteEnabled = false;
				this.IsPlayerCountEnabled = true;
				flag = true;
				this.AvailableMaps.Clear();
				this.AvailableCultures.Clear();
				MultiplayerIntermissionVotingManager.Instance.ClearVotes();
				this._votedMapItem = null;
				this._votedCultureItem = null;
			}
			if (this._currentIntermissionState == MultiplayerIntermissionState.CountingForEnd)
			{
				TextObject textObject = GameTexts.FindText("str_string_newline_string", null);
				textObject.SetTextVariable("STR1", this._matchFinishedText.ToString());
				textObject.SetTextVariable("STR2", this._returningToLobbyText.ToString());
				this.NextGameStateTimerLabel = textObject.ToString();
				this.NextGameStateTimerValue = string.Empty;
				this.IsMissionTimerEnabled = false;
				this.IsEndGameTimerEnabled = false;
				this.IsNextMapInfoEnabled = false;
				this.IsMapVoteEnabled = false;
				this.IsCultureVoteEnabled = false;
				this.IsPlayerCountEnabled = false;
				flag = false;
			}
			string text3;
			MultiplayerOptions.Instance.GetOptionFromOptionType(MultiplayerOptions.OptionType.Map, MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions).GetValue(out text3);
			this.NextMapID = (this.IsEndGameTimerEnabled ? string.Empty : text3);
			this.NextMapName = (this.IsEndGameTimerEnabled ? string.Empty : GameTexts.FindText("str_multiplayer_scene_name", text3).ToString());
			if (flag)
			{
				string text4;
				MultiplayerOptions.Instance.GetOptionFromOptionType(MultiplayerOptions.OptionType.CultureTeam1, MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions).GetValue(out text4);
				this.IsFactionAValid = !this.IsEndGameTimerEnabled && !string.IsNullOrEmpty(text4) && this._currentIntermissionState != MultiplayerIntermissionState.CountingForMapVote;
				this.NextFactionACultureID = (this.IsEndGameTimerEnabled ? string.Empty : text4);
				string text5;
				MultiplayerOptions.Instance.GetOptionFromOptionType(MultiplayerOptions.OptionType.CultureTeam2, MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions).GetValue(out text5);
				this.IsFactionBValid = !this.IsEndGameTimerEnabled && !string.IsNullOrEmpty(text5) && this._currentIntermissionState != MultiplayerIntermissionState.CountingForMapVote;
				this.NextFactionBCultureID = (this.IsEndGameTimerEnabled ? string.Empty : text5);
			}
			else
			{
				this.IsFactionAValid = false;
				this.IsFactionBValid = false;
			}
			string text6;
			MultiplayerOptions.Instance.GetOptionFromOptionType(MultiplayerOptions.OptionType.ServerName, MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions).GetValue(out text6);
			this.ServerName = text6;
			string text7;
			MultiplayerOptions.Instance.GetOptionFromOptionType(MultiplayerOptions.OptionType.GameType, MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions).GetValue(out text7);
			this.NextGameType = (this.IsEndGameTimerEnabled ? string.Empty : GameTexts.FindText("str_multiplayer_game_type", text7).ToString());
			string text8;
			MultiplayerOptions.Instance.GetOptionFromOptionType(MultiplayerOptions.OptionType.WelcomeMessage, MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions).GetValue(out text8);
			this.WelcomeMessage = (this.IsEndGameTimerEnabled ? string.Empty : text8);
			int num4;
			MultiplayerOptions.Instance.GetOptionFromOptionType(MultiplayerOptions.OptionType.MaxNumberOfPlayers, MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions).GetValue(out num4);
			this.MaxNumPlayersValueText = num4.ToString();
			this.ConnectedPlayersCountValueText = MBNetwork.NetworkPeers.Count.ToString();
		}

		// Token: 0x0600110B RID: 4363 RVA: 0x00038824 File Offset: 0x00036A24
		public void ExecuteQuitServer()
		{
			LobbyClient gameClient = NetworkMain.GameClient;
			if (gameClient.CurrentState == LobbyClient.State.InCustomGame)
			{
				gameClient.QuitFromCustomGame();
			}
			MultiplayerIntermissionVotingManager.Instance.ClearItems();
		}

		// Token: 0x0600110C RID: 4364 RVA: 0x00038854 File Offset: 0x00036A54
		private void OnPlayerVotedForMap(MPIntermissionMapItemVM mapItem)
		{
			int num;
			if (this._votedMapItem != null)
			{
				this._baseNetworkComponent.IntermissionCastVote(this._votedMapItem.MapID, -1);
				this._votedMapItem.IsSelected = false;
				MPIntermissionMapItemVM votedMapItem = this._votedMapItem;
				num = votedMapItem.Votes;
				votedMapItem.Votes = num - 1;
			}
			this._baseNetworkComponent.IntermissionCastVote(mapItem.MapID, 1);
			this._votedMapItem = mapItem;
			this._votedMapItem.IsSelected = true;
			MPIntermissionMapItemVM votedMapItem2 = this._votedMapItem;
			num = votedMapItem2.Votes;
			votedMapItem2.Votes = num + 1;
		}

		// Token: 0x0600110D RID: 4365 RVA: 0x000388DC File Offset: 0x00036ADC
		private void OnPlayerVotedForCulture(MPIntermissionCultureItemVM cultureItem)
		{
			int num;
			if (this._votedCultureItem != null)
			{
				this._baseNetworkComponent.IntermissionCastVote(this._votedCultureItem.CultureCode, -1);
				this._votedCultureItem.IsSelected = false;
				MPIntermissionCultureItemVM votedCultureItem = this._votedCultureItem;
				num = votedCultureItem.Votes;
				votedCultureItem.Votes = num - 1;
			}
			this._baseNetworkComponent.IntermissionCastVote(cultureItem.CultureCode, 1);
			this._votedCultureItem = cultureItem;
			this._votedCultureItem.IsSelected = true;
			MPIntermissionCultureItemVM votedCultureItem2 = this._votedCultureItem;
			num = votedCultureItem2.Votes;
			votedCultureItem2.Votes = num + 1;
		}

		// Token: 0x1700057C RID: 1404
		// (get) Token: 0x0600110E RID: 4366 RVA: 0x00038963 File Offset: 0x00036B63
		// (set) Token: 0x0600110F RID: 4367 RVA: 0x0003896B File Offset: 0x00036B6B
		[DataSourceProperty]
		public string ConnectedPlayersCountValueText
		{
			get
			{
				return this._connectedPlayersCountValueText;
			}
			set
			{
				if (value != this._connectedPlayersCountValueText)
				{
					this._connectedPlayersCountValueText = value;
					base.OnPropertyChangedWithValue<string>(value, "ConnectedPlayersCountValueText");
				}
			}
		}

		// Token: 0x1700057D RID: 1405
		// (get) Token: 0x06001110 RID: 4368 RVA: 0x0003898E File Offset: 0x00036B8E
		// (set) Token: 0x06001111 RID: 4369 RVA: 0x00038996 File Offset: 0x00036B96
		[DataSourceProperty]
		public string MaxNumPlayersValueText
		{
			get
			{
				return this._maxNumPlayersValueText;
			}
			set
			{
				if (value != this._maxNumPlayersValueText)
				{
					this._maxNumPlayersValueText = value;
					base.OnPropertyChangedWithValue<string>(value, "MaxNumPlayersValueText");
				}
			}
		}

		// Token: 0x1700057E RID: 1406
		// (get) Token: 0x06001112 RID: 4370 RVA: 0x000389B9 File Offset: 0x00036BB9
		// (set) Token: 0x06001113 RID: 4371 RVA: 0x000389C1 File Offset: 0x00036BC1
		[DataSourceProperty]
		public bool IsFactionAValid
		{
			get
			{
				return this._isFactionAValid;
			}
			set
			{
				if (value != this._isFactionAValid)
				{
					this._isFactionAValid = value;
					base.OnPropertyChangedWithValue(value, "IsFactionAValid");
				}
			}
		}

		// Token: 0x1700057F RID: 1407
		// (get) Token: 0x06001114 RID: 4372 RVA: 0x000389DF File Offset: 0x00036BDF
		// (set) Token: 0x06001115 RID: 4373 RVA: 0x000389E7 File Offset: 0x00036BE7
		[DataSourceProperty]
		public bool IsFactionBValid
		{
			get
			{
				return this._isFactionBValid;
			}
			set
			{
				if (value != this._isFactionBValid)
				{
					this._isFactionBValid = value;
					base.OnPropertyChangedWithValue(value, "IsFactionBValid");
				}
			}
		}

		// Token: 0x17000580 RID: 1408
		// (get) Token: 0x06001116 RID: 4374 RVA: 0x00038A05 File Offset: 0x00036C05
		// (set) Token: 0x06001117 RID: 4375 RVA: 0x00038A0D File Offset: 0x00036C0D
		[DataSourceProperty]
		public bool IsMissionTimerEnabled
		{
			get
			{
				return this._isMissionTimerEnabled;
			}
			set
			{
				if (value != this._isMissionTimerEnabled)
				{
					this._isMissionTimerEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsMissionTimerEnabled");
				}
			}
		}

		// Token: 0x17000581 RID: 1409
		// (get) Token: 0x06001118 RID: 4376 RVA: 0x00038A2B File Offset: 0x00036C2B
		// (set) Token: 0x06001119 RID: 4377 RVA: 0x00038A33 File Offset: 0x00036C33
		[DataSourceProperty]
		public bool IsEndGameTimerEnabled
		{
			get
			{
				return this._isEndGameTimerEnabled;
			}
			set
			{
				if (value != this._isEndGameTimerEnabled)
				{
					this._isEndGameTimerEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsEndGameTimerEnabled");
				}
			}
		}

		// Token: 0x17000582 RID: 1410
		// (get) Token: 0x0600111A RID: 4378 RVA: 0x00038A51 File Offset: 0x00036C51
		// (set) Token: 0x0600111B RID: 4379 RVA: 0x00038A59 File Offset: 0x00036C59
		[DataSourceProperty]
		public bool IsNextMapInfoEnabled
		{
			get
			{
				return this._isNextMapInfoEnabled;
			}
			set
			{
				if (value != this._isNextMapInfoEnabled)
				{
					this._isNextMapInfoEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsNextMapInfoEnabled");
				}
			}
		}

		// Token: 0x17000583 RID: 1411
		// (get) Token: 0x0600111C RID: 4380 RVA: 0x00038A77 File Offset: 0x00036C77
		// (set) Token: 0x0600111D RID: 4381 RVA: 0x00038A7F File Offset: 0x00036C7F
		[DataSourceProperty]
		public bool IsMapVoteEnabled
		{
			get
			{
				return this._isMapVoteEnabled;
			}
			set
			{
				if (value != this._isMapVoteEnabled)
				{
					this._isMapVoteEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsMapVoteEnabled");
				}
			}
		}

		// Token: 0x17000584 RID: 1412
		// (get) Token: 0x0600111E RID: 4382 RVA: 0x00038A9D File Offset: 0x00036C9D
		// (set) Token: 0x0600111F RID: 4383 RVA: 0x00038AA5 File Offset: 0x00036CA5
		[DataSourceProperty]
		public bool IsCultureVoteEnabled
		{
			get
			{
				return this._isCultureVoteEnabled;
			}
			set
			{
				if (value != this._isCultureVoteEnabled)
				{
					this._isCultureVoteEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsCultureVoteEnabled");
				}
			}
		}

		// Token: 0x17000585 RID: 1413
		// (get) Token: 0x06001120 RID: 4384 RVA: 0x00038AC3 File Offset: 0x00036CC3
		// (set) Token: 0x06001121 RID: 4385 RVA: 0x00038ACB File Offset: 0x00036CCB
		[DataSourceProperty]
		public bool IsPlayerCountEnabled
		{
			get
			{
				return this._isPlayerCountEnabled;
			}
			set
			{
				if (value != this._isPlayerCountEnabled)
				{
					this._isPlayerCountEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsPlayerCountEnabled");
				}
			}
		}

		// Token: 0x17000586 RID: 1414
		// (get) Token: 0x06001122 RID: 4386 RVA: 0x00038AE9 File Offset: 0x00036CE9
		// (set) Token: 0x06001123 RID: 4387 RVA: 0x00038AF1 File Offset: 0x00036CF1
		[DataSourceProperty]
		public string NextMapID
		{
			get
			{
				return this._nextMapId;
			}
			set
			{
				if (value != this._nextMapId)
				{
					this._nextMapId = value;
					base.OnPropertyChangedWithValue<string>(value, "NextMapID");
				}
			}
		}

		// Token: 0x17000587 RID: 1415
		// (get) Token: 0x06001124 RID: 4388 RVA: 0x00038B14 File Offset: 0x00036D14
		// (set) Token: 0x06001125 RID: 4389 RVA: 0x00038B1C File Offset: 0x00036D1C
		[DataSourceProperty]
		public string NextFactionACultureID
		{
			get
			{
				return this._nextFactionACultureId;
			}
			set
			{
				if (value != this._nextFactionACultureId)
				{
					this._nextFactionACultureId = value;
					base.OnPropertyChangedWithValue<string>(value, "NextFactionACultureID");
				}
			}
		}

		// Token: 0x17000588 RID: 1416
		// (get) Token: 0x06001126 RID: 4390 RVA: 0x00038B3F File Offset: 0x00036D3F
		// (set) Token: 0x06001127 RID: 4391 RVA: 0x00038B47 File Offset: 0x00036D47
		[DataSourceProperty]
		public string NextFactionBCultureID
		{
			get
			{
				return this._nextFactionBCultureId;
			}
			set
			{
				if (value != this._nextFactionBCultureId)
				{
					this._nextFactionBCultureId = value;
					base.OnPropertyChangedWithValue<string>(value, "NextFactionBCultureID");
				}
			}
		}

		// Token: 0x17000589 RID: 1417
		// (get) Token: 0x06001128 RID: 4392 RVA: 0x00038B6A File Offset: 0x00036D6A
		// (set) Token: 0x06001129 RID: 4393 RVA: 0x00038B72 File Offset: 0x00036D72
		[DataSourceProperty]
		public string PlayersLabel
		{
			get
			{
				return this._playersLabel;
			}
			set
			{
				if (value != this._playersLabel)
				{
					this._playersLabel = value;
					base.OnPropertyChangedWithValue<string>(value, "PlayersLabel");
				}
			}
		}

		// Token: 0x1700058A RID: 1418
		// (get) Token: 0x0600112A RID: 4394 RVA: 0x00038B95 File Offset: 0x00036D95
		// (set) Token: 0x0600112B RID: 4395 RVA: 0x00038B9D File Offset: 0x00036D9D
		[DataSourceProperty]
		public string MapVoteText
		{
			get
			{
				return this._mapVoteText;
			}
			set
			{
				if (value != this._mapVoteText)
				{
					this._mapVoteText = value;
					base.OnPropertyChangedWithValue<string>(value, "MapVoteText");
				}
			}
		}

		// Token: 0x1700058B RID: 1419
		// (get) Token: 0x0600112C RID: 4396 RVA: 0x00038BC0 File Offset: 0x00036DC0
		// (set) Token: 0x0600112D RID: 4397 RVA: 0x00038BC8 File Offset: 0x00036DC8
		[DataSourceProperty]
		public string CultureVoteText
		{
			get
			{
				return this._cultureVoteText;
			}
			set
			{
				if (value != this._cultureVoteText)
				{
					this._cultureVoteText = value;
					base.OnPropertyChangedWithValue<string>(value, "CultureVoteText");
				}
			}
		}

		// Token: 0x1700058C RID: 1420
		// (get) Token: 0x0600112E RID: 4398 RVA: 0x00038BEB File Offset: 0x00036DEB
		// (set) Token: 0x0600112F RID: 4399 RVA: 0x00038BF3 File Offset: 0x00036DF3
		[DataSourceProperty]
		public string NextGameStateTimerLabel
		{
			get
			{
				return this._nextGameStateTimerLabel;
			}
			set
			{
				if (value != this._nextGameStateTimerLabel)
				{
					this._nextGameStateTimerLabel = value;
					base.OnPropertyChangedWithValue<string>(value, "NextGameStateTimerLabel");
				}
			}
		}

		// Token: 0x1700058D RID: 1421
		// (get) Token: 0x06001130 RID: 4400 RVA: 0x00038C16 File Offset: 0x00036E16
		// (set) Token: 0x06001131 RID: 4401 RVA: 0x00038C1E File Offset: 0x00036E1E
		[DataSourceProperty]
		public string NextGameStateTimerValue
		{
			get
			{
				return this._nextGameStateTimerValue;
			}
			set
			{
				if (value != this._nextGameStateTimerValue)
				{
					this._nextGameStateTimerValue = value;
					base.OnPropertyChangedWithValue<string>(value, "NextGameStateTimerValue");
				}
			}
		}

		// Token: 0x1700058E RID: 1422
		// (get) Token: 0x06001132 RID: 4402 RVA: 0x00038C41 File Offset: 0x00036E41
		// (set) Token: 0x06001133 RID: 4403 RVA: 0x00038C49 File Offset: 0x00036E49
		[DataSourceProperty]
		public string WelcomeMessage
		{
			get
			{
				return this._welcomeMessage;
			}
			set
			{
				if (value != this._welcomeMessage)
				{
					this._welcomeMessage = value;
					base.OnPropertyChangedWithValue<string>(value, "WelcomeMessage");
				}
			}
		}

		// Token: 0x1700058F RID: 1423
		// (get) Token: 0x06001134 RID: 4404 RVA: 0x00038C6C File Offset: 0x00036E6C
		// (set) Token: 0x06001135 RID: 4405 RVA: 0x00038C74 File Offset: 0x00036E74
		[DataSourceProperty]
		public string ServerName
		{
			get
			{
				return this._serverName;
			}
			set
			{
				if (value != this._serverName)
				{
					this._serverName = value;
					base.OnPropertyChangedWithValue<string>(value, "ServerName");
				}
			}
		}

		// Token: 0x17000590 RID: 1424
		// (get) Token: 0x06001136 RID: 4406 RVA: 0x00038C97 File Offset: 0x00036E97
		// (set) Token: 0x06001137 RID: 4407 RVA: 0x00038C9F File Offset: 0x00036E9F
		[DataSourceProperty]
		public string NextGameType
		{
			get
			{
				return this._nextGameType;
			}
			set
			{
				if (value != this._nextGameType)
				{
					this._nextGameType = value;
					base.OnPropertyChangedWithValue<string>(value, "NextGameType");
				}
			}
		}

		// Token: 0x17000591 RID: 1425
		// (get) Token: 0x06001138 RID: 4408 RVA: 0x00038CC2 File Offset: 0x00036EC2
		// (set) Token: 0x06001139 RID: 4409 RVA: 0x00038CCA File Offset: 0x00036ECA
		[DataSourceProperty]
		public string NextMapName
		{
			get
			{
				return this._nextMapName;
			}
			set
			{
				if (value != this._nextMapName)
				{
					this._nextMapName = value;
					base.OnPropertyChangedWithValue<string>(value, "NextMapName");
				}
			}
		}

		// Token: 0x17000592 RID: 1426
		// (get) Token: 0x0600113A RID: 4410 RVA: 0x00038CED File Offset: 0x00036EED
		// (set) Token: 0x0600113B RID: 4411 RVA: 0x00038CF5 File Offset: 0x00036EF5
		[DataSourceProperty]
		public MBBindingList<MPIntermissionMapItemVM> AvailableMaps
		{
			get
			{
				return this._availableMaps;
			}
			set
			{
				if (value != this._availableMaps)
				{
					this._availableMaps = value;
					base.OnPropertyChangedWithValue<MBBindingList<MPIntermissionMapItemVM>>(value, "AvailableMaps");
				}
			}
		}

		// Token: 0x17000593 RID: 1427
		// (get) Token: 0x0600113C RID: 4412 RVA: 0x00038D13 File Offset: 0x00036F13
		// (set) Token: 0x0600113D RID: 4413 RVA: 0x00038D1B File Offset: 0x00036F1B
		[DataSourceProperty]
		public MBBindingList<MPIntermissionCultureItemVM> AvailableCultures
		{
			get
			{
				return this._availableCultures;
			}
			set
			{
				if (value != this._availableCultures)
				{
					this._availableCultures = value;
					base.OnPropertyChangedWithValue<MBBindingList<MPIntermissionCultureItemVM>>(value, "AvailableCultures");
				}
			}
		}

		// Token: 0x17000594 RID: 1428
		// (get) Token: 0x0600113E RID: 4414 RVA: 0x00038D39 File Offset: 0x00036F39
		// (set) Token: 0x0600113F RID: 4415 RVA: 0x00038D41 File Offset: 0x00036F41
		[DataSourceProperty]
		public string QuitText
		{
			get
			{
				return this._quitText;
			}
			set
			{
				if (value != this._quitText)
				{
					this._quitText = value;
					base.OnPropertyChangedWithValue<string>(value, "QuitText");
				}
			}
		}

		// Token: 0x04000813 RID: 2067
		private bool _hasBaseNetworkComponentSet;

		// Token: 0x04000814 RID: 2068
		private BaseNetworkComponent _baseNetworkComponent;

		// Token: 0x04000815 RID: 2069
		private MultiplayerIntermissionState _currentIntermissionState;

		// Token: 0x04000816 RID: 2070
		private readonly TextObject _voteLabelText = new TextObject("{=KOVHgkVq}Voting Ends In:", null);

		// Token: 0x04000817 RID: 2071
		private readonly TextObject _nextGameLabelText = new TextObject("{=lX9Qx7Wo}Next Game Starts In:", null);

		// Token: 0x04000818 RID: 2072
		private readonly TextObject _serverIdleLabelText = new TextObject("{=Rhcberxf}Awaiting Server", null);

		// Token: 0x04000819 RID: 2073
		private readonly TextObject _matchFinishedText = new TextObject("{=RbazQjFt}Match is Finished", null);

		// Token: 0x0400081A RID: 2074
		private readonly TextObject _returningToLobbyText = new TextObject("{=1UaxKbn6}Returning to the Lobby...", null);

		// Token: 0x0400081B RID: 2075
		private MPIntermissionMapItemVM _votedMapItem;

		// Token: 0x0400081C RID: 2076
		private MPIntermissionCultureItemVM _votedCultureItem;

		// Token: 0x0400081D RID: 2077
		private string _connectedPlayersCountValueText;

		// Token: 0x0400081E RID: 2078
		private string _maxNumPlayersValueText;

		// Token: 0x0400081F RID: 2079
		private bool _isFactionAValid;

		// Token: 0x04000820 RID: 2080
		private bool _isFactionBValid;

		// Token: 0x04000821 RID: 2081
		private bool _isMissionTimerEnabled;

		// Token: 0x04000822 RID: 2082
		private bool _isEndGameTimerEnabled;

		// Token: 0x04000823 RID: 2083
		private bool _isNextMapInfoEnabled;

		// Token: 0x04000824 RID: 2084
		private bool _isMapVoteEnabled;

		// Token: 0x04000825 RID: 2085
		private bool _isCultureVoteEnabled;

		// Token: 0x04000826 RID: 2086
		private bool _isPlayerCountEnabled;

		// Token: 0x04000827 RID: 2087
		private string _nextMapId;

		// Token: 0x04000828 RID: 2088
		private string _nextFactionACultureId;

		// Token: 0x04000829 RID: 2089
		private string _nextFactionBCultureId;

		// Token: 0x0400082A RID: 2090
		private string _nextGameStateTimerLabel;

		// Token: 0x0400082B RID: 2091
		private string _nextGameStateTimerValue;

		// Token: 0x0400082C RID: 2092
		private string _playersLabel;

		// Token: 0x0400082D RID: 2093
		private string _mapVoteText;

		// Token: 0x0400082E RID: 2094
		private string _cultureVoteText;

		// Token: 0x0400082F RID: 2095
		private string _serverName;

		// Token: 0x04000830 RID: 2096
		private string _welcomeMessage;

		// Token: 0x04000831 RID: 2097
		private string _nextGameType;

		// Token: 0x04000832 RID: 2098
		private string _nextMapName;

		// Token: 0x04000833 RID: 2099
		private string _quitText;

		// Token: 0x04000834 RID: 2100
		private MBBindingList<MPIntermissionMapItemVM> _availableMaps;

		// Token: 0x04000835 RID: 2101
		private MBBindingList<MPIntermissionCultureItemVM> _availableCultures;
	}
}
