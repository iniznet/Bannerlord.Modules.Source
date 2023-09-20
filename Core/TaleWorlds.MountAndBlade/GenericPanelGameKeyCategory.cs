using System;
using System.Collections.Generic;
using TaleWorlds.InputSystem;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000217 RID: 535
	public class GenericPanelGameKeyCategory : GameKeyContext
	{
		// Token: 0x170005F4 RID: 1524
		// (get) Token: 0x06001DD2 RID: 7634 RVA: 0x0006B9C1 File Offset: 0x00069BC1
		// (set) Token: 0x06001DD3 RID: 7635 RVA: 0x0006B9C8 File Offset: 0x00069BC8
		public static GenericPanelGameKeyCategory Current { get; private set; }

		// Token: 0x06001DD4 RID: 7636 RVA: 0x0006B9D0 File Offset: 0x00069BD0
		public GenericPanelGameKeyCategory(string categoryId = "GenericPanelGameKeyCategory")
			: base(categoryId, 108, GameKeyContext.GameKeyContextType.Default)
		{
			GenericPanelGameKeyCategory.Current = this;
			this.RegisterHotKeys();
			this.RegisterGameKeys();
			this.RegisterGameAxisKeys();
		}

		// Token: 0x06001DD5 RID: 7637 RVA: 0x0006B9F4 File Offset: 0x00069BF4
		private void RegisterHotKeys()
		{
			List<Key> list = new List<Key>
			{
				new Key(InputKey.Escape),
				new Key(InputKey.ControllerRRight)
			};
			List<Key> list2 = new List<Key>
			{
				new Key(InputKey.Enter),
				new Key(InputKey.NumpadEnter),
				new Key(InputKey.ControllerRLeft)
			};
			List<Key> list3 = new List<Key>
			{
				new Key(InputKey.ControllerRUp)
			};
			List<Key> list4 = new List<Key>
			{
				new Key(InputKey.Escape),
				new Key(InputKey.ControllerROption)
			};
			List<Key> list5 = new List<Key>
			{
				new Key(InputKey.Q),
				new Key(InputKey.ControllerLBumper)
			};
			List<Key> list6 = new List<Key>
			{
				new Key(InputKey.E),
				new Key(InputKey.ControllerRBumper)
			};
			List<Key> list7 = new List<Key>
			{
				new Key(InputKey.D),
				new Key(InputKey.ControllerRTrigger)
			};
			List<Key> list8 = new List<Key>
			{
				new Key(InputKey.A),
				new Key(InputKey.ControllerLTrigger)
			};
			List<Key> list9 = new List<Key>
			{
				new Key(InputKey.R),
				new Key(InputKey.ControllerRUp)
			};
			List<Key> list10 = new List<Key>
			{
				new Key(InputKey.ControllerROption)
			};
			List<Key> list11 = new List<Key>
			{
				new Key(InputKey.Delete),
				new Key(InputKey.ControllerRUp)
			};
			List<Key> list12 = new List<Key>
			{
				new Key(InputKey.Escape),
				new Key(InputKey.ControllerRUp)
			};
			List<Key> list13 = new List<Key>
			{
				new Key(InputKey.Enter),
				new Key(InputKey.NumpadEnter),
				new Key(InputKey.ControllerRDown)
			};
			base.RegisterHotKey(new HotKey("Exit", "GenericPanelGameKeyCategory", list, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
			base.RegisterHotKey(new HotKey("Confirm", "GenericPanelGameKeyCategory", list2, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
			base.RegisterHotKey(new HotKey("Reset", "GenericPanelGameKeyCategory", list3, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
			base.RegisterHotKey(new HotKey("ToggleEscapeMenu", "GenericPanelGameKeyCategory", list4, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
			base.RegisterHotKey(new HotKey("SwitchToPreviousTab", "GenericPanelGameKeyCategory", list5, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
			base.RegisterHotKey(new HotKey("SwitchToNextTab", "GenericPanelGameKeyCategory", list6, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
			base.RegisterHotKey(new HotKey("GiveAll", "GenericPanelGameKeyCategory", list7, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
			base.RegisterHotKey(new HotKey("TakeAll", "GenericPanelGameKeyCategory", list8, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
			base.RegisterHotKey(new HotKey("Randomize", "GenericPanelGameKeyCategory", list9, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
			base.RegisterHotKey(new HotKey("Start", "GenericPanelGameKeyCategory", list10, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
			base.RegisterHotKey(new HotKey("Delete", "GenericPanelGameKeyCategory", list11, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
			base.RegisterHotKey(new HotKey("SelectProfile", "GenericPanelGameKeyCategory", list12, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
			base.RegisterHotKey(new HotKey("Play", "GenericPanelGameKeyCategory", list13, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
		}

		// Token: 0x06001DD6 RID: 7638 RVA: 0x0006BD25 File Offset: 0x00069F25
		private void RegisterGameKeys()
		{
		}

		// Token: 0x06001DD7 RID: 7639 RVA: 0x0006BD27 File Offset: 0x00069F27
		private void RegisterGameAxisKeys()
		{
		}

		// Token: 0x04000A93 RID: 2707
		public const string CategoryId = "GenericPanelGameKeyCategory";

		// Token: 0x04000A94 RID: 2708
		public const string Exit = "Exit";

		// Token: 0x04000A95 RID: 2709
		public const string Confirm = "Confirm";

		// Token: 0x04000A96 RID: 2710
		public const string ResetChanges = "Reset";

		// Token: 0x04000A97 RID: 2711
		public const string ToggleEscapeMenu = "ToggleEscapeMenu";

		// Token: 0x04000A98 RID: 2712
		public const string SwitchToPreviousTab = "SwitchToPreviousTab";

		// Token: 0x04000A99 RID: 2713
		public const string SwitchToNextTab = "SwitchToNextTab";

		// Token: 0x04000A9A RID: 2714
		public const string GiveAll = "GiveAll";

		// Token: 0x04000A9B RID: 2715
		public const string TakeAll = "TakeAll";

		// Token: 0x04000A9C RID: 2716
		public const string Randomize = "Randomize";

		// Token: 0x04000A9D RID: 2717
		public const string Start = "Start";

		// Token: 0x04000A9E RID: 2718
		public const string Delete = "Delete";

		// Token: 0x04000A9F RID: 2719
		public const string SelectProfile = "SelectProfile";

		// Token: 0x04000AA0 RID: 2720
		public const string Play = "Play";
	}
}
