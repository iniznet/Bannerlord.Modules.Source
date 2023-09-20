using System;
using TaleWorlds.InputSystem;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000218 RID: 536
	public class InventoryHotKeyCategory : GameKeyContext
	{
		// Token: 0x06001DD8 RID: 7640 RVA: 0x0006BD29 File Offset: 0x00069F29
		public InventoryHotKeyCategory()
			: base("InventoryHotKeyCategory", 108, GameKeyContext.GameKeyContextType.Default)
		{
			this.RegisterHotKeys();
			this.RegisterGameKeys();
			this.RegisterGameAxisKeys();
		}

		// Token: 0x06001DD9 RID: 7641 RVA: 0x0006BD4B File Offset: 0x00069F4B
		private void RegisterHotKeys()
		{
			base.RegisterHotKey(new HotKey("SwitchAlternative", "InventoryHotKeyCategory", InputKey.LeftAlt, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
		}

		// Token: 0x06001DDA RID: 7642 RVA: 0x0006BD67 File Offset: 0x00069F67
		private void RegisterGameKeys()
		{
		}

		// Token: 0x06001DDB RID: 7643 RVA: 0x0006BD69 File Offset: 0x00069F69
		private void RegisterGameAxisKeys()
		{
		}

		// Token: 0x04000AA2 RID: 2722
		public const string CategoryId = "InventoryHotKeyCategory";

		// Token: 0x04000AA3 RID: 2723
		public const string SwitchAlternative = "SwitchAlternative";
	}
}
