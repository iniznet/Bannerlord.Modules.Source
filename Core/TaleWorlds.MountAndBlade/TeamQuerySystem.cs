using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Missions.Handlers;

namespace TaleWorlds.MountAndBlade
{
	public class TeamQuerySystem
	{
		public int MemberCount
		{
			get
			{
				return this._memberCount.Value;
			}
		}

		public WorldPosition MedianPosition
		{
			get
			{
				return this._medianPosition.Value;
			}
		}

		public Vec2 AveragePosition
		{
			get
			{
				return this._averagePosition.Value;
			}
		}

		public Vec2 AverageEnemyPosition
		{
			get
			{
				return this._averageEnemyPosition.Value;
			}
		}

		public WorldPosition MedianTargetFormationPosition
		{
			get
			{
				return this._medianTargetFormationPosition.Value;
			}
		}

		public WorldPosition LeftFlankEdgePosition
		{
			get
			{
				return this._leftFlankEdgePosition.Value;
			}
		}

		public WorldPosition RightFlankEdgePosition
		{
			get
			{
				return this._rightFlankEdgePosition.Value;
			}
		}

		public float InfantryRatio
		{
			get
			{
				return this._infantryRatio.Value;
			}
		}

		public float RangedRatio
		{
			get
			{
				return this._rangedRatio.Value;
			}
		}

		public float CavalryRatio
		{
			get
			{
				return this._cavalryRatio.Value;
			}
		}

		public float RangedCavalryRatio
		{
			get
			{
				return this._rangedCavalryRatio.Value;
			}
		}

		public int AllyUnitCount
		{
			get
			{
				return this._allyMemberCount.Value;
			}
		}

		public int EnemyUnitCount
		{
			get
			{
				return this._enemyMemberCount.Value;
			}
		}

		public float AllyInfantryRatio
		{
			get
			{
				return this._allyInfantryRatio.Value;
			}
		}

		public float AllyRangedRatio
		{
			get
			{
				return this._allyRangedRatio.Value;
			}
		}

		public float AllyCavalryRatio
		{
			get
			{
				return this._allyCavalryRatio.Value;
			}
		}

		public float AllyRangedCavalryRatio
		{
			get
			{
				return this._allyRangedCavalryRatio.Value;
			}
		}

		public float EnemyInfantryRatio
		{
			get
			{
				return this._enemyInfantryRatio.Value;
			}
		}

		public float EnemyRangedRatio
		{
			get
			{
				return this._enemyRangedRatio.Value;
			}
		}

		public float EnemyCavalryRatio
		{
			get
			{
				return this._enemyCavalryRatio.Value;
			}
		}

		public float EnemyRangedCavalryRatio
		{
			get
			{
				return this._enemyRangedCavalryRatio.Value;
			}
		}

		public float RemainingPowerRatio
		{
			get
			{
				return this._remainingPowerRatio.Value;
			}
		}

		public float TeamPower
		{
			get
			{
				return this._teamPower.Value;
			}
		}

		public float TotalPowerRatio
		{
			get
			{
				return this._totalPowerRatio.Value;
			}
		}

		public float InsideWallsRatio
		{
			get
			{
				return this._insideWallsRatio.Value;
			}
		}

		public BattlePowerCalculationLogic BattlePowerLogic
		{
			get
			{
				if (this._battlePowerLogic == null)
				{
					this._battlePowerLogic = this._mission.GetMissionBehavior<BattlePowerCalculationLogic>();
				}
				return this._battlePowerLogic;
			}
		}

		public CasualtyHandler CasualtyHandler
		{
			get
			{
				if (this._casualtyHandler == null)
				{
					this._casualtyHandler = this._mission.GetMissionBehavior<CasualtyHandler>();
				}
				return this._casualtyHandler;
			}
		}

		public float MaxUnderRangedAttackRatio
		{
			get
			{
				return this._maxUnderRangedAttackRatio.Value;
			}
		}

		public int DeathCount { get; private set; }

		public int DeathByRangedCount { get; private set; }

		public int AllyRangedUnitCount
		{
			get
			{
				return (int)(this.AllyRangedRatio * (float)this.AllyUnitCount);
			}
		}

		public int AllCavalryUnitCount
		{
			get
			{
				return (int)(this.AllyCavalryRatio * (float)this.AllyUnitCount);
			}
		}

