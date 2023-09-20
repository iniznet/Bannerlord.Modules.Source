using System;
using TaleWorlds.InputSystem;

namespace TaleWorlds.MountAndBlade
{
	public sealed class ArmyManagementHotkeyCategory : GameKeyContext
	{
		public ArmyManagementHotkeyCategory()
			: base("ArmyManagementHotkeyCategory", 108, GameKeyContext.GameKeyContextType.Default)
		{
			this.RegisterHotKeys();
		}

		private void RegisterHotKeys()
		{
			base.RegisterHotKey(new HotKey("RemoveParty", "ArmyManagementHotkeyCategory", InputKey.ControllerRBumper, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
		}

		public const string CategoryId = "ArmyManagementHotkeyCategory";

		public const string RemoveParty = "RemoveParty";
	}
}
