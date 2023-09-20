using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.InputSystem;

namespace TaleWorlds.MountAndBlade
{
	public sealed class CraftingHotkeyCategory : GameKeyContext
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
				new Key(InputKey.Escape),
				new Key(InputKey.ControllerRRight)
			};
			List<Key> list2 = new List<Key>
			{
				new Key(InputKey.Space),
				new Key(InputKey.ControllerRLeft)
			};
			List<Key> list3 = new List<Key>
			{
				new Key(InputKey.Q),
				new Key(InputKey.ControllerLBumper)
			};
			List<Key> list4 = new List<Key>
			{
				new Key(InputKey.E),
				new Key(InputKey.ControllerRBumper)
			};
			base.RegisterHotKey(new HotKey("Exit", "CraftingHotkeyCategory", list, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
			base.RegisterHotKey(new HotKey("Confirm", "CraftingHotkeyCategory", list2, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
			base.RegisterHotKey(new HotKey("SwitchToPreviousTab", "CraftingHotkeyCategory", list3, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
			base.RegisterHotKey(new HotKey("SwitchToNextTab", "CraftingHotkeyCategory", list4, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
			base.RegisterHotKey(new HotKey("Ascend", "CraftingHotkeyCategory", InputKey.MiddleMouseButton, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
			base.RegisterHotKey(new HotKey("Rotate", "CraftingHotkeyCategory", InputKey.LeftMouseButton, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
			base.RegisterHotKey(new HotKey("Zoom", "CraftingHotkeyCategory", InputKey.RightMouseButton, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
			base.RegisterHotKey(new HotKey("Copy", "CraftingHotkeyCategory", InputKey.C, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
			base.RegisterHotKey(new HotKey("Paste", "CraftingHotkeyCategory", InputKey.V, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
		}

		private void RegisterGameKeys()
		{
			base.RegisterGameKey(new GameKey(55, "ControllerZoomIn", "CraftingHotkeyCategory", InputKey.Invalid, InputKey.ControllerRTrigger, ""), true);
			base.RegisterGameKey(new GameKey(56, "ControllerZoomOut", "CraftingHotkeyCategory", InputKey.Invalid, InputKey.ControllerLTrigger, ""), true);
		}

		private void RegisterGameAxisKeys()
		{
			GameAxisKey gameAxisKey = GenericGameKeyContext.Current.RegisteredGameAxisKeys.First((GameAxisKey g) => g.Id.Equals("CameraAxisX"));
			GameAxisKey gameAxisKey2 = GenericGameKeyContext.Current.RegisteredGameAxisKeys.First((GameAxisKey g) => g.Id.Equals("CameraAxisY"));
			base.RegisterGameAxisKey(gameAxisKey, true);
			base.RegisterGameAxisKey(gameAxisKey2, true);
		}

		public const string CategoryId = "CraftingHotkeyCategory";

		public const string Zoom = "Zoom";

		public const string Rotate = "Rotate";

		public const string Ascend = "Ascend";

		public const string ResetCamera = "ResetCamera";

		public const string Copy = "Copy";

		public const string Paste = "Paste";

		public const string Exit = "Exit";

		public const string Confirm = "Confirm";

		public const string SwitchToPreviousTab = "SwitchToPreviousTab";

		public const string SwitchToNextTab = "SwitchToNextTab";

		public const string ControllerRotationAxisX = "CameraAxisX";

		public const string ControllerRotationAxisY = "CameraAxisY";

		public const int ControllerZoomIn = 55;

		public const int ControllerZoomOut = 56;
	}
}
