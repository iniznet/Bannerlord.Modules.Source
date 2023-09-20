﻿using System;
using System.Runtime.CompilerServices;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	public interface IStatisticsCampaignBehavior : ICampaignBehavior
	{
		void OnDefectionPersuasionSucess();

		void OnPlayerAcceptedRansomOffer(int ransomPrice);

		int GetHighestTournamentRank();

		int GetNumberOfTournamentWins();

		int GetNumberOfChildrenBorn();

		int GetNumberOfPrisonersRecruited();

		int GetNumberOfTroopsRecruited();

		int GetNumberOfClansDefected();

		int GetNumberOfIssuesSolved();

		int GetTotalInfluenceEarned();

		int GetTotalCrimeRatingGained();

		int GetNumberOfBattlesWon();

		int GetNumberOfBattlesLost();

		int GetLargestBattleWonAsLeader();

		int GetLargestArmyFormedByPlayer();

		int GetNumberOfEnemyClansDestroyed();

		int GetNumberOfHeroesKilledInBattle();

		int GetNumberOfTroopsKnockedOrKilledAsParty();

		int GetNumberOfTroopsKnockedOrKilledByPlayer();

		int GetNumberOfHeroPrisonersTaken();

		int GetNumberOfTroopPrisonersTaken();

		int GetNumberOfTownsCaptured();

		int GetNumberOfHideoutsCleared();

		int GetNumberOfCastlesCaptured();

		int GetNumberOfVillagesRaided();

		int GetNumberOfCraftingPartsUnlocked();

		int GetNumberOfWeaponsCrafted();

		int GetNumberOfCraftingOrdersCompleted();

		int GetNumberOfCompanionsHired();

		ulong GetTotalTimePlayedInSeconds();

		ulong GetTotalDenarsEarned();

		ulong GetDenarsEarnedFromCaravans();

		ulong GetDenarsEarnedFromWorkshops();

		ulong GetDenarsEarnedFromRansoms();

		ulong GetDenarsEarnedFromTaxes();

		ulong GetDenarsEarnedFromTributes();

		ulong GetDenarsPaidAsTributes();

		CampaignTime GetTotalTimePlayed();

		CampaignTime GetTimeSpentAsPrisoner();

		ValueTuple<string, int> GetMostExpensiveItemCrafted();

		[return: TupleElementNames(new string[] { "name", "value" })]
		ValueTuple<string, int> GetCompanionWithMostKills();

		[return: TupleElementNames(new string[] { "name", "value" })]
		ValueTuple<string, int> GetCompanionWithMostIssuesSolved();
	}
}
