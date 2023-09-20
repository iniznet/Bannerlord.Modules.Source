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
	public static class ChangeKingdomAction
	{
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

		public static void ApplyByJoinToKingdom(Clan clan, Kingdom newKingdom, bool showNotification = true)
		{
			ChangeKingdomAction.ApplyInternal(clan, newKingdom, ChangeKingdomAction.ChangeKingdomActionDetail.JoinKingdom, 0, false, showNotification);
		}

		public static void ApplyByJoinToKingdomByDefection(Clan clan, Kingdom newKingdom, bool showNotification = true)
		{
			ChangeKingdomAction.ApplyInternal(clan, newKingdom, ChangeKingdomAction.ChangeKingdomActionDetail.JoinKingdomByDefection, 0, false, showNotification);
		}

		public static void ApplyByCreateKingdom(Clan clan, Kingdom newKingdom, bool showNotification = true)
		{
			ChangeKingdomAction.ApplyInternal(clan, newKingdom, ChangeKingdomAction.ChangeKingdomActionDetail.CreateKingdom, 0, false, showNotification);
		}

		public static void ApplyByLeaveKingdom(Clan clan, bool showNotification = true)
		{
			ChangeKingdomAction.ApplyInternal(clan, null, ChangeKingdomAction.ChangeKingdomActionDetail.LeaveKingdom, 0, false, showNotification);
		}

		public static void ApplyByLeaveWithRebellionAgainstKingdom(Clan clan, bool showNotification = true)
		{
			ChangeKingdomAction.ApplyInternal(clan, null, ChangeKingdomAction.ChangeKingdomActionDetail.LeaveWithRebellion, 0, false, showNotification);
		}

		public static void ApplyByJoinFactionAsMercenary(Clan clan, Kingdom newKingdom, int awardMultiplier = 50, bool showNotification = true)
		{
			ChangeKingdomAction.ApplyInternal(clan, newKingdom, ChangeKingdomAction.ChangeKingdomActionDetail.JoinAsMercenary, awardMultiplier, false, showNotification);
		}

		public static void ApplyByLeaveKingdomAsMercenary(Clan mercenaryClan, bool showNotification = true)
		{
			ChangeKingdomAction.ApplyInternal(mercenaryClan, null, ChangeKingdomAction.ChangeKingdomActionDetail.LeaveAsMercenary, 0, false, showNotification);
		}

		public static void ApplyByLeaveKingdomByClanDestruction(Clan clan, bool showNotification = true)
		{
			ChangeKingdomAction.ApplyInternal(clan, null, ChangeKingdomAction.ChangeKingdomActionDetail.LeaveByClanDestruction, 0, false, showNotification);
		}

		public static void ApplyByLeaveKingdomAsMercenaryWithKingDecision(Clan mercenaryClan, bool showNotification = true)
		{
			ChangeKingdomAction.ApplyInternal(mercenaryClan, null, ChangeKingdomAction.ChangeKingdomActionDetail.LeaveAsMercenary, 0, false, showNotification);
		}

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

		public const float PotentialSettlementsPerNobleEffect = 0.2f;

		public const float NewGainedFiefsValueForKingdomConstant = 0.1f;

		public const float LordsUnitStrengthValue = 20f;

		public const float MercenaryUnitStrengthValue = 5f;

		public const float MinimumNeededGoldForRecruitingMercenaries = 20000f;

		public enum ChangeKingdomActionDetail
		{
			JoinAsMercenary,
			JoinKingdom,
			JoinKingdomByDefection,
			LeaveKingdom,
			LeaveWithRebellion,
			LeaveAsMercenary,
			LeaveByClanDestruction,
			CreateKingdom
		}
	}
}
