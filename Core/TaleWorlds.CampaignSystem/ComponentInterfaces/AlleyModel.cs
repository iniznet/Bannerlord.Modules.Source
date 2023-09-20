using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x020001CD RID: 461
	public abstract class AlleyModel : GameModel
	{
		// Token: 0x17000718 RID: 1816
		// (get) Token: 0x06001B81 RID: 7041
		public abstract CampaignTime DestroyAlleyAfterDaysWhenLeaderIsDeath { get; }

		// Token: 0x17000719 RID: 1817
		// (get) Token: 0x06001B82 RID: 7042
		public abstract int MinimumTroopCountInPlayerOwnedAlley { get; }

		// Token: 0x1700071A RID: 1818
		// (get) Token: 0x06001B83 RID: 7043
		public abstract int MaximumTroopCountInPlayerOwnedAlley { get; }

		// Token: 0x1700071B RID: 1819
		// (get) Token: 0x06001B84 RID: 7044
		public abstract float GetDailyCrimeRatingOfAlley { get; }

		// Token: 0x06001B85 RID: 7045
		public abstract float GetDailyXpGainForAssignedClanMember(Hero assignedHero);

		// Token: 0x06001B86 RID: 7046
		public abstract float GetDailyXpGainForMainHero();

		// Token: 0x06001B87 RID: 7047
		public abstract float GetInitialXpGainForMainHero();

		// Token: 0x06001B88 RID: 7048
		public abstract float GetXpGainAfterSuccessfulAlleyDefenseForMainHero();

		// Token: 0x06001B89 RID: 7049
		public abstract TroopRoster GetTroopsOfAIOwnedAlley(Alley alley);

		// Token: 0x06001B8A RID: 7050
		public abstract TroopRoster GetTroopsOfAlleyForBattleMission(Alley alley);

		// Token: 0x06001B8B RID: 7051
		public abstract int GetDailyIncomeOfAlley(Alley alley);

		// Token: 0x06001B8C RID: 7052
		public abstract List<ValueTuple<Hero, DefaultAlleyModel.AlleyMemberAvailabilityDetail>> GetClanMembersAndAvailabilityDetailsForLeadingAnAlley(Alley alley);

		// Token: 0x06001B8D RID: 7053
		public abstract TroopRoster GetTroopsToRecruitFromAlleyDependingOnAlleyRandom(Alley alley, float random);

		// Token: 0x06001B8E RID: 7054
		public abstract TextObject GetDisabledReasonTextForHero(Hero hero, Alley alley, DefaultAlleyModel.AlleyMemberAvailabilityDetail detail);

		// Token: 0x06001B8F RID: 7055
		public abstract float GetAlleyAttackResponseTimeInDays(TroopRoster troopRoster);
	}
}
