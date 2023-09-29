using System;
using System.Linq;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond.Ranked;
using TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Lobby.Friends;

namespace TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Lobby.Profile
{
	public class MPLobbyRankProgressInformationVM : ViewModel
	{
		public MPLobbyRankProgressInformationVM(Func<string> getExitText)
		{
			this._getExitText = getExitText;
			this.AllRanks = new MBBindingList<StringPairItemVM>();
			this.RefreshValues();
		}

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

		public override void OnFinalize()
		{
			base.OnFinalize();
			this.ExecuteClosePopup();
		}

		public void OpenWith(MPLobbyPlayerBaseVM player)
		{
			this.IsEnabled = true;
			Func<string> getExitText = this._getExitText;
			this.ClickToCloseText = ((getExitText != null) ? getExitText() : null);
			if (player.RankInfo == null)
			{
				Debug.FailedAssert("Can't request rank progression information of another player.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection\\Lobby\\Profile\\MPLobbyRankProgressInformationVM.cs", "OpenWith", 54);
				return;
			}
			this._basePlayer = player;
			this.Player = new MPLobbyPlayerBaseVM(player.ProvidedID, "", null, null);
			this.Player.UpdateRating(new Action(this.OnRatingReceived));
		}

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

		public void ExecuteClosePopup()
		{
			this.Player = null;
			this.IsEnabled = false;
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

		private MPLobbyPlayerBaseVM _basePlayer;

		private readonly Func<string> _getExitText;

		private TextObject _ratingRemainingTitleTextObject = new TextObject("{=7gQkFJqA}{RATING} points remaining to next rank", null);

		private TextObject _finalRankTextObject = new TextObject("{=6mZymVS8}You are at the final rank", null);

		private TextObject _evaluationTextObject = new TextObject("{=Ise5gWw3}{PLAYED_GAMES} / {TOTAL_GAMES} Evaluation matches played", null);

		private bool _isEnabled;

		private bool _isAtFinalRank;

		private bool _isEvaluating;

		private string _titleText;

		private string _clickToCloseText;

		private string _currentRankTitleText;

		private string _ratingRemainingTitleText;

		private string _currentRankID;

		private string _previousRankID;

		private string _nextRankID;

		private int _currentRating;

		private int _nextRankRating;

		private int _ratingRatio;

		private MPLobbyPlayerBaseVM _player;

		private MBBindingList<StringPairItemVM> _allRanks;
	}
}
