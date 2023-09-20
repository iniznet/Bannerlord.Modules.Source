using System;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade.ComponentInterfaces
{
	// Token: 0x02000407 RID: 1031
	public abstract class ItemPickupModel : GameModel
	{
		// Token: 0x0600355E RID: 13662
		public abstract float GetItemScoreForAgent(SpawnedItemEntity item, Agent agent);

		// Token: 0x0600355F RID: 13663
		public abstract bool IsItemAvailableForAgent(SpawnedItemEntity item, Agent agent, EquipmentIndex slotToPickUp);

		// Token: 0x06003560 RID: 13664
		public abstract bool IsAgentEquipmentSuitableForPickUpAvailability(Agent agent);
	}
}
