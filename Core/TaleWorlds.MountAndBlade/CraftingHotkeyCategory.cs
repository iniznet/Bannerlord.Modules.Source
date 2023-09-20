using System;
using System.Collections.Generic;
using TaleWorlds.InputSystem;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000210 RID: 528
	public class CraftingHotkeyCategory : GameKeyContext
	{
		// Token: 0x06001DBA RID: 7610 RVA: 0x0006B2FB File Offset: 0x000694FB
		public CraftingHotkeyCategory()
			: base("CraftingHotkeyCategory", 108, GameKeyContext.GameKeyContextType.Default)
		{
			this.RegisterHotKeys();
			this.RegisterGameKeys();
			this.RegisterGameAxisKeys();
		}

		// Token: 0x06001DBB RID: 7611 RVA: 0x0006B320 File Offset: 0x00069520
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

		// Token: 0x06001DBC RID: 7612 RVA: 0x0006B3F4 File Offset: 0x000695F4
		private void RegisterGameKeys()
		{
			base.RegisterGameKey(new GameKey(55, "ControllerZoomIn", "CraftingHotkeyCategory", InputKey.Invalid, InputKey.ControllerLTrigger, ""), true);
			base.RegisterGameKey(new GameKey(56, "ControllerZoomOut", "CraftingHotkeyCategory", InputKey.Invalid, InputKey.ControllerRTrigger, ""), true);
		}

		// Token: 0x06001DBD RID: 7613 RVA: 0x0006B447 File Offset: 0x00069647
		private void RegisterGameAxisKeys()
		{
		}

		// Token: 0x040009EF RID: 2543
		public const string CategoryId = "CraftingHotkeyCategory";

		// Token: 0x040009F0 RID: 2544
		public const string Zoom = "Zoom";

		// Token: 0x040009F1 RID: 2545
		public const string Rotate = "Rotate";

		// Token: 0x040009F2 RID: 2546
		public const string Ascend = "Ascend";

		// Token: 0x040009F3 RID: 2547
		public const string ResetCamera = "ResetCamera";

		// Token: 0x040009F4 RID: 2548
		public const string Copy = "Copy";

		// Token: 0x040009F5 RID: 2549
		public const string Paste = "Paste";

		// Token: 0x040009F6 RID: 2550
		public const string Confirm = "Confirm";

		// Token: 0x040009F7 RID: 2551
		public const string ControllerRotationAxisX = "CameraAxisX";

		// Token: 0x040009F8 RID: 2552
		public const string ControllerRotationAxisY = "CameraAxisY";

		// Token: 0x040009F9 RID: 2553
		public const int ControllerZoomIn = 55;

		// Token: 0x040009FA RID: 2554
		public const int ControllerZoomOut = 56;
	}
}
