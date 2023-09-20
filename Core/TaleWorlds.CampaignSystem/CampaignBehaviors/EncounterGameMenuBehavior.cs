using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Overlay;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.CampaignSystem.Siege;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	// Token: 0x02000390 RID: 912
	public class EncounterGameMenuBehavior : CampaignBehaviorBase
	{
		// Token: 0x060035B1 RID: 13745 RVA: 0x000EDD9C File Offset: 0x000EBF9C
		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<TroopRoster>("_breakInOutCasualties", ref this._breakInOutCasualties);
			dataStore.SyncData<int>("_intEncounterVariable", ref this._intEncounterVariable);
			dataStore.SyncData<int>("_breakInOutArmyCasualties", ref this._breakInOutArmyCasualties);
			dataStore.SyncData<TroopRoster>("_getAwayCasualties", ref this._getAwayCasualties);
			dataStore.SyncData<ItemRoster>("_getAwayLostBaggage", ref this._getAwayLostBaggage);
			dataStore.SyncData<bool>("_playerIsAlreadyInCastle", ref this._playerIsAlreadyInCastle);
		}

		// Token: 0x060035B2 RID: 13746 RVA: 0x000EDE15 File Offset: 0x000EC015
		public override void RegisterEvents()
		{
			CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));
			CampaignEvents.OnSettlementLeftEvent.AddNonSerializedListener(this, new Action<MobileParty, Settlement>(this.OnSettlementLeft));
		}

		// Token: 0x060035B3 RID: 13747 RVA: 0x000EDE45 File Offset: 0x000EC045
		private void OnSettlementLeft(MobileParty party, Settlement settlement)
		{
			if (party == MobileParty.MainParty)
			{
				this._playerIsAlreadyInCastle = false;
			}
		}

		// Token: 0x060035B4 RID: 13748 RVA: 0x000EDE56 File Offset: 0x000EC056
		private void OnSessionLaunched(CampaignGameStarter campaignGameStarter)
		{
			this.InitializeAccessDetails();
			this.AddGameMenus(campaignGameStarter);
		}

		// Token: 0x060035B5 RID: 13749 RVA: 0x000EDE68 File Offset: 0x000EC068
		private void InitializeAccessDetails()
		{
			Settlement currentSettlement = Settlement.CurrentSettlement;
			if (currentSettlement != null && (currentSettlement.IsFortification || currentSettlement.IsVillage))
			{
				Campaign.Current.Models.SettlementAccessModel.CanMainHeroEnterSettlement(Settlement.CurrentSettlement, out this._accessDetails);
			}
		}

		// Token: 0x060035B6 RID: 13750 RVA: 0x000EDEB0 File Offset: 0x000EC0B0
		private void AddGameMenus(CampaignGameStarter gameSystemInitializer)
		{
			gameSystemInitializer.AddGameMenu("taken_prisoner", "{=ezClQMBj}Your enemies take you as a prisoner.", new OnInitDelegate(this.game_menu_taken_prisoner_on_init), GameOverlays.MenuOverlayType.None, GameMenu.MenuFlags.None, null);
			gameSystemInitializer.AddGameMenuOption("taken_prisoner", "taken_prisoner_continue", "{=WVkc4UgX}Continue.", new GameMenuOption.OnConditionDelegate(this.game_menu_taken_prisoner_continue_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_taken_prisoner_continue_on_consequence), false, -1, false, null);
			gameSystemInitializer.AddGameMenu("defeated_and_taken_prisoner", "{=ezClQMBj}Your enemies take you as a prisoner.", new OnInitDelegate(this.game_menu_taken_prisoner_on_init), GameOverlays.MenuOverlayType.None, GameMenu.MenuFlags.None, null);
			gameSystemInitializer.AddGameMenuOption("defeated_and_taken_prisoner", "taken_prisoner_continue", "{=WVkc4UgX}Continue.", new GameMenuOption.OnConditionDelegate(this.game_menu_taken_prisoner_continue_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_taken_prisoner_continue_on_consequence), false, -1, false, null);
			gameSystemInitializer.AddGameMenu("encounter_meeting", "{=!}.", new OnInitDelegate(this.game_menu_encounter_meeting_on_init), GameOverlays.MenuOverlayType.None, GameMenu.MenuFlags.None, null);
			gameSystemInitializer.AddGameMenu("join_encounter", "{=jKWJpIES}{JOIN_ENCOUNTER_TEXT}. You decide to...", new OnInitDelegate(this.game_menu_join_encounter_on_init), GameOverlays.MenuOverlayType.None, GameMenu.MenuFlags.None, null);
			gameSystemInitializer.AddGameMenuOption("join_encounter", "join_encounter_help_attackers", "{=h3yEHb4U}Help {ATTACKER}.", new GameMenuOption.OnConditionDelegate(this.game_menu_join_encounter_help_attackers_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_join_encounter_help_attackers_on_consequence), false, -1, false, null);
			gameSystemInitializer.AddGameMenuOption("join_encounter", "join_encounter_help_defenders", "{=FwIgakj8}Help {DEFENDER}.", new GameMenuOption.OnConditionDelegate(this.game_menu_join_encounter_help_defenders_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_join_encounter_help_defenders_on_consequence), false, -1, false, null);
			gameSystemInitializer.AddGameMenuOption("join_encounter", "join_encounter_abandon", "{=Nr49hlfC}Abandon army.", new GameMenuOption.OnConditionDelegate(this.game_menu_join_encounter_abandon_army_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_encounter_abandon_on_consequence), false, -1, false, null);
			gameSystemInitializer.AddGameMenuOption("join_encounter", "join_encounter_leave", "{=!}{LEAVE_TEXT}", new GameMenuOption.OnConditionDelegate(this.game_menu_join_encounter_leave_no_army_on_condition), delegate(MenuCallbackArgs args)
			{
				if (MobileParty.MainParty.SiegeEvent != null && MobileParty.MainParty.SiegeEvent.BesiegerCamp != null && MobileParty.MainParty.SiegeEvent.BesiegerCamp.HasInvolvedPartyForEventType(PartyBase.MainParty, MapEvent.BattleTypes.Siege))
				{
					MobileParty.MainParty.BesiegerCamp = null;
				}
				PlayerEncounter.Finish(true);
			}, true, -1, false, null);
			gameSystemInitializer.AddGameMenu("join_sally_out", "{=CcNVobQU}Garrison of the settlement you are in decided to sally out. You decide to...", new OnInitDelegate(this.game_menu_join_sally_out_on_init), GameOverlays.MenuOverlayType.Encounter, GameMenu.MenuFlags.None, null);
			gameSystemInitializer.AddGameMenuOption("join_sally_out", "join_siege_event", "{=IaA8sbY2}Join the sally out", new GameMenuOption.OnConditionDelegate(this.game_menu_join_sally_out_event_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_join_sally_out_on_consequence), false, -1, false, null);
			gameSystemInitializer.AddGameMenuOption("join_sally_out", "join_siege_event_break_in", "{=z1RHDsOG}Stay in settlement", new GameMenuOption.OnConditionDelegate(this.game_menu_stay_in_settlement_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_stay_in_settlement_on_consequence), true, -1, false, null);
			gameSystemInitializer.AddGameMenu("join_siege_event", "{=xNyKVMHx}{JOIN_SIEGE_TEXT} You decide to...", new OnInitDelegate(this.game_menu_join_siege_event_on_init), GameOverlays.MenuOverlayType.None, GameMenu.MenuFlags.None, null);
			gameSystemInitializer.AddGameMenuOption("join_siege_event", "join_siege_event", "{=ZVsJf5Ff}Join the continuing siege.", new GameMenuOption.OnConditionDelegate(this.game_menu_join_siege_event_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_join_siege_event_on_consequence), false, -1, false, null);
			gameSystemInitializer.AddGameMenuOption("join_siege_event", "attack_besiegers", "{=CVg3P07C}Assault the siege camp.", new GameMenuOption.OnConditionDelegate(this.attack_besieger_side_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_join_encounter_help_defenders_on_consequence), false, -1, false, null);
			gameSystemInitializer.AddGameMenuOption("join_siege_event", "join_siege_event_break_in", "{=XAvwP3Ce}Break in to help the defenders", new GameMenuOption.OnConditionDelegate(this.break_in_to_help_defender_side_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_join_siege_event_on_defender_side_on_consequence), false, -1, false, null);
			gameSystemInitializer.AddGameMenuOption("join_siege_event", "join_encounter_leave", "{=ebUwP3Q3}Don't get involved.", new GameMenuOption.OnConditionDelegate(this.game_menu_join_encounter_leave_on_condition), delegate(MenuCallbackArgs args)
			{
				PlayerEncounter.Finish(true);
				if (Hero.MainHero.PartyBelongedTo != null && Hero.MainHero.PartyBelongedTo.Army != null && Hero.MainHero.PartyBelongedTo.Army.LeaderParty != MobileParty.MainParty)
				{
					Hero.MainHero.PartyBelongedTo.Army = null;
					MobileParty.MainParty.Ai.SetMoveModeHold();
				}
			}, true, -1, false, null);
			gameSystemInitializer.AddGameMenu("siege_attacker_left", "{=LR6Y57Rq}Attackers abandoned the siege.", null, GameOverlays.MenuOverlayType.None, GameMenu.MenuFlags.None, null);
			gameSystemInitializer.AddGameMenuOption("siege_attacker_left", "siege_attacker_left_return_to_settlement", "{=j7bZRFxc}Return to {SETTLEMENT}.", new GameMenuOption.OnConditionDelegate(this.game_menu_siege_attacker_left_return_to_settlement_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_siege_attacker_left_return_to_settlement_on_consequence), false, -1, false, null);
			gameSystemInitializer.AddGameMenuOption("siege_attacker_left", "siege_attacker_left_leave", "{=mfAP8Wlq}Leave settlement.", new GameMenuOption.OnConditionDelegate(this.game_menu_siege_attacker_left_leave_on_condition), delegate(MenuCallbackArgs args)
			{
				PlayerEncounter.Finish(true);
			}, true, -1, false, null);
			gameSystemInitializer.AddGameMenu("siege_attacker_defeated", "{=njbpMLdJ}Attackers have been defeated.", null, GameOverlays.MenuOverlayType.None, GameMenu.MenuFlags.None, null);
			gameSystemInitializer.AddGameMenuOption("siege_attacker_defeated", "siege_attacker_defeated_return_to_settlement", "{=j7bZRFxc}Return to {SETTLEMENT}.", new GameMenuOption.OnConditionDelegate(this.game_menu_siege_attacker_left_return_to_settlement_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_siege_attacker_left_return_to_settlement_on_consequence), false, -1, false, null);
			gameSystemInitializer.AddGameMenuOption("siege_attacker_defeated", "siege_attacker_defeated_leave", "{=mfAP8Wlq}Leave settlement.", new GameMenuOption.OnConditionDelegate(this.game_menu_siege_attacker_defeated_leave_on_condition), delegate(MenuCallbackArgs args)
			{
				PlayerEncounter.Finish(true);
			}, true, -1, false, null);
			gameSystemInitializer.AddGameMenu("hostile_action_end_by_leave_kingdom_as_mercenary", "{=lBHm1JlL}Your kingdom has ended your mercenary contract.", null, GameOverlays.MenuOverlayType.None, GameMenu.MenuFlags.None, null);
			gameSystemInitializer.AddGameMenuOption("hostile_action_end_by_leave_kingdom_as_mercenary", "hostile_action_end_by_leave_kingdom_end", "{=2YYRyrOO}Leave...", new GameMenuOption.OnConditionDelegate(this.game_menu_siege_attacker_defeated_leave_on_condition), delegate(MenuCallbackArgs args)
			{
				PlayerEncounter.Finish(true);
			}, true, -1, false, null);
			gameSystemInitializer.AddGameMenu("hostile_action_end_by_peace", "{=1rbg3Hz2}The {HOSTILE_FACTION} and {SETTLEMENT_FACTION} are no longer enemies.", new OnInitDelegate(this.game_menu_hostile_action_end_by_peace_on_init), GameOverlays.MenuOverlayType.None, GameMenu.MenuFlags.None, null);
			gameSystemInitializer.AddGameMenuOption("hostile_action_end_by_peace", "hostile_action_en_by_peace_end", "{=WVkc4UgX}Continue.", delegate(MenuCallbackArgs args)
			{
				args.optionLeaveType = GameMenuOption.LeaveType.Leave;
				return true;
			}, delegate(MenuCallbackArgs args)
			{
				if (PlayerEncounter.Current != null)
				{
					PlayerEncounter.Finish(true);
					return;
				}
				GameMenu.ExitToLast();
			}, true, -1, false, null);
			gameSystemInitializer.AddGameMenu("encounter", "{=!}{ENCOUNTER_TEXT}", new OnInitDelegate(this.game_menu_encounter_on_init), GameOverlays.MenuOverlayType.Encounter, GameMenu.MenuFlags.None, null);
			gameSystemInitializer.AddGameMenuOption("encounter", "continue_preparations", "{=FOoMM4AU}Continue siege preparations.", new GameMenuOption.OnConditionDelegate(this.game_menu_town_besiege_continue_siege_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_town_besiege_continue_siege_on_consequence), false, -1, false, null);
			gameSystemInitializer.AddGameMenuOption("encounter", "village_raid_action", "{=lvttCRi8}Plunder the village, then raze it.", new GameMenuOption.OnConditionDelegate(this.game_menu_village_hostile_action_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_village_raid_no_resist_on_consequence), false, -1, false, null);
			gameSystemInitializer.AddGameMenuOption("encounter", "village_force_volunteer_action", "{=9YHjPkb8}Force notables to give you recruits.", new GameMenuOption.OnConditionDelegate(this.game_menu_village_hostile_action_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_village_force_volunteers_no_resist_loot_on_consequence), false, -1, false, null);
			gameSystemInitializer.AddGameMenuOption("encounter", "village_force_supplies_action", "{=JMzyh6Gl}Force people to give you supplies.", new GameMenuOption.OnConditionDelegate(this.game_menu_village_hostile_action_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_village_force_supplies_no_resist_loot_on_consequence), false, -1, false, null);
			gameSystemInitializer.AddGameMenuOption("encounter", "attack", "{=o1pZHZOF}{ATTACK_TEXT}!", new GameMenuOption.OnConditionDelegate(this.game_menu_encounter_attack_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_encounter_attack_on_consequence), false, -1, false, null);
			gameSystemInitializer.AddGameMenuOption("encounter", "capture_the_enemy", "{=27yneDGL}Capture the enemy.", new GameMenuOption.OnConditionDelegate(this.game_menu_encounter_capture_the_enemy_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_capture_the_enemy_on_consequence), false, -1, false, null);
			gameSystemInitializer.AddGameMenuOption("encounter", "str_order_attack", "{=!}{SEND_TROOPS_TEXT}", new GameMenuOption.OnConditionDelegate(this.game_menu_encounter_order_attack_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_encounter_order_attack_on_consequence), false, -1, false, null);
			gameSystemInitializer.AddGameMenuOption("encounter", "leave_soldiers_behind", "{=qNgGoqmI}Try to get away.", new GameMenuOption.OnConditionDelegate(this.game_menu_encounter_leave_your_soldiers_behind_on_condition), delegate(MenuCallbackArgs args)
			{
				GameMenu.SwitchToMenu("try_to_get_away");
			}, false, -1, false, null);
			gameSystemInitializer.AddGameMenuOption("encounter", "surrender", "{=3nT5wWzb}Surrender.", new GameMenuOption.OnConditionDelegate(this.game_menu_encounter_surrender_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_encounter_surrender_on_consequence), false, -1, false, null);
			gameSystemInitializer.AddGameMenuOption("encounter", "leave", "{=2YYRyrOO}Leave...", new GameMenuOption.OnConditionDelegate(this.game_menu_encounter_leave_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_encounter_leave_on_consequence), true, -1, false, null);
			gameSystemInitializer.AddGameMenuOption("encounter", "abandon_army", "{=Nr49hlfC}Abandon army.", new GameMenuOption.OnConditionDelegate(this.game_menu_encounter_abandon_army_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_encounter_abandon_on_consequence), true, -1, false, null);
			gameSystemInitializer.AddGameMenuOption("encounter", "go_back_to_settlement", "{=j7bZRFxc}Return to {SETTLEMENT}.", new GameMenuOption.OnConditionDelegate(this.game_menu_sally_out_go_back_to_settlement_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_sally_out_go_back_to_settlement_consequence), true, -1, false, null);
			gameSystemInitializer.AddGameMenu("army_encounter", "{=!}{ARMY_ENCOUNTER_TEXT}", new OnInitDelegate(this.game_menu_army_encounter_on_init), GameOverlays.MenuOverlayType.None, GameMenu.MenuFlags.None, null);
			gameSystemInitializer.AddGameMenuOption("army_encounter", "army_talk_to_leader", "{=tYVW8iQN}Talk to army leader", new GameMenuOption.OnConditionDelegate(this.game_menu_army_talk_to_leader_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_army_talk_to_leader_on_consequence), false, -1, false, null);
			gameSystemInitializer.AddGameMenuOption("army_encounter", "army_talk_to_other_members", "{=b7APCGY2}Talk to other members", new GameMenuOption.OnConditionDelegate(this.game_menu_army_talk_to_other_members_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_army_talk_to_other_members_on_consequence), false, -1, false, null);
			gameSystemInitializer.AddGameMenuOption("army_encounter", "army_join_army", "{=N4Qa0WsT}Join army", new GameMenuOption.OnConditionDelegate(this.game_menu_army_join_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_army_join_on_consequence), false, -1, false, null);
			gameSystemInitializer.AddGameMenuOption("army_encounter", "army_attack_army", "{=0URijoc0}Attack army", new GameMenuOption.OnConditionDelegate(this.game_menu_army_attack_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_army_attack_on_consequence), false, -1, false, null);
			gameSystemInitializer.AddGameMenuOption("army_encounter", "army_leave", "{=2YYRyrOO}Leave...", new GameMenuOption.OnConditionDelegate(this.game_menu_army_leave_on_condition), new GameMenuOption.OnConsequenceDelegate(this.army_encounter_leave_on_consequence), true, -1, false, null);
			gameSystemInitializer.AddGameMenu("game_menu_army_talk_to_other_members", "{=yYTotiqW}Talk to...", new OnInitDelegate(this.game_menu_army_talk_to_other_members_on_init), GameOverlays.MenuOverlayType.None, GameMenu.MenuFlags.None, null);
			gameSystemInitializer.AddGameMenuOption("game_menu_army_talk_to_other_members", "game_menu_army_talk_to_other_members_item", "{=!}{CHAR_NAME}", new GameMenuOption.OnConditionDelegate(this.game_menu_army_talk_to_other_members_item_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_army_talk_to_other_members_item_on_consequence), false, -1, true, null);
			gameSystemInitializer.AddGameMenuOption("game_menu_army_talk_to_other_members", "game_menu_army_talk_to_other_members_back", GameTexts.FindText("str_back", null).ToString(), new GameMenuOption.OnConditionDelegate(this.game_menu_army_talk_to_other_members_back_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_army_talk_to_other_members_back_on_consequence), true, -1, false, null);
			gameSystemInitializer.AddGameMenu("try_to_get_away", "{=bwuawgrW}As the highest tactics skilled member {HIGHEST_TACTICS_SKILLED_MEMBER} ({HIGHEST_TACTICS_SKILL}) devise a plan to disperse into the wilderness to break away from your enemies. You and most men may escape with your lives, but as many as {NEEEDED_MEN_COUNT} {SOLDIER_OR_SOLDIERS} may be lost and part of your baggage could be captured.", new OnInitDelegate(this.game_menu_leave_soldiers_behind_on_init), GameOverlays.MenuOverlayType.Encounter, GameMenu.MenuFlags.None, null);
			gameSystemInitializer.AddGameMenuOption("try_to_get_away", "try_to_get_away_accept", "{=DbOv36TA}Go ahead with that.", new GameMenuOption.OnConditionDelegate(this.game_menu_try_to_get_away_accept_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_encounter_leave_your_soldiers_behind_accept_on_consequence), false, -1, false, null);
			gameSystemInitializer.AddGameMenuOption("try_to_get_away", "try_to_get_away_reject", "{=f1etg9oL}Think of something else.", new GameMenuOption.OnConditionDelegate(this.game_menu_try_to_get_away_reject_on_condition), delegate(MenuCallbackArgs args)
			{
				GameMenu.SwitchToMenu("encounter");
			}, true, -1, false, null);
			gameSystemInitializer.AddGameMenu("try_to_get_away_debrief", "{=ruU70rFl}You disperse into the shrubs and bushes. The enemies halt and seem to hesitate for a while before resuming their pursuit. {CASUALTIES_AND_LOST_BAGGAGE}", new OnInitDelegate(this.game_menu_get_away_debrief_on_init), GameOverlays.MenuOverlayType.Encounter, GameMenu.MenuFlags.None, null);
			gameSystemInitializer.AddGameMenuOption("try_to_get_away_debrief", "try_to_get_away_continue", "{=veWOovVv}Continue...", new GameMenuOption.OnConditionDelegate(this.game_menu_try_to_get_away_continue_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_try_to_get_away_end), false, -1, false, null);
			gameSystemInitializer.AddGameMenu("assault_town", "", new OnInitDelegate(this.game_menu_town_assault_on_init), GameOverlays.MenuOverlayType.None, GameMenu.MenuFlags.None, null);
			gameSystemInitializer.AddGameMenu("assault_town_order_attack", "", new OnInitDelegate(this.game_menu_town_assault_order_attack_on_init), GameOverlays.MenuOverlayType.None, GameMenu.MenuFlags.None, null);
			gameSystemInitializer.AddGameMenu("town_outside", "{=!}{TOWN_TEXT}", new OnInitDelegate(this.game_menu_town_outside_on_init), GameOverlays.MenuOverlayType.None, GameMenu.MenuFlags.None, null);
			gameSystemInitializer.AddGameMenuOption("town_outside", "approach_gates", "{=XlbDnuJx}Approach the gates and hail the guard.", new GameMenuOption.OnConditionDelegate(this.game_menu_castle_outside_approach_gates_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_town_outside_approach_gates_on_consequence), false, -1, false, null);
			gameSystemInitializer.AddGameMenuOption("town_outside", "town_disguise_yourself", "{=VCREeAF1}Disguise yourself and sneak through the gate. ({SNEAK_CHANCE}%)", new GameMenuOption.OnConditionDelegate(this.game_menu_town_disguise_yourself_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_town_disguise_yourself_on_consequence), false, -1, false, null);
			gameSystemInitializer.AddGameMenuOption("town_outside", "town_besiege", "{=WdIGdHuL}Besiege the town.", new GameMenuOption.OnConditionDelegate(this.game_menu_town_town_besiege_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_town_town_besiege_on_consequence), false, -1, false, null);
			gameSystemInitializer.AddGameMenuOption("town_outside", "town_enter_cheat", "{=!}Enter town (Cheat).", new GameMenuOption.OnConditionDelegate(this.game_menu_town_outside_cheat_enter_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_town_outside_enter_on_consequence), false, -1, false, null);
			gameSystemInitializer.AddGameMenuOption("town_outside", "town_outside_leave", "{=2YYRyrOO}Leave...", new GameMenuOption.OnConditionDelegate(this.game_menu_castle_outside_leave_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_castle_outside_leave_on_consequence), true, -1, false, null);
			gameSystemInitializer.AddGameMenu("menu_sneak_into_town_succeeded", "{=pSSDfAjR}Disguised in the garments of a poor pilgrim, you fool the guards and make your way into the town.", null, GameOverlays.MenuOverlayType.None, GameMenu.MenuFlags.None, null);
			gameSystemInitializer.AddGameMenuOption("menu_sneak_into_town_succeeded", "str_continue", "{=DM6luo3c}Continue", new GameMenuOption.OnConditionDelegate(EncounterGameMenuBehavior.menu_sneak_into_town_succeeded_continue_on_condition), new GameMenuOption.OnConsequenceDelegate(EncounterGameMenuBehavior.menu_sneak_into_town_succeeded_continue_on_consequence), false, -1, false, null);
			gameSystemInitializer.AddGameMenu("menu_sneak_into_town_caught", "{=u7yLV7Vr}As you try to sneak in, one of the guards recognizes you and raises the alarm! Another quickly slams the gate shut behind you, and you have no choice but to give up.", new OnInitDelegate(EncounterGameMenuBehavior.game_menu_sneak_into_town_caught_on_init), GameOverlays.MenuOverlayType.None, GameMenu.MenuFlags.None, null);
			gameSystemInitializer.AddGameMenuOption("menu_sneak_into_town_caught", "mno_sneak_caught_surrender", "{=3nT5wWzb}Surrender.", new GameMenuOption.OnConditionDelegate(EncounterGameMenuBehavior.mno_sneak_caught_surrender_on_condition), new GameMenuOption.OnConsequenceDelegate(EncounterGameMenuBehavior.mno_sneak_caught_surrender_on_consequence), false, -1, false, null);
			gameSystemInitializer.AddGameMenu("menu_captivity_castle_taken_prisoner", "{=AFJ3BvTH}You are quickly surrounded by guards who take away your weapons. With curses and insults, they throw you into the dungeon where you must while away the miserable days of your captivity.", null, GameOverlays.MenuOverlayType.None, GameMenu.MenuFlags.None, null);
			gameSystemInitializer.AddGameMenuOption("menu_captivity_castle_taken_prisoner", "mno_sneak_caught_surrender", "{=veWOovVv}Continue...", new GameMenuOption.OnConditionDelegate(EncounterGameMenuBehavior.game_menu_captivity_castle_taken_prisoner_cont_on_condition), new GameMenuOption.OnConsequenceDelegate(EncounterGameMenuBehavior.game_menu_captivity_castle_taken_prisoner_cont_on_consequence), false, -1, false, null);
			gameSystemInitializer.AddGameMenuOption("menu_captivity_castle_taken_prisoner", "cheat_continue", "{=!}Cheat : Leave.", new GameMenuOption.OnConditionDelegate(EncounterGameMenuBehavior.game_menu_captivity_taken_prisoner_cheat_on_condition), new GameMenuOption.OnConsequenceDelegate(EncounterGameMenuBehavior.game_menu_captivity_taken_prisoner_cheat_on_consequence), false, -1, false, null);
			gameSystemInitializer.AddGameMenu("fortification_crime_rating", "{=!}{FORTIFICATION_CRIME_RATING_TEXT}", new OnInitDelegate(this.game_menu_fortification_high_crime_rating_on_init), GameOverlays.MenuOverlayType.None, GameMenu.MenuFlags.None, null);
			gameSystemInitializer.AddGameMenuOption("fortification_crime_rating", "fortification_crime_rating_continue", "{=WVkc4UgX}Continue.", new GameMenuOption.OnConditionDelegate(this.game_menu_fortification_high_crime_rating_continue_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_fortification_high_crime_rating_continue_on_consequence), false, -1, false, null);
			gameSystemInitializer.AddGameMenu("army_left_settlement_due_to_war_declaration", "{=!}{ARMY_LEFT_SETTLEMENT_DUE_TO_WAR_TEXT}", new OnInitDelegate(this.game_menu_army_left_settlement_due_to_war_on_init), GameOverlays.MenuOverlayType.None, GameMenu.MenuFlags.None, null);
			gameSystemInitializer.AddGameMenuOption("army_left_settlement_due_to_war_declaration", "army_left_settlement_due_to_war_declaration_continue", "{=WVkc4UgX}Continue.", new GameMenuOption.OnConditionDelegate(this.game_menu_army_left_settlement_due_to_war_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_army_left_settlement_due_to_war_on_consequence), false, -1, false, null);
			gameSystemInitializer.AddGameMenu("castle_outside", "{=!}{TOWN_TEXT}", new OnInitDelegate(this.game_menu_castle_outside_on_init), GameOverlays.MenuOverlayType.None, GameMenu.MenuFlags.None, null);
			gameSystemInitializer.AddGameMenuOption("castle_outside", "approach_gates", "{=XlbDnuJx}Approach the gates and hail the guard.", new GameMenuOption.OnConditionDelegate(this.game_menu_castle_outside_approach_gates_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_castle_outside_approach_gates_on_consequence), false, -1, false, null);
			gameSystemInitializer.AddGameMenuOption("castle_outside", "town_besiege", "{=UzMYZgoE}Besiege the castle.", new GameMenuOption.OnConditionDelegate(this.game_menu_town_town_besiege_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_town_town_besiege_on_consequence), false, -1, false, null);
			gameSystemInitializer.AddGameMenuOption("castle_outside", "town_outside_leave", "{=2YYRyrOO}Leave...", new GameMenuOption.OnConditionDelegate(this.game_menu_castle_outside_leave_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_castle_outside_leave_on_consequence), true, -1, false, null);
			gameSystemInitializer.AddGameMenu("town_guard", "{=SxkaQbSa}You approach the gate. The men on the walls watch you closely.", null, GameOverlays.MenuOverlayType.None, GameMenu.MenuFlags.None, null);
			gameSystemInitializer.AddGameMenuOption("town_guard", "request_meeting_commander", "{=RSQbOjub}Request a meeting with someone.", new GameMenuOption.OnConditionDelegate(this.game_menu_request_meeting_someone_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_request_meeting_someone_on_consequence), false, -1, false, null);
			gameSystemInitializer.AddGameMenuOption("town_guard", "guard_discuss_criminal_surrender", "{=ACvQdkG8}Discuss the terms of your surrender", new GameMenuOption.OnConditionDelegate(this.outside_menu_criminal_on_condition), new GameMenuOption.OnConsequenceDelegate(this.outside_menu_criminal_on_consequence), false, -1, false, null);
			gameSystemInitializer.AddGameMenuOption("town_guard", "guard_back", GameTexts.FindText("str_back", null).ToString(), new GameMenuOption.OnConditionDelegate(this.game_menu_castle_outside_leave_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_town_guard_back_on_consequence), true, -1, false, null);
			gameSystemInitializer.AddGameMenu("castle_guard", "{=SxkaQbSa}You approach the gate. The men on the walls watch you closely.", null, GameOverlays.MenuOverlayType.None, GameMenu.MenuFlags.None, null);
			gameSystemInitializer.AddGameMenuOption("castle_guard", "request_shelter", "{=mG9jW8Fp}Request entry to the castle.", new GameMenuOption.OnConditionDelegate(this.game_menu_town_guard_request_shelter_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_request_entry_to_castle_on_consequence), false, -1, false, null);
			gameSystemInitializer.AddGameMenuOption("castle_guard", "request_meeting_commander", "{=RSQbOjub}Request a meeting with someone.", new GameMenuOption.OnConditionDelegate(this.game_menu_request_meeting_someone_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_request_meeting_someone_on_consequence), false, -1, false, null);
			gameSystemInitializer.AddGameMenuOption("castle_guard", "guard_back", GameTexts.FindText("str_back", null).ToString(), new GameMenuOption.OnConditionDelegate(this.game_menu_castle_outside_leave_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_town_guard_back_on_consequence), true, -1, false, null);
			gameSystemInitializer.AddGameMenu("menu_castle_entry_granted", "{=Mg1PotzO}After a brief wait, the guards open the gates for you and allow your party inside.", null, GameOverlays.MenuOverlayType.None, GameMenu.MenuFlags.None, null);
			gameSystemInitializer.AddGameMenuOption("menu_castle_entry_granted", "str_continue", "{=bLNocKd1}Continue..", new GameMenuOption.OnConditionDelegate(EncounterGameMenuBehavior.game_request_entry_to_castle_approved_continue_on_condition), new GameMenuOption.OnConsequenceDelegate(EncounterGameMenuBehavior.game_request_entry_to_castle_approved_continue_on_consequence), false, -1, false, null);
			gameSystemInitializer.AddGameMenu("menu_castle_entry_denied", "{=QpQQJjD6}The lord of this castle has forbidden you from coming inside these walls, and the guard sergeant informs you that his men will fire if you attempt to come any closer.", new OnInitDelegate(EncounterGameMenuBehavior.menu_castle_entry_denied_on_init), GameOverlays.MenuOverlayType.None, GameMenu.MenuFlags.None, null);
			gameSystemInitializer.AddGameMenuOption("menu_castle_entry_denied", "str_continue", "{=veWOovVv}Continue...", null, new GameMenuOption.OnConsequenceDelegate(EncounterGameMenuBehavior.game_request_entry_to_castle_rejected_continue_on_consequence), false, -1, false, null);
			gameSystemInitializer.AddGameMenu("request_meeting", "{=pBAx7jTM}With whom do you want to meet?", new OnInitDelegate(this.game_menu_town_menu_request_meeting_on_init), GameOverlays.MenuOverlayType.None, GameMenu.MenuFlags.None, null);
			gameSystemInitializer.AddGameMenuOption("request_meeting", "request_meeting_with", "{=!}{HERO_TO_MEET.LINK}", new GameMenuOption.OnConditionDelegate(this.game_menu_request_meeting_with_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_request_meeting_with_on_consequence), false, -1, true, null);
			gameSystemInitializer.AddGameMenuOption("request_meeting", "meeting_town_leave", "{=3nbuRBJK}Forget it.", new GameMenuOption.OnConditionDelegate(this.game_meeting_town_leave_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_request_meeting_town_leave_on_consequence), true, -1, false, null);
			gameSystemInitializer.AddGameMenuOption("request_meeting", "meeting_castle_leave", "{=3nbuRBJK}Forget it.", new GameMenuOption.OnConditionDelegate(this.game_meeting_castle_leave_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_request_meeting_castle_leave_on_consequence), true, -1, false, null);
			gameSystemInitializer.AddGameMenu("request_meeting_with_besiegers", "{=pBAx7jTM}With whom do you want to meet?", new OnInitDelegate(this.game_menu_town_menu_request_meeting_with_besiegers_on_init), GameOverlays.MenuOverlayType.None, GameMenu.MenuFlags.None, null);
			gameSystemInitializer.AddGameMenuOption("request_meeting_with_besiegers", "request_meeting_with", "{=!}{PARTY_LEADER.LINK}", new GameMenuOption.OnConditionDelegate(this.game_menu_request_meeting_with_besiegers_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_request_meeting_with_besiegers_on_consequence), false, -1, true, null);
			gameSystemInitializer.AddGameMenuOption("request_meeting_with_besiegers", "request_meeting_town_leave", "{=3nbuRBJK}Forget it.", new GameMenuOption.OnConditionDelegate(this.game_meeting_town_leave_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_request_meeting_town_leave_on_consequence), true, -1, false, null);
			gameSystemInitializer.AddGameMenuOption("request_meeting_with_besiegers", "request_meeting_castle_leave", "{=3nbuRBJK}Forget it.", new GameMenuOption.OnConditionDelegate(this.game_meeting_castle_leave_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_request_meeting_castle_leave_on_consequence), true, -1, false, null);
			gameSystemInitializer.AddGameMenu("village_outside", "{=!}.", new OnInitDelegate(this.VillageOutsideOnInit), GameOverlays.MenuOverlayType.None, GameMenu.MenuFlags.None, null);
			gameSystemInitializer.AddGameMenu("village_loot_complete", "{=qt5bkw8l}On your orders your troops sack the village, pillaging everything of any value, and then put the buildings to the torch. From the coins and valuables that are found, you get your share.", new OnInitDelegate(this.game_menu_village_loot_complete_on_init), GameOverlays.MenuOverlayType.None, GameMenu.MenuFlags.None, null);
			gameSystemInitializer.AddGameMenuOption("village_loot_complete", "continue", "{=veWOovVv}Continue...", new GameMenuOption.OnConditionDelegate(this.game_menu_village_loot_complete_continue_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_village_loot_complete_continue_on_consequence), false, -1, false, null);
			gameSystemInitializer.AddGameMenu("raid_interrupted", "{=KW7amS8c}While your troops are pillaging the countryside, you receive news that the enemy is approaching. You quickly gather up your soldiers and prepare for battle.", null, GameOverlays.MenuOverlayType.None, GameMenu.MenuFlags.None, null);
			gameSystemInitializer.AddGameMenuOption("raid_interrupted", "continue", "{=veWOovVv}Continue...", new GameMenuOption.OnConditionDelegate(this.game_menu_raid_interrupted_continue_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_raid_interrupted_continue_on_consequence), false, -1, false, null);
			gameSystemInitializer.AddGameMenu("encounter_interrupted", "{=lKWflUid}While you are waiting in {DEFENDER}, {ATTACKER} started an attack on it.", new OnInitDelegate(this.game_menu_encounter_interrupted_on_init), GameOverlays.MenuOverlayType.None, GameMenu.MenuFlags.None, null);
			gameSystemInitializer.AddGameMenuOption("encounter_interrupted", "encounter_interrupted_help_attackers", "{=h3yEHb4U}Help {ATTACKER}.", new GameMenuOption.OnConditionDelegate(this.game_menu_join_encounter_help_attackers_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_join_encounter_help_attackers_on_consequence), false, -1, false, null);
			gameSystemInitializer.AddGameMenuOption("encounter_interrupted", "encounter_interrupted_help_defenders", "{=FwIgakj8}Help {DEFENDER}.", new GameMenuOption.OnConditionDelegate(this.game_menu_join_encounter_help_defenders_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_join_encounter_help_defenders_on_consequence), false, -1, false, null);
			gameSystemInitializer.AddGameMenuOption("encounter_interrupted", "leave", "{=UgfmaQgx}Leave {DEFENDER}", new GameMenuOption.OnConditionDelegate(this.game_menu_encounter_interrupted_leave_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_encounter_interrupted_leave_on_consequence), true, -1, false, null);
			gameSystemInitializer.AddGameMenu("encounter_interrupted_siege_preparations", "{=ABeCWcLi}While you are resting, you hear news that a force led by {ATTACKER} has arrived outside the walls of {DEFENDER} and is beginning preparations for a siege.", new OnInitDelegate(this.game_menu_encounter_interrupted_siege_preparations_on_init), GameOverlays.MenuOverlayType.None, GameMenu.MenuFlags.None, null);
			gameSystemInitializer.AddGameMenuOption("encounter_interrupted_siege_preparations", "encounter_interrupted_siege_preparations_join_defend", "{=Lxx97yNh}Join the defense of {SETTLEMENT}", new GameMenuOption.OnConditionDelegate(this.game_menu_encounter_interrupted_siege_preparations_join_defend_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_encounter_interrupted_siege_preparations_join_defend_on_consequence), false, -1, false, null);
			gameSystemInitializer.AddGameMenuOption("encounter_interrupted_siege_preparations", "encounter_interrupted_siege_preparations_break_out_of_town", "{=ybzBF59f}Break out of {SETTLEMENT}.", new GameMenuOption.OnConditionDelegate(this.game_menu_encounter_interrupted_siege_preparations_break_out_of_town_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_encounter_interrupted_break_out_of_town_on_consequence), false, -1, false, null);
			gameSystemInitializer.AddGameMenuOption("encounter_interrupted_siege_preparations", "encounter_interrupted_siege_preparations_leave_town", "{=FILG5eZD}Leave {SETTLEMENT}.", new GameMenuOption.OnConditionDelegate(this.game_menu_encounter_interrupted_siege_preparations_leave_town_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_encounter_interrupted_leave_on_consequence), true, -1, false, null);
			gameSystemInitializer.AddGameMenu("encounter_interrupted_raid_started", "{=7o4AfEhN}While you are resting, you hear news that a force led by {ATTACKER} has arrived outside of {DEFENDER} to raid it.", new OnInitDelegate(this.game_menu_encounter_interrupted_by_raid_on_init), GameOverlays.MenuOverlayType.None, GameMenu.MenuFlags.None, null);
			gameSystemInitializer.AddGameMenuOption("encounter_interrupted_raid_started", "encounter_interrupted_raid_started_leave", "{=WVkc4UgX}Continue.", new GameMenuOption.OnConditionDelegate(this.game_menu_encounter_interrupted_by_raid_continue_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_encounter_interrupted_continue_on_consequence), false, -1, false, null);
			gameSystemInitializer.AddGameMenu("continue_siege_after_attack", "{=CVp0j9al}You have defeated the enemies outside the walls. Now you decide to...", null, GameOverlays.MenuOverlayType.None, GameMenu.MenuFlags.None, null);
			gameSystemInitializer.AddGameMenuOption("continue_siege_after_attack", "continue_siege", "{=zeKvSEpN}Continue the siege", new GameMenuOption.OnConditionDelegate(this.continue_siege_after_attack_on_condition), new GameMenuOption.OnConsequenceDelegate(this.continue_siege_after_attack_on_consequence), false, -1, false, null);
			gameSystemInitializer.AddGameMenuOption("continue_siege_after_attack", "leave_siege", "{=b7UHp4J9}Leave the siege", new GameMenuOption.OnConditionDelegate(this.leave_siege_after_attack_on_condition), new GameMenuOption.OnConsequenceDelegate(this.leave_siege_after_attack_on_consequence), true, -1, false, null);
			gameSystemInitializer.AddGameMenuOption("continue_siege_after_attack", "leave_army", "{=hSdJ0UUv}Leave Army", new GameMenuOption.OnConditionDelegate(this.leave_army_after_attack_on_condition), new GameMenuOption.OnConsequenceDelegate(this.leave_army_after_attack_on_consequence), true, -1, false, null);
			gameSystemInitializer.AddGameMenu("town_caught_by_guards", "{=gVuF84RZ}Guards caught you", null, GameOverlays.MenuOverlayType.None, GameMenu.MenuFlags.None, null);
			gameSystemInitializer.AddGameMenuOption("town_caught_by_guards", "town_caught_by_guards_criminal_outside_menu_give_yourself_up", "{=DM6luo3c}Continue", new GameMenuOption.OnConditionDelegate(this.outside_menu_criminal_on_condition), new GameMenuOption.OnConsequenceDelegate(this.caught_outside_menu_criminal_on_consequence), false, -1, false, null);
			gameSystemInitializer.AddGameMenuOption("town_caught_by_guards", "town_caught_by_guards_enemy_outside_menu_give_yourself_up", "{=DM6luo3c}Continue", new GameMenuOption.OnConditionDelegate(this.caught_outside_menu_enemy_on_condition), new GameMenuOption.OnConsequenceDelegate(this.caught_outside_menu_enemy_on_consequence), false, -1, false, null);
			gameSystemInitializer.AddGameMenu("break_in_menu", "{=sd15CQHI}You devised a plan to distract the besiegers so you can rush the fortress gates, expecting the defenders to let you in. You and most of your men may get through, but as many as {POSSIBLE_CASUALTIES} {?PLURAL}troops{?}troop{\\?} may be lost.", new OnInitDelegate(this.break_in_out_menu_on_init), GameOverlays.MenuOverlayType.Encounter, GameMenu.MenuFlags.None, null);
			gameSystemInitializer.AddGameMenuOption("break_in_menu", "break_in_menu_accept", "{=DbOv36TA}Go ahead with that.", new GameMenuOption.OnConditionDelegate(this.break_in_menu_accept_on_condition), new GameMenuOption.OnConsequenceDelegate(this.break_in_menu_accept_on_consequence), false, -1, false, null);
			gameSystemInitializer.AddGameMenuOption("break_in_menu", "break_in_menu_reject", "{=f1etg9oL}Think of something else.", new GameMenuOption.OnConditionDelegate(this.break_in_menu_reject_on_condition), new GameMenuOption.OnConsequenceDelegate(this.break_in_menu_reject_on_consequence), false, -1, false, null);
			gameSystemInitializer.AddGameMenu("break_in_debrief_menu", "{=PHe0oco1}You fought your way through the attackers to reach the gates. The defenders open them quickly to let you through. When the gates are safely closed behind you, you take a quick tally only to see you have lost the following: {CASUALTIES}.{OTHER_CASUALTIES}", new OnInitDelegate(this.break_in_out_debrief_menu_on_init), GameOverlays.MenuOverlayType.None, GameMenu.MenuFlags.None, null);
			gameSystemInitializer.AddGameMenuOption("break_in_debrief_menu", "break_in_debrief_continue", "{=DM6luo3c}Continue", new GameMenuOption.OnConditionDelegate(this.break_in_debrief_continue_on_condition), new GameMenuOption.OnConsequenceDelegate(this.break_in_debrief_continue_on_consequence), false, -1, false, null);
			gameSystemInitializer.AddGameMenu("break_out_menu", "{=J1aEaygO}You devised a plan to fight your way through the attackers to escape from the fortress. You and most of your men may survive, but as many as {POSSIBLE_CASUALTIES} {?PLURAL}troops{?}troop{\\?} may be lost.", new OnInitDelegate(this.break_in_out_menu_on_init), GameOverlays.MenuOverlayType.Encounter, GameMenu.MenuFlags.None, null);
			gameSystemInitializer.AddGameMenuOption("break_out_menu", "break_out_menu_accept", "{=DbOv36TA}Go ahead with that.", new GameMenuOption.OnConditionDelegate(this.break_out_menu_accept_on_condition), new GameMenuOption.OnConsequenceDelegate(this.break_out_menu_accept_on_consequence), false, -1, false, null);
			gameSystemInitializer.AddGameMenuOption("break_out_menu", "break_out_menu_reject", "{=f1etg9oL}Think of something else.", new GameMenuOption.OnConditionDelegate(this.break_out_menu_reject_on_condition), new GameMenuOption.OnConsequenceDelegate(this.break_out_menu_reject_on_consequence), false, -1, false, null);
			gameSystemInitializer.AddGameMenu("break_out_debrief_menu", "{=OzyrsZZK}You fought your way through the attackers to escape from the settlement. When, after some time your forces regroup, you take a quick tally only to see you have lost the following: {CASUALTIES}.{OTHER_CASUALTIES}", new OnInitDelegate(this.break_in_out_debrief_menu_on_init), GameOverlays.MenuOverlayType.None, GameMenu.MenuFlags.None, null);
			gameSystemInitializer.AddGameMenuOption("break_out_debrief_menu", "break_out_debrief_continue", "{=DM6luo3c}Continue", new GameMenuOption.OnConditionDelegate(this.break_out_debrief_continue_on_condition), new GameMenuOption.OnConsequenceDelegate(this.break_out_debrief_continue_on_consequence), false, -1, false, null);
		}

		// Token: 0x060035B7 RID: 13751 RVA: 0x000EF4F4 File Offset: 0x000ED6F4
		private bool game_menu_encounter_army_lead_inf_on_condition(MenuCallbackArgs args)
		{
			bool flag = MobileParty.MainParty.MapEvent != null && MobileParty.MainParty.MapEvent.PlayerSide == BattleSideEnum.Attacker && MobileParty.MainParty.MapEvent.DefenderSide.TroopCount == 0;
			if (MobileParty.MainParty.MapEvent != null && PlayerEncounter.CheckIfLeadingAvaliable() && !flag)
			{
				return MobileParty.MainParty.MapEvent.PartiesOnSide(MobileParty.MainParty.MapEvent.PlayerSide).Any((MapEventParty party) => party.Party.MemberRoster.GetTroopRoster().Any((TroopRosterElement tr) => tr.Character != null && tr.Character.GetFormationClass() == FormationClass.Infantry));
			}
			return false;
		}

		// Token: 0x060035B8 RID: 13752 RVA: 0x000EF594 File Offset: 0x000ED794
		private void game_menu_encounter_army_lead_inf_on_consequence(MenuCallbackArgs args)
		{
			this.game_menu_encounter_attack_on_consequence(args);
		}

		// Token: 0x060035B9 RID: 13753 RVA: 0x000EF5A0 File Offset: 0x000ED7A0
		private bool game_menu_encounter_army_lead_arc_on_condition(MenuCallbackArgs args)
		{
			bool flag = MobileParty.MainParty.MapEvent != null && MobileParty.MainParty.MapEvent.PlayerSide == BattleSideEnum.Attacker && MobileParty.MainParty.MapEvent.DefenderSide.TroopCount == 0;
			if (MobileParty.MainParty.MapEvent != null && PlayerEncounter.CheckIfLeadingAvaliable() && !flag)
			{
				return MobileParty.MainParty.MapEvent.PartiesOnSide(MobileParty.MainParty.MapEvent.PlayerSide).Any((MapEventParty party) => party.Party.MemberRoster.GetTroopRoster().Any((TroopRosterElement tr) => tr.Character != null && tr.Character.GetFormationClass() == FormationClass.Ranged));
			}
			return false;
		}

		// Token: 0x060035BA RID: 13754 RVA: 0x000EF640 File Offset: 0x000ED840
		private void game_menu_encounter_army_lead_arc_on_consequence(MenuCallbackArgs args)
		{
			this.game_menu_encounter_attack_on_consequence(args);
		}

		// Token: 0x060035BB RID: 13755 RVA: 0x000EF64C File Offset: 0x000ED84C
		private bool game_menu_encounter_army_lead_cav_on_condition(MenuCallbackArgs args)
		{
			bool flag = MobileParty.MainParty.MapEvent != null && MobileParty.MainParty.MapEvent.PlayerSide == BattleSideEnum.Attacker && MobileParty.MainParty.MapEvent.DefenderSide.TroopCount == 0;
			if (MobileParty.MainParty.MapEvent != null && PlayerEncounter.CheckIfLeadingAvaliable() && !flag)
			{
				return MobileParty.MainParty.MapEvent.PartiesOnSide(MobileParty.MainParty.MapEvent.PlayerSide).Any((MapEventParty party) => party.Party.MemberRoster.GetTroopRoster().Any((TroopRosterElement tr) => tr.Character != null && tr.Character.GetFormationClass() == FormationClass.Cavalry));
			}
			return false;
		}

		// Token: 0x060035BC RID: 13756 RVA: 0x000EF6EC File Offset: 0x000ED8EC
		private void game_menu_encounter_army_lead_cav_on_consequence(MenuCallbackArgs args)
		{
			this.game_menu_encounter_attack_on_consequence(args);
		}

		// Token: 0x060035BD RID: 13757 RVA: 0x000EF6F5 File Offset: 0x000ED8F5
		public static void game_menu_captivity_taken_prisoner_cheat_on_consequence(MenuCallbackArgs args)
		{
			PlayerEncounter.Finish(true);
		}

		// Token: 0x060035BE RID: 13758 RVA: 0x000EF700 File Offset: 0x000ED900
		private bool game_menu_encounter_army_lead_har_on_condition(MenuCallbackArgs args)
		{
			bool flag = MobileParty.MainParty.MapEvent != null && MobileParty.MainParty.MapEvent.PlayerSide == BattleSideEnum.Attacker && MobileParty.MainParty.MapEvent.DefenderSide.TroopCount == 0;
			if (MobileParty.MainParty.MapEvent != null && PlayerEncounter.CheckIfLeadingAvaliable() && !flag)
			{
				return MobileParty.MainParty.MapEvent.PartiesOnSide(MobileParty.MainParty.MapEvent.PlayerSide).Any((MapEventParty party) => party.Party.MemberRoster.GetTroopRoster().Any((TroopRosterElement tr) => tr.Character != null && tr.Character.GetFormationClass() == FormationClass.HorseArcher));
			}
			return false;
		}

		// Token: 0x060035BF RID: 13759 RVA: 0x000EF7A0 File Offset: 0x000ED9A0
		private void game_menu_encounter_army_lead_har_on_consequence(MenuCallbackArgs args)
		{
			this.game_menu_encounter_attack_on_consequence(args);
		}

		// Token: 0x060035C0 RID: 13760 RVA: 0x000EF7AC File Offset: 0x000ED9AC
		private void game_menu_join_encounter_on_init(MenuCallbackArgs args)
		{
			MapEvent encounteredBattle = PlayerEncounter.EncounteredBattle;
			PartyBase leaderParty = encounteredBattle.GetLeaderParty(BattleSideEnum.Attacker);
			PartyBase leaderParty2 = encounteredBattle.GetLeaderParty(BattleSideEnum.Defender);
			if (leaderParty.IsMobile && leaderParty.MobileParty.Army != null)
			{
				MBTextManager.SetTextVariable("ATTACKER", leaderParty.MobileParty.ArmyName, false);
			}
			else
			{
				MBTextManager.SetTextVariable("ATTACKER", leaderParty.Name, false);
			}
			if (leaderParty2.IsMobile && leaderParty2.MobileParty.Army != null)
			{
				MBTextManager.SetTextVariable("DEFENDER", leaderParty2.MobileParty.ArmyName, false);
			}
			else
			{
				MBTextManager.SetTextVariable("DEFENDER", leaderParty2.Name, false);
			}
			if (encounteredBattle.IsSallyOut)
			{
				MBTextManager.SetTextVariable("JOIN_ENCOUNTER_TEXT", GameTexts.FindText("str_defenders_make_sally_out", null), false);
				StringHelpers.SetCharacterProperties("BESIEGER_LEADER", Campaign.Current.Models.EncounterModel.GetLeaderOfMapEvent(encounteredBattle, BattleSideEnum.Defender).CharacterObject, null, false);
				return;
			}
			if (leaderParty2.IsSettlement)
			{
				TextObject textObject = new TextObject("{=kDiN9iYw}{ATTACKER} is besieging the walls of {DEFENDER}", null);
				if (encounteredBattle.IsSiegeAssault)
				{
					Settlement.SiegeState currentSiegeState = leaderParty2.Settlement.CurrentSiegeState;
					if (currentSiegeState != Settlement.SiegeState.OnTheWalls && currentSiegeState == Settlement.SiegeState.InTheLordsHall)
					{
						textObject = new TextObject("{=oXY2wnic}{ATTACKER} is fighting inside the lord's hall of {DEFENDER}", null);
					}
				}
				else if (encounteredBattle.IsRaid)
				{
					if (encounteredBattle.DefenderSide.TroopCount > 0)
					{
						textObject = new TextObject("{=kvNQLcCb}{ATTACKER} is fighting in {DEFENDER}", null);
					}
					else
					{
						textObject = new TextObject("{=BExNNwm0}{ATTACKER} is raiding {DEFENDER}", null);
					}
				}
				MBTextManager.SetTextVariable("JOIN_ENCOUNTER_TEXT", textObject, false);
				return;
			}
			MBTextManager.SetTextVariable("JOIN_ENCOUNTER_TEXT", GameTexts.FindText("str_come_across_battle", null), false);
		}

		// Token: 0x060035C1 RID: 13761 RVA: 0x000EF928 File Offset: 0x000EDB28
		private bool game_menu_join_encounter_help_attackers_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.DefendAction;
			MapEvent encounteredBattle = PlayerEncounter.EncounteredBattle;
			IFaction mapFaction = encounteredBattle.GetLeaderParty(BattleSideEnum.Defender).MapFaction;
			this.CheckFactionAttackableHonorably(args, mapFaction);
			return MapEventHelper.CanPartyJoinBattle(PartyBase.MainParty, encounteredBattle, BattleSideEnum.Attacker);
		}

		// Token: 0x060035C2 RID: 13762 RVA: 0x000EF964 File Offset: 0x000EDB64
		private void game_menu_join_encounter_help_attackers_on_consequence(MenuCallbackArgs args)
		{
			if (PlayerEncounter.InsideSettlement && PlayerEncounter.EncounterSettlement.IsUnderSiege)
			{
				PlayerEncounter.LeaveSettlement();
			}
			PlayerEncounter.JoinBattle(BattleSideEnum.Attacker);
			if (PlayerEncounter.Battle.DefenderSide.TroopCount > 0)
			{
				GameMenu.SwitchToMenu("encounter");
				return;
			}
			if (MobileParty.MainParty.Army != null)
			{
				if (MobileParty.MainParty.Army.LeaderParty != MobileParty.MainParty)
				{
					if (!MobileParty.MainParty.Army.LeaderParty.AttachedParties.Contains(MobileParty.MainParty))
					{
						MobileParty.MainParty.Army.AddPartyToMergedParties(MobileParty.MainParty);
						Campaign.Current.CameraFollowParty = MobileParty.MainParty.Army.LeaderParty.Party;
						CampaignEventDispatcher.Instance.OnArmyOverlaySetDirty();
					}
					if (PlayerEncounter.Battle.IsRaid)
					{
						GameMenu.SwitchToMenu("raiding_village");
						return;
					}
					GameMenu.SwitchToMenu("army_wait");
					return;
				}
				else
				{
					if (PlayerEncounter.Battle.IsRaid)
					{
						GameMenu.SwitchToMenu("raiding_village");
						MobileParty.MainParty.Ai.SetMoveModeHold();
						return;
					}
					GameMenu.SwitchToMenu("encounter");
					return;
				}
			}
			else
			{
				if (PlayerEncounter.Battle.IsRaid)
				{
					GameMenu.SwitchToMenu("raiding_village");
					MobileParty.MainParty.Ai.SetMoveModeHold();
					return;
				}
				GameMenu.SwitchToMenu("menu_siege_strategies");
				MobileParty.MainParty.Ai.SetMoveModeHold();
				return;
			}
		}

		// Token: 0x060035C3 RID: 13763 RVA: 0x000EFABF File Offset: 0x000EDCBF
		private bool game_menu_join_encounter_abandon_army_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Leave;
			return MobileParty.MainParty.Army != null && MobileParty.MainParty.Army.LeaderParty != MobileParty.MainParty;
		}

		// Token: 0x060035C4 RID: 13764 RVA: 0x000EFAF0 File Offset: 0x000EDCF0
		private bool game_menu_join_encounter_help_defenders_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.DefendAction;
			MapEvent encounteredBattle = PlayerEncounter.EncounteredBattle;
			IFaction mapFaction = encounteredBattle.GetLeaderParty(BattleSideEnum.Attacker).MapFaction;
			this.CheckFactionAttackableHonorably(args, mapFaction);
			return MapEventHelper.CanPartyJoinBattle(PartyBase.MainParty, encounteredBattle, BattleSideEnum.Defender);
		}

		// Token: 0x060035C5 RID: 13765 RVA: 0x000EFB2C File Offset: 0x000EDD2C
		public static bool game_menu_captivity_castle_taken_prisoner_cont_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Continue;
			return true;
		}

		// Token: 0x060035C6 RID: 13766 RVA: 0x000EFB38 File Offset: 0x000EDD38
		private void game_menu_join_encounter_help_defenders_on_consequence(MenuCallbackArgs args)
		{
			PartyBase encounteredParty = PlayerEncounter.EncounteredParty;
			if (((encounteredParty != null) ? encounteredParty.MapEvent : null) != null)
			{
				PlayerEncounter.JoinBattle(BattleSideEnum.Defender);
				GameMenu.ActivateGameMenu("encounter");
				return;
			}
			if (PlayerEncounter.Current != null)
			{
				if (PlayerEncounter.EncounterSettlement != null && PlayerEncounter.EncounterSettlement.SiegeEvent != null && !PlayerEncounter.EncounterSettlement.MapFaction.IsAtWarWith(MobileParty.MainParty.MapFaction))
				{
					PlayerEncounter.RestartPlayerEncounter(PlayerEncounter.EncounterSettlement.SiegeEvent.BesiegerCamp.BesiegerParty.Party, PartyBase.MainParty, false);
				}
				GameMenu.ActivateGameMenu("encounter");
			}
		}

		// Token: 0x060035C7 RID: 13767 RVA: 0x000EFBCC File Offset: 0x000EDDCC
		private void game_menu_join_siege_event_on_init(MenuCallbackArgs args)
		{
		}

		// Token: 0x060035C8 RID: 13768 RVA: 0x000EFBCE File Offset: 0x000EDDCE
		private void game_menu_join_sally_out_on_init(MenuCallbackArgs args)
		{
			if (PlayerEncounter.Current != null)
			{
				PlayerEncounter.Current.IsPlayerWaiting = false;
			}
		}

		// Token: 0x060035C9 RID: 13769 RVA: 0x000EFBE4 File Offset: 0x000EDDE4
		private bool game_menu_join_siege_event_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.HostileAction;
			TextObject textObject;
			if (DiplomacyHelper.DidMainHeroSwornNotToAttackFaction(Settlement.CurrentSettlement.MapFaction, out textObject))
			{
				args.IsEnabled = false;
				args.Tooltip = textObject;
			}
			return Settlement.CurrentSettlement.SiegeEvent.CanPartyJoinSide(MobileParty.MainParty.Party, BattleSideEnum.Attacker);
		}

		// Token: 0x060035CA RID: 13770 RVA: 0x000EFC34 File Offset: 0x000EDE34
		private void game_menu_join_siege_event_on_consequence(MenuCallbackArgs args)
		{
			if (!Settlement.CurrentSettlement.SiegeEvent.BesiegerCamp.BesiegerParty.IsMainParty && !Settlement.CurrentSettlement.SiegeEvent.CanPartyJoinSide(MobileParty.MainParty.Party, BattleSideEnum.Attacker))
			{
				Debug.FailedAssert("Player should not be able to join this siege.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\CampaignBehaviors\\EncounterGameMenuBehavior.cs", "game_menu_join_siege_event_on_consequence", 661);
				return;
			}
			MobileParty besiegerParty = Settlement.CurrentSettlement.SiegeEvent.BesiegerCamp.BesiegerParty;
			if (!besiegerParty.IsMainParty)
			{
				MobileParty.MainParty.Ai.SetMoveEscortParty(besiegerParty);
			}
			if (Settlement.CurrentSettlement.Party.MapEvent != null)
			{
				PlayerEncounter.JoinBattle(BattleSideEnum.Attacker);
				GameMenu.SwitchToMenu("encounter");
				return;
			}
			Settlement currentSettlement = Settlement.CurrentSettlement;
			if (Hero.MainHero.CurrentSettlement != null)
			{
				PlayerEncounter.LeaveSettlement();
			}
			PlayerEncounter.Finish(true);
			MobileParty.MainParty.BesiegerCamp = currentSettlement.SiegeEvent.BesiegerCamp;
			PlayerSiege.StartPlayerSiege(BattleSideEnum.Attacker, false, currentSettlement);
			PlayerSiege.StartSiegePreparation();
			Campaign.Current.TimeControlMode = CampaignTimeControlMode.UnstoppablePlay;
		}

		// Token: 0x060035CB RID: 13771 RVA: 0x000EFD2E File Offset: 0x000EDF2E
		private bool game_menu_join_sally_out_event_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.HostileAction;
			return true;
		}

		// Token: 0x060035CC RID: 13772 RVA: 0x000EFD39 File Offset: 0x000EDF39
		private bool game_menu_stay_in_settlement_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Leave;
			return true;
		}

		// Token: 0x060035CD RID: 13773 RVA: 0x000EFD44 File Offset: 0x000EDF44
		private void game_menu_join_sally_out_on_consequence(MenuCallbackArgs args)
		{
			EncounterManager.StartPartyEncounter(MobileParty.MainParty.Party, MobileParty.MainParty.CurrentSettlement.Town.GarrisonParty.MapEvent.DefenderSide.LeaderParty);
		}

		// Token: 0x060035CE RID: 13774 RVA: 0x000EFD78 File Offset: 0x000EDF78
		private void game_menu_stay_in_settlement_on_consequence(MenuCallbackArgs args)
		{
			GameMenu.SwitchToMenu("menu_siege_strategies");
			if (PlayerEncounter.Current != null)
			{
				PlayerEncounter.Current.IsPlayerWaiting = false;
			}
		}

		// Token: 0x060035CF RID: 13775 RVA: 0x000EFD96 File Offset: 0x000EDF96
		private bool break_in_to_help_defender_side_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.DefendAction;
			return this.common_join_siege_event_button_condition(args);
		}

		// Token: 0x060035D0 RID: 13776 RVA: 0x000EFDA8 File Offset: 0x000EDFA8
		private bool common_join_siege_event_button_condition(MenuCallbackArgs args)
		{
			MobileParty mainParty = MobileParty.MainParty;
			SiegeEvent siegeEvent = Settlement.CurrentSettlement.SiegeEvent;
			int lostTroopCountForBreakingInBesiegedSettlement = Campaign.Current.Models.TroopSacrificeModel.GetLostTroopCountForBreakingInBesiegedSettlement(mainParty, siegeEvent);
			Army army = mainParty.Army;
			int num = ((army != null) ? army.TotalRegularCount : mainParty.MemberRoster.TotalRegulars);
			TextObject textObject;
			if (DiplomacyHelper.DidMainHeroSwornNotToAttackFaction(siegeEvent.BesiegerCamp.BesiegerParty.MapFaction, out textObject))
			{
				args.IsEnabled = false;
				args.Tooltip = textObject;
			}
			else if (lostTroopCountForBreakingInBesiegedSettlement > num)
			{
				args.IsEnabled = false;
				args.Tooltip = new TextObject("{=MTbOGRCF}You don't have enough men!", null);
			}
			return siegeEvent.CanPartyJoinSide(MobileParty.MainParty.Party, BattleSideEnum.Defender);
		}

		// Token: 0x060035D1 RID: 13777 RVA: 0x000EFE51 File Offset: 0x000EE051
		private bool attack_besieger_side_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.HostileAction;
			return this.common_join_siege_event_button_condition(args);
		}

		// Token: 0x060035D2 RID: 13778 RVA: 0x000EFE62 File Offset: 0x000EE062
		private void game_menu_join_siege_event_on_defender_side_on_consequence(MenuCallbackArgs args)
		{
			GameMenu.SwitchToMenu("break_in_menu");
		}

		// Token: 0x060035D3 RID: 13779 RVA: 0x000EFE70 File Offset: 0x000EE070
		private bool game_menu_join_encounter_leave_no_army_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Leave;
			MBTextManager.SetTextVariable("LEAVE_TEXT", "{=ebUwP3Q3}Don't get involved.", false);
			if (MobileParty.MainParty.Army != null)
			{
				Army army = MobileParty.MainParty.Army;
				return ((army != null) ? army.LeaderParty : null) == MobileParty.MainParty;
			}
			return true;
		}

		// Token: 0x060035D4 RID: 13780 RVA: 0x000EFEC0 File Offset: 0x000EE0C0
		private bool game_menu_join_encounter_leave_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Leave;
			MobileParty mobileParty = ((Settlement.CurrentSettlement.SiegeEvent != null) ? Settlement.CurrentSettlement.SiegeEvent.BesiegerCamp.BesiegerParty : null);
			return mobileParty == null || !mobileParty.IsMainParty;
		}

		// Token: 0x060035D5 RID: 13781 RVA: 0x000EFF08 File Offset: 0x000EE108
		private void game_menu_hostile_action_end_by_peace_on_init(MenuCallbackArgs args)
		{
			PlayerEncounter playerEncounter = PlayerEncounter.Current;
			if (playerEncounter != null)
			{
				playerEncounter.ResetPlayerEncounterInterruptedByPeace();
			}
			StanceLink stanceLink = (from x in MobileParty.MainParty.MapFaction.Stances
				where !x.IsAtWar
				orderby x.PeaceDeclarationDate.ElapsedHoursUntilNow
				select x).FirstOrDefault<StanceLink>();
			if (stanceLink != null)
			{
				GameTexts.SetVariable("HOSTILE_FACTION", stanceLink.Faction1.EncyclopediaLinkWithName);
				GameTexts.SetVariable("SETTLEMENT_FACTION", stanceLink.Faction2.EncyclopediaLinkWithName);
			}
		}

		// Token: 0x060035D6 RID: 13782 RVA: 0x000EFFAF File Offset: 0x000EE1AF
		private bool game_menu_siege_attacker_left_return_to_settlement_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Submenu;
			GameTexts.SetVariable("SETTLEMENT", MobileParty.MainParty.LastVisitedSettlement.Name);
			return true;
		}

		// Token: 0x060035D7 RID: 13783 RVA: 0x000EFFD4 File Offset: 0x000EE1D4
		private void game_menu_siege_attacker_left_return_to_settlement_on_consequence(MenuCallbackArgs args)
		{
			if (PlayerEncounter.Current == null && MobileParty.MainParty.AttachedTo == null)
			{
				EncounterManager.StartSettlementEncounter(MobileParty.MainParty, MobileParty.MainParty.LastVisitedSettlement);
			}
			if (PlayerEncounter.LocationEncounter == null)
			{
				PlayerEncounter.EnterSettlement();
			}
			string genericStateMenu = Campaign.Current.Models.EncounterGameMenuModel.GetGenericStateMenu();
			if (string.IsNullOrEmpty(genericStateMenu))
			{
				GameMenu.ExitToLast();
				return;
			}
			GameMenu.SwitchToMenu(genericStateMenu);
		}

		// Token: 0x060035D8 RID: 13784 RVA: 0x000F003D File Offset: 0x000EE23D
		private bool game_menu_siege_attacker_left_leave_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Leave;
			return true;
		}

		// Token: 0x060035D9 RID: 13785 RVA: 0x000F0048 File Offset: 0x000EE248
		private bool game_menu_siege_attacker_defeated_leave_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Leave;
			return true;
		}

		// Token: 0x060035DA RID: 13786 RVA: 0x000F0053 File Offset: 0x000EE253
		private bool game_menu_encounter_cheat_on_condition(MenuCallbackArgs args)
		{
			return Game.Current.CheatMode;
		}

		// Token: 0x060035DB RID: 13787 RVA: 0x000F0060 File Offset: 0x000EE260
		private void game_menu_encounter_interrupted_on_init(MenuCallbackArgs args)
		{
			Settlement currentSettlement = Settlement.CurrentSettlement;
			PartyBase leaderParty = PlayerEncounter.EncounteredBattle.GetLeaderParty(BattleSideEnum.Attacker);
			MBTextManager.SetTextVariable("ATTACKER", leaderParty.Name, false);
			MBTextManager.SetTextVariable("DEFENDER", currentSettlement.Name, false);
		}

		// Token: 0x060035DC RID: 13788 RVA: 0x000F00A4 File Offset: 0x000EE2A4
		private void game_menu_encounter_interrupted_siege_preparations_on_init(MenuCallbackArgs args)
		{
			Settlement currentSettlement = Settlement.CurrentSettlement;
			TextObject name = Settlement.CurrentSettlement.SiegeEvent.BesiegerCamp.BesiegerParty.Name;
			TextObject text = args.MenuContext.GameMenu.GetText();
			text.SetTextVariable("ATTACKER", name);
			text.SetTextVariable("DEFENDER", currentSettlement.Name);
			if (PlayerEncounter.Current != null)
			{
				PlayerEncounter.Current.IsPlayerWaiting = false;
			}
		}

		// Token: 0x060035DD RID: 13789 RVA: 0x000F0114 File Offset: 0x000EE314
		private bool game_menu_encounter_interrupted_siege_preparations_leave_town_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Leave;
			args.MenuContext.GameMenu.GetText().SetTextVariable("SETTLEMENT", Settlement.CurrentSettlement.Name);
			return !FactionManager.IsAtWarAgainstFaction(Hero.MainHero.MapFaction, Hero.MainHero.CurrentSettlement.SiegeEvent.BesiegerCamp.BesiegerParty.MapFaction);
		}

		// Token: 0x060035DE RID: 13790 RVA: 0x000F0180 File Offset: 0x000EE380
		private void game_menu_encounter_interrupted_by_raid_on_init(MenuCallbackArgs args)
		{
			Settlement currentSettlement = Settlement.CurrentSettlement;
			TextObject name = currentSettlement.Party.MapEvent.GetLeaderParty(currentSettlement.Party.OpponentSide).Name;
			TextObject text = args.MenuContext.GameMenu.GetText();
			text.SetTextVariable("ATTACKER", name);
			text.SetTextVariable("DEFENDER", currentSettlement.Name);
			if (PlayerEncounter.Current != null)
			{
				PlayerEncounter.Current.IsPlayerWaiting = false;
			}
		}

		// Token: 0x060035DF RID: 13791 RVA: 0x000F01F4 File Offset: 0x000EE3F4
		private bool game_menu_encounter_interrupted_by_raid_continue_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Continue;
			return true;
		}

		// Token: 0x060035E0 RID: 13792 RVA: 0x000F0200 File Offset: 0x000EE400
		private void game_menu_settlement_hide_and_wait_on_consequence(MenuCallbackArgs args)
		{
			Settlement currentSettlement = Settlement.CurrentSettlement;
			SiegeEvent siegeEvent = currentSettlement.SiegeEvent;
			if (((siegeEvent != null) ? siegeEvent.BesiegerCamp.BesiegerParty : null) != null)
			{
				GameMenu.SwitchToMenu("encounter_interrupted_siege_preparations");
				return;
			}
			if (currentSettlement.IsTown)
			{
				GameMenu.SwitchToMenu("town");
				return;
			}
			if (currentSettlement.IsCastle)
			{
				GameMenu.SwitchToMenu("castle");
			}
		}

		// Token: 0x060035E1 RID: 13793 RVA: 0x000F025C File Offset: 0x000EE45C
		private bool game_menu_settlement_hide_and_wait_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Leave;
			return true;
		}

		// Token: 0x060035E2 RID: 13794 RVA: 0x000F0267 File Offset: 0x000EE467
		private bool wait_menu_settlement_hide_and_wait_on_condition(MenuCallbackArgs args)
		{
			args.MenuContext.GameMenu.GetText().SetTextVariable("SETTLEMENT", Settlement.CurrentSettlement.Name);
			Campaign.Current.TimeControlMode = CampaignTimeControlMode.StoppableFastForward;
			args.optionLeaveType = GameMenuOption.LeaveType.Wait;
			return true;
		}

		// Token: 0x060035E3 RID: 13795 RVA: 0x000F02A4 File Offset: 0x000EE4A4
		private bool game_menu_encounter_interrupted_siege_preparations_break_out_of_town_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.LeaveTroopsAndFlee;
			args.MenuContext.GameMenu.GetText().SetTextVariable("SETTLEMENT", Settlement.CurrentSettlement.Name);
			MobileParty mainParty = MobileParty.MainParty;
			SiegeEvent siegeEvent = Settlement.CurrentSettlement.SiegeEvent;
			int lostTroopCountForBreakingOutOfBesiegedSettlement = Campaign.Current.Models.TroopSacrificeModel.GetLostTroopCountForBreakingOutOfBesiegedSettlement(mainParty, siegeEvent);
			Army army = mainParty.Army;
			int num = ((army != null) ? army.TotalRegularCount : mainParty.MemberRoster.TotalRegulars);
			if (lostTroopCountForBreakingOutOfBesiegedSettlement > num)
			{
				args.Tooltip = new TextObject("{=MTbOGRCF}You don't have enough men!", null);
				args.IsEnabled = false;
			}
			if (Campaign.Current.Models.EncounterModel.GetLeaderOfSiegeEvent(siegeEvent, PlayerSiege.PlayerSide) != Hero.MainHero)
			{
				args.Tooltip = new TextObject("{=OmGHXuZB}You are not in command of the defenders.", null);
				args.IsEnabled = false;
			}
			return FactionManager.IsAtWarAgainstFaction(Hero.MainHero.MapFaction, siegeEvent.BesiegerCamp.BesiegerParty.MapFaction);
		}

		// Token: 0x060035E4 RID: 13796 RVA: 0x000F0398 File Offset: 0x000EE598
		private bool game_menu_encounter_interrupted_siege_preparations_hide_in_town_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Wait;
			IFaction mapFaction = Hero.MainHero.MapFaction;
			SiegeEvent siegeEvent = Settlement.CurrentSettlement.SiegeEvent;
			IFaction mapFaction2 = Settlement.CurrentSettlement.MapFaction;
			return mapFaction != siegeEvent.BesiegerCamp.BesiegerParty.MapFaction && (FactionManager.IsAtWarAgainstFaction(mapFaction2, mapFaction) || FactionManager.IsNeutralWithFaction(mapFaction2, mapFaction));
		}

		// Token: 0x060035E5 RID: 13797 RVA: 0x000F03F5 File Offset: 0x000EE5F5
		private void game_menu_encounter_interrupted_break_out_of_town_on_consequence(MenuCallbackArgs args)
		{
			GameMenu.SwitchToMenu("break_out_menu");
		}

		// Token: 0x060035E6 RID: 13798 RVA: 0x000F0404 File Offset: 0x000EE604
		private void game_menu_encounter_interrupted_siege_preparations_join_defend_on_consequence(MenuCallbackArgs args)
		{
			Settlement currentSettlement = Settlement.CurrentSettlement;
			PlayerSiege.StartPlayerSiege(BattleSideEnum.Defender, false, null);
			MobileParty.MainParty.Ai.SetMoveDefendSettlement(currentSettlement);
			PlayerSiege.StartSiegePreparation();
		}

		// Token: 0x060035E7 RID: 13799 RVA: 0x000F0434 File Offset: 0x000EE634
		private bool game_menu_encounter_interrupted_siege_preparations_join_defend_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.DefendAction;
			GameTexts.SetVariable("SETTLEMENT", Settlement.CurrentSettlement.Name);
			return Settlement.CurrentSettlement.SiegeEvent.CanPartyJoinSide(PartyBase.MainParty, BattleSideEnum.Defender);
		}

		// Token: 0x060035E8 RID: 13800 RVA: 0x000F0467 File Offset: 0x000EE667
		private void game_menu_encounter_interrupted_leave_on_consequence(MenuCallbackArgs args)
		{
			PlayerEncounter.Finish(true);
		}

		// Token: 0x060035E9 RID: 13801 RVA: 0x000F046F File Offset: 0x000EE66F
		public static void menu_sneak_into_town_succeeded_continue_on_consequence(MenuCallbackArgs args)
		{
			GameMenu.SwitchToMenu("town");
		}

		// Token: 0x060035EA RID: 13802 RVA: 0x000F047B File Offset: 0x000EE67B
		public static bool menu_sneak_into_town_succeeded_continue_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Continue;
			return true;
		}

		// Token: 0x060035EB RID: 13803 RVA: 0x000F0486 File Offset: 0x000EE686
		public static void game_menu_sneak_into_town_caught_on_init(MenuCallbackArgs args)
		{
			ChangeCrimeRatingAction.Apply(Settlement.CurrentSettlement.MapFaction, 10f, true);
		}

		// Token: 0x060035EC RID: 13804 RVA: 0x000F049D File Offset: 0x000EE69D
		public static void mno_sneak_caught_surrender_on_consequence(MenuCallbackArgs args)
		{
			GameMenu.SwitchToMenu("menu_captivity_castle_taken_prisoner");
		}

		// Token: 0x060035ED RID: 13805 RVA: 0x000F04A9 File Offset: 0x000EE6A9
		public static bool mno_sneak_caught_surrender_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Surrender;
			return true;
		}

		// Token: 0x060035EE RID: 13806 RVA: 0x000F04B4 File Offset: 0x000EE6B4
		private void game_menu_encounter_interrupted_continue_on_consequence(MenuCallbackArgs args)
		{
			GameMenu.SwitchToMenu("join_encounter");
		}

		// Token: 0x060035EF RID: 13807 RVA: 0x000F04C0 File Offset: 0x000EE6C0
		private bool game_menu_encounter_interrupted_leave_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Leave;
			return true;
		}

		// Token: 0x060035F0 RID: 13808 RVA: 0x000F04CB File Offset: 0x000EE6CB
		private void game_menu_town_assault_on_init(MenuCallbackArgs args)
		{
			GameMenu.SwitchToMenu("encounter");
			this.game_menu_encounter_attack_on_consequence(args);
		}

		// Token: 0x060035F1 RID: 13809 RVA: 0x000F04DE File Offset: 0x000EE6DE
		private void game_menu_town_assault_order_attack_on_init(MenuCallbackArgs args)
		{
			GameMenu.SwitchToMenu("encounter");
			this.game_menu_encounter_order_attack_on_consequence(args);
		}

		// Token: 0x060035F2 RID: 13810 RVA: 0x000F04F4 File Offset: 0x000EE6F4
		private void game_menu_army_encounter_on_init(MenuCallbackArgs args)
		{
			if (PlayerEncounter.LeaveEncounter)
			{
				PlayerEncounter.Finish(true);
				return;
			}
			if ((PlayerEncounter.Battle != null && PlayerEncounter.Battle.AttackerSide.LeaderParty != PartyBase.MainParty && PlayerEncounter.Battle.DefenderSide.LeaderParty != PartyBase.MainParty) || PlayerEncounter.MeetingDone)
			{
				if (PlayerEncounter.Battle == null)
				{
					PlayerEncounter.StartBattle();
				}
				if (PlayerEncounter.BattleChallenge)
				{
					GameMenu.SwitchToMenu("duel_starter_menu");
					return;
				}
				GameMenu.SwitchToMenu("encounter");
				return;
			}
			else
			{
				if (PlayerEncounter.EncounteredMobileParty.SiegeEvent != null && Settlement.CurrentSettlement != null)
				{
					GameMenu.SwitchToMenu("join_siege_event");
					return;
				}
				if (PlayerEncounter.EncounteredMobileParty != null && PlayerEncounter.EncounteredMobileParty.Army != null)
				{
					MBTextManager.SetTextVariable("ARMY", PlayerEncounter.EncounteredMobileParty.Army.Name, false);
					MBTextManager.SetTextVariable("ARMY_ENCOUNTER_TEXT", GameTexts.FindText("str_you_have_encountered_ARMY", null), true);
				}
				return;
			}
		}

		// Token: 0x060035F3 RID: 13811 RVA: 0x000F05D4 File Offset: 0x000EE7D4
		private void game_menu_encounter_on_init(MenuCallbackArgs args)
		{
			if (PlayerEncounter.Battle == null)
			{
				if (MobileParty.MainParty.MapEvent != null)
				{
					PlayerEncounter.Init();
				}
				else
				{
					PlayerEncounter.StartBattle();
				}
			}
			PlayerEncounter.Update();
			this.UpdateVillageHostileActionEncounter(args);
			if (PlayerEncounter.Current == null)
			{
				Campaign.Current.SaveHandler.SignalAutoSave();
			}
		}

		// Token: 0x060035F4 RID: 13812 RVA: 0x000F0624 File Offset: 0x000EE824
		private void UpdateVillageHostileActionEncounter(MenuCallbackArgs args)
		{
			MapEvent battle = PlayerEncounter.Battle;
			if (Game.Current.GameStateManager.ActiveState is MapState && ((battle != null) ? battle.MapEventSettlement : null) != null && battle.MapEventSettlement.IsVillage && battle.DefenderSide.LeaderParty.IsSettlement && battle.AttackerSide == battle.GetMapEventSide(battle.PlayerSide))
			{
				bool flag = battle.DefenderSide.Parties.All((MapEventParty x) => x.Party.MemberRoster.TotalHealthyCount == 0);
				bool flag2 = this.ConsiderMilitiaSurrenderPossibility();
				if (flag || flag2)
				{
					if (!flag)
					{
						for (int i = battle.DefenderSide.Parties.Count - 1; i >= 0; i--)
						{
							if (battle.DefenderSide.Parties[i].Party.IsMobile)
							{
								battle.DefenderSide.Parties[i].Party.MapEventSide = null;
							}
						}
						if (!battle.IsRaid)
						{
							battle.SetOverrideWinner(BattleSideEnum.Attacker);
						}
					}
					if (battle.IsRaid)
					{
						this.game_menu_village_raid_no_resist_on_consequence(args);
						return;
					}
					if (battle.IsForcingSupplies)
					{
						this.game_menu_village_force_supplies_no_resist_loot_on_consequence(args);
						return;
					}
					if (battle.IsForcingVolunteers)
					{
						this.game_menu_village_force_volunteers_no_resist_loot_on_consequence(args);
					}
				}
			}
		}

		// Token: 0x060035F5 RID: 13813 RVA: 0x000F0773 File Offset: 0x000EE973
		public static bool game_menu_captivity_taken_prisoner_cheat_on_condition(MenuCallbackArgs args)
		{
			return Game.Current.IsDevelopmentMode;
		}

		// Token: 0x060035F6 RID: 13814 RVA: 0x000F0780 File Offset: 0x000EE980
		private bool ConsiderMilitiaSurrenderPossibility()
		{
			bool flag = false;
			MapEvent battle = PlayerEncounter.Battle;
			if ((battle.IsRaid || battle.IsForcingSupplies || battle.IsForcingVolunteers) && battle.MapEventSettlement.IsVillage)
			{
				Settlement mapEventSettlement = battle.MapEventSettlement;
				float num = 0f;
				bool flag2 = false;
				foreach (MapEventParty mapEventParty in battle.DefenderSide.Parties)
				{
					num += mapEventParty.Party.TotalStrength;
					if (mapEventParty.Party.IsMobile && mapEventParty.Party.MobileParty.IsLordParty)
					{
						flag2 = true;
					}
				}
				float num2 = 0f;
				foreach (MapEventParty mapEventParty2 in battle.AttackerSide.Parties)
				{
					if (!mapEventParty2.Party.IsMobile || mapEventParty2.Party.MobileParty.Army == null)
					{
						num2 += mapEventParty2.Party.TotalStrength;
					}
					else if (mapEventParty2.Party.IsMobile && mapEventParty2.Party.MobileParty.Army != null && mapEventParty2.Party.MobileParty.Army.LeaderParty == mapEventParty2.Party.MobileParty)
					{
						foreach (MobileParty mobileParty in mapEventParty2.Party.MobileParty.Army.LeaderParty.AttachedParties)
						{
							num2 += mobileParty.Party.TotalStrength;
						}
					}
				}
				Clan ownerClan = mapEventSettlement.OwnerClan;
				bool flag3;
				if (ownerClan == null)
				{
					flag3 = null != null;
				}
				else
				{
					Hero leader = ownerClan.Leader;
					flag3 = ((leader != null) ? leader.PartyBelongedTo : null) != null;
				}
				float num3 = (flag3 ? mapEventSettlement.OwnerClan.Leader.PartyBelongedTo.Party.RandomFloatWithSeed(1U, 0.05f, 0.15f) : 0.1f);
				flag = !flag2 && num2 * num3 > num;
			}
			return flag;
		}

		// Token: 0x060035F7 RID: 13815 RVA: 0x000F09DC File Offset: 0x000EEBDC
		private bool game_menu_encounter_leave_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Leave;
			return (MobileParty.MainParty.Army == null || MobileParty.MainParty.Army.LeaderParty == MobileParty.MainParty) && MapEventHelper.CanLeaveBattle(MobileParty.MainParty) && (!MobileParty.MainParty.MapEvent.IsSallyOut || MobileParty.MainParty.MapEvent.PlayerSide == BattleSideEnum.Defender || MobileParty.MainParty.CurrentSettlement == null);
		}

		// Token: 0x060035F8 RID: 13816 RVA: 0x000F0A54 File Offset: 0x000EEC54
		private bool game_menu_sally_out_go_back_to_settlement_on_condition(MenuCallbackArgs args)
		{
			if (MobileParty.MainParty.MapEvent != null && MobileParty.MainParty.MapEvent.IsSallyOut && MobileParty.MainParty.MapEvent.PlayerSide == BattleSideEnum.Attacker && MobileParty.MainParty.CurrentSettlement != null && (MobileParty.MainParty.Army == null || MobileParty.MainParty.Army.LeaderParty == MobileParty.MainParty) && (MobileParty.MainParty.SiegeEvent == null || !MobileParty.MainParty.SiegeEvent.BesiegerCamp.IsBesiegerSideParty(MobileParty.MainParty)) && !PlayerEncounter.Current.CheckIfBattleShouldContinueAfterBattleMission())
			{
				args.optionLeaveType = GameMenuOption.LeaveType.Leave;
				GameTexts.SetVariable("SETTLEMENT", MobileParty.MainParty.LastVisitedSettlement.Name);
				return true;
			}
			return false;
		}

		// Token: 0x060035F9 RID: 13817 RVA: 0x000F0B20 File Offset: 0x000EED20
		private void game_menu_sally_out_go_back_to_settlement_consequence(MenuCallbackArgs args)
		{
			MapEvent playerMapEvent = MapEvent.PlayerMapEvent;
			playerMapEvent.BeginWait();
			if (Campaign.Current.Models.EncounterModel.GetLeaderOfMapEvent(playerMapEvent, playerMapEvent.PlayerSide) == Hero.MainHero)
			{
				PlayerEncounter.Current.FinalizeBattle();
			}
			else
			{
				PlayerEncounter.LeaveBattle();
			}
			GameMenu.SwitchToMenu("menu_siege_strategies");
		}

		// Token: 0x060035FA RID: 13818 RVA: 0x000F0B76 File Offset: 0x000EED76
		private bool game_menu_encounter_abandon_army_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Leave;
			return MobileParty.MainParty.Army != null && MobileParty.MainParty.Army.LeaderParty != MobileParty.MainParty && MapEventHelper.CanLeaveBattle(MobileParty.MainParty);
		}

		// Token: 0x060035FB RID: 13819 RVA: 0x000F0BB0 File Offset: 0x000EEDB0
		private void game_menu_army_talk_to_leader_on_consequence(MenuCallbackArgs args)
		{
			Campaign.Current.CurrentConversationContext = ConversationContext.PartyEncounter;
			args.optionLeaveType = GameMenuOption.LeaveType.Conversation;
			ConversationCharacterData conversationCharacterData = new ConversationCharacterData(CharacterObject.PlayerCharacter, PartyBase.MainParty, false, false, false, false, false, false);
			ConversationCharacterData conversationCharacterData2 = new ConversationCharacterData(ConversationHelper.GetConversationCharacterPartyLeader(PlayerEncounter.EncounteredParty), PlayerEncounter.EncounteredParty, false, false, false, false, false, false);
			PlayerEncounter.SetMeetingDone();
			CampaignMapConversation.OpenConversation(conversationCharacterData, conversationCharacterData2);
		}

		// Token: 0x060035FC RID: 13820 RVA: 0x000F0C0C File Offset: 0x000EEE0C
		private bool game_menu_army_talk_to_leader_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Conversation;
			PartyBase encounteredParty = PlayerEncounter.EncounteredParty;
			if (((encounteredParty != null) ? encounteredParty.LeaderHero : null) != null)
			{
				MenuHelper.SetIssueAndQuestDataForHero(args, PlayerEncounter.EncounteredParty.LeaderHero);
			}
			return true;
		}

		// Token: 0x060035FD RID: 13821 RVA: 0x000F0C3A File Offset: 0x000EEE3A
		public static void game_menu_captivity_castle_taken_prisoner_cont_on_consequence(MenuCallbackArgs args)
		{
			GameMenu.ExitToLast();
			PartyBase.MainParty.AddElementToMemberRoster(CharacterObject.PlayerCharacter, -1, true);
			TakePrisonerAction.Apply(Settlement.CurrentSettlement.Party, Hero.MainHero);
		}

		// Token: 0x060035FE RID: 13822 RVA: 0x000F0C68 File Offset: 0x000EEE68
		private bool game_menu_army_talk_to_other_members_on_condition(MenuCallbackArgs args)
		{
			foreach (MobileParty mobileParty in PlayerEncounter.EncounteredMobileParty.Army.LeaderParty.AttachedParties)
			{
				Hero leaderHero = mobileParty.LeaderHero;
				if (leaderHero != null)
				{
					MenuHelper.SetIssueAndQuestDataForHero(args, leaderHero);
				}
			}
			args.optionLeaveType = GameMenuOption.LeaveType.Submenu;
			return !FactionManager.IsAtWarAgainstFaction(MobileParty.MainParty.MapFaction, PlayerEncounter.EncounteredMobileParty.MapFaction) && PlayerEncounter.EncounteredMobileParty.Army.LeaderParty.AttachedParties.Count > 0;
		}

		// Token: 0x060035FF RID: 13823 RVA: 0x000F0D14 File Offset: 0x000EEF14
		private void game_menu_army_talk_to_other_members_on_consequence(MenuCallbackArgs args)
		{
			GameMenu.SwitchToMenu("game_menu_army_talk_to_other_members");
		}

		// Token: 0x06003600 RID: 13824 RVA: 0x000F0D20 File Offset: 0x000EEF20
		private void game_menu_army_talk_to_other_members_on_init(MenuCallbackArgs args)
		{
			if (PlayerEncounter.LeaveEncounter)
			{
				PlayerEncounter.Finish(true);
				return;
			}
			args.MenuContext.SetRepeatObjectList(PlayerEncounter.EncounteredMobileParty.Army.LeaderParty.AttachedParties.ToList<MobileParty>());
			if (PlayerEncounter.EncounteredMobileParty.MapFaction.IsAtWarWith(MobileParty.MainParty.MapFaction))
			{
				GameMenu.SwitchToMenu("encounter");
			}
		}

		// Token: 0x06003601 RID: 13825 RVA: 0x000F0D84 File Offset: 0x000EEF84
		private bool game_menu_army_talk_to_other_members_item_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Conversation;
			MobileParty mobileParty = args.MenuContext.GetCurrentRepeatableObject() as MobileParty;
			MBTextManager.SetTextVariable("CHAR_NAME", (mobileParty != null) ? mobileParty.LeaderHero.Name : null, false);
			if (mobileParty != null && mobileParty.LeaderHero != null)
			{
				MenuHelper.SetIssueAndQuestDataForHero(args, mobileParty.LeaderHero);
			}
			return true;
		}

		// Token: 0x06003602 RID: 13826 RVA: 0x000F0DE0 File Offset: 0x000EEFE0
		private void game_menu_army_talk_to_other_members_item_on_consequence(MenuCallbackArgs args)
		{
			MobileParty mobileParty = args.MenuContext.GetSelectedObject() as MobileParty;
			Campaign.Current.CurrentConversationContext = ConversationContext.PartyEncounter;
			CampaignMapConversation.OpenConversation(new ConversationCharacterData(CharacterObject.PlayerCharacter, PartyBase.MainParty, false, false, false, false, false, false), new ConversationCharacterData(ConversationHelper.GetConversationCharacterPartyLeader(mobileParty.Party), mobileParty.Party, false, false, false, false, false, false));
		}

		// Token: 0x06003603 RID: 13827 RVA: 0x000F0E3F File Offset: 0x000EF03F
		private bool game_menu_army_talk_to_other_members_back_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Leave;
			return true;
		}

		// Token: 0x06003604 RID: 13828 RVA: 0x000F0E4A File Offset: 0x000EF04A
		private void game_menu_army_talk_to_other_members_back_on_consequence(MenuCallbackArgs args)
		{
			GameMenu.SwitchToMenu("army_encounter");
		}

		// Token: 0x06003605 RID: 13829 RVA: 0x000F0E56 File Offset: 0x000EF056
		private bool game_menu_army_attack_on_condition(MenuCallbackArgs args)
		{
			this.CheckEnemyAttackableHonorably(args);
			args.optionLeaveType = GameMenuOption.LeaveType.HostileAction;
			return MobileParty.MainParty.MapFaction.IsAtWarWith(PlayerEncounter.EncounteredMobileParty.MapFaction);
		}

		// Token: 0x06003606 RID: 13830 RVA: 0x000F0E80 File Offset: 0x000EF080
		private void CheckEnemyAttackableHonorably(MenuCallbackArgs args)
		{
			if (MobileParty.MainParty.Army != null && MobileParty.MainParty.Army.LeaderParty != MobileParty.MainParty)
			{
				return;
			}
			if (PlayerEncounter.PlayerIsDefender)
			{
				return;
			}
			IFaction faction;
			if (PlayerEncounter.EncounteredMobileParty != null)
			{
				faction = PlayerEncounter.EncounteredMobileParty.MapFaction;
			}
			else
			{
				if (PlayerEncounter.EncounteredParty == null)
				{
					return;
				}
				faction = PlayerEncounter.EncounteredParty.MapFaction;
			}
			if (faction != null && faction.NotAttackableByPlayerUntilTime.IsFuture)
			{
				args.IsEnabled = false;
				args.Tooltip = EncounterGameMenuBehavior.EnemyNotAttackableTooltip;
			}
		}

		// Token: 0x06003607 RID: 13831 RVA: 0x000F0F08 File Offset: 0x000EF108
		private void CheckFactionAttackableHonorably(MenuCallbackArgs args, IFaction faction)
		{
			if (faction.NotAttackableByPlayerUntilTime.IsFuture)
			{
				args.IsEnabled = false;
				args.Tooltip = EncounterGameMenuBehavior.EnemyNotAttackableTooltip;
			}
		}

		// Token: 0x06003608 RID: 13832 RVA: 0x000F0F38 File Offset: 0x000EF138
		private void CheckFortificationAttackableHonorably(MenuCallbackArgs args)
		{
			if (MobileParty.MainParty.Army != null && MobileParty.MainParty.Army.LeaderParty != MobileParty.MainParty)
			{
				return;
			}
			IFaction mapFaction = PlayerEncounter.EncounterSettlement.MapFaction;
			if (mapFaction != null && mapFaction.NotAttackableByPlayerUntilTime.IsFuture)
			{
				args.IsEnabled = false;
				args.Tooltip = EncounterGameMenuBehavior.EnemyNotAttackableTooltip;
			}
		}

		// Token: 0x06003609 RID: 13833 RVA: 0x000F0F9A File Offset: 0x000EF19A
		private bool game_menu_army_leave_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Leave;
			return true;
		}

		// Token: 0x0600360A RID: 13834 RVA: 0x000F0FA5 File Offset: 0x000EF1A5
		private void game_menu_army_attack_on_consequence(MenuCallbackArgs args)
		{
			GameMenu.SwitchToMenu("encounter");
		}

		// Token: 0x0600360B RID: 13835 RVA: 0x000F0FB4 File Offset: 0x000EF1B4
		private bool game_menu_army_join_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Continue;
			if (PlayerEncounter.EncounteredMobileParty.MapFaction != MobileParty.MainParty.MapFaction)
			{
				return false;
			}
			if (PlayerEncounter.EncounteredMobileParty.Army == MobileParty.MainParty.Army)
			{
				return false;
			}
			if (PlayerEncounter.EncounteredMobileParty.MapFaction != null)
			{
				foreach (Kingdom kingdom in Kingdom.All)
				{
					if (kingdom.IsAtWarWith(Clan.PlayerClan.MapFaction) && kingdom.NotAttackableByPlayerUntilTime.IsFuture)
					{
						args.IsEnabled = false;
						args.Tooltip = GameTexts.FindText("str_cant_join_army_safe_passage", null);
					}
				}
			}
			return MobileParty.MainParty.Army == null && PlayerEncounter.EncounteredMobileParty.MapFaction == MobileParty.MainParty.MapFaction;
		}

		// Token: 0x0600360C RID: 13836 RVA: 0x000F10A4 File Offset: 0x000EF2A4
		private void game_menu_army_join_on_consequence(MenuCallbackArgs args)
		{
			MobileParty.MainParty.Army = PlayerEncounter.EncounteredMobileParty.Army;
			MobileParty.MainParty.Army.AddPartyToMergedParties(MobileParty.MainParty);
			PlayerEncounter.Finish(true);
		}

		// Token: 0x0600360D RID: 13837 RVA: 0x000F10D4 File Offset: 0x000EF2D4
		private void army_encounter_leave_on_consequence(MenuCallbackArgs args)
		{
			PlayerEncounter.Finish(true);
		}

		// Token: 0x0600360E RID: 13838 RVA: 0x000F10DC File Offset: 0x000EF2DC
		private void game_menu_encounter_leave_on_consequence(MenuCallbackArgs args)
		{
			MenuHelper.EncounterLeaveConsequence(args);
		}

		// Token: 0x0600360F RID: 13839 RVA: 0x000F10E4 File Offset: 0x000EF2E4
		private void game_menu_encounter_abandon_on_consequence(MenuCallbackArgs args)
		{
			((PlayerEncounter.Battle != null) ? PlayerEncounter.Battle : PlayerEncounter.EncounteredBattle).BeginWait();
			MobileParty.MainParty.Ai.SetMoveModeHold();
			Hero.MainHero.PartyBelongedTo.Army = null;
			PlayerEncounter.Finish(true);
			if (MobileParty.MainParty.BesiegerCamp != null)
			{
				MobileParty.MainParty.BesiegerCamp = null;
			}
		}

		// Token: 0x06003610 RID: 13840 RVA: 0x000F1148 File Offset: 0x000EF348
		private bool game_menu_encounter_surrender_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Surrender;
			return MobileParty.MainParty.MapEvent != null && !MapEventHelper.CanLeaveBattle(MobileParty.MainParty) && PartyBase.MainParty.Side == BattleSideEnum.Defender && MobileParty.MainParty.MapEvent.DefenderSide.TroopCount == MobileParty.MainParty.Party.NumberOfHealthyMembers;
		}

		// Token: 0x06003611 RID: 13841 RVA: 0x000F11A9 File Offset: 0x000EF3A9
		private void game_menu_encounter_surrender_on_consequence(MenuCallbackArgs args)
		{
			PlayerEncounter.PlayerSurrender = true;
			PlayerEncounter.Update();
			if (!Hero.MainHero.CanBecomePrisoner())
			{
				GameMenu.ActivateGameMenu("menu_captivity_end_no_more_enemies");
			}
		}

		// Token: 0x06003612 RID: 13842 RVA: 0x000F11CC File Offset: 0x000EF3CC
		private bool game_menu_encounter_attack_on_condition(MenuCallbackArgs args)
		{
			this.CheckEnemyAttackableHonorably(args);
			return MenuHelper.EncounterAttackCondition(args);
		}

		// Token: 0x06003613 RID: 13843 RVA: 0x000F11DB File Offset: 0x000EF3DB
		private bool game_menu_encounter_capture_the_enemy_on_condition(MenuCallbackArgs args)
		{
			return MenuHelper.EncounterCaptureEnemyCondition(args);
		}

		// Token: 0x06003614 RID: 13844 RVA: 0x000F11E3 File Offset: 0x000EF3E3
		private void game_menu_encounter_attack_on_consequence(MenuCallbackArgs args)
		{
			MenuHelper.EncounterAttackConsequence(args);
		}

		// Token: 0x06003615 RID: 13845 RVA: 0x000F11EB File Offset: 0x000EF3EB
		private void game_menu_capture_the_enemy_on_consequence(MenuCallbackArgs args)
		{
			MenuHelper.EncounterCaptureTheEnemyOnConsequence(args);
		}

		// Token: 0x06003616 RID: 13846 RVA: 0x000F11F4 File Offset: 0x000EF3F4
		private bool game_menu_encounter_order_attack_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.OrderTroopsToAttack;
			MapEvent playerMapEvent = MapEvent.PlayerMapEvent;
			if (playerMapEvent != null)
			{
				this.CheckEnemyAttackableHonorably(args);
				int num = 0;
				foreach (MapEventParty mapEventParty in playerMapEvent.PartiesOnSide(MobileParty.MainParty.Party.Side))
				{
					num += mapEventParty.Party.MemberRoster.Sum(delegate(TroopRosterElement x)
					{
						if (!x.Character.IsHero)
						{
							return x.Number - x.WoundedNumber;
						}
						if (x.Character == CharacterObject.PlayerCharacter)
						{
							return 0;
						}
						if (!x.Character.HeroObject.IsWounded)
						{
							return 1;
						}
						return 0;
					});
				}
				if (playerMapEvent.HasTroopsOnBothSides() && playerMapEvent.GetLeaderParty(PartyBase.MainParty.OpponentSide) != null && num > 0)
				{
					if (MobileParty.MainParty.MemberRoster.Sum(delegate(TroopRosterElement x)
					{
						if (!x.Character.IsHero)
						{
							return x.Number - x.WoundedNumber;
						}
						if (x.Character == CharacterObject.PlayerCharacter)
						{
							return 0;
						}
						if (!x.Character.HeroObject.IsWounded)
						{
							return 1;
						}
						return 0;
					}) > 0)
					{
						MBTextManager.SetTextVariable("SEND_TROOPS_TEXT", "{=QfMeoKOm}Send troops.", false);
					}
					else
					{
						MBTextManager.SetTextVariable("SEND_TROOPS_TEXT", "{=jo3UHKMD}Leave it to the others.", false);
					}
					if (playerMapEvent.IsInvulnerable)
					{
						playerMapEvent.IsInvulnerable = false;
					}
					return true;
				}
			}
			return false;
		}

		// Token: 0x06003617 RID: 13847 RVA: 0x000F1328 File Offset: 0x000EF528
		private void game_menu_encounter_order_attack_on_consequence(MenuCallbackArgs args)
		{
			MenuHelper.EncounterOrderAttackConsequence(args);
		}

		// Token: 0x06003618 RID: 13848 RVA: 0x000F1330 File Offset: 0x000EF530
		private bool game_menu_encounter_leave_your_soldiers_behind_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.LeaveTroopsAndFlee;
			if (PartyBase.MainParty.Side == BattleSideEnum.Defender && PlayerEncounter.Battle.DefenderSide.LeaderParty == PartyBase.MainParty && !MobileParty.MainParty.MapEvent.HasWinner)
			{
				int numberOfTroopsSacrificedForTryingToGetAway = Campaign.Current.Models.TroopSacrificeModel.GetNumberOfTroopsSacrificedForTryingToGetAway(PlayerEncounter.Current.PlayerSide, PlayerEncounter.Battle);
				this._intEncounterVariable = numberOfTroopsSacrificedForTryingToGetAway;
				int num = PartyBase.MainParty.NumberOfRegularMembers;
				if (MobileParty.MainParty.Army != null)
				{
					foreach (MobileParty mobileParty in MobileParty.MainParty.Army.LeaderParty.AttachedParties)
					{
						num += mobileParty.Party.NumberOfRegularMembers;
					}
				}
				if (numberOfTroopsSacrificedForTryingToGetAway < 0 || num < numberOfTroopsSacrificedForTryingToGetAway)
				{
					args.Tooltip = new TextObject("{=MTbOGRCF}You don't have enough men!", null);
					args.IsEnabled = false;
				}
				return true;
			}
			return false;
		}

		// Token: 0x06003619 RID: 13849 RVA: 0x000F1444 File Offset: 0x000EF644
		private void game_menu_leave_soldiers_behind_on_init(MenuCallbackArgs args)
		{
			Hero heroWithHighestSkill = MobilePartyHelper.GetHeroWithHighestSkill(MobileParty.MainParty, DefaultSkills.Tactics);
			int num = ((heroWithHighestSkill != null) ? heroWithHighestSkill.GetSkillValue(DefaultSkills.Tactics) : 0);
			MBTextManager.SetTextVariable("HIGHEST_TACTICS_SKILL", num);
			MBTextManager.SetTextVariable("HIGHEST_TACTICS_SKILLED_MEMBER", (heroWithHighestSkill != null && heroWithHighestSkill != Hero.MainHero) ? heroWithHighestSkill.Name : GameTexts.FindText("str_you", null), false);
			MBTextManager.SetTextVariable("NEEEDED_MEN_COUNT", this._intEncounterVariable);
			MBTextManager.SetTextVariable("SOLDIER_OR_SOLDIERS", (this._intEncounterVariable <= 1) ? GameTexts.FindText("str_soldier", null) : GameTexts.FindText("str_soldiers", null), false);
		}

		// Token: 0x0600361A RID: 13850 RVA: 0x000F14E3 File Offset: 0x000EF6E3
		public static void game_request_entry_to_castle_approved_continue_on_consequence(MenuCallbackArgs args)
		{
			GameMenu.SwitchToMenu("castle");
		}

		// Token: 0x0600361B RID: 13851 RVA: 0x000F14EF File Offset: 0x000EF6EF
		public static bool game_request_entry_to_castle_approved_continue_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Continue;
			return true;
		}

		// Token: 0x0600361C RID: 13852 RVA: 0x000F14FA File Offset: 0x000EF6FA
		public static void game_request_entry_to_castle_rejected_continue_on_consequence(MenuCallbackArgs args)
		{
			GameMenu.SwitchToMenu("castle_outside");
		}

		// Token: 0x0600361D RID: 13853 RVA: 0x000F1506 File Offset: 0x000EF706
		public static void menu_castle_entry_denied_on_init(MenuCallbackArgs args)
		{
		}

		// Token: 0x0600361E RID: 13854 RVA: 0x000F1508 File Offset: 0x000EF708
		private void game_menu_encounter_leave_your_soldiers_behind_accept_on_consequence(MenuCallbackArgs args)
		{
			PlayerEncounter.SacrificeTroops(this._intEncounterVariable, out this._getAwayCasualties, out this._getAwayLostBaggage);
			CampaignEventDispatcher.Instance.OnPlayerDesertedBattle(this._intEncounterVariable);
			if (MobileParty.MainParty.BesiegerCamp != null)
			{
				MobileParty.MainParty.BesiegerCamp = null;
			}
			if (Campaign.Current.CurrentMenuContext != null)
			{
				GameMenu.SwitchToMenu("try_to_get_away_debrief");
				return;
			}
			GameMenu.ActivateGameMenu("try_to_get_away_debrief");
		}

		// Token: 0x0600361F RID: 13855 RVA: 0x000F1574 File Offset: 0x000EF774
		private void game_menu_get_away_debrief_on_init(MenuCallbackArgs args)
		{
			TextObject textObject = GameTexts.FindText("str_STR1_space_STR2", null);
			if (this._getAwayCasualties != null)
			{
				TextObject textObject2;
				if (this._getAwayCasualties.Count > 0)
				{
					textObject2 = new TextObject("{=NhHzgs5e}When, after some time your forces regroup, you take a quick tally of your troops only to see you have lost: {CASUALTIES}.", null);
					textObject2.SetTextVariable("CASUALTIES", PartyBaseHelper.PrintRegularTroopCategories(this._getAwayCasualties));
				}
				else
				{
					textObject2 = new TextObject("{=bXzAluln}When, after some time your forces regroup, you take a quick tally of your troops to see that your forces are intact.", null);
				}
				textObject.SetTextVariable("STR1", textObject2);
			}
			if (this._getAwayLostBaggage != null)
			{
				TextObject textObject3 = TextObject.Empty;
				if (this._getAwayLostBaggage.Count > 0)
				{
					textObject3 = new TextObject("{=mrjz1ka4}And in your lost baggage you had: {LOST_BAGGAGE}.", null);
					textObject3.SetTextVariable("LOST_BAGGAGE", PartyBaseHelper.PrintSummarisedItemRoster(this._getAwayLostBaggage));
				}
				textObject.SetTextVariable("STR2", textObject3);
			}
			MBTextManager.SetTextVariable("CASUALTIES_AND_LOST_BAGGAGE", textObject, false);
		}

		// Token: 0x06003620 RID: 13856 RVA: 0x000F163F File Offset: 0x000EF83F
		private bool game_menu_try_to_get_away_continue_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Continue;
			return true;
		}

		// Token: 0x06003621 RID: 13857 RVA: 0x000F164A File Offset: 0x000EF84A
		private bool game_menu_try_to_get_away_reject_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Leave;
			return true;
		}

		// Token: 0x06003622 RID: 13858 RVA: 0x000F1655 File Offset: 0x000EF855
		private bool game_menu_try_to_get_away_accept_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Continue;
			return true;
		}

		// Token: 0x06003623 RID: 13859 RVA: 0x000F1660 File Offset: 0x000EF860
		private void game_menu_try_to_get_away_end(MenuCallbackArgs args)
		{
			foreach (MapEventParty mapEventParty in PlayerEncounter.Battle.PartiesOnSide(BattleSideEnum.Defender))
			{
				if (mapEventParty.Party.MobileParty != null)
				{
					if (mapEventParty.Party.MobileParty.BesiegerCamp != null)
					{
						mapEventParty.Party.MobileParty.BesiegerCamp = null;
					}
					if (mapEventParty.Party.MobileParty.CurrentSettlement != null && mapEventParty.Party == PartyBase.MainParty)
					{
						LeaveSettlementAction.ApplyForParty(mapEventParty.Party.MobileParty);
					}
				}
			}
			PlayerEncounter.Battle.DiplomaticallyFinished = true;
			PlayerEncounter.ProtectPlayerSide(1f);
			PlayerEncounter.Finish(true);
		}

		// Token: 0x06003624 RID: 13860 RVA: 0x000F172C File Offset: 0x000EF92C
		private bool game_menu_town_besiege_continue_siege_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Continue;
			PartyBase encounteredParty = PlayerEncounter.EncounteredParty;
			if (encounteredParty == null)
			{
				return false;
			}
			MapEvent encounteredBattle = PlayerEncounter.EncounteredBattle;
			return encounteredBattle != null && encounteredBattle.GetLeaderParty(PartyBase.MainParty.Side) == PartyBase.MainParty && encounteredParty.IsSettlement && encounteredParty.Settlement.IsFortification && FactionManager.IsAtWarAgainstFaction(Hero.MainHero.MapFaction, encounteredParty.MapFaction) && encounteredParty.Settlement.IsUnderSiege && encounteredParty.Settlement.CurrentSiegeState == Settlement.SiegeState.OnTheWalls;
		}

		// Token: 0x06003625 RID: 13861 RVA: 0x000F17B5 File Offset: 0x000EF9B5
		private void game_menu_town_besiege_continue_siege_on_consequence(MenuCallbackArgs args)
		{
			if (PlayerEncounter.Battle != null)
			{
				PlayerEncounter.Finish(true);
			}
			PlayerSiege.StartSiegePreparation();
		}

		// Token: 0x06003626 RID: 13862 RVA: 0x000F17CC File Offset: 0x000EF9CC
		private bool game_menu_village_hostile_action_on_condition(MenuCallbackArgs args)
		{
			if (Settlement.CurrentSettlement == null || !Settlement.CurrentSettlement.IsVillage)
			{
				return false;
			}
			args.optionLeaveType = GameMenuOption.LeaveType.Raid;
			MapEvent battle = PlayerEncounter.Battle;
			if (PartyBase.MainParty.Side == BattleSideEnum.Attacker)
			{
				return !battle.PartiesOnSide(BattleSideEnum.Defender).Any((MapEventParty party) => party.Party.NumberOfHealthyMembers > 0);
			}
			return false;
		}

		// Token: 0x06003627 RID: 13863 RVA: 0x000F1839 File Offset: 0x000EFA39
		private void game_menu_village_raid_no_resist_on_consequence(MenuCallbackArgs args)
		{
			BeHostileAction.ApplyEncounterHostileAction(PartyBase.MainParty, Settlement.CurrentSettlement.Party);
			if (PlayerEncounter.Current != null)
			{
				if (PlayerEncounter.InsideSettlement)
				{
					PlayerEncounter.LeaveSettlement();
				}
				GameMenu.ActivateGameMenu("raiding_village");
			}
		}

		// Token: 0x06003628 RID: 13864 RVA: 0x000F186C File Offset: 0x000EFA6C
		private void game_menu_village_force_supplies_no_resist_loot_on_consequence(MenuCallbackArgs args)
		{
			BeHostileAction.ApplyMinorCoercionHostileAction(PartyBase.MainParty, Settlement.CurrentSettlement.Party);
			GameMenu.ActivateGameMenu("force_supplies_village");
		}

		// Token: 0x06003629 RID: 13865 RVA: 0x000F188C File Offset: 0x000EFA8C
		private void game_menu_village_force_volunteers_no_resist_loot_on_consequence(MenuCallbackArgs args)
		{
			BeHostileAction.ApplyMajorCoercionHostileAction(PartyBase.MainParty, Settlement.CurrentSettlement.Party);
			GameMenu.ActivateGameMenu("force_volunteers_village");
		}

		// Token: 0x0600362A RID: 13866 RVA: 0x000F18AC File Offset: 0x000EFAAC
		private void game_menu_taken_prisoner_on_init(MenuCallbackArgs args)
		{
		}

		// Token: 0x0600362B RID: 13867 RVA: 0x000F18AE File Offset: 0x000EFAAE
		private bool game_menu_taken_prisoner_continue_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Continue;
			return true;
		}

		// Token: 0x0600362C RID: 13868 RVA: 0x000F18B9 File Offset: 0x000EFAB9
		private void game_menu_taken_prisoner_continue_on_consequence(MenuCallbackArgs args)
		{
			GameMenu.ExitToLast();
		}

		// Token: 0x0600362D RID: 13869 RVA: 0x000F18C0 File Offset: 0x000EFAC0
		private void game_menu_encounter_meeting_on_init(MenuCallbackArgs args)
		{
			if (PlayerEncounter.Current == null || ((PlayerEncounter.Battle == null || PlayerEncounter.Battle.AttackerSide.LeaderParty == PartyBase.MainParty || PlayerEncounter.Battle.DefenderSide.LeaderParty == PartyBase.MainParty) && !PlayerEncounter.MeetingDone))
			{
				PlayerEncounter.DoMeeting();
				return;
			}
			if (PlayerEncounter.LeaveEncounter)
			{
				PlayerEncounter.Finish(true);
				return;
			}
			if (PlayerEncounter.Battle == null)
			{
				PlayerEncounter.StartBattle();
			}
			if (PlayerEncounter.BattleChallenge)
			{
				GameMenu.SwitchToMenu("duel_starter_menu");
				return;
			}
			GameMenu.SwitchToMenu("encounter");
		}

		// Token: 0x0600362E RID: 13870 RVA: 0x000F194B File Offset: 0x000EFB4B
		private void VillageOutsideOnInit(MenuCallbackArgs args)
		{
			GameMenu.SwitchToMenu("village");
		}

		// Token: 0x0600362F RID: 13871 RVA: 0x000F1958 File Offset: 0x000EFB58
		private void game_menu_town_outside_on_init(MenuCallbackArgs args)
		{
			Settlement encounterSettlement = PlayerEncounter.EncounterSettlement;
			SettlementComponent settlementComponent = encounterSettlement.SettlementComponent;
			args.MenuTitle = encounterSettlement.Name;
			MBTextManager.SetTextVariable("SETTLEMENT_NAME", encounterSettlement.EncyclopediaLinkWithName, false);
			MBTextManager.SetTextVariable("FACTION_TERM", encounterSettlement.MapFaction.EncyclopediaLinkWithName, false);
			Campaign.Current.Models.SettlementAccessModel.CanMainHeroEnterSettlement(encounterSettlement, out this._accessDetails);
			SettlementAccessModel.AccessLevel accessLevel = this._accessDetails.AccessLevel;
			int num = (int)accessLevel;
			if (num != 0)
			{
				if (num == 1)
				{
					if (this._accessDetails.AccessLimitationReason == SettlementAccessModel.AccessLimitationReason.CrimeRating)
					{
						MBTextManager.SetTextVariable("FACTION", Settlement.CurrentSettlement.MapFaction.Name, false);
						MBTextManager.SetTextVariable("TOWN_TEXT", GameTexts.FindText("str_gate_down_criminal_text", null), false);
						goto IL_1B5;
					}
				}
			}
			else if (this._accessDetails.AccessLimitationReason == SettlementAccessModel.AccessLimitationReason.HostileFaction)
			{
				if (encounterSettlement.InRebelliousState)
				{
					TextObject textObject = GameTexts.FindText("str_gate_down_enemy_text_castle_low_loyalty", null);
					textObject.SetTextVariable("FACTION_INFORMAL_NAME", encounterSettlement.MapFaction.InformalName);
					MBTextManager.SetTextVariable("TOWN_TEXT", textObject, false);
					goto IL_1B5;
				}
				MBTextManager.SetTextVariable("TOWN_TEXT", GameTexts.FindText("str_gate_down_enemy_text_castle", null), false);
				goto IL_1B5;
			}
			else if (this._accessDetails.AccessLimitationReason == SettlementAccessModel.AccessLimitationReason.CrimeRating)
			{
				MBTextManager.SetTextVariable("FACTION", Settlement.CurrentSettlement.MapFaction.Name, false);
				MBTextManager.SetTextVariable("TOWN_TEXT", GameTexts.FindText("str_gate_down_criminal_text", null), false);
				goto IL_1B5;
			}
			if (encounterSettlement.InRebelliousState)
			{
				TextObject textObject2 = GameTexts.FindText("str_settlement_not_allowed_text_low_loyalty", null);
				textObject2.SetTextVariable("FACTION_INFORMAL_NAME", encounterSettlement.MapFaction.InformalName);
				MBTextManager.SetTextVariable("TOWN_TEXT", textObject2, false);
			}
			else
			{
				MBTextManager.SetTextVariable("TOWN_TEXT", GameTexts.FindText("str_settlement_not_allowed_text", null), false);
			}
			IL_1B5:
			if (this._accessDetails.PreliminaryActionObligation == SettlementAccessModel.PreliminaryActionObligation.Must && this._accessDetails.PreliminaryActionType == SettlementAccessModel.PreliminaryActionType.SettlementIsTaken)
			{
				GameMenu.SwitchToMenu("town");
				settlementComponent.IsTaken = false;
				return;
			}
			if (this._accessDetails.PreliminaryActionObligation == SettlementAccessModel.PreliminaryActionObligation.Optional && this._accessDetails.PreliminaryActionType == SettlementAccessModel.PreliminaryActionType.FaceCharges)
			{
				GameMenu.SwitchToMenu("town_inside_criminal");
				return;
			}
			if (this._accessDetails.AccessLevel == SettlementAccessModel.AccessLevel.FullAccess && this._accessDetails.AccessMethod == SettlementAccessModel.AccessMethod.Direct)
			{
				GameMenu.SwitchToMenu("town");
			}
		}

		// Token: 0x06003630 RID: 13872 RVA: 0x000F1B98 File Offset: 0x000EFD98
		private void game_menu_fortification_high_crime_rating_on_init(MenuCallbackArgs args)
		{
			TextObject textObject = new TextObject("{=DdeIg5hz}As you move through the streets, you hear whispers of an upcoming war between your faction and {SETTLEMENT_FACTION}. Upon hearing this, you slink away without attracting any suspicion.", null);
			textObject.SetTextVariable("SETTLEMENT_FACTION", Settlement.CurrentSettlement.MapFaction.EncyclopediaLinkWithName);
			MBTextManager.SetTextVariable("FORTIFICATION_CRIME_RATING_TEXT", textObject, false);
		}

		// Token: 0x06003631 RID: 13873 RVA: 0x000F1BD8 File Offset: 0x000EFDD8
		private bool game_menu_fortification_high_crime_rating_continue_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Continue;
			return true;
		}

		// Token: 0x06003632 RID: 13874 RVA: 0x000F1BE4 File Offset: 0x000EFDE4
		private void game_menu_army_left_settlement_due_to_war_on_init(MenuCallbackArgs args)
		{
			TextObject textObject = new TextObject("{=Nsb6SD4y}After receiving word of an upcoming war against {ENEMY_FACTION}, {ARMY_NAME} decided to leave {SETTLEMENT_NAME}.", null);
			textObject.SetTextVariable("ENEMY_FACTION", Settlement.CurrentSettlement.MapFaction.EncyclopediaLinkWithName);
			textObject.SetTextVariable("ARMY_NAME", MobileParty.MainParty.Army.Name);
			textObject.SetTextVariable("SETTLEMENT_NAME", Settlement.CurrentSettlement.EncyclopediaLinkWithName);
			MBTextManager.SetTextVariable("ARMY_LEFT_SETTLEMENT_DUE_TO_WAR_TEXT", textObject, false);
		}

		// Token: 0x06003633 RID: 13875 RVA: 0x000F1C55 File Offset: 0x000EFE55
		private bool game_menu_army_left_settlement_due_to_war_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Continue;
			return true;
		}

		// Token: 0x06003634 RID: 13876 RVA: 0x000F1C60 File Offset: 0x000EFE60
		private void game_menu_castle_outside_on_init(MenuCallbackArgs args)
		{
			Settlement encounterSettlement = PlayerEncounter.EncounterSettlement;
			SettlementComponent settlementComponent = encounterSettlement.SettlementComponent;
			args.MenuTitle = encounterSettlement.Name;
			Campaign.Current.Models.SettlementAccessModel.CanMainHeroEnterSettlement(encounterSettlement, out this._accessDetails);
			MBTextManager.SetTextVariable("SETTLEMENT_NAME", encounterSettlement.EncyclopediaLinkWithName, false);
			StringHelpers.SetCharacterProperties("LORD", encounterSettlement.OwnerClan.Leader.CharacterObject, null, false);
			MBTextManager.SetTextVariable("FACTION_TERM", encounterSettlement.MapFaction.EncyclopediaLinkWithName, false);
			SettlementAccessModel.AccessLevel accessLevel = this._accessDetails.AccessLevel;
			int num = (int)accessLevel;
			if (num != 0)
			{
				if (num == 1)
				{
					if (this._accessDetails.AccessLimitationReason == SettlementAccessModel.AccessLimitationReason.CrimeRating)
					{
						MBTextManager.SetTextVariable("FACTION", Settlement.CurrentSettlement.MapFaction.Name, false);
						MBTextManager.SetTextVariable("TOWN_TEXT", GameTexts.FindText("str_gate_down_criminal_text", null), false);
						goto IL_196;
					}
				}
				if (encounterSettlement.OwnerClan == Hero.MainHero.Clan)
				{
					MBTextManager.SetTextVariable("TOWN_TEXT", GameTexts.FindText("str_castle_text_yours", null), false);
				}
				else
				{
					MBTextManager.SetTextVariable("TOWN_TEXT", GameTexts.FindText("str_castle_text_1", null), false);
				}
			}
			else if (this._accessDetails.AccessLimitationReason == SettlementAccessModel.AccessLimitationReason.HostileFaction)
			{
				MBTextManager.SetTextVariable("TOWN_TEXT", GameTexts.FindText("str_gate_down_enemy_text_castle", null), false);
			}
			else if (this._accessDetails.AccessLimitationReason == SettlementAccessModel.AccessLimitationReason.CrimeRating)
			{
				MBTextManager.SetTextVariable("FACTION", Settlement.CurrentSettlement.MapFaction.Name, false);
				MBTextManager.SetTextVariable("TOWN_TEXT", GameTexts.FindText("str_gate_down_criminal_text", null), false);
			}
			else
			{
				MBTextManager.SetTextVariable("TOWN_TEXT", GameTexts.FindText("str_settlement_not_allowed_text", null), false);
			}
			IL_196:
			if (this._accessDetails.PreliminaryActionObligation == SettlementAccessModel.PreliminaryActionObligation.Must && this._accessDetails.PreliminaryActionType == SettlementAccessModel.PreliminaryActionType.SettlementIsTaken)
			{
				GameMenu.SwitchToMenu("castle");
				settlementComponent.IsTaken = false;
				return;
			}
			if (this._accessDetails.AccessLevel == SettlementAccessModel.AccessLevel.FullAccess && (this._accessDetails.AccessMethod == SettlementAccessModel.AccessMethod.Direct || (this._playerIsAlreadyInCastle && this._accessDetails.AccessMethod == SettlementAccessModel.AccessMethod.ByRequest)))
			{
				GameMenu.SwitchToMenu("castle");
				return;
			}
			this._playerIsAlreadyInCastle = false;
		}

		// Token: 0x06003635 RID: 13877 RVA: 0x000F1E78 File Offset: 0x000F0078
		private void game_menu_army_left_settlement_due_to_war_on_consequence(MenuCallbackArgs args)
		{
			MobileParty leaderParty = MobileParty.MainParty.Army.LeaderParty;
			Settlement currentSettlement = Settlement.CurrentSettlement;
			LeaveSettlementAction.ApplyForParty(leaderParty);
			SetPartyAiAction.GetActionForPatrollingAroundSettlement(leaderParty, currentSettlement);
		}

		// Token: 0x06003636 RID: 13878 RVA: 0x000F1EA6 File Offset: 0x000F00A6
		private void game_menu_town_outside_approach_gates_on_consequence(MenuCallbackArgs args)
		{
			GameMenu.SwitchToMenu("town_guard");
		}

		// Token: 0x06003637 RID: 13879 RVA: 0x000F1EB2 File Offset: 0x000F00B2
		private bool game_menu_castle_outside_approach_gates_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Submenu;
			return true;
		}

		// Token: 0x06003638 RID: 13880 RVA: 0x000F1EBC File Offset: 0x000F00BC
		private void game_menu_castle_outside_approach_gates_on_consequence(MenuCallbackArgs args)
		{
			GameMenu.SwitchToMenu("castle_guard");
		}

		// Token: 0x06003639 RID: 13881 RVA: 0x000F1EC8 File Offset: 0x000F00C8
		private void game_menu_fortification_high_crime_rating_continue_on_consequence(MenuCallbackArgs args)
		{
			if (Settlement.CurrentSettlement.IsTown)
			{
				GameMenu.SwitchToMenu("town_outside");
				return;
			}
			if (Settlement.CurrentSettlement.IsCastle)
			{
				GameMenu.SwitchToMenu("castle_outside");
			}
		}

		// Token: 0x0600363A RID: 13882 RVA: 0x000F1EF7 File Offset: 0x000F00F7
		private bool outside_menu_criminal_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Submenu;
			return this._accessDetails.AccessLevel == SettlementAccessModel.AccessLevel.LimitedAccess && this._accessDetails.AccessLimitationReason == SettlementAccessModel.AccessLimitationReason.CrimeRating;
		}

		// Token: 0x0600363B RID: 13883 RVA: 0x000F1F1E File Offset: 0x000F011E
		private void outside_menu_criminal_on_consequence(MenuCallbackArgs args)
		{
			GameMenu.SwitchToMenu("town_discuss_criminal_surrender");
		}

		// Token: 0x0600363C RID: 13884 RVA: 0x000F1F2A File Offset: 0x000F012A
		private void caught_outside_menu_criminal_on_consequence(MenuCallbackArgs args)
		{
			ChangeCrimeRatingAction.Apply(Settlement.CurrentSettlement.MapFaction, 10f, true);
			GameMenu.SwitchToMenu("town_inside_criminal");
		}

		// Token: 0x0600363D RID: 13885 RVA: 0x000F1F4B File Offset: 0x000F014B
		private bool caught_outside_menu_enemy_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Surrender;
			return Hero.MainHero.MapFaction.IsAtWarWith(Settlement.CurrentSettlement.MapFaction);
		}

		// Token: 0x0600363E RID: 13886 RVA: 0x000F1F6E File Offset: 0x000F016E
		private void caught_outside_menu_enemy_on_consequence(MenuCallbackArgs args)
		{
			GameMenu.SwitchToMenu("taken_prisoner");
		}

		// Token: 0x0600363F RID: 13887 RVA: 0x000F1F7C File Offset: 0x000F017C
		private bool game_menu_town_disguise_yourself_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.SneakIn;
			MBTextManager.SetTextVariable("SNEAK_CHANCE", MathF.Round(Campaign.Current.Models.DisguiseDetectionModel.CalculateDisguiseDetectionProbability(Settlement.CurrentSettlement) * 100f));
			return this._accessDetails.AccessLevel == SettlementAccessModel.AccessLevel.LimitedAccess && this._accessDetails.LimitedAccessSolution == SettlementAccessModel.LimitedAccessSolution.Disguise;
		}

		// Token: 0x06003640 RID: 13888 RVA: 0x000F1FE0 File Offset: 0x000F01E0
		private void game_menu_town_disguise_yourself_on_consequence(MenuCallbackArgs args)
		{
			bool flag = Campaign.Current.Models.DisguiseDetectionModel.CalculateDisguiseDetectionProbability(Settlement.CurrentSettlement) > MBRandom.RandomFloat;
			SkillLevelingManager.OnMainHeroDisguised(flag);
			Campaign.Current.IsMainHeroDisguised = true;
			if (flag)
			{
				GameMenu.SwitchToMenu("menu_sneak_into_town_succeeded");
				return;
			}
			GameMenu.SwitchToMenu("menu_sneak_into_town_caught");
		}

		// Token: 0x06003641 RID: 13889 RVA: 0x000F2038 File Offset: 0x000F0238
		private void game_menu_castle_town_sneak_grappling_hook_on_consequence(MenuCallbackArgs args)
		{
			bool flag = false;
			ItemObject @object = MBObjectManager.Instance.GetObject<ItemObject>("grappling_hook");
			for (int i = 0; i < PartyBase.MainParty.ItemRoster.Count; i++)
			{
				if (PartyBase.MainParty.ItemRoster.GetElementCopyAtIndex(i).EquipmentElement.Item == @object)
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				Campaign.Current.GameMenuManager.SetNextMenu("town_stealth");
				PlayerEncounter.LocationEncounter.CreateAndOpenMissionController(LocationComplex.Current.GetLocationWithId("center"), null, null, null);
				return;
			}
			MBInformationManager.AddQuickInformation(new TextObject("{=!}TODO You have not any grappling hook!", null), 0, null, "");
		}

		// Token: 0x06003642 RID: 13890 RVA: 0x000F20E5 File Offset: 0x000F02E5
		private bool game_menu_town_sneak_grappling_hook_on_condition(MenuCallbackArgs args)
		{
			return Campaign.Current.IsNight;
		}

		// Token: 0x06003643 RID: 13891 RVA: 0x000F20F4 File Offset: 0x000F02F4
		private bool game_menu_town_town_besiege_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.BesiegeTown;
			this.CheckFortificationAttackableHonorably(args);
			return FactionManager.IsAtWarAgainstFaction(Hero.MainHero.MapFaction, Settlement.CurrentSettlement.MapFaction) && PartyBase.MainParty.NumberOfHealthyMembers > 0 && !Settlement.CurrentSettlement.IsUnderSiege;
		}

		// Token: 0x06003644 RID: 13892 RVA: 0x000F2147 File Offset: 0x000F0347
		private void leave_siege_after_attack_on_consequence(MenuCallbackArgs args)
		{
			MobileParty.MainParty.BesiegerCamp = null;
			GameMenu.ExitToLast();
		}

		// Token: 0x06003645 RID: 13893 RVA: 0x000F2159 File Offset: 0x000F0359
		private bool leave_siege_after_attack_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Leave;
			return MobileParty.MainParty.Army == null || MobileParty.MainParty.Army.LeaderParty == MobileParty.MainParty;
		}

		// Token: 0x06003646 RID: 13894 RVA: 0x000F2187 File Offset: 0x000F0387
		private bool leave_army_after_attack_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Leave;
			return MobileParty.MainParty.Army != null && MobileParty.MainParty.Army.LeaderParty != MobileParty.MainParty;
		}

		// Token: 0x06003647 RID: 13895 RVA: 0x000F21B8 File Offset: 0x000F03B8
		private void leave_army_after_attack_on_consequence(MenuCallbackArgs args)
		{
			if (PlayerEncounter.Current != null)
			{
				PlayerEncounter.Finish(true);
			}
			else
			{
				GameMenu.ExitToLast();
			}
			if (Settlement.CurrentSettlement != null)
			{
				LeaveSettlementAction.ApplyForParty(MobileParty.MainParty);
				PartyBase.MainParty.Visuals.SetMapIconAsDirty();
			}
			MobileParty.MainParty.Army = null;
		}

		// Token: 0x06003648 RID: 13896 RVA: 0x000F2204 File Offset: 0x000F0404
		private void game_menu_town_town_besiege_on_consequence(MenuCallbackArgs args)
		{
			Settlement currentSettlement = Settlement.CurrentSettlement;
			if (PlayerEncounter.Current != null)
			{
				PlayerEncounter.Finish(true);
			}
			Campaign.Current.SiegeEventManager.StartSiegeEvent(currentSettlement, MobileParty.MainParty);
			PlayerSiege.StartPlayerSiege(BattleSideEnum.Attacker, false, null);
			PlayerSiege.StartSiegePreparation();
		}

		// Token: 0x06003649 RID: 13897 RVA: 0x000F2247 File Offset: 0x000F0447
		private void continue_siege_after_attack_on_consequence(MenuCallbackArgs args)
		{
			PlayerSiege.StartSiegePreparation();
		}

		// Token: 0x0600364A RID: 13898 RVA: 0x000F224E File Offset: 0x000F044E
		private bool continue_siege_after_attack_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Continue;
			return true;
		}

		// Token: 0x0600364B RID: 13899 RVA: 0x000F2259 File Offset: 0x000F0459
		private bool game_menu_town_outside_cheat_enter_on_condition(MenuCallbackArgs args)
		{
			return Game.Current.CheatMode;
		}

		// Token: 0x0600364C RID: 13900 RVA: 0x000F2265 File Offset: 0x000F0465
		private void game_menu_town_outside_enter_on_consequence(MenuCallbackArgs args)
		{
			GameMenu.SwitchToMenu("town");
			PlayerEncounter.LocationEncounter.IsInsideOfASettlement = true;
		}

		// Token: 0x0600364D RID: 13901 RVA: 0x000F227C File Offset: 0x000F047C
		private bool game_menu_castle_outside_leave_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Leave;
			return true;
		}

		// Token: 0x0600364E RID: 13902 RVA: 0x000F2287 File Offset: 0x000F0487
		private void game_menu_castle_outside_leave_on_consequence(MenuCallbackArgs args)
		{
			PlayerEncounter.Finish(true);
		}

		// Token: 0x0600364F RID: 13903 RVA: 0x000F2290 File Offset: 0x000F0490
		private bool game_menu_town_guard_request_shelter_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Submenu;
			if (this._accessDetails.AccessLevel == SettlementAccessModel.AccessLevel.NoAccess && this._accessDetails.AccessLimitationReason == SettlementAccessModel.AccessLimitationReason.CrimeRating)
			{
				args.Tooltip = new TextObject("{=03DZpTYi}You are a wanted criminal.", null);
				args.IsEnabled = false;
			}
			List<Location> list = Settlement.CurrentSettlement.LocationComplex.FindAll((string x) => x == "lordshall" || x == "prison").ToList<Location>();
			MenuHelper.SetIssueAndQuestDataForLocations(args, list);
			return true;
		}

		// Token: 0x06003650 RID: 13904 RVA: 0x000F2313 File Offset: 0x000F0513
		private void game_menu_request_entry_to_castle_on_consequence(MenuCallbackArgs args)
		{
			Settlement currentSettlement = Settlement.CurrentSettlement;
			if (this._accessDetails.AccessLevel == SettlementAccessModel.AccessLevel.FullAccess)
			{
				this._playerIsAlreadyInCastle = true;
				GameMenu.SwitchToMenu("menu_castle_entry_granted");
				return;
			}
			GameMenu.SwitchToMenu("menu_castle_entry_denied");
		}

		// Token: 0x06003651 RID: 13905 RVA: 0x000F2348 File Offset: 0x000F0548
		private bool game_menu_request_meeting_someone_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Conversation;
			List<Location> list = Settlement.CurrentSettlement.LocationComplex.FindAll((string x) => x == "lordshall").ToList<Location>();
			MenuHelper.SetIssueAndQuestDataForLocations(args, list);
			bool flag2;
			TextObject textObject;
			bool flag = Campaign.Current.Models.SettlementAccessModel.IsRequestMeetingOptionAvailable(Settlement.CurrentSettlement, out flag2, out textObject);
			args.Tooltip = textObject;
			args.IsEnabled = !flag2;
			return flag;
		}

		// Token: 0x06003652 RID: 13906 RVA: 0x000F23C6 File Offset: 0x000F05C6
		private void game_menu_request_meeting_someone_on_consequence(MenuCallbackArgs args)
		{
			GameMenu.SwitchToMenu("request_meeting");
		}

		// Token: 0x06003653 RID: 13907 RVA: 0x000F23D2 File Offset: 0x000F05D2
		private void game_menu_town_guard_back_on_consequence(MenuCallbackArgs args)
		{
			if (Settlement.CurrentSettlement != null && Settlement.CurrentSettlement.IsCastle)
			{
				GameMenu.SwitchToMenu("castle_outside");
				return;
			}
			GameMenu.SwitchToMenu("town_outside");
		}

		// Token: 0x06003654 RID: 13908 RVA: 0x000F23FC File Offset: 0x000F05FC
		private void game_menu_town_menu_request_meeting_on_init(MenuCallbackArgs args)
		{
			List<Hero> heroesToMeetInTown = TownHelpers.GetHeroesToMeetInTown(Settlement.CurrentSettlement);
			args.MenuContext.SetRepeatObjectList(heroesToMeetInTown);
		}

		// Token: 0x06003655 RID: 13909 RVA: 0x000F2420 File Offset: 0x000F0620
		private bool game_menu_request_meeting_with_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Conversation;
			Hero hero = args.MenuContext.GetCurrentRepeatableObject() as Hero;
			if (hero != null)
			{
				StringHelpers.SetCharacterProperties("HERO_TO_MEET", hero.CharacterObject, null, false);
				MenuHelper.SetIssueAndQuestDataForHero(args, hero);
				return true;
			}
			return false;
		}

		// Token: 0x06003656 RID: 13910 RVA: 0x000F2468 File Offset: 0x000F0668
		private void game_menu_town_menu_request_meeting_with_besiegers_on_init(MenuCallbackArgs args)
		{
			Settlement currentSettlement = Settlement.CurrentSettlement;
			if (currentSettlement.SiegeEvent == null)
			{
				if (MobileParty.MainParty.BesiegerCamp == null)
				{
					PlayerSiege.ClosePlayerSiege();
				}
				if (currentSettlement.IsTown)
				{
					GameMenu.SwitchToMenu("town");
					return;
				}
				if (currentSettlement.IsCastle)
				{
					GameMenu.SwitchToMenu("castle");
					return;
				}
				Debug.FailedAssert("non-fortification under siege", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\CampaignBehaviors\\EncounterGameMenuBehavior.cs", "game_menu_town_menu_request_meeting_with_besiegers_on_init", 2435);
			}
			List<MobileParty> list = new List<MobileParty>();
			if (currentSettlement.SiegeEvent.BesiegerCamp.BesiegerParty.Army != null)
			{
				list.Add(currentSettlement.SiegeEvent.BesiegerCamp.BesiegerParty.Army.LeaderParty);
			}
			else
			{
				list.Add(currentSettlement.SiegeEvent.BesiegerCamp.BesiegerParty);
			}
			args.MenuContext.SetRepeatObjectList(list.AsReadOnly());
		}

		// Token: 0x06003657 RID: 13911 RVA: 0x000F253C File Offset: 0x000F073C
		private bool game_menu_request_meeting_with_besiegers_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Conversation;
			Settlement currentSettlement = Settlement.CurrentSettlement;
			if (currentSettlement.SiegeEvent != null)
			{
				MobileParty mobileParty = ((currentSettlement.SiegeEvent.BesiegerCamp.BesiegerParty.Army != null) ? currentSettlement.SiegeEvent.BesiegerCamp.BesiegerParty.Army.LeaderParty : currentSettlement.SiegeEvent.BesiegerCamp.BesiegerParty);
				StringHelpers.SetCharacterProperties("PARTY_LEADER", mobileParty.LeaderHero.CharacterObject, null, false);
				return true;
			}
			return false;
		}

		// Token: 0x06003658 RID: 13912 RVA: 0x000F25C4 File Offset: 0x000F07C4
		private string GetMeetingScene(out string sceneLevel)
		{
			string text = GameSceneDataManager.Instance.MeetingScenes.GetRandomElementWithPredicate((MeetingSceneData x) => x.Culture == Settlement.CurrentSettlement.Culture).SceneID;
			if (string.IsNullOrEmpty(text))
			{
				text = GameSceneDataManager.Instance.MeetingScenes.GetRandomElement<MeetingSceneData>().SceneID;
			}
			sceneLevel = "";
			if (Settlement.CurrentSettlement.IsFortification)
			{
				sceneLevel = Campaign.Current.Models.LocationModel.GetUpgradeLevelTag(Settlement.CurrentSettlement.Town.GetWallLevel());
			}
			return text;
		}

		// Token: 0x06003659 RID: 13913 RVA: 0x000F2664 File Offset: 0x000F0864
		private void game_menu_request_meeting_with_besiegers_on_consequence(MenuCallbackArgs args)
		{
			string text;
			string meetingScene = this.GetMeetingScene(out text);
			MobileParty mobileParty = (MobileParty)args.MenuContext.GetSelectedObject();
			ConversationCharacterData conversationCharacterData = new ConversationCharacterData(Hero.MainHero.CharacterObject, PartyBase.MainParty, true, false, false, false, false, false);
			ConversationCharacterData conversationCharacterData2 = new ConversationCharacterData(ConversationHelper.GetConversationCharacterPartyLeader(mobileParty.Party), mobileParty.Party, false, false, false, false, false, false);
			CampaignMission.OpenConversationMission(conversationCharacterData, conversationCharacterData2, meetingScene, text);
		}

		// Token: 0x0600365A RID: 13914 RVA: 0x000F26CC File Offset: 0x000F08CC
		private void game_menu_request_meeting_with_on_consequence(MenuCallbackArgs args)
		{
			string text;
			string meetingScene = this.GetMeetingScene(out text);
			Hero hero = (Hero)args.MenuContext.GetSelectedObject();
			ConversationCharacterData conversationCharacterData = new ConversationCharacterData(Hero.MainHero.CharacterObject, PartyBase.MainParty, false, false, false, false, false, false);
			CharacterObject characterObject = hero.CharacterObject;
			MobileParty partyBelongedTo = hero.PartyBelongedTo;
			ConversationCharacterData conversationCharacterData2 = new ConversationCharacterData(characterObject, (partyBelongedTo != null) ? partyBelongedTo.Party : null, true, false, false, false, false, false);
			CampaignMission.OpenConversationMission(conversationCharacterData, conversationCharacterData2, meetingScene, text);
		}

		// Token: 0x0600365B RID: 13915 RVA: 0x000F273B File Offset: 0x000F093B
		private bool game_meeting_town_leave_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Leave;
			return Settlement.CurrentSettlement.IsTown;
		}

		// Token: 0x0600365C RID: 13916 RVA: 0x000F274F File Offset: 0x000F094F
		private bool game_meeting_castle_leave_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Leave;
			return Settlement.CurrentSettlement.IsCastle;
		}

		// Token: 0x0600365D RID: 13917 RVA: 0x000F2764 File Offset: 0x000F0964
		private void game_menu_request_meeting_town_leave_on_consequence(MenuCallbackArgs args)
		{
			if (PlayerSiege.PlayerSiegeEvent == null || PlayerSiege.PlayerSiegeEvent.BesiegedSettlement != Settlement.CurrentSettlement)
			{
				GameMenu.SwitchToMenu("town_guard");
				return;
			}
			GameMenu.ExitToLast();
			PlayerEncounter.LeaveEncounter = false;
			if (Hero.MainHero.CurrentSettlement != null && PlayerSiege.PlayerSiegeEvent == null)
			{
				PlayerEncounter.LeaveSettlement();
			}
			if (PlayerSiege.PlayerSiegeEvent.BesiegedSettlement.SiegeEvent != null)
			{
				PlayerSiege.StartSiegePreparation();
				return;
			}
			PlayerEncounter.Finish(true);
		}

		// Token: 0x0600365E RID: 13918 RVA: 0x000F27D4 File Offset: 0x000F09D4
		private void game_menu_request_meeting_castle_leave_on_consequence(MenuCallbackArgs args)
		{
			if (PlayerSiege.PlayerSiegeEvent == null || PlayerSiege.PlayerSiegeEvent.BesiegedSettlement != Settlement.CurrentSettlement)
			{
				GameMenu.SwitchToMenu("castle_guard");
				return;
			}
			GameMenu.ExitToLast();
			PlayerEncounter.LeaveEncounter = false;
			if (Hero.MainHero.CurrentSettlement != null && PlayerSiege.PlayerSiegeEvent == null)
			{
				PlayerEncounter.LeaveSettlement();
			}
			if (PlayerSiege.PlayerSiegeEvent.BesiegedSettlement.SiegeEvent != null)
			{
				PlayerSiege.StartSiegePreparation();
				return;
			}
			PlayerEncounter.Finish(true);
		}

		// Token: 0x0600365F RID: 13919 RVA: 0x000F2844 File Offset: 0x000F0A44
		private void game_menu_village_loot_complete_on_init(MenuCallbackArgs args)
		{
			PlayerEncounter.Update();
		}

		// Token: 0x06003660 RID: 13920 RVA: 0x000F284B File Offset: 0x000F0A4B
		private void game_menu_village_loot_complete_continue_on_consequence(MenuCallbackArgs args)
		{
			PlayerEncounter.Finish(true);
		}

		// Token: 0x06003661 RID: 13921 RVA: 0x000F2853 File Offset: 0x000F0A53
		private bool game_menu_village_loot_complete_continue_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Continue;
			return true;
		}

		// Token: 0x06003662 RID: 13922 RVA: 0x000F285E File Offset: 0x000F0A5E
		private void game_menu_raid_interrupted_continue_on_consequence(MenuCallbackArgs args)
		{
			GameMenu.SwitchToMenu("encounter");
		}

		// Token: 0x06003663 RID: 13923 RVA: 0x000F286A File Offset: 0x000F0A6A
		private bool game_menu_raid_interrupted_continue_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Continue;
			return true;
		}

		// Token: 0x06003664 RID: 13924 RVA: 0x000F2875 File Offset: 0x000F0A75
		private void break_out_menu_accept_on_consequence(MenuCallbackArgs args)
		{
			BreakInOutBesiegedSettlementAction.ApplyBreakOut(out this._breakInOutCasualties, out this._breakInOutArmyCasualties);
			GameMenu.SwitchToMenu("break_out_debrief_menu");
		}

		// Token: 0x06003665 RID: 13925 RVA: 0x000F2894 File Offset: 0x000F0A94
		private bool break_out_menu_accept_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.LeaveTroopsAndFlee;
			MobileParty mainParty = MobileParty.MainParty;
			SiegeEvent siegeEvent = Settlement.CurrentSettlement.SiegeEvent;
			int lostTroopCountForBreakingOutOfBesiegedSettlement = Campaign.Current.Models.TroopSacrificeModel.GetLostTroopCountForBreakingOutOfBesiegedSettlement(mainParty, siegeEvent);
			Army army = mainParty.Army;
			int num = ((army != null) ? army.TotalRegularCount : mainParty.MemberRoster.TotalRegulars);
			if (lostTroopCountForBreakingOutOfBesiegedSettlement > num)
			{
				args.IsEnabled = false;
				args.Tooltip = new TextObject("{=MTbOGRCF}You don't have enough men!", null);
			}
			return true;
		}

		// Token: 0x06003666 RID: 13926 RVA: 0x000F2909 File Offset: 0x000F0B09
		private void break_in_menu_accept_on_consequence(MenuCallbackArgs args)
		{
			BreakInOutBesiegedSettlementAction.ApplyBreakIn(out this._breakInOutCasualties, out this._breakInOutArmyCasualties);
			GameMenu.SwitchToMenu("break_in_debrief_menu");
		}

		// Token: 0x06003667 RID: 13927 RVA: 0x000F2928 File Offset: 0x000F0B28
		private bool break_in_menu_accept_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.LeaveTroopsAndFlee;
			MobileParty mainParty = MobileParty.MainParty;
			SiegeEvent siegeEvent = Settlement.CurrentSettlement.SiegeEvent;
			int lostTroopCountForBreakingInBesiegedSettlement = Campaign.Current.Models.TroopSacrificeModel.GetLostTroopCountForBreakingInBesiegedSettlement(mainParty, siegeEvent);
			Army army = mainParty.Army;
			int num = ((army != null) ? army.TotalRegularCount : mainParty.MemberRoster.TotalRegulars);
			if (lostTroopCountForBreakingInBesiegedSettlement > num)
			{
				args.IsEnabled = false;
				args.Tooltip = new TextObject("{=MTbOGRCF}You don't have enough men!", null);
			}
			return true;
		}

		// Token: 0x06003668 RID: 13928 RVA: 0x000F299D File Offset: 0x000F0B9D
		private void break_out_menu_reject_on_consequence(MenuCallbackArgs args)
		{
			if (PlayerSiege.PlayerSiegeEvent != null)
			{
				GameMenu.SwitchToMenu("menu_siege_strategies");
				return;
			}
			GameMenu.SwitchToMenu("encounter_interrupted_siege_preparations");
		}

		// Token: 0x06003669 RID: 13929 RVA: 0x000F29BB File Offset: 0x000F0BBB
		private bool break_out_menu_reject_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Submenu;
			return true;
		}

		// Token: 0x0600366A RID: 13930 RVA: 0x000F29C5 File Offset: 0x000F0BC5
		private void break_in_menu_reject_on_consequence(MenuCallbackArgs args)
		{
			GameMenu.SwitchToMenu("join_siege_event");
		}

		// Token: 0x0600366B RID: 13931 RVA: 0x000F29D1 File Offset: 0x000F0BD1
		private bool break_in_menu_reject_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Submenu;
			return true;
		}

		// Token: 0x0600366C RID: 13932 RVA: 0x000F29DC File Offset: 0x000F0BDC
		private void break_in_out_menu_on_init(MenuCallbackArgs args)
		{
			MobileParty mainParty = MobileParty.MainParty;
			SiegeEvent siegeEvent = Settlement.CurrentSettlement.SiegeEvent;
			int lostTroopCountForBreakingInBesiegedSettlement = Campaign.Current.Models.TroopSacrificeModel.GetLostTroopCountForBreakingInBesiegedSettlement(mainParty, siegeEvent);
			TextObject text = args.MenuContext.GameMenu.GetText();
			text.SetTextVariable("POSSIBLE_CASUALTIES", lostTroopCountForBreakingInBesiegedSettlement);
			text.SetTextVariable("PLURAL", (lostTroopCountForBreakingInBesiegedSettlement > 1) ? 1 : 0);
		}

		// Token: 0x0600366D RID: 13933 RVA: 0x000F2A44 File Offset: 0x000F0C44
		private void break_in_out_debrief_menu_on_init(MenuCallbackArgs args)
		{
			TextObject text = args.MenuContext.GameMenu.GetText();
			text.SetTextVariable("CASUALTIES", PartyBaseHelper.PrintRegularTroopCategories(this._breakInOutCasualties));
			if (this._breakInOutArmyCasualties > 0)
			{
				TextObject textObject = new TextObject("{=hxnCr8bm} Other parties of your army lost {NUMBER} {?PLURAL}troops{?}troop{\\?}.", null);
				textObject.SetTextVariable("NUMBER", this._breakInOutArmyCasualties);
				textObject.SetTextVariable("PLURAL", (this._breakInOutArmyCasualties > 1) ? 1 : 0);
				text.SetTextVariable("OTHER_CASUALTIES", textObject);
				return;
			}
			text.SetTextVariable("OTHER_CASUALTIES", TextObject.Empty);
		}

		// Token: 0x0600366E RID: 13934 RVA: 0x000F2AD8 File Offset: 0x000F0CD8
		private void break_out_debrief_continue_on_consequence(MenuCallbackArgs args)
		{
			Settlement besiegedSettlement = PlayerSiege.PlayerSiegeEvent.BesiegedSettlement;
			PlayerEncounter.Finish(true);
			besiegedSettlement.Party.Visuals.SetMapIconAsDirty();
			if (PlayerSiege.PlayerSiegeEvent != null)
			{
				PlayerSiege.ClosePlayerSiege();
			}
			PlayerEncounter.ProtectPlayerSide(1f);
		}

		// Token: 0x0600366F RID: 13935 RVA: 0x000F2B0F File Offset: 0x000F0D0F
		private bool break_out_debrief_continue_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Continue;
			return true;
		}

		// Token: 0x06003670 RID: 13936 RVA: 0x000F2B1A File Offset: 0x000F0D1A
		private bool break_in_debrief_continue_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Continue;
			return true;
		}

		// Token: 0x06003671 RID: 13937 RVA: 0x000F2B28 File Offset: 0x000F0D28
		private void break_in_debrief_continue_on_consequence(MenuCallbackArgs args)
		{
			if (Hero.MainHero.CurrentSettlement == null)
			{
				PlayerEncounter.EnterSettlement();
			}
			if (PlayerSiege.PlayerSiegeEvent == null)
			{
				PlayerSiege.StartPlayerSiege(BattleSideEnum.Defender, false, null);
			}
			if (Hero.MainHero.CurrentSettlement.Party.MapEvent != null)
			{
				GameMenu.SwitchToMenu("join_encounter");
				return;
			}
			PlayerSiege.StartSiegePreparation();
		}

		// Token: 0x06003672 RID: 13938 RVA: 0x000F2B7C File Offset: 0x000F0D7C
		[GameMenuInitializationHandler("army_encounter")]
		[GameMenuInitializationHandler("game_menu_army_talk_to_other_members")]
		private static void army_encounter_background_on_init(MenuCallbackArgs args)
		{
			if (PlayerEncounter.EncounteredMobileParty != null && PlayerEncounter.EncounteredMobileParty.Army != null)
			{
				args.MenuContext.SetBackgroundMeshName(PlayerEncounter.EncounteredMobileParty.Army.MapFaction.Culture.EncounterBackgroundMesh);
				return;
			}
			args.MenuContext.SetBackgroundMeshName("wait_fallback");
		}

		// Token: 0x06003673 RID: 13939 RVA: 0x000F2BD4 File Offset: 0x000F0DD4
		[GameMenuInitializationHandler("castle_outside")]
		[GameMenuInitializationHandler("town_outside")]
		[GameMenuInitializationHandler("fortification_crime_rating")]
		[GameMenuInitializationHandler("village_outside")]
		[GameMenuInitializationHandler("menu_sneak_into_town_succeeded")]
		private static void encounter_menu_ui_castle_on_init(MenuCallbackArgs args)
		{
			Settlement currentSettlement = Settlement.CurrentSettlement;
			args.MenuContext.SetBackgroundMeshName(currentSettlement.SettlementComponent.WaitMeshName);
		}

		// Token: 0x06003674 RID: 13940 RVA: 0x000F2BFD File Offset: 0x000F0DFD
		[GameMenuInitializationHandler("menu_castle_taken")]
		[GameMenuInitializationHandler("menu_settlement_taken")]
		private static void encounter_menu_settlement_taken_on_init(MenuCallbackArgs args)
		{
			args.MenuContext.SetBackgroundMeshName("encounter_win");
		}

		// Token: 0x06003675 RID: 13941 RVA: 0x000F2C0F File Offset: 0x000F0E0F
		[GameMenuInitializationHandler("encounter_meeting")]
		private static void game_menu_encounter_meeting_background_on_init(MenuCallbackArgs args)
		{
			args.MenuContext.SetBackgroundMeshName(PlayerEncounter.EncounteredParty.MapFaction.Culture.EncounterBackgroundMesh);
		}

		// Token: 0x06003676 RID: 13942 RVA: 0x000F2C30 File Offset: 0x000F0E30
		[GameMenuInitializationHandler("menu_castle_entry_denied")]
		private static void game_menu_castle_guard_on_init(MenuCallbackArgs args)
		{
			args.MenuContext.SetBackgroundMeshName("encounter_guards");
		}

		// Token: 0x06003677 RID: 13943 RVA: 0x000F2C42 File Offset: 0x000F0E42
		[GameMenuInitializationHandler("break_in_menu")]
		[GameMenuInitializationHandler("break_in_debrief_menu")]
		[GameMenuInitializationHandler("break_out_menu")]
		[GameMenuInitializationHandler("break_out_debrief_menu")]
		[GameMenuInitializationHandler("continue_siege_after_attack")]
		private static void game_menu_siege_background_on_init(MenuCallbackArgs args)
		{
			args.MenuContext.SetBackgroundMeshName("wait_besieging");
		}

		// Token: 0x04001166 RID: 4454
		private static readonly TextObject EnemyNotAttackableTooltip = GameTexts.FindText("str_enemy_not_attackable_tooltip", null);

		// Token: 0x04001167 RID: 4455
		private int _intEncounterVariable;

		// Token: 0x04001168 RID: 4456
		private TroopRoster _breakInOutCasualties;

		// Token: 0x04001169 RID: 4457
		private int _breakInOutArmyCasualties;

		// Token: 0x0400116A RID: 4458
		private SettlementAccessModel.AccessDetails _accessDetails;

		// Token: 0x0400116B RID: 4459
		private TroopRoster _getAwayCasualties;

		// Token: 0x0400116C RID: 4460
		private ItemRoster _getAwayLostBaggage;

		// Token: 0x0400116D RID: 4461
		private bool _playerIsAlreadyInCastle;

		// Token: 0x0400116E RID: 4462
		private const float SmugglingCrimeRate = 10f;
	}
}
