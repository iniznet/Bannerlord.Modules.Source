using System;

namespace TaleWorlds.CampaignSystem.BarterSystem
{
	public class FiefBarterGroup : BarterGroup
	{
		public override float AIDecisionWeight
		{
			get
			{
				return 0.05f;
			}
		}
	}
}
