using System;
using System.Collections.Generic;
using TaleWorlds.InputSystem;

namespace TaleWorlds.MountAndBlade
{
	public sealed class BoardGameHotkeyCategory : GameKeyContext
	{
		public BoardGameHotkeyCategory()
			: base("BoardGameHotkeyCategory", 108, GameKeyContext.GameKeyContextType.Default)
		{
			this.RegisterHotKeys();
			this.RegisterGameKeys();
			this.RegisterGameAxisKeys();
		}

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

		private void RegisterGameKeys()
		{
		}

		private void RegisterGameAxisKeys()
		{
		}

		public const string CategoryId = "BoardGameHotkeyCategory";

		public const string BoardGamePawnSelect = "BoardGamePawnSelect";

		public const string BoardGamePawnDeselect = "BoardGamePawnDeselect";

		public const string BoardGameDragPreview = "BoardGameDragPreview";

		public const string BoardGameRollDice = "BoardGameRollDice";
	}
}
