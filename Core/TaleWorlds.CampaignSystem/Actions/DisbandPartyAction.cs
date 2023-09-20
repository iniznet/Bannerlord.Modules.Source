using System;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.Actions
{
	// Token: 0x0200043D RID: 1085
	public static class DisbandPartyAction
	{
		// Token: 0x06003EED RID: 16109 RVA: 0x0012C994 File Offset: 0x0012AB94
		public static void StartDisband(MobileParty disbandParty)
		{
			if (disbandParty.IsDisbanding)
			{
				return;
			}
			if (disbandParty.MemberRoster.TotalManCount == 0)
			{
				DestroyPartyAction.Apply(null, disbandParty);
				return;
			}
			IDisbandPartyCampaignBehavior campaignBehavior = Campaign.Current.GetCampaignBehavior<IDisbandPartyCampaignBehavior>();
			if (campaignBehavior != null && campaignBehavior.IsPartyWaitingForDisband(disbandParty))
			{
				return;
			}
			if (disbandParty.Army != null)
			{
				if (disbandParty == disbandParty.Army.LeaderParty)
				{
					DisbandArmyAction.ApplyByUnknownReason(disbandParty.Army);
				}
				else
				{
					disbandParty.Army = null;
				}
			}
			TextObject textObject = new TextObject("{=ithcVNfA}{CLAN_NAME}{.o} Party", null);
			textObject.SetTextVariable("CLAN_NAME", (disbandParty.ActualClan != null) ? disbandParty.ActualClan.Name : CampaignData.NeutralFaction.Name);
			disbandParty.SetCustomName(textObject);
			CampaignEventDispatcher.Instance.OnPartyDisbandStarted(disbandParty);
		}

		// Token: 0x06003EEE RID: 16110 RVA: 0x0012CA4A File Offset: 0x0012AC4A
		public static void CancelDisband(MobileParty disbandParty)
		{
			CampaignEventDispatcher.Instance.OnPartyDisbandCanceled(disbandParty);
			disbandParty.IsDisbanding = false;
			disbandParty.SetCustomName(TextObject.Empty);
			disbandParty.Ai.SetMoveModeHold();
		}
	}
}
