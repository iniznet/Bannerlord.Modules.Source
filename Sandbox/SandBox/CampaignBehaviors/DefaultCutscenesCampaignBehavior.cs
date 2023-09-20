using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.MapNotificationTypes;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.SceneInformationPopupTypes;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace SandBox.CampaignBehaviors
{
	// Token: 0x0200009B RID: 155
	public class DefaultCutscenesCampaignBehavior : CampaignBehaviorBase
	{
		// Token: 0x0600079E RID: 1950 RVA: 0x0003BE90 File Offset: 0x0003A090
		public override void RegisterEvents()
		{
			CampaignEvents.HeroesMarried.AddNonSerializedListener(this, new Action<Hero, Hero, bool>(DefaultCutscenesCampaignBehavior.OnHeroesMarried));
			CampaignEvents.MapEventEnded.AddNonSerializedListener(this, new Action<MapEvent>(this.OnMapEventEnd));
			CampaignEvents.HeroComesOfAgeEvent.AddNonSerializedListener(this, new Action<Hero>(this.OnHeroComesOfAge));
			CampaignEvents.KingdomCreatedEvent.AddNonSerializedListener(this, new Action<Kingdom>(this.OnKingdomCreated));
			CampaignEvents.KingdomDestroyedEvent.AddNonSerializedListener(this, new Action<Kingdom>(this.OnKingdomDestroyed));
			CampaignEvents.ClanChangedKingdom.AddNonSerializedListener(this, new Action<Clan, Kingdom, Kingdom, ChangeKingdomAction.ChangeKingdomActionDetail, bool>(this.OnClanChangedKingdom));
			CampaignEvents.KingdomDecisionConcluded.AddNonSerializedListener(this, new Action<KingdomDecision, DecisionOutcome, bool>(this.OnKingdomDecisionConcluded));
			CampaignEvents.OnBeforeMainCharacterDiedEvent.AddNonSerializedListener(this, new Action<Hero, Hero, KillCharacterAction.KillCharacterActionDetail, bool>(this.OnBeforeMainCharacterDied));
		}

		// Token: 0x0600079F RID: 1951 RVA: 0x0003BF58 File Offset: 0x0003A158
		private void OnBeforeMainCharacterDied(Hero victim, Hero killer, KillCharacterAction.KillCharacterActionDetail detail, bool showNotification = true)
		{
			SceneNotificationData sceneNotificationData = null;
			if (victim == Hero.MainHero)
			{
				if (detail == 3)
				{
					sceneNotificationData = new DeathOldAgeSceneNotificationItem(victim);
				}
				else if (detail == 4)
				{
					if (this._heroWonLastMapEVent)
					{
						bool flag = !victim.CompanionsInParty.Any<Hero>();
						List<CharacterObject> list = new List<CharacterObject>();
						DefaultCutscenesCampaignBehavior.FillAllyCharacters(flag, ref list);
						sceneNotificationData = new MainHeroBattleVictoryDeathNotificationItem(victim, list);
					}
					else
					{
						sceneNotificationData = new MainHeroBattleDeathNotificationItem(victim, this._lastEnemyCulture);
					}
				}
				else if (detail == 6)
				{
					TextObject textObject = new TextObject("{=uYjEknNX}{VICTIM.NAME}'s execution by {EXECUTER.NAME}", null);
					TextObjectExtensions.SetCharacterProperties(textObject, "VICTIM", victim.CharacterObject, false);
					TextObjectExtensions.SetCharacterProperties(textObject, "EXECUTER", killer.CharacterObject, false);
					sceneNotificationData = HeroExecutionSceneNotificationData.CreateForInformingPlayer(killer, victim, 4);
				}
			}
			if (sceneNotificationData != null)
			{
				MBInformationManager.ShowSceneNotification(sceneNotificationData);
			}
		}

		// Token: 0x060007A0 RID: 1952 RVA: 0x0003C004 File Offset: 0x0003A204
		private void OnKingdomDecisionConcluded(KingdomDecision decision, DecisionOutcome chosenOutcome, bool isPlayerInvolved)
		{
			KingSelectionKingdomDecision.KingSelectionDecisionOutcome kingSelectionDecisionOutcome;
			if ((kingSelectionDecisionOutcome = chosenOutcome as KingSelectionKingdomDecision.KingSelectionDecisionOutcome) != null && isPlayerInvolved && kingSelectionDecisionOutcome.King == Hero.MainHero)
			{
				MBInformationManager.ShowSceneNotification(new BecomeKingSceneNotificationItem(kingSelectionDecisionOutcome.King));
			}
		}

		// Token: 0x060007A1 RID: 1953 RVA: 0x0003C040 File Offset: 0x0003A240
		private void OnClanChangedKingdom(Clan clan, Kingdom oldKingdom, Kingdom newKingdom, ChangeKingdomAction.ChangeKingdomActionDetail detail, bool showNotification = true)
		{
			SceneNotificationData sceneNotificationData = null;
			if (clan == Clan.PlayerClan && detail == 1)
			{
				sceneNotificationData = new JoinKingdomSceneNotificationItem(clan, newKingdom);
			}
			else if (Clan.PlayerClan.Kingdom == newKingdom && detail == 2)
			{
				sceneNotificationData = new JoinKingdomSceneNotificationItem(clan, newKingdom);
			}
			if (sceneNotificationData != null)
			{
				MBInformationManager.ShowSceneNotification(sceneNotificationData);
			}
		}

		// Token: 0x060007A2 RID: 1954 RVA: 0x0003C08C File Offset: 0x0003A28C
		private void OnKingdomDestroyed(Kingdom kingdom)
		{
			if (!kingdom.IsRebelClan)
			{
				if (kingdom.Leader == Hero.MainHero)
				{
					MBInformationManager.ShowSceneNotification(Campaign.Current.Models.CutsceneSelectionModel.GetKingdomDestroyedSceneNotification(kingdom));
					return;
				}
				Campaign.Current.CampaignInformationManager.NewMapNoticeAdded(new KingdomDestroyedMapNotification(kingdom));
			}
		}

		// Token: 0x060007A3 RID: 1955 RVA: 0x0003C0DE File Offset: 0x0003A2DE
		private void OnKingdomCreated(Kingdom kingdom)
		{
			if (Hero.MainHero.Clan.Kingdom == kingdom)
			{
				MBInformationManager.ShowSceneNotification(new KingdomCreatedSceneNotificationItem(kingdom));
			}
		}

		// Token: 0x060007A4 RID: 1956 RVA: 0x0003C100 File Offset: 0x0003A300
		private void OnHeroComesOfAge(Hero hero)
		{
			Hero mother = hero.Mother;
			if (((mother != null) ? mother.Clan : null) != Clan.PlayerClan)
			{
				Hero father = hero.Father;
				if (((father != null) ? father.Clan : null) != Clan.PlayerClan)
				{
					return;
				}
			}
			Hero mentorHeroForComeOfAge = this.GetMentorHeroForComeOfAge(hero);
			TextObject textObject = new TextObject("{=t4KwQOB7}{HERO.NAME} is now of age.", null);
			TextObjectExtensions.SetCharacterProperties(textObject, "HERO", hero.CharacterObject, false);
			Campaign.Current.CampaignInformationManager.NewMapNoticeAdded(new HeirComeOfAgeMapNotification(hero, mentorHeroForComeOfAge, textObject));
		}

		// Token: 0x060007A5 RID: 1957 RVA: 0x0003C17C File Offset: 0x0003A37C
		private void OnMapEventEnd(MapEvent mapEvent)
		{
			if (mapEvent.IsPlayerMapEvent)
			{
				this._heroWonLastMapEVent = mapEvent.WinningSide == mapEvent.PlayerSide;
				this._lastEnemyCulture = ((mapEvent.PlayerSide == 1) ? mapEvent.DefenderSide.MapFaction.Culture : mapEvent.AttackerSide.MapFaction.Culture);
			}
		}

		// Token: 0x060007A6 RID: 1958 RVA: 0x0003C1D6 File Offset: 0x0003A3D6
		private static void OnHeroesMarried(Hero firstHero, Hero secondHero, bool showNotification)
		{
			if (firstHero == Hero.MainHero || secondHero == Hero.MainHero)
			{
				Hero hero = (firstHero.IsFemale ? secondHero : firstHero);
				MBInformationManager.ShowSceneNotification(new MarriageSceneNotificationItem(hero, hero.Spouse, 0));
			}
		}

		// Token: 0x060007A7 RID: 1959 RVA: 0x0003C205 File Offset: 0x0003A405
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x060007A8 RID: 1960 RVA: 0x0003C208 File Offset: 0x0003A408
		private static void FillAllyCharacters(bool noCompanions, ref List<CharacterObject> allyCharacters)
		{
			if (noCompanions)
			{
				allyCharacters.Add(Hero.MainHero.MapFaction.Culture.RangedEliteMilitiaTroop);
				return;
			}
			List<CharacterObject> list = (from c in MobileParty.MainParty.MemberRoster.GetTroopRoster()
				where c.Character != CharacterObject.PlayerCharacter && c.Character.IsHero
				select c into t
				select t.Character).ToList<CharacterObject>();
			allyCharacters.AddRange(list.Take(3));
			int count = allyCharacters.Count;
			for (int i = 0; i < 3 - count; i++)
			{
				allyCharacters.Add(Extensions.GetRandomElement<Hero>(Hero.AllAliveHeroes).CharacterObject);
			}
		}

		// Token: 0x060007A9 RID: 1961 RVA: 0x0003C2CC File Offset: 0x0003A4CC
		private Hero GetMentorHeroForComeOfAge(Hero hero)
		{
			Hero hero2 = Hero.MainHero;
			if (hero.IsFemale)
			{
				if (hero.Mother != null && hero.Mother.IsAlive)
				{
					hero2 = hero.Mother;
				}
				else if (hero.Father != null && hero.Father.IsAlive)
				{
					hero2 = hero.Father;
				}
			}
			else if (hero.Father != null && hero.Father.IsAlive)
			{
				hero2 = hero.Father;
			}
			else if (hero.Mother != null && hero.Mother.IsAlive)
			{
				hero2 = hero.Mother;
			}
			if (hero.Mother == Hero.MainHero || hero.Father == Hero.MainHero)
			{
				hero2 = Hero.MainHero;
			}
			return hero2;
		}

		// Token: 0x0400031E RID: 798
		private bool _heroWonLastMapEVent;

		// Token: 0x0400031F RID: 799
		private CultureObject _lastEnemyCulture;
	}
}
