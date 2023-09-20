using System;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;

namespace TaleWorlds.CampaignSystem.Actions
{
	public static class RemoveCompanionAction
	{
		private static void ApplyInternal(Clan clan, Hero companion, RemoveCompanionAction.RemoveCompanionDetail detail)
		{
			PartyBase partyBase;
			if (companion.PartyBelongedTo == null)
			{
				Settlement currentSettlement = companion.CurrentSettlement;
				partyBase = ((currentSettlement != null) ? currentSettlement.Party : null);
			}
			else
			{
				partyBase = companion.PartyBelongedTo.Party;
			}
			PartyBase partyBase2 = partyBase;
			companion.CompanionOf = null;
			if (partyBase2 != null && detail != RemoveCompanionAction.RemoveCompanionDetail.ByTurningToLord)
			{
				if (partyBase2.LeaderHero != companion)
				{
					partyBase2.MemberRoster.AddToCounts(companion.CharacterObject, -1, false, 0, 0, true, -1);
				}
				else
				{
					partyBase2.MemberRoster.AddToCounts(companion.CharacterObject, -1, false, 0, 0, true, -1);
					partyBase2.MobileParty.RemovePartyLeader();
					if (partyBase2.MemberRoster.Count == 0)
					{
						DestroyPartyAction.Apply(null, partyBase2.MobileParty);
					}
					else
					{
						DisbandPartyAction.StartDisband(partyBase2.MobileParty);
					}
				}
			}
			if (detail == RemoveCompanionAction.RemoveCompanionDetail.Fire)
			{
				companion.CompanionOf = null;
				if (companion.PartyBelongedToAsPrisoner != null)
				{
					EndCaptivityAction.ApplyByEscape(companion, null);
				}
				else
				{
					MakeHeroFugitiveAction.Apply(companion);
				}
				if (companion.IsWanderer)
				{
					companion.ResetEquipments();
				}
			}
			if (companion.GovernorOf != null)
			{
				ChangeGovernorAction.RemoveGovernorOf(companion);
			}
			CampaignEventDispatcher.Instance.OnCompanionRemoved(companion, detail);
		}

		public static void ApplyByFire(Clan clan, Hero companion)
		{
			RemoveCompanionAction.ApplyInternal(clan, companion, RemoveCompanionAction.RemoveCompanionDetail.Fire);
		}

		public static void ApplyAfterQuest(Clan clan, Hero companion)
		{
			RemoveCompanionAction.ApplyInternal(clan, companion, RemoveCompanionAction.RemoveCompanionDetail.AfterQuest);
		}

		public static void ApplyByDeath(Clan clan, Hero companion)
		{
			RemoveCompanionAction.ApplyInternal(clan, companion, RemoveCompanionAction.RemoveCompanionDetail.Death);
		}

		public static void ApplyByByTurningToLord(Clan clan, Hero companion)
		{
			RemoveCompanionAction.ApplyInternal(clan, companion, RemoveCompanionAction.RemoveCompanionDetail.ByTurningToLord);
		}

		public enum RemoveCompanionDetail
		{
			Fire,
			Death,
			AfterQuest,
			ByTurningToLord
		}
	}
}
