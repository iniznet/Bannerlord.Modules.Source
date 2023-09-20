using System;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x0200019B RID: 411
	public abstract class SettlementMilitiaModel : GameModel
	{
		// Token: 0x06001A2F RID: 6703
		public abstract ExplainedNumber CalculateMilitiaChange(Settlement settlement, bool includeDescriptions = false);

		// Token: 0x06001A30 RID: 6704
		public abstract float CalculateEliteMilitiaSpawnChance(Settlement settlement);

		// Token: 0x06001A31 RID: 6705
		public abstract void CalculateMilitiaSpawnRate(Settlement settlement, out float meleeTroopRate, out float rangedTroopRate);
	}
}
