using System;

namespace TaleWorlds.CampaignSystem.BarterSystem
{
	public class DefaultsBarterGroup : BarterGroup
	{
		public override float AIDecisionWeight
		{
			get
			{
				return 0.75f;
			}
		}
	}
}
