using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;

namespace Helpers
{
	public static class TeleportationHelper
	{
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
