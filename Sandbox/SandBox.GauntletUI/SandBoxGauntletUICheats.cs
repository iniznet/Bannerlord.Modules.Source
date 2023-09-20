using System;
using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.CampaignSystem.ViewModelCollection.Inventory;
using TaleWorlds.Library;
using TaleWorlds.ScreenSystem;

namespace SandBox.GauntletUI
{
	public static class SandBoxGauntletUICheats
	{
		[CommandLineFunctionality.CommandLineArgumentFunction("set_inventory_search_enabled", "ui")]
		public static string SetInventorySearchEnabled(List<string> args)
		{
			string text = "Format is \"ui.set_inventory_search_enabled [1/0]\".";
			GauntletInventoryScreen gauntletInventoryScreen;
			if ((gauntletInventoryScreen = ScreenManager.TopScreen as GauntletInventoryScreen) == null)
			{
				return "Inventory screen is not open!";
			}
			if (args.Count != 1)
			{
				return text;
			}
			int num;
			if (int.TryParse(args[0], out num) && (num == 1 || num == 0))
			{
				FieldInfo field = typeof(GauntletInventoryScreen).GetField("_dataSource", BindingFlags.Instance | BindingFlags.NonPublic);
				SPInventoryVM spinventoryVM = (SPInventoryVM)field.GetValue(gauntletInventoryScreen);
				spinventoryVM.IsSearchAvailable = num == 1;
				field.SetValue(gauntletInventoryScreen, spinventoryVM);
				return "Success";
			}
			return text;
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("reload_pieces", "crafting")]
		public static string ReloadCraftingPieces(List<string> strings)
		{
			if (strings.Count != 2)
			{
				return "Usage: crafting.reload_pieces {MODULE_NAME} {XML_NAME}";
			}
			typeof(GauntletCraftingScreen).GetField("_reloadXmlPath", BindingFlags.Static | BindingFlags.NonPublic).SetValue(null, new KeyValuePair<string, string>(strings[0], strings[1]));
			return "Reloading crafting pieces...";
		}
	}
}