		public int EnemyRangedUnitCount
		{
			get
			{
				return (int)(this.EnemyRangedRatio * (float)this.EnemyUnitCount);
			}
		}

		public void Expire()
		{
			this._memberCount.Expire();
			this._medianPosition.Expire();
			this._averagePosition.Expire();
			this._averageEnemyPosition.Expire();
			this._medianTargetFormationPosition.Expire();
			this._leftFlankEdgePosition.Expire();
			this._rightFlankEdgePosition.Expire();
			this._infantryRatio.Expire();
			this._rangedRatio.Expire();
			this._cavalryRatio.Expire();
			this._rangedCavalryRatio.Expire();
			this._allyMemberCount.Expire();
			this._enemyMemberCount.Expire();
			this._allyInfantryRatio.Expire();
			this._allyRangedRatio.Expire();
			this._allyCavalryRatio.Expire();
			this._allyRangedCavalryRatio.Expire();
			this._enemyInfantryRatio.Expire();
			this._enemyRangedRatio.Expire();
			this._enemyCavalryRatio.Expire();
			this._enemyRangedCavalryRatio.Expire();
			this._remainingPowerRatio.Expire();
			this._teamPower.Expire();
			this._totalPowerRatio.Expire();
			this._insideWallsRatio.Expire();
			this._maxUnderRangedAttackRatio.Expire();
			foreach (Formation formation in this.Team.FormationsIncludingSpecialAndEmpty)
			{
				if (formation.CountOfUnits > 0)
				{
					formation.QuerySystem.Expire();
				}
			}
		}

		public void ExpireAfterUnitAddRemove()
		{
			this._memberCount.Expire();
			this._medianPosition.Expire();
			this._averagePosition.Expire();
			this._leftFlankEdgePosition.Expire();
			this._rightFlankEdgePosition.Expire();
			this._infantryRatio.Expire();
			this._rangedRatio.Expire();
			this._cavalryRatio.Expire();
			this._rangedCavalryRatio.Expire();
			this._allyMemberCount.Expire();
			this._allyInfantryRatio.Expire();
			this._allyRangedRatio.Expire();
			this._allyCavalryRatio.Expire();
			this._allyRangedCavalryRatio.Expire();
			this._remainingPowerRatio.Expire();
			this._teamPower.Expire();
			this._totalPowerRatio.Expire();
			this._insideWallsRatio.Expire();
			this._maxUnderRangedAttackRatio.Expire();
		}

		private void InitializeTelemetryScopeNames()
		{
		}

