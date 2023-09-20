using System;
using TaleWorlds.InputSystem;

namespace TaleWorlds.MountAndBlade
{
	public sealed class GenericGameKeyContext : GameKeyContext
	{
		public static GenericGameKeyContext Current { get; private set; }

		public GenericGameKeyContext()
			: base("Generic", 108, GameKeyContext.GameKeyContextType.Default)
		{
			GenericGameKeyContext.Current = this;
			this.RegisterHotKeys();
			this.RegisterGameKeys();
			this.RegisterGameAxisKeys();
		}

		private void RegisterHotKeys()
		{
		}

		private void RegisterGameKeys()
		{
			GameKey gameKey = new GameKey(0, "Up", "Generic", InputKey.W, InputKey.ControllerLStickUp, GameKeyMainCategories.ActionCategory);
			GameKey gameKey2 = new GameKey(1, "Down", "Generic", InputKey.S, InputKey.ControllerLStickDown, GameKeyMainCategories.ActionCategory);
			GameKey gameKey3 = new GameKey(2, "Left", "Generic", InputKey.A, InputKey.ControllerLStickLeft, GameKeyMainCategories.ActionCategory);
			GameKey gameKey4 = new GameKey(3, "Right", "Generic", InputKey.D, InputKey.ControllerLStickRight, GameKeyMainCategories.ActionCategory);
			base.RegisterGameKey(gameKey, true);
			base.RegisterGameKey(gameKey2, true);
			base.RegisterGameKey(gameKey3, true);
			base.RegisterGameKey(gameKey4, true);
			base.RegisterGameKey(new GameKey(4, "Leave", "Generic", InputKey.Tab, InputKey.ControllerRRight, GameKeyMainCategories.ActionCategory), true);
			base.RegisterGameKey(new GameKey(5, "ShowIndicators", "Generic", InputKey.LeftAlt, InputKey.ControllerLBumper, GameKeyMainCategories.ActionCategory), true);
			base.RegisterGameAxisKey(new GameAxisKey("MovementAxisX", InputKey.ControllerLStick, gameKey4, gameKey3, GameAxisKey.AxisType.X), true);
			base.RegisterGameAxisKey(new GameAxisKey("MovementAxisY", InputKey.ControllerLStick, gameKey, gameKey2, GameAxisKey.AxisType.Y), true);
			base.RegisterGameAxisKey(new GameAxisKey("CameraAxisX", InputKey.ControllerRStick, null, null, GameAxisKey.AxisType.X), true);
			base.RegisterGameAxisKey(new GameAxisKey("CameraAxisY", InputKey.ControllerRStick, null, null, GameAxisKey.AxisType.Y), true);
		}

		private void RegisterGameAxisKeys()
		{
		}

		public const string CategoryId = "Generic";

		public const int Up = 0;

		public const int Down = 1;

		public const int Right = 3;

		public const int Left = 2;

		public const string MovementAxisX = "MovementAxisX";

		public const string MovementAxisY = "MovementAxisY";

		public const string CameraAxisX = "CameraAxisX";

		public const string CameraAxisY = "CameraAxisY";

		public const int Leave = 4;

		public const int ShowIndicators = 5;
	}
}
