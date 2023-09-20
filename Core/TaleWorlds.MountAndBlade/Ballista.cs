using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Objects.Siege;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000348 RID: 840
	public class Ballista : RangedSiegeWeapon, ISpawnable
	{
		// Token: 0x170007FC RID: 2044
		// (get) Token: 0x06002CAC RID: 11436 RVA: 0x000AD3A9 File Offset: 0x000AB5A9
		// (set) Token: 0x06002CAD RID: 11437 RVA: 0x000AD3B1 File Offset: 0x000AB5B1
		private protected SynchedMissionObject ballistaBody { protected get; private set; }

		// Token: 0x170007FD RID: 2045
		// (get) Token: 0x06002CAE RID: 11438 RVA: 0x000AD3BA File Offset: 0x000AB5BA
		// (set) Token: 0x06002CAF RID: 11439 RVA: 0x000AD3C2 File Offset: 0x000AB5C2
		private protected SynchedMissionObject ballistaNavel { protected get; private set; }

		// Token: 0x170007FE RID: 2046
		// (get) Token: 0x06002CB0 RID: 11440 RVA: 0x000AD3CB File Offset: 0x000AB5CB
		public override float DirectionRestriction
		{
			get
			{
				return this.HorizontalDirectionRestriction;
			}
		}

		// Token: 0x170007FF RID: 2047
		// (get) Token: 0x06002CB1 RID: 11441 RVA: 0x000AD3D3 File Offset: 0x000AB5D3
		protected override float ShootingSpeed
		{
			get
			{
				return this.BallistaShootingSpeed;
			}
		}

		// Token: 0x17000800 RID: 2048
		// (get) Token: 0x06002CB2 RID: 11442 RVA: 0x000AD3DB File Offset: 0x000AB5DB
		public override Vec3 CanShootAtPointCheckingOffset
		{
			get
			{
				return new Vec3(0f, 0f, 0.5f, -1f);
			}
		}

		// Token: 0x06002CB3 RID: 11443 RVA: 0x000AD3F8 File Offset: 0x000AB5F8
		protected override void RegisterAnimationParameters()
		{
			this.SkeletonOwnerObjects = new SynchedMissionObject[1];
			this.Skeletons = new Skeleton[1];
			this.SkeletonOwnerObjects[0] = this.ballistaBody;
			this.Skeletons[0] = this.ballistaBody.GameEntity.Skeleton;
			base.SkeletonName = "ballista_skeleton";
			base.FireAnimation = "ballista_fire";
			base.FireAnimationIndex = MBAnimation.GetAnimationIndexWithName("ballista_fire");
			base.SetUpAnimation = "ballista_set_up";
			base.SetUpAnimationIndex = MBAnimation.GetAnimationIndexWithName("ballista_set_up");
			this._idleAnimationActionIndex = ActionIndexCache.Create(this.IdleActionName);
			this._reloadAnimationActionIndex = ActionIndexCache.Create(this.ReloadActionName);
			this._placeAmmoStartAnimationActionIndex = ActionIndexCache.Create(this.PlaceAmmoStartActionName);
			this._placeAmmoEndAnimationActionIndex = ActionIndexCache.Create(this.PlaceAmmoEndActionName);
			this._pickUpAmmoStartAnimationActionIndex = ActionIndexCache.Create(this.PickUpAmmoStartActionName);
			this._pickUpAmmoEndAnimationActionIndex = ActionIndexCache.Create(this.PickUpAmmoEndActionName);
		}

		// Token: 0x06002CB4 RID: 11444 RVA: 0x000AD4EA File Offset: 0x000AB6EA
		public override SiegeEngineType GetSiegeEngineType()
		{
			return DefaultSiegeEngineTypes.Ballista;
		}

		// Token: 0x06002CB5 RID: 11445 RVA: 0x000AD4F4 File Offset: 0x000AB6F4
		protected internal override void OnInit()
		{
			this.ballistaBody = base.GameEntity.CollectObjectsWithTag(this.BodyTag)[0];
			this.ballistaNavel = base.GameEntity.CollectObjectsWithTag(this.NavelTag)[0];
			this.RotationObject = this;
			base.OnInit();
			this.UsesMouseForAiming = true;
			this.GetSoundEventIndices();
			this._ballistaNavelInitialFrame = this.ballistaNavel.GameEntity.GetFrame();
			MatrixFrame globalFrame = this.ballistaBody.GameEntity.GetGlobalFrame();
			this._ballistaBodyInitialLocalFrame = this.ballistaBody.GameEntity.GetFrame();
			MatrixFrame globalFrame2 = base.PilotStandingPoint.GameEntity.GetGlobalFrame();
			this._pilotInitialLocalFrame = base.PilotStandingPoint.GameEntity.GetFrame();
			this._pilotInitialLocalIKFrame = globalFrame2.TransformToLocal(globalFrame);
			this._missileInitialLocalFrame = base.Projectile.GameEntity.GetFrame();
			base.PilotStandingPoint.AddComponent(new ClearHandInverseKinematicsOnStopUsageComponent());
			this.MissileStartingPositionEntityForSimulation = base.Projectile.GameEntity.Parent.GetChildren().FirstOrDefault((GameEntity x) => x.Name == "projectile_leaving_position");
			this.EnemyRangeToStopUsing = 5f;
			this.AttackClickWillReload = true;
			this.WeaponNeedsClickToReload = true;
			base.SetScriptComponentToTick(this.GetTickRequirement());
			Vec3 shootingDirection = this.ShootingDirection;
			Vec3 vec = new Vec3(0f, shootingDirection.AsVec2.Length, shootingDirection.z, -1f);
			this._verticalOffsetAngle = Vec3.AngleBetweenTwoVectors(vec, Vec3.Forward);
			this.ApplyAimChange();
		}

		// Token: 0x06002CB6 RID: 11446 RVA: 0x000AD696 File Offset: 0x000AB896
		protected override bool CanRotate()
		{
			return base.State != RangedSiegeWeapon.WeaponState.Shooting;
		}

		// Token: 0x06002CB7 RID: 11447 RVA: 0x000AD6A4 File Offset: 0x000AB8A4
		public override UsableMachineAIBase CreateAIBehaviorObject()
		{
			return new BallistaAI(this);
		}

		// Token: 0x06002CB8 RID: 11448 RVA: 0x000AD6AC File Offset: 0x000AB8AC
		protected override void OnRangedSiegeWeaponStateChange()
		{
			base.OnRangedSiegeWeaponStateChange();
			RangedSiegeWeapon.WeaponState state = base.State;
			if (state != RangedSiegeWeapon.WeaponState.Idle)
			{
				if (state == RangedSiegeWeapon.WeaponState.WaitingBeforeProjectileLeaving)
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
			else if (base.AmmoCount > 0)
			{
				if (!GameNetwork.IsClientOrReplay)
				{
					this.ConsumeAmmo();
					return;
				}
				this.SetAmmo(base.AmmoCount - 1);
			}
		}

		// Token: 0x17000801 RID: 2049
		// (get) Token: 0x06002CB9 RID: 11449 RVA: 0x000AD719 File Offset: 0x000AB919
		protected override float MaximumBallisticError
		{
			get
			{
				return 0.5f;
			}
		}

		// Token: 0x17000802 RID: 2050
		// (get) Token: 0x06002CBA RID: 11450 RVA: 0x000AD720 File Offset: 0x000AB920
		protected override float HorizontalAimSensitivity
		{
			get
			{
				return 1f;
			}
		}

		// Token: 0x17000803 RID: 2051
		// (get) Token: 0x06002CBB RID: 11451 RVA: 0x000AD727 File Offset: 0x000AB927
		protected override float VerticalAimSensitivity
		{
			get
			{
				return 1f;
			}
		}

		// Token: 0x17000804 RID: 2052
		// (get) Token: 0x06002CBC RID: 11452 RVA: 0x000AD730 File Offset: 0x000AB930
		protected override Vec3 VisualizationShootingDirection
		{
			get
			{
				Mat3 rotation = base.GameEntity.GetGlobalFrame().rotation;
				rotation.RotateAboutSide(-this.VisualizeReleaseTrajectoryAngle);
				return rotation.TransformToParent(new Vec3(0f, -1f, 0f, -1f));
			}
		}

		// Token: 0x06002CBD RID: 11453 RVA: 0x000AD77C File Offset: 0x000AB97C
		protected override void HandleUserAiming(float dt)
		{
			if (base.PilotAgent == null)
			{
				this.targetReleaseAngle = 0f;
			}
			base.HandleUserAiming(dt);
		}

		// Token: 0x06002CBE RID: 11454 RVA: 0x000AD798 File Offset: 0x000AB998
		protected override void ApplyAimChange()
		{
			MatrixFrame ballistaNavelInitialFrame = this._ballistaNavelInitialFrame;
			ballistaNavelInitialFrame.rotation.RotateAboutUp(this.currentDirection);
			this.ballistaNavel.GameEntity.SetFrame(ref ballistaNavelInitialFrame);
			this.ballistaNavel.GameEntity.RecomputeBoundingBox();
			MatrixFrame matrixFrame = this._ballistaNavelInitialFrame.TransformToLocal(this._pilotInitialLocalFrame);
			MatrixFrame matrixFrame2 = ballistaNavelInitialFrame.TransformToParent(matrixFrame);
			base.PilotStandingPoint.GameEntity.SetFrame(ref matrixFrame2);
			base.PilotStandingPoint.GameEntity.RecomputeBoundingBox();
			MatrixFrame ballistaBodyInitialLocalFrame = this._ballistaBodyInitialLocalFrame;
			ballistaBodyInitialLocalFrame.rotation.RotateAboutSide(-this.currentReleaseAngle + this._verticalOffsetAngle);
			this.ballistaBody.GameEntity.SetFrame(ref ballistaBodyInitialLocalFrame);
			this.ballistaBody.GameEntity.RecomputeBoundingBox();
		}

		// Token: 0x06002CBF RID: 11455 RVA: 0x000AD860 File Offset: 0x000ABA60
		protected override void ApplyCurrentDirectionToEntity()
		{
			this.ApplyAimChange();
		}

		// Token: 0x06002CC0 RID: 11456 RVA: 0x000AD868 File Offset: 0x000ABA68
		protected override void GetSoundEventIndices()
		{
			this.MoveSoundIndex = SoundEvent.GetEventIdFromString("event:/mission/siege/ballista/move");
			this.ReloadSoundIndex = SoundEvent.GetEventIdFromString("event:/mission/siege/ballista/reload");
			this.ReloadEndSoundIndex = SoundEvent.GetEventIdFromString("event:/mission/siege/ballista/reload_end");
		}

		// Token: 0x06002CC1 RID: 11457 RVA: 0x000AD89A File Offset: 0x000ABA9A
		protected internal override bool IsTargetValid(ITargetable target)
		{
			return !(target is ICastleKeyPosition);
		}

		// Token: 0x06002CC2 RID: 11458 RVA: 0x000AD8A8 File Offset: 0x000ABAA8
		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			if (base.GameEntity.IsVisibleIncludeParents())
			{
				return base.GetTickRequirement() | ScriptComponentBehavior.TickRequirement.Tick | ScriptComponentBehavior.TickRequirement.TickParallel;
			}
			return base.GetTickRequirement();
		}

		// Token: 0x06002CC3 RID: 11459 RVA: 0x000AD8C8 File Offset: 0x000ABAC8
		protected internal override void OnTick(float dt)
		{
			base.OnTick(dt);
			if (this._changeToState != RangedSiegeWeapon.WeaponState.Invalid)
			{
				base.State = this._changeToState;
				this._changeToState = RangedSiegeWeapon.WeaponState.Invalid;
			}
		}

		// Token: 0x06002CC4 RID: 11460 RVA: 0x000AD8F0 File Offset: 0x000ABAF0
		protected internal override void OnTickParallel(float dt)
		{
			base.OnTickParallel(dt);
			if (!base.GameEntity.IsVisibleIncludeParents())
			{
				return;
			}
			if (base.PilotAgent != null)
			{
				Agent pilotAgent = base.PilotAgent;
				MatrixFrame matrixFrame = this.ballistaBody.GameEntity.GetGlobalFrame();
				pilotAgent.SetHandInverseKinematicsFrameForMissionObjectUsage(this._pilotInitialLocalIKFrame, matrixFrame, this.AnimationHeightDifference);
				ActionIndexValueCache currentActionValue = base.PilotAgent.GetCurrentActionValue(1);
				if (currentActionValue == this._pickUpAmmoEndAnimationActionIndex || currentActionValue == this._placeAmmoStartAnimationActionIndex)
				{
					MatrixFrame matrixFrame2 = base.PilotAgent.AgentVisuals.GetBoneEntitialFrame(base.PilotAgent.Monster.MainHandItemBoneIndex, false);
					matrixFrame = base.PilotAgent.AgentVisuals.GetGlobalFrame();
					matrixFrame2 = matrixFrame.TransformToParent(matrixFrame2);
					base.Projectile.GameEntity.SetGlobalFrame(matrixFrame2);
				}
				else
				{
					base.Projectile.GameEntity.SetFrame(ref this._missileInitialLocalFrame);
				}
			}
			if (GameNetwork.IsClientOrReplay)
			{
				return;
			}
			switch (base.State)
			{
			case RangedSiegeWeapon.WeaponState.LoadingAmmo:
			{
				bool flag = false;
				if (base.PilotAgent != null)
				{
					ActionIndexValueCache currentActionValue2 = base.PilotAgent.GetCurrentActionValue(1);
					if (currentActionValue2 != this._pickUpAmmoStartAnimationActionIndex && currentActionValue2 != this._pickUpAmmoEndAnimationActionIndex && currentActionValue2 != this._placeAmmoStartAnimationActionIndex && currentActionValue2 != this._placeAmmoEndAnimationActionIndex && !base.PilotAgent.SetActionChannel(1, this._pickUpAmmoStartAnimationActionIndex, false, 0UL, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true) && base.PilotAgent.Controller != Agent.ControllerType.AI)
					{
						base.PilotAgent.StopUsingGameObjectMT(true, Agent.StopUsingGameObjectFlags.AutoAttachAfterStoppingUsingGameObject);
					}
					else if (currentActionValue2 == this._pickUpAmmoEndAnimationActionIndex || currentActionValue2 == this._placeAmmoStartAnimationActionIndex)
					{
						flag = true;
					}
					else if (currentActionValue2 == this._placeAmmoEndAnimationActionIndex)
					{
						flag = true;
						this._changeToState = RangedSiegeWeapon.WeaponState.WaitingBeforeIdle;
					}
				}
				base.Projectile.SetVisibleSynched(flag, false);
				return;
			}
			case RangedSiegeWeapon.WeaponState.WaitingBeforeIdle:
				if (base.PilotAgent == null)
				{
					this._changeToState = RangedSiegeWeapon.WeaponState.Idle;
					return;
				}
				if (base.PilotAgent.GetCurrentActionValue(1) != this._placeAmmoEndAnimationActionIndex)
				{
					if (base.PilotAgent.Controller != Agent.ControllerType.AI)
					{
						base.PilotAgent.StopUsingGameObjectMT(true, Agent.StopUsingGameObjectFlags.AutoAttachAfterStoppingUsingGameObject);
					}
					this._changeToState = RangedSiegeWeapon.WeaponState.Idle;
					return;
				}
				if (base.PilotAgent.GetCurrentActionProgress(1) > 0.9999f)
				{
					this._changeToState = RangedSiegeWeapon.WeaponState.Idle;
					if (base.PilotAgent != null && !base.PilotAgent.SetActionChannel(1, this._idleAnimationActionIndex, false, 0UL, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true) && base.PilotAgent.Controller != Agent.ControllerType.AI)
					{
						base.PilotAgent.StopUsingGameObjectMT(true, Agent.StopUsingGameObjectFlags.AutoAttachAfterStoppingUsingGameObject);
						return;
					}
				}
				break;
			case RangedSiegeWeapon.WeaponState.Reloading:
				if (base.PilotAgent != null && !base.PilotAgent.SetActionChannel(1, this._reloadAnimationActionIndex, false, 0UL, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true) && base.PilotAgent.Controller != Agent.ControllerType.AI)
				{
					base.PilotAgent.StopUsingGameObjectMT(true, Agent.StopUsingGameObjectFlags.AutoAttachAfterStoppingUsingGameObject);
					return;
				}
				break;
			default:
				if (base.PilotAgent != null)
				{
					if (base.PilotAgent.IsInBeingStruckAction)
					{
						if (base.PilotAgent.GetCurrentActionValue(1) != Ballista.act_strike_bent_over)
						{
							base.PilotAgent.SetActionChannel(1, Ballista.act_strike_bent_over, false, 0UL, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
							return;
						}
					}
					else if (!base.PilotAgent.SetActionChannel(1, this._idleAnimationActionIndex, false, 0UL, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true) && base.PilotAgent.Controller != Agent.ControllerType.AI)
					{
						base.PilotAgent.StopUsingGameObjectMT(true, Agent.StopUsingGameObjectFlags.AutoAttachAfterStoppingUsingGameObject);
					}
				}
				break;
			}
		}

		// Token: 0x06002CC5 RID: 11461 RVA: 0x000ADCE9 File Offset: 0x000ABEE9
		public override TextObject GetActionTextForStandingPoint(UsableMissionObject usableGameObject)
		{
			TextObject textObject = new TextObject("{=fEQAPJ2e}{KEY} Use", null);
			textObject.SetTextVariable("KEY", HyperlinkTexts.GetKeyHyperlinkText(HotKeyManager.GetHotKeyId("CombatHotKeyCategory", 13)));
			return textObject;
		}

		// Token: 0x06002CC6 RID: 11462 RVA: 0x000ADD13 File Offset: 0x000ABF13
		public override string GetDescriptionText(GameEntity gameEntity = null)
		{
			return new TextObject("{=abbALYlp}Ballista", null).ToString();
		}

		// Token: 0x06002CC7 RID: 11463 RVA: 0x000ADD28 File Offset: 0x000ABF28
		protected override void UpdateAmmoMesh()
		{
			int num = 8 - base.AmmoCount;
			foreach (GameEntity gameEntity in base.GameEntity.GetChildren())
			{
				for (int i = 0; i < gameEntity.MultiMeshComponentCount; i++)
				{
					MetaMesh metaMesh = gameEntity.GetMetaMesh(i);
					for (int j = 0; j < metaMesh.MeshCount; j++)
					{
						metaMesh.GetMeshAtIndex(j).SetVectorArgument(0f, (float)num, 0f, 0f);
					}
				}
			}
		}

		// Token: 0x06002CC8 RID: 11464 RVA: 0x000ADDCC File Offset: 0x000ABFCC
		public override float ProcessTargetValue(float baseValue, TargetFlags flags)
		{
			if (flags.HasAnyFlag(TargetFlags.NotAThreat))
			{
				return -1000f;
			}
			if (flags.HasAnyFlag(TargetFlags.IsSiegeEngine))
			{
				baseValue *= 0.2f;
			}
			if (flags.HasAnyFlag(TargetFlags.IsStructure))
			{
				baseValue *= 0.05f;
			}
			if (flags.HasAnyFlag(TargetFlags.DebugThreat))
			{
				baseValue *= 10000f;
			}
			return baseValue;
		}

		// Token: 0x06002CC9 RID: 11465 RVA: 0x000ADE24 File Offset: 0x000AC024
		public override TargetFlags GetTargetFlags()
		{
			TargetFlags targetFlags = TargetFlags.None;
			targetFlags |= TargetFlags.IsFlammable;
			targetFlags |= TargetFlags.IsSiegeEngine;
			if (this.Side == BattleSideEnum.Attacker)
			{
				targetFlags |= TargetFlags.IsAttacker;
			}
			targetFlags |= TargetFlags.IsSmall;
			if (base.IsDestroyed || this.IsDeactivated)
			{
				targetFlags |= TargetFlags.NotAThreat;
			}
			if (this.Side == BattleSideEnum.Attacker && DebugSiegeBehavior.DebugDefendState == DebugSiegeBehavior.DebugStateDefender.DebugDefendersToBallistae)
			{
				targetFlags |= TargetFlags.DebugThreat;
			}
			if (this.Side == BattleSideEnum.Defender && DebugSiegeBehavior.DebugAttackState == DebugSiegeBehavior.DebugStateAttacker.DebugAttackersToBallistae)
			{
				targetFlags |= TargetFlags.DebugThreat;
			}
			return targetFlags;
		}

		// Token: 0x06002CCA RID: 11466 RVA: 0x000ADE95 File Offset: 0x000AC095
		public override float GetTargetValue(List<Vec3> weaponPos)
		{
			return 30f * base.GetUserMultiplierOfWeapon() * this.GetDistanceMultiplierOfWeapon(weaponPos[0]) * base.GetHitPointMultiplierOfWeapon();
		}

		// Token: 0x06002CCB RID: 11467 RVA: 0x000ADEB8 File Offset: 0x000AC0B8
		public void SetSpawnedFromSpawner()
		{
			this._spawnedFromSpawner = true;
		}

		// Token: 0x04001121 RID: 4385
		private static readonly ActionIndexCache act_usage_ballista_ammo_pick_up_end = ActionIndexCache.Create("act_usage_ballista_ammo_pick_up_end");

		// Token: 0x04001122 RID: 4386
		private static readonly ActionIndexCache act_usage_ballista_ammo_pick_up_start = ActionIndexCache.Create("act_usage_ballista_ammo_pick_up_start");

		// Token: 0x04001123 RID: 4387
		private static readonly ActionIndexCache act_usage_ballista_ammo_place_end = ActionIndexCache.Create("act_usage_ballista_ammo_place_end");

		// Token: 0x04001124 RID: 4388
		private static readonly ActionIndexCache act_usage_ballista_ammo_place_start = ActionIndexCache.Create("act_usage_ballista_ammo_place_start");

		// Token: 0x04001125 RID: 4389
		private static readonly ActionIndexCache act_usage_ballista_idle = ActionIndexCache.Create("act_usage_ballista_idle");

		// Token: 0x04001126 RID: 4390
		private static readonly ActionIndexCache act_usage_ballista_reload = ActionIndexCache.Create("act_usage_ballista_reload");

		// Token: 0x04001127 RID: 4391
		private static readonly ActionIndexCache act_strike_bent_over = ActionIndexCache.Create("act_strike_bent_over");

		// Token: 0x04001128 RID: 4392
		public string NavelTag = "BallistaNavel";

		// Token: 0x04001129 RID: 4393
		public string BodyTag = "BallistaBody";

		// Token: 0x0400112A RID: 4394
		public float AnimationHeightDifference;

		// Token: 0x0400112D RID: 4397
		private MatrixFrame _ballistaBodyInitialLocalFrame;

		// Token: 0x0400112E RID: 4398
		private MatrixFrame _ballistaNavelInitialFrame;

		// Token: 0x0400112F RID: 4399
		private MatrixFrame _pilotInitialLocalFrame;

		// Token: 0x04001130 RID: 4400
		private MatrixFrame _pilotInitialLocalIKFrame;

		// Token: 0x04001131 RID: 4401
		private MatrixFrame _missileInitialLocalFrame;

		// Token: 0x04001132 RID: 4402
		[EditableScriptComponentVariable(true)]
		protected string IdleActionName = "act_usage_ballista_idle_attacker";

		// Token: 0x04001133 RID: 4403
		[EditableScriptComponentVariable(true)]
		protected string ReloadActionName = "act_usage_ballista_reload_attacker";

		// Token: 0x04001134 RID: 4404
		[EditableScriptComponentVariable(true)]
		protected string PlaceAmmoStartActionName = "act_usage_ballista_ammo_place_start_attacker";

		// Token: 0x04001135 RID: 4405
		[EditableScriptComponentVariable(true)]
		protected string PlaceAmmoEndActionName = "act_usage_ballista_ammo_place_end_attacker";

		// Token: 0x04001136 RID: 4406
		[EditableScriptComponentVariable(true)]
		protected string PickUpAmmoStartActionName = "act_usage_ballista_ammo_pick_up_start_attacker";

		// Token: 0x04001137 RID: 4407
		[EditableScriptComponentVariable(true)]
		protected string PickUpAmmoEndActionName = "act_usage_ballista_ammo_pick_up_end_attacker";

		// Token: 0x04001138 RID: 4408
		private ActionIndexCache _idleAnimationActionIndex;

		// Token: 0x04001139 RID: 4409
		private ActionIndexCache _reloadAnimationActionIndex;

		// Token: 0x0400113A RID: 4410
		private ActionIndexCache _placeAmmoStartAnimationActionIndex;

		// Token: 0x0400113B RID: 4411
		private ActionIndexCache _placeAmmoEndAnimationActionIndex;

		// Token: 0x0400113C RID: 4412
		private ActionIndexCache _pickUpAmmoStartAnimationActionIndex;

		// Token: 0x0400113D RID: 4413
		private ActionIndexCache _pickUpAmmoEndAnimationActionIndex;

		// Token: 0x0400113E RID: 4414
		private float _verticalOffsetAngle;

		// Token: 0x0400113F RID: 4415
		[EditableScriptComponentVariable(false)]
		public float HorizontalDirectionRestriction = 1.5707964f;

		// Token: 0x04001140 RID: 4416
		public float BallistaShootingSpeed = 120f;

		// Token: 0x04001141 RID: 4417
		private RangedSiegeWeapon.WeaponState _changeToState = RangedSiegeWeapon.WeaponState.Invalid;
	}
}
