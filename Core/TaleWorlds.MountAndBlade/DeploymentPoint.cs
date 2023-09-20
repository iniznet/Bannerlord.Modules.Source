using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Missions;
using TaleWorlds.MountAndBlade.Objects.Siege;
using TaleWorlds.MountAndBlade.Objects.Usables;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200034B RID: 843
	public class DeploymentPoint : SynchedMissionObject
	{
		// Token: 0x1400008E RID: 142
		// (add) Token: 0x06002D35 RID: 11573 RVA: 0x000B0DD0 File Offset: 0x000AEFD0
		// (remove) Token: 0x06002D36 RID: 11574 RVA: 0x000B0E08 File Offset: 0x000AF008
		public event Action<DeploymentPoint, SynchedMissionObject> OnDeploymentStateChanged;

		// Token: 0x1400008F RID: 143
		// (add) Token: 0x06002D37 RID: 11575 RVA: 0x000B0E40 File Offset: 0x000AF040
		// (remove) Token: 0x06002D38 RID: 11576 RVA: 0x000B0E78 File Offset: 0x000AF078
		public event Action<DeploymentPoint> OnDeploymentPointTypeDetermined;

		// Token: 0x1700081C RID: 2076
		// (get) Token: 0x06002D39 RID: 11577 RVA: 0x000B0EAD File Offset: 0x000AF0AD
		// (set) Token: 0x06002D3A RID: 11578 RVA: 0x000B0EB5 File Offset: 0x000AF0B5
		public Vec3 DeploymentTargetPosition { get; private set; }

		// Token: 0x1700081D RID: 2077
		// (get) Token: 0x06002D3B RID: 11579 RVA: 0x000B0EBE File Offset: 0x000AF0BE
		// (set) Token: 0x06002D3C RID: 11580 RVA: 0x000B0EC6 File Offset: 0x000AF0C6
		public WallSegment AssociatedWallSegment { get; private set; }

		// Token: 0x1700081E RID: 2078
		// (get) Token: 0x06002D3D RID: 11581 RVA: 0x000B0ECF File Offset: 0x000AF0CF
		public IEnumerable<SynchedMissionObject> DeployableWeapons
		{
			get
			{
				return this._weapons.Where((SynchedMissionObject w) => !w.IsDisabled);
			}
		}

		// Token: 0x1700081F RID: 2079
		// (get) Token: 0x06002D3E RID: 11582 RVA: 0x000B0EFB File Offset: 0x000AF0FB
		public bool IsDeployed
		{
			get
			{
				return this.DeployedWeapon != null;
			}
		}

		// Token: 0x17000820 RID: 2080
		// (get) Token: 0x06002D3F RID: 11583 RVA: 0x000B0F06 File Offset: 0x000AF106
		// (set) Token: 0x06002D40 RID: 11584 RVA: 0x000B0F0E File Offset: 0x000AF10E
		public SynchedMissionObject DeployedWeapon { get; private set; }

		// Token: 0x17000821 RID: 2081
		// (get) Token: 0x06002D41 RID: 11585 RVA: 0x000B0F17 File Offset: 0x000AF117
		// (set) Token: 0x06002D42 RID: 11586 RVA: 0x000B0F1F File Offset: 0x000AF11F
		public SynchedMissionObject DisbandedWeapon { get; private set; }

		// Token: 0x06002D43 RID: 11587 RVA: 0x000B0F28 File Offset: 0x000AF128
		protected internal override void OnInit()
		{
			this._weapons = new List<SynchedMissionObject>();
		}

		// Token: 0x06002D44 RID: 11588 RVA: 0x000B0F38 File Offset: 0x000AF138
		public override void AfterMissionStart()
		{
			base.OnInit();
			if (!GameNetwork.IsClientOrReplay)
			{
				this._weapons = this.GetWeaponsUnder();
				this._associatedSiegeLadders = new List<SiegeLadder>();
				if (this.DeployableWeapons.IsEmpty<SynchedMissionObject>())
				{
					this.SetVisibleSynched(false, false);
					this.SetBreachSideDeploymentPoint();
				}
				base.AfterMissionStart();
				if (!GameNetwork.IsClientOrReplay)
				{
					this.DetermineDeploymentPointType();
				}
				this.HideAllWeapons();
			}
		}

		// Token: 0x06002D45 RID: 11589 RVA: 0x000B0FA0 File Offset: 0x000AF1A0
		private void SetBreachSideDeploymentPoint()
		{
			Debug.Print("Deployment point " + ((base.GameEntity != null) ? ("upgrade level mask " + base.GameEntity.GetUpgradeLevelMask().ToString()) : "no game entity.") + "\n", 0, Debug.DebugColor.White, 17592186044416UL);
			this._isBreachSideDeploymentPoint = true;
			this._deploymentPointType = DeploymentPoint.DeploymentPointType.Breach;
			FormationAI.BehaviorSide deploymentPointSide = (this._weapons.FirstOrDefault((SynchedMissionObject w) => w is SiegeTower) as IPrimarySiegeWeapon).WeaponSide;
			this.AssociatedWallSegment = Mission.Current.ActiveMissionObjects.FindAllWithType<WallSegment>().FirstOrDefault((WallSegment ws) => ws.DefenseSide == deploymentPointSide);
			this.DeploymentTargetPosition = this.AssociatedWallSegment.GameEntity.GlobalPosition;
		}

		// Token: 0x06002D46 RID: 11590 RVA: 0x000B108F File Offset: 0x000AF28F
		public Vec3 GetDeploymentOrigin()
		{
			return base.GameEntity.GlobalPosition;
		}

		// Token: 0x06002D47 RID: 11591 RVA: 0x000B109C File Offset: 0x000AF29C
		public DeploymentPoint.DeploymentPointState GetDeploymentPointState()
		{
			switch (this._deploymentPointType)
			{
			case DeploymentPoint.DeploymentPointType.BatteringRam:
				if (!this.IsDeployed)
				{
					return DeploymentPoint.DeploymentPointState.NotDeployed;
				}
				return DeploymentPoint.DeploymentPointState.BatteringRam;
			case DeploymentPoint.DeploymentPointType.TowerLadder:
				if (!this.IsDeployed)
				{
					return DeploymentPoint.DeploymentPointState.SiegeLadder;
				}
				return DeploymentPoint.DeploymentPointState.SiegeTower;
			case DeploymentPoint.DeploymentPointType.Breach:
				return DeploymentPoint.DeploymentPointState.Breach;
			case DeploymentPoint.DeploymentPointType.Ranged:
				if (!this.IsDeployed)
				{
					return DeploymentPoint.DeploymentPointState.NotDeployed;
				}
				return DeploymentPoint.DeploymentPointState.Ranged;
			default:
				MBDebug.ShowWarning("Undefined deployment point type fetched.");
				return DeploymentPoint.DeploymentPointState.NotDeployed;
			}
		}

		// Token: 0x06002D48 RID: 11592 RVA: 0x000B10F9 File Offset: 0x000AF2F9
		public DeploymentPoint.DeploymentPointType GetDeploymentPointType()
		{
			return this._deploymentPointType;
		}

		// Token: 0x06002D49 RID: 11593 RVA: 0x000B1101 File Offset: 0x000AF301
		public List<SiegeLadder> GetAssociatedSiegeLadders()
		{
			return this._associatedSiegeLadders;
		}

		// Token: 0x06002D4A RID: 11594 RVA: 0x000B110C File Offset: 0x000AF30C
		private void DetermineDeploymentPointType()
		{
			if (this._isBreachSideDeploymentPoint)
			{
				this._deploymentPointType = DeploymentPoint.DeploymentPointType.Breach;
			}
			else if (this._weapons.Any((SynchedMissionObject w) => w is BatteringRam))
			{
				this._deploymentPointType = DeploymentPoint.DeploymentPointType.BatteringRam;
				this.DeploymentTargetPosition = (this._weapons.First((SynchedMissionObject w) => w is BatteringRam) as IPrimarySiegeWeapon).TargetCastlePosition.GameEntity.GlobalPosition;
			}
			else if (this._weapons.Any((SynchedMissionObject w) => w is SiegeTower))
			{
				SiegeTower tower = this._weapons.FirstOrDefault((SynchedMissionObject w) => w is SiegeTower) as SiegeTower;
				this._deploymentPointType = DeploymentPoint.DeploymentPointType.TowerLadder;
				this.DeploymentTargetPosition = tower.TargetCastlePosition.GameEntity.GlobalPosition;
				this._associatedSiegeLadders = (from sl in Mission.Current.ActiveMissionObjects.FindAllWithType<SiegeLadder>()
					where sl.WeaponSide == tower.WeaponSide
					select sl).ToList<SiegeLadder>();
			}
			else
			{
				this._deploymentPointType = DeploymentPoint.DeploymentPointType.Ranged;
				this.DeploymentTargetPosition = Vec3.Invalid;
			}
			Action<DeploymentPoint> onDeploymentPointTypeDetermined = this.OnDeploymentPointTypeDetermined;
			if (onDeploymentPointTypeDetermined == null)
			{
				return;
			}
			onDeploymentPointTypeDetermined(this);
		}

		// Token: 0x06002D4B RID: 11595 RVA: 0x000B1288 File Offset: 0x000AF488
		public List<SynchedMissionObject> GetWeaponsUnder()
		{
			TeamAISiegeComponent teamAISiegeComponent;
			List<SiegeWeapon> list;
			if ((teamAISiegeComponent = Mission.Current.Teams[0].TeamAI as TeamAISiegeComponent) != null)
			{
				list = teamAISiegeComponent.SceneSiegeWeapons;
			}
			else
			{
				List<GameEntity> list2 = new List<GameEntity>();
				base.GameEntity.Scene.GetEntities(ref list2);
				list = (from se in list2
					where se.HasScriptOfType<SiegeWeapon>()
					select se.GetScriptComponents<SiegeWeapon>().FirstOrDefault<SiegeWeapon>()).ToList<SiegeWeapon>();
			}
			IEnumerable<SynchedMissionObject> enumerable = from ssw in list
				where ssw.GameEntity.HasTag(this.SiegeWeaponTag) || (ssw.GameEntity.Parent != null && ssw.GameEntity.Parent.HasTag(this.SiegeWeaponTag))
				select (ssw);
			Vec3 globalPosition = base.GameEntity.GlobalPosition;
			float radiusSquared = this.Radius * this.Radius;
			IEnumerable<SynchedMissionObject> enumerable2 = from ssw in list
				where ssw.GameEntity != this.GameEntity && ssw.GameEntity.GlobalPosition.DistanceSquared(globalPosition) < radiusSquared
				select (ssw);
			return enumerable.Concat(enumerable2).Distinct<SynchedMissionObject>().ToList<SynchedMissionObject>();
		}

		// Token: 0x06002D4C RID: 11596 RVA: 0x000B13D4 File Offset: 0x000AF5D4
		public IEnumerable<SpawnerBase> GetSpawnersForEditor()
		{
			List<GameEntity> list = new List<GameEntity>();
			base.GameEntity.Scene.GetEntities(ref list);
			IEnumerable<SpawnerBase> enumerable = from se in list
				where se.HasScriptOfType<SpawnerBase>()
				select se.GetScriptComponents<SpawnerBase>().FirstOrDefault<SpawnerBase>();
			IEnumerable<SpawnerBase> enumerable2 = from ssw in enumerable
				where ssw.GameEntity.HasTag(this.SiegeWeaponTag)
				select (ssw);
			Vec3 globalPosition = base.GameEntity.GlobalPosition;
			float radiusSquared = this.Radius * this.Radius;
			IEnumerable<SpawnerBase> enumerable3 = from ssw in enumerable
				where ssw.GameEntity != this.GameEntity && ssw.GameEntity.GlobalPosition.DistanceSquared(globalPosition) < radiusSquared
				select (ssw);
			return enumerable2.Concat(enumerable3).Distinct<SpawnerBase>();
		}

		// Token: 0x06002D4D RID: 11597 RVA: 0x000B14EC File Offset: 0x000AF6EC
		protected internal override void OnEditorInit()
		{
			base.OnEditorInit();
			this._weapons = null;
		}

		// Token: 0x06002D4E RID: 11598 RVA: 0x000B14FC File Offset: 0x000AF6FC
		protected internal override void OnEditorTick(float dt)
		{
			base.OnEditorTick(dt);
			foreach (GameEntity gameEntity in this._highlightedEntites)
			{
				gameEntity.SetContourColor(null, true);
			}
			this._highlightedEntites.Clear();
			if (MBEditor.IsEntitySelected(base.GameEntity))
			{
				uint num = 4294901760U;
				if (this.Radius > 0f)
				{
					DebugExtensions.RenderDebugCircleOnTerrain(base.Scene, base.GameEntity.GetGlobalFrame(), this.Radius, num, true, false);
				}
				foreach (SpawnerBase spawnerBase in this.GetSpawnersForEditor())
				{
					spawnerBase.GameEntity.SetContourColor(new uint?(num), true);
					this._highlightedEntites.Add(spawnerBase.GameEntity);
				}
			}
		}

		// Token: 0x06002D4F RID: 11599 RVA: 0x000B1608 File Offset: 0x000AF808
		private void OnDeploymentStateChangedAux(SynchedMissionObject targetObject)
		{
			if (this.IsDeployed)
			{
				targetObject.SetVisibleSynched(true, false);
				targetObject.SetPhysicsStateSynched(true, true);
			}
			else
			{
				targetObject.SetVisibleSynched(false, false);
				targetObject.SetPhysicsStateSynched(false, true);
			}
			Action<DeploymentPoint, SynchedMissionObject> onDeploymentStateChanged = this.OnDeploymentStateChanged;
			if (onDeploymentStateChanged != null)
			{
				onDeploymentStateChanged(this, targetObject);
			}
			SiegeWeapon siegeWeapon;
			if ((siegeWeapon = targetObject as SiegeWeapon) != null)
			{
				siegeWeapon.OnDeploymentStateChanged(this.IsDeployed);
			}
		}

		// Token: 0x06002D50 RID: 11600 RVA: 0x000B1668 File Offset: 0x000AF868
		public SiegeMachineStonePile GetStonePileOfWeapon(SynchedMissionObject weapon)
		{
			if (weapon is SiegeWeapon)
			{
				foreach (GameEntity gameEntity in weapon.GameEntity.Parent.GetChildren())
				{
					SiegeMachineStonePile firstScriptOfType = gameEntity.GetFirstScriptOfType<SiegeMachineStonePile>();
					if (firstScriptOfType != null)
					{
						return firstScriptOfType;
					}
				}
			}
			return null;
		}

		// Token: 0x06002D51 RID: 11601 RVA: 0x000B16D0 File Offset: 0x000AF8D0
		public void Deploy(Type t)
		{
			this.DeployedWeapon = this._weapons.First((SynchedMissionObject w) => MissionSiegeWeaponsController.GetWeaponType(w) == t);
			this.OnDeploymentStateChangedAux(this.DeployedWeapon);
			this.ToggleDeploymentPointVisibility(false);
			this.ToggleDeployedWeaponVisibility(true);
		}

		// Token: 0x06002D52 RID: 11602 RVA: 0x000B1721 File Offset: 0x000AF921
		public void Deploy(SiegeWeapon s)
		{
			this.DeployedWeapon = s;
			this.DisbandedWeapon = null;
			this.OnDeploymentStateChangedAux(s);
			this.ToggleDeploymentPointVisibility(false);
			this.ToggleDeployedWeaponVisibility(true);
		}

		// Token: 0x06002D53 RID: 11603 RVA: 0x000B1746 File Offset: 0x000AF946
		public ScriptComponentBehavior Disband()
		{
			this.ToggleDeploymentPointVisibility(true);
			this.ToggleDeployedWeaponVisibility(false);
			this.DisbandedWeapon = this.DeployedWeapon;
			this.DeployedWeapon = null;
			this.OnDeploymentStateChangedAux(this.DisbandedWeapon);
			return this.DisbandedWeapon;
		}

		// Token: 0x17000822 RID: 2082
		// (get) Token: 0x06002D54 RID: 11604 RVA: 0x000B177B File Offset: 0x000AF97B
		public IEnumerable<Type> DeployableWeaponTypes
		{
			get
			{
				return this.DeployableWeapons.Select(new Func<SynchedMissionObject, Type>(MissionSiegeWeaponsController.GetWeaponType));
			}
		}

		// Token: 0x06002D55 RID: 11605 RVA: 0x000B1794 File Offset: 0x000AF994
		public void Hide()
		{
			this.ToggleDeploymentPointVisibility(false);
			foreach (SynchedMissionObject synchedMissionObject in this.GetWeaponsUnder())
			{
				if (synchedMissionObject != null)
				{
					synchedMissionObject.SetVisibleSynched(false, false);
					synchedMissionObject.SetPhysicsStateSynched(false, true);
				}
			}
		}

		// Token: 0x06002D56 RID: 11606 RVA: 0x000B17FC File Offset: 0x000AF9FC
		public void Show()
		{
			this.ToggleDeploymentPointVisibility(!this.IsDeployed);
			if (this.IsDeployed)
			{
				this.ToggleDeployedWeaponVisibility(true);
			}
		}

		// Token: 0x06002D57 RID: 11607 RVA: 0x000B181C File Offset: 0x000AFA1C
		private void ToggleDeploymentPointVisibility(bool visible)
		{
			this.SetVisibleSynched(visible, false);
			this.SetPhysicsStateSynched(visible, true);
		}

		// Token: 0x06002D58 RID: 11608 RVA: 0x000B182E File Offset: 0x000AFA2E
		private void ToggleDeployedWeaponVisibility(bool visible)
		{
			this.ToggleWeaponVisibility(visible, this.DeployedWeapon);
		}

		// Token: 0x06002D59 RID: 11609 RVA: 0x000B1840 File Offset: 0x000AFA40
		public void ToggleWeaponVisibility(bool visible, SynchedMissionObject weapon)
		{
			SynchedMissionObject synchedMissionObject;
			if (weapon == null)
			{
				synchedMissionObject = null;
			}
			else
			{
				GameEntity parent = weapon.GameEntity.Parent;
				synchedMissionObject = ((parent != null) ? parent.GetFirstScriptOfType<SynchedMissionObject>() : null);
			}
			SynchedMissionObject synchedMissionObject2 = synchedMissionObject;
			if (synchedMissionObject2 != null)
			{
				synchedMissionObject2.SetVisibleSynched(visible, false);
				synchedMissionObject2.SetPhysicsStateSynched(visible, true);
			}
			else
			{
				if (weapon != null)
				{
					weapon.SetVisibleSynched(visible, false);
				}
				if (weapon != null)
				{
					weapon.SetPhysicsStateSynched(visible, true);
				}
			}
			if (weapon is SiegeWeapon && weapon.GameEntity.Parent != null)
			{
				foreach (GameEntity gameEntity in weapon.GameEntity.Parent.GetChildren())
				{
					SiegeMachineStonePile firstScriptOfType = gameEntity.GetFirstScriptOfType<SiegeMachineStonePile>();
					if (firstScriptOfType != null)
					{
						firstScriptOfType.SetPhysicsStateSynched(visible, true);
						break;
					}
				}
			}
		}

		// Token: 0x06002D5A RID: 11610 RVA: 0x000B1908 File Offset: 0x000AFB08
		public void HideAllWeapons()
		{
			foreach (SynchedMissionObject synchedMissionObject in this.DeployableWeapons)
			{
				this.ToggleWeaponVisibility(false, synchedMissionObject);
			}
		}

		// Token: 0x040011AF RID: 4527
		private DeploymentPoint.DeploymentPointType _deploymentPointType;

		// Token: 0x040011B0 RID: 4528
		private List<SiegeLadder> _associatedSiegeLadders;

		// Token: 0x040011B1 RID: 4529
		private bool _isBreachSideDeploymentPoint;

		// Token: 0x040011B4 RID: 4532
		public BattleSideEnum Side = BattleSideEnum.Attacker;

		// Token: 0x040011B5 RID: 4533
		public float Radius = 3f;

		// Token: 0x040011B6 RID: 4534
		public string SiegeWeaponTag = "dpWeapon";

		// Token: 0x040011B7 RID: 4535
		private List<SynchedMissionObject> _weapons;

		// Token: 0x040011BA RID: 4538
		private readonly List<GameEntity> _highlightedEntites = new List<GameEntity>();

		// Token: 0x02000653 RID: 1619
		public enum DeploymentPointType
		{
			// Token: 0x0400207F RID: 8319
			BatteringRam,
			// Token: 0x04002080 RID: 8320
			TowerLadder,
			// Token: 0x04002081 RID: 8321
			Breach,
			// Token: 0x04002082 RID: 8322
			Ranged
		}

		// Token: 0x02000654 RID: 1620
		public enum DeploymentPointState
		{
			// Token: 0x04002084 RID: 8324
			NotDeployed,
			// Token: 0x04002085 RID: 8325
			BatteringRam,
			// Token: 0x04002086 RID: 8326
			SiegeLadder,
			// Token: 0x04002087 RID: 8327
			SiegeTower,
			// Token: 0x04002088 RID: 8328
			Breach,
			// Token: 0x04002089 RID: 8329
			Ranged
		}
	}
}
