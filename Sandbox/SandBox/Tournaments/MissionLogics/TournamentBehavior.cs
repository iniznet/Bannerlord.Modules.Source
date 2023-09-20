using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.TournamentGames;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace SandBox.Tournaments.MissionLogics
{
	public class TournamentBehavior : MissionLogic, ICameraModeLogic
	{
		public TournamentGame TournamentGame
		{
			get
			{
				return this._tournamentGame;
			}
		}

		public TournamentRound[] Rounds { get; private set; }

		public SpectatorCameraTypes GetMissionCameraLockMode(bool lockedToMainPlayer)
		{
			if (!this.IsPlayerParticipating)
			{
				return 2;
			}
			return -1;
		}

		public bool IsPlayerEliminated { get; private set; }

		public int CurrentRoundIndex { get; private set; }

		public TournamentMatch LastMatch { get; private set; }

		public TournamentRound CurrentRound
		{
			get
			{
				return this.Rounds[this.CurrentRoundIndex];
			}
		}

		public TournamentRound NextRound
		{
			get
			{
				if (this.CurrentRoundIndex != 3)
				{
					return this.Rounds[this.CurrentRoundIndex + 1];
				}
				return null;
			}
		}

		public TournamentMatch CurrentMatch
		{
			get
			{
				return this.CurrentRound.CurrentMatch;
			}
		}

		public TournamentParticipant Winner { get; private set; }

		public bool IsPlayerParticipating { get; private set; }

		public Settlement Settlement { get; private set; }

		public TournamentBehavior(TournamentGame tournamentGame, Settlement settlement, ITournamentGameBehavior gameBehavior, bool isPlayerParticipating)
		{
			this.Settlement = settlement;
			this._tournamentGame = tournamentGame;
			this._gameBehavior = gameBehavior;
			this.Rounds = new TournamentRound[4];
			this.CreateParticipants(isPlayerParticipating);
			this.CurrentRoundIndex = -1;
			this.LastMatch = null;
			this.Winner = null;
			this.IsPlayerParticipating = isPlayerParticipating;
		}

		public MBList<CharacterObject> GetAllPossibleParticipants()
		{
			return this._tournamentGame.GetParticipantCharacters(this.Settlement, true);
		}

		private void CreateParticipants(bool includePlayer)
		{
			this._participants = new TournamentParticipant[this._tournamentGame.MaximumParticipantCount];
			MBList<CharacterObject> participantCharacters = this._tournamentGame.GetParticipantCharacters(this.Settlement, includePlayer);
			Extensions.Shuffle<CharacterObject>(participantCharacters);
			int num = 0;
			while (num < participantCharacters.Count && num < this._tournamentGame.MaximumParticipantCount)
			{
				this._participants[num] = new TournamentParticipant(participantCharacters[num], default(UniqueTroopDescriptor));
				num++;
			}
		}

		public static void DeleteTournamentSetsExcept(GameEntity selectedSetEntity)
		{
			List<GameEntity> list = Mission.Current.Scene.FindEntitiesWithTag("arena_set").ToList<GameEntity>();
			list.Remove(selectedSetEntity);
			foreach (GameEntity gameEntity in list)
			{
				gameEntity.Remove(93);
			}
		}

		public static void DeleteAllTournamentSets()
		{
			foreach (GameEntity gameEntity in Mission.Current.Scene.FindEntitiesWithTag("arena_set").ToList<GameEntity>())
			{
				gameEntity.Remove(94);
			}
		}

		public override void AfterStart()
		{
			this.CurrentRoundIndex = 0;
			this.CreateTournamentTree();
			this.FillParticipants(this._participants.ToList<TournamentParticipant>());
			this.CalculateBet();
		}

		public override void OnMissionTick(float dt)
		{
			if (this.CurrentMatch != null && this.CurrentMatch.State == 1 && this._gameBehavior.IsMatchEnded())
			{
				this.EndCurrentMatch(false);
			}
		}

		public void StartMatch()
		{
			if (this.CurrentMatch.IsPlayerParticipating())
			{
				Campaign.Current.TournamentManager.OnPlayerJoinMatch(this._tournamentGame.GetType());
			}
			this.CurrentMatch.Start();
			base.Mission.SetMissionMode(7, true);
			this._gameBehavior.StartMatch(this.CurrentMatch, this.NextRound == null);
			CampaignEventDispatcher.Instance.OnPlayerStartedTournamentMatch(this.Settlement.Town);
		}

		public void SkipMatch(bool isLeave = false)
		{
			this.CurrentMatch.Start();
			this._gameBehavior.SkipMatch(this.CurrentMatch);
			this.EndCurrentMatch(isLeave);
		}

		private void EndCurrentMatch(bool isLeave)
		{
			this.LastMatch = this.CurrentMatch;
			this.CurrentRound.EndMatch();
			this._gameBehavior.OnMatchEnded();
			if (this.LastMatch.IsPlayerParticipating())
			{
				if (this.LastMatch.Winners.All((TournamentParticipant x) => x.Character != CharacterObject.PlayerCharacter))
				{
					this.OnPlayerEliminated();
				}
				else
				{
					this.OnPlayerWinMatch();
				}
			}
			if (this.NextRound != null)
			{
				for (;;)
				{
					if (!this.LastMatch.Winners.Any((TournamentParticipant x) => !x.IsAssigned))
					{
						break;
					}
					foreach (TournamentParticipant tournamentParticipant in this.LastMatch.Winners)
					{
						if (!tournamentParticipant.IsAssigned)
						{
							this.NextRound.AddParticipant(tournamentParticipant, false);
							tournamentParticipant.IsAssigned = true;
						}
					}
				}
			}
			if (this.CurrentRound.CurrentMatch == null)
			{
				if (this.CurrentRoundIndex < 3)
				{
					int i = this.CurrentRoundIndex;
					this.CurrentRoundIndex = i + 1;
					this.CalculateBet();
					return;
				}
				this.CalculateBet();
				MBInformationManager.AddQuickInformation(new TextObject("{=tWzLqegB}Tournament is over.", null), 0, null, "");
				this.Winner = this.LastMatch.Winners.FirstOrDefault<TournamentParticipant>();
				if (this.Winner.Character.IsHero)
				{
					if (this.Winner.Character == CharacterObject.PlayerCharacter)
					{
						this.OnPlayerWinTournament();
					}
					Campaign.Current.TournamentManager.GivePrizeToWinner(this._tournamentGame, this.Winner.Character.HeroObject, true);
					Campaign.Current.TournamentManager.AddLeaderboardEntry(this.Winner.Character.HeroObject);
				}
				MBList<CharacterObject> mblist = new MBList<CharacterObject>(this._participants.Length);
				foreach (TournamentParticipant tournamentParticipant2 in this._participants)
				{
					mblist.Add(tournamentParticipant2.Character);
				}
				CampaignEventDispatcher.Instance.OnTournamentFinished(this.Winner.Character, mblist, this.Settlement.Town, this._tournamentGame.Prize);
				if (this.TournamentEnd != null && !isLeave)
				{
					this.TournamentEnd();
				}
			}
		}

		public void EndTournamentViaLeave()
		{
			while (this.CurrentMatch != null)
			{
				this.SkipMatch(true);
			}
		}

		private void OnPlayerEliminated()
		{
			this.IsPlayerEliminated = true;
			this.BetOdd = 0f;
			if (this.BettedDenars > 0)
			{
				GiveGoldAction.ApplyForCharacterToSettlement(null, Settlement.CurrentSettlement, this.BettedDenars, false);
			}
			this.OverallExpectedDenars = 0;
			CampaignEventDispatcher.Instance.OnPlayerEliminatedFromTournament(this.CurrentRoundIndex, this.Settlement.Town);
		}

		private void OnPlayerWinMatch()
		{
			Campaign.Current.TournamentManager.OnPlayerWinMatch(this._tournamentGame.GetType());
		}

		private void OnPlayerWinTournament()
		{
			if (Campaign.Current.GameMode != 1)
			{
				return;
			}
			if (Hero.MainHero.MapFaction.IsKingdomFaction && Hero.MainHero.MapFaction.Leader != Hero.MainHero)
			{
				GainKingdomInfluenceAction.ApplyForDefault(Hero.MainHero, 1f);
			}
			if (this.OverallExpectedDenars > 0)
			{
				GiveGoldAction.ApplyBetweenCharacters(null, Hero.MainHero, this.OverallExpectedDenars, false);
			}
			Campaign.Current.TournamentManager.OnPlayerWinTournament(this._tournamentGame.GetType());
		}

		private void CreateTournamentTree()
		{
			int num = 16;
			int num2 = (int)MathF.Log((float)this._tournamentGame.MaxTeamSize, 2f);
			for (int i = 0; i < 4; i++)
			{
				int num3 = (int)MathF.Log((float)num, 2f);
				int num4 = MBRandom.RandomInt(1, MathF.Min(MathF.Min(3, num3), this._tournamentGame.MaxTeamNumberPerMatch));
				int num5 = MathF.Min(num3 - num4, num2);
				int num6 = MathF.Ceiling(MathF.Log((float)(1 + MBRandom.RandomInt((int)MathF.Pow(2f, (float)num5))), 2f));
				int num7 = num3 - (num4 + num6);
				this.Rounds[i] = new TournamentRound(num, MathF.PowTwo32(num7), MathF.PowTwo32(num4), num / 2, this._tournamentGame.Mode);
				num /= 2;
			}
		}

		private void FillParticipants(List<TournamentParticipant> participants)
		{
			foreach (TournamentParticipant tournamentParticipant in participants)
			{
				this.Rounds[this.CurrentRoundIndex].AddParticipant(tournamentParticipant, true);
			}
		}

		public override InquiryData OnEndMissionRequest(out bool canPlayerLeave)
		{
			InquiryData inquiryData = null;
			canPlayerLeave = false;
			return inquiryData;
		}

		public float BetOdd { get; private set; }

		public int MaximumBetInstance
		{
			get
			{
				return MathF.Min(150, this.PlayerDenars);
			}
		}

		public int BettedDenars { get; private set; }

		public int OverallExpectedDenars { get; private set; }

		public int PlayerDenars
		{
			get
			{
				return Hero.MainHero.Gold;
			}
		}

		public void PlaceABet(int bet)
		{
			this.BettedDenars += bet;
			this.OverallExpectedDenars += this.GetExpectedDenarsForBet(bet);
			GiveGoldAction.ApplyBetweenCharacters(Hero.MainHero, null, bet, true);
		}

		public int GetExpectedDenarsForBet(int bet)
		{
			return (int)(this.BetOdd * (float)bet);
		}

		public int GetMaximumBet()
		{
			int num = 150;
			if (Hero.MainHero.GetPerkValue(DefaultPerks.Roguery.DeepPockets))
			{
				num *= (int)DefaultPerks.Roguery.DeepPockets.PrimaryBonus;
			}
			return num;
		}

		private void CalculateBet()
		{
			if (this.IsPlayerParticipating)
			{
				if (this.CurrentRound.CurrentMatch == null)
				{
					this.BetOdd = 0f;
					return;
				}
				if (this.IsPlayerEliminated || !this.IsPlayerParticipating)
				{
					this.OverallExpectedDenars = 0;
					this.BetOdd = 0f;
					return;
				}
				List<KeyValuePair<Hero, int>> leaderboard = Campaign.Current.TournamentManager.GetLeaderboard();
				int num = 0;
				int num2 = 0;
				for (int i = 0; i < leaderboard.Count; i++)
				{
					if (leaderboard[i].Key == Hero.MainHero)
					{
						num = leaderboard[i].Value;
					}
					if (leaderboard[i].Value > num2)
					{
						num2 = leaderboard[i].Value;
					}
				}
				float num3 = 30f + (float)Hero.MainHero.Level + (float)MathF.Max(0, num * 12 - num2 * 2);
				float num4 = 0f;
				float num5 = 0f;
				float num6 = 0f;
				foreach (TournamentMatch tournamentMatch in this.CurrentRound.Matches)
				{
					foreach (TournamentTeam tournamentTeam in tournamentMatch.Teams)
					{
						float num7 = 0f;
						foreach (TournamentParticipant tournamentParticipant in tournamentTeam.Participants)
						{
							if (tournamentParticipant.Character != CharacterObject.PlayerCharacter)
							{
								int num8 = 0;
								if (tournamentParticipant.Character.IsHero)
								{
									for (int k = 0; k < leaderboard.Count; k++)
									{
										if (leaderboard[k].Key == tournamentParticipant.Character.HeroObject)
										{
											num8 = leaderboard[k].Value;
										}
									}
								}
								num7 += (float)(tournamentParticipant.Character.Level + MathF.Max(0, num8 * 8 - num2 * 2));
							}
						}
						if (tournamentTeam.Participants.Any((TournamentParticipant x) => x.Character == CharacterObject.PlayerCharacter))
						{
							num5 = num7;
							foreach (TournamentTeam tournamentTeam2 in tournamentMatch.Teams)
							{
								if (tournamentTeam != tournamentTeam2)
								{
									foreach (TournamentParticipant tournamentParticipant2 in tournamentTeam2.Participants)
									{
										int num9 = 0;
										if (tournamentParticipant2.Character.IsHero)
										{
											for (int l = 0; l < leaderboard.Count; l++)
											{
												if (leaderboard[l].Key == tournamentParticipant2.Character.HeroObject)
												{
													num9 = leaderboard[l].Value;
												}
											}
										}
										num6 += (float)(tournamentParticipant2.Character.Level + MathF.Max(0, num9 * 8 - num2 * 2));
									}
								}
							}
						}
						num4 += num7;
					}
				}
				float num10 = (num5 + num3) / (num6 + num5 + num3);
				float num11 = num3 / (num5 + num3 + 0.5f * (num4 - (num5 + num6)));
				float num12 = num10 * num11;
				float num13 = MathF.Clamp(MathF.Pow(1f / num12, 0.75f), 1.1f, 4f);
				this.BetOdd = (float)((int)(num13 * 10f)) / 10f;
			}
		}

		public event Action TournamentEnd;

		public const int RoundCount = 4;

		public const int ParticipantCount = 16;

		public const float EndMatchTimerDuration = 6f;

		public const float CheerTimerDuration = 1f;

		private TournamentGame _tournamentGame;

		private ITournamentGameBehavior _gameBehavior;

		private TournamentParticipant[] _participants;

		private const int MaximumBet = 150;

		public const float MaximumOdd = 4f;
	}
}
