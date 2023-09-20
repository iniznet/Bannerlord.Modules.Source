using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	public static class BannerlordCustomWidgetManager
	{
		public static void Initialize()
		{
			WidgetInfo.Reload();
			Debug.Print("Loading GauntletUI Bannerlord Custom Widgets", 0, Debug.DebugColor.White, 17592186044416UL);
		}
	}
}
