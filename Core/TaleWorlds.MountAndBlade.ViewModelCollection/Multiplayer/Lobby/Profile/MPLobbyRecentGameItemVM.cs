using System;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Profile
{
	// Token: 0x0200006F RID: 111
	public class MPLobbyRecentGameItemVM : ViewModel
	{
		// Token: 0x06000A43 RID: 2627 RVA: 0x000250E0 File Offset: 0x000232E0
		public MPLobbyRecentGameItemVM(Action<MPLobbyRecentGamePlayerItemVM> onActivatePlayerActions)
		{
			this._onActivatePlayerActions = onActivatePlayerActions;
			this.PlayersA = new MBBindingList<MPLobbyRecentGamePlayerItemVM>();
			this.PlayersB = new MBBindingList<MPLobbyRecentGamePlayerItemVM>();
			this.RefreshValues();
		}

		// Token: 0x06000A44 RID: 2628 RVA: 0x0002510C File Offset: 0x0002330C
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

		// Token: 0x06000A45 RID: 2629 RVA: 0x000251F8 File Offset: 0x000233F8
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

		// Token: 0x06000A46 RID: 2630 RVA: 0x000253B0 File Offset: 0x000235B0
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

		// Token: 0x1700032D RID: 813
		// (get) Token: 0x06000A47 RID: 2631 RVA: 0x00025438 File Offset: 0x00023638
		// (set) Token: 0x06000A48 RID: 2632 RVA: 0x00025440 File Offset: 0x00023640
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

		// Token: 0x1700032E RID: 814
		// (get) Token: 0x06000A49 RID: 2633 RVA: 0x00025463 File Offset: 0x00023663
		// (set) Token: 0x06000A4A RID: 2634 RVA: 0x0002546B File Offset: 0x0002366B
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

		// Token: 0x1700032F RID: 815
		// (get) Token: 0x06000A4B RID: 2635 RVA: 0x00025489 File Offset: 0x00023689
		// (set) Token: 0x06000A4C RID: 2636 RVA: 0x00025491 File Offset: 0x00023691
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

		// Token: 0x17000330 RID: 816
		// (get) Token: 0x06000A4D RID: 2637 RVA: 0x000254AF File Offset: 0x000236AF
		// (set) Token: 0x06000A4E RID: 2638 RVA: 0x000254B7 File Offset: 0x000236B7
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

		// Token: 0x17000331 RID: 817
		// (get) Token: 0x06000A4F RID: 2639 RVA: 0x000254DA File Offset: 0x000236DA
		// (set) Token: 0x06000A50 RID: 2640 RVA: 0x000254E2 File Offset: 0x000236E2
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

		// Token: 0x17000332 RID: 818
		// (get) Token: 0x06000A51 RID: 2641 RVA: 0x00025505 File Offset: 0x00023705
		// (set) Token: 0x06000A52 RID: 2642 RVA: 0x0002550D File Offset: 0x0002370D
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

		// Token: 0x17000333 RID: 819
		// (get) Token: 0x06000A53 RID: 2643 RVA: 0x00025530 File Offset: 0x00023730
		// (set) Token: 0x06000A54 RID: 2644 RVA: 0x00025538 File Offset: 0x00023738
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

		// Token: 0x17000334 RID: 820
		// (get) Token: 0x06000A55 RID: 2645 RVA: 0x0002555B File Offset: 0x0002375B
		// (set) Token: 0x06000A56 RID: 2646 RVA: 0x00025563 File Offset: 0x00023763
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

		// Token: 0x17000335 RID: 821
		// (get) Token: 0x06000A57 RID: 2647 RVA: 0x00025586 File Offset: 0x00023786
		// (set) Token: 0x06000A58 RID: 2648 RVA: 0x0002558E File Offset: 0x0002378E
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

		// Token: 0x17000336 RID: 822
		// (get) Token: 0x06000A59 RID: 2649 RVA: 0x000255B1 File Offset: 0x000237B1
		// (set) Token: 0x06000A5A RID: 2650 RVA: 0x000255B9 File Offset: 0x000237B9
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

		// Token: 0x17000337 RID: 823
		// (get) Token: 0x06000A5B RID: 2651 RVA: 0x000255DC File Offset: 0x000237DC
		// (set) Token: 0x06000A5C RID: 2652 RVA: 0x000255E4 File Offset: 0x000237E4
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

		// Token: 0x17000338 RID: 824
		// (get) Token: 0x06000A5D RID: 2653 RVA: 0x00025607 File Offset: 0x00023807
		// (set) Token: 0x06000A5E RID: 2654 RVA: 0x0002560F File Offset: 0x0002380F
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

		// Token: 0x17000339 RID: 825
		// (get) Token: 0x06000A5F RID: 2655 RVA: 0x00025632 File Offset: 0x00023832
		// (set) Token: 0x06000A60 RID: 2656 RVA: 0x0002563A File Offset: 0x0002383A
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

		// Token: 0x1700033A RID: 826
		// (get) Token: 0x06000A61 RID: 2657 RVA: 0x00025658 File Offset: 0x00023858
		// (set) Token: 0x06000A62 RID: 2658 RVA: 0x00025660 File Offset: 0x00023860
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

		// Token: 0x1700033B RID: 827
		// (get) Token: 0x06000A63 RID: 2659 RVA: 0x0002567E File Offset: 0x0002387E
		// (set) Token: 0x06000A64 RID: 2660 RVA: 0x00025686 File Offset: 0x00023886
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

		// Token: 0x1700033C RID: 828
		// (get) Token: 0x06000A65 RID: 2661 RVA: 0x000256A4 File Offset: 0x000238A4
		// (set) Token: 0x06000A66 RID: 2662 RVA: 0x000256AC File Offset: 0x000238AC
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

		// Token: 0x1700033D RID: 829
		// (get) Token: 0x06000A67 RID: 2663 RVA: 0x000256CA File Offset: 0x000238CA
		// (set) Token: 0x06000A68 RID: 2664 RVA: 0x000256D2 File Offset: 0x000238D2
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

		// Token: 0x06000A69 RID: 2665 RVA: 0x000256F0 File Offset: 0x000238F0
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
				Debug.FailedAssert("Unidentified culture id: " + cultureID, "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.ViewModelCollection\\Multiplayer\\Lobby\\Profile\\MPLobbyRecentGameItemVM.cs", "GetLocalizedCultureNameFromStringID", 383);
				return "";
			}
			return new TextObject("{=aseraifaction}Aserai", null).ToString();
		}

		// Token: 0x040004FC RID: 1276
		private MatchInfo _matchInfo;

		// Token: 0x040004FD RID: 1277
		private readonly Action<MPLobbyRecentGamePlayerItemVM> _onActivatePlayerActions;

		// Token: 0x040004FE RID: 1278
		public MBBindingList<MPLobbyRecentGamePlayerItemVM> _playersA;

		// Token: 0x040004FF RID: 1279
		public MBBindingList<MPLobbyRecentGamePlayerItemVM> _playersB;

		// Token: 0x04000500 RID: 1280
		private string _lastSeenPlayersText;

		// Token: 0x04000501 RID: 1281
		private string _factionNameA;

		// Token: 0x04000502 RID: 1282
		private string _factionNameB;

		// Token: 0x04000503 RID: 1283
		private string _cultureA;

		// Token: 0x04000504 RID: 1284
		private string _cultureB;

		// Token: 0x04000505 RID: 1285
		private string _scoreA;

		// Token: 0x04000506 RID: 1286
		private string _scoreB;

		// Token: 0x04000507 RID: 1287
		private string _gameMode;

		// Token: 0x04000508 RID: 1288
		private string _date;

		// Token: 0x04000509 RID: 1289
		private string _seperator;

		// Token: 0x0400050A RID: 1290
		private int _playerResultIndex;

		// Token: 0x0400050B RID: 1291
		private int _matchResultIndex;

		// Token: 0x0400050C RID: 1292
		private HintViewModel _abandonedHint;

		// Token: 0x0400050D RID: 1293
		private HintViewModel _wonHint;

		// Token: 0x0400050E RID: 1294
		private HintViewModel _lostHint;
	}
}
