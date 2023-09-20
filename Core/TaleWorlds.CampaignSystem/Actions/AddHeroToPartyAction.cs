using System;
using Helpers;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.Actions
{
	// Token: 0x02000423 RID: 1059
	public static class AddHeroToPartyAction
	{
		// Token: 0x06003E82 RID: 16002 RVA: 0x0012A380 File Offset: 0x00128580
		private static void ApplyInternal(Hero hero, MobileParty newParty, bool showNotification = true)
		{
			PartyBase partyBase = ((hero.PartyBelongedTo != null) ? hero.PartyBelongedTo.Party : ((hero.CurrentSettlement != null) ? hero.CurrentSettlement.Party : null));
			if (partyBase != null)
			{
				if (partyBase.IsSettlement && partyBase.Settlement.Notables.IndexOf(hero) >= 0)
				{
					hero.StayingInSettlement = null;
				}
				else
				{
					partyBase.MemberRoster.AddToCounts(hero.CharacterObject, -1, false, 0, 0, true, -1);
				}
			}
			if (hero.GovernorOf != null)
			{
				ChangeGovernorAction.RemoveGovernorOf(hero);
			}
			newParty.AddElementToMemberRoster(hero.CharacterObject, 1, false);
			CampaignEventDispatcher.Instance.OnHeroJoinedParty(hero, newParty);
			if (showNotification && newParty == MobileParty.MainParty && hero.IsPlayerCompanion)
			{
				TextObject textObject = GameTexts.FindText("str_companion_added", null);
				StringHelpers.SetCharacterProperties("COMPANION", hero.CharacterObject, textObject, false);
				MBInformationManager.AddQuickInformation(textObject, 0, null, "");
			}
		}

		// Token: 0x06003E83 RID: 16003 RVA: 0x0012A461 File Offset: 0x00128661
		public static void Apply(Hero hero, MobileParty party, bool showNotification = true)
		{
			AddHeroToPartyAction.ApplyInternal(hero, party, showNotification);
		}
	}
}
