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
	// Token: 0x02000358 RID: 856
	public class SiegeLadder : SiegeWeapon, IPrimarySiegeWeapon, IOrderableWithInteractionArea, IOrderable, ISpawnable
	{
		// Token: 0x17000855 RID: 2133
		// (get) Token: 0x06002E4C RID: 11852 RVA: 0x000B8A3E File Offset: 0x000B6C3E
		// (set) Token: 0x06002E4D RID: 11853 RVA: 0x000B8A46 File Offset: 0x000B6C46
		public GameEntity InitialWaitPosition { get; private set; }

		// Token: 0x17000856 RID: 2134
		// (get) Token: 0x06002E4E RID: 11854 RVA: 0x000B8A4F File Offset: 0x000B6C4F
		// (set) Token: 0x06002E4F RID: 11855 RVA: 0x000B8A57 File Offset: 0x000B6C57
		public int OnWallNavMeshId { get; private set; }

		// Token: 0x06002E50 RID: 11856 RVA: 0x000B8A60 File Offset: 0x000B6C60
		public override SiegeEngineType GetSiegeEngineType()
		{
			return DefaultSiegeEngineTypes.Ladder;
		}

		// Token: 0x06002E51 RID: 11857 RVA: 0x000B8A68 File Offset: 0x000B6C68
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

		// Token: 0x06002E52 RID: 11858 RVA: 0x000B9258 File Offset: 0x000B7458
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

		// Token: 0x06002E53 RID: 11859 RVA: 0x000B9338 File Offset: 0x000B7538
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

		// Token: 0x06002E54 RID: 11860 RVA: 0x000B9770 File Offset: 0x000B7970
		private void SetVisibilityOfAIBarriers(bool visibility)
		{
			foreach (GameEntity gameEntity in this._aiBarriers)
			{
				gameEntity.SetVisibilityExcludeParents(visibility);
			}
		}

		// Token: 0x17000857 RID: 2135
		// (get) Token: 0x06002E55 RID: 11861 RVA: 0x000B97C4 File Offset: 0x000B79C4
		public int OverTheWallNavMeshID
		{
			get
			{
				return 13;
			}
		}

		// Token: 0x06002E56 RID: 11862 RVA: 0x000B97C8 File Offset: 0x000B79C8
		public override OrderType GetOrder(BattleSideEnum side)
		{
			if (side != BattleSideEnum.Attacker)
			{
				return OrderType.Move;
			}
			return base.GetOrder(side);
		}

		// Token: 0x17000858 RID: 2136
		// (get) Token: 0x06002E57 RID: 11863 RVA: 0x000B97D7 File Offset: 0x000B79D7
		// (set) Token: 0x06002E58 RID: 11864 RVA: 0x000B97DF File Offset: 0x000B79DF
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

		// Token: 0x06002E59 RID: 11865 RVA: 0x000B9820 File Offset: 0x000B7A20
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

		// Token: 0x17000859 RID: 2137
		// (get) Token: 0x06002E5A RID: 11866 RVA: 0x000B99CA File Offset: 0x000B7BCA
		public MissionObject TargetCastlePosition
		{
			get
			{
				return this._targetWallSegment;
			}
		}

		// Token: 0x06002E5B RID: 11867 RVA: 0x000B99D2 File Offset: 0x000B7BD2
		public bool HasCompletedAction()
		{
			return this.State == SiegeLadder.LadderState.OnWall;
		}

		// Token: 0x1700085A RID: 2138
		// (get) Token: 0x06002E5C RID: 11868 RVA: 0x000B99DD File Offset: 0x000B7BDD
		// (set) Token: 0x06002E5D RID: 11869 RVA: 0x000B99E5 File Offset: 0x000B7BE5
		public FormationAI.BehaviorSide WeaponSide { get; private set; }

		// Token: 0x1700085B RID: 2139
		// (get) Token: 0x06002E5E RID: 11870 RVA: 0x000B99EE File Offset: 0x000B7BEE
		public float SiegeWeaponPriority
		{
			get
			{
				return 8f;
			}
		}

		// Token: 0x06002E5F RID: 11871 RVA: 0x000B99F8 File Offset: 0x000B7BF8
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

		// Token: 0x06002E60 RID: 11872 RVA: 0x000B9A50 File Offset: 0x000B7C50
		public override bool IsDisabledForBattleSide(BattleSideEnum sideEnum)
		{
			if (sideEnum == BattleSideEnum.Attacker)
			{
				return this.State == SiegeLadder.LadderState.FallToLand || this.State == SiegeLadder.LadderState.FallToWall || this.State == SiegeLadder.LadderState.OnWall || (this.State == SiegeLadder.LadderState.BeingPushedBack && this._animationState != SiegeLadder.LadderAnimationState.PhysicallyDynamic) || this.State == SiegeLadder.LadderState.BeingPushedBackStartFromWall || this.State == SiegeLadder.LadderState.BeingPushedBackStopped;
			}
			return this.State == SiegeLadder.LadderState.OnLand || this.State == SiegeLadder.LadderState.FallToLand || this.State == SiegeLadder.LadderState.BeingRaised || this.State == SiegeLadder.LadderState.BeingRaisedStartFromGround || this.State == SiegeLadder.LadderState.FallToWall;
		}

		// Token: 0x06002E61 RID: 11873 RVA: 0x000B9AD4 File Offset: 0x000B7CD4
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

		// Token: 0x1700085C RID: 2140
		// (get) Token: 0x06002E62 RID: 11874 RVA: 0x000B9BC6 File Offset: 0x000B7DC6
		public bool HoldLadders
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06002E63 RID: 11875 RVA: 0x000B9BC9 File Offset: 0x000B7DC9
		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			return base.GetTickRequirement() | ScriptComponentBehavior.TickRequirement.Tick | ScriptComponentBehavior.TickRequirement.TickParallel;
		}

		// Token: 0x1700085D RID: 2141
		// (get) Token: 0x06002E64 RID: 11876 RVA: 0x000B9BD5 File Offset: 0x000B7DD5
		public bool SendLadders
		{
			get
			{
				return this.State > SiegeLadder.LadderState.OnLand;
			}
		}

		// Token: 0x06002E65 RID: 11877 RVA: 0x000B9BE0 File Offset: 0x000B7DE0
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

		// Token: 0x06002E66 RID: 11878 RVA: 0x000BAAE4 File Offset: 0x000B8CE4
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

		// Token: 0x06002E67 RID: 11879 RVA: 0x000BAB84 File Offset: 0x000B8D84
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

		// Token: 0x06002E68 RID: 11880 RVA: 0x000BACAC File Offset: 0x000B8EAC
		public override UsableMachineAIBase CreateAIBehaviorObject()
		{
			return new SiegeLadderAI(this);
		}

		// Token: 0x06002E69 RID: 11881 RVA: 0x000BACB4 File Offset: 0x000B8EB4
		public void SetUpStateVisibility(bool isVisible)
		{
			base.GameEntity.CollectChildrenEntitiesWithTag(this.upStateEntityTag)[0].SetVisibilityExcludeParents(isVisible);
		}

		// Token: 0x06002E6A RID: 11882 RVA: 0x000BACD3 File Offset: 0x000B8ED3
		private void FlushQueueManager()
		{
			LadderQueueManager queueManagerForAttackers = this._queueManagerForAttackers;
			if (queueManagerForAttackers == null)
			{
				return;
			}
			queueManagerForAttackers.FlushQueueManager();
		}

		// Token: 0x06002E6B RID: 11883 RVA: 0x000BACE8 File Offset: 0x000B8EE8
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

		// Token: 0x06002E6C RID: 11884 RVA: 0x000BAD60 File Offset: 0x000B8F60
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

		// Token: 0x06002E6D RID: 11885 RVA: 0x000BADFC File Offset: 0x000B8FFC
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

		// Token: 0x06002E6E RID: 11886 RVA: 0x000BAE5C File Offset: 0x000B905C
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

		// Token: 0x06002E6F RID: 11887 RVA: 0x000BAEF6 File Offset: 0x000B90F6
		public override string GetDescriptionText(GameEntity gameEntity)
		{
			if (!gameEntity.HasTag(this.AmmoPickUpTag))
			{
				return new TextObject("{=G0AWk1rX}Ladder", null).ToString();
			}
			return new TextObject("{=F9AQxCax}Fork", null).ToString();
		}

		// Token: 0x06002E70 RID: 11888 RVA: 0x000BAF28 File Offset: 0x000B9128
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

		// Token: 0x06002E71 RID: 11889 RVA: 0x000BAFA0 File Offset: 0x000B91A0
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

		// Token: 0x06002E72 RID: 11890 RVA: 0x000BB0CC File Offset: 0x000B92CC
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

		// Token: 0x06002E73 RID: 11891 RVA: 0x000BB170 File Offset: 0x000B9370
		bool IOrderableWithInteractionArea.IsPointInsideInteractionArea(Vec3 point)
		{
			GameEntity gameEntity = base.GameEntity.CollectChildrenEntitiesWithTag("ui_interaction").FirstOrDefault<GameEntity>();
			return !(gameEntity == null) && gameEntity.GlobalPosition.AsVec2.DistanceSquared(point.AsVec2) < 25f;
		}

		// Token: 0x06002E74 RID: 11892 RVA: 0x000BB1C4 File Offset: 0x000B93C4
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

		// Token: 0x06002E75 RID: 11893 RVA: 0x000BB1F6 File Offset: 0x000B93F6
		public override float GetTargetValue(List<Vec3> weaponPos)
		{
			return 10f * base.GetUserMultiplierOfWeapon() * this.GetDistanceMultiplierOfWeapon(weaponPos[0]);
		}

		// Token: 0x06002E76 RID: 11894 RVA: 0x000BB212 File Offset: 0x000B9412
		protected override float GetDistanceMultiplierOfWeapon(Vec3 weaponPos)
		{
			if (this.GetMinimumDistanceBetweenPositions(weaponPos) >= 10f)
			{
				return 0.9f;
			}
			return 1f;
		}

		// Token: 0x06002E77 RID: 11895 RVA: 0x000BB230 File Offset: 0x000B9430
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

		// Token: 0x06002E78 RID: 11896 RVA: 0x000BB2A8 File Offset: 0x000B94A8
		public void SetSpawnedFromSpawner()
		{
			this._spawnedFromSpawner = true;
		}

		// Token: 0x06002E79 RID: 11897 RVA: 0x000BB2B1 File Offset: 0x000B94B1
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

		// Token: 0x06002E7A RID: 11898 RVA: 0x000BB2E8 File Offset: 0x000B94E8
		public bool GetNavmeshFaceIds(out List<int> navmeshFaceIds)
		{
			navmeshFaceIds = new List<int> { this.OnWallNavMeshId };
			return true;
		}

		// Token: 0x06002E7D RID: 11901 RVA: 0x000BB4D0 File Offset: 0x000B96D0
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

		// Token: 0x04001296 RID: 4758
		private static readonly ActionIndexCache act_usage_ladder_lift_from_left_1_start = ActionIndexCache.Create("act_usage_ladder_lift_from_left_1_start");

		// Token: 0x04001297 RID: 4759
		private static readonly ActionIndexCache act_usage_ladder_lift_from_left_2_start = ActionIndexCache.Create("act_usage_ladder_lift_from_left_2_start");

		// Token: 0x04001298 RID: 4760
		public const float ClimbingLimitRadian = -0.20135832f;

		// Token: 0x04001299 RID: 4761
		private static readonly ActionIndexCache act_usage_ladder_lift_from_right_1_start = ActionIndexCache.Create("act_usage_ladder_lift_from_right_1_start");

		// Token: 0x0400129A RID: 4762
		public const float ClimbingLimitDegree = -11.536982f;

		// Token: 0x0400129B RID: 4763
		private static readonly ActionIndexCache act_usage_ladder_lift_from_right_2_start = ActionIndexCache.Create("act_usage_ladder_lift_from_right_2_start");

		// Token: 0x0400129C RID: 4764
		public const float AutomaticUseActivationRange = 20f;

		// Token: 0x0400129D RID: 4765
		private static readonly ActionIndexCache act_usage_ladder_pick_up_fork_begin = ActionIndexCache.Create("act_usage_ladder_pick_up_fork_begin");

		// Token: 0x0400129E RID: 4766
		private static readonly ActionIndexCache act_usage_ladder_pick_up_fork_end = ActionIndexCache.Create("act_usage_ladder_pick_up_fork_end");

		// Token: 0x0400129F RID: 4767
		private static readonly ActionIndexCache act_usage_ladder_push_back = ActionIndexCache.Create("act_usage_ladder_push_back");

		// Token: 0x040012A0 RID: 4768
		private static readonly ActionIndexCache act_usage_ladder_push_back_stopped = ActionIndexCache.Create("act_usage_ladder_push_back_stopped");

		// Token: 0x040012A1 RID: 4769
		public string AttackerTag = "attacker";

		// Token: 0x040012A2 RID: 4770
		public string DefenderTag = "defender";

		// Token: 0x040012A3 RID: 4771
		public string downStateEntityTag = "ladderDown";

		// Token: 0x040012A4 RID: 4772
		public string IdleAnimation = "siege_ladder_idle";

		// Token: 0x040012A5 RID: 4773
		public int _idleAnimationIndex = -1;

		// Token: 0x040012A6 RID: 4774
		public string RaiseAnimation = "siege_ladder_rise";

		// Token: 0x040012A7 RID: 4775
		public string RaiseAnimationWithoutRootBone = "siege_ladder_rise_wo_rootbone";

		// Token: 0x040012A8 RID: 4776
		public int _raiseAnimationWithoutRootBoneIndex = -1;

		// Token: 0x040012A9 RID: 4777
		public string PushBackAnimation = "siege_ladder_push_back";

		// Token: 0x040012AA RID: 4778
		public int _pushBackAnimationIndex = -1;

		// Token: 0x040012AB RID: 4779
		public string PushBackAnimationWithoutRootBone = "siege_ladder_push_back_wo_rootbone";

		// Token: 0x040012AC RID: 4780
		public int _pushBackAnimationWithoutRootBoneIndex = -1;

		// Token: 0x040012AD RID: 4781
		public string TrembleWallHeavyAnimation = "siege_ladder_stop_wall_heavy";

		// Token: 0x040012AE RID: 4782
		public string TrembleWallLightAnimation = "siege_ladder_stop_wall_light";

		// Token: 0x040012AF RID: 4783
		public string TrembleGroundAnimation = "siege_ladder_stop_ground_heavy";

		// Token: 0x040012B0 RID: 4784
		public string RightStandingPointTag = "right";

		// Token: 0x040012B1 RID: 4785
		public string LeftStandingPointTag = "left";

		// Token: 0x040012B2 RID: 4786
		public string FrontStandingPointTag = "front";

		// Token: 0x040012B3 RID: 4787
		public string PushForkItemID = "push_fork";

		// Token: 0x040012B4 RID: 4788
		public string upStateEntityTag = "ladderUp";

		// Token: 0x040012B5 RID: 4789
		public string BodyTag = "ladder_body";

		// Token: 0x040012B6 RID: 4790
		public string CollisionBodyTag = "ladder_collision_body";

		// Token: 0x040012B7 RID: 4791
		public string InitialWaitPositionTag = "initialwaitposition";

		// Token: 0x040012B8 RID: 4792
		private string _targetWallSegmentTag = "";

		// Token: 0x040012B9 RID: 4793
		private WallSegment _targetWallSegment;

		// Token: 0x040012BA RID: 4794
		private string _sideTag;

		// Token: 0x040012BB RID: 4795
		private int _trembleWallLightAnimationIndex = -1;

		// Token: 0x040012BC RID: 4796
		public string BarrierTagToRemove = "barrier";

		// Token: 0x040012BD RID: 4797
		private int _trembleGroundAnimationIndex = -1;

		// Token: 0x040012BE RID: 4798
		public SiegeLadder.LadderState initialState;

		// Token: 0x040012BF RID: 4799
		private int _trembleWallHeavyAnimationIndex = -1;

		// Token: 0x040012C0 RID: 4800
		public string IndestructibleMerlonsTag = string.Empty;

		// Token: 0x040012C1 RID: 4801
		private int _raiseAnimationIndex = -1;

		// Token: 0x040012C2 RID: 4802
		private bool _isNavigationMeshDisabled;

		// Token: 0x040012C3 RID: 4803
		private bool _isLadderPhysicsDisabled;

		// Token: 0x040012C4 RID: 4804
		private bool _isLadderCollisionPhysicsDisabled;

		// Token: 0x040012C5 RID: 4805
		private Timer _tickOccasionallyTimer;

		// Token: 0x040012C6 RID: 4806
		private float _upStateRotationRadian;

		// Token: 0x040012C7 RID: 4807
		private float _downStateRotationRadian;

		// Token: 0x040012C8 RID: 4808
		private float _fallAngularSpeed;

		// Token: 0x040012C9 RID: 4809
		private MatrixFrame _ladderDownFrame;

		// Token: 0x040012CA RID: 4810
		private MatrixFrame _ladderUpFrame;

		// Token: 0x040012CB RID: 4811
		private SiegeLadder.LadderAnimationState _animationState;

		// Token: 0x040012CC RID: 4812
		private int _currentActionAgentCount;

		// Token: 0x040012CD RID: 4813
		private SiegeLadder.LadderState _state;

		// Token: 0x040012CE RID: 4814
		private List<GameEntity> _aiBarriers;

		// Token: 0x040012CF RID: 4815
		private List<StandingPoint> _attackerStandingPoints;

		// Token: 0x040012D0 RID: 4816
		private StandingPointWithWeaponRequirement _pushingWithForkStandingPoint;

		// Token: 0x040012D1 RID: 4817
		private StandingPointWithWeaponRequirement _forkPickUpStandingPoint;

		// Token: 0x040012D2 RID: 4818
		private ItemObject _forkItem;

		// Token: 0x040012D3 RID: 4819
		private MatrixFrame[] _attackerStandingPointLocalIKFrames;

		// Token: 0x040012D4 RID: 4820
		private MatrixFrame _ladderInitialGlobalFrame;

		// Token: 0x040012D5 RID: 4821
		private SynchedMissionObject _ladderParticleObject;

		// Token: 0x040012D6 RID: 4822
		private SynchedMissionObject _ladderBodyObject;

		// Token: 0x040012D7 RID: 4823
		private SynchedMissionObject _ladderCollisionBodyObject;

		// Token: 0x040012D8 RID: 4824
		private SynchedMissionObject _ladderObject;

		// Token: 0x040012D9 RID: 4825
		private Skeleton _ladderSkeleton;

		// Token: 0x040012DA RID: 4826
		private float _lastDotProductOfAnimationAndTargetRotation;

		// Token: 0x040012DB RID: 4827
		private LadderQueueManager _queueManagerForAttackers;

		// Token: 0x040012DC RID: 4828
		private LadderQueueManager _queueManagerForDefenders;

		// Token: 0x040012DE RID: 4830
		private Timer _forkReappearingTimer;

		// Token: 0x040012DF RID: 4831
		private float _forkReappearingDelay = 10f;

		// Token: 0x040012E1 RID: 4833
		private SynchedMissionObject _forkEntity;

		// Token: 0x02000668 RID: 1640
		public enum LadderState
		{
			// Token: 0x040020CB RID: 8395
			OnLand,
			// Token: 0x040020CC RID: 8396
			FallToLand,
			// Token: 0x040020CD RID: 8397
			BeingRaised,
			// Token: 0x040020CE RID: 8398
			BeingRaisedStartFromGround,
			// Token: 0x040020CF RID: 8399
			BeingRaisedStopped,
			// Token: 0x040020D0 RID: 8400
			OnWall,
			// Token: 0x040020D1 RID: 8401
			FallToWall,
			// Token: 0x040020D2 RID: 8402
			BeingPushedBack,
			// Token: 0x040020D3 RID: 8403
			BeingPushedBackStartFromWall,
			// Token: 0x040020D4 RID: 8404
			BeingPushedBackStopped,
			// Token: 0x040020D5 RID: 8405
			NumberOfStates
		}

		// Token: 0x02000669 RID: 1641
		public enum LadderAnimationState
		{
			// Token: 0x040020D7 RID: 8407
			Static,
			// Token: 0x040020D8 RID: 8408
			Animated,
			// Token: 0x040020D9 RID: 8409
			PhysicallyDynamic,
			// Token: 0x040020DA RID: 8410
			NumberOfStates
		}
	}
}
