using System;

namespace TaleWorlds.CampaignSystem.BarterSystem
{
	// Token: 0x02000409 RID: 1033
	public class GoldBarterGroup : BarterGroup
	{
		// Token: 0x17000CF9 RID: 3321
		// (get) Token: 0x06003D64 RID: 15716 RVA: 0x00126E54 File Offset: 0x00125054
		public override float AIDecisionWeight
		{
			get
			{
				return 0.6f;
			}
		}
	}
}
