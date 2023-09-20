using System;

namespace TaleWorlds.CampaignSystem.BarterSystem
{
	// Token: 0x0200040D RID: 1037
	public class OtherBarterGroup : BarterGroup
	{
		// Token: 0x17000CFD RID: 3325
		// (get) Token: 0x06003D6C RID: 15724 RVA: 0x00126E90 File Offset: 0x00125090
		public override float AIDecisionWeight
		{
			get
			{
				return 0.25f;
			}
		}
	}
}
