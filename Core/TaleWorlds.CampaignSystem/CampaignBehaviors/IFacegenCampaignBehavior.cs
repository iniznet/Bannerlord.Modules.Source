using System;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	// Token: 0x0200039E RID: 926
	public interface IFacegenCampaignBehavior : ICampaignBehavior
	{
		// Token: 0x06003735 RID: 14133
		IFaceGeneratorCustomFilter GetFaceGenFilter();
	}
}
