using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.LinQuick;

namespace Helpers
{
	// Token: 0x02000010 RID: 16
	public static class MapEventHelper
	{
		// Token: 0x0600008B RID: 139 RVA: 0x000074CC File Offset: 0x000056CC
		public static bool CanLeaveBattle(MobileParty mobileParty)
		{
			return mobileParty.MapEvent.DefenderSide.LeaderParty != mobileParty.Party && (!mobileParty.MapEvent.DefenderSide.LeaderParty.IsSettlement || mobileParty.CurrentSettlement != mobileParty.MapEvent.DefenderSide.LeaderParty.Settlement || mobileParty.MapFaction != mobileParty.MapEvent.DefenderSide.LeaderParty.MapFaction) && (mobileParty.MapEvent.PartiesOnSide(BattleSideEnum.Attacker).FindIndexQ((MapEventParty party) => party.Party == mobileParty.Party) < 0 || !mobileParty.MapEvent.IsRaid || mobileParty.Army == null || mobileParty.Army.LeaderParty == mobileParty) && (mobileParty.MapEvent.PartiesOnSide(BattleSideEnum.Defender).FindIndexQ((MapEventParty party) => party.Party == mobileParty.Party) < 0 || mobileParty.Army == null || mobileParty.Army.LeaderParty == mobileParty);
		}

		// Token: 0x0600008C RID: 140 RVA: 0x00007624 File Offset: 0x00005824
		public static bool CanPartyJoinBattle(PartyBase party, MapEvent mapEvent, BattleSideEnum side)
		{
			return mapEvent.GetMapEventSide(side).Parties.All((MapEventParty x) => !x.Party.MapFaction.IsAtWarWith(party.MapFaction)) && mapEvent.GetMapEventSide(mapEvent.GetOtherSide(side)).Parties.All((MapEventParty x) => x.Party.MapFaction.IsAtWarWith(party.MapFaction));
		}

		// Token: 0x0600008D RID: 141 RVA: 0x00007684 File Offset: 0x00005884
		public static void GetStrengthsRelativeToParty(BattleSideEnum partySide, MapEvent mapEvent, out float partySideStrength, out float opposingSideStrength)
		{
			partySideStrength = 0.1f;
			opposingSideStrength = 0.1f;
			if (mapEvent != null)
			{
				using (IEnumerator<PartyBase> enumerator = mapEvent.InvolvedParties.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						PartyBase partyBase = enumerator.Current;
						if (partyBase.Side == partySide)
						{
							partySideStrength += partyBase.TotalStrength;
						}
						else
						{
							opposingSideStrength += partyBase.TotalStrength;
						}
					}
					return;
				}
			}
			Debug.FailedAssert("Cannot retrieve party strengths. MapEvent parameter is null.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\Helpers.cs", "GetStrengthsRelativeToParty", 3353);
		}

		// Token: 0x0600008E RID: 142 RVA: 0x00007718 File Offset: 0x00005918
		public static void OnConversationEnd()
		{
			if (PlayerEncounter.Current != null && ((PlayerEncounter.EncounteredMobileParty != null && PlayerEncounter.EncounteredMobileParty.MapFaction != null && !PlayerEncounter.EncounteredMobileParty.MapFaction.IsAtWarWith(Hero.MainHero.MapFaction)) || (PlayerEncounter.EncounteredParty != null && PlayerEncounter.EncounteredParty.MapFaction != null && !PlayerEncounter.EncounteredParty.MapFaction.IsAtWarWith(Hero.MainHero.MapFaction))))
			{
				PlayerEncounter.LeaveEncounter = true;
			}
		}

		// Token: 0x0600008F RID: 143 RVA: 0x00007790 File Offset: 0x00005990
		public static bool CheckIfBattleShouldContinueAfterBattleMission(MapEvent mapEvent, CampaignBattleResult campaignBattleResult)
		{
			if (PlayerEncounter.PlayerSurrender || campaignBattleResult == null)
			{
				return false;
			}
			bool flag = mapEvent.IsSiegeAssault && mapEvent.BattleState == BattleState.AttackerVictory;
			MapEventSide mapEventSide = mapEvent.GetMapEventSide(mapEvent.PlayerSide);
			bool flag2 = (campaignBattleResult.PlayerDefeat && mapEventSide.GetTotalHealthyTroopCountOfSide() >= 1) || ((campaignBattleResult.PlayerVictory || campaignBattleResult.EnemyPulledBack) && mapEvent.DefeatedSide != BattleSideEnum.None && mapEvent.GetMapEventSide(mapEvent.DefeatedSide).GetTotalHealthyTroopCountOfSide() >= 1);
			return !mapEvent.IsHideoutBattle && !flag && flag2 && !mapEventSide.IsSurrendered;
		}
	}
}
