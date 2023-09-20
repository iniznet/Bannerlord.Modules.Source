using System;

namespace TaleWorlds.CampaignSystem.BarterSystem
{
	public class ItemBarterGroup : BarterGroup
	{
		public override float AIDecisionWeight
		{
			get
			{
				return 0.5f;
			}
		}
	}
}
