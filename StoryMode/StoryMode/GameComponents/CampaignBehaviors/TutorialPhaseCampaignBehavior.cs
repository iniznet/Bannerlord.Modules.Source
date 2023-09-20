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
	// Token: 0x02000052 RID: 82
	public class TutorialPhaseCampaignBehavior : CampaignBehaviorBase
	{
		// Token: 0x06000469 RID: 1129 RVA: 0x0001AB24 File Offset: 0x00018D24
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

		// Token: 0x0600046A RID: 1130 RVA: 0x0001AC48 File Offset: 0x00018E48
		private void OnCanHaveQuestsOrIssuesInfoIsRequested(Hero hero, ref bool result)
		{
			Settlement settlement = Settlement.Find("village_ES3_2");
			if (!StoryModeManager.Current.MainStoryLine.TutorialPhase.IsCompleted && settlement.Notables.Contains(hero))
			{
				result = false;
			}
		}

		// Token: 0x0600046B RID: 1131 RVA: 0x0001AC87 File Offset: 0x00018E87
		private void CanHeroMarry(Hero hero, ref bool result)
		{
			if (!TutorialPhase.Instance.IsCompleted && hero.Clan == Clan.PlayerClan)
			{
				result = false;
			}
		}

		// Token: 0x0600046C RID: 1132 RVA: 0x0001ACA5 File Offset: 0x00018EA5
		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<Equipment[]>("_mainHeroEquipmentBackup", ref this._mainHeroEquipmentBackup);
			dataStore.SyncData<Equipment[]>("_brotherEquipmentBackup", ref this._brotherEquipmentBackup);
		}

		// Token: 0x0600046D RID: 1133 RVA: 0x0001ACCC File Offset: 0x00018ECC
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

		// Token: 0x0600046E RID: 1134 RVA: 0x0001AE58 File Offset: 0x00019058
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

		// Token: 0x0600046F RID: 1135 RVA: 0x0001AF1D File Offset: 0x0001911D
		private void OnNewGameCreatedPartialFollowUp(CampaignGameStarter campaignGameStarter, int i)
		{
			if (i == 99)
			{
				PartyBase.MainParty.ItemRoster.Clear();
				this.AddDialogAndGameMenus(campaignGameStarter);
				this.InitializeTutorial();
			}
		}

		// Token: 0x06000470 RID: 1136 RVA: 0x0001AF40 File Offset: 0x00019140
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

		// Token: 0x06000471 RID: 1137 RVA: 0x0001AFF0 File Offset: 0x000191F0
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
				if (volunteerModel.CanHaveRecruits(hero3))
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
			StoryModeHeroes.ElderBrother.Clan = CampaignData.NeutralFaction;
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

		// Token: 0x06000472 RID: 1138 RVA: 0x0001B3A0 File Offset: 0x000195A0
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

		// Token: 0x06000473 RID: 1139 RVA: 0x0001B438 File Offset: 0x00019638
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

		// Token: 0x06000474 RID: 1140 RVA: 0x0001B4AC File Offset: 0x000196AC
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

		// Token: 0x06000475 RID: 1141 RVA: 0x0001B666 File Offset: 0x00019866
		private bool recruit_troops_village_menu_option_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = 13;
			return this.RecruitAndBuyProductsConditionsHold(args);
		}

		// Token: 0x06000476 RID: 1142 RVA: 0x0001B677 File Offset: 0x00019877
		private bool buy_products_village_menu_option_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = 14;
			return this.RecruitAndBuyProductsConditionsHold(args);
		}

		// Token: 0x06000477 RID: 1143 RVA: 0x0001B688 File Offset: 0x00019888
		private bool RecruitAndBuyProductsConditionsHold(MenuCallbackArgs args)
		{
			bool flag = TutorialPhase.Instance.TutorialQuestPhase >= TutorialQuestPhase.RecruitAndPurchaseStarted;
			args.IsEnabled = flag;
			args.Tooltip = (flag ? TextObject.Empty : new TextObject("{=TeMExjrH}This option is disabled during current active quest.", null));
			return true;
		}

		// Token: 0x06000478 RID: 1144 RVA: 0x0001B6C9 File Offset: 0x000198C9
		private bool raid_village_menu_option_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = 12;
			return this.PlaceholderOptionsClickableCondition(args);
		}

		// Token: 0x06000479 RID: 1145 RVA: 0x0001B6DA File Offset: 0x000198DA
		private bool wait_village_menu_option_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = 15;
			return this.PlaceholderOptionsClickableCondition(args);
		}

		// Token: 0x0600047A RID: 1146 RVA: 0x0001B6EB File Offset: 0x000198EB
		private bool PlaceholderOptionsClickableCondition(MenuCallbackArgs args)
		{
			args.IsEnabled = false;
			args.Tooltip = new TextObject("{=F7VxtCSd}This option is disabled during tutorial phase.", null);
			return true;
		}

		// Token: 0x0600047B RID: 1147 RVA: 0x0001B706 File Offset: 0x00019906
		private void storymode_recruit_volunteers_on_consequence(MenuCallbackArgs args)
		{
			TutorialPhase.Instance.PrepareRecruitOptionForTutorial();
			args.MenuContext.OpenRecruitVolunteers();
		}

		// Token: 0x0600047C RID: 1148 RVA: 0x0001B71D File Offset: 0x0001991D
		private void storymode_ui_village_buy_good_on_consequence(MenuCallbackArgs args)
		{
			InventoryManager.OpenScreenAsTrade(TutorialPhase.Instance.GetAndPrepareBuyProductsOptionForTutorial(Settlement.CurrentSettlement.Village), Settlement.CurrentSettlement.Village, -1, null);
		}

		// Token: 0x0600047D RID: 1149 RVA: 0x0001B744 File Offset: 0x00019944
		[GameMenuInitializationHandler("storymode_tutorial_village_game_menu")]
		private static void storymode_tutorial_village_game_menu_on_init_background(MenuCallbackArgs args)
		{
			args.MenuContext.SetBackgroundMeshName(Settlement.CurrentSettlement.Village.WaitMeshName);
		}

		// Token: 0x0600047E RID: 1150 RVA: 0x0001B760 File Offset: 0x00019960
		[GameMenuInitializationHandler("storymode_game_menu_blocker")]
		private static void storymode_tutorial_blocker_game_menu_on_init_background(MenuCallbackArgs args)
		{
			args.MenuContext.SetBackgroundMeshName(SettlementHelper.FindNearestVillage(null, null).Village.WaitMeshName);
		}

		// Token: 0x0600047F RID: 1151 RVA: 0x0001B77E File Offset: 0x0001997E
		private void storymode_game_menu_blocker_on_init(MenuCallbackArgs args)
		{
			if (Settlement.CurrentSettlement != null && Settlement.CurrentSettlement.StringId == "village_ES3_2")
			{
				GameMenu.SwitchToMenu("storymode_tutorial_village_game_menu");
			}
		}

		// Token: 0x06000480 RID: 1152 RVA: 0x0001B7A8 File Offset: 0x000199A8
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

		// Token: 0x06000481 RID: 1153 RVA: 0x0001B80A File Offset: 0x00019A0A
		private bool storymode_conversation_blocker_on_condition()
		{
			return StoryModeManager.Current.MainStoryLine.IsPlayerInteractionRestricted;
		}

		// Token: 0x06000482 RID: 1154 RVA: 0x0001B81C File Offset: 0x00019A1C
		private bool storymode_tutorial_village_enter_on_condition(MenuCallbackArgs args)
		{
			List<Location> list = Settlement.CurrentSettlement.LocationComplex.GetListOfLocations().ToList<Location>();
			if (TutorialPhase.Instance.TutorialQuestPhase == TutorialQuestPhase.TalkToTheHeadmanStarted)
			{
				GameMenuOption.IssueQuestFlags issueQuestFlags = Campaign.Current.IssueManager.CheckIssueForMenuLocations(list, true);
				args.OptionQuestData |= issueQuestFlags;
				args.OptionQuestData |= Campaign.Current.QuestManager.CheckQuestForMenuLocations(list);
			}
			else
			{
				args.OptionQuestData = 0;
			}
			args.optionLeaveType = 1;
			args.IsEnabled = !TutorialPhase.Instance.LockTutorialVillageEnter;
			if (!args.IsEnabled)
			{
				args.Tooltip = new TextObject("{=tWwXEWh6}Use the portrait to talk and enter the mission.", null);
			}
			return true;
		}

		// Token: 0x06000483 RID: 1155 RVA: 0x0001B8C8 File Offset: 0x00019AC8
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

		// Token: 0x06000484 RID: 1156 RVA: 0x0001B939 File Offset: 0x00019B39
		private bool game_menu_leave_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = 16;
			return true;
		}

		// Token: 0x06000485 RID: 1157 RVA: 0x0001B944 File Offset: 0x00019B44
		private bool game_menu_leave_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = 16;
			return true;
		}

		// Token: 0x06000486 RID: 1158 RVA: 0x0001B94F File Offset: 0x00019B4F
		private void game_menu_leave_on_consequence(MenuCallbackArgs args)
		{
			PlayerEncounter.Finish(true);
		}

		// Token: 0x06000487 RID: 1159 RVA: 0x0001B958 File Offset: 0x00019B58
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

		// Token: 0x06000488 RID: 1160 RVA: 0x0001B9E0 File Offset: 0x00019BE0
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

		// Token: 0x06000489 RID: 1161 RVA: 0x0001BABC File Offset: 0x00019CBC
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

		// Token: 0x0600048A RID: 1162 RVA: 0x0001BB68 File Offset: 0x00019D68
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

		// Token: 0x0600048B RID: 1163 RVA: 0x0001BBCF File Offset: 0x00019DCF
		private void DailyTick()
		{
			Campaign.Current.IssueManager.ToggleAllIssueTracks(false);
			this.CheckIfMainPartyStarving();
		}

		// Token: 0x0600048C RID: 1164 RVA: 0x0001BBE7 File Offset: 0x00019DE7
		private void OnGameLoadFinished()
		{
			if (Settlement.CurrentSettlement != null && Settlement.CurrentSettlement.StringId == "village_ES3_2" && !TutorialPhase.Instance.IsCompleted)
			{
				this.SpawnYourBrotherInLocation(StoryModeHeroes.ElderBrother, "village_center");
			}
		}

		// Token: 0x0600048D RID: 1165 RVA: 0x0001BC22 File Offset: 0x00019E22
		private void CheckIfMainPartyStarving()
		{
			if (!TutorialPhase.Instance.IsCompleted && PartyBase.MainParty.IsStarving)
			{
				PartyBase.MainParty.ItemRoster.AddToCounts(DefaultItems.Grain, 1);
			}
		}

		// Token: 0x0600048E RID: 1166 RVA: 0x0001BC54 File Offset: 0x00019E54
		private void SpawnAllNotablesForVillage(Village village)
		{
			int targetNotableCountForSettlement = Campaign.Current.Models.NotableSpawnModel.GetTargetNotableCountForSettlement(village.Settlement, 22);
			for (int i = 0; i < targetNotableCountForSettlement; i++)
			{
				HeroCreator.CreateHeroAtOccupation(22, village.Settlement);
			}
		}

		// Token: 0x040001CA RID: 458
		private bool _controlledByBrother;

		// Token: 0x040001CB RID: 459
		private bool _notifyPlayerAboutPosition;

		// Token: 0x040001CC RID: 460
		private Equipment[] _mainHeroEquipmentBackup = new Equipment[2];

		// Token: 0x040001CD RID: 461
		private Equipment[] _brotherEquipmentBackup = new Equipment[2];
	}
}
