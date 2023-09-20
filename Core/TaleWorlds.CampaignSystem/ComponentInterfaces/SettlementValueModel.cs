using System;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x020001A0 RID: 416
	public abstract class SettlementValueModel : GameModel
	{
		// Token: 0x06001A4B RID: 6731
		public abstract float CalculateSettlementValueForFaction(Settlement settlement, IFaction faction);

		// Token: 0x06001A4C RID: 6732
		public abstract float CalculateSettlementBaseValue(Settlement settlement);

		// Token: 0x06001A4D RID: 6733
		public abstract float CalculateSettlementValueForEnemyHero(Settlement settlement, Hero hero);
	}
}
