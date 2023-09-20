using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.MountAndBlade.Diamond.MultiplayerBadges;
using TaleWorlds.MountAndBlade.Diamond.Ranked;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.AfterBattle
{
	public class MPAfterBattlePopupVM : ViewModel
	{
		public MPAfterBattlePopupVM(Func<string> getContinueKeyText)
		{
			this._getContinueKeyText = getContinueKeyText;
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this._battleResultsTitleText = new TextObject("{=pguhTmXw}Battle Results", null).ToString();
			this._levelUpTitleText = new TextObject("{=0tUYng4e}Leveled Up!", null).ToString();
			this._rankProgressTitleText = new TextObject("{=XEGaQB2G}Rank Progression", null).ToString();
			this._promotedTitleText = new TextObject("{=bn0v5ST0}Promoted!", null).ToString();
			this._demotedTitleText = new TextObject("{=HUndnpNw}Demoted!", null).ToString();
			this._evaluationFinishedTitleText = new TextObject("{=2KZLf51A}Evaluation Matches Finished", null).ToString();
			this.LevelText = GameTexts.FindText("str_level", null).ToString();
			this.ExperienceText = new TextObject("{=SwSaXwQg}exp", null).ToString();
			this.PointsText = new TextObject("{=4dRTWSN3}Points", null).ToString();
		}

		public void OpenWith(int oldExperience, int newExperience, List<string> badgesEarned, int lootGained, RankBarInfo oldRankBarInfo, RankBarInfo newRankBarInfo)
		{
			Func<string> getContinueKeyText = this._getContinueKeyText;
			this.ClickToContinueText = ((getContinueKeyText != null) ? getContinueKeyText() : null);
			this._oldExperience = oldExperience;
			this._newExperience = newExperience;
			this._earnedBadgeIDs = badgesEarned;
			this._lootGained = lootGained;
			this._oldRankBarInfo = oldRankBarInfo;
			this._newRankBarInfo = newRankBarInfo;
			this._hasRatingChanged = oldRankBarInfo != null && newRankBarInfo != null && !oldRankBarInfo.IsEvaluating && !newRankBarInfo.IsEvaluating;
			this._hasRankChanged = this._hasRatingChanged && oldRankBarInfo.RankId != newRankBarInfo.RankId;
			this._hasFinishedEvaluation = this._oldRankBarInfo != null && this._newRankBarInfo != null && this._oldRankBarInfo.IsEvaluating && !this._newRankBarInfo.IsEvaluating;
			this.AdvanceState();
			this.IsEnabled = true;
		}

		private void AdvanceState()
		{
			this.HideInfo();
			switch (this._currentState)
			{
			case MPAfterBattlePopupVM.AfterBattleState.None:
				this._currentState = MPAfterBattlePopupVM.AfterBattleState.GeneralProgression;
				this.ShowGeneralProgression();
				return;
			case MPAfterBattlePopupVM.AfterBattleState.GeneralProgression:
				if (this._hasLeveledUp)
				{
					this._currentState = MPAfterBattlePopupVM.AfterBattleState.LevelUp;
					this.ShowLevelUp();
					return;
				}
				if (this._hasRatingChanged)
				{
					this._currentState = MPAfterBattlePopupVM.AfterBattleState.RatingChange;
					this.ShowRankProgression();
					return;
				}
				if (this._hasFinishedEvaluation)
				{
					this._currentState = MPAfterBattlePopupVM.AfterBattleState.RankChange;
					this.ShowRankChange();
					return;
				}
				this.Disable();
				return;
			case MPAfterBattlePopupVM.AfterBattleState.LevelUp:
				if (this._hasRatingChanged)
				{
					this._currentState = MPAfterBattlePopupVM.AfterBattleState.RatingChange;
					this.ShowRankProgression();
					return;
				}
				if (this._hasFinishedEvaluation)
				{
					this._currentState = MPAfterBattlePopupVM.AfterBattleState.RankChange;
					this.ShowRankChange();
					return;
				}
				this.Disable();
				return;
			case MPAfterBattlePopupVM.AfterBattleState.RatingChange:
				if (this._hasRankChanged)
				{
					this._currentState = MPAfterBattlePopupVM.AfterBattleState.RankChange;
					this.ShowRankChange();
					return;
				}
				this.Disable();
				return;
			case MPAfterBattlePopupVM.AfterBattleState.RankChange:
				this.Disable();
				return;
			default:
				return;
			}
		}

		private void ShowGeneralProgression()
		{
			this.TitleText = this._battleResultsTitleText;
			this.InitialRatio = 0;
			this.FinalRatio = 0;
			this.NumOfLevelUps = 0;
			PlayerDataExperience playerDataExperience = new PlayerDataExperience(this._oldExperience);
			PlayerDataExperience playerDataExperience2 = new PlayerDataExperience(this._newExperience);
			this.GainedExperience = this._newExperience - this._oldExperience;
			this.CurrentLevel = playerDataExperience.Level;
			this.NextLevel = this.CurrentLevel + 1;
			this.InitialRatio = (int)((float)playerDataExperience.ExperienceInCurrentLevel / (float)(playerDataExperience.ExperienceToNextLevel + playerDataExperience.ExperienceInCurrentLevel) * 100f);
			this.FinalRatio = (int)((float)playerDataExperience2.ExperienceInCurrentLevel / (float)(playerDataExperience2.ExperienceToNextLevel + playerDataExperience2.ExperienceInCurrentLevel) * 100f);
			this.NumOfLevelUps = playerDataExperience2.Level - playerDataExperience.Level;
			this._hasLeveledUp = this.NumOfLevelUps > 0;
			float num = (float)this.NumOfLevelUps + (float)this.FinalRatio / 100f;
			this.LevelsExperienceRequirment = (int)((float)this._newExperience / num);
			this.RewardsEarned = new MBBindingList<MPAfterBattleRewardItemVM>();
			foreach (string text in this._earnedBadgeIDs)
			{
				Badge byId = BadgeManager.GetById(text);
				if (byId != null)
				{
					this.RewardsEarned.Add(new MPAfterBattleBadgeRewardItemVM(byId));
				}
			}
			if (this._lootGained > 0)
			{
				int num2 = this._lootGained - this._earnedBadgeIDs.Count * Parameters.LootRewardPerBadgeEarned;
				int num3 = this._lootGained - num2;
				this.RewardsEarned.Add(new MPAfterBattleLootRewardItemVM(num2, num3));
			}
			this.IsShowingGeneralProgression = true;
		}

		private void ShowLevelUp()
		{
			this.TitleText = this._levelUpTitleText;
			int level = new PlayerDataExperience(this._newExperience).Level;
			GameTexts.SetVariable("STR1", GameTexts.FindText("str_level", null));
			GameTexts.SetVariable("STR2", level);
			this.ReachedLevelText = GameTexts.FindText("str_STR1_space_STR2", null).ToString();
			SoundEvent.PlaySound2D("event:/ui/multiplayer/levelup");
			this.IsShowingNewLevel = true;
		}

		private void ShowRankProgression()
		{
			this.TitleText = this._rankProgressTitleText;
			this.OldRankID = this._oldRankBarInfo.RankId;
			this.NewRankID = this._newRankBarInfo.RankId;
			this.OldRankName = MPLobbyVM.GetLocalizedRankName(this.OldRankID);
			this.NewRankName = MPLobbyVM.GetLocalizedRankName(this.NewRankID);
			this.HasLostRating = this._oldRankBarInfo.Rating > this._newRankBarInfo.Rating;
			if (this.HasLostRating)
			{
				this.ShownRating = this._newRankBarInfo.Rating;
				this.InitialRatio = (int)this._newRankBarInfo.ProgressPercentage;
				this.FinalRatio = (int)this._newRankBarInfo.ProgressPercentage;
				this._pointsLostTextObj.SetTextVariable("POINTS", this._oldRankBarInfo.Rating - this._newRankBarInfo.Rating);
				this.PointChangedText = this._pointsLostTextObj.ToString();
			}
			else
			{
				this.ShownRating = this._oldRankBarInfo.Rating;
				this.ShownRating = this._newRankBarInfo.Rating;
				this.InitialRatio = (int)this._oldRankBarInfo.ProgressPercentage;
				this.FinalRatio = (int)this._newRankBarInfo.ProgressPercentage;
				this.NumOfLevelUps = ((Ranks.RankIds.IndexOf(this.OldRankID) < Ranks.RankIds.IndexOf(this.NewRankID)) ? 1 : 0);
				this._pointsGainedTextObj.SetTextVariable("POINTS", this._newRankBarInfo.Rating - this._oldRankBarInfo.Rating);
				this.PointChangedText = this._pointsGainedTextObj.ToString();
			}
			this.IsShowingRankProgression = true;
		}

		private void ShowRankChange()
		{
			if (this.OldRankID != string.Empty && this.OldRankID != null)
			{
				this.TitleText = ((Ranks.RankIds.IndexOf(this.OldRankID) < Ranks.RankIds.IndexOf(this.NewRankID)) ? this._promotedTitleText : this._demotedTitleText);
				this.IsShowingNewRank = true;
				return;
			}
			if (this._hasFinishedEvaluation)
			{
				this.OldRankID = string.Empty;
				this.NewRankID = this._newRankBarInfo.RankId;
				this.OldRankName = MPLobbyVM.GetLocalizedRankName(this.OldRankID);
				this.NewRankName = MPLobbyVM.GetLocalizedRankName(this.NewRankID);
				this.TitleText = this._evaluationFinishedTitleText;
				this.IsShowingNewRank = true;
			}
		}

		private void HideInfo()
		{
			this.IsShowingGeneralProgression = false;
			this.IsShowingNewLevel = false;
			this.IsShowingRankProgression = false;
			this.IsShowingNewRank = false;
		}

		private void Disable()
		{
			this.HideInfo();
			this.ShownRating = 0;
			this._currentState = MPAfterBattlePopupVM.AfterBattleState.None;
			this.IsEnabled = false;
		}

		public void ExecuteClose()
		{
			if (this.IsEnabled)
			{
				this.AdvanceState();
			}
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
		public bool IsShowingGeneralProgression
		{
			get
			{
				return this._isShowingGeneralProgression;
			}
			set
			{
				if (value != this._isShowingGeneralProgression)
				{
					this._isShowingGeneralProgression = value;
					base.OnPropertyChangedWithValue(value, "IsShowingGeneralProgression");
				}
			}
		}

		[DataSourceProperty]
		public bool IsShowingNewLevel
		{
			get
			{
				return this._isShowingNewLevel;
			}
			set
			{
				if (value != this._isShowingNewLevel)
				{
					this._isShowingNewLevel = value;
					base.OnPropertyChangedWithValue(value, "IsShowingNewLevel");
				}
			}
		}

		[DataSourceProperty]
		public bool IsShowingRankProgression
		{
			get
			{
				return this._isShowingRankProgression;
			}
			set
			{
				if (value != this._isShowingRankProgression)
				{
					this._isShowingRankProgression = value;
					base.OnPropertyChangedWithValue(value, "IsShowingRankProgression");
				}
			}
		}

		[DataSourceProperty]
		public bool IsShowingNewRank
		{
			get
			{
				return this._isShowingNewRank;
			}
			set
			{
				if (value != this._isShowingNewRank)
				{
					this._isShowingNewRank = value;
					base.OnPropertyChangedWithValue(value, "IsShowingNewRank");
				}
			}
		}

		[DataSourceProperty]
		public bool HasLostRating
		{
			get
			{
				return this._hasLostRating;
			}
			set
			{
				if (value != this._hasLostRating)
				{
					this._hasLostRating = value;
					base.OnPropertyChangedWithValue(value, "HasLostRating");
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
		public string LevelText
		{
			get
			{
				return this._levelText;
			}
			set
			{
				if (value != this._levelText)
				{
					this._levelText = value;
					base.OnPropertyChangedWithValue<string>(value, "LevelText");
				}
			}
		}

		[DataSourceProperty]
		public string ExperienceText
		{
			get
			{
				return this._experienceText;
			}
			set
			{
				if (value != this._experienceText)
				{
					this._experienceText = value;
					base.OnPropertyChangedWithValue<string>(value, "ExperienceText");
				}
			}
		}

		[DataSourceProperty]
		public string ClickToContinueText
		{
			get
			{
				return this._clickToContinueText;
			}
			set
			{
				if (value != this._clickToContinueText)
				{
					this._clickToContinueText = value;
					base.OnPropertyChangedWithValue<string>(value, "ClickToContinueText");
				}
			}
		}

		[DataSourceProperty]
		public string ReachedLevelText
		{
			get
			{
				return this._reachedLevelText;
			}
			set
			{
				if (value != this._reachedLevelText)
				{
					this._reachedLevelText = value;
					base.OnPropertyChangedWithValue<string>(value, "ReachedLevelText");
				}
			}
		}

		[DataSourceProperty]
		public string PointsText
		{
			get
			{
				return this._pointsText;
			}
			set
			{
				if (value != this._pointsText)
				{
					this._pointsText = value;
					base.OnPropertyChangedWithValue<string>(value, "PointsText");
				}
			}
		}

		[DataSourceProperty]
		public string PointChangedText
		{
			get
			{
				return this._pointChangeText;
			}
			set
			{
				if (value != this._pointChangeText)
				{
					this._pointChangeText = value;
					base.OnPropertyChangedWithValue<string>(value, "PointChangedText");
				}
			}
		}

		[DataSourceProperty]
		public string OldRankID
		{
			get
			{
				return this._oldRankID;
			}
			set
			{
				if (value != this._oldRankID)
				{
					this._oldRankID = value;
					base.OnPropertyChangedWithValue<string>(value, "OldRankID");
				}
			}
		}

		[DataSourceProperty]
		public string NewRankID
		{
			get
			{
				return this._newRankID;
			}
			set
			{
				if (value != this._newRankID)
				{
					this._newRankID = value;
					base.OnPropertyChangedWithValue<string>(value, "NewRankID");
				}
			}
		}

		[DataSourceProperty]
		public string OldRankName
		{
			get
			{
				return this._oldRankName;
			}
			set
			{
				if (value != this._oldRankName)
				{
					this._oldRankName = value;
					base.OnPropertyChangedWithValue<string>(value, "OldRankName");
				}
			}
		}

		[DataSourceProperty]
		public string NewRankName
		{
			get
			{
				return this._newRankName;
			}
			set
			{
				if (value != this._newRankName)
				{
					this._newRankName = value;
					base.OnPropertyChangedWithValue<string>(value, "NewRankName");
				}
			}
		}

		[DataSourceProperty]
		public int FinalRatio
		{
			get
			{
				return this._finalRatio;
			}
			set
			{
				if (value != this._finalRatio)
				{
					this._finalRatio = value;
					base.OnPropertyChangedWithValue(value, "FinalRatio");
				}
			}
		}

		[DataSourceProperty]
		public int NumOfLevelUps
		{
			get
			{
				return this._numOfLevelUps;
			}
			set
			{
				if (value != this._numOfLevelUps)
				{
					this._numOfLevelUps = value;
					base.OnPropertyChangedWithValue(value, "NumOfLevelUps");
				}
			}
		}

		[DataSourceProperty]
		public int InitialRatio
		{
			get
			{
				return this._initialRatio;
			}
			set
			{
				if (value != this._initialRatio)
				{
					this._initialRatio = value;
					base.OnPropertyChangedWithValue(value, "InitialRatio");
				}
			}
		}

		[DataSourceProperty]
		public int GainedExperience
		{
			get
			{
				return this._gainedExperience;
			}
			set
			{
				if (value != this._gainedExperience)
				{
					this._gainedExperience = value;
					base.OnPropertyChangedWithValue(value, "GainedExperience");
				}
			}
		}

		[DataSourceProperty]
		public int LevelsExperienceRequirment
		{
			get
			{
				return this._levelsExperienceRequirment;
			}
			set
			{
				if (value != this._levelsExperienceRequirment)
				{
					this._levelsExperienceRequirment = value;
					base.OnPropertyChangedWithValue(value, "LevelsExperienceRequirment");
				}
			}
		}

		[DataSourceProperty]
		public int NextLevel
		{
			get
			{
				return this._nextLevel;
			}
			set
			{
				if (value != this._nextLevel)
				{
					this._nextLevel = value;
					base.OnPropertyChangedWithValue(value, "NextLevel");
				}
			}
		}

		[DataSourceProperty]
		public int CurrentLevel
		{
			get
			{
				return this._currentLevel;
			}
			set
			{
				if (value != this._currentLevel)
				{
					this._currentLevel = value;
					base.OnPropertyChangedWithValue(value, "CurrentLevel");
				}
			}
		}

		[DataSourceProperty]
		public int ShownRating
		{
			get
			{
				return this._shownRating;
			}
			set
			{
				if (value != this._shownRating)
				{
					this._shownRating = value;
					base.OnPropertyChangedWithValue(value, "ShownRating");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<MPAfterBattleRewardItemVM> RewardsEarned
		{
			get
			{
				return this._rewardsEarned;
			}
			set
			{
				if (value != this._rewardsEarned)
				{
					this._rewardsEarned = value;
					base.OnPropertyChangedWithValue<MBBindingList<MPAfterBattleRewardItemVM>>(value, "RewardsEarned");
				}
			}
		}

		private MPAfterBattlePopupVM.AfterBattleState _currentState;

		private bool _hasLeveledUp;

		private int _oldExperience;

		private int _newExperience;

		private List<string> _earnedBadgeIDs;

		private int _lootGained;

		private bool _hasRatingChanged;

		private bool _hasRankChanged;

		private bool _hasFinishedEvaluation;

		private RankBarInfo _oldRankBarInfo;

		private RankBarInfo _newRankBarInfo;

		private string _battleResultsTitleText;

		private string _levelUpTitleText;

		private string _rankProgressTitleText;

		private string _promotedTitleText;

		private string _demotedTitleText;

		private string _evaluationFinishedTitleText;

		private TextObject _pointsGainedTextObj = new TextObject("{=EFU3uo0y}You've gained {POINTS} points", null);

		private TextObject _pointsLostTextObj = new TextObject("{=oMYz0PvL}You've lost {POINTS} points", null);

		private readonly Func<string> _getContinueKeyText;

		private bool _isEnabled;

		private bool _isShowingGeneralProgression;

		private bool _isShowingNewLevel;

		private bool _isShowingRankProgression;

		private bool _isShowingNewRank;

		private bool _hasLostRating;

		private string _titleText;

		private string _levelText;

		private string _experienceText;

		private string _clickToContinueText;

		private string _reachedLevelText;

		private string _pointsText;

		private string _pointChangeText;

		private string _oldRankID;

		private string _newRankID;

		private string _oldRankName;

		private string _newRankName;

		private int _initialRatio;

		private int _finalRatio;

		private int _numOfLevelUps;

		private int _gainedExperience;

		private int _levelsExperienceRequirment;

		private int _currentLevel;

		private int _nextLevel;

		private int _shownRating;

		private MBBindingList<MPAfterBattleRewardItemVM> _rewardsEarned;

		private enum AfterBattleState
		{
			None,
			GeneralProgression,
			LevelUp,
			RatingChange,
			RankChange
		}
	}
}
