using System;
using System.Linq;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Party;

namespace TaleWorlds.CampaignSystem.Actions
{
	public static class DisbandArmyAction
	{
		private static void ApplyInternal(Army army, Army.ArmyDispersionReason reason)
		{
			if (reason == Army.ArmyDispersionReason.DismissalRequestedWithInfluence)
			{
				DiplomacyModel diplomacyModel = Campaign.Current.Models.DiplomacyModel;
				ChangeClanInfluenceAction.Apply(Clan.PlayerClan, (float)(-(float)diplomacyModel.GetInfluenceCostOfDisbandingArmy()));
				foreach (MobileParty mobileParty in army.Parties.ToList<MobileParty>())
				{
					if (mobileParty != MobileParty.MainParty && mobileParty.LeaderHero != null)
					{
						ChangeRelationAction.ApplyPlayerRelation(mobileParty.LeaderHero, diplomacyModel.GetRelationCostOfDisbandingArmy(mobileParty == mobileParty.Army.LeaderParty), true, true);
					}
				}
			}
			army.DisperseInternal(reason);
		}

		public static void ApplyByReleasedByPlayerAfterBattle(Army army)
		{
			DisbandArmyAction.ApplyInternal(army, Army.ArmyDispersionReason.DismissalRequestedWithInfluence);
		}

		public static void ApplyByArmyLeaderIsDead(Army army)
		{
			DisbandArmyAction.ApplyInternal(army, Army.ArmyDispersionReason.ArmyLeaderIsDead);
		}

		public static void ApplyByNotEnoughParty(Army army)
		{
			DisbandArmyAction.ApplyInternal(army, Army.ArmyDispersionReason.NotEnoughParty);
		}

		public static void ApplyByObjectiveFinished(Army army)
		{
			DisbandArmyAction.ApplyInternal(army, Army.ArmyDispersionReason.ObjectiveFinished);
		}

		public static void ApplyByPlayerTakenPrisoner(Army army)
		{
			DisbandArmyAction.ApplyInternal(army, Army.ArmyDispersionReason.PlayerTakenPrisoner);
		}

		public static void ApplyByFoodProblem(Army army)
		{
			DisbandArmyAction.ApplyInternal(army, Army.ArmyDispersionReason.FoodProblem);
		}

		public static void ApplyByCohesionDepleted(Army army)
		{
			DisbandArmyAction.ApplyInternal(army, Army.ArmyDispersionReason.CohesionDepleted);
		}

		public static void ApplyByNoActiveWar(Army army)
		{
			DisbandArmyAction.ApplyInternal(army, Army.ArmyDispersionReason.NoActiveWar);
		}

		public static void ApplyByUnknownReason(Army army)
		{
			DisbandArmyAction.ApplyInternal(army, Army.ArmyDispersionReason.Unknown);
		}

		public static void ApplyByLeaderPartyRemoved(Army army)
		{
			DisbandArmyAction.ApplyInternal(army, Army.ArmyDispersionReason.LeaderPartyRemoved);
		}
	}
}
