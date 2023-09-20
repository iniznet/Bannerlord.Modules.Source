using System;
using Helpers;
using StoryMode.Extensions;
using StoryMode.StoryModeObjects;
using StoryMode.StoryModePhases;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace StoryMode.GameComponents.CampaignBehaviors
{
	// Token: 0x02000051 RID: 81
	public class TrainingFieldCampaignBehavior : CampaignBehaviorBase
	{
		// Token: 0x06000455 RID: 1109 RVA: 0x0001A3BF File Offset: 0x000185BF
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x06000456 RID: 1110 RVA: 0x0001A3C4 File Offset: 0x000185C4
		public override void RegisterEvents()
		{
			CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));
			CampaignEvents.OnMissionEndedEvent.AddNonSerializedListener(this, new Action<IMission>(this.OnMissionEnded));
			CampaignEvents.OnCharacterCreationIsOverEvent.AddNonSerializedListener(this, new Action(this.OnCharacterCreationIsOver));
		}

		// Token: 0x06000457 RID: 1111 RVA: 0x0001A418 File Offset: 0x00018618
		private void OnCharacterCreationIsOver()
		{
			if (!this.SkipTutorialMission)
			{
				Settlement settlement = Settlement.Find("tutorial_training_field");
				MobileParty.MainParty.Position2D = settlement.Position2D;
				EncounterManager.StartSettlementEncounter(MobileParty.MainParty, settlement);
				PlayerEncounter.LocationEncounter.CreateAndOpenMissionController(LocationComplex.Current.GetLocationWithId("training_field"), null, null, null);
			}
			this.SkipTutorialMission = false;
			foreach (MobileParty mobileParty in MobileParty.All)
			{
				mobileParty.Party.UpdateVisibilityAndInspected(0f, false);
			}
			foreach (Settlement settlement2 in Settlement.All)
			{
				settlement2.Party.UpdateVisibilityAndInspected(0f, false);
			}
		}

		// Token: 0x06000458 RID: 1112 RVA: 0x0001A510 File Offset: 0x00018710
		private void OnSessionLaunched(CampaignGameStarter campaignGameStarter)
		{
			campaignGameStarter.AddGameMenu("training_field_menu", "{=5g9ZFGrN}You are at a training field. You can learn the basics of combat here.", new OnInitDelegate(this.game_menu_training_field_on_init), 0, 0, null);
			campaignGameStarter.AddGameMenuOption("training_field_menu", "training_field_enter", "{=F0ldgio8}Go back to training.", delegate(MenuCallbackArgs args)
			{
				args.optionLeaveType = 1;
				return true;
			}, new GameMenuOption.OnConsequenceDelegate(TrainingFieldCampaignBehavior.game_menu_enter_training_field_on_consequence), false, -1, false, null);
			campaignGameStarter.AddGameMenuOption("training_field_menu", "training_field_leave", "{=3sRdGQou}Leave", delegate(MenuCallbackArgs args)
			{
				args.optionLeaveType = 16;
				return true;
			}, new GameMenuOption.OnConsequenceDelegate(TrainingFieldCampaignBehavior.game_menu_settlement_leave_on_consequence), true, -1, false, null);
			campaignGameStarter.AddDialogLine("brother_training_field_start_coversation", "start", "training_field_line_2", "{=4vsPD3ec}{?PLAYER.GENDER}Sister{?}Brother{\\?}... It's been three days now we've been tracking those bastards. I think we're getting close. We need to think about what happens when we catch them. How are we going to rescue {PLAYER_LITTLE_BROTHER.LINK} and {PLAYER_LITTLE_SISTER.LINK}? Are we up for a fight?[if:convo_grave]", new ConversationSentence.OnConditionDelegate(this.storymode_training_field_start_on_condition), null, 1000001, null);
			campaignGameStarter.AddDialogLine("brother_training_field_start_coversation", "training_field_line_2", "player_answer_training_field", "{=MfczTFxp}This looks like an old training field for the legions. Perhaps we can spare some time and brush up on our skills. The practice could come in handy when we catch up with the raiders.", null, null, 1000001, null);
			campaignGameStarter.AddPlayerLine("player_answer_play_training_field", "player_answer_training_field", "play_tutorial", "{=FaQDaRri}I'm going to run the course. I need to know I can fight if I have to. (Continue tutorial)", null, delegate
			{
				this._talkedWithBrotherForTheFirstTime = true;
			}, 100, null, null);
			campaignGameStarter.AddPlayerLine("player_answer_skip_tutorial", "player_answer_training_field", "skip_tutorial", "{=gYYGGflb}We have no time to lose. We can do more if we split up. (Skip tutorial)", null, null, 100, null, null);
			campaignGameStarter.AddPlayerLine("player_answer_ask_about_raiders_1", "player_answer_training_field", "ask_about_raiders_1", "{=b7Z1OBas}So, do you think we'll catch up with the raiders soon?", null, new ConversationSentence.OnConsequenceDelegate(this.storymode_asked_about_raiders_1_consequence), 100, new ConversationSentence.OnClickableConditionDelegate(this.storymode_asked_about_raiders_1_clickable_condition), null);
			campaignGameStarter.AddPlayerLine("player_answer_ask_about_raiders_2", "player_answer_training_field", "ask_about_raiders_2", "{=tzkclhXs}How should we prepare for the fight?", null, new ConversationSentence.OnConsequenceDelegate(this.storymode_asked_about_raiders_2_consequence), 100, new ConversationSentence.OnClickableConditionDelegate(this.storymode_asked_about_raiders_2_clickable_condition), null);
			campaignGameStarter.AddDialogLine("end_prolouge_conversation", "play_tutorial", "close_window", "{=IYnFgEgy}Let's go on then. (Play the combat tutorial)", null, new ConversationSentence.OnConsequenceDelegate(this.storymode_go_to_end_tutorial_village_consequence), 100, null);
			campaignGameStarter.AddDialogLine("ask_about_tutorial_end_confirmation", "skip_tutorial", "skip_tutorial_confirmation", "{=FUwIgcZO}Are you sure about that? (This option will finish the tutorial, which has story elements, and start the full single player campaign. It is recommended that you pick this option only if you have already played the tutorial once.)", null, null, 100, null);
			campaignGameStarter.AddDialogLine("explanation_about_raiders_1", "ask_about_raiders_1", "training_field_line_2", "{=YAWCkOYa}The tracks look fresh, and I've seen some smoke on the horizon. They can't move too quickly if they're still looting and raiding. No, I'm pretty sure we'll be able to rescue the little ones... or die trying.", null, null, 100, null);
			campaignGameStarter.AddDialogLine("explanation_about_raiders_2", "ask_about_raiders_2", "training_field_line_2", "{=NItH4oL6}Well, if they're still pillaging they may have split up into smaller groups. Hopefully we won't need to take them all on at once. But it would help if we could hire or persuade some people to join us.", null, null, 100, null);
			campaignGameStarter.AddPlayerLine("end_tutorial_yes", "skip_tutorial_confirmation", "end_tutorial", "{=a4W7Gzka}Yes. Time is of the essence. (Skip tutorial)", null, new ConversationSentence.OnConsequenceDelegate(this.storymode_skip_tutorial_from_conversation_consequence), 100, new ConversationSentence.OnClickableConditionDelegate(this.storymode_skip_tutorial_from_conversation_clickable_condition), null);
			campaignGameStarter.AddPlayerLine("end_tutorial_no", "skip_tutorial_confirmation", "training_field_line_2", "{=5qhaDtef}No. Let me rethink this.", null, null, 100, null, null);
			campaignGameStarter.AddDialogLine("end_tutorial_goodbye_start", "end_tutorial", "end_tutorial_goodbye", "{=QF8B6XFS}All right then. Let us split up and look for the little ones separately. I'll send you a word if I find them before you do...", null, null, 100, null);
			campaignGameStarter.AddDialogLine("end_tutorial_select_family_name", "end_tutorial_goodbye", "close_window", "{=LbSvq3be}One other thing, {?PLAYER.GENDER}sister{?}brother{\\?}. We want people to take us seriously. We may be leading men into battle soon. Let's give our family a name and a banner, like the nobles do.", null, new ConversationSentence.OnConsequenceDelegate(this.storymode_go_to_end_tutorial_village_consequence), 100, null);
			campaignGameStarter.AddDialogLine("brother_training_field_default_coversation", "start", "player_answer_training_field_default", "{=kIklPYto}Are you ready to leave here?", new ConversationSentence.OnConditionDelegate(this.story_mode_training_field_default_conversation_with_brother_condition), null, 1000001, null);
			campaignGameStarter.AddPlayerLine("player_answer_play_training_field", "player_answer_training_field_default", "close_window", "{=k07wzat8}I am not ready yet.", null, null, 100, null, null);
			campaignGameStarter.AddPlayerLine("player_answer_skip_tutorial", "player_answer_training_field_default", "close_window", "{=bSDt8FN5}I am ready, let's go!", null, delegate
			{
				Campaign.Current.ConversationManager.ConversationEndOneShot += delegate
				{
					Mission.Current.EndMission();
				};
			}, 100, null, null);
		}

		// Token: 0x06000459 RID: 1113 RVA: 0x0001A88C File Offset: 0x00018A8C
		private void OnMissionEnded(IMission mission)
		{
			if (this._completeTutorial)
			{
				StoryModeManager.Current.MainStoryLine.CompleteTutorialPhase(true);
				this._completeTutorial = false;
			}
		}

		// Token: 0x0600045A RID: 1114 RVA: 0x0001A8B0 File Offset: 0x00018AB0
		private void game_menu_training_field_on_init(MenuCallbackArgs args)
		{
			Settlement settlement = ((Settlement.CurrentSettlement == null) ? MobileParty.MainParty.CurrentSettlement : Settlement.CurrentSettlement);
			Campaign.Current.GameMenuManager.MenuLocations.Clear();
			Campaign.Current.GameMenuManager.MenuLocations.Add(settlement.LocationComplex.GetLocationWithId("training_field"));
			PlayerEncounter.EnterSettlement();
			PlayerEncounter.LocationEncounter = new TrainingFieldEncounter(settlement);
			MapState mapState;
			if ((mapState = GameStateManager.Current.ActiveState as MapState) != null)
			{
				mapState.Handler.TeleportCameraToMainParty();
			}
		}

		// Token: 0x0600045B RID: 1115 RVA: 0x0001A93C File Offset: 0x00018B3C
		private static void game_menu_enter_training_field_on_consequence(MenuCallbackArgs args)
		{
			PlayerEncounter.LocationEncounter.CreateAndOpenMissionController(LocationComplex.Current.GetLocationWithId("training_field"), null, null, null);
		}

		// Token: 0x0600045C RID: 1116 RVA: 0x0001A95C File Offset: 0x00018B5C
		[GameMenuInitializationHandler("training_field_menu")]
		private static void storymode_tutorial_training_field_game_menu_on_init_background(MenuCallbackArgs args)
		{
			TrainingField trainingField = Settlement.Find("tutorial_training_field").TrainingField();
			args.MenuContext.SetBackgroundMeshName(trainingField.WaitMeshName);
		}

		// Token: 0x0600045D RID: 1117 RVA: 0x0001A98A File Offset: 0x00018B8A
		private static void game_menu_settlement_leave_on_consequence(MenuCallbackArgs args)
		{
			PlayerEncounter.LeaveSettlement();
			PlayerEncounter.Finish(true);
		}

		// Token: 0x0600045E RID: 1118 RVA: 0x0001A998 File Offset: 0x00018B98
		private bool storymode_training_field_start_on_condition()
		{
			StringHelpers.SetCharacterProperties("PLAYER_LITTLE_BROTHER", StoryModeHeroes.LittleBrother.CharacterObject, null, false);
			StringHelpers.SetCharacterProperties("PLAYER_LITTLE_SISTER", StoryModeHeroes.LittleSister.CharacterObject, null, false);
			if (StoryModeManager.Current.MainStoryLine.IsPlayerInteractionRestricted)
			{
				Settlement currentSettlement = Settlement.CurrentSettlement;
				if (((currentSettlement != null) ? currentSettlement.StringId : null) == "tutorial_training_field" && Hero.OneToOneConversationHero != null && Hero.OneToOneConversationHero == StoryModeHeroes.ElderBrother)
				{
					return !this._talkedWithBrotherForTheFirstTime;
				}
			}
			return false;
		}

		// Token: 0x0600045F RID: 1119 RVA: 0x0001AA20 File Offset: 0x00018C20
		private void storymode_go_to_end_tutorial_village_consequence()
		{
			TutorialPhase.Instance.PlayerTalkedWithBrotherForTheFirstTime();
			if (this._completeTutorial)
			{
				Campaign.Current.ConversationManager.ConversationEndOneShot += delegate
				{
					Mission.Current.EndMission();
				};
			}
		}

		// Token: 0x06000460 RID: 1120 RVA: 0x0001AA6D File Offset: 0x00018C6D
		private bool storymode_skip_tutorial_from_conversation_clickable_condition(out TextObject explanation)
		{
			explanation = new TextObject("{=XlSHcfsP}This option will end the tutorial!", null);
			return true;
		}

		// Token: 0x06000461 RID: 1121 RVA: 0x0001AA7D File Offset: 0x00018C7D
		private void storymode_skip_tutorial_from_conversation_consequence()
		{
			this._completeTutorial = true;
		}

		// Token: 0x06000462 RID: 1122 RVA: 0x0001AA86 File Offset: 0x00018C86
		private bool storymode_asked_about_raiders_1_clickable_condition(out TextObject explanation)
		{
			explanation = TextObject.Empty;
			return !this._askedAboutRaiders1;
		}

		// Token: 0x06000463 RID: 1123 RVA: 0x0001AA98 File Offset: 0x00018C98
		private bool storymode_asked_about_raiders_2_clickable_condition(out TextObject explanation)
		{
			explanation = TextObject.Empty;
			return !this._askedAboutRaiders2;
		}

		// Token: 0x06000464 RID: 1124 RVA: 0x0001AAAA File Offset: 0x00018CAA
		private void storymode_asked_about_raiders_1_consequence()
		{
			this._askedAboutRaiders1 = true;
		}

		// Token: 0x06000465 RID: 1125 RVA: 0x0001AAB3 File Offset: 0x00018CB3
		private void storymode_asked_about_raiders_2_consequence()
		{
			this._askedAboutRaiders2 = true;
		}

		// Token: 0x06000466 RID: 1126 RVA: 0x0001AABC File Offset: 0x00018CBC
		private bool story_mode_training_field_default_conversation_with_brother_condition()
		{
			return StoryModeManager.Current.MainStoryLine.IsPlayerInteractionRestricted && (Settlement.CurrentSettlement == null || Settlement.CurrentSettlement.StringId != "village_ES3_2") && CharacterObject.OneToOneConversationCharacter == StoryModeHeroes.ElderBrother.CharacterObject && this._talkedWithBrotherForTheFirstTime;
		}

		// Token: 0x040001C4 RID: 452
		public bool SkipTutorialMission;

		// Token: 0x040001C5 RID: 453
		private const string TrainingFieldLocationId = "training_field";

		// Token: 0x040001C6 RID: 454
		private bool _completeTutorial;

		// Token: 0x040001C7 RID: 455
		private bool _askedAboutRaiders1;

		// Token: 0x040001C8 RID: 456
		private bool _askedAboutRaiders2;

		// Token: 0x040001C9 RID: 457
		private bool _talkedWithBrotherForTheFirstTime;
	}
}
