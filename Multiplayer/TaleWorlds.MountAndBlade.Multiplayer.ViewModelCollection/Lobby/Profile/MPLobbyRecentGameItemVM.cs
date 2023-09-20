using System;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Lobby.Profile
{
	public class MPLobbyRecentGameItemVM : ViewModel
	{
		public MPLobbyRecentGameItemVM(Action<MPLobbyRecentGamePlayerItemVM> onActivatePlayerActions)
		{
			this._onActivatePlayerActions = onActivatePlayerActions;
			this.PlayersA = new MBBindingList<MPLobbyRecentGamePlayerItemVM>();
			this.PlayersB = new MBBindingList<MPLobbyRecentGamePlayerItemVM>();
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.LastSeenPlayersText = new TextObject("{=NJolh9ye}Recent Games", null).ToString();
			this.Seperator = new TextObject("{=4NaOKslb}-", null).ToString();
			this.PlayersA.ApplyActionOnAllItems(delegate(MPLobbyRecentGamePlayerItemVM x)
			{
				x.RefreshValues();
			});
			this.PlayersB.ApplyActionOnAllItems(delegate(MPLobbyRecentGamePlayerItemVM x)
			{
				x.RefreshValues();
			});
			this.AbandonedHint = new HintViewModel(new TextObject("{=eQPSEUml}Abandoned", null), null);
			this.WonHint = new HintViewModel(new TextObject("{=IS4SifJG}Won", null), null);
			this.LostHint = new HintViewModel(new TextObject("{=b2aqL7T2}Lost", null), null);
			if (this._matchInfo != null)
			{
				this.FillFrom(this._matchInfo);
			}
		}

		public void FillFrom(MatchInfo match)
		{
			this._matchInfo = match;
			this.PlayersA.Clear();
			this.PlayersB.Clear();
			this.GameMode = GameTexts.FindText("str_multiplayer_official_game_type_name", match.GameType).ToString();
			this.PlayerResultIndex = ((match.WinnerTeam == -1) ? 0 : 1);
			this.CultureA = match.Faction1;
			this.FactionNameA = MPLobbyRecentGameItemVM.GetLocalizedCultureNameFromStringID(match.Faction1);
			this.ScoreA = match.AttackerScore.ToString();
			this.CultureB = match.Faction2;
			this.FactionNameB = MPLobbyRecentGameItemVM.GetLocalizedCultureNameFromStringID(match.Faction2);
			this.ScoreB = match.DefenderScore.ToString();
			this.MatchResultIndex = ((match.DefenderScore == match.AttackerScore) ? 0 : ((match.DefenderScore > match.AttackerScore) ? 2 : 1));
			this.Date = LocalizedTextManager.GetDateFormattedByLanguage(BannerlordConfig.Language, match.MatchDate);
			foreach (PlayerInfo playerInfo in match.Players)
			{
				PlayerId playerId = PlayerId.FromString(playerInfo.PlayerId);
				if (!MultiplayerPlayerHelper.IsBlocked(playerId))
				{
					MPLobbyRecentGamePlayerItemVM mplobbyRecentGamePlayerItemVM = new MPLobbyRecentGamePlayerItemVM(playerId, match, this._onActivatePlayerActions);
					if (match.WinnerTeam != -1 && playerId == NetworkMain.GameClient.PlayerID)
					{
						this.PlayerResultIndex = ((playerInfo.TeamNo == match.WinnerTeam) ? 1 : 2);
					}
					if (playerInfo.TeamNo == 0)
					{
						this.PlayersA.Add(mplobbyRecentGamePlayerItemVM);
					}
					else
					{
						this.PlayersB.Add(mplobbyRecentGamePlayerItemVM);
					}
				}
			}
		}

		public void OnFriendListUpdated(bool forceUpdate = false)
		{
			foreach (MPLobbyRecentGamePlayerItemVM mplobbyRecentGamePlayerItemVM in this.PlayersA)
			{
				mplobbyRecentGamePlayerItemVM.UpdateNameAndAvatar(forceUpdate);
			}
			foreach (MPLobbyRecentGamePlayerItemVM mplobbyRecentGamePlayerItemVM2 in this.PlayersB)
			{
				mplobbyRecentGamePlayerItemVM2.UpdateNameAndAvatar(forceUpdate);
			}
		}

		[DataSourceProperty]
		public string LastSeenPlayersText
		{
			get
			{
				return this._lastSeenPlayersText;
			}
			set
			{
				if (value != this._lastSeenPlayersText)
				{
					this._lastSeenPlayersText = value;
					base.OnPropertyChangedWithValue<string>(value, "LastSeenPlayersText");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<MPLobbyRecentGamePlayerItemVM> PlayersA
		{
			get
			{
				return this._playersA;
			}
			set
			{
				if (value != this._playersA)
				{
					this._playersA = value;
					base.OnPropertyChangedWithValue<MBBindingList<MPLobbyRecentGamePlayerItemVM>>(value, "PlayersA");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<MPLobbyRecentGamePlayerItemVM> PlayersB
		{
			get
			{
				return this._playersB;
			}
			set
			{
				if (value != this._playersB)
				{
					this._playersB = value;
					base.OnPropertyChangedWithValue<MBBindingList<MPLobbyRecentGamePlayerItemVM>>(value, "PlayersB");
				}
			}
		}

		[DataSourceProperty]
		public string CultureA
		{
			get
			{
				return this._cultureA;
			}
			set
			{
				if (value != this._cultureA)
				{
					this._cultureA = value;
					base.OnPropertyChangedWithValue<string>(value, "CultureA");
				}
			}
		}

		[DataSourceProperty]
		public string CultureB
		{
			get
			{
				return this._cultureB;
			}
			set
			{
				if (value != this._cultureB)
				{
					this._cultureB = value;
					base.OnPropertyChangedWithValue<string>(value, "CultureB");
				}
			}
		}

		[DataSourceProperty]
		public string FactionNameA
		{
			get
			{
				return this._factionNameA;
			}
			set
			{
				if (value != this._factionNameA)
				{
					this._factionNameA = value;
					base.OnPropertyChangedWithValue<string>(value, "FactionNameA");
				}
			}
		}

		[DataSourceProperty]
		public string FactionNameB
		{
			get
			{
				return this._factionNameB;
			}
			set
			{
				if (value != this._factionNameB)
				{
					this._factionNameB = value;
					base.OnPropertyChangedWithValue<string>(value, "FactionNameB");
				}
			}
		}

		[DataSourceProperty]
		public string ScoreA
		{
			get
			{
				return this._scoreA;
			}
			set
			{
				if (value != this._scoreA)
				{
					this._scoreA = value;
					base.OnPropertyChangedWithValue<string>(value, "ScoreA");
				}
			}
		}

		[DataSourceProperty]
		public string ScoreB
		{
			get
			{
				return this._scoreB;
			}
			set
			{
				if (value != this._scoreB)
				{
					this._scoreB = value;
					base.OnPropertyChangedWithValue<string>(value, "ScoreB");
				}
			}
		}

		[DataSourceProperty]
		public string GameMode
		{
			get
			{
				return this._gameMode;
			}
			set
			{
				if (value != this._gameMode)
				{
					this._gameMode = value;
					base.OnPropertyChangedWithValue<string>(value, "GameMode");
				}
			}
		}

		[DataSourceProperty]
		public string Date
		{
			get
			{
				return this._date;
			}
			set
			{
				if (value != this._date)
				{
					this._date = value;
					base.OnPropertyChangedWithValue<string>(value, "Date");
				}
			}
		}

		[DataSourceProperty]
		public string Seperator
		{
			get
			{
				return this._seperator;
			}
			set
			{
				if (value != this._seperator)
				{
					this._seperator = value;
					base.OnPropertyChangedWithValue<string>(value, "Seperator");
				}
			}
		}

		[DataSourceProperty]
		public int MatchResultIndex
		{
			get
			{
				return this._matchResultIndex;
			}
			set
			{
				if (value != this._matchResultIndex)
				{
					this._matchResultIndex = value;
					base.OnPropertyChangedWithValue(value, "MatchResultIndex");
				}
			}
		}

		[DataSourceProperty]
		public int PlayerResultIndex
		{
			get
			{
				return this._playerResultIndex;
			}
			set
			{
				if (value != this._playerResultIndex)
				{
					this._playerResultIndex = value;
					base.OnPropertyChangedWithValue(value, "PlayerResultIndex");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel AbandonedHint
		{
			get
			{
				return this._abandonedHint;
			}
			set
			{
				if (value != this._abandonedHint)
				{
					this._abandonedHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "AbandonedHint");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel WonHint
		{
			get
			{
				return this._wonHint;
			}
			set
			{
				if (value != this._wonHint)
				{
					this._wonHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "WonHint");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel LostHint
		{
			get
			{
				return this._lostHint;
			}
			set
			{
				if (value != this._lostHint)
				{
					this._lostHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "LostHint");
				}
			}
		}

		private static string GetLocalizedCultureNameFromStringID(string cultureID)
		{
			if (cultureID == "sturgia")
			{
				return new TextObject("{=PjO7oY16}Sturgia", null).ToString();
			}
			if (cultureID == "vlandia")
			{
				return new TextObject("{=FjwRsf1C}Vlandia", null).ToString();
			}
			if (cultureID == "battania")
			{
				return new TextObject("{=0B27RrYJ}Battania", null).ToString();
			}
			if (cultureID == "empire")
			{
				return new TextObject("{=empirefaction}Empire", null).ToString();
			}
			if (cultureID == "khuzait")
			{
				return new TextObject("{=sZLd6VHi}Khuzait", null).ToString();
			}
			if (!(cultureID == "aserai"))
			{
				Debug.FailedAssert("Unidentified culture id: " + cultureID, "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection\\Lobby\\Profile\\MPLobbyRecentGameItemVM.cs", "GetLocalizedCultureNameFromStringID", 383);
				return "";
			}
			return new TextObject("{=aseraifaction}Aserai", null).ToString();
		}

		private MatchInfo _matchInfo;

		private readonly Action<MPLobbyRecentGamePlayerItemVM> _onActivatePlayerActions;

		public MBBindingList<MPLobbyRecentGamePlayerItemVM> _playersA;

		public MBBindingList<MPLobbyRecentGamePlayerItemVM> _playersB;

		private string _lastSeenPlayersText;

		private string _factionNameA;

		private string _factionNameB;

		private string _cultureA;

		private string _cultureB;

		private string _scoreA;

		private string _scoreB;

		private string _gameMode;

		private string _date;

		private string _seperator;

		private int _playerResultIndex;

		private int _matchResultIndex;

		private HintViewModel _abandonedHint;

		private HintViewModel _wonHint;

		private HintViewModel _lostHint;
	}
}
