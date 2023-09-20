using System;
using System.Collections.Generic;
using Helpers;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Siege;

namespace TaleWorlds.CampaignSystem.Actions
{
	// Token: 0x0200042D RID: 1069
	public static class ChangeKingdomAction
	{
		// Token: 0x06003EA0 RID: 16032 RVA: 0x0012B2F0 File Offset: 0x001294F0
		private static void ApplyInternal(Clan clan, Kingdom newKingdom, ChangeKingdomAction.ChangeKingdomActionDetail detail, int awardMultiplier = 0, bool byRebellion = false, bool showNotification = true)
		{
			Kingdom kingdom = clan.Kingdom;
			bool flag = PlayerSiege.PlayerSiegeEvent != null;
			bool flag2 = MobileParty.MainParty.MapEvent != null;
			bool flag3 = PlayerEncounter.Current != null;
			clan.DebtToKingdom = 0;
			if (detail == ChangeKingdomAction.ChangeKingdomActionDetail.JoinKingdom || detail == ChangeKingdomAction.ChangeKingdomActionDetail.JoinAsMercenary || detail == ChangeKingdomAction.ChangeKingdomActionDetail.JoinKingdomByDefection)
			{
				FactionHelper.AdjustFactionStancesForClanJoiningKingdom(clan, newKingdom);
			}
			if (detail == ChangeKingdomAction.ChangeKingdomActionDetail.JoinKingdom || detail == ChangeKingdomAction.ChangeKingdomActionDetail.CreateKingdom || detail == ChangeKingdomAction.ChangeKingdomActionDetail.JoinKingdomByDefection)
			{
				if (clan.IsUnderMercenaryService)
				{
					clan.EndMercenaryService(false);
				}
				if (kingdom != null)
				{
					clan.ClanLeaveKingdom(!byRebellion);
				}
				clan.Kingdom = newKingdom;
				if (newKingdom != null && detail == ChangeKingdomAction.ChangeKingdomActionDetail.CreateKingdom)
				{
					ChangeRulingClanAction.Apply(newKingdom, clan);
				}
				CampaignEventDispatcher.Instance.OnClanChangedKingdom(clan, kingdom, newKingdom, detail, showNotification);
			}
			else if (detail == ChangeKingdomAction.ChangeKingdomActionDetail.JoinAsMercenary)
			{
				if (clan.IsUnderMercenaryService)
				{
					clan.EndMercenaryService(true);
				}
				clan.MercenaryAwardMultiplier = awardMultiplier;
				clan.Kingdom = newKingdom;
				clan.StartMercenaryService();
				if (clan == Clan.PlayerClan)
				{
					Campaign.Current.KingdomManager.PlayerMercenaryServiceNextRenewDay = Campaign.CurrentTime + 720f;
				}
				CampaignEventDispatcher.Instance.OnClanChangedKingdom(clan, kingdom, newKingdom, detail, showNotification);
			}
			else if (detail == ChangeKingdomAction.ChangeKingdomActionDetail.LeaveWithRebellion || detail == ChangeKingdomAction.ChangeKingdomActionDetail.LeaveKingdom || detail == ChangeKingdomAction.ChangeKingdomActionDetail.LeaveAsMercenary || detail == ChangeKingdomAction.ChangeKingdomActionDetail.LeaveByClanDestruction)
			{
				clan.Kingdom = null;
				if (detail == ChangeKingdomAction.ChangeKingdomActionDetail.LeaveAsMercenary)
				{
					CampaignEventDispatcher.Instance.OnClanChangedKingdom(clan, kingdom, null, detail, showNotification);
					clan.EndMercenaryService(true);
				}
				if (detail == ChangeKingdomAction.ChangeKingdomActionDetail.LeaveWithRebellion)
				{
					if (clan == Clan.PlayerClan)
					{
						foreach (Clan clan2 in kingdom.Clans)
						{
							ChangeRelationAction.ApplyRelationChangeBetweenHeroes(clan.Leader, clan2.Leader, -40, true);
						}
						DeclareWarAction.ApplyByRebellion(kingdom, clan);
					}
					CampaignEventDispatcher.Instance.OnClanChangedKingdom(clan, kingdom, null, detail, showNotification);
				}
				else if (detail == ChangeKingdomAction.ChangeKingdomActionDetail.LeaveKingdom)
				{
					if (clan == Clan.PlayerClan && !clan.IsEliminated)
					{
						foreach (Clan clan3 in kingdom.Clans)
						{
							ChangeRelationAction.ApplyRelationChangeBetweenHeroes(clan.Leader, clan3.Leader, -20, true);
						}
					}
					foreach (Settlement settlement in new List<Settlement>(clan.Settlements))
					{
						ChangeOwnerOfSettlementAction.ApplyByLeaveFaction(kingdom.Leader, settlement);
						foreach (Hero hero in new List<Hero>(settlement.HeroesWithoutParty))
						{
							if (hero.CurrentSettlement != null && hero.Clan == clan)
							{
								if (hero.PartyBelongedTo != null)
								{
									LeaveSettlementAction.ApplyForParty(hero.PartyBelongedTo);
									EnterSettlementAction.ApplyForParty(hero.PartyBelongedTo, clan.Leader.HomeSettlement);
								}
								else
								{
									LeaveSettlementAction.ApplyForCharacterOnly(hero);
									EnterSettlementAction.ApplyForCharacterOnly(hero, clan.Leader.HomeSettlement);
								}
							}
						}
					}
					CampaignEventDispatcher.Instance.OnClanChangedKingdom(clan, kingdom, null, detail, showNotification);
				}
			}
			if (detail == ChangeKingdomAction.ChangeKingdomActionDetail.LeaveAsMercenary || detail == ChangeKingdomAction.ChangeKingdomActionDetail.LeaveKingdom || detail == ChangeKingdomAction.ChangeKingdomActionDetail.LeaveWithRebellion)
			{
				foreach (StanceLink stanceLink in new List<StanceLink>(clan.Stances))
				{
					if (stanceLink.IsAtWar && !stanceLink.IsAtConstantWar)
					{
						IFaction faction = ((stanceLink.Faction1 == clan) ? stanceLink.Faction2 : stanceLink.Faction1);
						if (detail != ChangeKingdomAction.ChangeKingdomActionDetail.LeaveWithRebellion || clan != Clan.PlayerClan || faction != kingdom)
						{
							MakePeaceAction.Apply(clan, faction, 0);
							FactionHelper.FinishAllRelatedHostileActionsOfFactionToFaction(clan, faction);
							FactionHelper.FinishAllRelatedHostileActionsOfFactionToFaction(faction, clan);
						}
					}
				}
				ChangeKingdomAction.CheckEventsAndHandleMenu(flag, flag2, flag3, detail);
				ChangeKingdomAction.CheckIfPartyIconIsDirty(clan, kingdom);
			}
			foreach (WarPartyComponent warPartyComponent in clan.WarPartyComponents)
			{
				if (warPartyComponent.MobileParty.MapEvent == null)
				{
					warPartyComponent.MobileParty.Ai.SetMoveModeHold();
				}
			}
		}

