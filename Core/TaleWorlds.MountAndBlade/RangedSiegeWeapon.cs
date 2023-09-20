using System;
using System.Collections.Generic;
using System.Linq;
using NetworkMessages.FromClient;
using NetworkMessages.FromServer;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Network.Messages;
using TaleWorlds.MountAndBlade.Objects.Usables;

namespace TaleWorlds.MountAndBlade
{
	public abstract class RangedSiegeWeapon : SiegeWeapon
	{
		public RangedSiegeWeapon.WeaponState State
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
						GameNetwork.WriteMessage(new SetRangedSiegeWeaponState(base.Id, value));
						GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
					}
					this._state = value;
					this.OnRangedSiegeWeaponStateChange();
				}
			}
		}

		protected virtual float MaximumBallisticError
		{
			get
			{
				return 1f;
			}
		}

		protected abstract float ShootingSpeed { get; }

		public virtual Vec3 CanShootAtPointCheckingOffset
		{
			get
			{
				return Vec3.Zero;
			}
		}

		public GameEntity cameraHolder { get; private set; }

		private protected SynchedMissionObject Projectile { protected get; private set; }

		protected Vec3 MissleStartingPositionForSimulation
		{
			get
			{
				if (this.MissileStartingPositionEntityForSimulation != null)
				{
					return this.MissileStartingPositionEntityForSimulation.GlobalPosition;
				}
				SynchedMissionObject projectile = this.Projectile;
				if (projectile == null)
				{
					return Vec3.Zero;
				}
				return projectile.GameEntity.GlobalPosition;
			}
		}

		protected string SkeletonName
		{
			set
			{
				this.SkeletonNames = new string[] { value };
			}
		}

		protected string FireAnimation
		{
			set
			{
				this.FireAnimations = new string[] { value };
			}
		}

		protected string SetUpAnimation
		{
			set
			{
				this.SetUpAnimations = new string[] { value };
			}
		}

		protected int FireAnimationIndex
		{
			set
			{
				this.FireAnimationIndices = new int[] { value };
			}
		}

		protected int SetUpAnimationIndex
		{
			set
			{
				this.SetUpAnimationIndices = new int[] { value };
			}
		}

		public event RangedSiegeWeapon.OnSiegeWeaponReloadDone OnReloadDone;

		public int AmmoCount
		{
			get
			{
				return this.CurrentAmmo;
			}
			protected set
			{
				this.CurrentAmmo = value;
			}
		}

		protected virtual bool HasAmmo
		{
			get
			{
				return this._hasAmmo;
			}
			set
			{
				this._hasAmmo = value;
			}
		}

		protected virtual void ConsumeAmmo()
		{
			int ammoCount = this.AmmoCount;
			this.AmmoCount = ammoCount - 1;
			if (GameNetwork.IsServerOrRecorder)
			{
				GameNetwork.BeginBroadcastModuleEvent();
				GameNetwork.WriteMessage(new SetRangedSiegeWeaponAmmo(base.Id, this.AmmoCount));
				GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
			}
			this.UpdateAmmoMesh();
			this.CheckAmmo();
		}

		public virtual void SetAmmo(int ammoLeft)
		{
			if (this.AmmoCount != ammoLeft)
			{
				this.AmmoCount = ammoLeft;
				this.UpdateAmmoMesh();
				this.CheckAmmo();
			}
		}

		protected virtual void CheckAmmo()
		{
			if (this.AmmoCount <= 0)
			{
				this.HasAmmo = false;
				base.SetForcedUse(false);
				foreach (StandingPointWithWeaponRequirement standingPointWithWeaponRequirement in this.AmmoPickUpStandingPoints)
				{
					standingPointWithWeaponRequirement.IsDeactivated = true;
				}
			}
		}

		public virtual float DirectionRestriction
		{
			get
			{
				return 2.0943952f;
			}
		}

		public Vec3 OriginalDirection
		{
			get
			{
				return this._originalDirection;
			}
		}

		protected virtual float HorizontalAimSensitivity
		{
			get
			{
				return 0.2f;
			}
		}

		protected virtual float VerticalAimSensitivity
		{
			get
			{
				return 0.2f;
			}
		}

		protected abstract void RegisterAnimationParameters();

		protected abstract void GetSoundEventIndices();

		public event Action<RangedSiegeWeapon, Agent> OnAgentLoadsMachine;

		protected void ChangeProjectileEntityServer(Agent loadingAgent, string missileItemID)
		{
			List<SynchedMissionObject> list = base.GameEntity.CollectObjectsWithTag("projectile");
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].GameEntity.HasTag(missileItemID))
				{
					this.Projectile = list[i];
					this._projectileIndex = i;
					break;
				}
			}
			this.LoadedMissileItem = Game.Current.ObjectManager.GetObject<ItemObject>(missileItemID);
			if (GameNetwork.IsServerOrRecorder)
			{
				GameNetwork.BeginBroadcastModuleEvent();
				GameNetwork.WriteMessage(new RangedSiegeWeaponChangeProjectile(base.Id, this._projectileIndex));
				GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
			}
			Action<RangedSiegeWeapon, Agent> onAgentLoadsMachine = this.OnAgentLoadsMachine;
			if (onAgentLoadsMachine == null)
			{
				return;
			}
			onAgentLoadsMachine(this, loadingAgent);
		}

		public void ChangeProjectileEntityClient(int index)
		{
			List<SynchedMissionObject> list = base.GameEntity.CollectObjectsWithTag("projectile");
			this.Projectile = list[index];
			this._projectileIndex = index;
		}

		protected internal override void OnInit()
		{
			base.OnInit();
			DestructableComponent destructableComponent = base.GameEntity.GetScriptComponents<DestructableComponent>().FirstOrDefault<DestructableComponent>();
			if (destructableComponent != null)
			{
				this._defaultSide = destructableComponent.BattleSide;
			}
			else
			{
				Debug.FailedAssert("Ranged siege weapons must have destructible component.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Objects\\Siege\\RangedSiegeWeapon.cs", "OnInit", 413);
			}
			this.ReleaseAngleRestrictionCenter = (this.TopReleaseAngleRestriction + this.BottomReleaseAngleRestriction) * 0.5f;
			this.ReleaseAngleRestrictionAngle = this.TopReleaseAngleRestriction - this.BottomReleaseAngleRestriction;
			this.currentReleaseAngle = (this._lastSyncedReleaseAngle = this.ReleaseAngleRestrictionCenter);
			this.OriginalMissileItem = Game.Current.ObjectManager.GetObject<ItemObject>(this.MissileItemID);
			this.LoadedMissileItem = this.OriginalMissileItem;
			this._originalMissileWeaponStatsDataForTargeting = new MissionWeapon(this.OriginalMissileItem, null, null).GetWeaponStatsDataForUsage(0);
			if (this.RotationObject == null)
			{
				this.RotationObject = this;
			}
			this._rotationObjectInitialFrame = this.RotationObject.GameEntity.GetFrame();
			this._originalDirection = this.RotationObject.GameEntity.GetGlobalFrame().rotation.f;
			this._originalDirection.RotateAboutZ(3.1415927f);
			this.currentDirection = (this._lastSyncedDirection = 0f);
			this._syncTimer = 0f;
			List<GameEntity> list = base.GameEntity.CollectChildrenEntitiesWithTag("cameraHolder");
			if (list.Count > 0)
			{
				this.cameraHolder = list[0];
				this.cameraHolderInitialFrame = this.cameraHolder.GetFrame();
				if (GameNetwork.IsClientOrReplay)
				{
					this.MakeVisibilityCheck = false;
				}
			}
			List<SynchedMissionObject> list2 = base.GameEntity.CollectObjectsWithTag("projectile");
			foreach (SynchedMissionObject synchedMissionObject in list2)
			{
				synchedMissionObject.GameEntity.SetVisibilityExcludeParents(false);
			}
			this.Projectile = list2.FirstOrDefault((SynchedMissionObject x) => x.GameEntity.HasTag(this.MissileItemID));
			this.Projectile.GameEntity.SetVisibilityExcludeParents(true);
			GameEntity gameEntity = base.GameEntity.GetChildren().FirstOrDefault((GameEntity x) => x.Name == "clean");
			GameEntity gameEntity2;
			if (gameEntity == null)
			{
				gameEntity2 = null;
			}
			else
			{
				gameEntity2 = gameEntity.GetChildren().FirstOrDefault((GameEntity x) => x.Name == "projectile_leaving_position");
			}
			this.MissileStartingPositionEntityForSimulation = gameEntity2;
			this.targetDirection = this.currentDirection;
			this.targetReleaseAngle = this.currentReleaseAngle;
			this.CanPickUpAmmoStandingPoints = new List<StandingPoint>();
			this.ReloadStandingPoints = new List<StandingPoint>();
			this.AmmoPickUpStandingPoints = new List<StandingPointWithWeaponRequirement>();
			if (base.StandingPoints != null)
			{
				foreach (StandingPoint standingPoint in base.StandingPoints)
				{
					standingPoint.AddComponent(new ResetAnimationOnStopUsageComponent(ActionIndexCache.act_none));
					if (standingPoint.GameEntity.HasTag("reload"))
					{
						this.ReloadStandingPoints.Add(standingPoint);
					}
					if (standingPoint.GameEntity.HasTag("can_pick_up_ammo"))
					{
						this.CanPickUpAmmoStandingPoints.Add(standingPoint);
					}
				}
			}
			List<StandingPointWithWeaponRequirement> list3 = base.StandingPoints.OfType<StandingPointWithWeaponRequirement>().ToList<StandingPointWithWeaponRequirement>();
			List<StandingPointWithWeaponRequirement> list4 = new List<StandingPointWithWeaponRequirement>();
			foreach (StandingPointWithWeaponRequirement standingPointWithWeaponRequirement in list3)
			{
				if (standingPointWithWeaponRequirement.GameEntity.HasTag(this.AmmoPickUpTag))
				{
					this.AmmoPickUpStandingPoints.Add(standingPointWithWeaponRequirement);
					standingPointWithWeaponRequirement.InitGivenWeapon(this.OriginalMissileItem);
					standingPointWithWeaponRequirement.SetupOnUsingStoppedBehavior(false, new Action<Agent, bool>(this.OnAmmoPickupUsingCancelled));
				}
				else
				{
					list4.Add(standingPointWithWeaponRequirement);
					standingPointWithWeaponRequirement.SetupOnUsingStoppedBehavior(false, new Action<Agent, bool>(this.OnLoadingAmmoPointUsingCancelled));
					standingPointWithWeaponRequirement.InitRequiredWeaponClasses(this.OriginalMissileItem.PrimaryWeapon.WeaponClass, WeaponClass.Undefined);
				}
			}
			if (this.AmmoPickUpStandingPoints.Count > 1)
			{
				this._stonePile = this.AmmoPickUpStandingPoints[0].GameEntity.Parent.GetFirstScriptOfType<SiegeMachineStonePile>();
				this._ammoPickupCenter = default(Vec3);
				foreach (StandingPointWithWeaponRequirement standingPointWithWeaponRequirement2 in this.AmmoPickUpStandingPoints)
				{
					standingPointWithWeaponRequirement2.SetHasAlternative(true);
					this._ammoPickupCenter += standingPointWithWeaponRequirement2.GameEntity.GlobalPosition;
				}
				this._ammoPickupCenter /= (float)this.AmmoPickUpStandingPoints.Count;
			}
			else
			{
				this._ammoPickupCenter = base.GameEntity.GlobalPosition;
			}
			list4.Sort(delegate(StandingPointWithWeaponRequirement element1, StandingPointWithWeaponRequirement element2)
			{
				if (element1.GameEntity.GlobalPosition.DistanceSquared(this._ammoPickupCenter) > element2.GameEntity.GlobalPosition.DistanceSquared(this._ammoPickupCenter))
				{
					return 1;
				}
				if (element1.GameEntity.GlobalPosition.DistanceSquared(this._ammoPickupCenter) < element2.GameEntity.GlobalPosition.DistanceSquared(this._ammoPickupCenter))
				{
					return -1;
				}
				return 0;
			});
			this.LoadAmmoStandingPoint = list4.FirstOrDefault<StandingPointWithWeaponRequirement>();
			this.SortCanPickUpAmmoStandingPoints();
			Vec3 vec = base.PilotStandingPoint.GameEntity.GlobalPosition - base.GameEntity.GlobalPosition;
			foreach (StandingPoint standingPoint2 in this.CanPickUpAmmoStandingPoints)
			{
				if (standingPoint2 != base.PilotStandingPoint)
				{
					float length = (standingPoint2.GameEntity.GlobalPosition - base.GameEntity.GlobalPosition + vec).Length;
					this.PilotReservePriorityValues.Add(standingPoint2, length);
				}
			}
			this.AmmoCount = this.startingAmmoCount;
			this.UpdateAmmoMesh();
			this.RegisterAnimationParameters();
			this.GetSoundEventIndices();
			this.InitAnimations();
			base.SetScriptComponentToTick(this.GetTickRequirement());
		}

		private void SortCanPickUpAmmoStandingPoints()
		{
			if (MBMath.GetSmallestDifferenceBetweenTwoAngles(this._lastCanPickUpAmmoStandingPointsSortedAngle, this.currentDirection) > 0.18849556f)
			{
				this._lastCanPickUpAmmoStandingPointsSortedAngle = this.currentDirection;
				int signOfAmmoPile = Math.Sign(Vec3.DotProduct(base.GameEntity.GetGlobalFrame().rotation.s, this._ammoPickupCenter - base.GameEntity.GlobalPosition));
				this.CanPickUpAmmoStandingPoints.Sort(delegate(StandingPoint element1, StandingPoint element2)
				{
					Vec3 vec = this._ammoPickupCenter - element1.GameEntity.GlobalPosition;
					Vec3 vec2 = this._ammoPickupCenter - element2.GameEntity.GlobalPosition;
					float num = vec.LengthSquared;
					float num2 = vec2.LengthSquared;
					float num3 = Vec3.DotProduct(this.GameEntity.GetGlobalFrame().rotation.s, element1.GameEntity.GlobalPosition - this.GameEntity.GlobalPosition);
					float num4 = Vec3.DotProduct(this.GameEntity.GetGlobalFrame().rotation.s, element2.GameEntity.GlobalPosition - this.GameEntity.GlobalPosition);
					if (!element1.GameEntity.HasTag("no_ammo_pick_up_penalty") && signOfAmmoPile != Math.Sign(num3))
					{
						num += num3 * num3 * 64f;
					}
					if (!element2.GameEntity.HasTag("no_ammo_pick_up_penalty") && signOfAmmoPile != Math.Sign(num4))
					{
						num2 += num4 * num4 * 64f;
					}
					if (element1.GameEntity.HasTag(this.PilotStandingPointTag))
					{
						num += 25f;
					}
					else if (element2.GameEntity.HasTag(this.PilotStandingPointTag))
					{
						num2 += 25f;
					}
					if (num > num2)
					{
						return 1;
					}
					if (num < num2)
					{
						return -1;
					}
					return 0;
				});
			}
		}

		protected internal override void OnEditorInit()
		{
			List<SynchedMissionObject> list = base.GameEntity.CollectObjectsWithTag("projectile");
			if (list.Count > 0)
			{
				this.Projectile = list[0];
			}
		}

		private void InitAnimations()
		{
			for (int i = 0; i < this.Skeletons.Length; i++)
			{
				this.Skeletons[i].SetAnimationAtChannel(this.SetUpAnimations[i], 0, 1f, 0f, 0f);
				this.Skeletons[i].SetAnimationParameterAtChannel(0, 1f);
				this.Skeletons[i].TickAnimations(0.0001f, MatrixFrame.Identity, true);
			}
		}

		protected internal override void OnMissionReset()
		{
			base.OnMissionReset();
			this.Projectile.GameEntity.SetVisibilityExcludeParents(true);
			foreach (StandingPoint standingPoint in base.StandingPoints)
			{
				Agent userAgent = standingPoint.UserAgent;
				if (userAgent != null)
				{
					userAgent.StopUsingGameObject(true, Agent.StopUsingGameObjectFlags.AutoAttachAfterStoppingUsingGameObject);
				}
				standingPoint.IsDeactivated = false;
			}
			this._state = RangedSiegeWeapon.WeaponState.Idle;
			this.currentDirection = (this._lastSyncedDirection = 0f);
			this._syncTimer = 0f;
			this.currentReleaseAngle = (this._lastSyncedReleaseAngle = this.ReleaseAngleRestrictionCenter);
			this.targetDirection = this.currentDirection;
			this.targetReleaseAngle = this.currentReleaseAngle;
			this.ApplyCurrentDirectionToEntity();
			this.AmmoCount = this.startingAmmoCount;
			this.UpdateAmmoMesh();
			if (this.MoveSound != null)
			{
				this.MoveSound.Stop();
				this.MoveSound = null;
			}
			this.hasFrameChangedInPreviousFrame = false;
			Skeleton[] skeletons = this.Skeletons;
			for (int i = 0; i < skeletons.Length; i++)
			{
				skeletons[i].Freeze(false);
			}
			foreach (StandingPointWithWeaponRequirement standingPointWithWeaponRequirement in this.AmmoPickUpStandingPoints)
			{
				standingPointWithWeaponRequirement.IsDeactivated = false;
			}
			this.InitAnimations();
			this.UpdateProjectilePosition();
			if (!GameNetwork.IsClientOrReplay)
			{
				this.SetActivationLoadAmmoPoint(false);
			}
		}

		public override void WriteToNetwork()
		{
			base.WriteToNetwork();
			GameNetworkMessage.WriteIntToPacket((int)this.State, CompressionMission.RangedSiegeWeaponStateCompressionInfo);
			GameNetworkMessage.WriteFloatToPacket(this.targetDirection, CompressionBasic.RadianCompressionInfo);
			GameNetworkMessage.WriteFloatToPacket(this.targetReleaseAngle, CompressionBasic.RadianCompressionInfo);
			GameNetworkMessage.WriteIntToPacket(this.AmmoCount, CompressionMission.RangedSiegeWeaponAmmoCompressionInfo);
			GameNetworkMessage.WriteIntToPacket(this._projectileIndex, CompressionMission.RangedSiegeWeaponAmmoIndexCompressionInfo);
		}

		protected virtual void UpdateProjectilePosition()
		{
		}

		public override bool IsInRangeToCheckAlternativePoints(Agent agent)
		{
			float num = ((this.AmmoPickUpStandingPoints.Count > 0) ? (agent.GetInteractionDistanceToUsable(this.AmmoPickUpStandingPoints[0]) + 2f) : 2f);
			return this._ammoPickupCenter.DistanceSquared(agent.Position) < num * num;
		}

		public override StandingPoint GetBestPointAlternativeTo(StandingPoint standingPoint, Agent agent)
		{
			if (this.AmmoPickUpStandingPoints.Contains(standingPoint))
			{
				IEnumerable<StandingPointWithWeaponRequirement> enumerable = this.AmmoPickUpStandingPoints.Where((StandingPointWithWeaponRequirement sp) => !sp.IsDeactivated && (sp.IsInstantUse || (!sp.HasUser && !sp.HasAIMovingTo)) && !sp.IsDisabledForAgent(agent));
				float num = standingPoint.GameEntity.GlobalPosition.DistanceSquared(agent.Position);
				StandingPoint standingPoint2 = standingPoint;
				foreach (StandingPointWithWeaponRequirement standingPointWithWeaponRequirement in enumerable)
				{
					float num2 = standingPointWithWeaponRequirement.GameEntity.GlobalPosition.DistanceSquared(agent.Position);
					if (num2 < num)
					{
						num = num2;
						standingPoint2 = standingPointWithWeaponRequirement;
					}
				}
				return standingPoint2;
			}
			return standingPoint;
		}

		protected virtual void OnRangedSiegeWeaponStateChange()
		{
			switch (this.State)
			{
			case RangedSiegeWeapon.WeaponState.Idle:
			case RangedSiegeWeapon.WeaponState.WaitingBeforeIdle:
				if (this.cameraState == RangedSiegeWeapon.CameraState.FreeMove)
				{
					this.cameraState = RangedSiegeWeapon.CameraState.ApproachToCamera;
				}
				else
				{
					this.cameraState = RangedSiegeWeapon.CameraState.StickToWeapon;
				}
				break;
			case RangedSiegeWeapon.WeaponState.WaitingBeforeProjectileLeaving:
				this.AttackClickWillReload = this.WeaponNeedsClickToReload;
				break;
			case RangedSiegeWeapon.WeaponState.Shooting:
				if (this.cameraHolder != null)
				{
					this.cameraState = RangedSiegeWeapon.CameraState.DontMove;
					this.dontMoveTimer = 0.35f;
				}
				break;
			case RangedSiegeWeapon.WeaponState.WaitingAfterShooting:
				this.AttackClickWillReload = this.WeaponNeedsClickToReload;
				this.CheckAmmo();
				break;
			case RangedSiegeWeapon.WeaponState.WaitingBeforeReloading:
				this.AttackClickWillReload = false;
				if (this.cameraHolder != null)
				{
					this.cameraState = RangedSiegeWeapon.CameraState.MoveDownToReload;
				}
				this.CheckAmmo();
				break;
			case RangedSiegeWeapon.WeaponState.LoadingAmmo:
				if (this.ReloadSound != null && this.ReloadSound.IsValid)
				{
					this.ReloadSound.Stop();
				}
				this.ReloadSound = null;
				Mission.Current.MakeSound(this.ReloadEndSoundIndex, base.GameEntity.GetGlobalFrame().origin, true, false, -1, -1);
				break;
			case RangedSiegeWeapon.WeaponState.Reloading:
				if (this.ReloadSound != null && this.ReloadSound.IsValid)
				{
					if (this.ReloadSound.IsPaused())
					{
						this.ReloadSound.Resume();
					}
					else
					{
						this.ReloadSound.PlayInPosition(base.GameEntity.GetGlobalFrame().origin);
					}
				}
				else
				{
					this.ReloadSound = SoundEvent.CreateEvent(this.ReloadSoundIndex, base.Scene);
					this.ReloadSound.PlayInPosition(base.GameEntity.GetGlobalFrame().origin);
				}
				break;
			case RangedSiegeWeapon.WeaponState.ReloadingPaused:
				if (this.ReloadSound != null && this.ReloadSound.IsValid)
				{
					this.ReloadSound.Pause();
				}
				break;
			default:
				Debug.FailedAssert("Invalid WeaponState.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Objects\\Siege\\RangedSiegeWeapon.cs", "OnRangedSiegeWeaponStateChange", 854);
				break;
			}
			if (!GameNetwork.IsClientOrReplay)
			{
				switch (this.State)
				{
				case RangedSiegeWeapon.WeaponState.Idle:
				case RangedSiegeWeapon.WeaponState.WaitingAfterShooting:
				case RangedSiegeWeapon.WeaponState.WaitingBeforeReloading:
					break;
				case RangedSiegeWeapon.WeaponState.WaitingBeforeProjectileLeaving:
				{
					for (int i = 0; i < this.SkeletonOwnerObjects.Length; i++)
					{
						this.SkeletonOwnerObjects[i].SetAnimationAtChannelSynched(this.FireAnimations[i], 0, 1f);
					}
					return;
				}
				case RangedSiegeWeapon.WeaponState.Shooting:
					this.ShootProjectile();
					return;
				case RangedSiegeWeapon.WeaponState.LoadingAmmo:
					this.SetActivationLoadAmmoPoint(true);
					this.ReloaderAgent = null;
					return;
				case RangedSiegeWeapon.WeaponState.WaitingBeforeIdle:
					this.SendReloaderAgentToOriginalPoint();
					this.SetActivationLoadAmmoPoint(false);
					return;
				case RangedSiegeWeapon.WeaponState.Reloading:
				{
					for (int j = 0; j < this.SkeletonOwnerObjects.Length; j++)
					{
						if (this.SkeletonOwnerObjects[j].GameEntity.IsSkeletonAnimationPaused())
						{
							this.SkeletonOwnerObjects[j].ResumeSkeletonAnimationSynched();
						}
						else
						{
							this.SkeletonOwnerObjects[j].SetAnimationAtChannelSynched(this.SetUpAnimations[j], 0, 1f);
						}
					}
					this._currentReloaderCount = 1;
					return;
				}
				case RangedSiegeWeapon.WeaponState.ReloadingPaused:
				{
					SynchedMissionObject[] skeletonOwnerObjects = this.SkeletonOwnerObjects;
					for (int k = 0; k < skeletonOwnerObjects.Length; k++)
					{
						skeletonOwnerObjects[k].PauseSkeletonAnimationSynched();
					}
					return;
				}
				default:
					Debug.FailedAssert("Invalid WeaponState.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Objects\\Siege\\RangedSiegeWeapon.cs", "OnRangedSiegeWeaponStateChange", 927);
					break;
				}
			}
		}

		protected virtual void SetActivationLoadAmmoPoint(bool activate)
		{
		}

		protected float GetDetachmentWeightAuxForExternalAmmoWeapons(BattleSideEnum side)
		{
			if (this.IsDisabledForBattleSideAI(side))
			{
				return float.MinValue;
			}
			this._usableStandingPoints.Clear();
			bool flag = false;
			bool flag2 = false;
			bool flag3 = !base.PilotStandingPoint.HasUser && !base.PilotStandingPoint.HasAIMovingTo && (this.ReloaderAgent == null || this.ReloaderAgentOriginalPoint != base.PilotStandingPoint);
			int num = -1;
			StandingPoint standingPoint = null;
			bool flag4 = false;
			for (int i = 0; i < base.StandingPoints.Count; i++)
			{
				StandingPoint standingPoint2 = base.StandingPoints[i];
				if (standingPoint2.GameEntity.HasTag("can_pick_up_ammo"))
				{
					if (this.ReloaderAgent == null || standingPoint2 != this.ReloaderAgentOriginalPoint)
					{
						if (standingPoint2.IsUsableBySide(side))
						{
							if (!standingPoint2.HasAIMovingTo)
							{
								if (!flag2)
								{
									this._usableStandingPoints.Clear();
									if (num != -1)
									{
										num = -1;
									}
								}
								flag2 = true;
							}
							else if (flag2 || standingPoint2.MovingAgent.Formation.Team.Side != side)
							{
								goto IL_16A;
							}
							flag = true;
							this._usableStandingPoints.Add(new ValueTuple<int, StandingPoint>(i, standingPoint2));
							if (flag3 && base.PilotStandingPoint == standingPoint2)
							{
								num = this._usableStandingPoints.Count - 1;
							}
						}
						else if (flag3 && standingPoint2.HasAIUser && (standingPoint == null || this.PilotReservePriorityValues[standingPoint2] > this.PilotReservePriorityValues[standingPoint] || flag4))
						{
							standingPoint = standingPoint2;
							flag4 = false;
						}
					}
					else if (flag3 && standingPoint == null)
					{
						standingPoint = standingPoint2;
						flag4 = true;
					}
				}
				IL_16A:;
			}
			if (standingPoint != null)
			{
				if (flag4)
				{
					this.ReloaderAgentOriginalPoint = base.PilotStandingPoint;
				}
				else
				{
					Agent userAgent = standingPoint.UserAgent;
					userAgent.StopUsingGameObject(true, Agent.StopUsingGameObjectFlags.DoNotWieldWeaponAfterStoppingUsingGameObject);
					userAgent.AIMoveToGameObjectEnable(base.PilotStandingPoint, this, base.Ai.GetScriptedFrameFlags(userAgent));
				}
				if (num != -1)
				{
					this._usableStandingPoints.RemoveAt(num);
				}
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

		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			if (base.GameEntity.IsVisibleIncludeParents())
			{
				return ScriptComponentBehavior.TickRequirement.Tick | base.GetTickRequirement();
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
				this.UpdateState(dt);
				if (base.PilotAgent != null && !base.PilotAgent.IsInBeingStruckAction)
				{
					if (base.PilotAgent.MovementFlags.HasAnyFlag(Agent.MovementControlFlag.AttackMask))
					{
						if (this.State == RangedSiegeWeapon.WeaponState.Idle)
						{
							this._aiRequestsShoot = false;
							this.Shoot();
						}
						else if (this.State == RangedSiegeWeapon.WeaponState.WaitingAfterShooting && this.AttackClickWillReload)
						{
							this._aiRequestsManualReload = false;
							this.ManualReload();
						}
					}
					if (this._aiRequestsManualReload)
					{
						this.ManualReload();
					}
					if (this._aiRequestsShoot)
					{
						this.Shoot();
					}
				}
				this._aiRequestsShoot = false;
				this._aiRequestsManualReload = false;
			}
			this.HandleUserAiming(dt);
		}

		protected virtual float CalculateShootingRange(float heightDifference)
		{
			return Mission.GetMissileRange(this.ShootingSpeed, heightDifference);
		}

		protected static bool ApproachToAngle(ref float angle, float angleToApproach, bool isMouse, float speed_limit, float dt, float sensitivity)
		{
			speed_limit = MathF.Abs(speed_limit);
			if (angle != angleToApproach)
			{
				float num = sensitivity * dt;
				float num2 = MathF.Abs(angle - angleToApproach);
				if (isMouse)
				{
					num *= MathF.Max(num2 * 8f, 0.15f);
				}
				if (speed_limit > 0f)
				{
					num = MathF.Min(num, speed_limit * dt);
				}
				if (num2 <= num)
				{
					angle = angleToApproach;
				}
				else
				{
					angle += num * (float)MathF.Sign(angleToApproach - angle);
				}
				return true;
			}
			return false;
		}

		protected virtual void HandleUserAiming(float dt)
		{
			bool flag = false;
			float horizontalAimSensitivity = this.HorizontalAimSensitivity;
			float verticalAimSensitivity = this.VerticalAimSensitivity;
			bool flag2 = false;
			if (this.cameraState != RangedSiegeWeapon.CameraState.DontMove)
			{
				if (this._inputGiven)
				{
					flag2 = true;
					if (this.CanRotate())
					{
						if (this._inputX != 0f)
						{
							this.targetDirection += horizontalAimSensitivity * dt * this._inputX;
							this.targetDirection = MBMath.WrapAngle(this.targetDirection);
							this.targetDirection = MBMath.ClampAngle(this.targetDirection, this.currentDirection, 0.7f);
							this.targetDirection = MBMath.ClampAngle(this.targetDirection, 0f, this.DirectionRestriction);
						}
						if (this._inputY != 0f)
						{
							this.targetReleaseAngle += verticalAimSensitivity * dt * this._inputY;
							this.targetReleaseAngle = MBMath.ClampAngle(this.targetReleaseAngle, this.currentReleaseAngle + 0.049999997f, 0.6f);
							this.targetReleaseAngle = MBMath.ClampAngle(this.targetReleaseAngle, this.ReleaseAngleRestrictionCenter, this.ReleaseAngleRestrictionAngle);
						}
					}
					this._inputGiven = false;
					this._inputX = 0f;
					this._inputY = 0f;
				}
				else if (this._exactInputGiven)
				{
					bool flag3 = false;
					if (this.CanRotate())
					{
						if (this.targetDirection != this._inputTargetX)
						{
							float num = horizontalAimSensitivity * dt;
							if (MathF.Abs(this.targetDirection - this._inputTargetX) < num)
							{
								this.targetDirection = this._inputTargetX;
							}
							else if (this.targetDirection < this._inputTargetX)
							{
								this.targetDirection += num;
								flag3 = true;
							}
							else
							{
								this.targetDirection -= num;
								flag3 = true;
							}
							this.targetDirection = MBMath.WrapAngle(this.targetDirection);
							this.targetDirection = MBMath.ClampAngle(this.targetDirection, this.currentDirection, 0.7f);
							this.targetDirection = MBMath.ClampAngle(this.targetDirection, 0f, this.DirectionRestriction);
						}
						if (this.targetReleaseAngle != this._inputTargetY)
						{
							float num2 = verticalAimSensitivity * dt;
							if (MathF.Abs(this.targetReleaseAngle - this._inputTargetY) < num2)
							{
								this.targetReleaseAngle = this._inputTargetY;
							}
							else if (this.targetReleaseAngle < this._inputTargetY)
							{
								this.targetReleaseAngle += num2;
								flag3 = true;
							}
							else
							{
								this.targetReleaseAngle -= num2;
								flag3 = true;
							}
							this.targetReleaseAngle = MBMath.ClampAngle(this.targetReleaseAngle, this.currentReleaseAngle + 0.049999997f, 0.6f);
							this.targetReleaseAngle = MBMath.ClampAngle(this.targetReleaseAngle, this.ReleaseAngleRestrictionCenter, this.ReleaseAngleRestrictionAngle);
						}
					}
					else
					{
						flag3 = true;
					}
					if (!flag3)
					{
						this._exactInputGiven = false;
					}
				}
			}
			switch (this.cameraState)
			{
			case RangedSiegeWeapon.CameraState.StickToWeapon:
				flag = RangedSiegeWeapon.ApproachToAngle(ref this.currentDirection, this.targetDirection, this.UsesMouseForAiming, -1f, dt, horizontalAimSensitivity) || flag;
				flag = RangedSiegeWeapon.ApproachToAngle(ref this.currentReleaseAngle, this.targetReleaseAngle, this.UsesMouseForAiming, -1f, dt, verticalAimSensitivity) || flag;
				this.cameraDirection = this.currentDirection;
				this.cameraReleaseAngle = this.currentReleaseAngle;
				break;
			case RangedSiegeWeapon.CameraState.DontMove:
				this.dontMoveTimer -= dt;
				if (this.dontMoveTimer < 0f)
				{
					if (!this.AttackClickWillReload)
					{
						this.cameraState = RangedSiegeWeapon.CameraState.MoveDownToReload;
						this.maxRotateSpeed = 0f;
						this.reloadTargetReleaseAngle = MBMath.ClampAngle((MathF.Abs(this.currentReleaseAngle) > 0.17453292f) ? 0f : this.currentReleaseAngle, this.currentReleaseAngle - 0.049999997f, 0.6f);
						this.targetDirection = this.cameraDirection;
						this.cameraReleaseAngle = this.targetReleaseAngle;
					}
					else
					{
						this.cameraState = RangedSiegeWeapon.CameraState.StickToWeapon;
					}
				}
				break;
			case RangedSiegeWeapon.CameraState.MoveDownToReload:
				this.maxRotateSpeed += dt * 1.2f;
				this.maxRotateSpeed = MathF.Min(this.maxRotateSpeed, 1f);
				flag = RangedSiegeWeapon.ApproachToAngle(ref this.currentReleaseAngle, this.reloadTargetReleaseAngle, this.UsesMouseForAiming, 0.4f + this.maxRotateSpeed, dt, verticalAimSensitivity) || flag;
				flag = RangedSiegeWeapon.ApproachToAngle(ref this.cameraDirection, this.targetDirection, this.UsesMouseForAiming, -1f, dt, horizontalAimSensitivity) || flag;
				flag = RangedSiegeWeapon.ApproachToAngle(ref this.cameraReleaseAngle, this.reloadTargetReleaseAngle, this.UsesMouseForAiming, 0.5f + this.maxRotateSpeed, dt, verticalAimSensitivity) || flag;
				if (!flag)
				{
					this.cameraState = RangedSiegeWeapon.CameraState.RememberLastShotDirection;
				}
				break;
			case RangedSiegeWeapon.CameraState.RememberLastShotDirection:
				if (this.State == RangedSiegeWeapon.WeaponState.Idle || flag2)
				{
					this.cameraState = RangedSiegeWeapon.CameraState.FreeMove;
					RangedSiegeWeapon.OnSiegeWeaponReloadDone onReloadDone = this.OnReloadDone;
					if (onReloadDone != null)
					{
						onReloadDone();
					}
				}
				break;
			case RangedSiegeWeapon.CameraState.FreeMove:
				flag = RangedSiegeWeapon.ApproachToAngle(ref this.cameraDirection, this.targetDirection, this.UsesMouseForAiming, -1f, dt, horizontalAimSensitivity) || flag;
				flag = RangedSiegeWeapon.ApproachToAngle(ref this.cameraReleaseAngle, this.targetReleaseAngle, this.UsesMouseForAiming, -1f, dt, verticalAimSensitivity) || flag;
				this.maxRotateSpeed = 0f;
				break;
			case RangedSiegeWeapon.CameraState.ApproachToCamera:
				this.maxRotateSpeed += 0.9f * dt + this.maxRotateSpeed * 2f * dt;
				flag = RangedSiegeWeapon.ApproachToAngle(ref this.cameraDirection, this.targetDirection, this.UsesMouseForAiming, -1f, dt, horizontalAimSensitivity) || flag;
				flag = RangedSiegeWeapon.ApproachToAngle(ref this.cameraReleaseAngle, this.targetReleaseAngle, this.UsesMouseForAiming, -1f, dt, verticalAimSensitivity) || flag;
				flag = RangedSiegeWeapon.ApproachToAngle(ref this.currentDirection, this.targetDirection, this.UsesMouseForAiming, this.maxRotateSpeed, dt, horizontalAimSensitivity) || flag;
				flag = RangedSiegeWeapon.ApproachToAngle(ref this.currentReleaseAngle, this.targetReleaseAngle, this.UsesMouseForAiming, this.maxRotateSpeed, dt, verticalAimSensitivity) || flag;
				if (!flag)
				{
					this.cameraState = RangedSiegeWeapon.CameraState.StickToWeapon;
				}
				break;
			}
			if (this.cameraHolder != null)
			{
				MatrixFrame globalFrame = this.cameraHolderInitialFrame;
				globalFrame.rotation.RotateAboutForward(this.cameraDirection - this.currentDirection);
				globalFrame.rotation.RotateAboutSide(this.cameraReleaseAngle - this.currentReleaseAngle);
				this.cameraHolder.SetFrame(ref globalFrame);
				globalFrame = this.cameraHolder.GetGlobalFrame();
				globalFrame.rotation.s.z = 0f;
				globalFrame.rotation.s.Normalize();
				globalFrame.rotation.u = Vec3.CrossProduct(globalFrame.rotation.s, globalFrame.rotation.f);
				globalFrame.rotation.u.Normalize();
				globalFrame.rotation.f = Vec3.CrossProduct(globalFrame.rotation.u, globalFrame.rotation.s);
				globalFrame.rotation.f.Normalize();
				this.cameraHolder.SetGlobalFrame(globalFrame);
			}
			if (flag && !this.hasFrameChangedInPreviousFrame)
			{
				this.OnRotationStarted();
			}
			else if (!flag && this.hasFrameChangedInPreviousFrame)
			{
				this.OnRotationStopped();
			}
			this.hasFrameChangedInPreviousFrame = flag;
			if ((flag && GameNetwork.IsClient && base.PilotAgent == Agent.Main) || GameNetwork.IsServerOrRecorder)
			{
				float num3 = ((GameNetwork.IsClient && base.PilotAgent == Agent.Main) ? 0.0001f : 0.02f);
				if (this._syncTimer > 0.2f && (MathF.Abs(this.currentDirection - this._lastSyncedDirection) > num3 || MathF.Abs(this.currentReleaseAngle - this._lastSyncedReleaseAngle) > num3))
				{
					this._lastSyncedDirection = this.currentDirection;
					this._lastSyncedReleaseAngle = this.currentReleaseAngle;
					MissionLobbyComponent missionBehavior = Mission.Current.GetMissionBehavior<MissionLobbyComponent>();
					if ((missionBehavior == null || missionBehavior.CurrentMultiplayerState != MissionLobbyComponent.MultiplayerGameState.Ending) && GameNetwork.IsClient && base.PilotAgent == Agent.Main)
					{
						GameNetwork.BeginModuleEventAsClient();
						GameNetwork.WriteMessage(new SetMachineRotation(base.Id, this.currentDirection, this.currentReleaseAngle));
						GameNetwork.EndModuleEventAsClient();
					}
					if (GameNetwork.IsServerOrRecorder)
					{
						GameNetwork.BeginBroadcastModuleEvent();
						GameNetwork.WriteMessage(new SetMachineTargetRotation(base.Id, this.currentDirection, this.currentReleaseAngle));
						GameNetwork.EventBroadcastFlags eventBroadcastFlags = GameNetwork.EventBroadcastFlags.ExcludeTargetPlayer | GameNetwork.EventBroadcastFlags.AddToMissionRecord;
						Agent pilotAgent = base.PilotAgent;
						NetworkCommunicator networkCommunicator;
						if (pilotAgent == null)
						{
							networkCommunicator = null;
						}
						else
						{
							MissionPeer missionPeer = pilotAgent.MissionPeer;
							networkCommunicator = ((missionPeer != null) ? missionPeer.GetNetworkPeer() : null);
						}
						GameNetwork.EndBroadcastModuleEvent(eventBroadcastFlags, networkCommunicator);
					}
				}
			}
			this._syncTimer += dt;
			if (this._syncTimer >= 1f)
			{
				this._syncTimer -= 1f;
			}
			if (flag)
			{
				this.ApplyAimChange();
			}
		}

		public void GiveInput(float inputX, float inputY)
		{
			this._exactInputGiven = false;
			this._inputGiven = true;
			this._inputX = inputX;
			this._inputY = inputY;
			this._inputX = MBMath.ClampFloat(this._inputX, -1f, 1f);
			this._inputY = MBMath.ClampFloat(this._inputY, -1f, 1f);
		}

		public void GiveExactInput(float targetX, float targetY)
		{
			this._exactInputGiven = true;
			this._inputGiven = false;
			this._inputTargetX = MBMath.ClampAngle(targetX, 0f, this.DirectionRestriction);
			this._inputTargetY = MBMath.ClampAngle(targetY, this.ReleaseAngleRestrictionCenter, this.ReleaseAngleRestrictionAngle);
		}

		protected virtual bool CanRotate()
		{
			return this.State == RangedSiegeWeapon.WeaponState.Idle;
		}

		protected virtual void ApplyAimChange()
		{
			if (this.CanRotate())
			{
				this.ApplyCurrentDirectionToEntity();
				return;
			}
			this.targetDirection = this.currentDirection;
			this.targetReleaseAngle = this.currentReleaseAngle;
		}

		protected virtual void ApplyCurrentDirectionToEntity()
		{
			MatrixFrame rotationObjectInitialFrame = this._rotationObjectInitialFrame;
			rotationObjectInitialFrame.rotation.RotateAboutUp(this.currentDirection);
			this.RotationObject.GameEntity.SetFrame(ref rotationObjectInitialFrame);
		}

		public virtual float GetTargetDirection(Vec3 target)
		{
			MatrixFrame globalFrame = base.GameEntity.GetGlobalFrame();
			globalFrame.rotation.RotateAboutUp(3.1415927f);
			return globalFrame.TransformToLocal(target).AsVec2.RotationInRadians;
		}

		public virtual float GetTargetReleaseAngle(Vec3 target)
		{
			return Mission.GetMissileVerticalAimCorrection(target - this.MissleStartingPositionForSimulation, this.ShootingSpeed, ref this._originalMissileWeaponStatsDataForTargeting, ItemObject.GetAirFrictionConstant(this.OriginalMissileItem.PrimaryWeapon.WeaponClass, this.OriginalMissileItem.PrimaryWeapon.WeaponFlags));
		}

		public virtual bool AimAtThreat(Threat threat)
		{
			Vec3 vec = threat.Position + this.GetEstimatedTargetMovementVector(threat.Position, threat.GetVelocity());
			float num = this.GetTargetDirection(vec);
			float num2 = this.GetTargetReleaseAngle(vec);
			num = MBMath.ClampAngle(num, 0f, this.DirectionRestriction);
			num2 = MBMath.ClampAngle(num2, this.ReleaseAngleRestrictionCenter, this.ReleaseAngleRestrictionAngle);
			if (!this._exactInputGiven || num != this._inputTargetX || num2 != this._inputTargetY)
			{
				this.GiveExactInput(num, num2);
			}
			return MathF.Abs(this.currentDirection - this._inputTargetX) < 0.001f && MathF.Abs(this.currentReleaseAngle - this._inputTargetY) < 0.001f;
		}

		public virtual void AimAtRotation(float horizontalRotation, float verticalRotation)
		{
			horizontalRotation = MBMath.ClampFloat(horizontalRotation, -3.1415927f, 3.1415927f);
			verticalRotation = MBMath.ClampFloat(verticalRotation, -3.1415927f, 3.1415927f);
			horizontalRotation = MBMath.ClampAngle(horizontalRotation, 0f, this.DirectionRestriction);
			verticalRotation = MBMath.ClampAngle(verticalRotation, this.ReleaseAngleRestrictionCenter, this.ReleaseAngleRestrictionAngle);
			if (!this._exactInputGiven || horizontalRotation != this._inputTargetX || verticalRotation != this._inputTargetY)
			{
				this.GiveExactInput(horizontalRotation, verticalRotation);
			}
		}

		protected void OnLoadingAmmoPointUsingCancelled(Agent agent, bool isCanceledBecauseOfAnimation)
		{
			if (agent.IsAIControlled)
			{
				if (isCanceledBecauseOfAnimation)
				{
					this.SendAgentToAmmoPickup(agent);
					return;
				}
				this.SendReloaderAgentToOriginalPoint();
			}
		}

		protected void OnAmmoPickupUsingCancelled(Agent agent, bool isCanceledBecauseOfAnimation)
		{
			if (agent.IsAIControlled)
			{
				this.SendAgentToAmmoPickup(agent);
			}
		}

		protected void SendAgentToAmmoPickup(Agent agent)
		{
			this.ReloaderAgent = agent;
			EquipmentIndex wieldedItemIndex = agent.GetWieldedItemIndex(Agent.HandIndex.MainHand);
			if (wieldedItemIndex != EquipmentIndex.None && agent.Equipment[wieldedItemIndex].CurrentUsageItem.WeaponClass == this.OriginalMissileItem.PrimaryWeapon.WeaponClass)
			{
				agent.AIMoveToGameObjectEnable(this.LoadAmmoStandingPoint, this, base.Ai.GetScriptedFrameFlags(agent));
				return;
			}
			StandingPoint standingPoint = base.AmmoPickUpPoints.FirstOrDefault((StandingPoint x) => !x.HasUser);
			if (standingPoint != null)
			{
				agent.AIMoveToGameObjectEnable(standingPoint, this, base.Ai.GetScriptedFrameFlags(agent));
				return;
			}
			this.SendReloaderAgentToOriginalPoint();
		}

		protected void SendReloaderAgentToOriginalPoint()
		{
			if (this.ReloaderAgent != null)
			{
				if (this.ReloaderAgentOriginalPoint != null && !this.ReloaderAgentOriginalPoint.HasAIMovingTo && !this.ReloaderAgentOriginalPoint.HasUser)
				{
					if (this.ReloaderAgent.InteractingWithAnyGameObject())
					{
						this.ReloaderAgent.StopUsingGameObject(true, Agent.StopUsingGameObjectFlags.None);
					}
					this.ReloaderAgent.AIMoveToGameObjectEnable(this.ReloaderAgentOriginalPoint, this, base.Ai.GetScriptedFrameFlags(this.ReloaderAgent));
					return;
				}
				if (this.ReloaderAgentOriginalPoint == null || (this.ReloaderAgentOriginalPoint.MovingAgent != this.ReloaderAgent && this.ReloaderAgentOriginalPoint.UserAgent != this.ReloaderAgent))
				{
					if (this.ReloaderAgent.IsUsingGameObject)
					{
						this.ReloaderAgent.StopUsingGameObject(true, Agent.StopUsingGameObjectFlags.AutoAttachAfterStoppingUsingGameObject);
					}
					this.ReloaderAgent = null;
				}
			}
		}

		private void UpdateState(float dt)
		{
			if (this.LoadAmmoStandingPoint != null)
			{
				if (this.ReloaderAgent != null)
				{
					if (!this.ReloaderAgent.IsActive() || this.ReloaderAgent.Detachment != this)
					{
						this.ReloaderAgent = null;
					}
					else if (this.ReloaderAgentOriginalPoint.UserAgent == this.ReloaderAgent)
					{
						this.ReloaderAgent = null;
					}
				}
				if (this.State == RangedSiegeWeapon.WeaponState.LoadingAmmo && this.ReloaderAgent == null && !this.LoadAmmoStandingPoint.HasUser)
				{
					this.SortCanPickUpAmmoStandingPoints();
					StandingPoint standingPoint = null;
					StandingPoint standingPoint2 = null;
					foreach (StandingPoint standingPoint3 in this.CanPickUpAmmoStandingPoints)
					{
						if (standingPoint3.HasUser && standingPoint3.UserAgent.IsAIControlled)
						{
							if (standingPoint3 != base.PilotStandingPoint)
							{
								standingPoint = standingPoint3;
								break;
							}
							standingPoint2 = standingPoint3;
						}
					}
					if (standingPoint == null && standingPoint2 != null)
					{
						standingPoint = standingPoint2;
					}
					if (standingPoint != null)
					{
						if (this.HasAmmo)
						{
							Agent userAgent = standingPoint.UserAgent;
							userAgent.StopUsingGameObject(true, Agent.StopUsingGameObjectFlags.DoNotWieldWeaponAfterStoppingUsingGameObject);
							this.ReloaderAgentOriginalPoint = standingPoint;
							this.SendAgentToAmmoPickup(userAgent);
						}
						else
						{
							this._isDisabledForAI = true;
						}
					}
				}
			}
			switch (this.State)
			{
			case RangedSiegeWeapon.WeaponState.Idle:
			case RangedSiegeWeapon.WeaponState.WaitingAfterShooting:
			case RangedSiegeWeapon.WeaponState.LoadingAmmo:
			case RangedSiegeWeapon.WeaponState.WaitingBeforeIdle:
				return;
			case RangedSiegeWeapon.WeaponState.WaitingBeforeProjectileLeaving:
				goto IL_406;
			case RangedSiegeWeapon.WeaponState.Shooting:
			{
				for (int i = 0; i < this.Skeletons.Length; i++)
				{
					int animationIndexAtChannel = this.Skeletons[i].GetAnimationIndexAtChannel(0);
					float animationParameterAtChannel = this.Skeletons[i].GetAnimationParameterAtChannel(0);
					if (animationIndexAtChannel == this.FireAnimationIndices[i] && animationParameterAtChannel >= 0.9999f)
					{
						this.State = ((!this.AttackClickWillReload) ? RangedSiegeWeapon.WeaponState.WaitingBeforeReloading : RangedSiegeWeapon.WeaponState.WaitingAfterShooting);
						this.animationTimeElapsed = 0f;
					}
				}
				return;
			}
			case RangedSiegeWeapon.WeaponState.WaitingBeforeReloading:
				break;
			case RangedSiegeWeapon.WeaponState.Reloading:
			{
				int num = 0;
				if (this.ReloadStandingPoints.Count == 0)
				{
					if (base.PilotAgent != null && !base.PilotAgent.IsInBeingStruckAction)
					{
						num = 1;
					}
				}
				else
				{
					foreach (StandingPoint standingPoint4 in this.ReloadStandingPoints)
					{
						if (standingPoint4.HasUser && !standingPoint4.UserAgent.IsInBeingStruckAction)
						{
							num++;
						}
					}
				}
				if (num == 0)
				{
					this.State = RangedSiegeWeapon.WeaponState.ReloadingPaused;
					return;
				}
				if (this._currentReloaderCount != num)
				{
					this._currentReloaderCount = num;
					float num2 = MathF.Sqrt((float)this._currentReloaderCount);
					for (int j = 0; j < this.SkeletonOwnerObjects.Length; j++)
					{
						float animationParameterAtChannel2 = this.SkeletonOwnerObjects[j].GameEntity.Skeleton.GetAnimationParameterAtChannel(0);
						this.SkeletonOwnerObjects[j].SetAnimationAtChannelSynched(this.SetUpAnimations[j], 0, num2);
						if (animationParameterAtChannel2 > 0f)
						{
							this.SkeletonOwnerObjects[j].SetAnimationChannelParameterSynched(0, animationParameterAtChannel2);
						}
					}
				}
				for (int k = 0; k < this.Skeletons.Length; k++)
				{
					int animationIndexAtChannel2 = this.Skeletons[k].GetAnimationIndexAtChannel(0);
					float animationParameterAtChannel3 = this.Skeletons[k].GetAnimationParameterAtChannel(0);
					if (animationIndexAtChannel2 == this.SetUpAnimationIndices[k] && animationParameterAtChannel3 >= 0.9999f)
					{
						this.State = RangedSiegeWeapon.WeaponState.LoadingAmmo;
						this.animationTimeElapsed = 0f;
					}
				}
				return;
			}
			case RangedSiegeWeapon.WeaponState.ReloadingPaused:
				if (this.ReloadStandingPoints.Count == 0)
				{
					if (base.PilotAgent != null && !base.PilotAgent.IsInBeingStruckAction)
					{
						this.State = RangedSiegeWeapon.WeaponState.Reloading;
						return;
					}
					return;
				}
				else
				{
					using (List<StandingPoint>.Enumerator enumerator = this.ReloadStandingPoints.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							StandingPoint standingPoint5 = enumerator.Current;
							if (standingPoint5.HasUser && !standingPoint5.UserAgent.IsInBeingStruckAction)
							{
								this.State = RangedSiegeWeapon.WeaponState.Reloading;
								break;
							}
						}
						return;
					}
				}
				break;
			default:
				Debug.FailedAssert("Invalid WeaponState.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Objects\\Siege\\RangedSiegeWeapon.cs", "UpdateState", 1899);
				return;
			}
			this.animationTimeElapsed += dt;
			if (this.animationTimeElapsed < this.timeGapBetweenShootingEndAndReloadingStart || (this.cameraState != RangedSiegeWeapon.CameraState.RememberLastShotDirection && this.cameraState != RangedSiegeWeapon.CameraState.FreeMove && this.cameraState != RangedSiegeWeapon.CameraState.StickToWeapon && !(this.cameraHolder == null)))
			{
				return;
			}
			if (this.ReloadStandingPoints.Count == 0)
			{
				if (base.PilotAgent != null && !base.PilotAgent.IsInBeingStruckAction)
				{
					this.State = RangedSiegeWeapon.WeaponState.Reloading;
					return;
				}
				return;
			}
			else
			{
				using (List<StandingPoint>.Enumerator enumerator = this.ReloadStandingPoints.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						StandingPoint standingPoint6 = enumerator.Current;
						if (standingPoint6.HasUser && !standingPoint6.UserAgent.IsInBeingStruckAction)
						{
							this.State = RangedSiegeWeapon.WeaponState.Reloading;
							break;
						}
					}
					return;
				}
			}
			IL_406:
			this.animationTimeElapsed += dt;
			if (this.animationTimeElapsed >= this.timeGapBetweenShootActionAndProjectileLeaving)
			{
				this.State = RangedSiegeWeapon.WeaponState.Shooting;
				return;
			}
		}

		public bool Shoot()
		{
			this._lastShooterAgent = base.PilotAgent;
			if (this.State == RangedSiegeWeapon.WeaponState.Idle)
			{
				this.State = RangedSiegeWeapon.WeaponState.WaitingBeforeProjectileLeaving;
				if (!GameNetwork.IsClientOrReplay)
				{
					this.animationTimeElapsed = 0f;
				}
				return true;
			}
			return false;
		}

		public void ManualReload()
		{
			if (this.AttackClickWillReload)
			{
				this.State = RangedSiegeWeapon.WeaponState.WaitingBeforeReloading;
			}
		}

		public void AiRequestsShoot()
		{
			this._aiRequestsShoot = true;
		}

		public void AiRequestsManualReload()
		{
			this._aiRequestsManualReload = true;
		}

		private Vec3 GetBallisticErrorAppliedDirection(float BallisticErrorAmount)
		{
			Mat3 mat = new Mat3
			{
				f = this.ShootingDirection,
				u = Vec3.Up
			};
			mat.Orthonormalize();
			float num = MBRandom.RandomFloat * 6.2831855f;
			mat.RotateAboutForward(num);
			float num2 = BallisticErrorAmount * MBRandom.RandomFloat;
			mat.RotateAboutSide(num2.ToRadians());
			return mat.f;
		}

		private void ShootProjectile()
		{
			if (this.LoadedMissileItem.StringId == "grapeshot_fire_stack")
			{
				ItemObject @object = Game.Current.ObjectManager.GetObject<ItemObject>("grapeshot_fire_projectile");
				for (int i = 0; i < 5; i++)
				{
					this.ShootProjectileAux(@object, true);
				}
			}
			else
			{
				this.ShootProjectileAux(this.LoadedMissileItem, false);
			}
			this._lastShooterAgent = null;
		}

		private void ShootProjectileAux(ItemObject missileItem, bool randomizeMissileSpeed)
		{
			Mat3 identity = Mat3.Identity;
			float num = this.ShootingSpeed;
			if (randomizeMissileSpeed)
			{
				num *= MBRandom.RandomFloatRanged(0.9f, 1.1f);
				identity.f = this.GetBallisticErrorAppliedDirection(2.5f);
				identity.Orthonormalize();
			}
			else
			{
				identity.f = this.GetBallisticErrorAppliedDirection(this.MaximumBallisticError);
				identity.Orthonormalize();
			}
			Mission mission = Mission.Current;
			Agent lastShooterAgent = this._lastShooterAgent;
			ItemModifier itemModifier = null;
			IAgentOriginBase origin = this._lastShooterAgent.Origin;
			mission.AddCustomMissile(lastShooterAgent, new MissionWeapon(missileItem, itemModifier, (origin != null) ? origin.Banner : null, 1), this.ProjectileEntityCurrentGlobalPosition, identity.f, identity, (float)this.LoadedMissileItem.PrimaryWeapon.MissileSpeed, num, false, this, -1);
		}

		protected virtual Vec3 ShootingDirection
		{
			get
			{
				return this.Projectile.GameEntity.GetGlobalFrame().rotation.u;
			}
		}

		public virtual Vec3 ProjectileEntityCurrentGlobalPosition
		{
			get
			{
				return this.Projectile.GameEntity.GetGlobalFrame().origin;
			}
		}

		protected void OnRotationStarted()
		{
			if (this.MoveSound == null || !this.MoveSound.IsValid)
			{
				this.MoveSound = SoundEvent.CreateEvent(this.MoveSoundIndex, base.Scene);
				this.MoveSound.PlayInPosition(this.RotationObject.GameEntity.GlobalPosition);
			}
		}

		protected void OnRotationStopped()
		{
			this.MoveSound.Stop();
			this.MoveSound = null;
		}

		public abstract override SiegeEngineType GetSiegeEngineType();

		public override BattleSideEnum Side
		{
			get
			{
				if (base.PilotAgent != null)
				{
					return base.PilotAgent.Team.Side;
				}
				return this._defaultSide;
			}
		}

		public bool CanShootAtBox(Vec3 boxMin, Vec3 boxMax, uint attempts = 5U)
		{
			Vec3 vec2;
			Vec3 vec = (vec2 = (boxMin + boxMax) / 2f);
			vec2.z = boxMin.z;
			Vec3 vec3 = vec;
			vec3.z = boxMax.z;
			uint num = attempts;
			for (;;)
			{
				Vec3 vec4 = Vec3.Lerp(vec2, vec3, num / attempts);
				if (this.CanShootAtPoint(vec4))
				{
					break;
				}
				num -= 1U;
				if (num <= 0U)
				{
					return false;
				}
			}
			return true;
		}

		public bool CanShootAtBoxSimplified(Vec3 boxMin, Vec3 boxMax)
		{
			Vec3 vec = (boxMin + boxMax) / 2f;
			Vec3 vec2 = vec;
			vec2.z = boxMax.z;
			return this.CanShootAtPoint(vec) || this.CanShootAtPoint(vec2);
		}

		public bool CanShootAtThreat(Threat threat)
		{
			Vec3 targetingOffset = threat.WeaponEntity.GetTargetingOffset();
			Vec3 vec = threat.BoundingBoxMax + targetingOffset;
			Vec3 vec2 = threat.BoundingBoxMin + targetingOffset;
			Vec3 vec3 = (vec + vec2) * 0.5f;
			Vec3 estimatedTargetMovementVector = this.GetEstimatedTargetMovementVector(vec3, threat.GetVelocity());
			vec3 += estimatedTargetMovementVector;
			vec += estimatedTargetMovementVector;
			Vec3 vec4 = vec3;
			vec4.z = vec.z;
			return this.CanShootAtPoint(vec3) || this.CanShootAtPoint(vec4);
		}

		public Vec3 GetEstimatedTargetMovementVector(Vec3 targetCurrentPosition, Vec3 targetVelocity)
		{
			if (targetVelocity != Vec3.Zero)
			{
				return targetVelocity * ((base.GameEntity.GlobalPosition - targetCurrentPosition).Length / this.ShootingSpeed + this.timeGapBetweenShootActionAndProjectileLeaving);
			}
			return Vec3.Zero;
		}

		public bool CanShootAtAgent(Agent agent)
		{
			Vec3 boxMax = agent.CollisionCapsule.GetBoxMax();
			Vec3 vec = (agent.CollisionCapsule.GetBoxMin() + boxMax) / 2f;
			return this.CanShootAtPoint(vec);
		}

		public unsafe bool CanShootAtPoint(Vec3 target)
		{
			float num = this.GetTargetReleaseAngle(target);
			if (num < this.BottomReleaseAngleRestriction || num > this.TopReleaseAngleRestriction)
			{
				return false;
			}
			float num2 = (target.AsVec2 - this.ProjectileEntityCurrentGlobalPosition.AsVec2).Normalized().AngleBetween(this.OriginalDirection.AsVec2.Normalized());
			if (this.DirectionRestriction / 2f - MathF.Abs(num2) < 0f)
			{
				return false;
			}
			if (this.Side == BattleSideEnum.Attacker)
			{
				foreach (SiegeWeapon siegeWeapon in *Mission.Current.GetAttackerWeaponsForFriendlyFirePreventing())
				{
					if (siegeWeapon.GameEntity != null && siegeWeapon.GameEntity.IsVisibleIncludeParents())
					{
						Vec3 vec = (siegeWeapon.GameEntity.PhysicsGlobalBoxMin + siegeWeapon.GameEntity.PhysicsGlobalBoxMax) * 0.5f;
						if ((MBMath.GetClosestPointInLineSegmentToPoint(vec, this.MissleStartingPositionForSimulation, target) - vec).LengthSquared < 100f)
						{
							return false;
						}
					}
				}
			}
			Vec3 missleStartingPositionForSimulation = this.MissleStartingPositionForSimulation;
			Vec3 vec2 = ((this.MissileStartingPositionEntityForSimulation == null) ? this.CanShootAtPointCheckingOffset : Vec3.Zero);
			Vec3 vec3 = target;
			return base.Scene.CheckPointCanSeePoint(missleStartingPositionForSimulation + vec2, vec3, null);
		}

		protected internal virtual bool IsTargetValid(ITargetable target)
		{
			return true;
		}

		public override OrderType GetOrder(BattleSideEnum side)
		{
			if (base.IsDestroyed)
			{
				return OrderType.None;
			}
			if (this.Side != side)
			{
				return OrderType.AttackEntity;
			}
			return OrderType.Use;
		}

		protected override GameEntity GetEntityToAttachNavMeshFaces()
		{
			return this.RotationObject.GameEntity;
		}

		public abstract float ProcessTargetValue(float baseValue, TargetFlags flags);

		public override void OnAfterReadFromNetwork(ValueTuple<BaseSynchedMissionObjectReadableRecord, ISynchedMissionObjectReadableRecord> synchedMissionObjectReadableRecord)
		{
			base.OnAfterReadFromNetwork(synchedMissionObjectReadableRecord);
			RangedSiegeWeapon.RangedSiegeWeaponRecord rangedSiegeWeaponRecord = (RangedSiegeWeapon.RangedSiegeWeaponRecord)synchedMissionObjectReadableRecord.Item2;
			this._state = (RangedSiegeWeapon.WeaponState)rangedSiegeWeaponRecord.State;
			this.targetDirection = rangedSiegeWeaponRecord.TargetDirection;
			this.targetReleaseAngle = MBMath.ClampFloat(rangedSiegeWeaponRecord.TargetReleaseAngle, this.BottomReleaseAngleRestriction, this.TopReleaseAngleRestriction);
			this.AmmoCount = rangedSiegeWeaponRecord.AmmoCount;
			this.currentDirection = this.targetDirection;
			this.currentReleaseAngle = this.targetReleaseAngle;
			this.currentDirection = this.targetDirection;
			this.currentReleaseAngle = this.targetReleaseAngle;
			this.ApplyCurrentDirectionToEntity();
			this.CheckAmmo();
			this.UpdateAmmoMesh();
			this.ChangeProjectileEntityClient(rangedSiegeWeaponRecord.ProjectileIndex);
		}

		protected virtual void UpdateAmmoMesh()
		{
			GameEntity gameEntity = this.AmmoPickUpStandingPoints[0].GameEntity;
			int num = 20 - this.AmmoCount;
			while (gameEntity.Parent != null)
			{
				for (int i = 0; i < gameEntity.MultiMeshComponentCount; i++)
				{
					MetaMesh metaMesh = gameEntity.GetMetaMesh(i);
					for (int j = 0; j < metaMesh.MeshCount; j++)
					{
						metaMesh.GetMeshAtIndex(j).SetVectorArgument(0f, (float)num, 0f, 0f);
					}
				}
				gameEntity = gameEntity.Parent;
			}
		}

		protected override bool IsAnyUserBelongsToFormation(Formation formation)
		{
			bool flag = base.IsAnyUserBelongsToFormation(formation);
			Agent reloaderAgent = this.ReloaderAgent;
			return flag | (((reloaderAgent != null) ? reloaderAgent.Formation : null) == formation);
		}

		public const float DefaultDirectionRestriction = 2.0943952f;

		public const string MultipleProjectileId = "grapeshot_fire_stack";

		public const string MultipleProjectileFlyingId = "grapeshot_fire_projectile";

		public const int MultipleProjectileCount = 5;

		public const string CanGoAmmoPickupTag = "can_pick_up_ammo";

		public const string DontApplySidePenaltyTag = "no_ammo_pick_up_penalty";

		public const string ReloadTag = "reload";

		public const string AmmoLoadTag = "ammoload";

		public const string CameraHolderTag = "cameraHolder";

		public const string ProjectileTag = "projectile";

		public string MissileItemID;

		protected bool UsesMouseForAiming;

		private RangedSiegeWeapon.WeaponState _state;

		public RangedSiegeWeapon.FiringFocus Focus;

		private int _projectileIndex;

		protected GameEntity MissileStartingPositionEntityForSimulation;

		protected Skeleton[] Skeletons;

		protected SynchedMissionObject[] SkeletonOwnerObjects;

		protected string[] SkeletonNames;

		protected string[] FireAnimations;

		protected string[] SetUpAnimations;

		protected int[] FireAnimationIndices;

		protected int[] SetUpAnimationIndices;

		protected SynchedMissionObject RotationObject;

		private MatrixFrame _rotationObjectInitialFrame;

		protected SoundEvent MoveSound;

		protected SoundEvent ReloadSound;

		protected int MoveSoundIndex = -1;

		protected int ReloadSoundIndex = -1;

		protected int ReloadEndSoundIndex = -1;

		protected ItemObject OriginalMissileItem;

		private WeaponStatsData _originalMissileWeaponStatsDataForTargeting;

		protected ItemObject LoadedMissileItem;

		protected List<StandingPoint> CanPickUpAmmoStandingPoints;

		protected List<StandingPoint> ReloadStandingPoints;

		protected List<StandingPointWithWeaponRequirement> AmmoPickUpStandingPoints;

		protected StandingPointWithWeaponRequirement LoadAmmoStandingPoint;

		protected Dictionary<StandingPoint, float> PilotReservePriorityValues = new Dictionary<StandingPoint, float>();

		protected Agent ReloaderAgent;

		protected StandingPoint ReloaderAgentOriginalPoint;

		protected bool AttackClickWillReload;

		protected bool WeaponNeedsClickToReload;

		public int startingAmmoCount = 20;

		protected int CurrentAmmo = 1;

		private bool _hasAmmo = true;

		protected float targetDirection;

		protected float targetReleaseAngle;

		protected float cameraDirection;

		protected float cameraReleaseAngle;

		protected float reloadTargetReleaseAngle;

		protected float maxRotateSpeed;

		protected float dontMoveTimer;

		private MatrixFrame cameraHolderInitialFrame;

		private RangedSiegeWeapon.CameraState cameraState;

		private bool _inputGiven;

		private float _inputX;

		private float _inputY;

		private bool _exactInputGiven;

		private float _inputTargetX;

		private float _inputTargetY;

		private Vec3 _ammoPickupCenter;

		protected float currentDirection;

		private Vec3 _originalDirection;

		protected float currentReleaseAngle;

		private float _lastSyncedDirection;

		private float _lastSyncedReleaseAngle;

		private float _syncTimer;

		public float TopReleaseAngleRestriction = 1.5707964f;

		public float BottomReleaseAngleRestriction = -1.5707964f;

		protected float ReleaseAngleRestrictionCenter;

		protected float ReleaseAngleRestrictionAngle;

		private float animationTimeElapsed;

		protected float timeGapBetweenShootingEndAndReloadingStart = 0.6f;

		protected float timeGapBetweenShootActionAndProjectileLeaving;

		private int _currentReloaderCount;

		private Agent _lastShooterAgent;

		private float _lastCanPickUpAmmoStandingPointsSortedAngle = -3.1415927f;

		protected BattleSideEnum _defaultSide;

		private bool hasFrameChangedInPreviousFrame;

		protected SiegeMachineStonePile _stonePile;

		private bool _aiRequestsShoot;

		private bool _aiRequestsManualReload;

		[DefineSynchedMissionObjectType(typeof(RangedSiegeWeapon))]
		public struct RangedSiegeWeaponRecord : ISynchedMissionObjectReadableRecord
		{
			public int State { get; private set; }

			public float TargetDirection { get; private set; }

			public float TargetReleaseAngle { get; private set; }

			public int AmmoCount { get; private set; }

			public int ProjectileIndex { get; private set; }

			public bool ReadFromNetwork(ref bool bufferReadValid)
			{
				this.State = GameNetworkMessage.ReadIntFromPacket(CompressionMission.RangedSiegeWeaponStateCompressionInfo, ref bufferReadValid);
				this.TargetDirection = GameNetworkMessage.ReadFloatFromPacket(CompressionBasic.RadianCompressionInfo, ref bufferReadValid);
				this.TargetReleaseAngle = GameNetworkMessage.ReadFloatFromPacket(CompressionBasic.RadianCompressionInfo, ref bufferReadValid);
				this.AmmoCount = GameNetworkMessage.ReadIntFromPacket(CompressionMission.RangedSiegeWeaponAmmoCompressionInfo, ref bufferReadValid);
				this.ProjectileIndex = GameNetworkMessage.ReadIntFromPacket(CompressionMission.RangedSiegeWeaponAmmoIndexCompressionInfo, ref bufferReadValid);
				return bufferReadValid;
			}
		}

		public enum WeaponState
		{
			Invalid = -1,
			Idle,
			WaitingBeforeProjectileLeaving,
			Shooting,
			WaitingAfterShooting,
			WaitingBeforeReloading,
			LoadingAmmo,
			WaitingBeforeIdle,
			Reloading,
			ReloadingPaused,
			NumberOfStates
		}

		public enum FiringFocus
		{
			Troops,
			Walls,
			RangedSiegeWeapons,
			PrimarySiegeWeapons
		}

		public delegate void OnSiegeWeaponReloadDone();

		public enum CameraState
		{
			StickToWeapon,
			DontMove,
			MoveDownToReload,
			RememberLastShotDirection,
			FreeMove,
			ApproachToCamera
		}
	}
}
