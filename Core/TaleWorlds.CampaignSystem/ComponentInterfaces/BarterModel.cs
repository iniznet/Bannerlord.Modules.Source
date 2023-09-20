using System;
using TaleWorlds.CampaignSystem.BarterSystem.Barterables;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x02000178 RID: 376
	public abstract class BarterModel : GameModel
	{
		// Token: 0x17000684 RID: 1668
		// (get) Token: 0x06001920 RID: 6432
		public abstract int BarterCooldownWithHeroInDays { get; }

		// Token: 0x17000685 RID: 1669
		// (get) Token: 0x06001921 RID: 6433
		public abstract float MaximumPercentageOfNpcGoldToSpendAtBarter { get; }

		// Token: 0x06001922 RID: 6434
		public abstract int CalculateOverpayRelationIncreaseCosts(Hero hero, float overpayAmount);

		// Token: 0x06001923 RID: 6435
		public abstract ExplainedNumber GetBarterPenalty(IFaction faction, ItemBarterable itemBarterable, Hero otherHero, PartyBase otherParty);
	}
}
