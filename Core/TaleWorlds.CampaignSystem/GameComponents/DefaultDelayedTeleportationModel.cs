using System;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Map;
using TaleWorlds.CampaignSystem.Party;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	// Token: 0x02000103 RID: 259
	public class DefaultDelayedTeleportationModel : DelayedTeleportationModel
	{
		// Token: 0x170005E1 RID: 1505
		// (get) Token: 0x06001527 RID: 5415 RVA: 0x000610AB File Offset: 0x0005F2AB
		public override float DefaultTeleportationSpeed
		{
			get
			{
				return 0.24f;
			}
		}

		// Token: 0x06001528 RID: 5416 RVA: 0x000610B4 File Offset: 0x0005F2B4
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

		// Token: 0x04000783 RID: 1923
		private const float MaximumDistanceForDelay = 300f;
	}
}
