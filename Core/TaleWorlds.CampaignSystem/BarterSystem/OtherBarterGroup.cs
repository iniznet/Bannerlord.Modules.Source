using System;

namespace TaleWorlds.CampaignSystem.BarterSystem
{
	public class OtherBarterGroup : BarterGroup
	{
		public override float AIDecisionWeight
		{
			get
			{
				return 0.25f;
			}
		}
	}
}
