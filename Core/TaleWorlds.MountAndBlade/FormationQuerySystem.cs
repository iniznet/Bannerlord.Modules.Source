using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000166 RID: 358
	public class FormationQuerySystem
	{
		// Token: 0x170003AD RID: 941
		// (get) Token: 0x06001215 RID: 4629 RVA: 0x00046360 File Offset: 0x00044560
		public TeamQuerySystem Team
		{
			get
			{
				return this.Formation.Team.QuerySystem;
			}
		}

		// Token: 0x170003AE RID: 942
		// (get) Token: 0x06001216 RID: 4630 RVA: 0x00046372 File Offset: 0x00044572
		public float FormationPower
		{
			get
			{
				return this._formationPower.Value;
			}
		}

		// Token: 0x170003AF RID: 943
		// (get) Token: 0x06001217 RID: 4631 RVA: 0x0004637F File Offset: 0x0004457F
		public float FormationMeleeFightingPower
		{
			get
			{
				return this._formationMeleeFightingPower.Value;
			}
		}

		// Token: 0x170003B0 RID: 944
		// (get) Token: 0x06001218 RID: 4632 RVA: 0x0004638C File Offset: 0x0004458C
		public Vec2 AveragePosition
		{
			get
			{
				return this._averagePosition.Value;
			}
		}

		// Token: 0x170003B1 RID: 945
		// (get) Token: 0x06001219 RID: 4633 RVA: 0x00046399 File Offset: 0x00044599
		public Vec2 CurrentVelocity
		{
			get
			{
				return this._currentVelocity.Value;
			}
		}

		// Token: 0x170003B2 RID: 946
		// (get) Token: 0x0600121A RID: 4634 RVA: 0x000463A6 File Offset: 0x000445A6
		public Vec2 EstimatedDirection
		{
			get
			{
				return this._estimatedDirection.Value;
			}
		}

		// Token: 0x170003B3 RID: 947
		// (get) Token: 0x0600121B RID: 4635 RVA: 0x000463B3 File Offset: 0x000445B3
		public float EstimatedInterval
		{
			get
			{
				return this._estimatedInterval.Value;
			}
		}

		// Token: 0x170003B4 RID: 948
		// (get) Token: 0x0600121C RID: 4636 RVA: 0x000463C0 File Offset: 0x000445C0
		public WorldPosition MedianPosition
		{
			get
			{
				return this._medianPosition.Value;
			}
		}

		// Token: 0x170003B5 RID: 949
		// (get) Token: 0x0600121D RID: 4637 RVA: 0x000463CD File Offset: 0x000445CD
		public Vec2 AverageAllyPosition
		{
			get
			{
				return this._averageAllyPosition.Value;
			}
		}

		// Token: 0x170003B6 RID: 950
		// (get) Token: 0x0600121E RID: 4638 RVA: 0x000463DA File Offset: 0x000445DA
		public float IdealAverageDisplacement
		{
			get
			{
				return this._idealAverageDisplacement.Value;
			}
		}

		// Token: 0x170003B7 RID: 951
		// (get) Token: 0x0600121F RID: 4639 RVA: 0x000463E7 File Offset: 0x000445E7
		public float FormationDispersedness
		{
			get
			{
				return this._formationDispersedness.Value;
			}
		}

		// Token: 0x170003B8 RID: 952
		// (get) Token: 0x06001220 RID: 4640 RVA: 0x000463F4 File Offset: 0x000445F4
		public FormationQuerySystem.FormationIntegrityDataGroup FormationIntegrityData
		{
			get
			{
				return this._formationIntegrityData.Value;
			}
		}

		// Token: 0x170003B9 RID: 953
		// (get) Token: 0x06001221 RID: 4641 RVA: 0x00046401 File Offset: 0x00044601
		public MBList<Agent> LocalAllyUnits
		{
			get
			{
				return this._localAllyUnits.Value;
			}
		}

		// Token: 0x170003BA RID: 954
		// (get) Token: 0x06001222 RID: 4642 RVA: 0x0004640E File Offset: 0x0004460E
		public MBList<Agent> LocalEnemyUnits
		{
			get
			{
				return this._localEnemyUnits.Value;
			}
		}

		// Token: 0x170003BB RID: 955
		// (get) Token: 0x06001223 RID: 4643 RVA: 0x0004641B File Offset: 0x0004461B
		public FormationClass MainClass
		{
			get
			{
				return this._mainClass.Value;
			}
		}

		// Token: 0x170003BC RID: 956
		// (get) Token: 0x06001224 RID: 4644 RVA: 0x00046428 File Offset: 0x00044628
		public float InfantryUnitRatio
		{
			get
			{
				return this._infantryUnitRatio.Value;
			}
		}

		// Token: 0x170003BD RID: 957
		// (get) Token: 0x06001225 RID: 4645 RVA: 0x00046435 File Offset: 0x00044635
		public float HasShieldUnitRatio
		{
			get
			{
				return this._hasShieldUnitRatio.Value;
			}
		}

		// Token: 0x170003BE RID: 958
		// (get) Token: 0x06001226 RID: 4646 RVA: 0x00046442 File Offset: 0x00044642
		public float RangedUnitRatio
		{
			get
			{
				return this._rangedUnitRatio.Value;
			}
		}

		// Token: 0x170003BF RID: 959
		// (get) Token: 0x06001227 RID: 4647 RVA: 0x0004644F File Offset: 0x0004464F
		public int InsideCastleUnitCountIncludingUnpositioned
		{
			get
			{
				return this._insideCastleUnitCountIncludingUnpositioned.Value;
			}
		}

		// Token: 0x170003C0 RID: 960
		// (get) Token: 0x06001228 RID: 4648 RVA: 0x0004645C File Offset: 0x0004465C
		public int InsideCastleUnitCountPositioned
		{
			get
			{
				return this._insideCastleUnitCountPositioned.Value;
			}
		}

		// Token: 0x170003C1 RID: 961
		// (get) Token: 0x06001229 RID: 4649 RVA: 0x00046469 File Offset: 0x00044669
		public float CavalryUnitRatio
		{
			get
			{
				return this._cavalryUnitRatio.Value;
			}
		}

		// Token: 0x170003C2 RID: 962
		// (get) Token: 0x0600122A RID: 4650 RVA: 0x00046476 File Offset: 0x00044676
		public float GetCavalryUnitRatioWithoutExpiration
		{
			get
			{
				return this._cavalryUnitRatio.GetCachedValue();
			}
		}

		// Token: 0x170003C3 RID: 963
		// (get) Token: 0x0600122B RID: 4651 RVA: 0x00046483 File Offset: 0x00044683
		public float RangedCavalryUnitRatio
		{
			get
			{
				return this._rangedCavalryUnitRatio.Value;
			}
		}

		// Token: 0x170003C4 RID: 964
		// (get) Token: 0x0600122C RID: 4652 RVA: 0x00046490 File Offset: 0x00044690
		public float GetRangedCavalryUnitRatioWithoutExpiration
		{
			get
			{
				return this._rangedCavalryUnitRatio.GetCachedValue();
			}
		}

		// Token: 0x170003C5 RID: 965
		// (get) Token: 0x0600122D RID: 4653 RVA: 0x0004649D File Offset: 0x0004469D
		public bool IsMeleeFormation
		{
			get
			{
				return this._isMeleeFormation.Value;
			}
		}

		// Token: 0x170003C6 RID: 966
		// (get) Token: 0x0600122E RID: 4654 RVA: 0x000464AA File Offset: 0x000446AA
		public bool IsInfantryFormation
		{
			get
			{
				return this._isInfantryFormation.Value;
			}
		}

		// Token: 0x170003C7 RID: 967
		// (get) Token: 0x0600122F RID: 4655 RVA: 0x000464B7 File Offset: 0x000446B7
		public bool HasShield
		{
			get
			{
				return this._hasShield.Value;
			}
		}

		// Token: 0x170003C8 RID: 968
		// (get) Token: 0x06001230 RID: 4656 RVA: 0x000464C4 File Offset: 0x000446C4
		public bool IsRangedFormation
		{
			get
			{
				return this._isRangedFormation.Value;
			}
		}

		// Token: 0x170003C9 RID: 969
		// (get) Token: 0x06001231 RID: 4657 RVA: 0x000464D1 File Offset: 0x000446D1
		public bool IsCavalryFormation
		{
			get
			{
				return this._isCavalryFormation.Value;
			}
		}

		// Token: 0x170003CA RID: 970
		// (get) Token: 0x06001232 RID: 4658 RVA: 0x000464DE File Offset: 0x000446DE
		public bool IsRangedCavalryFormation
		{
			get
			{
				return this._isRangedCavalryFormation.Value;
			}
		}

		// Token: 0x170003CB RID: 971
		// (get) Token: 0x06001233 RID: 4659 RVA: 0x000464EB File Offset: 0x000446EB
		public float MovementSpeedMaximum
		{
			get
			{
				return this._movementSpeedMaximum.Value;
			}
		}

		// Token: 0x170003CC RID: 972
		// (get) Token: 0x06001234 RID: 4660 RVA: 0x000464F8 File Offset: 0x000446F8
		public float MovementSpeed
		{
			get
			{
				return this._movementSpeed.Value;
			}
		}

		// Token: 0x170003CD RID: 973
		// (get) Token: 0x06001235 RID: 4661 RVA: 0x00046505 File Offset: 0x00044705
		public float MaximumMissileRange
		{
			get
			{
				return this._maximumMissileRange.Value;
			}
		}

		// Token: 0x170003CE RID: 974
		// (get) Token: 0x06001236 RID: 4662 RVA: 0x00046512 File Offset: 0x00044712
		public float MissileRangeAdjusted
		{
			get
			{
				return this._missileRangeAdjusted.Value;
			}
		}

		// Token: 0x170003CF RID: 975
		// (get) Token: 0x06001237 RID: 4663 RVA: 0x0004651F File Offset: 0x0004471F
		public float LocalInfantryUnitRatio
		{
			get
			{
				return this._localInfantryUnitRatio.Value;
			}
		}

		// Token: 0x170003D0 RID: 976
		// (get) Token: 0x06001238 RID: 4664 RVA: 0x0004652C File Offset: 0x0004472C
		public float LocalRangedUnitRatio
		{
			get
			{
				return this._localRangedUnitRatio.Value;
			}
		}

		// Token: 0x170003D1 RID: 977
		// (get) Token: 0x06001239 RID: 4665 RVA: 0x00046539 File Offset: 0x00044739
		public float LocalCavalryUnitRatio
		{
			get
			{
				return this._localCavalryUnitRatio.Value;
			}
		}

		// Token: 0x170003D2 RID: 978
		// (get) Token: 0x0600123A RID: 4666 RVA: 0x00046546 File Offset: 0x00044746
		public float LocalRangedCavalryUnitRatio
		{
			get
			{
				return this._localRangedCavalryUnitRatio.Value;
			}
		}

		// Token: 0x170003D3 RID: 979
		// (get) Token: 0x0600123B RID: 4667 RVA: 0x00046553 File Offset: 0x00044753
		public float LocalAllyPower
		{
			get
			{
				return this._localAllyPower.Value;
			}
		}

		// Token: 0x170003D4 RID: 980
		// (get) Token: 0x0600123C RID: 4668 RVA: 0x00046560 File Offset: 0x00044760
		public float LocalEnemyPower
		{
			get
			{
				return this._localEnemyPower.Value;
			}
		}

		// Token: 0x170003D5 RID: 981
		// (get) Token: 0x0600123D RID: 4669 RVA: 0x0004656D File Offset: 0x0004476D
		public float LocalPowerRatio
		{
			get
			{
				return this._localPowerRatio.Value;
			}
		}

		// Token: 0x170003D6 RID: 982
		// (get) Token: 0x0600123E RID: 4670 RVA: 0x0004657A File Offset: 0x0004477A
		public float CasualtyRatio
		{
			get
			{
				return this._casualtyRatio.Value;
			}
		}

		// Token: 0x170003D7 RID: 983
		// (get) Token: 0x0600123F RID: 4671 RVA: 0x00046587 File Offset: 0x00044787
		public bool IsUnderRangedAttack
		{
			get
			{
				return this._isUnderRangedAttack.Value;
			}
		}

		// Token: 0x170003D8 RID: 984
		// (get) Token: 0x06001240 RID: 4672 RVA: 0x00046594 File Offset: 0x00044794
		public float UnderRangedAttackRatio
		{
			get
			{
				return this._underRangedAttackRatio.Value;
			}
		}

		// Token: 0x170003D9 RID: 985
		// (get) Token: 0x06001241 RID: 4673 RVA: 0x000465A1 File Offset: 0x000447A1
		public float MakingRangedAttackRatio
		{
			get
			{
				return this._makingRangedAttackRatio.Value;
			}
		}

		// Token: 0x170003DA RID: 986
		// (get) Token: 0x06001242 RID: 4674 RVA: 0x000465AE File Offset: 0x000447AE
		public Formation MainFormation
		{
			get
			{
				return this._mainFormation.Value;
			}
		}

		// Token: 0x170003DB RID: 987
		// (get) Token: 0x06001243 RID: 4675 RVA: 0x000465BB File Offset: 0x000447BB
		public float MainFormationReliabilityFactor
		{
			get
			{
				return this._mainFormationReliabilityFactor.Value;
			}
		}

		// Token: 0x170003DC RID: 988
		// (get) Token: 0x06001244 RID: 4676 RVA: 0x000465C8 File Offset: 0x000447C8
		public Vec2 WeightedAverageEnemyPosition
		{
			get
			{
				return this._weightedAverageEnemyPosition.Value;
			}
		}

		// Token: 0x170003DD RID: 989
		// (get) Token: 0x06001245 RID: 4677 RVA: 0x000465D8 File Offset: 0x000447D8
		public FormationQuerySystem ClosestEnemyFormation
		{
			get
			{
				if (this._closestEnemyFormation.Value == null || this._closestEnemyFormation.Value.CountOfUnits == 0)
				{
					this._closestEnemyFormation.Expire();
				}
				Formation value = this._closestEnemyFormation.Value;
				if (value == null)
				{
					return null;
				}
				return value.QuerySystem;
			}
		}

		// Token: 0x170003DE RID: 990
		// (get) Token: 0x06001246 RID: 4678 RVA: 0x00046628 File Offset: 0x00044828
		public FormationQuerySystem ClosestSignificantlyLargeEnemyFormation
		{
			get
			{
				if (this._closestSignificantlyLargeEnemyFormation.Value == null || this._closestSignificantlyLargeEnemyFormation.Value.CountOfUnits == 0)
				{
					this._closestSignificantlyLargeEnemyFormation.Expire();
				}
				Formation value = this._closestSignificantlyLargeEnemyFormation.Value;
				if (value == null)
				{
					return null;
				}
				return value.QuerySystem;
			}
		}

		// Token: 0x170003DF RID: 991
		// (get) Token: 0x06001247 RID: 4679 RVA: 0x00046678 File Offset: 0x00044878
		public FormationQuerySystem FastestSignificantlyLargeEnemyFormation
		{
			get
			{
				if (this._fastestSignificantlyLargeEnemyFormation.Value == null || this._fastestSignificantlyLargeEnemyFormation.Value.CountOfUnits == 0)
				{
					this._fastestSignificantlyLargeEnemyFormation.Expire();
				}
				Formation value = this._fastestSignificantlyLargeEnemyFormation.Value;
				if (value == null)
				{
					return null;
				}
				return value.QuerySystem;
			}
		}

		// Token: 0x170003E0 RID: 992
		// (get) Token: 0x06001248 RID: 4680 RVA: 0x000466C5 File Offset: 0x000448C5
		public Vec2 HighGroundCloseToForeseenBattleGround
		{
			get
			{
				return this._highGroundCloseToForeseenBattleGround.Value;
			}
		}

		// Token: 0x06001249 RID: 4681 RVA: 0x000466D4 File Offset: 0x000448D4
		public FormationQuerySystem(Formation formation)
		{
			FormationQuerySystem.<>c__DisplayClass156_0 CS$<>8__locals1 = new FormationQuerySystem.<>c__DisplayClass156_0();
			CS$<>8__locals1.formation = formation;
			base..ctor();
			CS$<>8__locals1.<>4__this = this;
			this.Formation = CS$<>8__locals1.formation;
			Mission mission = Mission.Current;
			this._formationPower = new QueryData<float>(new Func<float>(CS$<>8__locals1.formation.GetFormationPower), 2.5f);
			this._formationMeleeFightingPower = new QueryData<float>(new Func<float>(CS$<>8__locals1.formation.GetFormationMeleeFightingPower), 2.5f);
			this._averagePosition = new QueryData<Vec2>(delegate
			{
				Vec2 vec = ((CS$<>8__locals1.formation.CountOfUnitsWithoutDetachedOnes > 1) ? CS$<>8__locals1.formation.GetAveragePositionOfUnits(true, true) : ((CS$<>8__locals1.formation.CountOfUnitsWithoutDetachedOnes > 0) ? CS$<>8__locals1.formation.GetAveragePositionOfUnits(true, false) : CS$<>8__locals1.formation.OrderPosition));
				float currentTime = Mission.Current.CurrentTime;
				float num = currentTime - CS$<>8__locals1.<>4__this._lastAveragePositionCalculateTime;
				if (num > 0f)
				{
					CS$<>8__locals1.<>4__this._currentVelocity.SetValue((vec - CS$<>8__locals1.<>4__this._averagePosition.GetCachedValue()) * (1f / num), currentTime);
				}
				CS$<>8__locals1.<>4__this._lastAveragePositionCalculateTime = currentTime;
				return vec;
			}, 0.05f);
			this._currentVelocity = new QueryData<Vec2>(delegate
			{
				CS$<>8__locals1.<>4__this._averagePosition.Evaluate(Mission.Current.CurrentTime);
				return CS$<>8__locals1.<>4__this._currentVelocity.GetCachedValue();
			}, 1f);
			this._estimatedDirection = new QueryData<Vec2>(delegate
			{
				if (CS$<>8__locals1.formation.CountOfUnitsWithoutDetachedOnes > 0)
				{
					Vec2 averagePositionOfUnits = CS$<>8__locals1.formation.GetAveragePositionOfUnits(true, true);
					float num2 = 0f;
					float num3 = 0f;
					Vec2 orderLocalAveragePosition = CS$<>8__locals1.formation.OrderLocalAveragePosition;
					int num4 = 0;
					foreach (IFormationUnit formationUnit in CS$<>8__locals1.formation.UnitsWithoutLooseDetachedOnes)
					{
						Agent agent = (Agent)formationUnit;
						Vec2? localPositionOfUnitOrDefault = CS$<>8__locals1.formation.Arrangement.GetLocalPositionOfUnitOrDefault(agent);
						if (localPositionOfUnitOrDefault != null)
						{
							Vec2 value = localPositionOfUnitOrDefault.Value;
							Vec2 asVec = agent.Position.AsVec2;
							num2 += (value.x - orderLocalAveragePosition.x) * (asVec.x - averagePositionOfUnits.x) + (value.y - orderLocalAveragePosition.y) * (asVec.y - averagePositionOfUnits.y);
							num3 += (value.x - orderLocalAveragePosition.x) * (asVec.y - averagePositionOfUnits.y) - (value.y - orderLocalAveragePosition.y) * (asVec.x - averagePositionOfUnits.x);
							num4++;
						}
					}
					if (num4 > 0)
					{
						float num5 = 1f / (float)num4;
						num2 *= num5;
						num3 *= num5;
						float num6 = MathF.Sqrt(num2 * num2 + num3 * num3);
						if (num6 > 0f)
						{
							float num7 = MathF.Acos(MBMath.ClampFloat(num2 / num6, -1f, 1f));
							Vec2 vec2 = Vec2.FromRotation(num7);
							Vec2 vec3 = Vec2.FromRotation(-num7);
							float num8 = 0f;
							float num9 = 0f;
							foreach (IFormationUnit formationUnit2 in CS$<>8__locals1.formation.UnitsWithoutLooseDetachedOnes)
							{
								Agent agent2 = (Agent)formationUnit2;
								Vec2? localPositionOfUnitOrDefault2 = CS$<>8__locals1.formation.Arrangement.GetLocalPositionOfUnitOrDefault(agent2);
								if (localPositionOfUnitOrDefault2 != null)
								{
									Vec2 vec4 = vec2.TransformToParentUnitF(localPositionOfUnitOrDefault2.Value - orderLocalAveragePosition);
									Vec2 vec5 = vec3.TransformToParentUnitF(localPositionOfUnitOrDefault2.Value - orderLocalAveragePosition);
									Vec2 asVec2 = agent2.Position.AsVec2;
									num8 += (vec4 - asVec2 + averagePositionOfUnits).LengthSquared;
									num9 += (vec5 - asVec2 + averagePositionOfUnits).LengthSquared;
								}
							}
							if (num8 >= num9)
							{
								return vec3;
							}
							return vec2;
						}
					}
				}
				return new Vec2(0f, 1f);
			}, 0.2f);
			this._estimatedInterval = new QueryData<float>(delegate
			{
				if (CS$<>8__locals1.formation.CountOfUnitsWithoutDetachedOnes > 0)
				{
					Vec2 estimatedDirection = CS$<>8__locals1.formation.QuerySystem.EstimatedDirection;
					Vec2 currentPosition = CS$<>8__locals1.formation.CurrentPosition;
					float num10 = 0f;
					float num11 = 0f;
					foreach (IFormationUnit formationUnit3 in CS$<>8__locals1.formation.UnitsWithoutLooseDetachedOnes)
					{
						Agent agent3 = (Agent)formationUnit3;
						Vec2? localPositionOfUnitOrDefault3 = CS$<>8__locals1.formation.Arrangement.GetLocalPositionOfUnitOrDefault(agent3);
						if (localPositionOfUnitOrDefault3 != null)
						{
							Vec2 vec6 = estimatedDirection.TransformToLocalUnitF(agent3.Position.AsVec2 - currentPosition);
							Vec2 vec7 = localPositionOfUnitOrDefault3.Value - vec6;
							Vec2 vec8 = CS$<>8__locals1.formation.Arrangement.GetLocalPositionOfUnitOrDefaultWithAdjustment(agent3, 1f).Value - localPositionOfUnitOrDefault3.Value;
							if (vec8.IsNonZero())
							{
								float num12 = vec8.Normalize();
								float num13 = Vec2.DotProduct(vec7, vec8);
								num10 += num13 * num12;
								num11 += num12 * num12;
							}
						}
					}
					if (num11 != 0f)
					{
						return Math.Max(0f, -num10 / num11 + CS$<>8__locals1.formation.Interval);
					}
				}
				return CS$<>8__locals1.formation.Interval;
			}, 0.2f);
			this._medianPosition = new QueryData<WorldPosition>(delegate
			{
				if (CS$<>8__locals1.formation.CountOfUnitsWithoutDetachedOnes != 0)
				{
					if (CS$<>8__locals1.formation.CountOfUnitsWithoutDetachedOnes != 1)
					{
						return CS$<>8__locals1.formation.GetMedianAgent(true, true, CS$<>8__locals1.<>4__this.AveragePosition).GetWorldPosition();
					}
					return CS$<>8__locals1.formation.GetMedianAgent(true, false, CS$<>8__locals1.<>4__this.AveragePosition).GetWorldPosition();
				}
				else
				{
					if (CS$<>8__locals1.formation.CountOfUnits == 0)
					{
						return CS$<>8__locals1.formation.CreateNewOrderWorldPosition(WorldPosition.WorldPositionEnforcedCache.None);
					}
					if (CS$<>8__locals1.formation.CountOfUnits != 1)
					{
						return CS$<>8__locals1.formation.GetMedianAgent(false, true, CS$<>8__locals1.<>4__this.AveragePosition).GetWorldPosition();
					}
					return CS$<>8__locals1.formation.GetFirstUnit().GetWorldPosition();
				}
			}, 0.05f);
			this._averageAllyPosition = new QueryData<Vec2>(delegate
			{
				int num14 = 0;
				Vec2 vec9 = Vec2.Zero;
				using (List<Team>.Enumerator enumerator3 = mission.Teams.GetEnumerator())
				{
					while (enumerator3.MoveNext())
					{
						if (enumerator3.Current.IsFriendOf(CS$<>8__locals1.formation.Team))
						{
							foreach (Formation formation2 in CS$<>8__locals1.<>4__this.Formation.Team.FormationsIncludingSpecialAndEmpty)
							{
								if (formation2.CountOfUnits > 0 && formation2 != CS$<>8__locals1.formation)
								{
									num14 += formation2.CountOfUnits;
									vec9 += formation2.GetAveragePositionOfUnits(false, false) * (float)formation2.CountOfUnits;
								}
							}
						}
					}
				}
				if (num14 > 0)
				{
					return vec9 * (1f / (float)num14);
				}
				return CS$<>8__locals1.<>4__this.AveragePosition;
			}, 5f);
			this._idealAverageDisplacement = new QueryData<float>(() => MathF.Sqrt(CS$<>8__locals1.formation.Width * CS$<>8__locals1.formation.Width * 0.5f * 0.5f + CS$<>8__locals1.formation.Depth * CS$<>8__locals1.formation.Depth * 0.5f * 0.5f) / 2f, 5f);
			this._formationIntegrityData = new QueryData<FormationQuerySystem.FormationIntegrityDataGroup>(delegate
			{
				FormationQuerySystem.FormationIntegrityDataGroup formationIntegrityDataGroup;
				if (CS$<>8__locals1.formation.CountOfUnitsWithoutDetachedOnes > 0)
				{
					float num15 = 0f;
					MBList<IFormationUnit> allUnits = CS$<>8__locals1.formation.Arrangement.GetAllUnits();
					int num16 = 0;
					float num17 = CS$<>8__locals1.formation.QuerySystem.EstimatedInterval - CS$<>8__locals1.formation.Interval;
					foreach (IFormationUnit formationUnit4 in allUnits)
					{
						Agent agent4 = (Agent)formationUnit4;
						Vec2? localPositionOfUnitOrDefaultWithAdjustment = CS$<>8__locals1.formation.Arrangement.GetLocalPositionOfUnitOrDefaultWithAdjustment(agent4, num17);
						if (localPositionOfUnitOrDefaultWithAdjustment != null)
						{
							Vec2 vec10 = CS$<>8__locals1.formation.QuerySystem.EstimatedDirection.TransformToParentUnitF(localPositionOfUnitOrDefaultWithAdjustment.Value) + CS$<>8__locals1.formation.CurrentPosition;
							num16++;
							num15 += (vec10 - agent4.Position.AsVec2).LengthSquared;
						}
					}
					if (num16 > 0)
					{
						float num18 = num15 / (float)num16 * 4f;
						float num19 = 0f;
						Vec2 vec11 = Vec2.Zero;
						float num20 = 0f;
						num16 = 0;
						foreach (IFormationUnit formationUnit5 in allUnits)
						{
							Agent agent5 = (Agent)formationUnit5;
							Vec2? localPositionOfUnitOrDefaultWithAdjustment2 = CS$<>8__locals1.formation.Arrangement.GetLocalPositionOfUnitOrDefaultWithAdjustment(agent5, num17);
							if (localPositionOfUnitOrDefaultWithAdjustment2 != null)
							{
								float lengthSquared = (CS$<>8__locals1.formation.QuerySystem.EstimatedDirection.TransformToParentUnitF(localPositionOfUnitOrDefaultWithAdjustment2.Value) + CS$<>8__locals1.formation.CurrentPosition - agent5.Position.AsVec2).LengthSquared;
								if (lengthSquared < num18)
								{
									num19 += lengthSquared;
									vec11 += agent5.AverageVelocity.AsVec2;
									num20 += agent5.MaximumForwardUnlimitedSpeed;
									num16++;
								}
							}
						}
						if (num16 > 0)
						{
							vec11 *= 1f / (float)num16;
							num19 /= (float)num16;
							num20 /= (float)num16;
							formationIntegrityDataGroup.AverageVelocityExcludeFarAgents = vec11;
							formationIntegrityDataGroup.DeviationOfPositionsExcludeFarAgents = MathF.Sqrt(num19);
							formationIntegrityDataGroup.AverageMaxUnlimitedSpeedExcludeFarAgents = num20;
							return formationIntegrityDataGroup;
						}
					}
				}
				formationIntegrityDataGroup.AverageVelocityExcludeFarAgents = Vec2.Zero;
				formationIntegrityDataGroup.DeviationOfPositionsExcludeFarAgents = 0f;
				formationIntegrityDataGroup.AverageMaxUnlimitedSpeedExcludeFarAgents = 0f;
				return formationIntegrityDataGroup;
			}, 1f);
			this._formationDispersedness = new QueryData<float>(delegate
			{
				if (CS$<>8__locals1.formation.CountOfUnits == 0)
				{
					return 0f;
				}
				float num21 = 0f;
				int num22 = 0;
				foreach (IFormationUnit formationUnit6 in CS$<>8__locals1.formation.Arrangement.GetAllUnits())
				{
					Vec2? localPositionOfUnitOrDefault4 = CS$<>8__locals1.formation.Arrangement.GetLocalPositionOfUnitOrDefault(formationUnit6);
					if (localPositionOfUnitOrDefault4 != null)
					{
						MatrixFrame matrixFrame = new MatrixFrame(Mat3.Identity, new Vec3(localPositionOfUnitOrDefault4.Value, 0f, -1f));
						MatrixFrame matrixFrame2 = new MatrixFrame(new Mat3(new Vec3(CS$<>8__locals1.formation.Direction.RightVec(), 0f, -1f), new Vec3(CS$<>8__locals1.formation.Direction, 0f, -1f), new Vec3(0f, 0f, 1f, -1f)), CS$<>8__locals1.formation.QuerySystem.MedianPosition.GetNavMeshVec3());
						MatrixFrame matrixFrame3 = matrixFrame2.TransformToParent(matrixFrame);
						num21 += (formationUnit6 as Agent).GetWorldPosition().GetGroundVec3().Distance(matrixFrame3.origin);
					}
					else
					{
						num22++;
					}
				}
				if (CS$<>8__locals1.formation.CountOfUnits - num22 > 0)
				{
					return num21 / (float)(CS$<>8__locals1.formation.CountOfUnits - num22);
				}
				return 0f;
			}, 2f);
			this._localAllyUnits = new QueryData<MBList<Agent>>(() => mission.GetNearbyAllyAgents(CS$<>8__locals1.<>4__this.AveragePosition, 30f, CS$<>8__locals1.formation.Team, CS$<>8__locals1.<>4__this._localAllyUnits.GetCachedValue()), 5f, new MBList<Agent>());
			this._localEnemyUnits = new QueryData<MBList<Agent>>(() => mission.GetNearbyEnemyAgents(CS$<>8__locals1.<>4__this.AveragePosition, 30f, CS$<>8__locals1.formation.Team, CS$<>8__locals1.<>4__this._localEnemyUnits.GetCachedValue()), 5f, new MBList<Agent>());
			this._infantryUnitRatio = new QueryData<float>(delegate
			{
				if (CS$<>8__locals1.formation.CountOfUnits > 0)
				{
					return (float)CS$<>8__locals1.formation.GetCountOfUnitsInClass(FormationClass.Infantry, false) / (float)CS$<>8__locals1.formation.CountOfUnits;
				}
				return 0f;
			}, 2.5f);
			this._hasShieldUnitRatio = new QueryData<float>(delegate
			{
				if (CS$<>8__locals1.formation.CountOfUnits > 0)
				{
					return (float)CS$<>8__locals1.formation.GetCountOfUnitsWithCondition(new Func<Agent, bool>(QueryLibrary.HasShield)) / (float)CS$<>8__locals1.formation.CountOfUnits;
				}
				return 0f;
			}, 2.5f);
			this._rangedUnitRatio = new QueryData<float>(delegate
			{
				if (CS$<>8__locals1.formation.CountOfUnits > 0)
				{
					return (float)CS$<>8__locals1.formation.GetCountOfUnitsInClass(FormationClass.Ranged, false) / (float)CS$<>8__locals1.formation.CountOfUnits;
				}
				return 0f;
			}, 2.5f);
			this._cavalryUnitRatio = new QueryData<float>(delegate
			{
				if (CS$<>8__locals1.formation.CountOfUnits > 0)
				{
					return (float)CS$<>8__locals1.formation.GetCountOfUnitsInClass(FormationClass.Cavalry, false) / (float)CS$<>8__locals1.formation.CountOfUnits;
				}
				return 0f;
			}, 2.5f);
			this._rangedCavalryUnitRatio = new QueryData<float>(delegate
			{
				if (CS$<>8__locals1.formation.CountOfUnits > 0)
				{
					return (float)CS$<>8__locals1.formation.GetCountOfUnitsInClass(FormationClass.HorseArcher, false) / (float)CS$<>8__locals1.formation.CountOfUnits;
				}
				return 0f;
			}, 2.5f);
			this._isMeleeFormation = new QueryData<bool>(() => CS$<>8__locals1.<>4__this.InfantryUnitRatio + CS$<>8__locals1.<>4__this.CavalryUnitRatio > CS$<>8__locals1.<>4__this.RangedUnitRatio + CS$<>8__locals1.<>4__this.RangedCavalryUnitRatio, 5f);
			this._isInfantryFormation = new QueryData<bool>(() => CS$<>8__locals1.<>4__this.InfantryUnitRatio >= CS$<>8__locals1.<>4__this.RangedUnitRatio && CS$<>8__locals1.<>4__this.InfantryUnitRatio >= CS$<>8__locals1.<>4__this.CavalryUnitRatio && CS$<>8__locals1.<>4__this.InfantryUnitRatio >= CS$<>8__locals1.<>4__this.RangedCavalryUnitRatio, 5f);
			this._hasShield = new QueryData<bool>(() => CS$<>8__locals1.<>4__this.HasShieldUnitRatio >= 0.4f, 5f);
			this._isRangedFormation = new QueryData<bool>(() => CS$<>8__locals1.<>4__this.RangedUnitRatio > CS$<>8__locals1.<>4__this.InfantryUnitRatio && CS$<>8__locals1.<>4__this.RangedUnitRatio >= CS$<>8__locals1.<>4__this.CavalryUnitRatio && CS$<>8__locals1.<>4__this.RangedUnitRatio >= CS$<>8__locals1.<>4__this.RangedCavalryUnitRatio, 5f);
			this._isCavalryFormation = new QueryData<bool>(() => CS$<>8__locals1.<>4__this.CavalryUnitRatio > CS$<>8__locals1.<>4__this.InfantryUnitRatio && CS$<>8__locals1.<>4__this.CavalryUnitRatio > CS$<>8__locals1.<>4__this.RangedUnitRatio && CS$<>8__locals1.<>4__this.CavalryUnitRatio >= CS$<>8__locals1.<>4__this.RangedCavalryUnitRatio, 5f);
			this._isRangedCavalryFormation = new QueryData<bool>(() => CS$<>8__locals1.<>4__this.RangedCavalryUnitRatio > CS$<>8__locals1.<>4__this.InfantryUnitRatio && CS$<>8__locals1.<>4__this.RangedCavalryUnitRatio > CS$<>8__locals1.<>4__this.RangedUnitRatio && CS$<>8__locals1.<>4__this.RangedCavalryUnitRatio > CS$<>8__locals1.<>4__this.CavalryUnitRatio, 5f);
			QueryData<float>.SetupSyncGroup(new IQueryData[]
			{
				this._infantryUnitRatio, this._hasShieldUnitRatio, this._rangedUnitRatio, this._cavalryUnitRatio, this._rangedCavalryUnitRatio, this._isMeleeFormation, this._isInfantryFormation, this._hasShield, this._isRangedFormation, this._isCavalryFormation,
				this._isRangedCavalryFormation
			});
			this._movementSpeedMaximum = new QueryData<float>(new Func<float>(CS$<>8__locals1.formation.GetAverageMaximumMovementSpeedOfUnits), 10f);
			this._movementSpeed = new QueryData<float>(new Func<float>(CS$<>8__locals1.formation.GetMovementSpeedOfUnits), 2f);
			this._maximumMissileRange = new QueryData<float>(delegate
			{
				if (CS$<>8__locals1.formation.CountOfUnits == 0)
				{
					return 0f;
				}
				float maximumRange = 0f;
				CS$<>8__locals1.formation.ApplyActionOnEachUnit(delegate(Agent agent)
				{
					if (agent.MaximumMissileRange > maximumRange)
					{
						maximumRange = agent.MaximumMissileRange;
					}
				}, null);
				return maximumRange;
			}, 10f);
			this._missileRangeAdjusted = new QueryData<float>(delegate
			{
				if (CS$<>8__locals1.formation.CountOfUnits == 0)
				{
					return 0f;
				}
				float sum = 0f;
				CS$<>8__locals1.formation.ApplyActionOnEachUnit(delegate(Agent agent)
				{
					sum += agent.MissileRangeAdjusted;
				}, null);
				return sum / (float)CS$<>8__locals1.formation.CountOfUnits;
			}, 10f);
			this._localInfantryUnitRatio = new QueryData<float>(delegate
			{
				if (CS$<>8__locals1.<>4__this.LocalAllyUnits.Count != 0)
				{
					return 1f * (float)CS$<>8__locals1.<>4__this.LocalAllyUnits.Count(new Func<Agent, bool>(QueryLibrary.IsInfantry)) / (float)CS$<>8__locals1.<>4__this.LocalAllyUnits.Count;
				}
				return 0f;
			}, 15f);
			this._localRangedUnitRatio = new QueryData<float>(delegate
			{
				if (CS$<>8__locals1.<>4__this.LocalAllyUnits.Count != 0)
				{
					return 1f * (float)CS$<>8__locals1.<>4__this.LocalAllyUnits.Count(new Func<Agent, bool>(QueryLibrary.IsRanged)) / (float)CS$<>8__locals1.<>4__this.LocalAllyUnits.Count;
				}
				return 0f;
			}, 15f);
			this._localCavalryUnitRatio = new QueryData<float>(delegate
			{
				if (CS$<>8__locals1.<>4__this.LocalAllyUnits.Count != 0)
				{
					return 1f * (float)CS$<>8__locals1.<>4__this.LocalAllyUnits.Count(new Func<Agent, bool>(QueryLibrary.IsCavalry)) / (float)CS$<>8__locals1.<>4__this.LocalAllyUnits.Count;
				}
				return 0f;
			}, 15f);
			this._localRangedCavalryUnitRatio = new QueryData<float>(delegate
			{
				if (CS$<>8__locals1.<>4__this.LocalAllyUnits.Count != 0)
				{
					return 1f * (float)CS$<>8__locals1.<>4__this.LocalAllyUnits.Count(new Func<Agent, bool>(QueryLibrary.IsRangedCavalry)) / (float)CS$<>8__locals1.<>4__this.LocalAllyUnits.Count;
				}
				return 0f;
			}, 15f);
			QueryData<float>.SetupSyncGroup(new IQueryData[] { this._localInfantryUnitRatio, this._localRangedUnitRatio, this._localCavalryUnitRatio, this._localRangedCavalryUnitRatio });
			this._localAllyPower = new QueryData<float>(() => CS$<>8__locals1.<>4__this.LocalAllyUnits.Sum((Agent lau) => lau.CharacterPowerCached), 5f);
			this._localEnemyPower = new QueryData<float>(() => CS$<>8__locals1.<>4__this.LocalEnemyUnits.Sum((Agent leu) => leu.CharacterPowerCached), 5f);
			this._localPowerRatio = new QueryData<float>(() => MBMath.ClampFloat(MathF.Sqrt((CS$<>8__locals1.<>4__this.LocalAllyUnits.Sum((Agent lau) => lau.CharacterPowerCached) + 1f) * 1f / (CS$<>8__locals1.<>4__this.LocalEnemyUnits.Sum((Agent leu) => leu.CharacterPowerCached) + 1f)), 0.5f, 1.75f), 5f);
			this._casualtyRatio = new QueryData<float>(delegate
			{
				if (CS$<>8__locals1.formation.CountOfUnits == 0)
				{
					return 0f;
				}
				CasualtyHandler missionBehavior = mission.GetMissionBehavior<CasualtyHandler>();
				int num23 = ((missionBehavior != null) ? missionBehavior.GetCasualtyCountOfFormation(CS$<>8__locals1.formation) : 0);
				return 1f - (float)num23 * 1f / (float)(num23 + CS$<>8__locals1.formation.CountOfUnits);
			}, 10f);
			this._isUnderRangedAttack = new QueryData<bool>(() => CS$<>8__locals1.formation.GetUnderAttackTypeOfUnits(10f) == Agent.UnderAttackType.UnderRangedAttack, 3f);
			this._underRangedAttackRatio = new QueryData<float>(delegate
			{
				float currentTime = MBCommon.GetTotalMissionTime();
				int countOfUnitsWithCondition = CS$<>8__locals1.formation.GetCountOfUnitsWithCondition((Agent agent) => currentTime - agent.LastRangedHitTime < 10f);
				if (CS$<>8__locals1.formation.CountOfUnits <= 0)
				{
					return 0f;
				}
				return (float)countOfUnitsWithCondition / (float)CS$<>8__locals1.formation.CountOfUnits;
			}, 3f);
			this._makingRangedAttackRatio = new QueryData<float>(delegate
			{
				float currentTime = MBCommon.GetTotalMissionTime();
				int countOfUnitsWithCondition2 = CS$<>8__locals1.formation.GetCountOfUnitsWithCondition((Agent agent) => currentTime - agent.LastRangedAttackTime < 10f);
				if (CS$<>8__locals1.formation.CountOfUnits <= 0)
				{
					return 0f;
				}
				return (float)countOfUnitsWithCondition2 / (float)CS$<>8__locals1.formation.CountOfUnits;
			}, 3f);
			this._closestEnemyFormation = new QueryData<Formation>(delegate
			{
				float num24 = float.MaxValue;
				Formation formation3 = null;
				foreach (Team team in mission.Teams)
				{
					if (team.IsEnemyOf(CS$<>8__locals1.formation.Team))
					{
						foreach (Formation formation4 in team.FormationsIncludingSpecialAndEmpty)
						{
							if (formation4.CountOfUnits > 0)
							{
								float num25 = formation4.QuerySystem.MedianPosition.GetNavMeshVec3().DistanceSquared(new Vec3(CS$<>8__locals1.<>4__this.AveragePosition, CS$<>8__locals1.<>4__this.MedianPosition.GetNavMeshZ(), -1f));
								if (num25 < num24)
								{
									num24 = num25;
									formation3 = formation4;
								}
							}
						}
					}
				}
				return formation3;
			}, 1.5f);
			this._closestSignificantlyLargeEnemyFormation = new QueryData<Formation>(delegate
			{
				float num26 = float.MaxValue;
				Formation formation5 = null;
				float num27 = float.MaxValue;
				Formation formation6 = null;
				foreach (Team team2 in mission.Teams)
				{
					if (team2.IsEnemyOf(CS$<>8__locals1.formation.Team))
					{
						foreach (Formation formation7 in team2.FormationsIncludingSpecialAndEmpty)
						{
							if (formation7.CountOfUnits > 0)
							{
								if (formation7.QuerySystem.FormationPower / CS$<>8__locals1.<>4__this.FormationPower > 0.2f || formation7.QuerySystem.FormationPower * CS$<>8__locals1.<>4__this.Team.TeamPower / (formation7.Team.QuerySystem.TeamPower * CS$<>8__locals1.<>4__this.FormationPower) > 0.2f)
								{
									float num28 = formation7.QuerySystem.MedianPosition.GetNavMeshVec3().DistanceSquared(new Vec3(CS$<>8__locals1.<>4__this.AveragePosition, CS$<>8__locals1.<>4__this.MedianPosition.GetNavMeshZ(), -1f));
									if (num28 < num26)
									{
										num26 = num28;
										formation5 = formation7;
									}
								}
								else if (formation5 == null)
								{
									float num29 = formation7.QuerySystem.MedianPosition.GetNavMeshVec3().DistanceSquared(new Vec3(CS$<>8__locals1.<>4__this.AveragePosition, CS$<>8__locals1.<>4__this.MedianPosition.GetNavMeshZ(), -1f));
									if (num29 < num27)
									{
										num27 = num29;
										formation6 = formation7;
									}
								}
							}
						}
					}
				}
				if (formation5 != null)
				{
					return formation5;
				}
				return formation6;
			}, 1.5f);
			this._fastestSignificantlyLargeEnemyFormation = new QueryData<Formation>(delegate
			{
				float num30 = float.MaxValue;
				Formation formation8 = null;
				float num31 = float.MaxValue;
				Formation formation9 = null;
				foreach (Team team3 in mission.Teams)
				{
					if (team3.IsEnemyOf(CS$<>8__locals1.formation.Team))
					{
						foreach (Formation formation10 in team3.FormationsIncludingSpecialAndEmpty)
						{
							if (formation10.CountOfUnits > 0)
							{
								if (formation10.QuerySystem.FormationPower / CS$<>8__locals1.<>4__this.FormationPower > 0.2f || formation10.QuerySystem.FormationPower * CS$<>8__locals1.<>4__this.Team.TeamPower / (formation10.Team.QuerySystem.TeamPower * CS$<>8__locals1.<>4__this.FormationPower) > 0.2f)
								{
									float num32 = formation10.QuerySystem.MedianPosition.GetNavMeshVec3().DistanceSquared(new Vec3(CS$<>8__locals1.<>4__this.AveragePosition, CS$<>8__locals1.<>4__this.MedianPosition.GetNavMeshZ(), -1f)) / (formation10.QuerySystem.MovementSpeed * formation10.QuerySystem.MovementSpeed);
									if (num32 < num30)
									{
										num30 = num32;
										formation8 = formation10;
									}
								}
								else if (formation8 == null)
								{
									float num33 = formation10.QuerySystem.MedianPosition.GetNavMeshVec3().DistanceSquared(new Vec3(CS$<>8__locals1.<>4__this.AveragePosition, CS$<>8__locals1.<>4__this.MedianPosition.GetNavMeshZ(), -1f)) / (formation10.QuerySystem.MovementSpeed * formation10.QuerySystem.MovementSpeed);
									if (num33 < num31)
									{
										num31 = num33;
										formation9 = formation10;
									}
								}
							}
						}
					}
				}
				if (formation8 != null)
				{
					return formation8;
				}
				return formation9;
			}, 1.5f);
			this._mainClass = new QueryData<FormationClass>(delegate
			{
				FormationClass formationClass = FormationClass.Infantry;
				float num34 = CS$<>8__locals1.<>4__this.InfantryUnitRatio;
				if (CS$<>8__locals1.<>4__this.RangedUnitRatio > num34)
				{
					formationClass = FormationClass.Ranged;
					num34 = CS$<>8__locals1.<>4__this.RangedUnitRatio;
				}
				if (CS$<>8__locals1.<>4__this.CavalryUnitRatio > num34)
				{
					formationClass = FormationClass.Cavalry;
					num34 = CS$<>8__locals1.<>4__this.CavalryUnitRatio;
				}
				if (CS$<>8__locals1.<>4__this.RangedCavalryUnitRatio > num34)
				{
					formationClass = FormationClass.HorseArcher;
				}
				return formationClass;
			}, 15f);
			this._mainFormation = new QueryData<Formation>(delegate
			{
				IEnumerable<Formation> formationsIncludingSpecialAndEmpty = CS$<>8__locals1.formation.Team.FormationsIncludingSpecialAndEmpty;
				Func<Formation, bool> func;
				if ((func = CS$<>8__locals1.<>9__53) == null)
				{
					func = (CS$<>8__locals1.<>9__53 = (Formation f) => f.CountOfUnits > 0 && f.AI.IsMainFormation && f != CS$<>8__locals1.formation);
				}
				return formationsIncludingSpecialAndEmpty.FirstOrDefault(func);
			}, 15f);
			this._mainFormationReliabilityFactor = new QueryData<float>(delegate
			{
				if (CS$<>8__locals1.<>4__this.MainFormation == null)
				{
					return 0f;
				}
				float num35 = ((CS$<>8__locals1.<>4__this.MainFormation.GetReadonlyMovementOrderReference().OrderEnum == MovementOrder.MovementOrderEnum.Charge || CS$<>8__locals1.<>4__this.MainFormation.GetReadonlyMovementOrderReference().OrderEnum == MovementOrder.MovementOrderEnum.ChargeToTarget || CS$<>8__locals1.<>4__this.MainFormation.GetReadonlyMovementOrderReference() == MovementOrder.MovementOrderRetreat) ? 0.5f : 1f);
				float num36 = ((CS$<>8__locals1.<>4__this.MainFormation.GetUnderAttackTypeOfUnits(10f) == Agent.UnderAttackType.UnderMeleeAttack) ? 0.8f : 1f);
				return num35 * num36;
			}, 5f);
			this._weightedAverageEnemyPosition = new QueryData<Vec2>(() => CS$<>8__locals1.<>4__this.Formation.Team.GetWeightedAverageOfEnemies(CS$<>8__locals1.<>4__this.Formation.CurrentPosition), 0.5f);
			this._highGroundCloseToForeseenBattleGround = new QueryData<Vec2>(delegate
			{
				WorldPosition medianPosition = CS$<>8__locals1.<>4__this.MedianPosition;
				medianPosition.SetVec2(CS$<>8__locals1.<>4__this.AveragePosition);
				WorldPosition medianTargetFormationPosition = CS$<>8__locals1.<>4__this.Team.MedianTargetFormationPosition;
				return mission.FindPositionWithBiggestSlopeTowardsDirectionInSquare(ref medianPosition, CS$<>8__locals1.<>4__this.AveragePosition.Distance(CS$<>8__locals1.<>4__this.Team.MedianTargetFormationPosition.AsVec2) * 0.5f, ref medianTargetFormationPosition).AsVec2;
			}, 10f);
			this._insideCastleUnitCountIncludingUnpositioned = new QueryData<int>(() => CS$<>8__locals1.<>4__this.Formation.CountUnitsOnNavMeshIDMod10(1, false), 3f);
			this._insideCastleUnitCountPositioned = new QueryData<int>(() => CS$<>8__locals1.<>4__this.Formation.CountUnitsOnNavMeshIDMod10(1, true), 3f);
			this.InitializeTelemetryScopeNames();
		}

		// Token: 0x0600124A RID: 4682 RVA: 0x00046E14 File Offset: 0x00045014
		public void EvaluateAllPreliminaryQueryData()
		{
			float currentTime = Mission.Current.CurrentTime;
			this._infantryUnitRatio.Evaluate(currentTime);
			this._hasShieldUnitRatio.Evaluate(currentTime);
			this._rangedUnitRatio.Evaluate(currentTime);
			this._cavalryUnitRatio.Evaluate(currentTime);
			this._rangedCavalryUnitRatio.Evaluate(currentTime);
			this._isInfantryFormation.Evaluate(currentTime);
			this._hasShield.Evaluate(currentTime);
			this._isRangedFormation.Evaluate(currentTime);
			this._isCavalryFormation.Evaluate(currentTime);
			this._isRangedCavalryFormation.Evaluate(currentTime);
			this._isMeleeFormation.Evaluate(currentTime);
		}

		// Token: 0x0600124B RID: 4683 RVA: 0x00046EB0 File Offset: 0x000450B0
		public void ForceExpireCavalryUnitRatio()
		{
			this._cavalryUnitRatio.Expire();
		}

		// Token: 0x0600124C RID: 4684 RVA: 0x00046EC0 File Offset: 0x000450C0
		public void Expire()
		{
			this._formationPower.Expire();
			this._formationMeleeFightingPower.Expire();
			this._averagePosition.Expire();
			this._currentVelocity.Expire();
			this._estimatedDirection.Expire();
			this._medianPosition.Expire();
			this._averageAllyPosition.Expire();
			this._idealAverageDisplacement.Expire();
			this._formationDispersedness.Expire();
			this._formationIntegrityData.Expire();
			this._localAllyUnits.Expire();
			this._localEnemyUnits.Expire();
			this._mainClass.Expire();
			this._infantryUnitRatio.Expire();
			this._hasShieldUnitRatio.Expire();
			this._rangedUnitRatio.Expire();
			this._cavalryUnitRatio.Expire();
			this._rangedCavalryUnitRatio.Expire();
			this._isMeleeFormation.Expire();
			this._isInfantryFormation.Expire();
			this._hasShield.Expire();
			this._isRangedFormation.Expire();
			this._isCavalryFormation.Expire();
			this._isRangedCavalryFormation.Expire();
			this._movementSpeedMaximum.Expire();
			this._movementSpeed.Expire();
			this._maximumMissileRange.Expire();
			this._missileRangeAdjusted.Expire();
			this._localInfantryUnitRatio.Expire();
			this._localRangedUnitRatio.Expire();
			this._localCavalryUnitRatio.Expire();
			this._localRangedCavalryUnitRatio.Expire();
			this._localAllyPower.Expire();
			this._localEnemyPower.Expire();
			this._localPowerRatio.Expire();
			this._casualtyRatio.Expire();
			this._isUnderRangedAttack.Expire();
			this._underRangedAttackRatio.Expire();
			this._makingRangedAttackRatio.Expire();
			this._mainFormation.Expire();
			this._mainFormationReliabilityFactor.Expire();
			this._weightedAverageEnemyPosition.Expire();
			this._closestEnemyFormation.Expire();
			this._closestSignificantlyLargeEnemyFormation.Expire();
			this._fastestSignificantlyLargeEnemyFormation.Expire();
			this._highGroundCloseToForeseenBattleGround.Expire();
		}

		// Token: 0x0600124D RID: 4685 RVA: 0x000470C8 File Offset: 0x000452C8
		public void ExpireAfterUnitAddRemove()
		{
			this._formationPower.Expire();
			float currentTime = Mission.Current.CurrentTime;
			this._infantryUnitRatio.Evaluate(currentTime);
			this._hasShieldUnitRatio.Evaluate(currentTime);
			this._rangedUnitRatio.Evaluate(currentTime);
			this._cavalryUnitRatio.Evaluate(currentTime);
			this._rangedCavalryUnitRatio.Evaluate(currentTime);
			this._isMeleeFormation.Evaluate(currentTime);
			this._isInfantryFormation.Evaluate(currentTime);
			this._hasShield.Evaluate(currentTime);
			this._isRangedFormation.Evaluate(currentTime);
			this._isCavalryFormation.Evaluate(currentTime);
			this._isRangedCavalryFormation.Evaluate(currentTime);
			this._mainClass.Evaluate(currentTime);
			if (this.Formation.CountOfUnits == 0)
			{
				this._infantryUnitRatio.SetValue(0f, currentTime);
				this._hasShieldUnitRatio.SetValue(0f, currentTime);
				this._rangedUnitRatio.SetValue(0f, currentTime);
				this._cavalryUnitRatio.SetValue(0f, currentTime);
				this._rangedCavalryUnitRatio.SetValue(0f, currentTime);
				this._isMeleeFormation.SetValue(false, currentTime);
				this._isInfantryFormation.SetValue(true, currentTime);
				this._hasShield.SetValue(false, currentTime);
				this._isRangedFormation.SetValue(false, currentTime);
				this._isCavalryFormation.SetValue(false, currentTime);
				this._isRangedCavalryFormation.SetValue(false, currentTime);
			}
		}

		// Token: 0x0600124E RID: 4686 RVA: 0x0004722E File Offset: 0x0004542E
		private void InitializeTelemetryScopeNames()
		{
		}

		// Token: 0x0600124F RID: 4687 RVA: 0x00047230 File Offset: 0x00045430
		public Vec2 GetAveragePositionWithMaxAge(float age)
		{
			return this._averagePosition.GetCachedValueWithMaxAge(age);
		}

		// Token: 0x06001250 RID: 4688 RVA: 0x0004723E File Offset: 0x0004543E
		public float GetClassWeightedFactor(float infantryWeight, float rangedWeight, float cavalryWeight, float rangedCavalryWeight)
		{
			return this.InfantryUnitRatio * infantryWeight + this.RangedUnitRatio * rangedWeight + this.CavalryUnitRatio * cavalryWeight + this.RangedCavalryUnitRatio * rangedCavalryWeight;
		}

		// Token: 0x040004CA RID: 1226
		public readonly Formation Formation;

		// Token: 0x040004CB RID: 1227
		private readonly QueryData<float> _formationPower;

		// Token: 0x040004CC RID: 1228
		private readonly QueryData<float> _formationMeleeFightingPower;

		// Token: 0x040004CD RID: 1229
		private readonly QueryData<Vec2> _averagePosition;

		// Token: 0x040004CE RID: 1230
		private readonly QueryData<Vec2> _currentVelocity;

		// Token: 0x040004CF RID: 1231
		private float _lastAveragePositionCalculateTime;

		// Token: 0x040004D0 RID: 1232
		private readonly QueryData<Vec2> _estimatedDirection;

		// Token: 0x040004D1 RID: 1233
		private readonly QueryData<float> _estimatedInterval;

		// Token: 0x040004D2 RID: 1234
		private readonly QueryData<WorldPosition> _medianPosition;

		// Token: 0x040004D3 RID: 1235
		private readonly QueryData<Vec2> _averageAllyPosition;

		// Token: 0x040004D4 RID: 1236
		private readonly QueryData<float> _idealAverageDisplacement;

		// Token: 0x040004D5 RID: 1237
		private readonly QueryData<float> _formationDispersedness;

		// Token: 0x040004D6 RID: 1238
		private readonly QueryData<FormationQuerySystem.FormationIntegrityDataGroup> _formationIntegrityData;

		// Token: 0x040004D7 RID: 1239
		private readonly QueryData<MBList<Agent>> _localAllyUnits;

		// Token: 0x040004D8 RID: 1240
		private readonly QueryData<MBList<Agent>> _localEnemyUnits;

		// Token: 0x040004D9 RID: 1241
		private readonly QueryData<FormationClass> _mainClass;

		// Token: 0x040004DA RID: 1242
		private readonly QueryData<float> _infantryUnitRatio;

		// Token: 0x040004DB RID: 1243
		private readonly QueryData<float> _hasShieldUnitRatio;

		// Token: 0x040004DC RID: 1244
		private readonly QueryData<float> _rangedUnitRatio;

		// Token: 0x040004DD RID: 1245
		private readonly QueryData<int> _insideCastleUnitCountIncludingUnpositioned;

		// Token: 0x040004DE RID: 1246
		private readonly QueryData<int> _insideCastleUnitCountPositioned;

		// Token: 0x040004DF RID: 1247
		private readonly QueryData<float> _cavalryUnitRatio;

		// Token: 0x040004E0 RID: 1248
		private readonly QueryData<float> _rangedCavalryUnitRatio;

		// Token: 0x040004E1 RID: 1249
		private readonly QueryData<bool> _isMeleeFormation;

		// Token: 0x040004E2 RID: 1250
		private readonly QueryData<bool> _isInfantryFormation;

		// Token: 0x040004E3 RID: 1251
		private readonly QueryData<bool> _hasShield;

		// Token: 0x040004E4 RID: 1252
		private readonly QueryData<bool> _isRangedFormation;

		// Token: 0x040004E5 RID: 1253
		private readonly QueryData<bool> _isCavalryFormation;

		// Token: 0x040004E6 RID: 1254
		private readonly QueryData<bool> _isRangedCavalryFormation;

		// Token: 0x040004E7 RID: 1255
		private readonly QueryData<float> _movementSpeedMaximum;

		// Token: 0x040004E8 RID: 1256
		private readonly QueryData<float> _movementSpeed;

		// Token: 0x040004E9 RID: 1257
		private readonly QueryData<float> _maximumMissileRange;

		// Token: 0x040004EA RID: 1258
		private readonly QueryData<float> _missileRangeAdjusted;

		// Token: 0x040004EB RID: 1259
		private readonly QueryData<float> _localInfantryUnitRatio;

		// Token: 0x040004EC RID: 1260
		private readonly QueryData<float> _localRangedUnitRatio;

		// Token: 0x040004ED RID: 1261
		private readonly QueryData<float> _localCavalryUnitRatio;

		// Token: 0x040004EE RID: 1262
		private readonly QueryData<float> _localRangedCavalryUnitRatio;

		// Token: 0x040004EF RID: 1263
		private readonly QueryData<float> _localAllyPower;

		// Token: 0x040004F0 RID: 1264
		private readonly QueryData<float> _localEnemyPower;

		// Token: 0x040004F1 RID: 1265
		private readonly QueryData<float> _localPowerRatio;

		// Token: 0x040004F2 RID: 1266
		private readonly QueryData<float> _casualtyRatio;

		// Token: 0x040004F3 RID: 1267
		private readonly QueryData<bool> _isUnderRangedAttack;

		// Token: 0x040004F4 RID: 1268
		private readonly QueryData<float> _underRangedAttackRatio;

		// Token: 0x040004F5 RID: 1269
		private readonly QueryData<float> _makingRangedAttackRatio;

		// Token: 0x040004F6 RID: 1270
		private readonly QueryData<Formation> _mainFormation;

		// Token: 0x040004F7 RID: 1271
		private readonly QueryData<float> _mainFormationReliabilityFactor;

		// Token: 0x040004F8 RID: 1272
		private readonly QueryData<Vec2> _weightedAverageEnemyPosition;

		// Token: 0x040004F9 RID: 1273
		private readonly QueryData<Formation> _closestEnemyFormation;

		// Token: 0x040004FA RID: 1274
		private readonly QueryData<Formation> _closestSignificantlyLargeEnemyFormation;

		// Token: 0x040004FB RID: 1275
		private readonly QueryData<Formation> _fastestSignificantlyLargeEnemyFormation;

		// Token: 0x040004FC RID: 1276
		private readonly QueryData<Vec2> _highGroundCloseToForeseenBattleGround;

		// Token: 0x020004D4 RID: 1236
		public struct FormationIntegrityDataGroup
		{
			// Token: 0x04001AE4 RID: 6884
			public Vec2 AverageVelocityExcludeFarAgents;

			// Token: 0x04001AE5 RID: 6885
			public float DeviationOfPositionsExcludeFarAgents;

			// Token: 0x04001AE6 RID: 6886
			public float AverageMaxUnlimitedSpeedExcludeFarAgents;
		}
	}
}
