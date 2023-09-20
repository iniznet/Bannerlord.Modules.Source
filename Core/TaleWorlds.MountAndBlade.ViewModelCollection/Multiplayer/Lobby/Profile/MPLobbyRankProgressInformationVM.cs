using System;
using System.Linq;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond.Ranked;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Friends;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.Profile
{
	// Token: 0x0200006E RID: 110
	public class MPLobbyRankProgressInformationVM : ViewModel
	{
		// Token: 0x06000A1E RID: 2590 RVA: 0x00024A70 File Offset: 0x00022C70
		public MPLobbyRankProgressInformationVM(Func<string> getContinueKeyText)
		{
			this._getContinueKeyText = getContinueKeyText;
			this.AllRanks = new MBBindingList<StringPairItemVM>();
			this.RefreshValues();
		}

		// Token: 0x06000A1F RID: 2591 RVA: 0x00024AD0 File Offset: 0x00022CD0
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.TitleText = new TextObject("{=XEGaQB2G}Rank Progression", null).ToString();
			this.AllRanks.Clear();
			string[] rankIds = Ranks.RankIds;
			for (int i = 0; i < rankIds.Length; i++)
			{
				string rank = rankIds[i];
				this.AllRanks.Add(new StringPairItemVM(rank, string.Empty, new BasicTooltipViewModel(() => MPLobbyVM.GetLocalizedRankName(rank))));
			}
		}

		// Token: 0x06000A20 RID: 2592 RVA: 0x00024B53 File Offset: 0x00022D53
		public override void OnFinalize()
		{
			base.OnFinalize();
			this.ExecuteClosePopup();
		}

		// Token: 0x06000A21 RID: 2593 RVA: 0x00024B64 File Offset: 0x00022D64
		public void OpenWith(MPLobbyPlayerBaseVM player)
		{
			this.IsEnabled = true;
			Func<string> getContinueKeyText = this._getContinueKeyText;
			this.ClickToCloseText = ((getContinueKeyText != null) ? getContinueKeyText() : null);
			if (player.RankInfo == null)
			{
				return;
			}
			this._basePlayer = player;
			this.Player = new MPLobbyPlayerBaseVM(player.ProvidedID, "", null, null);
			this.Player.UpdateRating(new Action(this.OnRatingReceived));
		}

		// Token: 0x06000A22 RID: 2594 RVA: 0x00024BD0 File Offset: 0x00022DD0
		private void OnRatingReceived()
		{
			MPLobbyPlayerBaseVM player = this.Player;
			if (player == null)
			{
				return;
			}
			bool flag = true;
			Action<string> action = new Action<string>(this.RefreshRankInfo);
			MPLobbyGameTypeVM mplobbyGameTypeVM = this._basePlayer.GameTypes.FirstOrDefault((MPLobbyGameTypeVM gt) => gt.IsSelected);
			player.RefreshSelectableGameTypes(flag, action, (mplobbyGameTypeVM != null) ? mplobbyGameTypeVM.GameTypeID : null);
		}

		// Token: 0x06000A23 RID: 2595 RVA: 0x00024C38 File Offset: 0x00022E38
		private void RefreshRankInfo(string gameType)
		{
			GameTypeRankInfo[] rankInfo = this.Player.RankInfo;
			GameTypeRankInfo gameTypeRankInfo = ((rankInfo != null) ? rankInfo.FirstOrDefault((GameTypeRankInfo r) => r.GameType == gameType) : null);
			if (gameTypeRankInfo == null || gameTypeRankInfo.RankBarInfo == null)
			{
				this.IsEnabled = false;
				return;
			}
			RankBarInfo rankBarInfo = gameTypeRankInfo.RankBarInfo;
			this.CurrentRankID = rankBarInfo.RankId;
			this.CurrentRankTitleText = MPLobbyVM.GetLocalizedRankName(this.CurrentRankID);
			this.CurrentRating = rankBarInfo.Rating;
			this.NextRankRating = this.CurrentRating + rankBarInfo.RatingToNextRank;
			this.AllRanks.ApplyActionOnAllItems(delegate(StringPairItemVM r)
			{
				r.Value = " ";
			});
			StringPairItemVM stringPairItemVM = this.AllRanks.FirstOrDefault((StringPairItemVM r) => r.Definition == this.CurrentRankID);
			if (stringPairItemVM != null)
			{
				stringPairItemVM.Value = new TextObject("{=sWnQva5O}Current Rank", null).ToString();
			}
			this.IsAtFinalRank = string.IsNullOrEmpty(rankBarInfo.NextRankId);
			this.IsEvaluating = rankBarInfo.IsEvaluating;
			if (rankBarInfo.IsEvaluating)
			{
				this.RatingRatio = MathF.Floor((float)rankBarInfo.EvaluationMatchesPlayed / (float)rankBarInfo.TotalEvaluationMatchesRequired * 100f);
				this.NextRankID = string.Empty;
				this.PreviousRankID = string.Empty;
				this.CurrentRating = rankBarInfo.EvaluationMatchesPlayed;
				this.NextRankRating = rankBarInfo.TotalEvaluationMatchesRequired;
				this._evaluationTextObject.SetTextVariable("PLAYED_GAMES", rankBarInfo.EvaluationMatchesPlayed);
				this._evaluationTextObject.SetTextVariable("TOTAL_GAMES", rankBarInfo.TotalEvaluationMatchesRequired);
				this.RatingRemainingTitleText = this._evaluationTextObject.ToString();
				return;
			}
			if (this.IsAtFinalRank)
			{
				this.RatingRatio = 100;
				this.NextRankID = string.Empty;
				this.PreviousRankID = string.Empty;
				this.RatingRemainingTitleText = this._finalRankTextObject.ToString();
				return;
			}
			this.RatingRatio = MathF.Floor(rankBarInfo.ProgressPercentage);
			this.NextRankID = rankBarInfo.NextRankId;
			this.PreviousRankID = rankBarInfo.PreviousRankId;
			this._ratingRemainingTitleTextObject.SetTextVariable("RATING", rankBarInfo.RatingToNextRank);
			this.RatingRemainingTitleText = this._ratingRemainingTitleTextObject.ToString();
		}

		// Token: 0x06000A24 RID: 2596 RVA: 0x00024E73 File Offset: 0x00023073
		public void ExecuteClosePopup()
		{
			this.Player = null;
			this.IsEnabled = false;
		}

		// Token: 0x1700031E RID: 798
		// (get) Token: 0x06000A25 RID: 2597 RVA: 0x00024E83 File Offset: 0x00023083
		// (set) Token: 0x06000A26 RID: 2598 RVA: 0x00024E8B File Offset: 0x0002308B
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

		// Token: 0x1700031F RID: 799
		// (get) Token: 0x06000A27 RID: 2599 RVA: 0x00024EA9 File Offset: 0x000230A9
		// (set) Token: 0x06000A28 RID: 2600 RVA: 0x00024EB1 File Offset: 0x000230B1
		[DataSourceProperty]
		public bool IsAtFinalRank
		{
			get
			{
				return this._isAtFinalRank;
			}
			set
			{
				if (value != this._isAtFinalRank)
				{
					this._isAtFinalRank = value;
					base.OnPropertyChangedWithValue(value, "IsAtFinalRank");
				}
			}
		}

		// Token: 0x17000320 RID: 800
		// (get) Token: 0x06000A29 RID: 2601 RVA: 0x00024ECF File Offset: 0x000230CF
		// (set) Token: 0x06000A2A RID: 2602 RVA: 0x00024ED7 File Offset: 0x000230D7
		[DataSourceProperty]
		public bool IsEvaluating
		{
			get
			{
				return this._isEvaluating;
			}
			set
			{
				if (value != this._isEvaluating)
				{
					this._isEvaluating = value;
					base.OnPropertyChangedWithValue(value, "IsEvaluating");
				}
			}
		}

		// Token: 0x17000321 RID: 801
		// (get) Token: 0x06000A2B RID: 2603 RVA: 0x00024EF5 File Offset: 0x000230F5
		// (set) Token: 0x06000A2C RID: 2604 RVA: 0x00024EFD File Offset: 0x000230FD
		[DataSourceProperty]
		public string TitleText
		{
			get
			{
				return this._titleText;
			}
			set
			{
				if (value != this._titleText)
				{
					this._titleText = value;
					base.OnPropertyChangedWithValue<string>(value, "TitleText");
				}
			}
		}

		// Token: 0x17000322 RID: 802
		// (get) Token: 0x06000A2D RID: 2605 RVA: 0x00024F20 File Offset: 0x00023120
		// (set) Token: 0x06000A2E RID: 2606 RVA: 0x00024F28 File Offset: 0x00023128
		[DataSourceProperty]
		public string ClickToCloseText
		{
			get
			{
				return this._clickToCloseText;
			}
			set
			{
				if (value != this._clickToCloseText)
				{
					this._clickToCloseText = value;
					base.OnPropertyChangedWithValue<string>(value, "ClickToCloseText");
				}
			}
		}

		// Token: 0x17000323 RID: 803
		// (get) Token: 0x06000A2F RID: 2607 RVA: 0x00024F4B File Offset: 0x0002314B
		// (set) Token: 0x06000A30 RID: 2608 RVA: 0x00024F53 File Offset: 0x00023153
		[DataSourceProperty]
		public string CurrentRankTitleText
		{
			get
			{
				return this._currentRankTitleText;
			}
			set
			{
				if (value != this._currentRankTitleText)
				{
					this._currentRankTitleText = value;
					base.OnPropertyChangedWithValue<string>(value, "CurrentRankTitleText");
				}
			}
		}

		// Token: 0x17000324 RID: 804
		// (get) Token: 0x06000A31 RID: 2609 RVA: 0x00024F76 File Offset: 0x00023176
		// (set) Token: 0x06000A32 RID: 2610 RVA: 0x00024F7E File Offset: 0x0002317E
		[DataSourceProperty]
		public string RatingRemainingTitleText
		{
			get
			{
				return this._ratingRemainingTitleText;
			}
			set
			{
				if (value != this._ratingRemainingTitleText)
				{
					this._ratingRemainingTitleText = value;
					base.OnPropertyChangedWithValue<string>(value, "RatingRemainingTitleText");
				}
			}
		}

		// Token: 0x17000325 RID: 805
		// (get) Token: 0x06000A33 RID: 2611 RVA: 0x00024FA1 File Offset: 0x000231A1
		// (set) Token: 0x06000A34 RID: 2612 RVA: 0x00024FA9 File Offset: 0x000231A9
		[DataSourceProperty]
		public string CurrentRankID
		{
			get
			{
				return this._currentRankID;
			}
			set
			{
				if (value != this._currentRankID)
				{
					this._currentRankID = value;
					base.OnPropertyChangedWithValue<string>(value, "CurrentRankID");
				}
			}
		}

		// Token: 0x17000326 RID: 806
		// (get) Token: 0x06000A35 RID: 2613 RVA: 0x00024FCC File Offset: 0x000231CC
		// (set) Token: 0x06000A36 RID: 2614 RVA: 0x00024FD4 File Offset: 0x000231D4
		[DataSourceProperty]
		public string PreviousRankID
		{
			get
			{
				return this._previousRankID;
			}
			set
			{
				if (value != this._previousRankID)
				{
					this._previousRankID = value;
					base.OnPropertyChangedWithValue<string>(value, "PreviousRankID");
				}
			}
		}

		// Token: 0x17000327 RID: 807
		// (get) Token: 0x06000A37 RID: 2615 RVA: 0x00024FF7 File Offset: 0x000231F7
		// (set) Token: 0x06000A38 RID: 2616 RVA: 0x00024FFF File Offset: 0x000231FF
		[DataSourceProperty]
		public string NextRankID
		{
			get
			{
				return this._nextRankID;
			}
			set
			{
				if (value != this._nextRankID)
				{
					this._nextRankID = value;
					base.OnPropertyChangedWithValue<string>(value, "NextRankID");
				}
			}
		}

		// Token: 0x17000328 RID: 808
		// (get) Token: 0x06000A39 RID: 2617 RVA: 0x00025022 File Offset: 0x00023222
		// (set) Token: 0x06000A3A RID: 2618 RVA: 0x0002502A File Offset: 0x0002322A
		[DataSourceProperty]
		public int CurrentRating
		{
			get
			{
				return this._currentRating;
			}
			set
			{
				if (value != this._currentRating)
				{
					this._currentRating = value;
					base.OnPropertyChangedWithValue(value, "CurrentRating");
				}
			}
		}

		// Token: 0x17000329 RID: 809
		// (get) Token: 0x06000A3B RID: 2619 RVA: 0x00025048 File Offset: 0x00023248
		// (set) Token: 0x06000A3C RID: 2620 RVA: 0x00025050 File Offset: 0x00023250
		[DataSourceProperty]
		public int NextRankRating
		{
			get
			{
				return this._nextRankRating;
			}
			set
			{
				if (value != this._nextRankRating)
				{
					this._nextRankRating = value;
					base.OnPropertyChangedWithValue(value, "NextRankRating");
				}
			}
		}

		// Token: 0x1700032A RID: 810
		// (get) Token: 0x06000A3D RID: 2621 RVA: 0x0002506E File Offset: 0x0002326E
		// (set) Token: 0x06000A3E RID: 2622 RVA: 0x00025076 File Offset: 0x00023276
		[DataSourceProperty]
		public int RatingRatio
		{
			get
			{
				return this._ratingRatio;
			}
			set
			{
				if (value != this._ratingRatio)
				{
					this._ratingRatio = value;
					base.OnPropertyChangedWithValue(value, "RatingRatio");
				}
			}
		}

		// Token: 0x1700032B RID: 811
		// (get) Token: 0x06000A3F RID: 2623 RVA: 0x00025094 File Offset: 0x00023294
		// (set) Token: 0x06000A40 RID: 2624 RVA: 0x0002509C File Offset: 0x0002329C
		[DataSourceProperty]
		public MPLobbyPlayerBaseVM Player
		{
			get
			{
				return this._player;
			}
			set
			{
				if (value != this._player)
				{
					this._player = value;
					base.OnPropertyChangedWithValue<MPLobbyPlayerBaseVM>(value, "Player");
				}
			}
		}

		// Token: 0x1700032C RID: 812
		// (get) Token: 0x06000A41 RID: 2625 RVA: 0x000250BA File Offset: 0x000232BA
		// (set) Token: 0x06000A42 RID: 2626 RVA: 0x000250C2 File Offset: 0x000232C2
		[DataSourceProperty]
		public MBBindingList<StringPairItemVM> AllRanks
		{
			get
			{
				return this._allRanks;
			}
			set
			{
				if (value != this._allRanks)
				{
					this._allRanks = value;
					base.OnPropertyChangedWithValue<MBBindingList<StringPairItemVM>>(value, "AllRanks");
				}
			}
		}

		// Token: 0x040004E8 RID: 1256
		private MPLobbyPlayerBaseVM _basePlayer;

		// Token: 0x040004E9 RID: 1257
		private readonly Func<string> _getContinueKeyText;

		// Token: 0x040004EA RID: 1258
		private TextObject _ratingRemainingTitleTextObject = new TextObject("{=7gQkFJqA}{RATING} points remaining to next rank", null);

		// Token: 0x040004EB RID: 1259
		private TextObject _finalRankTextObject = new TextObject("{=6mZymVS8}You are at the final rank", null);

		// Token: 0x040004EC RID: 1260
		private TextObject _evaluationTextObject = new TextObject("{=Ise5gWw3}{PLAYED_GAMES} / {TOTAL_GAMES} Evaluation matches played", null);

		// Token: 0x040004ED RID: 1261
		private bool _isEnabled;

		// Token: 0x040004EE RID: 1262
		private bool _isAtFinalRank;

		// Token: 0x040004EF RID: 1263
		private bool _isEvaluating;

		// Token: 0x040004F0 RID: 1264
		private string _titleText;

		// Token: 0x040004F1 RID: 1265
		private string _clickToCloseText;

		// Token: 0x040004F2 RID: 1266
		private string _currentRankTitleText;

		// Token: 0x040004F3 RID: 1267
		private string _ratingRemainingTitleText;

		// Token: 0x040004F4 RID: 1268
		private string _currentRankID;

		// Token: 0x040004F5 RID: 1269
		private string _previousRankID;

		// Token: 0x040004F6 RID: 1270
		private string _nextRankID;

		// Token: 0x040004F7 RID: 1271
		private int _currentRating;

		// Token: 0x040004F8 RID: 1272
		private int _nextRankRating;

		// Token: 0x040004F9 RID: 1273
		private int _ratingRatio;

		// Token: 0x040004FA RID: 1274
		private MPLobbyPlayerBaseVM _player;

		// Token: 0x040004FB RID: 1275
		private MBBindingList<StringPairItemVM> _allRanks;
	}
}
