using System;
using Helpers;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.Actions
{
	// Token: 0x0200043F RID: 1087
	public static class EndCaptivityAction
	{
		// Token: 0x06003EEF RID: 16111 RVA: 0x0012CA74 File Offset: 0x0012AC74
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

		// Token: 0x06003EF0 RID: 16112 RVA: 0x0012CB69 File Offset: 0x0012AD69
		public static void ApplyByReleasedAfterBattle(Hero character)
		{
			EndCaptivityAction.ApplyInternal(character, EndCaptivityDetail.ReleasedAfterBattle, null);
		}

		// Token: 0x06003EF1 RID: 16113 RVA: 0x0012CB73 File Offset: 0x0012AD73
		public static void ApplyByRansom(Hero character, Hero facilitator)
		{
			EndCaptivityAction.ApplyInternal(character, EndCaptivityDetail.Ransom, facilitator);
		}

		// Token: 0x06003EF2 RID: 16114 RVA: 0x0012CB7D File Offset: 0x0012AD7D
		public static void ApplyByPeace(Hero character, Hero facilitator = null)
		{
			EndCaptivityAction.ApplyInternal(character, EndCaptivityDetail.ReleasedAfterPeace, facilitator);
		}

		// Token: 0x06003EF3 RID: 16115 RVA: 0x0012CB87 File Offset: 0x0012AD87
		public static void ApplyByEscape(Hero character, Hero facilitator = null)
		{
			EndCaptivityAction.ApplyInternal(character, EndCaptivityDetail.ReleasedAfterEscape, facilitator);
		}

		// Token: 0x06003EF4 RID: 16116 RVA: 0x0012CB91 File Offset: 0x0012AD91
		public static void ApplyByDeath(Hero character)
		{
			EndCaptivityAction.ApplyInternal(character, EndCaptivityDetail.Death, null);
		}

		// Token: 0x06003EF5 RID: 16117 RVA: 0x0012CB9C File Offset: 0x0012AD9C
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

		// Token: 0x06003EF6 RID: 16118 RVA: 0x0012CC0C File Offset: 0x0012AE0C
		public static void ApplyByReleasedByChoice(Hero character, Hero facilitator = null)
		{
			EndCaptivityAction.ApplyInternal(character, EndCaptivityDetail.ReleasedByChoice, facilitator);
		}
	}
}
