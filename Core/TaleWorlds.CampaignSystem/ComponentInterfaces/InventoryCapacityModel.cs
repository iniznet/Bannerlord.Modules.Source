using System;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x02000197 RID: 407
	public abstract class InventoryCapacityModel : GameModel
	{
		// Token: 0x06001A1F RID: 6687
		public abstract ExplainedNumber CalculateInventoryCapacity(MobileParty mobileParty, bool includeDescriptions = false, int additionalManOnFoot = 0, int additionalSpareMounts = 0, int additionalPackAnimals = 0, bool includeFollowers = false);

		// Token: 0x06001A20 RID: 6688
		public abstract int GetItemAverageWeight();
	}
}
