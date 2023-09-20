using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Inventory;
using TaleWorlds.CampaignSystem.Overlay;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	// Token: 0x020003C0 RID: 960
	public class PlayerTownVisitCampaignBehavior : CampaignBehaviorBase
	{
		// Token: 0x06003927 RID: 14631 RVA: 0x00105A64 File Offset: 0x00103C64
		public override void RegisterEvents()
		{
			CampaignEvents.SettlementEntered.AddNonSerializedListener(this, new Action<MobileParty, Settlement, Hero>(this.OnSettlementEntered));
			CampaignEvents.OnSettlementLeftEvent.AddNonSerializedListener(this, new Action<MobileParty, Settlement>(this.OnSettlementLeft));
			CampaignEvents.OnNewGameCreatedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnAfterNewGameCreated));
			CampaignEvents.OnGameLoadedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnAfterNewGameCreated));
		}

		// Token: 0x06003928 RID: 14632 RVA: 0x00105ACD File Offset: 0x00103CCD
		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<CampaignTime>("_lastTimeRelationGivenPathfinder", ref this._lastTimeRelationGivenPathfinder);
			dataStore.SyncData<CampaignTime>("_lastTimeRelationGivenWaterDiviner", ref this._lastTimeRelationGivenWaterDiviner);
		}

		// Token: 0x06003929 RID: 14633 RVA: 0x00105AF3 File Offset: 0x00103CF3
		public void OnAfterNewGameCreated(CampaignGameStarter campaignGameStarter)
		{
			this.AddGameMenus(campaignGameStarter);
		}

		// Token: 0x0600392A RID: 14634 RVA: 0x00105AFC File Offset: 0x00103CFC
		protected void AddGameMenus(CampaignGameStarter campaignGameSystemStarter)
		{
			campaignGameSystemStarter.AddGameMenu("town", "{=!}{SETTLEMENT_INFO}", new OnInitDelegate(PlayerTownVisitCampaignBehavior.game_menu_town_on_init), GameOverlays.MenuOverlayType.SettlementWithBoth, GameMenu.MenuFlags.None, null);
			campaignGameSystemStarter.AddGameMenuOption("town", "town_streets", "{=R5ObSaUN}Take a walk around the town center", new GameMenuOption.OnConditionDelegate(PlayerTownVisitCampaignBehavior.game_menu_town_town_streets_on_condition), new GameMenuOption.OnConsequenceDelegate(PlayerTownVisitCampaignBehavior.game_menu_town_town_streets_on_consequence), false, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("town", "town_keep", "{=!}{GO_TO_KEEP_TEXT}", new GameMenuOption.OnConditionDelegate(PlayerTownVisitCampaignBehavior.game_menu_town_go_to_keep_on_condition), new GameMenuOption.OnConsequenceDelegate(PlayerTownVisitCampaignBehavior.game_menu_town_go_to_keep_on_consequence), false, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("town", "town_arena", "{=CfDlOdTH}Go to the arena", new GameMenuOption.OnConditionDelegate(PlayerTownVisitCampaignBehavior.game_menu_town_go_to_arena_on_condition), delegate(MenuCallbackArgs x)
			{
				GameMenu.SwitchToMenu("town_arena");
			}, false, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("town", "town_backstreet", "{=l9sFJawW}Go to the tavern district", new GameMenuOption.OnConditionDelegate(PlayerTownVisitCampaignBehavior.game_menu_go_to_tavern_district_on_condition), delegate(MenuCallbackArgs x)
			{
				GameMenu.SwitchToMenu("town_backstreet");
			}, false, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("town", "manage_production", "{=dgf6q4qB}Manage town", new GameMenuOption.OnConditionDelegate(PlayerTownVisitCampaignBehavior.game_menu_town_manage_town_on_condition), null, false, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("town", "manage_production_cheat", "{=zZ3GqbzC}Manage town (Cheat)", new GameMenuOption.OnConditionDelegate(PlayerTownVisitCampaignBehavior.game_menu_town_manage_town_cheat_on_condition), null, false, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("town", "recruit_volunteers", "{=E31IJyqs}Recruit troops", new GameMenuOption.OnConditionDelegate(PlayerTownVisitCampaignBehavior.game_menu_town_recruit_troops_on_condition), new GameMenuOption.OnConsequenceDelegate(PlayerTownVisitCampaignBehavior.game_menu_recruit_volunteers_on_consequence), false, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("town", "trade", "{=GmcgoiGy}Trade", new GameMenuOption.OnConditionDelegate(PlayerTownVisitCampaignBehavior.game_menu_trade_on_condition), new GameMenuOption.OnConsequenceDelegate(PlayerTownVisitCampaignBehavior.game_menu_town_town_market_on_consequence), false, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("town", "town_smithy", "{=McHsHbH8}Enter smithy", new GameMenuOption.OnConditionDelegate(PlayerTownVisitCampaignBehavior.game_menu_craft_item_on_condition), delegate(MenuCallbackArgs x)
			{
				CraftingHelper.OpenCrafting(CraftingTemplate.All[0], null);
			}, false, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("town", "town_wait", "{=zEoHYEUS}Wait here for some time", new GameMenuOption.OnConditionDelegate(PlayerTownVisitCampaignBehavior.game_menu_wait_here_on_condition), delegate(MenuCallbackArgs x)
			{
				GameMenu.SwitchToMenu("town_wait_menus");
			}, false, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("town", "town_return_to_army", "{=SK43eB6y}Return to Army", new GameMenuOption.OnConditionDelegate(this.game_menu_return_to_army_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_return_to_army_on_consequence), false, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("town", "town_leave", "{=3sRdGQou}Leave", new GameMenuOption.OnConditionDelegate(PlayerTownVisitCampaignBehavior.game_menu_town_town_leave_on_condition), new GameMenuOption.OnConsequenceDelegate(PlayerTownVisitCampaignBehavior.game_menu_settlement_leave_on_consequence), true, -1, false, null);
			campaignGameSystemStarter.AddGameMenu("town_keep", "{=!}{SETTLEMENT_INFO}", new OnInitDelegate(PlayerTownVisitCampaignBehavior.town_keep_on_init), GameOverlays.MenuOverlayType.SettlementWithCharacters, GameMenu.MenuFlags.None, null);
			campaignGameSystemStarter.AddGameMenuOption("town_keep", "town_lords_hall", "{=dv2ZNazN}Go to the lord's hall", new GameMenuOption.OnConditionDelegate(PlayerTownVisitCampaignBehavior.game_menu_castle_go_to_lords_hall_on_condition), new GameMenuOption.OnConsequenceDelegate(PlayerTownVisitCampaignBehavior.game_menu_town_lordshall_on_consequence), false, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("town_keep", "town_lords_hall_cheat", "{=!}Go to the lord's hall (Cheat)", new GameMenuOption.OnConditionDelegate(PlayerTownVisitCampaignBehavior.game_menu_castle_go_to_lords_hall_cheat_on_condition), new GameMenuOption.OnConsequenceDelegate(PlayerTownVisitCampaignBehavior.game_menu_lordshall_cheat_on_consequence), false, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("town_keep", "town_lords_hall_go_to_dungeon", "{=etjMHPjQ}Go to dungeon", new GameMenuOption.OnConditionDelegate(PlayerTownVisitCampaignBehavior.game_menu_go_dungeon_on_condition), new GameMenuOption.OnConsequenceDelegate(PlayerTownVisitCampaignBehavior.game_menu_go_dungeon_on_consequence), false, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("town_keep", "leave_troops_to_garrison", "{=7J9KNFTz}Donate troops to garrison", new GameMenuOption.OnConditionDelegate(PlayerTownVisitCampaignBehavior.game_menu_leave_troops_garrison_on_condition), new GameMenuOption.OnConsequenceDelegate(PlayerTownVisitCampaignBehavior.game_menu_leave_troops_garrison_on_consequece), false, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("town_keep", "manage_garrison", "{=QazTA60M}Manage garrison", new GameMenuOption.OnConditionDelegate(PlayerTownVisitCampaignBehavior.game_menu_manage_garrison_on_condition), new GameMenuOption.OnConsequenceDelegate(PlayerTownVisitCampaignBehavior.game_menu_manage_garrison_on_consequence), false, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("town_keep", "open_stash", "{=xl4K9ecB}Open stash", new GameMenuOption.OnConditionDelegate(PlayerTownVisitCampaignBehavior.game_menu_town_keep_open_stash_on_condition), new GameMenuOption.OnConsequenceDelegate(PlayerTownVisitCampaignBehavior.game_menu_town_keep_open_stash_on_consequence), false, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("town_keep", "town_castle_back", "{=qWAmxyYz}Back to town center", new GameMenuOption.OnConditionDelegate(PlayerTownVisitCampaignBehavior.back_on_condition), delegate(MenuCallbackArgs x)
			{
				GameMenu.SwitchToMenu("town");
			}, true, -1, false, null);
			campaignGameSystemStarter.AddGameMenu("town_keep_dungeon", "{=!}{PRISONER_INTRODUCTION}", new OnInitDelegate(PlayerTownVisitCampaignBehavior.town_keep_dungeon_on_init), GameOverlays.MenuOverlayType.SettlementWithCharacters, GameMenu.MenuFlags.None, null);
			campaignGameSystemStarter.AddGameMenuOption("town_keep_dungeon", "town_prison", "{=UnQFawna}Enter the dungeon", new GameMenuOption.OnConditionDelegate(PlayerTownVisitCampaignBehavior.game_menu_castle_enter_the_dungeon_on_condition), new GameMenuOption.OnConsequenceDelegate(PlayerTownVisitCampaignBehavior.game_menu_town_dungeon_on_consequence), false, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("town_keep_dungeon", "town_prison_cheat", "{=KBxajw4c}Enter the dungeon (Cheat)", new GameMenuOption.OnConditionDelegate(PlayerTownVisitCampaignBehavior.game_menu_castle_go_to_dungeon_cheat_on_condition), new GameMenuOption.OnConsequenceDelegate(PlayerTownVisitCampaignBehavior.game_menu_dungeon_cheat_on_consequence), false, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("town_keep_dungeon", "town_prison_leave_prisoners", "{=kmsNUfbA}Donate prisoners", new GameMenuOption.OnConditionDelegate(PlayerTownVisitCampaignBehavior.game_menu_castle_leave_prisoners_on_condition), new GameMenuOption.OnConsequenceDelegate(PlayerTownVisitCampaignBehavior.game_menu_castle_leave_prisoners_on_consequence), false, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("town_keep_dungeon", "town_prison_manage_prisoners", "{=VXkL5Ysd}Manage prisoners", new GameMenuOption.OnConditionDelegate(PlayerTownVisitCampaignBehavior.game_menu_castle_manage_prisoners_on_condition), new GameMenuOption.OnConsequenceDelegate(PlayerTownVisitCampaignBehavior.game_menu_castle_manage_prisoners_on_consequence), false, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("town_keep_dungeon", "town_keep_dungeon_back", "{=3sRdGQou}Leave", new GameMenuOption.OnConditionDelegate(PlayerTownVisitCampaignBehavior.back_on_condition), delegate(MenuCallbackArgs x)
			{
				GameMenu.SwitchToMenu("town_keep");
			}, true, -1, false, null);
			campaignGameSystemStarter.AddGameMenu("town_keep_bribe", "{=yyz111nn}The guards say that they can't just let anyone in.", new OnInitDelegate(PlayerTownVisitCampaignBehavior.town_keep_bribe_on_init), GameOverlays.MenuOverlayType.SettlementWithCharacters, GameMenu.MenuFlags.None, null);
			campaignGameSystemStarter.AddGameMenuOption("town_keep_bribe", "town_keep_bribe_pay", "{=fxEka7Bm}Pay a {AMOUNT}{GOLD_ICON} bribe to enter the keep", new GameMenuOption.OnConditionDelegate(PlayerTownVisitCampaignBehavior.game_menu_town_keep_bribe_pay_bribe_on_condition), new GameMenuOption.OnConsequenceDelegate(PlayerTownVisitCampaignBehavior.game_menu_town_keep_bribe_pay_bribe_on_consequence), false, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("town_keep_bribe", "town_keep_bribe_back", "{=3sRdGQou}Leave", new GameMenuOption.OnConditionDelegate(PlayerTownVisitCampaignBehavior.back_on_condition), delegate(MenuCallbackArgs x)
			{
				GameMenu.SwitchToMenu("town");
			}, true, -1, false, null);
			campaignGameSystemStarter.AddGameMenu("town_enemy_town_keep", "{=!}{SCOUT_KEEP_TEXT}", new OnInitDelegate(PlayerTownVisitCampaignBehavior.town_enemy_keep_on_init), GameOverlays.MenuOverlayType.SettlementWithCharacters, GameMenu.MenuFlags.None, null);
			campaignGameSystemStarter.AddGameMenuOption("town_enemy_town_keep", "settlement_go_back_to_center", "{=3sRdGQou}Leave", new GameMenuOption.OnConditionDelegate(PlayerTownVisitCampaignBehavior.back_on_condition), delegate(MenuCallbackArgs x)
			{
				GameMenu.SwitchToMenu("town");
			}, true, -1, false, null);
			campaignGameSystemStarter.AddGameMenu("town_backstreet", "{=Zwy8JybD}You are in the backstreets. The local tavern seems to be attracting its usual crowd.", new OnInitDelegate(PlayerTownVisitCampaignBehavior.town_backstreet_on_init), GameOverlays.MenuOverlayType.SettlementWithBoth, GameMenu.MenuFlags.None, null);
			campaignGameSystemStarter.AddGameMenuOption("town_backstreet", "town_tavern", "{=qcl3YTPh}Visit the tavern", new GameMenuOption.OnConditionDelegate(PlayerTownVisitCampaignBehavior.visit_the_tavern_on_condition), new GameMenuOption.OnConsequenceDelegate(PlayerTownVisitCampaignBehavior.game_menu_town_town_tavern_on_consequence), false, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("town_backstreet", "sell_all_prisoners", "{=xZIBKK0v}Ransom your prisoners ({RANSOM_AMOUNT}{GOLD_ICON})", new GameMenuOption.OnConditionDelegate(PlayerTownVisitCampaignBehavior.SellPrisonersCondition), delegate(MenuCallbackArgs x)
			{
				PlayerTownVisitCampaignBehavior.SellAllTransferablePrisoners();
			}, false, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("town_backstreet", "sell_some_prisoners", "{=Q8VN9UCq}Choose the prisoners to be ransomed", new GameMenuOption.OnConditionDelegate(PlayerTownVisitCampaignBehavior.SellPrisonerOneStackCondition), delegate(MenuCallbackArgs x)
			{
				PlayerTownVisitCampaignBehavior.ChooseRansomPrisoners();
			}, false, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("town_backstreet", "town_backstreet_back", "{=qWAmxyYz}Back to town center", new GameMenuOption.OnConditionDelegate(PlayerTownVisitCampaignBehavior.back_on_condition), delegate(MenuCallbackArgs x)
			{
				GameMenu.SwitchToMenu("town");
			}, true, -1, false, null);
			campaignGameSystemStarter.AddGameMenu("town_arena", "{=5id9mGrc}You are near the arena. {ADDITIONAL_STRING}", new OnInitDelegate(PlayerTownVisitCampaignBehavior.town_arena_on_init), GameOverlays.MenuOverlayType.SettlementWithCharacters, GameMenu.MenuFlags.None, null);
			campaignGameSystemStarter.AddGameMenuOption("town_arena", "town_enter_arena", "{=YQ3vm6er}Enter the arena", new GameMenuOption.OnConditionDelegate(PlayerTownVisitCampaignBehavior.game_menu_town_enter_the_arena_on_condition), new GameMenuOption.OnConsequenceDelegate(PlayerTownVisitCampaignBehavior.game_menu_town_town_arena_on_consequence), false, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("town_arena", "town_arena_back", "{=qWAmxyYz}Back to town center", new GameMenuOption.OnConditionDelegate(PlayerTownVisitCampaignBehavior.back_on_condition), delegate(MenuCallbackArgs x)
			{
				GameMenu.SwitchToMenu("town");
			}, true, -1, false, null);
			campaignGameSystemStarter.AddGameMenu("settlement_player_unconscious", "{=S5OEsjwg}You slip into unconsciousness. After a little while some of the friendlier locals manage to bring you around. A little confused but without any serious injuries, you resolve to be more careful next time.", null, GameOverlays.MenuOverlayType.SettlementWithBoth, GameMenu.MenuFlags.None, null);
			campaignGameSystemStarter.AddGameMenuOption("settlement_player_unconscious", "continue", "{=DM6luo3c}Continue", new GameMenuOption.OnConditionDelegate(PlayerTownVisitCampaignBehavior.continue_on_condition), new GameMenuOption.OnConsequenceDelegate(PlayerTownVisitCampaignBehavior.settlement_player_unconscious_continue_on_consequence), false, -1, false, null);
			campaignGameSystemStarter.AddWaitGameMenu("settlement_wait", "{=8AbHxCM8}{CAPTIVITY_TEXT}\nWaiting in captivity...", new OnInitDelegate(PlayerTownVisitCampaignBehavior.settlement_wait_on_init), new OnConditionDelegate(PlayerTownVisitCampaignBehavior.wait_menu_prisoner_wait_on_condition), null, new OnTickDelegate(PlayerTownVisitCampaignBehavior.wait_menu_settlement_wait_on_tick), GameMenu.MenuAndOptionType.WaitMenuHideProgressAndHoursOption, GameOverlays.MenuOverlayType.None, 0f, GameMenu.MenuFlags.None, null);
			campaignGameSystemStarter.AddWaitGameMenu("town_wait_menus", "{=ydbVysqv}You are waiting in {CURRENT_SETTLEMENT}.", new OnInitDelegate(this.game_menu_settlement_wait_on_init), new OnConditionDelegate(PlayerTownVisitCampaignBehavior.game_menu_town_wait_on_condition), null, delegate(MenuCallbackArgs args, CampaignTime dt)
			{
				this.SwitchToMenuIfThereIsAnInterrupt(args.MenuContext.GameMenu.StringId);
			}, GameMenu.MenuAndOptionType.WaitMenuHideProgressAndHoursOption, GameOverlays.MenuOverlayType.SettlementWithBoth, 0f, GameMenu.MenuFlags.None, null);
			campaignGameSystemStarter.AddGameMenuOption("town_wait_menus", "wait_leave", "{=UqDNAZqM}Stop waiting", new GameMenuOption.OnConditionDelegate(PlayerTownVisitCampaignBehavior.back_on_condition), delegate(MenuCallbackArgs args)
			{
				PlayerEncounter.Current.IsPlayerWaiting = false;
				this.SwitchToMenuIfThereIsAnInterrupt(args.MenuContext.GameMenu.StringId);
			}, true, -1, false, null);
			campaignGameSystemStarter.AddGameMenu("castle", "{=!}{SETTLEMENT_INFO}", new OnInitDelegate(PlayerTownVisitCampaignBehavior.game_menu_castle_on_init), GameOverlays.MenuOverlayType.SettlementWithBoth, GameMenu.MenuFlags.None, null);
			campaignGameSystemStarter.AddGameMenuOption("castle", "take_a_walk_around_the_castle", "{=R92XzKXE}Take a walk around the castle", new GameMenuOption.OnConditionDelegate(PlayerTownVisitCampaignBehavior.game_menu_castle_take_a_walk_on_condition), new GameMenuOption.OnConsequenceDelegate(PlayerTownVisitCampaignBehavior.game_menu_castle_take_a_walk_around_the_castle_on_consequence), false, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("castle", "castle_lords_hall", "{=dv2ZNazN}Go to the lord's hall", new GameMenuOption.OnConditionDelegate(PlayerTownVisitCampaignBehavior.game_menu_castle_go_to_lords_hall_on_condition), new GameMenuOption.OnConsequenceDelegate(PlayerTownVisitCampaignBehavior.game_menu_castle_lordshall_on_consequence), false, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("castle", "castle_lords_hall_cheat", "{=dl6YxNTT}Go to the lord's hall (Cheat)", new GameMenuOption.OnConditionDelegate(PlayerTownVisitCampaignBehavior.game_menu_castle_go_to_lords_hall_cheat_on_condition), new GameMenuOption.OnConsequenceDelegate(PlayerTownVisitCampaignBehavior.game_menu_lordshall_cheat_on_consequence), false, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("castle", "castle_prison", "{=esSm5V6t}Go to the dungeon", new GameMenuOption.OnConditionDelegate(PlayerTownVisitCampaignBehavior.game_menu_castle_go_to_the_dungeon_on_condition), new GameMenuOption.OnConsequenceDelegate(PlayerTownVisitCampaignBehavior.game_menu_keep_dungeon_on_consequence), false, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("castle", "castle_prison_cheat", "{=pa7oiQb1}Go to the dungeon (Cheat)", new GameMenuOption.OnConditionDelegate(PlayerTownVisitCampaignBehavior.game_menu_castle_go_to_dungeon_cheat_on_condition), new GameMenuOption.OnConsequenceDelegate(PlayerTownVisitCampaignBehavior.game_menu_dungeon_cheat_on_consequence), false, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("castle", "manage_garrison", "{=QazTA60M}Manage garrison", new GameMenuOption.OnConditionDelegate(PlayerTownVisitCampaignBehavior.game_menu_manage_garrison_on_condition), new GameMenuOption.OnConsequenceDelegate(PlayerTownVisitCampaignBehavior.game_menu_manage_garrison_on_consequence), false, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("castle", "manage_production", "{=Ll1EJHXF}Manage castle", new GameMenuOption.OnConditionDelegate(PlayerTownVisitCampaignBehavior.game_menu_manage_castle_on_condition), null, false, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("castle", "open_stash", "{=xl4K9ecB}Open stash", new GameMenuOption.OnConditionDelegate(PlayerTownVisitCampaignBehavior.game_menu_town_keep_open_stash_on_condition), new GameMenuOption.OnConsequenceDelegate(PlayerTownVisitCampaignBehavior.game_menu_town_keep_open_stash_on_consequence), false, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("castle", "leave_troops_to_garrison", "{=7J9KNFTz}Donate troops to garrison", new GameMenuOption.OnConditionDelegate(PlayerTownVisitCampaignBehavior.game_menu_leave_troops_garrison_on_condition), new GameMenuOption.OnConsequenceDelegate(PlayerTownVisitCampaignBehavior.game_menu_leave_troops_garrison_on_consequece), false, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("castle", "town_wait", "{=zEoHYEUS}Wait here for some time", new GameMenuOption.OnConditionDelegate(PlayerTownVisitCampaignBehavior.game_menu_wait_here_on_condition), delegate(MenuCallbackArgs x)
			{
				GameMenu.SwitchToMenu("town_wait_menus");
			}, false, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("castle", "castle_return_to_army", "{=SK43eB6y}Return to Army", new GameMenuOption.OnConditionDelegate(this.game_menu_return_to_army_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_return_to_army_on_consequence), false, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("castle", "leave", "{=3sRdGQou}Leave", new GameMenuOption.OnConditionDelegate(PlayerTownVisitCampaignBehavior.game_menu_town_town_leave_on_condition), new GameMenuOption.OnConsequenceDelegate(PlayerTownVisitCampaignBehavior.game_menu_settlement_leave_on_consequence), true, -1, false, null);
			campaignGameSystemStarter.AddGameMenu("castle_dungeon", "{=!}{PRISONER_INTRODUCTION}", new OnInitDelegate(PlayerTownVisitCampaignBehavior.town_keep_dungeon_on_init), GameOverlays.MenuOverlayType.SettlementWithCharacters, GameMenu.MenuFlags.None, null);
			campaignGameSystemStarter.AddGameMenuOption("castle_dungeon", "town_prison", "{=UnQFawna}Enter the dungeon", new GameMenuOption.OnConditionDelegate(PlayerTownVisitCampaignBehavior.game_menu_castle_enter_the_dungeon_on_condition), new GameMenuOption.OnConsequenceDelegate(PlayerTownVisitCampaignBehavior.game_menu_castle_dungeon_on_consequence), false, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("castle_dungeon", "town_prison_cheat", "{=KBxajw4c}Enter the dungeon (Cheat)", new GameMenuOption.OnConditionDelegate(PlayerTownVisitCampaignBehavior.game_menu_castle_go_to_dungeon_cheat_on_condition), new GameMenuOption.OnConsequenceDelegate(PlayerTownVisitCampaignBehavior.game_menu_dungeon_cheat_on_consequence), false, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("castle_dungeon", "town_prison_leave_prisoners", "{=kmsNUfbA}Donate prisoners", new GameMenuOption.OnConditionDelegate(PlayerTownVisitCampaignBehavior.game_menu_castle_leave_prisoners_on_condition), new GameMenuOption.OnConsequenceDelegate(PlayerTownVisitCampaignBehavior.game_menu_castle_leave_prisoners_on_consequence), false, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("castle_dungeon", "town_prison_manage_prisoners", "{=VXkL5Ysd}Manage prisoners", new GameMenuOption.OnConditionDelegate(PlayerTownVisitCampaignBehavior.game_menu_castle_manage_prisoners_on_condition), new GameMenuOption.OnConsequenceDelegate(PlayerTownVisitCampaignBehavior.game_menu_castle_manage_prisoners_on_consequence), false, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("castle_dungeon", "town_keep_dungeon_back", "{=3sRdGQou}Leave", new GameMenuOption.OnConditionDelegate(PlayerTownVisitCampaignBehavior.back_on_condition), delegate(MenuCallbackArgs x)
			{
				GameMenu.SwitchToMenu("castle");
			}, true, -1, false, null);
			campaignGameSystemStarter.AddGameMenu("village", "{=!}{SETTLEMENT_INFO}", new OnInitDelegate(PlayerTownVisitCampaignBehavior.game_menu_village_on_init), GameOverlays.MenuOverlayType.SettlementWithBoth, GameMenu.MenuFlags.None, null);
			campaignGameSystemStarter.AddGameMenuOption("village", "village_center", "{=U4azeSib}Take a walk through the lands", new GameMenuOption.OnConditionDelegate(PlayerTownVisitCampaignBehavior.game_menu_village_village_center_on_condition), new GameMenuOption.OnConsequenceDelegate(PlayerTownVisitCampaignBehavior.game_menu_village_village_center_on_consequence), false, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("village", "recruit_volunteers", "{=E31IJyqs}Recruit troops", new GameMenuOption.OnConditionDelegate(PlayerTownVisitCampaignBehavior.game_menu_recruit_volunteers_on_condition), new GameMenuOption.OnConsequenceDelegate(PlayerTownVisitCampaignBehavior.game_menu_recruit_volunteers_on_consequence), false, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("village", "trade", "{=VN4ctHIU}Buy products", new GameMenuOption.OnConditionDelegate(PlayerTownVisitCampaignBehavior.game_menu_village_buy_good_on_condition), null, false, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("village", "village_wait", "{=zEoHYEUS}Wait here for some time", new GameMenuOption.OnConditionDelegate(PlayerTownVisitCampaignBehavior.game_menu_wait_here_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_wait_village_on_consequence), false, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("village", "village_return_to_army", "{=SK43eB6y}Return to Army", new GameMenuOption.OnConditionDelegate(this.game_menu_return_to_army_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_return_to_army_on_consequence), false, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("village", "leave", "{=3sRdGQou}Leave", new GameMenuOption.OnConditionDelegate(PlayerTownVisitCampaignBehavior.game_menu_town_town_leave_on_condition), new GameMenuOption.OnConsequenceDelegate(PlayerTownVisitCampaignBehavior.game_menu_settlement_leave_on_consequence), true, -1, false, null);
			campaignGameSystemStarter.AddWaitGameMenu("village_wait_menus", "{=lsBuV9W7}You are waiting in the village.", new OnInitDelegate(this.game_menu_settlement_wait_on_init), new OnConditionDelegate(PlayerTownVisitCampaignBehavior.game_menu_village_wait_on_condition), null, delegate(MenuCallbackArgs args, CampaignTime dt)
			{
				this.SwitchToMenuIfThereIsAnInterrupt(args.MenuContext.GameMenu.StringId);
			}, GameMenu.MenuAndOptionType.WaitMenuHideProgressAndHoursOption, GameOverlays.MenuOverlayType.SettlementWithBoth, 0f, GameMenu.MenuFlags.None, null);
			campaignGameSystemStarter.AddGameMenuOption("village_wait_menus", "wait_leave", "{=UqDNAZqM}Stop waiting", new GameMenuOption.OnConditionDelegate(PlayerTownVisitCampaignBehavior.back_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_stop_waiting_at_village_on_consequence), true, -1, false, null);
			campaignGameSystemStarter.AddWaitGameMenu("prisoner_wait", "{=!}{CAPTIVITY_TEXT}", new OnInitDelegate(PlayerTownVisitCampaignBehavior.wait_menu_prisoner_wait_on_init), new OnConditionDelegate(PlayerTownVisitCampaignBehavior.wait_menu_prisoner_wait_on_condition), null, new OnTickDelegate(PlayerTownVisitCampaignBehavior.wait_menu_prisoner_wait_on_tick), GameMenu.MenuAndOptionType.WaitMenuHideProgressAndHoursOption, GameOverlays.MenuOverlayType.None, 0f, GameMenu.MenuFlags.None, null);
		}

		// Token: 0x0600392B RID: 14635 RVA: 0x00106974 File Offset: 0x00104B74
		private void game_menu_settlement_wait_on_init(MenuCallbackArgs args)
		{
			string text = (PlayerEncounter.EncounterSettlement.IsVillage ? "village" : (PlayerEncounter.EncounterSettlement.IsTown ? "town" : (PlayerEncounter.EncounterSettlement.IsCastle ? "castle" : null)));
			if (text != null)
			{
				PlayerTownVisitCampaignBehavior.UpdateMenuLocations(text);
			}
			if (PlayerEncounter.Current != null)
			{
				PlayerEncounter.Current.IsPlayerWaiting = true;
			}
		}

		// Token: 0x0600392C RID: 14636 RVA: 0x001069D8 File Offset: 0x00104BD8
		private static void OpenMissionWithSettingPreviousLocation(string previousLocationId, string missionLocationId)
		{
			Campaign.Current.GameMenuManager.NextLocation = LocationComplex.Current.GetLocationWithId(missionLocationId);
			Campaign.Current.GameMenuManager.PreviousLocation = LocationComplex.Current.GetLocationWithId(previousLocationId);
			PlayerEncounter.LocationEncounter.CreateAndOpenMissionController(Campaign.Current.GameMenuManager.NextLocation, null, null, null);
			Campaign.Current.GameMenuManager.NextLocation = null;
			Campaign.Current.GameMenuManager.PreviousLocation = null;
		}

		// Token: 0x0600392D RID: 14637 RVA: 0x00106A56 File Offset: 0x00104C56
		private void game_menu_stop_waiting_at_village_on_consequence(MenuCallbackArgs args)
		{
			EnterSettlementAction.ApplyForParty(MobileParty.MainParty, MobileParty.MainParty.LastVisitedSettlement);
			GameMenu.SwitchToMenu("village");
		}

		// Token: 0x0600392E RID: 14638 RVA: 0x00106A76 File Offset: 0x00104C76
		private static bool continue_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Continue;
			return true;
		}

		// Token: 0x0600392F RID: 14639 RVA: 0x00106A84 File Offset: 0x00104C84
		private static bool game_menu_castle_go_to_the_dungeon_on_condition(MenuCallbackArgs args)
		{
			SettlementAccessModel.AccessDetails accessDetails;
			Campaign.Current.Models.SettlementAccessModel.CanMainHeroEnterDungeon(Settlement.CurrentSettlement, out accessDetails);
			if (accessDetails.AccessLevel == SettlementAccessModel.AccessLevel.LimitedAccess)
			{
				args.Tooltip = new TextObject("{=aPoS8wOW}You have limited access to Dungeon.", null);
				args.IsEnabled = false;
			}
			if (FactionManager.IsAtWarAgainstFaction(Settlement.CurrentSettlement.MapFaction, Hero.MainHero.MapFaction))
			{
				args.Tooltip = new TextObject("{=h9i9VXLd}You cannot enter an enemy lord's hall.", null);
				args.IsEnabled = false;
			}
			args.optionLeaveType = GameMenuOption.LeaveType.Submenu;
			List<Location> list = Settlement.CurrentSettlement.LocationComplex.FindAll((string x) => x == "prison").ToList<Location>();
			MenuHelper.SetIssueAndQuestDataForLocations(args, list);
			return true;
		}

		// Token: 0x06003930 RID: 14640 RVA: 0x00106B44 File Offset: 0x00104D44
		private static bool game_menu_castle_enter_the_dungeon_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Mission;
			List<Location> list = Settlement.CurrentSettlement.LocationComplex.FindAll((string x) => x == "prison").ToList<Location>();
			MenuHelper.SetIssueAndQuestDataForLocations(args, list);
			return true;
		}

		// Token: 0x06003931 RID: 14641 RVA: 0x00106B94 File Offset: 0x00104D94
		private static bool game_menu_castle_go_to_dungeon_cheat_on_condition(MenuCallbackArgs args)
		{
			return Game.Current.IsDevelopmentMode;
		}

		// Token: 0x06003932 RID: 14642 RVA: 0x00106BA0 File Offset: 0x00104DA0
		private static bool game_menu_castle_leave_prisoners_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.DonatePrisoners;
			Settlement currentSettlement = Settlement.CurrentSettlement;
			if (currentSettlement.IsFortification)
			{
				if (currentSettlement.Party != null && currentSettlement.Party.PrisonerSizeLimit <= currentSettlement.Party.NumberOfPrisoners)
				{
					args.IsEnabled = false;
					args.Tooltip = GameTexts.FindText("str_dungeon_size_limit_exceeded", null);
					args.Tooltip.SetTextVariable("TROOP_NUMBER", currentSettlement.Party.NumberOfPrisoners);
				}
				return currentSettlement.OwnerClan != Clan.PlayerClan && currentSettlement.MapFaction == Hero.MainHero.MapFaction;
			}
			return false;
		}

		// Token: 0x06003933 RID: 14643 RVA: 0x00106C3C File Offset: 0x00104E3C
		private static bool game_menu_castle_manage_prisoners_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.ManagePrisoners;
			Settlement currentSettlement = Settlement.CurrentSettlement;
			return currentSettlement.OwnerClan == Clan.PlayerClan && currentSettlement.MapFaction == Hero.MainHero.MapFaction && currentSettlement.IsFortification;
		}

		// Token: 0x06003934 RID: 14644 RVA: 0x00106C7E File Offset: 0x00104E7E
		private static void game_menu_castle_leave_prisoners_on_consequence(MenuCallbackArgs args)
		{
			PartyScreenManager.OpenScreenAsDonatePrisoners();
		}

		// Token: 0x06003935 RID: 14645 RVA: 0x00106C85 File Offset: 0x00104E85
		private static void game_menu_castle_manage_prisoners_on_consequence(MenuCallbackArgs args)
		{
			PartyScreenManager.OpenScreenAsManagePrisoners();
		}

		// Token: 0x06003936 RID: 14646 RVA: 0x00106C8C File Offset: 0x00104E8C
		private static bool game_menu_town_go_to_keep_on_condition(MenuCallbackArgs args)
		{
			TextObject textObject = new TextObject("{=XZFQ1Jf6}Go to the keep", null);
			SettlementAccessModel.AccessDetails accessDetails;
			Campaign.Current.Models.SettlementAccessModel.CanMainHeroEnterLordsHall(Settlement.CurrentSettlement, out accessDetails);
			if (accessDetails.AccessLevel == SettlementAccessModel.AccessLevel.NoAccess)
			{
				args.IsEnabled = false;
				SettlementAccessModel.AccessLimitationReason accessLimitationReason = accessDetails.AccessLimitationReason;
				if (accessLimitationReason != SettlementAccessModel.AccessLimitationReason.HostileFaction)
				{
					if (accessLimitationReason != SettlementAccessModel.AccessLimitationReason.Disguised)
					{
						if (accessLimitationReason != SettlementAccessModel.AccessLimitationReason.LocationEmpty)
						{
							Debug.FailedAssert(string.Format("{0} is not a valid no access reason for town keep", accessDetails.AccessLimitationReason), "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\CampaignBehaviors\\PlayerTownVisitCampaignBehavior.cs", "game_menu_town_go_to_keep_on_condition", 383);
						}
						else
						{
							args.Tooltip = new TextObject("{=cojKmfSk}There is no one inside.", null);
						}
					}
					else
					{
						args.Tooltip = new TextObject("{=f91LSbdx}You cannot enter the keep while in disguise.", null);
					}
				}
				else
				{
					args.Tooltip = new TextObject("{=b3shPt8Q}You cannot enter an enemy keep.", null);
				}
			}
			else if (accessDetails.AccessLevel == SettlementAccessModel.AccessLevel.LimitedAccess && accessDetails.LimitedAccessSolution == SettlementAccessModel.LimitedAccessSolution.Disguise)
			{
				textObject = new TextObject("{=1GPa9aTQ}Scout the keep", null);
				args.Tooltip = new TextObject("{=ubOtRU3u}You have limited access to keep while in disguise.", null);
			}
			MBTextManager.SetTextVariable("GO_TO_KEEP_TEXT", textObject, false);
			List<Location> list = Settlement.CurrentSettlement.LocationComplex.FindAll((string x) => x == "lordshall" || x == "prison").ToList<Location>();
			MenuHelper.SetIssueAndQuestDataForLocations(args, list);
			args.optionLeaveType = GameMenuOption.LeaveType.Submenu;
			return true;
		}

		// Token: 0x06003937 RID: 14647 RVA: 0x00106DCC File Offset: 0x00104FCC
		private static bool game_menu_go_to_tavern_district_on_condition(MenuCallbackArgs args)
		{
			bool flag2;
			TextObject textObject;
			bool flag = Campaign.Current.Models.SettlementAccessModel.CanMainHeroAccessLocation(Settlement.CurrentSettlement, "tavern", out flag2, out textObject);
			List<Location> list = Settlement.CurrentSettlement.LocationComplex.FindAll((string x) => x == "tavern").ToList<Location>();
			MenuHelper.SetIssueAndQuestDataForLocations(args, list);
			args.optionLeaveType = GameMenuOption.LeaveType.Submenu;
			return MenuHelper.SetOptionProperties(args, flag, flag2, textObject);
		}

		// Token: 0x06003938 RID: 14648 RVA: 0x00106E48 File Offset: 0x00105048
		private static bool game_menu_trade_on_condition(MenuCallbackArgs args)
		{
			bool flag2;
			TextObject textObject;
			bool flag = Campaign.Current.Models.SettlementAccessModel.CanMainHeroDoSettlementAction(Settlement.CurrentSettlement, SettlementAccessModel.SettlementAction.Trade, out flag2, out textObject);
			args.optionLeaveType = GameMenuOption.LeaveType.Trade;
			return MenuHelper.SetOptionProperties(args, flag, flag2, textObject);
		}

		// Token: 0x06003939 RID: 14649 RVA: 0x00106E88 File Offset: 0x00105088
		private static bool game_menu_town_recruit_troops_on_condition(MenuCallbackArgs args)
		{
			bool flag2;
			TextObject textObject;
			bool flag = Campaign.Current.Models.SettlementAccessModel.CanMainHeroDoSettlementAction(Settlement.CurrentSettlement, SettlementAccessModel.SettlementAction.RecruitTroops, out flag2, out textObject);
			args.optionLeaveType = GameMenuOption.LeaveType.Recruit;
			return MenuHelper.SetOptionProperties(args, flag, flag2, textObject);
		}

		// Token: 0x0600393A RID: 14650 RVA: 0x00106EC8 File Offset: 0x001050C8
		private static bool game_menu_wait_here_on_condition(MenuCallbackArgs args)
		{
			bool flag2;
			TextObject textObject;
			bool flag = Campaign.Current.Models.SettlementAccessModel.CanMainHeroDoSettlementAction(Settlement.CurrentSettlement, SettlementAccessModel.SettlementAction.WaitInSettlement, out flag2, out textObject);
			args.optionLeaveType = GameMenuOption.LeaveType.Wait;
			return MenuHelper.SetOptionProperties(args, flag, flag2, textObject);
		}

		// Token: 0x0600393B RID: 14651 RVA: 0x00106F05 File Offset: 0x00105105
		private void game_menu_wait_village_on_consequence(MenuCallbackArgs args)
		{
			GameMenu.SwitchToMenu("village_wait_menus");
			LeaveSettlementAction.ApplyForParty(MobileParty.MainParty);
		}

		// Token: 0x0600393C RID: 14652 RVA: 0x00106F1B File Offset: 0x0010511B
		private bool game_menu_return_to_army_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Wait;
			return MobileParty.MainParty.Army != null && MobileParty.MainParty.Army.LeaderParty != MobileParty.MainParty;
		}

		// Token: 0x0600393D RID: 14653 RVA: 0x00106F4C File Offset: 0x0010514C
		private void game_menu_return_to_army_on_consequence(MenuCallbackArgs args)
		{
			GameMenu.SwitchToMenu("army_wait_at_settlement");
			if (MobileParty.MainParty.CurrentSettlement.IsVillage)
			{
				PlayerEncounter.LeaveSettlement();
				PlayerEncounter.Finish(true);
			}
		}

		// Token: 0x0600393E RID: 14654 RVA: 0x00106F74 File Offset: 0x00105174
		private static bool game_menu_castle_take_a_walk_on_condition(MenuCallbackArgs args)
		{
			List<Location> list = Settlement.CurrentSettlement.LocationComplex.FindAll((string x) => x == "center").ToList<Location>();
			MenuHelper.SetIssueAndQuestDataForLocations(args, list);
			args.optionLeaveType = GameMenuOption.LeaveType.Mission;
			return true;
		}

		// Token: 0x0600393F RID: 14655 RVA: 0x00106FC4 File Offset: 0x001051C4
		private static void game_menu_town_go_to_keep_on_consequence(MenuCallbackArgs args)
		{
			SettlementAccessModel.AccessDetails accessDetails;
			Campaign.Current.Models.SettlementAccessModel.CanMainHeroEnterLordsHall(Settlement.CurrentSettlement, out accessDetails);
			SettlementAccessModel.AccessLevel accessLevel = accessDetails.AccessLevel;
			int num = (int)accessLevel;
			if (num != 1)
			{
				if (num == 2)
				{
					GameMenu.SwitchToMenu("town_keep");
					return;
				}
			}
			else
			{
				if (accessDetails.LimitedAccessSolution == SettlementAccessModel.LimitedAccessSolution.Bribe)
				{
					GameMenu.SwitchToMenu("town_keep_bribe");
					return;
				}
				if (accessDetails.LimitedAccessSolution == SettlementAccessModel.LimitedAccessSolution.Disguise)
				{
					GameMenu.SwitchToMenu("town_enemy_town_keep");
					return;
				}
			}
			Debug.FailedAssert("invalid LimitedAccessSolution or AccessLevel for town keep", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\CampaignBehaviors\\PlayerTownVisitCampaignBehavior.cs", "game_menu_town_go_to_keep_on_consequence", 489);
		}

		// Token: 0x06003940 RID: 14656 RVA: 0x0010704C File Offset: 0x0010524C
		private static bool game_menu_go_dungeon_on_condition(MenuCallbackArgs args)
		{
			SettlementAccessModel.AccessDetails accessDetails;
			Campaign.Current.Models.SettlementAccessModel.CanMainHeroEnterDungeon(Settlement.CurrentSettlement, out accessDetails);
			if (Settlement.CurrentSettlement.BribePaid < Campaign.Current.Models.BribeCalculationModel.GetBribeToEnterDungeon(Settlement.CurrentSettlement) && accessDetails.AccessLevel == SettlementAccessModel.AccessLevel.LimitedAccess)
			{
				args.Tooltip = new TextObject("{=aPoS8wOW}You have limited access to Dungeon.", null);
				args.IsEnabled = false;
			}
			args.optionLeaveType = GameMenuOption.LeaveType.Submenu;
			List<Location> list = Settlement.CurrentSettlement.LocationComplex.FindAll((string x) => x == "prison").ToList<Location>();
			MenuHelper.SetIssueAndQuestDataForLocations(args, list);
			return true;
		}

		// Token: 0x06003941 RID: 14657 RVA: 0x00107102 File Offset: 0x00105302
		private static void game_menu_go_dungeon_on_consequence(MenuCallbackArgs args)
		{
			GameMenu.SwitchToMenu("town_keep_dungeon");
		}

		// Token: 0x06003942 RID: 14658 RVA: 0x0010710E File Offset: 0x0010530E
		private static bool back_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Leave;
			return true;
		}

		// Token: 0x06003943 RID: 14659 RVA: 0x00107119 File Offset: 0x00105319
		private static bool visit_the_tavern_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Mission;
			return true;
		}

		// Token: 0x06003944 RID: 14660 RVA: 0x00107124 File Offset: 0x00105324
		private static bool game_menu_town_go_to_arena_on_condition(MenuCallbackArgs args)
		{
			bool flag2;
			TextObject textObject;
			bool flag = Campaign.Current.Models.SettlementAccessModel.CanMainHeroAccessLocation(Settlement.CurrentSettlement, "arena", out flag2, out textObject);
			List<Location> list = Settlement.CurrentSettlement.LocationComplex.FindAll((string x) => x == "arena").ToList<Location>();
			MenuHelper.SetIssueAndQuestDataForLocations(args, list);
			args.optionLeaveType = GameMenuOption.LeaveType.Submenu;
			return MenuHelper.SetOptionProperties(args, flag, flag2, textObject);
		}

		// Token: 0x06003945 RID: 14661 RVA: 0x001071A0 File Offset: 0x001053A0
		private static bool game_menu_town_enter_the_arena_on_condition(MenuCallbackArgs args)
		{
			bool flag2;
			TextObject textObject;
			bool flag = Campaign.Current.Models.SettlementAccessModel.CanMainHeroDoSettlementAction(Settlement.CurrentSettlement, SettlementAccessModel.SettlementAction.WalkAroundTheArena, out flag2, out textObject);
			List<Location> list = Settlement.CurrentSettlement.LocationComplex.FindAll((string x) => x == "arena").ToList<Location>();
			MenuHelper.SetIssueAndQuestDataForLocations(args, list);
			args.optionLeaveType = GameMenuOption.LeaveType.Mission;
			return MenuHelper.SetOptionProperties(args, flag, flag2, textObject);
		}

		// Token: 0x06003946 RID: 14662 RVA: 0x00107218 File Offset: 0x00105418
		private static bool game_menu_craft_item_on_condition(MenuCallbackArgs args)
		{
			bool flag2;
			TextObject textObject;
			bool flag = Campaign.Current.Models.SettlementAccessModel.CanMainHeroDoSettlementAction(Settlement.CurrentSettlement, SettlementAccessModel.SettlementAction.Craft, out flag2, out textObject);
			args.optionLeaveType = GameMenuOption.LeaveType.Craft;
			return MenuHelper.SetOptionProperties(args, flag, flag2, textObject);
		}

		// Token: 0x06003947 RID: 14663 RVA: 0x00107254 File Offset: 0x00105454
		public static void wait_menu_prisoner_wait_on_init(MenuCallbackArgs args)
		{
			TextObject text = args.MenuContext.GameMenu.GetText();
			int captiveTimeInDays = PlayerCaptivity.CaptiveTimeInDays;
			TextObject textObject;
			if (captiveTimeInDays == 0)
			{
				textObject = GameTexts.FindText("str_prisoner_of_party_menu_text", null);
			}
			else
			{
				textObject = GameTexts.FindText("str_prisoner_of_party_for_days_menu_text", null);
				textObject.SetTextVariable("NUMBER_OF_DAYS", captiveTimeInDays);
				textObject.SetTextVariable("PLURAL", (captiveTimeInDays > 1) ? 1 : 0);
			}
			textObject.SetTextVariable("PARTY_NAME", (Hero.MainHero.PartyBelongedToAsPrisoner != null) ? Hero.MainHero.PartyBelongedToAsPrisoner.Name : TextObject.Empty);
			text.SetTextVariable("CAPTIVITY_TEXT", textObject);
		}

		// Token: 0x06003948 RID: 14664 RVA: 0x001072EF File Offset: 0x001054EF
		[GameMenuInitializationHandler("settlement_wait")]
		public static void wait_menu_prisoner_settlement_wait_ui_on_init(MenuCallbackArgs args)
		{
			if (Hero.MainHero.IsFemale)
			{
				args.MenuContext.SetBackgroundMeshName("wait_prisoner_female");
				return;
			}
			args.MenuContext.SetBackgroundMeshName("wait_prisoner_male");
		}

		// Token: 0x06003949 RID: 14665 RVA: 0x0010731E File Offset: 0x0010551E
		public static bool wait_menu_prisoner_wait_on_condition(MenuCallbackArgs args)
		{
			return true;
		}

		// Token: 0x0600394A RID: 14666 RVA: 0x00107324 File Offset: 0x00105524
		public static void wait_menu_prisoner_wait_on_tick(MenuCallbackArgs args, CampaignTime dt)
		{
			int captiveTimeInDays = PlayerCaptivity.CaptiveTimeInDays;
			if (captiveTimeInDays == 0)
			{
				return;
			}
			TextObject text = args.MenuContext.GameMenu.GetText();
			TextObject textObject = GameTexts.FindText("str_prisoner_of_party_for_days_menu_text", null);
			textObject.SetTextVariable("NUMBER_OF_DAYS", captiveTimeInDays);
			textObject.SetTextVariable("PLURAL", (captiveTimeInDays > 1) ? 1 : 0);
			textObject.SetTextVariable("PARTY_NAME", PlayerCaptivity.CaptorParty.Name);
			text.SetTextVariable("CAPTIVITY_TEXT", textObject);
		}

		// Token: 0x0600394B RID: 14667 RVA: 0x0010739C File Offset: 0x0010559C
		public static void wait_menu_settlement_wait_on_tick(MenuCallbackArgs args, CampaignTime dt)
		{
			int captiveTimeInDays = PlayerCaptivity.CaptiveTimeInDays;
			if (captiveTimeInDays == 0)
			{
				return;
			}
			TextObject textObject = (Hero.MainHero.IsPrisoner ? Hero.MainHero.PartyBelongedToAsPrisoner.Settlement.Name : Settlement.CurrentSettlement.Name);
			TextObject text = args.MenuContext.GameMenu.GetText();
			TextObject textObject2 = GameTexts.FindText("str_prisoner_of_settlement_for_days_menu_text", null);
			textObject2.SetTextVariable("NUMBER_OF_DAYS", captiveTimeInDays);
			textObject2.SetTextVariable("PLURAL", (captiveTimeInDays > 1) ? 1 : 0);
			textObject2.SetTextVariable("SETTLEMENT_NAME", textObject);
			text.SetTextVariable("CAPTIVITY_TEXT", textObject2);
		}

		// Token: 0x0600394C RID: 14668 RVA: 0x00107438 File Offset: 0x00105638
		private static bool SellPrisonersCondition(MenuCallbackArgs args)
		{
			if (PartyBase.MainParty.PrisonRoster.Count > 0)
			{
				int ransomValueOfAllTransferablePrisoners = PlayerTownVisitCampaignBehavior.GetRansomValueOfAllTransferablePrisoners();
				if (ransomValueOfAllTransferablePrisoners > 0)
				{
					MBTextManager.SetTextVariable("RANSOM_AMOUNT", ransomValueOfAllTransferablePrisoners);
					args.optionLeaveType = GameMenuOption.LeaveType.Ransom;
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600394D RID: 14669 RVA: 0x00107477 File Offset: 0x00105677
		private static bool SellPrisonerOneStackCondition(MenuCallbackArgs args)
		{
			if (PartyBase.MainParty.PrisonRoster.Count > 0)
			{
				args.optionLeaveType = GameMenuOption.LeaveType.Ransom;
				return true;
			}
			return false;
		}

		// Token: 0x0600394E RID: 14670 RVA: 0x00107498 File Offset: 0x00105698
		private static int GetRansomValueOfAllTransferablePrisoners()
		{
			int num = 0;
			List<string> list = Campaign.Current.GetCampaignBehavior<IViewDataTracker>().GetPartyPrisonerLocks().ToList<string>();
			foreach (TroopRosterElement troopRosterElement in PartyBase.MainParty.PrisonRoster.GetTroopRoster())
			{
				if (!list.Contains(troopRosterElement.Character.StringId))
				{
					num += Campaign.Current.Models.RansomValueCalculationModel.PrisonerRansomValue(troopRosterElement.Character, Hero.MainHero) * troopRosterElement.Number;
				}
			}
			return num;
		}

		// Token: 0x0600394F RID: 14671 RVA: 0x00107544 File Offset: 0x00105744
		private static void ChooseRansomPrisoners()
		{
			GameMenu.SwitchToMenu("town_backstreet");
			PartyScreenManager.OpenScreenAsRansom();
		}

		// Token: 0x06003950 RID: 14672 RVA: 0x00107555 File Offset: 0x00105755
		private static void SellAllTransferablePrisoners()
		{
			SellPrisonersAction.ApplyForAllPrisoners(MobileParty.MainParty, MobileParty.MainParty.PrisonRoster, Settlement.CurrentSettlement, true);
			GameMenu.SwitchToMenu("town_backstreet");
		}

		// Token: 0x06003951 RID: 14673 RVA: 0x0010757C File Offset: 0x0010577C
		private static bool game_menu_castle_go_to_lords_hall_on_condition(MenuCallbackArgs args)
		{
			if (FactionManager.IsAtWarAgainstFaction(Settlement.CurrentSettlement.MapFaction, Hero.MainHero.MapFaction))
			{
				args.Tooltip = new TextObject("{=h9i9VXLd}You cannot enter an enemy lord's hall.", null);
				args.IsEnabled = false;
			}
			List<Location> list = Settlement.CurrentSettlement.LocationComplex.FindAll((string x) => x == "lordshall").ToList<Location>();
			MenuHelper.SetIssueAndQuestDataForLocations(args, list);
			args.optionLeaveType = GameMenuOption.LeaveType.Mission;
			return true;
		}

		// Token: 0x06003952 RID: 14674 RVA: 0x00107600 File Offset: 0x00105800
		private static bool game_menu_town_keep_bribe_pay_bribe_on_condition(MenuCallbackArgs args)
		{
			int bribeToEnterLordsHall = Campaign.Current.Models.BribeCalculationModel.GetBribeToEnterLordsHall(Settlement.CurrentSettlement);
			MBTextManager.SetTextVariable("AMOUNT", bribeToEnterLordsHall);
			List<Location> list = Settlement.CurrentSettlement.LocationComplex.FindAll((string x) => x == "lordshall").ToList<Location>();
			MenuHelper.SetIssueAndQuestDataForLocations(args, list);
			args.optionLeaveType = GameMenuOption.LeaveType.Mission;
			if (Hero.MainHero.Gold < bribeToEnterLordsHall)
			{
				args.Tooltip = new TextObject("{=d0kbtGYn}You don't have enough gold.", null);
				args.IsEnabled = false;
			}
			return bribeToEnterLordsHall > 0;
		}

		// Token: 0x06003953 RID: 14675 RVA: 0x0010769D File Offset: 0x0010589D
		private static bool game_menu_castle_go_to_lords_hall_cheat_on_condition(MenuCallbackArgs args)
		{
			return Game.Current.IsDevelopmentMode;
		}

		// Token: 0x06003954 RID: 14676 RVA: 0x001076A9 File Offset: 0x001058A9
		private static void game_menu_castle_take_a_walk_around_the_castle_on_consequence(MenuCallbackArgs args)
		{
			LocationEncounter locationEncounter = PlayerEncounter.LocationEncounter;
			PlayerEncounter.LocationEncounter.CreateAndOpenMissionController(LocationComplex.Current.GetLocationWithId("center"), null, null, null);
		}

		// Token: 0x06003955 RID: 14677 RVA: 0x001076D0 File Offset: 0x001058D0
		private static bool CheckAndOpenNextLocation(MenuCallbackArgs args)
		{
			if (Campaign.Current.GameMenuManager.NextLocation != null && GameStateManager.Current.ActiveState is MapState)
			{
				PlayerEncounter.LocationEncounter.CreateAndOpenMissionController(Campaign.Current.GameMenuManager.NextLocation, Campaign.Current.GameMenuManager.PreviousLocation, null, null);
				string stringId = Campaign.Current.GameMenuManager.NextLocation.StringId;
				if (!(stringId == "center"))
				{
					if (!(stringId == "tavern"))
					{
						if (!(stringId == "arena"))
						{
							if (stringId == "lordshall" || stringId == "prison")
							{
								Campaign.Current.GameMenuManager.SetNextMenu("town_keep");
							}
						}
						else
						{
							Campaign.Current.GameMenuManager.SetNextMenu("town_arena");
						}
					}
					else
					{
						Campaign.Current.GameMenuManager.SetNextMenu("town_backstreet");
					}
				}
				else if (Settlement.CurrentSettlement.IsCastle)
				{
					Campaign.Current.GameMenuManager.SetNextMenu("castle");
				}
				else if (Settlement.CurrentSettlement.IsTown)
				{
					Campaign.Current.GameMenuManager.SetNextMenu("town");
				}
				else if (Settlement.CurrentSettlement.IsVillage)
				{
					Campaign.Current.GameMenuManager.SetNextMenu("village");
				}
				else
				{
					Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\CampaignBehaviors\\PlayerTownVisitCampaignBehavior.cs", "CheckAndOpenNextLocation", 768);
				}
				Campaign.Current.GameMenuManager.NextLocation = null;
				Campaign.Current.GameMenuManager.PreviousLocation = null;
				return true;
			}
			return false;
		}

		// Token: 0x06003956 RID: 14678 RVA: 0x00107880 File Offset: 0x00105A80
		private static void game_menu_town_on_init(MenuCallbackArgs args)
		{
			PlayerTownVisitCampaignBehavior.SetIntroductionText(Settlement.CurrentSettlement, false);
			PlayerTownVisitCampaignBehavior.UpdateMenuLocations(args.MenuContext.GameMenu.StringId);
			Campaign.Current.autoEnterTown = null;
			Settlement currentSettlement = Settlement.CurrentSettlement;
			if (PlayerTownVisitCampaignBehavior.CheckAndOpenNextLocation(args))
			{
				return;
			}
			MobileParty garrisonParty = Settlement.CurrentSettlement.Town.GarrisonParty;
			if (garrisonParty != null && garrisonParty.MemberRoster.Count <= 0)
			{
				MobileParty garrisonParty2 = Settlement.CurrentSettlement.Town.GarrisonParty;
				if (garrisonParty2 != null && garrisonParty2.PrisonRoster.Count <= 0)
				{
					DestroyPartyAction.Apply(null, Settlement.CurrentSettlement.Town.GarrisonParty);
				}
			}
			args.MenuTitle = new TextObject("{=mVKcvY2U}Town Center", null);
		}

		// Token: 0x06003957 RID: 14679 RVA: 0x00107940 File Offset: 0x00105B40
		private static void UpdateMenuLocations(string menuID)
		{
			Campaign.Current.GameMenuManager.MenuLocations.Clear();
			Settlement settlement = ((Settlement.CurrentSettlement == null) ? MobileParty.MainParty.CurrentSettlement : Settlement.CurrentSettlement);
			uint num = <PrivateImplementationDetails>.ComputeStringHash(menuID);
			if (num <= 1579208614U)
			{
				if (num <= 1192893027U)
				{
					if (num != 864577349U)
					{
						if (num != 1192893027U)
						{
							goto IL_3B2;
						}
						if (!(menuID == "village"))
						{
							goto IL_3B2;
						}
						Campaign.Current.GameMenuManager.MenuLocations.AddRange(settlement.LocationComplex.GetListOfLocations());
						return;
					}
					else
					{
						if (!(menuID == "town"))
						{
							goto IL_3B2;
						}
						Campaign.Current.GameMenuManager.MenuLocations.Add(settlement.LocationComplex.GetLocationWithId("center"));
						Campaign.Current.GameMenuManager.MenuLocations.Add(settlement.LocationComplex.GetLocationWithId("arena"));
						Campaign.Current.GameMenuManager.MenuLocations.Add(settlement.LocationComplex.GetLocationWithId("house_1"));
						Campaign.Current.GameMenuManager.MenuLocations.Add(settlement.LocationComplex.GetLocationWithId("house_2"));
						Campaign.Current.GameMenuManager.MenuLocations.Add(settlement.LocationComplex.GetLocationWithId("house_3"));
						return;
					}
				}
				else if (num != 1470125011U)
				{
					if (num != 1556184416U)
					{
						if (num != 1579208614U)
						{
							goto IL_3B2;
						}
						if (!(menuID == "town_backstreet"))
						{
							goto IL_3B2;
						}
						Campaign.Current.GameMenuManager.MenuLocations.Add(settlement.LocationComplex.GetLocationWithId("tavern"));
						return;
					}
					else if (!(menuID == "castle_dungeon"))
					{
						goto IL_3B2;
					}
				}
				else
				{
					if (!(menuID == "town_keep"))
					{
						goto IL_3B2;
					}
					Campaign.Current.GameMenuManager.MenuLocations.Add(settlement.LocationComplex.GetLocationWithId("lordshall"));
					return;
				}
			}
			else if (num <= 2781132822U)
			{
				if (num != 1924483413U)
				{
					if (num != 2596447321U)
					{
						if (num != 2781132822U)
						{
							goto IL_3B2;
						}
						if (!(menuID == "town_keep_dungeon"))
						{
							goto IL_3B2;
						}
					}
					else
					{
						if (!(menuID == "castle"))
						{
							goto IL_3B2;
						}
						Campaign.Current.GameMenuManager.MenuLocations.Add(settlement.LocationComplex.GetLocationWithId("center"));
						Campaign.Current.GameMenuManager.MenuLocations.Add(settlement.LocationComplex.GetLocationWithId("lordshall"));
						return;
					}
				}
				else if (!(menuID == "town_enemy_town_keep"))
				{
					goto IL_3B2;
				}
			}
			else if (num != 4029725827U)
			{
				if (num != 4147432362U)
				{
					if (num != 4246693001U)
					{
						goto IL_3B2;
					}
					if (!(menuID == "town_arena"))
					{
						goto IL_3B2;
					}
					Campaign.Current.GameMenuManager.MenuLocations.Add(settlement.LocationComplex.GetLocationWithId("arena"));
					return;
				}
				else
				{
					if (!(menuID == "town_keep_bribe"))
					{
						goto IL_3B2;
					}
					return;
				}
			}
			else
			{
				if (!(menuID == "town_center"))
				{
					goto IL_3B2;
				}
				Campaign.Current.GameMenuManager.MenuLocations.Add(settlement.LocationComplex.GetLocationWithId("center"));
				Campaign.Current.GameMenuManager.MenuLocations.Add(settlement.LocationComplex.GetLocationWithId("arena"));
				return;
			}
			Campaign.Current.GameMenuManager.MenuLocations.Add(settlement.LocationComplex.GetLocationWithId("prison"));
			return;
			IL_3B2:
			Debug.FailedAssert("Could not get the associated locations for Game Menu: " + menuID, "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\CampaignBehaviors\\PlayerTownVisitCampaignBehavior.cs", "UpdateMenuLocations", 874);
			Campaign.Current.GameMenuManager.MenuLocations.AddRange(settlement.LocationComplex.GetListOfLocations());
		}

		// Token: 0x06003958 RID: 14680 RVA: 0x00107D3D File Offset: 0x00105F3D
		private static void town_keep_on_init(MenuCallbackArgs args)
		{
			PlayerTownVisitCampaignBehavior.UpdateMenuLocations(args.MenuContext.GameMenu.StringId);
			if (PlayerTownVisitCampaignBehavior.CheckAndOpenNextLocation(args))
			{
				return;
			}
			PlayerTownVisitCampaignBehavior.SetIntroductionText(Settlement.CurrentSettlement, true);
			args.MenuTitle = new TextObject("{=723ig40Q}Keep", null);
		}

		// Token: 0x06003959 RID: 14681 RVA: 0x00107D7C File Offset: 0x00105F7C
		private static void town_enemy_keep_on_init(MenuCallbackArgs args)
		{
			PlayerTownVisitCampaignBehavior.UpdateMenuLocations(args.MenuContext.GameMenu.StringId);
			if (PlayerTownVisitCampaignBehavior.CheckAndOpenNextLocation(args))
			{
				return;
			}
			PlayerTownVisitCampaignBehavior.SetIntroductionText(Settlement.CurrentSettlement, true);
			TextObject text = args.MenuContext.GameMenu.GetText();
			MobileParty garrisonParty = Settlement.CurrentSettlement.Town.GarrisonParty;
			bool flag = (garrisonParty != null && garrisonParty.PrisonRoster.TotalHeroes > 0) || Settlement.CurrentSettlement.Party.PrisonRoster.TotalHeroes > 0;
			text.SetTextVariable("SCOUT_KEEP_TEXT", flag ? "{=Tfb9LNAr}You have observed the comings and goings of the guards at the keep. You think you've identified a guard who might be approached and offered a bribe." : "{=qGUrhBpI}After spending time observing the keep and eavesdropping on the guards, you conclude that there are no prisoners here who you can liberate.");
			args.MenuTitle = new TextObject("{=723ig40Q}Keep", null);
		}

		// Token: 0x0600395A RID: 14682 RVA: 0x00107E30 File Offset: 0x00106030
		private static void town_keep_dungeon_on_init(MenuCallbackArgs args)
		{
			PlayerTownVisitCampaignBehavior.UpdateMenuLocations(args.MenuContext.GameMenu.StringId);
			if (PlayerTownVisitCampaignBehavior.CheckAndOpenNextLocation(args))
			{
				return;
			}
			args.MenuTitle = new TextObject("{=x04UGQDn}Dungeon", null);
			TextObject textObject = TextObject.Empty;
			if (Settlement.CurrentSettlement.SettlementComponent.GetPrisonerHeroes().Count == 0)
			{
				textObject = new TextObject("{=O4flV28Q}There are no prisoners here.", null);
			}
			else
			{
				int count = Settlement.CurrentSettlement.SettlementComponent.GetPrisonerHeroes().Count;
				textObject = new TextObject("{=gAc8SWDt}There {?(PRISONER_COUNT > 1)}are {PRISONER_COUNT} prisoners{?}is 1 prisoner{\\?} here.", null);
				textObject.SetTextVariable("PRISONER_COUNT", count);
			}
			MBTextManager.SetTextVariable("PRISONER_INTRODUCTION", textObject, false);
		}

		// Token: 0x0600395B RID: 14683 RVA: 0x00107ED0 File Offset: 0x001060D0
		private static void town_keep_bribe_on_init(MenuCallbackArgs args)
		{
			args.MenuTitle = new TextObject("{=723ig40Q}Keep", null);
			PlayerTownVisitCampaignBehavior.UpdateMenuLocations(args.MenuContext.GameMenu.StringId);
			if (Campaign.Current.Models.BribeCalculationModel.GetBribeToEnterLordsHall(Settlement.CurrentSettlement) == 0)
			{
				GameMenu.ActivateGameMenu("town_keep");
			}
		}

		// Token: 0x0600395C RID: 14684 RVA: 0x00107F28 File Offset: 0x00106128
		private static void town_backstreet_on_init(MenuCallbackArgs args)
		{
			PlayerTownVisitCampaignBehavior.UpdateMenuLocations(args.MenuContext.GameMenu.StringId);
			string text = Settlement.CurrentSettlement.Culture.StringId + "_tavern";
			args.MenuContext.SetBackgroundMeshName(text);
			if (PlayerTownVisitCampaignBehavior.CheckAndOpenNextLocation(args))
			{
				return;
			}
			args.MenuTitle = new TextObject("{=a0MVffcN}Backstreet", null);
		}

		// Token: 0x0600395D RID: 14685 RVA: 0x00107F8C File Offset: 0x0010618C
		private static void town_arena_on_init(MenuCallbackArgs args)
		{
			PlayerTownVisitCampaignBehavior.UpdateMenuLocations(args.MenuContext.GameMenu.StringId);
			if (Settlement.CurrentSettlement != null && Settlement.CurrentSettlement.IsTown && Campaign.Current.TournamentManager.GetTournamentGame(Settlement.CurrentSettlement.Town) != null && Campaign.Current.IsDay)
			{
				TextObject textObject = GameTexts.FindText("str_town_new_tournament_text", null);
				MBTextManager.SetTextVariable("ADDITIONAL_STRING", textObject, false);
			}
			else
			{
				TextObject textObject2 = GameTexts.FindText("str_town_empty_arena_text", null);
				MBTextManager.SetTextVariable("ADDITIONAL_STRING", textObject2, false);
			}
			if (PlayerTownVisitCampaignBehavior.CheckAndOpenNextLocation(args))
			{
				return;
			}
			args.MenuTitle = new TextObject("{=mMU3H6HZ}Arena", null);
		}

		// Token: 0x0600395E RID: 14686 RVA: 0x00108034 File Offset: 0x00106234
		public static bool game_menu_town_manage_town_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Manage;
			Settlement currentSettlement = Settlement.CurrentSettlement;
			bool flag;
			TextObject textObject;
			return MenuHelper.SetOptionProperties(args, Campaign.Current.Models.SettlementAccessModel.CanMainHeroDoSettlementAction(currentSettlement, SettlementAccessModel.SettlementAction.ManageTown, out flag, out textObject), flag, textObject);
		}

		// Token: 0x0600395F RID: 14687 RVA: 0x00108071 File Offset: 0x00106271
		public static bool game_menu_town_manage_town_cheat_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Manage;
			return GameManagerBase.Current.IsDevelopmentMode && Settlement.CurrentSettlement.IsTown && Settlement.CurrentSettlement.OwnerClan.Leader != Hero.MainHero;
		}

		// Token: 0x06003960 RID: 14688 RVA: 0x001080B0 File Offset: 0x001062B0
		private static bool game_menu_town_keep_open_stash_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.OpenStash;
			return Settlement.CurrentSettlement.OwnerClan == Clan.PlayerClan && !Settlement.CurrentSettlement.Town.IsOwnerUnassigned;
		}

		// Token: 0x06003961 RID: 14689 RVA: 0x001080DF File Offset: 0x001062DF
		private static void game_menu_town_keep_open_stash_on_consequence(MenuCallbackArgs args)
		{
			InventoryManager.OpenScreenAsStash(Settlement.CurrentSettlement.Stash);
		}

		// Token: 0x06003962 RID: 14690 RVA: 0x001080F0 File Offset: 0x001062F0
		private static bool game_menu_manage_garrison_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.ManageGarrison;
			Settlement currentSettlement = Settlement.CurrentSettlement;
			return currentSettlement.OwnerClan == Clan.PlayerClan && currentSettlement.MapFaction == Hero.MainHero.MapFaction && currentSettlement.IsFortification;
		}

		// Token: 0x06003963 RID: 14691 RVA: 0x00108132 File Offset: 0x00106332
		private static bool game_menu_manage_castle_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Manage;
			return Settlement.CurrentSettlement.OwnerClan == Clan.PlayerClan && Settlement.CurrentSettlement.IsCastle;
		}

		// Token: 0x06003964 RID: 14692 RVA: 0x0010815C File Offset: 0x0010635C
		private static void game_menu_manage_garrison_on_consequence(MenuCallbackArgs args)
		{
			Settlement currentSettlement = Hero.MainHero.CurrentSettlement;
			if (currentSettlement.Town.GarrisonParty == null)
			{
				currentSettlement.AddGarrisonParty(false);
			}
			PartyScreenManager.OpenScreenAsManageTroops(currentSettlement.Town.GarrisonParty);
		}

		// Token: 0x06003965 RID: 14693 RVA: 0x00108198 File Offset: 0x00106398
		private static bool game_menu_leave_troops_garrison_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.DonateTroops;
			Settlement currentSettlement = Settlement.CurrentSettlement;
			return currentSettlement.OwnerClan != Clan.PlayerClan && currentSettlement.MapFaction == Hero.MainHero.MapFaction && currentSettlement.IsFortification && (currentSettlement.Town.GarrisonParty == null || currentSettlement.Town.GarrisonParty.Party.PartySizeLimit > currentSettlement.Town.GarrisonParty.Party.NumberOfAllMembers);
		}

		// Token: 0x06003966 RID: 14694 RVA: 0x00108217 File Offset: 0x00106417
		private static void game_menu_leave_troops_garrison_on_consequece(MenuCallbackArgs args)
		{
			PartyScreenManager.OpenScreenAsDonateGarrisonWithCurrentSettlement();
		}

		// Token: 0x06003967 RID: 14695 RVA: 0x00108220 File Offset: 0x00106420
		private static bool game_menu_town_town_streets_on_condition(MenuCallbackArgs args)
		{
			bool flag2;
			TextObject textObject;
			bool flag = Campaign.Current.Models.SettlementAccessModel.CanMainHeroAccessLocation(Settlement.CurrentSettlement, "center", out flag2, out textObject);
			List<Location> list = Settlement.CurrentSettlement.LocationComplex.FindAll((string x) => x == "center").ToList<Location>();
			MenuHelper.SetIssueAndQuestDataForLocations(args, list);
			args.optionLeaveType = GameMenuOption.LeaveType.Mission;
			return MenuHelper.SetOptionProperties(args, flag, flag2, textObject);
		}

		// Token: 0x06003968 RID: 14696 RVA: 0x0010829B File Offset: 0x0010649B
		private static void game_menu_town_town_streets_on_consequence(MenuCallbackArgs args)
		{
			LocationEncounter locationEncounter = PlayerEncounter.LocationEncounter;
			PlayerEncounter.LocationEncounter.CreateAndOpenMissionController(LocationComplex.Current.GetLocationWithId("center"), null, null, null);
		}

		// Token: 0x06003969 RID: 14697 RVA: 0x001082C0 File Offset: 0x001064C0
		private static void game_menu_town_lordshall_on_consequence(MenuCallbackArgs args)
		{
			LocationEncounter locationEncounter = PlayerEncounter.LocationEncounter;
			PlayerTownVisitCampaignBehavior.OpenMissionWithSettingPreviousLocation("center", "lordshall");
		}

		// Token: 0x0600396A RID: 14698 RVA: 0x001082D7 File Offset: 0x001064D7
		private static void game_menu_castle_lordshall_on_consequence(MenuCallbackArgs args)
		{
			PlayerTownVisitCampaignBehavior.OpenMissionWithSettingPreviousLocation("center", "lordshall");
		}

		// Token: 0x0600396B RID: 14699 RVA: 0x001082E8 File Offset: 0x001064E8
		private static void game_menu_town_keep_bribe_pay_bribe_on_consequence(MenuCallbackArgs args)
		{
			int bribeToEnterLordsHall = Campaign.Current.Models.BribeCalculationModel.GetBribeToEnterLordsHall(Settlement.CurrentSettlement);
			BribeGuardsAction.Apply(Settlement.CurrentSettlement, bribeToEnterLordsHall);
			LocationEncounter locationEncounter = PlayerEncounter.LocationEncounter;
			GameMenu.ActivateGameMenu("town_keep");
		}

		// Token: 0x0600396C RID: 14700 RVA: 0x0010832A File Offset: 0x0010652A
		private static void game_menu_lordshall_cheat_on_consequence(MenuCallbackArgs args)
		{
			PlayerTownVisitCampaignBehavior.OpenMissionWithSettingPreviousLocation("center", "lordshall");
		}

		// Token: 0x0600396D RID: 14701 RVA: 0x0010833B File Offset: 0x0010653B
		private static void game_menu_dungeon_cheat_on_consequence(MenuCallbackArgs ARGS)
		{
			GameMenu.SwitchToMenu("castle_dungeon");
		}

		// Token: 0x0600396E RID: 14702 RVA: 0x00108347 File Offset: 0x00106547
		private static void game_menu_town_dungeon_on_consequence(MenuCallbackArgs args)
		{
			LocationEncounter locationEncounter = PlayerEncounter.LocationEncounter;
			PlayerTownVisitCampaignBehavior.OpenMissionWithSettingPreviousLocation("center", "prison");
		}

		// Token: 0x0600396F RID: 14703 RVA: 0x0010835E File Offset: 0x0010655E
		private static void game_menu_castle_dungeon_on_consequence(MenuCallbackArgs args)
		{
			PlayerTownVisitCampaignBehavior.OpenMissionWithSettingPreviousLocation("center", "prison");
		}

		// Token: 0x06003970 RID: 14704 RVA: 0x0010836F File Offset: 0x0010656F
		private static void game_menu_keep_dungeon_on_consequence(MenuCallbackArgs args)
		{
			GameMenu.SwitchToMenu("castle_dungeon");
		}

		// Token: 0x06003971 RID: 14705 RVA: 0x0010837B File Offset: 0x0010657B
		private static void game_menu_town_town_tavern_on_consequence(MenuCallbackArgs args)
		{
			LocationEncounter locationEncounter = PlayerEncounter.LocationEncounter;
			PlayerTownVisitCampaignBehavior.OpenMissionWithSettingPreviousLocation("center", "tavern");
		}

		// Token: 0x06003972 RID: 14706 RVA: 0x00108392 File Offset: 0x00106592
		private static void game_menu_town_town_arena_on_consequence(MenuCallbackArgs args)
		{
			LocationEncounter locationEncounter = PlayerEncounter.LocationEncounter;
			PlayerTownVisitCampaignBehavior.OpenMissionWithSettingPreviousLocation("center", "arena");
		}

		// Token: 0x06003973 RID: 14707 RVA: 0x001083A9 File Offset: 0x001065A9
		private static void game_menu_town_town_market_on_consequence(MenuCallbackArgs args)
		{
			LocationEncounter locationEncounter = PlayerEncounter.LocationEncounter;
			InventoryManager.OpenScreenAsTrade(Settlement.CurrentSettlement.ItemRoster, Settlement.CurrentSettlement.Town, InventoryManager.InventoryCategoryType.None, null);
		}

		// Token: 0x06003974 RID: 14708 RVA: 0x001083CC File Offset: 0x001065CC
		private static bool game_menu_town_town_leave_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Leave;
			return MobileParty.MainParty.Army == null || MobileParty.MainParty.Army.LeaderParty == MobileParty.MainParty;
		}

		// Token: 0x06003975 RID: 14709 RVA: 0x001083FA File Offset: 0x001065FA
		private static void game_menu_settlement_leave_on_consequence(MenuCallbackArgs args)
		{
			PlayerEncounter.LeaveSettlement();
			PlayerEncounter.Finish(true);
			Campaign.Current.SaveHandler.SignalAutoSave();
		}

		// Token: 0x06003976 RID: 14710 RVA: 0x00108418 File Offset: 0x00106618
		private static void settlement_wait_on_init(MenuCallbackArgs args)
		{
			TextObject text = args.MenuContext.GameMenu.GetText();
			TextObject textObject = (Hero.MainHero.IsPrisoner ? Hero.MainHero.PartyBelongedToAsPrisoner.Settlement.Name : Settlement.CurrentSettlement.Name);
			int captiveTimeInDays = PlayerCaptivity.CaptiveTimeInDays;
			TextObject textObject2;
			if (captiveTimeInDays == 0)
			{
				textObject2 = GameTexts.FindText("str_prisoner_of_settlement_menu_text", null);
			}
			else
			{
				textObject2 = GameTexts.FindText("str_prisoner_of_settlement_for_days_menu_text", null);
				textObject2.SetTextVariable("NUMBER_OF_DAYS", captiveTimeInDays);
				textObject2.SetTextVariable("PLURAL", (captiveTimeInDays > 1) ? 1 : 0);
			}
			textObject2.SetTextVariable("SETTLEMENT_NAME", textObject);
			text.SetTextVariable("CAPTIVITY_TEXT", textObject2);
		}

		// Token: 0x06003977 RID: 14711 RVA: 0x001084C0 File Offset: 0x001066C0
		private static void game_menu_village_on_init(MenuCallbackArgs args)
		{
			PlayerTownVisitCampaignBehavior.SetIntroductionText(Settlement.CurrentSettlement, false);
			PlayerTownVisitCampaignBehavior.UpdateMenuLocations(args.MenuContext.GameMenu.StringId);
			SettlementAccessModel settlementAccessModel = Campaign.Current.Models.SettlementAccessModel;
			Settlement currentSettlement = Settlement.CurrentSettlement;
			SettlementAccessModel.AccessDetails accessDetails;
			settlementAccessModel.CanMainHeroEnterSettlement(currentSettlement, out accessDetails);
			if (currentSettlement != null)
			{
				Village village = currentSettlement.Village;
				if (accessDetails.AccessLevel == SettlementAccessModel.AccessLevel.NoAccess && accessDetails.AccessLimitationReason == SettlementAccessModel.AccessLimitationReason.VillageIsLooted)
				{
					GameMenu.SwitchToMenu("village_looted");
				}
			}
			args.MenuTitle = new TextObject("{=Ua6CNLBZ}Village", null);
		}

		// Token: 0x06003978 RID: 14712 RVA: 0x00108540 File Offset: 0x00106740
		private static void game_menu_castle_on_init(MenuCallbackArgs args)
		{
			MobileParty garrisonParty = Settlement.CurrentSettlement.Town.GarrisonParty;
			if (garrisonParty != null && garrisonParty.MemberRoster.Count <= 0)
			{
				DestroyPartyAction.Apply(null, Settlement.CurrentSettlement.Town.GarrisonParty);
			}
			PlayerTownVisitCampaignBehavior.SetIntroductionText(Settlement.CurrentSettlement, true);
			PlayerTownVisitCampaignBehavior.UpdateMenuLocations(args.MenuContext.GameMenu.StringId);
			if (Campaign.Current.GameMenuManager.NextLocation != null)
			{
				PlayerEncounter.LocationEncounter.CreateAndOpenMissionController(Campaign.Current.GameMenuManager.NextLocation, Campaign.Current.GameMenuManager.PreviousLocation, null, null);
				Campaign.Current.GameMenuManager.NextLocation = null;
				Campaign.Current.GameMenuManager.PreviousLocation = null;
			}
			args.MenuTitle = new TextObject("{=sVXa3zFx}Castle", null);
		}

		// Token: 0x06003979 RID: 14713 RVA: 0x00108618 File Offset: 0x00106818
		private static bool game_menu_village_village_center_on_condition(MenuCallbackArgs args)
		{
			List<Location> list = Settlement.CurrentSettlement.LocationComplex.GetListOfLocations().ToList<Location>();
			MenuHelper.SetIssueAndQuestDataForLocations(args, list);
			args.optionLeaveType = GameMenuOption.LeaveType.Mission;
			return Settlement.CurrentSettlement.Village.VillageState == Village.VillageStates.Normal;
		}

		// Token: 0x0600397A RID: 14714 RVA: 0x0010865A File Offset: 0x0010685A
		private static void game_menu_village_village_center_on_consequence(MenuCallbackArgs args)
		{
			(PlayerEncounter.LocationEncounter as VillageEncounter).CreateAndOpenMissionController(LocationComplex.Current.GetLocationWithId("village_center"), null, null, null);
		}

		// Token: 0x0600397B RID: 14715 RVA: 0x00108680 File Offset: 0x00106880
		private static bool game_menu_village_buy_good_on_condition(MenuCallbackArgs args)
		{
			Village village = Settlement.CurrentSettlement.Village;
			if (village.VillageState == Village.VillageStates.BeingRaided)
			{
				return false;
			}
			args.optionLeaveType = GameMenuOption.LeaveType.Trade;
			if (village.VillageState == Village.VillageStates.Normal && village.Owner.ItemRoster.Count > 0)
			{
				foreach (ValueTuple<ItemObject, float> valueTuple in village.VillageType.Productions)
				{
				}
				return true;
			}
			if (village.Gold > 0)
			{
				args.Tooltip = new TextObject("{=FbowXAC0}There are no available products right now.", null);
				return true;
			}
			args.IsEnabled = false;
			args.Tooltip = new TextObject("{=bmfo7CaO}Village shop is not available right now.", null);
			return true;
		}

		// Token: 0x0600397C RID: 14716 RVA: 0x00108744 File Offset: 0x00106944
		private static void game_menu_recruit_volunteers_on_consequence(MenuCallbackArgs args)
		{
		}

		// Token: 0x0600397D RID: 14717 RVA: 0x00108746 File Offset: 0x00106946
		private static bool game_menu_recruit_volunteers_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Recruit;
			return !Settlement.CurrentSettlement.IsVillage || Settlement.CurrentSettlement.Village.VillageState == Village.VillageStates.Normal;
		}

		// Token: 0x0600397E RID: 14718 RVA: 0x00108770 File Offset: 0x00106970
		private static bool game_menu_village_wait_on_condition(MenuCallbackArgs args)
		{
			Village village = Settlement.CurrentSettlement.Village;
			args.optionLeaveType = GameMenuOption.LeaveType.Wait;
			return village.VillageState == Village.VillageStates.Normal;
		}

		// Token: 0x0600397F RID: 14719 RVA: 0x0010878C File Offset: 0x0010698C
		private static bool game_menu_town_wait_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Wait;
			MBTextManager.SetTextVariable("CURRENT_SETTLEMENT", Settlement.CurrentSettlement.EncyclopediaLinkWithName, false);
			return true;
		}

		// Token: 0x06003980 RID: 14720 RVA: 0x001087AC File Offset: 0x001069AC
		public static void settlement_player_unconscious_continue_on_consequence(MenuCallbackArgs args)
		{
			Settlement currentSettlement = MobileParty.MainParty.CurrentSettlement;
			GameMenu.SwitchToMenu(currentSettlement.IsVillage ? "village" : (currentSettlement.IsCastle ? "castle" : "town"));
		}

		// Token: 0x06003981 RID: 14721 RVA: 0x001087EC File Offset: 0x001069EC
		private static void SetIntroductionText(Settlement settlement, bool fromKeep)
		{
			TextObject textObject = new TextObject("", null);
			if (settlement.IsTown && !fromKeep)
			{
				if (settlement.OwnerClan == Clan.PlayerClan)
				{
					textObject = new TextObject("{=kXVHwjoV}You have arrived at your fief of {SETTLEMENT_LINK}. {PROSPERITY_INFO} {MORALE_INFO}", null);
				}
				else
				{
					textObject = new TextObject("{=UWzQsHA2}{SETTLEMENT_LINK} is governed by {LORD.LINK}, {FACTION_OFFICIAL} of the {FACTION_TERM}. {PROSPERITY_INFO} {MORALE_INFO}", null);
				}
			}
			else if (settlement.IsTown && fromKeep)
			{
				if (settlement.OwnerClan == Clan.PlayerClan)
				{
					textObject = new TextObject("{=u0Dc5g4Z}You are in your town of {SETTLEMENT_LINK}. {KEEP_INFO}", null);
				}
				else
				{
					textObject = new TextObject("{=q3wD0rbq}{SETTLEMENT_LINK} is governed by {LORD.LINK}, {FACTION_OFFICIAL} of the {FACTION_TERM}. {KEEP_INFO}", null);
				}
			}
			else if (settlement.IsCastle)
			{
				if (settlement.OwnerClan == Clan.PlayerClan)
				{
					textObject = new TextObject("{=dA8RGoQ1}You have arrived at {SETTLEMENT_LINK}. {KEEP_INFO}", null);
				}
				else
				{
					textObject = new TextObject("{=4pmvrnmN}The castle of {SETTLEMENT_LINK} is owned by {LORD.LINK}, {FACTION_OFFICIAL} of the {FACTION_TERM}. {KEEP_INFO}", null);
				}
			}
			else if (settlement.IsVillage)
			{
				if (settlement.OwnerClan == Clan.PlayerClan)
				{
					textObject = new TextObject("{=M5iR1e5h}You have arrived at your fief of {SETTLEMENT_LINK}. {PROSPERITY_INFO}", null);
				}
				else
				{
					textObject = new TextObject("{=RVDojUOM}The lands around {SETTLEMENT_LINK} are owned mostly by {LORD.LINK}, {FACTION_OFFICIAL} of the {FACTION_TERM}. {PROSPERITY_INFO}", null);
				}
			}
			settlement.OwnerClan.Leader.SetPropertiesToTextObject(textObject, "LORD");
			string text = settlement.OwnerClan.Leader.MapFaction.Culture.StringId;
			if (settlement.OwnerClan.Leader.IsFemale)
			{
				text += "_f";
			}
			if (settlement.OwnerClan.Leader == Hero.MainHero && !Hero.MainHero.MapFaction.IsKingdomFaction)
			{
				textObject.SetTextVariable("FACTION_TERM", Hero.MainHero.Clan.EncyclopediaLinkWithName);
				textObject.SetTextVariable("FACTION_OFFICIAL", new TextObject("{=hb30yQPN}leader", null));
			}
			else
			{
				textObject.SetTextVariable("FACTION_TERM", settlement.MapFaction.EncyclopediaLinkWithName);
				if (settlement.OwnerClan.MapFaction.IsKingdomFaction && settlement.OwnerClan.Leader == settlement.OwnerClan.Leader.MapFaction.Leader)
				{
					textObject.SetTextVariable("FACTION_OFFICIAL", GameTexts.FindText("str_faction_ruler", text));
				}
				else
				{
					textObject.SetTextVariable("FACTION_OFFICIAL", GameTexts.FindText("str_faction_official", text));
				}
			}
			textObject.SetTextVariable("SETTLEMENT_LINK", settlement.EncyclopediaLinkWithName);
			settlement.SetPropertiesToTextObject(textObject, "SETTLEMENT_OBJECT");
			string text2 = settlement.SettlementComponent.GetProsperityLevel().ToString();
			if ((settlement.IsTown && settlement.Town.InRebelliousState) || (settlement.IsVillage && settlement.Village.Bound.Town.InRebelliousState))
			{
				textObject.SetTextVariable("PROSPERITY_INFO", GameTexts.FindText("str_settlement_rebellion", null));
				textObject.SetTextVariable("MORALE_INFO", "");
			}
			else if (settlement.IsTown)
			{
				textObject.SetTextVariable("PROSPERITY_INFO", GameTexts.FindText("str_town_long_prosperity_1", text2));
				textObject.SetTextVariable("MORALE_INFO", PlayerTownVisitCampaignBehavior.SetTownMoraleText(settlement));
			}
			else if (settlement.IsVillage)
			{
				textObject.SetTextVariable("PROSPERITY_INFO", GameTexts.FindText("str_village_long_prosperity", text2));
			}
			textObject.SetTextVariable("KEEP_INFO", "");
			if (fromKeep && LocationComplex.Current != null)
			{
				if (!LocationComplex.Current.GetLocationWithId("lordshall").GetCharacterList().Any((LocationCharacter x) => x.Character.IsHero))
				{
					textObject.SetTextVariable("KEEP_INFO", "{=OgkSLkFi}There is nobody in the lord's hall.");
				}
			}
			MBTextManager.SetTextVariable("SETTLEMENT_INFO", textObject, false);
		}

		// Token: 0x06003982 RID: 14722 RVA: 0x00108B50 File Offset: 0x00106D50
		private static TextObject SetTownMoraleText(Settlement settlement)
		{
			SettlementComponent.ProsperityLevel prosperityLevel = settlement.SettlementComponent.GetProsperityLevel();
			string text;
			if (settlement.Town.Loyalty < 25f)
			{
				if (prosperityLevel <= SettlementComponent.ProsperityLevel.Low)
				{
					text = "str_settlement_morale_rebellious_adversity";
				}
				else if (prosperityLevel <= SettlementComponent.ProsperityLevel.Mid)
				{
					text = "str_settlement_morale_rebellious_average";
				}
				else
				{
					text = "str_settlement_morale_rebellious_prosperity";
				}
			}
			else if (settlement.Town.Loyalty < 65f)
			{
				if (prosperityLevel <= SettlementComponent.ProsperityLevel.Low)
				{
				}
				if (prosperityLevel <= SettlementComponent.ProsperityLevel.Mid)
				{
					text = "str_settlement_morale_medium_average";
				}
				else
				{
					text = "str_settlement_morale_medium_prosperity";
				}
			}
			else if (prosperityLevel <= SettlementComponent.ProsperityLevel.Low)
			{
				text = "str_settlement_morale_high_adversity";
			}
			else if (prosperityLevel <= SettlementComponent.ProsperityLevel.Mid)
			{
				text = "str_settlement_morale_high_average";
			}
			else
			{
				text = "str_settlement_morale_high_prosperity";
			}
			return GameTexts.FindText(text, null);
		}

		// Token: 0x06003983 RID: 14723 RVA: 0x00108BF6 File Offset: 0x00106DF6
		[GameMenuInitializationHandler("town_guard")]
		[GameMenuInitializationHandler("menu_tournament_withdraw_verify")]
		[GameMenuInitializationHandler("menu_tournament_bet_confirm")]
		[GameMenuInitializationHandler("town_castle_not_enough_bribe")]
		[GameMenuInitializationHandler("settlement_player_unconscious")]
		[GameMenuInitializationHandler("castle")]
		[GameMenuInitializationHandler("town_castle_nobody_inside")]
		[GameMenuInitializationHandler("encounter_interrupted")]
		[GameMenuInitializationHandler("encounter_interrupted_siege_preparations")]
		[GameMenuInitializationHandler("castle_dungeon")]
		[GameMenuInitializationHandler("encounter_interrupted_raid_started")]
		public static void game_menu_town_menu_on_init(MenuCallbackArgs args)
		{
			args.MenuContext.SetBackgroundMeshName(Settlement.CurrentSettlement.SettlementComponent.WaitMeshName);
		}

		// Token: 0x06003984 RID: 14724 RVA: 0x00108C14 File Offset: 0x00106E14
		[GameMenuInitializationHandler("town_arena")]
		public static void game_menu_town_menu_arena_on_init(MenuCallbackArgs args)
		{
			string text = Settlement.CurrentSettlement.Culture.StringId + "_arena";
			args.MenuContext.SetBackgroundMeshName(text);
			args.MenuContext.SetAmbientSound("event:/map/ambient/node/settlements/2d/arena");
		}

		// Token: 0x06003985 RID: 14725 RVA: 0x00108C58 File Offset: 0x00106E58
		[GameMenuInitializationHandler("village_hostile_action")]
		[GameMenuInitializationHandler("force_volunteers_village")]
		[GameMenuInitializationHandler("force_supplies_village")]
		[GameMenuInitializationHandler("raid_village_no_resist_warn_player")]
		[GameMenuInitializationHandler("raid_village_resisted")]
		[GameMenuInitializationHandler("village_loot_no_resist")]
		[GameMenuInitializationHandler("village_take_food_confirm")]
		[GameMenuInitializationHandler("village_press_into_service_confirm")]
		[GameMenuInitializationHandler("menu_press_into_service_success")]
		[GameMenuInitializationHandler("menu_village_take_food_success")]
		public static void game_menu_village_menu_on_init(MenuCallbackArgs args)
		{
			Village village = Settlement.CurrentSettlement.Village;
			args.MenuContext.SetBackgroundMeshName(village.WaitMeshName);
		}

		// Token: 0x06003986 RID: 14726 RVA: 0x00108C84 File Offset: 0x00106E84
		[GameMenuInitializationHandler("town_keep")]
		public static void game_menu_town_menu_keep_on_init(MenuCallbackArgs args)
		{
			string text = Settlement.CurrentSettlement.Culture.StringId + "_keep";
			args.MenuContext.SetBackgroundMeshName(text);
			args.MenuContext.SetPanelSound("event:/ui/panels/settlement_keep");
			args.MenuContext.SetAmbientSound("event:/map/ambient/node/settlements/2d/keep");
		}

		// Token: 0x06003987 RID: 14727 RVA: 0x00108CD7 File Offset: 0x00106ED7
		[GameMenuEventHandler("town", "manage_production", GameMenuEventHandler.EventType.OnConsequence)]
		public static void game_menu_ui_town_manage_town_on_consequence(MenuCallbackArgs args)
		{
			args.MenuContext.OpenTownManagement();
		}

		// Token: 0x06003988 RID: 14728 RVA: 0x00108CE4 File Offset: 0x00106EE4
		[GameMenuEventHandler("town_keep", "manage_production", GameMenuEventHandler.EventType.OnConsequence)]
		public static void game_menu_ui_town_castle_manage_town_on_consequence(MenuCallbackArgs args)
		{
			args.MenuContext.OpenTownManagement();
		}

		// Token: 0x06003989 RID: 14729 RVA: 0x00108CF1 File Offset: 0x00106EF1
		[GameMenuEventHandler("village", "trade", GameMenuEventHandler.EventType.OnConsequence)]
		private static void game_menu_ui_village_buy_good_on_consequence(MenuCallbackArgs args)
		{
			InventoryManager.OpenScreenAsTrade(Settlement.CurrentSettlement.ItemRoster, Settlement.CurrentSettlement.Village, InventoryManager.InventoryCategoryType.None, null);
		}

		// Token: 0x0600398A RID: 14730 RVA: 0x00108D0E File Offset: 0x00106F0E
		[GameMenuEventHandler("village", "manage_production", GameMenuEventHandler.EventType.OnConsequence)]
		private static void game_menu_ui_village_manage_village_on_consequence(MenuCallbackArgs args)
		{
			args.MenuContext.OpenTownManagement();
		}

		// Token: 0x0600398B RID: 14731 RVA: 0x00108D1B File Offset: 0x00106F1B
		[GameMenuEventHandler("village", "recruit_volunteers", GameMenuEventHandler.EventType.OnConsequence)]
		[GameMenuEventHandler("town_backstreet", "recruit_volunteers", GameMenuEventHandler.EventType.OnConsequence)]
		[GameMenuEventHandler("town", "recruit_volunteers", GameMenuEventHandler.EventType.OnConsequence)]
		private static void game_menu_ui_recruit_volunteers_on_consequence(MenuCallbackArgs args)
		{
			args.MenuContext.OpenRecruitVolunteers();
		}

		// Token: 0x0600398C RID: 14732 RVA: 0x00108D28 File Offset: 0x00106F28
		[GameMenuInitializationHandler("prisoner_wait")]
		private static void wait_menu_ui_prisoner_wait_on_init(MenuCallbackArgs args)
		{
			if (Hero.MainHero.IsFemale)
			{
				args.MenuContext.SetBackgroundMeshName("wait_captive_female");
				return;
			}
			args.MenuContext.SetBackgroundMeshName("wait_captive_male");
		}

		// Token: 0x0600398D RID: 14733 RVA: 0x00108D57 File Offset: 0x00106F57
		[GameMenuInitializationHandler("town_backstreet")]
		public static void game_menu_town_menu_backstreet_sound_on_init(MenuCallbackArgs args)
		{
			args.MenuContext.SetAmbientSound("event:/map/ambient/node/settlements/2d/tavern");
		}

		// Token: 0x0600398E RID: 14734 RVA: 0x00108D69 File Offset: 0x00106F69
		[GameMenuInitializationHandler("town_enemy_town_keep")]
		[GameMenuInitializationHandler("town_keep_dungeon")]
		[GameMenuInitializationHandler("town_keep_bribe")]
		public static void game_menu_town_menu_keep_sound_on_init(MenuCallbackArgs args)
		{
			args.MenuContext.SetBackgroundMeshName(Settlement.CurrentSettlement.SettlementComponent.WaitMeshName);
			args.MenuContext.SetAmbientSound("event:/map/ambient/node/settlements/2d/keep");
		}

		// Token: 0x0600398F RID: 14735 RVA: 0x00108D95 File Offset: 0x00106F95
		[GameMenuInitializationHandler("town_wait_menus")]
		[GameMenuInitializationHandler("town_wait")]
		public static void game_menu_town_menu_sound_on_init(MenuCallbackArgs args)
		{
			args.MenuContext.SetAmbientSound("event:/map/ambient/node/settlements/2d/city");
			args.MenuContext.SetBackgroundMeshName(Settlement.CurrentSettlement.SettlementComponent.WaitMeshName);
		}

		// Token: 0x06003990 RID: 14736 RVA: 0x00108DC1 File Offset: 0x00106FC1
		[GameMenuInitializationHandler("town")]
		public static void game_menu_town_menu_enter_sound_on_init(MenuCallbackArgs args)
		{
			args.MenuContext.SetPanelSound("event:/ui/panels/settlement_city");
			args.MenuContext.SetAmbientSound("event:/map/ambient/node/settlements/2d/city");
			args.MenuContext.SetBackgroundMeshName(Settlement.CurrentSettlement.SettlementComponent.WaitMeshName);
		}

		// Token: 0x06003991 RID: 14737 RVA: 0x00108E00 File Offset: 0x00107000
		[GameMenuInitializationHandler("village_wait_menus")]
		public static void game_menu_village_menu_sound_on_init(MenuCallbackArgs args)
		{
			args.MenuContext.SetAmbientSound("event:/map/ambient/node/settlements/2d/village");
			Village village = Settlement.CurrentSettlement.Village;
			args.MenuContext.SetBackgroundMeshName(village.WaitMeshName);
		}

		// Token: 0x06003992 RID: 14738 RVA: 0x00108E3C File Offset: 0x0010703C
		[GameMenuInitializationHandler("village")]
		public static void game_menu_village__enter_menu_sound_on_init(MenuCallbackArgs args)
		{
			args.MenuContext.SetPanelSound("event:/ui/panels/settlement_village");
			args.MenuContext.SetAmbientSound("event:/map/ambient/node/settlements/2d/village");
			Village village = Settlement.CurrentSettlement.Village;
			args.MenuContext.SetBackgroundMeshName(village.WaitMeshName);
		}

		// Token: 0x06003993 RID: 14739 RVA: 0x00108E85 File Offset: 0x00107085
		private void OnSettlementEntered(MobileParty party, Settlement settlement, Hero hero)
		{
			if (party != null && party.IsMainParty)
			{
				settlement.HasVisited = true;
				if (MBRandom.RandomFloat > 0.5f && (settlement.IsVillage || settlement.IsTown))
				{
					this.CheckPerkAndGiveRelation(party, settlement);
				}
			}
		}

		// Token: 0x06003994 RID: 14740 RVA: 0x00108EC0 File Offset: 0x001070C0
		private void CheckPerkAndGiveRelation(MobileParty party, Settlement settlement)
		{
			bool isVillage = settlement.IsVillage;
			bool flag = (isVillage ? party.HasPerk(DefaultPerks.Scouting.WaterDiviner, true) : party.HasPerk(DefaultPerks.Scouting.Pathfinder, true));
			bool flag2 = (isVillage ? (this._lastTimeRelationGivenWaterDiviner.Equals(CampaignTime.Zero) || this._lastTimeRelationGivenWaterDiviner.ElapsedDaysUntilNow >= 1f) : (this._lastTimeRelationGivenPathfinder.Equals(CampaignTime.Zero) || this._lastTimeRelationGivenPathfinder.ElapsedDaysUntilNow >= 1f));
			if (flag && flag2)
			{
				Hero randomElement = settlement.Notables.GetRandomElement<Hero>();
				if (randomElement != null)
				{
					ChangeRelationAction.ApplyRelationChangeBetweenHeroes(party.ActualClan.Leader, randomElement, MathF.Round(isVillage ? DefaultPerks.Scouting.WaterDiviner.SecondaryBonus : DefaultPerks.Scouting.Pathfinder.SecondaryBonus), true);
				}
				if (isVillage)
				{
					this._lastTimeRelationGivenWaterDiviner = CampaignTime.Now;
					return;
				}
				this._lastTimeRelationGivenPathfinder = CampaignTime.Now;
			}
		}

		// Token: 0x06003995 RID: 14741 RVA: 0x00108FA8 File Offset: 0x001071A8
		private void OnSettlementLeft(MobileParty party, Settlement settlement)
		{
			if (party != null && party.IsMainParty)
			{
				Campaign.Current.IsMainHeroDisguised = false;
			}
		}

		// Token: 0x06003996 RID: 14742 RVA: 0x00108FC0 File Offset: 0x001071C0
		private void SwitchToMenuIfThereIsAnInterrupt(string currentMenuId)
		{
			string genericStateMenu = Campaign.Current.Models.EncounterGameMenuModel.GetGenericStateMenu();
			if (genericStateMenu != currentMenuId)
			{
				if (!string.IsNullOrEmpty(genericStateMenu))
				{
					GameMenu.SwitchToMenu(genericStateMenu);
					return;
				}
				GameMenu.ExitToLast();
			}
		}

		// Token: 0x040011C9 RID: 4553
		private CampaignTime _lastTimeRelationGivenPathfinder = CampaignTime.Zero;

		// Token: 0x040011CA RID: 4554
		private CampaignTime _lastTimeRelationGivenWaterDiviner = CampaignTime.Zero;
	}
}
