using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Objects.Siege;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000367 RID: 871
	public class Trebuchet : RangedSiegeWeapon, ISpawnable
	{
		// Token: 0x17000886 RID: 2182
		// (get) Token: 0x06002F6E RID: 12142 RVA: 0x000C1880 File Offset: 0x000BFA80
		public override float DirectionRestriction
		{
			get
			{
				return 1.3962635f;
			}
		}

		// Token: 0x06002F6F RID: 12143 RVA: 0x000C1888 File Offset: 0x000BFA88
		public override TextObject GetActionTextForStandingPoint(UsableMissionObject usableGameObject)
		{
			TextObject textObject;
			if (usableGameObject.GameEntity.HasTag(this.AmmoPickUpTag))
			{
				textObject = new TextObject("{=bNYm3K6b}{KEY} Pick Up", null);
			}
			else if (usableGameObject.GameEntity.HasTag("reload"))
			{
				textObject = new TextObject((base.PilotStandingPoint == usableGameObject) ? "{=fEQAPJ2e}{KEY} Use" : "{=Na81xuXn}{KEY} Rearm", null);
			}
			else if (usableGameObject.GameEntity.HasTag("rotate"))
			{
				textObject = new TextObject("{=5wx4BF5h}{KEY} Rotate", null);
			}
			else if (usableGameObject.GameEntity.HasTag("ammoload"))
			{
				textObject = new TextObject("{=ibC4xPoo}{KEY} Load Ammo", null);
			}
			else
			{
				textObject = TextObject.Empty;
			}
			textObject.SetTextVariable("KEY", HyperlinkTexts.GetKeyHyperlinkText(HotKeyManager.GetHotKeyId("CombatHotKeyCategory", 13)));
			return textObject;
		}

		// Token: 0x06002F70 RID: 12144 RVA: 0x000C194A File Offset: 0x000BFB4A
		public override string GetDescriptionText(GameEntity gameEntity = null)
		{
			if (!gameEntity.HasTag(this.AmmoPickUpTag))
			{
				return new TextObject("{=4Skg9QhO}Trebuchet", null).ToString();
			}
			return new TextObject("{=pzfbPbWW}Boulder", null).ToString();
		}

		// Token: 0x06002F71 RID: 12145 RVA: 0x000C197C File Offset: 0x000BFB7C
		protected override void RegisterAnimationParameters()
		{
			this.SkeletonOwnerObjects = new SynchedMissionObject[3];
			this.Skeletons = new Skeleton[3];
			this.SkeletonNames = new string[3];
			this.FireAnimations = new string[3];
			this.FireAnimationIndices = new int[3];
			this.SetUpAnimations = new string[3];
			this.SetUpAnimationIndices = new int[3];
			this.SkeletonOwnerObjects[0] = this._body;
			this.Skeletons[0] = this._body.GameEntity.Skeleton;
			this.SkeletonNames[0] = "trebuchet_a_skeleton";
			this.FireAnimations[0] = this.BodyFireAnimation;
			this.FireAnimationIndices[0] = MBAnimation.GetAnimationIndexWithName(this.BodyFireAnimation);
			this.SetUpAnimations[0] = this.BodySetUpAnimation;
			this.SetUpAnimationIndices[0] = MBAnimation.GetAnimationIndexWithName(this.BodySetUpAnimation);
			this.SkeletonOwnerObjects[1] = this._sling;
			this.Skeletons[1] = this._sling.GameEntity.Skeleton;
			this.SkeletonNames[1] = "trebuchet_a_sling_skeleton";
			this.FireAnimations[1] = this.SlingFireAnimation;
			this.FireAnimationIndices[1] = MBAnimation.GetAnimationIndexWithName(this.SlingFireAnimation);
			this.SetUpAnimations[1] = this.SlingSetUpAnimation;
			this.SetUpAnimationIndices[1] = MBAnimation.GetAnimationIndexWithName(this.SlingSetUpAnimation);
			this.SkeletonOwnerObjects[2] = this._rope;
			this.Skeletons[2] = this._rope.GameEntity.Skeleton;
			this.SkeletonNames[2] = "trebuchet_a_rope_skeleton";
			this.FireAnimations[2] = this.RopeFireAnimation;
			this.FireAnimationIndices[2] = MBAnimation.GetAnimationIndexWithName(this.RopeFireAnimation);
			this.SetUpAnimations[2] = this.RopeSetUpAnimation;
			this.SetUpAnimationIndices[2] = MBAnimation.GetAnimationIndexWithName(this.RopeSetUpAnimation);
		}

		// Token: 0x06002F72 RID: 12146 RVA: 0x000C1B3C File Offset: 0x000BFD3C
		public override SiegeEngineType GetSiegeEngineType()
		{
			return DefaultSiegeEngineTypes.Trebuchet;
		}

		// Token: 0x06002F73 RID: 12147 RVA: 0x000C1B43 File Offset: 0x000BFD43
		protected override void GetSoundEventIndices()
		{
			this.MoveSoundIndex = SoundEvent.GetEventIdFromString("event:/mission/siege/trebuchet/move");
			this.ReloadSoundIndex = SoundEvent.GetEventIdFromString("event:/mission/siege/trebuchet/reload");
			this.ReloadEndSoundIndex = SoundEvent.GetEventIdFromString("event:/mission/siege/trebuchet/reload_end");
		}

		// Token: 0x17000887 RID: 2183
		// (get) Token: 0x06002F74 RID: 12148 RVA: 0x000C1B75 File Offset: 0x000BFD75
		protected override float ShootingSpeed
		{
			get
			{
				return this.ProjectileSpeed;
			}
		}

		// Token: 0x06002F75 RID: 12149 RVA: 0x000C1B7D File Offset: 0x000BFD7D
		public override UsableMachineAIBase CreateAIBehaviorObject()
		{
			return new TrebuchetAI(this);
		}

		// Token: 0x06002F76 RID: 12150 RVA: 0x000C1B88 File Offset: 0x000BFD88
		protected internal override void OnInit()
		{
			List<SynchedMissionObject> list = base.GameEntity.CollectObjectsWithTag("body");
			this._body = list[0];
			list = base.GameEntity.CollectObjectsWithTag("sling");
			this._sling = list[0];
			list = base.GameEntity.CollectObjectsWithTag("rope");
			this._rope = list[0];
			List<GameEntity> list2 = base.GameEntity.CollectChildrenEntitiesWithTag("vertical_adjuster");
			this._verticalAdjuster = list2[0];
			this._verticalAdjusterSkeleton = this._verticalAdjuster.Skeleton;
			this._verticalAdjusterSkeleton.SetAnimationAtChannel(this.VerticalAdjusterAnimation, 0, 1f, -1f, 0f);
			this._verticalAdjusterStartingLocalFrame = this._verticalAdjuster.GetFrame();
			this._verticalAdjusterStartingLocalFrame = this._body.GameEntity.GetBoneEntitialFrameWithIndex(0).TransformToLocal(this._verticalAdjusterStartingLocalFrame);
			list = base.GameEntity.CollectObjectsWithTag("rotate_entity");
			this.RotationObject = list[0];
			base.OnInit();
			this.timeGapBetweenShootActionAndProjectileLeaving = this.TimeGapBetweenShootActionAndProjectileLeaving;
			this.timeGapBetweenShootingEndAndReloadingStart = 0f;
			this._ammoLoadPoints = new List<StandingPointWithWeaponRequirement>();
			if (base.StandingPoints != null)
			{
				for (int i = 0; i < base.StandingPoints.Count; i++)
				{
					if (base.StandingPoints[i].GameEntity.HasTag("ammoload"))
					{
						this._ammoLoadPoints.Add(base.StandingPoints[i] as StandingPointWithWeaponRequirement);
					}
				}
				MatrixFrame globalFrame = this._body.GameEntity.GetGlobalFrame();
				this._standingPointLocalIKFrames = new MatrixFrame[base.StandingPoints.Count];
				for (int j = 0; j < base.StandingPoints.Count; j++)
				{
					this._standingPointLocalIKFrames[j] = base.StandingPoints[j].GameEntity.GetGlobalFrame().TransformToLocal(globalFrame);
					base.StandingPoints[j].AddComponent(new ClearHandInverseKinematicsOnStopUsageComponent());
				}
			}
			this.ApplyAimChange();
			if (!GameNetwork.IsClientOrReplay)
			{
				this.SetActivationLoadAmmoPoint(false);
				this.EnemyRangeToStopUsing = 9f;
				this.MachinePositionOffsetToStopUsing = new Vec2(-2.05f, -1.9f);
				this._sling.SetAnimationAtChannelSynched((base.State == RangedSiegeWeapon.WeaponState.Idle) ? this.IdleWithAmmoAnimation : this.IdleEmptyAnimation, 0, 1f);
			}
			this._missileBoneIndex = Skeleton.GetBoneIndexFromName(this._sling.GameEntity.Skeleton.GetName(), "bn_projectile_holder");
			this._shootAnimPlayed = false;
			this.UpdateAmmoMesh();
			base.SetScriptComponentToTick(this.GetTickRequirement());
			this.UpdateProjectilePosition();
		}

		// Token: 0x06002F77 RID: 12151 RVA: 0x000C1E40 File Offset: 0x000C0040
		public override void AfterMissionStart()
		{
			if (this.AmmoPickUpStandingPoints != null)
			{
				foreach (StandingPointWithWeaponRequirement standingPointWithWeaponRequirement in this.AmmoPickUpStandingPoints)
				{
					standingPointWithWeaponRequirement.LockUserFrames = true;
				}
			}
			if (this._ammoLoadPoints != null)
			{
				foreach (StandingPointWithWeaponRequirement standingPointWithWeaponRequirement2 in this._ammoLoadPoints)
				{
					standingPointWithWeaponRequirement2.LockUserFrames = true;
				}
			}
		}

		// Token: 0x06002F78 RID: 12152 RVA: 0x000C1EE4 File Offset: 0x000C00E4
		protected override void OnRangedSiegeWeaponStateChange()
		{
			base.OnRangedSiegeWeaponStateChange();
			if (base.State == RangedSiegeWeapon.WeaponState.WaitingBeforeIdle)
			{
				this.UpdateProjectilePosition();
			}
			if (GameNetwork.IsClientOrReplay)
			{
				return;
			}
			RangedSiegeWeapon.WeaponState state = base.State;
			if (state <= RangedSiegeWeapon.WeaponState.Shooting)
			{
				if (state == RangedSiegeWeapon.WeaponState.Idle)
				{
					base.Projectile.SetVisibleSynched(true, false);
					return;
				}
				if (state != RangedSiegeWeapon.WeaponState.Shooting)
				{
					return;
				}
				base.Projectile.SetVisibleSynched(false, false);
				return;
			}
			else
			{
				if (state == RangedSiegeWeapon.WeaponState.LoadingAmmo)
				{
					this._sling.SetAnimationAtChannelSynched(this.IdleEmptyAnimation, 0, 1f);
					return;
				}
				if (state != RangedSiegeWeapon.WeaponState.Reloading)
				{
					return;
				}
				this._shootAnimPlayed = false;
				return;
			}
		}

		// Token: 0x17000888 RID: 2184
		// (get) Token: 0x06002F79 RID: 12153 RVA: 0x000C1F65 File Offset: 0x000C0165
		protected override float HorizontalAimSensitivity
		{
			get
			{
				return 0.1f;
			}
		}

		// Token: 0x17000889 RID: 2185
		// (get) Token: 0x06002F7A RID: 12154 RVA: 0x000C1F6C File Offset: 0x000C016C
		protected override float VerticalAimSensitivity
		{
			get
			{
				return 0.075f;
			}
		}

		// Token: 0x1700088A RID: 2186
		// (get) Token: 0x06002F7B RID: 12155 RVA: 0x000C1F74 File Offset: 0x000C0174
		protected override Vec3 ShootingDirection
		{
			get
			{
				Mat3 rotation = this.RotationObject.GameEntity.GetGlobalFrame().rotation;
				rotation.RotateAboutSide(-this.currentReleaseAngle);
				return rotation.TransformToParent(new Vec3(0f, -1f, 0f, -1f));
			}
		}

		// Token: 0x1700088B RID: 2187
		// (get) Token: 0x06002F7C RID: 12156 RVA: 0x000C1FC8 File Offset: 0x000C01C8
		protected override Vec3 VisualizationShootingDirection
		{
			get
			{
				Mat3 rotation = this.RotationObject.GameEntity.GetGlobalFrame().rotation;
				rotation.RotateAboutSide(-this.VisualizeReleaseTrajectoryAngle);
				return rotation.TransformToParent(new Vec3(0f, -1f, 0f, -1f));
			}
		}

		// Token: 0x1700088C RID: 2188
		// (get) Token: 0x06002F7D RID: 12157 RVA: 0x000C2019 File Offset: 0x000C0219
		// (set) Token: 0x06002F7E RID: 12158 RVA: 0x000C2045 File Offset: 0x000C0245
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

		// Token: 0x06002F7F RID: 12159 RVA: 0x000C2050 File Offset: 0x000C0250
		public override float ProcessTargetValue(float baseValue, TargetFlags flags)
		{
			if (flags.HasAnyFlag(TargetFlags.NotAThreat))
			{
				return -1000f;
			}
			if (flags.HasAnyFlag(TargetFlags.None))
			{
				baseValue *= 1.5f;
			}
			if (flags.HasAnyFlag(TargetFlags.IsSiegeEngine))
			{
				baseValue *= 2.5f;
			}
			if (flags.HasAnyFlag(TargetFlags.IsStructure))
			{
				baseValue *= 0.1f;
			}
			if (flags.HasAnyFlag(TargetFlags.DebugThreat))
			{
				baseValue *= 10000f;
			}
			return baseValue;
		}

		// Token: 0x06002F80 RID: 12160 RVA: 0x000C20BC File Offset: 0x000C02BC
		public override TargetFlags GetTargetFlags()
		{
			TargetFlags targetFlags = TargetFlags.None;
			targetFlags |= TargetFlags.IsFlammable;
			targetFlags |= TargetFlags.IsSiegeEngine;
			targetFlags |= TargetFlags.IsAttacker;
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

		// Token: 0x06002F81 RID: 12161 RVA: 0x000C211F File Offset: 0x000C031F
		public override float GetTargetValue(List<Vec3> weaponPos)
		{
			return 40f * base.GetUserMultiplierOfWeapon() * this.GetDistanceMultiplierOfWeapon(weaponPos[0]) * base.GetHitPointMultiplierOfWeapon();
		}

		// Token: 0x06002F82 RID: 12162 RVA: 0x000C2142 File Offset: 0x000C0342
		protected override bool CanRotate()
		{
			return base.State == RangedSiegeWeapon.WeaponState.Idle || base.State == RangedSiegeWeapon.WeaponState.LoadingAmmo || base.State == RangedSiegeWeapon.WeaponState.WaitingBeforeIdle;
		}

		// Token: 0x06002F83 RID: 12163 RVA: 0x000C2160 File Offset: 0x000C0360
		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			if (base.GameEntity.IsVisibleIncludeParents())
			{
				return base.GetTickRequirement() | ScriptComponentBehavior.TickRequirement.Tick | ScriptComponentBehavior.TickRequirement.TickParallel;
			}
			return base.GetTickRequirement();
		}

		// Token: 0x06002F84 RID: 12164 RVA: 0x000C2180 File Offset: 0x000C0380
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
						if (!(currentActionValue == Trebuchet.act_pickup_boulder_begin))
						{
							if (currentActionValue == Trebuchet.act_pickup_boulder_end)
							{
								MissionWeapon missionWeapon = new MissionWeapon(this.OriginalMissileItem, null, null);
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
							else if (!userAgent.SetActionChannel(1, Trebuchet.act_pickup_boulder_begin, false, 0UL, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true) && userAgent.Controller != Agent.ControllerType.AI)
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
					bool flag = false;
					foreach (StandingPointWithWeaponRequirement standingPointWithWeaponRequirement2 in this._ammoLoadPoints)
					{
						if (flag)
						{
							if (standingPointWithWeaponRequirement2.IsDeactivated)
							{
								if ((standingPointWithWeaponRequirement2.HasUser || standingPointWithWeaponRequirement2.HasAIMovingTo) && (standingPointWithWeaponRequirement2.UserAgent == this.ReloaderAgent || standingPointWithWeaponRequirement2.MovingAgent == this.ReloaderAgent))
								{
									base.SendReloaderAgentToOriginalPoint();
								}
								standingPointWithWeaponRequirement2.SetIsDeactivatedSynched(true);
							}
						}
						else if (standingPointWithWeaponRequirement2.HasUser)
						{
							flag = true;
							Agent userAgent2 = standingPointWithWeaponRequirement2.UserAgent;
							ActionIndexValueCache currentActionValue2 = userAgent2.GetCurrentActionValue(1);
							if (currentActionValue2 == Trebuchet.act_usage_trebuchet_load_ammo && userAgent2.GetCurrentActionProgress(1) > 0.56f)
							{
								EquipmentIndex wieldedItemIndex = userAgent2.GetWieldedItemIndex(Agent.HandIndex.MainHand);
								if (wieldedItemIndex != EquipmentIndex.None && userAgent2.Equipment[wieldedItemIndex].CurrentUsageItem.WeaponClass == this.OriginalMissileItem.PrimaryWeapon.WeaponClass)
								{
									base.ChangeProjectileEntityServer(userAgent2, userAgent2.Equipment[wieldedItemIndex].Item.StringId);
									userAgent2.RemoveEquippedWeapon(wieldedItemIndex);
									this._timeElapsedAfterLoading = 0f;
									base.Projectile.SetVisibleSynched(true, false);
									this._sling.SetAnimationAtChannelSynched(this.IdleWithAmmoAnimation, 0, 1f);
									base.State = RangedSiegeWeapon.WeaponState.WaitingBeforeIdle;
								}
								else
								{
									userAgent2.StopUsingGameObject(true, Agent.StopUsingGameObjectFlags.None);
									if (!userAgent2.IsPlayerControlled)
									{
										base.SendAgentToAmmoPickup(userAgent2);
									}
								}
							}
							else if (currentActionValue2 != Trebuchet.act_usage_trebuchet_load_ammo && !userAgent2.SetActionChannel(1, Trebuchet.act_usage_trebuchet_load_ammo, false, 0UL, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true))
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
								}
							}
						}
						else if (standingPointWithWeaponRequirement2.HasAIMovingTo)
						{
							Agent movingAgent = standingPointWithWeaponRequirement2.MovingAgent;
							EquipmentIndex wieldedItemIndex2 = movingAgent.GetWieldedItemIndex(Agent.HandIndex.MainHand);
							if (wieldedItemIndex2 == EquipmentIndex.None || movingAgent.Equipment[wieldedItemIndex2].CurrentUsageItem.WeaponClass != this.OriginalMissileItem.PrimaryWeapon.WeaponClass)
							{
								movingAgent.StopUsingGameObject(true, Agent.StopUsingGameObjectFlags.None);
								base.SendAgentToAmmoPickup(movingAgent);
							}
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

		// Token: 0x06002F85 RID: 12165 RVA: 0x000C2674 File Offset: 0x000C0874
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
			float num = MBMath.ClampFloat((this.currentReleaseAngle - this.BottomReleaseAngleRestriction) / (this.TopReleaseAngleRestriction - this.BottomReleaseAngleRestriction), 0f, 1f);
			this._verticalAdjusterSkeleton.SetAnimationParameterAtChannel(0, num);
			MatrixFrame matrixFrame = this._body.GameEntity.GetBoneEntitialFrameWithIndex(0).TransformToParent(this._verticalAdjusterStartingLocalFrame);
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
						if (base.StandingPoints[i].UserAgent.GetCurrentActionValue(1) == Trebuchet.act_usage_trebuchet_reload_2)
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
				if (base.PilotAgent != null)
				{
					ActionIndexValueCache currentActionValue = base.PilotAgent.GetCurrentActionValue(1);
					if (base.State == RangedSiegeWeapon.WeaponState.WaitingBeforeProjectileLeaving || base.State == RangedSiegeWeapon.WeaponState.Shooting || base.State == RangedSiegeWeapon.WeaponState.WaitingBeforeReloading)
					{
						if (!this._shootAnimPlayed && currentActionValue != Trebuchet.act_usage_trebuchet_shoot)
						{
							this._shootAnimPlayed = base.PilotAgent.SetActionChannel(1, Trebuchet.act_usage_trebuchet_shoot, false, 0UL, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
						}
						else if (currentActionValue != Trebuchet.act_usage_trebuchet_shoot && !base.PilotAgent.SetActionChannel(1, Trebuchet.act_usage_trebuchet_reload_idle, false, 0UL, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true) && base.PilotAgent.Controller != Agent.ControllerType.AI)
						{
							base.PilotAgent.StopUsingGameObjectMT(true, Agent.StopUsingGameObjectFlags.AutoAttachAfterStoppingUsingGameObject);
						}
					}
					else if (currentActionValue != Trebuchet.act_usage_trebuchet_reload && currentActionValue != Trebuchet.act_usage_trebuchet_shoot && !base.PilotAgent.SetActionChannel(1, Trebuchet.act_usage_trebuchet_idle, false, 0UL, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true) && base.PilotAgent.Controller != Agent.ControllerType.AI)
					{
						base.PilotAgent.StopUsingGameObjectMT(true, Agent.StopUsingGameObjectFlags.AutoAttachAfterStoppingUsingGameObject);
					}
				}
				if (base.State != RangedSiegeWeapon.WeaponState.Reloading)
				{
					foreach (StandingPoint standingPoint in this.ReloadStandingPoints)
					{
						if (standingPoint.HasUser && standingPoint != base.PilotStandingPoint)
						{
							Agent userAgent = standingPoint.UserAgent;
							if (!userAgent.SetActionChannel(1, Trebuchet.act_usage_trebuchet_reload_2_idle, false, 0UL, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true) && userAgent.Controller != Agent.ControllerType.AI)
							{
								userAgent.StopUsingGameObjectMT(true, Agent.StopUsingGameObjectFlags.AutoAttachAfterStoppingUsingGameObject);
							}
						}
					}
				}
				foreach (StandingPoint standingPoint2 in base.StandingPoints)
				{
					if (standingPoint2.HasUser && this.ReloadStandingPoints.IndexOf(standingPoint2) < 0 && (!(standingPoint2 is StandingPointWithWeaponRequirement) || (this._ammoLoadPoints.IndexOf((StandingPointWithWeaponRequirement)standingPoint2) < 0 && this.AmmoPickUpStandingPoints.IndexOf((StandingPointWithWeaponRequirement)standingPoint2) < 0)))
					{
						Agent userAgent2 = standingPoint2.UserAgent;
						if (!userAgent2.SetActionChannel(1, Trebuchet.act_usage_trebuchet_reload_2_idle, false, 0UL, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true) && userAgent2.Controller != Agent.ControllerType.AI)
						{
							userAgent2.StopUsingGameObjectMT(true, Agent.StopUsingGameObjectFlags.AutoAttachAfterStoppingUsingGameObject);
						}
					}
				}
			}
			RangedSiegeWeapon.WeaponState state = base.State;
			if (state == RangedSiegeWeapon.WeaponState.Reloading)
			{
				for (int j = 0; j < this.ReloadStandingPoints.Count; j++)
				{
					if (this.ReloadStandingPoints[j].HasUser)
					{
						Agent userAgent3 = this.ReloadStandingPoints[j].UserAgent;
						ActionIndexValueCache currentActionValue2 = userAgent3.GetCurrentActionValue(1);
						if (currentActionValue2 == Trebuchet.act_usage_trebuchet_reload || currentActionValue2 == Trebuchet.act_usage_trebuchet_reload_2)
						{
							userAgent3.SetCurrentActionProgress(1, this.Skeletons[0].GetAnimationParameterAtChannel(0));
						}
						else if (!GameNetwork.IsClientOrReplay)
						{
							ActionIndexCache actionIndexCache = Trebuchet.act_usage_trebuchet_reload;
							if (this.ReloadStandingPoints[j].GameEntity.HasTag("right"))
							{
								actionIndexCache = Trebuchet.act_usage_trebuchet_reload_2;
							}
							if (!userAgent3.SetActionChannel(1, actionIndexCache, false, 0UL, 0f, 1f, -0.2f, 0.4f, this.Skeletons[0].GetAnimationParameterAtChannel(0), false, -0.2f, 0, true) && userAgent3.Controller != Agent.ControllerType.AI)
							{
								userAgent3.StopUsingGameObjectMT(true, Agent.StopUsingGameObjectFlags.AutoAttachAfterStoppingUsingGameObject);
							}
						}
					}
				}
			}
		}

		// Token: 0x06002F86 RID: 12166 RVA: 0x000C2C4C File Offset: 0x000C0E4C
		protected override void SetActivationLoadAmmoPoint(bool activate)
		{
			foreach (StandingPointWithWeaponRequirement standingPointWithWeaponRequirement in this._ammoLoadPoints)
			{
				standingPointWithWeaponRequirement.SetIsDeactivatedSynched(!activate);
			}
		}

		// Token: 0x06002F87 RID: 12167 RVA: 0x000C2CA0 File Offset: 0x000C0EA0
		protected override void UpdateProjectilePosition()
		{
			MatrixFrame boneEntitialFrameWithIndex = this._sling.GameEntity.GetBoneEntitialFrameWithIndex(this._missileBoneIndex);
			base.Projectile.GameEntity.SetFrame(ref boneEntitialFrameWithIndex);
		}

		// Token: 0x06002F88 RID: 12168 RVA: 0x000C2CD6 File Offset: 0x000C0ED6
		protected internal override bool IsStandingPointNotUsedOnAccountOfBeingAmmoLoad(StandingPoint standingPoint)
		{
			return (this._ammoLoadPoints.Contains(standingPoint) && this.LoadAmmoStandingPoint != standingPoint) || base.IsStandingPointNotUsedOnAccountOfBeingAmmoLoad(standingPoint);
		}

		// Token: 0x06002F89 RID: 12169 RVA: 0x000C2CF8 File Offset: 0x000C0EF8
		protected override float GetDetachmentWeightAux(BattleSideEnum side)
		{
			return base.GetDetachmentWeightAuxForExternalAmmoWeapons(side);
		}

		// Token: 0x06002F8A RID: 12170 RVA: 0x000C2D01 File Offset: 0x000C0F01
		public void SetSpawnedFromSpawner()
		{
			this._spawnedFromSpawner = true;
		}

		// Token: 0x04001377 RID: 4983
		private static readonly ActionIndexCache act_usage_trebuchet_idle = ActionIndexCache.Create("act_usage_trebuchet_idle");

		// Token: 0x04001378 RID: 4984
		public const float TrebuchetDirectionRestriction = 1.3962635f;

		// Token: 0x04001379 RID: 4985
		private static readonly ActionIndexCache act_usage_trebuchet_reload = ActionIndexCache.Create("act_usage_trebuchet_reload");

		// Token: 0x0400137A RID: 4986
		private static readonly ActionIndexCache act_usage_trebuchet_reload_2 = ActionIndexCache.Create("act_usage_trebuchet_reload_2");

		// Token: 0x0400137B RID: 4987
		private static readonly ActionIndexCache act_usage_trebuchet_reload_idle = ActionIndexCache.Create("act_usage_trebuchet_reload_idle");

		// Token: 0x0400137C RID: 4988
		private static readonly ActionIndexCache act_usage_trebuchet_reload_2_idle = ActionIndexCache.Create("act_usage_trebuchet_reload_2_idle");

		// Token: 0x0400137D RID: 4989
		private static readonly ActionIndexCache act_usage_trebuchet_load_ammo = ActionIndexCache.Create("act_usage_trebuchet_load_ammo");

		// Token: 0x0400137E RID: 4990
		private static readonly ActionIndexCache act_usage_trebuchet_shoot = ActionIndexCache.Create("act_usage_trebuchet_shoot");

		// Token: 0x0400137F RID: 4991
		private static readonly ActionIndexCache act_strike_bent_over = ActionIndexCache.Create("act_strike_bent_over");

		// Token: 0x04001380 RID: 4992
		private static readonly ActionIndexCache act_pickup_boulder_begin = ActionIndexCache.Create("act_pickup_boulder_begin");

		// Token: 0x04001381 RID: 4993
		private static readonly ActionIndexCache act_pickup_boulder_end = ActionIndexCache.Create("act_pickup_boulder_end");

		// Token: 0x04001382 RID: 4994
		private const string BodyTag = "body";

		// Token: 0x04001383 RID: 4995
		private const string SlideTag = "slide";

		// Token: 0x04001384 RID: 4996
		private const string SlingTag = "sling";

		// Token: 0x04001385 RID: 4997
		private const string RopeTag = "rope";

		// Token: 0x04001386 RID: 4998
		private const string RotateTag = "rotate";

		// Token: 0x04001387 RID: 4999
		private const string VerticalAdjusterTag = "vertical_adjuster";

		// Token: 0x04001388 RID: 5000
		private const string MissileBoneName = "bn_projectile_holder";

		// Token: 0x04001389 RID: 5001
		private const string LeftTag = "left";

		// Token: 0x0400138A RID: 5002
		private const string _rotateObjectTag = "rotate_entity";

		// Token: 0x0400138B RID: 5003
		public float ProjectileSpeed = 45f;

		// Token: 0x0400138C RID: 5004
		public string AIAmmoLoadTag = "ammoload_ai";

		// Token: 0x0400138D RID: 5005
		private SynchedMissionObject _body;

		// Token: 0x0400138E RID: 5006
		private SynchedMissionObject _sling;

		// Token: 0x0400138F RID: 5007
		private SynchedMissionObject _rope;

		// Token: 0x04001390 RID: 5008
		public string IdleWithAmmoAnimation;

		// Token: 0x04001391 RID: 5009
		public string IdleEmptyAnimation;

		// Token: 0x04001392 RID: 5010
		public string BodyFireAnimation;

		// Token: 0x04001393 RID: 5011
		public string BodySetUpAnimation;

		// Token: 0x04001394 RID: 5012
		public string SlingFireAnimation;

		// Token: 0x04001395 RID: 5013
		public string SlingSetUpAnimation;

		// Token: 0x04001396 RID: 5014
		public string RopeFireAnimation;

		// Token: 0x04001397 RID: 5015
		public string RopeSetUpAnimation;

		// Token: 0x04001398 RID: 5016
		public string VerticalAdjusterAnimation;

		// Token: 0x04001399 RID: 5017
		public float TimeGapBetweenShootActionAndProjectileLeaving = 1.6f;

		// Token: 0x0400139A RID: 5018
		private GameEntity _verticalAdjuster;

		// Token: 0x0400139B RID: 5019
		private Skeleton _verticalAdjusterSkeleton;

		// Token: 0x0400139C RID: 5020
		private MatrixFrame _verticalAdjusterStartingLocalFrame;

		// Token: 0x0400139D RID: 5021
		private float _timeElapsedAfterLoading;

		// Token: 0x0400139E RID: 5022
		private bool _shootAnimPlayed;

		// Token: 0x0400139F RID: 5023
		private MatrixFrame[] _standingPointLocalIKFrames;

		// Token: 0x040013A0 RID: 5024
		private List<StandingPointWithWeaponRequirement> _ammoLoadPoints;

		// Token: 0x040013A1 RID: 5025
		private sbyte _missileBoneIndex;
	}
}