		// Token: 0x06003EA1 RID: 16033 RVA: 0x0012B728 File Offset: 0x00129928
		public static void ApplyByJoinToKingdom(Clan clan, Kingdom newKingdom, bool showNotification = true)
		{
			ChangeKingdomAction.ApplyInternal(clan, newKingdom, ChangeKingdomAction.ChangeKingdomActionDetail.JoinKingdom, 0, false, showNotification);
		}

		// Token: 0x06003EA2 RID: 16034 RVA: 0x0012B735 File Offset: 0x00129935
		public static void ApplyByJoinToKingdomByDefection(Clan clan, Kingdom newKingdom, bool showNotification = true)
		{
			ChangeKingdomAction.ApplyInternal(clan, newKingdom, ChangeKingdomAction.ChangeKingdomActionDetail.JoinKingdomByDefection, 0, false, showNotification);
		}

		// Token: 0x06003EA3 RID: 16035 RVA: 0x0012B742 File Offset: 0x00129942
		public static void ApplyByCreateKingdom(Clan clan, Kingdom newKingdom, bool showNotification = true)
		{
			ChangeKingdomAction.ApplyInternal(clan, newKingdom, ChangeKingdomAction.ChangeKingdomActionDetail.CreateKingdom, 0, false, showNotification);
		}

		// Token: 0x06003EA4 RID: 16036 RVA: 0x0012B74F File Offset: 0x0012994F
		public static void ApplyByLeaveKingdom(Clan clan, bool showNotification = true)
		{
			ChangeKingdomAction.ApplyInternal(clan, null, ChangeKingdomAction.ChangeKingdomActionDetail.LeaveKingdom, 0, false, showNotification);
		}

		// Token: 0x06003EA5 RID: 16037 RVA: 0x0012B75C File Offset: 0x0012995C
		public static void ApplyByLeaveWithRebellionAgainstKingdom(Clan clan, bool showNotification = true)
		{
			ChangeKingdomAction.ApplyInternal(clan, null, ChangeKingdomAction.ChangeKingdomActionDetail.LeaveWithRebellion, 0, false, showNotification);
		}

		// Token: 0x06003EA6 RID: 16038 RVA: 0x0012B769 File Offset: 0x00129969
		public static void ApplyByJoinFactionAsMercenary(Clan clan, Kingdom newKingdom, int awardMultiplier = 50, bool showNotification = true)
		{
			ChangeKingdomAction.ApplyInternal(clan, newKingdom, ChangeKingdomAction.ChangeKingdomActionDetail.JoinAsMercenary, awardMultiplier, false, showNotification);
		}

		// Token: 0x06003EA7 RID: 16039 RVA: 0x0012B776 File Offset: 0x00129976
		public static void ApplyByLeaveKingdomAsMercenary(Clan mercenaryClan, bool showNotification = true)
		{
			ChangeKingdomAction.ApplyInternal(mercenaryClan, null, ChangeKingdomAction.ChangeKingdomActionDetail.LeaveAsMercenary, 0, false, showNotification);
		}

