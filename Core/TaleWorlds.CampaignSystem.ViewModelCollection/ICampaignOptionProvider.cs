using System;
using System.Collections.Generic;

namespace TaleWorlds.CampaignSystem.ViewModelCollection
{
	// Token: 0x0200000B RID: 11
	public interface ICampaignOptionProvider
	{
		// Token: 0x06000088 RID: 136
		IEnumerable<ICampaignOptionData> GetGameplayCampaignOptions();

		// Token: 0x06000089 RID: 137
		IEnumerable<ICampaignOptionData> GetCharacterCreationCampaignOptions();
	}
}
