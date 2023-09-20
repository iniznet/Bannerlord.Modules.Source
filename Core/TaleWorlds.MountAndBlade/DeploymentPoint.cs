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
	public class DeploymentPoint : SynchedMissionObject
	{
		public event Action<DeploymentPoint, SynchedMissionObject> OnDeploymentStateChanged;

		public event Action<DeploymentPoint> OnDeploymentPointTypeDetermined;

		public Vec3 DeploymentTargetPosition { get; private set; }

		public WallSegment AssociatedWallSegment { get; private set; }

		public IEnumerable<SynchedMissionObject> DeployableWeapons
		{
			get
			{
				return this._weapons.Where((SynchedMissionObject w) => !w.IsDisabled);
			}
		}

		public bool IsDeployed
		{
			get
			{
				return this.DeployedWeapon != null;
			}
		}

		public SynchedMissionObject DeployedWeapon { get; private set; }

		public SynchedMissionObject DisbandedWeapon { get; private set; }

		protected internal override void OnInit()
		{
			this._weapons = new List<SynchedMissionObject>();
		}

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

		private void SetBreachSideDeploymentPoint()
		{
			Debug.Print("Deployment point " + ((base.GameEntity != null) ? ("upgrade level mask " + base.GameEntity.GetUpgradeLevelMask().ToString()) : "no game entity.") + "\n", 0, Debug.DebugColor.White, 17592186044416UL);
			this._isBreachSideDeploymentPoint = true;
			this._deploymentPointType = DeploymentPoint.DeploymentPointType.Breach;
			FormationAI.BehaviorSide deploymentPointSide = (this._weapons.FirstOrDefault((SynchedMissionObject w) => w is SiegeTower) as IPrimarySiegeWeapon).WeaponSide;
			this.AssociatedWallSegment = Mission.Current.ActiveMissionObjects.FindAllWithType<WallSegment>().FirstOrDefault((WallSegment ws) => ws.DefenseSide == deploymentPointSide);
			this.DeploymentTargetPosition = this.AssociatedWallSegment.GameEntity.GlobalPosition;
		}

		public Vec3 GetDeploymentOrigin()
		{
			return base.GameEntity.GlobalPosition;
		}

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

		public DeploymentPoint.DeploymentPointType GetDeploymentPointType()
		{
			return this._deploymentPointType;
		}

		public List<SiegeLadder> GetAssociatedSiegeLadders()
		{
			return this._associatedSiegeLadders;
		}

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

		protected internal override void OnEditorInit()
		{
			base.OnEditorInit();
			this._weapons = null;
		}

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

		public void Deploy(Type t)
		{
			this.DeployedWeapon = this._weapons.First((SynchedMissionObject w) => MissionSiegeWeaponsController.GetWeaponType(w) == t);
			this.OnDeploymentStateChangedAux(this.DeployedWeapon);
			this.ToggleDeploymentPointVisibility(false);
			this.ToggleDeployedWeaponVisibility(true);
		}

		public void Deploy(SiegeWeapon s)
		{
			this.DeployedWeapon = s;
			this.DisbandedWeapon = null;
			this.OnDeploymentStateChangedAux(s);
			this.ToggleDeploymentPointVisibility(false);
			this.ToggleDeployedWeaponVisibility(true);
		}

		public ScriptComponentBehavior Disband()
		{
			this.ToggleDeploymentPointVisibility(true);
			this.ToggleDeployedWeaponVisibility(false);
			this.DisbandedWeapon = this.DeployedWeapon;
			this.DeployedWeapon = null;
			this.OnDeploymentStateChangedAux(this.DisbandedWeapon);
			return this.DisbandedWeapon;
		}

		public IEnumerable<Type> DeployableWeaponTypes
		{
			get
			{
				return this.DeployableWeapons.Select(new Func<SynchedMissionObject, Type>(MissionSiegeWeaponsController.GetWeaponType));
			}
		}

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

		public void Show()
		{
			this.ToggleDeploymentPointVisibility(!this.IsDeployed);
			if (this.IsDeployed)
			{
				this.ToggleDeployedWeaponVisibility(true);
			}
		}

		private void ToggleDeploymentPointVisibility(bool visible)
		{
			this.SetVisibleSynched(visible, false);
			this.SetPhysicsStateSynched(visible, true);
		}

		private void ToggleDeployedWeaponVisibility(bool visible)
		{
			this.ToggleWeaponVisibility(visible, this.DeployedWeapon);
		}

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

		public void HideAllWeapons()
		{
			foreach (SynchedMissionObject synchedMissionObject in this.DeployableWeapons)
			{
				this.ToggleWeaponVisibility(false, synchedMissionObject);
			}
		}

		private DeploymentPoint.DeploymentPointType _deploymentPointType;

		private List<SiegeLadder> _associatedSiegeLadders;

		private bool _isBreachSideDeploymentPoint;

		public BattleSideEnum Side = BattleSideEnum.Attacker;

		public float Radius = 3f;

		public string SiegeWeaponTag = "dpWeapon";

		private List<SynchedMissionObject> _weapons;

		private readonly List<GameEntity> _highlightedEntites = new List<GameEntity>();

		public enum DeploymentPointType
		{
			BatteringRam,
			TowerLadder,
			Breach,
			Ranged
		}

		public enum DeploymentPointState
		{
			NotDeployed,
			BatteringRam,
			SiegeLadder,
			SiegeTower,
			Breach,
			Ranged
		}
	}
}
