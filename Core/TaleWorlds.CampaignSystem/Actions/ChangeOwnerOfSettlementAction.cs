﻿using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.Settlements;

namespace TaleWorlds.CampaignSystem.Actions
{
	public static class ChangeOwnerOfSettlementAction
	{
		private static void ApplyInternal(Settlement settlement, Hero newOwner, Hero capturerHero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail detail)
		{
			Clan ownerClan = settlement.OwnerClan;
			Hero hero = ((ownerClan != null) ? ownerClan.Leader : null);
			if (settlement.IsFortification)
			{
				settlement.Town.OwnerClan = newOwner.Clan;
			}
			if (settlement.IsFortification)
			{
				if (settlement.Town.GarrisonParty == null)
				{
					settlement.AddGarrisonParty(false);
				}
				if (settlement.Town.Governor != null)
				{
					ChangeGovernorAction.RemoveGovernorOf(settlement.Town.Governor);
				}
			}
			settlement.Party.Visuals.SetMapIconAsDirty();
			foreach (Village village in settlement.BoundVillages)
			{
				village.Settlement.Party.Visuals.SetMapIconAsDirty();
				if (village.VillagerPartyComponent != null && newOwner != null)
				{
					foreach (MobileParty mobileParty in MobileParty.All)
					{
						if (mobileParty.MapEvent == null && mobileParty != MobileParty.MainParty && mobileParty.ShortTermTargetParty == village.VillagerPartyComponent.MobileParty && !mobileParty.MapFaction.IsAtWarWith(newOwner.MapFaction))
						{
							mobileParty.Ai.SetMoveModeHold();
						}
					}
				}
			}
			bool flag = (detail == ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail.BySiege || detail == ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail.ByRevolt || detail == ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail.ByClanDestruction || detail == ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail.ByLeaveFaction) && settlement.IsFortification;
			if (newOwner != null)
			{
				IFaction mapFaction = newOwner.MapFaction;
				if (settlement.Party.MapEvent != null && !settlement.Party.MapEvent.AttackerSide.LeaderParty.MapFaction.IsAtWarWith(mapFaction) && settlement.Party.MapEvent.Winner == null)
				{
					settlement.Party.MapEvent.DiplomaticallyFinished = true;
					List<MobileParty> list = new List<MobileParty>();
					foreach (MapEventParty mapEventParty in settlement.Party.MapEvent.AttackerSide.Parties)
					{
						if (mapEventParty.Party.MobileParty != null)
						{
							list.Add(mapEventParty.Party.MobileParty);
						}
					}
					foreach (WarPartyComponent warPartyComponent in settlement.MapFaction.WarPartyComponents)
					{
						MobileParty mobileParty2 = warPartyComponent.MobileParty;
						if (mobileParty2.DefaultBehavior == AiBehavior.DefendSettlement && mobileParty2.TargetSettlement == settlement && mobileParty2.CurrentSettlement == null)
						{
							mobileParty2.Ai.SetMoveModeHold();
						}
					}
					settlement.Party.MapEvent.Update();
					foreach (MobileParty mobileParty3 in list)
					{
						mobileParty3.Ai.SetMoveModeHold();
						if (mobileParty3 == MobileParty.MainParty)
						{
							GameMenu.ActivateGameMenu("hostile_action_end_by_peace");
						}
					}
				}
				if (settlement.Party.SiegeEvent != null && !settlement.Party.SiegeEvent.BesiegerCamp.BesiegerParty.MapFaction.IsAtWarWith(mapFaction))
				{
					if (settlement.Party.SiegeEvent.BesiegerCamp.BesiegerParty == MobileParty.MainParty)
					{
						GameMenu.ActivateGameMenu("siege_attacker_left");
					}
					settlement.Party.SiegeEvent.FinalizeSiegeEvent();
				}
				foreach (Clan clan in Clan.NonBanditFactions)
				{
					if (mapFaction == null || (clan.Kingdom == null && !clan.IsAtWarWith(mapFaction)) || (clan.Kingdom != null && !clan.Kingdom.IsAtWarWith(mapFaction)))
					{
						foreach (WarPartyComponent warPartyComponent2 in clan.WarPartyComponents)
						{
							MobileParty mobileParty4 = warPartyComponent2.MobileParty;
							if (mobileParty4.BesiegedSettlement != settlement && (mobileParty4.DefaultBehavior == AiBehavior.RaidSettlement || mobileParty4.DefaultBehavior == AiBehavior.BesiegeSettlement || mobileParty4.DefaultBehavior == AiBehavior.AssaultSettlement) && mobileParty4.TargetSettlement == settlement)
							{
								Army army = mobileParty4.Army;
								if (army != null)
								{
									army.FinishArmyObjective();
								}
								mobileParty4.Ai.SetMoveModeHold();
							}
						}
					}
				}
			}
			CampaignEventDispatcher.Instance.OnSettlementOwnerChanged(settlement, flag, newOwner, hero, capturerHero, detail);
		}

		public static void ApplyByDefault(Hero hero, Settlement settlement)
		{
			ChangeOwnerOfSettlementAction.ApplyInternal(settlement, hero, null, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail.Default);
		}

		public static void ApplyByKingDecision(Hero hero, Settlement settlement)
		{
			ChangeOwnerOfSettlementAction.ApplyInternal(settlement, hero, null, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail.ByKingDecision);
			if (settlement.Town != null)
			{
				settlement.Town.IsOwnerUnassigned = false;
			}
		}

		public static void ApplyBySiege(Hero newOwner, Hero capturerHero, Settlement settlement)
		{
			if (settlement.Town != null)
			{
				settlement.Town.LastCapturedBy = capturerHero.Clan;
			}
			ChangeOwnerOfSettlementAction.ApplyInternal(settlement, newOwner, capturerHero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail.BySiege);
		}

		public static void ApplyByLeaveFaction(Hero hero, Settlement settlement)
		{
			ChangeOwnerOfSettlementAction.ApplyInternal(settlement, hero, null, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail.ByLeaveFaction);
		}

		public static void ApplyByBarter(Hero hero, Settlement settlement)
		{
			ChangeOwnerOfSettlementAction.ApplyInternal(settlement, hero, null, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail.ByBarter);
		}

		public static void ApplyByRebellion(Hero hero, Settlement settlement)
		{
			ChangeOwnerOfSettlementAction.ApplyInternal(settlement, hero, hero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail.ByRebellion);
		}

		public static void ApplyByDestroyClan(Settlement settlement, Hero newOwner)
		{
			ChangeOwnerOfSettlementAction.ApplyInternal(settlement, newOwner, null, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail.ByClanDestruction);
		}

		public static void ApplyByGift(Settlement settlement, Hero newOwner)
		{
			ChangeOwnerOfSettlementAction.ApplyInternal(settlement, newOwner, null, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail.ByGift);
		}

		public enum ChangeOwnerOfSettlementDetail
		{
			Default,
			BySiege,
			ByBarter,
			ByRevolt,
			ByLeaveFaction,
			ByKingDecision,
			ByGift,
			ByRebellion,
			ByClanDestruction
		}
	}
}