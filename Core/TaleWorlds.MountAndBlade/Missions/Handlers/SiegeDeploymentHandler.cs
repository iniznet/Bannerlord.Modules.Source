using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade.AI;

namespace TaleWorlds.MountAndBlade.Missions.Handlers
{
	public class SiegeDeploymentHandler : BattleDeploymentHandler
	{
		public IEnumerable<DeploymentPoint> PlayerDeploymentPoints { get; private set; }

		public IEnumerable<DeploymentPoint> AllDeploymentPoints { get; private set; }

		public SiegeDeploymentHandler(bool isPlayerAttacker)
			: base(isPlayerAttacker)
		{
		}

		public override void OnBehaviorInitialize()
		{
			MissionSiegeEnginesLogic missionBehavior = base.Mission.GetMissionBehavior<MissionSiegeEnginesLogic>();
			this._defenderSiegeWeaponsController = missionBehavior.GetSiegeWeaponsController(BattleSideEnum.Defender);
			this._attackerSiegeWeaponsController = missionBehavior.GetSiegeWeaponsController(BattleSideEnum.Attacker);
		}

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

		public override void FinishDeployment()
		{
			foreach (DeploymentPoint deploymentPoint in this.AllDeploymentPoints)
			{
				deploymentPoint.OnDeploymentStateChanged -= this.OnDeploymentStateChange;
			}
			base.FinishDeployment();
		}

		public void DeployAllSiegeWeaponsOfPlayer()
		{
			BattleSideEnum side = (this.isPlayerAttacker ? BattleSideEnum.Attacker : BattleSideEnum.Defender);
			new SiegeWeaponAutoDeployer((from dp in base.Mission.ActiveMissionObjects.FindAllWithType<DeploymentPoint>()
				where dp.Side == side
				select dp).ToList<DeploymentPoint>(), this.GetWeaponsControllerOfSide(side)).DeployAll(side);
		}

		public int GetMaxDeployableWeaponCountOfPlayer(Type weapon)
		{
			return this.GetWeaponsControllerOfSide(this.isPlayerAttacker ? BattleSideEnum.Attacker : BattleSideEnum.Defender).GetMaxDeployableWeaponCount(weapon);
		}

		public void DeployAllSiegeWeaponsOfAi()
		{
			BattleSideEnum side = (this.isPlayerAttacker ? BattleSideEnum.Defender : BattleSideEnum.Attacker);
			new SiegeWeaponAutoDeployer((from dp in base.Mission.ActiveMissionObjects.FindAllWithType<DeploymentPoint>()
				where dp.Side == side
				select dp).ToList<DeploymentPoint>(), this.GetWeaponsControllerOfSide(side)).DeployAll(side);
			this.RemoveDeploymentPoints(side);
		}

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

		public int GetDeployableWeaponCountOfPlayer(Type weapon)
		{
			return this.GetWeaponsControllerOfSide(this.isPlayerAttacker ? BattleSideEnum.Attacker : BattleSideEnum.Defender).GetMaxDeployableWeaponCount(weapon) - this.PlayerDeploymentPoints.Count((DeploymentPoint dp) => dp.IsDeployed && MissionSiegeWeaponsController.GetWeaponType(dp.DeployedWeapon) == weapon);
		}

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

		private IMissionSiegeWeaponsController GetWeaponsControllerOfSide(BattleSideEnum side)
		{
			if (side != BattleSideEnum.Defender)
			{
				return this._attackerSiegeWeaponsController;
			}
			return this._defenderSiegeWeaponsController;
		}

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

		private IMissionSiegeWeaponsController _defenderSiegeWeaponsController;

		private IMissionSiegeWeaponsController _attackerSiegeWeaponsController;
	}
}
