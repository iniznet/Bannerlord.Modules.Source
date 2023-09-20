using System;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x02000172 RID: 370
	public abstract class InformationRestrictionModel : GameModel
	{
		// Token: 0x060018F9 RID: 6393
		public abstract bool DoesPlayerKnowDetailsOf(Settlement settlement);

		// Token: 0x060018FA RID: 6394
		public abstract bool DoesPlayerKnowDetailsOf(Hero hero);
	}
}
