using System;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x020001CA RID: 458
	public abstract class DelayedTeleportationModel : GameModel
	{
		// Token: 0x17000717 RID: 1815
		// (get) Token: 0x06001B7A RID: 7034
		public abstract float DefaultTeleportationSpeed { get; }

		// Token: 0x06001B7B RID: 7035
		public abstract ExplainedNumber GetTeleportationDelayAsHours(Hero teleportingHero, PartyBase target);
	}
}
