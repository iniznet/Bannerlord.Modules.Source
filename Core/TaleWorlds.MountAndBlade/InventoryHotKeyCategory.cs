using System;
using TaleWorlds.InputSystem;

namespace TaleWorlds.MountAndBlade
{
	public class InventoryHotKeyCategory : GameKeyContext
	{
		public InventoryHotKeyCategory()
			: base("InventoryHotKeyCategory", 108, GameKeyContext.GameKeyContextType.Default)
		{
			this.RegisterHotKeys();
			this.RegisterGameKeys();
			this.RegisterGameAxisKeys();
		}

		private void RegisterHotKeys()
		{
			base.RegisterHotKey(new HotKey("SwitchAlternative", "InventoryHotKeyCategory", InputKey.LeftAlt, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
		}

		private void RegisterGameKeys()
		{
		}

		private void RegisterGameAxisKeys()
		{
		}

		public const string CategoryId = "InventoryHotKeyCategory";

		public const string SwitchAlternative = "SwitchAlternative";
	}
}
