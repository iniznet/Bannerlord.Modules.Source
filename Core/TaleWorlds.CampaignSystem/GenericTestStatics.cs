using System;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Overlay;

namespace TaleWorlds.CampaignSystem
{
	public static class GenericTestStatics
	{
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
