using System;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x02000174 RID: 372
	public abstract class PartySpeedModel : GameModel
	{
		// Token: 0x1700067F RID: 1663
		// (get) Token: 0x06001908 RID: 6408
		public abstract float BaseSpeed { get; }

		// Token: 0x17000680 RID: 1664
		// (get) Token: 0x06001909 RID: 6409
		public abstract float MinimumSpeed { get; }

		// Token: 0x0600190A RID: 6410
		public abstract ExplainedNumber CalculateBaseSpeed(MobileParty party, bool includeDescriptions = false, int additionalTroopOnFootCount = 0, int additionalTroopOnHorseCount = 0);

		// Token: 0x0600190B RID: 6411
		public abstract ExplainedNumber CalculateFinalSpeed(MobileParty mobileParty, ExplainedNumber finalSpeed);
	}
}
