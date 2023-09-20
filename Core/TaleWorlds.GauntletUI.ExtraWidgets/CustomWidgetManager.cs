using System;
using TaleWorlds.Library;

namespace TaleWorlds.GauntletUI.ExtraWidgets
{
	public static class CustomWidgetManager
	{
		public static void Initilize()
		{
			WidgetInfo.Reload();
			Debug.Print("Loading GauntletUI Extra Custom Widgets", 0, Debug.DebugColor.White, 17592186044416UL);
		}
	}
}
