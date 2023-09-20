using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Missions;

namespace TaleWorlds.MountAndBlade.AI
{
	public class SiegeWeaponAutoDeployer
	{
		public SiegeWeaponAutoDeployer(List<DeploymentPoint> deploymentPoints, IMissionSiegeWeaponsController weaponsController)
		{
			this.deploymentPoints = deploymentPoints;
			this.siegeWeaponsController = weaponsController;
		}

		public void DeployAll(BattleSideEnum side)
		{
			if (side == BattleSideEnum.Attacker)
			{
				this.DeployAllForAttackers();
				return;
			}
			if (side == BattleSideEnum.Defender)
			{
				this.DeployAllForDefenders();
				return;
			}
			Debug.FailedAssert("Invalid side", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\AI\\SiegeWeaponAutoDeployer.cs", "DeployAll", 32);
		}

		private bool DeployWeaponFrom(DeploymentPoint dp)
		{
			IEnumerable<Type> enumerable = dp.DeployableWeaponTypes.Where((Type t) => this.deploymentPoints.Count((DeploymentPoint dep) => dep.IsDeployed && MissionSiegeWeaponsController.GetWeaponType(dep.DeployedWeapon) == t) < this.siegeWeaponsController.GetMaxDeployableWeaponCount(t));
			if (!enumerable.IsEmpty<Type>())
			{
				Type type = enumerable.MaxBy((Type t) => this.GetWeaponValue(t));
				dp.Deploy(type);
				return true;
			}
			return false;
		}

		private void DeployAllForAttackers()
		{
			List<DeploymentPoint> list = this.deploymentPoints.Where((DeploymentPoint dp) => !dp.IsDisabled && !dp.IsDeployed).ToList<DeploymentPoint>();
			list.Shuffle<DeploymentPoint>();
			int num = this.deploymentPoints.Count((DeploymentPoint dp) => dp.GetDeploymentPointType() == DeploymentPoint.DeploymentPointType.Breach);
			bool flag = Mission.Current.AttackerTeam != Mission.Current.PlayerTeam && num >= 2;
			foreach (DeploymentPoint deploymentPoint in list)
			{
				if (!flag || deploymentPoint.GetDeploymentPointType() == DeploymentPoint.DeploymentPointType.Ranged)
				{
					this.DeployWeaponFrom(deploymentPoint);
				}
			}
		}

		private void DeployAllForDefenders()
		{
			Mission mission = Mission.Current;
			Scene scene = mission.Scene;
			List<ICastleKeyPosition> list = (from amo in mission.ActiveMissionObjects
				select amo.GameEntity into e
				select e.GetFirstScriptOfType<UsableMachine>() into um
				where um is ICastleKeyPosition
				select um).Cast<ICastleKeyPosition>().Where(delegate(ICastleKeyPosition x)
			{
				IPrimarySiegeWeapon attackerSiegeWeapon = x.AttackerSiegeWeapon;
				return attackerSiegeWeapon == null || attackerSiegeWeapon.WeaponSide != FormationAI.BehaviorSide.BehaviorSideNotSet;
			}).ToList<ICastleKeyPosition>();
			List<DeploymentPoint> list2 = this.deploymentPoints.Where((DeploymentPoint dp) => !dp.IsDeployed).ToList<DeploymentPoint>();
			while (!list2.IsEmpty<DeploymentPoint>())
			{
				Threat maxThreat = RangedSiegeWeaponAi.ThreatSeeker.GetMaxThreat(list);
				Vec3 mostDangerousThreatPosition = maxThreat.Position;
				DeploymentPoint deploymentPoint = list2.MinBy((DeploymentPoint dp) => dp.GameEntity.GlobalPosition.DistanceSquared(mostDangerousThreatPosition));
				if (this.DeployWeaponFrom(deploymentPoint))
				{
					maxThreat.ThreatValue *= 0.5f;
				}
				list2.Remove(deploymentPoint);
			}
		}

		protected virtual float GetWeaponValue(Type weaponType)
		{
			if (weaponType == typeof(BatteringRam) || weaponType == typeof(SiegeTower) || weaponType == typeof(SiegeLadder))
			{
				return 0.9f + MBRandom.RandomFloat * 0.2f;
			}
			if (typeof(RangedSiegeWeapon).IsAssignableFrom(weaponType))
			{
				return 0.7f + MBRandom.RandomFloat * 0.2f;
			}
			return 1f;
		}

		private IMissionSiegeWeaponsController siegeWeaponsController;

		private List<DeploymentPoint> deploymentPoints;
	}
}
