using System;

namespace TaleWorlds.CampaignSystem.BarterSystem
{
	public class GoldBarterGroup : BarterGroup
	{
		public override float AIDecisionWeight
		{
			get
			{
				return 0.6f;
			}
		}
	}
}
