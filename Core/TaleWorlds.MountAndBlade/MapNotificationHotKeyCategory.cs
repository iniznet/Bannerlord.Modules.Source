using System;
using TaleWorlds.InputSystem;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200021A RID: 538
	public class MapNotificationHotKeyCategory : GameKeyContext
	{
		// Token: 0x06001DE0 RID: 7648 RVA: 0x0006C177 File Offset: 0x0006A377
		public MapNotificationHotKeyCategory()
			: base("MapNotificationHotKeyCategory", 108, GameKeyContext.GameKeyContextType.Default)
		{
			this.RegisterHotKeys();
		}

		// Token: 0x06001DE1 RID: 7649 RVA: 0x0006C18D File Offset: 0x0006A38D
		private void RegisterHotKeys()
		{
			base.RegisterHotKey(new HotKey("RemoveNotification", "MapNotificationHotKeyCategory", InputKey.ControllerRUp, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
		}

		// Token: 0x04000AC0 RID: 2752
		public const string CategoryId = "MapNotificationHotKeyCategory";

		// Token: 0x04000AC1 RID: 2753
		public const string RemoveNotification = "RemoveNotification";
	}
}
