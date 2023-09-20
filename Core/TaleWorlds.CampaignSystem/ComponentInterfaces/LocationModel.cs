using System;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x0200016F RID: 367
	public abstract class LocationModel : GameModel
	{
		// Token: 0x060018EB RID: 6379
		public abstract int GetSettlementUpgradeLevel(LocationEncounter locationEncounter);

		// Token: 0x060018EC RID: 6380
		public abstract string GetCivilianSceneLevel(Settlement settlement);

		// Token: 0x060018ED RID: 6381
		public abstract string GetCivilianUpgradeLevelTag(int upgradeLevel);

		// Token: 0x060018EE RID: 6382
		public abstract string GetUpgradeLevelTag(int upgradeLevel);
	}
}
