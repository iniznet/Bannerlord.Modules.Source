using System;
using System.Collections.Generic;
using TaleWorlds.InputSystem;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200021F RID: 543
	public class ScoreboardHotKeyCategory : GameKeyContext
	{
		// Token: 0x06001DF0 RID: 7664 RVA: 0x0006C8B5 File Offset: 0x0006AAB5
		public ScoreboardHotKeyCategory()
			: base("ScoreboardHotKeyCategory", 108, GameKeyContext.GameKeyContextType.Default)
		{
			this.RegisterHotKeys();
			this.RegisterGameKeys();
			this.RegisterGameAxisKeys();
		}

		// Token: 0x06001DF1 RID: 7665 RVA: 0x0006C8D8 File Offset: 0x0006AAD8
		private void RegisterHotKeys()
		{
			List<Key> list = new List<Key>
			{
				new Key(InputKey.F),
				new Key(InputKey.ControllerRUp)
			};
			base.RegisterHotKey(new HotKey("ToggleFastForward", "ScoreboardHotKeyCategory", list, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
			base.RegisterHotKey(new HotKey("MenuShowContextMenu", "ScoreboardHotKeyCategory", InputKey.RightMouseButton, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
			base.RegisterHotKey(new HotKey("HoldShow", "ScoreboardHotKeyCategory", new List<Key>
			{
				new Key(InputKey.Tab),
				new Key(InputKey.ControllerRRight)
			}, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
		}

		// Token: 0x06001DF2 RID: 7666 RVA: 0x0006C97A File Offset: 0x0006AB7A
		private void RegisterGameKeys()
		{
			base.RegisterGameKey(new GameKey(35, "ShowMouse", "ScoreboardHotKeyCategory", InputKey.MiddleMouseButton, InputKey.ControllerLThumb, GameKeyMainCategories.ActionCategory), true);
		}

		// Token: 0x06001DF3 RID: 7667 RVA: 0x0006C9A3 File Offset: 0x0006ABA3
		private void RegisterGameAxisKeys()
		{
		}

		// Token: 0x04000AFF RID: 2815
		public const string CategoryId = "ScoreboardHotKeyCategory";

		// Token: 0x04000B00 RID: 2816
		public const int ShowMouse = 35;

		// Token: 0x04000B01 RID: 2817
		public const string HoldShow = "HoldShow";

		// Token: 0x04000B02 RID: 2818
		public const string ToggleFastForward = "ToggleFastForward";

		// Token: 0x04000B03 RID: 2819
		public const string MenuShowContextMenu = "MenuShowContextMenu";
	}
}
