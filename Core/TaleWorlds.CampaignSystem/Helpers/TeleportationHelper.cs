using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;

namespace Helpers
{
	// Token: 0x02000011 RID: 17
	public static class TeleportationHelper
	{
		// Token: 0x06000090 RID: 144 RVA: 0x00007834 File Offset: 0x00005A34
		public static float GetHoursLeftForTeleportingHeroToReachItsDestination(Hero teleportingHero)
		{
			ITeleportationCampaignBehavior campaignBehavior = Campaign.Current.GetCampaignBehavior<ITeleportationCampaignBehavior>();
			if (campaignBehavior != null)
			{
				return campaignBehavior.GetHeroArrivalTimeToDestination(teleportingHero).RemainingHoursFromNow;
			}
			return 0f;
		}
	}
}
