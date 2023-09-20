using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.Settlements;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	// Token: 0x02000399 RID: 921
	public interface IAlleyCampaignBehavior : ICampaignBehavior
	{
		// Token: 0x06003715 RID: 14101
		bool GetIsAlleyUnderAttack(Alley alley);

		// Token: 0x06003716 RID: 14102
		int GetPlayerOwnedAlleyTroopCount(Alley alley);

		// Token: 0x06003717 RID: 14103
		int GetResponseTimeLeftForAttackInDays(Alley alley);

		// Token: 0x06003718 RID: 14104
		void AbandonAlleyFromClanMenu(Alley alley);

		// Token: 0x06003719 RID: 14105
		Hero GetAssignedClanMemberOfAlley(Alley alley);

		// Token: 0x0600371A RID: 14106
		bool IsHeroAlleyLeaderOfAnyPlayerAlley(Hero hero);

		// Token: 0x0600371B RID: 14107
		List<Hero> GetAllAssignedClanMembersForOwnedAlleys();

		// Token: 0x0600371C RID: 14108
		void ChangeAlleyMember(Alley alley, Hero newAlleyLead);

		// Token: 0x0600371D RID: 14109
		void OnPlayerRetreatedFromMission();

		// Token: 0x0600371E RID: 14110
		void OnPlayerDiedInMission();
	}
}
