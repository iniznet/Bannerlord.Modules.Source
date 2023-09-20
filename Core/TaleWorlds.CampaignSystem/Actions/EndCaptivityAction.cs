using System;
using Helpers;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.Actions
{
	public static class EndCaptivityAction
	{
		private static void ApplyInternal(Hero prisoner, EndCaptivityDetail detail, Hero facilitatior = null)
		{
			PartyBase partyBelongedToAsPrisoner = prisoner.PartyBelongedToAsPrisoner;
			IFaction faction = ((partyBelongedToAsPrisoner != null) ? partyBelongedToAsPrisoner.MapFaction : null);
			if (prisoner == Hero.MainHero)
			{
				PlayerCaptivity.EndCaptivity();
				if (facilitatior != null && detail != EndCaptivityDetail.Death)
				{
					StringHelpers.SetCharacterProperties("FACILITATOR", facilitatior.CharacterObject, null, false);
					MBInformationManager.AddQuickInformation(new TextObject("{=xPuSASof}{FACILITATOR.NAME} paid a ransom and freed you from captivity.", null), 0, null, "");
				}
				CampaignEventDispatcher.Instance.OnHeroPrisonerReleased(prisoner, partyBelongedToAsPrisoner, faction, detail);
				return;
			}
			if (detail == EndCaptivityDetail.Death)
			{
				prisoner.StayingInSettlement = null;
			}
			if (partyBelongedToAsPrisoner != null && partyBelongedToAsPrisoner.PrisonRoster.Contains(prisoner.CharacterObject))
			{
				partyBelongedToAsPrisoner.PrisonRoster.RemoveTroop(prisoner.CharacterObject, 1, default(UniqueTroopDescriptor), 0);
			}
			if (detail != EndCaptivityDetail.Death)
			{
				if (prisoner.IsPlayerCompanion && detail != EndCaptivityDetail.Ransom)
				{
					MakeHeroFugitiveAction.Apply(prisoner);
				}
				else if (detail <= EndCaptivityDetail.ReleasedByChoice)
				{
					prisoner.ChangeState(Hero.CharacterStates.Released);
				}
				else
				{
					MakeHeroFugitiveAction.Apply(prisoner);
				}
				Settlement currentSettlement = prisoner.CurrentSettlement;
				if (currentSettlement != null)
				{
					currentSettlement.AddHeroWithoutParty(prisoner);
				}
				CampaignEventDispatcher.Instance.OnHeroPrisonerReleased(prisoner, partyBelongedToAsPrisoner, faction, detail);
			}
		}

		public static void ApplyByReleasedAfterBattle(Hero character)
		{
			EndCaptivityAction.ApplyInternal(character, EndCaptivityDetail.ReleasedAfterBattle, null);
		}

		public static void ApplyByRansom(Hero character, Hero facilitator)
		{
			EndCaptivityAction.ApplyInternal(character, EndCaptivityDetail.Ransom, facilitator);
		}

		public static void ApplyByPeace(Hero character, Hero facilitator = null)
		{
			EndCaptivityAction.ApplyInternal(character, EndCaptivityDetail.ReleasedAfterPeace, facilitator);
		}

		public static void ApplyByEscape(Hero character, Hero facilitator = null)
		{
			EndCaptivityAction.ApplyInternal(character, EndCaptivityDetail.ReleasedAfterEscape, facilitator);
		}

		public static void ApplyByDeath(Hero character)
		{
			EndCaptivityAction.ApplyInternal(character, EndCaptivityDetail.Death, null);
		}

		public static void ApplyByReleasedByChoice(FlattenedTroopRoster troopRoster)
		{
			foreach (FlattenedTroopRosterElement flattenedTroopRosterElement in troopRoster)
			{
				if (flattenedTroopRosterElement.Troop.IsHero)
				{
					EndCaptivityAction.ApplyInternal(flattenedTroopRosterElement.Troop.HeroObject, EndCaptivityDetail.ReleasedByChoice, null);
				}
			}
			CampaignEventDispatcher.Instance.OnPrisonerReleased(troopRoster);
		}

		public static void ApplyByReleasedByChoice(Hero character, Hero facilitator = null)
		{
			EndCaptivityAction.ApplyInternal(character, EndCaptivityDetail.ReleasedByChoice, facilitator);
		}
	}
}
