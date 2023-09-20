using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using NetworkMessages.FromServer;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Network.Messages;
using TaleWorlds.MountAndBlade.Objects.Siege;

namespace TaleWorlds.MountAndBlade
{
	public class SiegeLadder : SiegeWeapon, IPrimarySiegeWeapon, IOrderableWithInteractionArea, IOrderable, ISpawnable
	{
		public GameEntity InitialWaitPosition { get; private set; }

		public int OnWallNavMeshId { get; private set; }

		public override SiegeEngineType GetSiegeEngineType()
		{
			return DefaultSiegeEngineTypes.Ladder;
		}

		protected internal override void OnInit()
		{
			base.OnInit();
			this._tickOccasionallyTimer = new Timer(Mission.Current.CurrentTime, 0.2f + MBRandom.RandomFloat * 0.05f, true);
			this._aiBarriers = base.Scene.FindEntitiesWithTag(this.BarrierTagToRemove).ToList<GameEntity>();
			if (this.IndestructibleMerlonsTag != string.Empty)
			{
				foreach (GameEntity gameEntity in base.Scene.FindEntitiesWithTag(this.IndestructibleMerlonsTag))
				{
					DestructableComponent firstScriptOfType = gameEntity.GetFirstScriptOfType<DestructableComponent>();
					firstScriptOfType.SetDisabled(false);
					firstScriptOfType.CanBeDestroyedInitially = false;
				}
			}
			this._attackerStandingPoints = base.GameEntity.CollectObjectsWithTag(this.AttackerTag);
			this._pushingWithForkStandingPoint = base.GameEntity.CollectObjectsWithTag(this.DefenderTag).FirstOrDefault<StandingPointWithWeaponRequirement>();
			this._forkPickUpStandingPoint = base.GameEntity.CollectObjectsWithTag(this.AmmoPickUpTag).FirstOrDefault<StandingPointWithWeaponRequirement>();
			StandingPointWithWeaponRequirement forkPickUpStandingPoint = this._forkPickUpStandingPoint;
			if (forkPickUpStandingPoint != null)
			{
				forkPickUpStandingPoint.SetUsingBattleSide(BattleSideEnum.Defender);
			}
			this._ladderParticleObject = base.GameEntity.CollectObjectsWithTag("particles").FirstOrDefault<SynchedMissionObject>();
			this._forkEntity = base.GameEntity.CollectObjectsWithTag("push_fork").FirstOrDefault<SynchedMissionObject>();
			if (base.StandingPoints != null)
			{
				foreach (StandingPoint standingPoint in base.StandingPoints)
				{
					if (!standingPoint.GameEntity.HasTag(this.AmmoPickUpTag))
					{
						standingPoint.AddComponent(new ResetAnimationOnStopUsageComponent(standingPoint.GameEntity.HasTag(this.DefenderTag) ? SiegeLadder.act_usage_ladder_push_back_stopped : ActionIndexCache.act_none));
						standingPoint.IsDeactivated = true;
					}
				}
			}
			this._forkItem = Game.Current.ObjectManager.GetObject<ItemObject>(this.PushForkItemID);
			this._pushingWithForkStandingPoint.InitRequiredWeapon(this._forkItem);
			this._forkPickUpStandingPoint.InitGivenWeapon(this._forkItem);
			GameEntity gameEntity2 = base.GameEntity.CollectChildrenEntitiesWithTag(this.upStateEntityTag)[0];
			List<SynchedMissionObject> list = base.GameEntity.CollectObjectsWithTag(this.downStateEntityTag);
			this._ladderObject = list[0];
			this._ladderSkeleton = this._ladderObject.GameEntity.Skeleton;
			list = base.GameEntity.CollectObjectsWithTag(this.BodyTag);
			this._ladderBodyObject = list[0];
			list = base.GameEntity.CollectObjectsWithTag(this.CollisionBodyTag);
			this._ladderCollisionBodyObject = list[0];
			this._ladderDownFrame = this._ladderObject.GameEntity.GetFrame();
			this._ladderDownFrame.rotation.RotateAboutSide(this._downStateRotationRadian - this._ladderDownFrame.rotation.GetEulerAngles().x);
			this._ladderObject.GameEntity.SetFrame(ref this._ladderDownFrame);
			this._ladderObject.GameEntity.RecomputeBoundingBox();
			MatrixFrame frame = gameEntity2.GetFrame();
			frame.rotation = Mat3.Identity;
			frame.rotation.RotateAboutSide(this._upStateRotationRadian);
			this._ladderUpFrame = frame;
			this._ladderUpFrame = this._ladderObject.GameEntity.Parent.GetFrame().TransformToLocal(this._ladderUpFrame);
			this._ladderInitialGlobalFrame = this._ladderObject.GameEntity.GetGlobalFrame();
			this._attackerStandingPointLocalIKFrames = new MatrixFrame[this._attackerStandingPoints.Count];
			this.State = this.initialState;
			for (int i = 0; i < this._attackerStandingPoints.Count; i++)
			{
				this._attackerStandingPointLocalIKFrames[i] = this._attackerStandingPoints[i].GameEntity.GetGlobalFrame().TransformToLocal(this._ladderInitialGlobalFrame);
				this._attackerStandingPoints[i].AddComponent(new ClearHandInverseKinematicsOnStopUsageComponent());
			}
			this.CalculateNavigationAndPhysics();
			this.InitialWaitPosition = base.GameEntity.CollectChildrenEntitiesWithTag(this.InitialWaitPositionTag).FirstOrDefault<GameEntity>();
			foreach (GameEntity gameEntity3 in base.Scene.FindEntitiesWithTag(this._targetWallSegmentTag))
			{
				WallSegment firstScriptOfType2 = gameEntity3.GetFirstScriptOfType<WallSegment>();
				if (firstScriptOfType2 != null)
				{
					this._targetWallSegment = firstScriptOfType2;
					this._targetWallSegment.AttackerSiegeWeapon = this;
					break;
				}
			}
			string sideTag = this._sideTag;
			if (!(sideTag == "left"))
			{
				if (!(sideTag == "middle"))
				{
					if (!(sideTag == "right"))
					{
						this.WeaponSide = FormationAI.BehaviorSide.Middle;
					}
					else
					{
						this.WeaponSide = FormationAI.BehaviorSide.Right;
					}
				}
				else
				{
					this.WeaponSide = FormationAI.BehaviorSide.Middle;
				}
			}
			else
			{
				this.WeaponSide = FormationAI.BehaviorSide.Left;
			}
			base.SetForcedUse(false);
			LadderQueueManager[] array = base.GameEntity.GetScriptComponents<LadderQueueManager>().ToArray<LadderQueueManager>();
			MatrixFrame matrixFrame = base.GameEntity.GetGlobalFrame().TransformToLocal(this._ladderObject.GameEntity.GetGlobalFrame());
			int num = 0;
			int num2 = 1;
			for (int j = base.GameEntity.Name.Length - 1; j >= 0; j--)
			{
				if (char.IsDigit(base.GameEntity.Name[j]))
				{
					num += (int)(base.GameEntity.Name[j] - '0') * num2;
					num2 *= 10;
				}
				else if (num > 0)
				{
					break;
				}
			}
			if (array.Length != 0)
			{
				this._queueManagerForAttackers = array[0];
				this._queueManagerForAttackers.Initialize(this.OnWallNavMeshId, matrixFrame, -matrixFrame.rotation.f, BattleSideEnum.Attacker, 3, 2.3561945f, 2f, 0.8f, 6f, 5f, false, 0.8f, (float)num, 5f, false, -2, -2, num, 2);
			}
			if (array.Length > 1 && this._pushingWithForkStandingPoint != null)
			{
				this._queueManagerForDefenders = array[1];
				MatrixFrame matrixFrame2 = this._pushingWithForkStandingPoint.GameEntity.GetGlobalFrame();
				matrixFrame2.rotation.RotateAboutSide(1.5707964f);
				matrixFrame2.origin -= matrixFrame2.rotation.u;
				matrixFrame2 = base.GameEntity.GetGlobalFrame().TransformToLocal(matrixFrame2);
				this._queueManagerForDefenders.Initialize(this.OnWallNavMeshId, matrixFrame2, matrixFrame.rotation.f, BattleSideEnum.Defender, 1, 2.8274333f, 0.5f, 0.8f, 6f, 5f, true, 0.8f, float.MaxValue, 5f, false, -2, -2, 0, 0);
			}
			base.GameEntity.Scene.MarkFacesWithIdAsLadder(this.OnWallNavMeshId, true);
			this.EnemyRangeToStopUsing = 0f;
			this._idleAnimationIndex = MBAnimation.GetAnimationIndexWithName(this.IdleAnimation);
			this._raiseAnimationIndex = MBAnimation.GetAnimationIndexWithName(this.RaiseAnimation);
			this._raiseAnimationWithoutRootBoneIndex = MBAnimation.GetAnimationIndexWithName(this.RaiseAnimationWithoutRootBone);
			this._pushBackAnimationIndex = MBAnimation.GetAnimationIndexWithName(this.PushBackAnimation);
			this._pushBackAnimationWithoutRootBoneIndex = MBAnimation.GetAnimationIndexWithName(this.PushBackAnimationWithoutRootBone);
			this._trembleWallHeavyAnimationIndex = MBAnimation.GetAnimationIndexWithName(this.TrembleWallHeavyAnimation);
			this._trembleWallLightAnimationIndex = MBAnimation.GetAnimationIndexWithName(this.TrembleWallLightAnimation);
			this._trembleGroundAnimationIndex = MBAnimation.GetAnimationIndexWithName(this.TrembleGroundAnimation);
			this.SetUpStateVisibility(false);
			base.SetScriptComponentToTick(this.GetTickRequirement());
			bool flag = false;
			foreach (GameEntity gameEntity4 in this._ladderObject.GameEntity.GetEntityAndChildren())
			{
				PhysicsShape bodyShape = gameEntity4.GetBodyShape();
				if (bodyShape != null)
				{
					PhysicsShape.AddPreloadQueueWithName(bodyShape.GetName(), gameEntity4.GetGlobalScale());
					flag = true;
				}
			}
			if (flag)
			{
				PhysicsShape.ProcessPreloadQueue();
			}
		}

