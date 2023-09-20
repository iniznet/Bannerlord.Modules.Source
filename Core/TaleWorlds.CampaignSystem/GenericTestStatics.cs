using System;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Overlay;

namespace TaleWorlds.CampaignSystem
{
	// Token: 0x020000A2 RID: 162
	public static class GenericTestStatics
	{
		// Token: 0x06001182 RID: 4482 RVA: 0x00050384 File Offset: 0x0004E584
		public static void AddGameMenus(CampaignGameStarter gameSystemInitializer)
		{
			gameSystemInitializer.AddGameMenu("generic_test_menu", "{=!}Test Menu", null, GameOverlays.MenuOverlayType.None, GameMenu.MenuFlags.None, null);
			gameSystemInitializer.AddGameMenuOption("generic_test_menu", "bandit_loop_test", "{=!}Friday Loop Test.", null, delegate(MenuCallbackArgs args)
			{
				BanditLoopTestStatic.Init();
				GameMenu.ExitToLast();
			}, false, -1, false, null);
			gameSystemInitializer.AddGameMenuOption("generic_test_menu", "e3_siege_test_menu_leave", "{=!}Normal campaign.", null, delegate(MenuCallbackArgs args)
			{
				GameMenu.ExitToLast();
			}, false, -1, false, null);
		}
	}
}
