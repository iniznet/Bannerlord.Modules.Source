using System;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;

namespace TaleWorlds.CampaignSystem.Actions
{
	public static class LeaveSettlementAction
	{
		public static void ApplyForParty(MobileParty mobileParty)
		{
			Settlement currentSettlement = mobileParty.CurrentSettlement;
			if (mobileParty.Army != null && mobileParty.Army.LeaderParty == mobileParty)
			{
				foreach (MobileParty mobileParty2 in mobileParty.Army.LeaderParty.AttachedParties)
				{
					if (mobileParty2 == MobileParty.MainParty && PlayerEncounter.Current != null)
					{
						PlayerEncounter.Finish(true);
					}
					else if (mobileParty2.CurrentSettlement == currentSettlement)
					{
						LeaveSettlementAction.ApplyForParty(mobileParty2);
					}
				}
				foreach (MobileParty mobileParty3 in mobileParty.Army.Parties)
				{
					if (mobileParty3 != mobileParty && mobileParty3.MapEvent == null && mobileParty3.CurrentSettlement == null)
					{
						mobileParty3.Ai.SetMoveModeHold();
						mobileParty3.Ai.SetMoveEscortParty(mobileParty);
					}
				}
			}
			if (mobileParty == MobileParty.MainParty && (MobileParty.MainParty.Army == null || MobileParty.MainParty.Army.LeaderParty == MobileParty.MainParty))
			{
				mobileParty.Ai.SetMoveModeHold();
			}
			mobileParty.CurrentSettlement = null;
			currentSettlement.SettlementComponent.OnPartyLeft(mobileParty);
			CampaignEventDispatcher.Instance.OnSettlementLeft(mobileParty, currentSettlement);
		}

		public static void ApplyForCharacterOnly(Hero hero)
		{
			Settlement currentSettlement = hero.CurrentSettlement;
			if (hero != null)
			{
				hero.StayingInSettlement = null;
			}
			LocationComplex locationComplex = currentSettlement.LocationComplex;
			Location location = ((locationComplex != null) ? locationComplex.GetLocationOfCharacter(hero) : null);
			if (location != null && location.GetLocationCharacter(hero) != null)
			{
				currentSettlement.LocationComplex.RemoveCharacterIfExists(hero);
				LocationEncounter locationEncounter = PlayerEncounter.LocationEncounter;
				if (locationEncounter == null)
				{
					return;
				}
				locationEncounter.RemoveAccompanyingCharacter(hero);
			}
		}
	}
}
