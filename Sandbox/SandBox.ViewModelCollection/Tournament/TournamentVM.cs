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
	// Token: 0x0200000C RID: 12
	public class TournamentVM : ViewModel
	{
		// Token: 0x17000034 RID: 52
		// (get) Token: 0x060000B1 RID: 177 RVA: 0x00005FA0 File Offset: 0x000041A0
		public Action DisableUI { get; }

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x060000B2 RID: 178 RVA: 0x00005FA8 File Offset: 0x000041A8
		public TournamentBehavior Tournament { get; }

		// Token: 0x060000B3 RID: 179 RVA: 0x00005FB0 File Offset: 0x000041B0
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

		// Token: 0x060000B4 RID: 180 RVA: 0x00006128 File Offset: 0x00004328
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

		// Token: 0x060000B5 RID: 181 RVA: 0x00006333 File Offset: 0x00004533
		public void ExecuteBet()
		{
			this._thisRoundBettedAmount += this.WageredDenars;
			this.Tournament.PlaceABet(this.WageredDenars);
			this.RefreshBetProperties();
		}

		// Token: 0x060000B6 RID: 182 RVA: 0x00006360 File Offset: 0x00004560
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

		// Token: 0x060000B7 RID: 183 RVA: 0x000063B1 File Offset: 0x000045B1
		public void ExecuteSkipRound()
		{
			if (this.IsTournamentIncomplete)
			{
				this.Tournament.SkipMatch(false);
			}
			this.Refresh();
		}

		// Token: 0x060000B8 RID: 184 RVA: 0x000063D0 File Offset: 0x000045D0
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

		// Token: 0x060000B9 RID: 185 RVA: 0x00006440 File Offset: 0x00004640
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

		// Token: 0x060000BA RID: 186 RVA: 0x00006494 File Offset: 0x00004694
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

		// Token: 0x060000BB RID: 187 RVA: 0x000065A6 File Offset: 0x000047A6
		private void EndTournamentMission()
		{
			this.Tournament.EndTournamentViaLeave();
			Mission.Current.EndMission();
		}

		// Token: 0x060000BC RID: 188 RVA: 0x000065C0 File Offset: 0x000047C0
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

		// Token: 0x060000BD RID: 189 RVA: 0x000066CD File Offset: 0x000048CD
		private void OnNewRoundStarted(int prevRoundIndex, int currentRoundIndex)
		{
			this._isPlayerParticipating = this.Tournament.IsPlayerParticipating;
			this._thisRoundBettedAmount = 0;
		}

		// Token: 0x060000BE RID: 190 RVA: 0x000066E8 File Offset: 0x000048E8
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

		// Token: 0x060000BF RID: 191 RVA: 0x00006790 File Offset: 0x00004990
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

		// Token: 0x060000C0 RID: 192 RVA: 0x00006CC6 File Offset: 0x00004EC6
		private bool PlayerCanJoinMatch()
		{
			if (this.IsTournamentIncomplete)
			{
				return this.Tournament.CurrentMatch.Participants.Any((TournamentParticipant x) => x.Character == CharacterObject.PlayerCharacter);
			}
			return false;
		}

		// Token: 0x060000C1 RID: 193 RVA: 0x00006D08 File Offset: 0x00004F08
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

		// Token: 0x060000C2 RID: 194 RVA: 0x00006D76 File Offset: 0x00004F76
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

		// Token: 0x060000C3 RID: 195 RVA: 0x00006DB5 File Offset: 0x00004FB5
		public void ExecuteHidePrizeItemTooltip()
		{
			MBInformationManager.HideInformations();
		}

		// Token: 0x060000C4 RID: 196 RVA: 0x00006DBC File Offset: 0x00004FBC
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

		// Token: 0x060000C5 RID: 197 RVA: 0x00006DE5 File Offset: 0x00004FE5
		public void SetDoneInputKey(HotKey hotKey)
		{
			this.DoneInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		// Token: 0x060000C6 RID: 198 RVA: 0x00006DF4 File Offset: 0x00004FF4
		public void SetCancelInputKey(HotKey hotKey)
		{
			this.CancelInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x060000C7 RID: 199 RVA: 0x00006E03 File Offset: 0x00005003
		// (set) Token: 0x060000C8 RID: 200 RVA: 0x00006E0B File Offset: 0x0000500B
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

		// Token: 0x17000037 RID: 55
		// (get) Token: 0x060000C9 RID: 201 RVA: 0x00006E29 File Offset: 0x00005029
		// (set) Token: 0x060000CA RID: 202 RVA: 0x00006E31 File Offset: 0x00005031
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

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x060000CB RID: 203 RVA: 0x00006E4F File Offset: 0x0000504F
		// (set) Token: 0x060000CC RID: 204 RVA: 0x00006E57 File Offset: 0x00005057
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

		// Token: 0x17000039 RID: 57
		// (get) Token: 0x060000CD RID: 205 RVA: 0x00006E7A File Offset: 0x0000507A
		// (set) Token: 0x060000CE RID: 206 RVA: 0x00006E82 File Offset: 0x00005082
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

		// Token: 0x1700003A RID: 58
		// (get) Token: 0x060000CF RID: 207 RVA: 0x00006EA0 File Offset: 0x000050A0
		// (set) Token: 0x060000D0 RID: 208 RVA: 0x00006EA8 File Offset: 0x000050A8
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

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x060000D1 RID: 209 RVA: 0x00006ED4 File Offset: 0x000050D4
		[DataSourceProperty]
		public bool IsBetButtonEnabled
		{
			get
			{
				return this.PlayerCanJoinMatch() && this.Tournament.GetMaximumBet() > this._thisRoundBettedAmount && Hero.MainHero.Gold > 0;
			}
		}

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x060000D2 RID: 210 RVA: 0x00006F00 File Offset: 0x00005100
		// (set) Token: 0x060000D3 RID: 211 RVA: 0x00006F08 File Offset: 0x00005108
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

		// Token: 0x1700003D RID: 61
		// (get) Token: 0x060000D4 RID: 212 RVA: 0x00006F2B File Offset: 0x0000512B
		// (set) Token: 0x060000D5 RID: 213 RVA: 0x00006F33 File Offset: 0x00005133
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

		// Token: 0x1700003E RID: 62
		// (get) Token: 0x060000D6 RID: 214 RVA: 0x00006F56 File Offset: 0x00005156
		// (set) Token: 0x060000D7 RID: 215 RVA: 0x00006F5E File Offset: 0x0000515E
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

		// Token: 0x1700003F RID: 63
		// (get) Token: 0x060000D8 RID: 216 RVA: 0x00006F81 File Offset: 0x00005181
		// (set) Token: 0x060000D9 RID: 217 RVA: 0x00006F89 File Offset: 0x00005189
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

		// Token: 0x17000040 RID: 64
		// (get) Token: 0x060000DA RID: 218 RVA: 0x00006FAC File Offset: 0x000051AC
		// (set) Token: 0x060000DB RID: 219 RVA: 0x00006FB4 File Offset: 0x000051B4
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

		// Token: 0x17000041 RID: 65
		// (get) Token: 0x060000DC RID: 220 RVA: 0x00006FD2 File Offset: 0x000051D2
		// (set) Token: 0x060000DD RID: 221 RVA: 0x00006FDA File Offset: 0x000051DA
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

		// Token: 0x17000042 RID: 66
		// (get) Token: 0x060000DE RID: 222 RVA: 0x00006FFD File Offset: 0x000051FD
		// (set) Token: 0x060000DF RID: 223 RVA: 0x00007005 File Offset: 0x00005205
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

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x060000E0 RID: 224 RVA: 0x00007028 File Offset: 0x00005228
		// (set) Token: 0x060000E1 RID: 225 RVA: 0x00007030 File Offset: 0x00005230
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

		// Token: 0x17000044 RID: 68
		// (get) Token: 0x060000E2 RID: 226 RVA: 0x00007070 File Offset: 0x00005270
		// (set) Token: 0x060000E3 RID: 227 RVA: 0x00007078 File Offset: 0x00005278
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

		// Token: 0x17000045 RID: 69
		// (get) Token: 0x060000E4 RID: 228 RVA: 0x00007096 File Offset: 0x00005296
		// (set) Token: 0x060000E5 RID: 229 RVA: 0x0000709E File Offset: 0x0000529E
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

		// Token: 0x17000046 RID: 70
		// (get) Token: 0x060000E6 RID: 230 RVA: 0x000070C1 File Offset: 0x000052C1
		// (set) Token: 0x060000E7 RID: 231 RVA: 0x000070C9 File Offset: 0x000052C9
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

		// Token: 0x17000047 RID: 71
		// (get) Token: 0x060000E8 RID: 232 RVA: 0x000070EC File Offset: 0x000052EC
		// (set) Token: 0x060000E9 RID: 233 RVA: 0x000070F4 File Offset: 0x000052F4
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

		// Token: 0x17000048 RID: 72
		// (get) Token: 0x060000EA RID: 234 RVA: 0x00007117 File Offset: 0x00005317
		// (set) Token: 0x060000EB RID: 235 RVA: 0x0000711F File Offset: 0x0000531F
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

		// Token: 0x17000049 RID: 73
		// (get) Token: 0x060000EC RID: 236 RVA: 0x00007142 File Offset: 0x00005342
		// (set) Token: 0x060000ED RID: 237 RVA: 0x0000714A File Offset: 0x0000534A
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

		// Token: 0x1700004A RID: 74
		// (get) Token: 0x060000EE RID: 238 RVA: 0x0000716D File Offset: 0x0000536D
		// (set) Token: 0x060000EF RID: 239 RVA: 0x00007175 File Offset: 0x00005375
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

		// Token: 0x1700004B RID: 75
		// (get) Token: 0x060000F0 RID: 240 RVA: 0x00007198 File Offset: 0x00005398
		// (set) Token: 0x060000F1 RID: 241 RVA: 0x000071A0 File Offset: 0x000053A0
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

		// Token: 0x1700004C RID: 76
		// (get) Token: 0x060000F2 RID: 242 RVA: 0x000071C3 File Offset: 0x000053C3
		// (set) Token: 0x060000F3 RID: 243 RVA: 0x000071CB File Offset: 0x000053CB
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

		// Token: 0x1700004D RID: 77
		// (get) Token: 0x060000F4 RID: 244 RVA: 0x000071E0 File Offset: 0x000053E0
		// (set) Token: 0x060000F5 RID: 245 RVA: 0x000071E8 File Offset: 0x000053E8
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

		// Token: 0x1700004E RID: 78
		// (get) Token: 0x060000F6 RID: 246 RVA: 0x00007291 File Offset: 0x00005491
		// (set) Token: 0x060000F7 RID: 247 RVA: 0x000072AB File Offset: 0x000054AB
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

		// Token: 0x1700004F RID: 79
		// (get) Token: 0x060000F8 RID: 248 RVA: 0x000072AD File Offset: 0x000054AD
		// (set) Token: 0x060000F9 RID: 249 RVA: 0x000072B5 File Offset: 0x000054B5
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

		// Token: 0x17000050 RID: 80
		// (get) Token: 0x060000FA RID: 250 RVA: 0x000072E6 File Offset: 0x000054E6
		// (set) Token: 0x060000FB RID: 251 RVA: 0x000072EE File Offset: 0x000054EE
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

		// Token: 0x17000051 RID: 81
		// (get) Token: 0x060000FC RID: 252 RVA: 0x0000730C File Offset: 0x0000550C
		// (set) Token: 0x060000FD RID: 253 RVA: 0x00007314 File Offset: 0x00005514
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

		// Token: 0x17000052 RID: 82
		// (get) Token: 0x060000FE RID: 254 RVA: 0x00007332 File Offset: 0x00005532
		// (set) Token: 0x060000FF RID: 255 RVA: 0x0000733A File Offset: 0x0000553A
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

		// Token: 0x17000053 RID: 83
		// (get) Token: 0x06000100 RID: 256 RVA: 0x0000735D File Offset: 0x0000555D
		// (set) Token: 0x06000101 RID: 257 RVA: 0x00007365 File Offset: 0x00005565
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

		// Token: 0x17000054 RID: 84
		// (get) Token: 0x06000102 RID: 258 RVA: 0x00007388 File Offset: 0x00005588
		// (set) Token: 0x06000103 RID: 259 RVA: 0x00007390 File Offset: 0x00005590
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

		// Token: 0x17000055 RID: 85
		// (get) Token: 0x06000104 RID: 260 RVA: 0x000073B3 File Offset: 0x000055B3
		// (set) Token: 0x06000105 RID: 261 RVA: 0x000073BB File Offset: 0x000055BB
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

		// Token: 0x17000056 RID: 86
		// (get) Token: 0x06000106 RID: 262 RVA: 0x000073DE File Offset: 0x000055DE
		// (set) Token: 0x06000107 RID: 263 RVA: 0x000073E6 File Offset: 0x000055E6
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

		// Token: 0x17000057 RID: 87
		// (get) Token: 0x06000108 RID: 264 RVA: 0x00007404 File Offset: 0x00005604
		// (set) Token: 0x06000109 RID: 265 RVA: 0x0000740C File Offset: 0x0000560C
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

		// Token: 0x17000058 RID: 88
		// (get) Token: 0x0600010A RID: 266 RVA: 0x0000742A File Offset: 0x0000562A
		// (set) Token: 0x0600010B RID: 267 RVA: 0x00007432 File Offset: 0x00005632
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

		// Token: 0x17000059 RID: 89
		// (get) Token: 0x0600010C RID: 268 RVA: 0x00007450 File Offset: 0x00005650
		// (set) Token: 0x0600010D RID: 269 RVA: 0x00007458 File Offset: 0x00005658
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

		// Token: 0x1700005A RID: 90
		// (get) Token: 0x0600010E RID: 270 RVA: 0x00007476 File Offset: 0x00005676
		[DataSourceProperty]
		public bool InitializationOver
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700005B RID: 91
		// (get) Token: 0x0600010F RID: 271 RVA: 0x00007479 File Offset: 0x00005679
		// (set) Token: 0x06000110 RID: 272 RVA: 0x00007481 File Offset: 0x00005681
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

		// Token: 0x1700005C RID: 92
		// (get) Token: 0x06000111 RID: 273 RVA: 0x000074A4 File Offset: 0x000056A4
		// (set) Token: 0x06000112 RID: 274 RVA: 0x000074AC File Offset: 0x000056AC
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

		// Token: 0x1700005D RID: 93
		// (get) Token: 0x06000113 RID: 275 RVA: 0x000074CA File Offset: 0x000056CA
		// (set) Token: 0x06000114 RID: 276 RVA: 0x000074D2 File Offset: 0x000056D2
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

		// Token: 0x1700005E RID: 94
		// (get) Token: 0x06000115 RID: 277 RVA: 0x000074F5 File Offset: 0x000056F5
		// (set) Token: 0x06000116 RID: 278 RVA: 0x000074FD File Offset: 0x000056FD
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

		// Token: 0x1700005F RID: 95
		// (get) Token: 0x06000117 RID: 279 RVA: 0x0000751B File Offset: 0x0000571B
		// (set) Token: 0x06000118 RID: 280 RVA: 0x00007523 File Offset: 0x00005723
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

		// Token: 0x17000060 RID: 96
		// (get) Token: 0x06000119 RID: 281 RVA: 0x00007541 File Offset: 0x00005741
		// (set) Token: 0x0600011A RID: 282 RVA: 0x00007549 File Offset: 0x00005749
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

		// Token: 0x17000061 RID: 97
		// (get) Token: 0x0600011B RID: 283 RVA: 0x00007567 File Offset: 0x00005767
		// (set) Token: 0x0600011C RID: 284 RVA: 0x0000756F File Offset: 0x0000576F
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

		// Token: 0x17000062 RID: 98
		// (get) Token: 0x0600011D RID: 285 RVA: 0x0000758D File Offset: 0x0000578D
		// (set) Token: 0x0600011E RID: 286 RVA: 0x00007595 File Offset: 0x00005795
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

		// Token: 0x04000044 RID: 68
		private readonly List<TournamentRoundVM> _rounds;

		// Token: 0x04000045 RID: 69
		private int _thisRoundBettedAmount;

		// Token: 0x04000046 RID: 70
		private bool _isPlayerParticipating;

		// Token: 0x04000047 RID: 71
		private InputKeyItemVM _doneInputKey;

		// Token: 0x04000048 RID: 72
		private InputKeyItemVM _cancelInputKey;

		// Token: 0x04000049 RID: 73
		private TournamentRoundVM _round1;

		// Token: 0x0400004A RID: 74
		private TournamentRoundVM _round2;

		// Token: 0x0400004B RID: 75
		private TournamentRoundVM _round3;

		// Token: 0x0400004C RID: 76
		private TournamentRoundVM _round4;

		// Token: 0x0400004D RID: 77
		private int _activeRoundIndex = -1;

		// Token: 0x0400004E RID: 78
		private string _joinTournamentText;

		// Token: 0x0400004F RID: 79
		private string _skipRoundText;

		// Token: 0x04000050 RID: 80
		private string _watchRoundText;

		// Token: 0x04000051 RID: 81
		private string _leaveText;

		// Token: 0x04000052 RID: 82
		private bool _canPlayerJoin;

		// Token: 0x04000053 RID: 83
		private TournamentMatchVM _currentMatch;

		// Token: 0x04000054 RID: 84
		private bool _isCurrentMatchActive;

		// Token: 0x04000055 RID: 85
		private string _betTitleText;

		// Token: 0x04000056 RID: 86
		private string _betDescriptionText;

		// Token: 0x04000057 RID: 87
		private string _betOddsText;

		// Token: 0x04000058 RID: 88
		private string _bettedDenarsText;

		// Token: 0x04000059 RID: 89
		private string _overallExpectedDenarsText;

		// Token: 0x0400005A RID: 90
		private string _currentExpectedDenarsText;

		// Token: 0x0400005B RID: 91
		private string _totalDenarsText;

		// Token: 0x0400005C RID: 92
		private string _acceptText;

		// Token: 0x0400005D RID: 93
		private string _cancelText;

		// Token: 0x0400005E RID: 94
		private string _prizeItemName;

		// Token: 0x0400005F RID: 95
		private string _tournamentPrizeText;

		// Token: 0x04000060 RID: 96
		private string _currentWagerText;

		// Token: 0x04000061 RID: 97
		private int _wageredDenars = -1;

		// Token: 0x04000062 RID: 98
		private int _expectedBetDenars = -1;

		// Token: 0x04000063 RID: 99
		private string _betText;

		// Token: 0x04000064 RID: 100
		private int _maximumBetValue;

		// Token: 0x04000065 RID: 101
		private string _tournamentWinnerTitle;

		// Token: 0x04000066 RID: 102
		private TournamentParticipantVM _tournamentWinner;

		// Token: 0x04000067 RID: 103
		private string _tournamentTitle;

		// Token: 0x04000068 RID: 104
		private bool _isOver;

		// Token: 0x04000069 RID: 105
		private bool _hasPrizeItem;

		// Token: 0x0400006A RID: 106
		private bool _isWinnerHero;

		// Token: 0x0400006B RID: 107
		private bool _isBetWindowEnabled;

		// Token: 0x0400006C RID: 108
		private string _winnerIntro;

		// Token: 0x0400006D RID: 109
		private ImageIdentifierVM _prizeVisual;

		// Token: 0x0400006E RID: 110
		private ImageIdentifierVM _winnerBanner;

		// Token: 0x0400006F RID: 111
		private MBBindingList<TournamentRewardVM> _battleRewards;

		// Token: 0x04000070 RID: 112
		private HintViewModel _skipAllRoundsHint;
	}
}
