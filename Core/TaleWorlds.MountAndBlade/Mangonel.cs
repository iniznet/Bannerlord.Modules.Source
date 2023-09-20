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
	public class Mangonel : RangedSiegeWeapon, ISpawnable
	{
		protected override float MaximumBallisticError
		{
			get
			{
				return 1.5f;
			}
		}

		protected override float ShootingSpeed
		{
			get
			{
				return this.ProjectileSpeed;
			}
		}

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

		public override UsableMachineAIBase CreateAIBehaviorObject()
		{
			return new MangonelAI(this);
		}

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

		public override SiegeEngineType GetSiegeEngineType()
		{
			if (this._defaultSide != BattleSideEnum.Attacker)
			{
				return DefaultSiegeEngineTypes.Catapult;
			}
			return DefaultSiegeEngineTypes.Onager;
		}

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
			this.EnemyRangeToStopUsing = 9f;
			base.SetScriptComponentToTick(this.GetTickRequirement());
		}

		protected internal override void OnEditorInit()
		{
		}

		protected override bool CanRotate()
		{
			return base.State == RangedSiegeWeapon.WeaponState.Idle || base.State == RangedSiegeWeapon.WeaponState.LoadingAmmo || base.State == RangedSiegeWeapon.WeaponState.WaitingBeforeIdle;
		}

		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			if (base.GameEntity.IsVisibleIncludeParents())
			{
				return base.GetTickRequirement() | ScriptComponentBehavior.TickRequirement.Tick | ScriptComponentBehavior.TickRequirement.TickParallel;
			}
			return base.GetTickRequirement();
		}

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

		protected override void SetActivationLoadAmmoPoint(bool activate)
		{
			this.LoadAmmoStandingPoint.SetIsDeactivatedSynched(!activate);
		}

		protected override void UpdateProjectilePosition()
		{
			MatrixFrame boneEntitialFrameWithIndex = this.Skeletons[0].GetBoneEntitialFrameWithIndex(this._missileBoneIndex);
			base.Projectile.GameEntity.SetFrame(ref boneEntitialFrameWithIndex);
		}

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

		protected override void GetSoundEventIndices()
		{
			this.MoveSoundIndex = SoundEvent.GetEventIdFromString("event:/mission/siege/mangonel/move");
			this.ReloadSoundIndex = SoundEvent.GetEventIdFromString("event:/mission/siege/mangonel/reload");
			this.ReloadEndSoundIndex = SoundEvent.GetEventIdFromString("event:/mission/siege/mangonel/reload_end");
		}

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

		protected override float VerticalAimSensitivity
		{
			get
			{
				return 0.1f;
			}
		}

		protected override Vec3 ShootingDirection
		{
			get
			{
				Mat3 rotation = this._body.GameEntity.GetGlobalFrame().rotation;
				rotation.RotateAboutSide(-this.currentReleaseAngle);
				return rotation.TransformToParent(new Vec3(0f, -1f, 0f, -1f));
			}
		}

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

		protected override void ApplyAimChange()
		{
			base.ApplyAimChange();
			this.ShootingDirection.Normalize();
		}

		public override string GetDescriptionText(GameEntity gameEntity = null)
		{
			if (!gameEntity.HasTag(this.AmmoPickUpTag))
			{
				return new TextObject("{=NbpcDXtJ}Mangonel", null).ToString();
			}
			return new TextObject("{=pzfbPbWW}Boulder", null).ToString();
		}

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

		public override float GetTargetValue(List<Vec3> weaponPos)
		{
			return 40f * base.GetUserMultiplierOfWeapon() * this.GetDistanceMultiplierOfWeapon(weaponPos[0]) * base.GetHitPointMultiplierOfWeapon();
		}

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

		protected override float GetDetachmentWeightAux(BattleSideEnum side)
		{
			return base.GetDetachmentWeightAuxForExternalAmmoWeapons(side);
		}

		public void SetSpawnedFromSpawner()
		{
			this._spawnedFromSpawner = true;
		}

		private const string BodyTag = "body";

		private const string RopeTag = "rope";

		private const string RotateTag = "rotate";

		private const string LeftTag = "left";

		private const string VerticalAdjusterTag = "vertical_adjuster";

		private static readonly ActionIndexCache act_usage_mangonel_idle = ActionIndexCache.Create("act_usage_mangonel_idle");

		private static readonly ActionIndexCache act_usage_mangonel_load_ammo_begin = ActionIndexCache.Create("act_usage_mangonel_load_ammo_begin");

		private static readonly ActionIndexCache act_usage_mangonel_load_ammo_end = ActionIndexCache.Create("act_usage_mangonel_load_ammo_end");

		private static readonly ActionIndexCache act_pickup_boulder_begin = ActionIndexCache.Create("act_pickup_boulder_begin");

		private static readonly ActionIndexCache act_pickup_boulder_end = ActionIndexCache.Create("act_pickup_boulder_end");

		private static readonly ActionIndexCache act_usage_mangonel_reload = ActionIndexCache.Create("act_usage_mangonel_reload");

		private static readonly ActionIndexCache act_usage_mangonel_reload_2 = ActionIndexCache.Create("act_usage_mangonel_reload_2");

		private static readonly ActionIndexCache act_usage_mangonel_reload_2_idle = ActionIndexCache.Create("act_usage_mangonel_reload_2_idle");

		private static readonly ActionIndexCache act_usage_mangonel_rotate_left = ActionIndexCache.Create("act_usage_mangonel_rotate_left");

		private static readonly ActionIndexCache act_usage_mangonel_rotate_right = ActionIndexCache.Create("act_usage_mangonel_rotate_right");

		private static readonly ActionIndexCache act_usage_mangonel_shoot = ActionIndexCache.Create("act_usage_mangonel_shoot");

		private static readonly ActionIndexCache act_usage_mangonel_big_idle = ActionIndexCache.Create("act_usage_mangonel_big_idle");

		private static readonly ActionIndexCache act_usage_mangonel_big_shoot = ActionIndexCache.Create("act_usage_mangonel_big_shoot");

		private static readonly ActionIndexCache act_usage_mangonel_big_reload = ActionIndexCache.Create("act_usage_mangonel_big_reload");

		private static readonly ActionIndexCache act_usage_mangonel_big_load_ammo_begin = ActionIndexCache.Create("act_usage_mangonel_big_load_ammo_begin");

		private static readonly ActionIndexCache act_usage_mangonel_big_load_ammo_end = ActionIndexCache.Create("act_usage_mangonel_big_load_ammo_end");

		private static readonly ActionIndexCache act_strike_bent_over = ActionIndexCache.Create("act_strike_bent_over");

		private string _missileBoneName = "end_throwarm";

		private List<StandingPoint> _rotateStandingPoints;

		private SynchedMissionObject _body;

		private SynchedMissionObject _rope;

		private GameEntity _verticalAdjuster;

		private MatrixFrame _verticalAdjusterStartingLocalFrame;

		private Skeleton _verticalAdjusterSkeleton;

		private Skeleton _bodySkeleton;

		private float _timeElapsedAfterLoading;

		private MatrixFrame[] _standingPointLocalIKFrames;

		private StandingPoint _reloadWithoutPilot;

		public string MangonelBodySkeleton = "mangonel_skeleton";

		public string MangonelBodyFire = "mangonel_fire";

		public string MangonelBodyReload = "mangonel_set_up";

		public string MangonelRopeFire = "mangonel_holder_fire";

		public string MangonelRopeReload = "mangonel_holder_set_up";

		public string MangonelAimAnimation = "mangonel_a_anglearm_state";

		public string ProjectileBoneName = "end_throwarm";

		public string IdleActionName;

		public string ShootActionName;

		public string Reload1ActionName;

		public string Reload2ActionName;

		public string RotateLeftActionName;

		public string RotateRightActionName;

		public string LoadAmmoBeginActionName;

		public string LoadAmmoEndActionName;

		public string Reload2IdleActionName;

		public float ProjectileSpeed = 40f;

		private ActionIndexCache _idleAnimationActionIndex;

		private ActionIndexCache _shootAnimationActionIndex;

		private ActionIndexCache _reload1AnimationActionIndex;

		private ActionIndexCache _reload2AnimationActionIndex;

		private ActionIndexCache _rotateLeftAnimationActionIndex;

		private ActionIndexCache _rotateRightAnimationActionIndex;

		private ActionIndexCache _loadAmmoBeginAnimationActionIndex;

		private ActionIndexCache _loadAmmoEndAnimationActionIndex;

		private ActionIndexCache _reload2IdleActionIndex;

		private sbyte _missileBoneIndex;
	}
}
