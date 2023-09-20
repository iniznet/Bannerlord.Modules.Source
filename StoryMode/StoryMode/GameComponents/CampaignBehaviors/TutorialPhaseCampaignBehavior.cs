using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using StoryMode.Quests.TutorialPhase;
using StoryMode.StoryModeObjects;
using StoryMode.StoryModePhases;
using TaleWorlds.ActivitySystem;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.AgentOrigins;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Inventory;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.SceneInformationPopupTypes;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace StoryMode.GameComponents.CampaignBehaviors
{
	public class TutorialPhaseCampaignBehavior : CampaignBehaviorBase
	{
		public override void RegisterEvents()
		{
			CampaignEvents.OnNewGameCreatedPartialFollowUpEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter, int>(this.OnNewGameCreatedPartialFollowUp));
			CampaignEvents.TickEvent.AddNonSerializedListener(this, new Action<float>(this.Tick));
			CampaignEvents.OnQuestCompletedEvent.AddNonSerializedListener(this, new Action<QuestBase, QuestBase.QuestCompleteDetails>(this.OnQuestCompleted));
			CampaignEvents.OnSettlementLeftEvent.AddNonSerializedListener(this, new Action<MobileParty, Settlement>(this.OnSettlementLeft));
			CampaignEvents.OnGameLoadedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnGameLoaded));
			CampaignEvents.SettlementEntered.AddNonSerializedListener(this, new Action<MobileParty, Settlement, Hero>(this.OnSettlementEntered));
			CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, new Action(this.DailyTick));
			CampaignEvents.OnGameLoadFinishedEvent.AddNonSerializedListener(this, new Action(this.OnGameLoadFinished));
			CampaignEvents.OnCharacterCreationIsOverEvent.AddNonSerializedListener(this, new Action(this.OnCharacterCreationIsOver));
			CampaignEvents.CanHaveQuestsOrIssuesEvent.AddNonSerializedListener(this, new ReferenceAction<Hero, bool>(this.OnCanHaveQuestsOrIssuesInfoIsRequested));
			CampaignEvents.CanHeroMarryEvent.AddNonSerializedListener(this, new ReferenceAction<Hero, bool>(this.CanHeroMarry));
			StoryModeEvents.OnStoryModeTutorialEndedEvent.AddNonSerializedListener(this, new Action(this.OnStoryModeTutorialEnded));
		}

		private void OnCanHaveQuestsOrIssuesInfoIsRequested(Hero hero, ref bool result)
		{
			Settlement settlement = Settlement.Find("village_ES3_2");
			if (!StoryModeManager.Current.MainStoryLine.TutorialPhase.IsCompleted && settlement.Notables.Contains(hero))
			{
				result = false;
			}
		}

		private void CanHeroMarry(Hero hero, ref bool result)
		{
			if (!TutorialPhase.Instance.IsCompleted && hero.Clan == Clan.PlayerClan)
			{
				result = false;
			}
		}

		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<Equipment[]>("_mainHeroEquipmentBackup", ref this._mainHeroEquipmentBackup);
			dataStore.SyncData<Equipment[]>("_brotherEquipmentBackup", ref this._brotherEquipmentBackup);
		}

		private void Tick(float dt)
		{
			if (TutorialPhase.Instance.TutorialFocusSettlement == null && TutorialPhase.Instance.TutorialFocusMobileParty == null)
			{
				return;
			}
			float num = -1f;
			Vec2 vec = Vec2.Invalid;
			if (TutorialPhase.Instance.TutorialFocusSettlement != null)
			{
				num = Campaign.Current.Models.MapDistanceModel.GetDistance(MobileParty.MainParty, TutorialPhase.Instance.TutorialFocusSettlement);
				vec = TutorialPhase.Instance.TutorialFocusSettlement.GatePosition;
			}
			else if (TutorialPhase.Instance.TutorialFocusMobileParty != null)
			{
				num = Campaign.Current.Models.MapDistanceModel.GetDistance(MobileParty.MainParty, TutorialPhase.Instance.TutorialFocusMobileParty);
				vec = TutorialPhase.Instance.TutorialFocusMobileParty.Position2D;
			}
			if (num > MobileParty.MainParty.SeeingRange * 5f)
			{
				this._controlledByBrother = true;
				MobileParty.MainParty.Ai.SetMoveGoToPoint(vec);
			}
			if (this._controlledByBrother && !this._notifyPlayerAboutPosition)
			{
				this._notifyPlayerAboutPosition = true;
				MBInformationManager.AddQuickInformation(new TextObject("{=hadftxlO}We have strayed too far from our path. I'll take the lead for some time. You follow me.", null), 0, StoryModeHeroes.ElderBrother.CharacterObject, "");
				Campaign.Current.TimeControlMode = 3;
			}
			if (this._controlledByBrother && num < MobileParty.MainParty.SeeingRange)
			{
				this._controlledByBrother = false;
				this._notifyPlayerAboutPosition = false;
				MobileParty.MainParty.Ai.SetMoveModeHold();
				MobileParty.MainParty.Ai.SetMoveGoToPoint(MobileParty.MainParty.Position2D);
				MBInformationManager.AddQuickInformation(new TextObject("{=4vsvniPd}I think we are on the right path now. You are the better rider so you should take the lead.", null), 0, StoryModeHeroes.ElderBrother.CharacterObject, "");
			}
		}

		private void OnCharacterCreationIsOver()
		{
			ActivityManager.SetActivityAvailability("CompleteMainQuest", true);
			ActivityManager.StartActivity("CompleteMainQuest");
			this._mainHeroEquipmentBackup[0] = Hero.MainHero.BattleEquipment.Clone(false);
			this._mainHeroEquipmentBackup[1] = Hero.MainHero.CivilianEquipment.Clone(false);
			this._brotherEquipmentBackup[0] = StoryModeHeroes.ElderBrother.BattleEquipment.Clone(false);
			this._brotherEquipmentBackup[1] = StoryModeHeroes.ElderBrother.CivilianEquipment.Clone(false);
			Settlement settlement = Settlement.Find("village_ES3_2");
			StoryModeHeroes.LittleBrother.UpdateLastKnownClosestSettlement(settlement);
			StoryModeHeroes.LittleSister.UpdateLastKnownClosestSettlement(settlement);
			Hero.MainHero.Mother.UpdateLastKnownClosestSettlement(settlement);
			Hero.MainHero.Father.UpdateLastKnownClosestSettlement(settlement);
		}

		private void OnNewGameCreatedPartialFollowUp(CampaignGameStarter campaignGameStarter, int i)
		{
			if (i == 99)
			{
				PartyBase.MainParty.ItemRoster.Clear();
				this.AddDialogAndGameMenus(campaignGameStarter);
				this.InitializeTutorial();
			}
		}

		private void OnGameLoaded(CampaignGameStarter campaignGameStarter)
		{
			this.AddDialogAndGameMenus(campaignGameStarter);
			Settlement settlement = Settlement.Find("village_ES3_2");
			if (Extensions.IsEmpty<Hero>(settlement.Notables))
			{
				this.CreateHeadman(settlement);
				return;
			}
			TutorialPhase.Instance.TutorialVillageHeadman = settlement.Notables.First<Hero>();
			if (!TutorialPhase.Instance.TutorialVillageHeadman.FirstName.Equals(new TextObject("{=Sb46O8WO}Orthos", null)))
			{
				TextObject textObject = new TextObject("{=JWLBKIkR}Headman {HEADMAN.FIRSTNAME}", null);
				TextObject textObject2 = new TextObject("{=Sb46O8WO}Orthos", null);
				TutorialPhase.Instance.TutorialVillageHeadman.SetName(textObject, textObject2);
				StringHelpers.SetCharacterProperties("HEADMAN", TutorialPhase.Instance.TutorialVillageHeadman.CharacterObject, textObject, false);
			}
		}

		private void OnStoryModeTutorialEnded()
		{
			Settlement settlement = Settlement.Find("village_ES3_2");
			if (settlement.Notables.Count > 1)
			{
				Debug.FailedAssert("There are more than one notable in tutorial phase, control it.", "C:\\Develop\\MB3\\Source\\Bannerlord\\StoryMode\\GameComponents\\CampaignBehaviors\\TutorialPhaseCampaignBehavior.cs", "OnStoryModeTutorialEnded", 188);
				using (List<Hero>.Enumerator enumerator = settlement.Notables.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Hero hero = enumerator.Current;
						hero.SetPersonalRelation(Hero.MainHero, 0);
					}
					goto IL_8A;
				}
			}
			Hero hero2 = settlement.Notables[0];
			hero2.SetPersonalRelation(Hero.MainHero, 0);
			KillCharacterAction.ApplyByRemove(hero2, false, true);
			IL_8A:
			this.SpawnAllNotablesForVillage(settlement.Village);
			VolunteerModel volunteerModel = Campaign.Current.Models.VolunteerModel;
			foreach (Hero hero3 in settlement.Notables)
			{
				if (hero3.IsAlive && volunteerModel.CanHaveRecruits(hero3))
				{
					CharacterObject basicVolunteer = volunteerModel.GetBasicVolunteer(hero3);
					for (int i = 0; i < hero3.VolunteerTypes.Length; i++)
					{
						if (hero3.VolunteerTypes[i] == null && MBRandom.RandomFloat < 0.5f)
						{
							hero3.VolunteerTypes[i] = basicVolunteer;
						}
					}
				}
			}
			DisableHeroAction.Apply(StoryModeHeroes.ElderBrother);
			StoryModeHeroes.ElderBrother.Clan = null;
			foreach (TroopRosterElement troopRosterElement in PartyBase.MainParty.MemberRoster.GetTroopRoster())
			{
				if (!troopRosterElement.Character.IsPlayerCharacter)
				{
					PartyBase.MainParty.MemberRoster.RemoveTroop(troopRosterElement.Character, PartyBase.MainParty.MemberRoster.GetTroopCount(troopRosterElement.Character), default(UniqueTroopDescriptor), 0);
				}
			}
			foreach (TroopRosterElement troopRosterElement2 in PartyBase.MainParty.PrisonRoster.GetTroopRoster())
			{
				if (troopRosterElement2.Character.IsHero)
				{
					DisableHeroAction.Apply(troopRosterElement2.Character.HeroObject);
				}
				else
				{
					PartyBase.MainParty.PrisonRoster.RemoveTroop(troopRosterElement2.Character, PartyBase.MainParty.PrisonRoster.GetTroopCount(troopRosterElement2.Character), default(UniqueTroopDescriptor), 0);
				}
			}
			TutorialPhase.Instance.RemoveTutorialFocusSettlement();
			PartyBase.MainParty.ItemRoster.Clear();
			Hero.MainHero.BattleEquipment.FillFrom(this._mainHeroEquipmentBackup[0], true);
			Hero.MainHero.CivilianEquipment.FillFrom(this._mainHeroEquipmentBackup[1], true);
			StoryModeHeroes.ElderBrother.BattleEquipment.FillFrom(this._brotherEquipmentBackup[0], true);
			StoryModeHeroes.ElderBrother.CivilianEquipment.FillFrom(this._brotherEquipmentBackup[1], true);
			PartyBase.MainParty.ItemRoster.AddToCounts(DefaultItems.Grain, 2);
			Hero.MainHero.Heal(Hero.MainHero.MaxHitPoints, false);
			Hero.MainHero.Gold = 1000;
			if (TutorialPhase.Instance.TutorialQuestPhase == TutorialQuestPhase.Finalized && !TutorialPhase.Instance.IsSkipped)
			{
				InformationManager.ShowInquiry(new InquiryData(new TextObject("{=EWD4Op6d}Notification", null).ToString(), new TextObject("{=GCbqpeDs}Tutorial is over. You are now free to explore Calradia.", null).ToString(), true, false, new TextObject("{=yS7PvrTD}OK", null).ToString(), null, delegate
				{
					MBInformationManager.ShowSceneNotification(new FindingFirstBannerPieceSceneNotificationItem(Hero.MainHero, null));
					CampaignEventDispatcher.Instance.RemoveListeners(this);
				}, null, "", 0f, null, null, null), false, false);
			}
		}

		private void InitializeTutorial()
		{
			Hero elderBrother = StoryModeHeroes.ElderBrother;
			elderBrother.ChangeState(1);
			AddHeroToPartyAction.Apply(elderBrother, MobileParty.MainParty, false);
			elderBrother.SetHasMet();
			DisableHeroAction.Apply(StoryModeHeroes.Tacitus);
			DisableHeroAction.Apply(StoryModeHeroes.LittleBrother);
			DisableHeroAction.Apply(StoryModeHeroes.LittleSister);
			DisableHeroAction.Apply(StoryModeHeroes.Radagos);
			DisableHeroAction.Apply(StoryModeHeroes.ImperialMentor);
			DisableHeroAction.Apply(StoryModeHeroes.AntiImperialMentor);
			DisableHeroAction.Apply(StoryModeHeroes.RadagosHencman);
			Settlement settlement = Settlement.Find("village_ES3_2");
			this.CreateHeadman(settlement);
			PartyBase.MainParty.ItemRoster.AddToCounts(DefaultItems.Grain, 1);
		}

		private void CreateHeadman(Settlement settlement)
		{
			Hero hero = HeroCreator.CreateHeroAtOccupation(20, settlement);
			TextObject textObject = new TextObject("{=JWLBKIkR}Headman {HEADMAN.FIRSTNAME}", null);
			TextObject textObject2 = new TextObject("{=Sb46O8WO}Orthos", null);
			hero.SetName(textObject, textObject2);
			StringHelpers.SetCharacterProperties("HEADMAN", hero.CharacterObject, textObject, false);
			hero.AddPower((float)(Campaign.Current.Models.NotablePowerModel.NotableDisappearPowerLimit * 2));
			TutorialPhase.Instance.TutorialVillageHeadman = hero;
		}

		private void AddDialogAndGameMenus(CampaignGameStarter campaignGameStarter)
		{
			campaignGameStarter.AddDialogLine("storymode_conversation_blocker", "start", "close_window", "{=9XnFlRR0}Interaction with this person is disabled during tutorial stage.", new ConversationSentence.OnConditionDelegate(this.storymode_conversation_blocker_on_condition), null, 1000000, null);
			campaignGameStarter.AddGameMenu("storymode_game_menu_blocker", "{=pVKkclVk}Interactions are limited during tutorial phase. This interaction is disabled.", new OnInitDelegate(this.storymode_game_menu_blocker_on_init), 0, 0, null);
			campaignGameStarter.AddGameMenuOption("storymode_game_menu_blocker", "game_menu_blocker_leave", "{=3sRdGQou}Leave", new GameMenuOption.OnConditionDelegate(this.game_menu_leave_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_leave_on_consequence), true, -1, false, null);
			campaignGameStarter.AddGameMenu("storymode_tutorial_village_game_menu", "{=7VFLb3Qj}You have arrived at the village.", new OnInitDelegate(this.storymode_tutorial_village_game_menu_on_init), 3, 0, null);
			campaignGameStarter.AddGameMenuOption("storymode_tutorial_village_game_menu", "storymode_tutorial_village_enter", "{=Xrz05hYE}Take a walk around", new GameMenuOption.OnConditionDelegate(this.storymode_tutorial_village_enter_on_condition), new GameMenuOption.OnConsequenceDelegate(this.storymode_tutorial_village_enter_on_consequence), false, -1, false, null);
			campaignGameStarter.AddGameMenuOption("storymode_tutorial_village_game_menu", "storymode_tutorial_village_hostile_action", "{=GM3tAYMr}Take a hostile action", new GameMenuOption.OnConditionDelegate(this.raid_village_menu_option_condition), null, false, -1, false, null);
			campaignGameStarter.AddGameMenuOption("storymode_tutorial_village_game_menu", "storymode_tutorial_village_recruit", "{=E31IJyqs}Recruit troops", new GameMenuOption.OnConditionDelegate(this.recruit_troops_village_menu_option_condition), new GameMenuOption.OnConsequenceDelegate(this.storymode_recruit_volunteers_on_consequence), false, -1, false, null);
			campaignGameStarter.AddGameMenuOption("storymode_tutorial_village_game_menu", "storymode_tutorial_village_buy", "{=VN4ctHIU}Buy products", new GameMenuOption.OnConditionDelegate(this.buy_products_village_menu_option_condition), new GameMenuOption.OnConsequenceDelegate(this.storymode_ui_village_buy_good_on_consequence), false, -1, false, null);
			campaignGameStarter.AddGameMenuOption("storymode_tutorial_village_game_menu", "storymode_tutorial_village_wait", "{=zEoHYEUS}Wait here for some time", new GameMenuOption.OnConditionDelegate(this.wait_village_menu_option_condition), null, false, -1, false, null);
			campaignGameStarter.AddGameMenuOption("storymode_tutorial_village_game_menu", "storymode_tutorial_village_leave", "{=3sRdGQou}Leave", new GameMenuOption.OnConditionDelegate(this.game_menu_leave_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_leave_on_consequence), true, -1, false, null);
		}

		private bool recruit_troops_village_menu_option_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = 13;
			return this.RecruitAndBuyProductsConditionsHold(args);
		}

		private bool buy_products_village_menu_option_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = 14;
			return this.RecruitAndBuyProductsConditionsHold(args);
		}

		private bool RecruitAndBuyProductsConditionsHold(MenuCallbackArgs args)
		{
			bool flag = TutorialPhase.Instance.TutorialQuestPhase >= TutorialQuestPhase.RecruitAndPurchaseStarted;
			args.IsEnabled = flag;
			args.Tooltip = (flag ? TextObject.Empty : new TextObject("{=TeMExjrH}This option is disabled during current active quest.", null));
			return true;
		}

		private bool raid_village_menu_option_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = 12;
			return this.PlaceholderOptionsClickableCondition(args);
		}

		private bool wait_village_menu_option_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = 15;
			return this.PlaceholderOptionsClickableCondition(args);
		}

		private bool PlaceholderOptionsClickableCondition(MenuCallbackArgs args)
		{
			args.IsEnabled = false;
			args.Tooltip = new TextObject("{=F7VxtCSd}This option is disabled during tutorial phase.", null);
			return true;
		}

		private void storymode_recruit_volunteers_on_consequence(MenuCallbackArgs args)
		{
			TutorialPhase.Instance.PrepareRecruitOptionForTutorial();
			args.MenuContext.OpenRecruitVolunteers();
		}

		private void storymode_ui_village_buy_good_on_consequence(MenuCallbackArgs args)
		{
			InventoryManager.OpenScreenAsTrade(TutorialPhase.Instance.GetAndPrepareBuyProductsOptionForTutorial(Settlement.CurrentSettlement.Village), Settlement.CurrentSettlement.Village, -1, null);
		}

		[GameMenuInitializationHandler("storymode_tutorial_village_game_menu")]
		private static void storymode_tutorial_village_game_menu_on_init_background(MenuCallbackArgs args)
		{
			args.MenuContext.SetBackgroundMeshName(Settlement.CurrentSettlement.Village.WaitMeshName);
		}

		[GameMenuInitializationHandler("storymode_game_menu_blocker")]
		private static void storymode_tutorial_blocker_game_menu_on_init_background(MenuCallbackArgs args)
		{
			args.MenuContext.SetBackgroundMeshName(SettlementHelper.FindNearestVillage(null, null).Village.WaitMeshName);
		}

		private void storymode_game_menu_blocker_on_init(MenuCallbackArgs args)
		{
			if (Settlement.CurrentSettlement != null && Settlement.CurrentSettlement.StringId == "village_ES3_2")
			{
				GameMenu.SwitchToMenu("storymode_tutorial_village_game_menu");
			}
		}

		private void storymode_tutorial_village_game_menu_on_init(MenuCallbackArgs args)
		{
			if (!StoryModeManager.Current.MainStoryLine.IsPlayerInteractionRestricted)
			{
				GameMenu.SwitchToMenu("village_outside");
				return;
			}
			Settlement currentSettlement = Settlement.CurrentSettlement;
			Campaign.Current.GameMenuManager.MenuLocations.Clear();
			Campaign.Current.GameMenuManager.MenuLocations.AddRange(currentSettlement.LocationComplex.GetListOfLocations());
		}

		private bool storymode_conversation_blocker_on_condition()
		{
			return StoryModeManager.Current.MainStoryLine.IsPlayerInteractionRestricted;
		}

		private bool storymode_tutorial_village_enter_on_condition(MenuCallbackArgs args)
		{
			List<Location> list = Settlement.CurrentSettlement.LocationComplex.GetListOfLocations().ToList<Location>();
			GameMenuOption.IssueQuestFlags issueQuestFlags = Campaign.Current.IssueManager.CheckIssueForMenuLocations(list, true);
			args.OptionQuestData |= issueQuestFlags;
			args.OptionQuestData |= Campaign.Current.QuestManager.CheckQuestForMenuLocations(list);
			args.optionLeaveType = 1;
			args.IsEnabled = !TutorialPhase.Instance.LockTutorialVillageEnter;
			if (!args.IsEnabled)
			{
				args.Tooltip = new TextObject("{=tWwXEWh6}Use the portrait to talk and enter the mission.", null);
			}
			return true;
		}

		private void storymode_tutorial_village_enter_on_consequence(MenuCallbackArgs args)
		{
			if (MobileParty.MainParty.CurrentSettlement == null)
			{
				PlayerEncounter.EnterSettlement();
			}
			VillageEncounter villageEncounter = PlayerEncounter.LocationEncounter as VillageEncounter;
			if (TutorialPhase.Instance.TutorialQuestPhase == TutorialQuestPhase.TravelToVillageStarted)
			{
				villageEncounter.CreateAndOpenMissionController(LocationComplex.Current.GetLocationWithId("village_center"), null, StoryModeHeroes.ElderBrother.CharacterObject, null);
				return;
			}
			villageEncounter.CreateAndOpenMissionController(LocationComplex.Current.GetLocationWithId("village_center"), null, null, null);
		}

		private bool game_menu_leave_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = 16;
			return true;
		}

		private bool game_menu_leave_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = 16;
			return true;
		}

		private void game_menu_leave_on_consequence(MenuCallbackArgs args)
		{
			PlayerEncounter.Finish(true);
		}

		private void SpawnYourBrotherInLocation(Hero hero, string locationId)
		{
			if (LocationComplex.Current != null)
			{
				Location locationWithId = LocationComplex.Current.GetLocationWithId(locationId);
				Monster baseMonsterFromRace = FaceGen.GetBaseMonsterFromRace(hero.CharacterObject.Race);
				AgentData agentData = new AgentData(new PartyAgentOrigin(PartyBase.MainParty, hero.CharacterObject, -1, default(UniqueTroopDescriptor), false)).Monster(baseMonsterFromRace).NoHorses(true);
				locationWithId.AddCharacter(new LocationCharacter(agentData, new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddFixedCharacterBehaviors), null, true, 1, null, true, false, null, false, true, true));
			}
		}

		private void OnQuestCompleted(QuestBase quest, QuestBase.QuestCompleteDetails detail)
		{
			if (quest is TravelToVillageTutorialQuest)
			{
				new TalkToTheHeadmanTutorialQuest(Settlement.CurrentSettlement.Notables.First((Hero n) => n.IsHeadman)).StartQuest();
				TutorialPhase.Instance.SetTutorialQuestPhase(TutorialQuestPhase.TalkToTheHeadmanStarted);
				return;
			}
			if (quest is TalkToTheHeadmanTutorialQuest)
			{
				new LocateAndRescueTravellerTutorialQuest(Settlement.CurrentSettlement.Notables.First((Hero n) => n.IsHeadman)).StartQuest();
				TutorialPhase.Instance.SetTutorialQuestPhase(TutorialQuestPhase.LocateAndRescueTravellerStarted);
				return;
			}
			if (quest is LocateAndRescueTravellerTutorialQuest)
			{
				new FindHideoutTutorialQuest(quest.QuestGiver, SettlementHelper.FindNearestHideout(null, quest.QuestGiver.CurrentSettlement)).StartQuest();
				TutorialPhase.Instance.SetTutorialQuestPhase(TutorialQuestPhase.FindHideoutStarted);
			}
		}

		private void OnSettlementEntered(MobileParty party, Settlement settlement, Hero hero)
		{
			if (settlement.StringId == "village_ES3_2" && !TutorialPhase.Instance.IsCompleted)
			{
				if (party != null)
				{
					if (party.IsMainParty)
					{
						this.SpawnYourBrotherInLocation(StoryModeHeroes.ElderBrother, "village_center");
					}
					else if (!party.IsMilitia)
					{
						party.Ai.SetMoveGoToSettlement(SettlementHelper.FindNearestSettlement((Settlement s) => s != settlement && (s.IsFortification || s.IsVillage) && settlement != s && settlement.MapFaction == s.MapFaction, party));
					}
				}
				if (party == null && hero != null && !hero.IsNotable)
				{
					TeleportHeroAction.ApplyImmediateTeleportToSettlement(hero, SettlementHelper.FindNearestSettlement((Settlement s) => s != settlement && (s.IsFortification || s.IsVillage) && settlement != s && settlement.MapFaction == s.MapFaction, settlement));
				}
			}
		}

		private void OnSettlementLeft(MobileParty party, Settlement settlement)
		{
			if (settlement.StringId == "tutorial_training_field" && party == MobileParty.MainParty && TutorialPhase.Instance.TutorialQuestPhase == TutorialQuestPhase.None)
			{
				new TravelToVillageTutorialQuest().StartQuest();
				TutorialPhase.Instance.SetTutorialQuestPhase(TutorialQuestPhase.TravelToVillageStarted);
				Campaign.Current.IssueManager.ToggleAllIssueTracks(false);
			}
			if (party == MobileParty.MainParty)
			{
				this.CheckIfMainPartyStarving();
			}
		}

		private void DailyTick()
		{
			Campaign.Current.IssueManager.ToggleAllIssueTracks(false);
			this.CheckIfMainPartyStarving();
		}

		private void OnGameLoadFinished()
		{
			if (Settlement.CurrentSettlement != null && Settlement.CurrentSettlement.StringId == "village_ES3_2" && !TutorialPhase.Instance.IsCompleted)
			{
				this.SpawnYourBrotherInLocation(StoryModeHeroes.ElderBrother, "village_center");
			}
		}

		private void CheckIfMainPartyStarving()
		{
			if (!TutorialPhase.Instance.IsCompleted && PartyBase.MainParty.IsStarving)
			{
				PartyBase.MainParty.ItemRoster.AddToCounts(DefaultItems.Grain, 1);
			}
		}

		private void SpawnAllNotablesForVillage(Village village)
		{
			int targetNotableCountForSettlement = Campaign.Current.Models.NotableSpawnModel.GetTargetNotableCountForSettlement(village.Settlement, 22);
			for (int i = 0; i < targetNotableCountForSettlement; i++)
			{
				HeroCreator.CreateHeroAtOccupation(22, village.Settlement);
			}
		}

		private bool _controlledByBrother;

		private bool _notifyPlayerAboutPosition;

		private Equipment[] _mainHeroEquipmentBackup = new Equipment[2];

		private Equipment[] _brotherEquipmentBackup = new Equipment[2];
	}
}
