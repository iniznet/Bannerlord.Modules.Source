using System;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;

namespace TaleWorlds.CampaignSystem.Actions
{
	// Token: 0x02000448 RID: 1096
	public static class LeaveSettlementAction
	{
		// Token: 0x06003F2B RID: 16171 RVA: 0x0012DFC0 File Offset: 0x0012C1C0
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

		// Token: 0x06003F2C RID: 16172 RVA: 0x0012E120 File Offset: 0x0012C320
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
