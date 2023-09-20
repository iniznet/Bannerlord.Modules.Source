using System;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;

namespace TaleWorlds.CampaignSystem.Actions
{
	// Token: 0x02000451 RID: 1105
	public static class RemoveCompanionAction
	{
		// Token: 0x06003F40 RID: 16192 RVA: 0x0012ED14 File Offset: 0x0012CF14
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
				companion.ChangeState(Hero.CharacterStates.Fugitive);
				companion.CompanionOf = null;
				if (companion.PartyBelongedToAsPrisoner != null)
				{
					EndCaptivityAction.ApplyByEscape(companion, null);
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

		// Token: 0x06003F41 RID: 16193 RVA: 0x0012EE0B File Offset: 0x0012D00B
		public static void ApplyByFire(Clan clan, Hero companion)
		{
			RemoveCompanionAction.ApplyInternal(clan, companion, RemoveCompanionAction.RemoveCompanionDetail.Fire);
		}

		// Token: 0x06003F42 RID: 16194 RVA: 0x0012EE15 File Offset: 0x0012D015
		public static void ApplyAfterQuest(Clan clan, Hero companion)
		{
			RemoveCompanionAction.ApplyInternal(clan, companion, RemoveCompanionAction.RemoveCompanionDetail.AfterQuest);
		}

		// Token: 0x06003F43 RID: 16195 RVA: 0x0012EE1F File Offset: 0x0012D01F
		public static void ApplyByDeath(Clan clan, Hero companion)
		{
			RemoveCompanionAction.ApplyInternal(clan, companion, RemoveCompanionAction.RemoveCompanionDetail.Death);
		}

		// Token: 0x06003F44 RID: 16196 RVA: 0x0012EE29 File Offset: 0x0012D029
		public static void ApplyByByTurningToLord(Clan clan, Hero companion)
		{
			RemoveCompanionAction.ApplyInternal(clan, companion, RemoveCompanionAction.RemoveCompanionDetail.ByTurningToLord);
		}

		// Token: 0x02000767 RID: 1895
		public enum RemoveCompanionDetail
		{
			// Token: 0x04001E6B RID: 7787
			Fire,
			// Token: 0x04001E6C RID: 7788
			Death,
			// Token: 0x04001E6D RID: 7789
			AfterQuest,
			// Token: 0x04001E6E RID: 7790
			ByTurningToLord
		}
	}
}
