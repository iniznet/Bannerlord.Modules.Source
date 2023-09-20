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
	// Token: 0x02000357 RID: 855
	public abstract class RangedSiegeWeapon : SiegeWeapon
	{
		// Token: 0x1700083F RID: 2111
		// (get) Token: 0x06002DED RID: 11757 RVA: 0x000B58F1 File Offset: 0x000B3AF1
		// (set) Token: 0x06002DEE RID: 11758 RVA: 0x000B58F9 File Offset: 0x000B3AF9
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
						GameNetwork.WriteMessage(new SetRangedSiegeWeaponState(this, value));
						GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
					}
					this._state = value;
					this.OnRangedSiegeWeaponStateChange();
				}
			}
		}

		// Token: 0x17000840 RID: 2112
		// (get) Token: 0x06002DEF RID: 11759 RVA: 0x000B5931 File Offset: 0x000B3B31
		protected virtual float MaximumBallisticError
		{
			get
			{
				return 1f;
			}
		}

		// Token: 0x17000841 RID: 2113
		// (get) Token: 0x06002DF0 RID: 11760
		protected abstract float ShootingSpeed { get; }

		// Token: 0x17000842 RID: 2114
		// (get) Token: 0x06002DF1 RID: 11761 RVA: 0x000B5938 File Offset: 0x000B3B38
		public virtual Vec3 CanShootAtPointCheckingOffset
		{
			get
			{
				return Vec3.Zero;
			}
		}

		// Token: 0x17000843 RID: 2115
		// (get) Token: 0x06002DF2 RID: 11762 RVA: 0x000B593F File Offset: 0x000B3B3F
		// (set) Token: 0x06002DF3 RID: 11763 RVA: 0x000B5947 File Offset: 0x000B3B47
		public GameEntity cameraHolder { get; private set; }

		// Token: 0x17000844 RID: 2116
		// (get) Token: 0x06002DF4 RID: 11764 RVA: 0x000B5950 File Offset: 0x000B3B50
		// (set) Token: 0x06002DF5 RID: 11765 RVA: 0x000B5958 File Offset: 0x000B3B58
		private protected SynchedMissionObject Projectile { protected get; private set; }

		// Token: 0x17000845 RID: 2117
		// (get) Token: 0x06002DF6 RID: 11766 RVA: 0x000B5961 File Offset: 0x000B3B61
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

		// Token: 0x17000846 RID: 2118
		// (set) Token: 0x06002DF7 RID: 11767 RVA: 0x000B5997 File Offset: 0x000B3B97
		protected string SkeletonName
		{
			set
			{
				this.SkeletonNames = new string[] { value };
			}
		}

		// Token: 0x17000847 RID: 2119
		// (set) Token: 0x06002DF8 RID: 11768 RVA: 0x000B59A9 File Offset: 0x000B3BA9
		protected string FireAnimation
		{
			set
			{
				this.FireAnimations = new string[] { value };
			}
		}

		// Token: 0x17000848 RID: 2120
		// (set) Token: 0x06002DF9 RID: 11769 RVA: 0x000B59BB File Offset: 0x000B3BBB
		protected string SetUpAnimation
		{
			set
			{
				this.SetUpAnimations = new string[] { value };
			}
		}

		// Token: 0x17000849 RID: 2121
		// (set) Token: 0x06002DFA RID: 11770 RVA: 0x000B59CD File Offset: 0x000B3BCD
		protected int FireAnimationIndex
		{
			set
			{
				this.FireAnimationIndices = new int[] { value };
			}
		}

		// Token: 0x1700084A RID: 2122
		// (set) Token: 0x06002DFB RID: 11771 RVA: 0x000B59DF File Offset: 0x000B3BDF
		protected int SetUpAnimationIndex
		{
			set
			{
				this.SetUpAnimationIndices = new int[] { value };
			}
		}

		// Token: 0x14000093 RID: 147
		// (add) Token: 0x06002DFC RID: 11772 RVA: 0x000B59F4 File Offset: 0x000B3BF4
		// (remove) Token: 0x06002DFD RID: 11773 RVA: 0x000B5A2C File Offset: 0x000B3C2C
		public event RangedSiegeWeapon.OnSiegeWeaponReloadDone OnReloadDone;

		// Token: 0x1700084B RID: 2123
		// (get) Token: 0x06002DFE RID: 11774 RVA: 0x000B5A61 File Offset: 0x000B3C61
		// (set) Token: 0x06002DFF RID: 11775 RVA: 0x000B5A69 File Offset: 0x000B3C69
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

		// Token: 0x1700084C RID: 2124
		// (get) Token: 0x06002E00 RID: 11776 RVA: 0x000B5A72 File Offset: 0x000B3C72
		// (set) Token: 0x06002E01 RID: 11777 RVA: 0x000B5A7A File Offset: 0x000B3C7A
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

		// Token: 0x06002E02 RID: 11778 RVA: 0x000B5A84 File Offset: 0x000B3C84
		protected virtual void ConsumeAmmo()
		{
			int ammoCount = this.AmmoCount;
			this.AmmoCount = ammoCount - 1;
			if (GameNetwork.IsServerOrRecorder)
			{
				GameNetwork.BeginBroadcastModuleEvent();
				GameNetwork.WriteMessage(new SetRangedSiegeWeaponAmmo(this, this.AmmoCount));
				GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
			}
			this.UpdateAmmoMesh();
			this.CheckAmmo();
		}

		// Token: 0x06002E03 RID: 11779 RVA: 0x000B5AD2 File Offset: 0x000B3CD2
		public virtual void SetAmmo(int ammoLeft)
		{
			if (this.AmmoCount != ammoLeft)
			{
				this.AmmoCount = ammoLeft;
				this.UpdateAmmoMesh();
				this.CheckAmmo();
			}
		}

		// Token: 0x06002E04 RID: 11780 RVA: 0x000B5AF0 File Offset: 0x000B3CF0
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

		// Token: 0x1700084D RID: 2125
		// (get) Token: 0x06002E05 RID: 11781 RVA: 0x000B5B58 File Offset: 0x000B3D58
		public virtual float DirectionRestriction
		{
			get
			{
				return 2.0943952f;
			}
		}

		// Token: 0x1700084E RID: 2126
		// (get) Token: 0x06002E06 RID: 11782 RVA: 0x000B5B5F File Offset: 0x000B3D5F
		public Vec3 OriginalDirection
		{
			get
			{
				return this._originalDirection;
			}
		}

		// Token: 0x1700084F RID: 2127
		// (get) Token: 0x06002E07 RID: 11783 RVA: 0x000B5B67 File Offset: 0x000B3D67
		protected virtual float HorizontalAimSensitivity
		{
			get
			{
				return 0.2f;
			}
		}

		// Token: 0x17000850 RID: 2128
		// (get) Token: 0x06002E08 RID: 11784 RVA: 0x000B5B6E File Offset: 0x000B3D6E
		protected virtual float VerticalAimSensitivity
		{
			get
			{
				return 0.2f;
			}
		}

		// Token: 0x06002E09 RID: 11785
		protected abstract void RegisterAnimationParameters();

		// Token: 0x06002E0A RID: 11786
		protected abstract void GetSoundEventIndices();

		// Token: 0x14000094 RID: 148
		// (add) Token: 0x06002E0B RID: 11787 RVA: 0x000B5B78 File Offset: 0x000B3D78
		// (remove) Token: 0x06002E0C RID: 11788 RVA: 0x000B5BB0 File Offset: 0x000B3DB0
		public event Action<RangedSiegeWeapon, Agent> OnAgentLoadsMachine;

		// Token: 0x06002E0D RID: 11789 RVA: 0x000B5BE8 File Offset: 0x000B3DE8
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
				GameNetwork.WriteMessage(new RangedSiegeWeaponChangeProjectile(this, this._projectileIndex));
				GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord, null);
			}
			Action<RangedSiegeWeapon, Agent> onAgentLoadsMachine = this.OnAgentLoadsMachine;
			if (onAgentLoadsMachine == null)
			{
				return;
			}
			onAgentLoadsMachine(this, loadingAgent);
		}

		// Token: 0x06002E0E RID: 11790 RVA: 0x000B5C90 File Offset: 0x000B3E90
		public void ChangeProjectileEntityClient(int index)
		{
			List<SynchedMissionObject> list = base.GameEntity.CollectObjectsWithTag("projectile");
			this.Projectile = list[index];
			this._projectileIndex = index;
		}

		// Token: 0x06002E0F RID: 11791 RVA: 0x000B5CC4 File Offset: 0x000B3EC4
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
				Debug.FailedAssert("Ranged siege weapons must have destructible component.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Objects\\Siege\\RangedSiegeWeapon.cs", "OnInit", 396);
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
			int signOfAmmoPile = Math.Sign(Vec3.DotProduct(base.GameEntity.GetGlobalFrame().rotation.s, this._ammoPickupCenter - base.GameEntity.GlobalPosition));
			this.CanPickUpAmmoStandingPoints.Sort(delegate(StandingPoint element1, StandingPoint element2)
			{
				Vec3 vec2 = this._ammoPickupCenter - element1.GameEntity.GlobalPosition;
				Vec3 vec3 = this._ammoPickupCenter - element2.GameEntity.GlobalPosition;
				float num = vec2.LengthSquared;
				float num2 = vec3.LengthSquared;
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

		// Token: 0x06002E10 RID: 11792 RVA: 0x000B6300 File Offset: 0x000B4500
		protected internal override void OnEditorInit()
		{
			List<SynchedMissionObject> list = base.GameEntity.CollectObjectsWithTag("projectile");
			if (list.Count > 0)
			{
				this.Projectile = list[0];
			}
			this.VisualizeReleaseTrajectoryAngle = this.TopReleaseAngleRestriction;
		}

		// Token: 0x06002E11 RID: 11793 RVA: 0x000B6340 File Offset: 0x000B4540
		private void InitAnimations()
		{
			for (int i = 0; i < this.Skeletons.Length; i++)
			{
				this.Skeletons[i].SetAnimationAtChannel(this.SetUpAnimations[i], 0, 1f, 0f, 0f);
				this.Skeletons[i].SetAnimationParameterAtChannel(0, 1f);
				this.Skeletons[i].TickAnimations(0.0001f, MatrixFrame.Identity, true);
			}
		}

		// Token: 0x06002E12 RID: 11794 RVA: 0x000B63B0 File Offset: 0x000B45B0
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

		// Token: 0x06002E13 RID: 11795 RVA: 0x000B6530 File Offset: 0x000B4730
		public override bool ReadFromNetwork()
		{
			bool flag = true;
			flag = flag && base.ReadFromNetwork();
			int num = GameNetworkMessage.ReadIntFromPacket(CompressionMission.RangedSiegeWeaponStateCompressionInfo, ref flag);
			float num2 = GameNetworkMessage.ReadFloatFromPacket(CompressionBasic.RadianCompressionInfo, ref flag);
			float num3 = GameNetworkMessage.ReadFloatFromPacket(CompressionBasic.RadianCompressionInfo, ref flag);
			int num4 = GameNetworkMessage.ReadIntFromPacket(CompressionMission.RangedSiegeWeaponAmmoCompressionInfo, ref flag);
			int num5 = GameNetworkMessage.ReadIntFromPacket(CompressionMission.RangedSiegeWeaponAmmoIndexCompressionInfo, ref flag);
			if (flag)
			{
				this._state = (RangedSiegeWeapon.WeaponState)num;
				this.targetDirection = num2;
				this.targetReleaseAngle = MBMath.ClampFloat(num3, this.BottomReleaseAngleRestriction, this.TopReleaseAngleRestriction);
				this.AmmoCount = num4;
				this.currentDirection = this.targetDirection;
				this.currentReleaseAngle = this.targetReleaseAngle;
				this.currentDirection = this.targetDirection;
				this.currentReleaseAngle = this.targetReleaseAngle;
				this.ApplyCurrentDirectionToEntity();
				this.CheckAmmo();
				this.UpdateAmmoMesh();
				this.ChangeProjectileEntityClient(num5);
			}
			return flag;
		}

		// Token: 0x06002E14 RID: 11796 RVA: 0x000B660C File Offset: 0x000B480C
		public override void WriteToNetwork()
		{
			base.WriteToNetwork();
			GameNetworkMessage.WriteIntToPacket((int)this.State, CompressionMission.RangedSiegeWeaponStateCompressionInfo);
			GameNetworkMessage.WriteFloatToPacket(this.targetDirection, CompressionBasic.RadianCompressionInfo);
			GameNetworkMessage.WriteFloatToPacket(this.targetReleaseAngle, CompressionBasic.RadianCompressionInfo);
			GameNetworkMessage.WriteIntToPacket(this.AmmoCount, CompressionMission.RangedSiegeWeaponAmmoCompressionInfo);
			GameNetworkMessage.WriteIntToPacket(this._projectileIndex, CompressionMission.RangedSiegeWeaponAmmoIndexCompressionInfo);
		}

		// Token: 0x06002E15 RID: 11797 RVA: 0x000B666F File Offset: 0x000B486F
		protected virtual void UpdateProjectilePosition()
		{
		}

		// Token: 0x06002E16 RID: 11798 RVA: 0x000B6674 File Offset: 0x000B4874
		public override bool IsInRangeToCheckAlternativePoints(Agent agent)
		{
			float num = ((this.AmmoPickUpStandingPoints.Count > 0) ? (agent.GetInteractionDistanceToUsable(this.AmmoPickUpStandingPoints[0]) + 2f) : 2f);
			return this._ammoPickupCenter.DistanceSquared(agent.Position) < num * num;
		}

		// Token: 0x06002E17 RID: 11799 RVA: 0x000B66C8 File Offset: 0x000B48C8
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

		// Token: 0x06002E18 RID: 11800 RVA: 0x000B6794 File Offset: 0x000B4994
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
				Debug.FailedAssert("Invalid WeaponState.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Objects\\Siege\\RangedSiegeWeapon.cs", "OnRangedSiegeWeaponStateChange", 861);
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
					Debug.FailedAssert("Invalid WeaponState.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Objects\\Siege\\RangedSiegeWeapon.cs", "OnRangedSiegeWeaponStateChange", 934);
					break;
				}
			}
		}

		// Token: 0x06002E19 RID: 11801 RVA: 0x000B6AA8 File Offset: 0x000B4CA8
		protected virtual void SetActivationLoadAmmoPoint(bool activate)
		{
		}

		// Token: 0x06002E1A RID: 11802 RVA: 0x000B6AAC File Offset: 0x000B4CAC
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

		// Token: 0x06002E1B RID: 11803 RVA: 0x000B6CBC File Offset: 0x000B4EBC
		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			if (base.GameEntity.IsVisibleIncludeParents())
			{
				return ScriptComponentBehavior.TickRequirement.Tick | base.GetTickRequirement();
			}
			return base.GetTickRequirement();
		}

		// Token: 0x06002E1C RID: 11804 RVA: 0x000B6CDC File Offset: 0x000B4EDC
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

		// Token: 0x06002E1D RID: 11805 RVA: 0x000B6DA4 File Offset: 0x000B4FA4
		public void ToggleTrajectoryVisibility(bool isVisible)
		{
			if (isVisible)
			{
				this.VisualizeReleaseTrajectoryAngle = this.TopReleaseAngleRestriction;
				if (this._editorVisualizer == null)
				{
					this.VisualizeReleaseTrajectoryAngle = this.TopReleaseAngleRestriction;
					this._editorVisualizer = new TrajectoryVisualizer(base.GameEntity.Scene);
					this._editorVisualizer.Init(this.ProjectileEntityCurrentGlobalPosition, this.GetBallisticDirectionForVisualization() * this.ShootingSpeed, 6f, 200f);
				}
				this._editorVisualizer.SetVisible(true);
				this._editorVisualizer.Update(this.ProjectileEntityCurrentGlobalPosition, this.GetBallisticDirectionForVisualization() * this.ShootingSpeed, 6f, 200f, null);
				return;
			}
			if (this._editorVisualizer != null)
			{
				this._editorVisualizer.SetVisible(false);
				this._editorVisualizer = null;
			}
		}

		// Token: 0x06002E1E RID: 11806 RVA: 0x000B6E70 File Offset: 0x000B5070
		protected internal override void OnEditorTick(float dt)
		{
			if (MBEditor.IsEntitySelected(base.GameEntity))
			{
				if (this._editorVisualizer == null && this.RotationObject != null)
				{
					this._editorVisualizer = new TrajectoryVisualizer(base.GameEntity.Scene);
					this._editorVisualizer.Init(this.ProjectileEntityCurrentGlobalPosition, this.GetBallisticDirectionForVisualization() * this.ShootingSpeed, 6f, 200f);
				}
				if (this._editorVisualizer != null)
				{
					this._editorVisualizer.SetVisible(true);
					this._editorVisualizer.Update(this.ProjectileEntityCurrentGlobalPosition, this.GetBallisticDirectionForVisualization() * this.ShootingSpeed, 6f, 200f, null);
					return;
				}
			}
			else
			{
				TrajectoryVisualizer editorVisualizer = this._editorVisualizer;
				if (editorVisualizer == null)
				{
					return;
				}
				editorVisualizer.SetVisible(false);
			}
		}

		// Token: 0x06002E1F RID: 11807 RVA: 0x000B6F32 File Offset: 0x000B5132
		protected override void OnRemoved(int removeReason)
		{
			base.OnRemoved(removeReason);
			if (MBEditor.IsEditModeOn && this._editorVisualizer != null)
			{
				this._editorVisualizer.Clear();
				this._editorVisualizer = null;
			}
		}

		// Token: 0x06002E20 RID: 11808 RVA: 0x000B6F5C File Offset: 0x000B515C
		protected virtual float CalculateShootingRange(float heightDifference)
		{
			return Mission.GetMissileRange(this.ShootingSpeed, heightDifference);
		}

		// Token: 0x06002E21 RID: 11809 RVA: 0x000B6F6C File Offset: 0x000B516C
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

		// Token: 0x06002E22 RID: 11810 RVA: 0x000B6FE0 File Offset: 0x000B51E0
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
						GameNetwork.WriteMessage(new SetMachineRotation(this, this.currentDirection, this.currentReleaseAngle));
						GameNetwork.EndModuleEventAsClient();
					}
					if (GameNetwork.IsServerOrRecorder)
					{
						GameNetwork.BeginBroadcastModuleEvent();
						GameNetwork.WriteMessage(new SetMachineTargetRotation(this, this.currentDirection, this.currentReleaseAngle));
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

		// Token: 0x06002E23 RID: 11811 RVA: 0x000B7854 File Offset: 0x000B5A54
		public void GiveInput(float inputX, float inputY)
		{
			this._exactInputGiven = false;
			this._inputGiven = true;
			this._inputX = inputX;
			this._inputY = inputY;
			this._inputX = MBMath.ClampFloat(this._inputX, -1f, 1f);
			this._inputY = MBMath.ClampFloat(this._inputY, -1f, 1f);
		}

		// Token: 0x06002E24 RID: 11812 RVA: 0x000B78B3 File Offset: 0x000B5AB3
		public void GiveExactInput(float targetX, float targetY)
		{
			this._exactInputGiven = true;
			this._inputGiven = false;
			this._inputTargetX = MBMath.ClampAngle(targetX, 0f, this.DirectionRestriction);
			this._inputTargetY = MBMath.ClampAngle(targetY, this.ReleaseAngleRestrictionCenter, this.ReleaseAngleRestrictionAngle);
		}

		// Token: 0x06002E25 RID: 11813 RVA: 0x000B78F2 File Offset: 0x000B5AF2
		protected virtual bool CanRotate()
		{
			return this.State == RangedSiegeWeapon.WeaponState.Idle;
		}

		// Token: 0x06002E26 RID: 11814 RVA: 0x000B78FD File Offset: 0x000B5AFD
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

		// Token: 0x06002E27 RID: 11815 RVA: 0x000B7928 File Offset: 0x000B5B28
		protected virtual void ApplyCurrentDirectionToEntity()
		{
			MatrixFrame rotationObjectInitialFrame = this._rotationObjectInitialFrame;
			rotationObjectInitialFrame.rotation.RotateAboutUp(this.currentDirection);
			this.RotationObject.GameEntity.SetFrame(ref rotationObjectInitialFrame);
			this.RotationObject.GameEntity.RecomputeBoundingBox();
		}

		// Token: 0x06002E28 RID: 11816 RVA: 0x000B7970 File Offset: 0x000B5B70
		public virtual float GetTargetDirection(Vec3 target)
		{
			MatrixFrame globalFrame = base.GameEntity.GetGlobalFrame();
			globalFrame.rotation.RotateAboutUp(3.1415927f);
			return globalFrame.TransformToLocal(target).AsVec2.RotationInRadians;
		}

		// Token: 0x06002E29 RID: 11817 RVA: 0x000B79B4 File Offset: 0x000B5BB4
		public virtual float GetTargetReleaseAngle(Vec3 target)
		{
			return Mission.GetMissileVerticalAimCorrection(target - this.MissleStartingPositionForSimulation, this.ShootingSpeed, ref this._originalMissileWeaponStatsDataForTargeting, ItemObject.GetAirFrictionConstant(this.OriginalMissileItem.PrimaryWeapon.WeaponClass, this.OriginalMissileItem.PrimaryWeapon.WeaponFlags));
		}

		// Token: 0x06002E2A RID: 11818 RVA: 0x000B7A04 File Offset: 0x000B5C04
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

		// Token: 0x06002E2B RID: 11819 RVA: 0x000B7ABC File Offset: 0x000B5CBC
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

		// Token: 0x06002E2C RID: 11820 RVA: 0x000B7B38 File Offset: 0x000B5D38
		protected void OnLoadingAmmoPointUsingCancelled(Agent agent, bool isCanceledBecauseOfAnimation)
		{
			MBDebug.Print("(DUMP-372) 1", 0, Debug.DebugColor.White, 17592186044416UL);
			if (agent.IsAIControlled)
			{
				MBDebug.Print("(DUMP-372) 2", 0, Debug.DebugColor.White, 17592186044416UL);
				if (isCanceledBecauseOfAnimation)
				{
					MBDebug.Print("(DUMP-372) 3", 0, Debug.DebugColor.White, 17592186044416UL);
					this.SendAgentToAmmoPickup(agent);
					MBDebug.Print("(DUMP-372) 4", 0, Debug.DebugColor.White, 17592186044416UL);
					return;
				}
				MBDebug.Print("(DUMP-372) 5", 0, Debug.DebugColor.White, 17592186044416UL);
				this.SendReloaderAgentToOriginalPoint();
				MBDebug.Print("(DUMP-372) 6", 0, Debug.DebugColor.White, 17592186044416UL);
			}
		}

		// Token: 0x06002E2D RID: 11821 RVA: 0x000B7BE2 File Offset: 0x000B5DE2
		protected void OnAmmoPickupUsingCancelled(Agent agent, bool isCanceledBecauseOfAnimation)
		{
			if (agent.IsAIControlled)
			{
				this.SendAgentToAmmoPickup(agent);
			}
		}

		// Token: 0x06002E2E RID: 11822 RVA: 0x000B7BF4 File Offset: 0x000B5DF4
		protected void SendAgentToAmmoPickup(Agent agent)
		{
			MBDebug.Print("(DUMP-372) 10", 0, Debug.DebugColor.White, 17592186044416UL);
			this.ReloaderAgent = agent;
			MBDebug.Print("(DUMP-372) 11", 0, Debug.DebugColor.White, 17592186044416UL);
			EquipmentIndex wieldedItemIndex = agent.GetWieldedItemIndex(Agent.HandIndex.MainHand);
			MBDebug.Print("(DUMP-372) 12", 0, Debug.DebugColor.White, 17592186044416UL);
			if (wieldedItemIndex != EquipmentIndex.None && agent.Equipment[wieldedItemIndex].CurrentUsageItem.WeaponClass == this.OriginalMissileItem.PrimaryWeapon.WeaponClass)
			{
				MBDebug.Print("(DUMP-372) 13", 0, Debug.DebugColor.White, 17592186044416UL);
				agent.AIMoveToGameObjectEnable(this.LoadAmmoStandingPoint, this, base.Ai.GetScriptedFrameFlags(agent));
				MBDebug.Print("(DUMP-372) 14", 0, Debug.DebugColor.White, 17592186044416UL);
				return;
			}
			agent.AIMoveToGameObjectEnable(base.AmmoPickUpPoints.First((StandingPoint x) => !x.HasUser), this, base.Ai.GetScriptedFrameFlags(agent));
			MBDebug.Print("(DUMP-372) 15", 0, Debug.DebugColor.White, 17592186044416UL);
		}

		// Token: 0x06002E2F RID: 11823 RVA: 0x000B7D1C File Offset: 0x000B5F1C
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

		// Token: 0x06002E30 RID: 11824 RVA: 0x000B7DE4 File Offset: 0x000B5FE4
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
				goto IL_400;
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
				Debug.FailedAssert("Invalid WeaponState.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Objects\\Siege\\RangedSiegeWeapon.cs", "UpdateState", 1974);
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
			IL_400:
			this.animationTimeElapsed += dt;
			if (this.animationTimeElapsed >= this.timeGapBetweenShootActionAndProjectileLeaving)
			{
				this.State = RangedSiegeWeapon.WeaponState.Shooting;
				return;
			}
		}

		// Token: 0x06002E31 RID: 11825 RVA: 0x000B82D0 File Offset: 0x000B64D0
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

		// Token: 0x06002E32 RID: 11826 RVA: 0x000B8302 File Offset: 0x000B6502
		public void ManualReload()
		{
			if (this.AttackClickWillReload)
			{
				this.State = RangedSiegeWeapon.WeaponState.WaitingBeforeReloading;
			}
		}

		// Token: 0x06002E33 RID: 11827 RVA: 0x000B8313 File Offset: 0x000B6513
		public void AiRequestsShoot()
		{
			this._aiRequestsShoot = true;
		}

		// Token: 0x06002E34 RID: 11828 RVA: 0x000B831C File Offset: 0x000B651C
		public void AiRequestsManualReload()
		{
			this._aiRequestsManualReload = true;
		}

		// Token: 0x06002E35 RID: 11829 RVA: 0x000B8328 File Offset: 0x000B6528
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

		// Token: 0x06002E36 RID: 11830 RVA: 0x000B8390 File Offset: 0x000B6590
		private Vec3 GetBallisticDirectionForVisualization()
		{
			Mat3 mat = new Mat3
			{
				f = this.VisualizationShootingDirection,
				u = Vec3.Up
			};
			mat.Orthonormalize();
			return mat.f;
		}

		// Token: 0x06002E37 RID: 11831 RVA: 0x000B83D0 File Offset: 0x000B65D0
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

		// Token: 0x06002E38 RID: 11832 RVA: 0x000B8434 File Offset: 0x000B6634
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

		// Token: 0x17000851 RID: 2129
		// (get) Token: 0x06002E39 RID: 11833 RVA: 0x000B84E9 File Offset: 0x000B66E9
		protected virtual Vec3 ShootingDirection
		{
			get
			{
				return this.Projectile.GameEntity.GetGlobalFrame().rotation.u;
			}
		}

		// Token: 0x17000852 RID: 2130
		// (get) Token: 0x06002E3A RID: 11834 RVA: 0x000B8505 File Offset: 0x000B6705
		protected virtual Vec3 VisualizationShootingDirection
		{
			get
			{
				return this.Projectile.GameEntity.GetGlobalFrame().rotation.u;
			}
		}

		// Token: 0x17000853 RID: 2131
		// (get) Token: 0x06002E3B RID: 11835 RVA: 0x000B8521 File Offset: 0x000B6721
		public virtual Vec3 ProjectileEntityCurrentGlobalPosition
		{
			get
			{
				return this.Projectile.GameEntity.GetGlobalFrame().origin;
			}
		}

		// Token: 0x06002E3C RID: 11836 RVA: 0x000B8538 File Offset: 0x000B6738
		protected void OnRotationStarted()
		{
			if (this.MoveSound == null || !this.MoveSound.IsValid)
			{
				this.MoveSound = SoundEvent.CreateEvent(this.MoveSoundIndex, base.Scene);
				this.MoveSound.PlayInPosition(this.RotationObject.GameEntity.GlobalPosition);
			}
		}

		// Token: 0x06002E3D RID: 11837 RVA: 0x000B858D File Offset: 0x000B678D
		protected void OnRotationStopped()
		{
			this.MoveSound.Stop();
			this.MoveSound = null;
		}

		// Token: 0x06002E3E RID: 11838
		public abstract override SiegeEngineType GetSiegeEngineType();

		// Token: 0x17000854 RID: 2132
		// (get) Token: 0x06002E3F RID: 11839 RVA: 0x000B85A1 File Offset: 0x000B67A1
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

		// Token: 0x06002E40 RID: 11840 RVA: 0x000B85C4 File Offset: 0x000B67C4
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

		// Token: 0x06002E41 RID: 11841 RVA: 0x000B8624 File Offset: 0x000B6824
		public bool CanShootAtBoxSimplified(Vec3 boxMin, Vec3 boxMax)
		{
			Vec3 vec = (boxMin + boxMax) / 2f;
			Vec3 vec2 = vec;
			vec2.z = boxMax.z;
			return this.CanShootAtPoint(vec) || this.CanShootAtPoint(vec2);
		}

		// Token: 0x06002E42 RID: 11842 RVA: 0x000B8664 File Offset: 0x000B6864
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

		// Token: 0x06002E43 RID: 11843 RVA: 0x000B86F0 File Offset: 0x000B68F0
		public Vec3 GetEstimatedTargetMovementVector(Vec3 targetCurrentPosition, Vec3 targetVelocity)
		{
			if (targetVelocity != Vec3.Zero)
			{
				return targetVelocity * ((base.GameEntity.GlobalPosition - targetCurrentPosition).Length / this.ShootingSpeed + this.timeGapBetweenShootActionAndProjectileLeaving);
			}
			return Vec3.Zero;
		}

		// Token: 0x06002E44 RID: 11844 RVA: 0x000B8740 File Offset: 0x000B6940
		public bool CanShootAtAgent(Agent agent)
		{
			Vec3 boxMax = agent.CollisionCapsule.GetBoxMax();
			Vec3 vec = (agent.CollisionCapsule.GetBoxMin() + boxMax) / 2f;
			return this.CanShootAtPoint(vec);
		}

		// Token: 0x06002E45 RID: 11845 RVA: 0x000B8784 File Offset: 0x000B6984
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
						Vec3 vec = (siegeWeapon.GameEntity.GlobalBoxMin + siegeWeapon.GameEntity.GlobalBoxMax) / 2f;
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

		// Token: 0x06002E46 RID: 11846 RVA: 0x000B891C File Offset: 0x000B6B1C
		protected internal virtual bool IsTargetValid(ITargetable target)
		{
			return true;
		}

		// Token: 0x06002E47 RID: 11847 RVA: 0x000B891F File Offset: 0x000B6B1F
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

		// Token: 0x06002E48 RID: 11848 RVA: 0x000B8939 File Offset: 0x000B6B39
		protected override GameEntity GetEntityToAttachNavMeshFaces()
		{
			return this.RotationObject.GameEntity;
		}

		// Token: 0x06002E49 RID: 11849
		public abstract float ProcessTargetValue(float baseValue, TargetFlags flags);

		// Token: 0x06002E4A RID: 11850 RVA: 0x000B8948 File Offset: 0x000B6B48
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

		// Token: 0x0400123F RID: 4671
		public const float DefaultDirectionRestriction = 2.0943952f;

		// Token: 0x04001240 RID: 4672
		public const string MultipleProjectileId = "grapeshot_fire_stack";

		// Token: 0x04001241 RID: 4673
		public const string MultipleProjectileFlyingId = "grapeshot_fire_projectile";

		// Token: 0x04001242 RID: 4674
		public const int MultipleProjectileCount = 5;

		// Token: 0x04001243 RID: 4675
		public const string CanGoAmmoPickupTag = "can_pick_up_ammo";

		// Token: 0x04001244 RID: 4676
		public const string DontApplySidePenaltyTag = "no_ammo_pick_up_penalty";

		// Token: 0x04001245 RID: 4677
		public const string ReloadTag = "reload";

		// Token: 0x04001246 RID: 4678
		public const string AmmoLoadTag = "ammoload";

		// Token: 0x04001247 RID: 4679
		public const string CameraHolderTag = "cameraHolder";

		// Token: 0x04001248 RID: 4680
		public const string ProjectileTag = "projectile";

		// Token: 0x04001249 RID: 4681
		public string MissileItemID;

		// Token: 0x0400124A RID: 4682
		protected bool UsesMouseForAiming;

		// Token: 0x0400124B RID: 4683
		private RangedSiegeWeapon.WeaponState _state;

		// Token: 0x0400124C RID: 4684
		public RangedSiegeWeapon.FiringFocus Focus;

		// Token: 0x0400124F RID: 4687
		private int _projectileIndex;

		// Token: 0x04001250 RID: 4688
		protected GameEntity MissileStartingPositionEntityForSimulation;

		// Token: 0x04001251 RID: 4689
		protected Skeleton[] Skeletons;

		// Token: 0x04001252 RID: 4690
		protected SynchedMissionObject[] SkeletonOwnerObjects;

		// Token: 0x04001253 RID: 4691
		protected string[] SkeletonNames;

		// Token: 0x04001254 RID: 4692
		protected string[] FireAnimations;

		// Token: 0x04001255 RID: 4693
		protected string[] SetUpAnimations;

		// Token: 0x04001256 RID: 4694
		protected int[] FireAnimationIndices;

		// Token: 0x04001257 RID: 4695
		protected int[] SetUpAnimationIndices;

		// Token: 0x04001258 RID: 4696
		protected SynchedMissionObject RotationObject;

		// Token: 0x04001259 RID: 4697
		private MatrixFrame _rotationObjectInitialFrame;

		// Token: 0x0400125A RID: 4698
		protected SoundEvent MoveSound;

		// Token: 0x0400125B RID: 4699
		protected SoundEvent ReloadSound;

		// Token: 0x0400125C RID: 4700
		protected int MoveSoundIndex = -1;

		// Token: 0x0400125D RID: 4701
		protected int ReloadSoundIndex = -1;

		// Token: 0x0400125E RID: 4702
		protected int ReloadEndSoundIndex = -1;

		// Token: 0x0400125F RID: 4703
		protected ItemObject OriginalMissileItem;

		// Token: 0x04001260 RID: 4704
		private WeaponStatsData _originalMissileWeaponStatsDataForTargeting;

		// Token: 0x04001261 RID: 4705
		protected ItemObject LoadedMissileItem;

		// Token: 0x04001262 RID: 4706
		protected List<StandingPoint> CanPickUpAmmoStandingPoints;

		// Token: 0x04001263 RID: 4707
		protected List<StandingPoint> ReloadStandingPoints;

		// Token: 0x04001264 RID: 4708
		protected List<StandingPointWithWeaponRequirement> AmmoPickUpStandingPoints;

		// Token: 0x04001265 RID: 4709
		protected StandingPointWithWeaponRequirement LoadAmmoStandingPoint;

		// Token: 0x04001266 RID: 4710
		protected Dictionary<StandingPoint, float> PilotReservePriorityValues = new Dictionary<StandingPoint, float>();

		// Token: 0x04001267 RID: 4711
		protected Agent ReloaderAgent;

		// Token: 0x04001268 RID: 4712
		protected StandingPoint ReloaderAgentOriginalPoint;

		// Token: 0x0400126A RID: 4714
		protected bool AttackClickWillReload;

		// Token: 0x0400126B RID: 4715
		protected bool WeaponNeedsClickToReload;

		// Token: 0x0400126C RID: 4716
		public int startingAmmoCount = 20;

		// Token: 0x0400126D RID: 4717
		protected int CurrentAmmo = 1;

		// Token: 0x0400126E RID: 4718
		private bool _hasAmmo = true;

		// Token: 0x0400126F RID: 4719
		protected float targetDirection;

		// Token: 0x04001270 RID: 4720
		protected float targetReleaseAngle;

		// Token: 0x04001271 RID: 4721
		protected float cameraDirection;

		// Token: 0x04001272 RID: 4722
		protected float cameraReleaseAngle;

		// Token: 0x04001273 RID: 4723
		protected float reloadTargetReleaseAngle;

		// Token: 0x04001274 RID: 4724
		protected float maxRotateSpeed;

		// Token: 0x04001275 RID: 4725
		protected float dontMoveTimer;

		// Token: 0x04001276 RID: 4726
		private MatrixFrame cameraHolderInitialFrame;

		// Token: 0x04001277 RID: 4727
		private RangedSiegeWeapon.CameraState cameraState;

		// Token: 0x04001278 RID: 4728
		private bool _inputGiven;

		// Token: 0x04001279 RID: 4729
		private float _inputX;

		// Token: 0x0400127A RID: 4730
		private float _inputY;

		// Token: 0x0400127B RID: 4731
		private bool _exactInputGiven;

		// Token: 0x0400127C RID: 4732
		private float _inputTargetX;

		// Token: 0x0400127D RID: 4733
		private float _inputTargetY;

		// Token: 0x0400127E RID: 4734
		private Vec3 _ammoPickupCenter;

		// Token: 0x0400127F RID: 4735
		protected float currentDirection;

		// Token: 0x04001280 RID: 4736
		private Vec3 _originalDirection;

		// Token: 0x04001281 RID: 4737
		protected float currentReleaseAngle;

		// Token: 0x04001282 RID: 4738
		private float _lastSyncedDirection;

		// Token: 0x04001283 RID: 4739
		private float _lastSyncedReleaseAngle;

		// Token: 0x04001284 RID: 4740
		private float _syncTimer;

		// Token: 0x04001285 RID: 4741
		public float TopReleaseAngleRestriction = 1.5707964f;

		// Token: 0x04001286 RID: 4742
		public float BottomReleaseAngleRestriction = -1.5707964f;

		// Token: 0x04001287 RID: 4743
		protected float ReleaseAngleRestrictionCenter;

		// Token: 0x04001288 RID: 4744
		protected float ReleaseAngleRestrictionAngle;

		// Token: 0x04001289 RID: 4745
		private float animationTimeElapsed;

		// Token: 0x0400128A RID: 4746
		protected float timeGapBetweenShootingEndAndReloadingStart = 0.6f;

		// Token: 0x0400128B RID: 4747
		protected float timeGapBetweenShootActionAndProjectileLeaving;

		// Token: 0x0400128C RID: 4748
		private int _currentReloaderCount;

		// Token: 0x0400128D RID: 4749
		private Agent _lastShooterAgent;

		// Token: 0x0400128E RID: 4750
		protected BattleSideEnum _defaultSide;

		// Token: 0x0400128F RID: 4751
		private bool hasFrameChangedInPreviousFrame;

		// Token: 0x04001290 RID: 4752
		public float VisualizeReleaseTrajectoryAngle;

		// Token: 0x04001291 RID: 4753
		private TrajectoryVisualizer _editorVisualizer;

		// Token: 0x04001292 RID: 4754
		protected SiegeMachineStonePile _stonePile;

		// Token: 0x04001293 RID: 4755
		private bool _aiRequestsShoot;

		// Token: 0x04001294 RID: 4756
		private bool _aiRequestsManualReload;

		// Token: 0x02000661 RID: 1633
		public enum WeaponState
		{
			// Token: 0x040020AC RID: 8364
			Invalid = -1,
			// Token: 0x040020AD RID: 8365
			Idle,
			// Token: 0x040020AE RID: 8366
			WaitingBeforeProjectileLeaving,
			// Token: 0x040020AF RID: 8367
			Shooting,
			// Token: 0x040020B0 RID: 8368
			WaitingAfterShooting,
			// Token: 0x040020B1 RID: 8369
			WaitingBeforeReloading,
			// Token: 0x040020B2 RID: 8370
			LoadingAmmo,
			// Token: 0x040020B3 RID: 8371
			WaitingBeforeIdle,
			// Token: 0x040020B4 RID: 8372
			Reloading,
			// Token: 0x040020B5 RID: 8373
			ReloadingPaused,
			// Token: 0x040020B6 RID: 8374
			NumberOfStates
		}

		// Token: 0x02000662 RID: 1634
		public enum FiringFocus
		{
			// Token: 0x040020B8 RID: 8376
			Troops,
			// Token: 0x040020B9 RID: 8377
			Walls,
			// Token: 0x040020BA RID: 8378
			RangedSiegeWeapons,
			// Token: 0x040020BB RID: 8379
			PrimarySiegeWeapons
		}

		// Token: 0x02000663 RID: 1635
		// (Invoke) Token: 0x06003E7B RID: 15995
		public delegate void OnSiegeWeaponReloadDone();

		// Token: 0x02000664 RID: 1636
		public enum CameraState
		{
			// Token: 0x040020BD RID: 8381
			StickToWeapon,
			// Token: 0x040020BE RID: 8382
			DontMove,
			// Token: 0x040020BF RID: 8383
			MoveDownToReload,
			// Token: 0x040020C0 RID: 8384
			RememberLastShotDirection,
			// Token: 0x040020C1 RID: 8385
			FreeMove,
			// Token: 0x040020C2 RID: 8386
			ApproachToCamera
		}
	}
}
