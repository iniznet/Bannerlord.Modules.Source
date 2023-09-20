using System;
using TaleWorlds.InputSystem;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200021B RID: 539
	public class MissionOrderHotkeyCategory : GameKeyContext
	{
		// Token: 0x06001DE2 RID: 7650 RVA: 0x0006C1AC File Offset: 0x0006A3AC
		public MissionOrderHotkeyCategory()
			: base("MissionOrderHotkeyCategory", 108, GameKeyContext.GameKeyContextType.Default)
		{
			this.RegisterHotKeys();
			this.RegisterGameKeys();
			this.RegisterGameAxisKeys();
		}

		// Token: 0x06001DE3 RID: 7651 RVA: 0x0006C1CE File Offset: 0x0006A3CE
		private void RegisterHotKeys()
		{
		}

		// Token: 0x06001DE4 RID: 7652 RVA: 0x0006C1D0 File Offset: 0x0006A3D0
		private void RegisterGameKeys()
		{
			base.RegisterGameKey(new GameKey(67, "ViewOrders", "MissionOrderHotkeyCategory", InputKey.BackSpace, GameKeyMainCategories.OrderMenuCategory), true);
			base.RegisterGameKey(new GameKey(68, "SelectOrder1", "MissionOrderHotkeyCategory", InputKey.F1, InputKey.ControllerRLeft, GameKeyMainCategories.OrderMenuCategory), true);
			base.RegisterGameKey(new GameKey(69, "SelectOrder2", "MissionOrderHotkeyCategory", InputKey.F2, InputKey.ControllerRDown, GameKeyMainCategories.OrderMenuCategory), true);
			base.RegisterGameKey(new GameKey(70, "SelectOrder3", "MissionOrderHotkeyCategory", InputKey.F3, InputKey.ControllerRRight, GameKeyMainCategories.OrderMenuCategory), true);
			base.RegisterGameKey(new GameKey(71, "SelectOrder4", "MissionOrderHotkeyCategory", InputKey.F4, GameKeyMainCategories.OrderMenuCategory), true);
			base.RegisterGameKey(new GameKey(72, "SelectOrder5", "MissionOrderHotkeyCategory", InputKey.F5, GameKeyMainCategories.OrderMenuCategory), true);
			base.RegisterGameKey(new GameKey(73, "SelectOrder6", "MissionOrderHotkeyCategory", InputKey.F6, GameKeyMainCategories.OrderMenuCategory), true);
			base.RegisterGameKey(new GameKey(74, "SelectOrder7", "MissionOrderHotkeyCategory", InputKey.F7, GameKeyMainCategories.OrderMenuCategory), true);
			base.RegisterGameKey(new GameKey(75, "SelectOrder8", "MissionOrderHotkeyCategory", InputKey.F8, GameKeyMainCategories.OrderMenuCategory), true);
			base.RegisterGameKey(new GameKey(76, "SelectOrderReturn", "MissionOrderHotkeyCategory", InputKey.F9, GameKeyMainCategories.OrderMenuCategory), true);
			base.RegisterGameKey(new GameKey(77, "EveryoneHear", "MissionOrderHotkeyCategory", InputKey.D0, GameKeyMainCategories.OrderMenuCategory), true);
			base.RegisterGameKey(new GameKey(78, "Group0Hear", "MissionOrderHotkeyCategory", InputKey.D1, GameKeyMainCategories.OrderMenuCategory), true);
			base.RegisterGameKey(new GameKey(79, "Group1Hear", "MissionOrderHotkeyCategory", InputKey.D2, GameKeyMainCategories.OrderMenuCategory), true);
			base.RegisterGameKey(new GameKey(80, "Group2Hear", "MissionOrderHotkeyCategory", InputKey.D3, GameKeyMainCategories.OrderMenuCategory), true);
			base.RegisterGameKey(new GameKey(81, "Group3Hear", "MissionOrderHotkeyCategory", InputKey.D4, GameKeyMainCategories.OrderMenuCategory), true);
			base.RegisterGameKey(new GameKey(82, "Group4Hear", "MissionOrderHotkeyCategory", InputKey.D5, GameKeyMainCategories.OrderMenuCategory), true);
			base.RegisterGameKey(new GameKey(83, "Group5Hear", "MissionOrderHotkeyCategory", InputKey.D6, GameKeyMainCategories.OrderMenuCategory), true);
			base.RegisterGameKey(new GameKey(84, "Group6Hear", "MissionOrderHotkeyCategory", InputKey.D7, GameKeyMainCategories.OrderMenuCategory), true);
			base.RegisterGameKey(new GameKey(85, "Group7Hear", "MissionOrderHotkeyCategory", InputKey.D8, GameKeyMainCategories.OrderMenuCategory), true);
			base.RegisterGameKey(new GameKey(86, "HoldOrder", "MissionOrderHotkeyCategory", InputKey.Invalid, InputKey.ControllerLBumper, GameKeyMainCategories.OrderMenuCategory), true);
			base.RegisterGameKey(new GameKey(87, "SelectNextGroup", "MissionOrderHotkeyCategory", InputKey.Invalid, InputKey.ControllerLRight, GameKeyMainCategories.OrderMenuCategory), true);
			base.RegisterGameKey(new GameKey(88, "SelectPreviousGroup", "MissionOrderHotkeyCategory", InputKey.Invalid, InputKey.ControllerLLeft, GameKeyMainCategories.OrderMenuCategory), true);
			base.RegisterGameKey(new GameKey(89, "ToggleGroupSelection", "MissionOrderHotkeyCategory", InputKey.Invalid, InputKey.ControllerLDown, GameKeyMainCategories.OrderMenuCategory), true);
		}

		// Token: 0x06001DE5 RID: 7653 RVA: 0x0006C4BE File Offset: 0x0006A6BE
		private void RegisterGameAxisKeys()
		{
		}

		// Token: 0x04000AC2 RID: 2754
		public const string CategoryId = "MissionOrderHotkeyCategory";

		// Token: 0x04000AC3 RID: 2755
		public const int ViewOrders = 67;

		// Token: 0x04000AC4 RID: 2756
		public const int SelectOrder1 = 68;

		// Token: 0x04000AC5 RID: 2757
		public const int SelectOrder2 = 69;

		// Token: 0x04000AC6 RID: 2758
		public const int SelectOrder3 = 70;

		// Token: 0x04000AC7 RID: 2759
		public const int SelectOrder4 = 71;

		// Token: 0x04000AC8 RID: 2760
		public const int SelectOrder5 = 72;

		// Token: 0x04000AC9 RID: 2761
		public const int SelectOrder6 = 73;

		// Token: 0x04000ACA RID: 2762
		public const int SelectOrder7 = 74;

		// Token: 0x04000ACB RID: 2763
		public const int SelectOrder8 = 75;

		// Token: 0x04000ACC RID: 2764
		public const int SelectOrderReturn = 76;

		// Token: 0x04000ACD RID: 2765
		public const int EveryoneHear = 77;

		// Token: 0x04000ACE RID: 2766
		public const int Group0Hear = 78;

		// Token: 0x04000ACF RID: 2767
		public const int Group1Hear = 79;

		// Token: 0x04000AD0 RID: 2768
		public const int Group2Hear = 80;

		// Token: 0x04000AD1 RID: 2769
		public const int Group3Hear = 81;

		// Token: 0x04000AD2 RID: 2770
		public const int Group4Hear = 82;

		// Token: 0x04000AD3 RID: 2771
		public const int Group5Hear = 83;

		// Token: 0x04000AD4 RID: 2772
		public const int Group6Hear = 84;

		// Token: 0x04000AD5 RID: 2773
		public const int Group7Hear = 85;

		// Token: 0x04000AD6 RID: 2774
		public const int HoldOrder = 86;

		// Token: 0x04000AD7 RID: 2775
		public const int SelectNextGroup = 87;

		// Token: 0x04000AD8 RID: 2776
		public const int SelectPreviousGroup = 88;

		// Token: 0x04000AD9 RID: 2777
		public const int ToggleGroupSelection = 89;
	}
}
