using System;
using System.Collections.Generic;
using TaleWorlds.InputSystem;

namespace TaleWorlds.MountAndBlade
{
	public sealed class MultiplayerHotkeyCategory : GameKeyContext
	{
		public MultiplayerHotkeyCategory()
			: base("MultiplayerHotkeyCategory", 108, GameKeyContext.GameKeyContextType.Default)
		{
			this.RegisterHotKeys();
			this.RegisterGameKeys();
			this.RegisterGameAxisKeys();
		}

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

		private void RegisterGameKeys()
		{
		}

		private void RegisterGameAxisKeys()
		{
		}

		public const string CategoryId = "MultiplayerHotkeyCategory";

		private const string _storeCameraPositionBase = "StoreCameraPosition";

		public const string StoreCameraPosition1 = "StoreCameraPosition1";

		public const string StoreCameraPosition2 = "StoreCameraPosition2";

		public const string StoreCameraPosition3 = "StoreCameraPosition3";

		public const string StoreCameraPosition4 = "StoreCameraPosition4";

		public const string StoreCameraPosition5 = "StoreCameraPosition5";

		public const string StoreCameraPosition6 = "StoreCameraPosition6";

		public const string StoreCameraPosition7 = "StoreCameraPosition7";

		public const string StoreCameraPosition8 = "StoreCameraPosition8";

		public const string StoreCameraPosition9 = "StoreCameraPosition9";

		private const string _spectateCameraPositionBase = "SpectateCameraPosition";

		public const string SpectateCameraPosition1 = "SpectateCameraPosition1";

		public const string SpectateCameraPosition2 = "SpectateCameraPosition2";

		public const string SpectateCameraPosition3 = "SpectateCameraPosition3";

		public const string SpectateCameraPosition4 = "SpectateCameraPosition4";

		public const string SpectateCameraPosition5 = "SpectateCameraPosition5";

		public const string SpectateCameraPosition6 = "SpectateCameraPosition6";

		public const string SpectateCameraPosition7 = "SpectateCameraPosition7";

		public const string SpectateCameraPosition8 = "SpectateCameraPosition8";

		public const string SpectateCameraPosition9 = "SpectateCameraPosition9";

		public const string InspectBadgeProgression = "InspectBadgeProgression";

		public const string PerformActionOnCosmeticItem = "PerformActionOnCosmeticItem";

		public const string PreviewCosmeticItem = "PreviewCosmeticItem";

		public const string ToggleFriendsList = "ToggleFriendsList";
	}
}
