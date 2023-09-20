using System;
using System.Linq;
using TaleWorlds.InputSystem;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000211 RID: 529
	public class FaceGenHotkeyCategory : GameKeyContext
	{
		// Token: 0x06001DBE RID: 7614 RVA: 0x0006B449 File Offset: 0x00069649
		public FaceGenHotkeyCategory()
			: base("FaceGenHotkeyCategory", 108, GameKeyContext.GameKeyContextType.Default)
		{
			this.RegisterHotKeys();
			this.RegisterGameKeys();
			this.RegisterGameAxisKeys();
		}

		// Token: 0x06001DBF RID: 7615 RVA: 0x0006B46C File Offset: 0x0006966C
		private void RegisterHotKeys()
		{
			base.RegisterHotKey(new HotKey("Ascend", "FaceGenHotkeyCategory", InputKey.MiddleMouseButton, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
			base.RegisterHotKey(new HotKey("Rotate", "FaceGenHotkeyCategory", InputKey.LeftMouseButton, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
			base.RegisterHotKey(new HotKey("Zoom", "FaceGenHotkeyCategory", InputKey.RightMouseButton, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
			base.RegisterHotKey(new HotKey("Copy", "FaceGenHotkeyCategory", InputKey.C, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
			base.RegisterHotKey(new HotKey("Paste", "FaceGenHotkeyCategory", InputKey.V, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
		}

		// Token: 0x06001DC0 RID: 7616 RVA: 0x0006B504 File Offset: 0x00069704
		private void RegisterGameKeys()
		{
			base.RegisterGameKey(new GameKey(55, "ControllerZoomIn", "FaceGenHotkeyCategory", InputKey.Invalid, InputKey.ControllerRTrigger, ""), true);
			base.RegisterGameKey(new GameKey(56, "ControllerZoomOut", "FaceGenHotkeyCategory", InputKey.Invalid, InputKey.ControllerLTrigger, ""), true);
		}

		// Token: 0x06001DC1 RID: 7617 RVA: 0x0006B558 File Offset: 0x00069758
		private void RegisterGameAxisKeys()
		{
			GameAxisKey gameAxisKey = GenericGameKeyContext.Current.RegisteredGameAxisKeys.First((GameAxisKey g) => g.Id.Equals("CameraAxisX"));
			GameAxisKey gameAxisKey2 = GenericGameKeyContext.Current.RegisteredGameAxisKeys.First((GameAxisKey g) => g.Id.Equals("CameraAxisY"));
			base.RegisterGameAxisKey(gameAxisKey, true);
			base.RegisterGameAxisKey(gameAxisKey2, true);
		}

		// Token: 0x040009FB RID: 2555
		public const string CategoryId = "FaceGenHotkeyCategory";

		// Token: 0x040009FC RID: 2556
		public const string Zoom = "Zoom";

		// Token: 0x040009FD RID: 2557
		public const string Rotate = "Rotate";

		// Token: 0x040009FE RID: 2558
		public const string ControllerRotationAxis = "CameraAxisX";

		// Token: 0x040009FF RID: 2559
		public const string ControllerCameraUpDownAxis = "CameraAxisY";

		// Token: 0x04000A00 RID: 2560
		public const string Ascend = "Ascend";

		// Token: 0x04000A01 RID: 2561
		public const string Copy = "Copy";

		// Token: 0x04000A02 RID: 2562
		public const string Paste = "Paste";

		// Token: 0x04000A03 RID: 2563
		public const int ControllerZoomIn = 55;

		// Token: 0x04000A04 RID: 2564
		public const int ControllerZoomOut = 56;
	}
}
