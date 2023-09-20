using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.DedicatedCustomServer.ClientHelper
{
	internal static class ModLogger
	{
		public static void Log(string message, int logLevel = 0, Debug.DebugColor color = 4)
		{
			Debug.Print("DCS Client Helper :: " + message, logLevel, color, 17592186044416UL);
		}

		public static void Warn(string message)
		{
			ModLogger.Log(message, 0, 9);
		}
	}
}
