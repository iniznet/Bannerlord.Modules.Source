using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade.AI;

namespace TaleWorlds.MountAndBlade.Missions.Handlers
{
	// Token: 0x02000402 RID: 1026
	public class SiegeDeploymentHandler : BattleDeploymentHandler
	{
		// Token: 0x1700093D RID: 2365
		// (get) Token: 0x0600352C RID: 13612 RVA: 0x000DDB75 File Offset: 0x000DBD75
		// (set) Token: 0x0600352D RID: 13613 RVA: 0x000DDB7D File Offset: 0x000DBD7D
		public IEnumerable<DeploymentPoint> PlayerDeploymentPoints { get; private set; }

		// Token: 0x1700093E RID: 2366
		// (get) Token: 0x0600352E RID: 13614 RVA: 0x000DDB86 File Offset: 0x000DBD86
		// (set) Token: 0x0600352F RID: 13615 RVA: 0x000DDB8E File Offset: 0x000DBD8E
		public IEnumerable<DeploymentPoint> AllDeploymentPoints { get; private set; }

		// Token: 0x06003530 RID: 13616 RVA: 0x000DDB97 File Offset: 0x000DBD97
		public SiegeDeploymentHandler(bool isPlayerAttacker)
			: base(isPlayerAttacker)
		{
		}

		// Token: 0x06003531 RID: 13617 RVA: 0x000DDBA0 File Offset: 0x000DBDA0
		public override void OnBehaviorInitialize()
		{
			MissionSiegeEnginesLogic missionBehavior = base.Mission.GetMissionBehavior<MissionSiegeEnginesLogic>();
			this._defenderSiegeWeaponsController = missionBehavior.GetSiegeWeaponsController(BattleSideEnum.Defender);
			this._attackerSiegeWeaponsController = missionBehavior.GetSiegeWeaponsController(BattleSideEnum.Attacker);
		}

		// Token: 0x06003532 RID: 13618 RVA: 0x000DDBD4 File Offset: 0x000DBDD4
		public override void AfterStart()
		{
			base.AfterStart();
			this.AllDeploymentPoints = Mission.Current.ActiveMissionObjects.FindAllWithType<DeploymentPoint>();
			this.PlayerDeploymentPoints = this.AllDeploymentPoints.Where((DeploymentPoint dp) => dp.Side == base.team.Side);
			foreach (DeploymentPoint deploymentPoint in this.AllDeploymentPoints)
			{
				deploymentPoint.OnDeploymentStateChanged += this.OnDeploymentStateChange;
			}
			base.Mission.IsFormationUnitPositionAvailable_AdditionalCondition += base.Mission_IsFormationUnitPositionAvailable_AdditionalCondition;
		}

		// Token: 0x06003533 RID: 13619 RVA: 0x000DDC7C File Offset: 0x000DBE7C
		public override void FinishDeployment()
		{
			foreach (DeploymentPoint deploymentPoint in this.AllDeploymentPoints)
			{
				deploymentPoint.OnDeploymentStateChanged -= this.OnDeploymentStateChange;
			}
			base.FinishDeployment();
		}

		// Token: 0x06003534 RID: 13620 RVA: 0x000DDCD8 File Offset: 0x000DBED8
		public void DeployAllSiegeWeaponsOfPlayer()
		{
			BattleSideEnum side = (this.isPlayerAttacker ? BattleSideEnum.Attacker : BattleSideEnum.Defender);
			new SiegeWeaponAutoDeployer((from dp in base.Mission.ActiveMissionObjects.FindAllWithType<DeploymentPoint>()
				where dp.Side == side
				select dp).ToList<DeploymentPoint>(), this.GetWeaponsControllerOfSide(side)).DeployAll(side);
		}

		// Token: 0x06003535 RID: 13621 RVA: 0x000DDD3F File Offset: 0x000DBF3F
		public int GetMaxDeployableWeaponCountOfPlayer(Type weapon)
		{
			return this.GetWeaponsControllerOfSide(this.isPlayerAttacker ? BattleSideEnum.Attacker : BattleSideEnum.Defender).GetMaxDeployableWeaponCount(weapon);
		}

		// Token: 0x06003536 RID: 13622 RVA: 0x000DDD5C File Offset: 0x000DBF5C
		public void DeployAllSiegeWeaponsOfAi()
		{
			BattleSideEnum side = (this.isPlayerAttacker ? BattleSideEnum.Defender : BattleSideEnum.Attacker);
			new SiegeWeaponAutoDeployer((from dp in base.Mission.ActiveMissionObjects.FindAllWithType<DeploymentPoint>()
				where dp.Side == side
				select dp).ToList<DeploymentPoint>(), this.GetWeaponsControllerOfSide(side)).DeployAll(side);
			this.RemoveDeploymentPoints(side);
		}

		// Token: 0x06003537 RID: 13623 RVA: 0x000DDDD0 File Offset: 0x000DBFD0
		public void RemoveDeploymentPoints(BattleSideEnum side)
		{
			IEnumerable<DeploymentPoint> enumerable = base.Mission.ActiveMissionObjects.FindAllWithType<DeploymentPoint>();
			Func<DeploymentPoint, bool> <>9__0;
			Func<DeploymentPoint, bool> func;
			if ((func = <>9__0) == null)
			{
				func = (<>9__0 = (DeploymentPoint dp) => dp.Side == side);
			}
			foreach (DeploymentPoint deploymentPoint in enumerable.Where(func).ToArray<DeploymentPoint>())
			{
				foreach (SynchedMissionObject synchedMissionObject in deploymentPoint.DeployableWeapons.ToArray<SynchedMissionObject>())
				{
					if (deploymentPoint.DeployedWeapon == null || !synchedMissionObject.GameEntity.IsVisibleIncludeParents())
					{
						SiegeWeapon siegeWeapon = synchedMissionObject as SiegeWeapon;
						if (siegeWeapon != null)
						{
							siegeWeapon.SetDisabledSynched();
						}
					}
				}
				deploymentPoint.SetDisabledSynched();
			}
		}

