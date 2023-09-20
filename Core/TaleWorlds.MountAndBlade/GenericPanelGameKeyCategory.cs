using System;
using System.Collections.Generic;
using TaleWorlds.InputSystem;

namespace TaleWorlds.MountAndBlade
{
	public class GenericPanelGameKeyCategory : GameKeyContext
	{
		public static GenericPanelGameKeyCategory Current { get; private set; }

		public GenericPanelGameKeyCategory(string categoryId = "GenericPanelGameKeyCategory")
			: base(categoryId, 108, GameKeyContext.GameKeyContextType.Default)
		{
			GenericPanelGameKeyCategory.Current = this;
			this.RegisterHotKeys();
			this.RegisterGameKeys();
			this.RegisterGameAxisKeys();
		}

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

		private void RegisterGameKeys()
		{
		}

		private void RegisterGameAxisKeys()
		{
		}

		public const string CategoryId = "GenericPanelGameKeyCategory";

		public const string Exit = "Exit";

		public const string Confirm = "Confirm";

		public const string ResetChanges = "Reset";

		public const string ToggleEscapeMenu = "ToggleEscapeMenu";

		public const string SwitchToPreviousTab = "SwitchToPreviousTab";

		public const string SwitchToNextTab = "SwitchToNextTab";

		public const string GiveAll = "GiveAll";

		public const string TakeAll = "TakeAll";

		public const string Randomize = "Randomize";

		public const string Start = "Start";

		public const string Delete = "Delete";

		public const string SelectProfile = "SelectProfile";

		public const string Play = "Play";
	}
}
