using System;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000375 RID: 885
	internal class DropExtraWeaponOnStopUsageComponent : UsableMissionObjectComponent
	{
		// Token: 0x06003051 RID: 12369 RVA: 0x000C6C20 File Offset: 0x000C4E20
		protected internal override void OnUseStopped(Agent userAgent, bool isSuccessful = true)
		{
			if (isSuccessful && !userAgent.Equipment[EquipmentIndex.ExtraWeaponSlot].IsEmpty)
			{
				userAgent.DropItem(EquipmentIndex.ExtraWeaponSlot, WeaponClass.Undefined);
			}
		}
	}
}
