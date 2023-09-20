using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.CampaignSystem.MapNotificationTypes;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Siege;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	// Token: 0x02000388 RID: 904
	public class DefaultLogsCampaignBehavior : CampaignBehaviorBase
	{
		// Token: 0x0600352F RID: 13615 RVA: 0x000E5648 File Offset: 0x000E3848
		public override void RegisterEvents()
		{
			CampaignEvents.AlleyOwnerChanged.AddNonSerializedListener(this, new Action<Alley, Hero, Hero>(this.OnAlleyOwnerChanged));
			CampaignEvents.ArmyGathered.AddNonSerializedListener(this, new Action<Army, Settlement>(this.OnArmyGathered));
			CampaignEvents.BattleStarted.AddNonSerializedListener(this, new Action<PartyBase, PartyBase, object, bool>(this.OnBattleStarted));
			CampaignEvents.CharacterBecameFugitive.AddNonSerializedListener(this, new Action<Hero>(this.OnCharacterBecameFugitive));
			CampaignEvents.ClanChangedKingdom.AddNonSerializedListener(this, new Action<Clan, Kingdom, Kingdom, ChangeKingdomAction.ChangeKingdomActionDetail, bool>(this.ClanChangedKingdom));
			CampaignEvents.HeroPrisonerTaken.AddNonSerializedListener(this, new Action<PartyBase, Hero>(this.OnPrisonerTaken));
			CampaignEvents.HeroPrisonerReleased.AddNonSerializedListener(this, new Action<Hero, PartyBase, IFaction, EndCaptivityDetail>(this.OnPrisonerReleased));
			CampaignEvents.HeroesMarried.AddNonSerializedListener(this, new Action<Hero, Hero, bool>(this.OnHeroesMarried));
			CampaignEvents.ArmyDispersed.AddNonSerializedListener(this, new Action<Army, Army.ArmyDispersionReason, bool>(this.OnArmyDispersed));
			CampaignEvents.ArmyCreated.AddNonSerializedListener(this, new Action<Army>(this.OnArmyCreated));
			CampaignEvents.RebellionFinished.AddNonSerializedListener(this, new Action<Settlement, Clan>(this.OnRebellionFinished));
			CampaignEvents.KingdomDecisionAdded.AddNonSerializedListener(this, new Action<KingdomDecision, bool>(this.OnKingdomDecisionAdded));
			CampaignEvents.KingdomDecisionConcluded.AddNonSerializedListener(this, new Action<KingdomDecision, DecisionOutcome, bool>(this.OnKingdomDecisionConcluded));
			CampaignEvents.TournamentFinished.AddNonSerializedListener(this, new Action<CharacterObject, MBReadOnlyList<CharacterObject>, Town, ItemObject>(this.OnTournamentFinished));
			CampaignEvents.OnSiegeEventStartedEvent.AddNonSerializedListener(this, new Action<SiegeEvent>(this.OnSiegeEventStarted));
			CampaignEvents.PlayerTraitChangedEvent.AddNonSerializedListener(this, new Action<TraitObject, int>(this.OnPlayerTraitChanged));
			CampaignEvents.OnPlayerCharacterChangedEvent.AddNonSerializedListener(this, new Action<Hero, Hero, MobileParty, bool>(this.OnPlayerCharacterChanged));
			CampaignEvents.OnSiegeAftermathAppliedEvent.AddNonSerializedListener(this, new Action<MobileParty, Settlement, SiegeAftermathAction.SiegeAftermath, Clan, Dictionary<MobileParty, float>>(this.OnSiegeAftermathApplied));
		}

		// Token: 0x06003530 RID: 13616 RVA: 0x000E57F3 File Offset: 0x000E39F3
		private void OnSiegeAftermathApplied(MobileParty attackerParty, Settlement settlement, SiegeAftermathAction.SiegeAftermath aftermathType, Clan previousSettlementOwner, Dictionary<MobileParty, float> partyContributions)
		{
			LogEntry.AddLogEntry(new SiegeAftermathLogEntry(attackerParty, partyContributions.Keys, settlement, aftermathType));
		}

		// Token: 0x06003531 RID: 13617 RVA: 0x000E5809 File Offset: 0x000E3A09
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x06003532 RID: 13618 RVA: 0x000E580B File Offset: 0x000E3A0B
		private void OnPlayerCharacterChanged(Hero oldPlayer, Hero newPlayer, MobileParty newMobileParty, bool isMainPartyChanged)
		{
			LogEntry.AddLogEntry(new PlayerCharacterChangedLogEntry(oldPlayer, newPlayer));
		}

		// Token: 0x06003533 RID: 13619 RVA: 0x000E5819 File Offset: 0x000E3A19
		private void OnPrisonerTaken(PartyBase party, Hero hero)
		{
			LogEntry.AddLogEntry(new TakePrisonerLogEntry(party, hero));
		}

		// Token: 0x06003534 RID: 13620 RVA: 0x000E5827 File Offset: 0x000E3A27
		private void OnPrisonerReleased(Hero hero, PartyBase party, IFaction captuererFaction, EndCaptivityDetail detail)
		{
			LogEntry.AddLogEntry(new EndCaptivityLogEntry(hero, captuererFaction, detail));
		}

		// Token: 0x06003535 RID: 13621 RVA: 0x000E5837 File Offset: 0x000E3A37
		private void OnCommonAreaFightOccured(MobileParty attackerParty, MobileParty defenderParty, Hero attackerHero, Settlement settlement)
		{
			LogEntry.AddLogEntry(new CommonAreaFightLogEntry(attackerParty, defenderParty, attackerHero, settlement));
		}

		// Token: 0x06003536 RID: 13622 RVA: 0x000E5848 File Offset: 0x000E3A48
		private void ClanChangedKingdom(Clan clan, Kingdom oldKingdom, Kingdom newKingdom, ChangeKingdomAction.ChangeKingdomActionDetail detail, bool showNotifications)
		{
			if (detail == ChangeKingdomAction.ChangeKingdomActionDetail.JoinAsMercenary || detail == ChangeKingdomAction.ChangeKingdomActionDetail.LeaveAsMercenary)
			{
				LogEntry.AddLogEntry(new MercenaryClanChangedKingdomLogEntry(clan, oldKingdom, newKingdom));
			}
		}

		// Token: 0x06003537 RID: 13623 RVA: 0x000E5860 File Offset: 0x000E3A60
		private void OnCharacterBecameFugitive(Hero hero)
		{
			LogEntry.AddLogEntry(new CharacterBecameFugitiveLogEntry(hero));
		}

		// Token: 0x06003538 RID: 13624 RVA: 0x000E586D File Offset: 0x000E3A6D
		private void OnBattleStarted(PartyBase attackerParty, PartyBase defenderParty, object subject, bool showNotification)
		{
			if (showNotification)
			{
				LogEntry.AddLogEntry(new BattleStartedLogEntry(attackerParty, defenderParty, subject));
			}
		}

		// Token: 0x06003539 RID: 13625 RVA: 0x000E5880 File Offset: 0x000E3A80
		public void OnArmyDispersed(Army army, Army.ArmyDispersionReason reason, bool isPlayersArmy)
		{
			if (isPlayersArmy)
			{
				ArmyDispersionLogEntry armyDispersionLogEntry = new ArmyDispersionLogEntry(army, reason);
				LogEntry.AddLogEntry(armyDispersionLogEntry);
				if (army.LeaderParty.MapFaction == Hero.MainHero.MapFaction && army.Parties.IndexOf(MobileParty.MainParty) < 0)
				{
					Campaign.Current.CampaignInformationManager.NewMapNoticeAdded(new ArmyDispersionMapNotification(army, reason, armyDispersionLogEntry.GetEncyclopediaText()));
				}
			}
		}

		// Token: 0x0600353A RID: 13626 RVA: 0x000E58E4 File Offset: 0x000E3AE4
		private void OnArmyGathered(Army army, Settlement targetSettlement)
		{
			LogEntry.AddLogEntry(new GatherArmyLogEntry(army, targetSettlement));
		}

		// Token: 0x0600353B RID: 13627 RVA: 0x000E58F4 File Offset: 0x000E3AF4
		private void OnArmyCreated(Army army)
		{
			ArmyCreationLogEntry armyCreationLogEntry = new ArmyCreationLogEntry(army);
			LogEntry.AddLogEntry(armyCreationLogEntry);
			if (army.LeaderParty.MapFaction == MobileParty.MainParty.MapFaction && army.LeaderParty != MobileParty.MainParty)
			{
				Campaign.Current.CampaignInformationManager.NewMapNoticeAdded(new ArmyCreationMapNotification(army, armyCreationLogEntry.GetEncyclopediaText()));
			}
		}

		// Token: 0x0600353C RID: 13628 RVA: 0x000E5950 File Offset: 0x000E3B50
		private void OnRebellionFinished(Settlement settlement, Clan oldOwnerClan)
		{
			RebellionStartedLogEntry rebellionStartedLogEntry = new RebellionStartedLogEntry(settlement, oldOwnerClan);
			LogEntry.AddLogEntry(rebellionStartedLogEntry);
			if (oldOwnerClan == Clan.PlayerClan)
			{
				Campaign.Current.CampaignInformationManager.NewMapNoticeAdded(new SettlementRebellionMapNotification(settlement, rebellionStartedLogEntry.GetNotificationText()));
			}
		}

		// Token: 0x0600353D RID: 13629 RVA: 0x000E5990 File Offset: 0x000E3B90
		private void OnKingdomDecisionAdded(KingdomDecision decision, bool isPlayerInvolved)
		{
			LogEntry.AddLogEntry(new KingdomDecisionAddedLogEntry(decision, isPlayerInvolved));
			if (decision.NotifyPlayer && isPlayerInvolved && !decision.IsEnforced)
			{
				TextObject textObject = (decision.DetermineChooser().Leader.IsHumanPlayerCharacter ? decision.GetChooseTitle() : decision.GetSupportTitle());
				Campaign.Current.CampaignInformationManager.NewMapNoticeAdded(new KingdomDecisionMapNotification(decision.Kingdom, decision, textObject));
			}
		}

		// Token: 0x0600353E RID: 13630 RVA: 0x000E59F8 File Offset: 0x000E3BF8
		private void OnKingdomDecisionConcluded(KingdomDecision decision, DecisionOutcome chosenOutcome, bool isPlayerInvolved)
		{
			KingdomDecisionConcludedLogEntry kingdomDecisionConcludedLogEntry = new KingdomDecisionConcludedLogEntry(decision, chosenOutcome, isPlayerInvolved);
			LogEntry.AddLogEntry(kingdomDecisionConcludedLogEntry);
			if (decision.Kingdom == Hero.MainHero.MapFaction && decision.NotifyPlayer && !decision.IsEnforced && !isPlayerInvolved)
			{
				MBInformationManager.AddQuickInformation(kingdomDecisionConcludedLogEntry.GetNotificationText(), 0, null, "event:/ui/notification/kingdom_decision");
				Campaign.Current.CampaignInformationManager.NewMapNoticeAdded(new KingdomDecisionMapNotification(decision.Kingdom, decision, kingdomDecisionConcludedLogEntry.GetNotificationText()));
			}
		}

		// Token: 0x0600353F RID: 13631 RVA: 0x000E5A6C File Offset: 0x000E3C6C
		private void OnAlleyOwnerChanged(Alley alley, Hero newOwner, Hero oldOwner)
		{
			LogEntry.AddLogEntry(new ChangeAlleyOwnerLogEntry(alley, newOwner, oldOwner));
		}

		// Token: 0x06003540 RID: 13632 RVA: 0x000E5A7C File Offset: 0x000E3C7C
		private void OnHeroesMarried(Hero marriedHero, Hero marriedTo, bool showNotification)
		{
			CharacterMarriedLogEntry characterMarriedLogEntry = new CharacterMarriedLogEntry(marriedHero, marriedTo);
			LogEntry.AddLogEntry(characterMarriedLogEntry);
			if (marriedHero.Clan == Clan.PlayerClan || marriedTo.Clan == Clan.PlayerClan)
			{
				Campaign.Current.CampaignInformationManager.NewMapNoticeAdded(new MarriageMapNotification(marriedHero, marriedTo, characterMarriedLogEntry.GetEncyclopediaText()));
			}
		}

		// Token: 0x06003541 RID: 13633 RVA: 0x000E5AD0 File Offset: 0x000E3CD0
		private void OnSiegeEventStarted(SiegeEvent siegeEvent)
		{
			BesiegeSettlementLogEntry besiegeSettlementLogEntry = new BesiegeSettlementLogEntry(siegeEvent.BesiegerCamp.BesiegerParty, siegeEvent.BesiegedSettlement);
			LogEntry.AddLogEntry(besiegeSettlementLogEntry);
			if (siegeEvent.BesiegedSettlement.OwnerClan == Clan.PlayerClan)
			{
				Campaign.Current.CampaignInformationManager.NewMapNoticeAdded(new SettlementUnderSiegeMapNotification(siegeEvent, besiegeSettlementLogEntry.GetEncyclopediaText()));
			}
		}

		// Token: 0x06003542 RID: 13634 RVA: 0x000E5B27 File Offset: 0x000E3D27
		private void OnTournamentFinished(CharacterObject character, MBReadOnlyList<CharacterObject> participants, Town town, ItemObject prize)
		{
			if (character.IsHero)
			{
				LogEntry.AddLogEntry(new TournamentWonLogEntry(character.HeroObject, town, participants));
			}
		}

		// Token: 0x06003543 RID: 13635 RVA: 0x000E5B44 File Offset: 0x000E3D44
		private void OnPlayerTraitChanged(TraitObject trait, int previousLevel)
		{
			int traitLevel = Hero.MainHero.GetTraitLevel(trait);
			TextObject traitChangedText = DefaultLogsCampaignBehavior.GetTraitChangedText(trait, traitLevel, previousLevel);
			Campaign.Current.CampaignInformationManager.NewMapNoticeAdded(new TraitChangedMapNotification(trait, traitLevel != 0, previousLevel, traitChangedText));
		}

		// Token: 0x06003544 RID: 13636 RVA: 0x000E5B84 File Offset: 0x000E3D84
		private static TextObject GetTraitChangedText(TraitObject traitObject, int level, int previousLevel)
		{
			TextObject textObject;
			TextObject textObject2;
			if (level != 0)
			{
				textObject = GameTexts.FindText("str_trait_name_" + traitObject.StringId.ToLower(), (level + MathF.Abs(traitObject.MinValue)).ToString());
				textObject2 = GameTexts.FindText("str_trait_gained_text", null);
			}
			else
			{
				textObject = GameTexts.FindText("str_trait_name_" + traitObject.StringId.ToLower(), (previousLevel + MathF.Abs(traitObject.MinValue)).ToString());
				textObject2 = GameTexts.FindText("str_trait_lost_text", null);
			}
			textObject2.SetCharacterProperties("HERO", Hero.MainHero.CharacterObject, false);
			textObject2.SetTextVariable("TRAIT_NAME", textObject);
			return textObject2;
		}
	}
}
