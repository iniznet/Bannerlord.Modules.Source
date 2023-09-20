﻿using System;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade.ComponentInterfaces
{
	public abstract class ItemPickupModel : GameModel
	{
		public abstract float GetItemScoreForAgent(SpawnedItemEntity item, Agent agent);

		public abstract bool IsItemAvailableForAgent(SpawnedItemEntity item, Agent agent, EquipmentIndex slotToPickUp);

		public abstract bool IsAgentEquipmentSuitableForPickUpAvailability(Agent agent);
	}
}
