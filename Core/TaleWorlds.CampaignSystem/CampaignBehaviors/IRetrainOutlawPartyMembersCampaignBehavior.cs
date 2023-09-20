using System;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	// Token: 0x020003A2 RID: 930
	public interface IRetrainOutlawPartyMembersCampaignBehavior : ICampaignBehavior
	{
		// Token: 0x06003741 RID: 14145
		int GetRetrainedNumber(CharacterObject character);

		// Token: 0x06003742 RID: 14146
		void SetRetrainedNumber(CharacterObject character, int number);
	}
}
