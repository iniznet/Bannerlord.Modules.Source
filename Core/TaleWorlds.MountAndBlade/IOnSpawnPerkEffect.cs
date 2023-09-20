using System;
using System.Collections.Generic;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000313 RID: 787
	public interface IOnSpawnPerkEffect
	{
		// Token: 0x06002A7A RID: 10874
		float GetTroopCountMultiplier();

		// Token: 0x06002A7B RID: 10875
		float GetExtraTroopCount();

		// Token: 0x06002A7C RID: 10876
		List<ValueTuple<EquipmentIndex, EquipmentElement>> GetAlternativeEquipments(bool isPlayer, List<ValueTuple<EquipmentIndex, EquipmentElement>> alternativeEquipments, bool getAll = false);

		// Token: 0x06002A7D RID: 10877
		float GetDrivenPropertyBonusOnSpawn(bool isPlayer, DrivenProperty drivenProperty, float baseValue);

		// Token: 0x06002A7E RID: 10878
		float GetHitpoints(bool isPlayer);
	}
}
