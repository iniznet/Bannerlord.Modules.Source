using System;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
	internal class DropExtraWeaponOnStopUsageComponent : UsableMissionObjectComponent
	{
		protected internal override void OnUseStopped(Agent userAgent, bool isSuccessful = true)
		{
			if (isSuccessful && !userAgent.Equipment[EquipmentIndex.ExtraWeaponSlot].IsEmpty)
			{
				userAgent.DropItem(EquipmentIndex.ExtraWeaponSlot, WeaponClass.Undefined);
			}
		}
	}
}
