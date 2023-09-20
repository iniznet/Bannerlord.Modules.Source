using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade.Missions;

namespace TaleWorlds.MountAndBlade
{
	public class MissionSiegeEnginesLogic : MissionLogic
	{
		public MissionSiegeEnginesLogic(List<MissionSiegeWeapon> defenderSiegeWeapons, List<MissionSiegeWeapon> attackerSiegeWeapons)
		{
			this._defenderSiegeWeaponsController = new MissionSiegeWeaponsController(BattleSideEnum.Defender, defenderSiegeWeapons);
			this._attackerSiegeWeaponsController = new MissionSiegeWeaponsController(BattleSideEnum.Attacker, attackerSiegeWeapons);
		}

		public IMissionSiegeWeaponsController GetSiegeWeaponsController(BattleSideEnum side)
		{
			if (side == BattleSideEnum.Defender)
			{
				return this._defenderSiegeWeaponsController;
			}
			if (side == BattleSideEnum.Attacker)
			{
				return this._attackerSiegeWeaponsController;
			}
			return null;
		}

		public void GetMissionSiegeWeapons(out IEnumerable<IMissionSiegeWeapon> defenderSiegeWeapons, out IEnumerable<IMissionSiegeWeapon> attackerSiegeWeapons)
		{
			defenderSiegeWeapons = this._defenderSiegeWeaponsController.GetSiegeWeapons();
			attackerSiegeWeapons = this._attackerSiegeWeaponsController.GetSiegeWeapons();
		}

		private readonly MissionSiegeWeaponsController _defenderSiegeWeaponsController;

		private readonly MissionSiegeWeaponsController _attackerSiegeWeaponsController;
	}
}