		// Token: 0x06003538 RID: 13624 RVA: 0x000DDE94 File Offset: 0x000DC094
		public void RemoveUnavailableDeploymentPoints(BattleSideEnum side)
		{
			IMissionSiegeWeaponsController weapons = ((side == BattleSideEnum.Defender) ? this._defenderSiegeWeaponsController : this._attackerSiegeWeaponsController);
			IEnumerable<DeploymentPoint> enumerable = base.Mission.ActiveMissionObjects.FindAllWithType<DeploymentPoint>();
			Func<DeploymentPoint, bool> <>9__0;
			Func<DeploymentPoint, bool> func;
			if ((func = <>9__0) == null)
			{
				func = (<>9__0 = (DeploymentPoint dp) => dp.Side == side);
			}
			Func<Type, bool> <>9__1;
			foreach (DeploymentPoint deploymentPoint in enumerable.Where(func).ToArray<DeploymentPoint>())
			{
				IEnumerable<Type> deployableWeaponTypes = deploymentPoint.DeployableWeaponTypes;
				Func<Type, bool> func2;
				if ((func2 = <>9__1) == null)
				{
					func2 = (<>9__1 = (Type wt) => weapons.GetMaxDeployableWeaponCount(wt) > 0);
				}
				if (!deployableWeaponTypes.Any(func2))
				{
					foreach (SiegeWeapon siegeWeapon in deploymentPoint.DeployableWeapons.Select((SynchedMissionObject sw) => sw as SiegeWeapon))
					{
						siegeWeapon.SetDisabledSynched();
					}
					deploymentPoint.SetDisabledSynched();
				}
			}
		}

		// Token: 0x06003539 RID: 13625 RVA: 0x000DDFBC File Offset: 0x000DC1BC
		public void UnHideDeploymentPoints(BattleSideEnum side)
		{
			IEnumerable<DeploymentPoint> enumerable = base.Mission.ActiveMissionObjects.FindAllWithType<DeploymentPoint>();
			Func<DeploymentPoint, bool> <>9__0;
			Func<DeploymentPoint, bool> func;
			if ((func = <>9__0) == null)
			{
				func = (<>9__0 = (DeploymentPoint dp) => !dp.IsDisabled && dp.Side == side);
			}
			foreach (DeploymentPoint deploymentPoint in enumerable.Where(func))
			{
				deploymentPoint.Show();
			}
		}

		// Token: 0x0600353A RID: 13626 RVA: 0x000DE044 File Offset: 0x000DC244
		public int GetDeployableWeaponCountOfPlayer(Type weapon)
		{
			return this.GetWeaponsControllerOfSide(this.isPlayerAttacker ? BattleSideEnum.Attacker : BattleSideEnum.Defender).GetMaxDeployableWeaponCount(weapon) - this.PlayerDeploymentPoints.Count((DeploymentPoint dp) => dp.IsDeployed && MissionSiegeWeaponsController.GetWeaponType(dp.DeployedWeapon) == weapon);
		}

		// Token: 0x0600353B RID: 13627 RVA: 0x000DE094 File Offset: 0x000DC294
		private void OnDeploymentStateChange(DeploymentPoint deploymentPoint, SynchedMissionObject targetObject)
		{
			if (!deploymentPoint.IsDeployed && base.team.DetachmentManager.ContainsDetachment(deploymentPoint.DisbandedWeapon as IDetachment))
			{
				base.team.DetachmentManager.DestroyDetachment(deploymentPoint.DisbandedWeapon as IDetachment);
			}
			SiegeWeapon siegeWeapon;
			if ((siegeWeapon = targetObject as SiegeWeapon) != null)
			{
				IMissionSiegeWeaponsController weaponsControllerOfSide = this.GetWeaponsControllerOfSide(deploymentPoint.Side);
				if (deploymentPoint.IsDeployed)
				{
					weaponsControllerOfSide.OnWeaponDeployed(siegeWeapon);
					return;
				}
				weaponsControllerOfSide.OnWeaponUndeployed(siegeWeapon);
			}
		}

		// Token: 0x0600353C RID: 13628 RVA: 0x000DE10F File Offset: 0x000DC30F
		private IMissionSiegeWeaponsController GetWeaponsControllerOfSide(BattleSideEnum side)
		{
			if (side != BattleSideEnum.Defender)
			{
				return this._attackerSiegeWeaponsController;
			}
			return this._defenderSiegeWeaponsController;
		}

		// Token: 0x0600353D RID: 13629 RVA: 0x000DE124 File Offset: 0x000DC324
		[Conditional("DEBUG")]
		private void AssertSiegeWeapons(IEnumerable<DeploymentPoint> allDeploymentPoints)
		{
			HashSet<SynchedMissionObject> hashSet = new HashSet<SynchedMissionObject>();
			foreach (SynchedMissionObject synchedMissionObject in allDeploymentPoints.SelectMany((DeploymentPoint amo) => amo.DeployableWeapons))
			{
				if (!hashSet.Add(synchedMissionObject))
				{
					break;
				}
			}
		}

		// Token: 0x040016CD RID: 5837
		private IMissionSiegeWeaponsController _defenderSiegeWeaponsController;

		// Token: 0x040016CE RID: 5838
		private IMissionSiegeWeaponsController _attackerSiegeWeaponsController;
	}
}
