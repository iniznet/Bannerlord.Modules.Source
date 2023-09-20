using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200035A RID: 858
	public abstract class SiegeWeapon : UsableMachine, ITargetable
	{
		// Token: 0x1700086B RID: 2155
		// (get) Token: 0x06002EBD RID: 11965 RVA: 0x000BD545 File Offset: 0x000BB745
		// (set) Token: 0x06002EBE RID: 11966 RVA: 0x000BD54D File Offset: 0x000BB74D
		[EditorVisibleScriptComponentVariable(false)]
		public bool ForcedUse { get; private set; }

		// Token: 0x1700086C RID: 2156
		// (get) Token: 0x06002EBF RID: 11967 RVA: 0x000BD558 File Offset: 0x000BB758
		public bool IsUsed
		{
			get
			{
				using (List<Formation>.Enumerator enumerator = base.UserFormations.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.Team.Side == this.Side)
						{
							return true;
						}
					}
				}
				return false;
			}
		}

		// Token: 0x06002EC0 RID: 11968 RVA: 0x000BD5BC File Offset: 0x000BB7BC
		public void SetForcedUse(bool value)
		{
			this.ForcedUse = value;
		}

		// Token: 0x1700086D RID: 2157
		// (get) Token: 0x06002EC1 RID: 11969 RVA: 0x000BD5C5 File Offset: 0x000BB7C5
		public virtual BattleSideEnum Side
		{
			get
			{
				return BattleSideEnum.Attacker;
			}
		}

		// Token: 0x06002EC2 RID: 11970
		public abstract SiegeEngineType GetSiegeEngineType();

		// Token: 0x06002EC3 RID: 11971 RVA: 0x000BD5C8 File Offset: 0x000BB7C8
		protected virtual bool CalculateIsSufficientlyManned(BattleSideEnum battleSide)
		{
			if (this.GetDetachmentWeightAux(battleSide) < 1f)
			{
				return true;
			}
			foreach (Team team in Mission.Current.Teams)
			{
				if (team.Side == this.Side)
				{
					foreach (Formation formation in team.FormationsIncludingSpecialAndEmpty)
					{
						if (formation.CountOfUnits > 0 && base.IsUsedByFormation(formation) && (formation.Arrangement.UnitCount > 1 || (formation.Arrangement.UnitCount > 0 && !formation.HasPlayerControlledTroop)))
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		// Token: 0x06002EC4 RID: 11972 RVA: 0x000BD6B8 File Offset: 0x000BB8B8
		private bool HasNewMovingAgents()
		{
			foreach (StandingPoint standingPoint in base.StandingPoints)
			{
				if (standingPoint.HasAIMovingTo && standingPoint.PreviousUserAgent != standingPoint.MovingAgent)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06002EC5 RID: 11973 RVA: 0x000BD724 File Offset: 0x000BB924
		protected internal override void OnInit()
		{
			base.OnInit();
			this.ForcedUse = true;
			this._potentialUsingFormations = new List<Formation>();
			this._forcedUseFormations = new List<Formation>();
			base.GameEntity.SetAnimationSoundActivation(true);
			this._removeOnDeployEntities = Mission.Current.Scene.FindEntitiesWithTag(this.RemoveOnDeployTag).ToList<GameEntity>();
			this._addOnDeployEntities = Mission.Current.Scene.FindEntitiesWithTag(this.AddOnDeployTag).ToList<GameEntity>();
			foreach (StandingPoint standingPoint in base.StandingPoints)
			{
				if (!(standingPoint is StandingPointWithWeaponRequirement))
				{
					standingPoint.AutoEquipWeaponsOnUseStopped = true;
				}
			}
			base.SetScriptComponentToTick(this.GetTickRequirement());
			GameEntity gameEntity = base.GameEntity.CollectChildrenEntitiesWithTag("targeting_entity").FirstOrDefault<GameEntity>();
			this._targetingPositionOffset = ((gameEntity != null) ? new Vec3?(gameEntity.GlobalPosition) : null) - (base.GameEntity.GlobalBoxMax + base.GameEntity.GlobalBoxMin) * 0.5f;
			GameEntity gameEntity2 = base.GameEntity.CollectChildrenEntitiesWithTag("targeting_entity").FirstOrDefault<GameEntity>();
			this._targetingPositionOffset = ((gameEntity2 != null) ? new Vec3?(gameEntity2.GlobalPosition) : null) - (base.GameEntity.GlobalBoxMax + base.GameEntity.GlobalBoxMin) * 0.5f;
			this.EnemyRangeToStopUsing = 5f;
		}

		// Token: 0x06002EC6 RID: 11974 RVA: 0x000BD90C File Offset: 0x000BBB0C
		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			if (base.GameEntity.IsVisibleIncludeParents() && !GameNetwork.IsClientOrReplay)
			{
				return base.GetTickRequirement() | ScriptComponentBehavior.TickRequirement.Tick | ScriptComponentBehavior.TickRequirement.TickParallel;
			}
			return base.GetTickRequirement();
		}

		// Token: 0x06002EC7 RID: 11975 RVA: 0x000BD934 File Offset: 0x000BBB34
		private void TickAux(bool isParallel)
		{
			if (!GameNetwork.IsClientOrReplay && base.GameEntity.IsVisibleIncludeParents())
			{
				if (this.IsDisabledForBattleSide(this.Side))
				{
					using (List<StandingPoint>.Enumerator enumerator = base.StandingPoints.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							StandingPoint standingPoint = enumerator.Current;
							Agent userAgent = standingPoint.UserAgent;
							if (userAgent != null && !userAgent.IsPlayerControlled && userAgent.Formation != null && userAgent.Formation.Team.Side == this.Side)
							{
								if (isParallel)
								{
									this._needsSingleThreadTickOnce = true;
								}
								else
								{
									userAgent.Formation.StopUsingMachine(this, false);
									this._forcedUseFormations.Remove(userAgent.Formation);
									this._isValidated = false;
								}
							}
						}
						return;
					}
				}
				if (this.ForcedUse)
				{
					bool flag = false;
					foreach (Team team in Mission.Current.Teams)
					{
						if (team.Side == this.Side)
						{
							if (!this.CalculateIsSufficientlyManned(team.Side))
							{
								foreach (Formation formation in team.FormationsIncludingSpecialAndEmpty)
								{
									if (formation.CountOfUnits > 0 && formation.GetReadonlyMovementOrderReference().OrderEnum != MovementOrder.MovementOrderEnum.Retreat && (formation.Arrangement.UnitCount > 1 || (formation.Arrangement.UnitCount > 0 && !formation.HasPlayerControlledTroop)) && !formation.Detachments.Contains(this))
									{
										if (isParallel)
										{
											this._needsSingleThreadTickOnce = true;
										}
										else
										{
											this._potentialUsingFormations.Add(formation);
										}
									}
								}
								this._areMovingAgentsProcessed = false;
							}
							else if (this.HasNewMovingAgents())
							{
								if (!this._areMovingAgentsProcessed)
								{
									float num = float.MaxValue;
									Formation formation2 = null;
									foreach (Formation formation3 in team.FormationsIncludingSpecialAndEmpty)
									{
										if (formation3.CountOfUnits > 0 && formation3.GetReadonlyMovementOrderReference().OrderEnum != MovementOrder.MovementOrderEnum.Retreat && (formation3.Arrangement.UnitCount > 1 || (formation3.Arrangement.UnitCount > 0 && !formation3.HasPlayerControlledTroop)))
										{
											WorldPosition medianPosition = formation3.QuerySystem.MedianPosition;
											Vec3 vec = base.GameEntity.GlobalPosition;
											float num2 = medianPosition.DistanceSquaredWithLimit(vec, 10000f);
											if (num2 < num)
											{
												num = num2;
												formation2 = formation3;
											}
										}
									}
									if (formation2 != null && !base.IsUsedByFormation(formation2))
									{
										if (isParallel)
										{
											this._needsSingleThreadTickOnce = true;
										}
										else
										{
											this._potentialUsingFormations.Clear();
											this._potentialUsingFormations.Add(formation2);
											flag = true;
											this._areMovingAgentsProcessed = true;
										}
									}
									else
									{
										this._areMovingAgentsProcessed = true;
									}
								}
							}
							else
							{
								this._areMovingAgentsProcessed = false;
							}
							if (flag)
							{
								this._potentialUsingFormations[0].StartUsingMachine(this, !this._potentialUsingFormations[0].IsAIControlled);
								this._forcedUseFormations.Add(this._potentialUsingFormations[0]);
								this._potentialUsingFormations.Clear();
								this._isValidated = false;
								flag = false;
							}
							else if (this._potentialUsingFormations.Count > 0)
							{
								float num3 = float.MaxValue;
								Formation formation4 = null;
								foreach (Formation formation5 in this._potentialUsingFormations)
								{
									Vec2 averagePosition = formation5.QuerySystem.AveragePosition;
									Vec3 vec = base.GameEntity.GlobalPosition;
									float num4 = averagePosition.DistanceSquared(vec.AsVec2);
									if (num4 < num3)
									{
										num3 = num4;
										formation4 = formation5;
									}
								}
								int count = base.StandingPoints.Count;
								int num5 = 0;
								Formation formation6 = null;
								Vec2 vec2 = Vec2.Zero;
								for (int i = 0; i < count; i++)
								{
									Agent previousUserAgent = base.StandingPoints[i].PreviousUserAgent;
									if (previousUserAgent != null)
									{
										if (!previousUserAgent.IsActive() || previousUserAgent.Formation == null || (formation6 != null && previousUserAgent.Formation != formation6))
										{
											num5 = -1;
											break;
										}
										num5++;
										Vec2 vec3 = vec2;
										Vec3 vec = previousUserAgent.Position;
										vec2 = vec3 + vec.AsVec2;
										formation6 = previousUserAgent.Formation;
									}
								}
								Formation formation7 = formation4;
								if (num5 > 0 && this._potentialUsingFormations.Contains(formation6))
								{
									vec2 *= 1f / (float)num5;
									Vec3 vec = base.GameEntity.GlobalPosition;
									if (vec2.DistanceSquared(vec.AsVec2) < num3)
									{
										formation7 = formation6;
									}
								}
								formation7.StartUsingMachine(this, !formation7.IsAIControlled);
								this._forcedUseFormations.Add(formation7);
								this._potentialUsingFormations.Clear();
								this._isValidated = false;
							}
							else if (!this._isValidated)
							{
								if (this.GetDetachmentWeightAux(team.Side) == -3.4028235E+38f)
								{
									for (int j = this._forcedUseFormations.Count - 1; j >= 0; j--)
									{
										Formation formation8 = this._forcedUseFormations[j];
										if (formation8.Team.Side == this.Side)
										{
											bool flag2 = false;
											foreach (StandingPoint standingPoint2 in base.StandingPoints)
											{
												if (standingPoint2.UserAgent != null && standingPoint2.UserAgent.Formation == formation8)
												{
													flag2 = true;
													break;
												}
											}
											if (!flag2)
											{
												if (isParallel)
												{
													if (base.IsUsedByFormation(formation8))
													{
														this._needsSingleThreadTickOnce = true;
														break;
													}
													this._forcedUseFormations.Remove(formation8);
												}
												else
												{
													if (base.IsUsedByFormation(formation8))
													{
														formation8.StopUsingMachine(this, !formation8.IsAIControlled);
													}
													this._forcedUseFormations.Remove(formation8);
												}
											}
										}
									}
									if (isParallel && this._needsSingleThreadTickOnce)
									{
										break;
									}
								}
								if (!isParallel)
								{
									this._isValidated = true;
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06002EC8 RID: 11976 RVA: 0x000BDFDC File Offset: 0x000BC1DC
		protected internal override void OnTickParallel(float dt)
		{
			this.TickAux(true);
		}

		// Token: 0x06002EC9 RID: 11977 RVA: 0x000BDFE5 File Offset: 0x000BC1E5
		protected internal override void OnTick(float dt)
		{
			base.OnTick(dt);
			if (this._needsSingleThreadTickOnce)
			{
				this._needsSingleThreadTickOnce = false;
				this.TickAux(false);
			}
		}

		// Token: 0x06002ECA RID: 11978 RVA: 0x000BE004 File Offset: 0x000BC204
		protected internal virtual void OnDeploymentStateChanged(bool isDeployed)
		{
			foreach (GameEntity gameEntity in this._removeOnDeployEntities)
			{
				gameEntity.SetVisibilityExcludeParents(!isDeployed);
				StrategicArea firstScriptOfType = gameEntity.GetFirstScriptOfType<StrategicArea>();
				if (firstScriptOfType != null)
				{
					firstScriptOfType.OnParentGameEntityVisibilityChanged(!isDeployed);
				}
				else
				{
					foreach (StrategicArea strategicArea in from c in gameEntity.GetChildren()
						where c.HasScriptOfType<StrategicArea>()
						select c.GetFirstScriptOfType<StrategicArea>())
					{
						strategicArea.OnParentGameEntityVisibilityChanged(!isDeployed);
					}
				}
			}
			foreach (GameEntity gameEntity2 in this._addOnDeployEntities)
			{
				gameEntity2.SetVisibilityExcludeParents(isDeployed);
				MissionObject firstScriptOfType2 = gameEntity2.GetFirstScriptOfType<MissionObject>();
				if (firstScriptOfType2 != null)
				{
					firstScriptOfType2.SetAbilityOfFaces(isDeployed);
				}
				StrategicArea firstScriptOfType3 = gameEntity2.GetFirstScriptOfType<StrategicArea>();
				if (firstScriptOfType3 != null)
				{
					firstScriptOfType3.OnParentGameEntityVisibilityChanged(isDeployed);
				}
				else
				{
					foreach (StrategicArea strategicArea2 in from c in gameEntity2.GetChildren()
						where c.HasScriptOfType<StrategicArea>()
						select c.GetFirstScriptOfType<StrategicArea>())
					{
						strategicArea2.OnParentGameEntityVisibilityChanged(isDeployed);
					}
				}
			}
			if (this._addOnDeployEntities.Count > 0 || this._removeOnDeployEntities.Count > 0)
			{
				foreach (StandingPoint standingPoint in base.StandingPoints)
				{
					standingPoint.RefreshGameEntityWithWorldPosition();
				}
			}
		}

		// Token: 0x1700086E RID: 2158
		// (get) Token: 0x06002ECB RID: 11979 RVA: 0x000BE250 File Offset: 0x000BC450
		public override bool HasWaitFrame
		{
			get
			{
				return base.HasWaitFrame && (!(this is IPrimarySiegeWeapon) || !(this as IPrimarySiegeWeapon).HasCompletedAction());
			}
		}

		// Token: 0x1700086F RID: 2159
		// (get) Token: 0x06002ECC RID: 11980 RVA: 0x000BE274 File Offset: 0x000BC474
		public override bool IsDeactivated
		{
			get
			{
				return base.IsDisabled || base.GameEntity == null || !base.GameEntity.IsVisibleIncludeParents() || base.IsDeactivated;
			}
		}

		// Token: 0x06002ECD RID: 11981 RVA: 0x000BE2A1 File Offset: 0x000BC4A1
		public override bool ShouldAutoLeaveDetachmentWhenDisabled(BattleSideEnum sideEnum)
		{
			return this.AutoAttachUserToFormation(sideEnum);
		}

		// Token: 0x06002ECE RID: 11982 RVA: 0x000BE2AA File Offset: 0x000BC4AA
		public override bool AutoAttachUserToFormation(BattleSideEnum sideEnum)
		{
			return !base.IsDisabledDueToEnemyInRange(sideEnum);
		}

		// Token: 0x06002ECF RID: 11983 RVA: 0x000BE2B6 File Offset: 0x000BC4B6
		public override bool HasToBeDefendedByUser(BattleSideEnum sideEnum)
		{
			return base.IsDisabledDueToEnemyInRange(sideEnum);
		}

		// Token: 0x06002ED0 RID: 11984 RVA: 0x000BE2C0 File Offset: 0x000BC4C0
		protected float GetUserMultiplierOfWeapon()
		{
			int userCountIncludingInStruckAction = base.UserCountIncludingInStruckAction;
			if (userCountIncludingInStruckAction == 0)
			{
				return 0.1f;
			}
			return 0.7f + 0.3f * (float)userCountIncludingInStruckAction / (float)this.MaxUserCount;
		}

		// Token: 0x06002ED1 RID: 11985 RVA: 0x000BE2F3 File Offset: 0x000BC4F3
		protected virtual float GetDistanceMultiplierOfWeapon(Vec3 weaponPos)
		{
			if (this.GetMinimumDistanceBetweenPositions(weaponPos) > 20f)
			{
				return 0.4f;
			}
			Debug.FailedAssert("Invalid weapon type", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Objects\\Siege\\SiegeWeapon.cs", "GetDistanceMultiplierOfWeapon", 541);
			return 1f;
		}

		// Token: 0x06002ED2 RID: 11986 RVA: 0x000BE328 File Offset: 0x000BC528
		protected virtual float GetMinimumDistanceBetweenPositions(Vec3 position)
		{
			return base.GameEntity.GlobalPosition.DistanceSquared(position);
		}

		// Token: 0x06002ED3 RID: 11987 RVA: 0x000BE34C File Offset: 0x000BC54C
		protected float GetHitPointMultiplierOfWeapon()
		{
			if (base.DestructionComponent != null)
			{
				return MathF.Max(1f, 2f - MathF.Log10(base.DestructionComponent.HitPoint / base.DestructionComponent.MaxHitPoint * 10f + 1f));
			}
			return 1f;
		}

		// Token: 0x06002ED4 RID: 11988 RVA: 0x000BE39F File Offset: 0x000BC59F
		public GameEntity GetTargetEntity()
		{
			return base.GameEntity;
		}

		// Token: 0x06002ED5 RID: 11989 RVA: 0x000BE3A7 File Offset: 0x000BC5A7
		public Vec3 GetTargetingOffset()
		{
			if (this._targetingPositionOffset != null)
			{
				return this._targetingPositionOffset.Value;
			}
			return Vec3.Zero;
		}

		// Token: 0x06002ED6 RID: 11990 RVA: 0x000BE3C7 File Offset: 0x000BC5C7
		public BattleSideEnum GetSide()
		{
			return this.Side;
		}

		// Token: 0x06002ED7 RID: 11991 RVA: 0x000BE3CF File Offset: 0x000BC5CF
		public GameEntity Entity()
		{
			return base.GameEntity;
		}

		// Token: 0x06002ED8 RID: 11992
		public abstract TargetFlags GetTargetFlags();

		// Token: 0x06002ED9 RID: 11993
		public abstract float GetTargetValue(List<Vec3> weaponPos);

		// Token: 0x0400131E RID: 4894
		private const string TargetingEntityTag = "targeting_entity";

		// Token: 0x0400131F RID: 4895
		[EditableScriptComponentVariable(true)]
		internal string RemoveOnDeployTag = "";

		// Token: 0x04001320 RID: 4896
		[EditableScriptComponentVariable(true)]
		internal string AddOnDeployTag = "";

		// Token: 0x04001321 RID: 4897
		private List<GameEntity> _addOnDeployEntities;

		// Token: 0x04001323 RID: 4899
		protected bool _spawnedFromSpawner;

		// Token: 0x04001324 RID: 4900
		private List<GameEntity> _removeOnDeployEntities;

		// Token: 0x04001325 RID: 4901
		private List<Formation> _potentialUsingFormations;

		// Token: 0x04001326 RID: 4902
		private List<Formation> _forcedUseFormations;

		// Token: 0x04001327 RID: 4903
		private bool _needsSingleThreadTickOnce;

		// Token: 0x04001328 RID: 4904
		private bool _areMovingAgentsProcessed;

		// Token: 0x04001329 RID: 4905
		private bool _isValidated;

		// Token: 0x0400132A RID: 4906
		private Vec3? _targetingPositionOffset;
	}
}
