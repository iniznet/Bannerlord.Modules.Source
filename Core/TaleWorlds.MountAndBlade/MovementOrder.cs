using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public struct MovementOrder
	{
		public Formation TargetFormation { get; private set; }

		public Agent _targetAgent { get; }

		public OrderType OrderType
		{
			get
			{
				switch (this.OrderEnum)
				{
				case MovementOrder.MovementOrderEnum.AttackEntity:
					return OrderType.AttackEntity;
				case MovementOrder.MovementOrderEnum.Charge:
					return OrderType.Charge;
				case MovementOrder.MovementOrderEnum.ChargeToTarget:
					return OrderType.ChargeWithTarget;
				case MovementOrder.MovementOrderEnum.Follow:
					return OrderType.FollowMe;
				case MovementOrder.MovementOrderEnum.FollowEntity:
					return OrderType.FollowEntity;
				case MovementOrder.MovementOrderEnum.Guard:
					return OrderType.GuardMe;
				case MovementOrder.MovementOrderEnum.Move:
					return OrderType.Move;
				case MovementOrder.MovementOrderEnum.Retreat:
					return OrderType.Retreat;
				case MovementOrder.MovementOrderEnum.Stop:
					return OrderType.StandYourGround;
				case MovementOrder.MovementOrderEnum.Advance:
					return OrderType.Advance;
				case MovementOrder.MovementOrderEnum.FallBack:
					return OrderType.FallBack;
				default:
					Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\AI\\Orders\\MovementOrder.cs", "OrderType", 113);
					return OrderType.Move;
				}
			}
		}

		public MovementOrder.MovementStateEnum MovementState
		{
			get
			{
				switch (this.OrderEnum)
				{
				case MovementOrder.MovementOrderEnum.Charge:
				case MovementOrder.MovementOrderEnum.ChargeToTarget:
				case MovementOrder.MovementOrderEnum.Guard:
					return MovementOrder.MovementStateEnum.Charge;
				case MovementOrder.MovementOrderEnum.Retreat:
					return MovementOrder.MovementStateEnum.Retreat;
				case MovementOrder.MovementOrderEnum.Stop:
					return MovementOrder.MovementStateEnum.StandGround;
				}
				return MovementOrder.MovementStateEnum.Hold;
			}
		}

		private MovementOrder(MovementOrder.MovementOrderEnum orderEnum)
		{
			this.OrderEnum = orderEnum;
			if (orderEnum != MovementOrder.MovementOrderEnum.Charge)
			{
				switch (orderEnum)
				{
				case MovementOrder.MovementOrderEnum.Retreat:
					this._positionLambda = null;
					goto IL_50;
				case MovementOrder.MovementOrderEnum.Advance:
					this._positionLambda = null;
					goto IL_50;
				case MovementOrder.MovementOrderEnum.FallBack:
					this._positionLambda = null;
					goto IL_50;
				}
				this._positionLambda = null;
			}
			else
			{
				this._positionLambda = null;
			}
			IL_50:
			this.TargetFormation = null;
			this.TargetEntity = null;
			this._targetAgent = null;
			this._tickTimer = new Timer(Mission.Current.CurrentTime, 0.5f, true);
			this._lastPosition = WorldPosition.Invalid;
			this._isFacingDirection = false;
			this._position = WorldPosition.Invalid;
			this._getPositionResultCache = WorldPosition.Invalid;
			this._getPositionFirstSectionCache = WorldPosition.Invalid;
			this._getPositionIsNavmeshlessCache = false;
			this._followState = MovementOrder.FollowState.Stop;
			this._departStartTime = -1f;
		}

		private MovementOrder(MovementOrder.MovementOrderEnum orderEnum, Formation targetFormation)
		{
			this.OrderEnum = orderEnum;
			this._positionLambda = null;
			this.TargetFormation = targetFormation;
			this.TargetEntity = null;
			this._targetAgent = null;
			this._tickTimer = new Timer(Mission.Current.CurrentTime, 0.5f, true);
			this._lastPosition = WorldPosition.Invalid;
			this._isFacingDirection = false;
			this._position = WorldPosition.Invalid;
			this._getPositionResultCache = WorldPosition.Invalid;
			this._getPositionFirstSectionCache = WorldPosition.Invalid;
			this._getPositionIsNavmeshlessCache = false;
			this._followState = MovementOrder.FollowState.Stop;
			this._departStartTime = -1f;
		}

		private WorldPosition ComputeAttackEntityWaitPosition(Formation formation, GameEntity targetEntity)
		{
			Scene scene = formation.Team.Mission.Scene;
			WorldPosition worldPosition = new WorldPosition(scene, UIntPtr.Zero, targetEntity.GlobalPosition, false);
			Vec2 vec = formation.QuerySystem.AveragePosition - worldPosition.AsVec2;
			MatrixFrame matrixFrame = targetEntity.GetGlobalFrame();
			Vec2 vec2 = matrixFrame.rotation.f.AsVec2.Normalized();
			Vec2 vec3 = ((vec.DotProduct(vec2) >= 0f) ? vec2 : (-vec2));
			WorldPosition worldPosition2 = worldPosition;
			worldPosition2.SetVec2(worldPosition.AsVec2 + vec3 * 3f);
			if (scene.DoesPathExistBetweenPositions(worldPosition2, formation.QuerySystem.MedianPosition))
			{
				return worldPosition2;
			}
			WorldPosition worldPosition3 = worldPosition;
			worldPosition3.SetVec2(worldPosition.AsVec2 - vec3 * 3f);
			if (scene.DoesPathExistBetweenPositions(worldPosition3, formation.QuerySystem.MedianPosition))
			{
				return worldPosition3;
			}
			worldPosition3 = worldPosition;
			Vec2 asVec = worldPosition.AsVec2;
			matrixFrame = targetEntity.GetGlobalFrame();
			worldPosition3.SetVec2(asVec + matrixFrame.rotation.s.AsVec2.Normalized() * 3f);
			if (scene.DoesPathExistBetweenPositions(worldPosition3, formation.QuerySystem.MedianPosition))
			{
				return worldPosition3;
			}
			worldPosition3 = worldPosition;
			Vec2 asVec2 = worldPosition.AsVec2;
			matrixFrame = targetEntity.GetGlobalFrame();
			worldPosition3.SetVec2(asVec2 - matrixFrame.rotation.s.AsVec2.Normalized() * 3f);
			if (scene.DoesPathExistBetweenPositions(worldPosition3, formation.QuerySystem.MedianPosition))
			{
				return worldPosition3;
			}
			return worldPosition2;
		}

		private MovementOrder(MovementOrder.MovementOrderEnum orderEnum, GameEntity targetEntity, bool surroundEntity)
		{
			targetEntity.GetFirstScriptOfType<UsableMachine>();
			this.OrderEnum = orderEnum;
			this._positionLambda = delegate(Formation f)
			{
				WorldPosition worldPosition = new WorldPosition(Mission.Current.Scene, UIntPtr.Zero, targetEntity.GlobalPosition, false);
				Vec2 vec = f.QuerySystem.AveragePosition - worldPosition.AsVec2;
				MatrixFrame matrixFrame = targetEntity.GetGlobalFrame();
				Vec2 vec2 = matrixFrame.rotation.f.AsVec2.Normalized();
				Vec2 vec3 = ((vec.DotProduct(vec2) >= 0f) ? vec2 : (-vec2));
				WorldPosition worldPosition2 = worldPosition;
				worldPosition2.SetVec2(worldPosition.AsVec2 + vec3 * 3f);
				if (Mission.Current.Scene.DoesPathExistBetweenPositions(worldPosition2, f.QuerySystem.MedianPosition))
				{
					return worldPosition2;
				}
				WorldPosition worldPosition3 = worldPosition;
				worldPosition3.SetVec2(worldPosition.AsVec2 - vec3 * 3f);
				if (Mission.Current.Scene.DoesPathExistBetweenPositions(worldPosition3, f.QuerySystem.MedianPosition))
				{
					return worldPosition3;
				}
				worldPosition3 = worldPosition;
				Vec2 asVec = worldPosition.AsVec2;
				matrixFrame = targetEntity.GetGlobalFrame();
				worldPosition3.SetVec2(asVec + matrixFrame.rotation.s.AsVec2.Normalized() * 3f);
				if (Mission.Current.Scene.DoesPathExistBetweenPositions(worldPosition3, f.QuerySystem.MedianPosition))
				{
					return worldPosition3;
				}
				worldPosition3 = worldPosition;
				Vec2 asVec2 = worldPosition.AsVec2;
				matrixFrame = targetEntity.GetGlobalFrame();
				worldPosition3.SetVec2(asVec2 - matrixFrame.rotation.s.AsVec2.Normalized() * 3f);
				if (Mission.Current.Scene.DoesPathExistBetweenPositions(worldPosition3, f.QuerySystem.MedianPosition))
				{
					return worldPosition3;
				}
				return worldPosition2;
			};
			this.TargetEntity = targetEntity;
			this._tickTimer = new Timer(Mission.Current.CurrentTime, 0.5f, true);
			this.TargetFormation = null;
			this._targetAgent = null;
			this._lastPosition = WorldPosition.Invalid;
			this._isFacingDirection = false;
			this._position = WorldPosition.Invalid;
			this._getPositionResultCache = WorldPosition.Invalid;
			this._getPositionFirstSectionCache = WorldPosition.Invalid;
			this._getPositionIsNavmeshlessCache = false;
			this._followState = MovementOrder.FollowState.Stop;
			this._departStartTime = -1f;
		}

		private MovementOrder(MovementOrder.MovementOrderEnum orderEnum, Agent targetAgent)
		{
			this.OrderEnum = orderEnum;
			WorldPosition targetAgentPos = targetAgent.GetWorldPosition();
			if (orderEnum == MovementOrder.MovementOrderEnum.Follow)
			{
				this._positionLambda = delegate(Formation f)
				{
					WorldPosition targetAgentPos3 = targetAgentPos;
					targetAgentPos3.SetVec2(targetAgentPos3.AsVec2 - f.GetMiddleFrontUnitPositionOffset());
					return targetAgentPos3;
				};
			}
			else
			{
				this._positionLambda = delegate(Formation f)
				{
					WorldPosition targetAgentPos2 = targetAgentPos;
					targetAgentPos2.SetVec2(targetAgentPos2.AsVec2 - 4f * (f.QuerySystem.Team.MedianTargetFormationPosition.AsVec2 - targetAgentPos.AsVec2).Normalized());
					Vec2 asVec = targetAgentPos2.AsVec2;
					WorldPosition lastPosition = f.GetReadonlyMovementOrderReference()._lastPosition;
					if (asVec.DistanceSquared(lastPosition.AsVec2) > 6.25f)
					{
						return targetAgentPos2;
					}
					return f.GetReadonlyMovementOrderReference()._lastPosition;
				};
			}
			this._targetAgent = targetAgent;
			this.TargetFormation = null;
			this.TargetEntity = null;
			this._tickTimer = new Timer(targetAgent.Mission.CurrentTime, 0.5f, true);
			this._lastPosition = targetAgentPos;
			this._isFacingDirection = false;
			this._position = WorldPosition.Invalid;
			this._getPositionResultCache = WorldPosition.Invalid;
			this._getPositionFirstSectionCache = WorldPosition.Invalid;
			this._getPositionIsNavmeshlessCache = false;
			this._followState = MovementOrder.FollowState.Stop;
			this._departStartTime = -1f;
		}

		private MovementOrder(MovementOrder.MovementOrderEnum orderEnum, GameEntity targetEntity)
		{
			this.OrderEnum = orderEnum;
			this._positionLambda = delegate(Formation f)
			{
				WorldPosition worldPosition = new WorldPosition(Mission.Current.Scene, UIntPtr.Zero, targetEntity.GlobalPosition, false);
				worldPosition.SetVec2(worldPosition.AsVec2);
				return worldPosition;
			};
			this.TargetEntity = targetEntity;
			this.TargetFormation = null;
			this._targetAgent = null;
			this._tickTimer = new Timer(Mission.Current.CurrentTime, 0.5f, true);
			this._lastPosition = WorldPosition.Invalid;
			this._isFacingDirection = false;
			this._position = WorldPosition.Invalid;
			this._getPositionResultCache = WorldPosition.Invalid;
			this._getPositionFirstSectionCache = WorldPosition.Invalid;
			this._getPositionIsNavmeshlessCache = false;
			this._followState = MovementOrder.FollowState.Stop;
			this._departStartTime = -1f;
		}

		private MovementOrder(MovementOrder.MovementOrderEnum orderEnum, WorldPosition position)
		{
			this.OrderEnum = orderEnum;
			this._positionLambda = null;
			this._isFacingDirection = false;
			this.TargetFormation = null;
			this.TargetEntity = null;
			this._targetAgent = null;
			this._tickTimer = new Timer(Mission.Current.CurrentTime, 0.5f, true);
			this._lastPosition = WorldPosition.Invalid;
			this._position = position;
			this._getPositionResultCache = WorldPosition.Invalid;
			this._getPositionFirstSectionCache = WorldPosition.Invalid;
			this._getPositionIsNavmeshlessCache = false;
			this._followState = MovementOrder.FollowState.Stop;
			this._departStartTime = -1f;
		}

		public override bool Equals(object obj)
		{
			if (obj is MovementOrder)
			{
				MovementOrder movementOrder = (MovementOrder)obj;
				return (movementOrder) == this;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return (int)this.OrderEnum;
		}

		public static bool operator !=(in MovementOrder m, MovementOrder obj)
		{
			return m.OrderEnum != obj.OrderEnum;
		}

		public static bool operator ==(in MovementOrder m, MovementOrder obj)
		{
			return m.OrderEnum == obj.OrderEnum;
		}

		public static MovementOrder MovementOrderChargeToTarget(Formation targetFormation)
		{
			return new MovementOrder(MovementOrder.MovementOrderEnum.ChargeToTarget, targetFormation);
		}

		public static MovementOrder MovementOrderFollow(Agent targetAgent)
		{
			return new MovementOrder(MovementOrder.MovementOrderEnum.Follow, targetAgent);
		}

		public static MovementOrder MovementOrderGuard(Agent targetAgent)
		{
			return new MovementOrder(MovementOrder.MovementOrderEnum.Guard, targetAgent);
		}

		public static MovementOrder MovementOrderFollowEntity(GameEntity targetEntity)
		{
			return new MovementOrder(MovementOrder.MovementOrderEnum.FollowEntity, targetEntity);
		}

		public static MovementOrder MovementOrderMove(WorldPosition position)
		{
			return new MovementOrder(MovementOrder.MovementOrderEnum.Move, position);
		}

		public static MovementOrder MovementOrderAttackEntity(GameEntity targetEntity, bool surroundEntity)
		{
			return new MovementOrder(MovementOrder.MovementOrderEnum.AttackEntity, targetEntity, surroundEntity);
		}

		public static int GetMovementOrderDefensiveness(MovementOrder.MovementOrderEnum orderEnum)
		{
			if (orderEnum == MovementOrder.MovementOrderEnum.Charge || orderEnum == MovementOrder.MovementOrderEnum.ChargeToTarget)
			{
				return 0;
			}
			return 1;
		}

		public static int GetMovementOrderDefensivenessChange(MovementOrder.MovementOrderEnum previousOrderEnum, MovementOrder.MovementOrderEnum nextOrderEnum)
		{
			if (previousOrderEnum == MovementOrder.MovementOrderEnum.Charge || previousOrderEnum == MovementOrder.MovementOrderEnum.ChargeToTarget)
			{
				if (nextOrderEnum != MovementOrder.MovementOrderEnum.Charge && nextOrderEnum != MovementOrder.MovementOrderEnum.ChargeToTarget)
				{
					return 1;
				}
				return 0;
			}
			else
			{
				if (nextOrderEnum == MovementOrder.MovementOrderEnum.Charge || nextOrderEnum == MovementOrder.MovementOrderEnum.ChargeToTarget)
				{
					return -1;
				}
				return 0;
			}
		}

		private static void RetreatAux(Formation formation)
		{
			for (int i = formation.Detachments.Count - 1; i >= 0; i--)
			{
				formation.LeaveDetachment(formation.Detachments[i]);
			}
			formation.ApplyActionOnEachUnitViaBackupList(delegate(Agent agent)
			{
				if (agent.IsAIControlled)
				{
					agent.Retreat(true);
				}
			});
		}

		private static WorldPosition GetAlternatePositionForNavmeshlessOrOutOfBoundsPosition(Formation f, WorldPosition originalPosition)
		{
			float num = 1f;
			WorldPosition alternatePositionForNavmeshlessOrOutOfBoundsPosition = Mission.Current.GetAlternatePositionForNavmeshlessOrOutOfBoundsPosition(originalPosition.AsVec2 - f.QuerySystem.AveragePosition, originalPosition, ref num);
			FormationAI ai = f.AI;
			if (((ai != null) ? ai.ActiveBehavior : null) != null)
			{
				f.AI.ActiveBehavior.NavmeshlessTargetPositionPenalty = num;
			}
			return alternatePositionForNavmeshlessOrOutOfBoundsPosition;
		}

		private static void OnUnitJoinOrLeaveAux(Agent unit, Agent target, bool isJoining)
		{
			unit.SetGuardState(target, isJoining);
		}

		private void GetPositionAuxFollow(Formation f)
		{
			Vec2 vec = Vec2.Zero;
			if (this._followState != MovementOrder.FollowState.Move && this._targetAgent.MountAgent != null)
			{
				vec += f.Direction * -2f;
			}
			if (this._followState == MovementOrder.FollowState.Move && f.PhysicalClass.IsMounted())
			{
				vec += 2f * this._targetAgent.Velocity.AsVec2;
			}
			else if (this._followState == MovementOrder.FollowState.Move)
			{
				f.PhysicalClass.IsMounted();
			}
			WorldPosition worldPosition = this._targetAgent.GetWorldPosition();
			worldPosition.SetVec2(worldPosition.AsVec2 - f.GetMiddleFrontUnitPositionOffset() + vec);
			if (this._followState == MovementOrder.FollowState.Stop || this._followState == MovementOrder.FollowState.Depart)
			{
				float num = (f.PhysicalClass.IsMounted() ? 4f : 2.5f);
				if (Mission.Current.IsTeleportingAgents || worldPosition.AsVec2.DistanceSquared(this._lastPosition.AsVec2) > num * num)
				{
					this._lastPosition = worldPosition;
					return;
				}
			}
			else
			{
				this._lastPosition = worldPosition;
			}
		}

		public Vec2 GetPosition(Formation f)
		{
			return this.CreateNewOrderWorldPosition(f, WorldPosition.WorldPositionEnforcedCache.None).AsVec2;
		}

		public Vec2 GetTargetVelocity()
		{
			switch (this.OrderEnum)
			{
			case MovementOrder.MovementOrderEnum.AttackEntity:
			case MovementOrder.MovementOrderEnum.Charge:
			case MovementOrder.MovementOrderEnum.ChargeToTarget:
			case MovementOrder.MovementOrderEnum.FollowEntity:
			case MovementOrder.MovementOrderEnum.Move:
			case MovementOrder.MovementOrderEnum.Retreat:
			case MovementOrder.MovementOrderEnum.Stop:
			case MovementOrder.MovementOrderEnum.Advance:
			case MovementOrder.MovementOrderEnum.FallBack:
				return Vec2.Zero;
			case MovementOrder.MovementOrderEnum.Follow:
			case MovementOrder.MovementOrderEnum.Guard:
				return this._targetAgent.AverageVelocity.AsVec2;
			default:
				Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\AI\\Orders\\MovementOrder.cs", "GetTargetVelocity", 847);
				return Vec2.Zero;
			}
		}

		public WorldPosition CreateNewOrderWorldPosition(Formation f, WorldPosition.WorldPositionEnforcedCache worldPositionEnforcedCache)
		{
			if (!this.IsApplicable(f))
			{
				return f.CreateNewOrderWorldPosition(worldPositionEnforcedCache);
			}
			MovementOrder.MovementOrderEnum orderEnum = this.OrderEnum;
			WorldPosition worldPosition;
			if (orderEnum != MovementOrder.MovementOrderEnum.Follow)
			{
				if (orderEnum - MovementOrder.MovementOrderEnum.Advance > 1)
				{
					Func<Formation, WorldPosition> positionLambda = this._positionLambda;
					worldPosition = ((positionLambda != null) ? positionLambda(f) : this._position);
				}
				else
				{
					worldPosition = this.GetPositionAux(f, worldPositionEnforcedCache);
				}
			}
			else
			{
				this.GetPositionAuxFollow(f);
				worldPosition = this._lastPosition;
			}
			if (Mission.Current.Mode == MissionMode.Deployment && !Mission.Current.IsOrderPositionAvailable(worldPosition, f.Team))
			{
				worldPosition = f.CreateNewOrderWorldPosition(worldPositionEnforcedCache);
			}
			bool flag = false;
			if (this._getPositionFirstSectionCache.AsVec2 != worldPosition.AsVec2)
			{
				this._getPositionIsNavmeshlessCache = false;
				if (worldPosition.IsValid)
				{
					if (worldPositionEnforcedCache != WorldPosition.WorldPositionEnforcedCache.NavMeshVec3)
					{
						if (worldPositionEnforcedCache == WorldPosition.WorldPositionEnforcedCache.GroundVec3)
						{
							worldPosition.GetGroundVec3();
						}
					}
					else
					{
						worldPosition.GetNavMeshVec3();
					}
					this._getPositionFirstSectionCache = worldPosition;
					if (this.OrderEnum != MovementOrder.MovementOrderEnum.Follow && (worldPosition.GetNavMesh() == UIntPtr.Zero || !Mission.Current.IsPositionInsideBoundaries(worldPosition.AsVec2)))
					{
						worldPosition = MovementOrder.GetAlternatePositionForNavmeshlessOrOutOfBoundsPosition(f, worldPosition);
						if (worldPositionEnforcedCache != WorldPosition.WorldPositionEnforcedCache.NavMeshVec3)
						{
							if (worldPositionEnforcedCache == WorldPosition.WorldPositionEnforcedCache.GroundVec3)
							{
								worldPosition.GetGroundVec3();
							}
						}
						else
						{
							worldPosition.GetNavMeshVec3();
						}
					}
					else
					{
						flag = true;
						this._getPositionIsNavmeshlessCache = true;
					}
					this._getPositionResultCache = worldPosition;
				}
			}
			else
			{
				if (this._getPositionResultCache.IsValid)
				{
					if (worldPositionEnforcedCache != WorldPosition.WorldPositionEnforcedCache.NavMeshVec3)
					{
						if (worldPositionEnforcedCache == WorldPosition.WorldPositionEnforcedCache.GroundVec3)
						{
							this._getPositionResultCache.GetGroundVec3();
						}
					}
					else
					{
						this._getPositionResultCache.GetNavMeshVec3();
					}
				}
				worldPosition = this._getPositionResultCache;
			}
			if (this._getPositionIsNavmeshlessCache || flag)
			{
				FormationAI ai = f.AI;
				if (((ai != null) ? ai.ActiveBehavior : null) != null)
				{
					f.AI.ActiveBehavior.NavmeshlessTargetPositionPenalty = 1f;
				}
			}
			return worldPosition;
		}

		public void ResetPositionCache()
		{
			this._getPositionFirstSectionCache = WorldPosition.Invalid;
			this._getPositionResultCache = WorldPosition.Invalid;
		}

		public bool AreOrdersPracticallySame(MovementOrder m1, MovementOrder m2, bool isAIControlled)
		{
			if (m1.OrderEnum != m2.OrderEnum)
			{
				return false;
			}
			switch (m1.OrderEnum)
			{
			case MovementOrder.MovementOrderEnum.AttackEntity:
				return m1.TargetEntity == m2.TargetEntity;
			case MovementOrder.MovementOrderEnum.Charge:
				return true;
			case MovementOrder.MovementOrderEnum.ChargeToTarget:
				return m1.TargetFormation == m2.TargetFormation;
			case MovementOrder.MovementOrderEnum.Follow:
				return m1._targetAgent == m2._targetAgent;
			case MovementOrder.MovementOrderEnum.FollowEntity:
				return m1.TargetEntity == m2.TargetEntity;
			case MovementOrder.MovementOrderEnum.Guard:
				return m1._targetAgent == m2._targetAgent;
			case MovementOrder.MovementOrderEnum.Move:
				return isAIControlled && m1._position.AsVec2.DistanceSquared(m2._position.AsVec2) < 1f;
			case MovementOrder.MovementOrderEnum.Retreat:
				return true;
			case MovementOrder.MovementOrderEnum.Stop:
				return true;
			case MovementOrder.MovementOrderEnum.Advance:
				return true;
			case MovementOrder.MovementOrderEnum.FallBack:
				return true;
			default:
				return true;
			}
		}

		public void OnApply(Formation formation)
		{
			switch (this.OrderEnum)
			{
			case MovementOrder.MovementOrderEnum.AttackEntity:
				formation.FormAttackEntityDetachment(this.TargetEntity);
				break;
			case MovementOrder.MovementOrderEnum.ChargeToTarget:
				formation.SetTargetFormation(this.TargetFormation);
				break;
			case MovementOrder.MovementOrderEnum.Follow:
				formation.Arrangement.ReserveMiddleFrontUnitPosition(this._targetAgent);
				break;
			case MovementOrder.MovementOrderEnum.Guard:
			{
				Agent localTargetAgent = this._targetAgent;
				formation.ApplyActionOnEachUnit(delegate(Agent agent)
				{
					MovementOrder.OnUnitJoinOrLeaveAux(agent, localTargetAgent, true);
				}, null);
				break;
			}
			case MovementOrder.MovementOrderEnum.Move:
				formation.SetPositioning(new WorldPosition?(this.CreateNewOrderWorldPosition(formation, WorldPosition.WorldPositionEnforcedCache.None)), null, null);
				break;
			case MovementOrder.MovementOrderEnum.Retreat:
				MovementOrder.RetreatAux(formation);
				break;
			}
			MovementOrder.MovementOrderEnum orderEnum = this.OrderEnum;
			formation.ApplyActionOnEachUnit(delegate(Agent agent)
			{
				agent.RefreshBehaviorValues(orderEnum, formation.ArrangementOrder.OrderEnum);
			}, null);
		}

		public void OnCancel(Formation formation)
		{
			switch (this.OrderEnum)
			{
			case MovementOrder.MovementOrderEnum.AttackEntity:
				formation.DisbandAttackEntityDetachment();
				return;
			case MovementOrder.MovementOrderEnum.Charge:
			{
				Team team = formation.Team;
				TeamAISiegeComponent teamAISiegeComponent;
				if ((teamAISiegeComponent = ((team != null) ? team.TeamAI : null) as TeamAISiegeComponent) != null)
				{
					if (teamAISiegeComponent.InnerGate != null && teamAISiegeComponent.InnerGate.IsUsedByFormation(formation))
					{
						formation.StopUsingMachine(teamAISiegeComponent.InnerGate, true);
					}
					if (teamAISiegeComponent.OuterGate != null && teamAISiegeComponent.OuterGate.IsUsedByFormation(formation))
					{
						formation.StopUsingMachine(teamAISiegeComponent.OuterGate, true);
					}
					foreach (SiegeLadder siegeLadder in teamAISiegeComponent.Ladders)
					{
						if (siegeLadder.IsUsedByFormation(formation))
						{
							formation.StopUsingMachine(siegeLadder, true);
						}
					}
					if (formation.AttackEntityOrderDetachment != null)
					{
						formation.DisbandAttackEntityDetachment();
						this.TargetEntity = null;
					}
					this._position = WorldPosition.Invalid;
					return;
				}
				break;
			}
			case MovementOrder.MovementOrderEnum.ChargeToTarget:
				formation.SetTargetFormation(null);
				return;
			case MovementOrder.MovementOrderEnum.Follow:
				formation.Arrangement.ReleaseMiddleFrontUnitPosition();
				return;
			case MovementOrder.MovementOrderEnum.FollowEntity:
			case MovementOrder.MovementOrderEnum.Move:
			case MovementOrder.MovementOrderEnum.Stop:
			case MovementOrder.MovementOrderEnum.Advance:
				break;
			case MovementOrder.MovementOrderEnum.Guard:
			{
				Agent localTargetAgent = this._targetAgent;
				formation.ApplyActionOnEachUnit(delegate(Agent agent)
				{
					MovementOrder.OnUnitJoinOrLeaveAux(agent, localTargetAgent, false);
				}, null);
				return;
			}
			case MovementOrder.MovementOrderEnum.Retreat:
				formation.ApplyActionOnEachUnitViaBackupList(delegate(Agent agent)
				{
					if (agent.IsAIControlled)
					{
						agent.StopRetreatingMoraleComponent();
					}
				});
				return;
			case MovementOrder.MovementOrderEnum.FallBack:
				if (!Mission.Current.IsPositionInsideBoundaries(this.GetPosition(formation)))
				{
					formation.ApplyActionOnEachUnitViaBackupList(delegate(Agent agent)
					{
						if (agent.IsAIControlled)
						{
							agent.StopRetreatingMoraleComponent();
						}
					});
				}
				break;
			default:
				return;
			}
		}

		public void OnUnitJoinOrLeave(Formation formation, Agent unit, bool isJoining)
		{
			if (!this.IsApplicable(formation))
			{
				return;
			}
			MovementOrder.MovementOrderEnum orderEnum = this.OrderEnum;
			if (orderEnum == MovementOrder.MovementOrderEnum.Guard)
			{
				MovementOrder.OnUnitJoinOrLeaveAux(unit, this._targetAgent, isJoining);
			}
			if (isJoining)
			{
				if (this.OrderEnum != MovementOrder.MovementOrderEnum.Retreat)
				{
					unit.RefreshBehaviorValues(this.OrderEnum, formation.ArrangementOrder.OrderEnum);
					return;
				}
				if (unit.IsAIControlled)
				{
					unit.Retreat(false);
					return;
				}
			}
			else if (this.OrderEnum == MovementOrder.MovementOrderEnum.Retreat && unit.IsAIControlled && unit.IsActive())
			{
				unit.StopRetreatingMoraleComponent();
			}
		}

		public bool IsApplicable(Formation formation)
		{
			switch (this.OrderEnum)
			{
			case MovementOrder.MovementOrderEnum.AttackEntity:
			{
				UsableMachine firstScriptOfType = this.TargetEntity.GetFirstScriptOfType<UsableMachine>();
				if (firstScriptOfType != null)
				{
					return !firstScriptOfType.IsDestroyed;
				}
				DestructableComponent firstScriptOfType2 = this.TargetEntity.GetFirstScriptOfType<DestructableComponent>();
				return firstScriptOfType2 != null && !firstScriptOfType2.IsDestroyed;
			}
			case MovementOrder.MovementOrderEnum.Charge:
			{
				for (int i = 0; i < Mission.Current.Teams.Count; i++)
				{
					Team team = Mission.Current.Teams[i];
					if (team.IsEnemyOf(formation.Team) && team.ActiveAgents.Count > 0)
					{
						return true;
					}
				}
				return false;
			}
			case MovementOrder.MovementOrderEnum.ChargeToTarget:
				return this.TargetFormation.CountOfUnits > 0;
			case MovementOrder.MovementOrderEnum.Follow:
			case MovementOrder.MovementOrderEnum.Guard:
				return this._targetAgent.IsActive();
			case MovementOrder.MovementOrderEnum.FollowEntity:
			{
				UsableMachine firstScriptOfType3 = this.TargetEntity.GetFirstScriptOfType<UsableMachine>();
				return firstScriptOfType3 == null || !firstScriptOfType3.IsDestroyed;
			}
			default:
				return true;
			}
		}

		private bool IsInstance()
		{
			return this.OrderEnum != MovementOrder.MovementOrderEnum.Invalid && this.OrderEnum != MovementOrder.MovementOrderEnum.Charge && this.OrderEnum != MovementOrder.MovementOrderEnum.Retreat && this.OrderEnum != MovementOrder.MovementOrderEnum.Stop && this.OrderEnum != MovementOrder.MovementOrderEnum.Advance && this.OrderEnum != MovementOrder.MovementOrderEnum.FallBack;
		}

		public bool Tick(Formation formation)
		{
			object obj = !this.IsInstance() || this._tickTimer.Check(Mission.Current.CurrentTime);
			this.TickAux();
			object obj2 = obj;
			if (obj2 != null)
			{
				this.TickOccasionally(formation, this._tickTimer.PreviousDeltaTime);
			}
			return obj2 != null;
		}

		private void TickOccasionally(Formation formation, float dt)
		{
			MovementOrder.MovementOrderEnum orderEnum = this.OrderEnum;
			if (orderEnum - MovementOrder.MovementOrderEnum.Charge > 1)
			{
				if (orderEnum == MovementOrder.MovementOrderEnum.FallBack && !Mission.Current.IsPositionInsideBoundaries(this.GetPosition(formation)))
				{
					MovementOrder.RetreatAux(formation);
					return;
				}
			}
			else
			{
				Team team = formation.Team;
				TeamAISiegeComponent teamAISiegeComponent = ((team != null) ? team.TeamAI : null) as TeamAISiegeComponent;
				bool flag = false;
				bool flag2 = false;
				bool flag3 = false;
				bool flag4 = false;
				if (!Mission.Current.IsTeleportingAgents && teamAISiegeComponent != null)
				{
					flag4 = TeamAISiegeComponent.IsFormationInsideCastle(formation, false, 0.4f);
					bool flag5 = false;
					foreach (Team team2 in formation.Team.Mission.Teams)
					{
						if (team2.IsEnemyOf(formation.Team))
						{
							foreach (Formation formation2 in team2.FormationsIncludingEmpty)
							{
								if (formation2.CountOfUnits > 0 && flag4 == TeamAISiegeComponent.IsFormationInsideCastle(formation2, false, 0.4f))
								{
									flag5 = true;
									break;
								}
							}
							if (flag5)
							{
								break;
							}
						}
					}
					if (!flag5)
					{
						if (flag4 && !teamAISiegeComponent.CalculateIsAnyLaneOpenToGoOutside())
						{
							CastleGate gateToGetThrough = ((!teamAISiegeComponent.InnerGate.IsGateOpen) ? teamAISiegeComponent.InnerGate : teamAISiegeComponent.OuterGate);
							if (gateToGetThrough != null)
							{
								if (!gateToGetThrough.IsUsedByFormation(formation))
								{
									formation.StartUsingMachine(gateToGetThrough, true);
									SiegeLane siegeLane;
									if ((siegeLane = TeamAISiegeComponent.SiegeLanes.FirstOrDefault((SiegeLane sl) => sl.LaneSide == gateToGetThrough.DefenseSide)) == null)
									{
										siegeLane = TeamAISiegeComponent.SiegeLanes.FirstOrDefault((SiegeLane sl) => sl.LaneSide == FormationAI.BehaviorSide.Middle);
									}
									SiegeLane siegeLane2 = siegeLane;
									TacticalPosition tacticalPosition;
									if (siegeLane2 == null)
									{
										tacticalPosition = null;
									}
									else
									{
										ICastleKeyPosition castleKeyPosition = siegeLane2.DefensePoints.FirstOrDefault(delegate(ICastleKeyPosition dp)
										{
											UsableMachine usableMachine;
											return (usableMachine = dp.AttackerSiegeWeapon as UsableMachine) != null && !usableMachine.IsDisabled;
										});
										tacticalPosition = ((castleKeyPosition != null) ? castleKeyPosition.WaitPosition : null);
									}
									TacticalPosition tacticalPosition2 = tacticalPosition;
									if (tacticalPosition2 != null)
									{
										this._position = tacticalPosition2.Position;
									}
									else
									{
										WorldFrame? worldFrame;
										if (siegeLane2 == null)
										{
											worldFrame = null;
										}
										else
										{
											ICastleKeyPosition castleKeyPosition2 = siegeLane2.DefensePoints.FirstOrDefault(delegate(ICastleKeyPosition dp)
											{
												UsableMachine usableMachine2;
												return (usableMachine2 = dp.AttackerSiegeWeapon as UsableMachine) != null && !usableMachine2.IsDisabled;
											});
											worldFrame = ((castleKeyPosition2 != null) ? new WorldFrame?(castleKeyPosition2.DefenseWaitFrame) : null);
										}
										WorldFrame? worldFrame2 = worldFrame;
										WorldFrame worldFrame4;
										if (worldFrame2 == null)
										{
											WorldFrame? worldFrame3;
											if (siegeLane2 == null)
											{
												worldFrame3 = null;
											}
											else
											{
												ICastleKeyPosition castleKeyPosition3 = siegeLane2.DefensePoints.FirstOrDefault<ICastleKeyPosition>();
												worldFrame3 = ((castleKeyPosition3 != null) ? new WorldFrame?(castleKeyPosition3.DefenseWaitFrame) : null);
											}
											worldFrame4 = worldFrame3 ?? WorldFrame.Invalid;
										}
										else
										{
											worldFrame4 = worldFrame2.GetValueOrDefault();
										}
										WorldFrame worldFrame5 = worldFrame4;
										this._position = (worldFrame5.Origin.IsValid ? worldFrame5.Origin : formation.QuerySystem.MedianPosition);
									}
								}
								flag = true;
							}
						}
						else if (!teamAISiegeComponent.CalculateIsAnyLaneOpenToGetInside())
						{
							SiegeLadder siegeLadder = null;
							float num = float.MaxValue;
							foreach (SiegeLadder siegeLadder2 in teamAISiegeComponent.Ladders)
							{
								if (!siegeLadder2.IsDeactivated && !siegeLadder2.IsDisabled)
								{
									float num2 = siegeLadder2.WaitFrame.origin.DistanceSquared(formation.QuerySystem.MedianPosition.GetNavMeshVec3());
									if (num2 < num)
									{
										num = num2;
										siegeLadder = siegeLadder2;
									}
								}
							}
							if (siegeLadder != null)
							{
								if (!siegeLadder.IsUsedByFormation(formation))
								{
									formation.StartUsingMachine(siegeLadder, true);
									this._position = siegeLadder.WaitFrame.origin.ToWorldPosition();
								}
								else if (!this._position.IsValid)
								{
									this._position = siegeLadder.WaitFrame.origin.ToWorldPosition();
								}
								flag2 = true;
							}
							else
							{
								CastleGate castleGate = ((!teamAISiegeComponent.OuterGate.IsGateOpen) ? teamAISiegeComponent.OuterGate : teamAISiegeComponent.InnerGate);
								if (castleGate != null)
								{
									flag3 = true;
									if (formation.AttackEntityOrderDetachment == null)
									{
										formation.FormAttackEntityDetachment(castleGate.GameEntity);
										this.TargetEntity = castleGate.GameEntity;
										this._position = this.ComputeAttackEntityWaitPosition(formation, castleGate.GameEntity);
									}
									else if (this.TargetEntity != castleGate.GameEntity)
									{
										formation.DisbandAttackEntityDetachment();
										formation.FormAttackEntityDetachment(castleGate.GameEntity);
										this.TargetEntity = castleGate.GameEntity;
										this._position = this.ComputeAttackEntityWaitPosition(formation, castleGate.GameEntity);
									}
								}
							}
						}
					}
				}
				if (teamAISiegeComponent != null && flag4 && this._position.IsValid && !flag)
				{
					this._position = WorldPosition.Invalid;
					formation.SetPositioning(new WorldPosition?(this._position), null, null);
				}
				if (teamAISiegeComponent != null && !flag4 && this._position.IsValid && !flag2 && !flag3)
				{
					this._position = WorldPosition.Invalid;
					formation.SetPositioning(new WorldPosition?(this._position), null, null);
				}
				if (teamAISiegeComponent != null && formation.AttackEntityOrderDetachment != null && !flag3)
				{
					formation.DisbandAttackEntityDetachment();
					this.TargetEntity = null;
					this._position = WorldPosition.Invalid;
					formation.SetPositioning(new WorldPosition?(this._position), null, null);
				}
				if (this._position.IsValid)
				{
					formation.SetPositioning(new WorldPosition?(this._position), null, null);
				}
			}
		}

		private void TickAux()
		{
			MovementOrder.MovementOrderEnum orderEnum = this.OrderEnum;
			if (orderEnum == MovementOrder.MovementOrderEnum.Follow)
			{
				float length = this._targetAgent.GetCurrentVelocity().Length;
				if (length < 0.01f)
				{
					this._followState = MovementOrder.FollowState.Stop;
					return;
				}
				if (length < this._targetAgent.Monster.WalkingSpeedLimit * 0.7f)
				{
					if (this._followState == MovementOrder.FollowState.Stop)
					{
						this._followState = MovementOrder.FollowState.Depart;
						this._departStartTime = Mission.Current.CurrentTime;
						return;
					}
					if (this._followState == MovementOrder.FollowState.Move)
					{
						this._followState = MovementOrder.FollowState.Arrive;
						return;
					}
				}
				else if (this._followState == MovementOrder.FollowState.Depart)
				{
					if (Mission.Current.CurrentTime - this._departStartTime > 1f)
					{
						this._followState = MovementOrder.FollowState.Move;
						return;
					}
				}
				else
				{
					this._followState = MovementOrder.FollowState.Move;
				}
			}
		}

		public void OnArrangementChanged(Formation formation)
		{
			if (!this.IsApplicable(formation))
			{
				return;
			}
			MovementOrder.MovementOrderEnum orderEnum = this.OrderEnum;
			if (orderEnum == MovementOrder.MovementOrderEnum.Follow)
			{
				formation.Arrangement.ReserveMiddleFrontUnitPosition(this._targetAgent);
			}
		}

		public void Advance(Formation formation, float distance)
		{
			WorldPosition currentPosition = this.CreateNewOrderWorldPosition(formation, WorldPosition.WorldPositionEnforcedCache.None);
			Vec2 direction = formation.Direction;
			currentPosition.SetVec2(currentPosition.AsVec2 + direction * distance);
			this._positionLambda = (Formation f) => currentPosition;
		}

		public void FallBack(Formation formation, float distance)
		{
			this.Advance(formation, -distance);
		}

		private ValueTuple<Agent, float> GetBestAgent(List<Agent> candidateAgents)
		{
			if (candidateAgents.IsEmpty<Agent>())
			{
				return new ValueTuple<Agent, float>(null, float.MaxValue);
			}
			GameEntity targetEntity = this.TargetEntity;
			Vec3 targetEntityPos = targetEntity.GlobalPosition;
			Agent agent = candidateAgents.MinBy((Agent ca) => ca.Position.DistanceSquared(targetEntityPos));
			return new ValueTuple<Agent, float>(agent, agent.Position.DistanceSquared(targetEntityPos));
		}

		private ValueTuple<Agent, float> GetWorstAgent(List<Agent> currentAgents, int requiredAgentCount)
		{
			if (requiredAgentCount <= 0 || currentAgents.Count < requiredAgentCount)
			{
				return new ValueTuple<Agent, float>(null, float.MaxValue);
			}
			GameEntity targetEntity = this.TargetEntity;
			Vec3 targetEntityPos = targetEntity.GlobalPosition;
			Agent agent = currentAgents.MaxBy((Agent ca) => ca.Position.DistanceSquared(targetEntityPos));
			return new ValueTuple<Agent, float>(agent, agent.Position.DistanceSquared(targetEntityPos));
		}

		public MovementOrder GetSubstituteOrder(Formation formation)
		{
			MovementOrder.MovementOrderEnum orderEnum = this.OrderEnum;
			if (orderEnum == MovementOrder.MovementOrderEnum.Charge)
			{
				return MovementOrder.MovementOrderStop;
			}
			return MovementOrder.MovementOrderCharge;
		}

		private Vec2 GetDirectionAux(Formation f)
		{
			MovementOrder.MovementOrderEnum orderEnum = this.OrderEnum;
			if (orderEnum - MovementOrder.MovementOrderEnum.Advance > 1)
			{
				Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\AI\\Orders\\MovementOrder.cs", "GetDirectionAux", 1784);
				return Vec2.One;
			}
			FormationQuerySystem querySystem = f.QuerySystem;
			Formation targetFormation = f.TargetFormation;
			FormationQuerySystem formationQuerySystem = ((targetFormation != null) ? targetFormation.QuerySystem : null) ?? querySystem.ClosestEnemyFormation;
			if (formationQuerySystem == null)
			{
				return Vec2.One;
			}
			return (formationQuerySystem.MedianPosition.AsVec2 - querySystem.AveragePosition).Normalized();
		}

		private WorldPosition GetPositionAux(Formation f, WorldPosition.WorldPositionEnforcedCache worldPositionEnforcedCache)
		{
			MovementOrder.MovementOrderEnum orderEnum = this.OrderEnum;
			if (orderEnum != MovementOrder.MovementOrderEnum.Advance)
			{
				if (orderEnum != MovementOrder.MovementOrderEnum.FallBack)
				{
					Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\AI\\Orders\\MovementOrder.cs", "GetPositionAux", 1855);
					return WorldPosition.Invalid;
				}
				if (Mission.Current.Mode == MissionMode.Deployment)
				{
					return f.CreateNewOrderWorldPosition(worldPositionEnforcedCache);
				}
				Vec2 directionAux = this.GetDirectionAux(f);
				WorldPosition medianPosition = f.QuerySystem.MedianPosition;
				medianPosition.SetVec2(f.QuerySystem.AveragePosition - directionAux * 7f);
				return medianPosition;
			}
			else
			{
				if (Mission.Current.Mode == MissionMode.Deployment)
				{
					return f.CreateNewOrderWorldPosition(worldPositionEnforcedCache);
				}
				FormationQuerySystem querySystem = f.QuerySystem;
				Formation targetFormation = f.TargetFormation;
				FormationQuerySystem formationQuerySystem = ((targetFormation != null) ? targetFormation.QuerySystem : null) ?? querySystem.ClosestEnemyFormation;
				WorldPosition worldPosition;
				if (formationQuerySystem == null)
				{
					Agent closestEnemyAgent = querySystem.ClosestEnemyAgent;
					if (closestEnemyAgent == null)
					{
						return f.CreateNewOrderWorldPosition(worldPositionEnforcedCache);
					}
					worldPosition = closestEnemyAgent.GetWorldPosition();
				}
				else
				{
					worldPosition = formationQuerySystem.MedianPosition;
				}
				if (querySystem.IsRangedFormation || querySystem.IsRangedCavalryFormation || querySystem.HasThrowing)
				{
					Vec2 directionAux2 = this.GetDirectionAux(f);
					worldPosition.SetVec2(worldPosition.AsVec2 - directionAux2 * querySystem.MissileRangeAdjusted);
				}
				else if (formationQuerySystem != null)
				{
					Vec2 vec = (formationQuerySystem.AveragePosition - f.QuerySystem.AveragePosition).Normalized();
					float num = 2f;
					if (formationQuerySystem.FormationPower < f.QuerySystem.FormationPower * 0.2f)
					{
						num = 0.1f;
					}
					worldPosition.SetVec2(worldPosition.AsVec2 - vec * num);
				}
				return worldPosition;
			}
		}

		public static readonly MovementOrder MovementOrderNull = new MovementOrder(MovementOrder.MovementOrderEnum.Invalid);

		public static readonly MovementOrder MovementOrderCharge = new MovementOrder(MovementOrder.MovementOrderEnum.Charge);

		public static readonly MovementOrder MovementOrderRetreat = new MovementOrder(MovementOrder.MovementOrderEnum.Retreat);

		public static readonly MovementOrder MovementOrderStop = new MovementOrder(MovementOrder.MovementOrderEnum.Stop);

		public static readonly MovementOrder MovementOrderAdvance = new MovementOrder(MovementOrder.MovementOrderEnum.Advance);

		public static readonly MovementOrder MovementOrderFallBack = new MovementOrder(MovementOrder.MovementOrderEnum.FallBack);

		private MovementOrder.FollowState _followState;

		private float _departStartTime;

		public readonly MovementOrder.MovementOrderEnum OrderEnum;

		private Func<Formation, WorldPosition> _positionLambda;

		private WorldPosition _position;

		private WorldPosition _getPositionResultCache;

		private bool _getPositionIsNavmeshlessCache;

		private WorldPosition _getPositionFirstSectionCache;

		public GameEntity TargetEntity;

		private readonly Timer _tickTimer;

		private WorldPosition _lastPosition;

		public readonly bool _isFacingDirection;

		public enum MovementOrderEnum
		{
			Invalid,
			AttackEntity,
			Charge,
			ChargeToTarget,
			Follow,
			FollowEntity,
			Guard,
			Move,
			Retreat,
			Stop,
			Advance,
			FallBack
		}

		public enum MovementStateEnum
		{
			Charge,
			Hold,
			Retreat,
			StandGround
		}

		public enum Side
		{
			Front,
			Rear,
			Left,
			Right
		}

		private enum FollowState
		{
			Stop,
			Depart,
			Move,
			Arrive
		}
	}
}
