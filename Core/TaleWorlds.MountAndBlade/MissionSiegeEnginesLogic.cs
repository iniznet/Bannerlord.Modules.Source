using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade.Missions;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000277 RID: 631
	public class MissionSiegeEnginesLogic : MissionLogic
	{
		// Token: 0x060021A1 RID: 8609 RVA: 0x0007ADBD File Offset: 0x00078FBD
		public MissionSiegeEnginesLogic(List<MissionSiegeWeapon> defenderSiegeWeapons, List<MissionSiegeWeapon> attackerSiegeWeapons)
		{
			this._defenderSiegeWeaponsController = new MissionSiegeWeaponsController(BattleSideEnum.Defender, defenderSiegeWeapons);
			this._attackerSiegeWeaponsController = new MissionSiegeWeaponsController(BattleSideEnum.Attacker, attackerSiegeWeapons);
		}

		// Token: 0x060021A2 RID: 8610 RVA: 0x0007ADDF File Offset: 0x00078FDF
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

		// Token: 0x060021A3 RID: 8611 RVA: 0x0007ADF7 File Offset: 0x00078FF7
		public void GetMissionSiegeWeapons(out IEnumerable<IMissionSiegeWeapon> defenderSiegeWeapons, out IEnumerable<IMissionSiegeWeapon> attackerSiegeWeapons)
		{
			defenderSiegeWeapons = this._defenderSiegeWeaponsController.GetSiegeWeapons();
			attackerSiegeWeapons = this._attackerSiegeWeaponsController.GetSiegeWeapons();
		}

		// Token: 0x04000C6A RID: 3178
		private readonly MissionSiegeWeaponsController _defenderSiegeWeaponsController;

		// Token: 0x04000C6B RID: 3179
		private readonly MissionSiegeWeaponsController _attackerSiegeWeaponsController;
	}
}