		public TeamQuerySystem(Team team)
		{
			TeamQuerySystem <>4__this = this;
			this.Team = team;
			this._mission = Mission.Current;
			this._memberCount = new QueryData<int>(delegate
			{
				int num = 0;
				foreach (Formation formation in <>4__this.Team.FormationsIncludingSpecialAndEmpty)
				{
					num += formation.CountOfUnits;
				}
				return num;
			}, 2f);
			this._allyMemberCount = new QueryData<int>(delegate
			{
				int num2 = 0;
				foreach (Team team2 in <>4__this._mission.Teams)
				{
					if (team2.IsFriendOf(<>4__this.Team))
					{
						num2 += team2.QuerySystem.MemberCount;
					}
				}
				return num2;
			}, 2f);
			this._enemyMemberCount = new QueryData<int>(delegate
			{
				int num3 = 0;
				foreach (Team team3 in <>4__this._mission.Teams)
				{
					if (team3.IsEnemyOf(<>4__this.Team))
					{
						num3 += team3.QuerySystem.MemberCount;
					}
				}
				return num3;
			}, 2f);
			this._averagePosition = new QueryData<Vec2>(new Func<Vec2>(team.GetAveragePosition), 5f);
			this._medianPosition = new QueryData<WorldPosition>(() => team.GetMedianPosition(<>4__this.AveragePosition), 5f);
			this._averageEnemyPosition = new QueryData<Vec2>(delegate
			{
				Vec2 averagePositionOfEnemies = team.GetAveragePositionOfEnemies();
				if (averagePositionOfEnemies.IsValid)
				{
					return averagePositionOfEnemies;
				}
				if (team.Side == BattleSideEnum.Attacker)
				{
					SiegeDeploymentHandler missionBehavior = <>4__this._mission.GetMissionBehavior<SiegeDeploymentHandler>();
					if (missionBehavior != null)
					{
						return missionBehavior.GetEstimatedAverageDefenderPosition();
					}
				}
				if (!<>4__this.AveragePosition.IsValid)
				{
					return team.GetAveragePosition();
				}
				return <>4__this.AveragePosition;
			}, 5f);
			this._medianTargetFormationPosition = new QueryData<WorldPosition>(delegate
			{
				float num4 = float.MaxValue;
				Formation formation2 = null;
				foreach (Team team4 in <>4__this._mission.Teams)
				{
					if (team4.IsEnemyOf(<>4__this.Team))
					{
						foreach (Formation formation3 in team4.FormationsIncludingSpecialAndEmpty)
						{
							if (formation3.CountOfUnits > 0)
							{
								float num5 = formation3.QuerySystem.MedianPosition.AsVec2.DistanceSquared(<>4__this.AverageEnemyPosition);
								if (num4 > num5)
								{
									num4 = num5;
									formation2 = formation3;
								}
							}
						}
					}
				}
				if (formation2 != null)
				{
					return formation2.QuerySystem.MedianPosition;
				}
				return <>4__this.MedianPosition;
			}, 1f);
			QueryData<WorldPosition>.SetupSyncGroup(new IQueryData[] { this._averageEnemyPosition, this._medianTargetFormationPosition });
			this._leftFlankEdgePosition = new QueryData<WorldPosition>(delegate
			{
				Vec2 vec = (<>4__this.MedianTargetFormationPosition.AsVec2 - <>4__this.AveragePosition).RightVec();
				vec.Normalize();
				WorldPosition medianTargetFormationPosition = <>4__this.MedianTargetFormationPosition;
				medianTargetFormationPosition.SetVec2(medianTargetFormationPosition.AsVec2 - vec * 50f);
				return medianTargetFormationPosition;
			}, 5f);
			this._rightFlankEdgePosition = new QueryData<WorldPosition>(delegate
			{
				Vec2 vec2 = (<>4__this.MedianTargetFormationPosition.AsVec2 - <>4__this.AveragePosition).RightVec();
				vec2.Normalize();
				WorldPosition medianTargetFormationPosition2 = <>4__this.MedianTargetFormationPosition;
				medianTargetFormationPosition2.SetVec2(medianTargetFormationPosition2.AsVec2 + vec2 * 50f);
				return medianTargetFormationPosition2;
			}, 5f);
			this._infantryRatio = new QueryData<float>(delegate
			{
				if (<>4__this.MemberCount != 0)
				{
					return (<>4__this.Team.FormationsIncludingSpecialAndEmpty.Sum(delegate(Formation f)
					{
						if (f.CountOfUnits <= 0)
						{
							return 0f;
						}
						return f.QuerySystem.InfantryUnitRatio * (float)f.CountOfUnits;
					}) + (float)team.Heroes.Count((Agent h) => QueryLibrary.IsInfantry(h))) / (float)<>4__this.MemberCount;
				}
				return 0f;
			}, 15f);
			this._rangedRatio = new QueryData<float>(delegate
			{
				if (<>4__this.MemberCount != 0)
				{
					return (<>4__this.Team.FormationsIncludingSpecialAndEmpty.Sum(delegate(Formation f)
					{
						if (f.CountOfUnits <= 0)
						{
							return 0f;
						}
						return f.QuerySystem.RangedUnitRatio * (float)f.CountOfUnits;
					}) + (float)team.Heroes.Count((Agent h) => QueryLibrary.IsRanged(h))) / (float)<>4__this.MemberCount;
				}
				return 0f;
			}, 15f);
			this._cavalryRatio = new QueryData<float>(delegate
			{
				if (<>4__this.MemberCount != 0)
				{
					return (<>4__this.Team.FormationsIncludingSpecialAndEmpty.Sum(delegate(Formation f)
					{
						if (f.CountOfUnits <= 0)
						{
							return 0f;
						}
						return f.QuerySystem.CavalryUnitRatio * (float)f.CountOfUnits;
					}) + (float)team.Heroes.Count((Agent h) => QueryLibrary.IsCavalry(h))) / (float)<>4__this.MemberCount;
				}
				return 0f;
			}, 15f);
			this._rangedCavalryRatio = new QueryData<float>(delegate
			{
				if (<>4__this.MemberCount != 0)
				{
					return (<>4__this.Team.FormationsIncludingSpecialAndEmpty.Sum(delegate(Formation f)
					{
						if (f.CountOfUnits <= 0)
						{
							return 0f;
						}
						return f.QuerySystem.RangedCavalryUnitRatio * (float)f.CountOfUnits;
					}) + (float)team.Heroes.Count((Agent h) => QueryLibrary.IsRangedCavalry(h))) / (float)<>4__this.MemberCount;
				}
				return 0f;
			}, 15f);
			QueryData<float>.SetupSyncGroup(new IQueryData[] { this._infantryRatio, this._rangedRatio, this._cavalryRatio, this._rangedCavalryRatio });
			this._allyInfantryRatio = new QueryData<float>(delegate
			{
				float num6 = 0f;
				int num7 = 0;
				foreach (Team team5 in <>4__this._mission.Teams)
				{
					if (team5.IsFriendOf(<>4__this.Team))
					{
						int memberCount = team5.QuerySystem.MemberCount;
						num6 += team5.QuerySystem.InfantryRatio * (float)memberCount;
						num7 += memberCount;
					}
				}
				if (num7 != 0)
				{
					return num6 / (float)num7;
				}
				return 0f;
			}, 15f);
			this._allyRangedRatio = new QueryData<float>(delegate
			{
				float num8 = 0f;
				int num9 = 0;
				foreach (Team team6 in <>4__this._mission.Teams)
				{
					if (team6.IsFriendOf(<>4__this.Team))
					{
						int memberCount2 = team6.QuerySystem.MemberCount;
						num8 += team6.QuerySystem.RangedRatio * (float)memberCount2;
						num9 += memberCount2;
					}
				}
				if (num9 != 0)
				{
					return num8 / (float)num9;
				}
				return 0f;
			}, 15f);
			this._allyCavalryRatio = new QueryData<float>(delegate
			{
				float num10 = 0f;
				int num11 = 0;
				foreach (Team team7 in <>4__this._mission.Teams)
				{
					if (team7.IsFriendOf(<>4__this.Team))
					{
						int memberCount3 = team7.QuerySystem.MemberCount;
						num10 += team7.QuerySystem.CavalryRatio * (float)memberCount3;
						num11 += memberCount3;
					}
				}
				if (num11 != 0)
				{
					return num10 / (float)num11;
				}
				return 0f;
			}, 15f);
			this._allyRangedCavalryRatio = new QueryData<float>(delegate
			{
				float num12 = 0f;
				int num13 = 0;
				foreach (Team team8 in <>4__this._mission.Teams)
				{
					if (team8.IsFriendOf(<>4__this.Team))
					{
						int memberCount4 = team8.QuerySystem.MemberCount;
						num12 += team8.QuerySystem.RangedCavalryRatio * (float)memberCount4;
						num13 += memberCount4;
					}
				}
				if (num13 != 0)
				{
					return num12 / (float)num13;
				}
				return 0f;
			}, 15f);
			QueryData<float>.SetupSyncGroup(new IQueryData[] { this._allyInfantryRatio, this._allyRangedRatio, this._allyCavalryRatio, this._allyRangedCavalryRatio });
			this._enemyInfantryRatio = new QueryData<float>(delegate
			{
				float num14 = 0f;
				int num15 = 0;
				foreach (Team team9 in <>4__this._mission.Teams)
				{
					if (team9.IsEnemyOf(<>4__this.Team))
					{
						int memberCount5 = team9.QuerySystem.MemberCount;
						num14 += team9.QuerySystem.InfantryRatio * (float)memberCount5;
						num15 += memberCount5;
					}
				}
				if (num15 != 0)
				{
					return num14 / (float)num15;
				}
				return 0f;
			}, 15f);
			this._enemyRangedRatio = new QueryData<float>(delegate
			{
				float num16 = 0f;
				int num17 = 0;
				foreach (Team team10 in <>4__this._mission.Teams)
				{
					if (team10.IsEnemyOf(<>4__this.Team))
					{
						int memberCount6 = team10.QuerySystem.MemberCount;
						num16 += team10.QuerySystem.RangedRatio * (float)memberCount6;
						num17 += memberCount6;
					}
				}
				if (num17 != 0)
				{
					return num16 / (float)num17;
				}
				return 0f;
			}, 15f);
			this._enemyCavalryRatio = new QueryData<float>(delegate
			{
				float num18 = 0f;
				int num19 = 0;
				foreach (Team team11 in <>4__this._mission.Teams)
				{
					if (team11.IsEnemyOf(<>4__this.Team))
					{
						int memberCount7 = team11.QuerySystem.MemberCount;
						num18 += team11.QuerySystem.CavalryRatio * (float)memberCount7;
						num19 += memberCount7;
					}
				}
				if (num19 != 0)
				{
					return num18 / (float)num19;
				}
				return 0f;
			}, 15f);
			this._enemyRangedCavalryRatio = new QueryData<float>(delegate
			{
				float num20 = 0f;
				int num21 = 0;
				foreach (Team team12 in <>4__this._mission.Teams)
				{
					if (team12.IsEnemyOf(<>4__this.Team))
					{
						int memberCount8 = team12.QuerySystem.MemberCount;
						num20 += team12.QuerySystem.RangedCavalryRatio * (float)memberCount8;
						num21 += memberCount8;
					}
				}
				if (num21 != 0)
				{
					return num20 / (float)num21;
				}
				return 0f;
			}, 15f);
			this._teamPower = new QueryData<float>(() => team.FormationsIncludingSpecialAndEmpty.Sum(delegate(Formation f)
			{
				if (f.CountOfUnits <= 0)
				{
					return 0f;
				}
				return f.GetFormationPower();
			}), 5f);
			this._remainingPowerRatio = new QueryData<float>(delegate
			{
				BattlePowerCalculationLogic battlePowerLogic = <>4__this.BattlePowerLogic;
				CasualtyHandler casualtyHandler = <>4__this.CasualtyHandler;
				float num22 = 0f;
				float num23 = 0f;
				foreach (Team team13 in <>4__this.Team.Mission.Teams)
				{
					if (team13.IsEnemyOf(<>4__this.Team))
					{
						num23 += battlePowerLogic.GetTotalTeamPower(team13);
						using (List<Formation>.Enumerator enumerator15 = team13.FormationsIncludingSpecialAndEmpty.GetEnumerator())
						{
							while (enumerator15.MoveNext())
							{
								Formation formation4 = enumerator15.Current;
								num23 -= casualtyHandler.GetCasualtyPowerLossOfFormation(formation4);
							}
							continue;
						}
					}
					num22 += battlePowerLogic.GetTotalTeamPower(team13);
					foreach (Formation formation5 in team13.FormationsIncludingSpecialAndEmpty)
					{
						num22 -= casualtyHandler.GetCasualtyPowerLossOfFormation(formation5);
					}
				}
				num22 = MathF.Max(0f, num22);
				num23 = MathF.Max(0f, num23);
				return (num22 + 1f) / (num23 + 1f);
			}, 5f);
			this._totalPowerRatio = new QueryData<float>(delegate
			{
				BattlePowerCalculationLogic battlePowerLogic2 = <>4__this.BattlePowerLogic;
				float num24 = 0f;
				float num25 = 0f;
				foreach (Team team14 in <>4__this.Team.Mission.Teams)
				{
					if (team14.IsEnemyOf(<>4__this.Team))
					{
						num25 += battlePowerLogic2.GetTotalTeamPower(team14);
					}
					else
					{
						num24 += battlePowerLogic2.GetTotalTeamPower(team14);
					}
				}
				return (num24 + 1f) / (num25 + 1f);
			}, 10f);
			this._insideWallsRatio = new QueryData<float>(delegate
			{
				if (!(team.TeamAI is TeamAISiegeComponent))
				{
					return 1f;
				}
				if (<>4__this.AllyUnitCount == 0)
				{
					return 0f;
				}
				int num26 = 0;
				foreach (Team team15 in Mission.Current.Teams)
				{
					if (team15.IsFriendOf(team))
					{
						foreach (Formation formation6 in team15.FormationsIncludingSpecialAndEmpty)
						{
							if (formation6.CountOfUnits > 0)
							{
								num26 += formation6.CountUnitsOnNavMeshIDMod10(1, false);
							}
						}
					}
				}
				return (float)num26 / (float)<>4__this.AllyUnitCount;
			}, 10f);
			this._maxUnderRangedAttackRatio = new QueryData<float>(delegate
			{
				float num27;
				if (<>4__this.AllyUnitCount == 0)
				{
					num27 = 0f;
				}
				else
				{
					float currentTime = MBCommon.GetTotalMissionTime();
					int num28 = 0;
					Func<Agent, bool> <>9__34;
					foreach (Team team16 in <>4__this._mission.Teams)
					{
						if (team16.IsFriendOf(<>4__this.Team))
						{
							for (int i = 0; i < Math.Min(team16.FormationsIncludingSpecialAndEmpty.Count, 8); i++)
							{
								Formation formation7 = team16.FormationsIncludingSpecialAndEmpty[i];
								if (formation7.CountOfUnits > 0)
								{
									int num29 = num28;
									Formation formation8 = formation7;
									Func<Agent, bool> func;
									if ((func = <>9__34) == null)
									{
										func = (<>9__34 = (Agent agent) => currentTime - agent.LastRangedHitTime < 10f && !agent.Equipment.HasShield());
									}
									num28 = num29 + formation8.GetCountOfUnitsWithCondition(func);
								}
							}
						}
					}
					num27 = (float)num28 / (float)<>4__this.AllyUnitCount;
				}
				if (num27 <= <>4__this._maxUnderRangedAttackRatio.GetCachedValue())
				{
					return <>4__this._maxUnderRangedAttackRatio.GetCachedValue();
				}
				return num27;
			}, 3f);
			this.DeathCount = 0;
			this.DeathByRangedCount = 0;
			this.InitializeTelemetryScopeNames();
		}

