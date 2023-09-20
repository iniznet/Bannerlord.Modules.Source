using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade.Missions
{
	// Token: 0x02000400 RID: 1024
	public class MissionSiegeWeaponsController : IMissionSiegeWeaponsController
	{
		// Token: 0x0600351B RID: 13595 RVA: 0x000DD67E File Offset: 0x000DB87E
		public MissionSiegeWeaponsController(BattleSideEnum side, List<MissionSiegeWeapon> weapons)
		{
			this._side = side;
			this._weapons = weapons;
			this._undeployedWeapons = new List<MissionSiegeWeapon>(this._weapons);
			this._deployedWeapons = new Dictionary<DestructableComponent, MissionSiegeWeapon>();
		}

		// Token: 0x0600351C RID: 13596 RVA: 0x000DD6B0 File Offset: 0x000DB8B0
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

		// Token: 0x0600351D RID: 13597 RVA: 0x000DD714 File Offset: 0x000DB914
		public IEnumerable<IMissionSiegeWeapon> GetSiegeWeapons()
		{
			return this._weapons.Cast<IMissionSiegeWeapon>();
		}

		// Token: 0x0600351E RID: 13598 RVA: 0x000DD724 File Offset: 0x000DB924
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

		// Token: 0x0600351F RID: 13599 RVA: 0x000DD7C4 File Offset: 0x000DB9C4
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

		// Token: 0x06003520 RID: 13600 RVA: 0x000DD844 File Offset: 0x000DBA44
		private void OnWeaponHit(DestructableComponent target, Agent attackerAgent, in MissionWeapon weapon, ScriptComponentBehavior attackerScriptComponentBehavior, int inflictedDamage)
		{
			MissionSiegeWeapon missionSiegeWeapon;
			if (target.BattleSide == this._side && this._deployedWeapons.TryGetValue(target, out missionSiegeWeapon))
			{
				float num = Math.Max(0f, missionSiegeWeapon.Health - (float)inflictedDamage);
				missionSiegeWeapon.SetHealth(num);
			}
		}

		// Token: 0x06003521 RID: 13601 RVA: 0x000DD88C File Offset: 0x000DBA8C
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

		// Token: 0x06003522 RID: 13602 RVA: 0x000DD8F4 File Offset: 0x000DBAF4
		public static Type GetWeaponType(ScriptComponentBehavior weapon)
		{
			if (weapon is UsableGameObjectGroup)
			{
				return weapon.GameEntity.GetChildren().SelectMany((GameEntity c) => c.GetScriptComponents()).First((ScriptComponentBehavior s) => s is IFocusable)
					.GetType();
			}
			return weapon.GetType();
		}

		// Token: 0x06003523 RID: 13603 RVA: 0x000DD968 File Offset: 0x000DBB68
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

		// Token: 0x040016C8 RID: 5832
		private readonly List<MissionSiegeWeapon> _weapons;

		// Token: 0x040016C9 RID: 5833
		private readonly List<MissionSiegeWeapon> _undeployedWeapons;

		// Token: 0x040016CA RID: 5834
		private readonly Dictionary<DestructableComponent, MissionSiegeWeapon> _deployedWeapons;

		// Token: 0x040016CB RID: 5835
		private BattleSideEnum _side;
	}
}
