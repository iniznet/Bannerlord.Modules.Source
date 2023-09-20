using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.DedicatedCustomServer.ClientHelper
{
	// Token: 0x02000008 RID: 8
	internal static class ModLogger
	{
		// Token: 0x06000060 RID: 96 RVA: 0x00003025 File Offset: 0x00001225
		public static void Log(string message, int logLevel = 0, Debug.DebugColor color = 4)
		{
			Debug.Print("DCS Client Helper :: " + message, logLevel, color, 17592186044416UL);
		}

		// Token: 0x06000061 RID: 97 RVA: 0x00003042 File Offset: 0x00001242
		public static void Warn(string message)
		{
			ModLogger.Log(message, 0, 9);
		}
	}
}
