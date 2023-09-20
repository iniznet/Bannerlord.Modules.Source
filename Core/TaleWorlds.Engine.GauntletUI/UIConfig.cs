using System;
using System.Collections.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.Engine.GauntletUI
{
	// Token: 0x02000007 RID: 7
	public static class UIConfig
	{
		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000043 RID: 67 RVA: 0x000030DC File Offset: 0x000012DC
		// (set) Token: 0x06000044 RID: 68 RVA: 0x000030E3 File Offset: 0x000012E3
		public static bool DoNotUseGeneratedPrefabs { get; set; }

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000045 RID: 69 RVA: 0x000030EB File Offset: 0x000012EB
		// (set) Token: 0x06000046 RID: 70 RVA: 0x000030F2 File Offset: 0x000012F2
		public static bool DebugModeEnabled { get; set; }

		// Token: 0x06000047 RID: 71 RVA: 0x000030FC File Offset: 0x000012FC
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

		// Token: 0x06000048 RID: 72 RVA: 0x00003144 File Offset: 0x00001344
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
