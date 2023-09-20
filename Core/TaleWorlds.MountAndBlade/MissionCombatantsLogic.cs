using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
	public class MissionCombatantsLogic : MissionLogic
	{
		public BattleSideEnum PlayerSide
		{
			get
			{
				if (this._playerBattleCombatant == null)
				{
					return BattleSideEnum.None;
				}
				if (this._playerBattleCombatant != this._defenderLeaderBattleCombatant)
				{
					return BattleSideEnum.Attacker;
				}
				return BattleSideEnum.Defender;
			}
		}

		public MissionCombatantsLogic(IEnumerable<IBattleCombatant> battleCombatants, IBattleCombatant playerBattleCombatant, IBattleCombatant defenderLeaderBattleCombatant, IBattleCombatant attackerLeaderBattleCombatant, Mission.MissionTeamAITypeEnum teamAIType, bool isPlayerSergeant)
		{
			if (battleCombatants == null)
			{
				battleCombatants = new IBattleCombatant[] { defenderLeaderBattleCombatant, attackerLeaderBattleCombatant };
			}
			this._battleCombatants = battleCombatants;
			this._playerBattleCombatant = playerBattleCombatant;
			this._defenderLeaderBattleCombatant = defenderLeaderBattleCombatant;
			this._attackerLeaderBattleCombatant = attackerLeaderBattleCombatant;
			this._teamAIType = teamAIType;
			this._isPlayerSergeant = isPlayerSergeant;
		}

		public Banner GetBannerForSide(BattleSideEnum side)
		{
			if (side != BattleSideEnum.Defender)
			{
				return this._attackerLeaderBattleCombatant.Banner;
			}
			return this._defenderLeaderBattleCombatant.Banner;
		}

		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			if (!base.Mission.Teams.IsEmpty<Team>())
			{
				throw new MBIllegalValueException("Number of teams is not 0.");
			}
			BattleSideEnum side = this._playerBattleCombatant.Side;
			BattleSideEnum oppositeSide = side.GetOppositeSide();
			if (side == BattleSideEnum.Defender)
			{
				this.AddPlayerTeam(side);
			}
			else
			{
				this.AddEnemyTeam(oppositeSide);
			}
			if (side == BattleSideEnum.Attacker)
			{
				this.AddPlayerTeam(side);
			}
			else
			{
				this.AddEnemyTeam(oppositeSide);
			}
			this.AddPlayerAllyTeam(side);
		}

		public override void EarlyStart()
		{
			Mission.Current.MissionTeamAIType = this._teamAIType;
			switch (this._teamAIType)
			{
			case Mission.MissionTeamAITypeEnum.FieldBattle:
			{
				using (List<Team>.Enumerator enumerator = Mission.Current.Teams.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Team team8 = enumerator.Current;
						team8.AddTeamAI(new TeamAIGeneral(base.Mission, team8, 10f, 1f), false);
					}
					goto IL_182;
				}
				break;
			}
			case Mission.MissionTeamAITypeEnum.Siege:
				break;
			case Mission.MissionTeamAITypeEnum.SallyOut:
				goto IL_104;
			default:
				goto IL_182;
			}
			using (List<Team>.Enumerator enumerator = Mission.Current.Teams.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Team team2 = enumerator.Current;
					if (team2.Side == BattleSideEnum.Attacker)
					{
						team2.AddTeamAI(new TeamAISiegeAttacker(base.Mission, team2, 5f, 1f), false);
					}
					if (team2.Side == BattleSideEnum.Defender)
					{
						team2.AddTeamAI(new TeamAISiegeDefender(base.Mission, team2, 5f, 1f), false);
					}
				}
				goto IL_182;
			}
			IL_104:
			foreach (Team team3 in Mission.Current.Teams)
			{
				if (team3.Side == BattleSideEnum.Attacker)
				{
					team3.AddTeamAI(new TeamAISallyOutDefender(base.Mission, team3, 5f, 1f), false);
				}
				else
				{
					team3.AddTeamAI(new TeamAISallyOutAttacker(base.Mission, team3, 5f, 1f), false);
				}
			}
			IL_182:
			if (Mission.Current.Teams.Count > 0)
			{
				switch (Mission.Current.MissionTeamAIType)
				{
				case Mission.MissionTeamAITypeEnum.NoTeamAI:
				{
					using (List<Team>.Enumerator enumerator = Mission.Current.Teams.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							Team team4 = enumerator.Current;
							if (team4.HasTeamAi)
							{
								team4.AddTacticOption(new TacticCharge(team4));
							}
						}
						goto IL_4AD;
					}
					break;
				}
				case Mission.MissionTeamAITypeEnum.FieldBattle:
					break;
				case Mission.MissionTeamAITypeEnum.Siege:
					goto IL_3C4;
				case Mission.MissionTeamAITypeEnum.SallyOut:
					goto IL_433;
				default:
					goto IL_4AD;
				}
				using (List<Team>.Enumerator enumerator = Mission.Current.Teams.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Team team = enumerator.Current;
						if (team.HasTeamAi)
						{
							int num = this._battleCombatants.Where((IBattleCombatant bc) => bc.Side == team.Side).Max((IBattleCombatant bcs) => bcs.GetTacticsSkillAmount());
							team.AddTacticOption(new TacticCharge(team));
							if ((float)num >= 20f)
							{
								team.AddTacticOption(new TacticFullScaleAttack(team));
								if (team.Side == BattleSideEnum.Defender)
								{
									team.AddTacticOption(new TacticDefensiveEngagement(team));
									team.AddTacticOption(new TacticDefensiveLine(team));
								}
								if (team.Side == BattleSideEnum.Attacker)
								{
									team.AddTacticOption(new TacticRangedHarrassmentOffensive(team));
								}
							}
							if ((float)num >= 50f)
							{
								team.AddTacticOption(new TacticFrontalCavalryCharge(team));
								if (team.Side == BattleSideEnum.Defender)
								{
									team.AddTacticOption(new TacticDefensiveRing(team));
									team.AddTacticOption(new TacticHoldChokePoint(team));
								}
								if (team.Side == BattleSideEnum.Attacker)
								{
									team.AddTacticOption(new TacticCoordinatedRetreat(team));
								}
							}
						}
					}
					goto IL_4AD;
				}
				IL_3C4:
				using (List<Team>.Enumerator enumerator = Mission.Current.Teams.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Team team5 = enumerator.Current;
						if (team5.HasTeamAi)
						{
							if (team5.Side == BattleSideEnum.Attacker)
							{
								team5.AddTacticOption(new TacticBreachWalls(team5));
							}
							if (team5.Side == BattleSideEnum.Defender)
							{
								team5.AddTacticOption(new TacticDefendCastle(team5));
							}
						}
					}
					goto IL_4AD;
				}
				IL_433:
				foreach (Team team6 in Mission.Current.Teams)
				{
					if (team6.HasTeamAi)
					{
						if (team6.Side == BattleSideEnum.Defender)
						{
							team6.AddTacticOption(new TacticSallyOutHitAndRun(team6));
						}
						if (team6.Side == BattleSideEnum.Attacker)
						{
							team6.AddTacticOption(new TacticSallyOutDefense(team6));
						}
						team6.AddTacticOption(new TacticCharge(team6));
					}
				}
				IL_4AD:
				foreach (Team team7 in base.Mission.Teams)
				{
					team7.QuerySystem.Expire();
					team7.ResetTactic();
				}
			}
		}

		public override void AfterStart()
		{
			base.Mission.SetMissionMode(MissionMode.Battle, true);
		}

		public IEnumerable<IBattleCombatant> GetAllCombatants()
		{
			foreach (IBattleCombatant battleCombatant in this._battleCombatants)
			{
				yield return battleCombatant;
			}
			IEnumerator<IBattleCombatant> enumerator = null;
			yield break;
			yield break;
		}

		private void AddPlayerTeam(BattleSideEnum playerSide)
		{
			base.Mission.Teams.Add(playerSide, this._playerBattleCombatant.PrimaryColorPair.Item1, this._playerBattleCombatant.PrimaryColorPair.Item2, this._playerBattleCombatant.Banner, true, false, true);
			base.Mission.PlayerTeam = ((playerSide == BattleSideEnum.Attacker) ? base.Mission.AttackerTeam : base.Mission.DefenderTeam);
		}

		private void AddEnemyTeam(BattleSideEnum enemySide)
		{
			IBattleCombatant battleCombatant = ((enemySide == BattleSideEnum.Attacker) ? this._attackerLeaderBattleCombatant : this._defenderLeaderBattleCombatant);
			base.Mission.Teams.Add(enemySide, battleCombatant.PrimaryColorPair.Item1, battleCombatant.PrimaryColorPair.Item2, battleCombatant.Banner, true, false, true);
		}

		private void AddPlayerAllyTeam(BattleSideEnum playerSide)
		{
			if (this._battleCombatants != null)
			{
				foreach (IBattleCombatant battleCombatant in this._battleCombatants)
				{
					if (battleCombatant != this._playerBattleCombatant && battleCombatant.Side == playerSide && !this._isPlayerSergeant)
					{
						base.Mission.Teams.Add(playerSide, battleCombatant.PrimaryColorPair.Item1, battleCombatant.PrimaryColorPair.Item2, battleCombatant.Banner, true, false, true);
						if (playerSide != BattleSideEnum.Attacker)
						{
							Team defenderAllyTeam = base.Mission.DefenderAllyTeam;
							break;
						}
						Team attackerAllyTeam = base.Mission.AttackerAllyTeam;
						break;
					}
				}
			}
		}

		private readonly IEnumerable<IBattleCombatant> _battleCombatants;

		private readonly IBattleCombatant _playerBattleCombatant;

		private readonly IBattleCombatant _defenderLeaderBattleCombatant;

		private readonly IBattleCombatant _attackerLeaderBattleCombatant;

		private readonly Mission.MissionTeamAITypeEnum _teamAIType;

		private readonly bool _isPlayerSergeant;
	}
}
