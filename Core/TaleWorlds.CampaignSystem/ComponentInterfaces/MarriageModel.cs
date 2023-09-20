using System;
using System.Collections.Generic;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x020001B1 RID: 433
	public abstract class MarriageModel : GameModel
	{
		// Token: 0x06001ACD RID: 6861
		public abstract bool IsCoupleSuitableForMarriage(Hero firstHero, Hero secondHero);

		// Token: 0x06001ACE RID: 6862
		public abstract int GetEffectiveRelationIncrease(Hero firstHero, Hero secondHero);

		// Token: 0x06001ACF RID: 6863
		public abstract Clan GetClanAfterMarriage(Hero firstHero, Hero secondHero);

		// Token: 0x06001AD0 RID: 6864
		public abstract bool IsSuitableForMarriage(Hero hero);

		// Token: 0x06001AD1 RID: 6865
		public abstract bool IsClanSuitableForMarriage(Clan clan);

		// Token: 0x06001AD2 RID: 6866
		public abstract float NpcCoupleMarriageChance(Hero firstHero, Hero secondHero);

		// Token: 0x06001AD3 RID: 6867
		public abstract bool ShouldNpcMarriageBetweenClansBeAllowed(Clan consideringClan, Clan targetClan);

		// Token: 0x06001AD4 RID: 6868
		public abstract List<Hero> GetAdultChildrenSuitableForMarriage(Hero hero);

		// Token: 0x170006F3 RID: 1779
		// (get) Token: 0x06001AD5 RID: 6869
		public abstract int MinimumMarriageAgeMale { get; }

		// Token: 0x170006F4 RID: 1780
		// (get) Token: 0x06001AD6 RID: 6870
		public abstract int MinimumMarriageAgeFemale { get; }
	}
}
