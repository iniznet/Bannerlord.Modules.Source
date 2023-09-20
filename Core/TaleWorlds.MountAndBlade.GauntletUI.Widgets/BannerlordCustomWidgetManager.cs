using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	// Token: 0x02000006 RID: 6
	public static class BannerlordCustomWidgetManager
	{
		// Token: 0x0600000E RID: 14 RVA: 0x000021AF File Offset: 0x000003AF
		public static void Initialize()
		{
			WidgetInfo.Reload();
			Debug.Print("Loading GauntletUI Bannerlord Custom Widgets", 0, Debug.DebugColor.White, 17592186044416UL);
		}
	}
}
