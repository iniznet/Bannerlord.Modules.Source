using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Objects.Siege;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000353 RID: 851
	public class Mangonel : RangedSiegeWeapon, ISpawnable
	{
		// Token: 0x17000835 RID: 2101
		// (get) Token: 0x06002DBC RID: 11708 RVA: 0x000B410A File Offset: 0x000B230A
		protected override float MaximumBallisticError
		{
			get
			{
				return 1.5f;
			}
		}

		// Token: 0x17000836 RID: 2102
		// (get) Token: 0x06002DBD RID: 11709 RVA: 0x000B4111 File Offset: 0x000B2311
		protected override float ShootingSpeed
		{
			get
			{
				return this.ProjectileSpeed;
			}
		}

		// Token: 0x06002DBE RID: 11710 RVA: 0x000B411C File Offset: 0x000B231C
		protected override void RegisterAnimationParameters()
		{
			this.SkeletonOwnerObjects = new SynchedMissionObject[2];
			this.Skeletons = new Skeleton[2];
			this.SkeletonNames = new string[1];
			this.FireAnimations = new string[2];
			this.FireAnimationIndices = new int[2];
			this.SetUpAnimations = new string[2];
			this.SetUpAnimationIndices = new int[2];
			this.SkeletonOwnerObjects[0] = this._body;
			this.Skeletons[0] = this._body.GameEntity.Skeleton;
			this.SkeletonNames[0] = this.MangonelBodySkeleton;
			this.FireAnimations[0] = this.MangonelBodyFire;
			this.FireAnimationIndices[0] = MBAnimation.GetAnimationIndexWithName(this.MangonelBodyFire);
			this.SetUpAnimations[0] = this.MangonelBodyReload;
			this.SetUpAnimationIndices[0] = MBAnimation.GetAnimationIndexWithName(this.MangonelBodyReload);
			this.SkeletonOwnerObjects[1] = this._rope;
			this.Skeletons[1] = this._rope.GameEntity.Skeleton;
			this.FireAnimations[1] = this.MangonelRopeFire;
			this.FireAnimationIndices[1] = MBAnimation.GetAnimationIndexWithName(this.MangonelRopeFire);
			this.SetUpAnimations[1] = this.MangonelRopeReload;
			this.SetUpAnimationIndices[1] = MBAnimation.GetAnimationIndexWithName(this.MangonelRopeReload);
			this._missileBoneName = this.ProjectileBoneName;
			this._idleAnimationActionIndex = ActionIndexCache.Create(this.IdleActionName);
			this._shootAnimationActionIndex = ActionIndexCache.Create(this.ShootActionName);
			this._reload1AnimationActionIndex = ActionIndexCache.Create(this.Reload1ActionName);
			this._reload2AnimationActionIndex = ActionIndexCache.Create(this.Reload2ActionName);
			this._rotateLeftAnimationActionIndex = ActionIndexCache.Create(this.RotateLeftActionName);
			this._rotateRightAnimationActionIndex = ActionIndexCache.Create(this.RotateRightActionName);
			this._loadAmmoBeginAnimationActionIndex = ActionIndexCache.Create(this.LoadAmmoBeginActionName);
			this._loadAmmoEndAnimationActionIndex = ActionIndexCache.Create(this.LoadAmmoEndActionName);
			this._reload2IdleActionIndex = ActionIndexCache.Create(this.Reload2IdleActionName);
		}

		// Token: 0x06002DBF RID: 11711 RVA: 0x000B4300 File Offset: 0x000B2500
		public override UsableMachineAIBase CreateAIBehaviorObject()
		{
			return new MangonelAI(this);
		}

		// Token: 0x06002DC0 RID: 11712 RVA: 0x000B4308 File Offset: 0x000B2508
		public override void AfterMissionStart()
		{
			if (this.AmmoPickUpStandingPoints != null)
			{
				foreach (StandingPointWithWeaponRequirement standingPointWithWeaponRequirement in this.AmmoPickUpStandingPoints)
				{
					standingPointWithWeaponRequirement.LockUserFrames = true;
				}
			}
			this.UpdateProjectilePosition();
		}

		// Token: 0x06002DC1 RID: 11713 RVA: 0x000B4368 File Offset: 0x000B2568
		public override SiegeEngineType GetSiegeEngineType()
		{
			if (this._defaultSide != BattleSideEnum.Attacker)
			{
				return DefaultSiegeEngineTypes.Catapult;
			}
			return DefaultSiegeEngineTypes.Onager;
		}

		// Token: 0x06002DC2 RID: 11714 RVA: 0x000B4380 File Offset: 0x000B2580
		protected internal override void OnInit()
		{
			List<SynchedMissionObject> list = base.GameEntity.CollectObjectsWithTag("rope");
			if (list.Count > 0)
			{
				this._rope = list[0];
			}
			list = base.GameEntity.CollectObjectsWithTag("body");
			this._body = list[0];
			this._bodySkeleton = this._body.GameEntity.Skeleton;
			this.RotationObject = this._body;
			List<GameEntity> list2 = base.GameEntity.CollectChildrenEntitiesWithTag("vertical_adjuster");
			this._verticalAdjuster = list2[0];
			this._verticalAdjusterSkeleton = this._verticalAdjuster.Skeleton;
			if (this._verticalAdjusterSkeleton != null)
			{
				this._verticalAdjusterSkeleton.SetAnimationAtChannel(this.MangonelAimAnimation, 0, 1f, -1f, 0f);
			}
			this._verticalAdjusterStartingLocalFrame = this._verticalAdjuster.GetFrame();
			this._verticalAdjusterStartingLocalFrame = this._body.GameEntity.GetBoneEntitialFrameWithIndex(0).TransformToLocal(this._verticalAdjusterStartingLocalFrame);
			base.OnInit();
			this.timeGapBetweenShootActionAndProjectileLeaving = 0.23f;
			this.timeGapBetweenShootingEndAndReloadingStart = 0f;
			this._rotateStandingPoints = new List<StandingPoint>();
			if (base.StandingPoints != null)
			{
				foreach (StandingPoint standingPoint in base.StandingPoints)
				{
					if (standingPoint.GameEntity.HasTag("rotate"))
					{
						if (standingPoint.GameEntity.HasTag("left") && this._rotateStandingPoints.Count > 0)
						{
							this._rotateStandingPoints.Insert(0, standingPoint);
						}
						else
						{
							this._rotateStandingPoints.Add(standingPoint);
						}
					}
				}
				MatrixFrame globalFrame = this._body.GameEntity.GetGlobalFrame();
				this._standingPointLocalIKFrames = new MatrixFrame[base.StandingPoints.Count];
				for (int i = 0; i < base.StandingPoints.Count; i++)
				{
					this._standingPointLocalIKFrames[i] = base.StandingPoints[i].GameEntity.GetGlobalFrame().TransformToLocal(globalFrame);
					base.StandingPoints[i].AddComponent(new ClearHandInverseKinematicsOnStopUsageComponent());
				}
			}
			this._missileBoneIndex = Skeleton.GetBoneIndexFromName(this.Skeletons[0].GetName(), this._missileBoneName);
			this.ApplyAimChange();
			foreach (StandingPoint standingPoint2 in this.ReloadStandingPoints)
			{
				if (standingPoint2 != base.PilotStandingPoint)
				{
					this._reloadWithoutPilot = standingPoint2;
				}
			}
			if (!GameNetwork.IsClientOrReplay)
			{
				this.SetActivationLoadAmmoPoint(false);
			}
			this.EnemyRangeToStopUsing = 7f;
			base.SetScriptComponentToTick(this.GetTickRequirement());
		}

		// Token: 0x06002DC3 RID: 11715 RVA: 0x000B466C File Offset: 0x000B286C
		protected internal override void OnEditorInit()
		{
		}

		// Token: 0x06002DC4 RID: 11716 RVA: 0x000B466E File Offset: 0x000B286E
		protected override bool CanRotate()
		{
			return base.State == RangedSiegeWeapon.WeaponState.Idle || base.State == RangedSiegeWeapon.WeaponState.LoadingAmmo || base.State == RangedSiegeWeapon.WeaponState.WaitingBeforeIdle;
		}

		// Token: 0x06002DC5 RID: 11717 RVA: 0x000B468C File Offset: 0x000B288C
		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			if (base.GameEntity.IsVisibleIncludeParents())
			{
				return base.GetTickRequirement() | ScriptComponentBehavior.TickRequirement.Tick | ScriptComponentBehavior.TickRequirement.TickParallel;
			}
			return base.GetTickRequirement();
		}

		// Token: 0x06002DC6 RID: 11718 RVA: 0x000B46AC File Offset: 0x000B28AC
		protected internal override void OnTick(float dt)
		{
			base.OnTick(dt);
			if (!base.GameEntity.IsVisibleIncludeParents())
			{
				return;
			}
			if (!GameNetwork.IsClientOrReplay)
			{
				foreach (StandingPointWithWeaponRequirement standingPointWithWeaponRequirement in this.AmmoPickUpStandingPoints)
				{
					if (standingPointWithWeaponRequirement.HasUser)
					{
						Agent userAgent = standingPointWithWeaponRequirement.UserAgent;
						ActionIndexValueCache currentActionValue = userAgent.GetCurrentActionValue(1);
						if (!(currentActionValue == Mangonel.act_pickup_boulder_begin))
						{
							if (currentActionValue == Mangonel.act_pickup_boulder_end)
							{
								MissionWeapon missionWeapon = new MissionWeapon(this.OriginalMissileItem, null, null, 1);
								userAgent.EquipWeaponToExtraSlotAndWield(ref missionWeapon);
								userAgent.StopUsingGameObject(true, Agent.StopUsingGameObjectFlags.None);
								this.ConsumeAmmo();
								if (userAgent.IsAIControlled)
								{
									if (!this.LoadAmmoStandingPoint.HasUser && !this.LoadAmmoStandingPoint.IsDeactivated)
									{
										userAgent.AIMoveToGameObjectEnable(this.LoadAmmoStandingPoint, this, base.Ai.GetScriptedFrameFlags(userAgent));
									}
									else if (this.ReloaderAgentOriginalPoint != null && !this.ReloaderAgentOriginalPoint.HasUser && !this.ReloaderAgentOriginalPoint.HasAIMovingTo)
									{
										userAgent.AIMoveToGameObjectEnable(this.ReloaderAgentOriginalPoint, this, base.Ai.GetScriptedFrameFlags(userAgent));
									}
									else
									{
										Agent reloaderAgent = this.ReloaderAgent;
										if (reloaderAgent != null)
										{
											Formation formation = reloaderAgent.Formation;
											if (formation != null)
											{
												formation.AttachUnit(this.ReloaderAgent);
											}
										}
										this.ReloaderAgent = null;
									}
								}
							}
							else if (!userAgent.SetActionChannel(1, Mangonel.act_pickup_boulder_begin, false, 0UL, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true) && userAgent.Controller != Agent.ControllerType.AI)
							{
								userAgent.StopUsingGameObject(true, Agent.StopUsingGameObjectFlags.AutoAttachAfterStoppingUsingGameObject);
							}
						}
					}
				}
			}
			switch (base.State)
			{
			case RangedSiegeWeapon.WeaponState.LoadingAmmo:
				if (!GameNetwork.IsClientOrReplay)
				{
					if (this.LoadAmmoStandingPoint.HasUser)
					{
						Agent userAgent2 = this.LoadAmmoStandingPoint.UserAgent;
						if (userAgent2.GetCurrentActionValue(1) == this._loadAmmoEndAnimationActionIndex)
						{
							EquipmentIndex wieldedItemIndex = userAgent2.GetWieldedItemIndex(Agent.HandIndex.MainHand);
							if (wieldedItemIndex != EquipmentIndex.None && userAgent2.Equipment[wieldedItemIndex].CurrentUsageItem.WeaponClass == this.OriginalMissileItem.PrimaryWeapon.WeaponClass)
							{
								base.ChangeProjectileEntityServer(userAgent2, userAgent2.Equipment[wieldedItemIndex].Item.StringId);
								userAgent2.RemoveEquippedWeapon(wieldedItemIndex);
								this._timeElapsedAfterLoading = 0f;
								base.Projectile.SetVisibleSynched(true, false);
								base.State = RangedSiegeWeapon.WeaponState.WaitingBeforeIdle;
								return;
							}
							userAgent2.StopUsingGameObject(true, Agent.StopUsingGameObjectFlags.None);
							if (!userAgent2.IsPlayerControlled)
							{
								base.SendAgentToAmmoPickup(userAgent2);
								return;
							}
						}
						else if (userAgent2.GetCurrentActionValue(1) != this._loadAmmoBeginAnimationActionIndex && !userAgent2.SetActionChannel(1, this._loadAmmoBeginAnimationActionIndex, false, 0UL, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true))
						{
							for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.NumAllWeaponSlots; equipmentIndex++)
							{
								if (!userAgent2.Equipment[equipmentIndex].IsEmpty && userAgent2.Equipment[equipmentIndex].CurrentUsageItem.WeaponClass == this.OriginalMissileItem.PrimaryWeapon.WeaponClass)
								{
									userAgent2.RemoveEquippedWeapon(equipmentIndex);
								}
							}
							userAgent2.StopUsingGameObject(true, Agent.StopUsingGameObjectFlags.None);
							if (!userAgent2.IsPlayerControlled)
							{
								base.SendAgentToAmmoPickup(userAgent2);
								return;
							}
						}
					}
					else if (this.LoadAmmoStandingPoint.HasAIMovingTo)
					{
						Agent movingAgent = this.LoadAmmoStandingPoint.MovingAgent;
						EquipmentIndex wieldedItemIndex2 = movingAgent.GetWieldedItemIndex(Agent.HandIndex.MainHand);
						if (wieldedItemIndex2 == EquipmentIndex.None || movingAgent.Equipment[wieldedItemIndex2].CurrentUsageItem.WeaponClass != this.OriginalMissileItem.PrimaryWeapon.WeaponClass)
						{
							movingAgent.StopUsingGameObject(true, Agent.StopUsingGameObjectFlags.None);
							base.SendAgentToAmmoPickup(movingAgent);
						}
					}
				}
				break;
			case RangedSiegeWeapon.WeaponState.WaitingBeforeIdle:
				this._timeElapsedAfterLoading += dt;
				if (this._timeElapsedAfterLoading > 1f)
				{
					base.State = RangedSiegeWeapon.WeaponState.Idle;
					return;
				}
				break;
			case RangedSiegeWeapon.WeaponState.Reloading:
			case RangedSiegeWeapon.WeaponState.ReloadingPaused:
				break;
			default:
				return;
			}
		}

		// Token: 0x06002DC7 RID: 11719 RVA: 0x000B4AD8 File Offset: 0x000B2CD8
		protected internal override void OnTickParallel(float dt)
		{
			base.OnTickParallel(dt);
			if (!base.GameEntity.IsVisibleIncludeParents())
			{
				return;
			}
			if (base.State == RangedSiegeWeapon.WeaponState.WaitingBeforeProjectileLeaving)
			{
				this.UpdateProjectilePosition();
			}
			if (this._verticalAdjusterSkeleton != null)
			{
				float num = MBMath.ClampFloat((this.currentReleaseAngle - this.BottomReleaseAngleRestriction) / (this.TopReleaseAngleRestriction - this.BottomReleaseAngleRestriction), 0f, 1f);
				this._verticalAdjusterSkeleton.SetAnimationParameterAtChannel(0, num);
			}
			MatrixFrame matrixFrame = this.Skeletons[0].GetBoneEntitialFrameWithIndex(0).TransformToParent(this._verticalAdjusterStartingLocalFrame);
			this._verticalAdjuster.SetFrame(ref matrixFrame);
			MatrixFrame globalFrame = this._body.GameEntity.GetGlobalFrame();
			for (int i = 0; i < base.StandingPoints.Count; i++)
			{
				if (base.StandingPoints[i].HasUser)
				{
					if (base.StandingPoints[i].UserAgent.IsInBeingStruckAction)
					{
						base.StandingPoints[i].UserAgent.ClearHandInverseKinematics();
					}
					else if (base.StandingPoints[i] != base.PilotStandingPoint)
					{
						if (base.StandingPoints[i].UserAgent.GetCurrentActionValue(1) != this._reload2IdleActionIndex)
						{
							base.StandingPoints[i].UserAgent.SetHandInverseKinematicsFrameForMissionObjectUsage(this._standingPointLocalIKFrames[i], globalFrame, 0f);
						}
						else
						{
							base.StandingPoints[i].UserAgent.ClearHandInverseKinematics();
						}
					}
					else
					{
						base.StandingPoints[i].UserAgent.SetHandInverseKinematicsFrameForMissionObjectUsage(this._standingPointLocalIKFrames[i], globalFrame, 0f);
					}
				}
			}
			if (!GameNetwork.IsClientOrReplay)
			{
				for (int j = 0; j < this._rotateStandingPoints.Count; j++)
				{
					StandingPoint standingPoint = this._rotateStandingPoints[j];
					if (standingPoint.HasUser && !standingPoint.UserAgent.SetActionChannel(1, (j == 0) ? this._rotateLeftAnimationActionIndex : this._rotateRightAnimationActionIndex, false, 0UL, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true) && standingPoint.UserAgent.Controller != Agent.ControllerType.AI)
					{
						standingPoint.UserAgent.StopUsingGameObjectMT(true, Agent.StopUsingGameObjectFlags.AutoAttachAfterStoppingUsingGameObject);
					}
				}
				if (base.PilotAgent != null)
				{
					ActionIndexValueCache currentActionValue = base.PilotAgent.GetCurrentActionValue(1);
					if (base.State == RangedSiegeWeapon.WeaponState.WaitingBeforeProjectileLeaving)
					{
						if (base.PilotAgent.IsInBeingStruckAction)
						{
							if (currentActionValue != ActionIndexValueCache.act_none && currentActionValue != Mangonel.act_strike_bent_over)
							{
								base.PilotAgent.SetActionChannel(1, Mangonel.act_strike_bent_over, false, 0UL, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
							}
						}
						else if (!base.PilotAgent.SetActionChannel(1, this._shootAnimationActionIndex, false, 0UL, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true) && base.PilotAgent.Controller != Agent.ControllerType.AI)
						{
							base.PilotAgent.StopUsingGameObjectMT(true, Agent.StopUsingGameObjectFlags.AutoAttachAfterStoppingUsingGameObject);
						}
					}
					else if (!base.PilotAgent.SetActionChannel(1, this._idleAnimationActionIndex, false, 0UL, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true) && currentActionValue != this._reload1AnimationActionIndex && currentActionValue != this._shootAnimationActionIndex && base.PilotAgent.Controller != Agent.ControllerType.AI)
					{
						base.PilotAgent.StopUsingGameObjectMT(true, Agent.StopUsingGameObjectFlags.AutoAttachAfterStoppingUsingGameObject);
					}
				}
				if (this._reloadWithoutPilot.HasUser)
				{
					Agent userAgent = this._reloadWithoutPilot.UserAgent;
					if (!userAgent.SetActionChannel(1, this._reload2IdleActionIndex, false, 0UL, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true) && userAgent.GetCurrentActionValue(1) != this._reload2AnimationActionIndex && userAgent.Controller != Agent.ControllerType.AI)
					{
						userAgent.StopUsingGameObjectMT(true, Agent.StopUsingGameObjectFlags.AutoAttachAfterStoppingUsingGameObject);
					}
				}
			}
			RangedSiegeWeapon.WeaponState state = base.State;
			if (state == RangedSiegeWeapon.WeaponState.Reloading)
			{
				foreach (StandingPoint standingPoint2 in this.ReloadStandingPoints)
				{
					if (standingPoint2.HasUser)
					{
						ActionIndexValueCache currentActionValue2 = standingPoint2.UserAgent.GetCurrentActionValue(1);
						if (currentActionValue2 == this._reload1AnimationActionIndex || currentActionValue2 == this._reload2AnimationActionIndex)
						{
							standingPoint2.UserAgent.SetCurrentActionProgress(1, this._bodySkeleton.GetAnimationParameterAtChannel(0));
						}
						else if (!GameNetwork.IsClientOrReplay)
						{
							ActionIndexCache actionIndexCache = ((standingPoint2 == base.PilotStandingPoint) ? this._reload1AnimationActionIndex : this._reload2AnimationActionIndex);
							if (!standingPoint2.UserAgent.SetActionChannel(1, actionIndexCache, false, 0UL, 0f, 1f, -0.2f, 0.4f, this._bodySkeleton.GetAnimationParameterAtChannel(0), false, -0.2f, 0, true) && standingPoint2.UserAgent.Controller != Agent.ControllerType.AI)
							{
								standingPoint2.UserAgent.StopUsingGameObjectMT(true, Agent.StopUsingGameObjectFlags.AutoAttachAfterStoppingUsingGameObject);
							}
						}
					}
				}
			}
		}

		// Token: 0x06002DC8 RID: 11720 RVA: 0x000B503C File Offset: 0x000B323C
		protected override void SetActivationLoadAmmoPoint(bool activate)
		{
			this.LoadAmmoStandingPoint.SetIsDeactivatedSynched(!activate);
		}

		// Token: 0x06002DC9 RID: 11721 RVA: 0x000B5050 File Offset: 0x000B3250
		protected override void UpdateProjectilePosition()
		{
			MatrixFrame boneEntitialFrameWithIndex = this.Skeletons[0].GetBoneEntitialFrameWithIndex(this._missileBoneIndex);
			base.Projectile.GameEntity.SetFrame(ref boneEntitialFrameWithIndex);
		}

		// Token: 0x06002DCA RID: 11722 RVA: 0x000B5084 File Offset: 0x000B3284
		protected override void OnRangedSiegeWeaponStateChange()
		{
			base.OnRangedSiegeWeaponStateChange();
			RangedSiegeWeapon.WeaponState state = base.State;
			if (state != RangedSiegeWeapon.WeaponState.Idle)
			{
				if (state != RangedSiegeWeapon.WeaponState.Shooting)
				{
					if (state == RangedSiegeWeapon.WeaponState.WaitingBeforeIdle)
					{
						this.UpdateProjectilePosition();
						return;
					}
				}
				else
				{
					if (!GameNetwork.IsClientOrReplay)
					{
						base.Projectile.SetVisibleSynched(false, false);
						return;
					}
					base.Projectile.GameEntity.SetVisibilityExcludeParents(false);
					return;
				}
			}
			else
			{
				if (!GameNetwork.IsClientOrReplay)
				{
					base.Projectile.SetVisibleSynched(true, false);
					return;
				}
				base.Projectile.GameEntity.SetVisibilityExcludeParents(true);
			}
		}

		// Token: 0x06002DCB RID: 11723 RVA: 0x000B50FD File Offset: 0x000B32FD
		protected override void GetSoundEventIndices()
		{
			this.MoveSoundIndex = SoundEvent.GetEventIdFromString("event:/mission/siege/mangonel/move");
			this.ReloadSoundIndex = SoundEvent.GetEventIdFromString("event:/mission/siege/mangonel/reload");
			this.ReloadEndSoundIndex = SoundEvent.GetEventIdFromString("event:/mission/siege/mangonel/reload_end");
		}

		// Token: 0x17000837 RID: 2103
		// (get) Token: 0x06002DCC RID: 11724 RVA: 0x000B5130 File Offset: 0x000B3330
		protected override float HorizontalAimSensitivity
		{
			get
			{
				if (this._defaultSide == BattleSideEnum.Defender)
				{
					return 0.25f;
				}
				float num = 0.05f;
				foreach (StandingPoint standingPoint in this._rotateStandingPoints)
				{
					if (standingPoint.HasUser && !standingPoint.UserAgent.IsInBeingStruckAction)
					{
						num += 0.1f;
					}
				}
				return num;
			}
		}

		// Token: 0x17000838 RID: 2104
		// (get) Token: 0x06002DCD RID: 11725 RVA: 0x000B51B0 File Offset: 0x000B33B0
		protected override float VerticalAimSensitivity
		{
			get
			{
				return 0.1f;
			}
		}

		// Token: 0x17000839 RID: 2105
		// (get) Token: 0x06002DCE RID: 11726 RVA: 0x000B51B8 File Offset: 0x000B33B8
		protected override Vec3 ShootingDirection
		{
			get
			{
				Mat3 rotation = this._body.GameEntity.GetGlobalFrame().rotation;
				rotation.RotateAboutSide(-this.currentReleaseAngle);
				return rotation.TransformToParent(new Vec3(0f, -1f, 0f, -1f));
			}
		}

		// Token: 0x1700083A RID: 2106
		// (get) Token: 0x06002DCF RID: 11727 RVA: 0x000B520C File Offset: 0x000B340C
		protected override Vec3 VisualizationShootingDirection
		{
			get
			{
				Mat3 rotation = this._body.GameEntity.GetGlobalFrame().rotation;
				rotation.RotateAboutSide(-this.VisualizeReleaseTrajectoryAngle);
				return rotation.TransformToParent(new Vec3(0f, -1f, 0f, -1f));
			}
		}

		// Token: 0x1700083B RID: 2107
		// (get) Token: 0x06002DD0 RID: 11728 RVA: 0x000B525D File Offset: 0x000B345D
		// (set) Token: 0x06002DD1 RID: 11729 RVA: 0x000B5289 File Offset: 0x000B3489
		protected override bool HasAmmo
		{
			get
			{
				return base.HasAmmo || base.CurrentlyUsedAmmoPickUpPoint != null || this.LoadAmmoStandingPoint.HasUser || this.LoadAmmoStandingPoint.HasAIMovingTo;
			}
			set
			{
				base.HasAmmo = value;
			}
		}

		// Token: 0x06002DD2 RID: 11730 RVA: 0x000B5294 File Offset: 0x000B3494
		protected override void ApplyAimChange()
		{
			base.ApplyAimChange();
			this.ShootingDirection.Normalize();
		}

		// Token: 0x06002DD3 RID: 11731 RVA: 0x000B52B6 File Offset: 0x000B34B6
		public override string GetDescriptionText(GameEntity gameEntity = null)
		{
			if (!gameEntity.HasTag(this.AmmoPickUpTag))
			{
				return new TextObject("{=NbpcDXtJ}Mangonel", null).ToString();
			}
			return new TextObject("{=pzfbPbWW}Boulder", null).ToString();
		}

		// Token: 0x06002DD4 RID: 11732 RVA: 0x000B52E8 File Offset: 0x000B34E8
		public override TextObject GetActionTextForStandingPoint(UsableMissionObject usableGameObject)
		{
			TextObject textObject;
			if (usableGameObject.GameEntity.HasTag("reload"))
			{
				textObject = new TextObject((base.PilotStandingPoint == usableGameObject) ? "{=fEQAPJ2e}{KEY} Use" : "{=Na81xuXn}{KEY} Rearm", null);
			}
			else if (usableGameObject.GameEntity.HasTag("rotate"))
			{
				textObject = new TextObject("{=5wx4BF5h}{KEY} Rotate", null);
			}
			else if (usableGameObject.GameEntity.HasTag(this.AmmoPickUpTag))
			{
				textObject = new TextObject("{=bNYm3K6b}{KEY} Pick Up", null);
			}
			else if (usableGameObject.GameEntity.HasTag("ammoload"))
			{
				textObject = new TextObject("{=ibC4xPoo}{KEY} Load Ammo", null);
			}
			else
			{
				textObject = new TextObject("{=fEQAPJ2e}{KEY} Use", null);
			}
			textObject.SetTextVariable("KEY", HyperlinkTexts.GetKeyHyperlinkText(HotKeyManager.GetHotKeyId("CombatHotKeyCategory", 13)));
			return textObject;
		}

		// Token: 0x06002DD5 RID: 11733 RVA: 0x000B53B0 File Offset: 0x000B35B0
		public override TargetFlags GetTargetFlags()
		{
			TargetFlags targetFlags = TargetFlags.None;
			targetFlags |= TargetFlags.IsFlammable;
			targetFlags |= TargetFlags.IsSiegeEngine;
			if (this.Side == BattleSideEnum.Attacker)
			{
				targetFlags |= TargetFlags.IsAttacker;
			}
			if (base.IsDestroyed || this.IsDeactivated)
			{
				targetFlags |= TargetFlags.NotAThreat;
			}
			if (this.Side == BattleSideEnum.Attacker && DebugSiegeBehavior.DebugDefendState == DebugSiegeBehavior.DebugStateDefender.DebugDefendersToMangonels)
			{
				targetFlags |= TargetFlags.DebugThreat;
			}
			if (this.Side == BattleSideEnum.Defender && DebugSiegeBehavior.DebugAttackState == DebugSiegeBehavior.DebugStateAttacker.DebugAttackersToMangonels)
			{
				targetFlags |= TargetFlags.DebugThreat;
			}
			return targetFlags;
		}

		// Token: 0x06002DD6 RID: 11734 RVA: 0x000B541C File Offset: 0x000B361C
		public override float GetTargetValue(List<Vec3> weaponPos)
		{
			return 40f * base.GetUserMultiplierOfWeapon() * this.GetDistanceMultiplierOfWeapon(weaponPos[0]) * base.GetHitPointMultiplierOfWeapon();
		}

		// Token: 0x06002DD7 RID: 11735 RVA: 0x000B5440 File Offset: 0x000B3640
		public override float ProcessTargetValue(float baseValue, TargetFlags flags)
		{
			if (flags.HasAnyFlag(TargetFlags.NotAThreat))
			{
				return -1000f;
			}
			if (flags.HasAnyFlag(TargetFlags.IsSiegeEngine))
			{
				baseValue *= 10000f;
			}
			if (flags.HasAnyFlag(TargetFlags.IsStructure))
			{
				baseValue *= 2.5f;
			}
			if (flags.HasAnyFlag(TargetFlags.IsSmall))
			{
				baseValue *= 8f;
			}
			if (flags.HasAnyFlag(TargetFlags.IsMoving))
			{
				baseValue *= 8f;
			}
			if (flags.HasAnyFlag(TargetFlags.DebugThreat))
			{
				baseValue *= 10000f;
			}
			if (flags.HasAnyFlag(TargetFlags.IsSiegeTower))
			{
				baseValue *= 8f;
			}
			return baseValue;
		}

		// Token: 0x06002DD8 RID: 11736 RVA: 0x000B54D3 File Offset: 0x000B36D3
		protected override float GetDetachmentWeightAux(BattleSideEnum side)
		{
			return base.GetDetachmentWeightAuxForExternalAmmoWeapons(side);
		}

		// Token: 0x06002DD9 RID: 11737 RVA: 0x000B54DC File Offset: 0x000B36DC
		public void SetSpawnedFromSpawner()
		{
			this._spawnedFromSpawner = true;
		}

		// Token: 0x040011F9 RID: 4601
		private const string BodyTag = "body";

		// Token: 0x040011FA RID: 4602
		private const string RopeTag = "rope";

		// Token: 0x040011FB RID: 4603
		private const string RotateTag = "rotate";

		// Token: 0x040011FC RID: 4604
		private const string LeftTag = "left";

		// Token: 0x040011FD RID: 4605
		private const string VerticalAdjusterTag = "vertical_adjuster";

		// Token: 0x040011FE RID: 4606
		private static readonly ActionIndexCache act_usage_mangonel_idle = ActionIndexCache.Create("act_usage_mangonel_idle");

		// Token: 0x040011FF RID: 4607
		private static readonly ActionIndexCache act_usage_mangonel_load_ammo_begin = ActionIndexCache.Create("act_usage_mangonel_load_ammo_begin");

		// Token: 0x04001200 RID: 4608
		private static readonly ActionIndexCache act_usage_mangonel_load_ammo_end = ActionIndexCache.Create("act_usage_mangonel_load_ammo_end");

		// Token: 0x04001201 RID: 4609
		private static readonly ActionIndexCache act_pickup_boulder_begin = ActionIndexCache.Create("act_pickup_boulder_begin");

		// Token: 0x04001202 RID: 4610
		private static readonly ActionIndexCache act_pickup_boulder_end = ActionIndexCache.Create("act_pickup_boulder_end");

		// Token: 0x04001203 RID: 4611
		private static readonly ActionIndexCache act_usage_mangonel_reload = ActionIndexCache.Create("act_usage_mangonel_reload");

		// Token: 0x04001204 RID: 4612
		private static readonly ActionIndexCache act_usage_mangonel_reload_2 = ActionIndexCache.Create("act_usage_mangonel_reload_2");

		// Token: 0x04001205 RID: 4613
		private static readonly ActionIndexCache act_usage_mangonel_reload_2_idle = ActionIndexCache.Create("act_usage_mangonel_reload_2_idle");

		// Token: 0x04001206 RID: 4614
		private static readonly ActionIndexCache act_usage_mangonel_rotate_left = ActionIndexCache.Create("act_usage_mangonel_rotate_left");

		// Token: 0x04001207 RID: 4615
		private static readonly ActionIndexCache act_usage_mangonel_rotate_right = ActionIndexCache.Create("act_usage_mangonel_rotate_right");

		// Token: 0x04001208 RID: 4616
		private static readonly ActionIndexCache act_usage_mangonel_shoot = ActionIndexCache.Create("act_usage_mangonel_shoot");

		// Token: 0x04001209 RID: 4617
		private static readonly ActionIndexCache act_usage_mangonel_big_idle = ActionIndexCache.Create("act_usage_mangonel_big_idle");

		// Token: 0x0400120A RID: 4618
		private static readonly ActionIndexCache act_usage_mangonel_big_shoot = ActionIndexCache.Create("act_usage_mangonel_big_shoot");

		// Token: 0x0400120B RID: 4619
		private static readonly ActionIndexCache act_usage_mangonel_big_reload = ActionIndexCache.Create("act_usage_mangonel_big_reload");

		// Token: 0x0400120C RID: 4620
		private static readonly ActionIndexCache act_usage_mangonel_big_load_ammo_begin = ActionIndexCache.Create("act_usage_mangonel_big_load_ammo_begin");

		// Token: 0x0400120D RID: 4621
		private static readonly ActionIndexCache act_usage_mangonel_big_load_ammo_end = ActionIndexCache.Create("act_usage_mangonel_big_load_ammo_end");

		// Token: 0x0400120E RID: 4622
		private static readonly ActionIndexCache act_strike_bent_over = ActionIndexCache.Create("act_strike_bent_over");

		// Token: 0x0400120F RID: 4623
		private string _missileBoneName = "end_throwarm";

		// Token: 0x04001210 RID: 4624
		private List<StandingPoint> _rotateStandingPoints;

		// Token: 0x04001211 RID: 4625
		private SynchedMissionObject _body;

		// Token: 0x04001212 RID: 4626
		private SynchedMissionObject _rope;

		// Token: 0x04001213 RID: 4627
		private GameEntity _verticalAdjuster;

		// Token: 0x04001214 RID: 4628
		private MatrixFrame _verticalAdjusterStartingLocalFrame;

		// Token: 0x04001215 RID: 4629
		private Skeleton _verticalAdjusterSkeleton;

		// Token: 0x04001216 RID: 4630
		private Skeleton _bodySkeleton;

		// Token: 0x04001217 RID: 4631
		private float _timeElapsedAfterLoading;

		// Token: 0x04001218 RID: 4632
		private MatrixFrame[] _standingPointLocalIKFrames;

		// Token: 0x04001219 RID: 4633
		private StandingPoint _reloadWithoutPilot;

		// Token: 0x0400121A RID: 4634
		public string MangonelBodySkeleton = "mangonel_skeleton";

		// Token: 0x0400121B RID: 4635
		public string MangonelBodyFire = "mangonel_fire";

		// Token: 0x0400121C RID: 4636
		public string MangonelBodyReload = "mangonel_set_up";

		// Token: 0x0400121D RID: 4637
		public string MangonelRopeFire = "mangonel_holder_fire";

		// Token: 0x0400121E RID: 4638
		public string MangonelRopeReload = "mangonel_holder_set_up";

		// Token: 0x0400121F RID: 4639
		public string MangonelAimAnimation = "mangonel_a_anglearm_state";

		// Token: 0x04001220 RID: 4640
		public string ProjectileBoneName = "end_throwarm";

		// Token: 0x04001221 RID: 4641
		public string IdleActionName;

		// Token: 0x04001222 RID: 4642
		public string ShootActionName;

		// Token: 0x04001223 RID: 4643
		public string Reload1ActionName;

		// Token: 0x04001224 RID: 4644
		public string Reload2ActionName;

		// Token: 0x04001225 RID: 4645
		public string RotateLeftActionName;

		// Token: 0x04001226 RID: 4646
		public string RotateRightActionName;

		// Token: 0x04001227 RID: 4647
		public string LoadAmmoBeginActionName;

		// Token: 0x04001228 RID: 4648
		public string LoadAmmoEndActionName;

		// Token: 0x04001229 RID: 4649
		public string Reload2IdleActionName;

		// Token: 0x0400122A RID: 4650
		public float ProjectileSpeed = 40f;

		// Token: 0x0400122B RID: 4651
		private ActionIndexCache _idleAnimationActionIndex;

		// Token: 0x0400122C RID: 4652
		private ActionIndexCache _shootAnimationActionIndex;

		// Token: 0x0400122D RID: 4653
		private ActionIndexCache _reload1AnimationActionIndex;

		// Token: 0x0400122E RID: 4654
		private ActionIndexCache _reload2AnimationActionIndex;

		// Token: 0x0400122F RID: 4655
		private ActionIndexCache _rotateLeftAnimationActionIndex;

		// Token: 0x04001230 RID: 4656
		private ActionIndexCache _rotateRightAnimationActionIndex;

		// Token: 0x04001231 RID: 4657
		private ActionIndexCache _loadAmmoBeginAnimationActionIndex;

		// Token: 0x04001232 RID: 4658
		private ActionIndexCache _loadAmmoEndAnimationActionIndex;

		// Token: 0x04001233 RID: 4659
		private ActionIndexCache _reload2IdleActionIndex;

		// Token: 0x04001234 RID: 4660
		private sbyte _missileBoneIndex;
	}
}
