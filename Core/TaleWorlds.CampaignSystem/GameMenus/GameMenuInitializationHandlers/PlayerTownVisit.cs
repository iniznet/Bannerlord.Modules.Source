using System;
using TaleWorlds.CampaignSystem.Inventory;
using TaleWorlds.CampaignSystem.Settlements;

namespace TaleWorlds.CampaignSystem.GameMenus.GameMenuInitializationHandlers
{
	// Token: 0x020000EB RID: 235
	public class PlayerTownVisit
	{
		// Token: 0x0600142F RID: 5167 RVA: 0x000597E4 File Offset: 0x000579E4
		[GameMenuInitializationHandler("town")]
		[GameMenuInitializationHandler("castle")]
		private static void game_menu_town_on_init(MenuCallbackArgs args)
		{
			Settlement currentSettlement = Settlement.CurrentSettlement;
			args.MenuContext.SetBackgroundMeshName(currentSettlement.Town.WaitMeshName);
		}

		// Token: 0x06001430 RID: 5168 RVA: 0x0005980D File Offset: 0x00057A0D
		[GameMenuInitializationHandler("town_wait")]
		[GameMenuInitializationHandler("town_guard")]
		[GameMenuInitializationHandler("menu_tournament_withdraw_verify")]
		[GameMenuInitializationHandler("menu_tournament_bet_confirm")]
		public static void game_menu_town_menu_on_init(MenuCallbackArgs args)
		{
			args.MenuContext.SetBackgroundMeshName(Settlement.CurrentSettlement.SettlementComponent.WaitMeshName);
		}

		// Token: 0x06001431 RID: 5169 RVA: 0x00059829 File Offset: 0x00057A29
		[GameMenuEventHandler("town", "manage_production", GameMenuEventHandler.EventType.OnConsequence)]
		[GameMenuEventHandler("town", "manage_production_cheat", GameMenuEventHandler.EventType.OnConsequence)]
		public static void game_menu_town_manage_town_on_consequence(MenuCallbackArgs args)
		{
			args.MenuContext.OpenTownManagement();
		}

		// Token: 0x06001432 RID: 5170 RVA: 0x00059836 File Offset: 0x00057A36
		[GameMenuEventHandler("town_keep", "manage_production", GameMenuEventHandler.EventType.OnConsequence)]
		public static void game_menu_town_castle_manage_town_on_consequence(MenuCallbackArgs args)
		{
			args.MenuContext.OpenTownManagement();
		}

		// Token: 0x06001433 RID: 5171 RVA: 0x00059843 File Offset: 0x00057A43
		[GameMenuEventHandler("castle", "manage_production", GameMenuEventHandler.EventType.OnConsequence)]
		public static void game_menu_castle_manage_castle_on_consequence(MenuCallbackArgs args)
		{
			args.MenuContext.OpenTownManagement();
		}

		// Token: 0x06001434 RID: 5172 RVA: 0x00059850 File Offset: 0x00057A50
		[GameMenuEventHandler("tutorial", "mno_go_back_dot", GameMenuEventHandler.EventType.OnConsequence)]
		private static void mno_go_back_dot(MenuCallbackArgs args)
		{
		}

		// Token: 0x06001435 RID: 5173 RVA: 0x00059852 File Offset: 0x00057A52
		[GameMenuEventHandler("village", "buy_goods", GameMenuEventHandler.EventType.OnConsequence)]
		private static void game_menu_village_buy_good_on_consequence(MenuCallbackArgs args)
		{
			InventoryManager.OpenScreenAsTrade(Settlement.CurrentSettlement.ItemRoster, Settlement.CurrentSettlement.Village, InventoryManager.InventoryCategoryType.None, null);
		}

		// Token: 0x06001436 RID: 5174 RVA: 0x0005986F File Offset: 0x00057A6F
		[GameMenuEventHandler("village", "manage_production", GameMenuEventHandler.EventType.OnConsequence)]
		private static void game_menu_village_manage_village_on_consequence(MenuCallbackArgs args)
		{
			args.MenuContext.OpenTownManagement();
		}

		// Token: 0x06001437 RID: 5175 RVA: 0x0005987C File Offset: 0x00057A7C
		[GameMenuEventHandler("village", "recruit_volunteers", GameMenuEventHandler.EventType.OnConsequence)]
		[GameMenuEventHandler("town_backstreet", "recruit_volunteers", GameMenuEventHandler.EventType.OnConsequence)]
		[GameMenuEventHandler("town", "recruit_volunteers", GameMenuEventHandler.EventType.OnConsequence)]
		private static void game_menu_recruit_volunteers_on_consequence(MenuCallbackArgs args)
		{
			args.MenuContext.OpenRecruitVolunteers();
		}
	}
}
