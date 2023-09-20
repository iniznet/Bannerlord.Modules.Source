using System;
using System.Collections.Generic;
using System.Linq;
using SandBox.Tournaments.MissionLogics;
using SandBox.ViewModelCollection.Input;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.TournamentGames;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace SandBox.ViewModelCollection.Tournament
{
	public class TournamentVM : ViewModel
	{
		public Action DisableUI { get; }

		public TournamentBehavior Tournament { get; }

		public TournamentVM(Action disableUI, TournamentBehavior tournamentBehavior)
		{
			this.DisableUI = disableUI;
			this.CurrentMatch = new TournamentMatchVM();
			this.Round1 = new TournamentRoundVM();
			this.Round2 = new TournamentRoundVM();
			this.Round3 = new TournamentRoundVM();
			this.Round4 = new TournamentRoundVM();
			this._rounds = new List<TournamentRoundVM> { this.Round1, this.Round2, this.Round3, this.Round4 };
			this._tournamentWinner = new TournamentParticipantVM();
			this.Tournament = tournamentBehavior;
			this.WinnerIntro = GameTexts.FindText("str_tournament_winner_intro", null).ToString();
			this.BattleRewards = new MBBindingList<TournamentRewardVM>();
			for (int i = 0; i < this._rounds.Count; i++)
			{
				this._rounds[i].Initialize(this.Tournament.Rounds[i], GameTexts.FindText("str_tournament_round", i.ToString()));
			}
			this.Refresh();
			this.Tournament.TournamentEnd += this.OnTournamentEnd;
			this.PrizeVisual = (this.HasPrizeItem ? new ImageIdentifierVM(this.Tournament.TournamentGame.Prize, "") : new ImageIdentifierVM(0));
			this.SkipAllRoundsHint = new HintViewModel();
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.LeaveText = GameTexts.FindText("str_tournament_leave", null).ToString();
			this.SkipRoundText = GameTexts.FindText("str_tournament_skip_round", null).ToString();
			this.WatchRoundText = GameTexts.FindText("str_tournament_watch_round", null).ToString();
			this.JoinTournamentText = GameTexts.FindText("str_tournament_join_tournament", null).ToString();
			this.BetText = GameTexts.FindText("str_bet", null).ToString();
			this.AcceptText = GameTexts.FindText("str_accept", null).ToString();
			this.CancelText = GameTexts.FindText("str_cancel", null).ToString();
			this.TournamentWinnerTitle = GameTexts.FindText("str_tournament_winner_title", null).ToString();
			this.BetTitleText = GameTexts.FindText("str_wager", null).ToString();
			GameTexts.SetVariable("MAX_AMOUNT", this.Tournament.GetMaximumBet());
			GameTexts.SetVariable("GOLD_ICON", "{=!}<img src=\"General\\Icons\\Coin@2x\" extend=\"8\">");
			this.BetDescriptionText = GameTexts.FindText("str_tournament_bet_description", null).ToString();
			this.TournamentPrizeText = GameTexts.FindText("str_tournament_prize", null).ToString();
			this.PrizeItemName = this.Tournament.TournamentGame.Prize.Name.ToString();
			MBTextManager.SetTextVariable("SETTLEMENT_NAME", this.Tournament.Settlement.Name, false);
			this.TournamentTitle = GameTexts.FindText("str_tournament", null).ToString();
			this.CurrentWagerText = GameTexts.FindText("str_tournament_current_wager", null).ToString();
			this.SkipAllRoundsHint.HintText = new TextObject("{=GaOE4bdd}Skip All Rounds", null);
			TournamentRoundVM round = this._round1;
			if (round != null)
			{
				round.RefreshValues();
			}
			TournamentRoundVM round2 = this._round2;
			if (round2 != null)
			{
				round2.RefreshValues();
			}
			TournamentRoundVM round3 = this._round3;
			if (round3 != null)
			{
				round3.RefreshValues();
			}
			TournamentRoundVM round4 = this._round4;
			if (round4 != null)
			{
				round4.RefreshValues();
			}
			TournamentMatchVM currentMatch = this._currentMatch;
			if (currentMatch != null)
			{
				currentMatch.RefreshValues();
			}
			TournamentParticipantVM tournamentWinner = this._tournamentWinner;
			if (tournamentWinner == null)
			{
				return;
			}
			tournamentWinner.RefreshValues();
		}

		public void ExecuteBet()
		{
			this._thisRoundBettedAmount += this.WageredDenars;
			this.Tournament.PlaceABet(this.WageredDenars);
			this.RefreshBetProperties();
		}

		public void ExecuteJoinTournament()
		{
			if (this.PlayerCanJoinMatch())
			{
				this.Tournament.StartMatch();
				this.IsCurrentMatchActive = true;
				this.CurrentMatch.Refresh(true);
				this.CurrentMatch.State = 3;
				this.DisableUI();
				this.IsCurrentMatchActive = true;
			}
		}

		public void ExecuteSkipRound()
		{
			if (this.IsTournamentIncomplete)
			{
				this.Tournament.SkipMatch(false);
			}
			this.Refresh();
		}

		public void ExecuteSkipAllRounds()
		{
			int num = 0;
			int num2 = this.Tournament.Rounds.Sum((TournamentRound r) => r.Matches.Length);
			while (!this.CanPlayerJoin)
			{
				TournamentRound currentRound = this.Tournament.CurrentRound;
				if (((currentRound != null) ? currentRound.CurrentMatch : null) == null || num >= num2)
				{
					break;
				}
				this.ExecuteSkipRound();
				num++;
			}
		}

		public void ExecuteWatchRound()
		{
			if (!this.PlayerCanJoinMatch())
			{
				this.Tournament.StartMatch();
				this.IsCurrentMatchActive = true;
				this.CurrentMatch.Refresh(true);
				this.CurrentMatch.State = 3;
				this.DisableUI();
				this.IsCurrentMatchActive = true;
			}
		}

		public void ExecuteLeave()
		{
			if (this.CurrentMatch != null)
			{
				List<TournamentMatch> list = new List<TournamentMatch>();
				for (int i = this.Tournament.CurrentRoundIndex; i < this.Tournament.Rounds.Length; i++)
				{
					list.AddRange(this.Tournament.Rounds[i].Matches.Where((TournamentMatch x) => x.State != 2));
				}
				if (list.Any((TournamentMatch x) => x.Participants.Any((TournamentParticipant y) => y.Character == CharacterObject.PlayerCharacter)))
				{
					InformationManager.ShowInquiry(new InquiryData(GameTexts.FindText("str_forfeit", null).ToString(), GameTexts.FindText("str_tournament_forfeit_game", null).ToString(), true, true, GameTexts.FindText("str_yes", null).ToString(), GameTexts.FindText("str_no", null).ToString(), new Action(this.EndTournamentMission), null, "", 0f, null, null, null), true, false);
					return;
				}
			}
			this.EndTournamentMission();
		}

		private void EndTournamentMission()
		{
			this.Tournament.EndTournamentViaLeave();
			Mission.Current.EndMission();
		}

		private void RefreshBetProperties()
		{
			TextObject textObject = new TextObject("{=L9GnQvsq}Stake: {BETTED_DENARS}", null);
			textObject.SetTextVariable("BETTED_DENARS", this.Tournament.BettedDenars);
			this.BettedDenarsText = textObject.ToString();
			TextObject textObject2 = new TextObject("{=xzzSaN4b}Expected: {OVERALL_EXPECTED_DENARS}", null);
			textObject2.SetTextVariable("OVERALL_EXPECTED_DENARS", this.Tournament.OverallExpectedDenars);
			this.OverallExpectedDenarsText = textObject2.ToString();
			TextObject textObject3 = new TextObject("{=yF5fpwNE}Total: {TOTAL}", null);
			textObject3.SetTextVariable("TOTAL", this.Tournament.PlayerDenars);
			this.TotalDenarsText = textObject3.ToString();
			base.OnPropertyChanged("IsBetButtonEnabled");
			this.MaximumBetValue = MathF.Min(this.Tournament.GetMaximumBet() - this._thisRoundBettedAmount, Hero.MainHero.Gold);
			GameTexts.SetVariable("NORMALIZED_EXPECTED_GOLD", (int)(this.Tournament.BetOdd * 100f));
			GameTexts.SetVariable("GOLD_ICON", "{=!}<img src=\"General\\Icons\\Coin@2x\" extend=\"8\">");
			this.BetOddsText = GameTexts.FindText("str_tournament_bet_odd", null).ToString();
		}

		private void OnNewRoundStarted(int prevRoundIndex, int currentRoundIndex)
		{
			this._isPlayerParticipating = this.Tournament.IsPlayerParticipating;
			this._thisRoundBettedAmount = 0;
		}

		public void Refresh()
		{
			this.IsCurrentMatchActive = false;
			this.CurrentMatch = this._rounds[this.Tournament.CurrentRoundIndex].Matches.Find((TournamentMatchVM m) => m.IsValid && m.Match == this.Tournament.CurrentMatch);
			this.ActiveRoundIndex = this.Tournament.CurrentRoundIndex;
			this.CanPlayerJoin = this.PlayerCanJoinMatch();
			base.OnPropertyChanged("IsTournamentIncomplete");
			base.OnPropertyChanged("InitializationOver");
			base.OnPropertyChanged("IsBetButtonEnabled");
			this.HasPrizeItem = this.Tournament.TournamentGame.Prize != null && !this.IsOver;
		}

		private void OnTournamentEnd()
		{
			TournamentParticipantVM[] array = this.Round4.Matches.Last((TournamentMatchVM m) => m.IsValid).GetParticipants().ToArray<TournamentParticipantVM>();
			TournamentParticipantVM tournamentParticipantVM = array[0];
			TournamentParticipantVM tournamentParticipantVM2 = array[1];
			this.TournamentWinner = this.Round4.Matches.Last((TournamentMatchVM m) => m.IsValid).GetParticipants().First((TournamentParticipantVM p) => p.Participant == this.Tournament.Winner);
			this.TournamentWinner.Refresh();
			if (this.TournamentWinner.Participant.Character.IsHero)
			{
				Hero heroObject = this.TournamentWinner.Participant.Character.HeroObject;
				this.TournamentWinner.Character.ArmorColor1 = heroObject.MapFaction.Color;
				this.TournamentWinner.Character.ArmorColor2 = heroObject.MapFaction.Color2;
			}
			else
			{
				CultureObject culture = this.TournamentWinner.Participant.Character.Culture;
				this.TournamentWinner.Character.ArmorColor1 = culture.Color;
				this.TournamentWinner.Character.ArmorColor2 = culture.Color2;
			}
			this.IsWinnerHero = this.Tournament.Winner.Character.IsHero;
			if (this.IsWinnerHero)
			{
				this.WinnerBanner = new ImageIdentifierVM(BannerCode.CreateFrom(this.Tournament.Winner.Character.HeroObject.ClanBanner), true);
			}
			if (this.TournamentWinner.Participant.Character.IsPlayerCharacter)
			{
				TournamentParticipantVM tournamentParticipantVM3 = ((tournamentParticipantVM == this.TournamentWinner) ? tournamentParticipantVM2 : tournamentParticipantVM);
				GameTexts.SetVariable("TOURNAMENT_FINAL_OPPONENT", tournamentParticipantVM3.Name);
				this.WinnerIntro = GameTexts.FindText("str_tournament_result_won", null).ToString();
				if (this.Tournament.TournamentGame.TournamentWinRenown > 0f)
				{
					GameTexts.SetVariable("RENOWN", this.Tournament.TournamentGame.TournamentWinRenown.ToString("F1"));
					this.BattleRewards.Add(new TournamentRewardVM(GameTexts.FindText("str_tournament_renown", null).ToString()));
				}
				if (this.Tournament.TournamentGame.TournamentWinInfluence > 0f)
				{
					float tournamentWinInfluence = this.Tournament.TournamentGame.TournamentWinInfluence;
					TextObject textObject = GameTexts.FindText("str_tournament_influence", null);
					textObject.SetTextVariable("INFLUENCE", tournamentWinInfluence.ToString("F1"));
					textObject.SetTextVariable("INFLUENCE_ICON", "{=!}<img src=\"General\\Icons\\Influence@2x\" extend=\"7\">");
					this.BattleRewards.Add(new TournamentRewardVM(textObject.ToString()));
				}
				if (this.Tournament.TournamentGame.Prize != null)
				{
					string text = this.Tournament.TournamentGame.Prize.Name.ToString();
					GameTexts.SetVariable("REWARD", text);
					this.BattleRewards.Add(new TournamentRewardVM(GameTexts.FindText("str_tournament_reward", null).ToString(), new ImageIdentifierVM(this.Tournament.TournamentGame.Prize, "")));
				}
				if (this.Tournament.OverallExpectedDenars > 0)
				{
					int overallExpectedDenars = this.Tournament.OverallExpectedDenars;
					TextObject textObject2 = GameTexts.FindText("str_tournament_bet", null);
					textObject2.SetTextVariable("BET", overallExpectedDenars);
					textObject2.SetTextVariable("GOLD_ICON", "{=!}<img src=\"General\\Icons\\Coin@2x\" extend=\"8\">");
					this.BattleRewards.Add(new TournamentRewardVM(textObject2.ToString()));
				}
			}
			else if (tournamentParticipantVM.Participant.Character.IsPlayerCharacter || tournamentParticipantVM2.Participant.Character.IsPlayerCharacter)
			{
				TournamentParticipantVM tournamentParticipantVM4 = ((tournamentParticipantVM == this.TournamentWinner) ? tournamentParticipantVM : tournamentParticipantVM2);
				GameTexts.SetVariable("TOURNAMENT_FINAL_OPPONENT", tournamentParticipantVM4.Name);
				this.WinnerIntro = GameTexts.FindText("str_tournament_result_eliminated_at_final", null).ToString();
			}
			else
			{
				int num = 3;
				bool flag = this.Round3.GetParticipants().Any((TournamentParticipantVM p) => p.Participant.Character.IsPlayerCharacter);
				bool flag2 = this.Round2.GetParticipants().Any((TournamentParticipantVM p) => p.Participant.Character.IsPlayerCharacter);
				bool flag3 = this.Round1.GetParticipants().Any((TournamentParticipantVM p) => p.Participant.Character.IsPlayerCharacter);
				if (flag)
				{
					num = 3;
				}
				else if (flag2)
				{
					num = 2;
				}
				else if (flag3)
				{
					num = 1;
				}
				bool flag4 = tournamentParticipantVM == this.TournamentWinner;
				GameTexts.SetVariable("TOURNAMENT_FINAL_PARTICIPANT_A", flag4 ? tournamentParticipantVM.Name : tournamentParticipantVM2.Name);
				GameTexts.SetVariable("TOURNAMENT_FINAL_PARTICIPANT_B", flag4 ? tournamentParticipantVM2.Name : tournamentParticipantVM.Name);
				if (this._isPlayerParticipating)
				{
					GameTexts.SetVariable("TOURNAMENT_ELIMINATED_ROUND", num.ToString());
					this.WinnerIntro = GameTexts.FindText("str_tournament_result_eliminated", null).ToString();
				}
				else
				{
					this.WinnerIntro = GameTexts.FindText("str_tournament_result_spectator", null).ToString();
				}
			}
			this.IsOver = true;
		}

		private bool PlayerCanJoinMatch()
		{
			if (this.IsTournamentIncomplete)
			{
				return this.Tournament.CurrentMatch.Participants.Any((TournamentParticipant x) => x.Character == CharacterObject.PlayerCharacter);
			}
			return false;
		}

		public void OnAgentRemoved(Agent agent)
		{
			if (this.IsCurrentMatchActive && agent.IsHuman)
			{
				TournamentParticipant participant = this.CurrentMatch.Match.GetParticipant(agent.Origin.UniqueSeed);
				if (participant != null)
				{
					this.CurrentMatch.GetParticipants().First((TournamentParticipantVM p) => p.Participant == participant).IsDead = true;
				}
			}
		}

		public void ExecuteShowPrizeItemTooltip()
		{
			if (this.HasPrizeItem)
			{
				InformationManager.ShowTooltip(typeof(ItemObject), new object[]
				{
					new EquipmentElement(this.Tournament.TournamentGame.Prize, null, null, false)
				});
			}
		}

		public void ExecuteHidePrizeItemTooltip()
		{
			MBInformationManager.HideInformations();
		}

		public override void OnFinalize()
		{
			base.OnFinalize();
			InputKeyItemVM doneInputKey = this.DoneInputKey;
			if (doneInputKey != null)
			{
				doneInputKey.OnFinalize();
			}
			InputKeyItemVM cancelInputKey = this.CancelInputKey;
			if (cancelInputKey == null)
			{
				return;
			}
			cancelInputKey.OnFinalize();
		}

		public void SetDoneInputKey(HotKey hotKey)
		{
			this.DoneInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		public void SetCancelInputKey(HotKey hotKey)
		{
			this.CancelInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		[DataSourceProperty]
		public InputKeyItemVM DoneInputKey
		{
			get
			{
				return this._doneInputKey;
			}
			set
			{
				if (value != this._doneInputKey)
				{
					this._doneInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "DoneInputKey");
				}
			}
		}

		[DataSourceProperty]
		public InputKeyItemVM CancelInputKey
		{
			get
			{
				return this._cancelInputKey;
			}
			set
			{
				if (value != this._cancelInputKey)
				{
					this._cancelInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "CancelInputKey");
				}
			}
		}

		[DataSourceProperty]
		public string TournamentWinnerTitle
		{
			get
			{
				return this._tournamentWinnerTitle;
			}
			set
			{
				if (value != this._tournamentWinnerTitle)
				{
					this._tournamentWinnerTitle = value;
					base.OnPropertyChangedWithValue<string>(value, "TournamentWinnerTitle");
				}
			}
		}

		[DataSourceProperty]
		public TournamentParticipantVM TournamentWinner
		{
			get
			{
				return this._tournamentWinner;
			}
			set
			{
				if (value != this._tournamentWinner)
				{
					this._tournamentWinner = value;
					base.OnPropertyChangedWithValue<TournamentParticipantVM>(value, "TournamentWinner");
				}
			}
		}

		[DataSourceProperty]
		public int MaximumBetValue
		{
			get
			{
				return this._maximumBetValue;
			}
			set
			{
				if (value != this._maximumBetValue)
				{
					this._maximumBetValue = value;
					base.OnPropertyChangedWithValue(value, "MaximumBetValue");
					this._wageredDenars = -1;
					this.WageredDenars = 0;
				}
			}
		}

		[DataSourceProperty]
		public bool IsBetButtonEnabled
		{
			get
			{
				return this.PlayerCanJoinMatch() && this.Tournament.GetMaximumBet() > this._thisRoundBettedAmount && Hero.MainHero.Gold > 0;
			}
		}

		[DataSourceProperty]
		public string BetText
		{
			get
			{
				return this._betText;
			}
			set
			{
				if (value != this._betText)
				{
					this._betText = value;
					base.OnPropertyChangedWithValue<string>(value, "BetText");
				}
			}
		}

		[DataSourceProperty]
		public string BetTitleText
		{
			get
			{
				return this._betTitleText;
			}
			set
			{
				if (value != this._betTitleText)
				{
					this._betTitleText = value;
					base.OnPropertyChangedWithValue<string>(value, "BetTitleText");
				}
			}
		}

		[DataSourceProperty]
		public string CurrentWagerText
		{
			get
			{
				return this._currentWagerText;
			}
			set
			{
				if (value != this._currentWagerText)
				{
					this._currentWagerText = value;
					base.OnPropertyChangedWithValue<string>(value, "CurrentWagerText");
				}
			}
		}

		[DataSourceProperty]
		public string BetDescriptionText
		{
			get
			{
				return this._betDescriptionText;
			}
			set
			{
				if (value != this._betDescriptionText)
				{
					this._betDescriptionText = value;
					base.OnPropertyChangedWithValue<string>(value, "BetDescriptionText");
				}
			}
		}

		[DataSourceProperty]
		public ImageIdentifierVM PrizeVisual
		{
			get
			{
				return this._prizeVisual;
			}
			set
			{
				if (value != this._prizeVisual)
				{
					this._prizeVisual = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "PrizeVisual");
				}
			}
		}

		[DataSourceProperty]
		public string PrizeItemName
		{
			get
			{
				return this._prizeItemName;
			}
			set
			{
				if (value != this._prizeItemName)
				{
					this._prizeItemName = value;
					base.OnPropertyChangedWithValue<string>(value, "PrizeItemName");
				}
			}
		}

		[DataSourceProperty]
		public string TournamentPrizeText
		{
			get
			{
				return this._tournamentPrizeText;
			}
			set
			{
				if (value != this._tournamentPrizeText)
				{
					this._tournamentPrizeText = value;
					base.OnPropertyChangedWithValue<string>(value, "TournamentPrizeText");
				}
			}
		}

		[DataSourceProperty]
		public int WageredDenars
		{
			get
			{
				return this._wageredDenars;
			}
			set
			{
				if (value != this._wageredDenars)
				{
					this._wageredDenars = value;
					base.OnPropertyChangedWithValue(value, "WageredDenars");
					this.ExpectedBetDenars = ((this._wageredDenars == 0) ? 0 : this.Tournament.GetExpectedDenarsForBet(this._wageredDenars));
				}
			}
		}

		[DataSourceProperty]
		public int ExpectedBetDenars
		{
			get
			{
				return this._expectedBetDenars;
			}
			set
			{
				if (value != this._expectedBetDenars)
				{
					this._expectedBetDenars = value;
					base.OnPropertyChangedWithValue(value, "ExpectedBetDenars");
				}
			}
		}

		[DataSourceProperty]
		public string BetOddsText
		{
			get
			{
				return this._betOddsText;
			}
			set
			{
				if (value != this._betOddsText)
				{
					this._betOddsText = value;
					base.OnPropertyChangedWithValue<string>(value, "BetOddsText");
				}
			}
		}

		[DataSourceProperty]
		public string BettedDenarsText
		{
			get
			{
				return this._bettedDenarsText;
			}
			set
			{
				if (value != this._bettedDenarsText)
				{
					this._bettedDenarsText = value;
					base.OnPropertyChangedWithValue<string>(value, "BettedDenarsText");
				}
			}
		}

		[DataSourceProperty]
		public string OverallExpectedDenarsText
		{
			get
			{
				return this._overallExpectedDenarsText;
			}
			set
			{
				if (value != this._overallExpectedDenarsText)
				{
					this._overallExpectedDenarsText = value;
					base.OnPropertyChangedWithValue<string>(value, "OverallExpectedDenarsText");
				}
			}
		}

		[DataSourceProperty]
		public string CurrentExpectedDenarsText
		{
			get
			{
				return this._currentExpectedDenarsText;
			}
			set
			{
				if (value != this._currentExpectedDenarsText)
				{
					this._currentExpectedDenarsText = value;
					base.OnPropertyChangedWithValue<string>(value, "CurrentExpectedDenarsText");
				}
			}
		}

		[DataSourceProperty]
		public string TotalDenarsText
		{
			get
			{
				return this._totalDenarsText;
			}
			set
			{
				if (value != this._totalDenarsText)
				{
					this._totalDenarsText = value;
					base.OnPropertyChangedWithValue<string>(value, "TotalDenarsText");
				}
			}
		}

		[DataSourceProperty]
		public string AcceptText
		{
			get
			{
				return this._acceptText;
			}
			set
			{
				if (value != this._acceptText)
				{
					this._acceptText = value;
					base.OnPropertyChangedWithValue<string>(value, "AcceptText");
				}
			}
		}

		[DataSourceProperty]
		public string CancelText
		{
			get
			{
				return this._cancelText;
			}
			set
			{
				if (value != this._cancelText)
				{
					this._cancelText = value;
					base.OnPropertyChangedWithValue<string>(value, "CancelText");
				}
			}
		}

		[DataSourceProperty]
		public bool IsCurrentMatchActive
		{
			get
			{
				return this._isCurrentMatchActive;
			}
			set
			{
				this._isCurrentMatchActive = value;
				base.OnPropertyChangedWithValue(value, "IsCurrentMatchActive");
			}
		}

		[DataSourceProperty]
		public TournamentMatchVM CurrentMatch
		{
			get
			{
				return this._currentMatch;
			}
			set
			{
				if (value != this._currentMatch)
				{
					TournamentMatchVM currentMatch = this._currentMatch;
					if (currentMatch != null && currentMatch.IsValid)
					{
						this._currentMatch.State = 2;
						this._currentMatch.Refresh(false);
						int num = this._rounds.FindIndex((TournamentRoundVM r) => r.Matches.Any((TournamentMatchVM m) => m.Match == this.Tournament.LastMatch));
						if (num < this.Tournament.Rounds.Length - 1)
						{
							this._rounds[num + 1].Initialize();
						}
					}
					this._currentMatch = value;
					base.OnPropertyChangedWithValue<TournamentMatchVM>(value, "CurrentMatch");
					if (this._currentMatch != null)
					{
						this._currentMatch.State = 1;
					}
				}
			}
		}

		[DataSourceProperty]
		public bool IsTournamentIncomplete
		{
			get
			{
				return this.Tournament == null || this.Tournament.CurrentMatch != null;
			}
			set
			{
			}
		}

		[DataSourceProperty]
		public int ActiveRoundIndex
		{
			get
			{
				return this._activeRoundIndex;
			}
			set
			{
				if (value != this._activeRoundIndex)
				{
					this.OnNewRoundStarted(this._activeRoundIndex, value);
					this._activeRoundIndex = value;
					base.OnPropertyChangedWithValue(value, "ActiveRoundIndex");
					this.RefreshBetProperties();
				}
			}
		}

		[DataSourceProperty]
		public bool CanPlayerJoin
		{
			get
			{
				return this._canPlayerJoin;
			}
			set
			{
				if (value != this._canPlayerJoin)
				{
					this._canPlayerJoin = value;
					base.OnPropertyChangedWithValue(value, "CanPlayerJoin");
				}
			}
		}

		[DataSourceProperty]
		public bool HasPrizeItem
		{
			get
			{
				return this._hasPrizeItem;
			}
			set
			{
				if (value != this._hasPrizeItem)
				{
					this._hasPrizeItem = value;
					base.OnPropertyChangedWithValue(value, "HasPrizeItem");
				}
			}
		}

		[DataSourceProperty]
		public string JoinTournamentText
		{
			get
			{
				return this._joinTournamentText;
			}
			set
			{
				if (value != this._joinTournamentText)
				{
					this._joinTournamentText = value;
					base.OnPropertyChangedWithValue<string>(value, "JoinTournamentText");
				}
			}
		}

		[DataSourceProperty]
		public string SkipRoundText
		{
			get
			{
				return this._skipRoundText;
			}
			set
			{
				if (value != this._skipRoundText)
				{
					this._skipRoundText = value;
					base.OnPropertyChangedWithValue<string>(value, "SkipRoundText");
				}
			}
		}

		[DataSourceProperty]
		public string WatchRoundText
		{
			get
			{
				return this._watchRoundText;
			}
			set
			{
				if (value != this._watchRoundText)
				{
					this._watchRoundText = value;
					base.OnPropertyChangedWithValue<string>(value, "WatchRoundText");
				}
			}
		}

		[DataSourceProperty]
		public string LeaveText
		{
			get
			{
				return this._leaveText;
			}
			set
			{
				if (value != this._leaveText)
				{
					this._leaveText = value;
					base.OnPropertyChangedWithValue<string>(value, "LeaveText");
				}
			}
		}

		[DataSourceProperty]
		public TournamentRoundVM Round1
		{
			get
			{
				return this._round1;
			}
			set
			{
				if (value != this._round1)
				{
					this._round1 = value;
					base.OnPropertyChangedWithValue<TournamentRoundVM>(value, "Round1");
				}
			}
		}

		[DataSourceProperty]
		public TournamentRoundVM Round2
		{
			get
			{
				return this._round2;
			}
			set
			{
				if (value != this._round2)
				{
					this._round2 = value;
					base.OnPropertyChangedWithValue<TournamentRoundVM>(value, "Round2");
				}
			}
		}

		[DataSourceProperty]
		public TournamentRoundVM Round3
		{
			get
			{
				return this._round3;
			}
			set
			{
				if (value != this._round3)
				{
					this._round3 = value;
					base.OnPropertyChangedWithValue<TournamentRoundVM>(value, "Round3");
				}
			}
		}

		[DataSourceProperty]
		public TournamentRoundVM Round4
		{
			get
			{
				return this._round4;
			}
			set
			{
				if (value != this._round4)
				{
					this._round4 = value;
					base.OnPropertyChangedWithValue<TournamentRoundVM>(value, "Round4");
				}
			}
		}

		[DataSourceProperty]
		public bool InitializationOver
		{
			get
			{
				return true;
			}
		}

		[DataSourceProperty]
		public string TournamentTitle
		{
			get
			{
				return this._tournamentTitle;
			}
			set
			{
				if (value != this._tournamentTitle)
				{
					this._tournamentTitle = value;
					base.OnPropertyChangedWithValue<string>(value, "TournamentTitle");
				}
			}
		}

		[DataSourceProperty]
		public bool IsOver
		{
			get
			{
				return this._isOver;
			}
			set
			{
				if (this._isOver != value)
				{
					this._isOver = value;
					base.OnPropertyChangedWithValue(value, "IsOver");
				}
			}
		}

		[DataSourceProperty]
		public string WinnerIntro
		{
			get
			{
				return this._winnerIntro;
			}
			set
			{
				if (value != this._winnerIntro)
				{
					this._winnerIntro = value;
					base.OnPropertyChangedWithValue<string>(value, "WinnerIntro");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<TournamentRewardVM> BattleRewards
		{
			get
			{
				return this._battleRewards;
			}
			set
			{
				if (value != this._battleRewards)
				{
					this._battleRewards = value;
					base.OnPropertyChangedWithValue<MBBindingList<TournamentRewardVM>>(value, "BattleRewards");
				}
			}
		}

		[DataSourceProperty]
		public bool IsWinnerHero
		{
			get
			{
				return this._isWinnerHero;
			}
			set
			{
				if (value != this._isWinnerHero)
				{
					this._isWinnerHero = value;
					base.OnPropertyChangedWithValue(value, "IsWinnerHero");
				}
			}
		}

		[DataSourceProperty]
		public bool IsBetWindowEnabled
		{
			get
			{
				return this._isBetWindowEnabled;
			}
			set
			{
				if (value != this._isBetWindowEnabled)
				{
					this._isBetWindowEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsBetWindowEnabled");
				}
			}
		}

		[DataSourceProperty]
		public ImageIdentifierVM WinnerBanner
		{
			get
			{
				return this._winnerBanner;
			}
			set
			{
				if (value != this._winnerBanner)
				{
					this._winnerBanner = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "WinnerBanner");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel SkipAllRoundsHint
		{
			get
			{
				return this._skipAllRoundsHint;
			}
			set
			{
				if (value != this._skipAllRoundsHint)
				{
					this._skipAllRoundsHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "SkipAllRoundsHint");
				}
			}
		}

		private readonly List<TournamentRoundVM> _rounds;

		private int _thisRoundBettedAmount;

		private bool _isPlayerParticipating;

		private InputKeyItemVM _doneInputKey;

		private InputKeyItemVM _cancelInputKey;

		private TournamentRoundVM _round1;

		private TournamentRoundVM _round2;

		private TournamentRoundVM _round3;

		private TournamentRoundVM _round4;

		private int _activeRoundIndex = -1;

		private string _joinTournamentText;

		private string _skipRoundText;

		private string _watchRoundText;

		private string _leaveText;

		private bool _canPlayerJoin;

		private TournamentMatchVM _currentMatch;

		private bool _isCurrentMatchActive;

		private string _betTitleText;

		private string _betDescriptionText;

		private string _betOddsText;

		private string _bettedDenarsText;

		private string _overallExpectedDenarsText;

		private string _currentExpectedDenarsText;

		private string _totalDenarsText;

		private string _acceptText;

		private string _cancelText;

		private string _prizeItemName;

		private string _tournamentPrizeText;

		private string _currentWagerText;

		private int _wageredDenars = -1;

		private int _expectedBetDenars = -1;

		private string _betText;

		private int _maximumBetValue;

		private string _tournamentWinnerTitle;

		private TournamentParticipantVM _tournamentWinner;

		private string _tournamentTitle;

		private bool _isOver;

		private bool _hasPrizeItem;

		private bool _isWinnerHero;

		private bool _isBetWindowEnabled;

		private string _winnerIntro;

		private ImageIdentifierVM _prizeVisual;

		private ImageIdentifierVM _winnerBanner;

		private MBBindingList<TournamentRewardVM> _battleRewards;

		private HintViewModel _skipAllRoundsHint;
	}
}