		private float GetCurrentLadderAngularSpeed(int animationIndex)
		{
			float animationParameterAtChannel = this._ladderSkeleton.GetAnimationParameterAtChannel(0);
			MatrixFrame boneEntitialFrameWithIndex = this._ladderSkeleton.GetBoneEntitialFrameWithIndex(0);
			if (animationParameterAtChannel <= 0.01f)
			{
				return 0f;
			}
			this._ladderSkeleton.SetAnimationParameterAtChannel(0, animationParameterAtChannel - 0.01f);
			this._ladderSkeleton.TickAnimationsAndForceUpdate(0.0001f, this._ladderObject.GameEntity.GetGlobalFrame(), false);
			MatrixFrame boneEntitialFrameWithIndex2 = this._ladderSkeleton.GetBoneEntitialFrameWithIndex(0);
			Vec2 vec = new Vec2(boneEntitialFrameWithIndex.rotation.f.y, boneEntitialFrameWithIndex.rotation.f.z);
			Vec2 vec2 = new Vec2(boneEntitialFrameWithIndex2.rotation.f.y, boneEntitialFrameWithIndex2.rotation.f.z);
			return (vec.RotationInRadians - vec2.RotationInRadians) / (MBAnimation.GetAnimationDuration(animationIndex) * 0.01f);
		}

		private void OnLadderStateChange()
		{
			GameEntity gameEntity = this._ladderObject.GameEntity;
			if (this.State != SiegeLadder.LadderState.OnWall)
			{
				this.SetVisibilityOfAIBarriers(true);
			}
			switch (this.State)
			{
			case SiegeLadder.LadderState.OnLand:
				this._animationState = SiegeLadder.LadderAnimationState.Static;
				return;
			case SiegeLadder.LadderState.FallToLand:
				if (this._ladderSkeleton.GetAnimationIndexAtChannel(0) != this._trembleGroundAnimationIndex)
				{
					gameEntity.SetFrame(ref this._ladderDownFrame);
					gameEntity.RecomputeBoundingBox();
					this._ladderSkeleton.SetAnimationAtChannel(this._trembleGroundAnimationIndex, 0, 1f, -1f, 0f);
					this._animationState = SiegeLadder.LadderAnimationState.Static;
				}
				if (!GameNetwork.IsClientOrReplay)
				{
					this.State = SiegeLadder.LadderState.OnLand;
					return;
				}
				break;
			case SiegeLadder.LadderState.BeingRaised:
			case SiegeLadder.LadderState.BeingPushedBack:
				break;
			case SiegeLadder.LadderState.BeingRaisedStartFromGround:
			{
				this._animationState = SiegeLadder.LadderAnimationState.Animated;
				MatrixFrame frame = gameEntity.GetFrame();
				frame.rotation.RotateAboutSide(-1.5707964f);
				gameEntity.SetFrame(ref frame);
				gameEntity.RecomputeBoundingBox();
				this._ladderSkeleton.SetAnimationAtChannel(this._raiseAnimationIndex, 0, 1f, -1f, 0f);
				this._ladderSkeleton.ForceUpdateBoneFrames();
				this._lastDotProductOfAnimationAndTargetRotation = -1000f;
				if (!GameNetwork.IsClientOrReplay)
				{
					this._currentActionAgentCount = 1;
					this.State = SiegeLadder.LadderState.BeingRaised;
					return;
				}
				break;
			}
			case SiegeLadder.LadderState.BeingRaisedStopped:
			{
				this._animationState = SiegeLadder.LadderAnimationState.PhysicallyDynamic;
				MatrixFrame matrixFrame = gameEntity.GetGlobalFrame().TransformToParent(this._ladderSkeleton.GetBoneEntitialFrameWithIndex(0));
				matrixFrame.rotation.RotateAboutForward(1.5707964f);
				this._fallAngularSpeed = this.GetCurrentLadderAngularSpeed(this._raiseAnimationIndex);
				float animationParameterAtChannel = this._ladderSkeleton.GetAnimationParameterAtChannel(0);
				gameEntity.SetGlobalFrame(matrixFrame);
				gameEntity.RecomputeBoundingBox();
				this._ladderSkeleton.SetAnimationAtChannel(this._raiseAnimationWithoutRootBoneIndex, 0, 1f, -1f, 0f);
				this._ladderSkeleton.SetAnimationParameterAtChannel(0, animationParameterAtChannel);
				this._ladderSkeleton.TickAnimationsAndForceUpdate(0.0001f, gameEntity.GetGlobalFrame(), false);
				this._ladderSkeleton.SetAnimationAtChannel(this._idleAnimationIndex, 0, 1f, -1f, 0f);
				this._ladderObject.SetLocalPositionSmoothStep(ref this._ladderDownFrame.origin);
				if (!GameNetwork.IsClientOrReplay)
				{
					this.State = SiegeLadder.LadderState.BeingPushedBack;
					return;
				}
				break;
			}
			case SiegeLadder.LadderState.OnWall:
				this._animationState = SiegeLadder.LadderAnimationState.Static;
				this.SetVisibilityOfAIBarriers(false);
				return;
			case SiegeLadder.LadderState.FallToWall:
				if (GameNetwork.IsClientOrReplay)
				{
					int animationIndexAtChannel = this._ladderSkeleton.GetAnimationIndexAtChannel(0);
					if (animationIndexAtChannel != this._trembleWallHeavyAnimationIndex && animationIndexAtChannel != this._trembleWallLightAnimationIndex)
					{
						gameEntity.SetFrame(ref this._ladderUpFrame);
						gameEntity.RecomputeBoundingBox();
						this._ladderSkeleton.SetAnimationAtChannel((this._fallAngularSpeed < -0.5f) ? this._trembleWallHeavyAnimationIndex : this._trembleWallLightAnimationIndex, 0, 1f, -1f, 0f);
						this._animationState = SiegeLadder.LadderAnimationState.Static;
						return;
					}
				}
				else
				{
					this.State = SiegeLadder.LadderState.OnWall;
					SynchedMissionObject ladderParticleObject = this._ladderParticleObject;
					if (ladderParticleObject == null)
					{
						return;
					}
					ladderParticleObject.BurstParticlesSynched(false);
					return;
				}
				break;
			case SiegeLadder.LadderState.BeingPushedBackStartFromWall:
				this._animationState = SiegeLadder.LadderAnimationState.Animated;
				this._ladderSkeleton.SetAnimationAtChannel(this._pushBackAnimationIndex, 0, 1f, -1f, 0f);
				this._ladderSkeleton.TickAnimationsAndForceUpdate(0.0001f, gameEntity.GetGlobalFrame(), false);
				this._lastDotProductOfAnimationAndTargetRotation = -1000f;
				if (!GameNetwork.IsClientOrReplay)
				{
					this._currentActionAgentCount = 1;
					this.State = SiegeLadder.LadderState.BeingPushedBack;
					return;
				}
				break;
			case SiegeLadder.LadderState.BeingPushedBackStopped:
			{
				this._animationState = SiegeLadder.LadderAnimationState.PhysicallyDynamic;
				MatrixFrame matrixFrame2 = gameEntity.GetGlobalFrame().TransformToParent(this._ladderSkeleton.GetBoneEntitialFrameWithIndex(0));
				matrixFrame2.rotation.RotateAboutForward(1.5707964f);
				this._fallAngularSpeed = this.GetCurrentLadderAngularSpeed(this._pushBackAnimationIndex);
				float animationParameterAtChannel2 = this._ladderSkeleton.GetAnimationParameterAtChannel(0);
				gameEntity.SetGlobalFrame(matrixFrame2);
				gameEntity.RecomputeBoundingBox();
				this._ladderSkeleton.SetAnimationAtChannel(this._pushBackAnimationWithoutRootBoneIndex, 0, 1f, -1f, 0f);
				this._ladderSkeleton.SetAnimationParameterAtChannel(0, animationParameterAtChannel2);
				this._ladderSkeleton.TickAnimationsAndForceUpdate(0.0001f, gameEntity.GetGlobalFrame(), false);
				this._ladderSkeleton.SetAnimationAtChannel(this._idleAnimationIndex, 0, 1f, -1f, 0f);
				this._ladderObject.SetLocalPositionSmoothStep(ref this._ladderUpFrame.origin);
				if (!GameNetwork.IsClientOrReplay)
				{
					this.State = SiegeLadder.LadderState.BeingRaised;
				}
				this._ladderSkeleton.ForceUpdateBoneFrames();
				break;
			}
			default:
				return;
			}
		}

