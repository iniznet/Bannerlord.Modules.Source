using System;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x0200019E RID: 414
	public abstract class CrimeModel : GameModel
	{
		// Token: 0x170006B9 RID: 1721
		// (get) Token: 0x06001A3F RID: 6719
		public abstract int DeclareWarCrimeRatingThreshold { get; }

		// Token: 0x06001A40 RID: 6720
		public abstract float GetMaxCrimeRating();

		// Token: 0x06001A41 RID: 6721
		public abstract float GetMinAcceptableCrimeRating(IFaction faction);

		// Token: 0x06001A42 RID: 6722
		public abstract bool DoesPlayerHaveAnyCrimeRating(IFaction faction);

		// Token: 0x06001A43 RID: 6723
		public abstract bool IsPlayerCrimeRatingSevere(IFaction faction);

		// Token: 0x06001A44 RID: 6724
		public abstract bool IsPlayerCrimeRatingModerate(IFaction faction);

		// Token: 0x06001A45 RID: 6725
		public abstract bool IsPlayerCrimeRatingMild(IFaction faction);

		// Token: 0x06001A46 RID: 6726
		public abstract float GetCost(IFaction faction, CrimeModel.PaymentMethod paymentMethod, float minimumCrimeRating);

		// Token: 0x06001A47 RID: 6727
		public abstract ExplainedNumber GetDailyCrimeRatingChange(IFaction faction, bool includeDescriptions = false);

		// Token: 0x0200055B RID: 1371
		[Flags]
		public enum PaymentMethod : uint
		{
			// Token: 0x04001697 RID: 5783
			ExMachina = 4096U,
			// Token: 0x04001698 RID: 5784
			Gold = 1U,
			// Token: 0x04001699 RID: 5785
			Influence = 2U,
			// Token: 0x0400169A RID: 5786
			Punishment = 4U,
			// Token: 0x0400169B RID: 5787
			Execution = 8U
		}
	}
}
