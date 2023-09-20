using System;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.Actions
{
	public static class TakePrisonerAction
	{
		private static void ApplyInternal(PartyBase capturerParty, Hero prisonerCharacter, bool isEventCalled = true)
		{
			if (prisonerCharacter.PartyBelongedTo != null)
			{
				if (prisonerCharacter.PartyBelongedTo.LeaderHero == prisonerCharacter)
				{
					prisonerCharacter.PartyBelongedTo.RemovePartyLeader();
				}
				prisonerCharacter.PartyBelongedTo.MemberRoster.RemoveTroop(prisonerCharacter.CharacterObject, 1, default(UniqueTroopDescriptor), 0);
			}
			prisonerCharacter.CaptivityStartTime = CampaignTime.Now;
			prisonerCharacter.ChangeState(Hero.CharacterStates.Prisoner);
			capturerParty.AddPrisoner(prisonerCharacter.CharacterObject, 1);
			if (prisonerCharacter == Hero.MainHero)
			{
				PlayerCaptivity.StartCaptivity(capturerParty);
			}
			if (capturerParty.IsSettlement && prisonerCharacter.StayingInSettlement != null)
			{
				prisonerCharacter.StayingInSettlement = null;
			}
			Debug.Print(string.Format("{0} has taken prisoner by {1}.", prisonerCharacter.Name, capturerParty.Name), 0, Debug.DebugColor.White, 17592186044416UL);
			if (isEventCalled)
			{
				CampaignEventDispatcher.Instance.OnHeroPrisonerTaken(capturerParty, prisonerCharacter);
			}
		}

		public static void Apply(PartyBase capturerParty, Hero prisonerCharacter)
		{
			TakePrisonerAction.ApplyInternal(capturerParty, prisonerCharacter, true);
		}

		public static void ApplyByTakenFromPartyScreen(FlattenedTroopRoster roster)
		{
			foreach (FlattenedTroopRosterElement flattenedTroopRosterElement in roster)
			{
				if (flattenedTroopRosterElement.Troop.IsHero)
				{
					TakePrisonerAction.ApplyInternal(PartyBase.MainParty, flattenedTroopRosterElement.Troop.HeroObject, true);
				}
			}
			CampaignEventDispatcher.Instance.OnPrisonerTaken(roster);
		}
	}
}
