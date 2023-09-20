using System;
using System.Collections.Generic;
using Helpers;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.Actions
{
	public static class MarriageAction
	{
		private static void ApplyInternal(Hero firstHero, Hero secondHero, bool showNotification)
		{
			if (!Campaign.Current.Models.MarriageModel.IsCoupleSuitableForMarriage(firstHero, secondHero))
			{
				Debug.Print("MarriageAction.Apply() called for not suitable couple: " + firstHero.StringId + " and " + secondHero.StringId, 0, Debug.DebugColor.White, 17592186044416UL);
				return;
			}
			firstHero.Spouse = secondHero;
			secondHero.Spouse = firstHero;
			ChangeRelationAction.ApplyRelationChangeBetweenHeroes(firstHero, secondHero, Campaign.Current.Models.MarriageModel.GetEffectiveRelationIncrease(firstHero, secondHero), false);
			Clan clanAfterMarriage = Campaign.Current.Models.MarriageModel.GetClanAfterMarriage(firstHero, secondHero);
			if (firstHero.Clan != clanAfterMarriage)
			{
				Clan clan = firstHero.Clan;
				if (firstHero.GovernorOf != null)
				{
					ChangeGovernorAction.RemoveGovernorOf(firstHero);
				}
				if (firstHero.PartyBelongedTo != null)
				{
					MobileParty partyBelongedTo = firstHero.PartyBelongedTo;
					if (clan.Kingdom != clanAfterMarriage.Kingdom)
					{
						if (firstHero.PartyBelongedTo.Army != null)
						{
							if (firstHero.PartyBelongedTo.Army.LeaderParty == firstHero.PartyBelongedTo)
							{
								DisbandArmyAction.ApplyByUnknownReason(firstHero.PartyBelongedTo.Army);
							}
							else
							{
								firstHero.PartyBelongedTo.Army = null;
							}
						}
						IFaction faction = clanAfterMarriage.Kingdom;
						FactionHelper.FinishAllRelatedHostileActionsOfNobleToFaction(firstHero, faction ?? clanAfterMarriage);
					}
					if (partyBelongedTo.Party.IsActive && partyBelongedTo.Party.Owner == firstHero)
					{
						DisbandPartyAction.StartDisband(partyBelongedTo);
						partyBelongedTo.Party.SetCustomOwner(null);
					}
					firstHero.ChangeState(Hero.CharacterStates.Fugitive);
					MobileParty partyBelongedTo2 = firstHero.PartyBelongedTo;
					if (partyBelongedTo2 != null)
					{
						partyBelongedTo2.MemberRoster.RemoveTroop(firstHero.CharacterObject, 1, default(UniqueTroopDescriptor), 0);
					}
				}
				firstHero.Clan = clanAfterMarriage;
				foreach (Hero hero in clan.Heroes)
				{
					hero.UpdateHomeSettlement();
				}
				using (List<Hero>.Enumerator enumerator = clanAfterMarriage.Heroes.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Hero hero2 = enumerator.Current;
						hero2.UpdateHomeSettlement();
					}
					goto IL_350;
				}
			}
			Clan clan2 = secondHero.Clan;
			if (secondHero.GovernorOf != null)
			{
				ChangeGovernorAction.RemoveGovernorOf(secondHero);
			}
			if (secondHero.PartyBelongedTo != null)
			{
				MobileParty partyBelongedTo3 = secondHero.PartyBelongedTo;
				if (clan2.Kingdom != clanAfterMarriage.Kingdom)
				{
					if (partyBelongedTo3.Army != null)
					{
						if (partyBelongedTo3.Army.LeaderParty == partyBelongedTo3)
						{
							DisbandArmyAction.ApplyByUnknownReason(partyBelongedTo3.Army);
						}
						else
						{
							partyBelongedTo3.Army = null;
						}
					}
					IFaction faction = clanAfterMarriage.Kingdom;
					FactionHelper.FinishAllRelatedHostileActionsOfNobleToFaction(secondHero, faction ?? clanAfterMarriage);
				}
				if (partyBelongedTo3.Party.IsActive && partyBelongedTo3.Party.Owner == secondHero)
				{
					DisbandPartyAction.StartDisband(partyBelongedTo3);
					partyBelongedTo3.Party.SetCustomOwner(null);
				}
				if (partyBelongedTo3.MemberRoster.Contains(secondHero.CharacterObject))
				{
					partyBelongedTo3.MemberRoster.RemoveTroop(secondHero.CharacterObject, 1, default(UniqueTroopDescriptor), 0);
				}
				if (secondHero.CurrentSettlement == null)
				{
					secondHero.ChangeState(Hero.CharacterStates.Fugitive);
				}
				secondHero.Clan = clanAfterMarriage;
			}
			secondHero.Clan = clanAfterMarriage;
			foreach (Hero hero3 in clan2.Heroes)
			{
				hero3.UpdateHomeSettlement();
			}
			foreach (Hero hero4 in clanAfterMarriage.Heroes)
			{
				hero4.UpdateHomeSettlement();
			}
			IL_350:
			Romance.EndAllCourtships(firstHero);
			Romance.EndAllCourtships(secondHero);
			ChangeRomanticStateAction.Apply(firstHero, secondHero, Romance.RomanceLevelEnum.Marriage);
			CampaignEventDispatcher.Instance.OnHeroesMarried(firstHero, secondHero, showNotification);
		}

		public static void Apply(Hero firstHero, Hero secondHero, bool showNotification = true)
		{
			MarriageAction.ApplyInternal(firstHero, secondHero, showNotification);
		}
	}
}