		private void SetVisibilityOfAIBarriers(bool visibility)
		{
			foreach (GameEntity gameEntity in this._aiBarriers)
			{
				gameEntity.SetVisibilityExcludeParents(visibility);
			}
		}

		public int OverTheWallNavMeshID
		{
			get
			{
				return 13;
			}
		}

		public override OrderType GetOrder(BattleSideEnum side)
		{
			if (side != BattleSideEnum.Attacker)
			{
				return OrderType.Move;
			}
			return base.GetOrder(side);
		}

		public SiegeLadder.LadderState State
		{
			get
			{
				return this._state;
			}
			set
			{
				if (this._state != value)
				{
					if (GameNetwork.IsServerOrRecorder)
					{
						GameNetwork.BeginBroadcastModuleEvent();
						GameNetwork.WriteMessage(new SetSiegeLadderState(this, value));
						GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
					}
					this._state = value;
					this.OnLadderStateChange();
					this.CalculateNavigationAndPhysics();
				}
			}
		}

		private void CalculateNavigationAndPhysics()
		{
			if (!GameNetwork.IsClientOrReplay)
			{
				bool flag = this.State != SiegeLadder.LadderState.OnWall;
				if (this._isNavigationMeshDisabled != flag)
				{
					this._isNavigationMeshDisabled = flag;
					this.SetAbilityOfFaces(!this._isNavigationMeshDisabled);
				}
			}
			bool flag2 = (this.State == SiegeLadder.LadderState.BeingRaisedStartFromGround || this.State == SiegeLadder.LadderState.BeingRaised) && this._animationState != SiegeLadder.LadderAnimationState.PhysicallyDynamic;
			bool flag3 = true;
			if (this._isLadderPhysicsDisabled != flag2)
			{
				this._isLadderPhysicsDisabled = flag2;
				this._ladderBodyObject.GameEntity.SetPhysicsState(!this._isLadderPhysicsDisabled, true);
			}
			if (!flag2)
			{
				MatrixFrame matrixFrame = this._ladderObject.GameEntity.GetGlobalFrame().TransformToParent(this._ladderSkeleton.GetBoneEntitialFrameWithIndex(0));
				matrixFrame.rotation.RotateAboutForward(1.5707964f);
				this._ladderBodyObject.GameEntity.SetGlobalFrame(matrixFrame);
				this._ladderBodyObject.GameEntity.RecomputeBoundingBox();
				flag3 = this.State != SiegeLadder.LadderState.BeingPushedBack || matrixFrame.rotation.f.z < 0f;
				if (!flag3)
				{
					float num = MathF.Min(2.01f - matrixFrame.rotation.u.z * 2f, 1f);
					matrixFrame.rotation.ApplyScaleLocal(new Vec3(1f, num, 1f, -1f));
					this._ladderCollisionBodyObject.GameEntity.SetGlobalFrame(matrixFrame);
					this._ladderCollisionBodyObject.GameEntity.RecomputeBoundingBox();
				}
			}
			if (this._isLadderCollisionPhysicsDisabled != flag3)
			{
				this._isLadderCollisionPhysicsDisabled = flag3;
				this._ladderCollisionBodyObject.GameEntity.SetPhysicsState(!this._isLadderCollisionPhysicsDisabled, true);
			}
		}

		public MissionObject TargetCastlePosition
		{
			get
			{
				return this._targetWallSegment;
			}
		}

		public bool HasCompletedAction()
		{
			return this.State == SiegeLadder.LadderState.OnWall;
		}

		public FormationAI.BehaviorSide WeaponSide { get; private set; }

		public float SiegeWeaponPriority
		{
			get
			{
				return 8f;
			}
		}

		private ActionIndexCache GetActionCodeToUseForStandingPoint(StandingPoint standingPoint)
		{
			GameEntity gameEntity = standingPoint.GameEntity;
			if (!gameEntity.HasTag(this.RightStandingPointTag))
			{
				if (!gameEntity.HasTag(this.FrontStandingPointTag))
				{
					return SiegeLadder.act_usage_ladder_lift_from_left_2_start;
				}
				return SiegeLadder.act_usage_ladder_lift_from_left_1_start;
			}
			else
			{
				if (!gameEntity.HasTag(this.FrontStandingPointTag))
				{
					return SiegeLadder.act_usage_ladder_lift_from_right_2_start;
				}
				return SiegeLadder.act_usage_ladder_lift_from_right_1_start;
			}
		}

		public override bool IsDisabledForBattleSide(BattleSideEnum sideEnum)
		{
			if (sideEnum == BattleSideEnum.Attacker)
			{
				return this.State == SiegeLadder.LadderState.FallToLand || this.State == SiegeLadder.LadderState.FallToWall || this.State == SiegeLadder.LadderState.OnWall || (this.State == SiegeLadder.LadderState.BeingPushedBack && this._animationState != SiegeLadder.LadderAnimationState.PhysicallyDynamic) || this.State == SiegeLadder.LadderState.BeingPushedBackStartFromWall || this.State == SiegeLadder.LadderState.BeingPushedBackStopped;
			}
			return this.State == SiegeLadder.LadderState.OnLand || this.State == SiegeLadder.LadderState.FallToLand || this.State == SiegeLadder.LadderState.BeingRaised || this.State == SiegeLadder.LadderState.BeingRaisedStartFromGround || this.State == SiegeLadder.LadderState.FallToWall;
		}

		protected override float GetDetachmentWeightAux(BattleSideEnum side)
		{
			if (side == BattleSideEnum.Attacker)
			{
				return base.GetDetachmentWeightAux(side);
			}
			if (this.IsDisabledForBattleSideAI(side))
			{
				return float.MinValue;
			}
			this._usableStandingPoints.Clear();
			bool flag = false;
			bool flag2 = false;
			for (int i = 0; i < base.StandingPoints.Count; i++)
			{
				StandingPoint standingPoint = base.StandingPoints[i];
				if (standingPoint.IsUsableBySide(side) && (standingPoint != this._forkPickUpStandingPoint || this._pushingWithForkStandingPoint.IsUsableBySide(side)))
				{
					if (!standingPoint.HasAIMovingTo)
					{
						if (!flag2)
						{
							this._usableStandingPoints.Clear();
						}
						flag2 = true;
					}
					else if (flag2 || standingPoint.MovingAgent.Formation.Team.Side != side)
					{
						goto IL_A4;
					}
					flag = true;
					this._usableStandingPoints.Add(new ValueTuple<int, StandingPoint>(i, standingPoint));
				}
				IL_A4:;
			}
			this._areUsableStandingPointsVacant = flag2;
			if (!flag)
			{
				return float.MinValue;
			}
			if (flag2)
			{
				return 1f;
			}
			if (!this._isDetachmentRecentlyEvaluated)
			{
				return 0.1f;
			}
			return 0.01f;
		}

