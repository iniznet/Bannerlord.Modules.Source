using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000144 RID: 324
	public struct MovementOrder
	{
		// Token: 0x1700038D RID: 909
		// (get) Token: 0x0600107C RID: 4220 RVA: 0x00034DFD File Offset: 0x00032FFD
		public Agent _targetAgent { get; }

		// Token: 0x1700038E RID: 910
		// (get) Token: 0x0600107D RID: 4221 RVA: 0x00034E08 File Offset: 0x00033008
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

		// Token: 0x1700038F RID: 911
		// (get) Token: 0x0600107E RID: 4222 RVA: 0x00034E84 File Offset: 0x00033084
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

		// Token: 0x0600107F RID: 4223 RVA: 0x00034ECC File Offset: 0x000330CC
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

		// Token: 0x06001080 RID: 4224 RVA: 0x00034FA8 File Offset: 0x000331A8
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

		// Token: 0x06001081 RID: 4225 RVA: 0x00035040 File Offset: 0x00033240
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

		// Token: 0x06001082 RID: 4226 RVA: 0x000351F4 File Offset: 0x000333F4
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

		// Token: 0x06001083 RID: 4227 RVA: 0x000352B4 File Offset: 0x000334B4
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

		// Token: 0x06001084 RID: 4228 RVA: 0x00035384 File Offset: 0x00033584
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

		// Token: 0x06001085 RID: 4229 RVA: 0x00035438 File Offset: 0x00033638
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

		// Token: 0x06001086 RID: 4230 RVA: 0x000354CC File Offset: 0x000336CC
		public override bool Equals(object obj)
		{
			if (obj is MovementOrder)
			{
				MovementOrder movementOrder = (MovementOrder)obj;
				return (movementOrder) == this;
			}
			return false;
		}

		// Token: 0x06001087 RID: 4231 RVA: 0x000354F7 File Offset: 0x000336F7
		public override int GetHashCode()
		{
			return (int)this.OrderEnum;
		}

		// Token: 0x06001088 RID: 4232 RVA: 0x000354FF File Offset: 0x000336FF
		public static bool operator !=(in MovementOrder m, MovementOrder obj)
		{
			return m.OrderEnum != obj.OrderEnum;
		}

		// Token: 0x06001089 RID: 4233 RVA: 0x00035512 File Offset: 0x00033712
		public static bool operator ==(in MovementOrder m, MovementOrder obj)
		{
			return m.OrderEnum == obj.OrderEnum;
		}

		// Token: 0x0600108A RID: 4234 RVA: 0x00035522 File Offset: 0x00033722
		public static MovementOrder MovementOrderChargeToTarget(Formation targetFormation)
		{
			return new MovementOrder(MovementOrder.MovementOrderEnum.ChargeToTarget, targetFormation);
		}

		// Token: 0x0600108B RID: 4235 RVA: 0x0003552B File Offset: 0x0003372B
		public static MovementOrder MovementOrderFollow(Agent targetAgent)
		{
			return new MovementOrder(MovementOrder.MovementOrderEnum.Follow, targetAgent);
		}

		// Token: 0x0600108C RID: 4236 RVA: 0x00035534 File Offset: 0x00033734
		public static MovementOrder MovementOrderGuard(Agent targetAgent)
		{
			return new MovementOrder(MovementOrder.MovementOrderEnum.Guard, targetAgent);
		}

		// Token: 0x0600108D RID: 4237 RVA: 0x0003553D File Offset: 0x0003373D
		public static MovementOrder MovementOrderFollowEntity(GameEntity targetEntity)
		{
			return new MovementOrder(MovementOrder.MovementOrderEnum.FollowEntity, targetEntity);
		}

		// Token: 0x0600108E RID: 4238 RVA: 0x00035546 File Offset: 0x00033746
		public static MovementOrder MovementOrderMove(WorldPosition position)
		{
			return new MovementOrder(MovementOrder.MovementOrderEnum.Move, position);
		}

		// Token: 0x0600108F RID: 4239 RVA: 0x0003554F File Offset: 0x0003374F
		public static MovementOrder MovementOrderAttackEntity(GameEntity targetEntity, bool surroundEntity)
		{
			return new MovementOrder(MovementOrder.MovementOrderEnum.AttackEntity, targetEntity, surroundEntity);
		}

		// Token: 0x06001090 RID: 4240 RVA: 0x00035559 File Offset: 0x00033759
		public static int GetMovementOrderDefensiveness(MovementOrder.MovementOrderEnum orderEnum)
		{
			if (orderEnum == MovementOrder.MovementOrderEnum.Charge || orderEnum == MovementOrder.MovementOrderEnum.ChargeToTarget)
			{
				return 0;
			}
			return 1;
		}

		// Token: 0x06001091 RID: 4241 RVA: 0x00035566 File Offset: 0x00033766
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

		// Token: 0x06001092 RID: 4242 RVA: 0x00035588 File Offset: 0x00033788
		public static void SetDefensiveArrangementMoveBehaviorValues(Agent unit)
		{
			unit.SetAIBehaviorValues(HumanAIComponent.AISimpleBehaviorKind.GoToPos, 3f, 8f, 5f, 20f, 6f);
			unit.SetAIBehaviorValues(HumanAIComponent.AISimpleBehaviorKind.Melee, 4f, 5f, 0f, 20f, 0f);
			unit.SetAIBehaviorValues(HumanAIComponent.AISimpleBehaviorKind.Ranged, 0f, 7f, 0f, 20f, 0f);
			unit.SetAIBehaviorValues(HumanAIComponent.AISimpleBehaviorKind.ChargeHorseback, 0f, 7f, 0f, 30f, 0f);
			unit.SetAIBehaviorValues(HumanAIComponent.AISimpleBehaviorKind.RangedHorseback, 0f, 15f, 0f, 30f, 0f);
			unit.SetAIBehaviorValues(HumanAIComponent.AISimpleBehaviorKind.AttackEntityMelee, 5f, 12f, 7.5f, 30f, 9f);
			unit.SetAIBehaviorValues(HumanAIComponent.AISimpleBehaviorKind.AttackEntityRanged, 0.55f, 12f, 0.8f, 30f, 0.45f);
		}

		// Token: 0x06001093 RID: 4243 RVA: 0x00035678 File Offset: 0x00033878
		public static void SetFollowBehaviorValues(Agent unit)
		{
			unit.SetAIBehaviorValues(HumanAIComponent.AISimpleBehaviorKind.GoToPos, 3f, 7f, 5f, 20f, 6f);
			unit.SetAIBehaviorValues(HumanAIComponent.AISimpleBehaviorKind.Melee, 6f, 7f, 4f, 20f, 0f);
			unit.SetAIBehaviorValues(HumanAIComponent.AISimpleBehaviorKind.Ranged, 0f, 7f, 0f, 20f, 0f);
			unit.SetAIBehaviorValues(HumanAIComponent.AISimpleBehaviorKind.ChargeHorseback, 0f, 7f, 0f, 30f, 0f);
			unit.SetAIBehaviorValues(HumanAIComponent.AISimpleBehaviorKind.RangedHorseback, 0f, 15f, 0f, 30f, 0f);
			unit.SetAIBehaviorValues(HumanAIComponent.AISimpleBehaviorKind.AttackEntityMelee, 5f, 12f, 7.5f, 30f, 9f);
			unit.SetAIBehaviorValues(HumanAIComponent.AISimpleBehaviorKind.AttackEntityRanged, 0.55f, 12f, 0.8f, 30f, 0.45f);
		}

		// Token: 0x06001094 RID: 4244 RVA: 0x00035768 File Offset: 0x00033968
		public static void SetDefaultMoveBehaviorValues(Agent unit)
		{
			unit.SetAIBehaviorValues(HumanAIComponent.AISimpleBehaviorKind.GoToPos, 3f, 7f, 5f, 20f, 6f);
			unit.SetAIBehaviorValues(HumanAIComponent.AISimpleBehaviorKind.Melee, 8f, 7f, 5f, 20f, 0.01f);
			unit.SetAIBehaviorValues(HumanAIComponent.AISimpleBehaviorKind.Ranged, 0.02f, 7f, 0.04f, 20f, 0.03f);
			unit.SetAIBehaviorValues(HumanAIComponent.AISimpleBehaviorKind.ChargeHorseback, 10f, 7f, 5f, 30f, 0.05f);
			unit.SetAIBehaviorValues(HumanAIComponent.AISimpleBehaviorKind.RangedHorseback, 0.02f, 15f, 0.065f, 30f, 0.055f);
			unit.SetAIBehaviorValues(HumanAIComponent.AISimpleBehaviorKind.AttackEntityMelee, 5f, 12f, 7.5f, 30f, 9f);
			unit.SetAIBehaviorValues(HumanAIComponent.AISimpleBehaviorKind.AttackEntityRanged, 0.55f, 12f, 0.8f, 30f, 0.45f);
		}

		// Token: 0x06001095 RID: 4245 RVA: 0x00035858 File Offset: 0x00033A58
		private static void SetChargeBehaviorValues(Agent unit)
		{
			unit.SetAIBehaviorValues(HumanAIComponent.AISimpleBehaviorKind.GoToPos, 3f, 7f, 5f, 20f, 6f);
			unit.SetAIBehaviorValues(HumanAIComponent.AISimpleBehaviorKind.Melee, 8f, 7f, 4f, 20f, 1f);
			unit.SetAIBehaviorValues(HumanAIComponent.AISimpleBehaviorKind.Ranged, 2f, 7f, 4f, 20f, 5f);
			unit.SetAIBehaviorValues(HumanAIComponent.AISimpleBehaviorKind.ChargeHorseback, 2f, 25f, 5f, 30f, 5f);
			unit.SetAIBehaviorValues(HumanAIComponent.AISimpleBehaviorKind.RangedHorseback, 0f, 10f, 3f, 20f, 6f);
			unit.SetAIBehaviorValues(HumanAIComponent.AISimpleBehaviorKind.AttackEntityMelee, 5f, 12f, 7.5f, 30f, 9f);
			unit.SetAIBehaviorValues(HumanAIComponent.AISimpleBehaviorKind.AttackEntityRanged, 0.55f, 12f, 0.8f, 30f, 0.45f);
		}

		// Token: 0x06001096 RID: 4246 RVA: 0x00035948 File Offset: 0x00033B48
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
					agent.Retreat();
				}
			});
		}

		// Token: 0x06001097 RID: 4247 RVA: 0x000359A4 File Offset: 0x00033BA4
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

		// Token: 0x06001098 RID: 4248 RVA: 0x00035A00 File Offset: 0x00033C00
		private static void OnUnitJoinOrLeaveAux(Agent unit, Agent target, bool isJoining)
		{
			unit.SetGuardState(target, isJoining);
		}

		// Token: 0x06001099 RID: 4249 RVA: 0x00035A0C File Offset: 0x00033C0C
		private void GetPositionAuxFollow(Formation f)
		{
			Vec2 vec = Vec2.Zero;
			if (this._followState != MovementOrder.FollowState.Move && this._targetAgent.MountAgent != null)
			{
				vec += f.Direction * -2f;
			}
			if (this._followState == MovementOrder.FollowState.Move && f.IsMounted())
			{
				vec += 2f * this._targetAgent.Velocity.AsVec2;
			}
			else if (this._followState == MovementOrder.FollowState.Move)
			{
				f.IsMounted();
			}
			WorldPosition worldPosition = this._targetAgent.GetWorldPosition();
			worldPosition.SetVec2(worldPosition.AsVec2 - f.GetMiddleFrontUnitPositionOffset() + vec);
			if (this._followState == MovementOrder.FollowState.Stop || this._followState == MovementOrder.FollowState.Depart)
			{
				float num = (f.IsCavalry() ? 4f : 2.5f);
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

		// Token: 0x0600109A RID: 4250 RVA: 0x00035B20 File Offset: 0x00033D20
		public Vec2 GetPosition(Formation f)
		{
			return this.CreateNewOrderWorldPosition(f, WorldPosition.WorldPositionEnforcedCache.None).AsVec2;
		}

		// Token: 0x0600109B RID: 4251 RVA: 0x00035B40 File Offset: 0x00033D40
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

		// Token: 0x0600109C RID: 4252 RVA: 0x00035CF6 File Offset: 0x00033EF6
		public void ResetPositionCache()
		{
			this._getPositionFirstSectionCache = WorldPosition.Invalid;
			this._getPositionResultCache = WorldPosition.Invalid;
		}

		// Token: 0x0600109D RID: 4253 RVA: 0x00035D10 File Offset: 0x00033F10
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

		// Token: 0x0600109E RID: 4254 RVA: 0x00035DFC File Offset: 0x00033FFC
		public void OnApply(Formation formation)
		{
			switch (this.OrderEnum)
			{
			case MovementOrder.MovementOrderEnum.AttackEntity:
				formation.FormAttackEntityDetachment(this.TargetEntity);
				break;
			case MovementOrder.MovementOrderEnum.ChargeToTarget:
				formation.TargetFormation = this.TargetFormation;
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
			if (this.OrderEnum == MovementOrder.MovementOrderEnum.Charge || this.OrderEnum == MovementOrder.MovementOrderEnum.ChargeToTarget)
			{
				formation.ApplyActionOnEachUnit(new Action<Agent>(MovementOrder.SetChargeBehaviorValues), null);
				return;
			}
			if (this.OrderEnum == MovementOrder.MovementOrderEnum.Follow || formation.ArrangementOrder.OrderEnum == ArrangementOrder.ArrangementOrderEnum.Column)
			{
				formation.ApplyActionOnEachUnit(new Action<Agent>(MovementOrder.SetFollowBehaviorValues), null);
				return;
			}
			if (formation.ArrangementOrder.OrderEnum != ArrangementOrder.ArrangementOrderEnum.ShieldWall && formation.ArrangementOrder.OrderEnum != ArrangementOrder.ArrangementOrderEnum.Circle && formation.ArrangementOrder.OrderEnum != ArrangementOrder.ArrangementOrderEnum.Square)
			{
				formation.ApplyActionOnEachUnit(new Action<Agent>(MovementOrder.SetDefaultMoveBehaviorValues), null);
				return;
			}
			formation.ApplyActionOnEachUnit(new Action<Agent>(MovementOrder.SetDefensiveArrangementMoveBehaviorValues), null);
		}

		// Token: 0x0600109F RID: 4255 RVA: 0x00035F64 File Offset: 0x00034164
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
				formation.TargetFormation = null;
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

		// Token: 0x060010A0 RID: 4256 RVA: 0x00036124 File Offset: 0x00034324
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
				if (this.OrderEnum == MovementOrder.MovementOrderEnum.Retreat)
				{
					if (unit.IsAIControlled)
					{
						unit.Retreat();
						return;
					}
				}
				else
				{
					if (this.OrderEnum == MovementOrder.MovementOrderEnum.Charge || this.OrderEnum == MovementOrder.MovementOrderEnum.ChargeToTarget)
					{
						MovementOrder.SetChargeBehaviorValues(unit);
						return;
					}
					if (formation.ArrangementOrder.OrderEnum != ArrangementOrder.ArrangementOrderEnum.ShieldWall && formation.ArrangementOrder.OrderEnum != ArrangementOrder.ArrangementOrderEnum.Circle && formation.ArrangementOrder.OrderEnum != ArrangementOrder.ArrangementOrderEnum.Square)
					{
						MovementOrder.SetDefaultMoveBehaviorValues(unit);
						return;
					}
					MovementOrder.SetDefensiveArrangementMoveBehaviorValues(unit);
					return;
				}
			}
			else if (this.OrderEnum == MovementOrder.MovementOrderEnum.Retreat && unit.IsAIControlled && unit.IsActive())
			{
				unit.StopRetreatingMoraleComponent();
			}
		}

		// Token: 0x060010A1 RID: 4257 RVA: 0x000361E0 File Offset: 0x000343E0
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
				return this._targetAgent.IsActive();
			case MovementOrder.MovementOrderEnum.FollowEntity:
			{
				UsableMachine firstScriptOfType3 = this.TargetEntity.GetFirstScriptOfType<UsableMachine>();
				return firstScriptOfType3 == null || !firstScriptOfType3.IsDestroyed;
			}
			case MovementOrder.MovementOrderEnum.Guard:
				return this._targetAgent.IsActive();
			default:
				return true;
			}
		}

		// Token: 0x060010A2 RID: 4258 RVA: 0x000362E1 File Offset: 0x000344E1
		private bool IsInstance()
		{
			return this.OrderEnum != MovementOrder.MovementOrderEnum.Invalid && this.OrderEnum != MovementOrder.MovementOrderEnum.Charge && this.OrderEnum != MovementOrder.MovementOrderEnum.Retreat && this.OrderEnum != MovementOrder.MovementOrderEnum.Stop && this.OrderEnum != MovementOrder.MovementOrderEnum.Advance && this.OrderEnum != MovementOrder.MovementOrderEnum.FallBack;
		}

		// Token: 0x060010A3 RID: 4259 RVA: 0x00036320 File Offset: 0x00034520
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

		// Token: 0x060010A4 RID: 4260 RVA: 0x00036360 File Offset: 0x00034560
		private void TickOccasionally(Formation formation, float dt)
		{
			MovementOrder.MovementOrderEnum orderEnum = this.OrderEnum;
			if (orderEnum != MovementOrder.MovementOrderEnum.Charge)
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

		// Token: 0x060010A5 RID: 4261 RVA: 0x00036948 File Offset: 0x00034B48
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

		// Token: 0x060010A6 RID: 4262 RVA: 0x00036A04 File Offset: 0x00034C04
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

		// Token: 0x060010A7 RID: 4263 RVA: 0x00036A38 File Offset: 0x00034C38
		public void Advance(Formation formation, float distance)
		{
			WorldPosition currentPosition = this.CreateNewOrderWorldPosition(formation, WorldPosition.WorldPositionEnforcedCache.None);
			Vec2 direction = formation.Direction;
			currentPosition.SetVec2(currentPosition.AsVec2 + direction * distance);
			this._positionLambda = (Formation f) => currentPosition;
		}

		// Token: 0x060010A8 RID: 4264 RVA: 0x00036A94 File Offset: 0x00034C94
		public void FallBack(Formation formation, float distance)
		{
			this.Advance(formation, -distance);
		}

		// Token: 0x060010A9 RID: 4265 RVA: 0x00036AA0 File Offset: 0x00034CA0
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

		// Token: 0x060010AA RID: 4266 RVA: 0x00036B08 File Offset: 0x00034D08
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

		// Token: 0x060010AB RID: 4267 RVA: 0x00036B74 File Offset: 0x00034D74
		public MovementOrder GetSubstituteOrder(Formation formation)
		{
			MovementOrder.MovementOrderEnum orderEnum = this.OrderEnum;
			if (orderEnum == MovementOrder.MovementOrderEnum.Charge)
			{
				return MovementOrder.MovementOrderStop;
			}
			return MovementOrder.MovementOrderCharge;
		}

		// Token: 0x060010AC RID: 4268 RVA: 0x00036B98 File Offset: 0x00034D98
		private Vec2 GetDirectionAux(Formation f)
		{
			MovementOrder.MovementOrderEnum orderEnum = this.OrderEnum;
			if (orderEnum - MovementOrder.MovementOrderEnum.Advance > 1)
			{
				Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\AI\\Orders\\MovementOrder.cs", "GetDirectionAux", 1854);
				return Vec2.One;
			}
			FormationQuerySystem querySystem = f.QuerySystem;
			FormationQuerySystem closestEnemyFormation = querySystem.ClosestEnemyFormation;
			if (closestEnemyFormation == null)
			{
				return Vec2.One;
			}
			return (closestEnemyFormation.MedianPosition.AsVec2 - querySystem.AveragePosition).Normalized();
		}

		// Token: 0x060010AD RID: 4269 RVA: 0x00036C0C File Offset: 0x00034E0C
		private WorldPosition GetPositionAux(Formation f, WorldPosition.WorldPositionEnforcedCache worldPositionEnforcedCache)
		{
			MovementOrder.MovementOrderEnum orderEnum = this.OrderEnum;
			if (orderEnum != MovementOrder.MovementOrderEnum.Advance)
			{
				if (orderEnum != MovementOrder.MovementOrderEnum.FallBack)
				{
					Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\AI\\Orders\\MovementOrder.cs", "GetPositionAux", 1922);
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
				FormationQuerySystem closestEnemyFormation = querySystem.ClosestEnemyFormation;
				if (closestEnemyFormation == null)
				{
					return f.CreateNewOrderWorldPosition(worldPositionEnforcedCache);
				}
				WorldPosition medianPosition2 = closestEnemyFormation.MedianPosition;
				if (querySystem.IsRangedFormation || querySystem.IsRangedCavalryFormation)
				{
					if (medianPosition2.AsVec2.DistanceSquared(querySystem.AveragePosition) <= querySystem.MissileRangeAdjusted * querySystem.MissileRangeAdjusted)
					{
						Vec2 directionAux2 = this.GetDirectionAux(f);
						medianPosition2.SetVec2(medianPosition2.AsVec2 - directionAux2 * querySystem.MissileRangeAdjusted);
					}
				}
				else
				{
					Vec2 vec = (closestEnemyFormation.AveragePosition - f.QuerySystem.AveragePosition).Normalized();
					float num = 2f;
					if (closestEnemyFormation.FormationPower < f.QuerySystem.FormationPower * 0.2f)
					{
						num = 0.1f;
					}
					medianPosition2.SetVec2(medianPosition2.AsVec2 - vec * num);
				}
				return medianPosition2;
			}
		}

		// Token: 0x04000423 RID: 1059
		public static readonly MovementOrder MovermentOrderNull = new MovementOrder(MovementOrder.MovementOrderEnum.Invalid);

		// Token: 0x04000424 RID: 1060
		public static readonly MovementOrder MovementOrderCharge = new MovementOrder(MovementOrder.MovementOrderEnum.Charge);

		// Token: 0x04000425 RID: 1061
		public static readonly MovementOrder MovementOrderRetreat = new MovementOrder(MovementOrder.MovementOrderEnum.Retreat);

		// Token: 0x04000426 RID: 1062
		public static readonly MovementOrder MovementOrderStop = new MovementOrder(MovementOrder.MovementOrderEnum.Stop);

		// Token: 0x04000427 RID: 1063
		public static readonly MovementOrder MovementOrderAdvance = new MovementOrder(MovementOrder.MovementOrderEnum.Advance);

		// Token: 0x04000428 RID: 1064
		public static readonly MovementOrder MovementOrderFallBack = new MovementOrder(MovementOrder.MovementOrderEnum.FallBack);

		// Token: 0x04000429 RID: 1065
		private MovementOrder.FollowState _followState;

		// Token: 0x0400042A RID: 1066
		private float _departStartTime;

		// Token: 0x0400042B RID: 1067
		public readonly MovementOrder.MovementOrderEnum OrderEnum;

		// Token: 0x0400042C RID: 1068
		private Func<Formation, WorldPosition> _positionLambda;

		// Token: 0x0400042D RID: 1069
		private WorldPosition _position;

		// Token: 0x0400042E RID: 1070
		private WorldPosition _getPositionResultCache;

		// Token: 0x0400042F RID: 1071
		private bool _getPositionIsNavmeshlessCache;

		// Token: 0x04000430 RID: 1072
		private WorldPosition _getPositionFirstSectionCache;

		// Token: 0x04000431 RID: 1073
		public Formation TargetFormation;

		// Token: 0x04000432 RID: 1074
		public GameEntity TargetEntity;

		// Token: 0x04000434 RID: 1076
		private readonly Timer _tickTimer;

		// Token: 0x04000435 RID: 1077
		private WorldPosition _lastPosition;

		// Token: 0x04000436 RID: 1078
		public readonly bool _isFacingDirection;

		// Token: 0x02000485 RID: 1157
		public enum MovementOrderEnum
		{
			// Token: 0x0400197B RID: 6523
			Invalid,
			// Token: 0x0400197C RID: 6524
			AttackEntity,
			// Token: 0x0400197D RID: 6525
			Charge,
			// Token: 0x0400197E RID: 6526
			ChargeToTarget,
			// Token: 0x0400197F RID: 6527
			Follow,
			// Token: 0x04001980 RID: 6528
			FollowEntity,
			// Token: 0x04001981 RID: 6529
			Guard,
			// Token: 0x04001982 RID: 6530
			Move,
			// Token: 0x04001983 RID: 6531
			Retreat,
			// Token: 0x04001984 RID: 6532
			Stop,
			// Token: 0x04001985 RID: 6533
			Advance,
			// Token: 0x04001986 RID: 6534
			FallBack
		}

		// Token: 0x02000486 RID: 1158
		public enum MovementStateEnum
		{
			// Token: 0x04001988 RID: 6536
			Charge,
			// Token: 0x04001989 RID: 6537
			Hold,
			// Token: 0x0400198A RID: 6538
			Retreat,
			// Token: 0x0400198B RID: 6539
			StandGround
		}

		// Token: 0x02000487 RID: 1159
		public enum Side
		{
			// Token: 0x0400198D RID: 6541
			Front,
			// Token: 0x0400198E RID: 6542
			Rear,
			// Token: 0x0400198F RID: 6543
			Left,
			// Token: 0x04001990 RID: 6544
			Right
		}

		// Token: 0x02000488 RID: 1160
		private enum FollowState
		{
			// Token: 0x04001992 RID: 6546
			Stop,
			// Token: 0x04001993 RID: 6547
			Depart,
			// Token: 0x04001994 RID: 6548
			Move,
			// Token: 0x04001995 RID: 6549
			Arrive
		}
	}
}
