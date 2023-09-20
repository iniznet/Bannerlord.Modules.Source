using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x02000182 RID: 386
	public abstract class KingdomCreationModel : GameModel
	{
		// Token: 0x1700068A RID: 1674
		// (get) Token: 0x06001964 RID: 6500
		public abstract int MinimumClanTierToCreateKingdom { get; }

		// Token: 0x1700068B RID: 1675
		// (get) Token: 0x06001965 RID: 6501
		public abstract int MinimumNumberOfSettlementsOwnedToCreateKingdom { get; }

		// Token: 0x1700068C RID: 1676
		// (get) Token: 0x06001966 RID: 6502
		public abstract int MinimumTroopCountToCreateKingdom { get; }

		// Token: 0x1700068D RID: 1677
		// (get) Token: 0x06001967 RID: 6503
		public abstract int MaximumNumberOfInitialPolicies { get; }

		// Token: 0x06001968 RID: 6504
		public abstract bool IsPlayerKingdomCreationPossible(out List<TextObject> explanations);

		// Token: 0x06001969 RID: 6505
		public abstract bool IsPlayerKingdomAbdicationPossible(out List<TextObject> explanations);

		// Token: 0x0600196A RID: 6506
		public abstract IEnumerable<CultureObject> GetAvailablePlayerKingdomCultures();
	}
}
