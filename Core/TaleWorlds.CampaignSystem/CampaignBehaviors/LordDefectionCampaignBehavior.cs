using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem.BarterSystem;
using TaleWorlds.CampaignSystem.BarterSystem.Barterables;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Conversation.Persuasion;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	public class LordDefectionCampaignBehavior : CampaignBehaviorBase
	{
		public LordDefectionCampaignBehavior()
		{
			this._previousDefectionPersuasionAttempts = new List<PersuasionAttempt>();
		}

		public override void RegisterEvents()
		{
			CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));
			CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, new Action(this.OnDailyTick));
		}

		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<List<PersuasionAttempt>>("previousPersuasionAttempts", ref this._previousDefectionPersuasionAttempts);
		}

		public void OnSessionLaunched(CampaignGameStarter campaignGameStarter)
		{
			this.AddDialogs(campaignGameStarter);
		}

		public void ClearPersuasion()
		{
			this._previousDefectionPersuasionAttempts.Clear();
		}

		private PersuasionTask GetFailedPersuasionTask(LordDefectionCampaignBehavior.DefectionReservationType reservationType)
		{
			foreach (PersuasionTask persuasionTask in this._allReservations)
			{
				if (persuasionTask.ReservationType == (int)reservationType && !this.CanAttemptToPersuade(Hero.OneToOneConversationHero, (int)reservationType))
				{
					return persuasionTask;
				}
			}
			return null;
		}

		private PersuasionTask GetAnyFailedPersuasionTask()
		{
			foreach (PersuasionTask persuasionTask in this._allReservations)
			{
				if (!this.CanAttemptToPersuade(Hero.OneToOneConversationHero, persuasionTask.ReservationType))
				{
					return persuasionTask;
				}
			}
			return null;
		}

		private PersuasionTask GetCurrentPersuasionTask()
		{
			foreach (PersuasionTask persuasionTask in this._allReservations)
			{
				if (!persuasionTask.Options.All((PersuasionOptionArgs x) => x.IsBlocked))
				{
					return persuasionTask;
				}
			}
			return this._allReservations[this._allReservations.Count - 1];
		}

		protected void AddDialogs(CampaignGameStarter starter)
		{
			this.AddLordDefectionPersuasionOptions(starter);
			string text = "lord_ask_recruit_argument_1";
			string text2 = "lord_ask_recruit_persuasion";
			string text3 = "lord_defection_reaction";
			string text4 = "{=!}{DEFECTION_PERSUADE_ATTEMPT_1}";
			ConversationSentence.OnConditionDelegate onConditionDelegate = new ConversationSentence.OnConditionDelegate(this.conversation_lord_recruit_1_persuade_option_on_condition);
			ConversationSentence.OnConsequenceDelegate onConsequenceDelegate = new ConversationSentence.OnConsequenceDelegate(this.conversation_lord_recruit_1_persuade_option_on_consequence);
			int num = 100;
			ConversationSentence.OnPersuasionOptionDelegate onPersuasionOptionDelegate = new ConversationSentence.OnPersuasionOptionDelegate(this.SetupDefectionPersuasionOption1);
			starter.AddPlayerLine(text, text2, text3, text4, onConditionDelegate, onConsequenceDelegate, num, new ConversationSentence.OnClickableConditionDelegate(this.DefectionPersuasionOption1ClickableOnCondition1), onPersuasionOptionDelegate);
			string text5 = "lord_ask_recruit_argument_2";
			string text6 = "lord_ask_recruit_persuasion";
			string text7 = "lord_defection_reaction";
			string text8 = "{=!}{DEFECTION_PERSUADE_ATTEMPT_2}";
			ConversationSentence.OnConditionDelegate onConditionDelegate2 = new ConversationSentence.OnConditionDelegate(this.conversation_lord_recruit_2_persuade_option_on_condition);
			ConversationSentence.OnConsequenceDelegate onConsequenceDelegate2 = new ConversationSentence.OnConsequenceDelegate(this.conversation_lord_recruit_2_persuade_option_on_consequence);
			int num2 = 100;
			onPersuasionOptionDelegate = new ConversationSentence.OnPersuasionOptionDelegate(this.SetupDefectionPersuasionOption2);
			starter.AddPlayerLine(text5, text6, text7, text8, onConditionDelegate2, onConsequenceDelegate2, num2, new ConversationSentence.OnClickableConditionDelegate(this.DefectionPersuasionOption2ClickableOnCondition2), onPersuasionOptionDelegate);
			string text9 = "lord_ask_recruit_argument_3";
			string text10 = "lord_ask_recruit_persuasion";
			string text11 = "lord_defection_reaction";
			string text12 = "{=!}{DEFECTION_PERSUADE_ATTEMPT_3}";
			ConversationSentence.OnConditionDelegate onConditionDelegate3 = new ConversationSentence.OnConditionDelegate(this.conversation_lord_recruit_3_persuade_option_on_condition);
			ConversationSentence.OnConsequenceDelegate onConsequenceDelegate3 = new ConversationSentence.OnConsequenceDelegate(this.conversation_lord_recruit_3_persuade_option_on_consequence);
			int num3 = 100;
			onPersuasionOptionDelegate = new ConversationSentence.OnPersuasionOptionDelegate(this.SetupDefectionPersuasionOption3);
			starter.AddPlayerLine(text9, text10, text11, text12, onConditionDelegate3, onConsequenceDelegate3, num3, new ConversationSentence.OnClickableConditionDelegate(this.DefectionPersuasionOption3ClickableOnCondition3), onPersuasionOptionDelegate);
			string text13 = "lord_ask_recruit_argument_4";
			string text14 = "lord_ask_recruit_persuasion";
			string text15 = "lord_defection_reaction";
			string text16 = "{=!}{DEFECTION_PERSUADE_ATTEMPT_4}";
			ConversationSentence.OnConditionDelegate onConditionDelegate4 = new ConversationSentence.OnConditionDelegate(this.conversation_lord_recruit_4_persuade_option_on_condition);
			ConversationSentence.OnConsequenceDelegate onConsequenceDelegate4 = new ConversationSentence.OnConsequenceDelegate(this.conversation_lord_recruit_4_persuade_option_on_consequence);
			int num4 = 100;
			onPersuasionOptionDelegate = new ConversationSentence.OnPersuasionOptionDelegate(this.SetupDefectionPersuasionOption4);
			starter.AddPlayerLine(text13, text14, text15, text16, onConditionDelegate4, onConsequenceDelegate4, num4, new ConversationSentence.OnClickableConditionDelegate(this.DefectionPersuasionOption4ClickableOnCondition4), onPersuasionOptionDelegate);
			starter.AddPlayerLine("lord_ask_recruit_argument_no_answer", "lord_ask_recruit_persuasion", "lord_pretalk", "{=0eAtiZbL}I have no answer to that.", null, new ConversationSentence.OnConsequenceDelegate(this.conversation_on_end_persuasion_on_consequence), 100, null, null);
			starter.AddDialogLine("lord_ask_recruit_argument_reaction", "lord_defection_reaction", "lord_defection_next_reservation", "{=!}{PERSUASION_REACTION}", new ConversationSentence.OnConditionDelegate(this.conversation_lord_persuade_option_reaction_on_condition), new ConversationSentence.OnConsequenceDelegate(this.conversation_lord_persuade_option_reaction_on_consequence), 100, null);
		}

		private void AddLordDefectionPersuasionOptions(CampaignGameStarter starter)
		{
			starter.AddPlayerLine("player_is_requesting_enemy_change_sides", "lord_talk_speak_diplomacy_2", "persuasion_leave_faction_npc", "{=5a0NhbOA}Your liege, {FIRST_NAME}, is not worth of your loyalty.", new ConversationSentence.OnConditionDelegate(this.conversation_player_is_asking_to_recruit_enemy_on_condition), null, 100, null, null);
			starter.AddPlayerLine("player_is_requesting_neutral_change_sides", "lord_talk_speak_diplomacy_2", "persuasion_leave_faction_npc", "{=3gbgjJfZ}Candidly, what do you think of your liege, {FIRST_NAME}?", new ConversationSentence.OnConditionDelegate(this.conversation_player_is_asking_to_recruit_neutral_on_condition), null, 100, null, null);
			starter.AddPlayerLine("player_suggesting_treason", "lord_talk_speak_diplomacy_2", "persuasion_leave_faction_npc", "{=bKsb7tcr}Candidly, what do you think of our liege, {FIRST_NAME}?", new ConversationSentence.OnConditionDelegate(this.conversation_suggest_treason_on_condition), null, 100, null, null);
			starter.AddPlayerLine("persuasion_leave_faction_player_cheat", "lord_talk_speak_diplomacy_2", "persuasion_leave_faction_npc", "{=Cd405TC7}Clear past persuasion attempts (CHEAT)", delegate
			{
				if (Game.Current.IsDevelopmentMode)
				{
					return this._previousDefectionPersuasionAttempts.Any((PersuasionAttempt x) => x.PersuadedHero == Hero.OneToOneConversationHero);
				}
				return false;
			}, new ConversationSentence.OnConsequenceDelegate(this.conversation_clear_persuasion_on_consequence), 100, null, null);
			starter.AddPlayerLine("player_prisoner_talk", "hero_main_options", "persuasion_leave_faction_npc", "{=wNSH1JdJ}I have an offer for you: join us, and be set free.", new ConversationSentence.OnConditionDelegate(this.conversation_player_start_defection_with_prisoner_on_condition), null, 100, null, null);
			starter.AddDialogLine("player_prisoner_talk_pre_barter", "player_prisoner_defection", "persuasion_leave_faction_npc", "{=DRkWMe5X}Even now, I am not sure that's in my best interests...", null, null, 100, null);
			starter.AddDialogLine("persuasion_leave_faction_npc_refuse", "persuasion_leave_faction_npc", "lord_pretalk", "{=!}{LIEGE_IS_RELATIVE}", new ConversationSentence.OnConditionDelegate(this.conversation_lord_from_ruling_clan_on_condition), null, 100, null);
			starter.AddDialogLine("persuasion_leave_faction_npc_refuse_high_negative_score", "persuasion_leave_faction_npc", "lord_pretalk", "{=ZYUHljOa}I am happy with my current liege. Neither your purse nor our relationship is deep enough to change that.", new ConversationSentence.OnConditionDelegate(this.conversation_lord_persuade_option_reaction_pre_reject_on_condition), null, 100, null);
			starter.AddDialogLine("persuasion_leave_faction_npc_redirected", "persuasion_leave_faction_npc", "lord_pretalk", "{=UW1roOES}You should discuss this issue with {REDIRECT_HERO_RELATIONSHIP}, who speaks for our family.", new ConversationSentence.OnConditionDelegate(this.conversation_lord_redirects_to_clan_leader_on_condition), new ConversationSentence.OnConsequenceDelegate(this.persuasion_redierect_player_finish_on_consequece), 100, null);
			starter.AddDialogLine("persuasion_leave_faction_npc_answer", "persuasion_leave_faction_npc", "persuasion_leave_faction_player", "{=yub5GWVq}What are you saying, exactly?[if:convo_thinking]", () => !this.conversation_lord_redirects_to_clan_leader_on_condition(), null, 100, null);
			starter.AddPlayerLine("persuasion_leave_faction_player_start", "persuasion_leave_faction_player", "lord_defection_next_reservation", "{=!}{RECRUIT_START}", new ConversationSentence.OnConditionDelegate(this.conversation_lord_can_recruit_on_condition), new ConversationSentence.OnConsequenceDelegate(this.start_lord_defection_persuasion_on_consequence), 100, null, null);
			starter.AddDialogLine("lord_ask_recruit_next_reservation_fail", "lord_defection_next_reservation", "lord_pretalk", "{=!}{FAILED_PERSUASION_LINE}", new ConversationSentence.OnConditionDelegate(this.conversation_lord_player_has_failed_in_defection_on_condition), new ConversationSentence.OnConsequenceDelegate(this.conversation_on_end_persuasion_on_consequence), 100, null);
			starter.AddDialogLine("lord_ask_recruit_next_reservation_attempt", "lord_defection_next_reservation", "lord_ask_recruit_persuasion", "{=!}{PERSUASION_TASK_LINE}[if:convo_thinking]", new ConversationSentence.OnConditionDelegate(this.conversation_lord_recruit_check_if_reservations_met_on_condition), null, 100, null);
			starter.AddDialogLine("lord_ask_recruit_next_reservation_success_without_barter", "lord_defection_next_reservation", "close_window", "{=!}{DEFECTION_AGREE_WITHOUT_BARTER}", new ConversationSentence.OnConditionDelegate(this.conversation_lord_check_if_ready_to_join_faction_without_barter_on_condition), new ConversationSentence.OnConsequenceDelegate(this.conversation_lord_defect_to_clan_without_barter_on_consequence), 100, null);
			starter.AddDialogLine("lord_ask_recruit_next_reservation_success_with_barter", "lord_defection_next_reservation", "lord_ask_recruit_ai_argument_reaction", "{=BeYbp6M2}Very well. You've convinced me that this is something I can consider.", new ConversationSentence.OnConditionDelegate(this.conversation_lord_check_if_ready_to_join_faction_with_barter_on_condition), null, 100, null);
			starter.AddDialogLine("persuasion_leave_faction_npc_result_success_2", "lord_ask_recruit_ai_argument_reaction", "lord_defection_barter_line", "{=0dY1xyyK}This is a dangerous step, however, and I'm putting my life and the lives of my people at risk. I need some sort of support from you before I can change my allegiance.[if:convo_stern]", null, null, 100, null);
			starter.AddDialogLine("persuasion_leave_faction_npc_result_success_open_barter", "lord_defection_barter_line", "lord_defection_post_barter", "{=!}BARTER LINE - Covered by barter interface. Please do not remove these lines!", null, new ConversationSentence.OnConsequenceDelegate(this.conversation_leave_faction_barter_consequence), 100, null);
			starter.AddDialogLine("lord_defection_post_barter_s", "lord_defection_post_barter", "close_window", "{=9aZgTNiU}Very well. This is a great step to take, but it must be done.[if:convo_calm_friendly][ib:confident]", new ConversationSentence.OnConditionDelegate(this.defection_barter_successful_on_condition), new ConversationSentence.OnConsequenceDelegate(this.defection_successful_on_consequence), 100, null);
			starter.AddDialogLine("lord_defection_post_barter_f", "lord_defection_post_barter", "close_window", "{=BO9QV55x}I cannot do what you ask.[if:convo_grave]", () => !this.defection_barter_successful_on_condition(), null, 100, null);
		}

		private bool defection_barter_successful_on_condition()
		{
			return Campaign.Current.BarterManager.LastBarterIsAccepted;
		}

		private void defection_successful_on_consequence()
		{
			TraitLevelingHelper.OnPersuasionDefection(Hero.OneToOneConversationHero);
			if (PlayerEncounter.Current != null)
			{
				PlayerEncounter.LeaveEncounter = true;
			}
			foreach (PersuasionAttempt persuasionAttempt in this._previousDefectionPersuasionAttempts)
			{
				if (persuasionAttempt.PersuadedHero == Hero.OneToOneConversationHero)
				{
					PersuasionOptionResult result = persuasionAttempt.Result;
					if (result != PersuasionOptionResult.Success)
					{
						if (result == PersuasionOptionResult.CriticalSuccess)
						{
							int num = ((persuasionAttempt.Args.ArgumentStrength < PersuasionArgumentStrength.Normal) ? (MathF.Abs((int)persuasionAttempt.Args.ArgumentStrength) * 50) : 50);
							SkillLevelingManager.OnPersuasionSucceeded(Hero.MainHero, persuasionAttempt.Args.SkillUsed, PersuasionDifficulty.Medium, 2 * num);
						}
					}
					else
					{
						int num = ((persuasionAttempt.Args.ArgumentStrength < PersuasionArgumentStrength.Normal) ? (MathF.Abs((int)persuasionAttempt.Args.ArgumentStrength) * 50) : 50);
						SkillLevelingManager.OnPersuasionSucceeded(Hero.MainHero, persuasionAttempt.Args.SkillUsed, PersuasionDifficulty.Medium, num);
					}
				}
			}
			IStatisticsCampaignBehavior behavior = Campaign.Current.CampaignBehaviorManager.GetBehavior<IStatisticsCampaignBehavior>();
			if (behavior != null)
			{
				behavior.OnDefectionPersuasionSucess();
			}
		}

		private bool conversation_lord_recruit_1_persuade_option_on_condition()
		{
			PersuasionTask currentPersuasionTask = this.GetCurrentPersuasionTask();
			if (currentPersuasionTask.Options.Count > 0)
			{
				TextObject textObject = new TextObject("{=bSo9hKwr}{PERSUASION_OPTION_LINE} {SUCCESS_CHANCE}", null);
				textObject.SetTextVariable("SUCCESS_CHANCE", PersuasionHelper.ShowSuccess(currentPersuasionTask.Options.ElementAt(0), true));
				textObject.SetTextVariable("PERSUASION_OPTION_LINE", currentPersuasionTask.Options.ElementAt(0).Line);
				MBTextManager.SetTextVariable("DEFECTION_PERSUADE_ATTEMPT_1", textObject, false);
				return true;
			}
			return false;
		}

		private void conversation_lord_recruit_1_persuade_option_on_consequence()
		{
			PersuasionTask currentPersuasionTask = this.GetCurrentPersuasionTask();
			if (currentPersuasionTask.Options.Count > 0)
			{
				currentPersuasionTask.Options[0].BlockTheOption(true);
			}
		}

		private void conversation_lord_recruit_2_persuade_option_on_consequence()
		{
			PersuasionTask currentPersuasionTask = this.GetCurrentPersuasionTask();
			if (currentPersuasionTask.Options.Count > 1)
			{
				currentPersuasionTask.Options[1].BlockTheOption(true);
			}
		}

		private void conversation_lord_recruit_3_persuade_option_on_consequence()
		{
			PersuasionTask currentPersuasionTask = this.GetCurrentPersuasionTask();
			if (currentPersuasionTask.Options.Count > 2)
			{
				currentPersuasionTask.Options[2].BlockTheOption(true);
			}
		}

		private void conversation_lord_recruit_4_persuade_option_on_consequence()
		{
			PersuasionTask currentPersuasionTask = this.GetCurrentPersuasionTask();
			if (currentPersuasionTask.Options.Count > 3)
			{
				currentPersuasionTask.Options[3].BlockTheOption(true);
			}
		}

		private bool DefectionPersuasionOption1ClickableOnCondition1(out TextObject hintText)
		{
			hintText = TextObject.Empty;
			PersuasionTask currentPersuasionTask = this.GetCurrentPersuasionTask();
			if (currentPersuasionTask.Options.Count > 0)
			{
				return !currentPersuasionTask.Options.ElementAt(0).IsBlocked;
			}
			hintText = new TextObject("{=9ACJsI6S}Blocked", null);
			return false;
		}

		private bool DefectionPersuasionOption2ClickableOnCondition2(out TextObject hintText)
		{
			hintText = TextObject.Empty;
			PersuasionTask currentPersuasionTask = this.GetCurrentPersuasionTask();
			if (currentPersuasionTask.Options.Count > 1)
			{
				return !currentPersuasionTask.Options.ElementAt(1).IsBlocked;
			}
			hintText = new TextObject("{=9ACJsI6S}Blocked", null);
			return false;
		}

		private bool DefectionPersuasionOption3ClickableOnCondition3(out TextObject hintText)
		{
			hintText = TextObject.Empty;
			PersuasionTask currentPersuasionTask = this.GetCurrentPersuasionTask();
			if (currentPersuasionTask.Options.Count > 2)
			{
				return !currentPersuasionTask.Options.ElementAt(2).IsBlocked;
			}
			hintText = new TextObject("{=9ACJsI6S}Blocked", null);
			return false;
		}

		private bool DefectionPersuasionOption4ClickableOnCondition4(out TextObject hintText)
		{
			hintText = TextObject.Empty;
			PersuasionTask currentPersuasionTask = this.GetCurrentPersuasionTask();
			if (currentPersuasionTask.Options.Count > 3)
			{
				return !currentPersuasionTask.Options.ElementAt(3).IsBlocked;
			}
			hintText = new TextObject("{=9ACJsI6S}Blocked", null);
			return false;
		}

		private bool conversation_lord_recruit_2_persuade_option_on_condition()
		{
			PersuasionTask currentPersuasionTask = this.GetCurrentPersuasionTask();
			if (currentPersuasionTask.Options.Count > 1)
			{
				TextObject textObject = new TextObject("{=bSo9hKwr}{PERSUASION_OPTION_LINE} {SUCCESS_CHANCE}", null);
				textObject.SetTextVariable("SUCCESS_CHANCE", PersuasionHelper.ShowSuccess(currentPersuasionTask.Options.ElementAt(1), true));
				textObject.SetTextVariable("PERSUASION_OPTION_LINE", currentPersuasionTask.Options.ElementAt(1).Line);
				MBTextManager.SetTextVariable("DEFECTION_PERSUADE_ATTEMPT_2", textObject, false);
				return true;
			}
			return false;
		}

		private bool conversation_lord_recruit_3_persuade_option_on_condition()
		{
			PersuasionTask currentPersuasionTask = this.GetCurrentPersuasionTask();
			if (currentPersuasionTask.Options.Count > 2)
			{
				TextObject textObject = new TextObject("{=bSo9hKwr}{PERSUASION_OPTION_LINE} {SUCCESS_CHANCE}", null);
				textObject.SetTextVariable("SUCCESS_CHANCE", PersuasionHelper.ShowSuccess(currentPersuasionTask.Options.ElementAt(2), true));
				textObject.SetTextVariable("PERSUASION_OPTION_LINE", currentPersuasionTask.Options.ElementAt(2).Line);
				MBTextManager.SetTextVariable("DEFECTION_PERSUADE_ATTEMPT_3", textObject, false);
				return true;
			}
			return false;
		}

		private bool conversation_lord_recruit_4_persuade_option_on_condition()
		{
			PersuasionTask currentPersuasionTask = this.GetCurrentPersuasionTask();
			if (currentPersuasionTask.Options.Count > 3)
			{
				TextObject textObject = new TextObject("{=bSo9hKwr}{PERSUASION_OPTION_LINE} {SUCCESS_CHANCE}", null);
				textObject.SetTextVariable("SUCCESS_CHANCE", PersuasionHelper.ShowSuccess(currentPersuasionTask.Options.ElementAt(3), true));
				textObject.SetTextVariable("PERSUASION_OPTION_LINE", currentPersuasionTask.Options.ElementAt(3).Line);
				MBTextManager.SetTextVariable("DEFECTION_PERSUADE_ATTEMPT_4", textObject, false);
				return true;
			}
			return false;
		}

		private PersuasionOptionArgs SetupDefectionPersuasionOption1()
		{
			return this.GetCurrentPersuasionTask().Options.ElementAt(0);
		}

		private PersuasionOptionArgs SetupDefectionPersuasionOption2()
		{
			return this.GetCurrentPersuasionTask().Options.ElementAt(1);
		}

		private PersuasionOptionArgs SetupDefectionPersuasionOption3()
		{
			return this.GetCurrentPersuasionTask().Options.ElementAt(2);
		}

		private PersuasionOptionArgs SetupDefectionPersuasionOption4()
		{
			return this.GetCurrentPersuasionTask().Options.ElementAt(3);
		}

		private bool conversation_player_start_defection_with_prisoner_on_condition()
		{
			if (Hero.OneToOneConversationHero != null && Clan.PlayerClan.Kingdom != null && Hero.MainHero.IsKingdomLeader)
			{
				Clan clan = Hero.OneToOneConversationHero.Clan;
				if (((clan != null) ? clan.Leader : null) == Hero.OneToOneConversationHero && Hero.OneToOneConversationHero.HeroState == Hero.CharacterStates.Prisoner && Campaign.Current.CurrentConversationContext != ConversationContext.CapturedLord && Campaign.Current.CurrentConversationContext != ConversationContext.FreedHero && Hero.OneToOneConversationHero.Clan != Hero.OneToOneConversationHero.MapFaction.Leader.Clan)
				{
					return (Hero.OneToOneConversationHero.PartyBelongedToAsPrisoner != null && Hero.OneToOneConversationHero.PartyBelongedToAsPrisoner == PartyBase.MainParty) || (Hero.OneToOneConversationHero.CurrentSettlement != null && Hero.OneToOneConversationHero.CurrentSettlement.OwnerClan == Clan.PlayerClan);
				}
			}
			return false;
		}

		private bool conversation_lord_persuade_option_reaction_pre_reject_on_condition()
		{
			return Hero.OneToOneConversationHero.Clan.Leader == Hero.OneToOneConversationHero && (float)new JoinKingdomAsClanBarterable(Hero.OneToOneConversationHero, (Kingdom)Hero.MainHero.MapFaction, false).GetValueForFaction(Hero.OneToOneConversationHero.Clan) < -MathF.Min(2000000f, MathF.Max(500000f, 250000f + (float)Hero.MainHero.Gold / 3f));
		}

		private bool conversation_lord_persuade_option_reaction_on_condition()
		{
			PersuasionOptionResult item = ConversationManager.GetPersuasionChosenOptions().Last<Tuple<PersuasionOptionArgs, PersuasionOptionResult>>().Item2;
			PersuasionTask currentPersuasionTask = this.GetCurrentPersuasionTask();
			if (item == PersuasionOptionResult.Failure)
			{
				MBTextManager.SetTextVariable("IMMEDIATE_FAILURE_LINE", ((currentPersuasionTask != null) ? currentPersuasionTask.ImmediateFailLine : null) ?? TextObject.Empty, false);
				MBTextManager.SetTextVariable("PERSUASION_REACTION", "{=18xOURG4}Hmm.. No... {IMMEDIATE_FAILURE_LINE}", false);
			}
			else
			{
				if (item == PersuasionOptionResult.CriticalFailure)
				{
					MBTextManager.SetTextVariable("PERSUASION_REACTION", "{=Lj5Lghww}What? No...", false);
					TextObject textObject = ((currentPersuasionTask != null) ? currentPersuasionTask.ImmediateFailLine : null) ?? TextObject.Empty;
					MBTextManager.SetTextVariable("IMMEDIATE_FAILURE_LINE", textObject, false);
					MBTextManager.SetTextVariable("PERSUASION_REACTION", "{=18xOURG4}Hmm.. No... {IMMEDIATE_FAILURE_LINE}", false);
					using (List<PersuasionTask>.Enumerator enumerator = this._allReservations.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							PersuasionTask persuasionTask = enumerator.Current;
							persuasionTask.BlockAllOptions();
						}
						return true;
					}
				}
				MBTextManager.SetTextVariable("PERSUASION_REACTION", PersuasionHelper.GetDefaultPersuasionOptionReaction(item), false);
			}
			return true;
		}

		private void conversation_lord_persuade_option_reaction_on_consequence()
		{
			Tuple<PersuasionOptionArgs, PersuasionOptionResult> tuple = ConversationManager.GetPersuasionChosenOptions().Last<Tuple<PersuasionOptionArgs, PersuasionOptionResult>>();
			float difficulty = Campaign.Current.Models.PersuasionModel.GetDifficulty(PersuasionDifficulty.Medium);
			float num;
			float num2;
			Campaign.Current.Models.PersuasionModel.GetEffectChances(tuple.Item1, out num, out num2, difficulty);
			PersuasionTask persuasionTask = this.FindTaskOfOption(tuple.Item1);
			persuasionTask.ApplyEffects(num, num2);
			PersuasionAttempt persuasionAttempt = new PersuasionAttempt(Hero.OneToOneConversationHero, CampaignTime.Now, tuple.Item1, tuple.Item2, persuasionTask.ReservationType);
			this._previousDefectionPersuasionAttempts.Add(persuasionAttempt);
		}

		private PersuasionTask FindTaskOfOption(PersuasionOptionArgs optionChosenWithLine)
		{
			foreach (PersuasionTask persuasionTask in this._allReservations)
			{
				using (List<PersuasionOptionArgs>.Enumerator enumerator2 = persuasionTask.Options.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						if (enumerator2.Current.Line == optionChosenWithLine.Line)
						{
							return persuasionTask;
						}
					}
				}
			}
			return null;
		}

		private void conversation_on_end_persuasion_on_consequence()
		{
			this._allReservations = null;
			ConversationManager.EndPersuasion();
		}

		public bool conversation_lord_player_has_failed_in_defection_on_condition()
		{
			if (this.GetCurrentPersuasionTask().Options.All((PersuasionOptionArgs x) => x.IsBlocked) && !ConversationManager.GetPersuasionProgressSatisfied())
			{
				PersuasionTask anyFailedPersuasionTask = this.GetAnyFailedPersuasionTask();
				if (anyFailedPersuasionTask != null)
				{
					MBTextManager.SetTextVariable("FAILED_PERSUASION_LINE", anyFailedPersuasionTask.FinalFailLine, false);
				}
				return true;
			}
			return false;
		}

		public bool conversation_lord_recruit_check_if_reservations_met_on_condition()
		{
			PersuasionTask currentPersuasionTask = this.GetCurrentPersuasionTask();
			if (currentPersuasionTask == this._allReservations[this._allReservations.Count - 1])
			{
				if (currentPersuasionTask.Options.All((PersuasionOptionArgs x) => x.IsBlocked))
				{
					return false;
				}
			}
			if (!ConversationManager.GetPersuasionProgressSatisfied())
			{
				MBTextManager.SetTextVariable("PERSUASION_TASK_LINE", currentPersuasionTask.SpokenLine, false);
				return true;
			}
			return false;
		}

		public bool conversation_lord_check_if_ready_to_join_faction_without_barter_on_condition()
		{
			return false;
		}

		public void conversation_lord_defect_to_clan_without_barter_on_consequence()
		{
			Kingdom kingdom = Hero.MainHero.Clan.Kingdom;
			new JoinKingdomAsClanBarterable(Hero.OneToOneConversationHero, kingdom, true).Apply();
			this.defection_successful_on_consequence();
			ConversationManager.EndPersuasion();
		}

		public bool conversation_lord_check_if_ready_to_join_faction_with_barter_on_condition()
		{
			return ConversationManager.GetPersuasionProgressSatisfied();
		}

		private List<PersuasionTask> GetPersuasionTasksForDefection(Hero forLord, Hero newLiege)
		{
			Hero currentLiege = forLord.MapFaction.Leader;
			LogEntry logEntry = null;
			LogEntry logEntry2 = null;
			LogEntry logEntry3 = null;
			List<LogEntry> gameActionLogs = Campaign.Current.LogEntryHistory.GameActionLogs;
			bool flag = forLord.GetTraitLevel(DefaultTraits.Honor) + forLord.GetTraitLevel(DefaultTraits.Mercy) < 0;
			foreach (LogEntry logEntry4 in gameActionLogs)
			{
				if (logEntry4.GetValueAsPoliticsAbuseOfPower(forLord, currentLiege) > 0 && (logEntry == null || logEntry4.GetValueAsPoliticsAbuseOfPower(forLord, currentLiege) > logEntry.GetValueAsPoliticsAbuseOfPower(forLord, currentLiege)))
				{
					logEntry = logEntry4;
				}
				if (logEntry4.GetValueAsPoliticsShowedWeakness(forLord, currentLiege) > 0 && (logEntry2 == null || logEntry4.GetValueAsPoliticsShowedWeakness(forLord, currentLiege) > logEntry2.GetValueAsPoliticsSlightedClan(forLord, currentLiege)))
				{
					logEntry2 = logEntry4;
				}
				if (logEntry4.GetValueAsPoliticsSlightedClan(forLord, currentLiege) > 0 && (logEntry3 == null || logEntry4.GetValueAsPoliticsSlightedClan(forLord, currentLiege) > logEntry3.GetValueAsPoliticsSlightedClan(forLord, currentLiege)))
				{
					logEntry3 = logEntry4;
				}
			}
			List<PersuasionTask> list = new List<PersuasionTask>();
			StringHelpers.SetCharacterProperties("CURRENT_LIEGE", forLord.MapFaction.Leader.CharacterObject, null, false);
			StringHelpers.SetCharacterProperties("NEW_LIEGE", newLiege.CharacterObject, null, false);
			PersuasionTask persuasionTask = new PersuasionTask(0);
			persuasionTask.SpokenLine = new TextObject("{=PtWQ789Z}I'm not sure I trust you people.", null);
			persuasionTask.ImmediateFailLine = new TextObject("{=u3eGQRn8}I am not entirely comfortable discussing this with you.", null);
			persuasionTask.FinalFailLine = new TextObject("{=yxeyl4LW}I am simply not comfortable discussing this with you.", null);
			float unmodifiedClanLeaderRelationshipWithPlayer = Hero.OneToOneConversationHero.GetUnmodifiedClanLeaderRelationshipWithPlayer();
			PersuasionArgumentStrength persuasionArgumentStrength;
			if (unmodifiedClanLeaderRelationshipWithPlayer <= -10f)
			{
				persuasionTask.SpokenLine = new TextObject("{=GtIpsut6}I don't even like you. You expect me to discuss something like this with you?", null);
				persuasionArgumentStrength = PersuasionArgumentStrength.VeryHard;
			}
			else if (unmodifiedClanLeaderRelationshipWithPlayer <= 0f)
			{
				persuasionTask.SpokenLine = new TextObject("{=Owa28Kpr}I barely know you, and you're asking me to talk treason?", null);
				persuasionArgumentStrength = PersuasionArgumentStrength.Hard;
			}
			else if (unmodifiedClanLeaderRelationshipWithPlayer >= 20f)
			{
				persuasionTask.SpokenLine = new TextObject("{=HM7auUMA}You are my friend, but even so, this is a risky conversation to have.", null);
				persuasionArgumentStrength = PersuasionArgumentStrength.Easy;
			}
			else
			{
				persuasionTask.SpokenLine = new TextObject("{=arBQHbWv}I'm not sure I know you well enough to discuss something like this.", null);
				persuasionArgumentStrength = PersuasionArgumentStrength.Normal;
			}
			if (unmodifiedClanLeaderRelationshipWithPlayer >= 20f)
			{
				PersuasionOptionArgs persuasionOptionArgs = new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Honor, TraitEffect.Positive, persuasionArgumentStrength, true, new TextObject("{=qsnh0KGS}As your friend, I give you my word that I won't breathe a word to anyone else about this conversation.", null), null, false, true, false);
				persuasionTask.AddOptionToTask(persuasionOptionArgs);
			}
			else if (forLord.GetTraitLevel(DefaultTraits.Honor) > 0)
			{
				TextObject textObject = new TextObject("{=yZWBDAG0}You are known as a {?LORD.GENDER}woman{?}man{\\?} of honor. You may know me as one as well.", null);
				PersuasionOptionArgs persuasionOptionArgs2 = new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Honor, TraitEffect.Positive, persuasionArgumentStrength, true, textObject, null, false, true, false);
				StringHelpers.SetCharacterProperties("LORD", forLord.CharacterObject, textObject, false);
				persuasionTask.AddOptionToTask(persuasionOptionArgs2);
			}
			else if (forLord.GetTraitLevel(DefaultTraits.Honor) == 0)
			{
				TextObject textObject2 = new TextObject("{=0cMibkQO}You may know me as a {?PLAYER.GENDER}woman{?}man{\\?} of honor.", null);
				PersuasionOptionArgs persuasionOptionArgs3 = new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Honor, TraitEffect.Positive, persuasionArgumentStrength, true, textObject2, null, false, true, false);
				StringHelpers.SetCharacterProperties("PLAYER", CharacterObject.PlayerCharacter, textObject2, false);
				persuasionTask.AddOptionToTask(persuasionOptionArgs3);
			}
			if (Hero.OneToOneConversationHero.GetTraitLevel(DefaultTraits.Mercy) > 0 && unmodifiedClanLeaderRelationshipWithPlayer < 20f)
			{
				PersuasionOptionArgs persuasionOptionArgs4 = new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Mercy, TraitEffect.Positive, persuasionArgumentStrength, false, new TextObject("{=ch6zCk2w}You know me as someone who seeks to avoid bloodshed.", null), null, false, true, false);
				persuasionTask.AddOptionToTask(persuasionOptionArgs4);
			}
			else if (Hero.OneToOneConversationHero.GetTraitLevel(DefaultTraits.Valor) > 0 && unmodifiedClanLeaderRelationshipWithPlayer < 20f)
			{
				PersuasionOptionArgs persuasionOptionArgs5 = new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Valor, TraitEffect.Positive, persuasionArgumentStrength - 1, true, new TextObject("{=I5f6Xg3a}You must have heard of my deeds. I speak to you as one warrior to another.", null), null, false, true, false);
				persuasionTask.AddOptionToTask(persuasionOptionArgs5);
			}
			if (unmodifiedClanLeaderRelationshipWithPlayer >= 20f)
			{
				PersuasionOptionArgs persuasionOptionArgs6 = new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Calculating, TraitEffect.Positive, persuasionArgumentStrength, false, new TextObject("{=8wUfQc4W}You know me. I'll be careful not to get this get back to the wrong ears.", null), null, false, true, false);
				persuasionTask.AddOptionToTask(persuasionOptionArgs6);
			}
			else
			{
				PersuasionOptionArgs persuasionOptionArgs7 = new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Calculating, TraitEffect.Positive, persuasionArgumentStrength - 1, false, new TextObject("{=VA8BTMBR}You must know of my reputation. You know that it's not in my interest to betray your trust.", null), null, false, true, false);
				persuasionTask.AddOptionToTask(persuasionOptionArgs7);
			}
			list.Add(persuasionTask);
			PersuasionTask persuasionTask2 = new PersuasionTask(1);
			persuasionTask2.ImmediateFailLine = new TextObject("{=VnECJbmq}That is not enough.", null);
			persuasionTask2.FinalFailLine = new TextObject("{=KbQQV5rI}My oath is my oath. I cannot break it. (Oath persuasion failed.)", null);
			PersuasionArgumentStrength persuasionArgumentStrength2;
			if (forLord.IsEnemy(currentLiege) && logEntry != null)
			{
				persuasionTask2.SpokenLine = new TextObject("{=QY55NgWl}I gave an oath to {CURRENT_LIEGE.LINK} - though I'm not sure he's always kept his oath to me.", null);
				persuasionArgumentStrength2 = PersuasionArgumentStrength.Easy;
			}
			else if (forLord.GetTraitLevel(DefaultTraits.Honor) > 0)
			{
				persuasionTask2.SpokenLine = new TextObject("{=4HWFvX8M}I gave an oath to my liege. To break it, even for a good reason, would be a great stain on my honor.", null);
				persuasionArgumentStrength2 = PersuasionArgumentStrength.VeryHard;
			}
			else if (flag && logEntry2 != null)
			{
				persuasionTask2.SpokenLine = new TextObject("{=wOKF17ta}I gave an oath to {CURRENT_LIEGE.LINK} - though no oath binds me to serve a weak leader who'll take us all down.", null);
				persuasionArgumentStrength2 = PersuasionArgumentStrength.Easy;
			}
			else if (forLord.GetTraitLevel(DefaultTraits.Mercy) > 0 && newLiege.GetTraitLevel(DefaultTraits.Mercy) < 0)
			{
				persuasionTask2.SpokenLine = new TextObject("{=GlRZN1J5}I gave an oath to {CURRENT_LIEGE.LINK} - though no oath binds me to serve a weak leader who is too softhearted to rule.", null);
				persuasionArgumentStrength2 = PersuasionArgumentStrength.Easy;
			}
			else if ((forLord.GetTraitLevel(DefaultTraits.Egalitarian) > 0 && newLiege.GetTraitLevel(DefaultTraits.Oligarchic) > 0) || newLiege.GetTraitLevel(DefaultTraits.Authoritarian) > 0)
			{
				persuasionTask2.SpokenLine = new TextObject("{=CymOFgzv}I gave an oath to {CURRENT_LIEGE.LINK} - but {?LORD.GENDER}her{?}his{\\?} disregard for the common people of this realm does give me pause.", null);
				persuasionArgumentStrength2 = PersuasionArgumentStrength.Easy;
			}
			else if ((forLord.GetTraitLevel(DefaultTraits.Oligarchic) > 0 && newLiege.GetTraitLevel(DefaultTraits.Egalitarian) > 0) || newLiege.GetTraitLevel(DefaultTraits.Authoritarian) > 0)
			{
				persuasionTask2.SpokenLine = new TextObject("{=EYQI9HJv}I gave an oath to {CURRENT_LIEGE.LINK} - but {?LORD.GENDER}her{?}his{\\?} disregard for the laws of this realm does give me pause.", null);
				persuasionArgumentStrength2 = PersuasionArgumentStrength.Easy;
			}
			else
			{
				persuasionTask2.SpokenLine = new TextObject("{=VJoCtAvz}I gave an oath to my liege.", null);
				persuasionArgumentStrength2 = PersuasionArgumentStrength.Normal;
			}
			if (currentLiege.GetTraitLevel(DefaultTraits.Honor) + currentLiege.GetTraitLevel(DefaultTraits.Mercy) < 0)
			{
				PersuasionOptionArgs persuasionOptionArgs8 = new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Mercy, TraitEffect.Positive, PersuasionArgumentStrength.Normal, false, new TextObject("{=ITqVF9i4}You know {CURRENT_LIEGE.NAME} asks you to do dishonorable things, and no oath binds you to doing evil.", null), null, false, true, false);
				persuasionTask2.AddOptionToTask(persuasionOptionArgs8);
			}
			if (currentLiege.GetTraitLevel(DefaultTraits.Honor) < 0)
			{
				PersuasionOptionArgs persuasionOptionArgs9 = new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Honor, TraitEffect.Positive, persuasionArgumentStrength2 + 1, false, new TextObject("{=5lq4HNU5}{CURRENT_LIEGE.NAME} is not known for keeping his word.", null), null, false, true, false);
				persuasionTask2.AddOptionToTask(persuasionOptionArgs9);
			}
			if (logEntry != null || currentLiege.GetTraitLevel(DefaultTraits.Honor) <= 0)
			{
				PersuasionOptionArgs persuasionOptionArgs10 = new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Honor, TraitEffect.Positive, persuasionArgumentStrength2 + 1, false, new TextObject("{=nQStXojH}If {?CURRENT_LIEGE.GENDER}she{?}he{\\?} ever violated {?CURRENT_LIEGE.GENDER}her{?}his{\\?} oath to you, it absolves you of your duty to {?CURRENT_LIEGE.GENDER}her{?}him{\\?}.", null), null, false, true, false);
				persuasionTask2.AddOptionToTask(persuasionOptionArgs10);
			}
			PersuasionOptionArgs persuasionOptionArgs11 = new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Calculating, TraitEffect.Positive, PersuasionArgumentStrength.Hard, false, new TextObject("{=lhnuawq3}You know very well that in politics oaths are easily made, and just as easily broken.", null), null, false, true, false);
			persuasionTask2.AddOptionToTask(persuasionOptionArgs11);
			list.Add(persuasionTask2);
			PersuasionTask persuasionTask3 = new PersuasionTask(2);
			persuasionTask3.FinalFailLine = new TextObject("{=5E2bIcGb}I will not betray my liege. (Loyalty persuasion failed.)", null);
			persuasionTask3.SpokenLine = new TextObject("{=ttpan5jp}{CURRENT_LIEGE.LINK} and I have been through a great deal together.", null);
			PersuasionArgumentStrength persuasionArgumentStrength3;
			if (logEntry3 != null)
			{
				persuasionTask3.SpokenLine = new TextObject("{=IoaAvgRD}You know {NEW_LIEGE.LINK} have had our differences. You expect me to change sides for him?", null);
				persuasionArgumentStrength3 = PersuasionArgumentStrength.Hard;
			}
			else if (forLord.IsEnemy(newLiege))
			{
				persuasionTask3.SpokenLine = new TextObject("{=awaFsZ5l}I have always stood by {CURRENT_LIEGE.LINK}. Whether {CURRENT_LIEGE.LINK} has stood by me or not is another question...", null);
				persuasionArgumentStrength3 = PersuasionArgumentStrength.VeryEasy;
			}
			else if (forLord.IsFriend(currentLiege))
			{
				persuasionTask3.SpokenLine = new TextObject("{=PGkFvo77}{CURRENT_LIEGE.LINK} is a friend of mine. I cannot imagine betraying {?CURRENT_LIEGE.GENDER}her{?}him{\\?}.", null);
				persuasionArgumentStrength3 = PersuasionArgumentStrength.VeryHard;
			}
			else if ((forLord.GetTraitLevel(DefaultTraits.Egalitarian) > 0 && currentLiege.GetTraitLevel(DefaultTraits.Egalitarian) > 0) || (forLord.GetTraitLevel(DefaultTraits.Oligarchic) > 0 && currentLiege.GetTraitLevel(DefaultTraits.Oligarchic) > 0) || (forLord.GetTraitLevel(DefaultTraits.Authoritarian) > 0 && currentLiege.GetTraitLevel(DefaultTraits.Authoritarian) > 0))
			{
				persuasionTask3.SpokenLine = new TextObject("{=Xlb7Xxyl}{CURRENT_LIEGE.LINK} stands for what I believe in.", null);
				persuasionArgumentStrength3 = PersuasionArgumentStrength.Hard;
			}
			else if ((forLord.GetTraitLevel(DefaultTraits.Mercy) > 0 && currentLiege.GetTraitLevel(DefaultTraits.Mercy) > 0) || (forLord.GetTraitLevel(DefaultTraits.Honor) > 0 && currentLiege.GetTraitLevel(DefaultTraits.Honor) > 0))
			{
				persuasionTask3.SpokenLine = new TextObject("{=LtDqAAk4}I consider {CURRENT_LIEGE.LINK} to be an upright ruler. {NEW_LIEGE.LINK} is not.", null);
				persuasionArgumentStrength3 = PersuasionArgumentStrength.Hard;
			}
			else
			{
				persuasionArgumentStrength3 = PersuasionArgumentStrength.Normal;
			}
			CultureObject cultureObject = Hero.MainHero.Culture;
			if (Hero.MainHero.Clan.Kingdom != null)
			{
				cultureObject = Hero.MainHero.Clan.Kingdom.Culture;
			}
			if (forLord.Culture != cultureObject && persuasionArgumentStrength3 >= PersuasionArgumentStrength.Normal)
			{
				TextObject textObject3 = new TextObject("{=6lbjddM8}{PRIOR_LINE} We have been together in many wars. Including many against your {?IS_SAME_CULTURE}people{?}allies{\\?}, the {ETHNIC_TERM}, I should add.", null);
				textObject3.SetTextVariable("PRIOR_LINE", persuasionTask3.SpokenLine);
				textObject3.SetTextVariable("ETHNIC_TERM", GameTexts.FindText("str_neutral_term_for_culture", cultureObject.StringId));
				textObject3.SetTextVariable("IS_SAME_CULTURE", (Hero.MainHero.Culture == cultureObject) ? 1 : 0);
				persuasionTask3.SpokenLine = textObject3;
			}
			if (currentLiege.IsEnemy(forLord))
			{
				PersuasionOptionArgs persuasionOptionArgs12 = new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Generosity, TraitEffect.Positive, persuasionArgumentStrength3, false, new TextObject("{=z5cLVzC8}It's well known that you and {CURRENT_LIEGE.NAME} loathe each other.", null), null, false, true, false);
				persuasionTask3.AddOptionToTask(persuasionOptionArgs12);
			}
			else if (currentLiege.GetTraitLevel(DefaultTraits.Generosity) < 0)
			{
				PersuasionOptionArgs persuasionOptionArgs13 = new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Generosity, TraitEffect.Positive, persuasionArgumentStrength3, false, new TextObject("{=ZzR9VTU0}{CURRENT_LIEGE.NAME} isn't known for his sense of loyalty. Why do you feel so much to him?", null), null, false, true, false);
				persuasionTask3.AddOptionToTask(persuasionOptionArgs13);
			}
			else
			{
				PersuasionOptionArgs persuasionOptionArgs14 = new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Valor, TraitEffect.Positive, persuasionArgumentStrength3 - 1, false, new TextObject("{=abkmGhLH}Has {CURRENT_LIEGE.NAME} really repaid you for your service as you deserve?", null), null, false, true, false);
				persuasionTask3.AddOptionToTask(persuasionOptionArgs14);
			}
			if (HeroHelper.NPCPoliticalDifferencesWithNPC(forLord, newLiege) && !HeroHelper.NPCPoliticalDifferencesWithNPC(forLord, currentLiege))
			{
				PersuasionOptionArgs persuasionOptionArgs15 = new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Valor, TraitEffect.Positive, persuasionArgumentStrength3 + 1, false, new TextObject("{=OdS0e6Sb}{NEW_LIEGE.NAME} stands up for what you believe in, while {CURRENT_LIEGE.NAME} does not.", null), null, false, true, false);
				persuasionTask3.AddOptionToTask(persuasionOptionArgs15);
			}
			if (forLord.GetTraitLevel(DefaultTraits.Mercy) > 0 && currentLiege.GetTraitLevel(DefaultTraits.Mercy) < 0 && newLiege.GetTraitLevel(DefaultTraits.Mercy) >= 0)
			{
				PersuasionOptionArgs persuasionOptionArgs16 = new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Mercy, TraitEffect.Positive, persuasionArgumentStrength3, true, new TextObject("{=9cZeHcAC}The cruelty of {CURRENT_LIEGE.NAME} is legendary. Who cares what he stands for if the realm is drenched in blood?", null), null, false, true, false);
				persuasionTask3.AddOptionToTask(persuasionOptionArgs16);
			}
			PersuasionOptionArgs persuasionOptionArgs17 = new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Calculating, TraitEffect.Positive, persuasionArgumentStrength3, false, new TextObject("{=y3xguaCc}Put your interests and the good of the realm first. There's too much at stake for that.", null), null, false, true, false);
			persuasionTask3.AddOptionToTask(persuasionOptionArgs17);
			list.Add(persuasionTask3);
			PersuasionTask persuasionTask4 = new PersuasionTask(4);
			persuasionTask4.FinalFailLine = new TextObject("{=2P9mMbrq}It is not in my interest to change sides. (Self-interest persuasion failed.)", null);
			float renown = newLiege.Clan.Renown;
			List<Settlement> list2 = Settlement.All.Where((Settlement x) => x.MapFaction == newLiege.MapFaction).ToList<Settlement>();
			List<Settlement> list3 = Settlement.All.Where((Settlement x) => x.MapFaction == currentLiege.MapFaction).ToList<Settlement>();
			PersuasionArgumentStrength persuasionArgumentStrength4;
			if (renown < 1000f && newLiege == Hero.MainHero)
			{
				persuasionTask4.SpokenLine = new TextObject("{=p2rTaKo8}You have no claim to the throne. Even in the unlikely case that others follow you, another usurper will just rise up to defeat you.", null);
				persuasionArgumentStrength4 = PersuasionArgumentStrength.VeryHard;
			}
			else if (list2.Count * 3 < list3.Count)
			{
				persuasionTask4.SpokenLine = new TextObject("{=A6E74QyR}You are badly outnumbered. A rebellion should at least have a chance of success.", null);
				persuasionArgumentStrength4 = PersuasionArgumentStrength.VeryHard;
			}
			else if (list2.Count < list3.Count)
			{
				persuasionTask4.SpokenLine = new TextObject("{=ZQa7tXdK}You are somewhat outnumbered. Even if I agree with you, it would be wise of me to wait.", null);
				persuasionArgumentStrength4 = PersuasionArgumentStrength.Hard;
			}
			else
			{
				persuasionTask4.SpokenLine = new TextObject("{=GEBRuVcZ}Why change sides now? Once one declares oneself a rebel, there is usually no going back.", null);
				persuasionArgumentStrength4 = PersuasionArgumentStrength.Normal;
			}
			if (forLord.GetTraitLevel(DefaultTraits.Valor) > 0)
			{
				PersuasionOptionArgs persuasionOptionArgs18 = new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Valor, TraitEffect.Positive, PersuasionArgumentStrength.Normal, true, new TextObject("{=XFzbzt3W}You are known for your valor. Fortune favors the bold. Together, we will win this war quickly.", null), null, false, true, false);
				persuasionTask4.AddOptionToTask(persuasionOptionArgs18);
			}
			else if (forLord.GetTraitLevel(DefaultTraits.Valor) > 0)
			{
				PersuasionOptionArgs persuasionOptionArgs19 = new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Valor, TraitEffect.Positive, PersuasionArgumentStrength.Hard, true, new TextObject("{=7QdKwOhY}Fortune favors the bold. With you with us, we will win this war quickly.", null), null, false, true, false);
				persuasionTask4.AddOptionToTask(persuasionOptionArgs19);
			}
			PersuasionOptionArgs persuasionOptionArgs20 = new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Calculating, TraitEffect.Positive, PersuasionArgumentStrength.Normal, false, new TextObject("{=dGkJi1yb}I have a strategy to win. And my strategies always work, eventually.", null), null, false, true, false);
			persuasionTask4.AddOptionToTask(persuasionOptionArgs20);
			if (forLord.GetTraitLevel(DefaultTraits.Honor) + forLord.GetTraitLevel(DefaultTraits.Valor) < 0)
			{
				PersuasionOptionArgs persuasionOptionArgs21 = new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Honor, TraitEffect.Positive, PersuasionArgumentStrength.Normal, false, new TextObject("{=IpnQP7A1}Better to die fighting for a just ruler than to live under an unjust one.", null), null, false, true, false);
				persuasionTask4.AddOptionToTask(persuasionOptionArgs21);
			}
			if (Hero.MainHero == newLiege)
			{
				PersuasionOptionArgs persuasionOptionArgs22 = new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Generosity, TraitEffect.Positive, persuasionArgumentStrength4, false, new TextObject("{=a37zTVVe}Believe me, I'll be generous to those who came to me early. Perhaps not as generous to those who came late.", null), null, false, true, false);
				persuasionTask4.AddOptionToTask(persuasionOptionArgs22);
			}
			else
			{
				PersuasionOptionArgs persuasionOptionArgs23 = new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Honor, TraitEffect.Positive, PersuasionArgumentStrength.Normal, false, new TextObject("{=aMICdOjq}{NEW_LIEGE.NAME} will be grateful to those who backed {?NEW_LIEGE.GENDER}her{?}him{\\?} before {?NEW_LIEGE.GENDER}her{?}his{\\?} victory was assured. Not so much after it's assured.", null), null, false, true, false);
				persuasionTask4.AddOptionToTask(persuasionOptionArgs23);
			}
			list.Add(persuasionTask4);
			return list;
		}

		public bool conversation_player_is_asking_to_recruit_enemy_on_condition()
		{
			if (Hero.OneToOneConversationHero != null && Hero.MainHero.Clan.Kingdom != null && !Hero.MainHero.Clan.IsUnderMercenaryService && Hero.OneToOneConversationHero.MapFaction != null && Hero.OneToOneConversationHero.MapFaction.IsKingdomFaction && Hero.OneToOneConversationHero.MapFaction != Hero.MainHero.MapFaction && Hero.OneToOneConversationHero.MapFaction.Leader != Hero.OneToOneConversationHero && Hero.OneToOneConversationHero.Clan != null && !Hero.OneToOneConversationHero.Clan.IsMinorFaction && !Hero.OneToOneConversationHero.IsPrisoner && FactionManager.IsAtWarAgainstFaction(Hero.OneToOneConversationHero.MapFaction, Hero.MainHero.MapFaction))
			{
				Hero.OneToOneConversationHero.MapFaction.Leader.SetTextVariables();
				MBTextManager.SetTextVariable("FACTION_NAME", Hero.MainHero.MapFaction.Name, false);
				return true;
			}
			return false;
		}

		public bool conversation_player_is_asking_to_recruit_neutral_on_condition()
		{
			if (Hero.OneToOneConversationHero != null && Hero.MainHero.Clan.Kingdom != null && !Hero.MainHero.Clan.IsUnderMercenaryService && Hero.OneToOneConversationHero.MapFaction != null && Hero.OneToOneConversationHero.MapFaction.IsKingdomFaction && Hero.OneToOneConversationHero.MapFaction != Hero.MainHero.MapFaction && !FactionManager.IsAtWarAgainstFaction(Hero.OneToOneConversationHero.MapFaction, Hero.MainHero.MapFaction) && Hero.OneToOneConversationHero.Clan != null && !Hero.OneToOneConversationHero.Clan.IsMinorFaction && !Hero.OneToOneConversationHero.IsPrisoner && Hero.OneToOneConversationHero != Hero.OneToOneConversationHero.MapFaction.Leader)
			{
				Hero.OneToOneConversationHero.MapFaction.Leader.SetTextVariables();
				MBTextManager.SetTextVariable("FACTION_NAME", Hero.MainHero.MapFaction.Name, false);
				return true;
			}
			return false;
		}

		private bool conversation_suggest_treason_on_condition()
		{
			return false;
		}

		public bool conversation_lord_from_ruling_clan_on_condition()
		{
			float num = 0f;
			this._allReservations = this.GetPersuasionTasksForDefection(Hero.OneToOneConversationHero, Hero.MainHero.MapFaction.Leader);
			foreach (PersuasionTask persuasionTask in this._allReservations)
			{
				foreach (PersuasionAttempt persuasionAttempt in this._previousDefectionPersuasionAttempts)
				{
					if (persuasionAttempt.Matches(Hero.OneToOneConversationHero, persuasionTask.ReservationType))
					{
						switch (persuasionAttempt.Result)
						{
						case PersuasionOptionResult.CriticalFailure:
							num -= this._criticalFailValue;
							break;
						case PersuasionOptionResult.Failure:
							num -= 0f;
							break;
						case PersuasionOptionResult.Success:
							num += this._successValue;
							break;
						case PersuasionOptionResult.CriticalSuccess:
							num += this._criticalSuccessValue;
							break;
						}
					}
				}
			}
			if (this._maximumScoreCap > num)
			{
				if (this._previousDefectionPersuasionAttempts.Any((PersuasionAttempt x) => x.PersuadedHero == Hero.OneToOneConversationHero))
				{
					MBTextManager.SetTextVariable("LIEGE_IS_RELATIVE", new TextObject("{=03lc5R2t}You have tried to persuade me before. I will not stand your words again.", null), false);
					return true;
				}
			}
			if (Hero.OneToOneConversationHero.Clan.IsMapFaction)
			{
				return false;
			}
			if (Hero.OneToOneConversationHero.Clan == Hero.OneToOneConversationHero.MapFaction.Leader.Clan)
			{
				TextObject textObject = new TextObject("{=jF4Nl8Au}{NPC_LIEGE.NAME}, {LIEGE_RELATIONSHIP}? Long may {?NPC_LIEGE.GENDER}she{?}he{\\?} live.", null);
				StringHelpers.SetCharacterProperties("NPC_LIEGE", Hero.OneToOneConversationHero.Clan.Leader.CharacterObject, textObject, false);
				textObject.SetTextVariable("LIEGE_RELATIONSHIP", ConversationHelper.HeroRefersToHero(Hero.OneToOneConversationHero, Hero.OneToOneConversationHero.Clan.Leader, true));
				MBTextManager.SetTextVariable("LIEGE_IS_RELATIVE", textObject, false);
				return true;
			}
			if (Hero.OneToOneConversationHero.PartyBelongedTo != null && Hero.OneToOneConversationHero.PartyBelongedTo.Army != null && (Hero.OneToOneConversationHero.PartyBelongedTo.Army.LeaderParty == Hero.OneToOneConversationHero.PartyBelongedTo || Hero.OneToOneConversationHero.PartyBelongedTo.AttachedTo != null))
			{
				MBTextManager.SetTextVariable("LIEGE_IS_RELATIVE", new TextObject("{=MalIalPA}I will not listen to such matters while I'm in an army.", null), false);
				return true;
			}
			return false;
		}

		public bool conversation_lord_redirects_to_clan_leader_on_condition()
		{
			if (Hero.OneToOneConversationHero.Clan.IsMapFaction)
			{
				return false;
			}
			MBTextManager.SetTextVariable("REDIRECT_HERO_RELATIONSHIP", ConversationHelper.HeroRefersToHero(Hero.OneToOneConversationHero, Hero.OneToOneConversationHero.Clan.Leader, true), false);
			return !Hero.OneToOneConversationHero.Clan.IsMapFaction && Hero.OneToOneConversationHero.Clan.Leader != Hero.OneToOneConversationHero;
		}

		private void persuasion_redierect_player_finish_on_consequece()
		{
		}

		private bool conversation_lord_can_recruit_on_condition()
		{
			if (Hero.MainHero.MapFaction.Leader == Hero.MainHero)
			{
				MBTextManager.SetTextVariable("RECRUIT_START", new TextObject("{=Fr7wzk97}I am the rightful ruler of this land. I would like your support.", null), false);
			}
			else if (Hero.MainHero.MapFaction == Hero.OneToOneConversationHero.MapFaction)
			{
				StringHelpers.SetCharacterProperties("CURRENT_LIEGE", Hero.OneToOneConversationHero.MapFaction.Leader.CharacterObject, null, false);
				MBTextManager.SetTextVariable("RECRUIT_START", "{=V7qF7uas}I should lead our people, not {CURRENT_LIEGE.NAME}.", false);
			}
			else
			{
				StringHelpers.SetCharacterProperties("NEW_LIEGE", Hero.MainHero.MapFaction.Leader.CharacterObject, null, false);
				MBTextManager.SetTextVariable("RECRUIT_START", new TextObject("{=UwPs3wmj}My liege {NEW_LIEGE.NAME} would welcome your support. Join us!", null), false);
			}
			return true;
		}

		private void start_lord_defection_persuasion_on_consequence()
		{
			Hero hero = Hero.MainHero.MapFaction.Leader;
			if (Hero.MainHero.MapFaction == Hero.OneToOneConversationHero.MapFaction)
			{
				hero = Hero.MainHero;
			}
			this._allReservations = this.GetPersuasionTasksForDefection(Hero.OneToOneConversationHero, hero);
			this._maximumScoreCap = (float)this._allReservations.Count * 1f;
			float num = 0f;
			foreach (PersuasionTask persuasionTask in this._allReservations)
			{
				foreach (PersuasionAttempt persuasionAttempt in this._previousDefectionPersuasionAttempts)
				{
					if (persuasionAttempt.Matches(Hero.OneToOneConversationHero, persuasionTask.ReservationType))
					{
						switch (persuasionAttempt.Result)
						{
						case PersuasionOptionResult.CriticalFailure:
							num -= this._criticalFailValue;
							break;
						case PersuasionOptionResult.Failure:
							num -= 0f;
							break;
						case PersuasionOptionResult.Success:
							num += this._successValue;
							break;
						case PersuasionOptionResult.CriticalSuccess:
							num += this._criticalSuccessValue;
							break;
						}
					}
				}
			}
			ConversationManager.StartPersuasion(this._maximumScoreCap, this._successValue, this._failValue, this._criticalSuccessValue, this._criticalFailValue, num, PersuasionDifficulty.Medium);
		}

		private void OnDailyTick()
		{
			this.RemoveOldAttempts();
		}

		private void conversation_clear_persuasion_on_consequence()
		{
			this.ClearPersuasion();
		}

		private void conversation_leave_faction_barter_consequence()
		{
			BarterManager instance = BarterManager.Instance;
			Hero mainHero = Hero.MainHero;
			Hero oneToOneConversationHero = Hero.OneToOneConversationHero;
			PartyBase mainParty = PartyBase.MainParty;
			MobileParty partyBelongedTo = Hero.OneToOneConversationHero.PartyBelongedTo;
			instance.StartBarterOffer(mainHero, oneToOneConversationHero, mainParty, (partyBelongedTo != null) ? partyBelongedTo.Party : null, null, new BarterManager.BarterContextInitializer(BarterManager.Instance.InitializeJoinFactionBarterContext), 0, false, new Barterable[]
			{
				new JoinKingdomAsClanBarterable(Hero.OneToOneConversationHero, Clan.PlayerClan.Kingdom, false)
			});
			this._allReservations = null;
			ConversationManager.EndPersuasion();
			if (Hero.OneToOneConversationHero.PartyBelongedTo != null && !Hero.OneToOneConversationHero.PartyBelongedTo.MapFaction.IsAtWarWith(MobileParty.MainParty.MapFaction))
			{
				PlayerEncounter.LeaveEncounter = true;
			}
		}

		private bool CanAttemptToPersuade(Hero targetHero, int reservationType)
		{
			foreach (PersuasionAttempt persuasionAttempt in this._previousDefectionPersuasionAttempts)
			{
				if (persuasionAttempt.Matches(targetHero, reservationType) && !persuasionAttempt.IsSuccesful() && persuasionAttempt.GameTime.ElapsedWeeksUntilNow < 1f)
				{
					return false;
				}
			}
			return true;
		}

		private void RemoveOldAttempts()
		{
			for (int i = this._previousDefectionPersuasionAttempts.Count - 1; i >= 0; i--)
			{
				if (this._previousDefectionPersuasionAttempts[i].GameTime.ElapsedYearsUntilNow > 1f)
				{
					this._previousDefectionPersuasionAttempts.RemoveAt(i);
				}
			}
		}

		private const PersuasionDifficulty _difficulty = PersuasionDifficulty.Medium;

		private List<PersuasionTask> _allReservations;

		[SaveableField(1)]
		private List<PersuasionAttempt> _previousDefectionPersuasionAttempts;

		private float _maximumScoreCap;

		private float _successValue = 1f;

		private float _criticalSuccessValue = 2f;

		private float _criticalFailValue = 2f;

		private float _failValue;

		public class LordDefectionCampaignBehaviorTypeDefiner : SaveableTypeDefiner
		{
			public LordDefectionCampaignBehaviorTypeDefiner()
				: base(100000)
			{
			}

			protected override void DefineEnumTypes()
			{
				base.AddEnumDefinition(typeof(LordDefectionCampaignBehavior.DefectionReservationType), 1, null);
			}
		}

		private enum DefectionReservationType
		{
			LordDefectionPlayerTrust,
			LordDefectionOathToLiege,
			LordDefectionLoyalty,
			LordDefectionPolicy,
			LordDefectionSelfinterest
		}
	}
}
