using System;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	public abstract class CrimeModel : GameModel
	{
		public abstract int DeclareWarCrimeRatingThreshold { get; }

		public abstract float GetMaxCrimeRating();

		public abstract float GetMinAcceptableCrimeRating(IFaction faction);

		public abstract bool DoesPlayerHaveAnyCrimeRating(IFaction faction);

		public abstract bool IsPlayerCrimeRatingSevere(IFaction faction);

		public abstract bool IsPlayerCrimeRatingModerate(IFaction faction);

		public abstract bool IsPlayerCrimeRatingMild(IFaction faction);

		public abstract float GetCost(IFaction faction, CrimeModel.PaymentMethod paymentMethod, float minimumCrimeRating);

		public abstract ExplainedNumber GetDailyCrimeRatingChange(IFaction faction, bool includeDescriptions = false);

		[Flags]
		public enum PaymentMethod : uint
		{
			ExMachina = 4096U,
			Gold = 1U,
			Influence = 2U,
			Punishment = 4U,
			Execution = 8U
		}
	}
}
