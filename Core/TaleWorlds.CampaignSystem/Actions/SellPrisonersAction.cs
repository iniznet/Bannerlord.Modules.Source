using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.Actions
{
	public static class SellPrisonersAction
	{
		private static void ApplyInternal(MobileParty sellerParty, TroopRoster prisoners, Settlement currentSettlement, bool applyGoldChange, SellPrisonersAction.SellPrisonersDetail sellPrisonersDetail)
		{
			TroopRoster troopRoster = ((sellPrisonersDetail == SellPrisonersAction.SellPrisonersDetail.SellSettlementPrisoners) ? currentSettlement.Party.PrisonRoster : sellerParty.PrisonRoster);
			TroopRoster troopRoster2 = TroopRoster.CreateDummyTroopRoster();
			int num = 0;
			List<string> list = Campaign.Current.GetCampaignBehavior<IViewDataTracker>().GetPartyPrisonerLocks().ToList<string>();
			for (int i = prisoners.Count - 1; i >= 0; i--)
			{
				TroopRosterElement elementCopyAtIndex = prisoners.GetElementCopyAtIndex(i);
				if (elementCopyAtIndex.Character != CharacterObject.PlayerCharacter)
				{
					int woundedNumber = elementCopyAtIndex.WoundedNumber;
					int num2 = elementCopyAtIndex.Number - woundedNumber;
					if ((sellPrisonersDetail == SellPrisonersAction.SellPrisonersDetail.SellAllPrisoners || sellPrisonersDetail == SellPrisonersAction.SellPrisonersDetail.SellSettlementPrisoners) && !elementCopyAtIndex.Character.IsHero && !list.Contains(elementCopyAtIndex.Character.StringId))
					{
						troopRoster.AddToCounts(elementCopyAtIndex.Character, -num2 - woundedNumber, false, -woundedNumber, 0, true, -1);
						if (applyGoldChange)
						{
							int num3 = Campaign.Current.Models.RansomValueCalculationModel.PrisonerRansomValue(elementCopyAtIndex.Character, (sellerParty != null) ? sellerParty.LeaderHero : null);
							num += (num2 + woundedNumber) * num3;
						}
					}
					if (elementCopyAtIndex.Character.IsHero)
					{
						if (((sellerParty != null) ? sellerParty.LeaderHero : null) == Hero.MainHero)
						{
							EndCaptivityAction.ApplyByRansom(elementCopyAtIndex.Character.HeroObject, null);
							num += Campaign.Current.Models.RansomValueCalculationModel.PrisonerRansomValue(elementCopyAtIndex.Character, sellerParty.LeaderHero);
						}
						else
						{
							troopRoster.RemoveTroop(elementCopyAtIndex.Character, 1, default(UniqueTroopDescriptor), 0);
							if (currentSettlement.MapFaction.IsAtWarWith(elementCopyAtIndex.Character.HeroObject.MapFaction))
							{
								troopRoster.AddToCounts(elementCopyAtIndex.Character, num2 + woundedNumber, false, woundedNumber, 0, true, -1);
								CampaignEventDispatcher.Instance.OnPrisonersChangeInSettlement(currentSettlement, null, elementCopyAtIndex.Character.HeroObject, false);
							}
							else
							{
								EndCaptivityAction.ApplyByPeace(elementCopyAtIndex.Character.HeroObject, null);
								num += Campaign.Current.Models.RansomValueCalculationModel.PrisonerRansomValue(elementCopyAtIndex.Character, sellerParty.LeaderHero);
							}
						}
					}
					troopRoster2.AddToCounts(elementCopyAtIndex.Character, num2 + woundedNumber, false, 0, 0, true, -1);
				}
			}
			if (applyGoldChange)
			{
				if (sellPrisonersDetail == SellPrisonersAction.SellPrisonersDetail.SellAllPrisoners)
				{
					if (sellerParty.LeaderHero != null)
					{
						GiveGoldAction.ApplyBetweenCharacters(null, sellerParty.LeaderHero, num, false);
					}
					else if (sellerParty.Party.Owner != null)
					{
						GiveGoldAction.ApplyBetweenCharacters(null, sellerParty.Party.Owner, num, false);
					}
				}
				else if (sellPrisonersDetail == SellPrisonersAction.SellPrisonersDetail.SellSettlementPrisoners)
				{
					GiveGoldAction.ApplyForPartyToSettlement(null, currentSettlement, num, currentSettlement.OwnerClan != Clan.PlayerClan);
				}
			}
			if (sellPrisonersDetail != SellPrisonersAction.SellPrisonersDetail.SellSettlementPrisoners)
			{
				SkillLevelingManager.OnPrisonerSell(sellerParty, (float)troopRoster2.TotalManCount);
				CampaignEventDispatcher.Instance.OnPrisonerSold(sellerParty, troopRoster2, currentSettlement);
			}
		}

		public static void ApplyForAllPrisoners(MobileParty sellerParty, TroopRoster prisoners, Settlement currentSettlement, bool applyGoldChange = true)
		{
			SellPrisonersAction.ApplyInternal(sellerParty, prisoners, currentSettlement, applyGoldChange, SellPrisonersAction.SellPrisonersDetail.SellAllPrisoners);
		}

		public static void ApplyForSelectedPrisoners(MobileParty sellerParty, TroopRoster prisoners, Settlement currentSettlement)
		{
			SellPrisonersAction.ApplyInternal(sellerParty, prisoners, currentSettlement, false, SellPrisonersAction.SellPrisonersDetail.SellSelectedPrisoners);
		}

		public static void ApplyForSettlementPrisoners(Settlement sellerSettlement, TroopRoster soldPrisoners, bool applyGoldChange = true)
		{
			SellPrisonersAction.ApplyInternal(null, soldPrisoners, sellerSettlement, applyGoldChange, SellPrisonersAction.SellPrisonersDetail.SellSettlementPrisoners);
		}

		private enum SellPrisonersDetail
		{
			None,
			SellAllPrisoners,
			SellSelectedPrisoners,
			SellSettlementPrisoners
		}
	}
}
