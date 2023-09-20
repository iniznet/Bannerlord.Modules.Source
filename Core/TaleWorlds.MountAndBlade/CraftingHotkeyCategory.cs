using System;
using System.Collections.Generic;
using TaleWorlds.InputSystem;

namespace TaleWorlds.MountAndBlade
{
	public class CraftingHotkeyCategory : GameKeyContext
	{
		public CraftingHotkeyCategory()
			: base("CraftingHotkeyCategory", 108, GameKeyContext.GameKeyContextType.Default)
		{
			this.RegisterHotKeys();
			this.RegisterGameKeys();
			this.RegisterGameAxisKeys();
		}

		private void RegisterHotKeys()
		{
			List<Key> list = new List<Key>
			{
				new Key(InputKey.Space),
				new Key(InputKey.ControllerRLeft)
			};
			base.RegisterHotKey(new HotKey("Confirm", "CraftingHotkeyCategory", list, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
			base.RegisterHotKey(new HotKey("Ascend", "CraftingHotkeyCategory", InputKey.MiddleMouseButton, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
			base.RegisterHotKey(new HotKey("Rotate", "CraftingHotkeyCategory", InputKey.LeftMouseButton, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
			base.RegisterHotKey(new HotKey("Zoom", "CraftingHotkeyCategory", InputKey.RightMouseButton, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
			base.RegisterHotKey(new HotKey("Copy", "CraftingHotkeyCategory", InputKey.C, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
			base.RegisterHotKey(new HotKey("Paste", "CraftingHotkeyCategory", InputKey.V, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
		}

		private void RegisterGameKeys()
		{
			base.RegisterGameKey(new GameKey(55, "ControllerZoomIn", "CraftingHotkeyCategory", InputKey.Invalid, InputKey.ControllerLTrigger, ""), true);
			base.RegisterGameKey(new GameKey(56, "ControllerZoomOut", "CraftingHotkeyCategory", InputKey.Invalid, InputKey.ControllerRTrigger, ""), true);
		}

		private void RegisterGameAxisKeys()
		{
		}

		public const string CategoryId = "CraftingHotkeyCategory";

		public const string Zoom = "Zoom";

		public const string Rotate = "Rotate";

		public const string Ascend = "Ascend";

		public const string ResetCamera = "ResetCamera";

		public const string Copy = "Copy";

		public const string Paste = "Paste";

		public const string Confirm = "Confirm";

		public const string ControllerRotationAxisX = "CameraAxisX";

		public const string ControllerRotationAxisY = "CameraAxisY";

		public const int ControllerZoomIn = 55;

		public const int ControllerZoomOut = 56;
	}
}