		public bool HoldLadders
		{
			get
			{
				return false;
			}
		}

		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			return base.GetTickRequirement() | ScriptComponentBehavior.TickRequirement.Tick | ScriptComponentBehavior.TickRequirement.TickParallel;
		}

		public bool SendLadders
		{
			get
			{
				return this.State > SiegeLadder.LadderState.OnLand;
			}
		}

		protected internal override void OnTick(float dt)
		{
			base.OnTick(dt);
			if (this._tickOccasionallyTimer.Check(Mission.Current.CurrentTime))
			{
				this.TickRare();
			}
			if (!GameNetwork.IsClientOrReplay && this._forkReappearingTimer != null && this._forkReappearingTimer.Check(Mission.Current.CurrentTime))
			{
				this._forkPickUpStandingPoint.SetIsDeactivatedSynched(false);
				this._forkEntity.SetVisibleSynched(true, false);
			}
			int num = 0;
			int num2 = 0;
			SiegeLadder.<>c__DisplayClass113_0 CS$<>8__locals1;
			CS$<>8__locals1.ladderObjectEntity = this._ladderObject.GameEntity;
			if (!GameNetwork.IsClientOrReplay)
			{
				if (this._queueManagerForAttackers != null)
				{
					this._queueManagerForAttackers.IsDeactivated = this.State != SiegeLadder.LadderState.OnWall;
				}
				if (this._queueManagerForDefenders != null)
				{
					this._queueManagerForDefenders.IsDeactivated = this.State != SiegeLadder.LadderState.OnWall;
				}
				int animationIndexAtChannel = this._ladderSkeleton.GetAnimationIndexAtChannel(0);
				bool flag = false;
				if (animationIndexAtChannel >= 0)
				{
					flag = animationIndexAtChannel == this._trembleGroundAnimationIndex || animationIndexAtChannel == this._trembleWallHeavyAnimationIndex || animationIndexAtChannel == this._trembleWallLightAnimationIndex;
					if (flag)
					{
						flag = this._ladderSkeleton.GetAnimationParameterAtChannel(0) < 1f;
					}
				}
				num += ((this._pushingWithForkStandingPoint.HasUser && !this._pushingWithForkStandingPoint.UserAgent.IsInBeingStruckAction) ? 1 : 0);
				foreach (StandingPoint standingPoint in this._attackerStandingPoints)
				{
					if (standingPoint.HasUser && !standingPoint.UserAgent.IsInBeingStruckAction)
					{
						num2++;
					}
				}
				SiegeLadder.<>c__DisplayClass113_1 CS$<>8__locals2;
				CS$<>8__locals2.someoneOver = null;
				foreach (StandingPoint standingPoint2 in base.StandingPoints)
				{
					GameEntity gameEntity = standingPoint2.GameEntity;
					if (!gameEntity.HasTag(this.AmmoPickUpTag))
					{
						bool flag2 = false;
						if ((!standingPoint2.HasUser || standingPoint2.UserAgent.IsInBeingStruckAction) && this.State == SiegeLadder.LadderState.BeingRaised && gameEntity.HasTag(this.AttackerTag))
						{
							float animationParameterAtChannel = this._ladderSkeleton.GetAnimationParameterAtChannel(0);
							float animationDuration = MBAnimation.GetAnimationDuration(this._ladderSkeleton.GetAnimationIndexAtChannel(0));
							ActionIndexCache actionCodeToUseForStandingPoint = this.GetActionCodeToUseForStandingPoint(standingPoint2);
							int animationIndexOfAction = MBActionSet.GetAnimationIndexOfAction(MBGlobals.GetActionSetWithSuffix(Game.Current.DefaultMonster, false, "_warrior"), actionCodeToUseForStandingPoint);
							flag2 = animationParameterAtChannel * animationDuration / MathF.Max(MBAnimation.GetAnimationDuration(animationIndexOfAction), 0.01f) > 0.98f;
						}
						standingPoint2.SetIsDeactivatedSynched(flag2 || this.State == SiegeLadder.LadderState.BeingPushedBackStopped || (gameEntity.HasTag(this.AttackerTag) && (this.State == SiegeLadder.LadderState.OnWall || this.State == SiegeLadder.LadderState.FallToWall || (this.State == SiegeLadder.LadderState.BeingPushedBack && this._animationState != SiegeLadder.LadderAnimationState.PhysicallyDynamic) || this.State == SiegeLadder.LadderState.BeingPushedBackStartFromWall)) || (gameEntity.HasTag(this.DefenderTag) && (this.State == SiegeLadder.LadderState.OnLand || this._animationState == SiegeLadder.LadderAnimationState.PhysicallyDynamic || this.State == SiegeLadder.LadderState.BeingRaisedStopped || flag || this.State == SiegeLadder.LadderState.FallToLand || this.State == SiegeLadder.LadderState.BeingRaised || this.State == SiegeLadder.LadderState.BeingRaisedStartFromGround || this.<OnTick>g__GetIsSomeoneOver|113_0(ref CS$<>8__locals1, ref CS$<>8__locals2))));
					}
				}
				if (this._forkPickUpStandingPoint.HasUser)
				{
					Agent userAgent = this._forkPickUpStandingPoint.UserAgent;
					ActionIndexValueCache currentActionValue = userAgent.GetCurrentActionValue(1);
					if (!(currentActionValue == SiegeLadder.act_usage_ladder_pick_up_fork_begin))
					{
						if (currentActionValue == SiegeLadder.act_usage_ladder_pick_up_fork_end)
						{
							MissionWeapon missionWeapon = new MissionWeapon(this._forkItem, null, null);
							userAgent.EquipWeaponToExtraSlotAndWield(ref missionWeapon);
							this._forkPickUpStandingPoint.UserAgent.StopUsingGameObject(true, Agent.StopUsingGameObjectFlags.AutoAttachAfterStoppingUsingGameObject);
							this._forkPickUpStandingPoint.SetIsDeactivatedSynched(true);
							this._forkEntity.SetVisibleSynched(false, false);
							this._forkReappearingTimer = new Timer(Mission.Current.CurrentTime, this._forkReappearingDelay, true);
							if (userAgent.IsAIControlled)
							{
								StandingPoint suitableStandingPointFor = this.GetSuitableStandingPointFor(userAgent.Team.Side, userAgent, null, null);
								if (suitableStandingPointFor != null)
								{
									((IDetachment)this).AddAgent(userAgent, -1);
									if (userAgent.Formation != null)
									{
										userAgent.Formation.DetachUnit(userAgent, ((IDetachment)this).IsLoose);
										userAgent.Detachment = this;
										userAgent.DetachmentWeight = this.GetWeightOfStandingPoint(suitableStandingPointFor);
									}
								}
							}
						}
						else if (!this._forkPickUpStandingPoint.UserAgent.SetActionChannel(1, SiegeLadder.act_usage_ladder_pick_up_fork_begin, false, 0UL, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true))
						{
							this._forkPickUpStandingPoint.UserAgent.StopUsingGameObject(true, Agent.StopUsingGameObjectFlags.AutoAttachAfterStoppingUsingGameObject);
						}
					}
				}
				else if (this._forkPickUpStandingPoint.HasAIMovingTo)
				{
					Agent movingAgent = this._forkPickUpStandingPoint.MovingAgent;
					if (movingAgent.Team != null && !this._pushingWithForkStandingPoint.IsUsableBySide(movingAgent.Team.Side))
					{
						movingAgent.StopUsingGameObject(true, Agent.StopUsingGameObjectFlags.AutoAttachAfterStoppingUsingGameObject);
					}
				}
			}
			switch (this.State)
			{
			case SiegeLadder.LadderState.OnLand:
			case SiegeLadder.LadderState.FallToLand:
				if (!GameNetwork.IsClientOrReplay && num2 > 0)
				{
					this.State = SiegeLadder.LadderState.BeingRaisedStartFromGround;
				}
				break;
			case SiegeLadder.LadderState.BeingRaised:
			case SiegeLadder.LadderState.BeingRaisedStartFromGround:
			case SiegeLadder.LadderState.BeingPushedBackStopped:
				if (this._animationState == SiegeLadder.LadderAnimationState.Animated)
				{
					float animationParameterAtChannel2 = this._ladderSkeleton.GetAnimationParameterAtChannel(0);
					float animationDuration2 = MBAnimation.GetAnimationDuration(this._ladderSkeleton.GetAnimationIndexAtChannel(0));
					foreach (StandingPoint standingPoint3 in this._attackerStandingPoints)
					{
						if (standingPoint3.HasUser)
						{
							MBActionSet actionSet = standingPoint3.UserAgent.ActionSet;
							ActionIndexCache actionCodeToUseForStandingPoint2 = this.GetActionCodeToUseForStandingPoint(standingPoint3);
							ActionIndexValueCache currentActionValue2 = standingPoint3.UserAgent.GetCurrentActionValue(1);
							if (currentActionValue2 == actionCodeToUseForStandingPoint2)
							{
								int animationIndexOfAction2 = MBActionSet.GetAnimationIndexOfAction(actionSet, actionCodeToUseForStandingPoint2);
								float num3 = MBMath.ClampFloat(animationParameterAtChannel2 * animationDuration2 / MathF.Max(MBAnimation.GetAnimationDuration(animationIndexOfAction2), 0.01f), 0f, 1f);
								standingPoint3.UserAgent.SetCurrentActionProgress(1, num3);
							}
							else if (MBAnimation.GetActionType(currentActionValue2) == Agent.ActionCodeType.LadderRaiseEnd)
							{
								float animationDuration3 = MBAnimation.GetAnimationDuration(MBActionSet.GetAnimationIndexOfAction(actionSet, currentActionValue2));
								float num4 = animationDuration2 - animationDuration3;
								float num5 = MBMath.ClampFloat((animationParameterAtChannel2 * animationDuration2 - num4) / MathF.Max(animationDuration3, 0.01f), 0f, 1f);
								standingPoint3.UserAgent.SetCurrentActionProgress(1, num5);
							}
						}
					}
					bool flag3 = false;
					if (!GameNetwork.IsClientOrReplay)
					{
						if (num2 > 0)
						{
							if (num2 != this._currentActionAgentCount)
							{
								this._currentActionAgentCount = num2;
								float num6 = MathF.Sqrt((float)this._currentActionAgentCount);
								float animationParameterAtChannel3 = this._ladderSkeleton.GetAnimationParameterAtChannel(0);
								this._ladderObject.SetAnimationAtChannelSynched(this._raiseAnimationIndex, 0, num6);
								if (animationParameterAtChannel3 > 0f)
								{
									this._ladderObject.SetAnimationChannelParameterSynched(0, animationParameterAtChannel3);
								}
							}
							using (List<StandingPoint>.Enumerator enumerator = this._attackerStandingPoints.GetEnumerator())
							{
								while (enumerator.MoveNext())
								{
									StandingPoint standingPoint4 = enumerator.Current;
									if (standingPoint4.HasUser)
									{
										ActionIndexCache actionCodeToUseForStandingPoint3 = this.GetActionCodeToUseForStandingPoint(standingPoint4);
										Agent userAgent2 = standingPoint4.UserAgent;
										ActionIndexValueCache currentActionValue3 = userAgent2.GetCurrentActionValue(1);
										if (currentActionValue3 != actionCodeToUseForStandingPoint3 && MBAnimation.GetActionType(currentActionValue3) != Agent.ActionCodeType.LadderRaiseEnd)
										{
											if (!userAgent2.SetActionChannel(1, actionCodeToUseForStandingPoint3, false, 0UL, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true) && !userAgent2.IsAIControlled)
											{
												userAgent2.StopUsingGameObject(false, Agent.StopUsingGameObjectFlags.AutoAttachAfterStoppingUsingGameObject);
											}
										}
										else if (MBAnimation.GetActionType(currentActionValue3) == Agent.ActionCodeType.LadderRaiseEnd)
										{
											standingPoint4.UserAgent.ClearTargetFrame();
										}
									}
								}
								goto IL_7DA;
							}
						}
						this.State = SiegeLadder.LadderState.BeingRaisedStopped;
						flag3 = true;
					}
					IL_7DA:
					if (!flag3)
					{
						MatrixFrame matrixFrame = CS$<>8__locals1.ladderObjectEntity.GetGlobalFrame().TransformToParent(this._ladderSkeleton.GetBoneEntitialFrameWithIndex(0));
						matrixFrame.rotation.RotateAboutForward(1.5707964f);
						if ((animationParameterAtChannel2 > 0.9f && animationParameterAtChannel2 != 1f) || matrixFrame.rotation.f.z <= 0.2f)
						{
							this._animationState = SiegeLadder.LadderAnimationState.PhysicallyDynamic;
							this._fallAngularSpeed = this.GetCurrentLadderAngularSpeed(this._raiseAnimationIndex);
							CS$<>8__locals1.ladderObjectEntity.SetGlobalFrame(matrixFrame);
							CS$<>8__locals1.ladderObjectEntity.RecomputeBoundingBox();
							this._ladderSkeleton.SetAnimationAtChannel(this._raiseAnimationWithoutRootBoneIndex, 0, 1f, -1f, 0f);
							this._ladderSkeleton.SetAnimationParameterAtChannel(0, animationParameterAtChannel2);
							this._ladderSkeleton.TickAnimationsAndForceUpdate(0.0001f, CS$<>8__locals1.ladderObjectEntity.GetGlobalFrame(), false);
							this._ladderSkeleton.SetAnimationAtChannel(this._idleAnimationIndex, 0, 1f, -1f, 0f);
							this._ladderObject.SetLocalPositionSmoothStep(ref this._ladderUpFrame.origin);
						}
					}
				}
				else if (this._animationState == SiegeLadder.LadderAnimationState.PhysicallyDynamic)
				{
					MatrixFrame frame = CS$<>8__locals1.ladderObjectEntity.GetFrame();
					frame.rotation.RotateAboutSide(this._fallAngularSpeed * dt);
					CS$<>8__locals1.ladderObjectEntity.SetFrame(ref frame);
					CS$<>8__locals1.ladderObjectEntity.RecomputeBoundingBox();
					MatrixFrame matrixFrame2 = CS$<>8__locals1.ladderObjectEntity.GetFrame().TransformToParent(this._ladderSkeleton.GetBoneEntitialFrameWithIndex(0));
					float num7 = Vec3.DotProduct(matrixFrame2.rotation.f, this._ladderUpFrame.rotation.f);
					if (this._fallAngularSpeed < 0f && num7 > 0.95f && num7 < this._lastDotProductOfAnimationAndTargetRotation)
					{
						CS$<>8__locals1.ladderObjectEntity.SetFrame(ref this._ladderUpFrame);
						CS$<>8__locals1.ladderObjectEntity.RecomputeBoundingBox();
						this._ladderSkeleton.SetAnimationParameterAtChannel(0, 0f);
						this._ladderSkeleton.TickAnimationsAndForceUpdate(0.0001f, CS$<>8__locals1.ladderObjectEntity.GetGlobalFrame(), false);
						this._animationState = SiegeLadder.LadderAnimationState.Static;
						this._ladderSkeleton.SetAnimationAtChannel((this._fallAngularSpeed < -0.5f) ? this._trembleWallHeavyAnimationIndex : this._trembleWallLightAnimationIndex, 0, 1f, -1f, 0f);
						if (!GameNetwork.IsClientOrReplay)
						{
							this.State = SiegeLadder.LadderState.FallToWall;
						}
					}
					this._fallAngularSpeed -= dt * 2f * MathF.Max(0.3f, 1f - matrixFrame2.rotation.u.z);
					this._lastDotProductOfAnimationAndTargetRotation = num7;
				}
				break;
			case SiegeLadder.LadderState.BeingRaisedStopped:
			case SiegeLadder.LadderState.BeingPushedBack:
			case SiegeLadder.LadderState.BeingPushedBackStartFromWall:
				if (this._animationState == SiegeLadder.LadderAnimationState.Animated)
				{
					float animationParameterAtChannel4 = this._ladderSkeleton.GetAnimationParameterAtChannel(0);
					if (this._pushingWithForkStandingPoint.HasUser)
					{
						ActionIndexCache actionIndexCache = SiegeLadder.act_usage_ladder_push_back;
						if (this._pushingWithForkStandingPoint.UserAgent.GetCurrentActionValue(1) == actionIndexCache)
						{
							this._pushingWithForkStandingPoint.UserAgent.SetCurrentActionProgress(1, animationParameterAtChannel4);
						}
					}
					bool flag4 = false;
					if (!GameNetwork.IsClientOrReplay)
					{
						if (num > 0)
						{
							if (num != this._currentActionAgentCount)
							{
								this._currentActionAgentCount = num;
								float num8 = MathF.Sqrt((float)this._currentActionAgentCount);
								float animationParameterAtChannel5 = this._ladderSkeleton.GetAnimationParameterAtChannel(0);
								this._ladderObject.SetAnimationAtChannelSynched(this.PushBackAnimation, 0, num8);
								if (animationParameterAtChannel5 > 0f)
								{
									this._ladderObject.SetAnimationChannelParameterSynched(0, animationParameterAtChannel5);
								}
							}
							if (this._pushingWithForkStandingPoint.HasUser)
							{
								ActionIndexCache actionIndexCache2 = SiegeLadder.act_usage_ladder_push_back;
								Agent userAgent3 = this._pushingWithForkStandingPoint.UserAgent;
								if (userAgent3.GetCurrentActionValue(1) != actionIndexCache2 && animationParameterAtChannel4 < 1f && !userAgent3.SetActionChannel(1, actionIndexCache2, false, 0UL, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true) && !userAgent3.IsAIControlled)
								{
									userAgent3.StopUsingGameObject(false, Agent.StopUsingGameObjectFlags.AutoAttachAfterStoppingUsingGameObject);
								}
							}
						}
						else
						{
							this.State = SiegeLadder.LadderState.BeingPushedBackStopped;
							flag4 = true;
						}
					}
					if (!flag4)
					{
						MatrixFrame matrixFrame3 = CS$<>8__locals1.ladderObjectEntity.GetGlobalFrame().TransformToParent(this._ladderSkeleton.GetBoneEntitialFrameWithIndex(0));
						matrixFrame3.rotation.RotateAboutForward(1.5707964f);
						if (animationParameterAtChannel4 > 0.9999f || matrixFrame3.rotation.f.z >= 0f)
						{
							this._animationState = SiegeLadder.LadderAnimationState.PhysicallyDynamic;
							this._fallAngularSpeed = this.GetCurrentLadderAngularSpeed(this._pushBackAnimationIndex);
							CS$<>8__locals1.ladderObjectEntity.SetGlobalFrame(matrixFrame3);
							CS$<>8__locals1.ladderObjectEntity.RecomputeBoundingBox();
							this._ladderSkeleton.SetAnimationAtChannel(this._pushBackAnimationWithoutRootBoneIndex, 0, 1f, -1f, 0f);
							this._ladderSkeleton.SetAnimationParameterAtChannel(0, animationParameterAtChannel4);
							this._ladderSkeleton.TickAnimationsAndForceUpdate(0.0001f, CS$<>8__locals1.ladderObjectEntity.GetGlobalFrame(), false);
							this._ladderSkeleton.SetAnimationAtChannel(this._idleAnimationIndex, 0, 1f, -1f, 0f);
							this._ladderObject.SetLocalPositionSmoothStep(ref this._ladderDownFrame.origin);
						}
					}
				}
				else if (this._animationState == SiegeLadder.LadderAnimationState.PhysicallyDynamic)
				{
					MatrixFrame frame2 = CS$<>8__locals1.ladderObjectEntity.GetFrame();
					frame2.rotation.RotateAboutSide(this._fallAngularSpeed * dt);
					CS$<>8__locals1.ladderObjectEntity.SetFrame(ref frame2);
					CS$<>8__locals1.ladderObjectEntity.RecomputeBoundingBox();
					MatrixFrame matrixFrame4 = CS$<>8__locals1.ladderObjectEntity.GetFrame().TransformToParent(this._ladderSkeleton.GetBoneEntitialFrameWithIndex(0));
					matrixFrame4.rotation.RotateAboutForward(1.5707964f);
					float num9 = Vec3.DotProduct(matrixFrame4.rotation.f, this._ladderDownFrame.rotation.f);
					if (this._fallAngularSpeed > 0f && num9 > 0.95f && num9 < this._lastDotProductOfAnimationAndTargetRotation)
					{
						this._animationState = SiegeLadder.LadderAnimationState.Static;
						CS$<>8__locals1.ladderObjectEntity.SetFrame(ref this._ladderDownFrame);
						CS$<>8__locals1.ladderObjectEntity.RecomputeBoundingBox();
						this._ladderSkeleton.SetAnimationParameterAtChannel(0, 0f);
						this._ladderSkeleton.TickAnimationsAndForceUpdate(0.0001f, CS$<>8__locals1.ladderObjectEntity.GetGlobalFrame(), false);
						this._ladderSkeleton.SetAnimationAtChannel(this._trembleGroundAnimationIndex, 0, 1f, -1f, 0f);
						this._animationState = SiegeLadder.LadderAnimationState.Static;
						if (!GameNetwork.IsClientOrReplay)
						{
							this.State = SiegeLadder.LadderState.FallToLand;
						}
					}
					this._fallAngularSpeed += dt * 2f * MathF.Max(0.3f, 1f - matrixFrame4.rotation.u.z);
					this._lastDotProductOfAnimationAndTargetRotation = num9;
				}
				break;
			case SiegeLadder.LadderState.OnWall:
			case SiegeLadder.LadderState.FallToWall:
				if (num > 0 && !GameNetwork.IsClientOrReplay)
				{
					this.State = SiegeLadder.LadderState.BeingPushedBackStartFromWall;
				}
				break;
			default:
				Debug.FailedAssert("Invalid ladder action state.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Objects\\Siege\\SiegeLadder.cs", "OnTick", 1237);
				break;
			}
			this.CalculateNavigationAndPhysics();
		}

