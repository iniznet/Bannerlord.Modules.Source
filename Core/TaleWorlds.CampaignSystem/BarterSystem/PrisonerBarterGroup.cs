using System;

namespace TaleWorlds.CampaignSystem.BarterSystem
{
	public class PrisonerBarterGroup : BarterGroup
	{
		public override float AIDecisionWeight
		{
			get
			{
				return 0.7f;
			}
		}
	}
}
