using System;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x020001C8 RID: 456
	public abstract class ExecutionRelationModel : GameModel
	{
		// Token: 0x1700070D RID: 1805
		// (get) Token: 0x06001B69 RID: 7017
		public abstract int HeroKillingHeroClanRelationPenalty { get; }

		// Token: 0x1700070E RID: 1806
		// (get) Token: 0x06001B6A RID: 7018
		public abstract int HeroKillingHeroFriendRelationPenalty { get; }

		// Token: 0x1700070F RID: 1807
		// (get) Token: 0x06001B6B RID: 7019
		public abstract int PlayerExecutingHeroFactionRelationPenaltyDishonorable { get; }

		// Token: 0x17000710 RID: 1808
		// (get) Token: 0x06001B6C RID: 7020
		public abstract int PlayerExecutingHeroClanRelationPenaltyDishonorable { get; }

		// Token: 0x17000711 RID: 1809
		// (get) Token: 0x06001B6D RID: 7021
		public abstract int PlayerExecutingHeroFriendRelationPenaltyDishonorable { get; }

		// Token: 0x17000712 RID: 1810
		// (get) Token: 0x06001B6E RID: 7022
		public abstract int PlayerExecutingHeroHonorPenalty { get; }

		// Token: 0x17000713 RID: 1811
		// (get) Token: 0x06001B6F RID: 7023
		public abstract int PlayerExecutingHeroFactionRelationPenalty { get; }

		// Token: 0x17000714 RID: 1812
		// (get) Token: 0x06001B70 RID: 7024
		public abstract int PlayerExecutingHeroHonorableNobleRelationPenalty { get; }

		// Token: 0x17000715 RID: 1813
		// (get) Token: 0x06001B71 RID: 7025
		public abstract int PlayerExecutingHeroClanRelationPenalty { get; }

		// Token: 0x17000716 RID: 1814
		// (get) Token: 0x06001B72 RID: 7026
		public abstract int PlayerExecutingHeroFriendRelationPenalty { get; }

		// Token: 0x06001B73 RID: 7027
		public abstract int GetRelationChangeForExecutingHero(Hero victim, Hero hero, out bool showQuickNotification);
	}
}
