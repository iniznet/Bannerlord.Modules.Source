using System;
using System.Collections.Generic;
using TaleWorlds.InputSystem;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200021C RID: 540
	public class MultiplayerHotkeyCategory : GameKeyContext
	{
		// Token: 0x06001DE6 RID: 7654 RVA: 0x0006C4C0 File Offset: 0x0006A6C0
		public MultiplayerHotkeyCategory()
			: base("MultiplayerHotkeyCategory", 108, GameKeyContext.GameKeyContextType.Default)
		{
			this.RegisterHotKeys();
			this.RegisterGameKeys();
			this.RegisterGameAxisKeys();
		}

		// Token: 0x06001DE7 RID: 7655 RVA: 0x0006C4E4 File Offset: 0x0006A6E4
		private void RegisterHotKeys()
		{
			for (int i = 1; i <= 9; i++)
			{
				base.RegisterHotKey(new HotKey("StoreCameraPosition" + i.ToString(), "MultiplayerHotkeyCategory", InputKey.D0 + i, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
			}
			for (int j = 1; j <= 9; j++)
			{
				base.RegisterHotKey(new HotKey("SpectateCameraPosition" + j.ToString(), "MultiplayerHotkeyCategory", InputKey.D0 + j, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
			}
			List<Key> list = new List<Key>
			{
				new Key(InputKey.RightMouseButton),
				new Key(InputKey.ControllerRUp)
			};
			List<Key> list2 = new List<Key>
			{
				new Key(InputKey.LeftMouseButton),
				new Key(InputKey.ControllerRDown)
			};
			List<Key> list3 = new List<Key>
			{
				new Key(InputKey.RightMouseButton),
				new Key(InputKey.ControllerRUp)
			};
			List<Key> list4 = new List<Key>
			{
				new Key(InputKey.F),
				new Key(InputKey.ControllerRLeft)
			};
			base.RegisterHotKey(new HotKey("PerformActionOnCosmeticItem", "MultiplayerHotkeyCategory", list2, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
			base.RegisterHotKey(new HotKey("PreviewCosmeticItem", "MultiplayerHotkeyCategory", list3, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
			base.RegisterHotKey(new HotKey("InspectBadgeProgression", "MultiplayerHotkeyCategory", list, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
			base.RegisterHotKey(new HotKey("ToggleFriendsList", "MultiplayerHotkeyCategory", list4, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
		}

		// Token: 0x06001DE8 RID: 7656 RVA: 0x0006C65E File Offset: 0x0006A85E
		private void RegisterGameKeys()
		{
		}

		// Token: 0x06001DE9 RID: 7657 RVA: 0x0006C660 File Offset: 0x0006A860
		private void RegisterGameAxisKeys()
		{
		}

		// Token: 0x04000ADA RID: 2778
		public const string CategoryId = "MultiplayerHotkeyCategory";

		// Token: 0x04000ADB RID: 2779
		private const string _storeCameraPositionBase = "StoreCameraPosition";

		// Token: 0x04000ADC RID: 2780
		public const string StoreCameraPosition1 = "StoreCameraPosition1";

		// Token: 0x04000ADD RID: 2781
		public const string StoreCameraPosition2 = "StoreCameraPosition2";

		// Token: 0x04000ADE RID: 2782
		public const string StoreCameraPosition3 = "StoreCameraPosition3";

		// Token: 0x04000ADF RID: 2783
		public const string StoreCameraPosition4 = "StoreCameraPosition4";

		// Token: 0x04000AE0 RID: 2784
		public const string StoreCameraPosition5 = "StoreCameraPosition5";

		// Token: 0x04000AE1 RID: 2785
		public const string StoreCameraPosition6 = "StoreCameraPosition6";

		// Token: 0x04000AE2 RID: 2786
		public const string StoreCameraPosition7 = "StoreCameraPosition7";

		// Token: 0x04000AE3 RID: 2787
		public const string StoreCameraPosition8 = "StoreCameraPosition8";

		// Token: 0x04000AE4 RID: 2788
		public const string StoreCameraPosition9 = "StoreCameraPosition9";

		// Token: 0x04000AE5 RID: 2789
		private const string _spectateCameraPositionBase = "SpectateCameraPosition";

		// Token: 0x04000AE6 RID: 2790
		public const string SpectateCameraPosition1 = "SpectateCameraPosition1";

		// Token: 0x04000AE7 RID: 2791
		public const string SpectateCameraPosition2 = "SpectateCameraPosition2";

		// Token: 0x04000AE8 RID: 2792
		public const string SpectateCameraPosition3 = "SpectateCameraPosition3";

		// Token: 0x04000AE9 RID: 2793
		public const string SpectateCameraPosition4 = "SpectateCameraPosition4";

		// Token: 0x04000AEA RID: 2794
		public const string SpectateCameraPosition5 = "SpectateCameraPosition5";

		// Token: 0x04000AEB RID: 2795
		public const string SpectateCameraPosition6 = "SpectateCameraPosition6";

		// Token: 0x04000AEC RID: 2796
		public const string SpectateCameraPosition7 = "SpectateCameraPosition7";

		// Token: 0x04000AED RID: 2797
		public const string SpectateCameraPosition8 = "SpectateCameraPosition8";

		// Token: 0x04000AEE RID: 2798
		public const string SpectateCameraPosition9 = "SpectateCameraPosition9";

		// Token: 0x04000AEF RID: 2799
		public const string InspectBadgeProgression = "InspectBadgeProgression";

		// Token: 0x04000AF0 RID: 2800
		public const string PerformActionOnCosmeticItem = "PerformActionOnCosmeticItem";

		// Token: 0x04000AF1 RID: 2801
		public const string PreviewCosmeticItem = "PreviewCosmeticItem";

		// Token: 0x04000AF2 RID: 2802
		public const string ToggleFriendsList = "ToggleFriendsList";
	}
}
