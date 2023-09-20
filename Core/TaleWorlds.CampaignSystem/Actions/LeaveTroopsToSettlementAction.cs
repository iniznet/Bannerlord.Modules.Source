using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.Actions
{
	public static class LeaveTroopsToSettlementAction
	{
		private static void ApplyInternal(MobileParty mobileParty, Settlement settlement, int numberOfTroopsWillBeLeft, bool archersAreHighPriority)
		{
			TroopRoster troopRoster = TroopRoster.CreateDummyTroopRoster();
			int num = numberOfTroopsWillBeLeft;
			int i = 0;
			while (i < MathF.Abs(numberOfTroopsWillBeLeft))
			{
				CharacterObject characterObject = null;
				num--;
				int num2 = (archersAreHighPriority ? 4 : 1);
				for (int j = 0; j < num2; j++)
				{
					if (numberOfTroopsWillBeLeft > 0)
					{
						int num3 = MBRandom.RandomInt(mobileParty.MemberRoster.TotalRegulars);
						CharacterObject characterObject2 = null;
						PartyBase partyBase;
						int num4;
						mobileParty.Party.GetCharacterFromPartyRank(num3, out characterObject2, out partyBase, out num4, true);
						if (characterObject2.IsRanged)
						{
							characterObject = characterObject2;
							break;
						}
						if (!archersAreHighPriority || !characterObject2.IsMounted || characterObject == null)
						{
							characterObject = characterObject2;
						}
					}
					else
					{
						int num5 = settlement.Town.GarrisonParty.MemberRoster.TotalHeroes + MBRandom.RandomInt(settlement.Town.GarrisonParty.MemberRoster.TotalRegulars);
						CharacterObject characterObject3 = null;
						PartyBase partyBase;
						int num4;
						settlement.Town.GarrisonParty.Party.GetCharacterFromPartyRank(num5, out characterObject3, out partyBase, out num4, true);
						characterObject = characterObject3;
					}
				}
				if (numberOfTroopsWillBeLeft > 0)
				{
					using (List<TroopRosterElement>.Enumerator enumerator = mobileParty.Party.MemberRoster.GetTroopRoster().GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							TroopRosterElement troopRosterElement = enumerator.Current;
							if (troopRosterElement.Character == characterObject)
							{
								if (troopRosterElement.WoundedNumber > 0)
								{
									troopRoster.AddToCounts(characterObject, 1, false, 1, 0, true, -1);
									mobileParty.MemberRoster.AddToCounts(characterObject, -1, false, -1, 0, true, -1);
									break;
								}
								troopRoster.AddToCounts(characterObject, 1, false, 0, 0, true, -1);
								mobileParty.AddElementToMemberRoster(characterObject, -1, false);
								break;
							}
						}
						goto IL_231;
					}
					goto IL_178;
				}
				goto IL_178;
				IL_231:
				i++;
				continue;
				IL_178:
				foreach (TroopRosterElement troopRosterElement2 in settlement.Town.GarrisonParty.MemberRoster.GetTroopRoster())
				{
					if (troopRosterElement2.Character == characterObject)
					{
						if (troopRosterElement2.Number - troopRosterElement2.WoundedNumber > 0)
						{
							troopRoster.AddToCounts(characterObject, 1, false, 0, 0, true, -1);
							settlement.Town.GarrisonParty.MemberRoster.AddToCounts(characterObject, -1, false, 0, 0, true, -1);
							break;
						}
						troopRoster.AddToCounts(characterObject, 1, false, 1, 0, true, -1);
						settlement.Town.GarrisonParty.MemberRoster.AddToCounts(characterObject, -1, false, -1, 0, true, -1);
						break;
					}
				}
				goto IL_231;
			}
			if (troopRoster.Count > 0)
			{
				if (numberOfTroopsWillBeLeft > 0)
				{
					CampaignEventDispatcher.Instance.OnTroopGivenToSettlement(mobileParty.LeaderHero, settlement, troopRoster);
					if (settlement.Town.GarrisonParty == null)
					{
						settlement.AddGarrisonParty(false);
					}
					while (troopRoster.Count > 0)
					{
						TroopRosterElement elementCopyAtIndex = troopRoster.GetElementCopyAtIndex(0);
						troopRoster.AddToCounts(elementCopyAtIndex.Character, -elementCopyAtIndex.Number, false, 0, 0, true, -1);
						settlement.Town.GarrisonParty.MemberRoster.AddToCounts(elementCopyAtIndex.Character, elementCopyAtIndex.Number, false, 0, 0, true, -1);
					}
					if (mobileParty.LeaderHero != null && settlement.OwnerClan != mobileParty.LeaderHero.Clan)
					{
						float num6 = 0f;
						foreach (TroopRosterElement troopRosterElement3 in troopRoster.GetTroopRoster())
						{
							float troopPower = Campaign.Current.Models.MilitaryPowerModel.GetTroopPower(troopRosterElement3.Character, BattleSideEnum.Defender, MapEvent.PowerCalculationContext.Siege, 0f);
							num6 += troopPower * (float)troopRosterElement3.Number;
						}
						GainKingdomInfluenceAction.ApplyForLeavingTroopToGarrison(mobileParty.LeaderHero, num6 / 3f);
						return;
					}
				}
				else
				{
					while (troopRoster.Count > 0)
					{
						TroopRosterElement elementCopyAtIndex2 = troopRoster.GetElementCopyAtIndex(0);
						troopRoster.AddToCounts(elementCopyAtIndex2.Character, -elementCopyAtIndex2.Number, false, 0, 0, true, -1);
						mobileParty.MemberRoster.AddToCounts(elementCopyAtIndex2.Character, elementCopyAtIndex2.Number, false, 0, 0, true, -1);
					}
				}
			}
		}

		public static void Apply(MobileParty mobileParty, Settlement settlement, int number, bool archersAreHighPriority)
		{
			LeaveTroopsToSettlementAction.ApplyInternal(mobileParty, settlement, number, archersAreHighPriority);
		}
	}
}
