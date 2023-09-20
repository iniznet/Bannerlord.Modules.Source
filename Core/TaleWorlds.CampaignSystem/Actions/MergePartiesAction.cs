using System;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.Actions
{
	// Token: 0x0200044F RID: 1103
	public static class MergePartiesAction
	{
		// Token: 0x06003F3B RID: 16187 RVA: 0x0012EB68 File Offset: 0x0012CD68
		private static void ApplyInternal(PartyBase majorParty, PartyBase joinedParty)
		{
			int numberOfAllMembers = joinedParty.NumberOfAllMembers;
			majorParty.AddMembers(joinedParty.MemberRoster);
			majorParty.AddPrisoners(joinedParty.PrisonRoster);
			DestroyPartyAction.Apply(null, joinedParty.MobileParty);
			if (majorParty == PartyBase.MainParty)
			{
				MBTextManager.SetTextVariable("NUMBER_OF_TROOPS", numberOfAllMembers);
				MBInformationManager.AddQuickInformation(new TextObject("{=511LONpe}{NUMBER_OF_TROOPS} troops joined you.", null), 0, null, "");
			}
		}

		// Token: 0x06003F3C RID: 16188 RVA: 0x0012EBCA File Offset: 0x0012CDCA
		public static void Apply(PartyBase majorParty, PartyBase joinedParty)
		{
			MergePartiesAction.ApplyInternal(majorParty, joinedParty);
		}
	}
}
