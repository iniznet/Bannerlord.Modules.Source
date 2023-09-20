using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.CampaignSystem.Issues;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.Actions
{
	public static class KillCharacterAction
	{
		private static void ApplyInternal(Hero victim, Hero killer, KillCharacterAction.KillCharacterActionDetail actionDetail, bool showNotification, bool isForced = false)
		{
			if (!victim.CanDie(actionDetail) && !isForced)
			{
				return;
			}
			if (!victim.IsAlive)
			{
				Debug.FailedAssert("Victim: " + victim.Name + " is already dead!", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\Actions\\KillCharacterAction.cs", "ApplyInternal", 40);
				return;
			}
			if (victim.IsNotable)
			{
				IssueBase issue = victim.Issue;
				if (((issue != null) ? issue.IssueQuest : null) != null)
				{
					Debug.FailedAssert("Trying to kill a notable that has quest!", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\Actions\\KillCharacterAction.cs", "ApplyInternal", 47);
				}
			}
			MobileParty partyBelongedTo = victim.PartyBelongedTo;
			if (((partyBelongedTo != null) ? partyBelongedTo.MapEvent : null) == null)
			{
				MobileParty partyBelongedTo2 = victim.PartyBelongedTo;
				if (((partyBelongedTo2 != null) ? partyBelongedTo2.SiegeEvent : null) == null)
				{
					goto IL_E2;
				}
			}
			if (victim.DeathMark == KillCharacterAction.KillCharacterActionDetail.None)
			{
				victim.AddDeathMark(killer, actionDetail);
				return;
			}
			IL_E2:
			CampaignEventDispatcher.Instance.OnBeforeHeroKilled(victim, killer, actionDetail, showNotification);
			if (victim.IsHumanPlayerCharacter && !isForced)
			{
				CampaignEventDispatcher.Instance.OnBeforeMainCharacterDied(victim, killer, actionDetail, showNotification);
				return;
			}
			victim.EncyclopediaText = KillCharacterAction.CreateObituary(victim, actionDetail);
			if (victim.Clan != null && (victim.Clan.Leader == victim || victim == Hero.MainHero))
			{
				if (victim != Hero.MainHero && victim.Clan.Heroes.Any((Hero x) => !x.IsChild && x != victim && x.IsAlive && x.IsLord))
				{
					ChangeClanLeaderAction.ApplyWithoutSelectedNewLeader(victim.Clan);
				}
				if (victim.Clan.Kingdom != null && victim.Clan.Kingdom.RulingClan == victim.Clan)
				{
					List<Clan> list = victim.Clan.Kingdom.Clans.Where((Clan t) => !t.IsEliminated && t.Leader != victim && !t.IsUnderMercenaryService).ToList<Clan>();
					if (list.IsEmpty<Clan>())
					{
						if (!victim.Clan.Kingdom.IsEliminated)
						{
							DestroyKingdomAction.ApplyByKingdomLeaderDeath(victim.Clan.Kingdom);
						}
					}
					else if (list.Count > 1)
					{
						Clan clanToExclude = ((victim.Clan.Leader == victim || victim.Clan.Leader == null) ? victim.Clan : null);
						victim.Clan.Kingdom.AddDecision(new KingSelectionKingdomDecision(victim.Clan, clanToExclude), true);
						if (clanToExclude != null)
						{
							Clan randomElementWithPredicate = victim.Clan.Kingdom.Clans.GetRandomElementWithPredicate((Clan t) => t != clanToExclude && Campaign.Current.Models.DiplomacyModel.IsClanEligibleToBecomeRuler(t));
							ChangeRulingClanAction.Apply(victim.Clan.Kingdom, randomElementWithPredicate);
						}
					}
					else
					{
						ChangeRulingClanAction.Apply(victim.Clan.Kingdom, list[0]);
					}
				}
			}
			if (victim.PartyBelongedTo != null && (victim.PartyBelongedTo.LeaderHero == victim || victim == Hero.MainHero))
			{
				if (victim.PartyBelongedTo.Army != null)
				{
					if (victim.PartyBelongedTo.Army.LeaderParty == victim.PartyBelongedTo)
					{
						DisbandArmyAction.ApplyByArmyLeaderIsDead(victim.PartyBelongedTo.Army);
					}
					else
					{
						victim.PartyBelongedTo.Army = null;
					}
				}
				if (victim.PartyBelongedTo != MobileParty.MainParty)
				{
					victim.PartyBelongedTo.Ai.SetMoveModeHold();
					if (victim.Clan != null && victim.Clan.IsRebelClan)
					{
						DestroyPartyAction.Apply(null, victim.PartyBelongedTo);
					}
				}
			}
			KillCharacterAction.MakeDead(victim, true);
			if (victim.GovernorOf != null)
			{
				ChangeGovernorAction.RemoveGovernorOf(victim);
			}
			if (actionDetail == KillCharacterAction.KillCharacterActionDetail.Executed && killer == Hero.MainHero && victim.Clan != null && !victim.Clan.IsNeutralClan)
			{
				if (victim.GetTraitLevel(DefaultTraits.Honor) >= 0)
				{
					TraitLevelingHelper.OnLordExecuted();
				}
				foreach (Clan clan in Clan.All)
				{
					if (!clan.IsEliminated && !clan.IsBanditFaction && clan != Clan.PlayerClan && clan != CampaignData.NeutralFaction)
					{
						bool flag;
						int relationChangeForExecutingHero = Campaign.Current.Models.ExecutionRelationModel.GetRelationChangeForExecutingHero(victim, clan.Leader, out flag);
						if (relationChangeForExecutingHero != 0)
						{
							ChangeRelationAction.ApplyPlayerRelation(clan.Leader, relationChangeForExecutingHero, flag, true);
						}
					}
				}
			}
			if (victim.Clan != null && !victim.Clan.IsEliminated && !victim.Clan.IsBanditFaction && !victim.Clan.IsNeutralClan && victim.Clan != Clan.PlayerClan)
			{
				if (victim.Clan.Leader == victim)
				{
					DestroyClanAction.ApplyByClanLeaderDeath(victim.Clan);
				}
				else if (victim.Clan.Leader == null)
				{
					DestroyClanAction.Apply(victim.Clan);
				}
			}
			CampaignEventDispatcher.Instance.OnHeroKilled(victim, killer, actionDetail, showNotification);
			if (victim.Spouse != null)
			{
				victim.Spouse = null;
			}
			if (victim.CompanionOf != null)
			{
				RemoveCompanionAction.ApplyByDeath(victim.CompanionOf, victim);
			}
			if (victim.CurrentSettlement != null)
			{
				if (victim.CurrentSettlement == Settlement.CurrentSettlement)
				{
					LocationComplex locationComplex = LocationComplex.Current;
					if (locationComplex != null)
					{
						locationComplex.RemoveCharacterIfExists(victim);
					}
				}
				if (victim.StayingInSettlement != null)
				{
					victim.StayingInSettlement = null;
				}
			}
		}

		public static void ApplyByOldAge(Hero victim, bool showNotification = true)
		{
			KillCharacterAction.ApplyInternal(victim, null, KillCharacterAction.KillCharacterActionDetail.DiedOfOldAge, showNotification, false);
		}

		public static void ApplyByWounds(Hero victim, bool showNotification = true)
		{
			KillCharacterAction.ApplyInternal(victim, null, KillCharacterAction.KillCharacterActionDetail.WoundedInBattle, showNotification, false);
		}

		public static void ApplyByBattle(Hero victim, Hero killer, bool showNotification = true)
		{
			KillCharacterAction.ApplyInternal(victim, killer, KillCharacterAction.KillCharacterActionDetail.DiedInBattle, showNotification, false);
		}

		public static void ApplyByMurder(Hero victim, Hero killer = null, bool showNotification = true)
		{
			KillCharacterAction.ApplyInternal(victim, killer, KillCharacterAction.KillCharacterActionDetail.Murdered, showNotification, false);
		}

		public static void ApplyInLabor(Hero lostMother, bool showNotification = true)
		{
			KillCharacterAction.ApplyInternal(lostMother, null, KillCharacterAction.KillCharacterActionDetail.DiedInLabor, showNotification, false);
		}

		public static void ApplyByExecution(Hero victim, Hero executer, bool showNotification = true, bool isForced = false)
		{
			KillCharacterAction.ApplyInternal(victim, executer, KillCharacterAction.KillCharacterActionDetail.Executed, showNotification, isForced);
		}

		public static void ApplyByRemove(Hero victim, bool showNotification = false, bool isForced = true)
		{
			KillCharacterAction.ApplyInternal(victim, null, KillCharacterAction.KillCharacterActionDetail.Lost, showNotification, isForced);
		}

		public static void ApplyByDeathMark(Hero victim, bool showNotification = false)
		{
			KillCharacterAction.ApplyInternal(victim, victim.DeathMarkKillerHero, victim.DeathMark, showNotification, false);
		}

		public static void ApplyByDeathMarkForced(Hero victim, bool showNotification = false)
		{
			KillCharacterAction.ApplyInternal(victim, victim.DeathMarkKillerHero, victim.DeathMark, showNotification, true);
		}

		public static void ApplyByPlayerIllness()
		{
			KillCharacterAction.ApplyInternal(Hero.MainHero, null, KillCharacterAction.KillCharacterActionDetail.DiedOfOldAge, true, true);
		}

		private static void MakeDead(Hero victim, bool disbandVictimParty = true)
		{
			victim.ChangeState(Hero.CharacterStates.Dead);
			victim.DeathDay = CampaignTime.Now;
			victim.HeroDeveloper.ClearUnspentPoints();
			if (victim.PartyBelongedToAsPrisoner != null)
			{
				EndCaptivityAction.ApplyByDeath(victim);
			}
			if (victim.PartyBelongedTo != null)
			{
				MobileParty partyBelongedTo = victim.PartyBelongedTo;
				if (partyBelongedTo.LeaderHero == victim)
				{
					bool flag = false;
					if (!partyBelongedTo.IsMainParty)
					{
						foreach (TroopRosterElement troopRosterElement in partyBelongedTo.MemberRoster.GetTroopRoster())
						{
							if (troopRosterElement.Character.IsHero && troopRosterElement.Character != victim.CharacterObject)
							{
								partyBelongedTo.ChangePartyLeader(troopRosterElement.Character.HeroObject);
								flag = true;
								break;
							}
						}
					}
					if (!flag)
					{
						if (!partyBelongedTo.IsMainParty)
						{
							partyBelongedTo.RemovePartyLeader();
						}
						if (partyBelongedTo.IsActive && disbandVictimParty)
						{
							Hero owner = partyBelongedTo.Party.Owner;
							if (((owner != null) ? owner.CompanionOf : null) == Clan.PlayerClan)
							{
								partyBelongedTo.Party.SetCustomOwner(Hero.MainHero);
							}
							partyBelongedTo.MemberRoster.RemoveTroop(victim.CharacterObject, 1, default(UniqueTroopDescriptor), 0);
							DisbandPartyAction.StartDisband(partyBelongedTo);
						}
					}
				}
				if (victim.PartyBelongedTo != null)
				{
					partyBelongedTo.MemberRoster.RemoveTroop(victim.CharacterObject, 1, default(UniqueTroopDescriptor), 0);
				}
				if (partyBelongedTo.IsActive && partyBelongedTo.MemberRoster.TotalManCount == 0)
				{
					DestroyPartyAction.Apply(null, partyBelongedTo);
					return;
				}
			}
			else if (victim.IsHumanPlayerCharacter && !MobileParty.MainParty.IsActive)
			{
				DestroyPartyAction.Apply(null, MobileParty.MainParty);
			}
		}

		private static Clan SelectHeirClanForKingdom(Kingdom kingdom, bool exceptRulingClan)
		{
			Clan rulingClan = kingdom.RulingClan;
			Clan clan = null;
			float num = 0f;
			IEnumerable<Clan> clans = kingdom.Clans;
			Func<Clan, bool> <>9__0;
			Func<Clan, bool> func;
			if ((func = <>9__0) == null)
			{
				func = (<>9__0 = (Clan t) => t.Heroes.Any((Hero h) => h.IsAlive) && !t.IsMinorFaction && t != rulingClan);
			}
			foreach (Clan clan2 in clans.Where(func))
			{
				float clanStrength = Campaign.Current.Models.DiplomacyModel.GetClanStrength(clan2);
				if (num <= clanStrength)
				{
					num = clanStrength;
					clan = clan2;
				}
			}
			return clan;
		}

		private static TextObject CreateObituary(Hero hero, KillCharacterAction.KillCharacterActionDetail detail)
		{
			TextObject textObject;
			if (hero.IsLord)
			{
				if (hero.Clan != null && hero.Clan.IsMinorFaction)
				{
					textObject = new TextObject("{=L7qd6qfv}{CHARACTER.FIRSTNAME} was a member of the {CHARACTER.FACTION}. {FURTHER_DETAILS}.", null);
				}
				else
				{
					textObject = new TextObject("{=mfYzCeGR}{CHARACTER.NAME} was {TITLE} of the {CHARACTER_FACTION_SHORT}. {FURTHER_DETAILS}.", null);
					textObject.SetTextVariable("CHARACTER_FACTION_SHORT", hero.MapFaction.InformalName);
					textObject.SetTextVariable("TITLE", HeroHelper.GetTitleInIndefiniteCase(hero));
				}
			}
			else if (hero.HomeSettlement != null)
			{
				textObject = new TextObject("{=YNXK352h}{CHARACTER.NAME} was a prominent {.%}{PROFESSION}{.%} from {HOMETOWN}. {FURTHER_DETAILS}.", null);
				textObject.SetTextVariable("PROFESSION", HeroHelper.GetCharacterTypeName(hero));
				textObject.SetTextVariable("HOMETOWN", hero.HomeSettlement.Name);
			}
			else
			{
				textObject = new TextObject("{=!}{FURTHER_DETAILS}.", null);
			}
			StringHelpers.SetCharacterProperties("CHARACTER", hero.CharacterObject, textObject, true);
			TextObject textObject2 = TextObject.Empty;
			if (detail == KillCharacterAction.KillCharacterActionDetail.DiedInBattle)
			{
				textObject2 = new TextObject("{=6pCABUme}{?CHARACTER.GENDER}She{?}He{\\?} died in battle in {YEAR} at the age of {CHARACTER.AGE}. {?CHARACTER.GENDER}She{?}He{\\?} was reputed to be {REPUTATION}", null);
			}
			else if (detail == KillCharacterAction.KillCharacterActionDetail.DiedInLabor)
			{
				textObject2 = new TextObject("{=7Vw6iYNI}{?CHARACTER.GENDER}She{?}He{\\?} died in childbirth in {YEAR} at the age of {CHARACTER.AGE}. {?CHARACTER.GENDER}She{?}He{\\?} was reputed to be {REPUTATION}", null);
			}
			else if (detail == KillCharacterAction.KillCharacterActionDetail.Executed)
			{
				textObject2 = new TextObject("{=9Tq3IAiz}{?CHARACTER.GENDER}She{?}He{\\?} was executed in {YEAR} at the age of {CHARACTER.AGE}. {?CHARACTER.GENDER}She{?}He{\\?} was reputed to be {REPUTATION}", null);
			}
			else if (detail == KillCharacterAction.KillCharacterActionDetail.Lost)
			{
				textObject2 = new TextObject("{=SausWqM5}{?CHARACTER.GENDER}She{?}He{\\?} disappeared in {YEAR} at the age of {CHARACTER.AGE}. {?CHARACTER.GENDER}She{?}He{\\?} was reputed to be {REPUTATION}", null);
			}
			else if (detail == KillCharacterAction.KillCharacterActionDetail.Murdered)
			{
				textObject2 = new TextObject("{=TUDAvcTR}{?CHARACTER.GENDER}She{?}He{\\?} was assassinated in {YEAR} at the age of {CHARACTER.AGE}. {?CHARACTER.GENDER}She{?}He{\\?} was reputed to be {REPUTATION}", null);
			}
			else if (detail == KillCharacterAction.KillCharacterActionDetail.WoundedInBattle)
			{
				textObject2 = new TextObject("{=LsBCQtVX}{?CHARACTER.GENDER}She{?}He{\\?} died of war-wounds in {YEAR} at the age of {CHARACTER.AGE}. {?CHARACTER.GENDER}She{?}He{\\?} was reputed to be {REPUTATION}", null);
			}
			else
			{
				textObject2 = new TextObject("{=HU5n5KTW}{?CHARACTER.GENDER}She{?}He{\\?} died of natural causes in {YEAR} at the age of {CHARACTER.AGE}. {?CHARACTER.GENDER}She{?}He{\\?} was reputed to be {REPUTATION}", null);
			}
			StringHelpers.SetCharacterProperties("CHARACTER", hero.CharacterObject, textObject2, true);
			textObject2.SetTextVariable("REPUTATION", CharacterHelper.GetReputationDescription(hero.CharacterObject));
			textObject2.SetTextVariable("YEAR", CampaignTime.Now.GetYear.ToString());
			textObject.SetTextVariable("FURTHER_DETAILS", textObject2);
			return textObject;
		}

		public enum KillCharacterActionDetail
		{
			None,
			Murdered,
			DiedInLabor,
			DiedOfOldAge,
			DiedInBattle,
			WoundedInBattle,
			Executed,
			Lost
		}
	}
}
