using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade.Missions
{
	public class MissionSiegeWeaponsController : IMissionSiegeWeaponsController
	{
		public MissionSiegeWeaponsController(BattleSideEnum side, List<MissionSiegeWeapon> weapons)
		{
			this._side = side;
			this._weapons = weapons;
			this._undeployedWeapons = new List<MissionSiegeWeapon>(this._weapons);
			this._deployedWeapons = new Dictionary<DestructableComponent, MissionSiegeWeapon>();
		}

		public int GetMaxDeployableWeaponCount(Type t)
		{
			int num = 0;
			using (List<MissionSiegeWeapon>.Enumerator enumerator = this._weapons.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (MissionSiegeWeaponsController.GetSiegeWeaponBaseType(enumerator.Current.Type) == t)
					{
						num++;
					}
				}
			}
			return num;
		}

		public IEnumerable<IMissionSiegeWeapon> GetSiegeWeapons()
		{
			return this._weapons.Cast<IMissionSiegeWeapon>();
		}

		public void OnWeaponDeployed(SiegeWeapon missionWeapon)
		{
			SiegeEngineType missionWeaponType = missionWeapon.GetSiegeEngineType();
			int num = this._undeployedWeapons.FindIndex((MissionSiegeWeapon uw) => uw.Type == missionWeaponType);
			MissionSiegeWeapon missionSiegeWeapon = this._undeployedWeapons[num];
			DestructableComponent destructionComponent = missionWeapon.DestructionComponent;
			destructionComponent.MaxHitPoint = missionSiegeWeapon.MaxHealth;
			destructionComponent.HitPoint = missionSiegeWeapon.InitialHealth;
			destructionComponent.OnHitTaken += new DestructableComponent.OnHitTakenAndDestroyedDelegate(this.OnWeaponHit);
			destructionComponent.OnDestroyed += new DestructableComponent.OnHitTakenAndDestroyedDelegate(this.OnWeaponDestroyed);
			this._undeployedWeapons.RemoveAt(num);
			this._deployedWeapons.Add(destructionComponent, missionSiegeWeapon);
		}

		public void OnWeaponUndeployed(SiegeWeapon missionWeapon)
		{
			DestructableComponent destructionComponent = missionWeapon.DestructionComponent;
			MissionSiegeWeapon missionSiegeWeapon;
			this._deployedWeapons.TryGetValue(destructionComponent, out missionSiegeWeapon);
			SiegeEngineType siegeEngineType = missionWeapon.GetSiegeEngineType();
			destructionComponent.MaxHitPoint = (float)siegeEngineType.BaseHitPoints;
			destructionComponent.HitPoint = destructionComponent.MaxHitPoint;
			destructionComponent.OnHitTaken -= new DestructableComponent.OnHitTakenAndDestroyedDelegate(this.OnWeaponHit);
			destructionComponent.OnDestroyed -= new DestructableComponent.OnHitTakenAndDestroyedDelegate(this.OnWeaponDestroyed);
			this._deployedWeapons.Remove(destructionComponent);
			this._undeployedWeapons.Add(missionSiegeWeapon);
		}

		private void OnWeaponHit(DestructableComponent target, Agent attackerAgent, in MissionWeapon weapon, ScriptComponentBehavior attackerScriptComponentBehavior, int inflictedDamage)
		{
			MissionSiegeWeapon missionSiegeWeapon;
			if (target.BattleSide == this._side && this._deployedWeapons.TryGetValue(target, out missionSiegeWeapon))
			{
				float num = Math.Max(0f, missionSiegeWeapon.Health - (float)inflictedDamage);
				missionSiegeWeapon.SetHealth(num);
			}
		}

		private void OnWeaponDestroyed(DestructableComponent target, Agent attackerAgent, in MissionWeapon weapon, ScriptComponentBehavior attackerScriptComponentBehavior, int inflictedDamage)
		{
			MissionSiegeWeapon missionSiegeWeapon;
			if (target.BattleSide == this._side && this._deployedWeapons.TryGetValue(target, out missionSiegeWeapon))
			{
				missionSiegeWeapon.SetHealth(0f);
				target.OnHitTaken -= new DestructableComponent.OnHitTakenAndDestroyedDelegate(this.OnWeaponHit);
				target.OnDestroyed -= new DestructableComponent.OnHitTakenAndDestroyedDelegate(this.OnWeaponDestroyed);
				this._deployedWeapons.Remove(target);
			}
		}

		public static Type GetWeaponType(ScriptComponentBehavior weapon)
		{
			if (weapon is UsableGameObjectGroup)
			{
				return weapon.GameEntity.GetChildren().SelectMany((GameEntity c) => c.GetScriptComponents()).First((ScriptComponentBehavior s) => s is IFocusable)
					.GetType();
			}
			return weapon.GetType();
		}

		private static Type GetSiegeWeaponBaseType(SiegeEngineType siegeWeaponType)
		{
			if (siegeWeaponType == DefaultSiegeEngineTypes.Ladder)
			{
				return typeof(SiegeLadder);
			}
			if (siegeWeaponType == DefaultSiegeEngineTypes.Ballista)
			{
				return typeof(Ballista);
			}
			if (siegeWeaponType == DefaultSiegeEngineTypes.FireBallista)
			{
				return typeof(FireBallista);
			}
			if (siegeWeaponType == DefaultSiegeEngineTypes.Ram)
			{
				return typeof(BatteringRam);
			}
			if (siegeWeaponType == DefaultSiegeEngineTypes.SiegeTower)
			{
				return typeof(SiegeTower);
			}
			if (siegeWeaponType == DefaultSiegeEngineTypes.Onager || siegeWeaponType == DefaultSiegeEngineTypes.Catapult)
			{
				return typeof(Mangonel);
			}
			if (siegeWeaponType == DefaultSiegeEngineTypes.FireOnager || siegeWeaponType == DefaultSiegeEngineTypes.FireCatapult)
			{
				return typeof(FireMangonel);
			}
			if (siegeWeaponType == DefaultSiegeEngineTypes.Trebuchet)
			{
				return typeof(Trebuchet);
			}
			return null;
		}

		private readonly List<MissionSiegeWeapon> _weapons;

		private readonly List<MissionSiegeWeapon> _undeployedWeapons;

		private readonly Dictionary<DestructableComponent, MissionSiegeWeapon> _deployedWeapons;

		private BattleSideEnum _side;
	}
}
