using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Intermission
{
	public class MPIntermissionVM : ViewModel
	{
		public MPIntermissionVM()
		{
			this.AvailableMaps = new MBBindingList<MPIntermissionMapItemVM>();
			this.AvailableCultures = new MBBindingList<MPIntermissionCultureItemVM>();
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			this.QuitText = new TextObject("{=3sRdGQou}Leave", null).ToString();
			this.PlayersLabel = new TextObject("{=RfXJdNye}Players", null).ToString();
			this.MapVoteText = new TextObject("{=DraJ6bxq}Vote for the Next Map", null).ToString();
			this.CultureVoteText = new TextObject("{=oF27vprQ}Vote for the Next Culture", null).ToString();
		}

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

		public override void OnFinalize()
		{
			if (this._baseNetworkComponent != null)
			{
				BaseNetworkComponent baseNetworkComponent = this._baseNetworkComponent;
				baseNetworkComponent.OnIntermissionStateUpdated = (Action)Delegate.Remove(baseNetworkComponent.OnIntermissionStateUpdated, new Action(this.OnIntermissionStateUpdated));
			}
			MultiplayerIntermissionVotingManager.Instance.ClearItems();
		}

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

		public void ExecuteQuitServer()
		{
			LobbyClient gameClient = NetworkMain.GameClient;
			if (gameClient.CurrentState == LobbyClient.State.InCustomGame)
			{
				gameClient.QuitFromCustomGame();
			}
			MultiplayerIntermissionVotingManager.Instance.ClearItems();
		}

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

		private bool _hasBaseNetworkComponentSet;

		private BaseNetworkComponent _baseNetworkComponent;

		private MultiplayerIntermissionState _currentIntermissionState;

		private readonly TextObject _voteLabelText = new TextObject("{=KOVHgkVq}Voting Ends In:", null);

		private readonly TextObject _nextGameLabelText = new TextObject("{=lX9Qx7Wo}Next Game Starts In:", null);

		private readonly TextObject _serverIdleLabelText = new TextObject("{=Rhcberxf}Awaiting Server", null);

		private readonly TextObject _matchFinishedText = new TextObject("{=RbazQjFt}Match is Finished", null);

		private readonly TextObject _returningToLobbyText = new TextObject("{=1UaxKbn6}Returning to the Lobby...", null);

		private MPIntermissionMapItemVM _votedMapItem;

		private MPIntermissionCultureItemVM _votedCultureItem;

		private string _connectedPlayersCountValueText;

		private string _maxNumPlayersValueText;

		private bool _isFactionAValid;

		private bool _isFactionBValid;

		private bool _isMissionTimerEnabled;

		private bool _isEndGameTimerEnabled;

		private bool _isNextMapInfoEnabled;

		private bool _isMapVoteEnabled;

		private bool _isCultureVoteEnabled;

		private bool _isPlayerCountEnabled;

		private string _nextMapId;

		private string _nextFactionACultureId;

		private string _nextFactionBCultureId;

		private string _nextGameStateTimerLabel;

		private string _nextGameStateTimerValue;

		private string _playersLabel;

		private string _mapVoteText;

		private string _cultureVoteText;

		private string _serverName;

		private string _welcomeMessage;

		private string _nextGameType;

		private string _nextMapName;

		private string _quitText;

		private MBBindingList<MPIntermissionMapItemVM> _availableMaps;

		private MBBindingList<MPIntermissionCultureItemVM> _availableCultures;
	}
}
