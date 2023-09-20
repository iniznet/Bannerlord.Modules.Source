using System;
using System.Collections.Generic;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade.Missions
{
	// Token: 0x020003FF RID: 1023
	public interface IMissionSiegeWeaponsController
	{
		// Token: 0x06003517 RID: 13591
		int GetMaxDeployableWeaponCount(Type t);

		// Token: 0x06003518 RID: 13592
		IEnumerable<IMissionSiegeWeapon> GetSiegeWeapons();

		// Token: 0x06003519 RID: 13593
		void OnWeaponDeployed(SiegeWeapon missionWeapon);

		// Token: 0x0600351A RID: 13594
		void OnWeaponUndeployed(SiegeWeapon missionWeapon);
	}
}