		protected internal override void OnTickParallel(float dt)
		{
			base.OnTickParallel(dt);
			for (int i = 0; i < this._attackerStandingPoints.Count; i++)
			{
				if (this._attackerStandingPoints[i].HasUser)
				{
					if (!this._attackerStandingPoints[i].UserAgent.IsInBeingStruckAction)
					{
						this._attackerStandingPoints[i].UserAgent.SetHandInverseKinematicsFrameForMissionObjectUsage(this._attackerStandingPointLocalIKFrames[i], this._ladderInitialGlobalFrame, 0f);
					}
					else
					{
						this._attackerStandingPoints[i].UserAgent.ClearHandInverseKinematics();
					}
				}
			}
		}

		private void TickRare()
		{
			if (!GameNetwork.IsReplay)
			{
				float num = 20f + (base.ForcedUse ? 3f : 0f);
				num *= num;
				GameEntity gameEntity = base.GameEntity;
				Mission.TeamCollection teams = Mission.Current.Teams;
				int count = teams.Count;
				Vec3 globalPosition = gameEntity.GlobalPosition;
				for (int i = 0; i < count; i++)
				{
					Team team = teams[i];
					if (team.Side == BattleSideEnum.Attacker)
					{
						base.SetForcedUse(false);
						foreach (Formation formation in team.FormationsIncludingSpecialAndEmpty)
						{
							if (formation.CountOfUnits > 0 && formation.QuerySystem.MedianPosition.AsVec2.DistanceSquared(globalPosition.AsVec2) < num && formation.QuerySystem.MedianPosition.GetNavMeshZ() - globalPosition.z < 4f)
							{
								base.SetForcedUse(true);
								break;
							}
						}
					}
				}
			}
		}

