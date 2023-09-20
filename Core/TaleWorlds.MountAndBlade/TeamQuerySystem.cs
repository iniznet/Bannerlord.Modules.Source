using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Missions.Handlers;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200016B RID: 363
	public class TeamQuerySystem
	{
		// Token: 0x170003F6 RID: 1014
		// (get) Token: 0x0600128C RID: 4748 RVA: 0x00048189 File Offset: 0x00046389
		public int MemberCount
		{
			get
			{
				return this._memberCount.Value;
			}
		}

		// Token: 0x170003F7 RID: 1015
		// (get) Token: 0x0600128D RID: 4749 RVA: 0x00048196 File Offset: 0x00046396
		public WorldPosition MedianPosition
		{
			get
			{
				return this._medianPosition.Value;
			}
		}

		// Token: 0x170003F8 RID: 1016
		// (get) Token: 0x0600128E RID: 4750 RVA: 0x000481A3 File Offset: 0x000463A3
		public Vec2 AveragePosition
		{
			get
			{
				return this._averagePosition.Value;
			}
		}

		// Token: 0x170003F9 RID: 1017
		// (get) Token: 0x0600128F RID: 4751 RVA: 0x000481B0 File Offset: 0x000463B0
		public Vec2 AverageEnemyPosition
		{
			get
			{
				return this._averageEnemyPosition.Value;
			}
		}

		// Token: 0x170003FA RID: 1018
		// (get) Token: 0x06001290 RID: 4752 RVA: 0x000481BD File Offset: 0x000463BD
		public WorldPosition MedianTargetFormationPosition
		{
			get
			{
				return this._medianTargetFormationPosition.Value;
			}
		}

		// Token: 0x170003FB RID: 1019
		// (get) Token: 0x06001291 RID: 4753 RVA: 0x000481CA File Offset: 0x000463CA
		public WorldPosition LeftFlankEdgePosition
		{
			get
			{
				return this._leftFlankEdgePosition.Value;
			}
		}

		// Token: 0x170003FC RID: 1020
		// (get) Token: 0x06001292 RID: 4754 RVA: 0x000481D7 File Offset: 0x000463D7
		public WorldPosition RightFlankEdgePosition
		{
			get
			{
				return this._rightFlankEdgePosition.Value;
			}
		}

		// Token: 0x170003FD RID: 1021
		// (get) Token: 0x06001293 RID: 4755 RVA: 0x000481E4 File Offset: 0x000463E4
		public float InfantryRatio
		{
			get
			{
				return this._infantryRatio.Value;
			}
		}

		// Token: 0x170003FE RID: 1022
		// (get) Token: 0x06001294 RID: 4756 RVA: 0x000481F1 File Offset: 0x000463F1
		public float RangedRatio
		{
			get
			{
				return this._rangedRatio.Value;
			}
		}

		// Token: 0x170003FF RID: 1023
		// (get) Token: 0x06001295 RID: 4757 RVA: 0x000481FE File Offset: 0x000463FE
		public float CavalryRatio
		{
			get
			{
				return this._cavalryRatio.Value;
			}
		}

		// Token: 0x17000400 RID: 1024
		// (get) Token: 0x06001296 RID: 4758 RVA: 0x0004820B File Offset: 0x0004640B
		public float RangedCavalryRatio
		{
			get
			{
				return this._rangedCavalryRatio.Value;
			}
		}

		// Token: 0x17000401 RID: 1025
		// (get) Token: 0x06001297 RID: 4759 RVA: 0x00048218 File Offset: 0x00046418
		public int AllyUnitCount
		{
			get
			{
				return this._allyMemberCount.Value;
			}
		}

		// Token: 0x17000402 RID: 1026
		// (get) Token: 0x06001298 RID: 4760 RVA: 0x00048225 File Offset: 0x00046425
		public int EnemyUnitCount
		{
			get
			{
				return this._enemyMemberCount.Value;
			}
		}

		// Token: 0x17000403 RID: 1027
		// (get) Token: 0x06001299 RID: 4761 RVA: 0x00048232 File Offset: 0x00046432
		public float AllyInfantryRatio
		{
			get
			{
				return this._allyInfantryRatio.Value;
			}
		}

		// Token: 0x17000404 RID: 1028
		// (get) Token: 0x0600129A RID: 4762 RVA: 0x0004823F File Offset: 0x0004643F
		public float AllyRangedRatio
		{
			get
			{
				return this._allyRangedRatio.Value;
			}
		}

		// Token: 0x17000405 RID: 1029
		// (get) Token: 0x0600129B RID: 4763 RVA: 0x0004824C File Offset: 0x0004644C
		public float AllyCavalryRatio
		{
			get
			{
				return this._allyCavalryRatio.Value;
			}
		}

		// Token: 0x17000406 RID: 1030
		// (get) Token: 0x0600129C RID: 4764 RVA: 0x00048259 File Offset: 0x00046459
		public float AllyRangedCavalryRatio
		{
			get
			{
				return this._allyRangedCavalryRatio.Value;
			}
		}

		// Token: 0x17000407 RID: 1031
		// (get) Token: 0x0600129D RID: 4765 RVA: 0x00048266 File Offset: 0x00046466
		public float EnemyInfantryRatio
		{
			get
			{
				return this._enemyInfantryRatio.Value;
			}
		}

		// Token: 0x17000408 RID: 1032
		// (get) Token: 0x0600129E RID: 4766 RVA: 0x00048273 File Offset: 0x00046473
		public float EnemyRangedRatio
		{
			get
			{
				return this._enemyRangedRatio.Value;
			}
		}

		// Token: 0x17000409 RID: 1033
		// (get) Token: 0x0600129F RID: 4767 RVA: 0x00048280 File Offset: 0x00046480
		public float EnemyCavalryRatio
		{
			get
			{
				return this._enemyCavalryRatio.Value;
			}
		}

		// Token: 0x1700040A RID: 1034
		// (get) Token: 0x060012A0 RID: 4768 RVA: 0x0004828D File Offset: 0x0004648D
		public float EnemyRangedCavalryRatio
		{
			get
			{
				return this._enemyRangedCavalryRatio.Value;
			}
		}

		// Token: 0x1700040B RID: 1035
		// (get) Token: 0x060012A1 RID: 4769 RVA: 0x0004829A File Offset: 0x0004649A
		public float RemainingPowerRatio
		{
			get
			{
				return this._remainingPowerRatio.Value;
			}
		}

		// Token: 0x1700040C RID: 1036
		// (get) Token: 0x060012A2 RID: 4770 RVA: 0x000482A7 File Offset: 0x000464A7
		public float TeamPower
		{
			get
			{
				return this._teamPower.Value;
			}
		}

		// Token: 0x1700040D RID: 1037
		// (get) Token: 0x060012A3 RID: 4771 RVA: 0x000482B4 File Offset: 0x000464B4
		public float TotalPowerRatio
		{
			get
			{
				return this._totalPowerRatio.Value;
			}
		}

		// Token: 0x1700040E RID: 1038
		// (get) Token: 0x060012A4 RID: 4772 RVA: 0x000482C1 File Offset: 0x000464C1
		public float InsideWallsRatio
		{
			get
			{
				return this._insideWallsRatio.Value;
			}
		}

		// Token: 0x1700040F RID: 1039
		// (get) Token: 0x060012A5 RID: 4773 RVA: 0x000482CE File Offset: 0x000464CE
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

		// Token: 0x17000410 RID: 1040
		// (get) Token: 0x060012A6 RID: 4774 RVA: 0x000482EF File Offset: 0x000464EF
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

		// Token: 0x17000411 RID: 1041
		// (get) Token: 0x060012A7 RID: 4775 RVA: 0x00048310 File Offset: 0x00046510
		public float MaxUnderRangedAttackRatio
		{
			get
			{
				return this._maxUnderRangedAttackRatio.Value;
			}
		}

		// Token: 0x17000412 RID: 1042
		// (get) Token: 0x060012A8 RID: 4776 RVA: 0x0004831D File Offset: 0x0004651D
		// (set) Token: 0x060012A9 RID: 4777 RVA: 0x00048325 File Offset: 0x00046525
		public int DeathCount { get; private set; }

		// Token: 0x17000413 RID: 1043
		// (get) Token: 0x060012AA RID: 4778 RVA: 0x0004832E File Offset: 0x0004652E
		// (set) Token: 0x060012AB RID: 4779 RVA: 0x00048336 File Offset: 0x00046536
		public int DeathByRangedCount { get; private set; }

		// Token: 0x17000414 RID: 1044
		// (get) Token: 0x060012AC RID: 4780 RVA: 0x0004833F File Offset: 0x0004653F
		public int AllyRangedUnitCount
		{
			get
			{
				return (int)(this.AllyRangedRatio * (float)this.AllyUnitCount);
			}
		}

		// Token: 0x17000415 RID: 1045
		// (get) Token: 0x060012AD RID: 4781 RVA: 0x00048350 File Offset: 0x00046550
		public int AllCavalryUnitCount
		{
			get
			{
				return (int)(this.AllyCavalryRatio * (float)this.AllyUnitCount);
			}
		}

		// Token: 0x17000416 RID: 1046
		// (get) Token: 0x060012AE RID: 4782 RVA: 0x00048361 File Offset: 0x00046561
		public int EnemyRangedUnitCount
		{
			get
			{
				return (int)(this.EnemyRangedRatio * (float)this.EnemyUnitCount);
			}
		}

		// Token: 0x060012AF RID: 4783 RVA: 0x00048374 File Offset: 0x00046574
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

		// Token: 0x060012B0 RID: 4784 RVA: 0x000484F8 File Offset: 0x000466F8
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

		// Token: 0x060012B1 RID: 4785 RVA: 0x000485D6 File Offset: 0x000467D6
		private void InitializeTelemetryScopeNames()
		{
		}

		// Token: 0x060012B2 RID: 4786 RVA: 0x000485D8 File Offset: 0x000467D8
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

		// Token: 0x060012B3 RID: 4787 RVA: 0x00048984 File Offset: 0x00046B84
		public void RegisterDeath()
		{
			int deathCount = this.DeathCount;
			this.DeathCount = deathCount + 1;
		}

		// Token: 0x060012B4 RID: 4788 RVA: 0x000489A4 File Offset: 0x00046BA4
		public void RegisterDeathByRanged()
		{
			int deathByRangedCount = this.DeathByRangedCount;
			this.DeathByRangedCount = deathByRangedCount + 1;
		}

		// Token: 0x060012B5 RID: 4789 RVA: 0x000489C4 File Offset: 0x00046BC4
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

		// Token: 0x060012B6 RID: 4790 RVA: 0x000489FC File Offset: 0x00046BFC
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

		// Token: 0x0400051A RID: 1306
		public readonly Team Team;

		// Token: 0x0400051B RID: 1307
		private readonly Mission _mission;

		// Token: 0x0400051C RID: 1308
		private readonly QueryData<int> _memberCount;

		// Token: 0x0400051D RID: 1309
		private readonly QueryData<WorldPosition> _medianPosition;

		// Token: 0x0400051E RID: 1310
		private readonly QueryData<Vec2> _averagePosition;

		// Token: 0x0400051F RID: 1311
		private readonly QueryData<Vec2> _averageEnemyPosition;

		// Token: 0x04000520 RID: 1312
		private readonly QueryData<WorldPosition> _medianTargetFormationPosition;

		// Token: 0x04000521 RID: 1313
		private readonly QueryData<WorldPosition> _leftFlankEdgePosition;

		// Token: 0x04000522 RID: 1314
		private readonly QueryData<WorldPosition> _rightFlankEdgePosition;

		// Token: 0x04000523 RID: 1315
		private readonly QueryData<float> _infantryRatio;

		// Token: 0x04000524 RID: 1316
		private readonly QueryData<float> _rangedRatio;

		// Token: 0x04000525 RID: 1317
		private readonly QueryData<float> _cavalryRatio;

		// Token: 0x04000526 RID: 1318
		private readonly QueryData<float> _rangedCavalryRatio;

		// Token: 0x04000527 RID: 1319
		private readonly QueryData<int> _allyMemberCount;

		// Token: 0x04000528 RID: 1320
		private readonly QueryData<int> _enemyMemberCount;

		// Token: 0x04000529 RID: 1321
		private readonly QueryData<float> _allyInfantryRatio;

		// Token: 0x0400052A RID: 1322
		private readonly QueryData<float> _allyRangedRatio;

		// Token: 0x0400052B RID: 1323
		private readonly QueryData<float> _allyCavalryRatio;

		// Token: 0x0400052C RID: 1324
		private readonly QueryData<float> _allyRangedCavalryRatio;

		// Token: 0x0400052D RID: 1325
		private readonly QueryData<float> _enemyInfantryRatio;

		// Token: 0x0400052E RID: 1326
		private readonly QueryData<float> _enemyRangedRatio;

		// Token: 0x0400052F RID: 1327
		private readonly QueryData<float> _enemyCavalryRatio;

		// Token: 0x04000530 RID: 1328
		private readonly QueryData<float> _enemyRangedCavalryRatio;

		// Token: 0x04000531 RID: 1329
		private readonly QueryData<float> _remainingPowerRatio;

		// Token: 0x04000532 RID: 1330
		private readonly QueryData<float> _teamPower;

		// Token: 0x04000533 RID: 1331
		private readonly QueryData<float> _totalPowerRatio;

		// Token: 0x04000534 RID: 1332
		private readonly QueryData<float> _insideWallsRatio;

		// Token: 0x04000535 RID: 1333
		private BattlePowerCalculationLogic _battlePowerLogic;

		// Token: 0x04000536 RID: 1334
		private CasualtyHandler _casualtyHandler;

		// Token: 0x04000537 RID: 1335
		private readonly QueryData<float> _maxUnderRangedAttackRatio;
	}
}
