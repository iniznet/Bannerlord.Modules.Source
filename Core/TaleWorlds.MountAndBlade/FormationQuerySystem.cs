using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public class FormationQuerySystem
	{
		public TeamQuerySystem Team
		{
			get
			{
				return this.Formation.Team.QuerySystem;
			}
		}

		public float FormationPower
		{
			get
			{
				return this._formationPower.Value;
			}
		}

		public float FormationMeleeFightingPower
		{
			get
			{
				return this._formationMeleeFightingPower.Value;
			}
		}

		public Vec2 AveragePosition
		{
			get
			{
				return this._averagePosition.Value;
			}
		}

		public Vec2 CurrentVelocity
		{
			get
			{
				return this._currentVelocity.Value;
			}
		}

		public Vec2 EstimatedDirection
		{
			get
			{
				return this._estimatedDirection.Value;
			}
		}

		public float EstimatedInterval
		{
			get
			{
				return this._estimatedInterval.Value;
			}
		}

		public WorldPosition MedianPosition
		{
			get
			{
				return this._medianPosition.Value;
			}
		}

		public Vec2 AverageAllyPosition
		{
			get
			{
				return this._averageAllyPosition.Value;
			}
		}

		public float IdealAverageDisplacement
		{
			get
			{
				return this._idealAverageDisplacement.Value;
			}
		}

		public float FormationDispersedness
		{
			get
			{
				return this._formationDispersedness.Value;
			}
		}

		public FormationQuerySystem.FormationIntegrityDataGroup FormationIntegrityData
		{
			get
			{
				return this._formationIntegrityData.Value;
			}
		}

		public MBList<Agent> LocalAllyUnits
		{
			get
			{
				return this._localAllyUnits.Value;
			}
		}

		public MBList<Agent> LocalEnemyUnits
		{
			get
			{
				return this._localEnemyUnits.Value;
			}
		}

		public FormationClass MainClass
		{
			get
			{
				return this._mainClass.Value;
			}
		}

		public float InfantryUnitRatio
		{
			get
			{
				return this._infantryUnitRatio.Value;
			}
		}

		public float HasShieldUnitRatio
		{
			get
			{
				return this._hasShieldUnitRatio.Value;
			}
		}

		public float RangedUnitRatio
		{
			get
			{
				return this._rangedUnitRatio.Value;
			}
		}

		public int InsideCastleUnitCountIncludingUnpositioned
		{
			get
			{
				return this._insideCastleUnitCountIncludingUnpositioned.Value;
			}
		}

		public int InsideCastleUnitCountPositioned
		{
			get
			{
				return this._insideCastleUnitCountPositioned.Value;
			}
		}

		public float CavalryUnitRatio
		{
			get
			{
				return this._cavalryUnitRatio.Value;
			}
		}

		public float GetCavalryUnitRatioWithoutExpiration
		{
			get
			{
				return this._cavalryUnitRatio.GetCachedValue();
			}
		}

		public float RangedCavalryUnitRatio
		{
			get
			{
				return this._rangedCavalryUnitRatio.Value;
			}
		}

		public float GetRangedCavalryUnitRatioWithoutExpiration
		{
			get
			{
				return this._rangedCavalryUnitRatio.GetCachedValue();
			}
		}

		public bool IsMeleeFormation
		{
			get
			{
				return this._isMeleeFormation.Value;
			}
		}

		public bool IsInfantryFormation
		{
			get
			{
				return this._isInfantryFormation.Value;
			}
		}

		public bool HasShield
		{
			get
			{
				return this._hasShield.Value;
			}
		}

		public bool IsRangedFormation
		{
			get
			{
				return this._isRangedFormation.Value;
			}
		}

		public bool IsCavalryFormation
		{
			get
			{
				return this._isCavalryFormation.Value;
			}
		}

		public bool IsRangedCavalryFormation
		{
			get
			{
				return this._isRangedCavalryFormation.Value;
			}
		}

		public float MovementSpeedMaximum
		{
			get
			{
				return this._movementSpeedMaximum.Value;
			}
		}

		public float MovementSpeed
		{
			get
			{
				return this._movementSpeed.Value;
			}
		}

		public float MaximumMissileRange
		{
			get
			{
				return this._maximumMissileRange.Value;
			}
		}

		public float MissileRangeAdjusted
		{
			get
			{
				return this._missileRangeAdjusted.Value;
			}
		}

		public float LocalInfantryUnitRatio
		{
			get
			{
				return this._localInfantryUnitRatio.Value;
			}
		}

		public float LocalRangedUnitRatio
		{
			get
			{
				return this._localRangedUnitRatio.Value;
			}
		}

		public float LocalCavalryUnitRatio
		{
			get
			{
				return this._localCavalryUnitRatio.Value;
			}
		}

		public float LocalRangedCavalryUnitRatio
		{
			get
			{
				return this._localRangedCavalryUnitRatio.Value;
			}
		}

		public float LocalAllyPower
		{
			get
			{
				return this._localAllyPower.Value;
			}
		}

		public float LocalEnemyPower
		{
			get
			{
				return this._localEnemyPower.Value;
			}
		}

		public float LocalPowerRatio
		{
			get
			{
				return this._localPowerRatio.Value;
			}
		}

		public float CasualtyRatio
		{
			get
			{
				return this._casualtyRatio.Value;
			}
		}

		public bool IsUnderRangedAttack
		{
			get
			{
				return this._isUnderRangedAttack.Value;
			}
		}

		public float UnderRangedAttackRatio
		{
			get
			{
				return this._underRangedAttackRatio.Value;
			}
		}

		public float MakingRangedAttackRatio
		{
			get
			{
				return this._makingRangedAttackRatio.Value;
			}
		}

		public Formation MainFormation
		{
			get
			{
				return this._mainFormation.Value;
			}
		}

		public float MainFormationReliabilityFactor
		{
			get
			{
				return this._mainFormationReliabilityFactor.Value;
			}
		}

		public Vec2 WeightedAverageEnemyPosition
		{
			get
			{
				return this._weightedAverageEnemyPosition.Value;
			}
		}

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

		public Vec2 HighGroundCloseToForeseenBattleGround
		{
			get
			{
				return this._highGroundCloseToForeseenBattleGround.Value;
			}
		}

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

		public void ForceExpireCavalryUnitRatio()
		{
			this._cavalryUnitRatio.Expire();
		}

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

		private void InitializeTelemetryScopeNames()
		{
		}

		public Vec2 GetAveragePositionWithMaxAge(float age)
		{
			return this._averagePosition.GetCachedValueWithMaxAge(age);
		}

		public float GetClassWeightedFactor(float infantryWeight, float rangedWeight, float cavalryWeight, float rangedCavalryWeight)
		{
			return this.InfantryUnitRatio * infantryWeight + this.RangedUnitRatio * rangedWeight + this.CavalryUnitRatio * cavalryWeight + this.RangedCavalryUnitRatio * rangedCavalryWeight;
		}

		public readonly Formation Formation;

		private readonly QueryData<float> _formationPower;

		private readonly QueryData<float> _formationMeleeFightingPower;

		private readonly QueryData<Vec2> _averagePosition;

		private readonly QueryData<Vec2> _currentVelocity;

		private float _lastAveragePositionCalculateTime;

		private readonly QueryData<Vec2> _estimatedDirection;

		private readonly QueryData<float> _estimatedInterval;

		private readonly QueryData<WorldPosition> _medianPosition;

		private readonly QueryData<Vec2> _averageAllyPosition;

		private readonly QueryData<float> _idealAverageDisplacement;

		private readonly QueryData<float> _formationDispersedness;

		private readonly QueryData<FormationQuerySystem.FormationIntegrityDataGroup> _formationIntegrityData;

		private readonly QueryData<MBList<Agent>> _localAllyUnits;

		private readonly QueryData<MBList<Agent>> _localEnemyUnits;

		private readonly QueryData<FormationClass> _mainClass;

		private readonly QueryData<float> _infantryUnitRatio;

		private readonly QueryData<float> _hasShieldUnitRatio;

		private readonly QueryData<float> _rangedUnitRatio;

		private readonly QueryData<int> _insideCastleUnitCountIncludingUnpositioned;

		private readonly QueryData<int> _insideCastleUnitCountPositioned;

		private readonly QueryData<float> _cavalryUnitRatio;

		private readonly QueryData<float> _rangedCavalryUnitRatio;

		private readonly QueryData<bool> _isMeleeFormation;

		private readonly QueryData<bool> _isInfantryFormation;

		private readonly QueryData<bool> _hasShield;

		private readonly QueryData<bool> _isRangedFormation;

		private readonly QueryData<bool> _isCavalryFormation;

		private readonly QueryData<bool> _isRangedCavalryFormation;

		private readonly QueryData<float> _movementSpeedMaximum;

		private readonly QueryData<float> _movementSpeed;

		private readonly QueryData<float> _maximumMissileRange;

		private readonly QueryData<float> _missileRangeAdjusted;

		private readonly QueryData<float> _localInfantryUnitRatio;

		private readonly QueryData<float> _localRangedUnitRatio;

		private readonly QueryData<float> _localCavalryUnitRatio;

		private readonly QueryData<float> _localRangedCavalryUnitRatio;

		private readonly QueryData<float> _localAllyPower;

		private readonly QueryData<float> _localEnemyPower;

		private readonly QueryData<float> _localPowerRatio;

		private readonly QueryData<float> _casualtyRatio;

		private readonly QueryData<bool> _isUnderRangedAttack;

		private readonly QueryData<float> _underRangedAttackRatio;

		private readonly QueryData<float> _makingRangedAttackRatio;

		private readonly QueryData<Formation> _mainFormation;

		private readonly QueryData<float> _mainFormationReliabilityFactor;

		private readonly QueryData<Vec2> _weightedAverageEnemyPosition;

		private readonly QueryData<Formation> _closestEnemyFormation;

		private readonly QueryData<Formation> _closestSignificantlyLargeEnemyFormation;

		private readonly QueryData<Formation> _fastestSignificantlyLargeEnemyFormation;

		private readonly QueryData<Vec2> _highGroundCloseToForeseenBattleGround;

		public struct FormationIntegrityDataGroup
		{
			public Vec2 AverageVelocityExcludeFarAgents;

			public float DeviationOfPositionsExcludeFarAgents;

			public float AverageMaxUnlimitedSpeedExcludeFarAgents;
		}
	}
}
