using System;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x0200018B RID: 395
	public abstract class VolunteerModel : GameModel
	{
		// Token: 0x060019C8 RID: 6600
		public abstract int MaximumIndexHeroCanRecruitFromHero(Hero buyerHero, Hero sellerHero, int useValueAsRelation = -101);

		// Token: 0x060019C9 RID: 6601
		public abstract float GetDailyVolunteerProductionProbability(Hero hero, int index, Settlement settlement);

		// Token: 0x060019CA RID: 6602
		public abstract CharacterObject GetBasicVolunteer(Hero hero);

		// Token: 0x060019CB RID: 6603
		public abstract bool CanHaveRecruits(Hero hero);

		// Token: 0x170006A0 RID: 1696
		// (get) Token: 0x060019CC RID: 6604
		public abstract int MaxVolunteerTier { get; }
	}
}
