using System;
using TaleWorlds.InputSystem;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000216 RID: 534
	public class GenericGameKeyContext : GameKeyContext
	{
		// Token: 0x170005F3 RID: 1523
		// (get) Token: 0x06001DCC RID: 7628 RVA: 0x0006B83B File Offset: 0x00069A3B
		// (set) Token: 0x06001DCD RID: 7629 RVA: 0x0006B842 File Offset: 0x00069A42
		public static GenericGameKeyContext Current { get; private set; }

		// Token: 0x06001DCE RID: 7630 RVA: 0x0006B84A File Offset: 0x00069A4A
		public GenericGameKeyContext()
			: base("Generic", 108, GameKeyContext.GameKeyContextType.Default)
		{
			GenericGameKeyContext.Current = this;
			this.RegisterHotKeys();
			this.RegisterGameKeys();
			this.RegisterGameAxisKeys();
		}

		// Token: 0x06001DCF RID: 7631 RVA: 0x0006B872 File Offset: 0x00069A72
		private void RegisterHotKeys()
		{
		}

		// Token: 0x06001DD0 RID: 7632 RVA: 0x0006B874 File Offset: 0x00069A74
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

		// Token: 0x06001DD1 RID: 7633 RVA: 0x0006B9BF File Offset: 0x00069BBF
		private void RegisterGameAxisKeys()
		{
		}

		// Token: 0x04000A87 RID: 2695
		public const string CategoryId = "Generic";

		// Token: 0x04000A88 RID: 2696
		public const int Up = 0;

		// Token: 0x04000A89 RID: 2697
		public const int Down = 1;

		// Token: 0x04000A8A RID: 2698
		public const int Right = 3;

		// Token: 0x04000A8B RID: 2699
		public const int Left = 2;

		// Token: 0x04000A8C RID: 2700
		public const string MovementAxisX = "MovementAxisX";

		// Token: 0x04000A8D RID: 2701
		public const string MovementAxisY = "MovementAxisY";

		// Token: 0x04000A8E RID: 2702
		public const string CameraAxisX = "CameraAxisX";

		// Token: 0x04000A8F RID: 2703
		public const string CameraAxisY = "CameraAxisY";

		// Token: 0x04000A90 RID: 2704
		public const int Leave = 4;

		// Token: 0x04000A91 RID: 2705
		public const int ShowIndicators = 5;
	}
}
