using System;
using TaleWorlds.CampaignSystem.Map;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	// Token: 0x020003A5 RID: 933
	public interface ITeleportationCampaignBehavior : ICampaignBehavior
	{
		// Token: 0x060037B2 RID: 14258
		bool GetTargetOfTeleportingHero(Hero teleportingHero, out bool isGovernor, out bool isPartyLeader, out IMapPoint target);

		// Token: 0x060037B3 RID: 14259
		CampaignTime GetHeroArrivalTimeToDestination(Hero teleportingHero);
	}
}
