using System;
using System.Linq;
using TaleWorlds.InputSystem;

namespace TaleWorlds.MountAndBlade
{
	public sealed class FaceGenHotkeyCategory : GameKeyContext
	{
		public FaceGenHotkeyCategory()
			: base("FaceGenHotkeyCategory", 108, GameKeyContext.GameKeyContextType.Default)
		{
			this.RegisterHotKeys();
			this.RegisterGameKeys();
			this.RegisterGameAxisKeys();
		}

		private void RegisterHotKeys()
		{
			base.RegisterHotKey(new HotKey("Ascend", "FaceGenHotkeyCategory", InputKey.MiddleMouseButton, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
			base.RegisterHotKey(new HotKey("Rotate", "FaceGenHotkeyCategory", InputKey.LeftMouseButton, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
			base.RegisterHotKey(new HotKey("Zoom", "FaceGenHotkeyCategory", InputKey.RightMouseButton, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
			base.RegisterHotKey(new HotKey("Copy", "FaceGenHotkeyCategory", InputKey.C, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
			base.RegisterHotKey(new HotKey("Paste", "FaceGenHotkeyCategory", InputKey.V, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
		}

		private void RegisterGameKeys()
		{
			base.RegisterGameKey(new GameKey(55, "ControllerZoomIn", "FaceGenHotkeyCategory", InputKey.Invalid, InputKey.ControllerRTrigger, ""), true);
			base.RegisterGameKey(new GameKey(56, "ControllerZoomOut", "FaceGenHotkeyCategory", InputKey.Invalid, InputKey.ControllerLTrigger, ""), true);
		}

		private void RegisterGameAxisKeys()
		{
			GameAxisKey gameAxisKey = GenericGameKeyContext.Current.RegisteredGameAxisKeys.First((GameAxisKey g) => g.Id.Equals("CameraAxisX"));
			GameAxisKey gameAxisKey2 = GenericGameKeyContext.Current.RegisteredGameAxisKeys.First((GameAxisKey g) => g.Id.Equals("CameraAxisY"));
			base.RegisterGameAxisKey(gameAxisKey, true);
			base.RegisterGameAxisKey(gameAxisKey2, true);
		}

		public const string CategoryId = "FaceGenHotkeyCategory";

		public const string Zoom = "Zoom";

		public const string ControllerRotationAxis = "CameraAxisX";

		public const string ControllerCameraUpDownAxis = "CameraAxisY";

		public const string Rotate = "Rotate";

		public const string Ascend = "Ascend";

		public const string Copy = "Copy";

		public const int ControllerZoomIn = 55;

		public const string Paste = "Paste";

		public const int ControllerZoomOut = 56;
	}
}
