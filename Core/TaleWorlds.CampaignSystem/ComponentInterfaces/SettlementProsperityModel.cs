using System;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x020001A3 RID: 419
	public abstract class SettlementProsperityModel : GameModel
	{
		// Token: 0x06001A80 RID: 6784
		public abstract ExplainedNumber CalculateProsperityChange(Town fortification, bool includeDescriptions = false);

		// Token: 0x06001A81 RID: 6785
		public abstract ExplainedNumber CalculateHearthChange(Village village, bool includeDescriptions = false);
	}
}
