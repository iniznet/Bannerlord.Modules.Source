using System;
using System.Collections.Generic;
using TaleWorlds.InputSystem;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200020C RID: 524
	public class BoardGameHotkeyCategory : GameKeyContext
	{
		// Token: 0x06001DAA RID: 7594 RVA: 0x0006AA53 File Offset: 0x00068C53
		public BoardGameHotkeyCategory()
			: base("BoardGameHotkeyCategory", 108, GameKeyContext.GameKeyContextType.Default)
		{
			this.RegisterHotKeys();
			this.RegisterGameKeys();
			this.RegisterGameAxisKeys();
		}

		// Token: 0x06001DAB RID: 7595 RVA: 0x0006AA78 File Offset: 0x00068C78
		private void RegisterHotKeys()
		{
			List<Key> list = new List<Key>
			{
				new Key(InputKey.LeftMouseButton),
				new Key(InputKey.ControllerRDown)
			};
			List<Key> list2 = new List<Key>
			{
				new Key(InputKey.LeftMouseButton),
				new Key(InputKey.ControllerRDown)
			};
			List<Key> list3 = new List<Key>
			{
				new Key(InputKey.LeftMouseButton),
				new Key(InputKey.ControllerRDown)
			};
			List<Key> list4 = new List<Key>
			{
				new Key(InputKey.Space),
				new Key(InputKey.ControllerRBumper)
			};
			base.RegisterHotKey(new HotKey("BoardGamePawnSelect", "BoardGameHotkeyCategory", list, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
			base.RegisterHotKey(new HotKey("BoardGamePawnDeselect", "BoardGameHotkeyCategory", list2, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
			base.RegisterHotKey(new HotKey("BoardGameDragPreview", "BoardGameHotkeyCategory", list3, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
			base.RegisterHotKey(new HotKey("BoardGameRollDice", "BoardGameHotkeyCategory", list4, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
		}

		// Token: 0x06001DAC RID: 7596 RVA: 0x0006AB7E File Offset: 0x00068D7E
		private void RegisterGameKeys()
		{
		}

		// Token: 0x06001DAD RID: 7597 RVA: 0x0006AB80 File Offset: 0x00068D80
		private void RegisterGameAxisKeys()
		{
		}

		// Token: 0x040009B8 RID: 2488
		public const string CategoryId = "BoardGameHotkeyCategory";

		// Token: 0x040009B9 RID: 2489
		public const string BoardGamePawnSelect = "BoardGamePawnSelect";

		// Token: 0x040009BA RID: 2490
		public const string BoardGamePawnDeselect = "BoardGamePawnDeselect";

		// Token: 0x040009BB RID: 2491
		public const string BoardGameDragPreview = "BoardGameDragPreview";

		// Token: 0x040009BC RID: 2492
		public const string BoardGameRollDice = "BoardGameRollDice";
	}
}