		// Token: 0x06003EA8 RID: 16040 RVA: 0x0012B783 File Offset: 0x00129983
		public static void ApplyByLeaveKingdomByClanDestruction(Clan clan, bool showNotification = true)
		{
			ChangeKingdomAction.ApplyInternal(clan, null, ChangeKingdomAction.ChangeKingdomActionDetail.LeaveByClanDestruction, 0, false, showNotification);
		}

		// Token: 0x06003EA9 RID: 16041 RVA: 0x0012B790 File Offset: 0x00129990
		public static void ApplyByLeaveKingdomAsMercenaryWithKingDecision(Clan mercenaryClan, bool showNotification = true)
		{
			ChangeKingdomAction.ApplyInternal(mercenaryClan, null, ChangeKingdomAction.ChangeKingdomActionDetail.LeaveAsMercenary, 0, false, showNotification);
		}

		// Token: 0x06003EAA RID: 16042 RVA: 0x0012B7A0 File Offset: 0x001299A0
		private static void CheckIfPartyIconIsDirty(Clan clan, Kingdom oldKingdom)
		{
			IFaction faction;
			if (clan.Kingdom == null)
			{
				faction = clan;
			}
			else
			{
				IFaction kingdom = clan.Kingdom;
				faction = kingdom;
			}
			IFaction faction2 = faction;
			IFaction faction3 = oldKingdom ?? clan;
			foreach (MobileParty mobileParty in MobileParty.All)
			{
				if (mobileParty.IsVisible && mobileParty.Party.Visuals != null && ((mobileParty.Party.Owner != null && mobileParty.Party.Owner.Clan == clan) || (clan == Clan.PlayerClan && ((!FactionManager.IsAtWarAgainstFaction(mobileParty.MapFaction, faction2) && FactionManager.IsAtWarAgainstFaction(mobileParty.MapFaction, faction3)) || (FactionManager.IsAtWarAgainstFaction(mobileParty.MapFaction, faction2) && !FactionManager.IsAtWarAgainstFaction(mobileParty.MapFaction, faction3))))))
				{
					mobileParty.Party.Visuals.SetMapIconAsDirty();
				}
			}
			foreach (Settlement settlement in clan.Settlements)
			{
				settlement.Party.Visuals.SetMapIconAsDirty();
			}
		}

		// Token: 0x06003EAB RID: 16043 RVA: 0x0012B8EC File Offset: 0x00129AEC
		private static void CheckEventsAndHandleMenu(bool thereWasAPlayerSiege, bool thereWasAPlayerMapEvent, bool thereWasAPlayerEncounter, ChangeKingdomAction.ChangeKingdomActionDetail detail)
		{
			if ((thereWasAPlayerSiege && PlayerSiege.PlayerSiegeEvent == null) || (thereWasAPlayerMapEvent && MobileParty.MainParty.MapEvent == null) || (thereWasAPlayerEncounter && PlayerEncounter.Current == null))
			{
				if (CampaignMission.Current != null)
				{
					Campaign.Current.GameMenuManager.SetNextMenu((detail == ChangeKingdomAction.ChangeKingdomActionDetail.LeaveAsMercenary) ? "hostile_action_end_by_leave_kingdom_as_mercenary" : "hostile_action_end_by_peace");
					return;
				}
				if (detail != ChangeKingdomAction.ChangeKingdomActionDetail.CreateKingdom)
				{
					GameMenu.ActivateGameMenu((detail == ChangeKingdomAction.ChangeKingdomActionDetail.LeaveAsMercenary) ? "hostile_action_end_by_leave_kingdom_as_mercenary" : "hostile_action_end_by_peace");
				}
			}
		}

		// Token: 0x040012BE RID: 4798
		public const float PotentialSettlementsPerNobleEffect = 0.2f;

		// Token: 0x040012BF RID: 4799
		public const float NewGainedFiefsValueForKingdomConstant = 0.1f;

		// Token: 0x040012C0 RID: 4800
		public const float LordsUnitStrengthValue = 20f;

		// Token: 0x040012C1 RID: 4801
		public const float MercenaryUnitStrengthValue = 5f;

		// Token: 0x040012C2 RID: 4802
		public const float MinimumNeededGoldForRecruitingMercenaries = 20000f;

		// Token: 0x02000756 RID: 1878
		public enum ChangeKingdomActionDetail
		{
			// Token: 0x04001E17 RID: 7703
			JoinAsMercenary,
			// Token: 0x04001E18 RID: 7704
			JoinKingdom,
			// Token: 0x04001E19 RID: 7705
			JoinKingdomByDefection,
			// Token: 0x04001E1A RID: 7706
			LeaveKingdom,
			// Token: 0x04001E1B RID: 7707
			LeaveWithRebellion,
			// Token: 0x04001E1C RID: 7708
			LeaveAsMercenary,
			// Token: 0x04001E1D RID: 7709
			LeaveByClanDestruction,
			// Token: 0x04001E1E RID: 7710
			CreateKingdom
		}
	}
}
