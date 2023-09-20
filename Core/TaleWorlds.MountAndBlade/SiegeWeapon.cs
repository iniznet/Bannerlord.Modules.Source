using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public abstract class SiegeWeapon : UsableMachine, ITargetable
	{
		[EditorVisibleScriptComponentVariable(false)]
		public bool ForcedUse { get; private set; }

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

		public void SetForcedUse(bool value)
		{
			this.ForcedUse = value;
		}

		public virtual BattleSideEnum Side
		{
			get
			{
				return BattleSideEnum.Attacker;
			}
		}

		public abstract SiegeEngineType GetSiegeEngineType();

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
			this._targetingPositionOffset = ((gameEntity != null) ? new Vec3?(gameEntity.GlobalPosition) : null) - (base.GameEntity.PhysicsGlobalBoxMax + base.GameEntity.PhysicsGlobalBoxMin) * 0.5f;
			GameEntity gameEntity2 = base.GameEntity.CollectChildrenEntitiesWithTag("targeting_entity").FirstOrDefault<GameEntity>();
			this._targetingPositionOffset = ((gameEntity2 != null) ? new Vec3?(gameEntity2.GlobalPosition) : null) - (base.GameEntity.PhysicsGlobalBoxMax + base.GameEntity.PhysicsGlobalBoxMin) * 0.5f;
			this.EnemyRangeToStopUsing = 5f;
		}

		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			if (base.GameEntity.IsVisibleIncludeParents() && !GameNetwork.IsClientOrReplay)
			{
				return base.GetTickRequirement() | ScriptComponentBehavior.TickRequirement.Tick | ScriptComponentBehavior.TickRequirement.TickParallel;
			}
			return base.GetTickRequirement();
		}

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
								if (!this.HasToBeDefendedByUser(team.Side) && this.GetDetachmentWeightAux(team.Side) == -3.4028235E+38f)
								{
									for (int j = this._forcedUseFormations.Count - 1; j >= 0; j--)
									{
										Formation formation8 = this._forcedUseFormations[j];
										if (formation8.Team.Side == this.Side && !this.IsAnyUserBelongsToFormation(formation8))
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

		protected virtual bool IsAnyUserBelongsToFormation(Formation formation)
		{
			foreach (StandingPoint standingPoint in base.StandingPoints)
			{
				if (standingPoint.UserAgent != null && standingPoint.UserAgent.Formation == formation)
				{
					return true;
				}
			}
			return false;
		}

		protected internal override void OnTickParallel(float dt)
		{
			this.TickAux(true);
		}

		protected internal override void OnTick(float dt)
		{
			base.OnTick(dt);
			if (this._needsSingleThreadTickOnce)
			{
				this._needsSingleThreadTickOnce = false;
				this.TickAux(false);
			}
		}

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

		public override bool HasWaitFrame
		{
			get
			{
				return base.HasWaitFrame && (!(this is IPrimarySiegeWeapon) || !(this as IPrimarySiegeWeapon).HasCompletedAction());
			}
		}

		public override bool IsDeactivated
		{
			get
			{
				return base.IsDisabled || base.GameEntity == null || !base.GameEntity.IsVisibleIncludeParents() || base.IsDeactivated;
			}
		}

		public override bool ShouldAutoLeaveDetachmentWhenDisabled(BattleSideEnum sideEnum)
		{
			return this.AutoAttachUserToFormation(sideEnum);
		}

		public override bool AutoAttachUserToFormation(BattleSideEnum sideEnum)
		{
			return base.Ai.HasActionCompleted || !base.IsDisabledDueToEnemyInRange(sideEnum);
		}

		public override bool HasToBeDefendedByUser(BattleSideEnum sideEnum)
		{
			return !base.Ai.HasActionCompleted && base.IsDisabledDueToEnemyInRange(sideEnum);
		}

		protected float GetUserMultiplierOfWeapon()
		{
			int userCountIncludingInStruckAction = base.UserCountIncludingInStruckAction;
			if (userCountIncludingInStruckAction == 0)
			{
				return 0.1f;
			}
			return 0.7f + 0.3f * (float)userCountIncludingInStruckAction / (float)this.MaxUserCount;
		}

		protected virtual float GetDistanceMultiplierOfWeapon(Vec3 weaponPos)
		{
			if (this.GetMinimumDistanceBetweenPositions(weaponPos) > 20f)
			{
				return 0.4f;
			}
			Debug.FailedAssert("Invalid weapon type", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Objects\\Siege\\SiegeWeapon.cs", "GetDistanceMultiplierOfWeapon", 544);
			return 1f;
		}

		protected virtual float GetMinimumDistanceBetweenPositions(Vec3 position)
		{
			return base.GameEntity.GlobalPosition.DistanceSquared(position);
		}

		protected float GetHitPointMultiplierOfWeapon()
		{
			if (base.DestructionComponent != null)
			{
				return MathF.Max(1f, 2f - MathF.Log10(base.DestructionComponent.HitPoint / base.DestructionComponent.MaxHitPoint * 10f + 1f));
			}
			return 1f;
		}

		public GameEntity GetTargetEntity()
		{
			return base.GameEntity;
		}

		public Vec3 GetTargetingOffset()
		{
			if (this._targetingPositionOffset != null)
			{
				return this._targetingPositionOffset.Value;
			}
			return Vec3.Zero;
		}

		public BattleSideEnum GetSide()
		{
			return this.Side;
		}

		public GameEntity Entity()
		{
			return base.GameEntity;
		}

		public abstract TargetFlags GetTargetFlags();

		public abstract float GetTargetValue(List<Vec3> weaponPos);

		private const string TargetingEntityTag = "targeting_entity";

		[EditableScriptComponentVariable(true)]
		internal string RemoveOnDeployTag = "";

		[EditableScriptComponentVariable(true)]
		internal string AddOnDeployTag = "";

		private List<GameEntity> _addOnDeployEntities;

		protected bool _spawnedFromSpawner;

		private List<GameEntity> _removeOnDeployEntities;

		private List<Formation> _potentialUsingFormations;

		private List<Formation> _forcedUseFormations;

		private bool _needsSingleThreadTickOnce;

		private bool _areMovingAgentsProcessed;

		private bool _isValidated;

		private Vec3? _targetingPositionOffset;
	}
}