		public void RegisterDeath()
		{
			int deathCount = this.DeathCount;
			this.DeathCount = deathCount + 1;
		}

		public void RegisterDeathByRanged()
		{
			int deathByRangedCount = this.DeathByRangedCount;
			this.DeathByRangedCount = deathByRangedCount + 1;
		}

		public float GetLocalAllyPower(Vec2 target)
		{
			return this.Team.FormationsIncludingSpecialAndEmpty.Sum(delegate(Formation f)
			{
				if (f.CountOfUnits <= 0)
				{
					return 0f;
				}
				return f.QuerySystem.FormationPower / f.QuerySystem.AveragePosition.Distance(target);
			});
		}

		public float GetLocalEnemyPower(Vec2 target)
		{
			float num = 0f;
			foreach (Team team in Mission.Current.Teams)
			{
				if (this.Team.IsEnemyOf(team))
				{
					foreach (Formation formation in team.FormationsIncludingSpecialAndEmpty)
					{
						if (formation.CountOfUnits > 0)
						{
							num += formation.QuerySystem.FormationPower / formation.QuerySystem.AveragePosition.Distance(target);
						}
					}
				}
			}
			return num;
		}

		public readonly Team Team;

		private readonly Mission _mission;

		private readonly QueryData<int> _memberCount;

