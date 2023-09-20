using System;
using System.Collections.Generic;
using TaleWorlds.InputSystem;

namespace TaleWorlds.MountAndBlade
{
	public class ScoreboardHotKeyCategory : GameKeyContext
	{
		public ScoreboardHotKeyCategory()
			: base("ScoreboardHotKeyCategory", 108, GameKeyContext.GameKeyContextType.Default)
		{
			this.RegisterHotKeys();
			this.RegisterGameKeys();
			this.RegisterGameAxisKeys();
		}

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

		private void RegisterGameKeys()
		{
			base.RegisterGameKey(new GameKey(35, "ShowMouse", "ScoreboardHotKeyCategory", InputKey.MiddleMouseButton, InputKey.ControllerLThumb, GameKeyMainCategories.ActionCategory), true);
		}

		private void RegisterGameAxisKeys()
		{
		}

		public const string CategoryId = "ScoreboardHotKeyCategory";

		public const int ShowMouse = 35;

		public const string HoldShow = "HoldShow";

		public const string ToggleFastForward = "ToggleFastForward";

		public const string MenuShowContextMenu = "MenuShowContextMenu";
	}
}
