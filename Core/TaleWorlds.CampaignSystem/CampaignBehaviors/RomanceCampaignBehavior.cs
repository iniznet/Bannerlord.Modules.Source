using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.BarterSystem;
using TaleWorlds.CampaignSystem.BarterSystem.Barterables;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Conversation.Persuasion;
using TaleWorlds.CampaignSystem.Conversation.Tags;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	// Token: 0x020003CF RID: 975
	public class RomanceCampaignBehavior : CampaignBehaviorBase
	{
		// Token: 0x17000CEA RID: 3306
		// (get) Token: 0x06003A8F RID: 14991 RVA: 0x0010FCF1 File Offset: 0x0010DEF1
		private CampaignTime RomanceCourtshipAttemptCooldown
		{
			get
			{
				return CampaignTime.DaysFromNow(-1f);
			}
		}

		// Token: 0x06003A90 RID: 14992 RVA: 0x0010FCFD File Offset: 0x0010DEFD
		public RomanceCampaignBehavior()
		{
			this._previousRomancePersuasionAttempts = new List<PersuasionAttempt>();
		}

		// Token: 0x06003A91 RID: 14993 RVA: 0x0010FD34 File Offset: 0x0010DF34
		public override void RegisterEvents()
		{
			CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));
			CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, new Action(this.DailyTick));
			CampaignEvents.DailyTickClanEvent.AddNonSerializedListener(this, new Action<Clan>(this.DailyTickClan));
		}

		// Token: 0x06003A92 RID: 14994 RVA: 0x0010FD86 File Offset: 0x0010DF86
		private void DailyTickClan(Clan clan)
		{
			this.CheckNpcMarriages(clan);
		}

		// Token: 0x06003A93 RID: 14995 RVA: 0x0010FD90 File Offset: 0x0010DF90
		private void CheckNpcMarriages(Clan consideringClan)
		{
			if (this.IsClanSuitableForNpcMarriage(consideringClan))
			{
				MarriageModel marriageModel = Campaign.Current.Models.MarriageModel;
				foreach (Hero hero in consideringClan.Lords.ToList<Hero>())
				{
					if (hero.CanMarry())
					{
						Clan clan = Clan.All[MBRandom.RandomInt(Clan.All.Count)];
						if (this.IsClanSuitableForNpcMarriage(clan) && marriageModel.ShouldNpcMarriageBetweenClansBeAllowed(consideringClan, clan))
						{
							foreach (Hero hero2 in clan.Lords.ToList<Hero>())
							{
								float num = marriageModel.NpcCoupleMarriageChance(hero, hero2);
								if (num > 0f && MBRandom.RandomFloat < num)
								{
									bool flag = false;
									foreach (Romance.RomanticState romanticState in Romance.RomanticStateList)
									{
										if (romanticState.Level >= Romance.RomanceLevelEnum.MatchMadeByFamily && (romanticState.Person1 == hero || romanticState.Person2 == hero || romanticState.Person1 == hero2 || romanticState.Person2 == hero2))
										{
											flag = true;
											break;
										}
									}
									if (!flag)
									{
										MarriageAction.Apply(hero, hero2, true);
										return;
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06003A94 RID: 14996 RVA: 0x0010FF54 File Offset: 0x0010E154
		private bool IsClanSuitableForNpcMarriage(Clan clan)
		{
			return clan != Clan.PlayerClan && Campaign.Current.Models.MarriageModel.IsClanSuitableForMarriage(clan);
		}

		// Token: 0x06003A95 RID: 14997 RVA: 0x0010FF75 File Offset: 0x0010E175
		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<List<PersuasionAttempt>>("previousRomancePersuasionAttempts", ref this._previousRomancePersuasionAttempts);
		}

		// Token: 0x06003A96 RID: 14998 RVA: 0x0010FF89 File Offset: 0x0010E189
		public void OnSessionLaunched(CampaignGameStarter campaignGameStarter)
		{
			this.AddDialogs(campaignGameStarter);
		}

		// Token: 0x06003A97 RID: 14999 RVA: 0x0010FF94 File Offset: 0x0010E194
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

		// Token: 0x06003A98 RID: 15000 RVA: 0x00110030 File Offset: 0x0010E230
		private void RemoveUnneededPersuasionAttempts()
		{
			foreach (PersuasionAttempt persuasionAttempt in this._previousRomancePersuasionAttempts.ToList<PersuasionAttempt>())
			{
				if (persuasionAttempt.PersuadedHero.Spouse != null || !persuasionAttempt.PersuadedHero.IsAlive)
				{
					this._previousRomancePersuasionAttempts.Remove(persuasionAttempt);
				}
			}
		}

		// Token: 0x06003A99 RID: 15001 RVA: 0x001100A8 File Offset: 0x0010E2A8
		protected void AddDialogs(CampaignGameStarter starter)
		{
			starter.AddPlayerLine("lord_special_request_flirt", "lord_talk_speak_diplomacy_2", "lord_start_courtship_response", "{=!}{FLIRTATION_LINE}", new ConversationSentence.OnConditionDelegate(this.conversation_player_can_open_courtship_on_condition), new ConversationSentence.OnConsequenceDelegate(this.conversation_player_opens_courtship_on_consequence), 100, null, null);
			starter.AddPlayerLine("hero_romance_task_pt1", "hero_main_options", "hero_courtship_task_1_begin_reservations", "{=bHZyublA}So... I'm glad to have the chance to spend some time together.", new ConversationSentence.OnConditionDelegate(this.conversation_romance_at_stage_1_discussions_on_condition), new ConversationSentence.OnConsequenceDelegate(this.conversation_start_courtship_persuasion_pt1_on_consequence), 100, null, null);
			starter.AddPlayerLine("hero_romance_task_pt2", "hero_main_options", "hero_courtship_task_2_begin_reservations", "{=nGsQeTll}Perhaps we should discuss a future together...", new ConversationSentence.OnConditionDelegate(this.conversation_romance_at_stage_2_discussions_on_condition), new ConversationSentence.OnConsequenceDelegate(this.conversation_continue_courtship_stage_2_on_consequence), 100, null, null);
			starter.AddPlayerLine("hero_romance_task_pt3a", "hero_main_options", "hero_courtship_final_barter", "{=2aW6NC3Q}Let us discuss the final terms of our marriage.", new ConversationSentence.OnConditionDelegate(this.conversation_finalize_courtship_for_hero_on_condition), null, 100, null, null);
			starter.AddPlayerLine("hero_romance_task_pt3b", "hero_main_options", "hero_courtship_final_barter", "{=jd4qUGEA}I wish to discuss the final terms of my marriage with {COURTSHIP_PARTNER}.", new ConversationSentence.OnConditionDelegate(this.conversation_finalize_courtship_for_other_on_condition), null, 100, null, null);
			starter.AddPlayerLine("hero_romance_task_blocked", "hero_main_options", "hero_courtship_task_blocked", "{=OaRB1oVI}So... Earlier, we had discussed the possibility of marriage.", new ConversationSentence.OnConditionDelegate(this.conversation_romance_blocked_on_condition), null, 100, null, null);
			starter.AddDialogLine("hero_courtship_persuasion_fail", "hero_courtship_task_blocked", "lord_pretalk", "{=!}{ROMANCE_BLOCKED_REASON}", null, null, 100, null);
			starter.AddDialogLine("hero_courtship_persuasion_fail", "hero_courtship_task_1_begin_reservations", "lord_pretalk", "{=!}{FAILED_PERSUASION_LINE}", new ConversationSentence.OnConditionDelegate(this.conversation_lord_player_has_failed_in_courtship_on_condition), new ConversationSentence.OnConsequenceDelegate(this.conversation_fail_courtship_on_consequence), 100, null);
			starter.AddDialogLine("hero_courtship_persuasion_start", "hero_courtship_task_1_begin_reservations", "hero_courtship_task_1_next_reservation", "{=bW3ygxro}Yes, it's good to have a chance to get to know each other.", null, null, 100, null);
			starter.AddDialogLine("hero_courtship_persuasion_fail", "hero_courtship_task_1_next_reservation", "lord_pretalk", "{=!}{FAILED_PERSUASION_LINE}", new ConversationSentence.OnConditionDelegate(this.conversation_lord_player_has_failed_in_courtship_on_condition), new ConversationSentence.OnConsequenceDelegate(this.conversation_fail_courtship_on_consequence), 100, null);
			starter.AddDialogLine("hero_courtship_persuasion_attempt", "hero_courtship_task_1_next_reservation", "hero_courtship_argument", "{=!}{PERSUASION_TASK_LINE}", new ConversationSentence.OnConditionDelegate(this.conversation_check_if_unmet_reservation_on_condition), null, 100, null);
			starter.AddDialogLine("hero_courtship_persuasion_success", "hero_courtship_task_1_next_reservation", "lord_conclude_courtship_stage_1", "{=YcdQ1MWq}Well.. It seems we have a fair amount in common.", null, new ConversationSentence.OnConsequenceDelegate(this.conversation_courtship_stage_1_success_on_consequence), 100, null);
			starter.AddDialogLine("persuasion_leave_faction_npc_result_success_2", "lord_conclude_courtship_stage_1", "close_window", "{=SP7I61x2}Perhaps we can talk more when we meet again.", null, new ConversationSentence.OnConsequenceDelegate(this.courtship_conversation_leave_on_consequence), 100, null);
			starter.AddDialogLine("hero_courtship_persuasion_2_start", "hero_courtship_task_2_begin_reservations", "hero_courtship_task_2_next_reservation", "{=VNFKqpyV}Yes, well, I've been thinking about that.", null, null, 100, null);
			starter.AddDialogLine("hero_courtship_persuasion_2_fail", "hero_courtship_task_2_next_reservation", "lord_pretalk", "{=!}{FAILED_PERSUASION_LINE}", new ConversationSentence.OnConditionDelegate(this.conversation_lord_player_has_failed_in_courtship_on_condition), new ConversationSentence.OnConsequenceDelegate(this.conversation_fail_courtship_on_consequence), 100, null);
			starter.AddDialogLine("hero_courtship_persuasion_2_attempt", "hero_courtship_task_2_next_reservation", "hero_courtship_argument", "{=!}{PERSUASION_TASK_LINE}", new ConversationSentence.OnConditionDelegate(this.conversation_check_if_unmet_reservation_on_condition), null, 100, null);
			starter.AddDialogLine("hero_courtship_persuasion_2_success", "hero_courtship_task_2_next_reservation", "lord_conclude_courtship_stage_2", "{=xwS10c1b}Yes... I think I would be honored to accept your proposal.", null, new ConversationSentence.OnConsequenceDelegate(this.conversation_courtship_stage_2_success_on_consequence), 100, null);
			starter.AddDialogLine("persuasion_leave_faction_npc_result_success_2", "lord_conclude_courtship_stage_2", "close_window", "{=pvnY5Jwv}{CLAN_LEADER.LINK}, as head of our family, needs to give {?CLAN_LEADER.GENDER}her{?}his{\\?} blessing. There are usually financial arrangements to be made.", new ConversationSentence.OnConditionDelegate(this.courtship_hero_not_clan_leader_on_condition), new ConversationSentence.OnConsequenceDelegate(this.courtship_conversation_leave_on_consequence), 100, null);
			starter.AddDialogLine("persuasion_leave_faction_npc_result_success_2", "lord_conclude_courtship_stage_2", "close_window", "{=nnutwjOZ}We'll need to work out the details of how we divide our property.", null, new ConversationSentence.OnConsequenceDelegate(this.courtship_conversation_leave_on_consequence), 100, null);
			string text = "hero_courtship_argument_1";
			string text2 = "hero_courtship_argument";
			string text3 = "hero_courtship_reaction";
			string text4 = "{=!}{ROMANCE_PERSUADE_ATTEMPT_1}";
			ConversationSentence.OnConditionDelegate onConditionDelegate = new ConversationSentence.OnConditionDelegate(this.conversation_courtship_persuasion_option_1_on_condition);
			ConversationSentence.OnConsequenceDelegate onConsequenceDelegate = new ConversationSentence.OnConsequenceDelegate(this.conversation_romance_1_persuade_option_on_consequence);
			int num = 100;
			ConversationSentence.OnPersuasionOptionDelegate onPersuasionOptionDelegate = new ConversationSentence.OnPersuasionOptionDelegate(this.SetupCourtshipPersuasionOption1);
			starter.AddPlayerLine(text, text2, text3, text4, onConditionDelegate, onConsequenceDelegate, num, new ConversationSentence.OnClickableConditionDelegate(this.RomancePersuasionOption1ClickableOnCondition1), onPersuasionOptionDelegate);
			string text5 = "hero_courtship_argument_2";
			string text6 = "hero_courtship_argument";
			string text7 = "hero_courtship_reaction";
			string text8 = "{=!}{ROMANCE_PERSUADE_ATTEMPT_2}";
			ConversationSentence.OnConditionDelegate onConditionDelegate2 = new ConversationSentence.OnConditionDelegate(this.conversation_courtship_persuasion_option_2_on_condition);
			ConversationSentence.OnConsequenceDelegate onConsequenceDelegate2 = new ConversationSentence.OnConsequenceDelegate(this.conversation_romance_2_persuade_option_on_consequence);
			int num2 = 100;
			onPersuasionOptionDelegate = new ConversationSentence.OnPersuasionOptionDelegate(this.SetupCourtshipPersuasionOption2);
			starter.AddPlayerLine(text5, text6, text7, text8, onConditionDelegate2, onConsequenceDelegate2, num2, new ConversationSentence.OnClickableConditionDelegate(this.RomancePersuasionOption2ClickableOnCondition2), onPersuasionOptionDelegate);
			string text9 = "hero_courtship_argument_3";
			string text10 = "hero_courtship_argument";
			string text11 = "hero_courtship_reaction";
			string text12 = "{=!}{ROMANCE_PERSUADE_ATTEMPT_3}";
			ConversationSentence.OnConditionDelegate onConditionDelegate3 = new ConversationSentence.OnConditionDelegate(this.conversation_courtship_persuasion_option_3_on_condition);
			ConversationSentence.OnConsequenceDelegate onConsequenceDelegate3 = new ConversationSentence.OnConsequenceDelegate(this.conversation_romance_3_persuade_option_on_consequence);
			int num3 = 100;
			onPersuasionOptionDelegate = new ConversationSentence.OnPersuasionOptionDelegate(this.SetupCourtshipPersuasionOption3);
			starter.AddPlayerLine(text9, text10, text11, text12, onConditionDelegate3, onConsequenceDelegate3, num3, new ConversationSentence.OnClickableConditionDelegate(this.RomancePersuasionOption3ClickableOnCondition3), onPersuasionOptionDelegate);
			string text13 = "hero_courtship_argument_4";
			string text14 = "hero_courtship_argument";
			string text15 = "hero_courtship_reaction";
			string text16 = "{=!}{ROMANCE_PERSUADE_ATTEMPT_4}";
			ConversationSentence.OnConditionDelegate onConditionDelegate4 = new ConversationSentence.OnConditionDelegate(this.conversation_courtship_persuasion_option_4_on_condition);
			ConversationSentence.OnConsequenceDelegate onConsequenceDelegate4 = new ConversationSentence.OnConsequenceDelegate(this.conversation_romance_4_persuade_option_on_consequence);
			int num4 = 100;
			onPersuasionOptionDelegate = new ConversationSentence.OnPersuasionOptionDelegate(this.SetupCourtshipPersuasionOption4);
			starter.AddPlayerLine(text13, text14, text15, text16, onConditionDelegate4, onConsequenceDelegate4, num4, new ConversationSentence.OnClickableConditionDelegate(this.RomancePersuasionOption4ClickableOnCondition4), onPersuasionOptionDelegate);
			starter.AddPlayerLine("lord_ask_recruit_argument_no_answer", "hero_courtship_argument", "lord_pretalk", "{=!}{TRY_HARDER_LINE}", new ConversationSentence.OnConditionDelegate(this.conversation_courtship_try_later_on_condition), new ConversationSentence.OnConsequenceDelegate(this.conversation_fail_courtship_on_consequence), 100, null, null);
			starter.AddDialogLine("lord_ask_recruit_argument_reaction", "hero_courtship_reaction", "hero_courtship_task_1_next_reservation", "{=!}{PERSUASION_REACTION}", new ConversationSentence.OnConditionDelegate(this.conversation_courtship_reaction_stage_1_on_condition), new ConversationSentence.OnConsequenceDelegate(this.conversation_lord_persuade_option_reaction_on_consequence), 100, null);
			starter.AddDialogLine("lord_ask_recruit_argument_reaction", "hero_courtship_reaction", "hero_courtship_task_2_next_reservation", "{=!}{PERSUASION_REACTION}", new ConversationSentence.OnConditionDelegate(this.conversation_courtship_reaction_stage_2_on_condition), new ConversationSentence.OnConsequenceDelegate(this.conversation_lord_persuade_option_reaction_on_consequence), 100, null);
			starter.AddDialogLine("hero_courtship_end_conversation", "hero_courtship_end_conversation", "close_window", "{=Mk9k8Sec}As always, it is a delight to speak to you.", null, new ConversationSentence.OnConsequenceDelegate(this.courtship_conversation_leave_on_consequence), 100, null);
			starter.AddDialogLine("hero_courtship_final_barter", "hero_courtship_final_barter", "hero_courtship_final_barter_setup", "{=0UPds9x3}Very well, then...", null, null, 100, null);
			starter.AddDialogLine("hero_courtship_final_barter_setup", "hero_courtship_final_barter_setup", "hero_courtship_final_barter_conclusion", "{=qqzJTfo0}Barter line goes here.", null, new ConversationSentence.OnConsequenceDelegate(this.conversation_finalize_marriage_barter_consequence), 100, null);
			starter.AddDialogLine("hero_courtship_final_barter_setup", "hero_courtship_final_barter_conclusion", "close_window", "{=FGVzQUao}Congratulations, and may the Heavens bless you.", new ConversationSentence.OnConditionDelegate(this.conversation_marriage_barter_successful_on_condition), new ConversationSentence.OnConsequenceDelegate(this.conversation_marriage_barter_successful_on_consequence), 100, null);
			starter.AddDialogLine("hero_courtship_final_barter_setup", "hero_courtship_final_barter_conclusion", "close_window", "{=iunPaMFv}I guess we should put this aside, for now. But perhaps we can speak again at a later date.", () => !this.conversation_marriage_barter_successful_on_condition(), null, 100, null);
			starter.AddPlayerLine("lord_propose_marriage_conv_general_proposal", "lord_talk_speak_diplomacy_2", "lord_propose_marriage_to_clan_leader", "{=v9tQv4eN}I would like to propose an alliance between our families through marriage.", new ConversationSentence.OnConditionDelegate(this.conversation_discuss_marriage_alliance_on_condition), new ConversationSentence.OnConsequenceDelegate(this.conversation_find_player_relatives_eligible_for_marriage_on_consequence), 120, null, null);
			starter.AddDialogLine("lord_propose_marriage_conv_general_proposal_response", "lord_propose_marriage_to_clan_leader", "lord_propose_marriage_to_clan_leader_options", "{=MhPAHpND}And whose hand are you offering?", null, null, 100, null);
			starter.AddPlayerLine("lord_propose_marriage_conv_general_proposal", "lord_propose_marriage_to_clan_leader_options", "lord_propose_marriage_to_clan_leader_response", "{=N1Ue4Blt}My own hand.", new ConversationSentence.OnConditionDelegate(this.conversation_player_eligible_for_marriage_with_hero_rltv_on_condition), new ConversationSentence.OnConsequenceDelegate(this.conversation_player_nominates_self_for_marriage_on_consequence), 120, null, null);
			starter.AddRepeatablePlayerLine("lord_propose_marriage_conv_general_proposal_2", "lord_propose_marriage_to_clan_leader_options", "lord_propose_marriage_to_clan_leader_response", "{=QGj8zQIc}The hand of {MARRIAGE_CANDIDATE.NAME}.", "I am thinking of a different person.", "lord_propose_marriage_to_clan_leader", new ConversationSentence.OnConditionDelegate(this.conversation_player_relative_eligible_for_marriage_on_condition), new ConversationSentence.OnConsequenceDelegate(this.conversation_player_nominates_marriage_relative_on_consequence), 100, null);
			starter.AddPlayerLine("lord_propose_marriage_conv_general_proposal", "lord_propose_marriage_to_clan_leader_options", "lord_pretalk", "{=D33fIGQe}Never mind.", null, null, 120, null, null);
			starter.AddDialogLine("lord_propose_marriage_to_clan_leader_response", "lord_propose_marriage_to_clan_leader_response", "lord_propose_marriage_to_clan_leader_response_self", "{=DdtrRYEM}Well yes. I was looking for a suitable match.", new ConversationSentence.OnConditionDelegate(this.conversation_propose_clan_leader_for_player_nomination_on_condition), null, 100, null);
			starter.AddPlayerLine("lord_propose_marriage_to_clan_leader_response_yes", "lord_propose_marriage_to_clan_leader_response_self", "lord_start_courtship_response", "{=bx4MiPqN}Yes. I would be honored to be considered.", new ConversationSentence.OnConditionDelegate(this.conversation_player_opens_courtship_on_condition), new ConversationSentence.OnConsequenceDelegate(this.conversation_player_opens_courtship_on_consequence), 100, null, null);
			starter.AddPlayerLine("lord_propose_marriage_to_clan_leader_response_plyr_rltv_yes", "lord_propose_marriage_to_clan_leader_response_self", "lord_propose_marriage_to_clan_leader_confirm", "{=ziA4catk}Very good.", new ConversationSentence.OnConditionDelegate(this.conversation_player_rltv_agrees_on_courtship_on_condition), new ConversationSentence.OnConsequenceDelegate(this.conversation_player_agrees_on_courtship_on_consequence), 100, null, null);
			starter.AddPlayerLine("lord_propose_marriage_to_clan_leader_response_no", "lord_propose_marriage_to_clan_leader_response_self", "lord_pretalk", "{=Zw95lDI3}Hmm.. That might not work out.", null, null, 100, null, null);
			starter.AddDialogLine("lord_propose_marriage_to_clan_leader_response", "lord_propose_marriage_to_clan_leader_response", "lord_propose_marriage_to_clan_leader_response_other", "{=!}{ARRANGE_MARRIAGE_LINE}", new ConversationSentence.OnConditionDelegate(this.conversation_propose_spouse_for_player_nomination_on_condition), null, 100, null);
			starter.AddPlayerLine("lord_propose_marriage_to_clan_leader_response_plyr_yes", "lord_propose_marriage_to_clan_leader_response_other", "lord_propose_marriage_to_clan_leader_confirm", "{=ziA4catk}Very good.", new ConversationSentence.OnConditionDelegate(this.conversation_player_rltv_agrees_on_courtship_on_condition), new ConversationSentence.OnConsequenceDelegate(this.conversation_player_agrees_on_courtship_on_consequence), 100, null, null);
			starter.AddPlayerLine("lord_propose_marriage_to_clan_leader_response_plyr_no", "lord_propose_marriage_to_clan_leader_response_other", "lord_pretalk", "{=Zw95lDI3}Hmm.. That might not work out.", null, null, 100, null, null);
			starter.AddDialogLine("lord_propose_marriage_to_clan_leader_response_negative_plyr_response", "lord_propose_marriage_to_clan_leader_response", "lord_pretalk", "{=Zw95lDI3}Hmm.. That might not work out.", null, null, 100, null);
			starter.AddDialogLine("lord_propose_marriage_to_clan_leader_confirm", "lord_propose_marriage_to_clan_leader_confirm", "lord_start", "{=VJEM0IcV}Let's discuss the details then.", null, new ConversationSentence.OnConsequenceDelegate(this.conversation_lord_propose_marriage_to_clan_leader_confirm_consequences), 100, null);
			starter.AddDialogLine("lord_start_courtship_response", "lord_start_courtship_response", "lord_start_courtship_response_player_offer", "{=!}{INITIAL_COURTSHIP_REACTION}", new ConversationSentence.OnConditionDelegate(this.conversation_courtship_initial_reaction_on_condition), null, 100, null);
			starter.AddDialogLine("lord_start_courtship_response_decline", "lord_start_courtship_response", "lord_pretalk", "{=!}{COURTSHIP_DECLINE_REACTION}", new ConversationSentence.OnConditionDelegate(this.conversation_courtship_decline_reaction_to_player_on_condition), null, 100, null);
			starter.AddPlayerLine("lord_start_courtship_response_player_offer", "lord_start_courtship_response_player_offer", "lord_start_courtship_response_2", "{=cKtJBdPD}I wish to offer my hand in marriage.", new ConversationSentence.OnConditionDelegate(this.conversation_player_eligible_for_marriage_with_conversation_hero_on_condition), null, 120, null, null);
			starter.AddPlayerLine("lord_start_courtship_response_player_offer_2", "lord_start_courtship_response_player_offer", "lord_start_courtship_response_2", "{=gnXoIChw}Perhaps you and I...", new ConversationSentence.OnConditionDelegate(this.conversation_player_eligible_for_marriage_with_conversation_hero_on_condition), null, 120, null, null);
			starter.AddPlayerLine("lord_start_courtship_response_player_offer_nevermind", "lord_start_courtship_response_player_offer", "lord_pretalk", "{=D33fIGQe}Never mind.", null, null, 120, null, null);
			starter.AddDialogLine("lord_start_courtship_response_2", "lord_start_courtship_response_2", "lord_start_courtship_response_3", "{=!}{INITIAL_COURTSHIP_REACTION_TO_PLAYER}", new ConversationSentence.OnConditionDelegate(this.conversation_courtship_reaction_to_player_on_condition), null, 100, null);
			starter.AddDialogLine("lord_start_courtship_response_3", "lord_start_courtship_response_3", "close_window", "{=YHZsHohq}We meet from time to time, as is the custom, to see if we are right for each other. I hope to see you again soon.", null, new ConversationSentence.OnConsequenceDelegate(this.courtship_conversation_leave_on_consequence), 100, null);
			starter.AddDialogLine("lord_propose_marriage_conv_general_proposal_response", "lord_propose_general_proposal_response", "lord_propose_marriage_options", "{=k1hyviBO}Tell me, what is on your mind?", null, null, 100, null);
			starter.AddPlayerLine("lord_propose_marriage_conv_nevermind", "lord_propose_marriage_options", "lord_pretalk", "{=D33fIGQe}Never mind.", null, null, 100, null, null);
			starter.AddPlayerLine("lord_propose_marriage_conv_nevermind", "lord_propose_marry_our_children_options", "lord_pretalk", "{=D33fIGQe}Never mind.", null, null, 100, null, null);
			starter.AddPlayerLine("lord_propose_marriage_conv_nevermind", "lord_propose_marry_one_of_your_kind_options", "lord_pretalk", "{=D33fIGQe}Never mind.", null, null, 100, null, null);
		}

		// Token: 0x06003A9A RID: 15002 RVA: 0x00110AF0 File Offset: 0x0010ECF0
		private bool courtship_hero_not_clan_leader_on_condition()
		{
			Hero leader = Hero.OneToOneConversationHero.Clan.Leader;
			if (leader == Hero.OneToOneConversationHero)
			{
				return false;
			}
			StringHelpers.SetCharacterProperties("CLAN_LEADER", leader.CharacterObject, null, false);
			return true;
		}

		// Token: 0x06003A9B RID: 15003 RVA: 0x00110B2B File Offset: 0x0010ED2B
		private void courtship_conversation_leave_on_consequence()
		{
			if (PlayerEncounter.Current != null)
			{
				PlayerEncounter.LeaveEncounter = true;
			}
		}

		// Token: 0x06003A9C RID: 15004 RVA: 0x00110B3C File Offset: 0x0010ED3C
		private void conversation_finalize_marriage_barter_consequence()
		{
			Hero heroBeingProposedTo = Hero.OneToOneConversationHero;
			foreach (Hero hero in Hero.OneToOneConversationHero.Clan.Lords)
			{
				if (Romance.GetRomanticLevel(Hero.MainHero, hero) == Romance.RomanceLevelEnum.CoupleAgreedOnMarriage)
				{
					heroBeingProposedTo = hero;
					break;
				}
			}
			MarriageBarterable marriageBarterable = new MarriageBarterable(Hero.MainHero, PartyBase.MainParty, heroBeingProposedTo, Hero.MainHero);
			BarterManager instance = BarterManager.Instance;
			Hero mainHero = Hero.MainHero;
			Hero oneToOneConversationHero = Hero.OneToOneConversationHero;
			PartyBase mainParty = PartyBase.MainParty;
			MobileParty partyBelongedTo = Hero.OneToOneConversationHero.PartyBelongedTo;
			instance.StartBarterOffer(mainHero, oneToOneConversationHero, mainParty, (partyBelongedTo != null) ? partyBelongedTo.Party : null, null, (Barterable barterable, BarterData _args, object obj) => BarterManager.Instance.InitializeMarriageBarterContext(barterable, _args, new Tuple<Hero, Hero>(heroBeingProposedTo, Hero.MainHero)), (int)Romance.GetRomanticState(Hero.MainHero, heroBeingProposedTo).ScoreFromPersuasion, false, new Barterable[] { marriageBarterable });
			if (PlayerEncounter.Current != null)
			{
				PlayerEncounter.LeaveEncounter = true;
			}
		}

		// Token: 0x06003A9D RID: 15005 RVA: 0x00110C40 File Offset: 0x0010EE40
		private bool PersuasionAttemptRecentlyFailed(Hero targetHero, int reservationType)
		{
			foreach (PersuasionAttempt persuasionAttempt in this._previousRomancePersuasionAttempts)
			{
				if (persuasionAttempt.Matches(targetHero, reservationType) && !persuasionAttempt.IsSuccesful() && persuasionAttempt.GameTime.ElapsedWeeksUntilNow < 1f)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06003A9E RID: 15006 RVA: 0x00110CBC File Offset: 0x0010EEBC
		private void DailyTick()
		{
			foreach (Romance.RomanticState romanticState in Romance.RomanticStateList.ToList<Romance.RomanticState>())
			{
				if (romanticState.Person1.IsDead || romanticState.Person2.IsDead)
				{
					Romance.RomanticStateList.Remove(romanticState);
				}
			}
		}

		// Token: 0x06003A9F RID: 15007 RVA: 0x00110D34 File Offset: 0x0010EF34
		private bool PersuasionAttemptRecentlyCriticallyFailed(Hero targetHero, int reservationType)
		{
			foreach (PersuasionAttempt persuasionAttempt in this._previousRomancePersuasionAttempts)
			{
				if (persuasionAttempt.Matches(targetHero, reservationType) && persuasionAttempt.Result == PersuasionOptionResult.CriticalFailure && persuasionAttempt.GameTime.ElapsedWeeksUntilNow < 1f)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06003AA0 RID: 15008 RVA: 0x00110DB0 File Offset: 0x0010EFB0
		private IEnumerable<RomanceCampaignBehavior.RomanceReservationDescription> GetRomanceReservations(Hero wooed, Hero wooer)
		{
			List<RomanceCampaignBehavior.RomanceReservationDescription> list = new List<RomanceCampaignBehavior.RomanceReservationDescription>();
			bool flag = wooed.GetTraitLevel(DefaultTraits.Honor) + wooed.GetTraitLevel(DefaultTraits.Mercy) > 0;
			bool flag2 = wooed.GetTraitLevel(DefaultTraits.Honor) < 1 && wooed.GetTraitLevel(DefaultTraits.Valor) < 1 && wooed.GetTraitLevel(DefaultTraits.Calculating) < 1;
			bool flag3 = wooed.GetTraitLevel(DefaultTraits.Calculating) - wooed.GetTraitLevel(DefaultTraits.Mercy) >= 0;
			bool flag4 = wooed.GetTraitLevel(DefaultTraits.Valor) - wooed.GetTraitLevel(DefaultTraits.Calculating) > 0 && wooed.GetTraitLevel(DefaultTraits.Mercy) <= 0;
			if (flag)
			{
				list.Add(RomanceCampaignBehavior.RomanceReservationDescription.CompatibilityINeedSomeoneUpright);
			}
			else if (flag4 && wooed.IsFemale)
			{
				list.Add(RomanceCampaignBehavior.RomanceReservationDescription.CompatibiliyINeedSomeoneDangerous);
			}
			else
			{
				list.Add(RomanceCampaignBehavior.RomanceReservationDescription.CompatibilityNeedSomethingInCommon);
			}
			int attractionValuePercentage = Campaign.Current.Models.RomanceModel.GetAttractionValuePercentage(Hero.OneToOneConversationHero, Hero.MainHero);
			if (attractionValuePercentage > 70)
			{
				list.Add(RomanceCampaignBehavior.RomanceReservationDescription.AttractionIAmDrawnToYou);
			}
			else if (attractionValuePercentage > 40)
			{
				list.Add(RomanceCampaignBehavior.RomanceReservationDescription.AttractionYoureGoodEnough);
			}
			else
			{
				list.Add(RomanceCampaignBehavior.RomanceReservationDescription.AttractionYoureNotMyType);
			}
			List<Settlement> list2 = Settlement.All.Where((Settlement x) => x.OwnerClan == wooer.Clan).ToList<Settlement>();
			if (flag3 && wooer.IsFemale && list2.Count < 1)
			{
				list.Add(RomanceCampaignBehavior.RomanceReservationDescription.PropertyHowCanIMarryAnAdventuress);
			}
			else if (flag3 && list2.Count < 3)
			{
				list.Add(RomanceCampaignBehavior.RomanceReservationDescription.PropertyIWantRealWealth);
			}
			else if (list2.Count < 1)
			{
				list.Add(RomanceCampaignBehavior.RomanceReservationDescription.PropertyWeNeedToBeComfortable);
			}
			else
			{
				list.Add(RomanceCampaignBehavior.RomanceReservationDescription.PropertyYouSeemRichEnough);
			}
			float unmodifiedClanLeaderRelationshipWithPlayer = Hero.OneToOneConversationHero.GetUnmodifiedClanLeaderRelationshipWithPlayer();
			if (unmodifiedClanLeaderRelationshipWithPlayer < -10f)
			{
				list.Add(RomanceCampaignBehavior.RomanceReservationDescription.FamilyApprovalHowCanYouBeEnemiesWithOurFamily);
			}
			else if (!flag2 && unmodifiedClanLeaderRelationshipWithPlayer < 10f)
			{
				list.Add(RomanceCampaignBehavior.RomanceReservationDescription.FamilyApprovalItWouldBeBestToBefriendOurFamily);
			}
			else if (flag2 && unmodifiedClanLeaderRelationshipWithPlayer < 10f)
			{
				list.Add(RomanceCampaignBehavior.RomanceReservationDescription.FamilyApprovalYouNeedToBeFriendsWithOurFamily);
			}
			else
			{
				list.Add(RomanceCampaignBehavior.RomanceReservationDescription.FamilyApprovalIAmGladYouAreFriendsWithOurFamily);
			}
			return list;
		}

		// Token: 0x06003AA1 RID: 15009 RVA: 0x00110F98 File Offset: 0x0010F198
		private List<PersuasionTask> GetPersuasionTasksForCourtshipStage1(Hero wooed, Hero wooer)
		{
			StringHelpers.SetCharacterProperties("PLAYER", Hero.MainHero.CharacterObject, null, false);
			List<PersuasionTask> list = new List<PersuasionTask>();
			this.GetRomanceReservations(wooed, wooer);
			PersuasionTask persuasionTask = new PersuasionTask(0);
			list.Add(persuasionTask);
			persuasionTask.FinalFailLine = new TextObject("{=dY2PzpIV}I'm not sure how much we have in common..", null);
			persuasionTask.TryLaterLine = new TextObject("{=PoDVgQaz}Well, it would take a bit long to discuss this.", null);
			persuasionTask.SpokenLine = Campaign.Current.ConversationManager.FindMatchingTextOrNull("str_courtship_travel_task", CharacterObject.OneToOneConversationCharacter);
			Tuple<TraitObject, int>[] traitCorrelations = this.GetTraitCorrelations(1, -1, 0, 0, 1);
			PersuasionArgumentStrength argumentStrengthBasedOnTargetTraits = Campaign.Current.Models.PersuasionModel.GetArgumentStrengthBasedOnTargetTraits(CharacterObject.OneToOneConversationCharacter, traitCorrelations);
			PersuasionOptionArgs persuasionOptionArgs = new PersuasionOptionArgs(DefaultSkills.Leadership, DefaultTraits.Valor, TraitEffect.Positive, argumentStrengthBasedOnTargetTraits, false, new TextObject("{=YNBm3LkC}I feel lucky to live in a time where a valiant warrior can make a name for {?PLAYER.GENDER}herself{?}himself{\\?}.", null), traitCorrelations, false, true, false);
			persuasionTask.AddOptionToTask(persuasionOptionArgs);
			Tuple<TraitObject, int>[] traitCorrelations2 = this.GetTraitCorrelations(1, -1, 0, 0, 1);
			PersuasionArgumentStrength argumentStrengthBasedOnTargetTraits2 = Campaign.Current.Models.PersuasionModel.GetArgumentStrengthBasedOnTargetTraits(CharacterObject.OneToOneConversationCharacter, traitCorrelations2);
			PersuasionOptionArgs persuasionOptionArgs2 = new PersuasionOptionArgs(DefaultSkills.Roguery, DefaultTraits.Valor, TraitEffect.Positive, argumentStrengthBasedOnTargetTraits2, false, new TextObject("{=rtqD9cnu}Yeah, it's a rough world, but there are lots of opportunities to be seized right now if you're not afraid to get your hands a bit dirty.", null), traitCorrelations2, false, true, false);
			persuasionTask.AddOptionToTask(persuasionOptionArgs2);
			Tuple<TraitObject, int>[] traitCorrelations3 = this.GetTraitCorrelations(0, 1, 1, 0, -1);
			PersuasionArgumentStrength argumentStrengthBasedOnTargetTraits3 = Campaign.Current.Models.PersuasionModel.GetArgumentStrengthBasedOnTargetTraits(CharacterObject.OneToOneConversationCharacter, traitCorrelations3);
			PersuasionOptionArgs persuasionOptionArgs3 = new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Mercy, TraitEffect.Positive, argumentStrengthBasedOnTargetTraits3, false, new TextObject("{=rfyalLyY}What can I say? It's a beautiful world, but filled with so much suffering.", null), traitCorrelations3, false, true, false);
			persuasionTask.AddOptionToTask(persuasionOptionArgs3);
			Tuple<TraitObject, int>[] traitCorrelations4 = this.GetTraitCorrelations(-1, 0, -1, -1, 0);
			PersuasionArgumentStrength argumentStrengthBasedOnTargetTraits4 = Campaign.Current.Models.PersuasionModel.GetArgumentStrengthBasedOnTargetTraits(CharacterObject.OneToOneConversationCharacter, traitCorrelations4);
			PersuasionOptionArgs persuasionOptionArgs4 = new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Generosity, TraitEffect.Negative, argumentStrengthBasedOnTargetTraits4, false, new TextObject("{=ja5bAOMr}The world's a dungheap, basically. The sooner I earn enough to retire, the better.", null), traitCorrelations4, false, true, false);
			persuasionTask.AddOptionToTask(persuasionOptionArgs4);
			PersuasionTask persuasionTask2 = new PersuasionTask(1);
			list.Add(persuasionTask2);
			persuasionTask2.SpokenLine = new TextObject("{=5Vk6I1sf}Between your followers, your rivals and your enemies, you must have met a lot of interesting people...", null);
			persuasionTask2.FinalFailLine = new TextObject("{=lDJUL4lZ}I think we maybe see the world a bit differently.", null);
			persuasionTask2.TryLaterLine = new TextObject("{=ZmxbIXsp}I am sorry you feel that way. We can speak later.", null);
			Tuple<TraitObject, int>[] traitCorrelations5 = this.GetTraitCorrelations(1, 0, 1, 2, 0);
			PersuasionArgumentStrength argumentStrengthBasedOnTargetTraits5 = Campaign.Current.Models.PersuasionModel.GetArgumentStrengthBasedOnTargetTraits(CharacterObject.OneToOneConversationCharacter, traitCorrelations5);
			PersuasionOptionArgs persuasionOptionArgs5 = new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Generosity, TraitEffect.Positive, argumentStrengthBasedOnTargetTraits5, false, new TextObject("{=8BnWa83o}I'm just honored to have fought alongside comrades who thought nothing of shedding their blood to keep me alive.", null), traitCorrelations5, false, true, false);
			persuasionTask2.AddOptionToTask(persuasionOptionArgs5);
			Tuple<TraitObject, int>[] traitCorrelations6 = this.GetTraitCorrelations(0, 0, -1, 0, 1);
			PersuasionArgumentStrength argumentStrengthBasedOnTargetTraits6 = Campaign.Current.Models.PersuasionModel.GetArgumentStrengthBasedOnTargetTraits(CharacterObject.OneToOneConversationCharacter, traitCorrelations6);
			PersuasionOptionArgs persuasionOptionArgs6 = new PersuasionOptionArgs(DefaultSkills.Roguery, DefaultTraits.Calculating, TraitEffect.Positive, argumentStrengthBasedOnTargetTraits6, false, new TextObject("{=QHG6LU1g}Ah yes, I've seen cruelty, degradation and degeneracy like you wouldn't believe. Fascinating stuff, all of it.", null), traitCorrelations6, false, true, false);
			persuasionTask2.AddOptionToTask(persuasionOptionArgs6);
			Tuple<TraitObject, int>[] traitCorrelations7 = this.GetTraitCorrelations(0, 2, 0, 0, 0);
			PersuasionArgumentStrength argumentStrengthBasedOnTargetTraits7 = Campaign.Current.Models.PersuasionModel.GetArgumentStrengthBasedOnTargetTraits(CharacterObject.OneToOneConversationCharacter, traitCorrelations7);
			PersuasionOptionArgs persuasionOptionArgs7 = new PersuasionOptionArgs(DefaultSkills.Leadership, DefaultTraits.Mercy, TraitEffect.Positive, argumentStrengthBasedOnTargetTraits7, false, new TextObject("{=bwWdGLDv}I have seen great good and great evil, but I can only hope the good outweights the evil in most people's hearts.", null), traitCorrelations7, false, true, false);
			persuasionTask2.AddOptionToTask(persuasionOptionArgs7);
			Tuple<TraitObject, int>[] traitCorrelations8 = this.GetTraitCorrelations(-1, 0, -1, -1, 0);
			PersuasionArgumentStrength argumentStrengthBasedOnTargetTraits8 = Campaign.Current.Models.PersuasionModel.GetArgumentStrengthBasedOnTargetTraits(CharacterObject.OneToOneConversationCharacter, traitCorrelations8);
			PersuasionOptionArgs persuasionOptionArgs8 = new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Generosity, TraitEffect.Negative, argumentStrengthBasedOnTargetTraits8, false, new TextObject("{=3skTM1DC}Most people would put a knife in your back for a few coppers. Have a few friends and keep them close, I guess.", null), traitCorrelations8, false, true, false);
			persuasionTask2.AddOptionToTask(persuasionOptionArgs8);
			PersuasionTask persuasionTask3 = new PersuasionTask(2);
			list.Add(persuasionTask3);
			persuasionTask3.SpokenLine = Campaign.Current.ConversationManager.FindMatchingTextOrNull("str_courtship_aspirations_task", CharacterObject.OneToOneConversationCharacter);
			persuasionTask3.ImmediateFailLine = new TextObject("{=8hEVO9hw}Hmm. Perhaps you and I have different priorities in life.", null);
			persuasionTask3.FinalFailLine = new TextObject("{=HAtHptbV}In the end, I don't think we have that much in common.", null);
			persuasionTask3.TryLaterLine = new TextObject("{=ZmxbIXsp}I am sorry you feel that way. We can speak later.", null);
			Tuple<TraitObject, int>[] traitCorrelations9 = this.GetTraitCorrelations(0, 2, 1, 0, 0);
			PersuasionArgumentStrength argumentStrengthBasedOnTargetTraits9 = Campaign.Current.Models.PersuasionModel.GetArgumentStrengthBasedOnTargetTraits(CharacterObject.OneToOneConversationCharacter, traitCorrelations9);
			PersuasionOptionArgs persuasionOptionArgs9 = new PersuasionOptionArgs(DefaultSkills.Leadership, DefaultTraits.Mercy, TraitEffect.Positive, argumentStrengthBasedOnTargetTraits9, false, new TextObject("{=6kjacaiB}I hope I can bring peace to the land, and justice, and alleviate people's suffering.", null), traitCorrelations9, false, true, false);
			persuasionTask3.AddOptionToTask(persuasionOptionArgs9);
			Tuple<TraitObject, int>[] traitCorrelations10 = this.GetTraitCorrelations(1, 1, 0, 2, 0);
			PersuasionArgumentStrength argumentStrengthBasedOnTargetTraits10 = Campaign.Current.Models.PersuasionModel.GetArgumentStrengthBasedOnTargetTraits(CharacterObject.OneToOneConversationCharacter, traitCorrelations10);
			PersuasionOptionArgs persuasionOptionArgs10 = new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Generosity, TraitEffect.Positive, argumentStrengthBasedOnTargetTraits10, false, new TextObject("{=rrqCZa0H}I'll make sure those who stuck their necks out for me, who sweated and bled for me, get their due.", null), traitCorrelations10, false, true, false);
			persuasionTask3.AddOptionToTask(persuasionOptionArgs10);
			Tuple<TraitObject, int>[] traitCorrelations11 = this.GetTraitCorrelations(0, 0, 0, 0, 2);
			PersuasionArgumentStrength argumentStrengthBasedOnTargetTraits11 = Campaign.Current.Models.PersuasionModel.GetArgumentStrengthBasedOnTargetTraits(CharacterObject.OneToOneConversationCharacter, traitCorrelations11);
			PersuasionOptionArgs persuasionOptionArgs11 = new PersuasionOptionArgs(DefaultSkills.Roguery, DefaultTraits.Calculating, TraitEffect.Positive, argumentStrengthBasedOnTargetTraits11, false, new TextObject("{=ggKa4Bd8}Hmm... First thing to do after taking power is to work on your plan to remain in power.", null), traitCorrelations11, false, true, false);
			persuasionTask3.AddOptionToTask(persuasionOptionArgs11);
			Tuple<TraitObject, int>[] traitCorrelations12 = this.GetTraitCorrelations(0, -2, 0, -1, 1);
			PersuasionArgumentStrength argumentStrengthBasedOnTargetTraits12 = Campaign.Current.Models.PersuasionModel.GetArgumentStrengthBasedOnTargetTraits(CharacterObject.OneToOneConversationCharacter, traitCorrelations12);
			PersuasionOptionArgs persuasionOptionArgs12 = new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Calculating, TraitEffect.Positive, argumentStrengthBasedOnTargetTraits12, false, new TextObject("{=6L1b1nJa}Oh I have a long list of scores to settle. You can be sure of that.", null), traitCorrelations12, false, true, false);
			persuasionTask3.AddOptionToTask(persuasionOptionArgs12);
			persuasionTask2.FinalFailLine = new TextObject("{=Ns315pxY}Perhaps we are not meant for each other.", null);
			persuasionTask2.TryLaterLine = new TextObject("{=PoDVgQaz}Well, it would take a bit long to discuss this.", null);
			return list;
		}

		// Token: 0x06003AA2 RID: 15010 RVA: 0x00111500 File Offset: 0x0010F700
		private Tuple<TraitObject, int>[] GetTraitCorrelations(int valor = 0, int mercy = 0, int honor = 0, int generosity = 0, int calculating = 0)
		{
			return new Tuple<TraitObject, int>[]
			{
				new Tuple<TraitObject, int>(DefaultTraits.Valor, valor),
				new Tuple<TraitObject, int>(DefaultTraits.Mercy, mercy),
				new Tuple<TraitObject, int>(DefaultTraits.Honor, honor),
				new Tuple<TraitObject, int>(DefaultTraits.Generosity, generosity),
				new Tuple<TraitObject, int>(DefaultTraits.Calculating, calculating)
			};
		}

		// Token: 0x06003AA3 RID: 15011 RVA: 0x0011155C File Offset: 0x0010F75C
		private List<PersuasionTask> GetPersuasionTasksForCourtshipStage2(Hero wooed, Hero wooer)
		{
			StringHelpers.SetCharacterProperties("PLAYER", Hero.MainHero.CharacterObject, null, false);
			List<PersuasionTask> list = new List<PersuasionTask>();
			IEnumerable<RomanceCampaignBehavior.RomanceReservationDescription> romanceReservations = this.GetRomanceReservations(wooed, wooer);
			bool flag = romanceReservations.Any((RomanceCampaignBehavior.RomanceReservationDescription x) => x == RomanceCampaignBehavior.RomanceReservationDescription.AttractionIAmDrawnToYou);
			List<RomanceCampaignBehavior.RomanceReservationDescription> list2 = romanceReservations.Where((RomanceCampaignBehavior.RomanceReservationDescription x) => x == RomanceCampaignBehavior.RomanceReservationDescription.CompatibiliyINeedSomeoneDangerous || x == RomanceCampaignBehavior.RomanceReservationDescription.CompatibilityNeedSomethingInCommon || x == RomanceCampaignBehavior.RomanceReservationDescription.CompatibilityINeedSomeoneUpright || x == RomanceCampaignBehavior.RomanceReservationDescription.CompatibilityStrongPoliticalBeliefs).ToList<RomanceCampaignBehavior.RomanceReservationDescription>();
			if (list2.Count > 0)
			{
				RomanceCampaignBehavior.RomanceReservationDescription romanceReservationDescription = list2[0];
				PersuasionTask persuasionTask = new PersuasionTask(3);
				list.Add(persuasionTask);
				persuasionTask.SpokenLine = new TextObject("{=rtP6vnmj}I'm not sure we're compatible.", null);
				persuasionTask.FinalFailLine = new TextObject("{=bBTHy6f9}I just don't think that we would be happy together.", null);
				persuasionTask.TryLaterLine = new TextObject("{=o9ouu97M}I will endeavor to be worthy of your affections.", null);
				PersuasionArgumentStrength persuasionArgumentStrength = PersuasionArgumentStrength.Normal;
				if (romanceReservationDescription == RomanceCampaignBehavior.RomanceReservationDescription.CompatibiliyINeedSomeoneDangerous)
				{
					if (Hero.OneToOneConversationHero.IsFemale)
					{
						persuasionTask.SpokenLine = new TextObject("{=EkkNQb5N}I like a warrior who strikes fear in the hearts of his enemies. Are you that kind of man?", null);
					}
					else
					{
						persuasionTask.SpokenLine = new TextObject("{=3cw5pRFM}I had not thought that I might marry a shieldmaiden. But it is intriguing. Tell me, have you killed men in battle?", null);
					}
					PersuasionOptionArgs persuasionOptionArgs = new PersuasionOptionArgs(DefaultSkills.Roguery, DefaultTraits.Valor, TraitEffect.Positive, persuasionArgumentStrength, false, new TextObject("{=FEmiPPbO}Perhaps you've heard the stories about me, then. They're all true.", null), null, false, true, false);
					persuasionTask.AddOptionToTask(persuasionOptionArgs);
					PersuasionOptionArgs persuasionOptionArgs2 = new PersuasionOptionArgs(DefaultSkills.Roguery, DefaultTraits.Calculating, TraitEffect.Positive, persuasionArgumentStrength + 1, false, new TextObject("{=Oe5Tf7OZ}My foes may not fear my sword, but they should fear my cunning.", null), null, false, true, false);
					persuasionTask.AddOptionToTask(persuasionOptionArgs2);
					if (flag)
					{
						PersuasionOptionArgs persuasionOptionArgs3 = new PersuasionOptionArgs(DefaultSkills.Leadership, DefaultTraits.Calculating, TraitEffect.Negative, persuasionArgumentStrength - 1, true, new TextObject("{=zWTNOfHm}I want you and if you want me, that should be enough!", null), null, false, true, false);
						persuasionTask.AddOptionToTask(persuasionOptionArgs3);
					}
					PersuasionOptionArgs persuasionOptionArgs4 = new PersuasionOptionArgs(DefaultSkills.Roguery, DefaultTraits.Generosity, TraitEffect.Positive, persuasionArgumentStrength + 1, false, new TextObject("{=8a13MGzr}All I can say is that I try to repay good with good, and evil with evil.", null), null, false, true, false);
					persuasionTask.AddOptionToTask(persuasionOptionArgs4);
				}
				if (romanceReservationDescription == RomanceCampaignBehavior.RomanceReservationDescription.CompatibilityINeedSomeoneUpright)
				{
					persuasionTask.SpokenLine = new TextObject("{=lay7hKUK}I insist that my {?PLAYER.GENDER}wife{?}husband{\\?} conduct {?PLAYER.GENDER}herself{?}himself{\\?} according to the highest standards.", null);
					PersuasionOptionArgs persuasionOptionArgs5 = new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Honor, TraitEffect.Positive, PersuasionArgumentStrength.Normal, false, new TextObject("{=bOQEc7jA}I am a {?PLAYER.GENDER}woman{?}man{\\?} of my word. I hope that it is sufficient.", null), null, false, true, false);
					persuasionTask.AddOptionToTask(persuasionOptionArgs5);
					PersuasionOptionArgs persuasionOptionArgs6 = new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Mercy, TraitEffect.Positive, PersuasionArgumentStrength.Normal, false, new TextObject("{=faa9sFfE}I do what I can to alleviate suffering in this world. I hope that is enough.", null), null, false, true, false);
					persuasionTask.AddOptionToTask(persuasionOptionArgs6);
					if (flag)
					{
						PersuasionOptionArgs persuasionOptionArgs7 = new PersuasionOptionArgs(DefaultSkills.Leadership, DefaultTraits.Calculating, TraitEffect.Negative, persuasionArgumentStrength - 1, true, new TextObject("{=zWTNOfHm}I want you and if you want me, that should be enough!", null), null, false, true, false);
						persuasionTask.AddOptionToTask(persuasionOptionArgs7);
					}
					PersuasionOptionArgs persuasionOptionArgs8 = new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Generosity, TraitEffect.Positive, PersuasionArgumentStrength.Hard, false, new TextObject("{=b2ePtImV}Those who are loyal to me, I am loyal to them.", null), null, false, true, false);
					persuasionTask.AddOptionToTask(persuasionOptionArgs8);
				}
				if (romanceReservationDescription == RomanceCampaignBehavior.RomanceReservationDescription.CompatibilityNeedSomethingInCommon)
				{
					persuasionTask.SpokenLine = new TextObject("{=ZsGqHBlR}I need a partner whom I can trust...", null);
					PersuasionOptionArgs persuasionOptionArgs9 = new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Generosity, TraitEffect.Positive, persuasionArgumentStrength - 1, false, new TextObject("{=LTUEFTaF}I hope that I am known as someone who understands the value of loyalty.", null), null, false, true, false);
					persuasionTask.AddOptionToTask(persuasionOptionArgs9);
					PersuasionOptionArgs persuasionOptionArgs10 = new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Honor, TraitEffect.Positive, persuasionArgumentStrength, false, new TextObject("{=9qoLQva5}Whatever oath I give to you, you may be sure that I will keep it.", null), null, false, true, false);
					persuasionTask.AddOptionToTask(persuasionOptionArgs10);
					if (flag)
					{
						PersuasionOptionArgs persuasionOptionArgs11 = new PersuasionOptionArgs(DefaultSkills.Leadership, DefaultTraits.Calculating, TraitEffect.Negative, persuasionArgumentStrength - 1, true, new TextObject("{=zWTNOfHm}I want you and if you want me, that should be enough!", null), null, false, true, false);
						persuasionTask.AddOptionToTask(persuasionOptionArgs11);
					}
					PersuasionOptionArgs persuasionOptionArgs12 = new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Generosity, TraitEffect.Positive, persuasionArgumentStrength, false, new TextObject("{=b2ePtImV}Those who are loyal to me, I am loyal to them.", null), null, false, true, false);
					persuasionTask.AddOptionToTask(persuasionOptionArgs12);
				}
				if (romanceReservationDescription == RomanceCampaignBehavior.RomanceReservationDescription.CompatibilityStrongPoliticalBeliefs)
				{
					if (wooed.GetTraitLevel(DefaultTraits.Egalitarian) > 0)
					{
						persuasionTask.SpokenLine = new TextObject("{=s3Fna6wY}I've always seen myself as someone who sides with the weak of this realm. I don't want to find myself at odds with you.", null);
					}
					if (wooed.GetTraitLevel(DefaultTraits.Oligarchic) > 0)
					{
						persuasionTask.SpokenLine = new TextObject("{=DR2aK4aQ}I respect our ancient laws and traditions. I don't want to find myself at odds with you.", null);
					}
					if (wooed.GetTraitLevel(DefaultTraits.Authoritarian) > 0)
					{
						persuasionTask.SpokenLine = new TextObject("{=c2Yrci3B}I believe that we need a strong ruler in this realm. I don't want to find myself at odds with you.", null);
					}
					PersuasionOptionArgs persuasionOptionArgs13 = new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Mercy, TraitEffect.Positive, persuasionArgumentStrength, false, new TextObject("{=pVPkpP20}We may differ on politics, but I hope you'll think me a man with a good heart.", null), null, false, true, false);
					persuasionTask.AddOptionToTask(persuasionOptionArgs13);
					if (flag)
					{
						PersuasionOptionArgs persuasionOptionArgs14 = new PersuasionOptionArgs(DefaultSkills.Leadership, DefaultTraits.Calculating, TraitEffect.Negative, persuasionArgumentStrength - 1, true, new TextObject("{=yghMrFdT}Put petty politics aside and trust your heart!", null), null, false, true, false);
						persuasionTask.AddOptionToTask(persuasionOptionArgs14);
					}
					PersuasionOptionArgs persuasionOptionArgs15 = new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Generosity, TraitEffect.Positive, persuasionArgumentStrength, false, new TextObject("{=Tj8bGW4b}If a man and a woman respect each other, politics should not divide them.", null), null, false, true, false);
					persuasionTask.AddOptionToTask(persuasionOptionArgs15);
				}
			}
			if (romanceReservations.Where((RomanceCampaignBehavior.RomanceReservationDescription x) => x == RomanceCampaignBehavior.RomanceReservationDescription.AttractionYoureNotMyType).ToList<RomanceCampaignBehavior.RomanceReservationDescription>().Count > 0)
			{
				PersuasionTask persuasionTask2 = new PersuasionTask(4);
				list.Add(persuasionTask2);
				persuasionTask2.SpokenLine = new TextObject("{=cOyolp4F}I am just not... How can I say this? I am not attracted to you.", null);
				persuasionTask2.FinalFailLine = new TextObject("{=LjiYq9cH}I am sorry. I am not sure that I could ever love you.", null);
				persuasionTask2.TryLaterLine = new TextObject("{=E9s2bjqw}I can only hope that some day you could change your mind.", null);
				int num = 0;
				PersuasionArgumentStrength persuasionArgumentStrength2 = (PersuasionArgumentStrength)(num - Hero.OneToOneConversationHero.GetTraitLevel(DefaultTraits.Calculating));
				PersuasionOptionArgs persuasionOptionArgs16 = new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Calculating, TraitEffect.Positive, persuasionArgumentStrength2, false, new TextObject("{=hwjzKcUw}So what? This is supposed to be an alliance of our houses, not of our hearts.", null), null, false, true, false);
				persuasionTask2.AddOptionToTask(persuasionOptionArgs16);
				PersuasionArgumentStrength persuasionArgumentStrength3 = (PersuasionArgumentStrength)(num - Hero.OneToOneConversationHero.GetTraitLevel(DefaultTraits.Generosity));
				PersuasionOptionArgs persuasionOptionArgs17 = new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Generosity, TraitEffect.Positive, persuasionArgumentStrength3 - 1, true, new TextObject("{=m3EkYCA6}Perhaps if you see how much I love you, you could come to love me over time.", null), null, false, true, false);
				persuasionTask2.AddOptionToTask(persuasionOptionArgs17);
				PersuasionArgumentStrength persuasionArgumentStrength4 = (PersuasionArgumentStrength)(num - Hero.OneToOneConversationHero.GetTraitLevel(DefaultTraits.Honor));
				PersuasionOptionArgs persuasionOptionArgs18 = new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Honor, TraitEffect.Positive, persuasionArgumentStrength4, false, new TextObject("{=LN7SGvnS}Love is but an infatuation. Judge me by my character.", null), null, false, true, false);
				persuasionTask2.AddOptionToTask(persuasionOptionArgs18);
			}
			List<RomanceCampaignBehavior.RomanceReservationDescription> list3 = romanceReservations.Where((RomanceCampaignBehavior.RomanceReservationDescription x) => x == RomanceCampaignBehavior.RomanceReservationDescription.PropertyHowCanIMarryAnAdventuress || x == RomanceCampaignBehavior.RomanceReservationDescription.PropertyIWantRealWealth || x == RomanceCampaignBehavior.RomanceReservationDescription.PropertyWeNeedToBeComfortable).ToList<RomanceCampaignBehavior.RomanceReservationDescription>();
			if (list3.Count > 0)
			{
				RomanceCampaignBehavior.RomanceReservationDescription romanceReservationDescription2 = list3[0];
				PersuasionTask persuasionTask3 = new PersuasionTask(6);
				list.Add(persuasionTask3);
				persuasionTask3.SpokenLine = new TextObject("{=beK0AZ2y}I am concerned that you do not have the means to support a family.", null);
				persuasionTask3.FinalFailLine = new TextObject("{=z6vJlozm}I am sorry. I don't believe you have the means to support a family.)", null);
				persuasionTask3.TryLaterLine = new TextObject("{=vaISh0sx}I will go off to make something of myself, then, and shall return to you.", null);
				PersuasionArgumentStrength persuasionArgumentStrength5 = PersuasionArgumentStrength.Normal;
				if (romanceReservationDescription2 == RomanceCampaignBehavior.RomanceReservationDescription.PropertyIWantRealWealth)
				{
					persuasionTask3.SpokenLine = new TextObject("{=pbqjBGk0}I will be honest. I have plans, and I expect the person I marry to have the income to support them.", null);
					persuasionArgumentStrength5 = PersuasionArgumentStrength.Hard;
				}
				else if (romanceReservationDescription2 == RomanceCampaignBehavior.RomanceReservationDescription.PropertyHowCanIMarryAnAdventuress)
				{
					persuasionTask3.SpokenLine = new TextObject("{=ZNfWXliN}I will be honest, my lady. You are but a common adventurer, and by marrying you I give up a chance to forge an alliance with a family of real influence and power.", null);
					persuasionArgumentStrength5 = PersuasionArgumentStrength.Normal;
				}
				PersuasionOptionArgs persuasionOptionArgs19 = new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Calculating, TraitEffect.Positive, persuasionArgumentStrength5, false, new TextObject("{=erKuPRWA}I have a plan to rise in this world. I'm still only a little way up the ladder.", null), null, false, true, false);
				persuasionTask3.AddOptionToTask(persuasionOptionArgs19);
				PersuasionOptionArgs persuasionOptionArgs20 = new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Valor, TraitEffect.Positive, persuasionArgumentStrength5, false, (romanceReservationDescription2 == RomanceCampaignBehavior.RomanceReservationDescription.PropertyHowCanIMarryAnAdventuress) ? new TextObject("{=a2dJDUoL}My sword is my dowry. The gold and land will follow.", null) : new TextObject("{=DLc6NfiV}I shall win you the riches you deserve, or die in the attempt.", null), null, false, true, false);
				persuasionTask3.AddOptionToTask(persuasionOptionArgs20);
				if (flag)
				{
					PersuasionArgumentStrength persuasionArgumentStrength6 = persuasionArgumentStrength5 - Hero.OneToOneConversationHero.GetTraitLevel(DefaultTraits.Calculating);
					PersuasionOptionArgs persuasionOptionArgs21 = new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Generosity, TraitEffect.Positive, persuasionArgumentStrength6, true, new TextObject("{=6LfkfJiJ}Can't your passion for me overcome such base feelings?", null), null, false, true, false);
					persuasionTask3.AddOptionToTask(persuasionOptionArgs21);
				}
			}
			List<RomanceCampaignBehavior.RomanceReservationDescription> list4 = romanceReservations.Where((RomanceCampaignBehavior.RomanceReservationDescription x) => x == RomanceCampaignBehavior.RomanceReservationDescription.FamilyApprovalHowCanYouBeEnemiesWithOurFamily || x == RomanceCampaignBehavior.RomanceReservationDescription.FamilyApprovalItWouldBeBestToBefriendOurFamily || x == RomanceCampaignBehavior.RomanceReservationDescription.FamilyApprovalYouNeedToBeFriendsWithOurFamily).ToList<RomanceCampaignBehavior.RomanceReservationDescription>();
			if (list4.Count > 0 && list.Count < 3)
			{
				RomanceCampaignBehavior.RomanceReservationDescription romanceReservationDescription3 = list4[0];
				PersuasionTask persuasionTask4 = new PersuasionTask(5);
				list.Add(persuasionTask4);
				persuasionTask4.SpokenLine = new TextObject("{=fAdwIqbg}I think you should try to win my family's approval.", null);
				persuasionTask4.FinalFailLine = new TextObject("{=Xa7PsIao}I am sorry. I will not marry without my family's blessing.", null);
				persuasionTask4.TryLaterLine = new TextObject("{=44tA6fNa}I will try to earn your family's trust, then.", null);
				PersuasionArgumentStrength persuasionArgumentStrength7 = PersuasionArgumentStrength.Normal;
				PersuasionOptionArgs persuasionOptionArgs22 = new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Generosity, TraitEffect.Positive, persuasionArgumentStrength7, false, new TextObject("{=563qB3ar}I can only hope that if they come to know my loyalty, they will accept me.", null), null, false, true, false);
				persuasionTask4.AddOptionToTask(persuasionOptionArgs22);
				if (flag)
				{
					PersuasionOptionArgs persuasionOptionArgs23 = new PersuasionOptionArgs(DefaultSkills.Leadership, DefaultTraits.Valor, TraitEffect.Positive, persuasionArgumentStrength7, true, new TextObject("{=LEsuGM8a}Let no one - not even your family - come between us!", null), null, false, true, false);
					persuasionTask4.AddOptionToTask(persuasionOptionArgs23);
				}
				PersuasionOptionArgs persuasionOptionArgs24 = new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Honor, TraitEffect.Positive, persuasionArgumentStrength7, false, new TextObject("{=ZbvbsA4i}I can only hope that if they come to know my virtues, they will accept me.", null), null, false, true, false);
				persuasionTask4.AddOptionToTask(persuasionOptionArgs24);
			}
			else if (list4.Count == 0 && list.Count < 3)
			{
				PersuasionTask persuasionTask5 = new PersuasionTask(7);
				list.Add(persuasionTask5);
				persuasionTask5.SpokenLine = new TextObject("{=HFkXIyCV}My family likes you...", null);
				persuasionTask5.FinalFailLine = new TextObject("{=3IBVEOwh}I still think we may not be ready yet.", null);
				persuasionTask5.TryLaterLine = new TextObject("{=44tA6fNa}I will try to earn your family's trust, then.", null);
				PersuasionArgumentStrength persuasionArgumentStrength8 = PersuasionArgumentStrength.ExtremelyEasy;
				PersuasionOptionArgs persuasionOptionArgs25 = new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Generosity, TraitEffect.Positive, persuasionArgumentStrength8, false, new TextObject("{=2LrFafpB}And I will respect and cherish your family.", null), null, false, true, false);
				persuasionTask5.AddOptionToTask(persuasionOptionArgs25);
				PersuasionOptionArgs persuasionOptionArgs26 = new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Calculating, TraitEffect.Positive, persuasionArgumentStrength8, false, new TextObject("{=BaifRgT5}That's useful to know for when it comes time to discuss the exchange of dowries.", null), null, false, true, false);
				persuasionTask5.AddOptionToTask(persuasionOptionArgs26);
			}
			return list;
		}

		// Token: 0x06003AA4 RID: 15012 RVA: 0x00111E58 File Offset: 0x00110058
		private bool conversation_courtship_initial_reaction_on_condition()
		{
			IEnumerable<RomanceCampaignBehavior.RomanceReservationDescription> romanceReservations = this.GetRomanceReservations(Hero.OneToOneConversationHero, Hero.MainHero);
			if (Romance.GetRomanticLevel(Hero.MainHero, Hero.OneToOneConversationHero) == Romance.RomanceLevelEnum.FailedInPracticalities || Romance.GetRomanticLevel(Hero.MainHero, Hero.OneToOneConversationHero) == Romance.RomanceLevelEnum.FailedInCompatibility)
			{
				return false;
			}
			MBTextManager.SetTextVariable("INITIAL_COURTSHIP_REACTION", romanceReservations.Any((RomanceCampaignBehavior.RomanceReservationDescription x) => x == RomanceCampaignBehavior.RomanceReservationDescription.AttractionIAmDrawnToYou) ? "{=WEkjz9tg}Ah! Yes... We are considering offers... Did you have someone in mind?" : "{=KdhnBhZ1}Yes, we are considering offers. These things are not rushed into.", false);
			return true;
		}

		// Token: 0x06003AA5 RID: 15013 RVA: 0x00111EDC File Offset: 0x001100DC
		private bool conversation_courtship_decline_reaction_to_player_on_condition()
		{
			if (Romance.GetRomanticLevel(Hero.MainHero, Hero.OneToOneConversationHero) == Romance.RomanceLevelEnum.FailedInPracticalities)
			{
				MBTextManager.SetTextVariable("COURTSHIP_DECLINE_REACTION", "{=emLBsWj6}I am terribly sorry. It is practically not possible for us to be married.", false);
				return true;
			}
			if (Romance.GetRomanticLevel(Hero.MainHero, Hero.OneToOneConversationHero) == Romance.RomanceLevelEnum.FailedInCompatibility)
			{
				MBTextManager.SetTextVariable("COURTSHIP_DECLINE_REACTION", "{=s7idfhBO}I am terribly sorry. We are not really compatible with each other.", false);
				return true;
			}
			return false;
		}

		// Token: 0x06003AA6 RID: 15014 RVA: 0x00111F34 File Offset: 0x00110134
		private bool conversation_courtship_reaction_to_player_on_condition()
		{
			IEnumerable<RomanceCampaignBehavior.RomanceReservationDescription> romanceReservations = this.GetRomanceReservations(Hero.OneToOneConversationHero, Hero.MainHero);
			bool flag = Hero.OneToOneConversationHero.GetTraitLevel(DefaultTraits.Generosity) + Hero.OneToOneConversationHero.GetTraitLevel(DefaultTraits.Mercy) > 0;
			TraitObject persona = Hero.OneToOneConversationHero.CharacterObject.GetPersona();
			bool flag2 = ConversationTagHelper.UsesHighRegister(Hero.OneToOneConversationHero.CharacterObject);
			if (romanceReservations.Any((RomanceCampaignBehavior.RomanceReservationDescription x) => x == RomanceCampaignBehavior.RomanceReservationDescription.AttractionIAmDrawnToYou))
			{
				if (persona == DefaultTraits.PersonaIronic && flag2)
				{
					MBTextManager.SetTextVariable("INITIAL_COURTSHIP_REACTION_TO_PLAYER", "{=5ao0RdRT}Well, I do not deny that there is something about you to which I am drawn.", false);
				}
				if (persona == DefaultTraits.PersonaIronic && !flag2)
				{
					MBTextManager.SetTextVariable("INITIAL_COURTSHIP_REACTION_TO_PLAYER", "{=r77ZrSUJ}You're straightforward. I like that.", false);
				}
				else if (persona == DefaultTraits.PersonaCurt)
				{
					MBTextManager.SetTextVariable("INITIAL_COURTSHIP_REACTION_TO_PLAYER", Hero.MainHero.IsFemale ? "{=YXCGUSYd}Mm. Well, you'd make a very unusual match. But, well, I won't rule it out." : "{=iKYSgoZx}You're a handsome devil, I'll give you that.", false);
				}
				else if (persona == DefaultTraits.PersonaEarnest)
				{
					MBTextManager.SetTextVariable("INITIAL_COURTSHIP_REACTION_TO_PLAYER", "{=UCjFAPnk}I am flattered, {?PLAYER.GENDER}my lady{?}sir{\\?}.", false);
				}
				else
				{
					MBTextManager.SetTextVariable("INITIAL_COURTSHIP_REACTION_TO_PLAYER", "{=8PwNj5tR}Yes... Yes. We should, em, discuss this.", false);
				}
			}
			else if (romanceReservations.Any((RomanceCampaignBehavior.RomanceReservationDescription x) => x == RomanceCampaignBehavior.RomanceReservationDescription.PropertyHowCanIMarryAnAdventuress))
			{
				MBTextManager.SetTextVariable("INITIAL_COURTSHIP_REACTION_TO_PLAYER", "{=YRN4RBeI}Very well, madame, but I would have you know.... I intend to marry someone of my own rank.", false);
			}
			else
			{
				if (!flag)
				{
					if (romanceReservations.Any((RomanceCampaignBehavior.RomanceReservationDescription x) => x == RomanceCampaignBehavior.RomanceReservationDescription.PropertyIWantRealWealth || x == RomanceCampaignBehavior.RomanceReservationDescription.PropertyWeNeedToBeComfortable))
					{
						MBTextManager.SetTextVariable("INITIAL_COURTSHIP_REACTION_TO_PLAYER", "{=P407baEa}I think you would need to rise considerably in the world before I could consider such a thing...", false);
						return true;
					}
				}
				if (flag)
				{
					if (romanceReservations.Any((RomanceCampaignBehavior.RomanceReservationDescription x) => x == RomanceCampaignBehavior.RomanceReservationDescription.PropertyIWantRealWealth))
					{
						MBTextManager.SetTextVariable("INITIAL_COURTSHIP_REACTION_TO_PLAYER", "{=gS1noLvf}I do not know whether to find that charming or impertinent...", false);
						return true;
					}
				}
				if (romanceReservations.Any((RomanceCampaignBehavior.RomanceReservationDescription x) => x == RomanceCampaignBehavior.RomanceReservationDescription.AttractionYoureNotMyType))
				{
					MBTextManager.SetTextVariable("INITIAL_COURTSHIP_REACTION_TO_PLAYER", "{=ltXu3DbR}Em... Yes, well, I suppose I can consider your offer.", false);
				}
				else if (romanceReservations.Any((RomanceCampaignBehavior.RomanceReservationDescription x) => x == RomanceCampaignBehavior.RomanceReservationDescription.FamilyApprovalIAmGladYouAreFriendsWithOurFamily))
				{
					MBTextManager.SetTextVariable("INITIAL_COURTSHIP_REACTION_TO_PLAYER", "{=UQtXV3kf}Certainly, you have always been close to our family.", false);
				}
				else
				{
					MBTextManager.SetTextVariable("INITIAL_COURTSHIP_REACTION_TO_PLAYER", "{=VYmQmqIv}We are considering many offers. You may certainly add your name to the list.", false);
				}
			}
			return true;
		}

		// Token: 0x06003AA7 RID: 15015 RVA: 0x0011219C File Offset: 0x0011039C
		private void conversation_fail_courtship_on_consequence()
		{
			if (Romance.GetRomanticLevel(Hero.MainHero, Hero.OneToOneConversationHero) == Romance.RomanceLevelEnum.CourtshipStarted)
			{
				ChangeRomanticStateAction.Apply(Hero.MainHero, Hero.OneToOneConversationHero, Romance.RomanceLevelEnum.FailedInCompatibility);
			}
			else if (Romance.GetRomanticLevel(Hero.MainHero, Hero.OneToOneConversationHero) == Romance.RomanceLevelEnum.CoupleDecidedThatTheyAreCompatible)
			{
				ChangeRomanticStateAction.Apply(Hero.MainHero, Hero.OneToOneConversationHero, Romance.RomanceLevelEnum.FailedInPracticalities);
			}
			if (PlayerEncounter.Current != null)
			{
				PlayerEncounter.LeaveEncounter = true;
			}
			this._allReservations = null;
			ConversationManager.EndPersuasion();
		}

		// Token: 0x06003AA8 RID: 15016 RVA: 0x00112208 File Offset: 0x00110408
		private void conversation_start_courtship_persuasion_pt1_on_consequence()
		{
			if (Romance.GetRomanticLevel(Hero.MainHero, Hero.OneToOneConversationHero) == Romance.RomanceLevelEnum.MatchMadeByFamily)
			{
				ChangeRomanticStateAction.Apply(Hero.MainHero, Hero.OneToOneConversationHero, Romance.RomanceLevelEnum.CourtshipStarted);
			}
			Hero hero = Hero.MainHero.MapFaction.Leader;
			if (Hero.MainHero.MapFaction == Hero.OneToOneConversationHero.MapFaction)
			{
				hero = Hero.MainHero;
			}
			this._allReservations = this.GetPersuasionTasksForCourtshipStage1(Hero.OneToOneConversationHero, hero);
			this._maximumScoreCap = (float)this._allReservations.Count * 1f;
			float num = 0f;
			foreach (PersuasionTask persuasionTask in this._allReservations)
			{
				foreach (PersuasionAttempt persuasionAttempt in this._previousRomancePersuasionAttempts)
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
			this.RemoveUnneededPersuasionAttempts();
			ConversationManager.StartPersuasion(this._maximumScoreCap, this._successValue, this._failValue, this._criticalSuccessValue, this._criticalFailValue, num, PersuasionDifficulty.Medium);
		}

		// Token: 0x06003AA9 RID: 15017 RVA: 0x001123A0 File Offset: 0x001105A0
		private void conversation_courtship_stage_1_success_on_consequence()
		{
			Romance.RomanticState romanticState = Romance.GetRomanticState(Hero.MainHero, Hero.OneToOneConversationHero);
			float num = ConversationManager.GetPersuasionProgress() - ConversationManager.GetPersuasionGoalValue();
			romanticState.ScoreFromPersuasion = num;
			this._allReservations = null;
			ConversationManager.EndPersuasion();
			ChangeRomanticStateAction.Apply(Hero.MainHero, Hero.OneToOneConversationHero, Romance.RomanceLevelEnum.CoupleDecidedThatTheyAreCompatible);
		}

		// Token: 0x06003AAA RID: 15018 RVA: 0x001123EC File Offset: 0x001105EC
		private void conversation_courtship_stage_2_success_on_consequence()
		{
			Romance.RomanticState romanticState = Romance.GetRomanticState(Hero.MainHero, Hero.OneToOneConversationHero);
			float num = ConversationManager.GetPersuasionProgress() - ConversationManager.GetPersuasionGoalValue();
			romanticState.ScoreFromPersuasion += num;
			this._allReservations = null;
			ConversationManager.EndPersuasion();
			ChangeRomanticStateAction.Apply(Hero.MainHero, Hero.OneToOneConversationHero, Romance.RomanceLevelEnum.CoupleAgreedOnMarriage);
		}

		// Token: 0x06003AAB RID: 15019 RVA: 0x00112440 File Offset: 0x00110640
		private void conversation_continue_courtship_stage_2_on_consequence()
		{
			Hero hero = Hero.MainHero.MapFaction.Leader;
			if (Hero.MainHero.MapFaction == Hero.OneToOneConversationHero.MapFaction)
			{
				hero = Hero.MainHero;
			}
			this._allReservations = this.GetPersuasionTasksForCourtshipStage2(Hero.OneToOneConversationHero, hero);
			this._maximumScoreCap = (float)this._allReservations.Count * 1f;
			float num = 0f;
			foreach (PersuasionTask persuasionTask in this._allReservations)
			{
				foreach (PersuasionAttempt persuasionAttempt in this._previousRomancePersuasionAttempts)
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
			this.RemoveUnneededPersuasionAttempts();
			ConversationManager.StartPersuasion(this._maximumScoreCap, this._successValue, this._failValue, this._criticalSuccessValue, this._criticalFailValue, num, PersuasionDifficulty.Medium);
		}

		// Token: 0x06003AAC RID: 15020 RVA: 0x001125B8 File Offset: 0x001107B8
		private bool conversation_check_if_unmet_reservation_on_condition()
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

		// Token: 0x06003AAD RID: 15021 RVA: 0x00112630 File Offset: 0x00110830
		private bool conversation_lord_player_has_failed_in_courtship_on_condition()
		{
			PersuasionTask currentPersuasionTask = this.GetCurrentPersuasionTask();
			if (currentPersuasionTask.Options.All((PersuasionOptionArgs x) => x.IsBlocked) && !ConversationManager.GetPersuasionProgressSatisfied())
			{
				MBTextManager.SetTextVariable("FAILED_PERSUASION_LINE", currentPersuasionTask.FinalFailLine, false);
				return true;
			}
			return false;
		}

		// Token: 0x06003AAE RID: 15022 RVA: 0x0011268C File Offset: 0x0011088C
		private bool conversation_courtship_persuasion_option_1_on_condition()
		{
			PersuasionTask currentPersuasionTask = this.GetCurrentPersuasionTask();
			if (currentPersuasionTask.Options.Count > 0)
			{
				TextObject textObject = new TextObject("{=bSo9hKwr}{PERSUASION_OPTION_LINE} {SUCCESS_CHANCE}", null);
				textObject.SetTextVariable("SUCCESS_CHANCE", PersuasionHelper.ShowSuccess(currentPersuasionTask.Options.ElementAt(0), true));
				textObject.SetTextVariable("PERSUASION_OPTION_LINE", currentPersuasionTask.Options.ElementAt(0).Line);
				MBTextManager.SetTextVariable("ROMANCE_PERSUADE_ATTEMPT_1", textObject, false);
				return true;
			}
			return false;
		}

		// Token: 0x06003AAF RID: 15023 RVA: 0x00112704 File Offset: 0x00110904
		private bool conversation_courtship_persuasion_option_2_on_condition()
		{
			PersuasionTask currentPersuasionTask = this.GetCurrentPersuasionTask();
			if (currentPersuasionTask.Options.Count > 1)
			{
				TextObject textObject = new TextObject("{=bSo9hKwr}{PERSUASION_OPTION_LINE} {SUCCESS_CHANCE}", null);
				textObject.SetTextVariable("SUCCESS_CHANCE", PersuasionHelper.ShowSuccess(currentPersuasionTask.Options.ElementAt(1), true));
				textObject.SetTextVariable("PERSUASION_OPTION_LINE", currentPersuasionTask.Options.ElementAt(1).Line);
				MBTextManager.SetTextVariable("ROMANCE_PERSUADE_ATTEMPT_2", textObject, false);
				return true;
			}
			return false;
		}

		// Token: 0x06003AB0 RID: 15024 RVA: 0x0011277C File Offset: 0x0011097C
		private bool conversation_courtship_persuasion_option_3_on_condition()
		{
			PersuasionTask currentPersuasionTask = this.GetCurrentPersuasionTask();
			if (currentPersuasionTask.Options.Count > 2)
			{
				TextObject textObject = new TextObject("{=bSo9hKwr}{PERSUASION_OPTION_LINE} {SUCCESS_CHANCE}", null);
				textObject.SetTextVariable("SUCCESS_CHANCE", PersuasionHelper.ShowSuccess(currentPersuasionTask.Options.ElementAt(2), true));
				textObject.SetTextVariable("PERSUASION_OPTION_LINE", currentPersuasionTask.Options.ElementAt(2).Line);
				MBTextManager.SetTextVariable("ROMANCE_PERSUADE_ATTEMPT_3", textObject, false);
				return true;
			}
			return false;
		}

		// Token: 0x06003AB1 RID: 15025 RVA: 0x001127F4 File Offset: 0x001109F4
		private bool conversation_courtship_persuasion_option_4_on_condition()
		{
			PersuasionTask currentPersuasionTask = this.GetCurrentPersuasionTask();
			if (currentPersuasionTask.Options.Count > 3)
			{
				TextObject textObject = new TextObject("{=bSo9hKwr}{PERSUASION_OPTION_LINE} {SUCCESS_CHANCE}", null);
				textObject.SetTextVariable("SUCCESS_CHANCE", PersuasionHelper.ShowSuccess(currentPersuasionTask.Options.ElementAt(3), true));
				textObject.SetTextVariable("PERSUASION_OPTION_LINE", currentPersuasionTask.Options.ElementAt(3).Line);
				MBTextManager.SetTextVariable("ROMANCE_PERSUADE_ATTEMPT_4", textObject, false);
				return true;
			}
			return false;
		}

		// Token: 0x06003AB2 RID: 15026 RVA: 0x0011286C File Offset: 0x00110A6C
		private void conversation_romance_1_persuade_option_on_consequence()
		{
			PersuasionTask currentPersuasionTask = this.GetCurrentPersuasionTask();
			if (currentPersuasionTask.Options.Count > 0)
			{
				currentPersuasionTask.Options[0].BlockTheOption(true);
			}
		}

		// Token: 0x06003AB3 RID: 15027 RVA: 0x001128A0 File Offset: 0x00110AA0
		private void conversation_romance_2_persuade_option_on_consequence()
		{
			PersuasionTask currentPersuasionTask = this.GetCurrentPersuasionTask();
			if (currentPersuasionTask.Options.Count > 1)
			{
				currentPersuasionTask.Options[1].BlockTheOption(true);
			}
		}

		// Token: 0x06003AB4 RID: 15028 RVA: 0x001128D4 File Offset: 0x00110AD4
		private void conversation_romance_3_persuade_option_on_consequence()
		{
			PersuasionTask currentPersuasionTask = this.GetCurrentPersuasionTask();
			if (currentPersuasionTask.Options.Count > 2)
			{
				currentPersuasionTask.Options[2].BlockTheOption(true);
			}
		}

		// Token: 0x06003AB5 RID: 15029 RVA: 0x00112908 File Offset: 0x00110B08
		private void conversation_romance_4_persuade_option_on_consequence()
		{
			PersuasionTask currentPersuasionTask = this.GetCurrentPersuasionTask();
			if (currentPersuasionTask.Options.Count > 3)
			{
				currentPersuasionTask.Options[3].BlockTheOption(true);
			}
		}

		// Token: 0x06003AB6 RID: 15030 RVA: 0x0011293C File Offset: 0x00110B3C
		private bool RomancePersuasionOption1ClickableOnCondition1(out TextObject hintText)
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

		// Token: 0x06003AB7 RID: 15031 RVA: 0x00112988 File Offset: 0x00110B88
		private bool RomancePersuasionOption2ClickableOnCondition2(out TextObject hintText)
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

		// Token: 0x06003AB8 RID: 15032 RVA: 0x001129D4 File Offset: 0x00110BD4
		private bool RomancePersuasionOption3ClickableOnCondition3(out TextObject hintText)
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

		// Token: 0x06003AB9 RID: 15033 RVA: 0x00112A20 File Offset: 0x00110C20
		private bool RomancePersuasionOption4ClickableOnCondition4(out TextObject hintText)
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

		// Token: 0x06003ABA RID: 15034 RVA: 0x00112A6C File Offset: 0x00110C6C
		private PersuasionOptionArgs SetupCourtshipPersuasionOption1()
		{
			return this.GetCurrentPersuasionTask().Options.ElementAt(0);
		}

		// Token: 0x06003ABB RID: 15035 RVA: 0x00112A7F File Offset: 0x00110C7F
		private PersuasionOptionArgs SetupCourtshipPersuasionOption2()
		{
			return this.GetCurrentPersuasionTask().Options.ElementAt(1);
		}

		// Token: 0x06003ABC RID: 15036 RVA: 0x00112A92 File Offset: 0x00110C92
		private PersuasionOptionArgs SetupCourtshipPersuasionOption3()
		{
			return this.GetCurrentPersuasionTask().Options.ElementAt(2);
		}

		// Token: 0x06003ABD RID: 15037 RVA: 0x00112AA5 File Offset: 0x00110CA5
		private PersuasionOptionArgs SetupCourtshipPersuasionOption4()
		{
			return this.GetCurrentPersuasionTask().Options.ElementAt(3);
		}

		// Token: 0x06003ABE RID: 15038 RVA: 0x00112AB8 File Offset: 0x00110CB8
		private bool conversation_player_eligible_for_marriage_with_conversation_hero_on_condition()
		{
			return Hero.MainHero.Spouse == null && Hero.OneToOneConversationHero != null && this.MarriageCourtshipPossibility(Hero.MainHero, Hero.OneToOneConversationHero);
		}

		// Token: 0x06003ABF RID: 15039 RVA: 0x00112ADF File Offset: 0x00110CDF
		private bool conversation_player_eligible_for_marriage_with_hero_rltv_on_condition()
		{
			return Hero.MainHero.Spouse == null && Hero.OneToOneConversationHero != null;
		}

		// Token: 0x06003AC0 RID: 15040 RVA: 0x00112AF7 File Offset: 0x00110CF7
		private void conversation_find_player_relatives_eligible_for_marriage_on_consequence()
		{
			ConversationSentence.SetObjectsToRepeatOver(this.FindPlayerRelativesEligibleForMarriage(Hero.OneToOneConversationHero.Clan).ToList<CharacterObject>(), 5);
		}

		// Token: 0x06003AC1 RID: 15041 RVA: 0x00112B14 File Offset: 0x00110D14
		private void conversation_player_nominates_self_for_marriage_on_consequence()
		{
			this._playerProposalHero = Hero.MainHero;
		}

		// Token: 0x06003AC2 RID: 15042 RVA: 0x00112B24 File Offset: 0x00110D24
		private void conversation_player_nominates_marriage_relative_on_consequence()
		{
			CharacterObject characterObject = ConversationSentence.SelectedRepeatObject as CharacterObject;
			this._playerProposalHero = characterObject.HeroObject;
		}

		// Token: 0x06003AC3 RID: 15043 RVA: 0x00112B48 File Offset: 0x00110D48
		private bool conversation_player_relative_eligible_for_marriage_on_condition()
		{
			CharacterObject characterObject = ConversationSentence.CurrentProcessedRepeatObject as CharacterObject;
			TextObject selectedRepeatLine = ConversationSentence.SelectedRepeatLine;
			if (characterObject != null)
			{
				StringHelpers.SetRepeatableCharacterProperties("MARRIAGE_CANDIDATE", characterObject, false);
				return true;
			}
			return false;
		}

		// Token: 0x06003AC4 RID: 15044 RVA: 0x00112B78 File Offset: 0x00110D78
		private bool conversation_propose_clan_leader_for_player_nomination_on_condition()
		{
			foreach (Hero hero in Hero.OneToOneConversationHero.Clan.Lords.OrderByDescending((Hero x) => x.Age))
			{
				if (this.MarriageCourtshipPossibility(this._playerProposalHero, hero) && hero.CharacterObject == Hero.OneToOneConversationHero.CharacterObject)
				{
					this._proposedSpouseForPlayerRelative = hero;
					return true;
				}
			}
			return false;
		}

		// Token: 0x06003AC5 RID: 15045 RVA: 0x00112C1C File Offset: 0x00110E1C
		private bool conversation_propose_spouse_for_player_nomination_on_condition()
		{
			foreach (Hero hero in Hero.OneToOneConversationHero.Clan.Lords.OrderByDescending((Hero x) => x.Age))
			{
				if (this.MarriageCourtshipPossibility(this._playerProposalHero, hero) && hero != Hero.OneToOneConversationHero)
				{
					this._proposedSpouseForPlayerRelative = hero;
					TextObject textObject = new TextObject("{=TjAQbTab}Well, yes, we are looking for a suitable marriage for { OTHER_CLAN_NOMINEE.LINK}.", null);
					hero.SetPropertiesToTextObject(textObject, "OTHER_CLAN_NOMINEE");
					MBTextManager.SetTextVariable("ARRANGE_MARRIAGE_LINE", textObject, false);
					return true;
				}
			}
			return false;
		}

		// Token: 0x06003AC6 RID: 15046 RVA: 0x00112CD8 File Offset: 0x00110ED8
		private bool conversation_player_rltv_agrees_on_courtship_on_condition()
		{
			Hero courtedHeroInOtherClan = Romance.GetCourtedHeroInOtherClan(this._playerProposalHero, this._proposedSpouseForPlayerRelative);
			return courtedHeroInOtherClan == null || courtedHeroInOtherClan == this._proposedSpouseForPlayerRelative;
		}

		// Token: 0x06003AC7 RID: 15047 RVA: 0x00112D05 File Offset: 0x00110F05
		private void conversation_player_agrees_on_courtship_on_consequence()
		{
			ChangeRomanticStateAction.Apply(this._playerProposalHero, this._proposedSpouseForPlayerRelative, Romance.RomanceLevelEnum.MatchMadeByFamily);
		}

		// Token: 0x06003AC8 RID: 15048 RVA: 0x00112D1C File Offset: 0x00110F1C
		private void conversation_lord_propose_marriage_to_clan_leader_confirm_consequences()
		{
			MarriageBarterable marriageBarterable = new MarriageBarterable(Hero.MainHero, PartyBase.MainParty, this._playerProposalHero, this._proposedSpouseForPlayerRelative);
			BarterManager instance = BarterManager.Instance;
			Hero mainHero = Hero.MainHero;
			Hero oneToOneConversationHero = Hero.OneToOneConversationHero;
			PartyBase mainParty = PartyBase.MainParty;
			MobileParty partyBelongedTo = Hero.OneToOneConversationHero.PartyBelongedTo;
			instance.StartBarterOffer(mainHero, oneToOneConversationHero, mainParty, (partyBelongedTo != null) ? partyBelongedTo.Party : null, null, (Barterable barterableObj, BarterData _args, object obj) => BarterManager.Instance.InitializeMarriageBarterContext(barterableObj, _args, new Tuple<Hero, Hero>(this._playerProposalHero, this._proposedSpouseForPlayerRelative)), 0, false, new Barterable[] { marriageBarterable });
		}

		// Token: 0x06003AC9 RID: 15049 RVA: 0x00112D90 File Offset: 0x00110F90
		private bool conversation_romance_blocked_on_condition()
		{
			if (Hero.OneToOneConversationHero == null)
			{
				return false;
			}
			Romance.RomanceLevelEnum romanticLevel = Romance.GetRomanticLevel(Hero.MainHero, Hero.OneToOneConversationHero);
			if (!this.MarriageCourtshipPossibility(Hero.MainHero, Hero.OneToOneConversationHero) && romanticLevel >= Romance.RomanceLevelEnum.MatchMadeByFamily && romanticLevel < Romance.RomanceLevelEnum.Marriage)
			{
				MBTextManager.SetTextVariable("ROMANCE_BLOCKED_REASON", "{=BQn8yTs5}Ah, yes. I am afraid I can no longer entertain your proposal, at least not for now.", false);
				if (FactionManager.IsAtWarAgainstFaction(Hero.MainHero.MapFaction, Hero.OneToOneConversationHero.MapFaction))
				{
					MBTextManager.SetTextVariable("ROMANCE_BLOCKED_REASON", "{=wNxhmNOc}I am afraid I cannot entertain such a proposal so long as we are at war.", false);
				}
				else if (Hero.OneToOneConversationHero.Clan.Leader == Hero.OneToOneConversationHero)
				{
					MBTextManager.SetTextVariable("ROMANCE_BLOCKED_REASON", "{=1FcxAGWU}Ah, yes. I am afraid I can no longer entertain such a proposal. I am now the head of my family, and the factors that we must consider have changed. You would need to place your property under my control, and I do not think that you would accept that.", false);
				}
				ChangeRomanticStateAction.Apply(Hero.MainHero, Hero.OneToOneConversationHero, Romance.RomanceLevelEnum.FailedInCompatibility);
				return true;
			}
			return false;
		}

		// Token: 0x06003ACA RID: 15050 RVA: 0x00112E48 File Offset: 0x00111048
		private bool conversation_romance_at_stage_1_discussions_on_condition()
		{
			if (Hero.OneToOneConversationHero == null)
			{
				return false;
			}
			Romance.RomanceLevelEnum romanticLevel = Romance.GetRomanticLevel(Hero.MainHero, Hero.OneToOneConversationHero);
			if (this.MarriageCourtshipPossibility(Hero.MainHero, Hero.OneToOneConversationHero) && (romanticLevel == Romance.RomanceLevelEnum.CourtshipStarted || romanticLevel == Romance.RomanceLevelEnum.MatchMadeByFamily))
			{
				List<PersuasionAttempt> list = (from x in this._previousRomancePersuasionAttempts
					where x.PersuadedHero == Hero.OneToOneConversationHero
					orderby x.GameTime descending
					select x).ToList<PersuasionAttempt>();
				if (list.Count == 0 || list[0].GameTime < this.RomanceCourtshipAttemptCooldown)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06003ACB RID: 15051 RVA: 0x00112F00 File Offset: 0x00111100
		private bool conversation_romance_at_stage_2_discussions_on_condition()
		{
			if (Hero.OneToOneConversationHero == null)
			{
				return false;
			}
			Romance.RomanceLevelEnum romanticLevel = Romance.GetRomanticLevel(Hero.MainHero, Hero.OneToOneConversationHero);
			if (this.MarriageCourtshipPossibility(Hero.MainHero, Hero.OneToOneConversationHero) && romanticLevel == Romance.RomanceLevelEnum.CoupleDecidedThatTheyAreCompatible)
			{
				List<PersuasionAttempt> list = (from x in this._previousRomancePersuasionAttempts
					where x.PersuadedHero == Hero.OneToOneConversationHero
					orderby x.GameTime descending
					select x).ToList<PersuasionAttempt>();
				if (list.Count == 0 || list[0].GameTime < this.RomanceCourtshipAttemptCooldown)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06003ACC RID: 15052 RVA: 0x00112FB4 File Offset: 0x001111B4
		private bool conversation_finalize_courtship_for_hero_on_condition()
		{
			return this.MarriageCourtshipPossibility(Hero.MainHero, Hero.OneToOneConversationHero) && Hero.OneToOneConversationHero.Clan.Leader == Hero.OneToOneConversationHero && Romance.GetRomanticLevel(Hero.MainHero, Hero.OneToOneConversationHero) == Romance.RomanceLevelEnum.CoupleAgreedOnMarriage;
		}

		// Token: 0x06003ACD RID: 15053 RVA: 0x00112FF4 File Offset: 0x001111F4
		private bool conversation_finalize_courtship_for_other_on_condition()
		{
			if (Hero.OneToOneConversationHero != null)
			{
				Clan clan = Hero.OneToOneConversationHero.Clan;
				if (((clan != null) ? clan.Leader : null) == Hero.OneToOneConversationHero && !Hero.OneToOneConversationHero.IsPrisoner)
				{
					foreach (Hero hero in Hero.OneToOneConversationHero.Clan.Lords)
					{
						if (hero != Hero.OneToOneConversationHero && this.MarriageCourtshipPossibility(Hero.MainHero, hero) && Romance.GetRomanticLevel(Hero.MainHero, hero) == Romance.RomanceLevelEnum.CoupleAgreedOnMarriage)
						{
							MBTextManager.SetTextVariable("COURTSHIP_PARTNER", hero.Name, false);
							return true;
						}
					}
					return false;
				}
			}
			return false;
		}

		// Token: 0x06003ACE RID: 15054 RVA: 0x001130B8 File Offset: 0x001112B8
		private bool conversation_discuss_marriage_alliance_on_condition()
		{
			if (Hero.OneToOneConversationHero != null)
			{
				IFaction mapFaction = Hero.OneToOneConversationHero.MapFaction;
				if ((mapFaction == null || !mapFaction.IsMinorFaction) && !Hero.OneToOneConversationHero.IsPrisoner)
				{
					if (FactionManager.IsAtWarAgainstFaction(Hero.MainHero.MapFaction, Hero.OneToOneConversationHero.MapFaction))
					{
						return false;
					}
					if (Hero.OneToOneConversationHero.Clan == null || Hero.OneToOneConversationHero.Clan.Leader != Hero.OneToOneConversationHero)
					{
						return false;
					}
					bool flag = false;
					foreach (Hero hero in Hero.MainHero.Clan.Heroes)
					{
						foreach (Hero hero2 in Hero.OneToOneConversationHero.Clan.Lords)
						{
							if (this.MarriageCourtshipPossibility(hero, hero2))
							{
								flag = true;
							}
						}
					}
					return flag;
				}
			}
			return false;
		}

		// Token: 0x06003ACF RID: 15055 RVA: 0x001131D0 File Offset: 0x001113D0
		private bool conversation_player_can_open_courtship_on_condition()
		{
			if (Hero.OneToOneConversationHero != null)
			{
				IFaction mapFaction = Hero.OneToOneConversationHero.MapFaction;
				if ((mapFaction == null || !mapFaction.IsMinorFaction) && !Hero.OneToOneConversationHero.IsPrisoner)
				{
					if (this.MarriageCourtshipPossibility(Hero.MainHero, Hero.OneToOneConversationHero) && Romance.GetRomanticLevel(Hero.MainHero, Hero.OneToOneConversationHero) == Romance.RomanceLevelEnum.Untested)
					{
						if (Hero.MainHero.IsFemale)
						{
							MBTextManager.SetTextVariable("FLIRTATION_LINE", "{=bjJs0eeB}My lord, I note that you have not yet taken a wife.", false);
						}
						else
						{
							MBTextManager.SetTextVariable("FLIRTATION_LINE", "{=v1hC6Aem}My lady, I wish to profess myself your most ardent admirer.", false);
						}
						return true;
					}
					if (Romance.GetRomanticLevel(Hero.MainHero, Hero.OneToOneConversationHero) == Romance.RomanceLevelEnum.FailedInCompatibility || Romance.GetRomanticLevel(Hero.MainHero, Hero.OneToOneConversationHero) == Romance.RomanceLevelEnum.FailedInPracticalities)
					{
						if (Hero.MainHero.IsFemale)
						{
							MBTextManager.SetTextVariable("FLIRTATION_LINE", "{=2WnhUBMM}My lord, may you give me another chance to prove myself?", false);
						}
						else
						{
							MBTextManager.SetTextVariable("FLIRTATION_LINE", "{=4iTaEZKg}My lady, may you give me another chance to prove myself?", false);
						}
						return true;
					}
					return false;
				}
			}
			return false;
		}

		// Token: 0x06003AD0 RID: 15056 RVA: 0x001132B2 File Offset: 0x001114B2
		private bool conversation_player_opens_courtship_on_condition()
		{
			return this._playerProposalHero == Hero.MainHero;
		}

		// Token: 0x06003AD1 RID: 15057 RVA: 0x001132C1 File Offset: 0x001114C1
		private void conversation_player_opens_courtship_on_consequence()
		{
			if (Romance.GetRomanticLevel(Hero.MainHero, Hero.OneToOneConversationHero) != Romance.RomanceLevelEnum.FailedInCompatibility && Romance.GetRomanticLevel(Hero.MainHero, Hero.OneToOneConversationHero) != Romance.RomanceLevelEnum.FailedInPracticalities)
			{
				ChangeRomanticStateAction.Apply(Hero.MainHero, Hero.OneToOneConversationHero, Romance.RomanceLevelEnum.CourtshipStarted);
			}
		}

		// Token: 0x06003AD2 RID: 15058 RVA: 0x001132F8 File Offset: 0x001114F8
		private bool conversation_courtship_try_later_on_condition()
		{
			PersuasionTask currentPersuasionTask = this.GetCurrentPersuasionTask();
			MBTextManager.SetTextVariable("TRY_HARDER_LINE", currentPersuasionTask.TryLaterLine, false);
			return true;
		}

		// Token: 0x06003AD3 RID: 15059 RVA: 0x00113320 File Offset: 0x00111520
		private bool conversation_courtship_reaction_stage_1_on_condition()
		{
			PersuasionOptionResult item = ConversationManager.GetPersuasionChosenOptions().Last<Tuple<PersuasionOptionArgs, PersuasionOptionResult>>().Item2;
			if (Romance.GetRomanticLevel(Hero.MainHero, Hero.OneToOneConversationHero) == Romance.RomanceLevelEnum.CourtshipStarted)
			{
				if ((item == PersuasionOptionResult.Failure || item == PersuasionOptionResult.CriticalFailure) && this.GetCurrentPersuasionTask().ImmediateFailLine != null)
				{
					MBTextManager.SetTextVariable("PERSUASION_REACTION", this.GetCurrentPersuasionTask().ImmediateFailLine, false);
					if (item != PersuasionOptionResult.CriticalFailure)
					{
						return true;
					}
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
				return true;
			}
			return false;
		}

		// Token: 0x06003AD4 RID: 15060 RVA: 0x001133D4 File Offset: 0x001115D4
		private bool conversation_marriage_barter_successful_on_condition()
		{
			return Campaign.Current.BarterManager.LastBarterIsAccepted;
		}

		// Token: 0x06003AD5 RID: 15061 RVA: 0x001133E8 File Offset: 0x001115E8
		private void conversation_marriage_barter_successful_on_consequence()
		{
			foreach (PersuasionAttempt persuasionAttempt in this._previousRomancePersuasionAttempts)
			{
				if (persuasionAttempt.PersuadedHero == Hero.OneToOneConversationHero || Hero.OneToOneConversationHero.Clan.Lords.Contains(persuasionAttempt.PersuadedHero))
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
		}

		// Token: 0x06003AD6 RID: 15062 RVA: 0x001134F8 File Offset: 0x001116F8
		private bool conversation_courtship_reaction_stage_2_on_condition()
		{
			PersuasionOptionResult item = ConversationManager.GetPersuasionChosenOptions().Last<Tuple<PersuasionOptionArgs, PersuasionOptionResult>>().Item2;
			if (item == PersuasionOptionResult.Success)
			{
				MBTextManager.SetTextVariable("PERSUASION_REACTION", "{=KWBzmJQl}I am happy to hear that.", false);
			}
			else if (item == PersuasionOptionResult.CriticalSuccess)
			{
				MBTextManager.SetTextVariable("PERSUASION_REACTION", "{=RGZWdKDx}Ah. It makes me so glad to hear you say that!", false);
			}
			else if ((item == PersuasionOptionResult.Failure || item == PersuasionOptionResult.CriticalFailure) && this.GetCurrentPersuasionTask().ImmediateFailLine != null)
			{
				MBTextManager.SetTextVariable("PERSUASION_REACTION", this.GetCurrentPersuasionTask().ImmediateFailLine, false);
			}
			else if (item == PersuasionOptionResult.Failure)
			{
				MBTextManager.SetTextVariable("PERSUASION_REACTION", "{=OqqUatT9}I... I think this will be difficult. Perhaps we are not meant for each other.", false);
			}
			else if (item == PersuasionOptionResult.CriticalFailure)
			{
				MBTextManager.SetTextVariable("PERSUASION_REACTION", "{=APSE3Q6r}What? No... I cannot, I cannot agree.", false);
				foreach (PersuasionTask persuasionTask in this._allReservations)
				{
					persuasionTask.BlockAllOptions();
				}
			}
			return true;
		}

		// Token: 0x06003AD7 RID: 15063 RVA: 0x001135E0 File Offset: 0x001117E0
		private void conversation_lord_persuade_option_reaction_on_consequence()
		{
			PersuasionTask currentPersuasionTask = this.GetCurrentPersuasionTask();
			Tuple<PersuasionOptionArgs, PersuasionOptionResult> tuple = ConversationManager.GetPersuasionChosenOptions().Last<Tuple<PersuasionOptionArgs, PersuasionOptionResult>>();
			float difficulty = Campaign.Current.Models.PersuasionModel.GetDifficulty(PersuasionDifficulty.Medium);
			float num;
			float num2;
			Campaign.Current.Models.PersuasionModel.GetEffectChances(tuple.Item1, out num, out num2, difficulty);
			this.FindTaskOfOption(tuple.Item1).ApplyEffects(num, num2);
			PersuasionAttempt persuasionAttempt = new PersuasionAttempt(Hero.OneToOneConversationHero, CampaignTime.Now, tuple.Item1, tuple.Item2, currentPersuasionTask.ReservationType);
			this._previousRomancePersuasionAttempts.Add(persuasionAttempt);
		}

		// Token: 0x06003AD8 RID: 15064 RVA: 0x00113678 File Offset: 0x00111878
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

		// Token: 0x06003AD9 RID: 15065 RVA: 0x00113714 File Offset: 0x00111914
		private List<CharacterObject> FindPlayerRelativesEligibleForMarriage(Clan withClan)
		{
			List<CharacterObject> list = new List<CharacterObject>();
			MarriageModel marriageModel = Campaign.Current.Models.MarriageModel;
			using (List<Hero>.Enumerator enumerator = Hero.MainHero.Clan.Lords.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Hero characterRelative = enumerator.Current;
					IEnumerable<Hero> enumerable = withClan.Lords.Where((Hero x) => marriageModel.IsCoupleSuitableForMarriage(x, characterRelative));
					if (characterRelative != Hero.MainHero && enumerable.Any<Hero>())
					{
						list.Add(characterRelative.CharacterObject);
					}
				}
			}
			return list;
		}

		// Token: 0x06003ADA RID: 15066 RVA: 0x001137E0 File Offset: 0x001119E0
		private TextObject ShowSuccess(PersuasionOptionArgs optionArgs)
		{
			return TextObject.Empty;
		}

		// Token: 0x06003ADB RID: 15067 RVA: 0x001137E7 File Offset: 0x001119E7
		private bool MarriageCourtshipPossibility(Hero person1, Hero person2)
		{
			return Campaign.Current.Models.MarriageModel.IsCoupleSuitableForMarriage(person1, person2) && !FactionManager.IsAtWarAgainstFaction(person1.MapFaction, person2.MapFaction);
		}

		// Token: 0x04001203 RID: 4611
		private const PersuasionDifficulty _difficulty = PersuasionDifficulty.Medium;

		// Token: 0x04001204 RID: 4612
		private List<PersuasionTask> _allReservations;

		// Token: 0x04001205 RID: 4613
		[SaveableField(1)]
		private List<PersuasionAttempt> _previousRomancePersuasionAttempts;

		// Token: 0x04001206 RID: 4614
		private Hero _playerProposalHero;

		// Token: 0x04001207 RID: 4615
		private Hero _proposedSpouseForPlayerRelative;

		// Token: 0x04001208 RID: 4616
		private float _maximumScoreCap;

		// Token: 0x04001209 RID: 4617
		private float _successValue = 1f;

		// Token: 0x0400120A RID: 4618
		private float _criticalSuccessValue = 2f;

		// Token: 0x0400120B RID: 4619
		private float _criticalFailValue = 2f;

		// Token: 0x0400120C RID: 4620
		private float _failValue;

		// Token: 0x0200071C RID: 1820
		public enum RomanticPreference
		{
			// Token: 0x04001D49 RID: 7497
			Conventional,
			// Token: 0x04001D4A RID: 7498
			Moralist,
			// Token: 0x04001D4B RID: 7499
			AttractedToBravery,
			// Token: 0x04001D4C RID: 7500
			Macchiavellian,
			// Token: 0x04001D4D RID: 7501
			Romantic,
			// Token: 0x04001D4E RID: 7502
			Companionship,
			// Token: 0x04001D4F RID: 7503
			MadAndBad,
			// Token: 0x04001D50 RID: 7504
			Security,
			// Token: 0x04001D51 RID: 7505
			PreferencesEnd
		}

		// Token: 0x0200071D RID: 1821
		private enum RomanceReservationType
		{
			// Token: 0x04001D53 RID: 7507
			TravelChat,
			// Token: 0x04001D54 RID: 7508
			TravelLesson,
			// Token: 0x04001D55 RID: 7509
			Aspirations,
			// Token: 0x04001D56 RID: 7510
			Compatibility,
			// Token: 0x04001D57 RID: 7511
			Attraction,
			// Token: 0x04001D58 RID: 7512
			Family,
			// Token: 0x04001D59 RID: 7513
			MaterialWealth,
			// Token: 0x04001D5A RID: 7514
			NoObjection
		}

		// Token: 0x0200071E RID: 1822
		private enum RomanceReservationDescription
		{
			// Token: 0x04001D5C RID: 7516
			CompatibilityINeedSomeoneUpright,
			// Token: 0x04001D5D RID: 7517
			CompatibilityNeedSomethingInCommon,
			// Token: 0x04001D5E RID: 7518
			CompatibiliyINeedSomeoneDangerous,
			// Token: 0x04001D5F RID: 7519
			CompatibilityStrongPoliticalBeliefs,
			// Token: 0x04001D60 RID: 7520
			AttractionYoureNotMyType,
			// Token: 0x04001D61 RID: 7521
			AttractionYoureGoodEnough,
			// Token: 0x04001D62 RID: 7522
			AttractionIAmDrawnToYou,
			// Token: 0x04001D63 RID: 7523
			PropertyYouSeemRichEnough,
			// Token: 0x04001D64 RID: 7524
			PropertyWeNeedToBeComfortable,
			// Token: 0x04001D65 RID: 7525
			PropertyIWantRealWealth,
			// Token: 0x04001D66 RID: 7526
			PropertyHowCanIMarryAnAdventuress,
			// Token: 0x04001D67 RID: 7527
			FamilyApprovalIAmGladYouAreFriendsWithOurFamily,
			// Token: 0x04001D68 RID: 7528
			FamilyApprovalYouNeedToBeFriendsWithOurFamily,
			// Token: 0x04001D69 RID: 7529
			FamilyApprovalHowCanYouBeEnemiesWithOurFamily,
			// Token: 0x04001D6A RID: 7530
			FamilyApprovalItWouldBeBestToBefriendOurFamily
		}
	}
}
