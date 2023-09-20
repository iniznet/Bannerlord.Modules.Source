﻿using System;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.CharacterCreationContent
{
	public class FaceGenMount
	{
		public MountCreationKey MountKey { get; private set; }

		public ItemObject HorseItem { get; private set; }

		public ItemObject HarnessItem { get; private set; }

		public string ActionName { get; set; }

		public FaceGenMount(MountCreationKey mountKey, ItemObject horseItem, ItemObject harnessItem, string actionName = "act_inventory_idle_start")
		{
			this.MountKey = mountKey;
			this.HorseItem = horseItem;
			this.HarnessItem = harnessItem;
			this.ActionName = actionName;
		}
	}
}
