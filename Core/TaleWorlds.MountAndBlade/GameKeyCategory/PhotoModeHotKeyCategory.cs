using System;
using System.Collections.Generic;
using TaleWorlds.InputSystem;

namespace TaleWorlds.MountAndBlade.GameKeyCategory
{
	public class PhotoModeHotKeyCategory : GameKeyContext
	{
		public PhotoModeHotKeyCategory()
			: base("PhotoModeHotKeyCategory", 108, GameKeyContext.GameKeyContextType.Default)
		{
			this.RegisterHotKeys();
			this.RegisterGameKeys();
			this.RegisterGameAxisKeys();
		}

		private void RegisterHotKeys()
		{
			List<Key> list = new List<Key>
			{
				new Key(InputKey.LeftShift),
				new Key(InputKey.ControllerRTrigger)
			};
			base.RegisterHotKey(new HotKey("FasterCamera", "PhotoModeHotKeyCategory", list, HotKey.Modifiers.None, HotKey.Modifiers.None), true);
		}

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

		private void RegisterGameAxisKeys()
		{
		}

		public const string CategoryId = "PhotoModeHotKeyCategory";

		public const int HideUI = 90;

		public const int CameraRollLeft = 91;

		public const int CameraRollRight = 92;

		public const int ToggleCameraFollowMode = 95;

		public const int TakePicture = 93;

		public const int TakePictureWithAdditionalPasses = 94;

		public const int ToggleMouse = 96;

		public const int ToggleVignette = 97;

		public const int ToggleCharacters = 98;

		public const int Reset = 105;

		public const string FasterCamera = "FasterCamera";
	}
}
