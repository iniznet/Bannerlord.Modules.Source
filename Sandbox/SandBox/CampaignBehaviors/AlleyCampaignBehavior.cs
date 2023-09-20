using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using SandBox.Conversation;
using SandBox.Missions.AgentBehaviors;
using SandBox.Missions.MissionLogics;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.AgentOrigins;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.CampaignSystem.MapNotificationTypes;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace SandBox.CampaignBehaviors
{
	// Token: 0x02000091 RID: 145
	public class AlleyCampaignBehavior : CampaignBehaviorBase, IAlleyCampaignBehavior, ICampaignBehavior
	{
		// Token: 0x06000612 RID: 1554 RVA: 0x0002E568 File Offset: 0x0002C768
		public override void RegisterEvents()
		{
			CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));
			CampaignEvents.HeroKilledEvent.AddNonSerializedListener(this, new Action<Hero, Hero, KillCharacterAction.KillCharacterActionDetail, bool>(this.OnHeroKilled));
			CampaignEvents.LocationCharactersAreReadyToSpawnEvent.AddNonSerializedListener(this, new Action<Dictionary<string, int>>(this.LocationCharactersAreReadyToSpawn));
			CampaignEvents.OnNewGameCreatedPartialFollowUpEndEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnNewGameCreated));
			CampaignEvents.AlleyOccupiedByPlayer.AddNonSerializedListener(this, new Action<Alley, TroopRoster>(this.OnAlleyOccupiedByPlayer));
			CampaignEvents.AlleyClearedByPlayer.AddNonSerializedListener(this, new Action<Alley>(this.OnAlleyClearedByPlayer));
			CampaignEvents.AlleyOwnerChanged.AddNonSerializedListener(this, new Action<Alley, Hero, Hero>(this.OnAlleyOwnerChanged));
			CampaignEvents.DailyTickSettlementEvent.AddNonSerializedListener(this, new Action<Settlement>(this.DailyTickSettlement));
			CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, new Action(this.DailyTick));
			CampaignEvents.CanHeroLeadPartyEvent.AddNonSerializedListener(this, new ReferenceAction<Hero, bool>(this.CommonAlleyLeaderRestriction));
			CampaignEvents.CanBeGovernorOrHavePartyRoleEvent.AddNonSerializedListener(this, new ReferenceAction<Hero, bool>(this.CommonAlleyLeaderRestriction));
			CampaignEvents.AfterMissionStarted.AddNonSerializedListener(this, new Action<IMission>(this.OnAfterMissionStarted));
			CampaignEvents.CanHeroDieEvent.AddNonSerializedListener(this, new ReferenceAction<Hero, KillCharacterAction.KillCharacterActionDetail, bool>(this.CanHeroDie));
		}

		// Token: 0x06000613 RID: 1555 RVA: 0x0002E6A0 File Offset: 0x0002C8A0
		private void CanHeroDie(Hero hero, KillCharacterAction.KillCharacterActionDetail detail, ref bool result)
		{
			if (hero == Hero.MainHero && Mission.Current != null && this._playerIsInAlleyFightMission)
			{
				result = false;
			}
		}

		// Token: 0x06000614 RID: 1556 RVA: 0x0002E6BC File Offset: 0x0002C8BC
		private void OnAfterMissionStarted(IMission mission)
		{
			this._playerIsInAlleyFightMission = false;
		}

		// Token: 0x06000615 RID: 1557 RVA: 0x0002E6C5 File Offset: 0x0002C8C5
		private void OnAlleyOwnerChanged(Alley alley, Hero newOwner, Hero oldOwner)
		{
			if (oldOwner == Hero.MainHero)
			{
				TextObject textObject = new TextObject("{=wAgfOHio}You have lost the ownership of the alley at {SETTLEMENT}.", null);
				textObject.SetTextVariable("SETTLEMENT", alley.Settlement.Name);
				MBInformationManager.AddQuickInformation(textObject, 0, null, "");
			}
		}

		// Token: 0x06000616 RID: 1558 RVA: 0x0002E700 File Offset: 0x0002C900
		private void CommonAlleyLeaderRestriction(Hero hero, ref bool result)
		{
			if (this._playerOwnedCommonAreaData.Any((AlleyCampaignBehavior.PlayerAlleyData x) => x.AssignedClanMember == hero))
			{
				result = false;
			}
		}

		// Token: 0x06000617 RID: 1559 RVA: 0x0002E738 File Offset: 0x0002C938
		private void DailyTick()
		{
			for (int i = this._playerOwnedCommonAreaData.Count - 1; i >= 0; i--)
			{
				AlleyCampaignBehavior.PlayerAlleyData playerAlleyData = this._playerOwnedCommonAreaData[i];
				this.CheckConvertTroopsToBandits(playerAlleyData);
				SkillLevelingManager.OnDailyAlleyTick(playerAlleyData.Alley, playerAlleyData.AssignedClanMember);
				if (playerAlleyData.AssignedClanMember.IsDead && playerAlleyData.AssignedClanMember.DeathDay + Campaign.Current.Models.AlleyModel.DestroyAlleyAfterDaysWhenLeaderIsDeath < CampaignTime.Now)
				{
					this._playerOwnedCommonAreaData.Remove(playerAlleyData);
					playerAlleyData.DestroyAlley(false);
				}
				else if (!playerAlleyData.IsUnderAttack && !playerAlleyData.AssignedClanMember.IsDead)
				{
					this.CheckSpawningNewAlleyFight(playerAlleyData);
				}
				else if (playerAlleyData.IsUnderAttack && playerAlleyData.AttackResponseDueDate.IsPast)
				{
					this._playerOwnedCommonAreaData.Remove(playerAlleyData);
					playerAlleyData.DestroyAlley(false);
				}
			}
		}

		// Token: 0x06000618 RID: 1560 RVA: 0x0002E824 File Offset: 0x0002CA24
		private void CheckSpawningNewAlleyFight(AlleyCampaignBehavior.PlayerAlleyData playerOwnedArea)
		{
			if (MBRandom.RandomFloat < 0.015f)
			{
				if (playerOwnedArea.Alley.Settlement.Alleys.Any((Alley x) => x.State == 1))
				{
					this.StartNewAlleyAttack(playerOwnedArea);
				}
			}
		}

		// Token: 0x06000619 RID: 1561 RVA: 0x0002E87C File Offset: 0x0002CA7C
		private void StartNewAlleyAttack(AlleyCampaignBehavior.PlayerAlleyData playerOwnedArea)
		{
			playerOwnedArea.UnderAttackBy = Extensions.GetRandomElementInefficiently<Alley>(playerOwnedArea.Alley.Settlement.Alleys.Where((Alley x) => x.State == 1));
			playerOwnedArea.UnderAttackBy.Owner.SetHasMet();
			float alleyAttackResponseTimeInDays = Campaign.Current.Models.AlleyModel.GetAlleyAttackResponseTimeInDays(playerOwnedArea.TroopRoster);
			playerOwnedArea.AttackResponseDueDate = CampaignTime.DaysFromNow(alleyAttackResponseTimeInDays);
			TextObject textObject = new TextObject("{=5bIpeW9X}Your alley in {SETTLEMENT} is under attack from neighboring gangs. Unless you go to their help, the alley will be lost in {RESPONSE_TIME} days.", null);
			textObject.SetTextVariable("SETTLEMENT", playerOwnedArea.Alley.Settlement.Name);
			textObject.SetTextVariable("RESPONSE_TIME", alleyAttackResponseTimeInDays);
			ChangeRelationAction.ApplyPlayerRelation(playerOwnedArea.UnderAttackBy.Owner, -5, true, true);
			Campaign.Current.CampaignInformationManager.NewMapNoticeAdded(new AlleyUnderAttackMapNotification(playerOwnedArea.Alley, textObject));
		}

		// Token: 0x0600061A RID: 1562 RVA: 0x0002E964 File Offset: 0x0002CB64
		private void CheckConvertTroopsToBandits(AlleyCampaignBehavior.PlayerAlleyData playerOwnedArea)
		{
			foreach (FlattenedTroopRosterElement flattenedTroopRosterElement in playerOwnedArea.TroopRoster.ToFlattenedRoster())
			{
				if (MBRandom.RandomFloat < 0.01f && !flattenedTroopRosterElement.Troop.IsHero && flattenedTroopRosterElement.Troop.Occupation != 27)
				{
					playerOwnedArea.TroopRoster.RemoveTroop(flattenedTroopRosterElement.Troop, 1, default(UniqueTroopDescriptor), 0);
					CharacterObject characterObject = this._thug;
					if (characterObject.Tier < flattenedTroopRosterElement.Troop.Tier)
					{
						characterObject = this._expertThug;
					}
					if (characterObject.Tier < flattenedTroopRosterElement.Troop.Tier)
					{
						characterObject = this._masterThug;
					}
					playerOwnedArea.TroopRoster.AddToCounts(characterObject, 1, false, 0, 0, true, -1);
				}
			}
		}

		// Token: 0x0600061B RID: 1563 RVA: 0x0002EA50 File Offset: 0x0002CC50
		private void OnNewGameCreated(CampaignGameStarter gameStarter)
		{
			foreach (Town town in Town.AllTowns)
			{
				int num = MBRandom.RandomInt(0, town.Settlement.Alleys.Count);
				IEnumerable<Hero> enumerable = town.Settlement.Notables.Where((Hero x) => x.IsGangLeader);
				for (int i = num; i < num + 2; i++)
				{
					town.Settlement.Alleys[i % town.Settlement.Alleys.Count].SetOwner(enumerable.ElementAt(i % enumerable.Count<Hero>()));
				}
			}
		}

		// Token: 0x0600061C RID: 1564 RVA: 0x0002EB34 File Offset: 0x0002CD34
		private void DailyTickSettlement(Settlement settlement)
		{
			this.TickAlleyOwnerships(settlement);
		}

		// Token: 0x0600061D RID: 1565 RVA: 0x0002EB40 File Offset: 0x0002CD40
		private void TickAlleyOwnerships(Settlement settlement)
		{
			foreach (Hero hero in settlement.Notables)
			{
				if (hero.IsGangLeader)
				{
					int count = hero.OwnedAlleys.Count;
					float num = 0.02f - (float)count * 0.005f;
					float num2 = (float)count * 0.005f;
					if (MBRandom.RandomFloat < num)
					{
						Alley alley = settlement.Alleys.FirstOrDefault((Alley x) => x.State == 0);
						if (alley != null)
						{
							alley.SetOwner(hero);
						}
					}
					if (MBRandom.RandomFloat < num2)
					{
						Alley randomElement = Extensions.GetRandomElement<Alley>(hero.OwnedAlleys);
						if (randomElement != null)
						{
							randomElement.SetOwner(null);
						}
					}
				}
			}
		}

		// Token: 0x0600061E RID: 1566 RVA: 0x0002EC20 File Offset: 0x0002CE20
		private void OnAlleyOccupiedByPlayer(Alley alley, TroopRoster troopRoster)
		{
			alley.SetOwner(Hero.MainHero);
			AlleyCampaignBehavior.PlayerAlleyData playerAlleyData = new AlleyCampaignBehavior.PlayerAlleyData(alley, troopRoster);
			this._playerOwnedCommonAreaData.Add(playerAlleyData);
			TeleportHeroAction.ApplyDelayedTeleportToSettlement(playerAlleyData.AssignedClanMember, alley.Settlement);
			if (alley.Settlement.OwnerClan != Clan.PlayerClan)
			{
				ChangeRelationAction.ApplyPlayerRelation(alley.Settlement.Owner, -2, true, true);
			}
			SkillLevelingManager.OnAlleyCleared(alley);
			this.AddPlayerAlleyCharacters(alley);
			Mission.Current.ClearCorpses(false);
		}

		// Token: 0x0600061F RID: 1567 RVA: 0x0002EC9C File Offset: 0x0002CE9C
		private void OnAlleyClearedByPlayer(Alley alley)
		{
			ChangeRelationAction.ApplyPlayerRelation(alley.Owner, -5, true, true);
			foreach (Hero hero in alley.Settlement.Notables)
			{
				if (!hero.IsGangLeader)
				{
					ChangeRelationAction.ApplyPlayerRelation(hero, 1, true, true);
				}
			}
			AlleyCampaignBehavior.PlayerAlleyData playerAlleyData = this._playerOwnedCommonAreaData.FirstOrDefault((AlleyCampaignBehavior.PlayerAlleyData x) => x.Alley.Settlement == Settlement.CurrentSettlement);
			if (((playerAlleyData != null) ? playerAlleyData.UnderAttackBy : null) == alley)
			{
				playerAlleyData.UnderAttackBy = null;
			}
			alley.SetOwner(null);
		}

		// Token: 0x06000620 RID: 1568 RVA: 0x0002ED58 File Offset: 0x0002CF58
		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<List<AlleyCampaignBehavior.PlayerAlleyData>>("_playerOwnedCommonAreaData", ref this._playerOwnedCommonAreaData);
		}

		// Token: 0x06000621 RID: 1569 RVA: 0x0002ED6C File Offset: 0x0002CF6C
		public void OnSessionLaunched(CampaignGameStarter campaignGameStarter)
		{
			this._thug = MBObjectManager.Instance.GetObject<CharacterObject>("gangster_1");
			this._expertThug = MBObjectManager.Instance.GetObject<CharacterObject>("gangster_2");
			this._masterThug = MBObjectManager.Instance.GetObject<CharacterObject>("gangster_3");
			this.AddGameMenus(campaignGameStarter);
			this.AddDialogs(campaignGameStarter);
		}

		// Token: 0x06000622 RID: 1570 RVA: 0x0002EDC8 File Offset: 0x0002CFC8
		private void LocationCharactersAreReadyToSpawn(Dictionary<string, int> unusedUsablePointCount)
		{
			Settlement currentSettlement = Settlement.CurrentSettlement;
			if (currentSettlement.IsTown)
			{
				foreach (Alley alley in currentSettlement.Alleys)
				{
					if (alley.State == 1)
					{
						using (List<TroopRosterElement>.Enumerator enumerator2 = Campaign.Current.Models.AlleyModel.GetTroopsOfAIOwnedAlley(alley).GetTroopRoster().GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								TroopRosterElement troopRosterElement = enumerator2.Current;
								for (int i = 0; i < troopRosterElement.Number; i++)
								{
									this.AddCharacterToAlley(troopRosterElement.Character, alley);
								}
							}
							continue;
						}
					}
					if (alley.State == 2)
					{
						this.AddPlayerAlleyCharacters(alley);
					}
				}
			}
		}

		// Token: 0x06000623 RID: 1571 RVA: 0x0002EEB8 File Offset: 0x0002D0B8
		private void AddPlayerAlleyCharacters(Alley alley)
		{
			if (Mission.Current != null)
			{
				for (int i = Mission.Current.Agents.Count - 1; i >= 0; i--)
				{
					Agent agent = Mission.Current.Agents[i];
					if (agent.IsHuman && !agent.Character.IsHero)
					{
						CampaignAgentComponent component = agent.GetComponent<CampaignAgentComponent>();
						Hero hero;
						if (component == null)
						{
							hero = null;
						}
						else
						{
							AgentNavigator agentNavigator = component.AgentNavigator;
							if (agentNavigator == null)
							{
								hero = null;
							}
							else
							{
								Alley memberOfAlley = agentNavigator.MemberOfAlley;
								hero = ((memberOfAlley != null) ? memberOfAlley.Owner : null);
							}
						}
						if (hero == Hero.MainHero)
						{
							agent.FadeOut(false, true);
						}
					}
				}
			}
			foreach (TroopRosterElement troopRosterElement in this._playerOwnedCommonAreaData.First((AlleyCampaignBehavior.PlayerAlleyData x) => x.Alley == alley).TroopRoster.GetTroopRoster())
			{
				if (!troopRosterElement.Character.IsHero || !troopRosterElement.Character.HeroObject.IsTraveling)
				{
					for (int j = 0; j < troopRosterElement.Number; j++)
					{
						this.AddCharacterToAlley(troopRosterElement.Character, alley);
					}
				}
			}
		}

		// Token: 0x06000624 RID: 1572 RVA: 0x0002EFFC File Offset: 0x0002D1FC
		private void AddCharacterToAlley(CharacterObject character, Alley alley)
		{
			Location locationWithId = Settlement.CurrentSettlement.LocationComplex.GetLocationWithId("center");
			LocationCharacter locationCharacter = null;
			if (character.IsHero)
			{
				Location locationOfCharacter = Settlement.CurrentSettlement.LocationComplex.GetLocationOfCharacter(character.HeroObject);
				if (locationOfCharacter != null && locationOfCharacter == locationWithId)
				{
					return;
				}
				locationCharacter = Settlement.CurrentSettlement.LocationComplex.GetLocationCharacterOfHero(character.HeroObject);
			}
			if (locationCharacter == null)
			{
				Monster monsterWithSuffix = FaceGen.GetMonsterWithSuffix(character.Race, "_settlement");
				int num;
				int num2;
				Campaign.Current.Models.AgeModel.GetAgeLimitForLocation(character, ref num, ref num2, "AlleyGangMember");
				AgentData agentData = new AgentData(new SimpleAgentOrigin(character, -1, null, default(UniqueTroopDescriptor))).Monster(monsterWithSuffix).NoHorses(true).Age(MBRandom.RandomInt(num, num2));
				locationCharacter = new LocationCharacter(agentData, new LocationCharacter.AddBehaviorsDelegate(BehaviorSets.AddFixedCharacterBehaviors), alley.Tag, true, 0, ActionSetCode.GenerateActionSetNameWithSuffix(agentData.AgentMonster, agentData.AgentIsFemale, "_villain"), true, false, null, false, false, true);
			}
			locationCharacter.SpecialTargetTag = alley.Tag;
			locationCharacter.SetAlleyOfCharacter(alley);
			Settlement.CurrentSettlement.LocationComplex.ChangeLocation(locationCharacter, Settlement.CurrentSettlement.LocationComplex.GetLocationOfCharacter(locationCharacter), locationWithId);
			if (character.IsHero)
			{
				CampaignEventDispatcher.Instance.OnHeroGetsBusy(character.HeroObject, 3);
			}
		}

		// Token: 0x06000625 RID: 1573 RVA: 0x0002F14C File Offset: 0x0002D34C
		protected void AddGameMenus(CampaignGameStarter campaignGameSystemStarter)
		{
			campaignGameSystemStarter.AddGameMenuOption("town", "manage_alley", "Go to alley", new GameMenuOption.OnConditionDelegate(this.go_to_alley_on_condition), new GameMenuOption.OnConsequenceDelegate(this.go_to_alley_on_consequence), false, 5, false, null);
			campaignGameSystemStarter.AddGameMenu("manage_alley", "{=dWf6ztYu}You are in your alley by the {ALLEY_TYPE}, {FURTHER_INFO}", new OnInitDelegate(this.manage_alley_menu_on_init), 3, 0, null);
			campaignGameSystemStarter.AddGameMenuOption("manage_alley", "confront_hostile_alley_leader", "{=grhRXqen}Confront {HOSTILE_GANG_LEADER.NAME} about {?HOSTILE_GANG_LEADER.GENDER}her{?}his{\\?} attack on your alley.", new GameMenuOption.OnConditionDelegate(this.alley_under_attack_on_condition), new GameMenuOption.OnConsequenceDelegate(this.alley_under_attack_response_on_consequence), false, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("manage_alley", "change_leader_of_alley", "{=ClyaDhGU}Change the leader of the alley", new GameMenuOption.OnConditionDelegate(this.change_leader_of_alley_on_condition), new GameMenuOption.OnConsequenceDelegate(this.change_leader_of_the_alley_on_consequence), false, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("manage_alley", "manage_alley_troops", "{=QrBCe41Z}Manage alley troops", new GameMenuOption.OnConditionDelegate(this.manage_alley_on_condition), new GameMenuOption.OnConsequenceDelegate(this.manage_troops_of_alley), false, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("manage_alley", "abandon_alley", "{=ELfguvYD}Abandon the alley", new GameMenuOption.OnConditionDelegate(this.abandon_alley_on_condition), new GameMenuOption.OnConsequenceDelegate(this.abandon_alley_are_you_sure_consequence), false, -1, false, null);
			campaignGameSystemStarter.AddGameMenu("manage_alley_abandon_are_you_sure", "{=awjomtnJ}Are you sure?", null, 3, 0, null);
			campaignGameSystemStarter.AddGameMenuOption("manage_alley_abandon_are_you_sure", "abandon_alley_yes", "{=aeouhelq}Yes", new GameMenuOption.OnConditionDelegate(this.alley_continue_on_condition), new GameMenuOption.OnConsequenceDelegate(this.abandon_alley_consequence), false, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("manage_alley_abandon_are_you_sure", "abandon_alley_no", "{=8OkPHu4f}No", new GameMenuOption.OnConditionDelegate(this.alley_go_back_on_condition), new GameMenuOption.OnConsequenceDelegate(this.go_to_alley_on_consequence), true, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("manage_alley", "back", "{=4QNycK7T}Go back", new GameMenuOption.OnConditionDelegate(this.alley_go_back_on_condition), new GameMenuOption.OnConsequenceDelegate(this.leave_alley_menu_consequence), true, -1, false, null);
			campaignGameSystemStarter.AddGameMenu("alley_fight_lost", "{=po79q14T}You have failed to defend your alley against the attack, you have lost the ownership of it.", null, 0, 0, null);
			campaignGameSystemStarter.AddGameMenuOption("alley_fight_lost", "continue", "{=DM6luo3c}Continue", new GameMenuOption.OnConditionDelegate(this.alley_continue_on_condition), new GameMenuOption.OnConsequenceDelegate(this.alley_fight_continue_on_consequence), false, -1, false, null);
			campaignGameSystemStarter.AddGameMenu("alley_fight_won", "{=i1sgAm0F}You have succeeded in defending your alley against the attack. You might want to leave some troops in order to compensate for your losses in the fight.", null, 0, 0, null);
			campaignGameSystemStarter.AddGameMenuOption("alley_fight_won", "continue", "{=DM6luo3c}Continue", new GameMenuOption.OnConditionDelegate(this.alley_continue_on_condition), new GameMenuOption.OnConsequenceDelegate(this.alley_fight_continue_on_consequence), false, -1, false, null);
		}

		// Token: 0x06000626 RID: 1574 RVA: 0x0002F39E File Offset: 0x0002D59E
		private void abandon_alley_are_you_sure_consequence(MenuCallbackArgs args)
		{
			GameMenu.SwitchToMenu("manage_alley_abandon_are_you_sure");
		}

		// Token: 0x06000627 RID: 1575 RVA: 0x0002F3AA File Offset: 0x0002D5AA
		private bool alley_continue_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = 17;
			return true;
		}

		// Token: 0x06000628 RID: 1576 RVA: 0x0002F3B5 File Offset: 0x0002D5B5
		private void alley_fight_continue_on_consequence(MenuCallbackArgs args)
		{
			GameMenu.SwitchToMenu("town");
		}

		// Token: 0x06000629 RID: 1577 RVA: 0x0002F3C1 File Offset: 0x0002D5C1
		private bool alley_go_back_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = 16;
			return true;
		}

		// Token: 0x0600062A RID: 1578 RVA: 0x0002F3CC File Offset: 0x0002D5CC
		private bool abandon_alley_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = 21;
			return true;
		}

		// Token: 0x0600062B RID: 1579 RVA: 0x0002F3D8 File Offset: 0x0002D5D8
		private void alley_under_attack_response_on_consequence(MenuCallbackArgs args)
		{
			AlleyCampaignBehavior.PlayerAlleyData playerAlleyData = this._playerOwnedCommonAreaData.First((AlleyCampaignBehavior.PlayerAlleyData x) => x.Alley.Settlement == Settlement.CurrentSettlement);
			CampaignMapConversation.OpenConversation(new ConversationCharacterData(CharacterObject.PlayerCharacter, null, false, false, false, false, false, false), new ConversationCharacterData(playerAlleyData.UnderAttackBy.Owner.CharacterObject, null, false, false, false, false, false, false));
		}

		// Token: 0x0600062C RID: 1580 RVA: 0x0002F444 File Offset: 0x0002D644
		private bool alley_under_attack_on_condition(MenuCallbackArgs args)
		{
			AlleyCampaignBehavior.PlayerAlleyData playerAlleyData = this._playerOwnedCommonAreaData.FirstOrDefault((AlleyCampaignBehavior.PlayerAlleyData x) => x.Alley.Owner == Hero.MainHero && x.Alley.Settlement == Settlement.CurrentSettlement && x.IsUnderAttack);
			if (playerAlleyData != null)
			{
				args.optionLeaveType = 23;
				StringHelpers.SetCharacterProperties("HOSTILE_GANG_LEADER", playerAlleyData.UnderAttackBy.Owner.CharacterObject, null, false);
				TextObject textObject = new TextObject("{=9t1LGNz6}{RESPONSE_TIME} {?RESPONSE_TIME>1}days{?}day{\\?} left.", null);
				textObject.SetTextVariable("RESPONSE_TIME", this.GetResponseTimeLeftForAttackInDays(playerAlleyData.Alley));
				args.Tooltip = textObject;
				return true;
			}
			return false;
		}

		// Token: 0x0600062D RID: 1581 RVA: 0x0002F4D2 File Offset: 0x0002D6D2
		private bool manage_alley_on_condition(MenuCallbackArgs args)
		{
			if (this.alley_under_attack_on_condition(args))
			{
				args.IsEnabled = false;
				args.Tooltip = new TextObject("{=pdqi2qz1}You can not do this action while your alley is under attack.", null);
			}
			args.optionLeaveType = 29;
			return true;
		}

		// Token: 0x0600062E RID: 1582 RVA: 0x0002F4FE File Offset: 0x0002D6FE
		private bool change_leader_of_alley_on_condition(MenuCallbackArgs args)
		{
			if (this.alley_under_attack_on_condition(args))
			{
				args.IsEnabled = false;
				args.Tooltip = new TextObject("{=pdqi2qz1}You can not do this action while your alley is under attack.", null);
			}
			args.optionLeaveType = 17;
			return true;
		}

		// Token: 0x0600062F RID: 1583 RVA: 0x0002F52A File Offset: 0x0002D72A
		private bool go_to_alley_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = 2;
			return this._playerOwnedCommonAreaData.Any((AlleyCampaignBehavior.PlayerAlleyData x) => x.Alley.Settlement == Settlement.CurrentSettlement);
		}

		// Token: 0x06000630 RID: 1584 RVA: 0x0002F55D File Offset: 0x0002D75D
		private void go_to_alley_on_consequence(MenuCallbackArgs args)
		{
			GameMenu.SwitchToMenu("manage_alley");
		}

		// Token: 0x06000631 RID: 1585 RVA: 0x0002F569 File Offset: 0x0002D769
		private void leave_alley_menu_consequence(MenuCallbackArgs args)
		{
			GameMenu.SwitchToMenu("town_outside");
		}

		// Token: 0x06000632 RID: 1586 RVA: 0x0002F578 File Offset: 0x0002D778
		private void abandon_alley_consequence(MenuCallbackArgs args)
		{
			args.optionLeaveType = 9;
			AlleyCampaignBehavior.PlayerAlleyData playerAlleyData = this._playerOwnedCommonAreaData.First((AlleyCampaignBehavior.PlayerAlleyData x) => x.Alley.Settlement == Settlement.CurrentSettlement);
			this._playerOwnedCommonAreaData.Remove(playerAlleyData);
			playerAlleyData.AbandonTheAlley(false);
			GameMenu.SwitchToMenu("town_outside");
		}

		// Token: 0x06000633 RID: 1587 RVA: 0x0002F5D8 File Offset: 0x0002D7D8
		private void manage_troops_of_alley(MenuCallbackArgs args)
		{
			AlleyHelper.OpenScreenForManagingAlley(this._playerOwnedCommonAreaData.First((AlleyCampaignBehavior.PlayerAlleyData x) => x.Alley.Settlement == Settlement.CurrentSettlement).TroopRoster, new PartyPresentationDoneButtonDelegate(this.OnPartyScreenClosed), new TextObject("{=dQBArrqh}Manage Alley", null), null);
		}

		// Token: 0x06000634 RID: 1588 RVA: 0x0002F634 File Offset: 0x0002D834
		private bool OnPartyScreenClosed(TroopRoster leftMemberRoster, TroopRoster leftPrisonRoster, TroopRoster rightMemberRoster, TroopRoster rightPrisonRoster, FlattenedTroopRoster takenPrisonerRoster, FlattenedTroopRoster releasedPrisonerRoster, bool isForced, PartyBase leftParty, PartyBase rightParty)
		{
			AlleyCampaignBehavior.PlayerAlleyData playerAlleyData = this._playerOwnedCommonAreaData.First((AlleyCampaignBehavior.PlayerAlleyData x) => x.Alley.Settlement == Settlement.CurrentSettlement);
			playerAlleyData.TroopRoster = leftMemberRoster;
			if (Mission.Current != null)
			{
				this.AddPlayerAlleyCharacters(playerAlleyData.Alley);
			}
			return true;
		}

		// Token: 0x06000635 RID: 1589 RVA: 0x0002F688 File Offset: 0x0002D888
		private void change_leader_of_the_alley_on_consequence(MenuCallbackArgs args)
		{
			AlleyHelper.CreateMultiSelectionInquiryForSelectingClanMemberToAlley(this._playerOwnedCommonAreaData.First((AlleyCampaignBehavior.PlayerAlleyData x) => x.Alley.Settlement == Settlement.CurrentSettlement).Alley, new Action<List<InquiryElement>>(this.ChangeAssignedClanMemberOfAlley), null);
		}

		// Token: 0x06000636 RID: 1590 RVA: 0x0002F6D8 File Offset: 0x0002D8D8
		private void ChangeAssignedClanMemberOfAlley(List<InquiryElement> newClanMemberInquiryElement)
		{
			AlleyCampaignBehavior.PlayerAlleyData playerAlleyData = this._playerOwnedCommonAreaData.First((AlleyCampaignBehavior.PlayerAlleyData x) => x.Alley.Settlement == Settlement.CurrentSettlement);
			Hero heroObject = (newClanMemberInquiryElement.First<InquiryElement>().Identifier as CharacterObject).HeroObject;
			this.ChangeTheLeaderOfAlleyInternal(playerAlleyData, heroObject);
		}

		// Token: 0x06000637 RID: 1591 RVA: 0x0002F730 File Offset: 0x0002D930
		private void manage_alley_menu_on_init(MenuCallbackArgs args)
		{
			Campaign.Current.GameMenuManager.MenuLocations.Clear();
			Campaign.Current.GameMenuManager.MenuLocations.Add(Settlement.CurrentSettlement.LocationComplex.GetLocationWithId("alley"));
			AlleyCampaignBehavior.PlayerAlleyData playerAlleyData = this._playerOwnedCommonAreaData.FirstOrDefault((AlleyCampaignBehavior.PlayerAlleyData x) => x.Alley.Settlement == Settlement.CurrentSettlement);
			if (playerAlleyData == null)
			{
				GameMenu.SwitchToMenu(this._playerAbandonedAlleyFromDialogRecently ? "town" : "alley_fight_lost");
				this._playerAbandonedAlleyFromDialogRecently = false;
				return;
			}
			MBTextManager.SetTextVariable("ALLEY_TYPE", playerAlleyData.Alley.Name, false);
			TextObject textObject = TextObject.Empty;
			if (playerAlleyData.AssignedClanMember.IsTraveling)
			{
				textObject = new TextObject("{=AjBYneFr}{CLAN_MEMBER.NAME} is in charge of the alley. {?CLAN_MEMBER.GENDER}She{?}He{\\?} is currently traveling to the alley and will arrive after {HOURS} {?HOURS > 1}hours{?}hour{\\?}.", null);
				int num = MathF.Ceiling(TeleportationHelper.GetHoursLeftForTeleportingHeroToReachItsDestination(playerAlleyData.AssignedClanMember));
				textObject.SetTextVariable("HOURS", num);
				MBTextManager.SetTextVariable("FURTHER_INFO", textObject, false);
			}
			else if (playerAlleyData.AssignedClanMember.IsDead)
			{
				textObject = new TextObject("{=P5UbgK4c}{CLAN_MEMBER.NAME} was in charge of the alley. {?CLAN_MEMBER.GENDER}She{?}He{\\?} is dead. Alley will be abandoned after {REMAINING_DAYS} {?REMAINING_DAYS>1}days{?}day{\\?} unless a new overseer is assigned.", null);
				textObject.SetTextVariable("REMAINING_DAYS", (int)Math.Ceiling((playerAlleyData.AssignedClanMember.DeathDay + Campaign.Current.Models.AlleyModel.DestroyAlleyAfterDaysWhenLeaderIsDeath - CampaignTime.Now).ToDays));
				MBTextManager.SetTextVariable("FURTHER_INFO", textObject, false);
			}
			else
			{
				TextObject textObject2 = new TextObject("{=fcqdfY09}{CLAN_MEMBER.NAME} is in charge of the alley.", null);
				MBTextManager.SetTextVariable("FURTHER_INFO", textObject2, false);
			}
			StringHelpers.SetCharacterProperties("CLAN_MEMBER", playerAlleyData.AssignedClanMember.CharacterObject, null, false);
			if (this._waitForBattleResults)
			{
				this._waitForBattleResults = false;
				playerAlleyData.TroopRoster.AddToCounts(CharacterObject.PlayerCharacter, -1, true, 0, 0, true, -1);
				if ((playerAlleyData.TroopRoster.TotalManCount == 0 && this._playerDiedInMission) || this._playerRetreatedFromMission)
				{
					this._playerOwnedCommonAreaData.Remove(playerAlleyData);
					playerAlleyData.AlleyFightLost();
				}
				else
				{
					playerAlleyData.AlleyFightWon();
				}
				this._playerRetreatedFromMission = false;
				this._playerDiedInMission = false;
			}
			if (this._battleWillStartInCurrentSettlement)
			{
				this.StartAlleyFightWithOtherAlley();
			}
		}

		// Token: 0x06000638 RID: 1592 RVA: 0x0002F94C File Offset: 0x0002DB4C
		private void StartAlleyFightWithOtherAlley()
		{
			AlleyCampaignBehavior.PlayerAlleyData playerAlleyData = this._playerOwnedCommonAreaData.First((AlleyCampaignBehavior.PlayerAlleyData x) => x.Alley.Settlement == Settlement.CurrentSettlement);
			TroopRoster troopRoster = playerAlleyData.TroopRoster;
			if (playerAlleyData.AssignedClanMember.IsTraveling)
			{
				troopRoster.RemoveTroop(playerAlleyData.AssignedClanMember.CharacterObject, 1, default(UniqueTroopDescriptor), 0);
			}
			troopRoster.AddToCounts(CharacterObject.PlayerCharacter, 1, true, 0, 0, true, -1);
			TroopRoster troopsOfAlleyForBattleMission = Campaign.Current.Models.AlleyModel.GetTroopsOfAlleyForBattleMission(playerAlleyData.UnderAttackBy);
			int wallLevel = Settlement.CurrentSettlement.Town.GetWallLevel();
			string scene = Settlement.CurrentSettlement.LocationComplex.GetScene("center", wallLevel);
			Location locationWithId = LocationComplex.Current.GetLocationWithId("center");
			CampaignMission.OpenAlleyFightMission(scene, wallLevel, locationWithId, troopRoster, troopsOfAlleyForBattleMission);
			this._battleWillStartInCurrentSettlement = false;
			this._waitForBattleResults = true;
		}

		// Token: 0x06000639 RID: 1593 RVA: 0x0002FA34 File Offset: 0x0002DC34
		protected void AddDialogs(CampaignGameStarter campaignGameStarter)
		{
			campaignGameStarter.AddDialogLine("alley_talk_start_player_owned_thug", "start", "alley_player_owned_start_thug", "{=!}{FURTHER_DETAIL}", new ConversationSentence.OnConditionDelegate(this.alley_talk_player_owned_thug_on_condition), null, 120, null);
			campaignGameStarter.AddPlayerLine("alley_talk_start_player_owned_thug_answer", "alley_player_owned_start_thug", "close_window", "{=GvpvZEba}Very well, take care.", null, null, 120, null, null);
			campaignGameStarter.AddDialogLine("alley_talk_start_player_owned_alley_manager_not_under_attack", "start", "alley_player_owned_start", "{=cwqR0pp1}Greetings my {?PLAYER.GENDER}lady{?}lord{\\?}. It's good to see you here. How can I help you?", new ConversationSentence.OnConditionDelegate(this.alley_talk_player_owned_alley_managed_not_under_attack_on_condition), null, 120, null);
			campaignGameStarter.AddDialogLine("alley_talk_start_player_owned_alley_manager_under_attack", "start", "close_window", "{=jaFWM6sN}Good to have you here, my {?PLAYER.GENDER}lady{?}lord{\\?}. We shall raze them down now.", new ConversationSentence.OnConditionDelegate(this.alley_talk_player_owned_alley_managed_common_condition), null, 120, null);
			campaignGameStarter.AddPlayerLine("alley_talk_start_player_owned_alley_manager_answer_1", "alley_player_owned_start", "alley_manager_general_answer", "{=xJJeXW6j}Let me inspect your troops.", null, new ConversationSentence.OnConsequenceDelegate(this.manage_troops_of_alley_from_dialog), 100, null, null);
			campaignGameStarter.AddPlayerLine("alley_talk_start_player_owned_alley_manager_answer_2", "alley_player_owned_start", "player_asked_for_volunteers", "{=ah3WKIc8}I could use some more troops in my party. Have you found any volunteers?", null, null, 100, null, null);
			campaignGameStarter.AddPlayerLine("alley_talk_start_player_owned_alley_manager_answer_3", "alley_player_owned_start", "alley_manager_general_answer", "{=ut26sd6p}I want somebody else to take charge of this place.", null, new ConversationSentence.OnConsequenceDelegate(this.change_leader_of_alley_from_dialog), 100, null, null);
			campaignGameStarter.AddPlayerLine("alley_talk_start_player_owned_alley_manager_answer_4", "alley_player_owned_start", "abandon_alley_are_you_sure", "{=I8o7oarw}I want to abandon this area.", null, null, 100, null, null);
			campaignGameStarter.AddDialogLine("alley_talk_start_player_owned_alley_manager_answer_4_1", "abandon_alley_are_you_sure", "abandon_alley_are_you_sure_player", "{=6dDXb4iI}Are you sure? If you are, we can pack up and join you.", null, null, 100, null);
			campaignGameStarter.AddPlayerLine("alley_talk_start_player_owned_alley_manager_answer_4_2", "abandon_alley_are_you_sure_player", "start", "{=ALWqXMiP}Yes, I am sure.", null, new ConversationSentence.OnConsequenceDelegate(this.abandon_alley_from_dialog_consequence), 100, null, null);
			campaignGameStarter.AddPlayerLine("alley_talk_start_player_owned_alley_manager_answer_4_3", "abandon_alley_are_you_sure_player", "start", "{=YJkiQ6nM}No, I have changed my mind. We will stay here.", null, null, 100, null, null);
			campaignGameStarter.AddPlayerLine("alley_talk_start_player_owned_alley_manager_answer_5", "alley_player_owned_start", "close_window", "{=D33fIGQe}Never mind.", null, null, 100, null, null);
			campaignGameStarter.AddDialogLine("alley_talk_start_player_owned_alley_manager_volunteers_1", "player_asked_for_volunteers", "alley_player_owned_start", "{=nRVrXSbv}Not yet my {?PLAYER.GENDER}lady{?}lord{\\?}, but I am working on it. Better check back next week.", new ConversationSentence.OnConditionDelegate(this.alley_has_no_troops_to_recruit), null, 100, null);
			campaignGameStarter.AddDialogLine("alley_talk_start_player_owned_alley_manager_volunteers_2", "player_asked_for_volunteers", "alley_player_ask_for_troops", "{=aLrK7Si7}Yes. I have {TROOPS_TO_RECRUIT} ready to join you.", new ConversationSentence.OnConditionDelegate(this.get_troops_to_recruit_from_alley), null, 100, null);
			campaignGameStarter.AddPlayerLine("alley_talk_start_player_owned_alley_manager_volunteers_3", "alley_player_ask_for_troops", "give_troops_to_player", "{=BNz4ZA6S}Very well. Have them join me now.", null, new ConversationSentence.OnConsequenceDelegate(this.player_recruited_troops_from_alley), 100, null, null);
			campaignGameStarter.AddDialogLine("alley_talk_start_player_owned_alley_manager_volunteers_4", "give_troops_to_player", "start", "{=PlIYRSIz}All right my {?PLAYER.GENDER}lady{?}lord{\\?}, they will be ready.", null, null, 100, null);
			campaignGameStarter.AddPlayerLine("alley_talk_start_player_owned_alley_manager_volunteers_5", "alley_player_ask_for_troops", "start", "{=n1qrbQVa}I don't need them right now.", null, null, 100, null, null);
			campaignGameStarter.AddDialogLine("alley_talk_start_player_owned_alley_manager_answer_1", "alley_manager_general_answer", "start", "{=lF5HkBDy}As you wish.", null, null, 100, null);
			campaignGameStarter.AddDialogLine("alley_talk_start_normal", "start", "alley_talk_start", "{=qT4nbaAY}Oi, you, what are you doing here?", new ConversationSentence.OnConditionDelegate(this.alley_talk_start_normal_on_condition), null, 120, null);
			campaignGameStarter.AddDialogLine("alley_talk_start_normal", "start", "alley_talk_start_confront", "{=MzHbdTYe}Well well well, I wasn't expecting to see you there. There must be some little birds informing you about my plans. That won't change anything, though. I'll still crush you.", new ConversationSentence.OnConditionDelegate(this.alley_confront_dialog_on_condition), null, 100, null);
			campaignGameStarter.AddPlayerLine("alley_talk_start_normal", "alley_talk_start_confront", "close_window", "{=GMsZZQzI}Bring it on.", null, new ConversationSentence.OnConsequenceDelegate(this.start_alley_fight_after_conversation), 100, null, null);
			campaignGameStarter.AddPlayerLine("alley_talk_start_normal", "alley_talk_start_confront", "close_window", "{=QNpuyzc4}Take it easy. I have no interest in the place any more. Take it.", null, new ConversationSentence.OnConsequenceDelegate(this.abandon_alley_from_dialog_consequence), 100, new ConversationSentence.OnClickableConditionDelegate(this.alley_abandon_while_under_attack_clickable_condition), null);
			campaignGameStarter.AddPlayerLine("alley_start_1", "alley_talk_start", "alley_activity", "{=1NSRPYZt}Just passing through. What goes on here?", null, null, 100, null, null);
			campaignGameStarter.AddPlayerLine("alley_start_2", "alley_talk_start", "first_entry_to_alley_2", "{=HCmQmZbe}I'm just having a look. Do you mind?", null, null, 100, null, null);
			campaignGameStarter.AddPlayerLine("alley_start_3", "alley_talk_start", "close_window", "{=iW9iKbb8}Nothing.", null, null, 100, null, null);
			campaignGameStarter.AddPlayerLine("alley_entry_start_1", "alley_first_talk_start", "first_entry_to_alley_2", "{=X18yfvX7}Just passing through.", null, null, 100, null, null);
			campaignGameStarter.AddPlayerLine("alley_entry_start_2", "alley_first_talk_start", "first_entry_to_alley_2", "{=Y1O5bPpJ}Having a look. Do you mind?", null, null, 100, null, null);
			campaignGameStarter.AddPlayerLine("alley_entry_start_3", "alley_first_talk_start", "first_entry_to_alley_2", "{=eQfL2UmE}None of your business.", null, null, 100, null, null);
			campaignGameStarter.AddDialogLine("first_entry_to_alley", "first_entry_to_alley_2", "alley_options", "{=Ll2wN2Gm}This is how it goes, friend. This is our turf. We answer to {ALLEY_BOSS.NAME}, and {?ALLEY_BOSS.GENDER}she's{?}he's{\\?} like the {?ALLEY_BOSS.GENDER}queen{?}king{\\?} here. So if you haven't got a good reason to be here, clear out.", new ConversationSentence.OnConditionDelegate(this.enter_alley_rude_on_occasion), null, 100, null);
			campaignGameStarter.AddDialogLine("first_entry_to_alley_friendly", "first_entry_to_alley_2", "alley_options", "{=Fo47BuSY}Fine, you know {ALLEY_BOSS.NAME}, you can be here. Just no trouble, you understand?", null, null, 100, null);
			campaignGameStarter.AddPlayerLine("ally_entry_start_fight", "alley_options", "alley_fight_start", "{=2Fxva3RY}I don't take orders from the likes of you.", null, new ConversationSentence.OnConsequenceDelegate(this.start_alley_fight_on_consequence), 100, null, null);
			campaignGameStarter.AddPlayerLine("alley_entry_question_activity", "alley_options", "alley_activity", "{=aNZKqAAS}So what goes on here?", null, null, 100, null, null);
			campaignGameStarter.AddPlayerLine("alley_entry_end_conversation", "alley_options", "close_window", "{=Mk3Qfb4D}I don't want any trouble. Later.", null, null, 100, null, null);
			campaignGameStarter.AddDialogLine("alley_fight_start", "alley_fight_start", "close_window", "{=EN3Zqyx5}A mouthy one, eh? At him, lads![ib:aggressive][if:idle_angry][ib:normal]", null, null, 100, null);
			campaignGameStarter.AddDialogLine("alley_activity", "alley_activity", "alley_activity_2", "{=bZ5rXBY5}{ALLEY_ACTIVITY_STRING}", new ConversationSentence.OnConditionDelegate(this.alley_activity_on_condition), null, 100, null);
			campaignGameStarter.AddDialogLine("alley_activity_2", "alley_activity_2", "alley_options_player", "{=eZq11NVD}And by the way, we take orders from {ALLEY_BOSS.NAME}, and no one else.", new ConversationSentence.OnConditionDelegate(this.alley_activity_2_on_condition), null, 100, null);
			campaignGameStarter.AddDialogLine("alley_activity_back", "alley_options_decline", "alley_options", "{=pf4EIcBQ}Anything else? Because unless you've got business here, maybe you'd best move along.[ib:closed][ib:normal]", null, null, 100, null);
			campaignGameStarter.AddDialogLine("alley_activity_end", "alley_options_player", "close_window", "{=xb1Ps6ZC}Now get lost...", null, null, 100, null);
			campaignGameStarter.AddDialogLine("alley_meet_boss", "alley_meet_boss", "close_window", "{=NoxFbtEa}Wait here. We'll see if {?ALLEY_BOSS.GENDER}she{?}he{\\?} wants to talk to you. (NOT IMPLEMENTED)", null, null, 100, null);
			campaignGameStarter.AddDialogLine("gang_leader_bodyguard_start", "start", "close_window", "{=NVvfxdIc}You best talk to the boss.", new ConversationSentence.OnConditionDelegate(this.gang_leader_bodyguard_on_condition), null, 200, null);
		}

		// Token: 0x0600063A RID: 1594 RVA: 0x0003002A File Offset: 0x0002E22A
		private bool alley_abandon_while_under_attack_clickable_condition(out TextObject explanation)
		{
			explanation = new TextObject("{=3E1XVyGM}You will lose the ownership of the alley.", null);
			return true;
		}

		// Token: 0x0600063B RID: 1595 RVA: 0x0003003C File Offset: 0x0002E23C
		private bool alley_confront_dialog_on_condition()
		{
			AlleyCampaignBehavior.PlayerAlleyData playerAlleyData = this._playerOwnedCommonAreaData.FirstOrDefault((AlleyCampaignBehavior.PlayerAlleyData x) => x.Alley.Settlement == Settlement.CurrentSettlement);
			return playerAlleyData != null && playerAlleyData.IsUnderAttack && playerAlleyData.UnderAttackBy.Owner == Hero.OneToOneConversationHero;
		}

		// Token: 0x0600063C RID: 1596 RVA: 0x00030093 File Offset: 0x0002E293
		private void start_alley_fight_after_conversation()
		{
			this._battleWillStartInCurrentSettlement = true;
			Campaign.Current.GameMenuManager.SetNextMenu("manage_alley");
			if (Mission.Current != null)
			{
				Mission.Current.EndMission();
			}
		}

		// Token: 0x0600063D RID: 1597 RVA: 0x000300C4 File Offset: 0x0002E2C4
		private void player_recruited_troops_from_alley()
		{
			AlleyCampaignBehavior.PlayerAlleyData playerAlleyData = this._playerOwnedCommonAreaData.First((AlleyCampaignBehavior.PlayerAlleyData x) => x.Alley.Settlement == Settlement.CurrentSettlement);
			foreach (TroopRosterElement troopRosterElement in Campaign.Current.Models.AlleyModel.GetTroopsToRecruitFromAlleyDependingOnAlleyRandom(playerAlleyData.Alley, playerAlleyData.RandomFloatWeekly).GetTroopRoster())
			{
				MobileParty.MainParty.MemberRoster.AddToCounts(troopRosterElement.Character, troopRosterElement.Number, false, 0, 0, true, -1);
			}
			MBInformationManager.AddQuickInformation(new TextObject("{=8CW2y0p3}Troops have been joined to your party", null), 0, null, "");
			playerAlleyData.LastRecruitTime = CampaignTime.Now;
		}

		// Token: 0x0600063E RID: 1598 RVA: 0x000301A0 File Offset: 0x0002E3A0
		private bool get_troops_to_recruit_from_alley()
		{
			AlleyCampaignBehavior.PlayerAlleyData playerAlleyData = this._playerOwnedCommonAreaData.First((AlleyCampaignBehavior.PlayerAlleyData x) => x.Alley.Settlement == Settlement.CurrentSettlement);
			TroopRoster troopsToRecruitFromAlleyDependingOnAlleyRandom = Campaign.Current.Models.AlleyModel.GetTroopsToRecruitFromAlleyDependingOnAlleyRandom(playerAlleyData.Alley, playerAlleyData.RandomFloatWeekly);
			List<TextObject> list = new List<TextObject>();
			foreach (TroopRosterElement troopRosterElement in troopsToRecruitFromAlleyDependingOnAlleyRandom.GetTroopRoster())
			{
				TextObject textObject = new TextObject("{=!}{TROOP_COUNT} {?TROOP_COUNT > 1}{TROOP_NAME}{.s}{?}{TROOP_NAME}{\\?}", null);
				textObject.SetTextVariable("TROOP_COUNT", troopRosterElement.Number);
				textObject.SetTextVariable("TROOP_NAME", troopRosterElement.Character.Name);
				list.Add(textObject);
			}
			TextObject textObject2 = GameTexts.GameTextHelper.MergeTextObjectsWithComma(list, true);
			MBTextManager.SetTextVariable("TROOPS_TO_RECRUIT", textObject2, false);
			return true;
		}

		// Token: 0x0600063F RID: 1599 RVA: 0x00030294 File Offset: 0x0002E494
		private bool alley_has_no_troops_to_recruit()
		{
			return this._playerOwnedCommonAreaData.First((AlleyCampaignBehavior.PlayerAlleyData x) => x.Alley.Settlement == Settlement.CurrentSettlement).RandomFloatWeekly > 0.5f;
		}

		// Token: 0x06000640 RID: 1600 RVA: 0x000302CC File Offset: 0x0002E4CC
		private void change_leader_of_alley_from_dialog()
		{
			AlleyHelper.CreateMultiSelectionInquiryForSelectingClanMemberToAlley(this._playerOwnedCommonAreaData.First((AlleyCampaignBehavior.PlayerAlleyData x) => x.Alley.Settlement == Settlement.CurrentSettlement).Alley, new Action<List<InquiryElement>>(this.ChangeAssignedClanMemberOfAlley), null);
		}

		// Token: 0x06000641 RID: 1601 RVA: 0x0003031C File Offset: 0x0002E51C
		private void manage_troops_of_alley_from_dialog()
		{
			AlleyHelper.OpenScreenForManagingAlley(this._playerOwnedCommonAreaData.First((AlleyCampaignBehavior.PlayerAlleyData x) => x.Alley.Settlement == Settlement.CurrentSettlement).TroopRoster, new PartyPresentationDoneButtonDelegate(this.OnPartyScreenClosed), new TextObject("{=dQBArrqh}Manage Alley", null), null);
		}

		// Token: 0x06000642 RID: 1602 RVA: 0x00030378 File Offset: 0x0002E578
		private void abandon_alley_from_dialog_consequence()
		{
			AlleyCampaignBehavior.PlayerAlleyData playerAlleyData = this._playerOwnedCommonAreaData.First((AlleyCampaignBehavior.PlayerAlleyData x) => x.Alley.Settlement == Settlement.CurrentSettlement);
			if (Mission.Current != null)
			{
				for (int i = Mission.Current.Agents.Count - 1; i >= 0; i--)
				{
					Agent agent = Mission.Current.Agents[i];
					if (agent.IsHuman)
					{
						CampaignAgentComponent component = agent.GetComponent<CampaignAgentComponent>();
						Hero hero;
						if (component == null)
						{
							hero = null;
						}
						else
						{
							AgentNavigator agentNavigator = component.AgentNavigator;
							if (agentNavigator == null)
							{
								hero = null;
							}
							else
							{
								Alley memberOfAlley = agentNavigator.MemberOfAlley;
								hero = ((memberOfAlley != null) ? memberOfAlley.Owner : null);
							}
						}
						if (hero == Hero.MainHero && Hero.OneToOneConversationHero.CharacterObject != agent.Character)
						{
							agent.FadeOut(false, true);
						}
					}
				}
			}
			this._playerOwnedCommonAreaData.Remove(playerAlleyData);
			playerAlleyData.AbandonTheAlley(false);
			if (Mission.Current != null && playerAlleyData.Alley.Owner != null)
			{
				foreach (TroopRosterElement troopRosterElement in Campaign.Current.Models.AlleyModel.GetTroopsOfAIOwnedAlley(playerAlleyData.Alley).GetTroopRoster())
				{
					for (int j = 0; j < troopRosterElement.Number; j++)
					{
						this.AddCharacterToAlley(troopRosterElement.Character, playerAlleyData.Alley);
					}
				}
			}
			this._playerAbandonedAlleyFromDialogRecently = true;
		}

		// Token: 0x06000643 RID: 1603 RVA: 0x000304EC File Offset: 0x0002E6EC
		private bool alley_talk_player_owned_alley_managed_not_under_attack_on_condition()
		{
			AlleyCampaignBehavior.PlayerAlleyData playerAlleyData = this._playerOwnedCommonAreaData.FirstOrDefault((AlleyCampaignBehavior.PlayerAlleyData x) => x.Alley.Settlement == Settlement.CurrentSettlement);
			return playerAlleyData != null && !playerAlleyData.IsUnderAttack && this.alley_talk_player_owned_alley_managed_common_condition();
		}

		// Token: 0x06000644 RID: 1604 RVA: 0x00030538 File Offset: 0x0002E738
		private bool alley_talk_player_owned_alley_managed_common_condition()
		{
			AlleyCampaignBehavior.PlayerAlleyData playerAlleyData = this._playerOwnedCommonAreaData.FirstOrDefault((AlleyCampaignBehavior.PlayerAlleyData x) => x.Alley.Settlement == Settlement.CurrentSettlement);
			return playerAlleyData != null && playerAlleyData.AssignedClanMember == Hero.OneToOneConversationHero;
		}

		// Token: 0x06000645 RID: 1605 RVA: 0x00030584 File Offset: 0x0002E784
		private bool alley_talk_player_owned_thug_on_condition()
		{
			if (!CharacterObject.OneToOneConversationCharacter.IsHero)
			{
				AlleyCampaignBehavior.PlayerAlleyData playerAlleyData = this._playerOwnedCommonAreaData.FirstOrDefault((AlleyCampaignBehavior.PlayerAlleyData x) => x.Alley.Settlement == Settlement.CurrentSettlement);
				if (playerAlleyData != null)
				{
					CampaignAgentComponent component = ((Agent)Campaign.Current.ConversationManager.OneToOneConversationAgent).GetComponent<CampaignAgentComponent>();
					if (component != null && component.AgentNavigator.MemberOfAlley == playerAlleyData.Alley)
					{
						if (playerAlleyData.IsAssignedClanMemberDead)
						{
							TextObject textObject = new TextObject("{=SdKTUIVJ}Oi, my {?PLAYER.GENDER}lady{?}lord{\\?}. Sorry for your loss, {DEAD_ALLEY_LEADER.NAME} will be missed in these streets. We are waiting for you to appoint a new boss, whenever you’re ready.", null);
							StringHelpers.SetCharacterProperties("DEAD_ALLEY_LEADER", playerAlleyData.AssignedClanMember.CharacterObject, null, false);
							MBTextManager.SetTextVariable("FURTHER_DETAIL", textObject, false);
						}
						else if (playerAlleyData.AssignedClanMember.IsTraveling)
						{
							TextObject textObject2 = new TextObject("{=KKvOQAVa}We are waiting for {TRAVELING_ALLEY_LEADER.NAME} to come. Until {?TRAVELING_ALLEY_LEADER.GENDER}she{?}he{\\?} arrives, we'll be extra watchful.", null);
							StringHelpers.SetCharacterProperties("TRAVELING_ALLEY_LEADER", playerAlleyData.AssignedClanMember.CharacterObject, null, false);
							MBTextManager.SetTextVariable("FURTHER_DETAIL", textObject2, false);
						}
						else
						{
							TextObject textObject3 = new TextObject("{=OPwO5RXC}Welcome, boss. We're honored to have you here. You can be sure we're keeping an eye on everything going on.", null);
							MBTextManager.SetTextVariable("FURTHER_DETAIL", textObject3, false);
						}
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06000646 RID: 1606 RVA: 0x000306A0 File Offset: 0x0002E8A0
		private bool alley_activity_on_condition()
		{
			List<TextObject> list = new List<TextObject>();
			Alley lastVisitedAlley = CampaignMission.Current.LastVisitedAlley;
			if (lastVisitedAlley.Owner.GetTraitLevel(DefaultTraits.Thug) > 0)
			{
				list.Add(new TextObject("{=prJBRboS}we look after the honest folk here. Make sure no one smashes up their shops. And if they want to give us a coin or two as a way of saying thanks, well, who'd mind?", null));
			}
			if (lastVisitedAlley.Owner.GetTraitLevel(DefaultTraits.Smuggler) > 0)
			{
				list.Add(new TextObject("{=CqnAGehj}suppose someone wanted to buy some goods and didn't want to pay the customs tax. We might be able to help that person out.", null));
			}
			if (lastVisitedAlley.Owner.GetTraitLevel(DefaultTraits.Thief) > 0)
			{
				list.Add(new TextObject("{=HZb0PSyD}maybe you've lost something of value. Come ask us, and perhaps we can find it for you. For a fee, naturally.", null));
			}
			if (lastVisitedAlley.Owner.GetTraitLevel(DefaultTraits.Gambler) > 0)
			{
				list.Add(new TextObject("{=N6kmOVH0}some men might fancy a game of dice. Figure Lady Luck wants to give them a kiss. We're here to keep things honest.", null));
			}
			if (lastVisitedAlley.Owner.Gold > 100)
			{
				list.Add(new TextObject("{=U8iyCXmF}we help out those who are down on their luck. Give 'em a purse of silver to tide them by. With a bit of speculative interest, naturally.", null));
			}
			MBTextManager.SetTextVariable("ALLEY_ACTIVITY_STRING", "{=1rCk6xRa}Now then... If you're asking,[ib:normal]", false);
			for (int i = 0; i < list.Count; i++)
			{
				MBTextManager.SetTextVariable("ALLEY_ACTIVITY_ADDITION", list[i].ToString(), false);
				MBTextManager.SetTextVariable("ALLEY_ACTIVITY_STRING", new TextObject("{=jVjIkODa}{ALLEY_ACTIVITY_STRING} {ALLEY_ACTIVITY_ADDITION}", null).ToString(), false);
				if (i + 1 != list.Count)
				{
					MBTextManager.SetTextVariable("ALLEY_ACTIVITY_ADDITION", "{=lbNl0a8t}Also,", false);
					MBTextManager.SetTextVariable("ALLEY_ACTIVITY_STRING", new TextObject("{=jVjIkODa}{ALLEY_ACTIVITY_STRING} {ALLEY_ACTIVITY_ADDITION}", null).ToString(), false);
				}
			}
			return true;
		}

		// Token: 0x06000647 RID: 1607 RVA: 0x000307F6 File Offset: 0x0002E9F6
		private bool alley_activity_2_on_condition()
		{
			StringHelpers.SetCharacterProperties("ALLEY_BOSS", CampaignMission.Current.LastVisitedAlley.Owner.CharacterObject, null, false);
			return true;
		}

		// Token: 0x06000648 RID: 1608 RVA: 0x0003081C File Offset: 0x0002EA1C
		private bool alley_talk_start_normal_on_condition()
		{
			Agent oneToOneConversationAgent = ConversationMission.OneToOneConversationAgent;
			AgentNavigator agentNavigator = ((oneToOneConversationAgent != null) ? oneToOneConversationAgent.GetComponent<CampaignAgentComponent>().AgentNavigator : null);
			if (((agentNavigator != null) ? agentNavigator.MemberOfAlley : null) != null && agentNavigator.MemberOfAlley.State == 1 && agentNavigator.MemberOfAlley.Owner != Hero.MainHero)
			{
				CampaignMission.Current.LastVisitedAlley = agentNavigator.MemberOfAlley;
				return true;
			}
			return false;
		}

		// Token: 0x06000649 RID: 1609 RVA: 0x00030884 File Offset: 0x0002EA84
		private bool enter_alley_rude_on_occasion()
		{
			Agent oneToOneConversationAgent = ConversationMission.OneToOneConversationAgent;
			Hero owner = ((oneToOneConversationAgent != null) ? oneToOneConversationAgent.GetComponent<CampaignAgentComponent>().AgentNavigator.MemberOfAlley : null).Owner;
			float relationWithPlayer = owner.GetRelationWithPlayer();
			StringHelpers.SetCharacterProperties("ALLEY_BOSS", owner.CharacterObject, null, false);
			return !owner.HasMet || relationWithPlayer < -5f;
		}

		// Token: 0x0600064A RID: 1610 RVA: 0x000308E0 File Offset: 0x0002EAE0
		private void start_alley_fight_on_consequence()
		{
			this._playerIsInAlleyFightMission = true;
			Campaign.Current.ConversationManager.ConversationEndOneShot += delegate
			{
				Mission.Current.GetMissionBehavior<MissionAlleyHandler>().StartCommonAreaBattle(CampaignMission.Current.LastVisitedAlley);
			};
			LogEntry.AddLogEntry(new PlayerAttackAlleyLogEntry(CampaignMission.Current.LastVisitedAlley.Owner, Hero.MainHero.CurrentSettlement));
		}

		// Token: 0x0600064B RID: 1611 RVA: 0x00030945 File Offset: 0x0002EB45
		private bool gang_leader_bodyguard_on_condition()
		{
			return Settlement.CurrentSettlement != null && CharacterObject.OneToOneConversationCharacter == Settlement.CurrentSettlement.Culture.GangleaderBodyguard;
		}

		// Token: 0x0600064C RID: 1612 RVA: 0x00030968 File Offset: 0x0002EB68
		private void OnHeroKilled(Hero victim, Hero killer, KillCharacterAction.KillCharacterActionDetail detail, bool showNotification)
		{
			AlleyCampaignBehavior.PlayerAlleyData playerAlleyData = this._playerOwnedCommonAreaData.FirstOrDefault((AlleyCampaignBehavior.PlayerAlleyData x) => x.AssignedClanMember == victim);
			if (playerAlleyData != null)
			{
				playerAlleyData.TroopRoster.RemoveTroop(victim.CharacterObject, 1, default(UniqueTroopDescriptor), 0);
				Campaign.Current.CampaignInformationManager.NewMapNoticeAdded(new AlleyLeaderDiedMapNotification(playerAlleyData.Alley, new TextObject("{=EAPYyktd}One of your alleys has lost its leader or is lacking troops", null)));
			}
		}

		// Token: 0x0600064D RID: 1613 RVA: 0x000309E4 File Offset: 0x0002EBE4
		public bool GetIsAlleyUnderAttack(Alley alley)
		{
			AlleyCampaignBehavior.PlayerAlleyData playerAlleyData = this._playerOwnedCommonAreaData.FirstOrDefault((AlleyCampaignBehavior.PlayerAlleyData x) => x.Alley == alley);
			return playerAlleyData != null && playerAlleyData.IsUnderAttack;
		}

		// Token: 0x0600064E RID: 1614 RVA: 0x00030A24 File Offset: 0x0002EC24
		public int GetPlayerOwnedAlleyTroopCount(Alley alley)
		{
			return this._playerOwnedCommonAreaData.FirstOrDefault((AlleyCampaignBehavior.PlayerAlleyData x) => x.Alley == alley).TroopRoster.TotalRegulars;
		}

		// Token: 0x0600064F RID: 1615 RVA: 0x00030A60 File Offset: 0x0002EC60
		public int GetResponseTimeLeftForAttackInDays(Alley alley)
		{
			return (int)this._playerOwnedCommonAreaData.First((AlleyCampaignBehavior.PlayerAlleyData x) => x.Alley == alley).AttackResponseDueDate.RemainingDaysFromNow;
		}

		// Token: 0x06000650 RID: 1616 RVA: 0x00030A9C File Offset: 0x0002EC9C
		public void AbandonAlleyFromClanMenu(Alley alley)
		{
			AlleyCampaignBehavior.PlayerAlleyData playerAlleyData = this._playerOwnedCommonAreaData.FirstOrDefault((AlleyCampaignBehavior.PlayerAlleyData x) => x.Alley == alley);
			this._playerOwnedCommonAreaData.Remove(playerAlleyData);
			if (playerAlleyData != null)
			{
				playerAlleyData.AbandonTheAlley(true);
			}
		}

		// Token: 0x06000651 RID: 1617 RVA: 0x00030AE8 File Offset: 0x0002ECE8
		public bool IsHeroAlleyLeaderOfAnyPlayerAlley(Hero hero)
		{
			return this._playerOwnedCommonAreaData.Any((AlleyCampaignBehavior.PlayerAlleyData x) => x.AssignedClanMember == hero);
		}

		// Token: 0x06000652 RID: 1618 RVA: 0x00030B19 File Offset: 0x0002ED19
		public List<Hero> GetAllAssignedClanMembersForOwnedAlleys()
		{
			return this._playerOwnedCommonAreaData.Select((AlleyCampaignBehavior.PlayerAlleyData x) => x.AssignedClanMember).ToList<Hero>();
		}

		// Token: 0x06000653 RID: 1619 RVA: 0x00030B4C File Offset: 0x0002ED4C
		public void ChangeAlleyMember(Alley alley, Hero newAlleyLead)
		{
			AlleyCampaignBehavior.PlayerAlleyData playerAlleyData = this._playerOwnedCommonAreaData.FirstOrDefault((AlleyCampaignBehavior.PlayerAlleyData x) => x.Alley == alley);
			this.ChangeTheLeaderOfAlleyInternal(playerAlleyData, newAlleyLead);
		}

		// Token: 0x06000654 RID: 1620 RVA: 0x00030B86 File Offset: 0x0002ED86
		public void OnPlayerRetreatedFromMission()
		{
			this._playerRetreatedFromMission = true;
		}

		// Token: 0x06000655 RID: 1621 RVA: 0x00030B8F File Offset: 0x0002ED8F
		public void OnPlayerDiedInMission()
		{
			this._playerDiedInMission = true;
		}

		// Token: 0x06000656 RID: 1622 RVA: 0x00030B98 File Offset: 0x0002ED98
		public Hero GetAssignedClanMemberOfAlley(Alley alley)
		{
			AlleyCampaignBehavior.PlayerAlleyData playerAlleyData = this._playerOwnedCommonAreaData.FirstOrDefault((AlleyCampaignBehavior.PlayerAlleyData x) => x.Alley == alley);
			if (playerAlleyData != null)
			{
				return playerAlleyData.AssignedClanMember;
			}
			return null;
		}

		// Token: 0x06000657 RID: 1623 RVA: 0x00030BD8 File Offset: 0x0002EDD8
		private void ChangeTheLeaderOfAlleyInternal(AlleyCampaignBehavior.PlayerAlleyData alleyData, Hero newLeader)
		{
			Hero assignedClanMember = alleyData.AssignedClanMember;
			alleyData.AssignedClanMember = newLeader;
			if (!assignedClanMember.IsDead)
			{
				alleyData.TroopRoster.RemoveTroop(assignedClanMember.CharacterObject, 1, default(UniqueTroopDescriptor), 0);
			}
			alleyData.TroopRoster.AddToCounts(newLeader.CharacterObject, 1, true, 0, 0, true, -1);
			TeleportHeroAction.ApplyDelayedTeleportToSettlement(newLeader, alleyData.Alley.Settlement);
			if (Campaign.Current.CurrentMenuContext != null)
			{
				Campaign.Current.CurrentMenuContext.Refresh();
			}
		}

		// Token: 0x06000658 RID: 1624 RVA: 0x00030C5B File Offset: 0x0002EE5B
		[GameMenuInitializationHandler("manage_alley")]
		[GameMenuInitializationHandler("alley_fight_lost")]
		[GameMenuInitializationHandler("alley_fight_won")]
		[GameMenuInitializationHandler("manage_alley_abandon_are_you_sure")]
		public static void alley_related_menu_on_init(MenuCallbackArgs args)
		{
			args.MenuContext.SetBackgroundMeshName(Settlement.CurrentSettlement.SettlementComponent.WaitMeshName);
		}

		// Token: 0x06000659 RID: 1625 RVA: 0x00030C78 File Offset: 0x0002EE78
		[CommandLineFunctionality.CommandLineArgumentFunction("spawn_new_alley_attack", "campaign")]
		public static string SpawnNewAlleyAttack(List<string> strings)
		{
			foreach (AlleyCampaignBehavior.PlayerAlleyData playerAlleyData in Campaign.Current.GetCampaignBehavior<AlleyCampaignBehavior>()._playerOwnedCommonAreaData)
			{
				if (!playerAlleyData.IsUnderAttack)
				{
					if (playerAlleyData.Alley.Settlement.Alleys.Any((Alley x) => x.State == 1))
					{
						Campaign.Current.GetCampaignBehavior<AlleyCampaignBehavior>().StartNewAlleyAttack(playerAlleyData);
						return "OK.";
					}
				}
			}
			return "There is no suitable alley for spawning an alley attack.";
		}

		// Token: 0x0600065A RID: 1626 RVA: 0x00030D2C File Offset: 0x0002EF2C
		[CommandLineFunctionality.CommandLineArgumentFunction("make_random_gang_leader_occupy_alley_in_settlement", "campaign")]
		public static string OccupyRandomAlley(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (CampaignCheats.CheckHelp(strings))
			{
				return "Format is \"campaign.make_random_gang_leader_occupy_alley_in_settlement [TownName]\".";
			}
			Settlement settlement = null;
			if (strings.Count > 0)
			{
				settlement = Campaign.Current.Settlements.FirstOrDefault((Settlement x) => x.Name.ToString().ToLower() == strings[0].ToString());
			}
			if (settlement == null)
			{
				return "settlement not found";
			}
			Hero randomElementInefficiently = Extensions.GetRandomElementInefficiently<Hero>(settlement.Notables.Where((Hero x) => x.IsGangLeader));
			if (randomElementInefficiently == null)
			{
				return "there is no gang leader in the settlement";
			}
			IEnumerable<Alley> enumerable = settlement.Alleys.Where((Alley x) => x.State == 0);
			if (enumerable.Any<Alley>())
			{
				Extensions.GetRandomElementInefficiently<Alley>(enumerable).SetOwner(randomElementInefficiently);
				return "ok";
			}
			return "there is no empty alley int the settlement.";
		}

		// Token: 0x040002D4 RID: 724
		private const int DesiredOccupiedAlleyPerTownFrequency = 2;

		// Token: 0x040002D5 RID: 725
		private const int RelationLossWithSettlementOwnerAfterOccupyingAnAlley = -2;

		// Token: 0x040002D6 RID: 726
		private const int RelationLossWithOldOwnerUponClearingAlley = -5;

		// Token: 0x040002D7 RID: 727
		private const int RelationGainWithOtherNotablesUponClearingAlley = 1;

		// Token: 0x040002D8 RID: 728
		private const float SpawningNewAlleyFightDailyPercentage = 0.015f;

		// Token: 0x040002D9 RID: 729
		private const float ConvertTroopsToThugsDailyPercentage = 0.01f;

		// Token: 0x040002DA RID: 730
		private const float GainOrLoseAlleyDailyBasePercentage = 0.02f;

		// Token: 0x040002DB RID: 731
		private CharacterObject _thug;

		// Token: 0x040002DC RID: 732
		private CharacterObject _expertThug;

		// Token: 0x040002DD RID: 733
		private CharacterObject _masterThug;

		// Token: 0x040002DE RID: 734
		private List<AlleyCampaignBehavior.PlayerAlleyData> _playerOwnedCommonAreaData = new List<AlleyCampaignBehavior.PlayerAlleyData>();

		// Token: 0x040002DF RID: 735
		private bool _battleWillStartInCurrentSettlement;

		// Token: 0x040002E0 RID: 736
		private bool _waitForBattleResults;

		// Token: 0x040002E1 RID: 737
		private bool _playerRetreatedFromMission;

		// Token: 0x040002E2 RID: 738
		private bool _playerDiedInMission;

		// Token: 0x040002E3 RID: 739
		private bool _playerIsInAlleyFightMission;

		// Token: 0x040002E4 RID: 740
		private bool _playerAbandonedAlleyFromDialogRecently;

		// Token: 0x0200016C RID: 364
		public class AlleyCampaignBehaviorTypeDefiner : CampaignBehaviorBase.SaveableCampaignBehaviorTypeDefiner
		{
			// Token: 0x06001140 RID: 4416 RVA: 0x0007309C File Offset: 0x0007129C
			public AlleyCampaignBehaviorTypeDefiner()
				: base(515253)
			{
			}

			// Token: 0x06001141 RID: 4417 RVA: 0x000730A9 File Offset: 0x000712A9
			protected override void DefineClassTypes()
			{
				base.AddClassDefinition(typeof(AlleyCampaignBehavior.PlayerAlleyData), 1, null);
			}

			// Token: 0x06001142 RID: 4418 RVA: 0x000730BD File Offset: 0x000712BD
			protected override void DefineContainerDefinitions()
			{
				base.ConstructContainerDefinition(typeof(List<AlleyCampaignBehavior.PlayerAlleyData>));
			}
		}

		// Token: 0x0200016D RID: 365
		internal class PlayerAlleyData
		{
			// Token: 0x170001EE RID: 494
			// (get) Token: 0x06001143 RID: 4419 RVA: 0x000730D0 File Offset: 0x000712D0
			internal float RandomFloatWeekly
			{
				get
				{
					if (this.LastRecruitTime.ElapsedDaysUntilNow <= 7f)
					{
						return 2f;
					}
					return MBRandom.RandomFloatWithSeed((uint)CampaignTime.Now.ToWeeks, (uint)this.Alley.Tag.GetHashCode());
				}
			}

			// Token: 0x170001EF RID: 495
			// (get) Token: 0x06001144 RID: 4420 RVA: 0x00073118 File Offset: 0x00071318
			internal bool IsUnderAttack
			{
				get
				{
					return this.UnderAttackBy != null;
				}
			}

			// Token: 0x170001F0 RID: 496
			// (get) Token: 0x06001145 RID: 4421 RVA: 0x00073123 File Offset: 0x00071323
			internal bool IsAssignedClanMemberDead
			{
				get
				{
					return this.AssignedClanMember.IsDead;
				}
			}

			// Token: 0x06001146 RID: 4422 RVA: 0x00073130 File Offset: 0x00071330
			internal PlayerAlleyData(Alley alley, TroopRoster roster)
			{
				this.Alley = alley;
				this.TroopRoster = roster;
				this.AssignedClanMember = roster.GetTroopRoster().First<TroopRosterElement>().Character.HeroObject;
				this.UnderAttackBy = null;
			}

			// Token: 0x06001147 RID: 4423 RVA: 0x00073168 File Offset: 0x00071368
			internal void AlleyFightWon()
			{
				this.UnderAttackBy.Owner.AddPower(-(this.UnderAttackBy.Owner.Power * 0.2f));
				this.UnderAttackBy.SetOwner(null);
				this.UnderAttackBy = null;
				if (!this.TroopRoster.Contains(this.AssignedClanMember.CharacterObject))
				{
					this.TroopRoster.AddToCounts(this.AssignedClanMember.CharacterObject, 1, true, 0, 0, true, -1);
				}
				Hero.MainHero.AddSkillXp(DefaultSkills.Roguery, Campaign.Current.Models.AlleyModel.GetXpGainAfterSuccessfulAlleyDefenseForMainHero());
				GameMenu.SwitchToMenu("alley_fight_won");
			}

			// Token: 0x06001148 RID: 4424 RVA: 0x00073211 File Offset: 0x00071411
			internal void AlleyFightLost()
			{
				this.DestroyAlley(false);
				Hero.MainHero.HitPoints = 1;
				GameMenu.SwitchToMenu("alley_fight_lost");
			}

			// Token: 0x06001149 RID: 4425 RVA: 0x00073230 File Offset: 0x00071430
			internal void AbandonTheAlley(bool fromClanScreen = false)
			{
				if (!fromClanScreen)
				{
					foreach (TroopRosterElement troopRosterElement in this.TroopRoster.GetTroopRoster())
					{
						if (!troopRosterElement.Character.IsHero)
						{
							MobileParty.MainParty.MemberRoster.AddToCounts(troopRosterElement.Character, troopRosterElement.Number, false, 0, 0, true, -1);
						}
					}
				}
				this.DestroyAlley(true);
			}

			// Token: 0x0600114A RID: 4426 RVA: 0x000732BC File Offset: 0x000714BC
			internal void DestroyAlley(bool fromAbandoning = false)
			{
				if (!fromAbandoning && this.AssignedClanMember.IsAlive)
				{
					MakeHeroFugitiveAction.Apply(this.AssignedClanMember);
				}
				if (this.UnderAttackBy != null)
				{
					this.Alley.SetOwner(this.UnderAttackBy.Owner);
				}
				else
				{
					this.Alley.SetOwner(null);
				}
				this.TroopRoster.Clear();
				this.UnderAttackBy = null;
			}

			// Token: 0x0600114B RID: 4427 RVA: 0x00073322 File Offset: 0x00071522
			internal static void AutoGeneratedStaticCollectObjectsPlayerAlleyData(object o, List<object> collectedObjects)
			{
				((AlleyCampaignBehavior.PlayerAlleyData)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			// Token: 0x0600114C RID: 4428 RVA: 0x00073330 File Offset: 0x00071530
			protected virtual void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
			{
				collectedObjects.Add(this.Alley);
				collectedObjects.Add(this.AssignedClanMember);
				collectedObjects.Add(this.UnderAttackBy);
				collectedObjects.Add(this.TroopRoster);
				CampaignTime.AutoGeneratedStaticCollectObjectsCampaignTime(this.LastRecruitTime, collectedObjects);
				CampaignTime.AutoGeneratedStaticCollectObjectsCampaignTime(this.AttackResponseDueDate, collectedObjects);
			}

			// Token: 0x0600114D RID: 4429 RVA: 0x0007338F File Offset: 0x0007158F
			internal static object AutoGeneratedGetMemberValueAlley(object o)
			{
				return ((AlleyCampaignBehavior.PlayerAlleyData)o).Alley;
			}

			// Token: 0x0600114E RID: 4430 RVA: 0x0007339C File Offset: 0x0007159C
			internal static object AutoGeneratedGetMemberValueAssignedClanMember(object o)
			{
				return ((AlleyCampaignBehavior.PlayerAlleyData)o).AssignedClanMember;
			}

			// Token: 0x0600114F RID: 4431 RVA: 0x000733A9 File Offset: 0x000715A9
			internal static object AutoGeneratedGetMemberValueUnderAttackBy(object o)
			{
				return ((AlleyCampaignBehavior.PlayerAlleyData)o).UnderAttackBy;
			}

			// Token: 0x06001150 RID: 4432 RVA: 0x000733B6 File Offset: 0x000715B6
			internal static object AutoGeneratedGetMemberValueTroopRoster(object o)
			{
				return ((AlleyCampaignBehavior.PlayerAlleyData)o).TroopRoster;
			}

			// Token: 0x06001151 RID: 4433 RVA: 0x000733C3 File Offset: 0x000715C3
			internal static object AutoGeneratedGetMemberValueLastRecruitTime(object o)
			{
				return ((AlleyCampaignBehavior.PlayerAlleyData)o).LastRecruitTime;
			}

			// Token: 0x06001152 RID: 4434 RVA: 0x000733D5 File Offset: 0x000715D5
			internal static object AutoGeneratedGetMemberValueAttackResponseDueDate(object o)
			{
				return ((AlleyCampaignBehavior.PlayerAlleyData)o).AttackResponseDueDate;
			}

			// Token: 0x040006CC RID: 1740
			[SaveableField(1)]
			internal readonly Alley Alley;

			// Token: 0x040006CD RID: 1741
			[SaveableField(2)]
			internal Hero AssignedClanMember;

			// Token: 0x040006CE RID: 1742
			[SaveableField(3)]
			internal Alley UnderAttackBy;

			// Token: 0x040006CF RID: 1743
			[SaveableField(4)]
			internal TroopRoster TroopRoster;

			// Token: 0x040006D0 RID: 1744
			[SaveableField(5)]
			internal CampaignTime LastRecruitTime;

			// Token: 0x040006D1 RID: 1745
			[SaveableField(6)]
			internal CampaignTime AttackResponseDueDate;
		}
	}
}