		public override UsableMachineAIBase CreateAIBehaviorObject()
		{
			return new SiegeLadderAI(this);
		}

		public void SetUpStateVisibility(bool isVisible)
		{
			base.GameEntity.CollectChildrenEntitiesWithTag(this.upStateEntityTag)[0].SetVisibilityExcludeParents(isVisible);
		}

		private void FlushQueueManager()
		{
			LadderQueueManager queueManagerForAttackers = this._queueManagerForAttackers;
			if (queueManagerForAttackers == null)
			{
				return;
			}
			queueManagerForAttackers.FlushQueueManager();
		}

		private void FlushNeighborQueueManagers()
		{
			foreach (SiegeLadder siegeLadder in (from sl in Mission.Current.ActiveMissionObjects.FindAllWithType<SiegeLadder>()
				where sl.WeaponSide == this.WeaponSide
				select sl).ToList<SiegeLadder>())
			{
				if (siegeLadder != this)
				{
					siegeLadder.FlushQueueManager();
				}
			}
		}

		private void InformNeighborQueueManagers(LadderQueueManager ladderQueueManager)
		{
			foreach (SiegeLadder siegeLadder in (from sl in Mission.Current.ActiveMissionObjects.FindAllWithType<SiegeLadder>()
				where sl.WeaponSide == this.WeaponSide && sl._queueManagerForAttackers != null
				select sl).ToList<SiegeLadder>())
			{
				if (siegeLadder != this && siegeLadder._queueManagerForAttackers != null)
				{
					siegeLadder._queueManagerForAttackers.AssignNeighborQueueManager(ladderQueueManager);
					LadderQueueManager queueManagerForAttackers = this._queueManagerForAttackers;
					if (queueManagerForAttackers != null)
					{
						queueManagerForAttackers.AssignNeighborQueueManager(siegeLadder._queueManagerForAttackers);
					}
				}
			}
		}

