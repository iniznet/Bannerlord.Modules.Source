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
	// Token: 0x020000AC RID: 172
	public class MPAfterBattlePopupVM : ViewModel
	{
		// Token: 0x06001055 RID: 4181 RVA: 0x00036441 File Offset: 0x00034641
		public MPAfterBattlePopupVM(Func<string> getContinueKeyText)
		{
			this._getContinueKeyText = getContinueKeyText;
			this.RefreshValues();
		}

		// Token: 0x06001056 RID: 4182 RVA: 0x00036478 File Offset: 0x00034678
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

		// Token: 0x06001057 RID: 4183 RVA: 0x00036554 File Offset: 0x00034754
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

		// Token: 0x06001058 RID: 4184 RVA: 0x00036630 File Offset: 0x00034830
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

		// Token: 0x06001059 RID: 4185 RVA: 0x00036714 File Offset: 0x00034914
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

		// Token: 0x0600105A RID: 4186 RVA: 0x000368D0 File Offset: 0x00034AD0
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

		// Token: 0x0600105B RID: 4187 RVA: 0x00036948 File Offset: 0x00034B48
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

		// Token: 0x0600105C RID: 4188 RVA: 0x00036AF4 File Offset: 0x00034CF4
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

		// Token: 0x0600105D RID: 4189 RVA: 0x00036BB6 File Offset: 0x00034DB6
		private void HideInfo()
		{
			this.IsShowingGeneralProgression = false;
			this.IsShowingNewLevel = false;
			this.IsShowingRankProgression = false;
			this.IsShowingNewRank = false;
		}

		// Token: 0x0600105E RID: 4190 RVA: 0x00036BD4 File Offset: 0x00034DD4
		private void Disable()
		{
			this.HideInfo();
			this.ShownRating = 0;
			this._currentState = MPAfterBattlePopupVM.AfterBattleState.None;
			this.IsEnabled = false;
		}

		// Token: 0x0600105F RID: 4191 RVA: 0x00036BF1 File Offset: 0x00034DF1
		public void ExecuteClose()
		{
			if (this.IsEnabled)
			{
				this.AdvanceState();
			}
		}

		// Token: 0x1700053A RID: 1338
		// (get) Token: 0x06001060 RID: 4192 RVA: 0x00036C01 File Offset: 0x00034E01
		// (set) Token: 0x06001061 RID: 4193 RVA: 0x00036C09 File Offset: 0x00034E09
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

		// Token: 0x1700053B RID: 1339
		// (get) Token: 0x06001062 RID: 4194 RVA: 0x00036C27 File Offset: 0x00034E27
		// (set) Token: 0x06001063 RID: 4195 RVA: 0x00036C2F File Offset: 0x00034E2F
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

		// Token: 0x1700053C RID: 1340
		// (get) Token: 0x06001064 RID: 4196 RVA: 0x00036C4D File Offset: 0x00034E4D
		// (set) Token: 0x06001065 RID: 4197 RVA: 0x00036C55 File Offset: 0x00034E55
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

		// Token: 0x1700053D RID: 1341
		// (get) Token: 0x06001066 RID: 4198 RVA: 0x00036C73 File Offset: 0x00034E73
		// (set) Token: 0x06001067 RID: 4199 RVA: 0x00036C7B File Offset: 0x00034E7B
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

		// Token: 0x1700053E RID: 1342
		// (get) Token: 0x06001068 RID: 4200 RVA: 0x00036C99 File Offset: 0x00034E99
		// (set) Token: 0x06001069 RID: 4201 RVA: 0x00036CA1 File Offset: 0x00034EA1
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

		// Token: 0x1700053F RID: 1343
		// (get) Token: 0x0600106A RID: 4202 RVA: 0x00036CBF File Offset: 0x00034EBF
		// (set) Token: 0x0600106B RID: 4203 RVA: 0x00036CC7 File Offset: 0x00034EC7
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

		// Token: 0x17000540 RID: 1344
		// (get) Token: 0x0600106C RID: 4204 RVA: 0x00036CE5 File Offset: 0x00034EE5
		// (set) Token: 0x0600106D RID: 4205 RVA: 0x00036CED File Offset: 0x00034EED
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

		// Token: 0x17000541 RID: 1345
		// (get) Token: 0x0600106E RID: 4206 RVA: 0x00036D10 File Offset: 0x00034F10
		// (set) Token: 0x0600106F RID: 4207 RVA: 0x00036D18 File Offset: 0x00034F18
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

		// Token: 0x17000542 RID: 1346
		// (get) Token: 0x06001070 RID: 4208 RVA: 0x00036D3B File Offset: 0x00034F3B
		// (set) Token: 0x06001071 RID: 4209 RVA: 0x00036D43 File Offset: 0x00034F43
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

		// Token: 0x17000543 RID: 1347
		// (get) Token: 0x06001072 RID: 4210 RVA: 0x00036D66 File Offset: 0x00034F66
		// (set) Token: 0x06001073 RID: 4211 RVA: 0x00036D6E File Offset: 0x00034F6E
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

		// Token: 0x17000544 RID: 1348
		// (get) Token: 0x06001074 RID: 4212 RVA: 0x00036D91 File Offset: 0x00034F91
		// (set) Token: 0x06001075 RID: 4213 RVA: 0x00036D99 File Offset: 0x00034F99
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

		// Token: 0x17000545 RID: 1349
		// (get) Token: 0x06001076 RID: 4214 RVA: 0x00036DBC File Offset: 0x00034FBC
		// (set) Token: 0x06001077 RID: 4215 RVA: 0x00036DC4 File Offset: 0x00034FC4
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

		// Token: 0x17000546 RID: 1350
		// (get) Token: 0x06001078 RID: 4216 RVA: 0x00036DE7 File Offset: 0x00034FE7
		// (set) Token: 0x06001079 RID: 4217 RVA: 0x00036DEF File Offset: 0x00034FEF
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

		// Token: 0x17000547 RID: 1351
		// (get) Token: 0x0600107A RID: 4218 RVA: 0x00036E12 File Offset: 0x00035012
		// (set) Token: 0x0600107B RID: 4219 RVA: 0x00036E1A File Offset: 0x0003501A
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

		// Token: 0x17000548 RID: 1352
		// (get) Token: 0x0600107C RID: 4220 RVA: 0x00036E3D File Offset: 0x0003503D
		// (set) Token: 0x0600107D RID: 4221 RVA: 0x00036E45 File Offset: 0x00035045
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

		// Token: 0x17000549 RID: 1353
		// (get) Token: 0x0600107E RID: 4222 RVA: 0x00036E68 File Offset: 0x00035068
		// (set) Token: 0x0600107F RID: 4223 RVA: 0x00036E70 File Offset: 0x00035070
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

		// Token: 0x1700054A RID: 1354
		// (get) Token: 0x06001080 RID: 4224 RVA: 0x00036E93 File Offset: 0x00035093
		// (set) Token: 0x06001081 RID: 4225 RVA: 0x00036E9B File Offset: 0x0003509B
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

		// Token: 0x1700054B RID: 1355
		// (get) Token: 0x06001082 RID: 4226 RVA: 0x00036EBE File Offset: 0x000350BE
		// (set) Token: 0x06001083 RID: 4227 RVA: 0x00036EC6 File Offset: 0x000350C6
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

		// Token: 0x1700054C RID: 1356
		// (get) Token: 0x06001084 RID: 4228 RVA: 0x00036EE4 File Offset: 0x000350E4
		// (set) Token: 0x06001085 RID: 4229 RVA: 0x00036EEC File Offset: 0x000350EC
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

		// Token: 0x1700054D RID: 1357
		// (get) Token: 0x06001086 RID: 4230 RVA: 0x00036F0A File Offset: 0x0003510A
		// (set) Token: 0x06001087 RID: 4231 RVA: 0x00036F12 File Offset: 0x00035112
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

		// Token: 0x1700054E RID: 1358
		// (get) Token: 0x06001088 RID: 4232 RVA: 0x00036F30 File Offset: 0x00035130
		// (set) Token: 0x06001089 RID: 4233 RVA: 0x00036F38 File Offset: 0x00035138
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

		// Token: 0x1700054F RID: 1359
		// (get) Token: 0x0600108A RID: 4234 RVA: 0x00036F56 File Offset: 0x00035156
		// (set) Token: 0x0600108B RID: 4235 RVA: 0x00036F5E File Offset: 0x0003515E
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

		// Token: 0x17000550 RID: 1360
		// (get) Token: 0x0600108C RID: 4236 RVA: 0x00036F7C File Offset: 0x0003517C
		// (set) Token: 0x0600108D RID: 4237 RVA: 0x00036F84 File Offset: 0x00035184
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

		// Token: 0x17000551 RID: 1361
		// (get) Token: 0x0600108E RID: 4238 RVA: 0x00036FA2 File Offset: 0x000351A2
		// (set) Token: 0x0600108F RID: 4239 RVA: 0x00036FAA File Offset: 0x000351AA
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

		// Token: 0x17000552 RID: 1362
		// (get) Token: 0x06001090 RID: 4240 RVA: 0x00036FC8 File Offset: 0x000351C8
		// (set) Token: 0x06001091 RID: 4241 RVA: 0x00036FD0 File Offset: 0x000351D0
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

		// Token: 0x17000553 RID: 1363
		// (get) Token: 0x06001092 RID: 4242 RVA: 0x00036FEE File Offset: 0x000351EE
		// (set) Token: 0x06001093 RID: 4243 RVA: 0x00036FF6 File Offset: 0x000351F6
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

		// Token: 0x040007B7 RID: 1975
		private MPAfterBattlePopupVM.AfterBattleState _currentState;

		// Token: 0x040007B8 RID: 1976
		private bool _hasLeveledUp;

		// Token: 0x040007B9 RID: 1977
		private int _oldExperience;

		// Token: 0x040007BA RID: 1978
		private int _newExperience;

		// Token: 0x040007BB RID: 1979
		private List<string> _earnedBadgeIDs;

		// Token: 0x040007BC RID: 1980
		private int _lootGained;

		// Token: 0x040007BD RID: 1981
		private bool _hasRatingChanged;

		// Token: 0x040007BE RID: 1982
		private bool _hasRankChanged;

		// Token: 0x040007BF RID: 1983
		private bool _hasFinishedEvaluation;

		// Token: 0x040007C0 RID: 1984
		private RankBarInfo _oldRankBarInfo;

		// Token: 0x040007C1 RID: 1985
		private RankBarInfo _newRankBarInfo;

		// Token: 0x040007C2 RID: 1986
		private string _battleResultsTitleText;

		// Token: 0x040007C3 RID: 1987
		private string _levelUpTitleText;

		// Token: 0x040007C4 RID: 1988
		private string _rankProgressTitleText;

		// Token: 0x040007C5 RID: 1989
		private string _promotedTitleText;

		// Token: 0x040007C6 RID: 1990
		private string _demotedTitleText;

		// Token: 0x040007C7 RID: 1991
		private string _evaluationFinishedTitleText;

		// Token: 0x040007C8 RID: 1992
		private TextObject _pointsGainedTextObj = new TextObject("{=EFU3uo0y}You've gained {POINTS} points", null);

		// Token: 0x040007C9 RID: 1993
		private TextObject _pointsLostTextObj = new TextObject("{=oMYz0PvL}You've lost {POINTS} points", null);

		// Token: 0x040007CA RID: 1994
		private readonly Func<string> _getContinueKeyText;

		// Token: 0x040007CB RID: 1995
		private bool _isEnabled;

		// Token: 0x040007CC RID: 1996
		private bool _isShowingGeneralProgression;

		// Token: 0x040007CD RID: 1997
		private bool _isShowingNewLevel;

		// Token: 0x040007CE RID: 1998
		private bool _isShowingRankProgression;

		// Token: 0x040007CF RID: 1999
		private bool _isShowingNewRank;

		// Token: 0x040007D0 RID: 2000
		private bool _hasLostRating;

		// Token: 0x040007D1 RID: 2001
		private string _titleText;

		// Token: 0x040007D2 RID: 2002
		private string _levelText;

		// Token: 0x040007D3 RID: 2003
		private string _experienceText;

		// Token: 0x040007D4 RID: 2004
		private string _clickToContinueText;

		// Token: 0x040007D5 RID: 2005
		private string _reachedLevelText;

		// Token: 0x040007D6 RID: 2006
		private string _pointsText;

		// Token: 0x040007D7 RID: 2007
		private string _pointChangeText;

		// Token: 0x040007D8 RID: 2008
		private string _oldRankID;

		// Token: 0x040007D9 RID: 2009
		private string _newRankID;

		// Token: 0x040007DA RID: 2010
		private string _oldRankName;

		// Token: 0x040007DB RID: 2011
		private string _newRankName;

		// Token: 0x040007DC RID: 2012
		private int _initialRatio;

		// Token: 0x040007DD RID: 2013
		private int _finalRatio;

		// Token: 0x040007DE RID: 2014
		private int _numOfLevelUps;

		// Token: 0x040007DF RID: 2015
		private int _gainedExperience;

		// Token: 0x040007E0 RID: 2016
		private int _levelsExperienceRequirment;

		// Token: 0x040007E1 RID: 2017
		private int _currentLevel;

		// Token: 0x040007E2 RID: 2018
		private int _nextLevel;

		// Token: 0x040007E3 RID: 2019
		private int _shownRating;

		// Token: 0x040007E4 RID: 2020
		private MBBindingList<MPAfterBattleRewardItemVM> _rewardsEarned;

		// Token: 0x02000210 RID: 528
		private enum AfterBattleState
		{
			// Token: 0x04000E69 RID: 3689
			None,
			// Token: 0x04000E6A RID: 3690
			GeneralProgression,
			// Token: 0x04000E6B RID: 3691
			LevelUp,
			// Token: 0x04000E6C RID: 3692
			RatingChange,
			// Token: 0x04000E6D RID: 3693
			RankChange
		}
	}
}
