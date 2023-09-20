using System;
using System.Collections.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.Engine.GauntletUI
{
	public static class UIConfig
	{
		public static bool DoNotUseGeneratedPrefabs { get; set; }

		public static bool DebugModeEnabled { get; set; }

		[CommandLineFunctionality.CommandLineArgumentFunction("set_debug_mode", "ui")]
		public static string SetDebugMode(List<string> args)
		{
			string text = "Format is \"ui.set_debug_mode [1/0]\".";
			if (args.Count != 1)
			{
				return text;
			}
			int num;
			if (int.TryParse(args[0], out num) && (num == 1 || num == 0))
			{
				UIConfig.DebugModeEnabled = num == 1;
				return "Success.";
			}
			return text;
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("use_generated_prefabs", "ui")]
		public static string SetUsingGeneratedPrefabs(List<string> args)
		{
			string text = "Format is \"ui.use_generated_prefabs [1/0].\"";
			if (args.Count != 1)
			{
				return text;
			}
			int num;
			if (int.TryParse(args[0], out num) && (num == 1 || num == 0))
			{
				UIConfig.DoNotUseGeneratedPrefabs = num == 0;
				return "Success.";
			}
			return text;
		}
	}
}