		public override void SetAbilityOfFaces(bool enabled)
		{
			base.SetAbilityOfFaces(enabled);
			base.GameEntity.Scene.SetAbilityOfFacesWithId(this.OnWallNavMeshId, enabled);
			if (Mission.Current != null)
			{
				if (enabled)
				{
					this.FlushNeighborQueueManagers();
					this.InformNeighborQueueManagers(this._queueManagerForAttackers);
					return;
				}
				this.InformNeighborQueueManagers(null);
				LadderQueueManager queueManagerForAttackers = this._queueManagerForAttackers;
				if (queueManagerForAttackers == null)
				{
					return;
				}
				queueManagerForAttackers.AssignNeighborQueueManager(null);
			}
		}

		protected internal override void OnMissionReset()
		{
			this._ladderSkeleton.SetAnimationAtChannel(-1, 0, 1f, -1f, 0f);
			if (this.initialState == SiegeLadder.LadderState.OnLand)
			{
				if (!GameNetwork.IsClientOrReplay)
				{
					this.State = SiegeLadder.LadderState.OnLand;
				}
				this._ladderObject.GameEntity.SetFrame(ref this._ladderDownFrame);
				this._ladderObject.GameEntity.RecomputeBoundingBox();
				return;
			}
			if (!GameNetwork.IsClientOrReplay)
			{
				this.State = SiegeLadder.LadderState.OnWall;
			}
			this._ladderObject.GameEntity.SetFrame(ref this._ladderUpFrame);
			this._ladderObject.GameEntity.RecomputeBoundingBox();
		}

		public override string GetDescriptionText(GameEntity gameEntity)
		{
			if (!gameEntity.HasTag(this.AmmoPickUpTag))
			{
				return new TextObject("{=G0AWk1rX}Ladder", null).ToString();
			}
			return new TextObject("{=F9AQxCax}Fork", null).ToString();
		}

		public override TextObject GetActionTextForStandingPoint(UsableMissionObject usableGameObject)
		{
			TextObject textObject;
			if (usableGameObject.GameEntity.HasTag(this.AmmoPickUpTag))
			{
				textObject = new TextObject("{=bNYm3K6b}{KEY} Pick Up", null);
			}
			else
			{
				textObject = (usableGameObject.GameEntity.HasTag(this.AttackerTag) ? new TextObject("{=kbNcm68J}{KEY} Lift", null) : new TextObject("{=MdQJxiGz}{KEY} Push", null));
			}
			textObject.SetTextVariable("KEY", HyperlinkTexts.GetKeyHyperlinkText(HotKeyManager.GetHotKeyId("CombatHotKeyCategory", 13)));
			return textObject;
		}

