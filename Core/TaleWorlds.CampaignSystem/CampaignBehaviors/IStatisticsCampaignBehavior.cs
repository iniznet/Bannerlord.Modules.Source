using System;
using System.Runtime.CompilerServices;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	// Token: 0x020003A4 RID: 932
	public interface IStatisticsCampaignBehavior : ICampaignBehavior
	{
		// Token: 0x06003788 RID: 14216
		void OnDefectionPersuasionSucess();

		// Token: 0x06003789 RID: 14217
		void OnPlayerAcceptedRansomOffer(int ransomPrice);

		// Token: 0x0600378A RID: 14218
		int GetHighestTournamentRank();

		// Token: 0x0600378B RID: 14219
		int GetNumberOfTournamentWins();

		// Token: 0x0600378C RID: 14220
		int GetNumberOfChildrenBorn();

		// Token: 0x0600378D RID: 14221
		int GetNumberOfPrisonersRecruited();

		// Token: 0x0600378E RID: 14222
		int GetNumberOfTroopsRecruited();

		// Token: 0x0600378F RID: 14223
		int GetNumberOfClansDefected();

		// Token: 0x06003790 RID: 14224
		int GetNumberOfIssuesSolved();

		// Token: 0x06003791 RID: 14225
		int GetTotalInfluenceEarned();

		// Token: 0x06003792 RID: 14226
		int GetTotalCrimeRatingGained();

		// Token: 0x06003793 RID: 14227
		int GetNumberOfBattlesWon();

		// Token: 0x06003794 RID: 14228
		int GetNumberOfBattlesLost();

		// Token: 0x06003795 RID: 14229
		int GetLargestBattleWonAsLeader();

		// Token: 0x06003796 RID: 14230
		int GetLargestArmyFormedByPlayer();

		// Token: 0x06003797 RID: 14231
		int GetNumberOfEnemyClansDestroyed();

		// Token: 0x06003798 RID: 14232
		int GetNumberOfHeroesKilledInBattle();

		// Token: 0x06003799 RID: 14233
		int GetNumberOfTroopsKnockedOrKilledAsParty();

		// Token: 0x0600379A RID: 14234
		int GetNumberOfTroopsKnockedOrKilledByPlayer();

		// Token: 0x0600379B RID: 14235
		int GetNumberOfHeroPrisonersTaken();

		// Token: 0x0600379C RID: 14236
		int GetNumberOfTroopPrisonersTaken();

		// Token: 0x0600379D RID: 14237
		int GetNumberOfTownsCaptured();

		// Token: 0x0600379E RID: 14238
		int GetNumberOfHideoutsCleared();

		// Token: 0x0600379F RID: 14239
		int GetNumberOfCastlesCaptured();

		// Token: 0x060037A0 RID: 14240
		int GetNumberOfVillagesRaided();

		// Token: 0x060037A1 RID: 14241
		int GetNumberOfCraftingPartsUnlocked();

		// Token: 0x060037A2 RID: 14242
		int GetNumberOfWeaponsCrafted();

		// Token: 0x060037A3 RID: 14243
		int GetNumberOfCraftingOrdersCompleted();

		// Token: 0x060037A4 RID: 14244
		int GetNumberOfCompanionsHired();

		// Token: 0x060037A5 RID: 14245
		ulong GetTotalTimePlayedInSeconds();

		// Token: 0x060037A6 RID: 14246
		ulong GetTotalDenarsEarned();

		// Token: 0x060037A7 RID: 14247
		ulong GetDenarsEarnedFromCaravans();

		// Token: 0x060037A8 RID: 14248
		ulong GetDenarsEarnedFromWorkshops();

		// Token: 0x060037A9 RID: 14249
		ulong GetDenarsEarnedFromRansoms();

		// Token: 0x060037AA RID: 14250
		ulong GetDenarsEarnedFromTaxes();

		// Token: 0x060037AB RID: 14251
		ulong GetDenarsEarnedFromTributes();

		// Token: 0x060037AC RID: 14252
		ulong GetDenarsPaidAsTributes();

		// Token: 0x060037AD RID: 14253
		CampaignTime GetTotalTimePlayed();

		// Token: 0x060037AE RID: 14254
		CampaignTime GetTimeSpentAsPrisoner();

		// Token: 0x060037AF RID: 14255
		ValueTuple<string, int> GetMostExpensiveItemCrafted();

		// Token: 0x060037B0 RID: 14256
		[return: TupleElementNames(new string[] { "name", "value" })]
		ValueTuple<string, int> GetCompanionWithMostKills();

		// Token: 0x060037B1 RID: 14257
		[return: TupleElementNames(new string[] { "name", "value" })]
		ValueTuple<string, int> GetCompanionWithMostIssuesSolved();
	}
}
