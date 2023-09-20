using System;

namespace TaleWorlds.CampaignSystem.BarterSystem
{
	// Token: 0x0200040C RID: 1036
	public class PrisonerBarterGroup : BarterGroup
	{
		// Token: 0x17000CFC RID: 3324
		// (get) Token: 0x06003D6A RID: 15722 RVA: 0x00126E81 File Offset: 0x00125081
		public override float AIDecisionWeight
		{
			get
			{
				return 0.7f;
			}
		}
	}
}
