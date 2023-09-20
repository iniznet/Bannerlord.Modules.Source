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
	public class Ballista : RangedSiegeWeapon, ISpawnable
	{
		private protected SynchedMissionObject ballistaBody { protected get; private set; }

		private protected SynchedMissionObject ballistaNavel { protected get; private set; }

		public override float DirectionRestriction
		{
			get
			{
				return this.HorizontalDirectionRestriction;
			}
		}

		protected override float ShootingSpeed
		{
			get
			{
				return this.BallistaShootingSpeed;
			}
		}

		public override Vec3 CanShootAtPointCheckingOffset
		{
			get
			{
				return new Vec3(0f, 0f, 0.5f, -1f);
			}
		}

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

		public override SiegeEngineType GetSiegeEngineType()
		{
			return DefaultSiegeEngineTypes.Ballista;
		}

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

		protected override bool CanRotate()
		{
			return base.State != RangedSiegeWeapon.WeaponState.Shooting;
		}

		public override UsableMachineAIBase CreateAIBehaviorObject()
		{
			return new BallistaAI(this);
		}

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

		protected override float MaximumBallisticError
		{
			get
			{
				return 0.5f;
			}
		}

		protected override float HorizontalAimSensitivity
		{
			get
			{
				return 1f;
			}
		}

		protected override float VerticalAimSensitivity
		{
			get
			{
				return 1f;
			}
		}

		protected override Vec3 VisualizationShootingDirection
		{
			get
			{
				Mat3 rotation = base.GameEntity.GetGlobalFrame().rotation;
				rotation.RotateAboutSide(-this.VisualizeReleaseTrajectoryAngle);
				return rotation.TransformToParent(new Vec3(0f, -1f, 0f, -1f));
			}
		}

		protected override void HandleUserAiming(float dt)
		{
			if (base.PilotAgent == null)
			{
				this.targetReleaseAngle = 0f;
			}
			base.HandleUserAiming(dt);
		}

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

		protected override void ApplyCurrentDirectionToEntity()
		{
			this.ApplyAimChange();
		}

		protected override void GetSoundEventIndices()
		{
			this.MoveSoundIndex = SoundEvent.GetEventIdFromString("event:/mission/siege/ballista/move");
			this.ReloadSoundIndex = SoundEvent.GetEventIdFromString("event:/mission/siege/ballista/reload");
			this.ReloadEndSoundIndex = SoundEvent.GetEventIdFromString("event:/mission/siege/ballista/reload_end");
		}

		protected internal override bool IsTargetValid(ITargetable target)
		{
			return !(target is ICastleKeyPosition);
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
			if (this._changeToState != RangedSiegeWeapon.WeaponState.Invalid)
			{
				base.State = this._changeToState;
				this._changeToState = RangedSiegeWeapon.WeaponState.Invalid;
			}
		}

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

		public override TextObject GetActionTextForStandingPoint(UsableMissionObject usableGameObject)
		{
			TextObject textObject = new TextObject("{=fEQAPJ2e}{KEY} Use", null);
			textObject.SetTextVariable("KEY", HyperlinkTexts.GetKeyHyperlinkText(HotKeyManager.GetHotKeyId("CombatHotKeyCategory", 13)));
			return textObject;
		}

		public override string GetDescriptionText(GameEntity gameEntity = null)
		{
			return new TextObject("{=abbALYlp}Ballista", null).ToString();
		}

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

		public override float GetTargetValue(List<Vec3> weaponPos)
		{
			return 30f * base.GetUserMultiplierOfWeapon() * this.GetDistanceMultiplierOfWeapon(weaponPos[0]) * base.GetHitPointMultiplierOfWeapon();
		}

		public void SetSpawnedFromSpawner()
		{
			this._spawnedFromSpawner = true;
		}

		private static readonly ActionIndexCache act_usage_ballista_ammo_pick_up_end = ActionIndexCache.Create("act_usage_ballista_ammo_pick_up_end");

		private static readonly ActionIndexCache act_usage_ballista_ammo_pick_up_start = ActionIndexCache.Create("act_usage_ballista_ammo_pick_up_start");

		private static readonly ActionIndexCache act_usage_ballista_ammo_place_end = ActionIndexCache.Create("act_usage_ballista_ammo_place_end");

		private static readonly ActionIndexCache act_usage_ballista_ammo_place_start = ActionIndexCache.Create("act_usage_ballista_ammo_place_start");

		private static readonly ActionIndexCache act_usage_ballista_idle = ActionIndexCache.Create("act_usage_ballista_idle");

		private static readonly ActionIndexCache act_usage_ballista_reload = ActionIndexCache.Create("act_usage_ballista_reload");

		private static readonly ActionIndexCache act_strike_bent_over = ActionIndexCache.Create("act_strike_bent_over");

		public string NavelTag = "BallistaNavel";

		public string BodyTag = "BallistaBody";

		public float AnimationHeightDifference;

		private MatrixFrame _ballistaBodyInitialLocalFrame;

		private MatrixFrame _ballistaNavelInitialFrame;

		private MatrixFrame _pilotInitialLocalFrame;

		private MatrixFrame _pilotInitialLocalIKFrame;

		private MatrixFrame _missileInitialLocalFrame;

		[EditableScriptComponentVariable(true)]
		protected string IdleActionName = "act_usage_ballista_idle_attacker";

		[EditableScriptComponentVariable(true)]
		protected string ReloadActionName = "act_usage_ballista_reload_attacker";

		[EditableScriptComponentVariable(true)]
		protected string PlaceAmmoStartActionName = "act_usage_ballista_ammo_place_start_attacker";

		[EditableScriptComponentVariable(true)]
		protected string PlaceAmmoEndActionName = "act_usage_ballista_ammo_place_end_attacker";

		[EditableScriptComponentVariable(true)]
		protected string PickUpAmmoStartActionName = "act_usage_ballista_ammo_pick_up_start_attacker";

		[EditableScriptComponentVariable(true)]
		protected string PickUpAmmoEndActionName = "act_usage_ballista_ammo_pick_up_end_attacker";

		private ActionIndexCache _idleAnimationActionIndex;

		private ActionIndexCache _reloadAnimationActionIndex;

		private ActionIndexCache _placeAmmoStartAnimationActionIndex;

		private ActionIndexCache _placeAmmoEndAnimationActionIndex;

		private ActionIndexCache _pickUpAmmoStartAnimationActionIndex;

		private ActionIndexCache _pickUpAmmoEndAnimationActionIndex;

		private float _verticalOffsetAngle;

		[EditableScriptComponentVariable(false)]
		public float HorizontalDirectionRestriction = 1.5707964f;

		public float BallistaShootingSpeed = 120f;

		private RangedSiegeWeapon.WeaponState _changeToState = RangedSiegeWeapon.WeaponState.Invalid;
	}
}
