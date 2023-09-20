using System;
using TaleWorlds.InputSystem;

namespace TaleWorlds.MountAndBlade
{
	public sealed class MapNotificationHotKeyCategory : GameKeyContext
	{
		public MapNotificationHotKeyCategory()
			: base("MapNotificationHotKeyCategory", 108, GameKeyContext.GameKeyContextType.Default)
		{
			this.RegisterHotKeys();
		}

		private void RegisterHotKeys()
		{
			base.RegisterHotKey(new HotKey("RemoveNotification", "MapNotificationHotKeyCategory", InputKey.ControllerRUp, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
		}

		public const string CategoryId = "MapNotificationHotKeyCategory";

		public const string RemoveNotification = "RemoveNotification";
	}
}
