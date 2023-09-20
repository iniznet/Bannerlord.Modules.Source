using System;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x020001BE RID: 446
	public abstract class CompanionHiringPriceCalculationModel : GameModel
	{
		// Token: 0x06001B2B RID: 6955
		public abstract int GetCompanionHiringPrice(Hero companion);
	}
}
