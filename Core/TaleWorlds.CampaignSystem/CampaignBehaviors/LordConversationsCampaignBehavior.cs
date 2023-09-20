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
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Inventory;
using TaleWorlds.CampaignSystem.Issues;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Workshops;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	public class LordConversationsCampaignBehavior : CampaignBehaviorBase
	{
		public bool GetConversationHeroPoliticalPhilosophy(out TextObject philosophyString)
		{
			return GameTexts.TryGetText("str_political_philosophy_" + Hero.OneToOneConversationHero.StringId + "_for_" + Hero.OneToOneConversationHero.MapFaction.Leader.StringId, out philosophyString, null);
		}

		public bool GetConversationHeroPoliticalPhilosophy_2(out TextObject philosophyString_2)
		{
			return GameTexts.TryGetText(string.Concat(new string[]
			{
				"str_political_philosophy_",
				Hero.OneToOneConversationHero.StringId,
				"_for_",
				Hero.OneToOneConversationHero.MapFaction.Leader.StringId,
				"_b"
			}), out philosophyString_2, null);
		}

		public bool GetConversationHeroPoliticalPhilosophy_3(out TextObject philosophyString_3)
		{
			return GameTexts.TryGetText(string.Concat(new string[]
			{
				"str_political_philosophy_",
				Hero.OneToOneConversationHero.StringId,
				"_for_",
				Hero.OneToOneConversationHero.MapFaction.Leader.StringId,
				"_c"
			}), out philosophyString_3, null);
		}

		public TextObject GetLiegeTitle()
		{
			Hero leader = Hero.OneToOneConversationHero.MapFaction.Leader;
			if (!leader.IsFemale)
			{
				return Campaign.Current.ConversationManager.FindMatchingTextOrNull("str_liege_title", leader.CharacterObject);
			}
			return Campaign.Current.ConversationManager.FindMatchingTextOrNull("str_liege_title_female", leader.CharacterObject);
		}

		private void SetRecruitTextVariables()
		{
			Hero.OneToOneConversationHero.MapFaction.Leader.SetTextVariables();
		}

		private int GetMercenaryAwardFactor()
		{
			return Campaign.Current.Models.MinorFactionsModel.GetMercenaryAwardFactorToJoinKingdom(Clan.PlayerClan, Hero.OneToOneConversationHero.Clan.Kingdom, false);
		}

		private bool lord_comments()
		{
			return Campaign.Current.CurrentConversationContext != ConversationContext.FreedHero && Campaign.Current.CurrentConversationContext != ConversationContext.CapturedLord && (!ConversationHelper.ConversationTroopCommentShown && Hero.OneToOneConversationHero != null && this.UsesLordConversations(Hero.OneToOneConversationHero));
		}

		public bool UsesLordConversations(Hero hero)
		{
			return hero.IsLord || hero.IsWanderer || hero.IsMerchant || hero.IsPreacher || hero.IsHeadman || hero.IsArtisan || hero.IsGangLeader || hero.IsRuralNotable || hero.IsSpecial;
		}

		private bool too_many_companions()
		{
			return Clan.PlayerClan.Companions.Count >= Clan.PlayerClan.CompanionLimit;
		}

		private bool PlayerIsBesieging()
		{
			if (Settlement.CurrentSettlement != null && Settlement.CurrentSettlement.SiegeEvent != null)
			{
				return Settlement.CurrentSettlement.SiegeEvent.BesiegerCamp.GetInvolvedPartiesForEventType(MapEvent.BattleTypes.Siege).Any((PartyBase party) => party.MobileParty == Hero.MainHero.PartyBelongedTo);
			}
			return false;
		}

		private bool PlayerIsBesieged()
		{
			if (Settlement.CurrentSettlement != null && Settlement.CurrentSettlement.SiegeEvent != null)
			{
				return Settlement.CurrentSettlement.Parties.Any((MobileParty mobileParty) => mobileParty == Hero.MainHero.PartyBelongedTo);
			}
			return false;
		}

		private void AddVoiceStrings()
		{
		}

		public override void RegisterEvents()
		{
			CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));
			CampaignEvents.OnBarterAcceptedEvent.AddNonSerializedListener(this, new Action<Hero, Hero, List<Barterable>>(this.OnBarterAccepted));
			CampaignEvents.OnBarterCanceledEvent.AddNonSerializedListener(this, new Action<Hero, Hero, List<Barterable>>(this.OnBarterCanceled));
		}

		private void OnBarterCanceled(Hero offererHero, Hero otherHero, List<Barterable> barters)
		{
			if (offererHero == Hero.MainHero)
			{
				MBTextManager.SetTextVariable("BARTER_CONCLUSION_LINE", Campaign.Current.ConversationManager.FindMatchingTextOrNull("str_barter_refused", CharacterObject.OneToOneConversationCharacter), false);
				if (barters.Where((Barterable x) => x is JoinKingdomAsClanBarterable && x.OriginalOwner != Hero.MainHero).Any<Barterable>())
				{
					MBTextManager.SetTextVariable("BARTER_CONCLUSION_LINE", Campaign.Current.ConversationManager.FindMatchingTextOrNull("str_defect_barter_refused", CharacterObject.OneToOneConversationCharacter), false);
				}
			}
		}

		private void OnBarterAccepted(Hero offererHero, Hero otherHero, List<Barterable> barters)
		{
			if (offererHero == Hero.MainHero)
			{
				MBTextManager.SetTextVariable("BARTER_CONCLUSION_LINE", Campaign.Current.ConversationManager.FindMatchingTextOrNull("str_barter_agreed", CharacterObject.OneToOneConversationCharacter), false);
				if (barters.Where((Barterable x) => x is JoinKingdomAsClanBarterable && x.OriginalOwner != Hero.MainHero).Any<Barterable>())
				{
					MBTextManager.SetTextVariable("BARTER_CONCLUSION_LINE", Campaign.Current.ConversationManager.FindMatchingTextOrNull("str_defect_barter_agreed", CharacterObject.OneToOneConversationCharacter), false);
				}
			}
		}

		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<Dictionary<CharacterObject, CharacterObject>>("_previouslyMetWandererTemplates", ref this._previouslyMetWandererTemplates);
			dataStore.SyncData<bool>("_receivedVassalRewards", ref this._receivedVassalRewards);
		}

		private void OnSessionLaunched(CampaignGameStarter campaignGameStarter)
		{
			StringHelpers.SetCharacterProperties("PLAYER", Hero.MainHero.CharacterObject, null, false);
			MBTextManager.SetTextVariable("GOLD_ICON", "{=!}<img src=\"General\\Icons\\Coin@2x\" extend=\"8\">", false);
			this.AddDialogs(campaignGameStarter);
		}

		protected void AddDialogs(CampaignGameStarter starter)
		{
			GameTexts.AddGameTextWithVariation("STR_SALUTATION_FROM_PLAYER").Variation("{=xnR3sXJP}{PLAYER.NAME}", new object[] { "DefaultTag", 1 }).Variation("{=CRqdPoj9}your highness", new object[] { "NpcIsLiegeTag", 1 })
				.Variation("{=AM1ROQcT}your lordship", new object[] { "NpcIsNobleTag", 1 })
				.Variation("{=GDSwyH2p}your ladyship", new object[] { "NpcIsNobleTag", 1, "NpcIsFemaleTag", 1 })
				.Variation("{=edRggEQ4}my friend", new object[] { "MetBeforeTag", 2, "FriendlyRelationshipTag", 1 })
				.Variation("{=8eHRth3U}my wife", new object[] { "PlayerIsSpouseTag", 5, "NpcIsFemaleTag", 1 })
				.Variation("{=QuVgluRH}my husband", new object[] { "PlayerIsSpouseTag", 5, "NpcIsMaleTag", 1 });
			this.AddVoiceStrings();
			starter.AddDialogLine("set_vars", "start", "lord_intro", "{=!}Never see this", new ConversationSentence.OnConditionDelegate(this.conversation_set_first_on_condition), null, 100, null);
			starter.AddDialogLine("parley", "start", "lord_intro", "{=!}{STR_PARLEY_COMMENT}", new ConversationSentence.OnConditionDelegate(this.conversation_siege_parley_unmet_on_condition), null, 100, null);
			starter.AddDialogLine("parley_2", "start", "lord_start", "{=!}{STR_PARLEY_COMMENT}", new ConversationSentence.OnConditionDelegate(this.conversation_siege_parley_met_on_condition), null, 100, null);
			starter.AddDialogLine("start_attacking_unmet", "start", "lord_meet_player_response", "{=!}{VOICED_LINE}", new ConversationSentence.OnConditionDelegate(this.conversation_attacking_lord_set_meeting_meet_on_condition), null, 100, null);
			starter.AddDialogLine("start_attacking_met", "start", "lord_start", "{=!}{VOICED_LINE}", new ConversationSentence.OnConditionDelegate(this.conversation_lord_attacking_on_condition), null, 100, null);
			starter.AddDialogLine("ally_thanks_meet_after_helping_in_battle", "start", "ally_thanks_meet", "{=!}{MEETING_SENTENCE}", new ConversationSentence.OnConditionDelegate(this.conversation_ally_thanks_meet_after_helping_in_battle_on_condition), null, 110, null);
			starter.AddDialogLine("ally_thanks_after_helping_in_battle", "start", "close_window", "{=!}{GREETING_SENTENCE}", new ConversationSentence.OnConditionDelegate(this.conversation_ally_thanks_after_helping_in_battle_on_condition), new ConversationSentence.OnConsequenceDelegate(this.conversation_ally_thanks_meet_after_helping_in_battle_2_on_consequence), 110, null);
			starter.AddPlayerLine("player_prisoner_talk_let_go", "hero_main_options", "player_prisoner_let_go", "{=cCYHPyit}I have decided to free you. You may go.", new ConversationSentence.OnConditionDelegate(this.conversation_player_let_prisoner_go_on_condition), null, 100, null, null);
			starter.AddPlayerLine("player_prisoner_talk_let_go_answer", "player_prisoner_let_go", "close_window", "{=7V5SbkQ2}Well... Thank you very much. I am grateful.", null, new ConversationSentence.OnConsequenceDelegate(this.conversation_player_let_prisoner_go_on_consequence), 100, null, null);
			starter.AddDialogLine("start_wanderer_unmet", "start", "wanderer_meet_player_response", "{=!}{VOICED_LINE}", new ConversationSentence.OnConditionDelegate(this.conversation_wanderer_meet_on_condition), null, 110, null);
			starter.AddDialogLine("unmet_in_main_mobile_party", "start", "lord_meet_in_main_party_player_response", "{=!}{VOICED_LINE}", new ConversationSentence.OnConditionDelegate(this.conversation_unmet_lord_main_party_on_condition), null, 110, null);
			starter.AddDialogLine("start_lord_unmet", "start", "lord_meet_player_response", "{=!}{VOICED_LINE}", new ConversationSentence.OnConditionDelegate(this.conversation_lord_meet_on_condition), null, 110, null);
			starter.AddDialogLine("start_default_under_24_hours", "start", "lord_start", "{=!}{SHORT_ABSENCE_GREETING}", new ConversationSentence.OnConditionDelegate(this.conversation_lord_greets_under_24_hours_on_condition), null, 100, null);
			starter.AddDialogLine("start_default", "start", "lord_start", "{=!}{VOICED_LINE}", new ConversationSentence.OnConditionDelegate(this.conversation_lord_greets_over_24_hours_on_condition), null, 100, null);
			this.AddIntroductions(starter);
			this.AddWandererConversations(starter);
			this.AddHeroGeneralConversations(starter);
			this.AddLordLiberateConversations(starter);
			this.AddPoliticsAndBarter(starter);
			this.AddOtherConversations(starter);
			starter.AddPlayerLine("lord_meet_player_as_liege_response", "lord_meet_player_response", "lord_introduction", "{=PBZZrK90}Yes... Please go on.", new ConversationSentence.OnConditionDelegate(this.conversation_lord_meet_player_as_liege_response_on_condition), new ConversationSentence.OnConsequenceDelegate(this.conversation_lord_meet_player_response_on_consequence), 100, null, null);
			starter.AddPlayerLine("lord_in_main_party_meet_player_response", "lord_meet_in_main_party_player_response", "lord_start", "{=5Ly65EsX}It is nice to have you with us.", new ConversationSentence.OnConditionDelegate(this.conversation_lord_meet_in_player_party_player_on_condition), new ConversationSentence.OnConsequenceDelegate(this.conversation_lord_meet_player_response_on_consequence), 100, null, null);
			starter.AddPlayerLine("lord_meet_player_response1", "lord_meet_player_response", "lord_introduction", "{=JIJnrSq0}I am {PLAYER.NAME}. And who are you?", new ConversationSentence.OnConditionDelegate(this.conversation_lord_meet_player_response1_on_condition), new ConversationSentence.OnConsequenceDelegate(this.conversation_lord_meet_player_response_on_consequence), 100, null, null);
			starter.AddPlayerLine("lord_meet_player_response2", "lord_meet_player_response", "lord_introduction", "{=NmGJs7yB}My name is {PLAYER.NAME}, {?CONVERSATION_NPC.GENDER}madam{?}sir{\\?}. May I ask your name?", new ConversationSentence.OnConditionDelegate(this.conversation_lord_meet_player_response2_on_condition), new ConversationSentence.OnConsequenceDelegate(this.conversation_lord_meet_player_response_on_consequence), 100, null, null);
			starter.AddPlayerLine("lord_meet_player_response3", "lord_meet_player_response", "lord_introduction", "{=PtDgM4Xo}They know me as {PLAYER.NAME}. Mark it down, you shall be hearing of me a lot.", new ConversationSentence.OnConditionDelegate(this.conversation_lord_meet_player_response3_on_condition), new ConversationSentence.OnConsequenceDelegate(this.conversation_lord_meet_player_response_on_consequence), 100, null, null);
			starter.AddDialogLine("lord_ask", "lord_start", "lord_demands_surrender_after_comment", "{=!}{COMMENT_STRING}", new ConversationSentence.OnConditionDelegate(this.conversation_lord_makes_preattack_comment_on_condition), null, 100, null);
			starter.AddDialogLine("lord_ask_2", "lord_start", "hero_main_options", "{=!}{COMMENT_STRING}", new ConversationSentence.OnConditionDelegate(this.conversation_lord_makes_comment_on_condition), null, 100, null);
			starter.AddDialogLine("lord_ask_3", "lord_start", "player_responds_to_surrender_demand", "{=!}{SURRENDER_DEMAND_STRING}", new ConversationSentence.OnConditionDelegate(this.conversation_lord_makes_surrender_demand_on_condition), null, 100, null);
			starter.AddDialogLine("hero_ask_4", "lord_start", "hero_main_options", "{=7bBfNRVS}So, then. What is it?", null, null, 100, null);
			starter.AddDialogLine("lord_ask_5", "lord_demands_surrender_after_comment", "player_responds_to_surrender_demand", "{=!}{MINOR_FACTION_SURRENDER_DEMAND_STRING}[ib:agressive]", new ConversationSentence.OnConditionDelegate(this.conversation_minor_faction_makes_surrender_demand_on_condition), null, 100, null);
			starter.AddDialogLine("lord_ask_6", "lord_demands_surrender_after_comment", "player_responds_to_surrender_demand", "{=!}{SURRENDER_DEMAND_STRING}", new ConversationSentence.OnConditionDelegate(this.conversation_lord_makes_surrender_demand_on_condition), null, 100, null);
			this.AddFinalLines(starter);
		}

		private bool prisoner_barter_successful_condition()
		{
			return Campaign.Current.BarterManager.LastBarterIsAccepted;
		}

		private void AddFinalLines(CampaignGameStarter starter)
		{
			starter.AddPlayerLine("hero_special_request", "lord_talk_speak_diplomacy_2", "lord_pretalk", "{=PznWhAdU}Actually, never mind.", null, null, 1, null, null);
		}

		private void AddOtherConversations(CampaignGameStarter starter)
		{
			starter.AddPlayerLine("ally_thanks_meet", "ally_thanks_meet", "ally_thanks_meet_2", "{=O4KI2lgT}My name is {PLAYER.NAME}.", null, null, 100, null, null);
			starter.AddDialogLine("ally_thanks_meet_after_helping_in_battle_2", "ally_thanks_meet_2", "close_window", "{=jgbVweOs}{GRATITUDE_SENTENCE}[if:convo_calm_friendly]", null, new ConversationSentence.OnConsequenceDelegate(this.conversation_ally_thanks_meet_after_helping_in_battle_2_on_consequence), 100, null);
			starter.AddPlayerLine("talk_lord_defeat_to_lord_capture", "defeated_lord_answer", "defeat_lord_answer_1", "{=g5G8AJ5n}You are my prisoner now.", null, null, 100, null, null);
			starter.AddPlayerLine("talk_lord_defeat_to_lord_release_noncom", "defeated_lord_answer", "defeat_lord_answer_2", "{=SFWNy76G}As you are not a warrior, you are free to go.", new ConversationSentence.OnConditionDelegate(this.conversation_talk_lord_release_noncombatant_on_condition), new ConversationSentence.OnConsequenceDelegate(LordConversationsCampaignBehavior.conversation_talk_lord_defeat_to_lord_release_on_consequence), 100, null, null);
			starter.AddPlayerLine("talk_lord_defeat_to_lord_release", "defeated_lord_answer", "defeat_lord_answer_2", "{=vHKkVkAF}You have fought well. You are free to go.", new ConversationSentence.OnConditionDelegate(this.conversation_talk_lord_release_combatant_on_condition), new ConversationSentence.OnConsequenceDelegate(LordConversationsCampaignBehavior.conversation_talk_lord_defeat_to_lord_release_on_consequence), 100, null, null);
			starter.AddPlayerLine("talk_lord_freed_to_lord_capture", "freed_lord_answer", "freed_lord_answer_1", "{=l2hijFNU}You're not going anywhere, friend. You're my prisoner now.", null, new ConversationSentence.OnConsequenceDelegate(this.conversation_talk_lord_freed_to_lord_capture_on_consequence), 100, null, null);
			starter.AddPlayerLine("talk_lord_freed_to_lord_release", "freed_lord_answer", "freed_lord_answer_2", "{=5rBnjXqX}You are free to go wherever you want, {?CONVERSATION_NPC.GENDER}madam{?}sir{\\?}.", null, new ConversationSentence.OnConsequenceDelegate(this.conversation_talk_lord_freed_to_lord_release_on_consequence), 100, null, null);
			starter.AddDialogLine("talk_defeated_lord_capture_return", "defeat_lord_answer_1", "close_window", "{=X7Fod9WN}I am at your mercy.[if:convo_beaten]", null, delegate
			{
				Campaign.Current.ConversationManager.ConversationEndOneShot += this.conversation_talk_lord_defeat_to_lord_capture_on_consequence;
			}, 100, null);
			starter.AddDialogLine("talk_freed_lord_capture_return", "freed_lord_answer_1", "close_window", "{=xCy5AXrz}I'll have your head on a pike for this, you bastard! Someday![if:convo_furious][ib:agressive]", null, null, 100, null);
			starter.AddDialogLine("talk_defeated_lord_release_return", "defeat_lord_answer_2", "close_window", "{=!}{DEFEAT_LORD_ANSWER}", null, null, 100, null);
			starter.AddDialogLine("talk_freed_lord_release_return", "freed_lord_answer_2", "close_window", "{=ydGffr9O}Thank you, good {?PLAYER.GENDER}lady{?}sire{\\?}. I never forget someone who's done me a good turn.[if:convo_calm_friendly]", null, null, 100, null);
			starter.AddDialogLine("lord_request_mission_ask", "lord_request_mission_ask", "lord_mercenary_service", "{=YdZtydK4}As it happens, {PLAYER.NAME}, I promised {FACTION_LEADER} that I would hire a company of mercenaries for an upcoming campaign.", new ConversationSentence.OnConditionDelegate(this.conversation_lord_request_mission_ask_on_condition), null, 100, null);
			starter.AddPlayerLine("lord_mercenary_service_not_interested", "lord_mercenary_service", "lord_mercenary_service_reject", "{=79yyTwvu}I'm not interested, thank you.", null, null, 100, null, null);
			starter.AddPlayerLine("lord_mercenary_service_join", "lord_mercenary_service", "lord_mercenary_service_accept", "{=L802I9W2}Aye, I'll join the {FACTION_NAME}.", null, null, 100, null, null);
			starter.AddPlayerLine("lord_mercenary_service_tell_me_more", "lord_mercenary_service", "lord_mercenary_elaborate_pay", "{=OnPC6tvb}I'm interested. Please tell me more.", null, null, 100, null, null);
			starter.AddDialogLine("lord_mercenary_service_accept_verify", "lord_mercenary_service_accept", "lord_mercenary_service_verify", "{=lgBaqlVZ}Perfect. Of course you shall have to make a formal declaration of allegiance, and give your oath that you and your company will remain in service to {FACTION_NAME} for a period of no less than three months.", null, null, 100, null);
			starter.AddPlayerLine("lord_mercenary_service_verify_accept", "lord_mercenary_service_verify", "lord_mercenary_service_verify_2", "{=hnSFjIkM}As you wish. Your enemies are my enemies.", null, new ConversationSentence.OnConsequenceDelegate(this.conversation_lord_mercenary_service_verify_accept_on_consequence), 100, null, null);
			starter.AddPlayerLine("lord_mercenary_service_verify_reject", "lord_mercenary_service_verify", "lord_mercenary_service_reject", "{=ErkaEHBp}On second thought, forget it.", null, null, 100, null, null);
			starter.AddDialogLine("lord_mercenary_service_verify_2", "lord_mercenary_service_verify_2", "lord_mercenary_service_accept_3", "{=2oRJ0IzW}That will do. You've made a wise choice, my friend. We do well by our loyal fighters, and you can expect worthy rewards for your service.", null, null, 100, null);
			starter.AddDialogLine("lord_mercenary_service_accept_3", "lord_mercenary_service_accept_3", "lord_pretalk", "{=AK7jsatk}Now, I suggest you prepare for a serious campaign. Train and equip your soldiers as best you can in the meantime, and respond quickly when you are summoned for duty.", null, null, 100, null);
			starter.AddDialogLine("lord_mercenary_service_reject", "lord_mercenary_service_reject", "lord_pretalk", "{=5bza9sDs}I'm very sorry to hear that. You'll find no better employers than the {FACTION_NAME}, be sure of that.", null, null, 100, null);
			starter.AddDialogLine("lord_mercenary_elaborate_pay", "lord_mercenary_elaborate_pay", "lord_mercenary_elaborate_1", "{=L9840K0F}I can offer you a contract for three months. At the end of those three, it can be extended month by month. An initial sum of {OFFER_VALUE}{GOLD_ICON} will be paid to you to seal the contract. After that, you'll receive wages from {FACTION_LEADER} each week, according to the number and quality of the soldiers in your company. You still have your rights to battlefield loot and salvage, as well as any prisoners you capture. War can be very profitable at times...", null, null, 100, null);
			starter.AddDialogLine("lord_mercenary_service_elaborate_duty", "lord_mercenary_service_elaborate_duty", "lord_mercenary_elaborate_1", "{=pfxvubiK}Duties... There are only a few, none of them difficult. The very first thing is to declare your allegiance. An oath of loyalty to our cause. Once that's done, you shall be required to fulfill certain responsibilities. You'll participate in military campaigns, fulfill any duties given to you by your commanders, and most of all you shall attack the enemies of our kingdom wherever you might find them.", null, null, 100, null);
			starter.AddPlayerLine("lord_mercenary_elaborate_duty", "lord_mercenary_elaborate_1", "lord_mercenary_service_elaborate_duty", "{=hzjmvPcB}And what about my duties as a mercenary?", null, null, 100, null, null);
			starter.AddPlayerLine("lord_mercenary_elaborate_castle", "lord_mercenary_elaborate_1", "lord_mercenary_elaborate_castle", "{=K6Lhlnbh}Can I hold on to any castles I take?", null, null, 100, null, null);
			starter.AddPlayerLine("lord_mercenary_elaborate_banner", "lord_mercenary_elaborate_1", "lord_mercenary_elaborate_banner", "{=3a0oy2n3}Can I fly my own banner?", null, null, 100, null, null);
			starter.AddPlayerLine("lord_mercenary_elaborate_wage", "lord_mercenary_elaborate_1", "lord_mercenary_elaborate_pay", "{=bDva8kVX}How much will you pay me for my service?", null, null, 100, null, null);
			starter.AddPlayerLine("lord_mercenary_elaborate_accept", "lord_mercenary_elaborate_1", "lord_mercenary_service_accept", "{=HiWlXbgN}Sounds good. I wish to enter your service as a mercenary.", null, null, 100, null, null);
			starter.AddPlayerLine("lord_mercenary_elaborate_reject", "lord_mercenary_elaborate_1", "lord_mercenary_service_reject", "{=JfAQx4hD}Apologies, my sword is not for hire.", null, null, 100, null, null);
			starter.AddDialogLine("lord_mercenary_elaborate_castle_answer_faction_owner_to_women", "lord_mercenary_elaborate_castle", "lord_mercenary_elaborate_1", "{=607tZdso}Only my loyal vassals can own lands and castles in my realm -- and all my vassals are men.I am not inclined to depart from this tradition without a very good reason. If you prove yourself in battle, you can swear an oath of homage to me and become my vassal.We may then discuss how you may obtain a castle.", new ConversationSentence.OnConditionDelegate(this.conversation_lord_mercenary_elaborate_castle_answer_faction_owner_to_women_on_condition), null, 100, null);
			starter.AddDialogLine("lord_mercenary_elaborate_castle_answer_to_women", "lord_mercenary_elaborate_castle", "lord_mercenary_elaborate_1", "{=FaYXPkaX}Hmm... Only loyal vassals of {FACTION_LEADER} can own lands and castles. While kings will sometimes accept vassalage from men who prove themselves in battle, and grant them land, I have never heard of a king who gave fiefs to women. You had best discuss that issue with {FACTION_LEADER} himself.", new ConversationSentence.OnConditionDelegate(this.conversation_lord_mercenary_elaborate_castle_answer_to_women_on_condition), null, 100, null);
			starter.AddDialogLine("lord_mercenary_elaborate_castle_answer_faction_owner", "lord_mercenary_elaborate_castle", "lord_mercenary_elaborate_1", "{=xlFEUjhk}Only my loyal vassals can own lands and castles in my realm. A mercenary can not be trusted with such a responsibility. However, after serving for some time, you can swear homage to me and become my vassal. Then you will be rewarded with a fief.", new ConversationSentence.OnConditionDelegate(this.conversation_lord_mercenary_elaborate_castle_answer_faction_owner_on_condition), null, 100, null);
			starter.AddDialogLine("lord_mercenary_elaborate_castle_answer", "lord_mercenary_elaborate_castle", "lord_mercenary_elaborate_1", "{=9Jm0AnJO}Only loyal vassals of {FACTION_LEADER} can own lands and castles. You understand, a simple mercenary cannot be trusted with such responsibility. However, after serving for some time, you may earn the right to swear homage to {FACTION_LEADER} and become his vassal. Then you would be rewarded with a fief.", null, null, 100, null);
			starter.AddDialogLine("lord_mercenary_elaborate_banner_answer_faction_owner", "lord_mercenary_elaborate_banner", "lord_mercenary_elaborate_1", "{=qwfFgpCT}Only my noble vassals have the honour of carrying their own banners. However, after some time in mercenary service, you may earn the opportunity to swear homage to me and become my vassal, gaining the right to choose a banner of your own and fight under it in battle.", new ConversationSentence.OnConditionDelegate(this.conversation_lord_mercenary_elaborate_banner_answer_faction_owner_on_condition), null, 100, null);
			starter.AddDialogLine("lord_mercenary_elaborate_banner_answer", "lord_mercenary_elaborate_banner", "lord_mercenary_elaborate_1", "{=VlPD0Dhh}Only noble vassals of {FACTION_LEADER} have the honour of carrying their own banners. However, after some time of mercenary service, perhaps you can earn the opportunity to swear homage to {FACTION_LEADER} and become his vassal, gaining the right to choose a banner of your own and fight under it in battle.", null, null, 100, null);
			starter.AddDialogLine("lord_tell_mission_sworn_vassal_1", "lord_tell_mission_sworn_vassal_1", "lord_pretalk", "{=egOR9DAz}If a worthy task presents itself, however, I may have a favor to ask of you at a later date.", null, null, 100, null);
			starter.AddDialogLine("lord_mission_destroy_bandit_lair_start", "lord_tell_mission", "destroy_lair_quest_brief", "{=dM4Hm9XS}Yes -- there is something you can do for us. We have heard reports that a group of {s4} have established a hideout in this area, and have been attacking travellers. If you could find their lair and destroy it, we would be very grateful.", new ConversationSentence.OnConditionDelegate(this.conversation_lord_mission_destroy_bandit_lair_start_on_condition), null, 100, null);
			starter.AddDialogLine("convince_begin", "convince_begin", "convince_options", "{=qbelwdxI}I still don't see why I should accept what you're asking of me.", null, null, 100, null);
			starter.AddPlayerLine("convince_options_bribe", "convince_options", "convince_bribe", "{=CNbPKRXX}Then I'll make it worth your while. (-{BRIBE_MONEY}{GOLD_ICON})", new ConversationSentence.OnConditionDelegate(this.conversation_convince_options_bribe_on_condition), null, 100, null, null);
			starter.AddPlayerLine("convince_options_friendship", "convince_options", "convince_friendship", "{=VDlbYDmW}Please, do it for the sake of our friendship. (-{RELATION_DECREASE} to relation)", new ConversationSentence.OnConditionDelegate(this.conversation_convince_options_friendship_on_condition), null, 100, null, null);
			starter.AddPlayerLine("convince_options_persuasion", "convince_options", "convince_persuade_begin", "{=PE14ZF7l}Let me try and convince you. (Persuasion)", null, null, 100, null, null);
			starter.AddPlayerLine("convince_options_give_up", "convince_options", "lord_pretalk", "{=D33fIGQe}Never mind.", null, null, 100, null, null);
			starter.AddDialogLine("convince_bribe", "convince_bribe", "convince_bribe_verify", "{=F2SzaTmk}Mmm, a generous gift to my coffers would certainly help matters... {BRIBE_MONEY}{GOLD_ICON} should do it. If you agree, then I'll go with your suggestion.", null, null, 100, null);
			starter.AddPlayerLine("convince_bribe_cant_afford", "convince_bribe_verify", "convince_bribe_cant_afford", "{=IYRJKtkb}I'm afraid my finances will not allow for such a gift.", null, null, 100, null, null);
			starter.AddPlayerLine("convince_bribe_verify", "convince_bribe_verify", "convince_bribe_goon", "{=jzbaH2NE}Very well, please accept these {BRIBE_MONEY}{GOLD_ICON} as a token of my gratitude.", new ConversationSentence.OnConditionDelegate(this.conversation_convince_bribe_verify_on_condition), new ConversationSentence.OnConsequenceDelegate(this.conversation_convince_bribe_player_accept_on_consequence), 100, null, null);
			starter.AddPlayerLine("convince_bribe_reconsider", "convince_bribe_verify", "convince_begin", "{=wPpeHeeX}Let me think about this some more.", null, null, 100, null, null);
			starter.AddDialogLine("convince_bribe_cant_afford_response", "convince_bribe_cant_afford", "convince_options", "{=FHSfNxNR}Ah. In that case, there is little I can do, unless you have some further argument to make.", null, null, 100, null);
			starter.AddDialogLine("convince_bribe_verify_response", "convince_bribe_goon", "convince_accept", "{=xpafjLhC}My dear {PLAYER.NAME}, your generous gift has led me to reconsider what you ask, and I have come to appreciate the wisdom of your proposal.", null, null, 100, null);
			starter.AddDialogLine("convince_friendship", "convince_friendship", "convince_friendship_verify", "{=aPsDkV9w}You've done well by me in the past, {PLAYER.NAME}, and for that I will go along with your request, but know that I do not like you using our relationship this way.[ib:closed][if:convo_stern]", null, null, 100, null);
			starter.AddPlayerLine("convince_friendship_verify_positive", "convince_friendship_verify", "convince_friendship_go_on", "{=4JcB01xW}I am sorry, my friend, but I need your help in this.", null, null, 100, null, null);
			starter.AddPlayerLine("convince_friendship_verify_negative", "convince_friendship_verify", "lord_pretalk", "{=mkdzl8ma}If it will not please you, then I'll try something else.", null, null, 100, null, null);
			starter.AddDialogLine("convince_friendship_go_on", "convince_friendship_go_on", "convince_accept", "{=SKObiqQJ}All right then, {PLAYER.NAME}, I will accept this for your sake. But remember, you owe me for this.[if:convo_stern]", new ConversationSentence.OnConditionDelegate(this.conversation_convince_friendship_verify_go_on_on_condition), new ConversationSentence.OnConsequenceDelegate(this.conversation_convince_friendship_verify_go_on_on_consequence), 100, null);
			starter.AddDialogLine("convince_friendship_lord_response_no", "convince_friendship", "lord_pretalk", "{=XoftWx6z}I don't think I owe you such a favor {PLAYER.NAME}. I see no reason to accept this for you.[if:convo_stern]", new ConversationSentence.OnConditionDelegate(this.conversation_convince_friendship_lord_response_no_on_condition), null, 100, null);
			starter.AddDialogLine("convince_friendship_lord_response_angry", "convince_friendship", "lord_pretalk", "{=0Pt6Maba}Is this a joke? You've some nerve asking me for favours, {PLAYER.NAME}, and let me assure you you'll get none.[if:convo_stern]", new ConversationSentence.OnConditionDelegate(this.conversation_convince_friendship_lord_response_angry_on_condition), null, 100, null);
			starter.AddPlayerLine("lord_generic_mission_accept", "lord_mission_told", "lord_mission_accepted", "{=bTUPJjfk}You can count on me, {?CONVERSATION_NPC.GENDER}madame{?}sir{\\?}.", null, new ConversationSentence.OnConsequenceDelegate(this.conversation_lord_generic_mission_accept_on_consequence), 100, null, null);
			starter.AddPlayerLine("lord_generic_mission_reject", "lord_mission_told", "lord_mission_rejected", "{=e0HktW6w}I fear I cannot accept such a mission at the moment.", null, new ConversationSentence.OnConsequenceDelegate(this.conversation_lord_generic_mission_reject_on_consequence), 100, null, null);
			starter.AddDialogLine("lord_generic_mission_accepted", "lord_mission_accepted", "close_window", "{=tmqYGwMa}Excellent, {PLAYER.NAME}, excellent. I have every confidence in you.[if:idle_happy]", null, null, 100, null);
			starter.AddDialogLine("lord_generic_mission_rejected", "lord_mission_rejected", "lord_pretalk", "{=iB6P0D9N}Is that so? Well, I suppose you're just not up to the task. I shall have to look for somebody with more mettle.", null, null, 100, null);
			starter.AddDialogLine("lord_tell_mission_no_quest", "lord_tell_mission", "lord_pretalk", "{=Xr0iLlP2}I don't have any other jobs for you right now.", new ConversationSentence.OnConditionDelegate(this.conversation_lord_tell_mission_no_quest_on_condition), null, 100, null);
			starter.AddPlayerLine("player_threats_lord_verify", "lord_attack_verify", "party_encounter_lord_hostile_attacker_3", "{=6GhWT4vi}I repeat: Yield or fight!", new ConversationSentence.OnConditionDelegate(this.conversation_player_threats_lord_verify_on_condition), new ConversationSentence.OnConsequenceDelegate(this.conversation_player_threats_lord_verify_on_consequence), 100, null, null);
			starter.AddDialogLineWithVariation("player_threatens_enemy_lord", "player_threatens_enemy_lord", "player_verify_attack_on_enemy_lord", null, null, 100, "", "", "", "", null).Variation(new object[] { "{=XSWH8Z5B}I have no wish to fight you.[ib:closed][if:idle_angry]", "DefaultTag", 1 }).Variation(new object[] { "{=dU9HQB8H}I have no wish to fight, but I shall certainly not surrender.[ib:closed]", "ChivalrousTag", 1, "PersonaEarnestTag", 1 })
				.Variation(new object[] { "{=2XbzsQkM}Hmf. Really?[ib:closed][if:idle_angry]", "PersonaCurtTag", 1 })
				.Variation(new object[] { "{=gHTK6yH5}I don't want to fight, but I will if you make me.[ib:closed][if:idle_angry]", "PersonaEarnestTag", 1 })
				.Variation(new object[] { "{=tf1WPrkn}I had hoped that perhaps things would not come to fighting, at least not today.[ib:closed]", "PersonaSoftspokenTag", 1 })
				.Variation(new object[] { "{=5oYf9B8Z}Now that's an unpleasant set of options. How about we just not fight today?[ib:closed]", "PersonaIronicTag", 1 });
			starter.AddPlayerLine("player_verify_attack_on_enemy_lord", "player_verify_attack_on_enemy_lord", "party_encounter_lord_hostile_attacker_3", "{=hObVTgc7}You heard me. Yield or fight!", null, new ConversationSentence.OnConsequenceDelegate(this.conversation_player_threats_lord_verify_on_consequence), 100, null, null);
			starter.AddPlayerLine("player_cancels_attack_on_enemy_lord", "player_verify_attack_on_enemy_lord", "player_cancels_attack_on_enemy_respond", "{=Ukv1FQa2}I've changed my mind. You may go on your way.", null, new ConversationSentence.OnConsequenceDelegate(this.conversation_lord_attack_verify_cancel_on_consequence), 100, null, null);
			starter.AddDialogLine("player_cancels_attack_on_enemy_respond", "player_cancels_attack_on_enemy_respond", "close_window", "{=FHFfB4S0}I will do that.", null, null, 100, null);
			starter.AddPlayerLine("lord_tell_gathering_player_answer", "lord_tell_gathering", "lord_talk_player_ask_join_army", "{=OyjWJBhI}I want to join your army.", null, null, 100, new ConversationSentence.OnClickableConditionDelegate(this.conversation_lord_join_army_on_clickable_condition), null);
			starter.AddPlayerLine("lord_tell_gathering_player_answer_2", "lord_tell_gathering", "lord_pretalk", "{=DqSSCVNi}Great.", null, null, 100, null, null);
			starter.AddDialogLine("lord_tell_gathering_player_joined", "lord_talk_player_ask_join_army", "lord_pretalk", "{=0nqzQqGy}Sure. We will wait other parties around {GATHERING_SETTLEMENT} for a while, then follow us.", null, new ConversationSentence.OnConsequenceDelegate(this.conversation_lord_tell_gathering_player_joined_on_consequence), 100, null);
			starter.AddDialogLine("lord_ask_pardon_answer_bad_relation", "lord_ask_pardon", "lord_pretalk", "{=k27q07EZ}Do you indeed, {PLAYER.NAME}? Then go and trip on your sword. Give us all peace.", new ConversationSentence.OnConditionDelegate(this.conversation_lord_ask_pardon_answer_bad_relation_on_condition), null, 100, null);
			starter.AddDialogLine("lord_ask_pardon_answer_low_right_to_rule", "lord_ask_pardon", "lord_pretalk", "{=UfpmWfbG}{PLAYER.NAME}, you are a {?PLAYER.GENDER}lady{?}lord{\\?} without a master, holding lands in your name, with only the barest scrap of a claim to legitimacy. No king in Calradia would accept a lasting peace with you.", new ConversationSentence.OnConditionDelegate(this.conversation_lord_ask_pardon_answer_low_right_to_rule_on_condition), null, 100, null);
			starter.AddDialogLine("lord_ask_pardon_answer_no_advantage", "lord_ask_pardon", "lord_pretalk", "{=o7t4TFlW}Make peace when I have you at an advantage? I think not.", new ConversationSentence.OnConditionDelegate(this.conversation_lord_ask_pardon_answer_no_advantage_on_condition), null, 100, null);
			starter.AddDialogLine("lord_ask_pardon_answer_not_accepted", "lord_ask_pardon", "lord_pretalk", "{=dW09queg}I do not see it as being in my current interest to make peace.", new ConversationSentence.OnConditionDelegate(this.conversation_lord_ask_pardon_answer_not_accepted_on_condition), null, 100, null);
			starter.AddDialogLine("lord_ask_pardon_answer_accepted", "lord_ask_pardon", "lord_truce_offer", "{=bXdzsTdb}Yes... I am weary of fighting you. I could offer you a truce of forty days. If you keep your word and do not molest my lands and subjects, we may talk again...", new ConversationSentence.OnConditionDelegate(this.conversation_lord_ask_pardon_answer_accepted_on_condition), null, 100, null);
			starter.AddPlayerLine("952", "lord_truce_offer", "close_window", "{=bU7EG06q}I accept. Let us stop making war upon each other, for the time being anyway", null, null, 100, null, null);
			starter.AddPlayerLine("953", "lord_truce_offer", "lord_pretalk", "{=d94FO3tS}On second thought, such an accord would not be in my interests.", null, null, 100, null, null);
			starter.AddDialogLine("1175", "lord_give_oath_give_up", "close_window", "{=kvrZ4HIT}Indeed.... Did you offer vassalage, then, just to buy time? Very well -- you shall have time to reconsider, but if you are toying with me, it will do your reputation no credit.", null, new ConversationSentence.OnConsequenceDelegate(this.lord_give_oath_give_up_consequence), 100, null);
			starter.AddDialogLine("vassalage_offer_player_is_already_vassal", "lord_ask_enter_service_vassalage", "lord_give_oath_under_oath_already", "{=tvpAb5qH}You are already oath-bound to serve {FACTION_NAME}, are you not?", new ConversationSentence.OnConditionDelegate(this.conversation_vassalage_offer_player_is_already_vassal_on_condition), null, 100, null);
			starter.AddPlayerLine("vassalage_offer_player_is_already_vassal_player_answer", "lord_give_oath_under_oath_already", "lord_pretalk", "{=q1F03tON}Indeed I am, {LORD_SALUTATION}. Forgive my rambling.", null, null, 100, null, null);
			starter.AddDialogLine("vassalage_offer_player_has_low_relation", "lord_ask_enter_service_vassalage", "lord_pretalk", "{=r3pGLoKN}I accept oaths only from those I can trust to keep them, {PLAYER.NAME}.", new ConversationSentence.OnConditionDelegate(this.conversation_vassalage_offer_player_has_low_relation_on_condition), null, 100, null);
			starter.AddDialogLine("vassalage_offer_accepted", "lord_ask_enter_service_vassalage", "lord_give_oath_1", "{=2c7dIRla}You are known as a brave {?PLAYER.GENDER}warrior{?}warrior{\\?} and a fine leader of men, {PLAYER.NAME}. I shall be pleased to accept your sword into my service, if you are ready to swear homage to me.", new ConversationSentence.OnConditionDelegate(this.conversation_vassalage_offer_accepted_on_condition), null, 100, null);
			starter.AddDialogLine("vassalage_offer_rejected", "lord_ask_enter_service_vassalage", "lord_pretalk", "{=!}{VASSALAGE_REJECTION}[ib:closed]", new ConversationSentence.OnConditionDelegate(this.conversation_reject_vassalage_on_condition), null, 100, null);
			starter.AddPlayerLine("lord_give_oath_1_player_answer_1", "lord_give_oath_1", "lord_give_oath_2", "{=7bETSEg5}I am ready, {LORD.LINK}.", new ConversationSentence.OnConditionDelegate(this.conversation_lord_give_oath_1_player_answer_1_on_condition), null, 100, null, null);
			starter.AddPlayerLine("lord_give_oath_1_player_answer_2", "lord_give_oath_1", "lord_give_oath_give_up", "{=PdKIXiFa}Forgive me, {LORD.LINK}, I must give the matter more thought first...", null, null, 100, null, null);
			starter.AddDialogLine("1194", "lord_give_oath_give_up", "lord_pretalk", "{=fzNqSoeL}Take whatever time you need, my lady.", null, null, 100, null);
			starter.AddDialogLine("1195", "lord_give_oath_give_up", "close_window", "{=c64TS3NS}What are you playing at, {PLAYER.NAME}? Go and make up your mind, and stop wasting my time.", null, null, 100, null);
			starter.AddDialogLine("lord_give_oath_2", "lord_give_oath_2", "lord_give_oath_3", "{=54PbMkNw}Good. Then repeat the words of the oath with me: {OATH_LINE_1}", new ConversationSentence.OnConditionDelegate(this.conversation_set_oath_phrases_on_condition), null, 100, null);
			starter.AddPlayerLine("lord_give_oath_3_answer_1", "lord_give_oath_3", "lord_give_oath_4", "{=!}{OATH_LINE_1}", null, null, 100, null, null);
			starter.AddPlayerLine("lord_give_oath_3_answer_2", "lord_give_oath_3", "lord_give_oath_give_up", "{=8bLwh9yy}Excuse me, {?CONVERSATION_NPC.GENDER}my lady{?}sir{\\?}. But I feel I need to think about this.", null, null, 100, null, null);
			starter.AddDialogLine("1199", "lord_give_oath_4", "lord_give_oath_5", "{=!}{OATH_LINE_2}", null, null, 100, null);
			starter.AddPlayerLine("1200", "lord_give_oath_5", "lord_give_oath_6", "{=!}{OATH_LINE_2}", null, null, 100, null, null);
			starter.AddPlayerLine("1201", "lord_give_oath_5", "lord_give_oath_give_up", "{=LKdrCaTO}{?CONVERSATION_NPC.GENDER}My lady{?}Sir{\\?}, may I ask for some time to think about this?", null, null, 100, null, null);
			starter.AddDialogLine("1202", "lord_give_oath_6", "lord_give_oath_7", "{=!}{OATH_LINE_3}", null, null, 100, null);
			starter.AddPlayerLine("1203", "lord_give_oath_7", "lord_give_oath_8", "{=!}{OATH_LINE_3}", null, null, 100, null, null);
			starter.AddPlayerLine("1204", "lord_give_oath_7", "lord_give_oath_give_up", "{=aa5F4vP5}My {?CONVERSATION_NPC.GENDER}lady{?}lord{\\?}, please give me more time to think about this.", null, null, 100, null, null);
			starter.AddDialogLine("1205", "lord_give_oath_8", "lord_give_oath_9", "{=!}{OATH_LINE_4}", null, null, 100, null);
			starter.AddPlayerLine("1206", "lord_give_oath_9", "lord_give_oath_10", "{=!}{OATH_LINE_4}", null, null, 100, null, null);
			starter.AddPlayerLine("1207", "lord_give_oath_9", "lord_give_oath_give_up", "{=aupbQveh}{?CONVERSATION_NPC.GENDER}Madame{?}Sir{\\?}, I must have more time to consider this.", null, null, 100, null, null);
			starter.AddDialogLine("1208", "lord_give_oath_10", "lord_give_oath_go_on_2", "{=!}{RULER_VASSALAGE_SPEECH}", new ConversationSentence.OnConditionDelegate(this.conversation_lord_give_oath_go_on_condition), null, 100, null);
			starter.AddDialogLine("player_is_accepted_as_a_vassal", "lord_give_oath_go_on_2", "lord_give_oath_go_on_3", "{=XqWloWK0}{PLAYER_ACCEPTED_AS_VASSAL}", new ConversationSentence.OnConditionDelegate(this.conversation_liege_states_obligations_to_vassal_on_condition), new ConversationSentence.OnConsequenceDelegate(this.conversation_player_is_accepted_as_a_vassal_on_consequence), 100, null);
			starter.AddDialogLine("1210", "lord_give_oath_go_on_3", "lord_give_conclude", "{=dT3cdDSg}You have done a wise thing, {PLAYER.NAME}. Serve me well and I promise, you will rise high.", null, null, 100, null);
			starter.AddPlayerLine("1211", "lord_give_conclude", "lord_give_conclude_2", "{=YtM6vzTI}I thank you my {?CONVERSATION_NPC.GENDER}lady{?}lord{\\?}.", null, null, 100, null, null);
			starter.AddDialogLine("1213", "lord_give_conclude_2", "lord_pretalk", "{=ge22yngN}I have great hopes for you {PLAYER.NAME}. I know you shall prove yourself worthy of the trust I have placed in you.", null, null, 100, null);
			starter.AddDialogLine("1220", "lord_ask_leave_service", "lord_ask_leave_service_verify", "{=roIaYqrx}Hmm. Has your oath become burdensome, {PLAYER.NAME}? It is unusual to request release from homage, but in respect of your fine service, I will not hold you if you truly wish to end it. Though you would be sorely missed.", null, null, 100, null);
			starter.AddDialogLine("1221", "lord_ask_leave_service", "lord_ask_leave_service_verify", "{=231s4Fqi}Release from homage? Hmm, perhaps it would be for the best... However, {PLAYER.NAME}, you must be sure that release is what you desire. This is not a thing done lightly.", null, null, 100, null);
			starter.AddPlayerLine("1222", "lord_ask_leave_service_verify", "lord_ask_leave_service_2", "{=sBpcOmoi}It is something I must do, {LORD_SALUTATION}.", null, null, 100, null, null);
			starter.AddPlayerLine("1223", "lord_ask_leave_service_verify", "lord_ask_leave_service_giveup", "{=eaPcNqWR}You are right, {LORD_SALUTATION}. My place is here.", null, null, 100, null, null);
			starter.AddPlayerLine("1224", "lord_ask_leave_service_giveup", "lord_pretalk", "{=fMIVxJU0}I am pleased to hear it, {PLAYER.NAME}. I hope you'll banish such unworthy thoughts from your mind from now on.", null, null, 100, null, null);
			starter.AddDialogLine("1225", "lord_ask_leave_service_2", "lord_ask_leave_service_verify_again", "{=CtEbTd47}Then you are sure? Also, be aware that if you leave my services, you will be surrendering to me all the fiefs which you hold in my name.", null, null, 100, null);
			starter.AddPlayerLine("1226", "lord_ask_leave_service_verify_again", "lord_ask_leave_service_3", "{=IASba7yf}Yes, {LORD_SALUTATION}.", null, null, 100, null, null);
			starter.AddPlayerLine("1227", "lord_ask_leave_service_verify_again", "lord_ask_leave_service_giveup", "{=I80qeGOG}Of course not, {LORD_SALUTATION}. I am ever your loyal vassal.", null, null, 100, null, null);
			starter.AddDialogLine("player_leave_faction_accepted", "lord_ask_leave_service_3", "lord_ask_leave_service_end", "{=xCjCHRcS}As you wish. I hereby declare your oaths to be null and void. You will no longer hold land or titles in my name, and you are released from your duties to my house. You are free, {PLAYER.NAME}.", null, new ConversationSentence.OnConsequenceDelegate(this.conversation_player_leave_faction_accepted_on_consequence), 100, null);
			starter.AddPlayerLine("1229", "lord_ask_leave_service_end", "lord_ask_leave_service_end_2", "{=W7C30eri}Thank you, {?CONVERSATION_NPC.GENDER}madame{?}sir{\\?}. It was an honour to serve you..", null, null, 100, null, null);
			starter.AddPlayerLine("1230", "lord_ask_leave_service_end", "lord_ask_leave_service_end_2", "{=8UyOJNhU}My thanks. It feels good to be {?PLAYER.GENDER}free{?}a free man{\\?} once again.", null, null, 100, null, null);
			starter.AddDialogLine("player_leaves_faction", "lord_ask_leave_service_end_2", "close_window", "{=ZMbvMK6K}Farewell then, {PLAYER.NAME}, and good luck go with you.", null, new ConversationSentence.OnConsequenceDelegate(this.conversation_player_leave_faction_accepted_on_leave), 100, null);
		}

		public static void conversation_player_marriage_list_options_on_consequence()
		{
			List<Hero> list = new List<Hero>();
			if (Hero.OneToOneConversationHero.CanMarry() && Hero.MainHero.IsFemale != Hero.OneToOneConversationHero.IsFemale)
			{
				list.Add(Hero.OneToOneConversationHero);
			}
			foreach (Hero hero in Hero.OneToOneConversationHero.Children)
			{
				if (Campaign.Current.Models.MarriageModel.IsCoupleSuitableForMarriage(Hero.MainHero, hero))
				{
					list.Add(hero);
				}
			}
			list.Clear();
		}

		public bool conversation_player_marriage_select_on_condition()
		{
			Hero hero = ConversationSentence.CurrentProcessedRepeatObject as Hero;
			if (hero == null)
			{
				return false;
			}
			StringHelpers.SetCharacterProperties("MAIDENORSUITOR", hero.CharacterObject, null, false);
			return true;
		}

		public void conversation_player_marriage_on_consequence()
		{
			Hero selectedHero = (Hero)ConversationSentence.SelectedRepeatObject;
			BarterManager instance = BarterManager.Instance;
			Hero mainHero = Hero.MainHero;
			Hero oneToOneConversationHero = Hero.OneToOneConversationHero;
			PartyBase mainParty = PartyBase.MainParty;
			MobileParty partyBelongedTo = Hero.OneToOneConversationHero.PartyBelongedTo;
			instance.StartBarterOffer(mainHero, oneToOneConversationHero, mainParty, (partyBelongedTo != null) ? partyBelongedTo.Party : null, null, (Barterable barterable, BarterData _args, object obj) => BarterManager.Instance.InitializeMarriageBarterContext(barterable, _args, new Tuple<Hero, Hero>(selectedHero, Hero.MainHero)), 0, false, null);
			PlayerEncounter.LeaveEncounter = true;
		}

		public void conversation_player_marriage_on_refusal_consequence()
		{
			PlayerEncounter.LeaveEncounter = true;
		}

		public bool conversation_player_children_marriage_on_condition()
		{
			return !Campaign.Current.Models.MarriageModel.GetAdultChildrenSuitableForMarriage(Hero.MainHero).IsEmpty<Hero>() && !Campaign.Current.Models.MarriageModel.GetAdultChildrenSuitableForMarriage(Hero.OneToOneConversationHero).IsEmpty<Hero>();
		}

		public void conversation_player_children_marriage_list_options_on_consequence()
		{
			List<Tuple<Hero, Hero>> list = new List<Tuple<Hero, Hero>>();
			List<Hero> adultChildrenSuitableForMarriage = Campaign.Current.Models.MarriageModel.GetAdultChildrenSuitableForMarriage(Hero.MainHero);
			List<Hero> adultChildrenSuitableForMarriage2 = Campaign.Current.Models.MarriageModel.GetAdultChildrenSuitableForMarriage(Hero.OneToOneConversationHero);
			if (!adultChildrenSuitableForMarriage2.IsEmpty<Hero>() && !adultChildrenSuitableForMarriage2.IsEmpty<Hero>())
			{
				foreach (Hero hero in adultChildrenSuitableForMarriage)
				{
					foreach (Hero hero2 in adultChildrenSuitableForMarriage2)
					{
						if (Campaign.Current.Models.MarriageModel.IsCoupleSuitableForMarriage(hero, hero2))
						{
							Tuple<Hero, Hero> tuple = new Tuple<Hero, Hero>(hero, hero2);
							list.Add(tuple);
						}
					}
				}
			}
			ConversationSentence.SetObjectsToRepeatOver(list, 5);
			list.Clear();
		}

		public bool conversation_player_children_marriage_select_on_condition()
		{
			Tuple<Hero, Hero> tuple = ConversationSentence.CurrentProcessedRepeatObject as Tuple<Hero, Hero>;
			if (tuple == null)
			{
				return false;
			}
			if (tuple.Item1.IsFemale)
			{
				StringHelpers.SetCharacterProperties("MAIDEN", tuple.Item1.CharacterObject, null, false);
				StringHelpers.SetCharacterProperties("SUITOR", tuple.Item2.CharacterObject, null, false);
			}
			else
			{
				StringHelpers.SetCharacterProperties("SUITOR", tuple.Item1.CharacterObject, null, false);
				StringHelpers.SetCharacterProperties("MAIDEN", tuple.Item2.CharacterObject, null, false);
			}
			return true;
		}

		public void conversation_player_children_marriage_on_consequence()
		{
			Tuple<Hero, Hero> couple = (Tuple<Hero, Hero>)ConversationSentence.SelectedRepeatObject;
			BarterManager instance = BarterManager.Instance;
			Hero mainHero = Hero.MainHero;
			Hero oneToOneConversationHero = Hero.OneToOneConversationHero;
			PartyBase mainParty = PartyBase.MainParty;
			MobileParty partyBelongedTo = Hero.OneToOneConversationHero.PartyBelongedTo;
			instance.StartBarterOffer(mainHero, oneToOneConversationHero, mainParty, (partyBelongedTo != null) ? partyBelongedTo.Party : null, null, (Barterable barterable, BarterData _args, object obj) => BarterManager.Instance.InitializeMarriageBarterContext(barterable, _args, new Tuple<Hero, Hero>(couple.Item2, couple.Item1)), 0, false, null);
			PlayerEncounter.LeaveEncounter = true;
		}

		public void conversation_player_children_marriage_on_refusal_consequence()
		{
			PlayerEncounter.LeaveEncounter = true;
		}

		private void AddPoliticsAndBarter(CampaignGameStarter starter)
		{
			this.AddParleyDialogs(starter);
			starter.AddDialogLine("lord_politics_request", "lord_politics_request", "lord_talk_speak_diplomacy_2", "{=!}{STR_INTRIGUE_AGREEMENT}", new ConversationSentence.OnConditionDelegate(this.conversation_lord_agrees_to_discussion_on_condition), null, 100, null);
			starter.AddPlayerLine("player_want_to_join_faction_as_mercenary_or_vassal", "lord_talk_speak_diplomacy_2", "lord_ask_enter_service", "{=0eAc6WQk}I would like to enter {FACTION_SERVICE_TERM}.", new ConversationSentence.OnConditionDelegate(this.conversation_player_want_to_join_faction_as_mercenary_or_vassal_on_condition), null, 100, null, null);
			starter.AddPlayerLine("player_want_to_end_mercenary_service", "lord_talk_speak_diplomacy_2", "lord_ask_exit_service", "{=LVX2x6Jf}I would like to end my contract with the {SERVED_FACTION}.", new ConversationSentence.OnConditionDelegate(this.conversation_player_want_to_end_service_as_mercenary_on_condition), null, 100, null, null);
			starter.AddDialogLine("player_want_to_end_mercenary_service_response", "lord_ask_exit_service", "lord_ask_exit_service_confirm", "{=EN9s6oZz}Very well. As you're paid for each battle, not for a fixed period of time, you can end it whenever you like.", null, null, 100, null);
			starter.AddPlayerLine("lord_ask_exit_service_confirm", "lord_ask_exit_service_confirm", "lord_ask_exit_service_confirm_final", "{=dy3eiMFo}Let my contract be ended.", null, new ConversationSentence.OnConsequenceDelegate(this.conversation_player_want_to_end_service_as_mercenary_on_consequence), 100, null, null);
			starter.AddPlayerLine("lord_ask_exit_service_confirm_no", "lord_ask_exit_service_confirm", "lord_pretalk", "{=D33fIGQe}Never mind.", null, null, 100, null, null);
			starter.AddDialogLine("lord_ask_exit_service_confirm_final", "lord_ask_exit_service_confirm_final", "lord_pretalk", "{=7Qr1yZhJ}So be it. Come see me again if you want to arrange a new one.", null, null, 100, null);
			starter.AddPlayerLine("player_want_to_hire_mercenary", "lord_talk_speak_diplomacy_2", "player_ask_mercenary_to_join", "{=6eHl9Tz4}I would like you to serve {PLAYER_FACTION} as mercenary.", new ConversationSentence.OnConditionDelegate(this.conversation_player_want_to_hire_mercenary_on_condition), null, 100, null, null);
			starter.AddPlayerLine("player_want_to_fire_mercenary", "lord_talk_speak_diplomacy_2", "player_ask_mercenary_to_leave", "{=xOrwhUVJ}I want to end our contract.", new ConversationSentence.OnConditionDelegate(this.conversation_player_want_to_fire_mercenary_on_condition), null, 100, null, null);
			starter.AddDialogLine("player_want_to_fire_mercenary_paying_debt", "player_ask_mercenary_to_leave", "player_ask_mercenary_to_leave_there_is_debt", "{=aIbR4Nr3}Sure, but first you will need to pay me and my men {GOLD_AMOUNT} denars for our efforts.", new ConversationSentence.OnConditionDelegate(this.conversation_player_want_to_fire_mercenary_there_is_debt_on_condition), null, 100, null);
			starter.AddDialogLine("player_want_to_fire_mercenary_okay", "player_ask_mercenary_to_leave", "lord_pretalk", "{=6po3wjFa}Okay. I hope you will not regret this.", new ConversationSentence.OnConditionDelegate(this.conversation_player_want_to_fire_mercenary_no_debt_on_condition), new ConversationSentence.OnConsequenceDelegate(this.conversation_player_want_to_fire_mercenary_on_consequence), 100, null);
			starter.AddDialogLine("player_want_to_fire_mercenary_not_paying_debt", "player_ask_mercenary_to_leave_no_debt_payment", "lord_pretalk", "{=xbFa2L9A}We will not forget this.", null, null, 100, null);
			starter.AddPlayerLine("player_want_to_fire_mercenary_there_is_debt_accept_payment", "player_ask_mercenary_to_leave_there_is_debt", "player_ask_mercenary_to_leave", "{=zFAkHQRH}I am ready to pay my debt.", new ConversationSentence.OnConditionDelegate(this.conversation_player_want_to_fire_mercenary_with_paying_debt_on_condition), new ConversationSentence.OnConsequenceDelegate(this.conversation_player_want_to_fire_mercenary_with_paying_debt_on_consequence), 100, null, null);
			starter.AddPlayerLine("player_want_to_fire_mercenary_there_is_debt_reject_payment", "player_ask_mercenary_to_leave_there_is_debt", "player_ask_mercenary_to_leave_no_debt_payment", "{=VJbQNVDu}You don't deserve my coin. This contract is over.", null, new ConversationSentence.OnConsequenceDelegate(this.conversation_player_want_to_fire_mercenary_without_paying_debt_on_consequence), 100, null, null);
			starter.AddPlayerLine("player_want_to_fire_mercenary_there_is_debt_think_again", "player_ask_mercenary_to_leave_there_is_debt", "lord_pretalk", "{=HOMeZ9bB}Let me think about it.", null, null, 100, null, null);
			starter.AddDialogLine("player_ask_mercenary_to_join_response_reject", "player_ask_mercenary_to_join", "lord_pretalk", "{=0wTZx8EC}You don't seem trustworthy. I have no interest in your offer.", new ConversationSentence.OnConditionDelegate(this.conversation_mercenary_response_on_condition_reject), null, 100, null);
			starter.AddDialogLine("player_ask_mercenary_to_join_response_reject_poor", "player_ask_mercenary_to_join", "lord_pretalk", "{=BbbBqKUs}You don't have the money to hire us.", new ConversationSentence.OnConditionDelegate(this.conversation_mercenary_response_on_condition_reject_because_of_financial_reasons), null, 100, null);
			starter.AddDialogLine("player_ask_mercenary_to_join_response", "player_ask_mercenary_to_join", "lord_pretalk", "{=VIztLFNQ}You need to discuss this with our leader {LEADER.LINK}.", new ConversationSentence.OnConditionDelegate(this.conversation_mercenary_response_not_leader_on_condition), null, 100, null);
			starter.AddDialogLine("player_ask_mercenary_to_join_response_2", "player_ask_mercenary_to_join", "player_ask_mercenary_to_join_player_response", "{=dfEi6GSE}We will fight for you if you can afford our fee. Please have a look at our terms and see if they are acceptable.", new ConversationSentence.OnConditionDelegate(this.conversation_mercenary_response_on_condition), null, 100, null);
			starter.AddPlayerLine("player_ask_mercenary_to_join_player_response_accept", "player_ask_mercenary_to_join_player_response", "lord_pretalk", "{=Jt0HGGlR}This is fair. Join us", null, new ConversationSentence.OnConsequenceDelegate(this.conversation_mercenary_response_accept_on_consqequence), 100, new ConversationSentence.OnClickableConditionDelegate(this.conversation_mercenary_response_accept_reject_on_clickable_condition), null);
			starter.AddPlayerLine("player_ask_mercenary_to_join_player_response_reject", "player_ask_mercenary_to_join_player_response", "lord_pretalk", "{=a2GQ3VKM}I cannot afford this.", null, null, 100, new ConversationSentence.OnClickableConditionDelegate(this.conversation_mercenary_response_accept_reject_on_clickable_condition), null);
			starter.AddPlayerLine("player_ask_to_claim_land", "lord_talk_speak_diplomacy_2", "player_ask_to_claim_land", "{=73MYdXby}I want to claim {SETTLEMENT}.", new ConversationSentence.OnConditionDelegate(this.conversation_player_ask_to_claim_land_on_condition), null, 100, null, null);
			starter.AddPlayerLine("player_ask_to_join_players_party", "lord_talk_speak_diplomacy_2", "player_ask_to_join_players_party", "{=FiZVzLV3}I want you to join my party.", new ConversationSentence.OnConditionDelegate(LordConversationsCampaignBehavior.player_ask_to_join_players_party_on_condition), null, 100, null, null);
			starter.AddPlayerLine("player_wants_to_make_peace", "lord_talk_speak_diplomacy_2", "lord_talk_speak_diplomacy_3", "{=ldJDR7t1}Our realms should make peace.", new ConversationSentence.OnConditionDelegate(this.conversation_player_wants_to_make_peace_on_condition), null, 100, null, null);
			starter.AddDialogLine("player_wants_to_make_peace_npc_response", "lord_talk_speak_diplomacy_3", "player_wants_to_make_peace_answer", "{=!}{LORD_PEACE_OFFER_ANSWER}", new ConversationSentence.OnConditionDelegate(this.conversation_player_wants_to_make_peace_answer_on_condition), new ConversationSentence.OnConsequenceDelegate(this.conversation_player_wants_to_make_peace_on_consequence), 100, null);
			starter.AddPlayerLine("player_wants_to_make_peace_result", "player_wants_to_make_peace_answer", "lord_pretalk", "{=EaIO20WN}So be it.", null, null, 100, null, null);
			starter.AddDialogLine("player_ask_to_claim_land_answer", "player_ask_to_claim_land", "player_ask_to_claim_land_answer_player_decision", "{=EcYGHEVS}You may be entitled to that land in recompense for your service to us. [Will cost {VALUE} influence]", new ConversationSentence.OnConditionDelegate(LordConversationsCampaignBehavior.conversation_player_ask_to_claim_land_answer_on_condition), null, 100, null);
			starter.AddPlayerLine("player_ask_to_claim_land_answer_player_decision_ok", "player_ask_to_claim_land_answer_player_decision", "lord_pretalk", "{=gbXLb2WE}I shall claim it.", new ConversationSentence.OnConditionDelegate(this.conversation_player_ask_to_claim_land_answer_ok_on_condition), new ConversationSentence.OnConsequenceDelegate(this.conversation_player_ask_to_claim_land_answer_on_consequence), 100, null, null);
			starter.AddPlayerLine("player_ask_to_claim_land_answer_player_decision_no", "player_ask_to_claim_land_answer_player_decision", "lord_pretalk", "{=vyaIbCRj}Hmm... Perhaps I will need to do more.", null, null, 100, null, null);
			starter.AddDialogLine("player_ask_to_join_players_party_response", "player_ask_to_join_players_party", "close_window", "{=DlJt6YLY}I will be honored. See you outside.[DEBUG]", null, new ConversationSentence.OnConsequenceDelegate(LordConversationsCampaignBehavior.player_ask_to_join_players_party_on_consequence), 100, null);
			starter.AddPlayerLine("hero_barter", "lord_talk_speak_diplomacy_2", "lord_considers_barter", "{=CuFvVPNt}I have a proposal that may benefit us both.", new ConversationSentence.OnConditionDelegate(this.conversation_lord_barter_on_condition), null, 100, null, null);
			starter.AddDialogLine("lord_considers_barter", "lord_considers_barter", "lord_barter_line", "{=anrenBiB}I am listening.", new ConversationSentence.OnConditionDelegate(this.conversation_can_lord_barter), null, 100, null);
			starter.AddDialogLine("lord_refuses_barter", "lord_considers_barter", "lord_start", "{=3L8xN9uC}I believe it hasn't been long since we've last bartered.", null, null, 100, null);
			starter.AddDialogLine("barter_decision_thinking", "lord_barter_line", "lord_post_barter", "{=Xpekpwby}Barter line - player should not see this", null, new ConversationSentence.OnConsequenceDelegate(this.conversation_set_up_generic_barter_on_consequence), 100, null);
			starter.AddDialogLine("barter_decision_thinking_2", "lord_post_barter", "lord_pretalk", "{=!}{BARTER_CONCLUSION_LINE}", null, null, 100, null);
			starter.AddPlayerLine("lord_ask_ruling_philosophy", "lord_talk_speak_diplomacy_2", "lord_ruling_philosophy", "{=9QulsaxG}Do you have any general thoughts on politics?", new ConversationSentence.OnConditionDelegate(this.conversation_player_ask_ruling_philosophy_on_condition), null, 100, null, null);
			starter.AddDialogLine("lord_ruling_philosophy_long_start", "lord_ruling_philosophy", "lord_ruling_philosophy_long_1", "{=Z27lsnCO}Well. Let me put things this way...", new ConversationSentence.OnConditionDelegate(this.conversation_player_has_long_ruling_philosophy_on_condition), null, 100, null);
			starter.AddDialogLine("lord_ruling_philosophy_long_1", "lord_ruling_philosophy_long_1", "lord_ruling_philosophy_long_2", "{=!}{RULING_PHILOSOPHY}", new ConversationSentence.OnConditionDelegate(this.conversation_player_has_long_ruling_philosophy_on_condition), null, 100, null);
			starter.AddDialogLine("lord_ruling_philosophy_long_2", "lord_ruling_philosophy_long_2", "lord_ruling_philosophy_long_3", "{=!}{RULING_PHILOSOPHY_2}", null, null, 100, null);
			starter.AddDialogLine("lord_ruling_philosophy_long_3", "lord_ruling_philosophy_long_3", "lord_pretalk", "{=!}{RULING_PHILOSOPHY_3}", null, null, 100, null);
			starter.AddDialogLine("lord_ruling_philosophy_short", "lord_ruling_philosophy", "lord_pretalk", "{=!}{RULING_PHILOSOPHY}", null, null, 100, null);
			starter.AddDialogLine("lord_considers_barter_2", "lord_considers_barter", "lord_barter_pre_decision", "{=anrenBiB}I am listening.", new ConversationSentence.OnConditionDelegate(this.conversation_can_lord_barter), null, 100, null);
			starter.AddDialogLine("lord_refuses_barter_2", "lord_considers_barter", "lord_start", "{=3L8xN9uC}I believe it hasn't been long since we've last bartered.", null, null, 100, null);
			starter.AddDialogLine("lord_considers_army", "lord_considers_army", "lord_pretalk", "{=90ROmHcV}Very well. Follow us.", null, new ConversationSentence.OnConsequenceDelegate(this.lord_considers_army_on_consequence), 100, null);
			starter.AddDialogLine("lord_considers_joining_player_army", "lord_considers_joining_player_army", "lord_pretalk", "{=ao7gZafg}Very well. We will come with you.", null, new ConversationSentence.OnConsequenceDelegate(this.lord_considers_joining_player_army_on_consequence), 100, null);
			starter.AddDialogLine("lord_responds_to_changing_sides", "lord_barter_pre_decision_change_sides", "lord_barter_decision_change_sides", "{=WOXMzO3Z}I must think carefully about this.", null, null, 100, null);
			starter.AddDialogLine("barter_decision_refuses", "lord_barter_decision_change_sides", "lord_pretalk", "{=xhKJAmQM}{?BARTER_RESULT}{STR_BARTER_DECLINE_OFFER}{?}{STR_CHANGE_SIDES_DECLINE_OFFER}{\\?}", new ConversationSentence.OnConditionDelegate(this.barter_offer_reject_on_condition), new ConversationSentence.OnConsequenceDelegate(this.conversation_lord_leave_on_consequence), 100, null);
			starter.AddDialogLine("barter_decision_refuses_2", "lord_barter_decision_change_sides", "close_window", "{=Za6Du9Kf}Then you will pay with blood!", new ConversationSentence.OnConditionDelegate(this.barter_peace_offer_reject_on_condition), null, 100, null);
		}

		private void AddIntroductions(CampaignGameStarter starter)
		{
			starter.AddDialogLine("lord_introduction", "lord_introduction", "lord_start", "{=B7rEq40B}{LORD_INTRODUCTION_STRING} {TOWN_INFO_STRING}", new ConversationSentence.OnConditionDelegate(this.conversation_lord_introduction_on_condition), null, 100, null);
			starter.AddDialogLine("rebel_introduction", "lord_introduction", "lord_start", "{=!}{REBEL_INTRODUCTION_STRING}", new ConversationSentence.OnConditionDelegate(this.conversation_rebel_introduction_on_condition), null, 100, null);
			starter.AddDialogLine("merchant_introduction", "lord_introduction", "lord_start", "{=!}{MERCHANT_INTRODUCTION_STRING}", new ConversationSentence.OnConditionDelegate(this.conversation_merchant_introduction_on_condition), null, 100, null);
			starter.AddDialogLine("minor_faction_preacher_introduction", "lord_introduction", "lord_start", "{=amdgO9Hr}I am {CONVERSATION_HERO.FIRSTNAME}. I am a humble follower of the fellowship known as the {FACTION_NAME}, whom the {DIVINITY} have chosen to bear their message in this present Age.", new ConversationSentence.OnConditionDelegate(this.conversation_minor_faction_preacher_introduction_on_condition), null, 100, null);
			starter.AddDialogLine("puritan_preacher_introduction", "lord_introduction", "lord_start", "{=ewVEM7Y0}I am {CONVERSATION_HERO.FIRSTNAME}, {FACTION_DESCRIPTION}. I have come here because many have gone astray, and listen to false preachers who distort the true meaning of the divine revelations.", new ConversationSentence.OnConditionDelegate(this.conversation_puritan_preacher_introduction_on_condition), null, 100, null);
			starter.AddDialogLine("messianic_preacher_introduction", "lord_introduction", "lord_start", "{=mTHwgF8U}I am {CONVERSATION_HERO.FIRSTNAME}, {FACTION_DESCRIPTION}. I have come here to warn people that the time when the Heavens tolerate injustice -- those times are coming to an end.", new ConversationSentence.OnConditionDelegate(this.conversation_messianic_preacher_introduction_on_condition), null, 100, null);
			starter.AddDialogLine("mystic_preacher_introduction", "lord_introduction", "lord_start", "{=nFqt6bIk}I am {CONVERSATION_HERO.FIRSTNAME}, {FACTION_DESCRIPTION}. I have come here to share some fragments of the wisdom of the {DIVINITY} that I have been blessed with understanding.", new ConversationSentence.OnConditionDelegate(this.conversation_mystic_preacher_introduction_on_condition), null, 100, null);
			starter.AddDialogLine("special_notable_introduction", "lord_introduction", "lord_start", "{=RCUGYkGP}I am {CONVERSATION_HERO.FIRSTNAME}, {FACTION_DESCRIPTION}.", new ConversationSentence.OnConditionDelegate(this.conversation_special_notable_introduction_on_condition), null, 100, null);
			starter.AddDialogLine("gangleader_introduction_4", "lord_introduction", "lord_start", "{=DOKcia5B}I'm {CONVERSATION_HERO.FIRSTNAME}. There's some who know me as {CONVERSATION_HERO.LINK}. That's a term of respect, by the way.[if:convo_mocking_revenge]", new ConversationSentence.OnConditionDelegate(this.conversation_calculating_gangleader_introduction_on_condition), null, 100, null);
			starter.AddDialogLine("gangleader_introduction", "lord_introduction", "lord_start", "{=zbWsjfbn}I'm {CONVERSATION_HERO.FIRSTNAME}. Ask around about me. Let's just say I've got a talent for solving people's problems, so to speak.[if:convo_bemused]", new ConversationSentence.OnConditionDelegate(this.conversation_ironic_gangleader_introduction_on_condition), null, 100, null);
			starter.AddDialogLine("gangleader_introduction_2", "lord_introduction", "lord_start", "{=wVRwj2ff}I'm {CONVERSATION_HERO.FIRSTNAME}. Ask around about me. You'll learn I'm someone you don't want to mess with.[if:convo_stern]", new ConversationSentence.OnConditionDelegate(this.conversation_cruel_gangleader_introduction_on_condition), null, 100, null);
			starter.AddDialogLine("gangleader_introduction_3", "lord_introduction", "lord_start", "{=iJd59BPZ}I'm {CONVERSATION_HERO.FIRSTNAME}. Ask around about me. You'll be told I keep the peace in the back alleys.", new ConversationSentence.OnConditionDelegate(this.conversation_default_gangleader_introduction_on_condition), null, 100, null);
			starter.AddDialogLine("artisan_introduction", "lord_introduction", "lord_start", "{=ntEBl43n}I am {CONVERSATION_HERO.FIRSTNAME}. I'm a craftsman, a working man. A lot of the other honest men here in {TOWN_NAME}, the ones that work with their hands, they like me to speak for them.", new ConversationSentence.OnConditionDelegate(this.conversation_artisan_introduction_on_condition), null, 100, null);
			starter.AddDialogLine("headman_introduction", "lord_introduction", "lord_start", "{=eOSJHbBD}I'm {CONVERSATION_HERO.FIRSTNAME}. I've lived all my life here, working the land, as do my kin. A lot of the people here in {VILLAGE_NAME}, the common farmers and craftsmen like me, they like me to speak for them.", new ConversationSentence.OnConditionDelegate(this.conversation_headman_introduction_on_condition), null, 100, null);
			starter.AddDialogLine("rural_notable_introduction", "lord_introduction", "lord_start", "{=1bhjQFOe}I am {CONVERSATION_HERO.FIRSTNAME}. I own land around here. I speak for many of the people in this village.", new ConversationSentence.OnConditionDelegate(this.conversation_rural_notable_introduction_on_condition), null, 100, null);
			starter.AddDialogLine("minor_faction_generic_intro", "lord_introduction", "lord_start", "{=!}{MINOR_FACTION_INTRODUCTION_STRING}", new ConversationSentence.OnConditionDelegate(this.conversation_minor_faction_introduction_on_condition), null, 100, null);
		}

		private void AddWandererConversations(CampaignGameStarter starter)
		{
			starter.AddPlayerLine("wanderer_meet_player_response1", "wanderer_meet_player_response", "wanderer_preintroduction", "{=wFXj0bqj}My name is {PLAYER.NAME}, {?CONVERSATION_NPC.GENDER}madam{?}sir{\\?}. Tell me about yourself.", new ConversationSentence.OnConditionDelegate(this.conversation_wanderer_meet_player_on_condition), null, 100, null, null);
			starter.AddPlayerLine("wanderer_meet_player_response2", "wanderer_meet_player_response", "wanderer_skip_intro", "{=3hEmXhaW}I'm {PLAYER.NAME}. Let's skip the pleasantries and get right to business.", new ConversationSentence.OnConditionDelegate(this.conversation_wanderer_meet_player_on_condition), null, 100, null, null);
			starter.AddDialogLine("wanderer_skip_intro", "wanderer_skip_intro", "hero_main_options", "{=LUiQ6bpo}Very well, then. What is it?", null, null, 100, null);
			starter.AddDialogLine("wanderer_prebackstory", "wanderer_preintroduction", "wanderer_introduction_a", "{=!}{WANDERER_PREBACKSTORY}", new ConversationSentence.OnConditionDelegate(this.conversation_wanderer_preintroduction_on_condition), null, 100, null);
			starter.AddDialogLine("wanderer_introduction_a", "wanderer_introduction_a", "wanderer_introduction_b", "{=!}{WANDERER_BACKSTORY_A}", new ConversationSentence.OnConditionDelegate(this.conversation_wanderer_introduction_on_condition), null, 100, null);
			starter.AddDialogLine("wanderer_introduction_b", "wanderer_introduction_b", "wanderer_introduction_c", "{=!}{WANDERER_BACKSTORY_B}", null, null, 100, null);
			starter.AddDialogLine("wanderer_introduction_c", "wanderer_introduction_c", "wanderer_player_reaction", "{=!}{WANDERER_BACKSTORY_C}", null, null, 100, null);
			starter.AddPlayerLine("wanderer_meet_player_response1_2", "wanderer_player_reaction", "wanderer_introduction_d", "{=!}{BACKSTORY_RESPONSE_1}", null, null, 100, null, null);
			starter.AddPlayerLine("wanderer_meet_player_response2_2", "wanderer_player_reaction", "wanderer_introduction_d", "{=!}{BACKSTORY_RESPONSE_2}", null, null, 100, null, null);
			starter.AddDialogLine("wanderer_introduction_d", "wanderer_introduction_d", "wanderer_job_status", "{=!}{WANDERER_BACKSTORY_D}", null, null, 100, null);
			starter.AddDialogLine("wanderer_job_status_1", "wanderer_job_status", "hero_main_options", "{=EUBxMVXk}Do you have any orders for your alley, {?CONVERSATION_NPC.GENDER}madam{?}sir{\\?}", new ConversationSentence.OnConditionDelegate(this.conversation_wanderer_player_owned_on_condition), null, 100, null);
			starter.AddDialogLine("wanderer_job_status_1_2", "wanderer_job_status", "hero_main_options", "{=HVdZI3C1}Right now I'm working for {EMPLOYER}.", new ConversationSentence.OnConditionDelegate(this.conversation_wanderer_job_status_on_condition), null, 100, null);
			starter.AddDialogLine("wanderer_job_status_2", "wanderer_job_status", "hero_main_options", "{=!}{WANDERER_JOB_OFFER}", new ConversationSentence.OnConditionDelegate(this.conversation_wanderer_set_job_line_on_condition), null, 100, null);
			starter.AddDialogLine("wanderer_backstory_generic", "wanderer_preintroduction", "hero_main_options", "{=!}{WANDERER_GENERIC_BACKSTORY}", new ConversationSentence.OnConditionDelegate(this.conversation_wanderer_generic_introduction_on_condition), null, 100, null);
			starter.AddDialogLine("wanderer_backstory_generic_2", "wanderer_introduction_a", "hero_main_options", "{=!}{WANDERER_GENERIC_BACKSTORY}", new ConversationSentence.OnConditionDelegate(this.conversation_wanderer_generic_introduction_on_condition), null, 100, null);
		}

		private void AddAnimationTestConversations(CampaignGameStarter starter)
		{
			starter.AddPlayerLine("test_frown2", "lord_talk_ask_something_2", "lord_expression_test_frown2", "{=!}Frown and strike a fighting stance, please", () => Game.Current.IsDevelopmentMode, null, 100, null, null);
			starter.AddDialogLine("lord_expression_test_frown", "lord_expression_test_frown2", "lord_pretalk", "{=!}(Frowns using the internal set)", () => Game.Current.IsDevelopmentMode, null, 100, null);
			starter.AddPlayerLine("lord_animation", "hero_main_options", "lord_animation_tests", "{=!}Let's do animation tests.", () => Game.Current.IsDevelopmentMode, null, 100, null, null);
			starter.AddDialogLine("lord_animation_di", "lord_animation_tests", "lord_animation_tests_select", "{=!}Which ones?.", () => Game.Current.IsDevelopmentMode, null, 100, null);
			starter.AddPlayerLine("lord_animation_2", "lord_animation_tests_select", "lord_pretalk", "{=!}We're done. Go back.", () => Game.Current.IsDevelopmentMode, null, 100, null, null);
			starter.AddPlayerLine("test_expressions", "lord_animation_tests_select", "lord_test_expressions", "{=!}Let's test expressions", () => Game.Current.IsDevelopmentMode, null, 100, null, null);
			starter.AddDialogLine("lord_test_expressions", "lord_test_expressions", "lord_select_test_expression", "{=!}Test which expression?", () => Game.Current.IsDevelopmentMode, null, 100, null);
			starter.AddPlayerLine("lord_test_expressions_bared_teeth", "lord_select_test_expression", "lord_expression_test_bared_teeth", "{=!}bared_teeth face please", () => Game.Current.IsDevelopmentMode, null, 100, null, null);
			starter.AddDialogLine("lord_expression_test_bared_teeth", "lord_expression_test_bared_teeth", "lord_test_expressions", "{=!}(uses convo_bared_teeth)[if:convo_bared_teeth][ib:aggressive]", () => Game.Current.IsDevelopmentMode, null, 100, null);
			starter.AddPlayerLine("lord_test_expressions_dismayed", "lord_select_test_expression", "lord_expression_test_dismayed", "{=!}dismayed face please", () => Game.Current.IsDevelopmentMode, null, 100, null, null);
			starter.AddDialogLine("lord_expression_test_dismayed", "lord_expression_test_dismayed", "lord_test_expressions", "{=!}(uses convo_dismayed)[if:convo_dismayed][ib:nervous]", () => Game.Current.IsDevelopmentMode, null, 100, null);
			starter.AddPlayerLine("lord_test_expressions_aggressive", "lord_select_test_expression", "lord_expression_test_aggressive", "{=!}aggressive face please", () => Game.Current.IsDevelopmentMode, null, 100, null, null);
			starter.AddDialogLine("lord_expression_test_aggressive", "lord_expression_test_aggressive", "lord_test_expressions", "{=!}(uses convo_aggressive)[if:convo_aggressive][ib:aggressive2]", () => Game.Current.IsDevelopmentMode, null, 100, null);
			starter.AddPlayerLine("lord_test_expressions_predatory", "lord_select_test_expression", "lord_expression_test_predatory", "{=!}predatory face please", () => Game.Current.IsDevelopmentMode, null, 100, null, null);
			starter.AddDialogLine("lord_expression_test_predatory", "lord_expression_test_predatory", "lord_test_expressions", "{=!}(uses convo_predatory)[if:convo_predatory][ib:warrior]", () => Game.Current.IsDevelopmentMode, null, 100, null);
			starter.AddPlayerLine("lord_test_expressions_insulted", "lord_select_test_expression", "lord_expression_test_insulted", "{=!}Insulted face, please", () => Game.Current.IsDevelopmentMode, null, 100, null, null);
			starter.AddDialogLine("lord_expression_test_insulted", "lord_expression_test_insulted", "lord_test_expressions", "{=!}(Insulted face)[if:convo_insulted][ib:hip2]", () => Game.Current.IsDevelopmentMode, null, 100, null);
			starter.AddPlayerLine("lord_test_expressions_angry", "lord_select_test_expression", "lord_expression_test_angry", "{=!}angry face, please", () => Game.Current.IsDevelopmentMode, null, 100, null, null);
			starter.AddDialogLine("lord_expression_test_angry", "lord_expression_test_angry", "lord_test_expressions", "{=!}(angry face)[if:convo_angry][ib:warrior2]", () => Game.Current.IsDevelopmentMode, null, 100, null);
			starter.AddPlayerLine("lord_test_expressions_furious", "lord_select_test_expression", "lord_expression_test_furious", "{=!}Furious face, please", () => Game.Current.IsDevelopmentMode, null, 100, null, null);
			starter.AddDialogLine("lord_expression_test_furious", "lord_expression_test_furious", "lord_test_expressions", "{=!}(Furious face)[if:convo_furious][ib:aggressive]", () => Game.Current.IsDevelopmentMode, null, 100, null);
			starter.AddPlayerLine("lord_test_expressions_annoyed", "lord_select_test_expression", "lord_expression_test_annoyed", "{=!}annoyed face, please", () => Game.Current.IsDevelopmentMode, null, 100, null, null);
			starter.AddDialogLine("lord_expression_test_annoyed", "lord_expression_test_annoyed", "lord_test_expressions", "{=!}(annoyed face)[if:convo_annoyed][ib:closed]", () => Game.Current.IsDevelopmentMode, null, 100, null);
			starter.AddPlayerLine("lord_test_expressions_confused_annoyed", "lord_select_test_expression", "lord_expression_test_confused_annoyed", "{=!}confused_annoyed face, please", () => Game.Current.IsDevelopmentMode, null, 100, null, null);
			starter.AddDialogLine("lord_expression_test_confused_annoyed", "lord_expression_test_confused_annoyed", "lord_test_expressions", "{=!}(confused_annoyed face)[if:convo_confused_annoyed][ib:normal2]", () => Game.Current.IsDevelopmentMode, null, 100, null);
			starter.AddPlayerLine("lord_test_expressions_bored", "lord_select_test_expression", "lord_expression_test_bored", "{=!}Be bored", () => Game.Current.IsDevelopmentMode, null, 100, null, null);
			starter.AddDialogLine("lord_expression_test_bored", "lord_expression_test_bored", "lord_test_expressions", "{=!}(uses convo_bored plus action)[ib:closed][if:convo_bored]", () => Game.Current.IsDevelopmentMode, null, 100, null);
			starter.AddPlayerLine("lord_test_expressions_bored2", "lord_select_test_expression", "lord_expression_test_bored2", "{=!}Be bored2", () => Game.Current.IsDevelopmentMode, null, 100, null, null);
			starter.AddDialogLine("lord_expression_test_bored2", "lord_expression_test_bored2", "lord_test_expressions", "{=!}(uses convo_bored2 plus action)[ib:closed2][if:convo_bored2]", () => Game.Current.IsDevelopmentMode, null, 100, null);
			starter.AddPlayerLine("lord_test_expressions_beaten", "lord_select_test_expression", "lord_expression_test_beaten", "{=!}Be beaten", () => Game.Current.IsDevelopmentMode, null, 100, null, null);
			starter.AddDialogLine("lord_expression_test_beaten", "lord_expression_test_beaten", "lord_test_expressions", "{=!}(uses convo_beaten plus action)[ib:weary][if:convo_beaten]", () => Game.Current.IsDevelopmentMode, null, 100, null);
			starter.AddPlayerLine("lord_test_expressions_nervous", "lord_select_test_expression", "lord_expression_test_nervous", "{=!}Be nervous", () => Game.Current.IsDevelopmentMode, null, 100, null, null);
			starter.AddDialogLine("lord_expression_test_nervous", "lord_expression_test_nervous", "lord_test_expressions", "{=!}(uses convo_nervous plus action)[ib:weary2][if:convo_nervous]", () => Game.Current.IsDevelopmentMode, null, 100, null);
			starter.AddPlayerLine("lord_test_expressions_shocked", "lord_select_test_expression", "lord_expression_test_shocked", "{=!}Be shocked", () => Game.Current.IsDevelopmentMode, null, 100, null, null);
			starter.AddDialogLine("lord_expression_test_shocked", "lord_expression_test_shocked", "lord_test_expressions", "{=!}(uses convo_shocked plus action)[ib:nervous][if:convo_shocked]", () => Game.Current.IsDevelopmentMode, null, 100, null);
			starter.AddPlayerLine("lord_test_expressions_confused_normal", "lord_select_test_expression", "lord_expression_test_confused_normal", "{=!}Be confused_normal", () => Game.Current.IsDevelopmentMode, null, 100, null, null);
			starter.AddDialogLine("lord_expression_test_confused_normal", "lord_expression_test_confused_normal", "lord_test_expressions", "{=!}(uses convo_confused_normal plus action)[ib:nervous2][if:convo_confused_normal]", () => Game.Current.IsDevelopmentMode, null, 100, null);
			starter.AddPlayerLine("lord_test_expressions_happy", "lord_select_test_expression", "lord_expression_test_happy", "{=!}Happy face, please", () => Game.Current.IsDevelopmentMode, null, 100, null, null);
			starter.AddDialogLine("lord_expression_test_happy", "lord_expression_test_happy", "lord_test_expressions", "{=!}(happy face)[ib:normal][if:happy]", () => Game.Current.IsDevelopmentMode, null, 100, null);
			starter.AddPlayerLine("lord_test_expressions_friendly", "lord_select_test_expression", "lord_expression_test_friendly", "{=!}Calm Friendly face please", () => Game.Current.IsDevelopmentMode, null, 100, null, null);
			starter.AddDialogLine("lord_expression_test_friendly", "lord_expression_test_friendly", "lord_test_expressions", "{=!}(uses convo_calm_friendly)[ib:normal2][if:convo_calm_friendly]", () => Game.Current.IsDevelopmentMode, null, 100, null);
			starter.AddPlayerLine("lord_test_expressions_relaxed_happy", "lord_select_test_expression", "lord_expression_test_relaxed_happy", "{=!}Relaxed Happy face, please", () => Game.Current.IsDevelopmentMode, null, 100, null, null);
			starter.AddDialogLine("lord_expression_test_relaxed_happy", "lord_expression_test_relaxed_happy", "lord_test_expressions", "{=!}(relaxed happy face)[ib:confident][if:convo_relaxed_happy]", () => Game.Current.IsDevelopmentMode, null, 100, null);
			starter.AddPlayerLine("lord_test_expressions_focused_happy", "lord_select_test_expression", "lord_expression_test_focused_happy", "{=!}Focused Happy face, please", () => Game.Current.IsDevelopmentMode, null, 100, null, null);
			starter.AddDialogLine("lord_expression_test_focused_happy", "lord_expression_test_focused_happy", "lord_test_expressions", "{=!}(focused happy face)[ib:confident2][if:convo_focused_happy]", () => Game.Current.IsDevelopmentMode, null, 100, null);
			starter.AddPlayerLine("lord_test_expressions_bemused", "lord_select_test_expression", "lord_expression_test_bemused", "{=!}Bemused face, please", () => Game.Current.IsDevelopmentMode, null, 100, null, null);
			starter.AddDialogLine("lord_expression_test_bemused", "lord_expression_test_bemused", "lord_test_expressions", "{=!}(Bemused face)[ib:demure][if:convo_bemused]", () => Game.Current.IsDevelopmentMode, null, 100, null);
			starter.AddPlayerLine("lord_test_expressions_merry", "lord_select_test_expression", "lord_expression_test_merry", "{=!}Be merry", () => Game.Current.IsDevelopmentMode, null, 100, null, null);
			starter.AddDialogLine("lord_expression_test_merry", "lord_expression_test_merry", "lord_test_expressions", "{=!}(uses convo_merry plus action)[ib:demure2][if:convo_merry]", () => Game.Current.IsDevelopmentMode, null, 100, null);
			starter.AddPlayerLine("lord_test_expressions_delighted", "lord_select_test_expression", "lord_expression_test_delighted", "{=!}Be delighted", () => Game.Current.IsDevelopmentMode, null, 100, null, null);
			starter.AddDialogLine("lord_expression_test_delighted", "lord_expression_test_delighted", "lord_test_expressions", "{=!}(uses convo_delighted plus action)[ib:aggressive][if:convo_delighted]", null, null, 100, null);
			starter.AddPlayerLine("lord_test_expressions_approving", "lord_select_test_expression", "lord_expression_test_approving", "{=!}Approving face please", () => Game.Current.IsDevelopmentMode, null, 100, null, null);
			starter.AddDialogLine("lord_expression_test_approving", "lord_expression_test_approving", "lord_test_expressions", "{=!}(uses convo_approving)[ib:aggressive2][if:convo_approving]", () => Game.Current.IsDevelopmentMode, null, 100, null);
			starter.AddPlayerLine("lord_test_expressions_excited", "lord_select_test_expression", "lord_expression_test_excited", "{=!}Be excited", () => Game.Current.IsDevelopmentMode, null, 100, null, null);
			starter.AddDialogLine("lord_expression_test_excited", "lord_expression_test_excited", "lord_test_expressions", "{=!}(uses convo_excited plus action)[ib:confident3][if:convo_excited]", () => Game.Current.IsDevelopmentMode, null, 100, null);
			starter.AddPlayerLine("lord_test_expressions_grave", "lord_select_test_expression", "lord_expression_test_grave", "{=!}Grave face please", () => Game.Current.IsDevelopmentMode, null, 100, null, null);
			starter.AddDialogLine("lord_expression_test_grave", "lord_expression_test_grave", "lord_test_expressions", "{=!}(uses convo_grave)[ib:warrior][if:convo_grave]", () => Game.Current.IsDevelopmentMode, null, 100, null);
			starter.AddPlayerLine("lord_test_expressions_stern", "lord_select_test_expression", "lord_expression_test_stern", "{=!}stern face please", () => Game.Current.IsDevelopmentMode, null, 100, null, null);
			starter.AddDialogLine("lord_expression_test_stern", "lord_expression_test_stern", "lord_test_expressions", "{=!}(uses convo_stern)[ib:warrior2][if:convo_stern]", () => Game.Current.IsDevelopmentMode, null, 100, null);
			starter.AddPlayerLine("lord_test_expressions_very_stern", "lord_select_test_expression", "lord_expression_test_very_stern", "{=!}very_stern face please", () => Game.Current.IsDevelopmentMode, null, 100, null, null);
			starter.AddDialogLine("lord_expression_test_very_stern", "lord_expression_test_very_stern", "lord_test_expressions", "{=!}(uses convo_very_stern)[ib:closed2][if:convo_very_stern]", () => Game.Current.IsDevelopmentMode, null, 100, null);
			starter.AddPlayerLine("lord_test_expressions_undecided_closed", "lord_select_test_expression", "lord_expression_test_undecided_closed", "{=!}undecided_closed face please", () => Game.Current.IsDevelopmentMode, null, 100, null, null);
			starter.AddDialogLine("lord_expression_test_undecided_closed", "lord_expression_test_undecided_closed", "lord_test_expressions", "{=!}(uses convo_undecided_closed)[ib:nervous2][if:convo_undecided_closed]", () => Game.Current.IsDevelopmentMode, null, 100, null);
			starter.AddPlayerLine("lord_test_expressions_undecided_open", "lord_select_test_expression", "lord_expression_test_undecided_open", "{=!}undecided_open face please", () => Game.Current.IsDevelopmentMode, null, 100, null, null);
			starter.AddDialogLine("lord_expression_test_undecided_open", "lord_expression_test_undecided_open", "lord_test_expressions", "{=!}(uses convo_undecided_open)[ib:aggressive][if:convo_undecided_open]", () => Game.Current.IsDevelopmentMode, null, 100, null);
			starter.AddPlayerLine("lord_test_expressions_contemptuous", "lord_select_test_expression", "lord_expression_test_contemptuous", "{=!}Be contemptuous", () => Game.Current.IsDevelopmentMode, null, 100, null, null);
			starter.AddDialogLine("lord_expression_test_contemptuous", "lord_expression_test_contemptuous", "lord_test_expressions", "{=!}(uses convo_contemptuous plus action)[ib:aggressive2][if:convo_contemptuous]", () => Game.Current.IsDevelopmentMode, null, 100, null);
			starter.AddPlayerLine("lord_test_expressions_mocking_teasing", "lord_select_test_expression", "lord_expression_test_mocking_teasing", "{=!}mocking_teasing face please", () => Game.Current.IsDevelopmentMode, null, 100, null, null);
			starter.AddDialogLine("lord_expression_test_mocking_teasing", "lord_expression_test_mocking_teasing", "lord_test_expressions", "{=!}(uses convo_mocking_teasing)[ib:normal2][if:convo_mocking_teasing]", () => Game.Current.IsDevelopmentMode, null, 100, null);
			starter.AddPlayerLine("lord_test_expressions_mocking_revenge", "lord_select_test_expression", "lord_expression_test_mocking_revenge", "{=!}mocking_revenge face please", () => Game.Current.IsDevelopmentMode, null, 100, null, null);
			starter.AddDialogLine("lord_expression_test_mocking_revenge", "lord_expression_test_mocking_revenge", "lord_test_expressions", "{=!}(uses convo_mocking_revenge)[ib:confident3][if:convo_mocking_revenge]", () => Game.Current.IsDevelopmentMode, null, 100, null);
			starter.AddPlayerLine("lord_test_expressions_mocking_aristocratic", "lord_select_test_expression", "lord_expression_test_mocking_aristocratic", "{=!}mocking_aristocratic face please", () => Game.Current.IsDevelopmentMode, null, 100, null, null);
			starter.AddDialogLine("lord_expression_test_mocking_aristocratic", "lord_expression_test_mocking_aristocratic", "lord_test_expressions", "{=!}(uses convo_mocking_aristocratic)[ib:hip][if:convo_mocking_aristocratic]", () => Game.Current.IsDevelopmentMode, null, 100, null);
			starter.AddPlayerLine("lord_test_expressions_nonchalant", "lord_select_test_expression", "lord_expression_test_nonchalant", "{=!}Nonchalant face please", () => Game.Current.IsDevelopmentMode, null, 100, null, null);
			starter.AddDialogLine("lord_expression_test_nonchalant", "lord_expression_test_nonchalant", "lord_test_expressions", "{=!}(uses convo_nonchalant)[ib:hip2][if:convo_nonchalant]", () => Game.Current.IsDevelopmentMode, null, 100, null);
			starter.AddPlayerLine("lord_test_expressions_done", "lord_select_test_expression", "lord_pretalk", "{=!}That will be all", () => Game.Current.IsDevelopmentMode, null, 100, null, null);
			starter.AddPlayerLine("test_postures", "lord_animation_tests_select", "lord_test_postures", "{=!}Let's test postures", () => Game.Current.IsDevelopmentMode, null, 100, null, null);
			starter.AddDialogLine("lord_test_postures", "lord_test_postures", "lord_select_test_posture", "{=!}Test which posture?", () => Game.Current.IsDevelopmentMode, null, 100, null);
			starter.AddPlayerLine("lord_test_postures_normal", "lord_select_test_posture", "lord_test_posture_normal", "{=!}normal posture please", () => Game.Current.IsDevelopmentMode, null, 100, null, null);
			starter.AddDialogLine("lord_posture_test_normal", "lord_test_posture_normal", "lord_test_postures", "{=!}(uses normal2 posture)[ib:normal]", () => Game.Current.IsDevelopmentMode, null, 100, null);
			starter.AddPlayerLine("lord_test_postures_normal2", "lord_select_test_posture", "lord_test_posture_normal2", "{=!}normal2 posture please", () => Game.Current.IsDevelopmentMode, null, 100, null, null);
			starter.AddDialogLine("lord_posture_test_normal2", "lord_test_posture_normal2", "lord_test_postures", "{=!}(uses normal2 posture)[ib:normal2]", () => Game.Current.IsDevelopmentMode, null, 100, null);
			starter.AddPlayerLine("lord_test_postures_aggressive", "lord_select_test_posture", "lord_test_posture_aggressive", "{=!}aggressive posture please", () => Game.Current.IsDevelopmentMode, null, 100, null, null);
			starter.AddDialogLine("lord_posture_test_aggressive", "lord_test_posture_aggressive", "lord_test_postures", "{=!}(uses aggressive posture)[ib:aggressive]", () => Game.Current.IsDevelopmentMode, null, 100, null);
			starter.AddPlayerLine("lord_test_postures_aggressive2", "lord_select_test_posture", "lord_test_posture_aggressive2", "{=!}aggressive2 posture please", () => Game.Current.IsDevelopmentMode, null, 100, null, null);
			starter.AddDialogLine("lord_posture_test_aggressive2", "lord_test_posture_aggressive2", "lord_test_postures", "{=!}(uses aggressive2 posture)[ib:aggressive2]", () => Game.Current.IsDevelopmentMode, null, 100, null);
			starter.AddPlayerLine("lord_test_postures_warrior", "lord_select_test_posture", "lord_test_posture_warrior", "{=!}Warrior posture please", () => Game.Current.IsDevelopmentMode, null, 100, null, null);
			starter.AddDialogLine("lord_posture_test_warrior", "lord_test_posture_warrior", "lord_test_postures", "{=!}(uses warrior posture)[ib:warrior]", () => Game.Current.IsDevelopmentMode, null, 100, null);
			starter.AddPlayerLine("lord_test_postures_warrior2", "lord_select_test_posture", "lord_test_posture_warrior2", "{=!}Warrior2 posture please", () => Game.Current.IsDevelopmentMode, null, 100, null, null);
			starter.AddDialogLine("lord_posture_test_warrior2", "lord_test_posture_warrior2", "lord_test_postures", "{=!}(uses warrior2 posture)[ib:warrior2]", () => Game.Current.IsDevelopmentMode, null, 100, null);
			starter.AddPlayerLine("lord_test_postures_hip", "lord_select_test_posture", "lord_test_posture_hip", "{=!}hip posture please", () => Game.Current.IsDevelopmentMode, null, 100, null, null);
			starter.AddDialogLine("lord_posture_test_hip", "lord_test_posture_hip", "lord_test_postures", "{=!}(uses hip posture)[ib:hip]", () => Game.Current.IsDevelopmentMode, null, 100, null);
			starter.AddPlayerLine("lord_test_postures_hip2", "lord_select_test_posture", "lord_test_posture_hip2", "{=!}hip2 posture please", () => Game.Current.IsDevelopmentMode, null, 100, null, null);
			starter.AddDialogLine("lord_posture_test_hip2", "lord_test_posture_hip2", "lord_test_postures", "{=!}(uses hip2 posture)[ib:hip2]", () => Game.Current.IsDevelopmentMode, null, 100, null);
			starter.AddPlayerLine("lord_test_postures_closed", "lord_select_test_posture", "lord_test_posture_closed", "{=!}Closed posture please", () => Game.Current.IsDevelopmentMode, null, 100, null, null);
			starter.AddDialogLine("lord_posture_test_closed", "lord_test_posture_closed", "lord_test_postures", "{=!}(uses closed posture)[ib:closed]", () => Game.Current.IsDevelopmentMode, null, 100, null);
			starter.AddPlayerLine("lord_test_postures_closed2", "lord_select_test_posture", "lord_test_posture_closed2", "{=!}Closed2 posture please", () => Game.Current.IsDevelopmentMode, null, 100, null, null);
			starter.AddDialogLine("lord_posture_test_closed2", "lord_test_posture_closed2", "lord_test_postures", "{=!}(uses closed2 posture)[ib:closed2]", () => Game.Current.IsDevelopmentMode, null, 100, null);
			starter.AddPlayerLine("lord_test_postures_confident", "lord_select_test_posture", "lord_test_posture_confident", "{=!}confident posture please", () => Game.Current.IsDevelopmentMode, null, 100, null, null);
			starter.AddDialogLine("lord_posture_test_confident", "lord_test_posture_confident", "lord_test_postures", "{=!}(uses confident posture)[ib:confident]", () => Game.Current.IsDevelopmentMode, null, 100, null);
			starter.AddPlayerLine("lord_test_postures_confident2", "lord_select_test_posture", "lord_test_posture_confident2", "{=!}confident2 posture please", () => Game.Current.IsDevelopmentMode, null, 100, null, null);
			starter.AddDialogLine("lord_posture_test_confident2", "lord_test_posture_confident2", "lord_test_postures", "{=!}(uses confident2 posture)[ib:confident2]", () => Game.Current.IsDevelopmentMode, null, 100, null);
			starter.AddPlayerLine("lord_test_postures_confident3", "lord_select_test_posture", "lord_test_posture_confident3", "{=!}Confident3 posture please", () => Game.Current.IsDevelopmentMode, null, 100, null, null);
			starter.AddDialogLine("lord_posture_test_confident3", "lord_test_posture_confident3", "lord_test_postures", "{=!}(uses confident3 posture)[ib:confident3]", () => Game.Current.IsDevelopmentMode, null, 100, null);
			starter.AddPlayerLine("lord_test_postures_demure", "lord_select_test_posture", "lord_test_posture_demure", "{=!}demure posture please", () => Game.Current.IsDevelopmentMode, null, 100, null, null);
			starter.AddDialogLine("lord_posture_test_demure", "lord_test_posture_demure", "lord_test_postures", "{=!}(uses demure posture)[ib:demure]", () => Game.Current.IsDevelopmentMode, null, 100, null);
			starter.AddPlayerLine("lord_test_postures_demure2", "lord_select_test_posture", "lord_test_posture_demure2", "{=!}demure2 posture please", () => Game.Current.IsDevelopmentMode, null, 100, null, null);
			starter.AddDialogLine("lord_posture_test_demure2", "lord_test_posture_demure2", "lord_test_postures", "{=!}(uses demure2 posture)[ib:demure2]", () => Game.Current.IsDevelopmentMode, null, 100, null);
			starter.AddPlayerLine("lord_test_postures_nervous", "lord_select_test_posture", "lord_test_posture_nervous", "{=!}nervous posture please", () => Game.Current.IsDevelopmentMode, null, 100, null, null);
			starter.AddDialogLine("lord_posture_test_nervous", "lord_test_posture_nervous", "lord_test_postures", "{=!}(uses nervous posture)[ib:nervous]", () => Game.Current.IsDevelopmentMode, null, 100, null);
			starter.AddPlayerLine("lord_test_postures_nervous2", "lord_select_test_posture", "lord_test_posture_nervous2", "{=!}nervous2 posture please", () => Game.Current.IsDevelopmentMode, null, 100, null, null);
			starter.AddDialogLine("lord_posture_test_nervous2", "lord_test_posture_nervous2", "lord_test_postures", "{=!}(uses nervous2 posture)[ib:nervous2]", () => Game.Current.IsDevelopmentMode, null, 100, null);
			starter.AddPlayerLine("lord_test_postures_weary", "lord_select_test_posture", "lord_test_posture_weary", "{=!}weary posture please", () => Game.Current.IsDevelopmentMode, null, 100, null, null);
			starter.AddDialogLine("lord_posture_test_weary", "lord_test_posture_weary", "lord_test_postures", "{=!}(uses weary posture)[ib:weary]", () => Game.Current.IsDevelopmentMode, null, 100, null);
			starter.AddPlayerLine("lord_test_posture_weary2", "lord_select_test_posture", "lord_test_posture_weary2", "{=!}weary2 posture please", () => Game.Current.IsDevelopmentMode, null, 100, null, null);
			starter.AddDialogLine("lord_posture_test_weary2", "lord_test_posture_weary2", "lord_test_postures", "{=!}(uses weary2 posture)[ib:weary2]", () => Game.Current.IsDevelopmentMode, null, 100, null);
			starter.AddPlayerLine("lord_test_postures_done", "lord_select_test_posture", "lord_pretalk", "{=!}That will be all", () => Game.Current.IsDevelopmentMode, null, 100, null, null);
		}

		private void AddParleyDialogs(CampaignGameStarter starter)
		{
			starter.AddPlayerLine("lord_barter_let_go", "lord_talk_speak_diplomacy_2", "lord_considers_letting_player_go", "{=ak2ZPOce}What would it take for you to let me go my way?", new ConversationSentence.OnConditionDelegate(this.conversation_player_can_ask_for_siege_to_be_lifted_on_condition), null, 100, null, null);
			starter.AddPlayerLine("lord_barter_let_go_2", "lord_talk_speak_diplomacy_2", "lord_considers_letting_player_go", "{=ymuVaD4h}What would it take for you to go your way, and for me to go my way?", new ConversationSentence.OnConditionDelegate(this.conversation_player_can_bribe_lord_for_passage_on_condition), null, 100, null, null);
			starter.AddDialogLine("lord_considers_letting_player_go", "lord_considers_letting_player_go", "lord_pretalk", "{=!}{REFUSE_BARTER_LINE}", new ConversationSentence.OnConditionDelegate(this.conversation_lord_refuses_siege_lift_on_condition), null, 100, null);
			starter.AddDialogLine("lord_considers_letting_player_go_2", "lord_considers_letting_player_go", "lord_barter_pre_decision_safe_passage", "{=5bbvuAFf}What are you offering?", null, null, 100, null);
			starter.AddDialogLine("barter_decision_thinking_3", "lord_barter_pre_decision_safe_passage", "lord_barter_decision_safe_passage", "{=EPhBiTxd}Barter line - you should not see this.", null, new ConversationSentence.OnConsequenceDelegate(this.conversation_set_up_safe_passage_barter_on_consequence), 100, null);
			starter.AddDialogLine("barter_with_lord_postbarter_1", "lord_barter_decision_safe_passage", "close_window", "{=zcf9M1Qh}Very well... You may go.", new ConversationSentence.OnConditionDelegate(this.conversation_barter_successful_on_condition), null, 100, null);
			starter.AddDialogLine("barter_with_lord_postbarter_2", "lord_barter_decision_safe_passage", "close_window", "{=1gvHI0TH}Ah... Well, I am afraid that is not enough.", () => !this.conversation_barter_successful_on_condition(), null, 100, null);
		}

		private void let_go_prisoner_start_on_consequence()
		{
			Hero mainHero = Hero.MainHero;
			Hero oneToOneConversationHero = Hero.OneToOneConversationHero;
			MobileParty partyBelongedTo = Hero.OneToOneConversationHero.PartyBelongedTo;
			int valueForFaction = new SetPrisonerFreeBarterable(mainHero, oneToOneConversationHero, (partyBelongedTo != null) ? partyBelongedTo.Party : null, Hero.OneToOneConversationHero).GetValueForFaction(Hero.OneToOneConversationHero.MapFaction);
			ConversationManager.StartPersuasion((float)valueForFaction, (float)valueForFaction * 0.2f, (float)valueForFaction * 0f, (float)valueForFaction * 0.4f, (float)valueForFaction * -0.4f, -1f, PersuasionDifficulty.Medium);
		}

		private void AddHeroGeneralConversations(CampaignGameStarter starter)
		{
			starter.AddDialogLine("hero_pretalk", "lord_pretalk", "player_responds_to_surrender_demand", "{=!}{SURRENDER_DEMAND_STRING}", new ConversationSentence.OnConditionDelegate(this.conversation_lord_makes_surrender_demand_on_condition), null, 100, null);
			starter.AddDialogLine("hero_pretalk_2", "lord_pretalk", "hero_main_options", "{=DQBaaC0e}Is there anything else?", null, null, 100, null);
			starter.AddPlayerLine("main_option_hostile_1", "hero_main_options", "lord_predemand", "{=VrnlUvV8}I'm here to deliver you my demands!", new ConversationSentence.OnConditionDelegate(this.conversation_lord_is_threated_neutral_on_condition), null, 100, null, null);
			starter.AddDialogLine("lord_predemand", "lord_predemand", "lord_demand", "{=fBt8X6Tw}Eh? What do you want?", null, null, 100, null);
			starter.AddPlayerLine("lord_ultimatum", "lord_demand", "lord_ultimatum_surrender", "{=gZSdus34}I offer you one chance to surrender or die.", null, null, 100, null, null);
			starter.AddPlayerLine("lord_forgive_me", "lord_demand", "lord_pretalk", "{=M7O3AItb}Forgive me. It's nothing.", null, null, 100, null, null);
			starter.AddDialogLine("lord_attack", "lord_ultimatum_surrender", "lord_attack_verify", "{=ltS8zmH8}Are you mad? I'm not your enemy.", null, null, 100, null);
			starter.AddPlayerLine("lord_attack_verify1", "lord_attack_verify", "lord_attack_verify_cancel", "{=HEJOdRwi}Forgive me, {?CONVERSATION_NPC.GENDER}madame{?}sir{\\?}. I don't know what I was thinking.", null, new ConversationSentence.OnConsequenceDelegate(this.conversation_lord_attack_verify_cancel_on_consequence), 100, null, null);
			starter.AddDialogLine("lord_attack_verify2", "lord_attack_verify_cancel", "close_window", "{=vsxVIDqT}Be gone, then.", null, null, 100, null);
			starter.AddDialogLine("lord_ultimatum_surrender", "lord_ultimatum_surrender", "lord_attack_verify_b", "{=!}{s43}", null, null, 100, null);
			starter.AddPlayerLine("lord_attack_verify_b1", "lord_attack_verify_b", "lord_attack_verify_cancel", "{=8xvSu9fX}Forgive me {?CONVERSATION_NPC.GENDER}madame{?}sir{\\?}. I don't know what I was thinking.", null, null, 100, null, null);
			starter.AddPlayerLine("lord_attack_verify_b2", "lord_attack_verify_b", "lord_attack_verify_commit", "{=HKMdHYb7}I stand my ground. Prepare to fight!", null, null, 100, null, null);
			starter.AddDialogLine("lord_attack_verify_commit", "lord_attack_verify_commit", "close_window", "{=!}You should not see this.", null, null, 100, null);
			starter.AddPlayerLine("main_option_hostile_1_2", "hero_main_options", "player_threatens_enemy_lord", "{=PitXi5n6}You know we are at war. Surrender or die.", new ConversationSentence.OnConditionDelegate(this.conversation_player_can_attack_hero_on_condition), null, 100, new ConversationSentence.OnClickableConditionDelegate(this.conversation_player_can_attack_hero_on_clickable_condition), null);
			starter.AddPlayerLine("545", "player_responds_to_surrender_demand", "party_encounter_lord_hostile_attacker_3", "{=jBN2LlgF}We'll fight to our last drop of blood!", null, null, 100, null, null);
			starter.AddPlayerLine("546", "player_responds_to_surrender_demand", "party_encounter_player_makes_frivolous_surrender_demand", "{=F9LlA26R}Actually, I think you're the one who ought to surrender.", null, null, 100, null, null);
			starter.AddPlayerLine("pay_off_minor_faction_option", "player_responds_to_surrender_demand", "player_responds_to_gold_offer_demand", "{=FREVkpWP}I might be willing to pay for passage.", new ConversationSentence.OnConditionDelegate(this.conversation_pay_minor_faction_for_passage), null, 100, null, null);
			starter.AddDialogLine("paying_gold_option", "player_responds_to_gold_offer_demand", "lord_barter_pre_decision_safe_passage", "{=!}{AGREE_TO_TAKE_PAYMENT}[ib:aggressive]", new ConversationSentence.OnConditionDelegate(this.conversation_can_pay_minor_faction_for_payoff_set_text_on_condition), null, 100, null);
			starter.AddDialogLineWithVariation("frivolous_surrender_demand_response", "party_encounter_player_makes_frivolous_surrender_demand", "close_window", null, null, 100, "", "", "", "", null).Variation(new object[] { "{=6ykZ0Agl}So we fight.[if:idle_angry][ib:warrior]", "DefaultTag", 1 }).Variation(new object[] { "{=ekFDe1I7}Right...Well, I gave you a chance.[if:idle_angry][ib:warrior]", "PersonaSoftspokenTag", 1 })
				.Variation(new object[] { "{=jvic7wfc}Bah... Parley's over.[if:idle_angry][ib:warrior] ", "PersonaCurtTag", 1, "UncharitableTag", 1 })
				.Variation(new object[] { "{=70s1eahS}Hmf. I am amused. So amused that I might have to cut out your tongue after the battle and keep it to remind me of your wit.[if:idle_angry][ib:warrior] ", "PersonaIronicTag", 1, "HighRegisterTag", 1, "CruelTag", 1 })
				.Variation(new object[] { "{=ZnZAPDMo}So be it. You may ply your wit on the carrion-fowl.[if:idle_angry][ib:warrior]", "PersonaIronicTag", 1, "TribalRegisterTag", -1, "CruelTag", 1 });
			starter.AddDialogLineWithVariation("player_turns_down_surrender", "party_encounter_lord_hostile_attacker_3", "close_window", null, null, 100, "", "", "", "", null).Variation(new object[] { "{=QWzGkQrT}So we fight, then.[if:idle_angry][ib:warrior]", "DefaultTag", 1 }).Variation(new object[] { "{=6i7a1c4E}Very well. Death before dishonor![if:idle_angry][ib:warrior]", "PersonaEarnestTag", 1, "ChivalrousTag", 1 })
				.Variation(new object[] { "{=FMJuPZlm}I'm not surrendering, so do what you must.[if:idle_angry][ib:warrior]", "ChivalrousTag", 1 })
				.Variation(new object[] { "{=ZG6kWWwW}I'm not yielding, so let's go to it, then.[if:idle_angry][ib:warrior]", "PersonaCurtTag", 1 })
				.Variation(new object[] { "{=SPYDUXvx}We meet on the battlefield, then.[if:idle_angry][ib:warrior]", "PersonaSoftspokenTag", 1 })
				.Variation(new object[] { "{=3WA4MLzx}One way or the other, you'll regret this. If I fall, my people will have their revenge.[if:idle_angry][ib:warrior]", "UncharitableTag", 1 })
				.Variation(new object[] { "{=yzzY4uXN}Very well. Expect no mercy.[if:idle_angry][ib:warrior]", "CruelTag", 1, "FriendlyRelationshipTag", -1 });
			starter.AddPlayerLine("hero_give_issue", "hero_main_options", "issue_offer", "{=Kfbqriuh}I heard you may need some help with a problem?", new ConversationSentence.OnConditionDelegate(this.conversation_hero_main_options_have_issue_on_condition), null, 110, new ConversationSentence.OnClickableConditionDelegate(this.conversation_hero_main_options_have_issue_on_clickable_condition), null);
			starter.AddPlayerLine("hero_task_given", "hero_main_options", "quest_discuss", "{=dlBFVkDj}About the task you gave me...", new ConversationSentence.OnConditionDelegate(this.conversation_lord_task_given_on_condition), null, 100, null, null);
			starter.AddPlayerLine("hero_task_given_alternative", "hero_main_options", "issue_discuss_alternative_solution", "{=dlBFVkDj}About the task you gave me...", new ConversationSentence.OnConditionDelegate(this.conversation_lord_task_given_alternative_on_condition), null, 100, null, null);
			starter.AddPlayerLine("main_option_faction_hire", "hero_main_options", "companion_hire", "{=OlKbD2fa}I can use someone like you in my company.", new ConversationSentence.OnConditionDelegate(this.conversation_hero_hire_on_condition), null, 100, null, null);
			starter.AddPlayerLine("main_option_discussions_1", "hero_main_options", "lord_considers_army", "{=lord_conversations_227}I want to join your army.", new ConversationSentence.OnConditionDelegate(this.conversation_lord_join_army_on_condition), null, 100, new ConversationSentence.OnClickableConditionDelegate(this.conversation_lord_join_army_on_clickable_condition), null);
			starter.AddPlayerLine("main_option_discussions_2", "hero_main_options", "lord_considers_joining_player_army", "{=XD7xYD0U}I want you to join my army. ({INFLUENCE_COST}{INFLUENCE_ICON})", new ConversationSentence.OnConditionDelegate(this.player_ask_to_join_players_army_on_condition), new ConversationSentence.OnConsequenceDelegate(this.player_ask_to_join_players_army_on_consequence), 100, new ConversationSentence.OnClickableConditionDelegate(this.player_ask_to_join_players_army_on_clickable_condition), null);
			starter.AddPlayerLine("main_option_discussions_3", "hero_main_options", "lord_politics_request", "{=lord_conversations_343}There is something I'd like to discuss.", new ConversationSentence.OnConditionDelegate(this.conversation_hero_main_options_discussions), null, 100, null, null);
			starter.AddPlayerLine("main_option_discussions_4", "hero_main_options", "lord_politics_request", "{=lord_conversations_344}I have a proposal that may spare us both unnecessary bloodshed.", new ConversationSentence.OnConditionDelegate(this.conversation_lord_talk_politics_during_siege_parley_on_condition), null, 100, null, null);
			starter.AddPlayerLine("547", "player_responds_to_surrender_demand", "lord_politics_request", "{=lord_conversations_345}Stay your hand! Perhaps we don't have to come to blows.", new ConversationSentence.OnConditionDelegate(this.conversation_uses_pay_for_passage_lines), null, 100, null, null);
			starter.AddPlayerLine("main_option_questions_1", "hero_main_options", "lord_talk_ask_something", "{=b0m2DxeG}I have a quick question.", new ConversationSentence.OnConditionDelegate(this.conversation_player_has_question_on_condition), null, 100, null, null);
			starter.AddPlayerLine("main_option_prisoner_interaction", "hero_main_options", "lord_talk_about_prisoner", "{=QLbyXqiV}Can I talk to one of your prisoners?", new ConversationSentence.OnConditionDelegate(this.conversation_player_ask_prisoners_on_condition), new ConversationSentence.OnConsequenceDelegate(this.conversation_player_ask_prisoners_on_consequence), 100, null, null);
			starter.AddDialogLine("main_option_prisoner_interaction_lord_answer_1", "lord_talk_about_prisoner", "hero_main_options", "{=ogdIceFH}You? No, that's forbidden.", new ConversationSentence.OnConditionDelegate(this.conversation_player_ask_prisoners_forbidden_on_condition), null, 100, null);
			starter.AddDialogLine("main_option_prisoner_interaction_lord_answer_2", "lord_talk_about_prisoner", "lord_talk_about_prisoners_list_prisoners", "{=St6NxphR}Be my guest, who would you like to talk to?", null, null, 100, null);
			starter.AddRepeatablePlayerLine("main_option_prisoner_interaction_list_prisoners", "lord_talk_about_prisoners_list_prisoners", "lord_talk_about_prisoners_list_prisoner_selected", "{=!}{PRISONER_NAME}", "I was thinking of a different prisoner", "lord_talk_about_prisoner", new ConversationSentence.OnConditionDelegate(this.conversation_player_ask_prisoners_list_on_condition), new ConversationSentence.OnConsequenceDelegate(this.conversation_player_ask_prisoners_list_on_consequence), 100, null);
			starter.AddPlayerLine("main_option_prisoner_interaction_list_prisoners_cancel", "lord_talk_about_prisoners_list_prisoners", "lord_talk_about_prisoners_list_prisoners_cancel", "{=D33fIGQe}Never mind.", new ConversationSentence.OnConditionDelegate(this.conversation_player_ask_prisoners_list_on_condition), new ConversationSentence.OnConsequenceDelegate(this.conversation_player_ask_prisoners_list_on_consequence), 100, null, null);
			starter.AddDialogLine("main_option_prisoner_interaction_list_prisoners_cancel_answer", "lord_talk_about_prisoners_list_prisoners_cancel", "hero_main_options", "{=VT1hSCaw}All right.", null, null, 100, null);
			starter.AddDialogLine("main_option_prisoner_interaction_list_prisoners_selected", "lord_talk_about_prisoners_list_prisoner_selected", "lord_talk_about_prisoners_list_prisoner_selected_final", "{=Pqsbndn1}All right, Be my guest.", null, null, 100, null);
			starter.AddPlayerLine("main_option_prisoner_interaction_list_prisoners_selected_2", "lord_talk_about_prisoners_list_prisoner_selected_final", "close_window", "{=g8qb3Ame}Thank you.", null, new ConversationSentence.OnConsequenceDelegate(this.lord_talk_to_selected_prisoner_on_consequence), 100, null, null);
			starter.AddPlayerLine("player_is_leaving_neutral_or_friendly", "hero_main_options", "hero_leave", "{=9mBy0qNW}I must leave now.", new ConversationSentence.OnConditionDelegate(this.conversation_player_is_leaving_neutral_or_friendly_on_condition), null, 1, null, null);
			starter.AddPlayerLine("player_is_leaving_enemy_polite", "hero_main_options", "hero_leave", "{=XHTumMB9}I must beg my leave.", new ConversationSentence.OnConditionDelegate(this.conversation_player_is_leaving_enemy_on_condition), null, 100, null, null);
			starter.AddPlayerLine("player_is_leaving_enemy_prisoner", "hero_main_options", "prisoner_hero_leave", "{=4NYPsxgY}I need to leave. Good-bye, for now.", new ConversationSentence.OnConditionDelegate(this.conversation_player_is_leaving_enemy_prisoner_on_condition), null, 1, null, null);
			starter.AddPlayerLine("player_is_leaving_surrender", "player_responds_to_surrender_demand", "close_window", "{=za78F8gO}Don't attack! We surrender.", new ConversationSentence.OnConditionDelegate(this.conversation_player_dont_attack_we_surrender_on_condition), new ConversationSentence.OnConsequenceDelegate(this.conversation_player_is_leaving_surrender_on_consequence), 100, null, null);
			starter.AddPlayerLine("lord_diagnostics", "hero_main_options", "lord_diagnostics", "{=Ht3S4nvm}Let's do some diagnostics about your faction.", () => Game.Current.IsDevelopmentMode, null, 100, null, null);
			starter.AddPlayerLine("549", "player_responds_to_surrender_demand", "lord_diagnostics", "{=Ht3S4nvm}Let's do some diagnostics about your faction.", () => Game.Current.IsDevelopmentMode, null, 100, null, null);
			starter.AddPlayerLine("clan_member_manage_troops", "hero_main_options", "clan_member_manage_troops_party_screen", "{=TQKXkQAT}Let me inspect your troops.", new ConversationSentence.OnConditionDelegate(this.conversation_clan_member_manage_troops_on_condition), null, 100, null, null);
			starter.AddDialogLine("clan_member_manage_troops_screen", "clan_member_manage_troops_party_screen", "lord_pretalk", "{=!}party screen goes here.", null, new ConversationSentence.OnConsequenceDelegate(this.conversation_clan_member_manage_troops_on_consequence), 100, null);
			starter.AddDialogLine("companion_hire", "companion_hire", "player_companion_hire_response", "{=!}{HIRING_COST_EXPLANATION}", new ConversationSentence.OnConditionDelegate(this.conversation_companion_hire_gold_on_condition), null, 100, null);
			starter.AddPlayerLine("companion_hire_capacity_full", "player_companion_hire_response", "lord_pretalk", "{=afdN8ZU7}Thinking again, I already have more companions than I can manage.", new ConversationSentence.OnConditionDelegate(this.too_many_companions), null, 100, null, null);
			starter.AddPlayerLine("player_companion_hire_response_1", "player_companion_hire_response", "hero_leave", "{=EiFPu9Np}Right... {GOLD_AMOUNT} Here you are.", new ConversationSentence.OnConditionDelegate(this.conversation_companion_hire_on_condition), new ConversationSentence.OnConsequenceDelegate(this.conversation_companion_hire_on_consequence), 100, null, null);
			starter.AddPlayerLine("player_companion_hire_response_2", "player_companion_hire_response", "lord_pretalk", "{=65UMAav2}I can't afford that just now.", () => !this.too_many_companions(), null, 100, null, null);
			starter.AddDialogLine("player_has_plans_options", "player_has_plans", "player_has_plans_options", "{=pWYkIkKj}Very well, {PLAYER_SALUTATION}. Tell me what you have in mind.", new ConversationSentence.OnConditionDelegate(this.conversation_set_player_salutation), null, 100, null);
			starter.AddPlayerLine("player_ask_lords_to_follow_him", "player_has_plans_options", "player_ask_lords_to_follow_him_answer", "{=YmWJEAQZ}I need you to follow me for a while.", new ConversationSentence.OnConditionDelegate(this.conversation_player_ask_lords_to_follow_him_on_condition), null, 100, null, null);
			starter.AddPlayerLine("player_ask_lords_to_create_army", "player_has_plans_options", "player_ask_lords_to_create_army", "{=Wapbcyxx}I'm gathering an army. Summon the lords to rally to my banner. {AMOUNT}{GOLD_ICON}", new ConversationSentence.OnConditionDelegate(this.conversation_player_ask_lords_to_create_army_on_condition), new ConversationSentence.OnConsequenceDelegate(this.conversation_player_ask_lords_to_follow_him_already_at_army_on_consequence), 100, null, null);
			starter.AddPlayerLine("player_plans_never_mind", "player_has_plans_options", "lord_pretalk", "{=D33fIGQe}Never mind.", new ConversationSentence.OnConditionDelegate(this.conversation_player_plans_never_mind_on_condition), null, 100, null, null);
			starter.AddDialogLine("player_ask_lords_to_create_army_answer", "player_ask_lords_to_create_army", "lord_pretalk", "{=8DQF5kka}I shall inform any lord I meet that you are gathering the army, then.", null, null, 100, null);
			starter.AddDialogLine("player_ask_lords_to_follow_him_already_at_army", "player_ask_lords_to_follow_him_answer", "lord_pretalk", "{=Sktmhpbn}I am already currently escorting {ESCORTING_PARTY}. I cannot follow you until this task is complete.", new ConversationSentence.OnConditionDelegate(this.conversation_player_ask_lords_to_follow_him_already_at_army_on_condition), null, 100, null);
			starter.AddDialogLine("player_ask_lords_to_follow_him_2", "player_ask_lords_to_follow_him_answer", "how_many_days_to_escort", "{=2PPwmNmg}I can escort you for 10 days. After that, I must return to my own affairs.", new ConversationSentence.OnConditionDelegate(this.conversation_player_ask_lords_to_follow_him_on_condition), new ConversationSentence.OnConsequenceDelegate(this.conversation_player_ask_lords_to_follow_him_on_consequence), 100, null);
			starter.AddPlayerLine("how_many_days_to_escort_never_mind", "how_many_days_to_escort", "lord_answer_to_escorting", "{=3JGRrENx}So be it. Let's go. {AMOUNT}{GOLD_ICON}", null, new ConversationSentence.OnConsequenceDelegate(this.conversation_how_many_days_to_escort_on_consqeuence), 100, null, null);
			starter.AddPlayerLine("how_many_days_to_escort_never_mind_2", "how_many_days_to_escort", "lord_pretalk", "{=D33fIGQe}Never mind.", null, null, 100, null, null);
			starter.AddDialogLine("lord_answer_to_escorting", "lord_answer_to_escorting", "close_window", "{=ixyzGRPw}Lead on, then.", null, null, 100, null);
			starter.AddPlayerLine("player_want_to_leave_faction", "hero_main_options", "lord_ask_leave_service", "{=201kVrNa}{SALUTATION_BY_PLAYER}, I wish to be released from my oath to you.", new ConversationSentence.OnConditionDelegate(this.conversation_player_is_leaving_faction_on_condition), null, 100, null, null);
			starter.AddDialogLine("player_want_to_join_faction_as_vassal_lord_answer_di", "lord_ask_enter_service", "lord_pretalk", "{=CVClzSC7}I believe you have already pledged yourself to another liege.", new ConversationSentence.OnConditionDelegate(this.conversation_player_is_asking_service_while_in_faction_on_condition), null, 100, null);
			starter.AddDialogLine("player_want_to_join_faction_as_mercenary_or_vassal_answer", "lord_ask_enter_service", "lord_ask_enter_service_answer", "{=MlTofjrU}And how would you serve us?", null, null, 100, null);
			starter.AddPlayerLine("player_is_offering_mercenary", "lord_ask_enter_service_answer", "lord_ask_enter_service_mercenary", "{=Wuxn9sDq}My sword is yours. For the right sum.", new ConversationSentence.OnConditionDelegate(this.conversation_player_is_offering_mercenary_on_condition), null, 100, new ConversationSentence.OnClickableConditionDelegate(this.conversation_player_is_offering_mercenary_on_clickable_condition), null);
			starter.AddPlayerLine("player_want_to_join_faction_as_vassal_lord_answer", "lord_ask_enter_service_vassalage_player_response", "lord_pretalk", "{=SvGFarpZ}Then I will find and talk to {KING.LINK}.", null, null, 100, null, null);
			starter.AddPlayerLine("player_is_offering_vassalage", "lord_ask_enter_service_answer", "lord_ask_enter_service_vassalage", "{=meVKYu9a}{SALUTATION_BY_PLAYER}, I would pledge allegiance to you and be counted among your loyal followers.", new ConversationSentence.OnConditionDelegate(this.conversation_player_is_offering_vassalage_on_condition), null, 100, new ConversationSentence.OnClickableConditionDelegate(this.conversation_player_is_offering_vassalage_on_clickable_condition), null);
			starter.AddPlayerLine("player_is_offering_vassalage_while_mercenary", "lord_talk_speak_diplomacy_2", "lord_ask_enter_service_vassalage", "{=1OU1ZkaZ}{SALUTATION_BY_PLAYER}, I wish to be more than a mercenary. Is there a way I could pledge myself as a vassal?", new ConversationSentence.OnConditionDelegate(this.conversation_player_is_offering_vassalage_while_at_mercenary_service_on_condition), null, 100, null, null);
			starter.AddPlayerLine("player_is_offering_join_cancel", "lord_ask_enter_service_answer", "lord_pretalk", "{=B2z3mEue}Actually, I was going to talk about something else.", null, null, 100, null, null);
			starter.AddDialogLine("player_want_to_join_faction_as_vassal_lord_answer_2", "lord_ask_enter_service_vassalage", "lord_ask_enter_service_vassalage_player_response", "{=gKK0HlHK}You would need to talk to our leader {KING.LINK} on this matter. {?KING.GENDER}She{?}He{\\?} is {?IS_PRISONED}unfortunately held captive{?}currently{\\?} {?IS_IN_SETTLEMENT}at{?}near{\\?} {SETTLEMENT}.", new ConversationSentence.OnConditionDelegate(this.conversation_player_is_offering_vassalage_to_lord_on_condition), null, 100, null);
			starter.AddDialogLine("player_want_to_join_faction_as_vassal_lord_answer_3", "lord_ask_enter_service_vassalage_talking_with_king", "lord_pretalk", "{=wCMNQsBu}I will put in a word about you to {KING.LINK}.", null, null, 100, null);
			starter.AddDialogLine("player_want_to_join_faction_as_mercenary_king_answer", "lord_ask_enter_service_mercenary", "lord_pretalk", "{=JTt3Xu9t}Our kingdom is not at war, {PLAYER.NAME}. We have no use for mercenaries.", () => !FactionManager.GetEnemyFactions(Hero.OneToOneConversationHero.MapFaction).Any<IFaction>(), null, 100, null);
			starter.AddDialogLine("player_want_to_join_faction_as_mercenary_king_answer_2", "lord_ask_enter_service_mercenary", "lord_pretalk", "{=AzmriKR8}I have hardly heard of you, {PLAYER.NAME}. Go fight a few bandits, make a name for yourself. Then we can talk.", new ConversationSentence.OnConditionDelegate(this.conversation_mercenary_service_offer_rejected_on_condition), null, 100, null);
			starter.AddDialogLine("player_want_to_join_faction_as_mercenary_lord_answer", "lord_ask_enter_service_mercenary", "lord_pretalk", "{=dSSphiFs}We do not need hired swords currently because we are not at war. You should seek your fortune elsewhere.", () => FactionHelper.GetTotalEnemyKingdomPower((Kingdom)Hero.OneToOneConversationHero.MapFaction) < 1f, null, 100, null);
			starter.AddDialogLine("player_want_to_join_faction_as_mercenary_lord_answer_2", "lord_ask_enter_service_mercenary", "lord_pretalk", "{=9d3tffnL}We do not need hired swords to win this war. You should seek your fortune elsewhere.", () => FactionHelper.GetPowerRatioToEnemies((Kingdom)Hero.OneToOneConversationHero.MapFaction) > 3f, null, 100, null);
			starter.AddDialogLine("player_want_to_join_faction_as_mercenary_lord_answer_3", "lord_ask_enter_service_mercenary", "lord_ask_enter_service_mercenary_player_answer", "{=!}{MERCENARY_HIRING_PITCH}", new ConversationSentence.OnConditionDelegate(this.conversation_mercenary_hiring_pitch_on_condition), null, 100, null);
			starter.AddPlayerLine("mercenary_player_accepts", "lord_ask_enter_service_mercenary_player_answer", "lord_ask_enter_service_mercenary_player_accepted", "{=s5pftw9C}All right. I accept", null, null, 100, null, null);
			starter.AddPlayerLine("mercenary_player_rejects", "lord_ask_enter_service_mercenary_player_answer", "lord_ask_enter_service_mercenary_player_rejected", "{=H715sxmf}That is lower than what I had in mind. Let me think about it.", null, null, 100, null, null);
			starter.AddDialogLine("mercenary_player_accepts_lord_answer", "lord_ask_enter_service_mercenary_player_accepted", "player_joined_as_mercenary", "{=yUYNlQhU}Good, I'll have my men write up a simple contract. On behalf of the {KINGDOM_FORMAL_NAME}, I welcome you. May you put your sword to good use against our enemies.", delegate
			{
				if (Hero.OneToOneConversationHero.MapFaction.IsKingdomFaction)
				{
					MBTextManager.SetTextVariable("KINGDOM_FORMAL_NAME", FactionHelper.GetFormalNameForFactionCulture(Hero.OneToOneConversationHero.Clan.Kingdom.Culture), false);
					return true;
				}
				return false;
			}, new ConversationSentence.OnConsequenceDelegate(this.conversation_mercenary_player_accepts_lord_answer_on_consequence), 100, null);
			starter.AddPlayerLine("player_joined_2", "player_joined_as_mercenary", "lord_pretalk", "{=m0ybQ1Gz}You can count on me. As of now, your enemies are my enemies and your honor is my honor.", null, null, 100, null, null);
			starter.AddPlayerLine("player_joined_3", "player_joined_as_mercenary", "lord_pretalk", "{=O3BqrO85}So long as the denars keep flowing, so will the blood of your enemies.", null, null, 100, null, null);
			starter.AddDialogLine("mercenary_player_rejects_lord_answer", "lord_ask_enter_service_mercenary_player_rejected", "lord_pretalk", "{=wxK1bTZm}Do think about it. But make sure you do not end up on the losing side.", null, null, 100, null);
			starter.AddDialogLine("hero_no_available_task", "quest_offer", "hero_main_options", "{=NOH4FilQ}I have no task to offer you right now.", null, null, 1, null);
			starter.AddDialogLine("prisoner_hero_leave_answer", "prisoner_hero_leave", "close_window", "{=CWfEUmiF}Right. I won't be going anywhere.", null, new ConversationSentence.OnConsequenceDelegate(this.conversation_lord_leave_on_consequence), 100, null);
			starter.AddDialogLineWithVariation("hero_leave", "hero_leave", "close_window", new ConversationSentence.OnConditionDelegate(this.conversation_lord_leave_on_condition), new ConversationSentence.OnConsequenceDelegate(this.conversation_lord_leave_on_consequence), 100, "", "", "", "", null).Variation(new object[] { "{=BsXNJHCu}Until next time, then.", "DefaultTag", 1 }).Variation(new object[] { "{=a2qPdYbu}Of course, {PLAYER.NAME}. Farewell.", "FriendlyRelationshipTag", 2 })
				.Variation(new object[] { "{=GcCfYKDl}Farewell, then.", "HighRegisterTag", 2 })
				.Variation(new object[] { "{=FPvgVbtN}Yes, yes. Goodbye.", "PersonaCurtTag", 1, "UngratefulTag", 1 })
				.Variation(new object[] { "{=HCF4xGfk}Right... Well, you be off, then. Keep safe.", "FriendlyRelationshipTag", 2, "HighRegisterTag", -1 })
				.Variation(new object[] { "{=nePa28Sb}Very good, then. Be on your way.", "ImpoliteTag", 2, "HighRegisterTag", 1 })
				.Variation(new object[] { "{=qaFyoSF7}Good journeys to you, {PLAYER.NAME}.", "FriendlyRelationshipTag", 2, "GenerosityTag", 1 })
				.Variation(new object[] { "{=sbugaHdU}Right. Cheers, then.", "NoConflictTag", 1, "DrinkingInTavernTag", 1, "PersonaCurtTag", -1 })
				.Variation(new object[] { "{=gwQnjPhM}Yeah... Later.", "DrinkingInTavernTag", 1, "FriendlyRelationshipTag", -1, "PersonaCurtTag", 1, "UncharitableTag", 1 })
				.Variation(new object[] { "{=zXpNnn60}We will meet again. ", "PlayerIsEnemyTag", 6 })
				.Variation(new object[] { "{=zJVb2aCe}Farewell, {PLAYER.NAME}. I regret that we part on these terms.", "PlayerIsEnemyTag", 6, "FriendlyRelationshipTag", 1 })
				.Variation(new object[] { "{=VbUnP1M5}Very well. For now, go in peace.", "PlayerIsEnemyTag", 6, "MercyTag", 1 })
				.Variation(new object[] { "{=thxkhXmR}Very well. When next we meet, I can't promise things will go so peacefully.", "PlayerIsEnemyTag", 6, "MercyTag", -1 })
				.Variation(new object[] { "{=Qm6SGjkb}Farewell, my wife. Safe travels, and a swift return.", "PlayerIsFemaleTag", 1, "PlayerIsSpouseTag", 10, "HighRegisterTag", 1 })
				.Variation(new object[] { "{=BYuzbj3L}Farewell, my husband. Safe travels, and a swift return.", "PlayerIsFemaleTag", -1, "PlayerIsSpouseTag", 10, "HighRegisterTag", 1 })
				.Variation(new object[] { "{=IF6jGovm}Stay safe, my dear.", "PlayerIsFemaleTag", 1, "PlayerIsSpouseTag", 10, "HighRegisterTag", -1 })
				.Variation(new object[] { "{=QJVbCLbl}Keep safe, husband.", "PlayerIsFemaleTag", -1, "PlayerIsSpouseTag", 10, "HighRegisterTag", -1 })
				.Variation(new object[] { "{=TWSRs4gz}Farewell, my lady. I shall remain your most ardent admirer.", "PlayerIsFemaleTag", 1, "PlayerIsSpouseTag", -1, "RomanticallyInvolvedTag", 5, "HighRegisterTag", 1 })
				.Variation(new object[] { "{=lnTE6tdn}Let us speak again soon, {PLAYER.NAME}.", "PlayerIsFemaleTag", -1, "PlayerIsSpouseTag", -1, "RomanticallyInvolvedTag", 5, "HighRegisterTag", 1 })
				.Variation(new object[] { "{=8dMkj1qK}I'll be looking you up again soon, m'lady, with your permission,", "PlayerIsFemaleTag", 1, "PlayerIsSpouseTag", -1, "RomanticallyInvolvedTag", 5, "HighRegisterTag", -1 })
				.Variation(new object[] { "{=hBFHcNaz}Come see me again soon, {PLAYER.NAME}, you hear?", "PlayerIsFemaleTag", -1, "PlayerIsSpouseTag", -1, "RomanticallyInvolvedTag", 5, "HighRegisterTag", -1 })
				.Variation(new object[] { "{=3ZL6El0S}Very well, my lady. I hope we shall meet again soon.", "PlayerIsFemaleTag", 1, "NoConflictTag", 1, "AttractedToPlayerTag", 3 })
				.Variation(new object[] { "{=J6UU9cfU}Very well, my dear. I hope we can soon sort out the terms of our marriage.", "NoConflictTag", 1, "PlayerIsFemaleTag", 1, "EngagedToPlayerTag", 7 })
				.Variation(new object[]
				{
					"{=EK3dEjBv}I must say, my lady, I am indeed delighted to have met you. I hope to see you again soon.", "AttractedToPlayerTag", 5, "NoConflictTag", 1, "PlayerIsFemaleTag", 1, "HighRegisterTag", 1, "FirstMeetingTag",
					5, "PersonaCurtTag", -1
				})
				.Variation(new object[]
				{
					"{=uolvICE4}Let's talk again soon, m'lady.", "AttractedToPlayerTag", 5, "NoConflictTag", 1, "PlayerIsFemaleTag", 1, "LowRegisterTag", 1, "FirstMeetingTag",
					5, "PersonaCurtTag", -1
				})
				.Variation(new object[]
				{
					"{=VvxGEJoz}I am enchanted to have met you, my lady.", "AttractedToPlayerTag", 5, "NoConflictTag", 1, "PlayerIsFemaleTag", 1, "TribalRegisterTag", 1, "FirstMeetingTag",
					5, "PersonaCurtTag", -1
				})
				.Variation(new object[]
				{
					"{=9804jwU1}It was a pleasure, sir. I hope to see you again some time soon.", "AttractedToPlayerTag", 5, "PlayerIsEnemyTag", -1, "HostileRelationshipTag", -1, "PlayerIsFemaleTag", -1, "FirstMeetingTag",
					5, "HighRegisterTag", 1, "PersonaCurtTag", -1
				})
				.Variation(new object[]
				{
					"{=OB37sx91}Don't be a stranger, eh? Come see me again some time.", "AttractedToPlayerTag", 5, "PlayerIsEnemyTag", -1, "HostileRelationshipTag", -1, "PlayerIsFemaleTag", -1, "FirstMeetingTag",
					4, "LowRegisterTag", 1, "PersonaCurtTag", -1, "UnderCommandTag", -1
				})
				.Variation(new object[]
				{
					"{=kJDDW6Dk}It was a pleasure to meet you. Safe travels.", "AttractedToPlayerTag", 5, "PlayerIsEnemyTag", -1, "HostileRelationshipTag", -1, "PlayerIsFemaleTag", -1, "FirstMeetingTag",
					5, "TribalRegisterTag", 1, "PersonaCurtTag", -1
				})
				.Variation(new object[]
				{
					"{=iIlnWsBE}Keep safe, {PLAYER.NAME}. I hope to see you again soon.", "AttractedToPlayerTag", 5, "NoConflictTag", 1, "PlayerIsFemaleTag", -1, "FirstMeetingTag", 5, "PersonaCurtTag",
					-1
				})
				.Variation(new object[]
				{
					"{=sy5Hbp04}Safe travels to you, my lady. I hope to see you again soon.", "AttractedToPlayerTag", 5, "NoConflictTag", 1, "PlayerIsFemaleTag", 1, "FirstMeetingTag", 5, "PersonaCurtTag",
					-1, "LowRegisterTag", -1
				})
				.Variation(new object[]
				{
					"{=7b6yzkig}Always a please, m'lady. Keep safe.", "AttractedToPlayerTag", 5, "NoConflictTag", 1, "PlayerIsFemaleTag", 1, "FirstMeetingTag", 5, "PersonaCurtTag",
					-1, "LowRegisterTag", 1
				})
				.Variation(new object[] { "{=eah0gBXu}Good to meet you.", "FirstMeetingTag", 3 })
				.Variation(new object[] { "{=oiMs17Oh}Very well... You know where to find me.", "GangLeaderNotableTypeTag", 3, "FirstMeetingTag", 1 })
				.Variation(new object[] { "{=hfbyHRu7}Farewell. If no one slits your throat out there, perhaps we shall meet again.", "FirstMeetingTag", 1, "PlayerIsNobleTag", -1, "CruelTag", 1, "PersonaIronicTag", 1 })
				.Variation(new object[] { "{=km6aX5ev}Yeah... Go on, then.", "FirstMeetingTag", 1, "ImpoliteTag", 2, "LowRegisterTag", 1 })
				.Variation(new object[] { "{=wbxUMLZ8}Be on your way, then.", "FirstMeetingTag", 1, "ImpoliteTag", 3 })
				.Variation(new object[] { "{=eWGJmRMR}Yeah, sure. Later, then.", "FirstMeetingTag", 1, "PersonaCurtTag", 1, "LowRegisterTag", 1 })
				.Variation(new object[] { "{=BeD9fahY}A pleasure to have met you.", "FirstMeetingTag", 1, "PersonaEarnestTag", 1, "ImpoliteTag", -1, "HighRegisterTag", 1 })
				.Variation(new object[] { "{=V5Kygicn}It was good to meet you. Come see me if you need anything.", "FirstMeetingTag", 1, "CalculatingTag", 3 })
				.Variation(new object[] { "{=EBI3j6O8}Sure. You know where to find me.", "FirstMeetingTag", 2, "UnderCommandTag", -1, "DrinkingInTavernTag", 1, "PersonaCurtTag", 1 })
				.Variation(new object[] { "{=3INC3Thi}If you need me, you know where to find me.", "FirstMeetingTag", 2, "UnderCommandTag", -1, "DrinkingInTavernTag", 2 })
				.Variation(new object[]
				{
					"{=5ANh9qVW}Farewell for now. Walk the path of the righteous, and the Heavens will protect you.", "FirstMeetingTag", 1, "EmpireTag", 1, "CruelTag", -1, "HonorTag", 1, "PersonaCurtTag",
					-1, "PersonaIronicTag", -1
				})
				.Variation(new object[] { "{=o3rj0kbj}Yes, your highness.", "UnderCommandTag", 5, "PlayerIsLiegeTag", 5 })
				.Variation(new object[] { "{=MTxuTZDA}I'll be here, your {?PLAYER.GENDER}ladyship{?}lordship{\\?}.", "UnderCommandTag", 5, "AnyNotableTypeTag", 1, "WandererTag", -1 })
				.Variation(new object[] { "{=QffdjUxf}Very well. I'll get my gear and join you outside.", "UnderCommandTag", 5, "WandererTag", 2, "FirstMeetingTag", 1 })
				.Variation(new object[] { "{=F7yFADCx}Sure thing, boss.[ib:demure]", "UnderCommandTag", 6, "LowRegisterTag", 1, "PersonaIronicTag", 1 })
				.Variation(new object[] { "{=MPkoFNpr}Yes, captain.[ib:demure]", "UnderCommandTag", 7 })
				.Variation(new object[] { "{=5akTbqNs}I'll meet you outside then, captain.", "UnderCommandTag", 5, "PlayerIsLiegeTag", -1, "DrinkingInTavernTag", 1 })
				.Variation(new object[] { "{=Kwy6QDyf}Yes, brother.[ib:demure]", "PlayerIsBrotherTag", 10, "PlayerIsFemaleTag", -1 })
				.Variation(new object[] { "{=R8JAeEmO}Yes, sister.[ib:demure]", "PlayerIsSisterTag", 10, "PlayerIsFemaleTag", 1 });
			starter.AddDialogLine("lord_diagnostics_agree", "lord_diagnostics", "lord_diagnostic_options", "{=!}(TEST CHEAT) What do you want to know?", new ConversationSentence.OnConditionDelegate(this.debug_mode_enabled_condition), null, 100, null);
			starter.AddPlayerLine("lord_diagnostics_option_1", "lord_diagnostic_options", "lord_diagnostic_other_lords", "{=!}(CHEAT) How do you feel about other lords?", null, null, 100, null, null);
			starter.AddDialogLine("lord_diagnostics_option_1_di", "lord_diagnostic_other_lords", "lord_diagnostics", "{=!}{OTHER_LORDS}", new ConversationSentence.OnConditionDelegate(this.conversation_cheat_other_lords_on_condition), null, 100, null);
			starter.AddPlayerLine("lord_diagnostics_option_1_2", "lord_diagnostic_options", "lord_diagnostic_enmities", "{=!}(CHEAT) Tell me about the enmities in your faction..", null, null, 100, null, null);
			starter.AddDialogLine("lord_diagnostics_option_1_2_di", "lord_diagnostic_enmities", "lord_diagnostics", "{=!}{ENMITY_INFO}", new ConversationSentence.OnConditionDelegate(this.conversation_cheat_faction_enmities_on_condition), null, 100, null);
			starter.AddPlayerLine("lord_diagnostics_option_1_3", "lord_diagnostic_options", "lord_diagnostic_reputation", "{=!}(CHEAT) Tell me about your personal reputation", null, null, 100, null, null);
			starter.AddDialogLine("lord_diagnostics_option_1_3_di", "lord_diagnostic_reputation", "lord_diagnostics", "{=!}{REPUTATION}", new ConversationSentence.OnConditionDelegate(this.conversation_cheat_reputation_on_condition), null, 100, null);
			starter.AddPlayerLine("lord_diagnostics_option_1_4", "lord_diagnostic_options", "lord_pretalk", "{=D33fIGQe}Never mind.", null, null, 100, null, null);
			starter.AddDialogLine("hero_romance_reaction", "hero_romance_reaction", "lord_pretalk", "{=!}You should never see this text.", null, null, 100, null);
			starter.AddDialogLine("hero_active_mission_lord_ask", "lord_active_mission_1", "lord_active_mission_2", "{=zG5jo0bJ}Yes, have you made any progress on it?", new ConversationSentence.OnConditionDelegate(this.conversation_mission_in_progress_on_condition), null, 100, null);
			starter.AddPlayerLine("hero_active_mission_response_cont", "lord_active_mission_2", "lord_active_mission_3", "{=j7WWf8aM}I am still working on it.", new ConversationSentence.OnConditionDelegate(this.conversation_lord_active_mission_response_cont_on_condition), null, 100, null, null);
			starter.AddPlayerLine("hero_active_mission_response_failed", "lord_active_mission_2", "lord_mission_failed", "{=JWjBnGST}I am afraid I won't be able to do this quest.", new ConversationSentence.OnConditionDelegate(this.conversation_lord_active_mission_response_failed_on_condition), new ConversationSentence.OnConsequenceDelegate(this.conversation_lord_active_mission_response_failed_on_consequence), 100, null, null);
			starter.AddDialogLine("hero_active_mission_failed", "lord_mission_failed", "lord_pretalk", "{=AypnUIky}Well, I am disappointed, but I am sure that you will have many chances to redeem yourself.[ib:closed][if:convo_bored]", null, null, 100, null);
			starter.AddDialogLine("lord_special_request_specify", "lord_special_request", "lord_special_request_player", "{=M06TjiMd}What's that?", null, null, 100, null);
			starter.AddPlayerLine("lord_special_request_nevermind", "lord_special_request_player", "lord_pretalk", "{=D33fIGQe}Never mind.", null, null, 100, null, null);
			starter.AddDialogLine("hero_flirt_reaction", "hero_flirt_reaction", "lord_pretalk", "{=MQJMLdV7}{ROMANTIC_REACTION} {ROMANTIC_DEFICIENCY}", null, null, 100, null);
			starter.AddDialogLine("lord_talk_ask_something", "lord_talk_ask_something", "lord_talk_ask_something_2", "{=CX6JHwbB}Aye? What is it?", null, null, 100, null);
			this.AddAnimationTestConversations(starter);
			starter.AddDialogLine("lord_talk_ask_something_again", "lord_talk_ask_something_again", "lord_talk_ask_something_2", "{=DQBaaC0e}Is there anything else?", null, null, 100, null);
			starter.AddPlayerLine("lord_talk_ask_something_2", "lord_talk_ask_something_2", "lord_animation_test_smile", "{=!}Be glad to see me.", new ConversationSentence.OnConditionDelegate(this.debug_mode_enabled_condition), null, 100, null, null);
			starter.AddPlayerLine("lord_talk_ask_something_2_2", "lord_talk_ask_something_2", "lord_tell_objective_1", "{=!}What are you and your men doing? (v1)", new ConversationSentence.OnConditionDelegate(this.debug_mode_enabled_condition), null, 100, null, null);
			starter.AddPlayerLine("lord_talk_ask_something_2_3", "lord_talk_ask_something_2", "wanderer_introduction_a", "{=Ymgbv2gV}What's your story again?", new ConversationSentence.OnConditionDelegate(this.conversation_wanderer_on_condition), null, 100, null, null);
			starter.AddPlayerLine("lord_talk_ask_something_2_4", "lord_talk_ask_something_2", "lord_pretalk", "{=D33fIGQe}Never mind.", null, null, 100, null, null);
			starter.AddDialogLine("lord_answers_the_war_question", "lord_animation_test_smile", "lord_talk_ask_something_2", "{=!}Old friend! What now?[if:convo_delighted, ib:act_start_pleased_conversation]", new ConversationSentence.OnConditionDelegate(this.debug_mode_enabled_condition), new ConversationSentence.OnConsequenceDelegate(this.conversation_lord_animation_test_old_friend), 100, null);
			starter.AddDialogLineWithVariation("lord_tell_objective", "lord_tell_objective_1", "lord_talk_ask_something_again", null, null, 100, "", "", "", "", null).Variation(new object[] { "{=!}Nothing, really.", "DefaultTag", 0 }).Variation(new object[] { "{=!}Why are you asking?", "PersonaCurtTag", 1 })
				.Variation(new object[] { "{=!}Why, I'd be delighted to tell you!", "PersonaEarnestTag", 1 })
				.Variation(new object[] { "{=!}I shall pose you a riddle.", "PersonaIronicTag", 1 })
				.Variation(new object[] { "{=!}Mm... I'm shy.", "PersonaSoftspokenTag", 1 })
				.Variation(new object[] { "{=!}You should be telling me.", "UnderCommandTag", 1, "UnderCommandTag", 1, "UnderCommandTag", 1 })
				.Variation(new object[] { "{=!}Killing our enemies.", "CruelTag", 1 })
				.Variation(new object[] { "{=!}Whatever you ask, my spouse.", "PlayerIsSpouseTag", 5 })
				.Variation(new object[] { "{=!}Upholding our word.", "HonorTag", 1, "HonorTag", 1 });
			starter.AddDialogLine("hero_doesnt_have_quest", "player_requests_quest", "lord_pretalk", "{=wdQ54wn2}There's nothing I need right now.", null, null, 100, null);
		}

		private bool conversation_set_first_on_condition()
		{
			Campaign.Current.ConversationManager.CurrentConversationIsFirst = false;
			return false;
		}

		private bool conversation_lord_special_request_on_condition()
		{
			return Hero.OneToOneConversationHero == null || !Hero.OneToOneConversationHero.IsPlayerCompanion;
		}

		private bool conversation_mercenary_hiring_pitch_on_condition()
		{
			TextObject textObject = new TextObject("{=xWFIMImm}You want some mercenary work, eh? Well, we are glad to take fighters, whether they seek glory or gold. If you fight for us, you will receive {MERCENARY_AWARD}{GOLD_ICON} whenever you defeat a party of enemies, or for any other significant deed.", null);
			if (Campaign.Current.ConversationManager.IsTagApplicable("ChivalrousTag", Hero.OneToOneConversationHero.CharacterObject))
			{
				textObject = new TextObject("{=To8Zkjxu}Yes, well, we do hire mercenaries... And some of them, I'll admit, are men of honor. Your reward will be {MERCENARY_AWARD}{GOLD_ICON} for every group of enemies you vanquish, or for an equivalent deed.", null);
			}
			else if (Hero.OneToOneConversationHero.CharacterObject.GetPersona() == DefaultTraits.PersonaCurt)
			{
				textObject = new TextObject("{=BixmcFY3}Yes, we're hiring mercenaries. We pay you to fight, though, not just ride around the countryside. You'll get {MERCENARY_AWARD}{GOLD_ICON} whenever you take down an enemy party, or do a similar service.", null);
			}
			textObject.SetTextVariable("MERCENARY_AWARD", this.GetMercenaryAwardFactor());
			MBTextManager.SetTextVariable("MERCENARY_HIRING_PITCH", textObject, false);
			return true;
		}

		private void conversation_lord_animation_test_old_friend()
		{
		}

		private void AddLordLiberateConversations(CampaignGameStarter starter)
		{
			starter.AddDialogLine("talk_common_to_lord_free", "start", "defeated_lord_answer", "{=!}{SURRENDER_OFFER}", new ConversationSentence.OnConditionDelegate(this.conversation_capture_defeated_lord_on_condition), null, 100, null);
			starter.AddDialogLine("liberate_hero_1", "start", "liberate_player_choice", "{=XbnhxZbo}{PLAYER.NAME}.. Is that you? Am I free?", new ConversationSentence.OnConditionDelegate(this.conversation_liberate_known_hero_on_condition), null, 100, null);
			starter.AddDialogLine("liberate_hero_2", "start", "liberate_player_choice", "{=afRebqPd}What's happening? Am I free?", new ConversationSentence.OnConditionDelegate(this.conversation_liberate_unmet_hero_on_condition), null, 100, null);
			starter.AddPlayerLine("liberate_hero_3", "liberate_player_choice", "liberate_comment", "{=2xLwjXYm}Rest easy. Your captivity has ended.", null, null, 100, null, null);
			starter.AddPlayerLine("liberate_hero_4", "liberate_player_choice", "reprisoner_comment", "{=FNHbbbbn}{REPRISONER_DECISION}", new ConversationSentence.OnConditionDelegate(this.conversation_reprisoner_hero_decision_on_condition), null, 100, null, null);
			starter.AddDialogLineWithVariation("liberate_hero_5", "liberate_comment", "liberate_final_words", null, null, 100, "", "", "", "", null).Variation(new object[] { "{=bWVmR6WS}I thank you.[if:convo_focused_happy]", "DefaultTag", 1 }).Variation(new object[] { "{=ji5gXHNu}You're a good friend to have. A very good friend.[if:convo_focused_happy]", "FriendlyRelationshipTag", 1 })
				.Variation(new object[] { "{=F6bnwurv}I am deeply obligated to you. I shall endeavor to do what I can, within the bounds of honor, to return the favor.[if:convo_focused_happy]", "HighRegisterTag", 1, "HonorTag", 1 })
				.Variation(new object[] { "{=jrPxbfnb}You did me a good turn. I hope I can repay you.[if:convo_focused_happy]", "GenerosityTag", 1 });
			starter.AddDialogLineWithVariation("liberate_hero_6", "reprisoner_comment", "reprisoner_final_words", null, null, 100, "", "", "", "", null).Variation(new object[] { "{=xjrRbZ3U}Do what you must.", "DefaultTag", 1 }).Variation(new object[] { "{=flctCQUT}How gallant. I hope some day I may be in a position to return the favor.", "UncharitableTag", 1, "PersonaIronicTag", 1 })
				.Variation(new object[] { "{=5j6mIuu1}What? I don't understand.", "PlayerIsAlliedTag", 3 });
			starter.AddPlayerLine("liberate_hero_7", "liberate_final_words", "close_window", "{=ybagz7xY}No thanks are necessary.", null, new ConversationSentence.OnConsequenceDelegate(this.conversation_player_liberates_prisoner_on_consequence), 100, null, null);
			starter.AddPlayerLine("liberate_hero_8", "liberate_final_words", "close_window", "{=l4yDEvdV}You owe me one.", null, new ConversationSentence.OnConsequenceDelegate(this.conversation_player_liberates_prisoner_on_consequence), 100, null, null);
			starter.AddPlayerLine("liberate_hero_9", "reprisoner_final_words", "close_window", "{=7a3cGmFg}I will endeavor to treat you as well as I can.", null, new ConversationSentence.OnConsequenceDelegate(this.conversation_player_fails_to_release_prisoner_on_consequence), 100, null, null);
			starter.AddPlayerLine("liberate_hero_10", "reprisoner_final_words", "close_window", "{=BbnDPpxW}Did I ask you to speak? Your kin best scrape together a ransom before my patience runs out.", null, new ConversationSentence.OnConsequenceDelegate(this.conversation_player_fails_to_release_prisoner_on_consequence), 100, null, null);
			starter.AddPlayerLine("liberate_hero_11", "reprisoner_final_words", "liberate_comment", "{=kGg8xv4c}I changed my mind. You can go free.", null, null, 100, null, null);
			starter.AddPlayerLine("talk_common_to_lord_liberate_player_ally_enemy_no", "liberate_response_enemy", "player_refuses_to_liberate_enemy", "{=yAOQnMVw}You're of the {FACTION}, right? That means you're my prisoner.", null, new ConversationSentence.OnConsequenceDelegate(this.conversation_player_fails_to_release_prisoner_on_consequence), 100, null, null);
			starter.AddPlayerLine("talk_common_to_lord_liberate_player_yes_1", "liberate_response_enemy", "player_liberates_enemy", "{=lEK7gx3G}I won't take you prisoner today.", null, new ConversationSentence.OnConsequenceDelegate(this.conversation_player_liberates_prisoner_on_consequence), 100, null, null);
			starter.AddPlayerLine("talk_common_to_lord_liberate_player_yes_1_2", "liberate_response_enemy", "player_liberates_enemy", "{=BurJvC9s}Just get out of my sight.", null, new ConversationSentence.OnConsequenceDelegate(this.conversation_player_liberates_prisoner_on_consequence), 100, null, null);
			starter.AddDialogLine("talk_common_to_lord_liberate_political_enemy_friend_yes_answer", "player_liberates_enemy", "close_window", "{=0WgYomR5}Thank you. I cannot guarantee I'd do the same for you, but I thank you.", null, null, 100, null);
			starter.AddDialogLine("talk_common_to_lord_liberate_political_enemy_friend_no_answer", "player_refuses_to_liberate_enemy", "close_window", "{=QYoigG3b}I understand. I'd take you prisoner too, if duty demanded. And I will soon, I hope.", null, null, 100, null);
		}

		private bool conversation_start_parley_on_condition()
		{
			if (Hero.OneToOneConversationHero != null && FactionManager.IsAtWarAgainstFaction(Hero.OneToOneConversationHero.MapFaction, Hero.MainHero.MapFaction) && PlayerEncounter.Current != null && PlayerEncounter.InsideSettlement && Campaign.Current.IsMainPartyWaiting && PlayerEncounter.EncounterSettlement != null && PlayerEncounter.EncounterSettlement.IsUnderSiege && Hero.OneToOneConversationHero.PartyBelongedTo != null)
			{
				return PlayerEncounter.EncounterSettlement.SiegeEvent.BesiegerCamp.GetInvolvedPartiesForEventType(MapEvent.BattleTypes.Siege).Any((PartyBase party) => party.MobileParty == Hero.OneToOneConversationHero.PartyBelongedTo);
			}
			return false;
		}

		private void lord_considers_army_on_consequence()
		{
			MobileParty.MainParty.Ai.SetMoveEscortParty(Hero.OneToOneConversationHero.PartyBelongedTo);
			MobileParty.MainParty.Army = Hero.OneToOneConversationHero.PartyBelongedTo.Army;
			MobileParty.MainParty.Army.AddPartyToMergedParties(MobileParty.MainParty);
		}

		private void lord_considers_joining_player_army_on_consequence()
		{
			Hero.OneToOneConversationHero.PartyBelongedTo.Army = MobileParty.MainParty.Army;
			SetPartyAiAction.GetActionForEscortingParty(Hero.OneToOneConversationHero.PartyBelongedTo, MobileParty.MainParty);
		}

		private bool conversation_lord_recruit_reject_enough_politics_on_condition()
		{
			return false;
		}

		private bool conversation_lord_refuses_to_discuss_not_fighting_on_condition()
		{
			return !HeroHelper.LordWillConspireWithLord(Hero.OneToOneConversationHero, Hero.MainHero, false);
		}

		private bool conversation_lord_refuses_siege_lift_on_condition()
		{
			float totalStrengthWithFollowers = Hero.OneToOneConversationHero.PartyBelongedTo.GetTotalStrengthWithFollowers(true);
			float num = 0f;
			if (this.PlayerIsBesieged())
			{
				using (List<MobileParty>.Enumerator enumerator = Settlement.CurrentSettlement.Parties.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						MobileParty mobileParty = enumerator.Current;
						num += mobileParty.GetTotalStrengthWithFollowers(true);
					}
					goto IL_6D;
				}
			}
			num = Hero.MainHero.PartyBelongedTo.GetTotalStrengthWithFollowers(true);
			IL_6D:
			if (totalStrengthWithFollowers > num * 2f)
			{
				MBTextManager.SetTextVariable("REFUSE_BARTER_LINE", "{=QuzaaBD8}Why should I negotiate for your gold, when I have enough men to simply take it?", false);
				return true;
			}
			return false;
		}

		private bool conversation_lord_agrees_to_discussion_on_condition()
		{
			MBTextManager.SetTextVariable("STR_INTRIGUE_AGREEMENT", Campaign.Current.ConversationManager.FindMatchingTextOrNull("str_lord_intrigue_accept", Hero.OneToOneConversationHero.CharacterObject), false);
			return true;
		}

		private bool conversation_can_lord_barter()
		{
			return BarterManager.Instance.CanPlayerBarterWithHero(Hero.OneToOneConversationHero);
		}

		private void conversation_player_is_leaving_surrender_on_consequence()
		{
			PlayerEncounter.PlayerSurrender = true;
			PlayerEncounter.Update();
		}

		private bool conversation_uses_pay_for_passage_lines()
		{
			return !Hero.OneToOneConversationHero.MapFaction.IsMinorFaction;
		}

		private bool conversation_pay_minor_faction_for_passage()
		{
			return Hero.OneToOneConversationHero.MapFaction.IsMinorFaction;
		}

		private bool conversation_can_pay_minor_faction_for_payoff_set_text_on_condition()
		{
			MBTextManager.SetTextVariable("GOLD_AMOUNT", (int)((float)Hero.MainHero.Gold * 0.1f + 200f));
			MBTextManager.SetTextVariable("AGREE_TO_TAKE_PAYMENT", "{=VlU9nnLY}Good. Give us our due, and you can pass.", false);
			MBTextManager.SetTextVariable("ACCEPT_GOLD_STATEMENT", "{=GKX59dO2}Good. You may pass.", false);
			if (Hero.OneToOneConversationHero != null && Hero.OneToOneConversationHero.IsMinorFactionHero)
			{
				if (Hero.OneToOneConversationHero.Clan.IsNomad)
				{
					MBTextManager.SetTextVariable("AGREE_TO_TAKE_PAYMENT", "{=hd2Bjz7b}Good. Pay us our dues, according to the laws of our people, and you shall have safe passage.", false);
				}
				else if (Hero.OneToOneConversationHero.Clan.IsSect)
				{
					MBTextManager.SetTextVariable("AGREE_TO_TAKE_PAYMENT", "{=6MtiK8uc}Well now... A small donation to our cause in silver would earn you passage.", false);
				}
				else if (Hero.OneToOneConversationHero.Clan.IsMafia)
				{
					MBTextManager.SetTextVariable("AGREE_TO_TAKE_PAYMENT", "{=nIhIU9t8}Good. Let's see if we can work out fair compensation for us protecting the roads.", false);
				}
				MBTextManager.SetTextVariable("FACTION_NAME", Hero.OneToOneConversationHero.Clan.Name, false);
				MBTextManager.SetTextVariable("ACCEPT_GOLD_STATEMENT", "{=YT4OvaAG}Very well. You may consider yourself under the protection of the {FACTION_NAME}. You have until this time tomorrow to complete your journey, or we may ask for another payment from you.", false);
			}
			return true;
		}

		private bool conversation_escape_lord_by_gold_can_be_paid_on_condition()
		{
			this._goldAmount = (int)((float)Hero.MainHero.Gold * 0.12f + 250f);
			return this._goldAmount <= Hero.MainHero.Gold;
		}

		private void conversation_escape_lord_by_gold_can_be_paid_on_consequence()
		{
			PlayerEncounter.LeaveEncounter = true;
			GiveGoldAction.ApplyBetweenCharacters(Hero.MainHero, null, this._goldAmount, true);
			List<MobileParty> list = new List<MobileParty> { MobileParty.MainParty };
			List<MobileParty> list2 = new List<MobileParty> { MobileParty.ConversationParty };
			if (PlayerEncounter.Current != null)
			{
				PlayerEncounter.Current.FindAllNpcPartiesWhoWillJoinEvent(ref list, ref list2);
			}
			float num = 0f;
			foreach (MobileParty mobileParty in list2)
			{
				num += mobileParty.Party.TotalStrength;
			}
			int num2 = 0;
			int num3 = this._goldAmount;
			foreach (MobileParty mobileParty2 in list2)
			{
				num2++;
				if (mobileParty2.LeaderHero != null)
				{
					if (num2 == list2.Count)
					{
						GiveGoldAction.ApplyBetweenCharacters(null, mobileParty2.LeaderHero, num3, true);
					}
					else
					{
						int num4 = (int)((float)this._goldAmount * (mobileParty2.Party.TotalStrength / num));
						num3 -= num4;
						GiveGoldAction.ApplyBetweenCharacters(null, mobileParty2.LeaderHero, num4, true);
					}
				}
				else if (num2 == list2.Count)
				{
					mobileParty2.PartyTradeGold += num3;
				}
				else
				{
					int num5 = (int)((float)this._goldAmount * (mobileParty2.Party.TotalStrength / num));
					num3 -= num5;
					mobileParty2.PartyTradeGold += num5;
				}
			}
			foreach (MobileParty mobileParty3 in list2)
			{
				mobileParty3.Ai.SetDoNotAttackMainParty(24);
				mobileParty3.Ai.SetMoveModeHold();
			}
		}

		private bool conversation_minor_faction_makes_surrender_demand_on_condition()
		{
			if (HeroHelper.WillLordAttack() && Hero.OneToOneConversationHero.IsMinorFactionHero && (Hero.OneToOneConversationHero.Clan.IsNomad || Hero.OneToOneConversationHero.Clan.IsMafia || Hero.OneToOneConversationHero.Clan.IsSect))
			{
				MBTextManager.SetTextVariable("MINOR_FACTION_SURRENDER_DEMAND_STRING", "{=3MJSuB8G}So... In our lands, it's customary to pay us a tax for our protection. Unless you want a fight.", false);
				if (Hero.OneToOneConversationHero.Clan.IsSect)
				{
					MBTextManager.SetTextVariable("MINOR_FACTION_SURRENDER_DEMAND_STRING", "{=kqDvS5Vz}Now, it grieves us to do this, but we have mouths to feed and we're going to have ask you to pay for passage through our lands. If you refuse, we'll have no choice but to take your money by force.", false);
				}
				return true;
			}
			return false;
		}

		private bool conversation_lord_makes_surrender_demand_on_condition()
		{
			if (HeroHelper.WillLordAttack())
			{
				MBTextManager.SetTextVariable("SURRENDER_DEMAND_STRING", Campaign.Current.ConversationManager.FindMatchingTextOrNull("str_surrender_demand_hero", CharacterObject.OneToOneConversationCharacter), false);
				return true;
			}
			return false;
		}

		private bool conversationUseMeetingDialogs()
		{
			if (Hero.OneToOneConversationHero != null)
			{
				StringHelpers.SetCharacterProperties("CONVERSATION_NPC", Hero.OneToOneConversationHero.CharacterObject, null, false);
			}
			if (Campaign.Current.CurrentConversationContext == ConversationContext.FreedHero || Campaign.Current.CurrentConversationContext == ConversationContext.CapturedLord)
			{
				return false;
			}
			if (Hero.OneToOneConversationHero == null)
			{
				return false;
			}
			if (!this.UsesLordConversations(Hero.OneToOneConversationHero))
			{
				return false;
			}
			if (!Hero.OneToOneConversationHero.HasMet)
			{
				this.conversations_automeet_close_relatives();
			}
			if (Hero.OneToOneConversationHero.HasMet)
			{
				Campaign.Current.ConversationManager.CurrentConversationIsFirst = false;
				return false;
			}
			Campaign.Current.ConversationManager.CurrentConversationIsFirst = true;
			Hero.OneToOneConversationHero.SetHasMet();
			if (Campaign.Current.CurrentConversationContext != ConversationContext.Default && Campaign.Current.CurrentConversationContext != ConversationContext.PartyEncounter)
			{
				return false;
			}
			this.conversations_set_voiced_line();
			return true;
		}

		private void conversations_automeet_close_relatives()
		{
			if (Hero.OneToOneConversationHero.Spouse == Hero.MainHero || Hero.OneToOneConversationHero.Siblings.Contains(Hero.MainHero) || Hero.OneToOneConversationHero.Children.Contains(Hero.MainHero) || Hero.MainHero.Children.Contains(Hero.OneToOneConversationHero))
			{
				Debug.FailedAssert("player has not met with a family member", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\CampaignBehaviors\\LordConversationsCampaignBehavior.cs", "conversations_automeet_close_relatives", 2483);
				Hero.OneToOneConversationHero.SetHasMet();
			}
		}

		private void conversations_set_voiced_line()
		{
			StringHelpers.SetCharacterProperties("PLAYER", Hero.MainHero.CharacterObject, null, false);
			MBTextManager.SetTextVariable("STR_SALUTATION", Campaign.Current.ConversationManager.FindMatchingTextOrNull("str_salutation", Hero.OneToOneConversationHero.CharacterObject), false);
			TextObject textObject = Campaign.Current.ConversationManager.FindMatchingTextOrNull("str_context_line", CharacterObject.OneToOneConversationCharacter);
			MBTextManager.SetTextVariable("VOICED_LINE", textObject ?? TextObject.Empty, false);
		}

		private bool conversation_attacking_lord_set_meeting_meet_on_condition()
		{
			return this.conversation_lord_attacking_on_condition() && this.conversationUseMeetingDialogs();
		}

		private bool conversation_lord_attacking_on_condition()
		{
			if (Hero.OneToOneConversationHero == null)
			{
				return false;
			}
			if (!HeroHelper.WillLordAttack())
			{
				return false;
			}
			if (Campaign.Current.CurrentConversationContext == ConversationContext.FreedHero || Campaign.Current.CurrentConversationContext == ConversationContext.CapturedLord || Hero.OneToOneConversationHero.IsPrisoner)
			{
				return false;
			}
			this.conversations_set_voiced_line();
			return true;
		}

		private bool conversation_wanderer_on_condition()
		{
			return CharacterObject.OneToOneConversationCharacter != null && CharacterObject.OneToOneConversationCharacter.IsHero && CharacterObject.OneToOneConversationCharacter.Occupation == Occupation.Wanderer && CharacterObject.OneToOneConversationCharacter.HeroObject.HeroState != Hero.CharacterStates.Prisoner;
		}

		private bool conversation_wanderer_meet_on_condition()
		{
			return this.conversation_wanderer_on_condition() && this.conversationUseMeetingDialogs();
		}

		private bool conversation_player_let_prisoner_go_on_condition()
		{
			return Hero.OneToOneConversationHero != null && Hero.OneToOneConversationHero.IsPrisoner && Campaign.Current.CurrentConversationContext != ConversationContext.CapturedLord && Campaign.Current.CurrentConversationContext != ConversationContext.FreedHero && ((Hero.OneToOneConversationHero.PartyBelongedToAsPrisoner != null && Hero.OneToOneConversationHero.PartyBelongedToAsPrisoner.Owner.Clan == Clan.PlayerClan) || (Hero.OneToOneConversationHero.CurrentSettlement != null && Hero.OneToOneConversationHero.CurrentSettlement.OwnerClan == Clan.PlayerClan));
		}

		private void conversation_player_let_prisoner_go_on_consequence()
		{
			EndCaptivityAction.ApplyByReleasedByChoice(Hero.OneToOneConversationHero, Hero.MainHero);
			if (PlayerEncounter.Current != null)
			{
				PlayerEncounter.LeaveEncounter = true;
			}
		}

		private bool conversation_unmet_lord_main_party_on_condition()
		{
			return Hero.OneToOneConversationHero != null && Hero.OneToOneConversationHero.PartyBelongedTo == MobileParty.MainParty && !Hero.OneToOneConversationHero.HasMet && this.conversationUseMeetingDialogs();
		}

		private bool conversation_lord_meet_on_condition()
		{
			return !HeroHelper.WillLordAttack() && this.conversationUseMeetingDialogs();
		}

		private bool conversation_siege_parley_met_on_condition()
		{
			if ((this.PlayerIsBesieging() || this.PlayerIsBesieged()) && Hero.OneToOneConversationHero != null && Hero.OneToOneConversationHero.MapFaction.IsAtWarWith(Hero.MainHero.MapFaction) && !Campaign.Current.ConversationManager.CurrentConversationIsFirst && Campaign.Current.CurrentConversationContext != ConversationContext.CapturedLord && Campaign.Current.CurrentConversationContext != ConversationContext.FreedHero)
			{
				MBTextManager.SetTextVariable("STR_PARLEY_COMMENT", Campaign.Current.ConversationManager.FindMatchingTextOrNull("str_parley_comment", CharacterObject.OneToOneConversationCharacter), false);
				return true;
			}
			return false;
		}

		private bool conversation_unmet_rebels_on_condition()
		{
			if (Hero.OneToOneConversationHero != null && Hero.OneToOneConversationHero.Clan.IsRebelClan && !Hero.OneToOneConversationHero.HasMet)
			{
				MBTextManager.SetTextVariable("SETTLEMENT_NAME", Hero.OneToOneConversationHero.HomeSettlement.Name, false);
				return true;
			}
			return false;
		}

		private bool conversation_siege_parley_unmet_on_condition()
		{
			if ((this.PlayerIsBesieging() || this.PlayerIsBesieged()) && Hero.OneToOneConversationHero != null && Hero.OneToOneConversationHero.MapFaction.IsAtWarWith(Hero.MainHero.MapFaction) && Campaign.Current.ConversationManager.CurrentConversationIsFirst && Campaign.Current.CurrentConversationContext != ConversationContext.CapturedLord && Campaign.Current.CurrentConversationContext != ConversationContext.FreedHero)
			{
				MBTextManager.SetTextVariable("STR_PARLEY_COMMENT", Campaign.Current.ConversationManager.FindMatchingTextOrNull("str_parley_comment", CharacterObject.OneToOneConversationCharacter), false);
				return true;
			}
			return false;
		}

		private bool conversation_lord_meet_in_player_party_player_on_condition()
		{
			return true;
		}

		private bool conversation_lord_meet_player_as_liege_response_on_condition()
		{
			return HeroHelper.UnderPlayerCommand(Hero.OneToOneConversationHero) && this.UsesLordConversations(Hero.OneToOneConversationHero);
		}

		private bool conversation_lord_meet_player_response1_on_condition()
		{
			return !HeroHelper.UnderPlayerCommand(Hero.OneToOneConversationHero) && this.UsesLordConversations(Hero.OneToOneConversationHero);
		}

		private void conversation_lord_meet_player_response_on_consequence()
		{
		}

		private bool conversation_lord_meet_player_response2_on_condition()
		{
			return !HeroHelper.UnderPlayerCommand(Hero.OneToOneConversationHero) && !FactionManager.IsAtWarAgainstFaction(Hero.MainHero.MapFaction, Hero.OneToOneConversationHero.MapFaction) && this.UsesLordConversations(Hero.OneToOneConversationHero);
		}

		private bool conversation_lord_meet_player_response3_on_condition()
		{
			return !HeroHelper.UnderPlayerCommand(Hero.OneToOneConversationHero) && !FactionManager.IsAtWarAgainstFaction(Hero.MainHero.MapFaction, Hero.OneToOneConversationHero.MapFaction) && this.UsesLordConversations(Hero.OneToOneConversationHero);
		}

		private bool conversation_lord_comment_instead_introduction_on_condition()
		{
			return true;
		}

		private bool conversation_lord_introduction_on_condition()
		{
			if (Hero.OneToOneConversationHero != null && Campaign.Current.ConversationManager.CurrentConversationIsFirst && Hero.OneToOneConversationHero.IsLord && !Hero.OneToOneConversationHero.IsMinorFactionHero && !Hero.OneToOneConversationHero.IsRebel && Hero.OneToOneConversationHero.Clan.MapFaction.IsKingdomFaction && Hero.OneToOneConversationHero != null)
			{
				string text;
				if (Hero.OneToOneConversationHero.MapFaction.Leader == Hero.MainHero)
				{
					text = "str_comment_vassal_introduces_self";
				}
				else if (Hero.OneToOneConversationHero.MapFaction.Leader == Hero.OneToOneConversationHero)
				{
					text = "str_comment_liege_introduces_self";
				}
				else if (Hero.OneToOneConversationHero.MapFaction.Culture != Hero.OneToOneConversationHero.CharacterObject.Culture)
				{
					text = "str_comment_noble_generic_intro";
				}
				else if (Hero.OneToOneConversationHero.Clan.Renown >= 200f)
				{
					text = "str_comment_noble_introduces_self_and_clan";
				}
				else
				{
					text = "str_comment_noble_introduces_self";
				}
				TextObject textObject = Campaign.Current.ConversationManager.FindMatchingTextOrNull(text, CharacterObject.OneToOneConversationCharacter);
				CharacterObject.OneToOneConversationCharacter.HeroObject.SetPropertiesToTextObject(textObject, "CONVERSATION_CHARACTER");
				textObject.SetTextVariable("FACTION", Hero.OneToOneConversationHero.MapFaction.EncyclopediaLinkWithName);
				if (Hero.OneToOneConversationHero.MapFaction.Leader != null)
				{
					TextObject textObject2 = new TextObject("{=y3yN7QyC}{LEADER.LINK}, {LIEGE}", null);
					Hero.OneToOneConversationHero.MapFaction.Leader.SetPropertiesToTextObject(textObject2, "LEADER");
					textObject2.SetTextVariable("LIEGE", this.GetLiegeTitle());
					textObject.SetTextVariable("LIEGE_TITLE", textObject2);
				}
				textObject.SetTextVariable("CLAN_NAME", Hero.OneToOneConversationHero.Clan.EncyclopediaLinkWithName);
				MBTextManager.SetTextVariable("LORD_INTRODUCTION_STRING", textObject, false);
				List<TextObject> list = new List<TextObject>();
				foreach (Settlement settlement2 in Campaign.Current.Settlements.Where((Settlement settlement) => settlement.IsTown).ToList<Settlement>())
				{
					if (settlement2.OwnerClan.Leader == Hero.OneToOneConversationHero)
					{
						list.Add(settlement2.EncyclopediaLinkWithName);
					}
				}
				if (list.Count > 0)
				{
					if (list.Count > 4)
					{
						list = list.GetRange(0, 3);
						list.Add(new TextObject("{=CxavIji0}many more", null));
					}
					MBTextManager.SetTextVariable("TOWNS", GameTexts.GameTextHelper.MergeTextObjectsWithComma(list, true), false);
					if (Hero.OneToOneConversationHero.IsFemale)
					{
						MBTextManager.SetTextVariable("TOWN_INFO_STRING", GameTexts.FindText("str_and_the_lady_of_TOWNS", null), false);
					}
					else
					{
						MBTextManager.SetTextVariable("TOWN_INFO_STRING", GameTexts.FindText("str_and_the_lord_of_TOWNS", null), false);
					}
				}
				else
				{
					MBTextManager.SetTextVariable("TOWN_INFO_STRING", "", false);
				}
				return true;
			}
			return false;
		}

		private bool conversation_rebel_introduction_on_condition()
		{
			if (Hero.OneToOneConversationHero != null && Campaign.Current.ConversationManager.CurrentConversationIsFirst && Hero.OneToOneConversationHero.IsRebel)
			{
				TextObject textObject = new TextObject("{=fudD6PXR}I am {CONVERSATION_HERO.FIRSTNAME}. I have been chosen by the people of {REBEL_SETTLEMENT_NAME} to lead them in their struggle against tyranny.", null);
				textObject.SetTextVariable("REBEL_SETTLEMENT_NAME", Hero.OneToOneConversationHero.HomeSettlement.EncyclopediaLinkWithName);
				Hero.OneToOneConversationHero.SetPropertiesToTextObject(textObject, "CONVERSATION_HERO");
				MBTextManager.SetTextVariable("REBEL_INTRODUCTION_STRING", textObject, false);
				return true;
			}
			return false;
		}

		private bool conversation_minor_faction_introduction_on_condition()
		{
			if (Hero.OneToOneConversationHero != null && Campaign.Current.ConversationManager.CurrentConversationIsFirst && (Hero.OneToOneConversationHero.IsMinorFactionHero || !Hero.OneToOneConversationHero.MapFaction.IsKingdomFaction))
			{
				StringHelpers.SetCharacterProperties("CONVERSATION_HERO", Hero.OneToOneConversationHero.CharacterObject, null, false);
				PartyBase encounteredParty = PlayerEncounter.EncounteredParty;
				bool flag = FactionManager.IsAtWarAgainstFaction(((encounteredParty != null) ? encounteredParty.MapFaction : null) ?? Hero.OneToOneConversationHero.MapFaction, Hero.MainHero.MapFaction);
				if (flag)
				{
					MBTextManager.SetTextVariable("FACTION_DESCRIPTION", new TextObject("{=ADzzaxFz}You are passing through our lands without our permission.", null), false);
				}
				else if (Hero.OneToOneConversationHero.Clan.MapFaction == Hero.MainHero.Clan.MapFaction)
				{
					MBTextManager.SetTextVariable("FACTION_DESCRIPTION", new TextObject("{=CQqdXx51}Your people have been true to their word, and paid us on time, so we will be true to ours.", null), false);
				}
				else
				{
					MBTextManager.SetTextVariable("FACTION_DESCRIPTION", new TextObject("{=chj1X7VP}I do not believe we have any quarrel with you.", null), false);
				}
				if (Hero.OneToOneConversationHero.Clan.IsUnderMercenaryService)
				{
					if (flag)
					{
						MBTextManager.SetTextVariable("FACTION_DESCRIPTION", new TextObject("{=PNxKX3a5}I'm afraid I have to consider you my enemy.", null), false);
					}
					else if (Hero.OneToOneConversationHero.Clan.MapFaction == Hero.MainHero.Clan.MapFaction)
					{
						MBTextManager.SetTextVariable("FACTION_DESCRIPTION", new TextObject("{=qrej2bOU}Our pay has been arriving on time, so rest assured that we've got your back.", null), false);
					}
					else
					{
						MBTextManager.SetTextVariable("FACTION_DESCRIPTION", new TextObject("{=Yx9epCeA}As far as I know, we have no contract to fight you.", null), false);
					}
				}
				if (Hero.OneToOneConversationHero.Clan.IsSect)
				{
					MBTextManager.SetTextVariable("FACTION_DESCRIPTION", new TextObject("{=L5iagGyo}I am a brother in our order.", null), false);
					if (Hero.OneToOneConversationHero.IsFemale)
					{
						MBTextManager.SetTextVariable("FACTION_DESCRIPTION", new TextObject("{=avOVHFgy}I am a sister in our order.", null), false);
					}
				}
				MBTextManager.SetTextVariable("MINOR_FACTION_INTRODUCTION_STRING", "{=b3YNi4LG}I am {CONVERSATION_HERO.LINK}. {FACTION_DESCRIPTION}", false);
				return true;
			}
			return false;
		}

		private bool conversation_merchant_introduction_on_condition()
		{
			if (Campaign.Current.ConversationManager.CurrentConversationIsFirst && Hero.OneToOneConversationHero.IsMerchant)
			{
				TextObject textObject = TextObject.Empty;
				if (Settlement.CurrentSettlement == Hero.OneToOneConversationHero.HomeSettlement)
				{
					textObject = new TextObject("{=HfWBwR4v}I am {CONVERSATION_HERO.FIRSTNAME}, a merchant here in {SETTLEMENT_STRING}. {BUSINESS_STRING}", null);
				}
				else
				{
					textObject = new TextObject("{=1Resf6O4}I am {CONVERSATION_HERO.FIRSTNAME}, a merchant. I trade out of {SETTLEMENT_STRING}. {BUSINESS_STRING}", null);
				}
				Hero.OneToOneConversationHero.SetPropertiesToTextObject(textObject, "CONVERSATION_HERO");
				textObject.SetTextVariable("SETTLEMENT_STRING", Hero.OneToOneConversationHero.HomeSettlement.Name);
				TextObject textObject2 = TextObject.Empty;
				if (Hero.OneToOneConversationHero.HomeSettlement.IsTown)
				{
					Town town = Hero.OneToOneConversationHero.HomeSettlement.Town;
					IEnumerable<Workshop> enumerable = Hero.OneToOneConversationHero.OwnedWorkshops.DistinctBy((Workshop x) => x.WorkshopType);
					if (enumerable.Any<Workshop>())
					{
						List<TextObject> list = new List<TextObject>();
						foreach (Workshop workshop in enumerable)
						{
							TextObject textObject3 = GameTexts.FindText("str_a_STR", null);
							textObject3.SetTextVariable("STR", workshop.Name);
							list.Add(textObject3);
						}
						if (Settlement.CurrentSettlement == Hero.OneToOneConversationHero.HomeSettlement)
						{
							textObject2 = new TextObject("{=aHjUgEur}I own {.%}{BUSINESS_LIST}{.%} here.", null);
						}
						else
						{
							textObject2 = new TextObject("{=b1vTr2wT}I own {STRING_UNTIL_NOW} there.", null);
						}
						if (list.Count > 4)
						{
							list = list.GetRange(0, 3);
							list.Add(new TextObject("{=CxavIji0}many more", null));
						}
						textObject2.SetTextVariable("BUSINESS_LIST", GameTexts.GameTextHelper.MergeTextObjectsWithComma(list, true));
					}
				}
				textObject.SetTextVariable("BUSINESS_STRING", textObject2);
				MBTextManager.SetTextVariable("MERCHANT_INTRODUCTION_STRING", textObject, false);
				return true;
			}
			return false;
		}

		private bool conversation_minor_faction_preacher_introduction_on_condition()
		{
			if (Campaign.Current.ConversationManager.CurrentConversationIsFirst && Hero.OneToOneConversationHero.IsPreacher && Hero.OneToOneConversationHero.Clan != null && Hero.OneToOneConversationHero.Clan.IsMinorFaction)
			{
				StringHelpers.SetCharacterProperties("CONVERSATION_HERO", CharacterObject.OneToOneConversationCharacter, null, false);
				this.SetPreacherTextVariables();
				return true;
			}
			return false;
		}

		private bool conversation_puritan_preacher_introduction_on_condition()
		{
			if (Campaign.Current.ConversationManager.CurrentConversationIsFirst && Hero.OneToOneConversationHero.IsPreacher && Hero.OneToOneConversationHero.CharacterObject.GetTraitLevel(DefaultTraits.Generosity) <= -1)
			{
				StringHelpers.SetCharacterProperties("CONVERSATION_HERO", CharacterObject.OneToOneConversationCharacter, null, false);
				this.SetPreacherTextVariables();
				return true;
			}
			return false;
		}

		private bool conversation_messianic_preacher_introduction_on_condition()
		{
			if (Campaign.Current.ConversationManager.CurrentConversationIsFirst && Hero.OneToOneConversationHero.IsPreacher && Hero.OneToOneConversationHero.CharacterObject.GetTraitLevel(DefaultTraits.Generosity) >= 1 && Hero.OneToOneConversationHero.Culture.StringId == "khuzait")
			{
				StringHelpers.SetCharacterProperties("CONVERSATION_HERO", CharacterObject.OneToOneConversationCharacter, null, false);
				this.SetPreacherTextVariables();
				return true;
			}
			return false;
		}

		private void SetPreacherTextVariables()
		{
			if (Hero.OneToOneConversationHero.Culture.StringId == "khuzait")
			{
				MBTextManager.SetTextVariable("DIVINITY", new TextObject("{=ixjOzhua}ancestors", null), false);
				MBTextManager.SetTextVariable("FACTION_DESCRIPTION", new TextObject("{=NYLLIWgd}a messenger from the ancestors", null), false);
			}
			else
			{
				MBTextManager.SetTextVariable("DIVINITY", new TextObject("{=TGEgx2Fb}Heavens", null), false);
				MBTextManager.SetTextVariable("FACTION_DESCRIPTION", new TextObject("{=C3yPNCDt}a servant of the Heavens", null), false);
			}
			if (Hero.OneToOneConversationHero.Clan != null && Hero.OneToOneConversationHero.Clan.IsMinorFaction)
			{
				TextObject textObject = new TextObject("{=2mVZORaE}a servant of the {FACTION_NAME}", null);
				textObject.SetTextVariable("FACTION_NAME", Hero.OneToOneConversationHero.Clan.Name);
				MBTextManager.SetTextVariable("FACTION_DESCRIPTION", textObject, false);
			}
		}

		private bool conversation_mystic_preacher_introduction_on_condition()
		{
			if (Campaign.Current.ConversationManager.CurrentConversationIsFirst && Hero.OneToOneConversationHero.IsPreacher)
			{
				StringHelpers.SetCharacterProperties("CONVERSATION_HERO", CharacterObject.OneToOneConversationCharacter, null, false);
				this.SetPreacherTextVariables();
				return true;
			}
			return false;
		}

		private bool conversation_special_notable_introduction_on_condition()
		{
			if (Campaign.Current.ConversationManager.CurrentConversationIsFirst && Hero.OneToOneConversationHero.IsSpecial)
			{
				StringHelpers.SetCharacterProperties("CONVERSATION_HERO", CharacterObject.OneToOneConversationCharacter, null, false);
				return true;
			}
			return false;
		}

		private bool conversation_calculating_gangleader_introduction_on_condition()
		{
			if (Campaign.Current.ConversationManager.CurrentConversationIsFirst && Hero.OneToOneConversationHero.IsGangLeader && Hero.OneToOneConversationHero.CharacterObject.GetTraitLevel(DefaultTraits.Calculating) == 1)
			{
				StringHelpers.SetCharacterProperties("CONVERSATION_HERO", Hero.OneToOneConversationHero.CharacterObject, null, false);
				return true;
			}
			return false;
		}

		private bool conversation_ironic_gangleader_introduction_on_condition()
		{
			if (Campaign.Current.ConversationManager.CurrentConversationIsFirst && Hero.OneToOneConversationHero.IsGangLeader && Hero.OneToOneConversationHero.CharacterObject.GetPersona() == DefaultTraits.PersonaIronic)
			{
				StringHelpers.SetCharacterProperties("CONVERSATION_HERO", Hero.OneToOneConversationHero.CharacterObject, null, false);
				return true;
			}
			return false;
		}

		private bool conversation_cruel_gangleader_introduction_on_condition()
		{
			if (Campaign.Current.ConversationManager.CurrentConversationIsFirst && Hero.OneToOneConversationHero.IsGangLeader && Hero.OneToOneConversationHero.GetTraitLevel(DefaultTraits.Mercy) < 0)
			{
				StringHelpers.SetCharacterProperties("CONVERSATION_HERO", Hero.OneToOneConversationHero.CharacterObject, null, false);
				return true;
			}
			return false;
		}

		private bool conversation_default_gangleader_introduction_on_condition()
		{
			if (Campaign.Current.ConversationManager.CurrentConversationIsFirst && Hero.OneToOneConversationHero.IsGangLeader)
			{
				StringHelpers.SetCharacterProperties("CONVERSATION_HERO", Hero.OneToOneConversationHero.CharacterObject, null, false);
				return true;
			}
			return false;
		}

		private bool conversation_artisan_introduction_on_condition()
		{
			if (Campaign.Current.ConversationManager.CurrentConversationIsFirst && Hero.OneToOneConversationHero.IsArtisan)
			{
				StringHelpers.SetCharacterProperties("CONVERSATION_HERO", Hero.OneToOneConversationHero.CharacterObject, null, false);
				MBTextManager.SetTextVariable("TOWN_NAME", Settlement.CurrentSettlement.Name, false);
				return true;
			}
			return false;
		}

		private bool conversation_headman_introduction_on_condition()
		{
			if (Campaign.Current.ConversationManager.CurrentConversationIsFirst && Hero.OneToOneConversationHero.IsHeadman)
			{
				StringHelpers.SetCharacterProperties("CONVERSATION_HERO", Hero.OneToOneConversationHero.CharacterObject, null, false);
				MBTextManager.SetTextVariable("VILLAGE_NAME", Settlement.CurrentSettlement.Name, false);
				return true;
			}
			return false;
		}

		private bool conversation_rural_notable_introduction_on_condition()
		{
			if (Campaign.Current.ConversationManager.CurrentConversationIsFirst && Hero.OneToOneConversationHero.IsRuralNotable)
			{
				StringHelpers.SetCharacterProperties("CONVERSATION_HERO", Hero.OneToOneConversationHero.CharacterObject, null, false);
				return true;
			}
			return false;
		}

		private bool conversation_wanderer_preintroduction_on_condition()
		{
			string stringId = Hero.OneToOneConversationHero.Template.StringId;
			TextObject textObject = GameTexts.FindText("prebackstory", stringId);
			MBTextManager.SetTextVariable("WANDERER_PREBACKSTORY", textObject ?? TextObject.Empty, false);
			CharacterObject characterObject;
			this._previouslyMetWandererTemplates.TryGetValue(Hero.OneToOneConversationHero.Template, out characterObject);
			if (characterObject != null && characterObject != Hero.OneToOneConversationHero.CharacterObject)
			{
				TextObject textObject2;
				GameTexts.TryGetText("generic_backstory", out textObject2, stringId);
				if (textObject2 == null)
				{
					MBTextManager.SetTextVariable("WANDERER_PREBACKSTORY", "{=MnluqvyE}I do not care to talk about my past.", false);
					textObject2 = new TextObject("{=pBb6sevv}It is enough to say that I am looking for a new employer, and I will serve loyally so long as I am treated well and paid well.", null);
				}
				MBTextManager.SetTextVariable("WANDERER_GENERIC_BACKSTORY", textObject2, false);
			}
			return true;
		}

		private bool conversation_wanderer_introduction_on_condition()
		{
			if (this.conversation_wanderer_on_condition())
			{
				StringHelpers.SetCharacterProperties("CONVERSATION_CHARACTER", Hero.OneToOneConversationHero.CharacterObject, null, false);
				string stringId = Hero.OneToOneConversationHero.Template.StringId;
				CharacterObject characterObject;
				this._previouslyMetWandererTemplates.TryGetValue(Hero.OneToOneConversationHero.Template, out characterObject);
				if (characterObject == null || characterObject == Hero.OneToOneConversationHero.CharacterObject)
				{
					if (characterObject == null)
					{
						this._previouslyMetWandererTemplates[Hero.OneToOneConversationHero.Template] = Hero.OneToOneConversationHero.CharacterObject;
					}
					MBTextManager.SetTextVariable("IMPERIALCAPITAL", Settlement.FindFirst((Settlement x) => x.StringId == "town_ES4").Name, false);
					MBTextManager.SetTextVariable("WANDERER_BACKSTORY_A", GameTexts.FindText("backstory_a", stringId), false);
					MBTextManager.SetTextVariable("WANDERER_BACKSTORY_B", GameTexts.FindText("backstory_b", stringId), false);
					MBTextManager.SetTextVariable("WANDERER_BACKSTORY_C", GameTexts.FindText("backstory_c", stringId), false);
					MBTextManager.SetTextVariable("BACKSTORY_RESPONSE_1", GameTexts.FindText("response_1", stringId), false);
					MBTextManager.SetTextVariable("BACKSTORY_RESPONSE_2", GameTexts.FindText("response_2", stringId), false);
					MBTextManager.SetTextVariable("WANDERER_BACKSTORY_D", GameTexts.FindText("backstory_d", stringId), false);
					StringHelpers.SetCharacterProperties("MET_WANDERER", Hero.OneToOneConversationHero.CharacterObject, null, false);
					if (CampaignMission.Current.Location != null && CampaignMission.Current.Location.StringId != "tavern")
					{
						MBTextManager.SetTextVariable("WANDERER_PREBACKSTORY", GameTexts.FindText("spc_prebackstory_generic", null), false);
					}
					return true;
				}
			}
			return false;
		}

		private bool conversation_wanderer_player_owned_on_condition()
		{
			return CharacterObject.OneToOneConversationCharacter != null && CharacterObject.OneToOneConversationCharacter.IsHero && Hero.OneToOneConversationHero.CompanionOf != null && Hero.OneToOneConversationHero.IsPlayerCompanion;
		}

		private bool conversation_wanderer_job_status_on_condition()
		{
			if (Hero.OneToOneConversationHero.CompanionOf != null)
			{
				MBTextManager.SetTextVariable("EMPLOYER", Hero.OneToOneConversationHero.CompanionOf.Name, false);
				return true;
			}
			return false;
		}

		private bool conversation_wanderer_set_job_line_on_condition()
		{
			MBTextManager.SetTextVariable("WANDERER_JOB_OFFER", "{=BQjAAo9f}Right now I'm looking for work, if you have anything to offer.", false);
			if (Hero.OneToOneConversationHero.CharacterObject.GetPersona() == DefaultTraits.PersonaCurt)
			{
				MBTextManager.SetTextVariable("WANDERER_JOB_OFFER", "{=8wdzSgfZ}If you have work for me, we could discuss it.", false);
			}
			else if (Hero.OneToOneConversationHero.CharacterObject.GetPersona() == DefaultTraits.PersonaIronic)
			{
				MBTextManager.SetTextVariable("WANDERER_JOB_OFFER", "{=z1nFn4Ug}You could say perhaps that I am between jobs right now, so if by any chance you are looking to hire... Well, I'm open to discussion.", false);
			}
			else if (Hero.OneToOneConversationHero.Age >= 25f)
			{
				MBTextManager.SetTextVariable("WANDERER_JOB_OFFER", "{=nPyNq1MT}Right now I'm between jobs, so if you've got any work for me, I'm willing to discuss it.", false);
			}
			return true;
		}

		public bool conversation_wanderer_generic_introduction_on_condition()
		{
			if (this.conversation_wanderer_on_condition())
			{
				StringHelpers.SetCharacterProperties("CONVERSATION_CHARACTER", Hero.OneToOneConversationHero.CharacterObject, null, false);
				return true;
			}
			return false;
		}

		private bool conversation_wanderer_meet_player_on_condition()
		{
			return this.conversation_wanderer_on_condition();
		}

		private bool conversation_lord_makes_preattack_comment_on_condition()
		{
			return HeroHelper.WillLordAttack() && this.conversation_lord_makes_comment_on_condition();
		}

		private bool conversation_lord_makes_comment_on_condition()
		{
			if (this.lord_comments())
			{
				int num;
				string text;
				Campaign.Current.LogEntryHistory.GetRelevantComment(Hero.OneToOneConversationHero, out num, out text);
				if (num > 0)
				{
					ConversationHelper.ConversationTroopCommentShown = true;
					MBTextManager.SetTextVariable("COMMENT_STRING", Campaign.Current.ConversationManager.FindMatchingTextOrNull(text, CharacterObject.OneToOneConversationCharacter), false);
					if (text != "str_comment_intro" && !text.Contains("str_comment_special_clan_intro") && text != "str_comment_we_have_rebelled" && Campaign.Current.ConversationManager.CurrentConversationIsFirst)
					{
						MBTextManager.SetTextVariable("COMMENT_STRING_MAIN", Campaign.Current.ConversationManager.FindMatchingTextOrNull(text, CharacterObject.OneToOneConversationCharacter), false);
						MBTextManager.SetTextVariable("COMMENT_STRING", GameTexts.FindText("str_i_know_your_name", null), false);
					}
					return true;
				}
			}
			return false;
		}

		private bool conversation_lord_greets_under_24_hours_on_condition()
		{
			if (Campaign.Current.CurrentConversationContext == ConversationContext.FreedHero || Campaign.Current.CurrentConversationContext == ConversationContext.CapturedLord)
			{
				return false;
			}
			if (Hero.OneToOneConversationHero != null && Hero.OneToOneConversationHero.LastMeetingTimeWithPlayer.ElapsedHoursUntilNow < 24f)
			{
				TextObject textObject = new TextObject("{=!}{SALUTATION}...", null);
				textObject.SetTextVariable("SALUTATION", Campaign.Current.ConversationManager.FindMatchingTextOrNull("str_salutation", CharacterObject.OneToOneConversationCharacter));
				MBTextManager.SetTextVariable("SHORT_ABSENCE_GREETING", textObject, false);
				return true;
			}
			return false;
		}

		private bool conversation_lord_greets_over_24_hours_on_condition()
		{
			if (Campaign.Current.CurrentConversationContext == ConversationContext.FreedHero || Campaign.Current.CurrentConversationContext == ConversationContext.CapturedLord)
			{
				return false;
			}
			if (Hero.OneToOneConversationHero != null && Hero.OneToOneConversationHero.LastMeetingTimeWithPlayer.ElapsedHoursUntilNow < 24f)
			{
				return false;
			}
			if (Hero.OneToOneConversationHero != null && !Campaign.Current.ConversationManager.CurrentConversationIsFirst && this.UsesLordConversations(Hero.OneToOneConversationHero))
			{
				this.conversations_set_voiced_line();
				return true;
			}
			return false;
		}

		private bool debug_mode_enabled_condition()
		{
			return Game.Current.IsDevelopmentMode;
		}

		private bool conversation_lord_news_on_condition()
		{
			return CharacterObject.OneToOneConversationCharacter.HeroObject != null && (Hero.OneToOneConversationHero.MapFaction == Hero.MainHero.MapFaction || !FactionManager.IsAtWarAgainstFaction(Hero.OneToOneConversationHero.MapFaction, Hero.MainHero.MapFaction));
		}

		public bool conversation_lord_answers_war_question_about_faction_on_condition()
		{
			MBTextManager.SetTextVariable("FACTION_NAME", this._askedFaction.Name, false);
			return true;
		}

		public void conversation_lord_answers_the_war_question_on_consequence()
		{
			ConversationSentence.SetObjectsToRepeatOver(Kingdom.All, 5);
		}

		public bool conversation_lord_talk_ask_about_war_2_on_condition()
		{
			Clan clan = ConversationSentence.CurrentProcessedRepeatObject as Clan;
			if (clan != Hero.OneToOneConversationHero.MapFaction && FactionManager.IsAtWarAgainstFaction(Hero.OneToOneConversationHero.MapFaction, clan))
			{
				MBTextManager.SetTextVariable("FACTION_NAME", clan.Name, false);
				return true;
			}
			return false;
		}

		public void conversation_lord_talk_ask_about_war_2_on_consequence()
		{
			this._askedFaction = ConversationSentence.CurrentProcessedRepeatObject as Clan;
		}

		private bool conversation_hero_main_options_have_issue_on_condition()
		{
			if (Hero.OneToOneConversationHero == null || Hero.OneToOneConversationHero.IsPrisoner)
			{
				return false;
			}
			IssueBase issue = Hero.OneToOneConversationHero.Issue;
			return Hero.OneToOneConversationHero != null && issue != null && issue.IsOngoingWithoutQuest;
		}

		private bool conversation_hero_main_options_have_issue_on_clickable_condition(out TextObject hint)
		{
			Hero hero = Hero.OneToOneConversationHero;
			IssueBase issueBase;
			Campaign.Current.IssueManager.Issues.TryGetValue(hero, out issueBase);
			QuestBase questBase = Campaign.Current.QuestManager.Quests.FirstOrDefault((QuestBase x) => x.QuestGiver == hero || x.IsTracked(hero));
			hint = TextObject.Empty;
			if (issueBase != null)
			{
				hint = issueBase.Title;
			}
			else if (questBase != null)
			{
				hint = questBase.Title;
			}
			return true;
		}

		private bool conversation_lord_task_given_on_condition()
		{
			if (!Hero.OneToOneConversationHero.IsPrisoner && Campaign.Current.QuestManager.IsQuestGiver(Hero.OneToOneConversationHero))
			{
				foreach (QuestBase questBase in Campaign.Current.QuestManager.GetQuestGiverQuests(Hero.OneToOneConversationHero))
				{
					if (!questBase.IsSpecialQuest)
					{
						return questBase.IsThereDiscussDialogFlow;
					}
				}
				return false;
			}
			return false;
		}

		private bool conversation_lord_task_given_alternative_on_condition()
		{
			Hero oneToOneConversationHero = Hero.OneToOneConversationHero;
			bool flag;
			if (oneToOneConversationHero == null)
			{
				flag = false;
			}
			else
			{
				IssueBase issue = oneToOneConversationHero.Issue;
				bool? flag2 = ((issue != null) ? new bool?(issue.IsSolvingWithAlternative) : null);
				bool flag3 = true;
				flag = (flag2.GetValueOrDefault() == flag3) & (flag2 != null);
			}
			return flag && Hero.OneToOneConversationHero.Issue.IsThereDiscussDialogFlow;
		}

		private bool conversation_hero_hire_on_condition()
		{
			return Hero.OneToOneConversationHero != null && !Hero.OneToOneConversationHero.IsPlayerCompanion && this.conversation_wanderer_on_condition() && Hero.OneToOneConversationHero.PartyBelongedTo == null;
		}

		private bool conversation_companion_hire_gold_on_condition()
		{
			MBTextManager.SetTextVariable("GOLD_AMOUNT", Campaign.Current.Models.CompanionHiringPriceCalculationModel.GetCompanionHiringPrice(Hero.OneToOneConversationHero));
			MBTextManager.SetTextVariable("HIRING_COST_EXPLANATION", "{=7sAm6qwp}Very well. I'm going to need about {GOLD_AMOUNT}{GOLD_ICON} to settle up some debts, though. Can you pay?", false);
			if (Hero.OneToOneConversationHero.GetTraitLevel(DefaultTraits.Mercy) + Hero.OneToOneConversationHero.GetTraitLevel(DefaultTraits.Honor) < 0 && Hero.OneToOneConversationHero.CharacterObject.GetPersona() == DefaultTraits.PersonaIronic)
			{
				MBTextManager.SetTextVariable("HIRING_COST_EXPLANATION", "{=8Mx8gMmw}One other small thing... I've had to take some money from some fairly dangerous people around here. I'll need {GOLD_AMOUNT}{GOLD_ICON} to get that beast off my back. Do you reckon you can pay me that?", false);
			}
			else if (Hero.OneToOneConversationHero.GetTraitLevel(DefaultTraits.Mercy) + Hero.OneToOneConversationHero.GetTraitLevel(DefaultTraits.Generosity) > 0 && Hero.OneToOneConversationHero.GetTraitLevel(DefaultTraits.Honor) < 0 && !Hero.OneToOneConversationHero.IsFemale)
			{
				MBTextManager.SetTextVariable("HIRING_COST_EXPLANATION", "{=K1RtrtvH}So, uh, there's a young woman around here. I really need to leave her some money before I go anywhere. Let's say {GOLD_AMOUNT}{GOLD_ICON} - can you pay me that?", false);
			}
			else if (Hero.OneToOneConversationHero.CharacterObject.GetPersona() == DefaultTraits.PersonaCurt && Hero.OneToOneConversationHero.GetTraitLevel(DefaultTraits.Mercy) < 0)
			{
				MBTextManager.SetTextVariable("HIRING_COST_EXPLANATION", "{=PlhbjNOE}Just so you know... I'm not cheap. I want {GOLD_AMOUNT}{GOLD_ICON} as an advance, or there's no deal.[ib:warrior]", false);
			}
			else if (Hero.OneToOneConversationHero.CharacterObject.GetPersona() == DefaultTraits.PersonaCurt)
			{
				MBTextManager.SetTextVariable("HIRING_COST_EXPLANATION", "{=9kHU4AMD}Great. Going to need some money in advance though - {GOLD_AMOUNT}{GOLD_ICON}. Can you pay?", false);
			}
			else if (Hero.OneToOneConversationHero.GetTraitLevel(DefaultTraits.Honor) < 0)
			{
				MBTextManager.SetTextVariable("HIRING_COST_EXPLANATION", "{=loLetAI9}Very well. But the world being as it is, I'm going to need {GOLD_AMOUNT}{GOLD_ICON} as a down payment on my services. Can you pay that?", false);
			}
			else if (Hero.OneToOneConversationHero.GetTraitLevel(DefaultTraits.Mercy) > 0 || Hero.OneToOneConversationHero.GetTraitLevel(DefaultTraits.Generosity) > 0)
			{
				MBTextManager.SetTextVariable("HIRING_COST_EXPLANATION", "{=9g6FB5Y7}There are some townspeople who've looked after me here, made sure I was fed and that. I'd like to give them something before I go. Could I ask for {GOLD_AMOUNT}{GOLD_ICON} as an advance?", false);
			}
			return true;
		}

		private bool conversation_companion_hire_on_condition()
		{
			GameTexts.SetVariable("STR1", Campaign.Current.Models.CompanionHiringPriceCalculationModel.GetCompanionHiringPrice(Hero.OneToOneConversationHero));
			GameTexts.SetVariable("STR2", "{=!}<img src=\"General\\Icons\\Coin@2x\" extend=\"8\">");
			MBTextManager.SetTextVariable("GOLD_AMOUNT", GameTexts.FindText("str_STR1_STR2", null), false);
			return Hero.MainHero.Gold > Campaign.Current.Models.CompanionHiringPriceCalculationModel.GetCompanionHiringPrice(Hero.OneToOneConversationHero) && !this.too_many_companions();
		}

		private void conversation_companion_hire_on_consequence()
		{
			GiveGoldAction.ApplyBetweenCharacters(Hero.MainHero, Hero.OneToOneConversationHero, Campaign.Current.Models.CompanionHiringPriceCalculationModel.GetCompanionHiringPrice(Hero.OneToOneConversationHero), false);
			AddCompanionAction.Apply(Clan.PlayerClan, Hero.OneToOneConversationHero);
			AddHeroToPartyAction.Apply(Hero.OneToOneConversationHero, MobileParty.MainParty, true);
		}

		private bool conversation_lord_barter_on_condition()
		{
			return Hero.OneToOneConversationHero != null && Hero.OneToOneConversationHero.IsLord && !Hero.OneToOneConversationHero.IsPrisoner && !Hero.OneToOneConversationHero.MapFaction.IsMinorFaction && !FactionManager.IsAtWarAgainstFaction(Hero.MainHero.MapFaction, Hero.OneToOneConversationHero.MapFaction);
		}

		private bool conversation_lord_join_army_on_condition()
		{
			return Hero.OneToOneConversationHero != null && Hero.OneToOneConversationHero.PartyBelongedTo != null && Hero.OneToOneConversationHero.PartyBelongedTo.Army != null && Hero.OneToOneConversationHero.Clan != Hero.MainHero.Clan && MobileParty.MainParty.Army == null && Hero.OneToOneConversationHero.PartyBelongedTo.Army != MobileParty.MainParty.Army && Hero.OneToOneConversationHero.MapFaction == Hero.MainHero.MapFaction && Hero.OneToOneConversationHero.PartyBelongedTo.Army.LeaderParty == Hero.OneToOneConversationHero.PartyBelongedTo;
		}

		private bool conversation_player_can_ask_to_be_let_go_on_condition()
		{
			return HeroHelper.WillLordAttack();
		}

		private bool conversation_lord_join_army_on_clickable_condition(out TextObject hint)
		{
			foreach (Kingdom kingdom in Kingdom.All)
			{
				if (kingdom.IsAtWarWith(Clan.PlayerClan.MapFaction) && kingdom.NotAttackableByPlayerUntilTime.IsFuture)
				{
					hint = GameTexts.FindText("str_cant_join_army_safe_passage", null);
					return false;
				}
			}
			hint = TextObject.Empty;
			return true;
		}

		private bool conversation_player_can_ask_for_siege_to_be_lifted_on_condition()
		{
			return this.PlayerIsBesieged() && !Hero.OneToOneConversationHero.MapFaction.IsMinorFaction;
		}

		private bool conversation_player_can_bribe_lord_for_passage_on_condition()
		{
			return !this.PlayerIsBesieged() && HeroHelper.WillLordAttack() && !Hero.OneToOneConversationHero.MapFaction.IsMinorFaction;
		}

		private bool conversation_player_can_ask_for_honors_of_war_on_condition()
		{
			return this.PlayerIsBesieged();
		}

		private void conversation_set_up_generic_barter_on_consequence()
		{
			BarterManager instance = BarterManager.Instance;
			Hero mainHero = Hero.MainHero;
			Hero oneToOneConversationHero = Hero.OneToOneConversationHero;
			PartyBase mainParty = PartyBase.MainParty;
			MobileParty partyBelongedTo = Hero.OneToOneConversationHero.PartyBelongedTo;
			instance.StartBarterOffer(mainHero, oneToOneConversationHero, mainParty, (partyBelongedTo != null) ? partyBelongedTo.Party : null, null, null, 0, false, null);
		}

		private void conversation_set_up_safe_passage_barter_on_consequence()
		{
			BarterManager instance = BarterManager.Instance;
			Hero mainHero = Hero.MainHero;
			Hero oneToOneConversationHero = Hero.OneToOneConversationHero;
			PartyBase mainParty = PartyBase.MainParty;
			MobileParty conversationParty = MobileParty.ConversationParty;
			PartyBase partyBase = ((conversationParty != null) ? conversationParty.Party : null);
			Hero hero = null;
			BarterManager.BarterContextInitializer barterContextInitializer = new BarterManager.BarterContextInitializer(BarterManager.Instance.InitializeSafePassageBarterContext);
			int num = 0;
			bool flag = false;
			Barterable[] array = new Barterable[2];
			int num2 = 0;
			Hero oneToOneConversationHero2 = Hero.OneToOneConversationHero;
			Hero mainHero2 = Hero.MainHero;
			MobileParty conversationParty2 = MobileParty.ConversationParty;
			array[num2] = new SafePassageBarterable(oneToOneConversationHero2, mainHero2, (conversationParty2 != null) ? conversationParty2.Party : null, PartyBase.MainParty);
			int num3 = 1;
			Hero mainHero3 = Hero.MainHero;
			Hero oneToOneConversationHero3 = Hero.OneToOneConversationHero;
			PartyBase mainParty2 = PartyBase.MainParty;
			MobileParty conversationParty3 = MobileParty.ConversationParty;
			array[num3] = new NoAttackBarterable(mainHero3, oneToOneConversationHero3, mainParty2, (conversationParty3 != null) ? conversationParty3.Party : null, CampaignTime.Days(5f));
			instance.StartBarterOffer(mainHero, oneToOneConversationHero, mainParty, partyBase, hero, barterContextInitializer, num, flag, array);
		}

		private bool conversation_barter_successful_on_condition()
		{
			return Campaign.Current.BarterManager.LastBarterIsAccepted;
		}

		public bool conversation_lord_active_mission_response_cont_on_condition()
		{
			return true;
		}

		public bool conversation_mission_in_progress_on_condition()
		{
			return false;
		}

		public bool conversation_lord_active_mission_response_failed_on_condition()
		{
			return true;
		}

		public void conversation_lord_active_mission_response_failed_on_consequence()
		{
		}

		public bool conversation_lord_is_threated_neutral_on_condition()
		{
			return CharacterObject.OneToOneConversationCharacter.IsHero && !CharacterObject.OneToOneConversationCharacter.HeroObject.IsPrisoner && Campaign.Current.CurrentConversationContext == ConversationContext.PartyEncounter && Settlement.CurrentSettlement == null && CharacterObject.OneToOneConversationCharacter.IsHero && !CharacterObject.OneToOneConversationCharacter.HeroObject.IsPlayerCompanion && FactionManager.IsNeutralWithFaction(Hero.OneToOneConversationHero.MapFaction, Hero.MainHero.MapFaction);
		}

		private bool conversation_player_can_attack_hero_on_clickable_condition(out TextObject hint)
		{
			MobileParty encounteredMobileParty = PlayerEncounter.EncounteredMobileParty;
			if (encounteredMobileParty != null && encounteredMobileParty.MapFaction != null && DiplomacyHelper.DidMainHeroSwornNotToAttackFaction(encounteredMobileParty.MapFaction, out hint))
			{
				return false;
			}
			hint = TextObject.Empty;
			return true;
		}

		public bool conversation_player_can_attack_hero_on_condition()
		{
			return Hero.OneToOneConversationHero != null && Campaign.Current.CurrentConversationContext == ConversationContext.PartyEncounter && (FactionManager.IsAtWarAgainstFaction(Hero.OneToOneConversationHero.MapFaction, Hero.MainHero.MapFaction) && PlayerEncounter.EncounteredMobileParty != null) && PlayerEncounter.EncounteredMobileParty.LeaderHero == Hero.OneToOneConversationHero;
		}

		public bool barter_peace_offer_reject_on_condition()
		{
			return !Campaign.Current.BarterManager.LastBarterIsAccepted && PlayerEncounter.EncounteredMobileParty != null && FactionManager.IsAtWarAgainstFaction(Hero.MainHero.MapFaction, PlayerEncounter.EncounteredMobileParty.MapFaction) && PlayerEncounter.PlayerIsDefender;
		}

		public bool barter_offer_reject_on_condition()
		{
			if (!Campaign.Current.BarterManager.LastBarterIsAccepted && !this.conversation_player_can_attack_hero_on_condition())
			{
				TextObject textObject = Campaign.Current.ConversationManager.FindMatchingTextOrNull("STR_CHANGE_SIDES_DECLINE_OFFER", Hero.OneToOneConversationHero.CharacterObject);
				MBTextManager.SetTextVariable("STR_BARTER_DECLINE_OFFER", textObject, false);
				MBTextManager.SetTextVariable("BARTER_RESULT", "0", false);
				return true;
			}
			return false;
		}

		public bool barter_offer_accept_peace_on_condition()
		{
			return Campaign.Current.BarterManager.LastBarterIsAccepted;
		}

		public bool barter_offer_accept_let_go_on_condition()
		{
			return Campaign.Current.BarterManager.LastBarterIsAccepted;
		}

		public bool barter_offer_accept_on_condition()
		{
			return Campaign.Current.BarterManager.LastBarterIsAccepted;
		}

		public bool conversation_player_is_leaving_faction_on_condition()
		{
			if (CharacterObject.OneToOneConversationCharacter.IsHero && Hero.OneToOneConversationHero.MapFaction != null && MobileParty.MainParty.Army == null && !FactionManager.IsAtWarAgainstFaction(Hero.OneToOneConversationHero.MapFaction, Hero.MainHero.MapFaction) && Hero.OneToOneConversationHero.MapFaction.Leader == Hero.OneToOneConversationHero && Hero.MainHero.MapFaction == Hero.OneToOneConversationHero.MapFaction)
			{
				Hero.OneToOneConversationHero.SetTextVariables();
				MBTextManager.SetTextVariable("LORD_SALUTATION", GameTexts.FindText(CharacterObject.OneToOneConversationCharacter.IsFemale ? "str_player_salutation_my_lady" : "str_player_salutation_my_lord", null), false);
				return true;
			}
			return false;
		}

		public bool conversation_player_is_offering_mercenary_on_condition()
		{
			return !Hero.MainHero.MapFaction.IsKingdomFaction && Hero.OneToOneConversationHero != null && Hero.OneToOneConversationHero.Clan != null && !Hero.OneToOneConversationHero.Clan.IsUnderMercenaryService;
		}

		public bool conversation_player_is_offering_mercenary_on_clickable_condition(out TextObject hintText)
		{
			List<IFaction> list;
			List<IFaction> list2;
			bool flag = FactionHelper.CanPlayerOfferMercenaryService((Kingdom)Hero.OneToOneConversationHero.MapFaction, out list, out list2);
			hintText = new TextObject("", null);
			if (!flag)
			{
				if (Clan.PlayerClan.Tier < Campaign.Current.Models.ClanTierModel.MercenaryEligibleTier)
				{
					hintText = new TextObject("{=kXcUbkEW}Your Clan Tier needs to be {TIER}", null);
				}
				else if (Hero.OneToOneConversationHero.GetRelationWithPlayer() < (float)Campaign.Current.Models.DiplomacyModel.MinimumRelationWithConversationCharacterToJoinKingdom)
				{
					hintText = new TextObject("{=S9yOQgb1}You need {RELATION} relation with {HERO.NAME}.", null);
				}
				else if (list2.Intersect(list).Count<IFaction>() != list.Count)
				{
					hintText = new TextObject("{=5Que0iuJ}Your clan is at war with factions that {KINGDOM} is not hostile with.", null);
				}
				else if (!Clan.PlayerClan.Settlements.IsEmpty<Settlement>())
				{
					hintText = new TextObject("{=fJxnOIHS}Clans that own a settlement are not considered as mercenaries.", null);
				}
				else
				{
					hintText = new TextObject("{=x3y4bSJz}Your Clan Tier needs to be {TIER}.{newline} You need relations of {RELATION} with {HERO.NAME}.", null);
				}
				hintText.SetTextVariable("TIER", Campaign.Current.Models.ClanTierModel.MercenaryEligibleTier);
				hintText.SetTextVariable("RELATION", Campaign.Current.Models.DiplomacyModel.MinimumRelationWithConversationCharacterToJoinKingdom);
				hintText.SetTextVariable("KINGDOM", Hero.OneToOneConversationHero.MapFaction.Name);
				hintText.SetCharacterProperties("HERO", Hero.OneToOneConversationHero.CharacterObject, false);
				hintText.SetTextVariable("newline", "\n");
			}
			return flag;
		}

		public bool conversation_player_is_offering_vassalage_on_condition()
		{
			if ((!Hero.MainHero.MapFaction.IsKingdomFaction || Clan.PlayerClan.IsUnderMercenaryService) && Hero.OneToOneConversationHero != null && Hero.OneToOneConversationHero.Clan != null)
			{
				Hero.OneToOneConversationHero.SetTextVariables();
				return true;
			}
			return false;
		}

		public bool conversation_player_is_offering_vassalage_on_clickable_condition(out TextObject hintText)
		{
			List<IFaction> list;
			List<IFaction> list2;
			bool flag = FactionHelper.CanPlayerOfferVassalage((Kingdom)Hero.OneToOneConversationHero.MapFaction, out list, out list2);
			hintText = new TextObject("", null);
			if (!flag)
			{
				if (Hero.OneToOneConversationHero.MapFaction.Leader != Hero.OneToOneConversationHero)
				{
					hintText = new TextObject("{=9P3atGKL}Only a faction leader can grant vassalhood. Your relationship with that leader must be at least {RELATION} and you need a Clan Tier of {TIER}.", null);
				}
				else if (Clan.PlayerClan.Tier < Campaign.Current.Models.ClanTierModel.VassalEligibleTier)
				{
					hintText = new TextObject("{=D1JTdYt6}Your Clan Tier needs to be {TIER} to become a vassal.", null);
				}
				else if (Hero.OneToOneConversationHero.GetRelationWithPlayer() < (float)Campaign.Current.Models.DiplomacyModel.MinimumRelationWithConversationCharacterToJoinKingdom)
				{
					hintText = new TextObject("{=S9yOQgb1}You need {RELATION} relation with {HERO.NAME}.", null);
				}
				else if (list2.Intersect(list).Count<IFaction>() != list.Count)
				{
					hintText = new TextObject("{=B818zx3W}Your clan is at war with factions with which the {KINGDOM} is at peace.", null);
				}
				else
				{
					hintText = new TextObject("{=FOYSHPoA}Your Clan Tier needs to be {TIER}.{newline}You also need a relation of {RELATION} with {HERO.NAME}.", null);
				}
				hintText.SetTextVariable("TIER", Campaign.Current.Models.ClanTierModel.VassalEligibleTier);
				hintText.SetTextVariable("RELATION", Campaign.Current.Models.DiplomacyModel.MinimumRelationWithConversationCharacterToJoinKingdom);
				hintText.SetTextVariable("KINGDOM", Hero.OneToOneConversationHero.MapFaction.InformalName);
				hintText.SetCharacterProperties("HERO", Hero.OneToOneConversationHero.CharacterObject, false);
				hintText.SetTextVariable("newline", "\n");
			}
			return flag;
		}

		public bool conversation_player_is_offering_vassalage_while_at_mercenary_service_on_condition()
		{
			if (Hero.OneToOneConversationHero.Clan != null && !Hero.OneToOneConversationHero.IsPrisoner && Clan.PlayerClan.Tier >= Campaign.Current.Models.ClanTierModel.VassalEligibleTier && Hero.OneToOneConversationHero.Clan != Clan.PlayerClan && !Hero.OneToOneConversationHero.Clan.IsUnderMercenaryService && Hero.MainHero.Clan.IsUnderMercenaryService && Hero.MainHero.MapFaction == Hero.OneToOneConversationHero.MapFaction && Hero.OneToOneConversationHero.GetRelationWithPlayer() >= (float)Campaign.Current.Models.DiplomacyModel.MinimumRelationWithConversationCharacterToJoinKingdom)
			{
				Hero.OneToOneConversationHero.SetTextVariables();
				return true;
			}
			return false;
		}

		private bool conversation_reject_vassalage_on_condition()
		{
			CultureObject culture = Hero.OneToOneConversationHero.Culture;
			if (Hero.OneToOneConversationHero.Culture.StringId == "empire")
			{
				MBTextManager.SetTextVariable("VASSALAGE_REJECTION", "{=YaokE1mi}Valiant warriors have always been welcomed as citizens of the Empire and granted lands, titles and honors, {PLAYER.NAME}, but you have yet to prove yourself. Take your sword to my enemies and we may speak of this later.", false);
			}
			else if (Hero.OneToOneConversationHero.Culture.StringId == "vlandia" || Hero.OneToOneConversationHero.Culture.StringId == "battania")
			{
				MBTextManager.SetTextVariable("VASSALAGE_REJECTION", "{=NuaqWqze}You have yet to show yourself a competent leader of men, {PLAYER.NAME}. Prove yourself against my enemies, and I would be glad to have you as a vassal and entrust you with a fief of your own.", false);
			}
			else
			{
				MBTextManager.SetTextVariable("VASSALAGE_REJECTION", "{=sG9bKLdg}We welcome valiant warriors into our people, {PLAYER.NAME}, but you must prove yourself. Take your sword to my enemies and make a name for yourself, and we will formally adopt you as one of us. You may be given lands to protect and can speak at our councils.", false);
			}
			return true;
		}

		public bool conversation_player_is_asking_service_while_in_faction_on_condition()
		{
			return Hero.MainHero.MapFaction.IsKingdomFaction && Hero.MainHero.MapFaction != Hero.OneToOneConversationHero.MapFaction && !Clan.PlayerClan.IsUnderMercenaryService;
		}

		public bool conversation_player_is_offering_vassalage_to_lord_on_condition()
		{
			if (Hero.OneToOneConversationHero != null && Hero.OneToOneConversationHero.MapFaction.Leader != Hero.OneToOneConversationHero)
			{
				Hero leader = Hero.OneToOneConversationHero.MapFaction.Leader;
				Settlement closestSettlement = HeroHelper.GetClosestSettlement(leader);
				if (closestSettlement != null)
				{
					leader.UpdateLastKnownClosestSettlement(closestSettlement);
				}
				MBTextManager.SetTextVariable("IS_PRISONED", leader.IsPrisoner ? 1 : 0);
				MBTextManager.SetTextVariable("IS_IN_SETTLEMENT", (leader.CurrentSettlement == leader.LastKnownClosestSettlement) ? 1 : 0);
				MBTextManager.SetTextVariable("SETTLEMENT", leader.LastKnownClosestSettlement.EncyclopediaLinkWithName, false);
				StringHelpers.SetCharacterProperties("KING", leader.CharacterObject, null, false);
				return true;
			}
			return false;
		}

		public bool lord_ask_enter_service_vassalage_talking_with_king_on_condition()
		{
			float num = (Hero.MainHero.IsFriend(Hero.OneToOneConversationHero) ? 3f : (Hero.MainHero.IsEnemy(Hero.OneToOneConversationHero) ? 9f : 6f));
			if (Hero.MainHero.Clan.Influence > (float)((int)num) && Hero.MainHero.MapFaction == Hero.OneToOneConversationHero.MapFaction)
			{
				MBTextManager.SetTextVariable("AMOUNT", (int)num);
				return true;
			}
			return false;
		}

		public bool conversation_lord_ask_recruit_mercenary_response_on_condition()
		{
			return false;
		}

		public bool conversation_player_want_to_fire_mercenary_on_condition()
		{
			MBTextManager.SetTextVariable("FACTION_NAME", Clan.PlayerClan.Name, false);
			return Hero.MainHero.MapFaction != null && !Hero.OneToOneConversationHero.IsPrisoner && Hero.MainHero.MapFaction.IsKingdomFaction && ((Kingdom)Hero.MainHero.MapFaction).Leader == Hero.MainHero && Hero.OneToOneConversationHero.IsMinorFactionHero && Hero.OneToOneConversationHero.MapFaction == MobileParty.MainParty.MapFaction;
		}

		public bool conversation_player_want_to_hire_mercenary_on_condition()
		{
			MBTextManager.SetTextVariable("PLAYER_FACTION", Clan.PlayerClan.Name, false);
			bool flag;
			if (Hero.MainHero.MapFaction != null && !Hero.OneToOneConversationHero.IsPrisoner && Hero.MainHero.MapFaction.IsKingdomFaction && ((Kingdom)Hero.MainHero.MapFaction).Leader == Hero.MainHero && Hero.OneToOneConversationHero.IsMinorFactionHero)
			{
				MobileParty partyBelongedTo = Hero.OneToOneConversationHero.PartyBelongedTo;
				if (((partyBelongedTo != null) ? partyBelongedTo.Army : null) == null && !FactionManager.IsAtWarAgainstFaction(Hero.OneToOneConversationHero.MapFaction, Hero.MainHero.MapFaction))
				{
					flag = Hero.OneToOneConversationHero.Clan.MapFaction != Hero.MainHero.MapFaction;
					goto IL_BF;
				}
			}
			flag = false;
			IL_BF:
			if (flag)
			{
				int num = 0;
				using (List<Clan>.Enumerator enumerator = ((Kingdom)Hero.MainHero.MapFaction).Clans.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.IsUnderMercenaryService)
						{
							num++;
						}
					}
				}
				return num < 3;
			}
			return false;
		}

		public bool conversation_player_want_to_fire_mercenary_there_is_debt_on_condition()
		{
			MBTextManager.SetTextVariable("GOLD_AMOUNT", (int)Hero.OneToOneConversationHero.Clan.Influence * Hero.OneToOneConversationHero.Clan.MercenaryAwardMultiplier);
			return Hero.OneToOneConversationHero.Clan.Influence >= 1f;
		}

		public bool conversation_player_want_to_fire_mercenary_no_debt_on_condition()
		{
			return Hero.OneToOneConversationHero.Clan.Influence < 1f;
		}

		private void conversation_player_want_to_fire_mercenary_with_paying_debt_on_consequence()
		{
			int num = MathF.Max(0, (int)Hero.OneToOneConversationHero.Clan.Influence) * Hero.OneToOneConversationHero.Clan.MercenaryAwardMultiplier;
			GiveGoldAction.ApplyBetweenCharacters(Hero.MainHero, Hero.OneToOneConversationHero.Clan.Leader, num, false);
			ChangeClanInfluenceAction.Apply(Hero.OneToOneConversationHero.Clan, -Hero.OneToOneConversationHero.Clan.Influence);
		}

		private void conversation_player_want_to_fire_mercenary_without_paying_debt_on_consequence()
		{
			int num = MathF.Max(0, (int)Hero.OneToOneConversationHero.Clan.Influence) * Hero.OneToOneConversationHero.Clan.MercenaryAwardMultiplier;
			ChangeRelationAction.ApplyPlayerRelation(Hero.OneToOneConversationHero.Clan.Leader, -(int)(3f + MathF.Sqrt((float)((int)((float)num / 100f)))), true, true);
			ChangeKingdomAction.ApplyByLeaveKingdomAsMercenary(Hero.OneToOneConversationHero.Clan, true);
			ChangeClanInfluenceAction.Apply(Hero.OneToOneConversationHero.Clan, -Hero.OneToOneConversationHero.Clan.Influence);
		}

		private void conversation_player_want_to_fire_mercenary_on_consequence()
		{
			ChangeRelationAction.ApplyPlayerRelation(Hero.OneToOneConversationHero.Clan.Leader, -2, true, true);
			ChangeKingdomAction.ApplyByLeaveKingdomAsMercenary(Hero.OneToOneConversationHero.Clan, true);
		}

		public bool conversation_player_want_to_fire_mercenary_with_paying_debt_on_condition()
		{
			int num = MathF.Max(0, (int)Hero.OneToOneConversationHero.Clan.Influence) * Hero.OneToOneConversationHero.Clan.MercenaryAwardMultiplier;
			return Hero.MainHero.Gold >= num;
		}

		public bool conversation_mercenary_response_on_condition_reject()
		{
			return Hero.OneToOneConversationHero.Clan.Leader.GetRelation(Hero.MainHero) <= -10;
		}

		public bool conversation_mercenary_response_on_condition_reject_because_of_financial_reasons()
		{
			int gold = Hero.MainHero.Gold;
			int debtToKingdom = Clan.PlayerClan.DebtToKingdom;
			int mercenaryAwardFactorToJoinKingdom = Campaign.Current.Models.MinorFactionsModel.GetMercenaryAwardFactorToJoinKingdom(Hero.OneToOneConversationHero.Clan, (Kingdom)Hero.MainHero.MapFaction, true);
			return gold < 20 * mercenaryAwardFactorToJoinKingdom || debtToKingdom > 1000;
		}

		public bool conversation_mercenary_response_not_leader_on_condition()
		{
			StringHelpers.SetCharacterProperties("LEADER", Hero.OneToOneConversationHero.Clan.Leader.CharacterObject, null, false);
			return Hero.OneToOneConversationHero.Clan.Leader != Hero.OneToOneConversationHero;
		}

		public bool conversation_mercenary_response_on_condition()
		{
			if (Hero.OneToOneConversationHero.Clan.Leader != Hero.OneToOneConversationHero)
			{
				return false;
			}
			int num = Campaign.Current.Models.MinorFactionsModel.GetMercenaryAwardFactorToJoinKingdom(Hero.OneToOneConversationHero.Clan, (Kingdom)Hero.MainHero.MapFaction, true);
			if (Hero.OneToOneConversationHero.Clan.IsUnderMercenaryService)
			{
				num *= 3;
				num /= 2;
			}
			MBTextManager.SetTextVariable("GOLD_AMOUNT", num);
			return Hero.OneToOneConversationHero.Clan.Leader.GetRelation(Hero.MainHero) > -10;
		}

		private bool conversation_mercenary_response_accept_reject_on_clickable_condition(out TextObject explanation)
		{
			int mercenaryAwardFactorToJoinKingdom = Campaign.Current.Models.MinorFactionsModel.GetMercenaryAwardFactorToJoinKingdom(Hero.OneToOneConversationHero.Clan, (Kingdom)Hero.MainHero.MapFaction, true);
			explanation = new TextObject("{=r3GvpY5n}Mercenaries receive influence like vassals for fighting, but it is exchanged at the end of each day for denars at the rate of {GOLD_AMOUNT}{GOLD_ICON} per influence point.", null);
			explanation.SetTextVariable("GOLD_AMOUNT", mercenaryAwardFactorToJoinKingdom);
			explanation.SetTextVariable("GOLD_ICON", "{=!}<img src=\"General\\Icons\\Coin@2x\" extend=\"8\">");
			return true;
		}

		private void conversation_mercenary_response_accept_on_consqequence()
		{
			int num = Campaign.Current.Models.MinorFactionsModel.GetMercenaryAwardFactorToJoinKingdom(Hero.OneToOneConversationHero.Clan, (Kingdom)Hero.MainHero.MapFaction, true);
			if (Hero.OneToOneConversationHero.Clan.IsUnderMercenaryService)
			{
				num *= 3;
				num /= 2;
				if (Hero.OneToOneConversationHero.Clan.MapFaction.IsKingdomFaction)
				{
					ChangeKingdomAction.ApplyByLeaveKingdomAsMercenary(Hero.OneToOneConversationHero.Clan, true);
				}
			}
			ChangeKingdomAction.ApplyByJoinFactionAsMercenary(Hero.OneToOneConversationHero.Clan, (Kingdom)Hero.MainHero.MapFaction, num, true);
			if (Hero.OneToOneConversationHero.PartyBelongedTo != null && Hero.OneToOneConversationHero.PartyBelongedTo.CurrentSettlement == null)
			{
				Hero.OneToOneConversationHero.PartyBelongedTo.Ai.SetMoveModeHold();
			}
		}

		public bool conversation_player_want_to_join_faction_as_mercenary_or_vassal_on_condition()
		{
			if (Hero.OneToOneConversationHero.MapFaction != null && !Hero.OneToOneConversationHero.IsPrisoner && Hero.MainHero.MapFaction != Hero.OneToOneConversationHero.MapFaction && Hero.OneToOneConversationHero.MapFaction.IsKingdomFaction && (!Hero.MainHero.MapFaction.IsKingdomFaction || Clan.PlayerClan.IsUnderMercenaryService) && !FactionManager.IsAtWarAgainstFaction(Hero.MainHero.MapFaction, Hero.OneToOneConversationHero.MapFaction))
			{
				if (Hero.OneToOneConversationHero.MapFaction.Leader == Hero.OneToOneConversationHero)
				{
					MBTextManager.SetTextVariable("FACTION_SERVICE_TERM", "{=ZOkUKXV2}your service", false);
				}
				else
				{
					StringHelpers.SetCharacterProperties("RULER", Hero.OneToOneConversationHero.MapFaction.Leader.CharacterObject, null, false);
					TextObject textObject = new TextObject("{=tDnfaXKs}the service of {.^}{RULER_TITLE} {RULER.NAME}", null);
					textObject.SetTextVariable("RULER_TITLE", HeroHelper.GetTitleInIndefiniteCase(Hero.OneToOneConversationHero.MapFaction.Leader));
					MBTextManager.SetTextVariable("FACTION_SERVICE_TERM", textObject, false);
				}
				return true;
			}
			return false;
		}

		public bool conversation_player_want_to_end_service_as_mercenary_on_condition()
		{
			if (Hero.OneToOneConversationHero.MapFaction == null || Hero.OneToOneConversationHero.Clan == null)
			{
				return false;
			}
			if (Hero.OneToOneConversationHero.MapFaction.IsKingdomFaction)
			{
				MBTextManager.SetTextVariable("SERVED_FACTION", FactionHelper.GetFormalNameForFactionCulture(Hero.OneToOneConversationHero.Clan.Kingdom.Culture), false);
			}
			return !Hero.OneToOneConversationHero.IsPrisoner && Hero.MainHero.MapFaction == Hero.OneToOneConversationHero.MapFaction && Hero.OneToOneConversationHero.Clan != Hero.MainHero.Clan && Hero.MainHero.Clan.IsUnderMercenaryService;
		}

		public void conversation_player_want_to_end_service_as_mercenary_on_consequence()
		{
			ChangeClanInfluenceAction.Apply(Clan.PlayerClan, -Hero.MainHero.Clan.Influence);
			ChangeKingdomAction.ApplyByLeaveKingdomAsMercenaryWithKingDecision(Hero.MainHero.Clan, true);
		}

		public bool conversation_player_ask_to_claim_land_on_condition()
		{
			if (Hero.MainHero.MapFaction == Hero.OneToOneConversationHero.MapFaction && !Hero.OneToOneConversationHero.IsPrisoner)
			{
				foreach (Settlement settlement in Settlement.All)
				{
					if (settlement.CanBeClaimed == 1 && settlement.ClaimedBy != Hero.MainHero)
					{
						MBTextManager.SetTextVariable("SETTLEMENT", settlement.Name, false);
						return true;
					}
				}
				return false;
			}
			return false;
		}

		public static bool player_ask_to_join_players_party_on_condition()
		{
			return Hero.OneToOneConversationHero.PartyBelongedTo == null && !Hero.OneToOneConversationHero.IsPrisoner && Hero.OneToOneConversationHero.PartyBelongedToAsPrisoner == null && Hero.OneToOneConversationHero.Clan == Clan.PlayerClan;
		}

		private bool player_ask_to_join_players_army_on_condition()
		{
			if (Hero.OneToOneConversationHero != null && Hero.OneToOneConversationHero.PartyBelongedTo != null && Hero.OneToOneConversationHero.PartyBelongedTo.IsLordParty && Hero.OneToOneConversationHero.PartyBelongedTo.LeaderHero == Hero.OneToOneConversationHero && Hero.OneToOneConversationHero.PartyBelongedTo.MapEvent == null && Hero.OneToOneConversationHero.PartyBelongedTo.SiegeEvent == null && Hero.OneToOneConversationHero.PartyBelongedTo.Army == null && MobileParty.MainParty.Army != null && MobileParty.MainParty.Army.LeaderParty == MobileParty.MainParty && Hero.OneToOneConversationHero.MapFaction == Hero.MainHero.MapFaction && Hero.OneToOneConversationHero.MapFaction.Leader != Hero.OneToOneConversationHero)
			{
				MBTextManager.SetTextVariable("INFLUENCE_COST", Campaign.Current.Models.ArmyManagementCalculationModel.CalculatePartyInfluenceCost(MobileParty.MainParty, Hero.OneToOneConversationHero.PartyBelongedTo));
				MBTextManager.SetTextVariable("INFLUENCE_ICON", "{=!}<img src=\"General\\Icons\\Influence@2x\" extend=\"7\">", false);
				return true;
			}
			return false;
		}

		private void player_ask_to_join_players_army_on_consequence()
		{
			ChangeClanInfluenceAction.Apply(Clan.PlayerClan, (float)(-(float)Campaign.Current.Models.ArmyManagementCalculationModel.CalculatePartyInfluenceCost(MobileParty.MainParty, Hero.OneToOneConversationHero.PartyBelongedTo)));
		}

		private bool player_ask_to_join_players_army_on_clickable_condition(out TextObject explanation)
		{
			int num = Campaign.Current.Models.ArmyManagementCalculationModel.CalculatePartyInfluenceCost(MobileParty.MainParty, Hero.OneToOneConversationHero.PartyBelongedTo);
			float partySizeScore = Campaign.Current.Models.ArmyManagementCalculationModel.GetPartySizeScore(Hero.OneToOneConversationHero.PartyBelongedTo);
			if (Hero.MainHero.Clan.Influence >= (float)num && partySizeScore > 0.4f)
			{
				explanation = TextObject.Empty;
				return true;
			}
			if (partySizeScore <= 0.4f)
			{
				explanation = new TextObject("{=SVJlOYCB}Party has less men than 40% of it's party size limit.", null);
				return false;
			}
			explanation = new TextObject("{=KX7xtOI6} Your clan does not have enough influence to get them to do this!", null);
			return false;
		}

		public static void player_ask_to_join_players_party_on_consequence()
		{
			Campaign.Current.ConversationManager.ConversationEndOneShot += delegate
			{
				AddHeroToPartyAction.Apply(Hero.OneToOneConversationHero, MobileParty.MainParty, true);
			};
		}

		private bool conversation_player_wants_to_make_peace_on_condition()
		{
			return FactionManager.IsAtWarAgainstFaction(Hero.MainHero.MapFaction, Hero.OneToOneConversationHero.MapFaction);
		}

		private bool conversation_player_wants_to_make_peace_answer_on_condition()
		{
			TextObject textObject = TextObject.Empty;
			this._willDoPeaceBarter = false;
			if (Hero.OneToOneConversationHero.Clan.IsRebelClan && Hero.OneToOneConversationHero.Clan.IsAtWarWith(Hero.MainHero.MapFaction))
			{
				textObject = new TextObject("{=lH3cgbVX}We will not sign a peace until we are recognized by all the kingdoms of Calradia.", null);
			}
			else if (Hero.OneToOneConversationHero.Clan.IsUnderMercenaryService)
			{
				textObject = new TextObject("{=bdetTQa6}We are only mercenaries serving under {EMPLOYER_FACTION_INFORMAL_NAME}. We cannot negotiate peace on behalf of our employers.", null);
				textObject.SetTextVariable("EMPLOYER_FACTION_INFORMAL_NAME", Hero.OneToOneConversationHero.Clan.Kingdom.InformalName);
			}
			else if (Hero.OneToOneConversationHero.Clan.IsMinorFaction && Hero.OneToOneConversationHero.MapFaction.GetStanceWith(Hero.MainHero.MapFaction).IsAtConstantWar)
			{
				textObject = new TextObject("{=VWoHoUin}There will be no peace between us and the {ENEMY_INFORMAL_NAME}, any more than the wolf makes peace with the sheep.", null);
				textObject.SetTextVariable("ENEMY_INFORMAL_NAME", Hero.MainHero.MapFaction.InformalName);
			}
			else if (Hero.OneToOneConversationHero.Clan.Kingdom != null && Hero.OneToOneConversationHero.MapFaction.IsAtWarWith(Hero.MainHero.MapFaction) && Hero.MainHero.Clan.Kingdom != null)
			{
				if (Hero.OneToOneConversationHero.Clan.Kingdom.Leader == Hero.OneToOneConversationHero)
				{
					textObject = new TextObject("{=efTah9rk}I do not have the authority to make peace on behalf of the {ENEMY_INFORMAL_NAME}. Our council should decide whether to offer peace, and what the terms will be.", null);
				}
				else
				{
					textObject = new TextObject("{=JY717hPW}I do not have the authority to make peace on behalf of the {ENEMY_INFORMAL_NAME}. {ENEMY_RULER.NAME} and {?ENEMY_RULER.GENDER}her{?}his{\\?} council should decide whether to offer peace, and what the terms will be.", null);
					StringHelpers.SetCharacterProperties("ENEMY_RULER", Hero.OneToOneConversationHero.Clan.Kingdom.Leader.CharacterObject, null, false);
				}
				textObject.SetTextVariable("ENEMY_INFORMAL_NAME", Hero.OneToOneConversationHero.Clan.Kingdom.InformalName);
			}
			else
			{
				this._willDoPeaceBarter = true;
			}
			MBTextManager.SetTextVariable("LORD_PEACE_OFFER_ANSWER", textObject, false);
			return true;
		}

		private void conversation_player_wants_to_make_peace_on_consequence()
		{
			if (this._willDoPeaceBarter)
			{
				BarterManager instance = BarterManager.Instance;
				Hero mainHero = Hero.MainHero;
				Hero oneToOneConversationHero = Hero.OneToOneConversationHero;
				PartyBase mainParty = PartyBase.MainParty;
				MobileParty partyBelongedTo = Hero.OneToOneConversationHero.PartyBelongedTo;
				instance.StartBarterOffer(mainHero, oneToOneConversationHero, mainParty, (partyBelongedTo != null) ? partyBelongedTo.Party : null, null, new BarterManager.BarterContextInitializer(BarterManager.Instance.InitializeMakePeaceBarterContext), 0, false, new Barterable[]
				{
					new PeaceBarterable(Hero.OneToOneConversationHero, Clan.PlayerClan.MapFaction, Hero.OneToOneConversationHero.MapFaction, CampaignTime.Years(1f))
				});
			}
			this._willDoPeaceBarter = false;
		}

		public static bool conversation_player_ask_to_claim_land_answer_on_condition()
		{
			if (Hero.MainHero.MapFaction == Hero.OneToOneConversationHero.MapFaction)
			{
				int num = 1;
				foreach (Settlement settlement in Settlement.All)
				{
					if (settlement.OwnerClan.Leader == Hero.MainHero)
					{
						num += (settlement.IsTown ? 3 : 1);
					}
				}
				foreach (Settlement settlement2 in Settlement.All)
				{
					if (settlement2.CanBeClaimed == 1)
					{
						MBTextManager.SetTextVariable("VALUE", settlement2.ClaimValue * (float)num);
						return true;
					}
				}
				return false;
			}
			return false;
		}

		public bool conversation_player_ask_to_claim_land_answer_ok_on_condition()
		{
			if (Hero.MainHero.MapFaction == Hero.OneToOneConversationHero.MapFaction)
			{
				int num = 1;
				foreach (Settlement settlement in Settlement.All)
				{
					if (settlement.OwnerClan.Leader == Hero.MainHero)
					{
						num += (settlement.IsTown ? 3 : 1);
					}
				}
				int num2 = 0;
				foreach (Settlement settlement2 in Settlement.All)
				{
					if (settlement2.CanBeClaimed == 1)
					{
						MBTextManager.SetTextVariable("VALUE", (int)(settlement2.ClaimValue * (float)num));
						num2 = (int)(settlement2.ClaimValue * (float)num);
					}
				}
				if (Hero.MainHero.Clan.Influence > (float)num2)
				{
					return true;
				}
			}
			return false;
		}

		public void conversation_player_ask_to_claim_land_answer_on_consequence()
		{
			if (Hero.MainHero.MapFaction == Hero.OneToOneConversationHero.MapFaction)
			{
				int num = 1;
				foreach (Settlement settlement in Settlement.All)
				{
					if (settlement.OwnerClan.Leader == Hero.MainHero)
					{
						num += (settlement.IsTown ? 3 : 1);
					}
				}
				foreach (Settlement settlement2 in Settlement.All)
				{
					if (settlement2.CanBeClaimed == 1)
					{
						settlement2.ClaimedBy = Hero.MainHero;
						settlement2.ClaimValue = (float)((int)(settlement2.ClaimValue / (float)num));
						ChangeClanInfluenceAction.Apply(Clan.PlayerClan, -settlement2.ClaimValue);
					}
				}
			}
		}

		public void conversation_mercenary_player_accepts_lord_answer_on_consequence()
		{
			int mercenaryAwardFactor = this.GetMercenaryAwardFactor();
			ChangeKingdomAction.ApplyByJoinFactionAsMercenary(Clan.PlayerClan, Hero.OneToOneConversationHero.Clan.Kingdom, mercenaryAwardFactor, true);
			GainKingdomInfluenceAction.ApplyForJoiningFaction(Hero.MainHero, 5f);
		}

		public bool conversation_player_makes_faction_order()
		{
			MobileParty partyBelongedTo = Hero.OneToOneConversationHero.PartyBelongedTo;
			return partyBelongedTo != null && partyBelongedTo.MapFaction == MobileParty.MainParty.MapFaction && (partyBelongedTo.Army == null || partyBelongedTo.Army.LeaderParty == partyBelongedTo) && !Hero.OneToOneConversationHero.IsPlayerCompanion;
		}

		public bool conversation_player_wants_to_do_operation_on_condition()
		{
			HeroHelper.SetPlayerSalutation();
			return (Hero.OneToOneConversationHero == null || Hero.OneToOneConversationHero.IsPlayerCompanion) && Settlement.CurrentSettlement != null && Settlement.CurrentSettlement.IsTown;
		}

		public bool conversation_set_player_salutation()
		{
			HeroHelper.SetPlayerSalutation();
			return true;
		}

		public bool conversation_player_plans_never_mind_on_condition()
		{
			return true;
		}

		public bool conversation_player_ask_lords_to_follow_him_on_condition()
		{
			MobileParty partyBelongedTo = Hero.OneToOneConversationHero.PartyBelongedTo;
			if (partyBelongedTo != null && (partyBelongedTo.IsLordParty || partyBelongedTo.IsMilitia) && !partyBelongedTo.IsDisbanding && partyBelongedTo.MapFaction == MobileParty.MainParty.MapFaction && partyBelongedTo.CurrentSettlement == null && partyBelongedTo.Army == null && partyBelongedTo.DefaultBehavior != AiBehavior.EscortParty && partyBelongedTo.TargetParty != MobileParty.MainParty && (partyBelongedTo.Army == null || partyBelongedTo.Army.LeaderParty != partyBelongedTo))
			{
				LordConversationsCampaignBehavior.Number number = new LordConversationsCampaignBehavior.Number(5);
				float num = (Hero.OneToOneConversationHero.IsFriend(Hero.MainHero) ? 1f : (Hero.OneToOneConversationHero.IsEnemy(Hero.MainHero) ? 3f : 2f));
				num *= ((Hero.OneToOneConversationHero.PartyBelongedTo.Army != null) ? 3f : 1f);
				MBTextManager.SetTextVariable("AMOUNT", number.Value * (int)num);
				return Hero.MainHero.Clan.Influence >= (float)number.Value * num;
			}
			return false;
		}

		public bool conversation_player_ask_lords_to_follow_him_already_at_army_on_condition()
		{
			MobileParty partyBelongedTo = Hero.OneToOneConversationHero.PartyBelongedTo;
			if (partyBelongedTo != null && partyBelongedTo.MapFaction == MobileParty.MainParty.MapFaction && partyBelongedTo.Army != null && partyBelongedTo.Army.LeaderParty != partyBelongedTo)
			{
				MBTextManager.SetTextVariable("ESCORTING_PARTY", partyBelongedTo.Army.LeaderParty);
				return true;
			}
			return false;
		}

		public bool conversation_player_ask_lords_to_create_army_on_condition()
		{
			return false;
		}

		public void conversation_player_ask_lords_to_follow_him_already_at_army_on_consequence()
		{
			Army army = new Army((Kingdom)MobileParty.MainParty.MapFaction, MobileParty.MainParty, Army.ArmyTypes.Patrolling);
			army.AIBehavior = Army.AIBehaviorFlags.Gathering;
			army.AiBehaviorObject = MobileParty.MainParty;
		}

		public bool conversation_how_many_days_to_escort_condition()
		{
			LordConversationsCampaignBehavior.Number number = ConversationSentence.CurrentProcessedRepeatObject as LordConversationsCampaignBehavior.Number;
			if (number != null)
			{
				float num = (Hero.OneToOneConversationHero.IsFriend(Hero.MainHero) ? 1f : (Hero.OneToOneConversationHero.IsEnemy(Hero.MainHero) ? 3f : 2f));
				num *= ((Hero.OneToOneConversationHero.PartyBelongedTo.Army != null) ? 3f : 1f);
				MBTextManager.SetTextVariable("NUMBER_OF_DAYS", number.Value);
				MBTextManager.SetTextVariable("AMOUNT", number.Value * (int)num);
				if (Hero.MainHero.Clan.Influence < (float)number.Value * num)
				{
					return false;
				}
			}
			return true;
		}

		public void conversation_how_many_days_to_escort_on_consqeuence()
		{
			MobileParty partyBelongedTo = Hero.OneToOneConversationHero.PartyBelongedTo;
			if (partyBelongedTo != null)
			{
				LordConversationsCampaignBehavior.Number number = new LordConversationsCampaignBehavior.Number(10);
				partyBelongedTo.Ai.SetInitiative(0f, 0f, (float)(24 * number.Value));
				partyBelongedTo.Ai.SetMoveEscortParty(MobileParty.MainParty);
				PlayerEncounter.LeaveEncounter = true;
			}
		}

		public void conversation_player_ask_lords_to_follow_him_on_consequence()
		{
		}

		private bool conversation_player_ask_prisoners_on_condition()
		{
			if (Hero.OneToOneConversationHero != null && Hero.OneToOneConversationHero.PartyBelongedTo != null && Hero.OneToOneConversationHero.PartyBelongedTo.PrisonRoster.TotalHeroes > 0 && Hero.OneToOneConversationHero.PartyBelongedTo != MobileParty.MainParty)
			{
				GameStateManager gameStateManager = GameStateManager.Current;
				return ((gameStateManager != null) ? gameStateManager.ActiveState : null) is MapState;
			}
			return false;
		}

		private void conversation_player_ask_prisoners_on_consequence()
		{
			List<Hero> list = new List<Hero>();
			foreach (TroopRosterElement troopRosterElement in Hero.OneToOneConversationHero.PartyBelongedTo.PrisonRoster.GetTroopRoster())
			{
				if (troopRosterElement.Character.IsHero)
				{
					list.Add(troopRosterElement.Character.HeroObject);
				}
			}
			ConversationSentence.SetObjectsToRepeatOver(list, 5);
		}

		private bool conversation_player_ask_prisoners_forbidden_on_condition()
		{
			return FactionManager.IsAtWarAgainstFaction(Hero.OneToOneConversationHero.MapFaction, Hero.MainHero.MapFaction) || (FactionManager.IsNeutralWithFaction(Hero.OneToOneConversationHero.MapFaction, Hero.MainHero.MapFaction) && Hero.OneToOneConversationHero.GetRelationWithPlayer() < 0f);
		}

		private bool conversation_player_ask_prisoners_list_on_condition()
		{
			Hero hero = ConversationSentence.CurrentProcessedRepeatObject as Hero;
			ConversationSentence.SelectedRepeatLine.SetTextVariable("PRISONER_NAME", hero.Name);
			return true;
		}

		private void lord_talk_to_selected_prisoner_on_consequence()
		{
			CampaignMapConversation.OpenConversation(new ConversationCharacterData(CharacterObject.PlayerCharacter, null, false, false, false, false, false, false), new ConversationCharacterData(this._selectedPrisoner.CharacterObject, this._selectedPrisoner.PartyBelongedToAsPrisoner, false, true, false, true, false, false));
		}

		private void conversation_player_ask_prisoners_list_on_consequence()
		{
			this._selectedPrisoner = ConversationSentence.SelectedRepeatObject as Hero;
		}

		public bool conversation_player_has_question_on_condition()
		{
			return Game.Current.IsDevelopmentMode && Hero.OneToOneConversationHero != null && !FactionManager.IsAtWarAgainstFaction(Hero.OneToOneConversationHero.MapFaction, Hero.MainHero.MapFaction) && !Hero.OneToOneConversationHero.IsWanderer;
		}

		public bool conversation_hero_main_options_discussions()
		{
			return Hero.OneToOneConversationHero != null && !Hero.OneToOneConversationHero.IsNotable && !Hero.OneToOneConversationHero.IsWanderer && Hero.OneToOneConversationHero.Occupation != Occupation.Special && !HeroHelper.WillLordAttack() && !this.PlayerIsBesieged() && !this.PlayerIsBesieging();
		}

		public bool conversation_lord_talk_politics_during_siege_parley_on_condition()
		{
			return (Hero.OneToOneConversationHero == null || !this.conversation_wanderer_on_condition()) && (((this.PlayerIsBesieged() && (MobileParty.MainParty.Army == null || MobileParty.MainParty.Army.LeaderParty == MobileParty.MainParty)) || this.PlayerIsBesieging()) && Hero.OneToOneConversationHero.MapFaction.IsAtWarWith(Hero.MainHero.MapFaction));
		}

		public bool conversation_player_is_asking_pardon_on_condition()
		{
			if (FactionManager.IsAtWarAgainstFaction(Hero.OneToOneConversationHero.MapFaction, Hero.MainHero.MapFaction) && !Hero.MainHero.MapFaction.IsKingdomFaction)
			{
				MBTextManager.SetTextVariable("FACTION_NAME", Hero.OneToOneConversationHero.MapFaction.Name, false);
				return true;
			}
			return false;
		}

		public bool conversation_player_is_asking_peace_on_condition()
		{
			if (FactionManager.IsAtWarAgainstFaction(Hero.OneToOneConversationHero.MapFaction, Hero.MainHero.MapFaction) && Hero.MainHero.MapFaction.IsKingdomFaction)
			{
				MBTextManager.SetTextVariable("FACTION_NAME", Hero.OneToOneConversationHero.MapFaction.Name, false);
				return true;
			}
			return false;
		}

		public bool conversation_player_is_leaving_neutral_or_friendly_on_condition()
		{
			return Hero.OneToOneConversationHero != null && !FactionManager.IsAtWarAgainstFaction(Hero.OneToOneConversationHero.MapFaction, Hero.MainHero.MapFaction);
		}

		public bool conversation_player_is_leaving_enemy_on_condition()
		{
			return Hero.OneToOneConversationHero != null && !Hero.OneToOneConversationHero.IsPrisoner && FactionManager.IsAtWarAgainstFaction(Hero.OneToOneConversationHero.MapFaction, Hero.MainHero.MapFaction);
		}

		public bool conversation_player_is_leaving_enemy_prisoner_on_condition()
		{
			return Hero.OneToOneConversationHero != null && Hero.OneToOneConversationHero.IsPrisoner && FactionManager.IsAtWarAgainstFaction(Hero.OneToOneConversationHero.MapFaction, Hero.MainHero.MapFaction);
		}

		public bool conversation_cheat_other_lords_on_condition()
		{
			string text = Hero.OneToOneConversationHero.MapFaction.Leader.Name + ": " + Hero.OneToOneConversationHero.GetRelation(Hero.OneToOneConversationHero.MapFaction.Leader);
			foreach (Hero hero in (from x in Hero.AllAliveHeroes.Where((Hero x) => x.MapFaction == Hero.OneToOneConversationHero.MapFaction && x.IsLord && (x.Clan.IsMapFaction || x.Clan.Leader == x) && x != x.MapFaction.Leader && x != Hero.OneToOneConversationHero).ToList<Hero>()
				orderby x.GetRelation(Hero.OneToOneConversationHero) descending
				select x).ToList<Hero>())
			{
				text = string.Concat(new object[]
				{
					text,
					", ",
					hero.Name,
					Hero.OneToOneConversationHero.GetRelation(hero)
				});
			}
			MBTextManager.SetTextVariable("OTHER_LORDS", text, false);
			return true;
		}

		public bool conversation_player_dont_attack_we_surrender_on_condition()
		{
			return Settlement.CurrentSettlement == null || Settlement.CurrentSettlement.SiegeEvent == null;
		}

		public bool conversation_cheat_faction_enmities_on_condition()
		{
			string text = "{=!}Okay...";
			List<Hero> list = Hero.AllAliveHeroes.Where((Hero x) => x.MapFaction == Hero.OneToOneConversationHero.MapFaction && x.IsLord).ToList<Hero>();
			foreach (Hero hero in list)
			{
				foreach (Hero hero2 in list)
				{
					if (hero.GetRelation(hero2) <= -20 && hero.Clan.Renown <= hero2.Clan.Renown)
					{
						TextObject reasonForEnmity = this.GetReasonForEnmity(hero, hero2, Hero.OneToOneConversationHero);
						MBTextManager.SetTextVariable("LORD_1_NAME", hero.Name, false);
						MBTextManager.SetTextVariable("LORD_2_NAME", hero2.Name, false);
						MBTextManager.SetTextVariable("HISTORIC_COMMENT", GameTexts.FindText(reasonForEnmity.ToString(), null), false);
						text += new TextObject("{=!}{LORD_1_NAME} dislikes {LORD_2_NAME}. {HISTORIC_COMMENT}|", null).ToString();
					}
				}
			}
			MBTextManager.SetTextVariable("ENMITY_INFO", text, false);
			return true;
		}

		public TextObject GetReasonForEnmity(Hero lord1, Hero lord2, Hero talkTroop)
		{
			foreach (LogEntry logEntry in Campaign.Current.LogEntryHistory.GameActionLogs)
			{
				if (logEntry.AsReasonForEnmity(lord1, lord2) > 0)
				{
					return logEntry.GetHistoricComment(talkTroop);
				}
			}
			return new TextObject("{=GbOj39KC}I'm not sure why", null);
		}

		public bool conversation_cheat_reputation_on_condition()
		{
			MBTextManager.SetTextVariable("CONVERSATION_CHARACTER_REPUTATION", CharacterHelper.GetReputationDescription(CharacterObject.OneToOneConversationCharacter), false);
			foreach (TraitObject traitObject in DefaultTraits.Personality)
			{
				int traitLevel = Hero.OneToOneConversationHero.GetTraitLevel(traitObject);
				if (traitLevel != 0)
				{
					MBTextManager.SetTextVariable("PERSONALITY_DESCRIPTION", traitObject.Description, false);
					if (traitLevel < 0)
					{
						MBTextManager.SetTextVariable("SIGN", "{=!}Neg", false);
					}
					if (traitLevel > 0)
					{
						MBTextManager.SetTextVariable("SIGN", "{=!}Pos", false);
					}
				}
			}
			MBTextManager.SetTextVariable("RELATION_WITH_CHARACTER", Hero.OneToOneConversationHero.GetRelationWithPlayer());
			string text = "{=!}{CONVERSATION_CHARACTER_REPUTATION} {PERSONALITY_DESCRIPTION}: {SIGN}Rel to you: {RELATION_WITH_CHARACTER}";
			MBTextManager.SetTextVariable("REPUTATION", text, false);
			return true;
		}

		public bool conversation_lord_leave_on_condition()
		{
			StringHelpers.SetCharacterProperties("PLAYER", Hero.MainHero.CharacterObject, null, false);
			return true;
		}

		public void conversation_lord_leave_on_consequence()
		{
			if (PlayerEncounter.Current != null && Campaign.Current.ConversationManager.ConversationParty == PlayerEncounter.EncounteredMobileParty)
			{
				if (PlayerEncounter.EncounteredBattle == null)
				{
					Settlement encounterSettlement = PlayerEncounter.EncounterSettlement;
					if (((encounterSettlement != null) ? encounterSettlement.Party.MapEvent : null) == null)
					{
						Settlement encounterSettlement2 = PlayerEncounter.EncounterSettlement;
						if (((encounterSettlement2 != null) ? encounterSettlement2.Party.SiegeEvent : null) == null && (PlayerEncounter.EncounterSettlement != null || Settlement.CurrentSettlement != null))
						{
							return;
						}
					}
				}
				PlayerEncounter.LeaveEncounter = true;
			}
		}

		public bool conversation_capture_defeated_lord_on_condition()
		{
			if (Campaign.Current.CurrentConversationContext == ConversationContext.CapturedLord)
			{
				DialogHelper.SetDialogString("SURRENDER_OFFER", "str_surrender_offer");
				return true;
			}
			return false;
		}

		public bool conversation_liberate_known_hero_on_condition()
		{
			return Campaign.Current.CurrentConversationContext == ConversationContext.FreedHero && !Campaign.Current.ConversationManager.CurrentConversationIsFirst && CharacterObject.OneToOneConversationCharacter.Occupation == Occupation.Lord;
		}

		public bool conversation_liberate_unmet_hero_on_condition()
		{
			return Campaign.Current.CurrentConversationContext == ConversationContext.FreedHero && Campaign.Current.ConversationManager.CurrentConversationIsFirst && CharacterObject.OneToOneConversationCharacter.Occupation == Occupation.Lord;
		}

		public bool conversation_reprisoner_hero_decision_on_condition()
		{
			bool flag = Hero.OneToOneConversationHero.MapFaction != null && Hero.OneToOneConversationHero.MapFaction.IsAtWarWith(Hero.MainHero.MapFaction);
			if (flag)
			{
				MBTextManager.SetTextVariable("REPRISONER_DECISION", Campaign.Current.ConversationManager.FindMatchingTextOrNull("str_reprisoner_decision", CharacterObject.OneToOneConversationCharacter), false);
			}
			return flag;
		}

		public void conversation_player_liberates_prisoner_on_consequence()
		{
			ChangeRelationAction.ApplyPlayerRelation(CharacterObject.OneToOneConversationCharacter.HeroObject, 10, true, true);
			if (Hero.OneToOneConversationHero.IsPrisoner)
			{
				EndCaptivityAction.ApplyByReleasedAfterBattle(Hero.OneToOneConversationHero);
			}
		}

		public void conversation_player_fails_to_release_prisoner_on_consequence()
		{
			if (Hero.OneToOneConversationHero.IsPrisoner)
			{
				EndCaptivityAction.ApplyByReleasedByChoice(Hero.OneToOneConversationHero, null);
				TakePrisonerAction.Apply(PartyBase.MainParty, Hero.OneToOneConversationHero);
			}
		}

		public bool conversation_ally_thanks_meet_after_helping_in_battle_on_condition()
		{
			if (MapEvent.PlayerMapEvent != null && Hero.OneToOneConversationHero != null && !FactionManager.IsAtWarAgainstFaction(Hero.MainHero.MapFaction, Hero.OneToOneConversationHero.MapFaction) && MapEvent.PlayerMapEvent.WinningSide == PartyBase.MainParty.Side && !Hero.OneToOneConversationHero.HasMet)
			{
				if (MapEvent.PlayerMapEvent.InvolvedParties.Count((PartyBase t) => t.Side == PartyBase.MainParty.Side && t != PartyBase.MainParty) > 0)
				{
					int num = MBRandom.RandomInt(3);
					MBTextManager.SetTextVariable("MEETING_SENTENCE", GameTexts.FindText("str_ally_thanks_meet_after_helping_in_battle", num.ToString()), false);
					MBTextManager.SetTextVariable("GRATITUDE_SENTENCE", GameTexts.FindText("str_ally_thanks_after_helping_in_battle", num.ToString()), false);
					Hero.OneToOneConversationHero.SetHasMet();
					return true;
				}
			}
			return false;
		}

		public bool conversation_ally_thanks_after_helping_in_battle_on_condition()
		{
			if (MapEvent.PlayerMapEvent != null && Hero.OneToOneConversationHero != null && !FactionManager.IsAtWarAgainstFaction(Hero.MainHero.MapFaction, Hero.OneToOneConversationHero.MapFaction) && MapEvent.PlayerMapEvent.WinningSide == PartyBase.MainParty.Side && Hero.OneToOneConversationHero.HasMet)
			{
				if (MapEvent.PlayerMapEvent.InvolvedParties.Count((PartyBase t) => t.Side == PartyBase.MainParty.Side && t != PartyBase.MainParty) > 0)
				{
					MBTextManager.SetTextVariable("GREETING_SENTENCE", GameTexts.FindText("ally_thanks_after_helping_in_battle_has_met", MBRandom.RandomInt(0, 4).ToString()), false);
					return true;
				}
			}
			return false;
		}

		private void conversation_ally_thanks_meet_after_helping_in_battle_2_on_consequence()
		{
			int playerGainedRelationAmount = Campaign.Current.Models.BattleRewardModel.GetPlayerGainedRelationAmount(MapEvent.PlayerMapEvent, Hero.OneToOneConversationHero);
			ChangeRelationAction.ApplyPlayerRelation(Hero.OneToOneConversationHero, playerGainedRelationAmount, true, true);
			if (Hero.OneToOneConversationHero.IsPrisoner)
			{
				EndCaptivityAction.ApplyByReleasedAfterBattle(Hero.OneToOneConversationHero);
			}
		}

		public void conversation_talk_lord_defeat_to_lord_capture_on_consequence()
		{
			Campaign.Current.CurrentConversationContext = ConversationContext.Default;
			TakePrisonerAction.Apply(Campaign.Current.MainParty.Party, CharacterObject.OneToOneConversationCharacter.HeroObject);
		}

		public void conversation_talk_lord_defeat_to_lord_capture_and_kill_on_consequence()
		{
			KillCharacterAction.ApplyByExecution(Hero.OneToOneConversationHero, Hero.MainHero, true, false);
		}

		public bool conversation_talk_lord_release_noncombatant_on_condition()
		{
			return (Hero.OneToOneConversationHero.Clan == null || Hero.OneToOneConversationHero.Clan.IsMapFaction || Hero.OneToOneConversationHero.Clan.Leader != Hero.OneToOneConversationHero) && Hero.OneToOneConversationHero.IsNoncombatant;
		}

		public bool conversation_talk_lord_release_combatant_on_condition()
		{
			return (Hero.OneToOneConversationHero.Clan != null && !Hero.OneToOneConversationHero.Clan.IsMapFaction && Hero.OneToOneConversationHero.Clan.Leader == Hero.OneToOneConversationHero) || !Hero.OneToOneConversationHero.IsNoncombatant;
		}

		public bool conversation_player_ask_ruling_philosophy_on_condition()
		{
			TextObject textObject;
			if (Hero.OneToOneConversationHero.IsLord && !Hero.OneToOneConversationHero.MapFaction.IsMinorFaction && this.GetConversationHeroPoliticalPhilosophy(out textObject))
			{
				MBTextManager.SetTextVariable("RULING_PHILOSOPHY", textObject, false);
				return true;
			}
			return false;
		}

		public bool conversation_player_has_long_ruling_philosophy_on_condition()
		{
			TextObject textObject;
			TextObject textObject2;
			TextObject textObject3;
			if (Hero.OneToOneConversationHero.IsLord && this.GetConversationHeroPoliticalPhilosophy(out textObject) && this.GetConversationHeroPoliticalPhilosophy_2(out textObject2) && this.GetConversationHeroPoliticalPhilosophy_3(out textObject3))
			{
				MBTextManager.SetTextVariable("RULING_PHILOSOPHY", textObject, false);
				MBTextManager.SetTextVariable("RULING_PHILOSOPHY_2", textObject2, false);
				MBTextManager.SetTextVariable("RULING_PHILOSOPHY_3", textObject3, false);
				return true;
			}
			return false;
		}

		public static void conversation_talk_lord_defeat_to_lord_release_on_consequence()
		{
			if (Hero.OneToOneConversationHero.IsPrisoner)
			{
				EndCaptivityAction.ApplyByReleasedAfterBattle(Hero.OneToOneConversationHero);
			}
			else
			{
				MakeHeroFugitiveAction.Apply(Hero.OneToOneConversationHero);
			}
			ChangeRelationAction.ApplyPlayerRelation(CharacterObject.OneToOneConversationCharacter.HeroObject, 4, true, true);
			DialogHelper.SetDialogString("DEFEAT_LORD_ANSWER", "str_prisoner_released");
		}

		public void conversation_talk_lord_freed_to_lord_capture_on_consequence()
		{
			Campaign.Current.CurrentConversationContext = ConversationContext.Default;
			TakePrisonerAction.Apply(PartyBase.MainParty, Hero.OneToOneConversationHero);
		}

		public void conversation_talk_lord_freed_to_lord_release_on_consequence()
		{
			if (Hero.OneToOneConversationHero.IsPrisoner)
			{
				EndCaptivityAction.ApplyByReleasedByChoice(Hero.OneToOneConversationHero, Hero.MainHero);
			}
			ChangeRelationAction.ApplyPlayerRelation(CharacterObject.OneToOneConversationCharacter.HeroObject, 4, true, true);
			TraitLevelingHelper.OnLordFreed(Hero.OneToOneConversationHero);
		}

		public bool conversation_lord_request_mission_ask_on_condition()
		{
			if (Hero.MainHero.MapFaction != Clan.PlayerClan)
			{
				return false;
			}
			if (FactionManager.IsAtWarAgainstFaction(Clan.PlayerClan, Hero.OneToOneConversationHero.MapFaction))
			{
				return false;
			}
			if (Hero.OneToOneConversationHero.MapFaction.Leader == Hero.OneToOneConversationHero)
			{
				return false;
			}
			int num = 0;
			foreach (Kingdom kingdom in Kingdom.All)
			{
				if (FactionManager.IsAtWarAgainstFaction(Hero.OneToOneConversationHero.MapFaction, kingdom))
				{
					num++;
				}
			}
			if (num == 0)
			{
				return false;
			}
			Kingdom kingdom2 = (Kingdom)Hero.OneToOneConversationHero.MapFaction;
			if (kingdom2 == null)
			{
				return false;
			}
			if (kingdom2.LastMercenaryOfferTime.ElapsedDaysUntilNow <= 1f)
			{
				return false;
			}
			if (MBRandom.RandomFloat < 0.2f || kingdom2.LastMercenaryOfferTime.ElapsedDaysUntilNow > 7f)
			{
				int mercenaryWageAmount = Campaign.Current.KingdomManager.GetMercenaryWageAmount(Hero.MainHero);
				MBTextManager.SetTextVariable("OFFER_VALUE", mercenaryWageAmount);
				MBTextManager.SetTextVariable("FACTION_LEADER", Hero.OneToOneConversationHero.MapFaction.Leader.Name, false);
				MBTextManager.SetTextVariable("FACTION_NAME", Hero.OneToOneConversationHero.MapFaction.InformalName, false);
				kingdom2.LastMercenaryOfferTime = CampaignTime.Now;
				return true;
			}
			return false;
		}

		public void conversation_lord_mercenary_service_verify_accept_on_consequence()
		{
			int mercenaryWageAmount = Campaign.Current.KingdomManager.GetMercenaryWageAmount(Hero.MainHero);
			GiveGoldAction.ApplyBetweenCharacters(null, Hero.MainHero, mercenaryWageAmount, false);
			Campaign.Current.KingdomManager.PlayerMercenaryServiceNextRenewDay = Campaign.CurrentTime + 720f;
			ChangeKingdomAction.ApplyByJoinFactionAsMercenary(Hero.MainHero.Clan, Hero.OneToOneConversationHero.Clan.Kingdom, 50, true);
		}

		public bool conversation_lord_mercenary_elaborate_castle_answer_faction_owner_to_women_on_condition()
		{
			return Hero.OneToOneConversationHero.IsKingdomLeader && CharacterObject.PlayerCharacter.IsFemale;
		}

		public bool conversation_lord_mercenary_elaborate_castle_answer_to_women_on_condition()
		{
			return !Hero.OneToOneConversationHero.IsKingdomLeader && CharacterObject.PlayerCharacter.IsFemale;
		}

		public bool conversation_lord_mercenary_elaborate_castle_answer_faction_owner_on_condition()
		{
			return Hero.OneToOneConversationHero == Hero.OneToOneConversationHero.MapFaction.Leader;
		}

		public bool conversation_lord_mercenary_elaborate_banner_answer_faction_owner_on_condition()
		{
			return Hero.OneToOneConversationHero.IsKingdomLeader;
		}

		public bool conversation_lord_mission_destroy_bandit_lair_start_on_condition()
		{
			return false;
		}

		public bool conversation_convince_options_bribe_on_condition()
		{
			return false;
		}

		public bool conversation_convince_options_friendship_on_condition()
		{
			MBTextManager.SetTextVariable("RELATION_DECREASE", "10", false);
			return true;
		}

		public bool conversation_convince_bribe_verify_on_condition()
		{
			return Hero.MainHero.Gold >= this._bribeAmount;
		}

		public void conversation_convince_bribe_player_accept_on_consequence()
		{
			GiveGoldAction.ApplyBetweenCharacters(null, Hero.MainHero, this._bribeAmount, false);
			MBTextManager.SetTextVariable("GOLD_AMOUNT", this._bribeAmount);
			InformationManager.DisplayMessage(new InformationMessage(GameTexts.FindText("str_quest_collect_debt_quest_gold_removed", null).ToString(), "event:/ui/notification/coins_negative"));
		}

		public bool conversation_convince_friendship_verify_go_on_on_condition()
		{
			return false;
		}

		public void conversation_convince_friendship_verify_go_on_on_consequence()
		{
		}

		public bool conversation_convince_friendship_lord_response_no_on_condition()
		{
			return false;
		}

		public bool conversation_convince_friendship_lord_response_angry_on_condition()
		{
			return false;
		}

		public void conversation_lord_generic_mission_accept_on_consequence()
		{
		}

		public void conversation_lord_generic_mission_reject_on_consequence()
		{
		}

		public bool conversation_lord_tell_mission_no_quest_on_condition()
		{
			return false;
		}

		public void conversation_player_threats_lord_verify_on_consequence()
		{
			MobileParty encounteredMobileParty = PlayerEncounter.EncounteredMobileParty;
			if (encounteredMobileParty != null && !FactionManager.IsAtWarAgainstFaction(Hero.MainHero.MapFaction, encounteredMobileParty.MapFaction) && Hero.MainHero.MapFaction.Leader == Hero.MainHero)
			{
				ChangeRelationAction.ApplyPlayerRelation(Hero.OneToOneConversationHero, -10, true, true);
				ChangeRelationAction.ApplyPlayerRelation(Hero.OneToOneConversationHero.MapFaction.Leader, -10, true, true);
				DeclareWarAction.ApplyByPlayerHostility(Hero.MainHero.MapFaction, encounteredMobileParty.MapFaction);
			}
		}

		public bool conversation_player_threats_lord_verify_on_condition()
		{
			MobileParty encounteredMobileParty = PlayerEncounter.EncounteredMobileParty;
			return encounteredMobileParty == null || (encounteredMobileParty.LeaderHero.MapFaction != Hero.MainHero.MapFaction && (Hero.MainHero.MapFaction == Clan.PlayerClan || (Hero.MainHero.MapFaction != null && Hero.MainHero.MapFaction.Leader == Hero.MainHero) || FactionManager.IsAtWarAgainstFaction(encounteredMobileParty.LeaderHero.MapFaction, Hero.MainHero.MapFaction)));
		}

		private bool conversation_lord_declines_frivolous_player_surrender_demand_on_condition()
		{
			MBTextManager.SetTextVariable("LORD_DECLINES_FRIVOLOUS_SURRENDER_OFFER", Campaign.Current.ConversationManager.FindMatchingTextOrNull("str_comment_enemy_declines_frivolous_player_surrender_demand", CharacterObject.OneToOneConversationCharacter), false);
			return true;
		}

		public void conversation_lord_attack_verify_cancel_on_consequence()
		{
			PlayerEncounter.LeaveEncounter = true;
		}

		public bool conversation_lord_tell_objective_reconsider_on_condition()
		{
			return false;
		}

		public bool conversation_lord_tell_objective_besiege_on_condition()
		{
			if (CharacterObject.OneToOneConversationCharacter.HeroObject.IsActive)
			{
				MobileParty partyBelongedTo = Hero.OneToOneConversationHero.PartyBelongedTo;
				if (partyBelongedTo != null && (partyBelongedTo.DefaultBehavior == AiBehavior.BesiegeSettlement || (partyBelongedTo.DefaultBehavior == AiBehavior.EscortParty && partyBelongedTo.TargetParty.DefaultBehavior == AiBehavior.BesiegeSettlement)))
				{
					if (partyBelongedTo.DefaultBehavior == AiBehavior.EscortParty)
					{
						MBTextManager.SetTextVariable("TARGET_TOWN", partyBelongedTo.TargetParty.TargetSettlement.Name, false);
					}
					else
					{
						MBTextManager.SetTextVariable("TARGET_TOWN", partyBelongedTo.TargetSettlement.Name, false);
					}
					return true;
				}
			}
			return false;
		}

		public bool conversation_lord_tell_objective_defence_village_on_condition()
		{
			if (Hero.OneToOneConversationHero.IsActive)
			{
				MobileParty partyBelongedTo = Hero.OneToOneConversationHero.PartyBelongedTo;
				if (partyBelongedTo != null && ((partyBelongedTo.DefaultBehavior == AiBehavior.DefendSettlement && partyBelongedTo.TargetSettlement.IsVillage) || (partyBelongedTo.DefaultBehavior == AiBehavior.EscortParty && partyBelongedTo.TargetParty.DefaultBehavior == AiBehavior.DefendSettlement && partyBelongedTo.TargetParty.TargetSettlement.IsVillage)))
				{
					if (partyBelongedTo.DefaultBehavior == AiBehavior.EscortParty)
					{
						MBTextManager.SetTextVariable("TARGET_VILLAGE", partyBelongedTo.TargetParty.TargetSettlement.Name, false);
					}
					else
					{
						MBTextManager.SetTextVariable("TARGET_VILLAGE", partyBelongedTo.TargetSettlement.Name, false);
					}
					return true;
				}
			}
			return false;
		}

		public bool conversation_lord_tell_objective_defence_town_on_condition()
		{
			if (Hero.OneToOneConversationHero.IsActive)
			{
				MobileParty partyBelongedTo = Hero.OneToOneConversationHero.PartyBelongedTo;
				if (partyBelongedTo != null && ((partyBelongedTo.DefaultBehavior == AiBehavior.DefendSettlement && partyBelongedTo.TargetSettlement.IsFortification) || (partyBelongedTo.DefaultBehavior == AiBehavior.EscortParty && partyBelongedTo.TargetParty.DefaultBehavior == AiBehavior.DefendSettlement && partyBelongedTo.TargetParty.TargetSettlement.IsFortification)))
				{
					if (partyBelongedTo.DefaultBehavior == AiBehavior.EscortParty)
					{
						MBTextManager.SetTextVariable("TARGET_TOWN", partyBelongedTo.TargetParty.TargetSettlement.Name, false);
					}
					else
					{
						MBTextManager.SetTextVariable("TARGET_TOWN", partyBelongedTo.TargetSettlement.Name, false);
					}
					return true;
				}
			}
			return false;
		}

		public bool conversation_lord_tell_objective_patrolling_on_condition()
		{
			if (Hero.OneToOneConversationHero.IsActive)
			{
				MobileParty partyBelongedTo = Hero.OneToOneConversationHero.PartyBelongedTo;
				if (partyBelongedTo != null && (partyBelongedTo.DefaultBehavior == AiBehavior.PatrolAroundPoint || (partyBelongedTo.DefaultBehavior == AiBehavior.EscortParty && partyBelongedTo.TargetParty.DefaultBehavior == AiBehavior.PatrolAroundPoint)))
				{
					if (partyBelongedTo.DefaultBehavior == AiBehavior.EscortParty)
					{
						MBTextManager.SetTextVariable("TARGET_SETTLEMENT", partyBelongedTo.TargetParty.TargetSettlement.Name, false);
					}
					else
					{
						MBTextManager.SetTextVariable("TARGET_SETTLEMENT", partyBelongedTo.TargetSettlement.Name, false);
					}
					return true;
				}
			}
			return false;
		}

		public bool conversation_lord_tell_objective_waiting_for_siege_on_condition()
		{
			if (Hero.OneToOneConversationHero.IsActive)
			{
				MobileParty partyBelongedTo = Hero.OneToOneConversationHero.PartyBelongedTo;
				if (partyBelongedTo != null && partyBelongedTo.Army != null && partyBelongedTo.Army.ArmyType == Army.ArmyTypes.Besieger && partyBelongedTo.Army.AIBehavior == Army.AIBehaviorFlags.WaitingForArmyMembers)
				{
					MBTextManager.SetTextVariable("BESIEGED_TOWN", partyBelongedTo.Army.AiBehaviorObject.Name, false);
					return true;
				}
			}
			return false;
		}

		public bool conversation_lord_tell_objective_waiting_for_defence_on_condition()
		{
			if (Hero.OneToOneConversationHero.IsActive)
			{
				MobileParty partyBelongedTo = Hero.OneToOneConversationHero.PartyBelongedTo;
				if (partyBelongedTo != null && partyBelongedTo.Army != null && partyBelongedTo.Army.ArmyType == Army.ArmyTypes.Defender && partyBelongedTo.Army.AIBehavior == Army.AIBehaviorFlags.WaitingForArmyMembers)
				{
					MBTextManager.SetTextVariable("DEFENDED_TOWN", partyBelongedTo.Army.AiBehaviorObject.Name, false);
					return true;
				}
			}
			return false;
		}

		public bool conversation_lord_tell_objective_raiding_on_condition()
		{
			if (Hero.OneToOneConversationHero.IsActive)
			{
				MobileParty partyBelongedTo = Hero.OneToOneConversationHero.PartyBelongedTo;
				if (partyBelongedTo != null && (partyBelongedTo.DefaultBehavior == AiBehavior.RaidSettlement || (partyBelongedTo.DefaultBehavior == AiBehavior.EscortParty && partyBelongedTo.TargetParty.DefaultBehavior == AiBehavior.RaidSettlement)))
				{
					if (partyBelongedTo.DefaultBehavior == AiBehavior.EscortParty)
					{
						MBTextManager.SetTextVariable("TARGET_VILLAGE", partyBelongedTo.TargetParty.TargetSettlement.Name, false);
					}
					else
					{
						MBTextManager.SetTextVariable("TARGET_VILLAGE", partyBelongedTo.TargetSettlement.Name, false);
					}
					return true;
				}
			}
			return false;
		}

		public bool conversation_lord_tell_objective_waiting_for_raid_on_condition()
		{
			if (Hero.OneToOneConversationHero.IsActive)
			{
				MobileParty partyBelongedTo = Hero.OneToOneConversationHero.PartyBelongedTo;
				if (partyBelongedTo != null && partyBelongedTo.Army != null && partyBelongedTo.Army.ArmyType == Army.ArmyTypes.Raider && partyBelongedTo.Army.AIBehavior == Army.AIBehaviorFlags.WaitingForArmyMembers)
				{
					MBTextManager.SetTextVariable("RAIDED_VILLAGE", partyBelongedTo.Army.AiBehaviorObject.Name, false);
					return true;
				}
			}
			return false;
		}

		public bool conversation_lord_tell_objective_gathering_on_condition()
		{
			if (Hero.OneToOneConversationHero.IsActive)
			{
				MobileParty partyBelongedTo = Hero.OneToOneConversationHero.PartyBelongedTo;
				if (partyBelongedTo != null && partyBelongedTo.Army != null && partyBelongedTo.Army.AIBehavior == Army.AIBehaviorFlags.Gathering)
				{
					MBTextManager.SetTextVariable("GATHERING_SETTLEMENT", partyBelongedTo.Army.AiBehaviorObject.Name, false);
					return true;
				}
			}
			return false;
		}

		public void conversation_lord_tell_gathering_player_joined_on_consequence()
		{
			MobileParty partyBelongedTo = Hero.OneToOneConversationHero.PartyBelongedTo;
			MobileParty.MainParty.Army = partyBelongedTo.Army;
		}

		public bool conversation_lord_ask_pardon_answer_bad_relation_on_condition()
		{
			Hero heroObject = CharacterObject.OneToOneConversationCharacter.HeroObject;
			return Hero.MainHero != null && Hero.MainHero.IsEnemy(heroObject);
		}

		public bool conversation_lord_ask_pardon_answer_low_right_to_rule_on_condition()
		{
			return false;
		}

		public bool conversation_lord_ask_pardon_answer_no_advantage_on_condition()
		{
			bool flag = false;
			using (List<Settlement>.Enumerator enumerator = Campaign.Current.Settlements.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.OwnerClan.Leader == Hero.MainHero)
					{
						flag = true;
					}
				}
			}
			return flag && Campaign.Current.CurrentConversationContext == ConversationContext.PartyEncounter && PartyBase.MainParty.Side == BattleSideEnum.Defender;
		}

		public bool conversation_lord_ask_pardon_answer_not_accepted_on_condition()
		{
			bool flag = false;
			using (List<Settlement>.Enumerator enumerator = Campaign.Current.Settlements.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.OwnerClan.Leader == Hero.MainHero)
					{
						flag = true;
					}
				}
			}
			return flag;
		}

		public bool conversation_lord_ask_pardon_answer_accepted_on_condition()
		{
			bool flag = false;
			using (List<Settlement>.Enumerator enumerator = Campaign.Current.Settlements.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.OwnerClan.Leader == Hero.MainHero)
					{
						flag = true;
					}
				}
			}
			return flag;
		}

		public bool conversation_lord_answers_the_location_on_condition()
		{
			List<Settlement> list = Campaign.Current.Settlements.Where((Settlement settlement) => settlement.IsTown || settlement.IsVillage).ToList<Settlement>();
			bool flag = false;
			Settlement settlement3 = list[0];
			float num = 1000f;
			bool partyBelongedTo = ConversationHelper.AskedLord.HeroObject.PartyBelongedTo != null;
			StringHelpers.SetCharacterProperties("ASKED_LORD", ConversationHelper.AskedLord, null, false);
			if (partyBelongedTo)
			{
				foreach (Settlement settlement2 in list)
				{
					foreach (MobileParty mobileParty in settlement2.Parties)
					{
						Hero leaderHero = mobileParty.LeaderHero;
						if (((leaderHero != null) ? leaderHero.CharacterObject : null) == ConversationHelper.AskedLord)
						{
							flag = true;
							MBTextManager.SetTextVariable("TOWN_NAME", settlement2.Name, false);
							break;
						}
					}
					if (flag)
					{
						break;
					}
					float num2;
					if (Campaign.Current.Models.MapDistanceModel.GetDistance(ConversationHelper.AskedLord.HeroObject.PartyBelongedTo, settlement2, num, out num2))
					{
						settlement3 = settlement2;
						num = num2;
					}
				}
				if (flag)
				{
					MBTextManager.SetTextVariable("LORD_ANSWER", GameTexts.FindText("str_he_is_currently_at_town_name", null), false);
				}
				else
				{
					MBTextManager.SetTextVariable("SETTLEMENT_NAME", settlement3.Name, false);
					MBTextManager.SetTextVariable("LORD_ANSWER", GameTexts.FindText("str_he_is_in_the_field", null), false);
				}
			}
			else if (ConversationHelper.AskedLord.HeroObject.IsPrisoner)
			{
				StringHelpers.SetCharacterProperties("PRISON_PARTY_LEADER", ConversationHelper.GetConversationCharacterPartyLeader(ConversationHelper.AskedLord.HeroObject.PartyBelongedToAsPrisoner), null, false);
				MBTextManager.SetTextVariable("LORD_ANSWER", GameTexts.FindText("str_he_is_prisoner_at", null), false);
			}
			else if (ConversationHelper.AskedLord.HeroObject.IsFugitive)
			{
				MBTextManager.SetTextVariable("LORD_ANSWER", GameTexts.FindText("str_he_is_fugitive", null), false);
			}
			else
			{
				MBTextManager.SetTextVariable("LORD_ANSWER", GameTexts.FindText("str_i_do_not_know_where_he_is", null), false);
			}
			return true;
		}

		public bool conversation_lord_give_oath_1_player_answer_1_on_condition()
		{
			StringHelpers.SetCharacterProperties("LORD", CharacterObject.OneToOneConversationCharacter, null, false);
			return true;
		}

		public bool conversation_set_oath_phrases_on_condition()
		{
			string stringId = Hero.OneToOneConversationHero.Culture.StringId;
			MBTextManager.SetTextVariable("FACTION_TITLE", this.GetLiegeTitle(), false);
			StringHelpers.SetCharacterProperties("LORD", CharacterObject.OneToOneConversationCharacter, null, false);
			if (stringId == "empire")
			{
				MBTextManager.SetTextVariable("OATH_LINE_1", "{=ya8VF98X}I swear by my ancestors that you are lawful {FACTION_TITLE}.", false);
			}
			else if (stringId == "khuzait")
			{
				MBTextManager.SetTextVariable("OATH_LINE_1", "{=PP8VeNiC}I swear that you are my {?LORD.GENDER}khatun{?}khan{\\?}, my {?LORD.GENDER}mother{?}father{\\?}, my protector...", false);
			}
			else
			{
				MBTextManager.SetTextVariable("OATH_LINE_1", "{=MqIg6Mh2}I swear homage to you as lawful {FACTION_TITLE}.", false);
			}
			if (stringId == "empire")
			{
				MBTextManager.SetTextVariable("OATH_LINE_2", "{=vuEyisBW}I affirm that you are executor of the will of the Senate and people...", false);
			}
			else if (stringId == "khuzait")
			{
				MBTextManager.SetTextVariable("OATH_LINE_2", "{=QSPMKz2R}You are the chosen of the Sky, and I shall follow your banner as long as my breath remains...", false);
			}
			else if (stringId == "battania")
			{
				MBTextManager.SetTextVariable("OATH_LINE_2", "{=OHJYAaW5}The powers of Heaven and of the Earth have entrusted to you the guardianship of this sacred land...", false);
			}
			else if (stringId == "aserai")
			{
				MBTextManager.SetTextVariable("OATH_LINE_2", "{=kc3tLqGy}You command the sons of Asera in war and govern them in peace...", false);
			}
			else if (stringId == "sturgia")
			{
				MBTextManager.SetTextVariable("OATH_LINE_2", "{=Qs7qs3b0}You are the shield of our people against the wolves of the forest, the steppe and the sea.", false);
			}
			else
			{
				MBTextManager.SetTextVariable("OATH_LINE_2", "{=PypPEj5Z}I will be your loyal {?PLAYER.GENDER}follower{?}man{\\?} as long as my breath remains...", false);
			}
			if (stringId == "empire")
			{
				MBTextManager.SetTextVariable("OATH_LINE_3", "{=LWFDXeQc}Furthermore, I accept induction into the army of Calradia, at the rank of archon.", false);
			}
			else if (stringId == "khuzait")
			{
				MBTextManager.SetTextVariable("OATH_LINE_3", "{=8lOCOcXw}Your word shall direct the strike of my sword and the flight of my arrow...", false);
			}
			else if (stringId == "aserai")
			{
				MBTextManager.SetTextVariable("OATH_LINE_3", "{=bue9AShm}I swear to fight your enemies and give shelter and water to your friends...", false);
			}
			else if (stringId == "sturgia")
			{
				MBTextManager.SetTextVariable("OATH_LINE_3", "{=U3u2D6Ze}I give you my word and bond, to stand by your banner in battle so long as my breath remains...", false);
			}
			else if (stringId == "battania")
			{
				MBTextManager.SetTextVariable("OATH_LINE_3", "{=UwbhGhGw}I shall stand by your side and not foresake you, and fight until my life leaves my body...", false);
			}
			else
			{
				MBTextManager.SetTextVariable("OATH_LINE_3", "{=2o7U1bNV}..and I will be at your side to fight your enemies should you need my sword.", false);
			}
			if (stringId == "empire")
			{
				MBTextManager.SetTextVariable("OATH_LINE_4", "{=EsF8sEaQ}And as such, that you are my commander, and I shall follow you wherever you lead.", false);
			}
			else if (stringId == "battania")
			{
				MBTextManager.SetTextVariable("OATH_LINE_4", "{=6KbDn1HS}I shall heed your judgements and pay you the tribute that is your due, so that this land may have a strong protector.", false);
			}
			else if (stringId == "khuzait")
			{
				MBTextManager.SetTextVariable("OATH_LINE_4", "{=xDzxaYed}Your word shall divide the spoils of victory and the bounties of peace.", false);
			}
			else if (stringId == "aserai")
			{
				MBTextManager.SetTextVariable("OATH_LINE_4", "{=qObicX7y}I swear to heed your judgements according to the laws of the Aserai, and ensure that my kinfolk heed them as well...", false);
			}
			else if (stringId == "sturgia")
			{
				MBTextManager.SetTextVariable("OATH_LINE_4", "{=HpWYfcgw}..and to uphold your rights under the laws of the Sturgians, and the rights of your kin, and to avenge their blood as thought it were my own.", false);
			}
			else
			{
				MBTextManager.SetTextVariable("OATH_LINE_4", "{=waoSd6tj}.. and I shall defend your rights and the rights of your legitimate heirs.", false);
			}
			return true;
		}

		private void lord_give_oath_give_up_consequence()
		{
			if (PlayerEncounter.Current != null)
			{
				PlayerEncounter.LeaveEncounter = true;
			}
		}

		public bool conversation_vassalage_offer_player_is_already_vassal_on_condition()
		{
			if (Hero.MainHero.MapFaction != Clan.PlayerClan && !Clan.PlayerClan.IsUnderMercenaryService && Hero.MainHero.MapFaction != Hero.OneToOneConversationHero.MapFaction)
			{
				MBTextManager.SetTextVariable("FACTION_NAME", Hero.MainHero.MapFaction.Name, false);
				return true;
			}
			return false;
		}

		public bool conversation_vassalage_offer_player_has_low_relation_on_condition()
		{
			Hero heroObject = CharacterObject.OneToOneConversationCharacter.HeroObject;
			return Hero.MainHero != null && Hero.MainHero.IsEnemy(heroObject);
		}

		public bool conversation_mercenary_service_offer_rejected_on_condition()
		{
			return !this.conversation_mercenary_service_offer_accepted_on_condition();
		}

		public bool conversation_mercenary_service_offer_accepted_on_condition()
		{
			return FactionHelper.CanPlayerEnterFaction(false);
		}

		public bool conversation_vassalage_offer_accepted_on_condition()
		{
			return FactionHelper.CanPlayerEnterFaction(true);
		}

		public bool conversation_liege_states_obligations_to_vassal_on_condition()
		{
			if (Hero.OneToOneConversationHero.Culture.StringId == "vlandia")
			{
				MBTextManager.SetTextVariable("PLAYER_ACCEPTED_AS_VASSAL", "{=6oevXUSa}Let it be known that from this day forward, you are my sworn {?PLAYER.GENDER}follower{?}man{\\?} and vassal. I give you my protection and grant you the right to bear arms in my name, and I pledge that I shall not deprive you of your life, liberty or properties except by the lawful judgment of your peers or by the law and custom of the land.", false);
			}
			else if (Hero.OneToOneConversationHero.Culture.StringId == "khuzait")
			{
				MBTextManager.SetTextVariable("PLAYER_ACCEPTED_AS_VASSAL", "{=iWBrManr}Let it be known that you are adopted into the Khuzait confederacy, and that you shall be considered of the ancestry of the 12 sons of the she-wolf. You may sit in our councils of war and of peace. We shall ride to defend your flocks and avenge your blood if you fall. Your herds may graze in our lands and drink from our springs.", false);
			}
			else if (Hero.OneToOneConversationHero.Culture.StringId == "empire")
			{
				MBTextManager.SetTextVariable("PLAYER_ACCEPTED_AS_VASSAL", "{=IMSCdhyy}I proclaim you a citizen of the Empire, of the rank of Senator. Your life and property shall be protected by our laws, and shall not be taken from you except by law. You may serve as a magistrate over towns and villages and as a general over armies, if we call upon you to do so.", false);
			}
			else if (Hero.OneToOneConversationHero.Culture.StringId == "aserai")
			{
				MBTextManager.SetTextVariable("PLAYER_ACCEPTED_AS_VASSAL", "{=3v3ZTccn}You shall be numbered among the sons of Asera. Your blood is our blood. Our swords shall defend your rights as you defend ours. You may drink from our wells and rest in the shade of our trees. You may be granted the authority to judge disputes and collect revenues from oases and towns.", false);
			}
			else if (Hero.OneToOneConversationHero.Culture.StringId == "sturgia")
			{
				MBTextManager.SetTextVariable("PLAYER_ACCEPTED_AS_VASSAL", "{=fInFLbAV}I accept you as my sworn follower. You shall have the warrior's due: the warmth of my hearthfire and the bread of my fields, and gold for your valor. I shall uphold your rights under the Law of the Sturgians and avenge your blood if you fall.", false);
			}
			else if (Hero.OneToOneConversationHero.Culture.StringId == "battania")
			{
				MBTextManager.SetTextVariable("PLAYER_ACCEPTED_AS_VASSAL", "{=dhi3ggBC}Let it be known that you are one of the Battanians. You may till our soil and sit in our councils. Who quarrels with you, quarrels with all of us.", false);
			}
			else
			{
				MBTextManager.SetTextVariable("PLAYER_ACCEPTED_AS_VASSAL", "{=xd0MAjMf}Let it be known that you are one of us. We shall defend your rights as you defend ours. You may be granted lands in our domains and the authority to judge disputes.", false);
			}
			return true;
		}

		public void conversation_player_is_accepted_as_a_vassal_on_consequence()
		{
			if (Hero.MainHero.Clan.Kingdom == Hero.OneToOneConversationHero.Clan.Kingdom)
			{
				Hero.MainHero.Clan.EndMercenaryService(false);
			}
			else
			{
				if (Clan.PlayerClan.IsUnderMercenaryService)
				{
					Hero.MainHero.Clan.EndMercenaryService(true);
				}
				ChangeKingdomAction.ApplyByJoinToKingdom(Hero.MainHero.Clan, Hero.OneToOneConversationHero.Clan.Kingdom, true);
			}
			if (!this._receivedVassalRewards)
			{
				this.ReceiveVassalRewards();
			}
			GainKingdomInfluenceAction.ApplyForJoiningFaction(Hero.MainHero, Campaign.Current.Models.VassalRewardsModel.InfluenceReward);
			if (PlayerEncounter.Current != null)
			{
				PlayerEncounter.LeaveEncounter = true;
			}
		}

		private bool conversation_lord_give_oath_go_on_condition()
		{
			if (this._receivedVassalRewards)
			{
				MBTextManager.SetTextVariable("RULER_VASSALAGE_SPEECH", "{=lgcHkTXT}Very well. You have given me your solemn oath, {PLAYER.NAME}. May you uphold it always, with proper courage and devotion.", false);
			}
			else
			{
				MBTextManager.SetTextVariable("RULER_VASSALAGE_SPEECH", "{=WtjgSaFn}In exchange for your loyalty, I offer you the command of some of my best men. And in recognition of your worth, let me present you with this gift, which I hope will serve you well on the battlefield.", false);
			}
			return true;
		}

		public void conversation_player_leave_faction_accepted_on_consequence()
		{
			if (Clan.PlayerClan.IsUnderMercenaryService)
			{
				ChangeKingdomAction.ApplyByLeaveKingdomAsMercenary(Hero.MainHero.Clan, true);
				return;
			}
			ChangeKingdomAction.ApplyByLeaveKingdom(Hero.MainHero.Clan, true);
		}

		public void conversation_player_leave_faction_accepted_on_leave()
		{
			if (PlayerEncounter.Current != null)
			{
				PlayerEncounter.LeaveEncounter = true;
				return;
			}
			MapState mapState = Game.Current.GameStateManager.ActiveState.Predecessor as MapState;
			if (mapState != null && mapState.MenuContext != null)
			{
				mapState.ExitMenuMode();
			}
		}

		private void ReceiveVassalRewards()
		{
			VassalRewardsModel vassalRewardsModel = Campaign.Current.Models.VassalRewardsModel;
			InventoryManager.OpenScreenAsReceiveItems(vassalRewardsModel.GetEquipmentRewardsForJoiningKingdom(Hero.OneToOneConversationHero.Clan.Kingdom), new TextObject("{=exbSCGzi}Reward Items", null), null);
			PartyScreenManager.OpenScreenAsReceiveTroops(vassalRewardsModel.GetTroopRewardsForJoiningKingdom(Hero.OneToOneConversationHero.Clan.Kingdom), new TextObject("{=tKW8m6bZ}Reward Troops", null), null);
			ChangeRelationAction.ApplyPlayerRelation(Hero.OneToOneConversationHero.Clan.Kingdom.Leader, vassalRewardsModel.RelationRewardWithLeader, true, true);
			this._receivedVassalRewards = true;
		}

		public bool conversation_lord_talk_ask_location_2_on_condition()
		{
			Hero hero = (Hero)ConversationSentence.CurrentProcessedRepeatObject;
			StringHelpers.SetCharacterProperties("LORD", hero.CharacterObject, null, false);
			return true;
		}

		public void conversation_lord_talk_ask_location_2_on_consequence()
		{
			ConversationHelper.AskedLord = ((Hero)ConversationSentence.SelectedRepeatObject).CharacterObject;
		}

		private bool conversation_clan_member_manage_troops_on_condition()
		{
			Hero oneToOneConversationHero = Hero.OneToOneConversationHero;
			return oneToOneConversationHero != null && oneToOneConversationHero.Clan == Clan.PlayerClan && oneToOneConversationHero.PartyBelongedTo != null && oneToOneConversationHero.PartyBelongedTo.LeaderHero == oneToOneConversationHero && !oneToOneConversationHero.PartyBelongedTo.IsCaravan && !oneToOneConversationHero.PartyBelongedTo.IsMilitia && !oneToOneConversationHero.PartyBelongedTo.IsVillager;
		}

		private void conversation_clan_member_manage_troops_on_consequence()
		{
			PartyScreenManager.OpenScreenAsManageTroopsAndPrisoners(Hero.OneToOneConversationHero.PartyBelongedTo, new PartyScreenClosedDelegate(this.onPartyScreenClosed));
		}

		private void onPartyScreenClosed(PartyBase leftOwnerParty, TroopRoster leftMemberRoster, TroopRoster leftPrisonRoster, PartyBase rightOwnerParty, TroopRoster rightMemberRoster, TroopRoster rightPrisonRoster, bool fromCancel)
		{
			Campaign.Current.ConversationManager.ContinueConversation();
		}

		private const int LordStartPriority = 110;

		private int _goldAmount;

		private Dictionary<CharacterObject, CharacterObject> _previouslyMetWandererTemplates = new Dictionary<CharacterObject, CharacterObject>();

		private bool _receivedVassalRewards;

		private const int PlayerReleasesPrisonerRelationChange = 4;

		private const int PlayerCapturesPrisonerRelationChange = 0;

		private const int PlayerLiberatesPrisonerRelationChange = 10;

		private Hero _selectedPrisoner;

		private Clan _askedFaction;

		private int _bribeAmount;

		private bool _willDoPeaceBarter;

		public class Number
		{
			public Number(int value)
			{
				this.Value = value;
			}

			public IEnumerable<LordConversationsCampaignBehavior.Number> GetBetween(int start, int end)
			{
				int num;
				for (int i = start; i < end + 1; i = num)
				{
					LordConversationsCampaignBehavior.Number number = new LordConversationsCampaignBehavior.Number(i);
					yield return number;
					num = i + 1;
				}
				yield break;
			}

			public int Value;
		}
	}
}
