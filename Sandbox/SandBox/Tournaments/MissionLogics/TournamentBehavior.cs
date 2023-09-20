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
	// Token: 0x0200001B RID: 27
	public class TournamentBehavior : MissionLogic, ICameraModeLogic
	{
		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000102 RID: 258 RVA: 0x00007DAB File Offset: 0x00005FAB
		public TournamentGame TournamentGame
		{
			get
			{
				return this._tournamentGame;
			}
		}

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000103 RID: 259 RVA: 0x00007DB3 File Offset: 0x00005FB3
		// (set) Token: 0x06000104 RID: 260 RVA: 0x00007DBB File Offset: 0x00005FBB
		public TournamentRound[] Rounds { get; private set; }

		// Token: 0x06000105 RID: 261 RVA: 0x00007DC4 File Offset: 0x00005FC4
		public SpectatorCameraTypes GetMissionCameraLockMode(bool lockedToMainPlayer)
		{
			if (!this.IsPlayerParticipating)
			{
				return 2;
			}
			return -1;
		}

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000106 RID: 262 RVA: 0x00007DD1 File Offset: 0x00005FD1
		// (set) Token: 0x06000107 RID: 263 RVA: 0x00007DD9 File Offset: 0x00005FD9
		public bool IsPlayerEliminated { get; private set; }

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000108 RID: 264 RVA: 0x00007DE2 File Offset: 0x00005FE2
		// (set) Token: 0x06000109 RID: 265 RVA: 0x00007DEA File Offset: 0x00005FEA
		public int CurrentRoundIndex { get; private set; }

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x0600010A RID: 266 RVA: 0x00007DF3 File Offset: 0x00005FF3
		// (set) Token: 0x0600010B RID: 267 RVA: 0x00007DFB File Offset: 0x00005FFB
		public TournamentMatch LastMatch { get; private set; }

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x0600010C RID: 268 RVA: 0x00007E04 File Offset: 0x00006004
		public TournamentRound CurrentRound
		{
			get
			{
				return this.Rounds[this.CurrentRoundIndex];
			}
		}

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x0600010D RID: 269 RVA: 0x00007E13 File Offset: 0x00006013
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

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x0600010E RID: 270 RVA: 0x00007E2F File Offset: 0x0000602F
		public TournamentMatch CurrentMatch
		{
			get
			{
				return this.CurrentRound.CurrentMatch;
			}
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x0600010F RID: 271 RVA: 0x00007E3C File Offset: 0x0000603C
		// (set) Token: 0x06000110 RID: 272 RVA: 0x00007E44 File Offset: 0x00006044
		public TournamentParticipant Winner { get; private set; }

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x06000111 RID: 273 RVA: 0x00007E4D File Offset: 0x0000604D
		// (set) Token: 0x06000112 RID: 274 RVA: 0x00007E55 File Offset: 0x00006055
		public bool IsPlayerParticipating { get; private set; }

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x06000113 RID: 275 RVA: 0x00007E5E File Offset: 0x0000605E
		// (set) Token: 0x06000114 RID: 276 RVA: 0x00007E66 File Offset: 0x00006066
		public Settlement Settlement { get; private set; }

		// Token: 0x06000115 RID: 277 RVA: 0x00007E70 File Offset: 0x00006070
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

		// Token: 0x06000116 RID: 278 RVA: 0x00007EC9 File Offset: 0x000060C9
		public MBList<CharacterObject> GetAllPossibleParticipants()
		{
			return this._tournamentGame.GetParticipantCharacters(this.Settlement, true);
		}

		// Token: 0x06000117 RID: 279 RVA: 0x00007EE0 File Offset: 0x000060E0
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

		// Token: 0x06000118 RID: 280 RVA: 0x00007F58 File Offset: 0x00006158
		public static void DeleteTournamentSetsExcept(GameEntity selectedSetEntity)
		{
			List<GameEntity> list = Mission.Current.Scene.FindEntitiesWithTag("arena_set").ToList<GameEntity>();
			list.Remove(selectedSetEntity);
			foreach (GameEntity gameEntity in list)
			{
				gameEntity.Remove(93);
			}
		}

		// Token: 0x06000119 RID: 281 RVA: 0x00007FC8 File Offset: 0x000061C8
		public static void DeleteAllTournamentSets()
		{
			foreach (GameEntity gameEntity in Mission.Current.Scene.FindEntitiesWithTag("arena_set").ToList<GameEntity>())
			{
				gameEntity.Remove(94);
			}
		}

		// Token: 0x0600011A RID: 282 RVA: 0x00008030 File Offset: 0x00006230
		public override void AfterStart()
		{
			this.CurrentRoundIndex = 0;
			this.CreateTournamentTree();
			this.FillParticipants(this._participants.ToList<TournamentParticipant>());
			this.CalculateBet();
		}

		// Token: 0x0600011B RID: 283 RVA: 0x00008056 File Offset: 0x00006256
		public override void OnMissionTick(float dt)
		{
			if (this.CurrentMatch != null && this.CurrentMatch.State == 1 && this._gameBehavior.IsMatchEnded())
			{
				this.EndCurrentMatch(false);
			}
		}

		// Token: 0x0600011C RID: 284 RVA: 0x00008084 File Offset: 0x00006284
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

		// Token: 0x0600011D RID: 285 RVA: 0x000080FF File Offset: 0x000062FF
		public void SkipMatch(bool isLeave = false)
		{
			this.CurrentMatch.Start();
			this._gameBehavior.SkipMatch(this.CurrentMatch);
			this.EndCurrentMatch(isLeave);
		}

		// Token: 0x0600011E RID: 286 RVA: 0x00008124 File Offset: 0x00006324
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

		// Token: 0x0600011F RID: 287 RVA: 0x00008380 File Offset: 0x00006580
		public void EndTournamentViaLeave()
		{
			while (this.CurrentMatch != null)
			{
				this.SkipMatch(true);
			}
		}

		// Token: 0x06000120 RID: 288 RVA: 0x00008394 File Offset: 0x00006594
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

		// Token: 0x06000121 RID: 289 RVA: 0x000083F0 File Offset: 0x000065F0
		private void OnPlayerWinMatch()
		{
			Campaign.Current.TournamentManager.OnPlayerWinMatch(this._tournamentGame.GetType());
		}

		// Token: 0x06000122 RID: 290 RVA: 0x0000840C File Offset: 0x0000660C
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

		// Token: 0x06000123 RID: 291 RVA: 0x00008494 File Offset: 0x00006694
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

		// Token: 0x06000124 RID: 292 RVA: 0x00008568 File Offset: 0x00006768
		private void FillParticipants(List<TournamentParticipant> participants)
		{
			foreach (TournamentParticipant tournamentParticipant in participants)
			{
				this.Rounds[this.CurrentRoundIndex].AddParticipant(tournamentParticipant, true);
			}
		}

		// Token: 0x06000125 RID: 293 RVA: 0x000085C4 File Offset: 0x000067C4
		public override InquiryData OnEndMissionRequest(out bool canPlayerLeave)
		{
			InquiryData inquiryData = null;
			canPlayerLeave = false;
			return inquiryData;
		}

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x06000126 RID: 294 RVA: 0x000085CA File Offset: 0x000067CA
		// (set) Token: 0x06000127 RID: 295 RVA: 0x000085D2 File Offset: 0x000067D2
		public float BetOdd { get; private set; }

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x06000128 RID: 296 RVA: 0x000085DB File Offset: 0x000067DB
		public int MaximumBetInstance
		{
			get
			{
				return MathF.Min(150, this.PlayerDenars);
			}
		}

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x06000129 RID: 297 RVA: 0x000085ED File Offset: 0x000067ED
		// (set) Token: 0x0600012A RID: 298 RVA: 0x000085F5 File Offset: 0x000067F5
		public int BettedDenars { get; private set; }

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x0600012B RID: 299 RVA: 0x000085FE File Offset: 0x000067FE
		// (set) Token: 0x0600012C RID: 300 RVA: 0x00008606 File Offset: 0x00006806
		public int OverallExpectedDenars { get; private set; }

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x0600012D RID: 301 RVA: 0x0000860F File Offset: 0x0000680F
		public int PlayerDenars
		{
			get
			{
				return Hero.MainHero.Gold;
			}
		}

		// Token: 0x0600012E RID: 302 RVA: 0x0000861B File Offset: 0x0000681B
		public void PlaceABet(int bet)
		{
			this.BettedDenars += bet;
			this.OverallExpectedDenars += this.GetExpectedDenarsForBet(bet);
			GiveGoldAction.ApplyBetweenCharacters(Hero.MainHero, null, bet, true);
		}

		// Token: 0x0600012F RID: 303 RVA: 0x0000864C File Offset: 0x0000684C
		public int GetExpectedDenarsForBet(int bet)
		{
			return (int)(this.BetOdd * (float)bet);
		}

		// Token: 0x06000130 RID: 304 RVA: 0x00008658 File Offset: 0x00006858
		public int GetMaximumBet()
		{
			int num = 150;
			if (Hero.MainHero.GetPerkValue(DefaultPerks.Roguery.DeepPockets))
			{
				num *= (int)DefaultPerks.Roguery.DeepPockets.PrimaryBonus;
			}
			return num;
		}

		// Token: 0x06000131 RID: 305 RVA: 0x0000868C File Offset: 0x0000688C
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

		// Token: 0x14000002 RID: 2
		// (add) Token: 0x06000132 RID: 306 RVA: 0x00008AA8 File Offset: 0x00006CA8
		// (remove) Token: 0x06000133 RID: 307 RVA: 0x00008AE0 File Offset: 0x00006CE0
		public event Action TournamentEnd;

		// Token: 0x0400005D RID: 93
		public const int RoundCount = 4;

		// Token: 0x0400005E RID: 94
		public const int ParticipantCount = 16;

		// Token: 0x0400005F RID: 95
		public const float EndMatchTimerDuration = 6f;

		// Token: 0x04000060 RID: 96
		public const float CheerTimerDuration = 1f;

		// Token: 0x04000061 RID: 97
		private TournamentGame _tournamentGame;

		// Token: 0x04000062 RID: 98
		private ITournamentGameBehavior _gameBehavior;

		// Token: 0x04000064 RID: 100
		private TournamentParticipant[] _participants;

		// Token: 0x0400006C RID: 108
		private const int MaximumBet = 150;

		// Token: 0x0400006D RID: 109
		public const float MaximumOdd = 4f;
	}
}
