using System;
using System.Collections.Generic;
using TaleWorlds.InputSystem;

namespace TaleWorlds.MountAndBlade.GameKeyCategory
{
	// Token: 0x02000403 RID: 1027
	public class PhotoModeHotKeyCategory : GameKeyContext
	{
		// Token: 0x0600353F RID: 13631 RVA: 0x000DE1B1 File Offset: 0x000DC3B1
		public PhotoModeHotKeyCategory()
			: base("PhotoModeHotKeyCategory", 108, GameKeyContext.GameKeyContextType.Default)
		{
			this.RegisterHotKeys();
			this.RegisterGameKeys();
			this.RegisterGameAxisKeys();
		}

		// Token: 0x06003540 RID: 13632 RVA: 0x000DE1D4 File Offset: 0x000DC3D4
		private void RegisterHotKeys()
		{
			List<Key> list = new List<Key>
			{
				new Key(InputKey.LeftShift),
				new Key(InputKey.ControllerRTrigger)
			};
			base.RegisterHotKey(new HotKey("FasterCamera", "PhotoModeHotKeyCategory", list, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
		}

		// Token: 0x06003541 RID: 13633 RVA: 0x000DE220 File Offset: 0x000DC420
		private void RegisterGameKeys()
		{
			base.RegisterGameKey(new GameKey(90, "HideUI", "PhotoModeHotKeyCategory", InputKey.H, InputKey.ControllerRUp, GameKeyMainCategories.PhotoModeCategory), true);
			base.RegisterGameKey(new GameKey(91, "CameraRollLeft", "PhotoModeHotKeyCategory", InputKey.Q, InputKey.ControllerLBumper, GameKeyMainCategories.PhotoModeCategory), true);
			base.RegisterGameKey(new GameKey(92, "CameraRollRight", "PhotoModeHotKeyCategory", InputKey.E, InputKey.ControllerRBumper, GameKeyMainCategories.PhotoModeCategory), true);
			base.RegisterGameKey(new GameKey(95, "ToggleCameraFollowMode", "PhotoModeHotKeyCategory", InputKey.V, InputKey.ControllerRLeft, GameKeyMainCategories.PhotoModeCategory), true);
			base.RegisterGameKey(new GameKey(93, "TakePicture", "PhotoModeHotKeyCategory", InputKey.Enter, InputKey.ControllerRDown, GameKeyMainCategories.PhotoModeCategory), true);
			base.RegisterGameKey(new GameKey(94, "TakePictureWithAdditionalPasses", "PhotoModeHotKeyCategory", InputKey.BackSpace, InputKey.ControllerRBumper, GameKeyMainCategories.PhotoModeCategory), true);
			base.RegisterGameKey(new GameKey(96, "ToggleMouse", "PhotoModeHotKeyCategory", InputKey.C, InputKey.ControllerLThumb, GameKeyMainCategories.PhotoModeCategory), true);
			base.RegisterGameKey(new GameKey(97, "ToggleVignette", "PhotoModeHotKeyCategory", InputKey.X, InputKey.ControllerRThumb, GameKeyMainCategories.PhotoModeCategory), true);
			base.RegisterGameKey(new GameKey(98, "ToggleCharacters", "PhotoModeHotKeyCategory", InputKey.B, InputKey.ControllerRRight, GameKeyMainCategories.PhotoModeCategory), true);
			base.RegisterGameKey(new GameKey(105, "Reset", "PhotoModeHotKeyCategory", InputKey.T, InputKey.ControllerLOption, GameKeyMainCategories.PhotoModeCategory), true);
		}

		// Token: 0x06003542 RID: 13634 RVA: 0x000DE395 File Offset: 0x000DC595
		private void RegisterGameAxisKeys()
		{
		}

		// Token: 0x040016D1 RID: 5841
		public const string CategoryId = "PhotoModeHotKeyCategory";

		// Token: 0x040016D2 RID: 5842
		public const int HideUI = 90;

		// Token: 0x040016D3 RID: 5843
		public const int CameraRollLeft = 91;

		// Token: 0x040016D4 RID: 5844
		public const int CameraRollRight = 92;

		// Token: 0x040016D5 RID: 5845
		public const int ToggleCameraFollowMode = 95;

		// Token: 0x040016D6 RID: 5846
		public const int TakePicture = 93;

		// Token: 0x040016D7 RID: 5847
		public const int TakePictureWithAdditionalPasses = 94;

		// Token: 0x040016D8 RID: 5848
		public const int ToggleMouse = 96;

		// Token: 0x040016D9 RID: 5849
		public const int ToggleVignette = 97;

		// Token: 0x040016DA RID: 5850
		public const int ToggleCharacters = 98;

		// Token: 0x040016DB RID: 5851
		public const int Reset = 105;

		// Token: 0x040016DC RID: 5852
		public const string FasterCamera = "FasterCamera";
	}
}
