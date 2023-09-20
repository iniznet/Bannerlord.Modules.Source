using System;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Map;
using TaleWorlds.CampaignSystem.Party;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	public class DefaultDelayedTeleportationModel : DelayedTeleportationModel
	{
		public override float DefaultTeleportationSpeed
		{
			get
			{
				return 0.24f;
			}
		}

		public override ExplainedNumber GetTeleportationDelayAsHours(Hero teleportingHero, PartyBase target)
		{
			float num = 300f;
			IMapPoint mapPoint = teleportingHero.GetMapPoint();
			if (mapPoint != null)
			{
				if (target.IsSettlement)
				{
					if (teleportingHero.CurrentSettlement != null && teleportingHero.CurrentSettlement == target.Settlement)
					{
						num = 0f;
					}
					else
					{
						Campaign.Current.Models.MapDistanceModel.GetDistance(mapPoint, target.Settlement, 300f, out num);
					}
				}
				else if (target.IsMobile)
				{
					Campaign.Current.Models.MapDistanceModel.GetDistance(mapPoint, target.MobileParty, 300f, out num);
				}
			}
			return new ExplainedNumber(num * this.DefaultTeleportationSpeed, false, null);
		}

		private const float MaximumDistanceForDelay = 300f;
	}
}