		public override bool ReadFromNetwork()
		{
			bool flag = base.ReadFromNetwork();
			bool flag2 = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			int num = GameNetworkMessage.ReadIntFromPacket(CompressionMission.SiegeLadderStateCompressionInfo, ref flag);
			int num2 = GameNetworkMessage.ReadIntFromPacket(CompressionMission.SiegeLadderAnimationStateCompressionInfo, ref flag);
			float num3 = GameNetworkMessage.ReadFloatFromPacket(CompressionMission.SiegeMachineComponentAngularSpeedCompressionInfo, ref flag);
			MatrixFrame matrixFrame = GameNetworkMessage.ReadMatrixFrameFromPacket(ref flag);
			bool flag3 = GameNetworkMessage.ReadBoolFromPacket(ref flag);
			int num4 = -1;
			float num5 = 0f;
			if (flag3)
			{
				num4 = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.AnimationIndexCompressionInfo, ref flag);
				num5 = GameNetworkMessage.ReadFloatFromPacket(CompressionBasic.AnimationProgressCompressionInfo, ref flag);
			}
			if (flag)
			{
				this.initialState = (flag2 ? SiegeLadder.LadderState.OnLand : SiegeLadder.LadderState.OnWall);
				this._state = (SiegeLadder.LadderState)num;
				this._animationState = (SiegeLadder.LadderAnimationState)num2;
				this._fallAngularSpeed = num3;
				this._lastDotProductOfAnimationAndTargetRotation = -1000f;
				matrixFrame.rotation.Orthonormalize();
				this._ladderObject.GameEntity.SetGlobalFrame(matrixFrame);
				this._ladderObject.GameEntity.RecomputeBoundingBox();
				if (num4 >= 0)
				{
					this._ladderSkeleton.SetAnimationAtChannel(num4, 0, 1f, -1f, 0f);
					this._ladderSkeleton.SetAnimationParameterAtChannel(0, MBMath.ClampFloat(num5, 0f, 1f));
					this._ladderSkeleton.ForceUpdateBoneFrames();
				}
			}
			return flag;
		}

		public override void WriteToNetwork()
		{
			base.WriteToNetwork();
			GameNetworkMessage.WriteBoolToPacket(this.initialState == SiegeLadder.LadderState.OnLand);
			GameNetworkMessage.WriteIntToPacket((int)this.State, CompressionMission.SiegeLadderStateCompressionInfo);
			GameNetworkMessage.WriteIntToPacket((int)this._animationState, CompressionMission.SiegeLadderAnimationStateCompressionInfo);
			GameNetworkMessage.WriteFloatToPacket(this._fallAngularSpeed, CompressionMission.SiegeMachineComponentAngularSpeedCompressionInfo);
			GameNetworkMessage.WriteMatrixFrameToPacket(this._ladderObject.GameEntity.GetGlobalFrame());
			int animationIndexAtChannel = this._ladderSkeleton.GetAnimationIndexAtChannel(0);
			GameNetworkMessage.WriteBoolToPacket(animationIndexAtChannel >= 0);
			if (animationIndexAtChannel >= 0)
			{
				GameNetworkMessage.WriteIntToPacket(animationIndexAtChannel, CompressionBasic.AnimationIndexCompressionInfo);
				GameNetworkMessage.WriteFloatToPacket(this._ladderSkeleton.GetAnimationParameterAtChannel(0), CompressionBasic.AnimationProgressCompressionInfo);
			}
		}

		bool IOrderableWithInteractionArea.IsPointInsideInteractionArea(Vec3 point)
		{
			GameEntity gameEntity = base.GameEntity.CollectChildrenEntitiesWithTag("ui_interaction").FirstOrDefault<GameEntity>();
			return !(gameEntity == null) && gameEntity.GlobalPosition.AsVec2.DistanceSquared(point.AsVec2) < 25f;
		}

		public override TargetFlags GetTargetFlags()
		{
			TargetFlags targetFlags = TargetFlags.None;
			targetFlags |= TargetFlags.IsFlammable;
			targetFlags |= TargetFlags.IsSiegeEngine;
			targetFlags |= TargetFlags.IsAttacker;
			if (this.HasCompletedAction() || this.IsDeactivated)
			{
				targetFlags |= TargetFlags.NotAThreat;
			}
			return targetFlags;
		}

		public override float GetTargetValue(List<Vec3> weaponPos)
		{
			return 10f * base.GetUserMultiplierOfWeapon() * this.GetDistanceMultiplierOfWeapon(weaponPos[0]);
		}

		protected override float GetDistanceMultiplierOfWeapon(Vec3 weaponPos)
		{
			if (this.GetMinimumDistanceBetweenPositions(weaponPos) >= 10f)
			{
				return 0.9f;
			}
			return 1f;
		}

		protected override StandingPoint GetSuitableStandingPointFor(BattleSideEnum side, Agent agent = null, List<Agent> agents = null, List<ValueTuple<Agent, float>> agentValuePairs = null)
		{
			if (side == BattleSideEnum.Attacker)
			{
				return this._attackerStandingPoints.FirstOrDefault((StandingPoint sp) => !sp.IsDeactivated && (sp.IsInstantUse || (!sp.HasUser && !sp.HasAIMovingTo)));
			}
			if (this._pushingWithForkStandingPoint.IsDeactivated || (!this._pushingWithForkStandingPoint.IsInstantUse && (this._pushingWithForkStandingPoint.HasUser || this._pushingWithForkStandingPoint.HasAIMovingTo)))
			{
				return null;
			}
			return this._pushingWithForkStandingPoint;
		}

		public void SetSpawnedFromSpawner()
		{
			this._spawnedFromSpawner = true;
		}

		public void AssignParametersFromSpawner(string sideTag, string targetWallSegment, int onWallNavMeshId, float downStateRotationRadian, float upperStateRotationRadian, string barrierTagToRemove, string indestructibleMerlonsTag)
		{
			this._sideTag = sideTag;
			this._targetWallSegmentTag = targetWallSegment;
			this.OnWallNavMeshId = onWallNavMeshId;
			this._downStateRotationRadian = downStateRotationRadian;
			this._upStateRotationRadian = upperStateRotationRadian;
			this.BarrierTagToRemove = barrierTagToRemove;
			this.IndestructibleMerlonsTag = indestructibleMerlonsTag;
		}

		public bool GetNavmeshFaceIds(out List<int> navmeshFaceIds)
		{
			navmeshFaceIds = new List<int> { this.OnWallNavMeshId };
			return true;
		}

		[CompilerGenerated]
		private bool <OnTick>g__GetIsSomeoneOver|113_0(ref SiegeLadder.<>c__DisplayClass113_0 A_1, ref SiegeLadder.<>c__DisplayClass113_1 A_2)
		{
			if (A_2.someoneOver == null)
			{
				A_2.someoneOver = new bool?(false);
				if (!GameNetwork.IsMultiplayer)
				{
					Vec3 vec;
					Vec3 vec2;
					A_1.ladderObjectEntity.GetPhysicsMinMax(true, out vec, out vec2, false);
					float num = (vec2 - vec).AsVec2.Length * 0.5f;
					AgentProximityMap.ProximityMapSearchStruct proximityMapSearchStruct = AgentProximityMap.BeginSearch(Mission.Current, A_1.ladderObjectEntity.GlobalPosition.AsVec2, num, false);
					while (proximityMapSearchStruct.LastFoundAgent != null)
					{
						if (proximityMapSearchStruct.LastFoundAgent.GetSteppedMachine() == this)
						{
							A_2.someoneOver = new bool?(true);
							break;
						}
						AgentProximityMap.FindNext(Mission.Current, ref proximityMapSearchStruct);
					}
				}
			}
			return A_2.someoneOver.Value;
		}

		private static readonly ActionIndexCache act_usage_ladder_lift_from_left_1_start = ActionIndexCache.Create("act_usage_ladder_lift_from_left_1_start");

		private static readonly ActionIndexCache act_usage_ladder_lift_from_left_2_start = ActionIndexCache.Create("act_usage_ladder_lift_from_left_2_start");

		public const float ClimbingLimitRadian = -0.20135832f;

		private static readonly ActionIndexCache act_usage_ladder_lift_from_right_1_start = ActionIndexCache.Create("act_usage_ladder_lift_from_right_1_start");

		public const float ClimbingLimitDegree = -11.536982f;

		private static readonly ActionIndexCache act_usage_ladder_lift_from_right_2_start = ActionIndexCache.Create("act_usage_ladder_lift_from_right_2_start");

		public const float AutomaticUseActivationRange = 20f;

		private static readonly ActionIndexCache act_usage_ladder_pick_up_fork_begin = ActionIndexCache.Create("act_usage_ladder_pick_up_fork_begin");

		private static readonly ActionIndexCache act_usage_ladder_pick_up_fork_end = ActionIndexCache.Create("act_usage_ladder_pick_up_fork_end");

		private static readonly ActionIndexCache act_usage_ladder_push_back = ActionIndexCache.Create("act_usage_ladder_push_back");

		private static readonly ActionIndexCache act_usage_ladder_push_back_stopped = ActionIndexCache.Create("act_usage_ladder_push_back_stopped");

		public string AttackerTag = "attacker";

		public string DefenderTag = "defender";

		public string downStateEntityTag = "ladderDown";

		public string IdleAnimation = "siege_ladder_idle";

		public int _idleAnimationIndex = -1;

		public string RaiseAnimation = "siege_ladder_rise";

		public string RaiseAnimationWithoutRootBone = "siege_ladder_rise_wo_rootbone";

		public int _raiseAnimationWithoutRootBoneIndex = -1;

		public string PushBackAnimation = "siege_ladder_push_back";

		public int _pushBackAnimationIndex = -1;

		public string PushBackAnimationWithoutRootBone = "siege_ladder_push_back_wo_rootbone";

		public int _pushBackAnimationWithoutRootBoneIndex = -1;

		public string TrembleWallHeavyAnimation = "siege_ladder_stop_wall_heavy";

		public string TrembleWallLightAnimation = "siege_ladder_stop_wall_light";

		public string TrembleGroundAnimation = "siege_ladder_stop_ground_heavy";

		public string RightStandingPointTag = "right";

		public string LeftStandingPointTag = "left";

		public string FrontStandingPointTag = "front";

		public string PushForkItemID = "push_fork";

		public string upStateEntityTag = "ladderUp";

		public string BodyTag = "ladder_body";

		public string CollisionBodyTag = "ladder_collision_body";

		public string InitialWaitPositionTag = "initialwaitposition";

		private string _targetWallSegmentTag = "";

		private WallSegment _targetWallSegment;

		private string _sideTag;

		private int _trembleWallLightAnimationIndex = -1;

		public string BarrierTagToRemove = "barrier";

		private int _trembleGroundAnimationIndex = -1;

		public SiegeLadder.LadderState initialState;

		private int _trembleWallHeavyAnimationIndex = -1;

		public string IndestructibleMerlonsTag = string.Empty;

		private int _raiseAnimationIndex = -1;

		private bool _isNavigationMeshDisabled;

		private bool _isLadderPhysicsDisabled;

		private bool _isLadderCollisionPhysicsDisabled;

		private Timer _tickOccasionallyTimer;

		private float _upStateRotationRadian;

		private float _downStateRotationRadian;

		private float _fallAngularSpeed;

		private MatrixFrame _ladderDownFrame;

		private MatrixFrame _ladderUpFrame;

		private SiegeLadder.LadderAnimationState _animationState;

		private int _currentActionAgentCount;

		private SiegeLadder.LadderState _state;

		private List<GameEntity> _aiBarriers;

		private List<StandingPoint> _attackerStandingPoints;

		private StandingPointWithWeaponRequirement _pushingWithForkStandingPoint;

		private StandingPointWithWeaponRequirement _forkPickUpStandingPoint;

		private ItemObject _forkItem;

		private MatrixFrame[] _attackerStandingPointLocalIKFrames;

		private MatrixFrame _ladderInitialGlobalFrame;

		private SynchedMissionObject _ladderParticleObject;

		private SynchedMissionObject _ladderBodyObject;

		private SynchedMissionObject _ladderCollisionBodyObject;

		private SynchedMissionObject _ladderObject;

		private Skeleton _ladderSkeleton;

		private float _lastDotProductOfAnimationAndTargetRotation;

		private LadderQueueManager _queueManagerForAttackers;

		private LadderQueueManager _queueManagerForDefenders;

		private Timer _forkReappearingTimer;

		private float _forkReappearingDelay = 10f;

		private SynchedMissionObject _forkEntity;

		public enum LadderState
		{
			OnLand,
			FallToLand,
			BeingRaised,
			BeingRaisedStartFromGround,
			BeingRaisedStopped,
			OnWall,
			FallToWall,
			BeingPushedBack,
			BeingPushedBackStartFromWall,
			BeingPushedBackStopped,
			NumberOfStates
		}

		public enum LadderAnimationState
		{
			Static,
			Animated,
			PhysicallyDynamic,
			NumberOfStates
		}
	}
}