		private readonly QueryData<WorldPosition> _medianPosition;

		private readonly QueryData<Vec2> _averagePosition;

		private readonly QueryData<Vec2> _averageEnemyPosition;

		private readonly QueryData<WorldPosition> _medianTargetFormationPosition;

		private readonly QueryData<WorldPosition> _leftFlankEdgePosition;

		private readonly QueryData<WorldPosition> _rightFlankEdgePosition;

		private readonly QueryData<float> _infantryRatio;

		private readonly QueryData<float> _rangedRatio;

		private readonly QueryData<float> _cavalryRatio;

		private readonly QueryData<float> _rangedCavalryRatio;

		private readonly QueryData<int> _allyMemberCount;

		private readonly QueryData<int> _enemyMemberCount;

		private readonly QueryData<float> _allyInfantryRatio;

		private readonly QueryData<float> _allyRangedRatio;

		private readonly QueryData<float> _allyCavalryRatio;

		private readonly QueryData<float> _allyRangedCavalryRatio;

		private readonly QueryData<float> _enemyInfantryRatio;

		private readonly QueryData<float> _enemyRangedRatio;

		private readonly QueryData<float> _enemyCavalryRatio;

		private readonly QueryData<float> _enemyRangedCavalryRatio;

		private readonly QueryData<float> _remainingPowerRatio;

		private readonly QueryData<float> _teamPower;

		private readonly QueryData<float> _totalPowerRatio;

		private readonly QueryData<float> _insideWallsRatio;

		private BattlePowerCalculationLogic _battlePowerLogic;

		private CasualtyHandler _casualtyHandler;

		private readonly QueryData<float> _maxUnderRangedAttackRatio;
	}
}
