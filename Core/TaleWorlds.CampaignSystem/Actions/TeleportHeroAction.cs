using System;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.Actions
{
	// Token: 0x02000459 RID: 1113
	public static class TeleportHeroAction
	{
		// Token: 0x06003F62 RID: 16226 RVA: 0x0012FD50 File Offset: 0x0012DF50
		private static void ApplyInternal(Hero hero, Settlement targetSettlement, MobileParty targetParty, TeleportHeroAction.TeleportationDetail detail)
		{
			CampaignEventDispatcher.Instance.OnHeroTeleportationRequested(hero, targetSettlement, targetParty, detail);
			switch (detail)
			{
			case TeleportHeroAction.TeleportationDetail.ImmediateTeleportToSettlement:
				if (targetSettlement != null)
				{
					if (hero.IsTraveling)
					{
						hero.ChangeState(Hero.CharacterStates.Active);
					}
					if (hero.CurrentSettlement != null)
					{
						LeaveSettlementAction.ApplyForCharacterOnly(hero);
					}
					if (hero.PartyBelongedTo != null)
					{
						if (!hero.PartyBelongedTo.IsActive || hero.PartyBelongedTo.IsCurrentlyEngagingParty || hero.PartyBelongedTo.MapEvent != null)
						{
							return;
						}
						hero.PartyBelongedTo.MemberRoster.RemoveTroop(hero.CharacterObject, 1, default(UniqueTroopDescriptor), 0);
					}
					EnterSettlementAction.ApplyForCharacterOnly(hero, targetSettlement);
					return;
				}
				break;
			case TeleportHeroAction.TeleportationDetail.ImmediateTeleportToParty:
				if (hero.IsTraveling)
				{
					hero.ChangeState(Hero.CharacterStates.Active);
				}
				targetParty.MemberRoster.AddToCounts(hero.CharacterObject, 1, false, 0, 0, true, -1);
				return;
			case TeleportHeroAction.TeleportationDetail.ImmediateTeleportToPartyAsPartyLeader:
				if (hero.IsTraveling)
				{
					hero.ChangeState(Hero.CharacterStates.Active);
				}
				targetParty.MemberRoster.AddToCounts(hero.CharacterObject, 1, false, 0, 0, true, -1);
				targetParty.ChangePartyLeader(hero);
				targetParty.PartyComponent.ClearCachedName();
				targetParty.SetCustomName(null);
				targetParty.Party.Visuals.SetMapIconAsDirty();
				if (targetParty.IsDisbanding)
				{
					DisbandPartyAction.CancelDisband(targetParty);
				}
				if (targetParty.Ai.DoNotMakeNewDecisions)
				{
					targetParty.Ai.SetDoNotMakeNewDecisions(false);
					return;
				}
				break;
			case TeleportHeroAction.TeleportationDetail.DelayedTeleportToSettlement:
			case TeleportHeroAction.TeleportationDetail.DelayedTeleportToParty:
			case TeleportHeroAction.TeleportationDetail.DelayedTeleportToSettlementAsGovernor:
			case TeleportHeroAction.TeleportationDetail.DelayedTeleportToPartyAsPartyLeader:
				if (detail == TeleportHeroAction.TeleportationDetail.DelayedTeleportToSettlement && hero.CurrentSettlement == targetSettlement)
				{
					TeleportHeroAction.ApplyImmediateTeleportToSettlement(hero, targetSettlement);
					return;
				}
				if (hero.GovernorOf != null)
				{
					ChangeGovernorAction.RemoveGovernorOf(hero);
				}
				if (hero.CurrentSettlement != null && hero.CurrentSettlement != targetSettlement)
				{
					LeaveSettlementAction.ApplyForCharacterOnly(hero);
				}
				if (hero.PartyBelongedTo != null)
				{
					if (!hero.PartyBelongedTo.IsActive || (hero.PartyBelongedTo.IsCurrentlyEngagingParty && hero.PartyBelongedTo.MapEvent != null))
					{
						return;
					}
					hero.PartyBelongedTo.MemberRoster.RemoveTroop(hero.CharacterObject, 1, default(UniqueTroopDescriptor), 0);
				}
				if (detail == TeleportHeroAction.TeleportationDetail.DelayedTeleportToPartyAsPartyLeader)
				{
					TextObject textObject = new TextObject("{=ithcVNfA}{CLAN_NAME}{.o} Party", null);
					textObject.SetTextVariable("CLAN_NAME", (targetParty.ActualClan != null) ? targetParty.ActualClan.Name : CampaignData.NeutralFaction.Name);
					targetParty.SetCustomName(textObject);
				}
				hero.ChangeState(Hero.CharacterStates.Traveling);
				break;
			default:
				return;
			}
		}

		// Token: 0x06003F63 RID: 16227 RVA: 0x0012FF85 File Offset: 0x0012E185
		public static void ApplyImmediateTeleportToSettlement(Hero heroToBeMoved, Settlement targetSettlement)
		{
			TeleportHeroAction.ApplyInternal(heroToBeMoved, targetSettlement, null, TeleportHeroAction.TeleportationDetail.ImmediateTeleportToSettlement);
		}

		// Token: 0x06003F64 RID: 16228 RVA: 0x0012FF90 File Offset: 0x0012E190
		public static void ApplyImmediateTeleportToParty(Hero heroToBeMoved, MobileParty party)
		{
			TeleportHeroAction.ApplyInternal(heroToBeMoved, null, party, TeleportHeroAction.TeleportationDetail.ImmediateTeleportToParty);
		}

		// Token: 0x06003F65 RID: 16229 RVA: 0x0012FF9B File Offset: 0x0012E19B
		public static void ApplyImmediateTeleportToPartyAsPartyLeader(Hero heroToBeMoved, MobileParty party)
		{
			TeleportHeroAction.ApplyInternal(heroToBeMoved, null, party, TeleportHeroAction.TeleportationDetail.ImmediateTeleportToPartyAsPartyLeader);
		}

		// Token: 0x06003F66 RID: 16230 RVA: 0x0012FFA6 File Offset: 0x0012E1A6
		public static void ApplyDelayedTeleportToSettlement(Hero heroToBeMoved, Settlement targetSettlement)
		{
			TeleportHeroAction.ApplyInternal(heroToBeMoved, targetSettlement, null, TeleportHeroAction.TeleportationDetail.DelayedTeleportToSettlement);
		}

		// Token: 0x06003F67 RID: 16231 RVA: 0x0012FFB1 File Offset: 0x0012E1B1
		public static void ApplyDelayedTeleportToParty(Hero heroToBeMoved, MobileParty party)
		{
			TeleportHeroAction.ApplyInternal(heroToBeMoved, null, party, TeleportHeroAction.TeleportationDetail.DelayedTeleportToParty);
		}

		// Token: 0x06003F68 RID: 16232 RVA: 0x0012FFBC File Offset: 0x0012E1BC
		public static void ApplyDelayedTeleportToSettlementAsGovernor(Hero heroToBeMoved, Settlement targetSettlement)
		{
			TeleportHeroAction.ApplyInternal(heroToBeMoved, targetSettlement, null, TeleportHeroAction.TeleportationDetail.DelayedTeleportToSettlementAsGovernor);
		}

		// Token: 0x06003F69 RID: 16233 RVA: 0x0012FFC7 File Offset: 0x0012E1C7
		public static void ApplyDelayedTeleportToPartyAsPartyLeader(Hero heroToBeMoved, MobileParty party)
		{
			TeleportHeroAction.ApplyInternal(heroToBeMoved, null, party, TeleportHeroAction.TeleportationDetail.DelayedTeleportToPartyAsPartyLeader);
		}

		// Token: 0x0200076D RID: 1901
		public enum TeleportationDetail
		{
			// Token: 0x04001E8D RID: 7821
			ImmediateTeleportToSettlement,
			// Token: 0x04001E8E RID: 7822
			ImmediateTeleportToParty,
			// Token: 0x04001E8F RID: 7823
			ImmediateTeleportToPartyAsPartyLeader,
			// Token: 0x04001E90 RID: 7824
			DelayedTeleportToSettlement,
			// Token: 0x04001E91 RID: 7825
			DelayedTeleportToParty,
			// Token: 0x04001E92 RID: 7826
			DelayedTeleportToSettlementAsGovernor,
			// Token: 0x04001E93 RID: 7827
			DelayedTeleportToPartyAsPartyLeader
		}
	}
}
