using System;
using TaleWorlds.InputSystem;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200020B RID: 523
	public class ArmyManagementHotkeyCategory : GameKeyContext
	{
		// Token: 0x06001DA8 RID: 7592 RVA: 0x0006AA1E File Offset: 0x00068C1E
		public ArmyManagementHotkeyCategory()
			: base("ArmyManagementHotkeyCategory", 108, GameKeyContext.GameKeyContextType.Default)
		{
			this.RegisterHotKeys();
		}

		// Token: 0x06001DA9 RID: 7593 RVA: 0x0006AA34 File Offset: 0x00068C34
		private void RegisterHotKeys()
		{
			base.RegisterHotKey(new HotKey("RemoveParty", "ArmyManagementHotkeyCategory", InputKey.ControllerRBumper, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
		}

		// Token: 0x040009B6 RID: 2486
		public const string CategoryId = "ArmyManagementHotkeyCategory";

		// Token: 0x040009B7 RID: 2487
		public const string RemoveParty = "RemoveParty";
	}
}
