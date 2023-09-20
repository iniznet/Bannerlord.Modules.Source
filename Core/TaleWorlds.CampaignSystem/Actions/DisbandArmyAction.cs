using System;
using System.Linq;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Party;

namespace TaleWorlds.CampaignSystem.Actions
{
	// Token: 0x0200043C RID: 1084
	public static class DisbandArmyAction
	{
		// Token: 0x06003EE2 RID: 16098 RVA: 0x0012C880 File Offset: 0x0012AA80
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

		// Token: 0x06003EE3 RID: 16099 RVA: 0x0012C934 File Offset: 0x0012AB34
		public static void ApplyByReleasedByPlayerAfterBattle(Army army)
		{
			DisbandArmyAction.ApplyInternal(army, Army.ArmyDispersionReason.DismissalRequestedWithInfluence);
		}

		// Token: 0x06003EE4 RID: 16100 RVA: 0x0012C93D File Offset: 0x0012AB3D
		public static void ApplyByArmyLeaderIsDead(Army army)
		{
			DisbandArmyAction.ApplyInternal(army, Army.ArmyDispersionReason.ArmyLeaderIsDead);
		}

		// Token: 0x06003EE5 RID: 16101 RVA: 0x0012C947 File Offset: 0x0012AB47
		public static void ApplyByNotEnoughParty(Army army)
		{
			DisbandArmyAction.ApplyInternal(army, Army.ArmyDispersionReason.NotEnoughParty);
		}

		// Token: 0x06003EE6 RID: 16102 RVA: 0x0012C950 File Offset: 0x0012AB50
		public static void ApplyByObjectiveFinished(Army army)
		{
			DisbandArmyAction.ApplyInternal(army, Army.ArmyDispersionReason.ObjectiveFinished);
		}

		// Token: 0x06003EE7 RID: 16103 RVA: 0x0012C959 File Offset: 0x0012AB59
		public static void ApplyByPlayerTakenPrisoner(Army army)
		{
			DisbandArmyAction.ApplyInternal(army, Army.ArmyDispersionReason.PlayerTakenPrisoner);
		}

		// Token: 0x06003EE8 RID: 16104 RVA: 0x0012C962 File Offset: 0x0012AB62
		public static void ApplyByFoodProblem(Army army)
		{
			DisbandArmyAction.ApplyInternal(army, Army.ArmyDispersionReason.FoodProblem);
		}

		// Token: 0x06003EE9 RID: 16105 RVA: 0x0012C96C File Offset: 0x0012AB6C
		public static void ApplyByCohesionDepleted(Army army)
		{
			DisbandArmyAction.ApplyInternal(army, Army.ArmyDispersionReason.CohesionDepleted);
		}

		// Token: 0x06003EEA RID: 16106 RVA: 0x0012C975 File Offset: 0x0012AB75
		public static void ApplyByNoActiveWar(Army army)
		{
			DisbandArmyAction.ApplyInternal(army, Army.ArmyDispersionReason.NoActiveWar);
		}

		// Token: 0x06003EEB RID: 16107 RVA: 0x0012C97F File Offset: 0x0012AB7F
		public static void ApplyByUnknownReason(Army army)
		{
			DisbandArmyAction.ApplyInternal(army, Army.ArmyDispersionReason.Unknown);
		}

		// Token: 0x06003EEC RID: 16108 RVA: 0x0012C988 File Offset: 0x0012AB88
		public static void ApplyByLeaderPartyRemoved(Army army)
		{
			DisbandArmyAction.ApplyInternal(army, Army.ArmyDispersionReason.LeaderPartyRemoved);
		}
	}
}
