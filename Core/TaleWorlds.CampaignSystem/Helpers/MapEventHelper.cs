using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.LinQuick;

namespace Helpers
{
	public static class MapEventHelper
	{
		public static PartyBase GetSallyOutDefenderLeader()
		{
			PartyBase partyBase;
			if (MobileParty.MainParty.CurrentSettlement.Town.GarrisonParty != null)
			{
				partyBase = MobileParty.MainParty.CurrentSettlement.Town.GarrisonParty.MapEvent.DefenderSide.LeaderParty;
			}
			else
			{
				PartyBase party = MobileParty.MainParty.CurrentSettlement.Party;
				if (((party != null) ? party.MapEvent : null) != null)
				{
					partyBase = MobileParty.MainParty.CurrentSettlement.Party.MapEvent.DefenderSide.LeaderParty;
				}
				else
				{
					partyBase = MobileParty.MainParty.CurrentSettlement.SiegeEvent.BesiegerCamp.LeaderParty.Party;
				}
			}
			return partyBase;
		}

		public static bool CanLeaveBattle(MobileParty mobileParty)
		{
			return mobileParty.MapEvent.DefenderSide.LeaderParty != mobileParty.Party && (!mobileParty.MapEvent.DefenderSide.LeaderParty.IsSettlement || mobileParty.CurrentSettlement != mobileParty.MapEvent.DefenderSide.LeaderParty.Settlement || mobileParty.MapFaction != mobileParty.MapEvent.DefenderSide.LeaderParty.MapFaction) && (mobileParty.MapEvent.PartiesOnSide(BattleSideEnum.Attacker).FindIndexQ((MapEventParty party) => party.Party == mobileParty.Party) < 0 || !mobileParty.MapEvent.IsRaid || mobileParty.Army == null || mobileParty.Army.LeaderParty == mobileParty) && (mobileParty.MapEvent.PartiesOnSide(BattleSideEnum.Defender).FindIndexQ((MapEventParty party) => party.Party == mobileParty.Party) < 0 || mobileParty.Army == null || mobileParty.Army.LeaderParty == mobileParty);
		}

		public static void OnConversationEnd()
		{
			if (PlayerEncounter.Current != null && ((PlayerEncounter.EncounteredMobileParty != null && PlayerEncounter.EncounteredMobileParty.MapFaction != null && !PlayerEncounter.EncounteredMobileParty.MapFaction.IsAtWarWith(Hero.MainHero.MapFaction)) || (PlayerEncounter.EncounteredParty != null && PlayerEncounter.EncounteredParty.MapFaction != null && !PlayerEncounter.EncounteredParty.MapFaction.IsAtWarWith(Hero.MainHero.MapFaction))))
			{
				PlayerEncounter.LeaveEncounter = true;
			}
		}
	}
}
